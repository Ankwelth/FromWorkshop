// R e a d m e
// -----------
// 
//       Solar Panel Alignment Script V4.16 By Adomus
// 
//       For T style Solar Panels
//       Requires Left Side Upper Panel for detection - used for Left hand panel up/down actuation (Tag TL)
//       Requires Left Side Lower Panel for detection - used for Left hand panel up/down actuation (Tag BL)
//       Requires Left Side Panel for detection - used for left/right tower actuation (Tag LL)
//       Requires Right Side Upper Panel for detection - used for Left hand panel up/down actuation (Tag TR)
//       Requires Right Side Lower Panel for detection  - used for Left hand panel up/down actuation (Tag BR)
//       Requires Right Side Panel for detection - used for left/right tower actuation (Tag RR)
// 
//       Rename control solar panels in the below, feedback given in programmable block on operations. Will align solar panels of any size in
//       main collector area. 
// 
//       Would recommend setting left/right rotor to be set to share inertial tensor to "ON" to prevent wobble and clang death.
//       Expand as needed to get into basic scripting.
// 
//       Should also now report damaged control blocks.
// 
//       Example Setup (Programmable Block Custom Data):
// 
//         [SolarArrayConfiguration]
//         HorizontalRotorTag=SLR-1-MRS-H
//         TowerGroupName=SLR-1-MRS
//         Display1NameTag=[SLR-1-MRS Solar D1]
//         Display1SurfaceIndex=0
//         Display2NameTag=[SLR-1-MRS Solar D2]
//         Display2SurfaceIndex=0
//         Display3NameTag=[SLR-1-MRS Solar D3]
//         Display3SurfaceIndex=0
//         RotorSpeed=0.1
//         RotorSpeedMax=0.5
//         RotorSpeedStop=0
//         Deadband=0.5
//         DeadbandMult=50 
//       Enjoy your green energy. 
// 
//       Have fun!
// 
//       Adomus
//       
// 
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

string ver = "V4.16";
float rotor_speed = 0.1f;
float rotor_speed_max = 2.0f;
float rotor_stop_speed = 0.0f;
float deadband = 0.5f;
float db_multiplier = 50.0f;

//Block identification tags - Configurables
string Tower_name = "SLR-1-MRS"; //Tower name tag
string Horizontal_rotor_tag_main = "SLR-1-MRS-H"; //Main Horizontal Rotor Tag

//Display Configurables
string lcd_display_name = "[SLR-1-MRS Solar D1]";
int lcd_display_index = 0; //used for devices with multiple screen panels (0+)               
string lcd_display_name_2 = "[SLR-1-MRS Solar D2]";
int lcd_display_index_2 = 0; //used for devices with multiple screen panels (0+)
string lcd_display_name_3 = "[SLR-1-MRS Solar D3]";
int lcd_display_index_3 = 0; //used for devices with multiple screen panels (0+)

// Detector Solar Panel Tags
string detector_left_tag = "LL";
string detector_left_top_tag = "TL";
string detector_left_bot_tag = "BL";
string detector_right_tag = "RR";
string detector_right_top_tag = "TR";
string detector_right_bot_tag = "BR";
//Solar Rotor Tags
string vert_rotor_right_tag = "VR";
string vert_rotor_left_tag = "VL";
string horiz_rotor_tag = "H";
//Solar Power Panel Bank Name Tag (main solar collector for left/right banks tags go here)
string solar_bank_tag_right = "R";
string solar_bank_tag_left = "L";

//end of configurables
string uppanel_tag_name_L = "";
string downpanel_tag_name_L = "";
string uppanel_tag_name_R = "";
string downpanel_tag_name_R = "";
string leftpanel_tag_name = "";
string rightpanel_tag_name = "";
string horiz_rotor_tag_name = "";
string vert_rotor_tag_name_r = "";
string vert_rotor_tag_name_l = "";
string tag_right = "";
string tag_left = "";
string towereadyStatus = "Not Ready";
float font_zoom = 0.60f;

int calc_state_val;
string horiz_state;
string vert_l_state;
string vert_r_state;
string calc_state_msg;

float total_power_left;
float total_power_right;
float total_maxP_a;
float total_maxP_b;
float total_power_s;
float perc_bank_l;
float perc_bank_r;
float Panelpower;
float Panelpower_a;
float Panelpower_b;
float perc_bank_a_r;
float perc_bank_b_r;
bool rotorlocken = false;
bool powercalcen = false;

IMySolarPanel panel_right, panel_left, panel_up, panel_down, panel_up_2, panel_down_2;

IMyMotorStator rotor_horiz, rotor_vertical, rotor_vertical_2;
List<IMySolarPanel> spanels = new List<IMySolarPanel>(), spanels_locator_up_l = new List<IMySolarPanel>(), spanels_locator_down_l = new List<IMySolarPanel>(), spanels_locator_up_r = new List<IMySolarPanel>(), spanels_locator_down_r = new List<IMySolarPanel>(), spanels_locator_ll = new List<IMySolarPanel>(), spanels_locator_rr = new List<IMySolarPanel>(), spanels_l = new List<IMySolarPanel>(), spanels_r = new List<IMySolarPanel>();
List<IMyMotorStator> rotors_all = new List<IMyMotorStator>(), rotors_vert_left = new List<IMyMotorStator>(), rotors_vert_right = new List<IMyMotorStator>(), rotors_horiz = new List<IMyMotorStator>();
List<IMyTerminalBlock> display_all = new List<IMyTerminalBlock>(), display_tag_main = new List<IMyTerminalBlock>(), display_tag_2 = new List<IMyTerminalBlock>(), display_tag_3 = new List<IMyTerminalBlock>();
List<IMyOxygenFarm> oxygen_farms_all = new List<IMyOxygenFarm>(), oxygen_farms_l = new List<IMyOxygenFarm>(), oxygen_farms_r = new List<IMyOxygenFarm>();
List<SolarArray> SolarArray_all = new List<SolarArray>();

StringBuilder sbtext = new StringBuilder();
bool setup_complete = false;
IEnumerator<bool> listCoroutine;
bool listgenerator_finished = true;
double percent_list_solar_l = 0.0;
double percent_list_solar_r = 0.0;
double percent_list_tower_all = 0.0;
double percent_list_solarcalc = 0.0;
bool stage_1_ok = false;
bool stage_2_ok = false;
bool stage_3_ok = false;
MyIni _iniStore = new MyIni();

