/*
 * NEXUS Jump Assist Control System
 * Author: BATERCOOL
 * Version: 1.7.15
 *
 * Changelog:
 * v1.7.15 - Rendered TARGET digits as one aligned monospace string.
 * v1.7.14 - Lowered tab labels and emphasized active TARGET controls.
 * v1.7.13 - Smoothed compact panel corners with safe one-pixel steps.
 */

// --- Main Settings ---
string scriptVersion = "1.7.15";
string systemGroupName = "[NEXUS]";
float maxScanDistanceKm = 50f;
int screenIndex = 0;

// --- UI & Scale Settings ---
float uiScale = 0.6f;
int cockpitVisibleRows = 15;
float extUiScale = 0.7f;
int extVisibleRows = 15;
float textScale = 1.0f;
string textFont = "White";

// --- Runtime Settings ---
int activeUiRefreshTicks = 30;
int idleUiRefreshTicks = 60;
int diagnosticsRefreshTicks = 120;
int lightChargeRefreshTicks = 10;
int disabledLightsRefreshTicks = 120;
int autopilotCheckTicks = 10;

// --- Color Settings ---
VRageMath.Color foregroundColor = new VRageMath.Color(180, 90, 0);
VRageMath.Color backgroundColor = new VRageMath.Color(0, 0, 0);
VRageMath.Color gridTextureColor = new VRageMath.Color(162, 81, 0);
VRageMath.Color lcdOverlayColor = new VRageMath.Color(255, 255, 255, 31);

IMyCockpit cockpit;
IMyRemoteControl rc;
List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
List<IMyJumpDrive> jumpDrives = new List<IMyJumpDrive>();
List<IMyGyro> gyros = new List<IMyGyro>();
List<IMyTextPanel> extDisplays = new List<IMyTextPanel>();
List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyTerminalBlock> inventoryBlocks = new List<IMyTerminalBlock>();
IMyTextSurface display;
IMyTextSurface pbSurface;

bool isGroupFound = false;
bool isMenuActive = false;
int activeTab = 0;
bool isActionMenuOpen = false;
int selectedActionRow = 0;
int selectedDigit = 1;
int selectedPointRow = 0;
float calcDistance = 5f;
bool isAligning = false;
Vector3D alignTarget = Vector3D.Zero;
Vector3D autoPilotTarget = Vector3D.Zero;
string lastScanResult = "";
int scanDisplayTimer = 0;
Vector3 lastMoveIndicator = new Vector3();
float lastRollIndicator = 0f;
int tickCounter = 0;
int gpsTickCounter = 120;
int raycastTickCounter = 0;
int telemetryTickCounter = 120;
int diagnosticsTickCounter = 120;
int autopilotTickCounter = 10;
int lightChargeTickCounter = 10;
int disabledLightsTickCounter = 120;
int blinkTickCounter = 0;
bool wasCharging = false;
int greenBlinksLeft = 0;
int lightTickCounter = 0;
bool lightToggleState = false;
bool gyrosFrozen = false;
bool lightsDisabled = false;
float cachedJumpCharge = -1f;
string lastParsedCustomData = null;
string lastDiagnosticsText = "";
UpdateFrequency currentUpdateFrequency = UpdateFrequency.None;
StringBuilder diagText = new StringBuilder();

struct JumpPoint
{
    public string Name;
    public Vector3D Coords;
    public bool IsFolder;
    public bool IsBackBtn;
    public bool IsScan;
    public string FolderRef;
}

List<JumpPoint> rootPoints = new List<JumpPoint>();
Dictionary<string, List<JumpPoint>> folders = new Dictionary<string, List<JumpPoint>>();
List<string> folderOrder = new List<string>();
List<JumpPoint> displayPoints = new List<JumpPoint>();
Dictionary<string, int> folderCursorRows = new Dictionary<string, int>();
string currentActiveFolder = "";
int rootCursorRow = 0;
int totalGpsCount = 0;
float telemetryMaxJumpKm = 0f;
float telemetryJumpCharge = -1f;
float telemetryBatteryCharge = -1f;
float telemetryCargoFill = -1f;
float telemetryMassKg = 0f;
Vector3D telemetryCoords = Vector3D.Zero;

public Program()
{
    pbSurface = Me.GetSurface(0);
    pbSurface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    pbSurface.Font = textFont;
    pbSurface.FontSize = 1.0f;
    pbSurface.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
    FindBlocks();
    ParseGPSData(true);
    UpdateTelemetry();
    ResetGyros();
    if (jumpDrives.Count > 0 && jumpDrives[0] != null) calcDistance = (float)Math.Round(jumpDrives[0].JumpDistanceMeters / 1000f, 1);
    UpdateRuntimeFrequency();
    UpdateDiagnostics();
    DrawMenu();
}

