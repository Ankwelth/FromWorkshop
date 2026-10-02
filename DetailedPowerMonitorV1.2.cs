/*
    Construction Power Monitor
    Version: V1.2
*/

//==================================================
// USER SETTINGS
//==================================================
// Name of the cockpit/control seat to use.
// Change this if your cockpit has a different name.
string cockpitName = "Power Seat";

// Cockpit LCD screen index.
// 0 = Left screen
// 1 = Centre screen
// 2 = Right screen
int cockpitScreenIndex = 0;

// Name of the standalone LCD panel.
// Only used if no cockpit screen is found.
string lcdName = "Power LCD";

// Behaviour
bool autoReturnOverview = true;
int overviewTimeoutSeconds = 10;

//==================================================
// VERSION INFORMATION
//==================================================

const string version = "V1.2";


//==================================================
// CONSTRUCTION OS MODULE INFORMATION
//==================================================

const string moduleId = "construction.power.monitor";
const string moduleName = "Construction Power Monitor";
const string moduleVersion = "V1.2";
const string moduleAuthor = "IDecX";
const string moduleCommands =
    "overview,battery,generation,hydrogen,next,prev";

const string moduleDataStart =
    "; ===== CONSTRUCTION OS MODULE START =====";

const string moduleDataEnd =
    "; ===== CONSTRUCTION OS MODULE END =====";



//==================================================
// INTERNAL VARIABLES
//==================================================

string currentPage = "overview";
double secondsSincePageInput = 0;

string[] pages =
{
    "overview",
    "battery",
    "generation",
    "hydrogen"
};

