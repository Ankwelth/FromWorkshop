// ============================================================
// Connector-Status -> Lampengruppe Sync + Anzeige-Screen
// ============================================================
// TEIL 1: Lampen-Logik (wie bisher)
// ------------------------------------------------------------
// Du gibst Connector und Lampengruppe den GLEICHEN Präfix in
// eckigen Klammern im Namen, z.B.:
//   Connector-Name:      "[Hangar] Connector"
//   Lampengruppen-Name:  "[Hangar] Lampen"
//
// Farblogik (5 Zustände):
//   Aus         (Connector deaktiviert)           -> Rot
//   Aktiv       (an, aber nichts in Reichweite)    -> Weiß
//   Bereit      (Connectable, bereit zum Andocken) -> Gelb
//   Verbunden   (Connected, angedockt)             -> Grün
//   Getrennt    (kurz nachdem entkoppelt wurde)    -> Blau
//
// "Aus" kommt von Enabled, "Getrennt" ist ein selbst gebauter
// Übergangszustand (siehe letzterStatus/blauZaehler unten),
// weil das Spiel diese zwei Zustände nicht direkt als Status-
// Wert herausgibt.
//
// Mod-Connectoren werden automatisch mit erfasst, weil JEDER
// Connector-Block (auch aus Mods) das Spiel-Interface
// "IMyShipConnector" implementiert.
//
// TEIL 2: Bildschirm auf dem Programmierbaren Block (NEU)
// ------------------------------------------------------------
// Der Screen des Programmierbaren Blocks (Me.GetSurface(0))
// zeigt:
//   - oben eine kleine Statistik (wie viele Connectoren in
//     welchem Zustand sind)
//   - eine Liste der einzelnen [Prefix]-Gruppen mit Status
//   - unten eine kleine Docking-Animation: ein Schiff (Dreieck)
//     fliegt immer wieder auf eine Station (Rechteck) zu. Wenn
//     mindestens ein echter Connector wirklich "Verbunden" ist,
//     bleibt das Schiff angedockt und die Station leuchtet grün
//     - ansonsten läuft die Animation als Deko-Loop weiter.
//
// Sprites (die kleinen Bild-Bausteine wie Quadrat/Kreis/Dreieck)
// werden über "frame.Add(sprite)" auf den Screen gezeichnet.
// Das ist eine offizielle Spielfunktion, keine Mod.
// ============================================================

// ---------- Farben ----------
Color colorAus       = new Color(255, 0, 0);      // Rot
Color colorAktiv     = new Color(255, 255, 255);  // Weiß
Color colorBereit    = new Color(255, 255, 0);    // Gelb
Color colorVerbunden = new Color(0, 255, 0);      // Grün
Color colorGetrennt  = new Color(0, 100, 255);    // Blau

// Wie viele Skript-Durchläufe lang soll "Blau" nach dem Trennen
// angezeigt werden?
const int BLAU_DAUER_DURCHLAEUFE = 6;

// Merkt sich pro Connector (per EntityId) den letzten Status und
// wie viele Durchläufe er noch Blau leuchten soll.
Dictionary<long, MyShipConnectorStatus> letzterStatus = new Dictionary<long, MyShipConnectorStatus>();
Dictionary<long, int> blauZaehler = new Dictionary<long, int>();

// ---------- Screen-Zubehör ----------
IMyTextSurface pbScreen;
RectangleF viewport;
float animT = 0f; // Fortschritt der Docking-Animation, 0 = ganz links, 1 = angedockt

// Kleine Hilfsklasse, damit wir Farbe + Name zusammen
// zurückgeben können (einfacher als zwei Rückgabewerte separat
// zu verwalten).
class ConnectorZustand
{
    public Color Farbe;
    public string Name;
}

// Ein Eintrag für die Anzeige-Liste: ein verarbeiteter Connector
// mit seinem Prefix und aktuellem Zustand.
class ConnectorEintrag
{
    public string Prefix;
    public string Name;
    public Color Farbe;
}

