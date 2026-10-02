/**************************************************************************************************
                                    +-+-+-+-+-+-+-+-+-+-+-+-+
                                     heroldini's airlock script
                                    +-+-+-+-+-+-+-+-+-+-+-+-+



--- INSTALL:
Write This script into a programmable block, compile it and let it run.
Thats all, the Airlock script is ready to use.



--- CONTROL AIRLOCKS WITH THIS SCRIPT:
You have to build a airthigt airlock.
Minimum requirements are (Just blocks of the big grid allowed!):
- Outer Door
- Inner Door
- Airvent in the airlock (+ connected O2 tanks)

Additional:
- Sound block
- LCD panels
- Lights

You can build as many of each part as you like!
Now put following STRING-Tags in the NAME or in the CUSTOM DATA of each of this blocks:
[AirL:MyCustomName]

The doors additionally needs the information "Inner" or "Outer":
[AirL:MyCustomName:Inner]
[AirL:MyCustomName:Outer]

Thats it - you created a Airlocksystem named "MyCustomName".
(The O2 tanks dont need any of this STRING-Tags)
You can see now the information "Airlocks found: 1" on the CPU screen.

To create another one, replace "MyCustomName" in the string-tags with any other name for the next airlock.



--- BUTTONS TO CONTROL:
Build button panals, where ever you want to open/close the airlock from.
Now over the toolbar config, you can assign the programable block (which including the running script) to a button.
Use the option "Run". You have to enter an argument in the editbox:

MyCustomName:toggle
Toggles the airlock from Inner to Outer or visa versa. If Airlock is inactiv (see "free" or "lock" argument)
this button also initialize the airlock first and after a second click, it toggles again.

MyCustomName:lock 
Close both doors and disable it.

MyCustomName:free
It opens both doors of the airlock, for example when you are landed on earth.

MyCustomName:init
With this argument you can just initialize the airlock after an error or when its not in toggle mode.



--- TIPPS:
- You can rename each button in the terminal of the button panel. Look bottom in the middle, there is a List of Button 1 - 4, select one,
and under the selction field, there is a editbox to rename it. You can choose any text, what ever you want ;)

- The script hides all renamed (or custom data) blocks from the terminal and the toolbar config

- If you have a leak in the airlock, the airlocksystem set to an "save mode", which close all doors turns on flash lights and alarm sound.
You can open the door per hand now.
If the leak is sealed, the alarm ends and you have to initialize it. (also possible by the toggle button)
Close both doors befor initialize it!

- Sometimes there is a bug in the air calculation in the game, in this case stop the script, restart your game.
You may have to remove some blocks and rebuild.

- If the O2 tanks of the base are full or the vent can (de)pressurize for any reason,
there are some detection systems in the script. In this case, the doors open without (de)pressurizing.


--- SETTINGS:
In the following part of the code, you can change some settings:
(for example the STRING-Tag chars or the text on the LCDS.)

***************************************************************************************************/

// ---------------------------------------- SETTINGS ----------------------------------------------

// LCD text
const string TEXT_INIT = "initialize Airlock...";
const string TEXT_READY = "<< AIRLOCK READY >>";
const string TEXT_CLOSING = "close airlock...";
const string TEXT_CLOSED = "<< AIRLOCK CLOSED >>";
const string TEXT_FREE = "free airlock...";
const string TEXT_INACTIVE = "<< AIRLOCK INACTIVE >>";
const string TEXT_TOGGLE = "airlock toggle...";
const string TEXT_DEPRESS = "depressurize...";
const string TEXT_DEPRESSED = "<< DEPRESSURIZED >>";
const string TEXT_PRESS = "pressurize...";
const string TEXT_PRESSED = "<< PRESSURIZED >>";
const string TEXT_ERR_NOTOGGLE = "<< ERROR: Not in toggle mode! >>";
const string TEXT_ERR_NOTIGHT = "<< SAVE MODE: Airlock leaking!>>";
const string TEXT_ERR_NOPARTS = "<< ERROR: Parts are missing! >>";// not in use for now - maybe coming soon

// ID-Tag structure
const char sTStart = '[';
const string sTName = "AirL";
const char sTEnd = ']';
const char sTCut = ':';

// Parameter INNER / OUTER
const string PARAM_INNER = "Inner";
const string PARAM_OUTER = "Outer";

// Airlock commands
const string CMD_FAST = "fast";
const string CMD_INIT = "init";
const string CMD_LOCK = "lock";
const string CMD_FREE = "free";
const string CMD_TOGGLE = "toggle";
const string CMD_IDLE = "idle";
const string CMD_ERROR = "error";

const string CMD_DEBUG_ON = "debug:on";
const string CMD_DEBUG_OFF = "debug:off";

// Sound settings
const string SOUND_BLOCK_NORMAL = "Alarm 2";
const string SOUND_BLOCK_HANGAR = "Alarm 1";

// Automatically unshow the airlock blocks in the menus:
const bool HIDE_IN_TOOLBAR_MENU = true;
const bool HIDE_IN_TERMINAL = true;

// ERROR HANDLING: How many times in a row must the fan be at exactly the same fill level until it is recognized that it has "hung up"?
const int VENT_HANGED_CNT = 300;




// --- /!\ STOP - NO MORE CHANGES /!\ ----- MAIN PROGRAMM - /!\ DO NOT CHANGE ANYTHING BELOW THIS LINE /!\ ----- /!\ STOP - NO MORE CHANGES /!\ ---

// PROGRAM INIT
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;					            // Loop
}




