// Space Engineers - Smooth Prograde Vector Tracker (Anti-Jitter)
// Set cockpit hotbar slot to: Programmable Block -> Run (Argument: toggle)

private bool isTracking = false;
private const double speedMinimum = 1.0; 

// TUNING PARAMETERS:
private const double P_Gain = 15.0; // Alignment power. Lower if it overshoots.
private const double D_Gain = 12.0; // Braking power. Increase if the nose waves.
private const double Deadzone = 0.002; // Anti-jitter threshold. Raise slightly if jitter persists.

private Vector3D lastWorldRotationAxis = Vector3D.Zero;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.None;
}

public void Main(string argument, UpdateType updateSource)
{
    string arg = argument.ToLower().Trim();

    if (arg == "toggle")
    {
        isTracking = !isTracking;
        UpdateTrackingState();
    }
    else if (arg == "on")
    {
        isTracking = true;
        UpdateTrackingState();
    }
    else if (arg == "off")
    {
        isTracking = false;
        UpdateTrackingState();
    }

    if (isTracking && (updateSource & UpdateType.Update1) != 0)
    {
        ExecuteVectorAlignment();
    }
}

private void UpdateTrackingState()
{
    if (isTracking)
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update1; 
        lastWorldRotationAxis = Vector3D.Zero; 
        Echo("SMOOTH PROGRADE: ON");
    }
    else
    {
        Runtime.UpdateFrequency = UpdateFrequency.None;
        ResetGyros();
        Echo("SMOOTH PROGRADE: OFF");
    }
}

private void ExecuteVectorAlignment()
{
    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType<IMyShipController>(controllers, x => Me.IsSameConstructAs(x));

    if (controllers.Count == 0)
    {
        Echo("Error: No Cockpit found!");
        return;
    }

    IMyShipController cockpit = controllers[0];
    foreach (var ctrl in controllers)
    {
        if (ctrl.IsUnderControl && ctrl.CanControlShip)
        {
            cockpit = ctrl;
            break;
        }
    }

    Vector3D velocityVec = cockpit.GetShipVelocities().LinearVelocity;
    double currentSpeed = velocityVec.Length();

    if (currentSpeed < speedMinimum)
    {
        Echo("Speed too low. Locking gyros.");
        LockGyrosStationary();
        return;
    }

    List<IMyGyro> gyros = new List<IMyGyro>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, x => Me.IsSameConstructAs(x));

    if (gyros.Count == 0)
    {
        Echo("Error: No gyros found!");
        return;
    }

    Vector3D travelDirection = velocityVec / currentSpeed;
    Vector3D noseDirection = cockpit.WorldMatrix.Forward;

    // Proportional error vector
    Vector3D currentWorldRotationAxis = Vector3D.Cross(travelDirection, noseDirection);
    double errorMagnitude = currentWorldRotationAxis.Length();

    // JITTER FIX: Smoothly scale down input power if we are inside the deadzone
    double deadzoneScale = 1.0;
    if (errorMagnitude < Deadzone)
    {
        if (errorMagnitude <= 0.0001)
        {
            LockGyrosStationary();
            lastWorldRotationAxis = Vector3D.Zero;
            return;
        }
        // Creates a smooth braking curve instead of a hard drop-off
        deadzoneScale = errorMagnitude / Deadzone;
    }

    // Derivative calculation for counter-braking
    Vector3D angularVelocityDifference = currentWorldRotationAxis - lastWorldRotationAxis;
    lastWorldRotationAxis = currentWorldRotationAxis;

    // Apply the anti-jitter scale directly to the core alignment calculations
    Vector3D combinedWorldTorque = ((currentWorldRotationAxis * P_Gain) + (angularVelocityDifference * D_Gain * 60.0)) * deadzoneScale;

    foreach (var gyro in gyros)
    {
        gyro.GyroOverride = true;

        Vector3D localRotation = Vector3D.TransformNormal(combinedWorldTorque, MatrixD.Transpose(gyro.WorldMatrix));

        gyro.Pitch = (float)localRotation.X;
        gyro.Yaw = (float)localRotation.Y;
        gyro.Roll = (float)localRotation.Z;
    }
}

private void LockGyrosStationary()
{
    List<IMyGyro> gyros = new List<IMyGyro>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, x => Me.IsSameConstructAs(x));
    foreach (var gyro in gyros)
    {
        gyro.GyroOverride = true;
        gyro.Pitch = 0f;
        gyro.Yaw = 0f;
        gyro.Roll = 0f;
    }
}

private void ResetGyros()
{
    List<IMyGyro> gyros = new List<IMyGyro>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, x => Me.IsSameConstructAs(x));
    foreach (var gyro in gyros)
    {
        gyro.GyroOverride = false;
        gyro.Pitch = 0f;
        gyro.Yaw = 0f;
        gyro.Roll = 0f;
    }
}

