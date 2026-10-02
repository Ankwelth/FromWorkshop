// CCN v4.1 - SAM Cargo Collection Network
// 
class W{readonly Program A;int B;long C;bool D;readonly List<IMyTerminalBlock>E=new List<IMyTerminalBlock>();readonly
List<IMyShipDrill>F=new List<IMyShipDrill>();readonly List<IMyCargoContainer>G=new List<IMyCargoContainer>();readonly List<
IMyShipConnector>H=new List<IMyShipConnector>();readonly List<IMyLightingBlock>I=new List<IMyLightingBlock>();readonly List<
IMySoundBlock>J=new List<IMySoundBlock>();readonly List<IMyBatteryBlock>K=new List<IMyBatteryBlock>();readonly List<IMyGasTank>L=new
List<IMyGasTank>();IMyRemoteControl M;IMyProgrammableBlock N;readonly Dictionary<string,List<IMyCargoContainer>>Q=new
Dictionary<string,List<IMyCargoContainer>>(O.P);readonly Dictionary<string,List<IMyTextSurfaceProvider>>R=new Dictionary<string,
List<IMyTextSurfaceProvider>>(O.P);readonly HashSet<long>S=new HashSet<long>();static readonly List<IMyCargoContainer>T=new
List<IMyCargoContainer>();static readonly List<IMyTextSurfaceProvider>U=new List<IMyTextSurfaceProvider>();public W(Program
V){A=V;}public void Y(bool X){D=X;}public Vector3D Z(){return A.Me.GetPosition();}public IReadOnlyList<IMyShipDrill>a(){
return F;}public IReadOnlyList<IMyCargoContainer>b(){return G;}public IReadOnlyList<IMyShipConnector>c(){return H;}public
IMyProgrammableBlock d(){return N;}public IReadOnlyList<IMyLightingBlock>e(){return I;}public IReadOnlyList<IMySoundBlock>f(){return J;}
public IReadOnlyList<IMyBatteryBlock>g(){return K;}public IReadOnlyList<IMyGasTank>h(){return L;}public bool i{get{return F.
Count>0;}}public bool j{get{return M!=null;}}public long k{get{return C;}}public IReadOnlyList<IMyCargoContainer>n(string l){
List<IMyCargoContainer>m;return Q.TryGetValue(l,out m)?m:T;}public IReadOnlyList<IMyTextSurfaceProvider>o(string l){List<
IMyTextSurfaceProvider>m;return R.TryGetValue(l,out m)?m:U;}public bool q(long p){return S.Contains(p);}readonly Dictionary<long,int>r=new
Dictionary<long,int>();public bool t(long p,out int s){return r.TryGetValue(p,out s);}void z(string l,IMyTextSurfaceProvider u,
long p){int v=l.LastIndexOf('@');if(v>0&&l[l.Length-1]==']'){int w;if(int.TryParse(l.Substring(v+1,l.Length-v-2),out w)){r[p
]=w;l=l.Substring(0,v)+"]";}}var y=x(l);if(!y.Contains(u))y.Add(u);}readonly Dictionary<long,float>ª=new Dictionary<long,
float>();public float º(long p){float µ;return ª.TryGetValue(p,out µ)?µ:1f;}static float Å(string À){if(string.IsNullOrEmpty(
À))return 1f;int Â=À.IndexOf("FillLimit=",O.Á);if(Â<0)return 1f;Â+=10;int Ã=Â;while(Ã<À.Length&&À[Ã]>='0'&&À[Ã]<='9')Ã++;
if(Ã==Â)return 1f;int Ä;if(!int.TryParse(À.Substring(Â,Ã-Â),out Ä))return 1f;if(Ä<=0)return 0.01f;if(Ä>=100)return 1f;
return Ä/100f;}public void Æ(){B=0;}public void Ì(bool Ç){if(!Ç&&B>0){B--;return;}B=1;E.Clear();if(D)A.GridTerminalSystem.
GetBlocksOfType<IMyTerminalBlock>(E,È=>È.CubeGrid==A.Me.CubeGrid);else A.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(E,È=>È.
IsSameConstructAs(A.Me));F.Clear();G.Clear();H.Clear();I.Clear();J.Clear();K.Clear();L.Clear();ª.Clear();M=null;N=null;foreach(var y in Q
.Values)y.Clear();foreach(var y in R.Values)y.Clear();S.Clear();r.Clear();for(int É=0;É<E.Count;É++)Ê(E[É]);Ë();C++;}
readonly List<IMyBlockGroup>Í=new List<IMyBlockGroup>();readonly List<IMyTerminalBlock>Î=new List<IMyTerminalBlock>();void Ë(){Í
.Clear();A.GridTerminalSystem.GetBlockGroups(Í);for(int Ï=0;Ï<Í.Count;Ï++){string Ð=Í[Ï].Name;var Ó=Ñ.Ò(Ð);if(Ó.Count==0)
continue;Î.Clear();Í[Ï].GetBlocks(Î,È=>D?È.CubeGrid==A.Me.CubeGrid:È.IsSameConstructAs(A.Me));for(int Ô=0;Ô<Ó.Count;Ô++){if(!Ó[Ô
].StartsWith(Õ.Ö,O.Á))continue;string l=Ó[Ô];for(int È=0;È<Î.Count;È++){var Ø=Î[È];if(l==Õ.Ù){var Ú=Ø as IMyLightingBlock
;if(Ú!=null&&!I.Contains(Ú))I.Add(Ú);var Û=Ø as IMySoundBlock;if(Û!=null&&!J.Contains(Û))J.Add(Û);}var Ü=Ø as
IMyCargoContainer;if(Ü!=null){var y=Ý(l);if(!y.Contains(Ü))y.Add(Ü);S.Add(Ü.EntityId);}var u=Ø as IMyTextSurfaceProvider;if(u!=null&&Þ.ß(
Ø)){z(l,u,Ø.EntityId);}}}}}void Ê(IMyTerminalBlock Ø){if(Þ.à(Ø))F.Add((IMyShipDrill)Ø);if(Þ.á(Ø))H.Add((IMyShipConnector)
Ø);var â=Ø as IMyBatteryBlock;if(â!=null)K.Add(â);var ã=Ø as IMyGasTank;if(ã!=null)L.Add(ã);if(Þ.ä(Ø)&&M==null)M=(
IMyRemoteControl)Ø;var å=Ø as IMyProgrammableBlock;if(å!=null&&å!=A.Me&&N==null){if(Ñ.æ(Ø.CustomName))N=å;}string ç=Ø.CustomName;if(Þ.è(
Ø)&&Ñ.é(ç,Õ.Ù)){var Ú=Ø as IMyLightingBlock;if(Ú!=null)I.Add(Ú);var Û=Ø as IMySoundBlock;if(Û!=null)J.Add(Û);}if(Þ.ê(Ø)){
G.Add((IMyCargoContainer)Ø);float ë=Å(Ø.CustomData);if(ë<1f)ª[Ø.EntityId]=ë;var Ó=Ñ.Ò(ç);for(int Ô=0;Ô<Ó.Count;Ô++){if(Ó[
Ô].StartsWith(Õ.Ö,O.Á)){var y=Ý(Ó[Ô]);if(!y.Contains((IMyCargoContainer)Ø))y.Add((IMyCargoContainer)Ø);S.Add(Ø.EntityId);
}}}var u=Ø as IMyTextSurfaceProvider;if(u!=null&&Þ.ß(Ø)){var Ó=Ñ.Ò(ç);for(int Ô=0;Ô<Ó.Count;Ô++){if(Ó[Ô].StartsWith(Õ.Ö,O
.Á))z(Ó[Ô],u,Ø.EntityId);}}}List<IMyCargoContainer>Ý(string l){List<IMyCargoContainer>m;if(!Q.TryGetValue(l,out m)){m=new
List<IMyCargoContainer>();Q[l]=m;}return m;}List<IMyTextSurfaceProvider>x(string l){List<IMyTextSurfaceProvider>m;if(!R.
TryGetValue(l,out m)){m=new List<IMyTextSurfaceProvider>();R[l]=m;}return m;}}static class Þ{public static bool à(IMyTerminalBlock
È){return È is IMyShipDrill;}public static bool ä(IMyTerminalBlock È){return È is IMyRemoteControl;}public static bool ê(
IMyTerminalBlock È){return È is IMyCargoContainer;}public static bool ß(IMyTerminalBlock È){return È is IMyTextPanel||È is
IMyTextSurfaceProvider;}public static bool á(IMyTerminalBlock È){return È is IMyShipConnector;}public static bool è(IMyTerminalBlock È){return
È is IMyLightingBlock||È is IMySoundBlock;}}static class Ñ{public static bool é(string ç,string l){return ç.IndexOf(l,O.Á
)>=0;}public static bool æ(string ç){return ç.IndexOf("[SAM",O.Á)>=0;}public static string ì(string ç,string l){if(é(ç,l)
)return ç;return ç.TrimEnd()+" "+l;}public static string ï(string ç){string í=ç;while(true){int î=í.IndexOf(Õ.Ö,O.Á);if(î
<0)break;int Ã=í.IndexOf(']',î);if(Ã<0)break;í=(í.Substring(0,î)+í.Substring(Ã+1)).Trim();}return í;}public static List<
string>Ò(string ç){var Ó=new List<string>();int ð=0;while(ð<ç.Length){int î=ç.IndexOf(Õ.Ö,ð,O.Á);if(î<0)break;int Ã=ç.IndexOf(
']',î);if(Ã<0)break;Ó.Add(ç.Substring(î,Ã-î+1));ð=Ã+1;}return Ó;}}static class Õ{public const string Ö="[CCN:",ñ="]",ò=
"[CCN:LCD]",ó="[CCN:Fleet]",ô="[CCN:Stats]",õ="[CCN:Overflow]",Ù="[CCN:Alert]",ö="[CCN:All]",ø="[CCN:Debug]",ù="[CCN:Power]";public
static string û(string ú){return Ö+ú+ñ;}}class Ą{readonly Dictionary<string,ü>ý=new Dictionary<string,ü>(O.P);readonly þ ÿ;
readonly Ā ā;public Ą(þ Ă,Ā ă){ÿ=Ă;ā=ă;}public void ć(ü ą){ý[ą.Ć]=ą;}public void Ĕ(string Ĉ){if(string.IsNullOrEmpty(Ĉ))return;
string ĉ;string Ċ;int ċ=Ĉ.IndexOf(' ');if(ċ>=0){ĉ=Ĉ.Substring(0,ċ).Trim();Ċ=Ĉ.Substring(ċ+1).Trim();}else{ĉ=Ĉ.Trim();Ċ="";}ü ą
;if(!ý.TryGetValue(ĉ,out ą)){ā.Č("Cmd",$"Unknown command: {ĉ}. Available: {string.Join(", ",ý.Keys)}");return;}if(ą.č!=Ď.
ď&&ą.č!=ÿ.Ď){ā.Č("Cmd",$"{ĉ} not available in {Đ.đ(ÿ.Ď)} mode");return;}ā.Ē("Cmd",ĉ);ą.ē(Ċ,ÿ);}}class Ļ:ü{public string Ć
{get{return"SETUP";}}public Ď č{get{return Ď.ĕ;}}public void ē(string Ė,þ Ă){var ė=Ă.đ<W>();ė.Ì(true);var ă=Ă.đ<Ā>();var
V=Ă.đ<Program>();var Ę=new StringBuilder();Ę.AppendLine($"=== CCN {ę.Ě} Drone Setup ===");Ę.AppendLine(
$"  SAM PB: {(ė.d()!=null?ė.d().CustomName:"NOT FOUND")}");Ę.AppendLine($"  Connectors: {ė.c().Count}");var ě=ė.d();if(ě!=null){string ç=ě.CustomName;bool Ĝ=false;if(ç.IndexOf(
"[SAM LOOP]",O.Á)>=0){ç=ç.Replace("[SAM LOOP]","").Replace("[sam loop]","").Trim();Ĝ=true;}if(ç.IndexOf("[SAM LIST]",O.Á)>=0){ç=ç.
Replace("[SAM LIST]","").Replace("[sam list]","").Trim();Ĝ=true;}if(Ĝ){if(ç.IndexOf("[SAM]",O.Á)<0)ç=ç.TrimEnd()+" [SAM]";ě.
CustomName=ç;Ę.AppendLine($"  Fixed SAM tag: {ě.CustomName}");}string ĝ=ě.CustomName;ĝ=Ğ(ĝ,"MaxSpeed");ĝ=Ğ(ĝ,"DockingSpeed");ĝ=Ğ(ĝ
,"UndockDistance");ĝ=Ğ(ĝ,"DockDistance");ĝ=Ğ(ĝ,"ApproachingSpeed");ĝ=Ğ(ĝ,"TaxiingSpeed");ĝ=Ğ(ĝ,"TaxiingDistance");ě.
CustomName=ĝ;string ğ=ě.CustomData??"";if(!ğ.Contains("SAM.MaxSpeed")){string Ġ="SAM.MaxSpeed=100\n"+"SAM.DockingSpeed=10\n"+
"SAM.UndockDistance=2\n"+"SAM.DockDistance=3\n"+"SAM.ApproachingSpeed=80\n"+"SAM.TaxiingSpeed=30\n"+"SAM.TaxiingDistance=5\n"+
"SAM.ConvergingSpeed=100\n";ě.CustomData=ğ.Length>0?ğ.TrimEnd()+"\n"+Ġ:Ġ;Ę.AppendLine($"  SAM speeds set via CustomData");}else{Ę.AppendLine(
$"  SAM CustomData already configured");}}var ġ=ė.c();for(int É=0;É<ġ.Count;É++){string Ģ=ġ[É].CustomName;if(Ģ.IndexOf("[SAM MAIN]",O.Á)<0&&!Ñ.æ(Ģ)){ġ[É].
CustomName=Ģ.TrimEnd()+" [SAM MAIN]";Ę.AppendLine($"  Connector: {ġ[É].CustomName}");}}string ĥ=ģ.Ĥ(V.Me.CubeGrid.EntityId);string
Ħ=$"Drone-{ĥ}";var ħ=new List<IMyTerminalBlock>();V.GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(ħ,È=>È.CubeGrid==
V.Me.CubeGrid);if(ħ.Count>0){ħ[0].CustomName=Ħ;Ę.AppendLine($"  Antenna: {Ħ}");}if(V.Me.CustomData.IndexOf("[CCN]",O.Á)<0
)V.Me.CustomData=(V.Me.CustomData.TrimEnd()+"\n\n"+Ĩ.ĩ()).TrimStart();Ī(V,Ă.đ<ī>(),ė,Ę);Ę.AppendLine(
"Setup complete -- rebooting modules...");ă.Ē("Setup",Ę.ToString());V.Ĭ();}static void Ī(Program V,ī ĭ,W ė,StringBuilder Ę){string İ=ĭ.Į(ę.į,"HomeDock","").Trim
();IMyShipConnector ı=null;var Ĳ=ė.c();for(int É=0;É<Ĳ.Count;É++){if(Ĳ[É].Closed)continue;if(Ĳ[É].Status==
MyShipConnectorStatus.Connected){ı=Ĳ[É];break;}}if(ı==null){if(İ.Length==0)Ę.AppendLine(
"  WARNING: HomeDock not set — dock this drone at its home connector and run SETUP again");else Ę.AppendLine($"  HomeDock: {İ}");return;}string ĳ=ı.OtherConnector!=null?ı.OtherConnector.CustomName:"";string ĵ=
Ĵ(ĳ);if(ĵ==null){Ę.AppendLine($"  WARNING: docked connector '{ĳ}' has no [SAM Name=...] tag — HomeDock not detected");
return;}if(İ.Length==0||İ==ĵ){V.Me.CustomData=ī.Ķ(V.Me.CustomData,"HomeDock",ĵ);Ę.AppendLine(
$"  HomeDock={ĵ} (detected from docked connector)");}else{Ę.AppendLine(
$"  HomeDock kept: {İ} (currently docked at {ĵ}; blank the HomeDock= line and re-run SETUP to re-detect)");}}static string Ĵ(string ķ){int Â=ķ.IndexOf("[SAM Name=",O.Á);if(Â<0)return null;int î=Â+10;int Ã=ķ.IndexOf(']',î);if(
Ã<=î)return null;return ķ.Substring(î,Ã-î);}static string Ğ(string ç,string ĸ){string Ĺ=ĸ.ToLowerInvariant();int ð=0;
while(ð<ç.Length){int ĺ=ç.IndexOf("[SAM",ð,O.Á);if(ĺ<0)break;int Ã=ç.IndexOf(']',ĺ);if(Ã<0)break;string l=ç.Substring(ĺ,Ã-ĺ+1
);if(l.ToLowerInvariant().Contains(Ĺ)){ç=ç.Substring(0,ĺ).TrimEnd()+ç.Substring(Ã+1);}else{ð=Ã+1;}}return ç;}}class ŀ:ü{
public string Ć{get{return"HOME";}}public Ď č{get{return Ď.ĕ;}}public void ē(string Ė,þ Ă){var Ľ=Ă.đ<ļ>();if(Ľ==null){Ă.đ<Ā>()
.Č("Cmd","Drone not ready -- set HomeDock and run SETUP");return;}Ľ.ľ.Ŀ();}}class ł:ü{public string Ć{get{return"RESUME";
}}public Ď č{get{return Ď.ĕ;}}public void ē(string Ė,þ Ă){var Ľ=Ă.đ<ļ>();if(Ľ==null){Ă.đ<Ā>().Č("Cmd",
"Drone not ready -- set HomeDock and run SETUP");return;}Ľ.ľ.Ł();}}class ń:ü{public string Ć{get{return"STOP";}}public Ď č{get{return Ď.ĕ;}}public void ē(string Ė,þ Ă)
{var Ľ=Ă.đ<ļ>();if(Ľ==null){Ă.đ<Ā>().Č("Cmd","Drone not ready");return;}Ľ.ľ.Ń();}}class ŉ:ü{public string Ć{get{return
"DISMISS";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var Ň=Ă.đ<ņ>();if(Ň==null){Ă.đ<Ā>().Č("Cmd",
"Hub not ready -- run SETUP first");return;}Ň.ň();}}class Ō:ü{public string Ć{get{return"DISPATCH";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ
Ă){if(string.IsNullOrEmpty(Ė)){Ă.đ<Ā>().Č("Cmd","DISPATCH requires a station name");return;}var ŋ=Ă.đ<Ŋ>();if(ŋ==null){Ă.
đ<Ā>().Č("Cmd","Hub not ready -- run SETUP first");return;}ŋ.Ĕ(Ė);}}class Ŏ:ü{public string Ć{get{return"PAUSE";}}public
Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var ŋ=Ă.đ<Ŋ>();if(ŋ==null){Ă.đ<Ā>().Č("Cmd",
"Hub not ready -- run SETUP first");return;}ŋ.ō();}}class Œ:ü{public string Ć{get{return"PURGE";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){
var Ő=Ă.đ<ŏ>();if(Ő==null){Ă.đ<Ā>().Č("Cmd","Hub not ready -- run SETUP first");return;}Ő.ő();}}class Ŕ:ü{public string Ć{
get{return"RECALL";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var ŋ=Ă.đ<Ŋ>();if(ŋ==null){Ă.đ<Ā>().Č("Cmd",
"Hub not ready -- run SETUP first");return;}ŋ.œ();}}class ŕ:ü{public string Ć{get{return"RECONFIG";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ
Ă){var ă=Ă.đ<Ā>();ă.Ē("Reconfig","Rebooting with new config...");Ă.đ<Program>().Ĭ();}}class Ŗ:ü{public string Ć{get{
return"RESET";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var ă=Ă.đ<Ā>();var V=Ă.đ<Program>();V.Storage="";ă.Ē(
"Reset","Cleared saved state");ă.Ē("Reset","Rebooting...");V.Ĭ(false);}}class ś:ü{public string Ć{get{return"RESET_TAGS";}}
public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var V=Ă.đ<Program>();var ŗ=new List<IMyTerminalBlock>();V.
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ŗ,È=>È.CubeGrid==V.Me.CubeGrid);int Ř=0;for(int É=0;É<ŗ.Count;É++){string ř=ŗ[É].
CustomName;string Ś=Ñ.ï(ř);if(Ś!=ř){ŗ[É].CustomName=Ś;Ř++;}}Ă.đ<Ā>().Ē("Tags",$"Stripped CCN tags from {Ř} blocks");}}class Ƅ:ü{
public string Ć{get{return"SETUP";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var ă=Ă.đ<Ā>();var ė=Ă.đ<W>();var
V=Ă.đ<Program>();var ŝ=Ă.đ<Ŝ>();ė.Ì(true);var Ę=new StringBuilder();Ę.AppendLine(
$"=== CCN {ę.Ě} Hub Setup ({Đ.đ(ŝ.Ď)}) ===");var Ş=new List<IMyTerminalBlock>();V.GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(Ş,È=>È.CubeGrid==V.Me.
CubeGrid);int ş=0;var Š=new List<MyInventoryItem>();var š=new HashSet<string>();bool Ţ=false;for(int É=0;É<Ş.Count;É++){var ţ=Ñ.
Ò(Ş[É].CustomName);for(int Ô=0;Ô<ţ.Count;Ô++){if(ţ[Ô].StartsWith(Õ.Ö)){š.Add(ţ[Ô]);if(ţ[Ô]==Õ.ö)Ţ=true;}}}for(int É=0;É<Ş
.Count;É++){var Ť=Ş[É];if(Ñ.é(Ť.CustomName,Õ.Ö))continue;if(Ñ.æ(Ť.CustomName))continue;if(ŝ.Ď==ť.Ŧ){if(Ţ)continue;Ť.
CustomName=Ñ.ì(Ť.CustomName,Õ.ö);Ę.AppendLine($"  All: {Ť.CustomName}");Ţ=true;ş++;continue;}var ŧ=Ť.GetInventory(0);Š.Clear();ŧ.
GetItems(Š);var Ũ=new HashSet<string>();for(int ũ=0;ũ<Š.Count;ũ++){if(Ū.ū(Š[ũ].Type))Ũ.Add(Š[ũ].Type.SubtypeId);}if(Ũ.Count>1){
if(!Ţ){Ť.CustomName=Ñ.ì(Ť.CustomName,Õ.ö);š.Add(Õ.ö);Ţ=true;Ę.AppendLine($"  All (mixed): {Ť.CustomName}");ş++;}}else if(Ũ
.Count==1){string Ŭ=null;foreach(var ŭ in Ũ){Ŭ=ŭ;break;}if(ŝ.Ď==ť.Ů){var Ű=ŝ.ů(Ŭ);if(Ű.Count>0){bool ű=š.Contains(Õ.û(Ű[0
]));if(!ű){for(int Ï=0;Ï<Ű.Count;Ï++){string Ų=Õ.û(Ű[Ï]);Ť.CustomName=Ñ.ì(Ť.CustomName,Ų);š.Add(Ų);}Ę.AppendLine(
$"  Group: {Ť.CustomName}");ş++;}}else{string Ų=Õ.û(Ŭ);if(!š.Contains(Ų)){Ť.CustomName=Ñ.ì(Ť.CustomName,Ų);š.Add(Ų);Ę.AppendLine(
$"  Tagged: {Ť.CustomName}");ş++;}}}else{string Ų=Õ.û(Ŭ);if(!š.Contains(Ų)){Ť.CustomName=Ñ.ì(Ť.CustomName,Ų);š.Add(Ų);Ę.AppendLine(
$"  Tagged: {Ť.CustomName}");ş++;}}}}bool ų=false;for(int É=0;É<Ş.Count;É++){string Ģ=Ş[É].CustomName;if(Ñ.é(Ģ,Õ.ö)){ų=true;break;}var Ŵ=Ñ.Ò(Ģ);for
(int Ô=0;Ô<Ŵ.Count;Ô++){if(Ŵ[Ô]!=Õ.õ&&Ŵ[Ô]!=Õ.Ù&&Ŵ[Ô]!=Õ.ò&&Ŵ[Ô]!=Õ.ó&&Ŵ[Ô]!=Õ.ô&&Ŵ[Ô].StartsWith(Õ.Ö)){ų=true;break;}}if
(ų)break;}if(!ų&&Ş.Count>0){IMyTerminalBlock ŵ=null;for(int É=0;É<Ş.Count;É++){if(Ñ.é(Ş[É].CustomName,Õ.Ö))continue;var Ŷ
=Ş[É].GetInventory(0);if(Ŷ.ItemCount==0){ŵ=Ş[É];break;}}if(ŵ==null){for(int É=0;É<Ş.Count;É++){if(!Ñ.é(Ş[É].CustomName,Õ.
Ö)){ŵ=Ş[É];break;}}}if(ŵ==null){Ę.AppendLine("  No untagged container for [CCN:All]");}else if(ŵ.GetInventory(0).
ItemCount>0){var ŷ=ŵ.GetInventory(0);Š.Clear();ŷ.GetItems(Š);for(int ũ=0;ũ<Š.Count;ũ++){for(int Ÿ=0;Ÿ<Ş.Count;Ÿ++){if(Ş[Ÿ]==ŵ)
continue;var Ź=Ş[Ÿ].GetInventory(0);if(!Ź.IsFull){ŷ.TransferItemTo(Ź,Š[ũ]);break;}}}Ę.AppendLine(
$"  Relocated items from {ŵ.CustomName}");}if(ŵ!=null){ŵ.CustomName=Ñ.ï(ŵ.CustomName);ŵ.CustomName=Ñ.ì(ŵ.CustomName,Õ.ö);Ę.AppendLine(
$"  Auto [CCN:All]: {ŵ.CustomName}");ş++;}}for(int É=0;É<Ş.Count;É++){if(Ñ.é(Ş[É].CustomName,Õ.Ö))ź(Ş[É]);}var Ż=new List<IMyTerminalBlock>();V.
GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(Ż,È=>È.CubeGrid==V.Me.CubeGrid);for(int É=0;É<Ż.Count;É++){var ż=Ñ.Ò(Ż[É].CustomName);for
(int Ô=0;Ô<ż.Count;Ô++){if(ż[Ô].StartsWith(Õ.Ö))š.Add(ż[Ô]);}}string[]Ž={Õ.ò,Õ.ó,Õ.ô};int ž=0;for(int É=0;É<Ż.Count&&ž<Ž.
Length;É++){while(ž<Ž.Length&&š.Contains(Ž[ž]))ž++;if(ž>=Ž.Length)break;if(!Ñ.é(Ż[É].CustomName,Õ.Ö)&&!Ñ.æ(Ż[É].CustomName)){Ż
[É].CustomName=Ñ.ì(Ż[É].CustomName,Ž[ž]);š.Add(Ž[ž]);Ę.AppendLine($"  LCD: {Ż[É].CustomName}");ž++;}}if(V.Me.CustomData.
IndexOf("[CCN]",O.Á)<0)V.Me.CustomData=(V.Me.CustomData.TrimEnd()+"\n\n"+Ĩ.ſ()).TrimStart();var ƀ=new List<IMyTerminalBlock>();
V.GridTerminalSystem.GetBlocksOfType<Sandbox.ModAPI.Ingame.IMyShipConnector>(ƀ,È=>È.CubeGrid==V.Me.CubeGrid);bool Ɓ=false
;for(int É=0;É<ƀ.Count;É++){bool Ƃ=ƀ[É].CustomName.IndexOf("[SAM Name",O.Á)>=0;if(!Ƃ)continue;if(ƀ[É].BlockDefinition.
SubtypeId=="LargeBlockStructural_PlatformConnector")Ę.AppendLine(
$"  WARNING: {ƀ[É].CustomName}: SAM can't dock to Structural Platform Connectors");else Ɓ=true;}if(!Ɓ)Ę.AppendLine("  WARNING: no [SAM Name=...] connector — drones can't dock; tag your main connector")
;var ƃ=new List<IMyTerminalBlock>();V.GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(ƃ,È=>È.CubeGrid==V.Me.
CubeGrid);for(int É=0;É<ƃ.Count;É++){if(ƃ[É]==V.Me)continue;string ĝ=ƃ[É].CustomName;if(!Ñ.æ(ĝ))continue;if(ĝ.IndexOf(
"[SAM ADVERTISE]",O.Á)>=0){Ę.AppendLine($"  Hub SAM PB: already advertising");continue;}ĝ=ĝ.Replace("[SAM]","").Replace("[sam]","").Trim(
);ĝ=ĝ.Replace("[SAM LOOP]","").Replace("[SAM LIST]","").Trim();ĝ=ĝ.TrimEnd()+" [SAM ADVERTISE]";ƃ[É].CustomName=ĝ;Ę.
AppendLine($"  Hub SAM PB: {ƃ[É].CustomName}");}ė.Ì(true);Ę.AppendLine($"  Containers tagged: {ş}");Ę.AppendLine(
$"  LCDs tagged: {ž}");Ę.AppendLine("Setup complete — rebooting modules...");ă.Ē("Setup",Ę.ToString());V.Ĭ();}static void ź(IMyTerminalBlock
Ø){var À=Ø.CustomData;if(!string.IsNullOrEmpty(À)&&À.IndexOf("FillLimit=",O.Á)>=0)return;if(string.IsNullOrEmpty(À))Ø.
CustomData="FillLimit=100";else Ø.CustomData=À.TrimEnd()+"\nFillLimit=100";}}class Ƈ:ü{public string Ć{get{return"START";}}public
Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var ă=Ă.đ<Ā>();ă.Ē("Cmd",$"START received (mode={Đ.đ(Ă.Ď)})");var ŋ=Ă.đ<
Ŋ>();if(ŋ==null){ă.Č("Cmd","Hub not ready -- HubLoop is null, run SETUP first");return;}ŋ.ƅ();ă.Ē("Cmd",
$"Fleet state: {Đ.đ(ŋ.Ɔ)}");}}class ƍ:ü{public string Ć{get{return"STOP";}}public Ď č{get{return Ď.Ņ;}}public void ē(string Ė,þ Ă){var ă=Ă.đ<Ā>();
var Ɖ=Ă.đ<ƈ>();if(Ɖ==null){ă.Č("Cmd","Hub not ready");return;}Ɖ.Ɗ(Ƌ.ƌ,"hub_stop");ă.Č("Cmd",
"EMERGENCY STOP — all drones halted");var ŋ=Ă.đ<Ŋ>();if(ŋ!=null)ŋ.ō();}}class Ə:ü{public string Ć{get{return"TEST_ALERT";}}public Ď č{get{return Ď.Ņ;}}
public void ē(string Ė,þ Ă){var Ň=Ă.đ<ņ>();if(Ň==null){Ă.đ<Ā>().Č("Cmd","Hub not ready -- run SETUP first");return;}Ň.Ǝ();}}
interface ü{string Ć{get;}Ď č{get;}void ē(string Ė,þ Ă);}class Ƒ:ü{public string Ć{get{return"RECONFIG";}}public Ď č{get{return Ď
.Ɛ;}}public void ē(string Ė,þ Ă){var ă=Ă.đ<Ā>();ă.Ē("Reconfig","Rebooting with new config...");Ă.đ<Program>().Ĭ();}}class
ƥ:ü{public string Ć{get{return"SETUP";}}public Ď č{get{return Ď.Ɛ;}}public void ē(string Ė,þ Ă){var ă=Ă.đ<Ā>();var ė=Ă.đ<
W>();var V=Ă.đ<Program>();var ĭ=Ă.đ<ī>();ė.Ì(true);var Ę=new StringBuilder();Ę.AppendLine(
$"=== CCN {ę.Ě} Station Setup ===");Ę.AppendLine($"  Drills: {ė.a().Count}");string Ɠ=ƒ(ė);if(string.IsNullOrEmpty(Ɠ))Ɠ=ĭ.Į(ę.į,"StationName","");if(
string.IsNullOrEmpty(Ɠ)){Ɠ=Ɣ(ė,V,Ę);if(Ɠ!=null)Ę.AppendLine($"  Detected ore: {Ɠ}");}if(string.IsNullOrEmpty(Ɠ)){long p=ė.a().
Count>0?((IMyTerminalBlock)ė.a()[0]).CubeGrid.EntityId:V.Me.CubeGrid.EntityId;Ɠ="Station-"+ģ.Ĥ(p);}Ę.AppendLine(
$"  Station name: {Ɠ}");ƕ(ė,Ɠ,Ę);Ɩ(V,Ɠ,Ę);Ɨ(V,Ę);if(V.Me.CustomData.IndexOf("[CCN]",O.Á)<0)V.Me.CustomData=(V.Me.CustomData.TrimEnd()+"\n\n"+Ĩ
.Ƙ()).TrimStart();if(ĭ.Į(ę.į,"StationName","")!=Ɠ){string ƙ=V.Me.CustomData;if(ƙ.Contains("StationName=")){int î=ƙ.
IndexOf("StationName=");int Ã=ƙ.IndexOf('\n',î);if(Ã<0)Ã=ƙ.Length;ƙ=ƙ.Substring(0,î)+$"StationName={Ɠ}"+ƙ.Substring(Ã);}else{ƙ=
ƙ.TrimEnd()+$"\nStationName={Ɠ}\n";}V.Me.CustomData=ƙ;Ę.AppendLine($"  CustomData updated with StationName={Ɠ}");}var Ż=
new List<IMyTerminalBlock>();V.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(Ż,È=>È.CubeGrid==V.Me.CubeGrid);bool ƚ=
false;for(int É=0;É<Ż.Count;É++){if(Ñ.é(Ż[É].CustomName,Õ.ô)){ƚ=true;break;}}if(!ƚ){for(int É=0;É<Ż.Count;É++){if(!Ñ.é(Ż[É].
CustomName,Õ.Ö)&&!Ñ.æ(Ż[É].CustomName)){Ż[É].CustomName=Ñ.ì(Ż[É].CustomName,Õ.ô);Ę.AppendLine($"  LCD: {Ż[É].CustomName}");break;}
}}ė.Ì(true);Ę.AppendLine("Setup complete — rebooting modules...");ă.Ē("Setup",Ę.ToString());V.Ĭ();}static string ƒ(W ė){
var ġ=ė.c();for(int É=0;É<ġ.Count;É++){string ç=ġ[É].CustomName;int ĺ=ç.IndexOf("[SAM Name=");if(ĺ>=0){int î=ĺ+10;int Ã=ç.
IndexOf(']',î);if(Ã>î)return ç.Substring(î,Ã-î);}}return null;}static string Ɣ(W ė,Program V,StringBuilder Ę){var Š=new List<
MyInventoryItem>();string Ŭ=null;float ƛ=0;var Ɯ=ė.a();for(int É=0;É<Ɯ.Count;É++)Ɲ((IMyTerminalBlock)Ɯ[É],Š,ref Ŭ,ref ƛ);var ƞ=new List
<IMyTerminalBlock>();V.GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(ƞ,È=>È.CubeGrid==V.Me.CubeGrid);for(int É=0;
É<ƞ.Count;É++)Ɲ(ƞ[É],Š,ref Ŭ,ref ƛ);return Ŭ;}static void Ɲ(IMyTerminalBlock Ø,List<MyInventoryItem>Š,ref string Ŭ,ref
float ƛ){for(int ŧ=0;ŧ<Ø.InventoryCount;ŧ++){Š.Clear();Ø.GetInventory(ŧ).GetItems(Š);for(int ũ=0;ũ<Š.Count;ũ++){if(!Ū.ū(Š[ũ].
Type))continue;float Ɵ=(float)Š[ũ].Amount;if(Ɵ>ƛ){ƛ=Ɵ;Ŭ=Š[ũ].Type.SubtypeId;}}}}const string Ơ=
"LargeBlockStructural_PlatformConnector";static void ƕ(W ė,string Ɠ,StringBuilder Ę){var ġ=ė.c();bool ơ=false;for(int É=0;É<ġ.Count;É++){if(ġ[É].CustomName.
IndexOf("[SAM Name=")>=0){ơ=true;if(ġ[É].BlockDefinition.SubtypeId==Ơ)Ę.AppendLine(
$"  WARNING: {ġ[É].CustomName}: SAM can't dock to Structural Platform Connectors — retag a standard one");break;}}if(!ơ&&ġ.Count>0){IMyShipConnector Ƣ=null;for(int É=0;É<ġ.Count;É++){if(ġ[É].BlockDefinition.SubtypeId!=Ơ){Ƣ=ġ
[É];break;}}if(Ƣ==null){Ę.AppendLine("  WARNING: only Structural Platform Connectors — add a standard connector for SAM")
;return;}string ç=Ƣ.CustomName;if(Ñ.æ(ç))ç=ç.Replace("[SAM]","").Trim();Ƣ.CustomName=ç+$" [SAM Name={Ɠ}]";Ę.AppendLine(
$"  Connector: {Ƣ.CustomName}");}else if(ơ){Ę.AppendLine("  Connector: SAM Name already set");}}static void Ɩ(Program V,string Ɠ,StringBuilder Ę){var
ħ=new List<IMyTerminalBlock>();V.GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(ħ,È=>È.CubeGrid==V.Me.CubeGrid);if(ħ
.Count>0){var ƣ=ħ[0];if(!ƣ.CustomName.Contains(Ɠ)){ƣ.CustomName=$"{Ɠ}";Ę.AppendLine($"  Antenna: {ƣ.CustomName}");}else{Ę
.AppendLine("  Antenna: already named");}}else{Ę.AppendLine("  Antenna: none found");}}static void Ɨ(Program V,
StringBuilder Ę){var Ƥ=new List<IMyTerminalBlock>();V.GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(Ƥ,È=>È.CubeGrid==V.Me.
CubeGrid);for(int É=0;É<Ƥ.Count;É++){if(Ƥ[É]==V.Me)continue;string ç=Ƥ[É].CustomName;if(!Ñ.æ(ç))continue;if(ç.IndexOf(
"[SAM ADVERTISE]",O.Á)>=0){Ę.AppendLine($"  SAM PB: already advertising");continue;}if(ç.IndexOf("[SAM LOOP]",O.Á)>=0)ç=ç.Replace(
"[SAM LOOP]","").Replace("[sam loop]","").Trim();if(ç.IndexOf("[SAM LIST]",O.Á)>=0)ç=ç.Replace("[SAM LIST]","").Replace("[sam list]"
,"").Trim();if(ç.IndexOf("[SAM]",O.Á)>=0)ç=ç.Replace("[SAM]","").Replace("[sam]","").Trim();ç=ç.TrimEnd()+
" [SAM ADVERTISE]";Ƥ[É].CustomName=ç;Ę.AppendLine($"  SAM PB: {Ƥ[É].CustomName}");}}}class ƈ{const int Ʀ=32;readonly Program A;readonly Ā
ā;readonly string Ƨ;readonly IMyBroadcastListener ƨ;readonly Dictionary<string,Ʃ>ƪ=new Dictionary<string,Ʃ>();int ƫ,Ƭ;
public ƈ(Program V,Ā ă,string ƭ){A=V;ā=ă;Ƨ=ƭ;ƨ=V.IGC.RegisterBroadcastListener(Ƨ);}public void ư(string Ʈ,Ʃ Ư){ƪ[Ʈ]=Ư;}public
void Ɗ(string Ʊ,params object[]Ʋ){string Ƶ=Ƴ.ƴ(Ʊ,Ʋ);A.IGC.SendBroadcastMessage(Ƨ,Ƶ);}public void Ʒ(long ƶ,string Ʊ,params
object[]Ʋ){string Ƶ=Ƴ.ƴ(Ʊ,Ʋ);A.IGC.SendUnicastMessage(ƶ,Ƨ,Ƶ);}public void ƻ(){int Ƹ=Ʀ;while(Ƹ-->0&&ƨ.HasPendingMessage){var ƹ=
ƨ.AcceptMessage();ƺ(ƹ.Data?.ToString(),ƹ.Source);}while(Ƹ-->0&&A.IGC.UnicastListener.HasPendingMessage){var ƹ=A.IGC.
UnicastListener.AcceptMessage();if(ƹ.Tag!=Ƨ){ƫ++;continue;}ƺ(ƹ.Data?.ToString(),ƹ.Source);}}void ƺ(string Ƽ,long ƽ){var í=ƾ.ƿ(Ƽ);if(!í.
ǀ){Ƭ++;ā.Č("IGC",í.ǁ);return;}Ʃ Ư;if(ƪ.TryGetValue(í.ǂ.ǃ,out Ư))Ư.Ǆ(í.ǂ,ƽ);else ƫ++;}}static class Ƌ{public const int ǅ=4
;public const string ǆ="CCN",Ǉ="REPORT",ǈ="STATUS",ǉ="NETID",Ĕ="DISPATCH",Ǌ="COME_HOME",ƌ="EMERGENCY_STOP",ǋ="HOLD",ǌ=
"PROCEED";}interface Ʃ{void Ǆ(Ǎ Ƶ,long ƽ);}static class Ƴ{public static string ƴ(string Ʊ,params object[]Ʋ){var Ę=new
StringBuilder(64);Ę.Append('V');Ę.Append(Ƌ.ǅ);Ę.Append('|');ǎ(Ę,Ʊ);for(int É=0;É<Ʋ.Length;É++){Ę.Append('|');string Ä=Ʋ[É]!=null?Ʋ[É]
.ToString():"";ǎ(Ę,Ä);}return Ę.ToString();}static void ǎ(StringBuilder Ę,string Ǐ){for(int É=0;É<Ǐ.Length;É++){char Ť=Ǐ[
É];if(Ť=='\\'){Ę.Append('\\');Ę.Append('\\');}else if(Ť=='|'){Ę.Append('\\');Ę.Append('|');}else Ę.Append(Ť);}}}struct Ǎ{
public int Ě;public string ǃ;public IList<string>ǐ;}static class ƾ{public static Ǒ<Ǎ>ƿ(string Ƽ){if(string.IsNullOrEmpty(Ƽ))
return Ǒ<Ǎ>.ǒ("Empty message");var Ʋ=new List<string>();var Ę=new StringBuilder();bool Ǔ=false;for(int É=0;É<Ƽ.Length;É++){
char Ť=Ƽ[É];if(Ǔ){Ę.Append(Ť);Ǔ=false;continue;}if(Ť=='\\'){Ǔ=true;continue;}if(Ť=='|'){Ʋ.Add(Ę.ToString());Ę.Clear();
continue;}Ę.Append(Ť);}Ʋ.Add(Ę.ToString());if(Ʋ.Count<2)return Ǒ<Ǎ>.ǒ("Too few fields");string ǔ=Ʋ[0];if(ǔ.Length==0||ǔ[0]!='V')
return Ǒ<Ǎ>.ǒ("Missing V prefix");int Ǖ;if(!int.TryParse(ǔ.Substring(1),out Ǖ))return Ǒ<Ǎ>.ǒ("Bad version");if(Ǖ!=Ƌ.ǅ)return Ǒ
<Ǎ>.ǒ($"Version mismatch: {Ǖ}");string Ʊ=Ʋ[1];var ǖ=new List<string>();for(int É=2;É<Ʋ.Count;É++)ǖ.Add(Ʋ[É]);var Ƶ=new Ǎ(
);Ƶ.Ě=Ǖ;Ƶ.ǃ=Ʊ;Ƶ.ǐ=ǖ;return Ǒ<Ǎ>.Ǘ(Ƶ);}}static class Ĩ{public const int Ǚ=ę.ǘ,Ǜ=ę.ǚ,ǝ=ę.ǜ,ǟ=ę.Ǟ,Ǡ=15,ǡ=300,Ǣ=250;public
const string ǣ="",Ǥ="",ť="Dedicated",ǥ="RoundRobin";public const bool Ǧ=false;public static string ſ(){return"[CCN]\n"+
"Mode=Hub\n"+$"SortMode={ť}\n"+"; SortMode options: Dedicated, Single, Group\n"+"; OreGroup1=Iron,Nickel,Cobalt\n"+
"; OreGroup2=Gold,Silver,Platinum\n"+$"DispatchMode={ǥ}\n"+"; DispatchMode options: RoundRobin, Priority\n"+"; StationPriority=Iron,Gold,Cobalt\n"+
$"CloseThreshold={Ǚ}\n"+$"ReopenThreshold={Ǜ}\n"+$"ScheduleRun={ǝ}\n"+$"SchedulePause={ǟ}\n"+$"LostTimeout={Ǡ}\n"+
"; LostTimeout: secs of silence before LOST\n"+$"HubZoneRadius={ǡ}\n"+"; HubZoneRadius: m — one moving drone near hub at a time (0=off)\n"+$"SeparationRadius={Ǣ}\n"+
"; SeparationRadius: m — en-route drone separation (0=off)\n"+"NetworkId=\n"+"; NetworkId: auto-generated by the hub; drones/stations adopt it\n";}public static string ĩ(){return
"[CCN]\n"+"Mode=Drone\n"+"HomeDock=\n"+"NetworkId=\n";}public static string Ƙ(){return"[CCN]\n"+"Mode=Station\n"+"StationName=\n"
+"Enabled=true\n"+"NetworkId=\n";}}class ī{readonly Program A;string ǧ;readonly Dictionary<string,Dictionary<string,
string>>Ǩ=new Dictionary<string,Dictionary<string,string>>(O.P);static readonly Dictionary<string,string>ǩ=new Dictionary<
string,string>();public ī(Program V){A=V;Ǫ();}public bool Ǫ(){string Ƽ=A.Me.CustomData;string ǫ=Ƽ.GetHashCode().ToString();if(
ǫ==ǧ)return false;ǧ=ǫ;Ǩ.Clear();ƿ(Ƽ);return true;}void ƿ(string Ƽ){string Ǭ="";string[]ǭ=Ƽ.Split('\n');for(int É=0;É<ǭ.
Length;É++){string Ǯ=ǭ[É].TrimEnd('\r').Trim();if(Ǯ.Length==0)continue;if(Ǯ[0]==';'||Ǯ[0]=='#')continue;if(Ǯ[0]=='['){int Ã=Ǯ.
IndexOf(']');if(Ã>1)Ǭ=Ǯ.Substring(1,Ã-1);continue;}int ǯ=Ǯ.IndexOf('=');if(ǯ<=0)continue;string ǰ=Ǯ.Substring(0,ǯ).Trim();
string Ǐ=Ǯ.Substring(ǯ+1).Trim();Dictionary<string,string>Ǳ;if(!Ǩ.TryGetValue(Ǭ,out Ǳ)){Ǳ=new Dictionary<string,string>(O.P);Ǩ
[Ǭ]=Ǳ;}Ǳ[ǰ]=Ǐ;}}public string Į(string ǲ,string ǰ,string ǳ){Dictionary<string,string>Ǳ;if(!Ǩ.TryGetValue(ǲ,out Ǳ))return
ǳ;string Ä;return Ǳ.TryGetValue(ǰ,out Ä)?Ä:ǳ;}public int Ƕ(string ǲ,string ǰ,int ǳ,int Ǵ,int ǵ){string Ƽ=Į(ǲ,ǰ,null);if(Ƽ
==null)return ǳ;int Ä;if(!int.TryParse(Ƽ,out Ä))return ǳ;return Math.Max(Ǵ,Math.Min(ǵ,Ä));}public bool Ƿ(string ǲ,string ǰ
,bool ǳ){string Ƽ=Į(ǲ,ǰ,null);if(Ƽ==null)return ǳ;if(Ƽ=="1"||Ƽ.Equals("true",O.Á))return true;if(Ƽ=="0"||Ƽ.Equals("false"
,O.Á))return false;return ǳ;}public IEnumerable<KeyValuePair<string,string>>Ǹ(string ǲ){Dictionary<string,string>Ǳ;if(Ǩ.
TryGetValue(ǲ,out Ǳ))return Ǳ;return ǩ;}public static string Ķ(string ǹ,string ǰ,string Ǐ){ǹ=ǹ??"";string Ǯ=ǰ+"="+Ǐ;int Â=ǹ.IndexOf
(ǰ+"=",O.Á);if(Â>=0){int Ã=ǹ.IndexOf('\n',Â);if(Ã<0)Ã=ǹ.Length;return ǹ.Substring(0,Â)+Ǯ+ǹ.Substring(Ã);}int Ǳ=ǹ.IndexOf(
"[CCN]",O.Á);if(Ǳ>=0){int Ã=ǹ.IndexOf('\n',Ǳ);if(Ã<0)return ǹ+"\n"+Ǯ;return ǹ.Substring(0,Ã+1)+Ǯ+"\n"+ǹ.Substring(Ã+1);}return(
ǹ.TrimEnd()+"\n[CCN]\n"+Ǯ+"\n").TrimStart();}}enum ǥ{Ǻ,ǻ}struct ǽ{public readonly string ǣ;public readonly bool Ǧ;ǽ(
string İ,bool Ǽ){ǣ=İ;Ǧ=Ǽ;}public static Ǒ<ǽ>Ĥ(ī Ǿ,Ā ă){string ǿ=ę.į;string Ȁ=Ǿ.Į(ǿ,"HomeDock",Ĩ.ǣ);if(string.IsNullOrEmpty(Ȁ))
return Ǒ<ǽ>.ǒ("HomeDock not set — dock at the home connector and run SETUP");return Ǒ<ǽ>.Ǘ(new ǽ(Ȁ,Ǿ.Ƿ(ǿ,"Trace",Ĩ.Ǧ)));}}
class ȅ{readonly Dictionary<string,string>ȁ=new Dictionary<string,string>();const string Ö="DroneName.";public void ȃ(ī Ǿ){ȁ.
Clear();foreach(var Ȃ in Ǿ.Ǹ(ę.į)){if(Ȃ.Key.StartsWith(Ö)&&Ȃ.Key.Length>Ö.Length){string ĥ=Ȃ.Key.Substring(Ö.Length).
ToUpperInvariant();ȁ[ĥ]=Ȃ.Value;}}}public string Ȅ(string ĥ){string ç;if(ȁ.TryGetValue(ĥ,out ç))return ç;if(ĥ.Length>4&&ȁ.TryGetValue(ĥ.
Substring(ĥ.Length-4),out ç))return ç;return$"Drone-{ĥ}";}}struct ȏ{public readonly int Ǚ,Ǜ,Ȇ,ȇ,Ȉ,ǡ;public readonly bool Ǧ;ȏ(int
ȉ,int Ȋ,int ȋ,int Ȍ,int ȍ,int Ȏ,bool Ǽ){Ǚ=ȉ;Ǜ=Ȋ;Ȇ=ȋ;ȇ=Ȍ;Ȉ=ȍ;ǡ=Ȏ;Ǧ=Ǽ;}public static Ǒ<ȏ>Ĥ(ī Ǿ,Ā ă){string ǿ=ę.į;int ȉ=Ǿ.Ƕ(
ǿ,"CloseThreshold",Ĩ.Ǚ,1,100);int Ȋ=Ǿ.Ƕ(ǿ,"ReopenThreshold",Ĩ.Ǜ,0,99);if(Ȋ>=ȉ){ă.ǁ("Config",
$"ReopenThreshold({Ȋ}) >= CloseThreshold({ȉ}), using defaults");ȉ=Ĩ.Ǚ;Ȋ=Ĩ.Ǜ;}return Ǒ<ȏ>.Ǘ(new ȏ(ȉ,Ȋ,Ǿ.Ƕ(ǿ,"ScheduleRun",Ĩ.ǝ,0,1440),Ǿ.Ƕ(ǿ,"SchedulePause",Ĩ.ǟ,0,1440),Ǿ.Ƕ(ǿ,
"LostTimeout",Ĩ.Ǡ,5,300),Ǿ.Ƕ(ǿ,"HubZoneRadius",Ĩ.ǡ,0,5000),Ǿ.Ƿ(ǿ,"Trace",Ĩ.Ǧ)));}}class Ŝ{public readonly ť Ď;readonly Dictionary<
string,List<string>>Ȑ;static readonly List<string>ȑ=new List<string>();public Ŝ(ī Ǿ){Ȑ=new Dictionary<string,List<string>>(O.P
);string ȓ=Ǿ.Į(ę.į,ę.Ȓ,Ĩ.ť);Ď=Ȕ(ȓ);if(Ď!=ť.Ů)return;foreach(var Ȃ in Ǿ.Ǹ(ę.į)){if(!Ȃ.Key.StartsWith(ę.ȕ))continue;string[
]Ȗ=Ȃ.Value.Split(',');var ȗ=new List<string>();for(int É=0;É<Ȗ.Length;É++){string Ș=Ȗ[É].Trim();if(Ș.Length>0)ȗ.Add(Ș);}
for(int É=0;É<ȗ.Count;É++)Ȑ[ȗ[É]]=ȗ;}}public List<string>ů(string ú){List<string>ș;return Ȑ.TryGetValue(ú,out ș)?ș:ȑ;}
static ť Ȕ(string ǿ){if(ǿ==null)return ť.Ț;if(ǿ.Equals("Single",O.Á))return ť.Ŧ;if(ǿ.Equals("Group",O.Á))return ť.Ů;return ť.Ț
;}}enum ť{Ț,Ŧ,Ů}struct ț{public readonly string Ǥ;public readonly bool Ǧ;ț(string ç,bool Ǽ){Ǥ=ç;Ǧ=Ǽ;}public static Ǒ<ț>Ĥ(
ī Ǿ){string ǿ=ę.į;return Ǒ<ț>.Ǘ(new ț(Ǿ.Į(ǿ,"StationName",Ĩ.Ǥ),Ǿ.Ƿ(ǿ,"Trace",Ĩ.Ǧ)));}}static class O{public const System.
StringComparison Á=System.StringComparison.OrdinalIgnoreCase,Ȝ=System.StringComparison.Ordinal;public static readonly System.
StringComparer P=System.StringComparer.OrdinalIgnoreCase;public static readonly System.Globalization.CultureInfo ȝ=System.
Globalization.CultureInfo.InvariantCulture;public const System.Globalization.NumberStyles Ȟ=System.Globalization.NumberStyles.Float;}
static class ę{public const int ȟ=3,Ƞ=10,ȡ=60,Ȣ=300,ȣ=600,Ȥ=300,ȥ=30,Ȧ=1800,ȧ=5,ǘ=90,ǚ=70,ǜ=0,Ǟ=0;public const float Ȩ=0.95f,ȩ
=20f;public const string į="CCN",Ě="v4.1",Ȓ="SortMode",ȕ="OreGroup",Ȫ="DispatchMode";}class ȫ{readonly Program A;readonly
Ā ā;public ȫ(Program V,Ā ă){A=V;ā=ă;}public þ ɍ(Ď Ȭ=Ď.ď){var Ȯ=new ȭ();var ĭ=new ī(A);var ė=new W(A);ė.Ì(true);var Ȱ=new
ȯ();Ď Ȳ=Ȭ!=Ď.ď?Ȭ:Ȱ.ȱ(ė,ĭ,A.Me.CubeGrid.IsStatic);ā.Ē("Boot",$"Mode: {Đ.đ(Ȳ)} (Drills:{ė.i} RC:{ė.j})");ā.Ē("Boot",
$"Blocks scanned: {ė.k}, Connectors: {ė.c().Count}");if(A.Me.CustomData.IndexOf("[CCN]",O.Á)<0){string ȳ="";switch(Ȳ){case Ď.Ņ:ȳ=Ĩ.ſ();break;case Ď.Ɛ:ȳ=Ĩ.Ƙ();break;case Ď.
ĕ:ȳ=Ĩ.ĩ();break;}string ȴ=A.Me.CustomData;A.Me.CustomData=string.IsNullOrEmpty(ȴ)?ȳ:ȴ.TrimEnd()+"\n\n"+ȳ;ĭ.Ǫ();ā.Ē("Boot"
,"Default config written to CustomData");}bool Ǽ=ĭ.Ƿ(ę.į,"Trace",false);ā.ȵ=Ǽ;var Ă=new þ(Ȯ);Ă.Ď=Ȳ;Ă.ć(ĭ);Ă.ć(ė);Ă.ć(Ȯ);Ă
.ć(ā);Ă.ć(A);var ȷ=new ȶ();Ă.ć(ȷ);var ȹ=new ȸ();var Ƹ=new Ⱥ(A);Ă.ć(Ƹ);string Ȼ=ĭ.Į(ę.į,"NetworkId","").Trim();if(Ȳ==Ď.Ņ&&
Ȼ.Length==0){Ȼ=new Random().Next(10000000,100000000).ToString();A.Me.CustomData=ī.Ķ(A.Me.CustomData,"NetworkId",Ȼ);ĭ.Ǫ();
ā.Ē("Boot",$"Generated NetworkId: {Ȼ}");}string ƭ=Ȼ.Length>0?Ƌ.ǆ+"-"+Ȼ:Ƌ.ǆ;var Ɖ=new ƈ(A,ā,ƭ);Ă.ć(Ɖ);if(Ȼ.Length>0)ā.Ē(
"Boot",$"IGC channel: {ƭ}");if(Ȳ==Ď.Ņ){int ȼ=ĭ.Ƕ(ę.į,"SeparationRadius",Ĩ.Ǣ,0,5000);int Ƚ=ĭ.Ƕ(ę.į,"HubZoneRadius",Ĩ.ǡ,0,5000);
Ă.Ⱦ(()=>{var ȿ=A.Me.GetPosition();string ɀ=Ƴ.ƴ(Ƌ.ǉ,Ȼ,ȼ,Ƚ,(long)ȿ.X,(long)ȿ.Y,(long)ȿ.Z);A.IGC.SendBroadcastMessage(Ƌ.ǆ,ɀ)
;if(ƭ!=Ƌ.ǆ)A.IGC.SendBroadcastMessage(ƭ,ɀ);});}else{Ɖ.ư(Ƌ.ǉ,new Ɂ(A,ā,Ă,Ȼ));if(Ȼ.Length==0)ā.Ē("Boot",
"No NetworkId — listening for hub broadcast");}var Ƀ=new ɂ(A,ā);Ă.ć(Ƀ);Ă.Ʉ(()=>Ɖ.ƻ());Ă.Ⱦ(()=>ė.Ì(false));Ă.Ʌ(()=>{if(!ĭ.Ǫ())return;var Ɇ=Ă.đ<ȅ>();var ɇ=Ă.đ<ŏ>();if
(Ɇ!=null&&ɇ!=null){Ɇ.ȃ(ĭ);ɇ.Ɉ(Ɇ);}ā.Ē("Config","Config changed — RECONFIG to apply");});var ɉ=new Ą(Ă,ā);Ă.ć(ɉ);if(Ȳ==Ď.ĕ
)ė.Y(true);switch(Ȳ){case Ď.Ņ:Ɋ(Ă,ĭ,ė,Ɖ,ȷ,ȹ,Ƹ,Ȯ,ɉ);break;case Ď.Ɛ:ɋ(Ă,ĭ,ė,Ɖ,ȷ,ȹ,Ƹ,Ȯ,ɉ);break;case Ď.ĕ:Ɍ(Ă,ĭ,ė,Ɖ,ȷ,ȹ,Ƹ,Ȯ,ɉ
);break;}Ƀ.ȃ(A.Storage,Ă);A.Runtime.UpdateFrequency=UpdateFrequency.Update10;return Ă;}void Ɋ(þ Ɏ,ī ĭ,W ė,ƈ Ɖ,ȶ ȷ,ȸ ȹ,Ⱥ Ƹ
,ȭ Ȯ,Ą ɉ){ɉ.ć(new Ƅ());ɉ.ć(new Ƈ());ɉ.ć(new Ŏ());ɉ.ć(new Ŕ());ɉ.ć(new ŕ());ɉ.ć(new Ŗ());ɉ.ć(new ś());ɉ.ć(new Œ());ɉ.ć(new
Ə());ɉ.ć(new ŉ());ɉ.ć(new Ō());ɉ.ć(new ƍ());ā.Ē("Boot","Hub commands registered");var ɏ=ȏ.Ĥ(ĭ,ā);if(!ɏ.ǀ){ā.Č("Boot",
$"{ɏ.ǁ} -- run SETUP or fix CustomData");return;}ā.Ē("Boot",$"HubConfig OK: Close={ɏ.ǂ.Ǚ}, Reopen={ɏ.ǂ.Ǜ}");var ɐ=ɏ.ǂ;var ɑ=new ȅ();ɑ.ȃ(ĭ);Ɏ.ć(ɑ);var ŝ=new Ŝ(ĭ
);Ɏ.ć(ŝ);ā.Ē("Boot",$"SortMode: {Đ.đ(ŝ.Ď)}");var Ő=new ŏ(ā,Ȯ,ɑ,ɐ.Ȉ);Ɏ.ć(Ő);var ɓ=new ɒ(ā);Ɏ.ć(ɓ);string ɔ=ĭ.Į(ę.į,ę.Ȫ,Ĩ.ǥ
);ǥ ɕ=ǥ.Ǻ;if(ɔ.Equals("Priority",O.Á))ɕ=ǥ.ǻ;ā.Ē("Boot",$"DispatchMode: {Đ.đ(ɕ)}");string ɖ=ĭ.Į(ę.į,"StationPriority","");
string[]ɗ=null;if(!string.IsNullOrEmpty(ɖ)){ɗ=ɖ.Split(',');for(int É=0;É<ɗ.Length;É++)ɗ[É]=ɗ[É].Trim();ā.Ē("Boot",
$"StationPriority: {ɖ}");}var ə=new ɘ(ė,A,ā);Ɏ.ć(ə);var ɛ=new ɚ(Ő,ɓ,ɐ,ɕ,Ȯ,ɗ,ə,ė,ā);Ɏ.ć(ɛ);var Ň=new ņ(ė,ā);Ɏ.ć(Ň);var ɝ=new ɜ(ā);ɝ.ɞ(ɐ.Ȇ,ɐ.ȇ);Ɏ
.ć(ɝ);var ɟ=new Ŋ(A,ā,Ɏ,Ɖ,Ő,ɓ,ɛ,ə,Ň,ɝ,ɐ);Ɏ.ć(ɟ);ā.Ē("Boot","Hub modules wired — ready for START");var ɡ=new ɠ(ė,ȷ);ɡ.ɢ(Õ.
ò,new ɣ());ɡ.ɢ(Õ.ó,new ɤ());ɡ.ɢ(Õ.ô,new ɥ());ɡ.ɢ(Õ.ø,new ɦ(ā));Ɏ.ć(ɡ);var ɨ=new ɧ();var ɪ=new ɩ();var ɬ=new ɫ(ė);ɬ.ɭ();Ɏ.
Ⱦ(()=>ɬ.ɭ());Ɏ.Ʉ(()=>{ɟ.ɮ();var ɰ=ɯ(Ɏ,Ő,ɓ,Ň,ɝ,ɟ.Ɔ,ȷ,ȹ,Ƹ,Ȯ);ɰ.ɱ=ə.ɱ;ɰ.ɲ=ɛ.ɲ;ɰ.ɳ=ɬ;ɡ.ɴ(ɰ);if(Ȯ.ɵ)ɶ(A,ɨ,ɪ,ɰ);});Ɏ.Ʌ(()=>ɟ.ɷ(
));Ɏ.Ⱦ(()=>ɟ.ɸ());Ɏ.ɹ(()=>Ő.ő());}void ɋ(þ Ɏ,ī ĭ,W ė,ƈ Ɖ,ȶ ȷ,ȸ ȹ,Ⱥ Ƹ,ȭ Ȯ,Ą ɉ){ɉ.ć(new ƥ());ɉ.ć(new Ƒ());var ɺ=ț.Ĥ(ĭ);if(!
ɺ.ǀ){ā.Č("Boot",$"{ɺ.ǁ} — run SETUP or fix CustomData");return;}var ɼ=new ɻ(A,ā,ė,Ɖ,ĭ,ɺ.ǂ);Ɏ.ć(ɼ);var ɾ=new ɽ(ɼ);var ɨ=
new ɧ();var ɪ=new ɩ();var ɿ=new ɩ();var ɡ=new ɠ(ė,ȷ);ɡ.ɢ(Õ.ô,ɾ);ɡ.ɢ(Õ.ø,new ɦ(ā));Ɏ.ć(ɡ);Ɏ.Ʌ(()=>{ɼ.ɷ();var ɰ=ɯ(Ɏ,null,null
,null,null,ʀ.ʁ,ȷ,ȹ,Ƹ,Ȯ);ɰ.Ď=Ď.Ɛ;ɡ.ɴ(ɰ);ɶ(A,ɨ,ɪ,ɰ);ʂ(A,ɾ,ɿ,ɰ);});}void Ɍ(þ Ɏ,ī ĭ,W ė,ƈ Ɖ,ȶ ȷ,ȸ ȹ,Ⱥ Ƹ,ȭ Ȯ,Ą ɉ){ɉ.ć(new Ļ())
;ɉ.ć(new ŀ());ɉ.ć(new ł());ɉ.ć(new ń());var ɨ=new ɧ();var ɪ=new ɩ();var ɡ=new ɠ(ė,ȷ);ɡ.ɢ(Õ.ø,new ɦ(ā));Ɏ.ć(ɡ);Ɏ.Ʉ(()=>{
var ɰ=ɯ(Ɏ,null,null,null,null,ʀ.ʁ,ȷ,ȹ,Ƹ,Ȯ);ɰ.Ď=Ď.ĕ;ɡ.ɴ(ɰ);if(Ȯ.ɵ)ɶ(A,ɨ,ɪ,ɰ);});var ʃ=ǽ.Ĥ(ĭ,ā);if(!ʃ.ǀ){ā.Č("Boot",
$"{ʃ.ǁ} -- set HomeDock in CustomData then run SETUP");return;}var ʄ=new ļ(A,ā,ė,Ɖ,ʃ.ǂ);Ɏ.ć(ʄ);Ɏ.Ʉ(()=>{ʄ.ɮ();});}static ʅ ɯ(þ Ɏ,ŏ Ő,ɒ ɓ,ņ Ň,ɜ ɝ,ʀ ʆ,ȶ ȷ,ȸ ȹ,Ⱥ Ƹ,ȭ ʇ){var ɰ=
new ʅ();ɰ.ó=Ő;ɰ.ʈ=ɓ;ɰ.ʉ=Ň;ɰ.ʊ=ɝ;ɰ.ʀ=ʆ;ɰ.ʋ=ȷ;ɰ.ʌ=ȹ;ɰ.ʍ=Ƹ;ɰ.Ď=Ɏ.Ď;ɰ.ʎ=ʇ;ɰ.ʏ=Ɏ.đ<W>();return ɰ;}static void ɶ(Program V,ɧ ʐ,ɩ
ʑ,ʅ ɰ){if(V.Me.SurfaceCount>0)ʑ.ʒ(ʐ,V.Me.GetSurface(0),ɰ);}static void ʂ(Program V,ɽ ʐ,ɩ ʑ,ʅ ɰ){if(V.Me.SurfaceCount>0)ʑ.
ʒ(ʐ,V.Me.GetSurface(0),ɰ);}}class Ɂ:Ʃ{readonly Program A;readonly Ā ā;readonly þ ʓ;readonly string ʔ;public Ɂ(Program V,Ā
ă,þ Ɏ,string ʕ){A=V;ā=ă;ʓ=Ɏ;ʔ=ʕ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ.Count<1)return;string ʖ=Ƶ.ǐ[0];if(ʖ.Length!=8)return;
for(int É=0;É<ʖ.Length;É++)if(ʖ[É]<'0'||ʖ[É]>'9')return;if(ʔ.Length==0){A.Me.CustomData=ī.Ķ(A.Me.CustomData,"NetworkId",ʖ);
ā.Ē("IGC",$"Adopted NetworkId {ʖ} from hub — rebooting");A.Ĭ();return;}var ʗ=ʓ.đ<ļ>();if(ʗ==null)return;if(Ƶ.ǐ.Count>=6){
int ȼ,Ƚ;double ʘ,ʙ,ʚ;if(int.TryParse(Ƶ.ǐ[1],out ȼ)&&int.TryParse(Ƶ.ǐ[2],out Ƚ)&&double.TryParse(Ƶ.ǐ[3],out ʘ)&&double.
TryParse(Ƶ.ǐ[4],out ʙ)&&double.TryParse(Ƶ.ǐ[5],out ʚ)){ʗ.ľ.ʛ(ȼ,Ƚ,new Vector3D(ʘ,ʙ,ʚ));return;}}if(Ƶ.ǐ.Count>=2){int ȼ;if(int.
TryParse(Ƶ.ǐ[1],out ȼ))ʗ.ľ.ʜ(ȼ);}}}enum Ď{ď,Ņ,Ɛ,ĕ}class ȯ{public Ď ȱ(W ė,ī ĭ,bool ʝ){Ď ʟ=ʞ(ĭ);if(ʟ!=Ď.ď)return ʟ;if(ė.i)return Ď
.Ɛ;if(ė.j&&!ʝ)return Ď.ĕ;return Ď.Ņ;}public Ď ʞ(ī ĭ){string ȓ=ĭ.Į(ę.į,"Mode","");if(ȓ.Equals("Hub",O.Á))return Ď.Ņ;if(ȓ.
Equals("Station",O.Á))return Ď.Ɛ;if(ȓ.Equals("Drone",O.Á))return Ď.ĕ;return Ď.ď;}}class þ{readonly Dictionary<Type,object>ʠ=
new Dictionary<Type,object>();readonly List<Action>ʡ=new List<Action>(),ʢ=new List<Action>(),ʣ=new List<Action>(),ʤ=new
List<Action>();readonly ȭ ʥ;public Ď Ď{get;set;}public þ(ȭ Ȯ){ʥ=Ȯ;}public void ć<ʦ>(ʦ ʧ)where ʦ:class{ʠ[typeof(ʦ)]=ʧ;}public
ʦ đ<ʦ>()where ʦ:class{object ʨ;if(ʠ.TryGetValue(typeof(ʦ),out ʨ))return(ʦ)ʨ;return null;}public void Ʉ(Action ʩ){ʡ.Add(ʩ)
;}public void Ʌ(Action ʩ){ʢ.Add(ʩ);}public void Ⱦ(Action ʩ){ʣ.Add(ʩ);}public void ɹ(Action ʩ){ʤ.Add(ʩ);}public void ʯ(
UpdateType ʪ){ʥ.ʫ(ʪ);if(ʥ.ʬ)for(int É=0;É<ʡ.Count;É++)ʡ[É]();if(ʥ.ɵ)for(int É=0;É<ʢ.Count;É++)ʢ[É]();if(ʥ.ʭ)for(int É=0;É<ʣ.Count;
É++)ʣ[É]();if(ʥ.ʮ)for(int É=0;É<ʤ.Count;É++)ʤ[É]();}static string ʰ(string ǿ){return ǿ==null?"":ǿ.Replace('|','/');}
public void ˆ(ʱ ʲ){var ŋ=đ<Ŋ>();if(ŋ!=null)ʲ.ʳ('H',Đ.đ(ŋ.Ɔ));var ŧ=O.ȝ;var Ő=đ<ŏ>();if(Ő!=null){foreach(var Ȃ in Ő.ʴ){var ʵ=Ȃ.
Value;ʲ.ʳ('D',$"{ʵ.ģ}|{ʰ(ʵ.ʶ)}|{Đ.đ(ʵ.Ɔ)}|{ʰ(ʵ.ʷ)}|{ʵ.ʸ}|{ʵ.ʹ}|{ʵ.ʺ.ToString(ŧ)}|{ʵ.ʻ}|{ʵ.ʼ}|{ʵ.ʽ}");}}var ɓ=đ<ɒ>();if(ɓ!=
null){foreach(var Ȃ in ɓ.ö){var ǿ=Ȃ.Value;ʲ.ʳ('T',$"{ʰ(ǿ.Ć)}|{(ǿ.ʾ?1:0)}|{ǿ.ʿ.ToString(ŧ)}|{ǿ.ˀ}|{(ǿ.ˁ?1:0)}");}}}}class ȭ{
long ˇ;bool ˈ;public long ʯ{get{return ˇ;}}public bool ʬ{get{return ˈ;}}public bool ɵ{get{return ˈ&&ˇ%ę.ȟ==0;}}public bool ʭ
{get{return ˈ&&ˇ%ę.Ƞ==0;}}public bool ʮ{get{return ˈ&&ˇ%ę.ȡ==0;}}public void ʫ(UpdateType ʪ){ˈ=(ʪ&UpdateType.Update10)!=0
;if(ˈ)ˇ++;}}class ɠ{readonly W ˉ;readonly ȶ ˊ;readonly Dictionary<string,ˋ>ˌ=new Dictionary<string,ˋ>();readonly
Dictionary<long,int>ˍ=new Dictionary<long,int>();public ɠ(W ė,ȶ ȷ){ˉ=ė;ˊ=ȷ;}public void ɢ(string l,ˋ ʐ){ˌ[l]=ʐ;}public void ɴ(ʅ ɰ)
{foreach(var Ȃ in ˌ){string l=Ȃ.Key;ˋ ʐ=Ȃ.Value;var ˎ=ˉ.o(l);if(ˎ.Count==0)continue;int ǫ=ʐ.ˏ(ɰ);for(int É=0;É<ˎ.Count;É
++){var u=ˎ[É];var Ø=u as IMyTerminalBlock;long ː=Ø!=null?Ø.EntityId:0;int ˑ;bool ˠ=ˉ.t(ː,out ˑ);bool ˡ=false;for(int ǿ=0;
ǿ<u.SurfaceCount;ǿ++){if(ˠ&&ǿ!=ˑ)continue;long ǰ=unchecked(ː*100+ǿ);int ˢ;if(ˍ.TryGetValue(ǰ,out ˢ)&&ˢ==ǫ)continue;if(!ˡ
&&Ø!=null){ɰ.ˣ=ˤ.ƿ(Ø.CustomData);ˡ=true;}ʐ.ˬ(u.GetSurface(ǿ),ɰ);ˍ[ǰ]=ǫ;}ɰ.ˣ=null;}}}}class ɩ{int ˮ;bool Ͱ;public void ʒ(ˋ
ʐ,IMyTextSurface ͱ,ʅ ɰ){int ǫ=ʐ.ˏ(ɰ);if(Ͱ&&ǫ==ˮ)return;Ͱ=true;ˮ=ǫ;ʐ.ˬ(ͱ,ɰ);}}interface ˋ{string Ͳ{get;}int ˏ(ʅ ɰ);void ˬ(
IMyTextSurface ͱ,ʅ ɰ);}static class ʹ{public static string ͳ(string ǿ,int ǵ){if(string.IsNullOrEmpty(ǿ)||ǿ.Length<=ǵ)return ǿ;return ǿ
.Substring(0,ǵ-2)+"..";}}class ˤ{HashSet<string>Ͷ;public bool ͷ(string ú){return Ͷ==null||Ͷ.Contains(ú);}public static ˤ
ƿ(string ǹ){var ͺ=new ˤ();if(string.IsNullOrEmpty(ǹ))return ͺ;string[]ǭ=ǹ.Split('\n');bool ͻ=false;for(int É=0;É<ǭ.Length
;É++){string Ǯ=ǭ[É].TrimEnd('\r').Trim();bool ͼ=Ǯ.StartsWith("[CCN:",O.Á)||Ǯ.Equals("[CCN]",O.Á);if(ͼ){ͻ=true;continue;}
if(Ǯ.StartsWith("[")){ͻ=false;continue;}if(!ͻ)continue;int ǯ=Ǯ.IndexOf('=');if(ǯ<=0)continue;string ǰ=Ǯ.Substring(0,ǯ).
Trim();string Ä=Ǯ.Substring(ǯ+1).Trim();if(ǰ.Equals("Resources",O.Á)){ͺ.Ͷ=new HashSet<string>(O.P);string[]ͽ=Ä.Split(',');
for(int ũ=0;ũ<ͽ.Length;ũ++){string Ș=ͽ[ũ].Trim();if(Ș.Length>0)ͺ.Ͷ.Add(Ș);}}}return ͺ;}}class ȸ{public readonly Color Ά=new
Color(2,4,10),Έ=new Color(6,12,22),Ή=new Color(20,50,80),Ί=new Color(0,180,255),Ό=new Color(0,100,160),Ύ=new Color(140,180,
200),Ώ=new Color(40,60,80),ΐ=new Color(200,230,255),Α=new Color(0,230,90),Β=new Color(255,190,0),Γ=new Color(255,35,35),Δ=
new Color(12,20,30),Ε=new Color(0,130,255);}struct Κ:IDisposable{static bool Ζ;MySpriteDrawFrame Η;bool Θ;public Κ(
IMyTextSurface ͱ){Η=ͱ.DrawFrame();Θ=true;Ζ=!Ζ;if(Ζ){var Ι=MySprite.CreateSprite("SquareSimple",Vector2.Zero,Vector2.Zero);Ι.Color=new
Color(0,0,0,0);Η.Add(Ι);}}public void Π(string Λ,Vector2 ð,float Μ,Color Ν,TextAlignment Ξ){var Ο=MySprite.CreateText(Λ,
"Monospace",Ν,Μ,Ξ);Ο.Position=ð;Η.Add(Ο);}public void Σ(Vector2 ð,Vector2 Ρ,Color Ν){var Ο=MySprite.CreateSprite("SquareSimple",ð+Ρ
/2f,Ρ);Ο.Color=Ν;Η.Add(Ο);}public void Dispose(){if(Θ){Η.Dispose();Θ=false;}}}struct ʅ{public ŏ ó;public ɒ ʈ;public ņ ʉ;
public ɜ ʊ;public ʀ ʀ;public ˤ ˣ;public ȶ ʋ;public ȸ ʌ;public Ⱥ ʍ;public Ď Ď;public ȭ ʎ;public bool ɱ,ɲ;public ɫ ɳ;public W ʏ;
}class ɣ:ˋ{public string Ͳ{get{return Õ.ò;}}const float Τ=0.9f,Υ=0.7f,Φ=0.55f,Χ=14f,Ψ=24f,Ω=10f;public int ˏ(ʅ ɰ){var Ę=ɰ
.ʋ.Ϊ();long Ϋ=ɰ.ʎ!=null?ɰ.ʎ.ʯ:0;Ę.Append(Đ.đ(ɰ.ʀ));if(ɰ.ʈ!=null){foreach(var Ȃ in ɰ.ʈ.ö){var ǿ=Ȃ.Value;Ę.Append(ǿ.Ć);Ę.
Append((int)ǿ.ʿ);long ά=Ϋ-ǿ.ˀ;Ę.Append(ǿ.ˁ?'D':(ά>180||!ǿ.έ)?'O':ǿ.ʾ?'A':'C');}}if(ɰ.ó!=null){foreach(var Ȃ in ɰ.ó.ʴ){Ę.Append
(Ȃ.Value.ʶ);Ę.Append((int)Ȃ.Value.Ɔ);Ę.Append(Ȃ.Value.ʷ);Ę.Append(Ȃ.Value.ή);}}if(ɰ.ʉ!=null&&ɰ.ʉ.ί){Ę.Append((int)ɰ.ʉ.ΰ);
Ę.Append(ɰ.ʉ.α);}if(ɰ.ʊ!=null&&ɰ.ʊ.β){Ę.Append(ɰ.ʊ.γ);Ę.Append((int)ɰ.ʊ.δ.TotalSeconds);}if(ɰ.ɱ)Ę.Append("FULL");int ǫ=ε.
ζ(Ę);ɰ.ʋ.η(Ę);return ǫ;}public void ˬ(IMyTextSurface ͱ,ʅ ɰ){var V=ɰ.ʌ;var θ=ͱ.SurfaceSize;var ι=ͱ.TextureSize;float κ=(ι.
X-θ.X)/2f;float λ=(ι.Y-θ.Y)/2f;float μ=θ.X;float ν=λ+θ.Y-40f;ͱ.ContentType=ContentType.SCRIPT;ͱ.Script="";ͱ.
ScriptBackgroundColor=V.Ά;using(var ξ=new Κ(ͱ)){ξ.Σ(new Vector2(κ,λ),ι,V.Ά);float ο=λ+8f;ξ.Π("CARGO COLLECTION NETWORK",new Vector2(κ+μ/2f,ο)
,Τ,V.Ί,TextAlignment.CENTER);ο+=28f;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),V.Ε);ο+=8f;int π=ɰ.ʈ!=null?ɰ.ʈ.ö.Count:0;
int ς=ɰ.ó!=null?ɰ.ó.ρ:0;string σ=$"CCN {ę.Ě}  |  {Đ.đ(ɰ.ʀ)}  |  {π} Stations  |  {ς} Drones";ξ.Π(σ,new Vector2(κ+μ/2f,ο),Φ,
V.Ό,TextAlignment.CENTER);ο+=22f;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),V.Ή);ο+=6f;if(ɰ.ʈ!=null){ξ.Π("STATIONS",new
Vector2(κ+Ω,ο),Υ,V.Ό,TextAlignment.LEFT);ο+=22f;long Ϋ=ɰ.ʎ!=null?ɰ.ʎ.ʯ:0;int τ=0;int υ=ɰ.ʈ.ö.Count;foreach(var Ȃ in ɰ.ʈ.ö){if(ο
>ν){ξ.Π($"+{υ-τ} more...",new Vector2(κ+Ω+4,ο),Φ,V.Ώ,TextAlignment.LEFT);ο+=Ψ;break;}τ++;var ǿ=Ȃ.Value;long ά=Ϋ-ǿ.ˀ;Color
φ;string χ;if(ǿ.ˁ){χ="DISABLED";φ=V.Ώ;}else if(ά>180||!ǿ.έ){χ="OFFLINE";φ=V.Ώ;}else if(!ǿ.ʾ){χ="CLOSED";φ=V.Β;}else{χ=
"ACTIVE";φ=V.Α;}ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,Ψ-2),V.Έ);ξ.Π(ʹ.ͳ(ǿ.Ć,14),new Vector2(κ+Ω+4,ο+2),Φ,V.Ύ,TextAlignment.
LEFT);float ψ=κ+μ*0.48f;float ω=μ*0.3f;float ϊ=ǿ.ʿ/100f;Color ϋ=ϊ<=0.7f?V.Α:ϊ<=0.9f?V.Β:V.Γ;ξ.Σ(new Vector2(ψ,ο+4),new
Vector2(ω,Χ),V.Δ);if(ϊ>0)ξ.Σ(new Vector2(ψ,ο+4),new Vector2(ω*(ϊ>1f?1f:ϊ),Χ),ϋ);ξ.Π($"{ǿ.ʿ:F0}%",new Vector2(ψ+ω+4,ο+2),Φ,V.ΐ,
TextAlignment.LEFT);ξ.Π(χ,new Vector2(κ+μ-Ω-4,ο+2),Φ,φ,TextAlignment.RIGHT);ο+=Ψ;}ο+=4f;}ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),
V.Ή);ο+=6f;if(ɰ.ó!=null){ξ.Π($"FLEET ({ɰ.ó.ρ} drones)",new Vector2(κ+Ω,ο),Υ,V.Ό,TextAlignment.LEFT);ο+=22f;int ό=0;int ύ=
ɰ.ó.ρ;foreach(var Ȃ in ɰ.ó.ʴ){if(ο>ν){ξ.Π($"+{ύ-ό} more...",new Vector2(κ+Ω+4,ο),Φ,V.Ώ,TextAlignment.LEFT);ο+=Ψ;break;}ό
++;var ʵ=Ȃ.Value;Color ώ;switch(ʵ.Ɔ){case Ϗ.ϐ:ώ=V.Α;break;case Ϗ.ϑ:case Ϗ.ϒ:ώ=V.Ε;break;case Ϗ.ϓ:ώ=V.Β;break;case Ϗ.ϔ:ώ=V.
Ό;break;case Ϗ.ϕ:ώ=V.Γ;break;case Ϗ.ϖ:ώ=V.Ώ;break;default:ώ=V.Ύ;break;}string ϗ=ʵ.ή>0?$"Queue #{ʵ.ή}":Đ.đ(ʵ.Ɔ);bool Ϙ=(ο/
Ψ)%2<1;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,Ψ-2),Ϙ?V.Έ:V.Ά);ξ.Π(ʹ.ͳ(ʵ.ʶ,13),new Vector2(κ+Ω+4,ο+2),Φ,V.Ύ,
TextAlignment.LEFT);ξ.Π(ϗ,new Vector2(κ+μ*0.42f,ο+2),Φ,ώ,TextAlignment.LEFT);if(!string.IsNullOrEmpty(ʵ.ʷ))ξ.Π(ʹ.ͳ(ʵ.ʷ,12),new
Vector2(κ+μ-Ω-4,ο+2),Φ,V.Ύ,TextAlignment.RIGHT);ο+=Ψ;}ο+=4f;}ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),V.Ή);ο+=6f;if(ɰ.ʊ!=
null&&ɰ.ʊ.β)ξ.Π($"Schedule: {ɰ.ʊ.γ} ({ɰ.ʊ.δ:mm\\:ss})",new Vector2(κ+Ω,ο),Φ,V.Ό,TextAlignment.LEFT);if(ɰ.ɱ){ξ.Σ(new Vector2(
κ+Ω,ο+14),new Vector2(μ-Ω*2,18),V.Γ);ξ.Π("HUB STORAGE FULL — DRONES HALTED",new Vector2(κ+μ/2f,ο+15),Φ,V.ΐ,TextAlignment.
CENTER);ο+=20;}if(ɰ.ʉ!=null&&ɰ.ʉ.ί)ξ.Π($"ALERT: {Đ.đ(ɰ.ʉ.ΰ)} - {ɰ.ʉ.α}",new Vector2(κ+Ω,ο+16),Φ,V.Γ,TextAlignment.LEFT);}}}
class ɦ:ˋ{public string Ͳ{get{return Õ.ø;}}readonly Ā ā;public ɦ(Ā ă){ā=ă;}void ϝ(StringBuilder Ę,ʅ ɰ){Ę.AppendLine(
$"  CCN {ę.Ě} DEBUG LOG");Ę.AppendLine($"  Mode: {Đ.đ(ɰ.Ď)} | Up: {(ɰ.ʎ!=null?ɰ.ʎ.ʯ/6:0)}s");if(ɰ.ʍ!=null)Ę.AppendLine(
$"  avg:{ɰ.ʍ.ϙ:F2}ms p99:{ɰ.ʍ.Ϛ:F2}ms instr:{ɰ.ʍ.ϛ:F1}%");Ę.AppendLine(new string('-',44));ā.Ϝ(Ę);}public int ˏ(ʅ ɰ){var Ę=ɰ.ʋ.Ϊ();ϝ(Ę,ɰ);int ǫ=ε.ζ(Ę);ɰ.ʋ.η(Ę);return ǫ;}public
void ˬ(IMyTextSurface ͱ,ʅ ɰ){var Ę=ɰ.ʋ.Ϊ();var V=ɰ.ʌ;ϝ(Ę,ɰ);ͱ.ContentType=ContentType.TEXT_AND_IMAGE;ͱ.FontSize=ͱ.FontSize>
0.1f?ͱ.FontSize:0.45f;ͱ.Font="Monospace";ͱ.FontColor=V.Α;ͱ.BackgroundColor=V.Ά;ͱ.WriteText(Ę);ɰ.ʋ.η(Ę);}}class ɤ:ˋ{public
string Ͳ{get{return Õ.ó;}}const float Τ=0.9f,Υ=0.6f,Φ=0.55f,Ψ=20f,Ω=10f;public int ˏ(ʅ ɰ){var Ę=ɰ.ʋ.Ϊ();if(ɰ.ó!=null){foreach(
var Ȃ in ɰ.ó.ʴ){var Ϟ=Ȃ.Value;Ę.Append(Ϟ.ʶ);Ę.Append((int)Ϟ.Ɔ);Ę.Append(Ϟ.ʷ);Ę.Append((int)Ϟ.ϟ);Ę.Append((int)Ϟ.Ϡ);Ę.Append
((int)Ϟ.ϡ);Ę.Append(Ϟ.Ϣ?'1':'0');Ę.Append(Ϟ.ʹ);Ę.Append(Ϟ.ʸ);Ę.Append(Ϟ.ϣ?'h':Ϟ.Ϥ?'y':'-');}}Ę.Append(ɰ.ɱ?'1':'0');Ę.
Append(ɰ.ɲ?'1':'0');int ǫ=ε.ζ(Ę);ɰ.ʋ.η(Ę);return ǫ;}public void ˬ(IMyTextSurface ͱ,ʅ ɰ){var θ=ͱ.SurfaceSize;var ι=ͱ.
TextureSize;float κ=(ι.X-θ.X)/2f;float λ=(ι.Y-θ.Y)/2f;float μ=θ.X;float ν=λ+θ.Y-30f;var V=ɰ.ʌ;ͱ.ContentType=ContentType.SCRIPT;ͱ.
Script="";ͱ.ScriptBackgroundColor=V.Ά;using(var ξ=new Κ(ͱ)){ξ.Σ(new Vector2(κ,λ),ι,V.Ά);float ο=λ+8f;ξ.Π("FLEET MANAGEMENT",
new Vector2(κ+μ/2f,ο),Τ,V.Ί,TextAlignment.CENTER);ο+=28f;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),V.Ε);ο+=6f;if(ɰ.ó!=
null){int ı=0;int ϥ=0;int Ȁ=0;int Ϧ=0;foreach(var Ȃ in ɰ.ó.ʴ){switch(Ȃ.Value.Ɔ){case Ϗ.ϐ:ı++;break;case Ϗ.ϒ:case Ϗ.ϑ:case Ϗ.
ϧ:case Ϗ.ϔ:case Ϗ.ϓ:ϥ++;break;case Ϗ.ϖ:case Ϗ.Ϩ:Ȁ++;break;case Ϗ.ϕ:Ϧ++;break;}}ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,
18),V.Έ);string ϩ=$"Total:{ɰ.ó.ρ}  Active:{ϥ}  Hub:{ı}  Home:{Ȁ}  Lost:{Ϧ}";ξ.Π(ϩ,new Vector2(κ+μ/2f,ο+1),Φ,V.Ύ,
TextAlignment.CENTER);ο+=22f;float Ϫ=κ+Ω+4;float ϫ=κ+μ*0.22f;float Ϭ=κ+μ*0.38f;float ϭ=κ+μ*0.60f;float Ϯ=κ+μ*0.74f;float ϯ=κ+μ-Ω-4;ξ.
Π("Drone",new Vector2(Ϫ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Cmd",new Vector2(ϫ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Activity",
new Vector2(Ϭ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Target",new Vector2(ϭ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Crg",new Vector2((Ϯ
+ϯ)/2f,ο),Υ,V.Ό,TextAlignment.CENTER);ξ.Π("Power",new Vector2(ϯ,ο),Υ,V.Ό,TextAlignment.RIGHT);ο+=18f;ξ.Σ(new Vector2(κ+Ω,
ο),new Vector2(μ-Ω*2,1),V.Ή);ο+=3f;int ϰ=0;foreach(var Ȃ in ɰ.ó.ʴ){if(ο>ν){ξ.Π($"+{ɰ.ó.ρ-ϰ} more...",new Vector2(Ϫ,ο),Φ,V
.Ώ,TextAlignment.LEFT);ο+=Ψ;break;}var ʵ=Ȃ.Value;string ϱ=ʵ.ʸ??"READY";if(ϱ=="IDLE")ϱ="READY";Color ϲ;switch(ϱ){case
"RUNNING":ϲ=V.Α;break;case"READY":ϲ=V.Α;break;case"PAUSED":ϲ=V.Β;break;case"RECALLED":ϲ=V.Ό;break;case"HOME":ϲ=V.Ώ;break;case
"LOST":ϲ=V.Γ;break;default:ϲ=V.Ώ;break;}string ϳ;Color ϴ;switch(ʵ.Ɔ){case Ϗ.ϐ:if(ɰ.ɱ){ϳ="Wait:Storage";ϴ=V.Β;}else if(ʵ.ϟ>5){ϳ
="Unloading";ϴ=V.Β;}else if(ʵ.ϵ&&ʵ.Ϡ<98f){ϳ=$"Chrg B:{ʵ.Ϡ:F0}%";ϴ=ʵ.Ϡ<20?V.Γ:V.Β;}else if(ʵ.Ϸ&&ʵ.ϡ<98f){ϳ=
$"Chrg H:{ʵ.ϡ:F0}%";ϴ=ʵ.ϡ<20?V.Γ:V.Β;}else if(ɰ.ɲ){ϳ="Wait:Full";ϴ=V.Β;}else{ϳ="Ready";ϴ=V.Α;}break;case Ϗ.ϒ:case Ϗ.ϑ:ϳ=ʵ.Ϥ?"Yield:Sep":
"Flying Out";ϴ=V.Ε;break;case Ϗ.ϧ:ϳ="Loading";ϴ=V.ΐ;break;case Ϗ.ϔ:ϳ=ʵ.ή>0?$"Q#{ʵ.ή}":"Wait:Ore";ϴ=ʵ.ή>0?V.Ό:V.ΐ;break;case Ϗ.ϓ:ϳ=ʵ.
ϣ?"Hold:Ring":ʵ.Ϥ?"Yield:Sep":"Returning";ϴ=V.Β;break;case Ϗ.Ϩ:ϳ="Parked";ϴ=V.Ώ;break;case Ϗ.ϖ:ϳ="Docked";ϴ=V.Ώ;break;
case Ϗ.ϕ:ϳ="No Signal";ϴ=V.Γ;break;default:ϳ="-";ϴ=V.Ώ;break;}Color ϸ=ϰ%2==0?V.Έ:V.Ά;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*
2,Ψ-2),ϸ);ξ.Π(ʹ.ͳ(ʵ.ʶ,10),new Vector2(Ϫ,ο+1),Φ,V.Ύ,TextAlignment.LEFT);ξ.Π(ϱ,new Vector2(ϫ,ο+1),Φ,ϲ,TextAlignment.LEFT);ξ
.Π(ϳ,new Vector2(Ϭ,ο+1),Φ,ϴ,TextAlignment.LEFT);if(!string.IsNullOrEmpty(ʵ.ʷ))ξ.Π(ʹ.ͳ(ʵ.ʷ,9),new Vector2(ϭ,ο+1),Φ,V.Ύ,
TextAlignment.LEFT);ξ.Π($"{ʵ.ϟ:F0}%",new Vector2((Ϯ+ϯ)/2f,ο+1),Φ,V.ΐ,TextAlignment.CENTER);string Ϲ="";Color Ϻ=V.Α;if(ʵ.Ϣ){Ϲ="CHRG";Ϻ
=V.Β;}else if(ʵ.ϵ&&ʵ.Ϸ){Ϲ=$"B:{ʵ.Ϡ:F0} H:{ʵ.ϡ:F0}";Ϻ=(ʵ.Ϡ<20||ʵ.ϡ<20)?V.Γ:(ʵ.Ϡ<50||ʵ.ϡ<50)?V.Β:V.Α;}else if(ʵ.ϵ){Ϲ=
$"B:{ʵ.Ϡ:F0}%";Ϻ=ʵ.Ϡ<20?V.Γ:ʵ.Ϡ<50?V.Β:V.Α;}else if(ʵ.Ϸ){Ϲ=$"H2:{ʵ.ϡ:F0}%";Ϻ=ʵ.ϡ<20?V.Γ:ʵ.ϡ<50?V.Β:V.Α;}if(Ϲ.Length>0)ξ.Π(Ϲ,new
Vector2(ϯ,ο+1),Φ,Ϻ,TextAlignment.RIGHT);ο+=Ψ;ϰ++;}if(ο>ν-60f)return;ο+=6f;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),V.Ή);ο+=
6f;ξ.Π("TRIP STATISTICS",new Vector2(κ+μ/2f,ο),Υ,V.Ό,TextAlignment.CENTER);ο+=18f;float ϻ=κ+Ω+4;float ϼ=κ+μ*0.28f;float Ͻ=
κ+μ*0.42f;float Ͼ=κ+μ*0.58f;float Ͽ=κ+μ*0.76f;ξ.Π("Drone",new Vector2(ϻ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Trips",new
Vector2(ϼ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Last",new Vector2(Ͻ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Best",new Vector2(Ͼ,ο),Υ,V.Ό,
TextAlignment.LEFT);ξ.Π("Delivered",new Vector2(Ͽ,ο),Υ,V.Ό,TextAlignment.LEFT);ο+=16f;ϰ=0;foreach(var Ȃ in ɰ.ó.ʴ){if(ο>ν){ξ.Π(
$"+{ɰ.ó.ρ-ϰ} more...",new Vector2(ϻ,ο),Φ,V.Ώ,TextAlignment.LEFT);break;}var ʵ=Ȃ.Value;Color Ѐ=ϰ%2==0?V.Έ:V.Ά;ξ.Σ(new Vector2(κ+Ω,ο),new
Vector2(μ-Ω*2,Ψ-2),Ѐ);ξ.Π(ʹ.ͳ(ʵ.ʶ,11),new Vector2(ϻ,ο+1),Φ,V.Ύ,TextAlignment.LEFT);ξ.Π(ʵ.ʹ.ToString(),new Vector2(ϼ,ο+1),Φ,V.ΐ,
TextAlignment.LEFT);ξ.Π(Ё(ʵ.Ђ),new Vector2(Ͻ,ο+1),Φ,V.Ύ,TextAlignment.LEFT);ξ.Π(Ё(ʵ.ʻ),new Vector2(Ͼ,ο+1),Φ,V.Α,TextAlignment.LEFT);ξ
.Π(Ѓ(ʵ.ʺ),new Vector2(Ͽ,ο+1),Φ,V.ΐ,TextAlignment.LEFT);ο+=Ψ;ϰ++;}}}}static string Ё(long Є){if(Є<=0)return"-";int Ѕ=(int)
(Є/6);if(Ѕ<60)return$"{Ѕ}s";return$"{Ѕ/60}m{Ѕ%60:D2}s";}static string Ѓ(float І){if(І<=0)return"-";if(І>=1000000f)return
$"{І/1000000f:F1}Mt";if(І>=1000f)return$"{І/1000f:F0}kt";return$"{І:F0}kg";}}class ɧ:ˋ{public string Ͳ{get{return"";}}void ϝ(StringBuilder Ę
,ʅ ɰ){string Ї=Đ.đ(ɰ.Ď);if(ɰ.Ď==Ď.Ņ)Ї+=" | "+Đ.đ(ɰ.ʀ);Ę.AppendLine($"CCN {ę.Ě} [{Ї}]");if(ɰ.ó!=null)Ę.AppendLine(
$"Drones:{ɰ.ó.ρ} Stations:{(ɰ.ʈ!=null?ɰ.ʈ.ö.Count:0)}");else Ę.AppendLine("...");if(ɰ.ʍ!=null&&(Ј.Ǧ||ɰ.Ď==Ď.Ņ))Ę.AppendLine($"avg:{ɰ.ʍ.ϙ:F2}ms p99:{ɰ.ʍ.Ϛ:F2}ms");else if(ɰ.ʉ
!=null&&ɰ.ʉ.ί)Ę.AppendLine($"ALERT: {Đ.đ(ɰ.ʉ.ΰ)}");else Ę.AppendLine("OK");}public int ˏ(ʅ ɰ){var Ę=ɰ.ʋ.Ϊ();ϝ(Ę,ɰ);int ǫ=ε
.ζ(Ę);ɰ.ʋ.η(Ę);return ǫ;}public void ˬ(IMyTextSurface ͱ,ʅ ɰ){var Ę=ɰ.ʋ.Ϊ();ϝ(Ę,ɰ);ͱ.ContentType=ContentType.
TEXT_AND_IMAGE;ͱ.FontSize=0.8f;ͱ.Font="Monospace";ͱ.WriteText(Ę);ɰ.ʋ.η(Ę);}}class ɽ:ˋ{public string Ͳ{get{return Õ.ô;}}readonly ɻ Љ;
public ɽ(ɻ Њ){Љ=Њ;}void ϝ(StringBuilder Ę,ʅ ɰ){if(!Љ.β){Ę.AppendLine();Ę.AppendLine();Ę.AppendLine(
"  =============================");Ę.AppendLine("       STATION DISABLED");Ę.AppendLine("  =============================");Ę.AppendLine();Ę.AppendLine(
$"  {Љ.Ǥ}");Ę.AppendLine();Ę.AppendLine("  Not broadcasting to hub.");Ę.AppendLine("  Set Enabled=true in CustomData");Ę.
AppendLine("  then run RECONFIG to resume.");return;}var Ќ=Љ.Ћ;float ϊ=Ќ.ʿ;Ę.AppendLine($"  {Љ.Ǥ}");Ę.AppendLine(new string('-',40
));int Ѝ=Љ.ʏ.a().Count;int Ў=Љ.ʏ.b().Count;Ę.AppendLine($"  Drills: {Ѝ}    Containers: {Ў}");Ę.AppendLine();Ę.Append(
"  Fill: ");int Џ=20;int А=(int)(ϊ/100f*Џ);if(А>Џ)А=Џ;Ę.Append('[');for(int É=0;É<Џ;É++)Ę.Append(É<А?'#':'.');Ę.AppendLine(
$"] {ϊ:F1}%");Ę.AppendLine();Ę.AppendLine("  ORE INVENTORY");Ę.AppendLine($"  {"Ore",-16} {"Amount",12}");Ę.AppendLine(
$"  {new string('-',30)}");bool Б=false;foreach(var Ȃ in Ќ.В){if(ɰ.ˣ!=null&&!ɰ.ˣ.ͷ(Ȃ.Key))continue;Б=true;string ç=Ū.ʶ(Ȃ.Key);string Ɵ=Г(Ȃ.Value)
;Ę.AppendLine($"  {ç,-16} {Ɵ,12}");}if(!Б)Ę.AppendLine("  (no ore detected)");Ę.AppendLine();long Ϋ=ɰ.ʎ!=null?ɰ.ʎ.ʯ:0;Ę.
AppendLine($"  Up: {Ϋ/6}s");}public int ˏ(ʅ ɰ){var Ę=ɰ.ʋ.Ϊ();ϝ(Ę,ɰ);int ǫ=ε.ζ(Ę);ɰ.ʋ.η(Ę);return ǫ;}public void ˬ(IMyTextSurface ͱ
,ʅ ɰ){var Ę=ɰ.ʋ.Ϊ();var V=ɰ.ʌ;ϝ(Ę,ɰ);bool Д=Љ.β;ͱ.ContentType=ContentType.TEXT_AND_IMAGE;ͱ.FontSize=Д?0.55f:0.7f;ͱ.Font=
"Monospace";ͱ.FontColor=Д?V.Ύ:V.Γ;ͱ.BackgroundColor=V.Ά;ͱ.WriteText(Ę);ɰ.ʋ.η(Ę);}static string Г(float І){if(І>=1000000f)return
$"{І/1000000f:F2} Mt";if(І>=1000f)return$"{І/1000f:F1} kt";return$"{І:F0} kg";}}class ɥ:ˋ{public string Ͳ{get{return Õ.ô;}}const float Τ=0.9f
,Υ=0.6f,Φ=0.55f,Ψ=22f,Χ=12f,Ω=10f;struct З{public string û;public float Е;public int Ж;public char ǈ;}readonly List<З>И=
new List<З>();readonly HashSet<string>Й=new HashSet<string>();readonly List<MyInventoryItem>К=new List<MyInventoryItem>();
void Р(ʅ ɰ){И.Clear();if(ɰ.ɳ==null||ɰ.ʏ==null)return;var Л=ɰ.ɳ.В;Й.Clear();var М=ɰ.ʏ.n(Õ.õ);for(int É=0;É<М.Count;É++){if(М[
É].Closed)continue;К.Clear();М[É].GetInventory(0).GetItems(К);for(int ũ=0;ũ<К.Count;ũ++)if(Ū.ū(К[ũ].Type))Й.Add(К[ũ].Type
.SubtypeId);}К.Clear();var Н=Ū.ǻ;for(int É=0;É<Н.Length;É++)О(ɰ,Н[É],Л);foreach(var Ȃ in Л){bool П=false;for(int É=0;É<Н.
Length;É++)if(Н[É]==Ȃ.Key){П=true;break;}if(!П)О(ɰ,Ȃ.Key,Л);}}void О(ʅ ɰ,string Ș,IReadOnlyDictionary<string,float>Л){if(ɰ.ˣ!=
null&&!ɰ.ˣ.ͷ(Ș))return;float І;Л.TryGetValue(Ș,out І);var Ȁ=ɰ.ʏ.n(Õ.û(Ș));bool С=Ȁ.Count>0;if(!С)Ȁ=ɰ.ʏ.n(Õ.ö);if(І<=0&&!С)
return;long Т=0,ǵ=0;bool ċ=false;for(int É=0;É<Ȁ.Count;É++){if(Ȁ[É].Closed)continue;var ŧ=Ȁ[É].GetInventory(0);Т+=ŧ.
CurrentVolume.RawValue;ǵ+=ŧ.MaxVolume.RawValue;float У=(float)ŧ.MaxVolume.RawValue;if(У>0&&ŧ.CurrentVolume.RawValue/У<ɰ.ʏ.º(Ȁ[É].
EntityId))ċ=true;}var Ф=new З();Ф.û=Ș;Ф.Е=І;Ф.Ж=ǵ>0?(int)(Т*100/ǵ):-1;if(Й.Contains(Ș))Ф.ǈ='O';else if(Ȁ.Count==0)Ф.ǈ='-';else
if(!ċ)Ф.ǈ='F';else Ф.ǈ='K';И.Add(Ф);}public int ˏ(ʅ ɰ){Р(ɰ);var Ę=ɰ.ʋ.Ϊ();if(ɰ.ɳ!=null)Ę.Append((int)ɰ.ɳ.ʿ);for(int É=0;É<
И.Count;É++){var Ф=И[É];Ę.Append(Ф.û);Ę.Append((int)(Ф.Е/50f));Ę.Append(Ф.Ж);Ę.Append(Ф.ǈ);}int ǫ=ε.ζ(Ę);ɰ.ʋ.η(Ę);return
ǫ;}public void ˬ(IMyTextSurface ͱ,ʅ ɰ){Р(ɰ);var θ=ͱ.SurfaceSize;var ι=ͱ.TextureSize;float κ=(ι.X-θ.X)/2f;float λ=(ι.Y-θ.Y
)/2f;float μ=θ.X;float ν=λ+θ.Y-30f;var V=ɰ.ʌ;ͱ.ContentType=ContentType.SCRIPT;ͱ.Script="";ͱ.ScriptBackgroundColor=V.Ά;
using(var ξ=new Κ(ͱ)){ξ.Σ(new Vector2(κ,λ),ι,V.Ά);float ο=λ+8f;ξ.Π("HUB RESOURCES",new Vector2(κ+μ/2f,ο),Τ,V.Ί,TextAlignment.
CENTER);ο+=28f;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),V.Ε);ο+=6f;if(ɰ.ɳ!=null){ξ.Π($"Hub cargo: {ɰ.ɳ.ʿ:F0}% used",new
Vector2(κ+μ/2f,ο),Φ,V.Ό,TextAlignment.CENTER);ο+=20f;}float Ϫ=κ+Ω+4;float Х=κ+μ*0.30f;float Ц=κ+μ*0.50f;float Ч=μ*0.22f;float Ш
=κ+μ-Ω-4;ξ.Π("Ore",new Vector2(Ϫ,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Stored",new Vector2(Х,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π
("Fill",new Vector2(Ц,ο),Υ,V.Ό,TextAlignment.LEFT);ξ.Π("Status",new Vector2(Ш,ο),Υ,V.Ό,TextAlignment.RIGHT);ο+=18f;ξ.Σ(
new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,1),V.Ή);ο+=3f;if(И.Count==0){ξ.Π("(no ore stored)",new Vector2(κ+μ/2f,ο+8f),Φ,V.Ώ,
TextAlignment.CENTER);return;}int ϰ=0;for(int É=0;É<И.Count;É++){if(ο>ν){ξ.Π($"+{И.Count-É} more...",new Vector2(Ϫ,ο),Φ,V.Ώ,
TextAlignment.LEFT);break;}var Ф=И[É];Color ϸ=ϰ%2==0?V.Έ:V.Ά;ξ.Σ(new Vector2(κ+Ω,ο),new Vector2(μ-Ω*2,Ψ-2),ϸ);ξ.Π(ʹ.ͳ(Ū.ʶ(Ф.û),14),
new Vector2(Ϫ,ο+2),Φ,V.Ύ,TextAlignment.LEFT);ξ.Π(Щ(Ф.Е),new Vector2(Х,ο+2),Φ,V.ΐ,TextAlignment.LEFT);if(Ф.Ж>=0){float Ъ=Ф.Ж
/100f;Color ϋ=Ъ<=0.7f?V.Α:Ъ<=0.9f?V.Β:V.Γ;ξ.Σ(new Vector2(Ц,ο+4),new Vector2(Ч,Χ),V.Δ);if(Ъ>0)ξ.Σ(new Vector2(Ц,ο+4),new
Vector2(Ч*(Ъ>1f?1f:Ъ),Χ),ϋ);ξ.Π($"{Ф.Ж}%",new Vector2(Ц+Ч+4,ο+2),Φ,V.ΐ,TextAlignment.LEFT);}else{ξ.Π("-",new Vector2(Ц,ο+2),Φ,V
.Ώ,TextAlignment.LEFT);}string χ;Color Ы;switch(Ф.ǈ){case'F':χ="FULL";Ы=V.Γ;break;case'O':χ="OVFL";Ы=V.Β;break;case'K':χ=
"OK";Ы=V.Α;break;default:χ="-";Ы=V.Ώ;break;}ξ.Π(χ,new Vector2(Ш,ο+2),Φ,Ы,TextAlignment.RIGHT);ο+=Ψ;ϰ++;}}}static string Щ(
float І){if(І<=0)return"-";if(І>=1000000f)return$"{І/1000000f:F2} Mt";if(І>=1000f)return$"{І/1000f:F1} kt";return$"{І:F0} kg"
;}}class х{readonly Ā ā;readonly Ь Э;readonly Ю Я;readonly а б;readonly в г;readonly W ˉ;readonly ƈ д;readonly string е,ж
;з й=з.и;string к="",л="",м="";int н,о;long ˇ;float п;bool р;public х(Ā ă,Ь с,Ю т,а у,в ф,W ė,ƈ Ɖ,string ĥ,string İ){ā=ă;
Э=с;Я=т;б=у;г=ф;ˉ=ė;д=Ɖ;е=ĥ;ж=İ;}const float ц=15f;public void ɴ(){Я.ɭ();г.ɭ();var Ƣ=ч();bool ш=Ƣ!=null&&Ƣ.Status==
MyShipConnectorStatus.Connected;о++;ˇ++;if(ˇ%3==0)щ();if(ъ()&&й!=з.ы&&й!=з.ʮ&&й!=з.ь&&й!=з.и&&й!=з.ϕ&&й!=з.э){б.ю();Э.я();ā.Č("Drone",
$"LOW POWER — halted (B:{г.Ϡ:F0}% H:{г.ϡ:F0}%)");ѐ(з.ϕ);}switch(й){case з.и:if(ш){if(ё(Ƣ)){ѐ(з.ы);}else{ā.Ē("Drone","Booted at station — heading home");ђ(
"booted_at_station");}}else ѐ(з.ʮ);break;case з.ы:break;case з.ѓ:н++;if(!ш){if(!р){р=true;ā.Ē("Drone","Undocked — flying to station");}}
else if(ш&&р){if(ё(Ƣ)){ā.Č("Drone","Re-docked at home — dispatch aborted");м=ж;ѐ(з.ы);}else{м=Ƣ.OtherConnector?.CustomName??
"";ѐ(з.є);}}else if(н>ę.ȣ){ā.Č("Drone","SAM never undocked — lost");ѐ(з.ϕ);}break;case з.є:б.ѕ();ѐ(з.і);break;case з.і:б.ɴ
();if(о%60==0)ā.Ē("Drone",$"Loading... cargo:{Я.ї:F1}% loader:{(б.ί?"active":"idle")} tick:{о}");if(Я.ј(ę.Ȩ)){ā.Ē("Drone"
,$"Cargo full ({Я.ї:F0}%), heading home");б.ю();ђ("cargo_full");}else if(о>ę.Ȥ){if(Я.ї>п+1f){п=Я.ї;о=0;}else{ā.Ē("Drone",
$"Loading timeout ({Я.ї:F0}%), heading home");б.ю();ђ("loading_timeout");}}break;case з.ϔ:if(љ){if(о%60==0)ā.Ē("Drone",
$"Charging at station... B:{г.Ϡ:F0}% H:{г.ϡ:F0}%");if(њ()){ā.Ē("Drone","Charged — heading home");љ=false;ђ("charged");}}else if(о>ę.ȥ){ā.Ē("Drone",
"Wait timeout, heading home");ђ("wait_timeout");}break;case з.э:н++;if(ћ){ќ++;if(ќ>ѝ)ў("no clearance — timeout");else if(ъ())ў("low power");break;}
if(!ш){if(!р){р=true;ā.Ē("Drone","Undocked — flying home");}}else if(ш&&р){if(ё(Ƣ)){м=ж;ѐ(з.ы);}else{Э.я();ā.Č("Drone",
"Re-locked at station — charging");ѐ(з.ϕ);}}else if(н>ę.ȣ){Э.я();ā.Č("Drone","SAM never undocked — lost");ѐ(з.ϕ);}break;case з.ʮ:if(ш)ѐ(з.ы);else if(о>ę.
ȥ*2)ђ("idle_recovery");break;case з.ь:if(ш&&к.Length>0)к="";break;case з.ϕ:if(Ƣ!=null&&Ƣ.Status==Sandbox.ModAPI.Ingame.
MyShipConnectorStatus.Connectable){Ƣ.Connect();ā.Ē("Drone","Auto-locked connector to charge");}if(ш){if(ё(Ƣ)){ā.Ē("Drone",
"Docked at home — recovered");ѐ(з.ы);break;}bool џ=њ();if(о%60==0&&!џ)ā.Ē("Drone",
$"Charging at '{Ƣ.OtherConnector?.CustomName??"?"}' (home={ж}) B:{г.Ϡ:F0}% H:{г.ϡ:F0}%");if(џ){ā.Ē("Drone","Power restored at station — heading home");ђ("recovered");}}break;}Ѡ++;if(Ѡ>=3||й!=ѡ){Ѡ=0;ѡ=й;Ѣ();}
}int Ѡ;з ѡ=з.и;void ѐ(з ѣ){ā.Ē("Drone",$"{Đ.đ(й)} -> {Đ.đ(ѣ)}");й=ѣ;о=0;Ѥ=0;ѥ=-1f;ћ=false;ќ=0;Ѧ=false;ѧ=0;if(ѣ==з.ы||ѣ==з
.ʮ||ѣ==з.ь)к="";п=Я.ї;}const int Ѩ=360;float ѥ=-1f;int Ѥ;bool њ(){bool ѩ=true;float Ѫ=200f;if(г.Ϡ>=0){if(г.Ϡ<99f)ѩ=false;
if(г.Ϡ<Ѫ)Ѫ=г.Ϡ;}if(г.ϡ>=0){if(г.ϡ<99f)ѩ=false;if(г.ϡ<Ѫ)Ѫ=г.ϡ;}if(ѩ)return true;if(Ѫ>ѥ+0.2f){ѥ=Ѫ;Ѥ=0;return false;}Ѥ++;if(Ѥ
>Ѩ&&Ѫ>=ѫ){ā.Č("Drone",$"Charge stalled at {Ѫ:F0}% — proceeding anyway");Ѥ=0;return true;}return false;}public void Ѯ(
string Ѭ,string ĵ){if(й!=з.ы&&й!=з.ʮ){ā.Č("Drone",$"Cannot dispatch in state {Đ.đ(й)}");return;}к=Ѭ;л=ĵ;н=0;р=false;ā.Ē(
"Drone",$"Dispatch to {Ѭ} (dock: {ĵ})");if(Э.ѭ(ĵ))ѐ(з.ѓ);else{ā.Č("Drone","SAM GoTo failed");ѐ(з.ϕ);}}public void Ѱ(string ѯ){
if(й==з.ь)return;ђ(ѯ);}public void Ŀ(){ђ("manual");if(й==з.э){й=з.ь;к="";}}public void Ł(){if(й!=з.ь&&й!=з.ϕ)return;var Ƣ=
ч();if(Ƣ!=null&&Ƣ.Status==MyShipConnectorStatus.Connected){if(ё(Ƣ)){ѐ(з.ы);return;}ђ("resume");return;}ѐ(з.ʮ);}const int
ѝ=1080;bool ћ;int ќ;public void ѱ(){if(й!=з.э)return;ќ=0;if(ћ)return;ћ=true;Ѧ=false;Э.я();ā.Ē("Drone",
"HOLD — waiting for landing clearance");}public void Ѳ(){if(!ћ)return;ў("cleared");}void ў(string ѳ){ћ=false;ќ=0;ā.Ē("Drone",$"Proceeding to dock ({ѳ})");if(!
Э.ѭ(ж))ѐ(з.ϕ);}const int Ѵ=360;const double ѵ=75.0;class ѻ{public Vector3D Ѷ,ѷ;public long ʯ,Ѹ=-1;public bool ѹ,Ѻ;}
readonly Dictionary<string,ѻ>Ѽ=new Dictionary<string,ѻ>();int ѽ=250,Ѿ=300,ѧ;Vector3D ѿ,Ҁ;bool ҁ,Ѧ;long Ҋ=-1;string ҋ="";public
void ʜ(int Ф){ѽ=Ф;}public void ʛ(int ȼ,int Ƚ,Vector3D Ҍ){ѽ=ȼ;Ѿ=Ƚ;ѿ=Ҍ;ҁ=true;}public void ҏ(string ĥ,string ҍ,string Ҏ,
Vector3D ð){ѻ V;if(!Ѽ.TryGetValue(ĥ,out V)){V=new ѻ();Ѽ[ĥ]=V;}V.ѷ=V.Ѷ;V.Ѹ=V.ʯ>0?V.ʯ:-1;V.Ѷ=ð;V.ʯ=ˇ;V.ѹ=ҍ=="TravelingOut"||ҍ==
"TravelingHome";V.Ѻ=Ҏ.Length>0;}void щ(){if(ѽ<=0)return;if(й!=з.ѓ&&й!=з.э)return;if(ћ)return;var Ґ=ˉ.Z();if(ҁ&&Ѿ>0){double ґ=Ѿ*2.0;if(
Vector3D.DistanceSquared(Ґ,ѿ)<ґ*ґ){if(Ѧ)Ғ("hub airspace");return;}}if(Ѧ){ѧ++;ѻ V;bool ғ=!Ѽ.TryGetValue(ҋ,out V)||ˇ-V.ʯ>30||!V.ѹ
||V.Ѻ;if(!ғ){var Ҕ=V.Ѷ-Ґ;double ҕ=Ҕ.LengthSquared();if(ҕ>(double)ѽ*ѽ){ғ=true;}else if(ҕ>1&&V.Ѹ>=0&&V.ʯ>V.Ѹ&&V.ʯ-V.Ѹ<=30){
var Җ=(V.Ѷ-V.ѷ)*(6.0/(V.ʯ-V.Ѹ));double җ=-Vector3D.Dot(Ҕ,Җ)/System.Math.Sqrt(ҕ);if(җ<4)ғ=true;}}if(ғ||ѧ>Ѵ||ъ())Ғ(
"separation clear");return;}Vector3D Ҙ=Vector3D.Zero;if(Ҋ>0&&ˇ>Ҋ&&ˇ-Ҋ<30)Ҙ=(Ґ-Ҁ)*(6.0/(ˇ-Ҋ));Ҁ=Ґ;Ҋ=ˇ;string ҙ=null;double Қ=ѽ;foreach(var
Ȃ in Ѽ){var қ=Ȃ.Value;if(ˇ-қ.ʯ>30)continue;if(!қ.ѹ||қ.Ѻ)continue;var Ҕ=қ.Ѷ-Ґ;double Ҝ=Ҕ.Length();if(Ҝ>=Қ)continue;if(қ.Ѹ<
0||қ.ʯ<=қ.Ѹ||қ.ʯ-қ.Ѹ>30)continue;var ҝ=(қ.Ѷ-қ.ѷ)*(6.0/(қ.ʯ-қ.Ѹ));var Ҟ=ҝ-Ҙ;double ҟ=Ҟ.LengthSquared();if(ҟ<16)continue;
double Ҡ=-Vector3D.Dot(Ҕ,Ҟ)/ҟ;if(Ҡ<=0||Ҡ>20)continue;double ҡ=(Ҕ+Ҟ*Ҡ).Length();if(ҡ>ѵ)continue;Қ=Ҝ;ҙ=Ȃ.Key;}if(ҙ==null)return;
if(string.CompareOrdinal(е,ҙ)<=0)return;Ѧ=true;ѧ=0;ҋ=ҙ;Э.я();ā.Č("Drone",
$"Yielding — collision course with {ҙ} at {(int)Қ}m");}void Ғ(string ѳ){Ѧ=false;ѧ=0;ā.Ē("Drone",$"Resuming ({ѳ})");if(!Э.ѭ(й==з.ѓ?л:ж))ѐ(з.ϕ);}public void Ń(){if(й==з.ь||й
==з.ы||й==з.ʮ)return;б.ю();Э.я();ѐ(з.ϕ);ā.Č("Drone","EMERGENCY STOP — SAM halted");}const float ѫ=30f;bool љ;void ђ(string
ѯ){б.ю();var Ƣ=ч();bool ı=Ƣ!=null&&Ƣ.Status==MyShipConnectorStatus.Connected;if(ı&&ъ()){љ=true;ā.Č("Drone",
$"Low power — charging at station before return (B:{г.Ϡ:F0}% H:{г.ϡ:F0}%)");ѐ(з.ϔ);return;}љ=false;н=0;р=false;ā.Ē("Drone",$"Going home: {ѯ}");if(ı){Ƣ.Disconnect();р=true;ā.Ē("Drone",
"Force-disconnected");}if(!Э.ѭ(ж)){if(ı)Ƣ.Connect();ā.Č("Drone","SAM GoTo home failed");ѐ(з.ϕ);return;}ѐ(з.э);}bool ъ(){if(г.Ϡ>=0&&г.Ϡ<ц)
return true;if(г.ϡ>=0&&г.ϡ<ц)return true;return false;}void Ѣ(){var ð=ˉ.Z();д.Ɗ(Ƌ.ǈ,е,Đ.đ(й),к,м,Я.ї.ToString("F1"),ж,ћ?"HOLD"
:Ѧ?"YIELD":"",г.Ϡ.ToString("F1"),г.ϡ.ToString("F1"),г.Ϣ?"1":"0",(long)ð.X,(long)ð.Y,(long)ð.Z);}long Ң;bool ё(
IMyShipConnector Ƣ){var ĳ=Ƣ!=null?Ƣ.OtherConnector:null;if(ĳ==null)return false;if(Ң!=0&&ĳ.CubeGrid.EntityId==Ң)return true;bool ң=ĳ.
CustomName.IndexOf(ж,O.Á)>=0||(ĳ.CustomData!=null&&ĳ.CustomData.IndexOf(ж,O.Á)>=0);if(ң)Ң=ĳ.CubeGrid.EntityId;return ң;}
IMyShipConnector ч(){var Ĳ=ˉ.c();IMyShipConnector Ҥ=null;IMyShipConnector ҥ=null;for(int É=0;É<Ĳ.Count;É++){if(Ĳ[É].Closed)continue;if(ҥ
==null)ҥ=Ĳ[É];if(Ĳ[É].Status==MyShipConnectorStatus.Connected)return Ĳ[É];if(Ҥ==null&&Ĳ[É].Status==MyShipConnectorStatus.
Connectable)Ҥ=Ĳ[É];}return Ҥ!=null?Ҥ:ҥ;}}class а{readonly Program A;readonly W ˉ;readonly Ā ā;readonly Ҧ ҧ=new Ҧ();readonly List<
IMyInventory>Ҩ=new List<IMyInventory>(),ҩ=new List<IMyInventory>();bool Ҫ;int ҫ,Ҭ,ҭ;public bool ί{get{return Ҫ;}}public а(Program V,
W ė,Ā ă){A=V;ˉ=ė;ā=ă;}public void ѕ(){Ҩ.Clear();ҩ.Clear();var Ƣ=ч();if(Ƣ==null||Ƣ.Status!=MyShipConnectorStatus.Connected
){ā.Č("Loader","Not connected");return;}var Ү=Ƣ.OtherConnector;if(Ү==null)return;var ү=Ү.CubeGrid;var Ұ=new List<
IMyTerminalBlock>();A.GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Ұ,È=>È.CubeGrid==ү&&È.HasInventory&&(È is IMyCargoContainer||
È is IMyShipDrill||È is IMyShipConnector));for(int É=0;É<Ұ.Count;É++){for(int ŧ=0;ŧ<Ұ[É].InventoryCount;ŧ++)Ҩ.Add(Ұ[É].
GetInventory(ŧ));}var ұ=ˉ.b();for(int É=0;É<ұ.Count;É++){for(int ŧ=0;ŧ<ұ[É].InventoryCount;ŧ++)ҩ.Add(ұ[É].GetInventory(ŧ));}var Ҳ=ˉ.
c();for(int É=0;É<Ҳ.Count;É++){for(int ŧ=0;ŧ<Ҳ[É].InventoryCount;ŧ++)ҩ.Add(Ҳ[É].GetInventory(ŧ));}if(ҩ.Count==0){ā.Č(
"Loader","No drone cargo found");return;}ҧ.ҳ(Ҩ);Ҫ=true;ҫ=0;Ҭ=0;ҭ=0;ā.Ē("Loader",
$"Pulling from {Ҩ.Count} station inventories into {ҩ.Count} drone inventories");}public void ю(){Ҫ=false;Ҩ.Clear();ҩ.Clear();}public void ɴ(){if(!Ҫ)return;if(ҧ.Ҵ){Ҫ=false;return;}ҫ++;int Ҹ=ҧ.ҵ(ę.ȧ,(
Ҷ,ŷ)=>{if(!Ū.ū(Ҷ.Type))return false;for(int ʵ=0;ʵ<ҩ.Count;ʵ++){var Ź=ҩ[ʵ];if(Ź.IsFull)continue;bool ҷ=ŷ.TransferItemTo(Ź,
Ҷ);if(ҷ){Ҭ++;return true;}}ҭ++;return false;});if(ҫ%30==0)ā.Ē("Loader",
$"tick:{ҫ} moved:{Ҭ} fails:{ҭ} src:{Ҩ.Count} dst:{ҩ.Count}");if(ҧ.Ҵ){ҧ.ҳ(Ҩ);}}IMyShipConnector ч(){var Ĳ=ˉ.c();return Ĳ.Count>0?Ĳ[0]:null;}}class ļ{readonly Ā ā;readonly х ҹ;
readonly ƈ д;readonly string е;long Һ;һ Ҽ;public х ľ{get{return ҹ;}}public ļ(Program V,Ā ă,W ė,ƈ Ɖ,ǽ ĭ){ā=ă;д=Ɖ;е=ģ.Ĥ(V.Me.
CubeGrid.EntityId);var с=new Ь(ė,ă);var т=new Ю(ė);var у=new а(V,ė,ă);var ф=new в(ė);ҹ=new х(ă,с,т,у,ф,ė,Ɖ,е,ĭ.ǣ);Ҽ=new һ(V,ф,ė)
;ҽ();ā.Ē("Drone",$"ID: {е}, HomeDock: {ĭ.ǣ}");}void ҽ(){д.ư(Ƌ.Ĕ,new Ҿ(this));д.ư(Ƌ.Ǌ,new ҿ(this));д.ư(Ƌ.ƌ,new Ӏ(this));д.
ư(Ƌ.ǋ,new Ӂ(this));д.ư(Ƌ.ǌ,new ӂ(this));д.ư(Ƌ.ǈ,new Ӄ(this));}public void ɮ(){ҹ.ɴ();Ҽ.ɴ();}class Ҿ:Ʃ{readonly ļ ӄ;public
Ҿ(ļ Ӆ){ӄ=Ӆ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ.Count<3)return;if(Ƶ.ǐ[0]!=ӄ.е)return;ӄ.Һ=ƽ;ӄ.ҹ.Ѯ(Ƶ.ǐ[1],Ƶ.ǐ[2]);}}class ҿ:Ʃ{
readonly ļ ӄ;public ҿ(ļ Ӆ){ӄ=Ӆ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ.Count<2)return;if(Ƶ.ǐ[0]!=ӄ.е)return;ӄ.Һ=ƽ;ӄ.ҹ.Ѱ(Ƶ.ǐ[1]);}}
class Ӂ:Ʃ{readonly ļ ӄ;public Ӂ(ļ Ӆ){ӄ=Ӆ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ.Count<1)return;if(Ƶ.ǐ[0]!=ӄ.е)return;if(ӄ.Һ!=0&&ƽ
!=ӄ.Һ)return;ӄ.ҹ.ѱ();}}class ӂ:Ʃ{readonly ļ ӄ;public ӂ(ļ Ӆ){ӄ=Ӆ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ.Count<1)return;if(Ƶ.ǐ[0]
!=ӄ.е)return;if(ӄ.Һ!=0&&ƽ!=ӄ.Һ)return;ӄ.ҹ.Ѳ();}}class Ӄ:Ʃ{readonly ļ ӄ;public Ӄ(ļ Ӆ){ӄ=Ӆ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ
.Count<13)return;string ʖ=Ƶ.ǐ[0];if(ʖ==ӄ.е)return;double ӆ,ο,ґ;if(!double.TryParse(Ƶ.ǐ[10],out ӆ)||!double.TryParse(Ƶ.ǐ[
11],out ο)||!double.TryParse(Ƶ.ǐ[12],out ґ))return;ӄ.ҹ.ҏ(ʖ,Ƶ.ǐ[1],Ƶ.ǐ[6],new Vector3D(ӆ,ο,ґ));}}class Ӏ:Ʃ{readonly ļ ӄ;
public Ӏ(ļ Ӆ){ӄ=Ӆ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(ӄ.Һ!=0&&ƽ!=ӄ.Һ)return;ӄ.ҹ.Ń();}}}class һ{readonly Program A;readonly в г;
readonly W ˉ;readonly List<IMyLightingBlock>Ӈ=new List<IMyLightingBlock>();Color ӈ;long Ӊ=-1;static readonly Color ӊ=new Color(0
,255,0),Ӌ=new Color(255,160,0),ӌ=new Color(255,0,0);public һ(Program V,в ф,W ė){A=V;г=ф;ˉ=ė;}public void ɴ(){if(ˉ.k!=Ӊ){Ӊ
=ˉ.k;Ӎ();ӈ=default(Color);}if(Ӈ.Count==0)return;float ӎ=100f;if(г.Ϡ>=0)ӎ=г.Ϡ;if(г.ϡ>=0&&г.ϡ<ӎ)ӎ=г.ϡ;Color ŵ;if(ӎ>50f)ŵ=ӊ;
else if(ӎ>20f)ŵ=Ӌ;else ŵ=ӌ;if(ŵ==ӈ)return;ӈ=ŵ;for(int É=0;É<Ӈ.Count;É++){if(Ӈ[É].Closed)continue;Ӈ[É].Color=ŵ;}}void Ӎ(){Ӈ.
Clear();var ӏ=new List<IMyTerminalBlock>();A.GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(ӏ,È=>È.CubeGrid==A.Me.
CubeGrid);for(int É=0;É<ӏ.Count;É++){if(Ñ.é(ӏ[É].CustomName,Õ.ù))Ӈ.Add((IMyLightingBlock)ӏ[É]);}var Ӑ=new List<IMyBlockGroup>();
A.GridTerminalSystem.GetBlockGroups(Ӑ);for(int Ï=0;Ï<Ӑ.Count;Ï++){if(!Ñ.é(Ӑ[Ï].Name,Õ.ù))continue;var ŗ=new List<
IMyTerminalBlock>();Ӑ[Ï].GetBlocks(ŗ,È=>È.CubeGrid==A.Me.CubeGrid);for(int È=0;È<ŗ.Count;È++){var Ú=ŗ[È]as IMyLightingBlock;if(Ú!=null&&
!Ӈ.Contains(Ú))Ӈ.Add(Ú);}}}}enum з{и,ʮ,ы,ѓ,є,і,ϔ,э,ϕ,ь}class Ю{readonly W ˉ;float ӑ;public float ї{get{return ӑ;}}public
Ю(W ė){ˉ=ė;}public bool ј(float Ӓ){return ӑ>=Ӓ*100f;}public void ɭ(){long ӓ=0;long Ӕ=0;var Ü=ˉ.b();for(int É=0;É<Ü.Count;
É++){for(int ŧ=0;ŧ<Ü[É].InventoryCount;ŧ++){var ӕ=Ü[É].GetInventory(ŧ);ӓ+=ӕ.CurrentVolume.RawValue;Ӕ+=ӕ.MaxVolume.
RawValue;}}var ġ=ˉ.c();for(int É=0;É<ġ.Count;É++){for(int ŧ=0;ŧ<ġ[É].InventoryCount;ŧ++){var ӕ=ġ[É].GetInventory(ŧ);ӓ+=ӕ.
CurrentVolume.RawValue;Ӕ+=ӕ.MaxVolume.RawValue;}}ӑ=Ӕ>0?(float)ӓ/Ӕ*100f:0f;}}class в{readonly W ˉ;float Ӗ,ӗ;bool Ә;public float Ϡ{get{
return Ӗ;}}public float ϡ{get{return ӗ;}}public bool Ϣ{get{return Ә;}}public в(W ė){ˉ=ė;}public void ɭ(){float ә=0;float Ӛ=0;Ә
=false;var ӛ=ˉ.g();for(int É=0;É<ӛ.Count;É++){var â=ӛ[É];if(â.Closed||!â.IsFunctional)continue;ә+=â.CurrentStoredPower;Ӛ
+=â.MaxStoredPower;if(â.ChargeMode==ChargeMode.Recharge||â.IsCharging)Ә=true;}Ӗ=Ӛ>0?ә/Ӛ*100f:-1f;float Ӝ=0;float ӝ=0;var Ӟ
=ˉ.h();for(int É=0;É<Ӟ.Count;É++){var ã=Ӟ[É];if(ã.Closed||!ã.IsFunctional)continue;if(ã.BlockDefinition.SubtypeId.
Contains("Hydrogen")){Ӝ+=(float)ã.FilledRatio*ã.Capacity;ӝ+=ã.Capacity;}}ӗ=ӝ>0?Ӝ/ӝ*100f:-1f;}}class Ь{readonly W ˉ;readonly Ā ā;
public Ь(W ė,Ā ă){ˉ=ė;ā=ă;}public bool ѭ(string ĵ){return ē($"GO {ĵ}");}public bool я(){return ē("STOP");}bool ē(string ӟ){var
å=ˉ.d();if(å==null||!å.IsFunctional){ā.Č("SAM","SAM PB not available");return false;}bool ҷ=å.TryRun(ӟ);ā.Ē("SAM",
$"\"{ӟ}\" -> {(ҷ?"OK":"FAILED")}");return ҷ;}}class ņ{readonly W ˉ;readonly Ā ā;bool Ҫ,Ӡ;ӡ Ӣ,ӣ;string Ӥ;int ӥ;bool Ӧ;public bool ί{get{return Ҫ;}}public
ӡ ΰ{get{return Ӣ;}}public string α{get{return Ӥ;}}public ņ(W ė,Ā ă){ˉ=ė;ā=ă;}public void Ө(ӡ ӧ,string Ҏ){if(Ӧ&&ӣ==ӧ)
return;if(Ҫ&&Ӣ==ӧ)return;Ҫ=true;Ӣ=ӧ;Ӥ=Ҏ;ā.Č("Alert",$"{Đ.đ(ӧ)}: {Ҏ}");}public void ň(){if(Ҫ){Ӧ=true;ӣ=Ӣ;}ө();Ҫ=false;ā.Ē(
"Alert","Dismissed (silenced until this alert clears)");}public void Ӫ(){Ӧ=false;if(!Ҫ)return;Ҫ=false;ө();}void ө(){Ӡ=false;ӥ=0
;var ӫ=ˉ.e();for(int É=0;É<ӫ.Count;É++){ӫ[É].Enabled=true;ӫ[É].Color=Color.White;ӫ[É].BlinkIntervalSeconds=0;ӫ[É].
BlinkLength=0;}var Ӭ=ˉ.f();for(int É=0;É<Ӭ.Count;É++)Ӭ[É].Stop();}public void Ǝ(){Ҫ=true;Ӧ=false;Ӣ=ӡ.ӭ;Ӥ="TEST";ā.Ē("Alert",
"Test alert activated");}public void ɴ(){if(!Ҫ)return;ӥ++;if(ӥ>=3){ӥ=0;Ӡ=!Ӡ;Ӯ(Ӡ,new Color(255,0,0));ӯ(Ӡ);}}void Ӯ(bool Ӱ,Color Ν){var ӫ=ˉ.e();
for(int É=0;É<ӫ.Count;É++){ӫ[É].Enabled=Ӱ;ӫ[É].Color=Ν;}}void ӯ(bool Ӱ){var Ӭ=ˉ.f();for(int É=0;É<Ӭ.Count;É++){if(Ӱ)Ӭ[É].
Play();else Ӭ[É].Stop();}}}class ɘ{readonly W ˉ;readonly Program A;readonly Ā ā;readonly Ҧ ҧ=new Ҧ();readonly List<
IMyInventory>ӱ=new List<IMyInventory>();readonly List<IMyShipConnector>Ӳ=new List<IMyShipConnector>();readonly HashSet<long>ӳ=new
HashSet<long>();IMyShipConnector Ӵ;bool Ҫ,ӵ;int Ӷ,ҭ,ӷ;public bool ί{get{return Ҫ||Ӳ.Count>0;}}public bool ɱ{get{return ӵ;}}
public ɘ(W ė,Program V,Ā ă){ˉ=ė;A=V;ā=ă;}public void ɴ(){if(ӵ&&!Ҫ){ӷ++;if(ӷ>=ę.Ƞ){ӷ=0;if(Ӹ()){ӵ=false;ӳ.Clear();ā.Ē("Sort",
"Storage has space again — retrying docked drones");}}}ӹ();Ӻ();if(!Ҫ&&Ӳ.Count>0){Ӵ=Ӳ[0];Ӳ.RemoveAt(0);if(Ӵ.Status==MyShipConnectorStatus.Connected)ӻ(Ӵ);}if(!Ҫ)return;if(Ӵ
==null||Ӵ.Closed||Ӵ.Status!=MyShipConnectorStatus.Connected){Ҫ=false;ӱ.Clear();ā.Ē("Sort","Drone disconnected mid-sort");
return;}ҧ.ҵ(ę.ȧ,(Ҷ,ŷ)=>{if(!Ū.ū(Ҷ.Type))return false;string ú=Ҷ.Type.SubtypeId;IMyCargoContainer Қ=Ӽ(Õ.û(ú));if(Қ==null){bool
ӽ=ˉ.n(Õ.û(ú)).Count>0;if(!ӽ)Қ=Ӽ(Õ.ö);}if(Қ==null)Қ=Ӽ(Õ.õ);if(Қ!=null){bool ҷ=Ӿ(ŷ,Ҷ,Қ);if(!ҷ)ҭ++;return ҷ;}Ӷ++;return
false;});if(ҧ.Ҵ){if(Ӷ>0){ӵ=true;ā.Č("Sort",$"Sort done — {Ӷ} items stuck (hub storage full!)");}else{ӵ=false;ā.Ē("Sort",
"Sort complete");}if(ҭ>0)ā.Č("Sort",$"{ҭ} transfers failed — check conveyor path");Ҫ=false;Ӵ=null;ӱ.Clear();Ӷ=0;ҭ=0;}}bool Ӹ(){var Ş=ˉ.
b();for(int É=0;É<Ş.Count;É++){if(Ş[É].CubeGrid!=A.Me.CubeGrid)continue;if(!ˉ.q(Ş[É].EntityId))continue;var ŧ=Ş[É].
GetInventory(0);if(ŧ.IsFull)continue;float Ӕ=(float)ŧ.MaxVolume.RawValue;if(Ӕ<=0)continue;float ӿ=(float)ŧ.CurrentVolume.RawValue/Ӕ;
float Ԁ=ˉ.º(Ş[É].EntityId);if(ӿ<Ԁ)return true;}return false;}void ӹ(){var ġ=ˉ.c();for(int É=0;É<ġ.Count;É++){var Ť=ġ[É];if(Ť.
Closed||Ť.Status!=MyShipConnectorStatus.Connected)continue;var ĳ=Ť.OtherConnector;if(ĳ==null||ĳ.CubeGrid==A.Me.CubeGrid)
continue;long ʖ=Ť.EntityId;if(ӳ.Contains(ʖ))continue;ӳ.Add(ʖ);Ӳ.Add(Ť);ā.Ē("Sort",$"Drone queued on {Ť.CustomName}");}}readonly
List<long>ԁ=new List<long>();void Ӻ(){var Ԃ=ԁ;Ԃ.Clear();foreach(long ʖ in ӳ){bool ԃ=false;var ġ=ˉ.c();for(int É=0;É<ġ.Count;
É++){if(ġ[É].EntityId==ʖ&&ġ[É].Status==MyShipConnectorStatus.Connected){ԃ=true;break;}}if(!ԃ)Ԃ.Add(ʖ);}for(int É=0;É<Ԃ.
Count;É++)ӳ.Remove(Ԃ[É]);}public bool Ԅ(string l){return Ӽ(l)!=null;}IMyCargoContainer Ӽ(string l){var Ş=ˉ.n(l);if(Ş.Count==0
)return null;IMyCargoContainer Қ=null;float ԅ=1f;for(int Ť=0;Ť<Ş.Count;Ť++){if(Ş[Ť].Closed)continue;var Ź=Ş[Ť].
GetInventory(0);if(Ź.IsFull)continue;float Ӕ=(float)Ź.MaxVolume.RawValue;if(Ӕ<=0)continue;float ӿ=(float)Ź.CurrentVolume.RawValue/Ӕ;
float Ԁ=ˉ.º(Ş[Ť].EntityId);if(ӿ>=Ԁ)continue;if(ӿ<ԅ){ԅ=ӿ;Қ=Ş[Ť];}}return Қ;}bool Ӿ(IMyInventory Ԇ,MyInventoryItem Ҷ,
IMyCargoContainer ԇ){var Ź=ԇ.GetInventory(0);float Ԁ=ˉ.º(ԇ.EntityId);if(Ԁ>=1f)return Ԇ.TransferItemTo(Ź,Ҷ);double Ԉ=(double)Ź.MaxVolume.
RawValue*Ԁ-(double)Ź.CurrentVolume.RawValue;if(Ԉ<=0)return false;var ԉ=Ҷ.Type.GetItemInfo();if(ԉ.Volume<=0)return Ԇ.
TransferItemTo(Ź,Ҷ);MyFixedPoint Ԋ=(MyFixedPoint)(Ԉ/1000000.0/ԉ.Volume);if(Ԋ<=0)return false;if(Ԋ>=Ҷ.Amount)return Ԇ.TransferItemTo(Ź,
Ҷ);return Ԇ.TransferItemTo(Ź,Ҷ,Ԋ);}int ԋ;public void Ԑ(){if(Ҫ)return;ԋ++;if(ԋ<3)return;ԋ=0;var Ş=ˉ.b();var ԍ=Ԍ;int Ҹ=0;
for(int É=0;É<Ş.Count&&Ҹ<20;É++){var Ť=Ş[É];if(Ť.CubeGrid!=A.Me.CubeGrid)continue;var ŧ=Ť.GetInventory(0);ԍ.Clear();ŧ.
GetItems(ԍ);for(int ũ=ԍ.Count-1;ũ>=0&&Ҹ<20;ũ--){var Ҷ=ԍ[ũ];if(!Ū.ū(Ҷ.Type))continue;string ú=Ҷ.Type.SubtypeId;string Ų=Õ.û(ú);
bool ӽ=ˉ.n(Ų).Count>0;bool ԏ=Ԏ(Ų,Ť)||(!ӽ&&Ԏ(Õ.ö,Ť));if(ԏ)continue;IMyCargoContainer Қ=Ӽ(Ų);if(Қ==null&&!ӽ)Қ=Ӽ(Õ.ö);if(Қ!=
null&&Қ!=Ť){if(Ӿ(ŧ,Ҷ,Қ))Ҹ++;}}}if(Ҹ>0)ā.Ē("Sort",$"Organized {Ҹ} items on hub");}readonly List<MyInventoryItem>Ԍ=new List<
MyInventoryItem>();public void Ԓ(HashSet<string>ԑ){ԑ.Clear();var m=ˉ.n(Õ.õ);for(int É=0;É<m.Count;É++){if(m[É].Closed)continue;Ԍ.Clear(
);m[É].GetInventory(0).GetItems(Ԍ);for(int ũ=0;ũ<Ԍ.Count;ũ++)if(Ū.ū(Ԍ[ũ].Type))ԑ.Add(Ԍ[ũ].Type.SubtypeId);}Ԍ.Clear();}
bool Ԏ(string l,IMyCargoContainer Ť){var m=ˉ.n(l);for(int É=0;É<m.Count;É++)if(m[É]==Ť)return true;return false;}void ӻ(
IMyShipConnector Ƣ){ӱ.Clear();var Ү=Ƣ.OtherConnector;if(Ү==null)return;var ԓ=Ү.CubeGrid;var ƞ=new List<IMyTerminalBlock>();A.
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(ƞ,È=>È.CubeGrid==ԓ&&È.HasInventory);for(int É=0;É<ƞ.Count;É++){for(int ŧ=0;ŧ<ƞ[É].
InventoryCount;ŧ++)ӱ.Add(ƞ[É].GetInventory(ŧ));}ҧ.ҳ(ӱ);Ҫ=ӱ.Count>0;if(Ҫ)ā.Ē("Sort",
$"Sorting from {Ƣ.CustomName} — {ӱ.Count} inventories");}}struct ԗ{public Ԕ ĕ;public ԕ Ɛ;public string Ԗ;}class ɚ{readonly ŏ Ԙ;readonly ɒ ԙ;readonly Ā ā;readonly ȏ Ԛ;readonly
ǥ ԛ;int Ԝ,ԝ,Ԟ;readonly HashSet<string>ԟ=new HashSet<string>(),Й=new HashSet<string>();readonly ȭ Ԡ;readonly string[]ԡ;
readonly ɘ Ԣ;readonly W ˉ;const int ԣ=180;bool Ԥ;public bool ɲ{get{return Ԥ;}}public int ԥ{get{return ԝ;}}readonly List<ԕ>Ԧ=new
List<ԕ>();readonly ԧ Ա;class ԧ:IComparer<ԕ>{readonly ɚ Բ;public ԧ(ɚ Գ){Բ=Գ;}public int Compare(ԕ Դ,ԕ È){int Զ=Բ.Ե(Դ.Ć);int å
=Բ.Ե(È.Ć);if(Զ!=å)return Զ.CompareTo(å);return string.Compare(Դ.Ć,È.Ć,O.Ȝ);}}public ɚ(ŏ Ő,ɒ ɓ,ȏ ĭ,ǥ Ȳ,ȭ ʇ,string[]ɗ,ɘ ə,W
ė,Ā ă){Ԙ=Ő;ԙ=ɓ;Ԛ=ĭ;ԛ=Ȳ;Ԡ=ʇ;ԡ=ɗ??new string[0];Ԣ=ə;ˉ=ė;ā=ă;Ա=new ԧ(this);}bool Է(ԕ ǿ){if(ǿ.ˁ||!ǿ.έ)return false;long ά=Ԡ.ʯ
-ǿ.ˀ;return ά<=ԣ;}bool Ի(ԕ Ѭ){if(Ѭ.Ը.Count==0)return true;string Թ=null;float Ժ=0;foreach(var Ȃ in Ѭ.Ը){if(Ȃ.Value>Ժ){Ժ=Ȃ
.Value;Թ=Ȃ.Key;}}if(Թ==null)return true;if(Й.Contains(Թ))return false;string l=Õ.û(Թ);if(Ԣ.Ԅ(l))return true;if(ˉ.n(l).
Count==0&&Ԣ.Ԅ(Õ.ö))return true;return false;}public void ɴ(){if(Ԟ>0)Ԟ--;Ԣ.Ԓ(Й);ԟ.Clear();foreach(var Ȃ in Ԙ.ʴ){var ʵ=Ȃ.Value;
if(ʵ.Ɔ==Ϗ.ϐ||ʵ.Ɔ==Ϗ.Ϩ||ʵ.Ɔ==Ϗ.ϖ||ʵ.Ɔ==Ϗ.ϕ)continue;if(!string.IsNullOrEmpty(ʵ.ʷ))ԟ.Add(ʵ.ʷ);}foreach(var Ȃ in ԙ.ö){var ǿ=Ȃ
.Value;if(ǿ.Լ){ǿ.Խ=Ծ.Կ(ǿ,Ū.ǻ,Ԛ.Ǚ);ǿ.Լ=false;}}int Հ=0;int Ձ=0;foreach(var Ȃ in ԙ.ö){var ǿ=Ȃ.Value;if(!Է(ǿ))continue;if(ԟ.
Contains(ǿ.Ć))continue;Ձ++;if(!Ի(ǿ))Հ++;}ԝ=Հ;Ԥ=Հ>0&&Հ>=Ձ;if(Ԥ&&Ԟ<=0){ā.Ē("Dispatch",
$"{Հ} station(s) skipped — no container space");Ԟ=60;}}public ԗ?Յ(){var Ձ=Ԙ.Ղ();if(Ձ.Count==0)return null;if(ԛ==ǥ.Ǻ)return Ճ(Ձ);else return Մ(Ձ);}ԗ?Ճ(List<Ԕ>Ձ){var Ն=
Ԧ;Ն.Clear();foreach(var Ȃ in ԙ.ö){if(Է(Ȃ.Value))Ն.Add(Ȃ.Value);}if(Ն.Count==0)return null;Ն.Sort(Ա);ԕ Ѭ=null;int Շ=Ն.
Count;for(int Դ=0;Դ<Շ;Դ++){int Ո=(Ԝ+Դ)%Ն.Count;var Չ=Ն[Ո];if(ԟ.Contains(Չ.Ć))continue;if(!Ի(Չ))continue;Ѭ=Չ;Ԝ=Ԝ+Դ+1;break;}if
(Ѭ==null)return null;var Պ=new ԗ();Պ.ĕ=Ձ[0];Պ.Ɛ=Ѭ;Պ.Ԗ=Ѭ.Ć;return Պ;}ԗ?Մ(List<Ԕ>Ձ){ԕ Қ=null;float Ջ=-1;foreach(var Ȃ in ԙ.
ö){var ǿ=Ȃ.Value;if(!Է(ǿ))continue;if(ԟ.Contains(ǿ.Ć))continue;if(!Ի(ǿ))continue;if(ǿ.Խ>Ջ){Ջ=ǿ.Խ;Қ=ǿ;}}if(Қ==null)return
null;var Պ=new ԗ();Պ.ĕ=Ձ[0];Պ.Ɛ=Қ;Պ.Ԗ=Қ.Ć;return Պ;}public ԗ?Տ(string Ռ){ԕ ǿ;if(!ԙ.Ս(Ռ,out ǿ)){ā.Č("Dispatch",
$"Unknown station '{Ռ}'. Known: {ԙ.Վ()}");return null;}if(ǿ.ˁ){ā.Č("Dispatch",$"{ǿ.Ć} is DISABLED");return null;}if(!ǿ.ʾ){ā.Č("Dispatch",
$"{ǿ.Ć} is CLOSED ({ǿ.ʿ:F0}% full)");return null;}if(ԟ.Contains(ǿ.Ć)){ā.Č("Dispatch",$"{ǿ.Ć} already has a drone assigned");return null;}var Ձ=Ԙ.Ղ();if(Ձ.
Count==0){ā.Č("Dispatch","No drone ready (docked, 99% charged, empty)");return null;}var Պ=new ԗ();Պ.ĕ=Ձ[0];Պ.Ɛ=ǿ;Պ.Ԗ=ǿ.Ć;
return Պ;}public void Ր(ԕ ǿ){ǿ.Լ=true;}int Ե(string Ɠ){for(int É=0;É<ԡ.Length;É++){if(ԡ[É].Equals(Ɠ,O.Á))return É;}return ԡ.
Length;}}class Ԕ{public string ģ{get;set;}public long ʽ{get;set;}public string ʶ{get;set;}public Ϗ Ɔ{get;set;}public string ʷ{
get;set;}public string Ց{get;set;}public string Ւ{get;set;}public string ǣ{get;set;}public long Փ{get;set;}public long Ք{
get;set;}public float ϟ{get;set;}public Օ Օ{get;set;}public int ή{get;set;}public Vector3D Ֆ{get;set;}public bool ՙ{get;set
;}public bool ϣ{get;set;}public bool Ϥ{get;set;}public bool ա{get;set;}public long բ{get;set;}public float Ϡ{get;set;}
public float ϡ{get;set;}public bool Ϣ{get;set;}public bool ϵ{get;set;}public bool Ϸ{get;set;}public string ʸ{get;set;}public
int ʹ{get;set;}public float ʺ{get;set;}public long ʻ{get;set;}public long ʼ{get;set;}public long Ђ{get;set;}public long գ{
get;set;}public Ԕ(string ĥ,long դ){ģ=ĥ;ʽ=դ;ʶ=$"Drone-{ĥ}";Ɔ=Ϗ.ե;ʷ="";Ց="";Ւ="";ǣ="";ʸ="IDLE";}}enum Ϗ{ե,ϐ,ϒ,ϑ,ϧ,ϔ,ϓ,Ϩ,ϖ,ϕ}
enum Օ{զ,է,ը,թ,ժ,ի}enum ӡ{ӭ,ɱ}class ŏ{readonly Dictionary<string,Ԕ>լ=new Dictionary<string,Ԕ>();readonly Ā ā;readonly ȭ Ԡ;
readonly ȅ խ;readonly long ծ;public ŏ(Ā ă,ȭ ʇ,ȅ ɑ,int կ){ā=ă;Ԡ=ʇ;խ=ɑ;ծ=կ*6L;}public IReadOnlyDictionary<string,Ԕ>ʴ{get{return լ;
}}public int ρ{get{return լ.Count;}}public Ԕ հ(string ĥ,long դ){Ԕ ʵ;if(!լ.TryGetValue(ĥ,out ʵ)){ʵ=new Ԕ(ĥ,դ);ʵ.ʶ=խ.Ȅ(ĥ);լ
[ĥ]=ʵ;ā.Ē("Fleet",$"New drone: {ʵ.ʶ}");}ʵ.ʽ=դ;return ʵ;}public void մ(string ĥ,Ϗ ҍ,string Ѭ,string Ҏ,float Ü,float â,
float ձ,bool ղ){Ԕ ʵ;if(!լ.TryGetValue(ĥ,out ʵ))return;ʵ.Փ=Ԡ.ʯ;ʵ.ϟ=Ü;ʵ.ϣ=Ҏ=="HOLD";ʵ.Ϥ=Ҏ=="YIELD";ʵ.ϵ=â>=0;ʵ.Ϸ=ձ>=0;ʵ.Ϡ=â>=0?â
:0;ʵ.ϡ=ձ>=0?ձ:0;ʵ.Ϣ=ղ;if(ʵ.Ɔ==Ϗ.ϕ){if(ҍ!=Ϗ.ϕ&&ҍ!=Ϗ.ե){ʵ.Ɔ=ҍ;ʵ.Օ=Օ.զ;ʵ.Ք=Ԡ.ʯ;ʵ.ʷ=Ѭ??"";if(ҍ==Ϗ.ϖ)ʵ.ʸ="HOME";else if(ҍ==Ϗ.ϐ
||ҍ==Ϗ.Ϩ)ʵ.ʸ="READY";else ʵ.ʸ="RUNNING";ā.Ē("Fleet",$"{ʵ.ʶ} recovered — {Đ.đ(ҍ)}");}return;}if(ҍ==Ϗ.ϖ)ʵ.ʸ="HOME";else if(ҍ
==Ϗ.ϕ&&ʵ.ʸ!="RECALLED")ʵ.ʸ="LOST";if((ʵ.Ɔ==Ϗ.ϓ||ʵ.Ɔ==Ϗ.ϔ)&&ҍ==Ϗ.ϕ)return;if(ʵ.Ɔ!=ҍ){if(ҍ==Ϗ.ϐ&&ʵ.գ>0&&(ʵ.Ɔ==Ϗ.ϓ||ʵ.Ɔ==Ϗ.ϑ
||ʵ.Ɔ==Ϗ.ϧ)){long ճ=Ԡ.ʯ-ʵ.գ;ʵ.Ђ=ճ;ʵ.ʹ++;ʵ.ʺ+=Ü;if(ʵ.ʻ==0||ճ<ʵ.ʻ)ʵ.ʻ=ճ;if(ճ>ʵ.ʼ)ʵ.ʼ=ճ;ʵ.գ=0;}ʵ.Ք=Ԡ.ʯ;}ʵ.Ɔ=ҍ;ʵ.ʷ=Ѭ??"";}
readonly List<Ԕ>յ=new List<Ԕ>();public List<Ԕ>Ղ(){var í=յ;í.Clear();foreach(var Ȃ in լ){var ʵ=Ȃ.Value;if(ʵ.Ɔ!=Ϗ.ϐ)continue;if(ʵ.
ϵ&&ʵ.Ϡ<99f)continue;if(ʵ.Ϸ&&ʵ.ϡ<99f)continue;if(ʵ.ϟ>5)continue;í.Add(ʵ);}return í;}public int շ(){int ն=0;foreach(var Ȃ
in լ)if(Ȃ.Value.Ɔ==Ϗ.ϕ)ն++;return ն;}public void ɴ(){foreach(var Ȃ in լ){var ʵ=Ȃ.Value;if(ʵ.Ɔ==Ϗ.ϕ)continue;long ո=Ԡ.ʯ-ʵ.Փ
;if(ո>ծ){ʵ.Ɔ=Ϗ.ϕ;ʵ.Օ=Օ.է;if(ʵ.ʸ!="RECALLED")ʵ.ʸ="LOST";ʵ.Ք=Ԡ.ʯ;ā.Č("Fleet",$"{ʵ.ʶ} LOST (silent {ո/6}s)");continue;}long
չ=Ԡ.ʯ-ʵ.Ք;if(ʵ.Ɔ==Ϗ.ϓ&&չ>ę.Ȧ){ʵ.Ɔ=Ϗ.ϕ;ʵ.Օ=Օ.ժ;ʵ.Ք=Ԡ.ʯ;ā.Č("Fleet",$"{ʵ.ʶ} LOST (recall not docked after 5min)");}}}public
void ő(){var Ԃ=new List<string>();foreach(var Ȃ in լ){var ʵ=Ȃ.Value;if(ʵ.ʸ=="RECALLED"||ʵ.ή>0)continue;if(ʵ.Ɔ==Ϗ.ϕ){long ո=Ԡ
.ʯ-ʵ.Ք;if(ո>ę.Ȣ)Ԃ.Add(Ȃ.Key);}}for(int É=0;É<Ԃ.Count;É++){ā.Ē("Fleet",$"Purged {լ[Ԃ[É]].ʶ}");լ.Remove(Ԃ[É]);}}public void
Ɉ(ȅ ɑ){foreach(var Ȃ in լ)Ȃ.Value.ʶ=ɑ.Ȅ(Ȃ.Key);}}class Ŋ{readonly Program A;readonly Ā ā;readonly þ ÿ;readonly ƈ д;
readonly ŏ Ԙ;readonly ɒ ԙ;readonly ɚ պ;readonly ɘ Ԣ;readonly ņ ջ;readonly ɜ ʥ;readonly ȏ Ԛ;readonly Dictionary<long,int>ռ=new
Dictionary<long,int>();ʀ й=ʀ.ʁ;readonly List<Ԕ>ս=new List<Ԕ>();Ԕ վ;int տ,ր;public ʀ Ɔ{get{return й;}}public Ŋ(Program V,Ā ă,þ Ă,ƈ
Ɖ,ŏ Ő,ɒ ɓ,ɚ ɛ,ɘ ə,ņ Ň,ɜ Ȯ,ȏ ĭ){A=V;ā=ă;ÿ=Ă;д=Ɖ;Ԙ=Ő;ԙ=ɓ;պ=ɛ;Ԣ=ə;ջ=Ň;ʥ=Ȯ;Ԛ=ĭ;ҽ();}void ҽ(){д.ư(Ƌ.ǈ,new ց(this));д.ư(Ƌ.Ǉ,new
ւ(this));}public void ƅ(){if(й==ʀ.փ)ք();й=ʀ.օ;foreach(var Ȃ in Ԙ.ʴ){if(Ȃ.Value.Ɔ!=Ϗ.ϖ&&Ȃ.Value.Ɔ!=Ϗ.ϕ)Ȃ.Value.ʸ="RUNNING"
;}ā.Ē("Hub","Fleet STARTED");}public void ō(){if(й==ʀ.փ)ք();й=ʀ.ʁ;foreach(var Ȃ in Ԙ.ʴ){if(Ȃ.Value.ʸ=="RUNNING"||Ȃ.Value.
ʸ=="RECALLED")Ȃ.Value.ʸ="READY";}ā.Ē("Hub","Fleet PAUSED");}void ք(){for(int É=0;É<ս.Count;É++){var ʵ=ս[É];ʵ.Ɔ=Ϗ.ϓ;ʵ.Ք=ÿ.
đ<ȭ>().ʯ;ʵ.ή=0;д.Ʒ(ʵ.ʽ,Ƌ.Ǌ,ʵ.ģ,"recall_released");}if(ս.Count>0)ā.Ē("Hub",
$"Recall interrupted — {ս.Count} queued drone(s) sent home");ս.Clear();վ=null;ր=0;}public void œ(){й=ʀ.փ;д.Ɗ(Ƌ.ƌ,"recall_stop");ā.Ē("Hub","RECALL — STOP sent to all drones");ս.
Clear();վ=null;foreach(var Ȃ in Ԙ.ʴ){var ʵ=Ȃ.Value;if(ʵ.Ɔ==Ϗ.ϐ||ʵ.Ɔ==Ϗ.Ϩ||ʵ.Ɔ==Ϗ.ϖ)continue;ʵ.Ɔ=Ϗ.ϔ;ʵ.Ք=ÿ.đ<ȭ>().ʯ;ʵ.ή=ս.
Count+1;ʵ.ʸ="RECALLED";ս.Add(ʵ);}ր=6;ā.Ē("Hub",$"RECALL — waiting 3s then recalling {ս.Count} drones one by one");}long ֆ;
void א(){while(ս.Count>0){var և=ս[0];ս.RemoveAt(0);if(և.Ɔ==Ϗ.ϐ||և.Ɔ==Ϗ.Ϩ){և.ή=0;continue;}վ=և;ֆ=ÿ.đ<ȭ>().ʯ;և.Ɔ=Ϗ.ϓ;և.ή=0;д.Ʒ
(և.ʽ,Ƌ.Ǌ,և.ģ,"recall");ā.Ē("Hub",$"Recalling {և.ʶ} ({ս.Count} remaining)");return;}վ=null;foreach(var Ȃ in Ԙ.ʴ){var ʵ=Ȃ.
Value;if(ʵ.ʸ=="RECALLED"||ʵ.ʸ=="RUNNING")ʵ.ʸ="READY";ʵ.ή=0;}ā.Ē("Hub","RECALL complete — all drones home");й=ʀ.ʁ;}public void
Ĕ(string Ɠ){if(ב()){ā.Č("Hub","Airspace busy — an inbound is on final approach; try again shortly");return;}var Պ=պ.Տ(Ɠ);
if(Պ==null){ā.Č("Hub",$"Cannot dispatch to {Ɠ}");return;}ג(Պ.Value);}public void ɮ(){Ԣ.ɴ();ջ.ɴ();ד();}void ד(){var ė=ÿ.đ<W
>();var ġ=ė.c();for(int É=0;É<ġ.Count;É++){var Ť=ġ[É];if(Ť.Closed||Ť.Status!=Sandbox.ModAPI.Ingame.MyShipConnectorStatus.
Connectable){ռ.Remove(Ť.EntityId);continue;}int Ô;ռ.TryGetValue(Ť.EntityId,out Ô);Ô++;if(Ô>180){Ť.Connect();ā.Ē("Hub",
$"Auto-locked dead drone: {Ť.CustomName}");Ô=0;}ռ[Ť.EntityId]=Ô;}}public void ɷ(){Ԙ.ɴ();պ.ɴ();if(й!=ʀ.փ&&ʥ.ɴ()){if(ʥ.ה)ō();else ƅ();}if(й==ʀ.փ){if(ր>0){ր--;if(ր
==0)א();}else if(վ!=null){if(վ.Ɔ==Ϗ.ϐ||վ.Ɔ==Ϗ.Ϩ||վ.Ɔ==Ϗ.ϖ||վ.Ɔ==Ϗ.ϕ)א();else if(ÿ.đ<ȭ>().ʯ-ֆ>ę.Ȧ){ā.Č("Hub",
$"Recall stuck on {վ.ʶ} — moving on");א();}}else if(ս.Count>0){א();}}else{if(Ԣ.ɱ)ו();ז();}ח();if(տ>0)տ--;if(й==ʀ.օ&&!Ԣ.ί&&տ<=0)ט();י();}void ו(){foreach(var
Ȃ in Ԙ.ʴ){var ʵ=Ȃ.Value;if(ʵ.Ɔ==Ϗ.ϧ||ʵ.Ɔ==Ϗ.ϔ){ʵ.Ɔ=Ϗ.ϓ;ʵ.Ք=ÿ.đ<ȭ>().ʯ;д.Ʒ(ʵ.ʽ,Ƌ.Ǌ,ʵ.ģ,"storage_full");ā.Ē("Hub",
$"Recalling {ʵ.ʶ} — hub storage full");}}}void ז(){foreach(var Ȃ in Ԙ.ʴ){var ʵ=Ȃ.Value;if(ʵ.Ɔ==Ϗ.ϐ||ʵ.Ɔ==Ϗ.Ϩ||ʵ.Ɔ==Ϗ.ϖ||ʵ.Ɔ==Ϗ.ϓ||ʵ.Ɔ==Ϗ.ϕ)continue;bool ך=ʵ.
ϵ&&ʵ.Ϡ<ę.ȩ;bool כ=ʵ.Ϸ&&ʵ.ϡ<ę.ȩ;if(ך||כ){ʵ.Ɔ=Ϗ.ϓ;ʵ.Ք=ÿ.đ<ȭ>().ʯ;д.Ʒ(ʵ.ʽ,Ƌ.Ǌ,ʵ.ģ,"low_power");ā.Č("Hub",
$"Recalling {ʵ.ʶ} — low power (B:{ʵ.Ϡ:F0}% H:{ʵ.ϡ:F0}%)");}}}public void ɸ(){ԙ.ל(Ԛ.Ǚ,Ԛ.Ǜ);Ԣ.Ԑ();}void ט(){if(ב())return;var Պ=պ.Յ();if(Պ==null)return;ג(Պ.Value);}bool ב(){if(Ԛ.
ǡ<=0)return false;Vector3D Ҍ=A.Me.GetPosition();double ם=(double)Ԛ.ǡ*Ԛ.ǡ;foreach(var Ȃ in Ԙ.ʴ){var ʵ=Ȃ.Value;if(ʵ.Ɔ!=Ϗ.ϓ)
continue;if(ʵ.ϣ)continue;if(!ʵ.ՙ)return true;if(Vector3D.DistanceSquared(ʵ.Ֆ,Ҍ)<ם)return true;}return false;}readonly Dictionary
<string,string>מ=new Dictionary<string,string>();readonly List<string>ן=new List<string>();void ח(){if(Ԛ.ǡ<=0)return;
Vector3D Ҍ=A.Me.GetPosition();double נ=(double)Ԛ.ǡ*1.5*(Ԛ.ǡ*1.5);long Ϋ=ÿ.đ<ȭ>().ʯ;ן.Clear();foreach(var Ȃ in מ){Ԕ ס;if(!Ԙ.ʴ.
TryGetValue(Ȃ.Value,out ס)||ס.Ɔ!=Ϗ.ϓ||ס.ǣ!=Ȃ.Key)ן.Add(Ȃ.Key);}for(int É=0;É<ן.Count;É++)מ.Remove(ן[É]);foreach(var Ȃ in Ԙ.ʴ){var ʵ
=Ȃ.Value;if(ʵ.Ɔ!=Ϗ.ϓ){ʵ.ա=false;continue;}if(!ʵ.ՙ)continue;string ע=ʵ.ǣ??"";string ף;if(!מ.TryGetValue(ע,out ף)){מ[ע]=ʵ.ģ
;if(ʵ.ա){ʵ.ա=false;д.Ʒ(ʵ.ʽ,Ƌ.ǌ,ʵ.ģ);ā.Ē("Hub",$"{ʵ.ʶ} cleared to dock");}continue;}if(ף==ʵ.ģ)continue;double Ҝ=Vector3D.
DistanceSquared(ʵ.Ֆ,Ҍ);if(Ҝ<נ&&(!ʵ.ա||Ϋ-ʵ.բ>240)){bool פ=ʵ.ա;ʵ.ա=true;ʵ.բ=Ϋ;ʵ.Ք=Ϋ;д.Ʒ(ʵ.ʽ,Ƌ.ǋ,ʵ.ģ);if(!פ)ā.Ē("Hub",
$"{ʵ.ʶ} holding — dock busy");}}}void ג(ԗ Պ){Պ.ĕ.Ɔ=Ϗ.ϒ;Պ.ĕ.ʷ=Պ.Ɛ.Ć;Պ.ĕ.Ւ=Պ.Ԗ;Պ.ĕ.ʸ="RUNNING";Պ.ĕ.գ=ÿ.đ<ȭ>().ʯ;д.Ʒ(Պ.ĕ.ʽ,Ƌ.Ĕ,Պ.ĕ.ģ,Պ.Ɛ.Ć,Պ.Ԗ);տ=24;ā.
Ē("Hub",$"Dispatched {Պ.ĕ.ʶ} -> {Պ.Ɛ.Ć}");}void י(){if(й==ʀ.փ){ջ.Ӫ();return;}int Ϧ=Ԙ.շ();if(Ϧ>0){ջ.Ө(ӡ.ӭ,
$"{Ϧ} drone(s) lost");return;}if(Ԣ.ɱ){ջ.Ө(ӡ.ɱ,"Hub storage full — drones cannot unload!");return;}if(պ.ԥ>0){ջ.Ө(ӡ.ɱ,
$"{պ.ԥ} station(s) paused — main containers full");return;}ջ.Ӫ();}class ց:Ʃ{readonly Ŋ ץ;public ց(Ŋ ŋ){ץ=ŋ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ.Count<7)return;string ĥ=Ƶ.ǐ[
0];string ϗ=Ƶ.ǐ[1];string Ѭ=Ƶ.ǐ[2];string ע=Ƶ.ǐ[3];float Ü;float.TryParse(Ƶ.ǐ[4],out Ü);string İ=Ƶ.ǐ[5];string Ҏ=Ƶ.ǐ[6];
var ʵ=ץ.Ԙ.հ(ĥ,ƽ);Ϗ צ;ק(ϗ,out צ);if(צ==Ϗ.ϧ&&ʵ.Ւ.Length>0&&ע.Length>0&&ע.IndexOf(ʵ.Ւ,O.Á)<0){ʵ.Ɔ=Ϗ.ϕ;ʵ.Օ=Օ.ը;ʵ.Ք=ץ.ÿ.đ<ȭ>().ʯ
;ץ.ā.Č("Fleet",$"{ʵ.ʶ} wrong dock: expected {ʵ.Ւ}, got {ע}");ץ.д.Ʒ(ƽ,Ƌ.Ǌ,ĥ,"wrong_dock");return;}float â=-1f;float ձ=-1f;
bool ղ=false;if(Ƶ.ǐ.Count>=10){float.TryParse(Ƶ.ǐ[7],out â);float.TryParse(Ƶ.ǐ[8],out ձ);ղ=Ƶ.ǐ[9]=="1";}if(Ƶ.ǐ.Count>=13){
double ר,ש,ת;if(double.TryParse(Ƶ.ǐ[10],out ר)&&double.TryParse(Ƶ.ǐ[11],out ש)&&double.TryParse(Ƶ.ǐ[12],out ת)){ʵ.Ֆ=new
Vector3D(ר,ש,ת);ʵ.ՙ=true;}}ץ.Ԙ.մ(ĥ,צ,Ѭ,Ҏ,Ü,â,ձ,ղ);ʵ.Ց=ע;ʵ.ǣ=İ;}static void ק(string ǿ,out Ϗ ҍ){switch(ǿ){case"DockedHome":ҍ=Ϗ.ϐ;
return;case"TravelingOut":ҍ=Ϗ.ϑ;return;case"DockedStation":case"Loading":ҍ=Ϗ.ϧ;return;case"WaitingAtStation":ҍ=Ϗ.ϔ;return;case
"TravelingHome":ҍ=Ϗ.ϓ;return;case"Idle":ҍ=Ϗ.Ϩ;return;case"ManualHome":ҍ=Ϗ.ϖ;return;case"Booted":ҍ=Ϗ.ϐ;return;case"Lost":ҍ=Ϗ.ϕ;return;
default:ҍ=Ϗ.ե;return;}}}class ւ:Ʃ{readonly Ŋ ץ;public ւ(Ŋ ŋ){ץ=ŋ;}public void Ǆ(Ǎ Ƶ,long ƽ){if(Ƶ.ǐ.Count<3)return;string ç=Ƶ.ǐ[
0];float Ъ;float.TryParse(Ƶ.ǐ[1],out Ъ);int װ;int.TryParse(Ƶ.ǐ[2],out װ);var ǿ=ץ.ԙ.հ(ç);ǿ.ˀ=ץ.ÿ.đ<ȭ>().ʯ;ǿ.έ=true;if(Ъ<0)
{ǿ.ˁ=true;return;}ǿ.ˁ=false;ǿ.ʿ=Ъ;ǿ.Ը.Clear();int Â=3;for(int É=0;É<װ&&Â+1<Ƶ.ǐ.Count;É++){string Ș=Ƶ.ǐ[Â++];float І;float
.TryParse(Ƶ.ǐ[Â++],out І);ǿ.Ը[Ș]=І;}ץ.ԙ.ױ(ǿ);ץ.պ.Ր(ǿ);}}}enum ʀ{օ,ʁ,փ}class ɜ{readonly Ā ā;int ײ,ؠ,ء;bool آ=true,أ;public
bool ה{get{return أ&&!آ;}}public bool β{get{return أ;}}public TimeSpan δ{get{double Ѕ=ء/2.0;return TimeSpan.FromSeconds(Ѕ);}
}public ɜ(Ā ă){ā=ă;}public void ɞ(int ؤ,int إ){ײ=ؤ;ؠ=إ;أ=ؤ>0&&إ>0;if(أ&&ء<=0){آ=true;ء=ײ*120;}}public bool ɴ(){if(!أ)
return false;ء--;if(ء>0)return false;آ=!آ;ء=آ?ײ*120:ؠ*120;ā.Ē("Sched",آ?"Auto-RUN phase":"Auto-PAUSE phase");return true;}
public string γ{get{return آ?"RUN":"PAUSE";}}}class ɒ{readonly Dictionary<string,ԕ>ԙ=new Dictionary<string,ԕ>(O.P);readonly Ā
ā;public ɒ(Ā ă){ā=ă;}public IReadOnlyDictionary<string,ԕ>ö{get{return ԙ;}}public ԕ հ(string ç){ԕ ǿ;if(!ԙ.TryGetValue(ç,
out ǿ)){ǿ=new ԕ(ç);ԙ[ç]=ǿ;ā.Ē("Stations",$"New station: {ç}");}return ǿ;}public bool Ս(string ç,out ԕ ǿ){return ԙ.
TryGetValue(ç,out ǿ);}public string Վ(){if(ԙ.Count==0)return"(none)";var Ę=new System.Text.StringBuilder();foreach(var Ȃ in ԙ){if(Ę
.Length>0)Ę.Append(", ");Ę.Append(Ȃ.Key);}return Ę.ToString();}public void ױ(ԕ ئ){ئ.έ=true;ئ.Լ=true;}public void ל(int ا,
int ب){foreach(var Ȃ in ԙ){var ǿ=Ȃ.Value;if(!ǿ.έ)continue;if(ǿ.ʾ&&ǿ.ʿ>=ا){ǿ.ʾ=false;ā.Ē("Stations",
$"{ǿ.Ć} CLOSED ({ǿ.ʿ:F0}% >= {ا}%)");}else if(!ǿ.ʾ&&ǿ.ʿ<=ب){ǿ.ʾ=true;ā.Ē("Stations",$"{ǿ.Ć} REOPENED ({ǿ.ʿ:F0}% <= {ب}%)");}}}}class ԕ{public string Ć;
public bool ʾ=true,έ,ˁ,Լ=true;public long ˀ;public float ʿ,Խ;public Dictionary<string,float>Ը=new Dictionary<string,float>();
public ԕ(string ç){Ć=ç;}}static class Ծ{public static float Կ(ԕ ǿ,string[]ة,int ا){if(!ǿ.ʾ)return-1f;if(!ǿ.έ)return-1f;float ت
=ǿ.ʿ;float ث=0f;for(int É=0;É<ة.Length;É++){float І;if(ǿ.Ը.TryGetValue(ة[É],out І)&&І>0){ث=(ة.Length-É)*10f;break;}}
return ت+ث;}}class Ҧ{IReadOnlyList<IMyInventory>ج=null;int ح,خ,د;readonly List<MyInventoryItem>ذ=new List<MyInventoryItem>();
public void ҳ(IReadOnlyList<IMyInventory>ر){ج=ر;ح=0;خ=0;د=0;}public bool Ҵ{get{return ج==null||ح>=ج.Count;}}public int ҵ(int ز
,Func<MyInventoryItem,IMyInventory,bool>س){int ش=0;while(ش<ز&&!Ҵ){var ŧ=ج[ح];ذ.Clear();ŧ.GetItems(ذ);int Â=خ-د;while(Â<ذ.
Count&&ش<ز){if(س(ذ[Â],ŧ))د++;خ++;Â++;ش++;}if(Â>=ذ.Count){ح++;خ=0;د=0;}}return ش;}}static class Ū{public static readonly
string[]ǻ={"Iron","Nickel","Cobalt","Silicon","Gold","Silver","Platinum","Uranium","Magnesium","Stone","Ice"};static readonly
Dictionary<string,string>ص=new Dictionary<string,string>{{"Iron","Iron Ore"},{"Nickel","Nickel Ore"},{"Cobalt","Cobalt Ore"},{
"Silicon","Silicon Ore"},{"Gold","Gold Ore"},{"Silver","Silver Ore"},{"Platinum","Platinum Ore"},{"Uranium","Uranium Ore"},{
"Magnesium","Magnesium Ore"},{"Stone","Stone"},{"Ice","Ice"},{"Scrap","Scrap Metal"}};public static bool ū(MyItemType Ô){return Ô.
TypeId.EndsWith("_Ore",O.Ȝ);}public static string ʶ(MyItemType Ô){string ض=Ô.SubtypeId;string ç;return ص.TryGetValue(ض,out ç)?
ç:ض;}public static string ʶ(string ط){string ç;return ص.TryGetValue(ط,out ç)?ç:ط;}}class ɂ{readonly Program A;readonly Ā
ā;public const int ظ=4;public int ع{get;private set;}public ɂ(Program V,Ā ă){A=V;ā=ă;}public string ˆ(þ Ă){var ʲ=new ʱ();
ʲ.غ(ظ);ʲ.ػ(Ă.Ď);Ă.ˆ(ʲ);return ʲ.ؼ();}public bool ȃ(string ؽ,þ Ă){var ؿ=new ؾ(ā);if(!ؿ.غ(ؽ)){ā.Ē("Persist",
"No valid storage, starting fresh");return false;}ع=ؿ.ع;var ʇ=Ă.đ<ȭ>();long ـ=ʇ!=null?ʇ.ʯ:0;string Ǯ;while((Ǯ=ؿ.ف())!=null){if(ؿ.ق(Ǯ))break;if(Ǯ.Length<2
||Ǯ[1]!='|')continue;char ك=Ǯ[0];string À=Ǯ.Substring(2);switch(ك){case'D':ل(À,Ă,ـ);break;case'T':م(À,Ă);break;case'H':if(
À=="Running"){var ŋ=Ă.đ<Ŋ>();if(ŋ!=null){ŋ.ƅ();ā.Ē("Persist","Hub was Running — fleet resumed");}}break;}}ā.Ē("Persist",
$"Loaded v{ع} storage");return true;}void ل(string À,þ Ă,long ـ){string[]ͽ=À.Split('|');if(ͽ.Length<6)return;var Ő=Ă.đ<ŏ>();if(Ő==null)return;
string ĥ=ͽ[0];if(ĥ.Length!=8)return;long դ=0;if(ͽ.Length>=10)long.TryParse(ͽ[9],out դ);var ʵ=Ő.հ(ĥ,դ);ʵ.ʶ=ͽ[1];ʵ.ʷ=ͽ.Length>=4
?ͽ[3]:"";ʵ.ʸ=ͽ.Length>=5?ͽ[4]:"IDLE";ʵ.Փ=ـ;string ϗ=ͽ.Length>=3?ͽ[2]:"";if(ϗ=="Lost"){ʵ.Ɔ=Ϗ.ϕ;ʵ.ʸ="LOST";}else if(ϗ==
"HOME")ʵ.Ɔ=Ϗ.ϖ;int ن;if(ͽ.Length>=6&&int.TryParse(ͽ[5],out ن))ʵ.ʹ=ن;float ه;if(ͽ.Length>=7&&float.TryParse(ͽ[6],O.Ȟ,O.ȝ,out ه)
)ʵ.ʺ=ه;long Қ;if(ͽ.Length>=8&&long.TryParse(ͽ[7],out Қ))ʵ.ʻ=Қ;long و;if(ͽ.Length>=9&&long.TryParse(ͽ[8],out و))ʵ.ʼ=و;}
void م(string À,þ Ă){string[]ͽ=À.Split('|');if(ͽ.Length<4)return;var ɓ=Ă.đ<ɒ>();if(ɓ==null)return;string ç=ͽ[0];var ǿ=ɓ.հ(ç)
;ǿ.ʾ=ͽ[1]=="1";float Ъ;float.TryParse(ͽ[2],O.Ȟ,O.ȝ,out Ъ);ǿ.ʿ=Ъ;if(ͽ.Length>=5)ǿ.ˁ=ͽ[4]=="1";}}class ؾ{readonly Ā ā;
string[]ى;int ي;public int ع{get;private set;}public ؾ(Ā ă){ā=ă;}public bool غ(string ؽ){if(string.IsNullOrEmpty(ؽ))return
false;ى=ؽ.Split('\n');ي=0;string ٮ=ف();if(ٮ==null||!ٮ.StartsWith("V|"))return false;int ٯ;if(!int.TryParse(ٮ.Substring(2).
Trim(),out ٯ))return false;ع=ٯ;if(ٯ>ɂ.ظ){ā.Č("Persist",$"Future schema v{ٯ}, discarding");return false;}return true;}public
string ف(){while(ي<ى.Length){string Ǯ=ى[ي++].TrimEnd('\r');if(Ǯ.Length>0)return Ǯ;}return null;}public bool ق(string Ǯ){return
Ǯ!=null&&Ǯ.StartsWith("E|");}}class ʱ{readonly StringBuilder ٱ=new StringBuilder(1024);public void غ(int ٲ){ٱ.Clear();ٱ.
Append("V|");ٱ.AppendLine(ٲ.ToString());}public void ػ(Ď Ȳ){ٱ.Append("M|");ٱ.AppendLine(Ȳ.ToString());}public void ʳ(char ك,
string À){ٱ.Append(ك);ٱ.Append('|');ٱ.AppendLine(À);}public void ٳ(){ٱ.AppendLine("E|EOF");}public string ؼ(){ٳ();return ٱ.
ToString();}}ȫ ٴ;þ ÿ;Ā ٵ;Ⱥ ٶ;Ą ٷ;bool ٸ,ٹ;int ٺ;public
 Program
