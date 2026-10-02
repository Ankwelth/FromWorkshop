/*
 *   R e a d m e
 *   -----------
 * 
 *  Ships Main Program Mk2
 *  http://steamcommunity.com/sharedfiles/filedetails/?id=1103394724
 *  Guide http://steamcommunity.com/sharedfiles/filedetails/?id=1103890032
 *  Replacement  of 
 *  http://steamcommunity.com/sharedfiles/filedetails/?id=423016495
 *  Customization Options  All these can be in custom data as well
 *  autolevel = flip mode
 *  autolevelon = autolevelon
 *  autolevelgyro ## 
 *  rocketgyro true = rocketgyro
 *  autolevel6 = flip mode
 *  autolevel6on = 6 * sec on
 *  
 *  shipStatusLCDName [name]
 *  orbitalStatusLCDName [name]
 * 
 *    ** Orbital Section **
 *    orbital off       
 *    gravity       currentgravity  targetgravity   changegravity
 *    altitude      currentaltitude targetaltitude  changealtitude
 *    hover         currenthover    targethover     changehover
 *    
 *    gravdrive           gravedriveon
 *    velocitylimit     heightoffset
 *    GravityEmergency      EmergencyThrusterName
 *    
 *    1.200.027 Bug fix in airlocks/air pressure
 *    1.200.000 Optimization pass on resource sinks. Hardened this code.
 *    1.199.020 Changed Closed to CanAccess
 *    1.197.181 Fixed bug in DisplayLargeNumbers
 *    1.192.022 shared project autolevel updated.
 *    1.192.022 Updated closed methods in all projects.
 *    1.190.009 Updated close method.  WriteToLCD now only looks at own construct
 *    1.190.008 Added support for Cockpit lcds.  
 *    1.190.008 Major update to LCD.  Change altitude from 999km to 99,999km.  Reduced text.
 *    1.189.041 Echo only if no LCD.  Changed so outside vent is saved to airpressure without doors.
 *    1.189.039 Power Update, Replace Commo system
 *    1.188.105 O2farm to own construct, minor optimization
 *    1.188.022 remove small vent bug adjustment
 */

// O.I.S. Ship Main Computer Mk 2

//Run options
bool stopShipIfNotPiloted = true;
bool runAutoLevel = false;//runs auto leveling in natural Gravity
bool rocketGyro = false;//false if RC bottom towards gravity, true if RC back toward gravity
bool runAutoLevel6 = false;//runs auto level 6/1 sec

//Customized Options
string shipStatusLCDName = "[ShipStatus]";
string orbitalStatusLCDName = "[ShipStatus]";
string commoTag = "OISheartbeat";
int o2TankLow = 40;
int o2TankHigh = 60;
int h2TankLow = 90;
int h2TankHigh = 100;
bool rebuildneeded = false;
int rebuildCounter = 0;
int rebuildCounterLimit = 10;
float solarAlign = 0.04f;
string sunChaserName = "[SunChaser]";
float sunTurn = 0.018f;
float sunOverride = .1f;
bool runSunChaser = false;
bool runSolarArray = true;
bool worldPressure = true;

//air system
string controlLightName = "Control";//airlock control light
string outsideDoorName = "Outer";//default outside airlock door
string insideDoorName = "Inner";//default inside airlock door
string outsideVentName = "outsideVent";//vents outside ship
int airPressureWarning = 50;//O2 % Damage at 50
//Hangar
string hangarPressurizeName = "Pressurize";// hangar control light
string hangarDepressurizeName = "Depressurize";// hangar control light

//helpers
List<string> reservedNamesStatic = new List<string> { "[LCD]", "[ShipStatus]", "[EmergencyThrust]", "[SunChaser]",
    "[GravDrive]", "[OrbitThruster]", "[GPSCommo]", "[Messages]", "[EnemyGPS]",  "" };//Names that are [name] but not airzones
List<Airlock> airlocks = new List<Airlock>();
List<RoomPressure> roomPressures = new List<RoomPressure>();
List<Hangar> hangars = new List<Hangar>();
List<LandingPad> landingPads = new List<LandingPad>();
List<IMyAirVent> outsideVents = new List<IMyAirVent>();
List<IMyGasTank> o2Tanks = new List<IMyGasTank>();
List<IMyGasTank> h2Tanks = new List<IMyGasTank>();
List<IMyGasGenerator> gasGenerators = new List<IMyGasGenerator>();
List<IMyOxygenFarm> o2Farms = new List<IMyOxygenFarm>();
List<SolarArray> solarArrays = new List<SolarArray>();
List<IMyRadioAntenna> outRadios = new List<IMyRadioAntenna>();
List<IMyLaserAntenna> outLasers = new List<IMyLaserAntenna>();
WriteLCD shipStatusLCD;
WriteLCD orbitalStatusLCD;
List<IMyTextSurface> pbText = new List<IMyTextSurface>();
Orbital orbitalComputer;
int statusTime = 0;
bool sendRadioUpdate = true;
int radioUpdateTime = 1800;//5 minutes
int runCounter = 1;
static readonly MyDefinitionId HydrogenEngineId = MyDefinitionId.Parse("MyObjectBuilder_HydrogenEngine/");
static readonly MyDefinitionId HydrogenGasId = MyDefinitionId.Parse("MyObjectBuilder_GasProperties/Hydrogen");
static readonly MyDefinitionId OxygenGasId = MyDefinitionId.Parse("MyObjectBuilder_GasProperties/Oxygen");

//Method helpers
List<string> tempZoneNames = new List<string>();
List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
List<IMyInteriorLight> lightsA = new List<IMyInteriorLight>();
List<IMyInteriorLight> lightsB = new List<IMyInteriorLight>();
List<IMyAirVent> vents = new List<IMyAirVent>();
List<IMyAirVent> ventsA = new List<IMyAirVent>();
List<IMyAirVent> ventsB = new List<IMyAirVent>();
List<IMyDoor> doors = new List<IMyDoor>();
List<IMyDoor> doorsA = new List<IMyDoor>();
List<IMyDoor> doorsB = new List<IMyDoor>();
List<IMyShipConnector> connectors = new List<IMyShipConnector>();
List<IMyLandingGear> landingGears = new List<IMyLandingGear>();
List<IMyGasTank> localGasTanks = new List<IMyGasTank>();
List<IMySoundBlock> sounds = new List<IMySoundBlock>();
List<IMySensorBlock> sensors = new List<IMySensorBlock>();
double o2percentFull = 0;
double h2percentFull = 0;
MyResourceSinkComponent sink;
MyResourceSourceComponent source;
ListReader<MyDefinitionId> list;
string[] argMessages = new string[10];
string[] pieces = new string[2];
float currentPower = 0f;
float maxPower = 0f;
float percentPower = 0f;
List<IMyMotorStator> rotors = new List<IMyMotorStator>();
List<IMySolarPanel> panels = new List<IMySolarPanel>();
List<IMyReactor> reactors = new List<IMyReactor>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();//not used
List<IMyPowerProducer> power = new List<IMyPowerProducer>();
List<IMySolarPanel> sunChaserPanels = new List<IMySolarPanel>();
List<IMyGyro> sunChaserGyro = new List<IMyGyro>();
List<IMyThrust> sunChaserForeThrust = new List<IMyThrust>();
List<IMyThrust> sunChaserAftThrust = new List<IMyThrust>();

