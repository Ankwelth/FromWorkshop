// ============================================================
// [RADAR] ROTATING SENSOR RADAR V3
// Space Engineers 1 – Programmable Block
//
// Názvy bloků:
// [RADAR] Rotor
// [RADAR] Sensor
// [RADAR] Cockpit
// [RADAR] Display
// [RADAR] Contacts
//
// Funkce:
// - plynulá matematická radarová ručička
// - 1 oběh za 4,8 sekundy
// - kontakt svítí 2 sekundy a 3 sekundy mizí
// - chronologický seznam kontaktů
// - větší písmo
// - ignorování celé vlastní mechanické konstrukce
// ============================================================


// ============================================================
// NASTAVENÍ BLOKŮ
// ============================================================

const string ROTOR_NAME = "[RADAR] Rotor";
const string SENSOR_NAME = "[RADAR] Sensor";
const string COCKPIT_NAME = "[RADAR] Cockpit";
const string RADAR_LCD_NAME = "[RADAR] Display";
const string CONTACT_LCD_NAME = "[RADAR] Contacts";


// ============================================================
// NASTAVENÍ RADARU
// ============================================================

const double RADAR_RANGE = 5000.0;

// 4,8 sekundy na oběh = 12,5 RPM.
const double ROTATION_TIME = 4.8;

// 1 = ve směru hodinových ručiček.
// -1 = proti směru hodinových ručiček.
const float ROTOR_DIRECTION = 1f;

// Plný jas kontaktu.
const double BLIP_FULL_TIME = 2.0;

// Doba postupného zmizení.
const double BLIP_FADE_TIME = 3.0;

const double BLIP_TOTAL_TIME =
    BLIP_FULL_TIME + BLIP_FADE_TIME;

// Počet kružnic.
const int RANGE_RING_COUNT = 5;

// Maximální počet uložených kontaktů.
const int MAX_CONTACT_HISTORY = 30;

// Kolik kontaktů maximálně zobrazit na textovém LCD.
const int MAX_VISIBLE_CONTACTS = 7;

// Počet segmentů kružnic.
// Nižší hodnota výrazně snižuje zatížení PB.
const int CIRCLE_SEGMENTS = 40;

// Počet čar světelné stopy.
const int SWEEP_TRAIL_COUNT = 10;

// Velikost stopy v radiánech.
const double SWEEP_TRAIL_SPACING = 0.020;


// ============================================================
// FREKVENCE
// ============================================================

// Senzor se nemusí číst každý tick.
const int SENSOR_UPDATE_TICKS = 10;

// Seznam kontaktů se nemusí překreslovat každý tick.
const int CONTACT_LCD_UPDATE_TICKS = 10;

// Mechanickou skupinu překontrolovat přibližně jednou za 5 sekund.
const int LOCAL_GRID_UPDATE_TICKS = 300;


// ============================================================
// BLOKY
// ============================================================

IMyMotorStator rotor;
IMySensorBlock sensor;
IMyShipController reference;

IMyTextSurface radarSurface;
IMyTextSurface contactsSurface;


// ============================================================
// DATA
// ============================================================

readonly List<MyDetectedEntityInfo> detectedEntities =
    new List<MyDetectedEntityInfo>();

readonly Dictionary<long, RadarContact> contacts =
    new Dictionary<long, RadarContact>();

readonly List<RadarContact> sortedContacts =
    new List<RadarContact>();

readonly List<IMyMechanicalConnectionBlock> mechanicalBlocks =
    new List<IMyMechanicalConnectionBlock>();

// EntityId všech gridů tvořících vlastní mechanickou konstrukci.
readonly HashSet<long> localGridIds =
    new HashSet<long>();


double scriptTime = 0;
double visualSweepAngle = 0;

bool visualSweepInitialized = false;
bool radarRunning = true;

int tickCounter = 0;

string errorMessage = "";


// ============================================================
// PROGRAM
// ============================================================

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

    if (!FindBlocks())
        return;

    ConfigureSurfaces();
    ConfigureRotor();

    RebuildLocalGridList();

    visualSweepAngle = GetRealSensorAngle();
    visualSweepInitialized = true;
}


