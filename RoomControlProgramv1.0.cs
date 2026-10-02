/*
 *        Room Control Program Script
 *        version: 1.05 - updated 20. Jan 2021
 *        by AlfRomeo80
 * 
 *        deployed with MDK-SE - Malware's Development Kit for Space Engineers
 *        https://github.com/malware-dev/MDK-SE
 */

/* ---------------- LIST OF ALL COMMANDS ----------------
         *
         * !!! add " ~ BlocksCustomName" after the command !!!
         * to select the screen you want to interact with
         *
         * --- commands to use the control screen ---
         * menu- => show previous control screen
         * menu+ => show next control screen
         * menu=# => show control screen with number # (0 => main control, 1 => room control)
         * room- => show previous room
         * room+ => show next room
         * room=# => show room with number #
         * select- => move cursor to the previous item
         * select+ => move cursor to the next item
         * select=1 => select or deselect the item at current cursor position
         * switch=0 => activate or deactivate current item or all selected items
         * switch=1 => change Primary value of current item or all selected items
         * switch=2 => change Secondary value of current item or all selected items
         * switch=-1 => switch automatic room control for current shown room
         * cs- => decrease conveyor system number
         * cs+ => increase conveyor system number
         *
         * --- commands to use the instructions screen ---
         * tab=# => show instructions for button bar number #
         *
         * --- general commands --- no link to a screen needed ---
         * resetErrors => deletes all error messages
         * resetBlocks => set all Custom Data Entries to their default values
         * uninstall => deletes all entries at the name and the custom data field of all blocks the script made
         * >>> ONLY USE UNINSTALL IF YOU NEVER WANT TO USE THE SCRIPT AGAIN IN THIS WORLD <<<
         *
         * --- commands to use to change room settings --- no link to a screen needed ---
         * useAirlock=#1 > #2 => use airlock #1 to enter from room #2
         * useAirlock=#2 < #2 => use airlock #1 to leave to room #2
         * useHangar=# > => close hangar with room number # and pressurize
         * useHanger=# < => reduce atmosphere in hangar # and open gates
         * changeRoomO2=# => change pressurization of room #
         *
         * ------------------- SET PARAMETERS -------------------
         *
         * You can decide whether you want to use a name token to recognize blocks of a grid, or whether to let the script analyze the grid.
         * The script detection doesn't use blocks connected with connectors, but with merge blocks, rotors and pistons.
         * If you have other grid connected with merge blocks and you don't want these blocks to be used by the script,
         * you have to use the part of its name to seperate them.
         * scanConstruct = true => the script is analyzing the grid, or if "false" the names get analysed.
         * If scanConstruct = false => Enter a specific part of all blocks name, only blocks with this keyword in there names are controlled by the script.
         * e.g.:
         * static readonly bool scanConstruct = false;
         * const string gridName = "Base One";
         * or:
         * static readonly bool scanConstruct = true;
         * const string gridName = "";
         */
static readonly bool scanConstruct = true;
const string gridName = "";
/*
         * If not all your blocks are connected with one single conveyor system set
         * maxCS to the number of systems you are going to use
         * e.g.: static int maxCS = 3;
         */
const int maxCS = 2;
/*
         * Here you name all your rooms and mathc them with a room number used at the "room="-tag
         * in the custom data fields of the blocks.
         * If you build an airlock this room has to have "Airlock" in its name.
         * If you have build a hangar this room has to have "Hanger" in its name.
         * The first room has to be { 0, "outside" } allways to represent the atmosphere outside the ship.
         * e.g.:
         * static readonly Dictionary<int, string> roomNames = new Dictionary<int, string>()
         * {
         *     { 0, "outside" }, { 1, "Main Room" }, { 2, "Entrance Airlock" }, { 3, "Ship Hangar" }
         * };
         */
static readonly Dictionary<int, string> roomNames = new Dictionary<int, string>()
{
    { 0, "outside" }, { 1, "First Room" }
};
/*
         * -------------- MINOR IMPORTANT SETTINGS --------------
         */
// Language: english: en = true, translation: en = false
static readonly bool en = true;
// enable or disable fastBoot
static readonly bool fastBoot = true;
// airlock vents conveyor system oxygen level values
static bool leveling = false;
static float minVentCS = 0.3f;
static float maxVentCS = 0.7f;
// airlock timing values
const int waitTimer = 8;
// screens cycle speed - switch screens every # runs
const int cycleSpeed = 2;
// colors used in the script
static readonly Color
    mainColor = new Color(130, 130, 130), // main text and draw color
    warningColor = new Color(200, 200, 0), // warning outputs an programScreen
    colorOff = new Color(65, 0, 0), // block is offline
    colorOn = new Color(0, 65, 0), // block is online
    h2Color = new Color(90, 25, 25), // hydrogen amounts
    o2Color = new Color(25, 25, 90), // oxygen amounts
    leakColor = new Color(130, 65, 0), // unpressurizeable rooms
    depressColor = new Color(0, 100, 130), // depressurize a room
    backgroundColor = new Color(0, 0, 0), // screen background
    selectionColor = new Color(130, 130, 130), // selection square
    selectedColor = new Color(65, 65, 65); // selected background
