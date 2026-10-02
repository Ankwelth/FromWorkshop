/*
Migri Smart Airlock Script - V10
Corrected Closing Behavior

NORMAL STATE:
- One door is ON and available/open.
- The other door is CLOSED and OFF.

IMPORTANT:
- The script DOES NOT force the active door open.
- You can walk up to the active door and close it normally.
- You can also use a button panel that directly closes the door.
- The script only starts the airlock cycle once BOTH doors are fully closed.

CYCLE:
1. Active door closes.
2. Both doors are closed.
3. Both doors turn OFF.
4. Vent pressurizes/depressurizes.
5. Opposite door turns ON and opens.
6. That opposite door becomes the new active door.

EMERGENCY:
- Run PB with: AL:#:EMERGENCY
- Both doors turn ON.
- Once one door opens, the other door closes/turns OFF.
- Normal operation resumes.

GROUP NAMES:
Airlock:1
Airlock:2
Airlock:3

REQUIRED BLOCKS PER GROUP:
Door with "Interior" in the name
Door with "Exterior" in the name

OPTIONAL:
Air Vent
Inset Button Panel - AL
LCD - Interior
LCD - Exterior
*/

const string GROUP_PREFIX = "airlock:";

const int AUTO_SCAN_TICKS = 30;       // Update10 = about 5 seconds
const int SCREEN_UPDATE_TICKS = 6;    // Update10 = about 1 second

Dictionary<int, Airlock> airlocksById = new Dictionary<int, Airlock>();
List<Airlock> airlockList = new List<Airlock>();

List<IMyBlockGroup> groupBuffer = new List<IMyBlockGroup>();
List<IMyTerminalBlock> blockBuffer = new List<IMyTerminalBlock>();

System.Text.StringBuilder echoBuilder = new System.Text.StringBuilder();

int scanCounter = 0;
int screenCounter = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    ScanAirlocks();
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
    string arg = argument == null ? "" : argument.Trim();

    if (arg.Length > 0)
    {
        string lower = arg.ToLower();

        if (lower == "reset")
        {
            for (int i = 0; i < airlockList.Count; i++)
            {
                airlockList[i].ResetRuntime();
            }
        }
        else if (lower == "rescan")
        {
            ScanAirlocks();
        }
        else
        {
            if (!HandleCommand(arg))
            {
                ScanAirlocks();
                HandleCommand(arg);
            }
        }
    }

    scanCounter++;

    if (scanCounter >= AUTO_SCAN_TICKS)
    {
        scanCounter = 0;
        ScanAirlocks();
    }

    screenCounter++;

    bool forceScreenUpdate = false;

    if (screenCounter >= SCREEN_UPDATE_TICKS)
    {
        screenCounter = 0;
        forceScreenUpdate = true;
    }

    double dt = Runtime.TimeSinceLastRun.TotalSeconds;

    if (dt <= 0 || dt > 1)
    {
        dt = 1.0 / 6.0;
    }

    int validCount = 0;

    echoBuilder.Clear();

    for (int i = 0; i < airlockList.Count; i++)
    {
        Airlock al = airlockList[i];

        al.Tick(dt, forceScreenUpdate);

        if (al.Valid)
        {
            validCount++;
        }

        echoBuilder.AppendLine(al.EchoLine());
    }

    Echo("Smart Airlocks V10");
    Echo("Valid: " + validCount + " / Found: " + airlockList.Count);
    Echo("Cycle starts only when BOTH doors are closed.");
    Echo("");
    Echo("PB Commands:");
    Echo("AL:#:EMERGENCY");
    Echo("AL:#:reset");
    Echo("reset");
    Echo("rescan");
    Echo("");
    Echo(echoBuilder.ToString());
}

