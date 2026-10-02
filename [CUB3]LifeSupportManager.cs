/*
 * INSTRUCTIONS:
 * 
 * For the instructions, go to https://steamcommunity.com/sharedfiles/filedetails/?id=1972224511
 */


// PROPERTIES
public bool isLeakManagementOn;
public bool isProductionOn;
public double minimumOxygenInTanks;
public double maximumOxygenInTanks;
public double minimumHydrogenInTanks;
public double maximumHydrogenInTanks;


MyIni _ini = new MyIni();

// Router
CommandRouter commandRouter;

// View
ScreenManager screenManager;

// Controllers
AirlockController airlockController;
LeakController leakController;
ProductionController productionController;

int maxInstruction = 0;

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update1;

    commandRouter = new CommandRouter(this);
    airlockController =  new AirlockController(this);
    leakController = new LeakController(this);
    productionController = new ProductionController(this);
    screenManager = new ScreenManager(this);

    Configure();
}

private void Configure() {
    if (String.IsNullOrEmpty(Me.CustomData)) {
        SaveConfig(true);

    } else {
        MyIniParseResult result;
        if (!_ini.TryParse(Me.CustomData, out result))
            throw new Exception(result.ToString());

        minimumOxygenInTanks = _ini.Get("Production Management", "MinimumOxygenInTanks").ToDouble();
        maximumOxygenInTanks = _ini.Get("Production Management", "MaximumOxygenInTanks").ToDouble();
        minimumHydrogenInTanks = _ini.Get("Production Management", "MinimumHydrogenInTanks").ToDouble();
        maximumHydrogenInTanks = _ini.Get("Production Management", "MaximumHydrogenInTanks").ToDouble();

        isLeakManagementOn = _ini.Get("Do Not Modify", "LeakStatus").ToBoolean();
        isProductionOn = _ini.Get("Do Not Modify", "ProductionStatus").ToBoolean();
    }

}

private void SaveConfig(bool firstTime) {
    if (firstTime) {
        isLeakManagementOn = true;
        isProductionOn = true;
        minimumOxygenInTanks = 30;
        maximumOxygenInTanks = 70;
        minimumHydrogenInTanks = 30;
        maximumHydrogenInTanks = 70;

        _ini.Set("Production Management", "Start Oxigen Production When Below", minimumOxygenInTanks);
        _ini.Set("Production Management", "Stop Oxygen Production When Over", maximumOxygenInTanks);
        _ini.Set("Production Management", "Start Hydrogen Production When Below", minimumHydrogenInTanks);
        _ini.Set("Production Management", "Stop Hydrogen Production When Over", maximumHydrogenInTanks);

        _ini.Set("Do Not Modify", "LeakStatus", isLeakManagementOn);
        _ini.Set("Do Not Modify", "ProductionStatus", isProductionOn);
    } else {
        MyIniParseResult result;
        if (!_ini.TryParse(Me.CustomData, out result))
            throw new Exception(result.ToString());

        minimumOxygenInTanks = _ini.Get("Production Management", "Start Oxigen Production When Below").ToDouble();
        maximumOxygenInTanks = _ini.Get("Production Management", "Stop Oxygen Production When Over").ToDouble();
        minimumHydrogenInTanks = _ini.Get("Production Management", "Start Hydrogen Production When Below").ToDouble();
        maximumHydrogenInTanks = _ini.Get("Production Management", "Stop Hydrogen Production When Over").ToDouble();

        _ini.Set("Production Management", "Start Oxigen Production When Below", minimumOxygenInTanks);
        _ini.Set("Production Management", "Stop Oxygen Production When Over", maximumOxygenInTanks);
        _ini.Set("Production Management", "Start Hydrogen Production When Below", minimumHydrogenInTanks);
        _ini.Set("Production Management", "Stop Hydrogen Production When Over", maximumHydrogenInTanks);

        _ini.Set("Do Not Modify", "LeakStatus", isLeakManagementOn);
        _ini.Set("Do Not Modify", "ProductionStatus", isProductionOn);
    }

    Me.CustomData = _ini.ToString();
}

public void Save() {
    //SaveConfig(false);
}

public void Main(string argument, UpdateType updateSource) {
    commandRouter.ParseCommand(argument);
    airlockController.AirlockRuntime();
    leakController.LeakRuntime();
    productionController.OxygenRuntime();
    screenManager.ScreenRuntime();
    SaveConfig(false);
    GenerateStats();
}

private void GenerateStats() {
    IMyTextSurface pcScreen = Me.GetSurface(0);
    pcScreen.ContentType = ContentType.TEXT_AND_IMAGE;
    pcScreen.Font = "Monospace";
    pcScreen.FontSize = 0.7f;

    int curInstruction = Runtime.CurrentInstructionCount;
    if (curInstruction > maxInstruction) {
        maxInstruction = curInstruction;
    }

    string leakStatus = isLeakManagementOn ? "ON" : "OFF";
    string prodStatus = isProductionOn ? "ON" : "OFF";

    string text = "";
    text += $"Life Support\nManagement System\nv.1.0\n";
    text += $"\n";
    text += $"-----\n";
    text += $"\n";
    text += $"Airlocks count: {airlockController.Airlocks.Count()}\n";
    text += $"Leak Prevention: {leakStatus}\n";
    text += $"Production Prevention: {prodStatus}\n";
    text += $"\n";
    text += $"-----\n";
    text += $"\n";
    text += $"Instructions: {curInstruction}\n";
    text += $"Max Instructions: {maxInstruction}/{Runtime.MaxInstructionCount}\n";

    pcScreen.WriteText(text);

}

public class Airlock {

    Program myProgram;

    // Generic
    public string Name { get; set; }
    public string Command { get; set; }
    public string Status { get; set; }
    public string PublicStatus { get; set; }
    public bool IsValid { get; set; }

    // Stats
    public string RoomPressure { get; set; }
    public string OxygenLevel { get; set; }
    public string OxygenLevelDecimals { get; set; }
    public string OpenDoors { get; set; }
    public string OxygenTankFill { get; set; }
    public string OxygenTankFillDecimals { get; set; }
    public string Issues { get; set; }

    // Blocks
    public List<IMyAirVent> Airvents { get; set; }
    public List<IMyDoor> Doors { get; set; }
    public List<IMyGasTank> Tanks { get; set; }
    public List<IMyTextPanel> Panels { get; set; }
    public List<IMyTextSurface> Surfaces { get; set; }
    public List<IMyLightingBlock> Lights { get; set; }

    // Generics
    public List<string> Errors { get; set; }
    public List<string> Warnings { get; set; }

    public Airlock(IMyBlockGroup blockGroup, Program program) {
        myProgram = program;
        Errors = new List<string>();
        Warnings = new List<string>();

        Name = blockGroup.Name;
        Airvents = new List<IMyAirVent>();
        Doors = new List<IMyDoor>();
        Tanks = new List<IMyGasTank>();
        Panels = new List<IMyTextPanel>();
        Surfaces = new List<IMyTextSurface>();
        Lights = new List<IMyLightingBlock>();

        // necessary
        blockGroup.GetBlocksOfType(Airvents);
        blockGroup.GetBlocksOfType(Doors);
        blockGroup.GetBlocksOfType(Tanks);

        // not necessary
        blockGroup.GetBlocksOfType(Panels);
        blockGroup.GetBlocksOfType(Surfaces);
        blockGroup.GetBlocksOfType(Lights);

        // init
        Status = Constants.A_IDLE;
        PublicStatus = Constants.AP_IDLE;

        // write CustomData on doors and airvents
        foreach (IMyAirVent airvent in Airvents) {
            airvent.CustomData = Constants.T_LSM_AIRLOCK_AIRVENT;
        }
        foreach (IMyDoor door in Doors) {
            if (!door.CustomData.Contains(Constants.T_LSM_AIRLOCK_DOOR)) {
                string temp = door.CustomData;
                door.CustomData = Constants.T_LSM_AIRLOCK_DOOR + " " + temp;
            }
        }
    }

    public bool IsAirlockValid() {
        // Errors
        if (Utils.IsListEmpty(Airvents)) {
            string error = "ERROR: No airvents found.\n";
            myProgram.Echo(error);
            Errors.Add(error);
        }
        if (Utils.IsListEmpty(Doors)) {
            string error = "ERROR: No doors found.\n";
            myProgram.Echo(error);
            Errors.Add(error);
        }
        if (Utils.IsListEmpty(Tanks)) {
            string error = "ERROR: No oxygen tanks found.\n";
            myProgram.Echo(error);
            Errors.Add(error);
        }

        // Warnings
        if (Utils.IsListEmpty(Panels) || Utils.IsListEmpty(Surfaces)) {
            string warning = "WARNING: No screens found.\n";
            myProgram.Echo(warning);
            Warnings.Add(warning);
        }
        if (Utils.IsListEmpty(Lights)) {
            string warning = "WARNING: No lights found.\n";
            myProgram.Echo(warning);
            Warnings.Add(warning);
        }

        if (!Utils.IsListEmpty(Errors)) {
            foreach (string error in Errors) {
                myProgram.Echo(error);
            }
            myProgram.Echo($"There has been an error with {Name}, please fix your airlock\n");
            return false;
        } else {
            return true;
        }
    }

    public void GetAirlockDetails() {
        UpdateOxygenStatus();
        UpdateDoorStatus();
        UpdateTanksStatus();
    }

    private void UpdateOxygenStatus() {
        float oxygenLevel = 0;
        foreach (IMyAirVent airvent in Airvents) {
            oxygenLevel += airvent.GetOxygenLevel();
        }
        oxygenLevel = oxygenLevel / Airvents.Count;
        OxygenLevel = oxygenLevel.ToString("0.0");
        OxygenLevelDecimals = oxygenLevel.ToString("0.000");
        if (oxygenLevel >= 0.7) {
            RoomPressure = "High Pressure";
        } else if (oxygenLevel < 0.7 && oxygenLevel >= 0.1) {
            RoomPressure = "Low Pressure";
        } else {
            RoomPressure = "No Pressure";
        }

    }

    private void UpdateDoorStatus() {
        bool areInternalDoorsOpen = false;
        bool areExternalDoorsOpen = false;
        foreach (IMyDoor door in Doors) {
            if (door.Status.Equals(DoorStatus.Open) || door.Status.Equals(DoorStatus.Opening)) {
                if (door.CustomData.Contains(Constants.DOOR_INTERNAL)) {
                    areInternalDoorsOpen = true;
                } else if (door.CustomData.Contains(Constants.DOOR_EXTERNAL)) {
                    areExternalDoorsOpen = true;
                }
            }
        }
        if (areInternalDoorsOpen && areExternalDoorsOpen) {
            OpenDoors = "Both";
        } else if (areInternalDoorsOpen && !areExternalDoorsOpen) {
            OpenDoors = "Internal";
        } else if (!areInternalDoorsOpen && areExternalDoorsOpen) {
            OpenDoors = "External";
        } else {
            OpenDoors = "None";
        }
    }

