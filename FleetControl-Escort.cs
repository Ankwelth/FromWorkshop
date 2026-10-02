/* ============================================================================
 * UNIFIED FLIGHT OS: FLEET CONTROL - ESCORT (REV 8.1)
 * ============================================================================
 */

const string REMOTE_CONTROL_NAME = "Remote Control"; 
const string ESCORT_PAIR_TAG = "ESCORT_GLOBAL_PAIR";

const double KP_POSITION = 2.5; 
const double KD_VELOCITY = 3.5; 
const double BRAKING_BOOST_MULTIPLIER = 1.5; 
const double KP_ROTATION = 8.5;
const double KP_ALIGN = 3.0;
const double MAX_ANGULAR_VELOCITY = 3.5; 
const double MAX_POSITIONAL_CLAMP = 20.0; 
const double LANDING_DESCENT_SPEED = -5.0;

long _parentId = 0;
double _offsetX = 0, _offsetY = 0, _offsetZ = 0;
string _cachedCustomData = "";

IMyShipController _escortRC;
IMyBroadcastListener _parentListener;
IMyBroadcastListener _pairListener;

bool _isInitialized = false;
bool _manualControlActive = false;
int _networkTimeoutTicks = 0;
int _undockTicks = 0;
bool _landingDone = false;
int _escortLandingTicks = 0;

MatrixD _lastParentMatrix = MatrixD.Identity;
Vector3D _lastParentVelocity = Vector3D.Zero;
Vector3D _lastParentAngularVel = Vector3D.Zero; 
int _lastCommandState = 1;

List<IMyThrust> _thrustersForward = new List<IMyThrust>(), _thrustersBackward = new List<IMyThrust>();
List<IMyThrust> _thrustersUp = new List<IMyThrust>(), _thrustersDown = new List<IMyThrust>();
List<IMyThrust> _thrustersLeft = new List<IMyThrust>(), _thrustersRight = new List<IMyThrust>();
List<IMyThrust> _thrustersAll = new List<IMyThrust>();
List<IMyGyro> _gyros = new List<IMyGyro>();
List<IMyLandingGear> _landingGears = new List<IMyLandingGear>();
List<IMyShipConnector> _connectors = new List<IMyShipConnector>();
List<IMyFunctionalBlock> _aiCombatBlocks = new List<IMyFunctionalBlock>();
List<IMyGasTank> _hydroTanks = new List<IMyGasTank>();

int _rateTickCounter = 0;
bool _lastAiTargetState = true;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    _pairListener = IGC.RegisterBroadcastListener(ESCORT_PAIR_TAG);
    _pairListener.SetMessageCallback(ESCORT_PAIR_TAG);
    LoadConfig();
    CacheSystems();
    EnforceAIBlocksState(false);
}

public void Save()
{
    SaveConfig();
}

public void Main(string argument, UpdateType updateSource)
{
    if (!_isInitialized)
    {
        CacheSystems();
        if (!_isInitialized) return;
    }

    LoadConfig();

    if (_pairListener != null && _pairListener.HasPendingMessage)
    {
        ProcessAutoPairing();
    }

    if (_parentListener != null && _parentListener.HasPendingMessage)
    {
        while (_parentListener.HasPendingMessage)
        {
            MyIGCMessage msg = _parentListener.AcceptMessage();
            if (msg.Data is MyTuple<MatrixD, Vector3D, Vector3D, int>)
            {
                var payload = (MyTuple<MatrixD, Vector3D, Vector3D, int>)msg.Data;
                _lastParentMatrix = payload.Item1;
                _lastParentVelocity = payload.Item2;
                _lastParentAngularVel = payload.Item3;
                _lastCommandState = payload.Item4;
                _networkTimeoutTicks = 0;
            }
        }
    }

    if (!string.IsNullOrEmpty(argument) && argument != ESCORT_PAIR_TAG && !argument.StartsWith("ESCORT_SYNC_"))
    {
        ParseCommands(argument);
        return;
    }

    if ((updateSource & UpdateType.Update1) != 0)
    {
        _networkTimeoutTicks++;
        _rateTickCounter++;
        if (_rateTickCounter >= 60) _rateTickCounter = 0;
        if (_undockTicks > 0) _undockTicks--;
    }

    Echo($"=== FLEET CONTROL: ESCORT ===");
    Echo($"BOUND PARENT ID : {(_parentId == 0 ? "UNBOUND" : _parentId.ToString())}");
    Echo($"COMMAND STATE   : {_lastCommandState}");

    if (!_manualControlActive)
    {
        bool linkActive = (_networkTimeoutTicks <= 30 && _parentId != 0);
        if (linkActive)
        {
            ProcessParentDirectives(_lastParentMatrix, _lastParentVelocity, _lastParentAngularVel, _lastCommandState);
        }
        else
        {
            ExecuteLinkLossCombatHandover();
        }
    }
}