public Program()
{
    RegisterConstructionOSModule();
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    UpdateOverviewTimeout(updateSource);

    argument = argument.ToLower();

    if(argument == "next")
    {
        NextPage();
        ResetOverviewTimeout();
    }
    else if(argument == "prev")
    {
        PrevPage();
        ResetOverviewTimeout();
    }
    else if(IsPage(argument))
    {
        currentPage = argument;
        ResetOverviewTimeout();
    }

    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType(
        batteries,
        battery => battery.CubeGrid == Me.CubeGrid
    );

    List<IMyReactor> reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(
    reactors,
    reactor => reactor.CubeGrid == Me.CubeGrid
	);

    List<IMySolarPanel> panels = new List<IMySolarPanel>();
    GridTerminalSystem.GetBlocksOfType(
        panels,
        panel => panel.CubeGrid == Me.CubeGrid
    );

    List<IMyGasTank> hydrogenTanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(
        hydrogenTanks,
        tank =>
            tank.CubeGrid == Me.CubeGrid &&
            tank.BlockDefinition.SubtypeName.ToLower().Contains("hydrogen")
    );

    List<IMyPowerProducer> hydEngines = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType(
        hydEngines,
        engine =>
            engine.CubeGrid == Me.CubeGrid &&
            engine.BlockDefinition.SubtypeName.ToLower().Contains("hydrogenengine")
    );
	
	int damagedBatteries = 0;
	int damagedReactors = 0;
	int damagedPanels = 0;
	int damagedHydrogenTanks = 0;
	int damagedHydrogenEngines = 0;

    double hydrogenPercent = 0;

    foreach(var tank in hydrogenTanks)
    {
        hydrogenPercent += tank.FilledRatio;
    }

    if(hydrogenTanks.Count > 0)
    {
        hydrogenPercent = hydrogenPercent / hydrogenTanks.Count * 100;
    }

    double reactorOutput = 0;
    foreach(var reactor in reactors)
    {
        if(!reactor.IsFunctional)
        {
            damagedReactors++;
        }

        reactorOutput += reactor.CurrentOutput;
    }

    double solarOutput = 0;
    foreach(var panel in panels)
    {
        if(!panel.IsFunctional)
        {
            damagedPanels++;
        }

        solarOutput += panel.CurrentOutput;
    }

    foreach(var tank in hydrogenTanks)
    {
        if(!tank.IsFunctional)
        {
            damagedHydrogenTanks++;
        }
    }

    double engineOutput = 0;
    foreach(var engine in hydEngines)
    {
        if(!engine.IsFunctional)
        {
            damagedHydrogenEngines++;
        }

        engineOutput += engine.CurrentOutput;
    }

    double storedPower = 0;
    double maxPower = 0;
    double input = 0;
    double output = 0;
    double maxOutput = 0;

    int rechargeMode = 0;
    int autoMode = 0;

    foreach(var battery in batteries)
    {
        if(!battery.IsFunctional)
        {
            damagedBatteries++;
        }

        storedPower += battery.CurrentStoredPower;
        maxPower += battery.MaxStoredPower;
        input += battery.CurrentInput;
        output += battery.CurrentOutput;
        maxOutput += battery.MaxOutput;

        if(battery.ChargeMode == ChargeMode.Recharge)
        {
            rechargeMode++;
        }

        if(battery.ChargeMode == ChargeMode.Auto)
        {
            autoMode++;
        }
    }

    double chargePercent = 0;

    if(maxPower > 0)
    {
        chargePercent = storedPower / maxPower * 100;
    }

    string status = GetBatteryStatus(
        batteries.Count,
        chargePercent,
        input,
        output
    );

    string batteryMode = GetBatteryMode(
        batteries.Count,
        autoMode,
        rechargeMode
    );

    string batteryTime = GetBatteryTimeRemaining(
        batteries.Count,
        storedPower,
        maxPower,
        input,
        output
    );

    string healthStatus = GetHealthStatus(
        damagedBatteries,
        damagedHydrogenTanks,
        damagedHydrogenEngines,
        damagedReactors,
        damagedPanels
    );

    string batteryHealthStatus =
        GetBatteryHealthStatus(damagedBatteries);

    string generationHealthStatus =
        GetGenerationHealthStatus(
            damagedReactors,
            damagedPanels,
            damagedHydrogenEngines
        );

    string hydrogenHealthStatus =
        GetHydrogenHealthStatus(
            damagedHydrogenTanks,
            damagedHydrogenEngines
        );

    string screen = "";

    if(currentPage == "overview")
    {
        screen = BuildOverviewScreen(
            batteries.Count,
            chargePercent,
            hydrogenTanks.Count,
            hydrogenPercent,
            input,
            output,
            status,
            batteryTime,
            healthStatus
        );
    }
    else if(currentPage == "battery")
    {
        screen = BuildBatteryScreen(
            batteries.Count,
            chargePercent,
            storedPower,
            maxPower,
            input,
            output,
            maxOutput,
            status,
            batteryMode,
            batteryHealthStatus
        );
    }
    else if(currentPage == "generation")
    {
        screen = BuildGenerationScreen(
            reactors.Count,
            reactorOutput,
            panels.Count,
            solarOutput,
            hydEngines.Count,
            engineOutput,
            generationHealthStatus
        );
    }
    else if(currentPage == "hydrogen")
    {
        screen = BuildHydrogenScreen(
            hydrogenTanks.Count,
            hydrogenPercent,
            hydEngines.Count,
            engineOutput,
            hydrogenHealthStatus
        );
    }

    bool written = WriteToBestDisplay(screen);

    Echo("POWER MONITOR " + version);
    Echo("Page: " + currentPage);
    Echo("Status: " + status);
    Echo("Charge: " + Math.Round(chargePercent, 1) + "%");
    Echo("H2 Tanks : " + hydrogenTanks.Count);
    Echo("H2 Fill  : " + Math.Round(hydrogenPercent, 1) + "%");

    if(!written)
    {
        Echo("NO LCD OR COCKPIT SCREEN FOUND");
    }
}

void UpdateOverviewTimeout(UpdateType updateSource)
{
    if(!autoReturnOverview ||
       overviewTimeoutSeconds <= 0 ||
       currentPage == "overview")
    {
        secondsSincePageInput = 0;
        return;
    }

    if((updateSource & UpdateType.Update100) != 0)
    {
        secondsSincePageInput += 100.0 / 60.0;
    }

    if(secondsSincePageInput >= overviewTimeoutSeconds)
    {
        currentPage = "overview";
        secondsSincePageInput = 0;
    }
}

void ResetOverviewTimeout()
{
    secondsSincePageInput = 0;
}

bool IsPage(string page)
{
    return Array.IndexOf(pages, page) >= 0;
}

string GetBatteryStatus(
    int batteryCount,
    double chargePercent,
    double input,
    double output)
{
    if(batteryCount == 0)
    {
        return "NO BATTERIES";
    }

    if(chargePercent <= 10)
    {
        return "CRITICAL BATTERY";
    }

    if(chargePercent <= 25)
    {
        return "LOW BATTERY";
    }

    if(input > output)
    {
        return "CHARGING";
    }

    if(output > input)
    {
        return "DISCHARGING";
    }

    if(chargePercent >= 99)
    {
        return "FULLY CHARGED";
    }

    return "";
}

