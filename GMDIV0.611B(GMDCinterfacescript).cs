// R e a d m e
// -----------
// 
// program start
// GMDI Drone controller Interface V0.611B
    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }
    //change this
    string drone_tag = "SWRM_D";
    int scnpanel = 0; //display surface: 0 = default, 0+ range

string A="confirm";string B="incrval",C="decrval",D="incrsel",E="itemdown",F="itemup",G="menu",H="command",I="jobconf",J
="cancel",K="Comms",L="Interface",M="Display",N="",O="",P="",Q="",R="",S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j="",k,l,m,n=
"[ ]",o="[ ]",p="[ ]",q="[ ]",r="[ ]",s="[ ]",t="[ ]",u="[ ]",v="[ ]",w="[ ]",x="[ ]",y="[ ]",z="",ª="",µ="Jobinfo",º=
"GMDCJobData",À="",Á="",Â="",Ã="",Ä="";string Å="V0.611B";int Æ=0;int Ç=0,È=9,É=0,Ê=1,Ë=10,Ì,Í,Î=0,Ï=0,Ð=0,Ñ=0,Ò=0,Ó=0,Ô,Õ,Ö=0,Ø=0,Ù=
0,Ú=0,Û=0,Ü=0,Ý,Þ,ß,à,á,â=0,ã=0,ä=0,å=0,æ=0,ç=0,è=0,é=0,ê=7,ë=0;bool ì=false,í=false,î=false,ï=false,ð=false,ñ=false,ò=
false,ó=false,ô=false,õ=false,ö=false,ø=false,ù=true,ú,û,ü=false,ý;IMyProgrammableBlock þ;string ÿ;string Ā;string ā;double Ă
=0.0,ă,Ą,ą=0.0,Ć=0.0,ć=0.0,Ĉ,ĉ,Ċ,ċ=0.0;Vector3D Č,č;string Ď;int ď;int Đ=0;List<string>đ=new List<string>(),Ē=new List<
string>(),ē=new List<string>(),Ĕ=new List<string>(),ĕ=new List<string>(),Ė=new List<string>(),ė=new List<string>(),Ę=new List<
string>(),ę=new List<string>(),Ě=new List<string>(),ě=new List<string>(),Ĝ=new List<string>(),ĝ=new List<string>(),Ğ=new List<
string>(),ğ=new List<string>(),Ġ=new List<string>();int ġ=0;StringBuilder Ģ=new StringBuilder(),ģ=new StringBuilder(),Ĥ=new
StringBuilder();List<IMyTerminalBlock>ĥ=new List<IMyTerminalBlock>(),Ħ=new List<IMyTerminalBlock>();List<IMyProgrammableBlock>ħ=new
List<IMyProgrammableBlock>(),Ĩ=new List<IMyProgrammableBlock>();List<IMyRadioAntenna>ĩ=new List<IMyRadioAntenna>(),Ī=new
List<IMyRadioAntenna>();List<IMyRemoteControl>ī=new List<IMyRemoteControl>(),Ĭ=new List<IMyRemoteControl>();MyIni ĭ=new
MyIni(),Į=new MyIni(),į=new MyIni(),İ=new MyIni(),ı=new MyIni(),Ĳ=new MyIni();IMyCubeGrid ĳ;List<IMyMotorStator>Ĵ=new List<
IMyMotorStator>();List<IMyMotorAdvancedStator>ĵ=new List<IMyMotorAdvancedStator>();List<IMyPistonBase>Ķ=new List<IMyPistonBase>();List
<IMyTextSurface>ķ=new List<IMyTextSurface>();public void
 Save
