// ============================================================
// RAVEN - ROVER CONTROL SYSTEM
// ============================================================
//
// RAVEN = Rover Automated Vehicle Electronics Network
//
// Tags:
//
//   Headlight
//   Left Indicator
//   Right Indicator
//   Rear Light
//   Driver
//   Boost Thruster
//   RAVEN_LCD
//
// RAVEN LCD examples:
//
//   RAVEN_LCD
//   RAVEN_LCD:1
//   RAVEN_LCD:2
//   RAVEN_LCD:3
//
// Programmable Block argument:
//
//   toggleboost
//
// ============================================================

const string VERSION = "2.0.0";


// ============================================================
// CONFIGURATION
// ============================================================

class Config
{
    public bool DetectSubgrids = true;
    public int UpdateFrequency = 10;

    public string HeadlightTag = "Headlight";
    public string LeftIndicatorTag = "Left Indicator";
    public string RightIndicatorTag = "Right Indicator";
    public string RearLightTag = "Rear Light";
    public string DriverTag = "Driver";
    public string BoostThrusterTag = "Boost Thruster";
    public string RavenTag = "RAVEN_LCD";

    public Color HeadlightColor =
        new Color(255, 245, 220);

    public float HeadlightIntensity = 5f;
    public float HeadlightFalloff = 1f;

    public Color TailColor =
        new Color(255, 0, 0);

    public float TailIntensity = 1.25f;
    public float TailRange = 5f;
    public float TailFalloff = 1f;

    public Color BrakeColor =
        new Color(255, 0, 0);

    public float BrakeIntensity = 5f;
    public float BrakeRange = 5f;
    public float BrakeFalloff = 1f;

    public Color ReverseColor =
        new Color(255, 255, 255);

    public float ReverseIntensity = 5f;
    public float ReverseRange = 20f;
    public float ReverseFalloff = 1f;

    public Color IndicatorColor =
        new Color(255, 160, 0);

    public float IndicatorIntensity = 5f;
    public float IndicatorRange = 1f;
    public float IndicatorFalloff = 0f;
    public float IndicatorBlinkSpeed = 0.20f;

    public bool AutoWheelControl = true;

    public float MaxSteeringAngle = 25f;
    public float MinSteeringAngle = 4f;

    public float MaxStrength = 100f;
    public float MinStrength = 5f;

    public float MaxFriction = 100f;
    public float MinFriction = 20f;

    public float MaxPower = 60f;
    public float MinPower = 5f;

    public float SpeedForMinimumSteering = 10f;
    public float SpeedForMaximumSteering = 25f;

    public float TargetMassPerWheel = 2500f;

    public float LateralGripMultiplier = 1.0f;

    public float BrakeSpeedThreshold = 1.0f;
    public float ReverseSpeedThreshold = 1.0f;

    public float NormalSpeedLimit = 30f;
    public float BoostSpeedLimit = 100f;
}

Config cfg = new Config();


// ============================================================
// BLOCK LISTS
// ============================================================

List<IMyLightingBlock> headlights =
    new List<IMyLightingBlock>();

List<IMyLightingBlock> leftIndicators =
    new List<IMyLightingBlock>();

List<IMyLightingBlock> rightIndicators =
    new List<IMyLightingBlock>();

List<IMyLightingBlock> rearLights =
    new List<IMyLightingBlock>();

List<IMyMotorSuspension> wheels =
    new List<IMyMotorSuspension>();

List<IMyShipController> drivers =
    new List<IMyShipController>();

List<IMyThrust> boostThrusters =
    new List<IMyThrust>();


class RavenDisplay
{
    public IMyTextSurface Surface;
    public int Index;
    public IMyTerminalBlock Block;
}

List<RavenDisplay> ravenDisplays =
    new List<RavenDisplay>();


// ============================================================
// GRID CACHE
// ============================================================

HashSet<long> validGridIds =
    new HashSet<long>();


// ============================================================
// STATE
// ============================================================

IMyShipController activeDriver = null;

long driverGridId = 0;

double indicatorTimer = 0;

bool indicatorState = false;

bool reversing = false;

bool boostEnabled = false;

bool braking = false;

bool turningLeft = false;

bool turningRight = false;


// ============================================================
// PROGRAM
// ============================================================

public Program()
{
    EnsureConfiguration();
    LoadConfiguration();

    Runtime.UpdateFrequency =
        UpdateFrequency.Update10;

    DiscoverBlocks();
}


// ============================================================
// MAIN
// ============================================================

public void Main(
    string argument,
    UpdateType updateSource)
{
    if (!string.IsNullOrWhiteSpace(argument))
    {
        ProcessCommand(argument);
    }

    DiscoverActiveDriver();

    indicatorTimer +=
        Runtime.TimeSinceLastRun.TotalSeconds;

    if (indicatorTimer >= cfg.IndicatorBlinkSpeed)
    {
        indicatorTimer = 0;
        indicatorState = !indicatorState;
    }

    if (activeDriver != null)
    {
        UpdateLighting();
        UpdateWheels();
        UpdateBoostThrusters();
    }
    else
    {
        SetIndicators(false, false);
        SetBoostThrusters(false);

        braking = false;
        reversing = false;
        turningLeft = false;
        turningRight = false;
    }

    UpdateRavenDisplays();

    EchoStatus();
}


// ============================================================
// COMMANDS
// ============================================================

