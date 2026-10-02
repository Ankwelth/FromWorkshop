/*
=======================================================================
MPX DEFENSE SYSTEM - MAIN BRAIN
=======================================================================

*** UPDATED VERSION ***
- Now supports the "MPX Detection" block group
- Can read vanilla turrets and compatible modded radar/weapon/detection blocks
- Auto-refreshes detection blocks every few seconds
- Keeps all original MPX Defense features

--- DESCRIPTION ---
The MPX Defense System is a fully automated base defense and drone tracking 
network. It manages base alerts, coordinates automated guard drones, tracks 
enemies on a custom radar display, and dynamically responds to threats.

This script is 100% vanilla Programmable Block compatible. It is designed 
to work flawlessly on servers with modded weapons and radars, without 
requiring any plugin APIs or external server scripts.

--- REQUIRED BLOCK NAMES ---
- MPX LCD Panel
- MPX Drone LCD
- MPX Radar LCD
- MPX Antenna
- MPX Alarm

--- REQUIRED BLOCK GROUPS ---
- MPX Detection
- MPX Guard Bays
- MPX Alert Lights
- MPX Base Lights

--- HOW MPX DETECTION WORKS ---
Place any turret, radar, modded weapon, or detection block inside the 
"MPX Detection" group. The script safely scans these blocks. If a block 
exposes target/lock/detection data to the vanilla Programmable Block API, 
MPX will use it to trigger base alerts. 
* Vanilla Limitation: Some modded blocks do not expose target data to the 
  API; MPX cannot read hidden internal mod logic.

--- THREAT DETECTED ---
When a threat is detected:
- Alarm starts sounding
- Alert lights turn red (and base lights turn off)
- Antenna range increases to maximum alert range
- Guards automatically launch from their bays
- Radar and LCDs update with target data

--- THREAT CLEARED ---
When a threat clears (and the clear delay passes):
- Guards automatically return to base
- Alarm stops sounding
- Lights return to safe (base lights on, alert lights off)
- Antenna returns to idle range

--- SETUP STEPS ---
Step 1: Add all LCDs/antenna/alarm with exact names
Step 2: Create required block groups
Step 3: Add turrets/radar/weapons to MPX Detection
Step 4: Add guard connectors to MPX Guard Bays
Step 5: Compile and run script

--- CHANGELOG ---
- Added MPX Detection group
- Added generic detector reader
- Added auto-refresh
- Added active detector/source debug display
- Kept turret fallback
=======================================================================
*/

string LCD_NAME = "MPX LCD Panel";
string DRONE_LCD_NAME = "MPX Drone LCD";
string RADAR_LCD_NAME = "MPX Radar LCD";
string ANTENNA_NAME = "MPX Antenna";
string ALARM_NAME = "MPX Alarm";

string CONNECTOR_GROUP = "MPX Guard Bays";
string ALERT_LIGHT_GROUP = "MPX Alert Lights";
string BASE_LIGHT_GROUP = "MPX Base Lights";

string CMD_TAG = "MPX_CMD";
string STATUS_TAG = "MPX_STATUS";

float ANTENNA_ALERT_RANGE = 50000f;
float ANTENNA_IDLE_RANGE = 200f;

int CLEAR_DELAY_SECONDS = 5;
int DRONE_TIMEOUT_SECONDS = 8;
int LAST_ENEMY_MEMORY_SECONDS = 8;

double RADAR_RANGE_METERS = 500.0;
bool KEEP_LAST_ENEMY_BLIP = true;
int RADAR_TRAIL_LENGTH = 24;
bool RADAR_USE_BASE_ORIENTATION = true;

IMyTextPanel lcd;
IMyTextPanel droneLcd;
IMyTextPanel radarLcd;
IMyRadioAntenna antenna;
IMySoundBlock alarm;

string DETECTION_GROUP = "MPX Detection";

