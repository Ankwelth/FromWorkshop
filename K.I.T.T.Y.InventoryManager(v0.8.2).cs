string A="v0.8.1",B="[No KITTY]",C="---";int D,E,F;string G="[KITTY]";double H=0.1,I=0.5,J=1.0,K=0,L=0,M=0,N=1.0;bool O=
false,P=false;float Q=2000f,R=100f,S=400f,T=50f;bool U=true;float V=5000f;bool W=false,X=true;string Y=
"Iron,Nickel,Cobalt,Silicon,Magnesium,Gold,Silver,Platinum,Uranium";float Z=20000f,a=0f,b=0f;HashSet<string>c=new HashSet<string>(StringComparer.OrdinalIgnoreCase);int d=0,e=0,f=0,g=0,h=0
;Dictionary<string,int>i=new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);Dictionary<string,float>j=new
Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);HashSet<string>k=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
bool l=false;int m=0;IMyRefinery n=null;List<IMyRefinery>o=new List<IMyRefinery>();MyItemType p;DateTime q;Dictionary<string
,float>r=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);Dictionary<string,string>s=new Dictionary<string,
string>(StringComparer.OrdinalIgnoreCase),t=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase),u=new Dictionary<
string,string>(StringComparer.OrdinalIgnoreCase);bool v=false;int w=0;IMyAssembler x=null;MyDefinitionId y;MyItemType z;float
ª=0f;Dictionary<MyItemType,float>µ=new Dictionary<MyItemType,float>();DateTime º;Dictionary<string,Dictionary<string,
float>>À=new Dictionary<string,Dictionary<string,float>>(StringComparer.OrdinalIgnoreCase);string Á="Idle";const int Â=12;
TimeSpan Ã=TimeSpan.Zero;TimeSpan Ä=TimeSpan.Zero;double Å=0;private readonly Queue<string>Æ=new Queue<string>();private const
int Ç=40,È=600,É=10;private readonly Queue<double>Ê=new Queue<double>(),Ë=new Queue<double>(),Ì=new Queue<double>();private
const float Í=17.0f;UpdateType Î=UpdateType.None;int Ï=1;private const double Ð=0.25;private const int Ñ=1,Ò=64,Ó=40000;class
â{public IMyTerminalBlock Ô;public IMyTextSurface Õ;public RectangleF Ö;public MyIni Ø=new MyIni();public string Ù="None"
,Ú="None",Û="All",Ü="Grid",Ý="None",Þ=null;public int ß=1;public float à=0.8f;public bool á=true;}List<â>ã=new List<â>();
Dictionary<string,Dictionary<MyItemType,float>>ä=new Dictionary<string,Dictionary<MyItemType,float>>(StringComparer.
OrdinalIgnoreCase);static readonly Dictionary<string,string>å=new Dictionary<string,string>{{"MyObjectBuilder_Ore/Iron","Iron Ore"},{
"MyObjectBuilder_Ore/Nickel","Nickel Ore"},{"MyObjectBuilder_Ore/Cobalt","Cobalt Ore"},{"MyObjectBuilder_Ore/Silicon","Silicon Ore"},{
"MyObjectBuilder_Ore/Magnesium","Magnesium Ore"},{"MyObjectBuilder_Ore/Gold","Gold Ore"},{"MyObjectBuilder_Ore/Silver","Silver Ore"},{
"MyObjectBuilder_Ore/Platinum","Platinum Ore"},{"MyObjectBuilder_Ore/Uranium","Uranium Ore"},{"MyObjectBuilder_Ore/Ice","Ice"},{
"MyObjectBuilder_Ore/Stone","Stone"},{"MyObjectBuilder_Ingot/Iron","Iron Ingot"},{"MyObjectBuilder_Ingot/Nickel","Nickel Ingot"},{
"MyObjectBuilder_Ingot/Cobalt","Cobalt Ingot"},{"MyObjectBuilder_Ingot/Silicon","Silicon Wafer"},{"MyObjectBuilder_Ingot/Magnesium","Magnesium Powder"
},{"MyObjectBuilder_Ingot/Gold","Gold Ingot"},{"MyObjectBuilder_Ingot/Silver","Silver Ingot"},{
"MyObjectBuilder_Ingot/Platinum","Platinum Ingot"},{"MyObjectBuilder_Ingot/Uranium","Uranium Ingot"},{"MyObjectBuilder_Ingot/Stone","Gravel"},{
"MyObjectBuilder_Component/SteelPlate","Steel Plate"},{"MyObjectBuilder_Component/Construction","Construction Comp"},{"MyObjectBuilder_Component/MetalGrid",
"Metal Grid"},{"MyObjectBuilder_Component/InteriorPlate","Interior Plate"},{"MyObjectBuilder_Component/Girder","Girder"},{
"MyObjectBuilder_Component/SmallTube","Small Steel Tube"},{"MyObjectBuilder_Component/LargeTube","Large Steel Tube"},{"MyObjectBuilder_Component/Motor",
"Motor"},{"MyObjectBuilder_Component/Display","Display"},{"MyObjectBuilder_Component/BulletproofGlass","Bulletproof Glass"},{
"MyObjectBuilder_Component/Computer","Computer"},{"MyObjectBuilder_Component/Reactor","Reactor Component"},{"MyObjectBuilder_Component/Thrust",
"Thruster Component"},{"MyObjectBuilder_Component/GravityGenerator","Gravity Comp"},{"MyObjectBuilder_Component/Medical","Medical Component"
},{"MyObjectBuilder_Component/RadioCommunication","Radio Component"},{"MyObjectBuilder_Component/Detector",
"Detector Component"},{"MyObjectBuilder_Component/Explosives","Explosives"},{"MyObjectBuilder_Component/SolarCell","Solar Cell"},{
"MyObjectBuilder_Component/PowerCell","Power Cell"},{"MyObjectBuilder_Component/Superconductor","Superconductor"},{"MyObjectBuilder_Component/Canvas",
"Canvas"},{"MyObjectBuilder_Component/ZoneChip","Zone Chip"},{"MyObjectBuilder_PhysicalGunObject/AngleGrinderItem","Grinder"},{
"MyObjectBuilder_PhysicalGunObject/AngleGrinder2Item","Enahnced Grinder"},{"MyObjectBuilder_PhysicalGunObject/AngleGrinder3Item","Proficient Grinder"},{
"MyObjectBuilder_PhysicalGunObject/AngleGrinder4Item","Elite Grinder"},{"MyObjectBuilder_PhysicalGunObject/WelderItem","Welder"},{
"MyObjectBuilder_PhysicalGunObject/Welder2Item","Enahnced Welder"},{"MyObjectBuilder_PhysicalGunObject/Welder3Item","Proficient Welder"},{
"MyObjectBuilder_PhysicalGunObject/Welder4Item","Elite Welder "},{"MyObjectBuilder_PhysicalGunObject/HandDrillItem","Hand Drill"},{
"MyObjectBuilder_PhysicalGunObject/HandDrill2Item","Enahnced Hand Drill"},{"MyObjectBuilder_PhysicalGunObject/HandDrill3Item","Proficient Hand Drill"},{
"MyObjectBuilder_PhysicalGunObject/HandDrill4Item","Elite Hand Drill"},{"MyObjectBuilder_PhysicalGunObject/AutomaticRifleItem","Rifle"},{
"MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem","Precise Rifle"},{"MyObjectBuilder_PhysicalGunObject/RapidFireAutomaticRifleItem","Rapid-Fire Rifle"},{
"MyObjectBuilder_PhysicalGunObject/UltimateAutomaticRifleItem","Elite Rifle"},{"MyObjectBuilder_PhysicalGunObject/SemiAutoPistolItem","Pistol"},{
"MyObjectBuilder_PhysicalGunObject/FullAutoPistolItem","Full-Auto Pistol"},{"MyObjectBuilder_PhysicalGunObject/ElitePistolItem","Elite Pistol"},{
"MyObjectBuilder_PhysicalGunObject/BasicHandHeldLauncherItem","Rocket Launcher"},{"MyObjectBuilder_PhysicalGunObject/AdvancedHandHeldLauncherItem","Adv. Rocket Launcher"},{
"MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm","5.56x45mm Mag"},{"MyObjectBuilder_AmmoMagazine/NATO_25x184mm","25x184mm Mag"},{
"MyObjectBuilder_AmmoMagazine/Missile200mm","200mm Missile"},{"MyObjectBuilder_AmmoMagazine/SemiAutoPistolMagazine","Pistol Mag"},{
"MyObjectBuilder_AmmoMagazine/FullAutoPistolMagazine","FA Pistol Mag"},{"MyObjectBuilder_AmmoMagazine/ElitePistolMagazine","Elite Pistol Mag"},{
"MyObjectBuilder_AmmoMagazine/AutomaticRifleGun_Mag_20rd","Automatic Rifle Mag (20)"},{"MyObjectBuilder_AmmoMagazine/PreciseAutomaticRifleGun_Mag_5rd","Precise Rifle Mag (5)"},{
"MyObjectBuilder_AmmoMagazine/RapidFireAutomaticRifleGun_Mag_50rd","Rapid Fire Rifle Mag (50)"},{"MyObjectBuilder_AmmoMagazine/UltimateAutomaticRifleGun_Mag_30rd",
"Ultimate Rifle Mag (30)"},{"MyObjectBuilder_ConsumableItem/Medkit","Medical Kit"},{"MyObjectBuilder_ConsumableItem/Powerkit","Power Kit"},{
"MyObjectBuilder_GasContainerObject/HydrogenBottle","Hydrogen Bottle"},{"MyObjectBuilder_OxygenContainerObject/OxygenBottle","Oxygen Bottle"},},æ=new Dictionary<string,
string>(StringComparer.OrdinalIgnoreCase){{"Construction","ConstructionComponent"},{"Girder","GirderComponent"},{"MetalGrid",
"MetalGrid"},{"InteriorPlate","InteriorPlate"},{"SteelPlate","SteelPlate"},{"SmallTube","SmallTube"},{"LargeTube","LargeTube"},{
"Motor","MotorComponent"},{"Display","Display"},{"BulletproofGlass","BulletproofGlass"},{"Computer","ComputerComponent"},{
"Reactor","ReactorComponent"},{"Thrust","ThrustComponent"},{"GravityGenerator","GravityGeneratorComponent"},{"Medical",
"MedicalComponent"},{"RadioCommunication","RadioCommunicationComponent"},{"Detector","DetectorComponent"},{"Explosives",
"ExplosivesComponent"},{"SolarCell","SolarCell"},{"PowerCell","PowerCell"},{"Superconductor","Superconductor"},{"Canvas","Canvas"},{
"AngleGrinderItem","Position0010_AngleGrinder"},{"AngleGrinder2Item","Position0020_AngleGrinder2"},{"AngleGrinder3Item",
"Position0030_AngleGrinder3"},{"AngleGrinder4Item","Position0040_AngleGrinder4"},{"HandDrillItem","Position0050_HandDrill"},{"HandDrill2Item",
"Position0060_HandDrill2"},{"HandDrill3Item","Position0070_HandDrill3"},{"HandDrill4Item","Position0080_HandDrill4"},{"WelderItem",
"Position0090_Welder"},{"Welder2Item","Position0100_Welder2"},{"Welder3Item","Position0110_Welder3"},{"Welder4Item","Position0120_Welder4"},{
"SemiAutoPistolItem","Position0010_SemiAutoPistol"},{"AutomaticRifleItem","Position0040_AutomaticRifle"},{"RapidFireAutomaticRifleItem",
"Position0050_RapidFireAutomaticRifle"},{"PreciseAutomaticRifleItem","Position0060_PreciseAutomaticRifle"},{"UltimateAutomaticRifleItem",
"Position0070_UltimateAutomaticRifle"},{"SemiAutoPistolMagazine","Position0010_SemiAutoPistolMagazine"},{"FullAutoPistolMagazine",
"Position0020_FullAutoPistolMagazine"},{"ElitePistolMagazine","Position0030_ElitePistolMagazine"},{"AutomaticRifleGun_Mag_20rd",
"Position0040_AutomaticRifleGun_Mag_20rd"},{"RapidFireAutomaticRifleGun_Mag_50rd","Position0050_RapidFireAutomaticRifleGun_Mag_50rd"},{
"PreciseAutomaticRifleGun_Mag_5rd","Position0060_PreciseAutomaticRifleGun_Mag_5rd"},{"UltimateAutomaticRifleGun_Mag_30rd",
"Position0070_UltimateAutomaticRifleGun_Mag_30rd"},{"NATO_25x184mm","Position0080_NATO_25x184mmMagazine"},{"AutocannonClip","Position0090_AutocannonClip"},{
"Missile200mm","Position0100_Missile200mm"},{"MediumCalibreAmmo","Position0110_MediumCalibreAmmo"},{"LargeCalibreAmmo",
"Position0120_LargeCalibreAmmo"},{"SmallRailgunAmmo","Position0130_SmallRailgunAmmo"},{"LargeRailgunAmmo","Position0140_LargeRailgunAmmo"},};static
readonly Dictionary<string,float>ç=new Dictionary<string,float>{{"Iron",0.70f},{"Nickel",0.40f},{"Cobalt",0.30f},{"Silicon",
0.70f},{"Magnesium",0.007f},{"Gold",0.01f},{"Silver",0.10f},{"Platinum",0.005f},{"Uranium",0.007f},};static readonly
Dictionary<string,Dictionary<string,float>>è=new Dictionary<string,Dictionary<string,float>>(StringComparer.OrdinalIgnoreCase){{
"SteelPlate",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",21f}}},{"InteriorPlate",new Dictionary<string,
float>(StringComparer.OrdinalIgnoreCase){{"Iron",1f}}},{"ConstructionComponent",new Dictionary<string,float>(StringComparer.
OrdinalIgnoreCase){{"Iron",8f}}},{"GirderComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",6f}}},{
"MetalGrid",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",12f},{"Nickel",5f},{"Cobalt",3f}}},{"SmallTube",
new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",5f}}},{"LargeTube",new Dictionary<string,float>(
StringComparer.OrdinalIgnoreCase){{"Iron",30f}}},{"MotorComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{
"Iron",20f},{"Nickel",5f}}},{"Display",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",1f},{"Silicon",
5f}}},{"BulletproofGlass",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Silicon",15f}}},{
"ComputerComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",0.5f},{"Silicon",0.2f}}},{"ReactorComponent",new
Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",15f},{"Stone",20f},{"Silver",5f}}},{"ThrustComponent",new
Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",30f},{"Cobalt",10f},{"Gold",0.33f},{"Platinum",0.13f}}},{
"GravityGeneratorComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",600f},{"Silver",5f},{"Gold",10f},{"Cobalt",220f}
}},{"MedicalComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",60f},{"Nickel",70f},{
"Silver",20f}}},{"RadioCommunicationComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",8f},{
"Silicon",1f}}},{"DetectorComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",5f},{"Nickel",15f}}},
{"ExplosivesComponent",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Silicon",0.5f},{"Magnesium",2f}}}
,{"SolarCell",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Silicon",3f},{"Nickel",0.1f}}},{
"PowerCell",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",10f},{"Silicon",2f},{"Nickel",2f}}},{
"Superconductor",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",10f},{"Gold",2f}}},{"Canvas",new Dictionary<
string,float>(StringComparer.OrdinalIgnoreCase){{"Magnesium",0.3f}}},{"Position0090_Welder",new Dictionary<string,float>(
StringComparer.OrdinalIgnoreCase){{"Iron",5f},{"Silicon",3f},{"Nickel",1f}}},{"Position0100_Welder2",new Dictionary<string,float>(
StringComparer.OrdinalIgnoreCase){{"MyObjectBuilder_PhysicalGunObject/WelderItem",1f},{"Cobalt",1.5f}}},{"Position0110_Welder3",new
Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"MyObjectBuilder_PhysicalGunObject/Welder2Item",1f},{"Silver",1f}}},{
"Position0120_Welder4",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"MyObjectBuilder_PhysicalGunObject/Welder3Item",1f},{
"Platinum",1f}}},{"Position0010_AngleGrinder",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",6f},{
"Silicon",2f},{"Nickel",2f}}},{"Position0020_AngleGrinder2",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{
"MyObjectBuilder_PhysicalGunObject/AngleGrinderItem",1f},{"Cobalt",1f}}},{"Position0030_AngleGrinder3",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{
"MyObjectBuilder_PhysicalGunObject/AngleGrinder2Item",1f},{"Silver",1f}}},{"Position0040_AngleGrinder4",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{
"MyObjectBuilder_PhysicalGunObject/AngleGrinder3Item",1f},{"Platinum",1f}}},{"Position0050_HandDrill",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"Iron",
8f},{"Silicon",3f},{"Nickel",3f}}},{"Position0060_HandDrill2",new Dictionary<string,float>(StringComparer.
OrdinalIgnoreCase){{"MyObjectBuilder_PhysicalGunObject/HandDrillItem",1f},{"Cobalt",1.5f}}},{"Position0070_HandDrill3",new Dictionary<
string,float>(StringComparer.OrdinalIgnoreCase){{"MyObjectBuilder_PhysicalGunObject/HandDrill2Item",1f},{"Silver",1f}}},{
"Position0080_HandDrill4",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{"MyObjectBuilder_PhysicalGunObject/HandDrill3Item",1f},
{"Platinum",1f}}},{"Position0010_SemiAutoPistolMagazine",new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase){{
"Iron",0.2f},{"Nickel",0.06f}}},{"Position0040_AutomaticRifleGun_Mag_20rd",new Dictionary<string,float>(StringComparer.
OrdinalIgnoreCase){{"Iron",0.6f},{"Nickel",0.24f}}},{"Position0080_NATO_25x184mmMagazine",new Dictionary<string,float>(StringComparer.
OrdinalIgnoreCase){{"Iron",12f},{"Nickel",2f}}},{"Position0090_AutocannonClip",new Dictionary<string,float>(StringComparer.
OrdinalIgnoreCase){{"Iron",25f},{"Nickel",6f}}},{"Position0100_Missile200mm",new Dictionary<string,float>(StringComparer.
OrdinalIgnoreCase){{"Iron",25f},{"Nickel",2f},{"Silicon",1f},{"Magnesium",0.5f}}},};bool ì(string é,out MyDefinitionId ê){string ë;if(!t.
TryGetValue(é,out ë))æ.TryGetValue(é,out ë);if(ë==null){ê=default(MyDefinitionId);return false;}ê=ë.Contains("/")?MyDefinitionId.
Parse(ë):MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/"+ë);return true;}static string ð(MyItemType í){string î=í
.TypeId+"/"+í.SubtypeId;string ï;return å.TryGetValue(î,out ï)?ï:í.SubtypeId;}static string ñ(MyItemType í){string î=í.
TypeId+"/"+í.SubtypeId;return å.ContainsKey(î)?î:null;}int ò=0;IEnumerator<bool>ó;class ö{public IMyTerminalBlock Ô;public
IMyInventory ô;public HashSet<string>õ=new HashSet<string>(StringComparer.OrdinalIgnoreCase);}void ġ(){var ø=new List<
MyInventoryItem>();if(w==1){var ù=new List<IMyAssembler>();GridTerminalSystem.GetBlocksOfType(ù,ú=>ú.CubeGrid==Me.CubeGrid&&!ú.
CustomName.Contains(B));var û=new List<MyProductionItem>();foreach(var ú in ù){ú.GetQueue(û);if(û.Count==0)continue;x=ú;y=û[0].
BlueprintId;µ.Clear();ø.Clear();ú.GetInventory(0).GetItems(ø);foreach(var ý in ø){float ü;µ.TryGetValue(ý.Type,out ü);µ[ý.Type]=ü+(
float)ý.Amount;}ø.Clear();ú.GetInventory(1).GetItems(ø);ª=0f;foreach(var ý in ø)ª+=(float)ý.Amount;º=DateTime.UtcNow;w=2;
return;}return;}if(w==2){if(x==null){w=1;return;}var þ=x.GetInventory(1);ø.Clear();þ.GetItems(ø);float ÿ=0f;MyItemType Ā=
default(MyItemType);foreach(var ý in ø){ÿ+=(float)ý.Amount;Ā=ý.Type;}float ā=ÿ-ª;if(ā<1f||(ā<10f&&(DateTime.UtcNow-º).
TotalSeconds<1.0))return;z=Ā;w=3;return;}if(w==3){var Ă=new Dictionary<MyItemType,float>();var ă=new List<MyInventoryItem>();x.
GetInventory(0).GetItems(ă);foreach(var ý in ă){float Ą;Ă.TryGetValue(ý.Type,out Ą);Ă[ý.Type]=Ą+(float)ý.Amount;}float ą=0f;var Ć=
new List<MyInventoryItem>();x.GetInventory(1).GetItems(Ć);foreach(var ý in Ć)if(ý.Type==z)ą+=(float)ý.Amount;float ā=ą-ª;
var ć=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);if(ā>0f){foreach(var Ĉ in µ){float ĉ=Ĉ.Value;float Ċ;Ă
.TryGetValue(Ĉ.Key,out Ċ);float ċ=ĉ-Ċ;if(ċ>0.001f)ć[Ĉ.Key.TypeId+"/"+Ĉ.Key.SubtypeId]=ċ/ā;}}if(ā<=0f||ć.Count==0){w=1;x=
null;µ.Clear();return;}string Č=y.SubtypeId.ToString();string č=y.TypeId+"/"+Č;t[z.SubtypeId]=č;À[Č]=ć;var Ď=new MyIni();Ď.
TryParse(Me.CustomData);string ď;string Đ=č;if(u.TryGetValue(z.SubtypeId,out ď))Đ+="="+ď;Ď.Set("KITTY Learned",z.SubtypeId,Đ);
foreach(var đ in ć)Ď.Set("KITTY Learned Ingredients",Č+":"+đ.Key,đ.Value);Me.CustomData=Ď.ToString();Ē(z.SubtypeId);x.
ClearQueue();var ē=new List<ö>();var Ĕ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(Ĕ,ĕ=>ĕ!=Me&&ĕ.CustomName.
Contains(G)&&ĕ.HasInventory&&!ĕ.CustomName.Contains(B));foreach(var ĕ in Ĕ){var Ė=new ö{ô=ĕ.GetInventory(0)};foreach(var ė in
new[]{"Ores","Ingots","Components","Ammo","Utility","Other"})if(ĕ.CustomName.IndexOf(ė,StringComparison.OrdinalIgnoreCase)
>=0)Ė.õ.Add(ė);if(Ė.õ.Count>0)ē.Add(Ė);}var Ę=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(Ę,ĕ=>ĕ.
CubeGrid==Me.CubeGrid&&ĕ.HasInventory&&!ĕ.CustomName.Contains(G)&&!ĕ.CustomName.Contains(B)&&!(ĕ is IMyReactor)&&!(ĕ is
IMyGasGenerator)&&!(ĕ is IMyProductionBlock));foreach(int ę in new[]{1,0}){var Ě=x.GetInventory(ę);ø.Clear();Ě.GetItems(ø);for(int ě=ø.
Count-1;ě>=0;ě--){string ĝ=Ĝ(ø[ě].Type);bool Ğ=false;foreach(var Ė in ē){if(!Ė.õ.Contains(ĝ))continue;if(!Ė.ô.CanItemsBeAdded
(ø[ě].Amount,ø[ě].Type))continue;Ě.TransferItemTo(Ė.ô,ě,null,true,ø[ě].Amount);Ğ=true;break;}if(!Ğ){foreach(var ğ in Ę){
var Ġ=ğ.GetInventory(0);if(!Ġ.CanItemsBeAdded(ø[ě].Amount,ø[ě].Type))continue;Ě.TransferItemTo(Ġ,ě,null,true,ø[ě].Amount);
break;}}}}x=null;µ.Clear();y=default(MyDefinitionId);z=default(MyItemType);ª=0f;w=1;}}void ĵ(){if(n==null){l=false;m=0;return
;}var ø=new List<MyInventoryItem>();if(m==1){IMyRefinery Ģ=null;MyItemType ģ=default(MyItemType);bool Ĥ=false;foreach(var
ĥ in o){ø.Clear();ĥ.GetInventory(0).GetItems(ø);if(ø.Count==0)continue;foreach(var ý in ø){if(ç.ContainsKey(ý.Type.
SubtypeId)){Ĥ=true;continue;}if(ģ==default(MyItemType)){ģ=ý.Type;Ģ=ĥ;}}if(Ģ!=null)break;}if(Ģ==null){if(Ĥ)Echo(
"Waiting — only known ores in refineries. Insert the ore you want to learn.");return;}n=Ģ;float Ħ=0f;foreach(var ý in ø)if(ý.Type==ģ)Ħ+=(float)ý.Amount;p=ģ;a=Ħ;q=DateTime.UtcNow;var ħ=new List<
MyInventoryItem>();n.GetInventory(1).GetItems(ħ);b=0f;foreach(var ý in ħ)b+=(float)ý.Amount;m=2;if(Ĥ)Echo(
"Warning: known ore residue alongside "+p.SubtypeId+" in "+n.CustomName+" — results may be inaccurate.");Echo("Ore detected: "+p.SubtypeId+" x"+a+" in "+n.
CustomName+" — waiting for output...");return;}if(m==2){var þ=n.GetInventory(1);þ.GetItems(ø);bool Ĩ=(DateTime.UtcNow-q).
TotalSeconds>60.0;if(ø.Count==0&&!Ĩ)return;if(ø.Count==0){Echo("Smelt learning timed out — no ingots produced. Returning to wait.");
m=1;a=0f;b=0f;p=default(MyItemType);return;}var ĩ=n.GetInventory(0);var Ī=new List<MyInventoryItem>();ĩ.GetItems(Ī);float
ī=0f;foreach(var ý in Ī)if(ý.Type==p)ī+=(float)ý.Amount;float Ĭ=a-ī;float ĭ=-b;MyItemType Į=default(MyItemType);foreach(
var ý in ø){ĭ+=(float)ý.Amount;Į=ý.Type;}if(ĭ<0f)ĭ=0f;if(Ĭ<=0f){if(!Ĩ)return;Echo(
"Smelt learning timed out — ore not consumed. Returning to wait.");m=1;a=0f;b=0f;p=default(MyItemType);return;}float į=ĭ/Ĭ;string İ=p.SubtypeId;string ı=Į.TypeId+"/"+Į.SubtypeId;r[İ]=į;
s[İ]=ı;bool Ĳ=false;foreach(var ĳ in Y.Split(','))if(string.Equals(ĳ.Trim(),İ,StringComparison.OrdinalIgnoreCase)){Ĳ=true
;break;}if(!Ĳ)Y=Y.TrimEnd(',')+","+İ;var Ď=new MyIni();Ď.TryParse(Me.CustomData);Ď.Set("KITTY Learned Smelting",İ,ı+"|"+į
.ToString("R",System.Globalization.CultureInfo.InvariantCulture));Ď.Set("KITTY Refineries","RefineryPriority",Y);Me.
CustomData=Ď.ToString();Echo("Learned: "+İ+" → "+Į.SubtypeId+" @ "+į.ToString("F4")+" yield");var Ę=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType(Ę,ĕ=>ĕ.CubeGrid==Me.CubeGrid&&ĕ.HasInventory&&!ĕ.CustomName.Contains(G)&&!ĕ.CustomName.Contains(B)&&!(ĕ
is IMyReactor)&&!(ĕ is IMyGasGenerator)&&!(ĕ is IMyProductionBlock));foreach(int ę in new[]{1,0}){var Ě=n.GetInventory(ę);
var Ĵ=new List<MyInventoryItem>();Ě.GetItems(Ĵ);for(int ě=Ĵ.Count-1;ě>=0;ě--)foreach(var ğ in Ę){var Ġ=ğ.GetInventory(0);if
(!Ġ.CanItemsBeAdded(Ĵ[ě].Amount,Ĵ[ě].Type))continue;Ě.TransferItemTo(Ġ,ě,null,true,Ĵ[ě].Amount);break;}}foreach(var ĥ in
o)ĥ.UseConveyorSystem=!ĥ.CustomName.Contains(G);o.Clear();n=null;l=false;m=0;p=default(MyItemType);a=0f;b=0f;if(!v)
Runtime.UpdateFrequency&=~UpdateFrequency.Update10;}}static string Ĝ(MyItemType í){switch(í.TypeId){case"MyObjectBuilder_Ore":
return"Ores";case"MyObjectBuilder_Ingot":return"Ingots";case"MyObjectBuilder_Component":return"Components";case
"MyObjectBuilder_AmmoMagazine":return"Ammo";case"MyObjectBuilder_PhysicalGunObject":case"MyObjectBuilder_ConsumableItem":case
"MyObjectBuilder_GasContainerObject":case"MyObjectBuilder_OxygenContainerObject":return"Utility";default:return"Other";}}public
 Program
