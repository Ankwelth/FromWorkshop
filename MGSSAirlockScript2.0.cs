//==========================START CUSTOMIZATION==========================

//Delay for the inner doors to close (won't start timer unless door is fully open)
bool doorAutoClose = true;
int doorDelay = 10;

//Minimum tank fill. If a tank gets below the value it will start to auto-fill,
    //either through the airlock vent or main conveyor system
double tankFillLevel = 10.0;
double tankFillBuffer = 2.5;
double fullPressure = 95.0;

//Delay for the master LCD ([AIRLOCK]) screen cycling if number of airlocks are greater than 6
int screenDelay = 50;

//This sets the corner LCD background to black instead of the Keen default blue
    //If set to false, slider adjustments can be made in the terminal but any new blocks
    //will default to the Keen blue
bool hardcodeLCDForeground = true;
Color LCDForegroundColor = new Color(200,200,200);
bool hardcodeLCDBackground = true;
Color LCDBackgroundColor = new Color(0,0,0);

//Colors (Red, Green, Blue) for high pressure, low pressure, and no pressure on LCD panels
Color highPressureColor = new Color(0,32,0); //green
Color lowPressureColor = new Color(127,63,0); //orange
Color noPressureColor = new Color(100,0,0); //red

//Colors for the status lights
//Pressurized
Color lightsPressurizedColor = new Color(255, 255, 255);
float lightsPressurizedBlinkInterval = 0.0f;
float lightsPressurizedBlinkLength = 0.0f;
//Depressurized
Color lightsDepressurizedColor = new Color(0, 0, 127);
float lightsDepressurizedBlinkInterval = 0.0f;
float lightsDepressurizedBlinkLength = 0.0f;
//Cycling
Color lightsCyclingColor = new Color(255, 127, 0);
float lightsCyclingBlinkInterval = 1.0f;
float lightsCyclingBlinkLength = 50.0f;
//Error
Color lightsErrorColor = new Color(255, 0, 0);
float lightsErrorBlinkInterval = 1.0f;
float lightsErrorBlinkLength = 50.0f;
//Lockdown      
Color lightsLockdownColor = new Color(255, 0, 0);
float lightsLockdownBlinkInterval = 2.0f;
float lightsLockdownBlinkLength = 50.0f;

//rotating light speed (because the default is insane
bool controlRotation = true;
float rotationSpeed = 0.02f;

//Sound block settings
bool controlSelectedSound = true;
string cyclingSound = "ArcSoundBlockAlert1";

//Keyword tags; only change naming in quotes
string airlockTag = "AIRLOCK";
string airlockTagOB = "[";
string airlockTagCB = "]";

string controlLightTag = "Control";

string airlockVentTag = "Airlock";
string innerVentTag = "Inner";
string outerVentTag = "Outer";

string innerDoorTag = "Inner";
string outerDoorTag = "Outer";

string lockdownTag = "Lockdown";

//By default, any generators & O2 farms will turn off temporarily when the main tank "[AIRLOCK]" is cycled
    //Will only work if a main tank is on the grid. Airlock specific tanks [AIRLOCK 1] will not turn off gens/farms
    //Turning these to false will remove all control and will likely overfill your tanks
bool enableGeneratorControl = true;
bool enableO2FarmControl = true;

//Update Frequency: If you are getting low sim speed (especially on servers) change to Update100
    //Program will respond slower but will potentially save some processing
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

//==========================END CUSTOMIZATION==========================

string programName = "MGSS Airlock Script";
string programVersion = "V2.5";

public const string startString = "<<MGSS_AL>>";
public const string endString = "<</MGSS_AL>>";

List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
List<string> IDList = new List<string>();
List<List<IMyTerminalBlock>> aControls = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> aVents = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> iVents = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> oVents = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> iDoors = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> oDoors = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> sLights = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> cLCDs = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> sLCDs = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> aTanks = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> rLights = new List<List<IMyTerminalBlock>>();
List<List<IMyTerminalBlock>> aSounds = new List<List<IMyTerminalBlock>>();
List<IMyTerminalBlock> m_iVents = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> m_oVents = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> m_Tanks = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> m_LCDs = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> m_Panels = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> m_LockLights = new List<IMyTerminalBlock>();
List<bool> airlockIsComplete = new List<bool>();
List<bool> pressurizeModeEnabled = new List<bool>();
List<bool> aVentCanPres = new List<bool>();
List<bool> aVentOff = new List<bool>();
List<bool> aVentCycling = new List<bool>();
List<bool> iVentCanPres = new List<bool>();
List<bool> iVentIsMonitored = new List<bool>();
List<bool> oVentIsMonitored = new List<bool>();
List<bool> oVentCanPres = new List<bool>();
List<bool> oVentPresFull = new List<bool>();
List<bool> iDoorsFullClosed = new List<bool>();
List<bool> iDoorsFullOpen = new List<bool>();
List<bool> oDoorsFullClosed = new List<bool>();
List<bool> oDoorsFullOpen = new List<bool>();
List<bool> aTanksAreEmpty = new List<bool>();
List<bool> aTanksAreFull = new List<bool>();
List<double> aVentLevel = new List<double>();
List<double> iVentLevel = new List<double>();
List<double> oVentLevel = new List<double>();
List<double> aTankLevel = new List<double>();
List<string> aWarnings = new List<string>();
List<string> aStatusList = new List<string>();
bool m_iVentIsMonitored;
bool m_iVentCanPres;
double m_iVentLevel;
bool m_oVentIsMonitored;
bool m_oVentCanPres;
bool m_oVentPresFull;
double m_oVentLevel;
bool mTankisMonitored;
bool mTanksAreEmpty;
bool mTanksAreFull;
double mTankLevel;
bool lockdownEnabled;
bool anyDoorOpen;
int doorCounter;

public void Main()
{
    allBlocks.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(allBlocks, b => b.CubeGrid == Me.CubeGrid);   
    GetAirlockIDs();
    if(IDList.Count != 0){
        GroupRename();
        GetAirlockCompLists(); 
        GetMasterCompLists();
        CheckMasterStatus();
        CheckAirlockStatus();
        AirlockCycle();
        AuxilaryControls();
        LCDControl();
    }
    EchoDiagnostics();
}

void GetAirlockIDs(){
    List<IMyTerminalBlock> lights = new List<IMyTerminalBlock>();
    lights.Clear();
    foreach (var block in allBlocks){
        if (block.CustomName.Contains(airlockTagOB+airlockTag) && block.CustomName.Contains(airlockTagCB) && block.CustomName.Contains(controlLightTag) && block is IMyInteriorLight){
            lights.Add(block);
        }
    }
    List<string> duplicateIDList = new List<string>();
    duplicateIDList.Clear();
    IDList.Clear();
    if (lights.Count != 0){
        foreach (var light in lights){
            string clName = light.CustomName;
            int idStart = clName.IndexOf("[");
            int idLength = clName.LastIndexOf("]")+ 1 - idStart;
            if (light.CustomName.Contains(airlockTagOB+airlockTag+airlockTagCB)){}
            else if (idLength > 0){
                string airlockName = clName.Substring(idStart, idLength);
                duplicateIDList.Add(airlockName);
            }
        }
    }
    IDList = duplicateIDList.Distinct().ToList();
}