List<IMyShipConnector> connectors = new List<IMyShipConnector>();
List<IMyTerminalBlock> detectionBlocks = new List<IMyTerminalBlock>();
List<IMyLightingBlock> alertLights = new List<IMyLightingBlock>();
List<IMyLightingBlock> baseLights = new List<IMyLightingBlock>();

IMyBroadcastListener statusListener;

bool alertState = false;
bool alarmOn = false;

int tickCounter = 0;
int noThreatTicks = 0;
int clearDelayTicks = 0;
int droneTimeoutTicks = 0;
int lastEnemyMemoryTicks = 0;
int blockRefreshTicks = 0;

const int TICKS_PER_SECOND = 6;

Vector3D? lastEnemyPosition = null;
int lastEnemySeenTick = -999999;
int lastTargetingTurrets = 0;
int lastShootingTurrets = 0;
int activeDetectors = 0;
string lastTargetSource = "NONE";
bool lastTargetPosAvailable = false;

class GuardInfo
{
    public string Name = "";
    public string Type = "Unknown";
    public string State = "UNKNOWN";
    public Vector3D Position = new Vector3D(0, 0, 0);
    public double BatteryPct = 0;
    public int LastSeenTick = -999999;
    public bool EverSeen = false;
    public List<Vector3D> Trail = new List<Vector3D>();
}

Dictionary<string, GuardInfo> guards = new Dictionary<string, GuardInfo>();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    lcd = GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel;
    droneLcd = GridTerminalSystem.GetBlockWithName(DRONE_LCD_NAME) as IMyTextPanel;
    radarLcd = GridTerminalSystem.GetBlockWithName(RADAR_LCD_NAME) as IMyTextPanel;
    antenna = GridTerminalSystem.GetBlockWithName(ANTENNA_NAME) as IMyRadioAntenna;
    alarm = GridTerminalSystem.GetBlockWithName(ALARM_NAME) as IMySoundBlock;

    var connectorGroup = GridTerminalSystem.GetBlockGroupWithName(CONNECTOR_GROUP);
    if (connectorGroup != null)
        connectorGroup.GetBlocksOfType(connectors);

    RefreshDetectionBlocks();

    var alertGroup = GridTerminalSystem.GetBlockGroupWithName(ALERT_LIGHT_GROUP);
    if (alertGroup != null)
        alertGroup.GetBlocksOfType(alertLights);

    var baseGroup = GridTerminalSystem.GetBlockGroupWithName(BASE_LIGHT_GROUP);
    if (baseGroup != null)
        baseGroup.GetBlocksOfType(baseLights);

    statusListener = IGC.RegisterBroadcastListener(STATUS_TAG);
    statusListener.SetMessageCallback(STATUS_TAG);

    clearDelayTicks = CLEAR_DELAY_SECONDS * TICKS_PER_SECOND;
    droneTimeoutTicks = DRONE_TIMEOUT_SECONDS * TICKS_PER_SECOND;
    lastEnemyMemoryTicks = LAST_ENEMY_MEMORY_SECONDS * TICKS_PER_SECOND;

    PrepareTextSurface(droneLcd);
    PrepareScriptSurface(lcd);
    PrepareScriptSurface(radarLcd);

    UpdateLCDs();
    DrawRadarOnly();
}

void RefreshDetectionBlocks()
{
    detectionBlocks.Clear();
    var detGroup = GridTerminalSystem.GetBlockGroupWithName(DETECTION_GROUP);
    if (detGroup != null)
    {
        detGroup.GetBlocks(detectionBlocks);
    }
    
    if (detectionBlocks.Count == 0)
    {
        List<IMyLargeTurretBase> t = new List<IMyLargeTurretBase>();
        GridTerminalSystem.GetBlocksOfType(t);
        foreach(var block in t)
            detectionBlocks.Add(block);
    }
}

