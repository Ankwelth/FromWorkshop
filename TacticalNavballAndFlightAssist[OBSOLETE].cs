/* ============================================================================
 * TACTICAL NAVBALL & FLIGHT ASSIST MAINFRAME (CRUISE + TELEMETRY OS)
 * ============================================================================
 * DESCRIPTION:
 * A unified flagship operating system featuring a 3D Navball HUD with prograde/
 * retrograde tracking, PID cruise control, jump-drive waypoint management, 
 * predictive kinematic target locking, and full ship power/fuel telemetry.
 * It also actively listens for drone targeting data via antenna network.
 *
 * SETUP INSTRUCTIONS:
 * 1. HUD Screens : Name them to contain "HUD LCD" (transparent works best).
 * 2. Nav Screens : Name them to contain "Nav Data LCD".
 * 3. Sys Screens : Name them to contain "Sys Data LCD".
 * 4. Control Seat: Name exactly "Flight Seat".
 * 5. Raycast Cam : Name exactly "Targeting Camera".
 * 6. Waypoints   : Paste GPS text directly into this block's Custom Data.
 * * HOTBAR NAV ARGS: 
 * - NEXT / PREV  : Cycles through active waypoints.
 * - CLEAR        : Wipes all current waypoints and unlocks targets.
 * - DIR:[VECTOR] : Sets flight assist direction (FORWARD, BACKWARD, UP, DOWN)
 * - PING         : Fires camera raycast to lock onto physical targets.
 * - GPS:[Name]   : Saves current location as a waypoint.
 * * HOTBAR CRS ARGS: 
 * - START / STOP : Engages/Disengages cruise control at target speed.
 * - TOGGLE       : Toggles cruise on/off.
 * - +10 / -10    : Increases or decreases target speed.
 * - [Number]     : Sets target speed to specific value (e.g., 50).
 * ============================================================================
 */

const string LCD_NAME = "HUD LCD";
const string SEAT_NAME = "Flight Seat";
const string DATA_LCD_NAME = "Nav Data LCD";
const string SYS_LCD_NAME = "Sys Data LCD";
const string CAMERA_NAME = "Targeting Camera";

const bool MIRROR_TO_COCKPIT = true;
const int COCKPIT_SCREEN_INDEX = 0; 
Color _cockpitBgColor = new Color(0, 0, 0, 255); 

const float HUD_BRIGHTNESS = 0.5f; 
const float HUD_OPACITY    = 1.0f; 

const float PITCH_OFFSET = 0.0f; 
const float YAW_OFFSET   = 0.0f; 

const float FOV_ZOOM = 4.5f; 
const float FIN_LENGTH = 150f; 
const float FIN_THICKNESS = 2f; 
const float CIRCLE_RADIUS = 6f; 

const string IGC_CHANNEL = "FLEET_TARGETING_NET";
IMyBroadcastListener _radioListener;

IMyShipController _seat;
IMyTextSurface _cockpitScreen;

List<IMyThrust> _thrusters = new List<IMyThrust>();
List<IMyJumpDrive> _jumpDrives = new List<IMyJumpDrive>();
List<IMyGasTank> _hydroTanks = new List<IMyGasTank>();
List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();

// Screen Lists for multi-monitor support
List<IMyTextPanel> _hudLcds = new List<IMyTextPanel>();
List<IMyTextPanel> _navLcds = new List<IMyTextPanel>();
List<IMyTextPanel> _sysLcds = new List<IMyTextPanel>();

Color _hudColor;     
Color _retroColor;   
Color _gridColor;   
Color _targetColor;  
Color _trackColor; 

int _tick = 0;
string _lastCustomData = "";
List<Vector3D> _waypoints = new List<Vector3D>();
List<string> _waypointNames = new List<string>();
int _currentIndex = 0;

string _activeDirection = "FORWARD";

// --- PREDICTIVE TRACKING ---
bool _hasTargetLock = false;
Vector3D _lockBasePos = Vector3D.Zero;
Vector3D _lockVelocity = Vector3D.Zero;
string _lockName = "";
long _lockTick = 0;

// --- CRUISE CONTROL ---
const float KP_GAIN = 0.1f; 
float _targetSpeed = 0f;
bool _cruiseEnabled = false;

