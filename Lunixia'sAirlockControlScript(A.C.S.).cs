/*
 * Lunixia's Airlock Control Script (A.C.S.)
 * ----------------------------------------------------------------
 * 
 * This script manages Airlock Groups
 * It is important to note that blocks should only be members of 1 airlock group.
 * This is not to say that blocks cannot be part of other groups in general, of course they can.
 * For example, Hangar Doors can be part of a regular group (for opening with sensor/button)
 *     as well as be a part of an airlock group. But dont add said Hangar Door group to another
 *     airlock group.
 * 
 * Airlock Groups must contains at least:
 *     1 Airvent with the ACS-CV tag - this vent holds configurations for the group.
 *     1 Door with the ACS-In tag
 *     1 Door with the ACS-Out tag
 * 
 * Airlock Configs
 *     Each group has a vent with the ACS-CV tag
 *     In the custom data there are a few settings that can be modified:
 *             Keep in mind there are 60 ticks in a second. Adjustments on each airlock maybe needed
 *             depending on doors used, how long they take to open, etc.
 * 
 *             InnerDoorDelay and OutterDoorDelay - default=90
 *             -How many game ticks should pass before closing Inner out Outter door(s).
 * 
 *             (De)PressurizeDelay -default=90
 *             -How many game ticks should pass to allow for a room to (de)pressurize.         
 *             
 *             AlwaysDepressurize - default=false
 *             -Should the vents in the group always depressurize no matter what.
 * 
 *             RESET - default=NO
 *             -Set this to YES if you need to reset the config to the default values
 * 
 * Door Status
 *     |=| : Closed
 *     |-| : Closing
 *     |+| : Opening
 *     | | : Opened
 *     IDs : Inner Door(s)
 *     ODs : Outter Doors(s)
 * 
 * Vent Status
 *     [+] : Pressurizing
 *     [-] : Depressurizing
 *     V   : Vent(s)
 * 
 * DIR Values
 *     I-B : Inbound
 *     O-B : Outbound
 *     Rdy : Ready
 * 
 * Airlock status is always read from left to right regardless of direction (Inbound/Outbound)
 *     If a player is Inbound, the door on the left will be the outter door
 *     If a player is Outbound, the door on the left will be the inner door
 *     Door status on the LCD is displayed as such:
 *     DIR   IDs   V   ODS
 *     Rdy   |=|  [-]  |=|
 * 
 * Programmable Block Arguments
 *     stop : stops the script.
 *     resetlocks : resets all airlocks (closes doors, and clears airlock progress)
 *     resetconfigs : resets all configs on all airlocks to their default values. (Vent with ACS-CV tag)
 */

//======================================================================================================
// Configurations
//======================================================================================================

// Group Tag, Include this in your Airlock Group name
string strGroupTag = "G:ACS";

// Door Tags
    // Include these tags for inner and outter doors respectively.
    // You can name multiple doors with their respective tags (example: hangar doors).
    string strInnerDoorTag = "ACS-In";
    string strOutterDoorTag = "ACS-Out";

// Vent Tags
    // If you have multiple vents, include this tag in all but one.
    string strAirventTag = "ACS-V";

    // Each group must have ONE vent with this tag.
    // If you have multiple vents, use this tag for ONE.
    string strAirventConfig = "ACS-CV";

// LCD tag for dianostics
    string strLCD_Diagnostics = "ACS-Diag";

    // Hides the G:ACS tag on the LCD to make more room for scaling.
    bool hideGroupTagOnLCD = true;

    // Monospace is used to make all characters equal width.
    // This helps keep the door status symbols in line with each other.
    // It is recommended to leave the font as is, but other properties can be changed is needed.
    // LCD default screen settings
    string lcdFont = "Monospace";
    float lcdFontSize = 0.5f;
    float lcdPadding = 2f;

