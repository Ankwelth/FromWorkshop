public IMyCubeGrid Grid;

public DateTime renewListTime = DateTime.Now;

public List<IMyTerminalBlock>
batteryList = new List<IMyTerminalBlock>(),
reactorList = new List<IMyTerminalBlock>(),
solarList = new List<IMyTerminalBlock>(),
turbineList = new List<IMyTerminalBlock>(),
engineList = new List<IMyTerminalBlock>();

public List<IMyCubeGrid> includedGrids = new List<IMyCubeGrid>();

public List<GraphColumn>
batteryGraph = new List<GraphColumn>(),
reactorGraph = new List<GraphColumn>(),
solarGraph = new List<GraphColumn>(),
turbineGraph = new List<GraphColumn>(),
engineGraph = new List<GraphColumn>();

public bool
renewingLists = false, sameGridOnly = true,
renewedBatteries = false, renewedReactors = false,
renewedSolarPanels = false, gettingNumbers = false,
renewedSolarPower = false, renewedReactorPower = false,
renewedBatteryPower = false, rightScript = false,
rightVersion = false, renewedTurbines = false,
renewedTurbinePower = false, renewedEngines = false,
renewedEnginePower = false, controlEngines = false,
enginesEnabled = true, autoLoadSettings = true,
renewedGrids = false;

public float
solarRatio = 0F, batteryRatio = 0F, reactorRatio = 0F,
solarCurr = 0F, solarMax = 0F, batteryCurr = 0F,
batteryMax = 0F, reactorCurr = 0F, reactorMax = 0F,
HighOutputRatio = 0.99F, batteryStorCurr = 0F, batteryStorMax = 0F,
batteryStorRatio = 0F, LowBatteryStoreRatio = 0.15F,
turbineCurr = 0f, turbineMax = 0f, turbineRatio = 0f,
engineCurr = 0f, engineMax = 0f, engineRatio = 0f,
monospaceWidth = 19.459459f, monospaceHeight = 28.8f;

public int
index = 0, reps = 0, graphIndex = 0, graphWidth = 15, graphHeight = 9, width = 0, height = 12;

public double RecountDelay = 1.5, version = 1.05;

public string
LPanelDef = "MyObjectBuilder_TextPanel/LargeLCDPanel",
WPanelDef = "MyObjectBuilder_TextPanel/LargeLCDPanelWide",
XLPanelDef = "MyObjectBuilder_TextPanel/LargeLCDPanel2x2",
TPanelDef = "MyObjectBuilder_TextPanel/LargeTextPanel",
SPanelDef = "MyObjectBuilder_TextPanel/SmallLCDPanel",
SWPanelDef = "MyObjectBuilder_TextPanel/SmallLCDPanelWide",
STPanelDef = "MyObjectBuilder_TextPanel/SmallTextPanel",
cornerPanelDef = "blockcorner", scriptName = "Simple Power Manager",
settingsBackup = "", includeGridKeyword = "includeGrid";

public VRage.Game.GUI.TextPanel.ContentType content = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    Grid = Me.CubeGrid;
    LoadData();
    Save();
}

public void Save()
{

}

public void Main(string arg, UpdateType updateSource)
{
    try {
        string argument = arg;
        reps = 0;
        if (autoLoadSettings && Me.CustomData.Contains(";") && Me.CustomData != settingsBackup) argument = "load";
        if (argument != "") Commands(argument);
        else if (RenewList() && RenewNumbers()) {
            SetRuntimes(10);
            OutputValues();
            ControlReactors();
            ControlEngines();
            OutputGraphs(batteryGraph, "battery", "Battery Usage");
            OutputGraphs(solarGraph, "solar", "Solar Usage");
            OutputGraphs(reactorGraph, "reactor", "Reactor Usage");
            OutputGraphs(turbineGraph, "turbine", "Turbine Usage");
            OutputGraphs(engineGraph, "engine", "Engine Usage");
            OutputSummary();
        } else SetRuntimes(1);
    } catch { Echo("Error caught in main"); }
}