// PROGRAMM MAIN
public void Main(string argument, UpdateType updateSource)
{
    Echo("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n     heroldini's airlock script\n+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
    Echo("Version: " + SCRIPT_VERSION);
    
    
    // SUB TICKS WORK
    iSubTicks = iTicks++ %10;                                                       // Calculate subticks (for activity wich has to be done in longer time distances (it also inc the iTicks)
    string sSubDesc = "Processing: ";
    string sSubStatus = ARG_IDLE;
    
    if (!bBooted)
    {
        sSubStatus = ARG_BOOTING;
        FindNewAirlocks();                                                          // Search for new created airlock systems in the grid
        for (int i = 0; i < Als.Count; i++)                                         // go throug all airlock systems...
        {
            string sInitArg = Als[i].sName + ":" + CMD_INIT;                        // create init argument for this airlock system
            SetCommand(sInitArg);                                                   // set init command
        }
        bBooted = true;                                                             // Set BOOTED FLAG
    }
    
    if ((iSubTicks == 0 && argument == "") || argument == ARG_SEARCHING || DEBUG_FIND)
    {
        sSubStatus = ARG_SEARCHING;
        FindNewAirlocks();                                                          // Search for new created airlock systems in the grid
    }
    
    if ((iSubTicks == 5 && argument == "") || argument == ARG_UPDATE)
    {
        sSubStatus = ARG_UPDATE;
        UpdateExistingAirlocks();                                                   // Update blocks of all AirlockSystems
    }
    
    if (argument != "" && sSubStatus == ARG_IDLE)
    {
        sSubStatus = "CMD: '" + argument + "'";
    }
    
    Echo(sSubDesc + sSubStatus);                                                    // Write "Processing" line
    Echo("Airlocks found: " + Als.Count);                                           // Write number of current AirlockSystems
    Echo("O2 total filling level: "
         + (fOxygenTankFillRatio*100.00f).ToString("0.00") + " %");                 // Write number of current O2 Filling level
    WriteMyDisplay(sSubDesc + sSubStatus);                                          // Write text to display of programmable block
    
    
    // Set argument to airlock system
    if (argument != "")
    {
        switch (argument)
        {
            case CMD_DEBUG_ON:
                DEBUG_CLASS_ECHO = true;
                break;
                
            case CMD_DEBUG_OFF:
                DEBUG_CLASS_ECHO = false;
                break;
            default:
                SetCommand(argument);
                break;
        }
    }
    
    
    // TICK: Run Airlock systems
    for (int i = 0; i < Als.Count; i++) Als[i].Run(fOxygenTankFillRatio);           // Run each airlock system
    
    
    // DEBUG OUTPUT FROM THE CLASSES cAirlock
    if (DEBUG_CLASS_ECHO) Echo("\nList of airlocks (DEBUG ON):");
    else Echo("\nList of airlocks:");
    for (int i = 0; i < Als.Count; i++)
    {
        string sMyDebugClassOut = "  " + Als[i].sName + ": ";
        sMyDebugClassOut += Als[i].MyDebugReturn(DEBUG_CLASS_ECHO);
        Echo(sMyDebugClassOut);
        
        if (DEBUG_CLASS_ECHO && i<Als.Count-1) Echo("\n");
    }
}



// Writes a text on the textpanel on the programmable block where this script is running
void WriteMyDisplay(string s_AdditionalLine)
{
    string sPromt = "heroldini@" + Me.CubeGrid.CustomName + ": ~$";
    string sHashLine = "#############################################";
    string sMyTxt = sPromt + " sudo ./bin/airlocks";
           sMyTxt += "\n\n\n";
           sMyTxt += sHashLine + "\n";
           sMyTxt += "                             heroldini's airlock script\n";
           sMyTxt += sHashLine + "\n\n";
           sMyTxt += "      >> " + s_AdditionalLine;
           sMyTxt += "\n";
           sMyTxt += "      >> Airlocks found: " + Als.Count;
           sMyTxt += "\n";
           sMyTxt += "\n                                                                                    " + SCRIPT_VERSION;
           sMyTxt += "\n" + sHashLine;
           sMyTxt += "\n\n\n" + sPromt;
           if (iSubTicks < 5) sMyTxt += " _";
    
    IMyTextSurface oMyMainLcd = Me.GetSurface(0);
	oMyMainLcd.ContentType = ContentType.TEXT_AND_IMAGE;
    oMyMainLcd.Font = "DEBUG";
    oMyMainLcd.FontSize = 0.7f;
    oMyMainLcd.FontColor = new Color(0.0f, 1.0f, 0.0f);
    oMyMainLcd.BackgroundColor = new Color(0.0f, 0.01f, 0.0f);
	oMyMainLcd.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
	oMyMainLcd.WriteText(sMyTxt);
}




// ---------------------------------------- FUNCTIONS AIRLOCK LIST ----------------------------------------------

// search for new created airlock systems in the grid and add it to the list of airlock systems
public void FindNewAirlocks()
{
    List<IMyTerminalBlock> olFound = new List<IMyTerminalBlock>();                  // create list for searching results
    GridTerminalSystem.GetBlocks(olFound);                                          // Get all Blocks
    int iTmpOxyTankCnt = 0;                                                         // TEMP VAR to calc oxygen tank fill ratio (counter)
    double fTmpOxyTankFillRatio = 0.0;                                              // TEMP VAR to calc oxygen tank fill ratio (all over fillratio sum)
    
    
    // Find and add new airlock systems to the list of airlocks
    for (int i = 0; i < olFound.Count; i++)                                         // go trough list of found blocks...
    {
        if (olFound[i].CubeGrid.CustomName != Me.CubeGrid.CustomName) continue;     // if block is not from this grid -> ignore it
        
        
        // Get OXYGEN TANK fill statue
        if (IsBlockType(olFound[i], TYPE_OXYGENTANK)
        &&  olFound[i].BlockDefinition.SubtypeId == "")                             // Is oxygen tank...
        {
            var oOxyTank = (IMyGasTank)olFound[i];                                  // Get oxygen tank object
            iTmpOxyTankCnt++;                                                       // Count oxygen tanks
            fTmpOxyTankFillRatio += oOxyTank.FilledRatio;                           // get fill level 0.0-1.0 and add it to the current data
        }
        
        
        // DEBUGGING STUFF
        //Echo("Type: " + olFound[i].BlockDefinition.TypeIdString);                   //DEBUG Find TypeIds
        //Echo("Subtype: " + olFound[i].BlockDefinition.SubtypeId);                   //DEBUG Find SubTypeIds
        
        
        //// BUTTON PANELS [TEST - not possibility?]:
        //if (IsBlockType(olFound[i], TYPE_BUTTONPANEL) && IsBlockSubtype(olFound[i], SUBTYPE_ButtonPanelLarge))
        //{
        //    var oBtnPanel = (IMyButtonPanel)olFound[i];
        //    Echo("PANEL: " + olFound[i].CustomName);
        //    for (int j=0; j<4; j++)
        //    {
        //        Echo("  -> Btn " + j + ": " + oBtnPanel.GetButtonName(j));
        //    }
        //}
        
        
        // Find Tag-Data in name or costum data
        string sAirLName = GetTagDataStr(olFound[i].CustomName, olFound[i].CustomData, 1);
        if (sAirLName == "") continue;                                              // if  no name found, do nothing with this block
        
        // process block by TagData
        var oNewBlock = olFound[i];                                                 // block as var to use ref params
        int iAlsIdx = AirlockSystemExists(sAirLName);                               // get list index of current airlock system or -1
        if (iAlsIdx == -1)                                                          // if no Airlock system found...
        {
            cAirlock oNew = new cAirlock(sAirLName);                                // create new airlock system
            Als.Add(oNew);                                                          // add it to the list
            Als[Als.Count-1].AddBlock(ref oNewBlock);                               // add the block to the new airlock system
        }
        else
        {
            Als[iAlsIdx].AddBlock(ref oNewBlock);                                   // add block to existing airlock system (func checks itself if block already in the internal list)
        }//if AlsIdx == -1
    }//for olFound[i]
    
    
    // Calculate fill ratio of all oxygen tanks together
    fOxygenTankFillRatio = (float)(fTmpOxyTankFillRatio / (double)iTmpOxyTankCnt);
}



// updates all existing AirlockSystems in the list (search for new blocks or remove none existing from the systems)
public void UpdateExistingAirlocks()
{
    for (int i = Als.Count-1; i >= 0 ; i--)                                         // go trough all existing airlocksystems (backwards because of deleting)...
        if (!Als[i].UpdateBlockList()) Als.RemoveAt(i);                             // update blocks, delete from list if AirlockSystem no more exists...
}



// Checks if airlock already exists in the list of airlock systems
// Returns array index if AirlockSystem is found in the list OR -1 if not OR -2 if param is empty string ""
//          s_Name      The name of an AirlockSystem which should be checked if is already in the list
public int AirlockSystemExists(string s_Name)
{
    if (s_Name == "") return -2;
    for (int i = 0; i < Als.Count; i++)                                             // go trough list of airlock systems...
        if (Als[i].sName == s_Name) return i;                                       // if AirlockSystem is found, return index
    return -1;                                                                      // Nothing found, return -1
}



// Reads an argument and try to set it to the named airlock system
//          s_Argument              The program argument, format: "AirlockSysName:mode"
public void SetCommand(string s_Argument)
{
    int iIdx = AirlockSystemExists(s_Argument.Split(sTCut)[0]);                     // Get airlocksystemname from argument, search for airlock system in list
    if (iIdx > -1) Als[iIdx].SetMode(s_Argument.Split(sTCut)[1]);                   // Get parameter from argument and set it to airlock system
}



// Cuttes out the clear airlock name string + additional information
// Returns the clear airlock name string + additional infos
//          s_Name      Name of the Block
public static string GetClearStr(string s_Name)
{
    int iStart = s_Name.IndexOf(sTStart);                                           // get pos of start
    int iEnd = s_Name.IndexOf(sTEnd);                                               // gen pos of end
    if (iEnd < 0) return "";                                                        // nothing found -> end function
    int iEndLen = s_Name.Length - iEnd;                                             // calc length of end part which should be cutted
    
    if (iEnd < iStart) return "";                                                   // if end is befor start -> return empty string ""
    
    string sCutted = s_Name;                                                        // copy from parameter
    sCutted = sCutted.Remove(iEnd, iEndLen);                                        // remove chars after
    sCutted = sCutted.Remove(0, iStart +1);                                         // remove chars befor
    
    return sCutted;                                                                 // Return result
}



// Find Tag-Data in name or costum data
// Returns the TagData as string OR an empty string "" if nothing found
//          s_Name          BlockName
//          s_CostumData    CostumData of the block
//          i_SplitIndex    Index of split, where the data of interesst present
public static string GetTagDataStr(string s_Name, string s_CostumData, int i_SplitIndex)
{
    List<string> asData = new List<string>();
    asData.Add(s_Name);                                                             // Get name
    asData.Add(s_CostumData);                                                       // Get costum data
    
    string sTagData = "";
    foreach (string TextData in asData)                                             // check all strings...
    {
        sTagData = GetClearStr(TextData);                                           // get clear string
        if (sTagData != "") break;                                                  // if data found -> end loop
    }//foreach
    
    if (sTagData == "") return "";                                                  // if no name found, return FAIL
    if (sTagData.IndexOf(sTCut) == -1) return "";                                   // if there is no cut char, return FAIL
    var asTagDatas = sTagData.Split(sTCut);                                         // make array
    if (asTagDatas[0] != sTName) return "";                                         // if tag is not for this script -> return FAIL
    if (asTagDatas.Length-1 < i_SplitIndex) return "";                              // Check if search index exists, if not return FAIL
    return asTagDatas[i_SplitIndex];                                                // Return Data
}



// Checks if block already exists
// Returns TRUE if it does, FALSE if not
//          o_Block     The IMyTerminalBlock
public static bool BlockExists(IMyTerminalBlock o_Block)
{
    if (o_Block == null || o_Block.CubeGrid.GetCubeBlock(o_Block.Position) == null)
        return false;
    return true;
}



// Checks if Block is a specific type
// Returns TRUE if is, FALSE if not
//          o_Block                 Blockobjekt
//          s_BlockTypeConst        String constant "TYPE_..."
public static bool IsBlockType(IMyTerminalBlock o_Block, string s_BlockTypeConst)
{
    if (String.Compare(o_Block.BlockDefinition.TypeIdString, s_BlockTypeConst, false) == 0) return true;
    return false;
}



// Checks if Block is a specific sub type
// Returns TRUE if is, FALSE if not
//          o_Block                 Blockobjekt
//          s_BlockTypeConst        String constant "SUBTYPE_..."
public static bool IsBlockSubtype(IMyTerminalBlock o_Block, string s_BlockSubtypeConst)
{
    if (String.Compare(o_Block.BlockDefinition.SubtypeId, s_BlockSubtypeConst, false) == 0) return true;
    return false;
}




// ---------------------------------------- CLASS AIRLOCK SYSTEM ----------------------------------------------

// CLASS for one complete Airlock
public class cAirlock : List<IMyTerminalBlock>
{
    public string sName;
    private string sMode;
    private int iState;
    private int iToggleMode;
    private string sLastLcdTxt;
    private string sSaveLastLcdTxt;
    private int iLastLcdCol;
    private int iSaveLastLcdCol;
    private string sLastErrTxt;
    private int iTimer;
    private string sDebugString;
    private string sErrorLeaking;
    private bool bPreventLeakError;
    private float fOxygenFillRatio;
    private float fRememberLastVentRatio;
    private int iVentHangedCnt;
    private bool bVentHanged;
    
    public string sDebug;
    
    private Color Green = new Color(0.0f, 1.0f, 0.0f);
    private Color DarkGreen = new Color(0.0f, 0.01f, 0.0f);
    private Color Red = new Color(1.0f, 0.0f, 0.0f);
    private Color DarkRed = new Color(0.01f, 0.0f, 0.0f);
    private Color Yellow = new Color(1.0f, 1.0f, 0.0f);
    private Color DarkYellow = new Color(0.01f, 0.01f, 0.0f);
    
    private Color NormalLight = new Color(0.25f, 0.75f, 1.0f);
    
    
    
    // CONSTRUCTOR
    public cAirlock(string s_Name)
    {
        sName = s_Name;                                                             // save the name of this Airlocksystem
        
        iState = 0;                                                                 // init state (of mode)
        iToggleMode = 0;                                                            // valid toggle mode INIT
        sMode = CMD_IDLE;                                                           // beginn idle mode
        SetLcdText(COL_YELLOW, "\n" + TEXT_INACTIVE);                               // init lcd last settings
        sDebugString = "";
        sErrorLeaking = "";
        bPreventLeakError = false;
        fRememberLastVentRatio = -3.0f;
        iVentHangedCnt = 0;
        bVentHanged = false;
    }
    
    
    
    // searches in the grid, if new blocks should be added to the current airlock system, removes none existing blocks
    // Returns FALSE if no more blocks exists (it AirlockSystem can be removed from the list) or TRUE if still exists
    public bool UpdateBlockList()
    {
        // Check if Blocks exists
        for (int i = this.Count-1; i >= 0; i--)                                     // go trough block list...
        {
            IMyTerminalBlock oMyBlock = this[i];
            if (!BlockExists(oMyBlock))
            {
                this.RemoveAt(i);                                                   // If block is destroyed, remove it from Blocklist
            }
            else//Block still exists...
            {
                string sMyName = GetTagDataStr(oMyBlock.CustomName,
                                               oMyBlock.CustomData, 1);             // Get Name from tag
                if (sName != sMyName) this.RemoveAt(i);                             // Check if user still assigned the block to this airlock system by name
            }
        }
        
        
        // Return
        if (this.Count == 0) return false;                                          // check if any block still exists... (if not -> return FALSE to delte it from list of AirlockSystems)
        return true;
    }
    
    
    
    // Adds Blocks to the block list (checks if already exists)
    //          o_Block     The IMyTerminalBlock
    public void AddBlock(ref IMyTerminalBlock o_Block)
    {
        if (-1 == GetBlockListId(ref o_Block))
        {
            this.Add(o_Block);                                                      // If its not in the block list, add it
            
            if (HIDE_IN_TERMINAL) o_Block.ShowInTerminal = false;
            if (HIDE_IN_TOOLBAR_MENU) o_Block.ShowInToolbarConfig = false;
            
            if (IsBlockType(o_Block, TYPE_TEXTPANEL))                               // If block is LCD...
                SetLcdText(iLastLcdCol, sLastLcdTxt);                               // Write rewrite all lcds with last text (including the new one)
        }
    }
    
    
    
    // Search a block in the blocklist
    // Returns the array index of the block in the list OR -1 if not exists
    //          o_Block     The IMyTerminalBlock
    private int GetBlockListId(ref IMyTerminalBlock o_Block)
    {
        for (int i = 0; i < this.Count; i++)
            if (this[i].EntityId == o_Block.EntityId) return i;
        return -1;
    }
    
    
    
    // Counts the blocks of a specific type
    //          s_BlockTypeConst        String constant of IsBlockType(...)
    private int CountBlockType(string s_BlockTypeConst)
    {
        int iCnt = 0;
        for (int i = 0; i < this.Count; i++)
        {
            var oBlock = this[i];
            if (IsBlockType(oBlock, s_BlockTypeConst)) iCnt++;
        }
        
        return iCnt;
    }
    
    
    
    // Set a new mode (if possible)
    //          s   Command
    public void SetMode(string s)
    {
        if (sMode == s) return;                                                     // if new mode is the current mode -> do nothing
        
        // set new mode
        if (sMode == CMD_IDLE
        || sMode == CMD_TOGGLE
        || sMode == CMD_FAST)
        {
            sMode = s;
            iState = 0;
            if (s != CMD_TOGGLE) iToggleMode = 0;
            if (s == CMD_INIT) bPreventLeakError = true;
        }
    }
    
    
    
    // Runtime function
    //          f_OxygenFillRatio           The Stations O2 fill ratio (0.0 to 1.0)
    public void Run(float f_OxygenFillRatio = 0.5f)
    {
        fOxygenFillRatio = f_OxygenFillRatio;                                       // Save oxygen fillratio from the station
        
        
        // check if airlock is airthight (leaking error)
        if (!bPreventLeakError &&  GetPressureLevel() < 0.0f)
        {
            if (sErrorLeaking != ARG_ERR_LEAKING)                                   // check if save state is not already set (to prevent of permanently closing doors to be able to break out)
                DoSaveState("\n" + TEXT_ERR_NOTIGHT);
            sErrorLeaking = ARG_ERR_LEAKING;
            return;
        }
        else
        {
            // ResetLeakingError
            if (sErrorLeaking == ARG_ERR_LEAKING) ResetLeakingErrorSaveMode();
        }
        
        
        // do command
        switch (this.sMode)
        {
            case CMD_INIT:
                this.Init();
                break;
            case CMD_LOCK:
                this.Lock();
                break;
            case CMD_FREE:
                this.Free();
                break;
            case CMD_TOGGLE:
                this.Toggle();
                break;
            case CMD_FAST:
                this.Fast();
                break;
            case CMD_ERROR:
                this.ShowError();
                break;
            default:
                this.sMode = CMD_IDLE;
                this.iState = 0;
                break;
        }
    }
    
    
    
    // resets the leaking error (alarm off)
    private void ResetLeakingErrorSaveMode()
    {
        sErrorLeaking = "";
        sMode = CMD_IDLE;
        iState = 0;
        iToggleMode = 0;
        SetLcdText(COL_YELLOW, "\n" + TEXT_INACTIVE);
        SoundOnOff(OFF);
        SwitchLightOnOff(OFF, 0.0f, COL_YELLOW);
    }
    
    
    
    // Brings the airlock into the secured state
    //          s_ErrText       Text for the LCDs
    public void DoSaveState(string s_ErrText)
    {
        SetLcdText(COL_RED, s_ErrText);
        DoDoorOnOff(PARAM_INNER, ON);
        DoDoorOnOff(PARAM_OUTER, ON);
        DoDoorOpen(PARAM_INNER, CLOSE);
        DoDoorOpen(PARAM_OUTER, CLOSE);
        SetVentAction(OFF, ON);
        SwitchLightOnOff(ON, 0.3f, COL_RED);
        SoundOnOff(ON);
        
        sMode = CMD_ERROR;
        iState = 0;
    }
    
    
    
    // Process INIT mode
    private void Init()
    {
        switch (this.iState)
        {
            case 0:
                SetLcdText(COL_YELLOW, " " + TEXT_INIT + "\n");
                SwitchLightOnOff(ON, 0.3f, COL_YELLOW);
                if (GetPressureLevel() > 99.99f) this.iState = 20;
                else this.iState = 10;
                break;
            case 10:
                SoundOnOff(ON);
                DoDoorOnOff(PARAM_INNER, ON);
                DoDoorOnOff(PARAM_OUTER, ON);
                DoDoorOpen(PARAM_INNER, CLOSE);
                DoDoorOpen(PARAM_OUTER, CLOSE);
                this.iState = 11;
                break;
            case 11:
                if (GetDoorState(PARAM_INNER) == DOOR_CLOSED && GetDoorState(PARAM_OUTER) == DOOR_CLOSED)
                {
                    bPreventLeakError = false;
                    DoDoorOnOff(PARAM_OUTER, OFF);
                    SoundOnOff(OFF);
                    SetVentAction(ON, OFF);
                    SetLcdText(COL_YELLOW, " " + TEXT_INIT + "\n" + TXT_BAR);
                    this.iState = 13;
                }
                break;
            case 13:
                SetLcdText(COL_YELLOW, " " + TEXT_INIT + "\n" + TXT_BAR);
                if (GetPressureLevel(true) > 99.99f || fOxygenFillRatio < 0.000001f || bVentHanged) this.iState = 20;
                break;
            case 20:
                SetLcdText(COL_YELLOW, " " + TEXT_INIT + "\n");
                if (GetDoorState(PARAM_INNER) == DOOR_CLOSED)
                {
                    SoundOnOff(ON);
                }
                if (GetDoorState(PARAM_OUTER) == DOOR_CLOSED)
                {
                    DoDoorOnOff(PARAM_OUTER, OFF);
                    bPreventLeakError = false;
                }
                DoDoorOnOff(PARAM_INNER, ON);
                DoDoorOpen(PARAM_INNER, OPEN);
                this.iState = 30;
                break;
            case 30:
                if (GetDoorState(PARAM_INNER) == DOOR_OPEN)
                {
                    this.iToggleMode = 42;
                    SetLcdText(COL_GREEN, "\n" + TEXT_READY);
                    SwitchLightOnOff(ON, 0.0f, COL_NORMAL);
                    SoundOnOff(OFF);
                    this.sMode = CMD_IDLE;
                }
                break;
        }
    }
    
    
    
    // Process LOCK mode
    private void Lock()
    {
        switch (this.iState)
        {
            case 0:
                SetLcdText(COL_YELLOW, "\n" + TEXT_CLOSING);
                SwitchLightOnOff(ON, 0.3f, COL_RED);
                SoundOnOff(ON);
                DoDoorOnOff(PARAM_INNER, ON);
                DoDoorOnOff(PARAM_OUTER, ON);
                DoDoorOpen(PARAM_INNER, CLOSE);
                DoDoorOpen(PARAM_OUTER, CLOSE);
                this.iState = 20;
                break;
            case 20:
                if (GetDoorState(PARAM_INNER) == DOOR_CLOSED && GetDoorState(PARAM_OUTER) == DOOR_CLOSED)
                {
                    bPreventLeakError = false;
                    DoDoorOnOff(PARAM_INNER, OFF);
                    DoDoorOnOff(PARAM_OUTER, OFF);
                    SwitchLightOnOff(ON, 0.0f, COL_RED);
                    SoundOnOff(OFF);
                    SetLcdText(COL_RED, "\n" + TEXT_CLOSED);
                    this.sMode = CMD_IDLE;
                }
                break;
        }
    }
    
    
    
    // Process FREE mode
    private void Free()
    {
        switch (this.iState)
        {
            case 0:
                SetLcdText(COL_YELLOW, "\n" + TEXT_FREE);
                SwitchLightOnOff(ON, 0.3f, COL_YELLOW);
                SoundOnOff(ON);
                DoDoorOnOff(PARAM_INNER, ON);
                DoDoorOnOff(PARAM_OUTER, ON);
                DoDoorOpen(PARAM_INNER, OPEN);
                DoDoorOpen(PARAM_OUTER, OPEN);
                SetVentAction(OFF);
                this.iState = 20;
                break;
            case 20:
                if (GetDoorState(PARAM_INNER) == DOOR_OPEN && GetDoorState(PARAM_OUTER) == DOOR_OPEN)
                {
                    SetLcdText(COL_YELLOW, "\n" + TEXT_INACTIVE);
                    SwitchLightOnOff(ON, 0.0f, COL_NORMAL);
                    SoundOnOff(OFF);
                    this.sMode = CMD_IDLE;
                }
                break;
        }
    }
    
    
    
    // Process TOGGLE mode
    private void Toggle()
    {
        // check door state and toggle mode
        if (GetDoorState(PARAM_INNER) == DOOR_OPEN && GetDoorState(PARAM_OUTER) == DOOR_CLOSED && this.iToggleMode == 42)      // from inside (1)
            this.iToggleMode = 1;

        else if (GetDoorState(PARAM_INNER) == DOOR_CLOSED && GetDoorState(PARAM_OUTER) == DOOR_OPEN && this.iToggleMode == 42) // from outside (2)
            this.iToggleMode = 2;
        
        else if (this.iToggleMode == 1 || this.iToggleMode == 2)
        {
            //Nothing more to do, but its needed to select this case!  -->  this.iToggleMode = this.iToggleMode;
        }
        else
            this.iToggleMode = 41;  // not in toggle mode
        
        // from inside
        if (this.iToggleMode == 1)
        {
            switch (this.iState)
            {
                case 0:
                    SetLcdText(COL_YELLOW, "\n" + TEXT_TOGGLE);
                    DoDoorOpen(PARAM_INNER, CLOSE);
                    SwitchLightOnOff(ON, 0.3f, COL_YELLOW);
                    SoundOnOff(ON);
                    this.iState = 10;
                    break;
                case 10:
                    if (GetDoorState(PARAM_INNER) == DOOR_CLOSED)
                    {
                        DoDoorOnOff(PARAM_INNER, OFF);
                        SoundOnOff(OFF);
                        SetVentAction(ON, ON);
                        SetLcdText(COL_YELLOW, " " + TEXT_DEPRESS + "\n" + TXT_BAR);
                        this.iState = 30;
                    }
                    break;
                case 30:
                    SetLcdText(COL_YELLOW, " " + TEXT_DEPRESS + "\n" + TXT_BAR);
                    if (GetPressureLevel(true) < 0.01f || fOxygenFillRatio > 0.999999f || bVentHanged) this.iState = 40;
                    break;
                case 40:
                    SetLcdText(COL_YELLOW, "\n" + TEXT_TOGGLE);
                    DoDoorOnOff(PARAM_OUTER, ON);
                    SetVentAction(OFF);
                    SoundOnOff(ON);
                    DoDoorOpen(PARAM_OUTER, OPEN);
                    this.iState = 50;
                    break;
                case 50:
                    if(GetDoorState(PARAM_OUTER) == DOOR_OPEN)
                    {
                        SetLcdText(COL_RED, "\n" + TEXT_DEPRESSED);
                        SwitchLightOnOff(ON, 0.0f, COL_NORMAL);
                        SoundOnOff(OFF);
                        this.iToggleMode = 42;
                        this.sMode = CMD_IDLE;
                    }
                    break;
            }
        }
        
        
        // from outside
        if (this.iToggleMode == 2)
        {
            switch (this.iState)
            {
                case 0:
                    SetLcdText(COL_YELLOW, "\n" + TEXT_TOGGLE);
                    SwitchLightOnOff(ON, 0.3f, COL_YELLOW);
                    SoundOnOff(ON);
                    DoDoorOpen(PARAM_OUTER, OFF);
                    this.iState = 10;
                    break;
                case 10:
                    if (GetDoorState(PARAM_OUTER) == DOOR_CLOSED)
                    {
                        bPreventLeakError = false;
                        DoDoorOnOff(PARAM_OUTER, OFF);
                        SoundOnOff(OFF);
                        SetVentAction(ON, OFF);
                        SetLcdText(COL_YELLOW, " " + TEXT_PRESS + "\n" + TXT_BAR);
                        this.iState = 30;
                    }
                    break;
                case 30:
                    SetLcdText(COL_YELLOW, " " + TEXT_PRESS + "\n" + TXT_BAR);
                    if (GetPressureLevel(true) > 99.99f || fOxygenFillRatio < 0.000001f || bVentHanged) this.iState = 40;
                    break;
                case 40:
                    SetLcdText(COL_YELLOW, "\n" + TEXT_TOGGLE);
                    DoDoorOnOff(PARAM_INNER, ON);
                    SetVentAction(OFF);
                    SoundOnOff(ON);
                    DoDoorOpen(PARAM_INNER, OPEN);
                    this.iState = 50;
                    break;
                case 50:
                    if (GetDoorState(PARAM_INNER) == DOOR_OPEN)
                    {
                        SetLcdText(COL_GREEN, "\n" + TEXT_PRESSED);
                        SwitchLightOnOff(ON, 0.0f, COL_NORMAL);
                        SoundOnOff(OFF);
                        SetLcdText(COL_GREEN, "\n" + TEXT_READY);
                        this.iToggleMode = 42;
                        this.sMode = CMD_IDLE;
                    }
                    break; 
            }
        }
        
        
        // try toggle in wrong mode
        if (this.iToggleMode == 41)
        {
            sMode = CMD_INIT;
            iState = 0;
        }
    }
    
    
    
    // Process ShowError
    //          s_ErrText       String to display on LCD
    private void ShowError(string s_ErrText = "")
    {
        if (s_ErrText != "") sLastErrTxt = s_ErrText;
        switch (this.iState)
        {
            case 0:
                this.sMode = CMD_ERROR;
                sSaveLastLcdTxt = this.sLastLcdTxt;
                iSaveLastLcdCol = this.iLastLcdCol;
                SetLcdText(COL_RED, sLastErrTxt);
                iTimer = 7;
                this.iState = 10;
                break;
            case 10:
                if (--iTimer <= 0) this.iState = 20;
                break;
            case 20:
                SetLcdText(iSaveLastLcdCol, sSaveLastLcdTxt);
                this.sMode = CMD_IDLE;
                break;
        }
    }
    
    
    
    // Process FAST mode
    private void Fast()
    {
        
    }
    
    
    
    // Switch sound ON or OFF
    //          b_On        ON  or  OFF
    private void SoundOnOff(bool b_On)
    {
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is sound block
            var oBlock = this[i];
            if (IsBlockType(oBlock, TYPE_SOUNDBLOCK))
            {
                var oSound = (IMySoundBlock) this[i];                               // Get sound object
                if (!b_On)                                                          // if param says "OFF"...
                {
                    oSound.Stop();                                                  // switch off
                }
                else//param says ON
                {
                    if (oSound.DetailedInfo == "")                                  // if no detailed info is written (=Sound is off) ...
                    {
                        if (HasGreatDoors())
                            oSound.SelectedSound = SOUND_BLOCK_HANGAR;
                        else
                            oSound.SelectedSound = SOUND_BLOCK_NORMAL;              // select sound
                        oSound.LoopPeriod = 1800;                                   // time 30 min
                        oSound.Play();                                              // switch on
                    }
                }//if not on
            }//if
        }//for
    }
    
    
    
    // Checks if airlock has big doors (Hangar doors)
    // Returns TRUE if hangar doors exists, FALSE of not
    private bool HasGreatDoors()
    {
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is door
            var oBlock = this[i];
            if (IsBlockType(oBlock, TYPE_HANGARDOOR))
                return true;
        }
        
        return false;
    }
    
    
    
    // Switch lights ON or OFF
    //          b_On            ON  or  OFF
    //          f_Intervall     OPTIONAL    blink inteval in seconds
    //          i_ColorConst    OPTIONAL    COL_GREEN  or  COL_RED  or  COL_YELLOW
    private void SwitchLightOnOff(bool b_On, float f_Interval = 0.0f, int i_ColorConst = 0)
    {
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is light
            var oBlock = this[i];
            if (IsBlockType(oBlock, TYPE_INTERIORLIGTH)
             || IsBlockType(oBlock, TYPE_REFLECTORLIGTH))
            {
                var oLight = (IMyLightingBlock)this[i];                             // Get light object
                oLight.Enabled = b_On;                                              // lights on/off
                oLight.BlinkIntervalSeconds = f_Interval;                           // set blink interval
                switch (i_ColorConst)                                               // set color
                {
                    case COL_GREEN:
                        oLight.Color = Green;
                        break;
                    case COL_RED:
                        oLight.Color = Red;
                        break;
                    case COL_YELLOW:
                        oLight.Color = Yellow;
                        break;
                    default:
                        oLight.Color = NormalLight;
                        break;
                }
            }//if
        }//for
    }
    
    
    
    // Reads the door state
    // Returns the state: 0=nothing found,  1=OPEN,  2=CLOSED
    //          s_InOut     PARAM_INNER  or  PARAM_OUTER
    private int GetDoorState(string s_InOut)
    {
        int iAllOpen = 0;
        int iAllClosed = 0;
        int iFound = 0;
        
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is door
            var oBlock = this[i];
            if (GetTagDataStr(oBlock.CustomName, oBlock.CustomData, 2) == s_InOut &&
                (IsBlockType(oBlock, TYPE_DOOR)
              || IsBlockType(oBlock, TYPE_HANGARDOOR)
              || IsBlockType(oBlock, TYPE_SLIDEDOOR)) )
            {
                var oDoor = (IMyDoor) this[i];                                      // Get door object
                iFound++;                                                           // Count all doors
                switch (oDoor.Status)                                               // return door state
                {
                    case Sandbox.ModAPI.Ingame.DoorStatus.Open: iAllOpen++;
                        break;
                    case Sandbox.ModAPI.Ingame.DoorStatus.Closed: iAllClosed++;
                        break;
                    default:
                        break;
                }//switch
            }//if
        }//for
        //sDebugString = "AllO: " + iAllOpen + " AllC: " + iAllClosed + " All: " + iFound;
        if (iFound == iAllOpen) return DOOR_OPEN;                                   // if all doors open, return open
        if (iFound == iAllClosed) return DOOR_CLOSED;                               // if all doors closed, return closed
        return DOOR_NOTFOUND;                                                       // return 0 if nothing found
    }
    
    
    
    // Switch doors ON or OFF
    //          s_InOut     PARAM_INNER  or  PARAM_OUTER
    //          b_On        ON  or  OFF
    private void DoDoorOnOff(string s_InOut, bool b_On)
    {
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is door
            var oBlock = this[i];
            if (GetTagDataStr(oBlock.CustomName, oBlock.CustomData, 2) == s_InOut &&
                (IsBlockType(oBlock, TYPE_DOOR)
              || IsBlockType(oBlock, TYPE_HANGARDOOR)
              || IsBlockType(oBlock, TYPE_SLIDEDOOR)) )
            {
                var oDoor = (IMyDoor) this[i];                                      // Get door object
                oDoor.Enabled = b_On;                                               // open/close
            }//if
        }//for
    }
    
    
    
    // Opening or closing doors
    //          s_InOut     PARAM_INNER  or  PARAM_OUTER
    //          b_Open      OPTIONAL OPEN  or  CLOSE
    private void DoDoorOpen(string s_InOut) { DoDoorOpen(s_InOut, false, true); }
    private void DoDoorOpen(string s_InOut, bool b_Open, bool b_IgnoreOpenParam = false)
    {
        // disable leaking detection on opening outer door
        if (s_InOut == PARAM_OUTER && b_Open) bPreventLeakError = true;
        
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is door
            var oBlock = this[i];
            if (GetTagDataStr(oBlock.CustomName, oBlock.CustomData, 2) == s_InOut &&
                (IsBlockType(oBlock, TYPE_DOOR)
              || IsBlockType(oBlock, TYPE_HANGARDOOR)
              || IsBlockType(oBlock, TYPE_SLIDEDOOR)) )
            {
                var oDoor = (IMyDoor) this[i];                                      // Get door object
                if (b_IgnoreOpenParam)                                              // Open/close/toggle
                {
                    oDoor.ToggleDoor();
                }
                else
                {
                    if (b_Open) oDoor.OpenDoor();
                    else oDoor.CloseDoor();
                }//if b_IgnoreOpenParam
            }//if
        }//for
    }
    
    
    
    // Search for the first air vent
    // Returns the oxygen level 0.0 to 100.0 (-1.0 if nothing airvent found, -2.0 if cant pressurize)
    //          bUseHangedLogic         set to TRUE if the pressure level is used for waiting on airlock toggle
    private float GetPressureLevel(bool bUseHangedLogic = false)
    {
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is Airvent
            var oBlock = this[i];
            if (IsBlockType(oBlock, TYPE_AIRVENT))
            {
                IMyAirVent oVent = (IMyAirVent) this[i];                            // Get airvent object
                if (!oVent.CanPressurize) return -2.0f;                             // return error, if it cant pressurize
                float fMyLvl = oVent.GetOxygenLevel()*100.0f;                       // Get O2 level
                
                
                // check if vent hanged
                if (bUseHangedLogic)                                                // use hanged logic...
                {
                    if (fRememberLastVentRatio == fMyLvl)                           // If level is exactly the same as last time...
                    {
                        iVentHangedCnt++;                                           // increase error counter
                        if (iVentHangedCnt >= VENT_HANGED_CNT) bVentHanged = true;  // set hanged state, if counter exceeds
                    }
                }
                
                
                // Reset vent hanged error
                if (fRememberLastVentRatio != fMyLvl)
                {
                    iVentHangedCnt = 0;                                             // reset error counter
                    bVentHanged = false;                                            // reset hanged state
                }
                
                fRememberLastVentRatio = fMyLvl;                                    // Remember current value next time
                
                
                // return
                return fMyLvl;                                                      // Return O2 level
            }
        }
        
        return -1.0f;
    }
    
    
    
    // Change the vents action
    //          bOn                 AirVent ON or OFF
    //          bDepressurize       OPTIONAL Depressurizing ON or OFF
    private void SetVentAction(bool bOn) { SetVentAction(bOn, false, true); }
    private void SetVentAction(bool bOn, bool bDepressurize, bool bIgnoreDepressurize = false)
    {
        for (int i = 0; i < this.Count; i++)                                        // go through all blocks...
        {
            var oBlock = this[i];                                                   // get block object
            if (IsBlockType(oBlock, TYPE_AIRVENT))                                  // if its vent block...
            {
                var oVent = (IMyAirVent)this[i];                                    // get vent object
                oVent.Enabled = bOn;                                                // set vent on or off
                if (!bIgnoreDepressurize) oVent.Depressurize = bDepressurize;       // if depressurize param is set - set to block
            }//if IsBlockType
        }//for
    }
    
    
    
    // Sets a text to the text panels
    private void SetLcdText(int i_ColorConst, string s_Text)
    {
        // Select color
        Color colFont, colBg;
        switch (i_ColorConst)
        {
            case COL_GREEN:
                colFont = Green;
                colBg = DarkGreen;
                break;
                
            case COL_RED:
                colFont = Red;
                colBg = DarkRed;
                break;
                
            default:
                colFont = Yellow;
                colBg = DarkYellow;
                break;
        }
        
        // Split text into its parts
        var asLineParts = s_Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None);
        bool bOneLine = (asLineParts.Length < 2);
        
        
        // Set all LCDs
        for (int i = 0; i < this.Count; i++)
        {
            // Check if block is LCD
            var oBlock = this[i];
            
            
            // set common settings
            IMyTextSurface oLcd;
            if (IsBlockType(oBlock, TYPE_TEXTPANEL))
            {
                oLcd = (IMyTextSurface) this[i];                                    // Get lcd object
                oLcd.ContentType = ContentType.TEXT_AND_IMAGE;                      // Set to text mode
                oLcd.FontColor = colFont;                                           // Set text color
                oLcd.BackgroundColor = colBg;                                       // Set background color
                oLcd.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;     // Text center
                
                
                // Corner text panels
                if (IsBlockSubtype(oBlock, SUBTYPE_LargeBlockCorner_LCD_1)
                ||  IsBlockSubtype(oBlock, SUBTYPE_LargeBlockCorner_LCD_2)
                ||  IsBlockSubtype(oBlock, SUBTYPE_LargeBlockCorner_LCD_Flat_1)
                ||  IsBlockSubtype(oBlock, SUBTYPE_LargeBlockCorner_LCD_Flat_2))
                {
                    if (bOneLine)                                                   // If its just 1 line...
                    {
                        oLcd.FontSize = 4;                                          // Font size
                        oLcd.WriteText(asLineParts[0]);                             // Write text to panel
                    }
                    else
                    {
                        oLcd.FontSize = 3.8f;                                       // Font size
                        asLineParts[1] = GetLoadingBar(60, asLineParts[1]);         // Get Loadingbar
                        oLcd.WriteText(asLineParts[0] + "\n" + asLineParts[1]);     // Write text to panel
                    }
                }
                
                
                // LCD panel (= fills the block surface)
                if (IsBlockSubtype(oBlock, SUBTYPE_LargeLCDPanel))
                {
                    if (bOneLine)                                                   // If its just 1 line...
                    {
                        oLcd.FontSize = 1.4f;                                       // Font size
                        oLcd.WriteText(asLineParts[0]);                             // Write text to panel
                    }
                    else
                    {
                        oLcd.FontSize = 1.2f;                                       // Font size
                        asLineParts[1] = GetLoadingBar(60, asLineParts[1]);         // Get Loadingbar
                        oLcd.WriteText("\n\n\n\n\n"
                                     + asLineParts[0] + "\n"
                                     + asLineParts[1]);                             // Write text to panel
                    }
                }
                
                
                // Text panel (= small 1-block screen)
                if (IsBlockSubtype(oBlock, SUBTYPE_LargeTextPanel))
                {
                    if (bOneLine)                                                   // If its just 1 line...
                    {
                        oLcd.FontSize = 1.4f;                                       // Font size
                        oLcd.WriteText(asLineParts[0]);                             // Write text to panel
                    }
                    else
                    {
                        oLcd.FontSize = 1.2f;                                       // Font size
                        asLineParts[1] = GetLoadingBar(60, asLineParts[1]);         // Get Loadingbar
                        oLcd.WriteText("\n\n"
                                     + asLineParts[0] + "\n"
                                     + asLineParts[1]);                             // Write text to panel
                    }
                }
                
                
                // wide screen (= big 2-block screen)
                if (IsBlockSubtype(oBlock, SUBTYPE_LargeLCDPanelWide))
                {
                    if (bOneLine)                                                   // If its just 1 line...
                    {
                        oLcd.FontSize = 2.9f;                                       // Font size
                        oLcd.WriteText(asLineParts[0]);                             // Write text to panel
                    }
                    else
                    {
                        oLcd.FontSize = 2.5f;                                       // Font size
                        asLineParts[1] = GetLoadingBar(60, asLineParts[1]);         // Get Loadingbar
                        oLcd.WriteText("\n\n"
                                     + asLineParts[0] + "\n"
                                     + asLineParts[1]);                             // Write text to panel
                    }
                }
            }//IsBlockType
        }
        

        // save this color & text as "last color/text"
        sLastLcdTxt = s_Text;
        iLastLcdCol = i_ColorConst;
    }
    
    
    
    // calculate loadingbar
    //          i_MaxDash               Number of dashes
    //          s_ReplaceText           If there is an replace text, the loading bar is placed in the text where TXT_BAR is found. use empty string "" for not using
    //          f_PercentagePressure    OPTIONAL Percentage (if param is not set, it uses the pressure from the vents)
    private string GetLoadingBar(int i_MaxDash, string s_ReplaceText = " ", float f_PercentagePressure = -17.0f)
    {
        float fPerPress = f_PercentagePressure;                                     // Get percentage of pressure
        string sBar = "[";
        
        if (f_PercentagePressure == -17.0f) fPerPress = GetPressureLevel();         // If param == default -> get pressure from vents
        
        // validate input
        if (fPerPress > 100.0f) fPerPress = 100.0f;
        if (fPerPress < 0.0f) fPerPress = 0.0f;
        
        // calculate dashes
        int iLoad = (int)((float)i_MaxDash / 100.0f * fPerPress);
        int iRest = i_MaxDash - iLoad;
                
        // calc bar
        for (int i = 1; i <= iLoad; i++)
            sBar = sBar + "|";
        
        // calc rest
        for (int i = 1; i <= iRest; i++)
            sBar = sBar + "'";
        
        sBar = sBar + "]";
        
        if (s_ReplaceText != " ") sBar = s_ReplaceText.Replace(TXT_BAR, sBar);
                
        return sBar;
    }
    
    
    
    // DEBUG ECHO STRING
    public string MyDebugReturn(bool b_Debug = false)
    {
        string sTab = "\n    -> ";
        string sTmp = sMode;
        
        if (sErrorLeaking != "") sTmp += " " + sErrorLeaking;
        
        if (b_Debug)
        {
            sTmp += ":" + iState + ":" + iToggleMode + ":" + iTimer;
            if (sDebugString != "") sTmp += sTab + "Other: " + this.sDebugString;
            sTmp += sTab + "Pressure: " + GetPressureLevel().ToString("0.00") + " %";
            sTmp += sTab + "Blocks: " + this.Count;
            sTmp += sTab + "Vents: " + CountBlockType(TYPE_AIRVENT);
            sTmp += sTab + "Doors: " + (CountBlockType(TYPE_DOOR) + CountBlockType(TYPE_SLIDEDOOR) + CountBlockType(TYPE_HANGARDOOR));
            sTmp += sTab + "LCDs: " + CountBlockType(TYPE_TEXTPANEL);
            sTmp += sTab + "Light: " + (CountBlockType(TYPE_INTERIORLIGTH) + CountBlockType(TYPE_REFLECTORLIGTH));
            sTmp += sTab + "Sounds: " + CountBlockType(TYPE_SOUNDBLOCK);
            sTmp += sTab + "Leak Detection: " + !bPreventLeakError;
            sTmp += sTab + "Vent hanged: " + bVentHanged + " (" + iVentHangedCnt + "/" + VENT_HANGED_CNT + ")";
        }
        
        return sTmp;
    }
}