IEnumerator<bool> listPanelCalculator;
bool listpanelcalculator_finished = true;
StringBuilder sbtexttemp = new StringBuilder();
IMyCubeGrid meCubeGrid;
List<IMyMotorStator> rotors_all_2 = new List<IMyMotorStator>();
List<IMyMotorAdvancedStator> rotorAdvancedStators_all = new List<IMyMotorAdvancedStator>();
List<IMyPistonBase> pistons_all = new List<IMyPistonBase>();
List<IMyTextSurface> myTextSurfaces_d1 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d2 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d3 = new List<IMyTextSurface>();
string d1_tag = "";
string d2_tag = "";
string d3_tag = "";
string rotor_tag_display = "";
int runTick = 0;
string display_uppanel_tag_name_L;
string display_uppanel_tag_name_R;
float abs_speed_diff = 0.0f;
float abs_val_diff = 0.0f;

public void Save()
{

}

void SaveData(IMyTerminalBlock block)
{
    _iniStore.Clear();
    _iniStore.TryParse(block.CustomData.ToString()); // Safe to parse empty or existing data

    _iniStore.Set("SolarArrayConfiguration", "HorizontalRotorTag", Horizontal_rotor_tag_main);
    _iniStore.Set("SolarArrayConfiguration", "TowerGroupName", Tower_name);
    _iniStore.Set("SolarArrayConfiguration", "Display1NameTag", lcd_display_name);
    _iniStore.Set("SolarArrayConfiguration", "Display1SurfaceIndex", lcd_display_index);
    _iniStore.Set("SolarArrayConfiguration", "Display2NameTag", lcd_display_name_2);
    _iniStore.Set("SolarArrayConfiguration", "Display2SurfaceIndex", lcd_display_index_2);
    _iniStore.Set("SolarArrayConfiguration", "Display3NameTag", lcd_display_name_3);
    _iniStore.Set("SolarArrayConfiguration", "Display3SurfaceIndex", lcd_display_index_3);
    _iniStore.Set("SolarArrayConfiguration", "RotorSpeed", rotor_speed);
    _iniStore.Set("SolarArrayConfiguration", "RotorSpeedMax", rotor_speed_max);
    _iniStore.Set("SolarArrayConfiguration", "RotorSpeedStop", rotor_stop_speed);
    _iniStore.Set("SolarArrayConfiguration", "Deadband", deadband);
    _iniStore.Set("SolarArrayConfiguration", "DeadbandMult", db_multiplier);
    _iniStore.Set("SolarArrayConfiguration", "rotorlocken", rotorlocken);
    _iniStore.Set("SolarArrayConfiguration", "powercalcen", powercalcen);
    block.CustomData = _iniStore.ToString();
    _iniStore.Clear();
}

void LoadStorageData(string input)
{
    if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
    {
        Echo("No Storage data found.");
        return;
    }
    _iniStore.Clear();
    if (_iniStore.TryParse(input))
    {
        var str = "";
        str = _iniStore.Get("SolarArrayConfiguration", "HorizontalRotorTag").ToString().Trim();
        if (!string.IsNullOrWhiteSpace(str)) Horizontal_rotor_tag_main = str;

        str = _iniStore.Get("SolarArrayConfiguration", "TowerGroupName").ToString().Trim();
        if (!string.IsNullOrWhiteSpace(str)) Tower_name = str;

        str = _iniStore.Get("SolarArrayConfiguration", "Display1NameTag").ToString().Trim();
        if (!string.IsNullOrWhiteSpace(str)) lcd_display_name = str;

        str = _iniStore.Get("SolarArrayConfiguration", "Display1SurfaceIndex").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out lcd_display_index)) lcd_display_index = 0;

        str = _iniStore.Get("SolarArrayConfiguration", "Display2NameTag").ToString().Trim();
        if (!string.IsNullOrWhiteSpace(str)) lcd_display_name_2 = str;

        str = _iniStore.Get("SolarArrayConfiguration", "Display2SurfaceIndex").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out lcd_display_index_2)) lcd_display_index_2 = 0;

        str = _iniStore.Get("SolarArrayConfiguration", "Display3NameTag").ToString().Trim();
        if (!string.IsNullOrWhiteSpace(str)) lcd_display_name_3 = str;

        str = _iniStore.Get("SolarArrayConfiguration", "Display3SurfaceIndex").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !int.TryParse(str, out lcd_display_index_3)) lcd_display_index_3 = 0;

        str = _iniStore.Get("SolarArrayConfiguration", "RotorSpeed").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out rotor_speed)) rotor_speed = 0.1f;

        str = _iniStore.Get("SolarArrayConfiguration", "RotorSpeedMax").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out rotor_speed_max)) rotor_speed_max = 0.5f;

        str = _iniStore.Get("SolarArrayConfiguration", "RotorSpeedStop").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out rotor_stop_speed)) rotor_stop_speed = 0.0f;

        str = _iniStore.Get("SolarArrayConfiguration", "Deadband").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out deadband)) deadband = 0.5f;

        str = _iniStore.Get("SolarArrayConfiguration", "DeadbandMult").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !float.TryParse(str, out db_multiplier)) db_multiplier = 10.0f;

        str = _iniStore.Get("SolarArrayConfiguration", "rotorlocken").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !bool.TryParse(str, out rotorlocken)) rotorlocken = false;
        str = _iniStore.Get("SolarArrayConfiguration", "powercalcen").ToString().Trim();
        if (string.IsNullOrWhiteSpace(str) || !bool.TryParse(str, out powercalcen)) powercalcen = false;
    }
}

public class SolarArray
{
    public IMySolarPanel locatorTL;
    public IMySolarPanel locatorBL;
    public IMySolarPanel locatorLL;
    public IMySolarPanel locatorTR;
    public IMySolarPanel locatorBR;
    public IMySolarPanel locatorRR;
    public IMyMotorStator rotorVL;
    public IMyMotorStator rotorVR;
    public IMyMotorStator rotorH;
    public List<IMySolarPanel> panelBankL;
    public List<IMySolarPanel> panelBankR;
    public List<IMyOxygenFarm> OxygenBankL;
    public List<IMyOxygenFarm> OxygenBankR;
    public int status_l;
    public int status_r;
    public int status_h;
    public float power_l;
    public float power_r;
    public float totalpower;
    public float efficiency;
    public int spanelscount_l;
    public int spanelscount_r;
    public int oxyfarmcount_l;
    public int oxyfarmcount_r;
    public bool isComplete;
    public int solarid;
}