public void Save()
{
}


// ============================================================
// HLAVNÍ SMYČKA
// ============================================================

public void Main(string argument, UpdateType updateSource)
{
    double deltaTime =
        Runtime.TimeSinceLastRun.TotalSeconds;

    // Po lagu nebo načtení světa nedovolíme velký skok čáry.
    if (deltaTime <= 0 || deltaTime > 0.15)
        deltaTime = 1.0 / 60.0;

    scriptTime += deltaTime;
    tickCounter++;


    if (!string.IsNullOrWhiteSpace(argument))
        ProcessCommand(argument);


    if (!BlocksValid())
    {
        if (!FindBlocks())
        {
            DrawError();
            return;
        }

        ConfigureSurfaces();
        ConfigureRotor();
        RebuildLocalGridList();

        visualSweepAngle = GetRealSensorAngle();
        visualSweepInitialized = true;
    }


    MaintainRotorSpeed();

    // Čára se počítá každý tick.
    UpdateVisualSweep(deltaTime);

    // Vlastní subgridy se nemusí hledat pořád.
    if (tickCounter % LOCAL_GRID_UPDATE_TICKS == 0)
        RebuildLocalGridList();

    // Senzor čteme pouze 6× za sekundu.
    if (tickCounter % SENSOR_UPDATE_TICKS == 0)
        ScanSensor();

    // Kruhový radar se vykresluje každý tick.
    DrawRadar();

    // Textový displej jen 6× za sekundu.
    if (tickCounter % CONTACT_LCD_UPDATE_TICKS == 0)
        DrawContactList();

    EchoStatus();
}


// ============================================================
// HLEDÁNÍ BLOKŮ
// ============================================================

bool FindBlocks()
{
    errorMessage = "";

    rotor =
        GridTerminalSystem.GetBlockWithName(ROTOR_NAME)
        as IMyMotorStator;

    sensor =
        GridTerminalSystem.GetBlockWithName(SENSOR_NAME)
        as IMySensorBlock;

    reference =
        GridTerminalSystem.GetBlockWithName(COCKPIT_NAME)
        as IMyShipController;


    IMyTextSurfaceProvider radarProvider =
        GridTerminalSystem.GetBlockWithName(RADAR_LCD_NAME)
        as IMyTextSurfaceProvider;

    IMyTextSurfaceProvider contactsProvider =
        GridTerminalSystem.GetBlockWithName(CONTACT_LCD_NAME)
        as IMyTextSurfaceProvider;


    if (rotor == null)
        errorMessage += "Nenalezen: " + ROTOR_NAME + "\n";

    if (sensor == null)
        errorMessage += "Nenalezen: " + SENSOR_NAME + "\n";

    if (reference == null)
        errorMessage += "Nenalezen: " + COCKPIT_NAME + "\n";


    if (radarProvider == null)
    {
        errorMessage +=
            "Nenalezen: " + RADAR_LCD_NAME + "\n";
    }
    else
    {
        radarSurface = radarProvider.GetSurface(0);
    }


    if (contactsProvider == null)
    {
        errorMessage +=
            "Nenalezen: " + CONTACT_LCD_NAME + "\n";
    }
    else
    {
        contactsSurface = contactsProvider.GetSurface(0);
    }


    return string.IsNullOrEmpty(errorMessage);
}


bool BlocksValid()
{
    return rotor != null
        && sensor != null
        && reference != null
        && radarSurface != null
        && contactsSurface != null;
}


// ============================================================
// LCD
// ============================================================

void ConfigureSurfaces()
{
    ConfigureSurface(radarSurface);
    ConfigureSurface(contactsSurface);
}


void ConfigureSurface(IMyTextSurface surface)
{
    if (surface == null)
        return;

    surface.ContentType = ContentType.SCRIPT;
    surface.Script = "";

    surface.ScriptBackgroundColor = Color.Black;
    surface.ScriptForegroundColor =
        new Color(0, 255, 70);
}


