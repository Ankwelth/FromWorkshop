
/* v1.06 02/09/2022
; Copy&Paste the configuration data on "Custom Data" on the current "Programmable block"
; AirVentName (one block or Group) is necessary, other are optional, leave it blank
; Begin  Copy&Paste of configuration data ------------------------

[Room_AirVented1] 
AirVentName=
LightsGroupName=
LCDTextPanelName=
DoorsGroupName=
HangarGroupName=
AuxiliarAirVentName=
AutoLock=
ChangeColorLightsGroupName=
LightOriginaColor=
LightAlertColor=
ShowDepressurize=
ShowStatus=
ShowAutoLock=
ShowTanksFilledRatio=
ShowOpenDoors=
ShowRoomName= 

[Room_AirVented2] 
AirVentName=
LightsGroupName=
LCDTextPanelName=
DoorsGroupName=
HangarGroupName=
AuxiliarAirVentName=
AutoLock=
ChangeColorLightsGroupName=
LightOriginaColor=
LightAlertColor=
ShowDepressurize=
ShowStatus=
ShowAutoLock=
ShowTanksFilledRatio=
ShowOpenDoors=
ShowRoomName= 

[Room_AirVented3] 
AirVentName=
LightsGroupName=
LCDTextPanelName=
DoorsGroupName=
HangarGroupName=
AuxiliarAirVentName=
AutoLock=
ChangeColorLightsGroupName=
LightOriginaColor=
LightAlertColor=
ShowDepressurize=
ShowStatus=
ShowAutoLock=
ShowTanksFilledRatio=
ShowOpenDoors=
ShowRoomName= 

; End Copy&Paste of configuration data ---------------------------
    */

MyIni ini = new MyIni();
static readonly string ConfigSection = "Room";
static readonly string DisplaySectionPrefix = ConfigSection + "_AirVented";
StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
Rooms rooms;
List<Room> openingRoomsList = new List<Room>();
List<Room> openingRoomsListRemove = new List<Room>();
List<Room> closingRoomsList = new List<Room>();
List<Room> closingRoomsListRemove = new List<Room>();
static readonly StringBuilder errorMessage = new StringBuilder();
static readonly StringBuilder initErrorMessage = new StringBuilder();
static readonly List<IMyDoor> disableWhenDone = new List<IMyDoor>();