public void Main(string argument, UpdateType updateSource)
{
    try
    {
        string command = (argument ?? "").ToLowerInvariant();
        if (command == "update")
        {
            FindBlocks();
            ParseGPSData(true);
            UpdateTelemetry();
            UpdateDiagnostics();
            DrawMenu();
            UpdateRuntimeFrequency();
            return;
        }

        if (command == "menu")
        {
            isMenuActive = !isMenuActive;
            ToggleCockpitControl(!isMenuActive);
            if (!isMenuActive)
            {
                isAligning = false;
                isActionMenuOpen = false;
                ResetGyros();
            }
            UpdateDiagnostics();
            DrawMenu();
            UpdateRuntimeFrequency();
            return;
        }

        int tickStep = (updateSource & UpdateType.Update1) != 0 ? 1 : (updateSource & UpdateType.Update10) != 0 ? 10 : 0;
        if (tickStep == 0) return;
        HandleInput();
        HandleLights(tickStep);

        autopilotTickCounter += tickStep;
        if (autopilotTickCounter >= autopilotCheckTicks)
        {
            autopilotTickCounter = 0;
            if (rc != null && rc.IsAutoPilotEnabled && autoPilotTarget != Vector3D.Zero)
            {
                double dist = Vector3D.Distance(rc.GetPosition(), autoPilotTarget);
                if (dist < 200)
                {
                    rc.SetAutoPilotEnabled(false);
                    autoPilotTarget = Vector3D.Zero;
                    DrawMenu();
                }
            }
        }

        if (isAligning) AlignToTarget();
        else if (isMenuActive) FreezeGyros();

        tickCounter += tickStep;
        gpsTickCounter += tickStep;
        raycastTickCounter += tickStep;
        telemetryTickCounter += tickStep;
        diagnosticsTickCounter += tickStep;
        blinkTickCounter += tickStep;
        if (scanDisplayTimer > 0) scanDisplayTimer = Math.Max(0, scanDisplayTimer - tickStep);

        if (raycastTickCounter >= 7200)
        {
            foreach (var cam in cameras) if (cam != null && !cam.EnableRaycast) cam.EnableRaycast = true;
            raycastTickCounter = 0;
        }

        if (gpsTickCounter >= 120)
        {
            ParseGPSData();
            gpsTickCounter = 0;
        }

        if (telemetryTickCounter >= 120)
        {
            UpdateTelemetry();
            telemetryTickCounter = 0;
        }

        if (diagnosticsTickCounter >= diagnosticsRefreshTicks) UpdateDiagnostics();

        int uiRefreshTicks = isMenuActive ? activeUiRefreshTicks : idleUiRefreshTicks;
        if (tickCounter >= uiRefreshTicks)
        {
            DrawMenu();
        }
        UpdateRuntimeFrequency();
    }
    catch (Exception ex)
    {
        if (display == null) return;
        using (var frame = display.DrawFrame())
        {
            var errorSprite = new MySprite() { Type = SpriteType.TEXT, Data = "CRASH: " + ex.Message, Position = new Vector2(20, 20), RotationOrScale = 0.8f * textScale, Color = foregroundColor, FontId = textFont };
            frame.Add(errorSprite);
        }
    }
}

void ParseGPSData(bool force = false)
{
    gpsTickCounter = 0;
    string customData = Me.CustomData ?? "";
    if (!force && customData == lastParsedCustomData) return;
    lastParsedCustomData = customData;
    rootPoints.Clear();
    folders.Clear();
    folderOrder.Clear();
    totalGpsCount = 0;
    string currentFolder = "";
    string[] lines = customData.Split('\n');

    foreach (var line in lines)
    {
        string trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed)) continue;

        if (trimmed.StartsWith("[") && trimmed.EndsWith("]") && !trimmed.Contains("GPS:"))
        {
            currentFolder = trimmed.Substring(1, trimmed.Length - 2).Trim();
            if (!folders.ContainsKey(currentFolder))
            {
                folders[currentFolder] = new List<JumpPoint>();
                folderOrder.Add(currentFolder);
            }
            continue;
        }

        if (!trimmed.StartsWith("GPS:")) continue;
        string[] parts = trimmed.Split(':');
        if (parts.Length < 6) continue;

        double x, y, z;
        if (!double.TryParse(parts[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out x) ||
            !double.TryParse(parts[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out y) ||
            !double.TryParse(parts[4], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out z)) continue;

        var pt = new JumpPoint { Name = parts[1], Coords = new Vector3D(x, y, z) };
        totalGpsCount++;
        if (currentFolder != "") folders[currentFolder].Add(pt);
        else rootPoints.Add(pt);
    }
    BuildDisplayList();
}

void BuildDisplayList()
{
    displayPoints.Clear();
    if (currentActiveFolder == "")
    {
        displayPoints.Add(new JumpPoint { Name = "SCAN - DISTANCE", IsScan = true });
        displayPoints.AddRange(rootPoints);
        foreach (var folderName in folderOrder) displayPoints.Add(new JumpPoint { Name = "[+] " + folderName, IsFolder = true, FolderRef = folderName });
    }
    else
    {
        displayPoints.Add(new JumpPoint { Name = "< BACK TO MAIN", IsBackBtn = true });
        if (folders.ContainsKey(currentActiveFolder)) displayPoints.AddRange(folders[currentActiveFolder]);
        else
        {
            currentActiveFolder = "";
            selectedPointRow = rootCursorRow;
            BuildDisplayList();
            return;
        }
    }

    if (displayPoints.Count == 0) displayPoints.Add(new JumpPoint { Name = "NO DATA", IsBackBtn = true });
    if (selectedPointRow >= displayPoints.Count) selectedPointRow = Math.Max(0, displayPoints.Count - 1);
}

int FindFolderRow(string folderRef)
{
    for (int i = 0; i < displayPoints.Count; i++) if (displayPoints[i].IsFolder && displayPoints[i].FolderRef == folderRef) return i;
    return Math.Max(0, Math.Min(rootCursorRow, displayPoints.Count - 1));
}

void OpenFolder(string folderRef)
{
    rootCursorRow = selectedPointRow;
    currentActiveFolder = folderRef;
    int savedRow;
    selectedPointRow = folderCursorRows.TryGetValue(folderRef, out savedRow) ? savedRow : 0;
    BuildDisplayList();
}

void ReturnToRoot()
{
    string folderRef = currentActiveFolder;
    if (folderRef != "") folderCursorRows[folderRef] = selectedPointRow;
    currentActiveFolder = "";
    selectedPointRow = rootCursorRow;
    BuildDisplayList();
    if (folderRef != "") selectedPointRow = FindFolderRow(folderRef);
}

void ToggleCockpitControl(bool enable) { if (cockpit == null) return; cockpit.ControlThrusters = enable; cockpit.ControlWheels = enable; }

void UpdateRuntimeFrequency()
{
    UpdateFrequency requiredFrequency = isMenuActive || isAligning || greenBlinksLeft > 0 ? UpdateFrequency.Update1 : UpdateFrequency.Update10;
    if (currentUpdateFrequency == requiredFrequency) return;
    currentUpdateFrequency = requiredFrequency;
    Runtime.UpdateFrequency = requiredFrequency;
}

void FreezeGyros()
{
    if (gyrosFrozen) return;
    foreach (var gyro in gyros)
    {
        if (gyro == null) continue;
        gyro.GyroOverride = true;
        gyro.Pitch = gyro.Yaw = gyro.Roll = 0f;
    }
    gyrosFrozen = true;
}

