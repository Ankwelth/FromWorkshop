/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

MyIni iniInformation = new MyIni();
IMyTextSurface panel1, panel2;
IMyCockpit cockpit = null;
IMyRemoteControl remoteControl1;
IMySensorBlock sensor;
IMyCameraBlock camera;
int counter;
int distanceCounter = 0;
double distanceTemp;
Vector3D MeAngle;
const float angleTolerance = 0.01f, angleToleranceMultiplier = 1, angleMultiplier = 10 , angleMultiplier2 = 2;
const float breakLimit_Float = 150;
bool fisrtRun = true;
MatrixD direction_MatrixD;


const string shipInformation_Section = "ShipInformation";
const string sensor_Key = "Sensor";
const string camera_Key = "Camera";
const string thrustPowerCorrectionFactor_Key = "Thrust Power Correction Factor";
const string elevationOffset_Key = "Elevation Offset";
const string detectionRange_Key = "Detection Range";
const string finalRange_Key = "Final Range";
const string finalRangeSpeed_Key = "Final Range Speed";
const string buttonInstructions_Key = "Button Instructions";
const string brakePanel_Key = "Brake Panel";
const string copyMainScreen_Key = "Copy Main Screen";
const string maxSpeed_Key = "Max Speed";
const string cargoMassRatio_Key = "Cargo Mass Ratio";


const string mission_Section = "Mission";
const string mode_Key = "Mode";
const string mode_Stop = "Stop", mode_ThrustsOff = "Thrusts Off", mode_Charge = "Charge", mode_MaintainLevelFlight = "Maintain Level Flight", mode_FlyForward = "Fly Forward", mode_Up = "Up", mode_Down = "Down", mode_Adjust = "Adjust Posture", mode_FlytoTargetGPS = "Fly to Target GPS", mode_Brake = "Brake";
const string speed_Key = "Speed";
const string forward_String = "Forward", backward_String = "Backward", up_String = "Up", down_String = "Down";
public struct Point3D
{
    public double X;
    public double Y;
    public double Z;
}


public Program()
{

    Echo("program");

    // This time we _must_ check for failure since the user may have written invalid ini.
    MyIniParseResult result;
    if (!iniInformation.TryParse(Me.CustomData, out result))
        throw new Exception(result.ToString());

    //  Initialize CustomData
    string dataTemp;
    dataTemp = Me.CustomData;
    if (dataTemp == "" || dataTemp == null)
    {

        iniInformation.Set(shipInformation_Section, sensor_Key, "Sensor - Ground Proximity Warning");
        iniInformation.Set(shipInformation_Section, camera_Key, "Camera - Range Finding");
        iniInformation.Set(shipInformation_Section, buttonInstructions_Key, "BI");
        iniInformation.Set(shipInformation_Section, brakePanel_Key, "Brake_Panel");
        iniInformation.Set(shipInformation_Section, copyMainScreen_Key, "Copy_Main_Screen");
        iniInformation.Set(shipInformation_Section, thrustPowerCorrectionFactor_Key, "0.2");
        iniInformation.Set(shipInformation_Section, elevationOffset_Key, "30");
        iniInformation.Set(shipInformation_Section, detectionRange_Key, "1000");
        iniInformation.Set(shipInformation_Section, finalRange_Key, "20");
        iniInformation.Set(shipInformation_Section, finalRangeSpeed_Key, "0.5");
        iniInformation.Set(shipInformation_Section, maxSpeed_Key, "196");
        iniInformation.Set(shipInformation_Section, cargoMassRatio_Key, "2.7");

        iniInformation.Set(mission_Section, mode_Key, mode_Stop);
        iniInformation.Set(mission_Section, speed_Key, "100");

        Me.CustomData = iniInformation.ToString();

    }

    Runtime.UpdateFrequency = UpdateFrequency.Once | UpdateFrequency.Update10;


    Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(1).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(0).FontSize = 1f;
    Me.GetSurface(1).FontSize = 6f;
    Me.GetSurface(0).FontColor = new Color(0, 130, 255);
    Me.GetSurface(1).FontColor = new Color(0, 130, 255);
    Me.GetSurface(1).Alignment = TextAlignment.CENTER;
    Me.GetSurface(1).WriteText("Flight Control");
    Me.GetSurface(1).TextPadding = 20;

    Me.CustomName = "Auxiliary Flight Control System V3.0";
    Echo("program");

}// program end!

public void Save()
{

}

public void ButtonOnShip(string argument)
{
    switch (argument)
    {
        case "1"://Stop & Defult
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Stop);
            Mode_Stop();
            break;
        case "2"://Thrusts Off & Tanks On & Batteries On
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_ThrustsOff);
            ThrustEnabled(false);
            HydrogenCharge(false);
            BatteryCharge(false);
            break;
        case "3"://Thrusts & Tanks & Batteries Off
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Charge);
            ThrustEnabled(false);
            HydrogenCharge(true);
            BatteryCharge(true);
            break;
        case "4"://Speed Limitation at 1m/s
            SpeedChange(3);
            if (BrakeDistance(Vector3I.Forward) > breakLimit_Float) WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Brake);
            break;
        case "5"://Speed Limitation at Max
            SpeedChange(4);
            break;
        case "6"://Speed Limitation -5m/s
            SpeedChange(1);
            break;
        case "7"://Speed Limitation +5m/s
            SpeedChange(2);
            break;
        case "LF"://Maintain Level Flight
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_MaintainLevelFlight);
            break;
        case "FA"://Fly Forward
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_FlyForward);
            break;
        case "AD": //Adjust Posture
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Adjust);
            break;
        case "DN"://Straight Down
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Down);
            break;
        case "UP"://Straight Up
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Up);
            break;
        case "TA"://Fly to Target GPS
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_FlytoTargetGPS);
            break;
    }

}// ButtonOnShip END!

public void Mode_Stop()
{
    ThrustEnabled(true);
    HydrogenCharge(false);
    BatteryCharge(false);
    remoteControl1.ClearWaypoints();
    remoteControl1.SetAutoPilotEnabled(false);
    remoteControl1.DampenersOverride = true;
    remoteControl1.SetCollisionAvoidance(false);

    fisrtRun = true;
    camera.EnableRaycast = false;
    distanceCounter = 0;

    List<IMyGyro> gyros = new List<IMyGyro>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, gyro => gyro.IsSameConstructAs(Me));
    if (gyros.Count != 0)
    {
        foreach (var gyro in gyros)
        {
            gyro.GyroOverride = false;
        }
    }
}