(){ٵ=new Ā(this);ٴ=new ȫ(this,ٵ);ٻ();}void ٻ(){try{ÿ=ٴ.ɍ();ٶ=ÿ.đ<Ⱥ>();ٷ=ÿ.đ<Ą>();ٸ=false;}catch(Exception Գ){ٵ.ǁ("Boot",Գ
.Message);ٸ=false;Runtime.UpdateFrequency=UpdateFrequency.Update10;Echo("BOOT FAILED: "+Գ.Message);}}public void Ĭ(bool ټ
=true){ٸ=true;ٹ=ټ;}public void
 Save
(){if(ÿ!=null){var Ƀ=ÿ.đ<ɂ>();if(Ƀ!=null)Storage=Ƀ.ˆ(ÿ);}}public void
 Main
(string Ĉ,UpdateType ٽ){if(ٸ){ٵ.Ē("Core","Rebooting...");if(ٹ)Save();ٻ();ٵ.پ();return;}if(ÿ==null){ٵ.پ();return;}if(!
string.IsNullOrEmpty(Ĉ)&&ٷ!=null)ٷ.Ĕ(Ĉ);try{ÿ.ʯ(ٽ);ٺ=0;}catch(Exception Գ){ٺ++;ٵ.ǁ("Tick",Գ.Message);var ė=ÿ.đ<W>();if(ė!=null
)ė.Æ();if(ٺ>=3){ٺ=0;ٵ.Č("Core","3 tick failures — rebooting");Ĭ();}}if(ٶ!=null)ٶ.ٿ();ٵ.پ();}class ɫ{readonly W ˉ;readonly
Dictionary<string,float>ڀ=new Dictionary<string,float>();float ځ;readonly List<MyInventoryItem>ذ=new List<MyInventoryItem>();
public IReadOnlyDictionary<string,float>В{get{return ڀ;}}public float ʿ{get{return ځ;}}public ɫ(W ė){ˉ=ė;}public void ɭ(){ڀ.
Clear();long ӓ=0;long Ӕ=0;var Ɯ=ˉ.a();for(int É=0;É<Ɯ.Count;É++)ڂ((IMyTerminalBlock)Ɯ[É],ref ӓ,ref Ӕ);var Ü=ˉ.b();for(int É=0
;É<Ü.Count;É++)ڂ((IMyTerminalBlock)Ü[É],ref ӓ,ref Ӕ);var ġ=ˉ.c();for(int É=0;É<ġ.Count;É++)ڂ((IMyTerminalBlock)ġ[É],ref ӓ
,ref Ӕ);ځ=Ӕ>0?(float)ӓ/Ӕ*100f:0f;}void ڂ(IMyTerminalBlock Ø,ref long ӓ,ref long Ӕ){if(Ø==null)return;for(int ŧ=0;ŧ<Ø.
InventoryCount;ŧ++){var ӕ=Ø.GetInventory(ŧ);ӓ+=ӕ.CurrentVolume.RawValue;Ӕ+=ӕ.MaxVolume.RawValue;ذ.Clear();ӕ.GetItems(ذ);for(int ũ=0;ũ<
ذ.Count;ũ++){var Ҷ=ذ[ũ];if(!Ū.ū(Ҷ.Type))continue;string Ș=Ҷ.Type.SubtypeId;float ȴ;ڀ.TryGetValue(Ș,out ȴ);ڀ[Ș]=ȴ+(float)Ҷ
.Amount;}}}}class ɻ{readonly Ā ā;readonly W ˉ;string ڃ;readonly ɫ ڄ;readonly څ چ;bool أ=true;readonly Dictionary<long,int
>ռ=new Dictionary<long,int>();public string Ǥ{get{return ڃ;}}public ɫ Ћ{get{return ڄ;}}public W ʏ{get{return ˉ;}}public
bool β{get{return أ;}}public ɻ(Program V,Ā ă,W ė,ƈ Ɖ,ī ڇ,ț ĭ){ā=ă;ˉ=ė;أ=ڇ.Ƿ(ę.į,"Enabled",true);ڃ=!string.IsNullOrEmpty(ĭ.Ǥ)
?ĭ.Ǥ:ڈ(ė);ڄ=new ɫ(ė);چ=new څ(ڄ,Ɖ,ڃ);ā.Ē("Station",$"Name: {ڃ}");}string ڈ(W ė){var ġ=ė.c();for(int É=0;É<ġ.Count;É++){
string ç=ġ[É].CustomName;int ĺ=ç.IndexOf("[SAM Name=");if(ĺ>=0){int î=ĺ+10;int Ã=ç.IndexOf(']',î);if(Ã>î)return ç.Substring(î,
Ã-î);}}var Ɯ=ė.a();long p=Ɯ.Count>0?((IMyTerminalBlock)Ɯ[0]).CubeGrid.EntityId:0;return"Station-"+ģ.Ĥ(p);}int ډ;public
void ɷ(){ډ++;if(ډ>=3){ډ=0;چ.ɴ(أ);}ד();}void ד(){var ġ=ˉ.c();for(int É=0;É<ġ.Count;É++){var Ť=ġ[É];if(Ť.Closed||Ť.Status!=
Sandbox.ModAPI.Ingame.MyShipConnectorStatus.Connectable){ռ.Remove(Ť.EntityId);continue;}int Ô;ռ.TryGetValue(Ť.EntityId,out Ô);Ô
++;if(Ô>30){Ť.Connect();ā.Ē("Station",$"Auto-locked dead drone: {Ť.CustomName}");Ô=0;}ռ[Ť.EntityId]=Ô;}}}class څ{readonly
ɫ ڄ;readonly ƈ д;string ڃ;public څ(ɫ ڊ,ƈ Ɖ,string Ɠ){ڄ=ڊ;д=Ɖ;ڃ=Ɠ;}public void ɴ(bool Д){if(!Д){д.Ɗ(Ƌ.Ǉ,ڃ,-1f,0);return;}ڄ
.ɭ();var Ȗ=ڄ.В;int װ=Ȗ.Count;var Ċ=new object[3+װ*2];Ċ[0]=ڃ;Ċ[1]=ڄ.ʿ.ToString("F1");Ċ[2]=װ;int Â=3;foreach(var Ȃ in Ȗ){Ċ[
Â++]=Ȃ.Key;Ċ[Â++]=Ȃ.Value.ToString("F0");}д.Ɗ(Ƌ.Ǉ,Ċ);}}static class Đ{public static string đ(з ǿ){switch(ǿ){case з.и:
return"Booted";case з.ʮ:return"Idle";case з.ы:return"DockedHome";case з.ѓ:return"TravelingOut";case з.є:return"DockedStation";
case з.і:return"Loading";case з.ϔ:return"WaitingAtStation";case з.э:return"TravelingHome";case з.ϕ:return"Lost";case з.ь:
return"ManualHome";default:return"Unknown";}}public static string đ(Ϗ ǿ){switch(ǿ){case Ϗ.ե:return"Unknown";case Ϗ.ϐ:return
"DockedAtHub";case Ϗ.ϒ:return"Departing";case Ϗ.ϑ:return"EnRoute";case Ϗ.ϧ:return"AtStation";case Ϗ.ϔ:return"Waiting";case Ϗ.ϓ:return
"Returning";case Ϗ.Ϩ:return"Parked";case Ϗ.ϖ:return"HOME";case Ϗ.ϕ:return"Lost";default:return"Unknown";}}public static string đ(ʀ
ǿ){switch(ǿ){case ʀ.օ:return"Running";case ʀ.ʁ:return"Paused";case ʀ.փ:return"Recalling";default:return"Unknown";}}public
static string đ(Ď ʨ){switch(ʨ){case Ď.Ņ:return"Hub";case Ď.Ɛ:return"Station";case Ď.ĕ:return"Drone";default:return
"Unconfigured";}}public static string đ(ӡ Ÿ){switch(Ÿ){case ӡ.ӭ:return"DroneLost";case ӡ.ɱ:return"StorageFull";default:return"Unknown"
;}}public static string đ(ť ʨ){switch(ʨ){case ť.Ț:return"Dedicated";case ť.Ŧ:return"Single";case ť.Ů:return"Group";
default:return"Dedicated";}}public static string đ(ǥ ʨ){switch(ʨ){case ǥ.Ǻ:return"RoundRobin";case ǥ.ǻ:return"Priority";default
:return"RoundRobin";}}}static class ε{public static int ζ(StringBuilder Ę){unchecked{int ǫ=(int)2166136261;for(int É=0;É<
Ę.Length;É++){ǫ^=Ę[É];ǫ*=16777619;}return ǫ;}}}static class ģ{static readonly char[]ڋ="0123456789ABCDEF".ToCharArray();
public static string Ĥ(long p){char[]ڌ=new char[8];for(int É=0;É<8;É++)ڌ[É]=ڋ[(int)(p>>((7-É)*4))&0xF];return new string(ڌ);}}
class Ā{readonly Program A;readonly StringBuilder ڍ=new StringBuilder(512);readonly string[]ڎ;int ڏ,ڐ;bool ڑ;public Ā(Program
V,int ڒ=20){A=V;ڎ=new string[ڒ];}public bool ȵ{get{return ڑ;}set{ڑ=value;}}public void Ē(string ړ,string Ƶ){ڔ(
$"[{ړ}] {Ƶ}");}public void Č(string ړ,string Ƶ){ڔ($"[{ړ}] WARN: {Ƶ}");}public void ǁ(string ړ,string Ƶ){ڔ($"[{ړ}] ERR: {Ƶ}");}void ڔ
(string ڕ){ڎ[ڏ]=ڕ;ڏ=(ڏ+1)%ڎ.Length;if(ڐ<ڎ.Length)ڐ++;}public void Ϝ(StringBuilder Ę){int î=ڐ<ڎ.Length?0:ڏ;for(int É=0;É<ڐ
;É++){int Â=(î+É)%ڎ.Length;Ę.AppendLine(ڎ[Â]);}}public void پ(){ڍ.Clear();Ϝ(ڍ);A.Echo(ڍ.ToString());}}struct Ǒ<ʦ>{public
readonly bool ǀ;public readonly ʦ ǂ;public readonly string ǁ;Ǒ(bool ږ,ʦ Ǐ,string ڗ){ǀ=ږ;ǂ=Ǐ;ǁ=ڗ;}public static Ǒ<ʦ>Ǘ(ʦ Ǐ){return
new Ǒ<ʦ>(true,Ǐ,null);}public static Ǒ<ʦ>ǒ(string ڗ){return new Ǒ<ʦ>(false,default(ʦ),ڗ);}}class ȶ{readonly Stack<
StringBuilder>ژ=new Stack<StringBuilder>();public StringBuilder Ϊ(){if(ژ.Count>0){var Ę=ژ.Pop();Ę.Clear();return Ę;}return new
StringBuilder(256);}public void η(StringBuilder Ę){if(Ę!=null&&Ę.Capacity<=4096&&ژ.Count<16)ژ.Push(Ę);}}class Ⱥ{readonly Program A;
readonly double[]ڙ,ښ;int ڛ,ڜ,ڝ;double ڞ,ڟ,ڠ;public Ⱥ(Program V,int ڡ=1000){A=V;ڙ=new double[ڡ];ښ=new double[ڡ];}public void ٿ(){
double ڢ=A.Runtime.LastRunTimeMs;if(ڜ>=ڙ.Length)ڞ-=ڙ[ڛ];ڙ[ڛ]=ڢ;ڞ+=ڢ;ڛ=(ڛ+1)%ڙ.Length;if(ڜ<ڙ.Length)ڜ++;double ڣ=(double)A.
Runtime.CurrentInstructionCount/A.Runtime.MaxInstructionCount*100.0;if(ڣ>ڠ)ڠ=ڣ;if(++ڝ>=100){ڝ=0;ڤ();}}void ڤ(){if(ڜ==0)return;
int ڥ=(int)(ڜ*0.99);Array.Copy(ڙ,ښ,ڜ);Array.Sort(ښ,0,ڜ);ڟ=ښ[Math.Min(ڥ,ڜ-1)];}public double ϙ{get{return ڜ>0?ڞ/ڜ:0;}}public
double Ϛ{get{return ڟ;}}public double ϛ{get{return ڠ;}}}static class Ј{public const bool Ǧ=false;}
