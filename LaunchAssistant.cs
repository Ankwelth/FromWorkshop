/*
 *   R e a d m e
 *   -----------
 *   Setup: 
 *   1) Programmable block with this script
 *   2) Run argument as "toggle" to turn script on/off
 *   3) ?
 *   4) Profit
 *   
 *   Obviously, you need thrusters, gyros, and a ship controller (Cockpit/Flight seat/remote control)
 *   
 *   Features:
 *   Starts a timer once zero G has been reached.  Name it [LaunchAssist]. (Yes, you need the brackets)
 *   Checks which direction the ship has the strongest available thrust, and determines that side should be the "bottom"
 *   Uses gravity alignment code to orient ship to have strongest thrusters facing down
 *   If in mid-flight, this changes (eg. Atmo thrusters get weaker as you climb) ship re-orients to use new strongest set
 *      
 */
float TargetSpeed = 98;
List<IMyShipController> cons = new List<IMyShipController>();
List<IMyTextPanel> Lcds = new List<IMyTextPanel>();
IMyShipController controller;
bool Launch = false;
bool InAtmo;
bool PrevInAtmo;

List<IMyThrust> DorsalThrust = new List<IMyThrust>();
List<IMyThrust> VentralThrust = new List<IMyThrust>();
List<IMyThrust> PortThrust = new List<IMyThrust>();
List<IMyThrust> StarboardThrust = new List<IMyThrust>();
List<IMyThrust> ForeThrust = new List<IMyThrust>();
List<IMyThrust> AftThrust = new List<IMyThrust>();
List<IMyThrust> Thrusters = new List<IMyThrust>();
List<IMyThrust> Lifters = new List<IMyThrust>();
List<IMyTimerBlock> Timer = new List<IMyTimerBlock>();


IMyThrust ReferenceBlock;
List<IMyGyro> Gyros = new List<IMyGyro>();
double CTRL_COEFF = 0.5; //Set lower if overshooting, set higher to respond quicker

public void Setup()
{
    GridTerminalSystem.GetBlocksOfType<IMyShipController>(cons);
    controller = cons[0];
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    GridTerminalSystem.GetBlocksOfType(Gyros, block => block.CubeGrid == Me.CubeGrid); if (Gyros.Count == 0) { Echo("Error: No Gyroscopes Found"); }
    GridTerminalSystem.GetBlocksOfType(DorsalThrust, x => Base6Directions.GetOppositeDirection(x.Orientation.Forward) == Base6Directions.GetOppositeDirection(controller.Orientation.Up)); if (DorsalThrust.Count == 0) { Echo("Warning: No Dorsal Thrusters Detected"); }
    GridTerminalSystem.GetBlocksOfType(VentralThrust, x => Base6Directions.GetOppositeDirection(x.Orientation.Forward) == controller.Orientation.Up); if (VentralThrust.Count == 0) { Echo("Warning: No Ventral Thrusters Detected"); }
    GridTerminalSystem.GetBlocksOfType(PortThrust, x => Base6Directions.GetOppositeDirection(x.Orientation.Forward) == Base6Directions.GetOppositeDirection(controller.Orientation.Left)); if (PortThrust.Count == 0) { Echo("Warning: No Port Thrusters Detected"); }
    GridTerminalSystem.GetBlocksOfType(StarboardThrust, x => Base6Directions.GetOppositeDirection(x.Orientation.Forward) == controller.Orientation.Left); if (StarboardThrust.Count == 0) { Echo("Warning: No Starboard Thrusters Detected"); }
    GridTerminalSystem.GetBlocksOfType(ForeThrust, x => Base6Directions.GetOppositeDirection(x.Orientation.Forward) == Base6Directions.GetOppositeDirection(controller.Orientation.Forward)); if (ForeThrust.Count == 0) { Echo("Warning: No Fore Thrusters Detected"); }
    GridTerminalSystem.GetBlocksOfType(AftThrust, x => Base6Directions.GetOppositeDirection(x.Orientation.Forward) == controller.Orientation.Forward); if (AftThrust.Count == 0) { Echo("Warning: No Aft Thrusters Detected"); }
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(Thrusters);
    GridTerminalSystem.GetBlocksOfType(Timer, x => x.CustomName.StartsWith("[LaunchAssist]"));
}