string GetHealthStatus(
    int damagedBatteries,
    int damagedHydrogenTanks,
    int damagedHydrogenEngines,
    int damagedReactors,
    int damagedPanels)
{
    List<string> faultAreas = new List<string>();

    if(damagedBatteries > 0)
    {
        faultAreas.Add("BATTERY");
    }

    if(damagedHydrogenTanks > 0 ||
       damagedHydrogenEngines > 0)
    {
        faultAreas.Add("H2");
    }

    if(damagedReactors > 0 ||
       damagedPanels > 0 ||
       damagedHydrogenEngines > 0)
    {
        faultAreas.Add("GENERATION");
    }

    if(faultAreas.Count == 0)
    {
        return "OKAY";
    }

    return "FAULT IN " + string.Join(" + ", faultAreas);
}

string GetBatteryHealthStatus(int damagedBatteries)
{
    if(damagedBatteries == 0)
    {
        return "ALL BATTERIES OK";
    }

    if(damagedBatteries == 1)
    {
        return "1 BATTERY DAMAGED";
    }

    return damagedBatteries + " BATTERIES DAMAGED";
}

string GetGenerationHealthStatus(
    int damagedReactors,
    int damagedPanels,
    int damagedHydrogenEngines)
{
    List<string> faults = new List<string>();

    if(damagedReactors > 0)
    {
        faults.Add("REACTOR:" + damagedReactors);
    }

    if(damagedPanels > 0)
    {
        faults.Add("PANEL:" + damagedPanels);
    }

    if(damagedHydrogenEngines > 0)
    {
        faults.Add("H2 ENG:" + damagedHydrogenEngines);
    }

    if(faults.Count == 0)
    {
        return "ALL SYSTEMS OK";
    }

    return string.Join(" ", faults) + " DAMAGED";
}

string GetHydrogenHealthStatus(
    int damagedHydrogenTanks,
    int damagedHydrogenEngines)
{
    List<string> faults = new List<string>();

    if(damagedHydrogenTanks > 0)
    {
        faults.Add("TANK:" + damagedHydrogenTanks);
    }

    if(damagedHydrogenEngines > 0)
    {
        faults.Add("ENGINE:" + damagedHydrogenEngines);
    }

    if(faults.Count == 0)
    {
        return "ALL SYSTEMS OK";
    }

    return string.Join(" ", faults) + " DAMAGED";
}

string GetBatteryMode(
    int batteryCount,
    int autoMode,
    int rechargeMode)
{
    if(batteryCount == 0)
    {
        return "NONE";
    }

    if(autoMode == batteryCount)
    {
        return "AUTO";
    }

    if(rechargeMode == batteryCount)
    {
        return "RECHARGE";
    }

    return "MIXED";
}

string BuildOverviewScreen(
    int batteryCount,
    double chargePercent,
    int hydrogenTankCount,
    double hydrogenPercent,
    double input,
    double output,
    string status,
    string batteryTime,
    string healthStatus)
{
    string screen = "";

    screen += Header("OVERVIEW");
    screen += "STATUS   : " + status + "\n";
    screen += "HEALTH   : " + healthStatus + "\n";
    screen += "\n";

    if(batteryCount > 0)
    {
        screen += "BATTERY  : " + PercentageFullBar(chargePercent, 20) + " " +
                  Math.Round(chargePercent, 1) + "%\n";
    }
    else
    {
        screen += "BATTERY  : N/A\n";
    }

    if(hydrogenTankCount > 0)
    {
        screen += "H2             : " + PercentageFullBar(hydrogenPercent, 20) + " " +
                  Math.Round(hydrogenPercent, 1) + "%\n";
    }
    else
    {
        screen += "H2       : N/A\n";
    }

    screen += "\n";
    screen += "INPUT    : " + FormatPower(input) + "\n";
    screen += "OUTPUT   : " + FormatPower(output) + "\n";
    screen += "TIME     : " + (batteryCount > 0 ? batteryTime : "N/A") + "\n";
    screen += Footer();

    return screen;
}

