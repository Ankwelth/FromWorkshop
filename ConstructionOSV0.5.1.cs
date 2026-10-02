/*
    Construction OS V0.5
    By IDecX

    Commands:
// up      - Move selection up
// down    - Move selection down
// left    - Interactive module control
// right   - Interactive module control
// select  - Open a module or run the selected module command
// back    - Return / interactive module control
// home    - Exit interactive control and return to the launcher
// reboot  - Run the complete Construction OS boot sequence
// rescan  - Refresh the display and installed module registry
*/


// User settings

string lcdName = "OS LCD";
string cockpitName = "Helm";
int cockpitScreenIndex = 0;
float displayFontSize = 0.70f;
float displayPadding = 2.0f;

// Stops the runtime log getting ridiculously long.
int maxRuntimeLogEntries = 20;


// Stuff the script uses internally

const string OS_VERSION = "V0.5";
const string OS_AUTHOR = "IDecX";
const string MODULE_SECTION = "ConstructionOS";
const string SUPPORTED_PROTOCOL = "1";

const string STATUS_OK = "OK";
const string STATUS_WARN = "WARN";
const string STATUS_FAIL = "FAIL";
const string STATUS_FATAL = "FATAL";


// Runtime stuff

IMyTextSurface activeSurface = null;

readonly List<IMyProgrammableBlock> scannedProgrammableBlocks =
    new List<IMyProgrammableBlock>();

readonly List<ConstructionModule> detectedModules =
    new List<ConstructionModule>();

readonly List<string> bootLog =
    new List<string>();

readonly List<string> runtimeLog =
    new List<string>();

readonly MyIni moduleIni = new MyIni();

int bootStep = 0;
int bootDelay = 0;

bool booting = false;
bool fatalBootError = false;

string activeDisplayDescription = "None";

bool insideModule = false;
bool insideSystem = false;
bool showingModuleAlert = false;
bool interactiveControlActive = false;

ConstructionModule alertModule = null;

int selectedModuleIndex = 0;
int selectedCommandIndex = 0;

string launcherMessage = "";
string lastCommand = "None";
string lastModule = "None";
string lastCommandResult = "None";


// What we know about each detected module

class ConstructionModule
{
    public IMyProgrammableBlock ProgrammableBlock;

    public string ID = "";
    public string Name = "";
    public string Version = "";
    public string Author = "";
    public string Protocol = "";
    public string Mode = "";

    public readonly List<string> Commands =
        new List<string>();

    public readonly List<string> Controls =
        new List<string>();

    public bool Valid = false;
    public bool Online = false;
    public string Error = "";
}


// Startup

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    BeginBoot();
}


// Main loop / commands

public void Main(string argument, UpdateType updateSource)
{
    string command = argument.Trim().ToLowerInvariant();

    if (command == "reboot")
    {
        AddRuntimeLog("[INFO] Reboot requested");
        BeginBoot();
        return;
    }

    if (command == "rescan")
    {
        if (!booting)
        {
            QuickRescan();
        }

        return;
    }

    if (booting)
    {
        RunBootSequence();
        return;
    }

    if (command == "home")
    {
        ReturnHome();
        return;
    }

    if (interactiveControlActive)
    {
        RouteInteractiveControl(command);
        return;
    }

    switch (command)
    {
        case "up":
            MoveSelection(-1);
            break;

        case "down":
            MoveSelection(1);
            break;

        case "select":
            SelectCurrentItem();
            break;

        case "back":
            ReturnToLauncher();
            break;
    }
}


// Boot sequence

void BeginBoot()
{
    booting = true;
    fatalBootError = false;

    bootStep = 0;
    bootDelay = 0;

    activeSurface = null;
    activeDisplayDescription = "None";

    scannedProgrammableBlocks.Clear();
    detectedModules.Clear();
    bootLog.Clear();

    ResetLauncherState();

    Echo("Construction OS starting...");

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}


void RunBootSequence()
{
    if (bootDelay > 0)
    {
        bootDelay--;
        return;
    }

    switch (bootStep)
    {
        case 0:
            AddBootHeader();
            AddStatusLine("Initialising kernel", STATUS_OK);
            bootStep++;
            bootDelay = 2;
            break;

        case 1:
            LocateDisplay();
            bootStep++;
            bootDelay = 3;
            break;

        case 2:
            ScanGridSystems();
            bootStep++;
            bootDelay = 3;
            break;

        case 3:
            LoadModuleRegistry();
            bootStep++;
            bootDelay = 3;
            break;

        case 4:
            DisplayDetectedModules();
            bootStep++;
            bootDelay = 3;
            break;

        case 5:
            ValidateModuleCommands();
            bootStep++;
            bootDelay = 3;
            break;

        case 6:
            StartOperatorInterface();
            bootStep++;
            bootDelay = 3;
            break;

        case 7:
            CompleteBoot();
            bootStep++;
            bootDelay = 12;
            break;

        case 8:
            booting = false;

            if (!fatalBootError)
            {
                ClampLauncherSelection();
                AddRuntimeLog("[OK] Construction OS " + OS_VERSION + " online");
                ShowLauncher();
            }
            else
            {
                AddRuntimeLog("[FATAL] Boot halted");
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }

            break;
    }
}