void ProcessAutoPairing()
{
    if (_escortRC == null) return;
    Vector3D escortPos = _escortRC.GetPosition();
    double closestDistSq = double.MaxValue;
    long selectedParentId = 0;
    MatrixD selectedParentMatrix = MatrixD.Identity;

    while (_pairListener.HasPendingMessage)
    {
        MyIGCMessage msg = _pairListener.AcceptMessage();
        if (msg.Data is MyTuple<long, MatrixD>)
        {
            var ping = (MyTuple<long, MatrixD>)msg.Data;
            long candidateId = ping.Item1;
            MatrixD candidateMatrix = ping.Item2;

            double distSq = Vector3D.DistanceSquared(escortPos, candidateMatrix.Translation);
            if (distSq < closestDistSq)
            {
                closestDistSq = distSq;
                selectedParentId = candidateId;
                selectedParentMatrix = candidateMatrix;
            }
        }
    }

    if (selectedParentId != 0)
    {
        _parentId = selectedParentId;
        _lastParentMatrix = selectedParentMatrix;
        MatrixD invertParent = MatrixD.Invert(selectedParentMatrix);
        Vector3D relativeOffset = Vector3D.Transform(escortPos, invertParent);
        _offsetX = relativeOffset.X;
        _offsetY = relativeOffset.Y;
        _offsetZ = relativeOffset.Z;

        SaveConfig();
        BindParentListener();
        Echo($"[AUTO-PAIRED TO PARENT: {_parentId}]");
    }
}

void BindParentListener()
{
    if (_parentId != 0)
    {
        string channel = "ESCORT_SYNC_" + _parentId.ToString();
        _parentListener = IGC.RegisterBroadcastListener(channel);
        _parentListener.SetMessageCallback(channel);
    }
}

void RegisterDockingPort()
{
    if (_escortRC == null) return;

    IMyShipConnector connectedConn = null;
    for (int i = 0; i < _connectors.Count; i++)
    {
        if (_connectors[i].Status == MyShipConnectorStatus.Connected)
        {
            connectedConn = _connectors[i];
            break;
        }
    }

    if (connectedConn == null || connectedConn.OtherConnector == null)
    {
        Echo("[DOCK REGISTRATION FAILED: NO CONNECTED CONNECTOR]");
        return;
    }

    IMyShipConnector targetConn = connectedConn.OtherConnector;
    _parentId = targetConn.CubeGrid.EntityId;
    BindParentListener();

    MatrixD refParentMatrix = (_networkTimeoutTicks <= 30 && _lastParentMatrix != MatrixD.Identity) 
        ? _lastParentMatrix 
        : targetConn.CubeGrid.WorldMatrix;

    MatrixD invertParent = MatrixD.Invert(refParentMatrix);
    Vector3D relativeOffset = Vector3D.Transform(_escortRC.GetPosition(), invertParent);

    _offsetX = relativeOffset.X;
    _offsetY = relativeOffset.Y;
    _offsetZ = relativeOffset.Z;

    SaveConfig();
    Echo($"[DOCK REGISTERED | PARENT: {_parentId} | OFFSET: {_offsetX:0.00}, {_offsetY:0.00}, {_offsetZ:0.00}]");
}

