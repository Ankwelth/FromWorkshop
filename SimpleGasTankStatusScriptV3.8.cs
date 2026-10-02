// R e a d m e
// -----------
// 
// V3.8
// simple tank display script with indictator light and display with second tank display option - enable vars and change nametags as needed
// have fun
// Adomus o7o7o7
// start


public Program()
{            
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    ManageFirstLoad(Me);
}

public void Save()
{

}

void ManageFirstLoad(IMyTerminalBlock block)
{
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        WriteStorageData(block);
        LoadStorageData(block.CustomData);
    }
    else
    {
        LoadStorageData(block.CustomData);
    }
}

void LoadStorageData(string input)
{
    _Ini.Clear();
    if (_Ini.TryParse(input))
    {
        var str = "";
        str = _Ini.Get("TankConfig", "enable2ndtanktype").ToString();
        if (!bool.TryParse(str, out En_2nd_Tank_Type))
        {
            En_2nd_Tank_Type = true;
        }
        str = _Ini.Get("TankConfig", "remoteinterfacespresent").ToString();
        if (!bool.TryParse(str, out Remote_Interfaces))
        {
            Remote_Interfaces = false;
        }
        str = _Ini.Get("TankConfig", "enablelights").ToString();
        if (!bool.TryParse(str, out En_lights))
        {
            En_lights = false;
        }

        str = _Ini.Get("TankConfig", "tankname").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            tank_name = "HTR";
        }
        else
        {
            tank_name = str;
        }
            str = _Ini.Get("TankConfig", "tankindicator").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            tank_indicator = "HTR";
        }
        else
        {
            tank_indicator = str;
        }
        str = _Ini.Get("TankConfig", "displaytankname").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            display_tank_name = "HTR";
        }
        else
        {
            display_tank_name = str;
        }
        str = _Ini.Get("TankConfig", "tankname2nd").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            tank_name_2nd = "HTR";
        }
        else
        {
            tank_name_2nd = str;
        }
        str = _Ini.Get("TankConfig", "tankindicator2nd").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            tank_indicator_2nd = "OTR";
        }
        else
        {
            tank_indicator_2nd = str;
        }
        str = _Ini.Get("TankConfig", "displaytankname2nd").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            display_tank_name_2nd = "OTR";
        }
        else
        {
            display_tank_name_2nd = str;
        }
        str = _Ini.Get("TankConfig", "shipname").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            Ship_name = "Mars Base";
        }
        else
        {
            Ship_name = str;
        }
        str = _Ini.Get("TankConfig", "lcd1tag").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            lcd_display_name = "[Mars Base Tanks D1]";
        }
        else
        {
            lcd_display_name = str;
        }
        str = _Ini.Get("TankConfig", "lcd1surfaceindex").ToString();
        if (!int.TryParse(str, out lcd_display_index))
        {
            lcd_display_index = 0;
        }
        str = _Ini.Get("TankConfig", "lcd2tag").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            lcd_display_name_2 = "[Mars Base Tanks D2]";
        }
        else
        {
            lcd_display_name_2 = str;
        }
        str = _Ini.Get("TankConfig", "lcd2surfaceindex").ToString();
        if (!int.TryParse(str, out lcd_display_index_2))
        {
            lcd_display_index_2 = 0;
        }
        str = _Ini.Get("TankConfig", "lcd3tag").ToString();
        if (string.IsNullOrWhiteSpace(str))
        {
            lcd_display_name_3 = "[Mars Base Tanks D2]";
        }
        else
        {
            lcd_display_name_3 = str;
        }
        str = _Ini.Get("TankConfig", "lcd3surfaceindex").ToString();
        if (!int.TryParse(str, out lcd_display_index_3))
        {
            lcd_display_index_3 = 0;
        }
    }
        _Ini.Clear();
    sbtexttemp.AppendLine("Data loaded from pb customdata");
}
void WriteStorageData(IMyTerminalBlock block)
{
    _Ini.Clear();
    if (_Ini.TryParse(block.CustomData.ToString()))
    {
        _Ini.Set("TankConfig", "enable2ndtanktype", En_2nd_Tank_Type);
        _Ini.Set("TankConfig", "remoteinterfacespresent", Remote_Interfaces);
        _Ini.Set("TankConfig", "enablelights", En_lights);
        _Ini.Set("TankConfig", "tankname", tank_name);
        _Ini.Set("TankConfig", "tankindicator", tank_indicator);
        _Ini.Set("TankConfig", "displaytankname", display_tank_name);
        _Ini.Set("TankConfig", "tankname2nd", tank_name_2nd);
        _Ini.Set("TankConfig", "tankindicator2nd", tank_indicator_2nd);
        _Ini.Set("TankConfig", "displaytankname2nd", display_tank_name_2nd);
        _Ini.Set("TankConfig", "shipname", Ship_name);
        _Ini.Set("TankConfig", "lcd1tag", lcd_display_name);
        _Ini.Set("TankConfig", "lcd1surfaceindex", lcd_display_index);
        _Ini.Set("TankConfig", "lcd2tag", lcd_display_name_2);
        _Ini.Set("TankConfig", "lcd2surfaceindex", lcd_display_index_2);
        _Ini.Set("TankConfig", "lcd3tag", lcd_display_name_3);
        _Ini.Set("TankConfig", "lcd3surfaceindex", lcd_display_index_3);
    }
    else
    {
        _Ini.Set("TankConfig", "enable2ndtanktype", En_2nd_Tank_Type);
        _Ini.Set("TankConfig", "remoteinterfacespresent", Remote_Interfaces);
        _Ini.Set("TankConfig", "enablelights", En_lights);
        _Ini.Set("TankConfig", "tankname", tank_name);
        _Ini.Set("TankConfig", "tankindicator", tank_indicator);
        _Ini.Set("TankConfig", "displaytankname", display_tank_name);
        _Ini.Set("TankConfig", "tankname2nd", tank_name_2nd);
        _Ini.Set("TankConfig", "tankindicator2nd", tank_indicator_2nd);
        _Ini.Set("TankConfig", "displaytankname2nd", display_tank_name_2nd);
        _Ini.Set("TankConfig", "shipname", Ship_name);
        _Ini.Set("TankConfig", "lcd1tag", lcd_display_name);
        _Ini.Set("TankConfig", "lcd1surfaceindex", lcd_display_index);
        _Ini.Set("TankConfig", "lcd2tag", lcd_display_name_2);
        _Ini.Set("TankConfig", "lcd2surfaceindex", lcd_display_index_2);
        _Ini.Set("TankConfig", "lcd3tag", lcd_display_name_3);
        _Ini.Set("TankConfig", "lcd3surfaceindex", lcd_display_index_3);
    }
        block.CustomData = _Ini.ToString();
    _Ini.Clear();
    sbtexttemp.AppendLine("Data saved to pb customdata");
}



