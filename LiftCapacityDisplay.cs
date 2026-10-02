// ============================================================
// LIFT / CARGO CAPACITY DISPLAY
// Space Engineers 1 - Programmable Block
//
// Zeigt an, wie viel zusätzliche Masse das Schiff bei der
// aktuellen natürlichen Gravitation noch sicher tragen kann.
//
// Unterstützt:
// - normale LCD Panels
// - eingebaute Cockpit-/Control-Seat-Displays
//
// Benennung:
// Jeder Block, der "[Lift]" im Namen trägt, wird als Anzeige
// verwendet.
//
// Bei normalen LCDs wird direkt auf das LCD geschrieben.
// Bei Cockpits wird COCKPIT_SCREEN verwendet.
// ============================================================


// ------------------------------------------------------------
// EINSTELLUNGEN
// ------------------------------------------------------------

// LCD oder Cockpit muss diesen Text im Namen haben.
const string LCD_TAG = "[Lift]";

// Nummer des internen Cockpit-Displays.
//
// Falls das falsche Display beschrieben wird:
// 0 -> 1 -> 2 -> 3 ... durchprobieren.
//
// Wichtig:
// Die verfügbaren Surface-Nummern unterscheiden sich je
// nach Cockpit-Modell.
const int COCKPIT_SCREEN = 0;

// Sicherheitsreserve.
//
// 0.85 bedeutet:
// Nur 85 % der theoretischen maximalen Tragfähigkeit
// werden als sichere Tragfähigkeit verwendet.
//
// Dadurch bleiben 15 % Reserve.
const double SAFETY_FACTOR = 0.85;

// Unterhalb dieser natürlichen Gravitation behandeln wir
// das Schiff als praktisch schwerelos.
//
// Einheit: m/s²
const double MIN_GRAVITY = 0.05;

// Erdbeschleunigung.
//
// Wird nur verwendet, um die aktuelle Gravitation
// als "g" auf dem Display darzustellen.
//
// Einheit: m/s²
const double EARTH_GRAVITY = 9.81;


// ------------------------------------------------------------
// BLOCK-LISTEN
// ------------------------------------------------------------

List<IMyShipController> controllers = new List<IMyShipController>();
List<IMyThrust> thrusters = new List<IMyThrust>();
List<IMyTerminalBlock> displayBlocks = new List<IMyTerminalBlock>();


// ============================================================
// PROGRAM START
// ============================================================

// Zweck:
// Initialisiert das Skript.
//
// Input:
// Keiner.
//
// Output:
// Keiner.
//
// Side Effects:
// Aktiviert automatische Aktualisierung ungefähr alle
// 1,6 Sekunden.
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}


// ============================================================
// MAIN
// ============================================================

