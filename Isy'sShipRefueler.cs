
// Isy's Ship Refueler
// ===============
// Version: 1.9.5
// Date: 2021-03-09

// =======================================================================================
//                                                                            --- Configuration ---
// =======================================================================================

// -- Batteries --
// =======================================================================================

// By default, all batteries of the ship are managed by the script. If you want to monitor specific ones,
// group them together and put the group name here. Leave blank to monitor all.
// Example: string batteryGroupName = "My Vehicle Batteries";
string batteryGroupName = "";

// Set batteries to discharge when not docked?
// If this option is set to false, the batteries will be in Auto mode (neither charge nor discharge)
bool dischargeWhenUndocked = false;

// Always keep one backup battery when docked?
// The script will otherwise set all batteries to recharge if the ship is not controlled by a player.
// This is highly recommended for fully automated, self docking drones
bool keepOneBackupBattery = false;

// If the script detects, that your base hase not enough power to supply the ship, it will stop charging
// in order to prevent the base from being drained.
bool baseDrainProtection = true;


// -- Tanks --
// =======================================================================================

// If this is enabled, the script will switch all tanks to stockpile, if you are docked.
// That way the tanks will be filled very fast. Stockpile is deactivated when undocking.
bool fillTanks = true;

// If you only want specific tanks to be managed, put them in a group and define the groupname here.
// Example: string tankGroupName = "My Vehicle Tanks";
string tankGroupName = "";


// -- Connectors --
// =======================================================================================

// By default, the script monitors all connectors of the ship for a connection.
// If you only want to monitor specific ones, create a group and put the name here.
string connectorGroupName = "";


// -- Special connections --
// =======================================================================================
// If you don't want to perform a specific action when docked to a connector, add the keyword to its name.
// The keyword must be added to the other connector's name, NOT your ship's connector!

// -- No refuel --
// This disables battery charging and tank filling (they are turned off).
string noRefuelKeyword = "[No Refuel]";

// -- Ignore --
// This ignores a connection and leaves batteries and tanks the way they are.
string ignoreKeyword = "[Ignore]";

// -- No recharge --
// This disables battery recharging (they are turned off).
string noRechargeKeyword = "[No Recharge]";

// -- Discharge at connector --
// This discharges all batteries to a minimal value of dischargeLevel % charge (they are set to discharge).
string dischargeKeyword = "[Discharge]";
double dischargeLevel = 10;

// -- No tank filling --
// This disables tank filling of all tanks of the ship (they are turned off)
string noTankFillKeyword = "[No Refill]";

// -- No O2 filling --
// This disables O2 tank filling (they are turned off)
string noOxygenTankFillKeyword = "[No O2 Refill]";

// -- No H2 filling --
// This disables H2 tank filling (they are turned off)
string noHydrogenTankFillKeyword = "[No H2 Refill]";


// -- Emergency Power --
// =======================================================================================

// If your ship runs low on battery charge or the output gets overloaded, the script will
// enable reactors and hydrogen engines to help out. If everything is normal again, they will be disabled.
bool enableEmergencyPower = true;

// -- Used power producers --
// Which power producers do you want to use for emergency power?
bool useReactors = true;
bool useHydrogenEngines = true;

// -- Activation order --
// By default, the hydrogen engines will be turned on first and the reactors after that if still not enough power is available.
// Set this value to false, and the reactors will be used first (ignored if either of those is set to false in 'Used power producers').
bool activateHydrogenEngineFirst = true;

// -- Activation scenarios --
bool activateOnLowBattery = true;
bool activateOnOverload = true;
bool activateOnDamagedBatteries = true;
bool activateOnNoBatteries = true;

// -- Tresholds --
// At which point should the emergency devices kick in (in percent of max charge)?
double lowChargePercentageON = 10;
double lowChargePercentageOFF = 15;
double overloadPercentage = 90;


// -- Light control --
// =======================================================================================

// If you want to switch the lights of your ship automatically, you can activate this feature here.
// You need a solar panel to measure the light level. The light level is measured in % of the max output.
bool lightControl = false;
double lightLevel = 50;

// By default, all solar panels of the ship are monitored. If you want to monitor specific solar panels,
// declare a group for them.
// Example: string solarPanelGroupName = "My Vehicle Solar Panels";
string solarPanelGroupName = "";

// To only toggle specific lights, declare groups for them.
// Example: string[] lightGroups = { "Interior Lights", "Spotlights", "Position Lights" }
string[] lightGroups = { };


// --- LCD panels ---
// =======================================================================================

// To display the main script informations, add the following keyword to any LCD name (default: !ISR-main).
// You can enable or disable specific informations on the LCD by editing its custom data.
string mainLCDKeyword = "ISR-main";

// To display all current warnings and problems, add the following keyword to any LCD name (default: !warnings).
string warningsLCDKeyword = "ISR-warnings";

// To display the script performance, add the following keyword to any LCD name (default: !performance).
string performanceLCDKeyword = "ISR-performance";

// Default screen font, fontsize and padding, when a screen is first initialized. Fonts: "Debug" or "Monospace"
string defaultFont = "Debug";
float defaultFontSize = 0.8f;
float defaultPadding = 2f;


// --- Terminal statistics ---
// =======================================================================================

// The script can display informations in the names of the used blocks. The shown information is a percentage of
// the current output (solar panels, reactors and hydrogen engines) or the fill level (batteries and tanks).
// You can enable or disable single statistics or disable all using the master switch below.
bool enableTerminalStatistics = true;

bool showBatteryStats = true;
bool showTankStats = true;
bool showPowerDeviceStats = true;
bool showSolarStats = true;


// -- Timer trigger on event --
// =======================================================================================

// If you want to trigger a timer block to execute additional actions on specific events, you can
// declare it here. The timer will be executed with the action "Trigger Now" if its delay is set to exactly 1 second and
// "Start" if it is more than 1 second (1.x seconds or more)
//
// Events can be: "dock", "undock", a battery fill level like "25%" or a docking time in minutes:seconds like "5:30"
//
// Every event needs a timer block name in the exact same order as the events.
// Calling the same timer block with multiple events requires it's name multiple times in the timers list!
//
// Example:
// string[] events = { "dock", "undock", "25%" };
// string[] timers = { "Timer 1", "Timer 2", "Timer 3" };
// This will trigger "Timer 1" when docking, "Timer 2" when undocking and "Timer 3" when the battery level is at 25%
string[] events = { };
string[] timers = { };


// =======================================================================================
//                                                                      --- End of Configuration ---
//                                                        Don't change anything beyond this point!
// =======================================================================================