public Program() {
    if(Me.SurfaceCount >0) pbText.Add(Me.GetSurface(0));
    foreach(IMyTextSurface de in pbText) de.ContentType = ContentType.TEXT_AND_IMAGE;
    orbitalComputer = new Orbital(this);
    ArgumentParser(Me.CustomData);
    if (Storage.Length > 0) {
        ArgumentParser(Storage);
    }
    shipStatusLCD = new WriteLCD(this, shipStatusLCDName);
    orbitalStatusLCD = new WriteLCD(this, orbitalStatusLCDName);
    GridTerminalSystem.GetBlocksOfType(outRadios, b => b.CubeGrid == Me.CubeGrid);
    GridTerminalSystem.GetBlocksOfType(outLasers, b => b.CubeGrid == Me.CubeGrid);
    ZoneConstruction();
    worldPressure = WorldPressureTurnedOn();
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Save() {
    StringBuilder writeToStorage = new StringBuilder();
    if (orbitalComputer.OrbitalMode == Orbital.OrbitalOperation.Off) writeToStorage.Append("orbitaloff\n");
    if (orbitalComputer.OrbitalMode == Orbital.OrbitalOperation.AltitudeMode) {
        writeToStorage.Append($"altitude\ntargetaltitude {orbitalComputer.AltitudeTarget}\n");
    }
    if (orbitalComputer.OrbitalMode == Orbital.OrbitalOperation.GravityMode) {
        writeToStorage.Append($"gravity\ntargetgravity {orbitalComputer.GravityTarget}\n");
    }
    if (orbitalComputer.OrbitalMode == Orbital.OrbitalOperation.HoverMode) {
        writeToStorage.Append($"hover\ntargethover {orbitalComputer.HoverTarget}\n");
    }
    Storage = writeToStorage.ToString();
}

public void Main(string argument, UpdateType updateSource) {
    Echo(argument);
    if (argument == "rebuild") {
        ZoneConstruction();
        //return;
    }
    if (argument.Length > 0) {
        ArgumentParser(argument);
        //return;
    }
    if (rebuildneeded) {
        if (rebuildCounter++ > rebuildCounterLimit) {
            rebuildneeded = false;
            rebuildCounter = 0;
            ZoneConstruction();
            //return;
        }
    }
    if (rebuildneeded) { return; }
    if ((updateSource & UpdateType.Update10) == 0) { return; }
    //in update 10
    //shipStatusLCD.WriteToLCD($"\n{runCounter}\n{Runtime.LastRunTimeMs}");
    switch (runCounter++) {
        case 1:
            shipStatusLCD.WriteToLCD(Me.CubeGrid.DisplayName);
            shipStatusLCD.WriteToLCD($"\n {GetPowerOutputString()}");
            if (stopShipIfNotPiloted) shipStatusLCD.WriteToLCD($"\n Ship Piloted : {!StopShipIfNotPiloted()}");
            if (runAutoLevel) {
                if (rocketGyro) {
                    shipStatusLCD.WriteToLCD($"\n{GyroMain("Rocket")}");
                }
                else {
                    shipStatusLCD.WriteToLCD($"\n{GyroMain("")}");
                }
            }
            //Orbital();
            orbitalComputer.Update();
            orbitalStatusLCD.WriteToLCD(orbitalComputer.Status);
            break;
        case 2:
            GasTank(o2TankLow, o2TankHigh, h2TankLow, h2TankHigh);
            shipStatusLCD.WriteToLCD($"\n O2 Tanks {(o2percentFull == -1 ? "None" : $"{o2percentFull:n2}%")}," +
                $" H2 Tanks {(h2percentFull == -1 ? "None" : $"{h2percentFull:n2}%")}");
            if (runSolarArray) {
                foreach (SolarArray de in solarArrays) {
                    if (!de.Update()) {
                        rebuildneeded = true;
                        rebuildCounter = rebuildCounterLimit + 1;
                    }
                    shipStatusLCD.WriteToLCD($"\n Array Power {DisplayLargeNumber(de.Power * 1000000)}W");
                }
            }
            if (runAutoLevel && runAutoLevel6) {
                if (rocketGyro) GyroMain("Rocket");
                //else GyroMain("");
                else GyroMain("");
            }
            break;
        case 3:
            if (worldPressure) {
                foreach (Hangar de in hangars) {
                    if (!de.Update()) {
                        rebuildneeded = true;
                        rebuildCounter = rebuildCounterLimit + 1;
                    }
                    shipStatusLCD.WriteToLCD($"\n {de.Status}");
                }
            }
            else { shipStatusLCD.WriteToLCD("\n World Pressure Off.\n  or no vents"); }
            if (runAutoLevel && runAutoLevel6) {
                if (rocketGyro) GyroMain("Rocket");
                else GyroMain("");
            }
            break;
        case 4:
            if (worldPressure) {
                foreach (Airlock de in airlocks) {
                    if (!de.Update()) {
                        rebuildneeded = true;
                        rebuildCounter = rebuildCounterLimit + 1;
                    }
                    if (de.AirlockPressure < airPressureWarning) shipStatusLCD.WriteToLCD($"\n {de.Status}");
                }
            }
            if (runAutoLevel && runAutoLevel6) {
                if (rocketGyro) GyroMain("Rocket");
                else GyroMain("");
            }
            break;
        case 5:
            if (worldPressure) {
                foreach (RoomPressure de in roomPressures) {
                    if (!de.Update()) {
                        rebuildneeded = true;
                        rebuildCounter = rebuildCounterLimit + 1;
                    }
                    if (de.RoomVentPressure < airPressureWarning) shipStatusLCD.WriteToLCD($"\n {de.Status}");
                }
            }
            if (runAutoLevel && runAutoLevel6) {
                if (rocketGyro) GyroMain("Rocket");
                else GyroMain("");
            }
            break;
        case 6:
            foreach (LandingPad de in landingPads) {
                if (!de.Update()) {
                    rebuildneeded = true;
                    rebuildCounter = rebuildCounterLimit + 1;
                }
            }
            if (runAutoLevel && runAutoLevel6) {
                if (rocketGyro) GyroMain("Rocket");
                else GyroMain("");
            }
            if (runSunChaser) {
                SunChaser(sunChaserName, sunTurn, sunOverride);
                orbitalStatusLCD.WriteToLCD("\n SunChaser active");
            }/*
                    if (shipStatusLCD.Lcds.Count == 0) {
                        echoString = $"Ship Main Computer MK2\n{shipStatusLCD.ToString()}{orbitalStatusLCD.ToString()}";
                    }*/
            //temp playing
            foreach(IMyTextSurface de in pbText) de.WriteText($"Ship Main Computer MK2\n{shipStatusLCD.ToString()}{orbitalStatusLCD.ToString()}");
            shipStatusLCD.FlushToLCD();
            orbitalStatusLCD.FlushToLCD(shipStatusLCDName == orbitalStatusLCDName);
            runCounter = 1;
            break;
        default:
            runCounter = 1;
            break;
    }
    if (sendRadioUpdate && ++statusTime > radioUpdateTime) {
        statusTime = 0;
        IGC.SendBroadcastMessage(commoTag, $"MSG;anyone;{Me.CubeGrid.DisplayName};;Status:{GetPowerOutputString()}\nStatus: O2 {(o2percentFull == -1 ? "None" : $"{ o2percentFull:n2}% ")}, H2 {(h2percentFull == -1 ? "None" : $"{h2percentFull:n2}%")}");
    }
}

void ArgumentParser(string argument) {
    argMessages = argument.Split('\n');
    int tempInt = 0;
    double tempDouble = 0;
    bool test = false;
    foreach (string de in argMessages) {
        pieces = de.Split(' ');
        if (pieces.Count() < 1) continue;
        //at least 1 pieces below this
        if (pieces[0] == "autolevel") {
            runAutoLevel = !runAutoLevel;
            if (!runAutoLevel) {
                foreach (IMyGyro fr in gyros) fr.GyroOverride = false;
            }
        }
        if (pieces[0] == "autolevelon") runAutoLevel = true;
        if (pieces[0] == "autolevel6") runAutoLevel6 = !runAutoLevel6;
        if (pieces[0] == "autolevel6on") runAutoLevel6 = true;
        if (pieces[0] == "goland") orbitalComputer.GoLand();
        if (pieces[0] == "godeepspace") orbitalComputer.GoDeepSpace();
        if (pieces[0] == "gohover") orbitalComputer.GoHover();
        if (pieces[0] == "orbitaloff") orbitalComputer.TurnOffOrbital();
        if (pieces[0] == "orbitalrebuild") orbitalComputer.OrbitalListNeedsBuilding = true;
        if (pieces[0] == "gravity") orbitalComputer.OrbitalMode = Orbital.OrbitalOperation.GravityMode;
        if (pieces[0] == "altitude") orbitalComputer.OrbitalMode = Orbital.OrbitalOperation.AltitudeMode;
        if (pieces[0] == "hover") orbitalComputer.OrbitalMode = Orbital.OrbitalOperation.HoverMode;
        if (pieces[0] == "currentaltitude") orbitalComputer.CurrentAltitude();
        if (pieces[0] == "currentgravity") orbitalComputer.CurrentGravity();
        if (pieces[0] == "currenthover") orbitalComputer.CurrentHover();
        if (pieces[0] == "gravdrive") {
            orbitalComputer.GravDriveOn = !orbitalComputer.GravDriveOn;
        }
        if (pieces[0] == "gravdriveon") {
            orbitalComputer.GravDriveOn = true;
        }
        if (pieces[0] == "sunchaser") {
            runSunChaser = !runSunChaser;
            if (!runSunChaser) {
                foreach (IMyGyro fr in sunChaserGyro) fr.GyroOverride = false;
            }
        }
        if (pieces[0] == "sunchaseron") runSunChaser = true;
        if (pieces[0] == "rebuildlcd") {
            shipStatusLCD.LCDBuild(shipStatusLCDName);
            orbitalStatusLCD.LCDBuild(orbitalStatusLCDName);
        }
        if (pieces[0] == "solararrayon") runSolarArray = true;
        if (pieces[0] == "solararrayoff") runSolarArray = false;
        if (pieces.Count() < 2) continue;
        //at least 2 pieces below this
        if (pieces[0] == "shipStatusLCDName") {
            shipStatusLCDName = de.Substring("shipStatusLCDName ".Length);
        }
        if (pieces[0] == "orbitalStatusLCDName") {
            orbitalStatusLCDName = de.Substring("orbitalStatusLCDName ".Length);
        }
        if (pieces[0] == "o2TankLow") {
            if (int.TryParse(pieces[1], out tempInt)) o2TankLow = tempInt;
        }
        if (pieces[0] == "o2TankHigh") {
            if (int.TryParse(pieces[1], out tempInt)) o2TankHigh = tempInt;
        }
        if (pieces[0] == "h2TankLow") {
            if (int.TryParse(pieces[1], out tempInt)) h2TankLow = tempInt;
        }
        if (pieces[0] == "h2TankHigh") {
            if (int.TryParse(pieces[1], out tempInt)) h2TankHigh = tempInt;
        }
        if (pieces[0] == "rebuild") {
            if (int.TryParse(pieces[1], out tempInt)) {
                rebuildCounterLimit = tempInt;
                rebuildneeded = true;
                rebuildCounter = 0;
            }
        }
        if (pieces[0] == "stopShipIfNotPiloted") {
            if (bool.TryParse(pieces[1], out test)) {
                stopShipIfNotPiloted = test;
            }
        }
        if (pieces[0] == "rocketGyro") {
            if (bool.TryParse(pieces[1], out test)) {
                rocketGyro = test;
            }
        }
        if (pieces[0] == "targetaltitude") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.AltitudeTarget = tempDouble;
            }
        }
        if (pieces[0] == "targethover") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.HoverTarget = tempDouble;
            }
        }
        if (pieces[0] == "targetgravity") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.GravityTarget = tempDouble;
            }
        }
        if (pieces[0] == "velocitylimit") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.VelocityLimit = tempDouble;
            }
        }
        if (pieces[0] == "changealtitude") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.AltitudeTarget = orbitalComputer.AltitudeTarget + tempDouble;
            }
        }
        if (pieces[0] == "changehover") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.HoverTarget = orbitalComputer.HoverTarget + tempDouble;
            }
        }
        if (pieces[0] == "changegravity") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.GravityTarget = orbitalComputer.GravityTarget + tempDouble;
            }
        }
        if (pieces[0] == "heightoffset") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.HeightOffset = tempDouble;
            }
        }
        if (pieces[0] == "EmergencyThrusterName") {
            orbitalComputer.EmergencyThrusterName = de.Substring("EmergencyThrusterName ".Length);
        }
        if (pieces[0] == "GravityEmergency") {
            if (double.TryParse(pieces[1], out tempDouble)) {
                orbitalComputer.GravityEmergency = tempDouble;
            }
        }
        if (pieces[0] == "autolevelgyro") {
            if (int.TryParse(pieces[1], out tempInt)) {
                LIMIT_GYROS = tempInt;
                GridTerminalSystem.GetBlocksOfType<IMyGyro>(null, b => {
                    b.GyroOverride = false;
                    return false;
                });
                gyrosetup();
            }
        }
    }
}

string DisplayLargeNumber(float number) {
    if (float.IsNaN(number)) return "NaN";
    if (float.IsInfinity(number)) return "Infinity";
    string powerValue = " kMGTPEZY";
    float result = number;
    int ordinal = 0;
    while (ordinal < powerValue.Length-1 && result >= 1000) {
        result /= 1000;
        ordinal++;
    }
    string resultString = Math.Round(result, 1, MidpointRounding.AwayFromZero).ToString();
    if (ordinal > 0 && ordinal < 10) {
        resultString += " " + powerValue[ordinal];
    }
    return resultString;
}

void GasTank(int o2turnOn, int o2turnOff, int h2turnOn, int h2turnOff) {
    o2percentFull = GetTankFill(o2Tanks);
    var o2p = o2percentFull;
    h2percentFull = GetTankFill(h2Tanks);
    var h2p = h2percentFull;
    if (o2percentFull == -1 && h2percentFull == -1) return;
    if (h2percentFull == -1) h2p = h2turnOff;
    if (o2percentFull == -1) o2p = o2turnOff;
    if (o2p >= o2turnOff && h2p >= h2turnOff) {
        foreach (IMyGasGenerator de in gasGenerators) de.Enabled = false;
        foreach (IMyOxygenFarm de in o2Farms) de.Enabled = false;
    }
    else if (o2p <= o2turnOn || h2p <= h2turnOn) {
        foreach (IMyGasGenerator de in gasGenerators) de.Enabled = true;
        foreach (IMyOxygenFarm de in o2Farms) de.Enabled = true;
    }
}

string GetPowerOutputString() {
    GetPowerOutput();
    return $"Power: {percentPower:n2}%, current {DisplayLargeNumber(currentPower)}, max {DisplayLargeNumber(maxPower)}";
}

void GetPowerOutput() {
    currentPower = 0;
    maxPower = 0;
    percentPower = 0;
    foreach (var de in reactors) {
        if (de.Enabled) {
            currentPower += de.CurrentOutput;
            maxPower += de.MaxOutput;
        }
    }
    foreach (var de in power) {
        currentPower += de.CurrentOutput;
        maxPower += de.MaxOutput;
    }
    //all in MW,m need to change to watts
    maxPower *= 1000000;
    currentPower *= 1000000;
    if (maxPower > 0) { percentPower = (currentPower / maxPower) * 100; } else { percentPower = 100; }
}

double GetTankFill(List<IMyGasTank> tanks) {
    int tanksCount = 0;
    //get percent in tanks
    double tanksFill = 0;
    foreach (IMyGasTank de in tanks) {
        tanksFill += de.FilledRatio * 100;
        ++tanksCount;
        //}
    }
    if (tanksCount == 0) return -1;
    tanksFill /= tanksCount;
    return tanksFill;
}

List<string> GetZoneNames(List<string> reservedNames) {
    var zoneList = new List<string>();
    string tempName = "";
    GridTerminalSystem.SearchBlocksOfName("[", null, b => {
        if (b.CustomName.Contains(']')) {
            tempName = b.CustomName.Substring(b.CustomName.IndexOf('['), b.CustomName.IndexOf(']') - b.CustomName.IndexOf('[') + 1);
        }
        else { tempName = ""; }
        if (!reservedNames.Contains(tempName) && !zoneList.Contains(tempName)) {
            zoneList.Add(tempName);
        }
        return false;
    });
    return zoneList;
}

void GyroPitch(List<IMyGyro> gyro, float sunTurn) {
    foreach (IMyGyro de in gyro) {
        de.GyroOverride = true;
        de.Yaw = 0;
        de.Pitch = -sunTurn;
        de.Roll = 0;
    }
}

void GyroYaw(List<IMyGyro> gyro, float sunTurn) {
    foreach (IMyGyro de in gyro) {
        de.GyroOverride = true;
        de.Yaw = sunTurn;
        de.Pitch = 0;
        de.Roll = 0;
    }
}

