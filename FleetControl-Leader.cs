/* ============================================================================
 * UNIFIED FLIGHT OS: FLEET CONTROL - LEADER (REV 48.5)
 * ============================================================================
 * COMMAND DIRECTORY:
 * - FLEET:START                  : Initialize network and enter STANDBY mode.
 * - FLEET:FORM / FLEET:UNDOCK    : Release hardpoints; enter active FORMATION FLIGHT.
 * - FLEET:DOCK                   : Trigger ESCORT carrier docking (Followers remain airborne).
 * - FLEET:SYNC / FLEET:RECONNECT : Force all nodes to reload config and enter STANDBY.
 * - FLEET:STOP                   : Halt broadcast state; hold current position.
 * - FLEET:STAND                  : Suspend script flight overrides across all nodes.
 * - FLEET:ABORT                  : Reset thruster/gyro overrides and landing routines.
 * - FLEET:REG_ALL                : Spatial Formation Registration (Hovering in flight).
 * - FLEET:REG_DOCK               : Carrier Hardpoint Registration (Escorts only).
 * - FLEET:LAUNCH[:Altitude]      : Release hardpoints and initiate vertical ascent.
 * - FLEET:LAND                   : Execute braking descent and auto-park (Gear AutoLock active).
 * 
 * - FLEET:COMBAT:[GRP]:[SUB]:[PAT] : Targeted combat control directive.
 *   |
 *   |-- [GRP] Target Group (Which units engage combat):
 *   |   - ALL                    : Command all connected fleet nodes to engage.
 *   |   - ESCORT / ESCORTS       : Command only Tier-3 Escort sub-nodes to engage.
 *   |
 *   |-- [SUB] Subsystem Target (AI block focus priority):
 *   |   - WEAPON / WEAPONS       : Focus target weapons and turrets.
 *   |   - PROPULSION / THRUST    : Focus target thrusters and engines.
 *   |   - POWER / BATTERY        : Focus target reactors and batteries.
 *   |   - DEFAULT                : Center-of-mass / default target focus.
 *   |
 *   |-- [PAT] Attack Pattern (AI flight behavior pattern):
 *   |   - RANGE                  : Maintain standoff weapon range.
 *   |   - HIT / RUN              : Perform high-speed strafing passes.
 *   |   - INTERCEPT / RAM        : Direct intercept vector.
 *   |   - CIRCLE                 : Standard orbiting pattern (Default).
 *   |
 *   |-- SYNTAX EXAMPLES:
 *       - FLEET:COMBAT                     : Engage ALL units (Default targets/pattern).
 *       - FLEET:COMBAT:ESCORT:WEAPON:HIT   : ESCORTS target WEAPONS using HIT & RUN.
 *       - FLEET:COMBAT:ALL:POWER:RANGE     : ALL units target POWER from STANDOFF RANGE.
 *       - FLEET:COMBAT:ALL:THRUST:INTERCEPT: ALL units target PROPULSION via INTERCEPT.
 * ============================================================================
 */
const string REMOTE_CONTROL_NAME = "Remote Control"; 
const string LCD_NAME = "Mothership LCD";
const string IGC_CHANNEL_TAG = "FLEET_SYNC"; 
const string IGC_UPLINK_TAG = "DRONE_REPORT";
const float LAUNCH_ASCENT_SPEED = 20f;
const double LANDING_DESCENT_SPEED = -5.0; 

struct DroneTelemetry
{
    public string Name;
    public Vector3D Position;
    public float HydroPct;
    public float PowerPct;
    public float HydroTimeSec;
    public float PowerTimeSec;
    public int AgeTicks;
}

IMyShipController _mothershipRC;
IMyTextPanel _fleetLCD;
IMyBroadcastListener _droneListener;
bool _isInitialized = false;
bool _isActive = false;
int _commandState = 1; 

double _launchAltitudeTarget = 1000.0;
Vector3D _launchStartPos = Vector3D.Zero;
List<IMyThrust> _thrustersUp = new List<IMyThrust>();
List<IMyThrust> _thrustersDown = new List<IMyThrust>();
List<IMyGyro> _gyros = new List<IMyGyro>();
List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
List<IMyGasTank> _hydroTanks = new List<IMyGasTank>();
List<IMyBatteryBlock> _batteries = new List<IMyBatteryBlock>();

int _landingTicks = 0;
bool _mothershipGrounded = false; 
double _prevHydroVol = 0;