void GroupRename(){
	var blockGroups = new List<IMyBlockGroup>();
	GridTerminalSystem.GetBlockGroups(blockGroups);
	if (blockGroups.Count != 0){
		for (int i = 0; i < blockGroups.Count; i++){
			IMyBlockGroup group = blockGroups[i];
			if (group.Name.Contains(airlockTagOB+airlockTag) && group.Name.Contains(airlockTagCB)){
                string groupName = group.Name;
                string componentName = groupName.Substring(0, groupName.IndexOf(airlockTagOB) - 2);
                string airlockTag = groupName.Substring(groupName.IndexOf(airlockTagOB));
                var groupComps = new List<IMyTerminalBlock>();
                group.GetBlocks(groupComps);
                for (int j = 0; j < groupComps.Count; j++){
                    string componentID = (j + 1).ToString();
                    if (j < 9){
                        componentID = "0"+componentID;
                    }
                    groupComps[j].CustomName= componentName + " " + componentID + " " + airlockTag;
                }
                groupComps.Clear();
            }
		}
	}
	blockGroups.Clear();
}

void GetAirlockCompLists(){
    aControls.Clear();
    sLights.Clear();
    aVents.Clear();
    iVents.Clear();
    oVents.Clear();
    iDoors.Clear();
    oDoors.Clear();
    cLCDs.Clear();
    sLCDs.Clear();
    aTanks.Clear();
    rLights.Clear();
    aSounds.Clear();
    foreach (var ID in IDList){
        List<IMyTerminalBlock> aComps = new List<IMyTerminalBlock>();
        aComps.Clear();
        foreach (var block in allBlocks){
            if (block.CustomName.Contains(ID)){
                aComps.Add(block);
            }
        }
        List<IMyTerminalBlock> sub_aControls = new List<IMyTerminalBlock>();
        sub_aControls.Clear();
        List<IMyTerminalBlock> sub_sLights = new List<IMyTerminalBlock>();
        sub_sLights.Clear();
        List<IMyTerminalBlock> sub_aVents = new List<IMyTerminalBlock>();
        sub_aVents.Clear();
        List<IMyTerminalBlock> sub_iVents = new List<IMyTerminalBlock>();
        sub_iVents.Clear();
        List<IMyTerminalBlock> sub_oVents = new List<IMyTerminalBlock>();
        sub_oVents.Clear();
        List<IMyTerminalBlock> sub_iDoors = new List<IMyTerminalBlock>();
        sub_iDoors.Clear();
        List<IMyTerminalBlock> sub_oDoors = new List<IMyTerminalBlock>();
        sub_oDoors.Clear();
        List<IMyTerminalBlock> sub_cLCDs = new List<IMyTerminalBlock>();
        sub_cLCDs.Clear();
        List<IMyTerminalBlock> sub_sLCDs = new List<IMyTerminalBlock>();
        sub_sLCDs.Clear();
        List<IMyTerminalBlock> sub_aTanks = new List<IMyTerminalBlock>();
        sub_aTanks.Clear();
        List<IMyTerminalBlock> sub_rLights = new List<IMyTerminalBlock>();
        sub_rLights.Clear();
        List<IMyTerminalBlock> sub_aSounds = new List<IMyTerminalBlock>();
        sub_aSounds.Clear();
        foreach(var comp in aComps){
            if(comp is IMyInteriorLight){
                if (comp.CustomName.Contains(controlLightTag)){
                    sub_aControls.Add(comp);
                }
                else{
                    sub_sLights.Add(comp);
                }

            }            
            if(comp is IMyAirVent){
                if (comp.CustomName.Contains(airlockVentTag)){
                    sub_aVents.Add(comp);
                }
                else if (comp.CustomName.Contains(innerVentTag)){
                    sub_iVents.Add(comp);
                }
                else if (comp.CustomName.Contains(outerVentTag)){
                    sub_oVents.Add(comp);
                }
            }
            if(comp is IMyDoor){
                if (comp.CustomName.Contains(innerDoorTag)){
                    sub_iDoors.Add(comp);
                }
                else if (comp.CustomName.Contains(outerDoorTag)){
                    sub_oDoors.Add(comp);
                }
            }
            if(comp is IMyTextPanel){
                if(comp.BlockDefinition.SubtypeName.Contains("Corner")){
                    sub_cLCDs.Add(comp);
                }
                else{
                    sub_sLCDs.Add(comp);
                }
            }
            if(comp is IMyGasTank){
                sub_aTanks.Add(comp);
            }
            if(comp is IMyReflectorLight){
                sub_rLights.Add(comp);
            }
            if(comp is IMySoundBlock){
                sub_aSounds.Add(comp);
            }
        }
        aControls.Add(sub_aControls);
        sLights.Add(sub_sLights);
        aVents.Add(sub_aVents);
        iVents.Add(sub_iVents);
        oVents.Add(sub_oVents);
        iDoors.Add(sub_iDoors);
        oDoors.Add(sub_oDoors);
        cLCDs.Add(sub_cLCDs);
        sLCDs.Add(sub_sLCDs);
        aTanks.Add(sub_aTanks);
        rLights.Add(sub_rLights);
        aSounds.Add(sub_aSounds);
    }
}

void GetMasterCompLists(){
    List<IMyTerminalBlock> mComps = new List<IMyTerminalBlock>();
    mComps.Clear();
    foreach (var block in allBlocks){
        if (block.CustomName.Contains(airlockTagOB+airlockTag+airlockTagCB)){
            mComps.Add(block);
        }
    }
    m_iVents.Clear();
    m_oVents.Clear();
    m_Tanks.Clear();
    m_LCDs.Clear();
    m_Panels.Clear();
    m_LockLights.Clear();
    foreach (var comp in mComps){
        if (comp is IMyAirVent){
            if (comp.CustomName.Contains(innerVentTag)){
                m_iVents.Add(comp);
            }
            else if (comp.CustomName.Contains(outerVentTag)){
                m_oVents.Add(comp);
            }
        }
        if (comp is IMyGasTank){
            m_Tanks.Add(comp);
        }
        if (comp is IMyTextPanel){
            if (comp.BlockDefinition.SubtypeName.Contains("Text")){
                m_Panels.Add(comp);
            }
            else{
                m_LCDs.Add(comp);
            }
        }
        if (comp is IMyInteriorLight && comp.CustomName.Contains(lockdownTag)){
            m_LockLights.Add(comp);
        }
    }
}