bool ShipIsPiloted() {
    bool piloted = false;
    GridTerminalSystem.GetBlocksOfType<IMyShipController>(null, b => {
        if (b.IsUnderControl) {
            piloted = true; ;
        }
        return false;
    });
    return piloted;
}

bool StopShipIfNotPiloted() {
    if (!ShipIsPiloted()) {
        GridTerminalSystem.GetBlocksOfType<IMyShipController>(null, b => {
            b.DampenersOverride = true;
            return false;
        });
        return true;
    }
    return false;
}

void SunChaser(string sunChaserName, float sunTurn, float sunOverride) {
    bool sunMoveStarted = false;
    bool sunTurnStarted = false;
    foreach (IMySolarPanel de in sunChaserPanels) {
        if (de.CustomName.Contains("Port") && de.MaxOutput > 0) {
            sunTurnStarted = true;
            GyroYaw(sunChaserGyro, -sunTurn);
        }
        else if ((de.CustomName.Contains("Starboard") || de.CustomName.Contains("Aft")) && de.MaxOutput > 0) {
            sunTurnStarted = true;
            GyroYaw(sunChaserGyro, sunTurn);
        }
        else if (de.CustomName.Contains("Above") && de.MaxOutput > 0) {
            sunTurnStarted = true;
            GyroPitch(sunChaserGyro, sunTurn);
        }
        else if (de.CustomName.Contains("Below") && de.MaxOutput > 0) {
            sunTurnStarted = true;
            GyroPitch(sunChaserGyro, -sunTurn);
        }
        if (de.CustomName.Contains("Fore") && de.MaxOutput > 0) {
            sunMoveStarted = true;
            foreach (IMyThrust fr in sunChaserForeThrust) fr.Enabled = false;
            foreach (IMyThrust fr in sunChaserAftThrust) fr.ThrustOverridePercentage = sunOverride;
        }
    }
    if (!sunMoveStarted) {
        foreach (IMyThrust fr in sunChaserForeThrust) fr.Enabled = true;
        foreach (IMyThrust fr in sunChaserAftThrust) fr.ThrustOverridePercentage = 0;
    }
    if (!sunTurnStarted) {
        foreach (IMyGyro de in sunChaserGyro) de.GyroOverride = false;
    }
}

bool WorldPressureTurnedOn() {
    var vents = new List<IMyAirVent>();
    GridTerminalSystem.GetBlocksOfType(vents);
    if (vents.Count < 1) { return false; }
    return vents[0].PressurizationEnabled;
}

void ZoneConstruction() {
    ZoneConstruction(GetZoneNames(reservedNamesStatic));
}
void ZoneConstruction(List<string> zoneList) {
    foreach(IMyTextSurface de in pbText) de.ContentType = ContentType.TEXT_AND_IMAGE;
    //clear
    airlocks.Clear();
    roomPressures.Clear();
    hangars.Clear();
    landingPads.Clear();
    outsideVents.Clear();
    o2Tanks.Clear();
    h2Tanks.Clear();
    gasGenerators.Clear();
    o2Farms.Clear();
    solarArrays.Clear();
    rotors.Clear();
    panels.Clear();
    sunChaserPanels.Clear();
    sunChaserGyro.Clear();
    sunChaserForeThrust.Clear();
    sunChaserAftThrust.Clear();
    GridTerminalSystem.GetBlocksOfType(outsideVents, b =>
        b.CubeGrid == Me.CubeGrid && b.CustomName.Contains(outsideVentName));
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(null, b => {
        if (b.CubeGrid == Me.CubeGrid) {
            if(! b.Components.TryGet<MyResourceSinkComponent>(out sink))return false;
            list = sink.AcceptedResources;
            for (int j = 0; j < list.Count; ++j) {
                if (list[j] == OxygenGasId) o2Tanks.Add(b);
                if (list[j] == HydrogenGasId) h2Tanks.Add(b);
            }
        }
        return false;
    });
    GridTerminalSystem.GetBlocksOfType(gasGenerators, b => {
        if (b.CubeGrid == Me.CubeGrid) {
            if(! b.Components.TryGet<MyResourceSourceComponent>(out source)) return false;
            list = source.ResourceTypes;
            for (int j = 0; j < list.Count; ++j) {
                if (list[j] == OxygenGasId) return true;
                if (list[j] == HydrogenGasId) return true;
            }
        }
            return false;
    });
    GridTerminalSystem.GetBlocksOfType(o2Farms, b =>
        b.CubeGrid.IsSameConstructAs(Me.CubeGrid));
    GridTerminalSystem.GetBlocksOfType(rotors);
    foreach (IMyMotorStator v in rotors) {
        if (!v.IsSameConstructAs(Me)) continue;
        GridTerminalSystem.GetBlocksOfType(panels, b => b.CubeGrid == v.TopGrid);
        if (panels.Count > 0) {
            solarArrays.Add(new SolarArray(v, new List<IMySolarPanel>(panels), this, solarAlign));
        }
    }
    reactors.Clear();
    power.Clear();
    GridTerminalSystem.GetBlocksOfType(power, b => {
        if (b is IMyReactor) {
            reactors.Add((IMyReactor)b);
            return false;
        }
        return true;
    });
    GridTerminalSystem.SearchBlocksOfName(sunChaserName, null, b => {
        if (b.CubeGrid == Me.CubeGrid) {
            if (b is IMySolarPanel) sunChaserPanels.Add((IMySolarPanel)b);
            else if (b is IMyGyro) sunChaserGyro.Add((IMyGyro)b);
            else if (b is IMyThrust && b.CustomName.Contains("Fore")) sunChaserForeThrust.Add((IMyThrust)b);
            else if (b is IMyThrust && b.CustomName.Contains("Aft")) sunChaserAftThrust.Add((IMyThrust)b);
        }
        return false;
    });
    //walk through each possible zone
    foreach (string de in zoneList) {
        bool controlExists = false;
        bool hangerPressurizeExists = false;
        bool hangerDepressurizeExists = false;
        lights.Clear();
        lightsA.Clear();
        lightsB.Clear();
        doors.Clear();
        vents.Clear();
        connectors.Clear();
        landingGears.Clear();
        localGasTanks.Clear();
        sounds.Clear();
        sensors.Clear();
        bool outsideDoorExist = false;
        bool insideDoorExist = false;
        GridTerminalSystem.SearchBlocksOfName(de, null, b => {
            if (!(b.CubeGrid == Me.CubeGrid)) return false;
            if (b is IMyInteriorLight) {
                lights.Add((IMyInteriorLight)b);
                if (b.CustomName.Contains(controlLightName)) controlExists = true;
                else if (b.CustomName.Contains(hangarPressurizeName)) {
                    hangerPressurizeExists = true;
                    lightsA.Add((IMyInteriorLight)b);
                }
                else if (b.CustomName.Contains(hangarDepressurizeName)) {
                    hangerDepressurizeExists = true;
                    lightsB.Add((IMyInteriorLight)b);
                }
            }
            else if (b is IMyDoor) {
                doors.Add((IMyDoor)b);
                if (b.CustomName.Contains(outsideDoorName)) outsideDoorExist = true;
                if (b.CustomName.Contains(insideDoorName)) insideDoorExist = true;
            }
            else if (b is IMyAirVent) {
                vents.Add((IMyAirVent)b);
            }
            else if (b is IMyShipConnector) {
                connectors.Add((IMyShipConnector)b);
            }
            else if (b is IMyLandingGear) {
                landingGears.Add((IMyLandingGear)b);
            }
            else if (b is IMyGasTank) {
                b.Components.TryGet<MyResourceSinkComponent>(out sink);
                list = sink.AcceptedResources;
                for (int j = 0; j < list.Count; ++j) {
                    if (list[j] == OxygenGasId) localGasTanks.Add((IMyGasTank)b);
                }
            }
            else if (b is IMySoundBlock) {
                sounds.Add((IMySoundBlock)b);
            }
            else if (b is IMySensorBlock) {
                sensors.Add((IMySensorBlock)b);
            }
            return false;
        });
        //airlock?
        if (controlExists && vents.Count > 0 && doors.Count > 0) {
            doorsA.Clear();
            doorsB.Clear();
            ventsA.Clear();
            ventsB.Clear();
            if (localGasTanks.Count == 0) localGasTanks = new List<IMyGasTank>(o2Tanks);
            //standard airlock
            if (outsideDoorExist && insideDoorExist) {
                foreach (IMyDoor fr in doors) {
                    if (fr.CustomName.Contains(outsideDoorName)) {
                        doorsA.Add(fr);
                    }
                    else if (fr.CustomName.Contains(insideDoorName)) {
                        doorsB.Add(fr);
                    }
                }
                if (outsideVents.Count > 0) {
                    airlocks.Add(new Airlock(de, vents, lights, doorsA, doorsB, localGasTanks, this, airPressureWarning, outsideVents, new List<IMyAirVent>()));
                    continue;
                }
                else {
                    airlocks.Add(new Airlock(de, vents, lights, doorsA, doorsB, localGasTanks, this, airPressureWarning));
                    continue;
                }
            }
            //? ? airlock, try to figure out [zonename]
            tempZoneNames.Clear();
            foreach (IMyDoor fr in doors) {
                int startIndex = fr.CustomName.IndexOf(de);
                string tempString = fr.CustomName.Remove(startIndex, de.Length);
                string tempName = "";
                if (tempString.Contains('[') && tempString.Contains(']') && (tempString.IndexOf('[') < tempString.IndexOf(']'))) {
                    //second zone name found
                    tempName = tempString.Substring(tempString.IndexOf('['), tempString.IndexOf(']') - tempString.IndexOf('[') + 1);
                    if (!tempZoneNames.Contains(tempName) && !reservedNamesStatic.Contains(tempName)) {
                        tempZoneNames.Add(tempName);
                    }
                }
            }
            //BackWards compatiblity with older ships
            if (tempZoneNames.Count == 1 && outsideDoorExist) {
                tempZoneNames.Add(outsideDoorName);
            }
            if (tempZoneNames.Count == 2) {
                //valid airlock
                foreach (IMyDoor fr in doors) {
                    if (fr.CustomName.Contains(tempZoneNames[0])) {
                        doorsA.Add(fr);
                    }
                    else if (fr.CustomName.Contains(tempZoneNames[1])) {
                        doorsB.Add(fr);
                    }
                }
                GridTerminalSystem.GetBlocksOfType<IMyAirVent>(null, b => {
                    if (b.CustomName.Contains(tempZoneNames[0])) {
                        ventsA.Add(b);
                    }
                    else if (b.CustomName.Contains(tempZoneNames[1])) {
                        ventsB.Add(b);
                    }
                    return false;
                });
                if (ventsA.Count > 0 && ventsB.Count > 0 && doorsA.Count > 0 && doorsB.Count > 0) {
                    airlocks.Add(new Airlock(de, vents, lights, doorsA, doorsB, localGasTanks, this, airPressureWarning, ventsA, ventsB));
                    continue;
                }
                else if (ventsA.Count > 0 && tempZoneNames.Contains(outsideDoorName) && doorsA.Count > 0 && doorsB.Count > 0) {
                    airlocks.Add(new Airlock(de, vents, lights, doorsB, doorsA, localGasTanks, this, airPressureWarning, new List<IMyAirVent>(), ventsA));
                    continue;
                }
            }
        }
        //No Airlock found
        //Hanger
        if (hangerPressurizeExists && hangerDepressurizeExists && vents.Count > 0) {
            foreach (IMyInteriorLight fe in lightsA) lights.Remove(fe);
            foreach (IMyInteriorLight fe in lightsB) lights.Remove(fe);
            if (localGasTanks.Count == 0) localGasTanks = new List<IMyGasTank>(o2Tanks);
            hangars.Add(new Hangar(de, lightsA, lightsB, vents, lights, sounds, doors, sensors, outsideVents, localGasTanks, this, airPressureWarning));
            continue;
        }
        //RoomPressure
        if (vents.Count > 0) {
            if (de.Contains(outsideVentName)) {
                roomPressures.Add(new RoomPressure(de, vents, lights, new List<IMyDoor>(), this, airPressureWarning));
            }
            else {
                roomPressures.Add(new RoomPressure(de, vents, lights, doors, this, airPressureWarning));
            }
            continue;
        }//RoomPressure
        //Landing Pad
        if ((landingGears.Count > 0 && (connectors.Count > 0 || lights.Count > 0)) || (connectors.Count > 0 && (landingGears.Count > 0 || lights.Count > 0))) {
            landingPads.Add(new LandingPad(de, landingGears, lights, connectors));
            continue;
        }
    }
    //not built
}