void ProcessCommand(string argument)
{
    argument = argument.Trim();

    if (argument.Equals(
        "toggleboost",
        StringComparison.OrdinalIgnoreCase))
    {
        boostEnabled = !boostEnabled;
    }
}


// ============================================================
// DISCOVER BLOCKS
// ============================================================

void DiscoverBlocks()
{
    headlights.Clear();
    leftIndicators.Clear();
    rightIndicators.Clear();
    rearLights.Clear();
    wheels.Clear();
    drivers.Clear();
    boostThrusters.Clear();
    ravenDisplays.Clear();

    BuildValidGridList();

    List<IMyTerminalBlock> blocks =
        new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocks(blocks);

    foreach (IMyTerminalBlock block in blocks)
    {
        if (!IsValidGrid(block))
            continue;

        string name =
            block.CustomName;

        string data =
            block.CustomData;

        IMyLightingBlock light =
            block as IMyLightingBlock;

        if (light != null)
        {
            if (HasTag(
                name,
                data,
                cfg.HeadlightTag))
            {
                headlights.Add(light);
            }

            if (HasTag(
                name,
                data,
                cfg.LeftIndicatorTag))
            {
                leftIndicators.Add(light);
            }

            if (HasTag(
                name,
                data,
                cfg.RightIndicatorTag))
            {
                rightIndicators.Add(light);
            }

            if (HasTag(
                name,
                data,
                cfg.RearLightTag))
            {
                rearLights.Add(light);
            }
        }

        IMyMotorSuspension wheel =
            block as IMyMotorSuspension;

        if (wheel != null)
        {
            wheels.Add(wheel);
        }

        IMyShipController controller =
            block as IMyShipController;

        if (controller != null)
        {
            if (HasTag(
                name,
                data,
                cfg.DriverTag))
            {
                drivers.Add(controller);
            }
        }

        IMyThrust thruster =
            block as IMyThrust;

        if (thruster != null)
        {
            if (HasTag(
                name,
                data,
                cfg.BoostThrusterTag))
            {
                boostThrusters.Add(thruster);
            }
        }

        // ----------------------------------------------------
        // RAVEN LCD
        //
        // IMPORTANT:
        // The Programmable Block is allowed to use RAVEN_LCD
        // in its NAME, but its Custom Data is NOT searched.
        // This prevents the generated configuration from
        // accidentally making the PB itself an LCD.
        // ----------------------------------------------------

        int ravenIndex = 1;
        bool ravenFound = false;

        if (block == Me)
        {
            if (HasTagInName(
                name,
                cfg.RavenTag))
            {
                ravenFound = true;
            }
        }
        else
        {
            ravenFound =
                TryGetRavenIndex(
                    name,
                    data,
                    out ravenIndex);
        }

        if (ravenFound)
        {
            IMyTextSurfaceProvider provider =
                block as IMyTextSurfaceProvider;

            if (provider != null)
            {
                AddRavenDisplays(
                    block,
                    provider,
                    ravenIndex);
            }
        }
    }
}


// ============================================================
// BUILD VALID GRID LIST
// ============================================================

void BuildValidGridList()
{
    validGridIds.Clear();

    validGridIds.Add(
        Me.CubeGrid.EntityId);

    if (!cfg.DetectSubgrids)
        return;

    bool foundNewGrid = true;

    while (foundNewGrid)
    {
        foundNewGrid = false;

        List<IMyTerminalBlock> blocks =
            new List<IMyTerminalBlock>();

        GridTerminalSystem.GetBlocks(blocks);

        foreach (IMyTerminalBlock block in blocks)
        {
            if (!validGridIds.Contains(
                block.CubeGrid.EntityId))
            {
                continue;
            }

            IMyMechanicalConnectionBlock mechanical =
                block as IMyMechanicalConnectionBlock;

            if (mechanical == null)
                continue;

            IMyCubeGrid topGrid =
                mechanical.TopGrid;

            if (topGrid == null)
                continue;

            if (validGridIds.Contains(
                topGrid.EntityId))
            {
                continue;
            }

            validGridIds.Add(
                topGrid.EntityId);

            foundNewGrid = true;
        }
    }
}


// ============================================================
// GRID CHECK
// ============================================================

bool IsValidGrid(
    IMyTerminalBlock block)
{
    if (block == null)
        return false;

    return validGridIds.Contains(
        block.CubeGrid.EntityId);
}


// ============================================================
// TAG CHECK
// ============================================================

