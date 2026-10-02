static string[][] data;
MyIni inidata = new MyIni();
List<IMyDoor> alldoors = new List<IMyDoor>();
List<IMyDoor> outerGroup = new List<IMyDoor>();
List<IMyDoor> innerGroup = new List<IMyDoor>();
List<IMyAirVent> ventGroup = new List<IMyAirVent>();
IMyBroadcastListener BroadcastListener;
StringBuilder stringbuild = new StringBuilder();
MyIni thisDoor = new MyIni();

double version = 4.3;
const int settings_start_row_number = 2;
const int airlock_start_row_number = 11;
const string broadCastTag = "SW_airlockData";
string customData;
string[] parsedData;
string[][] gridedData;
double uptime;
double helpdisplayTime;
bool updateBlocksThisTick = false;
bool autoClose;
int opentimer;
int opentimerhangar;
int detectopentimer;
int detectopentimerhangar;
bool doAirlockDoors;
string reuseable;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    Echo("Stormworks Airlock Control Script\nVersion "+version.ToString()+"\n");

    BroadcastListener = IGC.RegisterBroadcastListener(broadCastTag);
    BroadcastListener.SetMessageCallback(broadCastTag);

    IMyTextSurface textSurface = Me.GetSurface(0);
    textSurface.ContentType = ContentType.TEXT_AND_IMAGE;
    textSurface.FontSize = 3.5F;
    textSurface.Alignment = TextAlignment.CENTER;
    textSurface.TextPadding = 0.2F;
    textSurface.WriteText("Stormworks\nAirlock\nControl");

    loadData();
}
public void Main(string runcommand, UpdateType updateSource)
{
    if (updateBlocksThisTick)//update settings and grab block data from terminal, called every minute or if new data is recieved from another instance of airlockControl
    {
        loadData();
    }
    //update logic
    if (updateSource == UpdateType.Update10) //on block auto-update
    {
        //Echo("Runtime lag: " + Runtime.LastRunTimeMs.ToString());
        if (uptime % 1 > 0.9 || uptime % 1 < 0.1)
        {
            if (autoClose) { updateDoors(); }
            updateAirlocks();
        }
        uptime += 1.0 / 6.0;
        if (helpdisplayTime > 0) { helpdisplayTime -= 1.0 / 6.0; }
        uptimeEcho(uptime, helpdisplayTime);
    }
    else if(updateSource == UpdateType.Trigger || updateSource == UpdateType.Terminal)//on button press
    {
        Object[] newStatus = SplitArgs(runcommand);
        if ((int)newStatus[0] != -1)
        {
            updateData((int)newStatus[0], (string)newStatus[1]);
        }
    }
    else if (updateSource == UpdateType.IGC)//if script update was from recieving an IGC communication
    {
        while (BroadcastListener.HasPendingMessage)//may have multiple incoming aka multiple blocks communicating at once
        {
            MyIGCMessage incomingMessage = BroadcastListener.AcceptMessage();
            if (incomingMessage.Tag == broadCastTag)
            {
                if (incomingMessage.Data is string)
                {
                    processIGCdata(incomingMessage.Data.ToString());
                }
            }
        }
        Storage = inidata.ToString();
        updateBlocksThisTick = true;
    }
}

public void Save()
{
    inidata.Set("Runtime", "uptime", uptime.ToString());
    inidata.Set("Runtime", "helpdisplayTime", helpdisplayTime.ToString());
    Storage = inidata.ToString(); 
}

//load any data needed for the script, called in program setup and whenever checking for new blocks and whatnot
public void loadData()
{
    alldoors.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyDoor>(alldoors);
    inidata.TryParse(Storage);

    customData = Me.CustomData;
    if (checkEmptyData()) { return; }
    parsedData = customData.Split('\n');
    gridedData = new string[parsedData.Length][];
    if (checkVersion())
    {
        updateDataVersion();
        updateBlocksThisTick = true;
        return;
    }
    data = ReadCustomData2D(Me.CustomData, true);

    //uptime tracking setup
    try
    {
        uptime = Double.Parse(inidata.Get("Runtime", "uptime").ToString());
        helpdisplayTime = Double.Parse(inidata.Get("Runtime", "helpdisplayTime").ToString());
    }
    catch (Exception e)
    {
        uptime = 0;
        inidata.Set("Runtime", "uptime", 0);
        inidata.Set("Runtime", "helpdisplayTime", 0);
        //Echo(e.ToString());
    }
    autoClose = inidata.Get("settings", "auto close").ToBoolean();
    opentimer = inidata.Get("settings", "auto close timer").ToInt32();
    opentimerhangar = inidata.Get("settings", "hangar door auto close timer").ToInt32();
    detectopentimer = inidata.Get("settings", "detection auto cycle timer").ToInt32();
    detectopentimerhangar = inidata.Get("settings", "hangar detection auto cycle timer").ToInt32();
    doAirlockDoors = inidata.Get("settings", "auto close airlock doors").ToBoolean();

    updateBlocksThisTick = false;
}