/// <summary>
        /// Airlock system.  Default side A = space, side B = inside.
        /// </summary>
public class Airlock {

    private Program _script;
    public string ZoneName { get; set; }
    public List<IMyAirVent> Vents { get; set; }
    public List<IMyInteriorLight> Lights { get; set; }
    public List<IMyDoor> DoorsA { get; set; }
    public List<IMyDoor> DoorsB { get; set; }
    public List<IMyAirVent> VentsA { get; set; }
    public List<IMyAirVent> VentsB { get; set; }
    public List<IMyGasTank> O2Tanks { get; set; }
    public string Status {
        get { return StatusBuilder.ToString(); }
        set {
            StatusBuilder.Clear();
            StatusBuilder.Append(value);
        }
    }
    private StringBuilder StatusBuilder = new StringBuilder();
    public string AirlockStatus { get; private set; }
    public int AirLimit { get; set; }

    public float AirlockPressure { get; private set; } = 0;
    private bool toBreakOut = true;
    public bool AirSideA { get; private set; } = false;
    public bool AirSideB { get; private set; } = true;
    public double O2TanksFill { get; private set; } = 0;
    public int AirTanksCount { get; private set; } = 0;
    private MyResourceSinkComponent sink;
    private ListReader<MyDefinitionId> list;

    public Airlock(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, List<IMyDoor> doorsSideA, List<IMyDoor> doorsSideB,
        List<IMyGasTank> o2Tanks, int airLimit = 5) : this(zoneName, vents, lights, doorsSideA, doorsSideB, o2Tanks, airLimit,
            new List<IMyAirVent>(), new List<IMyAirVent>()) {
    }

    public Airlock(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, List<IMyDoor> doorsSideA, List<IMyDoor> doorsSideB,
        List<IMyGasTank> o2Tanks, Program script, int airLimit = 5) : this(zoneName, vents, lights, doorsSideA, doorsSideB, o2Tanks, script, airLimit,
        new List<IMyAirVent>(), new List<IMyAirVent>()) {
    }

    public Airlock(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, List<IMyDoor> doorsSideA, List<IMyDoor> doorsSideB,
        List<IMyGasTank> o2Tanks, Program script, int airLimit, List<IMyAirVent> ventsSideA, List<IMyAirVent> ventsSideB) : this(zoneName, vents, lights, doorsSideA, doorsSideB, o2Tanks, airLimit, ventsSideA, ventsSideB) {
        _script = script;
    }

    public Airlock(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, List<IMyDoor> doorsSideA, List<IMyDoor> doorsSideB,
        List<IMyGasTank> o2Tanks, int airLimit, List<IMyAirVent> ventsSideA, List<IMyAirVent> ventsSideB) {
        ZoneName = zoneName;
        Vents = new List<IMyAirVent>(vents);
        Lights = new List<IMyInteriorLight>(lights);
        DoorsA = new List<IMyDoor>(doorsSideA);
        DoorsB = new List<IMyDoor>(doorsSideB);
        VentsA = new List<IMyAirVent>(ventsSideA);
        VentsB = new List<IMyAirVent>(ventsSideB);
        for (int i=0; i<o2Tanks.Count; ++i) {
            o2Tanks[i].Components.TryGet<MyResourceSinkComponent>(out sink);
            list = sink.AcceptedResources;
            bool hasO2 = false;
            foreach (var de in list) {
                if (de.SubtypeId.ToString() == "Oxygen") hasO2 = true;
            }
            if (!hasO2) {
                o2Tanks.RemoveAtFast(i);
                --i;
            }
        }
        O2Tanks = new List<IMyGasTank>(o2Tanks);
        AirLimit = airLimit;
        Status = "";
        //RoomPressure = new RoomPressure();
    }

    public bool Closed(IMyTerminalBlock block) {
        return !(_script?.GridTerminalSystem.CanAccess(block) ?? !block.Closed);
    }

    public bool Update() {
        if (Vents.Count == 0 || Lights.Count == 0 || DoorsA.Count == 0 || DoorsB.Count == 0) return false;
        foreach (var de in Vents) if (Closed(de)) return false;
        foreach (var de in Lights) if (Closed(de)) return false;
        foreach (var de in DoorsA) if (Closed(de)) return false;
        foreach (var de in DoorsB) if (Closed(de)) return false;
        foreach (var de in VentsA) if (Closed(de)) return false;
        foreach (var de in VentsB) if (Closed(de)) return false;
        toBreakOut = true;
        //RoomPressure handling
        AirlockPressure = Vents[0].GetOxygenLevel() * 100;
        if (AirlockPressure > AirLimit) {
            foreach (IMyInteriorLight de in Lights) de.Color = Color.White;
        } else {
            foreach (IMyInteriorLight de in Lights) de.Color = Color.Red;
            if (DoorsA[0].Status == DoorStatus.Open && DoorsB[0].Status == DoorStatus.Open && !Vents[0].Depressurize) {
                DoorClose(DoorsA);
                DoorClose(DoorsB);
            }
        }
            StatusBuilder.Clear();
            StatusBuilder.Append(ZoneName);
            StatusBuilder.Append($" O2 {AirlockPressure:n2}% {AirlockStatus}");

        foreach (IMyLightingBlock de in Lights) if (de.CustomName.Contains("Control") && !de.Enabled) toBreakOut = false;
        if (toBreakOut) return true;
        O2TanksFill = GetTankFill(O2Tanks);
        //get Airpressure
        if (VentsA.Count < 1 || (VentsA[0].GetOxygenLevel() * 100) < AirLimit) {
            AirSideA = false;
        } else {
            AirSideA = true;
        }
        if (VentsB.Count < 1 || (VentsB[0].GetOxygenLevel() * 100) > AirLimit) {
            AirSideB = true;
        } else {
            AirSideB = false;
        }
        //special cases
        if ((AirSideA && AirSideB && !Vents[0].Depressurize) || (!AirSideA && !AirSideB && Vents[0].Depressurize)) {
            DoorOn(DoorsA);
            DoorOn(DoorsB);
            DoorOpen(DoorsA);
            DoorOpen(DoorsB);
            AirlockStatus =" Both Doors Open";
            ControlOn(Lights);
            return true;
        }
        if ((DoorsA[0].Enabled && DoorsB[0].Enabled) || (!DoorsA[0].Enabled && !DoorsB[0].Enabled)) {
            if (DoorsB[0].Status == DoorStatus.Closed) {
                DoorOff(DoorsB);
                AirlockStatus =" Unknown state";
                return true;
            } else {
                DoorOn(DoorsA);
                DoorOn(DoorsB);
                DoorClose(DoorsA);
                DoorClose(DoorsB);
                AirlockStatus = " Unknown state";
                return true;
            }
        }

        if (!AirLockMirror(DoorsA, DoorsB, AirSideA)) { AirLockMirror(DoorsB, DoorsA, AirSideB); }
        return true;
    }

    private bool AirLockMirror(List<IMyDoor> doors1, List<IMyDoor> doors2, bool airSide1) {
        if (!doors1[0].Enabled) {
            if (!airSide1) {
                if (!Vents[0].Depressurize) {
                    foreach (IMyAirVent de in Vents) de.Depressurize = true;
                    DoorClose(doors2);
                    AirlockStatus = " Depressurizing";
                    return true;
                } else {
                    if (O2TanksFill == 100 || AirlockPressure < 1) {
                        foreach (IMyDoor de in doors2) { if (de.Status != DoorStatus.Closed) return true; }
                        DoorOff(doors2);
                        DoorOn(doors1);
                        DoorOpen(doors1);
                        AirlockStatus = " Depressurized";
                        ControlOn(Lights);
                        return true;
                    }
                }
            } else {
                if (!Vents[0].Depressurize) {
                    if (O2TanksFill == 0 || AirlockPressure > AirLimit) {
                        foreach (IMyDoor de in doors2) { if (de.Status != DoorStatus.Closed) return true; }
                        DoorOff(doors2);
                        DoorOn(doors1);
                        DoorOpen(doors1);
                        AirlockStatus = " Pressurized";
                        ControlOn(Lights);
                        return true;
                    }
                } else {
                    foreach (IMyAirVent de in Vents) de.Depressurize = false;
                    DoorClose(doors2);
                    AirlockStatus = " Pressurizing";
                    return true;
                }
            }
        }
        return false;
    }

    private void ControlOn(List<IMyInteriorLight> lights) {
        foreach (IMyLightingBlock de in Lights) if (de.CustomName.Contains("Control")) de.Enabled = true;
    }

    private void DoorClose(List<IMyDoor> doors) {
        foreach (IMyDoor de in doors) de.CloseDoor();
    }

    private void DoorOn(List<IMyDoor> doors) {
        foreach (IMyDoor de in doors) de.Enabled = true;
    }

    private void DoorOpen(List<IMyDoor> doors) {
        foreach (IMyDoor de in doors) de.OpenDoor();
    }

    private void DoorOff(List<IMyDoor> doors) {
        foreach (IMyDoor de in doors) de.Enabled = false;
    }

    double GetTankFill(List<IMyGasTank> tanks) {
        int tanksCount = 0;
        //get percent in tanks
        double tanksFill = 0;
        foreach (IMyGasTank de in tanks) {
            tanksFill += de.FilledRatio * 100;
            ++tanksCount;
            //}
        }
        if (tanksCount == 0) return -1;
        tanksFill /= tanksCount;
        return tanksFill;
    }
}

// http://forums.keenswh.com/threads/aligning-ship-to-planet-gravity.7373513/#post-1286885461

double CTRL_COEFF = 0.5;
int LIMIT_GYROS = 3; // max number of gyros to use to align craft. Leaving some available allows for player control to continue during auto-align
IMyShipController rc;
List<IMyGyro> gyros = new List<IMyGyro>();
float minAngleRad = 0.01f; // how tight to maintain horizontal Lower is tighter.
StringBuilder autoLevelStatus = new StringBuilder();
float maxTiltAngleRad = 1.5f; //how far off gravity to tilt  45 degree = 0.785 radian