// ============================================================
// ROTOR
// ============================================================

void ConfigureRotor()
{
    if (rotor == null)
        return;

    rotor.Enabled = true;
    rotor.RotorLock = false;

    rotor.LowerLimitDeg = float.MinValue;
    rotor.UpperLimitDeg = float.MaxValue;

    MaintainRotorSpeed();
}


void MaintainRotorSpeed()
{
    if (rotor == null)
        return;

    if (!radarRunning)
    {
        rotor.TargetVelocityRPM = 0;
        return;
    }

    float requiredRpm =
        (float)(60.0 / ROTATION_TIME)
        * ROTOR_DIRECTION;

    rotor.TargetVelocityRPM = requiredRpm;
}


// ============================================================
// PLYNULÁ RADAROVÁ ČÁRA
// ============================================================

void UpdateVisualSweep(double deltaTime)
{
    if (!visualSweepInitialized)
    {
        visualSweepAngle = GetRealSensorAngle();
        visualSweepInitialized = true;
        return;
    }

    if (!radarRunning)
        return;

    // Čára nepřebírá každý tick polohu rotoru.
    // Díky tomu ji nesráží skokové síťové aktualizace subgridu.
    double radiansPerSecond =
        Math.PI * 2.0 / ROTATION_TIME;

    visualSweepAngle +=
        radiansPerSecond
        * deltaTime
        * ROTOR_DIRECTION;

    visualSweepAngle =
        NormalizeAngle(visualSweepAngle);
}


double GetRealSensorAngle()
{
    if (sensor == null || reference == null)
        return 0;

    Vector3D direction =
        sensor.WorldMatrix.Forward;

    double forward =
        Vector3D.Dot(
            direction,
            reference.WorldMatrix.Forward
        );

    double right =
        Vector3D.Dot(
            direction,
            reference.WorldMatrix.Right
        );

    return NormalizeAngle(
        Math.Atan2(right, forward)
    );
}


double NormalizeAngle(double angle)
{
    double fullCircle = Math.PI * 2.0;

    angle %= fullCircle;

    if (angle < 0)
        angle += fullCircle;

    return angle;
}


// ============================================================
// VLASTNÍ GRIDY A SUBGRIDY
// ============================================================

void RebuildLocalGridList()
{
    localGridIds.Clear();

    // Začneme gridem, na kterém je programmable block.
    localGridIds.Add(Me.CubeGrid.EntityId);

    mechanicalBlocks.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyMechanicalConnectionBlock>(
        mechanicalBlocks
    );

    // Opakovaně procházíme mechanické bloky,
    // dokud nenajdeme všechny navázané gridy.
    bool foundNewGrid = true;
    int safety = 0;

    while (foundNewGrid && safety < 100)
    {
        foundNewGrid = false;
        safety++;

        for (int i = 0; i < mechanicalBlocks.Count; i++)
        {
            IMyMechanicalConnectionBlock mechanical =
                mechanicalBlocks[i];

            if (mechanical == null)
                continue;

            long baseGridId =
                mechanical.CubeGrid.EntityId;

            IMyCubeGrid topGrid =
                mechanical.TopGrid;

            if (topGrid == null)
                continue;

            long topGridId =
                topGrid.EntityId;

            bool baseIsLocal =
                localGridIds.Contains(baseGridId);

            bool topIsLocal =
                localGridIds.Contains(topGridId);


            if (baseIsLocal && !topIsLocal)
            {
                localGridIds.Add(topGridId);
                foundNewGrid = true;
            }
            else if (topIsLocal && !baseIsLocal)
            {
                localGridIds.Add(baseGridId);
                foundNewGrid = true;
            }
        }
    }
}


bool IsOwnMechanicalGrid(long entityId)
{
    return localGridIds.Contains(entityId);
}


// ============================================================
// SKENOVÁNÍ
// ============================================================