public void AdjustAttitudeToBrake()
{
    ThrustEnabled(true);

    Vector3D speedDirection_Vector3D = remoteControl1.GetShipVelocities().LinearVelocity;
    if (speedDirection_Vector3D.Length() < 10)
    {
        RecoverAttitude();
        return;
    }

    string thrustDirection_String;
    float thrustsPower_Float = GetMaxThrustsPower(out thrustDirection_String);
    GetBrakePanel("\n", true);
    GetBrakePanel($"Used_ThrustPower = {thrustsPower_Float / 1000} kN\n", true);
    GetBrakePanel($"Used_ThrustDirection = {thrustDirection_String}\n", true);

    switch (thrustDirection_String)
    {
        case down_String:
            AdjustAttitude(remoteControl1.WorldMatrix.Down, remoteControl1.WorldMatrix.Forward);
            break;
        case backward_String:
            AdjustAttitude(remoteControl1.WorldMatrix.Backward, remoteControl1.WorldMatrix.Down);
            break;
    }
}

public void RecoverAttitude()
{
    Vector3D temp_Vector3D;
    if (cockpit.TryGetPlanetPosition(out temp_Vector3D))
    {
        FlyStable();
        if(Math.Abs(MeAngle.Z) < angleTolerance && Math.Abs(MeAngle.X) < angleTolerance)
        {
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Stop);
            return;
        }
    }
    else
    {
        Vector3D angle = Vector3D.TransformNormal(remoteControl1.WorldMatrix.Forward, direction_MatrixD);
        ControlGyros(angle);
        if(Math.Abs(angle.Y) < angleTolerance * angleToleranceMultiplier)
        {
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Stop);
            return;
        }
    }
}

public void AdjustAttitude(Vector3D forward_Vector3D,Vector3D up_Vector3D)
{
    Vector3D speedDirection_Vector3D = cockpit.GetShipVelocities().LinearVelocity;
    MatrixD refLookAtMatrix = MatrixD.CreateLookAt(new Vector3D(), forward_Vector3D, up_Vector3D);
    Vector3D angle = Vector3D.TransformNormal(Vector3D.Normalize(speedDirection_Vector3D), refLookAtMatrix);

    ShowDegree(angle);

    List<IMyGyro> gyros = new List<IMyGyro>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, block => block.IsSameConstructAs(Me));
    if (gyros.Count < 1) return;


    foreach (var gyro in gyros)
    {

        //偏航姿态调整
        if (Math.Abs(angle.X) < angleTolerance * angleToleranceMultiplier && angle.Z < 0)
        {
            gyro.SetValue<float>("Yaw", 0);//偏航
            gyro.SetValue<float>("Roll", 0);//翻滚
        }
        else
        {
            if (angle.Z < 0)
            {
                gyro.SetValue<float>("Yaw", -Convert.ToSingle(angle.X) * angleMultiplier);
            }
            else
            {
            }
            gyro.GyroOverride = true;
        }

        //俯仰姿态调整
        if (Math.Abs(angle.Y) < angleTolerance * angleToleranceMultiplier && angle.Z < 0)
        {
            gyro.SetValue<float>("Pitch", 0);//俯仰
            gyro.SetValue<float>("Roll", 0);//翻滚
        }
        else
        {
            if (angle.Z < 0)
            {
                gyro.SetValue<float>("Pitch", Convert.ToSingle(angle.Y) * angleMultiplier);
            }
            else
            {
                gyro.SetValue<float>("Pitch", (-Convert.ToSingle(angle.Y) + 2) * angleMultiplier);
            }
            gyro.GyroOverride = true;
        }

        if (Math.Abs(angle.X) < angleTolerance * angleToleranceMultiplier && Math.Abs(angle.Y) < angleTolerance * angleToleranceMultiplier && angle.Z < 0)
        {
            gyro.SetValue<float>("Yaw", 0);//偏航
            gyro.SetValue<float>("Roll", 0);//翻滚
            gyro.SetValue<float>("Pitch", 0);//俯仰
            gyro.GyroOverride = false;
        }
    }
}

public void ControlGyros(Vector3D angle)
{

    ShowDegree(angle);

    List<IMyGyro> gyros = new List<IMyGyro>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, block => block.IsSameConstructAs(Me));
    if (gyros.Count < 1) return;


    foreach (var gyro in gyros)
    {

        //偏航姿态调整
        if (Math.Abs(angle.X) < angleTolerance * angleToleranceMultiplier && angle.Z < 0)
        {
            gyro.SetValue<float>("Yaw", 0);//偏航
            gyro.SetValue<float>("Roll", 0);//翻滚
        }
        else
        {
            if(angle.Z < 0)
            {
                gyro.SetValue<float>("Yaw", -Convert.ToSingle(angle.X) * angleMultiplier2);
            }
            else
            {
            }
            gyro.GyroOverride = true;
        }

        //俯仰姿态调整
        if (Math.Abs(angle.Y) < angleTolerance * angleToleranceMultiplier && angle.Z < 0)
        {
            gyro.SetValue<float>("Pitch", 0);//俯仰
            gyro.SetValue<float>("Roll", 0);//翻滚
        }
        else
        {
            if(angle.Z < 0)
            {
                gyro.SetValue<float>("Pitch", -Convert.ToSingle(angle.Y) * angleMultiplier2);
            }
            else
            {
                if (angle.Y >= 0) gyro.SetValue<float>("Pitch", -(-Convert.ToSingle(angle.Y) + 2) * angleMultiplier2);
                if (angle.Y < 0) gyro.SetValue<float>("Pitch", (Convert.ToSingle(angle.Y) + 2) * angleMultiplier2);
            }
            gyro.GyroOverride = true;
        }

        if (Math.Abs(angle.X) < angleTolerance * angleToleranceMultiplier && Math.Abs(angle.Y) < angleTolerance * angleToleranceMultiplier && angle.Z < 0)
        {
            gyro.SetValue<float>("Yaw", 0);//偏航
            gyro.SetValue<float>("Roll", 0);//翻滚
            gyro.SetValue<float>("Pitch", 0);//俯仰
            gyro.GyroOverride = false;
        }
    }
}

public void ShowDegree(Vector3D angle)
{
    double pitch_Double = Math.Round(Math.Asin(angle.Y) / Math.PI * 180);
    double yaw_Double = Math.Round(Math.Asin(angle.X) / Math.PI * 180);

    GetBrakePanel("\n",true);
    GetBrakePanel($"Pitch = {pitch_Double} deg\n",true);
    GetBrakePanel($"Yaw = {yaw_Double} deg\n",true);
}

