/*
 * 
 * 
 * /////////////////////////////////////////////////////
 *     Auto Docking 2.0
 *     
 *     Author:  Spug
 *     Please leave credit if the ship is
 *     used on the workshop.
 *     (Extra thanks to Whip for his PID controller)
 * /////////////////////////////////////////////////////
 */


// CHANGEABLE VARIABLES:

int speedSetting = 2;                           // 1 = Cinematic, 2 = Classic, 3 = Breakneck
                                                // Cinematic: Slower but looks cooler, especially for larger ships.
                                                // Classic: Lands at the classic pace.
                                                // Breakneck: Still safe, but will land pretty much as quick as it can.


double caution = 0.4;                                             // Between 0 - 0.9. Defines how close to max deceleration the ship will ride.
bool extra_info = false;                                          // If true, this script will give you more information about what's happening than usual.
string your_title = "Captain";                                  // How the ship will refer to you.
bool small_ship_rotate_on_connector = true;       //If enabled, small ships will rotate on the connector to face the saved direction.
bool large_ship_rotate_on_connector = false;      //If enabled, large ships will rotate on the connector to face the saved direction.
bool rotate_on_approach = false;                          //If enabled,  the ship will rotate to the saved direction on connector approach.
double topSpeed = 100;                                         // The top speed the ship will go in m/s.

bool extra_soft_landing_mode = false;                 // If your ship is hitting your connector too hard, enable this.
double connector_clearance = 0;                          // If you raise this number (measured in meters), the ship will fly connector_clearance higher before coming down onto the connector.
double add_acceleration = 0;                                // If your ship is accelerating very slow, or perhaps stopping at a low top speed, try raising this (e.g to 10).
double rotation_speed = 1;                                    // RECOMPILE after changing. between 0 - 1, how fast the ship will rotate when controlled by this script.

string lcd_tag = "[dock]";                                       // The text you can add to an LCD block name. The LCD will then output this block's output
string timer_tag = "[dock]";                                   // The text you can add to a timer block name. The timer will then be triggered on a completed dock.
bool force_timer_search_on_station = false;       // If enabled, the ship will make sure it always searches the station for [dock] (the timer_tag) in the names of any timer blocks.
string start_timer_tag = "[start dock]";                 // A timer with this text in the name will be triggered as soon as a docking procedure is started.

bool enable_antenna_function = true;                   //If enabled, the ship will try to search for an optional home script. Disable if the antenna functionality is giving you problems.

bool allow_connector_on_seperate_grid = false; // WARNING: All connectors on your ship must have [dock] in the name if you set this to true! This option allows your connector to not be on the same grid.

double high_speed_lead_amount = 1;                  // WARNING: Only change if the ship can't land on high speed ships (over 120 m/s).
                                                    //If this number is less than 1, the ship will fly ahead of the connector more when the connector is moving. This can be a negative number, however try 0 first.

// Waypoint settings:
double required_waypoint_accuracy = 6;             // how close the ship needs to be to a waypoint to complete it (measured in meters). Do note, closer waypoints are more accurate anyway.
double waypoints_top_speed = 100;                     // the top speed the ship will go in m/s when it's moving towards waypoints
bool rotate_during_waypoints = true;                    // if true, the ship will rotate to face each waypoint's direction as it goes along.



// This code has been minified by Malware's MDK minifier.
// Find the original source code here:
// https://github.com/ksqk34/Autodocking-2