// --- TELEMETRY ---
double _prevHydroVol = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1; 
    
    _hudColor = ApplyBrightness(0, 255, 255, 255);       
    _retroColor = ApplyBrightness(255, 50, 50, 255);     
    _gridColor = ApplyBrightness(255, 255, 255, 60);     
    _targetColor = ApplyBrightness(255, 200, 0, 255);    
    _trackColor = ApplyBrightness(255, 50, 100, 255); 

    _radioListener = IGC.RegisterBroadcastListener(IGC_CHANNEL);
    RefreshBlocks();
    LoadState();
    
    _prevHydroVol = GetTotalHydroVolume();
}

public void Save()
{
    Storage = _targetSpeed.ToString() + "|" + (_cruiseEnabled ? "1" : "0");
}

void LoadState()
{
    if (!string.IsNullOrWhiteSpace(Storage))
    {
        var parts = Storage.Split('|');
        if (parts.Length >= 2)
        {
            float parsedSpeed;
            if (float.TryParse(parts[0], out parsedSpeed)) _targetSpeed = parsedSpeed;
            _cruiseEnabled = (parts[1] == "1");
        }
    }
}

public void Main(string arg, UpdateType updateSource)
{
    // --- 1. ARGUMENT PARSER (UNIFIED) ---
    if (!string.IsNullOrEmpty(arg))
    {
        string cmd = arg.Trim().ToUpper();
        float parsedFloat;
        
        // Navball Commands
        if (cmd == "CLEAR")
        {
            Me.CustomData = "";
            _lastCustomData = "";
            _waypoints.Clear();
            _waypointNames.Clear();
            _currentIndex = 0;
            _hasTargetLock = false; 
            foreach(var jd in _jumpDrives) jd.JumpDistanceMeters = 0f;
        }
        else if (cmd.StartsWith("DIR:")) _activeDirection = cmd.Substring(4);
        else if (cmd.StartsWith("GPS:"))
        {
            if (_seat != null)
            {
                string name = arg.Substring(4).Trim();
                if (string.IsNullOrEmpty(name)) name = "Mark";
                Vector3D pos = _seat.GetPosition();
                string newGps = $"GPS:{name}:{pos.X:0.00}:{pos.Y:0.00}:{pos.Z:0.00}:#FF75C9F1:";
                if (string.IsNullOrWhiteSpace(Me.CustomData)) Me.CustomData = newGps;
                else Me.CustomData += "\n" + newGps;
                CheckForNewData();
            }
        }
        else if (cmd == "PING")
        {
            var cam = GridTerminalSystem.GetBlockWithName(CAMERA_NAME) as IMyCameraBlock;
            if (cam != null && cam.CanScan(2500))
            {
                cam.EnableRaycast = true;
                MyDetectedEntityInfo hit = cam.Raycast(2500);
                if (!hit.IsEmpty() && hit.HitPosition.HasValue)
                {
                    _hasTargetLock = true;
                    _lockBasePos = hit.HitPosition.Value;
                    _lockVelocity = hit.Velocity;
                    _lockName = $"TRK: {hit.Name}";
                    _lockTick = _tick;
                }
                else _hasTargetLock = false; 
            }
        }
        else if (cmd == "NEXT" && _waypoints.Count > 0)
        {
            _hasTargetLock = false; 
            _currentIndex++;
            if (_currentIndex >= _waypoints.Count) _currentIndex = 0;
            UpdateJumpDrives();
        }
        else if (cmd == "PREV" && _waypoints.Count > 0)
        {
            _hasTargetLock = false; 
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _waypoints.Count - 1;
            UpdateJumpDrives();
        }
        
        // Cruise Control Commands
        else if (cmd == "STOP") DisableCruise();
        else if (cmd == "START" && _targetSpeed > 0f) _cruiseEnabled = true;
        else if (cmd == "TOGGLE")
        {
            if (_cruiseEnabled) DisableCruise();
            else if (_targetSpeed > 0f) _cruiseEnabled = true;
        }
        else if (cmd.StartsWith("+") && float.TryParse(cmd.Substring(1), out parsedFloat)) _targetSpeed += parsedFloat;
        else if (cmd.StartsWith("-") && float.TryParse(cmd.Substring(1), out parsedFloat))
        {
            _targetSpeed = Math.Max(0f, _targetSpeed - parsedFloat);
            if (_targetSpeed <= 0f) DisableCruise();
        }
        else if (float.TryParse(cmd, out parsedFloat))
        {
            _targetSpeed = Math.Max(0f, parsedFloat);
            if (_targetSpeed > 0f) _cruiseEnabled = true;
            else DisableCruise();
        }
    }

    // --- 2. CLOCK & ROUTING ---
    if (_tick % 60 == 0) 
    {
        RefreshBlocks();
        CheckForNewData();
        UpdateNavLCD(); 
        UpdateSysLCD();
    }
    
    if (_seat == null) return;
    
    Vector3D velocity = _seat.GetShipVelocities().LinearVelocity;
    double speed = velocity.Length();

    // --- 3. CRUISE CONTROL PHYSICS LOOP ---
    if (_tick % 10 == 0 && _cruiseEnabled)
    {
        ProcessCruiseControl(speed);
    }

    _tick++;

    // --- 4. HUD RENDERING ---
    foreach (var hud in _hudLcds)
    {
        RenderHUD(hud, _seat, velocity, speed);
    }

    if (MIRROR_TO_COCKPIT && _cockpitScreen != null) 
    {
        RenderHUD(_cockpitScreen, _seat, velocity, speed);
    }
}

