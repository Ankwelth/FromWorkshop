//Version ALTM_2.1.3

const string NAME_Controller = "[LCD]Cockpit [ALTM Ctrl]"; //Should be the Main Ship Control Station
const string TAG_HTHRUSTERS = "[ALTM HThrust]"; //Tag for Hydrogen Thrusters List
const string TAG_ATHRUSTERS = "[ALTM AThrust]"; //Tag for Atmospheric Thrusters List
double ControllerElevCalibrate = 18.0; //Controller altitude from ground (meters)
double ElevErrorThreshold = 0.5; //Altitude error range (meters) (SUGGESTED NO LESS THAN 0.5(Ship Elev + ElevError)
double StopDesSpeed = -0.5; //Stop Altitude Decrease Speed m/s

double RdrAltTrigger = 0.0; //Stop ship to reach: no less than this altitude
double PlanetGravity = 1.0; //Max G of the targeted planet for landing

string scriptUpdateFreq = "10";

//-----Do not touch from here-----

int check = 0, //Check to see if script is updating
    TickCount = 0, //Tick Count (resets every seconds)
    TickPerSec = 0; //How much ticks counted until second has changed (Tick/s) (To Get/Create m/s Values)

int DateSec = -1, //Init Date
    oldDateSec = -1; //Init oldDate

bool DateSecHasChanged = false; //Second Change Reporter

List<IMyTerminalBlock> HThrusters;
List<IMyTerminalBlock> AThrusters;

const int LAND_STATE_IDLE = 0,
            LAND_STATE_WAITINGELEV = 1,
            LAND_STATE_ELEVREACHED = 2,
            LAND_STATE_ADJELEVLO = 3,
            LAND_STATE_ADJELEVHI = 4,
            LAND_STATE_ADJELEVOK = 5,
            LAND_STATE_LANDED = 6;

int LandingStatus = LAND_STATE_IDLE;

string[] StatusName = new string[7] { "IDLE", "WAITING ELEVATION", "ELEVATION REACHED", "ADJUST ELEV LO", "ADJUST ELEV HI", "STOP ELEV ALIGNED", "LANDING COMPLETED" };

const string CMD_TIMER_UPDATE = "TIMER:",
                CMDAR_TIMER_SET1 = "1",
                CMDAR_TIMER_SET10 = "10",
                CMDAR_TIMER_SET100 = "100";

const string CMD_LANDING_TRIG = "LAND:",
                CMDAR_LANDING_START = "Start",
                CMDAR_LANDING_RESET = "Reset";

const string CMD_ALT_CHANGE = "ALT:";

const string CMD_G_CHANGE = "G:";

const double Gravity_1G = -9.81; // 665, //1G Value

double Gravity_Sum = 0, //Average G + 1G (m/s)
        ActualGravity = 0.0, //Actual Gs
        oldActualGravity = -1.0, //old Actual Gs
        DeltaGravity = 0;

float ShipThrustSet = 0.0f; //Last Thrust Change (decimal 0.0 <-> 1.0) using Set

double ShipHThrust_N = 0, //Hydrogen Thrusters Force (N): Updates at program Init
        ShipAThrust_N = 0, //Atmosferic Thrusters Force (N): Updates every Second
        ShipTotThrust_N = 0, //Total Thrusters Force (N): Updates every Second
        ForceToAccelStop_N = 0, //Force (N) to defeat Gs
        ForceToAccelStop_Perc = 0, //Ship Force (%) to defeat Gs
        ShipAccel_ms = 0, //Calculated Ship Potential Acceleration (m/s^2)
        ShipAccelG_ms = 0, //Calculated Ship Potential Acceleration including G (m/s^2)
        ShipSpeed_ms = 0, //Calculated Ship Speed (m/s) from N/Mass: Updates every Tick
        oldShipSpeed_ms = 0, //old Ship Speed (m/s): Updates every Tick
        RealShipAccel_ms = 0; //Ship Speed (m/s^2)