Dictionary<long, DroneTelemetry> _fleetRegistry = new Dictionary<long, DroneTelemetry>();
List<long> _purgeBuffer = new List<long>();
System.Text.StringBuilder _stringBuffer = new System.Text.StringBuilder(1024);

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    _droneListener = IGC.RegisterBroadcastListener(IGC_UPLINK_TAG);
    _droneListener.SetMessageCallback(IGC_UPLINK_TAG);
    CacheSystems();
    if (_hydroTanks.Count > 0) _prevHydroVol = GetTotalHydroVolume();
}

public void Main(string argument, UpdateType updateSource)
{
    CacheSystems();

    if ((updateSource & UpdateType.IGC) != 0)
    {
        while (_droneListener.HasPendingMessage)
        {
            MyIGCMessage msg = _droneListener.AcceptMessage();
            if (msg.Data is MyTuple<long, string, Vector3D, Vector4>)
            {
                var packet = (MyTuple<long, string, Vector3D, Vector4>)msg.Data;
                long droneId = packet.Item1;
                Vector4 stats = packet.Item4;
                
                _fleetRegistry[droneId] = new DroneTelemetry 
                { 
                    Name = packet.Item2, 
                    Position = packet.Item3, 
                    HydroPct = stats.X,
                    PowerPct = stats.Y,
                    HydroTimeSec = stats.Z,
                    PowerTimeSec = stats.W,
                    AgeTicks = 0 
                };
            }
        }
    }

    if ((updateSource & UpdateType.Update1) != 0)
    {
        _purgeBuffer.Clear();
        var keys = new List<long>(_fleetRegistry.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            long id = keys[i];
            DroneTelemetry data = _fleetRegistry[id];
            data.AgeTicks++;
            if (data.AgeTicks > 60) _purgeBuffer.Add(id);
            else _fleetRegistry[id] = data;
        }
        for (int i = 0; i < _purgeBuffer.Count; i++) _fleetRegistry.Remove(_purgeBuffer[i]);
    }

    if (!string.IsNullOrEmpty(argument) && argument != IGC_UPLINK_TAG)
    {
        ParseTerminalCommands(argument);
    }

    UpdateTelemetryDisplay();

    if (!_isInitialized || !_isActive || _mothershipRC == null)
    {
        Echo("[STATUS: IDLE OR CONTROLLER MISSING - BROADCAST PAUSED]");
        return;
    }

    Vector3D currentVelocity = _mothershipRC.GetShipVelocities().LinearVelocity;

    if (_commandState == 4)
    {
        ExecuteLaunchAutomation(currentVelocity);
    }
    else if (_commandState == 5)
    {
        ExecuteLandingAutomation(currentVelocity);
    }

    Vector3D angularVelocity = _mothershipRC.GetShipVelocities().AngularVelocity;
    var payload = new MyTuple<MatrixD, Vector3D, Vector3D, int>(_mothershipRC.WorldMatrix, currentVelocity, angularVelocity, _commandState);
    IGC.SendBroadcastMessage(IGC_CHANNEL_TAG, payload);
}

void ExecuteLaunchAutomation(Vector3D velocity)
{
    if (_mothershipRC == null) return;
    Vector3D currentPos = _mothershipRC.GetPosition();
    Vector3D gravityVector = _mothershipRC.GetNaturalGravity();
    Vector3D upVector = (gravityVector.LengthSquared() > 0) ? -Vector3D.Normalize(gravityVector) : _mothershipRC.WorldMatrix.Up;

    double distanceAscended = Vector3D.Dot(currentPos - _launchStartPos, upVector);

    if (distanceAscended >= _launchAltitudeTarget)
    {
        ResetFlightOverrides();
        _commandState = 0; 
        return;
    }

    Vector3D alignmentAxis = Vector3D.Cross(_mothershipRC.WorldMatrix.Down, -upVector);
    Vector3D gyroTarget = alignmentAxis * 3.0;
    for (int i = 0; i < _gyros.Count; i++)
    {
        Vector3D localRotation = Vector3D.TransformNormal(gyroTarget, MatrixD.Transpose(_gyros[i].WorldMatrix));
        _gyros[i].Pitch = (float)-localRotation.X; _gyros[i].Yaw = (float)-localRotation.Y; _gyros[i].Roll = (float)-localRotation.Z;
        _gyros[i].GyroOverride = true;
    }

    double currentVerticalSpeed = Vector3D.Dot(velocity, upVector);
    double speedError = LAUNCH_ASCENT_SPEED - currentVerticalSpeed;
    double gridMass = _mothershipRC.CalculateShipMass().PhysicalMass;

    Vector3D requiredForceWorld = (upVector * (speedError * gridMass * 2.5)) - (gravityVector * gridMass);
    ComputeThrustMatrix(requiredForceWorld);
}