void ResetGyros()
{
    foreach (var gyro in gyros) if (gyro != null) gyro.GyroOverride = false;
    gyrosFrozen = false;
}

void AlignToTarget()
{
    if (cockpit == null || gyros.Count == 0) { isAligning = false; return; }
    gyrosFrozen = false;
    Vector3D forward = cockpit.WorldMatrix.Forward;
    Vector3D targetDir = Vector3D.Normalize(alignTarget - cockpit.GetPosition());
    double angle = Math.Acos(MathHelper.Clamp(Vector3D.Dot(forward, targetDir), -1.0, 1.0));
    if (double.IsNaN(angle) || angle < 0.002) { isAligning = false; DrawMenu(); return; }

    Vector3D axis = Math.Abs(angle - Math.PI) < 0.05 ? cockpit.WorldMatrix.Up : Vector3D.Normalize(Vector3D.Cross(forward, targetDir));
    double speed = Math.Min(angle * 0.5, 1.0);
    foreach (var gyro in gyros)
    {
        if (gyro == null) continue;
        gyro.GyroOverride = true;
        Vector3D localRotation = Vector3D.TransformNormal(axis, MatrixD.Transpose(gyro.WorldMatrix));
        gyro.Pitch = (float)(-localRotation.X * speed);
        gyro.Yaw = (float)(-localRotation.Y * speed);
        gyro.Roll = (float)(-localRotation.Z * speed);
    }
}

void HandleInput()
{
    if (cockpit == null) return;
    Vector3 currentMove = cockpit.MoveIndicator;
    float currentRoll = cockpit.RollIndicator;

    if (!cockpit.IsUnderControl && isMenuActive)
    {
        isMenuActive = false;
        isAligning = false;
        isActionMenuOpen = false;
        ResetGyros();
        ToggleCockpitControl(true);
        UpdateDiagnostics();
        DrawMenu();
        return;
    }

    if (!isMenuActive)
    {
        lastMoveIndicator = currentMove;
        lastRollIndicator = currentRoll;
        return;
    }

    bool wPressed = currentMove.Z < -0.5f && lastMoveIndicator.Z >= -0.5f;
    bool sPressed = currentMove.Z > 0.5f && lastMoveIndicator.Z <= 0.5f;
    bool aPressed = currentMove.X < -0.5f && lastMoveIndicator.X >= -0.5f;
    bool dPressed = currentMove.X > 0.5f && lastMoveIndicator.X <= 0.5f;
    bool spacePressed = currentMove.Y > 0.5f && lastMoveIndicator.Y <= 0.5f;
    bool qPressed = currentRoll < -0.5f && lastRollIndicator >= -0.5f;
    bool ePressed = currentRoll > 0.5f && lastRollIndicator <= 0.5f;
    bool actionMade = false;

    if (qPressed) { activeTab = 0; isActionMenuOpen = false; actionMade = true; }
    if (ePressed) { activeTab = 1; isActionMenuOpen = false; actionMade = true; }

    if (activeTab == 0)
    {
        if (aPressed) { selectedDigit = Math.Max(0, selectedDigit - 1); actionMade = true; }
        if (dPressed) { selectedDigit = Math.Min(4, selectedDigit + 1); actionMade = true; }
        if (wPressed) { AdjustValue(1f); actionMade = true; }
        if (sPressed) { AdjustValue(-1f); actionMade = true; }
        if (spacePressed) { ApplyDistance(); isAligning = false; actionMade = true; }
    }
    else if (isActionMenuOpen)
    {
        if (wPressed) { if (selectedActionRow > 0) selectedActionRow--; actionMade = true; }
        if (sPressed) { if (selectedActionRow < 2) selectedActionRow++; actionMade = true; }
        if (aPressed) { isActionMenuOpen = false; actionMade = true; }

        if (spacePressed || dPressed)
        {
            if (displayPoints.Count == 0 || selectedPointRow < 0 || selectedPointRow >= displayPoints.Count)
            {
                isActionMenuOpen = false;
                actionMade = true;
            }
            else
            {
                var item = displayPoints[selectedPointRow];
                if (selectedActionRow == 0)
                {
                    calcDistance = (float)Math.Round(Vector3D.Distance(cockpit.GetPosition(), item.Coords) / 1000.0, 1);
                    ApplyDistance();
                    alignTarget = item.Coords;
                    isAligning = true;
                }
                else if (selectedActionRow == 1 && rc != null)
                {
                    autoPilotTarget = item.Coords;
                    rc.ClearWaypoints();
                    rc.AddWaypoint(new Sandbox.ModAPI.Ingame.MyWaypointInfo(item.Name, item.Coords));
                    rc.FlightMode = Sandbox.ModAPI.Ingame.FlightMode.OneWay;
                    rc.SetCollisionAvoidance(true);
                    rc.SetAutoPilotEnabled(true);
                }
                isActionMenuOpen = false;
                actionMade = true;
            }
        }
    }
    else
    {
        if (wPressed) { if (selectedPointRow > 0) selectedPointRow--; actionMade = true; }
        if (sPressed) { if (selectedPointRow < displayPoints.Count - 1) selectedPointRow++; actionMade = true; }

        if (aPressed && currentActiveFolder != "")
        {
            ReturnToRoot();
            actionMade = true;
        }

        if (dPressed && displayPoints.Count > 0)
        {
            var item = displayPoints[selectedPointRow];
            if (item.IsFolder)
            {
                OpenFolder(item.FolderRef);
                actionMade = true;
            }
        }

        if (spacePressed && displayPoints.Count > 0)
        {
            var item = displayPoints[selectedPointRow];
            if (item.IsScan) PerformRaycast();
            else if (item.IsBackBtn) ReturnToRoot();
            else if (item.IsFolder) OpenFolder(item.FolderRef);
            else { isActionMenuOpen = true; selectedActionRow = 0; }
            actionMade = true;
        }
    }

    lastMoveIndicator = currentMove;
    lastRollIndicator = currentRoll;
    if (actionMade) DrawMenu();
}