void CheckMasterStatus(){
    m_iVentIsMonitored = false;
    m_iVentCanPres = true;
    if(m_iVents.Count != 0){
        m_iVentIsMonitored = true;
        double[] levels = new double[m_iVents.Count];
        for(int i = 0; i <m_iVents.Count; i++){
            IMyAirVent vent = (IMyAirVent)m_iVents[i];
            levels[i] = (double)vent.GetOxygenLevel()*100;
            if (vent.CanPressurize != true){
                m_iVentCanPres = false;
            }
        }
        if(m_iVents.Count == 1){
            m_iVentLevel = levels[0];
        }
        else{
            m_iVentLevel = levels.Average();
        }
    }
    m_oVentIsMonitored = false;
    m_oVentCanPres = false;
    m_oVentPresFull = false;
    if(m_oVents.Count != 0){
        m_oVentIsMonitored = true;
        double[] levels = new double[m_oVents.Count];
        for(int i = 0; i <m_oVents.Count; i++){
            IMyAirVent vent = (IMyAirVent)m_oVents[i];
            levels[i] = (double)vent.GetOxygenLevel()*100;
            if (vent.CanPressurize == true){
                m_oVentCanPres = true;
            }
            if (levels[i] >= 80.0){
                m_oVentPresFull = true;
            }
        }
        if(m_oVents.Count == 1){
            m_oVentLevel = levels[0];
        }
        else{
            m_oVentLevel = levels.Average();
        }
    }
    mTankisMonitored = false;
    mTanksAreEmpty = false;
    mTanksAreFull = false;
    if(m_Tanks.Count != 0){
        mTankisMonitored = true;
        double[] levels = new double[m_Tanks.Count];
        for(int i = 0; i <m_Tanks.Count; i++){
            IMyGasTank tank = (IMyGasTank)m_Tanks[i];
            levels[i] = (double)tank.FilledRatio*100;
            var mTankScriptData1 = new ScriptData(tank, startString, endString);
            if (mTankScriptData1.GetScriptData() == "Fill Required"){
                mTanksAreEmpty = true;
            }
            if (levels[i] <= tankFillLevel){
                mTankScriptData1.SetScriptData("Fill Required");
            }
            else if (levels[i] >= tankFillLevel + tankFillBuffer){
                mTankScriptData1.SetScriptData("Nah, I'm good Clark");
            }
            if (levels[i] >= 100.0){
                mTanksAreFull = true;
            }
        }
        if(m_Tanks.Count == 1){
            mTankLevel = levels[0];
        }
        else{
            mTankLevel = levels.Average();
        }
    }
    lockdownEnabled = false;
    if(m_LockLights.Count != 0){
        IMyInteriorLight lockLight = (IMyInteriorLight)m_LockLights[0];
        if(lockLight.Enabled == true){
            lockdownEnabled = true;
        }
        if(m_LockLights.Count > 1){
            lockLight.Color = new Color(255, 132, 203);
            lockLight.BlinkLength = 0.0f;
            lockLight.BlinkIntervalSeconds = 0.0f;
            for(int i = 1; i < m_LockLights.Count; i++){
                IMyInteriorLight extra = (IMyInteriorLight)m_LockLights[i];
                extra.Color = new Color(255, 182, 193);
                extra.BlinkLength = 0.0f;
                extra.BlinkIntervalSeconds = 0.0f;
            }
        }
        else{
            lockLight.Color = lightsLockdownColor;
            lockLight.BlinkLength = lightsLockdownBlinkLength;
            lockLight.BlinkIntervalSeconds = lightsLockdownBlinkInterval;
        }
    } 
}

void CheckAirlockStatus(){
    airlockIsComplete.Clear();
    pressurizeModeEnabled.Clear();
    aVentCanPres.Clear();
    aVentOff.Clear();
    iVentCanPres.Clear();
    iVentIsMonitored.Clear();
    oVentCanPres.Clear();
    oVentPresFull.Clear();
    oVentIsMonitored.Clear();
    iDoorsFullClosed.Clear();
    iDoorsFullOpen.Clear();
    oDoorsFullClosed.Clear();
    oDoorsFullOpen.Clear();
    aTanksAreEmpty.Clear();
    aTanksAreFull.Clear();
    aVentLevel.Clear();
    iVentLevel.Clear();
    oVentLevel.Clear();
    aTankLevel.Clear();
    aWarnings.Clear();
    anyDoorOpen = false;
    for(int i = 0; i <IDList.Count; i++){
        var warningSB = new System.Text.StringBuilder();
        warningSB.Clear();
        airlockIsComplete.Add(true);
        aVentCanPres.Add(true);
        aVentOff.Add(true);
        aVentLevel.Add(0.0);
        if(aVents[i].Count != 0){
            double[] levels = new double[aVents[i].Count];
            for(int j = 0; j <aVents[i].Count; j++){
                IMyAirVent vent = (IMyAirVent)aVents[i][j];
                levels[j] = (double)vent.GetOxygenLevel()*100;
                if (vent.CanPressurize != true){
                    aVentCanPres[i] = false;
                }
                if (vent.Enabled == true){
                    aVentOff[i] = false;
                }
            }
            if(aVents[i].Count == 1){
                aVentLevel[i] = levels[0];
            }
            else{
                aVentLevel[i] = levels.Average();
            }
        }
        else{
            airlockIsComplete[i] = false;
            warningSB.AppendLine("ERROR: No airlock vent(s) found");
        }
        iDoorsFullClosed.Add(true);
        iDoorsFullOpen.Add(false);
        if(iDoors[i].Count != 0){
            for(int j = 0; j <iDoors[i].Count; j++){
                IMyDoor iDoor = (IMyDoor)iDoors[i][j];
                if (iDoor.OpenRatio > 0.0f){
                    iDoorsFullClosed[i] = false;
                }
                if (iDoor.OpenRatio >= 1.0f){
                    iDoorsFullOpen[i] = true;
                    anyDoorOpen = true;
                }
            }
        }
        else{
            airlockIsComplete[i] = false;
            warningSB.AppendLine("ERROR: No inner door(s) found");
        }
        oDoorsFullClosed.Add(true);
        oDoorsFullOpen.Add(true);
        if(oDoors[i].Count != 0){
            for(int j = 0; j <oDoors[i].Count; j++){
                IMyDoor oDoor = (IMyDoor)oDoors[i][j];
                if (oDoor.OpenRatio > 0.0f){
                    oDoorsFullClosed[i] = false;
                }
                if (oDoor.OpenRatio < 1.0f){
                    oDoorsFullOpen[i] = false;
                }
            }
        }
        else{
            airlockIsComplete[i] = false;
            warningSB.AppendLine("ERROR: No outer door(s) found");
        }
        pressurizeModeEnabled.Add(false);
        IMyInteriorLight aControl = (IMyInteriorLight)aControls[i][0];
        if(aControl.Enabled == true){
            pressurizeModeEnabled[i] = true;
        }
        if(aControls[i].Count > 1){
            aControl.Color = new Color(255, 132, 203);
            for(int j = 1; j <aControls[i].Count; j++){
                IMyInteriorLight extra = (IMyInteriorLight)aControls[i][j];
                extra.Color = new Color(255, 182, 193);
            }
            warningSB.AppendLine("ERROR: Multiple control lights found");
        }
        else if(airlockIsComplete[i] == false){
            aControl.Color = new Color(255, 0, 0);
        }
        else{
            aControl.Color = new Color(0, 255, 0);
        }
        iVentCanPres.Add(true);
        iVentIsMonitored.Add(false);
        iVentLevel.Add(0.0);
        if(iVents[i].Count != 0){
            iVentIsMonitored[i] = true;
            double[] levels = new double[iVents[i].Count];
            for(int j = 0; j <iVents[i].Count; j++){
                IMyAirVent vent = (IMyAirVent)iVents[i][j];
                levels[j] = (double)vent.GetOxygenLevel()*100;
                if (vent.CanPressurize != true){
                    iVentCanPres[i] = false;
                }
            }
            if(iVents[i].Count == 1){
                iVentLevel[i] = levels[0];
            }
            else{
                iVentLevel[i] = levels.Average();
            }
        }
        else if(m_iVentIsMonitored){
            iVentCanPres[i] = m_iVentCanPres;
            iVentLevel[i] = m_iVentLevel;
        }
        else{
            iVentLevel[i] = 100.0;
        }
        oVentIsMonitored.Add(false);
        oVentCanPres.Add(false);
        oVentPresFull.Add(false);
        oVentLevel.Add(0.0);
        if(oVents[i].Count != 0){
            oVentIsMonitored[i] = true;
            double[] levels = new double[oVents[i].Count];
            for(int j = 0; j <oVents[i].Count; j++){
                IMyAirVent vent = (IMyAirVent)oVents[i][j];
                levels[j] = (double)vent.GetOxygenLevel()*100;
                if (vent.CanPressurize == true){
                    oVentCanPres[i] = true;
                }
                if (levels[j] >= 80.0){
                    oVentPresFull[i] = true;
                }
            }
            if(oVents[i].Count == 1){
                oVentLevel[i] = levels[0];
            }
            else{
                oVentLevel[i] = levels.Average();
            }
        }
        else if(m_oVentIsMonitored){
            oVentCanPres[i] = m_oVentCanPres;
            oVentPresFull[i] = m_oVentPresFull;
            oVentLevel[i] = m_oVentLevel;
        }
        else{
            oVentLevel[i] = 0.0;
        }
        if(aVentCanPres[i] == false){
            aVentLevel[i] = oVentLevel[i];
        }
        aTanksAreEmpty.Add(false);
        aTanksAreFull.Add(false);
        aTankLevel.Add(50.0);
        if(aTanks[i].Count != 0){
            double[] levels = new double[aTanks[i].Count];
            for(int j = 0; j <aTanks[i].Count; j++){
                IMyGasTank aTank = (IMyGasTank)aTanks[i][j];
                levels[j] = (double)aTank.FilledRatio*100;
                var aTankScriptData1 = new ScriptData(aTank, startString, endString);
                if (aTankScriptData1.GetScriptData() == "Fill Required"){
                    aTanksAreEmpty[i] = true;
                }
                if (levels[j] <= tankFillLevel){
                    aTankScriptData1.SetScriptData("Fill Required");
                }
                else if (levels[j] >= tankFillLevel + tankFillBuffer){
                    aTankScriptData1.SetScriptData("We're all fine here now, how are you?");
                }
                if (levels[j] >= 100.0){
                    aTanksAreFull[i] = true;
                }
            }
            if(aTanks[i].Count == 1){
                aTankLevel[i] = levels[0];
            }
            else{
                aTankLevel[i] = levels.Average();
            }
        }
        else if(mTankisMonitored){
            aTankLevel[i] = mTankLevel;
            aTanksAreFull[i] = mTanksAreFull;
        }
        aWarnings.Add(warningSB.ToString());
    }
    if(anyDoorOpen == true)
    {
        doorCounter++;
    }
    if(doorCounter > doorDelay || anyDoorOpen == false){
        doorCounter = 0;
    }
}