void ExecuteLandingAutomation(Vector3D velocity)
{
    if (_mothershipRC == null) return;
    Vector3D gravityVector = _mothershipRC.GetNaturalGravity();
    if (gravityVector.LengthSquared() == 0) { ResetFlightOverrides(); _commandState = 0; _mothershipGrounded = false; return; }
    Vector3D upVector = -Vector3D.Normalize(gravityVector);

    bool landingGearsLocked = false;
    for (int i = 0; i < _landingGears.Count; i++) 
    {
        _landingGears[i].AutoLock = true;
        if (_landingGears[i].IsLocked) landingGearsLocked = true;
    }

    if (_mothershipGrounded)
    {
        if (!landingGearsLocked && !_mothershipRC.HandBrake)
        {
            ResetFlightOverrides();
            _commandState = 1; 
            _mothershipGrounded = false;
            return;
        }

        ResetFlightOverrides();
        _mothershipRC.HandBrake = true;
        return;
    }

    double surfAlt = 0;
    bool hasSurf = _mothershipRC.TryGetPlanetElevation(MyPlanetElevation.Surface, out surfAlt);
    double targetDescentSpeed = LANDING_DESCENT_SPEED;

    if (hasSurf)
    {
        if (surfAlt > 150.0)
        {
            float gridMass = (float)_mothershipRC.CalculateShipMass().PhysicalMass;
            float maxLiftThrust = 0f;
            for (int i = 0; i < _thrustersUp.Count; i++) if (_thrustersUp[i].IsFunctional) maxLiftThrust += _thrustersUp[i].MaxEffectiveThrust;
            double accelPotential = (gridMass > 0) ? (maxLiftThrust / gridMass) : 0;
            double netDecel = accelPotential - gravityVector.Length();

            if (netDecel > 0.1)
            {
                double distToStop = surfAlt - 75.0;
                double maxSafeSpeed = Math.Sqrt(2.0 * netDecel * distToStop);
                targetDescentSpeed = -(double)MathHelper.Clamp((float)(maxSafeSpeed * 0.85), 5.0f, 500.0f);
            }
            else
            {
                targetDescentSpeed = -500.0;
            }
        }
        else
        {
            targetDescentSpeed = -Math.Max(0.5, surfAlt * 0.1);
        }
    }

    double currentVerticalSpeed = Vector3Dot(velocity, upVector);

    if (Math.Abs(currentVerticalSpeed) < 0.05 && hasSurf && surfAlt < 10.0)
    {
        _landingTicks++;
    }
    else
    {
        _landingTicks = 0;
    }

    if (landingGearsLocked || (_landingTicks > 30))
    {
        ResetFlightOverrides(); 
        _mothershipRC.HandBrake = true; 
        _mothershipGrounded = true;
        return;
    }

    Vector3D alignmentAxis = Vector3D.Cross(_mothershipRC.WorldMatrix.Down, gravityVector);
    Vector3D gyroTarget = alignmentAxis * 3.0;
    for (int i = 0; i < _gyros.Count; i++)
    {
        Vector3D localRotation = Vector3D.TransformNormal(gyroTarget, MatrixD.Transpose(_gyros[i].WorldMatrix));
        _gyros[i].Pitch = (float)-localRotation.X; _gyros[i].Yaw = (float)-localRotation.Y; _gyros[i].Roll = (float)-localRotation.Z;
        _gyros[i].GyroOverride = true;
    }

    double speedError = targetDescentSpeed - currentVerticalSpeed; 
    double gridMassVal = _mothershipRC.CalculateShipMass().PhysicalMass;

    Vector3D verticalAcc = upVector * (speedError * 2.5);
    Vector3D requiredForceWorld = (verticalAcc * gridMassVal) - (gravityVector * gridMassVal);
    ComputeThrustMatrix(requiredForceWorld);
}

void ComputeThrustMatrix(Vector3D forceVectorWorld)
{
    if (_mothershipRC == null) return;
    MatrixD transposeMatrix = MatrixD.Transpose(_mothershipRC.WorldMatrix);
    Vector3D localForce = Vector3D.TransformNormal(forceVectorWorld, transposeMatrix);
    ScaleThrustOutputs(_thrustersUp, _thrustersDown, localForce.Y);
}

