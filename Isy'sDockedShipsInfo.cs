
// Isy's Docked Ships Info
// ==================
// Version: 1.6.1
// Date: 2023-11-10

// =======================================================================================
//                                                                            --- Configuration ---
// =======================================================================================

// --- LCD panels ---
// =======================================================================================

// Just add the keyword below to all your LCD names, where you want to show docked ships on.
// The configuration, what to show, is entirely done via the LCD's custom data.
string mainLCDKeyword = "IDS-main";

// To display all current warnings and problems, add the following keyword to any LCD name (default: !warnings).
string warningsLCDKeyword = "IDS-warnings";

// To display the script performance, add the following keyword to any LCD name (default: !performance).
string performanceLCDKeyword = "IDS-performance";

// Default font ("Debug" or "Monospace") and fontsize for new LCDs
string defaultFont = "Debug";
float defaultFontSize = 0.61f;
float defaultPadding = 2f;

// LCD Master options: This set of options is applied to every new main LCD and every LCD that has master options enabled.
// Basic Options
const bool showNumbers = true;
const bool showDockingTime = true;
const bool showConnectorName = true;
const bool showShipHealth = true;
const bool showBatteryCharge = true;
const bool showHydrogenLevel = true;
const bool showOxygenLevel = true;
const bool showCargoLevel = true;

// Visual settings
const bool showHeading = true;
const bool compactShipStats = false;
const bool scrollText = true;
const bool showFreeConnectors = true;
const bool sortByConnector = true;
const bool emptyLineBetweenEntries = true;

// Monitored connectors:
const bool showGroupHeading = true;


// --- Settings for enthusiasts ---
// =======================================================================================

// Extra breaks between script methods in ticks (1 tick = 16.6ms, 60 ticks ~ 1 seconds).
double extraScriptTicks = 0;

// Also show free connectors (if enabled) on connected ships?
bool showFreeConnectorsOnConnectedShips = false;


// =======================================================================================
//                                                                      --- End of Configuration ---
//                                                        Don't change anything beyond this point!
// =======================================================================================