public float GetMaxThrustsPower(out string thrustDirection_String)
{
    float forwardPower_Float = ThrustsPower(Vector3I.Forward);
    float downPower_Float = ThrustsPower(Vector3I.Down);
    float backwardPower_Float = ThrustsPower(Vector3I.Backward);
    float currentThrustsPower_Float = 0;

    if (forwardPower_Float > downPower_Float)
    {
        thrustDirection_String = forward_String;
        currentThrustsPower_Float = forwardPower_Float;
    }
    else
    {
        thrustDirection_String = down_String;
        currentThrustsPower_Float = downPower_Float;
    }

    if(currentThrustsPower_Float < backwardPower_Float)
    {
        thrustDirection_String = backward_String;
        currentThrustsPower_Float = backwardPower_Float;
    }

    return currentThrustsPower_Float;
}

public float ThrustsPower(Vector3I thrustDirection_Vector3I)
{
    List<IMyThrust> thrusts = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType(thrusts, block => block.IsSameConstructAs(Me) && block.GridThrustDirection == thrustDirection_Vector3I);

    float thrustsPower_Float = 0;
    foreach(var thrust in thrusts)
    {
        thrustsPower_Float += thrust.MaxEffectiveThrust;
    }
    return thrustsPower_Float;
}

public float BrakeDistance(Vector3I thrustDirection)
{
    List<IMyThrust> thrusts = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType(thrusts, block => block.IsSameConstructAs(Me) && block.GridThrustDirection == thrustDirection);
    if (thrusts.Count < 1) return 0;

    float maxEffectiveThrust_Total_Float = 0;

    foreach (var thrust in thrusts)
    {
        maxEffectiveThrust_Total_Float += thrust.MaxEffectiveThrust;
    }

    float totalMass_Float = remoteControl1.CalculateShipMass().TotalMass;
    if (totalMass_Float == 0) totalMass_Float = 1;

    float acc_Float = maxEffectiveThrust_Total_Float / totalMass_Float;
    float v_Float = Convert.ToSingle(remoteControl1.GetShipSpeed());

    float s_Float = (v_Float * v_Float) / (2 * acc_Float);

    return s_Float;
}

public void CheckCharge(out string tank_state, out string battery_state, out string thrust_state)
{

    int tankCounter = 0;
    List<IMyGasTank> tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, block => block.IsSameConstructAs(Me));
    if (tanks.Count != 0)
    {

        foreach (IMyGasTank tank in tanks)
        {
            if (tank.Stockpile)
            {
                tankCounter++;
            }
        }

        if (tankCounter != 0)
        {
            tank_state = "Tank = Off";
        }
        else
        {
            tank_state = "Tank = On";
        }

    }
    else
    {
        tank_state = "Tank = Null";
    }

    int batteryCounter = 0;
    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, block => block.IsSameConstructAs(Me));
    if (tanks.Count != 0)
    {

        foreach (IMyBatteryBlock batery in batteries)
        {
            if (batery.ChargeMode == ChargeMode.Recharge)
            {
                batteryCounter++;
            }
        }

        if (batteryCounter != 0)
        {
            battery_state = "Battery = Off";
        }
        else

        {
            battery_state = "Battery = On";
        }

    }
    else
    {
        battery_state = "Battery = Null";
    }

    int thrustCounter = 0;
    List<IMyThrust> thrusts = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusts, block => block.IsSameConstructAs(Me));
    if (tanks.Count != 0)
    {

        foreach (IMyThrust thrust in thrusts)
        {
            if (thrust.Enabled == false)
            {
                thrustCounter++;
            }
        }

        if (thrustCounter != 0)
        {
            thrust_state = "Thrust = Off";
        }
        else

        {
            thrust_state = "Thrust = On";
        }

    }
    else
    {
        thrust_state = "Thrust = Null";
    }

}

public void ButtonInstructions()
{
    StringBuilder str = new StringBuilder();

    str.Append("Group 1\n");
    str.Append("  1=1——Stop & Defult\n");
    str.Append("  2=2——Thrusts Off & Tanks On & Batteries On\n");
    str.Append("  3=3——Thrusts & Tanks & Batteries Off\n");
    str.Append("  4=4——Speed Limitation at 1m/s\n");
    str.Append("  5=5——Speed Limitation at Max\n");
    str.Append("  6=6——Speed Limitation -5m/s\n");
    str.Append("  7=7——Speed Limitation +5m/s\n");
    str.Append("  8=LF——Maintain Level Flight\n");
    str.Append("  9=FA——Fly Forward\n");

    str.Append(" \n");

    str.Append("Group 2\n");
    str.Append("  1=1——Stop & Defult\n");
    str.Append("  2=2——Thrusts Off & Tanks On & Batteries On\n");
    str.Append("  3=3——Thrusts & Tanks & Batteries Off\n");
    str.Append("  4=4——Speed Limitation = 1m/s\n");
    str.Append("  5=5——Speed Limitation = Max\n");
    str.Append("  6=AD——Adjust Posture\n");
    str.Append("  7=(Null)\n");
    str.Append("  8=Dn——Straight Down\n");
    str.Append("  9=Up——Straight Up\n");


    string buttonInstructions_Value;
    GetConfiguration_from_MeCustomData(shipInformation_Section, buttonInstructions_Key, out buttonInstructions_Value);
    List<IMyTextPanel> panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels, panel => panel.CustomData == buttonInstructions_Value);

    if (buttonInstructions_Value == "" || buttonInstructions_Value == null) return;

    foreach (var panel in panels)
    {
        panel.WriteText(str.ToString());
        panel.ContentType = ContentType.TEXT_AND_IMAGE;
        panel.FontSize = 0.8f;
        panel.FontColor = new Color(0, 130, 255);
    }

}

public void DynamicTitle()
{
    counter++;
    switch (counter)
    {
        case 1:
            panel1.WriteText(" \n", false);
            Me.GetSurface(0).WriteText(" \n", false);
            break;
        case 2:
            panel1.WriteText("******************************************************\n", false);
            Me.GetSurface(0).WriteText("************************************************************\n", false);
            counter = 0;
            break;
    }
}

public void GetConfiguration_from_MeCustomData(string section, string key, out string value)
{

    // This time we _must_ check for failure since the user may have written invalid ini.
    MyIniParseResult result;
    if (!iniInformation.TryParse(Me.CustomData, out result))
        throw new Exception(result.ToString());

    string DefaultValue = "";

    // Read the integer value. If it does not exist, return the default for this value.
    value = iniInformation.Get(section, key).ToString(DefaultValue);
}

public void WriteConfiguration_to_MeCustomData(string section, string key, string value)
{
    iniInformation.Set(section, key, value);
    Me.CustomData = iniInformation.ToString();
}