float GetChargePct()
{
    float currentPwr = 0f;
    float maxPwr = 0f;
    foreach (var jd in jumpDrives)
    {
        if (jd == null) continue;
        currentPwr += jd.CurrentStoredPower;
        maxPwr += jd.MaxStoredPower;
    }
    return maxPwr > 0f ? currentPwr / maxPwr : -1f;
}

void RefreshJumpCharge()
{
    cachedJumpCharge = GetChargePct();
    lightChargeTickCounter = 0;
}

void UpdateTelemetry()
{
    telemetryTickCounter = 0;
    telemetryMaxJumpKm = 0f;
    RefreshJumpCharge();
    telemetryJumpCharge = cachedJumpCharge;
    foreach (var jd in jumpDrives) if (jd != null) telemetryMaxJumpKm = Math.Max(telemetryMaxJumpKm, jd.MaxJumpDistanceMeters / 1000f);

    float batteryPwr = 0f;
    float batteryMax = 0f;
    foreach (var battery in batteries)
    {
        if (battery == null) continue;
        batteryPwr += battery.CurrentStoredPower;
        batteryMax += battery.MaxStoredPower;
    }
    telemetryBatteryCharge = batteryMax > 0f ? batteryPwr / batteryMax : -1f;

    double cargoVolume = 0;
    double cargoMax = 0;
    foreach (var block in inventoryBlocks)
    {
        if (block == null || !block.HasInventory) continue;
        for (int i = 0; i < block.InventoryCount; i++)
        {
            var inventory = block.GetInventory(i);
            if (inventory == null) continue;
            cargoVolume += (double)inventory.CurrentVolume;
            cargoMax += (double)inventory.MaxVolume;
        }
    }
    telemetryCargoFill = cargoMax > 0 ? (float)(cargoVolume / cargoMax) : -1f;
    telemetryMassKg = cockpit != null ? cockpit.CalculateShipMass().TotalMass : 0f;
    telemetryCoords = cockpit != null ? cockpit.GetPosition() : rc != null ? rc.GetPosition() : Me.GetPosition();
}

void DisableLights()
{
    if (lightsDisabled && disabledLightsTickCounter < disabledLightsRefreshTicks) return;
    disabledLightsTickCounter = 0;
    foreach (var l in lights) if (l != null && l.Enabled) l.Enabled = false;
    lightsDisabled = true;
}

void HandleLights(int tickStep)
{
    if (lights.Count == 0) return;
    lightChargeTickCounter += tickStep;
    disabledLightsTickCounter += tickStep;
    if (lightChargeTickCounter >= lightChargeRefreshTicks) RefreshJumpCharge();
    float chargePct = cachedJumpCharge;
    if (chargePct < 0f)
    {
        wasCharging = false;
        greenBlinksLeft = 0;
        lightTickCounter = 0;
        DisableLights();
        return;
    }

    lightTickCounter += tickStep;
    if (chargePct < 0.999f)
    {
        wasCharging = true;
        greenBlinksLeft = 0;
        if (lightTickCounter < 30) return;
        lightTickCounter = 0;
        lightToggleState = !lightToggleState;
        foreach (var l in lights)
        {
            if (l == null) continue;
            l.Color = foregroundColor;
            l.Enabled = lightToggleState;
        }
        lightsDisabled = !lightToggleState;
        if (lightsDisabled) disabledLightsTickCounter = 0;
        return;
    }

    if (wasCharging)
    {
        wasCharging = false;
        greenBlinksLeft = 10;
        lightTickCounter = 0;
    }

    if (greenBlinksLeft <= 0) { DisableLights(); return; }
    if (lightTickCounter < 6) return;
    lightTickCounter = 0;
    greenBlinksLeft--;
    bool isOn = greenBlinksLeft % 2 != 0;
    foreach (var l in lights)
    {
        if (l == null) continue;
        l.Color = foregroundColor;
        l.Enabled = isOn;
    }
    lightsDisabled = !isOn;
    if (lightsDisabled) disabledLightsTickCounter = 0;
}

void PerformRaycast()
{
    if (cameras.Count == 0)
    {
        lastScanResult = "NO CAMERA";
        scanDisplayTimer = 180;
        return;
    }

    double maxScanMeters = maxScanDistanceKm * 1000f;
    bool hasHit = false;
    string bestHitName = "";
    double bestDistMeters = double.MaxValue;
    float[] pSpread = { 0f, -0.2f, 0.2f, 0f, 0f, -0.2f, -0.2f, 0.2f, 0.2f, -0.4f, 0.4f, 0f, 0f, -0.4f, -0.4f, 0.4f, 0.4f };
    float[] ySpread = { 0f, 0f, 0f, -0.2f, 0.2f, -0.2f, 0.2f, -0.2f, 0.2f, 0f, 0f, -0.4f, 0.4f, -0.4f, 0.4f, -0.4f, 0.4f };
    int shotCount = 0;

    foreach (var cam in cameras)
    {
        if (cam == null) continue;
        if (!cam.EnableRaycast) cam.EnableRaycast = true;
        double currentScanRange = Math.Min(maxScanMeters, cam.AvailableScanRange);
        if (currentScanRange < 5000) continue;

        int index = shotCount % pSpread.Length;
        MyDetectedEntityInfo info = cam.Raycast(currentScanRange, pSpread[index], ySpread[index]);
        shotCount++;
        if (info.IsEmpty() || !info.HitPosition.HasValue) continue;

        double dist = Vector3D.Distance(cam.GetPosition(), info.HitPosition.Value);
        if (dist >= bestDistMeters) continue;
        bestDistMeters = dist;
        bestHitName = info.Name;
        hasHit = true;
    }

    if (hasHit)
    {
        calcDistance = (float)Math.Round(Math.Max(5000, bestDistMeters - 300) / 1000.0, 1);
        ApplyDistance();
        lastScanResult = $"HIT: {bestHitName}";
    }
    else if (shotCount == 0)
    {
        lastScanResult = "CHG CAMERAS";
    }
    else
    {
        lastScanResult = $"NO HIT ({shotCount} SHT)";
    }
    scanDisplayTimer = 180;
}