public void ControlReactors()
{
    bool enabled = false;
    if (SolarOverworked() && TurbinesOverworked() && EnginesOverworked() && BatteriesOverworked()) enabled = true;
    for (int i = 0; i < reactorList.Count; i++)
        ((IMyFunctionalBlock)reactorList[i]).Enabled = enabled;
}

public void ControlEngines()
{
    if (controlEngines) {
        bool enabled = false;
        if (SolarOverworked() && TurbinesOverworked() && BatteriesOverworked()) enabled = true;
        for (int i = 0; i < engineList.Count; i++)
            ((IMyFunctionalBlock)engineList[i]).Enabled = enabled;
        enginesEnabled = enabled;
    }
}

public bool EnginesOverworked()
{
    return !enginesEnabled || engineList.Count == 0 || engineRatio >= HighOutputRatio;
}

public bool TurbinesOverworked()
{
    return turbineList.Count == 0 || turbineRatio >= HighOutputRatio;
}

public bool SolarOverworked()
{
    return solarList.Count == 0 || solarRatio >= HighOutputRatio;
}

public bool BatteriesOverworked()
{
    return batteryList.Count == 0 || (batteryRatio >= HighOutputRatio || batteryStorRatio < LowBatteryStoreRatio);
}

public void Commands(string argument)
{
    string arg = argument.ToLower().Replace(" ", "");
    if (arg == "save") SaveData();
    else if (arg == "load") LoadData();
}

public void OutputValues()
{
    string summary = "", lineA = "", lineB = "";
    lineA = "(" + solarList.Count + ") Solar Usage:";
    lineB = RoundPower(solarCurr) + " / " + RoundPower(solarMax) + " = " + solarRatio.ToString("N3");
    width = lineA.Length;
    if (lineB.Length > width) width = lineB.Length;
    Echo(lineA);
    Echo(lineB);
    summary = lineA + Environment.NewLine + lineB;
    lineA = "(" + reactorList.Count + ") Reactor Usage:";
    lineB = RoundPower(reactorCurr) + " / " + RoundPower(reactorMax) + " = " + reactorRatio.ToString("N3");
    if (lineA.Length > width) width = lineA.Length;
    if (lineB.Length > width) width = lineB.Length;
    Echo(lineA);
    Echo(lineB);
    summary += Environment.NewLine + lineA + Environment.NewLine + lineB;
    lineA = "(" + batteryList.Count + ") Battery Usage:";
    lineB = RoundPower(batteryCurr) + " / " + RoundPower(batteryMax) + " = " + batteryRatio.ToString("N3");
    if (lineA.Length > width) width = lineA.Length;
    if (lineB.Length > width) width = lineB.Length;
    Echo(lineA);
    Echo(lineB);
    summary += Environment.NewLine + lineA + Environment.NewLine + lineB;
    lineA = "(" + batteryList.Count + ") Battery Storage:";
    lineB = RoundPower(batteryStorCurr) + " / " + RoundPower(batteryStorMax) + " = " + batteryStorRatio.ToString("N3");
    if (lineA.Length > width) width = lineA.Length;
    if (lineB.Length > width) width = lineB.Length;
    Echo(lineA);
    Echo(lineB);
    summary += Environment.NewLine + lineA + Environment.NewLine + lineB;
    lineA = "(" + turbineList.Count + ") Turbine Usage:";
    lineB = RoundPower(turbineCurr) + " / " + RoundPower(turbineMax) + " = " + turbineRatio.ToString("N3");
    if (lineA.Length > width) width = lineA.Length;
    if (lineB.Length > width) width = lineB.Length;
    Echo(lineA);
    Echo(lineB);
    summary += Environment.NewLine + lineA + Environment.NewLine + lineB;
    lineA = "(" + engineList.Count + ") Engine Usage:";
    lineB = RoundPower(engineCurr) + " / " + RoundPower(engineMax) + " = " + engineRatio.ToString("N3");
    if (lineA.Length > width) width = lineA.Length;
    if (lineB.Length > width) width = lineB.Length;
    Echo(lineA);
    Echo(lineB);
    summary += Environment.NewLine + lineA + Environment.NewLine + lineB;
    OutputToPanel(summary, "power");
}