void AirlockCycle(){
    aStatusList.Clear();
    aVentCycling.Clear();
    for(int id = 0; id < IDList.Count; id++){
        aVentCycling.Add(false);
        aStatusList.Add("Initializing Cycle Check");
        if(lockdownEnabled == true){
            aStatusList[id] = "LOCKDOWN INITIATED";
            VentControl(id, "Off");
            DoorControl(id, "Inner", "Lock");
            DoorControl(id, "Outer", "Lock");
            LightControl(id, "Lockdown");
            aControls[id][0].ApplyAction("OnOff_On");
        }
        else if(airlockIsComplete[id] == false){
            aStatusList[id] = "Incomplete!";
            DoorControl(id, "Inner", "Unlock");
            DoorControl(id, "Outer", "Unlock");
            VentControl(id, "Off");
            LightControl(id, "Warning");
        }
        else if(oVentPresFull[id] == true){
            aStatusList[id] = "Pressurized Exterior";
            DoorControl(id, "Inner", "Unlock");
            if (pressurizeModeEnabled[id] == true){
                DoorControl(id, "Outer", "Lock");
                aVentLevel[id] = iVentLevel[id];
            }
            else{
                DoorControl(id, "Outer", "Open");
                aVentLevel[id] = oVentLevel[id];
            }
            VentControl(id, "Off");
            LightControl(id, "On");
        }
        else if(aTanksAreFull[id] == true){
            aStatusList[id] = "Tanks Full: Manual Purge";
            DoorControl(id, "Inner", "Unlock");
            DoorControl(id, "Outer", "Unlock");
            VentControl(id, "Off");
            LightControl(id, "Warning");
        }
        else if(iVentCanPres[id] == false){
            aStatusList[id] = "Interior Depressurized";
            DoorControl(id, "Inner", "Unlock");
            DoorControl(id, "Outer", "Unlock");
            VentControl(id, "Off");
            LightControl(id, "Warning");
        }
        else if(aVentCanPres[id] == false && oDoorsFullClosed[id] == true && aVentOff[id] == false){
            aStatusList[id] = "Airlock Leak";
            DoorControl(id, "Inner", "Unlock");
            DoorControl(id, "Outer", "Unlock");
            LightControl(id, "Warning");
        }
        else if(aTanks[id].Count != 0 && aTanksAreEmpty[id] == true && aVentCanPres[id] == true){
            aStatusList[id] = "Hold: Filling Airlock Tank";
            DoorControl(id, "Outer", "Lock");
            if (oDoorsFullClosed[id] == true)
            {
                DoorControl(id, "Inner", "Open");
                VentControl(id, "Depressurize");
            }
            LightControl(id, "Cycling");
        }
        else if(iVentCanPres[id] == true && iVentLevel[id] < 80.00){
            aStatusList[id] = "Pressurizing Interior";
            VentControl(id, "Off");
            DoorControl(id, "Inner", "Unlock");
            DoorControl(id, "Outer", "Unlock");
            LightControl(id, "Cycling");
        }
        else{
            if(pressurizeModeEnabled[id] == true){
                DoorControl(id, "Outer", "Lock");
                if(oDoorsFullClosed[id] == true){
                    if(aVentLevel[id] >= fullPressure){
                        DoorControl(id, "Inner", "Unlock");
                        aStatusList[id] = "Pressurized";
                        LightControl(id, "On");
                    }
                    else if(aVentLevel[id] < fullPressure){
                        DoorControl(id, "Inner", "Lock");
                        LightControl(id, "Cycling");
                        VentControl(id, "Pressurize");
                        aVentCycling[id] = true;
                        if (iDoorsFullClosed[id] == true){
                            aStatusList[id] = "Pressurizing";
                        }
                        else{
                            aStatusList[id] = "Closing Inner Doors";
                        }
                    }
                }
                else{
                    aStatusList[id] = "Closing Outer Doors";
                    LightControl(id, "Cycling");
                }
            }
            else{
                DoorControl(id, "Inner", "Lock");
                if(iDoorsFullClosed[id] == true){
                    if(aVentLevel[id] <= oVentLevel[id] + 0.01){
                        DoorControl(id, "Outer", "Open");
                        if(oDoorsFullOpen[id] == true){
                            LightControl(id, "Standby");
                            if(oVentLevel[id] > 0.0){
                                aStatusList[id] = "Stabalized to Exterior";
                            }
                            else{
                                aStatusList[id] = "Depressurized";
                            }
                            VentControl(id, "Off");
                        }
                        else{
                            aStatusList[id] = "Opening Outer Doors";
                            VentControl(id, "Off");
                            LightControl(id, "Cycling");
                        }
                    }
                    else{
                        aStatusList[id] = "Depressurizing";
                        VentControl(id, "Depressurize");
                        LightControl(id, "Cycling");
                        aVentCycling[id] = true;
                    }
                }
                else{
                    aStatusList[id] = "Closing Inner Doors";
                    LightControl(id, "Cycling");	
                }
            }
        }
        OpenInnerDoors(id);
    }
}

