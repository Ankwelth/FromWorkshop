/*
 *   Lite Solar Tracking
 *   http://steamcommunity.com/sharedfiles/filedetails/?id=1097902537
 *   
 *   1.199.020 Updated to use new constructor 
 *   1.197.181 Fixed bug in DisplayLargeNumbers
 *   1.192.022 Minor updates.
 *   1.190.009 Changed from Echo to PB LCD
 *   1.188.022 set so only looks at own construction (ie not on docking connectors)
 *   1.187.100 removed mp fix
 *   1.187.087 MP fix
 *   1.186.300 minor change.
 *   1.185.014 Change to self timer, API fixes
 *   1.182.103 Start
 * 
 */

// This file contains your actual script.
//
float solarAlign = 0.04f;
List<IMyMotorStator> rotors = new List<IMyMotorStator>();
List<IMySolarPanel> panels = new List<IMySolarPanel>();
List<IMyOxygenFarm> farms = new List<IMyOxygenFarm>();
List<SolarArray> solarArrays = new List<SolarArray>();
int buildCounter = 0;
bool tenSecondBuildDone = false;
//bool oneTimeRun = true;
List<IMyTextSurface> pbText = new List<IMyTextSurface>();
StringBuilder outText = new StringBuilder();


public Program() {
    // The constructor, called only once every session and
    // always before any other method is called. Use it to
    // initialize your script.
    //
    // The constructor is optional and can be removed if not
    // needed.
    BuildSolarArrays();
    if (Me.SurfaceCount > 0) pbText.Add(Me.GetSurface(0));
    foreach (IMyTextSurface de in pbText) de.ContentType = ContentType.TEXT_AND_IMAGE;
    //temp fix for mp
    //Runtime.UpdateFrequency = UpdateFrequency.Once;
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Save() {
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means.
    //
    // This method is optional and can be removed if not
    // needed.
}

public void Main(string argument, UpdateType updateSource) {
    // The main entry point of the script, invoked every time
    // one of the programmable block's Run actions are invoked.
    //
    // The method itself is required, but the argument above
    // can be removed if not needed.
    // temp fix for MP
    /*if (oneTimeRun) {
                Runtime.UpdateFrequency = UpdateFrequency.Update100;
                oneTimeRun = false;
            }*/
    //Echo(" O.I.S. lite Solar Array Tracking");
    outText.Clear();
    outText.Append(" O.I.S. lite Solar Array Tracking");
    if (argument == "rebuild") {
        BuildSolarArrays();
        //return;
    }
    if (!((updateSource & UpdateType.Update100) != 0)) {
        return;
    }
    //in update100
    if (!tenSecondBuildDone && buildCounter++ > 3) {
        tenSecondBuildDone = true;
        BuildSolarArrays(); }

    for (int i=0; i<solarArrays.Count; ++i) {
        if (!solarArrays[i].Update()) {
            /*
                    if (solarArrays[i].Rotor == solarArrays[i].Rotor.CubeGrid.GetCubeBlock(solarArrays[i].Rotor.Position)?.FatBlock) {
                        GridTerminalSystem.GetBlocksOfType(panels, b => b.CubeGrid == solarArrays[i].Rotor.TopGrid);
                        if (panels.Count > 0) {
                            solarArrays.Add(new SolarArray(solarArrays[i].Rotor, panels, solarAlign));
                        }
                    }
                    solarArrays.RemoveAt(i);
                    --i;
                    */
            BuildSolarArrays();
            Echo("\n Rebuilding ");
            break;
        }
        //Echo($" Array {i} Power {DisplayLargeNumber((solarArrays[i].Power*1000000))}W\n         Oxygen {DisplayLargeNumber((solarArrays[i].Oxygen))}l");
        //outText.Append($"\n Array {i} Power {DisplayLargeNumber((solarArrays[i].Power * 1000000))}W\n         Oxygen {DisplayLargeNumber((solarArrays[i].Oxygen))}l");
        outText.Append($"\n Array {i}: ");
        if (solarArrays[i].Power > 0.000001) {
            outText.Append($"Power {DisplayLargeNumber((solarArrays[i].Power * 1000000))}W");
        }
        if (solarArrays[i].Power > 0.000001 & solarArrays[i].Oxygen > 0) {
            outText.Append("\n         ");
        }
        if (solarArrays[i].Oxygen > 0) {
            outText.Append($"Oxygen {DisplayLargeNumber((solarArrays[i].Oxygen))}l");
        }
    }
    foreach (IMyTextSurface de in pbText) {
        de.WriteText(outText);
    }
    return;
}

void BuildSolarArrays() {
    solarArrays.Clear();
    GridTerminalSystem.GetBlocksOfType(rotors);
    foreach(IMyMotorStator v in rotors) {
        if (!v.IsSameConstructAs(Me)) continue;
        GridTerminalSystem.GetBlocksOfType(panels, b => b.CubeGrid == v.TopGrid);
        GridTerminalSystem.GetBlocksOfType(farms, b => b.CubeGrid == v.TopGrid);
        if (panels.Count > 0 || farms.Count > 0) {
            solarArrays.Add(new SolarArray(v, panels, farms, this, solarAlign));
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
    if (ordinal > 0) {
        resultString += " " + powerValue[ordinal];
    }
    return resultString;
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