public void OutputGraphs(List<GraphColumn> list, string panelKeyword, string panelHeader)
{
    List<StringBuilder> graphRows = new List<StringBuilder>();
    for (int i = 0; i < list.Count; i++) {
        for (int x = 0; x < list[i].charList.Count; x++) {
            if (graphRows.Count == x) graphRows.Add(new StringBuilder());
            graphRows[x].Append(list[i].charList[x]);
        }
    }
    OutputToPanel(BuilderListToString(graphRows, panelHeader), panelKeyword);
}

public void OutputSummary()
{
    try {
        List<StringBuilder> graphRows = new List<StringBuilder>();
        graphRows.Add(new StringBuilder());
        graphRows[0].Append(("Bat " + (batteryRatio * 100f).ToString("N0") + "%").PadRight(8) + "┬");
        graphRows[0].Append(("Eng " + (engineRatio * 100f).ToString("N0") + "%").PadRight(8) + "┬");
        graphRows[0].Append(("Rct " + (reactorRatio * 100f).ToString("N0") + "%").PadRight(8) + "┬");
        graphRows[0].Append(("Slr " + (solarRatio * 100f).ToString("N0") + "%").PadRight(8) + "┬");
        graphRows[0].Append(("Trb " + (turbineRatio * 100f).ToString("N0") + "%").PadRight(8));
        graphRows.Add(new StringBuilder());
        string spacer = "".PadRight(8, '─');
        graphRows[1].Append(spacer + "┼" + spacer + "┼" + spacer + "┼" + spacer + "┼" + spacer);
        for (int i = 0; i < graphHeight; i++) {
            graphRows.Add(new StringBuilder());
            graphRows[graphRows.Count - 1].Append(GetGraphSummary(batteryGraph, i) + "│");
            graphRows[graphRows.Count - 1].Append(GetGraphSummary(engineGraph, i) + "│");
            graphRows[graphRows.Count - 1].Append(GetGraphSummary(reactorGraph, i) + "│");
            graphRows[graphRows.Count - 1].Append(GetGraphSummary(solarGraph, i) + "│");
            graphRows[graphRows.Count - 1].Append(GetGraphSummary(turbineGraph, i));
            graphRows.Add(graphRows[graphRows.Count - 1]);
            graphRows.Add(graphRows[graphRows.Count - 1]);
        }
        graphRows.Add(new StringBuilder());
        graphRows[graphRows.Count - 1].Append(spacer + "┴" + spacer + "┴" + spacer + "┴" + spacer + "┴" + spacer);
        OutputToPanel(BuilderListToString(graphRows, "", false), "graph");
    } catch { Echo("Error caught making graph summary"); }
}

public string GetGraphSummary(List<GraphColumn> list, int row)
{
    string summary = "";
    try {
        int column = 0, summaryWidth = 8;
        if (list.Count > summaryWidth) column = list.Count - summaryWidth;
        if (list.Count < summaryWidth) summary = summary.PadLeft(summaryWidth - list.Count);
        if (list.Count > 0) {
            for (int i = column; i < list.Count; i++) {
                summary += list[i].charList[row].ToString();
            }
        }
    } catch { Echo("Error caught in graph summary"); }
    return summary;
}