void VentControl(int id, string pressureState){
    if (aVents[id].Count != 0){
		for(int i = 0; i <aVents[id].Count; i++){
			IMyAirVent vent = (IMyAirVent)aVents[id][i];
			if (pressureState == "Pressurize"){
				vent.Enabled = true;
                vent.Depressurize = false;
			}
			else if(pressureState == "Depressurize"){
				vent.Enabled = true;
                vent.Depressurize = true;
			}
			else if(pressureState == "Off"){
				vent.Enabled = false;
			}
		}
	}
}

void DoorControl(int id, string doorType, string openState){
    List<IMyTerminalBlock> doors = new List<IMyTerminalBlock>();
    doors.Clear();
    if(doorType == "Inner"){
        doors = iDoors[id];
    }
    else if(doorType == "Outer"){
        doors = oDoors[id];
    }
    if(doors.Count != 0){
        for(int i = 0; i <doors.Count; i++){
            IMyDoor door = (IMyDoor)doors[i];
            if(openState == "Open"){
                if (door.OpenRatio < 1){
                    door.Enabled = true;
                    door.OpenDoor();
                }
                else{
                    door.Enabled = false;
                }
            }
            else if(openState == "Lock"){
                if (door.OpenRatio > 0){
                    door.Enabled = true;
                    door.CloseDoor();
                }
                else{
                    door.Enabled = false;
                }
            }
            else if(openState == "Unlock"){
                door.Enabled = true;
            }
        }
    }
}

void OpenInnerDoors(int id){
    if(iDoors[id].Count != 0){
        for(int j = 0; j < iDoors[id].Count; j++){
            IMyDoor door = (IMyDoor)iDoors[id][j];
            var iDoorScriptData = new ScriptData(door, startString, endString);
            if(aVentLevel[id] < fullPressure && aVentCanPres[id] == true){
                iDoorScriptData.SetScriptData("Was Locked");
            }
            else if(aVentCanPres[id] == false && oVentPresFull[id] == false){
                iDoorScriptData.SetScriptData("No Air");
            }
            if(doorCounter >= doorDelay && doorAutoClose == true && iDoorScriptData.GetScriptData() == "Normal Operation"){
                door.CloseDoor();
            }
            if(iDoorScriptData.GetScriptData() == "Was Locked"){
                door.OpenDoor();
                if(door.OpenRatio >= 1.0){
                    iDoorScriptData.SetScriptData("Normal Operation");
                }
            }
        }
    }
}

void LightControl(int id, string lightState){
    if (sLights[id].Count != 0){
        for (int i = 0; i <sLights[id].Count; i++){
            IMyInteriorLight light = (IMyInteriorLight)sLights[id][i];
            if (lightState == "On"){
                light.Color = lightsPressurizedColor;
                light.BlinkLength = lightsPressurizedBlinkLength;
                light.BlinkIntervalSeconds = lightsPressurizedBlinkInterval;
            }
            else if (lightState == "Standby"){
                light.Color = lightsDepressurizedColor;
                light.BlinkLength = lightsDepressurizedBlinkLength;
                light.BlinkIntervalSeconds = lightsDepressurizedBlinkInterval;
            }
            else if (lightState == "Cycling"){
                light.Color = lightsCyclingColor;
                light.BlinkLength = lightsCyclingBlinkLength;
                light.BlinkIntervalSeconds = lightsCyclingBlinkInterval;
            }
            else if (lightState == "Warning"){
                light.Color = lightsErrorColor;
                light.BlinkLength = lightsErrorBlinkLength;
                light.BlinkIntervalSeconds = lightsErrorBlinkInterval;
            }
            else if (lightState == "Lockdown"){
                light.Color = lightsLockdownColor;
                light.BlinkLength = lightsLockdownBlinkLength;
                light.BlinkIntervalSeconds = lightsLockdownBlinkInterval;
            }
        }
    }
    if (rLights[id].Count != 0){
        for (int i = 0; i <rLights[id].Count; i++){
        IMyReflectorLight light = (IMyReflectorLight)rLights[id][i];
            light.Color = lightsCyclingColor;
            if(controlRotation == true){light.SetValue("RotationSpeed", rotationSpeed);}
            if (lightState == "Cycling"){
                light.Enabled = true;
            }
            else{
                light.Enabled = false;
            }
        }
    }
    if(aSounds[id].Count != 0){
        for (int i = 0; i <aSounds[id].Count; i++){
        IMySoundBlock sound = (IMySoundBlock)aSounds[id][i];
            var soundScriptData = new ScriptData(sound, startString, endString);
            if(controlSelectedSound == true){sound.SelectedSound = cyclingSound;}
            sound.LoopPeriod = 300.0f;
            if(lightState == "Cycling" && soundScriptData.GetScriptData() != "CyclingTrue"){
                sound.Play();
                soundScriptData.SetScriptData("CyclingTrue");
            }
            else if(lightState != "Cycling" && soundScriptData.GetScriptData() != "CyclingFalse"){
                sound.Stop();
                soundScriptData.SetScriptData("CyclingFalse");
            }
        }
    }
}