// Boot checks

void LocateDisplay()
{
    activeSurface = null;
    activeDisplayDescription = "None";

    IMyTextPanel lcd =
        GridTerminalSystem.GetBlockWithName(lcdName)
        as IMyTextPanel;

    if (lcd != null && lcd.IsSameConstructAs(Me))
    {
        activeSurface = lcd;
        activeDisplayDescription = lcd.CustomName;

        ConfigureSurface(activeSurface);

        AddStatusLine("Locating display", STATUS_OK);
        AddDetailLine("Using display: " + activeDisplayDescription);

        return;
    }

    IMyTerminalBlock cockpitBlock =
        GridTerminalSystem.GetBlockWithName(cockpitName)
        as IMyTerminalBlock;

    IMyTextSurfaceProvider surfaceProvider =
        cockpitBlock as IMyTextSurfaceProvider;

    if (cockpitBlock != null
        && cockpitBlock.IsSameConstructAs(Me)
        && surfaceProvider != null
        && cockpitScreenIndex >= 0
        && cockpitScreenIndex < surfaceProvider.SurfaceCount)
    {
        activeSurface =
            surfaceProvider.GetSurface(cockpitScreenIndex);

        activeDisplayDescription =
            cockpitBlock.CustomName
            + " : Screen "
            + cockpitScreenIndex;

        ConfigureSurface(activeSurface);

        AddStatusLine("Locating display", STATUS_WARN);
        AddDetailLine(lcdName + " not found");
        AddDetailLine("Using fallback: " + activeDisplayDescription);

        return;
    }

    fatalBootError = true;

    AddStatusLine("Locating display", STATUS_FATAL);
    AddDetailLine("Dedicated LCD not found: " + lcdName);
    AddDetailLine("Fallback cockpit unavailable: " + cockpitName);
    AddDetailLine("Boot output redirected to Echo");
}


void ScanGridSystems()
{
    PerformGridScan();

    AddStatusLine(
        "Scanning grid systems",
        STATUS_OK
    );

    AddDetailLine(
        scannedProgrammableBlocks.Count
        + " programmable block"
        + Plural(scannedProgrammableBlocks.Count)
        + " found"
    );
}


void LoadModuleRegistry()
{
    BuildModuleRegistry();

    AddStatusLine(
        "Loading module registry",
        STATUS_OK
    );

    AddDetailLine(
        detectedModules.Count
        + " module definition"
        + Plural(detectedModules.Count)
        + " found"
    );
}


void DisplayDetectedModules()
{
    AddBlankLine();
    AddPlainLine("Detecting installed modules...");
    AddBlankLine();

    if (detectedModules.Count == 0)
    {
        AddStatusLine("No modules detected", STATUS_WARN);
        return;
    }

    for (int i = 0; i < detectedModules.Count; i++)
    {
        ConstructionModule module = detectedModules[i];
        string displayName = module.Name;

        if (!string.IsNullOrWhiteSpace(module.Version))
        {
            displayName += " " + module.Version;
        }

        if (module.Valid && module.Online)
        {
            AddStatusLine(displayName, STATUS_OK);
        }
        else if (module.Valid && !module.Online)
        {
            AddStatusLine(displayName, STATUS_WARN);
            AddDetailLine("Module PB is switched off");
        }
        else
        {
            AddStatusLine(displayName, STATUS_FAIL);
            AddDetailLine(module.Error);
        }
    }
}


void ValidateModuleCommands()
{
    int validModules = 0;
    int invalidModules = 0;
    int commandCount = 0;

    CheckDuplicateModuleIDs();

    for (int i = 0; i < detectedModules.Count; i++)
    {
        ConstructionModule module = detectedModules[i];

        if (module.Valid)
        {
            validModules++;
            commandCount += module.Commands.Count;
        }
        else
        {
            invalidModules++;
        }
    }

    if (invalidModules > 0)
    {
        AddStatusLine("Validating module commands", STATUS_WARN);
        AddDetailLine(validModules + " valid, " + invalidModules + " rejected");
    }
    else
    {
        AddStatusLine("Validating module commands", STATUS_OK);
        AddDetailLine(commandCount + " command" + Plural(commandCount) + " registered");
    }
}


void StartOperatorInterface()
{
    if (fatalBootError)
    {
        AddStatusLine("Starting operator interface", STATUS_FAIL);
        AddDetailLine("No usable display available");
        return;
    }

    AddStatusLine("Starting operator interface", STATUS_OK);
}


void CompleteBoot()
{
    AddBlankLine();

    int validModuleCount = GetValidModuleCount();
    int invalidModuleCount =
        detectedModules.Count - validModuleCount;

    AddPlainLine(
        validModuleCount
        + " module"
        + Plural(validModuleCount)
        + " loaded."
    );

    if (fatalBootError)
    {
        AddPlainLine("Boot halted.");
        AddPlainLine("Check the programmable block Custom Data.");
    }
    else if (invalidModuleCount > 0)
    {
        AddPlainLine("System started with warnings.");
    }
    else
    {
        AddPlainLine("System ready.");
    }

    WriteBootOutput();
}


// Quick rescan