bool HandleCommand(string argument)
{
    string[] parts = argument.Split(':');

    if (parts.Length < 3)
    {
        return false;
    }

    string prefix = parts[0].Trim().ToLower();

    if (prefix != "al" && prefix != "airlock")
    {
        return false;
    }

    int id;

    if (!int.TryParse(parts[1].Trim(), out id))
    {
        return false;
    }

    Airlock al;

    if (!airlocksById.TryGetValue(id, out al))
    {
        return false;
    }

    string command = NormalizeCommand(parts[2]);

    if (command == "emergency" || command == "override" || command == "emergencyoverride")
    {
        al.EmergencyOverride();
        return true;
    }

    if (command == "reset")
    {
        al.ResetRuntime();
        return true;
    }

    return false;
}

string NormalizeCommand(string text)
{
    if (text == null)
    {
        return "";
    }

    text = text.ToLower();
    text = text.Replace(" ", "");
    text = text.Replace("_", "");
    text = text.Replace("-", "");

    return text;
}

void ScanAirlocks()
{
    Dictionary<int, Airlock> newMap = new Dictionary<int, Airlock>();
    List<Airlock> newList = new List<Airlock>();

    groupBuffer.Clear();
    GridTerminalSystem.GetBlockGroups(groupBuffer);

    for (int i = 0; i < groupBuffer.Count; i++)
    {
        IMyBlockGroup group = groupBuffer[i];

        int id = ParseAirlockId(group.Name);

        if (id < 0)
        {
            continue;
        }

        if (newMap.ContainsKey(id))
        {
            continue;
        }

        Airlock al = new Airlock(id, group.Name);

        Airlock old;

        if (airlocksById.TryGetValue(id, out old))
        {
            al.CopyRuntimeFrom(old);
        }

        blockBuffer.Clear();
        group.GetBlocks(blockBuffer);

        for (int b = 0; b < blockBuffer.Count; b++)
        {
            al.AddBlock(blockBuffer[b]);
        }

        al.FinishSetup();

        newMap[id] = al;
        newList.Add(al);
    }

    airlocksById = newMap;
    airlockList = newList;
}

int ParseAirlockId(string groupName)
{
    if (groupName == null)
    {
        return -1;
    }

    string lower = groupName.ToLower();

    if (!lower.StartsWith(GROUP_PREFIX))
    {
        return -1;
    }

    string rest = groupName.Substring(GROUP_PREFIX.Length).Trim();
    string digits = "";

    for (int i = 0; i < rest.Length; i++)
    {
        if (char.IsDigit(rest[i]))
        {
            digits += rest[i];
        }
        else
        {
            break;
        }
    }

    int id;

    if (!int.TryParse(digits, out id))
    {
        return -1;
    }

    return id;
}

enum AirlockStep
{
    Idle,
    ClosingBoth,
    Venting,
    OpeningTarget,
    EmergencyOverride
}

enum AirlockDirection
{
    None,
    InteriorToExterior,
    ExteriorToInterior
}

enum DoorSide
{
    Unknown,
    Interior,
    Exterior
}

class Airlock
{
    const double NO_PRESSURE_CHANGE_SECONDS = 3.0;
    const float PRESSURE_EPSILON = 0.0005f;
    const float LOW_PRESSURE = 0.01f;
    const float HIGH_PRESSURE = 0.99f;

    public int Id;
    public string GroupName;

    public IMyDoor InteriorDoor;
    public IMyDoor ExteriorDoor;
    public IMyAirVent Vent;

    public IMyTextSurface AlScreen;
    public IMyTextSurface InteriorScreen;
    public IMyTextSurface ExteriorScreen;

    public List<IMyDoor> Doors = new List<IMyDoor>();

    AirlockStep step = AirlockStep.Idle;
    AirlockDirection direction = AirlockDirection.None;
    DoorSide activeSide = DoorSide.Unknown;

    bool runtimeCopied = false;

    bool lastInteriorOpenish = false;
    bool lastExteriorOpenish = false;

    float lastPressure = -1f;
    double stablePressureSeconds = 0;

    bool screenDirty = true;