public void Main(string argument, UpdateType updateSource)
{
    tickCounter++;
    blockRefreshTicks++;

    // Auto-refresh detection blocks every ~5 seconds
    if (blockRefreshTicks >= 30)
    {
        blockRefreshTicks = 0;
        RefreshDetectionBlocks();
    }

    ReadDroneStatusMessages();
    UpdateDestroyedStates();

    int targetingTurrets = 0;
    int shootingTurrets = 0;
    bool threatDetected = false;
    bool enemySeenNow = false;
    Vector3D currentEnemyPos = new Vector3D();

    foreach (var block in detectionBlocks)
    {
        if (block == null || block.Closed)
            continue;

        bool isTargeting = false;
        bool isShooting = false;
        Vector3D? targetPos = null;

        TryReadDetector(block, out isTargeting, out isShooting, out targetPos);

        if (isTargeting || isShooting)
        {
            targetingTurrets++;
            if (isShooting) shootingTurrets++;
            
            if (targetPos.HasValue)
            {
                currentEnemyPos = targetPos.Value;
                enemySeenNow = true;
            }
            
            lastTargetSource = block.CustomName;
            threatDetected = true;
        }
    }

    activeDetectors = targetingTurrets;
    lastTargetingTurrets = targetingTurrets;
    lastShootingTurrets = shootingTurrets;
    lastTargetPosAvailable = enemySeenNow;

    if (activeDetectors == 0)
    {
        lastTargetSource = "NONE";
    }

    if (enemySeenNow)
    {
        lastEnemyPosition = currentEnemyPos;
        lastEnemySeenTick = tickCounter;
    }

    if (threatDetected)
    {
        noThreatTicks = 0;

        if (!alertState)
        {
            alertState = true;
            LaunchAllGuards();
        }

        if (antenna != null)
        {
            antenna.Enabled = true;
            antenna.Radius = ANTENNA_ALERT_RANGE;
        }

        if (!alarmOn && alarm != null)
        {
            alarm.ApplyAction("StopSound");
            alarm.ApplyAction("PlaySound");
            alarmOn = true;
        }

        SetAlertLights();
    }
    else
    {
        noThreatTicks++;

        if (noThreatTicks > clearDelayTicks && alertState)
        {
            alertState = false;
            ReturnAllGuards();
            lastTargetSource = "NONE";
        }

        if (!alertState)
        {
            if (antenna != null)
                antenna.Radius = ANTENNA_IDLE_RANGE;

            if (alarmOn && alarm != null)
            {
                alarm.ApplyAction("StopSound");
                alarmOn = false;
            }

            SetSafeLights();
        }
    }

    UpdateLCDs();
    DrawRadarOnly();

    Echo("MPX Brain Online");
    Echo("Detectors: " + detectionBlocks.Count);
    Echo("Active Detectors: " + activeDetectors);
    Echo("Last Target Source: " + lastTargetSource);
    Echo("Target Pos Available: " + (lastTargetPosAvailable ? "YES" : "NO"));
    Echo("Tracked Guards: " + guards.Count);
}

void TryReadDetector(IMyTerminalBlock block, out bool isTargeting, out bool isShooting, out Vector3D? targetPos)
{
    isTargeting = false;
    isShooting = false;
    targetPos = null;

    var turret = block as IMyLargeTurretBase;
    if (turret != null)
    {
        if (turret.HasTarget)
        {
            isTargeting = true;
            MyDetectedEntityInfo info = turret.GetTargetedEntity();
            if (!info.IsEmpty())
                targetPos = info.Position;
        }
        if (turret.IsShooting)
            isShooting = true;
        
        return;
    }

    try 
    {
        List<ITerminalProperty> props = new List<ITerminalProperty>();
        block.GetProperties(props);
        
        foreach (var p in props)
        {
            if (p.TypeName != "Boolean") continue;
            
            string idLower = p.Id.ToLower();
            if (idLower.Contains("target") || 
                idLower.Contains("lock") || 
                idLower.Contains("detect") || 
                idLower.Contains("radar") || 
                idLower.Contains("enemy") || 
                idLower.Contains("shoot") ||
                idLower.Contains("threat") ||
                idLower.Contains("tracking") ||
                idLower.Contains("contact") ||
                idLower.Contains("hostile"))
            {
                bool val = block.GetValueBool(p.Id);
                if (val) 
                {
                    isTargeting = true;
                    if (idLower.Contains("shoot")) isShooting = true;
                }
            }
        }
    }
    catch { }
}