void ScanSensor()
{
    detectedEntities.Clear();
    sensor.DetectedEntities(detectedEntities);

    Vector3D radarOrigin =
        reference.GetPosition();


    for (int i = 0; i < detectedEntities.Count; i++)
    {
        MyDetectedEntityInfo info =
            detectedEntities[i];

        if (info.IsEmpty())
            continue;

        // Ignorování hlavního gridu i všech vlastních subgridů.
        if (IsOwnMechanicalGrid(info.EntityId))
            continue;

        Vector3D position = info.Position;

        double distance =
            Vector3D.Distance(
                radarOrigin,
                position
            );

        if (distance > RADAR_RANGE)
            continue;


        RadarContact contact;

        if (!contacts.TryGetValue(
            info.EntityId,
            out contact))
        {
            contact = new RadarContact();
            contact.EntityId = info.EntityId;

            contacts.Add(
                info.EntityId,
                contact
            );
        }


        contact.Name = GetContactName(info);
        contact.Position = position;
        contact.Distance = distance;

        contact.Relationship =
            info.Relationship;

        contact.Type =
            info.Type;

        contact.LastDetectedTime =
            scriptTime;

        contact.DetectionCount++;

        CalculateClockAndBearing(contact);

        contacts[info.EntityId] = contact;
    }

    // Kdyby byl vlastní subgrid zachycen dříve,
    // odstraníme jej i z uložené historie.
    RemoveOwnGridsFromHistory();

    TrimHistory();
}


void RemoveOwnGridsFromHistory()
{
    sortedContacts.Clear();

    foreach (RadarContact contact in contacts.Values)
    {
        if (IsOwnMechanicalGrid(contact.EntityId))
            sortedContacts.Add(contact);
    }

    for (int i = 0; i < sortedContacts.Count; i++)
    {
        contacts.Remove(
            sortedContacts[i].EntityId
        );
    }
}


// ============================================================
// POLOHA KONTAKTU
// ============================================================

void CalculateClockAndBearing(
    RadarContact contact)
{
    Vector3D offset =
        contact.Position
        - reference.GetPosition();

    double forward =
        Vector3D.Dot(
            offset,
            reference.WorldMatrix.Forward
        );

    double right =
        Vector3D.Dot(
            offset,
            reference.WorldMatrix.Right
        );

    double bearing =
        Math.Atan2(right, forward);

    if (bearing < 0)
        bearing += Math.PI * 2.0;

    contact.BearingRadians = bearing;

    contact.BearingDegrees =
        bearing * 180.0 / Math.PI;

    contact.ClockPosition =
        BearingToClock(bearing);
}


int BearingToClock(double bearing)
{
    double sector =
        Math.PI * 2.0 / 12.0;

    int hour =
        (int)Math.Round(
            bearing / sector
        );

    hour %= 12;

    return hour == 0 ? 12 : hour;
}


// ============================================================
// HISTORIE
// ============================================================

void TrimHistory()
{
    if (contacts.Count <= MAX_CONTACT_HISTORY)
        return;

    sortedContacts.Clear();

    foreach (RadarContact contact in contacts.Values)
        sortedContacts.Add(contact);

    sortedContacts.Sort(
        delegate(RadarContact a, RadarContact b)
        {
            return a.LastDetectedTime.CompareTo(
                b.LastDetectedTime
            );
        }
    );

    while (
        contacts.Count > MAX_CONTACT_HISTORY
        && sortedContacts.Count > 0)
    {
        contacts.Remove(
            sortedContacts[0].EntityId
        );

        sortedContacts.RemoveAt(0);
    }
}


// ============================================================
// KRUHOVÝ RADAR
// ============================================================

void DrawRadar()
{
    Vector2 surfaceSize =
        radarSurface.SurfaceSize;

    Vector2 textureSize =
        radarSurface.TextureSize;

    Vector2 viewport =
        (textureSize - surfaceSize) * 0.5f;

    Vector2 centre =
        viewport + surfaceSize * 0.5f;

    float radarDiameter =
        Math.Min(
            surfaceSize.X,
            surfaceSize.Y
        ) * 0.82f;

    float radarRadius =
        radarDiameter * 0.5f;


    using (
        MySpriteDrawFrame frame =
            radarSurface.DrawFrame())
    {
        DrawRadarBackground(
            frame,
            centre,
            radarRadius
        );

        DrawRangeRings(
            frame,
            centre,
            radarRadius
        );

        DrawAxes(
            frame,
            centre,
            radarRadius
        );

        DrawBlips(
            frame,
            centre,
            radarRadius
        );

        DrawSweepLine(
            frame,
            centre,
            radarRadius
        );

        DrawRadarLabels(
            frame,
            centre,
            radarRadius
        );
    }
}


