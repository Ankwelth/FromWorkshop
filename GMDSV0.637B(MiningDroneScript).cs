// R e a d m e
// -----------
// 
// General Mining Drone Script v0.637B
// Adomus o7 o7 o7
// Adjustables below here:
// 
    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
        A(Storage, Me.CustomData);
    }
    //rename these for drone
    int drone_id_num = 1;
    string droneTag = "SWRM_D";

    //ore detection
    bool cargoSenseEnabled = true;
    float cargoSenseLimit = 0.0f;
    //dmg detect
    bool damageReportingEnabled = true;
    //collision sense ranges
    bool collisionSenseEnabled = true;
    float s_llm = 4.0f;
    float s_rlm = 4.0f;
    float s_btlm = 5.0f;
    float s_tlm = 3.0f;
    float s_bklm = 6.5f;
    float s_flm = 3.0f;
    //hydrogen recharge
    bool ignore_Htank = true;
    double gas_CHGhi = 100.0;
    double gas_CHGlow = 30.0;
    //battery recharge
    bool autoChargeMode = true;
    float bat_CHGhi = 100.0f;
    float bat_CHGlow = 30.0f;
    //drone nav settings
    float drill_speed = 1.0f;
    float nav_speed = 5.0f;
    float exit_speed = 1.0f;
    double nav_inst_thr = 0.05;
    double currentSpeedNotMovingThreshold = 0.1;
    //drone mining settings
    double drillSetLength = 100.0;
    double drill_el = 20.0;
    double req_dist = 1.0;
    double nav_prec = 0.5;
    double nav_prec2 = 1.2;
    double mine_prec = 0.5;
    double terrainclearoffset = 9.0;



    //statics
    bool udock_conf = true;
    bool skip_prec_mode = true;
    string Dock = "Dock";
    string Undock = "UnDock";
    string TON = "TON";
    string TOFF = "TOFF";
    string Reset = "Reset";
    string CA = "CA";
    string PrecM = "PrecM";
    string HT = "HT";
    string Sense = "Sense";
    string dmg = "Dmg";
    string thrusters = "Thrusters";
    string debug = "Debug";
    string display = "Info";
    string autodockCommand = "autodock";
    string collisionSenseCommand = "collision";
    string cargoSenseCommand = "cargo";
    string manualAssignCommand = "manual";
    string terrainClearCommand = "keepterrain";
    string exitDistanceCommand = "exitdistance";
    string exitSpeedCommand = "exitspeed";
    string navSpeedCommand = "navspeed";
    string drillSpeedCommand = "drillspeed";
    string terrainDistanceCommand = "terraincleardistance";


    string B="V0.637B";int C=5;int D=5;int E=120,F=360,G=0,H=0,I=0,J=0,K=0,L=0,M=0,N=0,O=0,P=0,Q=0,R=0,S=0,T=0,U,V=0,W=0
;double X=16.666,Y=0.0,Z=0.02,a=0.0,b=0.0,c=0.0,d,e,f,g=0,h=0.0,i=0.0,j=0.0,k=0.0,l=0.0,m,n=0.0,o;string p="",q="",r="",s
="",t="",u="",v="",w="",x="",y="",z="",ª="",µ="",º="",À="",Á="ping",Â="sync",Ã="OK",Ä="Idle",Å="recall",Æ="",Ç,È="",É="",
Ê="",Ë,Ì="",Í="",Î="",Ï="",Ð="",Ñ="ActivateBehavior",Ò="ActivateBehavior_Off",Ó="ActivateBehavior_On",Ô=
"ID_PLAY_CHECKBOX_On",Õ="ID_PLAY_CHECKBOX_Off",Ö="",Ø="Remote Control",Ù="Sensor",Ú="Thruster",Û="Atmospheric",Ü="Hydrogen",Ý="Ion",Þ=
"Prototech",ß="Antenna",à="Beacon",á="Camera",â="Connector",ã="Battery",ä="Hydrogen Tank",å="Oxygen Tank",æ="Drill",ç="Gyroscope",è
="Timer Block",é="AI Flight Move",ê="AI Task Recorder",ë="Indication Light",ì="Cargo Container",í="Display Panel",î=
"Event Controller",ï="---:-1:0:0:0:0:0:0:0:",ð,ñ="",ò="GMDSJobData",ó="Jobinfo",ô="",õ,ö="",ø="",ù="",ú="",û="",ü="",ý="",þ="",ÿ="",Ā="";
float ā=2,Ă=0.0f;bool ă=false,Ą=false,ą=false,Ć=false,ć=false,Ĉ=false,ĉ=false,Ċ=false,ċ=false,Č=true,č=false,Ď=false,ď=false,
Đ=false,đ=false,Ē=false,ē=false,Ĕ=false,ĕ=false,Ė=false,ė=false,Ę=false,ę=false,Ě=false,ě=false,Ĝ=false,ĝ=false,Ğ=false,ğ
=false,Ġ=false,ġ=false,Ģ=false,ģ=false,Ĥ=false,ĥ=false,Ħ=false,ħ=false,Ĩ=false,ĩ=false,Ī=false,ī=false,Ĭ=false,ĭ=false,Į=
false,į=false,İ=false,ı=false,Ĳ=false,ĳ=false,Ĵ=false,ĵ=false,Ķ=false,ķ=false,ĸ=false,Ĺ=false,ĺ=false,Ļ=false,ļ=false,Ľ=false
,ľ=false,Ŀ=false,ŀ=false,Ł=false,ł=false,Ń=false,ń=false,Ņ=false,ņ=false,Ň=false,ň=false,ŉ=false,Ŋ=false,ŋ=false,Ō=false,
ō=false,Ŏ=false,ŏ=false,Ő=false,ő=false,Œ=false,œ=false,Ŕ=false,ŕ=false;double Ŗ=0.0;string ŗ;float Ř=0.0f;float ř;float
Ś;bool ś=false;double Ŝ;string ŝ="";IMyRemoteControl Ş;IMyShipConnector ş;IMyRadioAntenna Š;IMyTimerBlock š,Ţ;
IMyPathRecorderBlock ţ,Ť;IMyFlightMovementBlock ť;IMyBatteryBlock Ŧ;IMyLightingBlock ŧ,Ũ,ũ,Ū,ū,Ŭ;IMySensorBlock ŭ;IMyGasTank Ů;Vector3D ů,Ű,
ű,Ų,ų,Ŵ,ŵ,Ŷ,ŷ,Ÿ,Ź,ź,Ż;Vector3D ż;string Ž="ID_PLAY_CHECKBOX";List<IMyRemoteControl>ž=new List<IMyRemoteControl>(),ſ=new
List<IMyRemoteControl>();List<IMySensorBlock>ƀ=new List<IMySensorBlock>(),Ɓ=new List<IMySensorBlock>();List<IMyCameraBlock>Ƃ
=new List<IMyCameraBlock>(),ƃ=new List<IMyCameraBlock>();List<IMyShipConnector>Ƅ=new List<IMyShipConnector>(),ƅ=new List<
IMyShipConnector>();List<IMyCargoContainer>Ɔ=new List<IMyCargoContainer>();List<IMyCargoContainer>Ƈ=new List<IMyCargoContainer>(),ƈ=new
List<IMyCargoContainer>();List<IMyRadioAntenna>Ɖ=new List<IMyRadioAntenna>(),Ɗ=new List<IMyRadioAntenna>();List<IMyBeacon>Ƌ=
new List<IMyBeacon>(),ƌ=new List<IMyBeacon>();List<IMyPathRecorderBlock>ƍ=new List<IMyPathRecorderBlock>(),Ǝ=new List<
IMyPathRecorderBlock>(),Ə=new List<IMyPathRecorderBlock>();List<IMyFlightMovementBlock>Ɛ=new List<IMyFlightMovementBlock>(),Ƒ=new List<
IMyFlightMovementBlock>();List<IMyTimerBlock>ƒ=new List<IMyTimerBlock>(),Ɠ=new List<IMyTimerBlock>(),Ɣ=new List<IMyTimerBlock>(),ƕ=new List<
IMyTimerBlock>(),Ɩ=new List<IMyTimerBlock>();List<IMyLightingBlock>Ɨ=new List<IMyLightingBlock>(),Ƙ=new List<IMyLightingBlock>(),ƙ=
new List<IMyLightingBlock>(),ƚ=new List<IMyLightingBlock>(),ƛ=new List<IMyLightingBlock>(),Ɯ=new List<IMyLightingBlock>(),Ɲ
=new List<IMyLightingBlock>();List<IMyBatteryBlock>ƞ=new List<IMyBatteryBlock>(),Ɵ=new List<IMyBatteryBlock>();List<
IMyGasTank>Ơ=new List<IMyGasTank>(),ơ=new List<IMyGasTank>();List<IMyShipDrill>Ƣ=new List<IMyShipDrill>(),ƣ=new List<IMyShipDrill>
();List<MyWaypointInfo>Ƥ=new List<MyWaypointInfo>();List<IMyThrust>ƥ=new List<IMyThrust>(),Ʀ=new List<IMyThrust>();List<
IMyTerminalBlock>Ƨ=new List<IMyTerminalBlock>(),ƨ=new List<IMyTerminalBlock>(),Ʃ=new List<IMyTerminalBlock>();List<IMyProjector>ƪ=new
List<IMyProjector>();List<IMyEventControllerBlock>ƫ=new List<IMyEventControllerBlock>();List<MyIGCMessage>Ƭ=new List<
MyIGCMessage>();IMyBlockGroup ƭ,Ʈ,Ư,ư;List<IMyGyro>Ʊ=new List<IMyGyro>();List<IMyGyro>Ʋ=new List<IMyGyro>();IMyTextSurface Ƴ;
StringBuilder ƴ=new StringBuilder(),Ƶ=new StringBuilder();MyIni ƶ=new MyIni(),Ʒ=new MyIni();IMyBroadcastListener Ƹ,ƹ,ƺ,ƻ,Ƽ;
MyIGCMessage ƽ,ƾ,ƿ,ǀ;MyIni ǁ=new MyIni();string ǂ="";public void
 Save
(){ƶ.Clear();ƶ.Set("configuration","runargument",ñ);ƶ.Set("configuration","secondary tag",Æ);ƶ.Set("commands","c1",ĩ);ƶ.
Set("commands","c2",Č);ƶ.Set("commands","c3",ė);ƶ.Set("commands","c4",Ď);ƶ.Set("commands","c5",č);ƶ.Set("commands","c6",ď);
ƶ.Set("commands","c7",Ą);ƶ.Set("dockmode","d1",S);ƶ.Set("dockmode","d2",Đ);ƶ.Set("dockmode","d3",K);ƶ.Set("dockmode","d4"
,T);ƶ.Set("unitstate","u1",Ė);ƶ.Set("unitstate","u2",Ļ);ƶ.Set("unitstate","u3",Q);ƶ.Set("unitstate","u4",Į);ƶ.Set(
"unitstate","u5",ĭ);ƶ.Set("unitstate","u6",İ);ƶ.Set("unitstate","u7",R);ƶ.Set("unitstate","u8",Ķ);ƶ.Set("unitstate","u9",ĵ);ƶ.Set(
"unitstate","u10",Ĵ);ƶ.Set("unitstate","u11",ĝ);ƶ.Set("unitstate","u12",ĳ);ƶ.Set("unitstate","u13",ĉ);ƶ.Set("unitstate","u14",Ċ);ƶ.
Set("unitstate","u15",Ĳ);ƶ.Set("unitstate","u16",ı);ƶ.Set("unitstate","u17",ġ);ƶ.Set("unitstate","u18",ķ);ƶ.Set("unitstate"
,"u19",ĸ);ƶ.Set("unitstate","u20",Ĺ);ƶ.Set("unitstate","u21",ĺ);ƶ.Set("unitstate","u22",g);ƶ.Set("coordinates","co1",Ű.
ToString().Trim());ƶ.Set("coordinates","co2",ű.ToString().Trim());ƶ.Set("coordinates","co3",Ų.ToString().Trim());ƶ.Set(
"coordinates","co4",ų.ToString().Trim());ƶ.Set("coordinates","co5",Ŵ.ToString().Trim());ƶ.Set("coordinates","co6",ŵ.ToString().Trim()
);ƶ.Set("coordinates","co7",ů.ToString().Trim());ƶ.Set("coordinates","co8",ż.ToString().Trim());ƶ.Set("coordinates","co9"
,Ŷ.ToString().Trim());ƶ.Set("coordinates","co10",ŷ.ToString().Trim());ƶ.Set("coordinates","co11",Ÿ.ToString().Trim());ƶ.
Set("coordinates","co12",Ź.ToString().Trim());ƶ.Set("coordinates","co13",ź.ToString().Trim());ƶ.Set("coordinates","co14",ŝ.
ToString().Trim());ƶ.Set("coordinates","drillterrainenabled",ŏ.ToString().Trim());ƶ.Set("coordinates","drillterrainoffset",
terrainclearoffset.ToString().Trim());ƶ.Set("coordinates","keepterrain",Ő.ToString().Trim());if(Ġ){ƶ.Set(ò,ó,ô);}else{ƶ.Set(ò,ó,ï);}
Storage=ƶ.ToString();ƶ.Clear();}void ǆ(string ǃ,string Ǆ){if(string.IsNullOrEmpty(ǃ)||string.IsNullOrWhiteSpace(ǃ)){Echo(
"No Storage data found.");}ƶ.Clear();if(ƶ.TryParse(ǃ)){var ǅ="";ǅ=ƶ.Get("configuration","runargument").ToString().Trim();ñ=ǅ;ǅ=ƶ.Get(
"configuration","secondary tag").ToString();Æ=ǅ;ǅ=ƶ.Get("commands","c1").ToString().Trim();bool.TryParse(ǅ,out ĩ);ǅ=ƶ.Get("commands",
"c2").ToString().Trim();bool.TryParse(ǅ,out Č);ǅ=ƶ.Get("commands","c3").ToString().Trim();bool.TryParse(ǅ,out ė);ǅ=ƶ.Get(
"commands","c4").ToString().Trim();bool.TryParse(ǅ,out Ď);ǅ=ƶ.Get("commands","c5").ToString().Trim();bool.TryParse(ǅ,out č);ǅ=ƶ.
Get("commands","c6").ToString().Trim();bool.TryParse(ǅ,out ď);ǅ=ƶ.Get("commands","c7").ToString().Trim();bool.TryParse(ǅ,
out Ą);ǅ=ƶ.Get("dockmode","d1").ToString().Trim();int.TryParse(ǅ,out S);ǅ=ƶ.Get("dockmode","d2").ToString().Trim();bool.
TryParse(ǅ,out Đ);ǅ=ƶ.Get("dockmode","d3").ToString().Trim();int.TryParse(ǅ,out K);ǅ=ƶ.Get("dockmode","d4").ToString().Trim();
int.TryParse(ǅ,out T);ǅ=ƶ.Get("unitstate","u1").ToString().Trim();bool.TryParse(ǅ,out Ė);ǅ=ƶ.Get("unitstate","u2").ToString
().Trim();bool.TryParse(ǅ,out Ļ);ǅ=ƶ.Get("unitstate","u3").ToString().Trim();int.TryParse(ǅ,out Q);ǅ=ƶ.Get("unitstate",
"u4").ToString().Trim();bool.TryParse(ǅ,out Į);ǅ=ƶ.Get("unitstate","u5").ToString().Trim();bool.TryParse(ǅ,out ĭ);ǅ=ƶ.Get(
"unitstate","u6").ToString().Trim();bool.TryParse(ǅ,out İ);ǅ=ƶ.Get("unitstate","u7").ToString().Trim();int.TryParse(ǅ,out R);ǅ=ƶ.
Get("unitstate","u8").ToString().Trim();bool.TryParse(ǅ,out Ķ);ǅ=ƶ.Get("unitstate","u9").ToString().Trim();bool.TryParse(ǅ,
out ĵ);ǅ=ƶ.Get("unitstate","u10").ToString().Trim();bool.TryParse(ǅ,out Ĵ);ǅ=ƶ.Get("unitstate","u11").ToString().Trim();
bool.TryParse(ǅ,out ĝ);ǅ=ƶ.Get("unitstate","u12").ToString().Trim();bool.TryParse(ǅ,out ĳ);ǅ=ƶ.Get("unitstate","u13").
ToString().Trim();bool.TryParse(ǅ,out ĉ);ǅ=ƶ.Get("unitstate","u14").ToString().Trim();bool.TryParse(ǅ,out Ĳ);ǅ=ƶ.Get("unitstate"
,"u15").ToString().Trim();bool.TryParse(ǅ,out ı);ǅ=ƶ.Get("unitstate","u16").ToString().Trim();bool.TryParse(ǅ,out ġ);ǅ=ƶ.
Get("unitstate","u18").ToString().Trim();bool.TryParse(ǅ,out ķ);ǅ=ƶ.Get("unitstate","u19").ToString().Trim();bool.TryParse(
ǅ,out ĸ);ǅ=ƶ.Get("unitstate","u20").ToString().Trim();bool.TryParse(ǅ,out Ĺ);ǅ=ƶ.Get("unitstate","u21").ToString().Trim()
;bool.TryParse(ǅ,out ĺ);ǅ=ƶ.Get("unitstate","u22").ToString().Trim();double.TryParse(ǅ,out g);ǅ=ƶ.Get("coordinates","co1"
).ToString().Trim();Vector3D.TryParse(ǅ,out Ű);ǅ=ƶ.Get("coordinates","co2").ToString().Trim();Vector3D.TryParse(ǅ,out ű);
ǅ=ƶ.Get("coordinates","co3").ToString().Trim();Vector3D.TryParse(ǅ,out Ų);ǅ=ƶ.Get("coordinates","co4").ToString().Trim();
Vector3D.TryParse(ǅ,out ų);ǅ=ƶ.Get("coordinates","co5").ToString().Trim();Vector3D.TryParse(ǅ,out Ŵ);ǅ=ƶ.Get("coordinates","co6"
).ToString().Trim();Vector3D.TryParse(ǅ,out ŵ);ǅ=ƶ.Get("coordinates","co7").ToString().Trim();Vector3D.TryParse(ǅ,out ů);
ǅ=ƶ.Get("coordinates","co8").ToString().Trim();Vector3D.TryParse(ǅ,out ż);ǅ=ƶ.Get("coordinates","co9").ToString().Trim();
Vector3D.TryParse(ǅ,out Ŷ);ǅ=ƶ.Get("coordinates","co10").ToString().Trim();Vector3D.TryParse(ǅ,out ŷ);ǅ=ƶ.Get("coordinates",
"co11").ToString().Trim();Vector3D.TryParse(ǅ,out Ÿ);ǅ=ƶ.Get("coordinates","co12").ToString().Trim();Vector3D.TryParse(ǅ,out Ź
);ǅ=ƶ.Get("coordinates","co13").ToString().Trim();Vector3D.TryParse(ǅ,out ź);ǅ=ƶ.Get("coordinates","co14").ToString().
Trim();ŝ=ǅ;ǅ=ƶ.Get("coordinates","drillterrainenabled").ToString().Trim();if(!bool.TryParse(ǅ,out ŏ)){ŏ=false;}ǅ=ƶ.Get(
"coordinates","keepterrain").ToString().Trim();if(!bool.TryParse(ǅ,out Ő)){Ő=false;}ǅ=ƶ.Get("coordinates","drillterrainoffset").
ToString().Trim();if(!double.TryParse(ǅ,out terrainclearoffset)){terrainclearoffset=9.0;}ǅ=ƶ.Get(ò,ó).ToString().Trim();if(!
string.IsNullOrWhiteSpace(ǅ)){ô=ǅ;}}}public void
 Main