void ReadDroneStatusMessages()
{
    while (statusListener.HasPendingMessage)
    {
        var msg = statusListener.AcceptMessage();
        var payload = msg.Data as string;

        if (string.IsNullOrWhiteSpace(payload))
            continue;

        var parts = payload.Split('|');
        if (parts.Length < 7)
            continue;

        string name = parts[0];
        string type = parts[1];
        string state = parts[2];

        double x = 0;
        double y = 0;
        double z = 0;
        double batt = 0;

        double.TryParse(parts[3], out x);
        double.TryParse(parts[4], out y);
        double.TryParse(parts[5], out z);
        double.TryParse(parts[6], out batt);

        GuardInfo info;
        if (!guards.TryGetValue(name, out info))
        {
            info = new GuardInfo();
            info.Name = name;
            guards[name] = info;
        }

        info.Type = type;
        info.State = state;
        info.Position = new Vector3D(x, y, z);
        info.BatteryPct = batt;
        info.LastSeenTick = tickCounter;
        info.EverSeen = true;

        info.Trail.Add(info.Position);
        if (info.Trail.Count > RADAR_TRAIL_LENGTH)
            info.Trail.RemoveAt(0);
    }
}

void UpdateDestroyedStates()
{
    foreach (var pair in guards)
    {
        var info = pair.Value;
        if (!info.EverSeen)
            continue;

        if (tickCounter - info.LastSeenTick > droneTimeoutTicks)
        {
            if (info.State != "DOCKED" && info.State != "DESTROYED")
                info.State = "DESTROYED";
        }
    }
}

void LaunchAllGuards()
{
    foreach (var c in connectors)
    {
        if (c != null && c.Status == MyShipConnectorStatus.Connected)
            c.Disconnect();
    }

    IGC.SendBroadcastMessage(CMD_TAG, "LAUNCH");
}

void ReturnAllGuards()
{
    IGC.SendBroadcastMessage(CMD_TAG, "RETURN");
}

void SetAlertLights()
{
    foreach (var l in baseLights)
        if (l != null) l.Enabled = false;

    foreach (var l in alertLights)
    {
        if (l == null) continue;
        l.Enabled = true;
        l.Color = new Color(255, 0, 0);
    }
}

void SetSafeLights()
{
    foreach (var l in alertLights)
        if (l != null) l.Enabled = false;

    foreach (var l in baseLights)
        if (l != null) l.Enabled = true;
}

int CountState(string a, string b = "", string c = "", string d = "")
{
    int count = 0;
    foreach (var pair in guards)
    {
        string s = pair.Value.State;
        if (s == a || s == b || s == c || s == d)
            count++;
    }
    return count;
}

int CountDestroyed()
{
    return CountState("DESTROYED");
}

void UpdateLCDs()
{
    UpdateDefenseDisplay();
    UpdateDroneText();
}