void AuxilaryControls(){
    bool mVentsCycling = false;
    for(int i = 0; i <IDList.Count; i++){
        if(aTanks[i].Count == 0 && m_Tanks.Count != 0 && aVentCycling[i] == true){
            mVentsCycling = true;
        }
        if(aTanks[i].Count != 0){
            for(int j = 0; j <aTanks[i].Count; j++){
                IMyGasTank aTank = (IMyGasTank)aTanks[i][j];
                var aTankScriptData2 = new ScriptData(aTank, startString, endString);
                if(aTankScriptData2.GetScriptData() == "Fill Required" || aVentCycling[i] == true){
                    aTank.Enabled = true;
                }
                else{
                    aTank.Enabled = false;
                }
            }
        }
        if(oVents[i].Count != 0){
            for(int j = 0; j <oVents[i].Count; j++){
                IMyAirVent vent = (IMyAirVent)oVents[i][j];
                if(oVentCanPres[i] == true){
                    vent.Depressurize = false;
                }
                else{
                    if(aTanksAreEmpty[i] == true){
                        vent.Depressurize = true;
                    }
                    else if(aVentCycling[i] == true){
                        vent.Depressurize = false;
                    }
                    else{
                        vent.Depressurize = true;
                    }
                }
            }
        }
    }
    if (m_Tanks.Count != 0){
        for(int i = 0; i <m_Tanks.Count; i++){
            IMyGasTank mTank = (IMyGasTank)m_Tanks[i];
            var mTankScriptData2 = new ScriptData(mTank, startString, endString);
            if(mTankScriptData2.GetScriptData() == "Fill Required"){
                mTank.Enabled =true;
            }
            else if(mVentsCycling == true){
                mTank.Enabled = true;
            }
            else{
                mTank.Enabled = false;
            }
        }
    }
    List<IMyGasGenerator> allGenerators = new List<IMyGasGenerator>();
    allGenerators.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(allGenerators); 
    if(allGenerators.Count != 0 && enableGeneratorControl == true){
        foreach(var generator in allGenerators){
            var genScriptData = new ScriptData(generator, startString, endString);
            if(generator.CustomData.Contains(startString + "NormalMode_") && mVentsCycling != true){
                if(generator.Enabled == true){genScriptData.SetScriptData("NormalMode_On");}
                else{genScriptData.SetScriptData("NormalMode_Off");}
            }
            else{
                if(genScriptData.GetScriptData() == "NormalMode_On"){genScriptData.SetScriptData("Enabled");}
                else if(genScriptData.GetScriptData() == "NormalMode_Off"){genScriptData.SetScriptData("Disabled");}
                if(mVentsCycling == true){generator.Enabled = false;}
                else{
                    if(genScriptData.GetScriptData() == "Enabled"){generator.Enabled = true;}
                    else if(genScriptData.GetScriptData() == "Disabled"){generator.Enabled = false;}
                    genScriptData.SetScriptData("NormalMode_");
                }
            }
        }
    }
    List<IMyOxygenFarm> allO2Farms = new List<IMyOxygenFarm>();
    allO2Farms.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyOxygenFarm>(allO2Farms);
    if(allO2Farms.Count != 0 && enableO2FarmControl == true){
        for(int i = 0; i <allO2Farms.Count; i++){
            IMyFunctionalBlock oFarm = (IMyFunctionalBlock)allO2Farms[i];
            var farmScriptData = new ScriptData(oFarm, startString, endString);
            if(oFarm.CustomData.Contains(startString + "NormalMode_") && mVentsCycling != true){
                if(oFarm.Enabled == true){farmScriptData.SetScriptData("NormalMode_On");}
                else{farmScriptData.SetScriptData("NormalMode_Off");}
            }
            else{
                if(farmScriptData.GetScriptData() == "NormalMode_On"){farmScriptData.SetScriptData("Enabled");}
                else if(farmScriptData.GetScriptData() == "NormalMode_Off"){farmScriptData.SetScriptData("Disabled");}
                else if(mVentsCycling == true){oFarm.Enabled = false;}
                else{
                    if(farmScriptData.GetScriptData() == "Enabled"){oFarm.Enabled = true;}
                    else if(farmScriptData.GetScriptData() == "Disabled"){oFarm.Enabled = false;}
                    farmScriptData.SetScriptData("NormalMode_");
                }
            }
        }
    }
    if(m_oVents.Count != 0)
    {
        for(int i = 0; i <m_oVents.Count; i++){
            IMyAirVent vent = (IMyAirVent)m_oVents[i];
            if(m_oVentCanPres == true){
                vent.Depressurize = false;
            }
            else{
                if(mTanksAreEmpty == true){
                    vent.Depressurize = true;
                }
                else if(mVentsCycling == true){
                    vent.Depressurize = false;
                }
                else{
                    vent.Depressurize = true;
                }
            }
        }
    }
}

int screenCounter = 0;
int pageNumber = 0;

RectangleF _viewport;