public void Main(string argument, UpdateType updateSource)
{
    runTick++;
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        SaveData(Me);
    }

    IMyGridTerminalSystem gts = GridTerminalSystem;
    if (!setup_complete)
    {
        if (!string.IsNullOrEmpty(Me.CustomData) && !string.IsNullOrWhiteSpace(Me.CustomData))
        {
            LoadStorageData(Me.CustomData);
            abs_speed_diff = Math.Abs(rotor_speed_max - rotor_speed);
            abs_val_diff = Math.Abs((deadband * db_multiplier) - (deadband));
        }
        SetupSystem(gts);
        Echo("Setup complete!");
        SaveData(Me);
    }

    if (!setup_complete)
    {
        ClearAllNonEmptyLists();
        Echo("Setup incomplete.");
    }

    if (display_tag_main.Count <= 0 || ((IMyTextSurfaceProvider)display_tag_main[0]).GetSurface(lcd_display_index) == null)
    {
        sbtexttemp.Append("LCD display: '").Append(lcd_display_name).Append("' not found.");
    }

    sbtexttemp.AppendLine("======");
    sbtexttemp.Append("Adomus' Solar Alignment Script ").Append(ver).Append(" Running ").AppendLine(calc_state_msg);
    sbtexttemp.AppendLine();

    //Display to LCD 
    sbtext.Clear();
    sbtext.Append("Solar Array Status ").Append(ver).Append(": '").Append(Tower_name).Append("' - ").Append(calc_state_msg).Append("%\n");
    sbtext.AppendLine("==================================");
    sbtext.AppendLine();
    sbtext.Append("Alignment Status - Solar Array #: ").Append(SolarArray_all.Count).Append(" - ").Append(percent_list_solarcalc).AppendLine("%");
    sbtext.AppendLine("----------------------------------");

    if (SolarArray_all.Count <= 0)
    {
        sbtext.AppendLine("No solar arrays found.");
        sbtext.Append("Please add rotors with tag '").Append(Horizontal_rotor_tag_main).AppendLine("'");
    }

    sbtexttemp.Append("Solar Arrays #: ").Append(SolarArray_all.Count).Append(" - ").Append(percent_list_solarcalc).AppendLine("%");
    sbtext.AppendLine();
    sbtexttemp.AppendLine("---");
    sbtexttemp.AppendLine();
    sbtexttemp.Append("Horiz. Rotor Tag: ").Append(rotor_tag_display).AppendLine(" ");
    sbtexttemp.Append("Display Tag (").Append(myTextSurfaces_d1.Count).Append("): ").AppendLine(d1_tag);

    //coroutine list
    if (listgenerator_finished)
    {
        listgenerator_finished = false;
        stage_1_ok = false;
        stage_2_ok = false;
        stage_3_ok = false;
    }
    if (listCoroutine == null && !listgenerator_finished)
    {
        listCoroutine = Inventory_Scan();
    }
    if (listCoroutine != null && !listgenerator_finished)
    {
        bool currentYield = listCoroutine.Current;

        if (!listCoroutine.MoveNext())
        {
            sbtexttemp.AppendLine("Inventory list complete.");
            listCoroutine?.Dispose();
            listCoroutine = null;
        }
        else
        {
            if (!currentYield)
            {
                if (SolarArray_all.Count > 0)
                {
                    sbtexttemp.Append("\nUpdating solar panel tower list...").Append(Math.Round(percent_list_tower_all, 1)).Append("%");
                    if (spanels_l.Count > 0)
                    {
                        sbtexttemp.Append("\nUpdating solar panel left list...").Append(Math.Round(percent_list_solar_l, 1)).Append("%");
                    }
                    if (spanels_r.Count > 0)
                    {
                        sbtexttemp.Append("\nUpdating solar panel right list...").Append(Math.Round(percent_list_solar_r, 1)).Append("%");
                    }
                }
                listCoroutine.MoveNext();
            }
        }
    }
    sbtexttemp.AppendLine();

    if (listpanelcalculator_finished)
    {
        listpanelcalculator_finished = false;
    }
    if (listPanelCalculator == null && !listpanelcalculator_finished)
    {
        listPanelCalculator = SolarCalc();
    }
    if (listPanelCalculator != null && !listpanelcalculator_finished)
    {
        bool currentYield = listPanelCalculator.Current;
        if (!listPanelCalculator.MoveNext())
        {
            sbtexttemp.AppendLine("Solar calculations complete.");
            percent_list_solarcalc = 0.0;
            listPanelCalculator?.Dispose();
            listPanelCalculator = null;
        }
        else
        {
            if (!currentYield)
            {
                listPanelCalculator.MoveNext();
            }
        }
    }
    sbtexttemp.AppendLine();

    if (!setup_complete)
    {
        ClearAllNonEmptyLists();
        sbtexttemp.AppendLine("Setup incomplete.");
        return;
    }

    calc_state_val++;

    if (calc_state_val >= 4)
    {
        calc_state_val = 0;
    }

    if (calc_state_val == 0) calc_state_msg = ".---";
    if (calc_state_val == 1) calc_state_msg = "-.--";
    if (calc_state_val == 2) calc_state_msg = "--.-";
    if (calc_state_val == 3) calc_state_msg = "---.";

    sbtexttemp.AppendLine();

    if (listgenerator_finished && listpanelcalculator_finished)
    {
        WriteToSurfaces();
    }
    else if (listpanelcalculator_finished && !listgenerator_finished && SolarArray_all.Count <= 0)
    {
        WriteToSurfaces();
    }

    if (runTick % 2 == 0)
    {
        Echo(sbtexttemp.ToString());
    }
    sbtexttemp.Clear();
    if (runTick > 60)
    {
        runTick = 0;
    }
}

private void WriteToSurfaces()
{
    if (myTextSurfaces_d1.Count > 0)
    {
        for (int i = 0; i < myTextSurfaces_d1.Count; i++)
        {
            myTextSurfaces_d1[i]?.WriteText(sbtext);
        }
    }
    if (myTextSurfaces_d2.Count > 0)
    {
        for (int i = 0; i < myTextSurfaces_d2.Count; i++)
        {
            myTextSurfaces_d2[i]?.WriteText(sbtext);
        }
    }
    if (myTextSurfaces_d3.Count > 0)
    {
        for (int i = 0; i < myTextSurfaces_d3.Count; i++)
        {
            myTextSurfaces_d3[i]?.WriteText(sbtext);
        }
    }
}