    private void UpdateTanksStatus() {
        double filledRatio = 0;
        foreach (IMyGasTank tank in Tanks) {
            filledRatio += tank.FilledRatio;
        }
        filledRatio = (filledRatio / Tanks.Count)*100;
        OxygenTankFill = filledRatio.ToString("0.0");
        OxygenTankFillDecimals = (filledRatio / 100).ToString("0.000");
    }



}

public class AirlockController {

    Program myProgram;

    List<IMyBlockGroup> airlockGroups = new List<IMyBlockGroup>();

    public Dictionary<string, Airlock> Airlocks { get; set; }

    Dictionary<string, string[]> airlocksToCycle = new Dictionary<string, string[]>();

    List<string> airlocksToRemove = new List<string>();

    public AirlockController(Program program) {
        myProgram = program;
        Init();
    }

    public void Init() {
        if (null == Airlocks) {
            Airlocks = new Dictionary<string, Airlock>();
        }
        myProgram.GridTerminalSystem.GetBlockGroups(airlockGroups, group => group.Name.Contains("Airlock"));
        if (airlockGroups.Count == 0) {
            myProgram.Echo("Warning, there are no valid airlocks on your ship / station");
        } else {
            foreach (IMyBlockGroup airlockGroup in airlockGroups) {
                Airlock airlock = new Airlock(airlockGroup, myProgram);
                if (airlock.IsAirlockValid()) {
                    Airlocks.Add(airlockGroup.Name, airlock);
                }
            }
        }
    }

    public void AddCommandToStack(MyCommandLine _commandLine) {
        if (Airlocks.Count > 0) {
            if (null != _commandLine.Argument(1) && (_commandLine.Argument(1).Equals(Constants.P_DEPRESSURIZE) || _commandLine.Argument(1).Equals(Constants.P_PRESSURIZE)) && null != _commandLine.Argument(2)) {
                Airlock airlock = null;
                try {
                    airlock = Airlocks[_commandLine.Argument(2)];
                    if (null != airlock && airlock.IsAirlockValid()) {
                        myProgram.Echo($"Command added for airlock: {airlock.Name}\n");
                        string[] commandLineArray = new string[2];
                        commandLineArray[0] = _commandLine.Argument(1);
                        commandLineArray[1] = _commandLine.Argument(2);
                        airlocksToCycle[commandLineArray[1]] = commandLineArray;
                    } else {
                        myProgram.Echo($"No valid airlock with name {_commandLine.Argument(2)} found.\n");
                    }
                } catch (Exception e) {
                    myProgram.Echo($"No valid airlock with name {_commandLine.Argument(2)} found.\n");
                }
            } else {
                myProgram.Echo("No valid parameters specified\n");
            }
        } else {
            myProgram.Echo("There are no airlocks to cycle\n");
        }
    }

    public void AirlockRuntime() {
        CheckAirlockCycling();
        UpdateAirlock();
    }

    private void CheckAirlockCycling() {
        if (airlocksToCycle.Count != 0) {
            foreach (KeyValuePair<string, string[]> airlockToCycle in airlocksToCycle) {
                if (CanCycleAirlock(Airlocks[airlockToCycle.Key], airlockToCycle.Value[0])) {
                    Cycle(Airlocks[airlockToCycle.Key], airlockToCycle.Value[0]);
                } else {
                    Airlocks[airlockToCycle.Key].PublicStatus = Constants.AP_ERROR;
                }
            }

            if (airlocksToRemove.Count != 0) {
                foreach (string airlock in airlocksToRemove) {
                    airlocksToCycle.Remove(airlock);
                }
                airlocksToRemove.Clear();
            }
        }
    }

    private bool CanCycleAirlock(Airlock airlock, string command) {
        bool canCycle = true;
        foreach (IMyAirVent airvent in airlock.Airvents) {
            if (!airvent.IsFunctional) {
                airlock.Errors.Add($"Airvent {airvent.CustomName} is broken or not fully built\n");
                canCycle = false;
            }
            if (!airvent.IsWorking) {
                airlock.Errors.Add($"Airvent {airvent.CustomName} is not powered\n");
                canCycle = false;
            }
        }
        foreach (IMyDoor door in airlock.Doors) {
            if (!door.IsFunctional) {
                airlock.Errors.Add($"Door {door.CustomName} is broken or not fully built\n");
                canCycle = false;
            }
            if (!door.IsWorking) {
                airlock.Errors.Add($"Door {door.CustomName} is not powered\n");
                canCycle = false;
            }
        }
        if (command.Equals(Constants.P_PRESSURIZE)) {
            bool emptyTanks = true;
            foreach (IMyGasTank tank in airlock.Tanks) {
                if (tank.FilledRatio > 0.05) {
                    emptyTanks = false;
                }
            }
            if (emptyTanks) {
                airlock.Errors.Add($"Not enough oxygen in tanks\n");
                canCycle = false;
            }
        }
        return canCycle;
    }

    private void Cycle(Airlock airlock, string command) {
        switch (airlock.Status) {
            case Constants.A_IDLE:
                airlock.Status = Constants.A_CLOSE_DOORS;
                airlock.PublicStatus = Constants.AP_CYCLE;
                break;
            case Constants.A_CLOSE_DOORS:
                CloseDoors(airlock);
                break;
            case Constants.A_CYCLE:
                CyclePressure(airlock, command);
                break;
            case Constants.A_OPEN_DOORS:
                OpenDoors(airlock, command);
                break;
            case Constants.A_COMPLETED:
                CompleteCycle(airlock, command);
                break;
        }
    }

    private void CloseDoors(Airlock airlock) {
        bool doorsAreClosed = true;
        foreach (IMyDoor door in airlock.Doors) {
            if (!door.Status.Equals(DoorStatus.Closed) && !door.Status.Equals(DoorStatus.Closing)) {
                door.CloseDoor();
            }
            if (!door.Status.Equals(DoorStatus.Closed)) {
                doorsAreClosed = false;
            }
        }
        if (doorsAreClosed) {
            airlock.Status = Constants.A_CYCLE;
        }
    }

    private void CyclePressure(Airlock airlock, string command) {
        bool canOpenDoors = true;

        foreach (IMyAirVent airvent in airlock.Airvents) {
            if (command.Equals(Constants.P_DEPRESSURIZE)) {
                airvent.Depressurize = true;
                if (airvent.GetOxygenLevel() != 0) {
                    canOpenDoors = false;
                }
            } else if (command.Equals(Constants.P_PRESSURIZE)) {
                airvent.Depressurize = false;
                if (airvent.GetOxygenLevel() != 1) {
                    canOpenDoors = false;
                }
            }
        }

        if (canOpenDoors) {
            airlock.Status = Constants.A_OPEN_DOORS;
        }
    }


    private void OpenDoors(Airlock airlock, string command) {
        bool isDoorOpen = false;
        foreach (IMyDoor door in airlock.Doors) {
            if (command.Equals(Constants.P_DEPRESSURIZE)) {
                if (door.CustomData.Contains(Constants.DOOR_EXTERNAL)) {
                    if (!door.Status.Equals(DoorStatus.Open) && !door.Status.Equals(DoorStatus.Opening)) {
                        door.OpenDoor();
                    }
                    if (!door.Status.Equals(DoorStatus.Open)) {
                        isDoorOpen = true;
                    }
                }
            } else if (command.Equals(Constants.P_PRESSURIZE)) {
                if (door.CustomData.Contains(Constants.DOOR_INTERNAL)) {
                    if (!door.Status.Equals(DoorStatus.Open) && !door.Status.Equals(DoorStatus.Opening)) {
                        door.OpenDoor();
                    }
                    if (!door.Status.Equals(DoorStatus.Open)) {
                        isDoorOpen = true;
                    }
                }
            }
        }
        if (!isDoorOpen) {
            airlock.Status = Constants.A_COMPLETED;
        }
    }

    private void CompleteCycle(Airlock airlock, string command) {
        airlocksToRemove.Add(airlock.Name);
        airlock.Status = Constants.A_IDLE;
        if (command.Equals(Constants.P_DEPRESSURIZE)) {
            airlock.PublicStatus = Constants.AP_DEPRESSURIZED;
        } else if (command.Equals(Constants.P_PRESSURIZE)) {
            airlock.PublicStatus = Constants.AP_PRESSURIZED;
        }
    }

    private void UpdateAirlock() {
        foreach (KeyValuePair<string, Airlock> airlock in Airlocks) {
            ChangeAirlockLightColor(airlock.Value);
            UpdateAirlockInfos(airlock.Value);
        }
    }

    private void ChangeAirlockLightColor(Airlock airlock) {
        int colorNum = (int)Math.Round(airlock.Airvents[0].GetOxygenLevel() * 255);
        foreach (IMyLightingBlock light in airlock.Lights) {
            light.Color = new Color(255, colorNum, colorNum);
        }
    }

    private void UpdateAirlockInfos(Airlock airlock) {
        airlock.GetAirlockDetails();
    }





}

public class AirlockScreen {

    Program myProgram;


    ScreenManager ScreenManager;

    public AirlockScreen(ScreenManager sManager, Program program) {
        ScreenManager = sManager;

        myProgram = program;
    }

    public void GenerateScreen(Dictionary<string, Airlock> airlocks) {

        foreach (KeyValuePair<string, Airlock> _al in airlocks) {
            Airlock airlock = _al.Value;

            foreach (IMyTextPanel panel in airlock.Panels) {
                using (var frame = panel.DrawFrame()) {

                    List<MySprite> backgroundSpriteList = new List<MySprite>();
                    List<MySprite> pressureInfoSpriteList = new List<MySprite>();
                    List<MySprite> oxygenInfoSpriteList = new List<MySprite>();
                    List<MySprite> pressureButtonSpriteList = new List<MySprite>();
                    List<MySprite> doorButtonSpriteList = new List<MySprite>();
                    List<MySprite> footerSpriteList = new List<MySprite>();


                    DrawBackground(airlock, backgroundSpriteList);

                    DrawPressureInfo(airlock, pressureInfoSpriteList);

                    DrawPressureButton(airlock, pressureButtonSpriteList);

                    DrawOxygenInfo(airlock, oxygenInfoSpriteList);

                    DrawDoorButtons(airlock, doorButtonSpriteList);

                    DrawFooter(airlock, footerSpriteList);

                    // SPRITES TO FRAME

                    frame.AddRange(backgroundSpriteList);
                    frame.AddRange(pressureInfoSpriteList);
                    frame.AddRange(pressureButtonSpriteList);
                    frame.AddRange(oxygenInfoSpriteList);
                    frame.AddRange(doorButtonSpriteList);
                    frame.AddRange(footerSpriteList);
                }
            }
        }
    }

    private void DrawBackground(Airlock airlock, List<MySprite> backgroundSpriteList) {
        Vector2 b_pos = new Vector2(256, 256);
        Vector2 b_size = new Vector2(512, 512);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = Constants.COLOR_BACKGROUND_MASK;

        Vector2 bc_pos = new Vector2(256, 150);
        Vector2 bc_size = new Vector2(280, 280);
        var backgroundCircle = MySprite.CreateSprite("Circle", bc_pos, bc_size);
        backgroundCircle.Color = Constants.COLOR_BACKGROUND;

        Vector2 bcr_pos = new Vector2(256, 150);
        Vector2 bcr_size = new Vector2(512, 170);
        var backgroundCircleRect = MySprite.CreateSprite("SquareSimple", bcr_pos, bcr_size);
        backgroundCircleRect.Color = Constants.COLOR_BACKGROUND;

        backgroundSpriteList.Add(background);
        backgroundSpriteList.Add(backgroundCircle);
        backgroundSpriteList.Add(backgroundCircleRect);

    }