void QuickRescan()
{
    launcherMessage = "Rescanning modules...";
    WriteCurrentMenu();

    RefreshDisplayWithoutBootLog();
    PerformGridScan();
    BuildModuleRegistry();
    CheckDuplicateModuleIDs();

    insideModule = false;
    insideSystem = false;
    showingModuleAlert = false;
    interactiveControlActive = false;
    alertModule = null;
    selectedCommandIndex = 0;

    ClampLauncherSelection();

    int validCount = GetValidModuleCount();
    int onlineCount = GetOnlineModuleCount();
    int offlineCount = validCount - onlineCount;
    int rejectedCount =
        detectedModules.Count - validCount;

    launcherMessage =
        "Rescan: "
        + onlineCount
        + " online";

    if (offlineCount > 0)
    {
        launcherMessage +=
            ", "
            + offlineCount
            + " offline";
    }

    if (rejectedCount > 0)
    {
        launcherMessage +=
            ", "
            + rejectedCount
            + " rejected";
    }

    AddRuntimeLog(
        (rejectedCount > 0 || offlineCount > 0)
        ? "[WARN] Module rescan - "
            + onlineCount + " online, "
            + offlineCount + " offline, "
            + rejectedCount + " rejected"
        : "[OK] Module rescan complete - "
            + onlineCount + " online"
    );

    ShowLauncher();

    Echo(
        "Construction OS rescan complete.\n"
        + onlineCount
        + " online module"
        + Plural(validCount)
        + ".\n"
        + rejectedCount
        + " rejected module"
        + Plural(rejectedCount)
        + "."
    );
}


void RefreshDisplayWithoutBootLog()
{
    activeSurface = null;
    activeDisplayDescription = "None";

    IMyTextPanel lcd =
        GridTerminalSystem.GetBlockWithName(lcdName)
        as IMyTextPanel;

    if (lcd != null && lcd.IsSameConstructAs(Me))
    {
        activeSurface = lcd;
        activeDisplayDescription = lcd.CustomName;

        ConfigureSurface(activeSurface);

        fatalBootError = false;
        return;
    }

    IMyTerminalBlock cockpitBlock =
        GridTerminalSystem.GetBlockWithName(cockpitName)
        as IMyTerminalBlock;

    IMyTextSurfaceProvider provider =
        cockpitBlock as IMyTextSurfaceProvider;

    if (cockpitBlock != null
        && cockpitBlock.IsSameConstructAs(Me)
        && provider != null
        && cockpitScreenIndex >= 0
        && cockpitScreenIndex < provider.SurfaceCount)
    {
        activeSurface =
            provider.GetSurface(cockpitScreenIndex);

        activeDisplayDescription =
            cockpitBlock.CustomName
            + " : Screen "
            + cockpitScreenIndex;

        ConfigureSurface(activeSurface);

        fatalBootError = false;
        return;
    }

    fatalBootError = true;
}


// Module scanning / registry

void PerformGridScan()
{
    scannedProgrammableBlocks.Clear();

    GridTerminalSystem.GetBlocksOfType(
        scannedProgrammableBlocks,
        block =>
            block != Me
            && block.IsSameConstructAs(Me)
    );
}


void BuildModuleRegistry()
{
    detectedModules.Clear();

    for (int i = 0;
         i < scannedProgrammableBlocks.Count;
         i++)
    {
        ConstructionModule module =
            ReadModuleMetadata(
                scannedProgrammableBlocks[i]
            );

        if (module != null)
        {
            detectedModules.Add(module);
        }
    }
}


ConstructionModule ReadModuleMetadata(
    IMyProgrammableBlock programmableBlock)
{
    string customData = programmableBlock.CustomData;

    if (string.IsNullOrWhiteSpace(customData))
    {
        return null;
    }

    MyIniParseResult parseResult;

    moduleIni.Clear();

    if (!moduleIni.TryParse(customData, out parseResult))
    {
        return null;
    }

    string type =
        moduleIni.Get(
            MODULE_SECTION,
            "Type"
        ).ToString();

    if (!type.Equals(
        "Module",
        StringComparison.OrdinalIgnoreCase))
    {
        return null;
    }

    ConstructionModule module =
        new ConstructionModule();

    module.ProgrammableBlock = programmableBlock;
    module.Online = programmableBlock.IsWorking;

    module.ID =
        moduleIni.Get(
            MODULE_SECTION,
            "ID"
        ).ToString().Trim();

    module.Name =
        moduleIni.Get(
            MODULE_SECTION,
            "Name"
        ).ToString().Trim();

    module.Version =
        moduleIni.Get(
            MODULE_SECTION,
            "Version"
        ).ToString().Trim();

    module.Author =
        moduleIni.Get(
            MODULE_SECTION,
            "Author"
        ).ToString().Trim();

    module.Protocol =
        moduleIni.Get(
            MODULE_SECTION,
            "Protocol"
        ).ToString().Trim();

    module.Mode =
        moduleIni.Get(
            MODULE_SECTION,
            "Mode"
        ).ToString().Trim();

    string controlText =
        moduleIni.Get(
            MODULE_SECTION,
            "Controls"
        ).ToString();

    string[] controlParts =
        controlText.Split(
            new char[] { ',' },
            StringSplitOptions.RemoveEmptyEntries
        );

    for (int i = 0; i < controlParts.Length; i++)
    {
        string control = controlParts[i].Trim().ToLowerInvariant();

        if (!string.IsNullOrWhiteSpace(control))
        {
            module.Controls.Add(control);
        }
    }

    string commandText =
        moduleIni.Get(
            MODULE_SECTION,
            "Commands"
        ).ToString();

    string[] commandParts =
        commandText.Split(
            new char[] { ',' },
            StringSplitOptions.RemoveEmptyEntries
        );

    for (int i = 0; i < commandParts.Length; i++)
    {
        string command = commandParts[i].Trim();

        if (!string.IsNullOrWhiteSpace(command))
        {
            module.Commands.Add(command);
        }
    }

    ValidateModuleMetadata(module);

    return module;
}


