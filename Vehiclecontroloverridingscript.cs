  #region "Utility"
  internal PidController
      Pid = new PidController(0.33, 0.05, 0, 100);

  public sealed class PidController
  {

    private double _previousValue = 0;

    public double D { get; set; } = 0;

    public double I { get; set; } = 0;

    public double P { get; set; } = 0;

    public double ClampValue { get; private set; } = 0;

    public double Integral { get; private set; } = 0;

    private IMyTextSurface surface = null;

    public PidController(double p, double i, double d, double ClampValue)
    {
      this.P = p;
      this.I = i;
      this.D = d;
      this.ClampValue = ClampValue;
    }

    public double ControlVariable(double setPoint, double value, TimeSpan timePassed)
    {
      double error = setPoint - value;

      Integral += (I * error * timePassed.TotalSeconds);
      Integral = Clamp(Integral);

      double dInput = value - _previousValue;
      double derivative = D * (dInput / timePassed.TotalSeconds);

      double proportional = P * error;

      double output = proportional + Integral - derivative;

      output = Clamp(output);

      _previousValue = value;

      return output;
    }

    double Clamp(double value)
    {
      if (Math.Abs(value) >= ClampValue)
        return ClampValue * Math.Sign(value);
      return value;
    }
  }

  internal class SuspensionWrapper
  {
    public IMyMotorSuspension Suspension;
    public int Side = 1;
    public IMyShipController Controller;
    private double ToGridCenterDistance = 0;

    public SuspensionWrapper(IMyMotorSuspension newSuspension, IMyShipController controller)
    {
      Suspension = newSuspension;
      Controller = controller;
      ToGridCenterDistance = (Controller.CubeGrid.GetPosition() - Suspension.GetPosition()).Length();
    }

    public void SuspensionStrengthChanged(object sender, SuspensionStrengthChangedEventArgs e)
    {
      Suspension.Strength = e.Strength;
    }

    public void SuspensionParametersChanged(object sender, SuspensionParametersChangedEventArgs e)
    {
      Suspension.SetValueFloat("Speed Limit", e.Speed);
      Suspension.Friction = e.Friction;
      Suspension.MaxSteerAngle = e.TurnLimit;
      Suspension.SetValueFloat("Steer override", e.Rudder);

      Suspension.SetValueFloat("Propulsion override", e.Movement * Side);
    }

    public void SuspensionStatusChanged(object sender, SuspensionEnableEventArg e)
    {
      Suspension.Enabled = e.Enabled;
    }
  }

  internal delegate void SuspensionParametersChangedHandler(object sender, SuspensionParametersChangedEventArgs e);
  internal delegate void SuspensionStrengthChangedHandler(object sender, SuspensionStrengthChangedEventArgs e);
  internal delegate void SuspensionEnableHandler(object sender, SuspensionEnableEventArg e);

  internal delegate void Steering(object sender, SteerArg e);
  internal class SteerArg : EventArgs
  { public float Angle { get; set; } }

  internal class SteeringWheel
  {
    IMyMotorStator _steeringWheel;
    internal SteeringWheel(IMyMotorStator steeringWheel)
    { _steeringWheel = steeringWheel; }

    public void Main(object sender, SteerArg e)
    {
      _steeringWheel.TargetVelocityRad = (e.Angle - _steeringWheel.Angle) * 7.5f;
    }
  }

  internal class SuspensionStrengthChangedEventArgs : EventArgs
  {
    public float Strength { get; set; }
    public SuspensionStrengthChangedEventArgs() { Strength = 1.0f; }
  }

  internal class SuspensionParametersChangedEventArgs : EventArgs
  {
    public float Speed { get; set; }
    public float Friction { get; set; }
    public float TurnLimit { get; set; }
    public float Rudder { get; set; }
    public float Movement { get; set; }
  }

  internal class SuspensionEnableEventArg : EventArgs
  {
    public bool Enabled = false;
    public SuspensionEnableEventArg(bool state)
    {
      Enabled = state;
    }
  }
  #endregion
  internal class Vehicle
  {
    #region "private fields"
    Program _program;
    IMyProgrammableBlock PBVDS = null;

    enum State { Idle, Prepare, Running, Stop, Detach, Attach };
    State _state;

    Color
      reverseLigthColor = new Color(175, 195, 235),
      dark = new Color(5, 5, 5),
      darkRed = new Color(7, 2, 2),
      mediumRed = new Color(75, 10, 5),
      brightRed = new Color(235, 25, 5);

    readonly bool stopIfUncontrolled = true;

    bool
     _handBrake = true,
     _actionMoveUp = false,
     _actionMoveForward = false,
     _actionMoveBackward = false,
     _actionBrake = true,
     _trailerReady = false,
     _fullThrottle = false,
     _semi = false,
     _haveCoupler = false,
     _keyboardSteering = false;

    Vector3D prevPos;

    event SuspensionParametersChangedHandler ChangeTruckSuspensionParameters;
    event SuspensionStrengthChangedHandler ChangeTruckSuspensionStrength;
    event SuspensionEnableHandler EnableTruckSuspension;

    //the lower this value the harder suspension
    float
      _truckSuspensionSoftnessFactor = 42.5f,
      _trailerSuspensionSoftnessFactor = 42.5f,
      _truckSuspensionStrength = 10.0f,
      _trailerSuspensionStrength = 10.0f,

      _requiredSpeed = 0,
      _acceleration = 0.2f,
      _maxForwardSpeed = 90,
      _maxBackwardSpeed = 15,

      _SteerOverride = 0,

      _truckMass = float.MaxValue,
      _truckCargoMass = 0,
      _trailerMass = float.MaxValue,
      _trailerCargoMass = 0;

    string
      _controllerName = "Cockpit",
      _couplerName = "Coupler",
      _steeringWheelName = "Rotor Small",
      _trailerControllerName = "Trailer Controller";
    StringBuilder _initMessage = new StringBuilder("");

    IMyShipController _controller;
    IMyMotorStator _coupler;

    SteeringWheel _steeringWheel;

    List<IMyTerminalBlock>
      _truckContainers = new List<IMyTerminalBlock>(),
      _trailerContainers = new List<IMyTerminalBlock>();

    List<IMyShipController> _trailerControllers = new List<IMyShipController>();

    List<SuspensionWrapper>
      _truckSuspensions = new List<SuspensionWrapper>(),
      _trailerSuspensions = new List<SuspensionWrapper>();

    List<IMyLightingBlock>
     _reverseLights = new List<IMyLightingBlock>(),
     _brakeLights = new List<IMyLightingBlock>();

    event Steering SteeringHandler = delegate { };
    event SuspensionParametersChangedHandler ChangeTrailerSuspensionParameters;
    event SuspensionStrengthChangedHandler ChangeTrailerSuspensionStrength;

    #endregion

    #region "private methods"
    private delegate bool ConstructionCheck(IMyTerminalBlock block);

    private bool SameGrid(IMyTerminalBlock block)
    { return block.CubeGrid == _program.Me.CubeGrid; }

    private bool SameConstruction(IMyTerminalBlock block)
    { return block.IsSameConstructAs(_program.Me); }

    private T GetFirstWithName<T>(string name, ConstructionCheck delCheck) where T : class, IMyTerminalBlock
    {
      name = name.Trim();
      List<T> units = new List<T>();
      _program.GridTerminalSystem.GetBlocksOfType(units);
      foreach (T unit in units)
        if (delCheck.Invoke(unit) && unit.CustomName.Trim() == name)
          return unit;
      return null;
    }

    private void GetFirstWithName<T>(ref T block, string name, ConstructionCheck delCheck) where T : class, IMyTerminalBlock
    {
      block = null;
      name = name.Trim();
      List<T> units = new List<T>();
      _program.GridTerminalSystem.GetBlocksOfType(units);
      foreach (T unit in units)
        if (delCheck.Invoke(unit) && unit.CustomName.Trim() == name)
        {
          block = unit;
          return;
        }
    }

    private void FillListWith<T>(ref List<T> refList, string name) where T : class, IMyTerminalBlock
    {
      name = name.ToLower();
      List<T> units = new List<T>();
      _program.GridTerminalSystem.GetBlocksOfType(units);
      foreach (T unit in units)
        if (unit.IsSameConstructAs(_program.Me) && unit.CustomName.ToLower().Contains(name))
          refList.Add(unit);
      units.Clear();
    }

    private void FillListWith<T>(ref List<T> refList) where T : class, IMyTerminalBlock
    {
      List<T> units = new List<T>();
      _program.GridTerminalSystem.GetBlocksOfType(units);
      foreach (T unit in units)
        if (unit.IsSameConstructAs(_program.Me))
          refList.Add(unit);
      units.Clear();
    }

    private void ReadStoredData()
    {
      string[] lines = _program.Me.CustomData.Split('\n');
      string[] words;
      words = lines[0].Split(':'); _controllerName = words[1].Trim();
      words = lines[1].Split(':'); _steeringWheelName = words[1].Trim();
      words = lines[2].Split(':'); _keyboardSteering = bool.Parse(words[1].Trim());
      words = lines[3].Split(':'); _couplerName = words[1].Trim();
      words = lines[4].Split(':'); _semi = bool.Parse(words[1].Trim());
      words = lines[5].Split(':'); _acceleration = float.Parse(words[1].Trim());
      words = lines[6].Split(':'); _maxForwardSpeed = float.Parse(words[1].Trim());
      words = lines[7].Split(':'); _maxBackwardSpeed = float.Parse(words[1].Trim());
      words = lines[8].Split(':'); _trailerControllerName = words[1].Trim();
    }

    private void WriteTemplateData()
    {
      StringBuilder sb = new StringBuilder("");
      sb.Append("Controller name: " + _controllerName + "\n");
      sb.Append("Steering wheel name: " + _steeringWheelName + "\n");
      sb.Append("Keyboard steering: " + _keyboardSteering + "\n");
      sb.Append("Coupler name: " + _couplerName + "\n");
      sb.Append("Is semi: " + _semi + "\n");
      sb.Append("Acceleration: " + _acceleration + "\n");
      sb.Append("Forward speed: " + _maxForwardSpeed + "\n");
      sb.Append("Backward speed: " + _maxBackwardSpeed + "\n");
      sb.Append("Trailer controller name: " + _trailerControllerName + "\n");
      sb.Append("Truck unit mass: " + _truckMass + "\n");
      _program.Me.CustomData = sb.ToString();
    }

    private void InitializeProperties()
    {
      try { ReadStoredData(); }
      catch { WriteTemplateData(); }

      try
      {
        string[] lines = _program.Me.CustomData.Split('\n');
        string[] words;
        words = lines[7].Split(':'); _truckMass = float.Parse(words[1].Trim());
      }
      catch { }
    }

    private void InitializeAndSubscribeSuspensions()
    {
      List<IMyTerminalBlock> units = new List<IMyTerminalBlock>();
      _program.GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(units);
      string name = "";
      foreach (IMyTerminalBlock unit in units)
        if (unit.CubeGrid == _program.Me.CubeGrid)
        {
          name = unit.CustomName.ToLower();
          SuspensionWrapper suspensionWrapper = new SuspensionWrapper(unit as IMyMotorSuspension, _controller);
          if (name.Contains("left"))
            suspensionWrapper.Side = -1;
          ChangeTruckSuspensionParameters += suspensionWrapper.SuspensionParametersChanged;
          ChangeTruckSuspensionStrength += suspensionWrapper.SuspensionStrengthChanged;
          EnableTruckSuspension += suspensionWrapper.SuspensionStatusChanged;
          _truckSuspensions.Add(suspensionWrapper);
        }
      units.Clear();
    }

    private void GatherTruckContainersFrom<T>(ref List<IMyTerminalBlock> containers) where T : class, IMyTerminalBlock
    {
      List<T> units = new List<T>();
      _program.GridTerminalSystem.GetBlocksOfType(units);
      foreach (T unit in units)
        if (unit.CubeGrid == _program.Me.CubeGrid && unit.HasInventory)
          containers.Add(unit);
      units.Clear();
    }

    private void GatherTrailerContainersFrom<T>(ref List<IMyTerminalBlock> containers) where T : class, IMyTerminalBlock
    {
      List<T> units = new List<T>();
      _program.GridTerminalSystem.GetBlocksOfType(units);
      foreach (T unit in units)
        if (unit.IsSameConstructAs(_program.Me) && unit.CubeGrid != _program.Me.CubeGrid && unit.HasInventory)
          containers.Add(unit);
      units.Clear();
    }

    private void CheckBlock<T>(ref T block, string Message) where T : class, IMyTerminalBlock
    { if (block == null) _initMessage.AppendLine(Message); }

    private void InitializeSystems()
    {
      GetFirstWithName<IMyShipController>(ref _controller, _controllerName, SameGrid);
      if (_controller != null) prevPos = _controller.CubeGrid.GetPosition();
      else throw new Exception("Error - Cockpit block is 't found.");

      GetFirstWithName<IMyProgrammableBlock>(ref PBVDS, "PB VDS", SameGrid);
      CheckBlock(ref PBVDS, "Warning - Dashboard script is't found.");

      IMyMotorStator steeringWheel = GetFirstWithName<IMyMotorStator>(_steeringWheelName, SameGrid);

      if (steeringWheel != null)
      {
        _steeringWheel = new SteeringWheel(steeringWheel);
        SteeringHandler += _steeringWheel.Main;
        _initMessage.AppendLine("Warning - Steering wheel is't found.");
      }

      GetFirstWithName<IMyMotorStator>(ref _coupler, _couplerName, SameConstruction);
      _haveCoupler = _coupler != null;
      CheckBlock(ref _coupler, "Warning - Coupler is't found.");
      GatherTruckContainersFrom<IMyCargoContainer>(ref _truckContainers);
      GatherTruckContainersFrom<IMyShipConnector>(ref _truckContainers);
      InitializeAndSubscribeSuspensions();
      if (_haveCoupler && _coupler.IsAttached)
        _state = State.Attach;
      InitializeLights();
    }

    private void InitializeLights()
    {
      _reverseLights = new List<IMyLightingBlock>();
      _brakeLights = new List<IMyLightingBlock>();
      string name = "";
      List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
      FillListWith(ref lights);
      foreach (IMyLightingBlock light in lights)
      {
        name = light.CustomName.ToLower();
        if (name.Contains("reverse"))
        {
          light.Enabled = false;
          light.Color = new Color(5, 5, 5);
          _reverseLights.Add(light);
          continue;
        }
        if (name.Contains("brake"))
        {
          light.Color = darkRed;
          light.Radius = 4.25f;
          light.Intensity = 10.0f;
          _brakeLights.Add(light);
        }
      }
    }

    private void SetBrakeLightsproperties(Color newColor, float newValue)
    {
      foreach (IMyLightingBlock light in _brakeLights)
      {
        light.Color = newColor;
        light.SetValueFloat("Intensity", newValue);
      }
    }

    private void SetReverseLightsProperties(Color newColor, bool enabled)
    {
      foreach (IMyLightingBlock light in _reverseLights)
      {
        light.Color = newColor;
        light.Enabled = enabled;
      }
    }

    private void LightController()
    {

      if (Math.Abs(_requiredSpeed) < 0.01f || _actionBrake)
        SetBrakeLightsproperties(brightRed, 10.0f);
      else
        SetBrakeLightsproperties(mediumRed, 5.0f);
      if (_requiredSpeed < 0)
        SetReverseLightsProperties(reverseLigthColor, true);
      else
        SetReverseLightsProperties(dark, false);
    }

    private void GatherTrailerSystems()
    {
      _trailerReady = false;
      if (!_haveCoupler) return;
      _trailerControllers = new List<IMyShipController>();
      FillListWith(ref _trailerControllers, _trailerControllerName);
      if (_trailerControllers.Count == 0) return;
      _trailerContainers = new List<IMyTerminalBlock>();
      GatherTrailerContainersFrom<IMyCargoContainer>(ref _trailerContainers);
      GatherTrailerContainersFrom<IMyShipConnector>(ref _trailerContainers);
      _trailerSuspensions = new List<SuspensionWrapper>();
      List<IMyTerminalBlock> units = new List<IMyTerminalBlock>();
      _program.GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(units);
      foreach (IMyTerminalBlock unit in units)
        if (unit.IsSameConstructAs(_program.Me) && unit.CubeGrid != _program.Me.CubeGrid)
        {
          SuspensionWrapper suspensionWrapper = new SuspensionWrapper(unit as IMyMotorSuspension, _trailerControllers[0]);
          ChangeTrailerSuspensionParameters += suspensionWrapper.SuspensionParametersChanged;
          ChangeTrailerSuspensionStrength += suspensionWrapper.SuspensionStrengthChanged;
          if (unit.CustomName.ToLower().Contains("left"))
            suspensionWrapper.Side = -1;
          _trailerSuspensions.Add(suspensionWrapper);
        }
      units.Clear();
      if (_trailerControllers.Count > 0)
        _trailerReady = true;
      InitializeLights();
    }

    private void CleanTrailerSystems()
    {
      if (!_haveCoupler) return;
      _trailerControllers = new List<IMyShipController>();
      _trailerSuspensions = new List<SuspensionWrapper>();
      _trailerContainers = new List<IMyTerminalBlock>();
      InitializeLights();
      _trailerReady = false;
    }

    private void SetSpeed(float newSpeed)
    {
      if (-_maxBackwardSpeed <= newSpeed && newSpeed <= _maxForwardSpeed)
        _requiredSpeed = newSpeed;
    }

    private void AccelerateByZInput()
    {
      SetSpeed(_requiredSpeed + _acceleration * (-_controller.MoveIndicator.Z));
    }

    private void BrakesController()
    {
      if (_trailerControllers.Count > 0)
        foreach (IMyShipController trailerController in _trailerControllers)
          trailerController.HandBrake = _controller.HandBrake;
    }

    private void Input()
    {

      if (!_actionMoveBackward)
      {
        if (_controller.MoveIndicator.Z > 0)
        {
          _actionMoveBackward = true;
          if (!_actionBrake)
            _handBrake = false;
          if (_requiredSpeed > _acceleration)
            _actionBrake = true;
        }
      }
      else
      {
        if (!_actionBrake) // moving backward
          AccelerateByZInput();
        if (_requiredSpeed >= _acceleration) // braking
          SetSpeed(_requiredSpeed + _acceleration * (-_controller.MoveIndicator.Z) * 2f);
        if (_actionBrake && _requiredSpeed < _acceleration)
        {
          _handBrake = true;
          _requiredSpeed = 0.0f;
          movement = 0;
        }
      }


      if (!_actionMoveForward)
      {
        if (_controller.MoveIndicator.Z < 0)
        {
          _actionMoveForward = true;
          if (!_actionBrake)
            _handBrake = false;
          if (_requiredSpeed < -_acceleration)
            _actionBrake = true;
        }
      }
      else
      {
        if (!_actionBrake || _requiredSpeed <= -_acceleration)
          AccelerateByZInput();
        if (_actionBrake && _requiredSpeed > -_acceleration)
        {
          _handBrake = true;
          _requiredSpeed = 0.0f;
          movement = 0;
        }
      }

      if (Math.Abs(_controller.MoveIndicator.Z) <= 0.01f)
      {
        _actionMoveForward = false;
        _actionMoveBackward = false;
        _actionBrake = false;
      }

      //handbrake
      if (!_actionMoveUp)
      {
        if (_controller.MoveIndicator.Y > 0)
        {
          _handBrake = !_handBrake;
          _actionMoveUp = true;
          _requiredSpeed = 0;
          movement = 0;
        }
      }
      else if (Math.Abs(_controller.MoveIndicator.Y) <= 0.01f)
        _actionMoveUp = false;
    }

    float _prevStrength = 10f;
    private void ApplySuspensionStrength()
    {
      SuspensionStrengthChangedEventArgs args = new SuspensionStrengthChangedEventArgs
      {
        Strength = _truckSuspensions[0].Suspension.Strength
      };
      if (args.Strength < _truckSuspensionStrength - 1.5f) // will prevent truck from jumping
        args.Strength += 1.5f;
      else
        args.Strength = _truckSuspensionStrength;
      ChangeTruckSuspensionStrength?.Invoke(this, args);

      if (!_haveCoupler) return;
      if (_trailerReady)
      {
        args.Strength = _trailerSuspensions[0].Suspension.Strength;
        if (args.Strength < _trailerSuspensionStrength - 1.5f)
          args.Strength += 1.5f;
        else
          args.Strength = _trailerSuspensionStrength;
        ChangeTrailerSuspensionStrength?.Invoke(this, args);
      }
    }

    readonly float HalfPI = (float)Math.PI / 2;

    private void CalculateSuspensionStrength()
    {
      float gravityFactor = (float)_controller.GetNaturalGravity().Length() / 9.81f;
      if (_semi)
      {
        float truckCurrentMass = _truckMass * gravityFactor;
        float massToWheelsDifference = truckCurrentMass / (_truckSuspensions.Count + _trailerSuspensions.Count);
        _truckSuspensionStrength = (float)Math.Sqrt(massToWheelsDifference / _truckSuspensionSoftnessFactor);
        _trailerSuspensionStrength = (float)Math.Sqrt(massToWheelsDifference / _trailerSuspensionSoftnessFactor);
      }
      else
      {
        float truckCurrentMass = (_truckMass + _truckCargoMass) * gravityFactor;
        float massToWheelDifference = truckCurrentMass / _truckSuspensions.Count;
        _truckSuspensionStrength = (float)Math.Sqrt(massToWheelDifference / _truckSuspensionSoftnessFactor);
        if (!_haveCoupler)
          return;
        if (_trailerReady)
        {
          float trailerCurrentMass = (_trailerMass + _trailerCargoMass) * gravityFactor;
          massToWheelDifference = trailerCurrentMass / _trailerSuspensions.Count;
          _trailerSuspensionStrength = (float)Math.Sqrt(massToWheelDifference / _trailerSuspensionSoftnessFactor);
        }
      }
    }

    private void CalculateTruckMass()
    {
      if (_coupler == null || _semi || !_coupler.IsAttached)
      {
        _truckMass = _controller.CalculateShipMass().PhysicalMass;
        WriteTemplateData();
        return;
      }

      foreach (IMyTerminalBlock container in _truckContainers)
        _truckCargoMass += container.GetInventory().CurrentMass.ToIntSafe();

      try
      {
        string[] lines = _program.Me.CustomData.Split('\n');
        string[] words;
        words = lines[7].Split(':');
        _truckMass = float.Parse(words[1].Trim());
      }
      catch
      {
        _truckMass = _controller.CalculateShipMass().PhysicalMass;
      }
      if (_coupler.IsAttached)
      {
        _trailerMass = _controller.CalculateShipMass().BaseMass - _truckMass;
        _trailerCargoMass = 0.0f;
        foreach (IMyTerminalBlock container in _trailerContainers)
          _trailerCargoMass += container.GetInventory().CurrentMass.ToIntSafe();
      }
      WriteTemplateData();
    }
    #endregion

    #region "public methods"
    double[] pitchFactorDump = { 0d, 0d, 0d, 0d, 0d };

    private void WritePitch(double factor)
    {
      pitchFactorDump[4] = pitchFactorDump[3];
      pitchFactorDump[3] = pitchFactorDump[2];
      pitchFactorDump[2] = pitchFactorDump[1];
      pitchFactorDump[1] = pitchFactorDump[0];
      pitchFactorDump[0] = factor;
    }

    private double GetMedianPitch()
    {
      return (pitchFactorDump[0] + pitchFactorDump[1] + pitchFactorDump[2] + pitchFactorDump[3] + pitchFactorDump[4]) / 5;
    }

    double speed = 0;
    float movement = 0; 
    bool previousInputIsError = false;
    private void SetSuspensionProperties()
    {
      SuspensionParametersChangedEventArgs args = new SuspensionParametersChangedEventArgs();
      args.Speed = float.MaxValue;// Math.Abs(_requiredSpeed);
      _controller.HandBrake = _handBrake;

  /*    Vector3D G = _controller.GetNaturalGravity();
      
      Vector3D forward = _controller.WorldMatrix.Forward;
      Vector3D pitchAxle = Vector3D.Cross(G, _controller.WorldMatrix.Right);

      Vector3D forwardToGProj = Vector3D.ProjectOnVector(ref forward, ref G);
      Vector3D forwardToPitchAxleProj = Vector3D.ProjectOnVector(ref forward, ref pitchAxle);

      Vector3D projPlane = forwardToGProj + forwardToPitchAxleProj;

      double pitchFactor = ((float)Math.Acos(Vector3D.Dot(projPlane, G) / (projPlane.Length() * G.Length())) - HalfPI) / HalfPI;

      WritePitch(pitchFactor);
      pitchFactor = GetMedianPitch();
      Vector3D MoveVector = _controller.CubeGrid.GetPosition() - prevPos;

      double cSpeed = Vector3D.ProjectOnVector(ref MoveVector, ref forward).Length() / _program.Runtime.TimeSinceLastRun.TotalSeconds;
      if (!double.IsNaN(cSpeed) && !double.IsInfinity(cSpeed)) speed = cSpeed;

      if (Vector3D.Dot(MoveVector, pitchAxle) > 0)
        speed = -speed;
*/
      speed = _controller.GetShipSpeed();

      double debugNumber = Math.Floor(_requiredSpeed * 10) / 10;
     // _program.Echo("Req Speed : " + debugNumber);
      debugNumber = Math.Floor(speed * 10) / 10;
      //_program.Echo("Speed : " + debugNumber);

      if (Math.Abs(_requiredSpeed) > 0.2)
      {

        float cMovement = (float)-_program.Pid.ControlVariable(0, speed - (double)_requiredSpeed, _program.Runtime.TimeSinceLastRun);
       // _program.Echo("PID : " + cMovement);
        if (!float.IsNaN(cMovement) && !previousInputIsError) movement = cMovement;
        previousInputIsError = float.IsNaN(cMovement);
        args.Movement = movement;
      }
      else
      {
        args.Movement = 0;
       // _program.Echo("PID : Error");
      }

      float speedFactor = (float)speed / _maxForwardSpeed;

      args.Friction = 50.0f - 25.0f * speedFactor;
      args.TurnLimit = (float)((40.0f - 20.0f * speedFactor) / 180.0f * Math.PI);
      args.Rudder = _SteerOverride;

      ChangeTruckSuspensionParameters?.Invoke(this, args);
      if (!_haveCoupler) return;
      if (_trailerReady)
      {
        foreach (IMyShipController trailerController in _trailerControllers)
          trailerController.HandBrake = _controller.HandBrake;
        float maxAngle = 110.0f - 100.0f * speedFactor;
        _coupler.LowerLimitDeg = -maxAngle;
        _coupler.UpperLimitDeg = maxAngle;
        double value = 1.0 - Math.Abs(_coupler.Angle) / (Math.PI);
        if (value < 0.0) value = 0.0;
        args.Movement *= (float)value;
        args.Friction *= (float)value;
        ChangeTrailerSuspensionParameters?.Invoke(this, args);
      }
    }

    private void Steer()
    {
      if (_controller.MoveIndicator.X > 0 && _SteerOverride < 1)
        _SteerOverride += _controller.MoveIndicator.X * 0.02f;
      if (_controller.MoveIndicator.X < 0 && _SteerOverride > -1)
        _SteerOverride += _controller.MoveIndicator.X * 0.02f;
      SteerArg args = new SteerArg();
      args.Angle = (float)_SteerOverride;
      SteeringHandler.Invoke(this, args);
    }

    public Vehicle(Program newProgram)
    {
      _program = newProgram;
      _program.Me.CustomName = "PB VCOS";
      InitializeProperties();
      InitializeSystems();
    }

    public void InputCommand(string argument)
    {
      switch (argument.ToLower())
      {
        case "trailer":
          if (!_haveCoupler) return;
          if (!_coupler.IsAttached)
          {
            //  _coupler.Attach();
            _coupler.ApplyAction("Attach");
            _state = State.Attach;
            _program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
          }
          else
          {
            //  _coupler.Detach();
            _coupler.ApplyAction("Detach");
            _state = State.Detach;
            _program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
          }
          if (PBVDS != null)
            _program.IGC.SendUnicastMessage(PBVDS.EntityId,"", "trailer");
          CalculateTruckMass();
          break;
        case "mass":
          CalculateTruckMass();
          CalculateSuspensionStrength();
          break;
        case "strength":
          ApplySuspensionStrength();
          break;
        case "full":
          _fullThrottle = !_fullThrottle;
          break;
      }
    }

    public int Main()
    {
      try
      {
        _program.Echo(_initMessage.ToString());
        _program.Echo("State : " + _state);
        _program.Echo("Coupler : " + _haveCoupler);
        if (_haveCoupler) _program.Echo("Is semi : " + _semi);
        switch (_state)
        {
          case State.Idle:
            if (_controller.IsUnderControl)
            {
              _program.Runtime.UpdateFrequency = UpdateFrequency.Update1;
              _state = State.Prepare;
              EnableTruckSuspension?.Invoke(this, new SuspensionEnableEventArg(true));
            }
            break; // End Idle

          case State.Prepare:
            CalculateTruckMass();
            CalculateSuspensionStrength();
            _state = State.Running;
            break; // End Idle

          case State.Running:
            if (!_controller.IsUnderControl)
            {
              _state = State.Stop;
              break;
            }

            Input();

            if (_keyboardSteering)
              Steer();

            BrakesController();

            if (_trailerReady)
              foreach (IMyShipController trailerController in _trailerControllers)
                trailerController.HandBrake = _controller.HandBrake;

            SetSuspensionProperties();
            prevPos = _controller.CubeGrid.GetPosition();

            LightController();
            break; // End Running

          case State.Attach:
            if (_coupler.IsAttached && !_trailerReady)
              GatherTrailerSystems();
            _state = State.Prepare;
            _program.Runtime.UpdateFrequency = UpdateFrequency.Update1;
            break; // End Attach

          case State.Detach:
            if (!_coupler.IsAttached && _trailerReady)
              CleanTrailerSystems();
            _state = State.Prepare;
            _program.Runtime.UpdateFrequency = UpdateFrequency.Update1;
            break; // End Detach

          case State.Stop:
            if (stopIfUncontrolled)
              _handBrake = true;
            Input();
            BrakesController();
            SetBrakeLightsproperties(darkRed, 1.0f);
            SetReverseLightsProperties(dark, false);
            SetSuspensionProperties();
            EnableTruckSuspension?.Invoke(this, new SuspensionEnableEventArg(false));
            _state = State.Idle;
            _program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
            break; // End Stop

          default:
            _state = State.Idle;
            break;
        }
      }
      catch (NullReferenceException e)
      {
        _program.Echo("Check your truck. Required block(s) is missing. Or check names stored in custom data.");
        _program.Echo(e.Message);
        _program.Runtime.UpdateFrequency = UpdateFrequency.Update100;
        return 1;
      }
      catch (Exception e)
      {
        _program.Echo(e.Message);
        _program.Echo(e.StackTrace);
        _program.Runtime.UpdateFrequency = UpdateFrequency.None;
        return 1;
      }
      return 0;
    }
    #endregion
  }

  public Program()
  {
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    _unit = new Vehicle(this);
  }
  Vehicle _unit;

  void Main(string argument)
  {
    _unit.Main();
    _unit.InputCommand(argument);
  }