public string BuilderListToString(List<StringBuilder> list, string panelHeader, bool pad = true)
{
    StringBuilder s = new StringBuilder();
    if (panelHeader != "") s.Append(panelHeader);
    for (int i = 0; i < list.Count; i++) {
        if (s.Length != 0) s.AppendLine();
        if (pad) s.Append(list[i].ToString().PadLeft(graphWidth));
        else s.Append(list[i].ToString());
    }
    return s.ToString();
}

public void OutputToPanel(string text, string panelKeyword)
{
    List<IMyTerminalBlock> textBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(textBlocks, (p => p.CustomName.ToLower().Contains(panelKeyword.ToLower()) && IncludeBlock(p)));
    Vector2 fontSize = new Vector2(monospaceWidth * (float)graphWidth, monospaceHeight * ((float)graphHeight + 1f));
    if (panelKeyword == "power") fontSize = new Vector2(monospaceWidth * (float)width, monospaceHeight * (float)height);
    else if (panelKeyword == "graph") fontSize = new Vector2(monospaceWidth * 44f, monospaceHeight * ((float)(graphHeight * 3) + 3f));
    for (int i = 0; i < textBlocks.Count; i++) {
        ((IMyTextPanel)textBlocks[i]).Font = "Monospace";
        AutosizedPanel panel = new AutosizedPanel();
        panel.block = textBlocks[i];
        panel.surface = ((IMyTextSurface)textBlocks[i]);
        SizePanel(panel, fontSize);
        panel.surface.ContentType = content;
        panel.surface.WriteText(text);
    }
}

public void AddColumn()
{
    batteryGraph.Add(new GraphColumn());
    reactorGraph.Add(new GraphColumn());
    solarGraph.Add(new GraphColumn());
    turbineGraph.Add(new GraphColumn());
    engineGraph.Add(new GraphColumn());
    while (batteryGraph.Count > graphWidth) {batteryGraph.RemoveAt(0);}
    while (reactorGraph.Count > graphWidth) {reactorGraph.RemoveAt(0);}
    while (solarGraph.Count > graphWidth) {solarGraph.RemoveAt(0);}
    while (turbineGraph.Count > graphWidth) {turbineGraph.RemoveAt(0);}
    while (engineGraph.Count > graphWidth) {engineGraph.RemoveAt(0);}
    EditColumn(batteryRatio, ref batteryGraph);
    EditColumn(reactorRatio, ref reactorGraph);
    EditColumn(solarRatio, ref solarGraph);
    EditColumn(turbineRatio, ref turbineGraph);
    EditColumn(engineRatio, ref engineGraph);
}

public void EditColumn(float ratio, ref List<GraphColumn> list)
{
    int position = (graphHeight - 1) - (int)((double)(graphHeight - 1) * ratio);
    for (int i = 0; i < graphHeight; i++) {
        char b = ' ';
        if (i == position) b = '█';
        list[list.Count - 1].charList.Add(b);
    }
}

public void SetRuntimes(int runtime = 10)
{
    Runtime.UpdateFrequency = UpdateFrequency.None;
    Runtime.UpdateFrequency &= ~UpdateFrequency.Update1;
    Runtime.UpdateFrequency &= ~UpdateFrequency.Update10;
    if (runtime == 1) Runtime.UpdateFrequency = UpdateFrequency.Update1;
    else Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void LoadData()
{
    Echo("Loading data");
    rightScript = false;
    rightVersion = false;
    if (Me.CustomData != "" && Storage != Me.CustomData) Storage = Me.CustomData;
    if (Storage != "")
    {
        try
        {
            if (Me.CustomData == "") Me.CustomData = Storage;
            if (autoLoadSettings) settingsBackup = Me.CustomData;
            string tmpB = Storage, tmpA = "";
            tmpB = tmpB.Replace("\r\n", String.Empty);
            tmpB = tmpB.Replace("\n", String.Empty);
            tmpB = tmpB.Replace("\r", String.Empty);
            tmpB = tmpB.Replace("\t", String.Empty);
            string[] settingArray = tmpB.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < settingArray.Length; i++)
            {
                tmpA = settingArray[i];
                if (!string.IsNullOrEmpty(tmpA) && tmpA != "" && tmpA.Contains("|"))
                    ProcessSetting(settingArray[i]);
            }
            if (!rightScript || !rightVersion) SaveData();
            else Echo("Settings: " + settingArray.Length);
            rightScript = true;
            rightVersion = true;
        }
        catch { Echo("Error loading data"); }
    }
    else SaveData();
}