void ProcessCruiseControl(double currentSpeed)
{
    if (_seat == null) return;

    double error = _targetSpeed - currentSpeed;
    float thrustCommand = MathHelper.Clamp((float)(error * KP_GAIN), 0f, 1f);

    var requiredFlameDir = Base6Directions.GetOppositeDirection(_seat.Orientation.Forward);

    foreach (var t in _thrusters)
    {
        if (t.Orientation.Forward == requiredFlameDir)
        {
            t.ThrustOverridePercentage = thrustCommand;
        }
    }
}

void DisableCruise()
{
    _cruiseEnabled = false;
    if (_seat == null) return;
    var requiredFlameDir = Base6Directions.GetOppositeDirection(_seat.Orientation.Forward);
    foreach (var t in _thrusters)
    {
        if (t.Orientation.Forward == requiredFlameDir) t.ThrustOverridePercentage = 0f;
    }
}

void RefreshBlocks()
{
    _seat = GridTerminalSystem.GetBlockWithName(SEAT_NAME) as IMyShipController;
    
    _thrusters.Clear();
    GridTerminalSystem.GetBlocksOfType(_thrusters, t => t.CubeGrid == Me.CubeGrid);
    
    _jumpDrives.Clear();
    GridTerminalSystem.GetBlocksOfType(_jumpDrives, j => j.CubeGrid == Me.CubeGrid);
    
    _hydroTanks.Clear();
    GridTerminalSystem.GetBlocksOfType(_hydroTanks, t => t.BlockDefinition.SubtypeId.Contains("Hydrogen") && t.IsSameConstructAs(Me));
    
    _batteries.Clear();
    GridTerminalSystem.GetBlocksOfType(_batteries, b => b.IsSameConstructAs(Me));

    // Multi-LCD Retrieval Logic
    _hudLcds.Clear();
    GridTerminalSystem.GetBlocksOfType(_hudLcds, b => b.CustomName.Contains(LCD_NAME) && b.IsSameConstructAs(Me));

    _navLcds.Clear();
    GridTerminalSystem.GetBlocksOfType(_navLcds, b => b.CustomName.Contains(DATA_LCD_NAME) && b.IsSameConstructAs(Me));

    _sysLcds.Clear();
    GridTerminalSystem.GetBlocksOfType(_sysLcds, b => b.CustomName.Contains(SYS_LCD_NAME) && b.IsSameConstructAs(Me));

    foreach (var hud in _hudLcds)
    {
        hud.ContentType = ContentType.SCRIPT;
    }

    if (MIRROR_TO_COCKPIT && _seat != null && _seat is IMyTextSurfaceProvider)
    {
        _cockpitScreen = ((IMyTextSurfaceProvider)_seat).GetSurface(COCKPIT_SCREEN_INDEX);
        if (_cockpitScreen != null)
        {
            _cockpitScreen.ContentType = ContentType.SCRIPT;
            _cockpitScreen.ScriptBackgroundColor = _cockpitBgColor;
        }
    }
}