string BuildBatteryScreen(
    int batteryCount,
    double chargePercent,
    double storedPower,
    double maxPower,
    double input,
    double output,
    double maxOutput,
    string status,
    string batteryMode,
    string batteryHealthStatus)
{
    string screen = "";

    screen += Header("BATTERY");
    screen += "STATUS     : " + status + "\n";
    screen += "HEALTH     : " + batteryHealthStatus + "\n";
    screen += "\n";
    screen += "BATTERIES  : " + batteryCount + "\n";
    screen += "BATTERY    : " + PercentageFullBar(chargePercent, 20) + " " + Math.Round(chargePercent, 1) + "%\n";
    screen += "STORED     : " + FormatPower(storedPower) + " / " + FormatPower(maxPower) + " h\n";
    screen += "INPUT      : " + FormatPower(input) + "\n";
    screen += "OUTPUT     : " + FormatPower(output) + "\n";
    screen += "MAX OUT    : " + FormatPower(maxOutput) + "\n";
    screen += "\n";
    screen += "MODE       : " + batteryMode + "\n";
    screen += Footer();

    return screen;
}

string BuildGenerationScreen(
    int reactorCount,
    double reactorOutput,
    int solarCount,
    double solarOutput,
    int engineCount,
    double engineOutput,
    string generationHealthStatus)
{
    string screen = "";

    screen += Header("GENERATION");
    screen += "STATUS   : " + generationHealthStatus + "\n";
    screen += "\n";

    bool hasGenerationSection = false;

    if(reactorCount > 0)
    {
        screen += "REACTORS : " + reactorCount + "\n";
        screen += "OUT      : " + FormatPower(reactorOutput) + "\n";
        hasGenerationSection = true;
    }

    if(solarCount > 0)
    {
        if(hasGenerationSection)
        {
            screen += "\n";
        }

        screen += "SOLAR    : " + solarCount + "\n";
        screen += "OUT      : " + FormatPower(solarOutput) + "\n";
        hasGenerationSection = true;
    }

    if(engineCount > 0)
    {
        if(hasGenerationSection)
        {
            screen += "\n";
        }

        screen += "H2 ENG   : " + engineCount + "\n";
        screen += "OUT      : " + FormatPower(engineOutput) + "\n";
        hasGenerationSection = true;
    }

    if(!hasGenerationSection)
    {
        screen += "NO GENERATION FOUND\n";
    }

    screen += "\n";
    screen += "TOTAL    : " + FormatPower(reactorOutput + solarOutput + engineOutput) + "\n";
    screen += Footer();

    return screen;
}

string BuildHydrogenScreen(
    int tankCount,
    double hydrogenPercent,
    int engineCount,
    double engineOutput,
    string hydrogenHealthStatus)
{
    string screen = "";

    screen += Header("HYDROGEN");
    screen += "STATUS   : " + hydrogenHealthStatus + "\n";
    screen += "\n";

    if(tankCount == 0)
    {
        screen += "NO H2 TANKS FITTED\n";
        screen += Footer();
        return screen;
    }

    screen += "TANKS    : " + tankCount + "\n";
    screen += "FILL     : " + Math.Round(hydrogenPercent, 1) + "%\n";
    screen += "H2       : " + PercentageFullBar(hydrogenPercent, 20) + "\n";

    if(engineCount > 0)
    {
        screen += "\n";
        screen += "H2 ENG   : " + engineCount + "\n";
        screen += "OUTPUT   : " + FormatPower(engineOutput) + "\n";
    }

    screen += Footer();

    return screen;
}

string Header(string pageName)
{
    string screen = "";

    screen += "========================\n";
    screen += " POWER MONITOR " + version + "\n";
    screen += "========================\n";
    screen += "PAGE   : " + pageName + "\n";
    screen += "------------------------\n";

    return screen;
}

string Footer()
{
    string screen = "";

    screen += "------------------------\n";
    screen += "NEXT : " + GetNextPage() + "\n";
    screen += "PREV : " + GetPrevPage() + "\n";
    screen += "========================\n";

    return screen;
}

void NextPage()
{
    int index = Array.IndexOf(pages, currentPage);
    index++;

    if(index >= pages.Length)
    {
        index = 0;
    }

    currentPage = pages[index];
}

void PrevPage()
{
    int index = Array.IndexOf(pages, currentPage);
    index--;

    if(index < 0)
    {
        index = pages.Length - 1;
    }

    currentPage = pages[index];
}

string GetNextPage()
{
    int index = Array.IndexOf(pages, currentPage);
    index++;

    if(index >= pages.Length)
    {
        index = 0;
    }

    return pages[index].ToUpper();
}

string GetPrevPage()
{
    int index = Array.IndexOf(pages, currentPage);
    index--;

    if(index < 0)
    {
        index = pages.Length - 1;
    }

    return pages[index].ToUpper();
}