(){Runtime.UpdateFrequency=UpdateFrequency.Update100;var Ķ=A.Substring(1).Split('.');D=Ķ.Length>=1?int.Parse(Ķ[0]):0;E=Ķ.
Length>=2?int.Parse(Ķ[1]):0;F=Ķ.Length>=3?int.Parse(Ķ[2]):0;var Ď=new MyIni();Ď.TryParse(Me.CustomData);G=Ď.Get("KITTY",
"LCDKeyword").ToString(G);H=Ď.Get("KITTY","BudgetMsPerFrame").ToDouble(H);P=Ď.Get("KITTY Reactors","ManageReactors").ToBoolean(P);Q=
(float)Ď.Get("KITTY Reactors","LargeGrid_LargeReactor_Uranium").ToDouble(Q);R=(float)Ď.Get("KITTY Reactors",
"LargeGrid_SmallReactor_Uranium").ToDouble(R);S=(float)Ď.Get("KITTY Reactors","SmallGrid_LargeReactor_Uranium").ToDouble(S);T=(float)Ď.Get(
"KITTY Reactors","SmallGrid_SmallReactor_Uranium").ToDouble(T);W=Ď.Get("KITTY Refineries","ManageRefineries").ToBoolean(W);string ķ=Ď.
Get("KITTY Refineries","RefineryMode").ToString("Auto");X=!string.Equals(ķ,"Manual",StringComparison.OrdinalIgnoreCase);Y=Ď
.Get("KITTY Refineries","RefineryPriority").ToString(Y);Z=(float)Ď.Get("KITTY Refineries","RefineryOreBuffer").ToDouble(Z
);c.Clear();foreach(var ĸ in Ď.Get("KITTY Refineries","RefineryDisabledOres").ToString("").Split(',')){var Ĺ=ĸ.Trim();if(
Ĺ.Length>0)c.Add(Ĺ);}U=Ď.Get("KITTY Gas Generators","ManageGasGenerators").ToBoolean(U);V=(float)Ď.Get(
"KITTY Gas Generators","IceTarget").ToDouble(V);O=Ď.Get("KITTY Connectors","EmptyConnectedGrids").ToBoolean(O);Ď.Set("KITTY","LCDKeyword",G);Ď
.Set("KITTY","BudgetMsPerFrame",H);Ď.Set("KITTY Reactors","ManageReactors",P);Ď.Set("KITTY Reactors",
"LargeGrid_LargeReactor_Uranium",Q);Ď.Set("KITTY Reactors","LargeGrid_SmallReactor_Uranium",R);Ď.Set("KITTY Reactors","SmallGrid_LargeReactor_Uranium",S
);Ď.Set("KITTY Reactors","SmallGrid_SmallReactor_Uranium",T);Ď.Set("KITTY Refineries","ManageRefineries",W);Ď.Set(
"KITTY Refineries","RefineryMode",X?"Auto":"Manual");Ď.Set("KITTY Refineries","RefineryPriority",Y);Ď.Set("KITTY Refineries",
"RefineryOreBuffer",Z);Ď.Set("KITTY Refineries","RefineryDisabledOres",string.Join(",",c));Ď.Set("KITTY Gas Generators",
"ManageGasGenerators",U);Ď.Set("KITTY Gas Generators","IceTarget",V);Ď.Set("KITTY Connectors","EmptyConnectedGrids",O);Me.CustomData=Ď.
ToString();var ĺ=new List<MyIniKey>();Ď.GetKeys("KITTY Learned",ĺ);foreach(var î in ĺ){string Ļ=Ď.Get(î).ToString("");if(Ļ=="")
continue;int ļ=Ļ.IndexOf('=');string ë=ļ>=0?Ļ.Substring(0,ļ):Ļ;string Ľ=ļ>=0?Ļ.Substring(ļ+1):null;t[î.Name]=ë;if(Ľ!=null&&Ľ.
Length>0)u[î.Name]=Ľ;}var ľ=new List<MyIniKey>();Ď.GetKeys("KITTY Learned Smelting",ľ);foreach(var î in ľ){string Ļ=Ď.Get(î).
ToString("");if(Ļ=="")continue;int ļ=Ļ.IndexOf('|');if(ļ<0)continue;string Į=Ļ.Substring(0,ļ);float Ŀ;if(!float.TryParse(Ļ.
Substring(ļ+1),System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out Ŀ)||Ŀ<=0f)continue;s
[î.Name]=Į;r[î.Name]=Ŀ;}var ŀ=new List<MyIniKey>();Ď.GetKeys("KITTY Learned Ingredients",ŀ);foreach(var î in ŀ){int ļ=î.
Name.IndexOf(':');if(ļ<0)ļ=î.Name.IndexOf('|');if(ļ<0)continue;string Ł=î.Name.Substring(0,ļ);string ł=î.Name.Substring(ļ+1)
;float Ń=(float)Ď.Get(î).ToDouble(0);if(Ń<=0f)continue;if(!À.ContainsKey(Ł))À[Ł]=new Dictionary<string,float>(
StringComparer.OrdinalIgnoreCase);À[Ł][ł]=Ń;}ń();if(ã.Count==0)throw new Exception("No blocks with "+G+" found!");}public void
 Save