void UpdateSysLCD()
{
    if (_sysLcds.Count == 0) return;
    
    string output = "";

    // --- HYDROGEN MATH ---
    if (_hydroTanks.Count > 0)
    {
        double currentVolume = GetTotalHydroVolume();
        double totalCapacity = 0;
        foreach(var t in _hydroTanks) totalCapacity += t.Capacity;
        
        double ratePerSecond = currentVolume - _prevHydroVol; 

        output += "=== HYDROGEN ===\n";
        output += $"Stored: {currentVolume:N0} L / {totalCapacity:N0} L\n";
        output += $"Level: {(currentVolume/totalCapacity)*100:N1}%\n";
        output += $"Flow: {ratePerSecond:N0} L/s\n";

        if (ratePerSecond < -1)
        {
            double secondsToEmpty = currentVolume / Math.Abs(ratePerSecond);
            TimeSpan time = TimeSpan.FromSeconds(secondsToEmpty);
            output += $"Status: DRAINING ({time.Days}d {time.Hours}h {time.Minutes}m)\n\n";
        }
        else if (ratePerSecond > 1)
        {
            double secondsToFull = (totalCapacity - currentVolume) / ratePerSecond;
            TimeSpan time = TimeSpan.FromSeconds(secondsToFull);
            output += $"Status: FILLING ({time.Days}d {time.Hours}h {time.Minutes}m)\n\n";
        }
        else
        {
            output += "Status: STABLE\n\n";
        }

        _prevHydroVol = currentVolume;
    }
    else
    {
        output += "=== HYDROGEN ===\nNo tanks found.\n\n";
    }

    // --- BATTERY MATH ---
    if (_batteries.Count > 0)
    {
        double currentCharge = 0;
        double maxCharge = 0;
        double currentInput = 0;
        double currentOutput = 0;

        foreach(var b in _batteries)
        {
            currentCharge += b.CurrentStoredPower; 
            maxCharge += b.MaxStoredPower;         
            currentInput += b.CurrentInput;        
            currentOutput += b.CurrentOutput;      
        }

        double netPower = currentInput - currentOutput; 

        output += "=== POWER ===\n";
        output += $"Stored: {currentCharge:N2} / {maxCharge:N2} MWh\n";
        output += $"Level: {(currentCharge/maxCharge)*100:N1}%\n";
        output += $"Net Load: {netPower:N2} MW\n";

        if (netPower < -0.01)
        {
            double hoursToEmpty = currentCharge / Math.Abs(netPower);
            TimeSpan time = TimeSpan.FromHours(hoursToEmpty);
            output += $"Status: DRAINING ({time.Days}d {time.Hours}h {time.Minutes}m)\n";
        }
        else if (netPower > 0.01)
        {
            double hoursToFull = (maxCharge - currentCharge) / netPower;
            TimeSpan time = TimeSpan.FromHours(hoursToFull);
            output += $"Status: CHARGING ({time.Days}d {time.Hours}h {time.Minutes}m)\n";
        }
        else
        {
            output += "Status: STABLE\n";
        }
    }
    else
    {
        output += "=== POWER ===\nNo batteries found.\n";
    }

    foreach (var sysLcd in _sysLcds)
    {
        sysLcd.ContentType = ContentType.TEXT_AND_IMAGE;
        sysLcd.WriteText(output);
    }
}

double GetTotalHydroVolume()
{
    double vol = 0;
    foreach(var t in _hydroTanks) vol += t.Capacity * t.FilledRatio;
    return vol;
}

void UpdateNavLCD()
{
    if (_navLcds.Count == 0) return;

    string displayString = "";

    if (_hasTargetLock)
    {
        displayString = $"--- ACTIVE OVERRIDE ---\n\n> KINEMATIC TRACKING\n  TARGET: {_lockName}";
    }
    else if (_waypoints.Count == 0)
    {
        displayString = "--- ACTIVE ROUTE ---\n  NO WAYPOINTS STORED";
    }
    else
    {
        displayString = "--- ACTIVE ROUTE ---\n";
        for (int i = 0; i < _waypoints.Count; i++)
        {
            string marker = (i == _currentIndex) ? "> " : "  ";
            Vector3D refPos = _seat != null ? _seat.GetPosition() : Me.GetPosition();
            string dist = (i == _currentIndex) ? $" ({Vector3D.Distance(refPos, _waypoints[i])/1000:0.0} km)" : "";
            displayString += $"{marker}WPT {i + 1}: {_waypointNames[i]}{dist}\n";
        }
    }

    foreach (var navLcd in _navLcds)
    {
        navLcd.ContentType = ContentType.TEXT_AND_IMAGE;
        navLcd.WriteText(displayString);
    }
}

