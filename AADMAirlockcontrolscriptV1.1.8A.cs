// R e a d m e
// -----------
// 
// AADM - Adomus's air lock door management V1.1.8
// 
// Airlock door management system for easy management of airlock doors
// 
// Add arguments to the programmable block of your airlock group e.g.
// 
// PB Run Argument Format: 
// 	Group Argument,Command Argument1,Command Argument2
// 
// 
//  Example Run argument: [BaseAirlocks],doorlimit=2,batchlimit=8
//  
//  Command Arguments:
// 	Group Argument: [BaseAirlocks] - The name of the airlock door group to manage. This is required.
//     commandArgument1 = "doorlimit" - The door closing delay in 10 tick cycles (default is 5 if not specified).
//     commandArgument2 = "batchlimit" - The number of doors to control in a batch before waiting for the next cycle.
// 
//  Use Tag: "Exterior" to enable airlock interlock Management to door groups to lock doors shut when airlock engaged
// 
//  This will look for any airlock door groups on your connected grids within the airlock door group "[BaseAirlocks]" and manage them. 
//  The number "1" indicates the number of 10 tick cycles to wait between each state check (default is 5 if not specified).
//  
//  Doors will auto close and indicator lights will change based on pressurisation status. Green = depressurised and doors closed, Yellow = Doors closed and pressurised, Red = pressurised / doors open.
// 
//  Will expand features in future updates.
// 

string airlockgroup_tag = "[Airlock]";
int doorcount_limit = 5;
bool SetupComplete = false;
string version = "1.1.8";

//display settings
float display_zoom = 0.803f;
string lcd_display_name = $"[Airlock D1]";
int lcd_display_index = 0; //used for devices with multiple screen panels (0+)      
string lcd_display_name_2 = "[Airlock D2]";
int lcd_display_index_2 = 0; //used for devices with multiple screen panels (0+)
string lcd_display_name_3 = "[Airlock D3]";
int lcd_display_index_3 = 0; //used for devices with multiple screen panels (0+)     

List<IMyDoor> allDoors = new List<IMyDoor>();

List<IMyAirVent> allVents = new List<IMyAirVent>();
List<IMyLightingBlock> allLights = new List<IMyLightingBlock>();
List<IMyReflectorLight> allReflectorLights = new List<IMyReflectorLight>();
List<airlockgroup> airlockgroups = new List<airlockgroup>();

List<IMyBlockGroup> groupsAll = new List<IMyBlockGroup>();
List<IMyBlockGroup> groupsTag = new List<IMyBlockGroup>();

StringBuilder sbtext = new StringBuilder();
List<IMyTerminalBlock> display_all = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> display_tag_main = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> display_tag_2 = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> display_tag_3 = new List<IMyTerminalBlock>();

List<IMyTextSurface> myTextSurfaces_d1 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d2 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d3 = new List<IMyTextSurface>();

List<IMyMotorStator> rotors_all = new List<IMyMotorStator>();
List<IMyMotorAdvancedStator> rotorAdvancedStators_all = new List<IMyMotorAdvancedStator>();
List<IMyPistonBase> pistons_all = new List<IMyPistonBase>();

IMyCubeGrid meCubeGrid;
StringBuilder blocktag = new StringBuilder();
string spinner = "";
int state = 0;

private IEnumerator<bool> airlockCoroutine_2;
bool listdoors_finished = false;
double percentgroups_2 = 0.0;
private IEnumerator<bool> airlockCoroutine_3;
bool listvents_finished = false;
double percentgroups_3 = 0.0;
int batchcountlimit = 64;
string ALGT = "";

string commandArgument1 = "doorlimit";
string commandArgument2 = "batchlimit";
string runargument;
StringBuilder sbtexttemp = new StringBuilder();
int runTick = 0;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    if (!string.IsNullOrWhiteSpace(Storage))
    {
        ParseAndApplyArguments(Storage);
        Echo("Configuration loaded from Storage.");
    }
}

