/*
 *     ____        _     _______            
 *    / __ \____ _(_)___/ / __(_)_______   
 *   / /_/ / __ `/ / __  / /_/ / ___/ _ \  
 *  / _, _/ /_/ / / /_/ / __/ / /  /  __/  
 * /_/ |_|\__,_/_/\__,_/_/ /_/_/   \___/    
 * 																	
 *                                                                        
 * //Station Refueler - v1.8\\
 * 
 * Firstly, thank you for using our script please rate and like if you find it useful. Also report any bugs you find on the workshop page. 
 * This Script is meant to run on a station to manage the hydrogen tanks to fill one at a time from the H2 generators. No timer block is needed for this script. 
 * Please read the options and adjust the settings of what you want and need the naming of groups and tag can be customized or stay with the default names 
 * for groups and tag accordingly. The Auto undock features i put it with IIM(Isys Inventory Manager) in mind since it can pull ore and has 
 * Special containers that fills up when amount it set(see Station Refueler configureable timer)
 * 
 * !!NOTICE READ: Remember tho if AutoUndock(false) is not used youl have to manually set your ship tanks stockpile to off 
 *   and the same with ship batteries(if RechargeBatteries is turned on) before you undock. Or it will fall if your using this script on a planet
 * 
 * At the top of the script there is instruction on each line for what each configurable option does. Setting up group toggle on options like Recharge, RefillOxygen, AutoUndock ships, 
 * Auto turn on ship thrusters and more. 
 * 
 * Ive added additional arguments for alot of the on/off toggles. Youl find the arguments in the config. 
 * Remember to set the values as you need. Like IceAmount, Station(Large, Small) Reactor amount, Ship(Large, Small)Reactor Amount
 * MaxRecharge Powerfor ships, MinimumStationPowerLevel and so on.
 * 
 * // === Features as of Q1 2024 === \\
 * * Auto refueling of station hydrogen tanks(one by one) When no ship is docked
 * * Displays Local Time
 * * ETC refueling timer
 * * Display Station Power Levels(batteries only as power is pulled from them first)
 * * Disply Hydrogen tanks either all tanks or queued tanks(not both, warning will be given if both are enabled)
 * * Display Docked Ships. 
 * * This script can handle multiple ships docking as the transfer of fuel, power and resources are
 *   entity based
 * * Argument "recheck" to update block groups if new blocks are added to any of the groups
 * * Auto hydrogen refueling of ship when docked (Now has a bool on/off if you dont want any docked ship to be auto refueled)
 * * Bool Auto Undock has two additional conditions besides refueling and recharging ship batteries
 * * The included conditions if its a miner and it has ore in it. It will not auto undock as long as the miner has ore in it
 * * The second condition is a auto undock timer default is 2 min. These where put in with IIM(Isys Inventory Manager) in mind
 * * Since IIM auto pulls ore and can refill Special Containers(Hence the auto undock timer)
 * * Bool Auto turn on thrusters(nice for planet use)
 * * Reactor and H2 Ice Handling(bool on/off), set amount for h2gen ice, large and small station and ship reactor refill.
 * * Displays warning on lcd if it cant find ice or reactor fuel in any container on the station
 * * Ship batteries recharge(bool on/off) + MaxRecharge and Station Power Level cutoff.
 * 
 * 
 * // === To Do 2024 === \\
 * * Rework parts of the script, optimize Q2,Q3
 * * Redo LCD output as an interactive menu with categories and settings for realtime config changes with hotkey Q2,Q3
 *   argument controls. This will in essence remove the need to edit script configs and you could then do it live while in a
 *   seat.
 * * Debug lcd??? Q2,Q3
 * * Will debate more threw Q3, Q4
 * 
 * // === Current Features === \\
 * * Auto refueling of station hydrogen tanks(one by one) When no ship is docked
 * * Displays Local Time
 * * ETC refueling timer
 * * Display Station Power Levels(batteries only as power is pulled from them first)
 * * Disply Hydrogen tanks either all tanks or queued tanks(not both, warning will be given if both are enabled)
 * * Display Docked Ships. 
 * * This script can handle multiple ships docking as the transfer of fuel, power and resources are
 *   entity based
 * * Argument "recheck" to update block groups if new blocks are added to any of the groups
 * * Auto hydrogen refueling of ship when docked (Now has a bool on/off if you dont want any docked ship to be auto refueled)
 * * Bool Auto Undock has two additional conditions besides refueling and recharging ship batteries
 * * The included conditions if its a miner and it has ore in it. It will not auto undock as long as the miner has ore in it
 * * The second condition is a auto undock timer default is 2 min. These where put in with IIM(Isys Inventory Manager) in mind
 * * Since IIM auto pulls ore and can refill Special Containers(Hence the auto undock timer)
 * * Bool Auto turn on thrusters(nice for planet use)
 * * Reactor and H2 Ice Handling(bool on/off), set amount for h2gen ice, large and small station and ship reactor refill.
 * * Displays warning on lcd if it cant find ice or reactor fuel in any container on the station
 * * Ship batteries recharge(bool on/off) + MaxRecharge and Station Power Level cutoff.
 * 
 * //TotalTanks            - This will display total tank % of all tanks at the above the below options.
 * //StationPowerLevel     - This will display the total power of the stations batteries(since power is pulled from batteries first)
 * //ListAllTanks          - This will show all tanks You can set them as permanent true
 * //ListQueuedTanks       - This will show tanks to be filled You can set them as permanent true 
 * //ListDockedShips       - This will list the docked ships on the named connectors if docking list is on.
 * You should only toggle on ListAllTanks or ListQueuedTanks and not both at the same time.
 * 
 * Lastly if some of the functions arent working properly then report back on the workshop page.
 * Please avoid reporting errors because of user error.
 * Errors mean: Not filling h2gen with ice when enabled, not filling correct ice amount, not filling reactors with correct amount,
 * not recharging batteries, not stopping recharge when reaching minimum station power(RechargeBatteris:ON), not displaying things on the lcd when set to show,
 * no warnings displayed on lcd when out of ice or reactor fuel, not auto undocking when enabled, when station reactor filling or station h2gen 
 * fill was enabled but then disabled neither the station reactors or station h2gens had UseConveyor set back to On.
 */