//reads the button input arguments, then splits them and returns the new door state
public Object[] SplitArgs(string runcommand)
{
    string[] splitruncommand = runcommand.Split('\"');
    //when split like this, index 0 == ""
    int newStatus = -1;

    if (splitruncommand.Length == 3)
    {
        splitruncommand[2] = splitruncommand[2].Replace(" ", string.Empty);
        if (string.Equals(splitruncommand[2].ToLower(), "in"))
        {
            newStatus = 4;
        }
        else if (string.Equals(splitruncommand[2].ToLower(), "out"))
        {
            newStatus = 1;
        }
        else if (string.Equals(splitruncommand[2].ToLower(), "cycle"))
        {
            newStatus = 6;//will just check which way it's going and reverse it
        }
        else if (string.Equals(splitruncommand[2].ToLower(), "close"))
        {
            newStatus = -2;//will just close both doors
        }
    }
    else if (string.Equals(splitruncommand[0].ToLower(), "help"))
    {
        helpText();
        helpdisplayTime = 15;
        return new Object[2] { -1, "" };
    }
    else if (string.Equals(splitruncommand[0].ToLower(), "clearcache"))
    {
        inidata.Clear();
        uptime = 0;
        Storage = "";
        updateBlocksThisTick = true;
        return new Object[2] { -1, "" };
    }
    else
    {
        Echo("Can't read arguments. Are they in the Right Format?\n\"Airlock Name\" out/in\nor\n\"all\" out/in");
        return new Object[2] { -1, "" };
    }
    Object[] status = new Object[2] { newStatus, splitruncommand[1] };
    return status;
}

//updates the parsed data with the new door status for the given airlock
public void updateData(int newStatus, string targetAirlock)
{
    string cyclestyle = inidata.Get("settings", "cycle style").ToString();
    Boolean overRide = false;
    Boolean leave = false;
    Boolean enter = false;
    Boolean auto = false;

    if (cyclestyle == "fast")
    {
        overRide = true;
        leave = false;
        enter = false;
        auto = true;
    }

    for (int row = airlock_start_row_number; row < data.Length; row++)
    {
        //If the current airlock being checked is matches the wanted name, or if all are being cycled
        if (targetAirlock.ToLower().Equals("all") || data[row][0].Equals(targetAirlock))
        {
            if (data[row][0].ToString() == "") { break; }
            if (data[row].Length > 5)
            {
                for (int command = 4; command < data[row].Length; command++)//check each command
                {
                    string thisCommand = data[row][command].ToLower();
                    if (thisCommand.Equals("override:off"))
                    {
                        overRide = false;
                    }
                    else if (thisCommand.Equals("override:leave") || thisCommand.Equals("override:limited"))
                    {
                        leave = true;
                    }
                    else if (thisCommand.Equals("override:enter"))
                    {
                        enter = true;
                    }
                    else if (thisCommand.Equals("override:auto"))
                    {
                        auto = true;
                    }
                    else if (thisCommand.Equals("detect"))
                    {
                        
                    }
                }
            }
            //status value is always the last value in the row
            int status = inidata.Get("Airlock Control Numbers", data[row][0]).ToInt32();
            if (status == newStatus && overRide)//if trying to override the pressurization sequence
            {
                if (newStatus == 1 && !enter)//force depressureize
                {
                    status = 2;
                }
                else if (newStatus == 4 && !leave)//force pressurize
                {
                    status = 5;
                }
            }
            else if (newStatus == 6)
            {
                if (status == 0 || status == 3)
                {
                    status++;
                }
                else if (overRide)
                {
                    if (status == 1 && !enter)
                    {
                        status++;
                    }
                    else if (status == 4 && !leave)
                    {
                        status++;
                    }
                }
            }
            else
            {
                status = newStatus;
            }
            if (auto)
            {
                if (status == 1 && !enter)//force depressureize
                {
                    status = 2;
                }
                else if (status == 4 && !leave)//force pressurize
                {
                    status = 5;
                }
            }
            inidata.Set("Airlock Control Numbers", data[row][0], status);
        }
    }
}