void UpdateDefenseDisplay()
{
    if (lcd == null)
        return;

    PrepareScriptSurface(lcd);
    var frame = lcd.DrawFrame();

    Vector2 size = lcd.TextureSize;
    Vector2 center = size * 0.5f;

    frame.Add(new MySprite(
        SpriteType.TEXTURE,
        "SquareSimple",
        center,
        size,
        new Color(8, 18, 28)
    ));

    DrawBorder(frame, center, size - new Vector2(10f, 10f), new Color(50, 220, 255));
    AddText(frame, "MPX DEFENSE NETWORK", new Vector2(center.X, 28f), 1.05f, Color.Cyan, TextAlignment.CENTER);

    int dockedCount = CountState("DOCKED");
    int airborneCount = CountState("LAUNCHED", "AIRBORNE");
    int returningCount = CountState("RETURNING");
    int damagedCount = CountState("DAMAGED");
    int destroyedCount = CountDestroyed();

    string threat = alertState ? "DETECTED" : "NONE";
    string system = alertState ? "ALERT" : "STABLE";

    Color threatColor = alertState ? new Color(255, 80, 80) : Color.White;
    Color systemColor = alertState ? new Color(255, 150, 100) : new Color(180, 255, 180);

    float leftX = 28f;
    float valueX = size.X - 32f;
    float y = 60f;
    float step = 28f;

    DrawDisplayLine(frame, leftX, valueX, y, "THREAT", threat, threatColor); y += step;
    DrawDisplayLine(frame, leftX, valueX, y, "SYSTEM", system, systemColor); y += step + 6f;
    
    DrawDisplayLine(frame, leftX, valueX, y, "DETECTORS", detectionBlocks.Count.ToString(), new Color(200, 200, 200)); y += step;
    DrawDisplayLine(frame, leftX, valueX, y, "ACTIVE", activeDetectors.ToString(), new Color(255, 140, 140)); y += step;
    DrawDisplayLine(frame, leftX, valueX, y, "SHOOTING", lastShootingTurrets.ToString(), new Color(255, 190, 140)); y += step;
    
    string dispSource = lastTargetSource ?? "NONE";
    if (dispSource.Length > 15) dispSource = dispSource.Substring(0, 15);
    DrawDisplayLine(frame, leftX, valueX, y, "SOURCE", dispSource, new Color(255, 255, 100)); y += step + 6f;

    DrawDisplayLine(frame, leftX, valueX, y, "DOCKED", dockedCount.ToString(), new Color(0, 255, 120)); y += step;
    DrawDisplayLine(frame, leftX, valueX, y, "AIRBORNE", airborneCount.ToString(), new Color(255, 80, 80)); y += step;
    DrawDisplayLine(frame, leftX, valueX, y, "RETURNING", returningCount.ToString(), new Color(255, 220, 0)); y += step;
    DrawDisplayLine(frame, leftX, valueX, y, "DAMAGED", damagedCount.ToString(), new Color(80, 180, 255)); y += step;
    DrawDisplayLine(frame, leftX, valueX, y, "DESTROYED", destroyedCount.ToString(), new Color(150, 150, 150));

    frame.Dispose();
}

void UpdateDroneText()
{
    if (droneLcd == null)
        return;

    PrepareTextSurface(droneLcd);
    droneLcd.WriteText("MPX DRONE TRACKING\n\n", false);

    Vector3D basePos = Me.GetPosition();

    foreach (var pair in guards)
    {
        var info = pair.Value;

        double dist = info.EverSeen ? Vector3D.Distance(basePos, info.Position) : 0;
        string battText = (info.BatteryPct * 100.0).ToString("0") + "%";
        string distText = info.EverSeen ? dist.ToString("0.0") + "m" : "-";

        droneLcd.WriteText(
            info.Name + " | " + info.Type + "\n" +
            "State: " + info.State + "\n" +
            "Battery: " + battText + " | Dist: " + distText + "\n\n",
            true
        );
    }
}