class Room : IEquatable<Room>
{
    public Room(string airVentName, string auxiliarAirVentName, string lightsGroupName, string lCDTextPanelName, string doorsGroupName, string hangarGroupName, bool forceAuto
        , string changeColorLightsGroupName, string lightOriginaColor, string lightAlertColor
        , bool showDepressurize, bool showStatus, bool showAutoLock, bool showTanksFilledRatio, bool showOpenDoors, bool showRoomName, string roomName
        , IMyGridTerminalSystem gridTerminalSystem)
    {
        this.AirVentName = airVentName;
        this.AuxiliarAirVentName = auxiliarAirVentName;
        this.LightsGroupName = lightsGroupName;
        this.LCDTextPanelName = lCDTextPanelName;
        this.DoorsGroupName = doorsGroupName;
        this.HangarGroupName = hangarGroupName;
        this.ForceAuto = forceAuto;
        this.ShowDepressurize = showDepressurize;
        this.ShowStatus = showStatus;
        this.ShowAutoLock = showAutoLock;
        this.ShowTanksFilledRatio = showTanksFilledRatio;
        this.ShowOpenDoors = showOpenDoors;
        this.ShowRoomName = showRoomName;
        this.RoomName = roomName;

        this.TextPanelList = new List<IMyTextSurface>();
        if (lCDTextPanelName != null && !lCDTextPanelName.Equals(""))
        {
            IMyBlockGroup textPanelGroup = gridTerminalSystem.GetBlockGroupWithName(lCDTextPanelName);
            if (textPanelGroup != null)
            {
                textPanelGroup.GetBlocksOfType<IMyTextSurface>(this.TextPanelList);
            }
            else
            {
                IMyTextSurface textSurface = gridTerminalSystem.GetBlockWithName(lCDTextPanelName) as IMyTextSurface;
                if (textSurface != null)
                {
                    this.TextPanelList.Add(textSurface);
                }
                else
                {
                    initErrorMessage.AppendLine("LCDTextPanelName: '" + lCDTextPanelName + "' not found!");
                }
            }

            foreach (IMyTextSurface textSurface in this.TextPanelList)
            {
                textSurface.ContentType = ContentType.TEXT_AND_IMAGE;
                textSurface.WriteText("");
            }
        }

        this.MainAirVents = new List<IMyAirVent>();
        if (airVentName != null && !airVentName.Equals(""))
        {
            IMyBlockGroup airVentGroup = gridTerminalSystem.GetBlockGroupWithName(airVentName);
            if (airVentGroup != null)
            {
                airVentGroup.GetBlocksOfType<IMyAirVent>(this.MainAirVents);
            }
            else
            {
                IMyAirVent airVent = gridTerminalSystem.GetBlockWithName(airVentName) as IMyAirVent;
                if (airVent != null)
                {
                    this.MainAirVents.Add(airVent);
                }
                else
                {
                    initErrorMessage.AppendLine("AirVentName: '" + airVentName + "' not found!");
                }
            }
        }
        this.AuxAirVents = new List<IMyAirVent>();
        if (auxiliarAirVentName != null && !auxiliarAirVentName.Equals(""))
        {
            IMyBlockGroup auxAirVentGroup = gridTerminalSystem.GetBlockGroupWithName(auxiliarAirVentName);
            if (auxAirVentGroup != null)
            {
                auxAirVentGroup.GetBlocksOfType<IMyAirVent>(this.AuxAirVents);
            }
            else
            {
                IMyAirVent airVent = gridTerminalSystem.GetBlockWithName(auxiliarAirVentName) as IMyAirVent;
                if (airVent != null)
                {
                    this.AuxAirVents.Add(airVent);
                }
                else
                {
                    initErrorMessage.AppendLine("AuxiliarAirVentName: '" + auxiliarAirVentName + "' not found!");
                }
            }
        }
        if ((this.MainAirVents.Count > 0 && !this.MainAirVents[0].Enabled) && this.AuxAirVents.Count > 0 && this.AuxAirVents[0].Enabled)
        {
            this.AirVents = this.AuxAirVents;
        }
        else
        {
            this.AirVents = this.MainAirVents;
        }

        this.OxygenLevel = -1;
        this.HangarDoors = new List<IMyDoor>();

        if (hangarGroupName != null && !hangarGroupName.Equals(""))
        {
            IMyBlockGroup doorsGroup = gridTerminalSystem.GetBlockGroupWithName(hangarGroupName);

            if (doorsGroup != null)
            {

                doorsGroup.GetBlocksOfType<IMyDoor>(HangarDoors);

            }
            else
            {
                IMyDoor door = gridTerminalSystem.GetBlockWithName(hangarGroupName) as IMyDoor;
                if (door != null)
                {
                    HangarDoors.Add(door);
                }
                else
                {
                    initErrorMessage.AppendLine("HangarGroupName: '" + hangarGroupName + "' not found!");
                }
            }
        }
        this.Doors = new List<IMyDoor>();
        if (doorsGroupName != null && !doorsGroupName.Equals(""))
        {
            IMyBlockGroup doorsGroup = gridTerminalSystem.GetBlockGroupWithName(doorsGroupName);

            if (doorsGroup != null)
            {
                doorsGroup.GetBlocksOfType<IMyDoor>(this.Doors);
            }
            else
            {
                IMyDoor door = gridTerminalSystem.GetBlockWithName(doorsGroupName) as IMyDoor;
                if (door != null)
                {
                    this.Doors.Add(door);
                }
                else
                {
                    initErrorMessage.AppendLine("DoorsGroupName : '" + doorsGroupName + "' not found!");
                }
            }
        }
        this.Lights = new List<IMyFunctionalBlock>();
        if (lightsGroupName != null && !lightsGroupName.Equals(""))
        {
            IMyBlockGroup lightsGroup = gridTerminalSystem.GetBlockGroupWithName(lightsGroupName);

            if (lightsGroup != null)
            {
                lightsGroup.GetBlocksOfType<IMyFunctionalBlock>(this.Lights);
            }
            else
            {
                IMyFunctionalBlock light = gridTerminalSystem.GetBlockWithName(lightsGroupName) as IMyFunctionalBlock;
                if (light != null)
                {
                    this.Lights.Add(light);
                }
                else
                {
                    initErrorMessage.AppendLine("LightsGroupName: '" + lightsGroupName + "' not found!");
                }
            }
        }

        this.ChangeColorLights = new List<IMyLightingBlock>();
        if (changeColorLightsGroupName != null && !changeColorLightsGroupName.Equals(""))
        {
            IMyBlockGroup lightsGroup = gridTerminalSystem.GetBlockGroupWithName(changeColorLightsGroupName);

            if (lightsGroup != null)
            {
                lightsGroup.GetBlocksOfType<IMyLightingBlock>(this.ChangeColorLights);
            }
            else
            {
                IMyLightingBlock light = gridTerminalSystem.GetBlockWithName(changeColorLightsGroupName) as IMyLightingBlock;
                if (light != null)
                {
                    this.ChangeColorLights.Add(light);
                }
                else
                {
                    initErrorMessage.AppendLine("ChangeColorLightsGroupName: '" + changeColorLightsGroupName + "' not found!");
                }
            }
        }

        this.LightOriginaColor = Color.White;
        if (lightOriginaColor != null && !lightOriginaColor.Equals(""))
        {
            try
            {
                string[] array = lightOriginaColor.Split(',');
                if (array.Length == 3)
                {
                    int r = Convert.ToInt32(array[0].Trim());
                    int g = Convert.ToInt32(array[1].Trim());
                    int b = Convert.ToInt32(array[2].Trim());
                    this.LightOriginaColor = new Color(r, g, b);
                }
                else if (array.Length == 4)
                {
                    int r = Convert.ToInt32(array[0].Trim());
                    int g = Convert.ToInt32(array[1].Trim());
                    int b = Convert.ToInt32(array[2].Trim());
                    int a = Convert.ToInt32(array[3].Trim());

                    this.LightOriginaColor = new Color(r, g, b, a);
                }
                else
                {
                    initErrorMessage.AppendLine("LightOriginaColor: '" + lightOriginaColor + "' is not a color!");
                }
            }
            catch
            {
                initErrorMessage.AppendLine("LightOriginaColor: '" + lightOriginaColor + "' is not a color!");
            }
        }

        this.LightAlertColor = Color.White;
        if (lightAlertColor != null && !lightAlertColor.Equals(""))
        {
            try
            {
                string[] array = lightAlertColor.Split(',');
                if (array.Length == 3)
                {
                    int r = Convert.ToInt32(array[0].Trim());
                    int g = Convert.ToInt32(array[1].Trim());
                    int b = Convert.ToInt32(array[2].Trim());
                    this.LightAlertColor = new Color(r, g, b);
                }
                else if (array.Length == 4)
                {
                    int r = Convert.ToInt32(array[0].Trim());
                    int g = Convert.ToInt32(array[1].Trim());
                    int b = Convert.ToInt32(array[2].Trim());
                    int a = Convert.ToInt32(array[3].Trim());

                    this.LightAlertColor = new Color(r, g, b, a);
                }
                else
                {
                    initErrorMessage.AppendLine("LightAlertColor: '" + lightAlertColor + "' is not a color!");
                }
            }
            catch
            {
                initErrorMessage.AppendLine("LightAlertColor: '" + lightAlertColor + "' is not a color!");
            }
        }

        this.SafeOpenDoor = new List<IMyDoor>();
        this.Waiting = false;
    }
    public string AirVentName { get; }
    public string LightsGroupName { get; }
    public string LCDTextPanelName { get; }
    public string DoorsGroupName { get; }
    public string HangarGroupName { get; }

