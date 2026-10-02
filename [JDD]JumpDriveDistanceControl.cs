//---- JUMP DRIVE DISTANCE CONTROL ----

// --- settings ---
string cockpitName = "JDD Cockpit";     // The name of the cockpit from which you want to control the jump drive.
int screenIndex = 0;                                       //  The number of the cockpit screen you want to display the information on.
string jumpDriveName = "JDD Drive";        //  The name of the jump drive or group.
bool isGroup = true;                                      //  One jump drive - false; a group of jump drives - true











// SCRIPT////////////////////////////////////////////////
IMyCockpit cockpit;
List<IMyJumpDrive> jumpDrives = new List<IMyJumpDrive>();
IMyTextSurface display;
IMyTextSurface pbSurface;

bool isMenuActive = false;
int selectedRow = 0;
float calcDistance = 5f;
Vector3 lastMoveIndicator = new Vector3();
int tickCounter = 0;

StringBuilder screenText = new StringBuilder();
StringBuilder diagText = new StringBuilder();

readonly string[] menuItems = {
    "[ -   1000   + ]",
    "[ -    100    + ]",
    "[ -     10     + ]",
    "[ -      1      + ]",
    "[  SET-DIST ]"
};

public Program()
{
    pbSurface = Me.GetSurface(0);
    pbSurface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    pbSurface.FontSize = 1.0f;
    pbSurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;

    FindBlocks();
    if (jumpDrives.Count > 0)
        calcDistance = (float)Math.Round(jumpDrives[0].JumpDistanceMeters / 1000f, 1);

    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    UpdateDiagnostics();
    DrawMenu();
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument.ToLower() == "update")
    {
        FindBlocks();
        UpdateDiagnostics();
        DrawMenu();
        return;
    }

    if (argument.ToLower() == "menu")
    {
        isMenuActive = !isMenuActive;
        ToggleCockpitControl(!isMenuActive);
        UpdateDiagnostics();
        DrawMenu();
        return;
    }

    if ((updateSource & UpdateType.Update1) != 0)
    {
        HandleInput();

        tickCounter++;

        if (tickCounter >= 20)
        {
            UpdateDiagnostics();
            DrawMenu();
            tickCounter = 0;
        }
    }
}

void ToggleCockpitControl(bool enable)
{
    if (cockpit == null) return;
    cockpit.ControlThrusters = enable;
    cockpit.ControlWheels = enable;
}

void HandleInput()
{
    if (cockpit != null && !cockpit.IsUnderControl && isMenuActive)
    {
        isMenuActive = false;
        ToggleCockpitControl(true);
        UpdateDiagnostics();
        DrawMenu();
        return;
    }

    if (!isMenuActive || cockpit == null || jumpDrives.Count == 0) return;

    Vector3 currentMove = cockpit.MoveIndicator;
    bool actionMade = false;

    if (currentMove.Z < -0.5f && lastMoveIndicator.Z >= -0.5f) { selectedRow = (selectedRow <= 0) ? 4 : selectedRow - 1; actionMade = true; }
    else if (currentMove.Z > 0.5f && lastMoveIndicator.Z <= 0.5f) { selectedRow = (selectedRow >= 4) ? 0 : selectedRow + 1; actionMade = true; }

    if (currentMove.X < -0.5f && lastMoveIndicator.X >= -0.5f) { AdjustValue(-1f); actionMade = true; }
    else if (currentMove.X > 0.5f && lastMoveIndicator.X <= 0.5f) { AdjustValue(1f); actionMade = true; }

    if (currentMove.Y > 0.5f && lastMoveIndicator.Y <= 0.5f && selectedRow == 4)
    {
        foreach (var jd in jumpDrives)
        {
            if (jd != null) jd.JumpDistanceMeters = calcDistance * 1000f;
        }
        actionMade = true;
    }

    lastMoveIndicator = currentMove;
    if (actionMade) DrawMenu();
}

void AdjustValue(float sign)
{
    if (selectedRow == 0) calcDistance += 1000f * sign;
    else if (selectedRow == 1) calcDistance += 100f * sign;
    else if (selectedRow == 2) calcDistance += 10f * sign;
    else if (selectedRow == 3) calcDistance += 1f * sign;

    if (calcDistance < 5) calcDistance = 5f;

    if (jumpDrives.Count > 0 && jumpDrives[0] != null)
    {
        float maxPossible = jumpDrives[0].MaxJumpDistanceMeters / 1000f;
        if (calcDistance > maxPossible) calcDistance = (float)Math.Round(maxPossible, 1);
    }
}

void UpdateDiagnostics()
{
    diagText.Clear();
    diagText.AppendLine("--- JUMP SYSTEM ---");
    diagText.AppendLine($"Status: {(isMenuActive ? "MENU OPEN" : "MONITORING")}");
    diagText.AppendLine($"Cockpit: {(cockpit != null ? "OK" : "MISSING")}");
    diagText.AppendLine($"Drives: {jumpDrives.Count}");
    diagText.AppendLine($"Target: {calcDistance} km");
    diagText.AppendLine("================");
    diagText.AppendLine("Controls: W/S: Nav, A/D: Adj,\nSpace: Set");

    Echo(diagText.ToString());
    pbSurface.WriteText(diagText);
}

void DrawMenu()
{
    if (display == null) return;

    screenText.Clear();

    float chargePct = 0;
    if (jumpDrives.Count > 0 && jumpDrives[0] != null)
        chargePct = jumpDrives[0].CurrentStoredPower / jumpDrives[0].MaxStoredPower;

    screenText.AppendLine(GetProgressBar(chargePct));

    string indicator = isMenuActive ? "[ ACTIVE ]" : "[ ........... ]";
    screenText.AppendLine($"{indicator} JUMP SYSTEM");
    screenText.AppendLine("-----------------------------------");

    if (jumpDrives.Count == 0)
    {
        screenText.AppendLine("\nERROR: NO DRIVES FOUND");
        display.WriteText(screenText);
        return;
    }

    float currentKm = jumpDrives[0].JumpDistanceMeters / 1000f;
    screenText.AppendLine($"STATUS: {Math.Round(currentKm, 1)} km");
    screenText.AppendLine($"TARGET: {calcDistance} km");
    screenText.AppendLine("-----------------------------------");

    for (int i = 0; i < menuItems.Length; i++)
    {
        if (isMenuActive && selectedRow == i)
            screenText.AppendLine($" > {menuItems[i]} < ");
        else
            screenText.AppendLine($"    {menuItems[i]}    ");
    }

    display.WriteText(screenText);
}

string GetProgressBar(float pct)
{
    int width = 40;
    int filled = (int)(pct * width);
    if (filled < 0) filled = 0;
    if (filled > width) filled = width;
    return "[" + new string('|', filled) + new string('.', width - filled) + $"] {Math.Round(pct * 100)}%";
}

void FindBlocks()
{
    cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit;
    if (cockpit != null)
    {
        display = cockpit.GetSurface(screenIndex);
        if (display != null)
        {
            display.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        }
    }

    jumpDrives.Clear();
    if (isGroup)
    {
        var g = GridTerminalSystem.GetBlockGroupWithName(jumpDriveName);
        g?.GetBlocksOfType(jumpDrives);
    }
    else
    {
        var jd = GridTerminalSystem.GetBlockWithName(jumpDriveName) as IMyJumpDrive;
        if (jd != null) jumpDrives.Add(jd);
    }
}