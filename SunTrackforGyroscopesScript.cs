//HellArea Sun Tracker for Gyro v.1.05 (2018-2019) / mailto:HellArea@Outlook.com
//Working will be paused when Grid is under control. 
//Run parameters added On, Off, On_Off (if On then it works)

string Master = "";//OPTIONAL CustomName of the Solar Panel to track the Sun
bool FindTheLight = true;//OPTIONAL Search for The Sun in the Shadows
float SolarPanelMaxRatio = 1; // OPTIONAL multiplier for modded panels

public Program() { Runtime.UpdateFrequency = Sandbox.ModAPI.Ingame.UpdateFrequency.Update10; }

public void Main(string argument, UpdateType updateSource)
{
    if (BootComplete) { if (!string.IsNullOrEmpty(argument)) { ProceedArgument(argument); } else { Thread(); } }
    else { Boot(); }
}

void ProceedArgument(string argument)
{
    switch (argument.ToLower())
    {
        case "on": Paused = false; break;
        case "off": Paused = true; break;
        case "on_off": Paused = !Paused; break;
    }
    Me.CustomData = (Paused) ? "Paused" : "";
}

List<IMyGyro> dlGyros = new List<IMyGyro>();
IMySolarPanel daSolarPanel;
bool BootComplete = false;
float MaxPwr;
void Boot()
{
    Me.CustomName = " Programmable Block Sun track";
    Paused = (Me.CustomData == "Paused");
    string E = "";

    GridTerminalSystem.GetBlocksOfType<IMyShipController>(ShipControllers, R => R.CubeGrid == Me.CubeGrid);

    if (Master != "")
    {
        daSolarPanel = GridTerminalSystem.GetBlockWithName(Master) as IMySolarPanel;
        if (daSolarPanel == null) { E += "Error getting " + Master + "\n"; }
    }
    else
    {
        List<IMySolarPanel> dlSolarPanels = new List<IMySolarPanel>();
        GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(dlSolarPanels, R => R.CubeGrid == Me.CubeGrid);
        if (dlSolarPanels.Count > 0) { daSolarPanel = dlSolarPanels[0]; } else { E += "No Solar Panel\n"; }
    }

    GridTerminalSystem.GetBlocksOfType<IMyGyro>(dlGyros, R => R.CubeGrid == Me.CubeGrid);
    if (dlGyros.Count < 1) { E += "No Gyroscope\n"; }

    MaxPwr = ((Me.CubeGrid.GridSizeEnum == MyCubeSize.Large) ? .16f : .04f) * SolarPanelMaxRatio;
    BootComplete = string.IsNullOrEmpty(E);
    Echo(System.DateTime.Now.ToString("HH:mm:ss") + " " + ((BootComplete) ? "Started successfully" : E));
    if (BootComplete) { LastPwr = daSolarPanel.MaxOutput; }
}

double MoveP = .01; double MoveY = .01; float LastPwr; int Step = 0; int Next;
void Thread()
{
    if (GetSkipTrigger()) { SetGyroRotation(daSolarPanel, dlGyros, 0, 0, 0); return; }
    double P = 0; double Y = 0;
    float Pwr = daSolarPanel.MaxOutput;
    if (Pwr < MaxPwr * .02)
    {
        if (FindTheLight) { SetGyroRotation(daSolarPanel, dlGyros, .1, .4); }
        else { SetGyroRotation(daSolarPanel, dlGyros, 0, 0, 0); }
        return;
    }
    int D = Math.Sign(Pwr - LastPwr);
    double V = 2 * MaxPwr / Pwr;
    //Echo("Step  " + Step + "\n" + Pwr + "\nMove " + (MoveP * V));
    if (Pwr > MaxPwr * .98) { if (Step > 0) { Step = 0; SetGyroRotation(daSolarPanel, dlGyros, 0, 0, 0); } return; }
    switch (Step)
    {
        case 0:
            Runtime.UpdateFrequency = Sandbox.ModAPI.Ingame.UpdateFrequency.Update1;
            Next = 0; Step++;
            break;

        case 1:
            if (D < 0) { MoveP = -MoveP; Next++; if (Next > 2) { Step++; Next = 0; } }
            P = MoveP;
            break;

        case 2:
            if (D < 0)
            {
                MoveY = -MoveY; Next++;
                if (Next > 2)
                {
                    SetGyroRotation(daSolarPanel, dlGyros, 0, 0, 0); Step = 0; Next = 0;
                    Runtime.UpdateFrequency = Sandbox.ModAPI.Ingame.UpdateFrequency.Update100;
                }
            }
            Y = MoveY;
            break;
    }
    SetGyroRotation(daSolarPanel, dlGyros, P * V, Y * V, 0);
    LastPwr = Pwr;
}

void SetGyroRotation(IMyTerminalBlock Master, List<IMyGyro> Gyros, double Pitch = 0, double Yaw = 0, double Roll = 0)
{
    Vector3D R = Vector3D.TransformNormal(new Vector3D(Pitch, Yaw, Roll), Master.WorldMatrix);
    Vector3D T;
    bool A = !(Pitch == 0 && Yaw == 0 && Roll == 0);
    foreach (IMyGyro G in Gyros)
    {
        T = Vector3D.TransformNormal(R, Matrix.Transpose(G.WorldMatrix));
        G.Pitch = (float)T.X;
        G.Yaw = (float)T.Y;
        G.Roll = (float)T.Z;
        G.GyroOverride = A;
    }
}

bool Paused = false;
List<IMyShipController> ShipControllers = new List<IMyShipController>();
bool GetSkipTrigger()
{
    bool R = false;
    foreach (IMyShipController A in ShipControllers)
    {
        if (A.CanControlShip)
        {
            R = R || A.IsUnderControl;
            if (A is IMyRemoteControl) { R = R || (A as IMyRemoteControl).IsAutoPilotEnabled; }
        }
    }
    return R || Paused;
}