    public string AuxiliarAirVentName { get; }

    public List<IMyDoor> SafeOpenDoor { get; }

    public float OxygenLevel { get; set; }

    public List<IMyTextSurface> TextPanelList { get; }

    public List<IMyAirVent> AirVents { get; set; }

    private List<IMyAirVent> MainAirVents { get; }
    private List<IMyAirVent> AuxAirVents { get; }

    public List<IMyDoor> HangarDoors { get; }

    public List<IMyDoor> Doors { get; }

    public List<IMyFunctionalBlock> Lights { get; }

    public bool Waiting { get; set; }

    public bool ForceAuto { get; }

    public Color LightOriginaColor { get; }

    public Color LightAlertColor { get; }

    public List<IMyLightingBlock> ChangeColorLights { get; }

    public bool ShowDepressurize { get; }
    public bool ShowStatus { get; }
    public bool ShowAutoLock { get; }
    public bool ShowTanksFilledRatio { get; }
    public bool ShowOpenDoors { get; }

    public bool ShowRoomName { get; }
    public string RoomName { get; }

    public bool Equals(Room other)
    {
        return other.AirVentName.Equals(this.AirVentName)
            && other.AuxiliarAirVentName.Equals(this.AuxiliarAirVentName)
            && other.LightsGroupName.Equals(this.LightsGroupName)
            && other.LCDTextPanelName.Equals(this.LCDTextPanelName)
            && other.DoorsGroupName.Equals(this.DoorsGroupName)
            && other.HangarGroupName.Equals(this.HangarGroupName);
    }