//updates all Doors
public void updateDoors()
{
    string doorname;
    bool found;
    reuseable = "Doors";

    foreach (var door in alldoors)
    {
        
        doorname = door.Name;
        int doorstatus;
        
        if(door.CustomData.Length == 0)
        {
            door.CustomData = "[Airlock Doors]\nignore this door=false";
        }

        try
        {
            doorstatus = Int32.Parse(inidata.Get(reuseable, doorname).ToString());
        }
        catch
        {   
            inidata.Set(reuseable, doorname, -2);
            doorstatus = -2;
        }

        bool airlockdetect = false;
        try
        {
            airlockdetect = inidata.Get("Detected Airlock Doors",doorname).ToBoolean();//only works if it exists
        }
        catch { }

        bool skip = false;
        
        try
        {
            thisDoor.TryParse(door.CustomData);

            if (thisDoor.Get("Airlock Doors", "ignore this door").ToBoolean())
            {
                skip = true;
            }
            if (!doAirlockDoors)
            {
                if (inidata.Get("Airlock Doors",door.Name).ToBoolean())
                {
                    skip = true;
                }
            }
        }
        catch{}

        //found = GridTerminalSystem.GetBlockWithId(door.EntityId) is IMyAirtightHangarDoor;
        found = door is IMyAirtightHangarDoor;
        //logic
        if (skip || airlockdetect){}//priority to skip over doors part of hangar systems or designated doors
        else if ((!found && doorstatus >= opentimer) || (found && doorstatus >= opentimerhangar))//door needs to close
        {
            door.CloseDoor();
            inidata.Set(reuseable, doorname, -1);
        }
        else if (door.OpenRatio > 0.99)//if door is open
        {
            if (doorstatus == -1) //just opened
            {
                inidata.Set(reuseable, doorname, 0);
            }
            else
            {
                doorstatus++;
                inidata.Set(reuseable, doorname, doorstatus);
            }
        }
        else if (door.OpenRatio < 0.01)
        {
            inidata.Set(reuseable, doorname, -1);
        }

        //autodetect system
        if (airlockdetect)
        {
            if ((!found && doorstatus >= detectopentimer) || (found && doorstatus >= detectopentimerhangar))//door needs to close
            {
                findDetectedAirlock(doorname);
                //Echo(doorstatus.ToString()+", "+detectopentimer.ToString());
                inidata.Set(reuseable, doorname, -1);
            }
            else if (door.OpenRatio > 0.99)//if door is open
            {
                if (doorstatus == -1) //just opened
                {
                    inidata.Set(reuseable, doorname, 0);
                }
                else
                {
                    doorstatus++;
                    inidata.Set(reuseable, doorname, doorstatus);
                }
            }
            else if (door.OpenRatio < 0.01)
            {
                inidata.Set(reuseable, doorname, -1);
            }
        }
    }
}

//finds which airlock was just detected opening, then update its cycling data
public void findDetectedAirlock(string doorname)
{
    List<IMyDoor> GroupDoors = new List<IMyDoor>();
    IMyDoor doorcheck;

    int airlockRow = -1;
    int direction = -1;
    //find which airlock it is
    for (int row = airlock_start_row_number; row < data.Length; row++)
    {
        if (data[row][0] == null || data[row][0] == "") { break; }
        
        bool outergrouped = false;
        bool innergrouped = false;
        bool found = false;

        try 
        {
            GroupDoors.Clear();
            IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(data[row][1]); 
            if (Group != null) 
            { 
                outergrouped = true;
                Group.GetBlocksOfType<IMyDoor>(GroupDoors);

                
                foreach(IMyDoor doorcheckgroup in GroupDoors)
                {
                    if(doorcheckgroup.Name == doorname)
                    {
                        found = true;
                        direction = 5;
                        break;
                    }
                }
            }
        } 
        catch { }
        if (!outergrouped)
        {
            doorcheck = GridTerminalSystem.GetBlockWithName(data[row][1]) as IMyDoor;
            if (doorcheck.Name == doorname)
            {
                found = true;
                direction = 5;
            }
        }

        try 
        {
            GroupDoors.Clear();
            IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(data[row][2]); 
            if (Group != null) 
            { 
                innergrouped = true;
                Group.GetBlocksOfType<IMyDoor>(GroupDoors);


                foreach (IMyDoor doorcheckgroup in GroupDoors)
                {
                    if (doorcheckgroup.Name == doorname)
                    {
                        found = true;
                        direction = 2;
                        break;
                    }
                }
            } 
        } 
        catch { }
        if (!innergrouped)
        {
            doorcheck = GridTerminalSystem.GetBlockWithName(data[row][2]) as IMyDoor;
            if (doorcheck.Name == doorname)
            {
                found = true;
                direction = 2;
            }
        }

        if (found)
        {
            airlockRow = row;
            break;
        }
    }
    //Echo(airlockRow.ToString());
    if(airlockRow != -1)
    {
        if (inidata.Get("Detect Airlock Tracker", data[airlockRow][0]).ToBoolean())
        {
            inidata.Set("Detect Airlock Tracker", data[airlockRow][0], "false");
            inidata.Set("Airlock Control Numbers", data[airlockRow][0], -2);
        }
        else
        {
            updateData(direction, data[airlockRow][0]);
            inidata.Set("Detect Airlock Tracker", data[airlockRow][0], "true");
        }
    }
}