// Zweck:
// Berechnet Lift, maximale Masse und verbleibende
// sichere Zuladung.
//
// Input:
// argument / updateSource werden von Space Engineers geliefert.
//
// Output:
// Keiner.
//
// Side Effects:
// Aktualisiert alle passenden Displays.
public void Main(string argument, UpdateType updateSource)
{
    IMyShipController controller = FindShipController();

    if (controller == null)
    {
        WriteOutput(
            "=== LIFT CAPACITY ===\n\n" +
            "ERROR\n\n" +
            "No cockpit or\n" +
            "remote control found."
        );

        return;
    }


    // --------------------------------------------------------
    // GRAVITATION
    // --------------------------------------------------------

    // Natürliche Gravitation am aktuellen Standort.
    //
    // Beispiel Erde:
    // ungefähr 9,81 m/s²
    Vector3D gravity = controller.GetNaturalGravity();

    double gravityStrength = gravity.Length();


    // --------------------------------------------------------
    // SCHIFFSMASSE
    // --------------------------------------------------------

    // PhysicalMass ist die für die tatsächliche Flugphysik
    // relevante Masse.
    //
    // Sie enthält auch die Masse der geladenen Gegenstände.
    MyShipMass shipMass = controller.CalculateShipMass();

    double currentMass = shipMass.PhysicalMass;


    // --------------------------------------------------------
    // SCHWERELOSIGKEIT
    // --------------------------------------------------------

    if (gravityStrength < MIN_GRAVITY)
    {
        string spaceOutput =
            "=== LIFT CAPACITY ===\n\n" +

            "Gravity:    0.00 g\n" +

            "Ship Mass:  " +
            FormatMass(currentMass) +
            "\n\n" +

            "NO NATURAL GRAVITY\n\n" +

            "Lift limit not relevant.";

        WriteOutput(spaceOutput);

        return;
    }


    // --------------------------------------------------------
    // LIFT-RICHTUNG
    // --------------------------------------------------------

    // Die Richtung, in die das Schiff getragen werden muss,
    // ist exakt entgegen der natürlichen Gravitation.
    //
    // Dadurch funktioniert die Berechnung auch dann korrekt,
    // wenn das Schiff schräg steht.
    Vector3D liftDirection =
        -Vector3D.Normalize(gravity);


    // --------------------------------------------------------
    // VERFÜGBAREN LIFT BERECHNEN
    // --------------------------------------------------------

    double totalLiftThrust =
        CalculateLiftThrust(liftDirection);


    if (totalLiftThrust <= 0)
    {
        string noThrustOutput =
            "=== LIFT CAPACITY ===\n\n" +

            "Gravity:    " +
            (gravityStrength / EARTH_GRAVITY).ToString("0.00") +
            " g\n" +

            "Ship Mass:  " +
            FormatMass(currentMass) +
            "\n\n" +

            "NO UPWARD THRUST";

        WriteOutput(noThrustOutput);

        return;
    }


    // --------------------------------------------------------
    // MAXIMALE MASSE
    // --------------------------------------------------------

    // Physik:
    //
    // F = m * g
    //
    // umgestellt:
    //
    // m = F / g
    //
    // totalLiftThrust = Newton
    // gravityStrength = m/s²
    // Ergebnis = kg
    double theoreticalMaxMass =
        totalLiftThrust / gravityStrength;


    // --------------------------------------------------------
    // SICHERE MAXIMALE MASSE
    // --------------------------------------------------------

    // Sicherheitsreserve anwenden.
    double safeMaxMass =
        theoreticalMaxMass * SAFETY_FACTOR;


    // --------------------------------------------------------
    // VERBLEIBENDE ZULADUNG
    // --------------------------------------------------------

    double remainingSafeMass =
        safeMaxMass - currentMass;


    // --------------------------------------------------------
    // AKTUELL BENÖTIGTER SCHUB
    // --------------------------------------------------------

    // Kraft, die aktuell notwendig ist,
    // um das Schiff nur schweben zu lassen.
    double requiredHoverThrust =
        currentMass * gravityStrength;


    // Anteil der theoretischen Lift-Kapazität,
    // der bereits durch die aktuelle Masse benötigt wird.
    double liftUsagePercent =
        (requiredHoverThrust / totalLiftThrust) * 100.0;


    // --------------------------------------------------------
    // STATUS
    // --------------------------------------------------------

    string status;

    if (currentMass > theoreticalMaxMass)
    {
        status = "OVERLOADED";
    }
    else if (currentMass > safeMaxMass)
    {
        status = "WARNING";
    }
    else
    {
        status = "SAFE";
    }


    // Negative Zuladung nicht als negative Zahl darstellen.
    double displayedRemainingMass =
        Math.Max(0, remainingSafeMass);


    // --------------------------------------------------------
    // DISPLAY-TEXT
    // --------------------------------------------------------

    string output =
        "=== LIFT CAPACITY ===\n\n" +

        "Gravity:       " +
        (gravityStrength / EARTH_GRAVITY).ToString("0.00") +
        " g\n" +

        "Ship Mass:     " +
        FormatMass(currentMass) +
        "\n\n" +

        "Lift Thrust:   " +
        FormatForce(totalLiftThrust) +
        "\n" +

        "Lift Usage:    " +
        liftUsagePercent.ToString("0.0") +
        " %\n\n" +

        "Max Mass:      " +
        FormatMass(theoreticalMaxMass) +
        "\n" +

        "Safe Max:      " +
        FormatMass(safeMaxMass) +
        "\n\n" +

        "CAN LOAD:\n" +
        FormatMass(displayedRemainingMass) +
        "\n\n" +

        "STATUS: " +
        status;


    WriteOutput(output);
}


// ============================================================
// SHIP CONTROLLER FINDEN
// ============================================================

// Zweck:
// Findet einen Ship Controller auf demselben Construct.
//
// Priorität:
// 1. Main Cockpit
// 2. irgendein anderes Cockpit / Remote Control
//
// Input:
// Keiner.
//
// Output:
// Gefundener Ship Controller oder null.
//
// Side Effects:
// Aktualisiert die interne Controller-Liste.
IMyShipController FindShipController()
{
    controllers.Clear();

    GridTerminalSystem.GetBlocksOfType(
        controllers,
        block => block.IsSameConstructAs(Me)
    );


    if (controllers.Count == 0)
        return null;


    // Main Cockpit bevorzugen.
    for (int i = 0; i < controllers.Count; i++)
    {
        if (controllers[i].IsMainCockpit)
            return controllers[i];
    }


    // Falls kein Main Cockpit gesetzt ist,
    // den ersten verfügbaren Controller verwenden.
    return controllers[0];
}


// ============================================================
// LIFT THRUST BERECHNEN
// ============================================================