    string lastAlText = "";
    string lastInteriorText = "";
    string lastExteriorText = "";

    System.Text.StringBuilder screenBuilder = new System.Text.StringBuilder(512);

    public bool Valid
    {
        get
        {
            return InteriorDoor != null && ExteriorDoor != null;
        }
    }

    public Airlock(int id, string groupName)
    {
        Id = id;
        GroupName = groupName;
    }

    public void CopyRuntimeFrom(Airlock old)
    {
        step = old.step;
        direction = old.direction;
        activeSide = old.activeSide;

        lastInteriorOpenish = old.lastInteriorOpenish;
        lastExteriorOpenish = old.lastExteriorOpenish;

        lastPressure = old.lastPressure;
        stablePressureSeconds = old.stablePressureSeconds;

        runtimeCopied = true;
        screenDirty = true;
    }

    public void AddBlock(IMyTerminalBlock block)
    {
        if (block == null)
        {
            return;
        }

        string name = block.CustomName.ToLower();

        IMyDoor door = block as IMyDoor;

        if (door != null)
        {
            Doors.Add(door);

            if (name.Contains("interior") && InteriorDoor == null)
            {
                InteriorDoor = door;
            }
            else if (name.Contains("exterior") && ExteriorDoor == null)
            {
                ExteriorDoor = door;
            }

            return;
        }

        IMyAirVent vent = block as IMyAirVent;

        if (vent != null)
        {
            if (Vent == null)
            {
                Vent = vent;
            }

            return;
        }

        TryAddScreen(block);
    }

    void TryAddScreen(IMyTerminalBlock block)
    {
        if (block == null)
        {
            return;
        }

        string name = block.CustomName.ToLower();

        IMyTextSurface directSurface = block as IMyTextSurface;

        if (directSurface != null)
        {
            AssignScreenByName(name, directSurface);
            return;
        }

        IMyTextSurfaceProvider provider = block as IMyTextSurfaceProvider;

        if (provider != null && provider.SurfaceCount > 0)
        {
            IMyTextSurface surface = provider.GetSurface(0);
            AssignScreenByName(name, surface);
        }
    }

    void AssignScreenByName(string name, IMyTextSurface surface)
    {
        if (surface == null)
        {
            return;
        }

        string key = NormalizeName(name);

        if (key.Contains("interior") && InteriorScreen == null)
        {
            InteriorScreen = surface;
        }
        else if (key.Contains("exterior") && ExteriorScreen == null)
        {
            ExteriorScreen = surface;
        }
        else if ((key.EndsWith("al") || key.Contains("control") || key.Contains("status") || key.Contains("inset")) && AlScreen == null)
        {
            AlScreen = surface;
        }
        else if (AlScreen == null)
        {
            AlScreen = surface;
        }
        else if (InteriorScreen == null)
        {
            InteriorScreen = surface;
        }
        else if (ExteriorScreen == null)
        {
            ExteriorScreen = surface;
        }

        screenDirty = true;
    }

    string NormalizeName(string text)
    {
        if (text == null)
        {
            return "";
        }

        text = text.ToLower();
        text = text.Replace(" ", "");
        text = text.Replace("_", "");
        text = text.Replace("-", "");
        text = text.Replace(":", "");

        return text;
    }

    public void FinishSetup()
    {
        if (InteriorDoor == null && Doors.Count > 0)
        {
            InteriorDoor = Doors[0];
        }

        if (ExteriorDoor == null)
        {
            for (int i = 0; i < Doors.Count; i++)
            {
                if (Doors[i] != InteriorDoor)
                {
                    ExteriorDoor = Doors[i];
                    break;
                }
            }
        }

        if (InteriorDoor == ExteriorDoor)
        {
            ExteriorDoor = null;
        }

        if (!runtimeCopied)
        {
            activeSide = DoorSide.Unknown;

            lastInteriorOpenish = IsOpenish(InteriorDoor);
            lastExteriorOpenish = IsOpenish(ExteriorDoor);
        }

        screenDirty = true;
    }