    public void LoadOxygenLevel()
    {

        if (this.AirVents != null)
        {

            this.OxygenLevel = this.AirVents[0].GetOxygenLevel();
        }
    }

    public void WriteText(string text, bool append)
    {

        foreach (IMyTextSurface textSurface in this.TextPanelList)
        {

            textSurface.WriteText(text, append);
        }
    }

    public void WriteTextLine(string text, bool append)
    {

        this.WriteText(text + "\n", append);
    }


    public void WriteTextLine(string text)
    {

        this.WriteTextLine(text, true);
    }

    public void Depressurize(bool value)
    {
        // start depressurizing and ativate the auxiliar system if they are not the active airvents
        if (value && this.AuxAirVents.Count > 0 && this.AuxAirVents != this.AirVents)
        {
            foreach (IMyAirVent airVent in this.AuxAirVents)
            {
                airVent.Enabled = true;
                airVent.Depressurize = value;
            }
        }

        //After the hangar door(s) is closed, disable the main air vent(s)
        if (!value && this.AuxAirVents.Count > 0 && this.AirVents[0].GetOxygenLevel() > 0.98)
        {
            foreach (IMyAirVent airVent in this.MainAirVents)
            {
                airVent.Enabled = false;
            }
            foreach (IMyAirVent airVent in this.AuxAirVents)
            {
                airVent.Enabled = true;
            }
            this.AirVents = this.AuxAirVents;
        }
        foreach (IMyAirVent airVent in this.AirVents)
        {
            airVent.Depressurize = value;
        }
    }

    public void CheckRoomPressure()
    {
        //normalize pressure in case of manualy close doors

        if (this.AirVents[0].Status == VentStatus.Depressurized || this.AirVents[0].Status == VentStatus.Depressurizing && this.AirVents[0].CanPressurize)
        {
            foreach (IMyAirVent airvent in this.AirVents)
            {
                airvent.Depressurize = false;
            }
        }


        if ((this.AirVents[0].Status == VentStatus.Pressurized || this.AirVents[0].Status == VentStatus.Pressurizing) && this.AirVents[0].GetOxygenLevel() < 0.65 && this.AirVents == this.AuxAirVents)
        {
            //the room is pressurized but the oxygen level may be droping, the auxiliar tanks may have run out of oxigen. Enable main air vents

            foreach (IMyAirVent airvent in this.MainAirVents)
            {
                airvent.Enabled = true;
                airvent.Depressurize = false;
            }
            this.AirVents = this.MainAirVents;

            //Disable auxiliar air vents to save energy
            foreach (IMyAirVent airvent in this.AuxAirVents)
            {
                airvent.Enabled = false;
            }
        }

        //check room if full pressurized and ativate the auxiliar if exists

        if (this.AuxAirVents.Count > 0 && this.AirVents[0].Status == VentStatus.Pressurized && this.AirVents[0].GetOxygenLevel() > 0.99 && this.AirVents == this.MainAirVents)
        {
            foreach (IMyAirVent airvent in this.AuxAirVents)
            {
                airvent.Enabled = true;
                airvent.Depressurize = false;
            }
            this.AirVents = this.AuxAirVents;

            //Disable auxiliar air vents to save energy
            foreach (IMyAirVent airvent in this.MainAirVents)
            {
                airvent.Enabled = false;
            }
        }
    }
}

