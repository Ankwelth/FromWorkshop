MyIni _ini = new MyIni();

List<IMyAirVent> airVents = new List<IMyAirVent>();
List<IMyDoor> airlockExteriorDoors = new List<IMyDoor>();
List<IMyDoor> airlockInteriorDoors = new List<IMyDoor>();
List<IMyGasTank> airlockOxygenTanks = new List<IMyGasTank>();
List<IMyTextSurfaceProvider> airlockInfoPanels = new List<IMyTextSurfaceProvider>();
List<IMyReflectorLight> airlockWarningLights = new List<IMyReflectorLight>();
List<IMySoundBlock> airlockWarningSirens = new List<IMySoundBlock>();
IMyAirVent exteriorVent;

const string STATUS_UNKNOWN = "Unknown";
const string STATUS_PRESSURIZED = "Pressurized";
const string STATUS_PRESSURIZING = "Pressurizing";
const string STATUS_DEPRESSURIZED = "Depressurized";
const string STATUS_DEPRESSURIZING = "Depressurizing";

const int DELAY_VAL = 2;

string _airlockVentsName;
string _airlockExteriorDoorsName;
string _airlockInteriorDoorsName;
int _minimumPressureThreshold = 100;

string _airlockOxygenTanksName;
string _airlockWarningSirensName;
string _airlockWarningLightsName;
string _airlockInfoPanelsName;
string _exteriorVentName;
bool _ventExcessOxygen = true;
int _oxygenExcessThreshold = 100;
int _exteriorOxygenSafetyThreshold = 80;

string _airlockStatus;

int _openDelay = 0;
int _lockDelay = 0;
int _pressureDelay = 0;