string GyroMain(string argument, Vector3D tiltVector = default(Vector3D)) {
    autoLevelStatus.Clear();
    //autoLevelStatus.Append("Auto level is turned on");
    if (rc == null) { gyrosetup(); }
    if (rc == null) {
        autoLevelStatus.Append("Auto-Level:No Cockpit or Remote Control.");
        return autoLevelStatus.ToString();
    }
    Matrix or;
    rc.Orientation.GetMatrix(out or);
    Vector3D down;
    if (argument.ToLower().Contains("rocket")) { down = or.Backward; } else { down = or.Down; }
    Vector3D grav = rc.GetNaturalGravity();
    grav.Normalize();
    //need if tiltVector is not null
    if (!(tiltVector == default(Vector3D) )) {
        //same direction
        //Math.Sign(gravityVector.Dot(shipVector))
        if (Math.Sign(grav.Dot(tiltVector)) < 0) {
            // ok vector are opposite
            tiltVector.Normalize();
            var rot = Vector3D.Cross(grav, -tiltVector);
            double ang = rot.Length();
            ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));
            if(!double.IsNaN(ang) && ang < maxTiltAngleRad) {
                //ok use tilt Vector3d instead of grav
                //autoLevelStatus.Append($"tilt angle {MathHelper.ToDegrees((float)ang)}\n");
                grav = -tiltVector;
            }
        }
    }
    autoLevelStatus.Append("Auto-Level:");
    for (int i = 0; i < gyros.Count; ++i) {
        var g = gyros[i];
        g.Orientation.GetMatrix(out or);
        var localDown = Vector3D.Transform(down, MatrixD.Transpose(or));
        var localGrav = Vector3D.Transform(grav, MatrixD.Transpose(g.WorldMatrix.GetOrientation()));
        //Since the gyro ui lies, we are not trying to control yaw,pitch,roll but rather we
        //need a rotation vector (axis around which to rotate)
        var rot = Vector3D.Cross(localDown, localGrav);
        double ang = rot.Length();
        ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));
        if (double.IsNaN(ang)) { // not in gravity
            //g.GyroOverride = false;
            autoLevelStatus.Append("Auto-Level:Not in gravity well");
            foreach (IMyGyro de in gyros) { de.GyroOverride = false; }
            return autoLevelStatus.ToString() ;
        }
        if (ang < minAngleRad) { // close enough
            g.GyroOverride = false;
            autoLevelStatus.Append($" on level.");
            continue;
        }
        if (tiltVector == default(Vector3D)) {
            autoLevelStatus.Append($" Off level: {(MathHelper.ToDegrees((float)ang)):n3} deg.");
        } else {
            autoLevelStatus.Append($" Off tilt: {(MathHelper.ToDegrees((float)ang)):n3} deg.");
        }
        double ctrl_vel = g.GetMaximum<float>("Yaw") * (ang / Math.PI) * CTRL_COEFF;
        ctrl_vel = Math.Min(g.GetMaximum<float>("Yaw"), ctrl_vel);
        ctrl_vel = Math.Max(0.01, ctrl_vel);
        rot.Normalize();
        rot *= ctrl_vel;
        g.Pitch = -(float)(MathHelper.RPMToRadiansPerSecond * rot.X);
        g.Yaw = -(float)(MathHelper.RPMToRadiansPerSecond * rot.Y);
        g.Roll = -(float)(MathHelper.RPMToRadiansPerSecond * rot.Z);
        g.GyroPower = 1;
        g.GyroOverride = true;
    }
    return autoLevelStatus.ToString();
}

string gyrosetup() {
    var l = new List<IMyTerminalBlock>();
    if (rc == null) {
        GridTerminalSystem.GetBlocksOfType<IMyShipController>(l, (x => { return x.CubeGrid == Me.CubeGrid; }));
        if (l.Count < 1) return "No RC!";
        rc = (IMyShipController)l[0];
    }
    GridTerminalSystem.GetBlocksOfType<IMyGyro>(l, (x => { return x.CubeGrid == Me.CubeGrid; }));
    if (gyros.Count > 0) gyros.Clear();
    foreach (var tb in l) {
        bool add = true;
        foreach (var name in reservedNamesStatic) {
            if (tb.CustomName.Contains(name) && !(name == "")) { add = false; }
        }
        if (add) { gyros.Add((IMyGyro)tb); }
    }
    if (gyros.Count > LIMIT_GYROS) { gyros.RemoveRange(LIMIT_GYROS, gyros.Count - LIMIT_GYROS); }
    return "G" + gyros.Count.ToString("00");
}
void gyrosOff() {
    for (int i = 0; i < gyros.Count; ++i) { gyros[i].GyroOverride = false; }
}

public class Hangar {

    private Program _script;
    public string ZoneName { get; set; }
    public List<IMyInteriorLight> ControlPressurize { get; set; }
    public List<IMyInteriorLight> ControlDepressurize { get; set; }
    public List<IMyAirVent> Vents { get; set; }
    public List<IMyInteriorLight> WarningLights { get; set; }
    public List<IMySoundBlock> WarningSounds { get; set; }
    public List<IMyDoor> HangarDoors { get; set; }
    public List<IMySensorBlock> HangarDoorSensors { get; set; }
    public List<IMyAirVent> OutsideVents { get; set; }
    public List<IMyGasTank> O2Tanks { get; set; }
    public string Status {
        get { return StatusBuilder.ToString(); }
        set {
            StatusBuilder.Clear();
            StatusBuilder.Append(value);
        }
    }
    private StringBuilder StatusBuilder = new StringBuilder();
    public int AirLimit { get; set; }

    public string HangarStatus { get; private set; }
    public float HangarPressure { get; private set; } = 0;
    public bool DepressurizeFlag { get; private set; }
    public bool PressurizeFlag { get; private set; }
    public double O2TanksFill { get; private set; } = 0;
    public int AirTanksCount { get; private set; } = 0;
    private MyResourceSinkComponent sink;
    private ListReader<MyDefinitionId> list;

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyInteriorLight> warningLights, List<IMySoundBlock> warningSounds, List<IMyDoor> hangarDoors,
        List<IMySensorBlock> bayDoorSensors, List<IMyAirVent> outsideVents, List<IMyGasTank> o2Tanks, int airLimit = 50) {
        ZoneName = zoneName;
        ControlPressurize = new List<IMyInteriorLight>(controlPressurize);
        ControlDepressurize = new List<IMyInteriorLight>(controlDepressurize);
        Vents = new List<IMyAirVent>(vents);
        WarningLights = new List<IMyInteriorLight>(warningLights);
        WarningSounds = new List<IMySoundBlock>(warningSounds);
        HangarDoors = new List<IMyDoor>(hangarDoors);
        HangarDoorSensors = new List<IMySensorBlock>(bayDoorSensors);
        OutsideVents = new List<IMyAirVent>(outsideVents);
        for (int i = 0; i < o2Tanks.Count; ++i) {
            o2Tanks[i].Components.TryGet<MyResourceSinkComponent>(out sink);
            list = sink.AcceptedResources;
            bool hasO2 = false;
            foreach (var de in list) {
                if (de == OxygenGasId) hasO2 = true;
            }
            if (!hasO2) {
                o2Tanks.RemoveAtFast(i);
                --i;
            }
        }
        O2Tanks = new List<IMyGasTank>(o2Tanks);
        AirLimit = airLimit;
        Status = "";
    }

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMySensorBlock> bayDoorSensors, List<IMySoundBlock> sounds, List<IMyGasTank> o2Tanks, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, new List<IMyInteriorLight>(), sounds,
         hangarDoors, bayDoorSensors, new List<IMyAirVent>(), o2Tanks, airLimit) { }

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMySensorBlock> bayDoorSensors, List<IMyGasTank> o2Tanks, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, new List<IMyInteriorLight>(), new List<IMySoundBlock>(),
              hangarDoors, bayDoorSensors, new List<IMyAirVent>(), o2Tanks, airLimit) { }

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMyAirVent> outsideVents, List<IMyGasTank> o2Tanks, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, new List<IMyInteriorLight>(), new List<IMySoundBlock>(),
              hangarDoors, new List<IMySensorBlock>(), outsideVents, o2Tanks, airLimit) { }

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMyGasTank> o2Tanks, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, hangarDoors, new List<IMySensorBlock>(), o2Tanks, airLimit) { }

    //script
    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyInteriorLight> warningLights, List<IMySoundBlock> warningSounds, List<IMyDoor> hangarDoors,
        List<IMySensorBlock> bayDoorSensors, List<IMyAirVent> outsideVents, List<IMyGasTank> o2Tanks, Program script, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, warningLights, warningSounds,
         hangarDoors, bayDoorSensors, outsideVents, o2Tanks, airLimit) {
        _script = script;
    }
    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMySensorBlock> bayDoorSensors, List<IMySoundBlock> sounds, List<IMyGasTank> o2Tanks, Program script, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, new List<IMyInteriorLight>(), sounds,
            hangarDoors, bayDoorSensors, new List<IMyAirVent>(), o2Tanks, script, airLimit){ }

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMySensorBlock> bayDoorSensors, List<IMyGasTank> o2Tanks,Program script, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, new List<IMyInteriorLight>(), new List<IMySoundBlock>(),
              hangarDoors, bayDoorSensors, new List<IMyAirVent>(), o2Tanks, script, airLimit) { }

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMyAirVent> outsideVents, List<IMyGasTank> o2Tanks, Program script, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, new List<IMyInteriorLight>(), new List<IMySoundBlock>(),
            hangarDoors, new List<IMySensorBlock>(), outsideVents, o2Tanks, script, airLimit) { }

    public Hangar(string zoneName, List<IMyInteriorLight> controlPressurize, List<IMyInteriorLight> controlDepressurize, List<IMyAirVent> vents,
        List<IMyDoor> hangarDoors, List<IMyGasTank> o2Tanks, Program script, int airLimit = 50)
        : this(zoneName, controlPressurize, controlDepressurize, vents, hangarDoors, new List<IMySensorBlock>(), o2Tanks, script, airLimit) { }

    public bool Closed(IMyTerminalBlock block) {
        return !(_script?.GridTerminalSystem.CanAccess(block) ?? !block.Closed);
    }

    double GetTankFill(List<IMyGasTank> tanks) {
        int tanksCount = 0;
        //get percent in tanks
        double tanksFill = 0;
        foreach (IMyGasTank de in tanks) {
            tanksFill += de.FilledRatio * 100;
            ++tanksCount;
            //}
        }
        if (tanksCount == 0) return -1;
        tanksFill /= tanksCount;
        return tanksFill;
    }

    public bool Update() {
        if (Vents.Count == 0 || (ControlDepressurize.Count == 0 && ControlPressurize.Count == 0)) { return false; }
        foreach (var de in ControlPressurize) if (Closed(de)) return false;
        foreach (var de in ControlDepressurize) if (Closed(de)) return false;
        foreach (var de in Vents) if (Closed(de)) return false;
        foreach (var de in WarningLights) if (Closed(de)) return false;
        foreach (var de in WarningSounds) if (Closed(de)) return false;
        foreach (var de in HangarDoors) if (Closed(de)) return false;
        foreach (var de in HangarDoorSensors) if (Closed(de)) return false;
        foreach (var de in OutsideVents) if (Closed(de)) return false;
        foreach (var de in O2Tanks) if (Closed(de)) return false;
        PressurizeFlag = false;
        DepressurizeFlag = false;
        foreach (IMyInteriorLight de in ControlPressurize) {
            if (!de.Enabled) {
                PressurizeFlag = true;
                break;
            }
        }
        foreach (IMyInteriorLight de in ControlDepressurize) {
            if (!de.Enabled) {
                DepressurizeFlag = true;
                break;
            }
        }
        //RoomPressure handling
        HangarPressure = Vents[0].GetOxygenLevel() * 100;
        if (HangarPressure > AirLimit) {
            foreach (IMyInteriorLight de in ControlPressurize) de.Color = Color.White;
            foreach (IMyInteriorLight de in ControlDepressurize) de.Color = Color.White;
        } else {
            foreach (IMyInteriorLight de in ControlPressurize) de.Color = Color.Red;
            foreach (IMyInteriorLight de in ControlDepressurize) de.Color = Color.Red;
        }
        StatusBuilder.Clear();
        StatusBuilder.Append(ZoneName);
        StatusBuilder.Append($" O2 {HangarPressure}% {HangarStatus}");
        //hangar
        if (!DepressurizeFlag && !PressurizeFlag) return true;
        if (DepressurizeFlag && !PressurizeFlag) {
            if (Vents[0].Depressurize) {
                if (HangarPressure == 0 || GetTankFill(O2Tanks) == 100) {
                    foreach (IMySoundBlock de in WarningSounds) de.Stop();
                    foreach (IMySensorBlock de in HangarDoorSensors) de.Enabled = true;
                    HangarStatus = " Hangar doors ready to open";
                    foreach (IMyInteriorLight de in ControlDepressurize) de.Enabled = true;
                    return true;
                } else return true;
            } else if (OutsideVents.Count > 0 && OutsideVents[0].GetOxygenLevel() * 100 > AirLimit) {
                foreach (IMySensorBlock de in HangarDoorSensors) de.Enabled = true;
                foreach (IMyInteriorLight de in ControlDepressurize) de.Enabled = true;
                HangarStatus = " Hangar doors ready for Earthside";
                return true;
            } else {
                foreach (IMyDoor de in HangarDoors) de.CloseDoor();
                foreach (IMySoundBlock de in WarningSounds) de.Play();
                foreach (IMyInteriorLight de in WarningLights) de.Enabled = true;
                foreach (IMyAirVent de in Vents) de.Depressurize = true;
                HangarStatus = " Hangar bay depressurizing";
                return true;
            }
        } else if (!DepressurizeFlag && PressurizeFlag) {
            if (Vents[0].Depressurize || (OutsideVents.Count > 0 && OutsideVents[0].GetOxygenLevel() * 100 > AirLimit && Vents[0].Depressurize)) {
                foreach (IMyDoor de in HangarDoors) de.CloseDoor();
                foreach (IMySensorBlock de in HangarDoorSensors) de.Enabled = false;
                foreach (IMyAirVent de in Vents) de.Depressurize = false;
                HangarStatus = " Hangar bay pressurizing";
                return true;
            } else if (!Vents[0].Depressurize) {
                foreach (IMyDoor de in HangarDoors) de.CloseDoor();
                foreach (IMySensorBlock de in HangarDoorSensors) de.Enabled = false;
                foreach (IMyInteriorLight de in WarningLights) de.Enabled = false;
                foreach (IMyInteriorLight de in ControlPressurize) de.Enabled = true;
                HangarStatus = " Hangar bay pressurized";
                return true;
            } else return true;
        }
        foreach (IMyDoor de in HangarDoors) de.CloseDoor();
        foreach (IMySensorBlock de in HangarDoorSensors) de.Enabled = false;
        HangarStatus = " Hangar bay in unknown status, resetting";
        foreach (IMyInteriorLight de in ControlDepressurize) de.Enabled = true;
        foreach (IMyInteriorLight de in ControlPressurize) de.Enabled = true;

        return true;
    }
}