// ---------------------------------------- INTERNAL GLOBAL VARS & SETTINGS ----------------------------------------------

// GLOBAL WORKING VARS
int iTicks = 0;                                                                     // counts the ticks
int iSubTicks;                                                                      // Subticks = ticks%10
List<cAirlock> Als = new List<cAirlock>();                                          // create List of airlock systems
float fOxygenTankFillRatio = 0.0f;                                                  // Contains the fillratio of all oxygen tanks (0.0 to 1.0)
bool bBooted = false;                                                               // FLAG to save if all airlocks are initialized on script start

// SHORT CONSTANTS
const bool ON = true;
const bool OFF = false;
const bool OPEN = true;
const bool CLOSE = false;
const string TXT_BAR = "{BAR}";

// PROCESSING ARGUMENT STRINGS
const string ARG_SEARCHING = "SEARCHING";
const string ARG_UPDATE    = "   UPDATE";
const string ARG_IDLE      = "     IDLE";
const string ARG_BOOTING   = "  BOOTING";

const string ARG_ERR_LEAKING = "LEAKING!";

// Door states
const int DOOR_NOTFOUND = 0;
const int DOOR_OPEN = 1;
const int DOOR_CLOSED = 2;

// colors constants
const int COL_NORMAL = 0;
const int COL_GREEN = 1;
const int COL_RED = 2;
const int COL_YELLOW = 3;