//updates all Airlocks
public void updateAirlocks()
{
    for (int row = airlock_start_row_number; row < data.Length; row++)
    {
        if (data[row][0] == null || data[row][0] == "") { break; }
        int status = -1;
        bool outergrouped = false;
        bool innergrouped = false;
        bool ventsgrouped = false;
        try
        {
            status = inidata.Get("Airlock Control Numbers", data[row][0]).ToInt32();
        }
        catch
        {
            inidata.Set("Airlock Control Numbers", data[row][0],-1);
        }
        try { IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(data[row][1]); if (Group != null) { outergrouped = true; } } catch { }
        try { IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(data[row][2]); if (Group != null) { innergrouped = true; } } catch { }
        try { IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(data[row][3]); if (Group != null) { ventsgrouped = true; } } catch { }

        status = updateThisAirlock(row, status, innergrouped, outergrouped, ventsgrouped); //updates the door
        inidata.Set("Airlock Control Numbers", data[row][0], status);//updates the status number in the inidata
    }
}

//updates the specific airlock, returns status value
public int updateThisAirlock(int rownumber, int currentStatus, bool innergrouped, bool outergrouped, bool ventsgrouped)
{
    try
    {
        IMyDoor doorout = null;
        IMyDoor doorin = null;
        IMyAirVent airvent = null;
        IMyBlockGroup Group;

        bool detectdoor = false;
        foreach (string command in data[rownumber])
        {
            if (command == "detect")
            {
                detectdoor = true;
                break;
            }
        }

        bool skipvents = false;
        bool lockdoors = inidata.Get("settings", "airlock doors lock").ToBoolean();
        if (outergrouped)
        {
            Group = GridTerminalSystem.GetBlockGroupWithName(data[rownumber][1]);
            Group.GetBlocksOfType(outerGroup);
            foreach (var door in outerGroup)
            {
                if (door.CustomData == "")
                {
                    door.CustomData = "[Airlock Doors]\nignore this door=true";
                }
                else
                {
                    bool marked = inidata.Get("Detected Airlock Doors",door.Name).ToBoolean();
                    if (!marked && detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", door.Name, "true");
                        //Echo("true");
                    }
                    else if (marked && !detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", door.Name, "false");
                        //Echo("false");
                    }
                    inidata.Set("Airlock Doors", door.Name, "true");
                }
            }
        }
        else
        {
            doorout = GridTerminalSystem.GetBlockWithName(data[rownumber][1]) as IMyDoor;
            if (doorout.CustomData == "")
            {
                doorout.CustomData = "[Airlock Doors]\nignore this door=true";
            }
            else
            {
                bool marked = inidata.Get("Detected Airlock Doors", doorout.Name).ToBoolean();
                //Echo("Doorout:" + detectdoor.ToString()+",marked: "+marked);
                if (!marked && detectdoor)
                {
                    inidata.Set("Detected Airlock Doors", doorout.Name, "true");
                    //Echo("true");
                }
                else if(marked && !detectdoor)
                {
                    inidata.Set("Detected Airlock Doors", doorout.Name, "false");
                    //Echo("false");
                }
                inidata.Set("Airlock Doors", doorout.Name, "true");
            }
        }
        if (innergrouped)
        {
            Group = GridTerminalSystem.GetBlockGroupWithName(data[rownumber][2]);
            Group.GetBlocksOfType(innerGroup);
            foreach (var door in innerGroup)
            {
                if (door.CustomData == "")
                {
                    door.CustomData = "[Airlock Doors]\nignore this door=true";
                }
                else
                {
                    bool marked = inidata.Get("Detected Airlock Doors", door.Name).ToBoolean();
                    if (!marked && detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", door.Name, "true");
                    }
                    else if (marked && !detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", door.Name, "false");
                    }
                    inidata.Set("Airlock Doors", door.Name, "true");
                }
            }
        }
        else
        {
            doorin = GridTerminalSystem.GetBlockWithName(data[rownumber][2]) as IMyDoor;
            if (doorin.CustomData == "")
            {
                doorin.CustomData = "[Airlock Doors]\nignore this door=true";
            }
            else
            {
                bool marked = inidata.Get("Detected Airlock Doors", doorin.Name).ToBoolean();
                if (!marked && detectdoor)
                {
                    inidata.Set("Detected Airlock Doors", doorin.Name, "true");
                    //Echo("true");
                }
                else if (marked && !detectdoor)
                {
                    inidata.Set("Detected Airlock Doors", doorin.Name, "false");
                    //Echo("false");
                }
                inidata.Set("Airlock Doors", doorin.Name, "true");
            }
        }
        if (ventsgrouped)
        {
            Group = GridTerminalSystem.GetBlockGroupWithName(data[rownumber][3]);
            Group.GetBlocksOfType(ventGroup);
        }
        else
        {
            try
            {
                airvent = GridTerminalSystem.GetBlockWithName(data[rownumber][3]) as IMyAirVent;
                if(airvent == null)
                {
                    skipvents = true;
                }
            }
            catch
            {
                skipvents = true;
            }
        }
        //1 is starting cycle out
        if (currentStatus == 1 || currentStatus == 2)
        {
            bool closed = true;
            if (innergrouped)
            {
                foreach (var door in innerGroup)
                {
                    if ((float)door.OpenRatio > 0)
                    {
                        door.CloseDoor();
                        closed = false;
                    }
                }
            }
            else
            {
                if ((float)doorin.OpenRatio > 0) //if the door is open even a little bit
                {
                    doorin.CloseDoor();
                    closed = false;
                }
            }
            if (closed)
            {
                if (innergrouped)
                {
                    foreach (var door in innerGroup)
                    {
                        if (lockdoors) { door.Enabled = false; }
                    }
                }
                else
                {
                    if (lockdoors) { doorin.Enabled = false; }//turn off the door
                }
                if (ventsgrouped)
                {
                    foreach (var vent in ventGroup)
                    {
                        vent.Depressurize = true;
                    }
                    if (ventGroup[0].GetOxygenLevel() <= 0.025 || currentStatus == 2)//if status is 2, force open after inner door closes
                    {
                        if (outergrouped)
                        {
                            foreach (var door in outerGroup)
                            {
                                door.Enabled = true;
                                door.OpenDoor();
                                currentStatus = 3;
                            }
                        }
                        else
                        {
                            doorout.Enabled = true;
                            doorout.OpenDoor();
                            currentStatus = 3;
                        }
                    }
                }
                else
                {
                    if (!skipvents)
                    {
                        airvent.Depressurize = true; //depressureize the airlock vent
                        if (airvent.GetOxygenLevel() <= 0.025 || currentStatus == 2)//if status is 2, force open after inner door closes
                        {
                            if (outergrouped)
                            {
                                foreach (var door in outerGroup)
                                {
                                    door.Enabled = true;
                                    door.OpenDoor();
                                    currentStatus = 3;
                                }
                            }
                            else
                            {
                                doorout.Enabled = true;
                                doorout.OpenDoor();
                                currentStatus = 3;
                            }
                        }
                    }
                    else
                    {
                        if (outergrouped)
                        {
                            foreach (var door in outerGroup)
                            {
                                door.Enabled = true;
                                door.OpenDoor();
                                currentStatus = 3;
                            }
                        }
                        else
                        {
                            doorout.Enabled = true;
                            doorout.OpenDoor();
                            currentStatus = 3;
                        }
                    }
                }
            }
        }
        //4 is cycling in
        else if (currentStatus == 4 || currentStatus == 5)
        {
            bool closed = true;
            if (outergrouped)
            {
                foreach (var door in outerGroup)
                {
                    if ((float)door.OpenRatio > 0)
                    {
                        door.CloseDoor();
                        closed = false;
                    }
                }
            }
            else
            {
                if ((float)doorout.OpenRatio > 0) //if the door is open even a little bit
                {
                    doorout.CloseDoor();
                    closed = false;
                }
            }
            if (closed)
            {
                if (outergrouped)
                {
                    foreach (var door in outerGroup)
                    {
                        if (lockdoors) { door.Enabled = false; }
                    }
                }
                else
                {
                    if (lockdoors) { doorout.Enabled = false; } //turn off the door
                }
                if (ventsgrouped)
                {
                    foreach (var vent in ventGroup)
                    {
                        vent.Depressurize = false;
                    }
                    if (ventGroup[0].GetOxygenLevel() >= 0.975 || currentStatus == 5)//if status is 5, force open after outer door closes
                    {
                        if (innergrouped)
                        {
                            foreach (var door in innerGroup)
                            {
                                door.Enabled = true;
                                door.OpenDoor();
                                currentStatus = 0;
                            }
                        }
                        else
                        {
                            doorin.Enabled = true;
                            doorin.OpenDoor();
                            currentStatus = 0;
                        }
                    }
                }
                else
                {
                    if (!skipvents)
                    {
                        airvent.Depressurize = false; //pressureize the airlock vent
                        if (airvent.GetOxygenLevel() >= 0.975 || currentStatus == 5)//if status is 5, force open after outer door closes
                        {
                            if (innergrouped)
                            {
                                foreach (var door in innerGroup)
                                {
                                    door.Enabled = true;
                                    door.OpenDoor();
                                    currentStatus = 0;
                                }
                            }
                            else
                            {
                                doorin.Enabled = true;
                                doorin.OpenDoor();
                                currentStatus = 0;
                            }
                        }
                    }
                    else
                    {
                        if (innergrouped)
                        {
                            foreach (var door in innerGroup)
                            {
                                door.Enabled = true;
                                door.OpenDoor();
                                currentStatus = 0;
                            }
                        }
                        else
                        {
                            doorin.Enabled = true;
                            doorin.OpenDoor();
                            currentStatus = 0;
                        }
                    }
                }
            }
        }
        else if(currentStatus == -2)
        {
            if (outergrouped)
            {
                foreach (var door in outerGroup)
                {
                    if ((float)door.OpenRatio > 0)
                    {
                        door.CloseDoor();
                    }
                }
            }
            else
            {
                if ((float)doorout.OpenRatio > 0) //if the door is open even a little bit
                {
                    doorout.CloseDoor();
                }
            }
            if (innergrouped)
            {
                foreach (var door in innerGroup)
                {
                    if ((float)door.OpenRatio > 0)
                    {
                        door.CloseDoor();
                    }
                }
            }
            else
            {
                if ((float)doorin.OpenRatio > 0) //if the door is open even a little bit
                {
                    doorin.CloseDoor();
                }
            }
            currentStatus = 0;//set to being "cycled in"
        }
    }
    catch (Exception e)
    {
        Echo("Status Update Error: Check that Custom Data names match the ones in the terminal.\n(It's Case Sensitive)");
        //Echo(e.ToString());
    }
    return currentStatus;
}