bool En_2nd_Tank_Type = true; // enable oxygen tank
bool Remote_Interfaces = false;
bool En_lights = true;
        
//tank set 1 names
string tank_name = "HTR";
string tank_indicator = "HTR";
string display_tank_name = "Hydrogen";
//tank set 2 names   
string tank_name_2nd = "OTR";
string tank_indicator_2nd = "OTR";
string display_tank_name_2nd = "Oxygen";

//display settings
string lcd_display_name = "[Resourcer Tanks D1]";
int lcd_display_index = 0; //used for devices with multiple screen panels (0+)               
string lcd_display_name_2 = "[Resourcer Tanks D2]";
int lcd_display_index_2 = 0; //used for devices with multiple screen panels (0+)
string lcd_display_name_3 = "[Resourcer Tanks D3]";
int lcd_display_index_3 = 0; //used for devices with multiple screen panels (0+)        
        
// display zoom setting
float display_zoom = 1.203f;
string Ship_name = "Resourcer";

//indictator light ranges tank set 1
double tank_high = 75.0;
double tank_low = 25.0;
double tank_danger = 10.0;
//indictator light ranges tank set 2
double tank_high_2nd = 75.0;
double tank_low_2nd = 25.0;
double tank_danger_2nd = 10.0;
    
//math stuff
double t_stored_gas;
double stored_gas_total;
double t_max_gas;
double max_gas_total;

double t_stored_gas_2nd;
double stored_gas_total_2nd;
double t_max_gas_2nd;
double max_gas_total_2nd;
double percent_gas_tank_2nd = 0.0;
double percent_gas_tank = 0.0;
string ver = "V3.8";
bool setup_complete = false;

IMyGasTank currenttank;
IMyGasTank currenttank_2nd;
IMyLightingBlock indicatorlight_actual;
IMyLightingBlock indicatorlight_actual_2nd;

List<IMyGasTank> tank_all = new List<IMyGasTank>();
List<IMyGasTank> tank_tag = new List<IMyGasTank>();
List<IMyGasTank> tank_tag_2nd = new List<IMyGasTank>();
List<IMyLightingBlock> lighting_all = new List<IMyLightingBlock>();
List<IMyLightingBlock> lighting_indicator_tag = new List<IMyLightingBlock>();
List<IMyLightingBlock> lighting_indicator_tag_2nd = new List<IMyLightingBlock>();
StringBuilder sbtext = new StringBuilder();
List<IMyTerminalBlock> display_all = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> display_tag_main = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> display_tag_2 = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> display_tag_3 = new List<IMyTerminalBlock>();