(){}void ń(){ã.Clear();var Ņ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(Ņ,ĕ=>ĕ!=Me&&ĕ.CustomName.
Contains(G)&&!ĕ.CustomName.Contains(B));foreach(var ņ in Ņ){var Ň=ņ as IMyTextSurfaceProvider;if(Ň==null||Ň.SurfaceCount==0)
continue;if(!ņ.CustomData.Contains("[KITTY]")){var ň=new MyIni();ň.TryParse(ņ.CustomData);ň.Set("KITTY","Mode","");ň.Set("KITTY"
,"Group","");ň.Set("KITTY","Columns",1);ň.Set("KITTY","FontSize",0.8);ņ.CustomData=ň.ToString();}var ŉ=Ň.GetSurface(0);Ŋ(
ŉ);ã.Add(new â{Ô=ņ,Õ=ŉ,Ö=new RectangleF((ŉ.TextureSize-ŉ.SurfaceSize)/2f,ŉ.SurfaceSize)});}}public void
 Main
(string ŋ,UpdateType Ō){if(ŋ=="learn_on"&&!v){v=true;w=1;var ù=new List<IMyAssembler>();GridTerminalSystem.
GetBlocksOfType(ù,ú=>ú.CubeGrid==Me.CubeGrid&&!ú.CustomName.Contains(B));var Ę=new List<IMyTerminalBlock>();GridTerminalSystem.
GetBlocksOfType(Ę,ĕ=>ĕ.CubeGrid==Me.CubeGrid&&ĕ.HasInventory&&!ĕ.CustomName.Contains(G)&&!ĕ.CustomName.Contains(B)&&!(ĕ is IMyReactor)
&&!(ĕ is IMyGasGenerator)&&!(ĕ is IMyProductionBlock));foreach(var ú in ù){ú.ClearQueue();for(int ę=0;ę<=1;ę++){var Ě=ú.
GetInventory(ę);var ō=new List<MyInventoryItem>();Ě.GetItems(ō);for(int ě=ō.Count-1;ě>=0;ě--)foreach(var ğ in Ę){var Ġ=ğ.
GetInventory(0);if(!Ġ.CanItemsBeAdded(ō[ě].Amount,ō[ě].Type))continue;Ě.TransferItemTo(Ġ,ě,null,true,ō[ě].Amount);break;}}ú.
UseConveyorSystem=false;}Runtime.UpdateFrequency|=UpdateFrequency.Update10;}else if(ŋ=="learn_off"&&v){v=false;w=0;x=null;µ.Clear();var ù
=new List<IMyAssembler>();GridTerminalSystem.GetBlocksOfType(ù,ú=>ú.CubeGrid==Me.CubeGrid&&!ú.CustomName.Contains(B));
foreach(var ú in ù)ú.UseConveyorSystem=!ú.CustomName.Contains(G);if(!l)Runtime.UpdateFrequency&=~UpdateFrequency.Update10;}else
if(ŋ=="learn_smelt_on"&&!l){var Ŏ=new List<IMyRefinery>();GridTerminalSystem.GetBlocksOfType(Ŏ,ŏ=>ŏ.CubeGrid==Me.CubeGrid
&&!ŏ.CustomName.Contains(B));n=Ŏ.Find(ŏ=>ŏ.CustomName.Contains(G))??Ŏ.FirstOrDefault();if(n==null){Echo(
"No refinery found on grid.");return;}var Ę=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(Ę,ĕ=>ĕ.CubeGrid==Me.CubeGrid&&ĕ.
HasInventory&&!ĕ.CustomName.Contains(G)&&!ĕ.CustomName.Contains(B)&&!(ĕ is IMyReactor)&&!(ĕ is IMyGasGenerator)&&!(ĕ is
IMyProductionBlock));o.Clear();foreach(var ĥ in Ŏ){for(int ę=0;ę<=1;ę++){var Ě=ĥ.GetInventory(ę);var ō=new List<MyInventoryItem>();Ě.
GetItems(ō);for(int ě=ō.Count-1;ě>=0;ě--)foreach(var ğ in Ę){var Ġ=ğ.GetInventory(0);if(!Ġ.CanItemsBeAdded(ō[ě].Amount,ō[ě].Type
))continue;Ě.TransferItemTo(Ġ,ě,null,true,ō[ě].Amount);break;}}ĥ.UseConveyorSystem=false;o.Add(ĥ);}l=true;m=1;Runtime.
UpdateFrequency|=UpdateFrequency.Update10;Echo("Smelt learning ON — insert ore into "+n.CustomName);}else if(ŋ=="learn_smelt_off"&&l){
foreach(var ĥ in o)ĥ.UseConveyorSystem=!ĥ.CustomName.Contains(G);o.Clear();n=null;l=false;m=0;p=default(MyItemType);a=0f;b=0f;
if(!v)Runtime.UpdateFrequency&=~UpdateFrequency.Update10;Echo("Smelt learning OFF.");}else if(ŋ=="refresh_lcds"){ń();}if((
Ō&UpdateType.Update10)!=0&&v)ġ();if((Ō&UpdateType.Update10)!=0&&l)ĵ();if((Ō&UpdateType.Once)!=0){Ã+=Runtime.
TimeSinceLastRun;Ő();ő();}if((Ō&UpdateType.Update100)!=0){var Ď=new MyIni();if(Ď.TryParse(Me.CustomData)){H=Ď.Get("KITTY",
"BudgetMsPerFrame").ToDouble(H);I=Ď.Get("KITTY","LogThreshold").ToDouble(I);J=Ď.Get("KITTY","SpikeThreshold").ToDouble(J);P=Ď.Get(
"KITTY Reactors","ManageReactors").ToBoolean(P);Q=(float)Ď.Get("KITTY Reactors","LargeGrid_LargeReactor_Uranium").ToDouble(Q);R=(float)Ď
.Get("KITTY Reactors","LargeGrid_SmallReactor_Uranium").ToDouble(R);S=(float)Ď.Get("KITTY Reactors",
"SmallGrid_LargeReactor_Uranium").ToDouble(S);T=(float)Ď.Get("KITTY Reactors","SmallGrid_SmallReactor_Uranium").ToDouble(T);W=Ď.Get("KITTY Refineries",
"ManageRefineries").ToBoolean(W);string Œ=Ď.Get("KITTY Refineries","RefineryMode").ToString("Auto");X=!string.Equals(Œ,"Manual",
StringComparison.OrdinalIgnoreCase);Y=Ď.Get("KITTY Refineries","RefineryPriority").ToString(Y);Z=(float)Ď.Get("KITTY Refineries",
"RefineryOreBuffer").ToDouble(Z);c.Clear();foreach(var ĸ in Ď.Get("KITTY Refineries","RefineryDisabledOres").ToString("").Split(',')){var Ĺ
=ĸ.Trim();if(Ĺ.Length>0)c.Add(Ĺ);}U=Ď.Get("KITTY Gas Generators","ManageGasGenerators").ToBoolean(U);V=(float)Ď.Get(
"KITTY Gas Generators","IceTarget").ToDouble(V);O=Ď.Get("KITTY Connectors","EmptyConnectedGrids").ToBoolean(O);}ò++;d++;e++;f++;if(f%300==0)ń(
);MyIniParseResult œ;foreach(var Ŕ in ã){string ŕ=Ŕ.Ô.CustomData;if(ŕ!=Ŕ.Þ){Ŕ.Þ=ŕ;if(Ŕ.Ø.TryParse(ŕ,out œ)){Ŕ.Ù=Ŕ.Ø.Get(
"KITTY","Mode").ToString("None");Ŕ.Ú=Ŕ.Ø.Get("KITTY","Group").ToString("None").Trim('"');Ŕ.ß=Math.Max(1,Ŕ.Ø.Get("KITTY",
"Columns").ToInt32(1));float Ŗ=(Ŕ.Ù=="Autocrafting"||Ŕ.Ù=="Status")?0.5f:0.8f;Ŕ.à=(float)Ŕ.Ø.Get("KITTY","FontSize").ToDouble(Ŗ);
Ŕ.Û=Ŕ.Ø.Get("KITTY","Category").ToString("All");Ŕ.Ü=Ŕ.Ø.Get("KITTY","Style").ToString("Grid");Ŕ.á=Ŕ.Ø.Get("KITTY",
"Scroll").ToBoolean(true);Ŕ.Ý=Ŕ.Ø.Get("KITTY","Sort").ToString("None");}}if(Ŕ.Ù=="Autocrafting"){i.Clear();k.Clear();var ŗ=new
List<MyIniKey>();Ŕ.Ø.GetKeys("KITTY Autocrafting",ŗ);if(ŗ.Count==0)Ř(Ŕ.Ô);else foreach(var î in ŗ){string ř=Ŕ.Ø.Get(î).
ToString();bool Ś=ř.EndsWith("-HIDE",StringComparison.OrdinalIgnoreCase);if(Ś)ř=ř.Substring(0,ř.Length-5);int ś;i[î.Name]=int.
TryParse(ř,out ś)?ś:0;if(Ś)k.Add(î.Name);}}var Ŝ=Ŕ.Õ.DrawFrame();switch(Ŕ.Ù){case"Inventory":ŝ(ref Ŝ,Ŕ,ò);break;case
"Autocrafting":Ş(ref Ŝ,Ŕ,ò);break;case"Status":ş(ref Ŝ,Ŕ,ò);break;case"Debug":Š(ref Ŝ,Ŕ);break;default:š(ref Ŝ,Ŕ,ò);break;}Ŝ.Dispose()
;}if(ó==null)ó=Ţ();Runtime.UpdateFrequency|=UpdateFrequency.Once;}double ţ,Ť,ť,Ŧ;ŧ(Ō,out ţ,out Ť,out ť,out Ŧ);string Ũ=
$"({g} of {Â})\n{Á}\n\nDEBUG:\n";Ũ+=$"Script Runtime: {ţ:N2} ms\n"+$"Avg (last {È} calls): {Ť:N2} ms\n"+
$"Est. ms/frame: {ť:N4} ms  ({ť/16.667*100:F3}% of game loop)\n"+$"Budget: {H:N2} ms/f\n"+$"ms/unit: {N:N4} ms\n"+$"Allowed units/tick: {Ï}  (ran {h} last)\n"+
$"Instructions: {Ŧ} / {Ó} (cap) / 50k (kill)";Echo(Ũ);}void š(ref MySpriteDrawFrame Ŝ,â Ŕ,int ũ){var Ū=new Vector2(256,0)+Ŕ.Ö.Position;float ū=1.0f;var ŭ=new
MySprite(){Type=SpriteType.TEXTURE,Data="Textures\\FactionLogo\\Others\\OtherIcon_29.dds",Position=Ŕ.Ö.Center,Size=Ŕ.Ö.Size/2,
Color=Ŭ(new Color(0,100,120,50),ũ),Alignment=TextAlignment.CENTER};Ŝ.Add(ŭ);var Ů=new MySprite(){Type=SpriteType.TEXT,Data=
$"K.I.T.T.Y. (v{D}.{E}.{F})",Position=Ū,RotationOrScale=ū,Color=Ŭ(Color.White,ũ),Alignment=TextAlignment.CENTER,FontId="White"};Ŝ.Add(Ů);}public
void Ŋ(IMyTextSurface ů){ů.ScriptBackgroundColor=new Color(0,0,0,255);ů.ContentType=ContentType.SCRIPT;ů.Script="";}static
Color Ŭ(Color Ą,int Ű)=>new Color(Ą.R,Ą.G,Ą.B,(byte)((Ą.A>1?Ą.A-1:Ą.A)+Ű%2));public void ŧ(UpdateType Ō,out double ţ,out
double Ť,out double ť,out double Ŧ){ţ=Runtime.LastRunTimeMs;Ê.Enqueue(ţ);K+=ţ;if(Ê.Count>È)K-=Ê.Dequeue();Ť=Ê.Count>0?K/Ê.
Count:0;if((Î&UpdateType.Update100)!=0){Ë.Enqueue(ţ);L+=ţ;if(Ë.Count>É)L-=Ë.Dequeue();}else if((Î&UpdateType.Once)!=0){Ì.
Enqueue(ţ);M+=ţ;if(Ì.Count>É)M-=Ì.Dequeue();}Î=Ō;double ű=Ë.Count>0?L/Ë.Count:0;double Ų=Ì.Count>0?M/Ì.Count:0;ť=(ű/100.0)+Ų;Ŧ=
Runtime.CurrentInstructionCount;}void Ő(){if(h<=0)return;double ų=Runtime.LastRunTimeMs/h;N=Ð*ų+(1.0-Ð)*N;if(N>0)Ï=(int)Math.
Max(Ñ,Math.Min(Ò,Math.Floor(H/N)));}void Ÿ(string ï,int Ŵ){if(Á!="Idle"){double ŵ=Runtime.LastRunTimeMs;if(ŵ>=I){bool Ŷ=ŵ>=
J;string ŷ=$"{(int)Ä.TotalHours:D2}:{Ä.Minutes:D2}:{Ä.Seconds:D2}";string ĳ=$"[{ŷ}] {(Ŷ?"!!":"  ")} {Á} ({ŵ:F2}ms)";Æ.
Enqueue(ĳ);if(Æ.Count>Ç)Æ.Dequeue();}}Á=ï;g=Ŵ;Ä=Ã;Å=Runtime.LastRunTimeMs;}public void ő(){if(ó==null)return;int Ź=0;while(Ź<Ï)
{if(Runtime.CurrentInstructionCount>=Ó)break;bool ź=ó.MoveNext();Ź++;if(!ź){ó.Dispose();ó=Ţ();}}h=Ź;Runtime.
UpdateFrequency|=UpdateFrequency.Once;}void ŝ(ref MySpriteDrawFrame Ŝ,â Ŕ,int ũ){Dictionary<MyItemType,float>Ż;if(!ä.TryGetValue(Ŕ.Ú,
out Ż))return;var ż=new Dictionary<MyItemType,float>();foreach(var Ĉ in Ż){if(Ŕ.Û!="All"&&Ĝ(Ĉ.Key)!=Ŕ.Û)continue;ż[Ĉ.Key]=Ĉ
.Value;}var Ž=new List<KeyValuePair<MyItemType,float>>(ż);if(Ŕ.Ý=="Num")Ž.Sort((ú,ĕ)=>ĕ.Value.CompareTo(ú.Value));else if
(Ŕ.Ý=="Alph")Ž.Sort((ú,ĕ)=>string.Compare(ð(ú.Key),ð(ĕ.Key),StringComparison.OrdinalIgnoreCase));var ž=new Dictionary<
MyItemType,float>();foreach(var Ĉ in Ž)ž[Ĉ.Key]=Ĉ.Value;if(Ŕ.Ü=="List")ſ(ref Ŝ,Ŕ,ž,ũ);else ƀ(ref Ŝ,Ŕ,ž,ũ);}void ƀ(ref
MySpriteDrawFrame Ŝ,â Ŕ,Dictionary<MyItemType,float>ż,int ũ){float Ɓ=Ŕ.à*32f;float Ƃ=Ɓ+4f;float ƃ=Ŕ.Ö.Size.X/Ŕ.ß;var Ƅ=Ŕ.Ö.Position;float
ƅ=Ŕ.Ö.Size.Y;var Ɔ=new List<KeyValuePair<MyItemType,float>>(ż);int Ƈ=(int)Math.Ceiling((double)Ɔ.Count/Ŕ.ß);int ƈ=Math.
Max(1,(int)(ƅ/Ƃ));int Ɖ=0;if(Ŕ.á&&Ƈ>ƈ)Ɖ=(ũ/120)%(Ƈ-ƈ+1);int Ɗ=0,Ƌ=0;foreach(var Ĉ in Ɔ){int ƌ=Ƌ-Ɖ;if(ƌ>=0&&ƌ<ƈ){var Ū=Ƅ+new
Vector2(Ɗ*ƃ,ƌ*Ƃ);string ƍ=ñ(Ĉ.Key);if(ƍ!=null)Ŝ.Add(new MySprite{Type=SpriteType.TEXTURE,Data=ƍ,Position=Ū+new Vector2(Ɓ/2f,Ɓ/
2f),Size=new Vector2(Ɓ,Ɓ),Color=Ŭ(Color.White,ũ),Alignment=TextAlignment.CENTER});float Ǝ=Ŕ.à*Í;float Ə=Ū.X+Ɓ+4f;float Ɛ=Ƅ
.X+(Ɗ+1)*ƃ;string Ƒ=Ĉ.Value.ToString("N0");float ƒ=(Ɛ-Ə)-Ƒ.Length*Ǝ;int Ɠ=Math.Max(0,(int)(ƒ/Ǝ));string ï=ð(Ĉ.Key);string
Ɣ=ï.Length>Ɠ?ï.Substring(0,Math.Max(0,Ɠ-1))+"~":ï;Ŝ.Add(new MySprite{Type=SpriteType.TEXT,Data=Ɣ,Position=new Vector2(Ə,Ū
.Y),RotationOrScale=Ŕ.à,Color=Ŭ(Color.White,ũ),Alignment=TextAlignment.LEFT,FontId="White"});Ŝ.Add(new MySprite{Type=
SpriteType.TEXT,Data=Ƒ,Position=new Vector2(Ɛ,Ū.Y),RotationOrScale=Ŕ.à,Color=Ŭ(Color.White,ũ),Alignment=TextAlignment.RIGHT,FontId
="White"});}Ɗ++;if(Ɗ>=Ŕ.ß){Ɗ=0;Ƌ++;}}}static readonly string[]ƕ={"Ores","Ingots","Components","Ammo","Utility","Other"};
class Ɯ{public bool Ɩ;public string Ɨ;public float Ƙ;public Ɯ(bool ƙ,string ƚ,float ƛ){Ɩ=ƙ;Ɨ=ƚ;Ƙ=ƛ;}}void ſ(ref
MySpriteDrawFrame Ŝ,â Ŕ,Dictionary<MyItemType,float>ż,int ũ){float Ɲ=Ŕ.à;float ƞ=Ɲ*26f;float ƅ=Ŕ.Ö.Size.Y;var Ƅ=Ŕ.Ö.Position;var Ɵ=new
List<Ɯ>();bool Ơ=Ŕ.Û=="All";if(Ơ){foreach(var ĝ in ƕ){bool ơ=true;foreach(var Ĉ in ż){if(Ĝ(Ĉ.Key)!=ĝ)continue;if(ơ){Ɵ.Add(
new Ɯ(true,ĝ,0f));ơ=false;}Ɵ.Add(new Ɯ(false,ð(Ĉ.Key),Ĉ.Value));}}}else{foreach(var Ĉ in ż)Ɵ.Add(new Ɯ(false,ð(Ĉ.Key),Ĉ.
Value));}int Ƈ=Ɵ.Count;int ƈ=Math.Max(1,(int)(ƅ/ƞ));int Ɖ=0;if(Ŕ.á&&Ƈ>ƈ)Ɖ=(ũ/120)%(Ƈ-ƈ+1);for(int ě=Ɖ;ě<Math.Min(Ƈ,Ɖ+ƈ);ě++){
var Ƌ=Ɵ[ě];float Ƣ=Ƅ.Y+(ě-Ɖ)*ƞ;if(Ƌ.Ɩ){Ŝ.Add(new MySprite{Type=SpriteType.TEXT,Data=$"— {Ƌ.Ɨ} —",Position=new Vector2(Ƅ.X+Ŕ
.Ö.Size.X/2f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(Color.White,ũ),Alignment=TextAlignment.CENTER,FontId="White"});}else{Ŝ.Add(new
MySprite{Type=SpriteType.TEXT,Data=Ƌ.Ɨ,Position=new Vector2(Ƅ.X,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(Color.White,ũ),Alignment=
TextAlignment.LEFT,FontId="White"});Ŝ.Add(new MySprite{Type=SpriteType.TEXT,Data=$"{Ƌ.Ƙ:N0}",Position=new Vector2(Ƅ.X+Ŕ.Ö.Size.X,Ƣ),
RotationOrScale=Ɲ,Color=Ŭ(Color.White,ũ),Alignment=TextAlignment.RIGHT,FontId="White"});}}}void Ř(IMyTerminalBlock ņ){var ň=new MyIni()
;ň.TryParse(ņ.CustomData);foreach(var Ĉ in æ)ň.Set("KITTY Autocrafting",Ĉ.Key,0);ņ.CustomData=ň.ToString();}void Ē(string
ƣ){var Ņ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(Ņ,ĕ=>ĕ!=Me&&ĕ.CustomName.Contains(G)&&!ĕ.
CustomName.Contains(B));foreach(var ņ in Ņ){var ň=new MyIni();if(!ň.TryParse(ņ.CustomData))continue;var Ƥ=new List<MyIniKey>();ň.
GetKeys("KITTY Autocrafting",Ƥ);if(Ƥ.Count==0)continue;if(ň.ContainsKey("KITTY Autocrafting",ƣ))continue;ň.Set(
"KITTY Autocrafting",ƣ,0);ņ.CustomData=ň.ToString();}}static string Ʀ(float ƥ){if(ƥ>=1000000f)return(ƥ/1000000f).ToString("F1")+"M";if(ƥ>=
1000f)return(ƥ/1000f).ToString("F1")+"k";return((int)ƥ).ToString();}static int ƨ(string ƣ){foreach(var Ĉ in å){if(!Ĉ.Key.
EndsWith("/"+ƣ))continue;string Ƨ=Ĉ.Key.Substring(0,Ĉ.Key.IndexOf('/'));switch(Ƨ){case"MyObjectBuilder_Component":return 0;case
"MyObjectBuilder_PhysicalGunObject":case"MyObjectBuilder_ConsumableItem":case"MyObjectBuilder_GasContainerObject":case
"MyObjectBuilder_OxygenContainerObject":return 1;case"MyObjectBuilder_AmmoMagazine":return 2;default:return 3;}}return 3;}string ƪ(string ƣ){string Ʃ;if(u.
TryGetValue(ƣ,out Ʃ))return Ʃ;foreach(var Ĉ in å)if(Ĉ.Key.EndsWith("/"+ƣ,StringComparison.OrdinalIgnoreCase))return Ĉ.Value;return
ƣ;}void Ş(ref MySpriteDrawFrame Ŝ,â Ŕ,int ũ){var ƫ=Ŕ.Ö.Position;float Ɲ=Ŕ.à;float ƞ=Ɲ*26f;float Ƣ=0f;Ŝ.Add(new MySprite{
Type=SpriteType.TEXT,Data="-- AUTOCRAFTING --",Position=ƫ+new Vector2(Ŕ.Ö.Size.X/2f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(Color.Cyan,
ũ),Alignment=TextAlignment.CENTER,FontId="White"});Ƣ+=ƞ*1.4f;var ŗ=new List<MyIniKey>();Ŕ.Ø.GetKeys("KITTY Autocrafting",
ŗ);var Ƭ=new List<MyIniKey>();var ƭ=new List<MyIniKey>();var Ʈ=new List<MyIniKey>();var Ư=new List<MyIniKey>();foreach(
var ư in ŗ){switch(ƨ(ư.Name)){case 0:Ƭ.Add(ư);break;case 1:ƭ.Add(ư);break;case 2:Ʈ.Add(ư);break;default:Ư.Add(ư);break;}}ŗ.
Clear();ŗ.AddRange(Ƭ);ŗ.AddRange(ƭ);ŗ.AddRange(Ʈ);ŗ.AddRange(Ư);foreach(var î in ŗ){if(k.Contains(î.Name))continue;int ś=Ŕ.Ø.
Get(î).ToInt32(0);if(ś<=0)continue;float Ʊ=0f;j.TryGetValue(î.Name,out Ʊ);float Ʋ=ś>0?Math.Min(Ʊ/ś,1f):0f;int Ƴ=(int)Math.
Round(Ʋ*5f);string ƴ="["+new string('█',Ƴ)+new string('░',5-Ƴ)+"]";Color Ɗ=Ʊ>=ś?Color.Green:Ʊ<=0f?Color.Yellow:Color.White;
string ƚ=ƪ(î.Name);string Ƶ=$"{ƚ,-20} {Ʀ(Ʊ).PadLeft(6)} / {Ʀ((float)ś).PadLeft(6)}  {ƴ}";Ŝ.Add(new MySprite{Type=SpriteType.
TEXT,Data=Ƶ,Position=ƫ+new Vector2(4f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(Ɗ,ũ),Alignment=TextAlignment.LEFT,FontId="Monospace"});Ƣ
+=ƞ;if(Ƣ>Ŕ.Ö.Size.Y)break;}}void ş(ref MySpriteDrawFrame Ŝ,â Ŕ,int ũ){var ƫ=Ŕ.Ö.Position;float Ɲ=Ŕ.à;float ƞ=Ɲ*26f;float Ƣ
=0f;Ŝ.Add(new MySprite{Type=SpriteType.TEXT,Data="-- K.I.T.T.Y. STATUS --",Position=ƫ+new Vector2(Ŕ.Ö.Size.X/2f,Ƣ),
RotationOrScale=Ɲ,Color=Ŭ(Color.Cyan,ũ),Alignment=TextAlignment.CENTER,FontId="White"});Ƣ+=ƞ*1.6f;Ŝ.Add(new MySprite{Type=SpriteType.
TEXT,FontId="Monospace",Alignment=TextAlignment.LEFT,Data=$"{"Reactors",-16} {(P?"MANAGED":"off")}",Position=ƫ+new Vector2(
4f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(P?Color.Green:Color.Gray,ũ)});Ƣ+=ƞ;string ƶ=W?(X?"Auto":"Manual")+"  ->  "+C:"off";Ŝ.Add(
new MySprite{Type=SpriteType.TEXT,FontId="Monospace",Alignment=TextAlignment.LEFT,Data=$"{"Refineries",-16} {ƶ}",Position=ƫ
+new Vector2(4f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(W?Color.Green:Color.Gray,ũ)});Ƣ+=ƞ*1.4f;int Ʒ=0,Ƹ=0;foreach(var Ĉ in i){if(Ĉ
.Value<=0)continue;Ʒ++;float ƹ;j.TryGetValue(Ĉ.Key,out ƹ);if(ƹ<Ĉ.Value)Ƹ++;}string ƺ=Ʒ==0?"off":Ƹ+" / "+Ʒ+" below quota";
Color ƻ=Ƹ>0?Color.Yellow:(Ʒ>0?Color.Green:Color.Gray);Ŝ.Add(new MySprite{Type=SpriteType.TEXT,FontId="Monospace",Alignment=
TextAlignment.LEFT,Data=$"{"Autocrafting",-16} {ƺ}",Position=ƫ+new Vector2(4f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(ƻ,ũ)});Ƣ+=ƞ*1.4f;string Ƽ;
Color ƽ;if(v){switch(w){case 1:Ƽ="ACTIVE - waiting";break;case 2:Ƽ="ACTIVE - watching";break;case 3:Ƽ="ACTIVE - recording";
break;default:Ƽ="ACTIVE";break;}ƽ=Color.Yellow;}else{Ƽ="off";ƽ=Color.Gray;}Ŝ.Add(new MySprite{Type=SpriteType.TEXT,FontId=
"Monospace",Alignment=TextAlignment.LEFT,Data=$"{"Learning Mode",-16} {Ƽ}",Position=ƫ+new Vector2(4f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(ƽ
,ũ)});Ƣ+=ƞ;string ƾ;Color ƿ;if(l){switch(m){case 1:ƾ="ACTIVE - insert ore";break;case 2:ƾ=p!=default(MyItemType)?
"ACTIVE - watching "+p.SubtypeId:"ACTIVE - watching";break;default:ƾ="ACTIVE";break;}ƿ=Color.Yellow;}else{ƾ="off";ƿ=Color.Gray;}Ŝ.Add(new
MySprite{Type=SpriteType.TEXT,FontId="Monospace",Alignment=TextAlignment.LEFT,Data=$"{"Smelt Learning",-16} {ƾ}",Position=ƫ+new
Vector2(4f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(ƿ,ũ)});Ƣ+=ƞ;Ŝ.Add(new MySprite{Type=SpriteType.TEXT,FontId="Monospace",Alignment=
TextAlignment.LEFT,Data=$"{"Drain Connectors",-16} {(O?"ON":"off")}",Position=ƫ+new Vector2(4f,Ƣ),RotationOrScale=Ɲ,Color=Ŭ(O?Color.
Green:Color.Gray,ũ)});}public IEnumerator<bool>Ţ(){Ÿ("Scanning cargo",1);var ǀ=new HashSet<string>();foreach(var ǁ in ã)if(ǁ.
Ù=="Inventory")ǀ.Add(ǁ.Ú);var ǂ=new List<string>();foreach(var ǃ in ä.Keys)if(!ǀ.Contains(ǃ))ǂ.Add(ǃ);foreach(var ǃ in ǂ)
ä.Remove(ǃ);var ō=new List<MyInventoryItem>();foreach(var ǅ in ǀ){var Ǆ=new List<IMyTerminalBlock>();var ǆ=
GridTerminalSystem.GetBlockGroupWithName(ǅ);if(ǆ!=null)ǆ.GetBlocks(Ǆ);var Ǉ=new Dictionary<MyItemType,float>();foreach(var ņ in Ǆ){for(int
ě=0;ě<ņ.InventoryCount;ě++){ō.Clear();ņ.GetInventory(ě).GetItems(ō);foreach(var ǈ in ō){float ü;Ǉ[ǈ.Type]=(Ǉ.TryGetValue(
ǈ.Type,out ü)?ü:0f)+(float)ǈ.Amount;}}yield return true;}ä[ǅ]=Ǉ;}Ÿ("Indexing containers",2);yield return true;var ǉ=new
List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ǉ,ĕ=>ĕ.CustomName.Contains(G)&&ĕ.HasInventory&&!ĕ.CustomName.
Contains(B));var ē=new List<ö>();foreach(var ņ in ǉ){string ï=ņ.CustomName;var Ė=new ö{Ô=ņ,ô=ņ.GetInventory(0)};if(ï.IndexOf(
"Ores",StringComparison.OrdinalIgnoreCase)>=0)Ė.õ.Add("Ores");if(ï.IndexOf("Ingots",StringComparison.OrdinalIgnoreCase)>=0)Ė.õ
.Add("Ingots");if(ï.IndexOf("Components",StringComparison.OrdinalIgnoreCase)>=0)Ė.õ.Add("Components");if(ï.IndexOf("Ammo"
,StringComparison.OrdinalIgnoreCase)>=0)Ė.õ.Add("Ammo");if(ï.IndexOf("Utility",StringComparison.OrdinalIgnoreCase)>=0)Ė.õ
.Add("Utility");if(ï.IndexOf("Other",StringComparison.OrdinalIgnoreCase)>=0)Ė.õ.Add("Other");if(Ė.õ.Count==0)continue;ē.
Add(Ė);yield return true;}Ÿ("Scanning autocrafting stock",3);ē.RemoveAll(Ė=>Ė.Ô.Closed);if(i.Count>0){j.Clear();var Ǌ=new
List<MyInventoryItem>();foreach(var Ė in ē){Ǌ.Clear();Ė.ô.GetItems(Ǌ);foreach(var ý in Ǌ){float ü;j.TryGetValue(ý.Type.
SubtypeId,out ü);j[ý.Type.SubtypeId]=ü+(float)ý.Amount;}yield return true;}}Ÿ("Pulling from untagged cargo",4);yield return true;
ē.RemoveAll(Ė=>Ė.Ô.Closed);var Ę=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(Ę,ĕ=>ĕ.CubeGrid==Me.
CubeGrid&&ĕ.HasInventory&&!ĕ.CustomName.Contains(G)&&!ĕ.CustomName.Contains(B)&&!(ĕ is IMyReactor)&&!(ĕ is IMyGasGenerator)&&!(ĕ
is IMyProductionBlock)&&!(ĕ is IMyLargeTurretBase)&&!(ĕ is IMySmallGatlingGun)&&!(ĕ is IMySmallMissileLauncher));var ǋ=new
List<MyInventoryItem>();foreach(var ğ in Ę){var ǌ=ğ.GetInventory(0);ǋ.Clear();ǌ.GetItems(ǋ);for(int ě=ǋ.Count-1;ě>=0;ě--){
var ǈ=ǋ[ě];string ĝ=Ĝ(ǈ.Type);foreach(var Ġ in ē){if(!Ġ.õ.Contains(ĝ))continue;if(!Ġ.ô.CanItemsBeAdded(ǈ.Amount,ǈ.Type))
continue;ǌ.TransferItemTo(Ġ.ô,ě,null,true,ǈ.Amount);break;}yield return true;}yield return true;}Ÿ("Re-sorting misplaced items",
5);ē.RemoveAll(Ė=>Ė.Ô.Closed);foreach(var Ǎ in ē){ǋ.Clear();Ǎ.ô.GetItems(ǋ);for(int ě=ǋ.Count-1;ě>=0;ě--){var ǈ=ǋ[ě];if(Ǎ
.õ.Contains(Ĝ(ǈ.Type)))continue;foreach(var Ġ in ē){if(Ġ==Ǎ)continue;if(!Ġ.õ.Contains(Ĝ(ǈ.Type)))continue;if(!Ġ.ô.
CanItemsBeAdded(ǈ.Amount,ǈ.Type))continue;Ǎ.ô.TransferItemTo(Ġ.ô,ě,null,true,ǈ.Amount);break;}yield return true;}yield return true;}Ÿ(
"Sorting inventory",6);yield return true;var ǎ=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ǎ,ĕ=>ĕ.CustomName.Contains(G
)&&ĕ.HasInventory&&!ĕ.CustomName.Contains(B));var Ǐ=new List<MyInventoryItem>();foreach(var ņ in ǎ){var Ě=ņ.GetInventory(
0);Ǐ.Clear();Ě.GetItems(Ǐ);for(int ě=0;ě<Ǐ.Count-1;ě++){for(int ǐ=Ǐ.Count-1;ǐ>ě;ǐ--){if(Ǐ[ǐ].Type==Ǐ[ě].Type){Ě.
TransferItemTo(Ě,ǐ,ě,true,Ǐ[ǐ].Amount);Ǐ.RemoveAt(ǐ);}}yield return true;}for(int ě=0;ě<Ǐ.Count-1;ě++){if(Ǒ(Ǐ[ě],Ǐ[ě+1])>0){Ě.
TransferItemTo(Ě,ě+1,ě,false,Ǐ[ě+1].Amount);var ǒ=Ǐ[ě];Ǐ[ě]=Ǐ[ě+1];Ǐ[ě+1]=ǒ;}yield return true;}}Ÿ("Managing gas generators",7);yield
return true;ē.RemoveAll(Ė=>Ė.Ô.Closed);var Ǔ=new List<IMyGasGenerator>();GridTerminalSystem.GetBlocksOfType(Ǔ,ǔ=>ǔ.CubeGrid==
Me.CubeGrid&&!ǔ.CustomName.Contains(B));if(Ǔ.Count>0&&U){var Ǖ=new MyItemType("MyObjectBuilder_Ore","Ice");var ǖ=new List<
IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType(ǖ,ĕ=>ĕ.CubeGrid==Me.CubeGrid&&ĕ.HasInventory&&!(ĕ is IMyGasGenerator)&&!ĕ.
CustomName.Contains(G)&&!ĕ.CustomName.Contains(B));var Ǘ=new List<MyInventoryItem>();foreach(var ǘ in Ǔ){var Ǚ=ǘ.GetInventory(0);Ǘ
.Clear();Ǚ.GetItems(Ǘ);for(int ě=Ǘ.Count-1;ě>=0;ě--){string ǚ=Ǘ[ě].Type.TypeId;if(ǚ!="MyObjectBuilder_GasContainerObject"
&&ǚ!="MyObjectBuilder_OxygenContainerObject")continue;foreach(var Ė in ē){if(!Ė.õ.Contains("Utility"))continue;if(!Ė.ô.
CanItemsBeAdded(Ǘ[ě].Amount,Ǘ[ě].Type))continue;Ǚ.TransferItemTo(Ė.ô,ě,null,true,Ǘ[ě].Amount);break;}yield return true;}Ǘ.Clear();Ǚ.
GetItems(Ǘ);float Ǜ=0f;foreach(var ý in Ǘ)if(ý.Type==Ǖ)Ǜ+=(float)ý.Amount;if(Ǜ<V){float ǜ=V-Ǜ;foreach(var ğ in Ę){if(ǜ<=0f)break
;var ǌ=ğ.GetInventory(0);Ǘ.Clear();ǌ.GetItems(Ǘ);for(int ě=Ǘ.Count-1;ě>=0;ě--){if(Ǘ[ě].Type!=Ǖ)continue;float ǝ=Math.Min(
(float)Ǘ[ě].Amount,ǜ);ǌ.TransferItemTo(Ǚ,ě,null,true,(MyFixedPoint)ǝ);ǜ-=ǝ;yield return true;break;}}}foreach(var Ǎ in ǖ)
{var ǌ=Ǎ.GetInventory(0);Ǘ.Clear();ǌ.GetItems(Ǘ);for(int ě=Ǘ.Count-1;ě>=0;ě--){string ǚ=Ǘ[ě].Type.TypeId;if(ǚ!=
"MyObjectBuilder_GasContainerObject"&&ǚ!="MyObjectBuilder_OxygenContainerObject")continue;if(!Ǚ.CanItemsBeAdded(Ǘ[ě].Amount,Ǘ[ě].Type))continue;ǌ.
TransferItemTo(Ǚ,ě,null,true,Ǘ[ě].Amount);yield return true;}yield return true;}yield return true;}}Ÿ("Fueling reactors",8);yield
return true;ē.RemoveAll(Ė=>Ė.Ô.Closed);if(P){var Ǟ=new List<IMyReactor>();GridTerminalSystem.GetBlocksOfType(Ǟ,ŏ=>ŏ.CubeGrid==
Me.CubeGrid&&!ŏ.CustomName.Contains(B));yield return true;var ǟ=new MyItemType("MyObjectBuilder_Ingot","Uranium");var Ǡ=
new List<MyInventoryItem>();foreach(var ǡ in Ǟ){bool Ǣ=ǡ.CubeGrid.GridSizeEnum==VRage.Game.MyCubeSize.Large;var ǣ=ǡ.Max-ǡ.
Min+Vector3I.One;bool Ǥ=ǣ.X*ǣ.Y*ǣ.Z>=8;float ǥ=Ǣ?(Ǥ?Q:R):(Ǥ?S:T);var Ǧ=ǡ.GetInventory(0);Ǡ.Clear();Ǧ.GetItems(Ǡ);float ǧ=0f
;foreach(var ý in Ǡ)if(ý.Type==ǟ)ǧ+=(float)ý.Amount;if(ǧ>=ǥ){yield return true;continue;}float ǜ=ǥ-ǧ;foreach(var Ė in ē){
if(ǜ<=0f)break;if(!Ė.õ.Contains("Ingots"))continue;Ǡ.Clear();Ė.ô.GetItems(Ǡ);for(int ě=Ǡ.Count-1;ě>=0&&ǜ>0f;ě--){if(Ǡ[ě].
Type!=ǟ)continue;float ǝ=Math.Min((float)Ǡ[ě].Amount,ǜ);Ė.ô.TransferItemTo(Ǧ,ě,null,true,(MyFixedPoint)ǝ);ǜ-=ǝ;yield return
true;}}foreach(var ğ in Ę){if(ǜ<=0f)break;var ǌ=ğ.GetInventory(0);Ǡ.Clear();ǌ.GetItems(Ǡ);for(int ě=Ǡ.Count-1;ě>=0&&ǜ>0f;ě--
){if(Ǡ[ě].Type!=ǟ)continue;float ǝ=Math.Min((float)Ǡ[ě].Amount,ǜ);ǌ.TransferItemTo(Ǧ,ě,null,true,(MyFixedPoint)ǝ);ǜ-=ǝ;
yield return true;}}yield return true;}}Ÿ("Managing refineries",9);yield return true;ē.RemoveAll(Ė=>Ė.Ô.Closed);if(W){var Ŏ=
new List<IMyRefinery>();GridTerminalSystem.GetBlocksOfType(Ŏ,ŏ=>ŏ.CubeGrid==Me.CubeGrid&&!ŏ.CustomName.Contains(B));if(l)Ŏ.
RemoveAll(o.Contains);foreach(var ŏ in Ŏ)ŏ.UseConveyorSystem=false;yield return true;if(Ŏ.Count>0&&d%20==0){var Ǩ=new List<
MyInventoryItem>();foreach(var ĥ in Ŏ){var þ=ĥ.GetInventory(1);Ǩ.Clear();þ.GetItems(Ǩ);for(int ě=Ǩ.Count-1;ě>=0;ě--){string ĝ=Ĝ(Ǩ[ě].
Type);foreach(var Ė in ē){if(!Ė.õ.Contains(ĝ))continue;if(!Ė.ô.CanItemsBeAdded(Ǩ[ě].Amount,Ǩ[ě].Type))continue;þ.
TransferItemTo(Ė.ô,ě,null,true,Ǩ[ě].Amount);break;}yield return true;}yield return true;}var ǩ=new List<MyInventoryItem>();var Ǫ=new
HashSet<string>(StringComparer.OrdinalIgnoreCase);foreach(var Ė in ē){if(!Ė.õ.Contains("Ores"))continue;ǩ.Clear();Ė.ô.GetItems(
ǩ);foreach(var ý in ǩ)if(ý.Type.TypeId=="MyObjectBuilder_Ore"&&(float)ý.Amount>=1f)Ǫ.Add(ý.Type.SubtypeId);}foreach(var ğ
in Ę){ǩ.Clear();ğ.GetInventory(0).GetItems(ǩ);foreach(var ý in ǩ)if(ý.Type.TypeId=="MyObjectBuilder_Ore"&&(float)ý.Amount
>=1f)Ǫ.Add(ý.Type.SubtypeId);}yield return true;string ǫ=null;if(Ǫ.Contains("Stone"))ǫ="Stone";else if(X){var Ǭ=new
Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);foreach(var Ė in ē){if(!Ė.õ.Contains("Ingots"))continue;ǩ.Clear();Ė.ô.
GetItems(ǩ);foreach(var ý in ǩ){if(ý.Type.TypeId!="MyObjectBuilder_Ingot")continue;float ü;Ǭ.TryGetValue(ý.Type.SubtypeId,out ü)
;Ǭ[ý.Type.SubtypeId]=ü+(float)ý.Amount;}}foreach(var ğ in Ę){ǩ.Clear();ğ.GetInventory(0).GetItems(ǩ);foreach(var ý in ǩ){
if(ý.Type.TypeId!="MyObjectBuilder_Ingot")continue;float ü;Ǭ.TryGetValue(ý.Type.SubtypeId,out ü);Ǭ[ý.Type.SubtypeId]=ü+(
float)ý.Amount;}}yield return true;var ǭ=new HashSet<string>(StringComparer.OrdinalIgnoreCase);var Ǯ=new List<string>();
foreach(var ĳ in Y.Split(',')){string ǯ=ĳ.Trim();if(c.Contains(ǯ))continue;if((ç.ContainsKey(ǯ)||r.ContainsKey(ǯ))&&Ǫ.Contains(
ǯ)&&ǭ.Add(ǯ))Ǯ.Add(ǯ);}foreach(var Ĉ in ç){if(c.Contains(Ĉ.Key))continue;if(Ǫ.Contains(Ĉ.Key)&&ǭ.Add(Ĉ.Key))Ǯ.Add(Ĉ.Key);
}foreach(var Ĉ in r){if(c.Contains(Ĉ.Key))continue;if(Ǫ.Contains(Ĉ.Key)&&ǭ.Add(Ĉ.Key))Ǯ.Add(Ĉ.Key);}float ǰ=float.
MaxValue;foreach(var ǯ in Ǯ){float Ŀ;if(!ç.TryGetValue(ǯ,out Ŀ))r.TryGetValue(ǯ,out Ŀ);if(Ŀ<=0f)continue;string Ǳ=ǯ;string ǲ;if(
s.TryGetValue(ǯ,out ǲ)){int ǳ=ǲ.IndexOf('/');if(ǳ>=0)Ǳ=ǲ.Substring(ǳ+1);}float Ǵ;Ǭ.TryGetValue(Ǳ,out Ǵ);float ǵ=Ǵ/Ŀ;if(ǵ<
ǰ){ǰ=ǵ;ǫ=ǯ;}}}else if(!X){foreach(var ĳ in Y.Split(',')){string ǯ=ĳ.Trim();if(c.Contains(ǯ))continue;if(Ǫ.Contains(ǯ)){ǫ=
ǯ;break;}}if(ǫ==null)foreach(var Ĉ in r)if(!c.Contains(Ĉ.Key)&&Ǫ.Contains(Ĉ.Key)){ǫ=Ĉ.Key;break;}}C=ǫ??"—";if(ǫ!=null){
var Ƕ="MyObjectBuilder_Ore";var Ƿ=new MyItemType(Ƕ,ǫ);float Ǹ=0f;foreach(var Ė in ē){if(!Ė.õ.Contains("Ores"))continue;ǩ.
Clear();Ė.ô.GetItems(ǩ);foreach(var ý in ǩ)if(ý.Type==Ƿ)Ǹ+=(float)ý.Amount;}foreach(var ğ in Ę){ǩ.Clear();ğ.GetInventory(0).
GetItems(ǩ);foreach(var ý in ǩ)if(ý.Type==Ƿ)Ǹ+=(float)ý.Amount;}foreach(var ŏ in Ŏ){ǩ.Clear();ŏ.GetInventory(0).GetItems(ǩ);
foreach(var ý in ǩ)if(ý.Type==Ƿ)Ǹ+=(float)ý.Amount;}float ǹ=Math.Min(Z,Ǹ/Ŏ.Count);foreach(var ĥ in Ŏ){var Ǻ=ĥ.GetInventory(0);Ǩ
.Clear();Ǻ.GetItems(Ǩ);for(int ě=Ǩ.Count-1;ě>=0;ě--){if(Ǩ[ě].Type.TypeId!=Ƕ)continue;if(Ǩ[ě].Type.SubtypeId==ǫ)continue;
bool Ğ=false;foreach(var Ė in ē){if(!Ė.õ.Contains("Ores"))continue;if(!Ė.ô.CanItemsBeAdded(Ǩ[ě].Amount,Ǩ[ě].Type))continue;Ǻ
.TransferItemTo(Ė.ô,ě,null,true,Ǩ[ě].Amount);Ğ=true;break;}if(!Ğ){foreach(var ğ in Ę){var ǻ=ğ.GetInventory(0);if(!ǻ.
CanItemsBeAdded(Ǩ[ě].Amount,Ǩ[ě].Type))continue;Ǻ.TransferItemTo(ǻ,ě,null,true,Ǩ[ě].Amount);break;}}yield return true;}Ǩ.Clear();Ǻ.
GetItems(Ǩ);float Ǽ=0f;foreach(var ý in Ǩ)if(ý.Type==Ƿ)Ǽ+=(float)ý.Amount;if(Ǽ>ǹ){float ǽ=Ǽ-ǹ;for(int ě=Ǩ.Count-1;ě>=0&&ǽ>0f;ě--
){if(Ǩ[ě].Type!=Ƿ)continue;float ǝ=Math.Min((float)Ǩ[ě].Amount,ǽ);bool Ğ=false;foreach(var Ė in ē){if(!Ė.õ.Contains(
"Ores"))continue;if(!Ė.ô.CanItemsBeAdded((MyFixedPoint)ǝ,Ƿ))continue;Ǻ.TransferItemTo(Ė.ô,ě,null,true,(MyFixedPoint)ǝ);ǽ-=ǝ;Ğ=
true;break;}if(!Ğ){foreach(var ğ in Ę){var ǻ=ğ.GetInventory(0);if(!ǻ.CanItemsBeAdded((MyFixedPoint)ǝ,Ƿ))continue;Ǻ.
TransferItemTo(ǻ,ě,null,true,(MyFixedPoint)ǝ);ǽ-=ǝ;break;}}yield return true;}Ǩ.Clear();Ǻ.GetItems(Ǩ);Ǽ=0f;foreach(var ý in Ǩ)if(ý.
Type==Ƿ)Ǽ+=(float)ý.Amount;}if(Ǽ<ǹ){float ǜ=ǹ-Ǽ;foreach(var Ė in ē){if(ǜ<=0f)break;if(!Ė.õ.Contains("Ores"))continue;Ǩ.Clear
();Ė.ô.GetItems(Ǩ);for(int ě=Ǩ.Count-1;ě>=0&&ǜ>0f;ě--){if(Ǩ[ě].Type!=Ƿ)continue;float ǝ=Math.Min((float)Ǩ[ě].Amount,ǜ);Ė.
ô.TransferItemTo(Ǻ,ě,null,true,(MyFixedPoint)ǝ);ǜ-=ǝ;yield return true;}}foreach(var ğ in Ę){if(ǜ<=0f)break;var ǌ=ğ.
GetInventory(0);Ǩ.Clear();ǌ.GetItems(Ǩ);for(int ě=Ǩ.Count-1;ě>=0&&ǜ>0f;ě--){if(Ǩ[ě].Type!=Ƿ)continue;float ǝ=Math.Min((float)Ǩ[ě].
Amount,ǜ);ǌ.TransferItemTo(Ǻ,ě,null,true,(MyFixedPoint)ǝ);ǜ-=ǝ;yield return true;}}}yield return true;}}}}else{var Ŏ=new List<
IMyRefinery>();GridTerminalSystem.GetBlocksOfType(Ŏ,ŏ=>ŏ.CubeGrid==Me.CubeGrid&&!ŏ.CustomName.Contains(B));foreach(var ŏ in Ŏ){if(l
&&ŏ==n)continue;ŏ.UseConveyorSystem=true;}}Ÿ("Collecting assembler output",10);yield return true;var Ǿ=new List<
IMyAssembler>();GridTerminalSystem.GetBlocksOfType(Ǿ,ú=>ú.CubeGrid==Me.CubeGrid&&ú.CustomName.Contains(G)&&!ú.CustomName.Contains(B)
);yield return true;var ǿ=new List<MyInventoryItem>();foreach(var Ȁ in Ǿ){if(!v)Ȁ.UseConveyorSystem=false;var þ=Ȁ.
GetInventory(1);ǿ.Clear();þ.GetItems(ǿ);for(int ě=ǿ.Count-1;ě>=0;ě--){string ĝ=Ĝ(ǿ[ě].Type);foreach(var Ė in ē){if(!Ė.õ.Contains(ĝ))
continue;if(!Ė.ô.CanItemsBeAdded(ǿ[ě].Amount,ǿ[ě].Type))continue;þ.TransferItemTo(Ė.ô,ě,null,true,ǿ[ě].Amount);break;}yield
return true;}yield return true;}Ÿ("Autocrafting",11);yield return true;if(i.Count>0&&!v){var ȁ=new List<IMyAssembler>();
GridTerminalSystem.GetBlocksOfType(ȁ,ú=>ú.CubeGrid==Me.CubeGrid&&ú.CustomName.Contains(G)&&!ú.CustomName.Contains(B)&&ú.Mode==
MyAssemblerMode.Assembly);if(ȁ.Count==0){yield return true;}else{var Ȃ=new List<MyProductionItem>();var ȃ=new List<KeyValuePair<string,
int>>(i);foreach(var Ĉ in ȃ){if(Ĉ.Value<=0)continue;float Ȅ;j.TryGetValue(Ĉ.Key,out Ȅ);float ȅ=Ĉ.Value-Ȅ;if(ȅ<=0f){yield
return true;continue;}MyDefinitionId ê;if(!ì(Ĉ.Key,out ê)){yield return true;continue;}float Ȇ=ȅ/ȁ.Count;foreach(var ú in ȁ){Ȃ
.Clear();ú.GetQueue(Ȃ);float ȇ=0f;foreach(var Ȉ in Ȃ)if(Ȉ.BlueprintId==ê)ȇ+=(float)Ȉ.Amount;float ȉ=Ȇ-ȇ;if(ȉ>0f)try{ú.
AddQueueItem(ê,(MyFixedPoint)ȉ);}catch{}}yield return true;}var Ȋ=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);
foreach(var Ĉ in j)Ȋ[Ĉ.Key]=Ĉ.Value;var ȋ=new List<MyInventoryItem>();var Ȍ=new List<MyProductionItem>();foreach(var Ȁ in ȁ){
var ȍ=Ȁ.GetInventory(0);Ȍ.Clear();Ȁ.GetQueue(Ȍ);foreach(var Ȉ in Ȍ){if(ȍ.MaxVolume>0&&(float)((double)ȍ.CurrentVolume/(
double)ȍ.MaxVolume)>=0.90f)break;string ë=Ȉ.BlueprintId.SubtypeId.ToString();Dictionary<string,float>Ȏ;bool ȏ=false;if(!è.
TryGetValue(ë,out Ȏ)){if(!À.TryGetValue(ë,out Ȏ))continue;ȏ=true;}float Ȑ=(float)Ȉ.Amount;var ȑ=new List<KeyValuePair<MyItemType,
float>>();foreach(var Ĉ in Ȏ){MyItemType í;if(ȏ){int Ȓ=Ĉ.Key.LastIndexOf('/');í=new MyItemType(Ĉ.Key.Substring(0,Ȓ),Ĉ.Key.
Substring(Ȓ+1));}else if(Ĉ.Key.Contains('/')){int Ȓ=Ĉ.Key.LastIndexOf('/');í=new MyItemType(Ĉ.Key.Substring(0,Ȓ),Ĉ.Key.Substring(
Ȓ+1));}else í=new MyItemType("MyObjectBuilder_Ingot",Ĉ.Key);ȑ.Add(new KeyValuePair<MyItemType,float>(í,Ĉ.Value));}ȋ.Clear
();ȍ.GetItems(ȋ);var ȓ=new Dictionary<string,float>(StringComparer.OrdinalIgnoreCase);foreach(var ȕ in ȋ){float Ȕ;ȓ.
TryGetValue(ȕ.Type.SubtypeId,out Ȕ);ȓ[ȕ.Type.SubtypeId]=Ȕ+(float)ȕ.Amount;}var Ȗ=new List<KeyValuePair<MyItemType,float>>();bool ȗ=
true;foreach(var đ in ȑ){float Ș=đ.Value*Ȑ;float ș;ȓ.TryGetValue(đ.Key.SubtypeId,out ș);float Ț=Ș-ș;if(Ț>0f){Ȗ.Add(new
KeyValuePair<MyItemType,float>(đ.Key,Ț));ȗ=false;}}if(ȗ)continue;bool ț=true;foreach(var ȝ in Ȗ){float Ȝ;Ȋ.TryGetValue(ȝ.Key.
SubtypeId,out Ȝ);if(Ȝ<ȝ.Value){ț=false;break;}}if(!ț)continue;foreach(var ȝ in Ȗ){float Ȟ=ȝ.Value;string ȟ=Ĝ(ȝ.Key);foreach(var Ė
in ē){if(Ȟ<=0f)break;if(!Ė.õ.Contains(ȟ))continue;ȋ.Clear();Ė.ô.GetItems(ȋ);for(int ě=ȋ.Count-1;ě>=0&&Ȟ>0f;ě--){if(ȋ[ě].
Type!=ȝ.Key)continue;float ǝ=Math.Min((float)ȋ[ě].Amount,Ȟ);Ė.ô.TransferItemTo(ȍ,ě,null,true,(MyFixedPoint)ǝ);Ȟ-=ǝ;}}float Ƞ
;Ȋ.TryGetValue(ȝ.Key.SubtypeId,out Ƞ);Ȋ[ȝ.Key.SubtypeId]=Ƞ-ȝ.Value;}yield return true;}yield return true;}}}Ÿ(
"Emptying connected grids",12);yield return true;if(O){var ȡ=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(ȡ,Ą=>Ą.CubeGrid==Me.
CubeGrid&&Ą.Status==MyShipConnectorStatus.Connected&&!Ą.CustomName.Contains(B));var Ȣ=new HashSet<long>();foreach(var Ą in ȡ)if(
Ą.OtherConnector!=null)Ȣ.Add(Ą.OtherConnector.CubeGrid.EntityId);if(Ȣ.Count>0){var ȣ=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType(ȣ,ĕ=>ĕ.HasInventory&&!ĕ.CustomName.Contains(G)&&!ĕ.CustomName.Contains(B)&&Ȣ.Contains(ĕ.CubeGrid.
EntityId));var Ȥ=new List<MyInventoryItem>();foreach(var ņ in ȣ){for(int ę=0;ę<ņ.InventoryCount;ę++){var ǌ=ņ.GetInventory(ę);Ȥ.
Clear();ǌ.GetItems(Ȥ);for(int ě=Ȥ.Count-1;ě>=0;ě--){string ĝ=Ĝ(Ȥ[ě].Type);foreach(var Ġ in ē){if(!Ġ.õ.Contains(ĝ))continue;if
(!Ġ.ô.CanItemsBeAdded(Ȥ[ě].Amount,Ȥ[ě].Type))continue;ǌ.TransferItemTo(Ġ.ô,ě,null,true,Ȥ[ě].Amount);break;}yield return
true;}yield return true;}}}}yield return true;}static int Ǒ(MyInventoryItem ú,MyInventoryItem ĕ){int ȥ=string.Compare(ú.Type
.TypeId,ĕ.Type.TypeId,StringComparison.Ordinal);if(ȥ!=0)return ȥ;float Ȧ=(float)ĕ.Amount-(float)ú.Amount;return Ȧ>0f?1:Ȧ<
0f?-1:0;}void Š(ref MySpriteDrawFrame Ŝ,â Ŕ){var ƫ=Ŕ.Ö.Position;float Ɲ=Ŕ.à;float ƞ=Ɲ*26f;float Ƣ=0f;Ŝ.Add(new MySprite{
Type=SpriteType.TEXT,Data=$"=== KITTY Task Log log>{I:F1} spike>{J:F1} ===",Position=ƫ+new Vector2(4f,Ƣ),RotationOrScale=Ɲ,
Color=Color.Cyan,Alignment=TextAlignment.LEFT,FontId="Monospace"});Ƣ+=ƞ*1.4f;var ȧ=Æ.ToArray();float Ȩ=Ŕ.Ö.Size.Y-ƞ;for(int ě
=ȧ.Length-1;ě>=0&&Ƣ<Ȩ;ě--){string Ƶ=ȧ[ě];Ŝ.Add(new MySprite{Type=SpriteType.TEXT,Data=Ƶ,Position=ƫ+new Vector2(4f,Ƣ),
RotationOrScale=Ɲ,Color=Ƶ.Contains("!!")?Color.Yellow:Color.White,Alignment=TextAlignment.LEFT,FontId="Monospace"});Ƣ+=ƞ;}}
