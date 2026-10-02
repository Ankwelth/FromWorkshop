const string A="4.4.4",B="Monospace",C="LargePrototechAssembler";const StringComparison D=StringComparison.
OrdinalIgnoreCase;static readonly StringComparer E=StringComparer.OrdinalIgnoreCase;const SpriteType F=SpriteType.TEXT,G=SpriteType.
TEXTURE;const TextAlignment H=TextAlignment.LEFT,I=TextAlignment.CENTER,J=TextAlignment.RIGHT;const MyAssemblerMode K=
MyAssemblerMode.Assembly;static readonly Dictionary<string,string>L=new Dictionary<string,string>(E){{"Iron","Iron"},{"Nickel","Nickel"
},{"Cobalt","Cobalt"},{"Magnesium","Magnesium"},{"Silicon","Silicon"},{"Silver","Silver"},{"Gold","Gold"},{"Platinum",
"Platinum"},{"Uranium","Uranium"},{"Stone","Gravel"},{"Scrap","Iron"},{"Ice","Ice"},},M=new Dictionary<string,string>(E){{
"SteelPlate","SteelPlate"},{"Construction","ConstructionComponent"},{"MetalGrid","MetalGrid"},{"InteriorPlate","InteriorPlate"},{
"Girder","GirderComponent"},{"SmallTube","SmallTube"},{"LargeTube","LargeTube"},{"Motor","MotorComponent"},{"Display","Display"}
,{"BulletproofGlass","BulletproofGlass"},{"Computer","ComputerComponent"},{"Reactor","ReactorComponent"},{"Thrust",
"ThrustComponent"},{"GravityGenerator","GravityGeneratorComponent"},{"Medical","MedicalComponent"},{"RadioCommunication",
"RadioCommunicationComponent"},{"Detector","DetectorComponent"},{"Explosives","ExplosivesComponent"},{"SolarCell","SolarCell"},{"PowerCell",
"PowerCell"},{"Superconductor","Superconductor"},},N=new Dictionary<string,string>(E){{"NATO_25x184mm",
"Position0080_NATO_25x184mmMagazine"},{"Missile200mm","Position0100_Missile200mm"},{"LargeCalibreAmmo","Position0120_LargeCalibreAmmo"},{"MediumCalibreAmmo"
,"Position0110_MediumCalibreAmmo"},{"AutocannonClip","Position0090_AutocannonClip"},{"LargeRailgunAmmo",
"Position0140_LargeRailgunAmmo"},{"SmallRailgunAmmo","Position0130_SmallRailgunAmmo"},{"SemiAutoPistolMagazine","Position0010_SemiAutoPistolMagazine"},
{"FullAutoPistolMagazine","Position0020_FullAutoPistolMagazine"},{"ElitePistolMagazine",
"Position0030_ElitePistolMagazine"},{"AutomaticRifleGun_Mag_20rd","Position0040_AutomaticRifleGun_Mag_20rd"},{"RapidFireAutomaticRifleGun_Mag_50rd",
"Position0050_RapidFireAutomaticRifleGun_Mag_50rd"},{"PreciseAutomaticRifleGun_Mag_5rd","Position0060_PreciseAutomaticRifleGun_Mag_5rd"},{
"UltimateAutomaticRifleGun_Mag_30rd","Position0070_UltimateAutomaticRifleGun_Mag_30rd"},{"FlareClip","Position0051_FlareGunMagazine"},},O=new Dictionary<
string,string>(E){{"AngleGrinderItem","Position0010_AngleGrinder"},{"AngleGrinder2Item","Position0020_AngleGrinder2"},{
"AngleGrinder3Item","Position0030_AngleGrinder3"},{"AngleGrinder4Item","Position0040_AngleGrinder4"},{"HandDrillItem",
"Position0050_HandDrill"},{"HandDrill2Item","Position0060_HandDrill2"},{"HandDrill3Item","Position0070_HandDrill3"},{"HandDrill4Item",
"Position0080_HandDrill4"},{"WelderItem","Position0090_Welder"},{"Welder2Item","Position0100_Welder2"},{"Welder3Item","Position0110_Welder3"},{
"Welder4Item","Position0120_Welder4"},{"OxygenBottle","Position0010_OxygenBottle"},{"HydrogenBottle","Position0020_HydrogenBottle"},{
"Medkit","Position0021_Medkit"},{"Powerkit","Position0022_Powerkit"},},P=new Dictionary<string,string>(E){{"PrototechPanel",
"PrototechPanel"},{"PrototechCapacitor","PrototechCapacitor"},{"PrototechPropulsionUnit","PrototechPropulsionUnit"},{
"PrototechMachinery","PrototechMachinery"},{"PrototechCircuitry","PrototechCircuitry"},{"PrototechCoolingUnit","PrototechCoolingUnit"},{
"PrototechFrame",""},};static readonly Dictionary<string,string>Q=new Dictionary<string,string>(E){{"Thrust","Thruster"},{
"GravityGenerator","Gravity Gen."},{"RadioCommunication","Radio Comm."},{"NATO_25x184mm","Gatling Ammo"},{"Missile200mm","Missile"},{
"LargeCalibreAmmo","Artillery Shell"},{"MediumCalibreAmmo","Assault Cannon"},{"AutocannonClip","Autocannon Mag"},{"LargeRailgunAmmo",
"Lg Railgun Sabot"},{"SmallRailgunAmmo","Sm Railgun Sabot"},{"SemiAutoPistolMagazine","Pistol S-10"},{"FullAutoPistolMagazine",
"Pistol S-10A"},{"ElitePistolMagazine","Pistol S-10E"},{"AutomaticRifleGun_Mag_20rd","Rifle MR-20"},{
"RapidFireAutomaticRifleGun_Mag_50rd","Rifle MR-50A"},{"PreciseAutomaticRifleGun_Mag_5rd","Rifle MR-8P"},{"UltimateAutomaticRifleGun_Mag_30rd","Rifle MR-30E"
},{"FlareClip","Flare Clip"},{"AngleGrinder2Item","Enh. Grinder"},{"AngleGrinder3Item","Pro. Grinder"},{
"AngleGrinder4Item","Elite Grinder"},{"HandDrill2Item","Enh. Drill"},{"HandDrill3Item","Pro. Drill"},{"HandDrill4Item","Elite Drill"},{
"Welder2Item","Enh. Welder"},{"Welder3Item","Pro. Welder"},{"Welder4Item","Elite Welder"},{"PrototechPanel","Proto Panel"},{
"PrototechCapacitor","Proto Cap."},{"PrototechPropulsionUnit","Proto Prop."},{"PrototechMachinery","Proto Mach."},{"PrototechCircuitry",
"Proto Circ."},{"PrototechCoolingUnit","Proto Cool."},{"PrototechFrame","Proto Frame"},};static string V(string R){if(R.EndsWith(
"Item",StringComparison.Ordinal))R=R.Substring(0,R.Length-4);var S=new char[R.Length*2];int T=0;for(int U=0;U<R.Length;U++){if
(U>0&&char.IsUpper(R[U])&&char.IsLower(R[U-1]))S[T++]=' ';S[T++]=R[U];}return new string(S,0,T);}struct Y{public string W
;public double X;}static readonly Dictionary<string,string>Z=new Dictionary<string,string>(E){{"Fe","Iron"},{"Ni",
"Nickel"},{"Co","Cobalt"},{"Si","Silicon"},{"Mg","Magnesium"},{"Ag","Silver"},{"Au","Gold"},{"Pt","Platinum"},{"U","Uranium"},{
"Grav","Gravel"},{"PS","PrototechScrap"},};static Y[]f(string R){var a=R.Split(',');var b=new Y[a.Length];for(int U=0;U<a.
Length;U++){var c=a[U].Split(':');string d=c[0].Trim();string e;if(Z.TryGetValue(d,out e))d=e;b[U]=new Y{W=d,X=double.Parse(c[
1],System.Globalization.CultureInfo.InvariantCulture)};}return b;}class Í{public Dictionary<string,double>g;public
Dictionary<string,string>h,i;public Dictionary<string,int>j,k,l,m;public HashSet<string>n,o;public int p,q,r;public double s,t,u,v
;public bool w,x,y,z,ª,µ,º,À;public string Á,Â,Ã,Ä,Å,Æ,Ç,È,É,Ê,Ë;public Dictionary<string,string[]>Ì;public Í(){g=new
Dictionary<string,double>(E);h=new Dictionary<string,string>(E);i=new Dictionary<string,string>(E);j=new Dictionary<string,int>(E)
;k=new Dictionary<string,int>(E);l=new Dictionary<string,int>(E);m=new Dictionary<string,int>(E);n=new HashSet<string>(E)
;p=600;s=2000.0;q=5;t=3.0;u=1.0;w=true;x=false;y=false;v=0.99;z=true;ª=false;µ=true;º=true;r=2000;Á="[PMO]";Â="[PMC]";Ã=
"[PMA]";Ä="[PMG]";Å="[PMX]";Æ="[PMLO]";Ç="[PMD]";È="[PM]";É="[PMW]";Ê="[PMI]";Ë="";À=false;n.Add("Silver");n.Add("Gold");n.Add(
"Platinum");n.Add("Uranium");o=new HashSet<string>(E);Ì=new Dictionary<string,string[]>(E){{"[PM]",new[]{"Flow","Deficit","Ore",
"Components","Ammo","Processing","Ingots","Projectors","Gear","Prototech","Storage","ConnectedGrids"}}};}public static Í ç(string Î)
{var Ï=new Í();if(string.IsNullOrWhiteSpace(Î))return Ï;var Ð=new MyIni();Ð.TryParse(Î);const string Ñ=
"ProductionManagementv3";double Ò;int U;foreach(var T in new[]{"Iron","Nickel","Cobalt","Magnesium","Silicon","Silver","Gold","Platinum",
"Uranium","Ice"})if(Ð.Get(Ñ,T+"IngotQuota").TryGetDouble(out Ò))Ï.g[T]=Ò;if(Ð.Get(Ñ,"GravelQuota").TryGetDouble(out Ò))Ï.g[
"Gravel"]=Ò;foreach(var Ó in M.Keys)if(Ð.Get(Ñ,Ó+"Quota").TryGetInt32(out U))Ï.j[Ó]=U;foreach(var Ó in N.Keys)if(Ð.Get(Ñ,Ó+
"Quota").TryGetInt32(out U))Ï.k[Ó]=U;foreach(var Ó in O.Keys)if(Ð.Get(Ñ,Ó+"Quota").TryGetInt32(out U))Ï.l[Ó]=U;if(Ð.Get(Ñ,
"PrototechScrapQuota").TryGetDouble(out Ò))Ï.g["PrototechScrap"]=Ò;foreach(var Ó in P.Keys)if(Ð.Get(Ñ,Ó+"Quota").TryGetInt32(out U))Ï.m[Ó]=U;
string Ô=Ð.Get(Ñ,"HighValueOres").ToString("Silver,Gold,Platinum,Uranium");Ï.n.Clear();foreach(var Õ in Ô.Split(',')){string Ö
=Õ.Trim();if(Ö.Length>0)Ï.n.Add(Ö);}Ï.p=Ð.Get(Ñ,"RescanInterval").ToInt32(600);Ï.s=Ð.Get(Ñ,"OreBatchKg").ToDouble(2000.0)
;Ï.q=Ð.Get(Ñ,"ProtoIngotBatch").ToInt32(5);Ï.t=Ð.Get(Ñ,"AssemblerEfficiency").ToDouble(3.0);Ï.u=Ð.Get(Ñ,
"ProtoAssemblerEfficiency").ToDouble(1.0);Ï.w=Ð.Get(Ñ,"SpeedRefineriesAllowHV").ToBoolean(true);Ï.x=Ð.Get(Ñ,"ManageRefineryConveyor").ToBoolean(
false);Ï.y=Ð.Get(Ñ,"ManageGasGenerators").ToBoolean(false);Ï.v=Ð.Get(Ñ,"GasFullThreshold").ToDouble(0.99);Ï.z=Ð.Get(Ñ,
"AutoQueueAssemblers").ToBoolean(true);Ï.ª=Ð.Get(Ñ,"AutoDisassemble").ToBoolean(false);Ï.µ=Ð.Get(Ñ,"CountBlockInventories").ToBoolean(true);Ï
.º=Ð.Get(Ñ,"ReclaimDockedBottles").ToBoolean(true);Ï.r=Ð.Get(Ñ,"MaxQueuedPerItem").ToInt32(2000);Ï.Á=Ð.Get(Ñ,
"OreIngotTag").ToString("[PMO]");Ï.Â=Ð.Get(Ñ,"ComponentTag").ToString("[PMC]");Ï.Ã=Ð.Get(Ñ,"AmmoTag").ToString("[PMA]");Ï.Ä=Ð.Get(Ñ,
"GearTag").ToString("[PMG]");Ï.Å=Ð.Get(Ñ,"MixedTag").ToString("[PMX]");Ï.Æ=Ð.Get(Ñ,"LoadOutTag").ToString("[PMLO]");Ï.Ç=Ð.Get(Ñ,
"DeliveryTag").ToString("[PMD]");Ï.È=Ð.Get(Ñ,"StatusTag").ToString("[PM]");Ï.É=Ð.Get(Ñ,"WarningsTag").ToString("[PMW]");Ï.Ê=Ð.Get(Ñ,
"IgnoreTag").ToString("[PMI]");Ï.Ë=Ð.Get("ProjectorManagerPB","Name").ToString("");Ï.À=Ð.Get("CargoIntegration","Enabled").
ToBoolean(false);var Ø=new List<MyIniKey>();Ð.GetKeys("DisplayTags",Ø);if(Ø.Count>0){Ï.Ì.Clear();foreach(var Ó in Ø){string Ù=Ð.
Get(Ó).ToString("").Trim();if(Ù.Length>0){var Ú=Ù.Split(',');var Û=new List<string>();foreach(var a in Ú){string Ö=a.Trim()
;if(Ö.Length>0)Û.Add(Ö);}if(Û.Count>0)Ï.Ì["["+Ó.Name+"]"]=Û.ToArray();}}}var Ü=new List<MyIniKey>();Ð.GetKeys(
"HideFromDisplay",Ü);foreach(var Ó in Ü)Ï.o.Add(Ó.Name);var Ý=new List<MyIniKey>();Ð.GetKeys("CustomItems",Ý);foreach(var Þ in Ý){string
ß=Þ.Name;string à=Ð.Get(Þ).ToString("0").Trim();int á;string â=ß;int ã=à.IndexOf(',');if(ã>=0){â=à.Substring(0,ã).Trim();
if(!int.TryParse(à.Substring(ã+1).Trim(),out á))á=0;}else{if(!int.TryParse(à,out á))á=0;}Ï.j[ß]=á;Ï.h[ß]=â;}var ä=new List
<MyIniKey>();Ð.GetKeys("CustomOres",ä);foreach(var Þ in ä){string ß=Þ.Name;string à=Ð.Get(Þ).ToString("0").Trim();double
å;string æ=ß;int ã=à.IndexOf(',');if(ã>=0){æ=à.Substring(0,ã).Trim();if(!double.TryParse(à.Substring(ã+1).Trim(),out å))å
=0;}else if(!double.TryParse(à,out å)){æ=à;å=0;}Ï.i[ß]=æ;if(å>0)Ï.g[æ]=å;}return Ï;}public string é(){const string Ñ=
"ProductionManagementv3";var è=new StringBuilder();è.Append("["+Ñ+"]\n");foreach(var T in new[]{"Iron","Nickel","Cobalt","Magnesium","Silicon",
"Silver","Gold","Platinum","Uranium","Ice"})è.Append(T+"IngotQuota=0\n");è.Append("GravelQuota=0\n");è.Append(
"HighValueOres=Silver,Gold,Platinum,Uranium\n");foreach(var Ó in M.Keys)è.Append(Ó+"Quota=0\n");foreach(var Ó in N.Keys)è.Append(Ó+"Quota=0\n");foreach(var Ó in O.
Keys)è.Append(Ó+"Quota=0\n");è.Append("PrototechScrapQuota=0\n");foreach(var Ó in P.Keys)è.Append(Ó+"Quota=0\n");è.Append("AutoQueueAssemblers=true\nAutoDisassemble=false\nCountBlockInventories=true\nReclaimDockedBottles=true\nMaxQueuedPerItem=2000\nRescanInterval=600\nOreBatchKg=1000\nProtoIngotBatch=5\nAssemblerEfficiency=3.0\nProtoAssemblerEfficiency=1.0\nSpeedRefineriesAllowHV=true\nManageRefineryConveyor=false\nManageGasGenerators=false\nGasFullThreshold=0.99\nOreIngotTag=[PMO]\nComponentTag=[PMC]\nAmmoTag=[PMA]\nGearTag=[PMG]\nMixedTag=[PMX]\nLoadOutTag=[PMLO]\nDeliveryTag=[PMD]\nStatusTag=[PM]\nWarningsTag=[PMW]\nIgnoreTag=[PMI]\n\n[ProjectorManagerPB]\n; Name of the Programmable Block running ProjectorManagement.\n; Leave blank to disable. PMV4 will turn it on when a projector is active, off when none are.\nName=\n\n[DisplayTags]\n; TagName=Section1,Section2,... (tag name WITHOUT brackets)\n; Sections: Flow DefSurAmt Deficit Ore Components Ammo Processing Ingots Projectors Gear Storage ConnectedGrids\nPM=Flow,Ore,DefSurAmt,Components,Ammo,Processing,Ingots,Projectors,Gear,Prototech,Storage,ConnectedGrids\n\n[HideFromDisplay]\n; List item subtypes to suppress from all display sections (still managed/queued).\n; Example: Motor=\n\n[CustomItems]\n; ComponentSubtype=Quota  or  ComponentSubtype=BlueprintSubtype,Quota\n\n[CustomOres]\n; Modded ores (auto-added when found). OreSubtype=Quota  or  OreSubtype=IngotSubtype,Quota\n\n[CargoIntegration]\n; Enable IGC cargo handlers for v3 cargo system\nEnabled=false\n"
);return è.ToString();}}class í{public string ê;public string[]ë;public List<IMyTextSurface>ì=new List<IMyTextSurface>();
}class ù{public IMyRefinery î;public IMyInventory ï,ð;public bool ñ,ò;public string ó="",ô="";public double õ=0;public
bool ö=true;public ù(IMyRefinery ø){î=ø;ï=ø.GetInventory(0);ð=ø.GetInventory(1);string T=ø.CustomName;ñ=T.IndexOf("[Y]",D)>=
0;ò=T.IndexOf("[S]",D)>=0;}}class ÿ{public IMyAssembler î;public bool ú,ö;public string û="";public double ü=0;public int
ý=0;public List<MyProductionItem>þ=new List<MyProductionItem>();public ÿ(IMyAssembler ø){î=ø;ú=!ø.CooperativeMode;ö=ø.
IsWorking;}}class Ĉ{public string Ā;public double ā,Ă,ă,Ą,ą,Ć;public int ć;}struct đ{public string ĉ,Ċ;public double ċ,Č,č,Ď;
public bool ď;public bool Đ=>č>0&&Č/č<0.2;}struct ě{public string Ē,ē;public int Ĕ,ĕ,Ė,ė;public bool Ę;public double Ď;public
int ę=>Math.Max(0,Ė-Ĕ-ĕ);public int Ě=>Math.Min(Ĕ-Ė,Ĕ-ė);public bool Đ=>Ė>0&&(double)Ĕ/Ė<0.2;}Í Ĝ;List<ù>ĝ=new List<ù>();
List<ÿ>Ğ=new List<ÿ>(),ğ=new List<ÿ>(),Ġ=new List<ÿ>(),ġ=new List<ÿ>(),Ģ=new List<ÿ>();List<MyProductionItem>ģ=new List<
MyProductionItem>();HashSet<MyItemType>Ĥ=new HashSet<MyItemType>();List<IMyCargoContainer>ĥ=new List<IMyCargoContainer>(),Ħ=new List<
IMyCargoContainer>(),ħ=new List<IMyCargoContainer>(),Ĩ=new List<IMyCargoContainer>(),ĩ=new List<IMyCargoContainer>(),Ī=new List<
IMyCargoContainer>(),ī=new List<IMyCargoContainer>(),Ĭ=new List<IMyCargoContainer>(),ĭ=new List<IMyCargoContainer>();Dictionary<
MyItemType,double>Į=new Dictionary<MyItemType,double>();List<int>į=new List<int>(),İ=new List<int>();List<double>ı=new List<double
>(),Ĳ=new List<double>();HashSet<IMyCargoContainer>ĳ=new HashSet<IMyCargoContainer>();List<IMyShipConnector>Ĵ=new List<
IMyShipConnector>(),ĵ=new List<IMyShipConnector>();List<IMyTerminalBlock>Ķ=new List<IMyTerminalBlock>();List<IMyTerminalBlock>ķ=new List
<IMyTerminalBlock>();List<IMyShipDrill>ĸ=new List<IMyShipDrill>();List<IMyShipWelder>Ĺ=new List<IMyShipWelder>();List<
IMyCockpit>ĺ=new List<IMyCockpit>();bool Ļ=true,ļ=false;int Ľ=0,ľ=0,Ŀ=0,ŀ=0;List<í>Ł=new List<í>();List<IMyTextSurface>ł=new List<
IMyTextSurface>();List<IMyGasTank>Ń=new List<IMyGasTank>();List<IMyGasGenerator>ń=new List<IMyGasGenerator>();List<IMyProjector>Ņ=new
List<IMyProjector>();IMyProgrammableBlock ņ;IMyTextSurface Ň;Dictionary<string,đ>ň=new Dictionary<string,đ>(E);Dictionary<
string,double>ŉ=new Dictionary<string,double>(E),Ŋ=new Dictionary<string,double>(E);Dictionary<string,ě>ŋ=new Dictionary<
string,ě>(E),Ō=new Dictionary<string,ě>(E),ō=new Dictionary<string,ě>(E),Ŏ=new Dictionary<string,ě>(E);List<đ>ŏ=new List<đ>(),
Ő=new List<đ>(),ő=new List<đ>(),Œ=new List<đ>(),œ=new List<đ>();List<ě>Ŕ=new List<ě>(),ŕ=new List<ě>(),Ŗ=new List<ě>(),ŗ=
new List<ě>();HashSet<string>Ř=new HashSet<string>(E),ř=new HashSet<string>(E);List<string>Ś=new List<string>(),ś=new List<
string>();Dictionary<string,Y[]>Ŝ=new Dictionary<string,Y[]>(E);static readonly string ŝ="\n[Recipes]\nBulletproofGlass=Si:15\nComputerComponent=Fe:0.5,Si:0.2\nConstructionComponent=Fe:8\nDetectorComponent=Fe:5,Ni:15\nDisplay=Fe:1,Si:5\nExplosivesComponent=Si:0.5,Mg:2\nGirderComponent=Fe:6\nGravityGeneratorComponent=Fe:600,Co:220,Ag:5,Au:10\nInteriorPlate=Fe:3\nLargeTube=Fe:30\nMedicalComponent=Fe:60,Ni:70,Ag:20\nMetalGrid=Fe:12,Ni:5,Co:3\nMotorComponent=Fe:20,Ni:5\nPowerCell=Fe:10,Si:1,Ni:2\nRadioCommunicationComponent=Fe:8,Si:1\nReactorComponent=Grav:20,Fe:15,Ag:5\nShieldComponent=Co:25,Ag:25,Au:25,Pt:25\nSmallTube=Fe:5\nSolarCell=Ni:3,Si:6\nSteelPlate=Fe:21\nSuperconductor=Fe:10,Au:2\nThrustComponent=Fe:30,Co:10,Au:1,Pt:0.4\nPosition0010_OxygenBottle=Fe:80,Si:10,Ni:30\nPosition0020_HydrogenBottle=Fe:80,Si:10,Ni:30\nPosition0021_Medkit=Fe:4.5,Si:2.75,Ni:1.75\nPosition0022_Powerkit=Fe:6,Si:1.25,Ni:1.75\nPosition0040_Datapad=Fe:1,Si:5,Ni:1\nPosition0050_FlareGun=Fe:1,Si:0.4,Ni:0.2\nPosition0051_FlareGunMagazine=Fe:0.2,Ni:0.05\nPosition0010_AngleGrinder=Fe:6,Si:1,Ni:2\nPosition0020_AngleGrinder2=Fe:6,Si:1,Ni:2,Co:1\nPosition0030_AngleGrinder3=Fe:6,Si:1,Ni:2,Co:1,Ag:1\nPosition0040_AngleGrinder4=Fe:6,Si:1,Ni:2,Co:1,Ag:1,Pt:1\nPosition0050_HandDrill=Fe:6,Si:1,Ni:2\nPosition0060_HandDrill2=Fe:6,Si:1,Ni:2,Co:1\nPosition0070_HandDrill3=Fe:6,Si:1,Ni:2,Co:1,Ag:1\nPosition0080_HandDrill4=Fe:6,Si:1,Ni:2,Co:1,Ag:1,Pt:1\nPosition0090_Welder=Fe:6,Si:1,Ni:2\nPosition0100_Welder2=Fe:6,Si:1,Ni:2,Co:1\nPosition0110_Welder3=Fe:6,Si:1,Ni:2,Co:1,Ag:1\nPosition0120_Welder4=Fe:6,Si:1,Ni:2,Co:1,Ag:1,Pt:1\nPosition0010_SemiAutoPistol=Fe:1,Ni:0.3\nPosition0020_FullAutoPistol=Fe:1,Ni:0.3,Co:0.5\nPosition0030_EliteAutoPistol=Fe:1,Ni:0.3,Ag:1,Pt:0.25\nPosition0040_AutomaticRifle=Fe:3,Si:1,Ni:1,Co:1\nPosition0050_RapidFireAutomaticRifle=Fe:3,Si:1,Ni:3,Co:2\nPosition0060_PreciseAutomaticRifle=Fe:3,Si:1,Ni:1,Co:2,Ag:1\nPosition0070_UltimateAutomaticRifle=Fe:3,Si:1,Ni:3,Co:2,Ag:1,Pt:0.5\nPosition0080_BasicHandHeldLauncher=Fe:20,Si:8,Ni:10,Co:5\nPosition0090_AdvancedHandHeldLauncher=Fe:20,Si:8,Ni:10,Co:5,Pt:0.75\nPosition0010_SemiAutoPistolMagazine=Fe:0.25,Ni:0.05\nPosition0020_FullAutoPistolMagazine=Fe:0.5,Mg:0.1,Ni:0.1\nPosition0030_ElitePistolMagazine=Fe:0.3,Mg:0.1,Ni:0.1\nPosition0040_AutomaticRifleGun_Mag_20rd=Fe:0.8,Mg:0.15,Ni:0.2\nPosition0050_RapidFireAutomaticRifleGun_Mag_50rd=Fe:2,Mg:0.4,Ni:0.5\nPosition0060_PreciseAutomaticRifleGun_Mag_5rd=Fe:0.8,Mg:0.15,Ni:0.2\nPosition0070_UltimateAutomaticRifleGun_Mag_30rd=Fe:1.2,Mg:0.25,Ni:0.4\nPosition0080_NATO_25x184mmMagazine=Fe:40,Mg:3,Ni:5\nPosition0090_AutocannonClip=Fe:25,Mg:2,Ni:3\nPosition0100_Missile200mm=Fe:55,Ni:7,Si:0.2,U:0.1,Pt:0.04,Mg:1.2\nPosition0110_MediumCalibreAmmo=Fe:15,Ni:2,Mg:1.2\nPosition0120_LargeCalibreAmmo=Fe:60,Ni:8,Mg:5,U:0.1\nPosition0130_SmallRailgunAmmo=Fe:4,Ni:0.5,Si:5,U:0.2\nPosition0140_LargeRailgunAmmo=Fe:20,Ni:3,Si:30,U:1\nPrototechPanel=Fe:35,Ni:7,Co:3,Mg:4\nPrototechCapacitor=Fe:12,Si:4,Ag:3,Au:6,PS:1.5\nPrototechPropulsionUnit=Fe:60,Co:24,Au:6,Pt:3,PS:1.25\nPrototechMachinery=Fe:45,Ni:12,Si:7,Au:3,PS:1.15\nPrototechCircuitry=Fe:5,Si:8,Au:2,Pt:1.5,PS:1.75\nPrototechCoolingUnit=Fe:80,Au:12,Pt:3.25,PS:2.5\n"
;Dictionary<string,int>Ş=new Dictionary<string,int>(E);double ş=0,Š=0,š=10000.0;StringBuilder Ţ=new StringBuilder();List<
MyInventoryItem>ţ=new List<MyInventoryItem>();List<MyIniKey>Ť=new List<MyIniKey>();readonly HashSet<string>ť=new HashSet<string>(E),Ŧ=
new HashSet<string>(E);static readonly List<đ>ŧ=new List<đ>();readonly List<IMyRefinery>Ũ=new List<IMyRefinery>();readonly
List<IMyAssembler>ũ=new List<IMyAssembler>();readonly List<IMyCargoContainer>Ū=new List<IMyCargoContainer>(),ū=new List<
IMyCargoContainer>();readonly List<IMyTerminalBlock>Ŭ=new List<IMyTerminalBlock>();readonly List<IMyProgrammableBlock>ŭ=new List<
IMyProgrammableBlock>();readonly List<IMyShipConnector>Ů=new List<IMyShipConnector>();List<Ĉ>ů=new List<Ĉ>();Dictionary<string,Ĉ>Ű=new
Dictionary<string,Ĉ>(E);readonly List<IMyGasTank>ű=new List<IMyGasTank>();List<ě>[]Ų,ų;List<IMyCargoContainer>[]Ŵ;readonly MyIni ŵ
=new MyIni();static readonly Color Ŷ=new Color(90,217,255),ŷ=new Color(255,200,50),Ÿ=new Color(95,210,110),Ź=new Color(
240,195,70),ź=new Color(235,75,80),Ż=new Color(45,62,85),ż=new Color(255,215,80),Ž=new Color(210,220,230);struct Ə{public
string ž,ſ,ƀ;public Color Ɓ,Ƃ;public float ƃ,Ƅ;public bool ƅ,Ɔ,Ƈ,ƈ,Ɖ;public Ə(string Ɗ,Color Ƌ,string ƌ="",string ƍ="",float Ǝ
=-1f){ž=Ɗ;Ɓ=Ƌ;ƀ=ƌ;ſ=ƍ;ƃ=Ǝ;Ƅ=0f;Ƃ=default(Color);ƅ=false;Ɔ=false;Ƈ=false;ƈ=false;Ɖ=false;}}struct ƒ{public string Ɛ,Ƒ;
public Color Ɓ;}ƒ[]Ɠ=new ƒ[5];List<Ə>Ɣ=new List<Ə>();string ƕ="MyObjectBuilder_Ore/",Ɩ="MyObjectBuilder_Ingot/",Ɨ=
"MyObjectBuilder_Component/",Ƙ="",ƙ="",ƚ="",ƛ="",Ɯ="",Ɲ="",ƞ="";const string Ɵ="MyObjectBuilder_Ore",Ơ="MyObjectBuilder_Ingot",ơ=
"MyObjectBuilder_Component",Ƣ="MyObjectBuilder_AmmoMagazine",ƣ="MyObjectBuilder_PhysicalGunObject",Ƥ="MyObjectBuilder_OxygenContainerObject",ƥ=
"MyObjectBuilder_GasContainerObject",Ʀ="MyObjectBuilder_ConsumableItem",Ƨ="MyObjectBuilder_BlueprintDefinition/",ƨ="PMV4_CARGO_CMD",Ʃ="PMV4_CARGO_STATUS",ƪ=
"PMV4_INVENTORY";IMyBroadcastListener ƫ;int Ƭ,ƭ,Ʈ;List<IMyCargoContainer>Ư=new List<IMyCargoContainer>();List<string>ư=new List<string>(
);List<int>Ʊ=new List<int>();StringBuilder Ʋ=new StringBuilder();double Ƴ;public
 Program