List<IMyMotorStator> rotors_all = new List<IMyMotorStator>();
List<IMyMotorAdvancedStator> rotorAdvancedStators_all = new List<IMyMotorAdvancedStator>();
List<IMyPistonBase> pistons_all = new List<IMyPistonBase>();

List<IMyTextSurface> myTextSurfaces_d1 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d2 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d3 = new List<IMyTextSurface>();
IMyCubeGrid meCubeGrid;

Color Colorone = new Color(0, 255, 0);
Color Colortwo = new Color(255, 255, 0);
Color Colorthree = new Color(255, 0, 0);
bool stage_1_ok = false;
bool stage_2_ok = false;
bool listgenerator_finished = true;
double percent_list_tank_1 = 0.0;
double percent_list_tank_2 = 0.0;
        
List<string> barg = new List<string>();
List<string> bargraph = new List<string>();
MyIni _Ini = new MyIni();


private IEnumerator<bool> listCoroutine;
string spinner = "";
int state = 0;
string shipnamevar = "";
string displaynamevar = "";
string tanknamevar = "";
string displaynamevar2 = "";
string tanknamevar2 = "";
StringBuilder sbtexttemp = new StringBuilder();
void Setup_System()
{
    IMyGridTerminalSystem gts = GridTerminalSystem as IMyGridTerminalSystem;

    display_all.Clear();
    display_tag_main.Clear();
    display_tag_2.Clear();
    display_tag_3.Clear();
    myTextSurfaces_d1.Clear();
    myTextSurfaces_d2.Clear();
    myTextSurfaces_d3.Clear();
    rotors_all.Clear();
    rotorAdvancedStators_all.Clear();
    pistons_all.Clear();
    sbtext.Clear();
    tank_all.Clear();
    tank_tag.Clear();
    tank_tag_2nd.Clear();
    bargraph.Clear();
    barg.Clear();
    bargraph.Add("");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");
    barg.Add("-");

    bool blockfinder = false;
    gts.GetBlocksOfType<IMyMotorStator>(rotors_all, b => b.TopGrid == Me.CubeGrid);

    if (rotors_all.Count <= 0)
    {
        Echo("Rotor top grid not found, checking advanced rotors");
    }

    if (rotors_all.Count > 0)
    {
        if (rotors_all[0] != null)
        {
            meCubeGrid = rotors_all[0].CubeGrid;
            Echo("Local cubegrid found - rotor");
            blockfinder = true;
        }
    }

    gts.GetBlocksOfType<IMyMotorAdvancedStator>(rotorAdvancedStators_all, b => b.TopGrid == Me.CubeGrid);

    if (rotorAdvancedStators_all.Count <= 0)
    {
        Echo("Rotor top grid not found, checking advanced rotors");
    }
    if (rotorAdvancedStators_all.Count > 0)
    {
        if (rotorAdvancedStators_all[0] != null)
        {
            meCubeGrid = rotorAdvancedStators_all[0].CubeGrid;
            Echo("Local cubegrid found - advanced rotor/hinge");
            blockfinder = true;
        }
    }


    gts.GetBlocksOfType<IMyPistonBase>(pistons_all, b => b.TopGrid == Me.CubeGrid);

    if (pistons_all.Count <= 0)
    {
        Echo("Rotor top grid not found, checking pistons");
    }
    if (pistons_all.Count > 0)
    {
        if (pistons_all[0] != null)
        {

            meCubeGrid = pistons_all[0].CubeGrid;
            Echo("Local cubegrid found - piston");
            blockfinder = true;
        }
    }


    if (rotorAdvancedStators_all.Count == 0 && rotors_all.Count == 0 && pistons_all.Count == 0)
    {
        meCubeGrid = Me.CubeGrid;
        Echo("Local cubegrid found - PB");
        blockfinder = false;
    }

    rotors_all.Clear();
    rotorAdvancedStators_all.Clear();
    pistons_all.Clear();


    if (Remote_Interfaces)
    {
        gts.GetBlocksOfType<IMyGasTank>(tank_all);
    }
    if (!Remote_Interfaces)
    {
        gts.GetBlocksOfType<IMyGasTank>(tank_all, b => b.CubeGrid == meCubeGrid);
    }
    if (tank_all.Count > 0)
    {
        for (int i = 0; i < tank_all.Count; i++)
        {
            //create new array from search array with tanks matching tag                       
            if (tank_all[i].BlockDefinition.SubtypeId.Contains("HydrogenTank"))
            {
                tank_tag.Add(tank_all[i]);
            }
            if (!(tank_all[i].BlockDefinition.SubtypeId.Contains("HydrogenTank")))
            {
                tank_tag_2nd.Add(tank_all[i]);
            }
        }
    }
    tank_all.Clear();

    if (blockfinder)
    {
        if (!Remote_Interfaces)
        {
            gts.GetBlocksOfType<IMyGasTank>(tank_all, b => b.CubeGrid == Me.CubeGrid);
        }
        if (tank_all.Count > 0)
        {
            for (int i = 0; i < tank_all.Count; i++)
            {
                //create new array from search array with tanks matching tag                       
                if (tank_all[i].BlockDefinition.SubtypeId.Contains("HydrogenTank"))
                {
                    tank_tag.Add(tank_all[i]);
                }
                if (!(tank_all[i].BlockDefinition.SubtypeId.Contains("HydrogenTank")))
                {
                    tank_tag_2nd.Add(tank_all[i]);
                }
            }
        }
        tank_all.Clear();
    }

    if (tank_tag.Count > 0)
    {
        for (int i = 0; i < tank_tag.Count; i++)
        {
            tank_tag[i].CustomName = $"Hydrogen Tank {i + 1} [{Ship_name}]";
        }
    }
    if (tank_tag_2nd.Count > 0)
    {
        for (int i = 0; i < tank_tag_2nd.Count; i++)
        {
            tank_tag_2nd[i].CustomName = $"Oxygen Tank {i + 1} [{Ship_name}]";
        }
    }
    //used to feedback docked state

    lighting_all = new List<IMyLightingBlock>();
    lighting_indicator_tag = new List<IMyLightingBlock>();
    lighting_indicator_tag_2nd = new List<IMyLightingBlock>();
    if (Remote_Interfaces && En_lights)
    {
        gts.GetBlocksOfType<IMyLightingBlock>(lighting_all);
    }
    if (!Remote_Interfaces && En_lights)
    {
        gts.GetBlocksOfType<IMyLightingBlock>(lighting_all, b => b.CubeGrid == meCubeGrid);
    }

    if (lighting_all.Count > 0 && En_lights)
    {
        for (int i = 0; i < lighting_all.Count; i++)
        {
            if (lighting_all[i] != null)
            {
                //create new array from search array with lights matching tag
                if (lighting_all[i].CustomName.Contains(tank_indicator) == true)
                {
                    lighting_indicator_tag.Add(lighting_all[i]);
                }
                //create new array from search array with lights matching tag
                if (lighting_all[i].CustomName.Contains(tank_indicator_2nd) == true)
                {
                    lighting_indicator_tag_2nd.Add(lighting_all[i]);
                }
            }
        }
    }
    lighting_all.Clear();


    if (blockfinder)
    {
        if (!Remote_Interfaces && En_lights)
        {
            gts.GetBlocksOfType<IMyLightingBlock>(lighting_all, b => b.CubeGrid == Me.CubeGrid);
        }

        if (lighting_all.Count > 0 && En_lights)
        {
            for (int i = 0; i < lighting_all.Count; i++)
            {
                if (lighting_all[i] != null)
                {
                    //create new array from search array with lights matching tag
                    if (lighting_all[i].CustomName.Contains(tank_indicator) == true)
                    {
                        lighting_indicator_tag.Add(lighting_all[i]);
                    }
                    //create new array from search array with lights matching tag
                    if (lighting_all[i].CustomName.Contains(tank_indicator_2nd) == true)
                    {
                        lighting_indicator_tag_2nd.Add(lighting_all[i]);
                    }
                }
            }
        }
        lighting_all.Clear();
    }



    if (blockfinder)
    {

        gts.GetBlocksOfType<IMyTerminalBlock>(display_all, b => b.CubeGrid == Me.CubeGrid);
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

    gts.GetBlocksOfType<IMyTerminalBlock>(display_all, b => b.CubeGrid == meCubeGrid);
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

    //find batteries, end if not found
    if (lighting_indicator_tag.Count <= 0 && En_lights)
        {
            Echo($"No lights found with tag {tank_name.Replace("[", "[[").Replace("]", "]]")}");

        }
        if (lighting_indicator_tag_2nd.Count <= 0 && En_lights)
        {
            Echo($"No lights found with tag {tank_name_2nd.Replace("[", "[[").Replace("]", "]]")}");
        }
        //find lights, end if not found
        if (lighting_indicator_tag.Count <= 0 && En_lights)
        {
            Echo($"Tank indicator light with tag: '{tank_indicator.Replace("[", "[[").Replace("]", "]]")}' not found.");
            //return;
        }
        if (lighting_indicator_tag.Count > 0)
        {
            if (En_lights && lighting_indicator_tag[0] != null)
            {
                indicatorlight_actual = lighting_indicator_tag[0];
            }
        }
        if (En_2nd_Tank_Type)
        {
            //find lights, end if not found
            if (lighting_indicator_tag_2nd.Count <= 0 && En_lights)
            {
                Echo($"Tank indicator light with tag: '{tank_indicator_2nd.Replace("[", "[[").Replace("]", "]]")}' not found.");
                //return;
            }
            if (lighting_indicator_tag_2nd.Count > 0)
            {
                if (En_lights && lighting_indicator_tag_2nd[0] != null)
                {
                    indicatorlight_actual_2nd = lighting_indicator_tag_2nd[0];
                }
            }
        }

        if (myTextSurfaces_d1.Count <= 0)
        {
            Echo($"Displays with tag {lcd_display_name.Replace("[", "[[").Replace("]","]]")} not found.");
            return;
        }
        if (myTextSurfaces_d1.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d1.Count; i++)
            {
                if (myTextSurfaces_d1[i] != null)
                {
                    if (myTextSurfaces_d1[i].ContentType != ContentType.TEXT_AND_IMAGE)
                    {
                        myTextSurfaces_d1[i].ContentType = ContentType.TEXT_AND_IMAGE;
                        myTextSurfaces_d1[i].Alignment = TextAlignment.CENTER;
                        myTextSurfaces_d1[i].FontSize = display_zoom;
                    }
                }
            }
        }
        if (myTextSurfaces_d2.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d2.Count; i++)
            {
                if (myTextSurfaces_d2[i] != null)
                {
                    if (myTextSurfaces_d2[i].ContentType != ContentType.TEXT_AND_IMAGE)
                    {
                        myTextSurfaces_d2[i].ContentType = ContentType.TEXT_AND_IMAGE;
                        myTextSurfaces_d2[i].Alignment = TextAlignment.CENTER;
                        myTextSurfaces_d2[i].FontSize = display_zoom;
                    }
                }
            }
        }
        if (myTextSurfaces_d3.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d3.Count; i++)
            {
                if (myTextSurfaces_d3[i] != null)
                {
                    if (myTextSurfaces_d3[i].ContentType != ContentType.TEXT_AND_IMAGE)
                    {
                        myTextSurfaces_d3[i].ContentType = ContentType.TEXT_AND_IMAGE;
                        myTextSurfaces_d3[i].Alignment = TextAlignment.CENTER;
                        myTextSurfaces_d3[i].FontSize = display_zoom;
                    }
                }
            }
        }

        shipnamevar = $"{Ship_name.Replace("[", "[[").Replace("]", "]]")}";
        displaynamevar = $"{display_tank_name.Replace("[", "[[").Replace("]", "]]")}";
        tanknamevar = $"{tank_name.Replace("[", "[[").Replace("]", "]]")}";
        displaynamevar2 = $"{display_tank_name_2nd.Replace("[", "[[").Replace("]", "]]")}";
        tanknamevar2 = $"{tank_name_2nd.Replace("[", "[[").Replace("]", "]]")}";
        Echo("Setup complete!");
        setup_complete = true;
    }       

