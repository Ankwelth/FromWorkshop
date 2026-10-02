// Production Inventory Manager v1.1c
// c BelaOkuma 2017-2025
// only compatible with SMS v1.4!
// 
// ### developed with MDK2 ###
// 
// the configuration is located in the customdata.
// 
// new in update 1.1b:
// 
//  - add Modsupport
// 	Sigma Draconis Core Mod
// 
// new in update 1.1c:
//  - add support for Apex items, add (sms,food) to containers
// 
// ----------------------------------------------------------------------------------------------------------------
public class A:IComparable<A>{int B=0;public List<C>D=new List<C>();public E F=new E();public IMyAssembler G;bool H=
false;bool I=false;bool J=false;public A(IMyAssembler K){G=K;I=K.BlockDefinition.TypeIdString=="SurvivalKit";}public bool L()
{return G.Closed;}public int CompareTo(A M){if(M.B<B)return 1;else if(M.B>B)return-1;return 0;}public void S(C N){if(L())
return;if(F.O()&&G.CanUseBlueprint(N.P)){N.Q.Add(this);N.R++;D.Add(N);B++;}}public bool b(C N){if(L())return false;if(G.Mode==
MyAssemblerMode.Disassembly)return false;var T=false;var X=(N.U-N.V-N.W);var Y=X/N.R;if(X<100){Y=X;T=true;}G.Repeating=false;try{if(N.Z
)G.AddQueueItem(N.P,(MyFixedPoint)Y);}catch(Exception a){N.Z=false;}return T;}public void g(c d){if(H){d.e(F.f);d.e(
" cannot unload output items.\n");}if(!L()&&!G.IsFunctional){d.e(F.f);d.e(" is damaged.\n");}}public void x(){if(L())return;var h=new List<
MyProductionItem>();var i=new List<C>();if(G.Mode==MyAssemblerMode.Assembly){var j=G.GetInventory(1);k(j);H=j.CurrentVolume>0;G.GetQueue
(h);for(int l=h.Count-1;l>=0;l--){var n=m(h[l]);if(n!=null){i.Add(n);}}}else k(G.GetInventory(0));if(!F.o(G.CustomName,
true))return;D.Clear();B=0;if(G.IsFunctional){if(G.IsQueueEmpty){if(!I){if(!p||F.q("Nooff"))G.Enabled=true;else G.Enabled=
false;}if(G.Mode==MyAssemblerMode.Disassembly)k(G.GetInventory(1));else k(G.GetInventory(0));}else{G.Enabled=true;if(G.Mode==
MyAssemblerMode.Assembly){if(J){J=false;if(r){for(int l=h.Count-1;l>=0;l--){var s=h[l];foreach(var n in i){if(n.P.SubtypeName==s.
BlueprintId.SubtypeName&&n.U!=0&&n.U<=n.V){G.RemoveQueueItem(l,s.Amount);h.RemoveAt(l);}}}}}else{J=true;C u=h.Count>1?t(h[0]):null;
if(u!=null){for(int l=h.Count-1;l>0;l--){var v=t(h[l]);if(v!=null&&v.U!=0&&v.w>u.w){G.MoveQueueItemRequest(h[l].ItemId,0);
break;}}}}}}}}}void Å(string y,string z,string ª,string µ){if(º!="Vanilla"&&(À.ContainsKey(º)?!À[º]:true))return;
MyDefinitionId Á;if(!MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/"+z,out Á))return;if(z.StartsWith("Position0"))z=z.
Substring(z.IndexOf('_')+1);if(!Â.Contains(º))Â.Add(º);var Ã=y+" "+z;if(!Ä.ContainsKey(Ã))Ä.Add(Ã,new C(y,z,º,(ª==""?"":y+" "+ª),
µ,Á));}void É(string Æ,string Ç="",string È=""){Å("Component",Æ,Ç,È);}void Ë(string Æ,string Ç="",string È=""){Å(Ê,Æ,Ç,È)
;}void Í(string Æ,string Ç="",string È=""){Å(Ì,Æ,Ç,È);}void Ï(string Æ,string Ç="",string È=""){Å(Î,Æ,Ç,È);}void Ñ(string
Æ,string Ç="",string È=""){Å(Ð,Æ,Ç,È);}void Ó(string Æ,string Ç="",string È=""){Å(Ò,Æ,Ç,È);}void Õ(string Æ,string Ç="",
string È=""){Å(Ô,Æ,Ç,È);}void Ø(string Æ,string Ç="",string È=""){Å(Ö,Æ,Ç,È);}void Ü(string Ù,string ª="",string Ú=""){Å(Û,Ù,ª
,Ú);}void Ý(string Æ,string Ç="",string È=""){Å("Ore",Æ,Ç,È);}public class C:IComparable<C>{public long w=0;public List<A
>Q=new List<A>();public bool Z=true;public MyDefinitionId P;public int R=0;public int V=0;public int Þ=0;public int W=0;
public int ß=0;public int à=0;public int U=1000;public string á="";public string â="";public string ã="";public string ä="";
public string å="";string æ="";public string ç="";string è="";static public void ì(){foreach(var ê in é.Values)ê.ë();}static
string ï(string í,string Æ){var î="Component";if(Æ=="Magnetron_Component")return î+" "+Æ;if(í==î&&Æ.EndsWith(î))return í+" "+Æ
.Substring(0,Æ.Length-î.Length);if(Æ.StartsWith("NATO_25")){î="Magazine";return í+" "+Æ.Substring(0,Æ.Length-î.Length);}
if(í==Ì){if(Æ.Contains("Drill")||Æ.Contains("Grinder")||Æ.Contains("Welder")||Æ.Contains("Rifle"))return í+" "+Æ+"Item";}
return í+" "+Æ;}const string ð="AutomaticRifleGun_Mag_";string[]ñ={Ì,Ô,Ò,Ð,};void þ(){ä=æ;if(ñ.Contains(æ))ä=ò;else if(ó.
Contains(â))ä="Seeds";else if(ô.Contains(â))ä=õ;else if(ç==ö.ø)ä=ö.ø;else if(ç.Contains("Deuterium"))ä="Deuterium";if(è!=""){ã=è
.Replace('_',' ');return;}ã=â.Split(' ')[1];if(ã.StartsWith("Position"))ã=ã.Substring(ã.IndexOf('_')+1);else if(ã.
StartsWith("MealPack"))ã=ã.Substring(9);else if(ã.StartsWith("K_HSR_"))ã=ã.Substring(6);else if(ã.Contains(ð)){if(ã.StartsWith(ð))
ã="AutoRifleGunMagazine";else ã=ã.Substring(0,ã.IndexOf(ð))+"RifleGunMagazine";}else if(æ==Ö)ã+=" Seeds";else if(æ==Ì&&á
==ù){string[]ú={"HandDrill","Grinder","Welder"};var û=false;foreach(var ü in ú){if(ã.Contains(ü)){var ý="";if(ã.Contains(
'4'))ý="Elite";else if(ã.Contains('3'))ý="Professional";else if(ã.Contains('2'))ý="Ultimate";ã=ý+ü;û=true;break;}}if(!û&&ã.
EndsWith("Item"))ã=ã.Substring(0,ã.Length-4);}if(ã.EndsWith("Magazine"))ã=ã.Substring(0,ã.Length-5);ã=ã.Replace('_',' ');}public
C(string ÿ,string Ā,string ā,string Ă,string ă,MyDefinitionId Ą){è=ă;æ=ÿ;ç=Ā;å=æ+" "+ç;P=Ą;â=Ă==""?ï(æ,ç):Ă;á=ā;þ();ą(0);
}public int CompareTo(C M){if(M.w<w)return-1;else if(M.w>w)return 1;return 0;}public bool Ć(){return(à>V+W);}public bool
ć(){return(U<=V);}public void ą(int Ĉ){U=Ĉ;ë();}void ë(){à=(U*ĉ)/100;}public void ċ(int Ċ){R=0;V=Ċ;if(U>0)Þ=U-Ċ;else Þ=-1
;}public void Č(){if(Þ<=0)w=0;else w=100-(V*100/U);}}void ğ(){if(!À[č]){É("ConstructionComponent");É("GirderComponent");É
("MetalGrid");É("InteriorPlate");É("SteelPlate");É("SmallTube");É("LargeTube");É("MotorComponent");É("Display");É(
"BulletproofGlass");É("ComputerComponent");É("ReactorComponent");É("ThrustComponent");É("GravityGeneratorComponent");É("MedicalComponent")
;É("RadioCommunicationComponent");É("DetectorComponent");É("ExplosivesComponent");É("SolarCell");É("PowerCell");É(
"Superconductor");É("PrototechPanel");É("PrototechCapacitor");É("PrototechPropulsionUnit");É("PrototechMachinery");É(
"PrototechCircuitry");É("PrototechCoolingUnit");Ñ("Position0010_OxygenBottle");Ó("Position0020_HydrogenBottle");É("Position0030_Canvas");Õ(
"Position0040_Datapad");Í("Position0005_FlareGun","FlareGunItem");Ë("Position0005_FlareGunMagazine","FlareClip");Ë(
"Position0007_FireworksBoxBlue");Ë("Position00071_FireworksBoxGreen");Ë("Position00072_FireworksBoxRed");Ë("Position00073_FireworksBoxYellow");Ë(
"Position00074_FireworksBoxPink");Ë("Position00075_FireworksBoxRainbow");Í("Position0010_AngleGrinder");Í("Position0020_AngleGrinder2");Í(
"Position0030_AngleGrinder3");Í("Position0040_AngleGrinder4");Í("Position0050_HandDrill");Í("Position0060_HandDrill2");Í("Position0070_HandDrill3");
Í("Position0080_HandDrill4");Í("Position0090_Welder");Í("Position0100_Welder2");Í("Position0110_Welder3");Í(
"Position0120_Welder4");Í("Position0010_SemiAutoPistol");Í("Position0020_FullAutoPistol");Í("Position0030_EliteAutoPistol");Í(
"Position0040_AutomaticRifle");Í("Position0050_RapidFireAutomaticRifle");Í("Position0060_PreciseAutomaticRifle");Í(
"Position0070_UltimateAutomaticRifle");Í("Position0080_BasicHandHeldLauncher");Í("Position0090_AdvancedHandHeldLauncher");Ë(
"Position0010_SemiAutoPistolMagazine");Ë("Position0020_FullAutoPistolMagazine");Ë("Position0030_ElitePistolMagazine");Ë(
"Position0040_AutomaticRifleGun_Mag_20rd");Ë("Position0050_RapidFireAutomaticRifleGun_Mag_50rd");Ë("Position0060_PreciseAutomaticRifleGun_Mag_5rd");Ë(
"Position0080_NATO_25x184mmMagazine");Ë("Position0070_UltimateAutomaticRifleGun_Mag_30rd");Ë("Position0090_AutocannonClip");Ë("Position0100_Missile200mm");Ë
("Position0110_MediumCalibreAmmo");Ë("Position0120_LargeCalibreAmmo");Ë("Position0130_SmallRailgunAmmo");Ë(
"Position0140_LargeRailgunAmmo");Ï("Position0010_CookMammalMeat","MammalMeatCooked");Ï("Position0020_CookSpiderMeat","InsectMeatCooked");Ï(
"Position0030_MealPack_KelpCrisp");Ï("Position0040_MealPack_FruitBar");Ï("Position0050_MealPack_GardenSlaw");Ï("Position0060_MealPack_RedPellets");Ï(
"Position0070_MealPack_Chili");Ï("Position0080_MealPack_Flatbread");Ï("Position0090_MealPack_Ramen");Ï("Position0100_MealPack_FruitPastry");Ï(
"Position0110_MealPack_VeggieBurger");Ï("Position0120_MealPack_Curry");Ï("Position0130_MealPack_GreenPellets");Ï("Position0140_MealPack_Dumplings");Ï(
"Position0150_MealPack_Spaghetti");Ï("Position0160_MealPack_Lasagna");Ï("Position0170_MealPack_Burrito");Ï("Position0180_MealPack_FrontierStew");Ï(
"Position0190_MealPack_SearedSabiroid");Ï("Position0200_MealPack_SteakDinner");Ø("Position0010_Seeds_Fruit","Fruit");Ø("Position0020_Seeds_Grain","Grain");Ø(
"Position0030_Seeds_Vegetables","Vegetables");Ø("Position0040_Spores_Mushrooms","Mushrooms");}º=Ď;É("Composting","Compost");É("CrateofTomatoes",
"CrateTomato");É("BottleWater","DrinkingWater");É("ToSmallArms","SmallArms");É("ToHeavyArms","HeavyArms");É("ToTools","ToolPack");É(
"ToAmmo","AmmoCache");É("ToMedicalCache","MedicalCache");É("ToRepairCache","RepairCache");É("toMachinePart",
"UpgradedBelterComponent");É("toUpgradedBelterComp","UpgradedBelterComponent");É("toOPAExpComponent","OPAExpComponent");É("IcetoNPTradeGoods",
"IceBox");É("RawMaterialsCraft","RawMaterials");É("RawMaterialsUse","InnerComponent");É("ComponentCreationTycho",
"TychoComponent");É("ComponentCreationCeres","CeresComponent");É("ComponentCreationVesta","VestaComponent");É("ComponentCreationMedina",
"MedinaComponent");É("PassengerCreationTycho","CeresPassenger");É("PassengerCreationCeres","TychoPassenger");É("PassengerCreationVesta",
"CeresPassenger");É("PassengerCreationGanymede","CeresPassenger");É("PassengerCreationMedina","CeresPassenger");É("SurviviorConsume",
"CeresPassenger");É("WelWalaConsume","CeresPassenger");É("toInnerComp","InnerComponent");É("toMCRNComponent","MCRNComponent");É(
"toMCRNAdvComponent","MCRNAdvComponent");É("toMCRNExpComponent","MCRNExpComponent");É("toUNNComponent","UNNComponent");É("toUNNAdvComponent"
,"UNNAdvComponent");É("toUNNExpComponent","UNNExpComponent");É("ToLithiumPowerCell","LithiumCell");É("toResearchInnerUNN"
,"Research");É("toResearchInnerMCRN","Research");É("toResearchBelter","Research");É("Fuel_Processing","Fuel_Tank");É(
"BulkTungsten","BulkTungstenCrate");É("BulkCopper","BulkCopperCrate");É("BulkLead","BulkLeadCrate");É("BulkTitanium",
"BulkTitaniumCrate");É("BulkIron","BulkIronCrate");É("BulkNickel","BulkNickelCrate");É("BulkCobalt","BulkCobaltCrate");É("BulkMagnesium",
"BulkMagnesiumCrate");É("BulkSilicon","BulkSiliconCrate");É("BulkSilver","BulkSilverCrate");É("BulkGold","BulkGoldCrate");É("BulkUranium",
"BulkUraniumCrate");º=ď;Ü("SubFresh","","Algae-Soy Product");Ü("WaterFood","","Drinking Water Packet");Ü("OrganicToNutrients","Nutrients",
"Nutrients");Ü("ArtificialFood");Ü("LuxuryMeal");Ü("SabiroidSteak");Ü("VeganFood");Ü("WolfSteak");Ü("WolfBouillon");Ü(
"SabiroidBouillon");Ü("CoffeeFood","","Coffee");Ü("EmergencyWater","WaterFood","Emergency Water Packets");Ü("EmergencyFood");Ý(
"ClonedAnimalMeat","WolfMeat");Ý("ClonedSabiroidMeat","SabiroidMeat");Ü("Potatoes");Ü("Tomatoes");Ü("Carrots");Ü("Cucumbers");Ü(
"PotatoSeeds");Ü("TomatoSeeds");Ü("CarrotSeeds");Ü("CucumberSeeds");Ü("Ketchup");Ü("MartianSpecial");Ü("OrganicToFertilizer",
"Fertilizer","Organic Fertilizer");Ü("IngotsToFertilizer","Fertilizer","Artificial Fertilizer");Ü("NotBeefBurger");Ü("ToFurkey","",
"ToFurkey Dinner");Ü("SpaceMealBar");Ü("HotChocolate");Ü("SpacersBreakfast");Ü("ProteinShake");º=Đ;É("AzimuthSuperchargerComponent",
"AzimuthSupercharger");º=đ;É("Naquadah","","Naquadah Bars");É("Trinium","","Trinium Plate");É("Neutronium","","Neutronium Crate");if(!À[đ]){º
=Ē;É("Naquadah","","Naquadah Bars");}º=ē;Í("Blueprint_PaintGun","PhysicalPaintGun");Ë("Blueprint_PaintGunMag",
"PaintGunMag");º=Ĕ;É("Magnetron_Component");Ü("DeuteriumOreToIngot","DeuteriumContainer","Deuterium");Ü("StonetoDeuterium",
"DeuteriumContainer","Deuterium (Stone)");Ü("IcetoDeuterium","DeuteriumContainer","Deuterium (Ice)");º=ĕ;É("ShieldComponentBP",
"ShieldComponent","Field Emitter");º=Ė;Ë("RailGunAmmoMag");º=ė;Ë("TorpedoMk1_Blueprint","TorpedoMk1");Ë("SwarmMissileMk1_Blueprint",
"SwarmMissile50mm");Ë("DestroyerMissileX_Blueprint","DestroyerMissileX");Ë("DestroyerMissileMk1_Blueprint","DestroyerMissileMk1");º=č;Ü(ö.
ø,"Uranium","Nuclear Fuel");É("CopperWire");É("Electromagnet");É("Lightbulb");É("AcidPowerCell");É("AlkalinePowerCell");É
("HeatingElement");É("POConstructionComponent","Construction");É("POSteelPlate","SteelPlate");É("POSmallTube","SmallTube"
);É("POLargeTube","LargeTube");É("POMotorComponent","Motor");É("POComputerComponent","Computer");É(
"PORadioCommunicationComponent","RadioCommunication");É("Electromagnet");É("Lightbulb");É("AlkalinePowerCell");É("HeatingElement");É("POMetalGrid",
"MetalGrid");É("GoldWire");É("POSuperconductor","Superconductor");É("CompositeArmor");É("TitaniumPlate");É("ArmoredPlate");É(
"POInteriorPlate","InteriorPlate");É("POGirderComponent","Girder");É("ArmorGlass");É("Ceramic");É("POBulletproofGlass","BulletproofGlass"
);É("Concrete");É("Capacitor");É("Cryocooler");É("POMedicalComponent","Medical");É("POSolarCell","SolarCell");É(
"Thermocouple");É("AdvancedComputer");É("PODisplay","Display");É("PODetectorComponent","Detector");É("Fabric");É("POCanvas","Canvas");
É("TokamakBlanket");É("SuperMagnet");É("LaserEmitter");É("POReactorComponent","Reactor");É("POPowerCell","PowerCell");É(
"ElectronMatrix");É("QuantumComputer");É("POThrustComponent","Thrust");É("POGravityGeneratorComponent","GravityGenerator");É(
"FSSolarCell");É("POExplosivesComponent","Explosives");Ü("Gunpowder","Magnesium");Ë("POInteriorTurret_Mag_50rd",
"InteriorTurret_Mag_50rd");Ë("PONATO_25x184mmMagazine","NATO_25x184mm");Ë("DUNATO_25x184mmMagazine","DUNATO_25x184mm");Ë("POMissile200mm",
"Missile200mm");Ë("POFullAutoPistolMagazine","FullAutoPistolMagazine");Ë("POElitePistolMagazine","ElitePistolMagazine");Ë(
"POAutomaticRifleGun_Mag_20rd","AutomaticRifleGun_Mag_20rd");Ë("PORapidFireAutomaticRifleGun_Mag_50rd","RapidFireAutomaticRifleGun_Mag_50rd");Ë(
"POPreciseAutomaticRifleGun_Mag_5rd","PreciseAutomaticRifleGun_Mag_5rd");Ë("POUltimateAutomaticRifleGun_Mag_30rd","UltimateAutomaticRifleGun_Mag_30rd");Ë(
"AutocannonClipDUAP");Ë("MediumCalibreAmmoHE");Ë("MediumCalibreAmmoDUAP");Ë("LargeCalibreAmmoHE");Ë("LargeCalibreAmmoDUAP");Ë(
"SmallRailgunAmmoDUAP");É("PolymerToPlastic","Plastic");É("Rubber");Ü("SyntheticPolymer","Polymer");É("Asphalt");Í("POWelder","WelderItem");Í(
"POAngleGrinder","AngleGrinderItem");Í("POHandDrill","HandDrillItem");Í("POSemiAutoPistol","SemiAutoPistolItem");Ë(
"POSemiAutoPistolMagazine","SemiAutoPistolMagazine");Ñ("POOxygenBottle","OxygenBottle");Ó("POHydrogenBottle","HydrogenBottle");Í("POWelder2",
"Welder2Item");Í("POAngleGrinder2","AngleGrinder2Item");Í("POHandDrill2","HandDrill2Item");Í("POAutomaticRifle","AutomaticRifleItem")
;Í("POPreciseAutomaticRifle","PreciseAutomaticRifleItem");Í("PORapidFireAutomaticRifle","RapidFireAutomaticRifleItem");Í(
"POBasicHandHeldLauncher","BasicHandHeldLauncherItem");Í("POFullAutoPistol","FullAutoPistolItem");Í("POWelder3","Welder3Item");Í(
"POAngleGrinder3","AngleGrinder3Item");Í("POHandDrill3","HandDrill3Item");Í("POUltimateAutomaticRifle","UltimateAutomaticRifleItem");Í(
"POAdvancedHandHeldLauncher","AdvancedHandHeldLauncherItem");Í("POEliteAutoPistol","ElitePistolItem");Í("POWelder4","Welder4Item");Í(
"POAngleGrinder4","AngleGrinder4Item");Í("POHandDrill4","HandDrill4Item");º=Ę;É("Foam");É("BuoyancyTube");º=ę;Ë("GravelMag");Ë(
"GravelMagBig");Ë("155mmAPShell");Ë("155mmDUAPShell");Ë("155mmHEShell");Ë("305mmAPShell");Ë("305mmDUAPShell");Ë("305mmHEShell");Ë(
"APCoilgunShell");Ë("DUAPCoilgunShell");Ë("CLGGMag","CLGG");º=Ě;Ï("SparklingWater");Ï("Emergency_Ration");º=ě;Ü("Soya");Ü("Herbs");Ï(
"AppleJuice");Ï("ApplePie");Ï("Tofu");Ï("MeatRoasted");Ï("ShroomSteak");Ï("Bread");Ï("Burger");Ï("Soup");Ï("MushroomSoup");Ï(
"TofuSoup");Ï("SparklingWaterCan","SparklingWater");Ï("EuropaTea");Ï("FarmedMushrooms","Mushrooms");Ï("FarmedApples","Apple");Ü(
"FarmedWheat","Wheat");Ü("FarmedHerbs","Herbs");Ü("FarmedSoya","Soya");Ü("FarmedPumpkin","Pumpkin");Ü("FarmedCabbage","Cabbage");º=Ĝ;
É("AryxLynxon_FusionComponentBP","AryxLynxon_FusionComponent","Fusion Coil");º=ĝ;É("K_HSR_Component_Rail_Vanilla",
"K_HSR_RailComponents");Ë("K_HSR_Ammuntion_Recipe_Slug","K_HSR_Slug");É("K_HSR_Component_AssemblerStabilizer_Vanilla","K_HSR_AssemblerSystem")
;É("K_HSR_Component_Rail","K_HSR_RailComponents");É("K_HSR_Component_PulseSystem","K_HSR_PulseSystem");É(
"K_HSR_Component_Globe","K_HSR_Globe");É("K_HSR_Component_Circuitry","K_HSR_HyperConductiveCircuitry");É("K_HSR_Component_HexPlate",
"K_HSR_HexagolPlating");É("K_HSR_Component_Conduit","K_HSR_GelConduit");É("K_HSR_Component_GlobeII","K_HSR_Globe_II");É(
"K_HSR_Component_RailII","K_HSR_RailComponents_II");É("K_HSR_Component_HexPlateII","K_HSR_HexagolPlating_II");É("K_HSR_Component_RailIII",
"K_HSR_RailComponents_III");Ë("K_HSR_Ammuntion_Recipe_Slug_Automated","K_HSR_Slug");Ü("K_HSR_NaniteSludge_Recipe","K_HSR_Nanites_Sludge");Ü(
"K_HSR_EnergizedGel_Recipe","K_HSR_Nanites_EnergizedGel");Ü("K_HSR_Hexagol_Recipe","K_HSR_Nanites_Hexagol");Ü("K_HSR_Chromium_Recipe",
"K_HSR_Nanites_Chromium");º=Ğ;Ë("R75ammo","","75mm Railgun Ammo");Ë("R150ammo","","150mm Railgun Ammo");Ë("R250ammo","","250mm Railgun Ammo");Ë(
"H203Ammo","","203mm HE Ammo");Ë("H203AmmoAP","","203mm AP Ammo");Ë("C30Ammo","","30mm Standard Ammo");Ë("C30DUammo","",
"30mm DU Ammo");Ë("CRAM30mmAmmo","","30mm C-RAM Ammo");Ë("C100mmAmmo","","100mm HE Ammo");Ë("C300AmmoAP","","300mm AP Ammo");Ë(
"C300AmmoHE","","300mm HE Ammo");Ë("C300AmmoG","","300mm Guided Ammo");Ë("C400AmmoAP","","400mm AP Ammo");Ë("C400AmmoHE","",
"400mm HE Ammo");Ë("C400AmmoCluster","","400mm Cluster Ammo");Ë("C500AmmoAP","","500mm AP Ammo");Ë("C500AmmoHE","","500mm HE Ammo");Ë(
"C500AmmoCasaba","","500mm Casaba Ammo");Ë("PlasmaCell10MJ","","10MJ PlasmaCannon Cell");º="";}public class E{public Dictionary<string,
string>Ġ=new Dictionary<string,string>();c ġ=new c(150);public c f=new c(150);bool Ģ=false;public bool o(string ģ,bool Ĥ=false
){if(ġ.ĥ(ģ))return Ģ;ġ.Ħ();ġ.e(ģ);Ġ.Clear();var ħ=ġ.ToString().ToLower();if(ħ.Contains("(sms")){ġ.Ĩ(f,0,ħ.IndexOf("(sms")
-1);f.ĩ();foreach(var Ī in ħ.Split('(')){if(Ī.Contains("sms")&&Ī.Contains(")")){foreach(var Æ in ī(Ī).Split(',',')')){if(
Æ!=""&&Æ!="Sms"){var Ĭ=Æ.IndexOf(':');if(Ĭ>-1)ĭ(Æ.Substring(0,Ĭ),Æ.Substring(Ĭ+1));else ĭ(Æ);}}if(Ĥ&&Ģ==false)Į=true;Ģ=
true;return true;}}}f.Ħ();f.e(ģ.Trim());if(Ĥ&&Ģ==true)Į=true;Ģ=false;return false;}void ĭ(string į,string İ=""){if(!q(į))Ġ.
Add(į,İ);}public bool q(string į){return Ġ.ContainsKey(į);}public bool O(){return Ģ;}}bool ı=true;static bool r=true;static
bool Ĳ=true;static bool p=true;static bool ĳ=true;bool Ĵ=true;bool ĵ=true;bool Ķ=true;int ķ=10;ĸ Ĺ=new ĸ(3);static int ĉ=80;
static string ĺ="";const int Ļ=300,ļ=5000;bool Ľ=true;static Dictionary<string,float>ľ=new Dictionary<string,float>();static
bool Į=true;static List<Ŀ>ŀ=new List<Ŀ>();List<IMyRefinery>Ł=new List<IMyRefinery>();List<IMyAssembler>ł=new List<
IMyAssembler>();static List<A>Ń=new List<A>();List<IMyUserControllableGun>ń=new List<IMyUserControllableGun>();List<IMyTerminalBlock
>Ņ=new List<IMyTerminalBlock>();List<string>ņ=new List<string>();string Ň="PIM controlled Guns";static List<ň>ŉ=new List<
ň>();static List<Ŋ>ŋ=new List<Ŋ>();Dictionary<string,Ō>ō=new Dictionary<string,Ō>();static Dictionary<string,bool>À=new
Dictionary<string,bool>();List<string>Â=new List<string>();string º=ù;static Dictionary<string,C>é=new Dictionary<string,C>();
static Dictionary<string,C>Ä=new Dictionary<string,C>();double Ŏ=0f;int ŏ=1;int Ő,ő=0;List<string>Œ;static
IMyGridProgramRuntimeInfo œ;IMyProgrammableBlock Ŕ;const string ŕ="PIM v1.1",Ŗ="c (c) BelaOkuma\n",ŗ="SMS v1.4",Ř="(sms,storage)";const string ř=
"### Config ###",Ś="### Config End ###",ś="  / =================================\n",Ŝ="UseConveyor";const string ŝ=
"Autocrafting_threshold";const string ù="Vanilla",Ď="SigmaDraconisCoreMod",ĝ="HSR_Mod",Ğ="NorthWindWeaponsMod",Ĝ="AryxEpsteinDriveMod",ě=
"PlantAndCookMod",Ě="EatDrinkSleepRepeatMod",ę="IndustrialOverhaulLockLoadMod",Ę="IndustrialOverhaulWaterMod",č="IndustrialOverhaulMod",ď
="DailyNeedsSurvivalMod",Đ="AzimuthThrusterMod",Ē="StarGateMod_Gates",đ="StarGateMod_Ores",ē="PaintGunMod",Ĕ=
"DeuteriumReactorMod",ĕ="DefenseShieldMod",Ė="MCRN_RailGunMod",ė="MWI_HomingWeaponryMod";const string ò="Tools&Guns",õ="Food",Ş="Component",Û
="Ingot",ş=Û+" ",Š=Ş+" ",Ô="Datapad",Î="ConsumableItem",š=Î+" ",Ţ="PhysicalObject",ţ=Ţ+" ",Ì="PhysicalGunObject",Ò=
"GasContainerObject",Ð="OxygenContainerObject",Ê="AmmoMagazine",Ť="MyObjectBuilder_",Ö="SeedItem",ť=Ö+" ";static Dictionary<string,Ŧ>ŧ=new
Dictionary<string,Ŧ>();static Dictionary<string,Ũ>ũ=new Dictionary<string,Ũ>();static Ũ ŭ(string Ū,float ū){if(!ũ.ContainsKey(Ū)){
var Ŭ=new Ũ(ū);ũ.Add(Ū,Ŭ);}return ũ[Ū];}static string Ų(string Ů,float Ċ,float ū){var Ű=Ċ==0?"  ":Ũ.ů(Ċ);return ŭ("@@@"+Ů,ū
).ű(Ů,Ű);}static string Ų(int Ċ,float ū,bool ų=false){var Ŵ=Ċ==0?"  ":Ċ.ToString();return ŭ(Ŵ+ū.ToString(),ū).ŵ(Ŵ,ų);}
static string Ų(float Ċ,float ū,bool ų=false){var Ű=Ċ==0?"  ":Ũ.ů(Ċ);return ŭ(Ű+ū.ToString(),ū).ŵ(Ű,ų);}static string Ŷ(int Ċ,
float ū,bool ų=false){var Ŵ=Ċ.ToString();return ŭ("###"+Ŵ+ū.ToString()+ų.ToString(),ū).ŵ(Ŵ,ų);}static string Ų(string Ŵ,float
ū,bool ų=false){return ŭ(Ŵ+ū.ToString()+ų.ToString(),ū).ŵ(Ŵ,ų);}class Ũ{const char ŷ='\u00AD';static List<Ÿ>Ź=new List<Ÿ>
();static Dictionary<char,float>ź=new Dictionary<char,float>();class Ÿ{static int Ż=700;int ż=5;public float Ž=0;string ž
="";public Ÿ(float ſ){Ž=ſ;if(Ž<0.6f)return;int ƀ=(int)(Ž/1.29166f);int Ɓ=(int)((Ž-(ƀ*1.29166f))/0.287035f);if(Ɓ>=ƀ){ƀ+=1;
Ɓ=0;}else ƀ-=Ɓ;ž+=new String(' ',ƀ);ž+=new String(ŷ,Ɓ);}public string ƃ(){ż=5;Ƃ();return ž;}static void Ƃ(){if(--Ż<1){for
(int l=Ź.Count-1;l>0;l--){Ÿ Ƅ=Ź[l];if(--Ƅ.ż<0)Ź.Remove(Ƅ);}Ż=500;}}}string ƅ="",Ɔ="",Ƈ="",ƈ="";float Ž=0;public Ũ(float Ɖ
){Ž=Ɖ;}public string ű(string Ɗ,string Ƌ){if(Ɗ!=ƅ||Ƌ!=Ɔ){var ƍ=ƌ(Ƌ);Ɗ=Ǝ(Ɗ,Ž-2.6f-ƍ);ƈ=Ə(Ž-ƌ(Ɗ)-ƍ);ƅ=Ɗ;Ɔ=Ƌ;}Ƈ=ƅ+ƈ+Ɔ;return
Ƈ;}public string ŵ(string Ɛ,bool Ƒ=false){if(Ɛ!=ƅ){Ɛ=Ǝ(Ɛ,Ž-2.6f);ƈ=Ə(Ž-ƌ(Ɛ));ƅ=Ɛ;}if(Ƒ)Ƈ=ƅ+ƈ;else Ƈ=ƈ+ƅ;return Ƈ;}string
Ǝ(string Ɛ,float ƒ){var Ɠ=ƌ(Ɛ);if(Ɠ>ƒ){var Ɣ=(int)((Ɠ-ƒ-5f)/1.5f);if(Ɣ>0&&Ɣ<Ɛ.Length-1){var ƕ=Ɛ.Substring(Ɣ);return Ɛ[0]+
"..."+ƕ;}}return Ɛ;}static Ÿ Ƙ(float Ɩ){foreach(Ÿ Ƅ in Ź){if(Ƅ.Ž==Ɩ)return Ƅ;}Ÿ Ɨ=new Ÿ(Ɩ);Ź.Add(Ɨ);return Ɨ;}static string Ə
(float Ɩ){return Ƙ(Ɩ).ƃ();}static void ƚ(){ƙ("\n",0f);ƙ("'|ÎÏ",1f);ƙ(" !`Iiîïjl",1.29166f);ƙ("(),.:;[]{}1ft",1.43076f);ƙ(
"\"-r",1.57627f);ƙ("*",1.72222f);ƙ("\\",1.86f);ƙ("/",2.16279f);ƙ("«»Lvx_ƒ",2.325f);ƙ("?7Jcçz",2.44736f);ƙ(
"3FKTaäàâbdeèéêëghknoöôpqsuüùûßyÿ",2.58333f);ƙ("+<>=^~EÈÉÊË",2.73529f);ƙ("#0245689CÇXZ",2.90625f);ƙ("$&GHPUÜÙÛVYŸ",3f);ƙ("AÄÀÂBDNOÖÔQRS",3.20689f);ƙ("%",
3.57692f);ƙ("@",3.72f);ƙ("M",3.875f);ƙ("æœmw",4.04347f);ƙ("WÆŒ",4.65f);ź.Add(ŷ,1.578695f);}static void ƙ(string Æ,float ƛ){
foreach(var Ɯ in Æ)ź.Add(Ɯ,ƛ);}static float ƌ(string Ɲ){if(ź.Count==0)ƚ();float ƞ=0;foreach(var Ɯ in Ɲ)ƞ+=ź.ContainsKey(Ɯ)?ź[Ɯ]
:2f;return ƞ;}public static string ů(double Ɵ){return Ơ(Ɵ,ơ);}public static string ƣ(double Ɵ){return Ơ(Ɵ,Ƣ);}static
string[]Ƣ=new string[]{" 0.# m "," 0.#   "," 0.# k"," 0.# M"},ơ=new string[]{" 0.0 g  "," 0.0 kg"," 0.0 T  "," 0.0 kT"};static
string Ơ(double K,string[]Ƥ){if(K>900000.0f)return(K/1000000).ToString(Ƥ[3]);else if(K>900.0f)return(K/1000).ToString(Ƥ[2]);
else if(K<1.0f)(K*1000).ToString(Ƥ[0]);return K.ToString(Ƥ[1]);}}class Ŧ:IComparable<Ŧ>{static string ƥ="";static public
void Ʀ(string æ){ƥ=æ;}string f="";string Ƨ="";public string æ="";Dictionary<string,int>ƨ=new Dictionary<string,int>();C Ʃ;
public float ƪ=1,ƫ=0;public List<ň>ŉ=new List<ň>();public int CompareTo(Ŧ M){var ƭ=Ƭ(ƥ);var Ʈ=M.Ƭ(ƥ);if(ƭ==Ʈ)return 0;if(Ʈ>ƭ)
return 1;return-1;}public Ŧ(string Ư){f=Ư;Ʃ=ư(Ư);æ=f.Substring(f.IndexOf(' ')+1);if(Ʃ==null)Ƨ=æ;else Ƨ=Ʃ.ã;}public void Ƴ(
string Ʊ,int Ʋ){if(Ʋ<0)Ʋ=0;else if(Ʋ>10)Ʋ=10;if(!ƨ.ContainsKey(Ʊ))ƨ.Add(Ʊ,Ʋ);else ƨ[Ʊ]=Ʋ;}public int Ƭ(string ƴ){if(ƨ.
ContainsKey(ƴ))return ƨ[ƴ];else return 0;}public string Ƶ(){return Ƨ;}public void ƶ(){if(ľ.ContainsKey(f)){ƪ=ľ[f]/ƫ;if(ƪ>1)ƪ=1;}
else ƪ=1;}}class ƻ{public string Ʒ="";public string Ƹ="";List<ň>ƹ=new List<ň>();public List<Ŧ>ŧ=new List<Ŧ>();public ƻ(
string æ,string ƺ){Ƹ=æ;Ʒ=ƺ;}public void ƽ(Ŧ Ƽ){if(!ŧ.Contains(Ƽ)){ŧ.Add(Ƽ);Ƽ.Ƴ(Ƹ,ŧ.Count);}}public void ƿ(ň ƾ){if(!ƹ.Contains(
ƾ))ƹ.Add(ƾ);}public void ǀ(ň ƾ){if(ƹ.Contains(ƾ))ƹ.Remove(ƾ);}public bool ǁ(){return ƹ.Count==0;}public Ŧ ǃ(string ǂ){
return ŧ.Find(K=>K.æ==ǂ);}}static Dictionary<string,ƻ>Ǆ=new Dictionary<string,ƻ>();static ƻ ǆ(ň ƾ){if(!Ǆ.ContainsKey(ƾ.ƴ)){Ǆ.
Add(ƾ.ƴ,new ƻ(ƾ.ƴ,ƾ.ǅ.DefinitionDisplayNameText));Ǆ[ƾ.ƴ].ƿ(ƾ);return Ǆ[ƾ.ƴ];}Ǆ[ƾ.ƴ].ƿ(ƾ);return null;}static void ǈ(){var Ǉ
=Ǆ.Keys.ToArray();for(int l=Ǉ.Length-1;l>=0;l--){if(Ǆ[Ǉ[l]].ǁ())Ǆ.Remove(Ǉ[l]);}}static Ŧ Ǌ(string ǉ){if(!ŧ.ContainsKey(ǉ
))ŧ.Add(ǉ,new Ŧ(ǉ));return ŧ[ǉ];}abstract class Ŀ{public IMyInventory ǋ=null;public Dictionary<string,float>ǌ=new
Dictionary<string,float>();abstract public bool Ǎ();public void ǘ(){if(!Ǎ())return;var ǎ=new List<MyInventoryItem>();var Ǐ=new
List<string>();var ǐ=new Dictionary<string,float>();ǋ.GetItems(ǎ);for(int l=ǎ.Count-1;l>=0;l--){var Ǒ=new List<
MyInventoryItem>();ǋ.GetItems(Ǒ,ǒ=>ǒ.Type==ǎ[l].Type);if(Ǒ.Count>1){ǋ.TransferItemTo(ǋ,Ǒ[Ǒ.Count-1]);}}ǎ.Clear();ǋ.GetItems(ǎ);for(int
l=ǎ.Count-1;l>=0;l--){var Ǔ=ǎ[l];var Ʊ=ǔ(Ǔ.Type);if(!ǌ.ContainsKey(Ʊ))Ǖ(ǋ,Ʊ,Ǔ);else{var ǖ=ǌ[Ʊ]-(float)Ǔ.Amount;if(ǖ<0){Ǖ(
ǋ,Ʊ,Ǔ,Math.Abs(ǖ));Ǐ.Add(Ʊ);}else if(ǖ==0)Ǐ.Add(Ʊ);else ǐ.Add(Ʊ,ǖ);}}foreach(var l in ǌ.Keys){if(Ǐ.Contains(l))continue;Ǘ
(l,(ǐ.ContainsKey(l)?ǐ[l]:ǌ[l]),ǋ);}}}static C m(MyProductionItem Ǚ){var n=t(Ǚ);if(n!=null)n.W+=Ǚ.Amount.ToIntSafe();
return n;}ǚ ǝ(MyItemType í){foreach(var Æ in Ǜ)if(Æ.æ==í)return Æ;var ǜ=new ǚ(í);Ǜ.Add(ǜ);return ǜ;}static string ǔ(MyItemType
æ){return æ.TypeId.Substring(æ.TypeId.IndexOf('_')+1)+" "+æ.SubtypeId;}static C ư(string ª){foreach(var Ǟ in é.Values)if(
Ǟ.â==ª)return Ǟ;foreach(var Ǟ in Ä.Values)if(Ǟ.â==ª)return Ǟ;return null;}static C t(MyProductionItem Ǚ){foreach(var Ǟ in
é.Values)if(Ǟ.P.SubtypeName==Ǚ.BlueprintId.SubtypeName)return Ǟ;foreach(var Ǟ in Ä.Values)if(Ǟ.P.SubtypeName==Ǚ.
BlueprintId.SubtypeName)return Ǟ;return null;}class Ō{public string æ="";public double ǟ=0,Ǡ=0;public Ō(string Æ){æ=Æ;}public void
ǡ(double Ɯ,double Ĉ){ǟ+=Ɯ;Ǡ+=Ĉ;}public int Ǣ(){return(int)(ǟ*100/Ǡ);}}class ǚ:IComparable<ǚ>{public enum Ǧ{ǣ,Ǥ,ǥ,}static
public Ǧ ǧ=Ǧ.ǣ;public enum Ǯ{ǣ,Ǩ,ǩ,Ǫ,ǫ,Ǭ,ǭ,}static public Ǯ ǯ=Ǯ.ǣ;static IMyInventory ǰ=null;static IMyInventory Ǳ=null;static
public void ǲ(){ǰ=null;Ǳ=null;}static public void ǳ(IMyInventory ǋ){if(ǰ==null||(ǰ.MaxVolume<ǋ.MaxVolume))ǰ=ǋ;if(Ǳ==null||(Ǳ.
MaxVolume-Ǳ.CurrentVolume<ǋ.MaxVolume-ǋ.CurrentVolume))Ǳ=ǋ;}class ǣ:IComparable<ǣ>{public int ǌ=0;public float Ċ=0;public float Ǵ
=0;public IMyInventory ǋ=null;public ǣ(IMyInventory l,MyFixedPoint K){Ċ=(float)K;ǋ=l;ǵ();}public void ǵ(){ǌ=ǋ.ItemCount;Ǵ
=(float)(ǋ.MaxVolume-ǋ.CurrentVolume);}public float Ƕ(){return(float)ǋ.MaxVolume;}public int CompareTo(ǣ M){if(ǯ==Ǯ.Ǩ){if
(M.Ċ==Ċ)return 0;return M.Ċ>Ċ?1:-1;}else if(ǯ==Ǯ.ǩ){if(M.Ċ==Ċ)return 0;return M.Ċ<Ċ?1:-1;}else if(ǯ==Ǯ.Ǫ){if(ǌ==1&&M.ǌ==1
)return 0;else if(ǌ==1)return 1;if(M.Ċ==Ċ)return 0;return M.Ċ<Ċ?1:-1;}else if(ǯ==Ǯ.ǫ){if(M.ǌ==ǌ)return 0;return M.ǌ<ǌ?1:-
1;}else if(ǯ==Ǯ.Ǭ){if(M.Ǵ==Ǵ)return 0;return M.Ǵ>Ǵ?1:-1;}else if(ǯ==Ǯ.ǭ){if(M.Ǵ==Ǵ)return 0;return M.Ǵ<Ǵ?1:-1;}return 0;}
}public MyItemType æ;float Ƿ=1;int Ǹ=0;public int ǹ{get{return Ǹ;}}float Ċ=0;float Ǻ=0;List<ǣ>ǻ=new List<ǣ>();
IMyInventory Ǽ=null,ǽ=null;public ǚ(MyItemType Ǿ){æ=Ǿ;Ƿ=æ.GetItemInfo().Volume;}void ǿ(){foreach(var l in ǻ)l.ǵ();}public void ȁ(
IMyInventory ǋ,MyFixedPoint Ȁ){Ǹ++;Ċ+=(float)Ȁ;Ǻ=Ċ*Ƿ;ǻ.Add(new ǣ(ǋ,Ȁ));}public int CompareTo(ǚ M){if(ǧ==Ǧ.ǣ){if(M.Ǹ==Ǹ){if(M.Ċ==Ċ)
return 0;return M.Ċ>Ċ?1:-1;}return M.Ǹ>Ǹ?1:-1;}else if(ǧ==Ǧ.Ǥ){if(M.Ǻ==Ǻ)return 0;return M.Ǻ>Ǻ?1:-1;}else if(ǧ==Ǧ.ǥ){if(M.Ǻ==Ǻ
)return 0;return M.Ǻ<Ǻ?1:-1;}return 0;}public bool Ȃ(){if(Ǹ==2){if(ǻ[0].Ƕ()>Ǻ){Ǽ=ǻ[1].ǋ;ǽ=ǻ[0].ǋ;return true;}else if(ǻ[1
].Ƕ()>Ǻ){Ǽ=ǻ[0].ǋ;ǽ=ǻ[1].ǋ;return true;}}return false;}public bool Ȇ(){var ȃ=new List<MyInventoryItem>();ǽ.GetItems(ȃ);
bool Ȅ=true;for(int Ĭ=ȃ.Count-1;Ĭ>=0;Ĭ--){var l=ȃ[Ĭ];if(l.Type!=æ){if(!Ǽ.TransferItemFrom(ǽ,l,null))Ȅ=false;}}var ȅ=Ǽ.
FindItem(æ);if(ȅ!=null)Ǽ.TransferItemTo(ǽ,(MyInventoryItem)ȅ,null);return(Ȅ&&ȅ==null);}public bool ȇ(){if(Ǹ<2)return true;if(((
float)(Ǳ.MaxVolume-Ǳ.CurrentVolume))<Ǻ)return false;foreach(var l in ǻ){if(l.ǋ!=Ǳ){var ȅ=l.ǋ.FindItem(æ);if(ȅ!=null){Ǳ.
TransferItemFrom(l.ǋ,(MyInventoryItem)ȅ,null);}}}return true;}public void ȉ(){if(Ǹ>1){ǿ();ǯ=Ǯ.Ǫ;ǻ.Sort();int Ĭ=0;int Ȉ=ǻ.Count-1;for(;Ĭ<
Ȉ;Ȉ--){var ȅ=ǻ[Ȉ].ǋ.FindItem(æ);if(ȅ!=null){if(ǻ[Ĭ].ǋ.TransferItemFrom(ǻ[Ȉ].ǋ,(MyInventoryItem)ȅ,null))Ĭ++;}}}}public
void ȋ(){if(Ǹ<2)return;ǿ();ǯ=Ǯ.Ǭ;ǻ.Sort();if(ǻ[0].Ǵ<Ǻ){var ǽ=ǻ[0].ǋ;ǯ=Ǯ.ǩ;ǻ.Sort();foreach(var Ȋ in ǻ){if(Ȋ.ǋ!=ǽ){var ȅ=Ȋ.ǋ.
FindItem(æ);if(ȅ!=null){ǽ.TransferItemFrom(Ȋ.ǋ,(MyInventoryItem)ȅ,null);}}}}else{for(int Ĭ=1;Ĭ<ǻ.Count-1;Ĭ++){var l=ǻ[Ĭ].ǋ;var ȅ
=l.FindItem(æ);if(ȅ!=null){ǻ[0].ǋ.TransferItemFrom(l,(MyInventoryItem)ȅ,null);}}}}public void Ȏ(){if(Ǹ<2)return;for(int Ĭ
=ǻ.Count-1;Ĭ>0;Ĭ--){var l=ǻ[Ĭ].ǋ;for(int Ȉ=Ĭ-1;Ȉ>=0;Ȉ--){if(l==ǻ[Ȉ].ǋ){var Ȍ=new List<MyInventoryItem>();l.GetItems(Ȍ,ȍ=>
ȍ.Type==æ);if(Ȍ.Count>1){l.TransferItemFrom(l,Ȍ[Ȍ.Count-1],Ȍ[Ȍ.Count-1].Amount);}}}}}}DateTime ȏ=DateTime.Now;static List
<ǚ>Ǜ=new List<ǚ>();int Ȑ=0;string ȑ="";int Ȓ=0;static string[]ȓ=new string[]{"Component","Ore","Ingot"};bool ȕ(string Ȕ){
return Convert.ToBoolean(Ȕ);}void Ș(){var Ȗ="  / attention!!!\n  / autocraftingconfig now via LCD Display,\n  / place a LCD and add '..(sms,autocrafting) to the name.\n  / follow the instructions, multiple lcds are possible'\n\n"
+ř+"\n\n"+ś+"  / options set to 'True' or 'False'.\n  / to activate changes, please restart script\n"+ś+
"\n  / show info on programmable blocks LCD\n"+"ShowInfoPBLcd="+ı.ToString()+"\n\n"+
"  / delete item from the production list (Assemblers)\n  / when the maximum value is reached.\n"+"delete_queueItem_if_max="+r.ToString()+"\n\n"+
"  / always recycle grey water on the Water Recycling System Block\n  / (only Daily Needs Survival Mod)\n"+"always_recycle_greywater="+Ĳ.ToString()+"\n\n"+"  / turn all assemblers off when production queue is empty\n"+
"assemblers_off="+p.ToString()+"\n\n"+"  / turn all refinerys off when inbound inventory is empty\n"+"refinerys_off="+ĳ.ToString()+"\n\n"
+"  / collect all ore\n"+"collect_all_Ore="+Ĵ.ToString()+"\n\n"+"  / collect all ingot\n"+"collect_all_Ingot="+ĵ.ToString
()+"\n\n"+"  / collect all component\n"+"collect_all_Component="+Ķ.ToString()+"\n\n"+
"  / stackingcycle in seconds, 0 = stacking off\n"+"stacking_cycle="+ķ.ToString()+"\n\n"+"  / group of PIM controlled Weapons\n  / Control of WeaponCore Turrets is not necessary\n  / and should remain switched off.\n"
+"PIM_controlled_Weapons="+Ň+"\n\n"+ś+"  / mods that can be used.\n  /     is there a mod missing? \n  /           write it in the comments of SMS or PIM\n\n"
;foreach(var ȗ in À.Keys)Ȗ+=ȗ+"="+À[ȗ].ToString()+"\n";Ȗ+="\n"+Ś+"\n";Me.CustomData=Ȗ;}void Ȝ(){string[]ș={ď,Đ,Ē,đ,ē,Ĕ,ĕ,
Ė,ė,č,ę,Ę,Ě,ě,Ĝ,Ğ,ĝ,Ď,};foreach(var Ĉ in ș)À.Add(Ĉ,false);bool Ț=false;foreach(var ț in Me.CustomData.Split('\n')){var Æ=
ț.Trim();if(Æ.Length==0||Æ[0]=='/')continue;else if(Æ==ř){Ț=true;continue;}else if(Æ==Ś)break;if(Ț){var î=Æ.Split('=');if
(î.Length<2)continue;switch(î[0]){case"ShowInfoPBLcd":ı=ȕ(î[1]);break;case"delete_queueItem_if_max":r=ȕ(î[1]);break;case
"always_recycle_greywater":Ĳ=ȕ(î[1]);break;case"assemblers_off":p=ȕ(î[1]);break;case"refinerys_off":ĳ=ȕ(î[1]);break;case"collect_all_Ore":Ĵ=ȕ(î[1]
);break;case"collect_all_Ingot":ĵ=ȕ(î[1]);break;case"collect_all_Component":Ķ=ȕ(î[1]);break;case"stacking_cycle":int.
TryParse(î[1],out ķ);break;case"PIM_controlled_Weapons":Ň=î[1];break;default:if(À.ContainsKey(î[0]))À[î[0]]=ȕ(î[1]);break;}
continue;}}Ș();}List<string>ȝ=new List<string>();void ȟ(){foreach(var Ȟ in é.Values)if(!ȝ.Contains(Ȟ.ä))ȝ.Add(Ȟ.ä);}string Ƞ=new
string(' ',85);const string ȡ="AutocraftingTypes";Ȣ ȣ=new Ȣ();void ȸ(){var Ȥ=new List<IMyTextPanel>();GridTerminalSystem.
GetBlocksOfType<IMyTextPanel>(Ȥ,ȥ=>ȥ.CustomName.Contains(Ȧ)&&Me.CubeGrid==ȥ.CubeGrid);foreach(var Ȫ in Ȥ){var ȧ=Ş+","+Ê;var Ȩ=false;var
ȩ=new Dictionary<string,string>();foreach(var ț in Ȫ.GetText().Split('\n')){var ȫ=ț.Split('|','=','%',':');for(int l=0;l<
ȫ.Count();l++)ȫ[l]=ȫ[l].Trim(' ','\u00AD');if(ȫ.Count()==0||ȫ[0]==""||ȫ[0][0]=='/')continue;else if(ȫ[0]==ȡ&&ȫ.Count()>1)
{if(ȫ[1]!="")ȧ=ȫ[1];}else if(ȫ[0]==ŝ){var Ȭ=ĉ;ĉ=ȭ(ȫ[1]);if(ĉ==0)ĉ=80;if(Ȭ!=ĉ)Ȩ=true;}else if(ȫ[0]=="Type"&&ȫ.Count()>2){
if(!ȩ.ContainsKey(ȫ[1]))ȩ.Add(ȫ[1],ȫ[2]);}else if(ȫ.Count()==6){if(é.ContainsKey(ȫ[4]))é[ȫ[4]].ą(Ȯ(ȫ[2]));}}if(Ȩ)C.ì();var
ȯ="/ Autocraftingdefinition:\n";ȯ+=
"/ add '...(sms)' to the name of assemblers to crafting their items,\n/ and set the max quantity as you want\n\n";ȯ+="/ if the quantity of items falls below this percentage value,\n/ then it will be increased to max.\n"+ŝ+" = "+ĉ+
"%\n\n";ȯ+="/ possible autocrafting types, please add them separated by comma.\n/ ";foreach(var í in ȝ)ȯ+=í+",";ȯ+="\n"+ȡ+"="+ȧ
;ȯ+="\n\n/                           Item            |     current    ­|       max      ­|   assembly\n";var Ȱ=ȧ.Split(
',').Select(Æ=>Æ.Trim()).Where(Æ=>!string.IsNullOrEmpty(Æ)).ToArray();foreach(var Ȳ in Ȱ){ȯ+=ȱ+"\n Type : "+Ȳ+" = ";if(ȩ.
ContainsKey(Ȳ)){ȯ+=(ȩ[Ȳ]==""?" * ":ȩ[Ȳ]);ȣ.ȳ(ȩ[Ȳ]);}else{ȯ+=" * ";ȣ.ȴ();}ȯ+=ȵ;var ȷ=é.Values.ToList().FindAll(Ǟ=>Ǟ.ä==Ȳ&&ȣ.ȶ(Ǟ.ã));
ȷ.Sort((Ĭ,Ȉ)=>Ĭ.ã.CompareTo(Ȉ.ã));foreach(var ê in ȷ){ȯ+=" "+Ų(ê.ã,60)+" | "+Ų(ê.V,25)+" | "+Ų(ê.U,25)+" | "+Ų((ê.W>0?ê.W
:(ê.ß>0?ê.ß:0)),25)+Ƞ+"|"+ê.å+"|\n";}}Ȫ.Alignment=TextAlignment.LEFT;Ȫ.ContentType=ContentType.TEXT_AND_IMAGE;Ȫ.WriteText
(ȯ);}}class Ȣ{List<string>ȹ=new List<string>();List<string>Ⱥ=new List<string>();public void ȴ(){Ⱥ.Clear();ȹ.Clear();ȹ.Add
("*");}public void ȳ(string Ȼ){Ⱥ.Clear();ȹ.Clear();foreach(var Æ in Ȼ.Split(',')){var ȼ=Æ.Trim();if(ȼ.Length>0&&ȼ[0]=='-'
)Ⱥ.Add(ȼ.Substring(1));else ȹ.Add(ȼ);}}public bool ȶ(string Ƚ){foreach(string Æ in Ⱥ)if(Ƚ.Contains(Æ))return false;
foreach(string Æ in ȹ)if(Æ=="*"||Ƚ.Contains(Æ))return true;return false;}}void ɂ(){var Æ=ŕ+Ŗ+Ⱦ()+(Ŕ==null?(" Running / "+ȿ+
" inst. per run\ncurrent cycle: "+Ŏ.ToString("0.0")+" sec.\n"+ɀ):"Standby\nMaster: "+Ŕ.CustomName);Echo(Æ);if(ı){var Ɂ=Me.GetSurface(0);Ɂ.Alignment=
TextAlignment.LEFT;Ɂ.ContentType=ContentType.TEXT_AND_IMAGE;Ɂ.WriteText(Æ);}}public
 Program
(){Ƀ.Add(new Ʉ());Ƀ.Add(new Ʌ());Ƀ.Add(new Ɇ());Ƀ.Add(new ɇ());Ȝ();ğ();Ɉ();ȿ=Ļ;œ=Runtime;ɉ();œ.UpdateFrequency=
UpdateFrequency.Update10;if(Ĵ)ņ.Add(Ť+"Ore");if(ĵ)ņ.Add(Ť+Û);if(Ķ)ņ.Add(Ť+Ş);ȑ=ȓ[0];}string Ɏ(){string Ɋ=
"Accepted BluePrints by Refinerysubtype\n";foreach(string Ɍ in ö.ɋ.Keys){Ɋ+="SubType:"+Ɍ+"\n";foreach(var ɍ in ö.ɋ[Ɍ]){Ɋ+="bprint -> "+ɍ.f+"\n";}}return Ɋ;}string
ɏ(){string Ɋ="Accepted BluePrints by Assemblersubtype\nPool:\n";foreach(var N in Ä){Ɋ+=N.Value.ä+" -> "+N.Value.ã+" / "+N
.Value.P+"\n";}Ɋ+="Active:\n";foreach(var N in é){Ɋ+=N.Value.ä+" -> "+N.Value.ã+" / "+N.Value.P+"\n";}return Ɋ;}string ɕ(
){var Ɋ="";foreach(var ɑ in ɐ.Keys){Ɋ+="IPrioList:"+ɑ+"\n";foreach(var ɒ in ɐ[ɑ]){Ɋ+="\t- "+ɒ.ɓ.ɔ+" % "+ɒ.ƭ+"\n";}}return
Ɋ;}string ɖ(){var Ɋ="";foreach(var ȅ in é.Values){Ɋ+=" # "+ȅ.ã+" -> "+ȅ.w+"\n";}return Ɋ;}string ɛ(){string Ɋ=
"Refineryrecipes\n";foreach(var ɘ in ɗ){Ɋ+=ɘ.f+" : "+ɘ.ə+" -> "+ɘ.ɚ+"\n";}return Ɋ;}string ɞ(){var Ɋ="InventoryManagerList:\n";foreach(var
ɝ in ɜ.Keys){Ɋ+=" - Key: "+ɝ+" / "+ɜ[ɝ].Count+" Inventorys\n";}return Ɋ;}string ɡ(){var Ɋ="Guns\n";foreach(var ǅ in ŉ){Ɋ
+=" * "+ǅ.ǅ.CustomName+" / "+ǅ.ɟ+"\n";foreach(var ȅ in ǅ.ɠ){Ɋ+="   - "+ȅ.Key+" / "+ȅ.Value+"\n";}}return Ɋ;}void ɣ(){var ɢ
=GridTerminalSystem.GetBlockWithName("PIMXXXDEBUG")as IMyTextPanel;if(ɢ==null)return;if(ɢ.CubeGrid!=Me.CubeGrid)return;
var Æ="";Æ+=ɞ();ɢ.WriteText(Æ+"\n"+ĺ);ĺ="";}bool ɤ(){return œ.CurrentInstructionCount>ȿ;}DateTime ɥ=DateTime.Now;void
 Main
(string ɦ,UpdateType ɧ){if(ɦ!=""){switch(ɦ.ToLower()){case"flushrefinerys_all":foreach(var Q in ɨ)Q.ɩ();ɪ("all ("+ɨ.Count
+") refinerys flushed.");break;default:ɪ("unknow command: \""+ɦ+"\"");break;}return;}do{switch(ŏ){case 0:if(Ĺ.ɫ())ŏ++;
else{ɂ();return;}Ŏ=(DateTime.Now-ɥ).TotalSeconds;ɥ=DateTime.Now;break;case 1:if(!Į){ĺ+="kein calc_ACDef\n";ŏ+=2;break;}ĺ+=
"calc_ACDef\n";GridTerminalSystem.GetBlocksOfType<IMyAssembler>(ł,ȥ=>ȥ.CubeGrid==Me.CubeGrid);GridTerminalSystem.GetBlocksOfType<
IMyRefinery>(Ł,ȥ=>ȥ.CubeGrid==Me.CubeGrid);Œ=new List<string>(é.Keys);for(int l=Œ.Count-1;l>=0;l--){var Ǟ=é[Œ[l]];Ä.Add(Œ[l],Ǟ);é.
Remove(Œ[l]);}Œ=new List<string>(Ä.Keys);Ő=Œ.Count-1;ŏ++;Į=false;break;case 2:for(int l=Ő;l>=0;l--,Ő--){if(ɤ()){ɂ();return;}
var Ǟ=Ä[Œ[l]];if(Œ[l]==ɬ.ɭ||Œ[l]==(ö.ɮ)){foreach(var ɯ in Ł){var ɰ=ɯ.BlockDefinition.SubtypeId;if(ɯ.CustomName.Contains(
"(sms")&&(ɰ.Contains("Hydroponics")||ɰ.Contains("Reprocessor"))){é.Add(Œ[l],Ǟ);Ä.Remove(Œ[l]);break;}}}else{foreach(var K in ł
){if(K.CustomName.Contains("(sms")&&K.CanUseBlueprint(Ǟ.P)){é.Add(Œ[l],Ǟ);Ä.Remove(Œ[l]);break;}}}}ȟ();ŏ++;break;case 3:
if(!ɉ()){ɂ();return;}ȸ();ɣ();ɱ(ľ);ɲ.Clear();ɳ.Clear();ō.Clear();foreach(var ɴ in ɜ.Values)ɴ.Clear();ɜ.Clear();ŏ++;break;
case 4:GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(Ņ,ȥ=>ȥ.IsSameConstructAs(Me));Ő=0;ŏ++;break;case 5:for(int l=Ő;
l<Ņ.Count;l++,Ő++){if(ɤ()){ɂ();return;}ɵ(Ņ[l]);}ŏ++;break;case 6:GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(Ņ,ȥ
=>ȥ.IsSameConstructAs(Me));Ő=0;ŏ++;break;case 7:for(int l=Ő;l<Ņ.Count;l++,Ő++){if(ɤ()){ɂ();return;}ɵ(Ņ[l]);}ŏ++;break;case
8:GridTerminalSystem.GetBlocksOfType<IMyShipController>(Ņ,ȥ=>ȥ.IsSameConstructAs(Me));Ő=0;ŏ++;break;case 9:for(int l=Ő;l<
Ņ.Count;l++,Ő++){if(ɤ()){ɂ();return;}ɵ(Ņ[l]);}ŏ++;break;case 10:var ɶ=GridTerminalSystem.GetBlockGroupWithName(Ň);if(ɶ==
null){if(ŉ.Count>0){for(int l=ŉ.Count-1;l>=0;l--)ŉ[l].ɷ();ŉ.Clear();}ŏ++;ŏ++;break;}else ɶ.GetBlocksOfType<
IMyUserControllableGun>(ń,ȥ=>ȥ.IsSameConstructAs(Me));for(int l=ŉ.Count-1;l>=0;l--){if(ń.Contains(ŉ[l].ǅ))ń.Remove(ŉ[l].ǅ);else{var ǅ=ŉ[l];ǅ.ɷ
();ŉ.Remove(ǅ);}}Ő=ń.Count-1;ŏ++;break;case 11:for(int l=Ő;l>=0;l--,Ő--){if(ɤ()){ɂ();return;}ŉ.Add(new ň(ń[l]));}ŏ++;
break;case 12:Ő=0;ŏ++;break;case 13:for(int l=Ő;l<ŉ.Count;l++,Ő++){if(ɤ()){ɂ();return;}ŉ[l].x();}ŏ++;break;case 14:
GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(Ņ,ɸ=>(ɸ.CustomName.Contains(Ř)));for(int l=ŋ.Count-1;l>=0;l--){if(Ņ.Contains(ŋ[l].ɹ)
)Ņ.Remove(ŋ[l].ɹ);else{var ɺ=ŋ[l];ɺ.ɷ();ŋ.Remove(ɺ);}}Ő=Ņ.Count-1;ŏ++;break;case 15:for(int l=Ő;l>=0;l--,Ő--){if(ɤ()){ɂ()
;return;}ŋ.Add(new Ŋ(Ņ[l]));}ŏ++;break;case 16:ŏ++;break;case 17:ŏ++;break;case 18:ŏ++;break;case 19:ŏ++;break;case 20:ŏ
++;break;case 21:ŏ++;break;case 22:ŏ++;break;case 23:ŏ++;break;case 24:GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(Ņ
,ȥ=>ȥ.IsSameConstructAs(Me));Ő=0;ŏ++;break;case 25:for(int l=Ő;l<Ņ.Count;l++,Ő++){if(ɤ()){ɂ();return;}var ɻ=Ņ[l].
BlockDefinition.SubtypeId.ToString();if(ɻ.Contains("ShipLaserMultitool")){bool ɼ=true;var ɽ=Ņ[l].GetProperty("ToolMode");if(ɽ!=null&&ɽ.
TypeName=="Boolean")ɼ=Ņ[l].GetValue<bool>("ToolMode");if(ɼ){if(!(Ņ[l]as IMyFunctionalBlock).Enabled)ɵ(Ņ[l]);}else ɵ(Ņ[l]);}else
if(!(Ņ[l]as IMyFunctionalBlock).Enabled)ɵ(Ņ[l]);}ŏ++;break;case 26:GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(Ņ,ȥ
=>ȥ.IsSameConstructAs(Me));Ő=0;ŏ++;break;case 27:for(int l=Ő;l<Ņ.Count;l++,Ő++){if(ɤ()){ɂ();return;}ɵ(Ņ[l]);}ŏ++;break;
case 28:GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(Ņ,ȥ=>ȥ.IsSameConstructAs(Me));Ő=0;ŏ++;break;case 29:for(int l=Ő;l<Ņ
.Count;l++,Ő++){if(ɤ()){ɂ();return;}ɵ(Ņ[l]);}ŏ++;break;case 30:Ő=0;if(ķ==0||(DateTime.Now-ȏ).TotalSeconds<ķ)ŏ+=2;else{Ǜ.
Clear();ǚ.ǲ();if(ɜ.ContainsKey(ȑ)){foreach(var l in ɜ[ȑ]){ɾ(l,ȑ);ǚ.ǳ(l);}}if(Ȑ==0){int ɿ=0;foreach(var Æ in Ǜ)if(ɿ<Æ.ǹ)ɿ=Æ.ǹ;
if(ɿ>1){ǚ.ǧ=ǚ.Ǧ.ǣ;Ǜ.Sort();}else{Ȑ=-1;}}else if(Ȑ==1){int ɿ=0;foreach(var Æ in Ǜ)if(ɿ<Æ.ǹ)ɿ=Æ.ǹ;if(ɿ>1){ǚ.ǧ=ǚ.Ǧ.ǣ;Ǜ.Sort()
;}else{Ȑ=-1;}}else if(Ȑ==2){ǚ.ǧ=ǚ.Ǧ.ǥ;Ǜ.Sort();}else if(Ȑ==3){ǚ.ǧ=ǚ.Ǧ.Ǥ;Ǜ.Sort();}else if(Ȑ==4){ǚ.ǧ=ǚ.Ǧ.ǣ;Ǜ.Sort();while(
Ǜ.Count>0){var Æ=Ǜ[0];if(Æ.Ȃ())break;Ǜ.Remove(Æ);}}ȏ=DateTime.Now;Ő=0;}ŏ++;break;case 31:switch(Ȑ){case 0:for(int l=Ő;l<Ǜ
.Count;l++,Ő++){if(ɤ()){ɂ();return;}Ǜ[l].Ȏ();}ŏ++;Ȑ++;break;case 1:for(int l=Ő;l<Ǜ.Count;l++,Ő++){if(ɤ()){ɂ();return;}Ǜ[l
].ȋ();}ŏ++;Ȑ++;break;case 2:for(int l=Ő;l<Ǜ.Count;l++,Ő++){if(ɤ()){ɂ();return;}if(!Ǜ[l].ȇ())break;}ŏ++;Ȑ++;break;case 3:
for(int l=Ő;l<Ǜ.Count;l++,Ő++){if(ɤ()){ɂ();return;}Ǜ[l].ȉ();}ŏ++;Ȑ++;break;case 4:if(Ǜ.Count>0)for(int l=Ő;l<300;l++,Ő++){
if(ɤ()){ɂ();return;}if(Ǜ[0].Ȇ())break;}ŏ++;Ȑ++;break;default:Ȑ=0;Ȓ++;if(Ȓ>=ȓ.Length)Ȓ=0;ȑ=ȓ[Ȓ];ŏ++;break;}break;case 32:ŏ
++;break;case 33:foreach(var Ǟ in é.Values)Ǟ.W=0;foreach(var Ǟ in Ä.Values)Ǟ.W=0;GridTerminalSystem.GetBlocksOfType<
IMyRefinery>(Ł,ȥ=>ȥ.CubeGrid==Me.CubeGrid);for(int l=ɨ.Count-1;l>=0;l--){if(Ł.Contains(ɨ[l].ʀ))Ł.Remove(ɨ[l].ʀ);else{Į=true;ɨ.
Remove(ɨ[l]);}}ö.ʁ="";Ő=Ł.Count-1;ŏ++;break;case 34:for(int l=Ő;l>=0;l--,Ő--){if(ɤ()){ɂ();return;}ɨ.Add(new ö(Ł[l]));}ŏ++;
break;case 35:GridTerminalSystem.GetBlocksOfType<IMyAssembler>(ł,ȥ=>ȥ.CubeGrid==Me.CubeGrid);for(int l=Ń.Count-1;l>=0;l--){if
(ł.Contains(Ń[l].G))ł.Remove(Ń[l].G);else{Į=true;Ń.Remove(Ń[l]);}}Ő=ł.Count-1;ŏ++;break;case 36:for(int l=Ő;l>=0;l--,Ő--)
{if(ɤ()){ɂ();return;}Ń.Add(new A(ł[l]));}ŏ++;break;case 37:Ő=0;ŏ++;break;case 38:for(int l=Ő;l<ɲ.Count;l++,Ő++){if(ɤ()){ɂ
();return;}k(ɲ[l]);}ŏ++;break;case 39:Ő=0;ŏ++;break;case 40:for(int l=Ő;l<ɳ.Count;l++,Ő++){if(ɤ()){ɂ();return;}k(ɳ[l],ņ);
}ŏ++;break;case 41:Ő=0;foreach(var K in ŧ.Values)K.ƶ();ŏ++;break;case 42:for(int l=Ő;l<ŀ.Count;l++,Ő++){if(ɤ()){ɂ();
return;}ŀ[l].ǘ();}ŏ++;break;case 43:ö.ʂ=0;foreach(var Ǟ in ɗ)Ǟ.ʃ=0;Ő=0;ŏ++;break;case 44:for(int l=Ő;l<ɨ.Count;l++,Ő++){if(ɤ()
){ɂ();return;}ɨ[l].x();}ŏ++;break;case 45:ʄ();ʅ();ʆ();Ő=0;ŏ++;break;case 46:for(int l=Ő;l<ɨ.Count;l++,Ő++){if(ɤ()){ɂ();
return;}ɨ[l].ʇ();}ŏ++;break;case 47:Ő=0;ŏ++;break;case 48:for(int l=Ő;l<Ń.Count;l++,Ő++){if(ɤ()){ɂ();return;}Ń[l].x();}ŏ++;
break;case 49:Œ=new List<string>(é.Keys);Ő=0;ŏ++;break;case 50:for(int l=Ő;l<Œ.Count;l++,Ő++){if(ɤ()){ɂ();return;}var Ǟ=é[Œ[l
]];Ǟ.ċ((int)ľ.GetValueOrDefault(Ǟ.â,0));Ǟ.Č();if(Ǟ.Ć()){if(Œ[l]!=ɬ.ɭ){foreach(var Q in Ń)Q.S(Ǟ);}}}ŏ++;break;case 51:Œ=
new List<string>(Ä.Keys);Ő=0;ŏ++;break;case 52:for(int l=Ő;l<Œ.Count;l++,Ő++){if(ɤ()){ɂ();return;}var Ǟ=Ä[Œ[l]];if(ľ.
ContainsKey(Ǟ.â))Ǟ.ċ((int)ľ[Ǟ.â]);}ŏ++;break;case 53:Ő=0;ő=0;Ń.Sort();ŏ++;break;case 54:for(int l=Ő;l<Ń.Count;l++,Ő++){if(ɤ()){ɂ();
return;}var Q=Ń[l];if(Q.G.CubeGrid==Me.CubeGrid&&Q.F.O()){if(Q.D.Count>0){Q.D.Sort();var Ǟ=Q.D[0];ő++;if(Q.b(Ǟ)){foreach(var ʈ
in Ǟ.Q)ʈ.D.Remove(Ǟ);}else Q.D.Remove(Ǟ);}}}if(ő==0)ŏ++;else ŏ--;break;default:for(int l=Ƀ.Count-1;l>=0;l--){if(Ƀ[l].ʉ())Ƀ
.Remove(Ƀ[l]);}Dictionary<string,string>ʓ=new Dictionary<string,string>{{ʊ.ʋ,ʌ.ʍ},{ɬ.ʋ,"Gravel"},{ʊ.ʎ,"Ice"},{ɬ.ʏ,"Water"
},{ɬ.ʐ,"Greywater"},{ɬ.ʑ,"Deuterium"},{ʊ.ʒ,"Organic"},};foreach(var ȅ in ʓ){var ʔ=(ľ.ContainsKey(ȅ.Key)&&ľ[ȅ.Key]>0&&!ɜ.
ContainsKey(ȅ.Key));ʕ(ʔ,ʖ.ʗ.ʘ,ȅ.Value);}foreach(var Ɯ in ō.Keys){var ʙ=ō[Ɯ].Ǣ();if(ʙ>=90){ʚ(ʖ.ʗ.ʛ,Ɯ);ʜ(ʖ.ʗ.ʝ,Ɯ);}else if(ʙ>=99){ʚ(ʖ
.ʗ.ʝ,Ɯ);ʜ(ʖ.ʗ.ʛ,Ɯ);}else{ʜ(ʖ.ʗ.ʛ,Ɯ);ʜ(ʖ.ʗ.ʝ,Ɯ);}}ʞ();Ľ=false;ŏ=0;break;}}while(!ɤ());ɂ();}static void ɱ(Dictionary<string
,float>ǎ){var ʟ=ǎ.Keys.ToArray();for(int l=0;l<ʟ.Length;l++)ǎ[ʟ[l]]=0;}static void ʦ(IMyInventory ʠ,Dictionary<string,
float>ʡ=null){var ʢ=new List<MyInventoryItem>();ʠ.GetItems(ʢ);foreach(var ʣ in ʢ){string ʤ=ǔ(ʣ.Type);var ʥ=(float)ʣ.Amount;if
(ľ.ContainsKey(ʤ))ľ[ʤ]+=ʥ;else ľ.Add(ʤ,ʥ);if(ʡ!=null){if(ʡ.ContainsKey(ʤ))ʡ[ʤ]+=ʥ;else ʡ.Add(ʤ,ʥ);}}}void ʪ(IMyInventory
ǋ,Dictionary<string,string>ʧ){foreach(var Ī in ʧ.Keys){var ʩ=ʨ(Ī);if(ʩ!=""){if(!ɜ.ContainsKey(ʩ))ɜ.Add(ʩ,new List<
IMyInventory>());if(!ɜ[ʩ].Contains(ǋ))ɜ[ʩ].Add(ǋ);if(!ō.ContainsKey(Ī))ō.Add(Ī,new Ō(Ī));ō[Ī].ǡ(ǋ.CurrentVolume.RawValue/1000,ǋ.
MaxVolume.RawValue/1000);}}}static Dictionary<string,List<IMyInventory>>ɜ=new Dictionary<string,List<IMyInventory>>();static List
<IMyInventory>ɲ=new List<IMyInventory>();static List<IMyInventory>ɳ=new List<IMyInventory>();void ɵ(IMyTerminalBlock í){
if(í.HasInventory){var ǋ=í.GetInventory(0);ʦ(ǋ);if(í.CustomName.Contains(Ř))return;E ʫ=new E();if(ʫ.o(í.CustomName)){if(!ʫ
.q("Keep"))ɲ.Add(ǋ);if(!ʫ.q("Infolcd"))ʪ(ǋ,ʫ.Ġ);}else if(í.BlockDefinition.SubtypeId.Contains("Container")||í.
BlockDefinition.SubtypeId.Contains("Connector")){ɲ.Add(ǋ);}else ɳ.Add(ǋ);}}int ȿ;void ʬ(){if(Ŕ!=null){œ.UpdateFrequency=UpdateFrequency
.Update100;ȿ=Ļ;}else{œ.UpdateFrequency=UpdateFrequency.Update10;if(Ŏ<3.5)ȿ-=100;else if(Ŏ>4.5)ȿ+=100;if(ȿ<Ļ)ȿ=Ļ;else if(ȿ
>ļ)ȿ=ļ;}}const string ʭ="@ASSEMBLERQUEUE;";const string ʮ="@ITEMMAX;";bool ɉ(){var ʯ=new List<IMyProgrammableBlock>();
GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(ʯ,ȥ=>ȥ.IsSameConstructAs(Me));Ŕ=null;foreach(var Ƥ in ʯ){if(Ƥ.Enabled&&Ƥ.
DetailedInfo.StartsWith(ŕ)){if(Me.EntityId<Ƥ.EntityId){Ŕ=Ƥ;break;}}else if(Ƥ.Enabled&&Ƥ.DetailedInfo.StartsWith(ŗ)&&!Ľ){var Æ="";
bool ʰ=false;foreach(var ʱ in Ƥ.CustomData.Split('\n')){if(ʱ.Contains(ř))ʰ=true;if(ʰ)Æ+=ʱ+'\n';if(ʱ.Contains(Ś))ʰ=false;}if(
Æ!="")Æ+="\n\n";foreach(var K in ɐ.Keys.ToArray())if(ɐ.ContainsKey(K)&&!ö.ʁ.Contains("@"+K))ɐ.Remove(K);foreach(var ʲ in
ɐ.Keys){Æ+="@INGOTPRIOLIST;"+ʲ+"\n";foreach(var l in ɐ[ʲ])if(l.ʳ>0)Æ+="@INGOTPRIO;"+l.ɓ.ɚ+";"+l.ʳ+"\n";}foreach(var Ɯ in
ō.Values)Æ+="@CARGOUSE;"+Ɯ.æ+";"+Ɯ.ǟ+";"+Ɯ.Ǡ+"\n";foreach(var Ǟ in é.Values){if(Ǟ.U>0)Æ+=ʮ+Ǟ.â+";"+Ǟ.U+";"+Ǟ.ç+"\n";if(Ǟ.
W>0)Æ+=ʭ+Ǟ.â+";"+Ǟ.W+";"+Ǟ.ç+"\n";else if(Ǟ.ß>0)Æ+=ʭ+Ǟ.â+";"+Ǟ.ß+";"+Ǟ.ç+"\n";}foreach(var Ǟ in Ä.Values){if(Ǟ.U>0)Æ+=ʮ+Ǟ
.â+";"+Ǟ.U+";"+Ǟ.ç+"\n";if(Ǟ.W>0)Æ+=ʭ+Ǟ.â+";"+Ǟ.W+";"+Ǟ.ç+"\n";else if(Ǟ.ß>0)Æ+=ʭ+Ǟ.â+";"+Ǟ.ß+";"+Ǟ.ç+"\n";}Ƥ.CustomData=
Æ;}}ʬ();return Ŕ==null;}static string[]ó=new string[]{ť+"Fruit",ť+"Grain",ť+"Vegetables",ť+"Mushrooms",};static string[]ʴ
=new string[]{ʊ.ʒ,ɬ.ʐ};static string[]ô=new string[]{š+"MammalMeatCooked",š+"InsectMeatCooked",š+"MealPack_KelpCrisp",š+
"MealPack_FruitBar",š+"MealPack_GardenSlaw",š+"MealPack_RedPellets",š+"MealPack_Chili",š+"MealPack_Flatbread",š+"MealPack_Ramen",š+
"MealPack_FruitPastry",š+"MealPack_VeggieBurger",š+"MealPack_Curry",š+"MealPack_GreenPellets",š+"MealPack_Dumplings",š+"MealPack_Spaghetti",š+
"MealPack_Lasagna",š+"MealPack_Burrito",š+"MealPack_FrontierStew",š+"MealPack_SearedSabiroid",š+"MealPack_SteakDinner",š+"Fruit",ţ+"Grain"
,š+"Vegetables",š+"Mushrooms",ɬ.ʏ,ɬ.ʵ,ɬ.ɭ,ɬ.ʶ,ş+"ArtificialFood",ş+"LuxuryMeal",ş+"SabiroidSteak",ş+"VeganFood",ş+
"WolfSteak",ş+"WolfBouillon",ş+"SabiroidBouillon",ş+"CoffeeFood",ş+"Potatoes",ş+"Tomatoes",ş+"Carrots",ş+"Cucumbers",ş+
"PotatoSeeds",ş+"TomatoSeeds",ş+"CarrotSeeds",ş+"CucumberSeeds",ş+"Ketchup",ş+"MartianSpecial","Ore WolfMeat","Ore SabiroidMeat",ş+
"Fertilizer",ş+"NotBeefBurger",ş+"ToFurkey",ş+"SpaceMealBar",ş+"HotChocolate",ş+"SpacersBreakfast",ş+"ProteinShake",ş+
"EmergencyFood",š+"SparklingWater",š+"Emergency_Ration",š+"AppleJuice",š+"ApplePie",š+"Tofu",š+"MeatRoasted",š+"ShroomSteak",š+"Bread",
š+"Burger",š+"Soup",š+"MushroomSoup",š+"TofuSoup",š+"EuropaTea",š+"Mushrooms",š+"Apple",š+"PrlnglesChips",š+"LaysChips",š
+"InterBeer",š+"CosmicCoffee",š+"ClangCola",š+"Meat",š+"MeatRoasted",ş+"Soya",ş+"Herbs",ş+"Wheat",ş+"Pumpkin",ş+"Cabbage"
,};static string ʷ(string í){if(í.Contains("RifleItem")||í.Contains(Ê)||í.Contains("PistolItem")||í.Contains(
"LauncherItem"))return"Armory";if(ó.Contains(í))return"Seeds";if(ô.Contains(í))return"Food";if(ʴ.Contains(í))return"Waste";return"";}
static void k(IMyInventory Ǽ,List<string>ʸ=null){var ȃ=new List<MyInventoryItem>();Ǽ.GetItems(ȃ);if(ȃ.Count()==0)return;for(
int ʹ=ȃ.Count()-1;ʹ>=0;ʹ--){bool ʺ=false;var ʻ=ȃ[ʹ].Type;var ʼ=true;if(ʸ!=null){ʼ=false;foreach(var ǜ in ʸ)if(ʻ.TypeId.
ToString()==ǜ){ʼ=true;break;}}if(!ʼ)continue;var ʽ=ʻ.TypeId.ToString().Split('_')[1];var ʾ=ʻ.SubtypeId.ToString();var ʿ=ʽ+" "+ʾ;
var ˀ=ʷ(ʿ);if(ɜ.ContainsKey(ʿ))ʺ=ˁ(Ǽ,ʹ,ɜ[ʿ]);if(!ʺ&&ˀ!=""&&ɜ.ContainsKey(ˀ))ʺ=ˁ(Ǽ,ʹ,ɜ[ˀ]);if(!ʺ&&ɜ.ContainsKey(ʽ))ˁ(Ǽ,ʹ,ɜ[ʽ
]);var ˇ=ˆ(ʽ);if(ɜ.ContainsKey(ʽ))ʜ(ʖ.ʗ.ˈ,ˇ);else ʚ(ʖ.ʗ.ˈ,ˇ);}}void ɾ(IMyInventory Ǽ,string ˉ){var ȃ=new List<
MyInventoryItem>();Ǽ.GetItems(ȃ);foreach(var l in ȃ){if(l.Type.TypeId.Contains(ˉ)&&!ɜ.ContainsKey(ǔ(l.Type))){ǝ(l.Type).ȁ(Ǽ,l.Amount);}
}}static bool Ǖ(IMyInventory Ǽ,string æ,MyInventoryItem ȅ,float Ċ=0){var y=æ.Substring(0,æ.IndexOf(' '));var ˊ=æ.
Substring(æ.IndexOf(' ')+1);var ˀ=ʷ(æ);var ˋ=false;if(Ċ==0)Ċ=(float)ȅ.Amount;if(ɜ.ContainsKey(æ))ˋ=ˌ(Ǽ,ȅ,Ċ,ɜ[æ]);else if(ˀ!=""&&ɜ
.ContainsKey(ˀ))ˋ=ˌ(Ǽ,ȅ,Ċ,ɜ[ˀ]);else if(ɜ.ContainsKey(y))ˋ=ˌ(Ǽ,ȅ,Ċ,ɜ[y]);return ˋ;}static bool ˌ(IMyInventory Ǽ,
MyInventoryItem ȅ,float Ċ,List<IMyInventory>ˍ){var Ǻ=(MyFixedPoint)Ċ*ȅ.Type.GetItemInfo().Volume;if(ˍ.Count>0){foreach(var ˎ in ˍ)if(Ǽ
==ˎ)return true;for(int l=0;l<ˍ.Count;l++){var ˏ=ˍ[l];if(Ǻ<(ˏ.MaxVolume-ˏ.CurrentVolume)){if(Ǽ.TransferItemTo(ˏ,ȅ,(
MyFixedPoint)Ċ))return true;}}}return false;}static bool ˁ(IMyInventory Ǽ,int ː,List<IMyInventory>ˍ){var ˋ=false;if(ˍ.Count!=0){
foreach(var ˎ in ˍ)if(Ǽ==ˎ)return true;for(int l=0;l<ˍ.Count;l++){var ˏ=ˍ[l];if(!ˏ.IsFull){ˋ=Ǽ.TransferItemTo(ˏ,ː,null,true,
null);}}}return ˋ;}static bool Ǘ(string Ʊ,float ˑ,IMyInventory ǽ,int?Ƥ=null){return ˠ(Ť+Ʊ.Substring(0,Ʊ.IndexOf(' ')),Ʊ.
Substring(Ʊ.IndexOf(' ')+1),ˑ,ǽ);}static bool ˠ(string ˡ,string ˢ,float ˑ,IMyInventory ǽ,int?Ƥ=null){List<IMyInventory>ˣ=null;var
ʽ=ˡ.Split('_')[1];var ˇ=ˆ(ʽ);var ˀ=ʷ(ʽ[1]+" "+ˢ);if(ɜ.ContainsKey(ʽ[1]+" "+ˢ))ˣ=ɜ[ʽ[1]+" "+ˢ];else if(ˀ!=""&&ɜ.
ContainsKey(ˀ))ˣ=ɜ[ˀ];else if(ɜ.ContainsKey(ʽ))ˣ=ɜ[ʽ];else{ʚ(ʖ.ʗ.ˈ,ˇ);return false;}ʜ(ʖ.ʗ.ˈ,ˇ);for(int l=0;l<ˣ.Count;l++){var ȃ=new
List<MyInventoryItem>();ˣ[l].GetItems(ȃ);if(ȃ.Count()>0){for(int ʹ=ȃ.Count()-1;ʹ>=0;ʹ--){if(ȃ[ʹ].Type.TypeId.ToString()==ˡ){
if(ȃ[ʹ].Type.SubtypeId.ToString()==ˢ){var ˤ=(MyFixedPoint)ˑ;if(ˣ[l].TransferItemTo(ǽ,ʹ,Ƥ,true,ˤ))return true;}}}}}return
false;}class Ŋ:Ŀ{public IMyCargoContainer ɹ=null;string ˬ="";public Ŋ(IMyTerminalBlock ˮ){ɹ=ˮ as IMyCargoContainer;ǋ=ɹ.
GetInventory();ŀ.Add(this);}const string Ͱ="StorageItemDefinition",ͱ="### "+Ͱ+"_begin ###",Ͳ="### "+Ͱ+"_end ###",ͳ="add_to_list:";
public override bool Ǎ(){if(ɹ.CustomData!=""&&ɹ.CustomData==ˬ)return true;bool ʹ=false;var Ͷ="";ǌ.Clear();foreach(var Æ in ɹ.
CustomData.Split('\n')){var ͷ=Æ.Trim();if(ͷ.StartsWith("/"))continue;else if(ͷ.Contains(ͱ))ʹ=true;else if(ͷ.Contains(Ͳ))ʹ=false;
else if(!ʹ&&ͷ.StartsWith(ͳ))Ͷ=ͷ;else if(ʹ){var ͺ=ͷ.Split(';');if(ͺ.Length>1){int Ċ=0;if(ľ.ContainsKey(ͺ[1])&&int.TryParse(ͺ[
0],out Ċ)){if(Ċ!=0)ǌ.Add(ͺ[1],Ċ);}}}}if(Ͷ!=""){var ͻ=Ͷ.Split(',',':');for(int l=1;l<ͻ.Length;l++){var ͼ=ͻ[l].Trim().
ToLower();if(ͼ=="")continue;foreach(var í in ľ.Keys){if(í.ToLower().Contains(ͼ)&&!ǌ.ContainsKey(í)){ǌ.Add(í,1);}}}}var ͽ="  / Itemdefinitionen:\n  / amount and type of items to be stored in the container\n  /\n  / add items to the list:\n  / write search terms after the '"
+ͳ+"', like 'steel' or 'tube'.\n  / close the window, after a few seconds you will find\n  / relevant items in the list below.\n"
+ͳ+"\n"+ś;ͽ+="  / List of items, delete the lines that are no longer needed,\n  / or set the value to 0.\n  / please change only the value before the semicolon\n"
+ͱ+"\n";foreach(var l in ǌ){ͽ+=l.Value+";"+l.Key+"\n";}ͽ+=Ͳ+"\n";ɹ.CustomData=ͽ;ˬ=ͽ;return true;}public void ɷ(){ŀ.Remove
(this);}}class ň:Ŀ{public IMyUserControllableGun ǅ=null;public string ƴ="";public string ɟ="";public List<Ŧ>Ά=new List<Ŧ>
();public Dictionary<string,int>ɠ=new Dictionary<string,int>();public ň(IMyUserControllableGun Έ){ǅ=Έ;ƴ=ǅ.BlockDefinition
.SubtypeId;ǋ=Έ.GetInventory();List<MyItemType>Ή=new List<MyItemType>();ǋ.GetAcceptedItems(Ή);var Ί=0;ƻ Ό=null;foreach(var
K in Ή)if(K.TypeId.EndsWith(Ê)&&K.SubtypeId!="Energy")Ί++;if((Ί>1)&&!(ǅ is IMyLargeInteriorTurret)){Ό=ǆ(this);}var Ύ=(Ί>1
)&&!(ǅ is IMyLargeInteriorTurret)?true:false;foreach(var K in Ή){if(K.TypeId.EndsWith(Ê)&&K.SubtypeId!="Energy"){var Ώ=Ê+
' '+K.SubtypeId;var ΐ=Ǌ(Ώ);ΐ.ŉ.Add(this);Ά.Add(ΐ);var Α=(int)((float)ǋ.MaxVolume/K.GetItemInfo().Volume);ΐ.ƫ+=Α;ɠ.Add(Ώ,Α);
if(Ό!=null)Ό.ƽ(ΐ);}}ŀ.Add(this);}public override bool Ǎ(){if(ǅ is IMyLargeInteriorTurret)return false;if(ɟ=="")return
false;int Β=(int)(ɠ[ɟ]*ŧ[ɟ].ƪ);if(Β<1)Β=1;if(ǌ.Count==0)ǌ.Add(ɟ,Β);else if(!ǌ.ContainsKey(ɟ)){ǌ.Clear();ǌ.Add(ɟ,Β);}else ǌ[ɟ]
=Β;return true;}public void x(){ʦ(ǋ);var Γ=ǅ.GetProperty(Ŝ);if(Γ!=null&&ǅ.GetValue<bool>(Ŝ))ǅ.ApplyAction(Ŝ);ɟ=Δ();}
string Δ(){if(ɠ.Count==0)return"";else if(ɠ.Count==1)return ɠ.Keys.First();var Ε="";var Ζ=0;foreach(var K in ɠ){var ƭ=Ǌ(K.Key)
.Ƭ(ƴ);if(ƭ>Ζ&&ľ.ContainsKey(K.Key)&&ľ[K.Key]>0){Ε=K.Key;Ζ=ƭ;}}return Ε;}public void ɷ(){var Ǉ=ŧ.Keys.ToArray();for(int l=
ŧ.Count-1;l>=0;l--){if(ŧ[Ǉ[l]].ŉ.Contains(this)){ŧ[Ǉ[l]].ŉ.Remove(this);if(ŧ[Ǉ[l]].ŉ.Count==0)ŧ.Remove(Ǉ[l]);else ŧ[Ǉ[l]]
.ƫ-=ɠ[Ǉ[l]];break;}}if(ŀ.Contains(this))ŀ.Remove(this);var Ƥ=ǅ.GetProperty(Ŝ);if(Ƥ!=null&&!ǅ.GetValue<bool>(Ŝ))ǅ.
ApplyAction(Ŝ);}}public class ĸ{DateTime Η=DateTime.Now;int Θ;public ĸ(int Ι=5){Θ=Ι;}public bool ɫ(bool Κ=true){if(Θ==0)return
false;if((DateTime.Now-Η).TotalSeconds>Θ){if(Κ)Η=DateTime.Now;return true;}return false;}}static string Ν(double Λ){if(Λ<1){
if((Λ*60)>1)return Math.Round(Λ*60,0)+" min.";else return Math.Round(Λ*60*60,0)+" s";}if(Λ<24){return Math.Round(Λ,1)+" h"
;}double Μ=Math.Round(Λ/24,1);if(Μ>365){return Math.Round(Μ/365,1)+" years";}if(Μ<1.1)return Μ+" day";else return Μ+
" days";}void ʄ(){foreach(var Ο in ɐ.Values)foreach(Ξ ɒ in Ο)ɒ.Π(0);foreach(var Ɍ in ö.ɋ.Keys){foreach(var Ρ in ö.ɋ[Ɍ]){var Τ=Ρ
.Σ;if(ľ.ContainsKey(Τ)&&ľ[Τ]>0){if(Ρ.Υ)Φ(Ɍ,Ρ,9999);else{var Χ=ľ[Ρ.Σ];var Ω=ľ.GetValueOrDefault(Ρ.Ψ,0);if(Ω==0)Φ(Ɍ,Ρ,200);
else if(Ω<500)Φ(Ɍ,Ρ,150);else if(Ω<Χ)Φ(Ɍ,Ρ,100-(int)(Ω/(Χ/97.0f)));else Φ(Ɍ,Ρ,1);}}}}Ϊ();}const string Ϋ="(sms,oreprio)";
const string ά="(sms,refining)";const string Ȧ="(sms,autocrafting)";const string έ="(sms,ammoprio)";void Ϊ(){var Ȥ=new List<
IMyTextPanel>();GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(Ȥ,ȥ=>ȥ.CustomName.Contains(Ϋ));if(Ȥ.Count>0){ö.ή();Dictionary<ί,int
>ΰ=null;Dictionary<IMyTextPanel,string>α=new Dictionary<IMyTextPanel,string>();foreach(var Ȫ in Ȥ){var β="";var γ=Ȫ.
GetText().Split('\n');foreach(var Æ in γ){var δ=Æ.Split(':','=','|');for(int l=0;l<δ.Count();l++){δ[l]=δ[l].Trim(' ','\u00AD');
}if(δ.Length==0||δ[0].Length==0||δ[0][0]=='/')continue;if(δ.Length>=5&&δ[3].ToLower().StartsWith("oreprio")&&(ΰ!=null)){
var ε=ɗ.Find(Ǟ=>Ǟ.f==δ[4]);if(ε!=null){int ƭ=-1;if(!int.TryParse(δ[1],out ƭ))ƭ=-1;if(!ΰ.ContainsKey(ε))ΰ.Add(ε,-1);ΰ[ε]=(ƭ<
0?-1:(ƭ>10000?10000:ƭ));if(ƭ>=0){Ξ η=Ξ.ζ(ɐ[β],ε);if(η!=null&&η.ƭ>0)η.Π(ƭ);}}}else if(δ.Length>=2&&δ[0].ToLower().
StartsWith("refinerytype")){ΰ=null;β="";if((δ[1]!="")&&ö.ɋ.ContainsKey(δ[1])){if(θ.ContainsKey(δ[1])){ΰ=θ[δ[1]];}else{ΰ=new
Dictionary<ί,int>();θ.Add(δ[1],ΰ);}β=δ[1];}}else if(δ.Length>1&&δ[0]=="Filter"){if(!α.ContainsKey(Ȫ)){α.Add(Ȫ,δ[1]);}}}}ι(Ȥ,α);}}
void ι(List<IMyTextPanel>Ȥ,Dictionary<IMyTextPanel,string>α){foreach(var Ȫ in Ȥ){var κ="*";if(α.ContainsKey(Ȫ)){ȣ.ȳ(α[Ȫ]);κ=
α[Ȫ];}else{ȣ.ȴ();}var λ="/ Orepriorityconfig:\n/ only refinerytypes with '(sms)' in the name are displayed.\n\n/ set the 'value' between 1 and 10000\n/ if value = 0 then the ore will be ignored\n/ if value empty then prio will be calculated by PIM.\n/ any type of scrap is always refined first\n\n/ Refinerytypefilter, separated by comma, '*' for all\n Filter: "
+κ+"\n\n/                Recipe                       |    Value   |       Current\n";foreach(var į in ö.ɋ.Keys){if(!θ.
ContainsKey(į))θ.Add(į,new Dictionary<ί,int>());var μ=ö.ɋ[į].FindAll(Q=>!Q.Υ);if(μ.Count<2||!ȣ.ȶ(į))continue;λ+=ν+
"\n RefineryType: "+į+δ;var ξ=ɐ[į];μ.Sort((Ĭ,Ȉ)=>Ĭ.ə.CompareTo(Ȉ.ə));foreach(var ê in μ){var ο="-1";var π=Ξ.ζ(ξ,ê);if(θ[į].ContainsKey(ê))ο
=θ[į][ê].ToString();else θ[į].Add(ê,-1);λ+=" "+(Ų(ê.f,65,true))+" | "+Ų((ο=="-1"?"  |  ":ο+"  |  "),23)+((π!=null&&π.ʳ>0)
?Ų(π.ʳ.ToString(),23)+"  ":"")+Ƞ+"|OrePrio:"+ê.f+"|\n";}}Ȫ.Alignment=TextAlignment.LEFT;Ȫ.ContentType=ContentType.
TEXT_AND_IMAGE;Ȫ.WriteText(λ);}}void ʅ(){var Ȥ=new List<IMyTextPanel>();GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(Ȥ,ȥ=>ȥ.
CustomName.Contains(έ));if(Ȥ.Count!=0){var Ȫ=Ȥ[0];string[]ρ=Ȫ.GetText().Split('\n');ƻ ς=null;foreach(var Æ in ρ){var δ=Æ.Split(':'
,'=','|');for(int l=0;l<δ.Count();l++){δ[l]=δ[l].Trim(' ','\u00AD');}if(δ.Length==0||δ[0].Length==0||δ[0][0]=='/')
continue;if(δ.Length>=3&&δ[0]=="GunType"){ς=Ǆ.GetValueOrDefault(δ[2],null);}else if(δ.Length>=3&&ς!=null){var σ=ς.ǃ(δ[2]);if(σ!=
null){σ.Ƴ(ς.Ƹ,int.Parse(δ[0]));}}}var τ="/ Ammopriodefinitions:\n/ the prio only affects weapons that can use\n/ different ammunition types. this determines\n/ which one is loaded into the inventory first.\n/ 0 means that the ammunition is not used\n"
;var υ="\n"+Ų("Priority",25)+" | Ammotyp\n";foreach(var φ in Ǆ){τ+=ν+"\nGunType: "+φ.Value.Ʒ+Ƞ+"|"+φ.Value.Ƹ+υ;Ŧ.Ʀ(φ.Key)
;φ.Value.ŧ.Sort();foreach(var Ƽ in φ.Value.ŧ){τ+=Ŷ(Ƽ.Ƭ(φ.Key),25)+" | "+Ų(Ƽ.Ƶ(),60,true)+Ƞ+" | "+Ƽ.æ+"\n";}}Ȫ.Alignment=
TextAlignment.LEFT;Ȫ.ContentType=ContentType.TEXT_AND_IMAGE;Ȫ.WriteText(τ);}}void ʆ(){var Ȥ=new List<IMyTextPanel>();
GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(Ȥ,ȥ=>ȥ.CustomName.Contains(ά));if(Ȥ.Count>0){ί.χ();var ω=" Refiningprogress:\n"+ψ+"\n";
foreach(var N in ɗ){if(N.ʃ>0){ω+=Ų(N.f,70,true)+" | "+Ų(N.ə,N.ϊ,70)+"\n"+Ų(N.ʃ.ToString()+" Refinerys.",30,true)+Ų("-> "+N.ϋ,40
,true)+" | "+Ų(N.ɚ,N.ό,70)+"\n"+ψ+"\n";}}foreach(var Ȫ in Ȥ){Ȫ.Alignment=TextAlignment.LEFT;Ȫ.ContentType=ContentType.
TEXT_AND_IMAGE;Ȫ.WriteText(ω);}}}static Dictionary<string,string>ϒ=new Dictionary<string,string>{{ʊ.ʋ,ʌ.ʍ},{ɬ.ύ,ɬ.ώ},{ɬ.ʋ,"Gravel"},{ɬ
.ʑ,ʌ.Ϗ},{ʊ.ʎ,"Ice"},{ɬ.ϐ,ʌ.ϑ},};static Dictionary<string,string>ϟ=new Dictionary<string,string>{{ʊ.ϓ,ʌ.ϔ},{ʊ.ϕ,ʌ.ϖ},{ʊ.ϗ,
ʌ.Ϙ},{ɬ.ϙ,ʌ.Ϛ+" Paste"},{ɬ.ϛ,ʌ.Ϝ},{ɬ.ϗ,ʌ.ϝ+" Nitrate"},{ʊ.ύ,"Crushed Niter"},{ɬ.ύ,ɬ.Ϟ},};static string Ϡ(string ǉ){if(À[č
]&&ϟ.ContainsKey(ǉ))return ϟ[ǉ];if(ϒ.ContainsKey(ǉ))return ϒ[ǉ];if(ǉ.StartsWith("Ore Crushed"))return"Crushed "+ǉ.
Substring(11);if(ǉ.StartsWith("Ore Purified"))return"Purified "+ǉ.Substring(12);return ǉ;}const string δ="\n"+ν+"\n";const string
ν="/-------------------------------------------------------------------------";const string ȵ="\n"+ȱ+"\n";const string ȱ=
"/"+ψ;const string ψ="-------------------------------------------------------------------------------------------";static
int Ȯ(string ʱ){if(ʱ=="")return 0;var ϡ=ʱ.Split(' ');if(ϡ.Count()==0)return 0;float Ϣ;if(!float.TryParse(ϡ[0],out Ϣ))return
0;if(ϡ.Count()==2){if(ϡ[1]=="k")Ϣ*=1000;else if(ϡ[1]=="M")Ϣ*=1000000;}return(int)Ϣ;}static int ȭ(string ʱ){float Ϣ;float.
TryParse(ʱ.Trim().Split('.')[0],out Ϣ);return(int)Ϣ;}static string ī(string ϣ){var Ϥ="";var ϥ=true;foreach(var Ϧ in ϣ){if(ϥ){ϥ=
false;Ϥ+=Char.ToUpper(Ϧ);continue;}if(Ϧ==' '|Ϧ==','|Ϧ=='-'|Ϧ=='_'|Ϧ=='&'|Ϧ==':'){ϥ=true;Ϥ+=Ϧ;continue;}Ϥ+=Char.ToLower(Ϧ);}
return Ϥ;}int ɯ=0;int ϧ=1;int Ϩ=7;c ϩ=new c(10);c Ⱦ(){ϩ.Ϫ('|');ɯ+=ϧ;if(ɯ<0){ɯ=1;ϧ=1;}else if(ɯ>Ϩ){ɯ=Ϩ-1;ϧ=-1;}for(int l=0;l<=Ϩ
;l++)ϩ.e(l==ɯ?(ϧ<0?'<':'>'):' ');ϩ.e("| ");return ϩ;}static Dictionary<string,string>ϫ=new Dictionary<string,string>{{
"Ammo",Ê},{ʌ.ʍ,ʊ.ʋ},{"Gravel",ɬ.ʋ},{"Tools",Ì},{"Kits",Î},{"Cash",Ţ},{"Datapads",Ô},{"H-Bottles",Ò},{"O-Bottles",Ð},{"Ice",ʊ.ʎ
},{"Water",ɬ.ʏ},{"Greywater",ɬ.ʐ},{"Deuterium",ɬ.ʑ},{"Organic",ʊ.ʒ},{"Seeds",Ö}};static string ˆ(string ʩ){foreach(var Ĭ
in ϫ)if(Ĭ.Value==ʩ)return Ĭ.Key;return ʩ;}string ʨ(string Ϭ){if(Ϭ.Contains("Dock"))return"";if(ϫ.ContainsKey(Ϭ))return ϫ[Ϭ
];switch(Ϭ){case"Steelplate":return Š+"SteelPlate";case"Metalgrid":return Š+"MetalGrid";case"Interiorplate":return Š+
"InteriorPlate";case"Smalltube":return Š+"SmallTube";case"Largetube":return Š+"LargeTube";case"Glass":return Š+"BulletproofGlass";case
"Gravity":return Š+"GravityGenerator";case"Radio":return Š+"RadioCommunication";case"Solar":return Š+"SolarCell";case"Power":
return Š+"PowerCell";case"Zonechip":return Š+"ZoneChip";case"Reactor":case"Thrust":case"Medical":case"Detector":case
"Explosives":case"Construction":case"Motor":case"Display":case"Girder":case"Computer":case"Canvas":return Š+Ϭ;default:return Ϭ;}}
static List<ö>ɨ=new List<ö>();public class ö{public enum ϳ{ϭ,Ϯ,ϯ,ϰ,ϱ,ϲ,}public enum Ϲ{ϴ,ϵ,Ϸ,ϸ}static public string ʁ="";static
public int ʂ=0;static public Dictionary<string,List<ί>>ɋ=new Dictionary<string,List<ί>>();static public void ή(){foreach(var K
in ɋ.Keys.ToArray())if(ɋ.ContainsKey(K)&&!ʁ.Contains("@"+K))ɋ.Remove(K);}IMyInventory Ϻ,ϻ;public Dictionary<string,float>ϼ
=new Dictionary<string,float>();public Ͻ Ͼ;public E F=new E();public IMyRefinery ʀ=null;public List<Ϲ>Ͽ=new List<Ϲ>();
public List<ί>Ѐ=null;public string Ё="";public int Ђ;ί Ѓ=null;ί Є=null;float Ѕ=0;float І=0;public class Ͻ{string Ї,Ј;bool Љ;ϳ
Њ;public Ͻ(){Ї="";Љ=true;Њ=ϳ.ϭ;Ј=ϳ.ϭ.ToString();}public Ͻ(string Ћ,ϳ ÿ,string Ќ=""){Ї=Ћ;Љ=true;Њ=ÿ;Ј=Ќ;}public Ͻ(bool Ѝ,
string Ћ,ϳ ÿ,string Ќ=""){Ї=Ћ;Љ=Ѝ;Њ=ÿ;Ј=Ќ;}public ϳ Ў(){return Њ;}public string Џ(){return Ј==""?Ї:Ј;}public bool А(){return Њ
==ϳ.Ϯ;}public bool Б(){return Њ==ϳ.ϭ;}public bool Г(string В){if(Њ==ϳ.ϭ){Ј=В;return true;}if(Љ&&Ї==В)return true;else if(!
Љ&&Ї.StartsWith(В))return true;return false;}}List<Ͻ>Д=new List<Ͻ>{new Ͻ(false,"WRS",ϳ.ϯ,"Water Recycling System"),new Ͻ(
"Blast Furnace",ϳ.Ϯ,"Basic Refinery"),new Ͻ("LargeRefineryIndustrial",ϳ.Ϯ,"Large Industrial Refinery"),new Ͻ("LargeRefinery",ϳ.Ϯ,
"Large Refinery"),new Ͻ("K_HSR_Refinery_A",ϳ.Ϯ,"HSR Refinery A"),new Ͻ(false,"Hydroponics",ϳ.ϰ,"Hydroponics Farm"),new Ͻ("RockCrusher",ϳ
.Ϯ),new Ͻ("OrePurifier",ϳ.Ϯ),new Ͻ("ChemicalPlant",ϳ.Ϯ),new Ͻ("Centrifuge",ϳ.Ϯ),new Ͻ("Incinerator",ϳ.ϲ),new Ͻ(
"BitumenExtractor",ϳ.Ϯ),new Ͻ("Reprocessor",ϳ.ϱ),new Ͻ("OilCracker",ϳ.Ϯ),new Ͻ("DeuteriumProcessor",ϳ.Ϯ,"Deuterium Refinery"),new Ͻ(),};
public ö(IMyRefinery Е){ʀ=Е;Ϻ=Е.GetInventory(0);ϻ=Е.GetInventory(1);Ё=Е.BlockDefinition.SubtypeId;Ͼ=Д.Find(í=>í.Г(Ё));Ё=Ͼ.Џ();
Ѐ=ɗ.FindAll(Ǟ=>ʀ.CanUseBlueprint(Ǟ.ɔ));Ж();}void Ж(){var З=new List<MyItemType>();Ϻ.GetAcceptedItems(З,l=>l.SubtypeId.
ToLower().Contains("scrap")&&!ί.И(l));foreach(var К in З){var Л=ί.Й(К);if(!Ѐ.Contains(Л))Ѐ.Add(Л);}}ί Н(MyItemType æ){return М(
ǔ(æ));}ί М(string О){return Ѐ.Find(Ǟ=>Ǟ.Σ==О);}public void ʇ(){if(L())return;if(F.O()){switch(Ͼ.Ў()){case ϳ.ϯ:if(é.
ContainsKey(ɬ.ʏ)&&!é[ɬ.ʏ].ć())П();else if(Ĳ&&ľ.ContainsKey(ɬ.ʐ)&&ľ[ɬ.ʐ]>0)П(true);else Р();break;case ϳ.ϰ:if(é.ContainsKey(ɬ.ɭ)&&!é
[ɬ.ɭ].ć())С();else Р();break;case ϳ.ϱ:if(é.ContainsKey(ɮ)&&!é[ɮ].ć())Т();else Р();break;case ϳ.Ϯ:У();break;}}}bool Х(
string[]Ф){foreach(var Æ in Ф)if(!(ϼ.ContainsKey(Æ)&&ϼ[Æ]!=0))return false;return true;}void Щ(string[]Ц,float[]Ч,float Ш){for
(int l=0;l<Ц.Length;l++){Ǘ(Ц[l],Ч[l]*Ш,Ϻ);}}public const string ɮ="Ingot "+ø,ø="SpentFuelReprocessing";static string[]Ы=
new string[]{ɬ.Ъ,ʊ.ʎ,ɬ.ϛ,ɬ.ϗ};static float[]Ь=new float[]{1f,0.75f,0.2f,0.3f,};void Т(){if(Х(Ы)&&Ђ<90)return;k(Ϻ);var Ĉ=((Ϻ
.MaxVolume.RawValue/1000)/56.9f)*50.5f;Щ(Ы,Ь,Ĉ);}static string[]Э=new string[]{ɬ.ʋ,ɬ.ʏ,ɬ.ʶ};static float[]Ю=new float[]{
16.428f,5f,15f};public void С(){if(Х(Э)&&Ђ<90)return;k(Ϻ);Щ(Э,Ю,((Ϻ.MaxVolume.RawValue/1000)/56.9f)*0.5f);}public void П(bool Я
=false){if(Ђ<10)Р();if(Я||(ľ.ContainsKey(ɬ.ʐ)&&ľ[ɬ.ʐ]>0)){Ǘ(ɬ.ʐ,1000,Ϻ,0);if(Я)return;}if(ľ.ContainsKey(ɬ.ʵ)&&ľ[ɬ.ʵ]>0){Ǘ
(ɬ.ʵ,1000,Ϻ,(!ϼ.ContainsKey(ɬ.ʵ)||ϼ[ɬ.ʵ]==0?0:1));}if(Ђ>70){Ǘ(ʊ.ʎ,1000,Ϻ);}}void б(){List<MyInventoryItem>а=new List<
MyInventoryItem>();Ϻ.GetItems(а);if(а.Count>0){var О=ǔ(а[0].Type);var N=Ѐ.Find(Ǟ=>Ǟ.Σ==О);if(N!=null)N.ʃ++;}}public void x(){if(L())
return;ɱ(ϼ);ʦ(Ϻ,ϼ);б();ʦ(ϻ);k(ϻ);в(Ϲ.ϵ,ϻ.CurrentVolume>0);if(!F.o(ʀ.CustomName,true))return;if(Ͼ.Ў()==ϳ.ϲ){if(Ͼ.Ў()==ϳ.ϲ)г(Ϲ.ϸ
);в(Ϲ.ϵ,ϻ.CurrentVolume>0);в(Ϲ.Ϸ,!ʀ.IsFunctional);return;}if(Ͼ.Б()){ʚ(ʖ.ʗ.д,F.f.ToString());return;}if(Ͼ.А()){if(!ʁ.
Contains("@"+Ё))ʁ+="@"+Ё;if(!ɐ.ContainsKey(Ё))ɐ.Add(Ё,new List<Ξ>());if(!ɋ.ContainsKey(Ё))ɋ.Add(Ё,Ѐ);}е();в(Ϲ.Ϸ,!ʀ.IsFunctional)
;if(ʀ.IsFunctional){ʂ++;ʀ.UseConveyorSystem=false;if(Ϻ.ItemCount==0){ʀ.Enabled=(ĳ&&!F.q("Nooff"))?false:true;ж(Ϲ.ϴ);}else
{ʀ.Enabled=true;if(Ϻ.CurrentVolume>0)ж(Ϲ.ϴ);}Ђ=100-(int)((Ϻ.CurrentVolume.RawValue*100)/Ϻ.MaxVolume.RawValue);}}void г(Ϲ
з){if(!Ͽ.Contains(з))Ͽ.Add(з);}void ж(Ϲ з){if(Ͽ.Contains(з))Ͽ.Remove(з);}void в(Ϲ з,bool ʔ){if(ʔ)г(з);else ж(з);}public
void ɩ(){k(Ϻ);k(ϻ);}public bool L(){return ʀ.Closed;}public void Р(){if(F.O())k(Ϻ);}static Dictionary<Ϲ,string>и=new
Dictionary<Ϲ,string>{{Ϲ.ϴ," could not be filled\n"},{Ϲ.ϵ," cannot unload outputitems.\n"},{Ϲ.Ϸ," is damaged.\n"},{Ϲ.ϸ,
" cannot filled by PIM.\n"},};public void g(c d){if(!F.O()&&Ͽ.Count==0)return;foreach(var з in Ͽ){d.e(F.f);d.e(и.GetValueOrDefault(з,
": unknown error\n"));}}void л(string й){if(é.ContainsKey(й)){var Ǟ=é[й];var к=Ǟ.U-Ǟ.V;if(Ǟ.U>0&&к>0)Ǟ.ß=к;else Ǟ.ß=0;}}void е(){string м=
"----";string н="----";float о=0f;float п=0f;switch(Ͼ.Ў()){case ϳ.ϰ:л(ɬ.ɭ);break;case ϳ.ϯ:л(ɬ.ʏ);break;case ϳ.ϱ:л(ɮ);break;}
var а=new List<MyInventoryItem>();Ϻ.GetItems(а);if(а.Count()>0){м=ǔ(а[0].Type);о=(float)а[0].Amount;if(а.Count()>1){н=ǔ(а[1
].Type);п=(float)а[1].Amount;}}Ѓ=Ѐ.Find(Ǟ=>Ǟ.Σ==м);Ѕ=о;Є=Ѐ.Find(Ǟ=>Ǟ.Σ==н);І=п;}void ц(){bool р=false;ί с=null;List<Ξ>ɒ=ɐ
[Ё];for(int l=0;l<ɒ.Count;l++){Ξ Ƥ=ɒ[l];if(!т(Ƥ.ɓ)||Ƥ.ƭ==0||!ľ.ContainsKey(Ƥ.ɓ.Σ))continue;с=Ƥ.ɓ;if(Ђ<50)Р();var у=с.Σ.
Split(' ');if(ľ.ContainsKey(с.Σ))р=ˠ("MyObjectBuilder_"+у[0],у[1],ľ[с.Σ],ʀ.GetInventory(0));if(!р)р=ф(с);в(Ϲ.ϴ,!р&&Ϻ.
CurrentVolume==0);}if(р)х(ɒ,с,ʂ);}bool ф(ί ε){var ч=0f;if(ε==Ѓ)ч=Ѕ;else if(ε==Є)ч=І;if(Ϻ.CurrentVolume.RawValue<100){foreach(ö Е in ɨ
){if(Е.L())continue;int ш=0;var щ=new List<MyInventoryItem>();Е.Ϻ.GetItems(щ);foreach(var К in щ){var ъ=Е.Н(К.Type);if(ε
==ъ&&(float)К.Amount>100&&(float)К.Amount>ч&&т(ъ)){var Ċ=MyFixedPoint.MultiplySafe(К.Amount,(ш==0?0.5f:1f));var ы=Е.Ϻ.
TransferItemTo(Ϻ,ш,null,true,Ċ);if(ы)return true;}ш++;}}}return false;}void ё(){ь=э.ю;var я=new List<MyInventoryItem>();Ϻ.GetItems(я);
if(я.Count==0)return;for(int l=я.Count-1;l>=0;l--){var К=я[l];var ˡ=ǔ(К.Type);var ɍ=Ѐ.Find(Ǟ=>Ǟ.Σ==ˡ);if(ɍ==null)continue;
var ѐ=Ξ.ζ(ɐ[Ё],ɍ);if(ѐ!=null&&ѐ.ʳ==0)Ǖ(Ϻ,ˡ,К);}}void љ(){ь=э.ђ;var я=new List<MyInventoryItem>();var ѓ=ʀ.GetInventory(0);ѓ.
GetItems(я);if(я.Count>1){var є=-1;for(int l=0;l<я.Count;l++)if(я[l].Type.SubtypeId.ToLower().Contains("scrap")){є=l;break;}if(є
==-1){int ѕ=0,і=0;var ї=ǔ(я[0].Type);var ј=ǔ(я[1].Type);foreach(Ξ Ƥ in ɐ[Ё]){if(ї==Ƥ.ɓ.Σ)ѕ=Ƥ.ʳ;else if(ј==Ƥ.ɓ.Σ)і=Ƥ.ʳ;}if(
ѕ<і)ѓ.TransferItemTo(ѓ,0,1,true,я[0].Amount);if(я.Count==3)Ǖ(ѓ,ǔ(я[2].Type),я[2]);}else ѓ.TransferItemTo(ѓ,є,0,true,я[є].
Amount);}}public bool т(ί О){if(ʀ.GetInventory(0).IsFull)return false;return Ѐ.Contains(О);}enum э{ђ,ю,};э ь=э.ђ;void У(){if(!
ɐ.ContainsKey(Ё))return;if(Ђ>80||њ())ц();if(ь==э.ђ)ё();else if(ь==э.ю)љ();}public bool њ(){int ћ=0;int ќ=0;foreach(Ξ Ƥ in
ɐ[Ё]){if(Ƥ.ɓ==Ѓ||Ƥ.ɓ==Є)ќ=ќ<Ƥ.ƭ?(int)(Ƥ.ƭ*1.5):ќ;else if(ћ==0&&т(Ƥ.ɓ))ћ=Ƥ.ƭ;}return ќ<ћ;}static void х(List<Ξ>ѝ,ί ê,int ў
){Ξ Ƥ=null;if(ѝ.Count>0&&null!=(Ƥ=Ξ.ζ(ѝ,ê))){if(Ƥ.ƭ<100){Ƥ.ƭ-=Ƥ.ƭ/ў;if(Ƥ.ƭ<0)Ƥ.ƭ=1;ѝ.Sort();}}}}void Φ(string џ,ί æ,int ƭ
){if(ƭ==0)ƭ=1;if(ɐ.ContainsKey(џ)){Ξ.ζ(ɐ[џ],æ,true).Π(ƭ);}}static Dictionary<string,List<Ξ>>ɐ=new Dictionary<string,List<
Ξ>>();public class Ξ:IComparable<Ξ>{public ί ɓ;public int ƭ=0;public int ʳ=0;public int CompareTo(Ξ M){if(M.ʳ==ʳ)return 0
;return M.ʳ>ʳ?1:-1;}public Ξ(ί N,int Ѡ=0){ɓ=N;ƭ=Ѡ;ʳ=Ѡ;}public void Π(int ѡ){if(ѡ>10000)ѡ=10000;ƭ=ѡ;if(ѡ==0||ʳ!=ѡ)ʳ=ѡ;}
public static Ξ ζ(List<Ξ>Ѣ,ί ê,bool ѣ=false){foreach(Ξ ɒ in Ѣ){if(ɒ.ɓ==ê)return ɒ;}if(ѣ){Ξ ѡ=new Ξ(ê);Ѣ.Add(ѡ);return ѡ;}else
return null;}}static List<ί>ɗ=new List<ί>();static Dictionary<string,Dictionary<ί,int>>θ=new Dictionary<string,Dictionary<ί,
int>>();static void ѧ(string Ѥ,string ѥ,string Ѧ){MyDefinitionId Á;if(!MyDefinitionId.TryParse(
"MyObjectBuilder_BlueprintDefinition/"+Ѥ,out Á))return;if(ɗ.Find(Ǟ=>Ǟ.ɔ==Á)==null){ɗ.Add(new ί(Á,ѥ,Ѧ));}}static void ѩ(string Ѩ){ѧ(Ѩ+"OreToIngot","Ore "+Ѩ,
"Ingot "+Ѩ);}static void ѯ(string[]Ѫ,string ѫ,string Ѭ,string ѭ,string Ѯ){foreach(var ê in Ѫ){ѧ(ѫ+ê+Ѭ,ѭ+ê,Ѯ+ê);}}void Ɉ(){ί.Ѱ();
ѧ("StoneOreToIngotBasic",ʊ.ʋ,ɬ.ʋ);ѧ("ScrapToIronIngot",ʊ.ѱ,ɬ.Ѳ);ѧ("ScrapIngotToIronIngot",ɬ.ѱ,ɬ.Ѳ);ѩ("Gold");ѩ("Platinum"
);ѩ("Stone");ѩ("Silver");ѩ("Iron");ѩ("Nickel");ѩ("Cobalt");ѩ("Silicon");ѩ("Uranium");if(À[Ď]){ѧ("TungstenToIngot",
"Ore Tungsten","Ingot TungstenIngot");ѧ("CopperToIngot","Ore Copper","Ingot CopperIngot");ѧ("LeadToIngot","Ore Lead","Ingot LeadIngot"
);ѧ("TitaniumToIngot","Ore Titanium","Ingot TitaniumIngot");ѧ("GraphiteOreToIngot","Ore Graphite","Ingot Carbon");ѧ(
"LithiumToIngot","Ore Lithium","Ingot LithiumIngot");ѧ("TungstenToIngot","Ore Tungsten","Ingot TungstenIngot");ѧ("CopperToIngot",
"Ore Copper","Ingot CopperIngot");ѧ("LeadToIngot","Ore Lead","Ingot LeadIngot");ѧ("GraphiteOreToIngot","Ore Graphite","Ingot Carbon"
);}if(À[Ĕ]){ѧ("StonetoDeuterium",ʊ.ʋ,ɬ.ʑ);ѧ("IcetoDeuterium",ʊ.ʎ,ɬ.ʑ);ѧ("DeuteriumOreToIngot",ʊ.ѳ,ɬ.ʑ);}if(À[ď]){ѩ(
"Carbon");ѩ("Potassium");ѩ("Phosphorus");}if(À[đ]){ѩ("Naquadah");ѩ("Trinium");ѩ("Neutronium");}if(!À[č]){ѩ("Magnesium");}else{
string[]Ѵ={"Iron","Nickel","Cobalt","Silicon","Silver","Gold","Platinum","Uranium","Copper","Lithium","Bauxite","Titanium",
"Tantalum","Sulfur",};ѩ("Copper");ѧ("BauxiteOreToIngot",ʊ.ϕ,ɬ.ѵ);ѩ("Titanium");ѩ("Tantalum");ѧ("CoalToCarbonBasic",ʊ.ϓ,ɬ.ϐ);ѯ(Ѵ,
"Crushed","OreToIngot","Ore Crushed","Ingot ");ѯ(Ѵ,"Purified","OreToIngot","Ore Purified","Ingot ");ѯ(Ѵ,"Crush","Ore","Ore ",
"Ore Crushed");ѧ("CrushNiterOre",ʊ.ϗ,ʊ.ύ);ѧ("CrushStoneOre",ʊ.ʋ,ɬ.ʋ);ѯ(Ѵ,"Purify","Ore","Ore Crushed","Ore Purified");ѧ(
"PurifyNiterOre",ʊ.ύ,"Ore PurifiedNiter");ѩ("Niter");ѩ("Lithium");ѩ("Sulfur");ѧ("CoalToCarbon",ʊ.ϓ,ɬ.ϐ);ѧ("CrushedNiterOreToIngot",ʊ.ύ,ɬ
.ϗ);ѧ("PurifiedNiterOreToIngot","Ore PurifiedNiter",ɬ.ϗ);ѧ("OilSandToCrudeOil","Ore OilSand","Ore CrudeOil");ѧ(
"CrudeOilCracking","Ore CrudeOil","Ingot FuelOil");ѩ("Uranium");}ɗ.Sort((Ĭ,Ȉ)=>Ĭ.ə.CompareTo(Ȉ.ə));}public class ί{static DateTime Ѷ=
DateTime.Now;static string[]ѷ={"Component C100ShellCasing",ʊ.ѱ,ɬ.ѱ};static MyItemType[]Ѹ;static Dictionary<MyItemType,ί>ѹ=new
Dictionary<MyItemType,ί>();public static void χ(){if((DateTime.Now-Ѷ).TotalSeconds>12){Ѷ=DateTime.Now;foreach(var N in ɗ){var Ѻ=N.
ϊ;N.ϊ=ľ.GetValueOrDefault(N.Σ,0);N.ό=ľ.GetValueOrDefault(N.Ψ,0);var Ѽ=N.ѻ;N.ѻ=DateTime.Now;var ѽ=(Ѻ-N.ϊ);N.ϋ=ѽ<0?"...":Ν(
(N.ϊ/ѽ)*(N.ѻ-Ѽ).TotalHours);}}}public static bool И(MyItemType Ѿ){return Ѹ.Contains(Ѿ);}public static ί Й(MyItemType Ѿ){
if(!ѹ.ContainsKey(Ѿ)){ѹ.Add(Ѿ,new ί(Ѿ));ɗ.Add(ѹ[Ѿ]);}return ѹ[Ѿ];}public static void Ѱ(){Ѹ=new MyItemType[ѷ.Length];for(
int l=0;l<ѷ.Length;l++){var í=ѷ[l];var ѿ=í.Split(' ');var Ҁ=MyItemType.Parse(Ť+ѿ[0]+"/"+ѿ[1]);Ѹ[l]=Ҁ;}}public
MyDefinitionId ɔ;public string f="";public string Σ="";public string ə="";public string Ψ="";public string ɚ="";public float ϊ=0;
public float ό=0;DateTime ѻ=DateTime.Now;public string ϋ="";public int ʃ=0;public bool Υ=false;public ί(MyDefinitionId ҁ,
string Ҋ,string ҋ){ɔ=ҁ;f=ҁ.SubtypeName;Σ=Ҋ;ə=Ϡ(Σ);Ψ=ҋ;ɚ=Ϡ(Ψ);Υ=ѷ.Contains(Σ);}public ί(MyItemType Ҍ){Σ=ǔ(Ҍ);ə=Ҍ.SubtypeId;f=ə+
"ToIngots";Υ=true;}}public class ʌ{public const string ҍ="powder",Ҏ="Magnesium",ʍ="Stone",ҏ="Iron",Ґ="Nickel",ґ="Silicon",Ғ=
"Cobalt",ғ="Platinum",Ҕ="Uranium",ҕ="Scrap",ϑ="Carbon",ϝ="Potassium",Җ="Phosphorus",җ="Naquadah",Ҙ="Trinium",ҙ="Neutronium",Қ=
"Copper",Ϛ="Lithium",ϖ="Bauxite",қ="Titanium",Ҝ="Tantalum",Ϝ="Sulfur",Ϙ="Niter",ϔ="Coal",Ϗ="Deuterium";}public class ɬ:ʌ{const
string ҝ="Ingot ";public const string ѱ=ҝ+ҕ,ύ=ҝ+Ҏ,ώ=Ҏ+ҍ,Ϟ="Gun"+ҍ,ʋ=ҝ+ʍ,Ѳ=ҝ+ҏ,Ҟ=ҝ+Ґ,ҟ=ҝ+ґ,Ҡ=ҝ+Ғ,ҡ=ҝ+ғ,Ң=ҝ+Ҕ,ʏ=ҝ+"WaterFood",ʶ=
ҝ+"Nutrients",ɭ=ҝ+"SubFresh",ʐ=ҝ+"GreyWater",ʵ=ҝ+"CleanWater",Ъ=ҝ+"SpentFuel",ϗ=ҝ+Ϙ,ʑ=ҝ+Ϗ+"Container",ϐ=ҝ+ϑ,ң=ҝ+ϝ,Ҥ=ҝ+Җ,ҥ
=ҝ+җ,Ҧ=ҝ+Ҙ,ҧ=ҝ+ҙ,Ҩ=ҝ+Қ,ϙ=ҝ+Ϛ,ҩ=ҝ+қ,Ҫ=ҝ+Ҝ,ϛ=ҝ+Ϝ,ѵ=ҝ+"Aluminium";}public class ʊ:ʌ{const string ҝ="Ore ";public const
string ѱ=ҝ+ҕ,ύ=ҝ+Ҏ,ʋ=ҝ+ʍ,Ѳ=ҝ+ҏ,Ҟ=ҝ+Ґ,ҟ=ҝ+ґ,Ҡ=ҝ+Ғ,ҡ=ҝ+ғ,Ң=ҝ+Ҕ,ʒ=ҝ+"Organic",ʎ=ҝ+"Ice",ѳ=ҝ+Ϗ,ϐ=ҝ+ϑ,ң=ҝ+ϝ,Ҥ=ҝ+Җ,ҥ=ҝ+җ,Ҧ=ҝ+Ҙ,ҧ=ҝ+ҙ
,ϗ=ҝ+Ϙ,Ҩ=ҝ+Қ,ϙ=ҝ+Ϛ,ϕ=ҝ+ϖ,ҩ=ҝ+қ,Ҫ=ҝ+Ҝ,ϛ=ҝ+Ϝ,ϓ=ҝ+ϔ;}static List<ҫ>Ƀ=new List<ҫ>();abstract class ҫ{public const string Ҭ=
"[Color=#FFFF0000]",ҭ="[Color=#FF000000]",Ү="[/Color]";public enum Ҳ{ү,Ұ,ұ}DateTime ҳ=DateTime.Now;public Ҳ æ=Ҳ.ү;int Ҵ=-1;public c ҵ=new c
(1000);public ҫ(){æ=Ҳ.Ұ;}public ҫ(Ҳ Ǿ){æ=Ǿ;}public ҫ(string Ҷ,Ҳ Ǿ,int Θ=-1){ҵ.Ϫ(Ҷ);æ=Ǿ;Ҵ=Θ;}public void ҷ(int Θ){Ҵ=Θ;}
public virtual c Ҹ(){return ҵ;}public bool ʉ(){if(Ҵ==-1)return false;if((DateTime.Now-ҳ).TotalSeconds>Ҵ)return true;return
false;}public void ҹ(){Ҵ=0;}}class Һ:ҫ{public Һ(string Ů,int Θ=-1):base(Ů,Ҳ.Ұ,Θ){}}class ʖ:ҫ{public enum ʗ{ү,ʛ,ʝ,ˈ,ʘ,д}public
ʗ һ=ʗ.ү;public string Ҽ="";public ʖ(ʗ ҽ,string Ҿ=""):base(Ҳ.ұ){һ=ҽ;Ҽ=Ҿ;ҿ(Ҿ);}public override c Ҹ(){if(ʉ())return null;
return ҵ;}void ҿ(string Ҽ){switch(һ){case ʗ.д:ҵ.Ϫ("! refinery '",Ҽ,"' not supported.");break;case ʗ.ʘ:ҵ.Ϫ("   - ",Ҽ,
" found. you can define cargo for it with ...(sms,",Ҽ.ToLower(),")");break;case ʗ.ˈ:ҵ.Ϫ("!! please define cargo for ",Ҽ,". name container like ...(sms,",Ҽ.ToLower(),")");
break;case ʗ.ʝ:ҵ.Ϫ("!!!! cargo with ",Ҽ," is full !!!!!");break;case ʗ.ʛ:ҵ.Ϫ("!!! cargo with ",Ҽ," is heavy.");break;}}}class
Ʉ:ҫ{public override c Ҹ(){if(ŉ.Count==0)return null;ҵ.Ϫ("AmmonitionManager: ",ŉ.Count.ToString()," weapons.");return ҵ;}}
class Ʌ:ҫ{public override c Ҹ(){if(ŋ.Count==0)return null;ҵ.Ϫ("StorageManager: ",ŋ.Count.ToString()," containers.");return ҵ;
}}class Ɇ:ҫ{public override c Ҹ(){ҵ.Ħ();foreach(var Е in ɨ)Е.g(ҵ);if(ҵ.Ӏ())return null;ҵ.Ӂ(0,
"--------------- RefineryManager ---------------\n");return ҵ;}}class ɇ:ҫ{public override c Ҹ(){ҵ.Ħ();foreach(var ӂ in Ń)ӂ.g(ҵ);if(ҵ.Ӏ())return null;ҵ.Ӂ(0,
"--------------- AssemblerManager ----------------\n");return ҵ;}}static void ɪ(string Ӄ,int ӄ=10){Ƀ.Add(new Һ(Ӄ,ӄ));}static void ʕ(bool ʔ,ʖ.ʗ Ӆ,string ç=""){if(ʔ)ʚ(Ӆ,ç);
else ʜ(Ӆ,ç);}static void ʚ(ʖ.ʗ Ӆ,string ç=""){if(ӆ(Ӆ,ç)==null)Ƀ.Add(new ʖ(Ӆ,ç));}static ʖ ӆ(ʖ.ʗ Ӆ,string ç){foreach(var ǒ in
Ƀ){if(!(ǒ is ʖ))continue;var Ӈ=ǒ as ʖ;if((Ӈ.һ==Ӆ)&&Ӈ.Ҽ==ç)return Ӈ;}return null;}static void ʜ(ʖ.ʗ Ӆ,string ç){var Ӈ=ӆ(Ӆ,
ç);if(Ӈ!=null)Ӈ.ҹ();}c ɀ=new c(2000);c ӈ=new c(1000);void ʞ(){ɀ.Ϫ("\n");ӈ.Ħ();foreach(var ǒ in Ƀ){switch(ǒ.æ){case ҫ.Ҳ.Ұ:
ɀ.Ӊ(ǒ.Ҹ());break;case ҫ.Ҳ.ұ:ӈ.ӊ(ǒ.Ҹ());break;}}if(!ӈ.Ӏ()){ɀ.e("\n--------------------- Hints ---------------------\n");ɀ.
e(ӈ);}}public class c{readonly StringBuilder Ӌ;public c(int ӌ){Ӌ=new StringBuilder(ӌ);}public StringBuilder Ӎ(){return Ӌ;
}public string ӎ(){return Ӌ.ToString();}public override string ToString(){return Ӌ.ToString();}public void ĩ(){int ӏ=0;
int Ӑ=0;for(int l=Ӌ.Length-1;l>=0;l--){if(char.IsWhiteSpace(Ӌ[l]))Ӑ++;else break;}for(int l=0;l<Ӌ.Length;l++){if(char.
IsWhiteSpace(Ӌ[l]))ӏ++;else break;}Ӌ.Remove(0,ӏ);Ӌ.Remove(Ӌ.Length-Ӑ,Ӑ);}public void Ĩ(c ӑ,int Ӓ,int ӓ=-1){Ĩ(ӑ.Ӌ,Ӓ,ӓ);}public void Ĩ
(StringBuilder Ӕ,int Ӓ,int ӓ=-1){Ӕ.Clear();if(Ӓ>Ӌ.Length)return;int ӕ;if(ӓ>0)ӕ=Ӓ+ӓ;else ӕ=Ӌ.Length-1;for(int l=Ӓ;l<=ӕ;l++
)Ӕ.Append(Ӌ[l]);}public bool Ӏ(){return Ӌ.Length==0;}public bool ĥ(string Ů){if(Ӌ.Length==Ů.Length&&Ӗ(Ů))return true;
return false;}public void Ӂ(int ӗ,string Ů){Ӌ.Insert(ӗ,Ů);}public void e(c Ә){if(Ә==null)return;Ӌ.Append(Ә.Ӌ);}public void e(
string Ů){Ӌ.Append(Ů);}public void e(char Ɯ){Ӌ.Append(Ɯ);}public void ӊ(string Ů){e(Ů+"\n");}public void ӊ(c Ә){if(Ә==null)
return;e(Ә);e('\n');}public void Ӊ(string Ů){if(Ů.Length>0)ӊ(Ů);}public void Ӊ(c Ә){if(Ә!=null&&!Ә.Ӏ())ӊ(Ә);}public void Ħ(){Ӌ
.Clear();}public void Ϫ(string ә,string Ӛ="",string ӛ="",string Ӝ="",string ӝ=""){Ħ();e(ә);if(Ӛ!="")e(Ӛ);if(ӛ!="")e(ӛ);if
(Ӝ!="")e(Ӝ);if(ӝ!="")e(ӝ);}public void Ϫ(c Ә){Ħ();e(Ә);}public void Ϫ(char Ɯ){Ħ();e(Ɯ);}public void Ӟ(){for(int l=0;l<Ӌ.
Length;l++)Ӌ[l]=char.ToLower(Ӌ[l]);}public bool Ӗ(string Æ){return ӟ(Æ)!=-1;}public int ӟ(string Æ){int Ӡ=0;int ӡ=0;int Ӣ=-1;
for(int l=0;Ӡ<Æ.Length&&l<Ӌ.Length&&l+(Æ.Length-Ӡ)<Ӌ.Length;l++,Ӡ+=ӡ){if(Ӌ[l]==Æ[Ӡ]){if(ӡ==0)Ӣ=l;ӡ=1;}else{ӡ=0;Ӡ=0;Ӣ=-1;}}
if(Ӡ<Æ.Length-1)Ӣ=-1;return Ӣ;}}