void ScaleThrustOutputs(List<IMyThrust> positiveGroup, List<IMyThrust> negativeGroup, double targetForce)
{
    List<IMyThrust> activeGroup = (targetForce > 0) ? positiveGroup : negativeGroup;
    List<IMyThrust> inactiveGroup = (targetForce > 0) ? negativeGroup : positiveGroup;
    double absoluteForce = Math.Abs(targetForce);
    float totalMaxThrust = 0f;
    for (int i = 0; i < activeGroup.Count; i++) if (activeGroup[i].IsWorking) totalMaxThrust += activeGroup[i].MaxEffectiveThrust;
    float overridePercentage = (totalMaxThrust > 0) ? MathHelper.Clamp((float)(absoluteForce / totalMaxThrust), 0.000001f, 1.0f) : 0f;
    for (int i = 0; i < activeGroup.Count; i++) activeGroup[i].ThrustOverridePercentage = overridePercentage;
    for (int i = 0; i < inactiveGroup.Count; i++) inactiveGroup[i].ThrustOverridePercentage = 0.000001f; 
}

double Vector3Dot(Vector3D a, Vector3D b) { return a.X * b.X + a.Y * b.Y + a.Z * b.Z; }

double GetTotalHydroVolume() 
{ 
    double vol = 0; 
    for (int i = 0; i < _hydroTanks.Count; i++) vol += _hydroTanks[i].Capacity * _hydroTanks[i].FilledRatio; 
    return vol; 
}

string FormatTimeSpan(float seconds)
{
    if (seconds < 0f || float.IsNaN(seconds) || float.IsInfinity(seconds)) return "STABLE";
    TimeSpan t = TimeSpan.FromSeconds(seconds);
    if (t.TotalHours >= 1) return $"{(int)t.TotalHours}h {t.Minutes:00}m";
    return $"{t.Minutes:00}m {t.Seconds:00}s";
}

void UpdateTelemetryDisplay()
{
    if (_fleetLCD == null) return;
    
    _stringBuffer.Clear();
    _stringBuffer.AppendLine("=======================================");
    _stringBuffer.AppendLine("        FLEET CONTROL: LEADER          ");
    _stringBuffer.AppendLine("=======================================");
    _stringBuffer.AppendLine($"Broadcast State  : " + (_isActive ? "BROADCASTING" : "SILENT"));
    _stringBuffer.AppendLine($"Command Mode     : " + GetCommandStateString(_commandState));
    _stringBuffer.AppendLine($"Script Load      : {Runtime.LastRunTimeMs.ToString("0.0000")} ms");
    
    if (_mothershipRC != null)
    {
        _stringBuffer.AppendLine($"Velocity Vector  : {Math.Round(_mothershipRC.GetShipVelocities().LinearVelocity.Length(), 1)} m/s");
        _stringBuffer.AppendLine($"Ship Mass Load   : {Math.Round(_mothershipRC.CalculateShipMass().PhysicalMass / 1000.0, 1)} t");
    }
    else
    {
        _stringBuffer.AppendLine(" [ERROR: REMOTE CONTROL NOT FOUND]");
    }
    _stringBuffer.AppendLine("---------------------------------------");

    if (_hydroTanks.Count > 0)
    {
        double vol = GetTotalHydroVolume(), cap = 0; 
        for (int i = 0; i < _hydroTanks.Count; i++) cap += _hydroTanks[i].Capacity;
        double pct = (cap > 0) ? (vol / cap) * 100.0 : 0;
        _stringBuffer.AppendLine($"FUEL (H2): {vol.ToString("N0")} / {cap.ToString("N0")} L ({pct.ToString("N1")}%)");
        _prevHydroVol = vol;
    }
    else
    {
        _stringBuffer.AppendLine("FUEL (H2): NO TANKS DETECTED");
    }

    if (_batteries.Count > 0)
    {
        double chg = 0, max = 0, inp = 0, outP = 0;
        for (int i = 0; i < _batteries.Count; i++) 
        { 
            chg += _batteries[i].CurrentStoredPower; 
            max += _batteries[i].MaxStoredPower; 
            inp += _batteries[i].CurrentInput; 
            outP += _batteries[i].CurrentOutput; 
        }
        double netPower = inp - outP;
        double pct = (max > 0) ? (chg / max) * 100.0 : 0;
        _stringBuffer.AppendLine($"POWER  : {chg.ToString("N2")} / {max.ToString("N2")} MWh ({pct.ToString("N1")}%)");
        _stringBuffer.AppendLine($"NET LOAD : {netPower.ToString("N2")} MW");
    }
    else
    {
        _stringBuffer.AppendLine("POWER  : NO BATTERIES DETECTED");
    }

    _stringBuffer.AppendLine("---------------------------------------");
    _stringBuffer.AppendLine($"CONNECTED DRONES : {_fleetRegistry.Count}");
    _stringBuffer.AppendLine("---------------------------------------");
    
    if (_fleetRegistry.Count == 0)
    {
        _stringBuffer.AppendLine(" [NO ACTIVE DRONE LINK DETECTED]");
    }
    else
    {
        Vector3D masterPos = (_mothershipRC != null) ? _mothershipRC.GetPosition() : Vector3D.Zero;
        foreach (var drone in _fleetRegistry.Values)
        {
            double range = (_mothershipRC != null) ? Vector3D.Distance(masterPos, drone.Position) : 0.0;
            string hydroStr = (drone.HydroPct >= 0) ? $"{Math.Round(drone.HydroPct, 0)}%" : "N/A";
            string pwrStr = (drone.PowerPct >= 0) ? $"{Math.Round(drone.PowerPct, 0)}%" : "N/A";
            string hydroTimeStr = FormatTimeSpan(drone.HydroTimeSec);
            string pwrTimeStr = FormatTimeSpan(drone.PowerTimeSec);
            
            _stringBuffer.Append("- ").Append(drone.Name).Append(" [").Append(Math.Round(range, 0)).Append("m]\n");
            _stringBuffer.Append("  H2 : ").Append(hydroStr).Append(" (").Append(hydroTimeStr).Append(")");
            _stringBuffer.Append(" | PWR: ").Append(pwrStr).Append(" (").Append(pwrTimeStr).Append(")\n");
        }
    }
    _stringBuffer.AppendLine("=======================================");
    
    _fleetLCD.WriteText(_stringBuffer.ToString());
}