private void Presence_Check(SolarArray Solararrayin, int m)
{
    if (Solararrayin.locatorTL == null)
    {
        sbtexttemp.Append("Vertical Detector Solar ").Append(m).Append(" Panel Upper T-Side Left with tag: '").Append(display_uppanel_tag_name_L).AppendLine("' not found.");
        setup_complete = false;
        return;
    }

    if (Solararrayin.locatorTR == null)
    {
        sbtexttemp.Append("Vertical Detector Solar ").Append(m).Append(" Panel Upper T-Side Right with tag: '").Append(display_uppanel_tag_name_R).AppendLine("' not found.");
        setup_complete = false;
        return;
    }

    if (Solararrayin.locatorBL == null)
    {
        sbtexttemp.Append("Vertical Detector Solar ").Append(m).Append(" Panel Lower T-Side Left with tag: '").Append(downpanel_tag_name_L.Replace("[", "[[").Replace("]", "]]")).AppendLine("' not found.");
        setup_complete = false;
        return;
    }

    if (Solararrayin.locatorBR == null)
    {
        sbtexttemp.Append("Vertical Detector Solar ").Append(m).Append(" Panel Lower T-Side Right with tag: '").Append(downpanel_tag_name_R.Replace("[", "[[").Replace("]", "]]")).AppendLine("' not found.");
        setup_complete = false;
        return;
    }
    if (Solararrayin.locatorLL == null)
    {
        sbtexttemp.Append("Horizontal Detector Solar ").Append(m).Append(" Panel T-Side Left with tag: '").Append(leftpanel_tag_name.Replace("[", "[[").Replace("]", "]]")).AppendLine("' not found.");
        setup_complete = false;
        return;
    }
    if (Solararrayin.locatorRR == null)
    {
        sbtexttemp.Append("Horizontal Detector Solar ").Append(m).Append(" Panel T-Side Right with tag: '").Append(rightpanel_tag_name.Replace("[", "[[").Replace("]", "]]")).AppendLine("' not found.");
        setup_complete = false;
        return;
    }

    if (Solararrayin.rotorH == null)
    {
        sbtexttemp.Append("Horizontal Direction Rotor ").Append(m).Append(" with tag: '").Append(horiz_rotor_tag_name.Replace("[", "[[").Replace("]", "]]")).AppendLine("' not found.");
        setup_complete = false;
        return;
    }
    if (Solararrayin.rotorVL == null)
    {
        sbtexttemp.Append("Vertical Left Direction Rotor ").Append(m).Append(" with tag: '").Append(vert_rotor_tag_name_l.Replace("[", "[[").Replace("]", "]]")).AppendLine("' not found.");
        setup_complete = false;
        return;
    }
    if (Solararrayin.rotorVR == null)
    {
        sbtexttemp.Append("Vertical Right Direction Rotor ").Append(m).Append(" with tag: '").Append(vert_rotor_tag_name_r.Replace("[", "[[").Replace("]", "]]")).AppendLine("' not found.");
        setup_complete = false;
        return;
    }
}