// === H2 Tank Group === \\

// For the hydrogen tanks on your grid you want to have managed you can rename the group name H2Tanks.
private const string TANK_GROUP_NAME = "H2Tanks"; // Default H2Tanks

// === H2 Generator Group === \\

// For the generators on your grid you want to have managed you can rename group name H2Generators.
private const string GENERATOR_GROUP_NAME = "H2Generators"; // Default H2Generators
private const string IceTag = "[ICE]"; // Add the tag to containers you are storing ice in.
private bool HandleH2Generators = true; // true/false || Enable/Disable with true or false to handle h2 generators
private int IceAmount = 5; // Amount of ice to send to each h2 generator.

// === Connector Group === \\

//For the connectors on your grid you want to have managed with the docking feature you can rename FuelConnectors.
private const string CONNECTOR_GROUP_NAME = "FuelConnectors"; // Default FuelConnectors


// === Reactor Handeling === \\

private bool ReactorHandling = false; // true/false || Enable/Disable with true or false for Reactor handling on the Station
private bool ReactorHandlingShip = false; // true/false || Enable/Disable with true or false for Reactor handling for docking ships
private string ReactorFuelSubtypeId = "Uranium"; // Default to Uranium but can input custom reactor fuel type. Use custom fuel subtypeId(case sensitive)
private int LargeStationReactorFuelAmount = 100; // Station Only Large Reactors
private int SmallStationReactorFuelAmount = 5; // Station Only Small Reactors
private int LargeShipReactorFuelAmount = 100; // Ship Only Large Reactors
private int SmallShipReactorFuelAmount = 10; // Ship Only Small Reactors

// === LCD Status === \\

//For LCD status you can add the tag "[STR]" to an lcd. You can also change the tag if you want to.
private const string LCD_PANEL_TAG = "[STR]";

// === LCD View Toggle === \\

private static  bool TOTAL_HYDROGEN = true; // true/false 11 Display total hydrogen % of all tanks displayed at the top of lcd
private static  bool LIST_QUEUED_TANKS = true; // true/false || List queued tanks will only display the tanks queued for refilling
private static  bool LIST_ALL_TANKS = false; // true/false || List all tanks in the group
private static  bool DISPLAY_STATION_POWER = true; // true/false || Display Station Power Level(Batteries)
private static  bool LIST_DOCKED_SHIPS = false; // true/false || List Docked Ships

// === Auto Refuel Ship === \\

private bool AutoRefuelShip = true; // true/false || Set true to auto refuel ship with hydrogen.

// === Auto Undock === \\

private bool AutoUndock = true; // true/false || Set true to allow auto undocking ship from station connector

// === Auto Undock Wait Timer === \\

private int AutoUndockWaitingPeriod = 20; // Seconds // This is used for various ships docking to your station and can be changed to give time to do stuff(auto filling of specific containers like if you use Special with the script called Isys Inventory)

// Ships that contain ore(Miners in this case) is excluded from the AutoUndockWaitingPeriod and will only auto undock once its cargo's are emtpy

// === Undock Thrusters === \\

private bool UndockThrust = true; // Set to true to turn on ship thrusters before undocking. Useful if planetary based Station.

// === Recharge Batteries === \\

private bool RechargeBatteries = true; // true(on) false(off) Recharges docked ships batteries to MaxRecharge.
private double MaxRecharge = 3.0f; // in MW
private const double MinimumStationPowerLevel = 10.0f; // A threshold to stop power transfer if Station power levels drop to low. Total power in MW

/* // Useable Arguments:
 * // Notifiction: Drag programmable block down to your hotbar and use Run for each argument
 * // Various Argument: (do not include "")
 * // "refuelship" Toggles on and off AutoRefuelShip
 * // "autoundock" Toggles on and off AutoUndock
 * // "rechargeship" Toggles on and off Auto Recharging of Ship
 * // "autothrust" Toggles on and off Auto Thrust if AutoUndock is Enabled
 * // "handleh2" Toggles on and off H2 Generator handling
 * // "streactor" Toggles on and off station reactor handling
 * // "shipreactor" Toggles on and off ship reactor handling
 * // "totalhydrogen" Toggles on and off Display total hydrogen % on lcd
 * // "alltanks" Toggles on and off Display all tanks on lcd
 * // "queuedtanks" Toggles on and off Display queued tanks on lcd
 * // "stationpower" Toggles on and off Display Station Power on lcd
 * // "dockedships" Toggles on and off Display of docked ships on lcd
 */