void DrawRadarOnly()
{
    if (radarLcd == null)
        return;

    PrepareScriptSurface(radarLcd);
    var frame = radarLcd.DrawFrame();

    Vector2 size = radarLcd.TextureSize;
    Vector2 center = size * 0.5f;
    float radius = Math.Min(size.X, size.Y) * 0.36f;

    frame.Add(new MySprite(
        SpriteType.TEXTURE,
        "SquareSimple",
        center,
        size,
        new Color(5, 14, 20)
    ));

    DrawBorder(frame, center, size - new Vector2(10f, 10f), new Color(40, 180, 220));
    AddText(frame, "MPX RADAR", new Vector2(center.X, 26f), 1.00f, Color.Cyan, TextAlignment.CENTER);

    frame.Add(new MySprite(
        SpriteType.TEXTURE,
        "Circle",
        center,
        new Vector2(radius * 2.1f, radius * 2.1f),
        new Color(0, 40, 30, 40)
    ));

    DrawRadarRing(frame, center, radius * 2.00f, new Color(0, 255, 120));
    DrawRadarRing(frame, center, radius * 1.50f, new Color(0, 180, 80));
    DrawRadarRing(frame, center, radius * 1.00f, new Color(0, 140, 60));
    DrawRadarRing(frame, center, radius * 0.50f, new Color(0, 100, 50));

    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", center, new Vector2(radius * 2f, 2f), new Color(0, 160, 80)));
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", center, new Vector2(2f, radius * 2f), new Color(0, 160, 80)));

    DrawLine(frame,
        center + new Vector2(-radius * 0.7f, -radius * 0.7f),
        center + new Vector2(radius * 0.7f, radius * 0.7f),
        new Color(0, 90, 50), 1.2f);

    DrawLine(frame,
        center + new Vector2(radius * 0.7f, -radius * 0.7f),
        center + new Vector2(-radius * 0.7f, radius * 0.7f),
        new Color(0, 90, 50), 1.2f);

    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", center, new Vector2(10f, 10f), Color.White));
    frame.Add(new MySprite(SpriteType.TEXTURE, "Triangle", center + new Vector2(0f, -10f), new Vector2(10f, 10f), new Color(100, 220, 255)));

    Vector3D basePos = Me.GetPosition();
    MatrixD world = Me.WorldMatrix;
    Vector3D forward = world.Forward;
    Vector3D right = world.Right;

    foreach (var pair in guards)
    {
        var g = pair.Value;
        if (!g.EverSeen)
            continue;

        for (int i = 0; i < g.Trail.Count; i++)
        {
            Vector3D relTrail = g.Trail[i] - basePos;
            Vector2 trailPos = RelativeToRadar(relTrail, center, radius, RADAR_RANGE_METERS, forward, right);

            int alpha = 20 + (i * 8);
            if (alpha > 150) alpha = 150;

            Color trailColor = GetGuardColor(g.State);
            trailColor.A = (byte)alpha;

            float dotSize = 3f + ((float)i / Math.Max(1f, (float)g.Trail.Count)) * 2.5f;

            frame.Add(new MySprite(
                SpriteType.TEXTURE,
                "Circle",
                trailPos,
                new Vector2(dotSize, dotSize),
                trailColor
            ));
        }

        Vector3D relNow = g.Position - basePos;
        Vector2 dronePos = RelativeToRadar(relNow, center, radius, RADAR_RANGE_METERS, forward, right);
        Color droneColor = GetGuardColor(g.State);

        frame.Add(new MySprite(
            SpriteType.TEXTURE,
            "Circle",
            dronePos,
            new Vector2(9f, 9f),
            droneColor
        ));

        string shortName = g.Name.Replace("Guard ", "G");
        AddText(frame, shortName, dronePos + new Vector2(10f, -2f), 0.55f, droneColor, TextAlignment.LEFT);
    }

    bool drawEnemy = false;
    Vector3D enemyPos = new Vector3D();

    if (lastEnemyPosition.HasValue)
    {
        if (KEEP_LAST_ENEMY_BLIP)
        {
            if (tickCounter - lastEnemySeenTick <= lastEnemyMemoryTicks)
            {
                drawEnemy = true;
                enemyPos = lastEnemyPosition.Value;
            }
        }
        else if (alertState)
        {
            drawEnemy = true;
            enemyPos = lastEnemyPosition.Value;
        }
    }

    if (drawEnemy)
    {
        Vector3D relEnemy = enemyPos - basePos;
        Vector2 enemyRadarPos = RelativeToRadar(relEnemy, center, radius, RADAR_RANGE_METERS, forward, right);

        frame.Add(new MySprite(
            SpriteType.TEXTURE,
            "Triangle",
            enemyRadarPos,
            new Vector2(15f, 15f),
            new Color(255, 70, 70)
        ));

        AddText(frame, "ENEMY", enemyRadarPos + new Vector2(10f, -2f), 0.60f, new Color(255, 120, 120), TextAlignment.LEFT);
    }

    AddText(frame, "RANGE " + RADAR_RANGE_METERS.ToString("0") + "m",
        new Vector2(center.X, size.Y - 24f),
        0.60f, new Color(120, 220, 255), TextAlignment.CENTER);

    frame.Dispose();
}