void ValidateModuleMetadata(ConstructionModule module)
{
    module.Valid = false;
    module.Error = "";

    if (string.IsNullOrWhiteSpace(module.ID))
    {
        module.Error = "Missing module ID";
        return;
    }

    if (string.IsNullOrWhiteSpace(module.Name))
    {
        module.Name = module.ProgrammableBlock.CustomName;
        module.Error = "Missing module name";
        return;
    }

    if (module.Protocol != SUPPORTED_PROTOCOL)
    {
        module.Error =
            "Unsupported protocol: "
            + module.Protocol;

        return;
    }

    if (!string.IsNullOrWhiteSpace(module.Mode)
        && !module.Mode.Equals(
            "Command",
            StringComparison.OrdinalIgnoreCase)
        && !module.Mode.Equals(
            "Interactive",
            StringComparison.OrdinalIgnoreCase))
    {
        module.Error =
            "Unsupported module mode: "
            + module.Mode;

        return;
    }

    if (IsInteractiveModule(module))
    {
        if (module.Controls.Count == 0)
        {
            module.Error = "No controls registered";
            return;
        }
    }
    else if (module.Commands.Count == 0)
    {
        module.Error = "No commands registered";
        return;
    }

    module.Valid = true;

    if (!module.Online)
    {
        module.Error = "Module PB is switched off";
    }
}


bool IsInteractiveModule(ConstructionModule module)
{
    return
        module != null
        && module.Mode.Equals(
            "Interactive",
            StringComparison.OrdinalIgnoreCase);
}


int GetModuleMenuItemCount(ConstructionModule module)
{
    if (module == null)
    {
        return 0;
    }

    return
        module.Commands.Count
        + (IsInteractiveModule(module) ? 1 : 0);
}