private void SetupSystem(IMyGridTerminalSystem gts)
{
    sbtext.Clear();
    ClearAllNonEmptyLists();

    uppanel_tag_name_L = Tower_name + " "+ detector_left_top_tag;
    uppanel_tag_name_R = Tower_name + " "+ detector_right_top_tag;
    downpanel_tag_name_L = Tower_name + " "+ detector_left_bot_tag;
    downpanel_tag_name_R = Tower_name + " "+ detector_right_bot_tag;
    leftpanel_tag_name = Tower_name + " "+ detector_left_tag;
    rightpanel_tag_name = Tower_name + " "+ detector_right_tag;
    horiz_rotor_tag_name = Tower_name + " "+ horiz_rotor_tag;
    vert_rotor_tag_name_r = Tower_name + " "+ vert_rotor_right_tag;
    vert_rotor_tag_name_l = Tower_name + " "+ vert_rotor_left_tag;
    tag_right = "Solar Panel "+ Tower_name + " "+ solar_bank_tag_right;
    tag_left = "Solar Panel "+ Tower_name + " "+ solar_bank_tag_left;

    display_uppanel_tag_name_L = uppanel_tag_name_L.Replace("[", "[[").Replace("]", "]]");
    display_uppanel_tag_name_R = uppanel_tag_name_R.Replace("[", "[[").Replace("]", "]]");

    if (!Me.CustomName.Contains(Tower_name))
    {
        Me.CustomName = Me.CustomName + " ["+ Tower_name + "]";
    }
    if (!Me.CustomName.Contains(horiz_rotor_tag_name))
    {
        Me.CustomName = Me.CustomName + " ["+ horiz_rotor_tag_name + "]";
    }

    bool blockfinder = false;
    IMyCubeGrid localMeGrid = Me.CubeGrid;

    gts.GetBlocksOfType<IMyMotorStator>(rotors_all_2, b => b.TopGrid == localMeGrid);

    if (rotors_all_2.Count <= 0) Echo("Rotor top grid not found, checking advanced rotors");
    else
    {
        meCubeGrid = rotors_all_2[0].CubeGrid;
        Echo("Local cubegrid found - rotor");
        blockfinder = true;
    }

    gts.GetBlocksOfType<IMyMotorAdvancedStator>(rotorAdvancedStators_all, b => b.TopGrid == localMeGrid);

    if (rotorAdvancedStators_all.Count <= 0) Echo("Rotor top grid not found, checking advanced rotors");
    else
    {
        meCubeGrid = rotorAdvancedStators_all[0].CubeGrid;
        Echo("Local cubegrid found - advanced rotor/hinge");
        blockfinder = true;
    }

    gts.GetBlocksOfType<IMyPistonBase>(pistons_all, b => b.TopGrid == localMeGrid);

    if (pistons_all.Count <= 0) Echo("Rotor top grid not found, checking pistons");
    else
    {
        meCubeGrid = pistons_all[0].CubeGrid;
        Echo("Local cubegrid found - piston");
        blockfinder = true;
    }

    if (rotorAdvancedStators_all.Count == 0 && rotors_all_2.Count == 0 && pistons_all.Count == 0)
    {
        meCubeGrid = localMeGrid;
        Echo("Local cubegrid found - PB");
        blockfinder = false;
    }

    rotors_all_2.Clear();
    rotorAdvancedStators_all.Clear();
    pistons_all.Clear();

    if (blockfinder)
    {
        gts.GetBlocksOfType<IMyMotorStator>(rotors_all, b => b.CubeGrid == localMeGrid);
        if (rotors_all.Count > 0)
        {
            for (int i = 0; i < rotors_all.Count; i++)
            {
                if (rotors_all[i].CustomName.Contains(horiz_rotor_tag_name) || rotors_all[i].CustomName.Contains(Horizontal_rotor_tag_main))
                {
                    Echo("Horizontal rotor found");
                    rotors_all[i].CustomName = "Solar Rotor Main "+ i + " ["+ Tower_name + "] ["+ horiz_rotor_tag_name + "]";
                    rotors_horiz.Add(rotors_all[i]);
                    SolarArray_all.Add(new SolarArray
                    {
                        rotorH = rotors_all[i],
                        panelBankL = new List<IMySolarPanel>(),
                        panelBankR = new List<IMySolarPanel>(),
                        OxygenBankL = new List<IMyOxygenFarm>(),
                        OxygenBankR = new List<IMyOxygenFarm>(),
                        solarid = i,
                        isComplete = false
                    });
                }
            }
        }
        rotors_all.Clear();
    }

    IMyCubeGrid meGridTarget = meCubeGrid;
    gts.GetBlocksOfType<IMyMotorStator>(rotors_all, b => b.CubeGrid == meGridTarget);
    if (rotors_all.Count > 0)
    {
        for (int i = 0; i < rotors_all.Count; i++)
        {
            if (rotors_all[i].CustomName.Contains(horiz_rotor_tag_name) || rotors_all[i].CustomName.Contains(Horizontal_rotor_tag_main))
            {
                Echo("Horizontal rotor found");
                rotors_all[i].CustomName = "Solar Rotor Main "+ i + " ["+ Tower_name + "] ["+ horiz_rotor_tag_name + "]";
                rotors_horiz.Add(rotors_all[i]);
                SolarArray_all.Add(new SolarArray
                {
                    rotorH = rotors_all[i],
                    panelBankL = new List<IMySolarPanel>(),
                    panelBankR = new List<IMySolarPanel>(),
                    OxygenBankL = new List<IMyOxygenFarm>(),
                    OxygenBankR = new List<IMyOxygenFarm>(),
                    solarid = i,
                    isComplete = false
                });
            }
        }
    }
    rotors_all.Clear();

    //Iterate SolarArray_all to find vertical all horizontal rotors for processing vertical rotors
    if (SolarArray_all.Count > 0)
    {
        for (int s = 0; s < SolarArray_all.Count; s++)
        {
            var currentArray = SolarArray_all[s];

            if (currentArray.rotorH != null && currentArray.rotorH.Top != null)
            {
                currentArray.isComplete = false;
                Echo("Scanning horizontal rotor attachments");

                IMyCubeGrid topGrid = currentArray.rotorH.TopGrid;
                gts.GetBlocksOfType<IMyMotorStator>(rotors_all, b => b.CubeGrid == topGrid);
                if (rotors_all.Count > 0)
                {
                    for (int i = 0; i < rotors_all.Count; i++)
                    {
                        if (rotors_all[i].CustomName.Contains(vert_rotor_left_tag))
                        {
                            rotors_all[i].CustomName = "Solar Rotor "+ currentArray.solarid + " ["+ Tower_name + "] ["+ vert_rotor_tag_name_l + "]";
                        }
                        if (rotors_all[i].CustomName.Contains(vert_rotor_tag_name_l))
                        {
                            rotors_vert_left.Add(rotors_all[i]);
                            currentArray.rotorVL = rotors_all[i];
                        }
                        if (rotors_all[i].CustomName.Contains(vert_rotor_right_tag))
                        {
                            rotors_all[i].CustomName = "Solar Rotor "+ currentArray.solarid + " ["+ Tower_name + "] ["+ vert_rotor_tag_name_r + "]";
                        }
                        if (rotors_all[i].CustomName.Contains(vert_rotor_tag_name_r))
                        {
                            rotors_vert_right.Add(rotors_all[i]);
                            currentArray.rotorVR = rotors_all[i];
                        }
                    }
                }
                rotors_all.Clear();
            }

            //Find Vertical Panel Banks Left
            if (currentArray.rotorVL != null && currentArray.rotorVL.Top != null)
            {
                Echo("Vertical left rotor found");
                IMyCubeGrid leftTopGrid = currentArray.rotorVL.TopGrid;
                gts.GetBlocksOfType<IMySolarPanel>(spanels, b => b.CubeGrid == leftTopGrid);
                if (spanels.Count > 0)
                {
                    for (int i = 0; i < spanels.Count; i++)
                    {
                        if (spanels[i].CustomName.Contains(detector_left_top_tag))
                        {
                            Echo("Detector panel left top tag found");
                            spanels[i].CustomName = "Solar Panel Detector "+ currentArray.solarid + " ["+ Tower_name + "] ["+ uppanel_tag_name_L + "]";
                        }
                        if (spanels[i].CustomName.Contains(uppanel_tag_name_L))
                        {
                            spanels_locator_up_l.Add(spanels[i]);
                            currentArray.locatorTL = spanels[i];
                            Echo("Left bank up detector panel found");
                        }
                        if (spanels[i].CustomName.Contains(detector_left_bot_tag))
                        {
                            Echo("Detector panel left bottom tag found");
                            spanels[i].CustomName = "Solar Panel Detector "+ currentArray.solarid + " ["+ Tower_name + "] ["+ downpanel_tag_name_L + "]";
                        }
                        if (spanels[i].CustomName.Contains(downpanel_tag_name_L))
                        {
                            spanels_locator_down_l.Add(spanels[i]);
                            currentArray.locatorBL = spanels[i];
                            Echo("Left bank down detector panel found");
                        }
                        if (spanels[i].CustomName.Contains(detector_left_tag))
                        {
                            Echo("Detector panel left tag found");
                            spanels[i].CustomName = "Solar Panel Detector "+ currentArray.solarid + " ["+ Tower_name + "] ["+ leftpanel_tag_name + "]";
                        }
                        if (spanels[i].CustomName.Contains(leftpanel_tag_name))
                        {
                            spanels_locator_ll.Add(spanels[i]);
                            currentArray.locatorLL = spanels[i];
                            Echo("Left horizontal detector panel found");
                        }
                    }
                }
            }
            spanels.Clear();

            //Find Vertical Panel Banks Right
            if (currentArray.rotorH != null && currentArray.rotorVR != null && currentArray.rotorVR.Top != null)
            {
                Echo("Vertical right rotor found");
                IMyCubeGrid rightTopGrid = currentArray.rotorVR.TopGrid;
                gts.GetBlocksOfType<IMySolarPanel>(spanels, b => b.CubeGrid == rightTopGrid);
                if (spanels.Count > 0)
                {
                    for (int i = 0; i < spanels.Count; i++)
                    {
                        if (spanels[i].CustomName.Contains(detector_right_top_tag))
                        {
                            Echo("Detector panel right top tag found");
                            spanels[i].CustomName = "Solar Panel Detector "+ currentArray.solarid + " ["+ Tower_name + "] ["+ uppanel_tag_name_R + "]";
                        }
                        if (spanels[i].CustomName.Contains(uppanel_tag_name_R))
                        {
                            spanels_locator_up_r.Add(spanels[i]);
                            currentArray.locatorTR = spanels[i];
                            Echo("Right bank up detector panel found");
                        }
                        if (spanels[i].CustomName.Contains(detector_right_bot_tag))
                        {
                            Echo("Detector panel right bottom tag found");
                            spanels[i].CustomName = "Solar Panel Detector "+ currentArray.solarid + " ["+ Tower_name + "] ["+ downpanel_tag_name_R + "]";
                        }
                        if (spanels[i].CustomName.Contains(downpanel_tag_name_R))
                        {
                            spanels_locator_down_r.Add(spanels[i]);
                            currentArray.locatorBR = spanels[i];
                            Echo("Right bank down detector panel found");
                        }
                        if (spanels[i].CustomName.Contains(detector_right_tag))
                        {
                            Echo("Detector panel right tag found");
                            spanels[i].CustomName = "Solar Panel Detector "+ currentArray.solarid + " ["+ Tower_name + "] ["+ rightpanel_tag_name + "]";
                        }
                        if (spanels[i].CustomName.Contains(rightpanel_tag_name))
                        {
                            spanels_locator_rr.Add(spanels[i]);
                            currentArray.locatorRR = spanels[i];
                            Echo("Right horizontal detector panel found");
                        }
                    }
                }
            }
            spanels.Clear();

            //find bank panels left
            if (currentArray.rotorVL != null && currentArray.rotorVL.Top != null)
            {
                IMyCubeGrid leftTopGrid = currentArray.rotorVL.TopGrid;
                gts.GetBlocksOfType<IMySolarPanel>(spanels, b => b.CubeGrid == leftTopGrid);
                if (spanels.Count > 0)
                {
                    for (int i = 0; i < spanels.Count; i++)
                    {
                        if (spanels[i] != null)
                        {
                            if (!spanels[i].CustomName.Contains(uppanel_tag_name_L) && !spanels[i].CustomName.Contains(downpanel_tag_name_L) && !spanels[i].CustomName.Contains(leftpanel_tag_name))
                            {
                                spanels[i].CustomName = tag_left + " "+ currentArray.solarid + " "+ (i + 1);
                            }
                            spanels_l.Add(spanels[i]);
                        }
                    }
                }
                currentArray.panelBankL = spanels_l;

                IMyCubeGrid oxygenTopGridLeft = rotors_vert_left[0].TopGrid;
                gts.GetBlocksOfType<IMyOxygenFarm>(oxygen_farms_all, b => b.CubeGrid == oxygenTopGridLeft);
                if (oxygen_farms_all.Count > 0)
                {
                    for (int i = 0; i < oxygen_farms_all.Count; i++)
                    {
                        if (oxygen_farms_all[i] != null)
                        {
                            oxygen_farms_all[i].CustomName = "Oxygen Farm ["+ Tower_name + "] "+ currentArray.solarid + " Left Bank "+ (i + 1);
                            oxygen_farms_l.Add(oxygen_farms_all[i]);
                        }
                    }
                }
            }
            currentArray.OxygenBankL = oxygen_farms_l;

            if (currentArray.panelBankL.Count > 0) Echo("Left bank panels found");
            if (currentArray.OxygenBankL.Count > 0) Echo("Left bank oxygen farms");

            spanels.Clear();
            oxygen_farms_all.Clear();

            //find bank panels right
            if (currentArray.rotorVR != null && currentArray.rotorVR.Top != null)
            {
                IMyCubeGrid rightTopGrid = currentArray.rotorVR.TopGrid;
                gts.GetBlocksOfType<IMySolarPanel>(spanels, b => b.CubeGrid == rightTopGrid);
                if (spanels.Count > 0)
                {
                    for (int i = 0; i < spanels.Count; i++)
                    {
                        if (spanels[i] != null)
                        {
                            if (!spanels[i].CustomName.Contains(uppanel_tag_name_R) && !spanels[i].CustomName.Contains(downpanel_tag_name_R) && !spanels[i].CustomName.Contains(rightpanel_tag_name))
                            {
                                spanels[i].CustomName = tag_right + " "+ currentArray.solarid + " "+ (i + 1);
                            }
                            spanels_r.Add(spanels[i]);
                        }
                    }
                }
                currentArray.panelBankR = spanels_r;

                IMyCubeGrid oxygenTopGridRight = rotors_vert_right[0].TopGrid;
                gts.GetBlocksOfType<IMyOxygenFarm>(oxygen_farms_all, b => b.CubeGrid == oxygenTopGridRight);
                if (oxygen_farms_all.Count > 0)
                {
                    for (int i = 0; i < oxygen_farms_all.Count; i++)
                    {
                        if (oxygen_farms_all[i] != null)
                        {
                            oxygen_farms_all[i].CustomName = "Oxygen Farm ["+ Tower_name + "] "+ currentArray.solarid + " Right Bank "+ (i + 1);
                            oxygen_farms_r.Add(oxygen_farms_all[i]);
                        }
                    }
                }
                currentArray.OxygenBankR = oxygen_farms_r;
            }
            if (currentArray.panelBankR.Count > 0) Echo("Right bank panels found");
            if (currentArray.OxygenBankR.Count > 0) Echo("Right bank oxygen farms");

            spanels.Clear();
            oxygen_farms_all.Clear();

            //Solar Array Completion Check
            if (currentArray.rotorVR != null && currentArray.rotorVL != null && currentArray.rotorH != null &&
                currentArray.locatorTL != null && currentArray.locatorTR != null &&
                currentArray.locatorBL != null && currentArray.locatorBR != null &&
                currentArray.locatorLL != null && currentArray.locatorRR != null)
            {
                currentArray.isComplete = true;
            }
            else
            {
                currentArray.isComplete = false;
            }
        }
    }

    //find displays            
    myTextSurfaces_d1.Clear();
    myTextSurfaces_d2.Clear();
    myTextSurfaces_d3.Clear();

    if (blockfinder)
    {
        gts.GetBlocksOfType<IMyTerminalBlock>(display_all, b => b.CubeGrid == localMeGrid);
        if (display_all.Count > 0)
        {
            for (int i = 0; i < display_all.Count; i++)
            {
                if (display_all[i].CustomName.Contains(lcd_display_name))
                {
                    display_tag_main.Add(display_all[i]);
                    myTextSurfaces_d1.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index));
                }
                if (display_all[i].CustomName.Contains(lcd_display_name_2))
                {
                    display_tag_2.Add(display_all[i]);
                    myTextSurfaces_d2.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index_2));
                }
                if (display_all[i].CustomName.Contains(lcd_display_name_3))
                {
                    display_tag_3.Add(display_all[i]);
                    myTextSurfaces_d3.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index_3));
                }
            }
        }
        display_all.Clear();
    }

    gts.GetBlocksOfType<IMyTerminalBlock>(display_all, b => b.CubeGrid == meGridTarget);
    if (display_all.Count > 0)
    {
        for (int i = 0; i < display_all.Count; i++)
        {
            if (display_all[i].CustomName.Contains(lcd_display_name))
            {
                display_tag_main.Add(display_all[i]);
                myTextSurfaces_d1.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index));
            }
            if (display_all[i].CustomName.Contains(lcd_display_name_2))
            {
                display_tag_2.Add(display_all[i]);
                myTextSurfaces_d2.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index_2));
            }
            if (display_all[i].CustomName.Contains(lcd_display_name_3))
            {
                display_tag_3.Add(display_all[i]);
                myTextSurfaces_d3.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index_3));
            }
        }
    }
    display_all.Clear();

    if (myTextSurfaces_d1.Count > 0)
    {
        for (int i = 0; i < myTextSurfaces_d1.Count; i++)
        {
            if (myTextSurfaces_d1[i] != null && myTextSurfaces_d1[i].ContentType != ContentType.TEXT_AND_IMAGE)
            {
                myTextSurfaces_d1[i].ContentType = ContentType.TEXT_AND_IMAGE;
                myTextSurfaces_d1[i].Alignment = TextAlignment.LEFT;
                myTextSurfaces_d1[i].FontSize = font_zoom;
            }
        }
    }
    if (myTextSurfaces_d2.Count > 0)
    {
        for (int i = 0; i < myTextSurfaces_d2.Count; i++)
        {
            if (myTextSurfaces_d2[i] != null && myTextSurfaces_d2[i].ContentType != ContentType.TEXT_AND_IMAGE)
            {
                myTextSurfaces_d2[i].ContentType = ContentType.TEXT_AND_IMAGE;
                myTextSurfaces_d2[i].Alignment = TextAlignment.LEFT;
                myTextSurfaces_d2[i].FontSize = font_zoom;
            }
        }
    }
    if (myTextSurfaces_d3.Count > 0)
    {
        for (int i = 0; i < myTextSurfaces_d3.Count; i++)
        {
            if (myTextSurfaces_d3[i] != null && myTextSurfaces_d3[i].ContentType != ContentType.TEXT_AND_IMAGE)
            {
                myTextSurfaces_d3[i].ContentType = ContentType.TEXT_AND_IMAGE;
                myTextSurfaces_d3[i].Alignment = TextAlignment.LEFT;
                myTextSurfaces_d3[i].FontSize = font_zoom;
            }
        }
    }

    d1_tag = lcd_display_name.Replace("[", "[[").Replace("]", "]]");
    d2_tag = lcd_display_name_2.Replace("[", "[[").Replace("]", "]]");
    d3_tag = lcd_display_name_3.Replace("[", "[[").Replace("]", "]]");
    rotor_tag_display = Horizontal_rotor_tag_main.Replace("[", "[[").Replace("]", "]]");
    setup_complete = true;
    Echo("Setup executed");
}