void RenderHUD(IMyTextSurface targetSurface, IMyCubeBlock refBlock, Vector3D velocity, double speed)
{
    var frame = targetSurface.DrawFrame();
    Vector2 surfaceSize = targetSurface.TextureSize;
    Vector2 center = surfaceSize / 2f;
    float fovScalar = Math.Min(surfaceSize.X, surfaceSize.Y) / FOV_ZOOM; 

    // --- PHYSICS ENGINE: TACTICAL BRAKING ---
    float currentSpeed = (float)speed;
    float physicalMass = _seat.CalculateShipMass().PhysicalMass;
    
    Base6Directions.Direction targetExhaustDir = _seat.Orientation.Forward;
    Vector3D exhaustVectorWorld = _seat.WorldMatrix.Forward; 
    
    switch (_activeDirection)
    {
        case "FORWARD":  targetExhaustDir = _seat.Orientation.Forward; exhaustVectorWorld = _seat.WorldMatrix.Forward; break;
        case "BACKWARD": targetExhaustDir = Base6Directions.GetOppositeDirection(_seat.Orientation.Forward); exhaustVectorWorld = _seat.WorldMatrix.Backward; break;
        case "UP":       targetExhaustDir = _seat.Orientation.Up; exhaustVectorWorld = _seat.WorldMatrix.Up; break;
        case "DOWN":     targetExhaustDir = Base6Directions.GetOppositeDirection(_seat.Orientation.Up); exhaustVectorWorld = _seat.WorldMatrix.Down; break;
        case "LEFT":     targetExhaustDir = _seat.Orientation.Left; exhaustVectorWorld = _seat.WorldMatrix.Left; break;
        case "RIGHT":    targetExhaustDir = Base6Directions.GetOppositeDirection(_seat.Orientation.Left); exhaustVectorWorld = _seat.WorldMatrix.Right; break;
    }

    float totalThrust = 0f;
    foreach(var t in _thrusters)
    {
        if (t.IsWorking && t.Orientation.Forward == targetExhaustDir) totalThrust += t.MaxEffectiveThrust;
    }

    Vector3D thrustAccWorld = -exhaustVectorWorld * (physicalMass > 0 ? (totalThrust / physicalMass) : 0f);
    Vector3D netAccWorld = thrustAccWorld + _seat.GetNaturalGravity();
    
    float netDecel = 0f;
    if (currentSpeed > 0.1f) 
    {
        Vector3D brakeDir = -velocity;
        brakeDir.Normalize();
        netDecel = (float)Vector3D.Dot(netAccWorld, brakeDir);
    }

    float stopDist = 0f;
    bool fatalDrift = false;

    if (netDecel > 0 && currentSpeed > 0.5f) stopDist = (currentSpeed * currentSpeed) / (2 * netDecel);
    else if (netDecel <= 0 && currentSpeed > 0.5f) fatalDrift = true; 

    // --- HUD DRAWING ---
    DrawBoresightGrid(ref frame, surfaceSize, center, fovScalar);
    DrawSprite(ref frame, "SquareSimple", center, new Vector2(2, 20), _hudColor);
    DrawSprite(ref frame, "SquareSimple", center, new Vector2(20, 2), _hudColor);

    // --- TARGET DETERMINATION ---
    Vector3D activeTargetPos = Vector3D.Zero;
    string activeTargetName = "";
    bool renderTarget = false;
    Color activeColor = _targetColor;

    if (_hasTargetLock)
    {
        double secondsElapsed = (_tick - _lockTick) / 60.0;
        activeTargetPos = _lockBasePos + (_lockVelocity * secondsElapsed);
        activeTargetName = _lockName;
        activeColor = _trackColor;
        renderTarget = true;
    }
    else if (_waypoints.Count > 0 && _currentIndex < _waypoints.Count)
    {
        activeTargetPos = _waypoints[_currentIndex];
        activeTargetName = _waypointNames[_currentIndex];
        activeColor = _targetColor;
        renderTarget = true;
    }

    if (renderTarget)
    {
        string etaDisplay = "--:--";
        double distance = Vector3D.Distance(refBlock.GetPosition(), activeTargetPos);
        
        Vector3D closingVel = velocity;
        if (_hasTargetLock) closingVel = velocity - _lockVelocity;
        
        double approachSpeed = closingVel.Length();
        if (approachSpeed > 0.5)
        {
            double seconds = distance / approachSpeed;
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            etaDisplay = t.TotalHours >= 1 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"mm\:ss");
        }
        
        DrawTargetLock(ref frame, refBlock, surfaceSize, center, fovScalar, etaDisplay, stopDist, activeTargetPos, activeTargetName, activeColor);
    }

    if (speed > 1.0)
    {
        Vector3D localVel = Vector3D.TransformNormal(velocity, MatrixD.Transpose(refBlock.WorldMatrix));
        localVel.Normalize();
        Vector3D retroVel = -localVel;

        double pYaw = Math.Atan2(localVel.X, -localVel.Z) + MathHelper.ToRadians(YAW_OFFSET);
        double pPitch = Math.Asin(localVel.Y) + MathHelper.ToRadians(PITCH_OFFSET);
        Vector2 pPos = center + new Vector2((float)pYaw * fovScalar, -(float)pPitch * fovScalar);

        double rYaw = Math.Atan2(retroVel.X, -retroVel.Z) + MathHelper.ToRadians(YAW_OFFSET);
        double rPitch = Math.Asin(retroVel.Y) + MathHelper.ToRadians(PITCH_OFFSET);
        Vector2 rPos = center + new Vector2((float)rYaw * fovScalar, -(float)rPitch * fovScalar);

        DrawPrograde(ref frame, pPos);
        DrawRetrograde(ref frame, rPos);
    }

    // --- LEFT HUD: SPEED & CRUISE ---
    var speedText = MySprite.CreateText($"SPD: {speed:0.0} m/s", "Monospace", _hudColor, 0.6f, TextAlignment.CENTER);
    speedText.Position = center + new Vector2(-120, 80);
    frame.Add(speedText);

    string cruiseStr = _cruiseEnabled ? $"[ CRS: {_targetSpeed:0.0} m/s ]" : "[ CRS: OFF ]";
    Color crsColor = _cruiseEnabled ? _targetColor : _gridColor;
    var cruiseText = MySprite.CreateText(cruiseStr, "Monospace", crsColor, 0.45f, TextAlignment.CENTER);
    cruiseText.Position = center + new Vector2(-120, 105);
    frame.Add(cruiseText);

    DrawFlightAssist(ref frame, center, physicalMass, stopDist, fatalDrift);

    frame.Dispose();
}