void PrepareTextSurface(IMyTextSurface s)
{
    if (s == null) return;
    s.ContentType = ContentType.TEXT_AND_IMAGE;
}

void PrepareScriptSurface(IMyTextSurface s)
{
    if (s == null) return;
    s.ContentType = ContentType.SCRIPT;
    s.ScriptBackgroundColor = Color.Black;
    s.ScriptForegroundColor = Color.White;
}

void DrawBorder(MySpriteDrawFrame frame, Vector2 pos, Vector2 size, Color border)
{
    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(pos.X, pos.Y - size.Y * 0.5f),
        new Vector2(size.X, 3f), border));

    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(pos.X, pos.Y + size.Y * 0.5f),
        new Vector2(size.X, 3f), border));

    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(pos.X - size.X * 0.5f, pos.Y),
        new Vector2(3f, size.Y), border));

    frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple",
        new Vector2(pos.X + size.X * 0.5f, pos.Y),
        new Vector2(3f, size.Y), border));
}

void DrawDisplayLine(MySpriteDrawFrame frame, float xLeft, float xRight, float y, string label, string value, Color valueColor)
{
    AddText(frame, label + ":", new Vector2(xLeft, y), 0.80f, Color.White, TextAlignment.LEFT);
    AddText(frame, value, new Vector2(xRight, y), 0.80f, valueColor, TextAlignment.RIGHT);
}

void DrawRadarRing(MySpriteDrawFrame frame, Vector2 center, float size, Color color)
{
    frame.Add(new MySprite(
        SpriteType.TEXTURE,
        "CircleHollow",
        center,
        new Vector2(size, size),
        color
    ));
}

void AddText(MySpriteDrawFrame frame, string text, Vector2 pos, float scale, Color color, TextAlignment align)
{
    var s = MySprite.CreateText(text, "Debug", color, scale, align);
    s.Position = pos;
    frame.Add(s);
}

void DrawLine(MySpriteDrawFrame frame, Vector2 from, Vector2 to, Color color, float thickness)
{
    Vector2 diff = to - from;
    float length = diff.Length();
    float rotation = (float)Math.Atan2(diff.Y, diff.X);

    var line = new MySprite(
        SpriteType.TEXTURE,
        "SquareSimple",
        (from + to) * 0.5f,
        new Vector2(length, thickness),
        color
    );
    line.RotationOrScale = rotation;
    frame.Add(line);
}

Vector2 RelativeToRadar(Vector3D relative, Vector2 center, float radius, double maxRange, Vector3D forward, Vector3D right)
{
    double rx = RADAR_USE_BASE_ORIENTATION ? Vector3D.Dot(relative, right) : relative.X;
    double rz = RADAR_USE_BASE_ORIENTATION ? Vector3D.Dot(relative, forward) : relative.Z;

    double dist = Math.Sqrt(rx * rx + rz * rz);
    if (dist > maxRange && dist > 0)
    {
        double scale = maxRange / dist;
        rx *= scale;
        rz *= scale;
    }

    float px = center.X + (float)(rx / maxRange) * radius;
    float py = center.Y - (float)(rz / maxRange) * radius;

    return new Vector2(px, py);
}

Color GetGuardColor(string state)
{
    if (state == "DOCKED") return new Color(0, 255, 120);
    if (state == "RETURNING") return new Color(255, 220, 0);
    if (state == "DAMAGED") return new Color(80, 180, 255);
    if (state == "DESTROYED") return new Color(140, 140, 140);
    if (state == "LAUNCHED" || state == "AIRBORNE") return new Color(255, 80, 80);
    return Color.White;
}