IEnumerator<bool> Inventory_Scan()
{
    if (powercalcen)
    {
        int batchcount = 0;
        int batchcountlim = 4;

        if (SolarArray_all.Count > 0)
        {
            percent_list_tower_all = 0.0;
            for (int r = 0; r < SolarArray_all.Count; r++)
            {
                if (SolarArray_all[r].isComplete)
                {
                    var currentArray = SolarArray_all[r];
                    spanels_l = currentArray.panelBankL;
                    spanels_r = currentArray.panelBankR;
                    oxygen_farms_l = currentArray.OxygenBankL;
                    oxygen_farms_r = currentArray.OxygenBankR;

                    total_power_left = 0;
                    total_power_right = 0;
                    total_maxP_a = 0;
                    total_maxP_b = 0;
                    total_power_s = 0;
                    Panelpower = 160f / 1000f;
                    percent_list_solar_l = 0.0;
                    percent_list_solar_r = 0.0;

                    Panelpower_a = (spanels_l.Count * Panelpower) * 1000f;
                    Panelpower_b = (spanels_r.Count * Panelpower) * 1000f;

                    if (spanels_l.Count > 0)
                    {
                        for (int j = 0; j < spanels_l.Count; j++)
                        {
                            if (spanels_l[j] != null)
                            {
                                total_power_left += (spanels_l[j].CurrentOutput * 1000f);
                                total_maxP_a += (spanels_l[j].MaxOutput * 1000f);
                                perc_bank_l = (total_power_left / total_maxP_a) * 100f;
                                perc_bank_a_r = (total_power_left / Panelpower_a) * 100f;
                            }

                            percent_list_solar_l = ((double)j / (double)(spanels_l.Count - 1)) * 100;
                            if (spanels_l.Count == 1) percent_list_solar_l = 100.0;

                            batchcount++;
                            if (batchcount >= batchcountlim)
                            {
                                batchcount = 0;
                                yield return false;
                            }
                        }
                        stage_1_ok = true;
                    }
                    if (spanels_r.Count > 0)
                    {
                        for (int k = 0; k < spanels_r.Count; k++)
                        {
                            if (spanels_r[k] != null)
                            {
                                total_power_right += (spanels_r[k].CurrentOutput * 1000f);
                                total_maxP_b += (spanels_r[k].MaxOutput * 1000f);
                                perc_bank_r = (total_power_right / total_maxP_b) * 100f;
                                perc_bank_b_r = (total_power_right / Panelpower_b) * 100f;
                            }

                            percent_list_solar_r = ((double)k / (double)(spanels_r.Count - 1)) * 100;
                            if (spanels_r.Count == 1) percent_list_solar_r = 100.0;

                            batchcount++;
                            if (batchcount >= batchcountlim)
                            {
                                batchcount = 0;
                                yield return false;
                            }
                        }
                        stage_2_ok = true;
                    }

                    if (spanels_l.Count <= 0) stage_1_ok = true;
                    if (spanels_r.Count <= 0) stage_2_ok = true;

                    if (stage_1_ok && stage_2_ok)
                    {
                        total_power_s = (total_power_left + total_power_right) / 1000f;
                    }

                    currentArray.totalpower = total_power_s;
                    currentArray.power_l = total_power_left / 1000f;
                    currentArray.power_r = total_power_right / 1000f;
                    currentArray.oxyfarmcount_l = oxygen_farms_l.Count;
                    currentArray.oxyfarmcount_r = oxygen_farms_r.Count;
                    currentArray.spanelscount_l = spanels_l.Count;
                    currentArray.spanelscount_r = spanels_r.Count;
                }

                if (SolarArray_all.Count <= 0) stage_3_ok = true;
                if (stage_3_ok)
                {
                    percent_list_tower_all = ((double)r / (double)(SolarArray_all.Count - 1)) * 100;
                }

                if (SolarArray_all.Count == 1) percent_list_tower_all = 100.0;
                yield return false;
            }
            stage_3_ok = true;
            listgenerator_finished = true;
        }
    }
    else
    {
        percent_list_solar_l = 0;
        percent_list_solar_r = 0;
        percent_list_tower_all = 100.0;
        stage_1_ok = true;
        stage_2_ok = true;
        stage_3_ok = true;
        listgenerator_finished = true;
    }
        yield return true;
}