double ShipMass = 0,
        ShipMass_N = 0, //Ship Mass: Updates every Tick
        ShipElev = 0, //Ship Elevation: Updates every Tick
        oldShipElev = 0, //old Ship Elevation
        ShipElevSpeed = 0, //Ship Elevation Speed (m/Tick)
        ShipElevSpeed_ms = 0, //Ship Elevation Speed (m/s)
        ShipCalcElevStop = 0, //Calculated Potential Stop Elevation with 100% Ship Force
        oldShipCalcElevStop = 0, //old Calculated Potential Stop Elevation with 100% Ship Force
        ShipCalcElevStopSpeed = 0, //Stop Elevation Speed (m/Tick)
        ShipCalcElevStopSpeed_ms = 0; //Stop Elevation Speed (m/s)

double StopT = 0, //Stop Time using 100% Ship Force
        StopD = 0, //Stop Distance using 100% Ship Force
        StopDvar_Dec = 0; 

public Program()
{
    HThrusters = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(TAG_HTHRUSTERS, HThrusters);

    AThrusters = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName(TAG_ATHRUSTERS, AThrusters);

    Gravity_Sum = (PlanetGravity * Gravity_1G);

    UpdateThrustInfos();

    Echo("Script Loaded\n");

    UpdateText();

    if(scriptUpdateFreq == "1") Runtime.UpdateFrequency = UpdateFrequency.Update1;
    else if (scriptUpdateFreq == "10") Runtime.UpdateFrequency = UpdateFrequency.Update10;
    else if (scriptUpdateFreq == "100") Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource)
{
    var Controller = GridTerminalSystem.GetBlockWithName(NAME_Controller) as IMyShipController;

    checkUpdate();
    UpdateThrustInfos();

    if (argument == "")
    {
        if (LandingStatus != LAND_STATE_IDLE)
        {
            if(LandingStatus == LAND_STATE_WAITINGELEV)
            {
                if(ShipCalcElevStop < RdrAltTrigger + ElevErrorThreshold * 1.3) //30% safe altitude (1.30)
                {//If ship is within safe elevation trigger: start thrusters
                    Controller.DampenersOverride = false;
                    ToggleShipThrusters();
                    SetShipThrust_Perc(0.0f);

                    LandingStatus = LAND_STATE_ELEVREACHED;
                }
            }
            else if( LandingStatus == LAND_STATE_ELEVREACHED)
            {
                if (ShipCalcElevStop < RdrAltTrigger + ElevErrorThreshold * 1.15) //15% safe altitude (1.15)
                {//If ship is within safe elevation trigger: start landing thrust manage
                    Controller.DampenersOverride = false;
                    SetShipThrust_Perc(0.0f);
                    LandingStatus = LAND_STATE_ADJELEVOK;
                }
            }
            else if(LandingStatus == LAND_STATE_ADJELEVOK || LandingStatus == LAND_STATE_ADJELEVHI || LandingStatus == LAND_STATE_ADJELEVLO)
            {
                if(ShipStopState() == -1)
                {//If StopElev is lower than programmed: Dampeners (100% thrust) (Cannot permit to crash into the ground)
                    Controller.DampenersOverride = false;
                    SetShipThrust_Perc(100.0f);

                    LandingStatus = LAND_STATE_ADJELEVHI;
                }
                else if(ShipStopState() == 0)
                {//If ship is in elev range as programmed: 
                    Controller.DampenersOverride = true;
                    SetShipThrust_Perc(0.0f);

                    //Controller.DampenersOverride = false;
                    //SetShipThrust_Perc(100.0f);

                    LandingStatus = LAND_STATE_ADJELEVOK;
                }
                else if(ShipStopState() == 1)
                {//If ship is higher than programmed: Set thrust to match desired StopElevSpeed
                    //reduce thrust based on ShipCalcElevStop distance (+ShipCalcElevStop distance == -Thrust * ShipCalcElevStop distance)
                    
                    if (ShipElevSpeed + (RealShipAccel_ms * 2) >= 0)
                    {
                        AddShipThrust_Perc(-50.0f);
                    }
                    else if(ShipThrustSet == 100.0f)
                    {
                        SetShipThrust_Perc(Convert.ToSingle(GetShipForce_Perc(GetShipForceVsAccel_N(Gravity_1G * (ActualGravity + DeltaGravity)))));
                    }
                    if (ShipCalcElevStop - RdrAltTrigger + ElevErrorThreshold < 30)
                    {//Fixed CalcElevStopSpeed
                        if (ShipCalcElevStopSpeed < StopDesSpeed)
                        {//If ElevStopSpeed too hi
                            Controller.DampenersOverride = false;
                            AddShipThrust_Perc(+0.02f);

                            Echo("under 500\nElevStopSpeed too hi");
                        }
                        else if (ShipCalcElevStopSpeed > StopDesSpeed)
                        {//If ElevStopSpeed too lo
                            Controller.DampenersOverride = false;
                            AddShipThrust_Perc(-0.02f);

                            Echo("under 500\nElevStopSpeed too lo");
                        }
                        else
                        {
                            Echo("under 500\nElevStopSpeed on spd");
                        }
                    }
                    else
                    {//Progressive CalcElevStopSpeed
                        //if Calculated Stop Elev Speed is LESS than ... More distance from goal multiplied to DesiredStopSpeed
                        double DesSpeed = ((ShipElev - ShipCalcElevStop - RdrAltTrigger + ElevErrorThreshold) / 100) * StopDesSpeed;
                        Echo("Desired Speed: " + DesSpeed);
                        if (ShipCalcElevStopSpeed < ((ShipElev - ShipCalcElevStop - RdrAltTrigger + ElevErrorThreshold) / 100) * StopDesSpeed)
                        {//If ElevStopSpeed too hi
                            Controller.DampenersOverride = false;
                            AddShipThrust_Perc(+0.02f);

                            Echo("ElevStopSpeed too hi");
                        }
                        else if (ShipCalcElevStopSpeed > ((ShipElev - ShipCalcElevStop - RdrAltTrigger + ElevErrorThreshold) / 100) * StopDesSpeed)
                        {//If ElevStopSpeed too lo
                            Controller.DampenersOverride = false;
                            AddShipThrust_Perc(-0.02f);

                            Echo("ElevStopSpeed too lo");
                        }
                        else
                        {
                            Echo("ElevStopSpeed on spd");
                        }
                    }

                    LandingStatus = LAND_STATE_ADJELEVLO;
                }
                if (StopT >= 0.0 && ShipElevSpeed >= 0 && ShipElev >= RdrAltTrigger && ShipElev <= RdrAltTrigger + ElevErrorThreshold)
                {
                    Controller.DampenersOverride = true;
                    SetShipThrust_Perc(0.0f);

                    LandingStatus = LAND_STATE_LANDED;
                }
            }
            else if (LandingStatus == LAND_STATE_LANDED)
            {
                LandingStatus = LAND_STATE_IDLE;
            }
        }
    }
    else
    {
        if (argument.StartsWith(CMD_LANDING_TRIG) || argument.StartsWith(CMD_ALT_CHANGE) || argument.StartsWith(CMD_G_CHANGE))
        {
            string[] splitArgument = argument.Split('|');

            for (int splitid = 0; splitid < splitArgument.Count(); splitid++)
            {
                if (splitArgument[splitid].StartsWith(CMD_LANDING_TRIG))
                {
                    string[] splitCmd = splitArgument[splitid].Split(':');
                    if (splitCmd[1] == CMDAR_LANDING_START)
                    {
                        if(LandingStatus == LAND_STATE_IDLE)
                        {
                            ToggleShipThrusters(false);

                            SetShipThrust_Perc(0.0f);

                            LandingStatus = LAND_STATE_WAITINGELEV;
                        }
                    }
                    else if (splitCmd[1] == CMDAR_LANDING_RESET)
                    {
                        if (LandingStatus != LAND_STATE_IDLE)
                        {
                            ToggleShipThrusters(true);

                            SetShipThrust_Perc(0.0f);

                            LandingStatus = LAND_STATE_IDLE;
                        }
                    }
                }

                else if (splitArgument[splitid].StartsWith(CMD_ALT_CHANGE))
                {
                    Echo("Altitude Command Running");
                    string[] splitCmd = splitArgument[splitid].Split(':');
                    if (IsNumeric(splitCmd[1]))
                    {
                        RdrAltTrigger = Convert.ToDouble(splitCmd[1]);
                        Echo("Altitude Changed");
                    }
                }

                else if (splitArgument[splitid].StartsWith(CMD_G_CHANGE))
                {
                    Echo("G Command Running");
                    string[] splitCmd = splitArgument[splitid].Split(':');
                    if (IsNumeric(splitCmd[1]))
                    {
                        PlanetGravity = Convert.ToDouble(splitCmd[1]);
                        Gravity_Sum = (PlanetGravity * Gravity_1G);
                        Echo("G Changed");
                    }
                }

                else if (splitArgument[splitid].StartsWith(CMD_G_CHANGE))
                {
                    Echo("Update Frequency Command Running");
                    string[] splitCmd = splitArgument[splitid].Split(':');
                    if (splitCmd[1] == CMDAR_TIMER_SET1)
                    {
                        if (splitArgument[splitid].StartsWith(CMD_G_CHANGE))
                        {
                            Echo("G Command Running");
                            if (splitCmd[1] == "1")
                            {
                                Runtime.UpdateFrequency = UpdateFrequency.Update1;
                                scriptUpdateFreq = splitCmd[1];
                            }
                            else if (splitCmd[1] == "10")
                            {
                                Runtime.UpdateFrequency = UpdateFrequency.Update10;
                                scriptUpdateFreq = splitCmd[1];
                            }
                            else if (splitCmd[1] == "100") 
                            {
                                Runtime.UpdateFrequency = UpdateFrequency.Update100;
                                scriptUpdateFreq = splitCmd[1];
                            }
                            else Echo("Values are: 1, 10, 100. Use EX: " + CMD_TIMER_UPDATE + CMDAR_TIMER_SET10);
                        }
                    }
                }
            }
        }
    }

    UpdateText();

    SaveOldValues(); //last Step of Main: replace with new (now old values)
}