void LCDControl(){
    List<string> aLevelStrings = new List<string>();
    aLevelStrings.Clear();
    List<string> iLevelStrings = new List<string>();
    iLevelStrings.Clear();
    List<string> oLevelStrings = new List<string>();
    oLevelStrings.Clear();
    List<string> tLevelStrings = new List<string>();
    tLevelStrings.Clear();
    List<Color> aLevelColors = new List<Color>();
    aLevelColors.Clear();
    List<Color> iLevelColors = new List<Color>();
    iLevelColors.Clear();
    List<Color> oLevelColors = new List<Color>();
    oLevelColors.Clear();
    List<Color> tLevelColors = new List<Color>();
    tLevelColors.Clear();
    for(int i = 0; i <IDList.Count; i++){
        int aSpriteLength;
        Color aLevelColor;
        VentLevelSprite(aVentLevel[i], out aSpriteLength, out aLevelColor);
        aLevelColors.Add(aLevelColor);
        string aLevelString;
        aLevelStrings.Add("");
        if(aVents[i].Count == 0){
            aLevelString = "No Airlock Vent Found!";
            aLevelStrings[i] = "Error!";
        }
        else{
            aLevelString = aVentLevel[i].ToString("0.0") + "%";
            aLevelStrings[i] = aLevelString;
        }
        int iSpriteLength;
        Color iLevelColor;
        VentLevelSprite(iVentLevel[i], out iSpriteLength, out iLevelColor);  
        iLevelColors.Add(iLevelColor);
        string iLevelString = iVentLevel[i].ToString("0.0") + "%";
        iLevelStrings.Add("");
        if(iVents[i].Count == 0 && m_iVentIsMonitored == false){
            iLevelStrings[i] = "N/A";
            iLevelString = "N/A, Assuming 100%";
        }
        else if(iVents[i].Count == 0){
            iLevelStrings[i] = iLevelString + "(M)";
            iLevelString = iLevelString + " (Main)";
        }
        else{
            iLevelStrings[i] = iLevelString;
        }
        int oSpriteLength;
        Color oLevelColor;
        VentLevelSprite(oVentLevel[i], out oSpriteLength, out oLevelColor);
        oLevelColors.Add(oLevelColor);
        string oLevelString = oVentLevel[i].ToString("0.0") + "%";;
        oLevelStrings.Add("");
        if(oVents[i].Count == 0 && m_oVentIsMonitored == false){
            oLevelStrings[i] = "N/A";
            oLevelString = "N/A, Assuming 0%";
        }
        else if(oVents[i].Count == 0){
            oLevelStrings[i] = oLevelString + "(M)";
            oLevelString = oLevelString + " (Main)";
        }
        else{
            oLevelStrings[i] = oLevelString;
        }
        int tSpriteLength = (Convert.ToInt32(Math.Round(aTankLevel[i], 0))*4)+10;
        string tLevelString = aTankLevel[i].ToString("0.0") + "%";
        tLevelStrings.Add("");
        if(aTanks[i].Count == 0 && mTankisMonitored == false){
            tLevelStrings[i] = "N/A";
            tLevelString = "N/A, Assuming 50%";
        }
        else if(aTanks[i].Count == 0){
            tLevelStrings[i] = tLevelString + "(M)";
            tLevelString = tLevelString + " (Main)";
        }
        else{
            tLevelStrings[i] = tLevelString;
        }
        Color tLevelColor = highPressureColor;
        if(aTankLevel[i] < tankFillLevel  || aTankLevel[i] >95){tLevelColor = noPressureColor;}
        else if(aTankLevel[i] > tankFillLevel + tankFillBuffer + 2.5){tLevelColor = lowPressureColor;}
        tLevelColors.Add(tLevelColor);
        if(cLCDs[i].Count != 0){
            for (int j = 0; j <cLCDs[i].Count; j++){
                IMyTextPanel lcd = (IMyTextPanel)cLCDs[i][j];
                lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
                if(hardcodeLCDForeground == true){lcd.ScriptForegroundColor = LCDForegroundColor;}
                if(hardcodeLCDBackground == true){lcd.ScriptBackgroundColor = LCDBackgroundColor;}
                _viewport = new RectangleF((lcd.TextureSize - lcd.SurfaceSize) / 2f, lcd.SurfaceSize);
                var frame = lcd.DrawFrame();
                var position = new Vector2(10, 5) + _viewport.Position;
                var sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = (IDList[i] + "  |  " + aStatusList[i]),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position = new Vector2(25, 50) + _viewport.Position;
                var size = new Vector2(25, 25);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "IconOxygen",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(30, 3);
                size = new Vector2(420, 28);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);                
                position += new Vector2(2, 0);
                size = new Vector2(416, 24);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptBackgroundColor,
                    Alignment = TextAlignment.LEFT
                };  
                frame.Add(sprite);
                position += new Vector2(3, 0);
                size = new Vector2(aSpriteLength, 20);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = aLevelColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                frame.Dispose();
            }
        }
        if(sLCDs[i].Count != 0){
            var itemCountSB = new System.Text.StringBuilder();
            itemCountSB.Clear();
            if(airlockIsComplete[i]){
                itemCountSB.AppendLine("Airlock Vents: " + aVents[i].Count);
                itemCountSB.AppendLine("Inner Doors: " + iDoors[i].Count);
                itemCountSB.AppendLine("Outer Doors: " + oDoors[i].Count);
                itemCountSB.AppendLine("Airlock Tanks: " + aTanks[i].Count);
            }
            for (int j = 0; j <sLCDs[i].Count; j++){
                IMyTextPanel lcd = (IMyTextPanel)sLCDs[i][j];
                lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
                if(hardcodeLCDForeground == true){lcd.ScriptForegroundColor = LCDForegroundColor;}
                if(hardcodeLCDBackground == true){lcd.ScriptBackgroundColor = LCDBackgroundColor;}
                _viewport = new RectangleF((lcd.TextureSize - lcd.SurfaceSize) / 2f, lcd.SurfaceSize);
                var frame = lcd.DrawFrame();
                var position = new Vector2(10, 5) + _viewport.Position;
                var sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = (IDList[i] + "  |  " + aStatusList[i]),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(0, 30);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = ("Airlock Vent: " + aLevelString),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = ("Inner Vent: " + iLevelString),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = ("Outer Vent: " + oLevelString),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = ("Tank Average: " + tLevelString),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(10, 90);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = (aWarnings[i]),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = (itemCountSB.ToString()),
                    Position = position,
                    RotationOrScale = 1.0f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position = new Vector2(25, 82) + _viewport.Position;
                var size = new Vector2(25, 25);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "IconOxygen",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "IconOxygen",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "IconOxygen",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "IconOxygen",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position = new Vector2(55, 82) + _viewport.Position;
                size = new Vector2(420, 25);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptForegroundColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                position = new Vector2(57, 82) + _viewport.Position;
                size = new Vector2(416, 21);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptBackgroundColor,
                    Alignment = TextAlignment.LEFT
                };  
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptBackgroundColor,
                    Alignment = TextAlignment.LEFT
                };  
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptBackgroundColor,
                    Alignment = TextAlignment.LEFT
                };  
                frame.Add(sprite);
                position += new Vector2(0, 60);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = lcd.ScriptBackgroundColor,
                    Alignment = TextAlignment.LEFT
                };  
                frame.Add(sprite);
                position = new Vector2(60, 82) + _viewport.Position;
                size = new Vector2(aSpriteLength, 17);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = aLevelColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                size = new Vector2(iSpriteLength, 17);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = iLevelColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                size = new Vector2(oSpriteLength, 17);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = oLevelColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                position += new Vector2(0, 60);
                size = new Vector2(tSpriteLength, 17);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Data = "SquareSimple",
                    Position = position,
                    Size = size,
                    Color = tLevelColor,
                    Alignment = TextAlignment.LEFT
                };
                frame.Add(sprite);
                frame.Dispose();
            }
        }
    }
    int maxPerList = 6;
    int startInt;
    int totalPages = (int)Math.Ceiling((double)IDList.Count/maxPerList);
    screenCounter++;
    if(screenCounter>screenDelay){
        screenCounter=0;
    }
    if(screenCounter == screenDelay){
        pageNumber++;
        if(pageNumber >= totalPages)
        {
            pageNumber=0;
        }
    }
    startInt = pageNumber*maxPerList;
    int endInt;
    if(IDList.Count - startInt < maxPerList){
        endInt = IDList.Count;
    }
    else{
        endInt = startInt + maxPerList;
    }
    if (m_LCDs.Count != 0){
        string isProgRunning;
        RunningCheck (out isProgRunning);
        var statusSB = new System.Text.StringBuilder();
        statusSB.Clear();
        statusSB.AppendLine("Program Status: " + isProgRunning);
        statusSB.AppendLine("Airlocks: " + IDList.Count + "  |  Main Tanks: " + m_Tanks.Count);
        for (int j = 0; j <m_LCDs.Count; j++){
            IMyTextPanel lcd = (IMyTextPanel)m_LCDs[j];
            lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
            if(hardcodeLCDForeground == true){lcd.ScriptForegroundColor = LCDForegroundColor;}
            if(hardcodeLCDBackground == true){lcd.ScriptBackgroundColor = LCDBackgroundColor;}
            _viewport = new RectangleF((lcd.TextureSize - lcd.SurfaceSize) / 2f, lcd.SurfaceSize);
            var frame = lcd.DrawFrame();
            var position = new Vector2(10, 5) + _viewport.Position;
            var sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = (statusSB.ToString()),
                Position = position,
                RotationOrScale = 1.0f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(50, 65);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Airlock\nVent",
                Position = position,
                RotationOrScale = 0.8f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(100, 0);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Inner\nVent",
                Position = position,
                RotationOrScale = 0.8f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(120, 0);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Outer\nVent",
                Position = position,
                RotationOrScale = 0.8f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(110, 0);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Tank\nAvg.",
                Position = position,
                RotationOrScale = 0.8f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position = new Vector2(10, 120) + _viewport.Position;
            var size = new Vector2(490, 2);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = position,
                Size = size,
                Color = lcd.ScriptForegroundColor,
                Alignment = TextAlignment.LEFT
            };
            frame.Add(sprite);
            for (int k = startInt; k < endInt; k++){
                position = new Vector2(10,125 + (k - maxPerList*pageNumber) * 50) + _viewport.Position;;
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = (IDList[k] + "  |  "+ aStatusList[k]),
                    Position = position,
                    RotationOrScale = 0.8f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(50, 25);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = aLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.8f,
                    Color = aLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(100, 0);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = iLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.8f,
                    Color = iLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(120, 0);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = oLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.8f,
                    Color = oLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(110, 0);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = tLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.8f,
                    Color = tLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
            }
            position = new Vector2(10, 480) + _viewport.Position;
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = programName + " " + programVersion,
                Position = position,
                RotationOrScale = 0.8f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(490, 0);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Screen " + (pageNumber + 1) + " of " + totalPages,
                Position = position,
                RotationOrScale = 0.8f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.RIGHT,
                FontId = "White"
            };
            frame.Add(sprite);
            frame.Dispose();
        }
    }
    if (m_Panels.Count != 0){
        for (int j = 0; j <m_Panels.Count; j++){
            IMyTextPanel lcd = (IMyTextPanel)m_Panels[j];
            lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.SCRIPT;
            if(hardcodeLCDForeground == true){lcd.ScriptForegroundColor = LCDForegroundColor;}
            if(hardcodeLCDBackground == true){lcd.ScriptBackgroundColor = LCDBackgroundColor;}
            _viewport = new RectangleF((lcd.TextureSize - lcd.SurfaceSize) / 2f, lcd.SurfaceSize);
            var frame = lcd.DrawFrame();
            var position = new Vector2(60, 5) + _viewport.Position;
            var sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Airlock\nVent",
                Position = position,
                RotationOrScale = 0.6f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(100, 0);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Inner\nVent",
                Position = position,
                RotationOrScale = 0.6f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(120, 0);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Outer\nVent",
                Position = position,
                RotationOrScale = 0.6f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position += new Vector2(110, 0);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Tank\nAvg.",
                Position = position,
                RotationOrScale = 0.6f,
                Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position = new Vector2(10, 45) + _viewport.Position;
            var size = new Vector2(490, 2);
            sprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = position,
                Size = size,
                Color = lcd.ScriptForegroundColor,
                Alignment = TextAlignment.LEFT
            };
            frame.Add(sprite);
            for (int k = startInt; k < endInt; k++){
                position = new Vector2(10,50 + (k - maxPerList*pageNumber) * 40) + _viewport.Position;;
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = (IDList[k] + "  |  "+ aStatusList[k]),
                    Position = position,
                    RotationOrScale = 0.6f,
                    Color = lcd.ScriptForegroundColor.Alpha(0.66f),
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(50, 20);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = aLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.6f,
                    Color = aLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(100, 0);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = iLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.6f,
                    Color = iLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(120, 0);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = oLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.6f,
                    Color = oLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
                position += new Vector2(110, 0);
                sprite = new MySprite()
                {
                    Type = SpriteType.TEXT,
                    Data = tLevelStrings[k],
                    Position = position,
                    RotationOrScale = 0.6f,
                    Color = tLevelColors[k],
                    Alignment = TextAlignment.LEFT,
                    FontId = "White"
                };
                frame.Add(sprite);
            }
            frame.Dispose();
        }
    }
}

