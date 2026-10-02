string _rcName = "RC Drill";
string _rollName = "Hinge Roll";
string _pitchName = "Hinge Pitch";
double _gain = 1.8;
double _smoothing = 0.2;
double _maxSpeed = 4.0;
double _deadZone = 0.1;

IMyRemoteControl _rc;
IMyMotorStator _mRoll, _mPitch;
double _filteredRoll = 0;
double _filteredPitch = 0;
long _tick = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    LoadConfig();
}

void LoadConfig()
{
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        Me.CustomData = "[Names]\nRemoteControl=RC Drill\nRollMotor=Hinge Roll\nPitchMotor=Hinge Pitch\n\n[Settings]\nGain=1.8\nSmoothing=0.2\nMaxSpeed=4.0\nDeadZone=0.1";
    }

    string[] lines = Me.CustomData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    string section = "";

    foreach (var line in lines)
    {
        var tLine = line.Trim();
        if (tLine.StartsWith("[") && tLine.EndsWith("]")) { section = tLine.ToLower(); continue; }

        if (section == "[names]")
        {
            if (tLine.Contains("RemoteControl=")) _rcName = tLine.Split('=')[1].Trim();
            if (tLine.Contains("RollMotor=")) _rollName = tLine.Split('=')[1].Trim();
            if (tLine.Contains("PitchMotor=")) _pitchName = tLine.Split('=')[1].Trim();
        }
        else if (section == "[settings]")
        {
            if (tLine.Contains("Gain=")) double.TryParse(tLine.Split('=')[1], out _gain);
            if (tLine.Contains("Smoothing=")) double.TryParse(tLine.Split('=')[1], out _smoothing);
            if (tLine.Contains("MaxSpeed=")) double.TryParse(tLine.Split('=')[1], out _maxSpeed);
            if (tLine.Contains("DeadZone=")) double.TryParse(tLine.Split('=')[1], out _deadZone);
        }
    }
}

public void Main(string argument, UpdateType updateSource)
{
    _tick++;
    if (argument.ToLower() == "reload") { LoadConfig(); return; }
    if (_tick % 300 == 0) LoadConfig();

    if (_rc == null || !_rc.IsWorking) _rc = GridTerminalSystem.GetBlockWithName(_rcName) as IMyRemoteControl;
    if (_mRoll == null) _mRoll = GridTerminalSystem.GetBlockWithName(_rollName) as IMyMotorStator;
    if (_mPitch == null) _mPitch = GridTerminalSystem.GetBlockWithName(_pitchName) as IMyMotorStator;

    if (_rc == null) return;

    Vector3D gravity = _rc.GetNaturalGravity();
    if (gravity.LengthSquared() < 0.1) return;
    gravity.Normalize();

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("=== STABILIZER SYSTEM ===");
    sb.AppendLine($"RC: {(_rc != null ? "OK" : "NOT FOUND")}");
    sb.AppendLine($"ROLL: {(_mRoll != null ? "OK" : "NOT FOUND")}");
    sb.AppendLine($"PITCH: {(_mPitch != null ? "OK" : "NOT FOUND")}");

    if (_mRoll != null && _mRoll.IsWorking)
    {
        double dot = Vector3D.Dot(_rc.WorldMatrix.Right, gravity);
        double rawError = Math.Asin(MathHelper.Clamp(dot, -1.0, 1.0)) * (180.0 / Math.PI);
        _filteredRoll = (_filteredRoll * (1.0 - _smoothing)) + (rawError * _smoothing);
        _mRoll.TargetVelocityRPM = ApplyLogic(_filteredRoll);
    }

    if (_mPitch != null && _mPitch.IsWorking)
    {
        double dot = Vector3D.Dot(_rc.WorldMatrix.Forward, gravity);
        double rawError = Math.Asin(MathHelper.Clamp(dot, -1.0, 1.0)) * (180.0 / Math.PI);
        _filteredPitch = (_filteredPitch * (1.0 - _smoothing)) + (rawError * _smoothing);
        _mPitch.TargetVelocityRPM = ApplyLogic(_filteredPitch);
    }

    var surface = Me.GetSurface(0);
    if (_tick < 3)
    {
        surface.ContentType = ContentType.TEXT_AND_IMAGE;
        surface.FontSize = 1.0f;
        surface.FontColor = new Color(100, 255, 255);
        surface.BackgroundColor = Color.Black;
    }
    if (_tick % 5 == 0) surface.WriteText(sb.ToString());
}

float ApplyLogic(double error)
{
    if (Math.Abs(error) < _deadZone) return 0;
    return (float)MathHelper.Clamp(error * _gain, -_maxSpeed, _maxSpeed);
}