string GetCommandStateString(int state)
{
    if (state >= 8 && (state % 10) == 8)
    {
        int subSysId = (state - 8) / 10 % 10;
        int patternId = (state - 8) / 100 % 10;
        int targetGroup = (state - 8) / 1000 % 10;

        string subStr = subSysId == 1 ? "WEAPONS" : subSysId == 2 ? "PROPULSION" : subSysId == 3 ? "POWER" : "DEFAULT";
        string patStr = patternId == 1 ? "RANGE" : patternId == 2 ? "HIT&RUN" : patternId == 3 ? "INTERCEPT" : "CIRCLE";
        string grpStr = targetGroup == 1 ? "ESCORTS ONLY" : "ALL UNITS";

        return $"COMBAT ({grpStr} | {subStr} | {patStr})";
    }

    switch (state)
    {
        case 0: return "FORMATION FLIGHT (STATE 0)";
        case 1: return "NETWORK CONNECTED / STANDBY (STATE 1)";
        case 2: return "ESCORT DOCKED / CARRIER TRANSIT (STATE 2)";
        case 3: return "HARDPOINT BRAKED (STATE 3)";
        case 4: return "AUTOMATED LAUNCH (STATE 4)";
        case 5: return "AUTOMATED LANDING (STATE 5)";
        case 6: return "FLEET STAND / MANUAL (STATE 6)";
        case 7: return "REGISTERING POSITIONS (STATE 7)";
        case 9: return "REGISTERING ESCORT DOCKS (STATE 9)";
        case 10: return "FLEET STAND / NETWORK SYNC (STATE 10)";
        default: return "UNKNOWN STATE";
    }
}