bool HasTag(
    string name,
    string customData,
    string tag)
{
    if (string.IsNullOrWhiteSpace(tag))
        return false;

    if (HasTagInName(name, tag))
        return true;

    if (!string.IsNullOrWhiteSpace(customData))
    {
        if (customData.IndexOf(
            tag,
            StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }
    }

    return false;
}


bool HasTagInName(
    string name,
    string tag)
{
    if (string.IsNullOrWhiteSpace(name))
        return false;

    if (string.IsNullOrWhiteSpace(tag))
        return false;

    return name.IndexOf(
        tag,
        StringComparison.OrdinalIgnoreCase) >= 0;
}


// ============================================================
// RAVEN TAG PARSER
// ============================================================

bool TryGetRavenIndex(
    string name,
    string customData,
    out int index)
{
    index = 1;

    string tag =
        cfg.RavenTag;

    // Check the block name first.
    if (TryFindRavenIndex(
        name,
        tag,
        out index))
    {
        return true;
    }

    // Then check Custom Data.
    if (TryFindRavenIndex(
        customData,
        tag,
        out index))
    {
        return true;
    }

    return false;
}


bool TryFindRavenIndex(
    string source,
    string tag,
    out int index)
{
    index = 1;

    if (string.IsNullOrWhiteSpace(source))
        return false;

    if (string.IsNullOrWhiteSpace(tag))
        return false;

    int position =
        source.IndexOf(
            tag,
            StringComparison.OrdinalIgnoreCase);

    if (position < 0)
        return false;

    int start =
        position + tag.Length;

    if (start >= source.Length)
    {
        index = 1;
        return true;
    }

    if (source[start] != ':')
    {
        index = 1;
        return true;
    }

    string number =
        source.Substring(
            start + 1)
        .Trim();

    int parsed;

    if (int.TryParse(
        number,
        out parsed))
    {
        if (parsed < 1)
            parsed = 1;

        index = parsed;

        return true;
    }

    index = 1;

    return true;
}


// ============================================================
// ADD RAVEN DISPLAY
// ============================================================

void AddRavenDisplays(
    IMyTerminalBlock block,
    IMyTextSurfaceProvider provider,
    int index)
{
    int surfaceIndex =
        index - 1;

    if (surfaceIndex < 0)
        surfaceIndex = 0;

    if (surfaceIndex >= provider.SurfaceCount)
        return;

    IMyTextSurface surface =
        provider.GetSurface(
            surfaceIndex);

    if (surface == null)
        return;

    RavenDisplay display =
        new RavenDisplay();

    display.Block = block;
    display.Surface = surface;
    display.Index = index;

    ravenDisplays.Add(display);
}


// ============================================================
// ACTIVE DRIVER
// ============================================================

void DiscoverActiveDriver()
{
    if (activeDriver != null)
    {
        if (activeDriver.IsUnderControl &&
            activeDriver.IsFunctional)
        {
            driverGridId =
                activeDriver.CubeGrid.EntityId;

            return;
        }
    }

    activeDriver = null;
    driverGridId = 0;

    foreach (
        IMyShipController controller
        in drivers)
    {
        if (!controller.IsFunctional)
            continue;

        if (!controller.IsUnderControl)
            continue;

        activeDriver =
            controller;

        driverGridId =
            controller.CubeGrid.EntityId;

        break;
    }
}


// ============================================================
// LIGHTING
// ============================================================

void UpdateLighting()
{
    Vector3 movement =
        activeDriver.MoveIndicator;

    Vector3D velocity =
        activeDriver
            .GetShipVelocities()
            .LinearVelocity;

    double forwardSpeed =
        Vector3D.Dot(
            velocity,
            activeDriver.WorldMatrix.Forward);

    bool headlightsOn =
        AreDriverGridHeadlightsOn();

    UpdateHeadlights(
        headlightsOn);

    turningLeft =
        movement.X < -0.1f;

    turningRight =
        movement.X > 0.1f;

    bool spaceBrake =
        activeDriver.HandBrake;

    bool pressingS =
        movement.Z > 0.1f;

    bool movingForwards =
        forwardSpeed >
        cfg.BrakeSpeedThreshold;

    bool movingBackwards =
        forwardSpeed <
        -cfg.ReverseSpeedThreshold;

    bool brakingFromS =
        pressingS &&
        movingForwards;

    braking =
        spaceBrake ||
        brakingFromS;

    if (movingBackwards)
    {
        reversing = true;
    }
    else if (!pressingS)
    {
        reversing = false;
    }

    UpdateRearLights(
        headlightsOn,
        braking,
        reversing);

    SetIndicators(
        turningLeft,
        turningRight);
}


// ============================================================
// HEADLIGHTS
// ============================================================
//
// Headlights are NOT switched on or off by RAVEN.
//
// RAVEN only reads whether headlights on the driver's local
// grid are enabled, then applies that state to headlights on
// other valid mechanically connected subgrids.
//
// RAVEN does control their colour/settings.
//
// ============================================================

void UpdateHeadlights(
    bool headlightsOn)
{
    foreach (
        IMyLightingBlock light
        in headlights)
    {
        light.Color =
            cfg.HeadlightColor;

        light.Intensity =
            cfg.HeadlightIntensity;

        light.Radius =
            float.MaxValue;

        light.Falloff =
            cfg.HeadlightFalloff;

        if (light.CubeGrid.EntityId !=
            driverGridId)
        {
            light.Enabled =
                headlightsOn;
        }
    }
}


// ============================================================
// DRIVER GRID HEADLIGHT STATE
// ============================================================

bool AreDriverGridHeadlightsOn()
{
    if (activeDriver == null)
        return false;

    foreach (
        IMyLightingBlock light
        in headlights)
    {
        if (light.CubeGrid.EntityId !=
            driverGridId)
        {
            continue;
        }

        if (light.Enabled)
            return true;
    }

    return false;
}


// ============================================================
// REAR LIGHTS
// ============================================================

void UpdateRearLights(
    bool headlightsOn,
    bool isBraking,
    bool isReversing)
{
    foreach (
        IMyLightingBlock light
        in rearLights)
    {
        if (isReversing)
        {
            light.Enabled = true;

            light.Color =
                cfg.ReverseColor;

            light.Intensity =
                cfg.ReverseIntensity;

            light.Radius =
                cfg.ReverseRange;

            light.Falloff =
                cfg.ReverseFalloff;

            continue;
        }

        if (isBraking)
        {
            light.Enabled = true;

            light.Color =
                cfg.BrakeColor;

            light.Intensity =
                cfg.BrakeIntensity;

            light.Radius =
                cfg.BrakeRange;

            light.Falloff =
                cfg.BrakeFalloff;

            continue;
        }

        if (headlightsOn)
        {
            light.Enabled = true;

            light.Color =
                cfg.TailColor;

            light.Intensity =
                cfg.TailIntensity;

            light.Radius =
                cfg.TailRange;

            light.Falloff =
                cfg.TailFalloff;
        }
        else
        {
            light.Enabled = false;
        }
    }
}


// ============================================================
// INDICATORS
// ============================================================

void SetIndicators(
    bool left,
    bool right)
{
    bool leftOn =
        left &&
        indicatorState;

    bool rightOn =
        right &&
        indicatorState;

    foreach (
        IMyLightingBlock light
        in leftIndicators)
    {
        light.Enabled =
            leftOn;

        light.Color =
            cfg.IndicatorColor;

        light.Intensity =
            cfg.IndicatorIntensity;

        light.Radius =
            cfg.IndicatorRange;

        light.Falloff =
            cfg.IndicatorFalloff;
    }

    foreach (
        IMyLightingBlock light
        in rightIndicators)
    {
        light.Enabled =
            rightOn;

        light.Color =
            cfg.IndicatorColor;

        light.Intensity =
            cfg.IndicatorIntensity;

        light.Radius =
            cfg.IndicatorRange;

        light.Falloff =
            cfg.IndicatorFalloff;
    }
}


// ============================================================
// BOOST THRUSTERS
// ============================================================

void UpdateBoostThrusters()
{
    SetBoostThrusters(
        boostEnabled);
}


void SetBoostThrusters(
    bool enabled)
{
    foreach (
        IMyThrust thruster
        in boostThrusters)
    {
        if (!thruster.IsFunctional)
            continue;

        thruster.Enabled =
            enabled;
    }
}


// ============================================================
// WHEEL CONTROL
// ============================================================

void UpdateWheels()
{
    if (!cfg.AutoWheelControl)
        return;

    if (wheels.Count == 0)
        return;

    Vector3D velocity =
        activeDriver
            .GetShipVelocities()
            .LinearVelocity;

    double speed =
        activeDriver.GetShipSpeed();

    double steeringFactor =
        Clamp(
            (
                speed -
                cfg.SpeedForMinimumSteering
            )
            /
            (
                cfg.SpeedForMaximumSteering -
                cfg.SpeedForMinimumSteering
            ),
            0,
            1);

    steeringFactor =
        steeringFactor *
        steeringFactor;

    double steeringAngle =
        Lerp(
            cfg.MaxSteeringAngle,
            cfg.MinSteeringAngle,
            steeringFactor);

    float steeringRadians =
        (float)(
            steeringAngle *
            Math.PI /
            180.0);

    MyShipMass mass =
        activeDriver.CalculateShipMass();

    double massPerWheel =
        mass.PhysicalMass /
        Math.Max(
            1,
            wheels.Count);

    double massFactor =
        Clamp(
            massPerWheel /
            Math.Max(
                1,
                cfg.TargetMassPerWheel),
            0.5,
            2.0);

    double strength =
        Lerp(
            cfg.MinStrength,
            cfg.MaxStrength,
            Clamp(
                massFactor,
                0,
                1));

    double lateralSpeed =
        Math.Abs(
            Vector3D.Dot(
                velocity,
                activeDriver.WorldMatrix.Right));

    double lateralFactor =
        Clamp(
            lateralSpeed /
            10.0,
            0,
            1);

    double friction =
        Lerp(
            cfg.MaxFriction,
            cfg.MinFriction,
            Clamp(
                lateralFactor *
                cfg.LateralGripMultiplier,
                0,
                1));

    double powerFactor =
        Clamp(
            1.0 -
            (
                speed /
                Math.Max(
                    1,
                    cfg.NormalSpeedLimit)
            ),
            0,
            1);

    double power =
        Lerp(
            cfg.MinPower,
            cfg.MaxPower,
            powerFactor);

    float speedLimitMS =
        boostEnabled
        ? cfg.BoostSpeedLimit
        : cfg.NormalSpeedLimit;

    float speedLimitKmh =
        speedLimitMS *
        3.6f;

    foreach (
        IMyMotorSuspension wheel
        in wheels)
    {
        if (!wheel.IsFunctional)
            continue;

        wheel.MaxSteerAngle =
            steeringRadians;

        wheel.Strength =
            (float)Clamp(
                strength,
                0,
                100);

        wheel.Friction =
            (float)Clamp(
                friction,
                0,
                100);

        wheel.Power =
            (float)Clamp(
                power,
                0,
                100);

        wheel.SetValueFloat(
            "Speed Limit",
            speedLimitKmh);
    }
}


// ============================================================
// RAVEN DISPLAY
// ============================================================

void UpdateRavenDisplays()
{
    if (ravenDisplays.Count == 0)
        return;

    double speed =
        activeDriver == null
        ? 0
        : activeDriver.GetShipSpeed();

    double speedKmh =
        speed *
        3.6;

    double speedLimit =
        boostEnabled
        ? cfg.BoostSpeedLimit
        : cfg.NormalSpeedLimit;

    foreach (
        RavenDisplay display
        in ravenDisplays)
    {
        DrawRaven(
            display.Surface,
            speed,
            speedKmh,
            speedLimit);
    }
}


// ============================================================
// DRAW RAVEN
// ============================================================

void DrawRaven(
    IMyTextSurface surface,
    double speed,
    double speedKmh,
    double speedLimit)
{
    surface.ContentType =
        ContentType.TEXT_AND_IMAGE;

    surface.Font =
        "Monospace";

    surface.Alignment =
        TextAlignment.CENTER;

    surface.TextPadding =
        2f;

    float width =
        surface.SurfaceSize.X;

    float height =
        surface.SurfaceSize.Y;

    // --------------------------------------------------------
    // Use the HEIGHT to determine the font size.
    //
    // This prevents the display from becoming zoomed in and
    // cutting off the bottom of the interface.
    // --------------------------------------------------------

    float fontSize =
        height /
        420f;

    fontSize =
        Math.Max(
            0.45f,
            Math.Min(
                1.2f,
                fontSize));

    surface.FontSize =
        fontSize;

    // --------------------------------------------------------
    // Calculate an approximate character capacity so the
    // title can expand across the entire LCD.
    // --------------------------------------------------------

    int characterWidth =
        (int)(
            width /
            (
                18f *
                fontSize
            ));

    if (characterWidth < 10)
        characterWidth = 10;

    if (characterWidth > 80)
        characterWidth = 80;

    string title =
        BuildTitle(
            characterWidth);

    StringBuilder text =
        new StringBuilder();

    text.AppendLine(title);

    text.AppendLine();

    text.AppendLine(
        "SPEED");

    text.AppendLine(
        speed.ToString("0.0") +
        " m/s");

    text.AppendLine(
        speedKmh.ToString("0") +
        " km/h");

    text.AppendLine();

    text.AppendLine(
        "MODE");

    text.AppendLine(
        boostEnabled
        ? "BOOST"
        : "NORMAL");

    text.AppendLine();

    text.AppendLine(
        "LIMIT");

    text.AppendLine(
        speedLimit.ToString("0.0") +
        " m/s");

    text.AppendLine();

    text.AppendLine(
        "HEADLIGHTS   " +
        (
            AreDriverGridHeadlightsOn()
            ? "ON"
            : "OFF"
        ));

    text.AppendLine();

    text.AppendLine(
        "BRAKE        " +
        (
            braking
            ? "ON"
            : "OFF"
        ));

    text.AppendLine();

    text.AppendLine(
        "REVERSE      " +
        (
            reversing
            ? "ON"
            : "OFF"
        ));

    surface.WriteText(
        text.ToString());
}


// ============================================================
// BUILD LCD TITLE
// ============================================================

string BuildTitle(
    int width)
{
    string centre =
        " RAVEN ";

    if (width <= centre.Length + 2)
        return centre;

    int remaining =
        width -
        centre.Length;

    int left =
        remaining / 2;

    int right =
        remaining -
        left;

    StringBuilder title =
        new StringBuilder();

    for (int i = 0; i < left; i++)
    {
        title.Append("=");
    }

    title.Append(centre);

    for (int i = 0; i < right; i++)
    {
        title.Append("=");
    }

    return title.ToString();
}


// ============================================================
// MATH
// ============================================================

double Clamp(
    double value,
    double min,
    double max)
{
    if (value < min)
        return min;

    if (value > max)
        return max;

    return value;
}


double Lerp(
    double a,
    double b,
    double t)
{
    return a +
        (b - a) *
        t;
}


// ============================================================
// CONFIGURATION GENERATION
// ============================================================

void EnsureConfiguration()
{
    string existing =
        Me.CustomData;

    StringBuilder output =
        new StringBuilder();

    output.AppendLine(
        "# ==================================================");

    output.AppendLine(
        "# RAVEN ROVER CONTROL SYSTEM CONFIGURATION");

    output.AppendLine(
        "# Automatically generated. Change values as needed.");

    output.AppendLine(
        "# Recompile the Programmable Block after changes.");

    output.AppendLine(
        "# ==================================================");

    output.AppendLine();

    output.AppendLine(
        "[General]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "General",
        "DetectSubgrids",
        "true",
        "Include mechanically connected subgrids. Connector-only grids are excluded.");

    AddSetting(
        output,
        existing,
        "General",
        "UpdateFrequency",
        "10",
        "PB update frequency. 10 = every 10 simulation ticks.");

    output.AppendLine();

    output.AppendLine(
        "[Tags]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "Tags",
        "Headlight",
        "Headlight",
        "Tag used to identify headlights.");

    AddSetting(
        output,
        existing,
        "Tags",
        "LeftIndicator",
        "Left Indicator",
        "Tag used to identify left indicators.");

    AddSetting(
        output,
        existing,
        "Tags",
        "RightIndicator",
        "Right Indicator",
        "Tag used to identify right indicators.");

    AddSetting(
        output,
        existing,
        "Tags",
        "RearLight",
        "Rear Light",
        "Tag used to identify rear/brake/reverse lights.");

    AddSetting(
        output,
        existing,
        "Tags",
        "Driver",
        "Driver",
        "Tag used to identify cockpits and remote controls.");

    AddSetting(
        output,
        existing,
        "Tags",
        "BoostThruster",
        "Boost Thruster",
        "Tag used to identify boost thrusters.");

    AddSetting(
        output,
        existing,
        "Tags",
        "RAVEN",
        "RAVEN_LCD",
        "Tag used to identify RAVEN LCD displays. Optional :1, :2, :3 index may be used.");

    output.AppendLine();

    output.AppendLine(
        "[Headlights]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "Headlights",
        "Color",
        "255,245,220",
        "Headlight RGB colour.");

    AddSetting(
        output,
        existing,
        "Headlights",
        "Intensity",
        "5",
        "Headlight intensity.");

    AddSetting(
        output,
        existing,
        "Headlights",
        "Falloff",
        "1",
        "Headlight falloff.");

    output.AppendLine();

    output.AppendLine(
        "[TailLights]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "TailLights",
        "Color",
        "255,0,0",
        "Normal tail-light RGB colour.");

    AddSetting(
        output,
        existing,
        "TailLights",
        "Intensity",
        "1.25",
        "Normal tail-light intensity.");

    AddSetting(
        output,
        existing,
        "TailLights",
        "Range",
        "5",
        "Normal tail-light range.");

    AddSetting(
        output,
        existing,
        "TailLights",
        "Falloff",
        "1",
        "Normal tail-light falloff.");

    output.AppendLine();

    output.AppendLine(
        "[BrakeLights]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "BrakeLights",
        "Color",
        "255,0,0",
        "Brake-light RGB colour.");

    AddSetting(
        output,
        existing,
        "BrakeLights",
        "Intensity",
        "5",
        "Brake-light intensity.");

    AddSetting(
        output,
        existing,
        "BrakeLights",
        "Range",
        "5",
        "Brake-light range.");

    AddSetting(
        output,
        existing,
        "BrakeLights",
        "Falloff",
        "1",
        "Brake-light falloff.");

    output.AppendLine();

    output.AppendLine(
        "[ReverseLights]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "ReverseLights",
        "Color",
        "255,255,255",
        "Reverse-light RGB colour.");

    AddSetting(
        output,
        existing,
        "ReverseLights",
        "Intensity",
        "5",
        "Reverse-light intensity.");

    AddSetting(
        output,
        existing,
        "ReverseLights",
        "Range",
        "20",
        "Reverse-light range.");

    AddSetting(
        output,
        existing,
        "ReverseLights",
        "Falloff",
        "1",
        "Reverse-light falloff.");

    output.AppendLine();

    output.AppendLine(
        "[Indicators]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "Indicators",
        "Color",
        "255,160,0",
        "Indicator RGB colour.");

    AddSetting(
        output,
        existing,
        "Indicators",
        "Intensity",
        "5",
        "Indicator intensity.");

    AddSetting(
        output,
        existing,
        "Indicators",
        "Range",
        "1",
        "Indicator range.");

    AddSetting(
        output,
        existing,
        "Indicators",
        "Falloff",
        "0",
        "Indicator falloff.");

    AddSetting(
        output,
        existing,
        "Indicators",
        "BlinkSpeed",
        "0.20",
        "Indicator blink half-period in seconds.");

    output.AppendLine();

    output.AppendLine(
        "[Wheels]");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "Wheels",
        "AutoWheelControl",
        "true",
        "Enable automatic wheel management.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MaxSteeringAngle",
        "25",
        "Maximum steering angle in degrees.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MinSteeringAngle",
        "4",
        "Minimum steering angle at high speed.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MaxStrength",
        "100",
        "Maximum wheel suspension strength.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MinStrength",
        "5",
        "Minimum wheel suspension strength.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MaxFriction",
        "100",
        "Maximum wheel friction.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MinFriction",
        "20",
        "Minimum wheel friction.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MaxPower",
        "60",
        "Maximum wheel propulsion power.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "MinPower",
        "5",
        "Minimum wheel propulsion power.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "SpeedForMinimumSteering",
        "10",
        "Speed at which steering starts reducing.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "SpeedForMaximumSteering",
        "25",
        "Speed at which minimum steering is reached.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "TargetMassPerWheel",
        "2500",
        "Target physical mass per wheel in kg.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "LateralGripMultiplier",
        "1.0",
        "Multiplier applied to lateral grip response.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "BrakeSpeedThreshold",
        "1.0",
        "Forward speed required before S counts as braking.");

    AddSetting(
        output,
        existing,
        "Wheels",
        "ReverseSpeedThreshold",
        "1.0",
        "Speed threshold used to identify reversing.");

    output.AppendLine();

    output.AppendLine(
        "[Speed]");

    output.AppendLine(
        "# Speed values are in metres per second.");

    output.AppendLine();

    AddSetting(
        output,
        existing,
        "Speed",
        "NormalSpeedLimit",
        "30",
        "Normal rover speed limit in m/s.");

    AddSetting(
        output,
        existing,
        "Speed",
        "BoostSpeedLimit",
        "100",
        "Boost rover speed limit in m/s.");

    output.AppendLine();

    Me.CustomData =
        output.ToString();
}