bool HasModuleControl(
    ConstructionModule module,
    string command)
{
    if (module == null
        || string.IsNullOrWhiteSpace(command))
    {
        return false;
    }

    for (int i = 0; i < module.Controls.Count; i++)
    {
        if (module.Controls[i].Equals(
            command,
            StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
    }

    return false;
}


void CheckDuplicateModuleIDs()
{
    for (int first = 0;
         first < detectedModules.Count;
         first++)
    {
        ConstructionModule firstModule =
            detectedModules[first];

        if (string.IsNullOrWhiteSpace(firstModule.ID))
        {
            continue;
        }

        for (int second = first + 1;
             second < detectedModules.Count;
             second++)
        {
            ConstructionModule secondModule =
                detectedModules[second];

            if (firstModule.ID.Equals(
                secondModule.ID,
                StringComparison.OrdinalIgnoreCase))
            {
                firstModule.Valid = false;
                secondModule.Valid = false;

                firstModule.Error =
                    "Duplicate module ID: "
                    + firstModule.ID;

                secondModule.Error =
                    "Duplicate module ID: "
                    + secondModule.ID;
            }
        }
    }
}


int GetValidModuleCount()
{
    int count = 0;

    for (int i = 0;
         i < detectedModules.Count;
         i++)
    {
        if (detectedModules[i].Valid)
        {
            count++;
        }
    }

    return count;
}


void RefreshModuleStates()
{
    for (int i = 0; i < detectedModules.Count; i++)
    {
        ConstructionModule module = detectedModules[i];

        if (!module.Valid || module.ProgrammableBlock == null)
        {
            continue;
        }

        bool wasOnline = module.Online;
        module.Online = module.ProgrammableBlock.IsWorking;

        if (wasOnline != module.Online)
        {
            AddRuntimeLog(
                module.Online
                ? "[OK] " + module.Name + " came online"
                : "[WARN] " + module.Name + " went offline"
            );
        }
    }
}


int GetOnlineModuleCount()
{
    int count = 0;

    for (int i = 0;
         i < detectedModules.Count;
         i++)
    {
        if (detectedModules[i].Valid
            && detectedModules[i].Online)
        {
            count++;
        }
    }

    return count;
}


ConstructionModule GetValidModuleByMenuIndex(
    int menuIndex)
{
    int currentValidIndex = 0;

    for (int i = 0;
         i < detectedModules.Count;
         i++)
    {
        ConstructionModule module =
            detectedModules[i];

        if (!module.Valid)
        {
            continue;
        }

        if (currentValidIndex == menuIndex)
        {
            return module;
        }

        currentValidIndex++;
    }

    return null;
}


// Menu controls

void ResetLauncherState()
{
    insideModule = false;
    insideSystem = false;
    showingModuleAlert = false;
    alertModule = null;

    selectedModuleIndex = 0;
    selectedCommandIndex = 0;

    launcherMessage = "";
}


void ClampLauncherSelection()
{
    int mainMenuCount =
        GetValidModuleCount() + 1;

    selectedModuleIndex =
        WrapIndex(
            selectedModuleIndex,
            mainMenuCount
        );

    if (selectedModuleIndex >=
        GetValidModuleCount())
    {
        selectedCommandIndex = 0;
        return;
    }

    ConstructionModule module =
        GetValidModuleByMenuIndex(
            selectedModuleIndex
        );

    int menuItemCount =
        GetModuleMenuItemCount(module);

    if (module == null
        || menuItemCount <= 0)
    {
        selectedCommandIndex = 0;
        return;
    }

    selectedCommandIndex =
        WrapIndex(
            selectedCommandIndex,
            menuItemCount
        );
}


void MoveSelection(int direction)
{
    launcherMessage = "";

    if (showingModuleAlert)
    {
        ShowModuleUnavailableAlert();
        return;
    }

    if (insideSystem)
    {
        ShowSystemScreen();
        return;
    }

    if (!insideModule)
    {
        int mainMenuCount =
            GetValidModuleCount() + 1;

        selectedModuleIndex =
            WrapIndex(
                selectedModuleIndex + direction,
                mainMenuCount
            );

        ShowLauncher();
        return;
    }

    ConstructionModule module =
        GetValidModuleByMenuIndex(
            selectedModuleIndex
        );

    int menuItemCount =
        GetModuleMenuItemCount(module);

    if (module == null
        || menuItemCount <= 0)
    {
        ReturnToLauncher();
        return;
    }

    selectedCommandIndex =
        WrapIndex(
            selectedCommandIndex + direction,
            menuItemCount
        );

    ShowModuleMenu(module);
}


void SelectCurrentItem()
{
    launcherMessage = "";
    RefreshModuleStates();

    if (showingModuleAlert)
    {
        ShowModuleUnavailableAlert();
        return;
    }

    if (insideSystem)
    {
        ShowSystemScreen();
        return;
    }

    if (!insideModule)
    {
        int validModuleCount =
            GetValidModuleCount();

        if (selectedModuleIndex ==
            validModuleCount)
        {
            insideSystem = true;

            AddRuntimeLog(
                "[INFO] Opened System page"
            );

            ShowSystemScreen();
            return;
        }

        ConstructionModule module =
            GetValidModuleByMenuIndex(
                selectedModuleIndex
            );

        if (module == null)
        {
            launcherMessage = "No module selected";
            ShowLauncher();
            return;
        }

        if (!module.Online)
        {
            showingModuleAlert = true;
            alertModule = module;

            AddRuntimeLog(
                "[WARN] Access blocked - "
                + module.Name
                + " is offline"
            );

            ShowModuleUnavailableAlert();
            return;
        }

        insideModule = true;
        selectedCommandIndex = 0;

        AddRuntimeLog(
            "[INFO] Opened module: "
            + module.Name
        );

        ShowModuleMenu(module);
        return;
    }

    ConstructionModule selectedModule =
        GetValidModuleByMenuIndex(
            selectedModuleIndex
        );

    if (selectedModule == null)
    {
        ReturnToLauncher();
        return;
    }

    RefreshModuleStates();

    if (!selectedModule.Online)
    {
        insideModule = false;
        interactiveControlActive = false;
        showingModuleAlert = true;
        alertModule = selectedModule;

        AddRuntimeLog(
            "[WARN] Command blocked - "
            + selectedModule.Name
            + " went offline"
        );

        ShowModuleUnavailableAlert();
        return;
    }

    bool interactive =
        IsInteractiveModule(selectedModule);

    if (interactive
        && selectedCommandIndex == 0)
    {
        EnterInteractiveControl(selectedModule);
        return;
    }

    int commandIndex =
        selectedCommandIndex
        - (interactive ? 1 : 0);

    if (commandIndex < 0
        || commandIndex >= selectedModule.Commands.Count)
    {
        launcherMessage = "No command selected";
        ShowModuleMenu(selectedModule);
        return;
    }

    string command =
        selectedModule.Commands[
            commandIndex
        ];

    bool accepted =
        selectedModule.ProgrammableBlock.TryRun(command);

    lastCommand =
        PrettifyCommand(command);

    lastModule =
        selectedModule.Name;

    lastCommandResult =
        accepted
        ? "SENT"
        : "REJECTED";

    launcherMessage =
        accepted
        ? "COMMAND : "
            + lastCommand
            + "\nRESULT  : SENT"
        : "COMMAND : "
            + lastCommand
            + "\nRESULT  : REJECTED";

    AddRuntimeLog(
        accepted
        ? "[OK] "
            + selectedModule.Name
            + " <- "
            + command
        : "[FAIL] "
            + selectedModule.Name
            + " rejected "
            + command
    );

    ShowModuleMenu(selectedModule);
}


void ReturnToLauncher()
{
    launcherMessage = "";

    if (showingModuleAlert)
    {
        showingModuleAlert = false;
        alertModule = null;
        ShowLauncher();
        return;
    }

    if (insideSystem)
    {
        insideSystem = false;
        ShowLauncher();
        return;
    }

    if (!insideModule)
    {
        ShowLauncher();
        return;
    }

    insideModule = false;
    interactiveControlActive = false;
    selectedCommandIndex = 0;

    ShowLauncher();
}


void EnterInteractiveControl(
    ConstructionModule module)
{
    if (module == null)
    {
        ReturnToLauncher();
        return;
    }

    interactiveControlActive = true;
    launcherMessage = "";

    AddRuntimeLog(
        "[INFO] Interactive control: "
        + module.Name
    );

    ShowInteractiveControlScreen(module);
}


void RouteInteractiveControl(string command)
{
    ConstructionModule module =
        GetValidModuleByMenuIndex(
            selectedModuleIndex
        );

    if (module == null)
    {
        ReturnHome();
        return;
    }

    RefreshModuleStates();

    if (!module.Online)
    {
        interactiveControlActive = false;
        insideModule = false;
        showingModuleAlert = true;
        alertModule = module;

        AddRuntimeLog(
            "[WARN] Interactive control lost - "
            + module.Name
            + " is offline"
        );

        ShowModuleUnavailableAlert();
        return;
    }

    if (!HasModuleControl(
        module,
        command))
    {
        ShowInteractiveControlScreen(module);
        return;
    }

    bool accepted =
        module.ProgrammableBlock.TryRun(command);

    lastModule = module.Name;
    lastCommand = PrettifyCommand(command);
    lastCommandResult =
        accepted
        ? "SENT"
        : "REJECTED";

    if (!accepted)
    {
        AddRuntimeLog(
            "[FAIL] "
            + module.Name
            + " rejected control "
            + command
        );
    }

    ShowInteractiveControlScreen(module);
}


void ReturnHome()
{
    bool wasInteractive =
        interactiveControlActive;

    insideModule = false;
    insideSystem = false;
    showingModuleAlert = false;
    interactiveControlActive = false;
    alertModule = null;
    selectedCommandIndex = 0;
    launcherMessage = "";

    if (wasInteractive)
    {
        AddRuntimeLog(
            "[INFO] Returned home from interactive module"
        );
    }

    ShowLauncher();
}


int WrapIndex(int value, int count)
{
    if (count <= 0)
    {
        return 0;
    }

    while (value < 0)
    {
        value += count;
    }

    while (value >= count)
    {
        value -= count;
    }

    return value;
}


// Menu screens

void WriteCurrentMenu()
{
    RefreshModuleStates();

    if (showingModuleAlert)
    {
        ShowModuleUnavailableAlert();
        return;
    }

    if (insideSystem)
    {
        ShowSystemScreen();
        return;
    }

    if (interactiveControlActive)
    {
        ConstructionModule interactiveModule =
            GetValidModuleByMenuIndex(
                selectedModuleIndex
            );

        if (interactiveModule != null)
        {
            ShowInteractiveControlScreen(
                interactiveModule
            );
            return;
        }

        ReturnHome();
        return;
    }

    if (insideModule)
    {
        ConstructionModule module =
            GetValidModuleByMenuIndex(
                selectedModuleIndex
            );

        if (module != null)
        {
            ShowModuleMenu(module);
            return;
        }
    }

    ShowLauncher();
}


void ShowLauncher()
{
    RefreshModuleStates();
    Runtime.UpdateFrequency = UpdateFrequency.None;

    StringBuilder output = new StringBuilder();

    AppendHeader(
        output,
        "CONSTRUCTION OS " + OS_VERSION
    );

    output.AppendLine("MODULES");
    output.AppendLine();

    int validModuleCount =
        GetValidModuleCount();

    int menuIndex = 0;

    for (int i = 0;
         i < detectedModules.Count;
         i++)
    {
        ConstructionModule module =
            detectedModules[i];

        if (!module.Valid)
        {
            continue;
        }

        output.Append(
            menuIndex == selectedModuleIndex
            ? "> "
            : "  "
        );

        output.Append(module.Name);

        if (!module.Online)
        {
            output.Append(" [OFFLINE]");
        }

        output.AppendLine();

        menuIndex++;
    }

    output.Append(
        selectedModuleIndex ==
        validModuleCount
        ? "> "
        : "  "
    );

    output.AppendLine("SYSTEM");

    output.AppendLine();
    output.AppendLine("------------------------");

    if (!string.IsNullOrWhiteSpace(launcherMessage))
    {
        output.AppendLine(launcherMessage);
        output.AppendLine("------------------------");
    }

    output.AppendLine(
        "MODULES : " + validModuleCount
    );

    output.AppendLine(
        "STATUS  : " + GetLauncherStatus()
    );

    AppendFooter(
        output,
        "UP/DOWN : move",
        "SELECT  : open"
    );

    WriteToDisplay(output.ToString());
    Echo(output.ToString());
}


void ShowModuleMenu(ConstructionModule module)
{
    RefreshModuleStates();
    Runtime.UpdateFrequency = UpdateFrequency.None;

    StringBuilder output = new StringBuilder();

    AppendHeader(
        output,
        module.Name.ToUpperInvariant()
    );

    if (!string.IsNullOrWhiteSpace(module.Version))
    {
        output.AppendLine(
            "VERSION : " + module.Version
        );

        output.AppendLine(
            "STATUS  : "
            + (module.Online ? "ONLINE" : "OFFLINE")
        );

        output.AppendLine();
    }

    int menuIndex = 0;

    if (IsInteractiveModule(module))
    {
        output.Append(
            selectedCommandIndex == menuIndex
            ? "> "
            : "  "
        );

        output.AppendLine("OPEN INTERFACE");
        menuIndex++;
    }

    for (int i = 0;
         i < module.Commands.Count;
         i++)
    {
        output.Append(
            selectedCommandIndex == menuIndex
            ? "> "
            : "  "
        );

        output.AppendLine(
            PrettifyCommand(
                module.Commands[i]
            )
        );

        menuIndex++;
    }

    output.AppendLine();

    if (!string.IsNullOrWhiteSpace(launcherMessage))
    {
        output.AppendLine("------------------------");
        output.AppendLine(launcherMessage);
    }

    AppendFooter(
        output,
        "UP/DOWN : move",
        "BACK    : MODULES"
    );

    WriteToDisplay(output.ToString());
    Echo(output.ToString());
}


void ShowInteractiveControlScreen(
    ConstructionModule module)
{
    RefreshModuleStates();
    Runtime.UpdateFrequency = UpdateFrequency.None;

    StringBuilder output = new StringBuilder();

    AppendHeader(
        output,
        module.Name.ToUpperInvariant()
    );

    if (!string.IsNullOrWhiteSpace(module.Version))
    {
        output.AppendLine(
            "VERSION : " + module.Version
        );
    }

    output.AppendLine(
        "STATUS  : "
        + (module.Online ? "ONLINE" : "OFFLINE")
    );

    output.AppendLine();
    output.AppendLine("INTERACTIVE CONTROL");
    output.AppendLine("ACTIVE");
    output.AppendLine();
    output.AppendLine("Controls are routed to");
    output.AppendLine("the module.");
    output.AppendLine();
    output.AppendLine("------------------------");
    output.AppendLine("HOME : MODULES");

    WriteToDisplay(output.ToString());
    Echo(output.ToString());
}


void ShowModuleUnavailableAlert()
{
    RefreshModuleStates();
    Runtime.UpdateFrequency = UpdateFrequency.None;

    StringBuilder output = new StringBuilder();

    output.AppendLine("------------------------");
    output.AppendLine("         ALERT");
    output.AppendLine("------------------------");
    output.AppendLine();
    output.AppendLine("MODULE UNAVAILABLE");
    output.AppendLine();

    if (alertModule != null)
    {
        output.AppendLine(
            TruncateText(
                alertModule.Name,
                22
            )
        );
    }
    else
    {
        output.AppendLine("Unknown module");
    }

    output.AppendLine("is currently OFFLINE.");
    output.AppendLine();
    output.AppendLine("Commands cannot be sent");
    output.AppendLine("until the module is online.");
    output.AppendLine();
    output.AppendLine("------------------------");
    output.AppendLine("BACK : return");

    WriteToDisplay(output.ToString());
    Echo(output.ToString());
}


void ShowSystemScreen()
{
    RefreshModuleStates();
    Runtime.UpdateFrequency = UpdateFrequency.None;

    StringBuilder output = new StringBuilder();

    AppendHeader(output, "SYSTEM");

    output.AppendLine(
        "OS       : " + OS_VERSION
    );

    output.AppendLine(
        "MODULES  : "
        + GetOnlineModuleCount()
        + "/"
        + GetValidModuleCount()
        + " ONLINE"
    );

    output.AppendLine(
        "DISPLAY  : "
        + ShortDisplayName()
    );

    output.AppendLine(
        "STATUS   : "
        + GetLauncherStatus()
    );

    output.AppendLine(
        "WARNINGS : "
        + GetWarningCount()
    );

    output.AppendLine();

    output.AppendLine(
        "LAST MOD : "
        + TruncateText(lastModule, 15)
    );

    output.AppendLine(
        "LAST CMD : "
        + TruncateText(lastCommand, 15)
    );

    output.AppendLine(
        "RESULT   : "
        + lastCommandResult
    );

    output.AppendLine();

    output.AppendLine(
        "LOG      : "
        + runtimeLog.Count
        + "/"
        + maxRuntimeLogEntries
    );

    AppendFooter(
        output,
        "RESCAN : run rescan",
        "BACK   : modules"
    );

    WriteToDisplay(output.ToString());
    Echo(output.ToString());
}


void AppendHeader(
    StringBuilder output,
    string title)
{
    output.AppendLine(title);
    output.AppendLine("------------------------");
    output.AppendLine();
}


void AppendFooter(
    StringBuilder output,
    string firstLine,
    string secondLine)
{
    output.AppendLine("------------------------");
    output.AppendLine(firstLine);
    output.AppendLine(secondLine);
}


string GetLauncherStatus()
{
    if (fatalBootError)
    {
        return "OFFLINE";
    }

    if (GetValidModuleCount() <
        detectedModules.Count)
    {
        return "WARNING";
    }

    if (GetOnlineModuleCount() <
        GetValidModuleCount())
    {
        return "WARNING";
    }

    return "ONLINE";
}


int GetWarningCount()
{
    int warnings = 0;

    if (fatalBootError)
    {
        warnings++;
    }

    for (int i = 0;
         i < detectedModules.Count;
         i++)
    {
        if (!detectedModules[i].Valid
            || !detectedModules[i].Online)
        {
            warnings++;
        }
    }

    return warnings;
}


string ShortDisplayName()
{
    if (activeDisplayDescription == lcdName)
    {
        return lcdName;
    }

    return TruncateText(
        activeDisplayDescription,
        16
    );
}


string TruncateText(
    string text,
    int maxLength)
{
    if (string.IsNullOrWhiteSpace(text))
    {
        return "None";
    }

    if (text.Length <= maxLength)
    {
        return text;
    }

    if (maxLength <= 3)
    {
        return text.Substring(
            0,
            maxLength
        );
    }

    return text.Substring(
        0,
        maxLength - 3
    ) + "...";
}


string PrettifyCommand(string command)
{
    if (string.IsNullOrWhiteSpace(command))
    {
        return "Unnamed Command";
    }

    string cleaned =
        command
        .Replace('_', ' ')
        .Replace('-', ' ')
        .Trim();

    StringBuilder result =
        new StringBuilder();

    bool capitaliseNext = true;

    for (int i = 0;
         i < cleaned.Length;
         i++)
    {
        char character = cleaned[i];

        if (char.IsWhiteSpace(character))
        {
            if (result.Length > 0
                && result[result.Length - 1] != ' ')
            {
                result.Append(' ');
            }

            capitaliseNext = true;
            continue;
        }

        if (capitaliseNext)
        {
            result.Append(
                char.ToUpperInvariant(character)
            );

            capitaliseNext = false;
        }
        else
        {
            result.Append(character);
        }
    }

    return result.ToString();
}


// Runtime log

void AddRuntimeLog(string message)
{
    if (string.IsNullOrWhiteSpace(message))
    {
        return;
    }

    runtimeLog.Add(message);

    while (runtimeLog.Count >
        maxRuntimeLogEntries)
    {
        runtimeLog.RemoveAt(0);
    }

    SaveLogsToCustomData();
}


// Boot log

void AddBootHeader()
{
    bootLog.Add(
        "CONSTRUCTION OS " + OS_VERSION
    );

    bootLog.Add(
        "Copyright (C) " + OS_AUTHOR
    );

    bootLog.Add("");

    WriteBootOutput();
}


void AddStatusLine(
    string message,
    string status)
{
    bootLog.Add(
        FormatStatusLine(
            message,
            status
        )
    );

    WriteBootOutput();
}


void AddDetailLine(string message)
{
    bootLog.Add("  " + message);
    WriteBootOutput();
}


void AddPlainLine(string message)
{
    bootLog.Add(message);
    WriteBootOutput();
}


void AddBlankLine()
{
    bootLog.Add("");
    WriteBootOutput();
}


string FormatStatusLine(
    string message,
    string status)
{
    const int targetWidth = 38;

    string result = message;

    int dotsNeeded =
        targetWidth
        - message.Length
        - status.Length
        - 3;

    if (dotsNeeded < 1)
    {
        dotsNeeded = 1;
    }

    result += new string(
        '.',
        dotsNeeded
    );

    result += " [";
    result += status;
    result += "]";

    return result;
}


void WriteBootOutput()
{
    StringBuilder output =
        new StringBuilder();

    for (int i = 0;
         i < bootLog.Count;
         i++)
    {
        output.AppendLine(
            bootLog[i]
        );
    }

    string logText =
        output.ToString();

    WriteToDisplay(logText);
    Echo(logText);

    SaveLogsToCustomData();
}


void SaveLogsToCustomData()
{
    StringBuilder customData =
        new StringBuilder();

    customData.AppendLine(
        "========================================"
    );

    customData.AppendLine(
        "CONSTRUCTION OS BOOT LOG"
    );

    customData.AppendLine(
        "========================================"
    );

    customData.AppendLine();

    for (int i = 0;
         i < bootLog.Count;
         i++)
    {
        customData.AppendLine(
            bootLog[i]
        );
    }

    customData.AppendLine();
    customData.AppendLine(
        "========================================"
    );

    customData.AppendLine(
        "CONSTRUCTION OS RUNTIME LOG"
    );

    customData.AppendLine(
        "========================================"
    );

    customData.AppendLine();

    if (runtimeLog.Count == 0)
    {
        customData.AppendLine(
            "No runtime events recorded."
        );
    }
    else
    {
        for (int i = 0;
             i < runtimeLog.Count;
             i++)
        {
            customData.AppendLine(
                runtimeLog[i]
            );
        }
    }

    Me.CustomData =
        customData.ToString();
}


// Display helpers

void ConfigureSurface(IMyTextSurface surface)
{
    if (surface == null)
    {
        return;
    }

    surface.ContentType =
        ContentType.TEXT_AND_IMAGE;

    surface.Font = "Debug";
    surface.FontSize = displayFontSize;
    surface.TextPadding = displayPadding;
    surface.Alignment = TextAlignment.LEFT;
}


void WriteToDisplay(string text)
{
    if (activeSurface == null)
    {
        return;
    }

    activeSurface.WriteText(
        text,
        false
    );
}


// Small helpers

string Plural(int value)
{
    return value == 1
        ? ""
        : "s";
}