public void BatteryCharge(bool charge)
{

    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, block => block.IsSameConstructAs(Me));
    if (batteries.Count == 0) return;

    if (charge)
    {
        foreach (IMyBatteryBlock battery in batteries)
        {

            battery.ChargeMode = ChargeMode.Recharge;

        }

    }
    else
    {
        foreach (IMyBatteryBlock battery in batteries)
        {

            battery.ChargeMode = ChargeMode.Auto;

        }

    }

}// End BatteryCharge

public void HydrogenCharge(bool charge)
{

    List<IMyGasTank> tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, block => block.IsSameConstructAs(Me));

    if (tanks.Count > 0)
    {
        foreach (IMyGasTank tank in tanks)
        {
            if (charge && !tank.Stockpile)
            {
                tank.Stockpile = true;
            }
            if (!charge && tank.Stockpile)
            {
                tank.Stockpile = false;
            }
        }
    }

}// End HydrogenCharge

public void ThrustEnabled(bool true_or_false)
{

    List<IMyThrust> thrusts = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusts, block => block.IsSameConstructAs(Me));
    if (thrusts.Count == 0) return;

    foreach (IMyThrust thrust in thrusts)
    {
        if (true_or_false)
        {
            thrust.Enabled = true;
            thrust.ThrustOverridePercentage = 0;
        }
        else
        {
            thrust.Enabled = false;
        }

    }

}// End ThrustEnabled

public void FlyStable()
{

    Vector3D planetPosition_Vector3D;
    Point3D planetPosition_Point3D, currentPosition_Point3D;
    bool isInGravity = remoteControl1.TryGetPlanetPosition(out planetPosition_Vector3D);
    if (isInGravity == false) return;
    Vector3D_to_Point3D(planetPosition_Vector3D, out planetPosition_Point3D);
    Vector3D currentPosition_Vector3D = remoteControl1.GetPosition();
    Vector3D_to_Point3D(currentPosition_Vector3D, out currentPosition_Point3D);
    Point3D upVector_Point3D;
    UnitVector(currentPosition_Point3D, planetPosition_Point3D, out upVector_Point3D);
    Vector3D upVector_Vector3D;
    Point3D_to_Vector3D(upVector_Point3D, out upVector_Vector3D);

    //转换为飞船旋转参数（坐标系转换）
    MatrixD refLookAtMatrix = MatrixD.CreateLookAt(new Vector3D(), remoteControl1.WorldMatrix.Forward, remoteControl1.WorldMatrix.Up);
    //MatrixD refLookAtMatrix = MatrixD.CreateLookAt(remoteControl.GetPosition(), remoteControl.WorldMatrix.Forward, remoteControl.WorldMatrix.Up);
    //驾驶舱的当前角度
    MeAngle = Vector3D.TransformNormal(upVector_Vector3D, refLookAtMatrix);


    Echo($"MeAngle:{MeAngle}");
    StringBuilder str = new StringBuilder();
    str.Append("\n");
    str.Append("\n");
    str.Append("Direction:");
    str.Append("\n");
    str.Append("X=");
    str.Append(MeAngle.X);
    str.Append("\n");
    str.Append("Y=");
    str.Append(MeAngle.Y);
    str.Append("\n");
    str.Append("Z=");
    str.Append(MeAngle.Z);
    //panel.WriteText(str, true);

    //Vector3 targetPYR = new Vector3(remoteControl1.RotationIndicator, remoteControl1.RollIndicator);
    ////角速度减速
    //targetPYR += Vector3D.TransformNormal(remoteControl1.GetShipVelocities().AngularVelocity, refLookAtMatrix) * 0.01;


    //获取所有陀螺仪
    List<IMyGyro> gyros = new List<IMyGyro>();
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, block => block.IsSameConstructAs(Me));
    if (gyros.Count < 1) return;
    foreach (var gyro in gyros)
    {
        string mode_Value;
        GetConfiguration_from_MeCustomData(mission_Section, mode_Key, out mode_Value);
        //panel.WriteText($"\nFlystable is {mode_Value}", true);

        if (mode_Value != mode_Stop)
        {
            gyro.GyroOverride = false;
            //仰角姿态矫正
            if (Math.Abs(MeAngle.Z) < angleTolerance && MeAngle.Y > 0)
            {
                gyro.SetValue<float>("Pitch", 0);
            }
            else if (MeAngle.Z > 0)
            {
                gyro.SetValue<float>("Pitch", Convert.ToSingle(MeAngle.Z) * angleMultiplier);
                gyro.GyroOverride = true;
            }
            else if (MeAngle.Z < 0)
            {
                gyro.SetValue<float>("Pitch", Convert.ToSingle(MeAngle.Z) * angleMultiplier);
                gyro.GyroOverride = true;

            }
            //翻转姿态矫正
            if (Math.Abs(MeAngle.X) < angleTolerance && MeAngle.Y > 0)
            {
                gyro.SetValue<float>("Roll", 0);
            }
            else if (MeAngle.X > 0)
            {

                gyro.SetValue<float>("Roll", Convert.ToSingle(MeAngle.X) * angleMultiplier);
                gyro.GyroOverride = true;

            }
            else if (MeAngle.X < 0)
            {

                gyro.SetValue<float>("Roll", Convert.ToSingle(MeAngle.X) * angleMultiplier);
                gyro.GyroOverride = true;

            }

        }
        else
        {
            gyro.SetValue<float>("Yaw", 0);
            gyro.SetValue<float>("Roll", 0);
            gyro.SetValue<float>("Pitch", 0);
            gyro.GyroOverride = false;

        }
        //panel.WriteText($"\nPitch is {gyro.GetValue<float>("Pitch")}", true);
        //panel.WriteText($"\nRoll is {gyro.GetValue<float>("Roll")}", true);
        //panel.WriteText($"\nYaw is {gyro.GetValue<float>("Yaw")}", true);


    }

}// End flystable

public void GetSurface()
{

    List<IMyCockpit> cockpits = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits, block => block.IsSameConstructAs(Me));
    Echo($"Cockpit Numbers = {cockpits.Count}");
    if (cockpits.Count < 1) return;


    int t = 0;
    foreach (IMyCockpit cp in cockpits)
    {
        if (cp.IsMainCockpit)
        {
            t++;
            cockpit = cp;
            panel1 = cockpit.GetSurface(0);
            Echo("Panel1 Got!");
            panel2 = cockpit.GetSurface(1);
            Echo("Panel3 Got!");

            panel1.ContentType = ContentType.TEXT_AND_IMAGE;
            panel2.ContentType = ContentType.TEXT_AND_IMAGE;
        }

    }

    Echo($"Used Cockpit = {t}");
}