public Program()
{
    Print("Initializing script...\n", false);

    if (String.IsNullOrEmpty(Me.CustomData)) {
        Print("No configuration in Custom Data found, writing default configuration.");
        Print("Please check the Custom Data option for this block and confirm the configuration and recompile.");
        _ini.Clear();
        _ini.Set("required", "airlockVentsName", "Airlock Vents");
        _ini.Set("required", "airlockExteriorDoorsName", "Airlock Exterior Doors");
        _ini.Set("required", "airlockInteriorDoorsName", "Airlock Interior Doors");
        _ini.Set("required", "minimumPressureThreshold", 100);
        _ini.Set("optional", "airlockOxygenTanksName", "Oxygen Tanks");
        _ini.Set("optional", "airlockWarningSirensName", "Airlock Warning Sirens");
        _ini.Set("optional", "airlockWarningLightsName", "Airlock Warning Lights");
        _ini.Set("optional", "airlockInfoPanelsName", "Airlock Info Panels");
        _ini.Set("optional", "exteriorVentName", "Exterior Air Vent");
        _ini.Set("optional", "ventExcessOxygen", true);
        _ini.Set("optional", "oxygenExcessThreshold", 100);
        _ini.Set("optional", "exteriorOxygenSafetyThreshold", 80);
        string comments = "";
        comments += ";To use this config, please make sure you have *groups* for each of the block names.\n";
        comments += ";The exception to this is the exterior vent, which is a single block.\n";
        comments += ";\n;\n";
        comments += "; REQUIRED CONFIG VALUES\n";
        comments += "; airlockVentsName - Group name of vents inside airlock.\n";
        comments += "; airlockExteriorDoorsName - Group name of exterior airlock doors.\n";
        comments += "; airlockInteriorDoorsName - Group name of interior airlock doors.\n";
        comments += "; minimumPressureThreshold - Percentage to consider airlock pressurized.\n";
        comments += ";      Greater than 50 is required to breathe. Default is 100%.\n";
        comments += "; OPTIONAL CONFIG VALUES\n";
        comments += "; If you delete any of these from your custom data, they will be defaulted off/disabled.\n";
        comments += "; You can add a single ; to the front of the line to disable the line and configure it later as well.\n";
        comments += "; airlockOxygenTanksName - Group name of tanks to calculate the current tank levels.\n";
        comments += "; airlockWarningSirensName - Group name of sound blocks to play warning during operation.\n";
        comments += "; airlockWarningLightsName - Group name of lights to enable during operation.\n";
        comments += "; airlockInfoPanelsName - Group name of LCD panels to display terminal data.\n";
        comments += "; exteriorVentName - Block name of air vent to the exterior to detect outside air.\n";
        comments += ";      Remove to turn off outside safety threshold checks.\n";
        comments += "; ventExcessOxygen - true/false to turn on venting when tanks are too full to depressurize fully.\n";
        comments += "; oxygenExcessThreshold - Percentage of tank fill to vent excess oxygen when enabled. Default 100%.\n";
        comments += "; exteriorOxygenSafetyThreshold - Percentage to consider safe oxygen levels outside. Default to 80%.\n";
        comments += ";      Greater than 50% is required to breathe. Earthlike oxygen is roughly 83%.\n";
        Me.CustomData = comments + _ini.ToString();
        return;
    }
    
    MyIniParseResult result;
    if (!_ini.TryParse(Me.CustomData, out result)) {
        Print("Failed to parse the configuration in your CustomData. You can always delete it and recompile to generate a new one.");
        return;
    }
    
    _airlockVentsName = _ini.Get("required", "airlockVentsName").ToString("Airlock Vents");
    _airlockExteriorDoorsName = _ini.Get("required", "airlockExteriorDoorsName").ToString("Airlock Exterior Doors");
    _airlockInteriorDoorsName = _ini.Get("required", "airlockInteriorDoorsName").ToString("Airlock Interior Doors");
    _minimumPressureThreshold = _ini.Get("required", "minimumPressureThreshold").ToInt32(100);
    _airlockOxygenTanksName = _ini.Get("optional", "airlockOxygenTanksName").ToString("");
    _airlockWarningSirensName = _ini.Get("optional", "airlockWarningSirensName").ToString("");
    _airlockWarningLightsName = _ini.Get("optional", "airlockWarningLightsName").ToString("");
    _airlockInfoPanelsName = _ini.Get("optional", "airlockInfoPanelsName").ToString("");
    _exteriorVentName = _ini.Get("optional", "exteriorVentName").ToString("");
    _ventExcessOxygen = _ini.Get("optional", "ventExcessOxygen").ToBoolean(false);
    _oxygenExcessThreshold = _ini.Get("optional", "oxygenExcessThreshold").ToInt32(100);
    _exteriorOxygenSafetyThreshold = _ini.Get("optional", "exteriorOxygenSafetyThreshold").ToInt32(80);
        
    if (!IsValidConfiguration())
    {
        Print("Cannot setup script properly. Fix errors and recompile.");
        return;
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    if (_airlockStatus == null || _airlockStatus == "") {
        _airlockStatus = Storage;
        if (_airlockStatus == null || _airlockStatus == "") {
            _airlockStatus = STATUS_UNKNOWN;
        }
    }
}

public void Save()
{
    Storage = _airlockStatus;
}

public void Main(string argument, UpdateType updateSource)
{
    Print("Primary Airlock Controller Terminal\n", false);
    if (argument == "force depressurized") {
        _airlockStatus = STATUS_DEPRESSURIZED;
        return;
    }

    // Loop code
    if (updateSource == UpdateType.Update100) {
        UpdateInfoPanels();
        Print("Current airlock status: " + _airlockStatus);
        Print("Vent reporting status: " + GetVentStatus());
        Print("Room Oxygen level: " + GetOxygen() + "%");
        if (airlockOxygenTanks.Count > 0) {
            double oxygenTankLevel = airlockOxygenTanks[0].FilledRatio * 100;
            Print("Oxygen Tank Level: " + oxygenTankLevel + "%");
        }
        if (exteriorVent != null) {
            Print("Exterior Oxygen level: " + GetExteriorOxygenLevel() + "%");
        }

        switch (_airlockStatus)
        {
            case STATUS_PRESSURIZING:
                EnableLightsAndSirens();
                Print("Closing exterior doors...");
                CloseExteriorDoors();

                Print("Waiting for exterior doors to close...");
                if (IsExteriorDoorsOpen()) {
                    return;
                }
                if(_openDelay < DELAY_VAL) {
                    _openDelay++;
                    break;
                }
                Print("Locking exterior doors");
                LockExteriorDoors();

                if(_lockDelay < DELAY_VAL) {
                    _lockDelay++;
                    break;
                }
                Print("All doors closed, pressurizing...");
                Pressurize();

                if (IsPressurized()) {
                    Print("Pressurization complete.");
                    DisableLightsAndSirens();
                    UnlockInteriorDoors();
                    
                    if(_pressureDelay < DELAY_VAL) {
                        _pressureDelay++;
                        break;
                    }
                    OpenInteriorDoors();
                    _airlockStatus = STATUS_PRESSURIZED;
                }
                break;

            case STATUS_DEPRESSURIZING:
                if (IsSafeOutside()) {
                    Print("Outside atmosphere is safe. Unlocking and opening doors.");
                    UnlockInteriorDoors();
                    UnlockExteriorDoors();
                    OpenExteriorDoors();
                    _airlockStatus = STATUS_DEPRESSURIZED;
                    break;
                }  
                EnableLightsAndSirens();
                Print("Closing interior doors");
                CloseInteriorDoors();

                Print("Waiting for interior doors to close...");
                if (IsInteriorDoorsOpen()) {
                    return;
                }
                
                if(_openDelay < DELAY_VAL) {
                    _openDelay++;
                    break;
                }

                Print("Locking interior doors");
                LockInteriorDoors();

                Print("Depressurizing...");
                Depressurize();

                if (!IsDepressurized() && !ShouldVentOxygen()) {
                    break;
                }
                if(_pressureDelay < DELAY_VAL) {
                    _pressureDelay++;
                    break;
                }
                DisableLightsAndSirens();
                Print("Depressurization complete. Unlocking and opening exterior doors.");
                UnlockExteriorDoors();
                OpenExteriorDoors();
                _airlockStatus = STATUS_DEPRESSURIZED;
                
                break;
            case STATUS_PRESSURIZED:
                Print("Airlock currently pressurized.");
                break;
            
            case STATUS_DEPRESSURIZED:
                Print("Depressurization completed.");
                break;
            default:
                _airlockStatus = GetVentStatus().ToString();
                break;
        }
        return;
    }

    _openDelay = 0;
    _lockDelay = 0;
    _pressureDelay = 0;

    // Trigger code
    switch (_airlockStatus)
    {
        case STATUS_PRESSURIZED:
            _airlockStatus = STATUS_DEPRESSURIZING;
            break;

        case STATUS_DEPRESSURIZED:
            _airlockStatus = STATUS_PRESSURIZING;
            break;
 
       case STATUS_PRESSURIZING:
            _airlockStatus = STATUS_DEPRESSURIZING;
            break;

        case STATUS_DEPRESSURIZING:
            _airlockStatus = STATUS_PRESSURIZING;
            break;

        default:
            break;
    }
}

private bool ShouldVentOxygen() {
    if (_ventExcessOxygen && airlockOxygenTanks.Count > 0) {
        foreach (var oxygenTank in airlockOxygenTanks) {
            Print("Oxygen fill percentage: " + oxygenTank.FilledRatio * 100);
            if (oxygenTank.FilledRatio * 100 < _oxygenExcessThreshold) {
                return false;
            }
        }
        return true;
    }
    return false;
}

private void OpenExteriorDoors() {
    foreach (var door in airlockExteriorDoors) {
        door.OpenDoor();
    }
}

private void CloseExteriorDoors() {
    foreach (var door in airlockExteriorDoors) {
        door.CloseDoor();
    }
}

private void LockExteriorDoors() {
    foreach (var door in airlockExteriorDoors) {
        door.Enabled = false;
    }
}

private void UnlockExteriorDoors() {
    foreach(var door in airlockExteriorDoors) {
        door.Enabled = true;
    }
}

private bool IsExteriorDoorsOpen() {
    foreach (var door in airlockExteriorDoors) {
        if (door.Status != DoorStatus.Closed) {
            return true;
        }
    }
    return false;
}

private void CloseInteriorDoors() {
    foreach (var door in airlockInteriorDoors) {
        door.CloseDoor();
    }
}

private void LockInteriorDoors() {
    foreach (var door in airlockInteriorDoors) {
        door.Enabled = false;
    }
}

private void OpenInteriorDoors() {
    foreach (var door in airlockInteriorDoors) {
        door.OpenDoor();
    }
}

private void UnlockInteriorDoors() {
    foreach (var door in airlockInteriorDoors) {
        door.Enabled = true;
    }
}

private bool IsInteriorDoorsOpen() {
    foreach (var door in airlockInteriorDoors) {
        if (door.Status != DoorStatus.Closed) {
            return true;
        }
    }
    return false;
}

private void Pressurize() {
    foreach (var vent in airVents) {
        vent.Depressurize = false;
    }
}

private void Depressurize() {
    foreach (var vent in airVents) {
        vent.Depressurize = true;
    }
}

private void EnableLightsAndSirens() {
    if (airlockWarningLights != null) {
        foreach (var warningLight in airlockWarningLights) {
            warningLight.Enabled = true;
        }
    }
    if (airlockWarningSirens != null) {
        foreach (var warningSiren in airlockWarningSirens) {
            warningSiren.Play();
        }
    }
}

private void DisableLightsAndSirens() {
    if (airlockWarningLights != null) {
        foreach (var warningLight in airlockWarningLights) {
            warningLight.Enabled = false;
        }
    }
    if (airlockWarningSirens != null) {
        foreach (var warningSiren in airlockWarningSirens) {
            warningSiren.Stop();
        }
    }
}

private VentStatus GetVentStatus() {
    return airVents[0].Status;
}

private float GetOxygen() {
    return airVents[0].GetOxygenLevel() * 100;
}

private bool IsPressurized() {
    return GetOxygen() >= _minimumPressureThreshold;
}

private bool IsDepressurized() {
    return GetOxygen() == 0;
}

private bool IsSafeOutside() {
    if (exteriorVent != null) {
        return GetExteriorOxygenLevel() >= _exteriorOxygenSafetyThreshold;
    }
    return false;
}

private float GetExteriorOxygenLevel() {
    if (exteriorVent != null) {
        return exteriorVent.GetOxygenLevel() * 100;
    }
    return 0;
}

private void Print(string text, bool append) {
    Echo(text);
    IMyTextSurface screen = Me.GetSurface(0);
    screen.ContentType = ContentType.TEXT_AND_IMAGE;
    screen.WriteText(text + "\n", append);
}

private void Print(string text) {
    Print(text, true);
}

private void UpdateInfoPanels() {
    if (airlockInfoPanels.Count == 0) {
        return;
    }
    foreach (var airlockInfoPanel in airlockInfoPanels) {
        IMyTextSurface screen = airlockInfoPanel.GetSurface(0);
        screen.ContentType = ContentType.TEXT_AND_IMAGE;
        screen.FontSize = 1.2F;
        screen.Alignment = TextAlignment.CENTER;
        screen.WriteText("Airlock Control Terminal\n\n", false);
        screen.WriteText("Airlock Status: " + _airlockStatus + "\n\n", true);
        if (airlockOxygenTanks.Count > 0) {
            double oxygenTankLevel = airlockOxygenTanks[0].FilledRatio * 100;
            screen.WriteText("Oxygen Tank Level: " + Convert.ToInt32(oxygenTankLevel) + "%\n", true);
        }
        screen.WriteText("Room Oxygen Level: " + Convert.ToInt32(GetOxygen()) + "%\n", true);
        if (exteriorVent != null) {
            screen.WriteText("Exterior Oxygen Level: " + Convert.ToInt32(GetExteriorOxygenLevel()) + "%\n", true);
        }
        
    }
}

private bool IsValidConfiguration() {
    IMyBlockGroup ventsGroup = GridTerminalSystem.GetBlockGroupWithName(_airlockVentsName);
    IMyBlockGroup airlockExteriorDoorsGroup = GridTerminalSystem.GetBlockGroupWithName(_airlockExteriorDoorsName);
    IMyBlockGroup airlockInteriorDoorsGroup = GridTerminalSystem.GetBlockGroupWithName(_airlockInteriorDoorsName);
    IMyBlockGroup airlockOxygenTanksGroup = GridTerminalSystem.GetBlockGroupWithName(_airlockOxygenTanksName);
    IMyBlockGroup airlockInfoPanelsGroup = GridTerminalSystem.GetBlockGroupWithName(_airlockInfoPanelsName);
    IMyBlockGroup airlockWarningLightsGroup = GridTerminalSystem.GetBlockGroupWithName(_airlockWarningLightsName);
    IMyBlockGroup airlockWarningSirensGroup = GridTerminalSystem.GetBlockGroupWithName(_airlockWarningSirensName);
    exteriorVent = GridTerminalSystem.GetBlockWithName(_exteriorVentName) as IMyAirVent;

    if (ventsGroup == null) { Print("Unable to detect airlock vent group named " + _airlockVentsName); return false; }
    Print("Airlock vents group named " + _airlockVentsName + " detected.");
    
    if (airlockExteriorDoorsGroup == null) { Print("Unable to detect exterior door group named " + _airlockExteriorDoorsName); return false; }
    Print("Exterior door group named " + _airlockExteriorDoorsName + " detected.");

    if (airlockInteriorDoorsGroup == null) { Print("Unable to detect interior door group named " + _airlockInteriorDoorsName); return false; }
    Echo("Interior door group named " + _airlockInteriorDoorsName + " detected.");

    if (_airlockOxygenTanksName != "" && airlockOxygenTanksGroup == null) {
        Print("Unable to detect airlock oxygen tanks group named " + _airlockOxygenTanksName
            + ". Disable with \"\" if you want to skip this feature.");
        return false;
    } else if (airlockOxygenTanksGroup != null) {
        airlockOxygenTanksGroup.GetBlocksOfType(airlockOxygenTanks);
        Print("Airlock oxygen tanks group named " + _airlockOxygenTanksName + " detected.");
    }

    if (_airlockInfoPanelsName != "" && airlockInfoPanelsGroup == null) {
        Print("Unable to detect airlock info panels group named " + _airlockInfoPanelsName
            + ". Disable with \"\" if you want to skip this feature.");
        return false;
    } else if (airlockInfoPanelsGroup != null) {
        airlockInfoPanelsGroup.GetBlocksOfType(airlockInfoPanels);
        Print("Airlock info panels group named " + _airlockInfoPanelsName + " detected.");
    }

    if (_airlockWarningLightsName != "" && airlockWarningLightsGroup == null) {
        Print("Unable to detect airlock warning lights group named " + _airlockWarningLightsName
            + ". Disable with \"\" if you want to skip this feature.");
        return false;
    } else if (airlockWarningLightsGroup != null) {
        airlockWarningLightsGroup.GetBlocksOfType(airlockWarningLights);
        Print("Airlock warning lights group named " + _airlockWarningLightsName + " detected.");
    }

    if (_airlockWarningSirensName != "" && airlockWarningSirensGroup == null) {
        Print("Unable to detect airlock warning sirens group named " + _airlockWarningSirensName
            + ". Disable with \"\" if you want to skip this feature.");
        return false;
    } else if (airlockWarningSirensGroup != null) {
        airlockWarningSirensGroup.GetBlocksOfType(airlockWarningSirens);
        Print("Airlock warning sirens group named" + _airlockWarningSirensName + " detected.");
    }

    if (_exteriorVentName != "" && exteriorVent == null) {
        Print("Unable to detect exterior air vent named " + _exteriorVentName
            + ". Disable with \"\" if you want to skip this feature.");
        return false;
    } else if (exteriorVent != null) {
        Print("Exterior air vent named " + _exteriorVentName + " detected.");
    }
   
    ventsGroup.GetBlocksOfType(airVents);
    airlockExteriorDoorsGroup.GetBlocksOfType(airlockExteriorDoors);
    airlockInteriorDoorsGroup.GetBlocksOfType(airlockInteriorDoors);
    
    return true;
}