public void Main(string argument, UpdateType updateSource)
{
    if (!setup_complete)
    {
        LoadStorageData(Me.CustomData);
        Setup_System();                
    }

    //populate array with batteries on grid(s) 
    if (argument.Contains("cfg:one") == true)
    {
        //   En_2nd_Tank_Type = false;
        sbtexttemp.AppendLine("Single tank type enabled!");
    }

    if (argument.Contains("cfg:two") == true)
    {
        //  En_2nd_Tank_Type = true;
        sbtexttemp.AppendLine("Second tank type enabled!");
    }

    //spinner control
    if (state > 3)
    {
        state = 0;
    }
    if (state == 0)
    {
        spinner = ".---";
    }
    if (state == 1)
    {
        spinner = "-.--";
    }
    if (state == 2)
    {
        spinner = "--.-";
    }
    if (state == 3)
    {
        spinner = "---.";
    }


    //find batteries, end if not found
    if (tank_tag.Count <= 0)
    {

        Echo($"No tanks found with tag {tank_name.Replace("[", "[[").Replace("]", "]]")}");
        return;
    }

    //find batteries, end if not found
    if (tank_tag_2nd.Count <= 0 && En_2nd_Tank_Type == true)
    {

        Echo($"No tanks found with tag {tank_name_2nd.Replace("[", "[[").Replace("]", "]]")}");
        return;
    }

    if (myTextSurfaces_d1.Count <= 0)
    {

        Echo($"Display with tag '{lcd_display_name.Replace("[", "[[").Replace("]", "]]")}' not found");
        return;
    }

            
    //logic start

    sbtexttemp.AppendLine($"Adomus' tank status {ver} Running {spinner}");
    if (En_lights && indicatorlight_actual != null && lighting_indicator_tag.Count > 0)
    {
        if (percent_gas_tank >= tank_high)
        {
            indicatorlight_actual.SetValue("Color", Colorone);
            indicatorlight_actual.BlinkIntervalSeconds = 0.0f;
            indicatorlight_actual.BlinkLength = 100.0f;
        }

        if (percent_gas_tank < tank_high && percent_gas_tank > tank_low)
        {
            indicatorlight_actual.SetValue("Color", Colortwo);
            indicatorlight_actual.BlinkIntervalSeconds = 0.0f;
            indicatorlight_actual.BlinkLength = 100.0f;
        }


        if (percent_gas_tank <= tank_low)
        {
            indicatorlight_actual.SetValue("Color", Colorthree);
            indicatorlight_actual.BlinkIntervalSeconds = 0.0f;
            indicatorlight_actual.BlinkLength = 100.0f;
        }


        if (percent_gas_tank <= tank_danger)
        {
            indicatorlight_actual.SetValue("Color", Colorthree);
            indicatorlight_actual.BlinkIntervalSeconds = 2.0f;
            indicatorlight_actual.BlinkLength = 50.0f;
        }
    }

    if (En_2nd_Tank_Type)
    {
        if (En_lights && indicatorlight_actual_2nd != null && lighting_indicator_tag_2nd.Count > 0)
        {
            if (percent_gas_tank_2nd >= tank_high_2nd)
            {
                indicatorlight_actual_2nd.SetValue("Color", Colorone);
                indicatorlight_actual_2nd.BlinkIntervalSeconds = 0.0f;
                indicatorlight_actual_2nd.BlinkLength = 100.0f;
            }

            if (percent_gas_tank_2nd < tank_high_2nd && percent_gas_tank_2nd > tank_low_2nd)
            {
                indicatorlight_actual_2nd.SetValue("Color", Colortwo);
                indicatorlight_actual_2nd.BlinkIntervalSeconds = 0.0f;
                indicatorlight_actual_2nd.BlinkLength = 100.0f;
            }


            if (percent_gas_tank_2nd <= tank_low_2nd)
            {
                indicatorlight_actual_2nd.SetValue("Color", Colorthree);
                indicatorlight_actual_2nd.BlinkIntervalSeconds = 0.0f;
                indicatorlight_actual_2nd.BlinkLength = 100.0f;
            }


            if (percent_gas_tank_2nd <= tank_danger_2nd)
            {
                indicatorlight_actual_2nd.SetValue("Color", Colorthree);
                indicatorlight_actual_2nd.BlinkIntervalSeconds = 2.0f;
                indicatorlight_actual_2nd.BlinkLength = 50.0f;
            }
        }
    }

    //coroutine list
    if (listgenerator_finished)
    {
        listgenerator_finished = false;
        stage_1_ok = false;
        stage_2_ok = false;
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
            sbtexttemp.Append("Gas inventory list complete.").Append('\n');
            listCoroutine?.Dispose();
            listCoroutine = null;
        }
        else if (!currentYield) // Simplified the nested else/if
        {
            if (tank_tag.Count > 0)
            {
                sbtexttemp.Append("Updating gas tank type 1 list... ").Append(Math.Round(percent_list_tank_1, 1)).Append("%").Append('\n');
            }
            if (tank_tag_2nd.Count > 0)
            {
                sbtexttemp.Append("Updating gas tank type 2 list... ").Append(Math.Round(percent_list_tank_2, 1)).Append("%").Append('\n');
            }
            listCoroutine.MoveNext();
        }
    }

    sbtext.Clear();
    sbtext.Append("--- [").Append(Ship_name).Append("] Gas Tank Status ---").Append('\n');
    sbtext.Append("============================").Append('\n');
    sbtext.Append('\n');
    sbtext.Append(display_tank_name).Append(" Status [").Append(Math.Round(percent_gas_tank, 2)).Append("%]").Append('\n');
    sbtext.Append('\n');

    barmaker((float)Math.Round(percent_gas_tank / 100, 2), 0);
    sbtext.Append(bargraph[0]).Append('\n');

    if (En_2nd_Tank_Type) // Removed unnecessary '== true'
    {
        sbtext.Append('\n');
        sbtext.Append(display_tank_name_2nd).Append(" Status [").Append(Math.Round(percent_gas_tank_2nd, 2)).Append("%]").Append('\n');
        sbtext.Append('\n');

        barmaker((float)Math.Round(percent_gas_tank_2nd / 100, 2), 0);
        sbtext.Append(bargraph[0]).Append('\n');
        sbtext.Append('\n');
    }

    if (listgenerator_finished)
    {
        if (myTextSurfaces_d1.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d1.Count; i++)
            {
                if (myTextSurfaces_d1[i] != null)
                {
                    myTextSurfaces_d1[i].WriteText(sbtext);
                }
            }
        }
        else // Replaced 'if (display_surface_1 == null)'
        {
            sbtexttemp.Append("Display missing").Append('\n');
        }
        if (myTextSurfaces_d2.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d2.Count; i++)
            {
                if (myTextSurfaces_d2[i] != null)
                {
                    myTextSurfaces_d2[i].WriteText(sbtext);
                }
            }
        }
        if (myTextSurfaces_d3.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d3.Count; i++)
            {
                if (myTextSurfaces_d3[i] != null)
                {
                    myTextSurfaces_d3[i].WriteText(sbtext);
                }
            }
        }

        sbtexttemp.Append("====== [[").Append(shipnamevar).Append("]] Gas Tank Status ======").Append('\n');
        sbtexttemp.Append(displaynamevar).Append("  Tank Status").Append('\n');
        sbtexttemp.Append(tanknamevar).Append(" Tank # ").Append(tank_tag.Count).Append('\n');
        sbtexttemp.Append("Capacity: ").Append(Math.Round(percent_gas_tank, 2)).Append(" %").Append('\n');

        if (En_2nd_Tank_Type)
        {
            sbtexttemp.Append(displaynamevar2).Append("  Tank Status").Append('\n');
            sbtexttemp.Append(tanknamevar2).Append(" Tank # ").Append(tank_tag_2nd.Count).Append('\n');
            sbtexttemp.Append("Capacity: ").Append(Math.Round(percent_gas_tank_2nd, 2)).Append(" %").Append('\n');
        }
    }
    else // Replaced 'if (!listgenerator_finished)'
    {
        sbtexttemp.Append("====== [[").Append(shipnamevar).Append("]] Gas Tank Status ======").Append('\n');
        sbtexttemp.Append(displaynamevar).Append("  Tank Status").Append('\n');
        sbtexttemp.Append(tanknamevar).Append(" Tank # ").Append(tank_tag.Count).Append('\n');
        sbtexttemp.Append("Capacity: ").Append(spinner).Append(" %").Append('\n');

        if (En_2nd_Tank_Type)
        {
            sbtexttemp.Append(displaynamevar2).Append("  Tank Status").Append('\n');
            sbtexttemp.Append(tanknamevar2).Append(" Tank # ").Append(tank_tag_2nd.Count).Append('\n');
            sbtexttemp.Append("Capacity: ").Append(spinner).Append(" %").Append('\n');
        }
    }

    Echo(sbtexttemp.ToString());
    sbtexttemp.Clear();
    state++;
}
IEnumerator<bool> Inventory_Scan()
{
    IMyGridTerminalSystem gts = GridTerminalSystem;
    int batchcount = 0;
    int batchcountlimit = 4;
    //reset power totals for array addition
    t_stored_gas = 0;
    stored_gas_total = 0;
    t_max_gas = 0;
    max_gas_total = 0;
    percent_gas_tank = 0.0;
    percent_list_tank_1 = 0.0;
    percent_list_tank_2 = 0.0;
    if (tank_tag.Count > 0)
    {
        for (int i = 0; i < tank_tag.Count; i++)
        {
            if (tank_tag[i] != null)
            {
                if (tank_tag[i].IsFunctional)
                {

                    currenttank = tank_tag[i];
                    //record stored and max battery capacity
                    t_stored_gas = currenttank.FilledRatio * 100.0f;
                    stored_gas_total = stored_gas_total + t_stored_gas;
                    t_max_gas = 100.0f;
                    max_gas_total = max_gas_total + t_max_gas;

                    //calculate storage capacity percent
                    percent_gas_tank = (stored_gas_total / max_gas_total) * 100.0f;
                    //sbtexttemp.AppendLine($"Tank Name: {tank_tag[i].CustomName}, Subtype: {tank_tag[i].BlockDefinition}");
                }
            }
            percent_list_tank_1 = ((double)i / (double)(tank_tag.Count - 1)) * 100;
            if (tank_tag.Count == 1)
            {
                percent_list_tank_1 = 100.0;
            }
            batchcount++;
            if (batchcount >= batchcountlimit)
            {
                batchcount = 0;
                yield return false;
            }
        }
        stage_1_ok = true;
    }

    if (En_2nd_Tank_Type)
    {
        //reset power totals for array addition
        t_stored_gas_2nd = 0;
        stored_gas_total_2nd = 0;
        t_max_gas_2nd = 0;
        max_gas_total_2nd = 0;

        if (tank_tag_2nd.Count > 0)
        {
            for (int i = 0; i < tank_tag_2nd.Count; i++)
            {
                if (tank_tag_2nd[i] != null)
                {
                    if (tank_tag_2nd[i].IsFunctional)
                    {
                        currenttank_2nd = tank_tag_2nd[i];
                        //record stored and max battery capacity
                        t_stored_gas_2nd = currenttank_2nd.FilledRatio * 100.0f;
                        stored_gas_total_2nd = stored_gas_total_2nd + t_stored_gas_2nd;
                        t_max_gas_2nd = 100.0f;
                        max_gas_total_2nd = max_gas_total_2nd + t_max_gas_2nd;

                        //calculate storage capacity percent
                        percent_gas_tank_2nd = (stored_gas_total_2nd / max_gas_total_2nd) * 100.0f;
                        //sbtexttemp.AppendLine($"Tank Name: {tank_tag_2nd[i].CustomName}, Subtype: {tank_tag_2nd[i].BlockDefinition}");
                    }
                }
                percent_list_tank_2 = ((double)i / (double)(tank_tag_2nd.Count - 1)) * 100;
                if (tank_tag_2nd.Count == 1)
                {
                    percent_list_tank_2 = 100.0;
                }
                batchcount++;
                if (batchcount >= batchcountlimit)
                {
                    batchcount = 0;
                    yield return false;
                }
            }
            stage_2_ok = true;
        }
    }
    if (tank_tag.Count <= 0)
    {
        stage_1_ok = true;
    }
    if (tank_tag_2nd.Count <= 0)
    {
        stage_2_ok = true;
    }
    if (stage_1_ok && stage_2_ok)
    {
        listgenerator_finished = true;
    }
    yield return true;
}