void AdjustValue(float sign)
{
    float amount = selectedDigit == 0 ? 10000f : selectedDigit == 1 ? 1000f : selectedDigit == 2 ? 100f : selectedDigit == 3 ? 10f : 1f;
    calcDistance = MathHelper.Clamp(calcDistance + amount * sign, 5f, 99999f);
}

void ApplyDistance()
{
    if (jumpDrives.Count == 0 || jumpDrives[0] == null) return;
    calcDistance = MathHelper.Clamp(calcDistance, 5f, jumpDrives[0].MaxJumpDistanceMeters / 1000f);
    foreach (var jd in jumpDrives) if (jd != null) jd.JumpDistanceMeters = calcDistance * 1000f;
}

void UpdateDiagnostics()
{
    diagnosticsTickCounter = 0;
    diagText.Clear();
    diagText.AppendLine("--- NEXUS STATUS ---");
    diagText.AppendLine($"Group '{systemGroupName}': {(isGroupFound ? "FOUND" : "MISSING!")}");
    diagText.AppendLine($"Cockpit: {(cockpit != null ? "OK" : "MISSING!")}");
    diagText.AppendLine($"Remote Ctrl: {(rc != null ? "OK" : "MISSING!")}");
    diagText.AppendLine($"Ext Displays: {extDisplays.Count}");
    diagText.AppendLine($"Lights: {lights.Count}");
    diagText.AppendLine($"Cameras: {cameras.Count}");
    diagText.AppendLine($"Jump Drives: {jumpDrives.Count}");
    diagText.AppendLine($"Gyros (Grid): {gyros.Count}");
    diagText.AppendLine("----------------------");
    diagText.AppendLine($"Folders: {folders.Count} | Total Points: {totalGpsCount}");
    diagText.AppendLine($"Aligning: {isAligning}");
    string diagnosticsText = diagText.ToString();
    Echo(diagnosticsText);
    if (pbSurface != null && diagnosticsText != lastDiagnosticsText)
    {
        pbSurface.WriteText(diagnosticsText);
        lastDiagnosticsText = diagnosticsText;
    }
}

Vector2 Snap(Vector2 value) { return new Vector2((float)Math.Round(value.X), (float)Math.Round(value.Y)); }

void DrawTextSprite(MySpriteDrawFrame frame, string text, Vector2 position, float scale, VRageMath.Color color, TextAlignment alignment = TextAlignment.LEFT) { frame.Add(new MySprite() { Type = SpriteType.TEXT, Data = text, Position = Snap(position), RotationOrScale = scale * textScale, Color = color, Alignment = alignment, FontId = textFont }); }

void DrawBoldTextSprite(MySpriteDrawFrame frame, string text, Vector2 position, float scale, VRageMath.Color color, TextAlignment alignment = TextAlignment.LEFT)
{
    DrawTextSprite(frame, text, position, scale, color, alignment);
    DrawTextSprite(frame, text, new Vector2(position.X + 1f, position.Y), scale, color, alignment);
}

void DrawActiveTextSprite(MySpriteDrawFrame frame, string text, Vector2 position, float scale, VRageMath.Color color, bool isActive, TextAlignment alignment = TextAlignment.LEFT)
{
    if (isActive) DrawBoldTextSprite(frame, text, position, scale, color, alignment);
    else DrawTextSprite(frame, text, position, scale, color, alignment);
}

void DrawRect(MySpriteDrawFrame frame, Vector2 position, Vector2 size, VRageMath.Color color) { frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = "SquareSimple", Position = Snap(position), Size = Snap(size), Color = color, Alignment = TextAlignment.CENTER }); }

void DrawTextureSprite(MySpriteDrawFrame frame, string texture, Vector2 position, Vector2 size, VRageMath.Color color) { frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = texture, Position = Snap(position), Size = Snap(size), Color = color, Alignment = TextAlignment.CENTER }); }

void DrawTiledGrid(MySpriteDrawFrame frame, RectangleF viewport)
{
    Vector2 tileSize = new Vector2(viewport.Width / 2f, viewport.Height / 2f);
    float leftX = viewport.X + tileSize.X / 2f;
    float rightX = viewport.X + tileSize.X * 1.5f;
    float topY = viewport.Y + tileSize.Y / 2f;
    float bottomY = viewport.Y + tileSize.Y * 1.5f;
    DrawTextureSprite(frame, "Grid", new Vector2(leftX, topY), tileSize, gridTextureColor);
    DrawTextureSprite(frame, "Grid", new Vector2(rightX, topY), tileSize, gridTextureColor);
    DrawTextureSprite(frame, "Grid", new Vector2(leftX, bottomY), tileSize, gridTextureColor);
    DrawTextureSprite(frame, "Grid", new Vector2(rightX, bottomY), tileSize, gridTextureColor);
}

void DrawScreenOverlay(MySpriteDrawFrame frame, RectangleF viewport) { DrawTextureSprite(frame, "LCD_Economy_Clear", viewport.Center, new Vector2(viewport.Width * 1.2f, viewport.Height * 1.2f), lcdOverlayColor); }

void DrawRoundedRect(MySpriteDrawFrame frame, RectangleF rect, float radius, VRageMath.Color color)
{
    float left = (float)Math.Round(rect.X);
    float top = (float)Math.Round(rect.Y);
    float right = (float)Math.Round(rect.X + rect.Width);
    float bottom = (float)Math.Round(rect.Y + rect.Height);
    float width = right - left;
    float height = bottom - top;
    float r = Math.Max(1f, (float)Math.Round(Math.Min(radius, Math.Min(width, height) / 2f)));
    if (r < 7f)
    {
        int steps = Math.Max(1, Math.Min(3, (int)r));
        Vector2 center = new Vector2((left + right) / 2f, (top + bottom) / 2f);
        for (int i = 0; i <= steps; i++)
        {
            float inset = steps - i;
            DrawRect(frame, center, new Vector2(width - inset * 2f, height - i * 2f), color);
        }
        return;
    }
    float diameter = r * 2f;
    Vector2 size = new Vector2(diameter, diameter);
    frame.Add(MySprite.CreateClipRect(new Rectangle((int)left, (int)top, (int)width, (int)height)));
    frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = "Circle", Position = new Vector2(left + r, top + r), Size = size, Color = color, Alignment = TextAlignment.CENTER });
    frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = "Circle", Position = new Vector2(right - r, top + r), Size = size, Color = color, Alignment = TextAlignment.CENTER });
    frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = "Circle", Position = new Vector2(left + r, bottom - r), Size = size, Color = color, Alignment = TextAlignment.CENTER });
    frame.Add(new MySprite() { Type = SpriteType.TEXTURE, Data = "Circle", Position = new Vector2(right - r, bottom - r), Size = size, Color = color, Alignment = TextAlignment.CENTER });
    DrawRect(frame, new Vector2((left + right) / 2f, (top + bottom) / 2f), new Vector2(width - diameter, height), color);
    DrawRect(frame, new Vector2((left + right) / 2f, (top + bottom) / 2f), new Vector2(width, height - diameter), color);
    frame.Add(MySprite.CreateClearClipRect());
}