void DrawFlightAssist(ref MySpriteDrawFrame frame, Vector2 center, float physicalMass, float stopDist, bool fatalDrift)
{
    string faText = $"[ FLIGHT ASSIST ]\n";
    faText += $"VECTOR: {_activeDirection}\n";
    faText += $"MASS  : {physicalMass/1000000:0.00} Mkg\n";
    
    if (!_seat.DampenersOverride) {
        faText += $"BRK   : DAMPENERS OFF\n";
    } else if (fatalDrift) {
        faText += $"BRK   : GRAV OVERRIDE\n";
    } else {
        string distStr = stopDist > 1000 ? $"{(stopDist/1000):0.00} km" : $"{stopDist:0} m";
        faText += $"BRK   : {distStr}\n";
    }

    var faSprite = MySprite.CreateText(faText, "Monospace", _hudColor, 0.45f, TextAlignment.CENTER);
    faSprite.Position = center + new Vector2(120, 80); 
    frame.Add(faSprite);
}

Color ApplyBrightness(int r, int g, int b, int a)
{
    return new Color((int)(r * HUD_BRIGHTNESS), (int)(g * HUD_BRIGHTNESS), (int)(b * HUD_BRIGHTNESS), (int)(a * HUD_OPACITY));
}

void CheckForNewData()
{
    bool updated = false;
    
    while (_radioListener.HasPendingMessage)
    {
        MyIGCMessage message = _radioListener.AcceptMessage();
        if (message.Data is string)
        {
            string incomingGps = ((string)message.Data).Trim();
            if (!Me.CustomData.StartsWith(incomingGps))
            {
                Me.CustomData = string.IsNullOrWhiteSpace(Me.CustomData) ? incomingGps : incomingGps + "\n" + Me.CustomData;
                _lastCustomData = Me.CustomData; 
                updated = true;
            }
        }
    }

    string currentData = Me.CustomData.Trim();
    if (currentData != _lastCustomData) 
    {
        _lastCustomData = currentData;
        updated = true;
    }

    if (updated)
    {
        _waypoints.Clear();
        _waypointNames.Clear();
        string[] lines = currentData.Split('\n');
        foreach (string line in lines)
        {
            string tLine = line.Trim();
            if (tLine.StartsWith("GPS:"))
            {
                string[] parts = tLine.Split(':');
                if (parts.Length >= 6)
                {
                    double x, y, z;
                    if (double.TryParse(parts[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) && 
                        double.TryParse(parts[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y) && 
                        double.TryParse(parts[4], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out z))
                    {
                        _waypoints.Add(new Vector3D(x, y, z));
                        _waypointNames.Add(parts[1]);
                    }
                }
            }
        }
        if (_waypoints.Count > 0 && _currentIndex >= _waypoints.Count) _currentIndex = 0; 
        UpdateJumpDrives();
    }
}

void UpdateJumpDrives()
{
    if (_waypoints.Count == 0) return;
    if (_jumpDrives.Count > 0)
    {
        Vector3D refPos = _seat != null ? _seat.GetPosition() : Me.GetPosition();
        double dist = Vector3D.Distance(refPos, _waypoints[_currentIndex]);
        foreach(var jd in _jumpDrives) jd.JumpDistanceMeters = (float)dist;
    }
}

void DrawTargetLock(ref MySpriteDrawFrame frame, IMyCubeBlock refBlock, Vector2 surfaceSize, Vector2 center, float fovScalar, string etaDisplay, float stopDist, Vector3D targetPos, string targetName, Color lockColor)
{
    Vector3D dir = targetPos - refBlock.GetPosition();
    double distance = dir.Length();
    dir.Normalize();
    Vector3D localDir = Vector3D.TransformNormal(dir, MatrixD.Transpose(refBlock.WorldMatrix));

    if (localDir.Z < 0) 
    {
        double tYaw = Math.Atan2(localDir.X, -localDir.Z) + MathHelper.ToRadians(YAW_OFFSET);
        double tPitch = Math.Asin(localDir.Y) + MathHelper.ToRadians(PITCH_OFFSET);
        Vector2 tPos = center + new Vector2((float)tYaw * fovScalar, -(float)tPitch * fovScalar);

        DrawSprite(ref frame, "SquareSimple", tPos, new Vector2(2, 10), lockColor);
        DrawSprite(ref frame, "SquareSimple", tPos, new Vector2(10, 2), lockColor);
        DrawSprite(ref frame, "SquareSimple", tPos + new Vector2(-20, 0), new Vector2(2, 40), lockColor); 
        DrawSprite(ref frame, "SquareSimple", tPos + new Vector2(-15, -19), new Vector2(10, 2), lockColor); 
        DrawSprite(ref frame, "SquareSimple", tPos + new Vector2(-15, 19), new Vector2(10, 2), lockColor); 
        DrawSprite(ref frame, "SquareSimple", tPos + new Vector2(20, 0), new Vector2(2, 40), lockColor); 
        DrawSprite(ref frame, "SquareSimple", tPos + new Vector2(15, -19), new Vector2(10, 2), lockColor); 
        DrawSprite(ref frame, "SquareSimple", tPos + new Vector2(15, 19), new Vector2(10, 2), lockColor); 
        
        string distText = distance > 1000 ? $"{(distance / 1000):0.00} km" : $"{distance:0} m";
        string routeText = $"[ {targetName.ToUpper()} ]\n{distText}\nETA: {etaDisplay}";
        
        var label = MySprite.CreateText(routeText, "Monospace", lockColor, 0.45f, TextAlignment.CENTER);
        label.Position = tPos + new Vector2(0, 25);
        frame.Add(label);

        if (stopDist > distance && distance > 100 && _seat.GetShipVelocities().LinearVelocity.Length() > 10)
        {
            var warn = MySprite.CreateText("<<< OVERSHOOT IMMINENT >>>", "Monospace", _retroColor, 0.5f, TextAlignment.CENTER);
            warn.Position = tPos + new Vector2(0, -25);
            frame.Add(warn);
        }
    }
    else
    {
        var warning = MySprite.CreateText($"<<< TARGET BEHIND: {targetName.ToUpper()} >>>", "Monospace", lockColor, 0.6f, TextAlignment.CENTER);
        warning.Position = center + new Vector2(0, -15f);
        frame.Add(warning);
    }
}

void DrawBoresightGrid(ref MySpriteDrawFrame frame, Vector2 surfaceSize, Vector2 center, float fovScalar)
{
    DrawSprite(ref frame, "SquareSimple", center, new Vector2(surfaceSize.X, 2), _gridColor);
    DrawSprite(ref frame, "SquareSimple", center, new Vector2(2, surfaceSize.Y), _gridColor);
    for (int i = 15; i <= 90; i += 15)
    {
        float rad = (float)Math.PI * i / 180f;
        float offset = rad * fovScalar;
        bool isMajor = (i % 45 == 0); 
        Vector2 yawSize = isMajor ? new Vector2(2, 20) : new Vector2(2, 10);
        DrawSprite(ref frame, "SquareSimple", center + new Vector2(offset, 0), yawSize, _gridColor);
        DrawSprite(ref frame, "SquareSimple", center + new Vector2(-offset, 0), yawSize, _gridColor);
        Vector2 pitchSize = isMajor ? new Vector2(20, 2) : new Vector2(10, 2);
        DrawSprite(ref frame, "SquareSimple", center + new Vector2(0, offset), pitchSize, _gridColor);
        DrawSprite(ref frame, "SquareSimple", center + new Vector2(0, -offset), pitchSize, _gridColor);
        if (isMajor)
        {
            var textRight = MySprite.CreateText(i.ToString(), "Monospace", _gridColor, 0.5f, TextAlignment.CENTER);
            textRight.Position = center + new Vector2(offset, 15); frame.Add(textRight);
            var textLeft = MySprite.CreateText(i.ToString(), "Monospace", _gridColor, 0.5f, TextAlignment.CENTER);
            textLeft.Position = center + new Vector2(-offset, 15); frame.Add(textLeft);
            var textDown = MySprite.CreateText(i.ToString(), "Monospace", _gridColor, 0.5f, TextAlignment.LEFT);
            textDown.Position = center + new Vector2(15, offset - 10); frame.Add(textDown);
            var textUp = MySprite.CreateText(i.ToString(), "Monospace", _gridColor, 0.5f, TextAlignment.LEFT);
            textUp.Position = center + new Vector2(15, -offset - 10); frame.Add(textUp);
        }
    }
}

void DrawPrograde(ref MySpriteDrawFrame frame, Vector2 pos)
{
    DrawSprite(ref frame, "Circle", pos, new Vector2(CIRCLE_RADIUS * 2, CIRCLE_RADIUS * 2), _hudColor);
    DrawSprite(ref frame, "Circle", pos, new Vector2(CIRCLE_RADIUS * 2 - 6, CIRCLE_RADIUS * 2 - 6), new Color(0,0,0,0)); 
    float offset = CIRCLE_RADIUS + (FIN_LENGTH / 2f);
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(0, -offset), new Vector2(FIN_THICKNESS, FIN_LENGTH), _hudColor); 
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(0, offset), new Vector2(FIN_THICKNESS, FIN_LENGTH), _hudColor);  
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(-offset, 0), new Vector2(FIN_LENGTH, FIN_THICKNESS), _hudColor); 
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(offset, 0), new Vector2(FIN_LENGTH, FIN_THICKNESS), _hudColor);  
}