    public void EmergencyOverride()
    {
        if (!Valid)
        {
            return;
        }

        SetState(AirlockStep.EmergencyOverride, AirlockDirection.None);

        SetEnabled(InteriorDoor, true);
        SetEnabled(ExteriorDoor, true);

        stablePressureSeconds = 0;
        lastPressure = -1f;

        screenDirty = true;
    }

    public void ResetRuntime()
    {
        SetState(AirlockStep.Idle, AirlockDirection.None);

        activeSide = DoorSide.Unknown;

        stablePressureSeconds = 0;
        lastPressure = -1f;

        SetEnabled(InteriorDoor, true);
        SetEnabled(ExteriorDoor, true);

        lastInteriorOpenish = IsOpenish(InteriorDoor);
        lastExteriorOpenish = IsOpenish(ExteriorDoor);

        screenDirty = true;
    }

    public void Tick(double dt, bool forceScreenUpdate)
    {
        if (!Valid)
        {
            if (forceScreenUpdate || screenDirty)
            {
                UpdateScreens();
            }

            return;
        }

        if (step == AirlockStep.Idle)
        {
            HandleIdle();
        }
        else if (step == AirlockStep.ClosingBoth)
        {
            HandleClosingBoth();
        }
        else if (step == AirlockStep.Venting)
        {
            HandleVenting(dt);
        }
        else if (step == AirlockStep.OpeningTarget)
        {
            HandleOpeningTarget();
        }
        else if (step == AirlockStep.EmergencyOverride)
        {
            HandleEmergencyOverride();
        }

        if (forceScreenUpdate || screenDirty || step == AirlockStep.Venting || step == AirlockStep.OpeningTarget)
        {
            UpdateScreens();
        }

        lastInteriorOpenish = IsOpenish(InteriorDoor);
        lastExteriorOpenish = IsOpenish(ExteriorDoor);
    }

    void HandleIdle()
    {
        if (activeSide == DoorSide.Unknown)
        {
            PickOrCreateActiveSide();
        }

        if (activeSide == DoorSide.Interior)
        {
            MaintainInteriorActiveState();
        }
        else if (activeSide == DoorSide.Exterior)
        {
            MaintainExteriorActiveState();
        }
    }

    void PickOrCreateActiveSide()
    {
        bool interiorOpenish = IsOpenish(InteriorDoor);
        bool exteriorOpenish = IsOpenish(ExteriorDoor);

        if (interiorOpenish && !exteriorOpenish)
        {
            activeSide = DoorSide.Interior;
            screenDirty = true;
            return;
        }

        if (exteriorOpenish && !interiorOpenish)
        {
            activeSide = DoorSide.Exterior;
            screenDirty = true;
            return;
        }

        if (interiorOpenish && exteriorOpenish)
        {
            // Safety recovery: choose interior as active, close exterior.
            activeSide = DoorSide.Interior;
            screenDirty = true;
            return;
        }

        // Startup/recovery only:
        // Both doors are closed and the script does not know which side is active.
        // Let the player open either door. Once one opens, the script will pick that side.
        SetEnabled(InteriorDoor, true);
        SetEnabled(ExteriorDoor, true);
        screenDirty = true;
    }

    void MaintainInteriorActiveState()
    {
        // Interior is the usable side. Do NOT force it open.
        // Player must be allowed to close it manually.
        SetEnabled(InteriorDoor, true);

        // Exterior must be closed and off.
        if (!IsClosed(ExteriorDoor))
        {
            SetEnabled(ExteriorDoor, true);
            ExteriorDoor.CloseDoor();
        }
        else
        {
            SetEnabled(ExteriorDoor, false);
        }

        // Only start the cycle once BOTH doors are fully closed.
        if (IsClosed(InteriorDoor) && IsClosed(ExteriorDoor))
        {
            direction = AirlockDirection.InteriorToExterior;
            LockBothAndStartPressureStep();
        }
    }