public void SaveData()
{
    try
    {
        Echo("Saving data");
        if (Me.CustomData == Storage || Me.CustomData == "")
        {
            string sDA = "";
            sDA = "RecountDelay|" + RecountDelay.ToString("N2") + ";";
            AddSet(ref sDA, "HighOutputRatio|" + HighOutputRatio.ToString("N3"));
            AddSet(ref sDA, "LowBatteryStoreRatio|" + LowBatteryStoreRatio.ToString("N3"));
            AddSet(ref sDA, "graphHeight|" + graphHeight);
            AddSet(ref sDA, "graphWidth|" + graphWidth);
            AddSet(ref sDA, "controlEngines|" + controlEngines);
            AddSet(ref sDA, "autoLoadSettings|" + autoLoadSettings);
            AddSet(ref sDA, "sameGridOnly|" + sameGridOnly);
            AddSet(ref sDA, "includeGridKeyword|" + includeGridKeyword);
            AddSet(ref sDA, "scriptName|" + scriptName);
            AddSet(ref sDA, "version|" + version.ToString("N2"));
            Me.CustomData = sDA;
            if (autoLoadSettings) settingsBackup = Me.CustomData;
            Storage = sDA;
            Echo("Saved data");
        }
        else
        {
            Storage = Me.CustomData;
            Echo("User Settings Moved From Custom Data To Storage" + Environment.NewLine +
            "Use Load Or Recompile To Load Settings Into Script");
        }
    }
    catch { Echo("Error Saving Data"); }
}

public void ProcessSetting(string sTX)
{
    try
    {
        int setIndex = sTX.IndexOf("|"), lH = sTX.Length;
        setIndex++;
        bool sBL = sTX.ToLower().Contains("true");
        double sDB = -123.321;
        string sST = sTX.Substring((sTX.IndexOf("|") + 1), sTX.Length - (sTX.IndexOf("|") + 1));
        try
        {
            sDB = double.Parse(sTX.Substring(setIndex, lH - setIndex));
        }
        catch { }
        try
        {
            if (sTX.StartsWith("LowBatteryStoreRatio|")) LowBatteryStoreRatio = (float)sDB;
            else if (sTX.StartsWith("HighOutputRatio|")) HighOutputRatio = (float)sDB;
            else if (sTX.StartsWith("graphHeight|")) graphHeight = (int)sDB;
            else if (sTX.StartsWith("graphWidth|")) graphWidth = (int)sDB;
            else if (sTX.StartsWith("RecountDelay|")) RecountDelay = sDB;
            else if (sTX.StartsWith("controlEngines|")) controlEngines = sBL;
            else if (sTX.StartsWith("sameGridOnly|")) sameGridOnly = sBL;
            else if (sTX.StartsWith("includeGridKeyword|")) includeGridKeyword = sST;
            else if (sTX.StartsWith("autoLoadSettings|")) autoLoadSettings = sBL;
            else if (sTX.StartsWith("version|")) {
                if (sDB == version) rightVersion = true;
            } else if (sTX.StartsWith("scriptName|")) {
                if (sST == scriptName) rightScript = true;
            }
            Echo("Processed Setting: " + sTX);
        }
        catch { Echo("Error Processing Setting: " + sTX); }
    }
    catch { Echo("Error Processing Setting: " + sTX); }
}

public void AddSet(ref string sDA, string tmpA)
{
    sDA += Environment.NewLine + tmpA + ";";
}