// DO NOT CHANGE BELOW THIS LINE
// Well you can try...
private readonly Ɔ systemsAnalyzer;
const double ǰ=10;const double Ǳ=2;const double ǲ=.5;const double ǳ=1/ǰ;ȷ Ǵ;List<ɀ>ǵ;double Ƕ=0;M Ƿ;M Ǹ;Đ ȁ;Ŗ ǹ;M Ǻ;
double ǻ;double Ǽ;double ǽ;List<IMyTerminalBlock>Ǿ=new List<IMyTerminalBlock>();string ǿ;double Ȁ;double ȃ;bool Ȅ;bool ȡ;bool
Ȏ;Vector3D ȏ;DateTime Ȑ;Vector3D ȑ=Vector3D.Zero;string Ȓ="";double ȓ=1;bool Ȕ;DateTime ȕ;int Ȗ=0;string ȗ="";double Ș;
double ș;double Ț=100;Program(){Ȅ=false;Runtime.UpdateFrequency=UpdateFrequency.Once;ȏ=Vector3D.Zero;GridTerminalSystem.
GetBlocks(Ǿ);ǵ=new List<ɀ>();ȁ=new Đ(this);if(Storage.Length>0)ț();Ǵ=new ȷ(this);systemsAnalyzer=new Ɔ(this);ǹ=new Ŗ(this);Ƿ=new
M(Ǳ,0,ǲ,-10,10,ǳ);Ǹ=new M(Ǳ,0,ǲ,-10,10,ǳ);Ǻ=new M(Ǳ,0,ǲ,-10,10,ǳ);Ș=0;ș=0;ȶ();}void ț(){var Ȝ=Storage.Split('#');var ȝ=Ȝ[
0].Split(';');foreach(var Ȟ in ȝ)if(Ȟ.Length>0){var ȟ=new ɀ(Ȟ,this);if(ȟ.ɒ!=null)ǵ.Add(ȟ);}}double Ƞ(ł ȉ,bool ȍ){if(ȉ.Ŋ){
var ȅ=systemsAnalyzer.q.ɒ;var ǯ=ȅ.GetPosition();var ƨ=-ȉ.Ń;var Ȇ=ƨ.Length();if(ƨ.LengthSquared()==0){foreach(var Š in
systemsAnalyzer.Ŧ)Š.SetValue("Override",false);return-1;}var ƿ=Matrix.CreateWorld(ǯ,ȅ.WorldMatrix.Up,-ȅ.WorldMatrix.Forward);var Ǯ=ƿ.
Forward;var ǀ=ƿ.Left;var ǁ=ƿ.Up;ǻ=Math.Acos(MathHelper.Clamp(ƨ.Dot(Ǯ)/Ȇ,-1,1))-Math.PI/2;Vector3D ȇ=Ǯ.Cross(ƨ);Ǽ=M.É(ǀ,ȇ);Ǽ*=M.
Î(M.Ì(ǀ,ƨ),ƨ);if(ȍ)ǽ=Math.Acos(MathHelper.Clamp(ȉ.Ō.Dot(ǀ),-1,1))-Math.PI/2;else ǽ=0;ǻ*=-1;Ǽ*=-1;var Ǆ=Math.Acos(
MathHelper.Clamp(ƨ.Dot(Ǯ),-1,1))*180/Math.PI;Ǆ-=90;var ǅ=Ǹ.Ɓ(Ǽ)*rotation_speed;var ǆ=Ƿ.Ɓ(ǻ)*rotation_speed;double Ǉ=0;if(ȍ)Ǉ=Ǻ.Ɓ(ǽ
)*rotation_speed;if(!Ȅ)ǹ.ŗ(ǆ,Ǉ,-ǅ,systemsAnalyzer.Ŧ,ƿ);return Ǆ;}return-1;}double Ȉ(ł ȉ){MatrixD ǧ=Matrix.CreateWorld(
systemsAnalyzer.q.ɗ,systemsAnalyzer.q.ǥ,(-systemsAnalyzer.q.Ǧ).Cross(systemsAnalyzer.q.ǥ));var Ȋ=Me.CubeGrid;Vector3D ȋ=ɀ.â(ȉ.Ń,
systemsAnalyzer.q);Vector3D Ȍ=ɀ.â(ȉ.Ō,systemsAnalyzer.q);var ƨ=ȋ;var ǯ=Ȋ.GetPosition();var ƿ=Matrix.CreateWorld(ǯ,Ȋ.WorldMatrix.Up,-Ȋ.
WorldMatrix.Forward);var Ǯ=ƿ.Forward;var ǀ=ƿ.Left;var ǁ=ƿ.Up;ǻ=Math.Acos(MathHelper.Clamp(ƨ.Dot(Ǯ),-1,1))-Math.PI/2;Vector3D ǂ=Ǯ.
Cross(ƨ);Ǽ=M.É(ǀ,ǂ);Ǽ*=M.Î(M.Ì(ǀ,ƨ),ƨ);Vector3D ǃ=(-Ȍ).Cross(ȋ);ǽ=Math.Acos(MathHelper.Clamp((-ǃ).Dot(ǀ),-1,1))-Math.PI/2;ǻ*=
-1;Ǽ*=-1;var Ǆ=Math.Acos(MathHelper.Clamp(ƨ.Dot(Ǯ),-1,1))*180/Math.PI;Ǆ-=90;var ǅ=Ǹ.Ɓ(Ǽ)*rotation_speed;var ǆ=Ƿ.Ɓ(ǻ)*
rotation_speed;double Ǉ=Ǻ.Ɓ(ǽ)*rotation_speed;if(!Ȅ)ǹ.ŗ(ǆ,Ǉ,-ǅ,systemsAnalyzer.Ŧ,ƿ);return Ǆ;}void Save(){ǎ();}void ǎ(){Storage="";
foreach(var ǈ in ǵ){ǉ(ǈ.ó()+";");}ǉ("#");}void ǉ(string Ǌ){Storage+=Ǌ;}void ǋ(string Z,IMyShipConnector ǌ=null){systemsAnalyzer
.q=ɿ(Z);if(systemsAnalyzer.q!=null){if(ǌ!=null){systemsAnalyzer.q.ɒ=ǌ;systemsAnalyzer.q.ɓ=ǌ.EntityId;}Me.CustomData=
systemsAnalyzer.q.ɒ.EntityId.ToString();ȁ.Ú();systemsAnalyzer.q.ɞ=Vector3D.Zero;systemsAnalyzer.q.ɔ=Vector3D.Zero;systemsAnalyzer.q.ɕ=
Vector3D.Zero;ǿ=Z;Ȕ=true;ȡ=false;Ȏ=false;Ȗ=0;Ȓ="";ȓ=1;Runtime.UpdateFrequency=UpdateFrequency.Update1;ȕ=DateTime.Now;Ȑ=DateTime.
Now;ȑ=systemsAnalyzer.ũ.GetShipVelocities().LinearVelocity;if(enable_antenna_function)Ǵ.ȣ(1);}else{ȶ();}}string Ǎ(string Z,
IMyShipConnector Ǐ){var Ü=Ǐ.OtherConnector;if(Ü==null){ȁ.û(
"\nSomething went wrong when finding the connector.\nMaybe you have multiple connectors on the go, "+your_title+"?");return"";}var ƾ=new ɀ(Z,Ǐ,Ü);var Ƴ=ǵ.LastIndexOf(ƾ);if(Ƴ!=-1){if(extra_info)ȁ.ā(
"- Docking location already Exists!\n- Adding argument.");if(!ǵ[Ƴ].ɑ.Contains(Z)){if(extra_info)ȁ.ā("Other arguments associated: "+ȁ.t(ǵ[Ƴ]));ǵ[Ƴ].ɑ.Add(Z);if(extra_info)ȁ.ā(
"- New argument added.");}else if(extra_info){ȁ.ā("- Argument already in!");if(extra_info)ȁ.ā("All arguments associated: "+ȁ.t(ǵ[Ƴ]));}ǵ[Ƴ].Û(Ǐ
,Ü);}else{ǵ.Add(ƾ);if(extra_info)ȁ.ā("- Added new docking location.");}var Ƭ=0;var ƭ=new List<ɀ>();foreach(var q in ǵ)if(
!q.Equals(ƾ))if(q.ɑ.Contains(Z)){Ƭ+=1;q.ɑ.Remove(Z);if(q.ɑ.Count==0)ƭ.Add(q);}while(ƭ.Count>0){ǵ.Remove(ƭ[0]);ƭ.RemoveAt(
0);}if(extra_info){if(Ƭ==1)ȁ.ā("- Found 1 other association with that argument. Removed this other.");else if(Ƭ>1)ȁ.ā(
"- Found "+Ƭ+" other associations with that argument. Removed these others.");}if(Z==""){if(!extra_info)return
"SAVED\nSaved docking location as no argument, "+your_title+".";return"Saved docking location as no argument, "+your_title+".";}if(!extra_info)return
"SAVED\nSaved docking location as "+Z+", "+your_title+".";return"Saved docking location as "+Z+", "+your_title+".";}bool Ʈ=false;bool Ư=false;string ư="";
List<Vector3D>Ʊ;List<Vector3D>Ʋ;List<Vector3D>ƴ;List<double>Ƽ;List<bool>Ƶ;void ƶ(){Ʈ=true;Ư=true;ȶ();ȁ.ā(
"RECORDING MODE\nPlease enter an argument that\nwill be associated with these waypoints then press Run, "+your_title+".\n\nTo cancel, press Recompile.");}void Ʒ(string e){if(e.ToLower().Trim()=="record"){ȁ.ā(
"RECORDING MODE\nPlease choose an argument\nother than record, then press Run, "+your_title+".\n\nTo cancel, press Recompile.");}else{Ʈ=true;Ư=false;ư=e;Ȗ=0;Ʊ=new List<Vector3D>();Ʋ=new List<Vector3D>
();ƴ=new List<Vector3D>();Ƽ=new List<double>();Ƶ=new List<bool>();ȁ.Ø(ư,Ȗ,"");}}double Ƹ(Vector3D ƹ,Vector3D ƺ){double ƻ=
Vector3D.Distance(ƹ,ƺ)/4;if(ƻ<0.2){ƻ=0.2;}return ƻ;}void ƫ(IMyShipConnector ƽ,string Z){if(ƽ==null){Ʊ.Add(Me.CubeGrid.
GetPosition());Ʋ.Add(Me.CubeGrid.WorldMatrix.Forward);ƴ.Add(Me.CubeGrid.WorldMatrix.Right);double ǟ=-1;bool Ǡ=true;if(Z.Trim().
Length>1){if(Z.Trim()[0]=='!'){string ǡ=Z.Remove(0,1);double Ǣ;bool ž=double.TryParse(ǡ,out Ǣ);if(ž){ǟ=Ǣ;}}}if(Z.ToLower().
Trim().Contains("no spin")||Z.ToLower().Trim().Contains("nospin")||Z.ToLower().Trim().Contains("!nospin")){Ǡ=false;}Ƽ.Add(ǟ)
;Ƶ.Add(Ǡ);string h="";if(ǟ>0){h="\nRecorded speed: "+ǟ;}if(!Ǡ){h+="\nRecorded no rotate";}Ȗ+=1;ȁ.Ø(ư,Ȗ,h);}else{ȁ.ā(
"FINISHED RECORDING");if(Ȗ==0){ȁ.ā("No waypoints recorded.");var ž=Ǎ(ư,ƽ);}else{Ʈ=false;Ư=false;var ž=Ǎ(ư,ƽ);ɀ q=ɿ(ư);List<ł>ǣ=new List<ł>()
;Vector3D Ǥ=q.ɗ;Vector3D ǥ=q.ǥ;Vector3D Ǧ=q.Ǧ;MatrixD ǧ=Matrix.CreateWorld(Ǥ,ǥ,(-Ǧ).Cross(ǥ));Vector3D Ǩ=Vector3D.Zero;
for(int ǩ=0;ǩ<Ȗ;ǩ++){Vector3D Ǫ=Ʊ[ǩ];Vector3D ǫ=ɀ.ĕ(Ǫ,ǧ);double Ǭ=required_waypoint_accuracy;if(ǩ>0){double ǭ=Ƹ(Ǫ,Ǩ);Ǭ=Math
.Min(ǭ,required_waypoint_accuracy);}if(ǩ==Ȗ-1){double ǭ=Ƹ(Ǫ,Ǥ)*0.7;Ǭ=Math.Min(ǭ,Ǭ);}Vector3D Ǟ=ɀ.ß(Ʋ[ǩ],ǧ);Vector3D ǐ=ɀ.ß
(ƴ[ǩ],ǧ);double ǖ=Ƽ[ǩ];if(ǖ<0){ǖ=waypoints_top_speed;}ł Ǒ=new ł(ǫ,Ǟ,ǐ){ŋ=true,ń=15,Ŋ=Ƶ[ǩ],Ŏ=Ǭ,Ň=ǖ};Ǩ=Ǫ;ǣ.Add(Ǒ);}if(q.ɣ.
ContainsKey(ư)){ȁ.ā("Overwriting existing waypoints.");}q.ɣ[ư]=ǣ;ȁ.ā("Recorded "+(Ȗ+1).ToString()+" waypoints to argument: "+Đ.ă(ư)
);Ȗ=0;}}}string ǒ(string Z){string[]Ǔ=Z.Split(' ');if(Ǔ.Length>1){if(Ǔ[0].ToLower()=="clear"){return Z.Remove(0,6);}else{
return null;}}else{return null;}}void ǔ(string Z){bool Ǖ=false;var ƭ=new List<ɀ>();foreach(var q in ǵ)if(q.ɑ.Contains(Z)){q.ɑ.
Remove(Z);if(q.ɣ.ContainsKey(Z)){q.ɣ.Remove(Z);}Ǖ=true;if(q.ɑ.Count==0)ƭ.Add(q);}while(ƭ.Count>0){ǵ.Remove(ƭ[0]);ƭ.RemoveAt(0)
;}if(Ǖ){ȁ.ā("CLEARED\nThe argument: "+Đ.ă(Z)+"\nhas been cleared from memory.");}else{ȁ.ā("WARNING\nThe argument: "+Đ.ă(Z
)+"\nwasn't found in memory.");}}string ǜ(){string í="";foreach(var ǈ in ǵ){í+=ǈ.ô()+"\n";}if(í.Length>1){í=í.Substring(0
,í.Length-1);}return í;}IMyShipConnector Ǘ(ref string Z){if(Z.Contains("!")){string[]ǘ=Z.Trim().Split('!');if(ǘ.Length>1)
{string Ǚ=ǘ[1].TrimEnd();if(Ǚ.Length>0){long ǚ;bool Ǜ=long.TryParse(Ǚ,out ǚ);if(Ǜ){IMyShipConnector Ȃ=(IMyShipConnector)
GridTerminalSystem.GetBlockWithId(ǚ);if(Ȃ!=null){string ǝ="";string[]Ȣ=Z.Split('!');for(int ƌ=0;ƌ<Ȣ.Length-1;ƌ++){ǝ+=Ȣ[ƌ];}ǝ=ǝ.TrimEnd();Z
=ǝ;return Ȃ;}}}}}return null;}bool ɺ(ref string Z){if(Z.Length>1){if(Z[0]=='!'&&Z.Contains(" ")){string[]ǘ=Z.Split(' ');
if(ǘ.Length>1){if(ǘ[0].ToLower()=="!readonly"){Z=Z.Remove(0,10);return true;}}}}return false;}int ɻ=0;void Main(string Z,
UpdateType ɼ){ɻ+=1;if((ɼ&(UpdateType.Update1|UpdateType.Once|UpdateType.IGC))==0){if(Ȅ){if(Z.ToLower().Trim()==
"[data_output_request]"){Me.CustomData=ǜ();}Ȅ=false;systemsAnalyzer.Ǝ();}if(!Ȅ){if(!Ʈ){var Ǐ=systemsAnalyzer.Ɩ();var ɽ=ǒ(Z);if(Z.ToLower().Trim
()=="record"){if(Ǐ==null){ƶ();}else{ȁ.ā(
"WARNING\nPlease make sure you are not connected\nto a home connector before recording, "+your_title+".");}}else if(Z.ToLower().Trim()=="[data_output_request]"){Me.CustomData=ǜ();}else if(ɽ!=null){ǔ(ɽ);}else{
var ǌ=Ǘ(ref Z);bool ɾ=ɺ(ref Z);if(ɾ&&Ǐ!=null){if(Ǐ.Status==MyShipConnectorStatus.Connectable){Ǐ=null;}}if(Ǐ==null){if(Ȕ&&Z
==ǿ){ȁ.ā("STOPPED\nAwaiting orders, "+your_title+".");ȶ();}else{if(ǌ!=null){ǋ(Z,ǌ);}else{ǋ(Z);}}}else{if(!ɾ){if(ǌ!=null){Ǐ
=ǌ;}var ž=Ǎ(Z,Ǐ);ȁ.ā(ž);ȶ();}else{ȁ.ā("OVERRIDDEN\nShip save has been overriden\ndue to the !readonly command.");}}}}else
{if(Ư){Ʒ(Z);}else{var ǌ=Ǘ(ref Z);var Ǐ=systemsAnalyzer.Ɩ();if(Ǐ!=null&&ǌ!=null){Ǐ=ǌ;}ƫ(Ǐ,Z);}}}ȁ.j();}if(Ȕ&&!Ȅ){Ș+=
Runtime.TimeSinceLastRun.TotalSeconds;if(Ș>=ǳ){systemsAnalyzer.ī();if(ȡ&&enable_antenna_function)Ǵ.ȣ(1);ʂ();Ș=0;}}if(
enable_antenna_function&&Ȕ&&!Ȅ&&!ȡ){ș+=Runtime.TimeSinceLastRun.TotalSeconds;if(ș>=1){Ǵ.ȣ(1);ș=0;}}if((ɼ&UpdateType.IGC)!=0&&
enable_antenna_function)Ǵ.ȧ();}ɀ ɿ(string Z){var Ƭ=0;ɀ ʀ=null;foreach(var q in ǵ)if(q.ɑ.Contains(Z)){Ƭ+=1;if(ʀ==null)ʀ=q;}if(Ƭ>1)ȁ.ā(
"Minor Warning:\nThere are "+Ƭ+" places\nthat argument is associated with!\nPicking first one found, "+your_title+".");else if(Ƭ==0)ȁ.ā(
"WARNING:\nNo docking location found with that argument, "+your_title+".\nPlease dock to a connector and press 'Run' with your argument\nto save it as a docking location.");
return ʀ;}void ʂ(){if(systemsAnalyzer.q.ɣ.ContainsKey(ǿ)){List<ł>ǣ=systemsAnalyzer.q.ɣ[ǿ];if(Ȗ>=ǣ.Count){ɨ(true);}else{ł ɥ=ǣ[Ȗ
];ł ɮ=null;if(Ȗ<ǣ.Count-1){ɮ=ǣ[Ȗ+1];}double ɦ=ʡ(ɥ,ɮ);double ɧ=Math.Min(required_waypoint_accuracy,ɥ.Ŏ);if(ɦ<ɧ){Ȗ+=1;}}}
else{ɨ(false);}}void ɨ(bool ɩ){var ɪ=!small_ship_rotate_on_connector&&!systemsAnalyzer.ŧ||!large_ship_rotate_on_connector&&
systemsAnalyzer.ŧ;if(systemsAnalyzer.q.ɒ.Status==MyShipConnectorStatus.Connected||systemsAnalyzer.q.ɒ.Status==MyShipConnectorStatus.
Connectable&&ɪ){ȵ();}else{ȁ.o(ǿ);if(systemsAnalyzer.Ţ){systemsAnalyzer.Ǝ();systemsAnalyzer.Ţ=false;}if(Ȅ==false){double ɫ=3;var ɬ=
0.015;double ɭ=5;if(speedSetting==1){ɭ=7;Ț=10;}else if(speedSetting==3){ɭ=4;Ț=topSpeed;ɬ=0.03;}else{ɭ=6;Ț=topSpeed;}if(
extra_soft_landing_mode)ɭ=8;ɭ+=connector_clearance;if(ɩ)ɭ=0;if(Ț>topSpeed)Ț=topSpeed;if(systemsAnalyzer.q.ɞ.Length()>5){ɬ=0.035;ɫ=5;}var ɯ=1-((
systemsAnalyzer.q.ɞ.Length()/100)*0.2*high_speed_lead_amount);var ɸ=systemsAnalyzer.q.ɗ+Ȁ*systemsAnalyzer.q.ɞ*ɯ;var ɰ=systemsAnalyzer.q
.ǥ;var ɱ=systemsAnalyzer.q.ɛ;var ɲ=ɸ+ɰ*ɭ;var ɳ=systemsAnalyzer.q.ɒ.GetPosition();var ɴ="Starting...";var ɵ=new ł(ɲ,ɰ,ɱ);
double ɶ;var ɷ=false;if(!ɪ&&systemsAnalyzer.q.ɒ.Status==MyShipConnectorStatus.Connectable){ɶ=Ƞ(ɵ,true);if(Math.Abs(ǽ)<ɬ){ȵ();ɷ
=true;}}else{var ʁ=false;if(rotate_on_approach&&Ȏ)ʁ=true;ɶ=Ƞ(ɵ,ʁ);}Ȏ=false;if(!ɷ){if(Math.Abs(ɶ)<15){var ʃ=M.w(ɸ,ɰ,ɳ);var
ʩ=ʃ-ɸ;var ʙ=ɰ.Dot(Vector3D.Normalize(ʩ))*ʩ.Length();var ʚ=(ɳ-ʃ).Length();if(ʚ>ɫ&&ʙ<ɭ*0.9&&!ɩ){const double ʛ=2;var ʜ=new
ł(ɳ+ɰ*(-ʙ+ʛ+ɭ),ɰ,ɱ);ʜ.ń=20;ʜ.ŉ=0.8;if(speedSetting==1)ɵ.ń=8;else if(speedSetting==3)ɵ.ń=20;else ɵ.ń=10;ʣ(ʜ);ɴ=
"Behind target, moving to be in front";}else if(ʚ>ɫ&&!ɩ){if(speedSetting==1)ɵ.ń=5;else if(speedSetting==3)ɵ.ń=15;else ɵ.ń=5;ʣ(ɵ);ɴ="Moving toward connector";}
else{var ʝ=systemsAnalyzer.q.ɚ+Ɔ.Ɛ(systemsAnalyzer.q.ɒ);var ʞ=new ł(ɸ+ɰ*ʝ,ɰ,ɱ);ʞ.ń=3;if(speedSetting==1)ɵ.ń=1;else if(
speedSetting==3)ɵ.ń=3;else ɵ.ń=1;if(extra_soft_landing_mode)Ț=2;var ʟ=ʣ(ʞ);ɴ="landing on connector";Ȏ=true;}}else{ȗ="Rotating";ɴ=
"Rotating to connector";}if(extra_info){ȁ.ā("Status: "+ȗ);ȁ.ā("Place in sequence: "+ɴ);}var ʠ=DateTime.Now-ȕ;ȁ.ā("\nTime elapsed: "+ʠ.Seconds+
"."+ʠ.Milliseconds.ToString().Substring(0,1));ȁ.j();}}}}double ʡ(ł ɥ,ł ɮ){ȁ.o(ǿ);if(systemsAnalyzer.Ţ){systemsAnalyzer.Ǝ();
systemsAnalyzer.Ţ=false;}if(Ȅ==true)return 0;if(speedSetting==1){Ț=10;}else if(speedSetting==3){Ț=ɥ.Ň;}else{Ț=ɥ.Ň;}if(Ț>ɥ.Ň)Ț=ɥ.Ň;if(
rotate_during_waypoints&&ɥ.Ŋ){Ȉ(ɥ);}if(speedSetting==1)ɥ.ń=15;else if(speedSetting==3)ɥ.ń=15;else ɥ.ń=20;double ʢ=ʣ(ɥ);if(extra_info){ȁ.ā(
"Status: "+ȗ);ȁ.ā("Moving to waypoint: "+(Ȗ+1).ToString()+".");}else{ȁ.ā("Moving to waypoint: "+(Ȗ+1).ToString()+".");}var ʠ=
DateTime.Now-ȕ;ȁ.ā("\nTime elapsed: "+ʠ.Seconds+"."+ʠ.Milliseconds.ToString().Substring(0,1));ȁ.j();return ʢ;}double ʣ(ł ȉ){ȃ=
Runtime.TimeSinceLastRun.TotalSeconds*10;Ȁ=(DateTime.Now-Ȑ).TotalSeconds;var ʤ=systemsAnalyzer.ũ.GetShipVelocities().
LinearVelocity;var ʥ=ʤ-ȑ;var ʦ=Vector3D.Zero;if(Ȁ>0)ʦ=ʥ/Ȁ;systemsAnalyzer.ę();ĝ ʧ=null;ȗ="ERROR";var ʨ=-systemsAnalyzer.q.ɔ*ȓ;var ɤ=(
systemsAnalyzer.ũ.GetNaturalGravity()+ʨ)*systemsAnalyzer.Ŭ;Vector3D ʘ=ȉ.ň;if(ȉ.ŋ){ʘ=ɀ.ç(ʘ,systemsAnalyzer.q);}var ʄ=ʘ-systemsAnalyzer.q
.ɒ.GetPosition();if(ȉ.ŋ){ʄ=ʘ-Me.CubeGrid.GetPosition();}var ʍ=Vector3D.Normalize(ʄ);var ʅ=ʄ.Length();var ʆ=(ʤ-
systemsAnalyzer.q.ɞ).Length()+ȃ*(ȉ.ń+Ƕ+add_acceleration);if(ʆ>Ț)ʆ=Ț;var ʇ=ʍ*ʆ+systemsAnalyzer.q.ɞ;var ʈ=ʤ-ʇ;double ʉ;if(ʈ.Length()==0){
ʉ=0;}else{var ʊ=Vector3D.Normalize(ʄ);ʊ=-Vector3D.Normalize(ʈ);ʧ=systemsAnalyzer.ƅ(ɤ,ʊ);if(ʧ==null){ȁ.ā(
"Not enough thrust!");ȑ=ʤ;ȓ=0;Ȑ=DateTime.Now;return ʅ;}ʉ=ʧ.ģ/systemsAnalyzer.Ŭ;}var ʋ=systemsAnalyzer.q.ɞ;var ʌ=ʤ-ʋ;double ʎ;if(ʌ.Length()==
0){ʎ=0;}else{var ʗ=-Vector3D.Normalize(ʄ);ʧ=systemsAnalyzer.ƅ(ɤ,ʗ);if(ʧ==null){ȁ.ā("Not enough thrust!");ȓ=0;ȑ=ʤ;Ȑ=
DateTime.Now;return ʅ;}ʎ=ʧ.ģ/systemsAnalyzer.Ŭ;}double ʏ=0;var ʐ=false;if(ʎ!=0){double ʑ=0;ʑ=ʌ.Length()/(ʎ*(1-caution)*ȉ.ņ);ʑ+=ȃ
;ʏ=ʌ.Length()*ʑ/2;}if(ʏ+ȉ.ŉ<ʅ)ʐ=true;if(ʐ){var ʒ=-ʈ/ȃ;var ʓ=ʒ*systemsAnalyzer.Ŭ;var ʔ=ʒ.Length();if(ʔ>ʉ){ʧ=
systemsAnalyzer.ƅ(ɤ,Vector3.Normalize(ʒ));ȗ="Speeding up";}else{ʧ=systemsAnalyzer.Ƣ(ɤ,ʓ);ȗ="Drifting";}}else{var ʕ=-ʌ/ȃ;var ʖ=ʕ*
systemsAnalyzer.Ŭ;var ʔ=ʕ.Length();if(ʔ>ʎ){ʧ=systemsAnalyzer.ƅ(ɤ,Vector3.Normalize(ʕ));ȗ="Slowing down";}else{ʧ=systemsAnalyzer.Ƣ(ɤ,ʖ);
ȗ="Finished";}}ȴ(ʧ);ȑ=ʤ;Ȑ=DateTime.Now;return ʅ;}void ɹ(Vector3D ɤ,Vector3D ȯ,double Ȳ){var ȳ=systemsAnalyzer.ƅ(-ɤ,ȯ,Ȳ);ǹ
.Ņ(ȳ.ġ[3],0);ǹ.Ņ(ȳ.ġ[4],0);ǹ.Ņ(ȳ.ġ[5],0);ǹ.Ņ(ȳ.ġ[0],ȳ.Ģ.X);ǹ.Ņ(ȳ.ġ[1],ȳ.Ģ.Y);ǹ.Ņ(ȳ.ġ[2],ȳ.Ģ.Z);}void ȴ(ĝ ȳ){ǹ.Ņ(ȳ.ġ[3],0)
;ǹ.Ņ(ȳ.ġ[4],0);ǹ.Ņ(ȳ.ġ[5],0);ǹ.Ņ(ȳ.ġ[0],ȳ.Ģ.X);ǹ.Ņ(ȳ.ġ[1],ȳ.Ģ.Y);ǹ.Ņ(ȳ.ġ[2],ȳ.Ģ.Z);}void ȵ(){systemsAnalyzer.q.ɒ.Connect(
);ȁ.Ā();ȁ.ā("DOCKED\nThe ship has docked "+your_title+"!\nI will patiently await for more orders in the future.");ȁ.j();ȁ
.Ą();ȶ();}void ȶ(){Runtime.UpdateFrequency=UpdateFrequency.None;Ȕ=false;Ȓ="";if(systemsAnalyzer!=null){foreach(var Š in
systemsAnalyzer.Ŧ)if(Š!=null)if(Š.IsWorking)Š.SetValue("Override",false);foreach(var ĸ in systemsAnalyzer.Ħ)if(ĸ!=null)if(ĸ.IsWorking)ĸ
.SetValue("Override",0f);}}class ȷ{private const string ȸ="Spug's position update response";private const string ȹ=
"Spug's position update request";private const string Ⱥ="Spug's recall request";private IMyBroadcastListener Ȼ;private IMyUnicastListener ȼ;public
IMyRadioAntenna Ƚ;public List<IMyTerminalBlock>Ǿ=new List<IMyTerminalBlock>();public Program ð;public ȷ(Program Ⱦ){ð=Ⱦ;Ȼ=ð.IGC.
RegisterBroadcastListener(Ⱥ);ȼ=ð.IGC.UnicastListener;ȼ.SetMessageCallback("UNICAST");Ȼ.SetMessageCallback(Ⱥ);}public string ȿ(){var Ȱ=0;ð.
GridTerminalSystem.GetBlocks(Ǿ);foreach(var ù in Ǿ)if(ù is IMyRadioAntenna&&ú(ù)){var ȩ=(IMyRadioAntenna)ù;if(Ȱ<1){Ȱ=1;Ƚ=ȩ;}if(ȩ.Enabled&&
Ȱ<2){Ȱ=2;Ƚ=ȩ;}if(ȩ.EnableBroadcasting&&ȩ.Enabled&&Ȱ<3){Ȱ=3;Ƚ=ȩ;}}if(Ȱ==3)return
"Antenna found:\nReady for use with the\noptional home script.";return"";}public void ȣ(long Ȥ){var ȥ=ð.systemsAnalyzer.q.ɖ;var Ȧ=ð.systemsAnalyzer.q.ɝ;ð.IGC.SendBroadcastMessage(ȹ,ȥ+
";"+Ȧ);}public bool ú(IMyTerminalBlock ù){return ù.CubeGrid.EntityId==ð.Me.CubeGrid.EntityId;}public void ȧ(){while(Ȼ.
HasPendingMessage){var Ȩ=Ȼ.AcceptMessage();if(Ȩ.Tag==Ⱥ){var Ǌ=Ȩ.Data.ToString();var ñ=Ǌ.Split(';');var e=ñ[0];long Ȫ=0;long.TryParse(ñ[1]
,out Ȫ);ð.ȁ.ā("Broadcast received. This ship has been ordered to dock.");var ȫ=ð.ɿ(e);if(ȫ!=null&&Ȫ!=ð.Me.CubeGrid.
EntityId)ð.Main(e,UpdateType.Script);}}var Ȭ=false;do{Ȭ=false;if(ȼ.HasPendingMessage){Ȭ=true;var ȭ=ȼ.AcceptMessage();if(ȭ.Tag==ȸ
)Ȯ(ȭ.Data.ToString());}}while(Ȭ);}private void Ȯ(string Ǌ){var ñ=Ǌ.Split(';');if(ñ.Length==1){ð.ȁ.Ā();ð.ȁ.ā(Ǌ);ð.ȶ();ð.ȁ.
j();}else if(ñ.Length>1){ð.ȡ=true;var ž=ð.systemsAnalyzer.q.Ċ(ñ);if(ž.Length>0)ð.Ȓ=ž;}}}class ɀ{private const char ɢ='¬';
private const char ɐ='`';public HashSet<string>ɑ=new HashSet<string>();public IMyShipConnector ɒ;public long ɓ;public Vector3D
ɔ=Vector3D.Zero;public Vector3D ɕ=Vector3D.Zero;public Vector3D ǥ;public long ɖ;public Vector3D Ǧ;public Vector3D ɗ;
public string ɘ=null;public string ə=null;public double ɚ;public Vector3D ɛ;public Vector3D ɜ;public long ɝ;public Vector3D ɞ=
Vector3D.Zero;private const char ɟ='ç';private const char ɠ='å';private const char ɡ='ã';public Dictionary<string,List<ł>>ɣ=new
Dictionary<string,List<ł>>();public void ɏ(string Ǌ){ɣ=new Dictionary<string,List<ł>>();if(Ǌ!=""){string[]ɇ=Ǌ.Split(ɟ);foreach(
string Ɂ in ɇ){if(Ɂ!=""){string[]ɂ=Ɂ.Split(ɠ);if(ɂ.Length>1){string Ƀ=ɂ[0];List<ł>Ʉ=new List<ł>();for(int ƌ=1;ƌ<ɂ.Length;ƌ++){
if(ɂ[ƌ]!=""){string[]Ʌ=ɂ[ƌ].Split(ɡ);Vector3D Ɇ=new Vector3D();Vector3D Ń=new Vector3D();Vector3D Ɉ=new Vector3D();double
ɍ=0.2;double Ň=1;bool ɉ=true;Vector3D.TryParse(Ʌ[0],out Ɇ);Vector3D.TryParse(Ʌ[1],out Ń);Vector3D.TryParse(Ʌ[2],out Ɉ);
double.TryParse(Ʌ[3],out Ň);bool.TryParse(Ʌ[4],out ɉ);double.TryParse(Ʌ[5],out ɍ);ł Ǒ=new ł(Ɇ,Ń,Ɉ);Ǒ.ŋ=true;Ǒ.Ŏ=ɍ;Ǒ.Ň=Ň;Ǒ.Ŋ=ɉ;
Ʉ.Add(Ǒ);}}if(Ʉ.Count>0){ɣ[Ƀ]=Ʉ;}}}}}}public string Ɋ(){string í="";foreach(KeyValuePair<string,List<ł>>ɋ in ɣ){string Ɍ=
ɋ.Key+ɠ;foreach(ł ȉ in ɋ.Value){string Ɏ="";Ɏ+=ȉ.ň.ToString()+ɡ;Ɏ+=ȉ.Ń.ToString()+ɡ;Ɏ+=ȉ.Ō.ToString()+ɡ;Ɏ+=ȉ.Ň.ToString()
+ɡ;Ɏ+=ȉ.Ŋ.ToString()+ɡ;Ɏ+=ȉ.Ŏ.ToString();Ɍ+=Ɏ+ɠ;}í+=Ɍ+ɟ;}return í;}public void ƪ(string ë){string[]È=ë.Split(ɠ);ə=È[0];ɘ=
È[1];}public string ì(){string í="";í+=ə+ɠ;í+=ɘ;return í;}public ɀ(string î,IMyShipConnector ä,IMyShipConnector Ü){ɑ.Add(
î);Û(ä,Ü);}public ɀ(string ï,Program ð){var ñ=ï.Split(ɢ);if(ñ.Length==8){long.TryParse(ñ[0],out ɓ);long.TryParse(ñ[1],out
ɖ);Vector3D.TryParse(ñ[2],out ɗ);Vector3D.TryParse(ñ[3],out ǥ);Vector3D.TryParse(ñ[4],out ɛ);long.TryParse(ñ[5],out ɝ);
double.TryParse(ñ[6],out ɚ);var ò=ñ[7].Split(ɐ);foreach(var e in ò)ɑ.Add(e);ĉ(ð);}else if(ñ.Length==12){long.TryParse(ñ[0],out
ɓ);long.TryParse(ñ[1],out ɖ);Vector3D.TryParse(ñ[2],out ɗ);Vector3D.TryParse(ñ[3],out ǥ);Vector3D.TryParse(ñ[4],out ɛ);
Vector3D.TryParse(ñ[5],out ɜ);Vector3D.TryParse(ñ[6],out Ǧ);long.TryParse(ñ[7],out ɝ);double.TryParse(ñ[8],out ɚ);ɏ(ñ[9]);ƪ(ñ[10
]);var ò=ñ[11].Split(ɐ);foreach(var e in ò)ɑ.Add(e);ĉ(ð);}else{ɒ=null;}}public string ó(){var í="";í+=ɓ.ToString()+ɢ;í+=ɖ
.ToString()+ɢ;í+=ɗ.ToString()+ɢ;í+=ǥ.ToString()+ɢ;í+=ɛ.ToString()+ɢ;í+=ɜ.ToString()+ɢ;í+=Ǧ.ToString()+ɢ;í+=ɝ.ToString()+ɢ
;í+=ɚ.ToString()+ɢ;í+=Ɋ()+ɢ;í+=ì()+ɢ;foreach(var e in ɑ)í+=e+ɐ;í=í.Substring(0,í.Length-1);return í;}public string ô(){
var í="";foreach(var e in ɑ){í+=ə+";";í+=ɘ+";";í+=ɒ.CustomName+";";í+=e+";";int ê=0;if(ɣ.ContainsKey(e)){ê=ɣ[e].Count;}í+=ê
.ToString();í+="\n";}if(í.Length>1){í=í.Substring(0,í.Length-1);}return í;}public void Û(IMyShipConnector ä,
IMyShipConnector Ü){ɓ=ä.EntityId;ɒ=ä;ɖ=Ü.EntityId;ɗ=Ü.GetPosition();ǥ=Ü.WorldMatrix.Forward;Ǧ=Ü.WorldMatrix.Left;ɘ=Ü.CustomName;ə=Ü.
CubeGrid.CustomName;var Ý=Vector3D.Normalize(M.À(ǥ,Vector3D.Zero,ä.WorldMatrix.Left));var Þ=Ý.Cross(ǥ);ɛ=Þ;ɜ=ß(ɛ,Ü.WorldMatrix);
ɝ=Ü.CubeGrid.EntityId;ɚ=Ɔ.Ɛ(Ü);}public static Vector3D ß(Vector3D à,MatrixD á){return Vector3D.TransformNormal(à,MatrixD.
Transpose(á));}public static Vector3D â(Vector3D ã,MatrixD á){return Vector3D.TransformNormal(ã,á);}public static Vector3D â(
Vector3D ã,ɀ å){MatrixD æ=Matrix.CreateWorld(å.ɗ,å.ǥ,(-å.Ǧ).Cross(å.ǥ));return Vector3D.TransformNormal(ã,æ);;}public static
Vector3D ç(Vector3D è,MatrixD á){return Vector3D.Transform(è,á);}public static Vector3D ç(Vector3D è,ɀ å){MatrixD æ=Matrix.
CreateWorld(å.ɗ,å.ǥ,(-å.Ǧ).Cross(å.ǥ));return Vector3D.Transform(è,æ);}public static Vector3D ĕ(Vector3D ć,MatrixD á){Vector3D Ĉ=ć-
á.Translation;return Vector3D.TransformNormal(Ĉ,MatrixD.Transpose(á));}public void ĉ(Program ð){ɒ=(IMyShipConnector)ð.
GridTerminalSystem.GetBlockWithId(ɓ);}public string Ċ(string[]ñ){var ċ="";if(ñ.Length==6){Vector3D.TryParse(ñ[0],out ɗ);Vector3D.TryParse(
ñ[1],out ǥ);Vector3D.TryParse(ñ[2],out Ǧ);Vector3D.TryParse(ñ[3],out ɞ);Vector3D.TryParse(ñ[4],out ɕ);Vector3D.TryParse(ñ
[5],out ɔ);if(ɜ!=Vector3D.Zero){MatrixD æ=Matrix.CreateWorld(ɗ,ǥ,(-Ǧ).Cross(ǥ));var Č=â(ɜ,æ);ɛ=Č;}}else{ċ=
"Warning:\nGot a corrupted message back from the optional home script.\nMaybe it is an old version?";}return ċ;}public override bool Equals(object č){if(č==null||GetType()!=č.GetType())return false;var Ď=(ɀ)č;if(ɓ==Ď.ɓ&&
ɖ==Ď.ɖ)return true;return false;}public override int GetHashCode(){var ď=-48872655;ď=ď*-1521134295+EqualityComparer<
HashSet<string>>.Default.GetHashCode(ɑ);ď=ď*-1521134295+ɓ.GetHashCode();ď=ď*-1521134295+ɖ.GetHashCode();return ď;}}class Đ{
private Program ð;private string đ="";public List<IMyTextSurface>Ē=new List<IMyTextSurface>();public List<IMyTimerBlock>ē=new
List<IMyTimerBlock>();public List<IMyTimerBlock>Ĕ=new List<IMyTimerBlock>();public Đ(Program Ė){ð=Ė;ö();}public static
double Ć(double º,int ü){if(º==0)return 0;var õ=Math.Pow(10,Math.Floor(Math.Log10(Math.Abs(º)))+1);return õ*Math.Round(º/õ,ü);
}public void ö(){Ĕ=new List<IMyTimerBlock>();if(ð.force_timer_search_on_station){List<IMyTerminalBlock>ø=new List<
IMyTerminalBlock>();ð.GridTerminalSystem.GetBlocks(ø);ē=new List<IMyTimerBlock>();Ē=new List<IMyTextSurface>();foreach(var ù in ø){if(ù
is IMyTimerBlock&&ù.CustomName.ToLower().Contains(ð.timer_tag))ē.Add((IMyTimerBlock)ù);if(ù is IMyTextSurface&&ù.
CustomName.ToLower().Contains(ð.lcd_tag))Ē.Add((IMyTextSurface)ù);if(ù is IMyTimerBlock&&ù.CustomName.ToLower().Contains(ð.
start_timer_tag)&&ú(ù))Ĕ.Add((IMyTimerBlock)ù);}}else{ē=new List<IMyTimerBlock>();Ē=new List<IMyTextSurface>();foreach(var ù in ð.Ǿ){if
(ù is IMyTimerBlock&&ù.CustomName.ToLower().Contains(ð.timer_tag)&&ú(ù))ē.Add((IMyTimerBlock)ù);if(ù is IMyTextSurface&&ù
.CustomName.ToLower().Contains(ð.lcd_tag)&&ú(ù))Ē.Add((IMyTextSurface)ù);if(ù is IMyTimerBlock&&ù.CustomName.ToLower().
Contains(ð.start_timer_tag)&&ú(ù))Ĕ.Add((IMyTimerBlock)ù);}}}public bool ú(IMyTerminalBlock ù){return ù.CubeGrid.EntityId==ð.Me.
CubeGrid.EntityId;}public void û(string ý){if(!ð.Ȅ)đ="";ā("ERROR:\n"+ý);ð.Ȅ=true;ð.ȶ();j();}public void ą(Vector3D þ,string ÿ=
"0"){ā("GPS:"+ÿ+":"+þ.X+":"+þ.Y+":"+þ.Z+":");}public void Ā(){đ="";}public void ā(object Ă){đ+=Ă+"\n";}public static string
ă(string Z){if(Z=="")return"no argument";return Z;}public void Ą(){if(ð.force_timer_search_on_station){ö();}if(ē.Count>0)
foreach(var c in ē)if(c!=null)if(c.IsWorking)c.Trigger();}public void Ú(){if(Ĕ.Count>0)foreach(var c in Ĕ)if(c!=null)if(c.
IsWorking)c.Trigger();}public void Ø(string e,int f,string h){if(f==0){ā("RECORDING MODE\nRecording to argument: "+ă(e)+
".\nPressing Run will record\nposition and rotation. To finish, press Run when docked.\n\nTo cancel, press Recompile.");}else{ā("RECORDING MODE\nRecorded "+f+" waypoints to argument: "+ă(e)+h+"\nPressing Run will record position and rotation again. To finish, press Run when docked.\n\nTo cancel, press Recompile."
);}}public void j(bool k=false,float l=1){if(đ!=""){if(ð.Ȓ.Length>0)ð.Ȓ+="\n";var m="= Spug's Auto Docking 2.0 =\n\n"+ð.Ȓ
+đ;ð.Echo(m);if(!k&&Ē.Count>0)foreach(var n in Ē)if(n!=null){n.ContentType=ContentType.TEXT_AND_IMAGE;n.WriteText(m);}đ=
"";}}public void p(){ā("Known docking locations:");var f=1;foreach(var q in ð.ǵ){var r="- Location "+f+" arguments: ";
foreach(var e in q.ɑ){var s=e;if(e=="")s="NO ARG";r+=s+", ";}ā(r.Substring(0,r.Length-2));f+=1;}}public string t(ɀ q){var r="";
foreach(var e in q.ɑ){var s=e;if(e=="")s="NO ARG";r+=s+", ";}if(r.Length>2)r=r.Substring(0,r.Length-2);return r;}public void o(
string Z){if(Z=="")ā("RUNNING\nAttempting docking sequence\nwith no argument.");else ā(
"RUNNING\nAttempting docking sequence\nwith argument: "+Z);}}class M{private bool B;private double C;private double D;private double E;private double F;private double G;
private double H;private double L;private bool O=true;private double W;private double P;private double Q;public M(double R,
double S,double T,double U,double V,double A){F=R;E=S;D=T;G=U;H=V;Q=A;W=1/Q;B=false;}public M(double R,double S,double T,
double Ê,double A){F=R;E=S;D=T;Q=A;W=1/Q;C=Ê;B=true;}public double Ë{get;private set;}public static Vector3D Ì(Vector3D u,
Vector3D Á){var Í=u.Dot(Á)/Á.LengthSquared()*Á;return Í;}public static int Î(Vector3D u,Vector3D Á){var Ï=u.Dot(Á);if(Ï<0)return
-1;return 1;}public static double Ð(Vector3D Ñ,Vector3D Ò,Vector3D Ó,bool Ô=false){Vector3D Õ=Ñ;if(Ô){Õ=À(Ó,Vector3D.Zero
,Ñ);}double Ö=Math.Acos(MathHelper.Clamp(Ò.Dot(Õ),-1,1));Vector3D Ù=Ò.Cross(Õ);if(Ó.Dot(Ù)<0){Ö=-Ö;}return Ö;}public
static double É(Vector3D u,Vector3D Á){if(u.LengthSquared()==0||Á.LengthSquared()==0)return 0;return Math.Acos(MathHelper.
Clamp(u.Dot(Á)/u.Length()/Á.Length(),-1,1));}public static Vector3D w(Vector3D x,Vector3D y,Vector3D z){var ª=Vector3D.
Normalize(y);var µ=z-x;var º=µ.Dot(ª);return x+ª*º;}public static Vector3D À(Vector3D Â,Vector3D Æ,Vector3D z){double Ã;Vector3D
Ä;Ã=Å(Â,Æ,z);Ã*=-1;Ä=Ç(Â,Ã);return z+Ä;}public static double Å(Vector3D Â,Vector3D Æ,Vector3D z){return Vector3D.Dot(Â,z-
Æ);}public static Vector3D Ç(Vector3D é,double ė){var Ų=Vector3D.Normalize(é);return Ų*=ė;}public static void Ŵ(double[,]
ŵ,double[]Ŷ){int ŷ,Ÿ,Ź,ź,Ż;Ż=Ŷ.Length;for(Ź=0;Ź<Ż;Ź++){ź=Ź+1;for(ŷ=Ź;ŷ<Ż;ŷ++)if(ŵ[ŷ,Ź]!=0){for(Ÿ=ź;Ÿ<Ż;Ÿ++)ŵ[ŷ,Ÿ]/=ŵ[ŷ,Ź]
;Ŷ[ŷ]/=ŵ[ŷ,Ź];}for(ŷ=ź;ŷ<Ż;ŷ++)if(ŵ[ŷ,Ź]!=0){for(Ÿ=ź;Ÿ<Ż;Ÿ++)ŵ[ŷ,Ÿ]-=ŵ[Ź,Ÿ];Ŷ[ŷ]-=Ŷ[Ź];}}for(ŷ=Ż-2;ŷ>=0;ŷ--)for(Ÿ=Ż-1;Ÿ>=
ŷ+1;Ÿ--)Ŷ[ŷ]-=ŵ[ŷ,Ÿ]*Ŷ[Ÿ];}public static void ż(ref Matrix3x3 Ž,out Matrix3x3 ž){var ſ=Ž.Determinant();var ƀ=1f/ſ;ž.M11=(
Ž.M22*Ž.M33-Ž.M32*Ž.M23)*ƀ;ž.M12=(Ž.M13*Ž.M32-Ž.M12*Ž.M33)*ƀ;ž.M13=(Ž.M12*Ž.M23-Ž.M13*Ž.M22)*ƀ;ž.M21=(Ž.M23*Ž.M31-Ž.M21*Ž
.M33)*ƀ;ž.M22=(Ž.M11*Ž.M33-Ž.M13*Ž.M31)*ƀ;ž.M23=(Ž.M21*Ž.M13-Ž.M11*Ž.M23)*ƀ;ž.M31=(Ž.M21*Ž.M32-Ž.M31*Ž.M22)*ƀ;ž.M32=(Ž.
M31*Ž.M12-Ž.M11*Ž.M32)*ƀ;ž.M33=(Ž.M11*Ž.M22-Ž.M21*Ž.M12)*ƀ;}public double Ɓ(double Ƃ){var ƃ=(Ƃ-P)*W;if(O){ƃ=0;O=false;}if(!
B){L+=Ƃ*Q;if(L>H)L=H;else if(L<G)L=G;}else{L=L*(1.0-C)+Ƃ*Q;}P=Ƃ;Ë=F*Ƃ+E*L+D*ƃ;return Ë;}public double Ɓ(double Ƃ,double A
){Q=A;W=1/Q;return Ɓ(Ƃ);}public void Ƅ(){L=0;P=0;O=true;}}class Ɔ{public ĝ ų;public bool Ţ=false;public IMyShipController
ũ;public ɀ q;public ĝ ţ;private bool Ť=true;public ĝ ť;public List<IMyGyro>Ŧ=new List<IMyGyro>();public bool ŧ;public ĝ Ũ
;public Program ð;public float Ū=9999;public IMyRemoteControl ű;public ĝ ū;public float Ŭ=9999;public Dictionary<
Base6Directions.Direction,ĝ>ŭ;public List<IMyThrust>Ħ=new List<IMyThrust>();double Ů=0;public ĝ ů;public Ɔ(Program Ű){ð=Ű;ð.ȁ.ā(
"INITIALIZED");Ǝ();}public ĝ ƅ(Vector3D Ƈ,Vector3D ƨ,double Ƙ=1){Base6Directions.Direction ƙ;Base6Directions.Direction ƚ;double ƛ;
double Ɯ;double Ɲ;ĝ ƍ;ĝ ƞ;ĝ Ɵ;if(Ƙ>1)Ƙ=1;else if(Ƙ<0)Ƙ=0;foreach(var Ě in ŭ){ƍ=Ě.Value;if(ƍ.Ĥ==Base6Directions.Direction.Up||ƍ
.Ĥ==Base6Directions.Direction.Down){ƞ=ŭ[Base6Directions.Direction.Left];Ɵ=ŭ[Base6Directions.Direction.Forward];}else if(ƍ
.Ĥ==Base6Directions.Direction.Left||ƍ.Ĥ==Base6Directions.Direction.Right){ƞ=ŭ[Base6Directions.Direction.Up];Ɵ=ŭ[
Base6Directions.Direction.Forward];}else if(ƍ.Ĥ==Base6Directions.Direction.Forward||ƍ.Ĥ==Base6Directions.Direction.Backward){ƞ=ŭ[
Base6Directions.Direction.Up];Ɵ=ŭ[Base6Directions.Direction.Left];}else{ð.ȁ.û("Encountered unusual thruster direction.\nIf you've gotten this error in particular,\nplease report it to the script owner, Spug."
);ƞ=ŭ[Base6Directions.Direction.Up];Ɵ=ŭ[Base6Directions.Direction.Left];}ƍ.ĥ[0,0]=ƞ.ļ.X;ƍ.ĥ[0,1]=Ɵ.ļ.X;ƍ.ĥ[0,2]=-ƨ.X;ƍ.ĥ[
1,0]=ƞ.ļ.Y;ƍ.ĥ[1,1]=Ɵ.ļ.Y;ƍ.ĥ[1,2]=-ƨ.Y;ƍ.ĥ[2,0]=ƞ.ļ.Z;ƍ.ĥ[2,1]=Ɵ.ļ.Z;ƍ.ĥ[2,2]=-ƨ.Z;ƍ.Ğ[0]=-ƍ.Ę*ƍ.ļ.X*Ƙ-Ƈ.X;ƍ.Ğ[1]=-ƍ.Ę*ƍ
.ļ.Y*Ƙ-Ƈ.Y;ƍ.Ğ[2]=-ƍ.Ę*ƍ.ļ.Z*Ƙ-Ƈ.Z;M.Ŵ(ƍ.ĥ,ƍ.Ğ);ƛ=ƍ.Ğ[0];Ɯ=ƍ.Ğ[1];Ɲ=ƍ.Ğ[2];ƙ=ƞ.Ĥ;ƚ=Ɵ.Ĥ;if(ƛ<0){ƙ=Base6Directions.
GetOppositeDirection(ƞ.Ĥ);ƛ*=-1;}if(Ɯ<0){ƚ=Base6Directions.GetOppositeDirection(Ɵ.Ĥ);Ɯ*=-1;}ƍ.Ģ.X=ƍ.Ę*Ƙ;ƍ.Ģ.Y=ƛ;ƍ.Ģ.Z=Ɯ;ƍ.ģ=Ɲ;ƍ.ġ[0]=ƍ;ƍ.ġ[1
]=ŭ[ƙ];ƍ.ġ[2]=ŭ[ƚ];ƍ.ġ[3]=ŭ[Base6Directions.GetOppositeDirection(ƍ.Ĥ)];ƍ.ġ[4]=ŭ[Base6Directions.GetOppositeDirection(ƞ.Ĥ)
];ƍ.ġ[5]=ŭ[Base6Directions.GetOppositeDirection(Ɵ.Ĥ)];}ĝ Ơ=null;double ơ=-999999999;foreach(var Ě in ŭ)if(Ě.Value.ģ>ơ)if(
Ě.Value.Ģ.Y<=Ě.Value.ġ[1].Ę+1&&Ě.Value.Ģ.Z<=Ě.Value.ġ[2].Ę+1){Ơ=Ě.Value;ơ=Ě.Value.ģ;}return Ơ;}public ĝ Ƣ(Vector3D ƣ,
Vector3D Ƥ){var ƥ=-ƣ+Ƥ;var ƍ=Ʀ(ƥ);ƍ.ĥ[0,0]=ƍ.ġ[0].ļ.X;ƍ.ĥ[0,1]=ƍ.ġ[1].ļ.X;ƍ.ĥ[0,2]=ƍ.ġ[2].ļ.X;ƍ.ĥ[1,0]=ƍ.ġ[0].ļ.Y;ƍ.ĥ[1,1]=ƍ.ġ[1
].ļ.Y;ƍ.ĥ[1,2]=ƍ.ġ[2].ļ.Y;ƍ.ĥ[2,0]=ƍ.ġ[0].ļ.Z;ƍ.ĥ[2,1]=ƍ.ġ[1].ļ.Z;ƍ.ĥ[2,2]=ƍ.ġ[2].ļ.Z;ƍ.Ğ[0]=ƥ.X;ƍ.Ğ[1]=ƥ.Y;ƍ.Ğ[2]=ƥ.Z;M.
Ŵ(ƍ.ĥ,ƍ.Ğ);ƍ.Ģ.X=ƍ.Ğ[0];ƍ.Ģ.Y=ƍ.Ğ[1];ƍ.Ģ.Z=ƍ.Ğ[2];return ƍ;}public ĝ Ʀ(Vector3D Ƨ){var Ʃ=new List<ĝ>();var Ɨ=new List<ĝ>(
);var ƈ=Vector3D.Normalize(Ƨ);foreach(var Ě in ŭ){var Ɖ=Ě.Value;if(Ɖ.ļ.Dot(ƈ)>0)Ʃ.Add(Ɖ);else Ɨ.Add(Ɖ);}if(Ʃ.Count>3){Ʃ.
Sort(delegate(ĝ Ɗ,ĝ Ƌ){if(Ɗ.ļ.Dot(ƈ)>Ƌ.ļ.Dot(ƈ))return-1;return 1;});for(var ƌ=3;ƌ<Ʃ.Count;ƌ++)Ɨ.Add(Ʃ[ƌ]);ð.ȁ.ā(
"Warning, more than 3 viable thruster groups found! removing extra ones");}if(Ʃ.Count<3)ð.ȁ.û(
"Only two viable thruster groups found!\nPlease rotate the ship slightly and recompile, that could fix this.");var ƍ=Ʃ[0];ƍ.ġ[0]=Ʃ[0];ƍ.ġ[1]=Ʃ[1];ƍ.ġ[2]=Ʃ[2];ƍ.ġ[3]=Ɨ[0];ƍ.ġ[4]=Ɨ[1];ƍ.ġ[5]=Ɨ[2];return ƍ;}public void Ǝ(){if(!Ť&&!ð
.Ȕ)ð.ȁ.ā("RE-INITIALIZED\nSome change was detected\nso I have re-checked ship data, "+ð.your_title+".");ũ=ĭ();if(ũ!=null)
{var Ĭ=ũ.CalculateShipMass();Ŭ=Ĭ.PhysicalMass;Ū=Ŭ;}else{ð.ȁ.û(
"The ship systems analyzer couldn't find some sort of cockpit or remote control.\nPlease check you have one of these, "+ð.your_title+".");}if(ð.Me.CubeGrid.GridSize==0.5)ŧ=false;else ŧ=true;Ħ=ƒ();Ŧ=Ɣ();var Ə="";if(ð.enable_antenna_function
)Ə=ð.Ǵ.ȿ();if(!ð.Ȅ)if(Ť){ð.ȁ.ā("Waiting for orders, "+ð.your_title+".\n");if(!ð.extra_info){ð.ȁ.ā(
"Ready for Waypoints.\n");}if(Ə!="")ð.ȁ.ā(Ə);if(ð.extra_info){if(Ŭ!=0)ð.ȁ.ā("Mass: "+Ŭ);ð.ȁ.ā("Thruster count: "+Ħ.Count);ð.ȁ.ā("Gyro count: "+Ŧ
.Count);ð.ȁ.ā("Main control: "+ũ.CustomName);ð.ȁ.ā("Is large ship: "+ŧ);ð.ȁ.p();}}Ĳ();ð.ȁ.j();Ť=false;}public bool ú(
IMyTerminalBlock ù){return ù.CubeGrid.EntityId==ð.Me.CubeGrid.EntityId;}public static double Ɛ(IMyShipConnector Ƒ){if(Ƒ.CubeGrid.
GridSize==0.5)return Ƒ.CubeGrid.GridSize;return Ƒ.CubeGrid.GridSize*0.5;}private List<IMyThrust>ƒ(){var Ɠ=new List<IMyThrust>();
foreach(var ù in ð.Ǿ)if(ù is IMyThrust&&ù.IsWorking&&ú(ù))Ɠ.Add((IMyThrust)ù);else if(ù is IMyThrust){if(!((IMyThrust)ù).
Enabled&&ú(ù)){Ů+=1;}}return Ɠ;}private List<IMyGyro>Ɣ(){var ƕ=new List<IMyGyro>();foreach(var ù in ð.Ǿ)if(ù is IMyGyro&&ù.
IsWorking&&ú(ù))ƕ.Add((IMyGyro)ù);return ƕ;}public IMyShipConnector Ɩ(){IMyShipConnector š=null;var Ĩ=new List<IMyShipConnector>(
);ð.GridTerminalSystem.GetBlocksOfType(Ĩ);var ş=false;var ĩ=false;foreach(var Ī in Ĩ)if((ũ.CubeGrid.ToString()==Ī.
CubeGrid.ToString()&&!Ī.CustomName.ToLower().Contains("[recall dock]")&&!ð.allow_connector_on_seperate_grid)||(ð.
allow_connector_on_seperate_grid&&Ī.CustomName.ToLower().Contains("[dock]"))){if(Ī.Status==MyShipConnectorStatus.Connected){if(!ş){ş=true;š=Ī;}}else if(
Ī.Status==MyShipConnectorStatus.Connectable){if(ş==false)if(!ĩ){ĩ=true;š=Ī;}}}return š;}public void ī(){var Ĭ=ũ.
CalculateShipMass();Ŭ=Ĭ.PhysicalMass;if(Ū!=Ŭ)Ǝ();Ū=Ŭ;}private IMyShipController ĭ(){var Į=new List<IMyShipController>();IMyShipController
į=null;var İ=false;foreach(var ù in ð.Ǿ)if(ù is IMyShipController&&ú(ù)){if(į==null)į=(IMyShipController)ù;if(ù is
IMyCockpit){var ĺ=(IMyCockpit)ù;if(İ==false)į=(IMyShipController)ù;if(ĺ.IsMainCockpit){İ=true;į=(IMyShipController)ù;}}if(ù is
IMyRemoteControl&&İ==false)į=(IMyShipController)ù;if(ù is IMyRemoteControl&&ű==null)ű=(IMyRemoteControl)ù;}return į;}public void Ĳ(){ŭ=
new Dictionary<Base6Directions.Direction,ĝ>();ť=new ĝ(ð,Base6Directions.Direction.Forward,ũ);ů=new ĝ(ð,Base6Directions.
Direction.Up,ũ);Ũ=new ĝ(ð,Base6Directions.Direction.Left,ũ);ų=new ĝ(ð,Base6Directions.Direction.Backward,ũ);ţ=new ĝ(ð,
Base6Directions.Direction.Down,ũ);ū=new ĝ(ð,Base6Directions.Direction.Right,ũ);ŭ.Add(Base6Directions.Direction.Forward,ť);ŭ.Add(
Base6Directions.Direction.Up,ů);ŭ.Add(Base6Directions.Direction.Left,Ũ);ŭ.Add(Base6Directions.Direction.Backward,ų);ŭ.Add(
Base6Directions.Direction.Down,ţ);ŭ.Add(Base6Directions.Direction.Right,ū);Vector3D ĳ;double Ĵ=0;double ĵ=0;double Ķ=0;var ķ=new List<
Base6Directions.Direction>();ķ.Add(Base6Directions.Direction.Forward);ķ.Add(Base6Directions.Direction.Up);ķ.Add(Base6Directions.
Direction.Left);ķ.Add(Base6Directions.Direction.Backward);ķ.Add(Base6Directions.Direction.Down);ķ.Add(Base6Directions.Direction.
Right);foreach(var ĸ in Ħ)if(ĸ.IsWorking&&ú(ĸ)){ĳ=-ĸ.WorldMatrix.Forward;Ĵ=Vector3D.Dot(ĳ,ũ.WorldMatrix.Forward);ĵ=Vector3D.
Dot(ĳ,ũ.WorldMatrix.Up);Ķ=Vector3D.Dot(ĳ,ũ.WorldMatrix.Left);var Ĺ=Base6Directions.Direction.Forward;var Ļ=true;if(Ĵ>=0.97)
{Ĺ=Base6Directions.Direction.Forward;Ļ=false;}else if(Ķ>=0.97){Ĺ=Base6Directions.Direction.Left;Ļ=false;}else if(ĵ>=0.97)
{Ĺ=Base6Directions.Direction.Up;Ļ=false;}else if(Ĵ<=-0.97){Ĺ=Base6Directions.Direction.Backward;Ļ=false;}else if(Ķ<=-0.97
){Ĺ=Base6Directions.Direction.Right;Ļ=false;}else if(ĵ<=-0.97){Ĺ=Base6Directions.Direction.Down;Ļ=false;}if(!Ļ){ŭ[Ĺ].œ(ĸ)
;if(ķ.Contains(Ĺ))ķ.Remove(Ĺ);}}string ı="";if(ķ.Count==6){ı="Sorry "+ð.your_title+
", I couldn't seem to find any thrusters on your ship.";if(Ů>0){ı+="\nI have detected that some thrusters\nare disabled. Could this be the problem?";}ð.ȁ.û(ı);}else if(ķ.Count
==1){ı="Sorry "+ð.your_title+
", it's required that all 6 directions have at least one thruster.\nThe missing direction might be "+ķ[0]+".";if(Ů>0){ı+="\nI have detected that some thrusters\nare disabled. Could this be the problem?";}ð.ȁ.û(ı);}else
if(ķ.Count>1){var ħ="";foreach(var Ĝ in ķ)ħ+=Ĝ+"-";ı="Sorry "+ð.your_title+
", it's required that all 6 directions have at least one thruster.\nIt seems the missing directions might be: "+ħ;if(Ů>0){ı+="\nI have detected that some thrusters\nare disabled. Could this be the problem?";}ð.ȁ.û(ı);}}public void
ę(){foreach(var Ě in ŭ)Ě.Value.ŕ();}private void ā(object Ă){ð.ȁ.ā(Ă);}private void j(bool k=false){ð.ȁ.j(k);}private
void û(string ě){ð.ȁ.û(ě);}}class ĝ{private Program ð;public double[]Ğ;public IMyTerminalBlock ğ;public int Ġ=1;public ĝ[]ġ;
public Vector3D Ģ;public double ģ;public Base6Directions.Direction Ĥ;public double[,]ĥ;public double Ę;public List<IMyThrust>Ħ
;public Vector3D ļ;public ĝ(Program Ė,Base6Directions.Direction ő,IMyTerminalBlock Œ){ð=Ė;ğ=Œ;Ħ=new List<IMyThrust>();Ę=0
;Ĥ=ő;ŕ();Ğ=new double[3];Ģ=new Vector3D();ġ=new ĝ[6];ĥ=new double[3,3];if(Ĥ==Base6Directions.Direction.Down||Ĥ==
Base6Directions.Direction.Right||Ĥ==Base6Directions.Direction.Backward)Ġ=-1;else Ġ=1;}public void œ(IMyThrust Ŕ){Ħ.Add(Ŕ);Ę+=Ŕ.
MaxEffectiveThrust;}public void ŕ(){ļ=ğ.WorldMatrix.GetDirectionVector(Ĥ);}}class Ŗ{private Program ð;public Ŗ(Program Ė){ð=Ė;}public void
ŗ(double Ř,double ř,double Ś,List<IMyGyro>ś,MatrixD Ŝ){var ŝ=new Vector3D(-Ř,ř,Ś);var Ş=Vector3D.TransformNormal(ŝ,Ŝ);var
Ł=false;foreach(var Š in ś)if(Š.IsWorking){var Ő=Š.WorldMatrix;var Ľ=Vector3D.TransformNormal(Ş,Matrix.Transpose(Ő));Š.
Pitch=(float)Ľ.X;Š.Yaw=(float)Ľ.Y;Š.Roll=(float)Ľ.Z;Š.GyroOverride=true;}else if(!Ł){ð.systemsAnalyzer.Ţ=true;ð.ȁ.ā(
"Warning:\nGyro damage detected, recomputing.");Ł=true;}}public void Ņ(ĝ ľ,double Ŀ){var ŀ=Ŀ/ľ.Ę;var Ł=false;foreach(var ĸ in ľ.Ħ)if(ĸ.IsWorking){ĸ.ThrustOverride=(
float)(ĸ.MaxThrust*ŀ);}else if(!Ł){ð.systemsAnalyzer.Ţ=true;ð.ȁ.ā("Warning:\nThruster damage detected, recomputing.");Ł=true;
}}}class ł{public Vector3D Ń;public double ń=5;public double ņ=1;public double Ŏ=5;public double Ň=1;public Vector3D ň;
public double ŉ=0.1;public bool Ŋ=true;public bool ŋ=false;public Vector3D Ō;public ł(Vector3D ō,Vector3D ȱ,Vector3D ŏ){ň=ō;Ń=
ȱ;Ō=ŏ;}}