IEnumerator<bool> SolarCalc()
{
    if (SolarArray_all.Count > 0)
    {
        for (int m = 0; m < SolarArray_all.Count; m++)
        {
            var currentArray = SolarArray_all[m];
            Presence_Check(currentArray, m);

            if (currentArray.rotorH != null && currentArray.isComplete)
            {
                panel_up = currentArray.locatorTL;
                panel_down = currentArray.locatorBL;
                panel_up_2 = currentArray.locatorTR;
                panel_down_2 = currentArray.locatorBR;
                panel_left = currentArray.locatorLL;
                panel_right = currentArray.locatorRR;
                rotor_vertical = currentArray.rotorVL;
                rotor_vertical_2 = currentArray.rotorVR;
                rotor_horiz = currentArray.rotorH;

                int v1_res = Align(currentArray.rotorVL, currentArray.locatorTL.CurrentOutput, currentArray.locatorBL.CurrentOutput, -1f);
                int v2_res = Align(currentArray.rotorVR, currentArray.locatorTR.CurrentOutput, currentArray.locatorBR.CurrentOutput, 1f);
                int h_res = Align(currentArray.rotorH, currentArray.locatorRR.CurrentOutput, currentArray.locatorLL.CurrentOutput, 1f);

                vert_l_state = GetStateChar(v1_res, "D", "U");
                vert_r_state = GetStateChar(v2_res, "D", "U");
                horiz_state = GetStateChar(h_res, "L", "R");
            }

            if (!rotor_horiz.IsFunctional) sbtexttemp.Append(rotor_horiz.CustomName).AppendLine(" is damaged");
            if (!rotor_vertical.IsFunctional) sbtexttemp.Append(rotor_vertical.CustomName).AppendLine(" is damaged");
            if (!rotor_vertical_2.IsFunctional) sbtexttemp.Append(rotor_vertical_2.CustomName).AppendLine(" is damaged");
            if (!panel_up.IsFunctional) sbtexttemp.Append(panel_up.CustomName).AppendLine(" is damaged");
            if (!panel_up_2.IsFunctional) sbtexttemp.Append(panel_up_2.CustomName).AppendLine(" is damaged");
            if (!panel_down.IsFunctional) sbtexttemp.Append(panel_down.CustomName).AppendLine(" is damaged");
            if (!panel_down_2.IsFunctional) sbtexttemp.Append(panel_down_2.CustomName).AppendLine(" is damaged");
            if (!panel_left.IsFunctional) sbtexttemp.Append(panel_left.CustomName).AppendLine(" is damaged");
            if (!panel_right.IsFunctional) sbtexttemp.Append(panel_right.CustomName).AppendLine(" is damaged");

            towereadyStatus = currentArray.isComplete ? "Ready": "Not Ready";

            percent_list_solarcalc = Math.Round(((double)(m + 1) / (double)(SolarArray_all.Count)) * 100.0, 1);
            if (SolarArray_all.Count == 1) percent_list_solarcalc = 100.0;

            sbtexttemp.Append("Solar Array ").Append(m).Append(" Status: ").Append(vert_l_state).Append("-").Append(horiz_state).Append("-").Append(vert_r_state).Append("  : ").Append(towereadyStatus).Append(" : (").Append(percent_list_solarcalc).AppendLine("%)");
            yield return false;

            sbtext.Append("\nSolar Array ").Append(m).Append(" Status: ").Append(vert_l_state).Append("-").Append(horiz_state).Append("-").Append(vert_r_state).Append(" : ").Append(towereadyStatus).Append(" : (").Append(percent_list_solarcalc).Append("%)\n");
            yield return false;
        }
    }
    else
    {
        sbtexttemp.Append("No solar arrays found. Please add rotors with tag '").Append(Horizontal_rotor_tag_main).AppendLine("'");
    }
    listpanelcalculator_finished = true;
    yield return true;
}