void VentLevelSprite(double level, out int spriteLength, out Color spriteColor){
    spriteLength = (Convert.ToInt32(Math.Round(level, 0))*4)+10;
    spriteColor = lowPressureColor;
    if(spriteLength >= 330){spriteColor = highPressureColor;}
    else if(spriteLength <= 110){spriteColor = noPressureColor;}
}

int counter;
int carrots;
void RunningCheck(out string running){
    counter++;
    if(counter>10){
        counter=0;
    }
    if(counter == 10){
        carrots++;
        if(carrots>3){
            carrots=0;
        }
    }
    string ellipsisString = new string('>', carrots);
    if (IDList.Count != 0){
        running = "Running"+ellipsisString;
    }
    else{
        running = "No Airlocks!\n  Check control light(s) naming";
    }
}

void EchoDiagnostics(){
    string isProgRunning;
    RunningCheck (out isProgRunning);
    var echoSB = new System.Text.StringBuilder();
    echoSB.Clear();
    echoSB.AppendLine(programName + " " + programVersion + " | " + isProgRunning);
    echoSB.AppendLine("");
    if(IDList.Count != 0){
        echoSB.AppendLine("Total Airlocks: " + IDList.Count.ToString());
        echoSB.AppendLine("");
        for(int i = 0; i <IDList.Count; i++){
            if(airlockIsComplete[i] == false){
                echoSB.AppendLine(IDList[i]);
                echoSB.AppendLine(aWarnings[i]);
            }
        }
    }
	Echo(echoSB.ToString());
	IMyTextSurface Log = Me.GetSurface(0);
	Log.ContentType = ContentType.TEXT_AND_IMAGE;
	Log.WriteText(echoSB.ToString());
}

 public class ScriptData{
    public IMyTerminalBlock block { get; private set; }
    public string startString { get; private set; }
    public string endString { get; private set; }

    public ScriptData(IMyTerminalBlock block, string startString, string endString){
        this.block = block;
        this.startString = startString;
        this.endString = endString;
    }

    public string GetScriptData(){
        var rawData = block.CustomData;

        if (rawData.Contains(startString) == false || rawData.Contains(endString) == false){
            return "Error: data Indexes not found";
        }

        var startIndex = rawData.IndexOf(startString);
        var endIndex = rawData.IndexOf(endString);
        var length = endIndex - startIndex;
        if (length < 0){
            return "Error: endIndex is before startIndex";
        }
        var data = rawData.Substring(startIndex + startString.Length, length - startString.Length);
        return data;
    }
    public void RemoveScriptData(){
        var rawData = block.CustomData;
        if (rawData.Contains(startString) == false || rawData.Contains(endString) == false)
        {
            throw new Exception("Error: RemoveScriptData can not find Indexes");
        }
        var startIndex = rawData.IndexOf(startString);
        var endIndex = rawData.IndexOf(endString);
        var length = endIndex - startIndex;
        if (length < 0){
            throw new Exception("Error: RemoveScriptData endIndex is before startIndex");
        }
        block.CustomData = rawData.Remove(startIndex, length + endString.Length);
    }
    public void SetScriptData(string data){
        var rawData = block.CustomData;
        if (rawData.Contains(startString) == true && rawData.Contains(endString) == true){
            RemoveScriptData();
        }
        rawData = block.CustomData;
        var scriptData = startString + data + endString;
        block.CustomData = rawData + scriptData;
    }
}