public void SaveOldValues()
{
    oldDateSec = DateSec;
    oldShipElev = ShipElev;
    oldShipCalcElevStop = ShipCalcElevStop;
    oldShipSpeed_ms = ShipSpeed_ms;
    if (ActualGravity > -1) oldActualGravity = ActualGravity;
    else oldActualGravity = -1.0;
}

public void UpdateThrustInfos()
{
    TickCount++;

    string DateTime = Convert.ToString(System.DateTime.UtcNow);
    string[] splitDateTime = DateTime.Split(':');
    DateSec = Int16.Parse(splitDateTime[2]);
    if (DateSec != oldDateSec)
    {
        DateSecHasChanged = true;
    }
    else if (DateSecHasChanged) DateSecHasChanged = false;

    if (DateSecHasChanged) OnDateSecChange();

    //-------------------------------------------------------------------------

    var Controller = GridTerminalSystem.GetBlockWithName(NAME_Controller) as IMyShipController;
    var ShipMassVar = Controller.CalculateShipMass();
    ShipMass = ShipMassVar.PhysicalMass;
    if (Controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out ShipElev)) ShipElev -= ControllerElevCalibrate;

    ShipElevSpeed = ShipElev - oldShipElev; //Elevation Speed (m/Tick)
    ShipElevSpeed_ms = GetPerSecond(ShipElevSpeed); //from <value>/Tick to <value>/s

    for (int HThrustersNum = 0; HThrustersNum < HThrusters.Count; HThrustersNum++)
    {
        var HThruster = HThrusters[HThrustersNum] as IMyThrust;
        ShipHThrust_N = HThruster.MaxEffectiveThrust * HThrusters.Count;
    }

    for (int AThrustersNum = 0; AThrustersNum < AThrusters.Count; AThrustersNum++)
    {
        var AThruster = AThrusters[AThrustersNum] as IMyThrust;
        ShipAThrust_N = AThruster.MaxEffectiveThrust * AThrusters.Count;
    }

    ShipTotThrust_N = ShipHThrust_N + ShipAThrust_N; //Tot thrust (N) of the Ship --- ShipHThrust_N + ShipAThrust_N

    ActualGravity = -Convert.ToDouble(Controller.GetNaturalGravity().Length() / Gravity_1G);

    DeltaGravity = ActualGravity - oldActualGravity;

    ShipSpeed_ms = Controller.GetShipSpeed();
    ShipAccel_ms = ShipTotThrust_N / ShipMass;
    ShipAccelG_ms = ShipAccel_ms + (PlanetGravity * Gravity_1G); //ShipAccel_ms + (PlanetGravity * Gravity_1G);

    ShipMass_N = ConvertMass(ShipMass);

    if (ShipElevSpeed <= 0) StopT = (ShipElevSpeed_ms / (-ShipAccelG_ms));
    else StopT = -(ShipElevSpeed_ms / (-ShipAccelG_ms));

    StopD = ShipAccelG_ms * (StopT * StopT) / 2;
    ShipCalcElevStop = ShipElev - StopD;

    RealShipAccel_ms = ShipSpeed_ms - oldShipSpeed_ms;

    if (ShipElev != -1)
    {
        ShipElevSpeed = ShipElev - oldShipElev;
        ShipElevSpeed_ms = GetPerSecond(ShipElevSpeed);

        ShipCalcElevStopSpeed = ShipCalcElevStop - oldShipCalcElevStop;
        ShipCalcElevStopSpeed_ms = GetPerSecond(ShipCalcElevStopSpeed);
    }
    else
    {
        ShipElevSpeed = 0;
        ShipElevSpeed_ms = 0;

        ShipCalcElevStopSpeed = 0;
        ShipCalcElevStopSpeed_ms = 0;
    }

    ForceToAccelStop_N = GetShipForceVsAccel_N();
    ForceToAccelStop_Perc = GetShipForce_Perc(ForceToAccelStop_N, true);

    //1000 / 100000 = 0.001
    StopDvar_Dec = 1.0 - ((RdrAltTrigger + ElevErrorThreshold) / StopD);
}