    private void DrawPressureInfo(Airlock airlock, List<MySprite> pressureInfoSpriteList) {
        Color contrastColor = new Color();
        if (airlock.RoomPressure.Equals("High Pressure")) {
            contrastColor = Constants.COLOR_GREEN;
        } else if (airlock.RoomPressure.Equals("Low Pressure")) {
            contrastColor = Constants.COLOR_YELLOW;
        } else {
            contrastColor = Constants.COLOR_RED;
        }

        Vector2 oc_pos = new Vector2(256, 150);
        Vector2 oc_size = new Vector2(240, 240);
        var outerCircle = MySprite.CreateSprite("Circle", oc_pos, oc_size);
        outerCircle.Color = contrastColor;

        Vector2 ocm_pos = new Vector2(256, 150);
        Vector2 ocm_size = new Vector2(230, 230);
        var outerCircleMask = MySprite.CreateSprite("Circle", ocm_pos, ocm_size);
        outerCircleMask.Color = Constants.COLOR_BACKGROUND_MASK;

        var pressureText = MySprite.CreateText(airlock.OxygenLevel, "Debug", Constants.COLOR_WHITE, 4f, TextAlignment.CENTER);
        pressureText.Position = new Vector2(256, 90);

        var barText = MySprite.CreateText("BAR", "Debug", contrastColor, 1f, TextAlignment.CENTER);
        barText.Position = new Vector2(256, 200);

        pressureInfoSpriteList.Add(outerCircle);
        pressureInfoSpriteList.Add(outerCircleMask);
        pressureInfoSpriteList.Add(pressureText);
        pressureInfoSpriteList.Add(barText);
    }

    private void DrawPressureButton(Airlock airlock, List<MySprite> pressureButtonSpriteList) {
        Vector2 buttonSize = new Vector2(130, 50); //136 48
        Vector2 frameSize = new Vector2(130, 50); //136 48
        Vector2 frameMaskSize = new Vector2(126, 46); //132 44
        Vector2 frameMaskSizeH = new Vector2(130, 30); // 136 28
        Vector2 frameMaskSizeV = new Vector2(110, 50); // 116 48

        // 130*50

        Vector2 bbl_pos = new Vector2(75, 330);
        var buttonBgLeft = MySprite.CreateSprite("SquareSimple", bbl_pos, buttonSize);
        buttonBgLeft.Color = Constants.COLOR_NAVY_BLUE;

        Vector2 bbc_pos = new Vector2(256, 330);
        var buttonBgCenter = MySprite.CreateSprite("SquareSimple", bbc_pos, buttonSize);
        buttonBgCenter.Color = Constants.COLOR_NAVY_BLUE;

        Vector2 bbr_pos = new Vector2(437, 330);
        var buttonBgRight = MySprite.CreateSprite("SquareSimple", bbr_pos, buttonSize);
        buttonBgRight.Color = Constants.COLOR_NAVY_BLUE;

        var frameButton = MySprite.CreateSprite("SquareSimple", bbc_pos, frameSize);
        var frameButtonMask = MySprite.CreateSprite("SquareSimple", bbc_pos, frameMaskSize);
        frameButtonMask.Color = Constants.COLOR_NAVY_BLUE;
        var frameButtonMaskH = MySprite.CreateSprite("SquareSimple", bbc_pos, frameMaskSizeH);
        frameButtonMaskH.Color = Constants.COLOR_NAVY_BLUE;
        var frameButtonMaskV = MySprite.CreateSprite("SquareSimple", bbc_pos, frameMaskSizeV);
        frameButtonMaskV.Color = Constants.COLOR_NAVY_BLUE;
        if (airlock.PublicStatus.Equals(Constants.AP_PRESSURIZED)) {
            frameButton.Color = Constants.COLOR_GREEN;
            frameButtonMask.Color = Constants.COLOR_GREEN_DARK;
            frameButtonMaskH.Color = Constants.COLOR_GREEN_DARK;
            frameButtonMaskV.Color = Constants.COLOR_GREEN_DARK;
            frameButton.Position = bbl_pos;
            frameButtonMask.Position = bbl_pos;
            frameButtonMaskH.Position = bbl_pos;
            frameButtonMaskV.Position = bbl_pos;
        } else if (airlock.PublicStatus.Equals(Constants.AP_CYCLE)) {
            frameButton.Color = Constants.COLOR_YELLOW;
            frameButtonMask.Color = Constants.COLOR_YELLOW_DARK;
            frameButtonMaskH.Color = Constants.COLOR_YELLOW_DARK;
            frameButtonMaskV.Color = Constants.COLOR_YELLOW_DARK;
            frameButton.Position = bbc_pos;
            frameButtonMask.Position = bbc_pos;
            frameButtonMaskH.Position = bbc_pos;
            frameButtonMaskV.Position = bbc_pos;
        } else if (airlock.PublicStatus.Equals(Constants.AP_DEPRESSURIZED)) {
            frameButton.Color = Constants.COLOR_RED;
            frameButtonMask.Color = Constants.COLOR_RED_DARK;
            frameButtonMaskH.Color = Constants.COLOR_RED_DARK;
            frameButtonMaskV.Color = Constants.COLOR_RED_DARK;
            frameButton.Position = bbr_pos;
            frameButtonMask.Position = bbr_pos;
            frameButtonMaskH.Position = bbr_pos;
            frameButtonMaskV.Position = bbr_pos;
        }

        var bblText = MySprite.CreateText("PRESSURIZED", "Debug", Constants.COLOR_WHITE, 0.6f, TextAlignment.CENTER);
        bblText.Position = new Vector2(75, 320);

        var bbcText = MySprite.CreateText("CYCLING", "Debug", Constants.COLOR_WHITE, 0.6f, TextAlignment.CENTER);
        bbcText.Position = new Vector2(256, 320);

        var bbrText = MySprite.CreateText("DEPRESSURIZED", "Debug", Constants.COLOR_WHITE, 0.6f, TextAlignment.CENTER);
        bbrText.Position = new Vector2(437, 320);

        pressureButtonSpriteList.Add(buttonBgCenter);
        pressureButtonSpriteList.Add(buttonBgLeft);
        pressureButtonSpriteList.Add(buttonBgRight);
        pressureButtonSpriteList.Add(frameButton);
        pressureButtonSpriteList.Add(frameButtonMask);
        pressureButtonSpriteList.Add(frameButtonMaskH);
        pressureButtonSpriteList.Add(frameButtonMaskV);
        pressureButtonSpriteList.Add(bbcText);
        pressureButtonSpriteList.Add(bblText);
        pressureButtonSpriteList.Add(bbrText);
    }