// Zweck:
// Berechnet, wie viel effektiver Thruster-Schub tatsächlich
// entgegen der aktuellen Gravitation wirkt.
//
// Input:
// liftDirection = normalisierter Weltvektor entgegen Gravity.
//
// Output:
// Gesamter nutzbarer Lift-Schub in Newton.
//
// Side Effects:
// Aktualisiert die interne Thruster-Liste.
double CalculateLiftThrust(Vector3D liftDirection)
{
    thrusters.Clear();

    GridTerminalSystem.GetBlocksOfType(
        thrusters,
        block => block.IsSameConstructAs(Me)
    );


    double liftThrust = 0;


    for (int i = 0; i < thrusters.Count; i++)
    {
        IMyThrust thruster = thrusters[i];


        // Nicht funktionierende oder ausgeschaltete Thruster
        // dürfen keine Tragfähigkeit vortäuschen.
        if (!thruster.IsWorking)
            continue;


        // Ein Thruster erzeugt seine Kraft entlang
        // seiner WorldMatrix.Backward-Richtung.
        Vector3D thrustDirection =
            thruster.WorldMatrix.Backward;


        // Dot Product zwischen:
        //
        // Thruster-Richtung
        // und
        // gewünschter Lift-Richtung.
        //
        // 1.0 = exakt nach oben
        // 0.5 = 50 % nutzbarer Lift
        // 0.0 = rein seitlich
        // < 0 = drückt in Richtung Gravity
        double alignment =
            Vector3D.Dot(
                thrustDirection,
                liftDirection
            );


        // Seitliche oder nach unten wirkende Thruster
        // tragen nicht zum Lift bei.
        if (alignment <= 0)
            continue;


        // MaxEffectiveThrust berücksichtigt die aktuell
        // tatsächlich mögliche Leistung des Thrusters.
        //
        // Das ist besonders wichtig für:
        // - Atmospheric Thrusters
        // - Ion Thrusters
        //
        // deren Effektivität von der Umgebung abhängt.
        liftThrust +=
            thruster.MaxEffectiveThrust *
            alignment;
    }


    return liftThrust;
}


// ============================================================
// DISPLAY AUSGABE
// ============================================================

// Zweck:
// Schreibt den Text auf alle passenden Anzeigen.
//
// Unterstützt:
//
// 1. normale LCD Panels mit "[Lift]" im Namen
//
// 2. Cockpits / Control Seats / andere Surface Provider
//    mit "[Lift]" im Namen
//
// Bei Cockpits wird COCKPIT_SCREEN verwendet.
//
// Input:
// text = kompletter Displaytext.
//
// Output:
// Keiner.
//
// Side Effects:
// Überschreibt den Inhalt der passenden Displays.
void WriteOutput(string text)
{
    displayBlocks.Clear();


    GridTerminalSystem.GetBlocksOfType(
        displayBlocks,
        block =>
            block.IsSameConstructAs(Me) &&
            block.CustomName.Contains(LCD_TAG)
    );


    // Zusätzlich im Programmable Block anzeigen.
    Echo(text);


    for (int i = 0; i < displayBlocks.Count; i++)
    {
        IMyTerminalBlock block =
            displayBlocks[i];


        // ----------------------------------------------------
        // NORMALES LCD
        // ----------------------------------------------------

        IMyTextPanel textPanel =
            block as IMyTextPanel;


        if (textPanel != null)
        {
            textPanel.ContentType =
                ContentType.TEXT_AND_IMAGE;

            textPanel.WriteText(
                text,
                false
            );

            continue;
        }


        // ----------------------------------------------------
        // COCKPIT / CONTROL SEAT / MULTI-SURFACE BLOCK
        // ----------------------------------------------------

        IMyTextSurfaceProvider surfaceProvider =
            block as IMyTextSurfaceProvider;


        if (surfaceProvider == null)
            continue;


        // Prüfen, ob dieses Cockpit überhaupt so viele
        // Displays besitzt.
        if (
            COCKPIT_SCREEN < 0 ||
            COCKPIT_SCREEN >= surfaceProvider.SurfaceCount
        )
        {
            continue;
        }


        IMyTextSurface surface =
            surfaceProvider.GetSurface(
                COCKPIT_SCREEN
            );


        surface.ContentType =
            ContentType.TEXT_AND_IMAGE;


        surface.WriteText(
            text,
            false
        );
    }
}


// ============================================================
// MASSENFORMATIERUNG
// ============================================================

// Zweck:
// Formatiert eine Masse lesbar für das Display.
//
// Input:
// massKg = Masse in Kilogramm.
//
// Output:
// Text in kg oder Tonnen.
//
// Side Effects:
// Keine.
string FormatMass(double massKg)
{
    if (massKg >= 1000)
    {
        return
            (massKg / 1000.0).ToString("0.0") +
            " t";
    }


    return
        massKg.ToString("0") +
        " kg";
}


// ============================================================
// KRAFTFORMATIERUNG
// ============================================================

// Zweck:
// Formatiert Thruster-Kraft kompakt.
//
// Input:
// forceNewton = Kraft in Newton.
//
// Output:
// Text in kN oder MN.
//
// Side Effects:
// Keine.
string FormatForce(double forceNewton)
{
    if (forceNewton >= 1000000)
    {
        return
            (forceNewton / 1000000.0).ToString("0.00") +
            " MN";
    }


    return
        (forceNewton / 1000.0).ToString("0.0") +
        " kN";
}