string FormatPct(float value) { return value < 0f ? "--" : $"{Math.Round(value * 100)}%"; }

string FormatMass(float value) { return $"{Math.Round(value)} KG"; }

void DrawProgressBar(MySpriteDrawFrame frame, float x, float y, float width, float height, float value)
{
    DrawRect(frame, new Vector2(x + width / 2f, y + height / 2f), new Vector2(width, height), foregroundColor);
    float inset = 1f;
    float emptyWidth = (width - inset * 2f) * (1f - MathHelper.Clamp(value, 0f, 1f));
    if (emptyWidth < 1f) return;
    DrawRect(frame, new Vector2(x + width - inset - emptyWidth / 2f, y + height / 2f), new Vector2(emptyWidth, height - inset * 2f), backgroundColor);
}

void DrawTelemetry(MySpriteDrawFrame frame, float x, float y, float width, float currentScale)
{
    float pad = 10f * currentScale;
    float lineH = 17f * currentScale;
    float textSize = currentScale * 0.55f;
    float barW = 80f * currentScale;
    float barH = Math.Max(7f, 10f * currentScale);
    x += pad;
    y += 8f * currentScale;
    float valueX = x + 82f * currentScale;

    DrawTextSprite(frame, "SHIP TELEMETRY", new Vector2(x, y), textSize, foregroundColor);
    y += lineH;
    DrawTextSprite(frame, "MAX JUMP", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, telemetryJumpCharge >= 0.999f ? $"{Math.Round(telemetryMaxJumpKm, 1)} KM" : "NOT READY", new Vector2(valueX, y), textSize, foregroundColor);
    y += lineH;
    DrawTextSprite(frame, "JUMP PWR", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, FormatPct(telemetryJumpCharge), new Vector2(valueX, y), textSize, foregroundColor);
    DrawProgressBar(frame, x + width - pad * 2f - barW, y + 2f * currentScale, barW, barH, telemetryJumpCharge);
    y += lineH;
    DrawTextSprite(frame, "BATTERY", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, FormatPct(telemetryBatteryCharge), new Vector2(valueX, y), textSize, foregroundColor);
    DrawProgressBar(frame, x + width - pad * 2f - barW, y + 2f * currentScale, barW, barH, telemetryBatteryCharge);
    y += lineH;
    DrawTextSprite(frame, "CARGO", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, FormatPct(telemetryCargoFill), new Vector2(valueX, y), textSize, foregroundColor);
    DrawProgressBar(frame, x + width - pad * 2f - barW, y + 2f * currentScale, barW, barH, telemetryCargoFill);
    y += lineH;
    DrawTextSprite(frame, "MASS", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, FormatMass(telemetryMassKg), new Vector2(valueX, y), textSize, foregroundColor);
    y += lineH * 1.3f;
    DrawTextSprite(frame, "POSITION", new Vector2(x, y), textSize, foregroundColor);
    y += lineH;
    DrawTextSprite(frame, "X", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, $"{Math.Round(telemetryCoords.X)}", new Vector2(valueX, y), textSize, foregroundColor);
    y += lineH;
    DrawTextSprite(frame, "Y", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, $"{Math.Round(telemetryCoords.Y)}", new Vector2(valueX, y), textSize, foregroundColor);
    y += lineH;
    DrawTextSprite(frame, "Z", new Vector2(x, y), textSize, foregroundColor);
    DrawTextSprite(frame, $"{Math.Round(telemetryCoords.Z)}", new Vector2(valueX, y), textSize, foregroundColor);
}

void DrawMenu()
{
    tickCounter = 0;
    if (display != null) RenderUI(display, uiScale, cockpitVisibleRows);
    foreach (var extDisplay in extDisplays) if (extDisplay != null) RenderUI(extDisplay, extUiScale, extVisibleRows);
}