    void MaintainExteriorActiveState()
    {
        // Exterior is the usable side. Do NOT force it open.
        // Player must be allowed to close it manually.
        SetEnabled(ExteriorDoor, true);

        // Interior must be closed and off.
        if (!IsClosed(InteriorDoor))
        {
            SetEnabled(InteriorDoor, true);
            InteriorDoor.CloseDoor();
        }
        else
        {
            SetEnabled(InteriorDoor, false);
        }

        // Only start the cycle once BOTH doors are fully closed.
        if (IsClosed(ExteriorDoor) && IsClosed(InteriorDoor))
        {
            direction = AirlockDirection.ExteriorToInterior;
            LockBothAndStartPressureStep();
        }
    }

    void LockBothAndStartPressureStep()
    {
        if (!IsClosed(InteriorDoor) || !IsClosed(ExteriorDoor))
        {
            SetState(AirlockStep.ClosingBoth, direction);
            return;
        }

        SetEnabled(InteriorDoor, false);
        SetEnabled(ExteriorDoor, false);

        stablePressureSeconds = 0;
        lastPressure = -1f;

        if (Vent != null)
        {
            SetEnabled(Vent, true);
            SetVentDepressurize(direction == AirlockDirection.InteriorToExterior);

            lastPressure = Vent.GetOxygenLevel();

            SetState(AirlockStep.Venting, direction);
        }
        else
        {
            SetState(AirlockStep.OpeningTarget, direction);
        }
    }

    void HandleClosingBoth()
    {
        SetEnabled(InteriorDoor, true);
        SetEnabled(ExteriorDoor, true);

        InteriorDoor.CloseDoor();
        ExteriorDoor.CloseDoor();

        if (IsClosed(InteriorDoor) && IsClosed(ExteriorDoor))
        {
            LockBothAndStartPressureStep();
        }
    }

    void HandleVenting(double dt)
    {
        SetEnabled(InteriorDoor, false);
        SetEnabled(ExteriorDoor, false);

        if (Vent == null)
        {
            SetState(AirlockStep.OpeningTarget, direction);
            return;
        }

        SetEnabled(Vent, true);
        SetVentDepressurize(direction == AirlockDirection.InteriorToExterior);

        float pressure = Vent.GetOxygenLevel();

        if (lastPressure >= 0)
        {
            if (Math.Abs(pressure - lastPressure) <= PRESSURE_EPSILON)
            {
                stablePressureSeconds += dt;
            }
            else
            {
                stablePressureSeconds = 0;
            }
        }

        lastPressure = pressure;

        bool targetReached = false;

        if (direction == AirlockDirection.InteriorToExterior)
        {
            targetReached = pressure <= LOW_PRESSURE;
        }
        else if (direction == AirlockDirection.ExteriorToInterior)
        {
            targetReached = pressure >= HIGH_PRESSURE;
        }

        if (targetReached || stablePressureSeconds >= NO_PRESSURE_CHANGE_SECONDS)
        {
            SetState(AirlockStep.OpeningTarget, direction);
        }
    }

    void HandleOpeningTarget()
    {
        IMyDoor source = SourceDoor();
        IMyDoor target = TargetDoor();

        if (source != null)
        {
            if (!IsClosed(source))
            {
                SetEnabled(source, true);
                source.CloseDoor();
                return;
            }

            SetEnabled(source, false);
        }

        if (target != null)
        {
            SetEnabled(target, true);
            target.OpenDoor();

            if (target.Status == DoorStatus.Open)
            {
                if (direction == AirlockDirection.InteriorToExterior)
                {
                    activeSide = DoorSide.Exterior;
                }
                else if (direction == AirlockDirection.ExteriorToInterior)
                {
                    activeSide = DoorSide.Interior;
                }

                SetState(AirlockStep.Idle, AirlockDirection.None);

                stablePressureSeconds = 0;
                lastPressure = -1f;
            }
        }
    }