    private void DrawOxygenInfo(Airlock airlock, List<MySprite> oxygenInfoSpriteList) {
        Vector2 orl_pos = new Vector2(112.5f, 390);
        Vector2 orl_size = new Vector2(205, 30);
        var outerRectLeft = MySprite.CreateSprite("SquareSimple", orl_pos, orl_size);
        outerRectLeft.Color = Constants.COLOR_WHITE;

        Vector2 orlm_pos = new Vector2(112.5f, 390);
        Vector2 orlm_size = new Vector2(201, 26);
        var outerRectLeftMask = MySprite.CreateSprite("SquareSimple", orlm_pos, orlm_size);
        outerRectLeftMask.Color = Constants.COLOR_BACKGROUND;

        Vector2 bfl_pos = new Vector2(112.5f, 390);
        Vector2 bfl_size = new Vector2((197 * float.Parse(airlock.OxygenLevelDecimals)), 22);
        var barFillLeft = MySprite.CreateSprite("SquareSimple", bfl_pos, bfl_size);
        barFillLeft.Color = Constants.COLOR_WHITE;

        var oxigenText = MySprite.CreateText("AIRLOCK OXYGEN", "Debug", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        oxigenText.Position = new Vector2(112.5f, 410);

        Vector2 orr_pos = new Vector2(399.5f, 390);
        Vector2 orr_size = new Vector2(205, 30);
        var outerRectRight = MySprite.CreateSprite("SquareSimple", orr_pos, orr_size);
        outerRectRight.Color = Constants.COLOR_WHITE;

        Vector2 orrm_pos = new Vector2(399.5f, 390);
        Vector2 orrm_size = new Vector2(201, 26);
        var outerRectRightMask = MySprite.CreateSprite("SquareSimple", orrm_pos, orrm_size);
        outerRectRightMask.Color = Constants.COLOR_BACKGROUND;

        Vector2 bfr_pos = new Vector2(399.5f, 390);
        Vector2 bfr_size = new Vector2((197 * float.Parse(airlock.OxygenTankFillDecimals)), 22);
        var barFillRight = MySprite.CreateSprite("SquareSimple", bfr_pos, bfr_size);
        barFillRight.Color = Constants.COLOR_WHITE;

        var tankText = MySprite.CreateText("02 TANKS STORAGE", "Debug", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        tankText.Position = new Vector2(399.5f, 410);

        oxygenInfoSpriteList.Add(outerRectLeft);
        oxygenInfoSpriteList.Add(outerRectLeftMask);
        oxygenInfoSpriteList.Add(barFillLeft);
        oxygenInfoSpriteList.Add(oxigenText);

        oxygenInfoSpriteList.Add(outerRectRight);
        oxygenInfoSpriteList.Add(outerRectRightMask);
        oxygenInfoSpriteList.Add(barFillRight);
        oxygenInfoSpriteList.Add(tankText);
    }

    private void DrawDoorButtons(Airlock airlock, List<MySprite> doorButtonSpriteList) {

        Color internalColor = new Color(255, 255, 255);
        Color externalColor = new Color(255, 255, 255);

        if (airlock.OpenDoors.Equals("Both")) {
            internalColor = Constants.COLOR_GREEN;
            externalColor = Constants.COLOR_GREEN;
        } else if (airlock.OpenDoors.Equals("Internal")) {
            internalColor = Constants.COLOR_GREEN;
            externalColor = Constants.COLOR_RED;
        } else if (airlock.OpenDoors.Equals("External")) {
            internalColor = Constants.COLOR_RED;
            externalColor = Constants.COLOR_GREEN;
        } else {
            internalColor = Constants.COLOR_RED;
            externalColor = Constants.COLOR_RED;
        }

        Vector2 ldr_pos = new Vector2(70, 150);
        Vector2 ldr_size = new Vector2(60, 60);
        var leftDoorRect = MySprite.CreateSprite("SquareSimple", ldr_pos, ldr_size);
        leftDoorRect.Color = Constants.COLOR_WHITE;

        Vector2 ldrm_pos = new Vector2(70, 150);
        Vector2 ldrm_size = new Vector2(56, 56);
        var leftDoorRectMask = MySprite.CreateSprite("SquareSimple", ldrm_pos, ldrm_size);
        leftDoorRectMask.Color = Constants.COLOR_BACKGROUND;

        Vector2 lds_pos = new Vector2(70, 187);
        Vector2 lds_size = new Vector2(60, 10);
        var leftDoorSignal = MySprite.CreateSprite("SquareSimple", lds_pos, lds_size);
        leftDoorSignal.Color = internalColor;

        var innerDoorText = MySprite.CreateText("INNER\nDOOR", "Debug", internalColor, 0.6f, TextAlignment.CENTER);
        innerDoorText.Position = new Vector2(70, 133);

        Vector2 rdr_pos = new Vector2(437, 150);
        Vector2 rdr_size = new Vector2(60, 60);
        var rightDoorRect = MySprite.CreateSprite("SquareSimple", rdr_pos, rdr_size);
        rightDoorRect.Color = Constants.COLOR_WHITE;

        Vector2 rdrm_pos = new Vector2(437, 150);
        Vector2 rdrm_size = new Vector2(56, 56);
        var rightDoorRectMask = MySprite.CreateSprite("SquareSimple", rdrm_pos, rdrm_size);
        rightDoorRectMask.Color = Constants.COLOR_BACKGROUND;

        Vector2 rds_pos = new Vector2(437, 187);
        Vector2 rds_size = new Vector2(60, 10);
        var rightDoorSignal = MySprite.CreateSprite("SquareSimple", rds_pos, rds_size);
        rightDoorSignal.Color = externalColor;

        var outerDoorText = MySprite.CreateText("OUTER\nDOOR", "Debug", externalColor, 0.6f, TextAlignment.CENTER);
        outerDoorText.Position = new Vector2(437, 133);

        doorButtonSpriteList.Add(leftDoorRect);
        doorButtonSpriteList.Add(leftDoorRectMask);
        doorButtonSpriteList.Add(leftDoorSignal);
        doorButtonSpriteList.Add(innerDoorText);
        doorButtonSpriteList.Add(rightDoorRect);
        doorButtonSpriteList.Add(rightDoorRectMask);
        doorButtonSpriteList.Add(rightDoorSignal);
        doorButtonSpriteList.Add(outerDoorText);

    }

    private void DrawFooter(Airlock airlock, List<MySprite> footerSpriteList) {

        Vector2 fb_pos = new Vector2(256, 457.5f);
        Vector2 fb_size = new Vector2(492, 5);
        var footerBar = MySprite.CreateSprite("SquareSimple", fb_pos, fb_size);
        footerBar.Color = Constants.COLOR_ORANGE;

        var airlockNameText = MySprite.CreateText(airlock.Name, "Debug", Constants.COLOR_ORANGE, 1.5f, TextAlignment.CENTER);
        airlockNameText.Position = new Vector2(256, 462f);

        footerSpriteList.Add(footerBar);
        footerSpriteList.Add(airlockNameText);

    }




}

public class CommandRouter {

    Program myProgram;

    MyCommandLine _commandLine = new MyCommandLine();

    public CommandRouter(Program program) {
        myProgram = program;
    }

    public void ParseCommand(string argument) {
        if (_commandLine.TryParse(argument)) {
            switch (_commandLine.Argument(0)) {
                case Constants.C_CYCLE:
                    myProgram.airlockController.AddCommandToStack(_commandLine);
                    break;
                case Constants.C_LEAK:
                    myProgram.leakController.AddCommandToStack(_commandLine);
                    break;
                case Constants.C_SLIDE:
                    myProgram.productionController.AddCommandToStack(_commandLine);
                    break;
                default:
                    myProgram.Echo($"No valid command specified: {_commandLine.Argument(0)}");
                    break;
            }
        }
    }

}

public class Constants {

    // TAGS
    public const string T_LSM = "[LSM]";
    public const string T_LSM_AIRLOCK_DOOR = "[LSM Airlock Door]";
    public const string T_LSM_AIRLOCK_AIRVENT = "[LSM Airlock Airvent]";
    public const string T_LSM_AIRLOCK_SCREEN = "[LSM Airlock Screen]";
    public const string T_LSM_AIRVENT_SCREEN = "[LSM Airvent Screen]";
    public const string T_LSM_PROD_SCREEN = "[LSM Production Screen]";

    // COMMANDS
    public const string C_CYCLE = "cycle";
    public const string C_LEAK = "leak";
    public const string C_SLIDE = "prod";

    // PARAMETERS
    public const string P_DEPRESSURIZE = "depressurize";
    public const string P_PRESSURIZE = "pressurize";
    public const string P_ON = "on";
    public const string P_OFF = "off";

    // AIRLOCK STATUS
    public const string A_IDLE = "idle";
    public const string A_CLOSE_DOORS = "close_doors";
    public const string A_CYCLE = "cycle";
    public const string A_OPEN_DOORS = "open_doors";
    public const string A_COMPLETED = "completed";

    // AIRLOCK PUBLIC STATUS
    public const string AP_IDLE = "IDLE";
    public const string AP_CYCLE = "CYCLING";
    public const string AP_PRESSURIZED = "PRESSURIZED";
    public const string AP_DEPRESSURIZED = "DEPRESSURIZED";
    public const string AP_ERROR = "ERROR";

    // DOOR
    public const string DOOR_INTERNAL = "internal";
    public const string DOOR_EXTERNAL = "external";

    // SCREENS
    public const string S_STATUS_INIT = "Initializing";
    public const string S_STATUS_RUNNING = "Running";

    // COLORS
    public static Color COLOR_BACKGROUND = new Color(27, 28, 33);
    public static Color COLOR_BACKGROUND_MASK = new Color(37, 39, 45);
    public static Color COLOR_BACKGROUND_LIGHT = new Color(67, 70, 81);
    public static Color COLOR_LOGO_PRIMARY = new Color(255, 217, 131);
    public static Color COLOR_LOGO_SECONDARY = new Color(132, 83, 47);

    public static Color COLOR_WHITE = new Color(256, 256, 256);
    public static Color COLOR_GREEN = new Color(29, 229, 128);
    public static Color COLOR_GREEN_DARK = new Color(7, 54, 30);
    public static Color COLOR_YELLOW = new Color(229, 157, 17);
    public static Color COLOR_YELLOW_DARK = new Color(71, 49, 5);
    public static Color COLOR_RED = new Color(255, 4, 99);
    public static Color COLOR_RED_DARK = new Color(73, 1, 28);
    public static Color COLOR_NAVY_BLUE = new Color(37, 38, 45);
    public static Color COLOR_ORANGE = new Color(255, 141, 18);

}

public class LeakController {

    Program myProgram;

    private List<IMyDoor> doors = new List<IMyDoor>();
    private List<IMyAirVent> airvents = new List<IMyAirVent>();

    public List<IMyDoor> Doors { get; set; }
    public List<IMyAirVent> Airvents { get; set; }

    public LeakController(Program program) {
        myProgram = program;
        Init();
    }

    private void Init() {
        myProgram.GridTerminalSystem.GetBlocksOfType(doors);
        myProgram.GridTerminalSystem.GetBlocksOfType(airvents);
        Doors = new List<IMyDoor>();
        Airvents = new List<IMyAirVent>();
        foreach (IMyAirVent airvent in airvents) {
            if (!airvent.CustomData.Equals(Constants.T_LSM_AIRLOCK_AIRVENT)) {
                Airvents.Add(airvent);
            }
        }
        foreach (IMyDoor door in doors) {
            if (!door.CustomData.Equals(Constants.T_LSM_AIRLOCK_DOOR)) {
                Doors.Add(door);
            }
        }
        myProgram.isLeakManagementOn = true;
    }

    public void AddCommandToStack(MyCommandLine _commandLine) {
        if (null != _commandLine.Argument(1)) {
            if (_commandLine.Argument(1).Equals(Constants.P_ON)) {
                myProgram.isLeakManagementOn = true;
            } else if (_commandLine.Argument(1).Equals(Constants.P_OFF)) {
                myProgram.isLeakManagementOn = false;
            } else {
                myProgram.Echo("No valid command\n");
            }
        }
    }

    public void LeakRuntime() {
        if (myProgram.isLeakManagementOn) {
            CheckAndManageLeaks();
        }
    }

    private void CheckAndManageLeaks() {
        foreach(IMyAirVent airvent in Airvents) {
            if (!airvent.CanPressurize) {
                foreach (IMyDoor door in Doors) {
                    door.CloseDoor();
                }
            }
        }
    }
}

public class LeakScreen {

    Program myProgram;

    List<IMyTextPanel> Panels;
    List<IMyAirVent> Airvents;

    ScreenManager ScreenManager;

    private float rotationRadiants = 0;

    public LeakScreen(Program program, ScreenManager sManager, List<IMyTextPanel> panels, List<IMyTextPanel> doublePanels, List<IMyAirVent> airvents) {
        myProgram = program;
        ScreenManager = sManager;
        Panels = panels;
        Panels.AddList(doublePanels);
        Airvents = airvents;
    }

    public void GenerateScreen() {
        foreach (IMyTextPanel panel in Panels) {

            int page = 1;
            if (panel.CustomData.Contains("page")) {
                int sIndex = panel.CustomData.IndexOf("=");
                string pString = panel.CustomData.Substring(sIndex + 1);
                page = Int32.Parse(pString);
            }

            using (var frame = panel.DrawFrame()) {
                AddElementsToPanel(page, frame);

            }
        }
    }

    public void AddElementsToPanel(int page, MySpriteDrawFrame frame) {
        List<MySprite> backgroundSpriteList = new List<MySprite>();
        List<List<MySprite>> airventRowList = new List<List<MySprite>>();
        List<MySprite> footerSpriteList = new List<MySprite>();

        DrawBackground(backgroundSpriteList);

        DrawAirvents(Airvents, airventRowList, page);

        DrawFooter(Airvents.Count, footerSpriteList, page);

        frame.AddRange(backgroundSpriteList);
        foreach (List<MySprite> airventRowSprites in airventRowList) {
            frame.AddRange(airventRowSprites);
        }
        frame.AddRange(footerSpriteList);
    }

    private void DrawBackground(List<MySprite> backgroundSpriteList) {
        Vector2 b_pos = new Vector2(256, 256);
        Vector2 b_size = new Vector2(512, 512);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = Constants.COLOR_BACKGROUND;

        backgroundSpriteList.Add(background);
    }

    private void DrawAirvents(List<IMyAirVent> airvents, List<List<MySprite>> airventRowList, int page) {
        float leftColumnX = 130.5f;
        float rightColumnX = 380.5f;
        float startColumnY = 43;
        float columnYincrement = 76;


        int maxPageIndex = page * 10;
        int minPageIndex = maxPageIndex - 10;
        int pageIndex = 0;



        int index = 0;
        int halfIndex = 0;

        foreach (IMyAirVent airvent in airvents) {
            if (pageIndex >= minPageIndex && pageIndex < maxPageIndex) {
                Color stateColor = new Color(256, 256, 256);
                bool isWorking = true;
                if (airvent.CanPressurize && airvent.IsWorking && airvent.IsFunctional) {
                    stateColor = Constants.COLOR_WHITE;
                    isWorking = true;
                } else {
                    stateColor = Constants.COLOR_RED;
                    isWorking = false;
                }

                List<MySprite> airventRow = new List<MySprite>();

                float x;
                float y;

                if (index % 2 == 0) {
                    x = leftColumnX;
                    y = startColumnY + (columnYincrement * halfIndex);
                } else {
                    x = rightColumnX;
                    y = startColumnY + (columnYincrement * halfIndex);
                }

                MySprite sectionBackground;
                DrawSectionBackground(x, y, out sectionBackground);

                MySprite iconBorder, iconBackground, fanH, fanV, fanCenterBackground, fanCenter;
                DrawAirventIcon(x - 101.5f, y - 14, stateColor, isWorking, out iconBorder, out iconBackground, out fanH, out fanV, out fanCenterBackground, out fanCenter);

                MySprite airventText, divider, statusTitle, statusInfo, oxygenTitle, oxygenInfo, pressurizeTitle, pressurizeInfo;
                DrawAirventInfos(x, y, stateColor, isWorking, airvent, out airventText, out divider, out statusTitle, out statusInfo, out oxygenTitle, out oxygenInfo, out pressurizeTitle, out pressurizeInfo);

                airventRow.Add(sectionBackground);

                airventRow.Add(iconBorder);
                airventRow.Add(iconBackground);
                airventRow.Add(fanH);
                airventRow.Add(fanV);
                airventRow.Add(fanCenterBackground);
                airventRow.Add(fanCenter);

                airventRow.Add(airventText);
                airventRow.Add(divider);
                airventRow.Add(statusTitle);
                airventRow.Add(statusInfo);
                airventRow.Add(oxygenTitle);
                airventRow.Add(oxygenInfo);
                airventRow.Add(pressurizeTitle);
                airventRow.Add(pressurizeInfo);

                airventRowList.Add(airventRow);

                if (index % 2 == 1) {
                    halfIndex++;
                }
                index++;
                pageIndex++;
            } else {
                pageIndex++;
                index++;
            }
        }
    }

    private void DrawSectionBackground(float posX, float posY, out MySprite sectionBackground) {
        Vector2 ib_pos = new Vector2(posX, posY);
        Vector2 ib_size = new Vector2(241, 66);
        sectionBackground = MySprite.CreateSprite("SquareSimple", ib_pos, ib_size);
        sectionBackground.Color = Constants.COLOR_BACKGROUND_MASK;
    }

    private void DrawAirventIcon(float posX, float posY, Color stateColor, bool isWorking, out MySprite iconBorder, out MySprite iconBackground, out MySprite fanH, out MySprite fanV, out MySprite fanCenterBackground, out MySprite fanCenter) {
        Vector2 ib_pos = new Vector2(posX, posY);
        Vector2 ib_size = new Vector2(30, 30);
        iconBorder = MySprite.CreateSprite("SquareSimple", ib_pos, ib_size);
        iconBorder.Color = stateColor;

        Vector2 ibg_pos = new Vector2(posX, posY);
        Vector2 ibg_size = new Vector2(26, 26);
        iconBackground = MySprite.CreateSprite("Circle", ibg_pos, ibg_size);
        iconBackground.Color = Constants.COLOR_BACKGROUND;

        Vector2 fh_pos = new Vector2(posX, posY);
        Vector2 fh_size = new Vector2(20, 4);
        fanH = MySprite.CreateSprite("SquareSimple", fh_pos, fh_size);
        fanH.Color = stateColor;
        if (isWorking)
            fanH.RotationOrScale = rotationRadiants;

        Vector2 fv_pos = new Vector2(posX, posY);
        Vector2 fv_size = new Vector2(4, 20);
        fanV = MySprite.CreateSprite("SquareSimple", fv_pos, fv_size);
        fanV.Color = stateColor;
        if (isWorking)
            fanV.RotationOrScale = rotationRadiants;

        rotationRadiants += 0.05f;

        Vector2 fcb_pos = new Vector2(posX, posY);
        Vector2 fcb_size = new Vector2(10, 10);
        fanCenterBackground = MySprite.CreateSprite("Circle", fcb_pos, fcb_size);
        fanCenterBackground.Color = Constants.COLOR_BACKGROUND;

        Vector2 fc_pos = new Vector2(posX, posY);
        Vector2 fc_size = new Vector2(6, 6);
        fanCenter = MySprite.CreateSprite("Circle", fc_pos, fc_size);
        fanCenter.Color = stateColor;
    }

    private void DrawAirventInfos(float posX, float posY, Color stateColor, bool isWorking, IMyAirVent airvent, out MySprite airventText, out MySprite divider, out MySprite statusTitle, out MySprite statusInfo, out MySprite oxygenTitle, out MySprite oxygenInfo, out MySprite pressurizeTitle, out MySprite pressurizeInfo) {
        string name = airvent.CustomName;
        if (name.Length > 17) {
            name = name.Substring(0, 14);
            name += "...";
        }

        airventText = MySprite.CreateText(name, "Debug", Constants.COLOR_WHITE, 0.8f, TextAlignment.LEFT);
        airventText.Position = new Vector2(posX - 83, posY - 25);

        Vector2 d_pos = new Vector2(posX + 17, posY);
        Vector2 d_size = new Vector2(199, 2);
        divider = MySprite.CreateSprite("SquareSimple", d_pos, d_size);
        divider.Color = Constants.COLOR_GREEN;

        statusTitle = MySprite.CreateText("Status", "Debug", Constants.COLOR_WHITE, 0.5f, TextAlignment.LEFT);
        statusTitle.Position = new Vector2(posX - 116, posY + 2);

        string airventStatus = "";
        Color textColor = Constants.COLOR_WHITE;
        if (!airvent.IsFunctional) {
            airventStatus = "BROKEN";
            textColor = Constants.COLOR_RED;
        } else if (!airvent.IsWorking) {
            airventStatus = "NOT WORKING";
            textColor = Constants.COLOR_RED;
        } else if (!airvent.CanPressurize) {
            airventStatus = "LEAK";
            textColor = Constants.COLOR_RED;
        } else {
            airventStatus = "OPTIMAL";
        }

        statusInfo = MySprite.CreateText(airventStatus, "Debug", textColor, 0.5f, TextAlignment.LEFT);
        statusInfo.Position = new Vector2(posX - 116, posY + 16);

        oxygenTitle = MySprite.CreateText("Oxygen", "Debug", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        oxygenTitle.Position = new Vector2(posX, posY + 2);

        oxygenInfo = MySprite.CreateText((airvent.GetOxygenLevel() * 100).ToString("0.0"), "Debug", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        oxygenInfo.Position = new Vector2(posX, posY + 16);

        pressurizeTitle = MySprite.CreateText("Action", "Debug", Constants.COLOR_WHITE, 0.5f, TextAlignment.RIGHT);
        pressurizeTitle.Position = new Vector2(posX + 116, posY + 2);

        string action = "";
        if (airvent.Depressurize) {
            action = "Depressurizing";
        } else {
            action = "Pressurizing";
        }
        pressurizeInfo = MySprite.CreateText(action, "Debug", Constants.COLOR_WHITE, 0.5f, TextAlignment.RIGHT);
        pressurizeInfo.Position = new Vector2(posX + 116, posY + 16);

    }

    private void DrawFooter(int count, List<MySprite> footerSpriteList, int page) {
        Vector2 pf_pos = new Vector2(256, 405);
        Vector2 pf_size = new Vector2(492, 30);
        MySprite pageFrame = MySprite.CreateSprite("SquareSimple", pf_pos, pf_size);
        pageFrame.Color = Constants.COLOR_BACKGROUND_MASK;

        int numPages = 1;
        if (count % 10 == 0) {
            numPages = count / 10;
        } else {
            numPages = count / 10 + 1;
        }
        MySprite pageNumber = MySprite.CreateText($"Page {page} of {numPages}", "Debug", Constants.COLOR_WHITE, 0.8f, TextAlignment.CENTER);
        pageNumber.Position = new Vector2(256, 392);

        Vector2 ff_pos = new Vector2(256, 466);
        Vector2 ff_size = new Vector2(492, 72);
        MySprite footerFrame = MySprite.CreateSprite("SquareSimple", ff_pos, ff_size);
        string statusText = "";
        if (myProgram.isLeakManagementOn) {
            footerFrame.Color = Constants.COLOR_GREEN;
            statusText = "ON";
        } else {
            footerFrame.Color = Constants.COLOR_RED;
            statusText = "OFF";
        }

        MySprite leakStatus = MySprite.CreateText($"Leak Prevention: {statusText}", "Debug", Constants.COLOR_WHITE, 1.5f, TextAlignment.CENTER);
        leakStatus.Position = new Vector2(256, 442);

        footerSpriteList.Add(pageFrame);
        footerSpriteList.Add(pageNumber);
        footerSpriteList.Add(footerFrame);
        footerSpriteList.Add(leakStatus);
    }



}

public class LifeSupportInfo {

    public bool IsLifeSupportAutomatic { get; set; }

    public bool IsGeneratorsWorking { get; set; }
    public bool IsOxygenFarmWorking { get; set; }

    public double TotalOxygenInTanks { get; set; }
    public double TotalHydrogenInTanks { get; set; }

    public string ReadableOxygenInTanks { get; set; }
    public string ReadableHydrogenInTanks { get; set; }

    public LifeSupportInfo() {

    }

}

public class ProductionController {

    Program myProgram;

    public LifeSupportInfo LifeSupportInfo { get; set; }

    List<IMyOxygenFarm> oxygenFarmList = new List<IMyOxygenFarm>();
    List<IMyGasGenerator> gasGeneratorList = new List<IMyGasGenerator>();

    public string WorkingOxygenFarms { get; set; }
    public string WorkingGenerators { get; set; }

    List<IMyGasTank> gasTankListTemp = new List<IMyGasTank>();
    List<IMyGasTank> gasTankList = new List<IMyGasTank>();

    public ProductionController(Program program) {
        myProgram = program;
        Init();
    }

    private void Init() {
        LifeSupportInfo = new LifeSupportInfo();
        myProgram.GridTerminalSystem.GetBlocksOfType(oxygenFarmList);
        myProgram.GridTerminalSystem.GetBlocksOfType(gasGeneratorList);
        myProgram.GridTerminalSystem.GetBlocksOfType(gasTankListTemp);
        foreach(IMyGasTank gasTank in gasTankListTemp) {
            bool isAirlockTank = false;
            foreach (KeyValuePair<string, Airlock> airlock in myProgram.airlockController.Airlocks) {
                foreach (IMyGasTank airlockGasTank in airlock.Value.Tanks) {
                    if (gasTank.EntityId == airlockGasTank.EntityId) {
                        isAirlockTank = true;
                    }
                }
            }
            if (!isAirlockTank) {
                gasTankList.Add(gasTank);
            }
        }
    }

    public void AddCommandToStack(MyCommandLine _commandLine) {
        if (null != _commandLine.Argument(1)) {
            if (_commandLine.Argument(1).Equals(Constants.P_ON)) {
                myProgram.isProductionOn = true;
            } else if (_commandLine.Argument(1).Equals(Constants.P_OFF)) {
                myProgram.isProductionOn = false;
            } else {
                myProgram.Echo("No valid command\n");
            }
        }
    }

    public void OxygenRuntime() {

        ManageGasProduction();

        LifeSupportInfo.IsGeneratorsWorking = IsGeneratorsWorking();
        LifeSupportInfo.IsOxygenFarmWorking = IsOxygenFarmWorking();

        LifeSupportInfo.TotalOxygenInTanks = GetOxygenInTanks();
        LifeSupportInfo.ReadableOxygenInTanks = (LifeSupportInfo.TotalOxygenInTanks).ToString("0.0") + "%";
        LifeSupportInfo.TotalHydrogenInTanks = GetHydrogenInTanks();
        LifeSupportInfo.ReadableHydrogenInTanks = (LifeSupportInfo.TotalHydrogenInTanks).ToString("0.0") + "%";

    }

    private void ManageGasProduction() {

        if (myProgram.isProductionOn) {
            bool oxygenProduction = true;
            if (LifeSupportInfo.TotalOxygenInTanks > myProgram.maximumOxygenInTanks) {
                oxygenProduction = false;
            }
            if (LifeSupportInfo.TotalOxygenInTanks < myProgram.minimumOxygenInTanks) {
                oxygenProduction = true;
            }

            bool hydrogenProduction = true;
            if (LifeSupportInfo.TotalHydrogenInTanks > myProgram.maximumHydrogenInTanks) {
                hydrogenProduction = false;
            }
            if (LifeSupportInfo.TotalOxygenInTanks < myProgram.minimumHydrogenInTanks) {
                hydrogenProduction = true;
            }

            if (oxygenProduction && hydrogenProduction) {
                ModifyProduction(true, true);
            } else if (!oxygenProduction && !hydrogenProduction) {
                ModifyProduction(false, false);
            } else if (oxygenProduction && !hydrogenProduction) {
                ModifyProduction(true, true);
            } else if (!oxygenProduction && hydrogenProduction) {
                ModifyProduction(false, true);
            }

        }
    }

    private void ModifyProduction(bool farmEnabled, bool generatorEnabled) {
        foreach (IMyOxygenFarm farm in oxygenFarmList) {
            IMyFunctionalBlock farmTest = farm as IMyFunctionalBlock;
            farmTest.Enabled = farmEnabled;
        }
        foreach (IMyGasGenerator generator in gasGeneratorList) {
            generator.Enabled = generatorEnabled;
        }
    }

    private bool IsGeneratorsWorking() {
        bool generatorsWorking = false;
        int workingGenerators = 0;
        foreach (IMyGasGenerator gen in gasGeneratorList) {
            if (gen.IsFunctional && gen.IsWorking) {
                workingGenerators++;
                generatorsWorking = true;
            }
        }
        WorkingGenerators = $"({workingGenerators} working out of {gasGeneratorList.Count} total)";
        return generatorsWorking;
    }

    private bool IsOxygenFarmWorking() {
        bool farmsWorking = false;
        int workingFarms = 0;
        foreach (IMyOxygenFarm farm in oxygenFarmList) {
            if (farm.IsFunctional && farm.IsWorking) {
                workingFarms++;
                farmsWorking = true;
            }
        }
        WorkingOxygenFarms = $"({workingFarms} working out of {oxygenFarmList.Count} total)";
        return farmsWorking;
    }

    private double GetOxygenInTanks() {
        double oxygenFillPercentage = 0;
        int tankCount = 0;
        foreach (IMyGasTank tank in gasTankList) {
            if (tank.DefinitionDisplayNameText.ToUpper().Contains("OXYGEN")) {
                oxygenFillPercentage += tank.FilledRatio;
                tankCount++;
            }
        }
        oxygenFillPercentage = (oxygenFillPercentage / tankCount) * 100;
        myProgram.Echo($"Oxygen: {oxygenFillPercentage.ToString("0.0")}%");
        return oxygenFillPercentage;
    }

    private double GetHydrogenInTanks() {
        double hydrogenFillPercentage = 0;
        int tankCount = 0;
        foreach (IMyGasTank tank in gasTankList) {
            if (tank.DefinitionDisplayNameText.ToUpper().Contains("HYDROGEN")) {
                hydrogenFillPercentage += tank.FilledRatio;
                tankCount++;
            }
        }
        hydrogenFillPercentage = (hydrogenFillPercentage / tankCount) * 100;
        myProgram.Echo($"Hydrogen: {hydrogenFillPercentage.ToString("0.0")}%");
        return hydrogenFillPercentage;
    }

}

public class ProductionScreen {

    Program myProgram;

    List<IMyTextPanel> Panels;
    List<IMyTextPanel> DoublePanels;
    ScreenManager ScreenManager;

    public ProductionScreen(Program program, ScreenManager sManager, List<IMyTextPanel> panels, List<IMyTextPanel> doublePanels) {
        myProgram = program;
        ScreenManager = sManager;
        Panels = panels;
        DoublePanels = doublePanels;
    }

    public void GenerateScreen() {
        foreach (IMyTextPanel panel in Panels) {
            using (var frame = panel.DrawFrame()) {

                AddElementsToPanels(frame, false);

            }
        }
        foreach (IMyTextPanel panel in DoublePanels) {
            using (var frame = panel.DrawFrame()) {
                myProgram.screenManager.leakScreen.AddElementsToPanel(1, frame);
                AddElementsToPanels(frame, true);

            }
        }
    }

    private void AddElementsToPanels(MySpriteDrawFrame frame, bool isDoublePanel) {
        List<MySprite> backgroundSpriteList = new List<MySprite>();
        List<MySprite> headerSpriteList = new List<MySprite>();
        List<MySprite> productionSpriteList = new List<MySprite>();
        List<MySprite> gasTanksSpriteList = new List<MySprite>();
        List<MySprite> airlocksSpriteList = new List<MySprite>();

        float screenOffset = 0;
        if (isDoublePanel) {
            screenOffset = 512;
        }

        DrawBackground(backgroundSpriteList, screenOffset);
        DrawHeader(headerSpriteList, screenOffset);
        DrawProduction(productionSpriteList, screenOffset);
        DrawGasTanks(gasTanksSpriteList, screenOffset);
        DrawAirlocks(airlocksSpriteList, screenOffset);

        frame.AddRange(backgroundSpriteList);
        frame.AddRange(headerSpriteList);
        frame.AddRange(productionSpriteList);
        frame.AddRange(gasTanksSpriteList);
        frame.AddRange(airlocksSpriteList);
    }

    private void DrawBackground(List<MySprite> backgroundSpriteList, float screenOffset) {
        Vector2 b_pos = new Vector2(256 + screenOffset, 256);
        Vector2 b_size = new Vector2(512, 512);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = Constants.COLOR_BACKGROUND;

        backgroundSpriteList.Add(background);
    }

    private void DrawHeader(List<MySprite> headerSpriteList, float screenOffset) {
        Color color = (myProgram.isProductionOn) ? Constants.COLOR_GREEN : Constants.COLOR_RED;

        Vector2 b_pos = new Vector2(256 + screenOffset, 34);
        Vector2 b_size = new Vector2(492, 48);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = color;

        string text = (myProgram.isProductionOn) ? "ON" : "OFF";
        var headerText = MySprite.CreateText($"AUTO O2 PRODUCTION: {text}", "White", Constants.COLOR_WHITE, 1.5f, TextAlignment.CENTER);
        headerText.Position = new Vector2(256 + screenOffset, 10);


        headerSpriteList.Add(background);
        headerSpriteList.Add(headerText);
    }

    private void DrawProduction(List<MySprite> productionSpriteList, float screenOffset) {
        Vector2 b_pos = new Vector2(130.5f + screenOffset, 130);
        Vector2 b_size = new Vector2(241, 124);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = Constants.COLOR_BACKGROUND_MASK;

        Vector2 h_pos = new Vector2(130.5f + screenOffset, 78);
        Vector2 h_size = new Vector2(241, 20);
        var header = MySprite.CreateSprite("SquareSimple", h_pos, h_size);
        header.Color = Constants.COLOR_GREEN;

        var headerText = MySprite.CreateText("PRODUCTION INFO", "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.LEFT);
        headerText.Position = new Vector2(20 + screenOffset, 69);

        Vector2 gb_pos = new Vector2(130.5f + screenOffset, 116.5f);
        Vector2 gb_size = new Vector2(221, 37);
        var generatorsBackground = MySprite.CreateSprite("SquareSimple", gb_pos, gb_size);
        generatorsBackground.Color = Constants.COLOR_BACKGROUND_LIGHT;

        Vector2 ofb_pos = new Vector2(130.5f + screenOffset, 163.5f);
        Vector2 ofb_size = new Vector2(221, 37);
        var oxygenFarmBackground = MySprite.CreateSprite("SquareSimple", ofb_pos, ofb_size);
        oxygenFarmBackground.Color = Constants.COLOR_BACKGROUND_LIGHT;

        Vector2 gs_pos = new Vector2(30 + screenOffset, 116.5f);
        Vector2 gs_size = new Vector2(20, 37);
        var generatorsSwitch = MySprite.CreateSprite("SquareSimple", gs_pos, gs_size);
        generatorsSwitch.Color = myProgram.productionController.LifeSupportInfo.IsGeneratorsWorking ? Constants.COLOR_GREEN : Constants.COLOR_RED;

        Vector2 ofs_pos = new Vector2(30 + screenOffset, 163.5f);
        Vector2 ofs_size = new Vector2(20, 37);
        var oxygenFarmSwitch = MySprite.CreateSprite("SquareSimple", ofs_pos, ofs_size);
        oxygenFarmSwitch.Color = myProgram.productionController.LifeSupportInfo.IsOxygenFarmWorking ? Constants.COLOR_GREEN : Constants.COLOR_RED;

        var generatorsSwitchText = MySprite.CreateText($"H2/O2 GENERATORS\n{myProgram.productionController.WorkingGenerators}", "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.LEFT);
        generatorsSwitchText.Position = new Vector2(47 + screenOffset, 98.5f);

        var oxygenFarmSwitchText = MySprite.CreateText($"OXYGEN FARMS\n{myProgram.productionController.WorkingOxygenFarms}", "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.LEFT);
        oxygenFarmSwitchText.Position = new Vector2(47 + screenOffset, 145.5f);

        productionSpriteList.Add(background);
        productionSpriteList.Add(header);
        productionSpriteList.Add(headerText);
        productionSpriteList.Add(generatorsBackground);
        productionSpriteList.Add(oxygenFarmBackground);
        productionSpriteList.Add(generatorsSwitch);
        productionSpriteList.Add(oxygenFarmSwitch);
        productionSpriteList.Add(generatorsSwitchText);
        productionSpriteList.Add(oxygenFarmSwitchText);
    }

    private void DrawGasTanks(List<MySprite> gasTanksSpriteList, float screenOffset) {
        Vector2 b_pos = new Vector2(130.5f + screenOffset, 352);
        Vector2 b_size = new Vector2(241, 300);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = Constants.COLOR_BACKGROUND_MASK;

        Vector2 h_pos = new Vector2(130.5f + screenOffset, 212);
        Vector2 h_size = new Vector2(241, 20);
        var header = MySprite.CreateSprite("SquareSimple", h_pos, h_size);
        header.Color = Constants.COLOR_GREEN;

        var headerText = MySprite.CreateText("GAS TANKS", "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.LEFT);
        headerText.Position = new Vector2(20 + screenOffset, 203);

        // OXYGEN TANK
        Color oxygenColor = Constants.COLOR_GREEN;
        if (myProgram.productionController.LifeSupportInfo.TotalOxygenInTanks <= 70) {
            oxygenColor = Constants.COLOR_YELLOW;
        } else if (myProgram.productionController.LifeSupportInfo.TotalOxygenInTanks <= 30) {
            oxygenColor = Constants.COLOR_RED;
        }

        Vector2 of_pos = new Vector2(47 + screenOffset, 362);
        Vector2 of_size = new Vector2(54, 260);
        var oxygenFrame = MySprite.CreateSprite("SquareSimple", of_pos, of_size);
        oxygenFrame.Color = Constants.COLOR_WHITE;

        Vector2 ofm_pos = new Vector2(47 + screenOffset, 362);
        Vector2 ofm_size = new Vector2(46, 252);
        var oxygenFrameMask = MySprite.CreateSprite("SquareSimple", ofm_pos, ofm_size);
        oxygenFrameMask.Color = Constants.COLOR_BACKGROUND_MASK;

        float oxygenBarPosY = (float)(374 + 110-((110 * myProgram.productionController.LifeSupportInfo.TotalOxygenInTanks) / 100));
        float oxygenBarHeight = (float)((220 * myProgram.productionController.LifeSupportInfo.TotalOxygenInTanks) / 100);

        Vector2 ofb_pos = new Vector2(47 + screenOffset, oxygenBarPosY);
        Vector2 ofb_size = new Vector2(38, oxygenBarHeight);
        var oxygenFrameBar = MySprite.CreateSprite("SquareSimple", ofb_pos, ofb_size);
        oxygenFrameBar.Color = oxygenColor;

        var oxygenBarText = MySprite.CreateText($"{myProgram.productionController.LifeSupportInfo.ReadableOxygenInTanks}", "White", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        oxygenBarText.Position = new Vector2(47 + screenOffset, oxygenBarPosY - (oxygenBarHeight / 2) - 15);

        var oxygenBarTitle = MySprite.CreateText("O\nX\nY\nG\nE\nN\n\nT\nA\nN\nK\nS", "White", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        oxygenBarTitle.Position = new Vector2(82 + screenOffset, 230);

        // HYDROGEN TANK
        Color hydrogenColor = Constants.COLOR_GREEN;
        if (myProgram.productionController.LifeSupportInfo.TotalHydrogenInTanks <= 70 && myProgram.productionController.LifeSupportInfo.TotalHydrogenInTanks > 30) {
            hydrogenColor = Constants.COLOR_YELLOW;
        } else if (myProgram.productionController.LifeSupportInfo.TotalHydrogenInTanks <= 30) {
            hydrogenColor = Constants.COLOR_RED;
        }

        Vector2 hf_pos = new Vector2(157 + screenOffset, 362);
        Vector2 hf_size = new Vector2(54, 260);
        var hydrogenFrame = MySprite.CreateSprite("SquareSimple", hf_pos, hf_size);
        hydrogenFrame.Color = Constants.COLOR_WHITE;

        Vector2 hfm_pos = new Vector2(157 + screenOffset, 362);
        Vector2 hfm_size = new Vector2(46, 252);
        var hydrogenFrameMask = MySprite.CreateSprite("SquareSimple", hfm_pos, hfm_size);
        hydrogenFrameMask.Color = Constants.COLOR_BACKGROUND_MASK;

        float hydrogenBarPosY = (float)(374 + 110 - ((110 * myProgram.productionController.LifeSupportInfo.TotalHydrogenInTanks) / 100));
        float hydrogenBarHeight = (float)((220 * myProgram.productionController.LifeSupportInfo.TotalHydrogenInTanks) / 100);

        Vector2 hfb_pos = new Vector2(157 + screenOffset, hydrogenBarPosY);
        Vector2 hfb_size = new Vector2(38, hydrogenBarHeight);
        var hydrogenFrameBar = MySprite.CreateSprite("SquareSimple", hfb_pos, hfb_size);
        hydrogenFrameBar.Color = hydrogenColor;

        var hydrogenBarText = MySprite.CreateText($"{myProgram.productionController.LifeSupportInfo.ReadableHydrogenInTanks}", "White", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        hydrogenBarText.Position = new Vector2(157 + screenOffset, hydrogenBarPosY - (hydrogenBarHeight / 2) - 15);

        var hydrogenBarTitle = MySprite.CreateText("H\nY\nD\nR\nO\nG\nE\nN\n\nT\nA\nN\nK\nS", "White", Constants.COLOR_WHITE, 0.5f, TextAlignment.CENTER);
        hydrogenBarTitle.Position = new Vector2(192 + screenOffset, 230);

        gasTanksSpriteList.Add(background);
        gasTanksSpriteList.Add(header);
        gasTanksSpriteList.Add(headerText);
        gasTanksSpriteList.Add(oxygenFrame);
        gasTanksSpriteList.Add(oxygenFrameMask);
        gasTanksSpriteList.Add(oxygenFrameBar);
        gasTanksSpriteList.Add(oxygenBarText);
        gasTanksSpriteList.Add(oxygenBarTitle);
        gasTanksSpriteList.Add(hydrogenFrame);
        gasTanksSpriteList.Add(hydrogenFrameMask);
        gasTanksSpriteList.Add(hydrogenFrameBar);
        gasTanksSpriteList.Add(hydrogenBarText);
        gasTanksSpriteList.Add(hydrogenBarTitle);
    }

    private void DrawAirlocks(List<MySprite> airlocksSpriteList, float screenOffset) {
        Vector2 b_pos = new Vector2(381.5f + screenOffset, 285);
        Vector2 b_size = new Vector2(241, 434);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = Constants.COLOR_BACKGROUND_MASK;

        Vector2 h_pos = new Vector2(381.5f + screenOffset, 78);
        Vector2 h_size = new Vector2(241, 20);
        var header = MySprite.CreateSprite("SquareSimple", h_pos, h_size);
        header.Color = Constants.COLOR_GREEN;

        var headerText = MySprite.CreateText("AIRLOCKS STATUS", "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.LEFT);
        headerText.Position = new Vector2(271 + screenOffset, 69);

        airlocksSpriteList.Add(background);
        airlocksSpriteList.Add(header);
        airlocksSpriteList.Add(headerText);

        float startY = 133; // 213
        float offset = 80;
        int counter = 0;
        foreach (KeyValuePair<string, Airlock> _al in myProgram.airlockController.Airlocks) {

            Airlock airlock = _al.Value;
            myProgram.Echo($"Airlock: {airlock.Name}");

            Color airlockStatusColor = new Color(256, 256, 256);
            if (airlock.PublicStatus.Equals(Constants.AP_IDLE) || airlock.PublicStatus.Equals(Constants.AP_CYCLE)) {
                airlockStatusColor = Constants.COLOR_YELLOW;
            } else if (airlock.PublicStatus.Equals(Constants.AP_PRESSURIZED)) {
                airlockStatusColor = Constants.COLOR_GREEN;
            } else if (airlock.PublicStatus.Equals(Constants.AP_DEPRESSURIZED)) {
                airlockStatusColor = Constants.COLOR_RED;
            }

            Vector2 ab_pos = new Vector2(381.5f + screenOffset, startY + (offset * counter));
            Vector2 ab_size = new Vector2(221, 70);
            var airlockBackground = MySprite.CreateSprite("SquareSimple", ab_pos, ab_size);
            airlockBackground.Color = Constants.COLOR_BACKGROUND_LIGHT;

            Vector2 asi_pos = new Vector2(276f + screenOffset, startY + (offset * counter));
            Vector2 asi_size = new Vector2(10, 70);
            var airlockStatusIndicator = MySprite.CreateSprite("SquareSimple", asi_pos, asi_size);
            airlockStatusIndicator.Color = airlockStatusColor;

            var airlockName = MySprite.CreateText(airlock.Name, "White", Constants.COLOR_WHITE, 1f, TextAlignment.LEFT);
            airlockName.Position = new Vector2(288f + screenOffset, 100 + (offset * counter));

            var airlockStatusTitle = MySprite.CreateText("STATUS", "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.LEFT);
            airlockStatusTitle.Position = new Vector2(288f + screenOffset, 128 + (offset * counter));

            var airlockStatus = MySprite.CreateText(airlock.PublicStatus, "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.RIGHT);
            airlockStatus.Position = new Vector2(480f + screenOffset, 128 + (offset * counter));

            var airlockDoorStatusTitle = MySprite.CreateText("DOORS", "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.LEFT);
            airlockDoorStatusTitle.Position = new Vector2(288.5f + screenOffset, 145 + (offset * counter));

            var airlockDoorStatus = MySprite.CreateText(airlock.OpenDoors, "White", Constants.COLOR_WHITE, 0.6f, TextAlignment.RIGHT);
            airlockDoorStatus.Position = new Vector2(480f + screenOffset, 145 + (offset * counter));

            airlocksSpriteList.Add(airlockBackground);
            airlocksSpriteList.Add(airlockStatusIndicator);
            airlocksSpriteList.Add(airlockName);
            airlocksSpriteList.Add(airlockStatusTitle);
            airlocksSpriteList.Add(airlockStatus);
            airlocksSpriteList.Add(airlockDoorStatusTitle);
            airlocksSpriteList.Add(airlockDoorStatus);

            counter++;

        }



    }
}

public class ScreenManager {

    Program myProgram;

    public string Status { get; set; }

    private List<IMyTextPanel> GlobalPanels { get; set; }

    private List<IMyTextPanel> ScriptPanels { get; set; }
    private List<IMyTextPanel> AirlockPanels { get; set; }
    private List<IMyTextPanel> LeakPanels { get; set; }
    private List<IMyTextPanel> ProductionPanels { get; set; }
    private List<IMyTextPanel> DoublePanels { get; set; }

    private int tick = 0;
    private int SplashLength { get; set; }



    private SplashScreen splashScreen;
    public AirlockScreen airlockScreen;
    public LeakScreen leakScreen;
    public ProductionScreen productionScreen;

    public ScreenManager(Program program) {
        myProgram = program;
        Init();
    }

    public void Init() {
        Status = Constants.S_STATUS_INIT;

        GlobalPanels = new List<IMyTextPanel>();
        myProgram.GridTerminalSystem.GetBlocksOfType(GlobalPanels);

        ScriptPanels = new List<IMyTextPanel>();
        AirlockPanels = new List<IMyTextPanel>();
        LeakPanels = new List<IMyTextPanel>();
        ProductionPanels = new List<IMyTextPanel>();
        DoublePanels = new List<IMyTextPanel>();

        myProgram.Echo("Checking leak and production screens");
        foreach (IMyTextPanel panel in GlobalPanels) {
            if (panel.CustomData.Contains(Constants.T_LSM_AIRVENT_SCREEN) && panel.CustomData.Contains(Constants.T_LSM_PROD_SCREEN)) {
                myProgram.Echo("Production Panel found");
                panel.ContentType = ContentType.SCRIPT;
                DoublePanels.Add(panel);
                ScriptPanels.Add(panel);
            } else if (panel.CustomData.Contains(Constants.T_LSM_AIRVENT_SCREEN) && !panel.CustomData.Contains(Constants.T_LSM_PROD_SCREEN)) {
                myProgram.Echo("Airvent Panel found");
                panel.ContentType = ContentType.SCRIPT;
                LeakPanels.Add(panel);
                ScriptPanels.Add(panel);
            } else if (!panel.CustomData.Contains(Constants.T_LSM_AIRVENT_SCREEN) && panel.CustomData.Contains(Constants.T_LSM_PROD_SCREEN)) {
                myProgram.Echo("Production Panel found");
                panel.ContentType = ContentType.SCRIPT;
                ProductionPanels.Add(panel);
                ScriptPanels.Add(panel);
            }
        }

        myProgram.Echo("Checking airlocks");
        foreach (KeyValuePair<string, Airlock> _al in myProgram.airlockController.Airlocks) {
            myProgram.Echo("Airlocks found");
            Airlock airlock = _al.Value;
            foreach (IMyTextPanel panel in airlock.Panels) {
                panel.ContentType = ContentType.SCRIPT;
                panel.CustomData = Constants.T_LSM_AIRLOCK_SCREEN;
                AirlockPanels.Add(panel);
                ScriptPanels.Add(panel);
            }
        }

        splashScreen = new SplashScreen(this, ScriptPanels);
        airlockScreen = new AirlockScreen(this, myProgram);
        leakScreen = new LeakScreen(myProgram, this, LeakPanels, DoublePanels, myProgram.leakController.Airvents);
        productionScreen = new ProductionScreen(myProgram, this, ProductionPanels, DoublePanels);
    }

    public void ScreenRuntime() {
        tick++;
        if (tick < 200) {
            splashScreen.GenerateScreen();
        } else {
            airlockScreen.GenerateScreen(myProgram.airlockController.Airlocks);
            leakScreen.GenerateScreen();
            productionScreen.GenerateScreen();
        }
    }


}

public class SplashScreen {

    List<IMyTextPanel> Panels;
    ScreenManager ScreenManager;

    public SplashScreen(ScreenManager sManager, List<IMyTextPanel> panels) {
        ScreenManager = sManager;
        Panels = panels;
    }

    public void GenerateScreen() {

        foreach (IMyTextPanel panel in Panels) {
            using (var frame = panel.DrawFrame()) {
                List<MySprite> spriteList = new List<MySprite>();
                DrawSprite(spriteList);
                frame.AddRange(spriteList);

            }
        }
    }

    private void DrawSprite(List<MySprite> spriteList) {
        Vector2 b_pos = new Vector2(256, 256);
        Vector2 b_size = new Vector2(512, 512);
        var background = MySprite.CreateSprite("SquareSimple", b_pos, b_size);
        background.Color = Constants.COLOR_BACKGROUND;

        Vector2 e1_pos = new Vector2(256, 256);
        Vector2 e1_size = new Vector2(26, 128);
        var elem1 = MySprite.CreateSprite("SquareSimple", e1_pos, e1_size);
        elem1.RotationOrScale = 0;
        elem1.Color = Constants.COLOR_WHITE;

        Vector2 e2_pos = new Vector2(207.3f, 271);
        Vector2 e2_size = new Vector2(119, 26);
        var elem2 = MySprite.CreateSprite("SquareSimple", e2_pos, e2_size);
        elem2.RotationOrScale = Utils.DegreeToRadian(30);
        elem2.Color = Constants.COLOR_WHITE;

        Vector2 e3_pos = new Vector2(217.3f, 179);
        Vector2 e3_size = new Vector2(119, 26);
        var elem3 = MySprite.CreateSprite("SquareSimple", e3_pos, e3_size);
        elem3.RotationOrScale = Utils.DegreeToRadian(30);
        elem3.Color = Constants.COLOR_WHITE;

        Vector2 e4_pos = new Vector2(340.7f, 196);
        Vector2 e4_size = new Vector2(25, 128);
        var elem4 = MySprite.CreateSprite("SquareSimple", e4_pos, e4_size);
        elem4.Color = Constants.COLOR_WHITE;

        Vector2 e5_pos = new Vector2(170f, 220);
        Vector2 e5_size = new Vector2(25, 60);
        var elem5 = MySprite.CreateSprite("SquareSimple", e5_pos, e5_size);
        elem5.Color = Constants.COLOR_WHITE;

        Vector2 e6_pos = new Vector2(284.9f, 118.3f);
        Vector2 e6_size = new Vector2(79, 26);
        var elem6 = MySprite.CreateSprite("SquareSimple", e6_pos, e6_size);
        elem6.RotationOrScale = Utils.DegreeToRadian(30);
        elem6.Color = Constants.COLOR_WHITE;

        Vector2 e7_pos = new Vector2(316.9f, 266.3f);
        Vector2 e7_size = new Vector2(69, 26);
        var elem7 = MySprite.CreateSprite("SquareSimple", e7_pos, e7_size);
        elem7.RotationOrScale = Utils.DegreeToRadian(-30);
        elem7.Color = Constants.COLOR_WHITE;

        Vector2 e8_pos = new Vector2(302.5f, 174.5f);
        Vector2 e8_size = new Vector2(99, 26);
        var elem8 = MySprite.CreateSprite("SquareSimple", e8_pos, e8_size);
        elem8.RotationOrScale = Utils.DegreeToRadian(-30);
        elem8.Color = Constants.COLOR_WHITE;

        Vector2 e9_pos = new Vector2(207.8f, 130.3f);
        Vector2 e9_size = new Vector2(127, 26);
        var elem9 = MySprite.CreateSprite("SquareSimple", e9_pos, e9_size);
        elem9.RotationOrScale = Utils.DegreeToRadian(-30);
        elem9.Color = Constants.COLOR_WHITE;

        Vector2 m1_pos = new Vector2(151f, 153f);
        Vector2 m1_size = new Vector2(15, 46);
        var mask1 = MySprite.CreateSprite("SquareSimple", m1_pos, m1_size);
        mask1.RotationOrScale = 0;
        mask1.Color = Constants.COLOR_BACKGROUND;

        Vector2 m2_pos = new Vector2(166.9f, 174f);
        Vector2 m2_size = new Vector2(45, 16);
        var mask2 = MySprite.CreateSprite("SquareSimple", m2_pos, m2_size);
        mask2.RotationOrScale = Utils.DegreeToRadian(30);
        mask2.Color = Constants.COLOR_BACKGROUND;

        Vector2 m3_pos = new Vector2(176.9f, 193f);
        Vector2 m3_size = new Vector2(45, 16);
        var mask3 = MySprite.CreateSprite("SquareSimple", m3_pos, m3_size);
        mask3.RotationOrScale = Utils.DegreeToRadian(30);
        mask3.Color = Constants.COLOR_BACKGROUND;

        Vector2 m4_pos = new Vector2(149f, 250.2f);
        Vector2 m4_size = new Vector2(15, 46);
        var mask4 = MySprite.CreateSprite("SquareSimple", m4_pos, m4_size);
        mask4.RotationOrScale = 0;
        mask4.Color = Constants.COLOR_BACKGROUND;

        Vector2 m5_pos = new Vector2(305.7f, 136.6f);
        Vector2 m5_size = new Vector2(45, 16);
        var mask5 = MySprite.CreateSprite("SquareSimple", m5_pos, m5_size);
        mask5.RotationOrScale = Utils.DegreeToRadian(-30);
        mask5.Color = Constants.COLOR_BACKGROUND;

        Vector2 m6_pos = new Vector2(318.7f, 140.6f);
        Vector2 m6_size = new Vector2(45, 16);
        var mask6 = MySprite.CreateSprite("SquareSimple", m6_pos, m6_size);
        mask6.RotationOrScale = Utils.DegreeToRadian(-30);
        mask6.Color = Constants.COLOR_BACKGROUND;

        Vector2 m7_pos = new Vector2(348.1f, 132f);
        Vector2 m7_size = new Vector2(45, 16);
        var mask7 = MySprite.CreateSprite("SquareSimple", m7_pos, m7_size);
        mask7.RotationOrScale = Utils.DegreeToRadian(30);
        mask7.Color = Constants.COLOR_BACKGROUND;

        Vector2 m8_pos = new Vector2(244.9f, 316.9f);
        Vector2 m8_size = new Vector2(45, 16);
        var mask8 = MySprite.CreateSprite("SquareSimple", m8_pos, m8_size);
        mask8.RotationOrScale = Utils.DegreeToRadian(30);
        mask8.Color = Constants.COLOR_BACKGROUND;

        Vector2 m9_pos = new Vector2(262.9f, 320.6f);
        Vector2 m9_size = new Vector2(45, 16);
        var mask9 = MySprite.CreateSprite("SquareSimple", m9_pos, m9_size);
        mask9.RotationOrScale = Utils.DegreeToRadian(-30);
        mask9.Color = Constants.COLOR_BACKGROUND;

        Vector2 m10_pos = new Vector2(286.7f, 279.2f);
        Vector2 m10_size = new Vector2(15, 46);
        var mask10 = MySprite.CreateSprite("SquareSimple", m10_pos, m10_size);
        mask10.RotationOrScale = 0;
        mask10.Color = Constants.COLOR_BACKGROUND;

        var logoUpperText = MySprite.CreateText("CUB3", "White", Constants.COLOR_GREEN, 3.5f, TextAlignment.CENTER);
        logoUpperText.Position = new Vector2(256, 300);

        var logoBottomText = MySprite.CreateText("SOFTWARE", "White", Constants.COLOR_WHITE, 1.6f, TextAlignment.CENTER);
        logoBottomText.Position = new Vector2(256, 380);





        spriteList.Add(background);

        spriteList.Add(elem1);
        spriteList.Add(elem2);
        spriteList.Add(elem3);
        spriteList.Add(elem4);
        spriteList.Add(elem5);
        spriteList.Add(elem6);
        spriteList.Add(elem7);
        spriteList.Add(elem8);
        spriteList.Add(elem9);

        spriteList.Add(mask1);
        spriteList.Add(mask2);
        spriteList.Add(mask3);
        spriteList.Add(mask4);
        spriteList.Add(mask5);
        spriteList.Add(mask6);
        spriteList.Add(mask7);
        spriteList.Add(mask8);
        spriteList.Add(mask9);
        spriteList.Add(mask10);

        spriteList.Add(logoUpperText);
        spriteList.Add(logoBottomText);

    }

}

public class Utils {

    public static bool IsListEmpty<T>(List<T> list) {
        if (null == list || list.Count == 0) {
            return true;
        }
        return false;
    }

    public static float DegreeToRadian(float angle) {
        return (float)(Math.PI * angle / 180.0);
    }

}