public void GetBrakePanel(string Value, bool append_Bool)
{
    string panelName_String;
    GetConfiguration_from_MeCustomData(shipInformation_Section, brakePanel_Key, out panelName_String);
    if (panelName_String == "" || panelName_String == null) return;

    List<IMyTextPanel> panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, block => block.IsSameConstructAs(Me) && block.CustomName == panelName_String);
    if (panels.Count < 1) return;

    foreach(var panel in panels)
    {
        panel.ContentType = ContentType.TEXT_AND_IMAGE;
        panel.FontSize = 1.2f;
        panel.FontColor =new Color(0, 130, 255);
        panel.WriteText(Value, append_Bool);
    }
}

public void GetRemoteControl()
{

    Echo("RemoteContol ...");

    List<IMyRemoteControl> remoteControls = new List<IMyRemoteControl>();
    GridTerminalSystem.GetBlocksOfType(remoteControls, r => r.IsSameConstructAs(Me));
    if (remoteControls.Count == 0)
    {
        Echo("No RemoteContol!");
        return;
    }

    //string autoPilotName1, autoPilotName2;
    //GetConfiguration_from_MeCustomData(shipInformation_Section, remoteControl1_Key, out autoPilotName1);
    //GetConfiguration_from_MeCustomData(shipInformation_Section, remoteControl2_Key, out autoPilotName2);

    foreach (IMyRemoteControl remoteControlTemp in remoteControls)
    {
        if (!remoteControlTemp.IsMainCockpit)
        {
            remoteControl1 = remoteControlTemp;
            Echo("RemoteContol Got!");
        }

        //if (remoteControlTemp.CustomName == autoPilotName1)
        //{
        //    remoteControl1 = remoteControlTemp;
        //    Echo("RemoteContol Got!");
        //}
        //else if (remoteControlTemp.CustomName == autoPilotName2)
        //{
        //    remoteControl2 = remoteControlTemp;
        //}

    }
    Echo("RemoteContol ...");
}

public void GetSensor()
{

    Echo("Sensor ...");

    List<IMySensorBlock> sensors = new List<IMySensorBlock>();
    GridTerminalSystem.GetBlocksOfType(sensors, s => s.IsSameConstructAs(Me));

    string sensor_Value;
    GetConfiguration_from_MeCustomData(shipInformation_Section, sensor_Key, out sensor_Value);

    int counter1 = 0;

    foreach (IMySensorBlock sensorTemp in sensors)
    {
        if (sensorTemp.CustomName == sensor_Value)
        {
            sensor = sensorTemp;
            Echo("Sensor Got!");
            counter1++;

        }
    }

    if (counter1 == 0)
    {
        Echo("Sensor Not Found!!");
    }

    Echo("Sensor ...");

}

public void GetCamera()
{
    Echo("Camera ...");

    List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
    GridTerminalSystem.GetBlocksOfType(cameras, block => block.IsSameConstructAs(Me));

    string camera_Value;
    GetConfiguration_from_MeCustomData(shipInformation_Section, camera_Key, out camera_Value);

    int counter1 = 0;

    foreach (var cam in cameras)
    {
        if (cam.CustomName == camera_Value)
        {
            camera = cam;
            Echo("Camera Got!!");
            counter1++;
        }
    }

    if (counter1 == 0)
    {
        Echo("Camera Not Found!!");
    }

    Echo("Camera ...");
}


// Math
public void UnitVector(Point3D p1, Point3D p2, out Point3D p3)
{

    double x, y, z, d;

    x = p1.X - p2.X;
    y = p1.Y - p2.Y;
    z = p1.Z - p2.Z;

    d = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2) + Math.Pow(z, 2));

    p3.X = x / d;
    p3.Y = y / d;
    p3.Z = z / d;

}

public void Vector3D_to_Point3D(Vector3D p1, out Point3D p2)
{
    p2.X = p1.X;
    p2.Y = p1.Y;
    p2.Z = p1.Z;
}

public void Point3D_to_Vector3D(Point3D p1, out Vector3D p2)
{
    p2.X = p1.X;
    p2.Y = p1.Y;
    p2.Z = p1.Z;
}

public void CalculateDistance(Point3D p1, Point3D p2, out double distance)
{

    double x = p1.X - p2.X;
    double y = p1.Y - p2.Y;
    double z = p1.Z - p1.Z;

    distance = Math.Sqrt((x * x) + (y * y) + (x * x));

}
// Math


public void DoorControl()
{
    List<IMySensorBlock> sensors = new List<IMySensorBlock>();
    GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(sensors);
    List<IMyDoor> doors = new List<IMyDoor>();
    GridTerminalSystem.GetBlocksOfType<IMyDoor>(doors);


    if (sensors.Count != 0 && doors.Count != 0)
    {

        foreach (IMySensorBlock sensor in sensors)
        {
            foreach (IMyDoor door in doors)
            {

                if (door.CustomName == sensor.CustomName)
                {
                    if (sensor.IsActive)
                    {
                        door.OpenDoor();
                    }
                    else
                    {
                        door.CloseDoor();
                    }
                }

            }
        }

    }

}

public void FlyForward()
{
    //  Brake
    if (cockpit.MoveIndicator.Z > 0 && BrakeDistance(Vector3I.Forward) > breakLimit_Float)
    {
        WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Brake);
        direction_MatrixD = MatrixD.CreateLookAt(new Vector3D(), remoteControl1.WorldMatrix.Forward, remoteControl1.WorldMatrix.Up);
        return;
    }


    //Fly_to_Target_GPS();

    // Get Speed
    string speedLimit_Value;
    GetConfiguration_from_MeCustomData(mission_Section, speed_Key, out speedLimit_Value);
    float speedLimit_Float = float.Parse(speedLimit_Value);
    double shipSpeed_double = cockpit.GetShipSpeed();
    // Get Speed End

    // Confirm Thrusts
    List<IMyThrust> thrusts = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusts, block => block.IsSameConstructAs(Me));
    int thrustCounter1 = 0, thrustCounter2 = 0;
    foreach (IMyThrust thrust in thrusts)
    {
        if (thrust.GridThrustDirection == Vector3I.Backward)
        {
            thrustCounter1++;
        }

        if (thrust.GridThrustDirection == Vector3I.Forward)
        {
            thrustCounter2++;
        }
    }
    if (thrustCounter1 == 0 || thrustCounter1 == 0) return;
    // Confirm Thrusts End


    if (shipSpeed_double - speedLimit_Float < 0)
    {
        // accelerate
        foreach (IMyThrust thrust in thrusts)
        {
            if (thrust.GridThrustDirection == Vector3I.Forward)
            {
                thrust.Enabled = false;
            }
            if (thrust.GridThrustDirection == Vector3I.Backward)
            {
                thrust.Enabled = true;
                thrust.ThrustOverridePercentage = 1;
            }
        }

    }
    else if (shipSpeed_double - speedLimit_Float >= 0 && shipSpeed_double - speedLimit_Float <= 5)
    {
        // maintain
        foreach (IMyThrust thrust in thrusts)
        {
            if (thrust.GridThrustDirection == Vector3I.Forward)
            {
                thrust.Enabled = false;
            }
            if (thrust.GridThrustDirection == Vector3I.Backward)
            {
                thrust.Enabled = true;
                thrust.ThrustOverridePercentage = 0;
            }
        }
    }
    else
    {
        // decelerate
        foreach (IMyThrust thrust in thrusts)
        {
            if (thrust.GridThrustDirection == Vector3I.Forward)
            {
                thrust.Enabled = true;
                thrust.ThrustOverridePercentage = 0;
            }
            if (thrust.GridThrustDirection == Vector3I.Backward)
            {
                thrust.Enabled = false;
            }
        }
    }// End if


}// End FlyForward