public void Main(string argument)
{
    if (argument == "toggle") Launch = !Launch;
    if (argument == "toggle" && Launch == true) Setup();
    double Alt; bool inPlanet = cons[0].TryGetPlanetElevation(MyPlanetElevation.Surface, out Alt);
    PrevInAtmo = InAtmo;
    InAtmo = inPlanet;
    if (InAtmo == false && PrevInAtmo == true && Timer.Count > 0) { Timer[0].StartCountdown(); }
    if (Launch == false)
    {
        foreach (var c in cons) c.ControlThrusters = true;
        foreach (var t in Thrusters) t.ThrustOverridePercentage = 0;
        foreach (var g in Gyros) g.GyroOverride = false;
    }
    else
    {


        Echo("Altitue " + Convert.ToString(Alt));
        Echo("InPlanet = " + inPlanet);
        if (Alt > 200 && inPlanet == true) { Align(); }
        if (!inPlanet)
        {
            foreach (var g in Gyros) g.GyroOverride = false;
            argument = "toggle";
            Launch = false;
        }
        //foreach (var c in cons) c.ControlThrusters = false;
        float tgtspd = TargetSpeed + 9.72f;
        var gravityDir = Vector3D.Normalize(controller.GetNaturalGravity());
        if (gravityDir.Length() == 0) Launch = false;
        Echo(Convert.ToString(gravityDir.Length()));

        var upVec = gravityDir;
        var vel = controller.GetShipVelocities().LinearVelocity;
        float VSpeed = Vector3.Dot(vel, upVec) * -1;

        float diff = (tgtspd - VSpeed);
        Echo(Convert.ToString(diff));

        var axisGrav = Vector3D.TransformNormal(gravityDir, MatrixD.Transpose(controller.WorldMatrix));
        var gravX = axisGrav.X; var gravY = axisGrav.Y; var gravZ = axisGrav.Z;
        var mass = controller.CalculateShipMass().PhysicalMass;
        var massX = mass * gravX; var massY = mass * gravY; var massZ = mass * gravZ;

        var dor = 0f; var ven = 0f; var por = 0f; var sta = 0f; var fore = 0f; var aft = 0f;
        foreach (var d in DorsalThrust) dor += d.MaxEffectiveThrust;
        foreach (var v in VentralThrust) ven += v.MaxEffectiveThrust;
        foreach (var p in PortThrust) por += p.MaxEffectiveThrust;
        foreach (var s in StarboardThrust) sta += s.MaxEffectiveThrust;
        foreach (var f in ForeThrust) fore += f.MaxEffectiveThrust;
        foreach (var a in AftThrust) aft += a.MaxEffectiveThrust;

        foreach (var t in DorsalThrust) t.ThrustOverridePercentage = (float)(massY / dor) * diff;
        foreach (var t in VentralThrust) t.ThrustOverridePercentage = (float)((massY * -1) / ven) * diff;
        foreach (var t in StarboardThrust) t.ThrustOverridePercentage = (float)((massX) / sta) * diff;
        foreach (var t in PortThrust) t.ThrustOverridePercentage = (float)((massX * -1) / por) * diff;
        foreach (var t in ForeThrust) t.ThrustOverridePercentage = (float)((massZ * -1) / fore) * diff;
        foreach (var t in AftThrust) t.ThrustOverridePercentage = (float)((massZ) / aft) * diff;
        foreach (var t in Thrusters) t.ThrustOverride += 1;
    }
}

public void Align()
{
    Echo("Align Running");
    Orientation();
    Aligner();
}

public void Orientation() //Determine which side of ship has the most available thrust and should be the "bottom"
{
    Echo("Orientation Runnung");

    var dor = 0f; var ven = 0f; var por = 0f; var sta = 0f; var fore = 0f; var aft = 0f;
    foreach (var d in DorsalThrust) dor += d.MaxEffectiveThrust;
    foreach (var v in VentralThrust) ven += v.MaxEffectiveThrust;
    foreach (var p in PortThrust) por += p.MaxEffectiveThrust;
    foreach (var s in StarboardThrust) sta += s.MaxEffectiveThrust;
    foreach (var f in ForeThrust) fore += f.MaxEffectiveThrust;
    foreach (var a in AftThrust) aft += a.MaxEffectiveThrust;

    var d1 = Math.Max(ven, aft);
    var d2 = Math.Max(por, sta);
    var d3 = Math.Max(dor, fore);
    var d4 = Math.Max(d1, d2);
    var d5 = Math.Max(d4, d3);

    if (fore == d5) Lifters = ForeThrust;
    if (sta == d5) Lifters = StarboardThrust;
    if (por == d5) Lifters = PortThrust;
    if (dor == d5) Lifters = DorsalThrust;
    if (aft == d5) Lifters = AftThrust;
    if (ven == d5) Lifters = VentralThrust;
    if (Lifters == null) Lifters = DorsalThrust;

    ReferenceBlock = Lifters[0];
}

public void Aligner()
{

    Echo("Aligner Running");
    //Get orientation from rc
    Matrix or;
    ReferenceBlock.Orientation.GetMatrix(out or);
    Vector3D down = or.Forward;

    var grav = cons[0].GetNaturalGravity();
    grav.Normalize();

    for (int i = 0; i < Gyros.Count; ++i)
    {
        var g = Gyros[i];

        g.Orientation.GetMatrix(out or);
        var localDown = Vector3D.Transform(down, MatrixD.Transpose(or));

        var localGrav = Vector3D.Transform(grav, MatrixD.Transpose(g.WorldMatrix.GetOrientation()));

        //Since the gyro ui lies, we are not trying to control yaw,pitch,roll but rather we
        //need a rotation vector (axis around which to rotate)
        var rot = Vector3D.Cross(localDown, localGrav);
        double ang = rot.Length();
        ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang))); //More numerically stable than: ang=Math.Asin(ang)

        if (ang < 0.01)
        {   //Close enough
            Echo("Level");
            //g.SetValueBool("Override", false);
            continue;
        }
        Echo("Off level: " + (ang * 180.0 / 3.14).ToString() + "deg");

        //Control speed to be proportional to distance (angle) we have left
        double ctrl_vel = g.GetMaximum<float>("Yaw") * (ang / Math.PI) * CTRL_COEFF;
        ctrl_vel = Math.Min(g.GetMaximum<float>("Yaw"), ctrl_vel);
        ctrl_vel = Math.Max(0.01, ctrl_vel); //Gyros don't work well at very low speeds
        rot.Normalize();
        rot *= ctrl_vel;
        g.SetValueFloat("Pitch", (float)rot.GetDim(0));
        g.SetValueFloat("Yaw", -(float)rot.GetDim(1));
        g.SetValueFloat("Roll", -(float)rot.GetDim(2));

        g.SetValueFloat("Power", 1.0f);
        g.SetValueBool("Override", true);
    }
}