public class LandingPad {

    public string ZoneName { get; set; }
    public List<IMyInteriorLight> Lights { get; set; }
    public List<IMyLandingGear> LandingLockPads { get; set; }
    public List<IMyShipConnector> Connectors { get; set; }
    public string Status {
        get { return StatusBuilder.ToString(); }
        set {
            StatusBuilder.Clear();
            StatusBuilder.Append(value);
        }
    }
    private StringBuilder StatusBuilder = new StringBuilder();
    public bool IsConnected { get; private set; }
    public bool IsConnectable { get; private set; }
    public bool IsLocked { get; private set; }
    public string PadStatus { get; private set; }

    public LandingPad(string zoneName, List<IMyLandingGear> landingGear, List<IMyInteriorLight> lights, List<IMyShipConnector> connectors) {
        ZoneName = zoneName;
        Lights = new List<IMyInteriorLight>(lights);
        LandingLockPads = new List<IMyLandingGear>(landingGear);
        Connectors = new List<IMyShipConnector>(connectors);
        Status = "";
    }

    public LandingPad(string zoneName, List<IMyLandingGear> landingGear, List<IMyInteriorLight> lights) :
        this(zoneName, landingGear, lights, new List<IMyShipConnector>()) { }

    public LandingPad(string zoneName, List<IMyLandingGear> landingGear, List<IMyShipConnector> connectors) :
        this(zoneName, landingGear, new List<IMyInteriorLight>(), connectors) { }

    public LandingPad(string zoneName, List<IMyShipConnector> connectors, List<IMyInteriorLight> lights) :
        this(zoneName, new List<IMyLandingGear>(), lights, connectors) { }

    public bool Update() {
        //if (LandingLockPads.Count == 0 && (Lights.Count == 0 || Connectors.Count == 0)) return false;
        IsConnected = false;
        IsConnectable = false;
        IsLocked = false;
        foreach (IMyShipConnector de in Connectors) {
            if (de.Status == MyShipConnectorStatus.Connected) {
                IsConnected = true;
            }
            if (de.Status == MyShipConnectorStatus.Connectable) {
                IsConnectable = true;
            }
        }
        if (IsConnected) {
            foreach (IMyLandingGear de in LandingLockPads) {
                if (de.LockMode == LandingGearMode.ReadyToLock) {
                    de.Lock();
                }
            }
        } else {
            foreach (IMyLandingGear de in LandingLockPads) {
                de.Unlock();
            }
        }
        foreach (IMyLandingGear de in LandingLockPads) {
            if (de.IsLocked) {
                IsLocked = true;
            }
        }
        if (IsLocked) {
            foreach (IMyInteriorLight de in Lights) de.Color = Color.Green;
        } else if (IsConnectable) {
            foreach (IMyInteriorLight de in Lights) de.Color = Color.Yellow;
        } else {
            foreach (IMyInteriorLight de in Lights) de.Color = Color.White;
        }
        return true;
    }
}

public class Orbital {

    Program _script;

    //Orbital
    public OrbitalOperation OrbitalMode { get; set; } = OrbitalOperation.Off;
    public bool GravDriveOn { get; set; } = true;
    public string EmergencyThrusterName { get; set; } = "[EmergencyThrust]";
    public double GravityEmergency { get; set; } = 0.3;//Where to turn on emergency thrust
    public double GravityTarget { get; set; } = 0.20;//target orbit
    public double AltitudeTarget { get; set; } = -1000;//target sealevel
    public double HoverTarget { get; set; } = 20;//
    public double VelocityLimit { get; set; } = 90;//m/s
    public double HeightOffset { get; set; } = 100;//height adjuster for varrious ships. generally height of shipController
    public double AltitudeBuffer { get; set; } = 5;//buffer above and below Target
    public string Status {
        get { return StatusBuilder.ToString(); }
        set {
            StatusBuilder.Clear();
            StatusBuilder.Append(value);
        }
    }
    private StringBuilder StatusBuilder = new StringBuilder();
    public bool OrbitalListNeedsBuilding { get; set; } = true;

    //orbital helper
    public enum OrbitalOperation { Off, GravityMode, AltitudeMode, HoverMode }
    double normalGravity = 0;
    List<IMyThrust> thrusters = new List<IMyThrust>();
    List<IMyThrust> emergencyThrusters = new List<IMyThrust>();
    List<IMyGravityGenerator> gravDrivePos = new List<IMyGravityGenerator>();
    List<IMyGravityGenerator> gravDriveNeg = new List<IMyGravityGenerator>();
    List<IMyArtificialMassBlock> vMass = new List<IMyArtificialMassBlock>();
    List<IMyShipController> shipControllers = new List<IMyShipController>();
    List<IMyCameraBlock> downCameras = new List<IMyCameraBlock>();
    Vector3D gravityVector = new Vector3D();


    public Orbital(Program script) {
        _script = script;
        Status = "";
    }

    public Orbital(Program script, OrbitalOperation oMode) : this(script) {
        OrbitalMode = oMode;
    }
    /// <summary>
            /// Gravity Orbit default
            /// </summary>
            /// <param name="script"></param>
            /// <param name="gravityEmergency"></param>
            /// <param name="emergencyThrusterName"></param>
            /// <param name="gravityTarget"></param>
            /// <param name="velocityLimit"></param>
    public Orbital(Program script, double gravityEmergency, string emergencyThrusterName, double gravityTarget,
        double velocityLimit) : this(script) {
        OrbitalMode = OrbitalOperation.GravityMode;
        GravityEmergency = gravityEmergency;
        EmergencyThrusterName = emergencyThrusterName;
        GravityTarget = gravityTarget;
        VelocityLimit = velocityLimit;
    }

    public void BuildOrbitList() {
        thrusters.Clear();
        emergencyThrusters.Clear();
        //gravDrive.Clear();
        gravDrivePos.Clear();
        gravDriveNeg.Clear();
        vMass.Clear();
        shipControllers.Clear();
        downCameras.Clear();
        _script.GridTerminalSystem.GetBlocksOfType(shipControllers, b => b.CubeGrid == _script.Me.CubeGrid);
        if (shipControllers.Count > 0) {
            gravityVector = shipControllers[0].GetNaturalGravity();
            normalGravity = gravityVector.Length() / 9.81;
        }
        if (shipControllers.Count == 0 || normalGravity == 0) {
            normalGravity = 0;
            return;
        }
        _script.GridTerminalSystem.GetBlocksOfType<IMyThrust>(null, b => {
            if (b.CubeGrid == _script.Me.CubeGrid) {
                switch (b.WorldMatrix.GetClosestDirection(gravityVector)) {
                    case Base6Directions.Direction.Forward:
                        if (b.CustomName.Contains(EmergencyThrusterName)) { emergencyThrusters.Add(b); } else { thrusters.Add(b); }
                        break;
                }
            }
            return false;
        });
        _script.GridTerminalSystem.GetBlocksOfType(vMass, b => b.CubeGrid == _script.Me.CubeGrid);
        _script.GridTerminalSystem.GetBlocksOfType<IMyGravityGenerator>(null, b => {
            if (b.CubeGrid == _script.Me.CubeGrid) {
                foreach (IMyArtificialMassBlock de in vMass) {
                    if (IsPositionInRange(b,de.GetPosition())) {
                        switch (b.WorldMatrix.GetClosestDirection(gravityVector)) {
                            case Base6Directions.Direction.Up:
                                gravDrivePos.Add(b);
                                break;
                            case Base6Directions.Direction.Down:
                                gravDriveNeg.Add(b);
                                break;
                        }
                    }
                }
            }
            return false;
        });
        _script.GridTerminalSystem.GetBlocksOfType(downCameras, b => {
            if (b.CubeGrid == _script.Me.CubeGrid) {
                switch (b.WorldMatrix.GetClosestDirection(gravityVector)) {
                    case Base6Directions.Direction.Forward:
                        return true;
                }
            }
            return false;
        });
        OrbitalListNeedsBuilding = false;
    }

    public bool Closed(IMyTerminalBlock block) {
        return !_script.GridTerminalSystem.CanAccess(block);
    }

    public void CurrentAltitude() {
        OrbitalMode = OrbitalOperation.AltitudeMode;
        AltitudeTarget = FindAltitude();
    }

    public void CurrentGravity() {
        OrbitalMode = OrbitalOperation.GravityMode;
        GravityTarget = FindGravity();
    }

    public void CurrentHover() {
        if (shipControllers.Count < 1) return;
        OrbitalMode = OrbitalOperation.HoverMode;
        double tempDouble = 0;
        double surfaceAltitude = 0;
        double lidarAltitude = 0;
        foreach (IMyShipController de in shipControllers) {
            de.TryGetPlanetElevation(MyPlanetElevation.Surface, out tempDouble);
            if (tempDouble > surfaceAltitude) surfaceAltitude = tempDouble;
        }
        gravityVector = shipControllers[0].GetNaturalGravity();
        lidarAltitude = LidarRange(downCameras, 0, 0);
        if (lidarAltitude < surfaceAltitude) HoverTarget = lidarAltitude;
        else HoverTarget = surfaceAltitude;
    }

    string DisplayLargeNumber(float number) {
        string powerValue = " kMGTPEZY";
        float result = number;
        int ordinal = 0;
        while (ordinal < powerValue.Length && result >= 1000) {
            result /= 1000;
            ordinal++;
        }
        string resultString = Math.Round(result, 1, MidpointRounding.AwayFromZero).ToString();
        if (ordinal > 0) {
            resultString += " " + powerValue[ordinal];
        }
        return resultString;
    }

    public void GoLand(double target = -1000) {
        OrbitalMode = OrbitalOperation.AltitudeMode;
        AltitudeTarget = target;
    }

    public void GoDeepSpace(double target = 0.01) {
        OrbitalMode = OrbitalOperation.GravityMode;
        GravityTarget = target;
    }

    public void GoHover(double target = 100) {
        OrbitalMode = OrbitalOperation.HoverMode;
        HoverTarget = target;
    }

    double FindAltitude() {
        if (shipControllers.Count < 1) { return 0; }
        double seaLevelAltitude = 0;
        shipControllers[0].TryGetPlanetElevation(MyPlanetElevation.Sealevel, out seaLevelAltitude);
        return seaLevelAltitude;
    }
    double FindGravity() {
        if (shipControllers.Count < 1) { return 0; }
        normalGravity = shipControllers[0].GetNaturalGravity().Length() / 9.81;
        return normalGravity;
    }