public void Up()
{
    Vector3D planetPosition_Vector3D;
    bool isInGravity = remoteControl1.TryGetPlanetPosition(out planetPosition_Vector3D);

    // Get Speed
    string speedLimit_Value;
    GetConfiguration_from_MeCustomData(mission_Section, speed_Key, out speedLimit_Value);
    float speedLimit_Float = float.Parse(speedLimit_Value);
    double shipSpeed_double = cockpit.GetShipSpeed();
    // Get Speed End

    // Confirm Thrusts
    List<IMyThrust> thrusts = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusts, block => block.IsSameConstructAs(Me));
    int thrustCounter1 = 0, thrustCounter2 = 0;
    foreach (IMyThrust thrust in thrusts)
    {
        if (thrust.GridThrustDirection == Vector3I.Backward)
        {
            thrustCounter1++;
        }

        if (thrust.GridThrustDirection == Vector3I.Forward)
        {
            thrustCounter2++;
        }
    }
    if (thrustCounter1 == 0 || thrustCounter2 == 0) return;
    // Confirm Thrusts End

    if (isInGravity == true)
    {
        if (shipSpeed_double - speedLimit_Float < 0)
        {
            foreach (IMyThrust thrust in thrusts)
            {
                if (thrust.GridThrustDirection == Vector3I.Up)
                {
                    thrust.Enabled = false;
                }
                if (thrust.GridThrustDirection == Vector3I.Down)
                {
                    thrust.Enabled = true;
                    thrust.ThrustOverridePercentage = 1;
                }
            }

        }
        else if (shipSpeed_double - speedLimit_Float >= 0 && shipSpeed_double - speedLimit_Float < 5)
        {
            foreach (IMyThrust thrust in thrusts)
            {
                if (thrust.GridThrustDirection == Vector3I.Up)
                {
                    thrust.Enabled = false;
                }
                if (thrust.GridThrustDirection == Vector3I.Down)
                {
                    thrust.Enabled = true;
                    thrust.ThrustOverridePercentage = 0;
                }
            }

        }
        else
        {
            foreach (IMyThrust thrust in thrusts)
            {
                if (thrust.GridThrustDirection == Vector3I.Up)
                {
                    thrust.Enabled = true;
                    thrust.ThrustOverridePercentage = 1;
                }
                if (thrust.GridThrustDirection == Vector3I.Down)
                {
                    thrust.Enabled = false;
                    thrust.ThrustOverridePercentage = 0;
                }
            }

        }

    }
    else
    {
        WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Stop);
        Mode_Stop();
    }


}// ENd Up

public void Down()
{

    FlyStable();

    double distance;
    AltitudeFinding(out distance);



    panel1.WriteText($"Elevation={distance}\n", true);

    if (distance <= 0)
    {
        remoteControl1.DampenersOverride = true;
        WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Stop);
        Mode_Stop();
    }
    else
    {

        string thrustPowerCorrectionFactor_Value;

        GetConfiguration_from_MeCustomData(shipInformation_Section, thrustPowerCorrectionFactor_Key, out thrustPowerCorrectionFactor_Value);

        double tpcf = Double.Parse(thrustPowerCorrectionFactor_Value);

        List<IMyThrust> thrusts = new List<IMyThrust>();
        GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusts, block => block.IsSameConstructAs(Me));
        int thrustCounter = 0;
        float thrustPower_Float = 0;
        foreach (IMyThrust thrust in thrusts)
        {
            if (thrust.GridThrustDirection == Vector3I.Down)
            {
                thrustCounter++;
                thrustPower_Float += thrust.MaxEffectiveThrust;
            }
        }

        if (thrustCounter == 0)
        {
            remoteControl1.SpeedLimit = 0;
            remoteControl1.DampenersOverride = true;
            WriteConfiguration_to_MeCustomData(mission_Section, mode_Key, mode_Stop);
        }
        else
        {


            float shipMass_Float = cockpit.CalculateShipMass().TotalMass;
            double accerlerationOfGravity_Double = cockpit.GetNaturalGravity().Length();
            float trueThrustPower_Float = thrustPower_Float - shipMass_Float * ((float)accerlerationOfGravity_Double);


            if (trueThrustPower_Float <= 0)
            {
                remoteControl1.DampenersOverride = true;
            }
            else
            {

                float distance2_Float = (float)remoteControl1.GetShipSpeed() * (float)remoteControl1.GetShipSpeed() / (2 * ((float)tpcf) * trueThrustPower_Float / shipMass_Float);
                Echo($"distance2_Float = {distance2_Float}");
                Echo($"distance = {distance}");



                if (distance2_Float > distance)
                {
                    remoteControl1.DampenersOverride = true;
                }
                else
                {
                    remoteControl1.DampenersOverride = false;
                }


                string finalRange_Value, finalRangeSpeed_Value;
                GetConfiguration_from_MeCustomData(shipInformation_Section, finalRange_Key, out finalRange_Value);
                double finalRange_Double = Convert.ToDouble(finalRange_Value);
                GetConfiguration_from_MeCustomData(shipInformation_Section, finalRangeSpeed_Key, out finalRangeSpeed_Value);
                double finalRangeSpeed_Dloat = Convert.ToDouble(finalRangeSpeed_Value);

                if (distance < finalRange_Double)
                {
                    if (remoteControl1.GetShipSpeed() > finalRangeSpeed_Dloat)
                    {
                        remoteControl1.DampenersOverride = true;
                    }
                    else
                    {
                        remoteControl1.DampenersOverride = false;
                    }
                }

            }

        }

    }

}// End Down