public void ClearAllNonEmptyLists()
{
    if (spanels != null) spanels.Clear();
    if (spanels_locator_up_l != null) spanels_locator_up_l.Clear();
    if (spanels_locator_down_l != null) spanels_locator_down_l.Clear();
    if (spanels_locator_up_r != null) spanels_locator_up_r.Clear();
    if (spanels_locator_down_r != null) spanels_locator_down_r.Clear();
    if (spanels_locator_ll != null) spanels_locator_ll.Clear();
    if (spanels_locator_rr != null) spanels_locator_rr.Clear();
    if (spanels_l != null) spanels_l.Clear();
    if (spanels_r != null) spanels_r.Clear();
    if (rotors_all != null) rotors_all.Clear();
    if (rotors_vert_left != null) rotors_vert_left.Clear();
    if (rotors_vert_right != null) rotors_vert_right.Clear();
    if (rotors_horiz != null) rotors_horiz.Clear();
    if (display_all != null) display_all.Clear();
    if (display_tag_main != null) display_tag_main.Clear();
    if (display_tag_2 != null) display_tag_2.Clear();
    if (display_tag_3 != null) display_tag_3.Clear();
    if (oxygen_farms_all != null) oxygen_farms_all.Clear();
    if (oxygen_farms_l != null) oxygen_farms_l.Clear();
    if (oxygen_farms_r != null) oxygen_farms_r.Clear();
    if (SolarArray_all != null) SolarArray_all.Clear();
}

private float ProportionalOutput(float min_speed, float panel_1, float panel_2, float min_differential, float max_differential)
{
    float differential_val = Math.Abs(panel_1 - panel_2);
    float working_differential = differential_val < min_differential ? min_differential : (differential_val > max_differential ? max_differential : differential_val);
    float xaxis_ratio = abs_val_diff > 0f ? working_differential / abs_val_diff : 0.0f;
    return (xaxis_ratio * abs_speed_diff) + min_speed;
}

int Align(IMyMotorStator rotor, float p1, float p2, float dir)
{
    if (rotor == null) return 0;

    float out1 = p1 * 100.0f;
    float out2 = p2 * 100.0f;

    if (Math.Abs(out1 - out2) <= deadband)
    {
        if (Math.Abs(rotor.TargetVelocityRPM - rotor_stop_speed) > 0.01f)
        {
            rotor.TargetVelocityRPM = rotor_stop_speed;
        }
        if (rotor.TargetVelocityRPM == rotor_stop_speed && !rotor.RotorLock && rotorlocken)
        {
            rotor.RotorLock = true;
        }
        else if (rotor.RotorLock && !rotorlocken)
        {
            rotor.RotorLock = false;
        }
        return 3;
    }

    float calculatedValue = ProportionalOutput(rotor_speed, out1, out2, deadband, deadband * db_multiplier);
    float targetVelocity = calculatedValue * dir;

    if (out1 < out2 - deadband)
    {
        if (rotor.RotorLock) rotor.RotorLock = false;
        if (Math.Abs(rotor.TargetVelocityRPM - targetVelocity) > 0.01f)
        {
            rotor.TargetVelocityRPM = targetVelocity;
        }
        return 1;
    }
    else
    {
        float targetVelocityOpposite = calculatedValue * -dir;
        if (rotor.RotorLock) rotor.RotorLock = false;

        if (Math.Abs(rotor.TargetVelocityRPM - targetVelocityOpposite) > 0.01f)
        {
            rotor.TargetVelocityRPM = targetVelocityOpposite;
        }
        return 2;
    }
}

string GetStateChar(int res, string c1, string c2)
{
    if (res == 1) return c1;
    if (res == 2) return c2;
    if (res == 3) return "|";
    return "-";
}