    public bool IsPositionInRange(IMyGravityGenerator grav, Vector3D worldPoint) {
        Vector3 halfExtents =  grav.FieldSize * 0.5f;
        MyOrientedBoundingBox myOrientedBoundingBox = new MyOrientedBoundingBox(grav.WorldMatrix.Translation, halfExtents, Quaternion.CreateFromRotationMatrix(grav.WorldMatrix));
        Vector3 vector = worldPoint;
        return myOrientedBoundingBox.Contains(ref vector);
    }

    double LidarRange(List<IMyCameraBlock> cameras, float pitch, float yaw) {
        double range = 9999999;
        double range2 = 9999999;
        MyDetectedEntityInfo info;
        foreach (IMyCameraBlock de in cameras) {
            if (de.EnableRaycast) {
                info = de.Raycast(de.AvailableScanRange, pitch, yaw);
                if (info.HitPosition.HasValue) {
                    range2 = Vector3D.Distance(de.GetPosition(), info.HitPosition.Value);
                }
                if (range2 < range) range = range2;
            }
        }
        return range;
    }

    public void Rebuild() {
        foreach (IMyThrust de in thrusters) de.ThrustOverridePercentage = 0f;
        foreach (IMyThrust de in emergencyThrusters) de.ThrustOverridePercentage = 0f;
        foreach (IMyGravityGenerator de in gravDrivePos) de.GravityAcceleration = 0;
        foreach (IMyGravityGenerator de in gravDriveNeg) de.GravityAcceleration = 0;
        BuildOrbitList();
    }

    public void TurnOffOrbital() {
        foreach (IMyThrust de in thrusters) de.ThrustOverridePercentage = 0f;
        foreach (IMyThrust de in emergencyThrusters) de.ThrustOverridePercentage = 0f;
        foreach (IMyGravityGenerator de in gravDrivePos) de.GravityAcceleration = 0;
        foreach (IMyGravityGenerator de in gravDriveNeg) de.GravityAcceleration = 0;
        OrbitalMode = OrbitalOperation.Off;
    }

    public string Update() {
        float emergencyThrust = 0;
        float normalThrust = 0;
        float gravThrust = 0;
        double seaLevelAltitude = 0;
        double surfaceAltitude = 0;
        double lidarAltitude = 0;
        double landingAltitude = 0;
        double tempDouble;
        double thrustApplied = 0;
        int workingGravMassPair = 0;
        StatusBuilder.Clear();
        StatusBuilder.Append($"\n OrbitalMode:  {OrbitalMode}");
        if (OrbitalMode == OrbitalOperation.Off) return StatusBuilder.ToString();
        if (shipControllers.Any(de => Closed(de))
            || thrusters.Any(de => Closed(de))
            || emergencyThrusters.Any(de => Closed(de))
            || gravDriveNeg.Any(de => Closed(de))
            || gravDrivePos.Any(de => Closed(de))
            || vMass.Any(de => Closed(de))
            || downCameras.Any(de => Closed(de)))
            OrbitalListNeedsBuilding = true;
        if (!OrbitalListNeedsBuilding && shipControllers.Count > 0) {
            gravityVector = shipControllers[0].GetNaturalGravity();
            normalGravity = gravityVector.Length() / 9.81;
            if (thrusters.Count > 0 && thrusters[0].WorldMatrix.GetClosestDirection(gravityVector) == Base6Directions.Direction.Forward) {
                OrbitalListNeedsBuilding = true;
            }
        }
        if (shipControllers.Count == 0 || OrbitalListNeedsBuilding) {
            foreach (IMyThrust de in thrusters) de.ThrustOverridePercentage = 0;
            foreach (IMyThrust de in emergencyThrusters) de.ThrustOverridePercentage = 0;
            foreach (IMyGravityGenerator de in gravDrivePos) de.GravityAcceleration = 0;
            foreach (IMyGravityGenerator de in gravDriveNeg) de.GravityAcceleration = 0;
            BuildOrbitList();
        }
        if (shipControllers.Count == 0) {
            //cannot go on.
            StatusBuilder.Append("\nOrbital: Error no shipController");
            return StatusBuilder.ToString();
        }
        float massOfShip = shipControllers[0].CalculateShipMass().PhysicalMass;
        Vector3D shipVector = shipControllers[0].GetShipVelocities().LinearVelocity;
        Vector3D shipToGravityVector = VectorProjection(shipVector, gravityVector);
        double velocityTowardGravity = ((int)(-1 * Math.Sign(gravityVector.Dot(shipVector)))) * shipToGravityVector.Length();
        StatusBuilder.Append($"\n Mass : {DisplayLargeNumber(massOfShip)} kg");
        StatusBuilder.Append($"\n Rate of Change: {velocityTowardGravity:n2} m/s");
        shipControllers[0].TryGetPlanetElevation(MyPlanetElevation.Sealevel, out seaLevelAltitude);
        foreach (IMyShipController de in shipControllers) {
            de.TryGetPlanetElevation(MyPlanetElevation.Surface, out tempDouble);
            if (tempDouble > surfaceAltitude) surfaceAltitude = tempDouble;
        }
        //thrusters
        foreach (IMyThrust de in thrusters) {
            if (de.IsWorking) normalThrust += de.MaxEffectiveThrust;
        }
        if (normalThrust == 0) normalThrust = 1;
        foreach (IMyThrust de in emergencyThrusters) emergencyThrust += de.MaxEffectiveThrust;
        if (GravDriveOn) {
            workingGravMassPair = WorkingGravDriveCount(gravDriveNeg, vMass) + WorkingGravDriveCount(gravDrivePos, vMass);
            gravThrust = 490500 * workingGravMassPair;
        }
        float thrustNeeded = (float)(massOfShip * normalGravity * 9.81);
        StatusBuilder.Append($"\n Thrust needed : {DisplayLargeNumber(thrustNeeded)} N");
        //Decision point
        if (normalGravity == 0) {
            foreach (IMyThrust de in thrusters) de.ThrustOverridePercentage = 0;
            foreach (IMyThrust de in emergencyThrusters) {
                de.ThrustOverridePercentage = 0;
                de.Enabled = false;
            }
            foreach (IMyGravityGenerator de in gravDriveNeg) de.GravityAcceleration = 0;
            foreach (IMyGravityGenerator de in gravDrivePos) de.GravityAcceleration = 0;
            StatusBuilder.Append($"\n Not in gravity well");
            return StatusBuilder.ToString();
        }
        double effectiveGravDriveThrust = gravThrust * MathHelper.Clamp(1 - 2 * normalGravity, 0f, 1f);
        double maxEffectiveThrust = effectiveGravDriveThrust + normalThrust;
        double tempAdjustmentGravity = MathHelper.Clamp(normalGravity * 9.81, 0, VelocityLimit / 2);
        double availableThrust = Math.Max(maxEffectiveThrust - thrustNeeded, 0);
        double tempAdjustment = MathHelper.Clamp(availableThrust / massOfShip, 0, VelocityLimit / 2);
        double targetVelocity = 0;
        if (OrbitalMode == OrbitalOperation.AltitudeMode || OrbitalMode == OrbitalOperation.HoverMode) {
            foreach (IMyCameraBlock de in downCameras) de.EnableRaycast = true;
            lidarAltitude = LidarRange(downCameras, 0, 0);
            if (lidarAltitude < surfaceAltitude) landingAltitude = lidarAltitude;
            else landingAltitude = surfaceAltitude;
            if (OrbitalMode == OrbitalOperation.HoverMode) tempDouble = HoverTarget + seaLevelAltitude - landingAltitude;
            else tempDouble = AltitudeTarget;
            if (seaLevelAltitude < tempDouble - AltitudeBuffer) {
                targetVelocity = MathHelper.Clamp(Math.Sqrt(tempAdjustmentGravity * Math.Abs(tempDouble - seaLevelAltitude - velocityTowardGravity)), 0, VelocityLimit);
            } else if (seaLevelAltitude > tempDouble + AltitudeBuffer) {
                if (landingAltitude < (HeightOffset + 100 + velocityTowardGravity * velocityTowardGravity / tempAdjustment)) {
                    //landing
                    targetVelocity = -Math.Max(Math.Sqrt(Math.Abs(MathHelper.Clamp((landingAltitude - HeightOffset - 12.5), 0d, 100) * tempAdjustmentGravity)), 1d);
                } else {
                    targetVelocity = -MathHelper.Clamp(Math.Sqrt(0.7 * tempAdjustment * Math.Abs(seaLevelAltitude - tempDouble + velocityTowardGravity)), 0, VelocityLimit);
                }
            }
        }
        //could just do else here
        if (OrbitalMode == OrbitalOperation.GravityMode) {
            if (Math.Round(normalGravity, 2) > Math.Round(GravityTarget, 2)) {
                if (VelocityLimit > tempAdjustmentGravity * 10) targetVelocity = 10 * tempAdjustmentGravity;
                else targetVelocity = VelocityLimit;
            } else if (Math.Round(normalGravity, 2) < Math.Round(GravityTarget, 2)) targetVelocity = -VelocityLimit;
        }
        StatusBuilder.Append($"\n requested velocity {targetVelocity:n2} m/s");
        double requestedThrust = (targetVelocity - velocityTowardGravity) * massOfShip + thrustNeeded;
        //gravDrive
        if (GravDriveOn) {
            thrustApplied = Math.Sign(requestedThrust) * Math.Min(Math.Abs(requestedThrust), effectiveGravDriveThrust);
            requestedThrust -= Math.Min(Math.Abs(requestedThrust), effectiveGravDriveThrust);
        }
        float requestedGravity = (float)((thrustApplied / (MathHelper.Clamp(1 - 2 * normalGravity, 0f, 1f)) / 50000) / workingGravMassPair);
        if (requestedGravity < 0 || float.IsNaN(requestedGravity)) requestedGravity = 0;
        foreach (IMyGravityGenerator de in gravDrivePos) de.GravityAcceleration = requestedGravity;
        foreach (IMyGravityGenerator de in gravDriveNeg) de.GravityAcceleration = -requestedGravity;
        //Normal Drive
        if (thrusters.Count > 0 && requestedThrust > 0) {
            thrustApplied = Math.Min(requestedThrust, normalThrust);
            requestedThrust -= thrustApplied;
        }
        float normalThrustPerThruster = (float)(thrustApplied / normalThrust);
        if (normalThrustPerThruster < .001) { normalThrustPerThruster = 0.001f; }
        foreach (IMyThrust de in thrusters) de.ThrustOverridePercentage = normalThrustPerThruster;
        if (OrbitalMode == OrbitalOperation.GravityMode && normalGravity > GravityEmergency) {
            // Emergency mode
            StatusBuilder.Append("\n Emergency thrust engaged");
            foreach (IMyThrust de in emergencyThrusters) {
                de.Enabled = true;
                de.ThrustOverridePercentage = 1;
            }
        } else if (OrbitalMode == OrbitalOperation.GravityMode && normalGravity < GravityEmergency) {
            foreach (IMyThrust de in emergencyThrusters) {
                de.Enabled = false;
                de.ThrustOverridePercentage = 0;
            }
        }
        if (OrbitalMode == OrbitalOperation.AltitudeMode || OrbitalMode == OrbitalOperation.HoverMode) {
            StatusBuilder.Append($"\n Altitude = {seaLevelAltitude:n0} m.");
            if (OrbitalMode == OrbitalOperation.AltitudeMode) StatusBuilder.Append($"  Target = {AltitudeTarget:n0} m.");
            else StatusBuilder.Append($"  Hover at = {HoverTarget:n0} m.");
            StatusBuilder.Append($"\n Ground level = {surfaceAltitude:n0} m.");
            StatusBuilder.Append($"\n Lidar Altitude = {lidarAltitude:n0} m.");
        }
        if (OrbitalMode == OrbitalOperation.GravityMode) {
            StatusBuilder.Append($"\n Current Orbit = {normalGravity:n3} g.");
            StatusBuilder.Append($"\n  Target Orbit = {GravityTarget:n3} g.");
        }
        StatusBuilder.Append($"\n Lift: Normal: {DisplayLargeNumber((float)(normalThrust / (normalGravity * 9.81)))} kg.");
        StatusBuilder.Append($"\n    GravDrive: {DisplayLargeNumber((float)(effectiveGravDriveThrust / (normalGravity * 9.81)))} kg.");
        StatusBuilder.Append($"\n    Emergency: {DisplayLargeNumber((float)(emergencyThrust / (normalGravity * 9.81)))} kg.");
        return StatusBuilder.ToString();
    }