void ProcessParentDirectives(MatrixD parentMatrix, Vector3D parentVelocity, Vector3D parentAngularVel, int commandState)
{
    if (_escortRC == null) return;

    if (commandState == 10)
    {
        _cachedCustomData = "";
        LoadConfig();
        CacheSystems();
        BindParentListener();
        ClearFlightOverrides();
        EnforceAIBlocksState(false);
        _networkTimeoutTicks = 0;
        Echo("[NETWORK SYNC: CONFIG & SYSTEMS RELOADED - STANDBY ENGAGED]");
        return;
    }

    bool combatModeActive = (commandState >= 8 && (commandState % 10) == 8);
    EnforceAIBlocksState(combatModeActive, commandState);

    if (combatModeActive)
    {
        ClearFlightOverrides();
        Echo("[ESCORT MODE: COMBAT / AI TARGETING ENGAGED]");
        return;
    }

    if (commandState == 1 || commandState == 6 || commandState == 7)
    {
        ClearFlightOverrides();
        EnforceAIBlocksState(false);
        _landingDone = false;
        _escortLandingTicks = 0;
        if (commandState == 7 && parentMatrix.Translation != Vector3D.Zero)
        {
            MatrixD invertParent = MatrixD.Invert(parentMatrix);
            Vector3D relativeOffset = Vector3D.Transform(_escortRC.GetPosition(), invertParent);
            _offsetX = relativeOffset.X; _offsetY = relativeOffset.Y; _offsetZ = relativeOffset.Z;
            SaveConfig();
        }
        return;
    }

    if (commandState != 5)
    {
        _landingDone = false;
        _escortLandingTicks = 0;
    }

    if (commandState == 5 && _landingDone)
    {
        ClearFlightOverrides();
        if (_escortRC != null) _escortRC.HandBrake = true;
        return;
    }

    bool landingGearsLocked = false;
    for (int i = 0; i < _landingGears.Count; i++)
    {
        if (commandState == 5) _landingGears[i].AutoLock = true;
        if (_landingGears[i].IsLocked) landingGearsLocked = true;
    }

    if (commandState == 5)
    {
        if (_escortRC != null) _escortRC.HandBrake = false;
        for (int i = 0; i < _connectors.Count; i++)
        {
            _connectors[i].PullStrength = 0f;
            if (_connectors[i].Status == MyShipConnectorStatus.Connected) _connectors[i].Disconnect();
        }
    }
    else if (commandState == 0 || commandState == 4)
    {
        ExecuteGridRelease();
    }

    Vector3D escortPos = _escortRC.GetPosition();
    Vector3D escortVel = _escortRC.GetShipVelocities().LinearVelocity;
    float escortMass = _escortRC.CalculateShipMass().PhysicalMass;
    Vector3D gravityVector = _escortRC.GetNaturalGravity();
    Vector3D upVector = (gravityVector.LengthSquared() > 0) ? -Vector3D.Normalize(gravityVector) : parentMatrix.Up;

    if (_undockTicks > 0)
    {
        Vector3D pushForce = (-_escortRC.WorldMatrix.Forward * escortMass * 2.0) - (gravityVector * escortMass);
        ComputeThrustMatrix(pushForce);
        return;
    }

    Vector3D requiredForceWorld = Vector3D.Zero;

    if (commandState == 5)
    {
        Vector3D localOffset = new Vector3D(_offsetX, _offsetY, _offsetZ);
        Vector3D targetWorldPosition = Vector3D.Transform(localOffset, parentMatrix);
        Vector3D positionError = targetWorldPosition - escortPos;

        Vector3D horizontalPosError = positionError - (Vector3D.Dot(positionError, upVector) * upVector);
        if (horizontalPosError.LengthSquared() > MAX_POSITIONAL_CLAMP * MAX_POSITIONAL_CLAMP)
        {
            horizontalPosError = Vector3D.Normalize(horizontalPosError) * MAX_POSITIONAL_CLAMP;
        }

        Vector3D horizontalVelocity = escortVel - (Vector3D.Dot(escortVel, upVector) * upVector);
        Vector3D parentHorizontalVel = parentVelocity - (Vector3D.Dot(parentVelocity, upVector) * upVector);
        Vector3D horizontalVelError = parentHorizontalVel - horizontalVelocity;
        Vector3D horizontalAcc = (horizontalPosError * KP_POSITION) + (horizontalVelError * KD_VELOCITY);

        double surfAlt = 0;
        bool hasSurf = _escortRC.TryGetPlanetElevation(MyPlanetElevation.Surface, out surfAlt);
        double targetDescentSpeed = LANDING_DESCENT_SPEED;

        if (hasSurf)
        {
            if (surfAlt > 150.0)
            {
                float maxEscortLift = 0f;
                for (int i = 0; i < _thrustersUp.Count; i++) if (_thrustersUp[i].IsFunctional) maxEscortLift += _thrustersUp[i].MaxEffectiveThrust;
                double accelPotential = (escortMass > 0) ? (maxEscortLift / escortMass) : 0;
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

        double currentVertSpeed = Vector3D.Dot(escortVel, upVector);

        if (Math.Abs(currentVertSpeed) < 0.05 && hasSurf && surfAlt < 10.0)
        {
            _escortLandingTicks++;
        }
        else
        {
            _escortLandingTicks = 0;
        }

        if (landingGearsLocked || (_escortLandingTicks > 30))
        {
            ClearFlightOverrides();
            if (_escortRC != null) _escortRC.HandBrake = true;
            _landingDone = true;
            return;
        }

        Vector3D verticalSpeedErrorVec = upVector * (targetDescentSpeed - currentVertSpeed);
        Vector3D verticalAcc = verticalSpeedErrorVec * 2.5;

        Vector3D totalAcc = horizontalAcc + verticalAcc;
        requiredForceWorld = (totalAcc * escortMass) - (gravityVector * escortMass);
    }
    else
    {
        Vector3D localOffset = new Vector3D(_offsetX, _offsetY, _offsetZ);
        Vector3D targetWorldPos = Vector3D.Transform(localOffset, parentMatrix);
        Vector3D positionError = targetWorldPos - escortPos;

        Vector3D worldOffset = Vector3D.TransformNormal(localOffset, parentMatrix);
        Vector3D tangentialVel = Vector3D.Cross(parentAngularVel, worldOffset);
        Vector3D targetVelocity = parentVelocity + tangentialVel;
        Vector3D velocityError = targetVelocity - escortVel;

        Vector3D clampedPosError = positionError;
        if (clampedPosError.LengthSquared() > MAX_POSITIONAL_CLAMP * MAX_POSITIONAL_CLAMP)
        {
            clampedPosError = Vector3D.Normalize(clampedPosError) * MAX_POSITIONAL_CLAMP;
        }

        double effectiveDamping = KD_VELOCITY;
        if (Vector3D.Dot(escortVel, velocityError) < 0) effectiveDamping *= BRAKING_BOOST_MULTIPLIER;

        Vector3D targetAcc = (clampedPosError * KP_POSITION) + (velocityError * effectiveDamping);
        requiredForceWorld = (targetAcc * escortMass) - (gravityVector * escortMass);
    }

    ComputeThrustMatrix(requiredForceWorld);

    if (commandState == 5)
    {
        Vector3D alignmentVector = Vector3D.Cross(_escortRC.WorldMatrix.Up, gravityVector.LengthSquared() > 0 ? gravityVector : -parentMatrix.Up);
        ApplyGyroMatrix(-alignmentVector * KP_ALIGN);
    }
    else
    {
        QuaternionD currentOrientation = QuaternionD.CreateFromRotationMatrix(_escortRC.WorldMatrix);
        QuaternionD targetOrientation = QuaternionD.CreateFromRotationMatrix(parentMatrix);
        QuaternionD errorQuaternion = targetOrientation * QuaternionD.Inverse(currentOrientation);
        Vector3D worldRotationAxis; double rotationAngle;
        errorQuaternion.GetAxisAngle(out worldRotationAxis, out rotationAngle);
        if (rotationAngle > Math.PI) rotationAngle -= 2 * Math.PI;

        double targetAngularVel = (double)MathHelper.Clamp((float)(rotationAngle * KP_ROTATION), (float)-MAX_ANGULAR_VELOCITY, (float)MAX_ANGULAR_VELOCITY);
        ApplyGyroMatrix(worldRotationAxis * targetAngularVel);
    }
}

void SafeApplyAction(IMyTerminalBlock block, string actionId)
{
    if (block == null) return;
    var action = block.GetActionWithName(actionId);
    if (action != null)
    {
        action.Apply(block);
    }
}

void EnforceAIBlocksState(bool enable, int commandState = 8)
{
    bool stateChanged = (_lastAiTargetState != enable);
    _lastAiTargetState = enable;

    if (stateChanged || (enable && _rateTickCounter == 0))
    {
        int subSysId = (commandState >= 8 && (commandState % 10) == 8) ? ((commandState - 8) / 10) % 10 : 0;
        int patternId = (commandState >= 8 && (commandState % 10) == 8) ? ((commandState - 8) / 100) % 10 : 0;

        for (int i = 0; i < _aiCombatBlocks.Count; i++)
        {
            var block = _aiCombatBlocks[i];
            if (block == null) continue;

            if (enable)
            {
                block.Enabled = true;
                SafeApplyAction(block, "OnOff_On");
                SafeApplyAction(block, "ActivateBehavior_On");
                ApplyAITargetingConfiguration(block, subSysId, patternId);
            }
            else
            {
                SafeApplyAction(block, "ActivateBehavior_Off");
                SafeApplyAction(block, "OnOff_Off");
                block.Enabled = false;
            }
        }
    }
}

void ApplyAITargetingConfiguration(IMyFunctionalBlock block, int subSysId, int patternId)
{
    if (block == null) return;
    List<ITerminalAction> actions = new List<ITerminalAction>();
    block.GetActions(actions);

    string subKey = "DEFAULT";
    if (subSysId == 1) subKey = "WEAPON";
    else if (subSysId == 2) subKey = "PROPULSION";
    else if (subSysId == 3) subKey = "POWER";

    for (int i = 0; i < actions.Count; i++)
    {
        if (actions[i] == null || actions[i].Id == null) continue;
        string id = actions[i].Id.ToUpper();
        if ((id.Contains("SUBSYSTEM") || id.Contains("TARGET")) && id.Contains(subKey))
        {
            actions[i].Apply(block);
            break;
        }
    }

    string patKey = "CIRCLE";
    if (patternId == 1) patKey = "RANGE";
    else if (patternId == 2) patKey = "HIT";
    else if (patternId == 3) patKey = "INTERCEPT";

    for (int i = 0; i < actions.Count; i++)
    {
        if (actions[i] == null || actions[i].Id == null) continue;
        string id = actions[i].Id.ToUpper();
        if ((id.Contains("PATTERN") || id.Contains("ATTACK") || id.Contains("MODE")) && id.Contains(patKey))
        {
            actions[i].Apply(block);
            break;
        }
    }
}

void ExecuteLinkLossCombatHandover()
{
    ClearFlightOverrides();
    EnforceAIBlocksState(true, 8);
    Echo("[STATUS: PARENT LINK LOST - AI COMBAT HANDOVER ENGAGED]");
}

void ExecuteGridRelease()
{
    if (_escortRC != null) _escortRC.HandBrake = false;
    for (int i = 0; i < _landingGears.Count; i++)
    {
        _landingGears[i].AutoLock = false;
        _landingGears[i].Unlock();
    }
    for (int i = 0; i < _connectors.Count; i++)
    {
        _connectors[i].PullStrength = 0f;
        if (_connectors[i].Status == MyShipConnectorStatus.Connected) 
        {
            _connectors[i].Disconnect();
            _undockTicks = 60;
        }
    }
}

void ComputeThrustMatrix(Vector3D forceVectorWorld)
{
    if (_escortRC == null) return;
    MatrixD transposeMatrix = MatrixD.Transpose(_escortRC.WorldMatrix);
    Vector3D localForce = Vector3D.TransformNormal(forceVectorWorld, transposeMatrix);
    ScaleThrustOutputs(_thrustersForward, _thrustersBackward, -localForce.Z);
    ScaleThrustOutputs(_thrustersRight, _thrustersLeft, localForce.X);
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

void ApplyGyroMatrix(Vector3D rotationVectorWorld)
{
    for (int i = 0; i < _gyros.Count; i++)
    {
        Vector3D localRotation = Vector3D.TransformNormal(rotationVectorWorld, MatrixD.Transpose(_gyros[i].WorldMatrix));
        _gyros[i].Pitch = (float)-localRotation.X; _gyros[i].Yaw = (float)-localRotation.Y; _gyros[i].Roll = (float)-localRotation.Z;
        _gyros[i].GyroOverride = true;
    }
}

void ClearFlightOverrides()
{
    Action<List<IMyThrust>> purgeThrust = list => { for (int i = 0; i < list.Count; i++) list[i].ThrustOverridePercentage = 0f; };
    purgeThrust(_thrustersForward); purgeThrust(_thrustersBackward);
    purgeThrust(_thrustersUp); purgeThrust(_thrustersDown);
    purgeThrust(_thrustersLeft); purgeThrust(_thrustersRight);
    for (int i = 0; i < _gyros.Count; i++) _gyros[i].GyroOverride = false;
    if (_escortRC != null) _escortRC.DampenersOverride = true;
}

bool IsAiBlockDefinition(IMyFunctionalBlock b)
{
    if (b == null) return false;
    string typeId = b.BlockDefinition.TypeIdString ?? "";
    string subtypeId = b.BlockDefinition.SubtypeId ?? "";

    return typeId.Contains("FlightMovement") ||
           typeId.Contains("OffensiveCombat") ||
           typeId.Contains("BasicMission") ||
           typeId.Contains("DefensiveCombat") ||
           subtypeId.ToUpper().Contains("FLIGHTMOVEMENT") ||
           subtypeId.ToUpper().Contains("OFFENSIVECOMBAT") ||
           subtypeId.ToUpper().Contains("BASICMISSION") ||
           subtypeId.ToUpper().Contains("DEFENSIVECOMBAT");
}

void CacheSystems()
{
    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.CubeGrid == Me.CubeGrid);
    
    _escortRC = null;
    for (int i = 0; i < controllers.Count; i++)
    {
        if (controllers[i].CustomName.IndexOf(REMOTE_CONTROL_NAME, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            _escortRC = controllers[i];
            break;
        }
    }
    if (_escortRC == null)
    {
        for (int i = 0; i < controllers.Count; i++)
        {
            if (controllers[i] is IMyRemoteControl)
            {
                _escortRC = controllers[i];
                break;
            }
        }
    }
    if (_escortRC == null && controllers.Count > 0)
    {
        _escortRC = controllers[0];
    }

    if (_escortRC == null) { _isInitialized = false; return; }

    List<IMyThrust> allThrusters = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType(allThrusters, t => t.CubeGrid == Me.CubeGrid);
    _thrustersForward.Clear(); _thrustersBackward.Clear(); _thrustersUp.Clear(); _thrustersDown.Clear(); _thrustersLeft.Clear(); _thrustersRight.Clear();
    _thrustersAll.Clear();
    _thrustersAll.AddRange(allThrusters);

    var fDir = Base6Directions.GetOppositeDirection(_escortRC.Orientation.Forward); var bDir = _escortRC.Orientation.Forward;
    var uDir = Base6Directions.GetOppositeDirection(_escortRC.Orientation.Up); var dDir = _escortRC.Orientation.Up;
    var lDir = Base6Directions.GetOppositeDirection(_escortRC.Orientation.Left); var rDir = _escortRC.Orientation.Left;

    for (int i = 0; i < allThrusters.Count; i++)
    {
        var t = allThrusters[i];
        if (t.Orientation.Forward == fDir) _thrustersForward.Add(t);
        else if (t.Orientation.Forward == bDir) _thrustersBackward.Add(t);
        else if (t.Orientation.Forward == uDir) _thrustersUp.Add(t);
        else if (t.Orientation.Forward == dDir) _thrustersDown.Add(t);
        else if (t.Orientation.Forward == lDir) _thrustersLeft.Add(t);
        else if (t.Orientation.Forward == rDir) _thrustersRight.Add(t);
    }

    _gyros.Clear(); GridTerminalSystem.GetBlocksOfType(_gyros, g => g.CubeGrid == Me.CubeGrid);
    _landingGears.Clear(); GridTerminalSystem.GetBlocksOfType(_landingGears, g => g.CubeGrid == Me.CubeGrid);
    _connectors.Clear(); GridTerminalSystem.GetBlocksOfType(_connectors, c => c.CubeGrid == Me.CubeGrid);

    _aiCombatBlocks.Clear();
    GridTerminalSystem.GetBlocksOfType(_aiCombatBlocks, b => b.CubeGrid == Me.CubeGrid && IsAiBlockDefinition(b));

    _hydroTanks.Clear();
    GridTerminalSystem.GetBlocksOfType(_hydroTanks, t => (t.BlockDefinition.SubtypeId ?? "").Contains("Hydrogen") && t.CubeGrid == Me.CubeGrid);

    BindParentListener();

    _isInitialized = true;
}

void LoadConfig()
{
    if (Me.CustomData == _cachedCustomData) return;
    string[] lines = Me.CustomData.Trim().Split('\n');
    foreach (var line in lines)
    {
        string[] parts = line.Split('=');
        if (parts.Length != 2) continue;
        string key = parts[0].Trim().ToUpper();

        if (key == "PARENT_ID")
        {
            long newParentId;
            if (long.TryParse(parts[1].Trim(), out newParentId))
            {
                if (newParentId != _parentId)
                {
                    _parentId = newParentId;
                    BindParentListener();
                }
            }
        }
        else
        {
            double val;
            if (double.TryParse(parts[1].Trim(), out val))
            {
                if (key == "X") _offsetX = val;
                else if (key == "Y") _offsetY = val;
                else if (key == "Z") _offsetZ = val;
            }
        }
    }
    SaveConfig();
}

void SaveConfig()
{
    _cachedCustomData = $"PARENT_ID={_parentId}\nX={_offsetX}\nY={_offsetY}\nZ={_offsetZ}";
    Me.CustomData = _cachedCustomData;
}

void ParseCommands(string arg)
{
    string[] parts = arg.Trim().Split(':');
    string command = parts[0].ToUpper();

    if (command == "MANCONTROL")
    {
        _manualControlActive = !_manualControlActive; ClearFlightOverrides(); return;
    }
    if (command == "RECONNECT" || command == "SYNC")
    {
        _cachedCustomData = "";
        LoadConfig();
        CacheSystems();
        BindParentListener();
        ClearFlightOverrides();
        EnforceAIBlocksState(false);
        _networkTimeoutTicks = 0;
        return;
    }
    if (command == "REG_DOCK" || (command == "DOCK" && parts.Length >= 2 && parts[1].ToUpper() == "REGISTER"))
    {
        RegisterDockingPort();
        return;
    }
    if (command == "SET_PARENT" && parts.Length >= 2)
    {
        long.TryParse(parts[1], out _parentId);
        SaveConfig();
        BindParentListener();
        return;
    }

    if (parts.Length >= 2)
    {
        if (command == "X") double.TryParse(parts[1], out _offsetX);
        else if (command == "Y") double.TryParse(parts[1], out _offsetY);
        else if (command == "Z") double.TryParse(parts[1], out _offsetZ);
        else if (command == "OFFSET" && parts.Length >= 4)
        {
            double.TryParse(parts[1], out _offsetX); double.TryParse(parts[2], out _offsetY); double.TryParse(parts[3], out _offsetZ);
        }
        SaveConfig();
    }
}