void DrawRadarBackground(
    MySpriteDrawFrame frame,
    Vector2 centre,
    float radius)
{
    MySprite background =
        MySprite.CreateSprite(
            "Circle",
            centre,
            new Vector2(
                radius * 2f,
                radius * 2f
            )
        );

    background.Color =
        new Color(0, 14, 4);

    frame.Add(background);

    AddCircle(
        frame,
        centre,
        radius,
        new Color(0, 180, 45),
        CIRCLE_SEGMENTS,
        3f
    );
}


void DrawRangeRings(
    MySpriteDrawFrame frame,
    Vector2 centre,
    float radarRadius)
{
    Color ringColor =
        new Color(0, 72, 20);

    for (int i = 1; i <= RANGE_RING_COUNT; i++)
    {
        float radius =
            radarRadius
            * i
            / RANGE_RING_COUNT;

        AddCircle(
            frame,
            centre,
            radius,
            ringColor,
            CIRCLE_SEGMENTS,
            i == RANGE_RING_COUNT
                ? 2f
                : 1f
        );
    }
}


void DrawAxes(
    MySpriteDrawFrame frame,
    Vector2 centre,
    float radius)
{
    Color axisColor =
        new Color(0, 52, 15);

    AddLine(
        frame,
        centre + new Vector2(-radius, 0),
        centre + new Vector2(radius, 0),
        1f,
        axisColor
    );

    AddLine(
        frame,
        centre + new Vector2(0, -radius),
        centre + new Vector2(0, radius),
        1f,
        axisColor
    );
}


// ============================================================
// RADAROVÁ ČÁRA
// ============================================================

void DrawSweepLine(
    MySpriteDrawFrame frame,
    Vector2 centre,
    float radarRadius)
{
    double angle = visualSweepAngle;

    // Stopu vykreslujeme nejdříve,
    // aby hlavní čára zůstala navrchu.
    for (int i = SWEEP_TRAIL_COUNT; i >= 1; i--)
    {
        double trailAngle =
            angle
            - ROTOR_DIRECTION
            * i
            * SWEEP_TRAIL_SPACING;

        Vector2 trailDirection =
            new Vector2(
                (float)Math.Sin(trailAngle),
                (float)-Math.Cos(trailAngle)
            );

        float intensity =
            1f
            - (float)i
            / (SWEEP_TRAIL_COUNT + 1);

        byte green =
            (byte)(120 * intensity);

        float width =
            0.8f + intensity * 1.2f;

        AddLine(
            frame,
            centre,
            centre
                + trailDirection
                * radarRadius,
            width,
            new Color(
                0,
                green,
                (byte)(green / 4)
            )
        );
    }


    Vector2 direction =
        new Vector2(
            (float)Math.Sin(angle),
            (float)-Math.Cos(angle)
        );

    Vector2 end =
        centre + direction * radarRadius;

    // Jemná záře pod čárou.
    AddLine(
        frame,
        centre,
        end,
        6f,
        new Color(0, 70, 20)
    );

    // Hlavní čára navrch.
    AddLine(
        frame,
        centre,
        end,
        2.5f,
        new Color(25, 255, 75)
    );

    MySprite centrePoint =
        MySprite.CreateSprite(
            "Circle",
            centre,
            new Vector2(9f, 9f)
        );

    centrePoint.Color =
        new Color(0, 255, 70);

    frame.Add(centrePoint);
}


// ============================================================
// TEČKY KONTAKTŮ
// ============================================================