(){}public void ļ(IMyTerminalBlock ĸ){if(į.TryParse(ĸ.CustomData)){var Ĺ="";bool ĺ=false;if(į.ContainsSection("GMDCJobID"
)){if(į.ContainsKey("GMDCJobID","Job1Name")){Ĺ=į.Get("GMDCJobID","Job1Name").ToString().Trim();if(Ġ[0]!=null){Ġ[0]=Ĺ;}}
else{if(Ġ[0]!=null){Ġ[0]="";}ĺ=true;}if(į.ContainsKey("GMDCJobID","Job2Name")){Ĺ=į.Get("GMDCJobID","Job2Name").ToString().
Trim();if(Ġ[1]!=null){Ġ[1]=Ĺ;}}else{if(Ġ[1]!=null){Ġ[1]="";}ĺ=true;}if(į.ContainsKey("GMDCJobID","Job3Name")){Ĺ=į.Get(
"GMDCJobID","Job3Name").ToString().Trim();if(Ġ[2]!=null){if(Ġ[2]!=null){Ġ[2]=Ĺ;}}}else{Ġ[2]="";ĺ=true;}if(į.ContainsKey("GMDCJobID"
,"Job4Name")){Ĺ=į.Get("GMDCJobID","Job4Name").ToString().Trim();if(Ġ[3]!=null){Ġ[3]=Ĺ;}}else{if(Ġ[3]!=null){Ġ[3]="";}ĺ=
true;}if(į.ContainsKey("GMDCJobID","Job5Name")){Ĺ=į.Get("GMDCJobID","Job5Name").ToString().Trim();if(Ġ[4]!=null){Ġ[4]=Ĺ;}}
else{if(Ġ[4]!=null){Ġ[4]="";}ĺ=true;}if(į.ContainsKey("GMDCJobID","Job6Name")){Ĺ=į.Get("GMDCJobID","Job6Name").ToString().
Trim();if(Ġ[5]!=null){Ġ[5]=Ĺ;}}else{if(Ġ[5]!=null){Ġ[5]="";}ĺ=true;}if(į.ContainsKey("GMDCJobID","Job7Name")){Ĺ=į.Get(
"GMDCJobID","Job7Name").ToString().Trim();if(Ġ[6]!=null){Ġ[6]=Ĺ;}}else{if(Ġ[6]!=null){Ġ[6]="";}ĺ=true;}if(į.ContainsKey("GMDCJobID"
,"Job8Name")){Ĺ=į.Get("GMDCJobID","Job8Name").ToString().Trim();if(Ġ[7]!=null){Ġ[7]=Ĺ;}}else{if(Ġ[7]!=null){Ġ[7]="";}ĺ=
true;}if(į.ContainsKey("GMDCJobID","Job9Name")){Ĺ=į.Get("GMDCJobID","Job9Name").ToString().Trim();if(Ġ[8]!=null){Ġ[8]=Ĺ;}}
else{if(Ġ[8]!=null){Ġ[8]="";}ĺ=true;}if(į.ContainsKey("GMDCJobID","Job10Name")){Ĺ=į.Get("GMDCJobID","Job10Name").ToString().
Trim();if(Ġ[9]!=null){Ġ[9]=Ĺ;}}else{if(Ġ[9]!=null){Ġ[9]="";}ĺ=true;}}else{ĺ=true;}if(ĺ){Ļ(ĸ);ĸ.CustomData=į.ToString();į.
Clear();}}}public void ń(IMyTerminalBlock ĸ,int Ľ,string I,string ľ,string Ŀ,string ŀ,string Ł,string ł,string Ń){if(ı.
TryParse(ĸ.CustomData.ToString())){if(Ľ==0){ı.Set("GMDCJobID","Job1Name",Ń);ı.Set("GMDCJobID","Job1Info",I);ı.Set("GMDCJobID",
"Job1State",ľ);ı.Set("GMDCJobID","Job1RCInfo",Ŀ);ı.Set("GMDCJobID","Job1RCTarget",ŀ);ı.Set("GMDCJobID","Job1RCAlign",Ł);ı.Set(
"GMDCJobID","Job1RCSafe",ł);}if(Ľ==1){ı.Set("GMDCJobID","Job2Name",Ń);ı.Set("GMDCJobID","Job2Info",I);ı.Set("GMDCJobID","Job2State"
,ľ);ı.Set("GMDCJobID","Job2RCInfo",Ŀ);ı.Set("GMDCJobID","Job2RCTarget",ŀ);ı.Set("GMDCJobID","Job2RCAlign",Ł);ı.Set(
"GMDCJobID","Job2RCSafe",ł);}if(Ľ==2){ı.Set("GMDCJobID","Job3Name",Ń);ı.Set("GMDCJobID","Job3Info",I);ı.Set("GMDCJobID","Job3State"
,ľ);ı.Set("GMDCJobID","Job3RCInfo",Ŀ);ı.Set("GMDCJobID","Job3RCTarget",ŀ);ı.Set("GMDCJobID","Job3RCAlign",Ł);ı.Set(
"GMDCJobID","Job3RCSafe",ł);}if(Ľ==3){ı.Set("GMDCJobID","Job4Name",Ń);ı.Set("GMDCJobID","Job4Info",I);ı.Set("GMDCJobID","Job4State"
,ľ);ı.Set("GMDCJobID","Job4RCInfo",Ŀ);ı.Set("GMDCJobID","Job4RCTarget",ŀ);ı.Set("GMDCJobID","Job4RCAlign",Ł);ı.Set(
"GMDCJobID","Job4RCSafe",ł);}if(Ľ==4){ı.Set("GMDCJobID","Job5Name",Ń);ı.Set("GMDCJobID","Job5Info",I);ı.Set("GMDCJobID","Job5State"
,ľ);ı.Set("GMDCJobID","Job5RCInfo",Ŀ);ı.Set("GMDCJobID","Job5RCTarget",ŀ);ı.Set("GMDCJobID","Job5RCAlign",Ł);ı.Set(
"GMDCJobID","Job5RCSafe",ł);}if(Ľ==5){ı.Set("GMDCJobID","Job6Name",Ń);ı.Set("GMDCJobID","Job6Info",I);ı.Set("GMDCJobID","Job6State"
,ľ);ı.Set("GMDCJobID","Job6RCInfo",Ŀ);ı.Set("GMDCJobID","Job6RCTarget",ŀ);ı.Set("GMDCJobID","Job6RCAlign",Ł);ı.Set(
"GMDCJobID","Job6RCSafe",ł);}if(Ľ==6){ı.Set("GMDCJobID","Job7Name",Ń);ı.Set("GMDCJobID","Job7Info",I);ı.Set("GMDCJobID","Job7State"
,ľ);ı.Set("GMDCJobID","Job7RCInfo",Ŀ);ı.Set("GMDCJobID","Job7RCTarget",ŀ);ı.Set("GMDCJobID","Job7RCAlign",Ł);ı.Set(
"GMDCJobID","Job7RCSafe",ł);}if(Ľ==7){ı.Set("GMDCJobID","Job8Name",Ń);ı.Set("GMDCJobID","Job8Info",I);ı.Set("GMDCJobID","Job8State"
,ľ);ı.Set("GMDCJobID","Job8RCInfo",Ŀ);ı.Set("GMDCJobID","Job8RCTarget",ŀ);ı.Set("GMDCJobID","Job8RCAlign",Ł);ı.Set(
"GMDCJobID","Job8RCSafe",ł);}if(Ľ==8){ı.Set("GMDCJobID","Job9Name",Ń);ı.Set("GMDCJobID","Job9Info",I);ı.Set("GMDCJobID","Job9State"
,ľ);ı.Set("GMDCJobID","Job9RCInfo",Ŀ);ı.Set("GMDCJobID","Job9RCTarget",ŀ);ı.Set("GMDCJobID","Job9RCAlign",Ł);ı.Set(
"GMDCJobID","Job9RCSafe",ł);}if(Ľ==9){ı.Set("GMDCJobID","Job10Name",Ń);ı.Set("GMDCJobID","Job10Info",I);ı.Set("GMDCJobID",
"Job10State",ľ);ı.Set("GMDCJobID","Job10RCInfo",Ŀ);ı.Set("GMDCJobID","Job10RCTarget",ŀ);ı.Set("GMDCJobID","Job10RCAlign",Ł);ı.Set(
"GMDCJobID","Job10RCSafe",ł);}ĸ.CustomData=ı.ToString();}}public void Ŋ(IMyTerminalBlock ĸ,int Ġ){var Ń="";var Ĺ="";var Ņ="";var ņ=
"";var Ň="";var ň="";var ŉ="";į.Clear();İ.Clear();if(į.TryParse(ĸ.CustomData.ToString())){Ĺ=į.Get("GMDCJobData","Jobinfo")
.ToString();Ņ=į.Get("jobdata","gridstatus").ToString();Ń=į.Get("GMDCJobData","jobname").ToString();}else{Ĺ="";Ņ="";}if(Ĭ.
Count>0){if(Ĭ[0]!=null){if(İ.TryParse(Ĭ[0].CustomData.ToString())){ņ=İ.Get("GMDCJobData","Jobinfo").ToString();Ň=İ.Get(
"GMDCJobData","TargetGPS").ToString();ň=İ.Get("GMDCJobData","AlignGPS").ToString();ŉ=İ.Get("GMDCJobData","SafeAlignDistance").
ToString();}else{ņ="";Ň="";ň="";ŉ="";}}}ı.Clear();ń(ĸ,Ġ,Ĺ,Ņ,ņ,Ň,ň,ŉ,Ń);ı.Clear();į.Clear();İ.Clear();ļ(ĸ);}public void ŋ(
IMyTerminalBlock ĸ,int Ġ){var Ĺ="";var Ņ="";var ņ="";var Ň="";var ň="";var ŉ="";var Ń="";į.Clear();İ.Clear();if(į.TryParse(ĸ.CustomData.
ToString())){if(Ġ==0){Ń=į.Get("GMDCJobID","Job1Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job1Info").ToString().Trim();Ņ=į.
Get("GMDCJobID","Job1State").ToString().Trim();ņ=į.Get("GMDCJobID","Job1RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job1RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job1RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job1RCSafe").ToString().
Trim();}if(Ġ==1){Ń=į.Get("GMDCJobID","Job2Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job2Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job2State").ToString().Trim();ņ=į.Get("GMDCJobID","Job2RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job2RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job2RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job2RCSafe").ToString().
Trim();}if(Ġ==2){Ń=į.Get("GMDCJobID","Job3Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job3Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job3State").ToString().Trim();ņ=į.Get("GMDCJobID","Job3RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job3RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job3RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job3RCSafe").ToString().
Trim();}if(Ġ==3){Ń=į.Get("GMDCJobID","Job4Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job4Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job4State").ToString().Trim();ņ=į.Get("GMDCJobID","Job4RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job4RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job4RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job4RCSafe").ToString().
Trim();}if(Ġ==4){Ń=į.Get("GMDCJobID","Job5Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job5Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job5State").ToString().Trim();ņ=į.Get("GMDCJobID","Job5RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job5RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job5RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job5RCSafe").ToString().
Trim();}if(Ġ==5){Ń=į.Get("GMDCJobID","Job6Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job6Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job6State").ToString().Trim();ņ=į.Get("GMDCJobID","Job6RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job6RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job6RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job6RCSafe").ToString().
Trim();}if(Ġ==6){Ń=į.Get("GMDCJobID","Job7Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job7Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job7State").ToString().Trim();ņ=į.Get("GMDCJobID","Job7RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job7RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job7RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job7RCSafe").ToString().
Trim();}if(Ġ==7){Ń=į.Get("GMDCJobID","Job8Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job8Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job8State").ToString().Trim();ņ=į.Get("GMDCJobID","Job8RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job8RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job8RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job8RCSafe").ToString().
Trim();}if(Ġ==8){Ń=į.Get("GMDCJobID","Job9Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job9Info").ToString().Trim();Ņ=į.Get
("GMDCJobID","Job9State").ToString().Trim();ņ=į.Get("GMDCJobID","Job9RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job9RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job9RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job9RCSafe").ToString().
Trim();}if(Ġ==9){Ń=į.Get("GMDCJobID","Job10Name").ToString().Trim();Ĺ=į.Get("GMDCJobID","Job10Info").ToString().Trim();Ņ=į.
Get("GMDCJobID","Job10State").ToString().Trim();ņ=į.Get("GMDCJobID","Job10RCInfo").ToString().Trim();Ň=į.Get("GMDCJobID",
"Job10RCTarget").ToString().Trim();ň=į.Get("GMDCJobID","Job10RCAlign").ToString().Trim();ŉ=į.Get("GMDCJobID","Job10RCSafe").ToString().
Trim();}}else{Ĺ="";Ņ="";ņ="";Ň="";ň="";ŉ="";}if(!string.IsNullOrEmpty(Ĺ)&&!string.IsNullOrEmpty(Ņ)){į.Set("GMDCJobData",
"Jobinfo",Ĺ);į.Set("jobdata","gridstatus",Ņ);į.Set("GMDCJobData","loadsave","true");į.Set("GMDCJobData","jobname",Ń);ĸ.CustomData
=į.ToString();}į.Clear();if(!string.IsNullOrEmpty(ņ)&&!string.IsNullOrEmpty(Ň)&&!string.IsNullOrEmpty(ň)&&!string.
IsNullOrEmpty(ŉ)){if(Ĭ.Count>0){if(Ĭ[0]!=null){if(İ.TryParse(Ĭ[0].CustomData.ToString())){İ.Set("GMDCJobData","Jobinfo",ņ);İ.Set(
"GMDCJobData","TargetGPS",Ň);İ.Set("GMDCJobData","AlignGPS",ň);İ.Set("GMDCJobData","SafeAlignDistance",ŉ);Ĭ[0].CustomData=İ.ToString(
);}}}}İ.Clear();}public void Ļ(IMyTerminalBlock ĸ){if(Ġ.Count>0){if(Ġ[0]!=null){į.Set("GMDCJobID","Job1Name",Ġ[0]);}if(Ġ[
1]!=null){į.Set("GMDCJobID","Job2Name",Ġ[1]);}if(Ġ[2]!=null){į.Set("GMDCJobID","Job3Name",Ġ[2]);}if(Ġ[3]!=null){į.Set(
"GMDCJobID","Job4Name",Ġ[3]);}if(Ġ[4]!=null){į.Set("GMDCJobID","Job5Name",Ġ[4]);}if(Ġ[5]!=null){į.Set("GMDCJobID","Job6Name",Ġ[5]);
}if(Ġ[6]!=null){į.Set("GMDCJobID","Job7Name",Ġ[6]);}if(Ġ[7]!=null){į.Set("GMDCJobID","Job8Name",Ġ[7]);}if(Ġ[8]!=null){į.
Set("GMDCJobID","Job9Name",Ġ[8]);}if(Ġ[9]!=null){į.Set("GMDCJobID","Job10Name",Ġ[9]);}}}public void ō(string Ō){if(!string.
IsNullOrEmpty(Ō)&&!string.IsNullOrWhiteSpace(Ō)){var Ĺ="";ĭ.Clear();if(ĭ.TryParse(Ō)){Ĺ=ĭ.Get("Configuration","drone group tag").
ToString().Trim();if(!string.IsNullOrEmpty(Ĺ)&&!string.IsNullOrWhiteSpace(Ĺ)){drone_tag=Ĺ;Echo(
$"Drone group tag found: {drone_tag}");}else{Echo("Drone group tag not found. Defaulting");drone_tag="SWRM_D";}Ĺ=ĭ.Get("Configuration","ship grid tag").
ToString().Trim();{P=Ĺ;Echo($"Ship grid tag found: {P.Replace("[","[[").Replace("]","]]")}");}}}else{drone_tag="SWRM_D";P="";
Echo("Storage not found. Defaulting");}ĭ.Clear();}public void Ŏ(){Ĥ.Append("GMDI ").Append(Å).Append(" Running ").AppendLine
(z);Ĥ.AppendLine("");Ĥ.AppendLine("Use the below run arguments to navigate:");Ĥ.AppendLine(
"----------------------------------------");Ĥ.AppendLine("");Ĥ.Append("Confirm = ").AppendLine(A);Ĥ.Append("Change increment = ").AppendLine(D);Ĥ.Append(
"Increase value = ").AppendLine(B);Ĥ.Append("Decrease value = ").AppendLine(C);Ĥ.Append("Main menu = ").AppendLine(G);Ĥ.Append('\n');Ĥ.
AppendLine($"Display tag ({ķ.Count}): {Â} ");Ĥ.AppendLine($"Controller tag: {Ã} ");Ĥ.AppendLine($"Ship tag: {Ä} ");}public void ŏ(
){if(ï){â=1;}else{â=0;}if(ð){ã=1;}else{ã=0;}}public void Ő(){á=0+Û;if(á<0){á=9;}if(á>9){á=0;}Þ=Í+Ð;if(Þ<1){Þ=1;}ď=Ì+Ï;if(
ď<1){ď=1;}Ċ=ă+ć;if(Ċ<0.1){Ċ=0.1;}Ĉ=Ą+ą;if(Ĉ<0.1){Ĉ=0.1;}ĉ=Ă+Ć;if(Ñ<0){Ñ=1;}if(Ñ>1){Ñ=0;}if(Ñ==0){f="No";}if(Ñ==1){f="Yes"
;}Đ=Ñ+â;if(Đ<0){Đ=1;}if(Đ>1){Đ=0;}if(Đ==0){ú=false;}if(Đ==1){ú=true;}if(Đ==0){f="No";}if(Đ==1){f="Yes";}à=Ë+Ó;if(à<1){à=1
;}ß=Ê+Ò;if(ß<0){ß=0;}if(Ô<0){Ô=1;}if(Ô>1){Ô=0;}Ý=Î+Ô;if(Ý<0){Ý=1;}if(Ý>1){Ý=0;}if(Õ<0){Õ=1;}if(Õ>1){Õ=0;}if(Õ==0){g="No";
}if(Õ==1){g="Yes";}ä=Õ+ã;if(ä<0){ä=1;}if(ä>1){ä=0;}if(ä==0){û=false;}if(ä==1){û=true;}if(ä==0){g="No";}if(ä==1){g="Yes";}
if(Ö<0){Ö=1;}if(Ö>1){Ö=0;}if(Ö==0){h="No";}if(Ö==1){h="Yes";}if(Û<0){Û=9;}if(Û>9){Û=0;}if(Ú<0){Ú=1;}if(Ú>1){Ú=0;}if(Ý==0){
Ď="No";}if(Ý==1){Ď="Yes";}if(Ù<0){Ù=1;}if(Ù>1){Ù=0;}if(Ù==0){i="No";}if(Ù==1){i="Yes";}if(å<0){å=1;}if(å>1){å=0;}if(å==1)
{õ=true;}if(å==0){õ=false;}if(æ<0){æ=1;}if(æ>1){æ=0;}if(æ==1){ô=true;}if(æ==0){ô=false;}if(ç<0){ç=1;}if(ç>1){ç=0;}if(ç==1
){ó=true;}if(ç==0){ó=false;}if(õ){k="Yes";}if(!õ){k="No";}if(ô){l="Yes";}if(!ô){l="No";}if(ó){m="Yes";}if(!ó){m="No";}}
public void ŗ(string ő){if(ő.Contains("confirm")){if(Æ==0){if(É==0){Æ=2;É=0;ő="";}}}if(ő.Contains("confirm")){if(Æ==0){if(É==1
){Æ=1;É=0;ő="";}}}if(ő.Contains("confirm")){if(Æ==0){if(É==2){if(Ġ.Count>0){if(Ĩ.Count>0){if(Ĩ[0]!=null){ļ(Ĩ[0]);}}}Æ=3;É
=0;ő="";}}}if(ő.Contains("confirm")){if(Æ==1){if(É==11&&!õ){É=0;ò=false;ő="";}if(É==11&&õ){if(Ö==1){è=7;Ù=0;}if(Ù==0){Œ()
;œ(Me,j);Ŕ(Me);ò=true;É=0;è=0;å=0;Ö=0;Ø=0;õ=false;ő="";}if(Ù==1){ò=true;Æ=0;É=0;è=0;å=0;Ö=0;Ù=0;Ø=0;õ=false;Ü=0;ő="";}}}}
if(ő.Contains("confirm")){if(Æ==2){if(É==11&&!ô){ŕ();ñ=false;ő="";}if(É==11&&ô){if(Ù==0){if(î){ģ.Clear();ģ.Append("GPS");ģ
.Append(":");ģ.Append("DDT");ģ.Append(":");ģ.Append(Math.Round(Č.X,2));ģ.Append(":");ģ.Append(Math.Round(Č.Y,2));ģ.Append
(":");ģ.Append(Math.Round(Č.Z,2));ģ.Append(":");ģ.Append("#FF75C9F1");ģ.Append(":");ģ.Append(Ĉ);ģ.Append(":");ģ.Append(Ċ)
;ģ.Append(":");ģ.Append(Þ);ģ.Append(":");ģ.Append(ď);ģ.Append(":");ģ.Append(ĉ);ģ.Append(":");ģ.Append(ú);ģ.Append(":");ģ.
Append(ß);ģ.Append(":");ģ.Append(à);ģ.Append(":");ģ.Append(Ý);ģ.Append(":");ģ.Append(û);ģ.Append(":");if(ý&&Ø==1){ģ.Append(
$"GPS:DDT:{č.X}:{č.Y}:{č.Z}:#FF75C9F1:{ċ}:");}}Ŗ(þ,ģ.ToString());ñ=true;ą=0.0;Ć=0.0;ć=0.0;Ò=0;Ó=0;Ð=0;Ï=0;Ô=0;Ñ=0;Õ=0;Ù=0;Ø=0;æ=0;ô=false;ŕ();ő="";}if(Ù==1){Æ=0;Ü=
0;É=0;ñ=true;ą=0.0;Ć=0.0;ć=0.0;Ò=0;Ó=0;Ð=0;Ï=0;Ô=0;Ñ=0;Õ=0;Ù=0;Ø=0;æ=0;ô=false;ő="";}}}}if(ő.Contains("confirm")){if(Æ==3
){if(É==11&&!ó){ŕ();ñ=false;ő="";}if(É==11&&ó){if(Ù==0){if(è==0){if(Ĩ.Count>0){if(Ĩ[0]!=null){ŋ(Ĩ[0],á);}}}if(è==1){if(Ĩ.
Count>0){if(Ĩ[0]!=null){Ŋ(Ĩ[0],á);}}}ñ=true;Û=0;Ú=0;Ù=0;ç=0;ó=false;Ü=0;ŕ();ő="";}if(Ù==1){Æ=0;Ü=0;É=0;ñ=true;Û=0;Ú=0;Ù=0;ç=0
;ó=false;ő="";}}}}if(ő.Contains("confirm")){if(Æ==1){if(É==0){É=7;ő="";}}}if(ő.Contains("confirm")){if(Æ==1){if(É==7){É=8
;ő="";}}}if(ő.Contains("confirm")){if(Æ==1){if(É==8){É=11;ő="";}}}if(ő.Contains("confirm")){if(Æ==2){if(É>=0&&É<=11){ŕ();
ő="";}}}if(ő.Contains("confirm")){if(Æ==3){if(É==0){ŕ();ő="";}}}if(ő.Contains("confirm")){if(Æ==3){if(É==1){É=10;ő="";}}}
if(ő.Contains("confirm")){if(Æ==3){if(É==10){ŕ();ő="";}}}}public void Ř(){if(Æ==0){if(Ü==0){if(É==0){e="";}}}if(Æ==1){if(Ü
==0){if(É==0){e="1";}if(É==7||É==8||É==9||É==10||É==11){e="Yes/No";}}}if(Æ==2){if(Ü==0){if(É==0||É==1||É==3||É==7||É==8){e
="1";}if(É==2||É==4||É==5){e="0.1";}if(É==6){e="Yes/No";}if(É==9){e="Yes/No";}if(É==10){e="Yes/No";}if(É==11){e="Yes/No";
}}if(Ü==1){if(É==0||É==1||É==7||É==8){e="5";}if(É==2||É==4||É==5){e="1.0";}if(É==6||É==3){e="Yes/No";}if(É==9){e="Yes/No"
;}if(É==10){e="Yes/No";}if(É==11){e="Yes/No";}}if(Ü==2){if(É==0||É==1||É==7||É==8){e="10";}if(É==2||É==4||É==5){e="10.0";
}if(É==6||É==3){e="Yes/No";}if(É==9){e="Yes/No";}if(É==10){e="Yes/No";}if(É==11){e="Yes/No";}}}if(Æ==3){if(Ü==0){if(É==0)
{e="Load/Save";}if(É==1){e="1";}if(É==10){e="Yes/No";}if(É==11){e="Yes/No";}}}}public void ŝ(string ő){if(ő=="setup"&&ü){
ü=false;ő="";Echo("Running setup...");}ř();if(ő.Contains(I)){Æ=2;É=0;Ü=0;ő="";}if(ő.Contains(H)){Æ=1;É=0;Ü=0;ő="";}if(ő.
Contains(G)){Æ=0;É=0;è=0;Ü=0;ő="";}if(ő.Contains(J)){Æ=0;É=0;Ü=0;è=7;Me.CustomData="";R="";ő="";}if(ő.Contains(F)){if(!ì){ŕ();ì=
true;ő="";}}if(ì){ì=false;}if(ő.Contains(E)){if(!í){Ś();í=true;ő="";}}if(í){í=false;}if(ő.Contains(D)){if(Æ==0&&!ö){Ü=0;ö=
true;}if(Æ==1&&!ö){Ü=0;ö=true;}if(Æ==2){if(!ö&&!ö){Ü++;ö=true;}if(Ü>2){Ü=0;ö=true;}}if(Æ==3&&!ö){Ü=0;ö=true;}ő="";}if(ö){ö=
false;}if(ő.Contains(B)){if(!ø){if(Ü==0){if(Æ==0){ŕ();ø=true;}if(Æ==1&&!ø){if(É==0){ś();}if(É==7){Ö++;}if(É==8){Ù++;}if(É==11
){å++;}ø=true;}if(Æ==2&&!ø){if(É==0){Ð++;}if(É==1){Ï++;}if(É==2){ć=ć+0.1;}if(É==3){Ô++;}if(É==4){ą=ą+0.1;}if(É==5){Ć=Ć+
0.1;}if(É==6){Ñ++;}if(É==7){Ó++;}if(É==8){Ò++;}if(É==9){Õ++;}if(É==10){Ù++;}if(É==11){æ++;}ø=true;}if(Æ==3&&!ø){if(É==0){ś(
);}if(É==1){Û++;}if(É==10){Ù++;}if(É==11){ç++;}ø=true;}}if(Ü==1){if(Æ==2&&!ø){if(É==0){Ð=Ð+5;}if(É==1){Ï=Ï+5;}if(É==2){ć=
ć+1.0;}if(É==3){Ô++;}if(É==4){ą=ą+1.0;}if(É==5){Ć=Ć+1.0;}if(É==6){Ñ=Ñ+1;}if(É==7){Ó=Ó+5;}if(É==8){Ò=Ò+5;}if(É==9){Õ++;}if
(É==10){Ù++;}if(É==11){æ++;}ø=true;}}if(Ü==2){if(Æ==2&&!ø){if(É==0){Ð=Ð+10;}if(É==1){Ï=Ï+10;}if(É==2){ć=ć+5.0;}if(É==3){Ô
++;;}if(É==4){ą=ą+10.0;}if(É==5){Ć=Ć+10.0;}if(É==6){Ñ++;}if(É==7){Ó=Ó+10;}if(É==8){Ò=Ò+10;}if(É==9){Õ++;}if(É==10){Ù++;}if
(É==11){æ++;}ø=true;}}ő="";}if(ø){ø=false;}}if(ő.Contains(C)){if(!ù){if(Ü==0){if(Æ==0){Ś();ù=true;}if(Æ==1&&!ù){if(É==0){
Ŝ();}if(É==7){Ö--;}if(É==8){Ù--;}if(É==11){å--;}ù=true;}if(Æ==2&&!ù){if(É==0){Ð--;}if(É==1){Ï--;}if(É==2){ć=ć-0.1;}if(É==
3){Ô--;}if(É==4){ą=ą-0.1;}if(É==5){Ć=Ć-0.1;}if(É==6){Ñ--;}if(É==7){Ó--;}if(É==8){Ò--;}if(É==9){Õ--;}if(É==10){Ù--;}if(É==
11){æ--;}ù=true;}if(Æ==3&&!ù){if(É==0){Ŝ();}if(É==1){Û--;}if(É==10){Ù--;}if(É==11){ç--;}ù=true;}}if(Ü==1){if(Æ==2&&!ù){if(
É==0){Ð=Ð-5;}if(É==1){Ï=Ï-5;}if(É==2){ć=ć-1.0;}if(É==3){Ô--;}if(É==4){ą=ą-1.0;}if(É==5){Ć=Ć-1.0;}if(É==6){Ñ--;}if(É==7){Ó
=Ó-5;}if(É==8){Ò=Ò-5;}if(É==9){Õ--;}if(É==10){Ù--;}if(É==11){æ--;}ù=true;}}if(Ü==2){if(Æ==2&&!ù){if(É==0){Ð=Ð-10;}if(É==1
){Ï=Ï-10;}if(É==2){ć=ć-5.0;}if(É==3){Ô--;}if(É==4){ą=ą-10.0;}if(É==5){Ć=Ć-10.0;}if(É==6){Ñ--;}if(É==7){Ó=Ó-10;}if(É==8){Ò
=Ò-10;}if(É==9){Õ--;}if(É==10){Ù--;}if(É==11){æ--;}ù=true;}}}ő="";}if(ù){ù=false;}Ř();Ő();ŗ(ő);}public void
 Main
