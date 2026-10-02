// R e a d m e
// -----------
// 
// GMDC Drone controller V0.637B
// 
// PB Run Argument Structure
// [
//   "DroneGroup" //Name of the drone control group *NB: MUST BE FIRST PARAMETER IN PB RUN ARGUMENT*
//   Command argument - "shipname", // Name of the ship to control
//   Command argument - "dronelength", // Length of the drone in meters
//   Command argument - "clearoffset", // Clear offset distance in meters
//   Command argument - "rotatehome", // Rotate home position for visualiser (true/false)
//   Command argument - "dronesperscreen" // Number of drones per screen
//   Command argument - "undockdelay" //delay in ticks (default = 12 - appx 1.16s)
// 
//   eg.   PB Run Argument: SWRM_D,shipname=MyShip,dronelength=2.6,clearoffset=12.0,rotatehome=true,dronesperscreen=8,undockdelay=24
//   NB1: "DroneGroup" MUST BE FIRST PARAMETER IN PB RUN ARGUMENT
//   NB2: The drone controller script will only work with the GMDC Drone Control Group. If you have renamed the group, please rename it back to "DroneGroup" or change the first parameter in the PB Run Argument to match your group name.
//   NB3: Arguments must be separated by commas and not spaces and command argument value must be in the format of "argument=value" with no spaces between the argument, equals sign and value.
//   NB4: If you have gates/doors/hangar doors in your ship, you can now choose to have GMDC automate your docking bay doors for your drones by assigning the drone name to the name of the gate e.g. [SWRM_D 1]. Recompile or run argument will refresh gate assignments.
// ]
// 
// 
    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
        A(Storage, Me.CustomData);
        B = true;
    }
    //default information
    string drone_tag = "SWRM_D"; //Mining drone group tag
    double drone_length = 2.6;
    double drone_clear_offset = 12.0; //drill clear mode distance offset
    string secondary = ""; //vessel/rig name (optional)

    //display surface indexes
    int srfM = 0;
    int srfL = 0;
    int srfD = 0;
    int srfV = 0;
    int drones_per_screen = 8;
    int droneUndockDelayTime = 12;
    int undock_delay_limit = 24;
    //Drone Comms
    int droneCommunicationsProcessingDelay = 0;
    int droneCommunicationsPingDelay = 30;

    int C=500;int D=250,E=44,F=1,G=10,H=0,I,J,K,L,M,N=0,O=0,P=1,Q=0,R=0,S=0,T=0,U=0,V=0,W=0,X=0,Y=0,Z=0,a=0,b=0,c=0,d,e=
0,f=-1,g=0,h=0,i=0,j=0,k=0,l=0,m=0,n=0,o=0,p=0,q=-1,r=0,s=0,t=0,u=0;int v=10;string w="V0.637B",x="Comms",y="Main",z=
"Drone",ª="Interface",µ="List",º="Display",À="Visual",Á,Â,Ã,Ä,Å,Æ,Ç,È,É,Ê,Ë,Ì,Í,Î,Ï,Ð,Ñ,Ò,Ó,Ô,Õ,Ö,Ø,Ù,Ú,Û,Ü,Ý,Þ="",ß="",à="",á=
"",â="",ã="",ä="ping",å="",æ="",ç="",è="",é="",ê="",ë="",ì="",í,î,ï,ð,ñ,ò,ó,ô,õ,ö,ø,ù,ú,û,ü,ý,þ="",ÿ="",Ā="",ā="",Ă="",ă=
"",Ą="",ą="",Ć="",ć="",Ĉ="",ĉ="",Ċ="",ċ="",Č="",č="Idle",Ď="reply",ď="prospector",Đ="sync",đ="recall",Ē="operate",ē,Ĕ,ĕ=""
,Ė="",ė="",Ę="",ę="",Ě="",ě="",Ĝ="Jobinfo",ĝ="GMDCJobData",Ğ="Default",ğ="",Ġ="",ġ="",Ģ="",ģ="",Ĥ="",ĥ="",Ħ="",ħ="",Ĩ="",
ĩ="",Ī="dronelength",ī="clearoffset",Ĭ="rotatehome",ĭ="dronesperscreen",Į="undockdelay";bool į=false,İ=false,ı=false,Ĳ=
false,ĳ=false,Ĵ=false,ĵ=false,Ķ=false,ķ=false,ĸ=false,Ĺ=false,ĺ=false,Ļ=false,ļ=false,Ľ=false,ľ=false,Ŀ=false,ŀ=false,Ł=false
,ł=false,Ń=false,ń=false,Ņ=false,ņ=true,Ň=false,ň=false,ŉ=false,Ŋ=false,ŋ=false,Ō=false,ō=false,Ŏ=false,ŏ=false,Ő=false,ő
=false,Œ=false,œ=false,Ŕ=false,ŕ=false,Ŗ=false,ŗ=false,Ř=false,ř=false,Ś=false,B=false,ś=false,Ŝ=false,ŝ=true;double Ş=
30.0,ş,Š=1.0,š=0.0,Ţ,ţ=0.0,Ť=0.0,ť=0.0,Ŧ=0.0,ŧ=0.0,Ũ=0.0,ũ=0.0,Ū=0.0,ū=16.666;int Ŭ=0;bool ŭ;double Ů=0.0,ů=0.0;string Ű;
string ű;string Ų;string ų="";string Ŵ="";string ŵ="";string Ŷ="";IEnumerator<bool>ŷ,Ÿ,Ź;IMyRadioAntenna ź;IMyLightingBlock Ż;
IMyRemoteControl ż;IMyProgrammableBlock Ž;Vector3D ž,ſ,ƀ,Ɓ;Vector3D Ƃ;StringBuilder ƃ=new StringBuilder(),Ƅ=new StringBuilder(),ƅ=new
StringBuilder(),Ɔ=new StringBuilder(),Ƈ=new StringBuilder(),ƈ=new StringBuilder(),Ɖ=new StringBuilder(),Ɗ=new StringBuilder(),Ƌ=new
StringBuilder(),ƌ=new StringBuilder(),ƍ=new StringBuilder();List<Vector3D>Ǝ=new List<Vector3D>();List<bool>Ə=new List<bool>(),Ɛ=new
List<bool>(),Ƒ=new List<bool>();List<string>ƒ=new List<string>(),Ɠ=new List<string>(),Ɣ=new List<string>();int ƕ;Color Ɩ=new
Color(0,255,0),Ɨ=new Color(255,255,0),Ƙ=new Color(255,0,0),ƙ=new Color(0,0,255),ƚ=new Color(235,90,33);List<IMyRemoteControl>
ƛ=new List<IMyRemoteControl>();List<IMyRemoteControl>Ɯ=new List<IMyRemoteControl>();List<IMyRadioAntenna>Ɲ=new List<
IMyRadioAntenna>(),ƞ=new List<IMyRadioAntenna>();List<IMyLightingBlock>Ɵ=new List<IMyLightingBlock>(),Ơ=new List<IMyLightingBlock>();
List<IMyTerminalBlock>ơ=new List<IMyTerminalBlock>(),Ƣ=new List<IMyTerminalBlock>(),ƣ=new List<IMyTerminalBlock>(),Ƥ=new
List<IMyTerminalBlock>(),ƥ=new List<IMyTerminalBlock>();List<IMyProgrammableBlock>Ʀ=new List<IMyProgrammableBlock>(),Ƨ=new
List<IMyProgrammableBlock>();List<IMyDoor>ƨ=new List<IMyDoor>();IMyTextSurface Ʃ,ƪ,ƫ,Ƭ;IMyCubeGrid ƭ;RectangleF Ʈ;string Ư;
int ư=0;bool Ʊ;string Ʋ="";IMyBroadcastListener Ƴ;IMyBroadcastListener ƴ;List<MyIGCMessage>Ƶ=new List<MyIGCMessage>(),ƶ=new
List<MyIGCMessage>();List<MySprite>Ʒ=new List<MySprite>();MyIni Ƹ=new MyIni();MyIni ƹ=new MyIni(),ƺ=new MyIni(),ƻ=new MyIni(
),Ƽ=new MyIni(),ƽ=new MyIni();string ƾ="";string ƿ="Jobinfo";string ǀ="";Dictionary<string,ǁ>ǂ=new Dictionary<string,ǁ>()
;float ǃ;List<IMyMotorStator>Ǆ=new List<IMyMotorStator>();List<IMyMotorAdvancedStator>ǅ=new List<IMyMotorAdvancedStator>(
);List<IMyPistonBase>ǆ=new List<IMyPistonBase>();List<IMyTextSurface>Ǉ=new List<IMyTextSurface>(),ǈ=new List<
IMyTextSurface>(),ǉ=new List<IMyTextSurface>(),Ǌ=new List<IMyTextSurface>();string ǋ="shipname";public void
 Save
(){if(Ł){Ɖ=new StringBuilder();Ƹ.Clear();Ƹ.Set("configuration","runargument",Ě);Ƹ.Set("configuration","ship grid tag",Ʋ);
if(Ɛ.Count>0&&Ə.Count>0){for(int ǌ=0;ǌ<Ɛ.Count;ǌ++){if(Ɛ[ǌ]){Ư="1";}else{Ư="0";}if(Ə[ǌ]){Ĕ="1";}else{Ĕ="0";}ĕ=Ǝ[ǌ].X.
ToString();Ė=Ǝ[ǌ].Y.ToString();ė=Ǝ[ǌ].Z.ToString();Ɖ.Append($"{Ư}:{Ĕ}:{ĕ}:{Ė}:{ė}:;");}Ƹ.Set("jobdata","gridstatus",Ɖ.ToString()
);Storage=Ƹ.ToString();Ƽ.Clear();if(Ƽ.TryParse(Me.CustomData)){Ƽ.Set("configuration","runargument",Ě);Ƽ.Set(
"configuration","ship grid tag",Ʋ);Ƽ.Set("jobdata","gridstatus",Ɖ.ToString());Me.CustomData=Ƽ.ToString();}Ƽ.Clear();Ɖ.Clear();Ƹ.Clear()
;}}}public void ǎ(IMyRadioAntenna Ǎ){ƹ.Clear();if(ƹ.TryParse(Ǎ.CustomData.ToString())){ƹ.Set("Configuration",
"drone group tag",drone_tag);ƹ.Set("Configuration","ship grid tag",Ʋ);}else{ƹ.Set("Configuration","drone group tag",drone_tag);ƹ.Set(
"Configuration","ship grid tag",Ʋ);}Ǎ.CustomData=ƹ.ToString();ƹ.Clear();}public void A(string Ǐ,string ǐ){if(!string.IsNullOrWhiteSpace
(Storage)&&!string.IsNullOrEmpty(Storage)){Ǒ(Storage);Echo("Running first parse");ǒ(Ě);Echo(
"Configuration loaded from Storage.");}else{Ǒ(Storage);ǒ(Ě);Echo("No Storage data found, configuration loaded from arguments or defaults.");}}public void
 Main