class Rooms
{
    public Rooms(IMyGridTerminalSystem gridTerminalSystem)
    {
        this.List = new List<Room>();
        this.GasTankList = new List<IMyGasTank>();
        gridTerminalSystem.GetBlocksOfType<IMyGasTank>(this.GasTankList, t => t.IsWorking && t.Enabled && t.DefinitionDisplayNameText.Equals("Oxygen Tank"));



    }
    public List<Room> List { get; }

    public List<IMyGasTank> GasTankList { get; }



    public float GetFilledRatio()
    {
        float totalCapacity = 0f;
        float totalFilled = 0f;
        foreach (IMyGasTank tank in this.GasTankList)
        {
            totalCapacity += tank.Capacity;
            totalFilled += (float)tank.FilledRatio * tank.Capacity;


        }
        return totalFilled / totalCapacity;
    }


    public Room GetRoomByDoor(string doorName, bool open)
    {
        Room candidateRoom = null;
        IMyDoor candidateDoor = null;
        foreach (Room room in this.List)
        {
            if (doorName.Equals(room.HangarGroupName, StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (IMyDoor door in room.HangarDoors)
                {
                    if (!room.SafeOpenDoor.Contains(door))
                    {
                        room.Waiting = true;
                        room.SafeOpenDoor.Add(door);
                    }

                }
                return room;
            }

            foreach (IMyDoor door in room.Doors)
            {
                if (door.CustomName.Equals(doorName, StringComparison.InvariantCultureIgnoreCase))
                {

                    if (candidateRoom == null)
                    {
                        candidateRoom = room;
                        candidateDoor = door;
                    }
                    else
                    {
                        // if door belong to 2 room, its a interior door and if the room have the same pressure, just open it or close, no need to change air vents
                        if (room.AirVents[0].Status == candidateRoom.AirVents[0].Status)
                        {
                            if (open)
                            {
                                bool previusStatus = door.Enabled;
                                door.Enabled = true;
                                door.OpenDoor();
                                if (room.ForceAuto || !previusStatus)
                                {
                                    disableWhenDone.Add(door);
                                }
                            }
                            else
                            {
                                bool previusStatus = door.Enabled;
                                door.Enabled = true;
                                door.CloseDoor();
                                if (room.ForceAuto || !previusStatus)
                                {
                                    disableWhenDone.Add(door);
                                }

                            }
                            candidateRoom = null;
                            candidateDoor = null;
                        }
                        else
                        //If looking for room to safe open door, return the pressurized room
                        if (open && room.AirVents[0].Status == VentStatus.Pressurized)
                        {
                            candidateRoom = room;
                            candidateDoor = door;
                        }
                        else if (!open && room.AirVents[0].Status == VentStatus.Depressurizing)
                        {
                            candidateRoom = room;
                            candidateDoor = door;
                        }
                    }
                }
            }
        }

        if (candidateRoom != null && !candidateRoom.SafeOpenDoor.Contains(candidateDoor))
        {
            candidateRoom.Waiting = true;
            candidateRoom.SafeOpenDoor.Add(candidateDoor);
        }
        return candidateRoom;
    }

    public void ClearPanel()
    {
        foreach (Room room in this.List)
        {
            room.WriteText("", false);
        }
    }

}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    this.Init();
}

public void Main(string argument, UpdateType updateSource)
{
    errorMessage.Clear();

    rooms.ClearPanel();

    this.CheckDoors();

    this.CheckRooms();

    this.CheckSafeOpeningDoor(argument);
    this.CheckClosingHangar(argument);

    if (errorMessage.ToString().Length > 0)
    {
        Echo(errorMessage.ToString());
    }
    if (initErrorMessage.ToString().Length > 0)
    {
        Echo(initErrorMessage.ToString());
    }
}