List<O>Ō=new List<O>();List<IMyTerminalBlock>ŋ=new List<IMyTerminalBlock>();List<IMyShipConnector>ō=new List<
IMyShipConnector>();HashSet<IMyCubeGrid>Ŧ=new HashSet<IMyCubeGrid>();List<IMyTerminalBlock>ť=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>Ť=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ţ=new List<IMyTerminalBlock>();int Ţ=0;int š=0;int Š=0;int ş=0;int
Ş=0;string[]ŝ={"/","-","\\","|"};int Ŝ=0;string ś="- No docked ship -";string[]Ü={
"Master options (disable to set individual options for this LCD):","================================================","useMasterOptions=true","","Basic settings:","============",
"showNumbers="+ƃ(showNumbers),"showDockingTime="+ƃ(showDockingTime),"showConnectorName="+ƃ(showConnectorName),"showShipHealth="+ƃ(
showShipHealth),"showBatteryCharge="+ƃ(showBatteryCharge),"showHydrogenLevel="+ƃ(showHydrogenLevel),"showOxygenLevel="+ƃ(
showOxygenLevel),"showCargoLevel="+ƃ(showCargoLevel),"","Visual settings:","============","showHeading="+ƃ(showHeading),
"compactShipStats="+ƃ(compactShipStats),"scrollText="+ƃ(scrollText),"showFreeConnectors="+ƃ(showFreeConnectors),"sortByConnector="+ƃ(
sortByConnector),"emptyLineBetweenEntries="+ƃ(emptyLineBetweenEntries),"","Monitored connectors:","=================",
"Put a single connector name or a groupname here.","Leave blank to monitor all!","monitoredConnectors=","showGroupHeading="+ƃ(showGroupHeading),};string[]Ś={
"showHeading=true","scrollTextIfNeeded=true"};bool Ř=false;bool ŗ=false;string Ņ;HashSet<string>Ŗ=new HashSet<string>();HashSet<string>ŕ=
new HashSet<string>();TimeSpan Ŕ=new TimeSpan();int œ=0;int Œ=int.MaxValue;int ő=1;bool Ő=false;bool ŏ=false;int Ŏ=0;string
[]Ŋ={"","Finding ships","Getting ship blocks","Getting lcds","Getting ship stats",};Program(){Echo(
"Script is launching..");Runtime.UpdateFrequency=UpdateFrequency.Update100;}void Main(){Ŕ+=Runtime.TimeSinceLastRun;if(Œ<extraScriptTicks){
Runtime.UpdateFrequency=UpdateFrequency.Update1;Œ++;return;}if(Ő){Runtime.UpdateFrequency=UpdateFrequency.Update1;if(Ŏ==0)Ŀ();
if(Ŏ==1)ň();if(Ŏ==2)ń();if(Ŏ>2){Ŏ=0;Ő=false;}return;}ŋ.Clear();GridTerminalSystem.GetBlocks(ŋ);if(ŋ.Count!=œ){Runtime.
UpdateFrequency=UpdateFrequency.Update10;œ=ŋ.Count;Ţ=0;ŏ=true;Ő=false;ő=1;}if(!ŏ){Runtime.UpdateFrequency=UpdateFrequency.Update100;Œ=0
;Ő=true;}if(ő==1){ĺ();}if(ő==2){if(ĳ()==false)return;}if(ő==3){Ļ();}if(ő==4){Ĳ();ŏ=false;}â(Ŋ[ő]);ū();if(ő>=4){ő=3;Ŗ=new
HashSet<string>(ŕ);ŕ.Clear();if(Ŗ.Count==0)Ņ=null;}else{ő++;}Ŝ=Ŝ>=3?0:Ŝ+1;}void Ļ(){ť.Clear();Ť.Clear();ţ.Clear();ť=Þ(
mainLCDKeyword,Ü);Ť=Þ(warningsLCDKeyword,Ś);ţ=Þ(performanceLCDKeyword,Ś);}void ĺ(){GridTerminalSystem.GetBlocksOfType<IMyShipConnector
>(ō,k=>k.IsSameConstructAs(Me));List<IMyShipConnector>Ĺ=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(Ĺ
);Ō.RemoveAll(ı=>!Ĺ.Contains(ı.K));Ŧ.Clear();if(ō.Count>0){foreach(var ķ in ō){if(ķ.Status==MyShipConnectorStatus.
Connected){Ŧ.Add(ķ.CubeGrid);if(Ō.Any(ı=>ı.N==ķ.OtherConnector.CubeGrid)){}else{Ō.Add(new O(ķ.OtherConnector,ķ,ķ.CustomName));}ĸ(
ķ.OtherConnector,ķ,ķ.CustomName);}else{Ō.Add(new O(null,ķ,ķ.CustomName,ś));}}}Ɓ();}void ĸ(IMyShipConnector ķ,
IMyShipConnector v,string u){Ŧ.Add(ķ.CubeGrid);List<IMyShipConnector>Ķ=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(Ķ
,k=>k.IsSameConstructAs(ķ));string ĵ;foreach(var Ĵ in Ķ){ĵ=u+" -> "+Ĵ.CustomName;if(Ĵ.Status==MyShipConnectorStatus.
Connected){if(!Ŧ.Contains(Ĵ.OtherConnector.CubeGrid)){if(Ō.Any(ı=>ı.N==Ĵ.OtherConnector.CubeGrid)){}else{Ō.Add(new O(Ĵ.
OtherConnector,v,ĵ));}ĸ(Ĵ.OtherConnector,v,ĵ);}}else{if(showFreeConnectorsOnConnectedShips)Ō.Add(new O(null,v,ĵ,ś));}}}bool ĳ(){for(
int Q=Ţ;Q<Ō.Count;Q++){if(ß(5))return false;Ţ++;if(Ō[Q].N!=null){GridTerminalSystem.GetBlocksOfType(Ō[Q].I,A=>A.CubeGrid.
IsSameConstructAs(Ō[Q].N));foreach(var Č in Ō[Q].I){if(Č is IMyBatteryBlock)Ō[Q].H.Add(Č as IMyBatteryBlock);if(Č is IMyGasTank){if(Č.
BlockDefinition.SubtypeId.Contains("Hydrogen")){Ō[Q].G.Add(Č as IMyGasTank);}else{Ō[Q].F.Add(Č as IMyGasTank);}}if(Č.InventoryCount==1)
Ō[Q].E.Add(Č);}}}Ţ=0;return true;}void Ĳ(){foreach(var ı in Ō){if(!Ř){if(ı.E.Count!=0){if(ı.E[0].GetInventory().MaxVolume
==VRage.MyFixedPoint.MaxValue||ı.E[0].GetInventory().MaxVolume==0)ŗ=true;Ř=true;}}ı.C=0;ı.B=0;ı.X=0;ı.Z=0;ı.º=0;ı.À=0;ı.µ=
0;ı.ª=0;ı.z=ı.I.Count;ı.y=ı.I.Count;foreach(var İ in ı.H){ı.C+=İ.CurrentStoredPower;ı.B+=İ.MaxStoredPower;}foreach(var ļ
in ı.G){ı.X+=ļ.Capacity*ļ.FilledRatio;ı.Z+=ļ.Capacity;}foreach(var ļ in ı.F){ı.º+=ļ.Capacity*ļ.FilledRatio;ı.À+=ļ.Capacity
;}foreach(var ŉ in ı.E){ı.µ+=(double)ŉ.GetInventory().CurrentVolume;ı.ª+=(double)ŉ.GetInventory().MaxVolume;}foreach(var
Č in ı.I){if(!Č.CubeGrid.GetCubeBlock(Č.Position).IsFullIntegrity){Ź("'"+Č.CustomName+"'\non ship\n'"+ı.M+
"'\nis damaged!");ı.z-=1;}}if(ŗ){if(ı.G.Count>0)ı.Z=int.MaxValue;if(ı.F.Count>0)ı.À=int.MaxValue;if(ı.E.Count>0)ı.ª=int.MaxValue;}ı.D+=Ŕ
;}Ŕ=new TimeSpan();}void ň(){if(Ť.Count==0){Ŏ++;return;}StringBuilder Ň=new StringBuilder();if(Ŗ.Count==0){Ň.Append(
"- No problems detected -");}else{int ņ=1;foreach(var Ņ in Ŗ){Ň.Append(ņ+". "+Ņ.Replace("\n"," ")+"\n");ņ++;}}for(int Q=š;Q<Ť.Count;Q++){if(ß(5))
return;š++;var Ń=Ť[Q].Ĝ(warningsLCDKeyword);foreach(var ł in Ń){var P=ł.Key;var ù=ł.Value;if(!P.GetText().EndsWith("\a")){P.
Font=defaultFont;P.FontSize=defaultFontSize;P.TextPadding=defaultPadding;P.Alignment=TextAlignment.LEFT;P.ContentType=
ContentType.TEXT_AND_IMAGE;}bool Ł=ù.û("showHeading");bool ī=ù.û("scrollTextIfNeeded");StringBuilder ŀ=new StringBuilder();if(Ł){ŀ.
Append("Isy's Docked Ships Info Warnings\n");ŀ.Append(P.Đ('=',P.ñ(ŀ))).Append("\n\n");}ŀ.Append(Ň);ŀ=P.Į(ŀ,Ł?3:0,ī);P.
WriteText(ŀ.Append("\a"));}}Ŏ++;š=0;}void ń(){if(ţ.Count==0){Ŏ++;return;}for(int Q=Š;Q<ţ.Count;Q++){if(ß(5))return;Š++;var Ń=ţ[Q]
.Ĝ(performanceLCDKeyword);foreach(var ł in Ń){var P=ł.Key;var ù=ł.Value;if(!P.GetText().EndsWith("\a")){P.Font=
defaultFont;P.FontSize=defaultFontSize;P.TextPadding=defaultPadding;P.Alignment=TextAlignment.LEFT;P.ContentType=ContentType.
TEXT_AND_IMAGE;}bool Ł=ù.û("showHeading");bool ī=ù.û("scrollTextIfNeeded");StringBuilder ŀ=new StringBuilder();if(Ł){ŀ.Append(
"Isy's Docked Ships Info Performance\n");ŀ.Append(P.Đ('=',P.ñ(ŀ))).Append("\n\n");}ŀ.Append(ì);ŀ=P.Į(ŀ,Ł?3:0,ī);P.WriteText(ŀ.Append("\a"));}}Ŏ++;Š=0;}void Ŀ()
{if(ť.Count==0){Ŏ++;return;}Dictionary<IMyTextSurface,string>ľ=new Dictionary<IMyTextSurface,string>();Dictionary<
IMyTextSurface,string>ř=new Dictionary<IMyTextSurface,string>();List<IMyTextSurface>ŧ=new List<IMyTextSurface>();List<IMyTextSurface>ƛ
=new List<IMyTextSurface>();foreach(var Č in ť){var Ń=Č.Ĝ(mainLCDKeyword);foreach(var ł in Ń){if(ł.Value.Contains(
mainLCDKeyword+":")){ľ[ł.Key]=ł.Value;ŧ.Add(ł.Key);}else{ř[ł.Key]=ł.Value;ƛ.Add(ł.Key);}}}HashSet<string>Ƒ=new HashSet<string>();
foreach(var P in ľ){Ƒ.Add(System.Text.RegularExpressions.Regex.Match(P.Value,mainLCDKeyword+@":[A-Za-z]+").Value);}Ƒ.
RemoveWhere(Ɛ=>Ɛ=="");List<string>Ə=Ƒ.ToList();for(int Q=ş;Q<Ə.Count;Q++){if(ß(5))return;ş++;var Ǝ=ľ.Where(ƍ=>ƍ.Value.Contains(Ə[Q]
));var ƌ=from pair in Ǝ orderby System.Text.RegularExpressions.Regex.Match(pair.Value,mainLCDKeyword+@":\w+").Value
ascending select pair;IMyTextSurface Ƌ=ƌ.ElementAt(0).Key;string ù=ƌ.ElementAt(0).Value;StringBuilder ŀ=Ƣ(Ƌ,ù);if(ù.û(
"scrollText")==true){bool Ł=ù.û("showHeading");int Ɗ=0;foreach(var Ɔ in ƌ){Ɗ+=Ɔ.Key.Ė();}ŀ=Ƌ.Į(ŀ,Ł?0:3,true,Ɗ);}var ĭ=ŀ.ToString().
Split('\n');int Ɖ=ĭ.Length;int ƈ=0;int Ī,Ƈ;foreach(var Ɔ in ƌ){IMyTextSurface P=Ɔ.Key;P.FontSize=Ƌ.TextureSize.Y/P.
TextureSize.Y*Ƌ.FontSize;P.Font=Ƌ.Font;P.TextPadding=Ƌ.TextPadding;P.Alignment=Ƌ.Alignment;P.ContentType=ContentType.TEXT_AND_IMAGE
;Ī=P.Ė();Ƈ=0;ŀ.Clear();while(ƈ<Ɖ&&Ƈ<Ī){ŀ.Append(ĭ[ƈ]+"\n");ƈ++;Ƈ++;}P.WriteText(ŀ.Append("\a"));}}for(int Q=Ş;Q<ƛ.Count;Q
++){if(ß(5))return;Ş++;IMyTextSurface P=ƛ[Q];string ù=ř[P];StringBuilder ŀ=Ƣ(P,ù);if(ù.û("scrollText")==true){bool Ł=ù.û(
"showHeading");ŀ=P.Į(ŀ,Ł?3:0);}P.WriteText(ŀ.Append("\a"));}Ŏ++;ş=0;Ş=0;}StringBuilder Ƣ(IMyTextSurface V,string ù){if(!V.GetText().
EndsWith("\a")){V.Font=defaultFont;V.FontSize=defaultFontSize;V.TextPadding=defaultPadding;V.Alignment=TextAlignment.LEFT;V.
ContentType=ContentType.TEXT_AND_IMAGE;}StringBuilder ŀ=new StringBuilder();var ơ=ù.Split('\n');if(ƅ(ơ,"useMasterOptions")){ŀ=Ơ(V,
showNumbers,showDockingTime,showConnectorName,showShipHealth,showBatteryCharge,showHydrogenLevel,showOxygenLevel,showCargoLevel,
showHeading,compactShipStats,sortByConnector,emptyLineBetweenEntries,Ƅ(ơ,"monitoredConnectors"),showGroupHeading,showFreeConnectors
);}else{ŀ=Ơ(V,ƅ(ơ,"showNumbers"),ƅ(ơ,"showDockingTime"),ƅ(ơ,"showConnectorName"),ƅ(ơ,"showShipHealth"),ƅ(ơ,
"showBatteryCharge"),ƅ(ơ,"showHydrogenLevel"),ƅ(ơ,"showOxygenLevel"),ƅ(ơ,"showCargoLevel"),ƅ(ơ,"showHeading"),ƅ(ơ,"compactShipStats"),ƅ(ơ,
"sortByConnector"),ƅ(ơ,"emptyLineBetweenEntries"),Ƅ(ơ,"monitoredConnectors"),ƅ(ơ,"showGroupHeading"),ƅ(ơ,"showFreeConnectors"));}return ŀ
;}StringBuilder Ơ(IMyTextSurface V,bool Ɵ=true,bool ƞ=true,bool Ɲ=true,bool ƣ=true,bool Ɯ=true,bool ƚ=true,bool ƙ=true,
bool Ƙ=true,bool Ł=true,bool Ɨ=false,bool Ɩ=false,bool ƕ=true,string ű="",bool Ɣ=true,bool Ɠ=true){StringBuilder ŀ=new
StringBuilder();bool ƒ=false;int ņ=1;string Ÿ="";List<String>Ũ=new List<String>();if(ű!=""){Ũ=Ų(ű);if(Ũ.Count==0){Ź(
"Connector not found:\n'"+ű+"'\nCheck your LCD's custom data!");}}if(Ł){ŀ.Append("Currently docked ships\n");ŀ.Append(V.Đ('=',V.ñ(ŀ))).Append(
"\n\n");}if(ű!=""&&Ũ.Count==0){ŀ.Append("Warning!\nConnector not found:\n'"+ű+"'\nCheck your LCD's custom data!");return ŀ;}if
(Ũ.Count>1&&Ɣ){ŀ.Append("Group '"+ű+"':\n\n");}List<O>ŷ=new List<O>();if(Ũ.Count!=0){foreach(var ķ in Ũ){foreach(var ı in
Ō){if(ķ==ı.L.CustomName){ŷ.Add(ı);}}}}else{ŷ=Ō.ToList();}if(!Ɠ){ŷ.RemoveAll(k=>k.M==ś);}if(Ɩ){ŷ=ŷ.OrderBy(Ŷ=>Ŷ.J).ToList(
);}else{ŷ=ŷ.OrderBy(Ŷ=>Ŷ.M).ToList();}foreach(var ı in ŷ){if(Ɵ){Ÿ=ņ+". ";ŀ.Append(Ÿ);ņ++;}if(Ɩ&&Ɲ){ŀ.Append(ı.J+":\n");}ŀ
.Append(ı.M+"\n");if(ƞ&&ı.N!=null){ŀ.Append("Docked for: "+ı.x()+"\n");}if(Ɲ&&!Ɩ){ŀ.Append("At: "+ı.J+"\n");}
StringBuilder ŵ=new StringBuilder();bool Ŵ=false;string ų="";if(ƣ&&ı.y>0){if(Ɨ){ŵ.Append("HP "+ı.z.ċ(ı.y));Ŵ=true;ų=", ";}else{ŀ.
Append(ž(V,"Health ",ı.z,ı.y,į:true));}}if(Ɯ&&ı.B>0){if(Ɨ){ŵ.Append("Bat "+ı.C.ċ(ı.B));Ŵ=true;ų=", ";}else{ŀ.Append(ž(V,
"Batteries ",ı.C,ı.B,ı.C.Ć(true),ı.B.Ć(true),į:true));}}if(ƚ&&ı.Z>0){if(Ɨ){ŵ.Append(ų+"H2 "+ı.X.ċ(ı.Z));Ŵ=true;ų=", ";}else{ŀ.Append
(ž(V,"H2 Tanks ",ı.X,ı.Z,ı.X.Ą(),ı.Z.Ą(),į:true));}}if(ƙ&&ı.À>0){if(Ɨ){ŵ.Append(ų+"O2 "+ı.º.ċ(ı.À));Ŵ=true;ų=", ";}else{ŀ
.Append(ž(V,"O2 Tanks ",ı.º,ı.À,ı.º.Ą(),ı.À.Ą(),į:true));}}if(Ƙ&&ı.ª>0){if(Ɨ){ŵ.Append(ų+"Car "+ı.µ.ċ(ı.ª));Ŵ=true;}else{
ŀ.Append(ž(V,"Cargo ",ı.µ,ı.ª,ı.µ.ô(),ı.ª.ô(),į:true));}}if(Ŵ){ŀ.Append(ŵ+"\n");}if(ƕ){ŀ.Append("\n");}ƒ=true;}if(!ƒ){ŀ.
Append("-- No docked ships --");}return ŀ;}List<string>Ų(string ű){List<IMyShipConnector>Ű=new List<IMyShipConnector>();List<
String>ů=new List<String>();var Ů=GridTerminalSystem.GetBlockGroupWithName(ű);if(Ů!=null){var ŭ=new List<IMyShipConnector>();Ů
.GetBlocksOfType<IMyShipConnector>(ŭ);Ű.AddRange(ŭ);}else{GridTerminalSystem.GetBlocksOfType(Ű,k=>k.CustomName.Contains(ű
)&&k.IsSameConstructAs(Me));}foreach(var ķ in Ű){ů.Add(ķ.CustomName);}ů.Sort((Ŭ,A)=>Ŭ.CompareTo(A));return ů;}void ū(){
Echo("Isy's Docked Ships Info "+ŝ[Ŝ]+"\n====================\n");if(Ņ!=null){Echo("Warning!\n"+Ņ+"\n");}StringBuilder Ū=new
StringBuilder();StringBuilder ũ=new StringBuilder();Ū.Append("The script is running.\n\n");Ū.Append(
"Build LCD panels and add the keyword '"+mainLCDKeyword+"' to their name.\n\n");Ū.Append("The configuration is done per LCD via their custom data field.\n\n");ũ
.Append("Connectors: "+ō.Count+"\n");ũ.Append("Docked ships: "+Ō.Count(ı=>ı.N!=null)+"\n\n");ũ.Append("Task: "+Ŋ[ő]+"\n")
;ũ.Append("Script step: "+ő+" / "+(Ŋ.Length-1)+"\n\n");ũ.Append(ì);ì=ũ;Echo(Ū.Append(ũ).ToString());}void Ź(string g){Ŗ.
Add(g);ŕ.Add(g);Ņ=Ŗ.ElementAt(0);}bool ƅ(string[]ù,string ø){foreach(var õ in ù){if(õ.StartsWith(ø+"=")){try{return Convert
.ToBoolean(õ.Replace(ø+"=",""));}catch{return true;}}}return true;}string Ƅ(string[]ù,string ø){foreach(var õ in ù){if(õ.
StartsWith(ø+"=")){return õ.Replace(ø+"=","");}}return"";}static string ƃ(bool Ƃ){return Ƃ.ToString().ToLower();}void Ɓ(){var ƀ=
Storage.Split('\n');for(int Q=0;Q<Ō.Count;Q++){if(Ō[Q].N==null)continue;foreach(var õ in ƀ){if(õ.StartsWith(Ō[Q].N.ToString()))
{try{Ō[Q].D=TimeSpan.Parse(õ.Substring(õ.IndexOf('=')+1));}catch{Ō[Q].D=new TimeSpan();}break;}}}Storage="";}void ſ(){
string ù="";foreach(var ı in Ō){if(ı.N==null)continue;ù+=ı.N.ToString()+"="+ı.D.ToString()+"\n";}Storage=ù;}void Save(){ſ();}
StringBuilder ž(IMyTextSurface V,string Ž,double Ö,double ż,string Ż=null,string ź=null,bool Ľ=false,bool į=false,string d=""){string
Ô=Ö.ToString();string Ó=ż.ToString();if(Ż!=null){Ô=Ż;}if(ź!=null){Ó=ź;}float e=V.FontSize;float Ò=0.61f;float Ñ=1.01f;if(
V.Font=="Monospace"){Ò=0.41f;Ñ=0.81f;}float Ð=V.ē();char Ï=' ';float Î=V.ę(Ï);StringBuilder Í=new StringBuilder(" "+Ö.ċ(ż
));Í=V.Đ(Ï,V.ñ("9999.9%")-V.ñ(Í)).Append(Í);StringBuilder Ì=new StringBuilder(Ô+" / "+Ó);StringBuilder Ë=new
StringBuilder();StringBuilder Ê=new StringBuilder();StringBuilder É;if(ż==0){Ë.Append(d+Ž+" ");É=V.Đ(Ï,Ð-V.ñ(Ë)-V.ñ(Ô));Ë.Append(É).
Append(Ô);return Ë.Append("\n");}double Ç=0;if(ż>0)Ç=Ö/ż>=1?1:Ö/ż;if(į&&!Ľ){if(e<Ò||(e<Ñ&&Ð>512)){Ë.Append(È(V,Ð*0.25f,Ç,d)+
" "+Ž+" ");É=V.Đ(Ï,Ð*0.75-V.ñ(Ë)-V.ñ(Ô+" /"));Ë.Append(É).Append(Ì);É=V.Đ(Ï,Ð-V.ñ(Ë)-V.ñ(Í));Ë.Append(É);Ë.Append(Í);}else{
Ë.Append(È(V,Ð*0.3f,Ç,d)+" "+Ž+" ");É=V.Đ(Ï,Ð-V.ñ(Ë)-V.ñ(Í));Ë.Append(É);Ë.Append(Í);}}else{Ë.Append(d+Ž+" ");if(e<Ò||(e<
Ñ&&Ð>512)){É=V.Đ(Ï,Ð*0.5-V.ñ(Ë)-V.ñ(Ô+" /"));Ë.Append(É).Append(Ì);É=V.Đ(Ï,Ð-V.ñ(Ë)-V.ñ(Í));Ë.Append(É).Append(Í);if(!Ľ){
Ê=È(V,Ð,Ç,d).Append("\n");}}else{É=V.Đ(Ï,Ð-V.ñ(Ë)-V.ñ(Ì));Ë.Append(É).Append(Ì);if(!Ľ){Ê=È(V,Ð-V.ñ(Í),Ç,d);Ê.Append(Í).
Append("\n");}}}return Ë.Append("\n").Append(Ê);}StringBuilder È(IMyTextSurface V,float f,double Ç,string d){StringBuilder Æ,Å
;char Ä='[';char Ã=']';char Õ='I';char Â='∙';float Ø=V.ę(Ä);float ð=V.ę(Ã);float î=0;if(d!="")î=V.ñ(d);float í=f-Ø-ð-î;Æ=
V.Đ(Õ,í*Ç);Å=V.Đ(Â,í-V.ñ(Æ));return new StringBuilder().Append(d).Append(Ä).Append(Æ).Append(Å).Append(Ã);}StringBuilder
ì=new StringBuilder("No performance Information available!");Dictionary<string,int>ë=new Dictionary<string,int>();List<
int>ê=new List<int>(new int[600]);List<double>é=new List<double>(new double[600]);double è,ç,æ,å,ä;int ï,ã=0;void â(string
á){ã=ã>=599?0:ã+1;ï=Runtime.CurrentInstructionCount;if(ï>ç)ç=ï;ê[ã]=ï;å=ê.Sum()/ê.Count;ì.Clear();ì.Append(
"Instructions: "+ï+" / "+Runtime.MaxInstructionCount+"\n");ì.Append("Max. Instructions: "+ç+" / "+Runtime.MaxInstructionCount+"\n");ì.
Append("Avg. Instructions: "+Math.Floor(å)+" / "+Runtime.MaxInstructionCount+"\n\n");è=Runtime.LastRunTimeMs;if(è>æ&&ë.
ContainsKey(á))æ=è;é[ã]=è;ä=é.Sum()/é.Count;ì.Append("Last runtime: "+Math.Round(è,4)+" ms\n");ì.Append("Max. runtime: "+Math.Round
(æ,4)+" ms\n");ì.Append("Avg. runtime: "+Math.Round(ä,4)+" ms\n\n");ì.Append("Instructions per Method:\n");ë[á]=ï;foreach
(var à in ë.OrderByDescending(Q=>Q.Value)){ì.Append("- "+à.Key+": "+à.Value+"\n");}ì.Append("\n");}bool ß(double Ö=10){
return Runtime.CurrentInstructionCount>Ö*1000;}List<IMyTerminalBlock>Þ(string Ý,string[]Ü=null,string Û="Debug",float Ú=0.6f,
float Ù=2f){string Á="[IsyLCD]";var Y=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>
(Y,A=>A.IsSameConstructAs(Me)&&(A.CustomName.Contains(Ý)||(A.CustomName.Contains(Á)&&A.CustomData.Contains(Ý))));var W=Y.
FindAll(A=>A.CustomName.Contains(Ý));foreach(var V in W){V.CustomName=V.CustomName.Replace(Ý,"").Replace(" "+Ý,"").TrimEnd(' ')
;bool U=false;bool T=false;int S=0;if(V is IMyTextSurface){if(!V.CustomName.Contains(Á))U=true;if(!V.CustomData.Contains(
Ý)){T=true;V.CustomData="@0 "+Ý+(Ü!=null?"\n"+String.Join("\n",Ü):"");}}else if(V is IMyTextSurfaceProvider){if(!V.
CustomName.Contains(Á))U=true;int R=(V as IMyTextSurfaceProvider).SurfaceCount;for(int Q=0;Q<R;Q++){if(!V.CustomData.Contains("@"+
Q)){T=true;S=Q;V.CustomData+=(V.CustomData==""?"":"\n\n")+"@"+Q+" "+Ý+(Ü!=null?"\n"+String.Join("\n",Ü):"");break;}}}else
{Y.Remove(V);}if(U)V.CustomName+=" "+Á;if(T){var P=(V as IMyTextSurfaceProvider).GetSurface(S);P.Font=Û;P.FontSize=Ú;P.
TextPadding=Ù;P.Alignment=TextAlignment.LEFT;P.ContentType=ContentType.TEXT_AND_IMAGE;}}return Y;}
}class O{public IMyCubeGrid N{get;set;}=null;public string M{get;set;}="";public IMyShipConnector L{get;set;}=null;public
IMyShipConnector K{get;set;}=null;public string J{get;set;}="";public List<IMyTerminalBlock>I{get;set;}=new List<IMyTerminalBlock>();
public List<IMyBatteryBlock>H{get;set;}=new List<IMyBatteryBlock>();public List<IMyGasTank>G{get;set;}=new List<IMyGasTank>();
public List<IMyGasTank>F{get;set;}=new List<IMyGasTank>();public List<IMyTerminalBlock>E{get;set;}=new List<IMyTerminalBlock>(
);public TimeSpan D{get;set;}=new TimeSpan();public double C{get;set;}=0;public double B{get;set;}=0;public double X{get;
set;}=0;public double Z{get;set;}=0;public double º{get;set;}=0;public double À{get;set;}=0;public double µ{get;set;}=0;
public double ª{get;set;}=0;public double z{get;set;}=0;public double y{get;set;}=0;public String x(){return D.ToString(
@"hh\:mm\:ss");}public O(IMyShipConnector w,IMyShipConnector v,string u,string t=""){K=w;if(w!=null)N=w.CubeGrid;L=v;J=u;if(t==""){if
(w!=null)M=N.CustomName;}else{M=t;}}}public static partial class r{private static Dictionary<char,float>q=new Dictionary<
char,float>();public static void p(string o,float m){foreach(char k in o){q[k]=m;}}public static void j(){if(q.Count>0)
return;p("3FKTabdeghknopqsuy£µÝàáâãäåèéêëðñòóôõöøùúûüýþÿāăąďđēĕėęěĝğġģĥħĶķńņňŉōŏőśŝşšŢŤŦũūŭůűųŶŷŸșȚЎЗКЛбдекруцяёђћўџ",18);p(
"ABDNOQRSÀÁÂÃÄÅÐÑÒÓÔÕÖØĂĄĎĐŃŅŇŌŎŐŔŖŘŚŜŞŠȘЅЊЖф□",22);p("#0245689CXZ¤¥ÇßĆĈĊČŹŻŽƒЁЌАБВДИЙПРСТУХЬ€",20);p("￥$&GHPUVY§ÙÚÛÜÞĀĜĞĠĢĤĦŨŪŬŮŰŲОФЦЪЯжы†‡",21);p(
"！ !I`ijl ¡¨¯´¸ÌÍÎÏìíîïĨĩĪīĮįİıĵĺļľłˆˇ˘˙˚˛˜˝ІЇії‹›∙",9);p("？7?Jcz¢¿çćĉċčĴźżžЃЈЧавийнопсъьѓѕќ",17);p("（）：《》，。、；【】(),.1:;[]ft{}·ţťŧț",10);p("+<=>E^~¬±¶ÈÉÊË×÷ĒĔĖĘĚЄЏЕНЭ−",19);
p("L_vx«»ĹĻĽĿŁГгзлхчҐ–•",16);p("\"-rª­ºŀŕŗř",11);p("WÆŒŴ—…‰",32);p("'|¦ˉ‘’‚",7);p("@©®мшњ",26);p("mw¼ŵЮщ",28);p("/ĳтэє",
15);p("\\°“”„",13);p("*²³¹",12);p("¾æœЉ",29);p("%ĲЫ",25);p("MМШ",27);p("½Щ",30);p("ю",24);p("ј",8);p("љ",23);p("ґ",14);p(
"™",31);}public static Vector2 h(this IMyTextSurface P,StringBuilder g){j();Vector2 f=new Vector2();if(P.Font=="Monospace")
{float e=P.FontSize;f.X=(float)(g.Length*19.4*e);f.Y=(float)(28.8*e);return f;}else{float e=(float)(P.FontSize*0.779);
foreach(char k in g.ToString()){try{f.X+=q[k]*e;}catch{}}f.Y=(float)(28.8*P.FontSize);return f;}}public static float ñ(this
IMyTextSurface V,StringBuilder g){Vector2 Ĕ=V.h(g);return Ĕ.X;}public static float ñ(this IMyTextSurface V,string g){Vector2 Ĕ=V.h(new
StringBuilder(g));return Ĕ.X;}public static float ę(this IMyTextSurface V,char Ę){float ė=ñ(V,new string(Ę,1));return ė;}public
static int Ė(this IMyTextSurface V){Vector2 Ē=V.SurfaceSize;float đ=V.TextureSize.Y;if(Ē.X<512||đ!=Ē.Y)Ē.Y*=512/đ;float ĕ=Ē.Y*
(100-V.TextPadding*2)/100;Vector2 Ĕ=V.h(new StringBuilder("T"));return(int)(ĕ/Ĕ.Y);}public static float ē(this
IMyTextSurface V){Vector2 Ē=V.SurfaceSize;float đ=V.TextureSize.Y;if(Ē.X<512||đ!=Ē.Y)Ē.X*=512/đ;return Ē.X*(100-V.TextPadding*2)/100;}
public static StringBuilder Đ(this IMyTextSurface V,char ď,double Ď){int Ě=(int)(Ď/ę(V,ď));if(Ě<0)Ě=0;return new StringBuilder
().Append(ď,Ě);}private static DateTime č=DateTime.Now;private static Dictionary<int,List<int>>ě=new Dictionary<int,List<
int>>();public static StringBuilder Į(this IMyTextSurface V,StringBuilder g,int Ĭ=3,bool ī=true,int Ī=0){int ĩ=V.
GetHashCode();if(!ě.ContainsKey(ĩ)){ě[ĩ]=new List<int>{1,3,Ĭ,0};}int Ĩ=ě[ĩ][0];int ħ=ě[ĩ][1];int Ħ=ě[ĩ][2];int ĥ=ě[ĩ][3];var Ĥ=g.
ToString().TrimEnd('\n').Split('\n');List<string>ĭ=new List<string>();if(Ī==0)Ī=V.Ė();float Ð=V.ē();StringBuilder d,ģ=new
StringBuilder();for(int Q=0;Q<Ĥ.Length;Q++){if(Q<Ĭ||Q<Ħ||ĭ.Count-Ħ>Ī||V.ñ(Ĥ[Q])<=Ð){ĭ.Add(Ĥ[Q]);}else{try{ģ.Clear();float Ģ,ġ;var Ġ=Ĥ
[Q].Split(' ');string ğ=System.Text.RegularExpressions.Regex.Match(Ĥ[Q],@"\d+(\.|\:)\ ").Value;d=V.Đ(' ',V.ñ(ğ));foreach(
var Ğ in Ġ){Ģ=V.ñ(ģ);ġ=V.ñ(Ğ);if(Ģ+ġ>Ð){ĭ.Add(ģ.ToString());ģ=new StringBuilder(d+Ğ+" ");}else{ģ.Append(Ğ+" ");}}ĭ.Add(ģ.
ToString());}catch{ĭ.Add(Ĥ[Q]);}}}if(ī){if(ĭ.Count>Ī){if(DateTime.Now.Second!=ĥ){ĥ=DateTime.Now.Second;if(ħ>0)ħ--;if(ħ<=0)Ħ+=Ĩ;
if(Ħ+Ī-Ĭ>=ĭ.Count&&ħ<=0){Ĩ=-1;ħ=3;}if(Ħ<=Ĭ&&ħ<=0){Ĩ=1;ħ=3;}}}else{Ħ=Ĭ;Ĩ=1;ħ=3;}ě[ĩ][0]=Ĩ;ě[ĩ][1]=ħ;ě[ĩ][2]=Ħ;ě[ĩ][3]=ĥ;}
else{Ħ=Ĭ;}StringBuilder ĝ=new StringBuilder();for(var õ=0;õ<Ĭ;õ++){ĝ.Append(ĭ[õ]+"\n");}try{for(var õ=Ħ;õ<ĭ.Count;õ++){ĝ.
Append(ĭ[õ]+"\n");}}catch{}return ĝ;}public static Dictionary<IMyTextSurface,string>Ĝ(this IMyTerminalBlock Č,string Ý,
Dictionary<string,string>ò=null){var Ă=new Dictionary<IMyTextSurface,string>();if(Č is IMyTextSurface){Ă[Č as IMyTextSurface]=Č.
CustomData;}else if(Č is IMyTextSurfaceProvider){var ā=System.Text.RegularExpressions.Regex.Matches(Č.CustomData,@"@(\d).*("+Ý+
@")");int Ā=(Č as IMyTextSurfaceProvider).SurfaceCount;foreach(System.Text.RegularExpressions.Match ÿ in ā){int þ=-1;if(int.
TryParse(ÿ.Groups[1].Value,out þ)){if(þ>=Ā)continue;string ö=Č.CustomData;int ý=ö.IndexOf("@"+þ);int ü=ö.IndexOf("@",ý+1)-ý;
string ù=ü<=0?ö.Substring(ý):ö.Substring(ý,ü);Ă[(Č as IMyTextSurfaceProvider).GetSurface(þ)]=ù;}}}return Ă;}public static bool
û(this string ù,string ø){var ö=ù.Replace(" ","").Split('\n');foreach(var õ in ö){if(õ.StartsWith(ø+"=")){try{return
Convert.ToBoolean(õ.Replace(ø+"=",""));}catch{return true;}}}return true;}public static string ú(this string ù,string ø){var ö=
ù.Replace(" ","").Split('\n');foreach(var õ in ö){if(õ.StartsWith(ø+"=")){return õ.Replace(ø+"=","");}}return"";}}public
static partial class r{public static string ô(this float Ö){string ó="kL";if(Ö<1){Ö*=1000;ó="L";}else if(Ö>=1000&&Ö<1000000){Ö
/=1000;ó="ML";}else if(Ö>=1000000&&Ö<1000000000){Ö/=1000000;ó="BL";}else if(Ö>=1000000000){Ö/=1000000000;ó="TL";}return
Math.Round(Ö,1)+" "+ó;}public static string ô(this double Ö){float ă=(float)Ö;return ă.ô();}}public static partial class r{
public static string ċ(this double Ċ,double ĉ){double Ĉ=Math.Round(Ċ/ĉ*100,1);if(ĉ==0){return"0%";}else{return Ĉ+"%";}}public
static string ċ(this float Ċ,float ĉ){double Ĉ=Math.Round(Ċ/ĉ*100,1);if(ĉ==0){return"0%";}else{return Ĉ+"%";}}}public static
partial class r{public static string Ć(this float Ö,bool ą=false){string ó="MW";string ć=Ö<0?"-":"";Ö=Math.Abs(Ö);if(Ö<1){Ö*=
1000;ó="kW";}else if(Ö>=1000&&Ö<1000000){Ö/=1000;ó="GW";}else if(Ö>=1000000&&Ö<1000000000){Ö/=1000000;ó="TW";}else if(Ö>=
1000000000){Ö/=1000000000;ó="PW";}if(ą)ó+="h";return ć+Math.Round(Ö,1)+" "+ó;}public static string Ć(this double Ö,bool ą=false){
float ă=(float)Ö;return ă.Ć(ą);}}public static partial class r{public static string Ą(this float Ö){string ó="L";if(Ö>=1000&&
Ö<1000000){Ö/=1000;ó="KL";}else if(Ö>=1000000&&Ö<1000000000){Ö/=1000000;ó="ML";}else if(Ö>=1000000000&&Ö<1000000000000){Ö
/=1000000000;ó="BL";}else if(Ö>=1000000000000){Ö/=1000000000000;ó="TL";}return Math.Round(Ö,1)+" "+ó;}public static string
Ą(this double Ö){float ă=(float)Ö;return ă.Ą();}