void RenderUI(IMyTextSurface surface, float currentScale, int visibleRows)
{
    RectangleF viewport = new RectangleF((surface.TextureSize - surface.SurfaceSize) / 2f, surface.SurfaceSize);
    using (var frame = surface.DrawFrame())
    {
        float frameThickness = Math.Max(8f, 14f * currentScale);
        float cornerRadius = Math.Max(8f, 16f * currentScale);
        float contentInset = 8f * currentScale;
        float headerGap = Math.Max(3f, 5f * currentScale);
        float headerLift = Math.Max(3f, 5f * currentScale);
        Vector2 frameCenter = Snap(viewport.Center);
        float screenW = (float)Math.Floor(viewport.Width - frameThickness * 2f);
        float screenH = (float)Math.Floor(viewport.Height - frameThickness * 2f);
        if ((int)screenW % 2 != 0) screenW--;
        if ((int)screenH % 2 != 0) screenH--;
        RectangleF screenRect = new RectangleF(frameCenter.X - screenW / 2f, frameCenter.Y - screenH / 2f, screenW, screenH);
        DrawRect(frame, frameCenter, new Vector2(viewport.Width, viewport.Height), foregroundColor);
        DrawTiledGrid(frame, viewport);
        float headerHeight = 35f * currentScale;
        RectangleF headerRect = new RectangleF(screenRect.X, screenRect.Y - headerLift, screenRect.Width, headerHeight);
        float mainTop = headerRect.Y + headerRect.Height + headerGap;
        RectangleF mainRect = new RectangleF(screenRect.X, mainTop, screenRect.Width, screenRect.Y + screenRect.Height - mainTop);
        RectangleF contentRect = new RectangleF(mainRect.X + contentInset, mainRect.Y + contentInset, mainRect.Width - contentInset * 2f, mainRect.Height - contentInset * 2f);
        DrawRoundedRect(frame, mainRect, cornerRadius, backgroundColor);

        float chargePct = cachedJumpCharge;
        float currentKm = jumpDrives.Count > 0 && jumpDrives[0] != null ? jumpDrives[0].JumpDistanceMeters / 1000f : 0f;
        string sysStatus = "READY";
        if (isAligning) sysStatus = "ALIGNING";
        else if (rc != null && rc.IsAutoPilotEnabled) sysStatus = "AUTO ON";
        else if (chargePct < 0f) sysStatus = "NO DRIVE";
        else if (chargePct < 0.999f) sysStatus = $"CHG {(int)(chargePct * 100)}%";
        if (scanDisplayTimer > 0) sysStatus = lastScanResult;

        float leftX = headerRect.X + 8f * currentScale;
        float rightX = headerRect.X + headerRect.Width - 8f * currentScale;
        float headerTextY = headerRect.Y + 2f * currentScale;
        float headerTextSize = currentScale * 1.08f;
        DrawBoldTextSprite(frame, "NEXUS - JA", new Vector2(leftX, headerTextY), headerTextSize, backgroundColor);
        DrawBoldTextSprite(frame, $"RANGE {Math.Round(currentKm, 1)} KM", new Vector2(headerRect.Center.X + 18f * currentScale, headerTextY), headerTextSize, backgroundColor, TextAlignment.CENTER);

        if (!isMenuActive || (blinkTickCounter / 30) % 2 == 0) DrawBoldTextSprite(frame, sysStatus, new Vector2(rightX, headerTextY), headerTextSize, backgroundColor, TextAlignment.RIGHT);

        float columnGap = 8f * currentScale;
        float colWidth = (contentRect.Width - columnGap) / 2f;
        float tabHeight = 25f * currentScale;
        float boxesY = contentRect.Y + tabHeight / 2f;
        Vector2 leftColPos = new Vector2(contentRect.X + colWidth / 2f, boxesY);
        Vector2 rightColPos = new Vector2(contentRect.X + contentRect.Width - colWidth / 2f, boxesY);
        bool leftActive = activeTab == 0 && isMenuActive;
        bool rightActive = activeTab == 1 && isMenuActive;
        float tabBgY = boxesY - tabHeight / 2f + 1f;
        if (leftActive) DrawRoundedRect(frame, new RectangleF(leftColPos.X - colWidth / 2f, tabBgY, colWidth, tabHeight), 5f * currentScale, foregroundColor);
        if (rightActive) DrawRoundedRect(frame, new RectangleF(rightColPos.X - colWidth / 2f, tabBgY, colWidth, tabHeight), 5f * currentScale, foregroundColor);
        float tabTextY = boxesY - 10f * currentScale;
        DrawActiveTextSprite(frame, leftActive ? "TARGET" : "TARGET [Q]", new Vector2(leftColPos.X - colWidth / 2f + 5f * currentScale, tabTextY), currentScale * 0.8f, leftActive ? backgroundColor : foregroundColor, leftActive);
        DrawActiveTextSprite(frame, rightActive ? "POINTS" : "POINTS [E]", new Vector2(rightColPos.X - colWidth / 2f + 5f * currentScale, tabTextY), currentScale * 0.8f, rightActive ? backgroundColor : foregroundColor, rightActive);

        float fixedLeftMenuY = boxesY + 39f * currentScale;
        float controlBgY = fixedLeftMenuY - 19f * currentScale;
        float controlBgHeight = 62f * currentScale;
        if (leftActive) DrawRoundedRect(frame, new RectangleF(leftColPos.X - colWidth / 2f, controlBgY, colWidth, controlBgHeight), 8f * currentScale, foregroundColor);
        VRageMath.Color leftControlColor = leftActive ? backgroundColor : foregroundColor;
        string distStr = ((int)calcDistance).ToString("D5");
        float charWidth = 16f * currentScale;
        float startX = leftColPos.X - 2f * charWidth;
        DrawActiveTextSprite(frame, "SET:", new Vector2(startX - 15f * currentScale, fixedLeftMenuY), currentScale, leftControlColor, leftActive, TextAlignment.RIGHT);
        DrawActiveTextSprite(frame, distStr, new Vector2(leftColPos.X, fixedLeftMenuY), currentScale, leftControlColor, leftActive, TextAlignment.CENTER);
        if (leftActive)
        {
            float selectedDigitX = startX + selectedDigit * charWidth;
            DrawTextSprite(frame, "^", new Vector2(selectedDigitX, fixedLeftMenuY - 16f * currentScale), currentScale * 0.7f, leftControlColor, TextAlignment.CENTER);
            DrawTextSprite(frame, "v", new Vector2(selectedDigitX, fixedLeftMenuY + 24f * currentScale), currentScale * 0.7f, leftControlColor, TextAlignment.CENTER);
        }
        DrawTelemetry(frame, leftColPos.X - colWidth / 2f + 8f * currentScale, fixedLeftMenuY + 50f * currentScale, colWidth - 16f * currentScale, currentScale);
        DrawTextSprite(frame, $"VER {scriptVersion}", new Vector2(mainRect.X + 8f * currentScale, mainRect.Y + mainRect.Height - 15f * currentScale), currentScale * 0.45f, foregroundColor);

        float rightContentY = boxesY + 20f * currentScale;
        if (activeTab == 1 && isActionMenuOpen && displayPoints.Count > 0 && selectedPointRow < displayPoints.Count)
        {
            var item = displayPoints[selectedPointRow];
            DrawTextSprite(frame, "// SELECT ACTION //", new Vector2(rightColPos.X, rightContentY), currentScale * 0.7f, foregroundColor, TextAlignment.CENTER);
            DrawTextSprite(frame, item.Name, new Vector2(rightColPos.X, rightContentY + 25f * currentScale), currentScale * 0.7f, foregroundColor, TextAlignment.CENTER);
            string[] menuOpts = { "JUMP ALIGN", "AUTOPILOT", "< CANCEL" };
            for (int i = 0; i < 3; i++)
            {
                float py = rightContentY + (i + 3) * 25f * currentScale;
                bool isSelected = selectedActionRow == i;
                if (isSelected) DrawRoundedRect(frame, new RectangleF(rightColPos.X - colWidth / 2f + 4f * currentScale, py + 1f, colWidth - 8f * currentScale, 20f * currentScale), 5f * currentScale, foregroundColor);
                DrawTextSprite(frame, "  " + menuOpts[i], new Vector2(rightColPos.X - colWidth / 2f + 10f * currentScale, py), currentScale * 0.7f, isSelected ? backgroundColor : foregroundColor);
            }
            if (rc == null && selectedActionRow == 1) DrawTextSprite(frame, "NO REMOTE CONTROL!", new Vector2(rightColPos.X, rightContentY + 7 * 25f * currentScale), currentScale * 0.6f, foregroundColor, TextAlignment.CENTER);
            DrawScreenOverlay(frame, viewport);
            return;
        }

        int currentScrollOffset = 0;
        if (visibleRows < displayPoints.Count)
        {
            int middle = visibleRows / 2;
            currentScrollOffset = Math.Max(0, Math.Min(selectedPointRow - middle, displayPoints.Count - visibleRows));
            if (currentScrollOffset > 0 && selectedPointRow == currentScrollOffset) currentScrollOffset--;
            if (currentScrollOffset + visibleRows < displayPoints.Count && selectedPointRow == currentScrollOffset + visibleRows - 1) currentScrollOffset++;
        }

        for (int i = 0; i < visibleRows; i++)
        {
            int pointIdx = i + currentScrollOffset;
            if (pointIdx >= displayPoints.Count) break;
            float py = rightContentY + i * 25f * currentScale;
            if (i == 0 && currentScrollOffset > 0)
            {
                DrawTextSprite(frame, ". . . ^ . . .", new Vector2(rightColPos.X, py), currentScale * 0.7f, foregroundColor, TextAlignment.CENTER);
                continue;
            }
            if (i == visibleRows - 1 && currentScrollOffset + visibleRows < displayPoints.Count)
            {
                DrawTextSprite(frame, ". . . v . . .", new Vector2(rightColPos.X, py), currentScale * 0.7f, foregroundColor, TextAlignment.CENTER);
                continue;
            }

            var item = displayPoints[pointIdx];
            bool isSelected = activeTab == 1 && selectedPointRow == pointIdx && isMenuActive;
            string pointName = "  " + item.Name;
            string distStrRight = "";
            if (item.IsScan) pointName = "  " + (cameras.Count > 0 && cameras[0] != null ? "SCAN - DISTANCE" : "SCAN // NO CAMERA");
            else if (!item.IsFolder && !item.IsBackBtn) distStrRight = cockpit != null ? $"{Math.Round(Vector3D.Distance(cockpit.GetPosition(), item.Coords) / 1000)}km" : "--";
            if (isSelected) DrawRoundedRect(frame, new RectangleF(rightColPos.X - colWidth / 2f + 4f * currentScale, py + 1f, colWidth - 8f * currentScale, 20f * currentScale), 5f * currentScale, foregroundColor);
            DrawTextSprite(frame, pointName, new Vector2(rightColPos.X - colWidth / 2f + 10f * currentScale, py), currentScale * 0.7f, isSelected ? backgroundColor : foregroundColor);
            DrawTextSprite(frame, distStrRight, new Vector2(rightColPos.X + colWidth / 2f - 10f * currentScale, py), currentScale * 0.7f, isSelected ? backgroundColor : foregroundColor, TextAlignment.RIGHT);
        }
        DrawScreenOverlay(frame, viewport);
    }
}