(){Ų=new[]{Ŕ,ŕ,Ŗ};ų=new[]{ŗ};Ŵ=new[]{ĥ,Ħ,ħ,Ĩ};Ň=Me.GetSurface(0);Ň.ContentType=ContentType.TEXT_AND_IMAGE;Ň.Font=B;Ň.
FontSize=0.55f;var ƴ=new List<string>();Ň.ClearImagesFromSelection();Ň.AddImageToSelection("ColorfulIcons_Component/SolarCell");
Ň.GetSelectedImages(ƴ);if(ƴ.Contains("ColorfulIcons_Component/SolarCell")){ƕ="ColorfulIcons_Ore/";Ɩ=
"ColorfulIcons_Ingot/";Ɨ="ColorfulIcons_Component/";Ƙ="ColorfulIcons_Refinery/LargeRefinery";ƙ="ColorfulIcons_Assembler/LargeAssembler";ƚ=
"ColorfulIcons_OxygenGenerator/(null)";ƛ="ColorfulIcons_OxygenTank/LargeHydrogenTank";Ɯ="ColorfulIcons_OxygenTank/(null)";Ɲ=
"ColorfulIcons_CargoContainer/LargeBlockLargeContainer";}Ň.ClearImagesFromSelection();if(!Me.CustomData.Contains("[ProductionManagementv3]"))Me.CustomData=new Í().é();Ĝ=Í.ç(Me
.CustomData);if(Ĝ.À){ƫ=IGC.RegisterBroadcastListener(ƨ);ƫ.SetMessageCallback("IGC");}Ƶ();Runtime.UpdateFrequency=
UpdateFrequency.Update10|UpdateFrequency.Update100;}public void
 Save
(){}public void
 Main
