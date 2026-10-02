// R e a d m e
// -----------
// 
// GMDP -  Mining Prospector Script - V0.607B
// 
    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
        A(Storage);
    }

    int drone_id = 1;
    string drone_tag = "SWRM_D";
    string scout_tag = "PSMD";
    string lcd_display_tag = "D1";

    double safe_position = 30.0;
    double free_center_position = 20000.0;
    double raycast_scan_distance = 32.0;
    //statics        
    string scan_cmd = "scan";
    string reset_cmd = "reset";
    string send_cmd = "send";
    string ast_en_cmd = "asten";
    string ast_dis_cmd = "astdis";
    string retry_send_cmd = "retx";
    string free_form_en_cmd = "freeen";
    string free_form_dis_cmd = "freedis";
    string up_val = "incrval";
    string down_val = "decrval";
    string menuitem_select = "select";
    string iterate_cmd = "iterate";
    string confirm_cmd = "confirm";

    int lcd_display_index = 0; //used for devices with multiple screen panels (0+) 
    string B="V0.607B";string C="",D="",E="",F="",G="",H="",I="TX",J="TGT",K="prospector",L="sync",M="scan",N="scan",O,P
="",Q="",R="",S="",T="",U="",V="",W="",X="",Y="",Z="",a=">:",b=":<",c="",d="Jobinfo",e="GMDCJobData",f="";
IMyBroadcastListener g;List<MyIGCMessage>h=new List<MyIGCMessage>();float i,j,k,l,m,n,o=0.850f,p=0.0f;bool q=false,r=false;bool s=false,t=
false,u=false,v=false,w,x=false,y=false,z=false,ª=false,µ=false,º=false;double À=0.0,Á=0.1;int Â=0,Ã=2,Ä=0,Å=4,Æ=0,Ç=0;
IMyRadioAntenna È;IMySensorBlock É;IMyCameraBlock Ê;IMyRemoteControl Ë;IMyBatteryBlock Ì;IMyLightingBlock Í,Î;Vector3D Ï;Vector3D Ð,Ñ,Ò
,Ó,Ô,Õ,Ö;List<IMyRadioAntenna>Ø=new List<IMyRadioAntenna>();List<IMyRadioAntenna>Ù=new List<IMyRadioAntenna>();List<
IMyBeacon>Ú=new List<IMyBeacon>(),Û=new List<IMyBeacon>();List<IMyBatteryBlock>Ü=new List<IMyBatteryBlock>(),Ý=new List<
IMyBatteryBlock>();List<IMyRemoteControl>Þ=new List<IMyRemoteControl>(),ß=new List<IMyRemoteControl>();List<IMySensorBlock>à=new List<
IMySensorBlock>(),á=new List<IMySensorBlock>();List<IMyCameraBlock>â=new List<IMyCameraBlock>(),ã=new List<IMyCameraBlock>(),ä=new
List<IMyCameraBlock>();List<IMyLightingBlock>å=new List<IMyLightingBlock>(),æ=new List<IMyLightingBlock>(),ç=new List<
IMyLightingBlock>();List<IMyThrust>è=new List<IMyThrust>(),é=new List<IMyThrust>();List<IMyShipConnector>ê=new List<IMyShipConnector>(),
ë=new List<IMyShipConnector>();List<IMyTerminalBlock>ì=new List<IMyTerminalBlock>(),í=new List<IMyTerminalBlock>();
IMyTextSurface î;MyIni ï=new MyIni();MyIni ð=new MyIni(),ñ=new MyIni(),ò=new MyIni();string ó="Jobinfo";StringBuilder ô=new
StringBuilder();StringBuilder õ=new StringBuilder(),ö=new StringBuilder(),ø=new StringBuilder(),ù=new StringBuilder();public void
 Save