void Init()
{
    rooms = new Rooms(this.GridTerminalSystem);

    List<String> SectionNames = new List<string>();
    ini.TryParse(Me.CustomData);
    ini.GetSections(SectionNames);
    foreach (var section in SectionNames)
    {
        if (section.StartsWith(DisplaySectionPrefix, ignoreCase))
        {
            string airventName = ini.Get(section, "AirVentName").ToString();
            string auxiliarAirVentName = ini.Get(section, "AuxiliarAirVentName").ToString();

            string lightsGroupName = ini.Get(section, "LightsGroupName").ToString();
            string lcdTextPanelName = ini.Get(section, "LCDTextPanelName").ToString();
            string doorsGroupName = ini.Get(section, "DoorsGroupName").ToString();
            string hangarGroupName = ini.Get(section, "HangarGroupName").ToString();


            string autoLockStr = ini.Get(section, "AutoLock").ToString();
            if (autoLockStr == null || autoLockStr.Equals(""))
            {
                autoLockStr = ini.Get(section, "ForceAuto").ToString();
            }

            bool autoLock = (autoLockStr.Equals("on", ignoreCase) || autoLockStr.Equals("yes", ignoreCase) || autoLockStr.Equals("true", ignoreCase));
            string changeColorLightsGroupName = ini.Get(section, "ChangeColorLightsGroupName").ToString();
            string lightOriginaColor = ini.Get(section, "LightOriginaColor").ToString();
            string lightAlertColor = ini.Get(section, "LightAlertColor").ToString();
            string showDepressurizeStr = ini.Get(section, "ShowDepressurize").ToString();
            string showStatusStr = ini.Get(section, "ShowStatus").ToString();
            string showAutoLockStr = ini.Get(section, "ShowAutoLock").ToString();
            string showTanksFilledRatioStr = ini.Get(section, "ShowTanksFilledRatio").ToString();
            string showOpenDoorsStr = ini.Get(section, "ShowOpenDoors").ToString();
            string showRoomNameStr = ini.Get(section, "ShowRoomName").ToString();

            bool showDepressurize = (showDepressurizeStr.Equals("on", ignoreCase) || showDepressurizeStr.Equals("yes", ignoreCase) || showDepressurizeStr.Equals("true", ignoreCase) || showDepressurizeStr.Equals(""));
            bool showStatus = (showStatusStr.Equals("on", ignoreCase) || showStatusStr.Equals("yes", ignoreCase) || showStatusStr.Equals("true", ignoreCase) || showStatusStr.Equals(""));
            bool showAutoLock = (showAutoLockStr.Equals("on", ignoreCase) || showAutoLockStr.Equals("yes", ignoreCase) || showAutoLockStr.Equals("true", ignoreCase) || showAutoLockStr.Equals(""));
            bool showTanksFilledRatio = (showTanksFilledRatioStr.Equals("on", ignoreCase) || showTanksFilledRatioStr.Equals("yes", ignoreCase) || showTanksFilledRatioStr.Equals("true", ignoreCase) || showTanksFilledRatioStr.Equals(""));
            bool showOpenDoors = (showOpenDoorsStr.Equals("on", ignoreCase) || showOpenDoorsStr.Equals("yes", ignoreCase) || showOpenDoorsStr.Equals("true", ignoreCase) || showOpenDoorsStr.Equals(""));
            bool showRoomName = (showRoomNameStr.Equals("on", ignoreCase) || showRoomNameStr.Equals("yes", ignoreCase) || showRoomNameStr.Equals("true", ignoreCase) || showRoomNameStr.Equals(""));

            if (airventName != null && !airventName.Equals(""))
            {
                Room room = new Room(airventName, auxiliarAirVentName, lightsGroupName, lcdTextPanelName, doorsGroupName, hangarGroupName, autoLock, changeColorLightsGroupName, lightOriginaColor
                    , lightAlertColor, showDepressurize, showStatus, showAutoLock, showTanksFilledRatio, showOpenDoors, showRoomName, section.Substring(DisplaySectionPrefix.Length)
                    , GridTerminalSystem);
                if (room.AirVents.Count > 0)
                {
                    rooms.List.Add(room);
                }
                else
                {
                    initErrorMessage.AppendLine("Air Vent(s): '" + airventName + "' not found!");
                }
            }




        }
    }
    if (rooms.List.Count < 1)
    {
        initErrorMessage.AppendLine("Could not load any room on configuration set in 'Custom Data'");
    }
}

