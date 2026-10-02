/*
 * R e a d m e
 * -----------
 * 
 * A pretty basic script to count diffrent types of blocks.
 * This includes weapons, refineries, batteries, assemblers, reactors, h2/o2 generators, hydro engines and weapons and
 * All you need to is either choose an name for the lcd to display it on or use the default name for the lcd.
 * You use the const string to choose a name for the lcd.
 */

private const string LCD_NAME = "BlockCounter";
Program(){Runtime.UpdateFrequency=UpdateFrequency.Update10;}void Main(){var Y="MyObjectBuilder_Refinery";var X=
"MyObjectBuilder_Assembler";var W="MyObjectBuilder_OxygenGenerator";var V="MyObjectBuilder_BatteryBlock";var U="MyObjectBuilder_HydrogenEngine";var
T="MyObjectBuilder_Reactor";var S=new string[]{"MyObjectBuilder_Thrust","MyObjectBuilder_LargeThrust",
"MyObjectBuilder_SmallThrust"};var R=new string[]{"MyObjectBuilder_LargeGatlingTurret","MyObjectBuilder_LargeTurretBase",
"MyObjectBuilder_LargeMissileBase","MyObjectBuilder_ConveyorSorter","MyObjectBuilder_InteriorTurret","MyObjectBuilder_SmallGatlingGun",
"MyObjectBuilder_SmallMissileLauncher","MyObjectBuilder_SmallMissileLauncherReload","MyObjectBuilder_LargeMissileTurretReload",
"MyObjectBuilder_SmallGatlingGunBase","MyObjectBuilder_ConveyorSorter"};var Q=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(Q);var P=0;var O=0;
var N=0;var M=0;var L=0;var K=0;var J=0;var I=0;foreach(var H in Q){if(H.BlockDefinition.TypeIdString==Y){P++;}else if(H.
BlockDefinition.TypeIdString==X){O++;}else if(H.BlockDefinition.TypeIdString==W){N++;}else if(H.BlockDefinition.TypeIdString==V){M++;}
else if(H.BlockDefinition.TypeIdString==U){L++;}else if(H.BlockDefinition.TypeIdString==T){K++;}else{bool G=false;foreach(
var F in R){if(H.BlockDefinition.TypeIdString==F){J++;G=true;break;}}if(!G){foreach(var E in S){if(H.BlockDefinition.
TypeIdString==E){I++;break;}}}}}var D=GridTerminalSystem.GetBlockWithName(LCD_NAME)as IMyTextPanel;if(D==null){Echo(
"Error: LCD panel named: "+LCD_NAME+" not found.");return;}D.ContentType=ContentType.TEXT_AND_IMAGE;D.FontSize=1.1f;D.Font="Monospace";D.Alignment
=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;string C="╔════════════════════════╗\n";C+="║     Block Counter     ║\n";C
+="╚════════════════════════╝\n\n";int B=Math.Max("Refineries:".Length,Math.Max("Assemblers:".Length,Math.Max(
"H2/O2 Generators:".Length,Math.Max("Thrusters:".Length,Math.Max("Batteries:".Length,Math.Max("Hydrogen Engines:".Length,Math.Max(
"Reactors:".Length,"Weapons:".Length)))))));string Z="{0,-"+B+"}{1,6}\n";string A=$"{C}";A+=string.Format(Z,"Refineries:",P);A+=
string.Format(Z,"Assemblers:",O);A+=string.Format(Z,"H2/O2 Generators:",N);A+=string.Format(Z,"Thrusters:",I);A+=string.Format
(Z,"Batteries:",M);A+=string.Format(Z,"Hydrogen Engines:",L);A+=string.Format(Z,"Reactors:",K);A+=string.Format(Z,
"Weapons:",J);D.WriteText(A);}