// ============================================================
// CONFIGURATION HELPERS
// ============================================================

void AddSetting(
    StringBuilder output,
    string existing,
    string section,
    string key,
    string defaultValue,
    string comment)
{
    string value =
        GetExistingValue(
            existing,
            section,
            key);

    if (value == null)
        value = defaultValue;

    output.AppendLine(
        "# " + comment);

    output.AppendLine(
        key + "=" + value);
}


string GetExistingValue(
    string data,
    string section,
    string key)
{
    if (string.IsNullOrWhiteSpace(data))
        return null;

    string[] lines =
        data
            .Replace("\r", "")
            .Split('\n');

    bool inSection = false;

    foreach (
        string rawLine
        in lines)
    {
        string line =
            rawLine.Trim();

        if (line.StartsWith("#"))
            continue;

        if (line.StartsWith("[") &&
            line.EndsWith("]"))
        {
            string currentSection =
                line.Substring(
                    1,
                    line.Length - 2);

            inSection =
                currentSection.Equals(
                    section,
                    StringComparison.OrdinalIgnoreCase);

            continue;
        }

        if (!inSection)
            continue;

        int equals =
            line.IndexOf('=');

        if (equals < 0)
            continue;

        string currentKey =
            line.Substring(
                0,
                equals)
            .Trim();

        if (!currentKey.Equals(
            key,
            StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        return line.Substring(
            equals + 1)
            .Trim();
    }

    return null;
}


// ============================================================
// VALUE PARSERS
// ============================================================

bool GetBool(
    string section,
    string key,
    bool fallback)
{
    string value =
        GetExistingValue(
            Me.CustomData,
            section,
            key);

    bool result;

    if (value != null &&
        bool.TryParse(
            value,
            out result))
    {
        return result;
    }

    return fallback;
}


int GetInt(
    string section,
    string key,
    int fallback)
{
    string value =
        GetExistingValue(
            Me.CustomData,
            section,
            key);

    int result;

    if (value != null &&
        int.TryParse(
            value,
            out result))
    {
        return result;
    }

    return fallback;
}


float GetFloat(
    string section,
    string key,
    float fallback)
{
    string value =
        GetExistingValue(
            Me.CustomData,
            section,
            key);

    float result;

    if (value != null &&
        float.TryParse(
            value,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture,
            out result))
    {
        return result;
    }

    return fallback;
}


string GetString(
    string section,
    string key,
    string fallback)
{
    string value =
        GetExistingValue(
            Me.CustomData,
            section,
            key);

    if (value == null)
        return fallback;

    return value;
}


Color GetColor(
    string section,
    string key,
    Color fallback)
{
    string value =
        GetExistingValue(
            Me.CustomData,
            section,
            key);

    if (string.IsNullOrWhiteSpace(value))
        return fallback;

    string[] parts =
        value.Split(',');

    if (parts.Length != 3)
        return fallback;

    int r;
    int g;
    int b;

    if (!int.TryParse(
        parts[0].Trim(),
        out r))
        return fallback;

    if (!int.TryParse(
        parts[1].Trim(),
        out g))
        return fallback;

    if (!int.TryParse(
        parts[2].Trim(),
        out b))
        return fallback;

    return new Color(
        (int)Clamp(r, 0, 255),
        (int)Clamp(g, 0, 255),
        (int)Clamp(b, 0, 255));
}


// ============================================================
// LOAD CONFIGURATION
// ============================================================

void LoadConfiguration()
{
    cfg.DetectSubgrids =
        GetBool(
            "General",
            "DetectSubgrids",
            cfg.DetectSubgrids);

    cfg.UpdateFrequency =
        GetInt(
            "General",
            "UpdateFrequency",
            cfg.UpdateFrequency);

    cfg.HeadlightTag =
        GetString(
            "Tags",
            "Headlight",
            cfg.HeadlightTag);

    cfg.LeftIndicatorTag =
        GetString(
            "Tags",
            "LeftIndicator",
            cfg.LeftIndicatorTag);

    cfg.RightIndicatorTag =
        GetString(
            "Tags",
            "RightIndicator",
            cfg.RightIndicatorTag);

    cfg.RearLightTag =
        GetString(
            "Tags",
            "RearLight",
            cfg.RearLightTag);

    cfg.DriverTag =
        GetString(
            "Tags",
            "Driver",
            cfg.DriverTag);

    cfg.BoostThrusterTag =
        GetString(
            "Tags",
            "BoostThruster",
            cfg.BoostThrusterTag);

    cfg.RavenTag =
        GetString(
            "Tags",
            "RAVEN",
            cfg.RavenTag);

    cfg.HeadlightColor =
        GetColor(
            "Headlights",
            "Color",
            cfg.HeadlightColor);

    cfg.HeadlightIntensity =
        GetFloat(
            "Headlights",
            "Intensity",
            cfg.HeadlightIntensity);

    cfg.HeadlightFalloff =
        GetFloat(
            "Headlights",
            "Falloff",
            cfg.HeadlightFalloff);

    cfg.TailColor =
        GetColor(
            "TailLights",
            "Color",
            cfg.TailColor);

    cfg.TailIntensity =
        GetFloat(
            "TailLights",
            "Intensity",
            cfg.TailIntensity);

    cfg.TailRange =
        GetFloat(
            "TailLights",
            "Range",
            cfg.TailRange);

    cfg.TailFalloff =
        GetFloat(
            "TailLights",
            "Falloff",
            cfg.TailFalloff);

    cfg.BrakeColor =
        GetColor(
            "BrakeLights",
            "Color",
            cfg.BrakeColor);

    cfg.BrakeIntensity =
        GetFloat(
            "BrakeLights",
            "Intensity",
            cfg.BrakeIntensity);

    cfg.BrakeRange =
        GetFloat(
            "BrakeLights",
            "Range",
            cfg.BrakeRange);

    cfg.BrakeFalloff =
        GetFloat(
            "BrakeLights",
            "Falloff",
            cfg.BrakeFalloff);

    cfg.ReverseColor =
        GetColor(
            "ReverseLights",
            "Color",
            cfg.ReverseColor);

    cfg.ReverseIntensity =
        GetFloat(
            "ReverseLights",
            "Intensity",
            cfg.ReverseIntensity);

    cfg.ReverseRange =
        GetFloat(
            "ReverseLights",
            "Range",
            cfg.ReverseRange);

    cfg.ReverseFalloff =
        GetFloat(
            "ReverseLights",
            "Falloff",
            cfg.ReverseFalloff);

    cfg.IndicatorColor =
        GetColor(
            "Indicators",
            "Color",
            cfg.IndicatorColor);

    cfg.IndicatorIntensity =
        GetFloat(
            "Indicators",
            "Intensity",
            cfg.IndicatorIntensity);

    cfg.IndicatorRange =
        GetFloat(
            "Indicators",
            "Range",
            cfg.IndicatorRange);

    cfg.IndicatorFalloff =
        GetFloat(
            "Indicators",
            "Falloff",
            cfg.IndicatorFalloff);

    cfg.IndicatorBlinkSpeed =
        Math.Max(
            0.1f,
            GetFloat(
                "Indicators",
                "BlinkSpeed",
                cfg.IndicatorBlinkSpeed));

    cfg.AutoWheelControl =
        GetBool(
            "Wheels",
            "AutoWheelControl",
            cfg.AutoWheelControl);

    cfg.MaxSteeringAngle =
        GetFloat(
            "Wheels",
            "MaxSteeringAngle",
            cfg.MaxSteeringAngle);

    cfg.MinSteeringAngle =
        GetFloat(
            "Wheels",
            "MinSteeringAngle",
            cfg.MinSteeringAngle);

    cfg.MaxStrength =
        GetFloat(
            "Wheels",
            "MaxStrength",
            cfg.MaxStrength);

    cfg.MinStrength =
        GetFloat(
            "Wheels",
            "MinStrength",
            cfg.MinStrength);

    cfg.MaxFriction =
        GetFloat(
            "Wheels",
            "MaxFriction",
            cfg.MaxFriction);

    cfg.MinFriction =
        GetFloat(
            "Wheels",
            "MinFriction",
            cfg.MinFriction);

    cfg.MaxPower =
        GetFloat(
            "Wheels",
            "MaxPower",
            cfg.MaxPower);

    cfg.MinPower =
        GetFloat(
            "Wheels",
            "MinPower",
            cfg.MinPower);

    cfg.SpeedForMinimumSteering =
        GetFloat(
            "Wheels",
            "SpeedForMinimumSteering",
            cfg.SpeedForMinimumSteering);

    cfg.SpeedForMaximumSteering =
        GetFloat(
            "Wheels",
            "SpeedForMaximumSteering",
            cfg.SpeedForMaximumSteering);

    cfg.TargetMassPerWheel =
        GetFloat(
            "Wheels",
            "TargetMassPerWheel",
            cfg.TargetMassPerWheel);

    cfg.LateralGripMultiplier =
        GetFloat(
            "Wheels",
            "LateralGripMultiplier",
            cfg.LateralGripMultiplier);

    cfg.BrakeSpeedThreshold =
        GetFloat(
            "Wheels",
            "BrakeSpeedThreshold",
            cfg.BrakeSpeedThreshold);

    cfg.ReverseSpeedThreshold =
        GetFloat(
            "Wheels",
            "ReverseSpeedThreshold",
            cfg.ReverseSpeedThreshold);

    cfg.NormalSpeedLimit =
        GetFloat(
            "Speed",
            "NormalSpeedLimit",
            cfg.NormalSpeedLimit);

    cfg.BoostSpeedLimit =
        GetFloat(
            "Speed",
            "BoostSpeedLimit",
            cfg.BoostSpeedLimit);
}


// ============================================================
// STATUS
// ============================================================

void EchoStatus()
{
    float selectedSpeed =
        boostEnabled
        ? cfg.BoostSpeedLimit
        : cfg.NormalSpeedLimit;

    Echo(
        "RAVEN ROVER CONTROL SYSTEM\n" +
        "---------------------------\n" +
        "Version: " +
        VERSION +
        "\n\n" +

        "Driver: " +
        (
            activeDriver == null
            ? "NONE"
            : activeDriver.CustomName
        ) +
        "\n\n" +

        "Boost: " +
        (
            boostEnabled
            ? "ACTIVE"
            : "OFF"
        ) +
        "\n" +

        "Speed Limit: " +
        selectedSpeed.ToString("0.0") +
        " m/s\n\n" +

        "Headlights: " +
        (
            AreDriverGridHeadlightsOn()
            ? "ON"
            : "OFF"
        ) +
        "\n" +

        "Braking: " +
        (
            braking
            ? "YES"
            : "NO"
        ) +
        "\n" +

        "Reversing: " +
        (
            reversing
            ? "YES"
            : "NO"
        ) +
        "\n\n" +

        "Headlights: " +
        headlights.Count +
        "\n" +

        "Rear Lights: " +
        rearLights.Count +
        "\n" +

        "Left Indicators: " +
        leftIndicators.Count +
        "\n" +

        "Right Indicators: " +
        rightIndicators.Count +
        "\n\n" +

        "Wheels: " +
        wheels.Count +
        "\n" +

        "Boost Thrusters: " +
        boostThrusters.Count +
        "\n" +

        "RAVEN Displays: " +
        ravenDisplays.Count +
        "\n" +

        "Drivers: " +
        drivers.Count
    );
}