(string ő,UpdateType Ş){ë++;IMyGridTerminalSystem ş=GridTerminalSystem as IMyGridTerminalSystem;if(!ü){Š(ş);}š();if(!ü){
Echo($"Setup not complete");return;}Ŏ();if(Á!=þ.CustomData){Ţ(þ.CustomData,þ);Á=þ.CustomData;}ř();ŏ();ŝ(ő);if(ü){ţ(É);Ť();if
(ķ.Count>0){for(int ť=0;ť<ķ.Count;ť++){if(ķ[ť]!=null){ķ[ť].WriteText(Ģ.ToString());}}}}if(ñ){ñ=false;}if(ò){ò=false;}if(ë
%2==0){Echo(Ĥ.ToString());}Ĥ.Clear();if(ë>60){}}void Ŗ(IMyTerminalBlock ĸ,string Ō){Į.Clear();if(Į.TryParse(ĸ.CustomData.
ToString())){Į.Set(º,µ,Ō);}else{Į.Set(º,µ,Ō);}ĸ.CustomData=Į.ToString();Į.Clear();}void Š(IMyGridTerminalSystem ş){N="["+
drone_tag+" "+K+"]";O="["+drone_tag+" "+L+" "+M+"]";Q=$"[{P}]";đ.Clear();Ē.Clear();ē.Clear();Ĕ.Clear();ĕ.Clear();Ė.Clear();ė.
Clear();Ę.Clear();ę.Clear();Ě.Clear();ě.Clear();Ĝ.Clear();ĝ.Clear();Ğ.Clear();Ģ.Clear();ģ.Clear();ğ.Clear();Ġ.Clear();Ġ.Add(
"");Ġ.Add("");Ġ.Add("");Ġ.Add("");Ġ.Add("");Ġ.Add("");Ġ.Add("");Ġ.Add("");Ġ.Add("");Ġ.Add("");Ğ.Add(
"Initialize mining grid");Ğ.Add("Reset drones");Ğ.Add("Run mining job");Ğ.Add("Recall drones to dock");Ğ.Add("Undock drones");Ğ.Add(
"Freeze command (dev)");Ğ.Add("Stop command (dev)");Ğ.Add("");ğ.Add("Load Job");ğ.Add("Save Job");ğ.Add("");đ.Add("Mining Job Configuration");
Ē.Add("Command Menu");ē.Add("Job Management Menu");Ĕ.Add("");ĕ.Add("");Ė.Add("");ė.Add("");Ę.Add("");ę.Add("");Ě.Add("");
ě.Add("");Ĝ.Add("");đ.Add("Command:");Ē.Add("---");ē.Add("---");Ĕ.Add("---");ĕ.Add("---");Ė.Add("---");ė.Add("---");Ę.Add
("Cancel:");ę.Add("Main Menu:");Ě.Add("---");ě.Add("---:");Ĝ.Add("Confirm:");đ.Add("Number Grid X positions:");Ē.Add(
"Number Grid Y positions:");ē.Add("Grid Spread:");Ĕ.Add("Perimeter Only:");ĕ.Add("Drill Depth:");Ė.Add("Ignore Depth:");ė.Add(
"Limit drones in-flight:");Ę.Add("In-Flight Hard Limit:");ę.Add("In-Flight Factor:");Ě.Add("Core out:");ě.Add("Main Menu:");Ĝ.Add("Confirm:");đ.
Add("Load/Save Job:");Ē.Add("Job number:");ē.Add("Job Name:");Ĕ.Add("");ĕ.Add("");Ė.Add("");ė.Add("");Ę.Add("");ę.Add("");Ě
.Add("");ě.Add("Main Menu:");Ĝ.Add("Confirm:");Æ=0;É=0;Me.CustomData="";ĩ.Clear();Ī.Clear();Ĵ.Clear();ĵ.Clear();Ķ.Clear()
;bool Ŧ=false;ş.GetBlocksOfType<IMyMotorStator>(Ĵ,ŧ=>ŧ.TopGrid==Me.CubeGrid);if(Ĵ.Count<=0){Echo(
"Rotor top grid not found, checking advanced rotors");}if(Ĵ.Count>0){if(Ĵ[0]!=null){ĳ=Ĵ[0].CubeGrid;Echo("Local cubegrid found - rotor");Ŧ=true;}}ş.GetBlocksOfType<
IMyMotorAdvancedStator>(ĵ,ŧ=>ŧ.TopGrid==Me.CubeGrid);if(ĵ.Count<=0){Echo("Rotor top grid not found, checking advanced rotors");}if(ĵ.Count>0){
if(ĵ[0]!=null){ĳ=ĵ[0].CubeGrid;Echo("Local cubegrid found - advanced rotor/hinge");Ŧ=true;}}ş.GetBlocksOfType<
IMyPistonBase>(Ķ,ŧ=>ŧ.TopGrid==Me.CubeGrid);if(Ķ.Count<=0){Echo("Rotor top grid not found, checking pistons");}if(Ķ.Count>0){if(Ķ[0]
!=null){ĳ=Ķ[0].CubeGrid;Echo("Local cubegrid found - piston");Ŧ=true;}}if(ĵ.Count==0&&Ĵ.Count==0&&Ķ.Count==0){ĳ=Me.
CubeGrid;Echo("Local cubegrid found - PB");Ŧ=false;}Ĵ.Clear();ĵ.Clear();Ķ.Clear();if(Ŧ){ş.GetBlocksOfType<IMyRadioAntenna>(ĩ,ŧ=>
ŧ.CubeGrid==Me.CubeGrid);if(ĩ.Count>0){for(int ť=0;ť<ĩ.Count;ť++){if(ĩ[ť].CustomName.Contains(K)){string Ũ=ĩ[ť].
CustomData;ō(Ũ);if(string.IsNullOrEmpty(drone_tag)||string.IsNullOrWhiteSpace(drone_tag)){Ĥ.AppendLine($"Invalid name for drone_tag {drone_tag.Replace("[","[[").Replace("]","]]")}. please add vailid drone tag (drone group name) to antenna custom data e.g. 'SWRM_D:Atlas:', '<drone_tag>:<ship_name>:"
);return;}Ī.Add(ĩ[ť]);}}}ĩ.Clear();}ş.GetBlocksOfType<IMyRadioAntenna>(ĩ,ŧ=>ŧ.CubeGrid==ĳ);if(ĩ.Count>0){for(int ť=0;ť<ĩ.
Count;ť++){if(ĩ[ť].CustomName.Contains(K)){string Ũ=ĩ[ť].CustomData;ō(Ũ);if(string.IsNullOrEmpty(drone_tag)||string.
IsNullOrWhiteSpace(drone_tag)){Ĥ.AppendLine($"Invalid name for drone_tag {drone_tag.Replace("[","[[").Replace("]","]]")}. please add vailid drone tag (drone group name) to antenna custom data e.g. 'SWRM_D:Atlas:', '<drone_tag>:<ship_name>:"
);return;}Ī.Add(ĩ[ť]);}}}else{ĩ.Clear();Echo("No ship antennas found with tag "+K.Replace("[","[[").Replace("]","]]")+
". Please add a ship antenna with tag "+K.Replace("[","[[").Replace("]","]]")+" to the main ship");return;}ĩ.Clear();ĥ.Clear();Ħ.Clear();ķ.Clear();if(Ŧ){ş.
GetBlocksOfType<IMyTerminalBlock>(ĥ,ŧ=>ŧ.CubeGrid==Me.CubeGrid);if(ĥ.Count>0){for(int ť=0;ť<ĥ.Count;ť++){if(ĥ[ť].CustomName.Contains(O)
){Ħ.Add(ĥ[ť]);ķ.Add(((IMyTextSurfaceProvider)ĥ[ť]).GetSurface(scnpanel));}}}ĥ.Clear();}ş.GetBlocksOfType<IMyTerminalBlock
>(ĥ,ŧ=>ŧ.CubeGrid==ĳ);if(ĥ.Count>0){for(int ť=0;ť<ĥ.Count;ť++){if(ĥ[ť].CustomName.Contains(O)){Ħ.Add(ĥ[ť]);ķ.Add(((
IMyTextSurfaceProvider)ĥ[ť]).GetSurface(scnpanel));}}}ĥ.Clear();ħ.Clear();Ĩ.Clear();if(Ŧ){ş.GetBlocksOfType<IMyProgrammableBlock>(ħ,ŧ=>ŧ.
CubeGrid==Me.CubeGrid);if(ħ.Count>0){for(int ť=0;ť<ħ.Count;ť++){if(ħ[ť].CustomName.Contains(N)){Ĩ.Add(ħ[ť]);}}}ħ.Clear();}ş.
GetBlocksOfType<IMyProgrammableBlock>(ħ,ŧ=>ŧ.CubeGrid==ĳ);if(ħ.Count>0){for(int ť=0;ť<ħ.Count;ť++){if(ħ[ť].CustomName.Contains(N)){Ĩ.
Add(ħ[ť]);}}}ħ.Clear();ī.Clear();Ĭ.Clear();if(Ŧ){ş.GetBlocksOfType<IMyRemoteControl>(ī,ŧ=>ŧ.CubeGrid==Me.CubeGrid);if(ī.
Count>0){for(int ť=0;ť<ī.Count;ť++){if(ī[ť].CustomName.Contains(N)||ī[ť].CustomName.Contains(K)){Ĭ.Add(ī[ť]);}}}ī.Clear();}ş.
GetBlocksOfType<IMyRemoteControl>(ī,ŧ=>ŧ.CubeGrid==ĳ);if(ī.Count>0){for(int ť=0;ť<ī.Count;ť++){if(ī[ť].CustomName.Contains(N)||ī[ť].
CustomName.Contains(K)){Ĭ.Add(ī[ť]);}}}ī.Clear();if(ķ.Count>0){for(int ť=0;ť<ķ.Count;ť++){if(ķ[ť]!=null){if(ķ[ť].ContentType!=
ContentType.TEXT_AND_IMAGE){ķ[ť].ContentType=ContentType.TEXT_AND_IMAGE;ķ[ť].Alignment=TextAlignment.LEFT;ķ[ť].FontSize=0.380f;ķ[ť]
.Font="White";}}}}Â=O.Replace("[","[[").Replace("]","]]");Ã=N.Replace("[","[[").Replace("]","]]");;Ä=P.Replace("[","[[").
Replace("]","]]");;ü=true;Ĥ.AppendLine("Setup complete!");}void š(){if(Ī.Count<=0){Echo($"Main antenna with tag {K.Replace("[","[[").Replace("]","]]")} not found. Please setup and configure main GMDC controller."
);return;}if(ķ.Count<=0){Echo($"Main Displays with tag '{O.Replace("[","[[").Replace("]","]]")}' not found");ü=false;
return;}if(Ĩ.Count>0){if(Ĩ[0]!=null){if(þ!=Ĩ[0]){þ=Ĩ[0];}}else{Echo(
$"Drone controller with with tag '{N.Replace("[","[[").Replace("]","]]")}' not found");ü=false;return;}}else{Echo($"Drone controller with with tag '{N.Replace("[","[[").Replace("]","]]")}' not found");ü=
false;return;}}void Ů(string Ō){Į.Clear();if(Į.TryParse(Ō)){var Ĺ="";bool ũ=false;bool Ū=false;bool ū=false;Ĺ=Į.Get(º,
"jobname").ToString().Trim();À=Ĺ;Ĺ=Į.Get(º,µ).ToString().Trim();ª=Ĺ;Ĺ=Į.Get(º,"TargetGPS").ToString().Trim();String[]Ŭ=Ĺ.Split(
':');if(Ŭ.Length>=5){if(!double.TryParse(Ŭ[2],out Č.X)){Č.X=0.0;}if(!double.TryParse(Ŭ[3],out Č.Y)){Č.Y=0.0;}if(!double.
TryParse(Ŭ[4],out Č.Z)){Č.Z=0.0;}}else{Č.X=0.0;Č.Y=0.0;Č.Z=0.0;}Ĺ=Į.Get(º,"AlignGPS").ToString().Trim();String[]ŭ=Ĺ.Split(':');
if(ŭ.Length>=5){if(!double.TryParse(Ŭ[2],out č.X)){č.X=0.0;}else{ũ=true;}if(!double.TryParse(Ŭ[3],out č.Y)){č.Y=0.0;}else{
Ū=true;}if(!double.TryParse(Ŭ[4],out č.Z)){č.Z=0.0;}else{ū=true;}}else{č.X=0.0;č.Y=0.0;č.Z=0.0;if(ý){ý=false;}}if(ũ&&Ū&&ū
&&!ý){ý=true;}Ĺ=Į.Get(º,"BoreSeparation").ToString().Trim();if(!double.TryParse(Ĺ,out ă)){ă=10.0;}Ĺ=Į.Get(º,"GridXBores").
ToString().Trim();if(!int.TryParse(Ĺ,out Í)){Í=1;}Ĺ=Į.Get(º,"GridYBores").ToString().Trim();if(!int.TryParse(Ĺ,out Ì)){Ì=1;}Ĺ=Į.
Get(º,"SkipBores").ToString().Trim();if(!int.TryParse(Ĺ,out Î)){Î=0;}Ĺ=Į.Get(º,"SafeAlignDistance").ToString().Trim();if(!
double.TryParse(Ĺ,out ċ)){ċ=30.0;}Ĺ=Į.Get(º,"DrillDepth").ToString().Trim();if(!double.TryParse(Ĺ,out Ą)){Ą=30.0;}Ĺ=Į.Get(º,
"IgnoreDepth").ToString().Trim();if(!double.TryParse(Ĺ,out Ă)){Ă=0.0;}Ĺ=Į.Get(º,"LimitDronesInFlight").ToString().Trim();if(!bool.
TryParse(Ĺ,out ï)){ï=false;}Ĺ=Į.Get(º,"DronesFlightHardLimit").ToString().Trim();if(!int.TryParse(Ĺ,out Ë)){Ë=10;}Ĺ=Į.Get(º,
"DronesFlightFactor").ToString().Trim();if(!int.TryParse(Ĺ,out Ê)){Ê=1;}Ĺ=Į.Get(º,"CoreOutFunction").ToString().Trim();if(!bool.TryParse(Ĺ,
out ð)){ð=false;}}Į.Clear();}void Ţ(string Ō,IMyTerminalBlock ĸ){if(string.IsNullOrWhiteSpace(Ō)||string.IsNullOrEmpty(Ō)){
return;}Ů(ĸ.CustomData);String[]ů=ª.Split(':');if(ů.Length<10){ÿ="";S="";T="";U="";V="";W="";X="";Y="";Z="";a="";Ā="";b="";c=
"";ā="";Ĥ.AppendLine("Please use prospector to assign a mining location");î=false;return;}else{î=true;}if(ů.Length>4){Č=
new Vector3D(Double.Parse(ů[2]),Double.Parse(ů[3]),Double.Parse(ů[4]));}if(ů.Length<6){ÿ="";Ą=1.0;S="";ă=0.0;T="";Í=0;U="";
Ì=0;V="";Ă=0.0;W="";ï=false;X="";Ê=1;Y="";Ë=6;Z="";Î=0;a="";ð=false;return;}if(ů.Length>5){if(ů.Length>5){ÿ=ů[6];d=ÿ;if(
Double.TryParse(d,out Ą)){Double.TryParse(d,out Ą);}else{Ą=1.0;}}else{ÿ="";Ą=1.0;}}if(ů.Length<7){S="";ă=0.0;T="";Í=0;U="";Ì=0
;V="";Ă=0.0;W="";ï=false;X="";Ê=1;Y="";Ë=6;Z="";Î=0;a="";ð=false;return;}if(ů.Length>6){S=ů[7];if(double.TryParse(S,out ă
)){double.TryParse(S,out ă);}else{ă=0.0;}}else{S="";ă=0.0;}if(ů.Length<8){S="";ă=0.0;T="";Í=0;U="";Ì=0;V="";Ă=0.0;W="";ï=
false;X="";Ê=1;Y="";Ë=6;Z="";Î=0;a="";ð=false;return;}if(ů.Length>7){T=ů[8];if(int.TryParse(T,out Í)){int.TryParse(T,out Í);}
else{Í=0;}}else{T="";Í=0;}if(ů.Length<9){S="";ă=0.0;T="";Í=0;U="";Ì=0;V="";Ă=0.0;W="";ï=false;X="";Ê=1;Y="";Ë=6;Z="";Î=0;a=
"";ð=false;return;}if(ů.Length>8){U=ů[9];if(int.TryParse(U,out Ì)){int.TryParse(U,out Ì);}else{Ì=0;}}else{U="";Ì=0;}if(ů.
Length<10){V="";Ă=0.0;W="";ï=false;X="";Ê=1;Y="";Ë=6;Z="";Î=0;a="";ð=false;return;}if(ů.Length>9){V=ů[10];if(Double.TryParse(V
,out Ă)){Double.TryParse(V,out Ă);}else{Ă=0.0;}}else{V="";Ă=0.0;}if(ů.Length<12){W="";ï=false;X="";Ê=1;Y="";Ë=6;Z="";Î=0;
a="";ð=false;return;}if(ů.Length>11){W=ů[11];if(bool.TryParse(W,out ï)){bool.TryParse(W,out ï);}else{ï=false;}}else{W="";
ï=false;}if(ů.Length<13){X="";Ê=1;Y="";Ë=6;Z="";Î=0;a="";ð=false;return;}if(ů.Length>12){X=ů[12];if(int.TryParse(X,out Ê)
){int.TryParse(X,out Ê);}else{Ê=1;}}else{X="";Ê=1;}if(ů.Length<14){Y="";Ë=6;Z="";Î=0;a="";ð=false;return;}if(ů.Length>13)
{Y=ů[13];if(int.TryParse(Y,out Ë)){int.TryParse(Y,out Ë);}else{Ë=6;}}else{Y="";Ë=6;}if(ů.Length<15){Z="";Î=0;a="";ð=false
;return;}if(ů.Length>14){Z=ů[14];if(int.TryParse(Z,out Î)){int.TryParse(Z,out Î);î=true;}else{Î=0;}}else{Z="";Î=0;}if(ů.
Length<16){a="";ð=false;return;}if(ů.Length>15){a=ů[15];if(bool.TryParse(a,out ð)){bool.TryParse(a,out ð);î=true;}else{ð=false
;}}else{a="";ð=false;}if(ů.Length>16){Ĥ.AppendLine($"gpsCommandLen:{ů.Length}");bool Ű;bool ű;bool Ų;if(ů.Length>16){}if(
ů.Length>17){}if(ů.Length>18){Ā=ů[18];}if(ů.Length>19){b=ů[19];}if(ů.Length>20){c=ů[20];}if(ů.Length>21){}if(ů.Length>22)
{ā=ů[22];}if(!double.TryParse(Ā,out č.X)){č.X=0.0;Ā="";Ű=false;}else{Ű=true;}if(!double.TryParse(b,out č.Y)){č.Y=0.0;b=""
;ű=false;}else{ű=true;}if(!double.TryParse(c,out č.Z)){č.Z=0.0;c="";Ų=false;}else{Ų=true;}if(Ű&&ű&&Ų){Ø=1;ý=true;}else{Ø=
0;ý=false;}if(ů.Length>22){if(!double.TryParse(ā,out ċ)){ċ=30.0;ā="";}}if(ý&&ů.Length>16&&ů.Length<18){ý=false;}}}public
void ţ(int ų){if(ų==0){n="[O]";}else{n="[ ]";}if(ų==1){o="[O]";}else{o="[ ]";}if(ų==2){p="[O]";}else{p="[ ]";}if(ų==3){q=
"[O]";}else{q="[ ]";}if(ų==4){r="[O]";}else{r="[ ]";}if(ų==5){s="[O]";}else{s="[ ]";}if(ų==6){t="[O]";}else{t="[ ]";}if(ų==7)
{u="[O]";}else{u="[ ]";}if(ų==8){v="[O]";}else{v="[ ]";}if(ų==9){w="[O]";}else{w="[ ]";}if(ų==10){x="[O]";}else{x="[ ]";}
if(ų==11){y="[O]";}else{y="[ ]";}}public void Ť(){Ģ.Clear();Ģ.Append("GMDI ").Append(P).Append(" - ").Append(Å).Append(
" - Current Job: [").Append(À).Append("]").AppendLine();Ģ.AppendLine("------------");if(Æ==0)Ģ.Append("Main Menu - Iteration: ").Append(e).
Append(" Item: ").Append(É+1);else if(Æ==1)Ģ.Append("Command Menu - Iteration: ").Append(e).Append(" Item: ").Append(É+1);else
if(Æ==2)Ģ.Append("Mining Job Config. - Iteration: ").Append(e).Append(" Item: ").Append(É+1);else if(Æ==3)Ģ.Append(
"Job Management. - Iteration: ").Append(e).Append(" Item: ").Append(É+1);Ģ.Append("\n\n\n");if(Æ==0){Ģ.Append(n).Append(" 1. ").AppendLine(đ[Æ]);Ģ.
Append(o).Append(" 2. ").AppendLine(Ē[Æ]);Ģ.Append(p).Append(" 3. ").AppendLine(ē[Æ]);Ģ.Append("\nCommand: ").AppendLine(R);}
else if(Æ==1){Ģ.Append(n).Append(" 1. ").AppendLine(Ğ[è]);Ģ.Append(o).Append(" ..  ").AppendLine(Ē[Æ]);Ģ.Append(u).Append(
" 8. ").Append(Ę[Æ]).Append(" ").AppendLine(h);Ģ.Append(v).Append(" 9.  ").Append(ę[Æ]).Append(" ").AppendLine(i);Ģ.Append(w).
Append(" ..  ").AppendLine(Ě[Æ]);Ģ.Append(y).Append(" 11. ").Append(Ĝ[Æ]).Append(" ").AppendLine(k);if(ò)Ģ.AppendLine(
"\n\nCommand confirmed!");Ģ.Append("\n\nCommand: ").AppendLine(R);}else if(Æ==2){Ģ.Append(n).Append(" 1. ").Append(đ[Æ]).Append(" ").Append(Þ).
Append('\n');Ģ.Append(o).Append(" 2. ").Append(Ē[Æ]).Append(" ").Append(ď).Append('\n');Ģ.Append(p).Append(" 3. ").Append(ē[Æ]
).Append(" ").Append(Ċ).AppendLine("m");Ģ.Append(q).Append(" 4. ").Append(Ĕ[Æ]).Append(" ").AppendLine(Ď);Ģ.Append(r).
Append(" 5. ").Append(ĕ[Æ]).Append(" ").Append(Ĉ).AppendLine("m");Ģ.Append(s).Append(" 6. ").Append(Ė[Æ]).Append(" ").Append(ĉ
).AppendLine("m");Ģ.Append(t).Append(" 7. ").Append(ė[Æ]).Append(" ").AppendLine(f);Ģ.Append(u).Append(" 8. ").Append(Ę[Æ
]).Append(" ").Append(à).Append('\n');Ģ.Append(v).Append(" 9. ").Append(ę[Æ]).Append(" ").Append(ß).Append('\n');Ģ.Append
(w).Append(" 10. ").Append(Ě[Æ]).Append(" ").AppendLine(g);Ģ.Append(x).Append(" 11. ").Append(ě[Æ]).Append(" ").
AppendLine(i);Ģ.Append(y).Append(" 12. ").Append(Ĝ[Æ]).Append(" ").AppendLine(l);if(ñ)Ģ.AppendLine("\n\nData confirmed!");}else if
(Æ==3){string Ŵ=(Ġ[á]!=null)?Ġ[á]:"undefined";Ģ.Append(n).Append(" 1. ").Append(đ[Æ]).Append(" ").AppendLine(ğ[è]);Ģ.
Append(o).Append(" 2. ").Append(Ē[Æ]).Append(" ").Append(á+1).Append('\n');Ģ.Append(p).Append(" 3. ").Append(ē[Æ]).Append(" ")
.AppendLine(Ŵ);Ģ.Append(w).Append(" ..  ").AppendLine(Ě[Æ]);Ģ.Append(x).Append(" 11. ").Append(ě[Æ]).Append(" ").
AppendLine(i);Ģ.Append(y).Append(" 12. ").Append(Ĝ[Æ]).Append(" ").AppendLine(m);if(ñ)Ģ.AppendLine("\n\nCommand confirmed!");Ģ.
Append("\n\nCommand: ").AppendLine(R);}if(î){Ģ.Append("\n\nMining Job Information\n-----------\n");Ģ.Append(
"Surface Distance: ").Append(ċ).Append('\n');Ģ.AppendLine("Target Coordinates:");Ģ.Append("X: ").Append(Math.Round(Č.X,2)).Append(", Y: ").
Append(Math.Round(Č.Y,2)).Append(", Z: ").Append(Math.Round(Č.Z,2)).Append('\n');if(ý){Ģ.AppendLine("Align Coordinates:");Ģ.
Append("X: ").Append(č.X).Append(", Y: ").Append(č.Y).Append(", Z: ").Append(č.Z).Append('\n');}}if(!î){Ģ.AppendLine("\n");Ģ.
AppendLine("\nNo target coordinates found!");Ģ.AppendLine("Please assign valid target using prospector.\n");}}public void ŕ(){if(Æ
==0){È=2;Ç=0;}if(Æ==1){È=1;Ç=0;}if(Æ==2){È=11;Ç=0;}if(Æ==3){È=11;Ç=0;}É++;if(É>È){É=Ç;}}public void Ś(){if(Æ==0){È=2;Ç=0;}
if(Æ==1){È=11;Ç=0;}if(Æ==2){È=11;Ç=0;}if(Æ==3){È=1;Ç=0;}É--;if(É<Ç){É=È;}}public void ś(){if(Æ==1){é=0;ê=6;}if(Æ==3){é=0;ê
=1;}è++;if(è>ê){è=é;}}public void Ŝ(){if(Æ==1){é=0;ê=6;}if(Æ==3){é=0;ê=1;}è--;if(è<é){è=ê;}}public void Œ(){if(è==0){j=
"init";}if(è==1){j="reset";}if(è==2){j="run";}if(è==3){j="recall";}if(è==4){j="eject";}if(è==5){j="freeze";}if(è==6){j="stop";
}if(è==7){j="";}}public void œ(IMyTerminalBlock ĸ,string H){Ĳ.Clear();if(Ĳ.TryParse(ĸ.CustomData.ToString())){Ĳ.Set(
"GMDIJobData","interfacecommand",H);}else{Ĳ.Set("GMDIJobData","interfacecommand",H);}ĸ.CustomData=Ĳ.ToString();Ĳ.Clear();}public void
Ŕ(IMyTerminalBlock ĸ){var Ĺ="";Ĳ.Clear();if(Ĳ.TryParse(ĸ.CustomData.ToString())){Ĺ=Ĳ.Get("GMDIJobData","interfacecommand"
).ToString();R=Ĺ;}else{R="";œ(ĸ,"");}Ĳ.Clear();}void Ŷ(int ŵ){if(ŵ==0){z=".---";}if(ŵ==1){z="-.--";}if(ŵ==2){z="--.-";}if
(ŵ==3){z="---.";}}void ř(){ġ++;if(ġ>3){ġ=0;}Ŷ(ġ);}