void CheckSafeOpeningDoor(string argument)
{
    if (argument.StartsWith("open ", StringComparison.InvariantCultureIgnoreCase))
    {
        string doorName = argument.Substring("open ".Length);
        Room room = rooms.GetRoomByDoor(doorName, true);
        if (room != null)
        {

            if (!openingRoomsList.Contains(room))
            {
                openingRoomsList.Add(room);
            }
            closingRoomsList.RemoveAll(r => r.Equals(room));
        }
    }
    if (openingRoomsList.Count > 0)
    {
        foreach (Room room in openingRoomsList)
        {
            this.SafeOpen(room);
        }
    }
    if (openingRoomsListRemove.Count > 0)
    {
        foreach (Room room in openingRoomsListRemove)
        {
            openingRoomsList.Remove(room);
        }
        openingRoomsListRemove.Clear();
    }
}

void CheckClosingHangar(string argument)
{
    if (argument.StartsWith("close ", StringComparison.InvariantCultureIgnoreCase))
    {
        string hangarDoorName = argument.Substring("close ".Length);
        Room room = rooms.GetRoomByDoor(hangarDoorName, false);
        if (room != null)
        {
            if (!closingRoomsList.Contains(room))
            {
                closingRoomsList.Add(room);
            }

            openingRoomsList.RemoveAll(r => r.Equals(room));
        }
    }
    if (closingRoomsList.Count > 0)
    {
        foreach (Room room in closingRoomsList)
        {
            this.SafeClose(room);
        }
    }
    if (closingRoomsListRemove.Count > 0)
    {
        foreach (Room room in closingRoomsListRemove)
        {
            closingRoomsList.Remove(room);
        }
        closingRoomsListRemove.Clear();
    }
}

void SafeOpen(Room room)
{
    bool allDoorsClosed = this.CloseDoors(room);

    if (!allDoorsClosed)
    {
        return;
    }

    room.Depressurize(true);

    bool progress = true;
    if (room.OxygenLevel == -1)
    {
        room.LoadOxygenLevel();
    }
    else
    {
        double oldOxygenLevel = Math.Round(room.OxygenLevel, 3);
        room.LoadOxygenLevel();
        double currentOxygenLevel = Math.Round(room.OxygenLevel, 3);
        progress = oldOxygenLevel != currentOxygenLevel;

    }
    room.WriteText("Oxygen Level changing: " + (progress ? "yes" : "no") + "\n", true);

    if (!progress)
    {
        //Close  Doors

        foreach (IMyDoor door in room.SafeOpenDoor)
        {

            if (door.Status == DoorStatus.Closed || door.Status == DoorStatus.Closing)
            {
                bool previusStatus = door.Enabled;
                door.Enabled = true;
                door.OpenDoor();
                if (room.ForceAuto || !previusStatus)
                {
                    disableWhenDone.Add(door);
                }
            }

        }
        room.Waiting = false;

        room.OxygenLevel = -1;
        openingRoomsListRemove.Add(room);
    }
}

void SafeClose(Room room)
{
    bool allclosed = true;

    foreach (IMyDoor door in room.SafeOpenDoor)
    {
        if (door.Status != DoorStatus.Closed && door.Status != DoorStatus.Closing)
        {
            bool previusStatus = door.Enabled;
            door.Enabled = true;
            door.CloseDoor();
            if (room.ForceAuto || !previusStatus)
            {
                disableWhenDone.Add(door);
            }
        }
        if (door.Status != DoorStatus.Closed)
        {
            allclosed = false;
        }
    }
    if (allclosed)
    {
        room.Waiting = false;
    }

    room.WriteText("Door closed: " + (allclosed ? "yes" : "no") + "\n", true);
    if (allclosed && room.AirVents[0].CanPressurize)
    {

        room.Depressurize(false);

        room.SafeOpenDoor.Clear();

        closingRoomsListRemove.Add(room);
    }
    else if (allclosed && !room.AirVents[0].CanPressurize)
    {
        room.WriteText("Can not Pressurize!\n", true);
    }
}