void ParseTerminalCommands(string arg)
{
    if (_mothershipRC == null) return;

    string[] parts = arg.Trim().Split(':');
    string command = parts[0].ToUpper();

    if (command == "FLEET" && parts.Length >= 2)
    {
        string subCmd = parts[1].ToUpper();
        if (subCmd == "START") { _isActive = true; _commandState = 1; _mothershipRC.HandBrake = false; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "FORM" || subCmd == "UNDOCK") { _isActive = true; _commandState = 0; _mothershipRC.HandBrake = false; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "DOCK") { _isActive = true; _commandState = 2; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "SYNC" || subCmd == "RECONNECT") { _isActive = true; _commandState = 10; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "STOP") { _commandState = 1; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "LAUNCH")
        {
            _isActive = true; _commandState = 4; _mothershipRC.HandBrake = false; _mothershipGrounded = false;
            for (int i = 0; i < _landingGears.Count; i++) if (_landingGears[i].IsLocked) _landingGears[i].Unlock();
            _launchStartPos = _mothershipRC.GetPosition(); _launchAltitudeTarget = 1000.0;
            if (parts.Length >= 3) double.TryParse(parts[2], out _launchAltitudeTarget);
            ResetFlightOverrides();
        }
        else if (subCmd == "LAND") { _isActive = true; _commandState = 5; _mothershipRC.HandBrake = false; _landingTicks = 0; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "ABORT") { _isActive = true; _commandState = 1; _mothershipRC.HandBrake = false; _landingTicks = 0; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "STAND") { _isActive = true; _commandState = 6; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "REG_ALL") { _isActive = true; _commandState = 7; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "REG_DOCK") { _isActive = true; _commandState = 9; _mothershipGrounded = false; ResetFlightOverrides(); }
        else if (subCmd == "COMBAT")
        {
            _isActive = true;
            int subSysId = 0;
            int patternId = 0;
            int targetGroup = 0;

            for (int p = 2; p < parts.Length; p++)
            {
                string key = parts[p].ToUpper().Trim();
                if (key.Contains("ESCORT")) targetGroup = 1;
                else if (key.Contains("WEAPON")) subSysId = 1;
                else if (key.Contains("PROPULSION") || key.Contains("THRUST")) subSysId = 2;
                else if (key.Contains("POWER") || key.Contains("BATTERY")) subSysId = 3;
                else if (key.Contains("RANGE")) patternId = 1;
                else if (key.Contains("HIT") || key.Contains("RUN")) patternId = 2;
                else if (key.Contains("INTERCEPT") || key.Contains("RAM")) patternId = 3;
            }

            _commandState = 8 + (subSysId * 10) + (patternId * 100) + (targetGroup * 1000);
            _mothershipGrounded = false;
            ResetFlightOverrides();
        }
    }
}

void ResetFlightOverrides()
{
    if (_mothershipRC == null) return;
    for (int i = 0; i < _thrustersUp.Count; i++) _thrustersUp[i].ThrustOverridePercentage = 0f;
    for (int i = 0; i < _thrustersDown.Count; i++) _thrustersDown[i].ThrustOverridePercentage = 0f;
    for (int i = 0; i < _gyros.Count; i++) _gyros[i].GyroOverride = false;
}

void CacheSystems()
{
    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.CubeGrid == Me.CubeGrid);
    
    _mothershipRC = null;
    for (int i = 0; i < controllers.Count; i++)
    {
        if (controllers[i].CustomName.IndexOf(REMOTE_CONTROL_NAME, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            _mothershipRC = controllers[i];
            break;
        }
    }
    if (_mothershipRC == null)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i] is IMyRemoteControl)
            {
                _mothershipRC = controllers[i];
                break;
            }
        }
    }
    if (_mothershipRC == null && controllers.Count > 0)
    {
        _mothershipRC = controllers[0];
    }

    _fleetLCD = GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel;
    if (_fleetLCD != null) _fleetLCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    if (_mothershipRC == null) { _isInitialized = false; return; }

    List<IMyThrust> allThrusters = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType(allThrusters, t => t.CubeGrid == Me.CubeGrid);
    _thrustersUp.Clear(); _thrustersDown.Clear();
    var uDir = Base6Directions.GetOppositeDirection(_mothershipRC.Orientation.Up);
    var dDir = _mothershipRC.Orientation.Up;
    for (int i = 0; i < allThrusters.Count; i++)
    {
        if (allThrusters[i].Orientation.Forward == uDir) _thrustersUp.Add(allThrusters[i]);
        else if (allThrusters[i].Orientation.Forward == dDir) _thrustersDown.Add(allThrusters[i]);
    }

    _gyros.Clear(); GridTerminalSystem.GetBlocksOfType(_gyros, g => g.IsSameConstructAs(Me));
    _landingGears.Clear(); GridTerminalSystem.GetBlocksOfType(_landingGears, g => g.CubeGrid == Me.CubeGrid);
    
    _hydroTanks.Clear();
    GridTerminalSystem.GetBlocksOfType(_hydroTanks, t => t.BlockDefinition.SubtypeId.Contains("Hydrogen") && t.IsSameConstructAs(Me));
    
    _batteries.Clear();
    GridTerminalSystem.GetBlocksOfType(_batteries, b => b.IsSameConstructAs(Me));

    _isInitialized = true;
}