//update internal list of doors to ignore using data from a connected airlock script instance
public void processIGCdata(string incomingData)//accepts incoming customdata
{
    string[][] otherdata = ReadCustomData2D(incomingData, false);

    for (int row = airlock_start_row_number; row < otherdata.Length; row++)
    {
        if (otherdata[row][0] == null || otherdata[row][0] == "") { break; }
        bool outergrouped = false;
        bool innergrouped = false;

        try { IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(otherdata[row][1]); if (Group != null) { outergrouped = true; } } catch { }
        try { IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(otherdata[row][2]); if (Group != null) { innergrouped = true; } } catch { }

        try
        {
            IMyDoor doorout = null;
            IMyDoor doorin = null;
            List<IMyDoor> outerGroup = new List<IMyDoor>();
            List<IMyDoor> innerGroup = new List<IMyDoor>();

            bool detectdoor = false;
            foreach (string command in otherdata[row])
            {
                if (command == "detect")
                {
                    detectdoor = true;
                    break;
                }
            }
            if (outergrouped)
            {
                IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(otherdata[row][1]);
                Group.GetBlocksOfType(outerGroup);
                foreach (var door in outerGroup)
                {
                    if (door.CustomData == "")
                    {
                        door.CustomData = "[Airlock Doors]\nignore this door=true";
                    }
                    else
                    {
                        bool marked = inidata.Get("Detected Airlock Doors", door.Name).ToBoolean();
                        if (!marked && detectdoor)
                        {
                            inidata.Set("Detected Airlock Doors", door.Name, "true");
                        }
                        else if (marked && !detectdoor)
                        {
                            inidata.Set("Detected Airlock Doors", door.Name, "false");
                        }
                        inidata.Set("Airlock Doors", door.Name, "true");
                    }
                }
            }
            else
            {
                doorout = GridTerminalSystem.GetBlockWithName(otherdata[row][1]) as IMyDoor;
                if (doorout.CustomData == "")
                {
                    doorout.CustomData = "[Airlock Doors]\nignore this door=true";
                }
                else
                {
                    bool marked = inidata.Get("Detected Airlock Doors", doorout.Name).ToBoolean();
                    if (!marked && detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", doorout.Name, "true");
                    }
                    else if (marked && !detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", doorout.Name, "false");
                    }
                    inidata.Set("Airlock Doors", doorout.Name, "true");
                }
            }
            if (innergrouped)
            {
                IMyBlockGroup Group = GridTerminalSystem.GetBlockGroupWithName(otherdata[row][2]);
                Group.GetBlocksOfType(innerGroup);
                foreach (var door in innerGroup)
                {
                    if (door.CustomData == "")
                    {
                        door.CustomData = "[Airlock Doors]\nignore this door=true";
                    }
                    else
                    {
                        bool marked = inidata.Get("Detected Airlock Doors", door.Name).ToBoolean();
                        if (!marked && detectdoor)
                        {
                            inidata.Set("Detected Airlock Doors", door.Name, "true");
                        }
                        else if (marked && !detectdoor)
                        {
                            inidata.Set("Detected Airlock Doors", door.Name, "false");
                        }
                        inidata.Set("Airlock Doors", door.Name, "true");
                    }
                }
            }
            else
            {
                doorin = GridTerminalSystem.GetBlockWithName(otherdata[row][2]) as IMyDoor;
                if (doorin.CustomData == "")
                {
                    doorin.CustomData = "[Airlock Doors]\nignore this door=true";
                }
                else
                {
                    bool marked = inidata.Get("Detected Airlock Doors", doorin.Name).ToBoolean();
                    if (!marked && detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", doorin.Name, "true");
                    }
                    else if (marked && !detectdoor)
                    {
                        inidata.Set("Detected Airlock Doors", doorin.Name, "false");
                    }
                    inidata.Set("Airlock Doors", doorin.Name, "true");
                }
            }
        }
        catch (Exception e)
        {
            //Echo("Status Update Error: Check that Custom Data names match the ones in the terminal.\n(It's Case Sensitive)");
            Echo(e.ToString());
        }
    }
}