    void HandleEmergencyOverride()
    {
        SetEnabled(InteriorDoor, true);
        SetEnabled(ExteriorDoor, true);

        bool interiorOpenish = IsOpenish(InteriorDoor);
        bool exteriorOpenish = IsOpenish(ExteriorDoor);

        bool interiorJustOpened = interiorOpenish && !lastInteriorOpenish;
        bool exteriorJustOpened = exteriorOpenish && !lastExteriorOpenish;

        if (interiorJustOpened && !exteriorJustOpened)
        {
            activeSide = DoorSide.Interior;
            SetState(AirlockStep.Idle, AirlockDirection.None);
            MaintainInteriorActiveState();
            return;
        }

        if (exteriorJustOpened && !interiorJustOpened)
        {
            activeSide = DoorSide.Exterior;
            SetState(AirlockStep.Idle, AirlockDirection.None);
            MaintainExteriorActiveState();
            return;
        }

        if (interiorOpenish && !exteriorOpenish)
        {
            activeSide = DoorSide.Interior;
            SetState(AirlockStep.Idle, AirlockDirection.None);
            MaintainInteriorActiveState();
            return;
        }

        if (exteriorOpenish && !interiorOpenish)
        {
            activeSide = DoorSide.Exterior;
            SetState(AirlockStep.Idle, AirlockDirection.None);
            MaintainExteriorActiveState();
            return;
        }

        if (interiorOpenish && exteriorOpenish)
        {
            activeSide = DoorSide.Interior;
            SetState(AirlockStep.Idle, AirlockDirection.None);
            MaintainInteriorActiveState();
            return;
        }

        // If both are still closed, emergency stays active.
        // Both remain ON until one door opens.
    }

    void SetState(AirlockStep newStep, AirlockDirection newDirection)
    {
        if (step != newStep || direction != newDirection)
        {
            step = newStep;
            direction = newDirection;
            screenDirty = true;
        }
    }

    void SetEnabled(IMyFunctionalBlock block, bool enabled)
    {
        if (block == null)
        {
            return;
        }

        if (block.Enabled != enabled)
        {
            block.Enabled = enabled;
            screenDirty = true;
        }
    }

    void SetVentDepressurize(bool depressurize)
    {
        if (Vent == null)
        {
            return;
        }

        if (Vent.Depressurize != depressurize)
        {
            Vent.Depressurize = depressurize;
            screenDirty = true;
        }
    }

    IMyDoor SourceDoor()
    {
        if (direction == AirlockDirection.InteriorToExterior)
        {
            return InteriorDoor;
        }

        if (direction == AirlockDirection.ExteriorToInterior)
        {
            return ExteriorDoor;
        }

        return null;
    }

    IMyDoor TargetDoor()
    {
        if (direction == AirlockDirection.InteriorToExterior)
        {
            return ExteriorDoor;
        }

        if (direction == AirlockDirection.ExteriorToInterior)
        {
            return InteriorDoor;
        }

        return null;
    }

    bool IsClosed(IMyDoor door)
    {
        if (door == null)
        {
            return false;
        }

        return door.Status == DoorStatus.Closed;
    }

    bool IsOpenish(IMyDoor door)
    {
        if (door == null)
        {
            return false;
        }

        return door.Status == DoorStatus.Open || door.Status == DoorStatus.Opening;
    }

    void UpdateScreens()
    {
        string common = BuildCommonText();

        string alText =
            "AIRLOCK " + Id + "\n" +
            "Mode: " + DirectionText() + "\n" +
            "Active Side: " + ActiveSideText() + "\n" +
            common;

        string interiorText =
            "AIRLOCK " + Id + " - INTERIOR\n" +
            common;

        string exteriorText =
            "AIRLOCK " + Id + " - EXTERIOR\n" +
            common;

        WriteScreen(AlScreen, ref lastAlText, alText);
        WriteScreen(InteriorScreen, ref lastInteriorText, interiorText);
        WriteScreen(ExteriorScreen, ref lastExteriorText, exteriorText);

        screenDirty = false;
    }