(string ƶ,UpdateType Ʒ){bool Ƹ=(Ʒ&UpdateType.Update10)!=0;bool ƹ=(Ʒ&UpdateType.Update100)!=0;bool ƺ=Ļ;Ļ=false;if(!string.
IsNullOrEmpty(ƶ)){string ƻ=ƶ.Trim().ToUpperInvariant();if(ƻ=="RESCAN"||ƻ=="RESET"){ƺ=true;}else if(ƻ=="CLEARQUEUE"){ƺ=true;foreach(
var Ƽ in Ğ)if(Ƽ.ú)Ƽ.î.ClearQueue();foreach(var Ƽ in Ġ)if(Ƽ.ú)Ƽ.î.ClearQueue();}}if(Ĝ.À){if((Ʒ&UpdateType.IGC)!=0||ƶ=="IGC")
ƽ();if(Ƭ!=0)ƾ();}double ƿ=Runtime.TimeSinceLastRun.TotalMilliseconds;ş+=ƿ;Š+=ƿ/1000.0;if(ƺ){ş=0;Ľ=1;ľ=0;}else if(Ľ==0&&ş
>=Ĝ.p*100.0){ş=0;Ľ=1;}if(Ľ>0)ǀ();else if(Ĝ.x&&ƹ)ǁ();if((Ƹ||ƹ)&&(Ľ==0||Runtime.CurrentInstructionCount<30000)){Ƶ();}if(ƹ||ƺ
){Echo($"Refineries: {ĝ.Count} | Assemblers: {Ğ.Count} | Proto: {Ġ.Count} | Projectors: {Ņ.Count}");Echo(
$"Ore types: {ň.Count} | Components: {ŋ.Count}");Echo($"Containers: {ĩ.Count}");if(Ľ>0){Echo($"Cycle running: phase {Ľ}/28");}else{double ǂ=Math.Max(0,(Ĝ.p*100.0-ş)/
1000.0);Echo($"Next rescan in: {ǂ:F1}s");}if(Ĝ.À){Echo(Ƭ==0?"Cargo: idle":$"Cargo: active ({ƞ})");if(++Ʈ>=18&&(Ľ<6||Ľ>11)){Ʈ=0
;ǃ();}}else{Echo("Cargo: disabled");}}}void Ǆ(){ŵ.TryParse(Me.CustomData);if(!ŵ.ContainsSection("Recipes")){Me.CustomData
+=ŝ;ŵ.TryParse(Me.CustomData);}Ŝ.Clear();Ť.Clear();ŵ.GetKeys("Recipes",Ť);foreach(var Ó in Ť){string Ù=ŵ.Get(Ó).ToString(
"");if(Ù!="")Ŝ[Ó.Name]=f(Ù);}}void ǈ(){if(string.IsNullOrWhiteSpace(Me.CustomData))return;ŵ.Clear();if(!ŵ.TryParse(Me.
CustomData))return;var ǅ=new MyIni();ǅ.TryParse(new Í().é());Ť.Clear();ǅ.GetKeys(Ť);int ǆ=0;foreach(var Ó in Ť){if(!ŵ.ContainsKey(
Ó)){ŵ.Set(Ó,ǅ.Get(Ó).ToString());ǆ++;}}if(ǆ>0){Me.CustomData=ŵ.ToString();Ǉ(
$"Config migrated: {ǆ} new option(s) added with defaults. Review CustomData.");}}void ǀ(){switch(Ľ){case 1:Ř.Clear();ř.Clear();Ś.Clear();ǈ();Ĝ=Í.ç(Me.CustomData);Ǆ();ľ=0;Ľ=2;break;case 2:ǉ();Ľ=3;
break;case 3:Ǌ();Ľ=4;break;case 4:ǋ();Ľ=5;break;case 5:ǌ();Ľ=6;break;case 6:if(ľ==0){Ǎ();ǎ();}while(ľ<ĩ.Count){if(Ǐ())return;
ǐ(ĩ[ľ++].GetInventory(0));}ľ=0;Ľ=7;break;case 7:if(Ĝ.µ){while(ľ<Ķ.Count){if(Ǐ())return;var Ǒ=Ķ[ľ++];bool ǒ=Ǒ is
IMyUserControllableGun||Ǒ is IMyReactor;for(int Ó=0;Ó<Ǒ.InventoryCount;Ó++)ǐ(Ǒ.GetInventory(Ó),ǒ);}}ľ=0;Ľ=8;break;case 8:while(ľ<ī.Count+ķ.
Count){if(Ǐ())return;int U=ľ++;if(U<ī.Count){var Ǔ=ī[U];if(!ǔ(Ǔ))ǐ(Ǔ.GetInventory(0));}else if(Ĝ.º){var Ǖ=ķ[U-ī.Count].
GetInventory(0);ǖ(Ǖ,Ƥ,ō,O,Ĝ.l);ǖ(Ǖ,ƥ,ō,O,Ĝ.l);}}ľ=0;Ľ=10;break;case 10:Ǘ();Ľ=11;break;case 11:ǘ();Ǚ();ǚ();Ľ=12;break;case 12:Ǜ();ǜ()
;ǝ();Ǟ();ǟ();Ľ=13;break;case 13:Ǡ();ǡ();Ǣ();Ľ=14;break;case 14:ǣ();Ǥ();ǥ();Ǧ();Ľ=15;break;case 15:if(ľ==0){if(ĥ.Count==0)
Ǉ($"No {Ĝ.Á} containers found.");if(Ħ.Count==0)Ǉ($"No {Ĝ.Â} containers found.");if(ħ.Count==0)Ǉ(
$"No {Ĝ.Ã} containers found.");if(Ĩ.Count==0)Ǉ($"No {Ĝ.Ä} containers found.");ǧ(Ĝ.Á,ĥ);ǧ(Ĝ.Â,Ħ);ǧ(Ĝ.Ã,ħ);ǧ(Ĝ.Ä,Ĩ);}if(Ǩ(ĝ,ǩ=>{if(ĥ.Count==0)return;Ǫ(
ǩ.ð,Ơ,ĥ);if(!(ǩ.ö&&ǩ.ô!=""))Ǫ(ǩ.ï,Ɵ,ĥ);}))Ľ=16;break;case 16:if(Ǩ(Ğ,Ƽ=>{var ǫ=Ƽ.î.GetInventory(1);if(Ħ.Count>0)Ǫ(ǫ,ơ,Ħ);
if(ħ.Count>0)Ǫ(ǫ,Ƣ,ħ);if(Ĩ.Count>0){Ǫ(ǫ,ƣ,Ĩ);Ǫ(ǫ,Ʀ,Ĩ);Ǫ(ǫ,Ƥ,Ĩ);Ǫ(ǫ,ƥ,Ĩ);}if(!(Ƽ.ö&&Ƽ.û!="")&&ĥ.Count>0)Ǫ(Ƽ.î.GetInventory(
0),Ơ,ĥ);}))Ľ=17;break;case 17:if(Ǩ(Ġ,Ƽ=>{if(Ħ.Count>0)Ǫ(Ƽ.î.GetInventory(1),ơ,Ħ);if(!(Ƽ.ö&&Ƽ.û!="")&&ĥ.Count>0)Ǫ(Ƽ.î.
GetInventory(0),Ơ,ĥ);}))Ľ=18;break;case 18:if(Ĩ.Count==0){Ľ=19;break;}if(Ǩ(Ń,Ö=>{Ǫ(Ö.GetInventory(0),Ƥ,Ĩ);Ǫ(Ö.GetInventory(0),ƥ,Ĩ);}
))Ľ=19;break;case 19:{if(Ĩ.Count==0){Ľ=20;break;}int Ǭ,ǭ;Ş.TryGetValue("OxygenBottle",out Ǭ);Ş.TryGetValue(
"HydrogenBottle",out ǭ);if(Ǩ(ń,Ǯ=>{var ǯ=Ǯ.GetInventory(0);Ǫ(ǯ,Ƥ,Ĩ);Ǫ(ǯ,ƥ,Ĩ);if(ǯ.IsFull)return;foreach(var Ǔ in Ĩ){if(ǯ.IsFull)break;
var ǰ=Ǔ.GetInventory(0);ţ.Clear();ǰ.GetItems(ţ);for(int Ǳ=ţ.Count-1;Ǳ>=0;Ǳ--){var ǲ=ţ[Ǳ];if(ǲ.Type.TypeId==Ƥ){if(Ǭ>0)
continue;}else if(ǲ.Type.TypeId==ƥ){if(ǭ>0)continue;}else continue;ǰ.TransferItemTo(ǯ,ǲ,null);if(ǯ.IsFull)break;}}}))Ľ=20;break;
}case 20:if(ǳ(ĥ,Ɵ,Ơ))Ľ=21;break;case 21:if(ǳ(Ħ,ơ))Ľ=22;break;case 22:if(ǳ(ħ,Ƣ))Ľ=23;break;case 23:if(ǳ(Ĩ,ƣ,Ʀ))Ľ=24;break;
case 24:if(Ǩ(Ī,Ǔ=>Ǵ(Ǔ.GetInventory(0))))Ľ=25;break;case 25:if(Ǩ(Ĵ,Ǔ=>Ǵ(Ǔ.GetInventory(0))))Ľ=26;break;case 26:ǵ(ĥ,Ɵ,Ơ);ǵ(Ħ,ơ
,null);ǵ(ħ,Ƣ,null);ǵ(Ĩ,ƣ,Ʀ);Ľ=27;break;case 27:Ƕ();Ƿ();Ľ=28;break;case 28:Ǹ();ǹ();Ǻ();ǻ();y();Ǽ();Ľ=0;break;}}void Ǻ(){
foreach(var ǩ in ĝ){ţ.Clear();ǩ.ï.GetItems(ţ);string ǽ="";double Ǿ=0;foreach(var ǲ in ţ){if(ǲ.Type.TypeId!=Ɵ)continue;double ǿ=
(double)ǲ.Amount;if(ǿ>Ǿ){ǽ=ǲ.Type.SubtypeId;Ǿ=ǿ;}}ǩ.ô=ǽ;ǩ.õ=Ǿ;}}void ǉ(){ĝ.Clear();Ğ.Clear();ĥ.Clear();Ħ.Clear();ħ.Clear(
);Ĩ.Clear();ĩ.Clear();Ī.Clear();Ĵ.Clear();ĳ.Clear();ĸ.Clear();Ĺ.Clear();ĺ.Clear();ĵ.Clear();ī.Clear();Ĭ.Clear();ł.Clear()
;Ń.Clear();ń.Clear();Ņ.Clear();Ũ.Clear();Ȁ(Ũ,ȁ);foreach(var b in Ũ){if(Ȃ(b))continue;if(b.BlockDefinition.SubtypeId.
IndexOf("Prototech",D)>=0)ĝ.Insert(0,new ù(b));else ĝ.Add(new ù(b));}ũ.Clear();Ġ.Clear();Ȁ(ũ,ȁ);foreach(var ȃ in ũ){if(Ȃ(ȃ))
continue;if(ȃ.BlockDefinition.SubtypeId.Equals(C,D))Ġ.Add(new ÿ(ȃ));else Ğ.Add(new ÿ(ȃ));}}void Ǌ(){Ū.Clear();Ȁ(Ū,ȁ);foreach(var
Ǔ in Ū){string Ȅ=Ǔ.CustomName;bool ȅ=!string.IsNullOrWhiteSpace(Ĝ.Å)&&Ȅ.Contains(Ĝ.Å);if(ȅ)ĳ.Add(Ǔ);if(ȅ||Ȅ.Contains(Ĝ.Á)
)ĥ.Add(Ǔ);if(ȅ||Ȅ.Contains(Ĝ.Â))Ħ.Add(Ǔ);if(ȅ||Ȅ.Contains(Ĝ.Ã))ħ.Add(Ǔ);if(ȅ||Ȅ.Contains(Ĝ.Ä))Ĩ.Add(Ǔ);}Ł.Clear();foreach
(var c in Ĝ.Ì)Ł.Add(new í{ê=c.Key,ë=c.Value});Ŭ.Clear();Ȁ(Ŭ,Ǒ=>ȁ(Ǒ)&&Ǒ is IMyTextSurfaceProvider&&Ǒ!=Me);foreach(var Ǒ in
Ŭ){var Ȇ=(IMyTextSurfaceProvider)Ǒ;string Ȅ=Ǒ.CustomName;if(Ȅ.Contains(Ĝ.É)){int ȇ;int Ȉ;ȉ(Ȅ,Ĝ.É,Ȇ.SurfaceCount,out Ȉ,out
ȇ);var Ȋ=Ȇ.GetSurface(ȇ);ȋ(Ȋ);ł.Add(Ȋ);continue;}foreach(var Ȍ in Ł){if(Ȅ.Contains(Ȍ.ê)){int ȍ,Ȏ;ȉ(Ȅ,Ȍ.ê,Ȇ.SurfaceCount,
out ȍ,out Ȏ);var ȏ=Ȇ.GetSurface(Ȏ);ȋ(ȏ);Ȍ.ì.Add(ȏ);break;}}}}void ǋ(){Ȁ(ĩ,ȁ);Ȁ(Ń,ȁ);Ȁ(ń,ȁ);Ȁ(ī,Ǒ=>Ǒ.CustomName.Contains(Ĝ.Æ
)&&!Ȑ(Ǒ));Ȁ(Ĭ,Ǒ=>Ǒ.CustomName.Contains(Ĝ.Ç)&&!Ȑ(Ǒ));Ȁ(Ķ,Ǒ=>ȁ(Ǒ)&&Ǒ.HasInventory&&!(Ǒ is IMyCargoContainer)&&!(Ǒ is
IMyGasGenerator)&&(!(Ǒ is IMyProductionBlock)||Ȃ(Ǒ)));Ȁ(ķ,Ǒ=>(Ǒ is IMyGasGenerator||Ǒ is IMyGasTank)&&!ǔ(Ǒ)&&!Ȑ(Ǒ));ū.Clear();ȑ(ū);
foreach(var Ǔ in ū){if(!ȁ(Ǔ))continue;if(Ȓ(Ǔ.CustomName))continue;Ī.Add(Ǔ);}}void ǌ(){ů.Clear();Ű.Clear();foreach(var Ǔ in ū){
if(ǔ(Ǔ)||Ȑ(Ǔ))continue;var ȓ=Ǔ.CubeGrid;if(ȓ==null)continue;var Ǖ=Ǔ.GetInventory(0);if(Ǖ==null)continue;var ȕ=Ȕ(ȓ.
CustomName);ȕ.ā+=(double)Ǖ.CurrentVolume;ȕ.Ă+=(double)Ǖ.MaxVolume;ȕ.ć++;}ű.Clear();ȑ(ű);foreach(var Ö in ű){if(ǔ(Ö)||Ȑ(Ö))continue
;var ȓ=Ö.CubeGrid;if(ȓ==null)continue;var ȕ=Ȕ(ȓ.CustomName);double Ȗ=Ö.Capacity;double ȗ=Ö.FilledRatio*Ȗ;if(Ö.
BlockDefinition.SubtypeId.Contains("Hydrogen")){ȕ.Ą+=Ȗ;ȕ.ă+=ȗ;}else{ȕ.Ć+=Ȗ;ȕ.ą+=ȗ;}}Ȁ(Ņ,ȁ);ņ=null;if(!string.IsNullOrWhiteSpace(Ĝ.Ë)){ŭ
.Clear();Ȁ(ŭ,Ǒ=>Ǒ.CustomName.Contains(Ĝ.Ë)&&!Ǒ.Equals(Me));ņ=ŭ.Count>0?ŭ[0]:null;}Ů.Clear();ȑ(Ů);foreach(var Ǔ in Ů){if(!
ȁ(Ǔ))continue;if(Ȓ(Ǔ.CustomName))continue;Ĵ.Add(Ǔ);}string Ș=Ĝ.Ç;Ȁ(ĸ,Ǒ=>!ǔ(Ǒ)&&!Ȑ(Ǒ)&&Ǒ.CustomName.Contains(Ș));Ȁ(Ĺ,Ǒ=>!ǔ
(Ǒ)&&!Ȑ(Ǒ)&&Ǒ.CustomName.Contains(Ș));Ȁ(ĺ,Ǒ=>!ǔ(Ǒ)&&!Ȑ(Ǒ)&&Ǒ.CustomName.Contains(Ș));Ȁ(ĵ,Ǒ=>!ǔ(Ǒ)&&!Ȑ(Ǒ)&&Ǒ.CustomName.
Contains(Ș));}void Ǎ(){ň.Clear();ŉ.Clear();ŋ.Clear();Ō.Clear();ō.Clear();Ŏ.Clear();Ş.Clear();}void Ǘ(){foreach(var Ǯ in ń)ǐ(Ǯ.
GetInventory(0));foreach(var ǩ in ĝ){ǩ.ö=ǩ.î.IsWorking;ţ.Clear();ǩ.ï.GetItems(ţ);string ǽ="";double Ǿ=0;foreach(var ǲ in ţ){if(ǲ.
Type.TypeId!=Ɵ)continue;double ǿ=(double)ǲ.Amount;ș(ǲ.Type.SubtypeId);var R=ň[ǲ.Type.SubtypeId];R.ċ+=ǿ;ň[ǲ.Type.SubtypeId]=R
;if(ǿ>Ǿ){ǽ=ǲ.Type.SubtypeId;Ǿ=ǿ;}}ǩ.ô=ǽ;ǩ.õ=Ǿ;Ț(ǩ.ð);}foreach(var Ƽ in Ğ){Ƽ.ö=Ƽ.î.IsWorking;ț(Ƽ.î.GetInventory(1));Ȝ(Ƽ.î.
GetInventory(1));ȝ(Ƽ.î.GetInventory(1));Ț(Ƽ.î.GetInventory(0));ț(Ƽ.î.GetInventory(0));Ƽ.þ.Clear();Ƽ.î.GetQueue(Ƽ.þ);Ƽ.ý=Ƽ.þ.Count;Ƽ.
û="";Ƽ.ü=0;bool Ȟ=Ƽ.î.Mode==K;for(int ȟ=0;ȟ<Ƽ.þ.Count;ȟ++){var Ƞ=Ƽ.þ[ȟ];string ȡ=Ƞ.BlueprintId.SubtypeId.ToString();
double Ȣ=(double)Ƞ.Amount;if(ȟ==0){Ƽ.û=ȡ;Ƽ.ü=Ȣ;}if(Ȟ){string Ȥ=ȣ(ȡ);if(Ȥ!=""){ȥ(Ȥ);var Ȧ=ŋ[Ȥ];Ȧ.ĕ+=(int)Math.Ceiling(Ȣ);ŋ[Ȥ]=Ȧ
;}string Ȩ=ȧ(ȡ);if(Ȩ!=""){ȩ(Ȩ);var Ȫ=Ō[Ȩ];Ȫ.ĕ+=(int)Math.Ceiling(Ȣ);Ō[Ȩ]=Ȫ;}string Ȭ=ȫ(ȡ);if(Ȭ!=""){ȭ(Ȭ);var Ȯ=ō[Ȭ];Ȯ.ĕ+=
(int)Math.Ceiling(Ȣ);ō[Ȭ]=Ȯ;}}else{ȯ(ȣ(ȡ),Ȣ);ȯ(ȧ(ȡ),Ȣ);ȯ(ȫ(ȡ),Ȣ);}}}foreach(var Ƽ in Ġ){Ƽ.ö=Ƽ.î.IsWorking;Ȱ(Ƽ.î.
GetInventory(1));Ț(Ƽ.î.GetInventory(0));Ƽ.þ.Clear();Ƽ.î.GetQueue(Ƽ.þ);Ƽ.ý=Ƽ.þ.Count;Ƽ.û="";Ƽ.ü=0;bool ȱ=Ƽ.î.Mode==K;for(int ȟ=0;ȟ<Ƽ.
þ.Count;ȟ++){var Ƞ=Ƽ.þ[ȟ];string ȡ=Ƞ.BlueprintId.SubtypeId.ToString();double Ȣ=(double)Ƞ.Amount;if(ȟ==0){Ƽ.û=ȡ;Ƽ.ü=Ȣ;}
string ȳ=Ȳ(ȡ);if(ȳ=="")continue;if(ȱ){ȴ(ȳ,Ŏ,P,Ĝ.m);var Ȧ=Ŏ[ȳ];Ȧ.ĕ+=(int)Math.Ceiling(Ȣ);Ŏ[ȳ]=Ȧ;}else{ȯ(ȳ,Ȣ);}}}}void ǘ(){
foreach(var ȵ in L.Keys)ș(ȵ);ś.Clear();foreach(var Ó in ň.Keys)ś.Add(Ó);foreach(var Þ in ś){var R=ň[Þ];double ȶ;ŉ.TryGetValue(R
.Ċ,out ȶ);if(R.ĉ.Equals("Ice",D))ȶ=R.ċ;R.Č=ȶ;ň[Þ]=R;}foreach(var ȷ in Ĝ.j)ȥ(ȷ.Key);foreach(var ȷ in Ĝ.k)ȩ(ȷ.Key);foreach(
var ȷ in Ĝ.l)ȭ(ȷ.Key);foreach(var ȷ in Ĝ.m)ȴ(ȷ.Key,Ŏ,P,Ĝ.m);foreach(var ȷ in Ĝ.g)if(!ȷ.Key.Equals("Ice",D)&&!ŉ.ContainsKey(
ȷ.Key))ŉ[ȷ.Key]=0;double ȸ=0;foreach(var ȷ in ň)if(ȷ.Value.ċ>ȸ)ȸ=ȷ.Value.ċ;š=Math.Max(Math.Max(ȸ,1.0),š*0.95);}void Ț(
IMyInventory Ǖ){ţ.Clear();Ǖ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type.TypeId!=Ơ)continue;string ß=ǲ.Type.SubtypeId;if(ß.Equals(
"Stone",D)||ß.Equals("Ingot",D))ß="Gravel";double ȹ;ŉ.TryGetValue(ß,out ȹ);ŉ[ß]=ȹ+(double)ǲ.Amount;}}void ț(IMyInventory Ǖ){ǖ(Ǖ
,ơ,ŋ,M,Ĝ.j);}void ș(string Ⱥ){if(ň.ContainsKey(Ⱥ))return;string Ȼ;if(!L.TryGetValue(Ⱥ,out Ȼ)&&!Ĝ.i.TryGetValue(Ⱥ,out Ȼ))Ȼ
=Ⱥ;double á=0;Ĝ.g.TryGetValue(Ȼ,out á);ň[Ⱥ]=new đ{ĉ=Ⱥ,Ċ=Ȼ,ċ=0,Č=0,č=á,ď=Ĝ.n.Contains(Ⱥ),Ď=0,};}void Ǚ(){bool ȼ=false;var
Ð=new MyIni();MyIniParseResult Ƚ;if(!Ð.TryParse(Me.CustomData,out Ƚ)){Ǉ($"CustomData parse error: {Ƚ.Error}");return;}
foreach(var ß in ŋ.Keys){if(M.ContainsKey(ß))continue;if(P.ContainsKey(ß))continue;if(Ĝ.h.ContainsKey(ß))continue;if(!Ð.Get(
"CustomItems",ß).IsEmpty)continue;Ð.Set("CustomItems",ß,"0");ȼ=true;}foreach(var ß in Ō.Keys){if(N.ContainsKey(ß))continue;if(!Ð.Get(
"CustomItems",ß).IsEmpty)continue;Ð.Set("CustomItems",ß,"0");ȼ=true;}foreach(var ß in ō.Keys){if(O.ContainsKey(ß))continue;if(!Ð.Get(
"CustomItems",ß).IsEmpty)continue;Ð.Set("CustomItems",ß,"0");ȼ=true;}foreach(var ß in ň.Keys){if(L.ContainsKey(ß))continue;if(Ĝ.i.
ContainsKey(ß))continue;if(!Ð.Get("CustomOres",ß).IsEmpty)continue;Ð.Set("CustomOres",ß,"0");ȼ=true;}if(ȼ)Me.CustomData=Ð.ToString(
);}void ǚ(){foreach(var ȷ in Ĝ.h){string ß=ȷ.Key;if(M.ContainsKey(ß))continue;int á;if(!Ĝ.j.TryGetValue(ß,out á)||á<=0)
continue;ě Ȧ;if(Ō.TryGetValue(ß,out Ȧ)){Ȧ.Ė=á;Ō[ß]=Ȧ;ŋ.Remove(ß);Ĝ.k[ß]=á;}else if(ō.TryGetValue(ß,out Ȧ)){Ȧ.Ė=á;ō[ß]=Ȧ;ŋ.Remove
(ß);Ĝ.l[ß]=á;}}}void ȥ(string ß){if(ŋ.ContainsKey(ß))return;string â;if(!M.TryGetValue(ß,out â)&&!Ĝ.h.TryGetValue(ß,out â
))â=ß;int á;bool Ⱦ=Ĝ.j.TryGetValue(ß,out á);ŋ[ß]=new ě{Ē=ß,ē=â,Ė=á,Ę=Ⱦ};}void ȩ(string ß){ȴ(ß,Ō,N,Ĝ.k);}void ȭ(string ß){
ȴ(ß,ō,O,Ĝ.l);}string ɀ(string à,Dictionary<string,string>ȿ){foreach(var c in ȿ)if(c.Value.Equals(à,D))return c.Key;return
"";}string ȣ(string R){return ɀ(R,M);}string ȧ(string R){return ɀ(R,N);}string ȫ(string R){return ɀ(R,O);}string Ȳ(string
R){return ɀ(R,P);}string Ɂ(string ß){string Ù;return Q.TryGetValue(ß,out Ù)?Ù:V(ß);}string ɂ(string R){string b=ȣ(R);if(b
!="")return b;b=ȧ(R);if(b!="")return b;b=ȫ(R);if(b!="")return b;b=Ȳ(R);if(b!="")return b;return R;}void Ȱ(IMyInventory Ǖ){
ǖ(Ǖ,ơ,Ŏ,P,Ĝ.m);}void Ȝ(IMyInventory Ǖ){ǖ(Ǖ,Ƣ,Ō,N,Ĝ.k);}void ȝ(IMyInventory Ǖ){ǖ(Ǖ,ƣ,ō,O,Ĝ.l);ǖ(Ǖ,Ʀ,ō,O,Ĝ.l);ǖ(Ǖ,Ƥ,ō,O,Ĝ.l
);ǖ(Ǖ,ƥ,ō,O,Ĝ.l);}void ȴ(string ß,Dictionary<string,ě>Ò,Dictionary<string,string>Ƀ,Dictionary<string,int>å){if(Ò.
ContainsKey(ß))return;string â;if(!Ƀ.TryGetValue(ß,out â)&&!Ĝ.h.TryGetValue(ß,out â))â=ß;int á=0;bool Ⱦ=å.TryGetValue(ß,out á);Ò[ß]
=new ě{Ē=ß,ē=â,Ė=á,Ę=Ⱦ};}void ǖ(IMyInventory Ǖ,string Ʉ,Dictionary<string,ě>Ò,Dictionary<string,string>Ƀ,Dictionary<
string,int>å){if(Ǖ==null)return;ţ.Clear();Ǖ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type.TypeId!=Ʉ)continue;string ß=ǲ.Type.
SubtypeId;ȴ(ß,Ò,Ƀ,å);var Ȧ=Ò[ß];Ȧ.Ĕ+=(int)(double)ǲ.Amount;Ò[ß]=Ȧ;}}void ǐ(IMyInventory Ǖ,bool ǒ=false){if(Ǖ==null)return;ţ.Clear
();Ǖ.GetItems(ţ);foreach(var ǲ in ţ){string Ʉ=ǲ.Type.TypeId;string ß=ǲ.Type.SubtypeId;double ǿ=(double)ǲ.Amount;if(Ʉ==Ɵ){
ș(ß);var R=ň[ß];R.ċ+=ǿ;ň[ß]=R;}else if(Ʉ==Ơ){if(ß.Equals("Stone",D)||ß.Equals("Ingot",D))ß="Gravel";double Ʌ;ŉ.
TryGetValue(ß,out Ʌ);ŉ[ß]=Ʌ+ǿ;}else if(Ʉ==ơ){Ɇ(ß,ǿ,ŋ,M,Ĝ.j,ǒ);Ɇ(ß,ǿ,Ŏ,P,Ĝ.m,ǒ);}else if(Ʉ==Ƣ){Ɇ(ß,ǿ,Ō,N,Ĝ.k,ǒ);}else if(Ʉ==ƣ||Ʉ==Ʀ
||Ʉ==Ƥ||Ʉ==ƥ){Ɇ(ß,ǿ,ō,O,Ĝ.l,ǒ);}}}void Ɇ(string ß,double ǿ,Dictionary<string,ě>Ò,Dictionary<string,string>Ƀ,Dictionary<
string,int>å,bool ǒ){ȴ(ß,Ò,Ƀ,å);var Ȧ=Ò[ß];Ȧ.Ĕ+=(int)ǿ;if(ǒ)Ȧ.ė+=(int)ǿ;Ò[ß]=Ȧ;}void ǎ(){foreach(var Ǔ in ī){if(string.
IsNullOrWhiteSpace(Ǔ.CustomData))continue;if(!ŵ.TryParse(Ǔ.CustomData))continue;ɇ("LoadOut.Components");ɇ("LoadOut.Ammo");ɇ(
"LoadOut.Tools");ɇ("LoadOut.Consumables");ɇ("LoadOut.Bottles");Ɉ("LoadOut.Ingots",false);Ɉ("LoadOut.Ores",true);}}void ɇ(string ɉ){Ť.
Clear();ŵ.GetKeys(ɉ,Ť);foreach(var Ó in Ť){int Ɋ=(int)ŵ.Get(Ó).ToDouble(0);if(Ɋ<=0)continue;string ß=Ó.Name;int Ʌ;if(Ĝ.j.
TryGetValue(ß,out Ʌ))Ĝ.j[ß]=Math.Max(Ʌ,0)+Ɋ;else if(Ĝ.k.TryGetValue(ß,out Ʌ))Ĝ.k[ß]=Math.Max(Ʌ,0)+Ɋ;else if(Ĝ.l.TryGetValue(ß,out Ʌ
))Ĝ.l[ß]=Math.Max(Ʌ,0)+Ɋ;else if(Ĝ.m.TryGetValue(ß,out Ʌ))Ĝ.m[ß]=Math.Max(Ʌ,0)+Ɋ;}}void Ɉ(string ɉ,bool ɋ){Ť.Clear();ŵ.
GetKeys(ɉ,Ť);foreach(var Ó in Ť){double Ɋ=ŵ.Get(Ó).ToDouble(0);if(Ɋ<=0)continue;string ß=Ó.Name;if(ɋ&&!ß.Equals("Ice",D))
continue;if(ß.Equals("Stone",D))ß="Gravel";double Ʌ;if(Ĝ.g.TryGetValue(ß,out Ʌ))Ĝ.g[ß]=Math.Max(Ʌ,0.0)+Ɋ;}}void ɏ(Dictionary<
string,ě>Ò,List<ě>Ɍ){ś.Clear();foreach(var Ó in Ò.Keys)ś.Add(Ó);foreach(var Þ in ś){var Ȧ=Ò[Þ];double ɍ=0;if(Ȧ.Ė>0)ɍ+=(1.0-(
double)Ȧ.Ĕ/Ȧ.Ė)*500.0;int Ɏ=Ȧ.ę;if(Ɏ>0)ɍ+=Math.Log10(Ɏ+1)*20.0;Ȧ.Ď=ɍ;Ò[Þ]=Ȧ;}Ɍ.Clear();foreach(var Þ in ś)Ɍ.Add(Ò[Þ]);Ɍ.Sort((
ȃ,Ǒ)=>Ǒ.Ď.CompareTo(ȃ.Ď));}void ɘ(List<ě>Ɍ,List<ÿ>ɐ,double ɑ=1.0){if(!Ĝ.z)return;ğ.Clear();foreach(var Ƽ in ɐ)if(Ƽ.ú&&Ƽ.ö
)ğ.Add(Ƽ);if(ğ.Count==0)return;foreach(var Ȧ in Ɍ){if(Ȧ.Ė<=0)continue;if(Ȧ.ē=="")continue;int ɒ=Math.Max(0,(int)(Ȧ.Ė*ɑ)-Ȧ
.Ĕ);if(Ĝ.r>0&&ɒ>Ĝ.r)ɒ=Ĝ.r;int ɓ=Math.Max(0,ɒ-Ȧ.ĕ);if(ɓ<=0)continue;MyDefinitionId ɔ;if(!MyDefinitionId.TryParse(Ƨ+Ȧ.ē,out
ɔ)){ř.Add(Ȧ.ē);continue;}int ɕ=ɓ/ğ.Count,ɖ=ɓ%ğ.Count;for(int ɗ=0;ɗ<ğ.Count;ɗ++){var Ƽ=ğ[ɗ];if(Ƽ.î.Mode!=K)continue;int ǿ=
ɕ+(ɗ<ɖ?1:0);if(ǿ>0)try{Ƽ.î.AddQueueItem(ɔ,(MyFixedPoint)ǿ);}catch{Ř.Add(Ȧ.ē);}}}}void Ǡ(){Ŋ.Clear();ə(Ğ,false);ə(Ġ,true);
}void ə(List<ÿ>ɚ,bool ɛ){double ɜ=ɛ?Ĝ.u:Ĝ.t;if(ɜ<=0)ɜ=1.0;foreach(var Ƽ in ɚ){if(Ƽ.î.Mode!=K)continue;for(int ȟ=0;ȟ<Ƽ.þ.
Count;ȟ++){var Ƞ=Ƽ.þ[ȟ];string ȡ=Ƞ.BlueprintId.SubtypeId.ToString();double ɝ=(double)Ƞ.Amount;Y[]ɞ;if(!Ŝ.TryGetValue(ȡ,out ɞ)
)continue;for(int ǩ=0;ǩ<ɞ.Length;ǩ++){double Ʌ;Ŋ.TryGetValue(ɞ[ǩ].W,out Ʌ);Ŋ[ɞ[ǩ].W]=Ʌ+ɞ[ǩ].X*ɝ/ɜ;}}}}void Ǜ(){ś.Clear();
foreach(var Ó in ň.Keys)ś.Add(Ó);foreach(var Þ in ś){var R=ň[Þ];double ɍ=0;bool ǅ=R.č>0&&R.Č<R.č;if(ǅ){double ȗ=R.Č/R.č;ɍ+=(1.0
-ȗ)*1000.0;}else if(R.ď){ɍ+=500.0;}if(R.ċ>0)ɍ+=Math.Log10(R.ċ+1)*10.0;R.Ď=ɍ;ň[Þ]=R;}ŏ.Clear();foreach(var ȷ in ň)ŏ.Add(ȷ.
Value);ŏ.Sort((ȃ,Ǒ)=>Ǒ.Ď.CompareTo(ȃ.Ď));}void ǜ(){ɏ(ŋ,Ŕ);}void ǡ(){foreach(var ǩ in ĝ)ǩ.ó="";Ő.Clear();ő.Clear();Œ.Clear();œ
.Clear();Ŧ.Clear();foreach(var R in ŏ){if(R.ĉ.Equals("Ice",D))continue;if(R.ċ>0)Ŧ.Add(R.Ċ);if(R.ċ<=0)continue;bool ǅ=R.č>
0&&R.Č<R.č;if(ǅ&&R.ď)Ő.Add(R);else if(ǅ)ő.Add(R);else if(R.ď)Œ.Add(R);else œ.Add(R);}ť.Clear();foreach(var R in ŏ){if(R.ĉ
.Equals("Ice",D))continue;if(R.č<=0||R.Č>=R.č)continue;if(!ť.Add(R.Ċ))continue;if(!Ŧ.Contains(R.Ċ))Ǉ(
$"No ore to refine {R.Ċ} ({R.Č:F0}/{R.č:F0}kg).");}bool ɟ=Ő.Count+ő.Count>0;List<đ>ɠ,ɡ,ɢ,ɣ;if(ɟ){ɠ=Ő;ɡ=ő;ɢ=ő;ɣ=Ő;}else{ɠ=Œ;ɡ=œ;ɢ=œ;ɣ=Œ;}if(ɠ.Count+ɡ.Count==0)return;int
ɤ=0,ɥ=0;var ɦ=Ĝ.w?ɣ:ŧ;foreach(var ǩ in ĝ){if(!ǩ.î.IsFunctional)continue;if(ǩ.ò&&!ǩ.ñ)ǩ.ó=ɧ(ɥ++,ɢ,ɦ);else ǩ.ó=ɧ(ɤ++,ɠ,ɡ);}
}string ɧ(int ɨ,List<đ>ɩ,List<đ>ɪ){if(ɩ.Count>0)return ɩ[ɨ%ɩ.Count].ĉ;if(ɪ.Count>0)return ɪ[ɨ%ɪ.Count].ĉ;return"";}void ǝ
(){ɏ(Ō,ŕ);}void Ǟ(){ɏ(ō,Ŗ);}void ǣ(){ɘ(Ŕ,Ğ);}void Ǥ(){ɘ(ŕ,Ğ);}void ǥ(){ɘ(Ŗ,Ğ);}void Ǧ(){ɘ(ŗ,Ġ);}void ǟ(){ɏ(Ŏ,ŗ);}void Ǣ()
{if(!Ĝ.ª)return;if(ɫ()){if(ɬ(Ų)||ɬ(ų))Ǉ("Disassembly paused: active projector.");return;}ɭ(Ų,Ğ,"master assembler");ɭ(ų,Ġ,
"proto master");}bool ɬ(List<ě>[]Ɍ){foreach(var ɮ in Ɍ)foreach(var Ȧ in ɮ){if(!Ȧ.Ę||Ȧ.ē=="")continue;int ɯ;Ş.TryGetValue(Ȧ.Ē,out ɯ);if
(Ȧ.Ě-ɯ>0)return true;}return false;}void ɭ(List<ě>[]Ɍ,List<ÿ>ɰ,string ɱ){bool ɲ=ɬ(Ɍ);if(!ɲ){foreach(var Ƽ in ɰ){if(!Ƽ.ú||
!Ƽ.ö)continue;if(Ƽ.î.Mode==MyAssemblerMode.Disassembly&&Ƽ.ý==0)Ƽ.î.Mode=K;}return;}ġ.Clear();foreach(var Ƽ in ɰ)if(Ƽ.ú&&Ƽ
.ö&&Ƽ.î.Mode==MyAssemblerMode.Disassembly)ġ.Add(Ƽ);if(ġ.Count==0){foreach(var Ƽ in ɰ){if(!Ƽ.ú||!Ƽ.ö)continue;if(Ƽ.î.Mode
==K&&Ƽ.ý==0){ģ.Clear();Ƽ.î.GetQueue(ģ);if(ģ.Count>0)continue;Ƽ.î.Mode=MyAssemblerMode.Disassembly;ġ.Add(Ƽ);break;}}}if(ġ.
Count==0){Ǉ($"Disassembly pending: no idle {ɱ} to flip.");return;}foreach(var ɮ in Ɍ){foreach(var Ȧ in ɮ){if(!Ȧ.Ę||Ȧ.ē=="")
continue;int ɳ;Ş.TryGetValue(Ȧ.Ē,out ɳ);int ɴ=Ȧ.Ě-ɳ;if(ɴ<=0)continue;MyDefinitionId ɵ;if(!MyDefinitionId.TryParse(Ƨ+Ȧ.ē,out ɵ))
continue;Ģ.Clear();foreach(var Ƽ in ġ){bool ɶ=false;foreach(var Ƞ in Ƽ.þ)if(Ƞ.BlueprintId.SubtypeId.ToString().Equals(Ȧ.ē,D)){ɶ=
true;break;}if(!ɶ)Ģ.Add(Ƽ);}if(Ģ.Count==0)continue;int ɷ=ɴ/Ģ.Count;int ɸ=ɴ%Ģ.Count;for(int ɗ=0;ɗ<Ģ.Count;ɗ++){int ǿ=ɷ+(ɗ<ɸ?1
:0);if(ǿ>0)try{Ģ[ɗ].î.AddQueueItem(ɵ,(MyFixedPoint)ǿ);}catch{}}}}}bool ɫ(){foreach(var a in Ņ)if(a.IsProjecting&&a.
RemainingBlocks>0)return true;return false;}void Ǽ(){if(ņ==null)return;bool ɹ=ɫ();if(!ɹ&&ņ.Enabled)ņ.TryRun("PMDISPLAY");ņ.Enabled=ɹ;}
void ƽ(){while(ƫ!=null&&ƫ.HasPendingMessage){var ɺ=ƫ.AcceptMessage();var Ò=ɺ.Data as string;if(Ò==null)continue;var a=Ò.
Split('|');if(a.Length<2)continue;string ɻ=a[0],ɼ=a[1];if(Ƭ!=0){IGC.SendBroadcastMessage(Ʃ,"BUSY|"+ƞ);continue;}if(ɻ=="PULL")
{ɽ(ɼ);}else if(ɻ=="PUSH"&&a.Length>=3){ɾ(ɼ,a[2]);}else if(ɻ=="PUSH_SURPLUS"&&a.Length>=3){double ɿ;if(double.TryParse(a[2
],out ɿ)){ʀ(ɼ,ɿ);}}}}void ƾ(){if(Ƭ==1)ʁ();else if(Ƭ==2)ʂ();else if(Ƭ==3)ʃ();}bool ʅ(string ʄ){Ư.Clear();ū.Clear();ȑ(ū);
foreach(var Ǔ in ū){if(Ǔ.CubeGrid==Me.CubeGrid)continue;if(Ȑ(Ǔ))continue;if(Ǔ.CubeGrid!=null&&string.Equals(Ǔ.CubeGrid.
CustomName,ʄ,StringComparison.OrdinalIgnoreCase))Ư.Add(Ǔ);}return Ư.Count>0;}void ʈ(string ʆ,out string Ʉ,out List<
IMyCargoContainer>ʇ){Ʉ=null;ʇ=null;if(ʆ=="Component"){Ʉ=ơ;ʇ=Ħ;}else if(ʆ=="AmmoMagazine"){Ʉ=Ƣ;ʇ=ħ;}else if(ʆ=="PhysicalGunObject"||ʆ==
"Gear"){Ʉ=ƣ;ʇ=Ĩ;}else if(ʆ=="Ingot"){Ʉ=Ơ;ʇ=ĥ;}else if(ʆ=="Ore"){Ʉ=Ɵ;ʇ=ĥ;}}void ɽ(string ɼ){if(!ʅ(ɼ)){IGC.SendBroadcastMessage(
Ʃ,"ERROR|"+ɼ+"|NO_CONTAINERS");return;}ƞ=ɼ;Ƭ=1;ƭ=0;}void ʁ(){if(ƭ>=Ư.Count){ʉ();IGC.SendBroadcastMessage(Ʃ,
"PULL_COMPLETE|"+ƞ);Ƭ=0;return;}Ǵ(Ư[ƭ].GetInventory(0));ƭ++;}double ʔ(string ʊ,int ɒ){int ʋ=ʊ.IndexOf('.');if(ʋ<0)return 0;string ʆ=ʊ.
Substring(0,ʋ),ß=ʊ.Substring(ʋ+1);string Ʉ;List<IMyCargoContainer>ʇ;ʈ(ʆ,out Ʉ,out ʇ);if(ʇ==null||Ʉ==null)return 0;if(ʆ=="Ingot"&&
ß.Equals("Gravel",D))ß="Stone";var ʌ=new MyItemType(Ʉ,ß);double ʍ=0;for(int ʎ=0;ʎ<2;ʎ++){var ʏ=ʎ==0?ʇ:Ī;foreach(var R in
ʏ){if(ʍ>=ɒ)break;var ʐ=R.GetInventory(0);ţ.Clear();ʐ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type!=ʌ)continue;double ʑ=Math.
Min((double)ǲ.Amount,ɒ-ʍ);foreach(var ʒ in Ư){var ʓ=ʒ.GetInventory(0);if(ʓ.IsFull)continue;if(ʐ.TransferItemTo(ʓ,ǲ,(
MyFixedPoint)ʑ)){ʍ+=ʑ;break;}}break;}}if(ʍ>=ɒ)break;}return ʍ;}void ɾ(string ɼ,string ʕ){if(!ʅ(ɼ)){IGC.SendBroadcastMessage(Ʃ,
"ERROR|"+ɼ+"|NO_CONTAINERS");return;}ƞ=ɼ;Ƭ=2;ƭ=0;ư.Clear();Ʊ.Clear();Ʋ.Clear();foreach(var ʖ in ʕ.Split(',')){int ʗ=ʖ.IndexOf(
':');if(ʗ<0)continue;ư.Add(ʖ.Substring(0,ʗ));int ȃ;if(int.TryParse(ʖ.Substring(ʗ+1),out ȃ))Ʊ.Add(ȃ);else Ʊ.Add(0);}}void ʂ(
){if(ƭ>=ư.Count){ʉ();IGC.SendBroadcastMessage(Ʃ,"PUSH_COMPLETE|"+ƞ+"|"+Ʋ.ToString());Ƭ=0;return;}double ʍ=ʔ(ư[ƭ],Ʊ[ƭ]);if
(ʍ>0){if(Ʋ.Length>0)Ʋ.Append(',');Ʋ.Append(ư[ƭ]).Append(':').Append((int)ʍ);}ƭ++;}void ʀ(string ɼ,double ʘ){if(!ʅ(ɼ)){IGC
.SendBroadcastMessage(Ʃ,"ERROR|"+ɼ+"|NO_CONTAINERS");return;}ƞ=ɼ;Ƭ=3;ƭ=0;Ƴ=ʘ;ư.Clear();Ʊ.Clear();Ʋ.Clear();foreach(var c
in ŋ){if(c.Value.Ė<=0)continue;int R=c.Value.Ě;if(R>0){ư.Add("Component."+c.Key);Ʊ.Add(R);}}foreach(var c in Ō){if(c.Value
.Ė<=0)continue;int R=c.Value.Ě;if(R>0){ư.Add("AmmoMagazine."+c.Key);Ʊ.Add(R);}}foreach(var c in ō){if(c.Value.Ė<=0)
continue;int R=c.Value.Ě;if(R>0){ư.Add("PhysicalGunObject."+c.Key);Ʊ.Add(R);}}foreach(var c in ŉ){double å;Ĝ.g.TryGetValue(c.Key
,out å);double ʙ;Ŋ.TryGetValue(c.Key,out ʙ);double R=c.Value-å-ʙ;if(R>100){ư.Add("Ingot."+c.Key);Ʊ.Add((int)Math.Round(R)
);}}foreach(var c in ň){if(c.Key=="Ice"){if(c.Value.č<=0)continue;double R=c.Value.ċ-c.Value.č;if(R>100){ư.Add("Ore.Ice")
;Ʊ.Add((int)R);}}}}void ʃ(){if(ƭ>=ư.Count||ʚ()>=Ƴ){ʉ();IGC.SendBroadcastMessage(Ʃ,"PUSH_COMPLETE|"+ƞ+"|"+Ʋ.ToString());Ƭ=
0;return;}double ʍ=ʔ(ư[ƭ],Ʊ[ƭ]);if(ʍ>0){if(Ʋ.Length>0)Ʋ.Append(',');Ʋ.Append(ư[ƭ]).Append(':').Append((int)ʍ);}ƭ++;}
double ʚ(){double Ù=0;foreach(var Ǔ in Ư)Ù+=(double)Ǔ.GetInventory(0).CurrentVolume*1000;return Ù;}void ǃ(){string ʛ=Me.
CubeGrid.EntityId.ToString();Ţ.Clear();Ţ.Append("INVENTORY|").Append(ʛ);ʜ(Ţ,ŋ,"Component",false);ʜ(Ţ,Ō,"AmmoMagazine",false);ʜ(Ţ
,ō,"PhysicalGunObject",false);foreach(var c in ŉ){if(c.Value>0){Ţ.Append('|');Ţ.Append("Ingot.").Append(c.Key).Append(':'
).Append((int)c.Value);}}IGC.SendBroadcastMessage(ƪ,Ţ.ToString());Ţ.Clear();Ţ.Append("SURPLUS|").Append(ʛ);ʜ(Ţ,ŋ,
"Component",true);ʜ(Ţ,Ō,"AmmoMagazine",true);ʜ(Ţ,ō,"PhysicalGunObject",true);foreach(var c in ŉ){double å;Ĝ.g.TryGetValue(c.Key,out
å);double ʙ;Ŋ.TryGetValue(c.Key,out ʙ);double R=c.Value-å-ʙ;if(R>0){Ţ.Append('|');Ţ.Append("Ingot.").Append(c.Key).Append
(':').Append((int)Math.Round(R));}}{đ ʝ;if(ň.TryGetValue("Ice",out ʝ)&&ʝ.č>0){double R=ʝ.ċ-ʝ.č;if(R>0){Ţ.Append('|').
Append("Ore.Ice:").Append((int)R);}}}IGC.SendBroadcastMessage(ƪ,Ţ.ToString());Ţ.Clear();Ţ.Append("DEFICIT|").Append(ʛ);foreach
(var c in ŋ){int Ò=c.Value.Ė-c.Value.Ĕ-c.Value.ĕ;if(Ò>0){Ţ.Append('|');Ţ.Append("Component.").Append(c.Key).Append(':').
Append(Ò);}}foreach(var c in Ō){int Ò=c.Value.Ė-c.Value.Ĕ-c.Value.ĕ;if(Ò>0){Ţ.Append('|');Ţ.Append("AmmoMagazine.").Append(c.
Key).Append(':').Append(Ò);}}foreach(var c in ō){int Ò=c.Value.Ė-c.Value.Ĕ-c.Value.ĕ;if(Ò>0){Ţ.Append('|');Ţ.Append(
"PhysicalGunObject.").Append(c.Key).Append(':').Append(Ò);}}foreach(var c in ŉ){double å;Ĝ.g.TryGetValue(c.Key,out å);double ʙ;Ŋ.TryGetValue
(c.Key,out ʙ);double Ò=å+ʙ-c.Value;if(Ò>=1.0){Ţ.Append('|');Ţ.Append("Ingot.").Append(c.Key).Append(':').Append((int)Math
.Ceiling(Ò));}}foreach(var c in Ŋ){if(ŉ.ContainsKey(c.Key))continue;double å;Ĝ.g.TryGetValue(c.Key,out å);double Ò=å+c.
Value;if(Ò>=1.0){Ţ.Append('|');Ţ.Append("Ingot.").Append(c.Key).Append(':').Append((int)Math.Ceiling(Ò));}}{đ ʝ;if(ň.
TryGetValue("Ice",out ʝ)&&ʝ.č>0){double Ò=ʝ.č-ʝ.ċ;if(Ò>=1.0){Ţ.Append('|').Append("Ore.Ice:").Append((int)Math.Ceiling(Ò));}}}IGC.
SendBroadcastMessage(ƪ,Ţ.ToString());}void ʜ(StringBuilder è,Dictionary<string,ě>ʞ,string ʟ,bool ʠ){foreach(var c in ʞ){if(ʠ&&c.Value.Ė<=0)
continue;int Ù=ʠ?c.Value.Ě:c.Value.Ĕ;if(Ù>0){è.Append('|');è.Append(ʟ).Append('.').Append(c.Key).Append(':').Append(Ù);}}}void ǧ
(string ʡ,List<IMyCargoContainer>ʢ){double ʣ=0,ʤ=0;foreach(var Ǔ in ʢ){var Ǖ=Ǔ.GetInventory(0);if(Ǖ==null)continue;ʣ+=(
double)Ǖ.CurrentVolume;ʤ+=(double)Ǖ.MaxVolume;}if(ʤ>0&&ʣ/ʤ>=0.9)Ǉ($"{ʡ} containers {ʣ/ʤ*100:F0}% full.");}void ǵ(List<
IMyCargoContainer>ʢ,string ʥ,string ʦ){int T=ʢ.Count;if(T<=1)return;Ĳ.Clear();double ʧ=0;for(int U=0;U<T;U++){double ɿ=(double)ʢ[U].
GetInventory(0).MaxVolume;Ĳ.Add(ɿ);ʧ+=ɿ;}Į.Clear();for(int U=0;U<T;U++){ţ.Clear();ʢ[U].GetInventory(0).GetItems(ţ);foreach(var ǲ in
ţ){string Ʉ=ǲ.Type.TypeId;if(Ʉ!=ʥ&&(ʦ==null||Ʉ!=ʦ))continue;double ʨ;Į.TryGetValue(ǲ.Type,out ʨ);Į[ǲ.Type]=ʨ+(double)ǲ.
Amount;}}foreach(var c in Į){if(Ǐ())return;MyItemType ʌ=c.Key;double ʩ=c.Value;if(ʩ<1.0)continue;ı.Clear();į.Clear();İ.Clear()
;for(int U=0;U<T;U++){double ʪ=ʧ>0?ʩ*(Ĳ[U]/ʧ):ʩ/T;ţ.Clear();ʢ[U].GetInventory(0).GetItems(ţ);double ʫ=0;foreach(var ǲ in
ţ)if(ǲ.Type==ʌ)ʫ+=(double)ǲ.Amount;ı.Add(ʫ);if(ʫ>ʪ+0.5)į.Add(U);else if(ʫ<ʪ-0.5)İ.Add(U);}int ǩ=0;for(int ȕ=0;ȕ<į.Count&&
ǩ<İ.Count;ȕ++){int ʐ=į[ȕ];double ʬ=ʧ>0?ʩ*(Ĳ[ʐ]/ʧ):ʩ/T;double ɴ=ı[ʐ]-ʬ;while(ɴ>0.5&&ǩ<İ.Count){int ʓ=İ[ǩ];double ʭ=ʧ>0?ʩ*(
Ĳ[ʓ]/ʧ):ʩ/T;double ʮ=ʭ-ı[ʓ];if(ʮ<=0.5){ǩ++;continue;}double ʑ=Math.Floor(Math.Min(ɴ,ʮ));if(ʑ<1.0){ǩ++;continue;}var ǰ=ʢ[ʐ
].GetInventory(0);var ʯ=ʢ[ʓ].GetInventory(0);if(ʯ.IsFull){ǩ++;continue;}ţ.Clear();ǰ.GetItems(ţ);bool ʍ=false;foreach(var
ǲ in ţ){if(ǲ.Type!=ʌ)continue;double ǿ=Math.Floor(Math.Min((double)ǲ.Amount,ʑ));if(ǰ.TransferItemTo(ʯ,ǲ,(MyFixedPoint)ǿ))
{ı[ʐ]-=ǿ;ı[ʓ]+=ǿ;ɴ-=ǿ;ʍ=true;}break;}if(!ʍ)break;if(ı[ʓ]>=ʭ-0.5)ǩ++;}}}}void Ǵ(IMyInventory ʇ,bool ʰ=false){if(ĥ.Count>0)
{Ǫ(ʇ,Ɵ,ĥ);Ǫ(ʇ,Ơ,ĥ);}if(Ħ.Count>0)Ǫ(ʇ,ơ,Ħ);if(ħ.Count>0)Ǫ(ʇ,Ƣ,ħ);if(Ĩ.Count>0){Ǫ(ʇ,ƣ,Ĩ);Ǫ(ʇ,Ʀ,Ĩ);}if(!ʰ){var ʱ=Ĩ;if(ʱ.
Count>0){Ǫ(ʇ,Ƥ,ʱ);Ǫ(ʇ,ƥ,ʱ);}}}void Ǉ(string ɺ){Echo($"WARN: {ɺ}");Ś.Add(ɺ);}void Ȁ<ʲ>(List<ʲ>ʳ,Func<ʲ,bool>ʴ)where ʲ:class,
IMyTerminalBlock{GridTerminalSystem.GetBlocksOfType(ʳ,ʴ);}void ȑ<ʲ>(List<ʲ>ʳ)where ʲ:class,IMyTerminalBlock{GridTerminalSystem.
GetBlocksOfType(ʳ);}bool Ǐ(){return Runtime.CurrentInstructionCount>38000;}bool Ǩ<ʲ>(List<ʲ>ʳ,Action<ʲ>ʵ){while(ľ<ʳ.Count){if(Ǐ())
return false;ʵ(ʳ[ľ++]);}ľ=0;return true;}bool ǳ(List<IMyCargoContainer>ʶ,string ɩ,string ɪ=null){if(ʶ.Count==0){ľ=0;return
true;}int ɨ=0;foreach(var ʇ in Ŵ){if(ʇ==ʶ)continue;foreach(var Ǔ in ʇ){if(ɨ>=ľ){if(Ǐ()){ľ=ɨ;return false;}if(!ĳ.Contains(Ǔ))
{var Ǖ=Ǔ.GetInventory(0);Ǫ(Ǖ,ɩ,ʶ);if(ɪ!=null)Ǫ(Ǖ,ɪ,ʶ);}}ɨ++;}}ľ=0;return true;}bool Ȓ(string T){return T.Contains(Ĝ.Æ)||T
.Contains(Ĝ.Ç)||T.Contains(Ĝ.Å)||T.Contains(Ĝ.Á)||T.Contains(Ĝ.Â)||T.Contains(Ĝ.Ã)||T.Contains(Ĝ.Ä);}void ȯ(string ß,
double ȃ){if(ß=="")return;int ʖ;Ş.TryGetValue(ß,out ʖ);Ş[ß]=ʖ+(int)Math.Ceiling(ȃ);}Ĉ Ȕ(string ʷ){Ĉ ȕ;if(!Ű.TryGetValue(ʷ,out
ȕ)){ȕ=new Ĉ{Ā=ʷ};Ű[ʷ]=ȕ;ů.Add(ȕ);}return ȕ;}void ʉ(){bool ʸ=Ľ<6||Ľ>11;Ľ=1;ş=0;ľ=0;if(ʸ)ǃ();}bool ǔ(IMyTerminalBlock Ǒ)=>Ǒ
.IsSameConstructAs(Me);bool Ȑ(IMyTerminalBlock Ǒ)=>Ǒ.CustomName.Contains(Ĝ.Ê);bool ȁ(IMyTerminalBlock Ǒ)=>ǔ(Ǒ)&&!Ȑ(Ǒ);
bool Ȃ(IMyCubeBlock Ǒ)=>Ǒ.BlockDefinition.SubtypeId.IndexOf("SurvivalKit",D)>=0;void Ǫ(IMyInventory ʇ,string ʹ,List<
IMyCargoContainer>ʺ){ţ.Clear();ʇ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type.TypeId!=ʹ)continue;foreach(var ʶ in ʺ){var ʯ=ʶ.GetInventory(0)
;if(ʯ.IsFull)continue;if(ʇ.TransferItemTo(ʯ,ǲ,null))break;}}}void Ƕ(){foreach(var Ǔ in Ĭ)Ǵ(Ǔ.GetInventory(0));foreach(var
Ǔ in ī)Ǵ(Ǔ.GetInventory(0),ʰ:true);foreach(var Ǒ in ĸ)Ǵ(Ǒ.GetInventory(0));foreach(var Ǒ in Ĺ)Ǵ(Ǒ.GetInventory(0));
foreach(var Ǒ in ĺ)Ǵ(Ǒ.GetInventory(0));foreach(var Ǒ in ĵ)Ǵ(Ǒ.GetInventory(0));if(Ĝ.º&&Ĩ.Count>0)foreach(var Ǒ in ķ){Ǫ(Ǒ.
GetInventory(0),Ƥ,Ĩ);Ǫ(Ǒ.GetInventory(0),ƥ,Ĩ);}}void Ƿ(){if(ī.Count==0)return;ĭ.Clear();foreach(var Ǔ in ĩ){if(Ǔ.CustomName.Contains
(Ĝ.Æ))continue;ĭ.Add(Ǔ);}foreach(var Ǔ in Ĭ)ĭ.Add(Ǔ);var Ð=new MyIni();foreach(var Ǔ in ī){if(string.IsNullOrWhiteSpace(Ǔ
.CustomData)){Ǔ.CustomData=ʻ();continue;}MyIniParseResult b;if(!Ð.TryParse(Ǔ.CustomData,out b))continue;var ʶ=Ǔ.
GetInventory(0);ʼ(Ð,"LoadOut.Components",ơ,ʶ,ĭ);ʼ(Ð,"LoadOut.Ingots",Ơ,ʶ,ĭ);ʼ(Ð,"LoadOut.Ores",Ɵ,ʶ,ĭ);ʼ(Ð,"LoadOut.Ammo",Ƣ,ʶ,ĭ);ʼ(Ð,
"LoadOut.Tools",ƣ,ʶ,ĭ);ʼ(Ð,"LoadOut.Consumables",Ʀ,ʶ,ĭ);ʼ(Ð,"LoadOut.Bottles",Ƥ,ʶ,ĭ,ń);ʼ(Ð,"LoadOut.Bottles",ƥ,ʶ,ĭ,ń);if(Ð.Get(
"LoadOut","UnloadExtra").ToBoolean(false))ʽ(Ð,ʶ);}}void ʽ(MyIni Ð,IMyInventory ʶ){Ĥ.Clear();ʾ(Ð,"LoadOut.Components",ơ);ʾ(Ð,
"LoadOut.Ingots",Ơ);ʾ(Ð,"LoadOut.Ores",Ɵ);ʾ(Ð,"LoadOut.Ammo",Ƣ);ʾ(Ð,"LoadOut.Tools",ƣ);ʾ(Ð,"LoadOut.Consumables",Ʀ);ʾ(Ð,
"LoadOut.Bottles",Ƥ);ʾ(Ð,"LoadOut.Bottles",ƥ);ţ.Clear();ʶ.GetItems(ţ);for(int U=ţ.Count-1;U>=0;U--){var ǲ=ţ[U];if(Ĥ.Contains(ǲ.Type))
continue;string Ʉ=ǲ.Type.TypeId;List<IMyCargoContainer>ʿ;if(Ʉ==Ɵ||Ʉ==Ơ)ʿ=ĥ;else if(Ʉ==ơ)ʿ=Ħ;else if(Ʉ==Ƣ)ʿ=ħ;else ʿ=Ĩ;foreach(
var Ò in ʿ){var Ǖ=Ò.GetInventory(0);if(!Ǖ.IsFull&&ʶ.TransferItemTo(Ǖ,ǲ,null))break;}}}void ʾ(MyIni Ð,string ɉ,string ʹ){Ť.
Clear();Ð.GetKeys(ɉ,Ť);foreach(var Ó in Ť)if(Ð.Get(Ó).ToDouble(0)>0)Ĥ.Add(new MyItemType(ʹ,Ó.Name));}void ʼ(MyIni Ð,string ɉ,
string ʹ,IMyInventory ʶ,List<IMyCargoContainer>ˀ,List<IMyGasGenerator>ˁ=null){Ť.Clear();Ð.GetKeys(ɉ,Ť);foreach(var Þ in Ť){
double ˆ=Ð.Get(Þ).ToDouble(0);if(ˆ<=0)continue;var ʌ=new MyItemType(ʹ,Þ.Name);double ʫ=0;ţ.Clear();ʶ.GetItems(ţ);foreach(var ǲ
in ţ)if(ǲ.Type==ʌ)ʫ+=(double)ǲ.Amount;double ˇ=ˆ-ʫ;if(ˇ<=0)continue;foreach(var ʇ in ˀ){if(ˇ<=0||ʶ.IsFull)break;var ǰ=ʇ.
GetInventory(0);ţ.Clear();ǰ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type!=ʌ)continue;double ʑ=Math.Min((double)ǲ.Amount,ˇ);if(ǰ.
TransferItemTo(ʶ,ǲ,(MyFixedPoint)ʑ))ˇ-=ʑ;break;}}if(ˁ!=null){foreach(var ˈ in ˁ){if(ˇ<=0||ʶ.IsFull)break;var ǰ=ˈ.GetInventory(0);ţ.
Clear();ǰ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type!=ʌ)continue;double ʑ=Math.Min((double)ǲ.Amount,ˇ);if(ǰ.TransferItemTo(ʶ,ǲ
,(MyFixedPoint)ʑ))ˇ-=ʑ;break;}}}}}const string ˉ="[LoadOut]\n; Set UnloadExtra=true to eject any item not listed below back to main storage\nUnloadExtra=false\n\n[LoadOut.Components]\n; Component=Amount  e.g. SteelPlate=500\n\n[LoadOut.Ingots]\n; Ingot=Amount(kg)  e.g. Iron=5000\n\n[LoadOut.Ores]\n; Ore/Ice=Amount(kg)  e.g. Ice=1000\n\n[LoadOut.Ammo]\n; AmmoMagazine=Amount  e.g. NATO_5p56x45mm=100\n\n[LoadOut.Tools]\n; Tool/Gun=Amount  e.g. WelderItem=1\n\n[LoadOut.Consumables]\n; Medkit=Amount or Powerkit=Amount\n\n[LoadOut.Bottles]\n; OxygenBottle=Amount or HydrogenBottle=Amount\n"
;string ʻ(){return ˉ;}void Ǹ(){if(ĥ.Count==0){Ǉ($"No {Ĝ.Á} containers found.");return;}int ˊ=0;foreach(var ǩ in ĝ){if(!ǩ.
î.IsFunctional)continue;var ˋ=ǩ.ï;ţ.Clear();ˋ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type.TypeId!=Ɵ)continue;if(ǲ.Type.
SubtypeId.Equals(ǩ.ó,D))continue;bool ʍ=false;foreach(var Ǔ in ĥ){var ʯ=Ǔ.GetInventory(0);if(ʯ.IsFull)continue;if(ˋ.
TransferItemTo(ʯ,ǲ,null)){ʍ=true;break;}}if(!ʍ)ˊ++;}}foreach(var ǩ in ĝ)ˊ+=ˌ(ǩ);if(ˊ>0)Ǉ(
$"{ˊ} ore transfer(s) failed (conveyors blocked or inventory full).");}int ˌ(ù ǩ){if(!ǩ.î.IsFunctional||string.IsNullOrEmpty(ǩ.ó))return 0;var ˋ=ǩ.ï;var ˍ=new MyItemType(
"MyObjectBuilder_Ore",ǩ.ó);ţ.Clear();ˋ.GetItems(ţ);bool ˎ=false;double ˏ=0;foreach(var ǲ in ţ){if(ǲ.Type.TypeId!=Ɵ)continue;if(ǲ.Type==ˍ)ˏ+=(
double)ǲ.Amount;else ˎ=true;}if(ˎ||ˏ>=Ĝ.s)return 0;int ː=0;double ˑ=Ĝ.s-ˏ;foreach(var Ǔ in ĥ){if(ˑ<=0)break;var ǰ=Ǔ.
GetInventory(0);ţ.Clear();ǰ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type!=ˍ)continue;double ˠ=(double)ǲ.Amount;double ʑ=Math.Min(ˠ,ˑ);
if(ǰ.TransferItemTo(ˋ,ǲ,(MyFixedPoint)ʑ))ˑ-=ʑ;else ː++;break;}}return ː;}int ˡ=0;void ǁ(){if(ĥ.Count==0||ĝ.Count==0)return
;if(ˡ>=ĝ.Count)ˡ=0;while(ˡ<ĝ.Count){if(Ǐ())return;ˌ(ĝ[ˡ++]);}ˡ=0;}void ǹ(){if(ĥ.Count==0||Ġ.Count==0)return;int ˢ=Ĝ.q;if(
ˢ<=0)return;foreach(var Ƽ in Ġ){if(!Ƽ.ú||!Ƽ.ö)continue;if(Ƽ.î.Mode!=K)continue;if(Ƽ.þ.Count==0)continue;var Ƞ=Ƽ.þ[0];
string ȡ=Ƞ.BlueprintId.SubtypeId.ToString();double ɝ=(double)Ƞ.Amount;Y[]ɞ;if(!Ŝ.TryGetValue(ȡ,out ɞ))continue;var Ǖ=Ƽ.î.
GetInventory(0);double ˣ=Math.Min(ɝ,ˢ);double ɜ=Ĝ.u;if(ɜ<=0)ɜ=1.0;foreach(var Ɋ in ɞ){double ˇ=Ɋ.X*ˣ/ɜ;var ˤ=new MyItemType(Ơ,Ɋ.W);
double ʫ=(double)Ǖ.GetItemAmount(ˤ);double ˬ=ˇ-ʫ;if(ˬ<=0)continue;foreach(var Ǔ in ĥ){if(ˬ<=0)break;var ǰ=Ǔ.GetInventory(0);ţ.
Clear();ǰ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type!=ˤ)continue;double ˠ=(double)ǲ.Amount;double ʑ=Math.Min(ˠ,ˬ);if(ǰ.
TransferItemTo(Ǖ,ǲ,(MyFixedPoint)ʑ))ˬ-=ʑ;break;}}}}}void ǻ(){foreach(var ǩ in ĝ){if(Ĝ.x&&ǩ.î.UseConveyorSystem)ǩ.î.UseConveyorSystem=
false;ǩ.î.Enabled=!string.IsNullOrEmpty(ǩ.ó);}}void y(){if(ń.Count==0)return;if(!Ĝ.y){if(ļ){foreach(var Ǯ in ń){if(Ǯ.CubeGrid
!=Me.CubeGrid)continue;if(!Ǯ.UseConveyorSystem)Ǯ.UseConveyorSystem=true;Ǯ.Enabled=true;}ļ=false;}return;}ļ=true;bool ˮ=
false;foreach(var Ö in Ń)if(Ö.FilledRatio<Ĝ.v){ˮ=true;break;}var Ͱ=new MyItemType(Ɵ,"Ice");bool ͱ=false,Ͳ=false;foreach(var Ǯ
in ń){if(Ǯ.CubeGrid!=Me.CubeGrid)continue;ţ.Clear();Ǯ.GetInventory(0).GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type.TypeId==Ƥ
||ǲ.Type.TypeId==ƥ)ͱ=true;else if(ǲ.Type==Ͱ)Ͳ=true;}}bool ͳ=false;foreach(var Ǔ in ĥ)if((double)Ǔ.GetInventory(0).
GetItemAmount(Ͱ)>0){ͳ=true;break;}bool ʹ=(ˮ||ͱ)&&(ͳ||Ͳ);foreach(var Ǯ in ń){if(Ǯ.CubeGrid!=Me.CubeGrid)continue;if(Ǯ.
UseConveyorSystem)Ǯ.UseConveyorSystem=false;Ǯ.Enabled=ʹ;if(ʹ)Ͷ(Ǯ,Ͱ);else if(ĥ.Count>0)Ǫ(Ǯ.GetInventory(0),Ɵ,ĥ);}}void Ͷ(IMyGasGenerator Ǯ
,MyItemType Ͱ){if(ĥ.Count==0)return;var Ǖ=Ǯ.GetInventory(0);double ˑ=Ĝ.s-(double)Ǖ.GetItemAmount(Ͱ);if(ˑ<=0)return;
foreach(var Ǔ in ĥ){if(ˑ<=0||Ǐ())break;var ǰ=Ǔ.GetInventory(0);ţ.Clear();ǰ.GetItems(ţ);foreach(var ǲ in ţ){if(ǲ.Type!=Ͱ)
continue;double ʑ=Math.Min((double)ǲ.Amount,ˑ);if(ǰ.TransferItemTo(Ǖ,ǲ,(MyFixedPoint)ʑ))ˑ-=ʑ;break;}}}string ͺ(){string ͷ=Ľ>0?
$"scan {Ľ}/28":$"next {Math.Max(0,(int)((Ĝ.p*100.0-ş)/1000.0))}s";return
$"v{A}  |  Ref: {ĝ.Count}  |  Asm: {Ğ.Count}  |  Proto: {Ġ.Count}  |  Gen: {ń.Count}  |  {ͷ}";}void Ƶ(){ͻ(Ň);int ͼ=0;foreach(var Ȍ in Ł)if(Ȍ.ì.Count>0)ͼ++;if(ł.Count>0)ͼ++;if(ͼ==0)return;int ʪ=Ŀ%ͼ;Ŀ++;string ͽ=ͺ()
;int ɨ=0;foreach(var Ȍ in Ł){if(Ȍ.ì.Count==0)continue;if(ɨ==ʪ){Ɣ.Clear();Ά(Ȍ.ë);foreach(var Ή in Ȍ.ì)Έ(Ή,Ɣ,ͽ);return;}ɨ++
;}if(ł.Count>0&&ɨ==ʪ){Ɣ.Clear();Ί();foreach(var Ή in ł)Έ(Ή,Ɣ,ͽ);}}void Ά(string[]Ό){Ύ("PRODUCTION MANAGEMENT",Ώ(Š));
foreach(var ΐ in Ό){switch(ΐ.Trim()){case"Flow":Α();break;case"Deficit":Β();break;case"Ore":Γ();break;case"Components":Δ(
"-- COMPONENTS vs QUOTA --",Ŕ,Ɨ,false);break;case"Ammo":Δ("-- AMMO vs QUOTA --",ŕ);break;case"Processing":Ε();break;case"Ingots":Ζ();break;case
"Projectors":Η();break;case"Gear":Δ("-- GEAR vs QUOTA --",Ŗ);break;case"Prototech":Θ();break;case"Storage":Ι();break;case
"ConnectedGrids":Κ();break;case"DefSurAmt":Λ();break;}}}void Ξ(List<ě>ʳ,ref bool Μ,ref bool Ν){foreach(var Ȧ in ʳ){if(Ȧ.Ė<=0)continue;Μ=
true;if(Ȧ.Ĕ<Ȧ.Ė)Ν=false;}}void Α(){Ο();bool Π=false;foreach(var c in ň)if(c.Value.ċ>0){Π=true;break;}Ɠ[0]=new ƒ{Ɛ="ORE",Ƒ=""
,Ɓ=Π?Ÿ:Ż};int Ρ=0,Σ=0;foreach(var ǩ in ĝ){if(ǩ.ö)Ρ++;if(ǩ.ö&&ǩ.ô!="")Σ++;}Ɠ[1]=new ƒ{Ɛ="REF",Ƒ=ĝ.Count.ToString(),Ɓ=ĝ.
Count==0?Ż:(Σ>0?Ÿ:(Ρ>0?Ź:Ż))};bool Τ=false,Υ=true;ť.Clear();foreach(var R in ŏ){if(R.č<=0||!ť.Add(R.Ċ))continue;Τ=true;if(R.Č
<R.č)Υ=false;}Ɠ[2]=new ƒ{Ɛ="ING",Ƒ="",Ɓ=!Τ?Ż:(Υ?Ÿ:Ź)};int Φ=0,Χ=0;foreach(var Ƽ in Ğ){if(Ƽ.ö)Φ++;if(Ƽ.ö&&Ƽ.û!="")Χ++;}Ɠ[3
]=new ƒ{Ɛ="ASM",Ƒ=Ğ.Count.ToString(),Ɓ=Ğ.Count==0?Ż:(Χ>0?Ÿ:(Φ>0?Ź:Ż))};bool Ψ=false,Ω=true;Ξ(Ŕ,ref Ψ,ref Ω);Ξ(ŕ,ref Ψ,ref
Ω);Ξ(Ŗ,ref Ψ,ref Ω);Ɠ[4]=new ƒ{Ɛ="CMP",Ƒ="",Ɓ=!Ψ?Ż:(Ω?Ÿ:Ź)};Ɣ.Add(new Ə("",Ž){Ƈ=true});}void Λ(){Ο();Ŧ.Clear();foreach(
var R in ŏ)if(R.ĉ.Equals(R.Ċ,D))Ŧ.Add(R.Ċ);var Ϊ=new List<string>();var Ϋ=new List<string>();ť.Clear();foreach(var R in ŏ){
if(!R.ĉ.Equals(R.Ċ,D)&&Ŧ.Contains(R.Ċ))continue;if(R.Č<=0)continue;if(!ť.Add(R.Ċ))continue;string έ=ά(R.ĉ.Equals("Ice",D)?
"Ice":Ɂ(R.Ċ),11);double ˇ;Ŋ.TryGetValue(R.Ċ,out ˇ);double ή=R.Č-ˇ;if(R.č>0){double Ò=R.č-ή;double ί=ή-R.č;if(Ò>=1.0)Ϊ.Add(
$"{έ,-11} ({ΰ(Math.Ceiling(Ò))})");if(ί>=1.0)Ϋ.Add($"{έ,-11}  {ΰ(Math.Round(ί))}");}else{if(ή>=1.0)Ϋ.Add($"{έ,-11}  {ΰ(Math.Round(ή))}");if(ή<=-1.0)Ϊ.Add
($"{έ,-11} ({ΰ(Math.Ceiling(-ή))})");}}foreach(var c in Ŋ){if(ť.Contains(c.Key)||c.Value<=0)continue;double ʫ;ŉ.
TryGetValue(c.Key,out ʫ);double Ò=c.Value-ʫ;if(Ò>=1.0)Ϊ.Add($"{ά(Ɂ(c.Key),11),-11} ({ΰ(Math.Ceiling(Ò))})");}α(
"-- DEFICIT AND SURPLUS AMOUNTS --",ŷ);if(Ϊ.Count==0&&Ϋ.Count==0){α("All ingots on target",Ż);return;}β($"{"Deficits",-22}Surplus",Ż);int γ=Math.Max(Ϊ.
Count,Ϋ.Count);for(int U=0;U<γ;U++){string δ=U<Ϊ.Count?Ϊ[U]:"";string ε=U<Ϋ.Count?Ϋ[U]:"";β($"{δ,-22}{ε}",Ž);}}void Β(){Ο();α
("-- DEFICITS --",ŷ);bool Μ=false;ť.Clear();foreach(var R in ŏ){if(R.č<=0)continue;if(!ť.Add(R.Ċ))continue;if(R.č-R.Č<1.0
)continue;Μ=true;double ζ=R.Č/R.č;Color Ƌ=R.Đ?ź:(ζ<0.4?Ź:Ž);Color ʋ=R.Đ?ź:(ζ>=0.8?Ÿ:Ź);string η=Ɂ(R.Ċ);string έ=ά(η,12);
string ǅ=("-"+ΰ(R.č-R.Č)).PadLeft(7);θ($"{έ,-12} {ǅ}",Ƌ,Ɩ+R.Ċ,-1f,ʋ);}if(ι(Ŕ,Ɨ,true,false,false))Μ=true;if(ι(ŕ,"",true,false,
false))Μ=true;if(ι(Ŗ,"",true,false,false))Μ=true;if(ι(ŗ,"",true,false,false))Μ=true;if(!Μ)α("All quotas met",Ÿ);}void Γ(){Ο()
;α("-- ORE & INGOTS vs QUOTA --",ŷ);ť.Clear();Ŧ.Clear();foreach(var R in ŏ)if(R.ĉ.Equals(R.Ċ,D))Ŧ.Add(R.Ċ);foreach(var R
in ŏ){if(!R.ĉ.Equals(R.Ċ,D)&&Ŧ.Contains(R.Ċ))continue;if(!ť.Add(R.Ċ))continue;bool κ=R.č>0;Color Ƌ;string λ,μ,ƌ;if(κ){
double ζ=R.Č/R.č;λ=R.Đ?" !":"";Ƌ=R.Đ?ź:(ζ<0.4?Ź:(R.ď?ż:Ž));μ=$"{ΰ(R.Č),7}/{ΰ(R.č),-7}";ƌ=R.ĉ.Equals("Ice",D)?ƕ+"Ice":Ɩ+R.Ċ;}
else{λ="";Ƌ=R.ď?ż:Ž;μ=$"{ΰ(R.Č),7}        ";ƌ=ƕ+R.ĉ;}bool ν=!R.ĉ.Equals(R.Ċ,D);string ξ=ν?R.ĉ+"/"+R.Ċ:R.ĉ;string ο=ά(ξ,12);
string π=$"{ΰ(R.ċ),7}";Color ʋ=!κ?default(Color):(R.Đ?ź:(R.Č/R.č>=0.8?Ÿ:Ź));double ρ=κ?Math.Min(R.Č/R.č,1.0):-1.0;string ς=κ?λ
:"";if(ρ>=0)θ($"{ο,-12} {π} {μ}",Ƌ,ƌ,(float)ρ,ʋ,ς);else σ($"{ο,-12} {π} {μ}",Ƌ,ƌ,"");}}void Ε(){Ο();α(
"-- PROCESSING NOW --",ŷ);foreach(var ǩ in ĝ){string τ="["+(ǩ.ñ?"Y":"")+(ǩ.ò?"S":"")+"]";string Ȅ=ά(ǩ.î.CustomName,9);Color υ=ǩ.ñ?ż:(ǩ.ò?Ŷ:Ž);
if(!ǩ.ö){σ($"{τ} {Ȅ,-12} OFFLINE",ź,Ƙ,"");continue;}if(ǩ.ô==""){σ($"{τ} {Ȅ,-12} (idle)",Ż,Ƙ,"");continue;}string φ=ǩ.ô.
Equals(ǩ.ó,D)?"~":"!";Color χ=Ĝ.n.Contains(ǩ.ô)?ż:υ;float ψ=(float)Math.Min(ǩ.õ/Ĝ.s,1.0);θ($"{τ} {Ȅ,-9} {ǩ.ô,-9}",χ,Ƙ,ψ,
default(Color),φ);}ω(Ğ,false);if(ń.Count>0){int ϊ=0;foreach(var Ǯ in ń)if(Ǯ.IsWorking)ϊ++;Color ϋ=ϊ==ń.Count?Ÿ:(ϊ==0?Ż:Ź);σ(
$"Gas Gens: {ϊ}/{ń.Count} online",ϋ,ƚ,"");}}void ω(List<ÿ>ɰ,bool ɛ){int ό=0,ύ=0;foreach(var Ƽ in ɰ){if(!Ƽ.ú){if(Ƽ.ö)ό++;else ύ++;continue;}string Ȅ=ά(Ƽ.î
.CustomName,11);bool ώ=Ƽ.î.Mode==MyAssemblerMode.Disassembly;string Ϗ=ώ?"[D]":"[M]";if(!Ƽ.ö){σ($"{Ϗ} {Ȅ,-11} OFFLINE",ź,ƙ
,"");continue;}if(Ƽ.û==""){σ($"{Ϗ} {Ȅ,-11} (idle)",Ż,ƙ,"");continue;}string ϐ=Ɂ(ɛ?Ȳ(Ƽ.û):ɂ(Ƽ.û));string ϑ=Ƽ.ý>1?
$"x{Ƽ.ü:F0} [{Ƽ.ý}]":$"x{Ƽ.ü:F0}";σ($"{Ϗ} {Ȅ,-11} {ϐ,-12} {ϑ}",ώ?Ź:Ÿ,ƙ,"");}if(!ɛ&&ό+ύ>0){Color ϒ=ύ>0?Ź:Ż;string ϓ=ύ>0?
$"Slaves: {ό}/{ό+ύ} online":$"Slaves: {ό} online";α($"    {ϓ}",ϒ);}}bool ι(List<ě>ɮ,string ϔ="",bool ϕ=false,bool ϖ=true,bool ϗ=true){bool Μ=false;
foreach(var Ȧ in ɮ){if(Ȧ.Ė<=0)continue;if(ϕ&&Ȧ.ę<=0)continue;if(Ĝ.o.Contains(Ȧ.Ē))continue;Μ=true;double ζ=(double)Ȧ.Ĕ/Ȧ.Ė;
float Ϙ=(float)Math.Min((double)Ȧ.ĕ/Ȧ.Ė,Math.Max(0.0,1.0-Math.Min(ζ,1.0)));bool ϙ=!ϕ&&Ȧ.ĕ>0&&ζ<1.0&&Ȧ.Ĕ+Ȧ.ĕ>=Ȧ.Ė;string λ=ϖ&&
Ȧ.Đ?"!":"";Color Ƌ=Ȧ.Đ?ź:(ϙ?Ÿ:(ζ<0.4?Ź:Ž));Color ʋ=Ȧ.Đ?ź:Ϛ(ζ);string η=Ɂ(Ȧ.Ē);string έ=ά(η,12);string à=ϕ?("-"+ΰ(Ȧ.ę)).
PadLeft(7):$"{ΰ((double)Ȧ.Ĕ),7}/{ΰ(Ȧ.Ė),-7}";θ($"{έ,-12} {à}",Ƌ,ϔ!=""?ϔ+Ȧ.Ē:"",ϗ?(float)Math.Min(ζ,1.0):-1f,ʋ,λ,Ϙ);}return Μ;}
int Ϝ(List<ě>ɮ){int T=0;foreach(var Ȧ in ɮ){if(Ȧ.Ė<=0||!Ȧ.Đ)continue;if(Ĝ.o.Contains(Ȧ.Ē))continue;T++;string ϛ=Ɂ(Ȧ.Ē);ϛ=ά(
ϛ,12);Ţ.AppendLine($"! {ϛ,-12}{(double)Ȧ.Ĕ/Ȧ.Ė*100,3:F0}%");}return T;}void Ί(){Ύ("WARNINGS",Ώ(Š));Ο();if(Ś.Count==0&&ř.
Count==0&&Ř.Count==0){α("No warnings.",Ÿ);return;}if(Ś.Count>0){α("-- RUNTIME --",ź);foreach(var ϝ in Ś)α($"! {ϝ}",Ź);Ο();}if
(ř.Count>0){α("-- BLUEPRINT NOT FOUND --",ź);α("  (check blueprint name)",Ż);foreach(var â in ř)α($"? {â}",Ź);Ο();}if(Ř.
Count>0){α("-- BLUEPRINT QUEUE FAILED --",ź);α("  (assembler rejected item)",Ż);foreach(var â in Ř)α($"! {â}",Ź);}}void ͻ(
IMyTextSurface Ϟ){Ϟ.ContentType=ContentType.TEXT_AND_IMAGE;Ţ.Clear();Ţ.AppendLine("PROD MGR");Ţ.AppendLine(
$"R:{ĝ.Count} A:{Ğ.Count} P:{Ġ.Count}");int ϟ=0;int Ϡ=0;foreach(var a in Ņ){if(!a.IsProjecting||a.RemainingBlocks<=0)continue;ϟ++;Ϡ+=a.RemainingBlocks;}if(ϟ>0
)Ţ.AppendLine($"PROJ:{ϟ} rem:{Ϡ}");Ţ.AppendLine();int ϡ=0;ť.Clear();foreach(var R in ŏ){if(R.č<=0)continue;if(!ť.Add(R.Ċ)
)continue;if(R.Đ){ϡ++;Ţ.AppendLine($"! {R.Ċ,-8}{R.Č/R.č*100,3:F0}%");}}ϡ+=Ϝ(Ŕ);ϡ+=Ϝ(ŕ);ϡ+=Ϝ(Ŗ);ϡ+=Ϝ(ŗ);Ţ.AppendLine(ϡ==0?
"All OK":$"{ϡ} CRITICAL");Ţ.AppendLine();int Ϣ=Runtime.CurrentInstructionCount;int ϣ=Runtime.MaxInstructionCount;double Ϥ=ϣ>0?(
double)Ϣ/ϣ*100.0:0;Ţ.AppendLine($"Inst:{Ϣ}/{ϣ} {Ϥ:F0}%");Ţ.AppendLine($"Ms  :{Runtime.LastRunTimeMs:F2}");Ϟ.WriteText(Ţ.
ToString());}void α(string Ɗ,Color Ƌ){Ɣ.Add(new Ə(Ɗ,Ƌ));}void β(string Ɗ,Color Ƌ){Ɣ.Add(new Ə(Ɗ,Ƌ){Ɖ=true});}void σ(string Ɗ,
Color Ƌ,string ƌ,string ε){Ɣ.Add(new Ə(Ɗ,Ƌ,ƌ,ε));}void Ο(){Ɣ.Add(new Ə("",Ž){ƅ=true});}void Ύ(string ϥ,string Ϧ){Ɣ.Add(new Ə(
ϥ,Ŷ,"",Ϧ){Ɔ=true});}void θ(string Ɗ,Color Ƌ,string ƌ,float ζ,Color ʋ=default(Color),string ς="",float Ϙ=0f,bool ϧ=false){
Ɣ.Add(new Ə(Ɗ,Ƌ,ƌ,ς,ζ){Ƃ=ʋ,Ƅ=Ϙ,ƈ=ϧ});}Color ϩ(double b,Color Ϩ){return b>=0.95?ź:(b>=0.75?Ź:Ϩ);}Color Ϫ(double b,Color Ϩ)
{return b<0.2?ź:(b<0.4?Ź:Ϩ);}Color Ϛ(double b){return b>=0.8?Ÿ:(b>=0.4?Ź:ź);}string ά(string R,int T){return R.Length>T?R
.Substring(0,T):R;}bool ϫ(List<ě>ɮ){foreach(var Ȧ in ɮ)if(Ȧ.Ė>0)return true;return false;}void Δ(string ϥ,List<ě>ɮ,string
ϔ="",bool Ϭ=true){if(Ϭ&&!ϫ(ɮ))return;Ο();α(ϥ,ŷ);ι(ɮ,ϔ);}void ϯ(string ʟ,double ȗ,double Ȗ,bool ϭ){if(Ȗ<=0)return;double b
=ȗ/Ȗ;bool Ϯ=ϭ?b>=0.95:b<0.2;Color Ƌ=ϭ?ϩ(b,Ž):Ϫ(b,Ž);Color ʋ=ϭ?ϩ(b,Ÿ):Ϫ(b,Ÿ);θ(ʟ,Ƌ,"",(float)Math.Min(b,1.0),ʋ,Ϯ?"!":"");}
static readonly string[]ϰ={"Iron","Nickel","Cobalt","Silicon","Magnesium","Silver","Gold","Platinum","Uranium","Gravel"};void
Ζ(){Ο();α("-- INGOT REQUIREMENTS (QUEUE) --",ŷ);bool Μ=false;bool ϱ=true;for(int U=0;U<ϰ.Length;U++){string d=ϰ[U];double
ˇ;if(!Ŋ.TryGetValue(d,out ˇ)||ˇ<=0)continue;Μ=true;double ʫ;ŉ.TryGetValue(d,out ʫ);bool Ν=ʫ>=ˇ;if(!Ν)ϱ=false;double ζ=ˇ>0
?(ʫ/ˇ<1.0?ʫ/ˇ:1.0):1.0;Color Ƌ=Ν?Ÿ:(ζ>=0.5?Ź:ź);string έ=ά(d,9);Color ʋ=Ν?Ÿ:(ζ>=0.5?Ź:ź);θ(
$"{έ,-9} need:{ΰ(ˇ),7}  have:{ΰ(ʫ),7}",Ƌ,"",(float)ζ,ʋ,Ν?"OK":"LOW!");}foreach(var c in Ŋ){if(c.Value<=0)continue;bool ϲ=false;for(int U=0;U<ϰ.Length;U++)if(ϰ
[U]==c.Key){ϲ=true;break;}if(ϲ)continue;Μ=true;double ʫ;ŉ.TryGetValue(c.Key,out ʫ);bool Ν=ʫ>=c.Value;if(!Ν)ϱ=false;double
ζ=c.Value>0?(ʫ/c.Value<1.0?ʫ/c.Value:1.0):1.0;Color Ƌ=Ν?Ÿ:(ζ>=0.5?Ź:ź);string έ=ά(c.Key,9);θ(
$"{έ,-9} need:{ΰ(c.Value),7}  have:{ΰ(ʫ),7}",Ƌ,"",(float)ζ,Ƌ,Ν?"OK":"LOW!");}if(!Μ){α("  (nothing queued)",Ż);return;}Ο();α(ϱ?"All ingot requirements met":
"Insufficient ingots for queue!",ϱ?Ÿ:ź);}void Η(){if(Ņ.Count==0)return;Ο();α("-- PROJECTORS --",ŷ);bool ϳ=false;foreach(var a in Ņ){string Ȅ=ά(a.
CustomName,14);if(!a.IsFunctional){α($"{Ȅ,-16} OFFLINE",ź);continue;}if(!a.IsProjecting){α($"{Ȅ,-16} (idle)",Ż);continue;}ϳ=true;
int ɖ=a.RemainingBlocks;int ʩ=a.TotalBlocks;int ϴ=a.BuildableBlocksCount;double ζ=ʩ>0?1.0-(double)ɖ/ʩ:1.0;Color Ƌ=ɖ==0?Ÿ:(ϴ
==0?Ź:Ž);Color ʋ=ɖ==0?Ÿ:(ϴ>0?Ÿ:Ź);string ς=ɖ==0?"DONE":(ϴ>0?$"+{ϴ}":"wait");θ($"{Ȅ,-16} {ɖ}/{ʩ}",Ƌ,"",(float)ζ,ʋ,ς);}if(ϳ)
α("  (disassembly suppressed while printing)",Ż);}void Θ(){bool ϵ=ϫ(ŗ);double Ϸ;Ĝ.g.TryGetValue("PrototechScrap",out Ϸ);
double ϸ;ŉ.TryGetValue("PrototechScrap",out ϸ);if(!ϵ&&Ϸ<=0)return;Ο();α("-- PROTOTECH vs QUOTA --",ŷ);if(Ϸ>0){double ζ=Ϸ>0?ϸ/Ϸ
:0;bool Ϲ=ζ<0.2;Color Ƌ=Ϲ?ź:(ζ<0.4?Ź:Ž);Color ʋ=Ϲ?ź:Ϛ(ζ);double ˇ;Ŋ.TryGetValue("PrototechScrap",out ˇ);string λ=Ϲ?"!":(ˇ
>ϸ?"LOW":"");θ($"{"Proto Scrap",-12} {ΰ(ϸ),7}/{ΰ(Ϸ),-7}",Ƌ,"",(float)Math.Min(ζ,1.0),ʋ,λ);}ι(ŗ);ω(Ġ,true);}void Ι(){Ο();α
("-- STORAGE --",ŷ);double Ϻ=0,ϻ=0,ϼ=0,Ͻ=0,Ͼ=0,Ͽ=0,Ѐ=0,Ё=0,Ђ=0,Ѓ=0;int Є=0,Ѕ=0,І=0,Ї=0,Ј=0;foreach(var Ǔ in ĩ){var Ǖ=Ǔ.
GetInventory(0);if(Ǖ==null)continue;double Ù=(double)Ǖ.CurrentVolume,ɗ=(double)Ǖ.MaxVolume;string T=Ǔ.CustomName;if(T.Contains(Ĝ.Á))
{Ϻ+=Ù;ϻ+=ɗ;Є++;}else if(T.Contains(Ĝ.Â)){ϼ+=Ù;Ͻ+=ɗ;Ѕ++;}else if(T.Contains(Ĝ.Ã)){Ͼ+=Ù;Ͽ+=ɗ;І++;}else if(T.Contains(Ĝ.Ä)){
Ѐ+=Ù;Ё+=ɗ;Ї++;}else{Ђ+=Ù;Ѓ+=ɗ;Ј++;}}Љ("OreIng",Ϻ,ϻ,Є,Ɲ);Љ("Comp  ",ϼ,Ͻ,Ѕ,Ɲ);Љ("Ammo  ",Ͼ,Ͽ,І,Ɲ);Љ("Gear  ",Ѐ,Ё,Ї,Ɲ);Љ(
"Other ",Ђ,Ѓ,Ј,Ɲ);double Њ=0,Ћ=0,Ќ=0,Ѝ=0;int Ў=0,Џ=0;foreach(var Ö in Ń){double Ȗ=Ö.Capacity;double ȗ=Ö.FilledRatio*Ȗ;if(Ö.
BlockDefinition.SubtypeId.Contains("Hydrogen")){Ћ+=Ȗ;Њ+=ȗ;Ў++;}else{Ѝ+=Ȗ;Ќ+=ȗ;Џ++;}}if(Ћ>0||Ѝ>0)Ο();А("H2   ",Њ,Ћ,Ў,ƛ);А("O2   ",Ќ,Ѝ,Џ,
Ɯ);}void Љ(string Б,double ʣ,double ʤ,int В=0,string ƌ=""){if(ʤ<=0)return;double b=ʣ/ʤ;θ(
$"{Б,-6}({В}) {ΰ(ʣ),7}/{ΰ(ʤ),-7}",Ž,ƌ,(float)Math.Min(b,1.0),default(Color),b>=0.95?"!":"",0f,true);}void А(string Б,double ȗ,double Ȗ,int В=0,string ƌ=
""){if(Ȗ<=0)return;double b=ȗ/Ȗ;Color Ƌ=Ϫ(b,Ž);θ($"{Б,-6}({В}) {ΰ(ȗ),7}/{ΰ(Ȗ),-7}",Ƌ,ƌ,(float)Math.Min(b,1.0),Ϫ(b,Ÿ),b<0.2
?"!":"");}void Κ(){if(ů.Count==0)return;Ο();α("-- CONNECTED GRIDS --",ŷ);foreach(var ȕ in ů){string έ=ά(ȕ.Ā,14);if(ȕ.Ă>0)
ϯ($"{έ,-14}({ȕ.ć}) {ΰ(ȕ.ā),7}/{ΰ(ȕ.Ă),-7}",ȕ.ā,ȕ.Ă,true);else α($"{έ}",Ż);ϯ($"  H2    {ΰ(ȕ.ă),7}/{ΰ(ȕ.Ą),-7}",ȕ.ă,ȕ.Ą,
false);ϯ($"  O2    {ΰ(ȕ.ą),7}/{ΰ(ȕ.Ć),-7}",ȕ.ą,ȕ.Ć,false);}}Color Г(Color Ǔ,int ȃ){return new Color(Ǔ.R,Ǔ.G,Ǔ.B,(byte)Math.
Min(ȃ,Ǔ.A));}void ȋ(IMyTextSurface R){R.ContentType=ContentType.SCRIPT;R.Script="";R.ScriptBackgroundColor=Color.Black;R.
Font=B;R.FontSize=0.5f;}void ȉ(string Ȅ,string ʡ,int Д,out int ȍ,out int Ȏ){ȍ=-1;Ȏ=0;int Е=Ȅ.IndexOf(ʡ)+ʡ.Length;int Ж=Е;int
З=Ж;while(Ж<Ȅ.Length&&Ȅ[Ж]>='0'&&Ȅ[Ж]<='9')Ж++;if(Ж>З)int.TryParse(Ȅ.Substring(З,Ж-З),out ȍ);if(Ж<Ȅ.Length&&Ȅ[Ж]==':'){Ж
++;int И=Ж;while(Ж<Ȅ.Length&&Ȅ[Ж]>='0'&&Ȅ[Ж]<='9')Ж++;if(Ж>И){int ʐ;if(int.TryParse(Ȅ.Substring(И,Ж-И),out ʐ)&&ʐ>=0&&ʐ<Д)Ȏ
=ʐ;}}}void Έ(IMyTextSurface Ϟ,List<Ə>Й,string ͽ){Ϟ.ContentType=ContentType.SCRIPT;Ϟ.Script="";var К=Ϟ.SurfaceSize;var Л=(
Ϟ.TextureSize-К)*0.5f;var М=Ϟ.DrawFrame();ŀ++;int Н=245+ŀ%10;if(Й.Count==0){М.Dispose();return;}int ʩ=Й.Count;float О=
0.5f;int П=0;for(int U=0;U<ʩ;U++){var Р=Й[U];if(Р.ƅ||Р.Ɔ)continue;if(Р.ž.Length>П)П=Р.ž.Length;}int С=ʩ;for(int U=0;U<ʩ;U++)
{var Р=Й[U];if(!Р.Ɔ&&!Р.Ƈ&&!Р.ƅ&&Р.Ɓ==ŷ){С=U;break;}}var Т=new List<int>();for(int ʳ=С;ʳ<ʩ;ʳ++)if(!Й[ʳ].Ɔ&&!Й[ʳ].Ƈ&&!Й[ʳ]
.ƅ&&Й[ʳ].Ɓ==ŷ)Т.Add(ʳ);int У=Т.Count;var Ф=new float[У];float Х=0f;for(int Ǒ=0;Ǒ<У;Ǒ++){int Ц=Ǒ+1<У?Т[Ǒ+1]:ʩ;for(int ʳ=Т[
Ǒ];ʳ<Ц;ʳ++)Ф[Ǒ]+=Й[ʳ].ƅ?0.5f:1f;Х+=Ф[Ǒ];}int Ч=К.X>=1000f?3:(К.X>=560f?2:1);float Ш=0.5f*21.56f,Щ=0.5f*34.4f,Ъ=Math.Max(
60f,Ш*9f);float Ы=П>0?П*Ш+Щ*1.76f+Ъ+24f:0f;int Ь=1;for(int Ǔ=1;Ǔ<=Ч;Ǔ++){Ь=Ǔ;float Э=К.X/Ǔ;float Ю=Ы>Э?Math.Max(0.25f,0.5f*
Э/Ы):0.5f;float Я=Ю*34.4f;float а=Я*0.92f;float б=0f;for(int ʳ=0;ʳ<С;ʳ++){if(Й[ʳ].Ɔ)б+=Я*1.5f;else if(!Й[ʳ].ƅ)б+=а;else б
+=а*0.4f;}if(С<ʩ)б+=а*0.3f;float ˠ=К.Y-Я*1.25f-8f-б;float в=ˠ>0f?ˠ/а:1f;bool г;if(У>0){float д=Х/Ǔ;int е=0;float ж=0f;г=
true;for(int Ǒ=0;Ǒ<У;Ǒ++){if(е<Ǔ-1&&ж>0f&&ж+Ф[Ǒ]>д*1.15f){е++;ж=0f;}if(ж+Ф[Ǒ]>в){г=false;break;}ж+=Ф[Ǒ];}}else{г=ʩ<=(int)(Ǔ*
в);}if(г)break;}float з=К.X/Ь;if(П>0){float и=О*21.56f,й=О*34.4f,к=Math.Max(60f,и*9f);float л=П*и+й*1.76f+к+24f;if(л>з){О
*=з/л;if(О<0.25f)О=0.25f;}}bool м=з<230f;if(м)Ь=1;float н=О*34.4f;float о=н*0.92f;float п=О*21.56f;float р=о;float с=Math.
Max(1f,н*0.06f);float т=м?Math.Max(34f,п*5f):Math.Max(60f,п*9f);float у=Math.Max(6f,о*0.34f);float ф=8f;float х=Л.X+ф;float
ц=Л.Y+ф;float ч=Л.X+К.X*0.5f;float ш=Л.X+К.X-ф;float щ=Л.Y+К.Y;float ъ=н*1.25f;float ы=щ-ъ;float ь=ц;for(int ʳ=0;ʳ<С;ʳ++)
{var э=Й[ʳ];if(э.Ɔ){if(ь+н*1.5f>ы)break;М.Add(new MySprite(F,э.ž,new Vector2(ч,ь),null,Г(Ŷ,Н),B,I,О*1.15f));ь+=н*1.5f;}
else if(э.Ƈ){float а=К.X>=280f?н*1.5f:н*5f;if(ь+а>ы)break;ю(М,К.X,х,ь,О,н,п,Н);ь+=а+4f;}else if(!э.ƅ){if(ь+о>ы)break;я(М,э,х
,ш,ref ь,м,О,о,п,р,с,т,у,Н);}else{ь+=о*0.4f;}}float ѐ=ь+(С<ʩ?о*0.3f:0f);if(С<ʩ&&Ь<=1){float ё=ѐ;for(int ʳ=С;ʳ<ʩ;ʳ++){if(Й
[ʳ].ƅ){ё+=о;continue;}if(ё+о>ы)break;я(М,Й[ʳ],х,ш,ref ё,м,О,о,п,р,с,т,у,Н);}}else if(С<ʩ){var ђ=new List<int>();for(int ʳ
=С;ʳ<ʩ;ʳ++)if(!Й[ʳ].Ɔ&&!Й[ʳ].Ƈ&&!Й[ʳ].ƅ&&Й[ʳ].Ɓ==ŷ)ђ.Add(ʳ);int ѓ=ђ.Count;var є=new float[ѓ];float ѕ=0f;for(int Ǒ=0;Ǒ<ѓ;Ǒ
++){int і=Ǒ+1<ѓ?ђ[Ǒ+1]:ʩ;float ї=0f;for(int ʳ=ђ[Ǒ];ʳ<і;ʳ++)ї+=Й[ʳ].ƅ?о*0.5f:о;є[Ǒ]=ї;ѕ+=ї;}float ʪ=ѕ/Ь;float ј=п*2f;float
љ=(К.X-ф*2f-ј*(Ь-1))/Ь;int Ƌ=0;float њ=0f;float ћ=ѐ;for(int Ǒ=0;Ǒ<ѓ;Ǒ++){if(Ƌ<Ь-1&&њ>0f&&њ+є[Ǒ]>ʪ*1.15f){Ƌ++;њ=0f;ћ=ѐ;}
float ќ=Л.X+ф+Ƌ*(љ+ј);float ѝ=ќ+љ;int і=Ǒ+1<ѓ?ђ[Ǒ+1]:ʩ;for(int ʳ=ђ[Ǒ];ʳ<і;ʳ++){if(Й[ʳ].ƅ){ћ+=о*0.5f;continue;}if(ћ+о>ы)break;
я(М,Й[ʳ],ќ,ѝ,ref ћ,м,О,о,п,р,с,т,у,Н);}ћ+=о*0.4f;њ+=є[Ǒ];}}if(ͽ!=null&&ͽ.Length>0){float ў=щ-ъ;М.Add(new MySprite(G,
"SquareSimple",new Vector2(ч,ў),new Vector2(К.X-ф*2f,1f),Г(Ż,Н)));string џ=м?("v"+A):ͽ;М.Add(new MySprite(F,џ,new Vector2(х,ў+(ъ-н)*
0.5f+2f),null,Г(Ž,Н),B,H,О*0.85f));}М.Dispose();}void я(MySpriteDrawFrame М,Ə э,float ќ,float ѝ,ref float ь,bool м,float О,
float н,float п,float р,float с,float т,float у,int Н){float Ѡ=(ќ+ѝ)*0.5f;if(э.ƅ){М.Add(new MySprite(G,"SquareSimple",new
Vector2(Ѡ,ь+н*0.5f),new Vector2(ѝ-ќ,с),Г(Ż,Н)));ь+=н;return;}if(э.ž.Length==0){ь+=н;return;}if(э.Ɓ==ŷ){М.Add(new MySprite(F,э.ž
,new Vector2(ќ,ь),null,Г(Ŷ,Н),B,H,О));ь+=н;return;}float ѡ=ќ;if(э.ƀ.Length>0){М.Add(new MySprite(G,э.ƀ,new Vector2(ѡ+р*
0.5f,ь+р*0.5f),new Vector2(р,р),Г(Color.White,Н)));ѡ+=р+4f;}else if(э.Ɖ){ѡ+=р+4f;}if(м){if(э.ƀ.Length==0){string Ö=э.ž.
TrimStart();int Ѣ=Ö.IndexOf(' ');string έ=ά(Ѣ>0?Ö.Substring(0,Ѣ):Ö,4);М.Add(new MySprite(F,έ,new Vector2(ѡ,ь),null,Г(э.Ɓ,Н),B,H,О
));ѡ+=п*4.5f;}if(э.ƃ>=0f){float ѣ=п*4.2f;float Ѥ=ѝ-6f-ѣ-ѡ;if(Ѥ>12f){float ѥ=ь+(н-у)*0.5f;М.Add(new MySprite(G,
"SquareSimple",new Vector2(ѡ+Ѥ*0.5f,ѥ+у*0.5f),new Vector2(Ѥ,у),Г(Ż,Н)));Color Ѧ=э.ƈ?(э.ƃ>=0.95f?ź:(э.ƃ>=0.75f?Ź:Ÿ)):(э.ƃ>=0.8f?Ÿ:(э.ƃ
>=0.4f?Ź:ź));if(э.Ƅ>0f){float ѧ=Math.Min(Ѥ,Ѥ*(э.ƃ+э.Ƅ));if(ѧ>0f)М.Add(new MySprite(G,"SquareSimple",new Vector2(ѡ+ѧ*0.5f,ѥ
+у*0.5f),new Vector2(ѧ,у),Г(new Color(Ŷ.R,Ŷ.G,Ŷ.B,130),Н)));}float Ѩ=Math.Max(Ѥ*э.ƃ,э.ƃ>0f?2f:0f);if(Ѩ>0f)М.Add(new
MySprite(G,"SquareSimple",new Vector2(ѡ+Ѩ*0.5f,ѥ+у*0.5f),new Vector2(Ѩ,у),Г(Ѧ,Н)));}М.Add(new MySprite(F,((int)(э.ƃ*100f))+"%",
new Vector2(ѝ-4f,ь),null,Г(э.Ɓ,Н),B,J,О));}ь+=н;return;}М.Add(new MySprite(F,э.ž,new Vector2(ѡ,ь),null,Г(э.Ɓ,Н),B,H,О));if(
э.ƃ>=0f){float ѩ=ѝ-8f-т;float ѥ=ь+(н-у)*0.5f;М.Add(new MySprite(G,"SquareSimple",new Vector2(ѩ+т*0.5f,ѥ+у*0.5f),new
Vector2(т,у),Г(Ż,Н)));Color Ѫ=э.ƈ?(э.ƃ>=0.95f?ź:(э.ƃ>=0.75f?Ź:Ÿ)):(э.ƃ>=0.8f?Ÿ:(э.ƃ>=0.4f?Ź:ź));if(э.Ƅ>0f){float ѧ=Math.Min(т,т
*(э.ƃ+э.Ƅ));if(ѧ>0f)М.Add(new MySprite(G,"SquareSimple",new Vector2(ѩ+ѧ*0.5f,ѥ+у*0.5f),new Vector2(ѧ,у),Г(new Color(Ŷ.R,Ŷ
.G,Ŷ.B,130),Н)));}float ѫ=Math.Max(т*э.ƃ,э.ƃ>0f?2f:0f);if(ѫ>0f)М.Add(new MySprite(G,"SquareSimple",new Vector2(ѩ+ѫ*0.5f,ѥ
+у*0.5f),new Vector2(ѫ,у),Г(Ѫ,Н)));if(э.ſ.Length>0)М.Add(new MySprite(F,э.ſ,new Vector2(ѩ-4f,ь),null,Г(э.Ɓ,Н),B,J,О*0.85f
));}else if(э.ſ.Length>0){М.Add(new MySprite(F,э.ſ,new Vector2(ѝ-8f-15f*п,ь),null,Г(э.Ɓ,Н),B,H,О));}ь+=н;}void Ѯ(
MySpriteDrawFrame М,Vector2 Ǔ,Vector2 R,float Ö,Color Ƌ){float Ѭ=R.X*0.5f,ѭ=R.Y*0.5f;М.Add(new MySprite(G,"SquareSimple",new Vector2(Ǔ.X,
Ǔ.Y-ѭ+Ö*0.5f),new Vector2(R.X,Ö),Ƌ));М.Add(new MySprite(G,"SquareSimple",new Vector2(Ǔ.X,Ǔ.Y+ѭ-Ö*0.5f),new Vector2(R.X,Ö)
,Ƌ));М.Add(new MySprite(G,"SquareSimple",new Vector2(Ǔ.X-Ѭ+Ö*0.5f,Ǔ.Y),new Vector2(Ö,R.Y),Ƌ));М.Add(new MySprite(G,
"SquareSimple",new Vector2(Ǔ.X+Ѭ-Ö*0.5f,Ǔ.Y),new Vector2(Ö,R.Y),Ƌ));}void ю(MySpriteDrawFrame М,float ѯ,float х,float ь,float О,float
н,float п,int Н){if(ѯ>=280f){float Ѱ=н*1.5f;float ј=п*1.2f;float ѱ=(ѯ-16f-ј*4f)/5f;for(int U=0;U<5;U++){var Ѳ=Ɠ[U];float
ѳ=х+U*(ѱ+ј);float Ѵ=ѳ+ѱ*0.5f;Ѯ(М,new Vector2(Ѵ,ь+Ѱ*0.5f),new Vector2(ѱ,Ѱ),2f,Г(Ѳ.Ɓ,Н));string ѵ=Ѳ.Ƒ.Length>0?Ѳ.Ɛ+" "+Ѳ.Ƒ:
Ѳ.Ɛ;М.Add(new MySprite(F,ѵ,new Vector2(Ѵ,ь+(Ѱ-н)*0.5f),null,Г(Ѳ.Ɓ,Н),B,I,О*0.9f));if(U<4)М.Add(new MySprite(F,">",new
Vector2(ѳ+ѱ+ј*0.5f,ь+(Ѱ-н)*0.5f),null,Г(Ż,Н),B,I,О));}}else{for(int U=0;U<5;U++){var Ѳ=Ɠ[U];float Ѷ=ь+U*н;М.Add(new MySprite(G,
"SquareSimple",new Vector2(х+н*0.3f,Ѷ+н*0.5f),new Vector2(н*0.5f,н*0.5f),Г(Ѳ.Ɓ,Н)));string Ö=Ѳ.Ƒ.Length>0?Ѳ.Ɛ+" "+Ѳ.Ƒ:Ѳ.Ɛ;М.Add(new
MySprite(F,Ö,new Vector2(х+н*0.7f,Ѷ),null,Г(Ѳ.Ɓ,Н),B,H,О));}}}string ΰ(double ѷ){if(ѷ>=1000000)return$"{ѷ/1000000:F1}M";if(ѷ>=
1000)return$"{ѷ/1000:F1}k";return$"{ѷ:F0}";}string Ώ(double Ѹ){int ї=(int)(Ѹ/3600);int ɗ=(int)(Ѹ%3600/60);int R=(int)(Ѹ%60);
return ї>0?$"{ї}:{ɗ:D2}:{R:D2}":$"{ɗ}:{R:D2}";}