//reads the blocks custom data and parses it out into a 2d array
public string[][] ReadCustomData2D(string data, bool isthisblock)
{
    string[] parsedData = data.Split('\n');
    string[][] gridedData = new string[parsedData.Length][];

    for (int row = 0; row < parsedData.Length; row++)
    {
        if (row == 0)
        {
            gridedData[row] = parsedData[row].Split('V'); //uses V to split off the version number
        }
        else if (row >= settings_start_row_number && row <= airlock_start_row_number - 2)
        {
            gridedData[row] = parsedData[row].Split(':');
            if (isthisblock) {inidata.Set("settings", gridedData[row][0], gridedData[row][1]); }
        }
        else if (row == airlock_start_row_number - 1)
        {
            gridedData[row] = parsedData[row].Split('!'); //uses ! to not split at all
        }
        else if (row >= airlock_start_row_number)
        {
            gridedData[row] = parsedData[row].Split(',');
        }
    }
    return gridedData;
}

//check if block's starting data is there or not. if not, initialize it.
public bool checkEmptyData()
{
    try
    {
        if (customData == "")
        {
            Echo("No Data. Initializing...");
            string startData = "Storms Airlock Control V"+version.ToString()+"\n_____Settings_____\nauto close:true\nauto close timer:5\nhangar door auto close timer:15\ncycle style:fast\nauto close airlock doors:false\nairlock doors lock:false\ndetection auto cycle timer:2\nhangar detection auto cycle timer:15\n_____Airlocks_____\n";

            Me.CustomData = startData;
            return true;
        }
        return false;
    }
    catch (Exception E)
    {
        Echo(E.ToString());
        return true;
    }
    
}