void DrawBlips(
    MySpriteDrawFrame frame,
    Vector2 centre,
    float radarRadius)
{
    foreach (RadarContact contact in contacts.Values)
    {
        double age =
            scriptTime
            - contact.LastDetectedTime;

        if (age > BLIP_TOTAL_TIME)
            continue;

        float intensity =
            GetBlipIntensity(age);

        float relativeDistance =
            (float)(
                contact.Distance
                / RADAR_RANGE
            );

        relativeDistance =
            MathHelper.Clamp(
                relativeDistance,
                0f,
                1f
            );

        float x =
            (float)Math.Sin(
                contact.BearingRadians
            )
            * radarRadius
            * relativeDistance;

        float y =
            (float)-Math.Cos(
                contact.BearingRadians
            )
            * radarRadius
            * relativeDistance;

        Vector2 position =
            centre + new Vector2(x, y);

        Color color =
            GetRelationshipColor(
                contact.Relationship,
                intensity
            );

        float size =
            9f + intensity * 4f;

        MySprite blip =
            MySprite.CreateSprite(
                "Circle",
                position,
                new Vector2(size, size)
            );

        blip.Color = color;
        frame.Add(blip);
    }
}


float GetBlipIntensity(double age)
{
    if (age <= BLIP_FULL_TIME)
        return 1f;

    double fadeAge =
        age - BLIP_FULL_TIME;

    double value =
        1.0
        - fadeAge / BLIP_FADE_TIME;

    return MathHelper.Clamp(
        (float)value,
        0f,
        1f
    );
}


// ============================================================
// POPISKY
// ============================================================

void DrawRadarLabels(
    MySpriteDrawFrame frame,
    Vector2 centre,
    float radarRadius)
{
    Color color =
        new Color(0, 220, 55);

    AddText(
        frame,
        "12",
        centre + new Vector2(
            0,
            -radarRadius - 19
        ),
        0.55f,
        color,
        TextAlignment.CENTER
    );

    AddText(
        frame,
        "3",
        centre + new Vector2(
            radarRadius + 14,
            -8
        ),
        0.5f,
        color,
        TextAlignment.CENTER
    );

    AddText(
        frame,
        "6",
        centre + new Vector2(
            0,
            radarRadius + 7
        ),
        0.5f,
        color,
        TextAlignment.CENTER
    );

    AddText(
        frame,
        "9",
        centre + new Vector2(
            -radarRadius - 14,
            -8
        ),
        0.5f,
        color,
        TextAlignment.CENTER
    );

    AddText(
        frame,
        "RANGE 5 KM",
        centre + new Vector2(
            0,
            radarRadius + 28
        ),
        0.42f,
        color,
        TextAlignment.CENTER
    );
}


// ============================================================
// SEZNAM KONTAKTŮ
// ============================================================

void DrawContactList()
{
    Vector2 surfaceSize =
        contactsSurface.SurfaceSize;

    Vector2 textureSize =
        contactsSurface.TextureSize;

    Vector2 viewport =
        (textureSize - surfaceSize) * 0.5f;


    using (
        MySpriteDrawFrame frame =
            contactsSurface.DrawFrame())
    {
        AddText(
            frame,
            "RADAR CONTACTS",
            viewport + new Vector2(18, 12),
            0.85f,
            new Color(0, 255, 65),
            TextAlignment.LEFT
        );

        AddLine(
            frame,
            viewport + new Vector2(15, 57),
            viewport + new Vector2(
                surfaceSize.X - 15,
                57
            ),
            2f,
            new Color(0, 100, 28)
        );


        sortedContacts.Clear();

        foreach (RadarContact contact in contacts.Values)
            sortedContacts.Add(contact);

        sortedContacts.Sort(
            delegate(RadarContact a, RadarContact b)
            {
                return b.LastDetectedTime.CompareTo(
                    a.LastDetectedTime
                );
            }
        );


        float y = 75f;

        // Větší rozestup odpovídá většímu písmu.
        float lineHeight = 48f;

        int availableLines =
            (int)(
                (surfaceSize.Y - y - 10)
                / lineHeight
            );

        int shown =
            Math.Min(
                MAX_VISIBLE_CONTACTS,
                Math.Min(
                    availableLines,
                    sortedContacts.Count
                )
            );


        for (int i = 0; i < shown; i++)
        {
            RadarContact contact =
                sortedContacts[i];

            double age =
                scriptTime
                - contact.LastDetectedTime;

            bool active =
                age <= BLIP_TOTAL_TIME;

            string state =
                active ? ">" : "-";

            string line =
                state
                + " "
                + contact.ClockPosition
                    .ToString()
                    .PadLeft(2, ' ')
                + "H "
                + RelationshipToText(
                    contact.Relationship
                )
                + " "
                + FormatDistance(
                    contact.Distance
                );

            Color color =
                active
                ? GetRelationshipColor(
                    contact.Relationship,
                    1f
                )
                : new Color(75, 110, 80);

            AddText(
                frame,
                line,
                viewport + new Vector2(18, y),
                0.72f,
                color,
                TextAlignment.LEFT
            );

            y += lineHeight;
        }


        if (sortedContacts.Count == 0)
        {
            AddText(
                frame,
                "NO CONTACTS",
                viewport
                    + surfaceSize * 0.5f,
                0.8f,
                new Color(0, 100, 28),
                TextAlignment.CENTER
            );
        }
    }
}