bool WriteToBestDisplay(string text)
{
    IMyTextSurface surface = GetCockpitSurface();

    if(surface != null)
    {
        WriteSurface(surface, text);
        return true;
    }

    IMyTextPanel lcd = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;

    if(lcd != null)
    {
        WriteSurface(lcd, text);
        return true;
    }

    return false;
}

IMyTextSurface GetCockpitSurface()
{
    IMyTerminalBlock cockpitBlock =
        GridTerminalSystem.GetBlockWithName(cockpitName);

    if(cockpitBlock == null)
    {
        return null;
    }

    IMyTextSurfaceProvider provider =
        cockpitBlock as IMyTextSurfaceProvider;

    if(provider == null)
    {
        return null;
    }

    if(cockpitScreenIndex < 0 ||
       cockpitScreenIndex >= provider.SurfaceCount)
    {
        return null;
    }

    return provider.GetSurface(cockpitScreenIndex);
}

void WriteSurface(IMyTextSurface surface, string text)
{
    surface.ContentType = ContentType.TEXT_AND_IMAGE;
    surface.FontSize = 0.75f;
    surface.Alignment = TextAlignment.LEFT;
    surface.WriteText(text);
}

string GetBatteryTimeRemaining(
    int batteryCount,
    double storedPower,
    double maxPower,
    double input,
    double output)
{
    if(batteryCount == 0 || maxPower <= 0)
    {
        return "N/A";
    }

    double netPower = output - input;

    // Discharging
    if(netPower > 0.001)
    {
        double hoursRemaining = storedPower / netPower;
        return FormatTime(hoursRemaining);
    }

    // Charging
    if(netPower < -0.001)
    {
        double powerIntoBattery = -netPower;
        double powerNeeded = maxPower - storedPower;

        if(powerNeeded <= 0.001)
        {
            return "FULL";
        }

        double hoursUntilFull = powerNeeded / powerIntoBattery;
        return "FULL IN " + FormatTime(hoursUntilFull);
    }

    return "STABLE";
}

string FormatTime(double hours)
{
    if(double.IsNaN(hours) ||
       double.IsInfinity(hours) ||
       hours < 0)
    {
        return "N/A";
    }

    if(hours > 999)
    {
        return "999h+";
    }

    int totalMinutes = (int)Math.Round(hours * 60);
    int wholeHours = totalMinutes / 60;
    int minutes = totalMinutes % 60;

    if(wholeHours <= 0)
    {
        return minutes + "m";
    }

    if(minutes == 0)
    {
        return wholeHours + "h";
    }

    return wholeHours + "h " + minutes + "m";
}

string FormatPower(double value)
{
    if(value < 1)
    {
        return Math.Round(value * 1000, 0) + " kW";
    }

    return Math.Round(value, 2) + " MW";
}

string PercentageFullBar(double percent, int width)
{
    percent = Math.Max(0, Math.Min(100, percent));

    int filled = (int)Math.Round(percent / 100 * width);
    int empty = width - filled;

    string bar = "[";

    for(int i = 0; i < filled; i++)
    {
        bar += "|";
    }

    for(int i = 0; i < empty; i++)
    {
        bar += ".";
    }

    bar += "]";

    return bar;
}

//==================================================
// CONSTRUCTION OS MODULE REGISTRATION
//==================================================

void RegisterConstructionOSModule()
{
    string moduleData =
        moduleDataStart + "\n" +
        "[ConstructionOS]\n" +
        "Type=Module\n" +
        "Protocol=1\n" +
        "ID=" + moduleId + "\n" +
        "Name=" + moduleName + "\n" +
        "Version=" + moduleVersion + "\n" +
        "Author=" + moduleAuthor + "\n" +
        "Commands=" + moduleCommands + "\n" +
        moduleDataEnd;

    string customData = Me.CustomData ?? "";

    int startIndex = customData.IndexOf(
        moduleDataStart,
        StringComparison.Ordinal
    );

    int endIndex = customData.IndexOf(
        moduleDataEnd,
        StringComparison.Ordinal
    );

    if(startIndex >= 0 &&
       endIndex >= startIndex)
    {
        endIndex += moduleDataEnd.Length;

        Me.CustomData =
            customData.Substring(0, startIndex) +
            moduleData +
            customData.Substring(endIndex);

        return;
    }

    if(customData.Length > 0 &&
       !customData.EndsWith("\n"))
    {
        customData += "\n";
    }

    if(customData.Length > 0)
    {
        customData += "\n";
    }

    Me.CustomData = customData + moduleData;
}