// output of the instruction screen - nine lists for each tab with one headline and eight instructions for each button
// The following list is my suggestion for setting up the G menu. I strongly recommend using button 1 of the G menu to change the instruction screen using the command "tab=#".
static readonly string[,] instructions = new string[9, 9]
{
    { "Room Program Settings", "show main control", "", "move cursor backwards", "move cursor forwards", "", "activate/deactivate", "change Primary", "change Secondary" },
    { "Room Program Control", "show previous room", "show next room", "move cursor backwards", "move cursor forwards", "switch auto. room control", "activate/deactivate block", "change Primary", "change Secondary" },
    { "Conveyor System Vent Settings", "show previous room", "show next room", "move cursor backwards", "move cursor forwards", "", "", "decrease CS", "increase CS" },
    { "not set up yet", "", "", "", "", "", "", "", "" },
    { "not set up yet", "", "", "", "", "", "", "", "" },
    { "not set up yet", "", "", "", "", "", "", "", "" },
    { "not set up yet", "", "", "", "", "", "", "", "" },
    { "not set up yet", "", "", "", "", "", "", "", "" },
    { "Room Program System Settings", "", "", "", "", "", "", "", "delete all error codes" },
};
// List of all german translations. You can edit them, if you want to use your own translations - { "script output", "translated term" }
static readonly Dictionary<string, string> translations = new Dictionary<string, string>
{
    {" y", " J"}, {" w", " W"}, {" d", " T"},
    {"deactivate", "ausschalten"}, {"activate", "einschalten"}, {"Primary", "Primär"}, {"Secondary", "Sekundär"}, {"decrease limit by ", "verringere Limit um "}, {"increase limit by ", "erhöhe Limit um "},
    {"ready to use", "einsatzbereit"}, {"leveling tanks", "gleiche Tanks aus"}, {"entrance requested", "Zugang erfragt"}, {"match pressure", "Druckausgleich"}, {"checking rooms", "prüfe Räume"},
    {"exit requested", "Ausgang erfragt"}, {"ready to leave", "Ausgang sicher"}, {"manual room control", "manuelle Raumsteuerung"}, {"automatic control", "automatische Steuerung"}, {"lethal", "tödlich"}, {"healthy", "atembar"},
    {"O2 Level", "O2 Level"}, {"Main", "Main"}, {"Backup", "Backup"}, {"Door ", "Tür "}, {"Gate ", "Tor "}, {"damaged", "defekt"}, {"Opening", "öffnet"}, {"Open", "offen"}, {"Closing", "schließt"}, {"Close", "geshlossen"},
    {"sealed", "versiegelt"}, {"unsealed", "frei"},  {"set to open", "öffnen"}, {"set to close", "schließen"}, {"offline", "offline"}, {"Tanks O2 level: ", "O2-Tankfüllungen: "}, {"depressurize", "entlüften"},
    {"depressurize room", "Raum entlüften"}, {"pressurize", "belüften"}, {"pressurize room", "Raum belüften"}, {"level oxygen tanks", "Tanks ausgleichen"}, {" Status", "-Status"}, {" Control", "-Steuerung"},
    {"symbol area calculation failed", "Berechnung Symbolbereich fehlgeschlagen"}, {"no blocks\navailable", "keine Blöcke\nverfügbar"}, {"Rooms", "Raum"}, {"Airlock", "Luftschleuse" }, {"manual block control", "manuelle Blocksteuerung"},
    {"specify the room to show\nadd \", #\" to RoomScreen\n\ni.e.: ScreenArea=RoomScreen, 3\nshowing room number 3", "lege den anzuzeigenden Raum fest\nfüge \", #\" zu RoomScreen hinzu\nz.B.: ScreenArea=RoomScreen, 3\nzeigt Raum Nummer 3"},
    {"Please enter room number of the\nroom this display is located and\nthe airlock you want to monitor\nto the custom data field\n\ni.e.: screen=RoomScreen, 1 > 2\nplaced room: 1, shown room: 2",
    "Bitte die Nummern der Räume\nin dem die Anzeige montiert ist und\nder Luftschleuse die angezeigt werden soll\nim Custom Data Feld angeben\n\nz.B.: screen=RoomScreen, 1 > 2\nmontiert in Raum: 1, angezeigt wird Raum: 2"},
    {"Room Control", "Raum-Steuerung"}, {"switch auto. room pressure and door control", "schalte auto. Raumdruck- und Türsteuerung um"}, {"switch automatic airlock oxygen leveling", "schalte auto. Tankausgleich um"},
    {"change upper oxygen limit for leveling", "ändere oberes Limit fur Tankausgleich"}, {"change lower oxygen limit for leveling", "ändere unteres Limit fur Tankausgleich"}, {"Room Program", "Raum Programm"},
    {"Room Control Program", "Raumsteuerungsprogramm"}, {"Room Program Manual", "Raum Programm Handbuch"}
};
// ------------ DON'T CHANGE ANYTHING BELOW THIS LINE ------------------------- DON'T CHANGE ANYTHING BELOW THIS LINE ------------- //
public enum LayoutTypes { RoomControlScreen, RoomScreen, AirlockScreen, RoomInstructionsScreen, RoomProgramScreen, NONE }
public enum BlockSetupTags { Name, Conveyor, Room, RoomInside, RoomOutside, DoorGroup };
public enum ScreenSetupTags { Sensor, Cockpit };
static List<u>ɀ=new List<u>();static List<Ń>Ɂ=new List<Ń>();static List<Ě>ş=new List<Ě>();static List<IMyBlockGroup>ɂ=
new List<IMyBlockGroup>();static List<Ė>Ƀ=new List<Ė>();static List<đ>Ʉ=new List<đ>();static List<IMyCockpit>Ʌ=new List<
IMyCockpit>();static Dictionary<int,ȡ>Ɇ=new Dictionary<int,ȡ>();static List<Ó>ɇ=new List<Ó>();static Ȼ ɖ=new Ȼ();static bool Ɉ=
false;static bool ɉ=false;const string Ɋ="@RoomControl";const string ɋ="[SETUP NEEDED]";const string Ɍ="[screens]";const
string ɍ="[/screens]";const string Ɏ="[setup]";const string ɏ="[/setup]";const string ɐ="[readmeControlProgram]";const string
ɑ="[/readmeControlProgram]";static bool ɒ=true;static System.Globalization.CultureInfo ɓ=System.Globalization.CultureInfo
.GetCultureInfo("en-US");static int ɔ;static int ɕ;static bool Æ;static int ɗ=int.MaxValue;static int ɘ=int.MaxValue;
static bool ɟ=false;static int ɠ=0;static int ɡ=1;static bool ɢ=true;static string ɣ="";static Func<float,string>ɤ=ə=>{if(Math
.Abs(ə)<1000)return ə.ToString("#,0.",ɓ)+" l";else return(ə/1000).ToString("#,0.",ɓ)+" m³";};static void ɥ(float[]ɦ)=>
Array.Clear(ɦ,0,maxCS);static void ɥ(bool[]ɦ)=>Array.Clear(ɦ,0,maxCS);bool ɧ<D>(D Y)where D:IMyTerminalBlock{if(scanConstruct
)return Y.IsSameConstructAs(Me);else return Y.CustomName.Contains(gridName);}static ȡ ɨ(int ȉ){ȡ Đ;if(!Ɇ.TryGetValue(ȉ,
out Đ)){Đ=new ʴ(ȉ);Ɇ.Add(ȉ,Đ);}return Đ;}static bool ɯ(bool ę)=>ɖ.ȸ=ɉ=ę;static bool ɮ(bool ę){ę&=ɉ;if(!ę)foreach(var ơ in Ɇ
.Values)ơ.ʾ();return leveling=ę;}Program(){if(maxCS>1)ɒ=false;if(!en)ɓ=System.Globalization.CultureInfo.GetCultureInfo(
"de-DE");Æ=true;ɔ=0;ɕ=10;Ɇ.Add(-1,new ʴ(-1,"no room set"));foreach(var ɭ in roomNames)Ɇ.Add(ɭ.Key,new ʴ(ɭ.Key,ɭ.Value));Runtime
.UpdateFrequency=UpdateFrequency.Update100;}void Save(){if(ɢ)Storage=ɬ(true);}string ɬ(bool ɢ=false){if(ɢ){ɣ=ɉ+"\n"+
leveling+"\n"+minVentCS+"\n"+maxVentCS+"\n"+ʴ.ʏ(Ɇ.Values.ToArray());}else{string[]ɫ=Storage.Split('\n');if(ɫ.Length>=5){try{ɉ=
bool.Parse(ɫ[0]);leveling=bool.Parse(ɫ[1]);minVentCS=float.Parse(ɫ[2]);maxVentCS=float.Parse(ɫ[3]);ʴ.ʂ(ɫ[4],Ɇ.Values.ToArray
());}catch{ɖ.ȗ("Loading failed\ndefault values used");return"";}}}return ɣ;}void Main(string ɪ,UpdateType ɩ){ɖ.Ã();if(ɡ>
cycleSpeed)ɡ=1;if(ɪ.Any())ȁ(ɪ);if(ɕ<10){ȩ();Echo(ɖ.Ȣ());ɇ.ForEach(ż=>ż.Ç(Æ));if(ɕ==9){ɇ.ForEach(ż=>ż.É(false));ɇ.Clear();Me.
Enabled=false;}return;}if(ɔ>=ɘ){u.Ã();ɝ(ɀ);ɝ(Ɂ);ɝ(ş);ɝ(Ʉ);ɝ(Ƀ);foreach(var Đ in Ɇ.Values)Đ.ˍ(Ɂ,ş,Ʉ,Ƀ);if(Ɉ)Ƙ.Ƣ();Ɉ=false;}if(ɔ
<=ɘ)ɞ();if(ɔ>ɗ){ɛ();Ǿ();ɇ.ForEach(ż=>ż.Ç(Æ));}Echo(ɖ.Ȣ());if(ɡ>=cycleSpeed)ɠ++;ɡ++;}void ɞ(){switch(ɔ){case 0:ɖ.Ȓ();ɖ.Ȗ(
"booting...\n");ɖ.Ȗ("initialization...done\n");ɖ.ȓ("loading all sensors/cockpits");break;case 1:Ʉ.Clear();List<IMySensorBlock>Ȱ=new
List<IMySensorBlock>();GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(Ȱ,ß=>ɧ(ß)&&ß.CustomData.Contains(Ɋ));Ȱ.ForEach(ß=>
Ʉ.Add(new đ(ß)));Ʉ=Ʉ.OrderBy(ß=>ß.ʴ.ɰ).ToList();Ʌ.Clear();GridTerminalSystem.GetBlocksOfType<IMyCockpit>(Ʌ,á=>ɧ(á));ɖ.ȕ()
;ɖ.ȓ("loading all screens");break;case 2:ɛ();if(fastBoot)Runtime.UpdateFrequency=UpdateFrequency.Update10;ɇ.ForEach(ż=>ż.
Ç(Æ));ɗ=ɔ;ɖ.ȕ();ɖ.ȓ("loading all oxygen tanks");break;case 3:ɀ.Clear();List<IMyGasTank>Ȫ=new List<IMyGasTank>();
GridTerminalSystem.GetBlocksOfType<IMyGasTank>(Ȫ,ȫ=>ɧ(ȫ)&&!ȫ.BlockDefinition.SubtypeId.Contains("Hydro"));Ȫ.ForEach(ȫ=>ɀ.Add(new u(ȫ)));ɀ=
ɀ.OrderBy(ȫ=>ȫ.ʑ).ToList();ɖ.ȕ();ɖ.ȓ("loading all air vents");break;case 4:Ɂ.Clear();List<IMyAirVent>Ȭ=new List<
IMyAirVent>();GridTerminalSystem.GetBlocksOfType<IMyAirVent>(Ȭ,ə=>ɧ(ə));Ȭ.ForEach(ə=>Ɂ.Add(new Ń(ə)));Ɂ=Ɂ.OrderBy(ə=>ə.ʑ).ToList()
;ɖ.ȕ();ɖ.ȓ("looking for grouped doors");break;case 5:ɂ.Clear();GridTerminalSystem.GetBlockGroups(ɂ,ɚ=>{List<
IMyTerminalBlock>Ŵ=new List<IMyTerminalBlock>();ɚ.GetBlocksOfType<IMyDoor>(Ŵ,Ŕ=>ɧ(Ŕ));return Ŵ.Count()>0;});ɖ.ȕ();ɖ.ȓ(
"loading all doors and gates");break;case 6:ş.Clear();List<IMyDoor>ȭ=new List<IMyDoor>();GridTerminalSystem.GetBlocksOfType<IMyDoor>(ȭ,Ŕ=>ɧ(Ŕ));ȭ.
ForEach(Ŕ=>ş.Add(new Ě(Ŕ)));ş=ş.OrderBy(Ŕ=>Ŕ.ʑ).ToList();Ɔ.ƅ=true;ɖ.ȕ();ɖ.ȓ("loading all warning lights");break;case 7:Ƀ.Clear(
);List<IMyReflectorLight>Ȯ=new List<IMyReflectorLight>();GridTerminalSystem.GetBlocksOfType<IMyReflectorLight>(Ȯ,Ə=>ɧ(Ə)
&&Ə.CustomData.Contains(Ɋ));Ȯ.ForEach(Ə=>Ƀ.Add(new Ė(Ə)));Ƀ=Ƀ.OrderBy(Ə=>Ə.ʴ.ɰ).ToList();ɖ.ȕ();ɖ.ȓ("load saved data");
break;case 8:ɬ();ɖ.ȕ();break;case 9:ɖ.Ȗ("booting...done");Runtime.UpdateFrequency=UpdateFrequency.Update100;Æ=false;ɘ=ɔ;Ƙ.ƅ=ƍ
.ƅ=Ɩ.ƅ=true;ɖ.Ȓ();ɟ=false;break;}ɔ++;}void ɝ<D>(List<D>Ż)where D:ʤ{List<D>ɜ=new List<D>();Ż.ForEach(Y=>{if(
GridTerminalSystem.GetBlockWithId(Y.Ñ)!=null)try{Y.ʦ();}catch{ɖ.ȗ("Conveyor system number error\nCS: "+(Y.P+1)+" outside the limits.\n"+Y.
ʑ+"\nnot used by the script.");ɜ.Add(Y);}else ɜ.Add(Y);});foreach(var Y in ɜ)Ż.Remove(Y);}void ɛ(){List<IMyTerminalBlock>
ȯ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(ȯ,ť=>ɧ(ť)&&ť.CustomData.
Contains(Ɋ));List<int>Ȑ=new List<int>();for(int ā=0;ā<ɇ.Count;ā++)if(!ȯ.Any(ǻ=>ɇ[ā].Ñ==ǻ.EntityId))Ȑ.Add(ā);Ȑ.ForEach(ā=>{try{ɇ[
ā].É(true);}catch{ɖ.ȗ("missing a screen");}ɇ.RemoveAt(ā);});List<IMyTerminalBlock>Ǽ=new List<IMyTerminalBlock>();ȯ.
ForEach(ǻ=>{if(!ɇ.Any(ż=>ż.Ñ==ǻ.EntityId))Ǽ.Add(ǻ);});Ǽ.ForEach(ǽ=>ɇ.Add(new U(ǽ)));}void Ǿ(){List<n>ǿ;foreach(var ż in ɇ){ǿ=ż.
Ê();ǿ.ForEach(Ȁ=>{if(Ȁ!=null)switch((int)Ȁ.A){case 0:Ɉ|=true;break;case 1:(Ȁ as Ɔ).C();break;case 2:(Ȁ as ŭ).C();break;}}
);}}void ȁ(string Ȃ){string ǵ=null;string[]ȃ=Ȃ.Split('=');if(ȃ[0].Equals("resetBlocks",StringComparison.OrdinalIgnoreCase
)){ɟ=true;return;}else if(ȃ[0].Equals("resetErrors",StringComparison.OrdinalIgnoreCase)){ɖ.ȑ();return;}else if(ȃ[0].
Equals("uninstall",StringComparison.OrdinalIgnoreCase)){ɕ=0;return;}else if(ȃ[0].StartsWith("useAirlock",StringComparison.
OrdinalIgnoreCase)){bool Ȅ=false,ȏ=false;int ȍ=-1;int Ȍ=-1;int ȋ=-1;int ą;if(ȃ[1].Contains('<')){ą=ȃ[1].IndexOf('<');Ȅ=ą>0;}else{ą=ȃ[1].
IndexOf('>');ȏ=ą>0;}try{ȍ=int.Parse(ȃ[1].Substring(0,ą-1));if(Ȅ)Ȍ=int.Parse(ȃ[1].Substring(ą+1));if(ȏ)ȋ=int.Parse(ȃ[1].
Substring(ą+1));}catch{ɖ.ȗ("unknown airlock command");}if(Ȅ&&ȍ>0&&Ȍ>=0)Ɇ[ȍ].ˁ(Ɇ[Ȍ]);if(ȏ&&ȍ>0&&ȋ>=0)Ɇ[ȍ].ˆ(Ɇ[ȋ]);return;}else if(
ȃ[0].StartsWith("useHangar",StringComparison.OrdinalIgnoreCase)){try{int Ȋ=int.Parse(ȃ[1].TrimEnd('>','<').Trim());if(ȃ[1
].EndsWith(">"))Ɇ[Ȋ].ˎ();if(ȃ[1].EndsWith("<"))Ɇ[Ȋ].ˏ();}catch{ɖ.ȗ("unknown hangar command");}return;}else if(ȃ[0].
StartsWith("changeRoomO2",StringComparison.OrdinalIgnoreCase)){int ȉ;try{ȉ=int.Parse(ȃ[1].Trim());}catch{ɖ.ȗ(
"unknown changeRoom command");ȉ=-1;}if(ȉ>=0&&ȉ<=ʴ.Γ)Ɇ[ȉ].Ĺ=!Ɇ[ȉ].Ĺ;else ɖ.ȗ("unknown room to change");return;}try{string[]Ȏ;Ȏ=Ȃ.Split('~');ǵ=Ȏ[1].
Trim();ȃ=Ȏ[0].Trim().Split('=');}catch{ɖ.ȗ("command ignored\nuse \"... ~ block name\" to\nselect the control screen");return
;}if(ȃ[0].StartsWith("tab",StringComparison.OrdinalIgnoreCase)){try{ǹ(int.Parse(ȃ[1]),ǵ);}catch{ɖ.ȗ("unknown tab command"
);}}else if(ȃ[0].StartsWith("menu",StringComparison.OrdinalIgnoreCase)){try{if(ȃ[0].EndsWith("+"))ȇ(1,-1,ǵ);else if(ȃ[0].
EndsWith("-"))ȇ(-1,-1,ǵ);else if(ȃ[1].Length>0)ȇ(0,int.Parse(ȃ[1]),ǵ);}catch{ɖ.ȗ("unknown menu command");ȇ(0,0,ǵ);}}else if(ȃ[0]
.StartsWith("select",StringComparison.OrdinalIgnoreCase)){try{if(ȃ[0].EndsWith("+"))ȅ(1,ǵ);else if(ȃ[0].EndsWith("-"))ȅ(-
1,ǵ);}catch{ɖ.ȗ("unknown select command");ȅ(0,ǵ);}}else if(ȃ[0].StartsWith("switch",StringComparison.OrdinalIgnoreCase)){
int Ȉ;try{Ȉ=int.Parse(ȃ[1].Trim());}catch{ɖ.ȗ("unknown switch command");Ȉ=-10;}if(Ȉ==0)L(ǵ);else if(Ȉ==1)M(ǵ);else if(Ȉ==2)
N(ǵ);else if(Ȉ==-1)K(ǵ);}else if(ȃ[0].StartsWith("cs",StringComparison.OrdinalIgnoreCase)){if(ȃ[0].EndsWith("+"))Ǹ(1,ǵ);
else if(ȃ[0].EndsWith("-"))Ǹ(-1,ǵ);else ɖ.ȗ("unknown cs command");}else if(ȃ[0].StartsWith("room",StringComparison.
OrdinalIgnoreCase)){try{if(ȃ[0].EndsWith("+"))Ƕ(1,-1,ǵ);else if(ȃ[0].EndsWith("-"))Ƕ(-1,-1,ǵ);else if(ȃ[1].Length>0)Ƕ(0,int.Parse(ȃ[1]),ǵ
);}catch{ɖ.ȗ("unknown room command");Ƕ(0,0,ǵ);}}}void ȇ(int Ƿ,int Ȇ,string ǵ){foreach(var ż in ɇ.Where(ż=>ż.Ð!=null&&ż.Ò.
Equals(ǵ)))if(Ȇ<0)ż.Õ+=Ƿ;else ż.Õ=Ȇ;}void ȅ(int Ƿ,string ǵ){foreach(var ż in ɇ.Where(ż=>ż.Ð!=null&&ż.Ò.Equals(ǵ)))ż.Ð.I(Ƿ);}
void L(string ǵ){foreach(var ż in ɇ.Where(ż=>ż.Ð!=null&&ż.Ò.Equals(ǵ)))ż.Ð.L();}void M(string ǵ){foreach(var ż in ɇ.Where(ż
=>ż.Ð!=null&&ż.Ò.Equals(ǵ)))ż.Ð.M();}void N(string ǵ){foreach(var ż in ɇ.Where(ż=>ż.Ð!=null&&ż.Ò.Equals(ǵ)))ż.Ð.N();}void
Ƕ(int Ƿ,int Đ,string ǵ){foreach(var ż in ɇ.Where(ż=>ż.Ð!=null&&ż.Ò.Equals(ǵ))){if(Đ<0)ż.Î+=Ƿ;else ż.Î=Đ;if(ż.Õ==0)ż.Õ=1;}
}void Ǹ(int Q,string ǵ){foreach(var ż in ɇ.Where(ż=>ż.Ð!=null&&ż.Ò.Equals(ǵ)))ż.Ð.O(Q,maxCS);}void K(string ǵ){foreach(
var ż in ɇ.Where(ż=>ż.Ð!=null&&ż.Ò.Equals(ǵ)))ż.Ð.K();}void ǹ(int Ë,string ǵ){foreach(var ż in ɇ.Where(ż=>ż.Ï!=null&&ż.Ò.
Equals(ǵ)))ż.Ì(Ë);}void ȩ(){switch(ɕ){case 0:ɖ.Ȓ();Ɔ.ƅ=ƍ.ƅ=ŭ.ƅ=Ƙ.ƅ=Ɩ.ƅ=false;ɖ.Ȗ("uninstalling...\n");ɖ.ȓ(
"cleaning all oxygen tanks");break;case 1:ɀ.Clear();List<IMyGasTank>Ȫ=new List<IMyGasTank>();GridTerminalSystem.GetBlocksOfType<IMyGasTank>(Ȫ,ȫ=>!ȫ
.BlockDefinition.SubtypeId.Contains("Hydro"));ȱ(Ȫ);ɖ.ȕ();ɖ.ȓ("cleaning all air vents");break;case 2:Ɂ.Clear();List<
IMyAirVent>Ȭ=new List<IMyAirVent>();GridTerminalSystem.GetBlocksOfType<IMyAirVent>(Ȭ);ȱ(Ȭ);ɖ.ȕ();ɖ.ȓ(
"cleaning all doors and gates");break;case 3:ɂ.Clear();ş.Clear();List<IMyDoor>ȭ=new List<IMyDoor>();GridTerminalSystem.GetBlocksOfType<IMyDoor>(ȭ);ȱ(ȭ
);ɖ.ȕ();ɖ.ȓ("cleaning all warning lights");break;case 4:Ƀ.Clear();List<IMyReflectorLight>Ȯ=new List<IMyReflectorLight>();
GridTerminalSystem.GetBlocksOfType<IMyReflectorLight>(Ȯ);ȱ(Ȯ);ɖ.ȕ();ɖ.ȓ("cleaning saved data");break;case 5:ɢ=false;Storage=string.Empty;ɖ
.ȕ();ɖ.ȓ("cleaning all screens");break;case 6:List<IMyTerminalBlock>ȯ=new List<IMyTerminalBlock>();GridTerminalSystem.
GetBlocksOfType<IMyTextSurfaceProvider>(ȯ);ȱ(ȯ);ɖ.ȕ();ɖ.ȓ("cleaning all sensors");break;case 7:Ʉ.Clear();List<IMySensorBlock>Ȱ=new List
<IMySensorBlock>();GridTerminalSystem.GetBlocksOfType<IMySensorBlock>(Ȱ);ȱ(Ȱ);ɖ.ȕ();break;case 8:ɖ.Ȗ(
"uninstalling...done\n");break;case 9:ɖ.Ȗ("script running terminated");Runtime.UpdateFrequency=UpdateFrequency.Once;break;default:break;}ɕ++;}
void ȱ<D>(List<D>Ŵ)where D:IMyTerminalBlock{foreach(var Y in Ŵ){if(Y.CustomName.EndsWith(ɋ))Y.CustomName=Y.CustomName.Remove
(Y.CustomName.Length-ɋ.Length-1,ɋ.Length+1);if(Y.CustomData.Contains(Ɋ)){StringBuilder ÿ=new StringBuilder();int Ă=0;
string[]Ā=Y.CustomData.Split('\n');while(Ă<Ā.Length&&!Ā[Ă].Contains(Ɋ)){ÿ.AppendLine(Ā[Ă]);Ă++;}if(Ă>=Ā.Length)return;try{if(Y
.CustomName.Contains("Seat"))throw new System.ArgumentNullException();}catch{}string[]Ȳ=Ā[Ă].Split(',');if(Ȳ.Length>1){
string ȳ="";for(int ā=0;ā<Ȳ.Length;ā++){if(!Ȳ[ā].Contains(Ɋ))ȳ+=Ȳ[ā].Trim(',',' ')+", ";}ÿ.Append(ȳ.TrimEnd(',',' ')+"\n");Ă++
;while(Ă<Ā.Length&&!Ā[Ă].Contains(Ɏ)){if(Ā[Ă].Contains('=')){string[]č=Ā[Ă].Split('=',',','>','<');LayoutTypes Ⱦ;if(Enum.
TryParse<LayoutTypes>(č[1],out Ⱦ))ÿ.Append(č[0]+"=\n");else ÿ.AppendLine(Ā[Ă]);}else ÿ.AppendLine(Ā[Ă]);Ă++;}ÿ.AppendLine(Ɏ);Ă++
;while(Ă<Ā.Length&&!Ā[Ă].Contains(ɏ)){if(Ā[Ă].Contains('=')){string[]č=Ā[Ă].Split('=');BlockSetupTags Ⱦ;if(Enum.TryParse<
BlockSetupTags>(č[0],out Ⱦ)){if((int)Ⱦ<3)ÿ.AppendLine(Ā[Ă]);}else ÿ.AppendLine(Ā[Ă]);}else ÿ.AppendLine(Ā[Ă]);Ă++;}ÿ.AppendLine(ɏ);}
while(Ă<Ā.Length&&!Ā[Ă].Contains(ɑ))Ă++;Ă++;while(Ă<Ā.Length){ÿ.AppendLine(Ā[Ă]);Ă++;}Y.CustomData=ÿ.ToString().Trim('\n');}}
}static string Ƚ(string ȼ){string ȿ;if(!en)if(translations.TryGetValue(ȼ,out ȿ))return ȿ;return ȼ;}class Ȼ{public string
ȹ{get{string ȶ;if(ȸ)ȶ="working...";else ȶ="disabled";return"Room Control: "+ȶ+"\n";}}public bool ȸ{get;set;}public string
ȷ{get{string ȶ;if(ȵ)ȶ="working...";else ȶ="checking...";if(!leveling)ȶ="disabled";return"leveling O2: "+ȶ+"\n";}}public
bool ȵ{get;set;}StringBuilder ȴ=new StringBuilder();Dictionary<int,string>Ⱥ=new Dictionary<int,string>();StringBuilder Ȩ=new
StringBuilder();public Ȼ(){ȸ=false;ȵ=false;}public void Ã(){Ȩ.Clear();ȸ=ɉ;ȵ=false;}public void ȑ()=>Ⱥ.Clear();public void Ȓ()=>ȴ.
Clear();public void ȓ(string Ȕ)=>ȴ.Append(Ȕ+"...");public void ȕ()=>ȕ("done");public void ȕ(string ł)=>ȴ.AppendLine(ł);public
void Ȗ(string Ȕ)=>ȴ.Append(Ȕ);public void ȗ(string Ș){int ș=Ș.GetHashCode();if(!Ⱥ.ContainsKey(ș))Ⱥ.Add(ș,Ș);}public string Ț
(){if(ɠ>=Ⱥ.Count)ɠ=0;if(Ⱥ.Count>0)return"argument \"resetErrors\"\ndeletes all error messages\nERROR: "+(ɠ+1)+" of "+Ⱥ.
Count+"\n"+Ⱥ.ElementAt(ɠ).Value+"\n";else return"";}public string ț(){StringBuilder è=new StringBuilder("");if(Ⱥ.Count>0)è.
Append("argument \"resetErrors\"\ndeletes all error messages\n");string[]ȧ=Ⱥ.Values.ToArray();for(int ā=0;ā<ȧ.Length;ā++)è.
Append("ERROR: "+(ā+1)+" of "+ȧ.Length+"\n"+ȧ[ā]+"\n");return è.ToString();}public string ȥ(){if(ȴ.Length>0)return ȴ+"\n";else
return"";}public string Ȥ(){if(Ȩ.Length>0)return Ȩ+"\n";else return"";}public string ȣ()=>"max. Conveyors available: "+maxCS+
"\n"+ȹ+ȷ;public string Ȣ()=>"-------- Room Program --------\n"+ț()+"\n"+ȥ()+ȣ()+"\n"+Ȥ();}interface ȡ{bool Ȧ{get;set;}string
Ƞ();string ȟ(bool ȝ=false);string Ȟ(bool ȝ);bool Ĺ{get;set;}bool Ļ{get;}bool Ǻ{get;}bool Ȝ{get;}int ɰ{get;}string ʑ{get;}
bool ʶ{get;set;}List<Ń>ʷ{get;}List<Ń>ʸ{get;}Dictionary<string,List<Ě>>ʹ{get;}Dictionary<string,List<Ě>>ʺ{get;}List<ʤ>ʻ{get;}
bool ē{get;}bool ʼ{get;}float ʽ{get;}void ʾ();void ʿ();bool ˀ(ȡ ɽ);void ˁ(ȡ ɾ);void ˆ(ȡ ɿ);void ˏ();void ˎ();void ˍ(List<Ń>ˌ
,List<Ě>ş,List<đ>Ʉ,List<Ė>ː);}interface ˋ{long Ñ{get;}int P{get;}bool ˊ{get;set;}bool ˉ{get;set;}bool ˈ{get;set;}Vector2
ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null);bool ʬ{get;}void ʢ();void ʣ(bool ĭ);Vector2 ǩ(Vector2 Ä,ǯ Ö);}interface ʤ:ˋ{string ʑ{get;}
new int P{get;set;}bool ʥ{get;}void ʦ();Vector2 ǩ(Vector2 Ä,Vector2 Å,ǯ Ö,ˋ Ô);}interface ʵ:ʤ{float ʳ{get;}float ʲ{get;}
float ʱ{get;}bool ʰ{get;}bool ʯ{get;}}interface ʮ:ʤ{ȡ ʴ{get;}bool ʭ{get;}string ʫ{get;}bool ʪ(ȡ Đ);}class ʩ:ˋ{public long Ñ{
get;}public int P{get;}Action<bool>ʨ;Func<bool>ʧ;string ʡ;string ˑ;public bool ˊ{get{return ʧ();}set{ʨ(value);}}public bool
ˉ{get{return ʧ();}set{ʨ(value);}}public bool ˈ{get{return ʧ();}set{ʨ(value);}}public bool ʬ{get{return false;}}public ʩ(
Action<bool>Δ,Func<bool>Ε,string Ζ,string Ȕ){P=-1;ʨ=Δ;ʧ=Ε;ˑ=Ȕ;ʡ=Ζ;Ñ=GetHashCode();}public void ʣ(bool ĭ){return;}public void ʢ
(){return;}public Vector2 ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null){if(!Ö.ƿ)Ö.Ʈ(0.65f);Vector2 Į=new Vector2(Ĕ.Center.X,Ĕ.Y);Į.Y+=Ö.ƶ(
Į,ˑ,mainColor,TextAlignment.CENTER).Y;if(ˊ)Į.Y+=Ö.ƶ(Į,Ƚ("deactivate"),mainColor,TextAlignment.CENTER).Y;else Į.Y+=Ö.ƶ(Į,Ƚ
("activate"),mainColor,TextAlignment.CENTER).Y;Ö.ư();return Į;}public Vector2 ǩ(Vector2 Ä,ǯ Ö){Vector2 Į=Ä,Å=new Vector2(
Ö.ǒ);float ł=Ö.Ǒ/2;Į.X+=Ö.ǣ(Ä,Ö.ǒ,ˊ).X+(Ö.Ǒ/2);Å.X+=Ö.ƶ(Į,ʡ,mainColor,TextAlignment.LEFT).X+(Ö.Ǒ/2);if(Ö.H!=null&&Ñ==Ö.H.
Ñ)Ö.Ǩ(Ä-new Vector2(ł*2),Å.X+(4*ł),Å.Y+(4*ł),selectionColor,false,ł);return Å;}}class Η:ˋ{public long Ñ{get;}public int P
{get;}Func<float,float>Θ;float Ι;const float Q=0.05f;string ˑ;public bool ˊ{get{return false;}set{return;}}public bool ˉ{
get{return false;}set{if(value)Ι=Θ(-Q);}}public bool ˈ{get{return false;}set{if(value)Ι=Θ(Q);}}public bool ʬ{get{return
false;}}public Η(Func<float,float>Δ,float Ε,string Ȕ){P=-1;Θ=Δ;Ι=Ε;ˑ=Ȕ;Ñ=GetHashCode();}public void ʣ(bool ĭ){return;}public
void ʢ(){return;}public Vector2 ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null){if(!Ö.ƿ)Ö.Ʈ(0.65f);Vector2 Į=new Vector2(Ĕ.Center.X,Ĕ.Y);Į.Y+=Ö
.ƶ(Į,ˑ,mainColor,TextAlignment.CENTER).Y;Į.X=Ĕ.X;if(Ö.ǀ){Ö.ƶ(Į,(-Q).ToString("P0",ɓ),mainColor,TextAlignment.LEFT);Į.X=Ĕ.
Right;Ö.ƶ(Į,"+"+Q.ToString("P0",ɓ),mainColor,TextAlignment.RIGHT);}else{Ö.ƶ(Į,Ƚ("Primary")+": "+Ƚ("decrease limit by ")+Q.
ToString("P0",ɓ),mainColor,TextAlignment.LEFT);Į.X=Ĕ.Right;Ö.ƶ(Į,Ƚ("Secondary")+": "+Ƚ("increase limit by ")+Q.ToString("P0",ɓ),
mainColor,TextAlignment.RIGHT);}Ö.ư();return Į;}public Vector2 ǩ(Vector2 Κ,ǯ Ö){Vector2 Į=Κ;Vector2 Å=new Vector2(Ö.ó.
MeasureStringInPixels(new StringBuilder(1f.ToString("P0",ɓ)),Ö.ǆ,Ö.ǔ).X,Ö.ǒ);float ł=Ö.Ǒ/2;Ö.ƶ(Į,Ι.ToString("P0",ɓ),mainColor,TextAlignment.
CENTER);if(Ö.H!=null&&Ñ==Ö.H.Ñ){Į.X=Κ.X-(Å.X/2);Ö.Ǩ(Į-new Vector2(ł*2),Å.X+(4*ł),Å.Y+(4*ł),selectionColor,false,ł);}return Å;}
}class ʴ:ȡ{static public int Γ{get;set;}public int ɰ{get;}public string ʑ{get;private set;}public bool Ȧ{get;set;}=true;
public bool ʶ{get{return Ή&&ʸ.Count>0;}set{Ή=value;}}bool Ή=true;public List<Ń>ʷ{get;private set;}public Dictionary<string,
List<Ě>>ʹ{get;}public Dictionary<string,List<Ě>>ʺ{get;}public List<ʤ>ʻ{get;private set;}List<Ě>ˠ=new List<Ě>();public bool Ĺ
{get{return ˡ;}set{ʜ(value);}}public float ʽ{get;private set;}public bool Ļ{get{return ʽ>0.5;}}bool ˡ=false;bool ˢ=true;
bool ˣ=false,ˤ=false,ʝ=false;public string ˬ{get{return Ȟ();}}enum ˮ{Ͱ,ͱ,Ͳ,ͳ,ʹ,Ͷ,ͷ,ͺ,Β,Α,ΐ};static string[]Ώ={Ƚ(
"ready to use"),Ƚ("leveling tanks"),Ƚ("entrance requested"),Ƚ("match pressure"),Ƚ("ready to use"),Ƚ("checking rooms"),Ƚ(
"exit requested"),Ƚ("match pressure"),Ƚ("ready to leave"),Ƚ("ready to use"),Ƚ("OFFLINE - no air")};List<Ė>ː=new List<Ė>();public bool Ǻ{
get;}public bool Ȝ{get;}public List<Ń>ʸ{get;private set;}List<đ>Ύ=new List<đ>();Dictionary<ȡ,List<Ě>>Ό=new Dictionary<ȡ,
List<Ě>>();public bool ē{get{return Ύ.Any(ß=>ß.ē);}}public bool ʼ{get{return Ά==ˮ.ΐ||(ʷ.Count==0&&ʸ.Count==0);}}bool Ί=true;
int Έ=0;ˮ Ά=ˮ.Ͱ;ȡ ɾ=null,ɿ=null;bool ͽ,ͼ;Func<ȡ,ȡ,bool>ͻ=(ʠ,ɼ)=>Math.Abs(ʠ.ʽ-ɼ.ʽ)<0.2;static public string ʏ(ȡ[]ʁ){
StringBuilder ɣ=new StringBuilder(ʁ.Length+">");for(int ā=0;ā<ʁ.Length;ā++)if(ā<ʁ.Length-1)ɣ.Append(ʁ[ā].Ȧ+"-"+ʁ[ā].ʶ+",");else ɣ.
Append(ʁ[ā].Ȧ+"-"+ʁ[ā].ʶ);return ɣ.ToString();}static public void ʂ(string ʃ,ȡ[]ʁ){try{int ʄ=ʁ.Length;int ʅ=int.Parse(ʃ.Split(
'>')[0]);if(ʅ!=ʄ)throw new System.FormatException();string[]ʆ=ʃ.Split('>')[1].Split(',');string[]ʇ;for(int ā=0;ā<ʁ.Length;ā
++){ʇ=ʆ[ā].Split('-');ʁ[ā].Ȧ=bool.Parse(ʇ[0]);ʁ[ā].ʶ=bool.Parse(ʇ[1]);}}catch{ɖ.ȗ(
"Loading room data failed\ndefault values used");return;}}public ʴ(int ȉ,string ź=""){Γ=Math.Max(Γ,ȉ);ɰ=ȉ;ʑ=ź;if(ʑ=="")ʑ="Room: "+ɰ;ʷ=new List<Ń>();ʸ=new List<Ń>();ʹ=
new Dictionary<string,List<Ě>>();ʺ=new Dictionary<string,List<Ě>>();ʻ=new List<ʤ>();if(ʑ.Contains("irlock"))Ǻ=true;else Ȝ=ʑ
.Contains("angar");}public string Ƞ(){if(ɉ){if(Ȧ){if(Ǻ||Ȝ)return Ώ[(int)Ά];else return Ƚ("automatic control");}else
return Ƚ("manual room control");}else return Ƚ("manual block control");}public string ȟ(bool ȝ=false){string è="";if(!ȝ)è=Ƚ(
"O2 Level")+": ";if(ʽ<0.8)return è+Ƚ("lethal");else return è+Ƚ("healthy");}public string Ȟ(bool ȝ=false){float ʎ=0;int ʈ=0;
StringBuilder è=new StringBuilder();è.Append(Ƞ()+" - ");è.AppendLine(ȟ(ȝ));if(ʷ.Count>0){if(!ȝ)è.Append(Ƚ("Main")+": ");foreach(var ə
in ʷ){ʈ++;ʎ+=ə.ĵ;}è.Append((ʎ/ʈ).ToString("P1",ɓ));}if(ʸ.Count>0){if(leveling)è.Append(" <=> ");else è.Append(" | ");if(!ȝ
)è.Append(Ƚ("Backup")+": ");ʈ=0;ʎ=0;foreach(var ʍ in ʸ){ʈ++;ʎ+=ʍ.ĵ;}è.Append((ʎ/ʈ).ToString("P1",ɓ));}return è.ToString()
;}public void ˍ(List<Ń>ʌ,List<Ě>ɱ,List<đ>ʋ,List<Ė>ʊ){ʷ.Clear();ʸ.Clear();ʻ.Clear();Ύ.Clear();ː=ʊ.Where(Ə=>Ə.ʴ.ɰ==ɰ).
ToList();ʽ=0;ˣ&=Ή;float ʉ=0;int ʈ=0;ͽ=ͼ=false;if(ɰ<0)return;if(ɰ==0){foreach(var ə in ʌ.Where(ə=>ə.ʴ.ɰ==0)){ʷ.Add(ə);ʻ.Add(ə);
if(!ɉ)ə.Ĺ=ə.ˊ=true;if(ə.ʥ){ʉ+=ə.ķ;ʈ++;}}if(ʈ>0)ʽ=ʉ/ʈ;ɹ(ɱ);}if(ɰ>0){foreach(var ə in ʌ.Where(ə=>ə.ʴ.ɰ==ɰ)){if(Ǻ||Ȝ){if(ə.ʑ.
Contains("ackup"))ʸ.Add(ə);else ʷ.Add(ə);}else ʷ.Add(ə);ʻ.Add(ə);if(ə.ʥ){ʉ+=ə.ķ;ʈ++;}ͽ|=u.Á[ə.P];ͼ|=u.Â[ə.P];}if(ʈ>0)ʽ=ʉ/ʈ;
foreach(var ß in ʋ)if(ß.ʪ(this))Ύ.Add(ß);ɹ(ɱ);if(!Program.Æ&&ɉ){if((ˡ&&!ͼ)||(!ˡ&&!ͽ))Ά=ˮ.ΐ;ʞ();ʟ();if(!Ȧ)Ά=ˮ.Α;if(Ȧ&&Ǻ)ɶ();if(Ȧ
&&Ȝ)ʀ();ɖ.ȵ|=ˣ;}}}void ɹ(List<Ě>ɱ){ȡ ɲ;Ό.Clear();ˠ.Clear();ʺ.Clear();ʹ.Clear();foreach(var Ŕ in ɱ.Where(Ŕ=>Ŕ.ʪ(this))){ɲ=Ŕ
.Ŗ(this);if(Ŕ.ʭ)ɵ(Ŕ,ʹ);else ɵ(Ŕ,ʺ);if(!Ό.ContainsKey(ɲ))Ό.Add(ɲ,new List<Ě>{Ŕ});else Ό[ɲ].Add(Ŕ);}foreach(var ɳ in ʺ.
Values)ˠ.AddRange(ɳ);foreach(var ɴ in ʹ.Values)ˠ.AddRange(ɴ);ʻ.AddRange(ˠ);}void ɵ(Ě ě,Dictionary<string,List<Ě>>Ś){if(ě.Ĭ){if
(Ś.ContainsKey(ě.ʫ))Ś[ě.ʫ].Add(ě);else Ś.Add(ě.ʫ,new List<Ě>(){ě});}else Ś.Add(ě.ʑ,new List<Ě>(){ě});}void ɶ(){if(ʷ.Count
==0||Ύ.Count==0||Ό.Keys.Count<2){if(Ί)ɖ.ȗ(ʑ+" is not working.\n"+"Neighboring rooms: "+Ό.Count+" - 2 needed\nVents: "+ʷ.
Count+" - 1 needed\nSensors: "+Ύ.Count+" - 1 needed");Ί=false;return;}else Ί=true;bool ɷ=ˠ.Any(Ŕ=>Ŕ.İ);bool ɸ=ē;if(Έ>
waitTimer)Ά=ˮ.Α;switch(Ά){case ˮ.ΐ:Έ++;ɾ=ɿ=null;ˣ=false;break;case ˮ.Α:foreach(var Ŕ in ˠ)Ŕ.Ō();Έ=0;ɾ=ɿ=null;ˣ=leveling&&Ή&&!ɸ&&!
ɷ&&ɾ==null&&ɿ==null&&ʸ.Count>0;if(ˣ)Ά=ˮ.ͱ;else Ά=ˮ.Ͱ;break;case ˮ.Ͱ:if(ɸ&&ɷ){Έ=0;ɾ=ˠ.First(Ŕ=>Ŕ.İ).Ŗ(this);Ά=ˮ.Ͷ;}else Έ
++;break;case ˮ.ͱ:ʙ();if(!ˣ){ʾ();Ά=ˮ.Ͱ;}break;case ˮ.Ͳ:ʜ(ɾ.Ļ);Ά=ˮ.ͳ;break;case ˮ.ͳ:if(ͻ(this,ɾ)){foreach(var ɺ in Ό.Where(
ɺ=>ͻ(this,ɺ.Key)))ɺ.Value.ForEach(Ŕ=>{Ŕ.ň=false;Ŕ.ō();});Έ=0;Ά=ˮ.ʹ;}else Έ++;break;case ˮ.ʹ:if(ɸ)Ά=ˮ.Ͷ;else Έ++;break;
case ˮ.Ͷ:bool ɻ=false;foreach(var ɺ in Ό.Where(ɺ=>ɺ.Key.ɰ!=ɾ.ɰ&&ͻ(this,ɺ.Key))){ɺ.Value.ForEach(Ŕ=>{Ŕ.ň=false;Ŕ.ō();});ɻ=
true;}if(ɻ){ɿ=null;Ά=ˮ.Β;}else{ɿ=Ό.Keys.First(ɼ=>!ͻ(this,ɼ));Ά=ˮ.ͷ;}ɾ=null;break;case ˮ.ͷ:ʜ(ɿ.Ļ);Ά=ˮ.ͺ;break;case ˮ.ͺ:if(ͻ(
this,ɿ)){foreach(var ɺ in Ό.Where(ɺ=>ͻ(this,ɺ.Key)))ɺ.Value.ForEach(Ŕ=>{Ŕ.ň=false;Ŕ.ō();});Έ=0;Ά=ˮ.Β;ɿ=null;}else Έ++;break;
case ˮ.Β:if(!ɸ)Ά=ˮ.Α;else Έ++;break;default:Ά=ˮ.Α;break;}}public bool ˀ(ȡ ɽ)=>ͻ(this,ɽ);public void ˁ(ȡ ɾ){if(ē)return;ˣ=
false;this.ɾ=ɾ;ɿ=null;Έ=0;if(ˀ(ɾ))Ά=ˮ.ͳ;else Ά=ˮ.Ͳ;}public void ˆ(ȡ ɿ){if(!ē)return;ˣ=false;this.ɿ=ɿ;ɾ=null;Έ=0;if(ˀ(ɿ))Ά=ˮ.ͺ
;else Ά=ˮ.ͷ;}void ʀ(){if(ʷ.Count==0||Ύ.Count==0||!Ό.Keys.Any(ơ=>ơ.ɰ==0)){if(Ί)ɖ.ȗ(ʑ+" is not working.\n"+
"outside doors: "+ˠ.Where(Ŕ=>Ŕ.ʪ(Ɇ[0])).Count()+" - 1 needed\nVents: "+ʷ.Count+" - 1 needed\nSensors: "+Ύ.Count+" - 1 needed");Ί=false;
return;}else Ί=true;bool ɷ=ˠ.Any(Ŕ=>Ŕ.İ);bool ɸ=ē;if(Έ>waitTimer)Ά=ˮ.Α;switch(Ά){case ˮ.ΐ:Έ++;ɾ=ɿ=null;ˣ=false;break;case ˮ.Α:
Έ=0;ɾ=ɿ=null;ˣ=leveling&&Ή&&!ɸ&&!ɷ&&ɾ==null&&ɿ==null&&ʸ.Count>0;if(ˣ)Ά=ˮ.ͱ;else Ά=ˮ.Ͱ;break;case ˮ.Ͱ:Έ++;break;case ˮ.ͱ:ʙ
();if(!ˣ){ʾ();Ά=ˮ.Ͱ;}break;case ˮ.Ͳ:ʜ(ɾ.Ļ);Ά=ˮ.ͳ;break;case ˮ.ͳ:if(ͻ(this,ɾ)){foreach(var ɺ in Ό.Where(ɺ=>ɺ.Key.ɰ==0))ɺ.
Value.ForEach(Ŕ=>{Ŕ.ň=false;Ŕ.ō();});Έ=0;Ά=ˮ.Ͱ;}else Έ++;break;case ˮ.ͷ:ʜ(true);Ά=ˮ.ͺ;break;case ˮ.ͺ:if(Ļ){foreach(var ɺ in Ό
.Where(ɺ=>ͻ(this,ɺ.Key)))ɺ.Value.ForEach(Ŕ=>Ŕ.ň=false);Έ=0;Ά=ˮ.Ͱ;ɿ=null;}else Έ++;break;default:Ά=ˮ.Ͱ;break;}}public void
ˏ(){ˣ=false;ɾ=Ɇ[0];if(ˀ(ɾ))Ά=ˮ.ͳ;else Ά=ˮ.Ͳ;}public void ˎ(){ˣ=false;if(Ļ)Ά=ˮ.ͺ;else Ά=ˮ.ͷ;}void ʙ(){if(!(Program.
leveling&&Ή)){ˣ=false;return;}if(Math.Abs(maxVentCS-minVentCS)<0.1||maxVentCS<minVentCS){leveling=ˣ=false;ɖ.ȗ(
"min. or max. value for\nairlock O2 leveling are wrong");return;}ˣ&=ɾ==null&&ɿ==null&&!ˠ.Any(Ŕ=>Ŕ.İ)&&ʸ.Count>0;if(ˣ){float ʚ=(maxVentCS+minVentCS)/2;foreach(var ə in ʷ){if(ə.
ĵ<minVentCS){ə.ˊ=ˤ=true;ə.Ĺ=!ˤ;ʝ|=true;}if(ə.ĵ>maxVentCS){ə.Ĺ=ə.ˊ=true;ˤ=false;ʝ|=true;}if(ə.ĵ>(ʚ-0.025)&&ə.ĵ<(ʚ+0.025)||
!ʝ)ˣ=ˤ=ʝ=false;}foreach(var ʍ in ʸ){ʍ.ˊ=ʝ;ʍ.Ĺ=ˤ;}}else ʾ();}public void ʾ(){ˣ=ʝ=false;ʷ.ForEach(ə=>{ə.Ĺ=ˢ;ə.ˊ=true;});ʸ.
ForEach(ʍ=>{ʍ.Ĺ=ˢ;ʍ.ˊ=false;});Έ=0;}void Ę(bool ę)=>ː.ForEach(Ə=>Ə.Ę(ę));public void ʿ(){if(Ά!=ˮ.ΐ)ˠ.ForEach(Ŕ=>Ŕ.ň=true);}void
ʟ(){if(Ά==ˮ.ΐ){ˣ=false;ˠ.ForEach(Ŕ=>Ŕ.ň=false);if((ˡ&&ͼ)||(!ˡ&&ͽ))Ά=ˮ.Α;return;}if(!ˣ){foreach(var Ŕ in ˠ)Ŕ.ň=!ͻ(this,Ŕ.Ŗ
(this));if((ˢ&&ʽ<0.8)||(!ˢ&&ʽ>0.2))ʿ();}}void ʞ(){if(!Ȧ&&(ʷ.Any(ə=>ə.ʥ&&ə.Ĺ!=ˢ||ʸ.Any(ʍ=>ʍ.ʥ&&ʍ.Ĺ!=ˢ))))ʜ(!ˢ);bool ʝ=true
;if(!ˣ&&!ˠ.Any(Ŕ=>Ŕ.İ)){foreach(var ə in ʷ){ə.ˊ=true;ə.Ĺ=ˡ;ʝ&=!((ˡ&&ə.ĵ>0.05)||(!ˡ&&ə.ĵ<0.95));}foreach(var ʍ in ʸ){ʍ.ˊ=ʝ
&&!((ˡ&&ʽ>=0.8)||(!ˡ&&ʽ<0.2));ʍ.Ĺ=ˡ;}}Ę(ˣ||(ē&&!ˡ));}void ʜ(bool ʛ){Ę(ē&&!ʛ);if(ɰ>0)ˠ.ForEach(Ŕ=>Ŕ.Ō());Έ=0;ˢ=ˡ=ʛ;if(!Ȧ){ʷ
.ForEach(ə=>ə.Ĺ=ʛ);ʸ.ForEach(ʍ=>ʍ.Ĺ=ʛ);}}}abstract class ʘ{protected IMyTerminalBlock Y;protected List<BlockSetupTags>ʐ=
new List<BlockSetupTags>();static protected string[]W=new string[Enum.GetValues(typeof(BlockSetupTags)).Length];public
string ʑ{get{return Ğ();}}string ź="";public long Ñ{get{return Y.EntityId;}}public int P{get{return ʒ;}set{Ģ(value);}}int ʒ=0;
protected ȡ Đ=Ɇ[-1];protected ȡ ʓ=Ɇ[-1];protected string ʔ="";protected int g=0;bool ʕ=false;static ʘ(){W[0]=
" - shown name in main screen";W[1]=" - conveyor connected system number";W[2]=" - room number - 0 for outside vent";W[3]=
" - doors inside room number - 0 not possible";W[4]=" - doors outside room number - 0 for atmosphere";W[5]=" - group name, if it should be treated in a group";}public
ʘ(IMyTerminalBlock Y){if(Y!=null){this.Y=Y;if(ɟ)Y.CustomData="";}}protected void ü(params BlockSetupTags[]ý){foreach(var
ã in ý)if(!ʐ.Contains(ã))ʐ.Add(ã);}protected void ʗ(){if(ɟ)Y.CustomData="";if(g!=Y.CustomData.GetHashCode()){ʕ=false;Y.
CustomData=þ();g=Y.CustomData.GetHashCode();if(!ʕ&&Y.CustomName.EndsWith(ɋ))Y.CustomName=Y.CustomName.Remove(Y.CustomName.Length-ɋ
.Length-1,ɋ.Length+1);}}string þ(){var ÿ=new StringBuilder();string[]Ā=Y.CustomData.Trim().Trim('\n').Split('\n');int ā,Ă
=0;ʕ=false;while(Ă<Ā.Length&&!Ā[Ă].Trim().StartsWith("@")){ÿ.AppendLine(Ā[Ă]);Ă++;}if(Ă<Ā.Length){if(!Ā[Ă].Contains(Ɋ))ÿ.
Append(Ā[Ă].Trim()+", "+Ɋ+"\n");else ÿ.Append(Ā[Ă].Trim()+"\n");Ă++;}else ÿ.Append(Ɋ+"\n");List<BlockSetupTags>Ď=ʐ.ToList();
string[]č;ÿ.Append(Ɏ+"\n");if(Ă<Ā.Length&&Ā[Ă].StartsWith(Ɏ)){Ă++;while(Ă<Ā.Length&&!Ā[Ă].StartsWith(ɏ)){if(Ā[Ă].Contains('=')
){č=Ā[Ă].Split('=');ā=Ď.IndexOf((BlockSetupTags)Enum.Parse(typeof(BlockSetupTags),č[0]));if(ā>=0){ö(Ď[ā],č[1]);Ď.RemoveAt
(ā);}}ÿ.Append(Ā[Ă]+"\n");Ă++;}Ă++;}if(Ď.Count>0){ʕ|=true;foreach(var ã in Ď)ÿ.Append(ã+â(ã)+"\n");}ÿ.Append(ɏ+"\n"+ɐ+
"\n");Ď=ʐ.ToList();if(Ă<Ā.Length&&Ā[Ă].StartsWith(ɐ)){Ă++;while(Ă<Ā.Length&&!Ā[Ă].StartsWith(ɑ)){if(Ā[Ă].Contains('-')){č=Ā[
Ă].Trim().Split('-');ā=Ď.IndexOf((BlockSetupTags)Enum.Parse(typeof(BlockSetupTags),č[0]));if(ā>=0)Ď.RemoveAt(ā);}ÿ.Append
(Ā[Ă]+"\n");Ă++;}Ă++;}if(Ď.Count>0)foreach(var ã in Ď)ÿ.Append(ã+W[(int)ã]+"\n");ÿ.Append(ɑ+"\n");while(Ă<Ā.Length){if(Ā[
Ă].StartsWith("[readme")){do Ă++;while(Ă<Ā.Length&&!Ā[Ă].StartsWith("[/readme"));Ă++;}if(Ă<Ā.Length)ÿ.Append(Ā[Ă]+"\n");Ă
++;}if(ʕ&&!Y.CustomName.EndsWith(ɋ)){Y.CustomName+=" "+ɋ;ɖ.ȗ("block setup needed\nsearch for blocks with\n"+ɋ+
" at its Custom Name");}return ÿ.ToString().Trim('\n').Trim();}void ö(BlockSetupTags ã,string Ú){bool ʖ=false;switch((int)ã){case 0:try{ź=Ú.
Trim();}catch{ź="";ʖ|=true;}break;case 1:try{ʒ=Int32.Parse(Ú)-1;}catch{ʒ=-1;ʖ|=true;}break;case 2:try{Đ=ɨ(int.Parse(Ú));}
catch{Đ=Ɇ[-1];ʖ|=true;}break;case 3:try{Đ=ɨ(int.Parse(Ú));}catch{Đ=Ɇ[-1];ʖ|=true;}break;case 4:try{ʓ=ɨ(int.Parse(Ú));}catch{Đ
=Ɇ[-1];ʖ|=true;}break;case 5:ʔ=Ú;break;}if(ʖ){ɖ.ȗ("couldn't read Custom Data\n"+Y.CustomName+"\n"+ã+"="+Ú);}ʕ|=ʖ;}string
â(BlockSetupTags ã){ʕ=true;string è="=";switch((int)ã){case 1:è+=(P+1);break;case 2:Đ=Ɇ[-1];break;case 3:Đ=Ɇ[-1];break;
case 4:Đ=Ɇ[-1];break;}return è;}string Ğ(){if(ź.Trim().Length>0)return ź;return Y.CustomName;}protected string ğ(
BlockSetupTags ã,string Ġ){var ġ=new StringBuilder();string[]Ā=Y.CustomData.Split('\n');for(int ā=0;ā<Ā.Length;ā++){if(Ā[ā].StartsWith
(ã.ToString(),StringComparison.Ordinal))ġ.Append(ã+"="+Ġ+"\n");else ġ.Append(Ā[ā]+"\n");}return ġ.ToString().TrimStart(
'\n').TrimEnd('\n');}void Ģ(int ģ){ʒ=ģ;Y.CustomData=ğ(BlockSetupTags.Conveyor,(ģ+1).ToString());g=Y.CustomData.GetHashCode()
;}}abstract class Ĥ:ʘ,ʤ{protected float ĥ;protected bool Ħ;protected bool ħ;protected Vector3I Ĩ;public bool ˊ{get{return
(Y as IMyFunctionalBlock).Enabled;}set{(Y as IMyFunctionalBlock).Enabled=value;}}public virtual bool ˉ{get{return false;}
set{return;}}public virtual bool ˈ{get{return false;}set{return;}}public bool ʥ{get{return Y.IsWorking;}}protected Color ĩ;
protected bool Ī{get{return!Y.IsFunctional;}}public bool ʬ{get;private set;}protected Ĥ(IMyTerminalBlock Y,params BlockSetupTags[
]ý):base(Y){if(Y!=null){ĥ=Y.CubeGrid.GridSize;Ħ=this.Y.CubeGrid.GridSize>2f;Ĩ=this.Y.Max-this.Y.Min+new Vector3I(1,1,1);ʬ
=false;ü(BlockSetupTags.Name);ü(ý);ʗ();}}public virtual Vector2 ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null){if(!Ö.ƿ)Ö.Ʈ(0.65f);Vector2 Į
=new Vector2(Ĕ.Center.X,Ĕ.Y);Į.Y+=Ö.ƶ(Į,ʑ+", CS: "+(P+1),mainColor,TextAlignment.CENTER).Y;return Į;}virtual protected
bool į(){if(Y.IsWorking&&!Ī){ĩ=colorOn;return true;}else{ĩ=colorOff;return false;}}public void ʣ(bool ĭ)=>ʬ=ĭ;public void ʢ(
)=>ʬ=!ʬ;virtual public void ʦ()=>ʗ();virtual public Vector2 ǩ(Vector2 Ä,ǯ Ö){return Ä;}abstract public Vector2 ǩ(Vector2
Ä,Vector2 Å,ǯ Ö,ˋ Ô);}abstract class ī:Ĥ,ʮ{public ȡ ʴ{get{return Đ;}}public bool ʭ{get{return Đ.ɰ==0||ʓ.ɰ==0;}}public
string ʫ{get{return ʔ;}}public ī(IMyTerminalBlock Y,params BlockSetupTags[]ý):base(Y,ý){}public bool ʪ(ȡ Đ)=>this.Đ.ɰ==Đ.ɰ||ʓ.
ɰ==Đ.ɰ;}class đ:ī,ʮ{IMySensorBlock Ē;public string Ò{get{return Ē.CustomName;}}public bool ē{get{return Ē.IsActive;}}
public đ(IMySensorBlock Ē):base(Ē,BlockSetupTags.Room){this.Ē=Ē;}override public Vector2 ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null)=>base.ˇ(
Ĕ,Ö);public override Vector2 ǩ(Vector2 Ä,Vector2 Å,ǯ Ö,ˋ Ô)=>new Vector2(Å.X,Å.Y);}class Ė:ī,ʮ{IMyReflectorLight ė;public
Ė(IMyReflectorLight ė):base(ė,BlockSetupTags.Room){ė.Color=warningColor;this.ė=ė;Ę(false);}public void Ę(bool ę)=>ė.
Enabled=ę;override public Vector2 ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null)=>base.ˇ(Ĕ,Ö);public override Vector2 ǩ(Vector2 Ä,Vector2 Å,ǯ Ö,ˋ
Ô)=>new Vector2(Å.X,Å.Y);}class Ě:ī,ʮ{public IMyDoor ě;List<Ě>Ĝ=new List<Ě>();public bool Ĭ{get;protected set;}public ȡ ĝ
{get{return ʓ;}}public bool İ{get{return ě.Status==DoorStatus.Open||ě.Status==DoorStatus.Opening;}}public bool ņ{get{
return ě.Status==DoorStatus.Closed;}}public bool Ň{get{return ě.Status==DoorStatus.Closed;}}public bool ň{get{return ŉ;}set{Œ(
value);}}bool ŉ=false;public bool Ŋ{get;private set;}public override bool ˉ{get{return ě.Status==DoorStatus.Closed;}set{if(Ĭ
&&ɉ)ŗ(value);else Ŏ(value);}}public Ě(IMyDoor ě):base(ě,BlockSetupTags.RoomInside,BlockSetupTags.RoomOutside,
BlockSetupTags.DoorGroup){this.ě=ě;if(ě is IMyAirtightHangarDoor||ě.BlockDefinition.SubtypeId.Contains("Gate"))Ŋ=true;}override public
Vector2 ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null){if(!Ö.ƿ)Ö.Ʈ(0.65f);Color ĩ=mainColor;string ŋ="";if(ʔ.Length>0)ŋ=ʫ+" <- ";ŋ+=ʑ;Vector2 Į=
new Vector2(Ĕ.Center.X,Ĕ.Y);Į.Y+=Ö.ƶ(Į,ŋ,mainColor,TextAlignment.CENTER).Y;ŋ=Ƚ("Door ");if(Ŋ)ŋ=Ƚ("Gate ");if(Ī){ŋ+=Ƚ("Open"
)+" - "+Ƚ("damaged");ĩ=warningColor;}else ŋ+=Ƚ(ě.Status.ToString("G"));Ö.ƶ(new Vector2(Ĕ.X,Į.Y),ŋ,ĩ,TextAlignment.LEFT);Į
.X=Ĕ.Right;if(ĕ!=null)Į.X-=Ö.ƶ(Į,Ŗ(ĕ).ʑ,mainColor,TextAlignment.RIGHT).X;if(ŉ)Į.Y+=Ö.ƶ(Į,Ƚ("sealed")+" - ",colorOff,
TextAlignment.RIGHT).Y;else Į.Y+=Ö.ƶ(Į,Ƚ("unsealed")+" - ",colorOn,TextAlignment.RIGHT).Y;if(ņ){if(Ö.ǀ)Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ(
"set to open"),mainColor,TextAlignment.LEFT);else Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ("Primary")+": "+Ƚ("set to open"),mainColor,TextAlignment
.LEFT);}else{if(Ö.ǀ)Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ("set to close"),mainColor,TextAlignment.LEFT);else Ö.ƶ(new Vector2(Ĕ.X,Į.Y
),Ƚ("Primary")+": "+Ƚ("set to close"),mainColor,TextAlignment.LEFT);}Ö.ư();return Į;}public void Ō()=>Ŏ(true);public void
ō()=>Ŏ(false);void Ŏ(bool ŏ){if(ŏ&&(ě.Status==DoorStatus.Open||ě.Status==DoorStatus.Opening))ě.CloseDoor();if(!ŏ&&(ě.
Status==DoorStatus.Closing||ě.Status==DoorStatus.Closed))ě.OpenDoor();}public void ř()=>ŗ(true);public void Ř()=>ŗ(false);void
ŗ(bool ŏ){if(Ĭ)Ĝ.ForEach(Ŕ=>Ŕ.Ŏ(ŏ));}public ȡ Ŗ(ȡ ŕ){if(ʴ.ɰ==ŕ.ɰ)return ĝ;return ʴ;}public override void ʦ(){ʗ();Ĝ.Clear(
);if(!ʔ.Equals("")){Ĭ=true;foreach(var ě in ş.Where(Ŕ=>Ŕ.ʪ(ʴ)&&Ŕ.ʫ.Equals(ʔ)))Ĝ.Add(ě);}else Ĭ=false;if(ŉ)Œ();}public
void Œ(bool ő=true){if(ɉ){if(ő){ě.Enabled=!ņ;Ŏ(true);}else ě.Enabled=true;ŉ=ő;}}public override Vector2 ǩ(Vector2 Ä,Vector2
Å,ǯ Ö,ˋ Ô){į();Color Ő=backgroundColor;Vector2 œ=Ä;float ł;if(Ŋ)ł=Å.Y;else{ł=Å.Y/2;if(ʓ.ɰ==0)œ.Y+=Å.Y-ł;}if(Ö.B&&ʬ){Ö.Ǩ(Ä
-new Vector2(0,Å.Y),Å.X,Å.Y*3,selectedColor,true);Ő=selectedColor;}if(Ŋ){Ö.Ƽ(œ,Å.X,ł,Ő);Ö.Ƽ(œ,Å.X*(1-ě.OpenRatio),ł,ĩ);}
else{Ö.Ƽ(œ,Å.X,ł,ĩ);Ö.ƻ(œ+new Vector2(Å.X/2,ł/2),Å.X*ě.OpenRatio,ł,Ő);}if(Ö.B&&Ô!=null&&Ñ==Ô.Ñ){ł=Ö.Ǒ;Ö.Ƽ(Ä-new Vector2(0,Å.
Y+(ł/2)),Å.X,ł/2,selectionColor);Ö.Ƽ(Ä+new Vector2(0,Å.Y*2),Å.X,ł/2,selectionColor);}if(Ī)Ö.ǩ(Ä+new Vector2(Å.X/2,Å.Y/2),
Å.Y*2.5f,"Danger",warningColor);return new Vector2(Å.X,Å.Y);}}class Ń:ī,ʮ{public const float w=1f;static public float Ĳ{
get;private set;}IMyAirVent ĳ;Color Ĵ;public float ĵ{get{return u.º[P]/u.À[P];}}public float Ķ{get;}public float ķ{get{
return ĳ.GetOxygenLevel();}}public bool ĸ{get{return ĳ.CanPressurize;}}public bool Ĺ{get{return!ĳ.Depressurize;}set{ĳ.
Depressurize=!value;}}public bool ĺ{get{return ʥ&&ĳ.Status==VentStatus.Pressurizing;}}public bool Ļ{get{return ʥ&&ĳ.Status==
VentStatus.Pressurized;}}public bool ļ{get{return ʥ&&ĳ.Status==VentStatus.Depressurizing;}}public bool Ľ{get{return ʥ&&ĳ.Status==
VentStatus.Depressurized;}}public override bool ˉ{get{if(ɉ)return Đ.Ĺ;else return!ĳ.Depressurize;}set{if(ɉ)Đ.Ĺ=value;else ĳ.
Depressurize=!value;}}public override bool ˈ{get{return ɉ&&Đ.Ȧ&&Đ.ʶ&&(Đ.Ǻ||Đ.Ȝ);}set{Đ.ʶ=ɉ&&(Đ.Ǻ||Đ.Ȝ)&&Đ.Ȧ&&value;}}public Ń(
IMyAirVent ĳ):base(ĳ,BlockSetupTags.Conveyor,BlockSetupTags.Room){this.ĳ=ĳ;ħ=false;if(Ħ)Ķ=300*60;else Ķ=30*60;ʦ();if(ʴ.ɰ==0)Ĺ=true
;else ʴ.Ĺ=!ĳ.Depressurize;}override public Vector2 ˇ(RectangleF Ĕ,ǯ Ö,ȡ ĕ=null){string Ņ=ĳ.Status.ToString("G");Color ĩ=
colorOn;if(ʥ&&(Ľ||ļ))ĩ=depressColor;if(!ʥ&&!Ī){ĩ=colorOff;Ņ=Ƚ("offline");}if(Ī){ĩ=warningColor;Ņ=Ƚ("damaged");}Vector2 Į=base.ˇ
(Ĕ,Ö);Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ("Tanks O2 level: ")+ĵ.ToString("P",ɓ),mainColor,TextAlignment.LEFT);Į.X=Ĕ.Right;Į.X-=Ö.ƶ
(Į,Ņ,ĩ,TextAlignment.RIGHT).X;Į.Y+=Ö.ƶ(Į,"Status: ",mainColor,TextAlignment.RIGHT).Y;if(ˉ){if(Ö.ǀ)Ö.ƶ(new Vector2(Ĕ.X,Į.Y
),Ƚ("depressurize"),mainColor,TextAlignment.LEFT);else{if(ɉ)Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ("Primary")+": "+Ƚ(
"depressurize room"),mainColor,TextAlignment.LEFT);else Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ("Primary")+": "+Ƚ("depressurize vent"),mainColor,
TextAlignment.LEFT);}}else{if(Ö.ǀ)Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ("pressurize"),mainColor,TextAlignment.LEFT);else{if(ɉ)Ö.ƶ(new Vector2(Ĕ.
X,Į.Y),Ƚ("Primary")+": "+Ƚ("pressurize room"),mainColor,TextAlignment.LEFT);else Ö.ƶ(new Vector2(Ĕ.X,Į.Y),Ƚ("Primary")+
": "+Ƚ("pressurize vent"),mainColor,TextAlignment.LEFT);}}if(ɉ&&leveling&&(Đ.Ǻ||Đ.Ȝ)&&Đ.ʸ.Count>0){Į.X=Ĕ.Right-Ö.ǒ;Ö.ǣ(Į,Ö.ǒ
,ˈ);Į.X-=Ö.Ǒ/2;if(Ö.ǀ)Ö.ƶ(Į,Ƚ("level oxygen tanks"),mainColor,TextAlignment.RIGHT);else Ö.ƶ(Į,Ƚ("Secondary")+": "+Ƚ(
"level oxygen tanks"),mainColor,TextAlignment.RIGHT);}Ö.ư();return Į;}override protected bool į(){bool ń=ĳ.IsWorking&&!Ī;if(ń){ĩ=colorOn;if(
!ĳ.CanPressurize){if(Đ.ɰ==0)ĩ=colorOn;else ĩ=leakColor;}if(!Ĺ)ĩ=depressColor;}else ĩ=colorOff;if(ĳ.GetOxygenLevel()<0.8){
if(ĳ.Depressurize)Ĵ=depressColor;else Ĵ=leakColor;}else Ĵ=colorOn;if(!ń)Ĵ=colorOff;return ń;}override public void ʦ(){ʗ();
į();if(ʴ.ɰ==0)Ĳ=ĳ.GetOxygenLevel();}public override Vector2 ǩ(Vector2 Ä,Vector2 Å,ǯ Ö,ˋ Ô){į();float ł=Ö.Ǒ/2;Ö.Ʈ(Å.X/100)
;float Ł=Ö.ǔ;float ŀ=Ö.ǒ;if(Ö.B&&this==Ö.H)Ö.Ǩ(Ä-new Vector2(ł*2),Å.X+(4*ł),Å.Y+(4*ł),selectionColor,false,ł);if(Ö.B&&ʬ)Ö
.Ǩ(Ä-new Vector2(ł),Å.X+(2*ł),Å.Y+(2*ł),selectedColor,true);ł=Math.Min(Å.X,Å.Y)*0.05f;Vector2 Į=Ä,Ŀ=Ä+new Vector2(Å.X,Å.Y
);;float ľ=Å.X-(2*ł);if(!Ħ){ľ*=0.75f;Ł*=0.75f;}Į.Y=Ä.Y+ł;Į.X=Ä.X+((Å.X-ľ)/2);Ö.Ǩ(Į,ľ,ľ,ĩ,false,ł);Ö.Ǩ(Į+new Vector2(ł),ľ-
(ł*2),ľ-(ł*2),backgroundColor,true);Ö.Ǩ(Į+new Vector2(ł,ł+((ľ-(2*ł))*(1-ĵ))),ľ-(ł*2),(ľ-(ł*2))*ĵ,o2Color,true);if(!ɒ){Ö.ƫ
(Ł);Ö.ƶ(Į+new Vector2(ł)," "+(P+1).ToString(),mainColor,TextAlignment.LEFT);}Į+=new Vector2(ľ/2);if(!Ī){float ı=ľ*0.2f;
Vector2 ď=new Vector2(ľ*0.22f,ı*2);Vector2 Ù=Į-new Vector2(0,ı);Ö.ǲ(new MySprite(SpriteType.TEXTURE,"Triangle",Ù,ď,ĩ,null,
TextAlignment.CENTER,(float)Math.PI));Vector2 p=new Vector2(Į.X-(float)(Math.Cos(Math.PI*2*30/360)*ı),Į.Y+(float)(Math.Sin(Math.PI*2*
30/360)*ı));Ö.ǲ(new MySprite(SpriteType.TEXTURE,"Triangle",p,ď,ĩ,null,TextAlignment.CENTER,(float)Math.PI/3));Vector2 q=
new Vector2(Į.X+(float)(Math.Cos(Math.PI*2*30/360)*ı),Į.Y+(float)(Math.Sin(Math.PI*2*30/360)*ı));Ö.ǲ(new MySprite(
SpriteType.TEXTURE,"Triangle",q,ď,ĩ,null,TextAlignment.CENTER,(float)Math.PI/-3));if((Ĺ&&!u.Â[P])||(!Ĺ&&!u.Á[P]))Ö.ǩ(Į,ľ*0.75f,
"Danger",mainColor);}else Ö.ǩ(Į,ľ,"Danger",mainColor);Ö.ư();return Å;}}class u:Ĥ,ʵ{public const float w=1/1.1618f;IMyGasTank z;
public float ʲ{get{return z.Capacity;}}public float ʱ{get{return ʲ*(float)ʳ;}}public bool ʰ{get{return ʥ&&ʳ<1;}}public bool ʯ{
get{return!ª&&ʥ&&ʳ>0;}}public float ʳ{get{return(float)z.FilledRatio;}}protected bool ª{get{return z.Stockpile;}}protected
bool µ{get{return z.AutoRefillBottles;}set{z.AutoRefillBottles=value;}}public static float[]º=new float[maxCS];public static
float[]À=new float[maxCS];public static bool[]Á=new bool[maxCS];public static bool[]Â=new bool[maxCS];public static void Ã(){
ɥ(º);ɥ(À);ɥ(Á);ɥ(Â);}public u(IMyGasTank z):base(z,BlockSetupTags.Conveyor){this.z=z;if(Ħ)ħ=(Ĩ.X*Ĩ.Y*Ĩ.Z*Math.Pow(ĥ,3))>
370f;else ħ=(Ĩ.X*Ĩ.Y*Ĩ.Z*Math.Pow(ĥ,3))>15f;}public override void ʦ(){ʗ();À[P]+=ʲ;if(ʰ)Á[P]|=true;if(ʯ){º[P]+=ʱ;Â[P]|=true;}
}public override Vector2 ǩ(Vector2 Ä,Vector2 Å,ǯ Ö,ˋ Ô)=>new Vector2(0);}interface Ó{string Ò{get;}long Ñ{get;}G Ð{get;}R
Ï{get;}int Õ{get;set;}int Î{get;set;}void Ì(int Ë);List<n>Ê();void É(bool È);void Ç(bool Æ=false);}interface n{
LayoutTypes A{get;}int P{get;}bool B{get;}void C<D>(List<D>E)where D:ʤ;void F();}interface G:n{ˋ H{get;}void I(int J);void K();void
L();void M();void N();void O(int Q,int k);}interface R:n{int S{get;set;}}class U:Ó{protected List<ScreenSetupTags>V=new
List<ScreenSetupTags>();static string[]W=new string[Enum.GetValues(typeof(ScreenSetupTags)).Length];public string Ò{get{
return Y.CustomName;}}public long Ñ{get{return Y.EntityId;}}public G Ð{get{if(j<0)return null;else return a[j]as G;}}public R
Ï{get{if(Í<0)return null;else return a[Í]as R;}}public int Õ{get{return ø;}set{ø=ô(value);}}public int Î{get{return ú.ɰ;}
set{ú=ì(value);}}public bool X{get;private set;}IMyTerminalBlock Y{get;}IMyTextSurfaceProvider Z;n[]a;List<đ>e=new List<đ>(
);List<IMyCockpit>f=new List<IMyCockpit>();int g=0;G[]h=new G[2];int j=-1;int Í=-1;bool m=false;bool Ø=false;int ø=0;int
ù=1;ȡ ú=null;bool û=false;static U(){W[0]=" - names of the sensors to activate this LCD, separated by ,";W[1]=
" - names of the cockpits used to activate this LCD, separated by ,";}public U(IMyTerminalBlock Y){this.Y=Y;Z=Y as IMyTextSurfaceProvider;a=new n[Z.SurfaceCount];û=this.Y.CustomName.
Contains("irlock");ü(0,1);j=-1;ú=Ɇ[0];X=false;þ();g=this.Y.CustomData.GetHashCode();}void ü(params int[]ý){foreach(var ã in ý)if
(!V.Contains((ScreenSetupTags)ã))V.Add((ScreenSetupTags)ã);}void þ(){m=Ø=false;Í=j=-1;var ÿ=new StringBuilder();string[]Ā
=Y.CustomData.Trim().Trim('\n').Split('\n');int ā,å=0,Ă=0;bool ă=false;while(Ă<Ā.Length&&!Ā[Ă].Trim().StartsWith("@")){ÿ.
AppendLine(Ā[Ă]);Ă++;}if(Ă<Ā.Length){ă=Ā[Ă].Contains("@H2O2");if(!Ā[Ă].Contains(Ɋ))ÿ.Append(Ā[Ă].Trim()+", "+Ɋ+"\n");else ÿ.Append
(Ā[Ă].Trim()+"\n");Ă++;}else ÿ.Append(Ɋ+"\n");ÿ.Append(Ɍ+"\n");if(Ă<Ā.Length&&Ā[Ă].StartsWith(Ɍ)){Ă++;for(å=0;å<Z.
SurfaceCount;å++){if(Ă<Ā.Length&&!Ā[Ă].StartsWith(ɍ)){if(Ā[Ă].StartsWith(Z.GetSurface(å).DisplayName))ÿ.Append(Ċ(å,Ā[Ă].Split('=')[1
])+"\n");else ÿ.Append(Ą(å)+"\n");}else break;Ă++;}}for(;å<Z.SurfaceCount;å++)ÿ.Append(Ą(å)+"\n");Ă++;ÿ.Append(ɍ+"\n"+Ɏ+
"\n");List<ScreenSetupTags>Ď=V.ToList();string[]č;if(Ă<Ā.Length&&Ā[Ă].StartsWith(Ɏ)){Ă++;while(Ă<Ā.Length&&!Ā[Ă].StartsWith(
ɏ)){if(Ā[Ă].Contains('=')){č=Ā[Ă].Split('=');try{ā=Ď.IndexOf((ScreenSetupTags)Enum.Parse(typeof(ScreenSetupTags),č[0]));}
catch{ā=-1;}if(ā>=0){ÿ.Append(ö(Ď[ā],č[1])+"\n");Ď.RemoveAt(ā);}else ÿ.Append(Ā[Ă]+"\n");}Ă++;}Ă++;}if(Ď.Count>0)foreach(var
ã in Ď)ÿ.Append(ã+â(ã)+"\n");if(!ă){ÿ.AppendLine(ɏ+"\n"+ɐ+"\n--- setup section info ---");foreach(var ã in V)ÿ.AppendLine
(ã+W[(int)ã]);ÿ.AppendLine("--- available screens ---");foreach(string Č in Enum.GetNames(typeof(LayoutTypes)))ÿ.
AppendLine(Č);ÿ.AppendLine(ɑ);}else return;bool ċ=true;while(Ă<Ā.Length){ċ&=!Ā[Ă].StartsWith("[readme");if(ċ)ÿ.Append(Ā[Ă]+"\n");ċ
|=Ā[Ă].StartsWith("[/readme");Ă++;}Y.CustomData=ÿ.ToString().Trim('\n').Trim();}string Ċ(int å,string Ú){string Û=Z.
GetSurface(å).DisplayName+"=";LayoutTypes ĉ;int Ĉ=-1,ć=-1;int Ć=Ú.IndexOf(',');int ą=Ú.IndexOf('>');try{if(Ć>0){if(ą<0)Ĉ=Int32.
Parse(Ú.Trim().Substring(Ć+1));else{ć=Int32.Parse(Ú.Trim().Substring(Ć+1,ą-Ć-1));Ĉ=Int32.Parse(Ú.Trim().Substring(ą+1));}Ú=Ú.
Substring(0,Ć);}else ć=Ĉ=-1;}catch{ć=Ĉ=-1;}try{if(Enum.TryParse(Ú,out ĉ)){a[å]=ä(å,ĉ,ć,Ĉ);if((int)a[å].A==3){Ø=true;Í=å;}if(a[å].
B){m=true;Û+=(LayoutTypes)0;}else{Û+=a[å].A;if(ć>=0&&Ĉ>=0)Û+=", "+ć+" > "+Ĉ;else if(Ĉ>=0)Û+=", "+Ĉ;}}else throw new
System.InvalidCastException();}catch{a[å]=null;Û+=Ú;}return Û;}string Ą(int å){if(å==0&&û)a[å]=ä(å,LayoutTypes.AirlockScreen,-
1,-1);else a[å]=ä(å,LayoutTypes.NONE);return Z.GetSurface(å).DisplayName+"="+a[å].A;}string ö(ScreenSetupTags ã,string Ú)
{string Û=ã+"=";string[]Ü;switch((int)ã){case 0:e.Clear();Ü=Ú.Split(',');try{foreach(string Ý in Ü.Where(Þ=>Þ.Length>0))e
.Add(Ʉ.First(ß=>ß.Ò.Equals(Ý.Trim())));Û+=Ú;}catch{ɖ.ȗ("Sensor for "+Y.CustomName+
" missing\nsensor detection disabled\nPlease check names and recompile!");}break;case 1:f.Clear();Ü=Ú.Split(',');try{foreach(string à in Ü.Where(Þ=>Þ.Length>0))f.Add(Ʌ.First(á=>á.CustomName.
Equals(à.Trim())));Û+=Ú;}catch{ɖ.ȗ("Cockpit for "+Y.CustomName+
" missing\ncockpit detection disabled\nPlease check names and recompile!");}break;}return Û;}string â(ScreenSetupTags ã){string Û="=";switch((int)ã){case 0:e.Clear();break;case 1:f.Clear();
break;}return Û;}n ä(int å,LayoutTypes æ=LayoutTypes.NONE,int ç=-1,int õ=-1){IMyTextSurface ó=Z.GetSurface(å);n ò;switch((int
)æ){case 0:if(!m){j=å;h=ñ(å);ò=h[ø];}else{ɖ.ȗ("a controllable screen allready exist\n"+Y.CustomName+"- Screen: "+j);ò=a[å
];}break;case 1:ò=new Ɔ(ó,ɨ(õ));break;case 2:ò=new ŭ(ó,ɨ(õ),ɨ(ç));break;case 3:if(!Ø){Í=å;ò=new Ɩ(ó,ù);}else{ɖ.ȗ(
"a instructions screen allready exist\n"+Y.CustomName+"- Screen: "+j);ò=new Ǝ(ó);}break;case 4:ò=new ƍ(ó);break;case 5:ò=new Ǝ(ó);break;default:ò=null;break;}
return ò;}G[]ñ(int ð){G[]ï=new G[9];IMyTextSurface Ö=Z.GetSurface(ð);ï[0]=new Ƙ(Ö);ï[1]=new Ɔ(Ö,ú,true);return ï;}int ô(int î)
{ø=î;if(ø<0)ø=1;if(ø>1)ø=0;if(j>=0){int í=a[j].P;;a[j]=h[ø];}return ø;}ȡ ì(int ë){if(Ð is Ɔ){if(ë>ʴ.Γ)ë=0;if(ë<0)ë=ʴ.Γ;(Ð
as Ɔ).Ǯ=ú=ɨ(ë);}return ú;}public void Ì(int Ë){try{(a[Í]as R).S=ù=Ë;}catch{ɖ.ȗ(
"no InstructionScreen found\ncheck ~ BlockName of the command");}}bool ê(){if(e.Count==0&&f.Count==0)return true;try{return e.Any(é=>é.ē)||f.Any(o=>o.IsUnderControl);}catch{ɖ.ȗ(
"Sensor or cockpit not found at "+Y.CustomName+"\nSensor and cockpit detection disabled\nCheck names and recompile!");return true;}}public void É(bool È)
{if(È){for(int ā=0;ā<Z.SurfaceCount;ā++){Z.GetSurface(ā).ContentType=ContentType.NONE;Z.GetSurface(ā).WriteText("");}}
else{for(int ā=0;ā<a.Length;ā++){if(a[ā]!=null&&a[ā].A!=LayoutTypes.NONE){Z.GetSurface(ā).ContentType=ContentType.NONE;Z.
GetSurface(ā).WriteText("");}}}}public List<n>Ê(){û=Y.CustomName.Contains("irlock");if(Y.CustomData.GetHashCode()!=g){þ();g=Y.
CustomData.GetHashCode();}X=a.Any(ß=>ß is Ɩ);return a.ToList();}public void Ç(bool Æ=false){if(Æ||ê())foreach(var ß in a.Where(ß=>
ß!=null))ß.F();}}abstract class ƽ{public IMyTextSurface ó;protected RectangleF ƾ;public bool ƿ{get;private set;}public
bool ǀ{get;private set;}Vector2 ǁ=new Vector2(512,512);Vector2 ǂ;protected float ǃ,Ǆ,ǅ;public string ǆ{get;private set;}
protected float Ǉ;public float ǔ{get;private set;}public float ǒ{get;private set;}public float Ǒ{get;}MySpriteDrawFrame ǐ;List<
MySprite>Ǐ=new List<MySprite>();Color ǎ;MySprite Ǎ;protected Vector2 Ǔ;public ƽ(IMyTextSurface ó,Color Ʋ){this.ó=ó;ǂ=new Vector2
((Math.Max(ó.TextureSize.X,ó.SurfaceSize.X)-Math.Min(ó.TextureSize.X,ó.SurfaceSize.X))/2,(Math.Max(ó.TextureSize.Y,ó.
SurfaceSize.Y)-Math.Min(ó.TextureSize.Y,ó.SurfaceSize.Y))/2);ƾ=new RectangleF(ǂ,new Vector2(Math.Min(ó.TextureSize.X,ó.SurfaceSize.
X),Math.Min(ó.TextureSize.Y,ó.SurfaceSize.Y)));ƿ=ƾ.Height/ƾ.Width<0.65f;ǀ=ƾ.Height<=256;Ǒ=Math.Min(ƾ.Width,ƾ.Height)/50f;
ǃ=ǂ.X+Ǒ;Ǆ=ǂ.X+ƾ.Width-Ǒ;ǅ=ǂ.X+(ƾ.Width/2);float ǌ=ƾ.Width/ǁ.X,ǋ=ƾ.Height/ǁ.Y;ƫ("Debug",Math.Min(ǌ,ǋ));Ǉ=ǔ;Ʊ(Ʋ);Ǌ();}
protected void Ǌ(){ǐ=ó.DrawFrame();Ǐ.Clear();ó.WriteText("");ó.ContentType=ContentType.SCRIPT;ó.Script="";Ǔ=ǂ;ư();}protected void
ǉ()=>ó.ContentType=ContentType.NONE;public void ǈ(){ǐ.Add(Ǎ);Ǐ.ForEach(ß=>ǐ.Add(ß));ǐ.Dispose();}public void ƫ(string ƭ,
float Ƭ){ǆ=ƭ;ǔ=Ƭ;ǔ=Math.Max(ǔ,0.45f);ǒ=ó.MeasureStringInPixels(new StringBuilder("0"),ǆ,ǔ).Y;}public void ƫ(string ƭ)=>ƫ(ƭ,ǔ)
;public void ƫ(float Ƭ)=>ƫ(ǆ,Ƭ);public void Ʈ(float Ư)=>ƫ(ǆ,ǔ*Ư);public void ư()=>ƫ(Ǉ);protected void Ʊ(Color Ʋ){ǎ=Ʋ;Ǎ=
new MySprite(SpriteType.TEXTURE,"SquareSimple",ƾ.Center,ƾ.Size,ǎ);}protected void Ƴ(float ƴ=0,float Ƶ=0)=>Ǔ+=new Vector2(ƴ,
Ƶ);public Vector2 ƶ(Vector2 Į,string ƺ,Color Ʒ,TextAlignment Ƹ){Ǐ.Add(new MySprite(SpriteType.TEXT,ƺ,Į,null,Ʒ,ǆ,Ƹ,ǔ));
return ó.MeasureStringInPixels(new StringBuilder(ƺ),ǆ,ǔ);}public void ƹ(string ƺ,Color Ʒ,TextAlignment Ƹ)=>Ǔ.Y+=ƶ(Ǔ,ƺ,Ʒ,Ƹ).Y;
public Vector2 ƻ(Vector2 Į,float ƴ,float Ƶ,Color Ʒ){Ǐ.Add(new MySprite(SpriteType.TEXTURE,"SquareSimple",Į,new Vector2(ƴ,Ƶ),Ʒ,
null,TextAlignment.CENTER));return new Vector2(ƴ,Ƶ);}public Vector2 Ƽ(Vector2 Į,float ƴ,float Ƶ,Color Ʒ)=>ƻ(new Vector2(Į.X+
(ƴ/2),Į.Y+(Ƶ/2)),ƴ,Ƶ,Ʒ);protected Vector2 Ǖ(Vector2 Į,float ƴ,float Ƶ,Color Ʒ,bool ǧ,params float[]ł){if(ǧ)Ǐ.Add(new
MySprite(SpriteType.TEXTURE,"SquareSimple",Į,new Vector2(ƴ,Ƶ),Ʒ,null,TextAlignment.CENTER));else{Vector2 Ä=new Vector2(Į.X-(ƴ/2)
,Į.Y-(Ƶ/2));Ǩ(Ä,ƴ,Ƶ,Ʒ,ǧ,ł);}return new Vector2(ƴ,Ƶ);}public void Ǩ(Vector2 Į,float ƴ,float Ƶ,Color Ʒ,bool ǧ,params float[
]ł){if(ǧ)Ǖ(new Vector2(Į.X+(ƴ/2),Į.Y+(Ƶ/2)),ƴ,Ƶ,Ʒ,ǧ);else{Vector2 Ŀ=new Vector2(Į.X+ƴ,Į.Y+Ƶ);Ƽ(Į,ł[0],Ƶ,Ʒ);Ƽ(Į,ƴ,ł[0],Ʒ);
Ƽ(Į+new Vector2(ƴ-ł[0],0),ł[0],Ƶ,Ʒ);Ƽ(Į+new Vector2(0,Ƶ-ł[0]),ƴ,ł[0],Ʒ);}}public void ǩ(Vector2 Į,float Å,string ǳ,Color
Ʒ)=>Ǐ.Add(new MySprite(SpriteType.TEXTURE,ǳ,Į,new Vector2(Å),Ʒ,null,TextAlignment.CENTER));protected void Ǵ(Vector2 Į,
float Å,string ǳ,Color Ʒ)=>ǩ(new Vector2(Į.X+(Å/2),Į.Y+(Å/2)),Å,ǳ,Ʒ);public void ǲ(MySprite ǰ)=>Ǐ.Add(ǰ);}abstract class ǯ:ƽ,
n,G{public LayoutTypes A{get;}public bool B{get;}public ȡ Ǯ{get;set;}protected RectangleF ǭ,Ǭ,ǫ,Ǫ,Ǳ;protected bool Ǧ=
false;protected Vector2 ǟ;protected Vector2 ǖ;protected int Ā=0,Ǘ=0;protected string ǘ;protected string Ǚ=Ƚ(" Status");const
float ǚ=1.15f;public int P{get;private set;}protected List<ʤ>Ǜ=new List<ʤ>();public ˋ H{get{return Ų();}}protected ˋ Ô;public
ǯ(IMyTextSurface ó,int ǜ,int Ƅ,string ǘ,bool Ű=false):base(ó,backgroundColor){if(ǜ>=maxCS)P=-1;else P=ǜ;A=(LayoutTypes)Ƅ;
B=Ű;this.ǘ=ǘ;if(B)Ǚ=Ƚ(" Control");Ǯ=Ɇ[-1];ǟ=ó.MeasureStringInPixels(new StringBuilder("000,000.0 m³"),ǆ,ǔ);ǭ.Position=new
Vector2(ǃ,ƾ.Y);ǭ.Size=new Vector2(ƾ.Width-(2*Ǒ),0);}protected void ǝ(string Ǟ){if(P>=0)Ǟ=Ǟ+", CS: "+(P+1);Ʈ(ǚ);Ǔ=new Vector2(ǅ,
ƾ.Y);ƹ(Ǟ,mainColor,TextAlignment.CENTER);ư();Ǔ.Y+=ǥ();Ǔ.X=ǃ;ǭ.Height=Ǔ.Y-ƾ.Y;}protected float ǥ(){ƻ(new Vector2(ƾ.Center.
X,Ǔ.Y+(Ǒ/2)),ƾ.Width-(Ǒ*2),Ǒ/2,mainColor);return Ǒ;}public Vector2 ǣ(Vector2 Ä,float Å,bool Ǣ){float ł=Å*0.1f;Ǩ(Ä,Å,Å,
mainColor,false,ł);Color ǡ=colorOff;if(Ǣ)Ǩ(Ä+new Vector2(2*ł),Å*0.6f,Å*0.6f,colorOn,true);else Ǵ(Ä+new Vector2(2*ł),Å*0.6f,
"Cross",colorOff);return new Vector2(Å);}protected void Ǥ(){Ǭ=ǫ=Ǫ=Ǳ=new RectangleF(0,0,0,0);ǫ.Size=new Vector2(ƾ.Width-(2*Ǒ),2*
ǒ);if(B){ǫ.Height=3*ǒ;if(!ƿ&&!ǀ)ǫ.Height*=0.65f;}ǫ.Position=new Vector2(ǃ,ƾ.Bottom-ǫ.Height-Ǒ);if(this is ŭ){Ǳ.Size=new
Vector2(Math.Min((ƾ.Width/2)-(1.5f*Ǒ),ǫ.Y-ǭ.Bottom-(2*Ǒ)));Ǫ.Size=new Vector2(ƾ.Width-(3*Ǒ)-Ǳ.Width,ǫ.Y-ǭ.Bottom-(2*Ǒ));Ǳ.
Position=new Vector2(ǃ,ǭ.Bottom+((ǫ.Y-ǭ.Bottom-Ǳ.Height)/2));Ǫ.Position=new Vector2(Ǳ.Right+Ǒ,ǭ.Bottom+Ǒ);}else{Ǳ=new RectangleF
(0,0,0,0);Ǫ=new RectangleF(ǃ,ǭ.Bottom+Ǒ,ƾ.Width-(2*Ǒ),ǫ.Y-ǭ.Bottom-(2*Ǒ));}Ǭ=new RectangleF(Ǫ.Position+new Vector2(2*Ǒ),
new Vector2(Ǫ.Width-(4*Ǒ),Ǫ.Height-(4*Ǒ)));Ǧ=true;}protected void Ǡ(){Ǭ=ǫ=Ǫ=Ǳ=new RectangleF(0,0,0,0);ǫ.Size=new Vector2(ƾ.
Width-(2*Ǒ),ǒ*2);if(!ƿ&&!ǀ)ǫ.Height*=0.65f;ǫ.Position=new Vector2(ǃ,ƾ.Bottom-ǫ.Height-Ǒ);Ǭ.Size=Ǳ.Size=new Vector2((ƾ.Width/2
)-(1.5f*Ǒ),ǫ.Y-ǭ.Bottom-(2*Ǒ));Ǭ.Position=new Vector2(ǃ,ǭ.Bottom+Ǒ);Ǳ.Position=new Vector2(Ǆ-Ǳ.Width,ǭ.Bottom+Ǒ);Ǧ=true;}
virtual public void I(int J){if(Ǜ.Count==0){Ô=null;return;}int Ơ=0,ƣ;if(Ô!=null&&Ô is ʤ)Ơ=Ǜ.IndexOf(Ô as ʤ);if(Ơ<0){Ô=Ǜ.First()
;return;}else{ƣ=Ơ+J;while(ƣ<0||ƣ>=Ǜ.Count){if(ƣ<0)ƣ+=Ǜ.Count;else ƣ-=Ǜ.Count;}}Ô=Ǜ[ƣ];}public void K()=>Ǯ.Ȧ=!Ǯ.Ȧ;public
void L()=>H.ˊ=!H.ˊ;public void M()=>H.ˉ=!H.ˉ;public void N()=>H.ˈ=!H.ˈ;public void O(int Q,int k){int ģ=H.P+Q;if(ģ>(k-1))ģ=0
;if(ģ<0)ģ=k-1;(H as ʤ).P=ģ;}public virtual void C<D>(List<D>E)where D:ʤ{if(E.Count>0){if(P>=0)Ǜ=E.Where(ť=>ť.P==P).Select
(ť=>ť as ʤ).ToList();else Ǜ=E.Select(ť=>ť as ʤ).ToList();}else{Ǜ.Clear();Ô=null;}if(Ǜ.Count>0)if(!Ǜ.Contains(Ô))Ô=Ǜ.First
();}ˋ Ų(){if(B&&Ô!=null)return Ô;else return null;}protected void ų(List<ʤ>Ŵ,float ŵ){if(!Ǧ){ƹ(Ƚ(
"symbol area calculation failed"),mainColor,TextAlignment.LEFT);return;}if(this is Ɔ&&Ǯ.ɰ<0){ƶ(Ǭ.Center-new Vector2(0,ǒ),Ƚ(
"specify the room to show\nadd \", #\" to RoomScreen\n\ni.e.: ScreenArea=RoomScreen, 3\nshowing room number 3"),warningColor,TextAlignment.CENTER);return;}if(Ŵ.Count==0){ƶ(Ǭ.Center-new Vector2(0,ǒ),Ƚ("no blocks\navailable"),
warningColor,TextAlignment.CENTER);return;}float Ŷ=Ǒ*1.5f;Ā=0;Ǘ=1;while(Ā*Ǘ<Ŵ.Count){Ā++;ǖ.Y=(Ǭ.Height-Ŷ-(Ā*Ŷ))/Ā;ǖ.X=ǖ.Y*ŵ;Ǘ=(int)
Math.Floor((Ǭ.Width-Ŷ)/(ǖ.X+Ŷ));}Ā=(int)Math.Ceiling(Ŵ.Count/(double)Ǘ);Ǘ=(int)Math.Ceiling(Ŵ.Count/(double)Ā);float ŷ=(Ǭ.
Width-(ǖ.X*Ǘ))/(Ǘ+1);float Ÿ=(Ǭ.Height-(ǖ.Y*Ā))/(Ā+1);Ǔ=Ǭ.Position+new Vector2(ŷ,Ÿ);int á=0;foreach(var ť in Ŵ){ť.ǩ(Ǔ,ǖ,this,
H);Ǔ.X+=ǖ.X+ŷ;á++;if(á>=Ǘ){Ǔ.X=Ǭ.X+ŷ;Ǔ.Y+=ǖ.Y+Ÿ;á=0;}}Ǔ=new Vector2(ǃ,Ǭ.Bottom);}protected void Ź<D>(string ź,List<D>Ż)
where D:ʤ{Ǔ.X=ǅ;ƹ(ź+" found: "+Ż.Count,mainColor,TextAlignment.CENTER);ƹ(ź+" shown: "+Ǜ.Where(ż=>ż is D).ToList().Count,
mainColor,TextAlignment.CENTER);}protected void Ź(){Ʈ(0.7f);Ǔ.X=ǃ;ƹ(ɖ.Ț(),warningColor,TextAlignment.LEFT);ƹ(ɖ.ȥ(),mainColor,
TextAlignment.LEFT);ư();}abstract public void F();}class Ɔ:ǯ,n,G{public static bool ƅ{protected get;set;}public Ɔ(IMyTextSurface ó,ȡ
Đ,bool Ű=false,int Ƅ=1):base(ó,-1,Ƅ,Ƚ("Rooms"),Ű){if(Đ.ɰ>=0)Ǯ=Đ;ƅ=!Æ;}public void C(){Ǜ=Ǯ.ʻ;if(Ǜ.Any()){if(!Ǜ.Contains(Ô)
)Ô=Ǜ.First();}else{Ǜ.Clear();Ô=null;}}float ƃ(float Ɓ,IEnumerable<List<Ě>>ƀ,IEnumerable<List<Ě>>ſ,out float Ū,out float ū
){int ž=1,Ž=1;foreach(var Ś in ƀ)ž+=Ś.Count+1;foreach(var Ś in ſ)Ž+=Ś.Count+1;float Å=Ɓ/Math.Max(ž,Ž);Ū=(Ɓ-(ž*Å))/2;ū=(Ɓ-
(Ž*Å))/2;return Å;}void ś(IEnumerable<List<Ě>>Ŝ,float ŝ,float Ş){foreach(var ş in Ŝ){bool Š=false,š=false;Color ĩ=colorOn
;Vector2 œ=Ǔ;ş.ForEach(Ţ=>{Š|=Ţ.ʥ;š|=!Ţ.ʥ;œ.X+=Ţ.ǩ(œ,new Vector2(ŝ,Ş),this,H).X;});if(š){ĩ=warningColor;if(!Š)ĩ=colorOff;
}ƻ(Ǔ-new Vector2(Ş/2,-Ş/2),Ş,Ş*3,ĩ);Ƽ(œ,ŝ,Ş,mainColor);ƻ(œ+new Vector2(Ş/2),Ş,Ş*3,ĩ);Ǔ.X=œ.X+ŝ;}}void ţ(bool Ť)=>Ŧ(Ǯ.ʹ.
Values.ToList(),Ǯ.ʺ.Values.ToList(),Ǜ.Where(ť=>ť is Ń).ToList(),Ť);protected void Ŧ(List<List<Ě>>ŧ,List<List<Ě>>ű,List<ʤ>Ũ,
bool Ť){float ł=Ǒ,ũ,Ū,ū;ũ=ƃ(Ǫ.Width-(2*ł),ŧ,ű,out Ū,out ū);Ǩ(Ǫ.Position+new Vector2(ł),Ǫ.Width-(ł*2),(Ǫ.Height-(ł*2))*Ǯ.ʽ,
o2Color,true);if(Ť){float Ŭ=Ǭ.Height;Ǭ.Height-=ǒ+Ǒ;ų(Ũ,Ń.w);Ǔ.X=Ǭ.Center.X;Ǔ.Y+=Ǒ/2;ƹ(Ǯ.Ƞ(),mainColor,TextAlignment.CENTER);Ǭ.
Height=Ŭ;}else ų(Ũ,Ń.w);if(Ǯ.ɰ>0){Ǔ=new Vector2(Ǫ.X+ł,Ǫ.Bottom-ł);Ǔ.X+=Ƽ(Ǔ,ū+ũ,ł,mainColor).X;ś(ű,ũ,ł);Ƽ(Ǔ,Ǫ.Right-Ǔ.X,ł,
mainColor);Ƽ(Ǫ.Position,ł,Ǫ.Height,mainColor);Ƽ(Ǫ.Position+new Vector2(Ǫ.Width,0),-ł,Ǫ.Height,mainColor);Ǔ=new Vector2(Ǫ.X+ł,Ǫ.Y)
;}else{Ǔ=new Vector2(Ǫ.X+ł,Ǫ.Bottom-ł);}Ǔ.X+=Ƽ(Ǔ,Ū+ũ,ł,mainColor).X;ś(ŧ,ũ,ł);Ƽ(Ǔ,Ǫ.Right-Ǔ.X,ł,mainColor);}public
override void F(){Ǌ();if(!ƅ){Ź("vents",Ɂ);Ź("doors",Program.ş);Ź();}else{ǝ(Ǯ.ʑ+Ǚ);if(!Ǧ)Ǥ();ţ(B);if(!B){Ǔ=new Vector2(ǫ.Center.X
,ǫ.Position.Y);ƹ(Ǯ.Ȟ(ǀ),mainColor,TextAlignment.CENTER);}else{if(H!=null)H.ˇ(ǫ,this,Ǯ);}}ǈ();}}class ŭ:Ɔ,n{public ȡ Ů{get
;set;}public ŭ(IMyTextSurface ó,ȡ ĕ,ȡ ů=null,bool Ű=false):base(ó,ĕ,Ű,2){ǘ=Ƚ("Airlocks");if(ů==null)Ů=Ɇ[-1];else Ů=ů;}
void Ƃ(){if(Ů.ɰ<0)return;List<List<Ě>>ŧ=new List<List<Ě>>();List<List<Ě>>ű=new List<List<Ě>>();List<ʤ>Ũ=new List<ʤ>();if(!Ǯ.
ˀ(Ů))Ǵ(Ǳ.Position,Ǳ.Height,"No Entry",Color.White);else{if(Ǯ.ē||Ǯ.ʼ)Ǵ(Ǳ.Position,Ǳ.Height,"Danger",warningColor);else Ǵ(Ǳ
.Position,Ǳ.Height,"Arrow",Color.Green);}if(Ǯ.ɰ==0){foreach(var Ś in Ů.ʹ.Values)if(Ś.Any(ě=>ě.ʭ))ŧ.Add(Ś);}else{foreach(
var Ś in Ǯ.ʺ.Values)if(!Ś.Any(ě=>ě.ʪ(Ů)))ŧ.Add(Ś);else ű.Add(Ś);foreach(var Ś in Ǯ.ʹ.Values)if(!Ś.Any(ě=>ě.ʪ(Ů)))ŧ.Add(Ś);
else ű.Add(Ś);Ũ=Ǜ.Where(ť=>ť is Ń).ToList();}Ŧ(ŧ,ű,Ũ,false);}public override void F(){Ǌ();if(!ƅ){Ź("vents",Ɂ);Ź("doors",
Program.ş);Ź();}else{ǝ(Ǯ.ʑ);if(!Ǧ)Ǥ();if(Ǯ.ɰ>=0&&Ů.ɰ>=0){Ƃ();Ǔ=new Vector2(ǫ.Center.X,ǫ.Position.Y);ƹ(Ǯ.Ȟ(ǀ),mainColor,
TextAlignment.CENTER);}else{Ǔ=Ǭ.Position;Ǔ.X=ǅ;ƹ(Ƚ("Please enter room number of the\nroom this display is located and\nthe airlock you want to monitor\nto the custom data field\n\ni.e.: screen=RoomScreen, 1 > 2\nplaced room: 1, shown room: 2"
),warningColor,TextAlignment.CENTER);}}ǈ();}}class Ƙ:ǯ,n,G{public static bool ƅ{private get;set;}Vector2 ƙ;static int ƚ=0
;static int ƛ=0;static int Ɯ=0;static int Ɲ=0;static List<ˋ>ƞ=new List<ˋ>();static Action<bool>Ɵ=(Ƨ)=>{ɯ(Ƨ);};static Func
<bool>Ʃ=()=>{return ɉ;};static Action<bool>ƨ=(Ƨ)=>{ɮ(Ƨ);};static Func<bool>Ʀ=()=>{return leveling&&ɉ;};static Func<float,
float>ƥ=(Ƥ)=>{if(Ƥ>0||maxVentCS-minVentCS>0.15)maxVentCS+=Ƥ;maxVentCS=Math.Min(maxVentCS,1);maxVentCS=Math.Max(maxVentCS,0);
return maxVentCS;};static Func<float,float>ƪ=(Ƥ)=>{if(Ƥ<0||maxVentCS-minVentCS>0.15)minVentCS+=Ƥ;minVentCS=Math.Min(minVentCS,
1);minVentCS=Math.Max(minVentCS,0);return minVentCS;};static Ƙ(){ƅ=!Æ;ƞ.Add(new ʩ(Ɵ,Ʃ,Ƚ("Room Control"),Ƚ(
"switch auto. room pressure and door control")));ƞ.Add(new ʩ(ƨ,Ʀ,Ƚ("level oxygen tanks"),Ƚ("switch automatic airlock oxygen leveling")));ƞ.Add(new Η(ƥ,maxVentCS,Ƚ(
"change upper oxygen limit for leveling")));ƞ.Add(new Η(ƪ,minVentCS,Ƚ("change lower oxygen limit for leveling")));}public static void Ƣ(){ƛ=ƚ=Ɯ=Ɲ=0;foreach(var
ơ in Ɇ.Values){ƛ++;if(ơ.Ȧ)ƚ++;if(ơ.Ǻ||ơ.Ȝ){Ɲ++;if(ơ.ʶ)Ɯ++;}}if(ƞ[0].ˊ!=ɉ)ƞ[0].ˊ=ɉ;if(ƞ[1].ˊ!=leveling&&ɉ)ƞ[1].ˊ=leveling
&&ɉ;}public Ƙ(IMyTextSurface ó):base(ó,-1,0,Ƚ("Room Program"),true){if(Ô==null)Ô=ƞ.First();}override public void I(int J){
if(ƞ.Count==0){Ô=null;return;}int Ơ=0,ƣ;if(Ô!=null)Ơ=ƞ.IndexOf(Ô);if(Ơ<0){Ô=ƞ.First();return;}else{ƣ=Ơ+J;while(ƣ<0||ƣ>=ƞ.
Count){if(ƣ<0)ƣ+=ƞ.Count;else ƣ-=ƞ.Count;}}Ô=ƞ[ƣ];}void Ɨ(Vector2 Ä,Vector2 Å,float Ƈ,float ƈ){Ä.X+=Å.X*0.2f;Å.X*=0.6f;float
ł=Math.Min(Å.X,Å.Y)*0.05f;Vector2 Į=Ä,Ɖ=Ä+new Vector2(Å.X,0);Vector2 Ɗ=Ä+new Vector2(0,Å.Y),Ŀ=Ɖ+new Vector2(0,Å.Y);Į=Ä;Į.
Y+=Å.Y*0.2f;Ǩ(Į,Å.X,Å.Y*0.8f,colorOn,false,ł);Į.X+=ł;Į.Y+=ł;Ǩ(Į,Å.X-(2*ł),(Å.Y*0.8f)-(2*ł),o2Color,false,ł);Į=Ä;Į.X+=Å.X*
0.35f;Ǩ(Į+new Vector2(ł,0),(Å.X*0.3f)-(2*ł),(Å.Y*0.2f)+(2*ł),o2Color,true);Ƽ(Į,ł,Å.Y*0.2f,colorOn);Į=Ɖ;Į.X-=Å.X*0.35f;Ƽ(Į,-ł,
Å.Y*0.2f,colorOn);Į=Ɗ;Į.X+=2*ł;Į.Y-=2*ł;Ǩ(Į,Å.X-(4*ł),((-Å.Y*0.8f)+(4*ł))*0.5f,o2Color,true);if(Program.leveling){Ƽ(Į-new
Vector2(ł,((Å.Y*0.8f)-(4*ł))*Ƈ),Å.X-(2*ł),Ǒ/3,mainColor);Ƽ(Į-new Vector2(ł,((Å.Y*0.8f)-(4*ł))*ƈ),Å.X-(2*ł),Ǒ/3,mainColor);}}
public override void F(){Ǌ();if(!ƅ){Ź("O2 tanks",ɀ);Ź();}else{ǝ(ǘ+Ǚ);if(!Ǧ){Ǡ();ƙ.X=(Ǭ.Width/2)-Ǒ;ƙ.Y=ƙ.X/u.w;}Ǔ=Ǭ.Position+
new Vector2((Ǒ/2)+(ƙ.X/2),(Ǭ.Height-ƙ.Y)/2);Ɨ(Ǔ,ƙ,minVentCS,maxVentCS);Ǔ.X+=ƙ.X/2;Ǔ.Y-=ǒ*2;ƞ[2].ǩ(Ǔ,this);Ǔ.Y+=(3*ǒ)+ƙ.Y;ƞ[
3].ǩ(Ǔ,this);Ǔ=Ǳ.Position+new Vector2(Ǒ);if(ƿ)Ʈ(1.2f);else Ʈ(0.8f);Ʈ(1.3f);Ǔ.Y+=ƞ[0].ǩ(Ǔ,this).Y+ǒ;Ʈ(1/1.3f);Vector2 Ƌ=Ǔ+
new Vector2(0,Ǒ);Vector2 ƌ=Ƌ+new Vector2(0,ǒ*3);if(ɉ){ƶ(Ƌ,"auto. controlled rooms:\n"+ƚ+" out of "+ƛ,mainColor,
TextAlignment.LEFT);if(leveling)ƶ(ƌ,"auto. leveled rooms:\n"+Ɯ+" out of "+Ɲ,mainColor,TextAlignment.LEFT);}Ǔ.Y=Ǳ.Bottom-ǒ-Ǒ;ƞ[1].ǩ(Ǔ,
this);ư();if(H!=null)Ô.ˇ(ǫ,this);}ǈ();}}class ƍ:ǯ,n{public static bool ƅ{private get;set;}static ƍ(){ƅ=!Æ;}public ƍ(
IMyTextSurface ó,bool Ű=false):base(ó,-1,4,Ƚ("Room Control Program"),Ű){ƫ("Monospace",ǔ);}public override void F(){Ǌ();ǝ(ǘ);ƹ(ɖ.Ț(),
warningColor,TextAlignment.LEFT);ƹ(ɖ.ȣ(),mainColor,TextAlignment.LEFT);if(!ƅ)ƹ(ɖ.ȥ(),mainColor,TextAlignment.LEFT);ǈ();ư();ó.
WriteText(ɖ.Ȣ(),false);}}class Ɩ:ǯ,n,R{public static bool ƅ{private get;set;}public int S{get{return ƕ+1;}set{ƕ=value-1;}}int ƕ=0
;RectangleF Ɣ;float Ɠ;static Ɩ(){ƅ=!Æ;}public Ɩ(IMyTextSurface ó,int ƒ):base(ó,-1,3,Ƚ("Room Program Manual"),false){Ɣ=ƾ;Ɠ
=1;ƕ=Math.Min(Math.Max(ƒ-1,0),8);}void Ƒ(){Ɣ.Position=ǭ.Position+new Vector2(0,ǭ.Height+(Ǒ/2));Ɣ.Height=ƾ.Height-ǭ.Height
-(1.5f*Ǒ);Ɣ.Width=ǭ.Width;float Ɛ=(Ɣ.Height-(Ǒ*1.5f))/10;Ɠ=Ɛ/ǒ;if(ǔ*Ɠ>1.2f)Ɠ=1.2f/ǔ;Ǧ=true;}public override void F(){Ǌ();
ǝ(ǘ);if(!Ǧ)Ƒ();if(!ƅ)ƹ(ɖ.ȥ(),mainColor,TextAlignment.LEFT);else{Ʈ(Ɠ);Ǔ=new Vector2(Ɣ.Center.X,Ǔ.Y);ƹ("Current Tab: "+(ƕ+1
),mainColor,TextAlignment.CENTER);ƹ(instructions[ƕ,0],mainColor,TextAlignment.CENTER);Ǔ.X=Ɣ.X+ǒ;Ǔ.Y+=(Ɣ.Bottom-Ǔ.Y-(8*ǒ))
/2;for(int Ə=1;Ə<9;Ə++){if(instructions[ƕ,Ə].Length>1){ƶ(Ǔ,(Ə+1)+":",mainColor,TextAlignment.RIGHT);Ǔ.Y+=ƶ(new Vector2(Ǔ.
X+(ǒ*0.5f),Ǔ.Y),instructions[ƕ,Ə],mainColor,TextAlignment.LEFT).Y;}else Ǔ.Y+=ǒ;}}ǈ();}}class Ǝ:ǯ,n{public Ǝ(
IMyTextSurface ó,int ǜ=-1,bool Ű=false):base(ó,-1,5,"",false){ǉ();}public override void F(){return;}}