//check if block's data is in the updated format, if not, skip this run and set the data properly
public bool checkVersion()
{
    try
    {
        gridedData[0] = parsedData[0].Split('V'); //uses V to split off the version number

        if (gridedData[0][1] != version.ToString())
        {
            Echo("Data Version outdata\nSkipping this run to update data formatting");

            return true;
        }
        return false;
    }
    catch (Exception E)
    {
        //Echo(E.ToString());
        return true;
    }

}

//updates the data formatting, while preserving the data itself. only works for V4 and up
public void updateDataVersion()
{
    string data = Me.CustomData;
    string[] parsedData = data.Split('\n');
    string[][] gridedData = new string[parsedData.Length][];

    bool settings = false;
    int settingsrow = 0;
    bool airlocks = false;
    int airlocksrow = 0;
    int hasVersionNum = 0;


    for (int row = 0; row < parsedData.Length; row++)
    {
        if (parsedData[row] == "_____Settings_____")
        {
            settings = true;
            settingsrow = row+1;
        }
        else if (parsedData[row] == "_____Airlocks_____")
        {
            settings = false;
            airlocks = true;
            airlocksrow = row+1;
        }
        else if (settings)
        {
            gridedData[row] = parsedData[row].Split(':');
            inidata.Set("settings", gridedData[row][0], gridedData[row][1]);
        }
        else if (airlocks)
        {
            gridedData[row] = parsedData[row].Split(',');
        }
    }

    //if it has a version number, it will change whether it adds an extra line or not. (V4.0 doesn't have the V number line at the top
    try
    {
        gridedData[0] = parsedData[0].Split('V'); //uses V to split off the version number
    }
    catch (Exception E)
    {
        hasVersionNum = 1;
    }

    //rejoin the data in the correct format
    string lineData = "";
    string[] rowData = new string[parsedData.Length+1+hasVersionNum];

    for (int row = 0; row < gridedData.Length+hasVersionNum; row++)
    {
        Echo(row.ToString());
        if (row == 0)
        {
            lineData = "Storms Airlock Control V" + version.ToString();
        }
        else if (row == 1)
        {
            lineData = "_____Settings_____";
        }
        else if (row >= settings_start_row_number && row <= airlock_start_row_number - 2)
        {
            
            lineData = string.Join(":", gridedData[(row-settings_start_row_number)+settingsrow]);
        }
        else if (row == airlock_start_row_number - 1)
        {
            lineData = "_____Airlocks_____";
        }
        else if (row > airlock_start_row_number - 1)
        {
            try
            {
                lineData = string.Join(",", gridedData[(row - airlock_start_row_number) + airlocksrow]);
            }
            catch
            {
                lineData = "E";
            }
        }
        rowData.SetValue(lineData, row);
    }
    string finishedData = String.Join("\n", rowData);

    Me.CustomData = finishedData;
}