public void OnDateSecChange()
{
    TickPerSec = TickCount;
    TickCount = 1;
}

public void UpdateText()
{
    Echo("\nState: " + StatusName[LandingStatus] + "\nDateTime: " + DateSec + " sec (" + TickPerSec + " Tick / s)\nTime changed: " + DateSecHasChanged);
    Echo("\nHidroThrust: " + StrToDec((ShipHThrust_N / 1000)) + " kN\nAtmoThrust: " + StrToDec((ShipAThrust_N / 1000)) + " kN\nShip Thrust: " + StrToDec((ShipTotThrust_N / 1000)) + " kN");
    Echo("\nGravity Set: " + PlanetGravity + " G (" + Gravity_Sum + " m/s)\nAct G: " + StrToDec(ActualGravity) + " G (" + StrToDec(ActualGravity * Gravity_1G, 5) + " m / s)\nDelta G: " + StrToDec(DeltaGravity, 5) + " G/Tick\nDelta G: " + StrToDec(GetPerSecond(DeltaGravity), 5) + " G/s");
    Echo("Altitude Trigger: " + RdrAltTrigger + " m");
    Echo("\nThrust: " + ShipTotThrust_N + " N\nMass: " + ShipMass + " kg\n\nSpeed: " + StrToDec(ShipSpeed_ms) + " m/s\nAccel: " + StrToDec(ShipAccel_ms, 5) + " m/s\nAccel with G: " + StrToDec(ShipAccelG_ms, 5) + "m/s\nReal Accel: " + StrToDec(RealShipAccel_ms, 5));
    Echo("\nStop Time: " + StrToDec(StopT, 1) + " sec\nStop Distance: " + StrToDec(StopD, 3) + " m\nShip Elevation: " + StrToDec(ShipElev, 3) + " m\nElev Speed: " + StrToDec(ShipElevSpeed, 3) + " m/Tick\nElev Speed: " + StrToDec(ShipElevSpeed_ms, 3) + " m/s");
    //Echo("\nShipElev - oldShipElev = ShipElevSpeed\n" + StrToDec(ShipElev, 0) + " - " + StrToDec(oldShipElev, 0) + " = " + StrToDec(ShipElevSpeed));
    Echo("\nStop Altitude: " + StrToDec(ShipCalcElevStop, 3) + " m\nStop Alt Speed: " + StrToDec(ShipCalcElevStopSpeed_ms, 3) + " m/s\nStop Alt Speed: " + StrToDec(ShipCalcElevStopSpeed) + " m/Tick");
    Echo("\nThrust Need: " + StrToDec(ForceToAccelStop_N, 3) + " kN\nThrust Need: " + StrToDec(ForceToAccelStop_Perc, 1) + " %");
    Echo("\nThrust Need 10: " + StrToDec(GetShipForce_Perc(GetShipForceVsAccel_N(10.0)), 1) + " %\nThrust Need 6: " + StrToDec(GetShipForce_Perc(GetShipForceVsAccel_N(6.0)), 1) + " %");
    Echo("\nThust Mod: " + StrToDec(ShipThrustSet * 100, 5) + " %\nDistanceVar: " + StrToDec(StopDvar_Dec, 6));
}