(){ï.Clear();if(ï.TryParse(Storage.ToString())){ï.Set("State","Safedistance",safe_position);ï.Set("State",
"freecenterposition",free_center_position);ï.Set("State","scantype",Ã);ï.Set("State","raycast",raycast_scan_distance);}else{ï.Set("State",
"Safedistance",safe_position);ï.Set("State","freecenterposition",free_center_position);ï.Set("State","scantype",Ã);ï.Set("State",
"raycast",raycast_scan_distance);}Storage=ï.ToString();ï.Clear();}public void A(string ú){if(!string.IsNullOrWhiteSpace(ú)&&!
string.IsNullOrEmpty(ú)){û(ú);ô.AppendLine("Configuration loaded from Storage.");}else{ô.AppendLine(
"No Storage data found, configuration loaded from arguments or defaults.");}}public void Ă(){IMyGridTerminalSystem ü=GridTerminalSystem as IMyGridTerminalSystem;C="["+scout_tag+" "+drone_id+"]"
;D=drone_tag+" "+K;E="["+drone_tag+"]"+" "+L;g=IGC.RegisterBroadcastListener(E);F="["+scout_tag+" "+drone_id+" "+I+"]";G=
"["+scout_tag+" "+drone_id+" "+J+"]";H="["+scout_tag+" "+drone_id+" "+lcd_display_tag+"]";Ø.Clear();Ù.Clear();Ú.Clear();Û.
Clear();Ü.Clear();Ý.Clear();Þ.Clear();ß.Clear();à.Clear();á.Clear();â.Clear();ã.Clear();ä.Clear();å.Clear();æ.Clear();ç.Clear
();ì.Clear();í.Clear();è.Clear();é.Clear();ê.Clear();ë.Clear();h.Clear();string ý="";ü.GetBlocksOfType<IMyRadioAntenna>(Ø
,þ=>þ.CubeGrid==Me.CubeGrid);if(Ø.Count>0){for(int ÿ=0;ÿ<Ø.Count;ÿ++){if(!Ø[ÿ].CustomName.Contains(C)){string Ā=Ø[ÿ].
CustomData;ā(Ā,Ø[ÿ]);if(string.IsNullOrEmpty(drone_tag)||string.IsNullOrWhiteSpace(drone_tag)){Echo($"Invalid name for drone_tag {drone_tag.Replace("[","[[").Replace("]","]]")}. Please add drone tag to antenna e.g. '1:PSMD:SWRM_D', '<drone_id>:<prospector_drone_name>:<drone_group_tag>'"
);return;}ý=$"Antenna {(ÿ+1)}";Ø[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";Ù.Add(Ø[ÿ]);}if(Ø[ÿ].CustomName.Contains(C)){string
Ā=Ø[ÿ].CustomData;ā(Ā,Ø[ÿ]);if(drone_tag==""||drone_tag==null){Echo($"Invalid name for drone_tag {drone_tag.Replace("[","[[").Replace("]","]]")} Please add drone tag to antenna e.g. '1:PSMD:SWRM_D', '<drone_id>:<prospector_drone_name>:<drone_group_tag>'"
);return;}ý=$"Antenna {(ÿ+1)}";Ø[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";Ø[ÿ].HudText=$"{C} {K}";Ø[ÿ].ShowShipName=true;Ù.Add
(Ø[ÿ]);}}}Ø.Clear();ü.GetBlocksOfType<IMyBeacon>(Ú,þ=>þ.CubeGrid==Me.CubeGrid);if(Ú.Count>0){for(int ÿ=0;ÿ<Ú.Count;ÿ++){
if(Ú[ÿ].CustomName.Contains(C)){ý=$"Beacon {(ÿ+1)}";Ú[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";Û.Add(Ú[ÿ]);}if(!Ú[ÿ].CustomName
.Contains(C)){ý=$"Beacon {(ÿ+1)}";Ú[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";Ú[ÿ].HudText=$"{C} {K}";Û.Add(Ú[ÿ]);}}}Ú.Clear();
ü.GetBlocksOfType<IMyRemoteControl>(Þ,þ=>þ.CubeGrid==Me.CubeGrid);if(Þ.Count>0){for(int ÿ=0;ÿ<Þ.Count;ÿ++){if(Þ[ÿ].
CustomName.Contains(C)){ý=$"Remote Control {(ÿ+1)}";Þ[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";ß.Add(Þ[ÿ]);break;}if(!Þ[ÿ].CustomName.
Contains(C)){ý=$"Remote Control {(ÿ+1)}";Þ[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";ß.Add(Þ[ÿ]);}}}Þ.Clear();ü.GetBlocksOfType<
IMyCameraBlock>(â,þ=>þ.CubeGrid==Me.CubeGrid);if(â.Count>0){for(int ÿ=0;ÿ<â.Count;ÿ++){if(â[ÿ].CustomName.Contains(M)){ý=
$"Camera {(ÿ+1)}";â[ÿ].CustomName=ý+" "+C+" "+M+" "+"["+D+"]";ã.Add(â[ÿ]);ä.Add(â[ÿ]);break;}}}â.Clear();ü.GetBlocksOfType<
IMyBatteryBlock>(Ü,þ=>þ.CubeGrid==Me.CubeGrid);if(Ü.Count>0){for(int ÿ=0;ÿ<Ü.Count;ÿ++){if(Ü[ÿ].CustomName.Contains(C)){ý=
$"Battery {(ÿ+1)}";Ü[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";Ý.Add(Ü[ÿ]);}if(!Ü[ÿ].CustomName.Contains(C)){ý=$"Battery {(ÿ+1)}";Ü[ÿ].
CustomName=ý+" "+C+" "+"["+D+"]";Ý.Add(Ü[ÿ]);}}}Ü.Clear();ì=new List<IMyTerminalBlock>();í=new List<IMyTerminalBlock>();ü.
GetBlocksOfType<IMyTerminalBlock>(ì,þ=>þ.CubeGrid==Me.CubeGrid);if(ì.Count>0){for(int ÿ=0;ÿ<ì.Count;ÿ++){if(ì[ÿ].CustomName.Contains(
lcd_display_tag)||ì[ÿ].CustomName.Contains(H)){ì[ÿ].CustomName=$"GMDP Interface Display {H}";í.Add(ì[ÿ]);}}}ì.Clear();ü.GetBlocksOfType
<IMyLightingBlock>(å,þ=>þ.CubeGrid==Me.CubeGrid);if(å.Count>0){for(int ÿ=0;ÿ<å.Count;ÿ++){if(å[ÿ].CustomName.Contains(F)
||å[ÿ].CustomName.Contains(I)){ý=$"Interior light {(ÿ+1)}";å[ÿ].CustomName=$"{ý} {F} [{D}]";ç.Add(å[ÿ]);break;}}for(int ÿ=
0;ÿ<å.Count;ÿ++){if(å[ÿ].CustomName.Contains(G)||å[ÿ].CustomName.Contains(J)){ý=$"Interior light {(ÿ+1)}";å[ÿ].CustomName
=$"{ý} {G} [{D}]";æ.Add(å[ÿ]);break;}}}å.Clear();ü.GetBlocksOfType<IMySensorBlock>(à,þ=>þ.CubeGrid==Me.CubeGrid);if(à.
Count>0){for(int ÿ=0;ÿ<à.Count;ÿ++){if(à[ÿ].CustomName.Contains(N)){ý=$"Sensor {(ÿ+1)}";à[ÿ].CustomName=ý+" "+C+" "+"["+D+"]"
+N;á.Add(à[ÿ]);break;}}}à.Clear();ü.GetBlocksOfType<IMyThrust>(è,þ=>þ.CubeGrid==Me.CubeGrid);if(è.Count>0){for(int ÿ=0;ÿ<
è.Count;ÿ++){if(è[ÿ].CustomName.Contains(C)){ý=$"Thruster {(ÿ+1)}";è[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";é.Add(è[ÿ]);}if(
!è[ÿ].CustomName.Contains(C)){ý=$"Thruster {(ÿ+1)}";è[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";é.Add(è[ÿ]);}}}è.Clear();ü.
GetBlocksOfType<IMyShipConnector>(ê,þ=>þ.CubeGrid==Me.CubeGrid);if(ê.Count>0){for(int ÿ=0;ÿ<ê.Count;ÿ++){if(ê[ÿ].CustomName.Contains(C)
){ý=$"Connector {(ÿ+1)}";ê[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";ë.Add(ê[ÿ]);}if(!ê[ÿ].CustomName.Contains(C)){ý=
$"Connector {(ÿ+1)}";ê[ÿ].CustomName=ý+" "+C+" "+"["+D+"]";ë.Add(ê[ÿ]);}}}ê.Clear();if(í.Count>0){î=((IMyTextSurfaceProvider)í[0]).
GetSurface(lcd_display_index);ô.AppendLine($"LCD display: '{H.Replace("[","[[").Replace("]","]]")}' found.");}f=
$"{D.Replace("[","[[").Replace("]","]]")}";µ=true;ô.AppendLine("Setup complete!");}void Ą(){ă();}void ă(){if(È!=null&&Ù[0]!=null){if(g.HasPendingMessage){
MyIGCMessage ą=g.AcceptMessage();h.Add(ą);}if(h.Count>0){r=true;}else{r=false;}if(r){c=h[0].Data.ToString();Ć(c);if(º){ć(È);}if(h.
Count>0&&r){h.RemoveAt(0);}}}}public void Ć(string ú){º=false;if(scout_tag!=ú){scout_tag=ú;º=true;}else{return;}}public void
ć(IMyRadioAntenna Ĉ){ð.Clear();if(ð.TryParse(Ĉ.CustomData.ToString())){ð.Set("droneconfig","drone tag",drone_tag);ð.Set(
"droneconfig","scout tag",scout_tag);ð.Set("droneconfig","drone id num",drone_id);ð.Set("droneconfig","lcd display tag",
lcd_display_tag);}else{ð.Set("droneconfig","drone tag",drone_tag);ð.Set("droneconfig","scout tag",scout_tag);ð.Set("droneconfig",
"drone id num",drone_id);ð.Set("droneconfig","lcd display tag",lcd_display_tag);}Ĉ.CustomData=ð.ToString();ð.Clear();}public void ā(
string ú,IMyRadioAntenna Ĉ){if(!string.IsNullOrEmpty(ú)){if(ð.TryParse(ú)){var ĉ="";ĉ=ð.Get("droneconfig","drone tag").
ToString().Trim();if(!string.IsNullOrEmpty(ĉ)&&!string.IsNullOrWhiteSpace(ĉ)){drone_tag=ĉ;}else{drone_tag="SWRM_D";}ĉ=ð.Get(
"droneconfig","scout tag").ToString();if(!string.IsNullOrEmpty(ĉ)){scout_tag=ĉ;}else{scout_tag="";}ĉ=ð.Get("droneconfig",
"drone id num").ToString().Trim();if(!int.TryParse(ĉ,out drone_id)){drone_id=1;}else{int.TryParse(ĉ,out drone_id);}ĉ=ð.Get(
"droneconfig","lcd display tag").ToString().Trim();if(!string.IsNullOrEmpty(ĉ)&&!string.IsNullOrWhiteSpace(ĉ)){lcd_display_tag=ĉ;}
else{lcd_display_tag="D1";}}else{ć(Ĉ);return;}}else{drone_id=1;drone_tag="SWRM_D";scout_tag=" ";lcd_display_tag="D1";ć(Ĉ);}
Echo($"Drone info: {scout_tag.Replace("[","[[").Replace("]","]]")}:{drone_tag.Replace("[","[[").Replace("]","]]")}");C="["+
scout_tag+" "+drone_id+"]";D=drone_tag+" "+K;E="["+drone_tag+"]"+" "+L;g=IGC.RegisterBroadcastListener(E);F="["+scout_tag+" "+
drone_id+" "+I+"]";G="["+scout_tag+" "+drone_id+" "+J+"]";H="["+scout_tag+" "+drone_id+" "+lcd_display_tag+"]";Me.CustomName=
$"GMDP Programmable Block {C} [{drone_tag}] {K}";Me.CubeGrid.CustomName=$"{C} {K} drone";}public void û(string ú){if(!string.IsNullOrEmpty(ú)&&!string.
IsNullOrWhiteSpace(ú)){if(ï.TryParse(ú)){var ĉ="";ĉ=ï.Get("State","Safedistance").ToString().Trim();if(double.TryParse(ĉ,out safe_position
)){double.TryParse(ĉ,out safe_position);}else{safe_position=30.0;}ĉ=ï.Get("State","freecenterposition").ToString().Trim()
;if(double.TryParse(ĉ,out free_center_position)){double.TryParse(ĉ,out free_center_position);}else{free_center_position=
20000.0;}if(free_center_position==0.0){free_center_position=20000.0;}ĉ=ï.Get("State","scantype").ToString().Trim();if(int.
TryParse(ĉ,out Ã)){int.TryParse(ĉ,out Ã);}else{Ã=2;}ĉ=ï.Get("State","raycast").ToString().Trim();if(double.TryParse(ĉ,out
raycast_scan_distance)){double.TryParse(ĉ,out raycast_scan_distance);}else{raycast_scan_distance=32.0;}ô.AppendLine("Storage Loaded");}}}
public void Ċ(){if(Ù.Count<=0||Ù[0]==null){Echo($"Antenna with tag: '{C.Replace("[","[[").Replace("]","]]")}' not found.");µ=
false;return;}È=Ù[0];if(í.Count<=0||((IMyTextSurfaceProvider)í[0]).GetSurface(lcd_display_index)==null){Echo(
$"LCD display: '{lcd_display_tag.Replace("[","[[").Replace("]","]]")}' not found.");µ=false;}if(ß.Count<=0||ß[0]==null){Echo(
$"Remote control with tag: '{C.Replace("[","[[").Replace("]","]]")}' not found.");µ=false;return;}Ë=ß[0];if(ä.Count<=0||ä[0]==null){Echo(
$"Camera with tag: '{M.Replace("[","[[").Replace("]","]]")}' not found.");µ=false;return;}Ê=ä[0];if(Ê!=null){if(!Ê.EnableRaycast){Ê.EnableRaycast=true;}}if(æ.Count<=0||æ[0]==null){Echo(
$"dock indicator light with tag: '{G.Replace("[","[[").Replace("]","]]")}' not found.");µ=false;return;}Í=æ[0];if(ç.Count<=0||ç[0]==null){Echo(
$"undock indicator light with tag: '{F.Replace("[","[[").Replace("]","]]")}' not found.");µ=false;return;}Î=ç[0];if(á.Count<=0||á[0]==null){Echo(
$"Sensor with tag: '{N.Replace("[","[[").Replace("]","]]")}' not found. Please add tag to scanning sensor");µ=false;return;}É=á[0];if(í.Count<=0||í[0]==null){Echo(
$"LCD display with tag: '{H.Replace("[","[[").Replace("]","]]")}' not found.");return;}if(Ý.Count<=0||Ý[0]==null){Echo($"Batteries with tag: '{C.Replace("[","[[").Replace("]","]]")}' not found.");µ
=false;return;}}public void
 Main
(string ċ,UpdateType Č){Ä++;if(È!=null){if(º){º=false;µ=false;}}if(!µ){Ă();}Ċ();if(!µ){Echo(
$"Setup incomplete. Terminating");return;}Ą();if(î!=null){if(î.ContentType!=ContentType.TEXT_AND_IMAGE){î.ContentType=ContentType.TEXT_AND_IMAGE;î.
FontSize=o;}}i=0;j=0;k=0;l=0;m=0;n=0;p=0.0f;for(int ÿ=0;ÿ<Ý.Count;ÿ++){if(Ý[ÿ]!=null){Ì=Ý[ÿ];i=Ì.CurrentStoredPower;k=Ì.
MaxStoredPower;m=Ì.CurrentOutput;}j=j+i;l=l+k;n=n+m;p=(j/l)*100;}ô.AppendLine($"GMDP {B} Running {P}");if(É.DetectAsteroids==false){É.
DetectAsteroids=true;É.DetectEnemy=false;É.DetectFriendly=false;É.DetectLargeShips=false;É.DetectSmallShips=false;É.DetectSubgrids=
false;É.DetectFloatingObjects=false;É.DetectStations=false;É.DetectPlayers=false;É.DetectNeutral=false;É.DetectOwner=false;}
if(ċ.Contains(confirm_cmd)&&!ª&&Å>=0&&Å<4){ª=true;}if(ċ.Contains(menuitem_select)||ċ.Contains(confirm_cmd)&&ª){Å++;if(Å==2
&&Ã==0){Å=3;}if(Å==2&&Ã==1){Å=3;}if(Å>4){Å=1;}if(Å<1){Å=4;}}if(ċ.Contains(iterate_cmd)){Ç++;if(Ç>4){Ç=0;}}if(Å==0){S="";W=
"";T="";X="";U="";Y="";V="";Z="";}if(Å==1){S=a;W=b;T="";X="";U="";Y="";V="";Z="";}if(Å==2){T=a;X=b;S="";W="";U="";Y="";V=
"";Z="";}if(Å==3){U=a;Y=b;T="";X="";S="";W="";V="";Z="";Ç=1;}if(Å==4){V=a;Y=b;T="";X="";S="";W="";U="";Y="";Ç=1;}if(Ç==0){
Á=0.1;}if(Ç==1){Á=1.0;}if(Ç==2){Á=10.0;}if(Ç==3){Á=100.0;}if(Ç==4){Á=1000.0;}if(ċ.Contains(up_val)&&Å==1){safe_position+=
Á;if(safe_position>2000.0){safe_position=2000.0;}}if(ċ.Contains(down_val)&&Å==1){safe_position-=Á;if(safe_position<0.0){
safe_position=0.0;}}if(ċ.Contains(up_val)&&Å==2){free_center_position+=Á;if(free_center_position<0.0){free_center_position=0.0;}}if(ċ
.Contains(down_val)&&Å==2){free_center_position-=Á;if(free_center_position<0.0){free_center_position=0.0;}}if(ċ.Contains(
up_val)&&Å==3){Ã++;if(Ã>2){Ã=0;}}if(ċ.Contains(down_val)&&Å==3){Ã--;if(Ã<0){Ã=2;}}if(ċ.Contains(up_val)&&Å==4){Æ++;if(Æ>3){Æ=0
;}}if(ċ.Contains(down_val)&&Å==4){Æ--;if(Æ<0){Æ=3;}}if(Ã==0){Q="Planetary";w=false;v=false;}if(Ã==1){Q="Asteroid";w=true;
v=false;}if(Ã==2){Q="Free Align";w=false;v=true;}if(Æ==0){R="Scan";}if(Æ==1){R="Send";}if(Æ==2){R="Reset";}if(Æ==3){R=
"Cancel";}if(ċ.Contains(confirm_cmd)&&Å==4&&ª){ċ="";ª=false;}if(ċ.Contains(confirm_cmd)&&Å==4&&Æ==0){if(!x){x=true;}Å=0;if(ª){ª=
false;}}if(ċ.Contains(confirm_cmd)&&Å==4&&Æ==1){if(!y){y=true;}Å=0;if(ª){ª=false;}}if(ċ.Contains(confirm_cmd)&&Å==4&&Æ==2){if
(!z){z=true;}Å=0;if(ª){ª=false;}}if(ċ.Contains(confirm_cmd)&&Å==4&&Æ==3){Å=0;z=false;y=false;x=false;if(ª){ª=false;}}if(ċ
.Contains(reset_cmd)||z){q=false;ô.AppendLine("Scan reset.");u=false;ô.AppendLine("Transmission reset.");Í.Enabled=false;
Î.Enabled=false;t=false;s=false;z=false;}if(ċ.Contains(scan_cmd)||x){x=false;if(q==false){MyDetectedEntityInfo č=Ê.
Raycast(raycast_scan_distance);if(č.IsEmpty()){ô.AppendLine($"Mining surface not found within: '{raycast_scan_distance}'m.");Ï.
X=Math.Round(Ë.GetPosition().X,2);Ï.Z=Math.Round(Ë.GetPosition().Y,2);Ï.Y=Math.Round(Ë.GetPosition().Z,2);s=false;}if(!č.
IsEmpty()){À=(č.HitPosition.Value-Ê.GetPosition()).Length();ô.AppendLine($"Surface found'{À}'m.");Ï.X=Math.Round(č.HitPosition.
Value.X,2);Ï.Y=Math.Round(č.HitPosition.Value.Y,2);Ï.Z=Math.Round(č.HitPosition.Value.Z,2);s=true;}if(s==true&&q==false){if(É
.IsActive==true&&w==true){t=true;ô.AppendLine("Asteroid detected");}if(É.IsActive==false&&w==true||w==false){t=false;}if(
!t&&!v){Õ=Ë.GetNaturalGravity();Ö=Vector3D.Normalize(new Vector3D(-Õ));Vector3D Ď=Ö*-free_center_position;Ñ.Y=Math.Round(
Ï.Y+Ď.Y,2);Ñ.X=Math.Round(Ï.X+Ď.X,2);Ñ.Z=Math.Round(Ï.Z+Ď.Z,2);ô.AppendLine("align to gravity");}if(t&&!v){Ò=É.
LastDetectedEntity.BoundingBox.Center;Ö=Vector3D.Normalize(new Vector3D(-(Ò-Ï)));ô.AppendLine("align to asteroid");}if(v){Ó=Ê.GetPosition(
);Ö=Vector3D.Normalize(new Vector3D(-(Ï-Ó)));ô.AppendLine("align to scan vector");}if(v){Vector3D ď=Ö*-
free_center_position;Ô.Y=Math.Round(Ï.Y+ď.Y,2);Ô.X=Math.Round(Ï.X+ď.X,2);Ô.Z=Math.Round(Ï.Z+ď.Z,2);}Vector3D Đ=Ö*safe_position;Ð.Y=Math.
Round(Ï.Y+Đ.Y,2);Ð.X=Math.Round(Ï.X+Đ.X,2);Ð.Z=Math.Round(Ï.Z+Đ.Z,2);ô.AppendLine("Navigation point calculated");ô.AppendLine
("Coordinates ready.");q=true;Í.Enabled=true;Î.Enabled=false;}}if(q==false){ô.AppendLine(
"Please initiate scan before sending coordinates to controller");u=false;Í.Enabled=false;Î.Enabled=false;}}if(ċ.Contains(ast_dis_cmd)){if(w==true){w=false;}}if(ċ.Contains(ast_en_cmd))
{if(w==false){w=true;}}if(ċ.Contains(free_form_en_cmd)){if(v==false){v=true;}}if(ċ.Contains(free_form_dis_cmd)){if(v==
true){v=false;}}if(ċ.Contains(retry_send_cmd)){if(u==true){u=false;}}if(ċ.Contains(send_cmd)&&q==true&&u==false||y&&q==true
&&u==false){y=false;ö.Clear();ø.Clear();ù.Clear();ö.Append("GPS");ö.Append(":");ö.Append("TGT");ö.Append(":");ö.Append(Ð.X
);ö.Append(":");ö.Append(Ð.Y);ö.Append(":");ö.Append(Ð.Z);ö.Append(":");ö.Append("#FF75C9F1");ö.Append(":");ö.Append(
safe_position);ö.Append(":");ø.Append("GPS");ø.Append(":");ø.Append("GRV");ø.Append(":");ø.Append(Math.Round(Ñ.X,2));ø.Append(":");ø.
Append(Math.Round(Ñ.Y,2));ø.Append(":");ø.Append(Math.Round(Ñ.Z,2));ø.Append(":");ø.Append("#FF75C9F1");ø.Append(":");đ(Ë,ø.
ToString());if(t==true){ö.Clear();ù.Clear();ö.Append("GPS");ö.Append(":");ö.Append("AST");ö.Append(":");ö.Append(Math.Round(Ð.X,
2));ö.Append(":");ö.Append(Math.Round(Ð.Y,2));ö.Append(":");ö.Append(Math.Round(Ð.Z,2));ö.Append(":");ö.Append("#FF1551")
;ö.Append(":");ö.Append(safe_position);ö.Append(":");ù.Append("GPS");ù.Append(":");ù.Append("AST");ù.Append(":");ù.Append
(Math.Round(Ò.X,2));ù.Append(":");ù.Append(Math.Round(Ò.Y,2));ù.Append(":");ù.Append(Math.Round(Ò.Z,2));ù.Append(":");ù.
Append("#FF1551");ù.Append(":");đ(Ë,ù.ToString());}if(v==true){ö.Clear();ù.Clear();ö.Append("GPS");ö.Append(":");ö.Append(
"FRE");ö.Append(":");ö.Append(Math.Round(Ð.X,2));ö.Append(":");ö.Append(Math.Round(Ð.Y,2));ö.Append(":");ö.Append(Math.Round(
Ð.Z,2));ö.Append(":");ö.Append("#FF1551");ö.Append(":");ö.Append(safe_position);ö.Append(":");ù.Append("GPS");ù.Append(
":");ù.Append("FRE");ù.Append(":");ù.Append(Math.Round(Ô.X,2));ù.Append(":");ù.Append(Math.Round(Ô.Y,2));ù.Append(":");ù.
Append(Math.Round(Ô.Z,2));ù.Append(":");ù.Append("#FF1551");ù.Append(":");đ(Ë,ù.ToString());}Ē(Me,ö.ToString());ò.Clear();if(v
||t){O=ö+ù.ToString();;}else{O=ö+ø.ToString();}ö.Clear();ø.Clear();ù.Clear();IGC.SendBroadcastMessage(D,O,
TransmissionDistance.TransmissionDistanceMax);u=true;Î.Enabled=true;ô.AppendLine("Data sent.");}ô.Append("Channel: ").Append(f).Append('\n')
;ô.Append("Target: ").Append(s).Append('\n');ô.Append("TX: ").Append(Ð.X).Append(" TY: ").Append(Ð.Y).Append(" TZ: ").
Append(Ð.Z).Append(" SafeD: ").Append(safe_position).Append("m\n");ô.Append("Free scan: ").Append(v).Append('\n');ô.Append(
"Asteroid detection: ").Append(w).Append('\n');ô.Append("Asteroid: ").Append(t).Append('\n');ô.Append("--------\n");ô.Append("PB Arguments:\n"
);ô.Append("========\n");ô.Append("Increase selection menu = ").Append(up_val).Append('\n');ô.Append(
"Decrease selection menu = ").Append(down_val).Append('\n');ô.Append("Menu item = ").Append(menuitem_select).Append('\n');ô.Append(
"Iteration value = ").Append(iterate_cmd).Append('\n');ô.Append("Confirm = ").Append(confirm_cmd).Append('\n');ô.Append("--------\n");ô.
Append("Direct commands:\n");ô.Append("--------\n");ô.Append("Scan = ").Append(scan_cmd).Append('\n');ô.Append("Reset = ").
Append(reset_cmd).Append('\n');ô.Append("Send = ").Append(send_cmd).Append('\n');ô.Append("Asteroid EN = ").Append(ast_en_cmd)
.Append('\n');ô.Append("Asteroid DIS = ").Append(ast_dis_cmd).Append('\n');ô.Append("Reset send = ").Append(
retry_send_cmd).Append('\n');ô.Append("Free form EN = ").Append(free_form_en_cmd).Append('\n');ô.Append("Free form DIS = ").Append(
free_form_dis_cmd).Append('\n');if(t){ô.Append("AX: ").Append(Math.Round(Ò.X,2)).Append(" AY: ").Append(Math.Round(Ò.Y,2)).Append(" AZ: "
).Append(Math.Round(Ò.Z,2)).Append('\n');}if(v&&q){ô.Append("FX: ").Append(Math.Round(Ô.X,2)).Append(" FY: ").Append(Math
.Round(Ô.Y,2)).Append(" FZ: ").Append(Math.Round(Ô.Z,2)).Append('\n');}õ.Clear();õ.Append('\n');õ.Append("GMDP ").Append(
B).Append(" ").Append(scout_tag).Append(" Running ").Append(P).Append(" (").Append(Math.Round((double)p,1)).AppendLine(
")%");õ.Append("Channel: ").AppendLine(D);õ.Append("\nTarget: ").Append(s).Append("\n");õ.Append("TX: ").Append(Ð.X).Append(
" TY: ").Append(Ð.Y).Append(" TZ: ").Append(Ð.Z).Append("\n");õ.Append("\nAdjust value: ").Append(Á).Append("\n");õ.Append(S).
Append(" Surface distance: ").Append(safe_position).Append("m ").AppendLine(W);if(v){õ.Append(T).Append("Align Depth: ").
Append(free_center_position).Append("m ").AppendLine(X);}õ.Append("\n").Append(U).Append(" Scan type: ").Append(Q).Append(" ")
.AppendLine(Y);õ.Append(V).Append(" Command: ").Append(R).Append(" ").AppendLine(Z);õ.Append("\n\n");if(!w){õ.Append(
"Asteroid detection: ").Append(w).Append("\n");}if(v){õ.Append("Free scan: ").Append(v).Append("\n");}if(w){õ.Append("Asteroid: ").Append(t).
Append(" Sensor: ").Append(É.IsActive).Append("\n");}õ.Append("Scan: ").Append(q).Append("\n");õ.Append("Transmit: ").Append(u
).Append("\n");if(t){õ.Append("AX: ").Append(Math.Round(Ò.X,2)).Append(" AY: ").Append(Math.Round(Ò.Y,2)).Append(" AZ: ")
.Append(Math.Round(Ò.Z,2)).Append("\n");}if(v&&q){õ.Append("FX: ").Append(Math.Round(Ô.X,2)).Append(" FY: ").Append(Math.
Round(Ô.Y,2)).Append(" FZ: ").Append(Math.Round(Ô.Z,2)).Append("\n");}if(î!=null){î.WriteText(õ);}if(Ä%10==0){Echo(ô.ToString
());}if(Ä>60){Ä=0;}õ.Clear();ô.Clear();ē();}void Ē(IMyTerminalBlock Ĉ,string ú){ñ.Clear();if(ñ.TryParse(Ĉ.CustomData.
ToString())){ñ.Set(e,d,ú);}else{ñ.Set(e,d,ú);}Ĉ.CustomData=ñ.ToString();ñ.Clear();}void đ(IMyTerminalBlock Ĉ,string ú){ñ.Clear()
;if(ñ.TryParse(Ĉ.CustomData.ToString())){ñ.Set(e,ó,ú);}else{ñ.Set(e,ó,ú);}Ĉ.CustomData=ñ.ToString();ñ.Clear();}void ĕ(int
Ĕ){if(Ĕ==0){P=".---";}if(Ĕ==1){P="-.--";}if(Ĕ==2){P="--.-";}if(Ĕ==3){P="---.";}}void ē(){Â++;if(Â>3){Â=0;}ĕ(Â);}