//takes the parsed data and shoves it back into one string to be saved back to the block
public string joinCustomData2D(string[][] arraydata)
{
    string lineData = "";
    string[] rowData = new string[arraydata.Length];

    for (int row = 0; row < arraydata.Length; row++)
    {
        if (row == 0)
        {
            lineData = "Storms Airlock Control V"+version.ToString();
        }
        else if (row == 1)
        {
            lineData = "_____Settings_____";
        }
        else if (row >= settings_start_row_number && row <= airlock_start_row_number-2)
        {
            lineData = string.Join(":", arraydata[row]);
        }
        else if(row == airlock_start_row_number-1)
        {
            lineData = "_____Airlocks_____";
        }
        else if (row >= airlock_start_row_number)
        {
            lineData = string.Join(",", arraydata[row]);
        }
        rowData.SetValue(lineData, row);
    }

    string finishedData = String.Join("\n", rowData);

    return finishedData;
}

//output handler for runtime/uptime tracking
public void uptimeEcho(double uptime, double helpdisplayTime)
{
    stringbuild.Clear();

    //uptime output
    int hrs = 0;
    int minutes = 0;
    int seconds;
    seconds = (int)Math.Round(uptime, 0);
    if (seconds > 60)
    {
        minutes = seconds / 60;
        seconds -= minutes * 60;

        if (((double)seconds/60.0 % 1) == 0)
        {
            Echo("Update");
            inidata.Set("Runtime", "uptime", uptime.ToString());
            inidata.Set("Runtime", "helpdisplayTime", helpdisplayTime.ToString());
            Storage = inidata.ToString();
            updateBlocksThisTick = true;
            //Send out airlock data to other instances of Airlock Control
            IGC.SendBroadcastMessage(broadCastTag, Me.CustomData, TransmissionDistance.ConnectedConstructs);
        }
        if (minutes > 60)
        {
            hrs = minutes / 60;
            minutes -= hrs * 60;

            if(hrs == 12)
            {
                inidata.Clear();
                uptime = 0;
                Storage = "";
                updateBlocksThisTick = true;
            }
        }
    }

    //Airlock Count
    List<MyIniKey> Airlocks = new List<MyIniKey>();
    inidata.GetKeys("Airlock Control Numbers", Airlocks);
    
    //helpDisplay Timer
    int helpUptime = (int)Math.Round(helpdisplayTime, 0);

    //Run Displays
    stringbuild.Append("Stormworks Airlock Control Script\nVersion ");
    stringbuild.Append(version);
    stringbuild.AppendLine();

    stringbuild.Append("Uptime: ");
    if (minutes > 0)
    {
        if (hrs > 0)
        {
            stringbuild.Append(hrs);
            stringbuild.Append(" hrs, ");
        }
        else
        {
            stringbuild.Append(minutes);
            stringbuild.Append(" mins, ");
        }
    }
    stringbuild.Append(seconds);
    stringbuild.Append(" seconds");
    stringbuild.AppendLine();

    if (hrs == 11 && minutes >= 50)
    {
        stringbuild.Append("Block Auto Cache Clear will occurr at the 12 hour mark.");
        stringbuild.AppendLine();
    }

    stringbuild.Append(Airlocks.Count.ToString());
    stringbuild.Append(" Airlocks Managed");
    stringbuild.AppendLine();
    stringbuild.Append(alldoors.Count.ToString());
    stringbuild.Append(" Doors Managed");
    stringbuild.AppendLine();
    Echo(stringbuild.ToString());

    if (helpUptime > 0)
    {
        helpText();
    }
}

//easy access for the help text pastable
public void helpText()
{
    Echo("\n---Command Help---\nCustom Data Format:\nAirlock Name,Outer Door(s),Inner Door(s),Air Vent(s)(optional),(Extra Options)\n\n---Extra Options---\ndetect\n\n---Settings---\nauto close:true/false  whether auto-close is active\n\nauto close timer:#, time in seconds before doors auto close\n\nhangar door auto close:#, same thing, but for hangar doors specifically\n\ncycle style:fast/fancy/fast out/fast in, fast skips pressurization stage\n\nauto close airlock doors:true/false, whether auto close affects airlock doors\n\nairlock doors lock:true/false, wether doors turn off when airlocking");
}