public void checkUpdate()
{
    if (check == 0)
    {
        Echo("- Refresh -");
        check++;
    }
    else if (check == 1)
    {
        Echo("\\ Refresh \\");
        check++;
    }
    else if (check == 2)
    {
        Echo("| Refresh |");
        check++;
    }
    else if (check == 3)
    {
        Echo("/ Refresh /");
        check = 0;
    }
}

public double GetPerSecond(double value_Tick)
{
    return value_Tick * TickPerSec;
}

public string StrToDec(double value, int decimals = 2)
{
    string strDec = "{0:0";
    for (int deciNum = 1; deciNum <= decimals; deciNum++)
    {

        if (deciNum == 1) strDec += ".0";
        else if (deciNum > 1) strDec += "0";
    }
    strDec += "}";
    return strDec = String.Format(strDec, value);
}

public bool IsNumeric(string str)
{
    if (!str.Any(c => c < '0' || c > '9'))
    {
        return true;
    }
    return false;
}

public double GetShipForceVsAccel_N(double ms2_accel = Gravity_1G)
{
    return ms2_accel * ShipMass;
}

public double GetShipForce_Perc(double Force, bool outDecimal = false)
{
    double Perc = ShipTotThrust_N / Force;
    if (!outDecimal) Perc *= 100;
    return Perc;
}