private void ParseAndApplyArguments(string input)
{
    if (string.IsNullOrEmpty(input))
        return;
            
    runargument = input;

    string[] airlockdata = input.Split(',');

    if (airlockdata.Length >= 1 && !string.IsNullOrWhiteSpace(airlockdata[0]))
    {
        airlockgroup_tag = airlockdata[0].Trim();
        Echo($"Airlock Tag set to: {airlockgroup_tag}");
        ALGT = airlockgroup_tag.Replace("[", "[[").Replace("]", "]]");
    }

    if (airlockdata.Length > 1)
    {

        for (int i = 1; i < airlockdata.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(airlockdata[i]))
            {
                string[] argumentdata = airlockdata[i].Split('=');
                if (argumentdata.Length > 1)
                {
                    if (!string.IsNullOrWhiteSpace(argumentdata[0]) && !string.IsNullOrWhiteSpace(argumentdata[1]))
                    {
                        if (argumentdata[0].Trim().Contains(commandArgument1))
                        {
                            if (!int.TryParse(argumentdata[1].Trim(), out doorcount_limit))
                            {
                                doorcount_limit = 2;
                                Echo($"Invalid Door Limit value: {argumentdata[1].Trim()}");
                            }
                        }

                        else if (argumentdata[0].Trim().Contains(commandArgument2))

                        {
                            if (!int.TryParse(argumentdata[1].Trim(), out batchcountlimit))
                            {
                                batchcountlimit = 8;
                                Echo($"Invalid Batch Count Limit value: {argumentdata[1].Trim()}");
                            }
                        }

                    }

                }
            }
        }
    }

}

public void Save()
{
    // Combine the current configuration into the Storage string
    Storage = runargument;
    Echo($"Configuration saved to Storage: {Storage}");
}

public class airlockgroup
{
    public List<IMyDoor> InteriorDoors;
    public List<int> interiorDoorCounter;
    public List<IMyDoor> ExteriorDoors;
    public List<int> exteriorDoorCounter;
    public List<IMyDoor> GeneralDoors;
    public List<int> generalDoorCounter;
    public List<IMyAirVent> vents;
    public List<IMyLightingBlock> indicatorlights;
    //public List<IMyLightingBlock> warningLights;
    public List<IMyReflectorLight> reflectorLights;
    public int pressurizedvents;
    public int interiordoorsopen;
    public int exteriordoorsopen;
    public int generaldoorsopen;
    string groupname;            

    public airlockgroup(bool init = true, string gn = "")
    {
        InteriorDoors = new List<IMyDoor>();
        interiorDoorCounter = new List<int>();
        ExteriorDoors = new List<IMyDoor>();
        exteriorDoorCounter = new List<int>();
        GeneralDoors = new List<IMyDoor>();
        generalDoorCounter = new List<int>();
        vents = new List<IMyAirVent>();
        indicatorlights = new List<IMyLightingBlock>();
        //warningLights = new List<IMyLightingBlock>();
        reflectorLights = new List<IMyReflectorLight>();
        pressurizedvents = 0;
        interiordoorsopen = 0;
        exteriordoorsopen = 0;
        generaldoorsopen = 0;
        groupname = gn;

    }
}