public bool RenewNumbers()
{
    if (!gettingNumbers) {
        gettingNumbers = true;
        solarCurr = 0F;
        solarMax = 0F;
        solarRatio = 0F;
        batteryCurr = 0F;
        batteryMax = 0F;
        batteryRatio = 0F;
        batteryStorCurr = 0F;
        batteryStorMax = 0F;
        batteryStorRatio = 0F;
        reactorCurr = 0F;
        reactorMax = 0F;
        reactorRatio = 0F;
        turbineCurr = 0f;
        turbineMax = 0f;
        turbineRatio = 0f;
        engineCurr = 0f;
        engineMax = 0f;
        engineRatio = 0f;
    }
    if (!renewedBatteryPower) {
        for (int i = index; i < batteryList.Count; i++) {
            reps++;
            index++;
            GetNumbers(batteryList[i]);
            if (reps >= graphHeight / 2) break;
        }
        if (index >= batteryList.Count) {
            index = 0;
            batteryRatio = batteryCurr / batteryMax;
            if (batteryList.Count == 0) batteryRatio = 0f;
            else if (batteryMax == 0f && batteryCurr == 0F) batteryRatio = 1f;
            batteryStorRatio = batteryStorCurr / batteryStorMax;
            if (batteryList.Count == 0) batteryStorRatio = 0f;
            renewedBatteryPower = true;
        }
        return false;
    } else if (!renewedReactorPower) {
        for (int i = index; i < reactorList.Count; i++) {
            reps++;
            index++;
            GetNumbers(reactorList[i]);
            if (reps >= graphHeight / 2) break;
        }
        if (index >= reactorList.Count) {
            index = 0;
            reactorRatio = reactorCurr / reactorMax;
            if (reactorList.Count == 0) reactorRatio = 0f;
            renewedReactorPower = true;
        }
        return false;
    } else if (!renewedSolarPower) {
        for (int i = index; i < solarList.Count; i++) {
            reps++;
            index++;
            GetNumbers(solarList[i]);
            if (reps >= graphHeight / 2) break;
        }
        if (index >= solarList.Count) {
            index = 0;
            solarRatio = solarCurr / solarMax;
            if (solarList.Count == 0) solarRatio = 0f;
            else if (solarMax == 0f) solarRatio = 1f;
            renewedSolarPower = true;
        }
        return false;
    } else if (!renewedTurbinePower) {
        for (int i = index; i < turbineList.Count; i++) {
            reps++;
            index++;
            GetNumbers(turbineList[i]);
            if (reps >= graphHeight / 2) break;
        }
        if (index >= turbineList.Count) {
            index = 0;
            turbineRatio = turbineCurr / turbineMax;
            if (turbineList.Count == 0) turbineRatio = 0f;
            renewedTurbinePower = true;
        }
        return false;
    } else if (!renewedEnginePower) {
        for (int i = index; i < engineList.Count; i++) {
            reps++;
            index++;
            GetNumbers(engineList[i]);
            if (reps >= graphHeight / 2) break;
        }
        if (index >= engineList.Count) {
            index = 0;
            engineRatio = engineCurr / engineMax;
            if (engineList.Count == 0) engineRatio = 0f;
            renewedEnginePower = true;
            AddColumn();
        }
        return false;
    }
    return true;
}