// ============================================================
// GRAFICKÉ FUNKCE
// ============================================================

void AddCircle(
    MySpriteDrawFrame frame,
    Vector2 centre,
    float radius,
    Color color,
    int segments,
    float thickness)
{
    Vector2 previous =
        centre + new Vector2(0, -radius);

    for (int i = 1; i <= segments; i++)
    {
        double angle =
            Math.PI * 2.0 * i / segments;

        Vector2 current =
            centre + new Vector2(
                (float)Math.Sin(angle) * radius,
                (float)-Math.Cos(angle) * radius
            );

        AddLine(
            frame,
            previous,
            current,
            thickness,
            color
        );

        previous = current;
    }
}


void AddLine(
    MySpriteDrawFrame frame,
    Vector2 start,
    Vector2 end,
    float width,
    Color color)
{
    Vector2 difference =
        end - start;

    float length =
        difference.Length();

    if (length <= 0.001f)
        return;

    float rotation =
        (float)Math.Atan2(
            difference.Y,
            difference.X
        );

    MySprite line =
        MySprite.CreateSprite(
            "SquareSimple",
            (start + end) * 0.5f,
            new Vector2(length, width)
        );

    line.RotationOrScale = rotation;
    line.Color = color;

    frame.Add(line);
}


void AddText(
    MySpriteDrawFrame frame,
    string text,
    Vector2 position,
    float scale,
    Color color,
    TextAlignment alignment)
{
    MySprite sprite =
        MySprite.CreateText(
            text,
            "Monospace",
            color,
            scale,
            alignment
        );

    sprite.Position = position;
    frame.Add(sprite);
}


// ============================================================
// BARVY A TEXTY
// ============================================================

Color GetRelationshipColor(
    MyRelationsBetweenPlayerAndBlock relationship,
    float intensity)
{
    intensity =
        MathHelper.Clamp(
            intensity,
            0f,
            1f
        );

    Color baseColor;

    switch (relationship)
    {
        case MyRelationsBetweenPlayerAndBlock.Enemies:
            baseColor = new Color(255, 45, 25);
            break;

        case MyRelationsBetweenPlayerAndBlock.Friends:
        case MyRelationsBetweenPlayerAndBlock.FactionShare:
        case MyRelationsBetweenPlayerAndBlock.Owner:
            baseColor = new Color(40, 255, 90);
            break;

        case MyRelationsBetweenPlayerAndBlock.Neutral:
        case MyRelationsBetweenPlayerAndBlock.NoOwnership:
            baseColor = new Color(255, 195, 30);
            break;

        default:
            baseColor = new Color(210, 210, 210);
            break;
    }

    return new Color(
        (byte)(baseColor.R * intensity),
        (byte)(baseColor.G * intensity),
        (byte)(baseColor.B * intensity),
        255
    );
}