public Program()
{
    pbScreen = Me.GetSurface(0);

    // Screen für eigenes Sprite-Zeichnen vorbereiten (kein
    // eingebautes Skript wie "Text" oder "Bild" soll aktiv sein)
    pbScreen.ContentType = ContentType.SCRIPT;
    pbScreen.Script = "";

    // Viewport = der tatsächlich sichtbare/gezeichnete Bereich,
    // zentriert auf der Textur des Screens
    viewport = new RectangleF(
        (pbScreen.TextureSize - pbScreen.SurfaceSize) / 2f,
        pbScreen.SurfaceSize
    );

    // Update10 = ca. alle 0,16s -> flüssig genug für die
    // Animation, ohne zu viel Rechenlast zu erzeugen.
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

void Main(string argument, UpdateType updateSource)
{
    List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);

    // Zähler für die Statistik, alle Kategorien starten bei 0
    Dictionary<string, int> zaehler = new Dictionary<string, int>
    {
        { "Verbunden", 0 },
        { "Bereit", 0 },
        { "Aktiv", 0 },
        { "Getrennt", 0 },
        { "Aus", 0 },
    };

    List<ConnectorEintrag> verarbeiteteConnectoren = new List<ConnectorEintrag>();

    if (connectors.Count == 0)
    {
        Echo("Keine Connectoren gefunden.");
        ZeichneScreen(zaehler, verarbeiteteConnectoren, false);
        return;
    }

    foreach (var connector in connectors)
    {
        string prefix = GetPrefix(connector.CustomName);
        if (prefix == null)
            continue; // kein "[...]" im Namen -> ignorieren

        IMyBlockGroup lightGroup = FindGroupByPrefix(prefix);

        ConnectorZustand zustand = ErmittleZustand(connector);
        zaehler[zustand.Name]++;

        verarbeiteteConnectoren.Add(new ConnectorEintrag
        {
            Prefix = prefix,
            Name = zustand.Name,
            Farbe = zustand.Farbe,
        });

        if (lightGroup != null)
            SetGroupColor(lightGroup, zustand.Farbe);
    }

    bool mindestensEinVerbunden = false;
    foreach (var eintrag in verarbeiteteConnectoren)
    {
        if (eintrag.Name == "Verbunden")
        {
            mindestensEinVerbunden = true;
            break;
        }
    }

    Echo($"{verarbeiteteConnectoren.Count} von {connectors.Count} Connectoren mit Prefix verarbeitet.");
    ZeichneScreen(zaehler, verarbeiteteConnectoren, mindestensEinVerbunden);
}

// Sucht den Text zwischen der ersten "[" und "]" in einem Namen.
string GetPrefix(string name)
{
    int start = name.IndexOf('[');
    int end = name.IndexOf(']');

    if (start == -1 || end == -1 || end <= start)
        return null;

    return name.Substring(start + 1, end - start - 1);
}

// Durchsucht alle Blockgruppen nach einer, die den gesuchten
// Präfix (in eckigen Klammern) im Namen trägt.
IMyBlockGroup FindGroupByPrefix(string prefix)
{
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);

    string tag = $"[{prefix}]";

    foreach (var group in groups)
    {
        if (group.Name.Contains(tag))
            return group;
    }

    return null;
}

// Ermittelt Farbe UND Namen des aktuellen Zustands für EINEN
// Connector und aktualisiert dabei den "Blau-Zähler".
ConnectorZustand ErmittleZustand(IMyShipConnector connector)
{
    long id = connector.EntityId;

    MyShipConnectorStatus vorher = letzterStatus.ContainsKey(id)
        ? letzterStatus[id]
        : connector.Status;

    if (vorher == MyShipConnectorStatus.Connected &&
        connector.Status != MyShipConnectorStatus.Connected)
    {
        blauZaehler[id] = BLAU_DAUER_DURCHLAEUFE;
    }

    letzterStatus[id] = connector.Status;

    if (!connector.Enabled)
        return new ConnectorZustand { Farbe = colorAus, Name = "Aus" };

    int verbleibendeBlauDurchlaeufe;
    if (blauZaehler.TryGetValue(id, out verbleibendeBlauDurchlaeufe) &&
        verbleibendeBlauDurchlaeufe > 0)
    {
        blauZaehler[id] = verbleibendeBlauDurchlaeufe - 1;
        return new ConnectorZustand { Farbe = colorGetrennt, Name = "Getrennt" };
    }

    switch (connector.Status)
    {
        case MyShipConnectorStatus.Connected:
            return new ConnectorZustand { Farbe = colorVerbunden, Name = "Verbunden" };
        case MyShipConnectorStatus.Connectable:
            return new ConnectorZustand { Farbe = colorBereit, Name = "Bereit" };
        case MyShipConnectorStatus.Unconnected:
        default:
            return new ConnectorZustand { Farbe = colorAktiv, Name = "Aktiv" };
    }
}

// Setzt die Farbe aller Lichtquellen in einer Gruppe.
void SetGroupColor(IMyBlockGroup group, Color color)
{
    List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
    group.GetBlocksOfType<IMyLightingBlock>(lights);

    foreach (var light in lights)
    {
        light.Color = color;
    }
}

// ============================================================
// Ab hier: alles, was auf den Bildschirm des Programmierbaren
// Blocks gezeichnet wird.
// ============================================================