    string BuildCommonText()
    {
        screenBuilder.Clear();

        screenBuilder.Append("Interior Door: ");
        screenBuilder.AppendLine(DoorText(InteriorDoor));

        screenBuilder.Append("Exterior Door: ");
        screenBuilder.AppendLine(DoorText(ExteriorDoor));

        screenBuilder.AppendLine(VentText());

        screenBuilder.Append("State: ");
        screenBuilder.AppendLine(StepText());

        return screenBuilder.ToString();
    }

    void WriteScreen(IMyTextSurface screen, ref string lastText, string text)
    {
        if (screen == null)
        {
            return;
        }

        if (lastText == text)
        {
            return;
        }

        screen.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        screen.FontSize = 0.75f;
        screen.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
        screen.WriteText(text, false);

        lastText = text;
    }

    string DoorText(IMyDoor door)
    {
        if (door == null)
        {
            return "MISSING";
        }

        string power = door.Enabled ? "ON" : "OFF";

        return door.Status.ToString() + " / Power " + power;
    }

    string VentText()
    {
        if (Vent == null)
        {
            return "Vent: NONE";
        }

        float pressure = Vent.GetOxygenLevel();
        int percent = (int)Math.Round(pressure * 100);

        string pressureState = "";

        if (pressure >= HIGH_PRESSURE)
        {
            pressureState = "PRESSURIZED";
        }
        else if (pressure <= LOW_PRESSURE)
        {
            pressureState = "DEPRESSURIZED";
        }
        else
        {
            pressureState = "CHANGING";
        }

        string mode = Vent.Depressurize ? "Depressurizing" : "Pressurizing";
        string power = Vent.Enabled ? "ON" : "OFF";

        return "Vent: " + pressureState + " " + percent + "% / " + mode + " / Power " + power;
    }

    string StepText()
    {
        if (!Valid)
        {
            return "INVALID GROUP";
        }

        if (step == AirlockStep.Idle)
        {
            return "Idle";
        }

        if (step == AirlockStep.ClosingBoth)
        {
            return "Closing both doors";
        }

        if (step == AirlockStep.Venting)
        {
            if (direction == AirlockDirection.InteriorToExterior)
            {
                return "Depressurizing";
            }

            if (direction == AirlockDirection.ExteriorToInterior)
            {
                return "Pressurizing";
            }

            return "Venting";
        }

        if (step == AirlockStep.OpeningTarget)
        {
            if (direction == AirlockDirection.InteriorToExterior)
            {
                return "Opening exterior door";
            }

            if (direction == AirlockDirection.ExteriorToInterior)
            {
                return "Opening interior door";
            }

            return "Opening target door";
        }

        if (step == AirlockStep.EmergencyOverride)
        {
            return "EMERGENCY OVERRIDE";
        }

        return "Unknown";
    }

    string DirectionText()
    {
        if (direction == AirlockDirection.InteriorToExterior)
        {
            return "Interior -> Exterior";
        }

        if (direction == AirlockDirection.ExteriorToInterior)
        {
            return "Exterior -> Interior";
        }

        return "None";
    }

    string ActiveSideText()
    {
        if (activeSide == DoorSide.Interior)
        {
            return "Interior";
        }

        if (activeSide == DoorSide.Exterior)
        {
            return "Exterior";
        }

        return "Unknown";
    }

    public string EchoLine()
    {
        if (!Valid)
        {
            return GroupName + " : INVALID - need 2 doors";
        }

        string pressure = "No vent";

        if (Vent != null)
        {
            pressure = ((int)Math.Round(Vent.GetOxygenLevel() * 100)).ToString() + "%";
        }

        return "Airlock:" + Id + " | " + StepText() + " | Active: " + ActiveSideText() + " | " + pressure;
    }
}