public void Main(string argument, UpdateType updateSource)
{
    runTick++;
    // Check if a new argument was passed (manually via run or timer setup)
    if (!string.IsNullOrEmpty(argument))
    {
        // --- Argument takes precedence for setup and override ---
        ParseAndApplyArguments(argument);
        Save();
        // Force a full setup if arguments changed
        SetupComplete = false;
    }

    if (!SetupComplete)
    {
        setup_system();
    }

    if (airlockgroups.Count == 0)
    {
        Echo($"{argument}");
        Echo($"No airlock groups found with tag: {ALGT}");
        return;
    }

    sbtexttemp.Clear();

    sbtexttemp.Append("AADM Control Status ").Append(version).Append('\n');
    sbtexttemp.Append("------------------------------").Append('\n');
    sbtexttemp.Append('\n');
    sbtexttemp.Append("Airlock Group: ").Append(ALGT).Append('\n');
    ;
    sbtexttemp.Append("Airlocks Monitored: ").Append(airlockgroups.Count).Append('\n');
    sbtexttemp.Append("Batch: ").Append(batchcountlimit).Append(" Door delay: ").Append(doorcount_limit).Append('\n');
    sbtexttemp.Append("Status: AADM Running ").Append(spinner).Append('\n');


    scanairlockvents();
    manageairlockdoors();

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

    sbtext.Clear();
    sbtext.Append("---  AADM ").Append(airlockgroup_tag).Append(" Control Status ---").Append('\n');
    sbtext.Append("============================").Append('\n');
    sbtext.Append('\n');
    sbtext.Append('\n');
    sbtext.Append("Airlock Group: ").Append(airlockgroup_tag).Append('\n');
    sbtext.Append("Airlocks Monitored: ").Append(airlockgroups.Count).Append('\n');
    sbtext.Append("Status: AADM Running ").Append(spinner).Append('\n');
    displaymng(sbtext);
    if (runTick % 6 == 0)
    {
        Echo(sbtexttemp.ToString());
    }
    state++;
    if (runTick > 60)
    {
        runTick = 0;              
    }
}

public void displaymng(StringBuilder sbtext)
{
    if (myTextSurfaces_d1.Count == 0)
    {
        sbtexttemp.Append("Display missing").Append('\n');
    }

    // WriteText accepts StringBuilder directly, which is perfectly optimized.
    for (int i = 0; i < myTextSurfaces_d1.Count; i++)
        myTextSurfaces_d1[i]?.WriteText(sbtext);

    for (int i = 0; i < myTextSurfaces_d2.Count; i++)
        myTextSurfaces_d2[i]?.WriteText(sbtext);

    for (int i = 0; i < myTextSurfaces_d3.Count; i++)
        myTextSurfaces_d3[i]?.WriteText(sbtext);
}


public void manageairlockdoors()
{

    //coroutine list
    if (listdoors_finished)
    {
        listdoors_finished = false;
    }
    if (airlockCoroutine_2 == null && !listdoors_finished)
    {
        airlockCoroutine_2 = doors_light_mng();
    }
    if (airlockCoroutine_2 != null && !listdoors_finished)
    {
        // Check the current yield value
        bool currentYield = airlockCoroutine_2.Current;

        // If the coroutine is finished, you can perform completion logic
        if (!airlockCoroutine_2.MoveNext())
        {
            // The coroutine has finished executing
            sbtexttemp.AppendLine("Airlock doors complete.");
            airlockCoroutine_2?.Dispose();
            airlockCoroutine_2 = null;

            //doors_light_mng().Dispose();
        }
        else
        {
            // Handle intermediate status if needed
            if (!currentYield)
            {
                if (airlockgroups.Count > 0)
                {
                    sbtexttemp.AppendLine($"Airlock door list... {Math.Round(percentgroups_2, 1)}%");
                }
                airlockCoroutine_2.MoveNext();
            }
        }

    }


}

public void scanairlockvents()
{
    //coroutine list
    if (listvents_finished)
    {
        listvents_finished = false;
    }
    if (airlockCoroutine_3 == null && !listvents_finished)
    {
        airlockCoroutine_3 = vent_door_scan();
    }
    if (airlockCoroutine_3 != null && !listvents_finished)
    {
        // Check the current yield value
        bool currentYield = airlockCoroutine_3.Current;

        // If the coroutine is finished, you can perform completion logic
        if (!airlockCoroutine_3.MoveNext())
        {
            // The coroutine has finished executing
            sbtexttemp.AppendLine("Airlock vents complete.");
            airlockCoroutine_3?.Dispose();
            airlockCoroutine_3 = null;
        }
        else
        {
            // Handle intermediate status if needed
            if (!currentYield)
            {
                if (airlockgroups.Count > 0)
                {
                    sbtexttemp.AppendLine($"Airlock vents list... {Math.Round(percentgroups_3, 1)}%");
                }
                airlockCoroutine_3.MoveNext();
            }
        }

    }

}