void CheckLights(Room room)
{

    foreach (IMyFunctionalBlock light in room.Lights)
    {
        light.Enabled = room.AirVents[0].GetOxygenLevel() < 0.5;
    }


    if (room.AirVents[0].GetOxygenLevel() < 0.5 && room.LightOriginaColor != room.LightAlertColor)
    {
        foreach (IMyLightingBlock light in room.ChangeColorLights)
        {
            light.Color = room.LightAlertColor;
        }
    }

    if (room.AirVents[0].GetOxygenLevel() >= 0.5 && room.LightOriginaColor != room.LightAlertColor)
    {
        foreach (IMyLightingBlock light in room.ChangeColorLights)
        {
            light.Color = room.LightOriginaColor;
        }
    }


}

void CheckRooms()
{

    float filledRatio = rooms.GetFilledRatio();
    List<IMyTextSurface> textSurfaces = new List<IMyTextSurface>();
    foreach (Room room in rooms.List)
    {
        room.CheckRoomPressure();

        this.CheckDoors(room);
        this.CheckLights(room);
        this.UpdatePanel(room);

        if (room.ShowTanksFilledRatio)
        {
            foreach (IMyTextSurface textSurface in room.TextPanelList)
            {
                if (!textSurfaces.Contains(textSurface))
                {
                    textSurfaces.Add(textSurface);
                }
            }
        }

    }

    foreach (IMyTextSurface textSurface in textSurfaces)
    {
        textSurface.WriteText("Oxygen tanks: " + (Math.Round(filledRatio * 100, 1)) + "%\n", true);
    }
}

void CheckDoors()
{

    foreach (IMyDoor door in disableWhenDone)
    {
        if (door.Status == DoorStatus.Closed || door.Status == DoorStatus.Open)
        {
            door.Enabled = false;
        }
    }

    disableWhenDone.RemoveAll(door => door.Status == DoorStatus.Closed || door.Status == DoorStatus.Open);
}

bool CloseDoors(Room room)
{
    bool allDoorsClosed = true;
    foreach (IMyDoor door in room.Doors)
    {
        if (!room.SafeOpenDoor.Contains(door))
        {
            if (door.Status != DoorStatus.Closed || door.Status != DoorStatus.Closing)
            {
                bool previusStatus = door.Enabled;
                if (!previusStatus)
                {
                    door.Enabled = true;
                }
                door.CloseDoor();
                if (!previusStatus)
                {
                    disableWhenDone.Add(door);
                }

            }
            if (door.Status != DoorStatus.Closed)
            {
                allDoorsClosed = false;
            }
        }
    }
    return allDoorsClosed;
}

void CheckDoors(Room room)
{
    foreach (IMyDoor door in room.Doors)
    {
        if (room.AirVents[0].GetOxygenLevel() < 0.5 && door.Status == DoorStatus.Open && !room.SafeOpenDoor.Contains(door))
        {
            bool previusStatus = door.Enabled;
            if (!previusStatus)
            {
                door.Enabled = true;
            }
            door.CloseDoor();
            if (!previusStatus)
            {
                disableWhenDone.Add(door);
            }
        }
    }
    if (!room.Waiting)
    {
        room.SafeOpenDoor.RemoveAll(d => d.Status == DoorStatus.Closed);
    }
}

void UpdatePanel(Room room)
{
    if (room.ShowRoomName)
    {
        room.WriteTextLine("Room: " + room.RoomName);
    }
    room.WriteTextLine(room.AirVents[0].CustomName + ": " + Math.Round(room.AirVents[0].GetOxygenLevel() * 100, 1) + "%");
    if (room.ShowDepressurize)
    {
        room.WriteTextLine("Depressurize: " + (room.AirVents[0].Depressurize ? "yes" : "no"));
    }
    if (room.ShowStatus)
    {
        room.WriteTextLine("Status: " + (room.AirVents[0].Status));
    }
    if (room.ShowAutoLock)
    {
        room.WriteTextLine("AutoLock: " + (room.ForceAuto ? "on" : "off"));
    }


    if (room.ShowOpenDoors)
    {
        foreach (IMyDoor door in room.SafeOpenDoor)
        {
            room.WriteTextLine("Safe open: " + door.CustomName);
        }
    }


    errorMessage.AppendLine(room.AirVents[0].CustomName + ": " + Math.Round(room.AirVents[0].GetOxygenLevel() * 100, 1) + "%");

}