public void GetNumbers(IMyTerminalBlock block)
{
    if (block is IMyBatteryBlock) {
        if (((IMyBatteryBlock)block).ChargeMode != ChargeMode.Recharge) {
            batteryCurr += ((IMyBatteryBlock)block).CurrentOutput * 1000000F;
            batteryMax += ((IMyBatteryBlock)block).MaxOutput * 1000000F;
        }
        batteryStorCurr += ((IMyBatteryBlock)block).CurrentStoredPower * 1000000F;
        batteryStorMax += ((IMyBatteryBlock)block).MaxStoredPower * 1000000F;
    }
    else if (block is IMyReactor) {
        reactorCurr += ((IMyReactor)block).CurrentOutput * 1000000F;
        reactorMax += ((IMyReactor)block).MaxOutput * 1000000F;
    }
    else if (block is IMySolarPanel) {
        solarCurr += ((IMySolarPanel)block).CurrentOutput * 1000000F;
        solarMax += ((IMySolarPanel)block).MaxOutput * 1000000F;
    } else if (block.BlockDefinition.ToString().Contains("WindTurbine")) {
        turbineCurr += ((IMyPowerProducer)block).CurrentOutput * 1000000F;
        turbineMax += ((IMyPowerProducer)block).MaxOutput * 1000000F;
    } else if (block.BlockDefinition.ToString().Contains("HydrogenEngine")) {
        engineCurr += ((IMyPowerProducer)block).CurrentOutput * 1000000F;
        engineMax += ((IMyPowerProducer)block).MaxOutput * 1000000F;
    }
}

public bool RenewList()
{
    if (DateTime.Now >= renewListTime) {
        if (!renewingLists) {
            renewingLists = true;
            gettingNumbers = false;
            //Clear lists
            batteryList.Clear();
            reactorList.Clear();
            solarList.Clear();
            turbineList.Clear();
            engineList.Clear();
            includedGrids.Clear();
            //Set list renewals
            renewedBatteries = false;
            renewedReactors = false;
            renewedSolarPanels = false;
            renewedTurbines = false;
            renewedEngines = false;
            renewedGrids = false;
            //Set power renewals
            renewedBatteryPower = false;
            renewedReactorPower = false;
            renewedSolarPower = false;
            renewedTurbinePower = false;
            renewedEnginePower = false;
        }
        if (!renewedGrids) {
            List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(blocks, p => p.CubeGrid != Grid && p.CustomData.ToLower().Contains(includeGridKeyword.ToLower()));
            for (int i = 0; i < blocks.Count; i++)
                if (!includedGrids.Contains(blocks[i].CubeGrid)) includedGrids.Add(blocks[i].CubeGrid);
            includedGrids.Add(Grid);
            renewedGrids = true;
        }
        else if (!renewedBatteries) {
            GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteryList, p => IncludeBlock(p));
            renewedBatteries = true;
        } else if (!renewedReactors) {
            GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactorList, p => IncludeBlock(p));
            renewedReactors = true;
        } else if (!renewedSolarPanels) {
            GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solarList, p => IncludeBlock(p));
            renewedSolarPanels = true;
        } else if (!renewedTurbines) {
            GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(turbineList, p => IncludeBlock(p) && p.BlockDefinition.ToString().Contains("WindTurbine") && ((IMyPowerProducer)p).MaxOutput > 0F);
            renewedTurbines = true;
        } else if (!renewedEngines) {
            GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(engineList, p => IncludeBlock(p) && p.BlockDefinition.ToString().Contains("HydrogenEngine") && (controlEngines || ((IMyFunctionalBlock)p).Enabled) && FillLevel(p) > 0.0);
            renewedEngines = true;
        }
        if (renewedBatteries && renewedReactors && renewedSolarPanels && renewedTurbines && renewedEngines) {
            renewingLists = false;
            renewListTime = DateTime.Now.AddSeconds(RecountDelay);
        }
        return false;
    }
    return true;
}

public bool IncludeBlock(IMyTerminalBlock block)
{
    return ((IMyFunctionalBlock)block).IsFunctional && (!sameGridOnly || includedGrids.Contains(block.CubeGrid) || block.CustomData.ToLower().Contains("crossgrid"));
}

public double FillLevel(IMyTerminalBlock block)
{
    string detailedInfo = block.DetailedInfo;
    detailedInfo = detailedInfo.Substring(detailedInfo.IndexOf("(") + 1);
    detailedInfo = detailedInfo.Substring(0, detailedInfo.IndexOf("L"));
    return double.Parse(detailedInfo);
}