List<IMyShipConnector>Ž=new List<IMyShipConnector>();List<IMyBatteryBlock>ż=new List<IMyBatteryBlock>();List<IMyGasTank>
Ż=new List<IMyGasTank>();List<IMyGasTank>ź=new List<IMyGasTank>();List<IMyGasTank>Ź=new List<IMyGasTank>();List<
IMySolarPanel>Ÿ=new List<IMySolarPanel>();List<IMyInteriorLight>ŷ=new List<IMyInteriorLight>();List<IMyReflectorLight>Ŷ=new List<
IMyReflectorLight>();List<IMyPowerProducer>ŵ=new List<IMyPowerProducer>();List<IMyPowerProducer>Ŵ=new List<IMyPowerProducer>();List<
IMyPowerProducer>ų=new List<IMyPowerProducer>();List<IMyTerminalBlock>Ų=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ű=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>Ű=new List<IMyTerminalBlock>();List<IMyShipController>ů=new List<IMyShipController>();List<
IMyTerminalBlock>Ů=new List<IMyTerminalBlock>();float ő=0;float ĵ=0;float ŏ=0;float Ŏ=0;float ō=0;float Ō=0;float ŋ=0;float Ŋ=0;float ŉ=
0;bool ň=false;bool Ň=false;TimeSpan ņ=new TimeSpan();DateTime Ņ=DateTime.Now;double ń=0;double Ő=0;double Ń=0;double Ł=0
;bool ŀ=false;string Ŀ="";double ľ=0;double Ľ=0;bool ļ=false;bool Ļ=false;bool ĺ=false;int Ĺ=0;int ĸ=0;double ķ=0;string
Ķ="";string ł="";IMyShipConnector Œ=null;bool ş=false;bool ŭ=false;bool Ŭ=false;bool ū=false;bool Ū=true;bool ũ=false;
bool Ũ=true;bool ŧ=true;bool Ŧ=true;string ť="nothing";DateTime Ť=DateTime.MinValue;TimeSpan ţ=new TimeSpan();string[]Ţ={"/"
,"-","\\","|"};int š=0;string[]x={"showHeading=true","showWarnings=true","showConnectionStatus=true","showPowerTime=true"
,"showEmergencyPower=true","showBatteryStats=true","showTankStats=true","showLightStats=true","scrollTextIfNeeded=true"};
string[]Š={"showHeading=true","scrollTextIfNeeded=true"};string[]Ş={"solarPanelsCount=0","outputMax=0.01","docked=0",
"dockingTime="+DateTime.Now.ToOADate()};string ŝ="";string Ŝ="";string ś="";DateTime Ś;string Ð;HashSet<string>ř=new HashSet<string>()
;HashSet<string>Ř=new HashSet<string>();bool ŗ=false;int Ŗ=0;int ŕ=1;bool Ŕ=true;bool ž=true;string[]œ={"Get blocks",
"Get block stats","Manage blocks","Check Timers","Check light level"};Program(){lightLevel=(lightLevel%100)/100;dischargeLevel=(
dischargeLevel%100)/100;Runtime.UpdateFrequency=UpdateFrequency.Update10;}void Main(string Ƭ){if(Ŕ){Echo("Initializing.. "+Ţ[ŕ%4]);if(
ŕ==1)ƨ();if(ŕ>=2)Echo("Found: "+ż.Count+" batteries");if(ŕ>=3&&fillTanks){Echo("Found: "+ź.Count+" oxygen tanks");Echo(
"Found: "+Ź.Count+" hydrogen tanks");}if(ŕ>=4&&enableEmergencyPower){if(useReactors)Echo("Found: "+Ĺ+" reactors");if(
useHydrogenEngines)Echo("Found: "+ĸ+" hydrogen engines");}if(ŕ>=5&&lightControl)Echo("Found: "+Ÿ.Count+" solar panels");if(ŕ>=6&&
lightControl)Echo("Found: "+ŷ.Count+" interior lights");if(ŕ>=7&&lightControl)Echo("Found: "+Ŷ.Count+" spotlights");if(ŕ>=8)Echo(
"Found: "+Ž.Count+" connectors");if(ŕ>=9)Echo("Found: "+(ŀ?timers.Length:0)+" timer blocks");if(ŕ>=10)Echo("\nStarting script..")
;ŕ++;if(ŕ>=15){ŕ=1;Ŕ=false;}return;}if(ŕ==1){ŗ=ƛ();}if(ŗ){Echo("Warning!\n\nThe script has detected, that it is run on or as part of a station.\nSince the ship refueler is designed to run on ships, it is now stopped to prevent further issues!"
);if(ŕ>=4){ŕ=0;}else{ŕ++;}return;}try{if(ŭ&&Œ.Status!=MyShipConnectorStatus.Connected){ƴ();return;}}catch{}if(Ƭ!=""){ś=Ƭ;
ŕ=0;Ŝ="";Ś=DateTime.Now;}if(ƭ(ś))return;if(ž){ƕ();Ï();Ô();ž=false;return;}š=š>=3?0:š+1;ž=true;var ƚ=new List<
IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ƚ,n=>n.IsSameConstructAs(Me));int Ư=ƚ.Count;if(ŕ==0||Ư!=Ŗ){ƨ();Ŗ=Ư;if(ŕ==0){µ(œ[ŕ
]);ð();ŕ++;}return;}if(keepOneBackupBattery){ň=true;}else{Ň=false;foreach(var Ʈ in ů){if(Ʈ.IsUnderControl){if(!ň)ŕ=2;Ň=
true;ň=true;break;}}if(!Ň)ň=false;}if(ŕ==1){ƶ();}if(ŕ==2){ƴ();}if(ŀ&&ŕ==3){Ɗ();}if(lightControl&&ŕ==4){Ƅ();}µ(œ[ŕ]);ð();if(ŕ
>=4){ŕ=0;ř=new HashSet<string>(Ř);Ř.Clear();if(ř.Count==0)Ð=null;}else{ŕ++;}}bool ƭ(string Ƭ){bool ƫ=true;bool ſ=true;bool
ƪ=false;if(Ƭ!="msg"){if(!Ƭ.Contains(" on")&&!Ƭ.Contains(" off")&&!Ƭ.Contains(" toggle"))return false;if(Ƭ.Contains(" off"
))ſ=false;if(Ƭ.Contains(" toggle"))ƪ=true;}if(Ƭ=="msg"){}else if(Ƭ.StartsWith("dischargeWhenUndocked")){ŝ=
"Discharge when undocked";if(ƪ)ſ=!dischargeWhenUndocked;dischargeWhenUndocked=ſ;}else if(Ƭ.StartsWith("keepOneBackupBattery")){ŝ=
"Keep one backup battery";if(ƪ)ſ=!keepOneBackupBattery;keepOneBackupBattery=ſ;}else if(Ƭ.StartsWith("fillTanks")){ŝ="Fill tanks";if(ƪ)ſ=!
fillTanks;fillTanks=ſ;}else if(Ƭ.StartsWith("enableEmergencyPower")){ŝ="Emergency power";if(ƪ)ſ=!enableEmergencyPower;
enableEmergencyPower=ſ;}else if(Ƭ.StartsWith("useReactors")){ŝ="Use reactors";if(ƪ)ſ=!useReactors;useReactors=ſ;}else if(Ƭ.StartsWith(
"useHydrogenEngines")){ŝ="Use hydrogen engines";if(ƪ)ſ=!useHydrogenEngines;useHydrogenEngines=ſ;}else if(Ƭ.StartsWith(
"activateHydrogenEngineFirst")){ŝ="Activate hydrogen engine first";if(ƪ)ſ=!activateHydrogenEngineFirst;activateHydrogenEngineFirst=ſ;}else if(Ƭ.
StartsWith("activateOnLowBattery")){ŝ="Activate on low battery";if(ƪ)ſ=!activateOnLowBattery;activateOnLowBattery=ſ;}else if(Ƭ.
StartsWith("activateOnOverload")){ŝ="Activate on overload";if(ƪ)ſ=!activateOnOverload;activateOnOverload=ſ;}else if(Ƭ.StartsWith(
"activateOnDamagedBatteries")){ŝ="Activate on damaged batteries";if(ƪ)ſ=!activateOnDamagedBatteries;activateOnDamagedBatteries=ſ;}else if(Ƭ.
StartsWith("activateOnNoBatteries")){ŝ="Activate on no batteries";if(ƪ)ſ=!activateOnNoBatteries;activateOnNoBatteries=ſ;}else if(Ƭ
.StartsWith("lightControl")){ŝ="Light control";if(ƪ)ſ=!lightControl;lightControl=ſ;}else if(Ƭ.StartsWith(
"enableTerminalStatistics")){ŝ="Terminal statistics";if(ƪ)ſ=!enableTerminalStatistics;enableTerminalStatistics=ſ;Ɲ();}else if(Ƭ.StartsWith(
"showBatteryStats")){ŝ="Show battery stats";if(ƪ)ſ=!showBatteryStats;showBatteryStats=ſ;Ɲ();}else if(Ƭ.StartsWith("showTankStats")){ŝ=
"Show tank stats";if(ƪ)ſ=!showTankStats;showTankStats=ſ;Ɲ();}else if(Ƭ.StartsWith("showPowerDeviceStats")){ŝ="Show power device stats";if
(ƪ)ſ=!showPowerDeviceStats;showPowerDeviceStats=ſ;Ɲ();}else if(Ƭ.StartsWith("showSolarStats")){ŝ="Show solar stats";if(ƪ)
ſ=!showSolarStats;showSolarStats=ſ;Ɲ();}else{ƫ=false;}if(ƫ){TimeSpan Ʃ=DateTime.Now-Ś;if(Ŝ=="")Ŝ=ŝ+" temporarily "+(ſ?
"enabled":"disabled")+"!\n";Echo(Ŝ);Echo("Continuing in "+Math.Ceiling(3-Ʃ.TotalSeconds)+" seconds..");ś="msg";if(Ʃ.TotalSeconds
>=3){ŝ="";Ŝ="";ś="";}}return ƫ;}void ƨ(){Ų.Clear();ű.Clear();Ű.Clear();Ų=y(mainLCDKeyword,x,defaultFont,defaultFontSize,
defaultPadding);ű=y(warningsLCDKeyword,Š,defaultFont,defaultFontSize,defaultPadding);Ű=y(performanceLCDKeyword,Š,defaultFont,
defaultFontSize,defaultPadding);if(connectorGroupName!=""){var ư=GridTerminalSystem.GetBlockGroupWithName(connectorGroupName);if(ư!=
null){ư.GetBlocksOfType<IMyShipConnector>(Ž);}else{é("Connector group not found:\n'"+connectorGroupName+
"'\nUsing all connectors on the grid.");GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(Ž,Ĝ=>Ĝ.IsSameConstructAs(Me));}}else{GridTerminalSystem.
GetBlocksOfType<IMyShipConnector>(Ž,Ĝ=>Ĝ.IsSameConstructAs(Me));}List<IMyBatteryBlock>Ƨ=new List<IMyBatteryBlock>();if(batteryGroupName
!=""){var Ʀ=GridTerminalSystem.GetBlockGroupWithName(batteryGroupName);if(Ʀ!=null){Ʀ.GetBlocksOfType<IMyBatteryBlock>(Ƨ);}
else{é("Battery group not found:\n'"+batteryGroupName+"'\nUsing all connectors on the grid.");GridTerminalSystem.
GetBlocksOfType<IMyBatteryBlock>(Ƨ,n=>n.IsSameConstructAs(Me));}}else{GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(Ƨ,n=>n.
IsSameConstructAs(Me));}TimeSpan ƥ=DateTime.Now-Ņ;if(ƥ.TotalSeconds>=10||ż.Count!=Ƨ.Count){ż=Ƨ;if(ż.Count>=2&&Ŏ<ō*0.999){ż.Sort((Ƥ,n)=>Ƥ.
CurrentStoredPower.CompareTo(n.CurrentStoredPower));}Ņ=DateTime.Now;}if(fillTanks){if(tankGroupName!=""){var ƣ=GridTerminalSystem.
GetBlockGroupWithName(tankGroupName);if(ƣ!=null){ƣ.GetBlocksOfType<IMyGasTank>(ź,Ƣ=>Ƣ.IsSameConstructAs(Me)&&!Ƣ.BlockDefinition.SubtypeId.
Contains("Hydrogen"));ƣ.GetBlocksOfType<IMyGasTank>(Ź,Ƣ=>Ƣ.IsSameConstructAs(Me)&&Ƣ.BlockDefinition.SubtypeId.Contains(
"Hydrogen"));}else{é("Tank group not found:\n'"+tankGroupName+"'\nUsing all tanks on the grid.");GridTerminalSystem.
GetBlocksOfType<IMyGasTank>(ź,Ƣ=>Ƣ.IsSameConstructAs(Me)&&!Ƣ.BlockDefinition.SubtypeId.Contains("Hydrogen"));GridTerminalSystem.
GetBlocksOfType<IMyGasTank>(Ź,Ƣ=>Ƣ.IsSameConstructAs(Me)&&Ƣ.BlockDefinition.SubtypeId.Contains("Hydrogen"));}}else{GridTerminalSystem.
GetBlocksOfType<IMyGasTank>(ź,Ƣ=>Ƣ.IsSameConstructAs(Me)&&!Ƣ.BlockDefinition.SubtypeId.Contains("Hydrogen"));GridTerminalSystem.
GetBlocksOfType<IMyGasTank>(Ź,Ƣ=>Ƣ.IsSameConstructAs(Me)&&Ƣ.BlockDefinition.SubtypeId.Contains("Hydrogen"));}Ż.Clear();Ż.AddRange(ź);Ż.
AddRange(Ź);}foreach(var ƅ in timers){ŀ=true;var ƿ=GridTerminalSystem.GetBlockWithName(ƅ)as IMyTimerBlock;if(ƿ==null){é(
"Timer block not found:\n'"+ƅ+"'\nTimer functions disabled!");ŀ=false;break;}}if(lightControl){if(solarPanelGroupName!=""){var ƾ=GridTerminalSystem
.GetBlockGroupWithName(solarPanelGroupName);if(ƾ!=null){ƾ.GetBlocksOfType<IMySolarPanel>(Ÿ);}else{é(
"Solar panel group not found:\n'"+solarPanelGroupName+"'\nUsing all solar panels on the grid.");GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(Ÿ,ƽ=>ƽ.
IsSameConstructAs(Me));}}else{GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(Ÿ,ƽ=>ƽ.IsSameConstructAs(Me));}if(Ÿ.Count==0){é(
"No solar panel found!\nAdd one to use the light control!");}ŷ.Clear();Ŷ.Clear();if(lightGroups.Length>0){var Ƽ=new List<IMyInteriorLight>();var ƻ=new List<IMyReflectorLight>();
foreach(var ƺ in lightGroups){var ƹ=GridTerminalSystem.GetBlockGroupWithName(ƺ);if(ƹ!=null){ƹ.GetBlocksOfType<IMyInteriorLight>
(Ƽ);ŷ.AddRange(Ƽ);ƹ.GetBlocksOfType<IMyReflectorLight>(ƻ);Ŷ.AddRange(ƻ);}else{é("Light group not found:\n'"+ƺ+
"'\nUsing all lights on the grid.");GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(ŷ,Ƹ=>Ƹ.IsSameConstructAs(Me));GridTerminalSystem.GetBlocksOfType<
IMyReflectorLight>(Ŷ,Ƹ=>Ƹ.IsSameConstructAs(Me));}}}else{GridTerminalSystem.GetBlocksOfType<IMyInteriorLight>(ŷ,Ƹ=>Ƹ.IsSameConstructAs(Me
));GridTerminalSystem.GetBlocksOfType<IMyReflectorLight>(Ŷ,Ƹ=>Ƹ.IsSameConstructAs(Me));}}if(enableEmergencyPower){if(
useReactors)GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(ŵ,Ʒ=>(Ʒ is IMyReactor)&&Ʒ.IsSameConstructAs(Me)&&Ʒ.IsFunctional);
if(useHydrogenEngines)GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(Ŵ,Ʒ=>Ʒ.BlockDefinition.TypeIdString.Contains(
"HydrogenEngine")&&Ʒ.IsSameConstructAs(Me)&&Ʒ.IsFunctional);ų.Clear();ų.AddRange(ŵ);ų.AddRange(Ŵ);Ĺ=ŵ.Count;ĸ=Ŵ.Count;}
GridTerminalSystem.GetBlocksOfType<IMyShipController>(ů,Ĝ=>Ĝ.IsSameConstructAs(Me));}void ƶ(){Ŏ=0;ō=0;Ō=0;ŋ=0;Ŋ=0;ŉ=0;foreach(var ç in ż){
float Ƒ=ç.CurrentStoredPower;float Ɛ=ç.MaxStoredPower;Ŏ+=Ƒ;ō+=Ɛ;Ō+=ç.CurrentInput;ŋ+=ç.MaxInput;Ŋ+=ç.CurrentOutput;ŉ+=ç.
MaxOutput;}if(ŭ&&Ō>0){ņ=TimeSpan.FromHours(((ō-Ŏ)/Ō));}else if(!ŭ&&Ŋ>0){ņ=TimeSpan.FromHours((Ŏ/Ŋ));}else{ņ=TimeSpan.FromHours(0)
;}if(fillTanks){ń=0;Ő=0;Ń=0;Ł=0;foreach(var å in ź){ń+=å.Capacity;Ő+=å.Capacity*å.FilledRatio;}foreach(var å in Ź){Ń+=å.
Capacity;Ł+=å.Capacity*å.FilledRatio;}}if(lightControl){ľ=0;Ľ=î("outputMax");if(Ÿ.Count!=î("solarPanelsCount")){í(
"solarPanelsCount",Ÿ.Count);í("outputMax",0);}foreach(var Ɯ in Ÿ){ľ+=Ɯ.MaxOutput;if(showSolarStats&&enableTerminalStatistics){double Ƶ=0;
double.TryParse(Ɯ.CustomData,out Ƶ);if(Ƶ<Ɯ.MaxOutput){Ƶ=Ɯ.MaxOutput;Ɯ.CustomData=Ƶ.ToString();}ƀ(Ɯ,true,"",Ɯ.MaxOutput,Ƶ);}}if
(ľ>Ľ){Ľ=ľ;í("outputMax",Ľ);}}}void ƴ(){ş=false;if(ŭ){Ŭ=true;}else{Ŭ=false;}foreach(var Ƴ in Ž){if(!ŭ&&Ƴ.Status==
MyShipConnectorStatus.Connected){ŭ=true;Œ=Ƴ;ť="Normal Connector";ū=false;Ū=true;ũ=false;Ũ=true;ŧ=true;Ŧ=true;if(î("docked")==1){Ť=DateTime.
FromOADate(î("dockingTime"));}else{Ť=DateTime.Now;í("dockingTime",Ť.ToOADate());}string Ʋ=Ƴ.OtherConnector.CustomName;List<string>
Ʊ=new List<string>();if(Ʋ.Contains(noRefuelKeyword)){Ʊ.Add("No Refuel");Ū=false;Ũ=false;}if(Ʋ.Contains(ignoreKeyword)){Ʊ.
Add("Ignored");ū=true;}if(Ʋ.Contains(noRechargeKeyword)){Ʊ.Add("No Recharge");Ū=false;}if(Ʋ.Contains(dischargeKeyword)){Ʊ.
Add("Discharge");ũ=true;Ū=false;}if(Ʋ.Contains(noTankFillKeyword)){Ʊ.Add("No Refill");Ũ=false;}if(Ʋ.Contains(
noOxygenTankFillKeyword)){Ʊ.Add("No O2 Refill");ŧ=false;}if(Ʋ.Contains(noHydrogenTankFillKeyword)){Ʊ.Add("No H2 Refill");Ŧ=false;}if(Ʊ.Count>0)
{ť=String.Join(", ",Ʊ)+" Connector";}if(events.Length!=0&&î("docked")==0)Ɗ("dock");í("docked",1);break;}}try{if(ŭ&&Œ.
Status!=MyShipConnectorStatus.Connected){ŭ=false;Ť=DateTime.Now;if(events.Length!=0&&î("docked")==1)Ɗ("undock");í("docked",0);
í("dockingTime",Ť.ToOADate());}}catch{ŭ=false;í("docked",0);}if(Ť==DateTime.MinValue)Ť=DateTime.FromOADate(î(
"dockingTime"));ţ=DateTime.Now-Ť;string ſ="";string ǀ=null;if(baseDrainProtection&&Ŭ){if(ż.Count>0&&!ū&&!ũ&&Ū){GridTerminalSystem.
GetBlocksOfType(Ů,n=>(n is IMyPowerProducer||n is IMyBatteryBlock)&&n.IsSameConstructAs(Œ.OtherConnector));ő=0;ĵ=0;ŏ=0;bool Ơ=false;
bool Ɠ=false;for(int h=0;h<Ů.Count;h++){if(Ů[h]is IMyBatteryBlock){Ơ=true;ő+=(Ů[h]as IMyBatteryBlock).CurrentStoredPower;ĵ+=
(Ů[h]as IMyBatteryBlock).MaxStoredPower;}else if(Ů[h]is IMyPowerProducer){Ɠ=true;ŏ+=(Ů[h]as IMyPowerProducer).
CurrentOutput;}}if(Ơ&&ő<=ĵ*0.1){if(!Ɠ||(Ɠ&&ŏ==0))ş=true;}else if(Ɠ&&ŏ==0)ş=true;if(ş)é("Not enough power!\nCharging aborted!");}}
foreach(var ç in ż){if(!ç.CubeGrid.GetCubeBlock(ç.Position).IsFullIntegrity){ǀ=ç.CustomName+" is damaged!";}string ƒ=(ç.
CurrentInput-ç.CurrentOutput).Ĉ();double Ƒ=ç.CurrentStoredPower;double Ɛ=ç.MaxStoredPower;if(ŭ){if(ş){ſ="Forced Auto @ "+ƒ;ç.Enabled
=true;ç.ChargeMode=ChargeMode.Auto;}else if(ū){}else if(!Ū&&!ũ){ç.ChargeMode=ChargeMode.Auto;if(ň&&ç==ż[ż.Count-1]){ſ=
"Backup @ "+ƒ;ç.Enabled=true;}else{ſ="No Recharge";ç.Enabled=false;}}else if(Ū){ç.Enabled=true;if(ň&&ç==ż[ż.Count-1]){ſ="Backup @ "
+ƒ;ç.ChargeMode=ChargeMode.Auto;}else if(Ƒ>=Ɛ*0.999){ſ="Recharged";ç.ChargeMode=ChargeMode.Recharge;}else{ſ=
"Recharging @ "+ƒ;ç.ChargeMode=ChargeMode.Recharge;}}else if(ũ){ç.Enabled=true;if(Ƒ<=Ɛ*dischargeLevel){ſ="Low Charge!";ç.ChargeMode=
ChargeMode.Auto;}else{ſ="Discharging @ "+ƒ;ç.ChargeMode=ChargeMode.Discharge;}}}else{ç.Enabled=true;if(dischargeWhenUndocked&&Ķ!=
"lowCharge"){ſ="Discharging @ "+ƒ;ç.ChargeMode=ChargeMode.Discharge;}else{ſ="Auto @ "+ƒ;ç.ChargeMode=ChargeMode.Auto;}}if(
showBatteryStats&&enableTerminalStatistics){ƀ(ç,true,ſ,Ƒ,Ɛ);}}if(fillTanks){foreach(var å in Ż){double Ə=å.Capacity;double Ǝ=å.Capacity*
å.FilledRatio;if(ŭ){if(ū){}else if(!Ũ){ſ="No Refill";å.Enabled=false;}else if(!ŧ&&!å.BlockDefinition.SubtypeId.Contains(
"Hydrogen")){ſ="No O2 Refill";å.Enabled=false;}else if(!Ŧ&&å.BlockDefinition.SubtypeId.Contains("Hydrogen")){ſ="No H2 Refill";å.
Enabled=false;}else if(å.FilledRatio>=0.999){ſ="Refilled";å.Stockpile=true;}else{ſ="Refilling";å.Stockpile=true;}}else{ſ="";å.
Enabled=true;å.Stockpile=false;}if(showTankStats&&enableTerminalStatistics){ƀ(å,true,ſ,Ǝ,Ə);}}}if(enableEmergencyPower&&ų.Count
>0){double ƍ=lowChargePercentageON%100/100;double ƌ=lowChargePercentageOFF%100/100;double Ƌ=overloadPercentage%100/100;if
(Ķ=="lowCharge"||Ķ==""){if(activateOnLowBattery&&Ŏ<ō*ƍ){ļ=true;Ķ="lowCharge";ł="LOW CHARGE!";}else if(
activateOnLowBattery&&Ŏ>ō*ƌ){ļ=false;Ķ="";}}if(Ķ=="overload"||Ķ==""){if(activateOnOverload&&Ŋ>ŉ*Ƌ&&!ŭ){ļ=true;Ķ="overload";ł="OVERLOAD!";}
else{ļ=false;Ķ="";}}if(Ķ=="damage"||Ķ==""){if(activateOnDamagedBatteries&&ǀ!=null){ļ=true;Ķ="damage";ł=ǀ;}else{ļ=false;Ķ="";
}}if(Ķ=="noBat"||Ķ==""){if(activateOnNoBatteries&&ż.Count==0){ļ=true;Ķ="noBat";ł="NO BATTERIES!";}else{ļ=false;Ķ="";}}if(
Ŏ<ķ||(ļ&&Ļ&&ĺ)){Ļ=true;ĺ=true;}else{if(activateHydrogenEngineFirst&&ĸ>0){ĺ=true;Ļ=false;}else if(!
activateHydrogenEngineFirst&&Ĺ>0){ĺ=false;Ļ=true;}else{ĺ=true;Ļ=true;}}ķ=Ŏ;foreach(var æ in ų){if(ļ){if(Ļ&&æ.BlockDefinition.TypeIdString.Contains(
"Reactor")){æ.Enabled=true;ſ="Online";}else if(ĺ&&æ.BlockDefinition.TypeIdString.Contains("HydrogenEngine")){æ.Enabled=true;ſ=
"Online";}else{æ.Enabled=false;ſ="Standby";}}else{æ.Enabled=false;ſ="Standby";}if(showPowerDeviceStats&&enableTerminalStatistics
){ƀ(æ,true,ſ,æ.CurrentOutput,æ.MaxOutput);}}}}void Ɗ(string ƈ=""){if(!ŀ)return;if(events.Length==0){é(
"No events for triggering specified!");return;}if(timers.Length==0){é("No timers for triggering specified!");return;}if(events.Length!=timers.Length){é(
"Every event needs a timer block name!\nFound "+events.Length+" events and "+timers.Length+" timers.");return;}int Ƈ=-1;if(ƈ==""){for(int h=0;h<=events.Length-1;h++){
if(events[h]==ƈ){Ƈ=h;}}}for(int h=0;h<=events.Length-1;h++){string Ɔ=events[h].ToLower();Ɔ=Ɔ.Replace(" ","");if(ƈ==""){if(
Ɔ==Ŏ.č(ō)){ƈ=Ɔ;}if(ŭ&&Ɔ==ţ.ToString(@"m\:ss")){ƈ=Ɔ;}}if(Ɔ==ƈ){Ƈ=h;break;}}if(Ƈ>=0&&ƈ!=Ŀ){var ƅ=GridTerminalSystem.
GetBlockWithName(timers[Ƈ])as IMyTimerBlock;if(ƅ!=null){ƅ.Enabled=true;if(ƅ.TriggerDelay==1){ƅ.Trigger();}else{ƅ.StartCountdown();}}}Ŀ=ƈ
;}void Ƅ(){bool ƃ=true;if(ľ>Ľ*lightLevel){ƃ=false;}foreach(var Ƃ in ŷ){Ƃ.Enabled=ƃ;}foreach(var Ɓ in Ŷ){Ɓ.Enabled=ƃ;}}
void ƀ(IMyTerminalBlock Î,bool Ɖ=true,string ſ="",double Ɣ=0,double ơ=0){string Ɵ=Î.CustomName;string ƞ=System.Text.
RegularExpressions.Regex.Match(Î.CustomName,@" *\(\d+\.*\d*%.*\)").Value;if(ƞ!=String.Empty){Ɵ=Î.CustomName.Replace(ƞ,"");}if(Ɖ){Ɵ+=" ("+Ɣ
.č(ơ);if(ſ!=""){Ɵ+=", "+ſ;}Ɵ+=")";}if(Ɵ!=Î.CustomName){Î.CustomName=Ɵ;}}void Ɲ(){foreach(var Ɯ in Ÿ){Ɯ.CustomData="";ƀ(Ɯ,
false);}foreach(var ç in ż){ƀ(ç,false);}foreach(var å in Ż){ƀ(å,false);}foreach(var æ in ų){ƀ(æ,false);}}bool ƛ(){if(Me.
CubeGrid.IsStatic){return true;}else{var ƚ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ƚ,n=>n.
IsSameConstructAs(Me)&&n.CubeGrid.IsStatic);if(ƚ.Count>0){return true;}else{return false;}}}StringBuilder ƙ(IMyTextSurface d,bool Ó=true,
bool Ü=true,bool Û=true,bool Ú=true,bool Ù=true,bool Ø=true,bool Ö=true,bool Õ=true){bool Ƙ=false;StringBuilder Ò=new
StringBuilder();if(Ó){Ò.Append("Isy's Ship Refueler "+Ţ[š]+"\n");Ò.Append(d.Ĳ('=',d.ę(Ò))).Append("\n\n");}if(Ü&&Ð!=null){Ò.Append(
"Warning!\n"+Ð+"\n\n");Ƙ=true;}if(Û){if(ŭ){Ò.Append("Docked to: "+ť+"\n");Ò.Append("Docked for: "+ţ.ToString(@"hh\:mm\:ss")+"\n");}
else{Ò.Append("Undocked for: "+ţ.ToString(@"hh\:mm\:ss")+"\n");}Ƙ=true;}if(Ú&&ż.Count>0){if(ŭ&&ņ.TotalSeconds>1){Ò.Append(
"Batteries FULL in: "+ņ.ToString(@"hh\:mm\:ss")+"\n");}else if(ŭ){Ò.Append("Batteries are FULL!\n");}else if(Ŏ<=0.1){Ò.Append(
"Batteries are EMPTY!\n");}else{Ò.Append("Batteries EMPTY in: "+ņ.ToString(@"hh\:mm\:ss")+"\n");}Ƙ=true;}if(Ù&&ļ){if(Ƙ)Ò.Append("\n");Ò.Append(ł
+"\nEmergency power ACTIVATED!\n");Ƙ=true;}if(Ƙ)Ò.Append("\n");if(Ø&&ż.Count>0){Ò.Append("Stats for "+ż.Count+
" Batteries:\n");Ò.Append(ä(d,"Input",Ō,ŋ,Ō.Ĉ(),ŋ.Ĉ()));Ò.Append(ä(d,"Output",Ŋ,ŉ,Ŋ.Ĉ(),ŉ.Ĉ()));Ò.Append(ä(d,"Charge",Ŏ,ō,Ŏ.Ĉ(true),ō.Ĉ
(true)));Ò.Append("\n");Ƙ=true;}if(Ö&&Ż.Count>0){Ò.Append("Stats for "+Ż.Count+" Tanks:\n");if(ź.Count>0){Ò.Append(ä(d,ź.
Count+"x Oxygen",Ő,ń,Ő.Ą(),ń.Ą()));}if(Ź.Count>0){Ò.Append(ä(d,Ź.Count+"x Hydrogen",Ł,Ń,Ł.Ą(),Ń.Ą()));}Ò.Append("\n");Ƙ=true;
}if(Õ&&lightControl&&Ÿ.Count>0){string Ɨ="< ";string Ɩ="ON";if(ľ>Ľ*lightLevel){Ɨ="> ";Ɩ="OFF";}Ò.Append(ä(d,"Light Level"
,ľ,Ľ,Ɨ+(lightLevel*100)+"%",Ɩ));Ƙ=true;}if(!Ƙ){Ò.Append("-- No informations to show --");}return Ò;}void ƕ(){if(Ų.Count==
0)return;foreach(var Î in Ų){var Í=Î.ý(mainLCDKeyword);foreach(var Ì in Í){var Ë=Ì.Key;var Ê=Ì.Value;bool Ó=Ê.ô(
"showHeading");bool Ü=Ê.ô("showWarnings");bool Û=Ê.ô("showConnectionStatus");bool Ú=Ê.ô("showPowerTime");bool Ù=Ê.ô(
"showEmergencyPower");bool Ø=Ê.ô("showBatteryStats");bool Ö=Ê.ô("showTankStats");bool Õ=Ê.ô("showLightStats");bool É=Ê.ô(
"scrollTextIfNeeded");StringBuilder Ò=new StringBuilder();Ò=ƙ(Ë,Ó,Ü,Û,Ú,Ù,Ø,Ö,Õ);Ò=Ë.Ĭ(Ò,Ó?3:0,É);Ë.WriteText(Ò);}}}void Ô(){if(ű.Count==0)
return;foreach(var Î in ű){var Í=Î.ý(warningsLCDKeyword);foreach(var Ì in Í){var Ë=Ì.Key;var Ê=Ì.Value;bool Ó=Ê.ô(
"showHeading");bool É=Ê.ô("scrollTextIfNeeded");StringBuilder Ò=new StringBuilder();if(Ó){Ò.Append("Isy's Ship Refuler Warnings\n");Ò
.Append(Ë.Ĳ('=',Ë.ę(Ò))).Append("\n\n");}if(ř.Count==0){Ò.Append("- No problems detected -");}else{int Ñ=1;foreach(var Ð
in ř){Ò.Append(Ñ+". "+Ð.Replace("\n"," ")+"\n");Ñ++;}}Ò=Ë.Ĭ(Ò,Ó?3:0,É);Ë.WriteText(Ò);}}}void Ï(){if(Ű.Count==0)return;
foreach(var Î in Ű){var Í=Î.ý(performanceLCDKeyword);foreach(var Ì in Í){var Ë=Ì.Key;var Ê=Ì.Value;bool Ó=Ê.ô("showHeading");
bool É=Ê.ô("scrollTextIfNeeded");StringBuilder Ò=new StringBuilder();if(Ó){Ò.Append("Isy's Ship Refueler Performance\n");Ò.
Append(Ë.Ĳ('=',Ë.ę(Ò))).Append("\n\n");}Ò.Append(N);Ò=Ë.Ĭ(Ò,Ó?3:0,É);Ë.WriteText(Ò);}}}void ð(){Echo("Isy's Ship Refueler "+Ţ[
š]+"\n====================\n");if(Ð!=null){Echo("Warning!\n"+Ð+"\n");}StringBuilder Ò=new StringBuilder();Ò.Append(
"Ship is "+(ŭ?"docked for ":"undocked for ")+ţ.ToString(@"hh\:mm\:ss")+"\n\n");Ò.Append("Task: "+œ[ŕ]+"\n");Ò.Append(
"Script step: "+ŕ+" / "+(œ.Length-1)+"\n\n");Ò.Append(N);N=Ò;Echo(Ò.ToString());if(Ų.Count==0){Echo(
"Hint:\nBuild a LCD and add the main LCD\nkeyword '"+mainLCDKeyword+"' to its name to get\nmore informations about your ship\nand the current script actions.\n");}}double î
(string ì){ë();var ê=Storage.Split('\n');for(int h=0;h<ê.Length;h++){if(ê[h].StartsWith(ì)){return Convert.ToDouble(ê[h].
Replace(ì+"=",""));}}return 0;}void í(string ì,double Ý){ë();var ê=Storage.Split('\n');for(int h=0;h<ê.Length;h++){if(ê[h].
StartsWith(ì)){ê[h]=ì+"="+Ý;break;}}Storage=String.Join("\n",ê);}void ë(){var ê=Storage.Split('\n');if(ê.Length!=Ş.Length){Storage
=String.Join("\n",Ş);}}void é(string è){ř.Add(è);Ř.Add(è);Ð=ř.ElementAt(0);}void Save(){foreach(var ç in ż){ç.Enabled=
true;ç.ChargeMode=ChargeMode.Auto;}foreach(var æ in ų){æ.Enabled=true;}foreach(var å in Ż){å.Stockpile=false;}Ɲ();}
StringBuilder ä(IMyTextSurface d,string ã,double Ý,double â,string á=null,string à=null,bool ß=false,bool Þ=false,string L=""){string
È=Ý.ToString();string e=â.ToString();if(á!=null){È=á;}if(à!=null){e=à;}float A=d.FontSize;float Z=0.61f;float Y=1.01f;if(
d.Font=="Monospace"){Z=0.41f;Y=0.81f;}float X=d.ĳ();char W=' ';float V=d.ė(W);StringBuilder U=new StringBuilder(" "+Ý.č(â
));U=d.Ĳ(W,d.ę("9999.9%")-d.ę(U)).Append(U);StringBuilder T=new StringBuilder(È+" / "+e);StringBuilder S=new
StringBuilder();StringBuilder R=new StringBuilder();StringBuilder Q;if(â==0){S.Append(L+ã+" ");Q=d.Ĳ(W,X-d.ę(S)-d.ę(È));S.Append(Q).
Append(È);return S.Append("\n");}double M=0;if(â>0)M=Ý/â>=1?1:Ý/â;if(Þ&&!ß){if(A<Z||(A<Y&&X>512)){S.Append(P(d,X*0.25f,M,L)+
" "+ã+" ");Q=d.Ĳ(W,X*0.75-d.ę(S)-d.ę(È+" /"));S.Append(Q).Append(T);Q=d.Ĳ(W,X-d.ę(S)-d.ę(U));S.Append(Q);S.Append(U);}else{
S.Append(P(d,X*0.3f,M,L)+" "+ã+" ");Q=d.Ĳ(W,X-d.ę(S)-d.ę(U));S.Append(Q);S.Append(U);}}else{S.Append(L+ã+" ");if(A<Z||(A<
Y&&X>512)){Q=d.Ĳ(W,X*0.5-d.ę(S)-d.ę(È+" /"));S.Append(Q).Append(T);Q=d.Ĳ(W,X-d.ę(S)-d.ę(U));S.Append(Q).Append(U);if(!ß){
R=P(d,X,M,L).Append("\n");}}else{Q=d.Ĳ(W,X-d.ę(S)-d.ę(T));S.Append(Q).Append(T);if(!ß){R=P(d,X-d.ę(U),M,L);R.Append(U).
Append("\n");}}}return S.Append("\n").Append(R);}StringBuilder P(IMyTextSurface d,float O,double M,string L){StringBuilder K,J
;char I='[';char H=']';char G='I';char F='∙';float E=d.ė(I);float D=d.ė(H);float C=0;if(L!="")C=d.ę(L);float B=O-E-D-C;K=
d.Ĳ(G,B*M);J=d.Ĳ(F,B-d.ę(K));return new StringBuilder().Append(L).Append(I).Append(K).Append(J).Append(H);}StringBuilder
N=new StringBuilder("No performance Information available!");Dictionary<string,int>f=new Dictionary<string,int>();List<
int>w=new List<int>(new int[600]);List<double>Ç=new List<double>(new double[600]);double Å,Ä,Ã,Â,Á;int À,º=0;void µ(string
ª){º=º>=599?0:º+1;À=Runtime.CurrentInstructionCount;if(À>Ä)Ä=À;w[º]=À;Â=w.Sum()/w.Count;N.Clear();N.Append(
"Instructions: "+À+" / "+Runtime.MaxInstructionCount+"\n");N.Append("Max. Instructions: "+Ä+" / "+Runtime.MaxInstructionCount+"\n");N.
Append("Avg. Instructions: "+Math.Floor(Â)+" / "+Runtime.MaxInstructionCount+"\n\n");Å=Runtime.LastRunTimeMs;if(Å>Ã&&f.
ContainsKey(ª))Ã=Å;Ç[º]=Å;Á=Ç.Sum()/Ç.Count;N.Append("Last runtime: "+Math.Round(Å,4)+" ms\n");N.Append("Max. runtime: "+Math.Round
(Ã,4)+" ms\n");N.Append("Avg. runtime: "+Math.Round(Á,4)+" ms\n\n");N.Append("Instructions per Method:\n");f[ª]=À;foreach
(var z in f.OrderByDescending(h=>h.Value)){N.Append("- "+z.Key+": "+z.Value+"\n");}N.Append("\n");}List<IMyTerminalBlock>
y(string Æ,string[]x=null,string v="Debug",float u=0.6f,float r=2f){string q="[IsyLCD]";var p=new List<IMyTerminalBlock>(
);GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(p,n=>n.IsSameConstructAs(Me)&&(n.CustomName.Contains(Æ)||(n.
CustomName.Contains(q)&&n.CustomData.Contains(Æ))));var o=p.FindAll(n=>n.CustomName.Contains(Æ));foreach(var d in o){d.CustomName=
d.CustomName.Replace(Æ,"").Replace(" "+Æ,"").TrimEnd(' ');bool m=false;bool k=false;int j=0;if(d is IMyTextSurface){if(!d
.CustomName.Contains(q))m=true;if(!d.CustomData.Contains(Æ)){k=true;d.CustomData="@0 "+Æ+(x!=null?"\n"+String.Join("\n",x
):"");}}else if(d is IMyTextSurfaceProvider){if(!d.CustomName.Contains(q))m=true;int ï=(d as IMyTextSurfaceProvider).
SurfaceCount;for(int h=0;h<ï;h++){if(!d.CustomData.Contains("@"+h)){k=true;j=h;d.CustomData+=(d.CustomData==""?"":"\n\n")+"@"+h+" "+
Æ+(x!=null?"\n"+String.Join("\n",x):"");break;}}}else{p.Remove(d);}if(m)d.CustomName+=" "+q;if(k){var Ë=(d as
IMyTextSurfaceProvider).GetSurface(j);Ë.Font=v;Ë.FontSize=u;Ë.TextPadding=r;Ë.Alignment=TextAlignment.LEFT;Ë.ContentType=ContentType.
TEXT_AND_IMAGE;}}return p;}
}public static partial class Ć{private static Dictionary<char,float>Ġ=new Dictionary<char,float>();public static void ğ(
string Ğ,float ĝ){foreach(char Ĝ in Ğ){Ġ[Ĝ]=ĝ;}}public static void ě(){if(Ġ.Count>0)return;ğ(
"3FKTabdeghknopqsuy£µÝàáâãäåèéêëðñòóôõöøùúûüýþÿāăąďđēĕėęěĝğġģĥħĶķńņňŉōŏőśŝşšŢŤŦũūŭůűųŶŷŸșȚЎЗКЛбдекруцяёђћўџ",18);ğ("ABDNOQRSÀÁÂÃÄÅÐÑÒÓÔÕÖØĂĄĎĐŃŅŇŌŎŐŔŖŘŚŜŞŠȘЅЊЖф□",22);ğ("#0245689CXZ¤¥ÇßĆĈĊČŹŻŽƒЁЌАБВДИЙПРСТУХЬ€",20);ğ(
"￥$&GHPUVY§ÙÚÛÜÞĀĜĞĠĢĤĦŨŪŬŮŰŲОФЦЪЯжы†‡",21);ğ("！ !I`ijl ¡¨¯´¸ÌÍÎÏìíîïĨĩĪīĮįİıĵĺļľłˆˇ˘˙˚˛˜˝ІЇії‹›∙",9);ğ("？7?Jcz¢¿çćĉċčĴźżžЃЈЧавийнопсъьѓѕќ",17);ğ(
"（）：《》，。、；【】(),.1:;[]ft{}·ţťŧț",10);ğ("+<=>E^~¬±¶ÈÉÊË×÷ĒĔĖĘĚЄЏЕНЭ−",19);ğ("L_vx«»ĹĻĽĿŁГгзлхчҐ–•",16);ğ("\"-rª­ºŀŕŗř",11);ğ("WÆŒŴ—…‰",32);ğ("'|¦ˉ‘’‚",7)
;ğ("@©®мшњ",26);ğ("mw¼ŵЮщ",28);ğ("/ĳтэє",15);ğ("\\°“”„",13);ğ("*²³¹",12);ğ("¾æœЉ",29);ğ("%ĲЫ",25);ğ("MМШ",27);ğ("½Щ",30);
ğ("ю",24);ğ("ј",8);ğ("љ",23);ğ("ґ",14);ğ("™",31);}public static Vector2 Ě(this IMyTextSurface Ë,StringBuilder è){ě();
Vector2 O=new Vector2();if(Ë.Font=="Monospace"){float A=Ë.FontSize;O.X=(float)(è.Length*19.4*A);O.Y=(float)(28.8*A);return O;}
else{float A=(float)(Ë.FontSize*0.779);foreach(char Ĝ in è.ToString()){try{O.X+=Ġ[Ĝ]*A;}catch{}}O.Y=(float)(28.8*Ë.FontSize)
;return O;}}public static float ę(this IMyTextSurface d,StringBuilder è){Vector2 Ę=d.Ě(è);return Ę.X;}public static float
ę(this IMyTextSurface d,string è){Vector2 Ę=d.Ě(new StringBuilder(è));return Ę.X;}public static float ė(this
IMyTextSurface d,char Ė){float ĕ=ę(d,new string(Ė,1));return ĕ;}public static int Ĕ(this IMyTextSurface d){Vector2 ē=d.SurfaceSize;
float ġ=d.TextureSize.Y;if(ē.X<512||ġ!=ē.Y)ē.Y*=512/ġ;float Ĵ=ē.Y*(100-d.TextPadding*2)/100;Vector2 Ę=d.Ě(new StringBuilder(
"T"));return(int)(Ĵ/Ę.Y);}public static float ĳ(this IMyTextSurface d){Vector2 ē=d.SurfaceSize;float ġ=d.TextureSize.Y;if(ē
.X<512||ġ!=ē.Y)ē.X*=512/ġ;return ē.X*(100-d.TextPadding*2)/100;}public static StringBuilder Ĳ(this IMyTextSurface d,char
ı,double İ){int į=(int)(İ/ė(d,ı));if(į<0)į=0;return new StringBuilder().Append(ı,į);}private static DateTime Į=DateTime.
Now;private static Dictionary<int,List<int>>ĭ=new Dictionary<int,List<int>>();public static StringBuilder Ĭ(this
IMyTextSurface d,StringBuilder è,int ī=3,bool É=true,int Ī=0){int ĩ=d.GetHashCode();if(!ĭ.ContainsKey(ĩ)){ĭ[ĩ]=new List<int>{1,3,ī,0};
}int Ĩ=ĭ[ĩ][0];int ħ=ĭ[ĩ][1];int Ħ=ĭ[ĩ][2];int ĥ=ĭ[ĩ][3];var Ĥ=è.ToString().TrimEnd('\n').Split('\n');List<string>ģ=new
List<string>();if(Ī==0)Ī=d.Ĕ();float X=d.ĳ();StringBuilder L,Ģ=new StringBuilder();for(int h=0;h<Ĥ.Length;h++){if(h<ī||h<Ħ||
ģ.Count-Ħ>Ī||d.ę(Ĥ[h])<=X){ģ.Add(Ĥ[h]);}else{try{Ģ.Clear();float Ē,đ;var ā=Ĥ[h].Split(' ');string Ā=System.Text.
RegularExpressions.Regex.Match(Ĥ[h],@"\d+(\.|\:)\ ").Value;L=d.Ĳ(' ',d.ę(Ā));foreach(var ÿ in ā){Ē=d.ę(Ģ);đ=d.ę(ÿ);if(Ē+đ>X){ģ.Add(Ģ.
ToString());Ģ=new StringBuilder(L+ÿ+" ");}else{Ģ.Append(ÿ+" ");}}ģ.Add(Ģ.ToString());}catch{ģ.Add(Ĥ[h]);}}}if(É){if(ģ.Count>Ī){
if(DateTime.Now.Second!=ĥ){ĥ=DateTime.Now.Second;if(ħ>0)ħ--;if(ħ<=0)Ħ+=Ĩ;if(Ħ+Ī-ī>=ģ.Count&&ħ<=0){Ĩ=-1;ħ=3;}if(Ħ<=ī&&ħ<=0)
{Ĩ=1;ħ=3;}}}else{Ħ=ī;Ĩ=1;ħ=3;}ĭ[ĩ][0]=Ĩ;ĭ[ĩ][1]=ħ;ĭ[ĩ][2]=Ħ;ĭ[ĩ][3]=ĥ;}else{Ħ=ī;}StringBuilder þ=new StringBuilder();for(
var ó=0;ó<ī;ó++){þ.Append(ģ[ó]+"\n");}for(var ó=Ħ;ó<ģ.Count;ó++){þ.Append(ģ[ó]+"\n");}return þ;}public static Dictionary<
IMyTextSurface,string>ý(this IMyTerminalBlock Î,string Æ,Dictionary<string,string>ü=null){var û=new Dictionary<IMyTextSurface,string>(
);if(Î is IMyTextSurface){û[Î as IMyTextSurface]=Î.CustomData;}else if(Î is IMyTextSurfaceProvider){var ú=System.Text.
RegularExpressions.Regex.Matches(Î.CustomData,@"@(\d).*("+Æ+@")");int Ă=(Î as IMyTextSurfaceProvider).SurfaceCount;foreach(System.Text.
RegularExpressions.Match ù in ú){int ø=-1;if(int.TryParse(ù.Groups[1].Value,out ø)){if(ø>=Ă)continue;string ñ=Î.CustomData;int ö=ñ.IndexOf
("@"+ø);int õ=ñ.IndexOf("@",ö+1)-ö;string Ê=õ<=0?ñ.Substring(ö):ñ.Substring(ö,õ);û[(Î as IMyTextSurfaceProvider).
GetSurface(ø)]=Ê;}}}return û;}public static bool ô(this string Ê,string ì){var ñ=Ê.Replace(" ","").Split('\n');foreach(var ó in ñ)
{if(ó.StartsWith(ì+"=")){try{return Convert.ToBoolean(ó.Replace(ì+"=",""));}catch{return true;}}}return true;}public
static string ò(this string Ê,string ì){var ñ=Ê.Replace(" ","").Split('\n');foreach(var ó in ñ){if(ó.StartsWith(ì+"=")){return
ó.Replace(ì+"=","");}}return"";}}public static partial class Ć{public static string Đ(this char ď,int Ď){if(Ď<=0){return
"";}return new string(ď,Ď);}}public static partial class Ć{public static string č(this double Č,double ċ){double Ċ=Math.
Round(Č/ċ*100,1);if(ċ==0){return"0%";}else{return Ċ+"%";}}public static string č(this float Č,float ċ){double Ċ=Math.Round(Č/
ċ*100,1);if(ċ==0){return"0%";}else{return Ċ+"%";}}}public static partial class Ć{public static string Ĉ(this float Ý,bool
ć=false){string ą="MW";string ĉ=Ý<0?"-":"";Ý=Math.Abs(Ý);if(Ý<1){Ý*=1000;ą="kW";}else if(Ý>=1000&&Ý<1000000){Ý/=1000;ą=
"GW";}else if(Ý>=1000000&&Ý<1000000000){Ý/=1000000;ą="TW";}else if(Ý>=1000000000){Ý/=1000000000;ą="PW";}if(ć)ą+="h";return ĉ
+Math.Round(Ý,1)+" "+ą;}public static string Ĉ(this double Ý,bool ć=false){float ă=(float)Ý;return ă.Ĉ(ć);}}public static
partial class Ć{public static string Ą(this float Ý){string ą="L";if(Ý>=1000&&Ý<1000000){Ý/=1000;ą="KL";}else if(Ý>=1000000&&Ý<
1000000000){Ý/=1000000;ą="ML";}else if(Ý>=1000000000&&Ý<1000000000000){Ý/=1000000000;ą="BL";}else if(Ý>=1000000000000){Ý/=
1000000000000;ą="TL";}return Math.Round(Ý,1)+" "+ą;}public static string Ą(this double Ý){float ă=(float)Ý;return ă.Ą();}