public void SetShipThrust_Perc(float ThrustPerc, bool isDecimal = false)
{
    if (!isDecimal) ShipThrustSet = ThrustPerc / 100;

    for (int HThrustersNum = 0; HThrustersNum < HThrusters.Count; HThrustersNum++)
    {
        var HThruster = HThrusters[HThrustersNum] as IMyThrust;
        HThruster.ThrustOverridePercentage = ShipThrustSet;
    }
    for (int AThrustersNum = 0; AThrustersNum < AThrusters.Count; AThrustersNum++)
    {
        var AThruster = AThrusters[AThrustersNum] as IMyThrust;
        AThruster.ThrustOverridePercentage = ShipThrustSet;
    }
}

public void AddShipThrust_Perc(float ThrustPerc)
{

    for (int HThrustersNum = 0; HThrustersNum < HThrusters.Count; HThrustersNum++)
    {
        var HThruster = HThrusters[HThrustersNum] as IMyThrust;
        HThruster.ThrustOverridePercentage += ThrustPerc;

        ShipThrustSet = HThruster.ThrustOverridePercentage + (ThrustPerc / 100);
    }
    for (int AThrustersNum = 0; AThrustersNum < AThrusters.Count; AThrustersNum++)
    {
        var AThruster = AThrusters[AThrustersNum] as IMyThrust;
        AThruster.ThrustOverridePercentage += ThrustPerc;

        ShipThrustSet = AThruster.ThrustOverridePercentage + (ThrustPerc / 100);
    }
}

public void ToggleShipThrusters(bool toggle = true)
{
    for (int HThrustersNum = 0; HThrustersNum < HThrusters.Count; HThrustersNum++)
    {
        var HThruster = HThrusters[HThrustersNum] as IMyThrust;
        HThruster.Enabled = toggle;
    }
    for (int AThrustersNum = 0; AThrustersNum < AThrusters.Count; AThrustersNum++)
    {
        var AThruster = AThrusters[AThrustersNum] as IMyThrust;
        AThruster.Enabled = toggle;
    }
}

public double ConvertMass(double Mass, bool Mass_kg = true)
{
    double outMass = -1.0;
    if (Mass_kg) outMass = Mass * ActualGravity;
    else outMass = Mass / ActualGravity;
    return outMass;
}

public int ShipStopState()
{
    //-1 == Ship Lower | 0 == Ship Ok | 1 == Ship Higher
    if (ShipCalcElevStop < RdrAltTrigger || ShipCalcElevStop <= RdrAltTrigger - ShipElevSpeed + RealShipAccel_ms) return -1;
    else if (ShipCalcElevStop >= RdrAltTrigger && ShipCalcElevStop <= RdrAltTrigger + ElevErrorThreshold) return 0;
    else if (ShipCalcElevStop > RdrAltTrigger + ElevErrorThreshold) return 1;

    return -1;
}