public string RoundPower(float power)
{
    string powerString = "Negative Power";
    if (power >= 1000000000F) {
        powerString = (power / 1000000000F).ToString("N2") + "GWh";
    }
    else if (power >= 1000000F) {
        powerString = (power / 1000000F).ToString("N2") + "MWh";
    }
    else if (power >= 1000F) {
        powerString = (power / 1000F).ToString("N2") + "kWh";
    }
    else if (power >= 0F) {
        powerString = power.ToString("N0") + "Wh";
    }
    return powerString;
}

public void SizePanel(AutosizedPanel panel, Vector2 textSize)
{
    float tmpA = 0f, tmpB = 0f, tmpC = 2f;
    IMyTextSurface surface = panel.surface;
    IMyTerminalBlock block = panel.block;
    Vector2 pnlSize = surface.SurfaceSize;
    AdjustSize(ref pnlSize, ref tmpC, panel);
    tmpA = (pnlSize.X / textSize.X) * ((100f - (surface.TextPadding * 2f)) / 100F);
    tmpB = (pnlSize.Y / textSize.Y) * ((100f - (surface.TextPadding * tmpC)) / 100F);
    if (tmpA < tmpB && tmpA >= 0.1 && tmpA <= 10.0) surface.FontSize = tmpA;
    else if (tmpB >= 0.1 && tmpB <= 10.0) surface.FontSize = tmpB;
    else if (surface.FontSize < 0.1 || surface.FontSize > 10.0) surface.FontSize = 1f;
}

public void AdjustSize(ref Vector2 size, ref float spacing, AutosizedPanel panel)
{
    IMyTerminalBlock block = panel.block;
    string def = block.BlockDefinition.ToString();
    int index = panel.surfaceIndex;
    if (def.ToLower().Contains("blockcorner_lcd_")) {
        if (def.ToLower().Contains("small")) {
            if (def.ToLower().Contains("flat"))
                size.Y = size.Y * 0.31f;
            else size.Y = size.Y * 0.26f;
        }
        else {
            if (def.ToLower().Contains("flat"))
                size.Y = size.Y * 0.170745F;
            else size.Y = size.Y * 0.147193F;
        }
        spacing = 10f;
    }
    else switch (def)
    {
        case "MyObjectBuilder_Cockpit/LargeBlockCockpitSeat":
            size = new Vector2(size.X * 2f, size.Y * 2f);
        break;
        case "MyObjectBuilder_Cockpit/CockpitOpen":
            size = new Vector2(size.X * 4f, size.Y * 4f);
        break;
        case "MyObjectBuilder_MyProgrammableBlock/SmallProgrammableBlock":
            if (index == 0) size = new Vector2(size.X * 2f, size.Y * 2f);
            else if (index == 1) size = new Vector2(size.X * 4f, size.Y * 4f);
        break;
        case "MyObjectBuilder_MyProgrammableBlock/LargeProgrammableBlock":
            if (index == 1) size = new Vector2(size.X * 2f, size.Y * 2f);
        break;
        case "MyObjectBuilder_Cockpit/DBSmallBlockFighterCockpit":
            if (index == 0) size = new Vector2(size.X, size.Y * 4f);
            else if (index == 1 || index == 2 || index == 3 || index == 5) size = new Vector2(size.X * 4f, size.Y * 4f);
            else if (index == 4) size = new Vector2(size.X * 2f, size.Y * 4f);
        break;
        case "MyObjectBuilder_Cockpit/SmallBlockCockpit":
            size = new Vector2(size.X * 2f, size.Y * 2f);
        break;
    }
}

public class AutosizedPanel
{
    public IMyTerminalBlock block;
    public int surfaceIndex = 0;
    public IMyTextSurface surface;
}

public class GraphColumn
{
    public List<char> charList = new List<char>();
}