//======================================================================================================
// MAIN CODE - NO TOUCHY UNLESS YOU KNOW WHAT YOU ARE DOING.
//======================================================================================================

// Default values for each door, each door can be configured individually in the CustomData of the Airvent its paired with.
int TicksUntilClose = 90; int TicksToAirVent = 90; int c = 0; int PlusTicks = 10;
string idd = "InnerDoorDelay:"; string odd = "OutterDoorDelay:";
string vdd = "(De)PressurizeDelay:"; string ad = "AlwaysDepressurize:";
string strCustomDataResetTag = "RESET"; string lcdDefaultsSet = "lcdDefaultsSet";
string[] prog = { "\\", "|", "/", "-" }; bool IsResettingAirlocks = false;
List<IMyBlockGroup> BlockGroups = new List<IMyBlockGroup>();
Dictionary<string, List<IMyTerminalBlock>> GroupNamesWithBlockList = new Dictionary<string, List<IMyTerminalBlock>>();
Dictionary<string, int> GroupNameInternalDoorTimers = new Dictionary<string, int>();
Dictionary<string, int> GroupNameExternalDoorTimers = new Dictionary<string, int>();
Dictionary<string, int> GroupNameInternalDoorTicks = new Dictionary<string, int>();
Dictionary<string, int> GroupNameExternalDoorTicks = new Dictionary<string, int>();
Dictionary<string, int> GroupNameOutBoundVentProc = new Dictionary<string, int>();
Dictionary<string, int> GroupNameInBoundVentProc = new Dictionary<string, int>();
Dictionary<string, int> GroupNameVentTimers = new Dictionary<string, int>();
Dictionary<string, int> GroupNameVentTicks = new Dictionary<string, int>();
Dictionary<string, bool> GroupVentAlwaysDepressurize = new Dictionary<string, bool>();
Dictionary<string, bool> GroupNameInternalDoorStatus = new Dictionary<string, bool>();
Dictionary<string, bool> GroupNameExternalDoorStatus = new Dictionary<string, bool>();
Dictionary<string, string> GroupNameWithStatus = new Dictionary<string, string>();
StringBuilder mainSB = new StringBuilder(); StringBuilder diagSB = new StringBuilder();
public Program() { Runtime.UpdateFrequency = UpdateFrequency.Update10; }
public void Save() { }
void DisplayDiagnosticLCD(string strText, string LCDName)
{
    List<IMyTextSurface> myLCDs = new List<IMyTextSurface>();
    GridTerminalSystem.GetBlocksOfType<IMyTextSurface>(myLCDs);
    foreach (IMyTextSurface lcd in myLCDs) {
        if (System.Text.RegularExpressions.Regex.IsMatch((lcd as IMyTerminalBlock).CustomName,
            LCDName,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        )
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch((lcd as IMyTerminalBlock).CustomData,
                lcdDefaultsSet,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            )
            {
                lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                lcd.Font = lcdFont;
                lcd.FontSize = lcdFontSize;
                lcd.TextPadding = lcdPadding;
                (lcd as IMyTerminalBlock).CustomData = lcdDefaultsSet;
            }
            lcd.WriteText(strText);
        }
    }
}
IMyAirVent MakeAirlockConfig (List<IMyAirVent> GroupBlocks) {
    List<IMyAirVent> Vents = new List<IMyAirVent>();
    foreach (IMyAirVent T in GroupBlocks) {
        if (System.Text.RegularExpressions.Regex.IsMatch(T.CustomName,
            strAirventConfig,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        ) { Vents.Add(T as IMyAirVent); break; }
    }
    if (System.Text.RegularExpressions.Regex.IsMatch(Vents[0].CustomData,
        $"{strCustomDataResetTag}:yes",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase)
    ) { Vents[0].CustomData = string.Empty; }
    if (Vents[0].CustomData.Length > 0) { return Vents[0]; }
    StringBuilder config = new StringBuilder();
    config.AppendLine($"{idd}{TicksUntilClose}");
    config.AppendLine($"{odd}{TicksUntilClose}");
    config.AppendLine($"{vdd}{TicksToAirVent}");
    config.AppendLine($"{ad}false");
    config.AppendLine($"{strCustomDataResetTag.ToUpper()}:NO");
    Vents[0].CustomData = config.ToString();
    return Vents[0];
}
float GetDoorOpenRatio (List<IMyDoor> Doors) {
    float Ratio = 0; int Total = 0;
    foreach (IMyDoor rd in Doors) { if (rd.OpenRatio > 0) { Total++; Ratio += rd.OpenRatio; } }
    return Ratio/Total;
}
float GetDoorClosedRatio (List<IMyDoor> Doors) {
    float Ratio = 0;
    foreach (IMyDoor rd in Doors) { if (rd.OpenRatio < 1) { Ratio += rd.OpenRatio; } }
    return Ratio;
}
IMyAirVent GetMembers (string GroupName, List<IMyTerminalBlock> tBlockList) {
    List<IMyAirVent> GroupVents = new List<IMyAirVent>();
    List<IMyDoor> GroupDoors = new List<IMyDoor>();
    foreach (IMyTerminalBlock T in tBlockList) {
        if (T is IMyAirVent) { GroupVents.Add((IMyAirVent)T); }
        if (T is IMyDoor) { GroupDoors.Add((IMyDoor)T); }
    }
    bool ConfigVentFound = false;
    foreach (IMyAirVent V in GroupVents) {
        if (System.Text.RegularExpressions.Regex.IsMatch((V as IMyTerminalBlock).CustomName,
            strAirventConfig,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
            ConfigVentFound = true;
        }
    }
    bool InDoorsFound = false;
    bool OutDoorsFound = false;
    foreach (IMyDoor D in GroupDoors) {
        if (System.Text.RegularExpressions.Regex.IsMatch((D as IMyTerminalBlock).CustomName,
            strInnerDoorTag,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
            InDoorsFound = true;
        }
        if (System.Text.RegularExpressions.Regex.IsMatch((D as IMyTerminalBlock).CustomName,
            strOutterDoorTag,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
            OutDoorsFound = true;
        }
    }
    if (!ConfigVentFound) { Echo($"Missing Vent with ACS-CV Tag in Group | {GroupName}"); this.Main("stop"); }
    if (!InDoorsFound) { Echo($"Missing Inner Door(s) with ACS-In Tag in Group | {GroupName}"); this.Main("stop"); }
    if (!OutDoorsFound) { Echo($"Missing Outter Door(s) with ACS-Out Tag in Group |  {GroupName}"); this.Main("stop"); }
    return MakeAirlockConfig(GroupVents);
}
void OpenDoors (List<IMyDoor> Doors) {
    foreach (IMyDoor D in Doors) { D.OpenDoor(); }
}
void CloseDoors (List<IMyDoor> Doors) {
    foreach (IMyDoor D in Doors) { D.CloseDoor(); }
}
void EnableDoors (List<IMyDoor> Doors, bool IsEnabled) {
    foreach (IMyDoor D in Doors) { D.Enabled = IsEnabled; }
}
void EnableDepressurization (List<IMyAirVent> Vents, bool IsDepressurized) {
    foreach (IMyAirVent V in Vents) { V.Depressurize = IsDepressurized; }
}
void CheckBlocks(string gName, List<IMyDoor> IN, List<IMyDoor> OUT, List<IMyAirVent> AIR) {
    // OUTBOUND - If Inner Door Partially Open
    if (GetDoorOpenRatio(IN) > 0 &&
        !GroupNameInternalDoorStatus[gName] &&
        !GroupNameExternalDoorStatus[gName] &&
        GroupNameOutBoundVentProc[gName] == 0 &&
        GetDoorClosedRatio(OUT) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}","O-B","|+|","[+]","|=|");
        EnableDoors(OUT, false);
        GroupNameInternalDoorStatus[gName] = true;
        GroupNameOutBoundVentProc[gName] = 1;
        if (GroupVentAlwaysDepressurize[gName]) {
            EnableDepressurization(AIR, true);
        } else {
            EnableDepressurization(AIR, false);
        } return;
    }
    // OUTBOUND - If Inner Door Fully Open and Outter Door Fully Closed
    if (GetDoorOpenRatio(IN) == 1 &&
        GroupNameInternalDoorStatus[gName] &&
        !GroupNameExternalDoorStatus[gName] &&
        GroupNameOutBoundVentProc[gName] == 1 &&
        GetDoorClosedRatio(OUT) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "O-B", "| |", "[+]", "|=|");
        GroupNameInternalDoorTicks[gName] += PlusTicks;
        if (GroupNameInternalDoorTicks[gName] >= GroupNameInternalDoorTimers[gName]) {
            GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "O-B", "|-|", "[+]", "|=|");
            CloseDoors(IN);
            GroupNameInternalDoorTicks[gName] = 0;
            GroupNameOutBoundVentProc[gName] = 2;
        } return;
    }
    // OUTBOUND - If both doors are closed
    if (GetDoorClosedRatio(IN) == 0 &&
        GroupNameInternalDoorStatus[gName] &&
        !GroupNameExternalDoorStatus[gName] &&
        GroupNameOutBoundVentProc[gName] == 2 &&
        GetDoorClosedRatio(OUT) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "O-B", "|=|", "[+]", "|=|");
        EnableDoors(IN, false);
        EnableDepressurization(AIR, true);
        GroupNameOutBoundVentProc[gName] = 3;
        return;
    }
    // OUTBOUND - Depressurize
    if (GetDoorClosedRatio(IN) == 0 &&
        GroupNameInternalDoorStatus[gName] &&
        !GroupNameExternalDoorStatus[gName] &&
        GroupNameOutBoundVentProc[gName] == 3 &&
        GetDoorClosedRatio(OUT) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "O-B", "|=|", "[-]", "|=|");
        EnableDepressurization(AIR, true);
        GroupNameVentTicks[gName] += PlusTicks;
        if (GroupNameVentTicks[gName] >= GroupNameVentTimers[gName]) {
            GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "O-B", "|=|", "[-]", "|+|");
            EnableDoors(OUT, true);
            OpenDoors(OUT);
            GroupNameExternalDoorStatus[gName] = true;
            GroupNameVentTicks[gName] = 0;
            GroupNameOutBoundVentProc[gName] = 4;
        } return;
    }
    // OUTBOUND - Close Outter Door
    if (GetDoorClosedRatio(IN) == 0 &&
        GroupNameInternalDoorStatus[gName] &&
        GroupNameExternalDoorStatus[gName] &&
        GroupNameOutBoundVentProc[gName] == 4 &&
        GetDoorOpenRatio(OUT) == 1) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "O-B", "|=|", "[-]", "| |");
        GroupNameExternalDoorTicks[gName] += PlusTicks;
        if (GroupNameExternalDoorTicks[gName] >= GroupNameExternalDoorTimers[gName]) {
            GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "O-B", "|=|", "[-]", "|-|");
            CloseDoors(OUT);
            GroupNameExternalDoorTicks[gName] = 0;
            GroupNameOutBoundVentProc[gName] = 5;
        } return;
    }
    // INBOUND - If Outter Door Partially Open
    if (GetDoorOpenRatio(OUT) > 0 &&
        !GroupNameInternalDoorStatus[gName] &&
        !GroupNameExternalDoorStatus[gName] &&
        GroupNameInBoundVentProc[gName] == 0 &&
        GetDoorClosedRatio(IN) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "|+|", "[-]", "|=|");
        EnableDoors(IN, false);
        GroupNameExternalDoorStatus[gName] = true;
        GroupNameInBoundVentProc[gName] = 1;
        EnableDepressurization(AIR, true);
        return;
    }
    // INBOUND - If Outter Door Fully Open and Inner Door Fully Closed
    if (GetDoorOpenRatio(OUT) == 1 &&
        !GroupNameInternalDoorStatus[gName] &&
        GroupNameExternalDoorStatus[gName] &&
        GroupNameInBoundVentProc[gName] == 1 &&
        GetDoorClosedRatio(IN) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "| |", "[-]", "|=|");
        GroupNameExternalDoorTicks[gName] += PlusTicks;
        if (GroupNameExternalDoorTicks[gName] >= GroupNameExternalDoorTimers[gName]) {
            GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "|-|", "[-]", "|=|");
            CloseDoors(OUT);
            GroupNameExternalDoorTicks[gName] = 0;
            GroupNameInBoundVentProc[gName] = 2;
        } return;
    }
    // INBOUND - If both doors are closed
    if (GetDoorClosedRatio(OUT) == 0 &&
        !GroupNameInternalDoorStatus[gName] &&
        GroupNameExternalDoorStatus[gName] &&
        GroupNameInBoundVentProc[gName] == 2 &&
        GetDoorClosedRatio(IN) < 1) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "|=|", "[-]", "|=|");
        EnableDoors(OUT, false);
        if (GroupVentAlwaysDepressurize[gName]) {
            EnableDepressurization(AIR, true);
        } else {
            EnableDepressurization(AIR, false);
        }
        GroupNameInBoundVentProc[gName] = 3;
        return;
    }
    // INBOUND - Pressurize
    if (GetDoorClosedRatio(OUT) == 0 &&
        !GroupNameInternalDoorStatus[gName] &&
        GroupNameExternalDoorStatus[gName] &&
        GroupNameInBoundVentProc[gName] == 3 &&
        GetDoorClosedRatio(IN) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "|=|", "[+]", "|=|");
        GroupNameVentTicks[gName] += PlusTicks;
        if (GroupNameVentTicks[gName] >= GroupNameVentTimers[gName]) {
            GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "|=|", "[+]", "|+|");
            EnableDoors(IN, true);
            OpenDoors(IN);
            GroupNameInternalDoorStatus[gName] = true;
            GroupNameVentTicks[gName] = 0;
            GroupNameInBoundVentProc[gName] = 4;
        } return;
    }
    // INBOUND - Close Inner Door
    if (GetDoorClosedRatio(OUT) == 0 &&
        GroupNameInternalDoorStatus[gName] &&
        GroupNameExternalDoorStatus[gName] &&
        GroupNameInBoundVentProc[gName] == 4 &&
        GetDoorOpenRatio(IN) == 1) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "|=|", "[+]", "| |");
        GroupNameInternalDoorTicks[gName] += PlusTicks;
        if (GroupNameInternalDoorTicks[gName] >= GroupNameInternalDoorTimers[gName]) {
            GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "I-B", "|=|", "[+]", "|-|");
            CloseDoors(IN);
            GroupNameInternalDoorTicks[gName] = 0;
            GroupNameInBoundVentProc[gName] = 5;
        } return;
    }
    // RESET DOORS FOR PROC
    if (GetDoorClosedRatio(IN) == 0 &&
        GroupNameInternalDoorStatus[gName] &&
        GroupNameExternalDoorStatus[gName] &&
        (GroupNameInBoundVentProc[gName] == 5 || GroupNameOutBoundVentProc[gName] == 5) &&
        GetDoorClosedRatio(OUT) == 0) {
        GroupNameWithStatus[gName] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "Rdy", "|=|", "[-]", "|=|");
        GroupNameInternalDoorStatus[gName] = false;
        GroupNameExternalDoorStatus[gName] = false;
        GroupNameOutBoundVentProc[gName] = 0;
        EnableDoors(IN, true);
        EnableDoors(OUT, true);
        GroupNameInBoundVentProc[gName] = 0;
        GroupNameOutBoundVentProc[gName] = 0;
        EnableDepressurization(AIR, true);
        return;
    }
}
void ResetConfigs() {
    foreach (KeyValuePair<string, List<IMyTerminalBlock>> AirlockGroup in GroupNamesWithBlockList) {
        IMyAirVent mainVent = GetMembers(AirlockGroup.Key, AirlockGroup.Value);
        mainVent.CustomData = string.Empty;
    }
}
void ResetAirlocks() {
    foreach (KeyValuePair<string, List<IMyTerminalBlock>> AirlockGroup in GroupNamesWithBlockList) {
        foreach(IMyTerminalBlock T in AirlockGroup.Value) {
            if (T is IMyDoor) { (T as IMyDoor).Enabled = true; (T as IMyDoor).CloseDoor(); }
            if (T is IMyAirVent) { (T as IMyAirVent).Depressurize = true; }
        }
    }
}
void CheckAirLocks() {
    foreach (KeyValuePair<string, List<IMyTerminalBlock>> AirlockGroup in GroupNamesWithBlockList) {
        foreach (IMyTerminalBlock T in GroupNamesWithBlockList[AirlockGroup.Key]) {
            if (T is IMyDoor) {
                if ((T as IMyDoor).OpenRatio > 0) {
                    mainSB.AppendLine("Resetting Airlocks");
                    diagSB.AppendLine("Resetting Airlocks");
                    DisplayDiagnosticLCD(diagSB.ToString(), strLCD_Diagnostics);
                    Echo(mainSB.ToString());
                    return;
                }
            } else {
                continue;
            }
        }
    }
    Echo("Clearing Status");
    foreach(string s in GroupNamesWithBlockList.Keys) {
        GroupNameWithStatus[s] = string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "Rdy", "|=|", "[-]", "|=|");
        GroupNameInternalDoorStatus[s] = false;
        GroupNameExternalDoorStatus[s] = false;
        GroupNameInBoundVentProc[s] = 0;
        GroupNameOutBoundVentProc[s] = 0;
    }
    IsResettingAirlocks = false;
}
void EnumForOrphanItems_String_Int(Dictionary<string, int> items) {
    List<string> remove = new List<string>();
    foreach (String s in items.Keys) {
        if (!GroupNamesWithBlockList.ContainsKey(s)) { remove.Add(s); }
    }
    foreach (string s in remove) { items.Remove(s); }
}
void EnumForOrphanItems_String_Bool(Dictionary<string, bool> items) {
    List<string> remove = new List<string>();
    foreach (String s in items.Keys) {
        if (!GroupNamesWithBlockList.ContainsKey(s)) { remove.Add(s); }
    }
    foreach (string s in remove) { items.Remove(s); }
}
void EnumForOrphanItems_String_String(Dictionary<string, string> items) {
    List<string> remove = new List<string>();
    foreach (String s in items.Keys) {
        if (!GroupNamesWithBlockList.ContainsKey(s)) { remove.Add(s); }
    }
    foreach (string s in remove) { items.Remove(s); }
}
void PurgeOrphanGroups() {
    EnumForOrphanItems_String_Int(GroupNameInternalDoorTimers);
    EnumForOrphanItems_String_Int(GroupNameExternalDoorTimers);
    EnumForOrphanItems_String_Int(GroupNameInternalDoorTicks);
    EnumForOrphanItems_String_Int(GroupNameExternalDoorTicks);
    EnumForOrphanItems_String_Int(GroupNameOutBoundVentProc);
    EnumForOrphanItems_String_Int(GroupNameInBoundVentProc);
    EnumForOrphanItems_String_Int(GroupNameVentTimers);
    EnumForOrphanItems_String_Int(GroupNameVentTicks);
    EnumForOrphanItems_String_Bool(GroupVentAlwaysDepressurize);
    EnumForOrphanItems_String_Bool(GroupNameInternalDoorStatus);
    EnumForOrphanItems_String_Bool(GroupNameExternalDoorStatus);
    EnumForOrphanItems_String_String(GroupNameWithStatus);
}
public void Main(string arg) {
    GroupNamesWithBlockList.Clear(); BlockGroups.Clear(); mainSB.Clear(); diagSB.Clear();
    GridTerminalSystem.GetBlockGroups(BlockGroups);
    mainSB.AppendLine("Lunixia's Airlock Control Script");
    diagSB.AppendLine("Lunixia's Airlock Control Script");
    if (Runtime.UpdateFrequency == UpdateFrequency.Update1) { PlusTicks = 1; }
    if (Runtime.UpdateFrequency == UpdateFrequency.Update10) { PlusTicks = 10; }
    if (System.Text.RegularExpressions.Regex.IsMatch(arg,
        "stop",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
        Runtime.UpdateFrequency = UpdateFrequency.None; return;
    }
    if (System.Text.RegularExpressions.Regex.IsMatch(arg,
        "resetlocks",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
        IsResettingAirlocks = true; ResetAirlocks();
    }
    if (System.Text.RegularExpressions.Regex.IsMatch(arg,
        "resetconfigs",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
        ResetConfigs();
    }
    if (IsResettingAirlocks) { CheckAirLocks(); return; }
    foreach (IMyBlockGroup bg in BlockGroups) {
        if (System.Text.RegularExpressions.Regex.IsMatch(bg.Name,
            strGroupTag,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
            List<IMyTerminalBlock> g = new List<IMyTerminalBlock>(); bg.GetBlocks(g);
            if (!GroupNamesWithBlockList.ContainsKey(bg.Name)) {
                GroupNamesWithBlockList.Add(bg.Name, g);
            }
            if (!GroupNameInternalDoorStatus.ContainsKey(bg.Name)) {
                GroupNameInternalDoorStatus.Add(bg.Name, false);
            }
            if (!GroupNameExternalDoorStatus.ContainsKey(bg.Name)) {
                GroupNameExternalDoorStatus.Add(bg.Name, false);
            }
            if (!GroupNameInternalDoorTicks.ContainsKey(bg.Name)) {
                GroupNameInternalDoorTicks.Add(bg.Name, 0);
            }
            if (!GroupNameExternalDoorTicks.ContainsKey(bg.Name)) {
                GroupNameExternalDoorTicks.Add(bg.Name, 0);
            }
            if (!GroupNameVentTicks.ContainsKey(bg.Name)) {
                GroupNameVentTicks.Add(bg.Name, 0);
            }
            if (!GroupNameInBoundVentProc.ContainsKey(bg.Name)) {
                GroupNameInBoundVentProc.Add(bg.Name, 0);
            }
            if (!GroupNameOutBoundVentProc.ContainsKey(bg.Name)) {
                GroupNameOutBoundVentProc.Add(bg.Name, 0);
            }
            if (!GroupNameWithStatus.ContainsKey(bg.Name)) {
                GroupNameWithStatus.Add(bg.Name, string.Format("{0,4}\t{1,4}\t{2,4}\t{3,4}", "Rdy", "|=|", "[-]", "|=|"));
            }
            if (!GroupVentAlwaysDepressurize.ContainsKey(bg.Name)) {
                GroupVentAlwaysDepressurize.Add(bg.Name, false);
            }
        }
    }
    foreach(KeyValuePair<string, List<IMyTerminalBlock>> AirlockGroup in GroupNamesWithBlockList) {
        IMyAirVent mainVent = GetMembers(AirlockGroup.Key, AirlockGroup.Value);
        foreach (string line in mainVent.CustomData.Split('\n')) {
            if (line.Contains(idd)) {
                if (!GroupNameInternalDoorTimers.ContainsKey(AirlockGroup.Key)) {
                    GroupNameInternalDoorTimers.Add(AirlockGroup.Key, Convert.ToInt32(line.Replace(idd, "")));
                } else {
                    GroupNameInternalDoorTimers[AirlockGroup.Key] = Convert.ToInt32(line.Replace(idd, ""));
                }
            }
            if (line.Contains(odd)) {
                if (!GroupNameExternalDoorTimers.ContainsKey(AirlockGroup.Key)) {
                    GroupNameExternalDoorTimers.Add(AirlockGroup.Key, Convert.ToInt32(line.Replace(odd, "")));
                } else {
                    GroupNameExternalDoorTimers[AirlockGroup.Key] = Convert.ToInt32(line.Replace(odd, ""));
                }
            }
            if (line.Contains(vdd)) {
                if (!GroupNameVentTimers.ContainsKey(AirlockGroup.Key)) {
                    GroupNameVentTimers.Add(AirlockGroup.Key, Convert.ToInt32(line.Replace(vdd, "")));
                } else {
                    GroupNameVentTimers[AirlockGroup.Key] = Convert.ToInt32(line.Replace(vdd, ""));
                }
            }
            if (line.Contains(ad)) {
                if (!GroupVentAlwaysDepressurize.ContainsKey(AirlockGroup.Key)) {
                    GroupVentAlwaysDepressurize.Add(AirlockGroup.Key, Convert.ToBoolean(line.Replace(ad, "")));
                } else {
                    GroupVentAlwaysDepressurize[AirlockGroup.Key] = Convert.ToBoolean(line.Replace(ad, ""));
                }
            }
        }
    }
    foreach (KeyValuePair<string, List<IMyTerminalBlock>> AirlockGroup in GroupNamesWithBlockList) {
        List<IMyDoor> IN = new List<IMyDoor>();
        List<IMyDoor> OUT = new List<IMyDoor>();
        List<IMyAirVent> AIR = new List<IMyAirVent>();
        foreach (IMyTerminalBlock T in AirlockGroup.Value) {
            if (System.Text.RegularExpressions.Regex.IsMatch(T.CustomName,
                strInnerDoorTag,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
                IN.Add(T as IMyDoor);
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(T.CustomName,
                strOutterDoorTag,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
                OUT.Add(T as IMyDoor);
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(T.CustomName,
                strAirventTag,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                System.Text.RegularExpressions.Regex.IsMatch(T.CustomName,
                strAirventConfig,
                System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
                AIR.Add(T as IMyAirVent);
            }
        }
        CheckBlocks(AirlockGroup.Key, IN, OUT, AIR);
    }
    mainSB.AppendLine($"Managing {GroupNamesWithBlockList.Count} Airlock Groups : {prog[c]}");
    diagSB.AppendLine($"Managing {GroupNamesWithBlockList.Count} Airlock Groups : {prog[c]}");
    string[] Headers = { "DIR", "IDs", "V", "ODs" };
    string Header = string.Format("{0,4}\t{1,4}\t{2,3}\t{3,5}", Headers[0], Headers[1], Headers[2], Headers[3]);
    diagSB.AppendLine();
    diagSB.AppendLine("AirLock Group Status");
    diagSB.AppendLine(Header);
    foreach (string s in GroupNamesWithBlockList.Keys) {
        string g = string.Empty;
        if (hideGroupTagOnLCD){ g = s.Replace(strGroupTag, string.Empty); } else { g = s; }
        mainSB.AppendLine(g);
        diagSB.AppendLine($"{GroupNameWithStatus[s]} {g}");
    }
    DisplayDiagnosticLCD(diagSB.ToString(), strLCD_Diagnostics);
    Echo(mainSB.ToString());
    c = c >= 3 ? 0 : c + 1;
    PurgeOrphanGroups();
}