public void setup_system()
{
    IMyGridTerminalSystem gts = GridTerminalSystem;
    if (!SetupComplete)
    {
        groupsAll.Clear();
        groupsTag.Clear();
        airlockgroups.Clear();
        allDoors.Clear();
        allVents.Clear();
        allLights.Clear();
        allReflectorLights.Clear();
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
        gts.GetBlockGroups(groupsAll);

        if (groupsAll.Count > 0)
        {
            Echo("Found Groups");
            foreach (var group in groupsAll)
            {
                //add groups with airlock tag to working list
                if (group.Name.Contains(airlockgroup_tag))
                {
                    groupsTag.Add(group);
                    airlockgroups.Add(new airlockgroup(true, group.Name));
                }
            }
        }
        groupsAll.Clear();
        //Process airlock block groups

        if (groupsTag.Count > 0)
        {
            Echo("Found Airlock Groups: "+ groupsTag.Count);
            for (int i = 0; i < groupsTag.Count; i++)
            {
                if (groupsTag[i] != null)
                {
                    //get doors
                    allDoors.Clear();
                    groupsTag[i].GetBlocksOfType<IMyDoor>(allDoors);
                    //get vents
                    allVents.Clear();
                    groupsTag[i].GetBlocksOfType<IMyAirVent>(allVents);
                    //get lights
                    allLights.Clear();
                    groupsTag[i].GetBlocksOfType<IMyLightingBlock>(allLights);
                    allReflectorLights.Clear();
                    groupsTag[i].GetBlocksOfType<IMyReflectorLight>(allReflectorLights);

                    //sort doors into interior and exterior
                    if (allDoors.Count > 0)
                    {
                        foreach (var door in allDoors)
                        {
                            if (door != null)
                            {
                                if (door.CustomName.Contains("Interior") || door.CustomName.Contains("interior") || door.CustomName.Contains("Internal") || door.CustomName.Contains("internal"))
                                {
                                    if (!door.CustomName.Contains(airlockgroup_tag))
                                    {
                                        blocktag.Clear();
                                        blocktag.Append(door.CustomName).Append($" [{airlockgroup_tag}] {i}");
                                        door.CustomName = blocktag.ToString();
                                        blocktag.Clear();
                                    }                                            
                                    airlockgroups[i].InteriorDoors.Add(door);
                                    airlockgroups[i].interiorDoorCounter.Add(0);
                                }
                                else if (door.CustomName.Contains("Exterior") || door.CustomName.Contains("exterior") || door.CustomName.Contains("External") || door.CustomName.Contains("external"))
                                {
                                    if (!door.CustomName.Contains(airlockgroup_tag))
                                    {
                                        blocktag.Clear();
                                        blocktag.Append(door.CustomName).Append($" [{airlockgroup_tag}] {i}");
                                        door.CustomName = blocktag.ToString();
                                        blocktag.Clear();
                                    }
                                    airlockgroups[i].ExteriorDoors.Add(door);
                                    airlockgroups[i].exteriorDoorCounter.Add(0);
                                    airlockgroups[i].GeneralDoors.Add(door);
                                    airlockgroups[i].generalDoorCounter.Add(0);
                                }
                                else
                                {
                                    if (!door.CustomName.Contains(airlockgroup_tag))
                                    {
                                        blocktag.Clear();
                                        blocktag.Append(door.CustomName).Append($" [{airlockgroup_tag}] {i}");
                                        door.CustomName = blocktag.ToString();
                                        blocktag.Clear();
                                    }
                                    airlockgroups[i].GeneralDoors.Add(door);
                                    airlockgroups[i].generalDoorCounter.Add(0);
                                }
                            }
                        }
                    }
                    allDoors.Clear();

                    //sort lights into indicator and warning
                    if (allLights.Count > 0)
                    {
                        foreach (var light in allLights)
                        {
                            if (light != null)
                            {
                                if (!light.BlockDefinition.SubtypeName.Contains("RotatingLight"))
                                {
                                    if (!light.CustomName.Contains(airlockgroup_tag))
                                    {
                                        blocktag.Clear();
                                        blocktag.Append(light.CustomName).Append($" [{airlockgroup_tag}] {i}");
                                        light.CustomName = blocktag.ToString();
                                        blocktag.Clear();
                                    }                                            
                                    airlockgroups[i].indicatorlights.Add(light);
                                }
                            }
                        }
                    }
                    allLights.Clear();

                    //sort lights into indicator and warning
                    if (allReflectorLights.Count > 0)
                    {
                        foreach (var light in allReflectorLights)
                        {
                            if (light != null)
                            {
                                if (!light.CustomName.Contains(airlockgroup_tag))
                                {
                                    blocktag.Clear();
                                    blocktag.Append(light.CustomName).Append($" [{airlockgroup_tag}] {i}");
                                    light.CustomName = blocktag.ToString();
                                    blocktag.Clear();
                                }
                                light.CustomName = $"Airlock (reflector Light) {airlockgroup_tag} {i}";
                                airlockgroups[i].reflectorLights.Add(light);
                            }
                        }
                    }
                    allReflectorLights.Clear();

                    //add vents
                    if (allVents.Count > 0)
                    {
                        foreach (var vent in allVents)
                        {
                            if (vent != null)
                            {
                                if (!vent.CustomName.Contains(airlockgroup_tag))
                                {
                                    blocktag.Clear();
                                    blocktag.Append(vent.CustomName).Append($" [{airlockgroup_tag}] {i}");
                                    vent.CustomName = blocktag.ToString();
                                    blocktag.Clear();
                                }
                                vent.CustomName = $"Airlock (Air Vent) {airlockgroup_tag} {i}";
                                airlockgroups[i].vents.Add(vent);
                            }
                        }
                    }
                    allVents.Clear();
                }


            }
        }
        groupsTag.Clear();

        lcd_display_name = $"{airlockgroup_tag} D1";
        lcd_display_name_2 = $"{airlockgroup_tag} D2";
        lcd_display_name_3 = $"{airlockgroup_tag} D3";
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
        if (myTextSurfaces_d1.Count <= 0)
        {
            Echo($"Displays with tag {lcd_display_name.Replace("[", "[[").Replace("]", "]]")} not found.");
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

        SetupComplete = true;

        ALGT = airlockgroup_tag.Replace("[", "[[").Replace("]", "]]");            
        Echo("Setup complete!");
    }
}


IEnumerator<bool> vent_door_scan()
{
    int batchcount = 0;
    int batchcountlimit = 8;
    if (airlockgroups.Count > 0)
    {
        for (int i = 0; i < airlockgroups.Count; i++)
        {
            airlockgroup group = airlockgroups[i];
            //scan vents
            group.pressurizedvents = 0;
            if (group.vents.Count > 0)
            {
                foreach (var vent in airlockgroups[i].vents)
                {
                    if (vent != null)
                    {
                        float status = vent.GetOxygenLevel();
                        if (status > 0.0f)
                        {
                            group.pressurizedvents++;
                        }
                    }
                }
            }

            //scan general doors
            if (group.GeneralDoors.Count > 0)
            {
                for (int j = 0; j < group.GeneralDoors.Count; j++)
                {
                    var door = group.GeneralDoors[j];
                    if (door != null)
                    {
                        if (door.OpenRatio == 1.0f || (door.OpenRatio > 0.0f && door.OpenRatio < 1.0f))
                        {
                            group.generalDoorCounter[j]++;
                        }
                    }
                }
            }



            // 3. General Doors Processing
            if (group.generalDoorCounter.Count > 0 && group.GeneralDoors.Count == group.generalDoorCounter.Count)
            {
                group.generaldoorsopen = 0;
                for (int j = 0; j < group.generalDoorCounter.Count; j++)
                {
                    var door = group.GeneralDoors[j];
                    if (door != null)
                    {
                        float openRatio = door.OpenRatio; // Cross the engine boundary ONCE

                        if (group.generalDoorCounter[j] > 0)
                        {
                            group.generaldoorsopen++;
                        }
                        if (group.generalDoorCounter[j] > doorcount_limit && openRatio == 1.0f)
                        {
                            door.CloseDoor();
                        }
                        if (group.generalDoorCounter[j] > 0 && openRatio == 0.0f)
                        {
                            group.generalDoorCounter[j] = 0;
                        }
                    }
                }
                if (group.ExteriorDoors.Count > 0)
                {
                    for (int j = 0; j < group.GeneralDoors.Count; j++)
                    {
                        if (group.GeneralDoors[j] != null)
                        {
                            if (group.generaldoorsopen > 0 && group.GeneralDoors[j].OpenRatio == 0.0f && group.GeneralDoors[j].Enabled)
                            {
                                group.GeneralDoors[j].Enabled = false;
                            }
                            if (group.generaldoorsopen == 0 && group.GeneralDoors[j].OpenRatio == 0.0f && !group.GeneralDoors[j].Enabled)
                            {
                                group.GeneralDoors[j].Enabled = true;
                            }
                        }
                    }
                }
            }

            if (group.reflectorLights.Count > 0)
            {
                foreach (var light in group.reflectorLights)
                {
                    if (light != null)
                    {
                        if (group.interiordoorsopen > 0 || group.exteriordoorsopen > 0 || group.generaldoorsopen > 0)
                        {
                            //turn off indicator lights
                            light.Enabled = true;
                            light.Color = Color.Orange;
                        }
                        if (group.interiordoorsopen == 0 && group.exteriordoorsopen == 0 && group.generaldoorsopen == 0)
                        {
                            //turn off indicator lights
                            light.Enabled = false;
                            light.Color = Color.Orange;
                        }
                    }


                }
            }
            airlockgroups[i] = group;

            percentgroups_3 = ((double)i / (double)(airlockgroups.Count - 1)) * 100;
            if (airlockgroups.Count == 1)
            {
                percentgroups_3 = 100.0;
            }
            if (i == airlockgroups.Count - 1)
            {
                listvents_finished = true;
            }
            batchcount++;
            if (batchcount > batchcountlimit)
            {
                batchcount = 0;
                yield return false;
            }
        }
    }
    yield return true;
}

IEnumerator<bool> doors_light_mng()
{
    int batchcount = 0;
    if (airlockgroups.Count > 0)
    {
        for (int i = 0; i < airlockgroups.Count; i++)
        {
            airlockgroup group = airlockgroups[i];

            if (group.indicatorlights.Count > 0)
            {
                foreach (var light in group.indicatorlights)
                {
                    if (light != null)
                    {
                        if (group.pressurizedvents > 0 || group.interiordoorsopen > 0 || group.exteriordoorsopen > 0 || group.generaldoorsopen > 0)
                        {
                            //turn off indicator lights
                            light.Enabled = true;
                            light.Color = Color.Red;
                        }
                        else
                        {
                            //turn on indicator lights
                            light.Enabled = true;
                            light.Color = Color.Green;
                        }

                        if (group.interiordoorsopen == 0 && group.exteriordoorsopen == 0 && group.generaldoorsopen == 0 && group.pressurizedvents > 0)
                        {
                            //turn off indicator lights
                            light.Enabled = true;
                            light.Color = Color.Yellow;
                        }
                        if (group.interiordoorsopen == 0 && group.exteriordoorsopen == 0 && group.generaldoorsopen == 0 && group.pressurizedvents == 0)
                        {
                            //turn off indicator lights
                            light.Enabled = true;
                            light.Color = Color.Green;
                        }
                    }


                }
            }
            airlockgroups[i] = group;
            // Update progress tracking metrics
            percentgroups_2 = ((double)i / (double)(airlockgroups.Count - 1)) * 100;
            if (airlockgroups.Count == 1) percentgroups_2 = 100.0;
            if (i == airlockgroups.Count - 1) listdoors_finished = true;

            batchcount++;
            if (batchcount > batchcountlimit)
            {
                batchcount = 0;
                yield return false;
            }
        }
    }
    yield return true;
}