// String constants to compare with IsBlockType()
const string OBJ_BUILD = "MyObjectBuilder_";
const string TYPE_AIRVENT = OBJ_BUILD + "AirVent";
const string TYPE_INTERIORLIGTH = OBJ_BUILD + "InteriorLight";
const string TYPE_REFLECTORLIGTH = OBJ_BUILD + "ReflectorLight";
const string TYPE_SLIDEDOOR = OBJ_BUILD + "AirtightSlideDoor";
const string TYPE_HANGARDOOR = OBJ_BUILD + "AirtightHangarDoor";    // IMyAirtightHangarDoor
const string TYPE_DOOR = OBJ_BUILD + "Door";                        // IMyDoor
const string TYPE_TEXTPANEL = OBJ_BUILD + "TextPanel";
const string TYPE_SOUNDBLOCK = OBJ_BUILD + "SoundBlock";
const string TYPE_OXYGENTANK = OBJ_BUILD + "OxygenTank";
//const string TYPE_BUTTONPANEL = OBJ_BUILD + "ButtonPanel";

// String constants to compare with IsBlockSubtype()
const string SUBTYPE_LargeBlockCorner_LCD_1 = "LargeBlockCorner_LCD_1";
const string SUBTYPE_LargeBlockCorner_LCD_2 = "LargeBlockCorner_LCD_2";
const string SUBTYPE_LargeTextPanel = "LargeTextPanel";
const string SUBTYPE_LargeLCDPanelWide = "LargeLCDPanelWide";
const string SUBTYPE_LargeLCDPanel = "LargeLCDPanel";
const string SUBTYPE_LargeBlockCorner_LCD_Flat_1 = "LargeBlockCorner_LCD_Flat_1";
const string SUBTYPE_LargeBlockCorner_LCD_Flat_2 = "LargeBlockCorner_LCD_Flat_2";
//const string SUBTYPE_ButtonPanelLarge = "ButtonPanelLarge";

// DEBUG Vars
const string SCRIPT_VERSION = "v1.1.4";
const bool DEBUG_FIND = false;
bool DEBUG_CLASS_ECHO = false;