void barmaker(float val_in, int indexer)
{
    StringBuilder sb = new StringBuilder();
    float perc = (val_in * 100);
    float pwr_per_b = 100 / 20;
    float pwr_rel = perc / pwr_per_b;
    if (pwr_rel <= 0)
    {
        for (int i = 0; i < barg.Count; i++)
        {
            barg[i] = "-";
        }
    }
    if (pwr_rel > 0)
    {
        barg[0] = "|";
    }
    else
    {
        barg[0] = "-";
    }
    if (pwr_rel > 1)
    {
        barg[1] = "|";
    }
    else
    {
        barg[1] = "-";
    }
    if (pwr_rel > 2)
    {
        barg[2] = "|";
    }
    else
    {
        barg[2] = "-";
    }
    if (pwr_rel > 3)
    {
        barg[3] = "|";
    }
    else
    {
        barg[3] = "-";
    }
    if (pwr_rel > 4)
    {
        barg[4] = "|";
    }
    else
    {
        barg[4] = "-";
    }
    if (pwr_rel > 5)
    {
        barg[5] = "|";
    }
    else
    {
        barg[5] = "-";
    }
    if (pwr_rel > 6)
    {
        barg[6] = "|";
    }
    else
    {
        barg[6] = "-";
    }
    if (pwr_rel > 7)
    {
        barg[7] = "|";
    }
    else
    {
        barg[7] = "-";
    }
    if (pwr_rel > 8)
    {
        barg[8] = "|";
    }
    else
    {
        barg[8] = "-";
    }
    if (pwr_rel > 9)
    {
        barg[9] = "|";
    }
    else
    {
        barg[9] = "-";
    }
    if (pwr_rel > 10)
    {
        barg[10] = "|";
    }
    else
    {
        barg[10] = "-";
    }
    if (pwr_rel > 11)
    {
        barg[11] = "|";
    }
    else
    {
        barg[11] = "-";
    }
    if (pwr_rel > 12)
    {
        barg[12] = "|";
    }
    else
    {
        barg[12] = "-";
    }
    if (pwr_rel > 13)
    {
        barg[13] = "|";
    }
    else
    {
        barg[13] = "-";
    }
    if (pwr_rel > 14)
    {
        barg[14] = "|";
    }
    else
    {
        barg[14] = "-";
    }
    if (pwr_rel > 15)
    {
        barg[15] = "|";
    }
    else
    {
        barg[15] = "-";
    }
    if (pwr_rel > 16)
    {
        barg[16] = "|";
    }
    else
    {
        barg[16] = "-";
    }
    if (pwr_rel > 17)
    {
        barg[17] = "|";
    }
    else
    {
        barg[17] = "-";
    }
    if (pwr_rel > 18)
    {
        barg[18] = "|";
    }
    else
    {
        barg[18] = "-";
    }
    if (pwr_rel > 19)
    {
        barg[19] = "|";
    }
    else
    {
        barg[19] = "-";
    }
    sb.Append("[");
    for (int j = 0; j < barg.Count; j++)
    {
        sb.Append(barg[j]);
    }
    sb.Append("]");
    bargraph[indexer] = sb.ToString();
}
// end