string RelationshipToText(
    MyRelationsBetweenPlayerAndBlock relationship)
{
    switch (relationship)
    {
        case MyRelationsBetweenPlayerAndBlock.Enemies:
            return "ENEMY";

        case MyRelationsBetweenPlayerAndBlock.Friends:
        case MyRelationsBetweenPlayerAndBlock.FactionShare:
            return "FRIEND";

        case MyRelationsBetweenPlayerAndBlock.Owner:
            return "OWN";

        case MyRelationsBetweenPlayerAndBlock.Neutral:
            return "NEUTRAL";

        case MyRelationsBetweenPlayerAndBlock.NoOwnership:
            return "UNOWNED";

        default:
            return "UNKNOWN";
    }
}


string GetContactName(
    MyDetectedEntityInfo info)
{
    if (!string.IsNullOrWhiteSpace(info.Name))
        return info.Name;

    return info.Type.ToString();
}


string FormatDistance(double distance)
{
    if (distance >= 1000)
    {
        return (distance / 1000.0)
            .ToString("0.0")
            + "km";
    }

    return distance.ToString("0") + "m";
}


// ============================================================
// PŘÍKAZY
// ============================================================

void ProcessCommand(string argument)
{
    string command =
        argument.Trim().ToLowerInvariant();

    if (command == "reset")
    {
        contacts.Clear();
        return;
    }

    if (command == "start")
    {
        radarRunning = true;
        rotor.Enabled = true;

        // Při spuštění se čára srovná s fyzickým senzorem.
        visualSweepAngle =
            GetRealSensorAngle();

        MaintainRotorSpeed();
        return;
    }

    if (command == "stop")
    {
        radarRunning = false;
        rotor.TargetVelocityRPM = 0;
        return;
    }

    if (command == "sync")
    {
        // Ruční přesné srovnání grafiky se senzorem.
        visualSweepAngle =
            GetRealSensorAngle();

        return;
    }

    if (command == "reload")
    {
        FindBlocks();
        ConfigureSurfaces();
        ConfigureRotor();
        RebuildLocalGridList();

        visualSweepAngle =
            GetRealSensorAngle();

        visualSweepInitialized = true;
        return;
    }

    if (command == "grids")
    {
        RebuildLocalGridList();
        RemoveOwnGridsFromHistory();
        return;
    }
}


// ============================================================
// CHYBA A ECHO
// ============================================================

void DrawError()
{
    Echo(errorMessage);

    if (radarSurface != null)
    {
        radarSurface.ContentType =
            ContentType.TEXT_AND_IMAGE;

        radarSurface.WriteText(
            "RADAR ERROR\n\n"
            + errorMessage
        );
    }

    if (contactsSurface != null)
    {
        contactsSurface.ContentType =
            ContentType.TEXT_AND_IMAGE;

        contactsSurface.WriteText(
            "RADAR ERROR\n\n"
            + errorMessage
        );
    }
}


void EchoStatus()
{
    Echo("=== [RADAR] V3 ===");

    Echo(
        "Rotor: "
        + rotor.TargetVelocityRPM
            .ToString("0.00")
        + " RPM"
    );

    Echo(
        "Obeh: "
        + ROTATION_TIME
            .ToString("0.0")
        + " s"
    );

    Echo(
        "Kontakty: "
        + contacts.Count
    );

    Echo(
        "Vlastni gridy: "
        + localGridIds.Count
    );

    Echo(
        "Senzor detekuje: "
        + detectedEntities.Count
    );

    Echo("");

    Echo("reset / start / stop");
    Echo("sync / reload / grids");
}


// ============================================================
// DATOVÁ TŘÍDA
// ============================================================

class RadarContact
{
    public long EntityId;
    public string Name;

    public Vector3D Position;

    public double Distance;
    public double BearingRadians;
    public double BearingDegrees;

    public int ClockPosition;

    public MyRelationsBetweenPlayerAndBlock Relationship;
    public MyDetectedEntityType Type;

    public double LastDetectedTime;
    public int DetectionCount;
}