// === End Of Config === //
/*
*
* // Add argument "recheck" without "" after dragging your pb to the hotbar and doing action run
* // This is used to reinitialize the groups after you have added or removed tanks, h2generators or connectors from the groups.
* // Sometimes you might have to recompile. Atleast when some specific settings are changed. Or groups have been updated. Since
* // even if you use recheck it still wont use the new added tanks or h2gens until the refueling process has been intiated again.
* // Sometimes you will have to recompile script if you for example add new reactors to the station and have reactor handling enabled.
*/
private StringBuilder lcdTextBuilder = new StringBuilder();
IMyBlockGroup Ģ;IMyBlockGroup ġ;IMyBlockGroup Ġ;IMyTextPanel ù;List<IMyGasTank>ğ=new List<IMyGasTank>();List<
IMyTerminalBlock>Ğ=new List<IMyTerminalBlock>();List<IMyShipConnector>ģ=new List<IMyShipConnector>();List<IMyReactor>Å=new List<
IMyReactor>();List<IMyReactor>ĝ=new List<IMyReactor>();List<IMyReactor>Ĝ=new List<IMyReactor>();bool ě=false;int Ě=0;float ę=0f;
bool Ę=false;bool ė=false;bool Ė=false;bool ĕ=false;bool Ĕ=false;Dictionary<IMyCubeGrid,bool>ē=new Dictionary<IMyCubeGrid,
bool>();Dictionary<IMyShipConnector,IMyCubeGrid>Ē=new Dictionary<IMyShipConnector,IMyCubeGrid>();Dictionary<IMyCubeGrid,
DateTime>ï=new Dictionary<IMyCubeGrid,DateTime>();HashSet<IMyCubeGrid>ÿ=new HashSet<IMyCubeGrid>();Program(){Runtime.
UpdateFrequency=UpdateFrequency.Update10;þ();}void þ(){ý(ref Ģ,TANK_GROUP_NAME);ý(ref ġ,GENERATOR_GROUP_NAME);ý(ref Ġ,
CONNECTOR_GROUP_NAME);ù=û(LCD_PANEL_TAG);Ģ.GetBlocksOfType(ğ);ġ.GetBlocks(Ğ);Ġ.GetBlocksOfType(ģ);Ę=false;ė=false;p(ReactorFuelSubtypeId);ď(
);foreach(IMyGasTank D in ğ){if(D.Enabled==true){D.Enabled=false;}}}void ý(ref IMyBlockGroup ü,string ú){ü=
GridTerminalSystem.GetBlockGroupWithName(ú);if(ü==null){Echo($"Error: No group found with name '{ú}'.\n");}}IMyTextPanel û(string ú){List<
IMyTextPanel>Ā=new List<IMyTextPanel>();GridTerminalSystem.GetBlocksOfType(Ā);foreach(IMyTextPanel ù in Ā){if(ù.CustomName.Contains(
ú)){return ù;}}Echo($"Error: No LCD panel found with name containing '{ú}'.\n");return null;}string ö(){StringBuilder õ=
new StringBuilder();float ô=Ç();float Ö=Ù();õ.AppendLine($" <=== Station Hydrogen Info ===>");õ.AppendLine(
$"Average Station HydrogenFillLevel: {C():P0}");õ.AppendLine($"H2Gen Output: {Ï(Ö)}");õ.AppendLine($"ETC Refueling: {ŏ(ô)}");õ.AppendLine(
$" <=== Info & Settings ===>");õ.AppendLine($"Ship Docked: {R()}");õ.AppendLine($"Refuel Ship: {AutoRefuelShip}");õ.AppendLine(
$"Charge Ship Batteries: {RechargeBatteries}"+" | "+$"Reactor Handeling Ship: {ReactorHandlingShip}");õ.AppendLine($"Auto Undock Ship: {AutoUndock}"+"      | "+
$"Auto On Ship Thrust: {UndockThrust}");õ.AppendLine($" <=== Other Station Features ===>");õ.AppendLine($"Reactor Handeling Station: {ReactorHandling}");õ.
AppendLine($"H2 Handling: {HandleH2Generators}"+"                | "+$"Ice per cycle: {IceAmount}");õ.AppendLine(
$"Minimum Station Power: {MinimumStationPowerLevel}");õ.AppendLine($"Auto Undock Timer: {AutoUndockWaitingPeriod}"+" Seconds");IMyTextSurface ó=Me.GetSurface(0);ó.
ContentType=ContentType.TEXT_AND_IMAGE;ó.Alignment=TextAlignment.LEFT;ó.FontSize=0.7f;string ò="Station Refueler v1.8";ó.WriteText(
ò+"\n"+õ.ToString());return õ.ToString();}void Main(string ñ){if(ñ=="recheck"){þ();}bool ð=R();float ø=C();bool ā=ğ.Any(D
=>D.FilledRatio<1);Ĵ();n();if(ð){E();f();ě=false;}if(ā&&!ě&&!ð){Q();}if(Ō()){N();È();Ą();Ĥ();}if(!ā&&!ð&&ğ.Any(D=>D.
FilledRatio==1)){F();}foreach(IMyCubeGrid đ in ÿ){Echo($"Ship with Ore: {đ.CustomName}");}foreach(var Đ in ï){Echo(
$"Docked Ship: {Đ.Key.CustomName}, Docked Time: {Đ.Value}");}ŋ();ö();Č(ñ);}void ď(){string č=Me.CustomData;AutoRefuelShip=ċ(č,"AutoRefuelShip");AutoUndock=ċ(č,"AutoUndock");
RechargeBatteries=ċ(č,"RechargeBatteries");UndockThrust=ċ(č,"UndockThrust");HandleH2Generators=ċ(č,"HandleH2Generators");ReactorHandling=
ċ(č,"ReactorHandling");ReactorHandlingShip=ċ(č,"ReactorHandlingShip");TOTAL_HYDROGEN=ċ(č,"DEFAULT_TOTAL_TANKS");
LIST_ALL_TANKS=ċ(č,"DEFAULT_LIST_ALL_TANKS");LIST_QUEUED_TANKS=ċ(č,"DEFAULT_LIST_QUEUED_TANKS");DISPLAY_STATION_POWER=ċ(č,
"DEFAULT_DISPLAY_STATION_POWER");LIST_DOCKED_SHIPS=ċ(č,"LIST_DOCKED_SHIPS");}void Ď(){string č="";č+="AutoRefuelShip="+AutoRefuelShip.ToString()+"\n";č
+="AutoUndock="+AutoUndock.ToString()+"\n";č+="RechargeBatteries="+RechargeBatteries.ToString()+"\n";č+="UndockThrust="+
UndockThrust.ToString()+"\n";č+="HandleH2Generators="+HandleH2Generators.ToString()+"\n";č+="ReactorHandling="+ReactorHandling.
ToString()+"\n";č+="ReactorHandlingShip="+ReactorHandlingShip.ToString()+"\n";č+="TOTAL_HYDROGEN="+TOTAL_HYDROGEN.ToString()+
"\n";č+="LIST_ALL_TANKS="+LIST_ALL_TANKS.ToString()+"\n";č+="LIST_QUEUED_TANKS="+LIST_QUEUED_TANKS.ToString()+"\n";č+=
"DISPLAY_STATION_POWER="+DISPLAY_STATION_POWER.ToString()+"\n";č+="LIST_DOCKED_SHIPS="+LIST_DOCKED_SHIPS.ToString()+"\n";Me.CustomData=č;}void Č
(string ñ){if(ñ=="refuelship"){AutoRefuelShip=!AutoRefuelShip;Ď();}else if(ñ=="autoundock"){AutoUndock=!AutoUndock;Ď();}
else if(ñ=="rechargeship"){RechargeBatteries=!RechargeBatteries;Ď();}else if(ñ=="autothrust"){UndockThrust=!UndockThrust;Ď()
;}else if(ñ=="handleh2"){HandleH2Generators=!HandleH2Generators;Ď();}else if(ñ=="streactor"){ReactorHandling=!
ReactorHandling;Ď();}else if(ñ=="shipreactor"){ReactorHandlingShip=!ReactorHandlingShip;Ď();}else if(ñ=="totalhydrogen"){TOTAL_HYDROGEN
=!TOTAL_HYDROGEN;Ď();}else if(ñ=="alltanks"){LIST_ALL_TANKS=!LIST_ALL_TANKS;Ď();}else if(ñ=="queuedtanks"){
LIST_QUEUED_TANKS=!LIST_QUEUED_TANKS;Ď();}else if(ñ=="stationpower"){DISPLAY_STATION_POWER=!DISPLAY_STATION_POWER;Ď();}else if(ñ==
"dockedships"){LIST_DOCKED_SHIPS=!LIST_DOCKED_SHIPS;Ď();}}bool ċ(string č,string Ċ){int ĉ=č.IndexOf(Ċ+"=");if(ĉ!=-1){int Ĉ=ĉ+Ċ.Length
+1;int ć=č.IndexOf("\n",Ĉ);if(ć!=-1){string Ć=č.Substring(Ĉ,ć-Ĉ);bool ą;return bool.TryParse(Ć,out ą)?ą:false;}}return
false;}void Ą(){bool ă=(!HandleH2Generators&&!Ę);if(HandleH2Generators){if(!Ę){foreach(IMyGasGenerator G in Ğ){G.
UseConveyorSystem=false;}Ę=true;}Ĺ();}else if(!HandleH2Generators&&Ę){foreach(IMyGasGenerator G in Ğ){G.UseConveyorSystem=true;}Ę=false;}
else if(ă){foreach(IMyGasGenerator G in Ğ){G.UseConveyorSystem=true;}Ę=false;}}void Ĥ(){bool ă=(!ReactorHandling&&!ė);if(
ReactorHandling){if(!ė){foreach(IMyReactor Z in Å){Z.UseConveyorSystem=false;}ė=true;}j();}else if(!ReactorHandling&&ė){foreach(
IMyReactor Z in Å){Z.UseConveyorSystem=true;}ė=false;}else if(ă){foreach(IMyReactor Z in Å){Z.UseConveyorSystem=true;}ė=false;}}
bool Ō(){return ě;}void ŋ(){if(ù!=null){ù.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;ù.Alignment=VRage.
Game.GUI.TextPanel.TextAlignment.CENTER;ù.FontSize=0.6f;string Ŋ=ŗ();string Ň=ù.GetText();StringBuilder ŉ=ň(Ň,Ŋ);ŀ(ŉ);}}
StringBuilder ň(string Ň,string Ŋ){StringBuilder ņ=new StringBuilder();string[]Ņ=Ň.Split('\n');string[]ń=Ŋ.Split('\n');foreach(string
Ń in ń){bool ł=false;foreach(string Ł in Ņ){if(Ł.Trim()==Ń.Trim()){ņ.AppendLine(Ń);ł=true;break;}}if(!ł){ņ.AppendLine(Ń);
}}return ņ;}void ŀ(StringBuilder Ŀ){float ľ=1f;ù.WriteText("");string[]Ľ=Ŀ.ToString().Split('\n');foreach(string Ý in Ľ){
string ō=Ý;while(ù.MeasureStringInPixels(new StringBuilder(ō),ù.Font,ľ).X>ù.SurfaceSize.X){int œ=ō.Length/2;int Ś=ō.
LastIndexOf(' ',œ);if(Ś!=-1){string ř=ō.Substring(0,Ś);string Ř=ō.Substring(Ś+1);ù.WriteText(ř+"\n",true);ō=Ř;}else{œ++;}}ù.
WriteText(ō+"\n",true);}}string ŗ(){StringBuilder Ħ=new StringBuilder();Ħ.AppendLine("==================================");Ħ.
AppendLine("||                  Station Refueler v1.8                  ||");Ħ.AppendLine("==================================");Ħ.
AppendLine($"Local Time: {DateTime.Now:HH:mm:ss}");float Ŗ=Ç();Echo($"Time: {ŏ(Ŗ)}");Ħ.AppendLine($"ETC Refueling: {ŏ(Ŗ)}");Ŕ(ref
Ħ,Ė,"Warning: No ice ore found in cargo containers.");Ŕ(ref Ħ,ĕ,
$"Warning: No {ReactorFuelSubtypeId} found in cargo containers.");if(DISPLAY_STATION_POWER){Ħ.AppendLine("=> Station Power Level <=");double Æ=î();string ŕ=Æ.ToString("F0");Ħ.
AppendLine($"{ŕ} MW");if(Æ<=MinimumStationPowerLevel){Ħ.AppendLine("WARNING: Low Station Power. Undocking ships to conserve power"
);}}if(TOTAL_HYDROGEN){Ħ.AppendLine("=> Total Hydrogen <=");į(Ħ);}if(LIST_QUEUED_TANKS&&LIST_ALL_TANKS){Ħ.AppendLine(
"Warning: Both 'List Queued Tanks' and 'List All Tanks' options are enabled. Please enable only one of them.");}else{string Ő=
"Warning: Both 'List Queued Tanks' and 'List All Tanks' options are enabled. Please enable only one of them.";if(Ħ.ToString().Contains(Ő)){int Ĉ=Ħ.ToString().IndexOf(Ő);int ć=Ĉ+Ő.Length;Ħ.Remove(Ĉ,ć-Ĉ);}if(LIST_QUEUED_TANKS){Ħ.
AppendLine("=> Queued Tanks <=");ĩ(Ħ);}if(LIST_ALL_TANKS){Ħ.AppendLine("=> All Tanks <=");Ĩ(Ħ);}}if(LIST_DOCKED_SHIPS){Ħ.
AppendLine("=> Docked Ships <=");ħ(Ħ);}return Ħ.ToString();}void Ŕ(ref StringBuilder Ħ,bool Œ,string Ő){if(Œ){int Ĉ=Ħ.ToString().
IndexOf(Ő);if(Ĉ!=-1){Ħ.Remove(Ĉ,Ő.Length);}}else{Ħ.AppendLine(Ő);}}void ő(ref StringBuilder Ħ,string Ő){int Ĉ=Ħ.ToString().
IndexOf(Ő);if(Ĉ!=-1){Ħ.Remove(Ĉ,Ő.Length);}}string ŏ(float Ŏ){if(Ŏ<0){return"N/A";}TimeSpan ļ=TimeSpan.FromMilliseconds(Ŏ);int
Ļ=(int)(ļ.TotalMilliseconds/(1000*60*60*24));int ĥ=(int)((ļ.TotalMilliseconds/(1000*60*60))%24);int Ĳ=(int)((ļ.
TotalMilliseconds/(1000*60))%60);int ı=(int)((ļ.TotalMilliseconds/1000)%60);string İ="";if(Ļ>0){İ+=$"{Ļ} day{(Ļ>1?"s":"")} ";}if(ĥ>0||Ļ>0
){İ+=$"{ĥ} hour{(ĥ>1?"s":"")} ";}if(Ĳ>0||ĥ>0||Ļ>0){İ+=$"{Ĳ} minute{(Ĳ>1?"s":"")} ";}İ+=$"{ı} second{(ı>1?"s":"")}";return
İ;}void į(StringBuilder Ħ){float Į=C();Ħ.AppendLine($"Hydrogen: {Į:P0}");}void ĭ(StringBuilder Ħ,bool Ĭ){for(int ĳ=0;ĳ<ğ.
Count;ĳ++){float ī=(float)ğ[ĳ].FilledRatio;Ħ.AppendLine($"{ĳ+1}: {ğ[ĳ].CustomName} ({ī:P0})");if(!Ĭ&&ĳ>=Ě){break;}}}void ĩ(
StringBuilder Ħ){ĭ(Ħ,Ĭ:false);}void Ĩ(StringBuilder Ħ){ĭ(Ħ,Ĭ:true);}void ħ(StringBuilder Ħ){foreach(IMyShipConnector X in ģ){if(X.
Status==MyShipConnectorStatus.Connected){IMyShipConnector e=X.OtherConnector;if(e!=null){IMyEntity d=(IMyEntity)e.CubeGrid;if(
d!=null){string Ī=d.DisplayName;Ħ.AppendLine($"{Ī}");}}}}}void Ĵ(){List<IMyCargoContainer>ĸ=new List<IMyCargoContainer>()
;GridTerminalSystem.GetBlocksOfType(ĸ,l=>l.CustomName.Contains(IceTag)&&l.HasInventory&&l.GetInventory(0)!=null);Ė=ĸ.Any(
l=>{List<MyInventoryItem>k=new List<MyInventoryItem>();l.GetInventory(0).GetItems(k);return k.Any(g=>g.Type.SubtypeId.
ToLower()=="ice");});}void Ĺ(){List<IMyCargoContainer>ĸ=new List<IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType(ĸ,l=>l
.CustomName.Contains(IceTag)&&l.HasInventory&&l.GetInventory(0)!=null);Ė=ĸ.Any(l=>{List<MyInventoryItem>k=new List<
MyInventoryItem>();l.GetInventory(0).GetItems(k);return k.Any(g=>g.Type.SubtypeId.ToLower()=="ice");});bool ĺ=Ğ.All(G=>ĵ((
IMyGasGenerator)G)==0);if(ĺ){double ķ=IceAmount*Ğ.Count;foreach(var l in ĸ){IMyInventory ª=l.GetInventory(0);List<MyInventoryItem>k=new
List<MyInventoryItem>();ª.GetItems(k,null);foreach(var G in Ğ){double Ķ=IceAmount;foreach(var g in k){if(g.Type.SubtypeId.
ToLower()=="ice"){double º=Math.Min((double)g.Amount,Ķ);ª.TransferItemTo(G.GetInventory(),g,(VRage.MyFixedPoint)º);Ķ-=º;if(Ķ<=0
){break;}}}}}}else{}}double ĵ(IMyGasGenerator G){IMyInventory ª=G.GetInventory();double y=0;List<MyInventoryItem>k=new
List<MyInventoryItem>();ª.GetItems(k);foreach(var g in k){if(g.Type.SubtypeId.ToLower()=="ice"){y+=(double)g.Amount;}}return
y;}void w(){try{ĝ.Clear();Ĝ.Clear();GridTerminalSystem.GetBlocksOfType(Å);if(Å.Count==0){Echo("No reactors found.");
return;}foreach(IMyReactor Z in Å){if(v(Z)){ĝ.Add(Z);}else{Ĝ.Add(Z);}}Echo(
$"Found {ĝ.Count} large reactors and {Ĝ.Count} small reactors.");}catch(Exception ex){Echo($"Error classifying reactors: {ex.Message}");}}bool v(IMyReactor Z){IMyCubeBlock u=Z as
IMyCubeBlock;Vector3I t=u.Min;Vector3I s=u.Max;int r=s.X-t.X+1;int x=s.Y-t.Y+1;int q=s.Z-t.Z+1;return(r>=2||x>=2||q>=2);}void p(
string o){ReactorFuelSubtypeId=o;}void n(){List<IMyCargoContainer>m=new List<IMyCargoContainer>();GridTerminalSystem.
GetBlocksOfType(m,l=>l.HasInventory&&l.GetInventory(0)!=null);ĕ=m.Any(l=>{List<MyInventoryItem>k=new List<MyInventoryItem>();l.
GetInventory(0).GetItems(k);return k.Any(g=>g.Type.SubtypeId.ToLower()==ReactorFuelSubtypeId.ToLower());});}void j(){w();List<
IMyCargoContainer>m=new List<IMyCargoContainer>();GridTerminalSystem.GetBlocksOfType(m,l=>l.HasInventory&&l.GetInventory(0)!=null);Á(m,
ReactorFuelSubtypeId,ĝ,LargeStationReactorFuelAmount);Á(m,ReactorFuelSubtypeId,Ĝ,SmallStationReactorFuelAmount);}void Á(List<
IMyCargoContainer>m,string o,List<IMyReactor>Å,double Ä){ĕ=m.Any(l=>{List<MyInventoryItem>k=new List<MyInventoryItem>();l.GetInventory(0)
.GetItems(k);return k.Any(g=>g.Type.SubtypeId.ToLower()==o.ToLower());});if(!ĕ){Echo(
$"Warning: No {o} found in cargo containers.");return;}bool Ã=Å.All(Z=>µ(Z)==0);if(Ã){double Â=Ä*Å.Count;foreach(var l in m){IMyInventory ª=l.GetInventory(0);List<
MyInventoryItem>k=new List<MyInventoryItem>();ª.GetItems(k,null);foreach(var Z in Å){double À=Ä;foreach(var g in k){if(g.Type.SubtypeId
.ToLower()==o.ToLower()){double º=Math.Min((double)g.Amount,À);ª.TransferItemTo(Z.GetInventory(),g,(VRage.MyFixedPoint)º)
;À-=º;if(À<=0){break;}}}}}}else{}}double µ(IMyReactor Z){IMyInventory ª=Z.GetInventory();double z=0;List<MyInventoryItem>
k=new List<MyInventoryItem>();ª.GetItems(k);foreach(var g in k){if(g.Type.SubtypeId.ToLower()==ReactorFuelSubtypeId.
ToLower()){z+=(double)g.Amount;}}return z;}void Q(){ě=true;foreach(IMyGasTank D in ğ){D.Enabled=false;}I();}void P(){if(ě){O();
}}void O(){foreach(IMyGasTank D in ğ){D.Enabled=false;}Ě=0;ě=false;}void N(){if(ě){M(ğ,ref Ě);}else{I();}}void M(List<
IMyGasTank>L,ref int K){Echo("Refueling tanks one by one.\n");while(K<L.Count){J(L[K]);if(L[K].FilledRatio<1){break;}K++;}if(K==L.
Count){P();}}void J(IMyGasTank D){if(D.FilledRatio<1){D.Enabled=true;}else{D.Enabled=false;}}void I(){foreach(IMyGasGenerator
G in Ğ){G.ApplyAction("OnOff_On");}}void H(){foreach(IMyGasGenerator G in Ğ){G.ApplyAction("OnOff_Off");}}void F(){H();ğ.
ForEach(D=>D.Enabled=false);}void E(){ğ.ForEach(D=>D.Enabled=true);H();}float C(){if(ğ.Count==0){return 0;}float B=ğ.Sum(D=>(
float)D.FilledRatio);float A=(B/ğ.Count);if(A!=ę){ę=A;}return ę;}bool R(){return ģ.Any(X=>X.Status==MyShipConnectorStatus.
Connected);}void f(){foreach(IMyShipConnector X in ģ){if(X.Status==MyShipConnectorStatus.Connected){IMyShipConnector e=X.
OtherConnector;if(e!=null){IMyCubeGrid d=(IMyCubeGrid)e.CubeGrid;if(d!=null){Ē[X]=d;if(!ē.ContainsKey(d)){ē[d]=false;}List<IMyGasTank>
V=new List<IMyGasTank>();List<IMyThrust>U=new List<IMyThrust>();List<IMyBatteryBlock>T=new List<IMyBatteryBlock>();List<
IMyReactor>S=new List<IMyReactor>();foreach(IMyReactor c in S){c.UseConveyorSystem=false;}GridTerminalSystem.GetBlocksOfType(V,D=>
D.CubeGrid==d);GridTerminalSystem.GetBlocksOfType(U,b=>b.CubeGrid==d);GridTerminalSystem.GetBlocksOfType(T,a=>a.CubeGrid
==d);GridTerminalSystem.GetBlocksOfType(S,Z=>Z.CubeGrid==d);Y(X,d,V,U,T,S);}}}}}void Y(IMyShipConnector X,IMyCubeGrid W,
List<IMyGasTank>V,List<IMyThrust>U,List<IMyBatteryBlock>T,List<IMyReactor>S){List<IMyGasTank>h=V.Where(D=>D.DetailedInfo.
Contains("Type: Hydrogen Tank")||D.DetailedInfo.Contains("Type: Small Hydrogen Tank")).ToList();double Æ=î();if(
ReactorHandlingShip){foreach(IMyReactor Z in S){Z.UseConveyorSystem=false;List<IMyReactor>é=new List<IMyReactor>();List<IMyReactor>è=new
List<IMyReactor>();IMyCubeBlock u=(IMyReactor)Z;Vector3I t=u.Min;Vector3I s=u.Max;int r=s.X-t.X+1;int x=s.Y-t.Y+1;int q=s.Z-
t.Z+1;if(r>2||x>2||q>2){é.Add(Z);}else{è.Add(Z);}List<IMyCargoContainer>ç=new List<IMyCargoContainer>();
GridTerminalSystem.GetBlocksOfType(ç,l=>l.CubeGrid==Me.CubeGrid&&l.HasInventory&&l.GetInventory(0)!=null);double æ=
LargeShipReactorFuelAmount*é.Count;double ä=SmallShipReactorFuelAmount*è.Count;foreach(var l in ç){IMyInventory ª=l.GetInventory(0);List<
MyInventoryItem>k=new List<MyInventoryItem>();ª.GetItems(k,null);foreach(var â in é){double ã=LargeShipReactorFuelAmount-µ(â);if(ã>0){
foreach(var g in k){if(g.Type.SubtypeId.ToLower()==ReactorFuelSubtypeId.ToLower()){double º=Math.Min((double)g.Amount,ã);ª.
TransferItemTo(â.GetInventory(),g,(VRage.MyFixedPoint)º);ã-=º;if(ã<=0){break;}}}}}foreach(var â in è){double å=
SmallShipReactorFuelAmount-µ(â);if(å>0){foreach(var g in k){if(g.Type.SubtypeId.ToLower()==ReactorFuelSubtypeId.ToLower()){double º=Math.Min((
double)g.Amount,å);ª.TransferItemTo(â.GetInventory(),g,(VRage.MyFixedPoint)º);å-=º;if(å<=0){break;}}}}}}if(æ<=0&&ä<=0)break;}}
if(RechargeBatteries){foreach(IMyBatteryBlock a in T){if(a.CurrentStoredPower<MaxRecharge){a.ChargeMode=ChargeMode.
Recharge;}else if(a.CurrentStoredPower>=MaxRecharge){a.ChargeMode=ChargeMode.Auto;}}if(Æ>=MinimumStationPowerLevel){foreach(
IMyBatteryBlock a in T){a.ChargeMode=ChargeMode.Recharge;}}else{foreach(IMyBatteryBlock a in T){a.ChargeMode=ChargeMode.Auto;}}}if(
AutoRefuelShip){foreach(IMyGasTank D in h){if(!D.Stockpile&&!ē[W]){D.Stockpile=true;}}}bool í=ğ.All(D=>D.FilledRatio<=0);if((
AutoRefuelShip&&í&&(RechargeBatteries&&T.All(a=>a.CurrentStoredPower>=MaxRecharge))||(RechargeBatteries&&Æ<MinimumStationPowerLevel))
||(AutoRefuelShip&&h.All(D=>D.FilledRatio>=1&&(RechargeBatteries&&T.All(a=>a.CurrentStoredPower>=MaxRecharge))||(!
AutoRefuelShip&&RechargeBatteries&&Æ<MinimumStationPowerLevel||RechargeBatteries&&T.All(a=>a.CurrentStoredPower>=MaxRecharge))||(
RechargeBatteries&&Æ<MinimumStationPowerLevel)))){ē[W]=true;if(AutoUndock){ê(X,W,h,U,T,S);}}}double î(){double Æ=0.0;List<IMyBatteryBlock
>ì=new List<IMyBatteryBlock>();GridTerminalSystem.GetBlocksOfType(ì);IMyCubeGrid ë=Me.CubeGrid;foreach(IMyBatteryBlock a
in ì){if(a.CubeGrid==ë){Æ+=a.CurrentStoredPower;}}return Æ;}void ê(IMyShipConnector X,IMyCubeGrid W,List<IMyGasTank>h,List
<IMyThrust>U,List<IMyBatteryBlock>T,List<IMyReactor>S){bool á=Í(W);if(AutoUndock&&ÿ.Contains(W)&&!á){foreach(IMyGasTank D
in h){if(D.Stockpile){D.Stockpile=false;}}foreach(IMyBatteryBlock a in T){ChargeMode Ë=a.ChargeMode;if(Ë==ChargeMode.
Recharge){a.ChargeMode=ChargeMode.Auto;}}if(UndockThrust){foreach(IMyThrust b in U){if(!b.Enabled){b.Enabled=true;}}}if(
AutoUndock){foreach(IMyReactor Z in S){Z.UseConveyorSystem=true;}X.Disconnect();}ÿ.Remove(W);return;}if(AutoUndock&&á){ÿ.Add(W);if
(ï.ContainsKey(W)){ï.Remove(W);}return;}if(!ï.ContainsKey(W)){ï[W]=DateTime.Now;}bool Ì=(DateTime.Now-ï[W]).TotalSeconds
>=AutoUndockWaitingPeriod;if(AutoUndock&&Ì){foreach(IMyGasTank D in h){if(D.Stockpile){D.Stockpile=false;}}foreach(
IMyBatteryBlock a in T){ChargeMode Ë=a.ChargeMode;if(Ë==ChargeMode.Recharge){a.ChargeMode=ChargeMode.Auto;}}if(UndockThrust){foreach(
IMyThrust b in U){if(!b.Enabled){b.Enabled=true;}}}if(AutoUndock){foreach(IMyReactor Z in S){Z.UseConveyorSystem=true;}X.
Disconnect();}ï.Remove(W);}}bool Í(IMyCubeGrid W){List<IMyCargoContainer>É=new List<IMyCargoContainer>();GridTerminalSystem.
GetBlocksOfType(É,l=>l.CubeGrid==W);return É.Any(l=>{List<MyInventoryItem>k=new List<MyInventoryItem>();l.GetInventory().GetItems(k);
return k.Any(g=>g.Type.TypeId.ToString()=="MyObjectBuilder_Ore"&&g.Type.SubtypeId!="ice");});}void È(){if(!Ĕ){foreach(
IMyShipConnector X in ģ){if(X.Status==MyShipConnectorStatus.Unconnected){Echo($"{X.Status} No Ships docked\n");if(Ē.ContainsKey(X)){
IMyCubeGrid d=Ē[X];if(ē.ContainsKey(d)){ē.Remove(d);ě=false;}Ē.Remove(X);}}}Ĕ=true;}}float Ç(){float Ê=ğ.Sum(D=>(float)D.
FilledRatio*(float)D.Capacity);float Ø=ğ.Sum(D=>(float)D.Capacity);float Ö=0;foreach(IMyGasGenerator G in Ğ){string ß=G.CustomInfo;
string[]Þ=ß.Split('\n');foreach(string Ý in Þ){if(Ý.StartsWith("Output H2:")){string Ü=Ý.Split(':')[1].Trim().Split(' ')[0];
float Ð;if(float.TryParse(Ü,out Ð)){Ö+=Ð;}}}}if(Ö<=0||Ê>=Ø){return-1;}string Û=Ö<1f?"L/s":"kL/s";float Ú=Ö;if(Ö>=1f){Û="kL/s"
;Ú/=1000f;}float à=(Ø-Ê)/Ú;return à;}float Ù(){float Ö=0f;foreach(IMyGasGenerator G in Ğ){string Õ=G.CustomInfo;if(Õ.
Contains("Output H2:")){string Ô=Õ.Substring(Õ.IndexOf("Output H2:"));string[]Ó=Ô.Split(':');if(Ó.Length>1){string Ò=Ó[1].Trim()
;int Ñ=Ò.IndexOf(' ');if(Ñ!=-1){Ò=Ò.Substring(0,Ñ);}float Ð;if(float.TryParse(Ò,out Ð)){Ö+=Ð;}}}}Echo(
$"Generator Output: {Ö}");return Ö;}string Ï(float Ă){if(Ă>=1000f){float Î=Ă/1000f;return$"{Î} kL/s";}else{return$"{Ă} L/s";}}