void ZeichneScreen(Dictionary<string, int> zaehler, List<ConnectorEintrag> connectorListe, bool mindestensEinVerbunden)
{
    var frame = pbScreen.DrawFrame();

    // Hintergrund
    MySprite hintergrund = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = viewport.Center,
        Size = viewport.Size,
        Color = new Color(10, 10, 20),
    };
    frame.Add(hintergrund);

    float x = viewport.Position.X + 16;
    float y = viewport.Position.Y + 12;

    MySprite titel = MySprite.CreateText("CONNECTOR STATUS", "Debug", Color.White, 0.9f, TextAlignment.LEFT);
    titel.Position = new Vector2(x, y);
    frame.Add(titel);
    y += 34;

    // Statistik-Zeilen
    ZeichneStatistikZeile(ref frame, x, ref y, "Verbunden", zaehler["Verbunden"], colorVerbunden);
    ZeichneStatistikZeile(ref frame, x, ref y, "Bereit", zaehler["Bereit"], colorBereit);
    ZeichneStatistikZeile(ref frame, x, ref y, "Aktiv", zaehler["Aktiv"], colorAktiv);
    ZeichneStatistikZeile(ref frame, x, ref y, "Getrennt", zaehler["Getrennt"], colorGetrennt);
    ZeichneStatistikZeile(ref frame, x, ref y, "Aus", zaehler["Aus"], colorAus);

    y += 8;

    // Liste der einzelnen [Prefix]-Connectoren, max. 5 Zeilen,
    // damit der Screen nicht überläuft
    int maxAnzeigen = 5;
    for (int i = 0; i < connectorListe.Count && i < maxAnzeigen; i++)
    {
        ZeichneConnectorZeile(ref frame, x, ref y, connectorListe[i]);
    }
    if (connectorListe.Count > maxAnzeigen)
    {
        MySprite mehr = MySprite.CreateText($"... und {connectorListe.Count - maxAnzeigen} weitere", "Debug", new Color(140, 140, 140), 0.55f, TextAlignment.LEFT);
        mehr.Position = new Vector2(x, y);
        frame.Add(mehr);
    }

    ZeichneDockingAnimation(ref frame, mindestensEinVerbunden);

    frame.Dispose();
}

// Eine Zeile der Statistik: farbiges Quadrat + "Label: Anzahl"
void ZeichneStatistikZeile(ref MySpriteDrawFrame frame, float x, ref float y, string label, int anzahl, Color farbe)
{
    MySprite quadrat = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = new Vector2(x + 7, y + 7),
        Size = new Vector2(14, 14),
        Color = farbe,
    };
    frame.Add(quadrat);

    MySprite text = MySprite.CreateText($"{label}: {anzahl}", "Debug", Color.White, 0.65f, TextAlignment.LEFT);
    text.Position = new Vector2(x + 24, y);
    frame.Add(text);

    y += 22;
}

// Eine Zeile für einen einzelnen Connector: farbiger Punkt +
// "[Prefix] Zustand"
void ZeichneConnectorZeile(ref MySpriteDrawFrame frame, float x, ref float y, ConnectorEintrag eintrag)
{
    MySprite punkt = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "Circle",
        Position = new Vector2(x + 6, y + 6),
        Size = new Vector2(12, 12),
        Color = eintrag.Farbe,
    };
    frame.Add(punkt);

    MySprite text = MySprite.CreateText($"[{eintrag.Prefix}] {eintrag.Name}", "Debug", new Color(190, 190, 190), 0.55f, TextAlignment.LEFT);
    text.Position = new Vector2(x + 22, y);
    frame.Add(text);

    y += 19;
}

// Zeichnet die kleine Docking-Animation im unteren Bereich des
// Screens: Ein Dreieck ("Schiff") fliegt von links nach rechts
// auf ein Quadrat ("Station") zu.
void ZeichneDockingAnimation(ref MySpriteDrawFrame frame, bool mindestensEinVerbunden)
{
    float animY = viewport.Position.Y + viewport.Height - 40;
    float startX = viewport.Position.X + 24;
    float endX = viewport.Position.X + viewport.Width - 34;

    // Fortschritt (0..1) berechnen. Ist wirklich etwas verbunden,
    // bleibt das Schiff einfach bei 1 (angedockt) stehen.
    float t;
    if (mindestensEinVerbunden)
    {
        t = 1f;
    }
    else
    {
        // Animation läuft weiter, damit es nicht "einfriert"
        animT += 0.02f;
        if (animT > 1.3f)
            animT = 0f;

        // 0..1 = Anflug, 1..1.3 = kurze Pause angedockt
        t = MathHelper.Clamp(animT, 0f, 1f);
    }

    // Ease-Out: schnell am Anfang, sanftes Abbremsen am Ende
    float easedT = 1f - (1f - t) * (1f - t);
    float schiffX = MathHelper.Lerp(startX, endX - 6, easedT);

    bool zeigtAngedockt = mindestensEinVerbunden || animT >= 1f;
    Color stationFarbe = zeigtAngedockt ? colorVerbunden : new Color(110, 110, 120);

    MySprite station = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "SquareSimple",
        Position = new Vector2(endX, animY),
        Size = new Vector2(18, 34),
        Color = stationFarbe,
    };
    frame.Add(station);

    MySprite schiff = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "Triangle",
        Position = new Vector2(schiffX, animY),
        Size = new Vector2(20, 20),
        Color = Color.White,
        RotationOrScale = MathHelper.ToRadians(90),
    };
    frame.Add(schiff);
}