void FindBlocks()
{
    gyrosFrozen = false;
    lightsDisabled = false;
    disabledLightsTickCounter = disabledLightsRefreshTicks;
    jumpDrives.Clear();
    cameras.Clear();
    gyros.Clear();
    extDisplays.Clear();
    lights.Clear();
    batteries.Clear();
    inventoryBlocks.Clear();
    cockpit = null;
    rc = null;
    display = null;

    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(systemGroupName);
    isGroupFound = group != null;
    if (isGroupFound)
    {
        List<IMyCockpit> groupCockpits = new List<IMyCockpit>();
        List<IMyRemoteControl> groupRCs = new List<IMyRemoteControl>();
        group.GetBlocksOfType(groupCockpits);
        group.GetBlocksOfType(groupRCs);
        if (groupCockpits.Count > 0) cockpit = groupCockpits[0];
        if (groupRCs.Count > 0) rc = groupRCs[0];
        group.GetBlocksOfType(cameras);
        group.GetBlocksOfType(jumpDrives);
        group.GetBlocksOfType(extDisplays);
        group.GetBlocksOfType(lights);
        foreach (var ext in extDisplays)
        {
            ext.ContentType = ContentType.SCRIPT;
            ext.Script = "";
            ext.ScriptBackgroundColor = backgroundColor;
        }
        foreach (var cam in cameras) if (cam != null && !cam.EnableRaycast) cam.EnableRaycast = true;
    }

    GridTerminalSystem.GetBlocksOfType(gyros, g => g.CubeGrid == Me.CubeGrid);
    GridTerminalSystem.GetBlocksOfType(batteries, b => b.IsSameConstructAs(Me));
    GridTerminalSystem.GetBlocksOfType(inventoryBlocks, b => b.IsSameConstructAs(Me) && b.HasInventory);
    if (cockpit == null || screenIndex < 0 || screenIndex >= cockpit.SurfaceCount) return;
    display = cockpit.GetSurface(screenIndex);
    display.ContentType = ContentType.SCRIPT;
    display.Script = "";
    display.ScriptBackgroundColor = backgroundColor;
}