void DrawRetrograde(ref MySpriteDrawFrame frame, Vector2 pos)
{
    DrawSprite(ref frame, "Circle", pos, new Vector2(CIRCLE_RADIUS * 2, CIRCLE_RADIUS * 2), _retroColor);
    DrawSprite(ref frame, "Circle", pos, new Vector2(CIRCLE_RADIUS * 2 - 6, CIRCLE_RADIUS * 2 - 6), new Color(0,0,0,0)); 
    DrawSprite(ref frame, "SquareSimple", pos, new Vector2(3, 22), _retroColor, 0.785f); 
    DrawSprite(ref frame, "SquareSimple", pos, new Vector2(3, 22), _retroColor, -0.785f);
    float offset = CIRCLE_RADIUS + (FIN_LENGTH / 2f);
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(0, -offset), new Vector2(FIN_THICKNESS, FIN_LENGTH), _retroColor); 
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(0, offset), new Vector2(FIN_THICKNESS, FIN_LENGTH), _retroColor);  
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(-offset, 0), new Vector2(FIN_LENGTH, FIN_THICKNESS), _retroColor); 
    DrawSprite(ref frame, "SquareSimple", pos + new Vector2(offset, 0), new Vector2(FIN_LENGTH, FIN_THICKNESS), _retroColor);  
}

void DrawSprite(ref MySpriteDrawFrame frame, string type, Vector2 pos, Vector2 size, Color color, float rotation = 0f)
{
    var sprite = new MySprite(SpriteType.TEXTURE, type, size: size, color: color);
    sprite.Position = pos;
    sprite.RotationOrScale = rotation;
    frame.Add(sprite);
}