    int WorkingGravDriveCount(List<IMyGravityGenerator> gravDrive, List<IMyArtificialMassBlock> masses) {
        int tempInt = 0;
        foreach (IMyGravityGenerator de in gravDrive) {
            foreach(IMyArtificialMassBlock fr in masses) {
                if ((de.Enabled || de.IsWorking) && fr.IsWorking && IsPositionInRange(de, fr.GetPosition())) ++tempInt;
            }
        }
        return tempInt;
    }

    Vector3D VectorProjection(Vector3D a, Vector3D b) {//project a onto b
        Vector3D projection = a.Dot(b) / b.LengthSquared() * b;
        return projection;
    }
}

public class RoomPressure {

    private Program _script;
    public string ZoneName { get; set; }
    public List<IMyInteriorLight> Lights { get; set; }
    public List<IMyDoor> Doors { get; set; }
    public List<IMyAirVent> Vents { get; set; }
    public string Status {
        get { return StatusBuilder.ToString(); }
        set {
            StatusBuilder.Clear();
            StatusBuilder.Append(value);
        }
    }
    private StringBuilder StatusBuilder = new StringBuilder();
    public int AirLimit { get; set; }
    public float RoomVentPressure { get; private set; } = 0;
    public const int airLimitConst = 50;

    public RoomPressure(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, List<IMyDoor> doors, int airLimit = airLimitConst) {
        ZoneName = zoneName;
        Vents = new List<IMyAirVent>(vents);
        Lights = new List<IMyInteriorLight>(lights);
        Doors = new List<IMyDoor>(doors);
        Status = "";
        AirLimit = airLimit;
    }
    public RoomPressure(string zoneName, List<IMyAirVent> vents, int airLimit = airLimitConst)
        : this(zoneName, vents, new List<IMyInteriorLight>(), new List<IMyDoor>(), airLimit) { }

    public RoomPressure(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, int airLimit = airLimitConst)
        : this(zoneName, vents, lights, new List<IMyDoor>(), airLimit) { }

    public RoomPressure(string zoneName, List<IMyAirVent> vents, List<IMyDoor> doors, int airlimit = airLimitConst)
        : this(zoneName, vents, new List<IMyInteriorLight>(), doors, airlimit) { }

    //script
    public RoomPressure(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, List<IMyDoor> doors, Program script, int airLimit = airLimitConst)
        : this( zoneName, vents, lights, doors, airLimit) {
        _script = script;
    }
    public RoomPressure(string zoneName, List<IMyAirVent> vents, Program script, int airLimit = airLimitConst)
        : this(zoneName, vents, new List<IMyInteriorLight>(), new List<IMyDoor>(), airLimit) { }

    public RoomPressure(string zoneName, List<IMyAirVent> vents, List<IMyInteriorLight> lights, Program script, int airLimit = airLimitConst)
        : this(zoneName, vents, lights, new List<IMyDoor>(), script, airLimit) { }

    public RoomPressure(string zoneName, List<IMyAirVent> vents, List<IMyDoor> doors, Program script, int airlimit = airLimitConst)
        : this(zoneName, vents, new List<IMyInteriorLight>(), doors, script, airlimit) { }

    public bool Closed(IMyTerminalBlock block) {
        return !(_script?.GridTerminalSystem.CanAccess(block) ?? !block.Closed);
    }

    public bool Update(bool closeDoors = true) {
        if (Vents.Count == 0) return false;
        foreach (var de in Vents) if (Closed(de)) return false;
        foreach (var de in Lights) if (Closed(de)) return false;
        foreach (var de in Doors) if (Closed(de)) return false;
        StatusBuilder.Clear();
        StatusBuilder.Append(ZoneName);
        RoomVentPressure = Vents[0].GetOxygenLevel() * 100;
        StatusBuilder.Append($" O2 {RoomVentPressure:n2}%");
        if (!Vents[0].CanPressurize) StatusBuilder.Append(" Open to outside");
        if (RoomVentPressure > AirLimit) {
            foreach (IMyInteriorLight de in Lights) de.Color = Color.White;
        } else {
            foreach (IMyInteriorLight de in Lights) de.Color = Color.Red;
            if (closeDoors && !Vents[0].Depressurize) {
                foreach (IMyDoor de in Doors) {
                    de.Enabled = true;
                    de.CloseDoor();
                }
            }
        }

        return true;
    }

}

public class SolarArray {
    // Solar panels update each 100tic
    //5 seconds = 3 * 100 tic
    //5 seconds = 5 * 100 tic
    private Program _script;
    const int updateConstant = 5;//5 seconds = 3 * 100 tic
    //const Single rpmToRadian = 0.10467f;
    public IMyMotorStator Rotor { get; set; }
    public List<IMySolarPanel> Panels { get; set; }
    public List<IMyOxygenFarm> Farms { get; set; }
    public float SpeedSetting { get; set; } //RPM
    public int TimeCount { get; set; }
    public float Power { get; set; } //MW
    public float PowerOld { get; set; }
    public float Oxygen { get; set; }
    public float OxygenOld { get; set; }
    public float Direction { get; set; }

    public SolarArray(IMyMotorStator rotor, List<IMySolarPanel> panels, float speedSetting = 0.04f):
        this(rotor, panels, new List<IMyOxygenFarm>(), speedSetting ){
    }

    public SolarArray(IMyMotorStator rotor, List<IMySolarPanel> panels, Program script, float speedSetting = 0.04f) :
        this(rotor, panels, new List<IMyOxygenFarm>(),script, speedSetting) {
    }

    public SolarArray(IMyMotorStator rotor, List<IMySolarPanel> panels, List<IMyOxygenFarm> farms, Program script, float speedSetting = 0.04f) : this(rotor, panels, farms, speedSetting) {
        _script = script;
    }

    public SolarArray(IMyMotorStator rotor, List<IMySolarPanel> panels, List<IMyOxygenFarm> farms, float speedSetting = 0.04f) {
        Rotor = rotor;
        Panels = new List<IMySolarPanel>(panels);
        SpeedSetting = speedSetting;
        TimeCount = 0;
        Power = 0f;
        PowerOld = 0f;
        Direction = 1f;
        Farms = new List<IMyOxygenFarm>(farms);
        Oxygen = 0f;
        OxygenOld = 0f;
    }

    public bool Closed(IMyTerminalBlock block) {
        return !(_script?.GridTerminalSystem.CanAccess(block) ?? !block.Closed);
    }

    public bool Update() {
        if (TimeCount++ < updateConstant) return true;
        TimeCount = 0;
        if (Closed(Rotor)) return false;
        PowerOld = Power;
        Power = 0f;
        OxygenOld = Oxygen;
        Oxygen = 0f;
        foreach (IMySolarPanel v in Panels) {
            if (Closed(v)) return false;
            Power += v.MaxOutput;
        }
        foreach (IMyOxygenFarm v in Farms) {
            if (Closed(v)) return false;
            Oxygen += v.GetOutput();
        }
        float current = Power;
        float old = PowerOld;
        if (Panels.Count == 0 && Farms.Count > 0) {
            current = Oxygen;
            old = OxygenOld;
        }

        //2x3 logic grid.  Moving, not moving : Power > old, Power < old, power == old
        if (current == old) {
            //moving, power == old = Stop
            if (!(Rotor.TargetVelocityRPM == 0)) {
                Direction = (Rotor.TargetVelocityRPM < 0 ? -1 : 1);
                Rotor.TargetVelocityRPM = 0f;
            }
        } else if (!(Rotor.TargetVelocityRPM == 0) && current < old) {
            // moving, power < old = reverse direction
            Rotor.TargetVelocityRPM = - Rotor.TargetVelocityRPM;
        } else if (Rotor.TargetVelocityRPM == 0){
            // not moving, P < old, P > old = start moving
            Rotor.TargetVelocityRPM = Direction * SpeedSetting; }
        return true;
    }

}


public class WriteLCD {
    public WriteLCD(Program script) {
        _script = script;
        LcdStringBuilder = new StringBuilder();
        Lcds = new List<IMyTextSurface>();
        surfacesProviders = new List<IMyTextSurfaceProvider>();
    }
    public WriteLCD(Program script, string lcdNameIn) {
        _script = script;
        LcdStringBuilder = new StringBuilder();
        Lcds = new List<IMyTextSurface>();
        surfacesProviders = new List<IMyTextSurfaceProvider>();
        lcdName = lcdNameIn;
        LCDBuild();
    }

    Program _script;
    public StringBuilder LcdStringBuilder { get; }
    public List<IMyTextSurface> Lcds { get; }
    List<IMyTextSurfaceProvider> surfacesProviders;
    private const int MAX_NUMBER_CHARACTERS = 100000;
    string lcdName = "";

    public void CleanLCD() {
        for (int i = 0; i < Lcds.Count; ++i) {
            if (! _script.GridTerminalSystem.CanAccess((IMyTerminalBlock)Lcds[i]) ) {
                LCDBuild();
                break;
            }
        }
        foreach (IMyTextSurface de in Lcds) {
            de.WriteText("", false);
        }
    }

    public void FlushToLCD(bool append = false) {
        if (LcdStringBuilder.Length >= MAX_NUMBER_CHARACTERS) {
            LcdStringBuilder.Clear();
            LcdStringBuilder.Append(" ERROR/n Attempted to write/n too many characters");
        }
        foreach (IMyTextSurface de in Lcds) {
            de.WriteText(LcdStringBuilder, append);
        }
        LcdStringBuilder.Clear();
    }

    public void FlushToLCD(Color color, bool append = false) {
        for (int i = 0; i < Lcds.Count; ++i) {
            if (!_script.GridTerminalSystem.CanAccess((IMyTerminalBlock)Lcds[i]) ) {
                LCDBuild();
                break;
            }
        }
        foreach (IMyTextSurface de in Lcds) {
            de.FontColor = color;
        }
        FlushToLCD(append);
    }

    public void LCDBuild(string lcdNameIn) {
        lcdName = lcdNameIn;
        LCDBuild();
    }

    public void LCDBuild() {
        Lcds.Clear();
        surfacesProviders.Clear();
        _script.GridTerminalSystem.GetBlocksOfType(surfacesProviders, b =>{
            if ( ((IMyTerminalBlock)b).CubeGrid == _script.Me.CubeGrid && b is IMyTextSurface && ((IMyTerminalBlock)b).CustomName.Contains(lcdName) && ((IMyTerminalBlock)b).IsFunctional) {
                Lcds.Add((IMyTextSurface)b);
                return false;
            }
            if ( ((IMyTerminalBlock)b).CubeGrid == _script.Me.CubeGrid &&((IMyTerminalBlock)b).CustomData.Contains(lcdName) && ((IMyTerminalBlock)b).IsFunctional) return true;
            return false;
        });
        foreach (IMyTextSurfaceProvider de in surfacesProviders){
            string[] argMessages = ((IMyTerminalBlock)de).CustomData.Split('\n');
            string[] pieces;
            int tempInt = 0;
            foreach (string fr in argMessages) {
                pieces = fr.Split(' ');
                if (pieces.Count() < 2) continue;
                //at least 2 pieces below this
                if (pieces[0].Contains(lcdName)) {
                    if (int.TryParse(pieces[1], out tempInt)) {
                        if (de.SurfaceCount > tempInt && tempInt >= 0) {
                            if (de is IMyTextSurface && !Lcds.Contains((IMyTextSurface)de)) {
                                Lcds.Add((IMyTextSurface)de);
                            }
                            else {
                                if (!Lcds.Contains(de.GetSurface(tempInt))){
                                    Lcds.Add(de.GetSurface(tempInt));
                                }
                            }
                        }
                    }
                }
            }
        };
        foreach (IMyTextSurface de in Lcds) de.ContentType = ContentType.TEXT_AND_IMAGE;
    }

    public override string ToString() {
        return LcdStringBuilder.ToString();
    }

    public void WriteToLCD(string textToWrite) {
        LcdStringBuilder.Append(textToWrite);
    }
}