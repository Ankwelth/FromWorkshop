/*
* Wico craft controller Master Control Script
*
* Control Script for Rovers and Drones and Oribtal craft
* 
* Uncompressed source for this script here: https://github.com/Wicorel/SpaceEngineers/tree/master/MDK%20Craft%20Control
 * 
 * 
 * Handles:
* Master timer for sub-modules
* Calculates ship speed and vectors (obsolete)
* Calculates simspeed (obsolete)
* Configure craft_operation settings
* making sure antenna doesn't get turned off (bug in SE turn off antenna when trying to remotely connect to grid)
* 
* Calculates cargo and power percentages and cargo multiplier and hydro fill and oxy tank fill
 * 
 * Detects grid changes and initiates re-init
 * 
* * 
* MODE_IDLE
* MODE_ATTENTION
* 
* Commands:
* 
* setsimspeed <value>: sets the current simspeed so the calculations can be accurate. (obsolete)
* init: re-init all blocks
* idle : force MODE_IDLE
* coast: turns on/off backward thrusters
* masterreset: attempts to do a master reset of all saved information
* setvaluef <blockname>:<property>:<value>  -> sets specified block's property to specified value
* Example:
*  setvaluef Advanced Rotor:UpperLimit:-24
* genpatrol [distance [up]]
* Examples:
* genpatrol
* genpatrol 300 150
* genpatrol 500
*
* Need:

* Want:
* 
* menu management for commands (including sub-modules)
* 
* minimize serialized data and make sub-modules pass their own seperately, OR support extra data in state
* 
* common function for 'handle this' combining 'me' grid and an exclusion name check
*
* multi-script handling for modes
* 
* * advanced trigger: only when module handles that mode... (so need mode->module dictionary)
* override base modes?
*
*
*
* WANT:
* setvalueb
* Actions
* Trigger timers on 'events'.
* set antenna name to match mode?
* *
* 2.0 Removed many built-in functions to make script room. These functions were duplicated in sub-modules anyway.
* 2.0.1
* 0.2 Remove items from serialize that main control no longer calculates (cargo, battery, etc).
* if simspeed>1.01, assume 1.0 and recalculate.
* 0.3 re-org code sections
* Pass arguments to sub-modules 
* 0.4 (re)integrate power and cargo
* 0.4a process multiple arguments on a command line
* 0.4b check mass change and request reinit including sub-modules.
* 
* 2.1 Code Reorg
* Cache all blocks and grids.  Support for multi-grid constructions.
* !Needs handling for grids connected via connectors..
* 
* .1a Don't force re-init on working projector.
* .1b Add 'brake' command
* Add braking for sleds (added wheelinit)
* 
* 2.2 PB changes in 1.172
* 
* .2a Added modes. Default PB name
* 
* 2.3 Start to add Power information
* 
* .3a Add drills and ejectors to reset motion. Add welders, drills, connectors and grinders to cargo check.
* don't set PB name because it erases settings.. :(
* 
* .3b getblocks fixes when called before gridsinit
* 
* 3.0 remove older items from serialize that are no longer needed
* removed NAV support
* fixed battery maxoutput values
* 
* 3.0a support no remote control blocks. Check for Cryo when getting default controller.
* 3.0b sBanner
* 3.0c caching optimizations
* 3.0d fix connectorsanyconnectors not using localdock
* 3.0e Add Master Reset command
* 3.0f 
* check for grid changes and re-init 
* rotor NOFOLLOW
* ignore projectors with !WCC in name or customdata
* ignore 'cutter' thrusters
* 
* 3.0g Fix problem with allBlockCount being loaded after it has changed
* 
* 3.0H 
* fix problems with docking/undocking and perm re-init
* 
* 05/13: fix GetBlocksContains<T>()
* 
* 3.0I MDK Version 08/20/2017   MDK: https://github.com/malware-dev/MDK-SE/
* Uncompressed source for this script here: https://github.com/Wicorel/SpaceEngineers/tree/master/MDK%20Craft%20Control
* 
* 3.0J Add moduleDoPreModes() to Main()
* Move pre-mode to moduleDoPreModes()
* add clearing of gpsPanel to moduleDoPreModes()
* 
* 3.0K more init states if larger number of blocks in grid system.
* 
* 3.0K2  search order for text panels
* 
* 3.1 Verison for SE 1.185 PB Major changes
* 
* 3.1A init cycle optimizations
* 
* 3.1B Handle no controller (stations, etc)
* 12092017
* 
* 3.1C 12132017
* don't count ejectors in cargo%
* fix bug in DoTriggerMain() causing updates to stop
* 
* 3.1D Section processing for save information (text panels)
* fix bug in serialize wrting z,y z, instead of x,y,z (oops)
* 
* 3.2 INI WCCM 01062018
*
* 3.2A
* FilledRatio Change
* 
* 3.2B Lots of INI processing
* 
* 3.3 Handle multiple output panels.
* Only write to panels at end
* 
* 3.3A Redo Serlialize.
* Module Serlialize
* 
* 3.4 
* add namecameras
* 
* 3.4a
* init optimizations for text panels
* 
* 3.4B turn off auto-pirate mode.
* 
* 3.4C options for timer names
* options for debugupdate
* options for submodule trigger rate
* 
* 3.4D Mar 29 2018 Current Source
* removed ModeScans/MODE_DOSCAN
* 
* 3.4E Mar 22, 2018
* Error messages on missing blocks on startup
* Re-try startup if there are errors.
* 
* 3.4F 
* handle stations having no propulsion methods
* increase default sub-module trigger to 5seconds
* May 27, 2018
* 
* 3.4G June 08,2018
* Add setmode and setstate commands
* Clear all panels on masterreset command
* 
* 3.4H June 19,2018
* Add genpatrol command to generate a set of patrol waypoints around this ship.
* Defaults for distance are 500 and up  is 500
* 
* 3.4I
* July 23 SE 1.187 MDK 1.1.16
* 
* 3.4J Sep 08 2018
* MDK Update
* Performance Pass
* 
* 
* genpatrol [distance [up]]
* genpatrol
* genpatrol 300 150
* genpatrol 500
* 
* 3.4J
* 
* 3.5 SE V1.189
* 
* 3.7 05292019 SE 1.190
* Current source (no antenna send)
* 
* 3.7a 11242019 
* Current source
* 
* 3.8 12222019
* Old IGC removal in SE 1.193.100.  Remove references to old IGC.
* 
*/
string ʴ="Wico Craft";string ʳ="Master";string ʲ="3.8";const string ʱ="0.00";double ʰ=2;double ʯ=-1;double ʮ=5;double ʭ=
-1;void ʬ(){ɻ("clear",Ȱ);ɻ("clear",Ȯ);if(j){Echo("Startup Error Detected"+k);ɻ("Startup Error Detected"+k,Ȱ);}else if(k!=
"")Echo(k);string q="";if(ɗ>0){q+="Ship\n";}else if(ɗ<0){}else{q+="Station\n";}if(º()){if(Ê())q+="Connected";else q+=
"Not Connected";if(µ())q+="\nLocked";else q+=" : Not Locked";}if(q!=""){Echo(q);ʐ(q);}q="";if(h)Echo("FAST!");if(ʯ>ʰ){ʯ=0;Σ();}else{if(
ʯ<0){ʯ=ʰ+5;}ʯ+=Runtime.TimeSinceLastRun.TotalSeconds;}if(Κ>=0)Echo("Cargo="+Κ.ToString()+"%");if(ʭ>ʮ){ʭ=0;ʛ(0,false);}
else{if(ʭ<0){ʭ=ʮ+5;}ʭ+=Runtime.TimeSinceLastRun.TotalSeconds;}if(ˀ.Count>0&&ʽ>0){q+=" : "+(ʞ()/ʽ*100).ToString("0.00")+"%";q
+="\n Storage="+ʼ.ToString()+"%";}if(q!="")Echo(q);q="";float ʫ=0;θ(out ʫ);if(ψ.Count>0){q="Reactors: #"+ψ.Count.ToString(
);q+=" - "+ω.ToString("0.00")+"MW\n";float ʪ=(float)(ʫ/ƕ*100);q+=" Curr Output="+ʫ.ToString("0.00")+"MW"+" : "+ʪ.ToString
("0.00")+"%";}if(q!="")Echo(q);q="";Ⱦ();if(ȿ>=0){Echo("O:"+ȿ.ToString("000.0%"));}if(ɀ>=0){Echo("H:"+ɀ.ToString("000.0%")
);}if(Ͱ.Count>0){Echo(Ͱ.Count+" Gas Gens");}if(U>=0){Echo("Grav="+U.ToString(ʱ));ʐ("Planet Gravity "+U.ToString(ʱ)+" g");
ʐ(ʊ((int)(U/1.1*100)));}else ʐ("ERROR: No Remote Control found!");ɻ("clear",Ȯ);}void ʩ(){ù();}void ʨ(bool ʧ=false){ĕ(ϴ);ȕ
();ї(ъ);ї(ѕ);if(ǌ is IMyRemoteControl)((IMyRemoteControl)ǌ).SetAutoPilotEnabled(false);if(ǌ is IMyShipController)((
IMyShipController)ǌ).DampenersOverride=true;if(!ʧ)ò();ơ(0);}void ʵ(){ɵ();Ͳ();ʨ();Φ=0;ε="";γ="init";Ϊ.Ƿ("");ΰ();h=true;}void ʦ(){Χ=ˑ;{int
ʶ=0;if(Me.CustomName.ToLower().Contains("nad"))Χ|=ˉ;if(Me.CustomName.ToLower().Contains("rotor"))Χ|=ˏ;else if(Me.
CustomName.ToLower().Contains("sled"))Χ|=ː;if(Ϩ>0){ʶ++;}if(ϯ>0){ʶ++;}if(ϸ>0){ʶ++;}if(ъ!=null&&ъ.Count>0&&ѕ.Count>0)Χ|=ˏ;if(Ɛ!=null
&&Ɛ.Count>0&&ʶ>0)Χ|=ː;if(ǃ!=null&&ǃ.Count>0&&!Me.CustomName.ToLower().Contains("nogyros"))Χ|=ˍ;if(ʶ>1||Me.CustomName.
ToLower().Contains("orbital"))Χ|=ˌ;if((Ƒ!=null&&Ƒ.Count>0&&!((Χ&ː)>0))||Me.CustomName.ToLower().Contains("wheel"))Χ|=ˎ;if(Me.
CustomName.ToLower().Contains("rocket"))Χ|=ˋ;if(Me.CustomName.ToLower().Contains("pet"))Χ|=ˊ;if(Me.CustomName.ToLower().Contains(
"noautogyro"))Χ|=ˈ;if(Me.CustomName.ToLower().Contains("nopower"))Χ|=ˤ;if(Me.CustomName.ToLower().Contains("notank"))Χ|=ˇ;}}void ˁ()
{string q="";q+=V.ToString(ʱ)+" m/s";q+=" ("+(V*3.6).ToString(ʱ)+"km/h)";ʐ(q);}void ʿ(Ǫ F){}void ʾ(Ǫ F){}double ʽ=-1;int
ʼ=-1;List<IMyTerminalBlock>ˀ=new List<IMyTerminalBlock>();bool ʻ(IMyTerminalBlock ß){if(ß is IMyBatteryBlock){
IMyBatteryBlock ʷ=ß as IMyBatteryBlock;return(ʷ.ChargeMode==ChargeMode.Recharge);}else return false;}bool ʹ(IMyTerminalBlock ß){if(ß is
IMyBatteryBlock){IMyBatteryBlock ʷ=ß as IMyBatteryBlock;return(ʷ.ChargeMode==ChargeMode.Discharge);}else return false;}bool ʸ(
IMyTerminalBlock ß){if(ß is IMyBatteryBlock){IMyBatteryBlock ʷ=ß as IMyBatteryBlock;return ʷ.IsCharging;}else return false;}void ʺ(){ˀ.
Clear();ʼ=-1;ʽ=-1;GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(ˀ,Ǒ);if(ˀ.Count>0)ʽ=0;foreach(var ʝ in ˀ){
IMyBatteryBlock ʜ=ʝ as IMyBatteryBlock;ʽ+=ʜ.MaxOutput;}}double ʞ(){double q=0;foreach(var ʝ in ˀ){IMyBatteryBlock ʜ=ʝ as
IMyBatteryBlock;q+=ʜ.CurrentOutput;}return q;}bool ʛ(int ʚ,bool ʙ=true,bool ʘ=false){float ʗ=0;float ʖ=0;bool ʕ=false;float ʔ;if(ˀ.
Count<1)ʺ();if(ˀ.Count<1)return false;ʼ=0;for(int ʓ=0;ʓ<ˀ.Count;ʓ++){float ʒ=0;float ʑ=0;int ʟ=100;IMyBatteryBlock ñ;ñ=ˀ[ʓ]as
IMyBatteryBlock;ʔ=ñ.MaxStoredPower;ʑ+=ʔ;ʗ+=ʔ;ʔ=ñ.CurrentStoredPower;ʒ+=ʔ;ʖ+=ʔ;if(ʑ>0){ʔ=((ʒ*100)/ʑ);ʔ=(float)Math.Round(ʔ,0);ʟ=(int)ʔ;}
string Û;Û="";if(ʻ(ˀ[ʓ]))Û+="R";else if(ʹ(ˀ[ʓ]))Û+="D";else Û+="a";float ī;ī=ñ.CurrentInput;if(ī>0)Û+="+";else Û+=" ";ī=ñ.
CurrentOutput;if(ī>0)Û+="-";else Û+=" ";Û+=ʟ+"%";Û+=":"+ˀ[ʓ].CustomName;if(ʙ)Echo(Û);if(ʻ(ˀ[ʓ])&&ʚ>0){if(ʟ<ʚ)ʕ=true;else if(ʟ>99){ñ.
ChargeMode=ChargeMode.Recharge;}}if(!(ñ.ChargeMode==ChargeMode.Recharge)&&ʟ<ʚ&&!ʕ){ñ.ChargeMode=ChargeMode.Recharge;ʕ=true;}}if(ʗ>
0){ʔ=((ʖ*100)/ʗ);ʔ=(float)Math.Round(ʔ,0);ʼ=(int)ʔ;}else ʼ=-1;return ʕ;}void ʥ(){for(int w=0;w<ˀ.Count;w++){
IMyBatteryBlock ñ;ñ=ˀ[w]as IMyBatteryBlock;ñ.ChargeMode=ChargeMode.Auto;}}void ʤ(bool ʙ=false,bool ʣ=true){if(ʙ)Echo(ˀ.Count+
" Batteries");string Û;for(int w=0;w<ˀ.Count;w++){IMyBatteryBlock ñ;ñ=ˀ[w]as IMyBatteryBlock;if(ʣ){ñ.ChargeMode=ChargeMode.Discharge
;}else ñ.ChargeMode=ChargeMode.Recharge;Û=ñ.CustomName+": ";if(ñ.ChargeMode==ChargeMode.Recharge){Û+="RECHARGE/";}else Û
+="NOTRECHARGE/";if(ñ.ChargeMode==ChargeMode.Discharge){Û+="DISCHARGE";}else{Û+="NOTDISCHARGE";}if(ʙ)Echo(Û);}}void ʢ(List
<IMyTerminalBlock>ũ,bool Ï=true){foreach(var ñ in ũ){IMyFunctionalBlock ʠ=ñ as IMyFunctionalBlock;if(ʠ==null)continue;ʠ.
Enabled=Ï;}}void ʡ(List<IMyTerminalBlock>ũ){foreach(var ñ in ũ){IMyFunctionalBlock ʠ=ñ as IMyFunctionalBlock;if(ʠ==null)
continue;ʠ.Enabled=!ʠ.Enabled;}}int ˆ=5;int Κ=-1;double Θ=-1;string Η="CARGO";void Ζ(Ǫ F){F.Ų(Η,"cargopctmin",ref ˆ,true);}List<
IMyTerminalBlock>Ε=null;bool Δ=false;double Γ=0.0;void Ι(){var ũ=new List<IMyTerminalBlock>();if(Ε==null)Ε=new List<IMyTerminalBlock>();
else Ε.Clear();ǖ<IMyCargoContainer>(ref ũ);Ε.AddRange(ũ);Κ=-1;Θ=-1;}void Α(){var ũ=new List<IMyTerminalBlock>();ǖ<
IMyShipConnector>(ref ũ);foreach(var Ž in ũ){if(Ž.CustomName.Contains("Ejector")||Ž.CustomData.Contains("Ejector"))continue;else Ε.Add(Ž
);}}void ΐ(){var ũ=new List<IMyTerminalBlock>();ǖ<IMyShipDrill>(ref ũ);Ε.AddRange(ũ);}void Ώ(){var ũ=new List<
IMyTerminalBlock>();ǖ<IMyShipWelder>(ref ũ);Ε.AddRange(ũ);}void Β(){var ũ=new List<IMyTerminalBlock>();ǖ<IMyShipGrinder>(ref ũ);Ε.
AddRange(ũ);}bool Ν=true;void Τ(){var ũ=new List<IMyTerminalBlock>();if(Ε==null)Ε=new List<IMyTerminalBlock>();else Ε.Clear();if
(!Ν)GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(ũ,Ǒ);else ǖ<IMyCargoContainer>(ref ũ);Ε.AddRange(ũ);ũ.Clear();
if(!Ν)GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(ũ,Ǒ);else ǖ<IMyShipConnector>(ref ũ);foreach(var Ž in ũ){if(Ž.
CustomName.Contains("Ejector")||Ž.CustomData.Contains("Ejector"))continue;else if(Ž.CustomName.Contains("Sorter")||Ž.CustomData.
Contains("Sorter"))continue;else Ε.Add(Ž);}ũ.Clear();if(!Ν)GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(ũ,Ǒ);else ǖ<
IMyShipDrill>(ref ũ);Ε.AddRange(ũ);ũ.Clear();if(!Ν)GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(ũ,Ǒ);else ǖ<IMyShipWelder>(ref
ũ);Ε.AddRange(ũ);ũ.Clear();if(!Ν)GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(ũ,Ǒ);else ǖ<IMyShipGrinder>(ref ũ);Ε.
AddRange(ũ);Κ=-1;Θ=-1;}void Σ(){if(Ε==null)Τ();if(Ε.Count<1){Κ=-1;Θ=-1;return;}Γ=0.0;double Ρ=0.0;double Π=0;bool Ο=true;bool Ξ=
false;for(int w=0;w<Ε.Count;w++){double ʑ=-1;var Λ=Ε[w].InventoryCount;for(var Ό=0;Ό<Λ;Ό++){var ˡ=Ε[w].GetInventory(Ό);if(ˡ!=
null){Γ+=(double)ˡ.CurrentVolume;if((double)ˡ.MaxVolume>9223372036854){Δ=true;}else{Δ=false;}if(!Δ){ʑ=(double)ˡ.MaxVolume;
double ͺ=ˣ(Ε[w]);if(ͺ>0)Θ=ʑ/ͺ;}else{ʑ=ˣ(Ε[w])*10;Θ=9999;}if((double)ˡ.CurrentVolume<ʑ){if(!(Ε[w]is IMyShipDrill)){Ο=false;}}
else{if(Ε[w]is IMyShipDrill){Ξ=true;}}}Ρ+=ʑ;}}if(Ρ>0){Π=(Γ/Ρ)*100;}else{Π=100;}Κ=(int)Π;if(Ο&&Ξ)Κ=101;}double Μ(
IMyTerminalBlock ˢ){double ʑ=-1;var Λ=ˢ.InventoryCount;for(var Ό=0;Ό<Λ;Ό++){var ˡ=ˢ.GetInventory(Ό);if(ˡ!=null){Γ+=(double)ˡ.
CurrentVolume;if((double)ˡ.MaxVolume>9223372036854){Δ=true;}else{Δ=false;}if(!Δ){ʑ=(double)ˡ.MaxVolume;double ͺ=ˣ(ˢ);if(ͺ>0)Θ=ʑ/ͺ;}
else{ʑ=ˣ(ˢ)*10;Θ=9999;}}}return ʑ;}double ˣ(IMyTerminalBlock ˢ){var ˡ=ˢ.GetInventory(0);string ˠ=ˢ.BlockDefinition.SubtypeId
;double ʑ=(double)ˡ.MaxVolume;if(ʑ<999999999)return ʑ;if(ˢ is IMyCargoContainer){if(ˠ.Contains("LargeBlockLargeContainer"
))ʑ=421.875008;else if(ˠ.Contains("LargeBlockSmallContainer"))ʑ=15.625;else if(ˠ.Contains("SmallBlockLargeContainer"))ʑ=
15.625;else if(ˠ.Contains("SmallBlockMediumContainer"))ʑ=3.375;else if(ˠ.Contains("SmallBlockSmallContainer"))ʑ=0.125;else if(
ˠ.Contains("Azimuth_LargeContainer"))ʑ=7780.8;else if(ˠ.Contains("Azimuth_MediumLargeContainer"))ʑ=1945.2;else if(ˠ.
Contains("Azimuth_MediumContainer"))ʑ=1878.6;else if(ˠ.Contains("Azimuth_SmallContainer"))ʑ=10.125;}else if(ˠ.Contains(
"SmallBlockDrill"))ʑ=3.375;else if(ˠ.Contains("LargeBlockDrill"))ʑ=23.4375;else if(ˠ.Contains("ConnectorMedium"))ʑ=1.152;else if(ˠ.
Contains("ConnectorSmall"))ʑ=0.064;else if(ˠ.Contains("Connector"))ʑ=8.000;else if(ˠ.Contains("LargeShipWelder"))ʑ=15.625;else
if(ˠ.Contains("LargeShipGrinder"))ʑ=15.625;else if(ˠ.Contains("SmallShipWelder"))ʑ=3.375;else if(ˠ.Contains(
"SmallShipGrinder"))ʑ=3.375;else{Echo("Unknown cargo for default Capacity:"+ˢ.DefinitionDisplayNameText+":"+ˢ.BlockDefinition.SubtypeId);ʑ
=12;}return ʑ;}const int ˑ=0;const int ː=2;const int ˏ=4;const int ˎ=8;const int ˍ=16;const int ˌ=32;const int ˋ=64;const
int ˊ=128;const int ˉ=256;const int ˈ=512;const int ˤ=1024;const int ˇ=2048;const int ˬ=0xfff;string Ί(){string ʆ="FLAGS:";
if((Χ&ː)>0)ʆ+="SLED ";if((Χ&ˌ)>0)ʆ+="ORBITAL ";if((Χ&ˋ)>0)ʆ+="ROCKET ";if((Χ&ˏ)>0)ʆ+="ROTOR ";if((Χ&ˎ)>0)ʆ+="WHEEL ";if((Χ
&ˊ)>0)ʆ+="PET ";if((Χ&ˉ)>0)ʆ+="NAD ";if((Χ&ˈ)>0)ʆ+="NO Gyro ";if((Χ&ˇ)>0)ʆ+="No Tank ";if((Χ&ˤ)>0)ʆ+="No Power ";return ʆ
;}long Ή=0;MyIni Έ=new MyIni();string Ά="";string ͽ="";void ͼ(){if(Ϋ==null){ͽ=Storage;}else{ͽ=Ϋ.GetText();}if(Ϊ==null)
return;if(ͽ==Ά){Echo("Load Skip");return;}Ά=ͽ;ͽ=ͽ.Trim();MyIniParseResult ͻ;if(!Έ.TryParse(ͽ,out ͻ)){Echo(
"MyIni:Error parsing INI:"+ͻ.ToString());string[]ǵ=ͽ.Split('\n');for(int Ǵ=0;Ǵ<ǵ.Count();Ǵ++){Echo(Ǵ+1+":"+ǵ[Ǵ]);}}Ϊ.Ƿ(ͽ);Ϊ.Ų(έ,"SaveID",ref Ή);if
(ͷ()){Ϊ.Ƿ("");}ʿ(Ϊ);Ϊ.Ų(έ,"Mode",ref ɱ,true);Ϊ.Ų(έ,"current_state",ref Ω,true);Ϊ.Ų(έ,"PassedArgument",ref γ,true);Ϊ.Ų(έ,
"AlertStates",ref Φ,true);Ϊ.Ų(έ,"craft_operation",ref Χ,true);Ϊ.Ų(έ,"PassedArgument",ref γ);Ϊ.Ų(έ,"ReceivedMessage",ref ε);}bool ͷ(){
if(Ϋ==null||N)return false;if(Ή<=0||Ή==(long)Ϋ.EntityId)return false;else return true;}bool Ͷ(string ʹ){ʹ=ʹ.Trim().ToLower
();return(ʹ=="True"||ʹ=="true");}void ͳ(){Echo("mode="+ɱ.ToString());if(ɱ==ɰ)ͱ();else if(ɱ==ɭ){ɻ("clear",Ȱ);ɻ(ʳ+
":ATTENTION!",Ȱ);ɻ(ʳ+": current_state="+Ω.ToString(),Ȱ);ɻ("\nCraft Needs attention",Ȱ);}}void Ͳ(){ɻ(DateTime.Now.ToString()+
" ACTION: Reset To Idle",ȹ,true);ʨ();н(ɰ);if(ɗ>0&&Ê()&&ɱ!=ɫ&&ɱ!=ɣ&&!((Χ&ˌ)>0)&&!((Χ&ˉ)>0))н(ɧ);}void ͱ(){ɻ("clear",Ȱ);ɻ(ʴ+":"+ʳ+
":Manual Control (idle)",Ȱ);if(ɗ>0&&Ê()&&ɱ!=ɫ&&ɱ!=ɣ&&!((Χ&ˌ)>0)&&!((Χ&ˉ)>0))н(ɧ);}List<IMyTerminalBlock>Ͱ=new List<IMyTerminalBlock>();string ˮ(
){Ͱ.Clear();Ͱ=ǖ<IMyGasGenerator>();return"GG"+Ͱ.Count.ToString("00");}void Ύ(bool Ï=true){ʢ(Ͱ,Ï);}bool ɴ(){return true;}
void ɉ(){if(Ȼ()>99){ʢ(Ͱ,false);}else{ʢ(Ͱ,true);}}List<IMyTerminalBlock>Ɉ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>
ɇ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ɇ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ʌ=new List<
IMyTerminalBlock>();const int Ʉ=1;const int Ƀ=2;int ɂ=0;int Ɂ=0;double ɀ=-1;double ȿ=-1;void Ⱦ(){ɀ=Ȼ(Ƀ);ȿ=Ȼ(Ʉ);}bool Ƚ(){return Ɇ.Count>
0;}string ȼ(){{Ɉ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyGasTank>(Ɉ,(î=>î.CubeGrid==Me.
CubeGrid));}ɂ=0;Ɂ=0;for(int w=0;w<Ɉ.Count;++w){if(Ɏ(Ɉ[w])==Ʉ){if(Ɉ[w].CustomName.ToLower().Contains("isolated"))Ʌ.Add(Ɉ[w]);else
ɇ.Add(Ɉ[w]);Ɂ++;}else if(Ɏ(Ɉ[w])==Ƀ){Ɇ.Add(Ɉ[w]);ɂ++;}}return"T"+Ɉ.Count.ToString("00");}double Ȼ(List<IMyTerminalBlock>Ɉ
){double ɒ=0;int ɑ=0;for(int w=0;w<Ɉ.Count;++w){{IMyGasTank ȱ=Ɉ[w]as IMyGasTank;if(ȱ==null)continue;float ɏ=(float)ȱ.
FilledRatio;ɒ+=ɏ;ɑ++;}}if(ɑ>0){return ɒ*100/ɑ;}else return 0;}double Ȼ(int Ď=0xff){if(Ɉ.Count<1)ȼ();if(Ɉ.Count<1)return-1;double ɐ=
0;int ɑ=0;for(int w=0;w<Ɉ.Count;++w){int Ⱥ=Ɏ(Ɉ[w]);if((Ⱥ&Ď)>0){IMyGasTank ȱ=Ɉ[w]as IMyGasTank;if(ȱ==null)continue;float ɏ
=(float)ȱ.FilledRatio;ɐ+=ɏ;ɑ++;}}if(ɑ>0){return ɐ/ɑ;}else return-1;}int Ɏ(IMyTerminalBlock ɍ){if(ɍ is IMyGasTank){if(ɍ.
BlockDefinition.SubtypeId.Contains("Hydro"))return Ƀ;else return Ʉ;}return 0;}void Ɍ(bool ɋ=true,int Ď=0xff){if(Ɉ.Count<1)ȼ();if(Ɉ.
Count<1)return;for(int w=0;w<Ɉ.Count;++w){int Ⱥ=Ɏ(Ɉ[w]);if((Ⱥ&Ď)>0){IMyGasTank ȱ=Ɉ[w]as IMyGasTank;if(ȱ==null)continue;ȱ.
Stockpile=ɋ;}}}string Ȧ="[DOCK]";List<IMyTerminalBlock>ȥ=new List<IMyTerminalBlock>();void Ȥ(Ǫ F){F.Ų(Ã,"GearUse",ref Ȧ,true);}
void ȣ(){if(ȥ.Count<1)ȥ=Ǘ<IMyLandingGear>(Ȧ);if(ȥ.Count<1)ȥ=ǖ<IMyLandingGear>();return;}string Ȣ(){{ȥ.Clear();ȣ();}return
"LG"+ȥ.Count.ToString("00");}bool ȡ(){for(int w=0;w<ȥ.Count;w++){IMyLandingGear Ƞ;Ƞ=ȥ[w]as IMyLandingGear;if(Ƞ!=null&&Ƞ.
IsLocked)return true;}return false;}bool ȟ(IMyTerminalBlock ß){var Ȟ=ß as IMyLandingGear;if(Ȟ==null)return false;return((int)Ȟ.
LockMode==1);}bool ȝ(){var Ȝ=new StringBuilder();for(int w=0;w<ȥ.Count;w++){if(ȟ(ȥ[w]))return true;}return false;}void ȧ(bool ț=
true){for(int w=0;w<ȥ.Count;w++){IMyLandingGear Ȟ=ȥ[w]as IMyLandingGear;if(Ȟ==null)continue;if(ț)Ȟ.Lock();else Ȟ.Unlock();}}
string ȸ="LOGGING";void ȷ(Ǫ F){F.Ų(ȸ,"TextPanelReport",ref ȯ,true);F.Ų(ȸ,"StatusName",ref ȳ,true);F.Ų(ȸ,"LongStatus",ref Ȳ,
true);F.Ų(ȸ,"RangeReport",ref ȵ,true);F.Ų(ȸ,"SledReport",ref ȫ,true);F.Ų(ȸ,"GPSTag",ref ȭ,true);}ȩ ȶ=null;string ȵ="[RANGE]"
;ȩ ȴ=null;string ȳ="Wico Craft Status";ȩ ȹ=null;string Ȳ="Wico Craft Log";ȩ Ȱ=null;string ȯ="Craft Report";ȩ Ȯ=null;
string ȭ="[GPS]";ȩ Ȭ=null;string ȫ="[SMREPORT]";bool Ȫ=false;bool Ȩ=false;class ȩ{Program ǡ;string ʃ="";List<IMyTextPanel>ʂ=
new List<IMyTextPanel>();string ʁ="";string ʀ="";bool ɿ=false;bool ɾ=true;public ȩ(Program Ǹ,string ǳ,bool ɽ=false){ǡ=Ǹ;ʃ=ǳ
;ɿ=ɽ;ɾ=true;ʁ="";ʀ="";ʂ.Clear();ʂ=ǡ.ƾ(ʃ);if(ʂ.Count<1)ʂ=ǡ.ƿ(ʃ);}public void ɻ(string ɺ,bool ɹ=false){if(ɺ=="clear"){ʁ="";
ʀ="X";ɾ=false;return;}if(ɿ&&ɾ){ɾ=false;if(ʂ.Count>0){ʁ=ʂ[0].GetText();ʀ="X";}}if(ɹ){ʁ=ɺ+"\n"+ʁ;}else ʁ+=ɺ+"\n";}public
void ɸ(){if(ʀ!=ʁ){ɾ=true;foreach(var ɷ in ʂ){ɷ.WriteText(ʁ);}ʀ=ʁ;}}}void ɶ(){ȴ=ʎ(true);ȹ=ʄ(Ȳ,true);;Ȱ=ʄ(ȯ);ȶ=ʄ(ȵ);Ȯ=ʄ(ȭ,N);Ȭ
=ʄ(ȫ);Ȫ=true;}void ɵ(){if(ȴ!=null)ɻ("clear",ȴ);if(ȹ!=null)ɻ("clear",ȹ);if(Ȱ!=null)ɻ("clear",Ȱ);if(ȶ!=null)ɻ("clear",ȶ);if
(Ȯ!=null)ɻ("clear",Ȯ);if(Ȭ!=null)ɻ("clear",Ȭ);}void ɼ(){if(ȴ!=null)ȴ.ɸ();if(ȹ!=null)ȹ.ɸ();if(Ȱ!=null)Ȱ.ɸ();if(ȶ!=null)ȶ.ɸ
();if(Ȯ!=null)Ȯ.ɸ();if(Ȭ!=null)Ȭ.ɸ();}ȩ ʄ(string ʋ,bool ɽ=false){ȩ ʏ=new ȩ(this,ʋ,ɽ);return ʏ;}ȩ ʎ(bool ʍ=false){if((ȴ!=
null||Ȫ)&&!ʍ)return ȴ;ȴ=ʄ(ȳ);return ȴ;}void ɻ(string ɺ,ȩ ʌ,bool ɹ=false){if(ʌ==null)return;ʌ.ɻ(ɺ,ɹ);}void ʐ(string ɺ){ɻ(ɺ,ʎ(
));if(Ȩ&&ɺ!="clear")Echo(ɺ);}string ʊ(double ʉ){int ʈ=75;if(ʉ<0)ʉ=0;int ʇ=(int)(ʉ*ʈ)/100;if(ʇ>ʈ)ʇ=ʈ;string ʆ="["+new
String('|',ʇ)+new String('\'',ʈ-ʇ)+"]";return ʆ;}void ʅ(string ǳ,Vector3D ȍ){string Ƴ;Ƴ="GPS:"+ǳ+":"+ƨ(ȍ)+":";ɻ(Ƴ,Ȯ);}string ɠ
(string ɩ,string ɟ){string Û;int ɞ=ɩ.Length;int ɝ=ɟ.Length;if(ɞ+ɝ>32){if(ɝ>31)return"INVALID";ɞ=32-ɝ;}Û=ɩ.Substring(0,ɞ)+
ɟ;Û.Replace(":","_");Û.Replace(";","_");return Û;}string ɜ(double ɛ){string ɚ="";if(ɛ>1000){ɚ=ɛ.ToString("N0")+"km";}else
if(ɛ>10){ɚ=ɛ.ToString("0.0")+"m";}else{ɚ=ɛ.ToString("0.000")+"m";}return ɚ;}void ə(){}void ɘ(Ǫ D){Â(D);Ϻ(D);ǈ(D);Ȥ(D);Ɠ(D)
;Ζ(D);Ť(D);}double ɗ=-1;string ɖ(){if(j){Echo("(RE)INIT:"+k);}do{ù("Init:"+l+" ");if(j){Echo("ERROR: Need (RE)INIT:"+k);
Echo(k);}switch(l){case 0:k="";if(j)à();j=false;ɻ(DateTime.Now.ToString()+" "+ʴ+":"+ʳ+":INIT",ȹ,true);break;case 1:if(!Í.
ContainsKey("doscans"))Í.Add("doscans",ф);break;case 2:m+=à();break;case 3:ɶ();break;case 4:ů();break;case 5:m+=κ();ͼ();break;case
6:m+=ǋ();break;case 7:Τ();break;case 8:ƒ();break;case 9:m+=ϕ(ǌ);break;case 10:m+=ș();break;case 11:if(ǌ is
IMyRemoteControl){Vector3D ɕ;bool ɔ=((IMyRemoteControl)ǌ).GetNearestPlayer(out ɕ);IMyRemoteControl ɓ=(IMyRemoteControl)ǌ;ɓ.
SetCollisionAvoidance(false);ɓ.SetDockingMode(false);ɓ.Direction=Base6Directions.Direction.Forward;ɓ.FlightMode=FlightMode.OneWay;ɓ.
ClearWaypoints();}break;case 12:m+=Ɖ(ǌ);break;case 13:m+=ќ();break;case 14:m+=Á();break;case 15:m+=ȼ();break;case 16:m+=ö();break;case
17:break;case 18:m+=þ();break;case 19:m+=ő();break;case 20:m+=ˮ();break;case 21:ʦ();break;case 22:if(ţ)Ì="*"+ʴ+":"+ʳ+" V"+
ʲ+" ";if(Ì.Length>34){Ì=ʴ+":"+ʳ+"\nV"+ʲ+" ";}if(ǌ is IMyShipController){MyShipMass å;å=((IMyShipController)ǌ).
CalculateShipMass();ɗ=å.BaseMass;}m+=ɳ();break;case 23:{o=true;if(ǌ==null){ǌ=Me;k+="\nUsing "+Me.CustomName+" as orientation";k+=
"\nNo Ship Controller";U=-1.0;}else{if(ǌ is IMyShipController){V=((IMyShipController)ǌ).GetShipSpeed();Vector3D û=((IMyShipController)ǌ).
GetNaturalGravity();double ú=û.Length();U=ú/9.81;}else{U=-1.0;}}Echo("Grid Mass="+ɗ.ToString());if(ɗ>0){if(U==0){if(Ϩ<1&&ϯ<1)k+=
"\nIn Space, but no valid thrusters";}if(Ϩ<1&&ϯ<1&&ϸ<1){if(Ɛ.Count<1){if(ѕ.Count<1){if(Ɗ.Count<1){j=true;k+="\nNo Propulsion Method Found";k+=
"\nNo Thrusters.\nNo NAV Rotors\nNo Sled Wheels\nNo Wheels";}}}else{j=true;k+="\nNo Valid Propulsion Method Found";k+="\nSled wheels, but No Thrusters.\nNo NAV Rotors";if(ǃ.Count<
1){j=true;k="\nSled wheels, but no Gyros";}}}else{if(ǃ.Count<1){j=true;k+="\nNo Gyros Found";}if(ǌ is IMyShipController){
}}if(ŭ(Ŕ)){if(Ϋ==null){j=true;k+="\nSubmodule timer, but no text\n panel named:"+ή;}}else{if(u){j=true;k+=
"\nSubmodules Enabled, but no\n timer containing:"+Ŕ;if(Ϋ==null){j=true;k+="\n No text\n panel containing:"+ή;}}}}if(!j){o=true;}else{l=-1;}break;}}l++;}while(!o&&(((
float)Runtime.CurrentInstructionCount/(float)Runtime.MaxInstructionCount)<0.2f));if(o)l=0;if(j){Echo("ERROR: Need (RE)INIT:"+
k);}Echo(k);ʐ(m);return m;}string ɳ(){return">";}int ɱ=-1;const int ɰ=0;const int ɯ=1;const int ɮ=2;const int ɭ=3;const
int ɬ=4;const int ɫ=5;const int ɲ=7;const int ɪ=8;const int ɨ=9;const int ɧ=13;const int ɦ=10;const int ɥ=11;const int ɤ=12
;const int ɣ=14;const int ɢ=15;const int Ɋ=16;const int ɡ=17;const int Υ=18;const int д=19;const int в=20;const int б=21;
const int а=22;const int Я=23;const int Ю=24;const int Э=25;const int Ь=26;const int г=27;const int Ы=28;const int Щ=29;const
int Ш=30;const int Ч=31;const int Ц=33;const int Х=50;const int Ф=60;const int У=111;const int Ъ=112;const int е=200;const
int о=210;const int ш=220;const int ц=225;const int х=290;const int ф=400;const int у=410;const int т=500;const int с=510;
const int р=600;const int ч=610;const int п=999;void н(int м){if(ɱ==м)return;ɱ=м;Ω=0;ŧ();}const string л="WICOB_NAVADDTARGET"
;const string к="WICOB_NAVSTART";const string й="WICOB_NAVRESET";const string и="WICO_NAVLAUNCH";const string з=
"WICO_NAVDOCK";const string ж="WICO_NAVORBITALLAUNCH";const string С="WICO_NAVLAND";bool Д=false;public enum Н{В,Б,А,Џ,Ў,Ѝ,Ќ,Ћ,Њ};
class Љ{public Program Ǹ;public Н Ј=Н.В;public Vector3D Ї;public bool І=false;public int Ѕ=Ш;public int Г=0;public double Є=
9999;public double Е=50;public string М="";public bool Р(){switch(Ј){case Н.Б:{Ǹ.Ї=Ї;Ǹ.І=true;Ǹ.Ѕ=Ѕ;Ǹ.Г=Г;Ǹ.Е=Е;Ǹ.М=М;Ǹ.Є=Є;
Ǹ.Д=true;Ǹ.k+="Going to:"+Ǹ.М;Ǹ.k+=" arrivald="+Ǹ.Е.ToString();Ǹ.н(ɲ);}break;case Н.А:{Ǹ.Ї=Ї;Ǹ.І=true;Ǹ.Ѕ=Ѕ;Ǹ.Г=Г;Ǹ.М=М;Ǹ
.Д=false;Ǹ.н(ɲ);}break;case Н.Ў:{Ǹ.Є=Є;Ǹ.н(ч);}break;case Н.Џ:{Ǹ.Е=Е;Ǹ.н(ч);}break;case Н.Ѝ:{Ǹ.н(ɫ);}break;case Н.Њ:{Ǹ.н(
Ы);}break;case Н.Ћ:{Ǹ.н(ɨ);}break;case Н.Ќ:{Ǹ.н(Щ);}break;case Н.В:{Ǹ.Echo("Unknown Command");Ǹ.н(ɭ);return true;}}return
false;}}List<Љ>П=new List<Љ>();Vector3D Ї;bool І=false;DateTime О;double Є=9999;double Е=50;int Ѕ=Ш;int Г=0;string М="";bool
Л=true;bool К=true;bool Й=true;bool И=false;bool З=false;float Ж=-1;bool Т=true;string щ="NAV";void ў(Ǫ F){F.Ų(щ,
"DTMDebug",ref Л,true);F.Ų(щ,"CameraCollision",ref К,true);F.Ų(щ,"SensorCollision",ref Й,true);F.Ų(щ,"NAVEmulateOld",ref И,true);F
.Ų(щ,"NAVGravityMinElevation",ref Ж,true);F.Ų(щ,"NavBeaconDebug",ref Т,true);F.Ų(щ,"AllowBlindNav",ref З,true);if(Є>I)Є=I
;}void ѥ(Ǫ F){F.ź(щ,"vTarget",Ї);F.ź(щ,"ValidNavTarget",І);F.ź(щ,"TargetName",М);F.ź(щ,"dStartShip",О);F.ź(щ,
"shipSpeedMax",Є);F.ź(щ,"arrivalDistanceMin",Е);F.ź(щ,"NAVArrivalMode",Ѕ);F.ź(щ,"NAVArrivalState",Г);}void Ѥ(Ǫ F){F.Ų(щ,"vTarget",ref
Ї,true);F.Ų(щ,"ValidNavTarget",ref І,true);F.Ų(щ,"TargetName",ref М,true);F.Ų(щ,"dStartShip",ref О,true);F.Ų(щ,
"shipSpeedMax",ref Є,true);F.Ų(щ,"arrivalDistanceMin",ref Е,true);F.Ų(щ,"NAVArrivalMode",ref Ѕ,true);F.Ų(щ,"NAVArrivalState",ref Г,
true);}List<IMyBeacon>ѣ=new List<IMyBeacon>();void Ѣ(string ѡ){if(Т){if(ѣ.Count<1)GridTerminalSystem.GetBlocksOfType(ѣ);
foreach(var Ѧ in ѣ){Ѧ.CustomName=ѡ;}}}void Ѡ(){IGC.SendBroadcastMessage(й,"",TransmissionDistance.CurrentConstruct);}void џ(
Vector3D ύ,int ђ=ч,int ё=0,double ѐ=50,string я="",double ю=9999,bool э=true){string ѧ=ѩ(ύ,ђ,ё,ѐ,я,ю,э);IGC.SendBroadcastMessage
(л,ѧ,TransmissionDistance.CurrentConstruct);}void Ѩ(Vector3D ύ,int ђ=Ш,int ё=0,double ѐ=50,string я="",double ю=9999,bool
э=true){string ѧ=ѩ(ύ,ђ,ё,ѐ,я,ю,э);IGC.SendBroadcastMessage(к,ѧ,TransmissionDistance.CurrentConstruct);}void Ѫ(){IGC.
SendBroadcastMessage(к,"",TransmissionDistance.CurrentConstruct);}string ѩ(Vector3D ύ,int ђ=ч,int ё=0,double ѐ=50,string я="",double ю=9999,
bool э=true){string є="";є+=ƨ(ύ);є+="\n";є+=ђ.ToString();є+="\n";є+=ё.ToString();є+="\n";є+=ѐ.ToString();є+="\n";є+=я;є+=
"\n";є+=ю.ToString();є+="\n";є+=э.ToString();є+="\n";return є;}void ѝ(string є,out Vector3D ύ,out int ђ,out int ё,out double
ѐ,out string я,out double ю,out bool э){є=є.Trim();string[]ь=є.Split('\n');string[]Ƭ=ь[0].Split(',');if(Ƭ.Length<3){Ƭ=ь[0
].Split(':');}double î,Ʈ,ƭ;int ы=0;bool ƫ=double.TryParse(Ƭ[ы++].Trim(),out î);bool ƪ=double.TryParse(Ƭ[ы++].Trim(),out Ʈ
);bool Ʃ=double.TryParse(Ƭ[ы++].Trim(),out ƭ);if(!ƫ||!ƪ||!Ʃ){Echo("Invalid Command:("+ь[0]+")");}ύ=new Vector3D(î,Ʈ,ƭ);
int.TryParse(ь[1],out ђ);int.TryParse(ь[2],out ё);double.TryParse(ь[3],out ѐ);я=ь[4];double.TryParse(ь[5],out ю);э=true;if(
ь.Length>5)bool.TryParse(ь[6],out э);}List<IMyTerminalBlock>ѓ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ъ=new
List<IMyTerminalBlock>();List<IMyTerminalBlock>ѕ=new List<IMyTerminalBlock>();string ќ(){ѓ.Clear();ъ.Clear();ѕ.Clear();
GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(ѓ,Ǒ);for(int w=0;w<ѓ.Count;w++){if(ѓ[w].CustomName.Contains("[LEFT]")||ѓ[w].CustomData.
Contains("[LEFT]")){ъ.Add(ѓ[w]);}else if(ѓ[w].CustomName.Contains("[RIGHT]")||ѓ[w].CustomData.Contains("[RIGHT]")){ѕ.Add(ѓ[w]);}
}return"NR:L"+ъ.Count.ToString("0")+"R"+ѕ.Count.ToString("0");}bool ћ(float Ơ){if(ъ.Count<1)return false;float љ=ъ[0].
GetMaximum<float>("Velocity");var è=ъ[0]as IMyMotorStator;float њ=è.TargetVelocityRPM;float ƛ=(њ/љ*100);ƛ=Math.Abs(ƛ);if(Ơ>(ƛ+5f))
Ơ=ƛ+5;if(Ơ<(ƛ-5))Ơ=ƛ-5;if(Ơ<0f)Ơ=0f;if(Ơ>100f)Ơ=100f;if(Math.Abs(Ơ)>0){ћ(ъ,-Ơ);ћ(ѕ,Ơ);return true;}else return false;}
bool ћ(List<IMyTerminalBlock>і,float Ơ){for(int w=0;w<і.Count;w++){var è=і[w]as IMyMotorStator;float љ=è.GetMaximum<float>(
"Velocity");if(!è.Enabled)è.Enabled=true;float ј=љ*(Ơ/100.0f);è.TargetVelocityRPM=ј;}return true;}bool ї(){ї(ъ);ї(ѕ);return true;}
bool ї(List<IMyTerminalBlock>і){for(int w=0;w<і.Count;w++){IMyMotorStator è=і[w]as IMyMotorStator;è.TargetVelocityRPM=0;}
return true;}bool ϧ(double σ){float Ơ;if(Math.Abs(σ)>1.0){Ơ=50;}else if(Math.Abs(σ)>.7){Ơ=50;}else if(Math.Abs(σ)>0.5){Ơ=30;}
else if(Math.Abs(σ)>0.1){Ơ=20;}else if(Math.Abs(σ)>0.01){Ơ=5;}else if(Math.Abs(σ)>0.001){Ơ=0;}else Ơ=0;Ơ/=3;Ơ=Ơ*-Math.Sign(σ
);if(Math.Abs(Ơ)>0){ћ(ъ,Ơ);}if(Math.Abs(Ơ)>0){ћ(ѕ,Ơ);}if(Math.Abs(Ơ)>0)return false;else return true;}bool ς(string Q){
string[]ρ=Q.Trim().Split(';');for(int π=0;π<ρ.Length;π++){string[]ο=ρ[π].Trim().Split(' ');if(ο[0]=="timer"){ˁ();}else if(ο[0]
=="idle")Ͳ();else if(ο[0]=="setmode"){if(ο.Length<2){Echo("Invalid command format:\nsetmode <mode#>");}else{int ƅ;bool ξ=
int.TryParse(ο[1],out ƅ);if(!ξ){Echo("Invalid INT value:"+ο[1]);}else{Echo("Set Mode to"+ƅ);н(ƅ);}}}else if(ο[0]==
"setstate"){if(ο.Length<2){Echo("Invalid command format:\nsetstate <state#>");}else{int ƅ;bool ξ=int.TryParse(ο[1],out ƅ);if(!ξ){
Echo("Invalid INT value:"+ο[1]);}else{Echo("Set State to"+ƅ);Ω=ƅ;}}}else if(ο[0]=="masterreset")ʵ();else if(ο[0].ToLower()==
"coast"){if(ϲ.Count>1){ʡ(ϲ);}}else if(ο[0]=="setvaluef"){Echo("SetValueFloat");string ν="";for(int w=1;w<ο.Length;w++){ν+=ο[w];
if(w<ο.Length-1){ν+=" ";}}string[]μ=ν.Trim().Split(':');if(μ.Length<3){Echo("Invalid Args");continue;}IMyTerminalBlock ß;ß
=(IMyTerminalBlock)GridTerminalSystem.GetBlockWithName(μ[0]);if(ß==null){Echo("Block not found:"+μ[0]);continue;}float τ=
0;bool λ=float.TryParse(μ[2].Trim(),out τ);if(!λ){Echo("invalid float value:"+μ[2]);continue;}Echo("SetValueFloat:"+μ[0]+
" "+μ[1]+" to:"+τ.ToString());ß.SetValueFloat(μ[1],τ);}else if(ο[0]=="brake"){Echo("brake");if(ǌ is IMyShipController){
IMyShipController υ=ǌ as IMyShipController;bool ϑ=υ.HandBrake;υ.ApplyAction("HandBrake");}else Echo("No Ship Controller found");}else if(
ο[0]=="genpatrol"){Echo("genpatrol");double ϐ=500;double Ϗ=500;if(ο.Length>1){bool λ=double.TryParse(ο[1].Trim(),out ϐ);}
if(ο.Length>2){bool λ=double.TryParse(ο[2].Trim(),out Ϗ);}string ώ="WICO:PATROL:";ώ="";Vector3D ύ;Vector3D ό=ǌ.WorldMatrix
.Up;if(ǌ is IMyShipController){Vector3D û=((IMyShipController)ǌ).GetNaturalGravity();if(û.Length()>0.05){ό=û;ό.Normalize(
);}}ώ+="4:";ɻ("clear",Ȯ);ύ=ǌ.GetPosition()+ǌ.WorldMatrix.Up*Ϗ+ǌ.WorldMatrix.Right*ϐ;ʅ("Patrol0",ύ);ώ+=ƨ(ύ)+":";ύ=ǌ.
GetPosition()+ǌ.WorldMatrix.Up*Ϗ+ǌ.WorldMatrix.Forward*ϐ;ʅ("Patrol1",ύ);ώ+=ƨ(ύ)+":";ύ=ǌ.GetPosition()+ǌ.WorldMatrix.Up*Ϗ+ǌ.
WorldMatrix.Left*ϐ;ʅ("Patrol2",ύ);ώ+=ƨ(ύ)+":";ύ=ǌ.GetPosition()+ǌ.WorldMatrix.Up*Ϗ+ǌ.WorldMatrix.Backward*ϐ;ʅ("Patrol3",ύ);ώ+=ƨ(ύ)+
":";ŉ("PATROL",ώ);k+="PATROL:\n"+ώ;}else if(ο[0]=="wcct"||ο[0]==""){}else{int ϋ;if(Í.TryGetValue(ο[0].ToLower(),out ϋ)){н(ϋ
);}else Echo("Unrecognized Command:"+ρ[π]);}}return false;}bool ϊ(string Q){return false;}double ω=-1;List<
IMyTerminalBlock>ψ=new List<IMyTerminalBlock>();void χ(){ψ.Clear();ǖ<IMyReactor>(ref ψ);float β;θ(out β);}double φ(){double q=0;foreach(
var ʝ in ψ){IMyReactor ʜ=ʝ as IMyReactor;q+=ʜ.CurrentOutput;}return q;}bool θ(out float β){β=0;ω=-1;bool α=false;if(ψ.Count
>0)ω=0;foreach(IMyReactor ʜ in ψ){β+=ʜ.CurrentOutput;ω+=ʜ.MaxOutput;}return α;}void ΰ(){if(Ϊ==null)return;ʾ(Ϊ);Ϊ.ź(έ,
"Mode",ɱ.ToString());Ϊ.ź(έ,"current_state",Ω.ToString());Ϊ.ź(έ,"PassedArgument",γ);Ϊ.ź(έ,"AlertStates",Φ.ToString());Ϊ.ź(έ,
"craft_operation",Χ.ToString());Ϊ.ź(έ,"ReceivedMessage",ε);long ί=0;if(Ϋ!=null)ί=Ϋ.EntityId;Ϊ.ź(έ,"SaveID",(long)ί);if(Ϊ.Ǻ){if(Ϊ.Ǻ){
string Ƕ=Ϊ.Ʒ();if(Ϋ==null){Echo("WARNING: saving to Storage");Storage=Ƕ;}else{Ϋ.WriteText(Ƕ,false);}}}else{Echo(
"Not saving: Same");}}string ή="Wico Craft Save";string έ="WCCM2";void ά(Ǫ F){F.Ų(έ,"SAVE_FILE_NAME",ref ή,true);}IMyTextPanel Ϋ=null;Ǫ Ϊ=
null;int Ω=0;long Ψ=0;int Χ=ˑ;string γ="";int Φ=0;string ε="";string κ(){string m="S";Ϋ=null;List<IMyTerminalBlock>ũ=new
List<IMyTerminalBlock>();ũ=ǁ<IMyTextPanel>(ή);if(ũ.Count>1){j=true;k+="\nMultiple blocks found:\""+ή+"\"";}else if(ũ.Count==
0){ũ=Ǘ<IMyTextPanel>(ή);if(ũ.Count==1)Ϋ=ũ[0]as IMyTextPanel;else{ũ=Ƽ<IMyTextPanel>(ή);if(ũ.Count==1)Ϋ=ũ[0]as IMyTextPanel
;}}else Ϋ=ũ[0]as IMyTextPanel;Ϊ=new Ǫ(this,"");if(Ϋ==null){m="-";}return m;}bool ι(){return Ϋ!=null;}string ƨ(Vector3D Ƨ)
{string Û;Û=Ƨ.X.ToString("0.00")+":"+Ƨ.Y.ToString("0.00")+":"+Ƨ.Z.ToString("0.00");return Û;}bool Ư(string ƶ,out double î
,out double Ʈ,out double ƭ){string[]Ƭ=ƶ.Trim().Split(',');if(Ƭ.Length<3){Ƭ=ƶ.Trim().Split(':');}î=0;Ʈ=0;ƭ=0;if(Ƭ.Length<3
)return false;bool ƫ=double.TryParse(Ƭ[0].Trim(),out î);bool ƪ=double.TryParse(Ƭ[1].Trim(),out Ʈ);bool Ʃ=double.TryParse(
Ƭ[2].Trim(),out ƭ);if(!ƫ||!ƪ||!Ʃ){return false;}return true;}List<IMyTerminalBlock>η=new List<IMyTerminalBlock>();float δ
=0;double ζ=-1;void ϒ(){η.Clear();ζ=-1;GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(η,Ǒ);Ϸ();}void Ϸ(){if(η.Count>0)
ζ=0;δ=0;foreach(var ʝ in η){IMySolarPanel ʜ=ʝ as IMySolarPanel;ζ+=ʜ.MaxOutput;δ+=ʜ.CurrentOutput;}}List<IMyTerminalBlock>
ϴ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ϳ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ϲ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>ϱ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ϰ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>ϵ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ϧ=new List<IMyTerminalBlock>();double Ϯ=0;double ϭ=0;double Ϭ=0;
double ϫ=0;double Ϫ=0;double ϩ=0;int Ϩ=0;int ϯ=0;int ϸ=0;int Ϲ=0;const int Ѓ=1;const int Ё=2;const int Ѐ=4;const int Ͽ=8;const
int Ͼ=0xff;Matrix Ͻ=new Matrix(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);string ϼ="IGNORE";string ϻ="cutter";string Ђ="THRUSTERS";
void Ϻ(Ǫ F){F.Ų(Ђ,"IgnoreThruster",ref ϼ);F.Ų(Ђ,"CutterThruster",ref ϻ);}void ϕ(IMyTerminalBlock ƈ,ref List<IMyTerminalBlock
>ϳ,ref List<IMyTerminalBlock>ϲ,ref List<IMyTerminalBlock>ϱ,ref List<IMyTerminalBlock>ϰ,ref List<IMyTerminalBlock>ϵ,ref
List<IMyTerminalBlock>Ϧ,int Ϛ=Ͼ){ϳ.Clear();ϲ.Clear();ϱ.Clear();ϰ.Clear();ϵ.Clear();Ϧ.Clear();ϴ.Clear();if(ƈ==null)return;var
ϟ=new List<IMyTerminalBlock>();ǖ<IMyThrust>(ref ϟ);for(int w=0;w<ϟ.Count;w++){if(ϟ[w].CustomName.ToLower().Contains(ϻ)||ϟ
[w].CustomData.ToLower().Contains(ϻ))continue;if(ϟ[w].CustomName.ToLower().Contains(ϼ)||ϟ[w].CustomData.ToLower().
Contains(ϼ))continue;ϴ.Add(ϟ[w]);}Matrix ϙ;ƈ.Orientation.GetMatrix(out ϙ);Matrix.Transpose(ref ϙ,out ϙ);Ϯ=0;ϭ=0;Ϭ=0;ϫ=0;Ϫ=0;ϩ=0;
for(int w=0;w<ϴ.Count;++w){var ē=ϴ[w]as IMyThrust;Matrix Ϙ;ē.Orientation.GetMatrix(out Ϙ);Vector3 ϗ=Vector3.Transform(Ϙ.
Backward,ϙ);int ϖ=ϔ(ϴ[w]);if(ϖ==Ѓ)ϸ++;else if(ϖ==Ё)ϯ++;else if(ϖ==Ѐ)Ϩ++;else if(ϖ==Ͽ)Ϲ++;if(ϗ==Ͻ.Left){Ϫ+=ĝ((IMyThrust)ϴ[w]);ϵ.
Add(ϴ[w]);}else if(ϗ==Ͻ.Right){ϩ+=ĝ((IMyThrust)ϴ[w]);Ϧ.Add(ϴ[w]);}else if(ϗ==Ͻ.Backward){ϭ+=ĝ((IMyThrust)ϴ[w]);ϲ.Add(ϴ[w]);
}else if(ϗ==Ͻ.Forward){Ϯ+=ĝ((IMyThrust)ϴ[w]);ϳ.Add(ϴ[w]);}else if(ϗ==Ͻ.Up){ϫ+=ĝ((IMyThrust)ϴ[w]);ϰ.Add(ϴ[w]);}else if(ϗ==
Ͻ.Down){Ϭ+=ĝ((IMyThrust)ϴ[w]);ϱ.Add(ϴ[w]);}}}string ϕ(IMyTerminalBlock ƈ){ϳ.Clear();ϲ.Clear();ϱ.Clear();ϰ.Clear();ϵ.Clear
();Ϧ.Clear();ϴ.Clear();if(ƈ==null)return"No Orientation Block";ϕ(ƈ,ref ϳ,ref ϲ,ref ϱ,ref ϰ,ref ϵ,ref Ϧ);string Û;Û=">";Û
+="F"+ϳ.Count.ToString("00");Û+="B"+ϲ.Count.ToString("00");Û+="D"+ϱ.Count.ToString("00");Û+="U"+ϰ.Count.ToString("00");Û+=
"L"+ϵ.Count.ToString("00");Û+="R"+Ϧ.Count.ToString("00");Û+="<";return Û;}int ϔ(IMyTerminalBlock ɍ){if(ɍ is IMyThrust){if(ɍ
.BlockDefinition.SubtypeId.Contains("AtmosphericHover"))return Ͽ;else if(ɍ.BlockDefinition.SubtypeId.Contains("Atmo"))
return Ѓ;else if(ɍ.BlockDefinition.SubtypeId.Contains("Hydro"))return Ё;else if(ɍ.BlockDefinition.SubtypeId.Contains(
"SmallBlock_HoverEngine"))return Ͽ;else return Ѐ;}return 0;}double ĝ(IMyThrust ē){return ē.MaxEffectiveThrust;}double ϓ(List<IMyTerminalBlock>ė,
int Ď=Ͼ){double ĥ=0;for(int Ī=0;Ī<ė.Count;Ī++){int Đ=ϔ(ė[Ī]);if((Đ&Ď)>0){IMyThrust ē=ė[Ī]as IMyThrust;double Ϥ=ē.
MaxEffectiveThrust;ĥ+=Ϥ;}}return ĥ;}double ϣ(List<IMyTerminalBlock>ė,float Ϣ=5f,float ϡ=2f,float ϥ=1f){double Ϡ=0;foreach(var ß in ė){var
ē=ß as IMyThrust;if(ē==null)continue;if(ϔ(ē)==Ѓ)Ϡ+=ē.MaxEffectiveThrust*Ϣ;else if(ϔ(ē)==Ѐ)Ϡ+=ē.MaxEffectiveThrust*ϡ;else
if(ϔ(ē)==Ё)Ϡ+=ē.MaxEffectiveThrust*ϥ;else Ϡ+=ē.MaxEffectiveThrust;}return Ϡ;}bool Ϟ(List<IMyTerminalBlock>ė,out float ϝ,
out float ɀ,out float Ϝ){ϝ=0;ɀ=0;Ϝ=0;double ϛ=ϓ(ė,Ѐ);double Ț=ϓ(ė,Ѓ);double į=ϓ(ė,Ё);MyShipMass å;å=((IMyShipController)ǌ).
CalculateShipMass();double Ğ=0;Ğ=å.PhysicalMass*U*9.810;if(Ț>0){if(Ț<Ğ){ϝ=100;Ğ-=Ț;}else{ϝ=(float)(Ğ/Ț*100);if(ϝ>0)Ğ-=(Ț*ϝ/100);}}if(ϛ>0
&&Ğ>0){if(ϛ<Ğ){Ϝ=100;Ğ-=ϛ;}else{Ϝ=(float)(Ğ/ϛ*100);if(Ϝ>0)Ğ-=((ϛ*Ϝ)/100);}}if(į>0&&Ğ>0){if(į<Ğ){ɀ=100;Ğ-=į;}else{ɀ=(float)
(Ğ/į*100);if(ɀ>0)Ğ-=((į*ɀ)/100);;}}if(Ğ>0)return false;return true;}List<IMyTerminalBlock>Į(string ĭ){var Ĭ=new List<
IMyTerminalBlock>();var ĩ=new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(ĩ);for(int ġ=0;ġ<ĩ.Count;ġ++){if(ĩ[ġ].Name==ĭ){
List<IMyTerminalBlock>ė=null;ĩ[ġ].GetBlocks(ė,Ǒ);for(int Ī=0;Ī<ė.Count;Ī++){Ĭ.Add(ė[Ī]);}break;}}return Ĭ;}int Ė(List<
IMyTerminalBlock>ė,float ī,int Ď=Ͼ){int ę=0;if(ī>100)ī=100;if(ī<0)ī=0;for(int Ī=0;Ī<ė.Count;Ī++){int Đ=ϔ(ė[Ī]);if((Đ&Ď)>0){IMyThrust ē=ė
[Ī]as IMyThrust;if(!ē.IsWorking){if(!ē.Enabled)ē.Enabled=true;}ę+=1;ē.ThrustOverridePercentage=ī/100f;}}return ę;}int Ė(
List<IMyTerminalBlock>ė,int Ĳ=100,int Ď=Ͼ){return Ė(ė,(float)Ĳ,Ď);}bool Ė(string ı,int Ĳ=100,int Ď=Ͼ){if(Ĳ>100)Ĳ=100;var ĩ=
new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(ĩ);for(int ġ=0;ġ<ĩ.Count;ġ++){if(ĩ[ġ].Name==ı){List<
IMyTerminalBlock>ė=null;ĩ[ġ].GetBlocks(ė,Ǒ);return(Ė(ė,Ĳ,Ď)>0);}}return false;}int ĕ(List<IMyTerminalBlock>ė,int Ď=Ͼ,bool İ=false){int ę
=0;for(int Ī=0;Ī<ė.Count;Ī++){int Đ=ϔ(ė[Ī]);if((Đ&Ď)>0){ę++;IMyThrust ē=ė[Ī]as IMyThrust;ē.ThrustOverride=0;if(ē.
IsWorking&&İ&&ē.Enabled==true)ē.Enabled=false;else if(!ē.IsWorking&&!İ&&ē.Enabled==false)ē.Enabled=true;}}return ę;}bool ĕ(string
ı){var ĩ=new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(ĩ);for(int ġ=0;ġ<ĩ.Count;ġ++){if(ĩ[ġ].Name==ı){List<
IMyTerminalBlock>ė=null;ĩ[ġ].GetBlocks(ė,Ǒ);return(ĕ(ė)>0);}}return false;}bool Ė(){return(Ė(ϳ)>0);}bool ĕ(){return(ĕ(ϳ)>0);}double Ĕ(
List<IMyTerminalBlock>đ,int Ď=Ͼ){for(int w=0;w<đ.Count;w++){int Đ=ϔ(đ[w]);if((Đ&Ď)>0&&đ[w].IsWorking){var ē=đ[w]as IMyThrust
;return ē.ThrustOverride;}}return 0;}bool Ē(List<IMyTerminalBlock>đ,int Ď=Ͼ){for(int w=0;w<đ.Count;w++){int Đ=ϔ(đ[w]);if(
(Đ&Ď)>0&&đ[w].IsWorking){return true;}}return false;}int ď(List<IMyTerminalBlock>đ,int Ď=Ͼ){int ę=0;for(int w=0;w<đ.Count
;w++){int Đ=ϔ(đ[w]);if((Đ&Ď)>0&&đ[w].IsWorking){ę++;}}return ę;}IMyThrust ħ(List<IMyTerminalBlock>Ø,int Ħ=Ͼ){foreach(var
ĥ in ϴ){if(ĥ is IMyThrust&&(ϔ(ĥ)&Ħ)>0)return ĥ as IMyThrust;}return null;}double Ĥ(){if(ϸ<1)return 0;var ģ=ħ(ϴ,Ѓ);if(ģ==
null)return 0;return ģ.MaxEffectiveThrust/ģ.MaxThrust;}double Ĩ(List<IMyTerminalBlock>Ģ,double Ġ,double ğ){var å=((
IMyShipController)ǌ).CalculateShipMass();double Ğ=å.PhysicalMass*ğ*9.810;double ĝ=ϓ(Ģ);double Ĝ=(ĝ-Ğ)/å.PhysicalMass;double ě=Ġ/Ĝ;double
Ę=Ġ/2*ě;return Ę;}int Ě=0;void ĳ(float Š,float ş,List<IMyTerminalBlock>Ş,List<IMyTerminalBlock>ŝ){if(Ě<0)Ě=0;double ĝ=ϓ(Ş
);MyShipMass å;å=((IMyShipController)ǌ).CalculateShipMass();double Ŝ=å.PhysicalMass;float ś=100f;if(Ŝ>0){double Ĝ=(ĝ)/Ŝ;
if(Ĝ>0)ś=(float)(Š/Ĝ);}if(V>ş){ĕ(ϴ);}else if(V<(Š*0.90)){if(V<0.09)Ě++;if(V<Š*0.25)Ě++;Ė(Ş,ś+Ě/5);}else if(V<(Š*1.1)){Ě--;
ĕ(ŝ,Ͼ,true);ĕ(Ş);}else{Ě--;Ě--;Ė(Ş,1f);}}void Ś(){Ě=0;}string[]Ř={"-","\\","|","/","-","\\","|","/"};int ŗ=99;string Ŗ(){
ŗ++;if(ŗ>=Ř.Length)ŗ=0;return Ř[ŗ];}string ŕ="[WCCT]";string Ŕ="[WCCS]";string œ="[WCCM]";string Œ="WICOTIMERS";void ř(Ǫ
F){F.Ų(Œ,"FastTimer",ref ŕ,true);F.Ų(Œ,"SubModuleTimer",ref Ŕ,true);F.Ų(Œ,"MainTimer",ref œ,true);}Dictionary<string,List
<IMyTerminalBlock>>Ũ=new Dictionary<string,List<IMyTerminalBlock>>();void ů(){Ũ.Clear();ŭ(ŕ);ŭ(Ŕ);ŭ(œ);}bool ŭ(string ū){
var ũ=new List<IMyTerminalBlock>();if(Ũ.ContainsKey(ū)){ũ=Ũ[ū];if(ũ.Count>0)return true;}else{ũ=Ǘ<IMyTimerBlock>(ū);Ũ.Add(ū
,ũ);if(ũ.Count>0)return true;}return false;}bool Ŭ(string ū="[WCCS]"){bool Ū=false;List<IMyTerminalBlock>ũ=new List<
IMyTerminalBlock>();IMyTimerBlock Ů=null;if(Ũ.ContainsKey(ū)){ũ=Ũ[ū];}else{ũ=Ǘ<IMyTimerBlock>(ū);Ũ.Add(ū,ũ);}for(int w=0;w<ũ.Count;w++){
Ů=ũ[w]as IMyTimerBlock;if(Ů!=null){if(Ů.Enabled){Ů.Trigger();Ū=true;}else{Echo("Timer:"+Ů.CustomName+" is OFF");}}}return
Ū;}void ŧ(){Runtime.UpdateFrequency|=UpdateFrequency.Once;}bool Ŧ=false;string ť="COMMUNICATIONS";void Ť(Ǫ F){F.Ų(ť,
"CommunicationsStealth",ref Ŧ,false);}bool ţ=false;List<IMyRadioAntenna>Ţ=new List<IMyRadioAntenna>();List<IMyLaserAntenna>š=new List<
IMyLaserAntenna>();string ő(){Ţ.Clear();š.Clear();ǖ<IMyRadioAntenna>(ref Ţ);ǖ<IMyLaserAntenna>(ref š);for(int ł=0;ł<Ţ.Count;++ł){if(Ţ[ł
].CustomName.Contains("unused")||Ţ[ł].CustomData.Contains("unused"))continue;if(!ţ){ʴ="Wico "+Ţ[ł].CustomName.Split('!')[
0].Trim();ţ=true;}}return"A"+Ţ.Count.ToString("0");}void Ŋ(){for(int w=0;w<Ţ.Count;w++){Ţ[w].Enabled=true;}}string ŀ="";
void Ŀ(){if(ε!=""){if(ŀ==ε){ε="";}ŀ=ε;}else ŀ="";}void ľ(){}bool Ľ(){return true;}void ļ(bool ĸ=false){if(Ţ.Count<1)ő();
foreach(var Ļ in Ţ){Ļ.Radius=200;}}void ĺ(float Ĺ=200,bool ĸ=false){if(Ţ.Count<1)ő();foreach(var Ķ in Ţ){{Ķ.Radius=Ĺ;Ķ.Enabled=
true;}}}Vector3D ķ(){if(Ţ.Count<1)ő();foreach(var Ķ in Ţ){return Ķ.GetPosition();}Vector3D ĵ=new Vector3D();return ĵ;}float
Ł=float.MaxValue;void Ĵ(bool ĸ=false,float Ő=float.MaxValue){if(Ő<200)Ő=200;Ł=Ő;ŏ(ĸ);}void ŏ(bool ĸ=false){if(Ţ==null||Ţ.
Count<1)ő();foreach(var Ļ in Ţ){{float Ŏ=Ļ.GetMaximum<float>("Radius");if(Ł<Ŏ)Ŏ=Ł;Ļ.Radius=Ŏ;Ļ.Enabled=true;}}}int ō(){if(Ţ.
Count<1)ő();return(Ţ.Count);}List<string>Ō=new List<string>();void ŋ(){}void ŉ(string Ň,string ņ){IGC.SendBroadcastMessage(Ň,
ņ);}void ŉ(long ň,string Ň,string ņ){IGC.SendUnicastMessage(ň,Ň,ņ);}List<string>Ņ=new List<string>();void ń(){if(Ņ.Count>
0){if(ε==""){ε=Ņ[0];Ņ.RemoveAt(0);}else Echo("Waiting for message to be processed");ŧ();}if(Ņ.Count>0){}}void Ń(string ņ)
{Echo("RECEIVE:\n"+ņ);Ņ.Add(ņ);ń();}void č(){if(Ţ.Count>0){Echo(Ņ.Count+" Pending Incoming Messages");for(int w=0;w<Ņ.
Count;w++)Echo(w+":"+Ņ[w]);}else Echo("No antennas found");}List<IMyTerminalBlock>É=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>È=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ç=new List<IMyTerminalBlock>();bool Æ=false;string Å="[BASE]";
string Ä="[DOCK]";string Ã="CONNECTORS";void Â(Ǫ F){F.Ų(Ã,"BaseConnector",ref Å,true);F.Ų(Ã,"DockConnector",ref Ä,true);}
string Á(){Æ=false;É.Clear();È.Clear();Ç.Clear();À();return"CL"+É.Count.ToString()+"CD"+È.Count.ToString()+"CB"+Ç.Count.
ToString();}void À(){if(É.Count<1&&!Æ)É=ǖ<IMyShipConnector>();if(È.Count<1&&!Æ)È=Ǘ<IMyShipConnector>(Ä);if(È.Count<1&&!Æ)È=É;if(
Ç.Count<1&&!Æ)Ç=Ǘ<IMyShipConnector>(Å);Æ=true;return;}bool º(){return È.Count>1;}bool µ(){À();for(int w=0;w<È.Count;w++){
var ª=È[w]as IMyShipConnector;if(ª==null)continue;if(ª.Status==MyShipConnectorStatus.Connectable)return true;}return false;
}bool Ê(){À();for(int w=0;w<È.Count;w++){var ª=È[w]as IMyShipConnector;if(ª==null)continue;if(ª.Status==
MyShipConnectorStatus.Connected){var Î=ª.OtherConnector;if(Î.CubeGrid==ª.CubeGrid){continue;}else return true;}}return false;}void Ö(){for(
int w=0;w<È.Count;w++){var ª=È[w]as IMyShipConnector;if(ª==null)continue;}}IMyTerminalBlock Õ(){À();if(È.Count>0){return È[
0];}return null;}IMyTerminalBlock Ô(bool Ó=false){À();for(int w=0;w<È.Count;w++){var ª=È[w]as IMyShipConnector;if(ª==null
)continue;if(ª.Status==MyShipConnectorStatus.Connected){var Î=ª.OtherConnector;if(Î.CubeGrid==ª.CubeGrid){continue;}else{
if(!Ó){return ª.OtherConnector;}else{return È[w];}}}}return null;}void Ñ(bool Ð=true,bool Ï=true){À();for(int w=0;w<È.
Count;w++){var ª=È[w]as IMyShipConnector;if(ª==null)continue;if(ª.Status==MyShipConnectorStatus.Connected){var Î=ª.
OtherConnector;if(Î.CubeGrid==ª.CubeGrid){continue;}}if(Ð){if(ª.Status==MyShipConnectorStatus.Connectable)ª.Connect();}else{if(ª.
Status==MyShipConnectorStatus.Connected)ª.Disconnect();}ª.Enabled=Ï;}return;}Dictionary<string,int>Í=new Dictionary<string,int
>();string Ì="";UpdateFrequency Ò=UpdateFrequency.Once;bool u=true;bool d=true;bool O=false;bool N=false;double M=5;
double L=-1;double K=5;double J=-1;float I=100;string H="WORLD";void G(Ǫ F){F.Ų(H,"MaxWorldMps",ref I,true);}string E=
"WICOCRAFT";Program(){ə();Ǫ D=new Ǫ(this,Me.CustomData);D.Ų(E,"EchoOn",ref P,true);D.Ų(E,"DebugUpdate",ref O,true);D.Ų(E,
"SubModules",ref u,true);D.Ų(E,"SubmoduleTriggerWait",ref M,true);A=Echo;Echo=R;G(D);ą(D);ȷ(D);ř(D);ɘ(D);if(D.Ǻ){Me.CustomData=D.Ʒ(
true);}Ì=ʴ+":"+ʳ+" V"+ʲ+" ";A(Ì+"Creator");ɶ();ɻ("clear",ȹ,true);if(!Ŭ(œ)){Runtime.UpdateFrequency|=UpdateFrequency.
Update100;}if(!Me.Enabled){Echo("I am turned OFF!");}IMyTextSurface C=Me.GetSurface(0);C.ContentType=VRage.Game.GUI.TextPanel.
ContentType.TEXT_AND_IMAGE;C.WriteText("Wicorel\n"+ʳ);C.FontSize=2;C.Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
IMyTextSurface B=Me.GetSurface(1);B.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;B.WriteText("Version:"+ʲ);B.
Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;B.TextPadding=0.25f;B.FontSize=3.5f;}bool P=true;Action<string>A;void R(
string q){if(P)A(q);}bool o=false;bool n=false;string m="";int l=0;string k="";bool j=false;bool h=false;bool p=false;bool e=
false;double Z=5;double Y=-1;double X=3;double W=-1;double V=-1;double U=-2;void Main(string Q,UpdateType S){Echo(Ì+Ŗ());if(O
){Echo(S.ToString()+" : "+(int)S);}h=false;p=false;if(Y>Z){Y=0;e=false;var Ø=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyProjector>(Ø,Ǒ);for(int w=0;w<Ø.Count;w++){if(Ø[w].IsWorking){if(Ø[w].CustomName.Contains("!WCC")||Ø
[w].CustomData.Contains("!WCC"))continue;Echo("Working local Projector found!");e=true;}}}else{if(Y<0){Y=Z+5;}Y+=Runtime.
TimeSinceLastRun.TotalSeconds;}γ="";double ü=0;if(ǌ is IMyShipController){if(W>X||!o){W=0;MyShipMass å;å=((IMyShipController)ǌ).
CalculateShipMass();ü=å.BaseMass;if(ü!=ɗ&&ɗ>0){Echo("MASS CHANGE");ɻ(ʴ+":"+ʳ+":MASS CHANGE",ȹ,true);{o=false;m="";J=0;}}}else{if(W<0){W=X
+5;}W+=Runtime.TimeSinceLastRun.TotalSeconds;ü=ɗ;}}else{ɗ=ü=-1;if(j&&!n){o=false;m="";J=0;}}if(J>K){J=0;if(j){Q="init";
Echo("RESCAN!");J=0;}}else{if(j){Echo("Waiting for Rescan:"+J.ToString("0.0")+"("+K.ToString("0.0")+")");J+=Runtime.
TimeSinceLastRun.TotalSeconds;}}if((Q=="init"&&l==0)||(Math.Abs(ü-ɗ)>1&&ɗ>0&&l==0)){ʐ("INIT or GRID/MASS CHANGE!");Echo(
"Arg init or grid/mass change!");m="";J=K+5;o=false;l=0;k="";γ="init";}ʐ("clear");if(!o){if(e){ʐ(
"Construction in Progress\nTurn off projector to continue");ɻ("Construction in Progress\nTurn off projector to continue",Ȱ);}else{}h=true;if(l==0){j=false;k="";}ɖ();if(j)h=false;
n=true;if(o){Q="";J=0;}}else{if(u)ͼ();γ=Q;if(n){ɻ(DateTime.Now.ToString()+" "+m,ȹ,true);}if(ǌ!=null){}if(ǌ is
IMyShipController){V=((IMyShipController)ǌ).GetShipSpeed();Vector3D û=((IMyShipController)ǌ).GetNaturalGravity();double ú=û.Length();U=ú/
9.81;}else{U=-1.0;}if((S&(UpdateType.Trigger|UpdateType.Terminal))>0||(S&(UpdateType.Trigger))>0||(S&(UpdateType.Terminal))>
0||(S&(UpdateType.Mod))>0||(S&(UpdateType.Script))>0){if(ς(Q)){ΰ();ɼ();return;}}else if((S&(UpdateType.IGC))>0){if(!ϊ(Q))
{Ń(Q);}ΰ();ŧ();ɼ();return;}else{Q="";}ń();ŋ();ʬ();ͳ();}if(u)ΰ();if(u){if((Ϋ==null)){}else{if((S&(UpdateType.Trigger|
UpdateType.Terminal))>0||(S&(UpdateType.Mod))>0||(S&(UpdateType.Script))>0||L>M||n){L=0;Ŭ(Ŕ);}else{L+=Runtime.TimeSinceLastRun.
TotalSeconds;}}}else Echo("Submodules turned off");if(h){Echo("FAST!");Runtime.UpdateFrequency|=Ò;}else{Runtime.UpdateFrequency&=~(Ò
);}if(p){Echo("MEDIUM");Runtime.UpdateFrequency|=UpdateFrequency.Update10;}else{Runtime.UpdateFrequency&=~(
UpdateFrequency.Update10);}if(d)Echo(Ί());ʩ();ɼ();n=false;}void ù(string Ì=null){float ø=0;ø=Runtime.CurrentInstructionCount/(float)
Runtime.MaxInstructionCount;if(Ì==null)Ì="Instructions=";Echo(Ì+(ø*100).ToString("0.00")+"%");}List<IMyTerminalBlock>ý=new List
<IMyTerminalBlock>();string ö(){List<IMyTerminalBlock>ô=new List<IMyTerminalBlock>();ý.Clear();ô=ǖ<IMyShipDrill>();
foreach(var ñ in ô)ý.Add(ñ as IMyTerminalBlock);return"D"+ý.Count.ToString("00");}void ó(){foreach(IMyFunctionalBlock ñ in ý){ñ
.Enabled=true;}}void ò(){if(ý.Count<1)ö();foreach(IMyFunctionalBlock ñ in ý){ñ.Enabled=false;}}bool ð(){if(ý.Count<1)ö();
if(ý.Count<1)return false;return true;}List<IMyTerminalBlock>õ=new List<IMyTerminalBlock>();string þ(){List<
IMyTerminalBlock>ô=new List<IMyTerminalBlock>();õ.Clear();ô=Ǘ<IMyShipConnector>("Ejector");foreach(var ñ in ô)õ.Add(ñ as
IMyTerminalBlock);return"E"+õ.Count.ToString("00");}void ċ(){if(õ.Count<1)þ();foreach(IMyFunctionalBlock ñ in õ){if(!ñ.Enabled)ñ.Enabled
=true;}}void Ċ(){if(õ.Count<1)þ();foreach(IMyFunctionalBlock ñ in õ){if(ñ.Enabled)ñ.Enabled=false;}}string ĉ="NOFOLLOW";
string Ĉ="!WCC";string ć="[NAV]";string Č="Craft Remote Control";string Ć="GRIDS";void ą(Ǫ F){F.Ų(Ć,"NoFollow",ref ĉ,true);F.Ų
(Ć,"BlockIgnore",ref Ĉ,true);F.Ų(Ć,"OrientationBlockContains",ref ć,true);F.Ų(Ć,"OrientationBlockNamed",ref Č,true);}List
<IMyTerminalBlock>Ą=new List<IMyTerminalBlock>();List<IMyTextPanel>ă=new List<IMyTextPanel>();List<IMyTextPanel>Ă=new
List<IMyTextPanel>();List<IMyTerminalBlock>ā=new List<IMyTerminalBlock>();List<IMyCubeGrid>Ā=new List<IMyCubeGrid>();List<
IMyCubeGrid>ÿ=new List<IMyCubeGrid>();List<IMyCubeGrid>ï=new List<IMyCubeGrid>();List<IMyCubeGrid>ã=new List<IMyCubeGrid>();bool ê(
){List<IMyTerminalBlock>á=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(á);if(Ψ!=á.
Count){return true;}return false;}string à(){Ą.Clear();ã.Clear();Ā.Clear();ÿ.Clear();ï.Clear();ă.Clear();Ă.Clear();ā.Clear();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Ą);Ψ=Ą.Count;foreach(var ß in Ą){var Ù=ß.CubeGrid;if(!ã.Contains(Ù)){ã.Add(Ù);}}Ú(Me.
CubeGrid);foreach(var Ù in ã){if(Ā.Contains(Ù))continue;bool Þ=false;List<IMyShipConnector>Ý=new List<IMyShipConnector>();
GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(Ý,(Ë=>Ë.CubeGrid==Ù));foreach(var Ü in Ý){if(Ü.Status==MyShipConnectorStatus.
Connected){if(Ā.Contains(Ü.OtherConnector.CubeGrid)||ÿ.Contains(Ü.OtherConnector.CubeGrid)){continue;}if(Ā.Contains(Ü.
OtherConnector.CubeGrid))Þ=true;else Þ=false;}}if(Þ){if(!ï.Contains(Ù)){ï.Add(Ù);}}if(!ÿ.Contains(Ù)){ÿ.Add(Ù);}}string Û="";Û+="B"+Ą.
Count.ToString();Û+="G"+ã.Count.ToString();Û+="L"+Ā.Count.ToString();Û+="D"+ï.Count.ToString();Û+="R"+ÿ.Count.ToString();Echo
("Found "+ã.Count.ToString()+" Grids");Echo("Found "+Ā.Count.ToString()+" Local Grids");for(int w=0;w<Ā.Count;w++)Echo(
"|"+Ā[w].CustomName);Echo("Found "+ï.Count.ToString()+" Docked Grids");for(int w=0;w<ï.Count;w++)Echo("|"+ï[w].CustomName);
Echo("Found "+ÿ.Count.ToString()+" Remote Grids");for(int w=0;w<ÿ.Count;w++)Echo("|"+ÿ[w].CustomName);return Û;}void Ú(
IMyCubeGrid Ù){if(Ù==null)return;if(!Ā.Contains(Ù)){Ā.Add(Ù);â(Ù);í(Ù);é(Ù);ǔ(Ù);}}void â(IMyCubeGrid Ù){List<IMyMotorStator>ä=new
List<IMyMotorStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(ä,(î=>î.TopGrid==Ù));foreach(var è in ä){if(è.
CustomName.Contains(ĉ)||è.CustomData.Contains(ĉ))continue;Ú(è.CubeGrid);}List<IMyMotorAdvancedStator>æ=new List<
IMyMotorAdvancedStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(æ,(î=>î.TopGrid==Ù));foreach(var è in æ){if(è.CustomName.
Contains(ĉ)||è.CustomData.Contains(ĉ))continue;Ú(è.CubeGrid);}}void í(IMyCubeGrid Ù){List<IMyPistonBase>ì=new List<IMyPistonBase
>();GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(ì,(î=>î.TopGrid==Ù));foreach(var ë in ì){Ú(ë.CubeGrid);}}void é(
IMyCubeGrid Ù){List<IMyMotorStator>ä=new List<IMyMotorStator>();GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(ä,(Ë=>Ë.CubeGrid
==Ù));foreach(var è in ä){if(è.CustomName.Contains(ĉ)||è.CustomData.Contains(ĉ))continue;IMyCubeGrid ç=è.TopGrid;if(ç!=
null&&ç!=Ù){Ú(ç);}}ä.Clear();List<IMyMotorAdvancedStator>æ=new List<IMyMotorAdvancedStator>();GridTerminalSystem.
GetBlocksOfType<IMyMotorAdvancedStator>(æ,(Ë=>Ë.CubeGrid==Ù));foreach(var è in æ){if(è.CustomName.Contains(ĉ)||è.CustomData.Contains(ĉ)
)continue;IMyCubeGrid ç=è.TopGrid;if(ç!=null&&ç!=Ù){Ú(ç);}}}void ǔ(IMyCubeGrid Ù){List<IMyPistonBase>ì=new List<
IMyPistonBase>();GridTerminalSystem.GetBlocksOfType<IMyPistonBase>(ì,(Ë=>Ë.CubeGrid==Ù));foreach(var ë in ì){IMyCubeGrid ç=ë.TopGrid;
if(ç!=null&&ç!=Ù){if(!Ā.Contains(ç)){Ú(ç);}}}}List<IMyCubeGrid>Ǔ(){if(Ā.Count<1){à();}return Ā;}List<IMyCubeGrid>ǒ(){if(Ā.
Count<1){à();}return ï;}bool Ǒ(IMyTerminalBlock ß){return Ǔ().Contains(ß.CubeGrid);}bool ǐ(long Ǐ){for(int ł=0;ł<Ā.Count;ł++)
{if((long)Ā[ł].EntityId==Ǐ)return true;}return false;}bool ǐ(IMyCubeGrid Ǐ){return Ǔ().Contains(Ǐ);}bool ǎ(
IMyTerminalBlock ß){var Ǎ=ǒ();if(Ǎ==null)return false;return Ǎ.Contains(ß.CubeGrid);}void Ǖ(){if(Ą.Count<1)à();ā.Clear();foreach(var ǚ
in Ą){if(Ǒ(ǚ)&&!(ǚ.CustomName.Contains(Ĉ)))ā.Add(ǚ);}}IMyTerminalBlock Ǚ(string ǘ){IMyTerminalBlock ß;ß=(IMyTerminalBlock)
GridTerminalSystem.GetBlockWithName(ǘ);if(ß==null)throw new Exception(ǘ+" Not Found");return ß;}List<ƻ>ǖ<ƻ>(ref List<ƻ>ô,string ƺ=null)
where ƻ:class{if(ô==null)ô=new List<ƻ>();else ô.Clear();if(ā.Count<1)Ǖ();for(int ƹ=0;ƹ<ā.Count;ƹ++){if(ā[ƹ]is ƻ&&((ƺ==null)||
(ƺ!=null&&ā[ƹ].CustomName.StartsWith(ƺ)))){ô.Add((ƻ)ā[ƹ]);}}return ô;}List<IMyTerminalBlock>ǖ<ƻ>(ref List<
IMyTerminalBlock>ô,string ƺ=null)where ƻ:class{if(Ą.Count<1)à();if(ô==null)ô=new List<IMyTerminalBlock>();else ô.Clear();if(ā.Count<1)Ǖ(
);for(int ƹ=0;ƹ<ā.Count;ƹ++){if(ā[ƹ]is ƻ&&((ƺ==null)||(ƺ!=null&&ā[ƹ].CustomName.StartsWith(ƺ)))){ô.Add(ā[ƹ]);}}return ô;}
List<IMyTerminalBlock>ǖ<ƻ>(string ƺ=null)where ƻ:class{var ô=new List<IMyTerminalBlock>();ǖ<ƻ>(ref ô,ƺ);return ô;}List<
IMyTerminalBlock>Ǘ<ƻ>(string ƺ=null)where ƻ:class{var ô=new List<IMyTerminalBlock>();if(ā.Count<1)Ǖ();for(int ƹ=0;ƹ<ā.Count;ƹ++){if(ā[ƹ]
is ƻ&&ƺ!=null&&(ā[ƹ].CustomName.Contains(ƺ)||ā[ƹ].CustomData.Contains(ƺ))){ô.Add(ā[ƹ]);}}return ô;}List<IMyTextPanel>ƿ(
string ƺ=null){if(Ą.Count<1)à();var ô=new List<IMyTextPanel>();if(ă.Count>1){foreach(var ƽ in ă){if(ƺ!=null&&(ƽ.CustomName.
Contains(ƺ)||ƽ.CustomData.Contains(ƺ)))ô.Add(ƽ);}}else{foreach(var ƽ in Ą){if(ƽ is IMyTextPanel&&Ǒ(ƽ)&&!(ƽ.CustomName.Contains(Ĉ
)||ƽ.CustomData.Contains(Ĉ))){if(ƺ!=null&&(ƽ.CustomName.Contains(ƺ)||ƽ.CustomData.Contains(ƺ)))ô.Add(ƽ as IMyTextPanel);ă
.Add(ƽ as IMyTextPanel);}}}return ô;}List<IMyTextPanel>ƾ(string ƺ=null){if(ā.Count<1)Ǖ();var ô=new List<IMyTextPanel>();
if(Ă.Count>1){foreach(var ƽ in Ă){if(ƺ!=null&&(ƽ.CustomName.Contains(ƺ)||ƽ.CustomData.Contains(ƺ)))ô.Add(ƽ);}}else{foreach
(var ƽ in ā){if(ƽ is IMyTextPanel&&Me.CubeGrid==ƽ.CubeGrid){if(ƺ!=null&&(ƽ.CustomName.Contains(ƺ)||ƽ.CustomData.Contains(
ƺ)))ô.Add(ƽ as IMyTextPanel);Ă.Add(ƽ as IMyTextPanel);}}}return ô;}List<IMyTerminalBlock>Ƽ<ƻ>(string ƺ=null)where ƻ:class
{if(ā.Count<1)Ǖ();var ô=new List<IMyTerminalBlock>();for(int ƹ=0;ƹ<ā.Count;ƹ++){if(ā[ƹ]is ƻ&&Me.CubeGrid==ā[ƹ].CubeGrid&&
ƺ!=null&&(ā[ƹ].CustomName.Contains(ƺ)||ā[ƹ].CustomData.Contains(ƺ))){ô.Add(ā[ƹ]);}}return ô;}List<IMyTerminalBlock>ǁ<ƻ>(
string ƺ=null)where ƻ:class{if(ā.Count<1)Ǖ();var ô=new List<IMyTerminalBlock>();for(int ƹ=0;ƹ<ā.Count;ƹ++){if(ā[ƹ]is ƻ&&ƺ!=
null&&ā[ƹ].CustomName==ƺ){ô.Add(ā[ƹ]);}}return ô;}IMyTerminalBlock ǌ=null;string ǋ(){string m="";var Ǌ=new List<
IMyTerminalBlock>();ǖ<IMyTerminalBlock>(ref Ǌ,Č);if(Ǌ.Count==0){Ǌ=Ǘ<IMyRemoteControl>(ć);if(Ǌ.Count==0){ǖ<IMyRemoteControl>(ref Ǌ);if(Ǌ.
Count==0){ǖ<IMyCockpit>(ref Ǌ);int w=0;for(;w<Ǌ.Count;w++){Echo("Checking Controller:"+Ǌ[w].CustomName);if(Ǌ[w]is
IMyCryoChamber)continue;break;}if(w>=Ǌ.Count){m+="!!NO valid Controller:"+w+"\n";Echo("No Controller found");}else{m+="S";Echo(
"Using good ship Controller: "+Ǌ[w].CustomName);}}else{m+="R";Echo("Using First Remote control found: "+Ǌ[0].CustomName);}}}else{m+="N";Echo(
"Using Named: "+Ǌ[0].CustomName);}if(Ǌ.Count>0)ǌ=Ǌ[0];return m;}string ǉ="!NAV";void ǈ(Ǫ F){F.Ų(Ć,"GyroIgnore",ref ǉ,true);F.Ų(Ć,
"LIMIT_GYROS",ref ǆ,true);F.Ų(Ć,"LEAVE_GYROS",ref ǅ,true);F.Ų(Ć,"CTRL_COEFF",ref Ǉ,true);}double Ǉ=0.9;int ǆ=99;int ǅ=-1;
IMyShipController Ǆ;List<IMyGyro>ǃ=new List<IMyGyro>();float ǀ=0.01f;bool ǂ(string Ǜ){if(Ǆ==null)ș();if(Ǆ is IMyShipController){Vector3D
ȉ=(Ǆ as IMyShipController).GetNaturalGravity();return ǂ(Ǜ,ȉ,ǌ);}else{Echo("No Controller for gravity");}return true;}bool
ǂ(string Ǜ,Vector3D Ȉ,IMyTerminalBlock ȇ){bool Ȇ=true;if(Ǆ==null)ș();Matrix ȅ;ȇ.Orientation.GetMatrix(out ȅ);Vector3D Ȅ;Ǜ
=Ǜ.ToLower();if(Ǜ.Contains("rocket"))Ȅ=ȅ.Backward;else if(Ǜ.Contains("up"))Ȅ=ȅ.Up;else if(Ǜ.Contains("backward"))Ȅ=ȅ.
Backward;else if(Ǜ.Contains("forward"))Ȅ=ȅ.Forward;else if(Ǜ.Contains("right"))Ȅ=ȅ.Right;else if(Ǜ.Contains("left"))Ȅ=ȅ.Left;
else Ȅ=ȅ.Down;Ȉ.Normalize();for(int ł=0;ł<ǃ.Count;++ł){var Ǎ=ǃ[ł];Ǎ.Orientation.GetMatrix(out ȅ);var Ȃ=Vector3D.Transform(Ȅ,
MatrixD.Transpose(ȅ));var ȁ=Vector3D.Transform(Ȉ,MatrixD.Transpose(Ǎ.WorldMatrix.GetOrientation()));var Ȁ=Vector3D.Cross(Ȃ,ȁ);
double ǿ=Vector3D.Dot(Ȃ,ȁ);double Ǿ=Ȁ.Length();Ǿ=Math.Atan2(Ǿ,Math.Sqrt(Math.Max(0.0,1.0-Ǿ*Ǿ)));if(ǿ<0)Ǿ=Math.PI-Ǿ;if(Ǿ<ǀ){Ǎ.
GyroOverride=false;continue;}float ǽ=(float)(2*Math.PI);double Ǽ=ǽ*(Ǿ/Math.PI)*Ǉ;Ǽ=Math.Min(ǽ,Ǽ);Ǽ=Math.Max(0.01,Ǽ);Ȁ.Normalize();Ȁ
*=Ǽ;float ȃ=-(float)Ȁ.X;if(Math.Abs(Ǎ.Pitch-ȃ)>0.01)Ǎ.Pitch=ȃ;float Ȋ=-(float)Ȁ.Y;if(Math.Abs(Ǎ.Yaw-Ȋ)>0.01)Ǎ.Yaw=Ȋ;float
ȓ=-(float)Ȁ.Z;if(Math.Abs(Ǎ.Roll-ȓ)>0.01)Ǎ.Roll=ȓ;Ǎ.GyroOverride=true;Ȇ=false;}return Ȇ;}string ș(){string Û="";var Ș=new
List<IMyTerminalBlock>();Ǆ=ǌ as IMyShipController;ǃ.Clear();if(Ǆ==null){if(Ș.Count<1)return"No RC!";}ȕ();GridTerminalSystem.
GetBlocksOfType<IMyGyro>(Ș,î=>î.CubeGrid==ǌ.CubeGrid);int ȗ=0;for(int ł=0;ł<Ș.Count;ł++){if(Ș[ł].CustomName.Contains(ǉ)||Ș[ł].
CustomData.Contains(ǉ)){ȗ++;continue;}ǃ.Add(Ș[ł]as IMyGyro);}if(ǆ>0){if(ǃ.Count>ǆ){ǃ.RemoveRange(ǆ,ǃ.Count-ǆ);}else{if((ǅ-ȗ)>0){
int Ȗ=ǃ.Count-(ǅ-ȗ);ǃ.RemoveRange(Ȗ,(ǅ-ȗ));}}}ȕ();Û+="GYRO#"+ǃ.Count.ToString("00")+"#";return Û;}void ȕ(){if(ǃ!=null){for(
int ł=0;ł<ǃ.Count;++ł){ǃ[ł].GyroOverride=false;ǃ[ł].Enabled=true;}}}bool Ȕ(Vector3D Ȓ,Vector3D ȑ,IMyTerminalBlock Ȑ){bool ȏ
=false;Vector3D Ȏ=(ȑ-Ȓ);Vector3D ȍ;if(Ȑ is IMyShipController){ȍ=((IMyShipController)Ȑ).CenterOfMass;}else{ȍ=Ȑ.GetPosition
();}Vector3D Ȍ=(ȑ-ȍ);Vector3D ȋ=Ǭ(Ȏ,Ȍ);Vector3D ǻ=(ȑ-ȋ*2)-ȍ;ȏ=ǂ("forward",ǻ,Ȑ);return ȏ;}Vector3D Ǭ(Vector3D Ļ,Vector3D ñ
){if(Vector3D.IsZero(ñ))return Vector3D.Zero;return Ļ-Ļ.Dot(ñ)/ñ.LengthSquared()*ñ;}class Ǫ{char ǩ='[';char Ǩ=']';string
ǧ=";";string Ǧ="";public bool ǥ=false;public string Ǥ="";string ǣ="---";char Ǣ='|';private MyGridProgram ǡ;private
Dictionary<string,string>Ǡ;private Dictionary<string,string[]>ǟ;private Dictionary<string,Dictionary<string,string>>Ǟ;private
string ǝ="";static string[]ǫ={"true","yes","on","1"};const StringComparison ǜ=StringComparison.OrdinalIgnoreCase;const char ǭ=
'=';public bool Ǻ{get;private set;}=false;public Ǫ(MyGridProgram Ǹ,string Ƕ){ǡ=Ǹ;Ǡ=new Dictionary<string,string>();ǟ=new
Dictionary<string,string[]>();Ǟ=new Dictionary<string,Dictionary<string,string>>();Ƿ(Ƕ);}public int Ƿ(string Ƕ){Ƕ.TrimEnd();if(ǝ==
Ƕ){return Ǡ.Count;}Ǡ.Clear();ǟ.Clear();Ǟ.Clear();Ǧ="";Ǥ="";Ǻ=false;ǝ=Ƕ;string[]ǵ=Ƕ.Split('\n');for(int Ǵ=0;Ǵ<ǵ.Count();Ǵ
++){string ǹ="";ǵ[Ǵ].Trim();if(ǵ[Ǵ].StartsWith(ǩ.ToString())){string ǳ="";for(int ǲ=1;ǲ<ǵ[Ǵ].Length;ǲ++)if(ǵ[Ǵ][ǲ]==Ǩ)
break;else ǳ+=ǵ[Ǵ][ǲ];if(ǳ!=""){ǹ=ǳ.ToUpper();}else continue;Ǵ++;string Ƃ="";var Ǳ=new string[ǵ.Count()-Ǵ];int ǰ=0;var ż=new
Dictionary<string,string>();for(;Ǵ<ǵ.Count();Ǵ++){ǵ[Ǵ].Trim();if(ǵ[Ǵ].StartsWith(ǩ.ToString())||ǵ[Ǵ].StartsWith(ǣ)){Ǵ--;break;}Ƃ+=
ǵ[Ǵ]+"\n";Ǳ[ǰ++]=ǵ[Ǵ];if(ǵ[Ǵ].Contains(ǭ)){string[]ǯ=ǵ[Ǵ].Split('=');if(ǯ.Count()>1){string Ÿ=ǯ[0];string Ǯ="";for(int ł=
1;ł<ǯ.Count();ł++){Ǯ+=ǯ[ł];if(ł+1<ǯ.Count())Ǯ+=ǭ;}if(Ǯ==""){int Ƹ=Ǵ+1;for(;Ƹ<ǯ.Count();Ƹ++){ǵ[Ƹ].Trim();if(ǵ[Ƹ].Length>1
&&ǵ[Ƹ][0]==Ǣ){Ǯ+=ǵ[Ƹ].Substring(1).Trim()+"\n";break;}}Ǵ=Ƹ;}ż.Add(Ÿ,Ǯ);}}else if(ǵ[Ǵ].StartsWith(ǧ)){}}if(!Ǟ.ContainsKey(ǹ
)){Ǟ.Add(ǹ,ż);if(!ǟ.ContainsKey(ǹ))ǟ.Add(ǹ,Ǳ);}else{}if(!Ǡ.ContainsKey(ǹ)){Ǡ.Add(ǹ,Ƃ);}else{Ǻ=true;}}else if(ǵ[Ǵ].
StartsWith(ǣ)){Ǵ++;for(;Ǵ<ǵ.Count();Ǵ++){Ǥ+=ǵ[Ǵ];}}else{Ǧ+=ǵ[Ǵ]+"\n";}}return Ǡ.Count;}public string ƣ(string ű){string Ƃ="";if(Ǡ.
ContainsKey(ű))Ƃ=Ǡ[ű];return Ƃ;}public string[]Ɓ(string ű){string[]ƀ={""};if(ǟ.ContainsKey(ű))ƀ=ǟ[ű];return ƀ;}public bool Ų(string
ű,string Ÿ,ref string ſ,bool Ŷ=false){ű=ű.ToUpper();if(Ǟ.ContainsKey(ű)){var Ż=Ǟ[ű];if(Ż.ContainsKey(Ÿ)){ſ=Ż[Ÿ];return
true;}}if(Ŷ)ź(ű,Ÿ,ſ);return false;}public bool Ų(string ű,string Ÿ,ref long ž,bool Ŷ=false){string ŵ="";if(!Ų(ű,Ÿ,ref ŵ)){if
(Ŷ){ź(ű,Ÿ,ž);}return false;}ž=Convert.ToInt64(ŵ);return true;}public bool Ų(string ű,string Ÿ,ref int ƅ,bool Ŷ=false){
string ŵ="";if(!Ų(ű,Ÿ,ref ŵ)){if(Ŷ){ź(ű,Ÿ,ƅ);}return false;}ƅ=Convert.ToInt32(ŵ);return true;}public bool Ų(string ű,string Ÿ,
ref double Ɔ,bool Ŷ=false){string ŵ="";if(!Ų(ű,Ÿ,ref ŵ)){if(Ŷ){ź(ű,Ÿ,Ɔ);}return false;}bool Ƅ=double.TryParse(ŵ,out Ɔ);
return true;}public bool Ų(string ű,string Ÿ,ref float ƃ,bool Ŷ=false){string ŵ="";if(!Ų(ű,Ÿ,ref ŵ)){if(Ŷ){ź(ű,Ÿ,ƃ.ToString())
;}return false;}bool Ƅ=float.TryParse(ŵ,out ƃ);return true;}public bool Ų(string ű,string Ÿ,ref DateTime Ź,bool Ŷ=false){
string ŵ="";if(!Ų(ű,Ÿ,ref ŵ)){if(Ŷ){ź(ű,Ÿ,Ź);}return false;}Ź=DateTime.Parse(ŵ);return true;}public bool Ų(string ű,string Ÿ,
ref Vector3D ŷ,bool Ŷ=false){string ŵ="";if(!Ų(ű,Ÿ,ref ŵ)){if(Ŷ){ź(ű,Ÿ,ŷ);}return false;}double Ë,Ŵ,ų;Ư(ŵ,out Ë,out Ŵ,out ų
);ŷ.X=Ë;ŷ.Y=Ŵ;ŷ.Z=ų;return true;}public bool Ų(string ű,string Ÿ,ref bool Ű,bool Ŷ=false){string ŵ="";if(!Ų(ű,Ÿ,ref ŵ)){
if(Ŷ){ź(ű,Ÿ,Ű);}return false;}Ű=ǫ.Any(Ž=>string.Equals(ŵ,Ž,ǜ));return true;}public bool ź(string ű,string Ÿ,string ŵ){if(Ǡ
.ContainsKey(ű)){Ǡ[ű]="";}else{Ǡ.Add(ű,"");Ǻ=true;}if(Ǟ.ContainsKey(ű)){var ż=new Dictionary<string,string>();var Ż=Ǟ[ű];
if(Ż.ContainsKey(Ÿ)){if(Ż[Ÿ]==ŵ)return false;Ż[Ÿ]=ŵ;}else{Ż.Add(Ÿ,ŵ);}Ǻ=true;}else{var ż=new Dictionary<string,string>();ż
.Add(Ÿ,ŵ);Ǟ.Add(ű,ż);Ǻ=true;}return true;}public bool ź(string ű,string Ÿ,Vector3D ŷ){ź(ű,Ÿ,ƨ(ŷ));return true;}public
bool ź(string ű,string Ÿ,bool Ű){ź(ű,Ÿ,Ű.ToString());return true;}public bool ź(string ű,string Ÿ,int Ʀ){ź(ű,Ÿ,Ʀ.ToString())
;return true;}public bool ź(string ű,string Ÿ,long ƥ){ź(ű,Ÿ,ƥ.ToString());return true;}public bool ź(string ű,string Ÿ,
DateTime Ź){ź(ű,Ÿ,Ź.ToString());return true;}public bool ź(string ű,string Ÿ,float ƃ){ź(ű,Ÿ,ƃ.ToString());return true;}public
bool ź(string ű,string Ÿ,double Ɔ){ź(ű,Ÿ,Ɔ.ToString());return true;}public void Ƥ(string ű,string Ƃ){Ƃ.TrimEnd();ű=ű.ToUpper
();if(Ǡ.ContainsKey(ű)){if(Ǡ[ű]!=Ƃ){Ǡ[ű]=Ƃ;Ǻ=true;}}else{Ǻ=true;Ǡ.Add(ű,Ƃ);}}public string Ʒ(bool Ƶ=true){string ƴ="";
string Ƴ=Ǧ.Trim();if(ǥ&&Ƴ!="")ƴ=Ƴ+"\n";foreach(var Ʋ in Ǡ){ƴ+=ǩ+Ʋ.Key.Trim()+Ǩ+"\n";if(Ʋ.Value.TrimEnd()==""){string Ʊ="";if(Ǟ
.ContainsKey(Ʋ.Key)){foreach(var ư in Ǟ[Ʋ.Key]){Ʊ+=ư.Key+ǭ+ư.Value+"\n";}}Ʊ+="\n";ƴ+=Ʊ;}else{ƴ+=Ʋ.Value.Trim()+"\n\n";}}
if(Ǥ!=""){ƴ+="\n"+ǣ+"\n";ƴ+=Ǥ+"\n";}if(Ƶ){Ǻ=false;ǝ=ƴ;}return ƴ;}bool Ư(string ƶ,out double î,out double Ʈ,out double ƭ){
string[]Ƭ=ƶ.Trim().Split(',');if(Ƭ.Length<3){Ƭ=ƶ.Trim().Split(':');}î=0;Ʈ=0;ƭ=0;if(Ƭ.Length<3)return false;bool ƫ=double.
TryParse(Ƭ[0].Trim(),out î);bool ƪ=double.TryParse(Ƭ[1].Trim(),out Ʈ);bool Ʃ=double.TryParse(Ƭ[2].Trim(),out ƭ);if(!ƫ||!ƪ||!Ʃ){
return false;}return true;}string ƨ(Vector3D Ƨ){string Û;Û=Ƨ.X.ToString("0.00")+":"+Ƨ.Y.ToString("0.00")+":"+Ƨ.Z.ToString(
"0.00");return Û;}}int Ɩ=80;int ƞ=20;double ƕ=0;string Ɣ="POWER";void Ɠ(Ǫ F){F.Ų(Ɣ,"batterypcthigh",ref Ɩ,true);F.Ų(Ɣ,
"batterypctlow",ref ƞ,true);}void ƒ(){ƕ=0;Echo("Init Reactors");χ();Echo("Init Solar");ϒ();Echo("Init Batteries");ʺ();if(ω>0)ƕ+=ω;if(ʽ>
0)ƕ+=ʽ;}List<IMyTerminalBlock>Ƒ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ɛ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ə=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ǝ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ƍ=new List<
IMyTerminalBlock>();List<IMyTerminalBlock>ƌ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>Ƌ=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ɗ=new List<IMyTerminalBlock>();string Ɖ(IMyTerminalBlock ƈ){Ƒ.Clear();Ɛ.Clear();Ə.Clear();Ǝ.Clear();ƍ.Clear();ƌ.Clear()
;Ƌ.Clear();Ɗ.Clear();ǖ<IMyMotorSuspension>(ref Ƒ);for(int w=0;w<Ƒ.Count;w++){if(Ƒ[w].CustomName.Contains("[SLED]")||Ƒ[w].
CustomData.Contains("[SLED]")){Ɛ.Add(Ƒ[w]);if(Ƒ[w].CustomName.Contains("[REAR]")||Ƒ[w].CustomData.Contains("[FRONT]")){Ə.Add(Ƒ[w])
;}if(Ƒ[w].CustomName.Contains("[FRONT]")||Ƒ[w].CustomData.Contains("[FRONT]")){Ǝ.Add(Ƒ[w]);}}else{if(Ƒ[w].CustomName.
Contains("[LEFT]")||Ƒ[w].CustomData.Contains("[LEFT]")){Ƌ.Add(Ƒ[w]);}else if(Ƒ[w].CustomName.Contains("[RIGHT]")||Ƒ[w].
CustomData.Contains("[RIGHT]")){Ɗ.Add(Ƒ[w]);}if(Ƒ[w].CustomName.Contains("[REAR]")||Ƒ[w].CustomData.Contains("[FRONT]")){ƍ.Add(Ƒ[w
]);}if(Ƒ[w].CustomName.Contains("[FRONT]")||Ƒ[w].CustomData.Contains("[FRONT]")){ƌ.Add(Ƒ[w]);}}}return"W"+Ƒ.Count.
ToString("0")+"WS"+Ɛ.Count.ToString("0")+"SR"+Ə.Count.ToString("0")+"SF"+Ǝ.Count.ToString("0");}bool Ƈ(){if(Ɛ.Count>0)return
true;return false;}void Ɨ(){foreach(var Ɲ in Ɛ){var Ƙ=Ɲ as IMyMotorSuspension;Ƙ.SetValueFloat("Friction",0);}}bool Ƣ(){if(Ƒ.
Count>0){return true;}return false;}bool ơ(float Ơ,float ƙ=-1){Echo("WPP:"+Ơ.ToString()+":"+ƙ.ToString());bool Ɵ=true;if(Ơ<0f
)Ơ=0f;if(Ơ>100f)Ơ=100f;foreach(var Ɲ in Ɗ){var Ƙ=Ɲ as IMyMotorSuspension;float Ɯ=Ƙ.GetValueFloat("Propulsion override");
Echo("CPower:"+Ɯ.ToString("0.00")+"\n"+Ƙ.CustomName);float ƛ=(Ɯ);ƛ=Math.Abs(ƛ);if(ƛ<1)ƛ*=100f;if(Ơ>(ƛ+5f)){Ɵ=false;ƛ+=5;}
else if(Ơ<(ƛ-5)){Ɵ=false;ƛ-=5;}else ƛ=Ơ;if(ƙ>=0)Ƙ.SetValueFloat("Friction",ƙ);Echo("Setting override to"+ƛ.ToString("0.000")
);Ƙ.SetValueFloat("Propulsion override",-ƛ);}foreach(var Ɲ in Ƌ){var Ƙ=Ɲ as IMyMotorSuspension;float Ɯ=Ƙ.GetValueFloat(
"Propulsion override");Echo("CPower:"+Ɯ.ToString("0.00")+"\n"+Ƙ.CustomName);float ƛ=(Ɯ);ƛ=Math.Abs(ƛ);if(ƛ<1)ƛ*=100f;if(Ơ>(ƛ+5f)){Ɵ=false;ƛ+=
5;}else if(Ơ<(ƛ-5)){Ɵ=false;ƛ-=5;}else ƛ=Ơ;if(ƙ>=0)Ƙ.SetValueFloat("Friction",ƙ);Echo("Setting override to"+(ƛ).ToString(
"0.000"));Ƙ.SetValueFloat("Propulsion override",ƛ);}return Ɵ;}void ƚ(float ƙ){foreach(var Ɲ in Ƒ){var Ƙ=Ɲ as IMyMotorSuspension
;Ƙ.SetValueFloat("Friction",ƙ);}}