public void AltitudeFinding(out double distance)
{
    camera.EnableRaycast = true;

    string detectionRange_Value, finalRange_Value;
    GetConfiguration_from_MeCustomData(shipInformation_Section, detectionRange_Key, out detectionRange_Value);
    MyDetectedEntityInfo info;

    GetConfiguration_from_MeCustomData(shipInformation_Section, finalRange_Key, out finalRange_Value);
    double finalRange_Double = Convert.ToDouble(finalRange_Value);

    if (distanceCounter != 0)
    {
        info = camera.Raycast(distanceTemp, 0, 0);
    }
    else
    {
        info = camera.Raycast(Convert.ToDouble(detectionRange_Value), 0, 0);
    }


    if (!info.IsEmpty())
    {

        Vector3D hitPosition_Vector3D = info.HitPosition.Value;
        Point3D hitPosition_Point3D, cameraPosition_Point3D;
        Vector3D_to_Point3D(hitPosition_Vector3D, out hitPosition_Point3D);
        Vector3D cameraPosition_Vector3D = camera.GetPosition();
        Vector3D_to_Point3D(cameraPosition_Vector3D, out cameraPosition_Point3D);
        CalculateDistance(hitPosition_Point3D, cameraPosition_Point3D, out distance);

        string elevationOffset_Value;
        GetConfiguration_from_MeCustomData(shipInformation_Section, elevationOffset_Key, out elevationOffset_Value);
        double elevationOffset_Double = Convert.ToDouble(elevationOffset_Value);

        distance = distance - elevationOffset_Double;
        distanceTemp = distance;


    }
    else
    {
        distance = Convert.ToDouble(detectionRange_Value);
    }

    if (distanceCounter == 0)
    {
        distanceTemp = distance;
        distanceCounter++;

    }
    else
    {
        distance = distanceTemp;
    }


}

public void Fly_to_Target_GPS()
{

    if (!remoteControl1.CurrentWaypoint.IsEmpty())
    {
        if (!remoteControl1.IsAutoPilotEnabled)
        {
            string speedLimit_Value;
            GetConfiguration_from_MeCustomData(shipInformation_Section, maxSpeed_Key, out speedLimit_Value);
            float speedLimit_Float = Convert.ToSingle(speedLimit_Value);

            remoteControl1.FlightMode = FlightMode.OneWay;
            remoteControl1.Direction = Base6Directions.Direction.Forward;
            remoteControl1.SpeedLimit = speedLimit_Float;
            remoteControl1.SetAutoPilotEnabled(true);

        }
    }
}

public void SpeedChange(int x)
{
    string speedLimit_Value;
    GetConfiguration_from_MeCustomData(mission_Section, speed_Key, out speedLimit_Value);
    float speedLimit_Float = float.Parse(speedLimit_Value);

    switch (x)
    {
        case 1://Speed Limitation -5m/s
            if (speedLimit_Float - 5 < 0)
            {
                speedLimit_Float = 0;
            }
            else
            {
                speedLimit_Float = speedLimit_Float - 5;
            }
            break;
        case 2://Speed Limitation +5m
            speedLimit_Float = speedLimit_Float + 5;
            break;
        case 3://Speed Limitation = 1m/s
            speedLimit_Float = 1;
            break;
        case 4://Speed Limitation = Max
            string maxSpeed_String;
            GetConfiguration_from_MeCustomData(shipInformation_Section, maxSpeed_Key, out maxSpeed_String);
            speedLimit_Float = float.Parse(maxSpeed_String);
            break;
    }

    WriteConfiguration_to_MeCustomData(mission_Section, speed_Key, speedLimit_Float.ToString());

}

public void Logic()
{
    string mode;
    GetConfiguration_from_MeCustomData(mission_Section, mode_Key, out mode);
    panel1.WriteText($"Mode = {mode}\n", true);
    panel1.WriteText($"Sensor = {sensor.IsActive}\n", true);
    Echo("LOGIC");

    switch (mode)
    {
        case mode_Stop:
            Echo($"{mode}");
            break;
        case mode_ThrustsOff:
            Echo($"{mode}");
            break;
        case mode_Charge:
            Echo($"{mode}");
            break;
        case mode_MaintainLevelFlight:
            Echo($"{mode}");
            if (sensor.IsActive)
            {
                WriteConfiguration_to_MeCustomData(mission_Section, speed_Key, "1");
                FlyStable();
                FlyForward();
            }
            else
            {
                FlyStable();
                FlyForward();

                double elevation_Double;
                string elevation_String;
                cockpit.TryGetPlanetElevation(0, out elevation_Double);
                GetConfiguration_from_MeCustomData(mission_Section, "Altitude", out elevation_String);

                if (fisrtRun || elevation_String == "")
                {
                    WriteConfiguration_to_MeCustomData(mission_Section, "Altitude", elevation_Double.ToString());
                    fisrtRun = false;
                }
                else
                {
                    if (elevation_Double > Convert.ToDouble(elevation_String) + 50)
                    {
                        remoteControl1.DampenersOverride = false;
                    }
                    else if (elevation_Double < Convert.ToDouble(elevation_String) - 50)
                    {
                        remoteControl1.DampenersOverride = true;
                    }
                }

            }
            break;
        case mode_FlyForward:
            Echo($"{mode}");
            if (sensor.IsActive)
            {
                WriteConfiguration_to_MeCustomData(mission_Section, speed_Key, "1");
                FlyForward();
            }
            else
            {
                FlyForward();
            }
            break;
        case mode_Up:
            Echo($"{mode}");
            string speed_String;
            GetConfiguration_from_MeCustomData(mission_Section, speed_Key, out speed_String);
            double speed_double = float.Parse(speed_String);

            if (speed_double > 5)
            {
                Up();
            }
            break;
        case mode_Down:
            Echo($"{mode}");
            Down();
            break;
        case mode_Adjust:
            Echo($"{mode}");
            FlyStable();
            break;
        case mode_FlytoTargetGPS:
            Fly_to_Target_GPS();
            break;
        case mode_Brake:
            AdjustAttitudeToBrake();
            break;
    }
}