(string Ǔ,UpdateType ǔ){int Ǖ=Runtime.CurrentInstructionCount;t++;if(!string.IsNullOrEmpty(Ǔ)&&!string.IsNullOrWhiteSpace
(Ǔ)&&!B){Ě=Ǔ;ǒ(Ǔ);Save();ň=true;Ł=false;}ǖ(ǔ);Ǘ();B=false;if(!Ł){Echo("Setup incomplete - exiting");ǘ();return;}Ǚ(Ǔ);ǚ();
Ǜ();ǜ();ǝ();Ǟ();if(t%10==0){Echo($"Runtime: {Math.Round(ů,3)}ms");Echo($"Average Runtime: {Math.Round(ů/s,3)}ms");Echo(ƌ.
ToString());}ƌ.Clear();ƌ.AppendLine($"Main Total: {Runtime.CurrentInstructionCount-Ǖ}");}void ǖ(UpdateType ǔ){int Ǖ=Runtime.
CurrentInstructionCount;double ǟ=Runtime.LastRunTimeMs;ů+=ǟ;s++;if(s==10){s=0;ů=0;}}void Ǘ(){int Ǖ=Runtime.CurrentInstructionCount;if(!Ł){Ǡ();Ł
=true;Echo("Setup complete!");}ǡ();if(!Ł){Echo("Setup incomplete - exiting");ǘ();return;}ǎ(ź);ƌ.AppendLine(
$"GMDC {w} Running {Ę} ");ƌ.AppendLine($"Channel: {Ĩ} ");ƌ.AppendLine($"Ship Name: {ĩ} ");ƌ.AppendLine($"D1 Tag ({Ǉ.Count}): {ģ} ");ƌ.AppendLine
($"D2 Tag ({ǈ.Count}: {Ĥ} ");ƌ.AppendLine($"D2 DrnPS: (#{drones_per_screen}) ");ƌ.AppendLine($"D3 Tag ({ǉ.Count}): {ĥ} ")
;ƌ.AppendLine($"D4 Tag ({Ǌ.Count}): {Ħ} ");ƌ.AppendLine($"D4 Vis rotation: {ŝ} ");ƌ.AppendLine(
$"Clear offset: {drone_clear_offset}m ");ƌ.AppendLine($"Drone length: {drone_length}m ");ƌ.AppendLine($"Undock delay: ({Math.Round((((double)droneUndockDelayTime*ū)/1000.0)*(double)v,1)}s) / ({Math.Round((((double)undock_delay_limit*ū)/1000.0)*(double)v,1)}s) "
);}void Ǚ(string Ǔ){string Ǣ="";int Ǖ=Runtime.CurrentInstructionCount;ǣ();Ǥ(Ǣ);}void ǚ(){int Ǖ=Runtime.
CurrentInstructionCount;ǥ();}void Ǜ(){int Ǖ=Runtime.CurrentInstructionCount;Ǧ();ǧ();Ǩ();ǩ();if(ğ!=ż.CustomData){Ǫ(ż.CustomData,ż);ğ=ż.
CustomData;}if(ľ){}if(Ŗ){Storage=null;ľ=false;Ŀ=false;Ǫ(ż.CustomData,ż);if(ľ){ƌ.AppendLine(
$"Post-Prospect: Main PB: {ı} Align coords=:{ƀ}");}if(Ľ){ƌ.AppendLine($"Formatting CustomData with: {Ƃ.X}, {Ƃ.Y}, {Ƃ.Z}");ƃ.Clear().AppendFormat(
"GPS:PDT:{0:0.##}:{1:0.##}:{2:0.##}:#FF75C9F1:5.0:10.0:1:1:0:False:1:10:0:False:",Ƃ.X,Ƃ.Y,Ƃ.Z);if(ľ){ƃ.Append($"GPS:TGT:{ƀ.X}:{ƀ.Y}:{ƀ.Z}:#F77668:{ţ}:");}ǫ(ƃ.ToString(),Me,ĝ,Ĝ);}Ŗ=false;Ĳ=false;}if(ę!=
Me.CustomData){Ǭ(Me.CustomData,Me);ę=Me.CustomData;}ǭ();Ǯ();}void Ǩ(){if(ś){Ĳ=false;ň=true;}}void ǜ(){int Ǖ=Runtime.
CurrentInstructionCount;if(ǂ.Count>0&&Ĳ&&ł){ǯ();ǰ();Ǳ();ǲ();ǳ();Ǵ();}ǵ();Ƕ();}void ǝ(){int Ǖ=Runtime.CurrentInstructionCount;Ƿ(true);Ǹ();ǹ();}
void Ǟ(){int Ǖ=Runtime.CurrentInstructionCount;Ǻ();if(t%10==0){ǻ(Runtime.LastRunTimeMs);}if(t>10){t=0;}}void ǻ(double ǟ){
double Ǽ=(ǟ/ū)*100.0;ƌ.Append("Load: ").Append(Math.Round(Ǽ,3)).Append("% (").Append(Math.Round(ǟ,3)).Append("ms) S#:").Append
(r).Append(" ").Append(Ś).Append('\n');ƌ.Append("Drones #: ").Append(ǂ.Count).Append('\n');ƌ.Append(
"Drone comms buffer: ").Append(Ƶ.Count).Append(" OK: ").Append(Ř).Append('\n');double ǽ=(((double)droneCommunicationsProcessingDelay*ū)/1000.0
)*(double)v;ƌ.Append("Cycles since last broadcast: ").Append(g).Append(" (").Append(Math.Round(ǽ,1)).Append("s) ").Append
(ł).Append('\n');double Ǿ=(((double)droneCommunicationsPingDelay*ū)/1000.0)*(double)v;ƌ.Append("Cycles since last ping: "
).Append(h).Append(" (").Append(Math.Round(Ǿ,1)).Append("s)").Append('\n');double ǿ=(((double)n*ū)/1000.0)*(double)v;
double Ȁ=(((double)undock_delay_limit*ū)/1000.0)*(double)v;ƌ.Append("Undock cycle timer: ").Append(n).Append(" (").Append(Math
.Round(ǿ,1)).Append("s) (").Append(Math.Round(Ȁ,1)).Append("s)").Append('\n');ƌ.Append("Drones Undocking: ").Append(Ő).
Append(" ").Append(ư).Append('\n');ƌ.Append("Prospect comms buffer: ").Append(ƶ.Count).Append('\n');ȁ();}void Ǻ(){g++;if(g>=
droneCommunicationsProcessingDelay){ł=true;}h++;if(h>=droneCommunicationsPingDelay&&Ƶ.Count<=0){ĳ=false;}if(Ő){n++;}if(n>droneUndockDelayTime){Ő=false;}}
void ǹ(){if(ř){if(Ź==null&&!ŕ){Ź=Ȃ(ž,Ɓ,ş,J,I,į,ŝ);}if(Ź!=null&&!ŕ){bool ȃ=Ź.Current;if(!Ź.MoveNext()){ƌ.AppendLine(
"Job rendering complete.");Ź?.Dispose();Ź=null;Ȃ(ž,Ɓ,ş,J,I,į,ŝ).Dispose();}else{if(!ȃ){ƌ.AppendLine(
$"Rendering mining job ... {Math.Round(Ũ,1)}%  {Math.Round(ũ,1)}%");Ź.MoveNext();}}}if(ƥ.Count>0&&ƥ[0]!=null&&Ɛ.Count>0&&ŕ){ŕ=false;if(r>=C){Ƭ.DrawFrame();var Ȅ=new MySprite();Ƭ.
DrawFrame().Add(Ȅ);if(Ʒ.Count==0){Ƭ.DrawFrame().Dispose();}}else{var ȅ=Ƭ.DrawFrame();Ȇ(ref ȅ);ȅ.Dispose();Ʒ.Clear();}if(r>C+1){r=
0;Ś=false;}}}}void Ǹ(){if(Ÿ==null&&!œ){Ÿ=ȇ();}if(Ÿ!=null&&!œ){bool ȃ=Ÿ.Current;if(!Ÿ.MoveNext()){ƌ.AppendLine(
"Mining list complete.");Ÿ?.Dispose();Ÿ=null;}else{if(!ȃ){ƌ.AppendLine($"Updating mining job list... {Math.Round(ŧ,1)}%");Ÿ.MoveNext();}}}if(œ)
{if(ǉ.Count>0){for(int ǌ=0;ǌ<ǉ.Count;ǌ++){if(ǉ[ǌ]!=null){ǉ[ǌ].WriteText(Ƈ);}}}œ=false;Ƈ.Clear();Ŕ=false;}}void Ƕ(){if(Ż==
null||Ơ[0]==null){ƌ.AppendLine($"Indicator light missing {ç.Replace("[","[[").Replace("]","]]")} - early exit");return;}if(ķ
&&!Ń||í=="Init"){Ż.SetValue("Color",Ƙ);if(í=="Init"){Ż.SetValue("Color",Ɩ);}Ż.Enabled=true;Ż.BlinkIntervalSeconds=0.7f;Ż.
BlinkLength=20.0f;Ż.Enabled=true;č="Not Ready";}if(ķ&&Ń){Ż.SetValue("Color",Ɩ);Ż.BlinkIntervalSeconds=0;Ż.BlinkLength=10.0f;Ż.
Enabled=true;č="Ready";}if(!ķ&&í=="Stop"||í==""||í=="Freeze"||í=="Eject"||í=="Recall"){Ż.SetValue("Color",Ƙ);if(í=="Eject"){Ż.
SetValue("Color",Ɨ);}if(í=="Recall"){Ż.SetValue("Color",ƙ);}Ż.BlinkIntervalSeconds=0.7f;Ż.BlinkLength=20.0f;Ż.Enabled=true;č=
"Not Ready";}if(Q>0&&ƕ<P&&ķ&&ĵ||ŀ){Ż.BlinkIntervalSeconds=0;if(l>0){Ż.BlinkIntervalSeconds=0.7f;}Ż.SetValue("Color",Ɨ);if((k>0)||(m
>0)){Ż.BlinkIntervalSeconds=0.7f;Ż.SetValue("Color",ƚ);}if(ŀ){Ż.BlinkIntervalSeconds=0;Ż.SetValue("Color",ƚ);}Ż.Enabled=
true;č="Working";}if(ƕ>=P){Ż.SetValue("Color",Ƙ);Ż.Enabled=true;č="Sequence Finished";}}void ǵ(){if(ǂ.Count>0&&Ķ){i=Ȉ(
"GpsListPosition",-1);j=T;if(i==ǂ.Count&&j==ǂ.Count){Ń=true;}}}void ǳ(){ǁ ȉ=null;if(!string.IsNullOrEmpty(ġ)&&ŗ){if(!ǂ.TryGetValue(ġ,out
ȉ)){return;}int ǌ=q;if(ĺ||Ķ||ȉ.Ȋ||ő){ŭ=true;}else ŭ=false;e=ȋ("Dst");if(e<ǂ.Count){ŀ=true;}else ŀ=false;if(!ȉ.Ȍ||Ķ||ĺ||ő)
{ȉ.ȍ=0;}Ɔ.Clear();if(ǂ.Count>0){if(ȉ.Ȏ>-1&&!ȉ.ȏ){ȉ.Ȏ=-1;}}if(ǂ.Count>0){if(ȉ.Ȏ>-1&&ȉ.ȏ&&ȉ.Ȑ.Contains("Docked")&&ȉ.ȑ==
"True"&&ȉ.Ȓ&&ȉ.ȓ){if(Ɛ.Count>0){if(Ɛ[ȉ.Ȏ]){ȉ.Ȏ=-1;ȉ.Ȓ=false;ȉ.ȏ=false;}}}}if((ȉ.Ȏ==-1&&!ȉ.ȏ&&ȉ.Ȕ=="True"&&ȉ.ȑ=="False"&&!ȉ.Ȍ&&
!ń)||(ȉ.Ȏ==-1&&!ȉ.ȏ&&ȉ.Ȕ=="False"&&ȉ.ȑ=="False"&&!ȉ.Ȍ&&!ń)){ȉ.Ȍ=true;}if(ȉ.Ȍ){á=ȉ.ȕ+" "+đ;IGC.SendBroadcastMessage(á,đ,
TransmissionDistance.TransmissionDistanceMax);}if(!ȉ.Ȍ){á=ȉ.ȕ+" "+đ;IGC.SendBroadcastMessage(á,Ē,TransmissionDistance.
TransmissionDistanceMax);}if(ȉ.Ȑ.Contains("Docked")&&ȉ.Ȏ==-1&&ȉ.Ȓ&&(ȉ.Ȗ==0||ȉ.Ȗ==8)){ȉ.Ȓ=false;}if(ȉ.Ȑ.Contains("Docked")&&ȉ.Ȏ==-1&&!ȉ.Ȓ&&(ȉ.Ȗ
==0||ȉ.Ȗ==8)){if(ȉ.ȗ.Count>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){if(!ȉ.Ȑ.Contains("Docked")){if(!ȉ.ȗ[Ș].Enabled
){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.Closed||ȉ.ȗ[Ș].Status==DoorStatus.Closing){ȉ.ȗ[Ș].OpenDoor();}if(ȉ.ȗ[
Ș].Status!=DoorStatus.Open){}}else if(ȉ.ȑ=="True"&&ȉ.Ȑ.Contains("Docked")){if(ȉ.ȗ[Ș].Status==DoorStatus.Open||ȉ.ȗ[Ș].
Status==DoorStatus.Opening){if(!Ļ){ȉ.ȗ[Ș].CloseDoor();}}if(ȉ.ȗ[Ș].Status!=DoorStatus.Closed){}}}}}}if(((R)>=d&&!ȉ.Ȓ&&ƕ<=P)||(d
==0&&!ȉ.Ȓ)){if(!İ||Ő){ȉ.ș=true;}if((İ&&Q>K)||(Ő)){ȉ.ș=true;}if(İ&&Q<=K){ȉ.ș=false;}}else if(((R)<d&&ƕ<P)||(ȉ.Ȓ&&(R)<=d)){
if(!İ){ȉ.ș=false;}if(İ&&Q<K){ȉ.ș=false;}if((İ&&Q>K)||(Ő)){ȉ.ș=true;}}if((ȉ.Ȏ==-1&&(R)>=d)||(Ő)){if(!İ){ȉ.ș=true;}if((İ&&Q
>=K)||(Ő)){ȉ.ș=true;}if((İ&&Q<K)||(Ő)){ȉ.ș=true;}}if(ȉ.Ȏ>-1&&ȉ.Ȏ<Ǝ.Count){if(Ə[ȉ.Ȏ]&&!ȉ.Ȓ){if(!İ||Ő){ȉ.ș=true;}if((İ&&Q>=K
)||(Ő)){ȉ.ș=true;}if((İ&&Q<K)||(Ő)){ȉ.ș=true;}}else if(ƕ<P&&!Ə[ȉ.Ȏ]&&!Ɛ[ȉ.Ȏ]&&!ȉ.Ȓ){if(!İ){ȉ.ș=false;}if(İ&&Q<K){ȉ.ș=
false;}if((İ&&Q>=K)||(Ő)){ȉ.ș=true;}}if(!Ɛ[ȉ.Ȏ]){int Ț=Ȉ("GpsListPosition",ȉ.Ȏ);if(Ə[ȉ.Ȏ]&&Ț==0){Ə[ȉ.Ȏ]=false;}}}ț(ȉ);f=ȉ.Ȏ;
if(ȉ.Ȑ=="Docked Idle"){ȉ.ȓ=true;}if(ȉ.Ȑ.Contains("Recharging")||ȉ.Ȑ.Contains("Unloading")){ȉ.ȓ=false;}if(ȉ.Ȑ.Contains(
"Idle")&&ȉ.Ȕ=="True"&&ȉ.ȑ=="False"&&d==0&&!ȉ.Ȍ){ȉ.Ȍ=true;}if(ȉ.Ȏ==-1&&ȉ.ȏ&&ȉ.ȑ=="True"&&ȉ.Ȑ.Contains("Docked")){ȉ.ȏ=false;}if(
ȉ.Ȏ>-1){if(ȉ.Ȗ==0&&ȉ.ȓ&&ȉ.Ȝ=="False"&&ȉ.Ȑ.Contains("Docked")&&Ɛ[ȉ.Ȏ]&&ȉ.Ȓ&&ȉ.ȏ&&ĵ){ȉ.Ȓ=false;ȉ.ȏ=false;Ə[ȉ.Ȏ]=true;ȉ.Ȏ=-1
;f=-1;Ŷ=f.ToString();ȝ(ȉ,"0");}if(ȉ.Ȗ>0&&ȉ.ȑ=="True"&&ȉ.Ȑ.Contains("Docked")&&!ȉ.ȏ&&ȉ.Ȏ>-1&&ĵ){ȉ.Ȓ=false;ȉ.Ȗ=0;ȉ.Ȏ=-1;f=-
1;Ŷ=f.ToString();ȝ(ȉ,"0");}}if(ȉ.ȓ&&ȉ.Ȝ=="False"&&ȉ.ȑ=="True"&&ĵ&&!ȉ.ȏ&&ȉ.Ȗ==0&&!ȉ.ș&&!ȉ.Ȓ&&!Ņ&&ļ){if(ƕ<P&&ļ&&!ȉ.ȏ&&!ȉ.ș)
{if(Ɛ.Count>0){if(H>Ɛ.Count){H=0;}if(H>0){for(int Ȟ=0;Ȟ<H;Ȟ++){if(Ȟ>Ɛ.Count-1||Ȟ>H-1){break;}Ɛ[Ȟ]=true;}}for(int ȟ=0;ȟ<Ɛ.
Count;ȟ++){if(ȟ>Ɛ.Count-1){ȟ=Ɛ.Count-1;}if(!Ɛ[ȟ]){if(ǂ.Count>0){int Ț=Ȉ("GpsListPosition",ȟ);if(!Ə[ȟ]&&Ț>0){Ə[ȟ]=true;}}}if(!
Ɛ[ȟ]&&!Ə[ȟ]){Ŭ=ȟ;break;}}}N=Ŭ;if(f==-1){f=Ŭ;ȉ.Ƞ=Ǝ[f];ȉ.Ȏ=f;}else{f=ȉ.Ȏ;if(f>-1&&f<Ǝ.Count){ȉ.Ƞ=Ǝ[f];}}if(!ļ){P=1;if(ǂ.
Count>0){ȉ.Ƞ=ž;}f=0;Ŭ=0;}ƌ.AppendLine($"Drone coords: {ǌ}");if(ǌ<ǂ.Count){ȉ.ȏ=true;ƌ.AppendLine(
$"Drone coords assigned: {ǌ} {ȉ.ȏ}");}}else if(!ļ){P=1;ȉ.Ƞ=ž;ȉ.ȏ=true;f=0;Ŭ=0;ƌ.AppendLine("invalid grid - defaulting");}ƌ.AppendLine("data staging");if(ȉ.
Ȏ>-1){if(Ə[ȉ.Ȏ]&&!ȉ.Ȓ){ȉ.ș=true;}else if(((R)<d&&ƕ<P)||(!Ə[ȉ.Ȏ]&&!Ɛ[ȉ.Ȏ]&&!ȉ.Ȓ)){ȉ.ș=false;}if(ƕ!=P&&!ȉ.ș){ȉ.Ȗ=1;ȉ.Ȓ=true
;Ə[ȉ.Ȏ]=true;}else{ȉ.Ȗ=0;ȉ.Ȓ=false;}if(Ɛ[ȉ.Ȏ]){ƌ.AppendLine($"Drone position finished {ǌ}");ȉ.Ȗ=0;ȉ.Ȓ=false;ȉ.ȏ=false;ȉ.Ȏ
=-1;f=-1;}}}Á=ȉ.ȕ;Ŷ=f.ToString();Ć="0";Ă=Math.Round(ȉ.Ƞ.X,2).ToString();ă=Math.Round(ȉ.Ƞ.Y,2).ToString();Ą=Math.Round(ȉ.Ƞ
.Z,2).ToString();ą=ð;ć=(Ţ+ţ).ToString();Ĉ=(Ů+ţ+drone_length-drone_clear_offset).ToString();if(ľ||Ŀ){ĉ=Math.Round(((ȉ.Ƞ.X-
ž.X)+ƀ.X),2).ToString();Ċ=Math.Round(((ȉ.Ƞ.Y-ž.Y)+ƀ.Y),2).ToString();ċ=Math.Round(((ȉ.Ƞ.Z-ž.Z)+ƀ.Z),2).ToString();}else{ĉ
="";Ċ="";ċ="";}if((ȉ.Ȗ==1&&ȉ.ȏ&&!ȉ.ș&&!Ņ)||(ȉ.Ȗ==2&&ȉ.Ȑ=="Docked Idle"&&ȉ.ȑ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ)){ȉ.ȡ=true;if(ȉ.ȗ.Count
>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){if(!ȉ.ȗ[Ș].Enabled){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.
Closed||ȉ.ȗ[Ș].Status==DoorStatus.Closing){ȉ.ȗ[Ș].OpenDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.Open){ȉ.ȡ=false;}}}}ȉ.Ȓ=true;if(ȉ.Ȏ
>-1){Ə[ȉ.Ȏ]=true;}ȉ.Ȣ=Ƅ.ToString();if(ȉ.ȡ){ȉ.Ȗ=2;ȝ(ȉ,"7");}else{ȉ.Ȗ=1;ȝ(ȉ,"0");}}if((ȉ.Ȗ==2&&ȉ.Ȑ=="Undocked"&&ȉ.Ȕ=="True"
&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ)||(ȉ.Ȗ==2&&ȉ.Ȑ=="Docking"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ)){ȉ.Ȗ=3;ȝ(ȉ,"0");}if(ȉ.Ȗ==2&&ȉ.Ȑ=="Undocking"&&ȉ.ȑ==
"False"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ&&ȉ.ȣ<=Ş){ȉ.Ȗ=13;ȝ(ȉ,"0");}if(ȉ.Ȗ==8&&ȉ.Ȑ.Contains("RTB Ready")&&ȉ.ȑ=="False"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){ȉ.ȡ=true;Ȥ(ȉ)
;if(ȉ.Ȏ>-1){Ə[ȉ.Ȏ]=false;}f=-1;ȉ.Ȗ=13;if(ķ&&ȉ.ȥ&&ȉ.ȡ){ȝ(ȉ,"0");}ȉ.ȥ=false;}if(((ȉ.Ȗ==13||ȉ.Ȗ==8)&&(ȉ.Ȑ=="Idle"||ȉ.Ȑ.
Contains("RTB"))&&ȉ.ȑ=="False"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ)||(ȉ.Ȗ==5&&ȉ.Ȑ=="Docking"&&ȉ.ȑ=="False"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ&&ȉ.ȣ<=Ş)){ȉ.ȡ=true;Ȥ(ȉ);ȉ.Ȗ=8
;if(ķ&&ȉ.ȥ&&ȉ.ȡ){ȝ(ȉ,"6");}ȉ.ȥ=false;}if(ȉ.Ȗ==3&&ȉ.Ȑ=="Idle"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){Ȧ(ȉ);ȉ.Ȗ=4;ȝ(ȉ,"4");}if(ȉ.Ȗ==4&&
ȉ.Ȑ=="Nav End"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){Ȧ(ȉ);ȉ.Ȗ=5;ȝ(ȉ,"0");}if(ȉ.Ȗ==4&&ȉ.Ȑ=="RTB"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){Ȧ(ȉ);ȉ.
Ȗ=8;f=-1;ȝ(ȉ,"6");}if(ȉ.Ȗ==4&&ȉ.Ȑ=="Undocked"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){Ȧ(ȉ);ȉ.Ȗ=2;ȝ(ȉ,"7");}if(ȉ.Ȗ==4&&ȉ.Ȑ==
"Docked Idle"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){ȉ.Ȗ=1;ȝ(ȉ,"0");}if(ȉ.Ȗ==5&&ȉ.Ȑ=="Idle"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){Ȧ(ȉ);ȉ.Ȗ=6;ȝ(ȉ,"2");}if(ȉ.Ȗ==6&&ȉ.Ȑ==
"Nav End"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){Ȧ(ȉ);ȉ.Ȗ=7;ȝ(ȉ,"0");}if(ȉ.Ȗ==7&&ȉ.Ȑ=="Idle"&&ȉ.Ȕ=="True"&&ȉ.ȏ&&ȉ.Ȓ&&!Ņ){ȉ.Ȗ=8;ȝ(ȉ,"5");}if(
(ȉ.Ȗ>=8&&ȉ.Ȑ.Contains("Dock")&&ȉ.Ȓ)||(ȉ.Ȗ==4&&ȉ.Ȑ.Contains("Docked")&&ȉ.Ȓ)){if(ȉ.Ȏ>-1){Ə[ȉ.Ȏ]=false;}}if(ȉ.Ȗ>=8&&(ȉ.Ȑ.
Contains("Dock")||ȉ.Ȑ.Contains("Exit")||ȉ.Ȑ.Contains("RTB"))&&ȉ.Ȓ&&ȉ.ȏ&&ȉ.Ȓ&&ȉ.Ȝ=="True"){if(ȉ.Ȏ>-1){if(!Ɛ[ȉ.Ȏ]){Ɛ[ȉ.Ȏ]=true;ƌ.
AppendLine($"Grid bore finished: {ȉ.Ȏ}");}}}if(ȉ.Ȗ==8&&ȉ.ȓ&&ȉ.ȑ=="True"&&(ȉ.Ȝ=="False"&&ȉ.Ȏ>-1&&ȉ.Ȏ<Ɛ.Count)&&ȉ.ȏ&&!Ņ){ȉ.Ȗ=1;ȝ(ȉ,
"0");}if((ȉ.Ȗ==8&&!ȉ.ȓ&&ȉ.ȑ=="True"&&ȉ.Ȝ=="False"&&ȉ.ȏ&&!Ņ)||(ȉ.Ȗ==8&&!ȉ.ȓ&&ȉ.ȑ=="True"&&ȉ.Ȝ=="True"&&ȉ.ȏ&&!Ņ)||(ȉ.Ȗ>=1&&ȉ.
Ȗ<=4&&!ȉ.ȓ&&ȉ.ȑ=="True"&&ȉ.Ȝ=="False"&&ȉ.ȏ&&!Ņ)){ȉ.Ȗ=0;ȉ.ȏ=false;ȉ.Ȓ=false;f=-1;ȝ(ȉ,"0");}if(ȉ.Ȗ==8&&ȉ.ȓ&&ȉ.Ȓ&&ȉ.ȑ==
"True"&&(ȉ.Ȝ=="True")&&ȉ.ȏ&&!Ņ){ȉ.Ȗ=9;ȝ(ȉ,"0");}if(ȉ.Ȗ==8&&ȉ.ȓ&&ȉ.ȑ=="True"&&ȉ.Ȏ==-1&&!ȉ.ȏ&&!Ņ){ȉ.Ȗ=0;ȝ(ȉ,"0");}if(ȉ.Ȗ==9&&ȉ.ȓ
&&ȉ.ȑ=="True"&&((ȉ.Ȝ=="True")&&ĵ&&ȉ.ȏ&&!Ņ)||(ȉ.Ȗ==9&&ȉ.ȓ&&ȉ.ȑ=="True"&&(ȉ.Ȝ=="True")&&(!ȉ.ȏ)&&!Ņ)){ȉ.Ȗ=10;ȝ(ȉ,"0");}if((ȉ.
Ȗ==10&&ȉ.ȓ&&ȉ.ȑ=="True"&&(ȉ.Ȝ=="True")&&ŭ&&ȉ.ȏ&&!Ņ)||(ȉ.Ȗ==10&&ȉ.ȓ&&ȉ.ȑ=="True"&&(ȉ.Ȝ=="True")&&ȉ.ȏ&&!Ņ)||(ȉ.Ȗ==0&&ȉ.ȓ&&ȉ
.ȑ=="True"&&(ȉ.Ȝ=="True")&&ȉ.ȏ&&!Ņ)){ȉ.Ȗ=11;O++;f=-1;ȉ.Ȋ=false;ȝ(ȉ,"8");}if(ȉ.Ȗ==11&&ȉ.ȓ&&ȉ.ȑ=="True"&&ȉ.Ȝ=="False"&&ȉ.ȏ
&&O<=P&&ļ&&!Ņ){ȉ.Ȗ=0;ȉ.ȏ=false;ȝ(ȉ,"0");}if((ȉ.Ȗ==11&&ȉ.Ȑ.Contains("Docked")&&ȉ.ȑ=="True"&&ȉ.Ȝ=="False"&&Ŭ<P&&ȉ.ȏ&&O>P&&!Ņ
)||(ȉ.Ȗ==11&&ȉ.ȓ&&ȉ.ȑ=="True"&&ȉ.Ȝ=="False"&&ȉ.ȏ&&ļ==false&&O>=P&&!Ņ)){ȉ.Ȗ=12;ȉ.ȏ=false;f=-1;ȝ(ȉ,"0");Ɔ.Append('\n');Ɔ.
Append("Mining seq. complete");}if(ȉ.Ȗ==12&&ȉ.Ȑ.Contains("RTB")&&ȉ.ȑ=="False"&&ȉ.Ȝ=="True"&&!Ņ){if(ȉ.ȗ.Count>0){for(int Ș=0;Ș<
ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){if(!ȉ.ȗ[Ș].Enabled){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.Closed||ȉ.ȗ[Ș].
Status==DoorStatus.Closing){ȉ.ȗ[Ș].OpenDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.Open){}}}}ȉ.ȏ=false;f=-1;ȝ(ȉ,"0");}if(ȉ.Ȗ==12&&ȉ.Ȑ
.Contains("Idle")&&ȉ.ȑ=="False"&&!ȉ.ȏ&&!Ņ){if(ȉ.ȗ.Count>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){if(!ȉ.ȗ[Ș].
Enabled){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.Closed||ȉ.ȗ[Ș].Status==DoorStatus.Closing){ȉ.ȗ[Ș].OpenDoor();}if(ȉ.ȗ
[Ș].Status!=DoorStatus.Open){}}}}f=-1;if(ķ&&ȉ.ȥ&&ȉ.ȡ){ȝ(ȉ,"6");}ȉ.ȥ=false;}if((ȉ.Ȑ.Contains("Docked")&&ȉ.ȑ=="True"&&ȉ.Ȝ==
"True"&&ŭ)||(ȉ.Ȑ.Contains("Docked")&&ȉ.ȑ=="True"&&ȉ.Ȝ=="True"&&ŭ&&!Ņ)){if(ȉ.ȗ.Count>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]
!=null){if(!ȉ.Ȑ.Contains("Docked")){if(!ȉ.ȗ[Ș].Enabled){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.Closed||ȉ.ȗ[Ș].
Status==DoorStatus.Closing){ȉ.ȗ[Ș].OpenDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.Open){}}else if(ȉ.ȑ=="True"&&ȉ.Ȑ.Contains("Docked"
)){if(ȉ.ȗ[Ș].Status==DoorStatus.Open||ȉ.ȗ[Ș].Status==DoorStatus.Opening){if(!Ļ){ȉ.ȗ[Ș].CloseDoor();}}if(ȉ.ȗ[Ș].Status!=
DoorStatus.Closed){}}}}}ȉ.Ȗ=0;O=0;ȉ.ȏ=false;ȉ.Ȓ=false;Ŭ=0;f=-1;ȉ.Ȋ=false;ȝ(ȉ,"8");}if((ȉ.Ȑ.Contains("Docked")&&ȉ.ȑ=="True"&&ȉ.Ȝ==
"False"&&ŭ&&ȉ.Ȗ==0&&!Ņ)||(ȉ.Ȑ.Contains("Docked")&&ȉ.ȑ=="True"&&ȉ.Ȝ=="False"&&ŭ&&!Ņ)||(ȉ.Ȗ==6&&ȉ.Ȑ=="Docked Idle"&&ȉ.ȑ=="True"&&
ȉ.ȏ&&ȉ.Ȓ&&!Ņ)){if(ȉ.ȗ.Count>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){if(!ȉ.Ȑ.Contains("Docked")){if(!ȉ.ȗ[Ș].
Enabled){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.Closed||ȉ.ȗ[Ș].Status==DoorStatus.Closing){ȉ.ȗ[Ș].OpenDoor();}if(ȉ.ȗ
[Ș].Status!=DoorStatus.Open){}}else if(ȉ.ȑ=="True"&&ȉ.Ȑ.Contains("Docked")){if(ȉ.ȗ[Ș].Status==DoorStatus.Open||ȉ.ȗ[Ș].
Status==DoorStatus.Opening){if(!Ļ){ȉ.ȗ[Ș].CloseDoor();}}if(ȉ.ȗ[Ș].Status!=DoorStatus.Closed){}}}}}ȉ.Ȗ=0;O=0;ȉ.ȏ=false;ȉ.Ȓ=
false;Ŭ=0;f=-1;ȉ.Ȋ=false;ȝ(ȉ,"0");}if(ĸ&&!ȉ.Ȍ&&!ń){ȉ.Ȍ=true;}if(ĸ&&!ȉ.Ȍ&&!ń){ȉ.Ȍ=true;}if(ȉ.Ȍ){ȉ.ȡ=true;if((ȉ.ȍ==0&&ȉ.Ȑ==
"Idle")||(ȉ.ȍ==0&&ȉ.Ȑ=="Undocked")||(ȉ.ȍ==0&&ȉ.Ȑ=="Nav")||(ȉ.ȍ==0&&ȉ.Ȑ=="Undocking")||(ȉ.ȍ==0&&ȉ.Ȑ=="Docking")||(ȉ.ȍ==0&&ȉ.Ȑ==
"Initiating mining")||(ȉ.ȍ==0&&ȉ.Ȑ.Contains("RTB"))){ȉ.ȍ=1;if(ȉ.Ȗ>0){ȉ.Ȗ=0;}}if(ȉ.ȗ.Count>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){
if(!ȉ.Ȑ.Contains("Docked")){if(!ȉ.ȗ[Ș].Enabled){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.Closed||ȉ.ȗ[Ș].Status==
DoorStatus.Closing){ȉ.ȗ[Ș].OpenDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.Open){ȉ.ȡ=false;}}else if(ȉ.ȑ=="True"&&ȉ.Ȑ.Contains("Docked"))
{if(ȉ.ȗ[Ș].Status==DoorStatus.Open||ȉ.ȗ[Ș].Status==DoorStatus.Opening){ȉ.ȗ[Ș].CloseDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.
Closed){ȉ.ȡ=false;}}}}}if(ȉ.ȍ==0&&ȉ.Ȑ=="Nav End"){ȉ.ȍ=3;}if(ȉ.ȍ==1){ȉ.ȍ=2;ȉ.Ȗ=0;f=ȉ.Ȏ;Ŷ=f.ToString();Ć="0";ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ
,Ċ,ċ);ȉ.Ȣ=Ƅ.ToString();}if(ȉ.ȍ==2&&ȉ.Ȑ=="Idle"){ȉ.ȍ=3;f=ȉ.Ȏ;Ŷ=f.ToString();Ć="1";ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,ċ);ȉ.Ȣ=Ƅ.ToString(
);}if(ȉ.ȍ==3&&ȉ.Ȑ=="Nav End"){ȉ.ȍ=4;f=ȉ.Ȏ;Ŷ=f.ToString();Ć="0";ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,ċ);ȉ.Ȣ=Ƅ.ToString();}if((ȉ.ȍ==3&&ȉ.Ȑ
=="Nav"&&ȉ.Ȏ==-1)||(ȉ.ȍ==3&&ȉ.Ȑ=="Idle"&&ȉ.Ȏ>=-1)||(ȉ.ȍ==3&&ȉ.Ȑ=="RTB"&&ȉ.Ȏ>=-1)){ȉ.ȍ=4;f=ȉ.Ȏ;Ŷ=f.ToString();Ć="0";ȧ(Ŷ,Ă,ă
,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,ċ);ȉ.Ȣ=Ƅ.ToString();}if(ȉ.ȍ==4&&ȉ.Ȑ=="Idle"){ȉ.ȍ=5;ȉ.Ȗ=0;f=ȉ.Ȏ;Ŷ=f.ToString();Ć="6";ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,
ċ);ȉ.Ȣ=Ƅ.ToString();}if(ȉ.ȍ==5&&ȉ.Ȑ=="Idle"){ȉ.ȍ=5;ȉ.Ȗ=0;f=ȉ.Ȏ;Ŷ=f.ToString();Ć="6";ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,ċ);ȉ.Ȣ=Ƅ.
ToString();}if((ȉ.ȍ==5&&ȉ.Ȑ.Contains("Docked"))||(ȉ.ȍ==0&&ȉ.Ȑ.Contains("Docked"))){ȉ.ȍ=0;ȉ.ȏ=false;ȉ.Ȗ=0;ȉ.Ȍ=false;ȉ.Ȓ=false;f=-
1;ȉ.Ȋ=true;Ŷ=f.ToString();Ć="0";ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,ċ);ȉ.Ȣ=Ƅ.ToString();}if(ȉ.ȡ){if(ķ&&ȉ.ȥ){Ȩ(ȉ);}}ȉ.ȥ=false;}if(ń){ȉ.ȡ
=true;if(ȉ.Ȑ=="Docked Idle"){if(ȉ.ȗ.Count>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){if(ȉ.Ȑ.Contains("Docked")){if(
!ȉ.ȗ[Ș].Enabled){ȉ.ȗ[Ș].Enabled=true;}if(ȉ.ȗ[Ș].Status==DoorStatus.Closed||ȉ.ȗ[Ș].Status==DoorStatus.Closing){ȉ.ȗ[Ș].
OpenDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.Open){ȉ.ȡ=false;}}else if(ȉ.Ȕ=="True"&&ȉ.Ȑ.Contains("Undocked")){if(ȉ.ȗ[Ș].Status==
DoorStatus.Open||ȉ.ȗ[Ș].Status==DoorStatus.Opening){ȉ.ȗ[Ș].CloseDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.Closed){ȉ.ȡ=false;}}}}}f=ȉ.Ȏ;
Ŷ=f.ToString();if(ȉ.ȡ){Ć="7";}ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,ċ);ȉ.Ȣ=Ƅ.ToString();if(ȉ.ȡ){if(ķ&&ȉ.ȥ){Ȩ(ȉ);}}ȉ.ȥ=false;}else{if(ȉ.ȗ.
Count>0){for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){if(ȉ.ȗ[Ș]!=null){if(ȉ.Ȕ=="True"&&ȉ.Ȑ.Contains("Undocked")){if(ȉ.ȗ[Ș].Status==DoorStatus
.Open||ȉ.ȗ[Ș].Status==DoorStatus.Opening){ȉ.ȗ[Ș].CloseDoor();}if(ȉ.ȗ[Ș].Status!=DoorStatus.Closed){ȉ.ȡ=false;}}}}}}}if(Ĺ)
{if(ȉ.Ȑ=="Undocked"||ȉ.Ȑ=="Idle"){f=ȉ.Ȏ;ȝ(ȉ,"0");}}ŗ=false;q=-1;if(Ƶ.Count>0){Ƶ.RemoveAt(0);}}}void ǲ(){if(ĸ){IGC.
SendBroadcastMessage(à,đ,TransmissionDistance.TransmissionDistanceMax);}else{IGC.SendBroadcastMessage(à,Ē,TransmissionDistance.
TransmissionDistanceMax);}if(ĸ&&Ŭ>0){Ŭ=0;}}void Ǳ(){ł=false;g=0;}void Ǯ(){if(ǂ.Count>0){K=ǂ.Count-F;if(K<=1){K=1;}if(K>G){K=G;}}}public void Ȫ(
IMyTerminalBlock Ǎ,string ȩ){ƽ.Clear();if(ƽ.TryParse(Ǎ.CustomData)){ƽ.Set("GMDIJobData","interfacecommand",ȩ);}else{ƽ.Set("GMDIJobData",
"interfacecommand",ȩ);}Ǎ.CustomData=ƽ.ToString();ƽ.Clear();}public void Ȭ(IMyTerminalBlock Ǎ){var ȫ="";ƽ.Clear();if(ƽ.TryParse(Ǎ.
CustomData)){ȫ=ƽ.Get("GMDIJobData","interfacecommand").ToString();ē=ȫ;}else{ē="";Ȫ(Ǎ,"");}ƽ.Clear();}void ǭ(){if(Ž==null||Ƨ[0]==
null){ƌ.AppendLine($"Interface PB not found {ħ}");}if((I==0&&!Ĳ)||(J==0&&!Ĳ)||(ş==0&&!Ĳ)){Ǝ.Clear();Ə.Clear();Ɛ.Clear();Ĳ=
true;ſ=ž;Ə.Add(false);Ɛ.Add(false);Ǝ.Add(ſ);P=Ǝ.Count;Ŭ=0;if(Ń){Ń=false;}if(u==0&&Ŝ){Ŝ=false;}if(u==1&&!Ŝ){Ŝ=true;}}if(!Ĳ){
if(!Ʊ){Ǝ.Clear();Ɛ.Clear();Ə.Clear();if(u==0&&Ŝ){Ŝ=false;}if(u==1&&!Ŝ){Ŝ=true;}Ʊ=true;}if(Ń){Ń=false;}if(Ɯ[0]==null||ż==
null){ƌ.AppendLine($"Remote control {æ.Replace("[","[[").Replace("]","]]")} not present - early exit");return;}Vector3D ȭ=ż.
GetNaturalGravity();Ǭ(Me.CustomData,Me);if(ľ||Ŀ){Ɓ=((ž-ƀ));}if(!ľ&&!Ŀ){Ɓ=ȭ;}Ɓ.Normalize();Vector3D Ȯ=Vector3D.
CalculatePerpendicularVector(Ɓ);Ȯ.Normalize();Vector3D ȯ=ž;if(!string.IsNullOrEmpty(Storage)&&!string.IsNullOrWhiteSpace(Storage)&&!Ĳ&&Ʊ&&!Œ&&!ś){Ŭ=
0;N=Ŭ;Ǒ(Storage);ƌ.AppendLine("Grid positions restored");ő=true;Storage=null;ĳ=false;h=0;Œ=true;}if((ŷ==null&&!Œ&&Ʊ)||(ŷ
!=null&&!ŷ.MoveNext()&&!Œ&&Ʊ)){ŷ=Ȱ(ȯ,Ɓ,ş,J,I,į,Ŝ);}if(ŷ!=null&&!Œ&&Ʊ){bool ȃ=ŷ.Current;if(!ŷ.MoveNext()){ƌ.AppendLine(
"Grid generation complete.");ŷ?.Dispose();ŷ=null;Ȱ(ȯ,Ɓ,ş,J,I,į,Ŝ).Dispose();}else{if(!ȃ){ƌ.AppendLine(
$"Generating grid positions... {Math.Round(Ū,1)}%");ŷ.MoveNext();}if(ȃ){Œ=true;Ȫ(Ž,"");ĺ=false;ň=false;ē="";}}}if(!Ĳ&&Ʊ&&Œ&&ś){Ǝ.Clear();Ɛ.Clear();Ə.Clear();Ŭ=0;N=Ŭ;ȱ(Me)
;ƌ.AppendLine("Grid positions restored");ő=true;Storage=null;ĳ=false;h=0;Œ=true;ś=false;}if(Ǝ.Count>0&&Œ){Ĳ=true;Ȫ(Ž,"");
ĺ=false;ň=false;ē="";}P=Ǝ.Count;if((I==0)||(I==0)||(ş==0)||(I==0&&I==0&&ş==0)){ļ=false;}if(I>0&&I>0&&ş>0){ļ=true;}if(!ļ){
P=1;}O=0;ƕ=0;Ŭ=0;}if(Ǝ.Count>0){if(C!=Ǝ.Count+(Ǝ.Count/2)+(ǂ.Count*10)){C=Ǝ.Count+(Ǝ.Count/2)+(ǂ.Count*10);}if(D!=(Ǝ.
Count/2)){D=(Ǝ.Count/2);}}else{if(C!=500){C=500;}if(D!=250){D=250;}}ƌ.AppendLine($"Grid: {Ĳ} - Bores: {P} - Remaining: {d}");
}void ǥ(){if(ź!=null&&ƞ[0]!=null){if(Ƴ.HasPendingMessage){MyIGCMessage Ȳ=Ƴ.AcceptMessage();Ƶ.Add(Ȳ);}if(Ƶ.Count>0){Ř=true
;}if(Ƶ.Count>0){Â=Ƶ[0].Data.ToString();ȳ(Â);ȴ();}if(Ƶ.Count<=0){Ř=false;}if(ƴ.HasPendingMessage){MyIGCMessage ȵ=ƴ.
AcceptMessage();ƶ.Add(ȵ);}if(ƶ.Count<=0){Ŗ=false;}if(ƶ.Count>0){Ŗ=true;Ã=ƶ[0].Data.ToString();if(ż!=null&&Ɯ[0]!=null){ǫ(Ã,ż,ĝ,Ĝ);}
else{ƌ.AppendLine($"Remote control {æ.Replace("[","[[").Replace("]","]]")} not present");return;}ƶ.RemoveAt(0);Ĳ=false;ň=
true;}}}void ǩ(){if(ź!=null&&ƞ[0]!=null){string ȶ="";if(ǂ.Count==0&&!ĳ||ǂ.Count>0&&!ĳ){IGC.SendBroadcastMessage(â,ä,
TransmissionDistance.TransmissionDistanceMax);if(!string.IsNullOrEmpty(å)&&!string.IsNullOrWhiteSpace(å)){ȶ=å;}else{ȶ="";}IGC.
SendBroadcastMessage(ã,ȶ,TransmissionDistance.TransmissionDistanceMax);ĳ=true;h=0;}}}void ǧ(){if(ǀ!=Me.CustomData){ǀ=Me.CustomData;}if(!
string.IsNullOrWhiteSpace(ǀ)){Ĵ=true;}else{ƌ.AppendLine($"Job custom data invalid - initialising job data");Ĵ=false;if(ǀ!=
"GPS:---:0:0:0:#FF75C9F1:5.0:10.0:1:1:0:False:1:10:0:False"){ƃ.Clear();ƃ.Append($"GPS:---:0:0:0:#FF75C9F1:5.0:10.0:1:1:0:False:1:10:0:False");ȷ(Me,ƃ.ToString());}}if(Ĵ){if(ǀ==ę){
return;}else if(ǀ!=ę){Ǭ(ǀ,Me);ę=ǀ;}}}void ȷ(IMyTerminalBlock Ǎ,string Ǐ){ƺ.Clear();ƺ.Set(ĝ,Ĝ,Ǐ);Ǎ.CustomData=ƺ.ToString();ƺ.
Clear();}void Ǧ(){if(!Œ&&o>0){o=0;}if(Œ&&o>=1||ĺ&&Œ){o=0;Œ=false;}if(ĺ&&Ĳ&&!Œ&&!ő){Ĳ=false;ļ=false;Ŭ=0;f=-1;Ʊ=false;Œ=false;ľ
=false;Ŀ=false;}if(ő){ő=false;}}void ǡ(){if(!Ł){Ǡ();Ł=true;Echo("Setup complete!");}ȸ();if(!Ł){return;}}void ǣ(){if(Ƨ.
Count>0){if(Ƨ[0]!=null){Ž=Ƨ[0];}else{ƌ.AppendLine($"Interface programmable block not present {ħ}");return;}Ň=true;if(Ġ!=Ž.
CustomData){Ȭ(Ž);Ġ=Ž.CustomData;}ƌ.AppendLine($"Interface PB: {ħ}");ƌ.AppendLine($"Display command: {ē} P:{ľ} C:{Ŀ}");}else{ƌ.
AppendLine($"Interface programmable block not present {ħ}");return;}if(Ƨ.Count>0){if(Ň&&!string.IsNullOrEmpty(Ž.CustomData)){if(ē
==""&&!ŏ){ŏ=true;}else{ŏ=false;}if(ē.Contains("init")&&!ň){ň=true;}if(!ē.Contains("init")&&ň){ň=false;}if(ē.Contains(
"reset")&&!ŉ){ŉ=true;}else{ŉ=false;}if(ē.Contains("run")&&!Ŋ){Ŋ=true;}else{Ŋ=false;}if(ē.Contains("recall")&&!ŋ){ŋ=true;}else{ŋ
=false;}if(ē.Contains("eject")&&!Ō){Ō=true;}else{Ō=false;}if(ē.Contains("freeze")&&!ō){ō=true;}else{ō=false;}if(ē.
Contains("stop")&&!Ŏ){Ŏ=true;}else{Ŏ=false;}}if(!Ň||ŏ||Ž.CustomData==null){ō=false;Ō=false;ŋ=false;Ŋ=false;ŉ=false;ň=false;}}}
void Ǥ(string Ǔ){if(Ǔ=="setup"&&Ł){Ł=false;Ǔ="";ƌ.AppendLine("Running Setup..");}if(Ǔ.Contains("run")||Ŋ){ĵ=true;Ķ=false;ķ=
true;ĸ=false;ĺ=false;Ļ=false;Ĺ=false;í="Run";}if(Ǔ.Contains("reset")||ŉ){Ķ=true;ń=false;ĵ=false;ķ=true;ĸ=false;ĺ=false;Ĺ=
false;Ļ=true;í="Reset";Ŭ=0;}if(Ǔ.Contains("stop")||Ŏ){ķ=false;ń=false;ĵ=false;Ķ=false;ĸ=false;ĺ=false;Ĺ=false;Ļ=true;ĺ=false;
í="Stop";}if(Ǔ.Contains("recall")||ŋ){ĸ=true;ń=false;Ķ=false;ķ=true;ĵ=false;Ĺ=false;Ļ=false;ĺ=false;í="Recall";Ŭ=0;}if(Ǔ.
Contains("init")||ň){ĸ=false;ń=false;Ķ=false;ķ=false;ĵ=false;ĺ=true;Ĺ=false;Ļ=false;í="Init";Ŭ=0;N=Ŭ;Storage=null;}if(Ǔ.Contains
("eject")||Ō){ĸ=false;ń=true;Ķ=false;ķ=true;ĵ=false;Ļ=false;ĺ=false;Ĺ=false;í="Eject";Ŭ=0;}if(Ǔ.Contains("freeze")||ō){ķ=
true;Ĺ=true;ń=false;ĵ=false;Ķ=false;Ļ=true;ĸ=false;ĺ=false;í="Freeze";}if(ń||ĸ||Ĺ){Ņ=true;}else{Ņ=false;}}void ȹ(){if(Ɣ.
Count!=ǂ.Count){Ɣ.Clear();foreach(ǁ ȉ in ǂ.Values){Ɣ.Add(ȉ.ȕ);}}}void Ƿ(bool Ⱥ=false){int Ǖ=Runtime.CurrentInstructionCount;
if(Ƥ.Count==0||ǂ.Count==0||ǈ.Count==0)return;ȹ();if(ņ){ƅ.Clear().Append(
$"Mining Drone Status {Ʋ} [{drone_tag}] - GMDC {w} {Ę}\n");ņ=false;}int Ȼ=drones_per_screen*ǈ.Count;if(Ȼ<Ɣ.Count){ƌ.AppendLine(
$"Insufficient displays '{é.Replace("[","[[").Replace("]","]]")}': {Ȼ} < {Ɣ.Count}");return;}if(Ⱥ){Ɣ.Sort();}if((drones_per_screen>0&&drones_per_screen<=4)){for(int ǌ=0;ǌ<Ɣ.Count;ǌ++){bool ȼ=false;Ƚ(ǌ,ȼ?
ǌ+1:ǌ,ȼ);int Ⱦ=ǌ/drones_per_screen;if(Ⱦ<ǈ.Count&&ǈ[Ⱦ]!=null){ǈ[Ⱦ].WriteText(ƅ);ņ=true;}}}else{for(int ǌ=0;ǌ<Ɣ.Count;ǌ+=2)
{bool ȼ=ǌ+1<Ɣ.Count;Ƚ(ǌ,ȼ?ǌ+1:ǌ,ȼ);int Ⱦ=ǌ/drones_per_screen;if(Ⱦ<ǈ.Count&&ǈ[Ⱦ]!=null&&(ǌ%drones_per_screen==
drones_per_screen-2||ǌ>=Ɣ.Count-2)){ǈ[Ⱦ].WriteText(ƅ);ņ=true;}}}}struct Ɏ{public int ȿ,ȑ,ɀ,Ȕ,Ɂ,ɂ,Ƀ,Ʉ,Ʌ,Ɇ,ɇ,Ɉ,ɉ,Ɋ,ɋ,Ɍ,ɍ;}void ǯ(){int Ǖ=
Runtime.CurrentInstructionCount;ƕ=ɏ(Ɛ);d=P-ƕ;Q=ȋ("IsMining");ư=Ȉ("ControlSequence",2);Ɏ ɐ=new Ɏ();foreach(ǁ ȉ in ǂ.Values){
string ɑ=ȉ.Ȑ;string ɓ=ȉ.ɒ;ɐ.ȿ+=ɑ.Contains("Docking")?1:0;ɐ.ȑ+=ɑ.Contains("Docked")?1:0;ɐ.ɀ+=ɑ.Contains("Undocking")?1:0;ɐ.Ȕ+=ɑ
.Contains("Undocked")?1:0;ɐ.Ʉ+=ɑ.Contains("Exit")?1:0;ɐ.Ʌ+=ɑ.Equals("Idle")?1:0;ɐ.Ɇ+=ɑ.Contains("Recharg")?1:0;ɐ.ɇ+=ɑ.
Contains("Unload")?1:0;ɐ.Ɉ+=ɑ.Contains("Min")?1:0;ɐ.ɉ+=ɑ.Contains("RTB: Request")?1:0;ɐ.Ɋ+=ɑ.Contains("RTB: Ready")?1:0;ɐ.ɋ+=ɑ.
Contains("Nav")?1:0;ɐ.Ɍ+=ɑ.Equals("Docked Idle")?1:0;ɐ.Ɂ+=ɓ=="DMG"?1:0;ɐ.ɂ+=ɓ=="UNK"?1:0;ɐ.Ƀ+=ɓ=="OK"?1:0;ɐ.ɍ=ȉ.ɍ=="True"?1:0;}S
=ɐ.ȿ;T=ɐ.ȑ;U=ɐ.ɀ;V=ɐ.Ȕ;a=ɐ.Ʉ;Y=ɐ.Ʌ;W=ɐ.Ɇ;X=ɐ.ɇ;b=ɐ.Ɉ;c=ɐ.ɋ;Z=ɐ.Ɍ;k=ɐ.Ɂ;l=ɐ.ɂ;R=Q-S;m=ɐ.ɍ;if(R<0){R=0;}}int[]ɔ=new int[0];
void ǰ(){int Ǖ=Runtime.CurrentInstructionCount;if(ư>0)Ő=true;if(Ő){n++;if(n>=undock_delay_limit){Ő=false;n=0;}}else if(n!=0)
n=0;if(Ə.Count>0){if(ɔ.Length<Ə.Count)ɔ=new int[Ə.Count];Array.Clear(ɔ,0,Ə.Count);foreach(ǁ ȉ in ǂ.Values)if(ȉ.Ȏ>=0&&ȉ.Ȏ<
ɔ.Length)ɔ[ȉ.Ȏ]++;for(int ɕ=0;ɕ<Ə.Count;ɕ++)if(Ə[ɕ]&&ɔ[ɕ]==0)Ə[ɕ]=false;}}public void ț(ǁ ȉ){Ɔ.Append(
$"Drone Controller Status - GMDC {w} - [{drone_tag}] {Ę}");Ɔ.Append('\n');if(ȉ.Ȗ==12){f=-1;ȉ.Ȓ=false;Ɔ.Append('\n');Ɔ.Append("Mining seq. complete");;}if(Ǝ.Count>0){Ɔ.Append(
'\n');Ɔ.Append("Grid pos: "+Ǝ.Count);Ɔ.Append('\n');Ɔ.Append("Grid dist: "+ò+"m #X: "+ó+" #Y: "+ô);Ɔ.Append('\n');Ɔ.Append(
"Grid OK: "+ļ);Ɔ.Append('\n');Ɔ.Append("Bores: "+Ǝ.Count+" Remain: "+d+"  Skip: "+H);Ɔ.Append('\n');}else{Ɔ.Append('\n');Ɔ.Append(
"Bores: "+P+" Remaining: "+d);}if(P>0){Ɔ.Append('\n');Ɔ.Append("Current mine idx: "+N+" of "+(P-1)+" ("+ƕ+") ");}else{Ɔ.Append(
'\n');Ɔ.Append("Current mine idx: "+Ŭ+" of "+(P-1)+" ("+ƕ+") ");}if(Ŭ>P||!ļ&&ƕ>=P||d==0){ĵ=false;Ɔ.Append('\n');Ɔ.Append(
"Mine seq. complete");Ɔ.Append('\n');ȉ.Ȗ=12;Ŭ=0;}}void ȳ(string ɖ){String[]ɗ=ɖ.Split(':');if(ɗ.Length>5){Ä=ɗ[0];if(Ä.Contains(drone_tag)){Å=
ɗ[1];Æ=ɗ[2];Ç=ɗ[3];È=ɗ[4];É=ɗ[5];if(ɗ.Length>6){Ê=ɗ[6];}if(ɗ.Length>7){Õ=ɗ[7];}if(ɗ.Length>8){Ø=ɗ[8];}if(ɗ.Length>9){Ù=ɗ[
9];}if(ɗ.Length>10){Ú=ɗ[10];}if(ɗ.Length>11){Ë=ɗ[11];}if(ɗ.Length>12){Ì=ɗ[12];}if(ɗ.Length>13){Í=ɗ[13];}if(ɗ.Length>14){Û
=ɗ[14];}if(ɗ.Length>15){Ü=ɗ[15];}if(ɗ.Length>16){Ý=ɗ[16];}if(ɗ.Length>17){Î=ɗ[17];}if(ɗ.Length>18){Ï=ɗ[18];}if(ɗ.Length>
19){Ð=ɗ[19];}if(ɗ.Length>20){Ñ=ɗ[20];}if(ɗ.Length>21){Ò=ɗ[21];}else{Ò="False";}if(ɗ.Length>22){if(ɗ[22]!=null){Ö=ɗ[22];}
else{Ö="";}}else{Ö="";}if(ɗ.Length>23){if(ɗ[23]!=null){Ó=ɗ[23];}else{Ó="N/A";}}else{Ó="N/A";}if(ɗ.Length>24){if(ɗ[24]!=null)
{Ô=ɗ[24];}}}else{Ä="";Å="";Æ="";Ç="";È="";É="";Ê="";Õ="";Ø="";Ù="";Ú="";Ë="";Ì="";Í="";Û="";Ü="";Ý="";Î="";Ï="";Ð="";Ö=""
;Ó="";Ô="";}if(Î==""){M=-1;}else{if(!int.TryParse(Î,out M)){M=-1;}}if(Û==""){š=0.0;}if(!double.TryParse(Û,out š)){š=0.0;}
if(!int.TryParse(Ö,out L)){L=0;}}}void Ǫ(string Ǐ,IMyTerminalBlock Ǎ){if(Ǎ==null||Ɯ[0]==null){Echo(
$"Remote Control {æ.Replace("[","[[").Replace("]","]]")} not present");return;}if(string.IsNullOrEmpty(Ǎ.CustomData)||string.IsNullOrWhiteSpace(Ǎ.CustomData)){ƌ.AppendLine(
"Prospector job data not found");return;}if(!string.IsNullOrEmpty(Ǎ.CustomData)&&!Ǎ.CustomData.Contains("[GMDCJobData]")){String[]ɘ=Ǎ.CustomData.Split(
':');if(ɘ.Length>0){ǫ(Ǎ.CustomData,Ǎ,ĝ,ƿ);}return;}if(string.IsNullOrWhiteSpace(Ǐ)||string.IsNullOrEmpty(Ǐ)){return;}ə(ż);
String[]ɚ=ě.Split(':');if(ɚ.Length<6){ų="";þ="";ÿ="";Ŵ="";Ľ=false;return;}if(ɚ.Length>6){Ľ=true;ų=ɚ[2];þ=ɚ[3];ÿ=ɚ[4];Ŵ=ɚ[6];if
(!double.TryParse(ų,out Ƃ.X)){Ƃ.X=0.0;ų="";}if(!double.TryParse(þ,out Ƃ.Y)){Ƃ.Y=0.0;þ="";}if(!double.TryParse(ÿ,out Ƃ.Z))
{Ƃ.Z=0.0;ÿ="";}if(!double.TryParse(Ŵ,out ţ)){ţ=0.0;}}if(ɚ.Length<11&&ɚ.Length>7&&!ľ){ŵ="";Ā="";ā="";ľ=false;return;}if(ɚ.
Length>7&&!ľ){bool ɛ=false;bool ɜ=false;bool ɝ=false;ŵ=ɚ[9];Ā=ɚ[10];ā=ɚ[11];if(!double.TryParse(ŵ,out ƀ.X)){ƀ.X=0.0;ŵ="";ɛ=
false;}else{ɛ=true;}if(!double.TryParse(Ā,out ƀ.Y)){ƀ.Y=0.0;Ā="";ɜ=false;}else{ɜ=true;}if(!double.TryParse(ā,out ƀ.Z)){ƀ.Z=
0.0;ā="";ɝ=false;}else{ɝ=true;}if(ɛ&&ɜ&&ɝ){ľ=true;}ɞ(ż,ě);}}void ə(IMyTerminalBlock Ǐ){var ȫ="";bool ɟ=false;bool ɠ=false;
bool ɡ=false;ƺ.Clear();if(ƺ.TryParse(Ǐ.CustomData.ToString())){ȫ=ƺ.Get(ĝ,ƿ).ToString().Trim();ě=ȫ;ȫ=ƺ.Get(ĝ,"TargetGPS").
ToString().Trim();String[]ɢ=ȫ.Split(':');if(ɢ.Length>=5){if(!double.TryParse(ɢ[2],out Ƃ.X)){Ƃ.X=0.0;}if(!double.TryParse(ɢ[3],
out Ƃ.Y)){Ƃ.Y=0.0;}if(!double.TryParse(ɢ[4],out Ƃ.Z)){Ƃ.Z=0.0;}}else{Ƃ.X=0.0;Ƃ.Y=0.0;Ƃ.Z=0.0;}ȫ=ƺ.Get(ĝ,"AlignGPS").
ToString().Trim();String[]ɣ=ȫ.Split(':');if(ɣ.Length>=5){if(!double.TryParse(ɣ[2],out ƀ.X)){ƀ.X=0.0;}else{ɟ=true;}if(!double.
TryParse(ɣ[3],out ƀ.Y)){ƀ.Y=0.0;}else{ɠ=true;}if(!double.TryParse(ɣ[4],out ƀ.Z)){ƀ.Z=0.0;}else{ɡ=true;}}else{ƀ.X=0.0;ƀ.Y=0.0;ƀ.Z
=0.0;}ȫ=ƺ.Get(ĝ,"SafeAlignDistance").ToString().Trim();if(!double.TryParse(ȫ,out ţ)){ţ=30.0;}}if(ɟ&&ɠ&&ɡ&&!Ŀ){Ŀ=true;}ƺ.
Clear();}void ɞ(IMyTerminalBlock Ǎ,string Ǐ){ƺ.Clear();if(ƺ.TryParse(Ǎ.CustomData.ToString())){ƺ.Set(ĝ,ƿ,Ǐ);ƺ.Set(ĝ,
"TargetGPS",$"GPS:PDT:{Ƃ.X}:{Ƃ.Y}:{Ƃ.Z}:#FF75C9F1:");ƺ.Set(ĝ,"AlignGPS",$"GPS:TGT:{ƀ.X}:{ƀ.Y}:{ƀ.Z}:#F77668:");ƺ.Set(ĝ,
"SafeAlignDistance",ţ);Ǎ.CustomData=ƺ.ToString();}else{ƺ.Set(ĝ,ƿ,Ǐ);ƺ.Set(ĝ,"TargetGPS",$"GPS:PDT:{Ƃ.X}:{Ƃ.Y}:{Ƃ.Z}:#FF75C9F1:");ƺ.Set(ĝ,
"AlignGPS",$"GPS:TGT:{ƀ.X}:{ƀ.Y}:{ƀ.Z}:#F77668:");ƺ.Set(ĝ,"SafeAlignDistance",ţ);Ǎ.CustomData=ƺ.ToString();}ƺ.Clear();}void Ǭ(
string Ǐ,IMyTerminalBlock Ǎ){if(!string.IsNullOrEmpty(Ǎ.CustomData)&&!Ǎ.CustomData.Contains(ĝ)){String[]ɤ=Ǎ.CustomData.
ToString().Split(':');if(ɤ.Length>0){ǫ(Ǎ.CustomData,Ǎ,ĝ,Ĝ);}ƌ.AppendLine("Dataconversion");return;}if(string.IsNullOrWhiteSpace(
Ǎ.CustomData.ToString())){ƌ.AppendLine("Datablank");return;}ɥ(Ǎ);String[]ɦ=ƾ.Split(':');Ŀ=false;if(ɦ.Length<10){Ű="";î=""
;ï="";ð="";ñ="";ò="";ó="";ô="";õ="";ö="";ø="";ù="";ú="";û="";ű="";ü="";ý="";Ų="";ƌ.AppendLine("Data format invalid - GPS:name:x:y:z:depth:grid:numx:numy:limit=True/False:flightfactor:flighthardlimit:perimeteronly 0,1"
);return;}if(ɦ.Length>4){bool ɧ;bool ɨ;bool ɩ;Ű=ɦ[2];î=ɦ[3];ï=ɦ[4];ð=ɦ[5];if(!double.TryParse(Ű,out ž.X)){ž.X=0.0;Ű="";ɧ=
false;}else{ɧ=true;}if(!double.TryParse(î,out ž.Y)){ž.Y=0.0;î="";ɨ=false;}else{ɨ=true;}if(!double.TryParse(ï,out ž.Z)){ž.Z=
0.0;ï="";ɩ=false;}else{ɩ=true;}if(ɧ&&ɨ&&ɩ){ı=true;}else{ı=false;}}if(ɦ.Length>6){ñ=ɦ[6];if(!Double.TryParse(ñ,out Ţ)){Ţ=1.0
;ñ="";}}if(ɦ.Length>7){ò=ɦ[7];if(!Double.TryParse(ò,out ş)){ş=0.0;ò="";}}if(ɦ.Length>8){ó=ɦ[8];if(!int.TryParse(ó,out J))
{J=0;ó="";}}if(ɦ.Length>9){ô=ɦ[9];if(!int.TryParse(ô,out I)){I=0;ô="";}}if(ɦ.Length>10){õ=ɦ[10];if(!Double.TryParse(õ,out
Ů)){Ů=0.0;õ="";}}if(ɦ.Length>11){ö=ɦ[11];if(!bool.TryParse(ö,out İ)){İ=false;ö="";}}if(ɦ.Length>12){ø=ɦ[12];if(!int.
TryParse(ø,out F)){F=1;ø="";}}if(ɦ.Length>13){ù=ɦ[13];if(!int.TryParse(ù,out G)){G=6;ù="";}}if(ɦ.Length>14){ú=ɦ[14];if(!int.
TryParse(ú,out u)){H=0;u=0;Ŝ=false;ú="";}}if(u==0&&Ŝ){Ŝ=false;}if(u==1&&!Ŝ){Ŝ=true;}if(ɦ.Length>15){û=ɦ[15];if(!bool.TryParse(û,
out į)){į=false;û="";}}if(ɦ.Length>16&&!ľ){ƌ.AppendLine($"gpsCommandLen:{ɦ.Length}");bool ɪ;bool ɫ;bool ɬ;if(ɦ.Length>18){ű
=ɦ[18];}if(ɦ.Length>19){ü=ɦ[19];}if(ɦ.Length>20){ý=ɦ[20];}if(ɦ.Length>22){Ų=ɦ[22];}if(!double.TryParse(ű,out ƀ.X)){ƀ.X=
0.0;ű="";ɪ=false;}else{ɪ=true;}if(!double.TryParse(ü,out ƀ.Y)){ƀ.Y=0.0;ü="";ɫ=false;}else{ɫ=true;}if(!double.TryParse(ý,out
ƀ.Z)){ƀ.Z=0.0;ý="";ɬ=false;}else{ɬ=true;}if(ɪ&&ɫ&&ɬ){Ŀ=true;}else{ľ=false;Ŀ=false;}if(!double.TryParse(Ų,out ţ)){ţ=30.0;Ų
="";}}if((ľ||Ŀ)&&ɦ.Length>16&&ɦ.Length<18){string ɭ=ƾ;string ɮ=ɭ+$"GPS:TGT:{ƀ.X}:{ƀ.Y}:{ƀ.Z}:#F77668:{ţ}:";ǫ(ɮ,Ǎ,ĝ,Ĝ);}}
void ɥ(IMyTerminalBlock Ǐ){var ȫ="";bool ɯ=false;ƺ.Clear();if(ƺ.TryParse(Ǐ.CustomData.ToString())){if(ƺ.ContainsKey(ĝ,Ĝ)){ȫ=
ƺ.Get(ĝ,Ĝ).ToString().Trim();ƾ=ȫ;}else{ƺ.Set(ĝ,Ĝ,ƾ);ɯ=true;}if(ƺ.ContainsKey("GMDCJobData","loadsave")){ȫ=ƺ.Get(
"GMDCJobData","loadsave").ToString().Trim();if(!bool.TryParse(ȫ,out ś)){ś=false;}}else{ƺ.Set("GMDCJobData","loadsave","false");ɯ=true
;}if(ƺ.ContainsKey("GMDCJobData","jobname")){ȫ=ƺ.Get("GMDCJobData","jobname").ToString().Trim();Ğ=ȫ;}else{ƺ.Set(
"GMDCJobData","jobname","");ɯ=true;}}if(ɯ){Ǐ.CustomData=ƺ.ToString();ɯ=false;}ƺ.Clear();}void ɰ(IMyTerminalBlock Ǐ){var ȫ="";bool ɯ=
false;ƺ.Clear();if(ƺ.TryParse(Ǐ.CustomData.ToString())){if(ƺ.ContainsKey(ĝ,Ĝ)){ȫ=ƺ.Get(ĝ,Ĝ).ToString().Trim();ƾ=ȫ;}else{ƺ.Set
(ĝ,Ĝ,ƾ);ɯ=true;}if(ƺ.ContainsKey("GMDCJobData","loadsave")){ȫ=ƺ.Get("GMDCJobData","loadsave").ToString().Trim();if(!bool.
TryParse(ȫ,out ś)){ś=false;}ƺ.Set("GMDCJobData","loadsave","true");ɯ=true;}if(ƺ.ContainsKey("GMDCJobData","jobname")){ȫ=ƺ.Get(
"GMDCJobData","jobname").ToString().Trim();Ğ=ȫ;}else{ƺ.Set("GMDCJobData","jobname","");ɯ=true;}}if(ɯ){Ǐ.CustomData=ƺ.ToString();ɯ=
false;}ƺ.Clear();}public void Ƚ(int ɱ,int ɲ,bool ɳ){if(Ɣ.Count<=0)return;ǁ ɴ;ǁ ɵ=null;if(!ǂ.TryGetValue(Ɣ[ɱ],out ɴ))return;if
(ɳ){if(!ǂ.TryGetValue(Ɣ[ɲ],out ɵ)){ɳ=false;}}bool ɶ=ɴ.Ȏ!=-1&&Ɛ.Count>0&&ɴ.Ȏ<Ɛ.Count;bool ɷ=ɳ&&ɵ.Ȏ!=-1&&Ɛ.Count>0&&ɵ.Ȏ<Ɛ.
Count;string ɸ="";string ɹ="";if((ɴ.ɒ=="DMG")||(ɴ.ɍ=="True")){ɸ="FLT";}else if((ĸ&&ɴ.ɺ)||(ɴ.Ȍ)){ɸ="RCL";}else if((ń&&ɴ.ɺ&&ɴ.ɒ
!="DMG")){ɸ="EGR";}else if(Ķ&&ɴ.ɺ&&ɴ.ɒ!="DMG"&&ɴ.ɍ!="True"){ɸ="RST";}else{ɸ="NOM";}if(ɳ){if((ɵ.ɒ=="DMG")||(ɴ.ɍ=="True")){ɹ
="FLT";}else if((ĸ&&ɵ.ɺ)||(ɵ.Ȍ)){ɹ="RCL";}else if((ń&&ɵ.ɺ&&ɵ.ɒ!="DMG")){ɹ="EGR";}else if(Ķ&&ɵ.ɺ&&ɵ.ɒ!="DMG"&&ɴ.ɍ!="True")
{ɹ="RST";}else{ɹ="NOM";}}ƅ.AppendLine();int ɻ;ɻ=ƅ.Length;ƅ.Append(Ɣ[ɱ]).Append(" Version: ").Append(ɴ.ɼ).Append(" - ").
Append(ɸ).Append(" ").Append(ɴ.ɒ);ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append(Ɣ[ɲ]).Append(" Version: ").Append(ɵ.ɼ
).Append(" - ").Append(ɹ).Append(" ").Append(ɵ.ɒ);ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append(Ɣ[ɱ]).Append(" Status: ").Append(ɴ.Ȑ
);ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append(Ɣ[ɲ]).Append(" Status: ").Append(ɵ.Ȑ);ƅ.AppendLine();ɻ=ƅ.Length;
ƅ.Append(Ɣ[ɱ]).Append(" Docked: ").Append((ɴ.ȑ)).Append(" Rdy: ").Append((ɴ.ȓ));ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));
if(ɳ)ƅ.Append(Ɣ[ɲ]).Append(" Docked: ").Append((ɵ.ȑ)).Append(" Rdy: ").Append((ɵ.ȓ));ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append(Ɣ[
ɱ]).Append(" Undocked: ").Append((ɴ.Ȕ)).Append(" Gates: ").Append(ɴ.ȗ.Count).Append(" ");ƅ.Append(' ',Math.Max(E-(ƅ.
Length-ɻ),0));if(ɳ)ƅ.Append(Ɣ[ɲ]).Append(" Undocked: ").Append((ɵ.Ȕ)).Append(" Gates: ").Append(ɵ.ȗ.Count).Append(" ");ƅ.
AppendLine();ɻ=ƅ.Length;ƅ.Append(Ɣ[ɱ]).Append(" Finished: ").Append((ɴ.Ȝ)).Append(" Bore: ");if(ɶ)ƅ.Append(Ɛ[ɴ.Ȏ]);else ƅ.Append(
"N/A");ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ){ƅ.Append(Ɣ[ɲ]).Append(" Finished: ").Append((ɵ.Ȝ)).Append(" Bore: ");if
(ɷ)ƅ.Append(Ɛ[ɵ.Ȏ]);else ƅ.Append("N/A");}ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append(Ɣ[ɱ]).Append(" Mining: ").Append((ɴ.Ȓ)).
Append(" HGR: ").Append((ɴ.ȡ));ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append(Ɣ[ɲ]).Append(" Mining: ").Append((ɵ.Ȓ)).
Append("  HGR: ").Append((ɵ.ȡ));ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append(Ɣ[ɱ]).Append(" Waiting: ").Append((ɴ.ș)).Append(" Reset: ")
.Append((ɴ.Ȋ));ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append(Ɣ[ɲ]).Append(" Waiting: ").Append((ɵ.ș)).Append(
" Reset: ").Append((ɵ.Ȋ));ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append("Charge: ").Append(ɴ.ɽ).Append("% Tank: ").Append(ɴ.ɾ).Append(
"% Cargo: ").Append(ɴ.ɿ).Append("%");ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append("Charge: ").Append(ɵ.ɽ).Append(
"% Tank: ").Append(ɵ.ɾ).Append("% Cargo: ").Append(ɵ.ɿ).Append("%");ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append("Drill depth: ").Append(ɴ.ʀ
).Append("m Start: ").Append(ɴ.ʁ).Append("m");ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append("Drill depth: ").
Append(ɵ.ʀ).Append("m Start: ").Append(ɵ.ʁ).Append("m");ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append("Current depth: ").Append(ɴ.ʂ).
Append("m").Append(" - AI Flt: ").Append((ɴ.ɍ));ƅ.Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append("Current depth: ").
Append(ɵ.ʂ).Append("m").Append(" - AI Flt: ").Append((ɵ.ɍ));ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append("Drone seq: ").Append(ɴ.Ȗ).
Append(" Recall seq: ").Append(ɴ.ȍ).Append(" ").Append((ɴ.Ȍ)).Append(" CReq: ").Append((ɴ.ʃ));ƅ.Append(' ',Math.Max(E-(ƅ.
Length-ɻ),0));if(ɳ)ƅ.Append("Drone seq: ").Append(ɵ.Ȗ).Append(" Recall seq: ").Append(ɵ.ȍ).Append(" ").Append((ɵ.Ȍ)).Append(
" CReq: ").Append((ɵ.ʃ));ƅ.AppendLine();ɻ=ƅ.Length;ƅ.Append("Location: ").Append(ɴ.Ȏ).Append(" Assigned: ").Append((ɴ.ȏ));ƅ.
Append(' ',Math.Max(E-(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append("Location: ").Append(ɵ.Ȏ).Append(" Assigned: ").Append((ɵ.ȏ));ƅ.
AppendLine();ɻ=ƅ.Length;ƅ.Append("X: ").Append(ɴ.ʄ).Append(" Y: ").Append(ɴ.ʅ).Append(" Z: ").Append(ɴ.ʆ);ƅ.Append(' ',Math.Max(E-
(ƅ.Length-ɻ),0));if(ɳ)ƅ.Append("X: ").Append(ɵ.ʄ).Append(" Y: ").Append(ɵ.ʅ).Append(" Z: ").Append(ɵ.ʆ);ƅ.AppendLine();}
IEnumerator<bool>ȇ(){if(!Ŕ){Ƈ.Append(Ʋ).Append(" Mining Grid Status [").Append(Ğ).Append("] - GMDC ").Append(w).Append(" ").Append(
Ę).AppendLine();Ƈ.Append("\nRemaining bores: ").Append(d).Append(" - Current Index: ").Append(Ŭ).AppendLine();Ŕ=true;}for
(int ǌ=0;ǌ<Ɛ.Count;ǌ++){Č="";for(int ʇ=0;ʇ<Ɣ.Count;ʇ++){ǁ ȉ;if(!ǂ.TryGetValue(Ɣ[ʇ],out ȉ))continue;if(!Ə[ǌ]){Č="";}else
if(ǌ==ȉ.Ȏ){Č=ȉ.ȕ;ȉ.ʈ++;}if(ȉ.ʈ>1){Ə[ǌ]=false;}ȉ.ʈ=0;}if(!Ɛ[ǌ]){Ƈ.Append("\nGrid Index: ").Append(ǌ).Append(" - Occupied: "
).Append(Ə[ǌ]).Append(" - Assigned: ").Append(Č);}if(ǌ==Ɛ.Count-1){œ=true;}ŧ=((double)ǌ/(double)Ɛ.Count)*100;yield return
false;}yield return true;}IEnumerator<bool>Ȱ(Vector3D ȯ,Vector3D ʉ,double ş,int J,int I,bool ʊ,bool Ŝ=false){int ʋ=0;int ʌ=0;
int ʍ=0;int ʎ=0;int ʏ=0;Vector3D ʐ=Vector3D.CalculatePerpendicularVector(ʉ);Vector3D ʑ=Vector3D.Cross(ʉ,ʐ);Vector3D ʒ=(J-1)
*0.5*ş*ʐ;Vector3D ʓ=(I-1)*0.5*ş*ʑ;if(Ŝ){int ʔ=J-2;int ʕ=I-2;if(ʔ<0)ʔ=0;if(ʕ<0)ʕ=0;ʌ=(J*I)-(ʔ*ʕ);}else{ʌ=J*I;}List<
Vector3D>ʖ=new List<Vector3D>();for(int ǌ=0;ǌ<J;ǌ++){for(int Ȟ=0;Ȟ<I;Ȟ++){if(Ŝ&&!(ǌ==0||ǌ==J-1||Ȟ==0||Ȟ==I-1)){continue;}
Vector3D ʗ=ȯ+ǌ*ş*ʐ-Ȟ*ş*ʑ-ʒ+ʓ;ʖ.Add(ʗ);}yield return false;}ʖ.Sort((ʘ,ʙ)=>Vector3D.DistanceSquared(ʘ,ȯ).CompareTo(Vector3D.
DistanceSquared(ʙ,ȯ)));foreach(var ʚ in ʖ){Ǝ.Add(ʚ);Ə.Add(false);Ɛ.Add(false);}yield return false;if(ʊ&&!Ŝ){ʎ=J-1;ʏ=I-1;Vector3D ʛ=(ʎ-1
)*0.5*ş*ʐ;Vector3D ʜ=(ʏ-1)*0.5*ş*ʑ;if(ʎ<1)ʎ=1;if(ʏ<1)ʏ=1;ʋ=ʎ*ʏ;if(ʋ>=1){List<Vector3D>ʝ=new List<Vector3D>();for(int ǌ=0;
ǌ<ʎ;ǌ++){for(int Ȟ=0;Ȟ<ʏ;Ȟ++){Vector3D ʗ=ȯ+ǌ*ş*ʐ-Ȟ*ş*ʑ-ʛ+ʜ;ʝ.Add(ʗ);}yield return false;}ʝ.Sort((ʘ,ʙ)=>Vector3D.
DistanceSquared(ʘ,ȯ).CompareTo(Vector3D.DistanceSquared(ʙ,ȯ)));foreach(var ʚ in ʝ){Ǝ.Add(ʚ);Ə.Add(false);Ɛ.Add(false);}yield return
false;}}ʍ=ʋ+ʌ;Ū=(double)Ǝ.Count/(double)ʍ;if(Ǝ.Count==ʍ){Œ=true;}else{Œ=false;}yield return true;}float ʣ(string ʞ,float ʟ,
float ʠ,float ʡ){float ʢ=ʞ.Length*ʡ;if(ʢ>ʟ){return(ʟ/ʢ)*ʠ;}return ʠ;}IEnumerator<bool>Ȃ(Vector3D ȯ,Vector3D ʉ,double ş,int J,
int I,bool ʊ,bool ʤ=true){float ʥ=Ʈ.Width*0.9f;float ʦ=14.0f;int ʧ=0;int ʨ=0;var ʩ=0.4f;var ʪ=Ʈ.Height*ʩ;var ʫ=Ʈ.Width*ʩ;
var ʬ=ʫ/J;var ʭ=ʪ/I;var ʮ=new Vector2(ʬ,ʭ);var ʯ=ş*J;var ʰ=ş*I;var ʱ=(float)(ʫ/ʯ)*1.5f;var ʲ=(float)(ʪ/ʰ)*1.5f;if(Ǝ.Count>0
){Vector3D ʐ=Vector3D.CalculatePerpendicularVector(ʉ);Vector3D ʑ=Vector3D.Cross(ʉ,ʐ);bool ʳ=ʤ&&ż!=null;double ʴ=1.0;
double ʵ=0.0;if(ʳ){ʱ*=0.8f;ʲ*=0.8f;if(ʮ!=new Vector2(ʬ*0.8f,ʭ*0.8f)){ʮ=new Vector2(ʬ*0.8f,ʭ*0.8f);}Vector3D ʶ=ż.GetPosition();
Vector3D ʷ=ʶ-ȯ;double ʸ=Vector3D.Dot(ʷ,ʐ);double ʹ=Vector3D.Dot(ʷ,ʑ);double ʺ=-ʸ;double ʻ=-ʹ;double ʼ=Math.Sqrt(ʺ*ʺ+ʻ*ʻ);if(ʼ>
0.0001){double ʽ=Math.Atan2(ʻ,ʺ);double ʾ=-Math.PI/2.0;double ʿ=ʾ-ʽ;ʴ=Math.Cos(ʿ);ʵ=Math.Sin(ʿ);}else{if(ʮ!=new Vector2(ʬ,ʭ)){
ʮ=new Vector2(ʬ,ʭ);}ʳ=false;}}float ˀ=Math.Min(Ʈ.Size.X,Ʈ.Size.Y)/2*0.8f;if(ż!=null){Vector3D ʶ=ż.GetPosition();Vector3D
ʷ=ʶ-ȯ;string ˁ="AH_BoreSight";var ˆ=Color.Purple;var ˇ=1.0f;double ʸ=Vector3D.Dot(ʷ,ʐ);double ʹ=Vector3D.Dot(ʷ,ʑ);double
ʺ=-ʸ;double ʻ=-ʹ;if(ʳ){if(ʮ!=new Vector2(ʬ*0.8f,ʭ*0.8f)){ʮ=new Vector2(ʬ*0.8f,ʭ*0.8f);}double ˈ=ʺ*ʴ-ʻ*ʵ;double ˉ=ʺ*ʵ+ʻ*ʴ;
ʺ=ˈ;ʻ=ˉ;}var ˊ=(float)ʺ*ʱ;var ˋ=(float)ʻ*ʲ;float ˌ=(float)Math.Sqrt(ˊ*ˊ+ˋ*ˋ);float ˍ=ˌ>0?ˊ/ˌ:0;float ˎ=ˌ>0?ˋ/ˌ:-1;var ˏ=Ʈ
.Center-ˀ*new Vector2(ˍ,ˎ);float ː=(float)Math.Atan2(ˎ,ˍ)+(float)Math.PI;var ˑ=new MySprite(){Type=SpriteType.TEXTURE,
Data=ˁ,Position=ˏ,RotationOrScale=ː,Size=ʮ,Color=ˆ.Alpha(ˇ),Alignment=TextAlignment.CENTER};if(ˌ!=0.0f){Ʒ.Add(ˑ);r++;}}Ƌ.
Clear();Ƌ.Append("--- ").Append(Ʋ).Append(" Mining Grid Status ---");var ˠ=new Vector2(256,20)+Ʈ.Position;Ģ=Ƌ.ToString();ǃ=ʣ(
Ģ,ʥ,1.0f,ʦ);var ˡ=new MySprite(){Type=SpriteType.TEXT,Data=Ģ,Position=ˠ,RotationOrScale=ǃ,Size=ʮ,Color=Color.WhiteSmoke.
Alpha(1.0f),Alignment=TextAlignment.CENTER,FontId="White"};Ʒ.Add(ˡ);ˠ=new Vector2(256,60)+Ʈ.Position;Ƌ.Clear();Ƌ.Append("[").
Append(Ğ).Append("] - Total Bores: ").Append(P).Append(" - Remaining: ").Append(d).Append(" - Drones: ").Append(R).Append(" ")
.Append("(").Append(Q).Append(")");Ģ=Ƌ.ToString();ǃ=ʣ(Ģ,ʥ,1.0f,ʦ);ˡ=new MySprite(){Type=SpriteType.TEXT,Data=Ģ,Position=ˠ
,RotationOrScale=ǃ,Size=ʮ,Color=Color.WhiteSmoke.Alpha(1.0f),Alignment=TextAlignment.CENTER,FontId="White"};Ʒ.Add(ˡ);Ƌ.
Clear();Ģ="";for(int ǌ=0;ǌ<Ǝ.Count;ǌ++){ʧ++;Vector3D ˢ=Ǝ[ǌ]-ȯ;double ˣ=Vector3D.Dot(ˢ,ʐ);double ˤ=Vector3D.Dot(ˢ,ʑ);double ˬ=
-ˣ;double ˮ=-ˤ;if(ʳ){double ˈ=ˬ*ʴ-ˮ*ʵ;double ˉ=ˬ*ʵ+ˮ*ʴ;ˬ=ˈ;ˮ=ˉ;}var Ͱ=-(float)ˬ;var ͱ=-(float)ˮ;string Ͳ=Ɛ[ǌ]?
"CircleHollow":"Circle";var ͳ=Ə[ǌ]?1.0f:0.5f;var ʹ=Ə[ǌ]?Color.LightSkyBlue:Color.DeepSkyBlue;var ʗ=new Vector2(Ͱ*ʱ,ͱ*ʲ)+Ʈ.Center;var Ͷ
=new MySprite(){Type=SpriteType.TEXTURE,Data=Ͳ,Position=ʗ,Size=ʮ,Color=ʹ.Alpha(ͳ),Alignment=TextAlignment.CENTER};Ʒ.Add(Ͷ
);Ũ=(((double)ǌ+(double)1)/((double)Ǝ.Count))*100;r++;yield return false;}if(Ɣ.Count>0){ʨ=0;for(int ʇ=0;ʇ<Ɣ.Count;ʇ++){ǁ
ȉ;if(!ǂ.TryGetValue(Ɣ[ʇ],out ȉ))continue;double ͷ=0.0;double ͺ=0.0;double ͻ=0.0;ʨ++;if(!double.TryParse(ȉ.ʄ,out ͷ)){ͷ=0.0
;}if(!double.TryParse(ȉ.ʅ,out ͺ)){ͺ=0.0;}if(!double.TryParse(ȉ.ʆ,out ͻ)){ͻ=0.0;}Vector3D ͼ=new Vector3D(ͷ,ͺ,ͻ);Vector3D ˢ
=ͼ-ȯ;double ˣ=Vector3D.Dot(ˢ,ʐ);double ˤ=Vector3D.Dot(ˢ,ʑ);double ˬ=-ˣ;double ˮ=-ˤ;if(ʳ){double ˈ=ˬ*ʴ-ˮ*ʵ;double ˉ=ˬ*ʵ+ˮ*
ʴ;ˬ=ˈ;ˮ=ˉ;}var Ͱ=-(float)ˬ;var ͱ=-(float)ˮ;string ͽ="Circle";var Ά=Color.Gray;var Έ=1.0f;if(ȉ.Ȑ.Contains("Docked")||ȉ.Ȑ.
Contains("Undocked")||ȉ.Ȑ.Contains("Docking")||ȉ.Ȑ.Contains("Undocking")){Έ=0.25f;}if(ȉ.Ȓ){if(ȉ.Ȑ.Contains("Min")){Ά=Color.
Purple;}else if(ȉ.Ȑ.Contains("Exit")){Ά=Color.Orange;}else if(ȉ.Ȑ.Contains("RTB: Ready")){Ά=Color.Green;}else if(ȉ.Ȑ.Contains(
"Undock")){Ά=Color.Yellow;}else{Ά=Color.Navy;}Έ=1.0f;}if(ȉ.ɒ=="DMG"){Ά=Color.Red;}var ʗ=new Vector2(Ͱ*ʱ,ͱ*ʲ)+Ʈ.Center;var Ͷ=new
MySprite(){Type=SpriteType.TEXTURE,Data=ͽ,Position=ʗ,Size=ʮ*0.8f,Color=Ά.Alpha(Έ),Alignment=TextAlignment.CENTER};Ʒ.Add(Ͷ);if(ȉ.
Ή.Contains("True")||ȉ.Ί.Contains("True")){if(ȉ.Ί.Contains("True")&&ȉ.Ή.Contains("True")){Ά=Color.White;}else if(ȉ.Ί.
Contains("True")){Ά=Color.YellowGreen;}else if(ȉ.Ή.Contains("True")){Ά=Color.RosyBrown;}var Ό=new MySprite(){Type=SpriteType.
TEXTURE,Data="CircleHollow",Position=ʗ,Size=ʮ*0.8f,Color=Ά.Alpha(Έ),Alignment=TextAlignment.CENTER};Ʒ.Add(Ό);r++;}if(ȉ.Ȑ.
Contains("Recharg")||ȉ.Ȑ.Contains("Unload")||ȉ.Ί.Contains("True")){Ά=Color.Yellow;var Ύ=new MySprite(){Type=SpriteType.TEXTURE,
Data="IconEnergy",Position=ʗ,Size=ʮ*0.8f,Color=Ά.Alpha(Έ),Alignment=TextAlignment.CENTER};Ʒ.Add(Ύ);r++;}Ά=Color.WhiteSmoke;
var Ώ=new MySprite(){Type=SpriteType.TEXT,Data=$"{ȉ.ȕ}- ({ȉ.ɽ}%)",Position=ʗ,RotationOrScale=0.3f,Size=ʮ*0.5f,Color=Ά.Alpha
(Έ),Alignment=TextAlignment.CENTER,FontId="White"};Ʒ.Add(Ώ);ũ=((double)ʨ/(double)Ɣ.Count)*100;r++;yield return false;}}if
(Ɣ.Count==0){ŕ=(ʧ==Ǝ.Count);}else if(Ɣ.Count>0){if(ʧ==Ǝ.Count&&ʨ==Ɣ.Count){ŕ=true;ʨ=0;}else{ŕ=false;}}yield return true;}
}public void Ȇ(ref MySpriteDrawFrame ȅ){if(r>=D&&!Ś){var ΐ=new MySprite();ȅ.Add(ΐ);ƌ.AppendLine("Frame shift");Ś=true;r++
;}var Ͷ=new MySprite(){Type=SpriteType.TEXTURE,Data="Grid",Position=Ʈ.Center,Size=Ʈ.Size,Color=Ƭ.ScriptForegroundColor.
Alpha(0.0f),Alignment=TextAlignment.CENTER};ȅ.Add(Ͷ);r++;for(int ǌ=0;ǌ<Ʒ.Count;ǌ++){ȅ.Add(Ʒ[ǌ]);}}int ɏ(List<bool>Α){int Β=0;
foreach(bool Γ in Α){if(Γ){Β++;}}return Β;}int ȋ(string Δ){int Β=0;foreach(ǁ ȉ in ǂ.Values){if(Δ=="Dst"&&ȉ.ɺ)Β++;if(Δ==
"IsMining"&&ȉ.Ȓ)Β++;}return Β;}int Ȉ(string Δ,int Ε){int Β=0;foreach(ǁ ȉ in ǂ.Values){if(Δ=="GpsListPosition"&&ȉ.Ȏ==Ε){Β++;}else
if(Δ=="ControlSequence"&&ȉ.Ȗ==Ε){Β++;}}return Β;}public void ȧ(string Ζ,string Η,string Θ,string Ι,string Κ,string Λ,
string Μ,string Ν,string Ξ,string Ο,string Π){const string Ρ="GPS:{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:";const string Σ=
"GPS:PAD:{0}:{1}:{2}:#FF75C9F1:";Ƅ.Clear().EnsureCapacity((ľ||Ŀ)?120:80);Ƅ.AppendFormat(Ρ,Ζ,Η,Θ,Ι,Κ,Λ,Μ,Ν);if(ľ||Ŀ)Ƅ.AppendFormat(Σ,Ξ,Ο,Π);}void Ȩ(ǁ ȉ){
IGC.SendBroadcastMessage(Á,ȉ.Ȣ,TransmissionDistance.TransmissionDistanceMax);}void Ǒ(string Ǐ){if(!string.IsNullOrEmpty(Ǐ)
&&!string.IsNullOrWhiteSpace(Ǐ)){var ȫ="";string Τ="";if(Ƹ.TryParse(Ǐ)){ȫ=Ƹ.Get("configuration","runargument").ToString().
Trim();Ě=ȫ;ȫ=Ƹ.Get("configuration","ship grid tag").ToString();secondary=ȫ;ȫ=Ƹ.Get("jobdata","gridstatus").ToString().Trim()
;Τ=ȫ;}Echo("Loading grid data");if(Ł){string[]Υ=Τ.Split(';');for(int ǌ=0;ǌ<Υ.Length;ǌ++){if(string.IsNullOrEmpty(Υ[ǌ]))
continue;string[]Φ=Υ[ǌ].Split(':');if(Φ.Length>=2){int Χ,Ψ;bool Ω=int.TryParse(Φ[0],out Χ);bool Ϊ=int.TryParse(Φ[1],out Ψ);Ɛ.Add
(Ω&&Χ>0);Ə.Add(Ϊ&&Ψ>0);if(Φ.Length>=5){double Ϋ=double.TryParse(Φ[2],out Ť)?Ť:0.0;double ά=double.TryParse(Φ[3],out ť)?ť:
0.0;double έ=double.TryParse(Φ[4],out Ŧ)?Ŧ:0.0;Ǝ.Add(new Vector3D(Ϋ,ά,έ));}else{Ǝ.Add(new Vector3D(0,0,0));}}}}}}void ȱ(
IMyProgrammableBlock Ǎ){var ȫ="";string Τ="";Echo("Loading grid data");if(Ł){Ƹ.Clear();if(Ƹ.TryParse(Ǎ.CustomData.ToString())){ȫ=Ƹ.Get(
"jobdata","gridstatus").ToString().Trim();Τ=ȫ;string[]Υ=Τ.Split(';');for(int ǌ=0;ǌ<Υ.Length;ǌ++){if(string.IsNullOrEmpty(Υ[ǌ]))
continue;string[]Φ=Υ[ǌ].Split(':');if(Φ.Length>=2){int Χ,Ψ;bool Ω=int.TryParse(Φ[0],out Χ);bool Ϊ=int.TryParse(Φ[1],out Ψ);Ɛ.Add
(Ω&&Χ>0);Ə.Add(Ϊ&&Ψ>0);if(Φ.Length>=5){double Ϋ=double.TryParse(Φ[2],out Ť)?Ť:0.0;double ά=double.TryParse(Φ[3],out ť)?ť:
0.0;double έ=double.TryParse(Φ[4],out Ŧ)?Ŧ:0.0;Ǝ.Add(new Vector3D(Ϋ,ά,έ));}else{Ǝ.Add(new Vector3D(0,0,0));}}}}Ƹ.Set(ĝ,
"loadsave",false);}Ǎ.CustomData=Ƹ.ToString();Ƹ.Clear();}void ί(int ή){if(ή==0){Ę=".---";}if(ή==1){Ę="-.--";}if(ή==2){Ę="--.-";}if(
ή==3){Ę="---.";}}void ȁ(){p++;if(p>3){p=0;}ί(p);}public void ǒ(string Ǐ){if(string.IsNullOrWhiteSpace(Ǐ)){ƌ.AppendLine(
"No arguments provided, using defaults.");drone_tag="SWRM_D";drone_length=2.6;drone_clear_offset=12.0;secondary="";return;}string[]ΰ=Ǐ.Split(',');if(ΰ.Length==0
){ƌ.AppendLine("No arguments provided, using defaults.");return;}if(ΰ.Length>=1&&!string.IsNullOrWhiteSpace(ΰ[0])){
drone_tag=ΰ[0].ToString().Trim();}else{drone_tag="SWRM_D";}if(ΰ.Length>1){if(string.IsNullOrWhiteSpace(ΰ[0])){ƌ.AppendLine(
"Drone tag is empty, using default: SWRM_D");}for(int ǌ=1;ǌ<ΰ.Length;ǌ++){if(ΰ[ǌ]!=null){if(!string.IsNullOrWhiteSpace(ΰ[ǌ])){string[]α=ΰ[ǌ].Split('=');if(α.Length
>0){if(α[0].Contains(ǋ)){if(α.Length>1){if(α[1]!=null&&!string.IsNullOrWhiteSpace(α[1])){secondary=α[1].Trim();}else{
secondary="";}}}if(α[0].Contains(Ī)){if(α.Length>1){if(α[1]!=null&&!string.IsNullOrWhiteSpace(α[1])){if(!double.TryParse(α[1].
Trim(),out drone_length)){drone_length=2.6;}}else{drone_length=2.6;}}}if(α[0].Contains(ī)){if(α.Length>1){if(α[1]!=null&&!
string.IsNullOrWhiteSpace(α[1])){if(!double.TryParse(α[1].Trim(),out drone_clear_offset)){drone_clear_offset=12.0;}}else{
drone_clear_offset=12.0;}}}if(α[0].Contains(Ĭ)){if(α.Length>1){if(α[1]!=null&&!string.IsNullOrWhiteSpace(α[1])){if(!bool.TryParse(α[1].
Trim(),out ŝ)){ŝ=true;}}else{ŝ=true;}}}if(α[0].Contains(ĭ)){if(α.Length>1){if(α[1]!=null&&!string.IsNullOrWhiteSpace(α[1])){
if(!int.TryParse(α[1].Trim(),out drones_per_screen)){drones_per_screen=8;}}else{drones_per_screen=8;}}}if(α[0].Contains(Į)
){if(α.Length>1){if(α[1]!=null&&!string.IsNullOrWhiteSpace(α[1])){if(!int.TryParse(α[1].Trim(),out undock_delay_limit)){
undock_delay_limit=12;}droneUndockDelayTime=undock_delay_limit/2;}else{undock_delay_limit=12;droneUndockDelayTime=undock_delay_limit/2;}}}
}}}}}β();ɰ(Me);}public void Ǡ(){ǘ();Echo("Running Setup");IMyGridTerminalSystem γ=GridTerminalSystem as
IMyGridTerminalSystem;Echo("Loading Names");æ="["+drone_tag+" "+x+"]";ç="["+drone_tag+" "+x+"]";è="["+drone_tag+" "+y+" "+º+"]";é="["+
drone_tag+" "+z+" "+º+"]";ê="["+drone_tag+" "+µ+" "+º+"]";ì="["+drone_tag+" "+ª+"]";ë="["+drone_tag+" "+À+" "+º+"]";Ʋ="["+
secondary+"]";Þ=drone_tag+" "+Ď;ß=drone_tag+" "+ď;à=drone_tag+" "+đ;â="["+drone_tag+"]"+" "+ä;ã="["+drone_tag+"]"+" "+Đ;å=
secondary;Echo("Clearning Lists");Ǆ.Clear();ǅ.Clear();ǆ.Clear();bool δ=false;γ.GetBlocksOfType<IMyMotorStator>(Ǆ,ʙ=>ʙ.TopGrid==Me
.CubeGrid);if(Ǆ.Count<=0){Echo("Rotor top grid not found, checking advanced rotors");}if(Ǆ.Count>0){if(Ǆ[0]!=null){ƭ=Ǆ[0]
.CubeGrid;Echo("Local cubegrid found - rotor");δ=true;}}γ.GetBlocksOfType<IMyMotorAdvancedStator>(ǅ,ʙ=>ʙ.TopGrid==Me.
CubeGrid);if(ǅ.Count<=0){Echo("Rotor top grid not found, checking advanced rotors");}if(ǅ.Count>0){if(ǅ[0]!=null){ƭ=ǅ[0].
CubeGrid;Echo("Local cubegrid found - advanced rotor/hinge");δ=true;}}γ.GetBlocksOfType<IMyPistonBase>(ǆ,ʙ=>ʙ.TopGrid==Me.
CubeGrid);if(ǆ.Count<=0){Echo("Rotor top grid not found, checking pistons");}if(ǆ.Count>0){if(ǆ[0]!=null){ƭ=ǆ[0].CubeGrid;Echo(
"Local cubegrid found - piston");δ=true;}}if(ǅ.Count==0&&Ǆ.Count==0&&ǆ.Count==0){ƭ=Me.CubeGrid;Echo("Local cubegrid found - PB");δ=false;}Ǆ.Clear();ǅ.
Clear();ǆ.Clear();Echo("Stage 1");Ǝ.Clear();Ƒ.Clear();Echo("Stage 2");Ʒ.Clear();ƛ.Clear();Ɯ.Clear();Ɣ.Clear();ƒ.Clear();Ɠ.
Clear();Echo("Stage 3");ƃ.Clear();Ɔ.Clear();Ƈ.Clear();ƅ.Clear();Ƅ.Clear();ƈ.Clear();Ɗ.Clear();Echo("ListClear Complete");Ʊ=
false;Ƴ=IGC.RegisterBroadcastListener(Þ);ƴ=IGC.RegisterBroadcastListener(ß);for(int ǌ=0;ǌ<12;ǌ++){ƒ.Add("");Ɠ.Add("");}Ɲ.
Clear();ƞ.Clear();if(δ){γ.GetBlocksOfType<IMyRadioAntenna>(Ɲ,ʙ=>ʙ.CubeGrid==ƭ);if(Ɲ.Count>0){for(int ǌ=0;ǌ<Ɲ.Count;ǌ++){if(Ɲ[
ǌ].CustomName.Contains(æ)||Ɲ[ǌ].CustomName.Contains(x)){string ε=Ɲ[ǌ].CustomData;if(string.IsNullOrEmpty(drone_tag)||
string.IsNullOrWhiteSpace(drone_tag)){Echo($"Invalid name for drone_tag {drone_tag} please add drone tag to GMDC antenna custom data '<yourdronetaghere>:<Yourshiptaghere>:' e.g. 'SWRM_D:Atlas:'"
);return;}Ɲ[ǌ].CustomName=$"GMDC Antenna {Ʋ} {æ}";ƞ.Add(Ɲ[ǌ]);}}}Ɲ.Clear();}γ.GetBlocksOfType<IMyRadioAntenna>(Ɲ,ʙ=>ʙ.
CubeGrid==Me.CubeGrid);if(Ɲ.Count>0){for(int ǌ=0;ǌ<Ɲ.Count;ǌ++){if(Ɲ[ǌ].CustomName.Contains(æ)||Ɲ[ǌ].CustomName.Contains(x)){
string ε=Ɲ[ǌ].CustomData;if(string.IsNullOrEmpty(drone_tag)||string.IsNullOrWhiteSpace(drone_tag)){Echo($"Invalid name for drone_tag {drone_tag} please add drone tag to GMDC antenna custom data '<yourdronetaghere>:<Yourshiptaghere>:' e.g. 'SWRM_D:Atlas:'"
);return;}Ɲ[ǌ].CustomName=$"GMDC Antenna {Ʋ} {æ}";ƞ.Add(Ɲ[ǌ]);}}}Ɲ.Clear();Me.CustomName=
$"GMDC Programmable Block {Ʋ} {æ}";ƛ.Clear();Ɯ.Clear();if(δ){γ.GetBlocksOfType<IMyRemoteControl>(ƛ,ʙ=>ʙ.CubeGrid==Me.CubeGrid);if(ƛ.Count>0){for(int ǌ=0;ǌ
<ƛ.Count;ǌ++){if(ƛ[ǌ].CustomName.Contains(æ)||ƛ[ǌ].CustomName.Contains(x)){ƛ[ǌ].CustomName=$"GMDC Remote Control {Ʋ} {æ}"
;Ɯ.Add(ƛ[ǌ]);}}}ƛ.Clear();}γ.GetBlocksOfType<IMyRemoteControl>(ƛ,ʙ=>ʙ.CubeGrid==ƭ);if(ƛ.Count>0){for(int ǌ=0;ǌ<ƛ.Count;ǌ
++){if(ƛ[ǌ].CustomName.Contains(æ)||ƛ[ǌ].CustomName.Contains(x)){ƛ[ǌ].CustomName=$"GMDC Remote Control {Ʋ} {æ}";Ɯ.Add(ƛ[ǌ]
);}}}ƛ.Clear();Ɵ.Clear();Ơ.Clear();if(δ){γ.GetBlocksOfType<IMyLightingBlock>(Ɵ,ʙ=>ʙ.CubeGrid==Me.CubeGrid);if(Ɵ.Count>0){
for(int ǌ=0;ǌ<Ɵ.Count;ǌ++){if(Ɵ[ǌ].CustomName.Contains(ç)||Ɵ[ǌ].CustomName.Contains(x)){Ɵ[ǌ].CustomName=
$"GMDC Indicator Light {Ʋ} {ç}";Ơ.Add(Ɵ[ǌ]);}}}Ɵ.Clear();}γ.GetBlocksOfType<IMyLightingBlock>(Ɵ,ʙ=>ʙ.CubeGrid==ƭ);for(int ǌ=0;ǌ<Ɵ.Count;ǌ++){if(Ɵ[ǌ].
CustomName.Contains(ç)||Ɵ[ǌ].CustomName.Contains(x)){Ɵ[ǌ].CustomName=$"GMDC Indicator Light {Ʋ} {ç}";Ơ.Add(Ɵ[ǌ]);}}Ɵ.Clear();ơ.
Clear();Ƣ.Clear();ƣ.Clear();Ƥ.Clear();ƥ.Clear();Ǉ.Clear();ǈ.Clear();ǉ.Clear();Ǌ.Clear();if(δ){γ.GetBlocksOfType<
IMyTerminalBlock>(ơ,ʙ=>ʙ.CubeGrid==Me.CubeGrid);if(ơ.Count>0){for(int ǌ=0;ǌ<ơ.Count;ǌ++){if(ơ[ǌ].CustomName.Contains(è)){Ƣ.Add(ơ[ǌ]);Ǉ.
Add(((IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(srfM));}if(ơ[ǌ].CustomName.Contains(é)){Ƥ.Add(ơ[ǌ]);ǈ.Add(((
IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(srfD));}if(ơ[ǌ].CustomName.Contains(ê)){ƣ.Add(ơ[ǌ]);ǉ.Add(((IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(
srfL));}if(ơ[ǌ].CustomName.Contains(ë)){ƥ.Add(ơ[ǌ]);Ǌ.Add(((IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(srfV));}}}ơ.Clear();}γ.
GetBlocksOfType<IMyTerminalBlock>(ơ,ʙ=>ʙ.CubeGrid==ƭ);if(ơ.Count>0){for(int ǌ=0;ǌ<ơ.Count;ǌ++){if(ơ[ǌ].CustomName.Contains(è)){Ƣ.Add(ơ[
ǌ]);Ǉ.Add(((IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(srfM));}if(ơ[ǌ].CustomName.Contains(é)){Ƥ.Add(ơ[ǌ]);ǈ.Add(((
IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(srfD));}if(ơ[ǌ].CustomName.Contains(ê)){ƣ.Add(ơ[ǌ]);ǉ.Add(((IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(
srfL));}if(ơ[ǌ].CustomName.Contains(ë)){ƥ.Add(ơ[ǌ]);Ǌ.Add(((IMyTextSurfaceProvider)ơ[ǌ]).GetSurface(srfV));}}}ơ.Clear();Ʀ.
Clear();Ƨ.Clear();if(δ){γ.GetBlocksOfType<IMyProgrammableBlock>(Ʀ,ʙ=>ʙ.CubeGrid==Me.CubeGrid);if(Ʀ.Count>0){for(int ǌ=0;ǌ<Ʀ.
Count;ǌ++){if(Ʀ[ǌ].CustomName.Contains(ì)||Ʀ[ǌ].CustomName.Contains(ª)){Ʀ[ǌ].CustomName=$"GMDI Programmable Block {Ʋ} {ì}";Ƨ.
Add(Ʀ[ǌ]);}}}Ʀ.Clear();}γ.GetBlocksOfType<IMyProgrammableBlock>(Ʀ,ʙ=>ʙ.CubeGrid==ƭ);if(Ʀ.Count>0){for(int ǌ=0;ǌ<Ʀ.Count;ǌ++
){if(Ʀ[ǌ].CustomName.Contains(ì)||Ʀ[ǌ].CustomName.Contains(ª)){Ʀ[ǌ].CustomName=$"GMDI Programmable Block {Ʋ} {ì}";Ƨ.Add(Ʀ
[ǌ]);}}}Ʀ.Clear();Ƶ.Clear();ƶ.Clear();if(Runtime.UpdateFrequency==UpdateFrequency.Update1){v=1;}if(Runtime.
UpdateFrequency==UpdateFrequency.Update10){v=10;}if(Runtime.UpdateFrequency==UpdateFrequency.Update100){v=100;}if(Ǌ.Count>0){if(Ǌ[0]!=
null){Ƭ=Ǌ[0];if(Ƭ.ContentType!=ContentType.SCRIPT){ƌ.AppendLine("Correcting visualiser display");Ƭ.ContentType=ContentType.
SCRIPT;Ƭ.Script="";ř=true;Ʈ=new RectangleF((Ƭ.TextureSize-Ƭ.SurfaceSize)/2f,Ƭ.SurfaceSize);}}}if(Ƭ==null){Echo(
$"Panel:'{srfV}' on '{ë.Replace("[","[[").Replace("]","]]")}' not found");}if(ƥ.Count<=0||ƥ[0]==null){Echo($"Display with tag '{ë.Replace("[","[[").Replace("]","]]")}' not found");ř=false;}if(
ƥ.Count>0&&ƥ[0]!=null){Echo($"Display with tag '{ë.Replace("[","[[").Replace("]","]]")}' found");Ʈ=new RectangleF((Ƭ.
TextureSize-Ƭ.SurfaceSize)/2f,Ƭ.SurfaceSize);ř=true;}if(Ǉ.Count>0){for(int ǌ=0;ǌ<Ǉ.Count;ǌ++){if(Ǉ[ǌ]!=null){if(Ǉ[ǌ].ContentType!=
ContentType.TEXT_AND_IMAGE){Ǉ[ǌ].ContentType=ContentType.TEXT_AND_IMAGE;Ǉ[ǌ].Alignment=TextAlignment.LEFT;Ǉ[ǌ].FontSize=0.50f;Ǉ[ǌ].
Font="White";}}}}if(ǈ.Count>0){for(int ǌ=0;ǌ<ǈ.Count;ǌ++){if(ǈ[ǌ]!=null){if(ǈ[ǌ].ContentType!=ContentType.TEXT_AND_IMAGE){ǈ[
ǌ].ContentType=ContentType.TEXT_AND_IMAGE;ǈ[ǌ].Alignment=TextAlignment.LEFT;ǈ[ǌ].FontSize=0.290f;ǈ[ǌ].Font="Monospace";}}
}}if(ǉ.Count>0){for(int ǌ=0;ǌ<ǉ.Count;ǌ++){if(ǉ[ǌ]!=null){if(ǉ[ǌ].ContentType!=ContentType.TEXT_AND_IMAGE){ǉ[ǌ].
ContentType=ContentType.TEXT_AND_IMAGE;ǉ[ǌ].Alignment=TextAlignment.LEFT;ǉ[ǌ].FontSize=0.50f;ǉ[ǌ].Font="White";}}}}if(Ǌ.Count>0){
for(int ǌ=0;ǌ<Ǌ.Count;ǌ++){if(Ǌ[ǌ]!=null){if(Ǌ[ǌ].ContentType!=ContentType.SCRIPT){Ǌ[ǌ].ContentType=ContentType.SCRIPT;Ǌ[ǌ]
.Script="";}}}}ģ=è.Replace("[","[[").Replace("]","]]");Ĥ=é.Replace("[","[[").Replace("]","]]");ĥ=ê.Replace("[","[[").
Replace("]","]]");Ħ=ë.Replace("[","[[").Replace("]","]]");Ĩ=drone_tag.Replace("[","[[").Replace("]","]]");ĩ=Ʋ.Replace("[","[[")
.Replace("]","]]");ħ=ì.Replace("[","[[").Replace("]","]]");}public void ȸ(){if(ƞ.Count<=0||ƞ[0]==null){Echo(
$"Antenna with tag: '{æ.Replace("[","[[").Replace("]","]]")}' not found.");Ł=!Ł;return;}ź=ƞ[0];if(Ɯ.Count<=0||Ɯ[0]==null){Echo(
$"remote control with tag: '{æ.Replace("[","[[").Replace("]","]]")}' not found.");Ł=!Ł;return;}ż=Ɯ[0];if(Ơ.Count<=0||Ơ[0]==null){Echo(
$"Indicator light with tag: '{ç.Replace("[","[[").Replace("]","]]")}' not found.");Ł=!Ł;return;}Ż=Ơ[0];Ż.SetValue("Color",Ƙ);if(Ƨ.Count<=0||Ƨ[0]==null){Echo($"Interface PB with tag: '{ħ}' not found.");
Ł=!Ł;return;}if(Ƣ.Count<=0||Ƣ[0]==null){Echo($"Display with tag '{è.Replace("[","[[").Replace("]","]]")}' not found");}if
(Ƣ.Count>0&&Ƣ[0]!=null){ƪ=((IMyTextSurfaceProvider)Ƣ[0]).GetSurface(srfM);}if(ƪ==null){Echo(
$"Panel:'{srfM}' on '{è.Replace("[","[[").Replace("]","]]")}' not found");}if(ƣ.Count<=0||ƣ[0]==null){Echo($"Display with tag '{è.Replace("[","[[").Replace("]","]]")}' not found");}if(ƣ.Count>
0&&ƣ[0]!=null){ƫ=((IMyTextSurfaceProvider)ƣ[0]).GetSurface(srfL);}if(ƫ==null){Echo(
$"Panel:'{srfL}' on '{ê.Replace("[","[[").Replace("]","]]")}' not found");}if(Ƥ.Count<=0||Ƥ[0]==null){Echo($"Display with tag '{ê.Replace("[","[[").Replace("]","]]")}' not found");}if(Ƥ.Count>
0&&Ƥ[0]!=null){Ʃ=((IMyTextSurfaceProvider)Ƥ[0]).GetSurface(srfD);}if(ƪ==null){Echo(
$"Panel:'{srfD}' on '{é.Replace("[","[[").Replace("]","]]")}' not found");}if(Ƨ.Count<=0||Ƨ[0]==null){Echo($"Interface PB with tag: '{ì.Replace("[","[[").Replace("]","]]")}' not found.");}if(ƥ
.Count>0&&ƥ[0]!=null){Ƭ=((IMyTextSurfaceProvider)ƥ[0]).GetSurface(srfV);if(Ƭ.ContentType!=ContentType.SCRIPT){Echo(
"Correcting vis");Ƭ.ContentType=ContentType.SCRIPT;Ƭ.Script="";ř=true;Ʈ=new RectangleF((Ƭ.TextureSize-Ƭ.SurfaceSize)/2f,Ƭ.SurfaceSize);}
}if(Ƭ==null){Echo($"Panel:'{srfV}' on '{ë.Replace("[","[[").Replace("]","]]")}' not found");}if(ƥ.Count<=0||ƥ[0]==null){
Echo($"Display with tag '{ë.Replace("[","[[").Replace("]","]]")}' not found");ř=false;}}public void ȴ(){ǁ ȉ;string ζ=Ä.Trim(
);if(!ǂ.ContainsKey(ζ)){ǁ η=new ǁ();η.ȕ=ζ;η.ɒ=Å;η.Ȝ=Æ;η.Ȑ=Ç;η.ȑ=È;η.Ȕ=É;η.θ=Ê;η.Ȏ=-1;η.Ƞ=ż.GetPosition();η.ʀ=Ë;η.ʂ=Ì;η.ʁ=
Í;η.ʄ=Ø;η.ʅ=Ù;η.ʆ=Ú;η.ɽ=Û;η.ɾ=Ü;η.ɿ=Ý;η.Ȓ=false;η.ȏ=false;η.Ȗ=0;η.ȍ=0;η.Ȣ="";η.ȓ=false;η.ș=true;η.ȣ=0.0;η.ɺ=true;η.ȥ=true
;η.Ȍ=false;η.Ȋ=false;η.ʈ=0;η.Ή=Ï;η.Ί=Ð;η.ι=Õ;η.κ=Ñ;η.λ=Ò;η.ʃ=L;η.ɼ=Ó;η.ɍ=Ô;η.ȗ=new List<IMyDoor>();μ(ζ,η.ȗ);Ɣ.Add(η.ȕ);ǂ.
Add(ζ,η);}else if(ǂ.TryGetValue(ζ,out ȉ)){ȉ.ɒ=Å;ȉ.Ȝ=Æ;ȉ.Ȑ=Ç;ȉ.ȑ=È;ȉ.Ȕ=É;ȉ.θ=Ê;ȉ.Ȏ=M;ȉ.ʀ=Ë;ȉ.ʂ=Ì;ȉ.ʁ=Í;ȉ.ʄ=Ø;ȉ.ʅ=Ù;ȉ.ʆ=Ú;ȉ.ɽ
=Û;ȉ.ɾ=Ü;ȉ.ɿ=Ý;ȉ.ȣ=š;ȉ.ȥ=true;ȉ.Ή=Ï;ȉ.Ί=Ð;ȉ.ι=Õ;ȉ.κ=Ñ;ȉ.λ=Ò;ȉ.ʃ=L;ȉ.ɼ=Ó;ȉ.ɍ=Ô;if(ȉ.ȣ<=Š||ȉ.ɍ=="True"){ȉ.ɺ=false;}if(ȉ.ȣ>Š
&&ȉ.ɍ!="True"){ȉ.ɺ=true;}ŗ=true;ġ=ζ;}}void Ǵ(){Ɔ.Clear().EnsureCapacity(512);Ɔ.Append("GMDC ").Append(w).Append(" ").
Append(Ʋ).Append(" [").Append(drone_tag).Append("] [").Append(Ğ).Append("] Running ").Append(Ę).Append(" ").AppendLine();Ɔ.
AppendLine("------------------------------\n");Ɔ.Append("Total drones detected: ").Append(ǂ.Count).AppendLine();Ɔ.Append(
"Drones active: ").Append(Q).Append(" - Fault: ").Append(k);if(İ){Ɔ.Append(" (Max: ").Append(K).Append(" (").Append(F).Append(
")) Hard limit: ").Append(G);}Ɔ.AppendLine();Ɔ.Append("Docking: ").Append(S).Append(" Docked: ").Append(T).Append(" - Unload: ").Append(X
).Append(" Recharge: ").Append(W).Append(" Idle: ").Append(Z).AppendLine("  ");Ɔ.Append("Undocking: ").Append(U).Append(
" Undocked: ").Append(V).Append(" - Idle: ").Append(Y).Append(" Nav: ").Append(c).Append(" Mining: ").Append(b).Append(" Exit: ").
Append(a).AppendLine();Ɔ.Append("Drone AI Faults: ").Append(m).AppendLine();Ɔ.AppendLine();Ɔ.Append("Surface distance: ").
Append(ţ).AppendLine("m");Ɔ.Append("Drill depth: ").Append(Ţ).Append("m (").Append(Ţ+ţ).AppendLine("m)");Ɔ.Append(
"Req. ignore depth: ").Append(Ů).Append("m (Drone length: ").Append(drone_length).AppendLine("m)");Ɔ.Append("Ignore depth: ").Append(ţ+
drone_length-drone_clear_offset+Ů).Append("m (Drill Start: ").Append((Ţ+ţ)-(Ů+ţ+drone_length-drone_clear_offset)).AppendLine("m)\n")
;Ɔ.Append("Command: ").Append(í).Append(" Reset: ").Append(ŭ).AppendLine();Ɔ.Append("Status: ").Append(č).AppendLine("\n"
);Ɔ.Append("Target Coordinates [").Append(Ğ).AppendLine("]:");Ɔ.AppendLine(ž.ToString());if(ľ||Ŀ){Ɔ.Append(
"Align Coordinates:\n").AppendLine(ƀ.ToString());}if(Ǉ.Count>0){for(int ǌ=0;ǌ<Ǉ.Count;ǌ++){if(Ǉ[ǌ]!=null){Ǉ[ǌ].WriteText(Ɔ);}}}}public void ǘ(
){ν(ƛ,Ɯ);ν(Ɲ,ƞ);ν(Ɵ,Ơ);ν(ơ,Ƣ,ƣ,Ƥ,ƥ);ν(Ʀ,Ƨ);ν(ƒ,Ɠ);ξ(Ƒ,Ə,Ɛ);ξ(Ǝ);ξ(Ʒ);ξ(Ƶ,ƶ);if(ƃ?.Length>0)ƃ.Clear();if(Ɔ?.Length>0)Ɔ.
Clear();if(Ƈ?.Length>0)Ƈ.Clear();if(ƅ?.Length>0)ƅ.Clear();if(Ƅ?.Length>0)Ƅ.Clear();if(ƈ?.Length>0)ƈ.Clear();if(Ɗ?.Length>0)Ɗ.
Clear();}void ν<ο>(params List<ο>[]π)where ο:class{foreach(var Α in π){if(Α?.Count>0){Α.Clear();}}}void ξ<ο>(params List<ο>[]
π){foreach(var Α in π){if(Α?.Count>0){Α.Clear();}}}public void ǫ(string ρ,IMyTerminalBlock Ǎ,string ς="GMDCJobData",
string σ="Jobinfo"){var τ=new MyIni();τ.Clear();if(τ.TryParse(Ǎ.CustomData.ToString())){τ.Set(ς,σ,ρ);}else{τ.Set(ς,σ,ρ);;}Ǎ.
CustomData=τ.ToString();τ.Clear();ƌ.AppendLine($"Raw input stored successfully in [{ς}] {σ}.");}public void μ(string υ,List<
IMyDoor>φ){φ.Clear();GridTerminalSystem.GetBlocksOfType<IMyDoor>(φ,ʙ=>ʙ.CubeGrid==ƭ&&ʙ.CustomName.Contains(υ));if(φ.Count>0){
for(int ǌ=0;ǌ<φ.Count;ǌ++){if(φ[ǌ]!=null){if(!φ[ǌ].CustomName.Contains("GMDC")){ƍ.Clear();ƍ.Append(φ[ǌ].CustomName).Append(
$" [GMDC] {ǌ}");φ[ǌ].CustomName=ƍ.ToString();ƍ.Clear();}}}}}public void β(){ƨ.Clear();GridTerminalSystem.GetBlocksOfType<IMyDoor>(ƨ,ʙ
=>ʙ.CubeGrid==ƭ);foreach(var ȉ in ǂ.Values){ȉ.ȗ.Clear();for(int ǌ=0;ǌ<ƨ.Count;ǌ++){var χ=ƨ[ǌ];if(χ.CustomName.IndexOf(ȉ.ȕ,
StringComparison.OrdinalIgnoreCase)>=0){ȉ.ȗ.Add(χ);}}}ƨ.Clear();}void ȝ(ǁ ȉ,string ψ){Ŷ=f.ToString();Ć=ψ;ȧ(Ŷ,Ă,ă,Ą,ą,Ć,ć,Ĉ,ĉ,Ċ,ċ);ȉ.Ȣ=Ƅ.
ToString();if(ķ&&ȉ.ȥ){Ȩ(ȉ);}ȉ.ȥ=false;}void Ȥ(ǁ ȉ){if(ȉ.ȗ.Count==0)return;ȉ.ȡ=true;for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){var ω=ȉ.ȗ[Ș];if(
ω!=null){if(!ω.Enabled)ω.Enabled=true;if(ω.Status==DoorStatus.Closed||ω.Status==DoorStatus.Closing){ω.OpenDoor();}if(ω.
Status!=DoorStatus.Open){ȉ.ȡ=false;}}}}void Ȧ(ǁ ȉ){if(ȉ.ȗ.Count==0)return;for(int Ș=0;Ș<ȉ.ȗ.Count;Ș++){var ω=ȉ.ȗ[Ș];if(ω!=null
){if(!ω.Enabled)ω.Enabled=true;if(ω.Status==DoorStatus.Open||ω.Status==DoorStatus.Opening){if(!Ļ){ω.CloseDoor();}}}}}
public class ǁ{public string ȕ,ɒ,Ȝ,Ȑ,ȑ,Ȕ,θ,κ,λ,ι,ʄ,ʅ,ʆ,ʀ,ʂ,ʁ,ɽ,ɾ,ɿ,Ή,Ί,ɼ,ɍ,Ȣ;public int Ȏ,Ȗ,ȍ,ʈ,ʃ;public Vector3D Ƞ;public
bool Ȓ,ȏ,ȓ,ș,ɺ,ȥ,Ȍ,Ȋ,ȡ;public double ȣ;public List<IMyDoor>ȗ=new List<IMyDoor>();}