(string Ǉ){o=Runtime.LastRunTimeMs;U=Runtime.CurrentInstructionCount;if(!string.IsNullOrEmpty(Ǉ)&&!string.
IsNullOrWhiteSpace(Ǉ)){ñ=Ǉ;ǈ(Ǉ);ļ=false;}if(!ļ){ǉ();Ǌ();ļ=true;ǋ();if(!ļ){Echo("Setup not complete.");return;}ǌ();if(!ļ){Echo(
"Setup not complete.");return;}Echo("Setup complete!");Save();}Ƶ.Append("GMDS ").Append(B).AppendLine(" Running...");bool Ǎ=(ď);ǎ();ǋ();ǌ();
if(!ļ){Echo("Setup not complete.");return;}if(V%6==0){Ǐ();ǐ();}if(V%6==2){Ǒ();}if(V%6==4){ǒ();}V++;if(V>60){V=0;}Ǔ();ǔ();Ǖ
();ǖ();Ǘ(ô);ǘ();Ǚ();ǚ(Ŀ);Ǜ();ǜ();ǝ();Ǟ();ǟ();Ǡ();ǡ();Ǣ();ǣ();if(Ď){Ǥ();}if(č||ė){ǥ(Ŋ);}Ǧ(Ǎ,Ŋ);ǧ(Ŀ);Ǩ();ǩ(Ŋ,Ş,Š,Ŀ);Ǫ();ǫ()
;Ǭ();ǭ(G);Ǯ();ǯ();}void ǌ(){if(!ļ){Ƶ.AppendLine("Drone parts missing - exiting");ǉ();return;}}void ǎ(){if(Ƥ.Count>0){Ƥ.
Clear();}}Vector3D ǻ(Vector3D ǰ){if(Ş==null){Ƶ.AppendLine("No RC found");return Vector3D.Zero;}Vector3D Ǳ=Ş.GetPosition();
Vector3D ǲ=Ş.WorldMatrix.Forward;Vector3D ǳ=Ş.WorldMatrix.Up;Vector3D Ǵ=Ş.WorldMatrix.Right;Vector3D ǵ=-ǳ;Vector3D Ƕ;if(ą){Ƕ=
Vector3D.Normalize(ǰ-Ǳ);}else{Ƕ=Ş.GetNaturalGravity();if(Ƕ.LengthSquared()>0)Ƕ=Vector3D.Normalize(Ƕ);else return Vector3D.Zero;}
Vector3D Ƿ=Vector3D.Cross(ǵ,Ƕ);double Ǹ=Vector3D.Dot(Ƿ,Ǵ);double ǹ=Vector3D.Dot(Ƿ,ǲ);double Ǻ=Vector3D.Dot(Ƿ,ǳ);return new
Vector3D(Ǻ,ǹ,Ǹ);}void Ǖ(){Y=(Z*drillSetLength)+0.6;}void ȁ(bool Ǽ,Vector3 ǽ,bool Ǿ,float ǿ=1.0f){if(Ʋ.Count>0){for(int Ȁ=0;Ȁ<Ʋ.
Count;Ȁ++){if(Ʋ[Ȁ]==null){Ƶ.AppendLine("Gyro [{i}] not found resetting setup flag");ļ=false;}if(Ʋ[Ȁ]!=null){if(Ǿ&&Ʋ[Ȁ].
Enabled){Ʋ[Ȁ].Enabled=false;}else if(!Ǿ&&!Ʋ[Ȁ].Enabled&&Ʋ[Ȁ].IsFunctional){Ʋ[Ȁ].Enabled=true;}if(!Ʋ[Ȁ].GyroOverride&&Ǽ&&Ʋ[Ȁ].
IsFunctional){Ʋ[Ȁ].GyroOverride=true;}else if(Ʋ[Ȁ].GyroOverride&&!Ǽ){Ʋ[Ȁ].GyroOverride=false;}if(Ʋ[Ȁ].GyroPower!=ǿ){Ʋ[Ȁ].GyroPower=ǿ
;}if(Ʋ[Ȁ].Yaw!=ǽ.GetDim(0)){Ʋ[Ȁ].Yaw=ǽ.GetDim(0);}if(Ʋ[Ȁ].Pitch!=ǽ.GetDim(1)){Ʋ[Ȁ].Pitch=ǽ.GetDim(1);}if(Ʋ[Ȁ].Roll!=ǽ.
GetDim(2)){Ʋ[Ȁ].Roll=ǽ.GetDim(2);}}}}}void Ȃ(string ǃ){ǁ.Clear();if(ǁ.TryParse(ǃ)){var ǅ="";ǅ=ǁ.Get(ò,ó).ToString().Trim();ô=ǅ
;}ǁ.Clear();}void Ȏ(string ǃ,IMyTerminalBlock ȃ){if(!string.IsNullOrEmpty(ȃ.CustomData)&&!ȃ.CustomData.Contains(ò)){ǁ.
Clear();if(!ǁ.TryParse(Me.CustomData)){String[]Ȅ=ȃ.CustomData.ToString().Split(':');if(Ȅ.Length>0){ȅ(ȃ.CustomData,ȃ,ò,ó);œ=
true;}Ƶ.AppendLine("Dataconversion");}}if(string.IsNullOrEmpty(ȃ.CustomData)||string.IsNullOrWhiteSpace(ȃ.CustomData)){Ƶ.
AppendLine("Custom Data is empty");return;}Ȃ(ȃ.CustomData);string[]Ȇ=ô.Split(':');int ȇ=Ȇ.Length;if(ȇ<5){Ƶ.AppendLine(
"Custom Data is faulty");ô=ï;return;}ŝ=Ȇ[1];double ȉ=Ȉ(Ȇ,2,0.0);double Ȋ=Ȉ(Ȇ,3,0.0);double ȋ=Ȉ(Ȇ,4,0.0);ů=new Vector3D(ȉ,Ȋ,ȋ);H=Ȍ(Ȇ,6,0);
drillSetLength=Ȉ(Ȇ,7,1.0);Ŗ=Ȉ(Ȇ,8,0.0);Ð=Ȇ[8];a=ȍ(Ȇ,11,out Ī);b=ȍ(Ȇ,12,out ī);c=ȍ(Ȇ,13,out Ĭ);if(Ī&&ī&&Ĭ){ą=true;Ŷ=new Vector3D(a,b,c)
;}else{ą=false;}}void Ȕ(bool ȏ,bool Ȑ,bool ȑ=false){if(ƣ.Count<=0){Ȓ();}if(ƣ.Count>0){for(int ȓ=0;ȓ<ƣ.Count;ȓ++){if(ƣ[ȓ]
==null){ļ=false;}if(ƣ[ȓ]!=null){if(ȏ&&!ƣ[ȓ].Enabled){ƣ[ȓ].Enabled=true;}if(!ȏ&&ƣ[ȓ].Enabled){ƣ[ȓ].Enabled=false;}if(ȑ){if(
!ƣ[ȓ].TerrainClearingMode){ƣ[ȓ].TerrainClearingMode=true;}}else{if(ƣ[ȓ].TerrainClearingMode){ƣ[ȓ].TerrainClearingMode=
false;}}if(Ȑ){if(!ƣ[ȓ].UseConveyorSystem){ƣ[ȓ].UseConveyorSystem=true;}}else{if(ƣ[ȓ].UseConveyorSystem){ƣ[ȓ].
UseConveyorSystem=false;}}}}}}void Ȓ(){Ƣ.Clear();ƣ.Clear();GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(Ƣ,ȕ=>ȕ.CubeGrid==Me.CubeGrid)
;if(Ƣ.Count>0){for(int ȓ=0;ȓ<Ƣ.Count;ȓ++){if(Ƣ[ȓ].CustomName.Contains(p)){Ö=æ+" "+(ȓ+1)+" "+p;Ƣ[ȓ].CustomName=Ö;ƣ.Add(Ƣ[ȓ
]);}if(!Ƣ[ȓ].CustomName.Contains(p)){Ö=æ+" "+(ȓ+1)+" "+p;Ƣ[ȓ].CustomName=Ö;ƣ.Add(Ƣ[ȓ]);}}}Ƣ.Clear();}void Ȗ(){if(ť!=null)
{ť.Enabled=true;ť.ApplyAction(Ò);}if(ţ!=null){ţ.Enabled=true;if(ţ.GetValue<bool>(Ž)){ţ.ApplyAction(Õ);}ţ.ApplyAction(Ò);}
if(Ť!=null){Ť.Enabled=true;if(Ť.GetValue<bool>(Ž)){Ť.ApplyAction(Õ);}Ť.ApplyAction(Ò);}if(ũ!=null){if(ũ.Enabled){ũ.Enabled
=false;}}if(Ū!=null){if(Ū.Enabled){Ū.Enabled=false;}}if(collisionSenseEnabled){if(ŭ!=null){if(ŭ.Enabled){ŭ.Enabled=false;
}}}}void ǈ(string ǃ){if(string.IsNullOrWhiteSpace(ǃ)){Ƶ.AppendLine("No arguments provided, using defaults.");Ŋ=false;
collisionSenseEnabled=true;cargoSenseEnabled=true;ŋ=false;droneTag="UnassignedMiningDroneA";drone_id_num=0;terrainclearoffset=9.0;Ő=false;
drill_el=20.0;exit_speed=1.0f;nav_speed=5.0f;drill_speed=1.0f;return;}string[]ȗ=ǃ.Split(',');if(ȗ.Length==0){Ƶ.AppendLine(
"No arguments provided, using defaults.");return;}if(ȗ.Length>=1&&!string.IsNullOrWhiteSpace(ȗ[0])){droneTag=ȗ[0].Trim();}else{droneTag="UnassignedMiningDroneC"
;}if(ȗ.Length>=2&&!string.IsNullOrWhiteSpace(ȗ[1])){if(!int.TryParse(ȗ[1].Trim(),out drone_id_num)){drone_id_num=0;}}else
{drone_id_num=0;}if(ȗ.Length>2){Ŋ=false;collisionSenseEnabled=false;cargoSenseEnabled=false;ŋ=false;Ő=false;
terrainclearoffset=9.0;for(int ȓ=2;ȓ<ȗ.Length;ȓ++){string Ș=ȗ[ȓ].Trim().ToLower();if(Ș.Contains(autodockCommand)){Ŋ=true;}if(Ș.Contains(
cargoSenseCommand)){cargoSenseEnabled=true;}if(Ș.Contains(collisionSenseCommand)){collisionSenseEnabled=true;}if(Ș.Contains(
manualAssignCommand)){ŋ=true;}if(Ș.Contains(terrainClearCommand)){Ő=true;}if(Ș.Contains(terrainDistanceCommand)){string ș=Ș;if(!string.
IsNullOrWhiteSpace(ș)){String[]Ț=ș.Split(':');if(Ț.Length>0){if(Ț[0].Contains(terrainClearCommand)){if(Ț.Length>1){if(!string.
IsNullOrWhiteSpace(Ț[1])){if(!double.TryParse(Ț[1],out terrainclearoffset)){terrainclearoffset=9.0;}}}}}}}if(Ș.Contains(
exitDistanceCommand)){string ș=Ș;if(!string.IsNullOrWhiteSpace(ș)){String[]ț=ș.Split(':');if(ț.Length>0){if(ț[0].Contains(
exitDistanceCommand)){if(ț.Length>1){if(!string.IsNullOrWhiteSpace(ț[1])){if(!double.TryParse(ț[1],out drill_el)){drill_el=20.0;}}}}}}}if(Ș
.Contains(exitSpeedCommand)){string ș=Ș;if(!string.IsNullOrWhiteSpace(ș)){String[]Ȝ=ș.Split(':');if(Ȝ.Length>0){if(Ȝ[0].
Contains(exitSpeedCommand)){if(Ȝ.Length>1){if(!string.IsNullOrWhiteSpace(Ȝ[1])){if(!float.TryParse(Ȝ[1],out exit_speed)){
exit_speed=1.0f;}}}}}}}if(Ș.Contains(navSpeedCommand)){string ș=Ș;if(!string.IsNullOrWhiteSpace(ș)){String[]ȝ=ș.Split(':');if(ȝ.
Length>0){if(ȝ[0].Contains(navSpeedCommand)){if(ȝ.Length>1){if(!string.IsNullOrWhiteSpace(ȝ[1])){if(!float.TryParse(ȝ[1],out
nav_speed)){nav_speed=1.0f;}}}}}}}if(Ș.Contains(drillSpeedCommand)){string ș=Ș;if(!string.IsNullOrWhiteSpace(ș)){String[]Ȟ=ș.
Split(':');if(Ȟ.Length>0){if(Ȟ[0].Contains(drillSpeedCommand)){if(Ȟ.Length>1){if(!string.IsNullOrWhiteSpace(Ȟ[1])){if(!float.
TryParse(Ȟ[1],out drill_speed)){drill_speed=1.0f;}}}}}}}}}}void ǭ(int ȟ){if(ȟ==0){Ë="Idle";}if(ȟ==1||ȟ==4){Ë=
$"Nav CA {collisionSenseEnabled}";}if(ȟ==2||ȟ==3){Ë="Nav P";}if(ȟ==5){Ë="Navi Dest Reach";}if(ȟ==6){Ë="Mine Calc shaft";}if(ȟ==7){Ë="Mine Start";}if(ȟ==8
){Ë="Mine Calc WP";}if(ȟ==9){Ë="Mine Add WP";}if(ȟ==10){Ë="Mine to WP";}if(ȟ==11){Ë="Mine En AP";}if(ȟ==12){Ë=
"Mine WP reach";}if(ȟ==13){Ë="Mine Trunc";}if(ȟ==14){Ë="Mine Fin";}if(ȟ==15){Ë="Mine new WP";}if(ȟ==16){Ë="Mine Fnshd";}if(ȟ==17){Ë=
"WP mine exit";}if(ȟ==18){Ë="Nav mine exit";}if(ȟ==19){Ë="Mine exit reach";}if(ȟ==20){Ë="Cl WP dock";}if(ȟ==21){Ë="Rtn dock";}if(ȟ==22
){Ë="Rtn unload";}if(ȟ==23){Ë="Stablz";}if(ȟ==24){Ë="Read dt";}if(ȟ==25){Ë="Comp cmd data";}if(ȟ==26){Ë="RTB Ready A";}if
(ȟ==27){Ë="RTB Ready B";}}public void A(string ǃ,string Ǆ){if(!string.IsNullOrWhiteSpace(ǃ)&&!string.IsNullOrEmpty(ǃ)){Ȃ(
Me.CustomData.ToString().Trim());ǆ(ǃ,Ǆ);ǈ(ñ);ȅ(ô,Me,ò,ó);œ=true;Ƶ.AppendLine("Configuration loaded from Storage.");}else{ǈ
(ñ);ȅ(ô,Me,ò,ó);œ=true;Ƶ.AppendLine("No Storage data found, configuration loaded from arguments or defaults.");}}public
void Ƞ(IMySensorBlock ȃ){if(ȃ!=null){string ǃ="";Ƶ.AppendLine("No sensor range data found, using default.");Ʒ.Clear();if(Ʒ.
TryParse(ȃ.CustomData)){var ǅ="";ǅ=Ʒ.Get("SensorRange","LeftExtend").ToString();if(!float.TryParse(ǅ,out s_llm)){s_llm=4.0f;Ʒ.
Set("SensorRange","LeftExtend",s_llm);}ǅ=Ʒ.Get("SensorRange","RightExtend").ToString();if(!float.TryParse(ǅ,out s_rlm)){
s_rlm=4.0f;Ʒ.Set("SensorRange","RightExtend",s_rlm);}ǅ=Ʒ.Get("SensorRange","BottomExtend").ToString();if(!float.TryParse(ǅ,
out s_btlm)){s_btlm=5.0f;Ʒ.Set("SensorRange","BottomExtend",s_btlm);}ǅ=Ʒ.Get("SensorRange","TopExtend").ToString();if(!
float.TryParse(ǅ,out s_tlm)){s_tlm=3.0f;Ʒ.Set("SensorRange","TopExtend",s_tlm);}ǅ=Ʒ.Get("SensorRange","BackExtend").ToString(
);if(!float.TryParse(ǅ,out s_bklm)){s_bklm=6.5f;Ʒ.Set("SensorRange","BackExtend",s_bklm);}ǅ=Ʒ.Get("SensorRange",
"FrontExtend").ToString();if(!float.TryParse(ǅ,out s_flm)){s_flm=3.0f;Ʒ.Set("SensorRange","FrontExtend",s_flm);}}else{Ʒ.Set(
"SensorRange","LeftExtend",s_llm);Ʒ.Set("SensorRange","RightExtend",s_rlm);Ʒ.Set("SensorRange","BottomExtend",s_btlm);Ʒ.Set(
"SensorRange","TopExtend",s_tlm);Ʒ.Set("SensorRange","BackExtend",s_bklm);Ʒ.Set("SensorRange","FrontExtend",s_flm);}ǃ=Ʒ.ToString();ȃ.
CustomData=ǃ;Ʒ.Clear();}}public void ȡ(){ª=$"[{Æ}]";Me.CustomName=$"GMDS Programmable Block {p} {ª}";if(!Me.CubeGrid.CustomName.
Contains(p)){var Ö=Me.CubeGrid.CustomName;Me.CubeGrid.CustomName=Ö+($" - {p}");}if(Š!=null){Š.HudText=$"{p} {ª}";Š.ShowShipName=
true;}if(ƌ.Count>0){if(ƌ[0]!=null){ƌ[0].HudText=$"{p} {ª}";}}}public void Ǌ(){IMyGridTerminalSystem Ȣ=GridTerminalSystem as
IMyGridTerminalSystem;ƴ.Clear();;Ƭ.Clear();if(string.IsNullOrEmpty(droneTag)||string.IsNullOrWhiteSpace(droneTag)){Echo(
$"Invalid name for drone_tag {droneTag.Replace("[","[[").Replace("]","]]")}");return;}Ì=droneTag+" reply";Í=droneTag+" "+Å;p=$"[{droneTag} {drone_id_num}]";q=$"[{droneTag} {drone_id_num}]";r=
$"[{droneTag} {drone_id_num} {Dock}]";s=$"[{droneTag} {drone_id_num} {Undock}]";t="["+droneTag+" "+drone_id_num+" "+TON+"]";u="["+droneTag+" "+drone_id_num+
" "+TOFF+"]";v=$"[{droneTag} {drone_id_num} {Reset}]";w=$"[{droneTag} {drone_id_num} {CA}]";x=
$"[{droneTag} {drone_id_num} {PrecM}]";y=$"[{droneTag} {drone_id_num} {HT}]";z=$"[{droneTag} {drone_id_num} {Sense}]";µ=$"[{droneTag} {drone_id_num} {dmg}]";º
=$"[{droneTag}] {Á}";À=$"{thrusters} [{droneTag} {drone_id_num}]";Î=p+" "+Å;Ï="["+droneTag+"]"+" "+Â;ª=$"[{Æ}]";Ƽ=IGC.
RegisterBroadcastListener(Ï);Me.CustomName=$"GMDS Programmable Block {p} {ª}";if(!Me.CubeGrid.CustomName.Contains(p)){var Ö=Me.CubeGrid.
CustomName;Me.CubeGrid.CustomName=Ö+($" - {p}");}ŀ=false;Ł=false;ł=false;Ń=false;Ɖ.Clear();Ɗ.Clear();Ȣ.GetBlocksOfType<
IMyRadioAntenna>(Ɖ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ɖ.Count>0){for(int ȓ=0;ȓ<Ɖ.Count;ȓ++){if(Ɖ[ȓ].CustomName.Contains(p)){string ȣ=Ɖ[ȓ].
CustomData;if(string.IsNullOrEmpty(droneTag)||string.IsNullOrWhiteSpace(droneTag)){Echo(
$"Invalid name for drone_tag {droneTag.Replace("[","[[").Replace("]","]]")}");return;}Ö=ß+" "+(ȓ+1)+" "+p;Ɖ[ȓ].CustomName=Ö;Ɖ[ȓ].HudText=$"{p} {ª}";Ɖ[ȓ].ShowShipName=true;Ɗ.Add(Ɖ[ȓ]);}if(!Ɖ[ȓ].
CustomName.Contains(p)){string ȣ=Ɖ[ȓ].CustomData;if(string.IsNullOrEmpty(droneTag)||string.IsNullOrWhiteSpace(droneTag)){Echo(
$"Invalid name for drone_tag {droneTag.Replace("[","[[").Replace("]","]]")}");return;}Ö=ß+" "+(ȓ+1)+" "+p;Ɖ[ȓ].CustomName=Ö;Ɖ[ȓ].HudText=$"{p} {ª}";Ɖ[ȓ].ShowShipName=true;Ɗ.Add(Ɖ[ȓ]);}}}Ɖ.Clear();
ž.Clear();ſ.Clear();Ȣ.GetBlocksOfType<IMyRemoteControl>(ž,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(ž.Count>0){for(int ȓ=0;ȓ<ž.Count
;ȓ++){if(ž[ȓ].CustomName.Contains(p)){Ö=Ø+" "+(ȓ+1)+" "+p;ž[ȓ].CustomName=Ö;ſ.Add(ž[ȓ]);}if(!ž[ȓ].CustomName.Contains(p))
{Ö=Ø+" "+(ȓ+1)+" "+p;ž[ȓ].CustomName=Ö;ſ.Add(ž[ȓ]);}}}ž.Clear();Ƌ.Clear();ƌ.Clear();Ȣ.GetBlocksOfType<IMyBeacon>(Ƌ,ȕ=>ȕ.
CubeGrid==Me.CubeGrid);if(Ƌ.Count>0){for(int ȓ=0;ȓ<Ƌ.Count;ȓ++){if(Ƌ[ȓ].CustomName.Contains(p)){Ö=à+" "+(ȓ+1)+" "+p;Ƌ[ȓ].
CustomName=Ö;Ƌ[ȓ].HudText=$"{p} {ª}";ƌ.Add(Ƌ[ȓ]);}if(!Ƌ[ȓ].CustomName.Contains(p)){Ö=à+" "+(ȓ+1)+" "+p;Ƌ[ȓ].CustomName=Ö;Ƌ[ȓ].
HudText=$"{p} {ª}";ƌ.Add(Ƌ[ȓ]);}}}Ƌ.Clear();if(collisionSenseEnabled){ƀ.Clear();Ɓ.Clear();Ȣ.GetBlocksOfType<IMySensorBlock>(ƀ,ȕ
=>ȕ.CubeGrid==Me.CubeGrid);if(ƀ.Count>0){for(int ȓ=0;ȓ<ƀ.Count;ȓ++){if(ƀ[ȓ].CustomName.Contains(p)){Ö=Ù+" "+(ȓ+1)+" "+p;ƀ[
ȓ].CustomName=Ö;Ɓ.Add(ƀ[ȓ]);}if(!ƀ[ȓ].CustomName.Contains(p)){Ö=Ù+" "+(ȓ+1)+" "+p;ƀ[ȓ].CustomName=Ö;Ɓ.Add(ƀ[ȓ]);}}}ƀ.
Clear();if(Ɓ.Count<=0||Ɓ[0]==null){Echo($"Sensor with tag: '{p.Replace("[","[[").Replace("]","]]")}' not found.");return;}ŭ=Ɓ
[0];ŭ.DetectAsteroids=true;ŭ.DetectEnemy=true;ŭ.DetectFriendly=true;ŭ.DetectLargeShips=true;ŭ.DetectSmallShips=true;ŭ.
DetectSubgrids=true;ŭ.DetectFloatingObjects=false;ŭ.DetectStations=true;ŭ.DetectPlayers=false;ŭ.DetectNeutral=true;ŭ.DetectOwner=true;
Ƞ(ŭ);ŭ.LeftExtend=s_llm;ŭ.RightExtend=s_rlm;ŭ.BottomExtend=s_btlm;ŭ.TopExtend=s_tlm;ŭ.BackExtend=s_bklm;ŭ.FrontExtend=
s_flm;}Ƃ.Clear();ƃ.Clear();Ȣ.GetBlocksOfType<IMyCameraBlock>(Ƃ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ƃ.Count>0){for(int ȓ=0;ȓ<Ƃ.
Count;ȓ++){if(Ƃ[ȓ].CustomName.Contains(p)){Ö=á+" "+(ȓ+1)+" "+p;Ƃ[ȓ].CustomName=Ö;ƃ.Add(Ƃ[ȓ]);}if(!Ƃ[ȓ].CustomName.Contains(p)
){Ö=á+" "+(ȓ+1)+" "+p;Ƃ[ȓ].CustomName=Ö;ƃ.Add(Ƃ[ȓ]);}}}Ƃ.Clear();Ƅ.Clear();ƅ.Clear();Ȣ.GetBlocksOfType<IMyShipConnector>(
Ƅ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ƅ.Count>0){for(int ȓ=0;ȓ<Ƅ.Count;ȓ++){if(Ƅ[ȓ].CustomName.Contains(q)&&!Ƅ[ȓ].CustomName.
Contains("Refresh")){Ö=â+" "+(ȓ+1)+" "+p;Ƅ[ȓ].CustomName=Ö;ƅ.Add(Ƅ[ȓ]);}if(!Ƅ[ȓ].CustomName.Contains(p)&&!Ƅ[ȓ].CustomName.
Contains("Refresh")){Ö=â+" "+(ȓ+1)+" "+p;Ƅ[ȓ].CustomName=Ö;ƅ.Add(Ƅ[ȓ]);}}}Ƅ.Clear();Ɔ.Clear();Ƈ.Clear();ƈ.Clear();Ȣ.
GetBlocksOfType<IMyCargoContainer>(Ɔ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ɔ.Count>0){for(int ȓ=0;ȓ<Ɔ.Count;ȓ++){if(ŋ){if(Ɔ[ȓ].CustomName.
Contains(p)){string Ȥ="";if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockSmall")||Ɔ[ȓ].BlockDefinition.SubtypeId.Contains(
"LargeBlockSmall")){Ȥ="Small ";}if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockMedium")){Ȥ="Medium ";}if(Ɔ[ȓ].BlockDefinition.
SubtypeId.Contains("LargeBlockLarge")||Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockLarge")){Ȥ="Large ";}Ö=Ȥ+ì+" "+(ȓ+1)+
" "+p;Ɔ[ȓ].CustomName=Ö;Ƈ.Add(Ɔ[ȓ]);}if(Ɔ[ȓ].CustomName.Contains(z)){string Ȥ="";if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains
("SmallBlockSmall")||Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockSmall")){Ȥ="Small ";}if(Ɔ[ȓ].BlockDefinition.
SubtypeId.Contains("SmallBlockMedium")){Ȥ="Medium ";}if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockLarge")||Ɔ[ȓ].
BlockDefinition.SubtypeId.Contains("SmallBlockLarge")){Ȥ="Large ";}Ö=Ȥ+ì+" "+(ȓ+1)+" "+p;ƈ.Add(Ɔ[ȓ]);}if(!Ɔ[ȓ].CustomName.Contains(p)&&
!Ɔ[ȓ].CustomName.Contains(z)){string Ȥ="";if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockSmall")||Ɔ[ȓ].
BlockDefinition.SubtypeId.Contains("LargeBlockSmall")){Ȥ="Small ";}if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("Medium")){Ȥ="Medium ";}
if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockLarge")||Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockLarge"))
{Ȥ="Large ";}Ö=Ȥ+ì+" "+(ȓ+1)+" "+p;Ɔ[ȓ].CustomName=Ö+" "+p;Ƈ.Add(Ɔ[ȓ]);}}else{string Ȥ="";if(Ɔ[ȓ].BlockDefinition.
SubtypeId.Contains("SmallBlockSmall")||Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockSmall")){Ȥ="Small ";Ö=Ȥ+ì+" "+(ȓ+1)+" "
+p+" "+z;Ɔ[ȓ].CustomName=Ö+" "+ª;ƈ.Add(Ɔ[ȓ]);}if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("Medium")){Ȥ="Medium ";Ö=Ȥ+ì+" "
+(ȓ+1)+" "+p;Ɔ[ȓ].CustomName=Ö;Ƈ.Add(Ɔ[ȓ]);}if(Ɔ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockLarge")||Ɔ[ȓ].
BlockDefinition.SubtypeId.Contains("SmallBlockLarge")){Ȥ="Large ";Ö=Ȥ+ì+" "+(ȓ+1)+" "+p;Ɔ[ȓ].CustomName=Ö;Ƈ.Add(Ɔ[ȓ]);}}}}Ɔ.Clear();ƍ.
Clear();Ǝ.Clear();Ə.Clear();Ȣ.GetBlocksOfType<IMyPathRecorderBlock>(ƍ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(ƍ.Count>0){for(int ȓ=0;ȓ
<ƍ.Count;ȓ++){if(ƍ[ȓ].CustomName.Contains(r)||ƍ[ȓ].CustomName.Contains($" {Dock}")){Ö=ê+" Dock";ƍ[ȓ].CustomName=Ö+" "+(ȓ+
1)+" "+r;Ǝ.Add(ƍ[ȓ]);}if(ƍ[ȓ].CustomName.Contains(s)||ƍ[ȓ].CustomName.Contains($" {Undock}")){Ö=ê+" Undock";ƍ[ȓ].
CustomName=Ö+" "+(ȓ+1)+" "+s;Ə.Add(ƍ[ȓ]);}}}ƍ.Clear();Ɛ.Clear();Ƒ.Clear();Ȣ.GetBlocksOfType<IMyFlightMovementBlock>(Ɛ,ȕ=>ȕ.
CubeGrid==Me.CubeGrid);if(Ɛ.Count>0){for(int ȓ=0;ȓ<Ɛ.Count;ȓ++){if(Ɛ[ȓ].CustomName.Contains(p)){Ö=é;Ɛ[ȓ].CustomName=Ö+" "+(ȓ+1)+
" "+p;Ƒ.Add(Ɛ[ȓ]);}if(!Ɛ[ȓ].CustomName.Contains(p)){Ö=é;Ɛ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+p;Ƒ.Add(Ɛ[ȓ]);}}}Ɛ.Clear();ƥ.Clear(
);Ʀ.Clear();Ȣ.GetBlocksOfType<IMyThrust>(ƥ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(ƥ.Count>0){for(int ȓ=0;ȓ<ƥ.Count;ȓ++){if(ƥ[ȓ].
CustomName.Contains(p)){string Ȥ="";if(ƥ[ȓ].BlockDefinition.SubtypeId.Contains("Hydrogen")){Ȥ=Ü;}if(ƥ[ȓ].BlockDefinition.SubtypeId
.Contains("Atmospheric")){Ȥ=Û;}if(ƥ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockLargeThrust")||ƥ[ȓ].BlockDefinition.
SubtypeId.Contains("SmallBlockLargeThrust")||ƥ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockSmallThrust")||ƥ[ȓ].
BlockDefinition.SubtypeId.Contains("LargeBlockSmallThrust")||ƥ[ȓ].BlockDefinition.SubtypeId.Contains("ModularThruster")){Ȥ=Ý;}if(ƥ[ȓ].
BlockDefinition.SubtypeId.Contains("LargeBlockPrototechThruster")||ƥ[ȓ].BlockDefinition.SubtypeId.Contains(
"SmallBlockPrototechThruster")){Ȥ=Þ;}Ö=Ȥ+" "+Ú+" "+(ȓ+1)+" "+p;ƥ[ȓ].CustomName=Ö;Ʀ.Add(ƥ[ȓ]);}if(!ƥ[ȓ].CustomName.Contains(p)){string Ȥ="";if(ƥ[ȓ].
BlockDefinition.SubtypeId.Contains("Hydrogen")){Ȥ=Ü;}if(ƥ[ȓ].BlockDefinition.SubtypeId.Contains("Atmospheric")){Ȥ=Û;}if(ƥ[ȓ].
BlockDefinition.SubtypeId.Contains("LargeBlockLargeThrust")||ƥ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockLargeThrust")||ƥ[ȓ].
BlockDefinition.SubtypeId.Contains("SmallBlockSmallThrust")||ƥ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockSmallThrust")||ƥ[ȓ].
BlockDefinition.SubtypeId.Contains("ModularThruster")){Ȥ=Ý;}if(ƥ[ȓ].BlockDefinition.SubtypeId.Contains("LargeBlockPrototechThruster")||
ƥ[ȓ].BlockDefinition.SubtypeId.Contains("SmallBlockPrototechThruster")){Ȥ=Þ;}Ö=Ȥ+" "+Ú+" "+(ȓ+1)+" "+p;ƥ[ȓ].CustomName=Ö;
Ʀ.Add(ƥ[ȓ]);}}}ƥ.Clear();ư=Ȣ.GetBlockGroupWithName(À)as IMyBlockGroup;if(Ʀ.Count>0){ŀ=true;Ʀ.Clear();Ȣ.GetBlocksOfType<
IMyThrust>(Ʀ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);Echo($"Thruster Group {À.Replace("[","[[").Replace("]","]]")} found");}else{ŀ=false;Echo
($"Thruster Group {À.Replace("[","[[").Replace("]","]]")} not found");}ƒ.Clear();Ɠ.Clear();Ɣ.Clear();ƕ.Clear();Ɩ.Clear();
Ȣ.GetBlocksOfType<IMyTimerBlock>(ƒ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(ƒ.Count>0){for(int ȓ=0;ȓ<ƒ.Count;ȓ++){if(ƒ[ȓ].
CustomName.Contains(t)||ƒ[ȓ].CustomName.Contains(TON)){Ö=è;ƒ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+t;Ɠ.Add(ƒ[ȓ]);}if(ƒ[ȓ].CustomName.
Contains(u)||ƒ[ȓ].CustomName.Contains(TOFF)){Ö=è;ƒ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+u;Ɣ.Add(ƒ[ȓ]);}if(ƒ[ȓ].CustomName.Contains(x)||
ƒ[ȓ].CustomName.Contains(PrecM)){Ö=è;ƒ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+x;ƕ.Add(ƒ[ȓ]);}if(ƒ[ȓ].CustomName.Contains(s)||ƒ[ȓ].
CustomName.Contains($" {Undock}")){Ö=è;ƒ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+s;Ɩ.Add(ƒ[ȓ]);}}}ƒ.Clear();Ɨ.Clear();Ƙ.Clear();ƙ.Clear();ƚ.
Clear();ƛ.Clear();Ɯ.Clear();Ɲ.Clear();ƭ=Ȣ.GetBlockGroupWithName(x);if(ƭ!=null){Ł=true;Echo(
$"Precision mode group {x.Replace("[","[[").Replace("]","]]")} found");}else{Ł=false;Echo($"Precision mode group {x.Replace("[","[[").Replace("]","]]")} not found");}Ʈ=Ȣ.
GetBlockGroupWithName(s);if(Ʈ!=null){ł=true;Echo($"Undock mode group {s.Replace("[","[[").Replace("]","]]")} found");}else{ł=false;Echo(
$"Undock mode group {s.Replace("[","[[").Replace("]","]]")} not found");}Ư=Ȣ.GetBlockGroupWithName(v);if(Ư!=null){Ń=true;Echo(
$"Reset mode group {v.Replace("[","[[").Replace("]","]]")} found");}else{Ń=false;Echo($"Reset mode group {v.Replace("[","[[").Replace("]","]]")} not found");}Ȣ.GetBlocksOfType<
IMyLightingBlock>(Ɨ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ɨ.Count>0){for(int ȓ=0;ȓ<Ɨ.Count;ȓ++){if(Ɨ[ȓ].CustomName.Contains(r)||Ɨ[ȓ].CustomName
.Contains($" {Dock}")){Ö=ë;Ɨ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+r;ƙ.Add(Ɨ[ȓ]);}if(!ł&&(Ɨ[ȓ].CustomName.Contains(s)||Ɨ[ȓ].
CustomName.Contains($" {Undock}"))){Ö=ë;Ɨ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+s;Ƙ.Add(Ɨ[ȓ]);}if(Ɨ[ȓ].CustomName.Contains(w)||Ɨ[ȓ].
CustomName.Contains($" {CA}")){Ö=ë;Ɨ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+w;ƚ.Add(Ɨ[ȓ]);}if(!Ń&&(Ɨ[ȓ].CustomName.Contains(v)||Ɨ[ȓ].
CustomName.Contains($" {Reset}"))){Ö=ë;Ɨ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+v;Ɯ.Add(Ɨ[ȓ]);}if(!Ł&&(Ɨ[ȓ].CustomName.Contains(x)||Ɨ[ȓ].
CustomName.Contains($" {PrecM}"))){Ö=ë;Ɨ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+x;ƛ.Add(Ɨ[ȓ]);}if(Ɨ[ȓ].CustomName.Contains(µ)||Ɨ[ȓ].
CustomName.Contains($" {dmg}")){Ö=ë;Ɨ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+µ;Ɲ.Add(Ɨ[ȓ]);}}if(Ł){ƭ.GetBlocksOfType<IMyLightingBlock>(ƛ,ȕ
=>ȕ.CubeGrid==Me.CubeGrid);if(ƛ.Count>0){for(int ȓ=0;ȓ<ƛ.Count;ȓ++){Ö=ë;ƛ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+x;}}}if(ł){Ʈ.
GetBlocksOfType<IMyLightingBlock>(Ƙ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ƙ.Count>0){for(int ȓ=0;ȓ<Ƙ.Count;ȓ++){Ö=ë;Ƙ[ȓ].CustomName=Ö+" "+(ȓ+1
)+" "+s;}}}if(Ń){Ư.GetBlocksOfType<IMyLightingBlock>(Ɯ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ɯ.Count>0){for(int ȓ=0;ȓ<Ɯ.Count;ȓ
++){Ö=ë;Ɯ[ȓ].CustomName=Ö+" "+(ȓ+1)+" "+v;}}}}Ɨ.Clear();ƞ.Clear();Ɵ.Clear();Ȣ.GetBlocksOfType<IMyBatteryBlock>(ƞ,ȕ=>ȕ.
CubeGrid==Me.CubeGrid);if(ƞ.Count>0){for(int ȓ=0;ȓ<ƞ.Count;ȓ++){if(ƞ[ȓ].CustomName.Contains(p)){string Ȥ="";if(ƞ[ȓ].CustomName.
Contains("Small")){Ȥ="Small";}if(ƞ[ȓ].CustomName.Contains("Medium")){Ȥ="Medium";}Ö=Ȥ+" "+ã+" "+(ȓ+1)+" "+p;ƞ[ȓ].CustomName=Ö;Ɵ.
Add(ƞ[ȓ]);}if(!ƞ[ȓ].CustomName.Contains(p)){string Ȥ="";if(ƞ[ȓ].CustomName.Contains("Small")){Ȥ="Small";}if(ƞ[ȓ].CustomName
.Contains("Medium")){Ȥ="Medium";}Ö=Ȥ+" "+ã+" "+(ȓ+1)+" "+p;ƞ[ȓ].CustomName=Ö;Ɵ.Add(ƞ[ȓ]);}}}ƞ.Clear();Ơ.Clear();ơ.Clear()
;Ȣ.GetBlocksOfType<IMyGasTank>(Ơ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ơ.Count>0){for(int ȓ=0;ȓ<Ơ.Count;ȓ++){if(Ơ[ȓ].CustomName.
Contains(y)&&Ơ[ȓ].BlockDefinition.SubtypeId.Contains("HydrogenTank")){Ö=ä+" "+(ȓ+1)+" "+y;Ơ[ȓ].CustomName=Ö;ơ.Add(Ơ[ȓ]);}if(!Ơ[ȓ
].CustomName.Contains(y)&&Ơ[ȓ].BlockDefinition.SubtypeId.Contains("HydrogenTank")){Ö=ä+" "+(ȓ+1)+" "+y;Ơ[ȓ].CustomName=Ö;
ơ.Add(Ơ[ȓ]);}if(!Ơ[ȓ].BlockDefinition.SubtypeId.Contains("HydrogenTank")){Ö=å+" "+(ȓ+1)+" "+y;Ơ[ȓ].CustomName=Ö;}}}Ơ.
Clear();Ƣ.Clear();ƣ.Clear();Ȣ.GetBlocksOfType<IMyShipDrill>(Ƣ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ƣ.Count>0){for(int ȓ=0;ȓ<Ƣ.Count
;ȓ++){if(Ƣ[ȓ].CustomName.Contains(p)){Ö=æ+" "+(ȓ+1)+" "+p;Ƣ[ȓ].CustomName=Ö;ƣ.Add(Ƣ[ȓ]);}if(!Ƣ[ȓ].CustomName.Contains(p))
{Ö=æ+" "+(ȓ+1)+" "+p;Ƣ[ȓ].CustomName=Ö;ƣ.Add(Ƣ[ȓ]);}}}Ƣ.Clear();Ʊ.Clear();Ʋ.Clear();Ȣ.GetBlocksOfType<IMyGyro>(Ʊ,ȕ=>ȕ.
CubeGrid==Me.CubeGrid);if(Ʊ.Count>0){for(int ȓ=0;ȓ<Ʊ.Count;ȓ++){if(Ʊ[ȓ].CustomName.Contains(p)){Ö=ç+" "+(ȓ+1)+" "+p;Ʊ[ȓ].
CustomName=Ö;Ʋ.Add(Ʊ[ȓ]);}if(!Ʊ[ȓ].CustomName.Contains(p)){Ö=ç+" "+(ȓ+1)+" "+p;Ʊ[ȓ].CustomName=Ö;Ʋ.Add(Ʊ[ȓ]);}}}Ʊ.Clear();Ƨ.Clear(
);ƨ.Clear();Ʃ.Clear();Ȣ.GetBlocksOfType<IMyTerminalBlock>(Ƨ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ƨ.Count>0){for(int ȓ=0;ȓ<Ƨ.
Count;ȓ++){string Ȥ="";if((Ƨ[ȓ].BlockDefinition.SubtypeId.Contains("TextPanel")&&!Ƨ[ȓ].Name.Contains(debug)&&Ƨ[ȓ].Name.
Contains(display))||(!Ƨ[ȓ].BlockDefinition.SubtypeId.Contains("TextPanel")&&!Ƨ[ȓ].Name.Contains(debug)&&Ƨ[ȓ].Name.Contains(
display))){Ȥ=í;Ö=Ȥ+í+" "+(ȓ+1)+" "+p+" "+display;Ƨ[ȓ].CustomName=Ö;ƨ.Add(Ƨ[ȓ]);}if(Ƨ[ȓ].BlockDefinition.SubtypeId.Contains(
"TextPanel")&&Ƨ[ȓ].Name.Contains(debug)&&!Ƨ[ȓ].Name.Contains(display)){Ȥ=í;Ö=Ȥ+í+" "+(ȓ+1)+" "+p+" "+debug;Ƨ[ȓ].CustomName=Ö;Ʃ.Add(
Ƨ[ȓ]);}if(Ƨ[ȓ].BlockDefinition.SubtypeId.Contains("TextPanel")&&!Ƨ[ȓ].Name.Contains(debug)&&!Ƨ[ȓ].Name.Contains(display))
{Ȥ=í;Ö=Ȥ+í+" "+(ȓ+1)+" "+p+" "+display;Ƨ[ȓ].CustomName=Ö;ƨ.Add(Ƨ[ȓ]);}}}Ƨ.Clear();ƪ.Clear();Ȣ.GetBlocksOfType<
IMyProjector>(ƪ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(ƪ.Count>0){for(int ȓ=0;ȓ<ƪ.Count;ȓ++){string Ȥ="";Ȥ="Projector";Ö=Ȥ+" "+(ȓ+1)+" "+p;ƪ
[ȓ].CustomName=Ö;}}ƪ.Clear();ƫ.Clear();Ȣ.GetBlocksOfType<IMyEventControllerBlock>(ƫ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(ƫ.
Count>0){for(int ȓ=0;ȓ<ƫ.Count;ȓ++){Ö=î+" "+(ȓ+1)+" "+p;ƫ[ȓ].CustomName=Ö;}}ƫ.Clear();Ƥ.Clear();ŗ=p;Ƹ=IGC.
RegisterBroadcastListener(ŗ);ƹ=IGC.RegisterBroadcastListener(Í);ƺ=IGC.RegisterBroadcastListener(Î);ƻ=IGC.RegisterBroadcastListener(º);Ƽ=IGC.
RegisterBroadcastListener(Ï);ǂ=z.Replace("[","[[").Replace("]","]]");ö=p.Replace("[","[[").Replace("]","]]");ø=q.Replace("[","[[").Replace("]",
"]]");ù=r.Replace("[","[[").Replace("]","]]");ú=s.Replace("[","[[").Replace("]","]]");û=t.Replace("[","[[").Replace("]","]]"
);ü=u.Replace("[","[[").Replace("]","]]");ý=x.Replace("[","[[").Replace("]","]]");þ=v.Replace("[","[[").Replace("]","]]")
;ÿ=w.Replace("[","[[").Replace("]","]]");Ā=µ.Replace("[","[[").Replace("]","]]");}public void ǋ(){if(!ļ){Echo(
"Setup not complete.");return;}if(Ʀ.Count<=0&&ŀ){Echo("Please add thrusters to grid");return;}if(ƣ.Count<=0){Echo(
$"Drills with tag: '{ö}' not found.");return;}if(ȥ(Ʋ,$"Gyro with tag: '{ö}' not found.")==null)return;Ş=ȥ(ſ,$"Remote control with tag: '{ö}' not found.");if
(Ş==null)return;if(collisionSenseEnabled){ŭ=ȥ(Ɓ,$"Sensor with tag: '{ö}' not found.");if(ŭ==null)return;}if(ȥ(ƃ,
$"Camera with tag: '{ö}' not found.",false)==null)return;ş=ȥ(ƅ,$"Connector with tag: '{ø}' not found.");if(ş==null)return;if(ȥ(Ƈ,
$"Cargo containers with tag: '{ö}' not found.")==null)return;if(cargoSenseEnabled&&(ƈ==null||ƈ.Count<=0||ƈ[0]==null)){Echo(
$"Sense container with tag: '{ǂ}' not found. Add '{ǂ}' tag to container");}Š=ȥ(Ɗ,$"Antenna with tag: '{ö}' not found.");if(Š==null)return;ţ=ȥ(Ǝ,
$"Docking AI task recorder with tag: '{ù}' not found. Add ' {Dock}' tag");if(ţ==null)return;Ť=ȥ(Ə,$"Undocking AI task recorder with tag: '{ú}' not found. Add ' {Undock}' tag");if(Ť==null)
return;ť=ȥ(Ƒ,$"Flight movement with tag: '{ö}' not found.");if(ť==null)return;if(!ŀ){š=ȥ(Ɠ,
$"Thrust ON timer block with tag: '{û}' not found. Add ' {TON}' tag");if(š==null)return;Ţ=ȥ(Ɣ,$"Thrust OFF timer block with tag: '{ü}' not found. Add ' {TOFF}' tag");if(Ţ==null)return;}if(
!Ł&&ȥ(ƕ,$"Precision mode timer block with tag: '{ý}' not found. Add ' {PrecM}' tag")==null)return;if(!ł&&ȥ(Ɩ,
$"Undock mode timer block with tag: '{ú}' not found. Add ' {Undock}' tag")==null)return;ŧ=ȥ(ƙ,$"dock indicator light with tag: '{ù}' not found. Add ' {Dock}' tag");if(ŧ==null)return;string Ȧ=ł?$"Add undock indicator light with tag: '{ú}' to {ú} group - ensure {ú} group is in AI {ú} task recorder waypoint actions"
:$"undock indicator light with tag: '{ú}' not found. Add ' {Undock}' tag";Ũ=ȥ(Ƙ,Ȧ);if(Ũ==null)return;ũ=ȥ(ƚ,
$"collision avoidance required indicator light with tag: '{ÿ}' not found. Add ' {CA}' tag");if(ũ==null)return;string ȧ=Ł?$"Add precision mode indicator light with tag: '{ý}' to {ý} group - ensure {ý} group is in AI {ù} task recorder waypoint actions"
:$"Precision mode required indicator light with tag: '{ý}' not found. Add ' {PrecM}' tag";Ū=ȥ(ƛ,ȧ);if(Ū==null)return;
string Ȩ=Ń?$"Reset mode indicator light with tag: '{þ}' to {þ} group - ensure {þ} group is in Sensor {ö} detect action only":
$"Dock reset indicator light with tag: '{þ}' not found. Add ' {Reset}' tag";ū=ȥ(Ɯ,Ȩ);if(ū==null)return;if(damageReportingEnabled){Ŭ=ȥ(Ɲ,
$"Damage indicator light with tag: '{Ā}' not found. Add ' {dmg}' tag\n",false);}if(Ɵ==null||Ɵ.Count<=0){Echo($"Batteries with tag: '{ö}' not found.");ļ=false;return;}}ȩ ȥ<ȩ>(List<ȩ>Ȫ,string ȫ
,bool Ȭ=true)where ȩ:class{if(Ȫ==null||Ȫ.Count<=0||Ȫ[0]==null){Echo(ȫ);if(Ȭ)ļ=false;return null;}return Ȫ[0];}public void
Ǐ(){float ȭ=0.0f;float Ȯ=0.0f;Ř=0;for(int ȓ=0;ȓ<Ƈ.Count;ȓ++){if(Ƈ[ȓ]!=null){if(Ƈ[ȓ].IsFunctional){float ȯ=(float)Ƈ[ȓ].
GetInventory(0).CurrentVolume;float Ȱ=(float)Ƈ[ȓ].GetInventory(0).MaxVolume;ȭ+=ȯ;Ȯ+=Ȱ;}}else{Ƶ.AppendLine(
$"Warning: Cargo container [{ȓ}] is null in cargo_check");}}if(Ȯ>0.0f){Ř=(ȭ/Ȯ)*100;}else{Ř=0.0f;}if(Ř==100.0f){đ=true;}if(Ř<100.0f){đ=false;}if(Ř==0.0f){Ē=true;}if(Ř>0.0f){Ē=
false;}if(Ē&&ē){ē=false;}if(cargoSenseEnabled&&ƈ.Count>0){float ȱ=0.0f;float Ȳ=0.0f;float ȳ=0.0f;for(int ȓ=0;ȓ<ƈ.Count;ȓ++){
if(ƈ[ȓ]!=null){if(Ƈ[ȓ].IsFunctional){float ȴ=(float)ƈ[ȓ].GetInventory(0).CurrentVolume;float ȵ=(float)ƈ[ȓ].GetInventory(0)
.MaxVolume;ȱ+=ȴ;Ȳ+=ȵ;}}else{Ƶ.AppendLine($"Warning: Sense cargo container [{ȓ}] is null in cargo_check");}}if(Ȳ>0.0f){ȳ=(
ȱ/Ȳ)*100;}else{ȳ=0.0f;}Ĉ=(ȳ>cargoSenseLimit);}else{Ĉ=false;}}public void Ǩ(){if(Ş==null){Ƶ.AppendLine(
"Remote control is null in remote_control_position_update");return;}Ż=Ş.GetPosition();}public void Ǔ(){if(Ş!=null){m=Ş.GetShipSpeed();h=m;}}public void ǐ(){if(!
damageReportingEnabled){Ã="OK";}if(damageReportingEnabled){if(Ŭ==null&&damageReportingEnabled){Ã="UNK";Ƶ.AppendLine(
"Warning: Damage light is null in damage check");return;}if(Ŭ!=null){if(Ŭ.Enabled&&damageReportingEnabled&&Ŭ.IsFunctional||damageReportingEnabled&&!Ŭ.IsFunctional){Ã=
"DMG";}if(!Ŭ.Enabled&&damageReportingEnabled&&Ŭ.IsFunctional){Ã="OK";}}}}public void Ǒ(){ř=0f;Ś=0f;int ȶ=0;for(int ȓ=0;ȓ<Ɵ.
Count;ȓ++){if(Ɵ[ȓ]!=null){if(Ɵ[ȓ].IsFunctional){Ŧ=Ɵ[ȓ];ř+=Ŧ.CurrentStoredPower;Ś+=Ŧ.MaxStoredPower;ȶ++;}}}Ă=(Ś>0)?(ř/Ś)*100f:
0f;Ĕ=(Ă>=bat_CHGhi);ĕ=(Ă<=bat_CHGlow);if(!ĕ&&ĥ&&Ĕ){ĥ=false;}}public void ǒ(){if(ơ.Count<=0){}Ŝ=0;d=0;e=0;f=0;n=0.0;if(ơ.
Count>0){for(int ȓ=0;ȓ<ơ.Count;ȓ++){if(ơ[ȓ]!=null){if(ơ[ȓ].IsFunctional){Ů=ơ[ȓ];Ŝ=Ů.FilledRatio*100.0f;d=d+Ŝ;e=100.0f;f=f+e;n
=(d/f)*100.0f;}}}}if(n==gas_CHGhi){Ģ=true;}if(n<gas_CHGhi){Ģ=false;}if(n<=gas_CHGlow){ģ=true;}if(n>gas_CHGlow){ģ=false;}
if((!ģ&&Ĥ&&Ģ&&!ignore_Htank)||(ignore_Htank)){Ĥ=false;}if(!Ĥ&&!ignore_Htank){if(!ŉ){ȷ(false);ŉ=true;ň=false;}}}public void
ǔ(){if(ĥ||Ĥ){Ė=true;}else{Ė=false;}}public void ǖ(){if(Ƹ.HasPendingMessage){ƽ=Ƹ.AcceptMessage();Ç=ƽ.Data.ToString();õ=Ç;}
if(ƹ.HasPendingMessage){ƾ=ƹ.AcceptMessage();È=ƾ.Data.ToString();}if(ƻ.HasPendingMessage){ƿ=ƻ.AcceptMessage();É=ƿ.Data.
ToString();}if(ƺ.HasPendingMessage){ǀ=ƺ.AcceptMessage();Ê=ǀ.Data.ToString();}ȸ();if(Ç!=null){if(õ!=ô){ȅ(Ç,Me,ò,ó);œ=true;}}if(È
!=null){if(È.Contains(Å)){Ľ=true;}else{Ľ=false;}}if(Ê!=null){if(Ê.Contains(Å)){ľ=true;}else{ľ=false;}}if(Ľ||ľ){ĩ=true;}
else{ĩ=false;}if(É!=null){if(É.Contains(Á)){ă=true;}else{ă=false;}}}void ȸ(){if(Š!=null&&Ɗ[0]!=null){if(Ƽ.HasPendingMessage)
{MyIGCMessage ȹ=Ƽ.AcceptMessage();Ƭ.Add(ȹ);}if(Ƭ.Count>0){ō=true;}else{ō=false;}if(ō){ð=Ƭ[0].Data.ToString();Ⱥ(ð);if(Ŏ){ȡ
();Ŏ=false;}if(Ƭ.Count>0&&ō){Ƭ.RemoveAt(0);}}}}public void Ⱥ(string ǃ){Ŏ=false;if(Æ!=ǃ){Æ=ǃ;Ŏ=true;}else{return;}}public
void Ǘ(string ǃ){if(!string.IsNullOrEmpty(ǃ)&&!string.IsNullOrWhiteSpace(ǃ)&&ǃ!=ï){Ġ=true;}else Ġ=false;if(string.
IsNullOrEmpty(ǃ)||string.IsNullOrWhiteSpace(ǃ)||ǃ==ï){ğ=true;if(ǃ!=ï){ǃ=ï;}}else ğ=false;}public void ǘ(){if(Ġ||ğ){if(J==1){I=H;J=0;}
if(J==0){if(œ){Ȏ(Me.CustomData.ToString(),Me);œ=false;}J=1;if(H!=I){ċ=true;}if(H==I){ċ=false;}}}}public void Ǚ(){if(ğ&&!ė)
{H=0;Ä="Idle";}if((H==0)||(ċ&&Ą&&H!=7)){Č=true;P=0;}else Č=false;if(H==0&&ė){ė=false;Ĵ=false;}if(H==0&&Ĵ){Ĵ=false;}if(H>=
1&&H<=4){Ď=true;}else Ď=false;if(H==5&&(!Ċ)){č=true;}else č=false;if(H==6){ď=true;}else{ď=false;}if(H==7&&(!ĩ||!Ė)){Đ=
true;}else Đ=false;if((H==8&&ġ)||(H==0&&ş.IsConnected&&ġ&&!Đ&&!ē&&Ē&&!Ė)){ġ=false;Ä="Resetting";}if(H==0&&Ä=="Docking"){Ä=
"Idle";}}public void ǚ(bool Ŀ){if(ş!=null){if(ş.IsConnected&&Ŀ){Ŀ=false;}}if(ş!=null){if((ş.Status==MyShipConnectorStatus.
Connected&&((autoChargeMode&&!Đ))||(!autoChargeMode&&!Đ&&ĥ))){if(!ņ){Ȼ(ChargeMode.Recharge);ņ=true;Ň=false;}}}if((!autoChargeMode
&&!ĥ)||(!ş.IsConnected||Đ)){if(!Ň){Ȼ(ChargeMode.Auto);ņ=false;Ň=true;}}}public void Ǜ(){int ȼ=Runtime.
CurrentInstructionCount;if(!ď){return;}if(ş==null){Ƶ.AppendLine("Connector missing - exiting");return;}if(Ũ==null){Ƶ.AppendLine(
"Undock light missing - exiting");return;}if(ŧ==null){Ƶ.AppendLine("Docklight missing - exiting");return;}if(ũ==null){Ƶ.AppendLine(
"Collision avoidance light missing - exiting");return;}if(collisionSenseEnabled&&ŭ==null){Ƶ.AppendLine("Collision sensor missing missing - exiting");return;}if(ď&&!ş
.IsConnected&&S==0){Ä="Docking init";Ŀ=false;if(!Ũ.Enabled&&!ş.IsConnected&&ş.Status!=MyShipConnectorStatus.Connectable||
!Ũ.Enabled){Ũ.Enabled=true;}if(!Ę||!ę){Ȗ();}S=1;Q=0;ũ.Enabled=true;if(collisionSenseEnabled){if(!ŭ.Enabled){ŭ.Enabled=
true;}}if(ŧ.Enabled){ŧ.Enabled=false;}}if(ş!=null&&ş.Status==MyShipConnectorStatus.Connected){if(ĕ&&!ĥ){ĥ=true;}if(ģ&&!Ĥ&&!
ignore_Htank){Ĥ=true;}if(ţ!=null&&Ť!=null&&ť!=null){if(ţ.GetValue<bool>(Ž)&&(Č||ď)){ţ.GetActionWithName(Õ).Apply(ţ);if(Ť.GetValue<
bool>(Ž)&&(Č||ď)){Ť.GetActionWithName(Õ).Apply(Ť);}if(ť.GetValue<bool>(Ñ)&&(Č||ď)){ť.GetActionWithName(Ò).Apply(ť);}if(ţ.
GetValue<bool>(Ñ)&&(Č||ď)){ţ.GetActionWithName(Ò).Apply(ţ);}if(Ť.GetValue<bool>(Ñ)&&(Č||ď)){Ť.GetActionWithName(Ò).Apply(Ť);}}}}
}public void ǜ(){if(Ĝ&&!Ņ&&ŀ){Ƚ(true);Ņ=true;ń=false;}if(ş!=null){if(Đ&&Ė&&ş.IsConnected&&(ĕ||(ģ&&!ignore_Htank))){Đ=
false;}}if(ş!=null){if(Đ&&!Ė&&Ē&&!ē&&!Ĵ&&ş.IsConnected&&T==0&&(!ŀ||ŀ)){if(!Ę||!ę){Ȗ();}K=0;Ä="Undocking";if(ŧ!=null){ŧ.
Enabled=false;}ş.Enabled=false;if(!ŀ){if(š!=null){if(!š.Enabled){š.Enabled=true;}if(!š.IsCountingDown){š.Trigger();}}}else{Ƚ(
true);Ņ=true;ń=false;}if(ť!=null){if(ť.SpeedLimit!=nav_speed){ť.SpeedLimit=nav_speed;}}T=1;}}if(ş!=null){if(T==1&&!ş.
IsConnected){Ȗ();ş.Enabled=false;if(ũ!=null){ũ.Enabled=false;}if(Ũ!=null){Ũ.Enabled=false;}if(ť!=null){ť.PrecisionMode=true;ť.
CollisionAvoidance=false;if(ť.SpeedLimit!=nav_speed){ť.SpeedLimit=nav_speed;}if(!ť.GetValue<bool>(Ñ)){ť.ApplyAction(Ó);}}if(Ť!=null){if(!Ť
.GetValue<bool>(Ñ)){Ť.ApplyAction(Ó);}if(!Ť.GetValue<bool>(Ž)){Ť.ApplyAction(Ô);}}T=2;if(ŀ&&!Ņ){Ƚ(true);Ņ=true;ń=false;}}
}if(T==2&&Ũ.Enabled&&!ş.IsConnected){if(ũ!=null){ũ.Enabled=false;}if(collisionSenseEnabled){if(ŭ!=null){if(ŭ.Enabled){ŭ.
Enabled=false;}}}if(ť!=null){ť.PrecisionMode=false;ť.CollisionAvoidance=true;if(ť.SpeedLimit!=nav_speed){ť.SpeedLimit=nav_speed
;}}if(ş!=null){ş.Enabled=true;}T=3;}if(ş!=null&&Ť!=null&&Ũ!=null){if(!ş.IsConnected&&!Ť.GetValue<bool>(Ž)&&T==2&&!Ũ.
Enabled){if(udock_conf){T=1;}if(!udock_conf){Ũ.Enabled=true;}}}if(Ĝ&&T==3&&K==0){Ä="Undocked";Ȗ();K=1;}Ǔ();if(T>0&&T<3&&m<=
currentSpeedNotMovingThreshold){ę=true;if(Ť!=null){if(!Ť.GetValue<bool>(Ž)){N++;}}j=Math.Round(((double)N*(double)10*X)/(double)1000,1);}if(Ħ&&T>0&&T<
3&&!ş.IsConnected){if(ū!=null){if(!ū.Enabled){ū.Enabled=true;}}if(Ũ!=null){if(!Ũ.Enabled){Ũ.Enabled=true;}}T=3;}if(T>2&&Ĝ
){ę=false;}}public void ǝ(){int ȼ=Runtime.CurrentInstructionCount;if(S>0){Ę=true;}else Ę=false;if(ę||Ę){Ě=true;}else Ě=
false;if(ŧ!=null){if(ŧ.Enabled){ě=true;}else ě=false;}else{ě=false;}if(Ũ!=null){if(Ũ.Enabled){Ĝ=true;}else Ĝ=false;}else{Ĝ=
false;}}public void Ǟ(){if(Č){Q=0;Į=false;ĭ=false;if(!ė){if(Ş!=null){Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş
.SetAutoPilotEnabled(false);Ş.ClearWaypoints();}İ=false;Ķ=false;}if(ţ!=null){if(ţ.GetValue<bool>(Ž)){ţ.ApplyAction(Õ);}if
(ţ.GetValue<bool>(Ñ)){ţ.ApplyAction(Ò);}}if(Ť!=null){if(Ť.GetValue<bool>(Ž)){Ť.ApplyAction(Õ);}if(Ť.GetValue<bool>(Ñ)){Ť.
ApplyAction(Ò);}}if(ť!=null){if(ť.GetValue<bool>(Ñ)){ť.ApplyAction(Ò);}}if(!Ċ){if(!Ŀ){R=0;}č=false;}Ď=false;ď=false;Ä="Idle";T=0;G=
0;}if(R==0&&Č&&Ċ){Ċ=false;}if(č&&!ė&&!Ŀ){ė=true;}if(ĝ&&ė){ė=false;ĝ=false;if(!Ŀ){R=0;}}if(ĝ&&!ė){ĝ=false;}if(!č&&!ė&&R>0)
{if(!Ŀ){R=0;}}if(č||Ď){Ą=true;}else Ą=false;if(ğ&&Ĵ&&!Ċ&&ė&&J==1&&Ĝ){Ċ=true;}if(Ĵ||ē||Ė||ĩ){ĉ=true;}else ĉ=false;}public
void ǟ(){if(Ş!=null){ź=Ş.GetNaturalGravity();}if(ą){ż=Ŷ;}if(!ą){ż=ź;}}public void Ǡ(){if(ź==Vector3D.Zero){Ō=false;}else{Ō=
true;}}public void ǡ(){if(Ō){if(ť!=null){if(!ť.AlignToPGravity){ť.AlignToPGravity=true;}}}if(!Ō){if(ť!=null){if(ť.
AlignToPGravity){ť.AlignToPGravity=false;}}}}public void Ǣ(){Vector3D Ⱦ;if(ě||!Ĝ&&!ě||ę||Ę||Č){ś=false;}else if(Ď||(č&&!(Ę||ę))){ś=true
;}if(R>=6&&R<=10){ś=true;}if(Ď||č){Ⱦ=ǻ(ż)*ā;}else{Ⱦ=Vector3D.Zero;}ȁ(ś,Ⱦ,ě);double ȿ=Ⱦ.GetDim(0);double ɀ=Ⱦ.GetDim(1);
double Ɂ=Ⱦ.GetDim(2);if(ȿ>nav_inst_thr&&!ě||ȿ<-nav_inst_thr&&!ě){G=23;ķ=true;}else ķ=false;if(ɀ>nav_inst_thr&&!ě||ɀ<-
nav_inst_thr&&!ě){ĸ=true;G=23;}else ĸ=false;if(Ɂ>nav_inst_thr&&!ě||Ɂ<-nav_inst_thr&&!ě){Ĺ=true;G=23;}else Ĺ=false;if(Q>0&&Q<4&&ą){Ļ=
true;}else{Ļ=false;}if((ķ&&!Ļ&&!ě)||(ĸ&&!Ļ&&!ě)||(Ĺ&&!Ļ&&!ě)||(ū.Enabled&&!Ę&&!ě)){ĺ=true;G=23;}else{ĺ=false;}}public void Ǥ
(){int ȼ=Runtime.CurrentInstructionCount;if(!Ď){return;}if(Ş==null){Ƶ.AppendLine(
"Error: Remote control is null in navigation_management");return;}Ǩ();if(ş!=null){if((!ĭ&&Q==0&&J==1&&Ď&&!(ş.IsConnected||ş.Status==MyShipConnectorStatus.Connectable)&&Ĝ)&&!Į){
Ş.ClearWaypoints();Q=1;G=1;Ä="mine nav gps--";if(!Ũ.Enabled){Ũ.Enabled=true;}if(ŧ.Enabled){ŧ.Enabled=false;}}if((!ĭ&&Q==1
&&J==1&&Ď&&!(ş.IsConnected||ş.Status==MyShipConnectorStatus.Connectable)&&Ĝ)){Ş.ClearWaypoints();ĭ=true;Į=false;Q=2;Ş.
AddWaypoint(ů,"mine nav gps");G=1;Ä="Nav";if(!Ũ.Enabled){Ũ.Enabled=true;}if(ŧ.Enabled){ŧ.Enabled=false;}}}else{ļ=false;return;}if(Q
==2&&Ż!=Ş.CurrentWaypoint.Coords&&Ď&&Ĝ&&!Į&&ĭ){Q=3;Ş.SpeedLimit=nav_speed;if(H==1){Ş.SetCollisionAvoidance(true);Ş.
SetDockingMode(false);Ş.SetAutoPilotEnabled(!ĺ);G=1;Ȗ();if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}}if(H==2){Ş.SetCollisionAvoidance(
false);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(!ĺ);G=2;Ȗ();if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}}if(H==3){Ş.
SetCollisionAvoidance(false);Ş.SetDockingMode(true);Ş.SetAutoPilotEnabled(!ĺ);G=3;}if(H==4){Ş.SetCollisionAvoidance(true);Ş.SetDockingMode(
false);Ş.SetAutoPilotEnabled(!ĺ);G=4;if(collisionSenseEnabled){if(ŭ!=null){if(!ŭ.Enabled){if(!ŭ.Enabled){ŭ.Enabled=true;}}}}}
Ä="Nav";if(Ũ!=null){if(!Ũ.Enabled){Ũ.Enabled=true;}}if(ŧ!=null){if(ŧ.Enabled){ŧ.Enabled=false;}}}if((Q==3&&ĺ&&H==1)||(Q==
3&&ĺ&&H==4)){Ş.ClearWaypoints();Q=1;ĭ=false;Ȗ();if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}}if((m<=
currentSpeedNotMovingThreshold&&M<D&&Q==3&&!ū.Enabled&&!ĺ&&H==4)||(m<=currentSpeedNotMovingThreshold&&M<D&&Q==3&&!ū.Enabled&&!ĺ&&H==1)){M++;l=Math.
Round(((double)M*(double)10*X)/(double)1000,1);}if((Q==3&&ū.Enabled&&H==1)||(Q==3&&ū.Enabled&&H==4||į)){Ş.ClearWaypoints();Q=
0;ĭ=false;Ȗ();if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}į=false;M=0;l=Math.Round(((double)M*(double)10*X)/(double)1000,
1);}double ɂ=ů.X;double Ƀ=ů.Y;double Ʉ=ů.Z;if(Q==3&&Ż!=Ş.CurrentWaypoint.Coords&&Ď&&Ĝ&&!Į&&ĭ&&!Ş.IsAutoPilotEnabled&&!ĺ){
Ş.SpeedLimit=nav_speed;if(H==1){Ş.SetCollisionAvoidance(true);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(!ĺ);G=1;Ȗ();
if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}}if(H==2){Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.
SetAutoPilotEnabled(!ĺ);G=2;Ȗ();if(ū.Enabled){ū.Enabled=false;}}if(H==3){Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(true);Ş.
SetAutoPilotEnabled(!ĺ);G=3;}if(H==4){Ş.SetCollisionAvoidance(true);Ş.SetDockingMode(true);Ş.SetAutoPilotEnabled(!ĺ);G=4;if(
collisionSenseEnabled){if(!ŭ.Enabled){if(!ŭ.Enabled){ŭ.Enabled=true;}}}}Ä="Nav";if(Ũ!=null){if(!Ũ.Enabled){Ũ.Enabled=true;}}if(ŧ!=null){if(ŧ.
Enabled){ŧ.Enabled=false;}}}if(Q==3&&Ż.X>=ɂ-nav_prec&&Ż.X<=ɂ+nav_prec&&Ż.Y>=Ƀ-nav_prec&&Ż.Y<=Ƀ+nav_prec&&Ż.Z>=Ʉ-nav_prec&&Ż.Z<=
Ʉ+nav_prec&&Ď&&Ĝ&&!Į&&ĭ){Q=4;Į=true;ĭ=false;Ş.SetCollisionAvoidance(true);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(
false);Ş.ClearWaypoints();G=5;Ä="Nav End";if(Ũ!=null){if(!Ũ.Enabled){Ũ.Enabled=true;}}if(ŧ!=null){if(ŧ.Enabled){ŧ.Enabled=
false;}}Q=0;}Ş.GetWaypointInfo(Ƥ);if(Q==3&&ĭ&&Ƥ.Count<=0&&!Į&&Ĝ&&Ď){Q=4;Į=true;ĭ=false;Ş.SetCollisionAvoidance(true);Ş.
SetDockingMode(false);Ş.SetAutoPilotEnabled(false);Ş.ClearWaypoints();G=5;Ä="Nav End";if(!Ũ.Enabled){Ũ.Enabled=true;}if(ŧ!=null){if(ŧ.
Enabled){ŧ.Enabled=false;}}Q=0;}if((Q>0&&Ė)||(Q>0&&ĉ)||(Q>0&&(ş.IsConnected||ş.Status==MyShipConnectorStatus.Connectable))){Q=0
;Į=true;ĭ=false;Ş.SetCollisionAvoidance(true);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(false);Ş.ClearWaypoints();ı=
false;Ĳ=false;G=21;if(ė){ĝ=true;}Ċ=false;ė=false;Ĵ=false;if(!Ę||!ę){Ȗ();if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}}S=1;if(!
ũ.Enabled){ũ.Enabled=true;}if(collisionSenseEnabled){if(ŭ!=null){if(!ŭ.Enabled){ŭ.Enabled=true;}}}if(Ũ!=null){if(!Ũ.
Enabled){Ũ.Enabled=true;}}if(ŧ!=null){if(ŧ.Enabled){ŧ.Enabled=false;}}Ä="RTB";}}public void ǥ(bool Ʌ){if(Ş==null){Ƶ.AppendLine(
"Error: Remote control is null in mining_management");return;}Ǩ();if(!İ&&č&&J==1&&R==0&&!Ě&&Ĝ){Ä="Calculating mineshaft";ĵ=false;Ų.X=ů.X;Ų.Z=ů.Z;Ų.Y=ů.Y;if(ą){ŷ=Vector3D.
Normalize(new Vector3D(-(ů-ż)));}else if(!ą){ŷ=Vector3D.Normalize(new Vector3D(ź));}Vector3D Ɇ=ŷ*drillSetLength;ų.Y=Math.Round(Ų.
Y+Ɇ.Y,2);ų.X=Math.Round(Ų.X+Ɇ.X,2);ų.Z=Math.Round(Ų.Z+Ɇ.Z,2);G=6;İ=true;}double ɇ=Vector3D.Distance(Ż,ų);Vector3D Ɉ=ŷ;
Vector3D ɉ=Ż-Ų;double Ɋ=Vector3D.Dot(ɉ,Ɉ);bool ɋ=Ɋ>drillSetLength+Y;if(!Ĵ&&Ż.X>=ų.X-Y&&Ż.X<=ų.X+Y&&Ż.Y>=ų.Y-Y&&Ż.Y<=ų.Y+Y&&Ż.Z>=
ų.Z-Y&&Ż.Z<=ų.Z+Y&&č&&İ&&Ĝ&&R>0){Ĵ=true;}else if(!Ĵ&&ɋ&&č&&İ&&Ĝ){Ĵ=true;G=26;Ƶ.AppendLine("Overshoot detected");}else if(
!Ĵ&&ɇ<=Y&&č&&İ&&Ĝ){Ĵ=true;Ƶ.AppendLine("Target depth achieved");}else{Ĵ=false;}if(Ĵ&&!ġ){ġ=true;}if(đ&&č&&İ){ē=true;}if((
ĕ&&č&&İ&&!ĥ)||(ĕ&&ş.IsConnected&&!ĥ)||(ĕ&&!ş.IsConnected&&!ĥ&&Q>0&&!č&&Ĝ)){ĥ=true;}if(((ģ&&č&&İ&&!Ĥ&&!ignore_Htank)||(ģ&&
ş.IsConnected&&!Ĥ&&!ignore_Htank))||(ģ&&!ş.IsConnected&&!Ĥ&&!ignore_Htank&&Q>0&&!č&&Ĝ)){Ĥ=true;}if(!Ĵ&&R==0&&č&&İ&&!Ě&&!ş
.IsConnected&&Ĝ){R=1;G=7;Ä="Initiating mining";Ȗ();if(ū.Enabled){ū.Enabled=false;}}if(R==1&&!Ĵ&&č&&İ&&!Ě&&Ĝ&&!ĵ){R=2;ĵ=
true;Ɍ();if(!ĺ&&!Ŕ){Ŕ=true;}Ȕ(Ŕ,Ć,ŏ);G=8;Ä="Mining";}if(R==1&&!Ĵ&&č&&İ&&!Ě&&Ĝ&&ĵ){Ȕ(false,Ć,ŏ);if(Ŕ){Ŕ=false;}G=8;Ä=
"Initiating RTB";}if(R==2&&!Ķ&&!Ĵ&&č&&İ&&!Ě&&Ĝ){R=3;Ķ=true;if(Ş.SpeedLimit!=drill_speed){Ş.SpeedLimit=drill_speed;}ɍ();Ş.AddWaypoint(Ű,
"mineloc");G=9;Ä="Mining+";}if(R==3&&Ķ&&!Ĵ&&č&&İ&&!Ě&&Ĝ){R=4;Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(true);Ş.
SetAutoPilotEnabled(!ĺ);G=10;Ä="Mining++";}if(R==3&&Ķ&&!Ĵ&&č&&İ&&!Ě&&Ĝ&&!Ş.IsAutoPilotEnabled){R=2;Ķ=false;G=10;Ä="Mining++";}double Ɏ=Ű.X;
double ɏ=Ű.Y;double ɐ=Ű.Z;if(R==4&&!ĳ&&Ķ&&!Ĵ&&Ş.CurrentWaypoint.Name==null&&č&&İ&&!Ċ&&!Ě&&Ĝ){R=1;Ş.SetAutoPilotEnabled(!ĺ);Ķ=
false;ĵ=false;G=11;Ä="Mining++-";}if(R==4&&!ĳ&&Ķ&&!Ĵ&&!Ş.IsAutoPilotEnabled&&č&&İ&&!Ċ&&!Ě&&Ĝ){R=2;Ş.SetAutoPilotEnabled(!ĺ);Ķ
=false;G=11;Ä="Mining+++";}if(R==4&&!ĳ&&Ķ&&!Ĵ&&Ż.X>=Ɏ-mine_prec&&Ż.X<=Ɏ+mine_prec&&Ż.Y>=ɏ-mine_prec&&Ż.Y<=ɏ+mine_prec&&Ż.
Z>=ɐ-mine_prec&&Ż.Z<=ɐ+mine_prec&&č&&İ&&!Ċ&&!Ě&&Ĝ){R=5;ĳ=true;Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(true);Ş.
SetAutoPilotEnabled(false);G=12;Ä="Mining++++";}if(R==4&&ĳ&&Ķ&&!Ĵ&&!Ş.IsAutoPilotEnabled&&č&&İ&&!Ċ&&!Ě&&Ĝ){R=5;Ş.SetCollisionAvoidance(
false);Ş.SetDockingMode(true);Ş.SetAutoPilotEnabled(false);ĳ=true;G=12;Ä="Mining+++";}if(R==4&&!ĳ&&!Ĵ&&ğ&&ė&&İ&&!Ċ&&!Ě&&Ĝ){R=
6;Ċ=true;ı=false;Ĳ=false;Ş.SpeedLimit=exit_speed;Ş.ClearWaypoints();Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false
);Ş.SetAutoPilotEnabled(false);G=13;Ä="Terminating mining";}if(R>=1&&R<=4&&!ĳ&&!Ĵ&&Ċ&&ė&&İ&&Ş.CurrentWaypoint.Name!=
"exit shaft"&&!Ě&&Ĝ){R=6;ı=false;Ĳ=false;Ş.SpeedLimit=exit_speed;Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.
SetAutoPilotEnabled(false);Ş.ClearWaypoints();G=13;Ä="Terminating mining";}if(ĉ&&R>=1&&R<=4&&!ĳ&&Ġ&&J==1&&ė&&İ&&!Ċ&&!Ě&&Ĝ){R=6;Ċ=true;ı=
false;Ĳ=false;Ş.SpeedLimit=exit_speed;Ş.ClearWaypoints();Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.
SetAutoPilotEnabled(false);G=14;Ä="Terminating mining";}if(R==5&&ĳ&&ĉ&&č&&İ&&!Ċ&&!Ě&&Ĝ){R=6;Ċ=true;Ş.SpeedLimit=exit_speed;Ş.
SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(false);Ş.ClearWaypoints();ı=false;Ĳ=false;G=16;Ä=
"Terminating mining";}if(R==5&&ĳ&&!Ĵ&&!ĉ&&č&&İ&&!Ċ&&!Ě&&Ĝ){R=1;Ĵ=false;if(Ş.SpeedLimit!=exit_speed){Ş.SpeedLimit=exit_speed;}Ş.
SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(false);Ş.ClearWaypoints();ĵ=false;Ķ=false;ĳ=false;G=15;Ä="Mining";
}g=(Ş.GetPosition()-ų).Length();if((g<=drillSetLength-Ŗ)||ş.IsConnected||Ĉ){Ć=true;}else{Ć=false;}if((g<=drillSetLength-Ŗ
+terrainclearoffset)||ş.IsConnected||Ĉ||Ő){ŏ=false;}else{ŏ=true;}if(R==6&&!ı&&!Ĳ&&ė&&İ&&Ċ&&!Ě&&Ĝ){R=7;ć=false;ı=true;Ĳ=
false;Ȗ();if(ū.Enabled){ū.Enabled=false;}if(Ş.SpeedLimit!=exit_speed){Ş.SpeedLimit=exit_speed;}if(ą){Ź=Vector3D.Normalize(new
Vector3D(-(ů-ż)));}else if(!ą){Ź=Vector3D.Normalize(new Vector3D(ź));}Vector3D ɑ=Ź*drill_el;Vector3D ɒ=Ź*req_dist;Ŵ.Y=Math.Round
(Ų.Y-ɑ.Y,2);Ŵ.X=Math.Round(Ų.X-ɑ.X,2);Ŵ.Z=Math.Round(Ų.Z-ɑ.Z,2);G=17;Ä="Exit path";}if(R==7&&ı&&!Ĳ&&İ&&Ċ&&!Ě&&Ĝ){if(!ć){R
=8;ć=true;if(ą){Ÿ=Vector3D.Normalize(new Vector3D(-(ů-ż)));}else if(!ą){Ÿ=Vector3D.Normalize(new Vector3D(ź));}Vector3D ɓ
=Ÿ*req_dist;ŵ.X=Math.Round(Ż.X-ɓ.X,2);ŵ.Y=Math.Round(Ż.Y-ɓ.Y,2);ŵ.Z=Math.Round(Ż.Z-ɓ.Z,2);Ş.ClearWaypoints();Ş.
AddWaypoint(ŵ,"exit shaft");G=18;Ä="Exiting mineshaft";}}if(R==8&&ć&&!Ĳ&&ė&&İ&&Ċ&&!Ě&&Ĝ){R=9;Ş.SetCollisionAvoidance(false);Ş.
SetDockingMode(false);Ş.SetAutoPilotEnabled(!ĺ);G=18;Ä="Exiting mineshaft";}if(R==9&&ı&&!Ĳ&&ė&&İ&&Ċ&&Ż!=Ŵ&&Ş.CurrentWaypoint.Name!=
"exit shaft"&&!Ě&&Ĝ){Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(!ĺ);G=18;Ä="Exiting mineshaft";if(
S>0){Ä="Returning to dock";}}if(R==9&&ı&&!Ĳ&&ė&&İ&&Ċ&&Ż!=Ŵ&&!Ş.IsAutoPilotEnabled&&!Ě&&Ĝ){Ş.SetCollisionAvoidance(false);
Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(!ĺ);G=18;Ä="Exiting mineshaft reloading WP";if(S>0){Ä="Returning to dock";}
}if(R==9&&ı&&!Ĳ&&ė&&İ&&!Ş.IsAutoPilotEnabled&&!Ě&&Ĝ&&ć){R=7;ć=false;Ä="Exiting mineshaft reloading WP 2";if(S>0){Ä=
"Returning to dock";}}double ɔ=Ŵ.X;double ɕ=Ŵ.Y;double ɖ=Ŵ.Z;if(R==9&&!Ĳ&&ı&&Ż.X>=Ŵ.X-nav_prec2&&Ż.X<=Ŵ.X+nav_prec2&&Ż.Y>=Ŵ.Y-nav_prec2&&Ż.
Y<=Ŵ.Y+nav_prec2&&Ż.Z>=Ŵ.Z-nav_prec2&&Ż.Z<=Ŵ.Z+nav_prec2&&İ&&ė&&Ċ&&!Ě&&Ĝ){R=10;Ĳ=true;ı=true;Ş.SetCollisionAvoidance(
false);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(false);G=19;Ä="Exit Clear";}if(R==9&&!Ĳ&&ı&&g>=(drillSetLength+drill_el)
&&İ&&ė&&Ċ&&!Ě&&Ĝ){R=10;Ĳ=true;ı=true;Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(false);G
=19;Ä="Exit Clear";}if(R==9&&!Ĳ&&ı&&Ż.X>=ŵ.X-nav_prec&&Ż.X<=ŵ.X+nav_prec&&Ż.Y>=ŵ.Y-nav_prec&&Ż.Y<=ŵ.Y+nav_prec&&Ż.Z>=ŵ.Z-
nav_prec&&Ż.Z<=ŵ.Z+nav_prec&&İ&&ė&&Ċ&&!Ě&&Ĝ){R=7;ć=false;Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.
SetAutoPilotEnabled(false);G=19;Ä="Getting next WP";}if(R==9&&ė&&Ş.CurrentWaypoint.Name!="exit shaft"&&Ċ&&İ&&!ı&&!Ĳ&&Č&&!Ě&&Ĝ){Ş.
ClearWaypoints();Ş.AddWaypoint(ŵ,"exit shaft");Ş.SetCollisionAvoidance(false);Ş.SetDockingMode(false);Ş.SetAutoPilotEnabled(!ĺ);Ä=
"Exiting mineshaft";}if(R==10&&ı&&Ĳ&&İ&&Ċ&&!Ě&&Ĝ){R=11;Ş.ClearWaypoints();ı=false;Ĳ=false;Ċ=false;G=20;Ä="Exit Clear";}if(R==11&&Ĵ&&ė&&İ&&!
Ċ&&!Ě&&Ĝ){Ş.ClearWaypoints();ı=false;Ĳ=false;Ċ=false;if(ė){ĝ=true;}if(!Ę||!ę){Ȗ();}G=21;Q=0;if(ũ!=null){ũ.Enabled=true;}
if(collisionSenseEnabled){if(ŭ!=null){if(!ŭ.Enabled){if(!ŭ.Enabled){ŭ.Enabled=true;}}}}if(Ũ!=null){if(!Ũ.Enabled){Ũ.
Enabled=true;}}if(ŧ!=null){if(ŧ.Enabled){ŧ.Enabled=false;}}if(Ʌ){S=1;Ä="RTB Request A";Ŀ=false;O=0;k=0;ė=false;}else{S=0;R=12;Ä
="Preparing A";}}if(R==11&&!Ĵ&&ė&&İ&&!Ċ&&!Ě&&Ĝ){Ş.ClearWaypoints();ı=false;Ĳ=false;G=22;Ċ=false;Q=0;if(ė){ĝ=true;}if(!Ę||
!ę){Ȗ();}if(ũ!=null){if(!ũ.Enabled){ũ.Enabled=true;}}if(Ū!=null){if(Ū.Enabled){Ū.Enabled=false;}}if(collisionSenseEnabled
){if(ŭ!=null){if(!ŭ.Enabled){if(!ŭ.Enabled){ŭ.Enabled=true;}}}}if(Ũ!=null){if(!Ũ.Enabled){Ũ.Enabled=true;}}if(ŧ!=null){if
(ŧ.Enabled){ŧ.Enabled=false;}}if(Ʌ){S=1;Ä="RTB Request B";Ŀ=false;O=0;k=0;}else{S=0;R=13;Ä="Preparing B";}}if(R==12&&ė&&İ
&&!Ċ&&!Ě&&Ĝ&&Ĵ){O=0;k=0;Ä="RTB Ready A";G=26;Ŀ=true;}if(R==13&&ė&&İ&&!Ċ&&!Ě&&Ĝ&&!Ĵ){O=0;k=0;Ä="RTB Ready B";G=27;Ŀ=true;}}
void ɍ(){Ű.X=ű.X;Ű.Y=ű.Y;Ű.Z=ű.Z;Vector3D ɗ=ų-Ų;double ɘ=ɗ.Length();if(ɘ==0){Ƶ.AppendLine(
"Error: Drill path length is zero!");return;}ɗ.Normalize();Vector3D ə=Ż-Ų;double ɚ=Vector3D.Dot(ə,ɗ);double ɛ=ɚ/ɘ;ɛ=Math.Max(0,Math.Min(1,ɛ));Vector3D ɜ=Ų+
ɗ*ɚ;Vector3D ɝ=Ż-ɜ;double ɞ=Vector3D.Dot(ɝ,ɗ);Vector3D ɟ=ɝ-(ɗ*ɞ);double ɠ=ɟ.Length();if(ɠ>Y*2){Ƶ.AppendLine(
$"Drift: {ɠ:F2}m - Correcting");Ű=ɜ;}}void Ɍ(){if(ą){Ÿ=Vector3D.Normalize(new Vector3D(-(ů-ż)));}if(!ą){Ÿ=Vector3D.Normalize(new Vector3D(ź));}
Vector3D ɓ=Ÿ*req_dist;ű.X=Math.Round(Ż.X+ɓ.X,2);ű.Y=Math.Round(Ż.Y+ɓ.Y,2);ű.Z=Math.Round(Ż.Z+ɓ.Z,2);}public void Ǧ(bool Ǎ,bool Ʌ
){if(!Ǎ){if(S>0){S=0;}return;}if(((Ǎ)&&S==0&&!ě)||((Ǎ)&&S==3&&!ě&&(((!ţ.GetValue<bool>(Ž)&&!ţ.GetValue<bool>(Ñ))&&Ä==
"Idle")))){S=1;if(!Ņ){Ƚ(true);Ņ=true;ń=false;}if(ť!=null){if(ť.SpeedLimit!=nav_speed){ť.SpeedLimit=nav_speed;}}}if(ū!=null){if
(ū.Enabled&&S>0){if(ş!=null){ş.Enabled=false;}Ȗ();if(ū.Enabled){if(ş!=null){ş.Enabled=true;}ū.Enabled=false;}S=1;Ä=
"Reset Docking Sequence";if(!Ũ.Enabled){Ũ.Enabled=true;}if(ŧ.Enabled){ŧ.Enabled=false;}}}if(Ū!=null){if(S>0&&Ū.Enabled){if(collisionSenseEnabled
){if(ŭ!=null){if(ŭ.Enabled){if(ŭ.Enabled){ŭ.Enabled=false;}}}}if(ũ!=null){if(ũ.Enabled){ũ.Enabled=false;}}if(ť!=null){if(
!ť.PrecisionMode){ť.PrecisionMode=true;}if(ť.CollisionAvoidance){ť.CollisionAvoidance=false;}}}}if(Ū!=null){if(S>0&&!Ū.
Enabled){if(ť!=null){ť.PrecisionMode=false;}}}if(S>0&&ũ.Enabled){if(ť!=null){if(!ť.CollisionAvoidance){ť.CollisionAvoidance=
true;}}}if(S==1){if(ş!=null){if(!ş.Enabled){ş.Enabled=true;}}Ȕ(false,Ć);if(Ŕ){Ŕ=false;}if(Ũ!=null){if(!Ũ.Enabled){if(ş!=null
){if(!ş.IsConnected&&ş.Status!=MyShipConnectorStatus.Connectable){Ũ.Enabled=true;}}}}if(ş!=null){if(ş.Status!=
MyShipConnectorStatus.Connectable&&S==1){if(!skip_prec_mode){if(ť!=null){if(!ť.PrecisionMode){ť.PrecisionMode=true;}}}if(ť!=null){if(!ť.
CollisionAvoidance){ť.CollisionAvoidance=true;}if(ť.SpeedLimit!=nav_speed){ť.SpeedLimit=nav_speed;}}if(ť!=null){if((skip_prec_mode&&ť.
CollisionAvoidance)||(!skip_prec_mode&&ť.PrecisionMode&&ť.CollisionAvoidance)){if(!ť.GetValue<bool>(Ñ)){ť.ApplyAction(Ó);}if(ţ!=null){if(!
ţ.GetValue<bool>(Ñ)){ţ.ApplyAction(Ó);}if(!ţ.GetValue<bool>(Ž)){ţ.ApplyAction(Ô);}}if(ũ!=null){if(!ũ.Enabled){ũ.Enabled=
true;}}if(collisionSenseEnabled){if(ŭ!=null){if(!ŭ.Enabled){if(!ŭ.Enabled){ŭ.Enabled=true;}}}}}}S=2;Ä="Docking";}else{S=2;Ä=
"Returning to dock";}}}if(ŕ&&S==2){if(h>currentSpeedNotMovingThreshold){if(O>0){O=0;}if(ŕ){ŕ=false;}}}if(ŕ&&S==2){if(Ū!=null){if(Ū.Enabled)
{Ū.Enabled=false;}}if(ũ!=null){if(!ũ.Enabled){ũ.Enabled=true;}}if(ť!=null){if(!ť.CollisionAvoidance){ť.CollisionAvoidance
=true;}if(ť.PrecisionMode){ť.PrecisionMode=false;}}if(collisionSenseEnabled&&ŭ!=null){if(!ŭ.Enabled){ŭ.Enabled=true;}}}if
(S==2&&ŕ){if(ū!=null){if(ū.Enabled){if(ŕ){ŕ=false;}}}else{ŕ=false;}}if(S==2){IMyAutopilotWaypoint ɡ=ť.CurrentWaypoint;if(
ū!=null&&ş!=null&&ŭ!=null&&Ū!=null){if((ş.Status!=MyShipConnectorStatus.Connectable&&!Ū.Enabled&&ŭ.Enabled&&!ū.Enabled&&h
<currentSpeedNotMovingThreshold&&O<F)||(ş.Status!=MyShipConnectorStatus.Connectable&&Ū.Enabled&&!ŭ.Enabled&&!ū.Enabled&&h
<currentSpeedNotMovingThreshold&&O<F)||(ş.Status!=MyShipConnectorStatus.Connectable&&Ū.Enabled&&!ŭ.Enabled&&!ū.Enabled&&h
<currentSpeedNotMovingThreshold&&O<F)||(ş.Status!=MyShipConnectorStatus.Connectable&&!Ū.Enabled&&ŭ.Enabled&&!ū.Enabled&&h
<currentSpeedNotMovingThreshold&&O<F)){O++;k=Math.Round(((double)O*(double)10*X)/(double)1000,1);}}if(ť!=null){if(ť.
SpeedLimit!=nav_speed){ť.SpeedLimit=nav_speed;}if(!ť.GetValue<bool>(Ñ)){ť.ApplyAction(Ó);}}Ȕ(false,Ć);if(Ŕ){Ŕ=false;}if(ş!=null){
if(ş.Status==MyShipConnectorStatus.Connectable&&S==2){ş.Connect();ĝ=true;Ä="Docked";T=0;}if(ş.Status==
MyShipConnectorStatus.Connected&&S==2){Ƚ(false);S=3;}}if(ş!=null&&ţ!=null&&Ū!=null){if(ş.Status!=MyShipConnectorStatus.Connectable&&S==2&&!ħ
&&(!ţ.GetValue<bool>(Ž)&&(!ţ.GetValue<bool>(Ñ)))&&!Ū.Enabled){if(collisionSenseEnabled){if(ŭ!=null){if(!ŭ.Enabled){ŭ.
Enabled=!ŭ.Enabled;}}}if(ť!=null){if(!ť.GetValue<bool>(Ñ)){ť.ApplyAction(Ó);}}if(!ţ.GetValue<bool>(Ñ)){ţ.ApplyAction(Ó);}if(!ţ.
GetValue<bool>(Ž)){ţ.ApplyAction(Ô);}Ä="Docking";}}if(ş!=null&&ţ!=null){if(ş.Status!=MyShipConnectorStatus.Connectable&&S==2&&ħ
&&(ţ.GetValue<bool>(Ž)&&(ţ.GetValue<bool>(Ñ)))){if(Ū!=null){Ū.Enabled=false;}if(ő){if(ť!=null){if(ť.PrecisionMode&&!Œ){ť.
PrecisionMode=false;Œ=true;}if(!ť.PrecisionMode&&!Œ){ť.PrecisionMode=true;Œ=true;}}}if(!ţ.GetValue<bool>(Ñ)){ţ.ApplyAction(Ó);}if(!ţ.
GetValue<bool>(Ž)){ţ.ApplyAction(Ô);}}}if(ş!=null&&ţ!=null){if(ş.Status!=MyShipConnectorStatus.Connectable&&S==2&&ħ&&(!ţ.
GetValue<bool>(Ž)&&(!ţ.GetValue<bool>(Ñ)||(ţ.GetValue<bool>(Ñ))))){if(ū!=null){if(!ū.Enabled){ū.Enabled=true;}}if(Ū!=null){Ū.
Enabled=false;}if(ő){if(ť!=null){if(ť.PrecisionMode&&!Œ){ť.PrecisionMode=false;Œ=true;}if(!ť.PrecisionMode&&!Œ){ť.PrecisionMode
=true;Œ=true;}}}}}}if(ő&&S==2){if(ť!=null){if(ť.PrecisionMode&&Œ){ť.PrecisionMode=false;Œ=false;ő=false;}if(!ť.
PrecisionMode&&Œ){ť.PrecisionMode=true;Œ=false;ő=false;}}}if(S==3&&ě){O=0;Ȕ(false,Ć);if(Ŕ){Ŕ=false;}if(!ŀ){if(Ţ!=null){if(!Ţ.Enabled)
{Ţ.Enabled=true;}Ţ.Trigger();}}else{Ƚ(false);}Ȗ();if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}if(ŧ!=null){if(!ŧ.Enabled){
ŧ.Enabled=true;}}if(Ũ!=null){if(Ũ.Enabled){Ũ.Enabled=false;}}if(Ū!=null){if(Ū.Enabled){Ū.Enabled=false;}}if(ĥ){for(int ȓ=
0;ȓ<Ɵ.Count;ȓ++){if(!ņ){Ȼ(ChargeMode.Recharge);ņ=true;Ň=false;}Ä="Recharging";}}if(!ĥ){if(!Ň){Ȼ(ChargeMode.Auto);ņ=false;
Ň=true;}}if(Ĥ&&!ignore_Htank){if(!ň){ȷ(true);ŉ=false;ň=true;}Ä="Recharging";}if(!Ĥ&&!ignore_Htank){if(!ŉ){ȷ(false);ŉ=true
;ň=false;}}if(!Ė){S=0;}}if(S>=1&&S<=2&&Č&&Ę&&!ě){Ȗ();if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}S=0;}}public void ǧ(bool
Ŀ){if(ş==null){Ŀ=false;return;}if(ş.IsConnected&&Ŀ){Ŀ=false;}if((ş.IsConnected&&ignore_Htank)||(ş.IsConnected&&!
ignore_Htank)){if(!ň){ȷ(true);ŉ=false;ň=true;}}if(!ş.IsConnected&&ignore_Htank){if(!ŉ){ȷ(false);ŉ=true;ň=false;}}if((ş.IsConnected&&
ē)||(ş.IsConnected&&!Ē)){Ä="Docked Unloading";if(collisionSenseEnabled){if(ŭ!=null){if(ŭ.Enabled){ŭ.Enabled=false;}}}if(ũ
!=null){if(ũ.Enabled){ũ.Enabled=false;}}if(ū!=null){if(ū.Enabled){ū.Enabled=false;}}}if(ş.IsConnected&&Ė){Ä=
"Docked Recharging";if(collisionSenseEnabled){if(ŭ!=null){if(ŭ.Enabled){ŭ.Enabled=false;}}}if(ũ!=null){if(ũ.Enabled){ũ.Enabled=false;}}if(ū
!=null){if(ū.Enabled){ū.Enabled=false;}}}if(ş.IsConnected&&!Đ&&!ē&&Ē&&!Ė){Ä="Docked Idle";if(collisionSenseEnabled){if(ŭ!=
null){if(ŭ.Enabled){ŭ.Enabled=false;}}}if(ũ!=null){if(ũ.Enabled){ũ.Enabled=false;}}if(ū!=null){if(ū.Enabled){ū.Enabled=false
;}}}if(!ş.IsConnected){if(ŧ!=null){if(ŧ.Enabled){ŧ.Enabled=false;}}}if(ş.IsConnected){if(ŧ!=null){if(!ŧ.Enabled){ŧ.
Enabled=true;}}if(Ũ!=null){if(Ũ.Enabled){Ũ.Enabled=false;}}if(!ń&&ŀ){Ƚ(false);ń=true;Ņ=false;}}}public void Ǫ(){if(į){į=false;l
=Math.Round(((double)M*(double)10*X)/(double)1000,1);M=0;}}public void ǫ(){if((!ę||Ĝ)&&j>0.0){j=0;N=0;}if(Ħ){j=Math.Round
(((double)N*(double)10*X)/(double)1000,1);Ħ=false;N=0;}}public void Ǭ(){if((!Ę||ě)&&k>0.0){k=0;O=0;}if(ħ){k=Math.Round(((
double)O*(double)10*X)/(double)1000,1);ħ=false;O=0;}}public void ǩ(bool Ʌ,IMyRemoteControl ɢ,IMyRadioAntenna Š,bool Ŀ){string
ɣ;if(Š==null){Ƶ.AppendLine("Error: antenna is null in drone_message_transmission_management");return;}if(ɢ==null){Ƶ.
AppendLine("Error: remote control is null in drone_message_transmission_management");return;}if(Ĩ&&ă){i=Math.Round(((double)L*(
double)10*X)/(double)1000,1);Ĩ=false;L=0;}if(ă){const string ɤ=
"{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}:{12}:{13}:{14}:{15}:{16}:{17}:{18}:{19}:{20}:{21}:{22}:{23}:{24}:";ƴ.Clear().EnsureCapacity(128);ƴ.AppendFormat(ɤ,p,Ã,ġ,Ä,ě,Ĝ,Ě,ɢ.IsAutoPilotEnabled,Math.Round(Ż.X,2),Math.Round(Ż.Y,2),
Math.Round(Ż.Z,2),drillSetLength,Math.Round(g,2),Math.Round(drillSetLength-Ŗ,2),Math.Round(Ă,2),Math.Round(n,2),Math.Round(Ř
,2),ŝ,ē,Ė,Ʌ,Ŀ,H,B,ŕ);ɣ=ƴ.ToString();IGC.SendBroadcastMessage(Ì,ɣ,TransmissionDistance.TransmissionDistanceMax);Ƶ.
AppendLine("Transmission sent");ɣ="";ă=false;É="";}}public void ǣ(){if(J==1&&P==0){P=1;Ğ=false;}if(!Ğ&&J==1){Ğ=true;ĭ=false;Ķ=
false;Ş.ClearWaypoints();Ş.SetAutoPilotEnabled(false);}if(Ğ&&J==1){if(Ď&&ċ&&Ĝ){Q=1;Ä="Nav";}if(č&&ċ&&Ĝ&&!Ŀ){R=0;İ=false;}}}
public void Ǯ(){double ɥ=Math.Round((o/X)*100.0,3);double ɦ=Math.Round(o,3);double ɧ=Math.Round(Ř,2);double ɨ=Math.Round(Ă,2);
double ɩ=ơ.Count>0?Math.Round(n,2):0;double ɪ=Math.Round(g,2);double ɫ=Math.Round(m,2);double ɬ=Math.Round(h,2);Ƶ.Append(
"Load: ").Append(ɥ).Append("% (").Append(ɦ).Append("ms) I#: ").Append(U).Append(" ").Append(nav_speed).Append('\n');Ƶ.Append(
"Drone ID: ").Append(p.Replace("[","[[").Replace("]","]]")).Append(" # ").Append(Ã).Append('\n');Ƶ.Append("Status Ints: ").Append(Ë)
.Append('\n');Ƶ.Append("Drone Status: ").Append(Ä).Append('\n');Ƶ.Append("Distance ID: ").Append(İ).Append('\n');Ƶ.Append
("Command seq: ").Append(H).Append(" AIFault: ").Append(ŕ).Append('\n');Ƶ.Append("Cargo: ").Append(ɧ).Append("%  Full: ")
.Append(ē).Append('\n');Ƶ.Append("Charge: ").Append(ɨ).Append("%  Recharge: ").Append(ĥ).Append('\n');if(ơ.Count>0){Ƶ.
Append("HTank: ").Append(ɩ).Append("%  Recharge: ").Append(Ĥ).Append('\n');}Ƶ.Append("Mine distance: ").Append(ɪ).Append(
"m  Mine Start: ").Append(drillSetLength-Ŗ).Append("m\n");Ƶ.Append("Mine: ").Append(č).Append(" - Stage: ").Append(R).Append(" WM:").
Append(ė).Append(" - Ed: ").Append(drill_el).Append('\n');Ƶ.Append("Nav: ").Append(Ď).Append(" - Stage: ").Append(Q).Append(
'\n');Ƶ.Append("Dock: ").Append(ě).Append(" - Stage: ").Append(S).Append(" DR: ").Append(Ŀ).Append('\n');Ƶ.Append("Undock: "
).Append(Ĝ).Append(" - Stage: ").Append(T).Append('\n');Ƶ.Append("Connected: ").Append(ş.IsConnected).Append('\n');Ƶ.
Append("Depth Achieved: ").Append(Ĵ).Append('\n');Ƶ.Append("Stopped: ").Append(Č).Append('\n');Ƶ.Append("Last response: ").
Append(i).Append("s waiting: ").Append(Ĩ).Append('\n');Ƶ.Append("Undock timer: ").Append(j).Append("s ").Append(Ħ).Append('\n'
);Ƶ.Append("Dock timer: ").Append(k).Append("s ").Append(ħ).Append(" GRef: ").Append(W).Append('\n');Ƶ.Append(
"Nav timer: ").Append(l).Append("s ").Append(į).Append('\n');Ƶ.Append("Speed: ").Append(ɫ).Append(" ").Append(ɬ).Append('\n');if(ƨ.
Count>0){for(int ȓ=0;ȓ<ƨ.Count;ȓ++){if(ƨ[ȓ]!=null){Ƴ=((IMyTextSurfaceProvider)ƨ[0]).GetSurface(0);if(Ƴ.ContentType!=
ContentType.TEXT_AND_IMAGE){Ƴ.ContentType=ContentType.TEXT_AND_IMAGE;Ƴ.FontSize=0.66f;Ƴ.Font="White";}Ƴ.WriteText(Ƶ.ToString());}}}
if(Ʃ.Count>0){for(int ȓ=0;ȓ<Ʃ.Count;ȓ++){if(Ʃ[ȓ]!=null){Ƴ=((IMyTextSurfaceProvider)Ʃ[0]).GetSurface(0);if(Ƴ.ContentType!=
ContentType.TEXT_AND_IMAGE){Ƴ.ContentType=ContentType.TEXT_AND_IMAGE;Ƴ.FontSize=0.66f;Ƴ.Font="White";}Ƴ.WriteText(Ƶ.ToString());}}}
if(ƨ.Count==0){if(V%10==0){Echo(Ƶ.ToString());}}Ƶ.Clear();}public void ǯ(){L++;if(L>=C){Ĩ=true;}if(M>=D){į=true;}if(N>=E){
Ħ=true;}if(O>=F){ħ=true;if(!ŕ){ŕ=true;}}}public void Ƚ(bool ɭ){IMyGridTerminalSystem Ȣ=GridTerminalSystem as
IMyGridTerminalSystem;Ʀ.Clear();Ȣ.GetBlocksOfType<IMyThrust>(Ʀ,ȕ=>ȕ.CubeGrid==Me.CubeGrid);if(Ʀ.Count>0){ŀ=true;}else{ļ=false;ŀ=false;return;
}if(Ʀ.Count>0){for(int ȓ=0;ȓ<Ʀ.Count;ȓ++){if(Ʀ[ȓ]!=null){if(Ʀ[ȓ].Enabled!=ɭ){Ʀ[ȓ].Enabled=ɭ;}}}}else{if(ư!=null){Ƶ.
AppendLine($"Thrusters not found in {ư.Name.Replace("[","[[").Replace("]","]]")}. Please add thrusters");}return;}}void Ȼ(
ChargeMode ɮ){if(Ɵ.Count>0){for(int ȓ=0;ȓ<Ɵ.Count;ȓ++){if(Ɵ[ȓ]!=null){if(Ɵ[ȓ].ChargeMode!=ɮ){Ɵ[ȓ].ChargeMode=ɮ;}}}}}void ȷ(bool ɯ)
{if(ơ.Count>0){for(int ȓ=0;ȓ<ơ.Count;ȓ++){if(ơ[ȓ]!=null){if(ơ[ȓ].Stockpile!=ɯ){ơ[ȓ].Stockpile=ɯ;}}}}}public void ȅ(string
ɰ,IMyTerminalBlock ȃ,string ɱ="GMDCJobData",string ɲ="Jobinfo"){var ɳ=new MyIni();ɳ.Clear();if(ɳ.TryParse(ȃ.CustomData.
ToString())){ɳ.Set(ɱ,ɲ,ɰ);}else{ɳ.Set(ɱ,ɲ,ɰ);}ȃ.CustomData=ɳ.ToString();ɳ.Clear();}public void ǉ(){ɴ<IMyRemoteControl>(ž,ſ);ɴ<
IMySensorBlock>(ƀ,Ɓ);ɴ<IMyCameraBlock>(Ƃ,ƃ);ɴ<IMyShipConnector>(Ƅ,ƅ);ɴ<IMyCargoContainer>(Ɔ,Ƈ,ƈ);ɴ<IMyRadioAntenna>(Ɖ,Ɗ);ɴ<
IMyPathRecorderBlock>(ƍ,Ǝ,Ə);ɴ<IMyFlightMovementBlock>(Ɛ,Ƒ);ɴ<IMyTimerBlock>(ƒ,Ɠ,Ɣ,ƕ,Ɩ);ɴ<IMyLightingBlock>(Ɨ,Ƙ,ƙ,ƚ,ƛ,Ɯ,Ɲ);ɴ<IMyBatteryBlock
>(ƞ,Ɵ);ɴ<IMyGasTank>(Ơ,ơ);ɴ<IMyShipDrill>(Ƣ,ƣ);ɴ<IMyThrust>(ƥ,Ʀ);ɴ<IMyGyro>(Ʊ,Ʋ);if(Ƥ!=null&&Ƥ.Count>0){Ƥ.Clear();}}void
ɴ<ȩ>(params List<ȩ>[]ɵ)where ȩ:class{foreach(var Ȫ in ɵ){if(Ȫ!=null&&Ȫ.Count>0){Ȫ.Clear();}}}double Ȉ(string[]ɶ,int ɷ,
double ɸ){double ɹ;if(ɷ<ɶ.Length&&double.TryParse(ɶ[ɷ],out ɹ))return ɹ;return ɸ;}int Ȍ(string[]ɶ,int ɷ,int ɸ){int ɹ;if(ɷ<ɶ.
Length&&int.TryParse(ɶ[ɷ],out ɹ))return ɹ;return ɸ;}double ȍ(string[]ɶ,int ɷ,out bool ɺ){double ɹ;if(ɷ<ɶ.Length&&!string.
IsNullOrEmpty(ɶ[ɷ])&&double.TryParse(ɶ[ɷ],out ɹ)){ɺ=true;return ɹ;}ɺ=false;return 0.0;}