public void Display_InventoryUse()
{

    float totalVolume = 0;
    float currentVolume = 0;
    float percentage_Float = 0;
    string percentage_String;
    float gyroPower_Float = 0.15f;
    string gyroPower_String;

    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, block => block.IsSameConstructAs(Me) && block.HasInventory && block.CustomData != "X" && block.BlockDefinition.ToString().IndexOf("Tank") == -1);

    foreach (var block in blocks)
    {
        totalVolume += block.GetInventory().MaxVolume.ToIntSafe();
        currentVolume += block.GetInventory().CurrentVolume.ToIntSafe();
        Echo(block.BlockDefinition.ToString());
    }

    percentage_Float = currentVolume / totalVolume;

    percentage_String = (percentage_Float * 100).ToString("F0");

    panel2.FontSize = 1.5f;
    panel2.WriteText("Inventory Usage\n");
    panel2.WriteText($"={percentage_String}%\n", true);

    //show Inventory Usage
    ///////////////////////////////////////////////
    //show Gyroscope Power

    string ratio_String;
    GetConfiguration_from_MeCustomData(shipInformation_Section, cargoMassRatio_Key, out ratio_String);
    float ratio_Float = float.Parse(ratio_String);

    float cargoMass_Float = ratio_Float * totalVolume * 1000;
    //Echo($"Total Volume = {totalVolume}");
    //Echo($"Cargo Mass = {cargoMass_Float}");


    List<IMyCockpit> cockpits = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits, cockpit => cockpit.IsSameConstructAs(Me));
    if (cockpits.Count == 0)
    {
        Echo("No Cockpit!!");
    }
    else
    {
        float shipAndCargoMass_Float = cockpits[0].CalculateShipMass().BaseMass + cargoMass_Float;
        gyroPower_Float = cockpits[0].CalculateShipMass().TotalMass / shipAndCargoMass_Float;
        if (gyroPower_Float > 1)
        {
            gyroPower_Float = 1;
        }
        List<IMyGyro> gyros = new List<IMyGyro>();
        GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros, gyro => gyro.IsSameConstructAs(Me));
        if (gyros.Count == 0)
        {
            Echo("No Gyros!!");
        }
        else
        {
            Echo($"Got {gyros.Count} Gyros");
            foreach (var gyro in gyros)
            {
                gyro.GyroPower = gyroPower_Float;
            }
            gyroPower_String = (gyroPower_Float * 100).ToString("F0");
            panel2.WriteText("Gyroscope Power\n", true);
            panel2.WriteText($"={gyroPower_String}%\n", true);

        }

        double ag = cockpits[0].GetNaturalGravity().Length();
        panel2.WriteText("Acceleration of Gravity\n", true);
        panel2.WriteText($"={Math.Round(ag,2)}\n", true);

    }
}

public void AntennaHudText()
{

    List<IMyCockpit> cockpits = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType(cockpits, cockpit => cockpit.IsSameConstructAs(Me));

    IMyCockpit cockpit1 = null;
    foreach (var cockpit in cockpits)
    {
        if (cockpit.IsMainCockpit)
        {
            cockpit1 = cockpit;
        }
    }

    if (cockpit1 == null) return;

    List<IMyRadioAntenna> antennas = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType(antennas, antenna => antenna.IsSameConstructAs(Me));
    if (antennas.Count == 0) return;

    string speed_String;
    GetConfiguration_from_MeCustomData(mission_Section, speed_Key, out speed_String);

    string mode_String;
    GetConfiguration_from_MeCustomData(mission_Section, mode_Key, out mode_String);

    foreach (var antenna in antennas)
    {
        if (cockpit1.GetShipSpeed() != 0)
        {
            antenna.HudText = "<Mode:" + mode_String + "> <SpeedLimit:" + speed_String + "m/s>";
        }
        else
        {
            antenna.HudText = "Parking";
        }
    }

}

public string ShowBrakeDistance(Vector3I thrustsDirection)
{
    string brakeDistance_String;
    if (BrakeDistance(thrustsDirection) > 1000)
    {
        brakeDistance_String = Math.Round(BrakeDistance(thrustsDirection) / 1000, 2).ToString() + " km";
        return brakeDistance_String;
    }
    else
    {
        brakeDistance_String = Math.Round(BrakeDistance(thrustsDirection), 0).ToString() + " m";
        return brakeDistance_String;
    }
}

public void BrakePanel()
{
    GetBrakePanel("", false);

    StringBuilder str = new StringBuilder();

    str.Append("BrakeDistance: ");
    str.Append(" \n");
    str.Append("Forward = ");
    str.Append(ShowBrakeDistance(Vector3I.Forward));
    str.Append("\n");
    str.Append("Down = ");
    str.Append(ShowBrakeDistance(Vector3I.Down));
    str.Append("\n");
    str.Append("Back = ");
    str.Append(ShowBrakeDistance(Vector3I.Backward));
    str.Append("\n");
    str.Append("\n");
    str.Append("ThrustsForwardMax = ");
    str.Append(ThrustsPower(Vector3I.Forward) / 1000);
    str.Append(" kN");
    str.Append("\n");
    str.Append("ThrustsDownMax = ");
    str.Append(ThrustsPower(Vector3I.Down) / 1000);
    str.Append(" kN");
    str.Append("\n");
    str.Append("ThrustsBackMax = ");
    str.Append(ThrustsPower(Vector3I.Backward) / 1000);
    str.Append(" kN");
    str.Append("\n");

    GetBrakePanel(str.ToString(), true);
}

public void MainScreen()
{
    string speedLimit_Value;
    GetConfiguration_from_MeCustomData(mission_Section, speed_Key, out speedLimit_Value);
    panel1.WriteText($"Speed Limitation = {speedLimit_Value}m/s\n", true);
    string tank_state, battery_state, thrust_state;
    CheckCharge(out tank_state, out battery_state, out thrust_state);
    panel1.WriteText($"{tank_state}\n{battery_state}\n{thrust_state}\n", true);
}

public void CopyMainScreen()
{
    string panelName_String;
    GetConfiguration_from_MeCustomData(shipInformation_Section, copyMainScreen_Key, out panelName_String);
    if (panelName_String == "" || panelName_String == null) return;

    List<IMyTextPanel> panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, block => block.IsSameConstructAs(Me) && block.CustomName == panelName_String);

    foreach(var panel in panels)
    {
        panel.ContentType = ContentType.TEXT_AND_IMAGE;
        panel.FontColor = new Color(0, 130, 255);
        panel.WriteText(panel1.GetText());
    }
}

public void Main(string argument, UpdateType updateSource)
{

    Echo("main");

    GetRemoteControl();

    GetSensor();

    GetCamera();

    GetSurface();

    DynamicTitle();

    BrakePanel();

    ButtonInstructions();

    Echo("main");
    ButtonOnShip(argument);

    MainScreen();

    Display_InventoryUse();

    AntennaHudText();

    Logic();

    CopyMainScreen();
}