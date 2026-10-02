const string TAG_ORE = "[Ore]";
const string TAG_INGOT = "[Ingot]";
const string TAG_COMPONENT = "[Component]";
const string TAG_AMMO = "[Ammo]";
const string TAG_TOOL = "[Tool]";
const string TAG_BOTTLE = "[Bottle]";
const string TAG_FOOD = "[Food]";
const string TAG_OVERFLOW = "[Overflow]";
const string LCD_TAG_DEF = "[INV:TagDef]";
const string LCD_CRAFT = "[INV:Craft]";
const string LCD_STATUS = "[INV:Status]";
const string LCD_LOADOUT = "[INV:Loadout:";
const string LCD_POWER = "[INV:Power]";
const string LCD_LOG = "[INV:Log]";
const string LCD_WARN = "[INV:Warnings]";
const string LCD_SMELT = "[INV:Smelt]";
const string LCD_CRAFT_DBG = "[CraftDbg]";
const string TAG_NO_SORT = "[NoSort]";
const string TAG_NO_MANAGE = "[NoManage]";
const string TAG_MANUAL = "[Manual]";
const string TAG_LEARN = "[Learn]";
const string TAG_FUEL = "[Fuel]";
const string IGC_CHANNEL = "INV_DATA";
string[] LOCKED_KEYWORDS = { "[locked]", "control station", "control seat", "safe zone" };
string[] HIDDEN_KEYWORDS = { "[Hidden]" };
bool enableReactorHandling = true;
bool enableGeneratorHandling = true;
bool enableRefineryHandling = true;
bool enableOreCycling = true;
bool enableAutoCrafting = true;
bool enableAutoDisassembly = true;
bool enablePowerManagement = true;
bool enableShipFueling = true;
bool enableBottleFilling = true;
bool enableLoadouts = true;
double uraniumPerLargeReactor = 2500;
double uraniumPerSmallReactor = 1000;
double icePerLargeGenerator = 100000;
double icePerSmallGenerator = 20000;
double icePerIrrigation = 5000;
bool enableIrrigationHandling = true;
double orePerRefinery = 50000;
double maxRuntimeMs = 0.4;
bool enableAutoThrottle = true;
double tickBudgetMs = 0.25;
bool showFillLevel = true;
const string OB = "MyObjectBuilder_";
const string T_ORE = "Ore";
const string T_INGOT = "Ingot";
const string T_COMP = "Component";
const string T_AMMO = "AmmoMagazine";
const string T_OXYB = "OxygenContainerObject";
const string T_GASB = "GasContainerObject";
const string T_GUN = "PhysicalGunObject";
const string T_PHYS = "PhysicalObject";
const string T_FOOD = "ConsumableItem";
const string T_DATA = "Datapad";
const string BP_PREFIX = OB + "BlueprintDefinition/";
static readonly Dictionary<string, string> BUILTIN_CATEGORIES = new Dictionary<string, string> {
{ OB + T_ORE, TAG_ORE },
{ OB + T_INGOT, TAG_INGOT },
{ OB + T_COMP, TAG_COMPONENT },
{ OB + T_AMMO, TAG_AMMO },
{ OB + T_OXYB, TAG_BOTTLE },
{ OB + T_GASB, TAG_BOTTLE },
{ OB + T_GUN, TAG_TOOL },
{ OB + T_FOOD, TAG_FOOD },
{ OB + T_DATA, TAG_TOOL },
};
static readonly Dictionary<string, string> KNOWN_BLUEPRINTS = new Dictionary<string, string> {
{"BulletproofGlass","BulletproofGlass"},{"Canvas","Position0030_Canvas"},
{"Computer","ComputerComponent"},{"Construction","ConstructionComponent"},
{"Detector","DetectorComponent"},{"Display","Display"},{"Explosives","ExplosivesComponent"},
{"Girder","GirderComponent"},{"GravityGenerator","GravityGeneratorComponent"},
{"InteriorPlate","InteriorPlate"},{"LargeTube","LargeTube"},{"Medical","MedicalComponent"},
{"MetalGrid","MetalGrid"},{"Motor","MotorComponent"},{"PowerCell","PowerCell"},
{"RadioCommunication","RadioCommunicationComponent"},{"Reactor","ReactorComponent"},
{"SmallTube","SmallTube"},{"SolarCell","SolarCell"},{"SteelPlate","SteelPlate"},
{"Superconductor","Superconductor"},{"Thrust","ThrustComponent"},
{"Medkit","Position0021_Medkit"},{"Powerkit","Position0022_Powerkit"},
{"NATO_25x184mm","Position0080_NATO_25x184mmMagazine"},
{"Missile200mm","Position0100_Missile200mm"},{"AutocannonClip","Position0090_AutocannonClip"},
{"LargeCalibreAmmo","Position0120_LargeCalibreAmmo"},{"MediumCalibreAmmo","Position0110_MediumCalibreAmmo"},
{"LargeRailgunAmmo","Position0140_LargeRailgunAmmo"},{"SmallRailgunAmmo","Position0130_SmallRailgunAmmo"},
{"SemiAutoPistolMagazine","Position0010_SemiAutoPistolMagazine"},
{"ElitePistolMagazine","Position0030_ElitePistolMagazine"},
{"FullAutoPistolMagazine","Position0020_FullAutoPistolMagazine"},
{"AutomaticRifleGun_Mag_20rd","Position0040_AutomaticRifleGun_Mag_20rd"},
{"RapidFireAutomaticRifleGun_Mag_50rd","Position0050_RapidFireAutomaticRifleGun_Mag_50rd"},
{"PreciseAutomaticRifleGun_Mag_5rd","Position0060_PreciseAutomaticRifleGun_Mag_5rd"},
{"UltimateAutomaticRifleGun_Mag_30rd","Position0070_UltimateAutomaticRifleGun_Mag_30rd"},
{"OxygenBottle","Position0010_OxygenBottle"},{"HydrogenBottle","Position0020_HydrogenBottle"},
{"AngleGrinderItem","Position0010_AngleGrinder"},{"AngleGrinder2Item","Position0020_AngleGrinder2"},
{"AngleGrinder3Item","Position0030_AngleGrinder3"},{"AngleGrinder4Item","Position0040_AngleGrinder4"},
{"HandDrillItem","Position0050_HandDrill"},{"HandDrill2Item","Position0060_HandDrill2"},
{"HandDrill3Item","Position0070_HandDrill3"},{"HandDrill4Item","Position0080_HandDrill4"},
{"WelderItem","Position0090_Welder"},{"Welder2Item","Position0100_Welder2"},
{"Welder3Item","Position0110_Welder3"},{"Welder4Item","Position0120_Welder4"},
{"AutomaticRifleItem","Position0040_AutomaticRifle"},
{"RapidFireAutomaticRifleItem","Position0050_RapidFireAutomaticRifle"},
{"PreciseAutomaticRifleItem","Position0060_PreciseAutomaticRifle"},
{"UltimateAutomaticRifleItem","Position0070_UltimateAutomaticRifle"},
{"SemiAutoPistolItem","Position0010_SemiAutoPistol"},{"FullAutoPistolItem","Position0020_FullAutoPistol"},
{"ElitePistolItem","Position0030_EliteAutoPistol"},
{"BasicHandHeldLauncherItem","Position0080_BasicHandHeldLauncher"},
{"AdvancedHandHeldLauncherItem","Position0090_AdvancedHandHeldLauncher"},
};
static readonly Dictionary<string, string> DISPLAY_NAMES = new Dictionary<string, string> {
{"Construction","Construction Comp."},{"MetalGrid","Metal Grid"},{"InteriorPlate","Interior Plate"},
{"SteelPlate","Steel Plate"},{"SmallTube","Small Tube"},{"LargeTube","Large Tube"},
{"BulletproofGlass","Bulletproof Glass"},{"Reactor","Reactor Comp."},{"Thrust","Thruster Comp."},
{"GravityGenerator","Gravity Gen. Comp."},{"Medical","Medical Comp."},
{"RadioCommunication","Radio-comm Comp."},{"Detector","Detector Comp."},
{"SolarCell","Solar Cell"},{"PowerCell","Power Cell"},{"ZoneChip","Zone Chip"},
{"NATO_25x184mm","Gatling Ammo"},{"Missile200mm","Rockets"},{"AutocannonClip","Autocannon Mag"},
{"MediumCalibreAmmo","Assault Cannon Shell"},{"LargeCalibreAmmo","Artillery Shell"},
{"SmallRailgunAmmo","Small Railgun Sabot"},{"LargeRailgunAmmo","Large Railgun Sabot"},
{"SemiAutoPistolMagazine","S-10 Mag"},{"ElitePistolMagazine","S-10E Mag"},
{"FullAutoPistolMagazine","S-20A Mag"},{"AutomaticRifleGun_Mag_20rd","MR-20 Mag"},
{"RapidFireAutomaticRifleGun_Mag_50rd","MR-50A Mag"},{"PreciseAutomaticRifleGun_Mag_5rd","MR-8P Mag"},
{"UltimateAutomaticRifleGun_Mag_30rd","MR-30E Mag"},
{"AngleGrinderItem","Angle Grinder"},{"AngleGrinder2Item","Enh. Grinder"},
{"AngleGrinder3Item","Prof. Grinder"},{"AngleGrinder4Item","Elite Grinder"},
{"HandDrillItem","Hand Drill"},{"HandDrill2Item","Enh. Drill"},
{"HandDrill3Item","Prof. Drill"},{"HandDrill4Item","Elite Drill"},
{"WelderItem","Welder"},{"Welder2Item","Enh. Welder"},
{"Welder3Item","Prof. Welder"},{"Welder4Item","Elite Welder"},
{"AutomaticRifleItem","MR-20 Rifle"},{"RapidFireAutomaticRifleItem","MR-50A Rifle"},
{"PreciseAutomaticRifleItem","MR-8P Rifle"},{"UltimateAutomaticRifleItem","MR-30E Rifle"},
{"SemiAutoPistolItem","S-10 Pistol"},{"FullAutoPistolItem","S-20A Pistol"},
{"ElitePistolItem","S-10E Pistol"},{"BasicHandHeldLauncherItem","RO-1 Launcher"},
{"AdvancedHandHeldLauncherItem","PRO-1 Launcher"},
{"OxygenBottle","Oxygen Bottle"},{"HydrogenBottle","Hydrogen Bottle"},
{"Scrap","Scrap Metal"},
};
IEnumerator<bool> mainRoutine;
int throttleSkip = 0;
DateTime tickStart;
List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> inventoryBlocks = new List<IMyTerminalBlock>();
Dictionary<string, List<IMyTerminalBlock>> taggedContainers = new Dictionary<string, List<IMyTerminalBlock>>();
List<IMyTerminalBlock> overflowContainers = new List<IMyTerminalBlock>();
List<IMyAssembler> assemblers = new List<IMyAssembler>();
List<IMyRefinery> refineries = new List<IMyRefinery>();
List<IMyReactor> reactors = new List<IMyReactor>();
List<IMyGasGenerator> generators = new List<IMyGasGenerator>();
List<IMyShipConnector> connectors = new List<IMyShipConnector>();
List<IMyTextPanel> allPanels = new List<IMyTextPanel>();
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyTerminalBlock> hydrogenEngines = new List<IMyTerminalBlock>();
List<IMyAssembler> learningAssemblers = new List<IMyAssembler>();
List<IMyGasTank> gasTanks = new List<IMyGasTank>();
List<IMyTerminalBlock> irrigationBlocks = new List<IMyTerminalBlock>();
bool powerDischargeMode = false;
double powerUpperThreshold = 95;
double powerLowerThreshold = 80;
bool managePowerReactors = true;
bool managePowerHydroEngines = true;
bool gasStockpileMode = false;
double gasUpperThreshold = 95;
double gasLowerThreshold = 80;
bool manageGasGenerators = true;
HashSet<IMyCubeGrid> noSortGrids = new HashSet<IMyCubeGrid>();
HashSet<IMyCubeGrid> noManageGrids = new HashSet<IMyCubeGrid>();
Dictionary<MyItemType, ItemInfo> itemDb = new Dictionary<MyItemType, ItemInfo>();
Dictionary<string, MyDefinitionId> blueprintMap = new Dictionary<string, MyDefinitionId>();
Dictionary<string, Dictionary<MyItemType, double>> customTags = new Dictionary<string, Dictionary<MyItemType, double>>();
Dictionary<string, string> customNames = new Dictionary<string, string>();
Dictionary<string, int> customTagPriorities = new Dictionary<string, int>();
Dictionary<MyItemType, double> craftTargets = new Dictionary<MyItemType, double>();
Dictionary<MyItemType, string> craftSources = new Dictionary<MyItemType, string>();
Dictionary<MyItemType, int> craftPriorities = new Dictionary<MyItemType, int>();
Dictionary<MyItemType, double> stasisTargets = new Dictionary<MyItemType, double>();
Dictionary<MyItemType, double> ingotTargets = new Dictionary<MyItemType, double>();
Dictionary<MyItemType, int> ingotPriorities = new Dictionary<MyItemType, int>();
int oreRotationIndex = 0;
Dictionary<MyItemType, double> globalCounts = new Dictionary<MyItemType, double>();
string craftDbg = "";
Dictionary<MyItemType, double> loadoutCraftNeeds = new Dictionary<MyItemType, double>();
Dictionary<string, Dictionary<MyItemType, double>> loadoutProfiles =
new Dictionary<string, Dictionary<MyItemType, double>>();
Dictionary<string, double> profileUraniumOverride = new Dictionary<string, double>();
Dictionary<string, double> profileIceOverride = new Dictionary<string, double>();
Dictionary<long, int> loadoutPhase = new Dictionary<long, int>();
const int LOAD_UNLOAD = 0, LOAD_STOCK = 1, LOAD_DONE = 2;
Dictionary<long, int> fuelPhase = new Dictionary<long, int>();
const int FUEL_DOCK = 0, FUEL_UNLOAD = 1, FUEL_STOCK = 2, FUEL_WAIT = 3, FUEL_RELEASE = 4, FUEL_DONE = 5;
Dictionary<long, MyDefinitionId> lastLearnBp = new Dictionary<long, MyDefinitionId>();
List<string> actionLog = new List<string>();
List<string> warnings = new List<string>();
List<string> persistentWarnings = new List<string>();
string statusText = "Initializing...";
int stepIndex = 0;
int mainCadenceTick = 0;
int mainCadenceDivisor = 1;
int startupDelayTicks = 180; // first start warmup (~30s at Update10)
Program() {
Runtime.UpdateFrequency = UpdateFrequency.Update10;
LoadBlueprintMap();
PrePopulateItemDb();
mainRoutine = RunCycle();
}
void PrePopulateItemDb() {
string[] comps = { "BulletproofGlass","Canvas","Computer","Construction","Detector","Display",
"Explosives","Girder","GravityGenerator","InteriorPlate","LargeTube","Medical","MetalGrid",
"Motor","PowerCell","RadioCommunication","Reactor","SmallTube","SolarCell","SteelPlate",
"Superconductor","Thrust","ZoneChip" };
foreach (var s in comps) { var t = new MyItemType(OB+T_COMP,s); itemDb[t] = new ItemInfo(t); }
string[] ammo = { "NATO_25x184mm","Missile200mm","AutocannonClip","MediumCalibreAmmo",
"LargeCalibreAmmo","SmallRailgunAmmo","LargeRailgunAmmo","SemiAutoPistolMagazine",
"ElitePistolMagazine","FullAutoPistolMagazine","AutomaticRifleGun_Mag_20rd",
"RapidFireAutomaticRifleGun_Mag_50rd","PreciseAutomaticRifleGun_Mag_5rd",
"UltimateAutomaticRifleGun_Mag_30rd" };
foreach (var s in ammo) { var t = new MyItemType(OB+T_AMMO,s); itemDb[t] = new ItemInfo(t); }
string[] guns = { "AngleGrinderItem","AngleGrinder2Item","AngleGrinder3Item","AngleGrinder4Item",
"HandDrillItem","HandDrill2Item","HandDrill3Item","HandDrill4Item",
"WelderItem","Welder2Item","Welder3Item","Welder4Item",
"AutomaticRifleItem","RapidFireAutomaticRifleItem","PreciseAutomaticRifleItem",
"UltimateAutomaticRifleItem","SemiAutoPistolItem","FullAutoPistolItem","ElitePistolItem",
"BasicHandHeldLauncherItem","AdvancedHandHeldLauncherItem" };
foreach (var s in guns) { var t = new MyItemType(OB+T_GUN,s); itemDb[t] = new ItemInfo(t); }
itemDb[new MyItemType(OB+T_OXYB,"OxygenBottle")] = new ItemInfo(new MyItemType(OB+T_OXYB,"OxygenBottle"));
itemDb[new MyItemType(OB+T_GASB,"HydrogenBottle")] = new ItemInfo(new MyItemType(OB+T_GASB,"HydrogenBottle"));
foreach (var s in new[]{ "Medkit","Powerkit" }) { var t = new MyItemType(OB+T_FOOD,s); itemDb[t] = new ItemInfo(t); }
foreach (var s in new[]{ "Iron","Nickel","Cobalt","Silicon","Silver","Gold","Platinum","Uranium","Magnesium","Stone","Ice","Scrap" }) {
var t = new MyItemType(OB+T_ORE,s); itemDb[t] = new ItemInfo(t);
}
foreach (var s in new[]{ "Iron","Nickel","Cobalt","Silicon","Silver","Gold","Platinum","Uranium","Magnesium","Stone" }) {
var t = new MyItemType(OB+T_INGOT,s); itemDb[t] = new ItemInfo(t);
}
}
void LoadBlueprintMap() {
foreach (var kv in KNOWN_BLUEPRINTS) {
MyDefinitionId bpId;
if (MyDefinitionId.TryParse(BP_PREFIX + kv.Value, out bpId))
blueprintMap[kv.Key] = bpId;
}
if (!string.IsNullOrEmpty(Storage)) {
var lines = Storage.Split('\n');
foreach (var line in lines) {
var parts = line.Split('|');
if (parts.Length == 2) {
MyDefinitionId bpId;
if (MyDefinitionId.TryParse(parts[1], out bpId))
blueprintMap[parts[0]] = bpId;
}
}
}
}
void Save() {
var sb = new System.Text.StringBuilder();
foreach (var kv in blueprintMap)
sb.AppendLine(kv.Key + "|" + kv.Value.ToString());
Storage = sb.ToString().TrimEnd();
}
void Main(string argument, UpdateType updateSource) {
if (!string.IsNullOrEmpty(argument)) {
HandleCommand(argument);
return;
}
if (startupDelayTicks > 0) {
startupDelayTicks--;
return;
}
if (enableAutoThrottle && throttleSkip > 0) {
throttleSkip--;
return;
}
if (++mainCadenceTick % mainCadenceDivisor != 0) return;
tickStart = DateTime.UtcNow;
if (mainRoutine != null) {
int steps = 0;
while (steps < 1) {
bool hasMore = false;
try {
hasMore = mainRoutine.MoveNext();
} catch (Exception e) {
Log("ERROR: " + e.Message);
mainRoutine.Dispose();
mainRoutine = RunCycle();
break;
}
if (!hasMore) {
mainRoutine.Dispose();
mainRoutine = RunCycle();
break;
}
steps++;
}
}
if (enableAutoThrottle && Runtime.LastRunTimeMs > maxRuntimeMs)
throttleSkip = (int)(Runtime.LastRunTimeMs / maxRuntimeMs) + 1;
UpdateEcho();
}
void HandleCommand(string cmd) {
cmd = cmd.Trim().ToLower();
if (cmd == "rescan") {
mainRoutine?.Dispose();
mainRoutine = RunCycle();
Log("Forced rescan.");
} else if (cmd == "sort") {
Log("Sort triggered.");
} else if (cmd == "craft") {
Log("Craft check triggered.");
} else if (cmd.StartsWith("loadout:")) {
string profile = cmd.Substring(8).Trim();
bool found = false;
foreach (var conn in connectors) {
if (conn.Status != MyShipConnectorStatus.Connected || conn.OtherConnector == null) continue;
string name = ExtractLoadoutName(conn.CustomName);
if (name == null) name = ExtractLoadoutName(conn.OtherConnector.CustomName);
if (name != null && name.Equals(profile, StringComparison.OrdinalIgnoreCase)) {
loadoutPhase[conn.EntityId] = LOAD_UNLOAD;
Log("Loadout triggered: '" + profile + "' on " + conn.CustomName);
found = true;
}
}
if (!found) Warn("No connected connector with [Loadout:" + profile + "] found");
} else if (cmd == "reset") {
Storage = "";
blueprintMap.Clear();
LoadBlueprintMap();
itemDb.Clear();
foreach (var a in assemblers) a.ClearQueue();
mainRoutine?.Dispose();
mainRoutine = RunCycle();
Log("Full reset.");
}
}
void UpdateEcho() {
Echo("=== Inventory Manager (Core) ===");
Echo("Step: " + stepIndex + " | " + statusText);
Echo("Blocks: " + inventoryBlocks.Count + " | Items tracked: " + itemDb.Count);
Echo("Instructions: " + Runtime.CurrentInstructionCount + "/" + Runtime.MaxInstructionCount);
Echo("Runtime: " + Math.Round(Runtime.LastRunTimeMs, 3) + "ms");
if (warnings.Count > 0) {
Echo("\nWarnings:");
for (int i = 0; i < Math.Min(warnings.Count, 5); i++)
Echo("  ! " + warnings[i]);
}
foreach (var p in allPanels) { if (p.CustomName.Contains(LCD_CRAFT_DBG)) p.WriteText(craftDbg); }
}
IEnumerator<bool> RunCycle() {
while (true) {
if (warnings.Count > 0) {
persistentWarnings.Clear();
persistentWarnings.AddRange(warnings);
}
warnings.Clear();
stepIndex = 1;
statusText = "Scanning";
ScanConnectors();
yield return true;
var sr = ScanBlocks();
while (sr.MoveNext()) yield return true;
ReadCustomTags();
if (BudgetExceeded()) yield return true;
RescanCustomTagContainers();
if (BudgetExceeded()) yield return true;
yield return true;
{var ci=CountAllItems();while(ci.MoveNext())yield return true;}
if (learningAssemblers.Count > 0) { LearnFromAssemblers(); if (BudgetExceeded()) yield return true; }
yield return true;
stepIndex = 2;
statusText = "Sorting";
var sortRoutine = SortItems();
while (sortRoutine.MoveNext()) {
if (BudgetExceeded()) yield return true;
}
stepIndex = 3;
statusText = "Managing";
if (enableReactorHandling) { ManageReactors(); if (BudgetExceeded()) yield return true; }
if (enableGeneratorHandling) { ManageGenerators(); if (BudgetExceeded()) yield return true; }
if (enableIrrigationHandling) { ManageIrrigation(); if (BudgetExceeded()) yield return true; }
if (enableRefineryHandling) {
var refRoutine = ManageRefineries();
while (refRoutine.MoveNext()) {
if (BudgetExceeded()) yield return true;
}
}
if (enableOreCycling) {
ReadSmeltTargets();
if (BudgetExceeded()) yield return true;
var oreRoutine = ManageOreCycling();
while (oreRoutine.MoveNext()) {
if (BudgetExceeded()) yield return true;
}
}
if (enableBottleFilling) { ManageBottles(); if (BudgetExceeded()) yield return true; }
if (enablePowerManagement) {
ReadPowerConfig();
if (BudgetExceeded()) yield return true;
ManagePower();
if (BudgetExceeded()) yield return true;
ManageGas();
if (BudgetExceeded()) yield return true;
}
stepIndex = 4;
statusText = "Crafting";
if (enableAutoCrafting) {
ReadCraftTargets();
if (BudgetExceeded()) yield return true;
MergeCustomTagTargets();
if (BudgetExceeded()) yield return true;
MergeLoadoutCraftNeeds();
if (BudgetExceeded()) yield return true;
var craftRoutine = RunAutoCrafting();
while (craftRoutine.MoveNext()) {
if (BudgetExceeded()) yield return true;
}
if (enableAutoDisassembly) {
ReadStasisTargets();
if (BudgetExceeded()) yield return true;
var disRoutine = RunAutoDisassembly();
while (disRoutine.MoveNext()) {
if (BudgetExceeded()) yield return true;
}
}
}
stepIndex = 5;
statusText = "Loadouts";
if (enableLoadouts) {
ReadLoadoutProfiles();
if (BudgetExceeded()) yield return true;
var loadRoutine = ProcessLoadouts();
while (loadRoutine.MoveNext()) {
if (BudgetExceeded()) yield return true;
}
}
if (enableShipFueling) {
var fuelRoutine = FuelConnectedShips();
while (fuelRoutine.MoveNext()) {
if (BudgetExceeded()) yield return true;
}
}
stepIndex = 6;
statusText = "Broadcasting";
{
var igcSb = new System.Text.StringBuilder();
igcSb.AppendLine("[LOG]");
foreach (var entry in actionLog) igcSb.AppendLine(entry);
igcSb.AppendLine("[WARN]");
var active = warnings.Count > 0 ? warnings : persistentWarnings;
foreach (var w in active) igcSb.AppendLine(w);
igcSb.AppendLine("[STATE]");
igcSb.AppendLine("step=" + stepIndex);
igcSb.AppendLine("status=" + statusText);
igcSb.AppendLine("powerMode=" + (powerDischargeMode ? "DISCHARGE" : "NORMAL"));
igcSb.AppendLine("gasMode=" + (gasStockpileMode ? "STOCKPILE" : "NORMAL"));
igcSb.AppendLine("blocks=" + inventoryBlocks.Count);
igcSb.AppendLine("items=" + itemDb.Count);
igcSb.AppendLine("instructions=" + Runtime.CurrentInstructionCount);
igcSb.AppendLine("maxInst=" + Runtime.MaxInstructionCount);
igcSb.AppendLine("runtime=" + Runtime.LastRunTimeMs.ToString("0.000"));
IGC.SendBroadcastMessage(IGC_CHANNEL, igcSb.ToString(), TransmissionDistance.CurrentConstruct);
}
if (showFillLevel) { if (BudgetExceeded()) yield return true; UpdateFillLevels(); }
yield return true;
stepIndex = 0;
statusText = "Idle";
}
}
bool BudgetExceeded(int limit = 2500) {
if ((DateTime.UtcNow - tickStart).TotalMilliseconds > tickBudgetMs) return true;
return Runtime.CurrentInstructionCount > limit;
}
void ScanConnectors() {
noSortGrids.Clear();
noManageGrids.Clear();
connectors.Clear();
GridTerminalSystem.GetBlocksOfType(connectors);
foreach (var conn in connectors) {
if (conn.Status != MyShipConnectorStatus.Connected) continue;
if (conn.OtherConnector == null) continue;
bool noSort = conn.CustomName.Contains(TAG_NO_SORT) ||
conn.OtherConnector.CustomName.Contains(TAG_NO_SORT) ||
conn.CustomName.Contains("[Loadout:") ||
conn.OtherConnector.CustomName.Contains("[Loadout:");
bool noManage = conn.CustomName.Contains(TAG_NO_MANAGE) ||
conn.OtherConnector.CustomName.Contains(TAG_NO_MANAGE);
if (noSort) {
var otherGrid = conn.OtherConnector.CubeGrid;
if (otherGrid != Me.CubeGrid) noSortGrids.Add(otherGrid);
else noSortGrids.Add(conn.CubeGrid);
}
if (noManage) {
var otherGrid = conn.OtherConnector.CubeGrid;
if (otherGrid != Me.CubeGrid) noManageGrids.Add(otherGrid);
else noManageGrids.Add(conn.CubeGrid);
}
}
}
IEnumerator<bool> ScanBlocks() {
allBlocks.Clear();
inventoryBlocks.Clear();
taggedContainers.Clear();
overflowContainers.Clear();
assemblers.Clear();
refineries.Clear();
reactors.Clear();
generators.Clear();
allPanels.Clear();
batteries.Clear();
hydrogenEngines.Clear();
learningAssemblers.Clear();
gasTanks.Clear();
irrigationBlocks.Clear();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(allBlocks, b => b.HasInventory || b is IMyTextPanel);
yield return true;
int scannedBlocks = 0;
foreach (var block in allBlocks) {
if (noManageGrids.Contains(block.CubeGrid)) continue;
if (block is IMyTextPanel && block.IsSameConstructAs(Me))
allPanels.Add(block as IMyTextPanel);
if (!block.HasInventory) continue;
if (IsLocked(block)) continue;
if (IsHidden(block)) continue;
bool onNoSortGrid = noSortGrids.Contains(block.CubeGrid);
if (onNoSortGrid && !(block is IMyReactor) && !(block is IMyGasGenerator)) continue;
inventoryBlocks.Add(block);
string name = block.CustomName;
string[] allTags = { TAG_ORE, TAG_INGOT, TAG_COMPONENT, TAG_AMMO, TAG_TOOL, TAG_BOTTLE, TAG_FOOD };
foreach (var tag in allTags) {
if (name.Contains(tag)) {
if (!taggedContainers.ContainsKey(tag))
taggedContainers[tag] = new List<IMyTerminalBlock>();
taggedContainers[tag].Add(block);
}
}
if (name.Contains(TAG_OVERFLOW)) overflowContainers.Add(block);
if (block is IMyAssembler && !name.Contains(TAG_MANUAL) && block.IsSameConstructAs(Me)) {
if (name.Contains(TAG_LEARN))
learningAssemblers.Add(block as IMyAssembler);
else
assemblers.Add(block as IMyAssembler);
}
if (block is IMyRefinery && block.IsSameConstructAs(Me)) {
if (!name.Contains(TAG_MANUAL))
refineries.Add(block as IMyRefinery);
else
(block as IMyRefinery).UseConveyorSystem = false;
}
if (block is IMyReactor && !name.Contains(TAG_MANUAL) && block.IsSameConstructAs(Me) && !onNoSortGrid)
reactors.Add(block as IMyReactor);
if (block is IMyGasGenerator && !name.Contains(TAG_MANUAL) && block.IsSameConstructAs(Me) && !onNoSortGrid)
generators.Add(block as IMyGasGenerator);
if (name.Contains("[Irrigation]") && !name.Contains(TAG_MANUAL) && block.IsSameConstructAs(Me))
irrigationBlocks.Add(block);
if (++scannedBlocks % 10 == 0) yield return true;
}
yield return true;
var tempBat = new List<IMyBatteryBlock>();
GridTerminalSystem.GetBlocksOfType(tempBat, b => b.IsSameConstructAs(Me)
&& !noManageGrids.Contains(b.CubeGrid) && !b.CustomName.Contains(TAG_MANUAL));
batteries.AddRange(tempBat);
yield return true;
var tempProd = new List<IMyPowerProducer>();
GridTerminalSystem.GetBlocksOfType(tempProd, b => b.IsSameConstructAs(Me)
&& !noManageGrids.Contains(b.CubeGrid) && !b.CustomName.Contains(TAG_MANUAL)
&& b.BlockDefinition.SubtypeId.Contains("HydrogenEngine"));
foreach (var p in tempProd) hydrogenEngines.Add(p);
var tempTanks = new List<IMyGasTank>();
GridTerminalSystem.GetBlocksOfType(tempTanks, b => b.IsSameConstructAs(Me)
&& !noManageGrids.Contains(b.CubeGrid) && !b.CustomName.Contains(TAG_MANUAL));
gasTanks.AddRange(tempTanks);
foreach (var kv in taggedContainers) {
kv.Value.Sort((a, b) => GetPriority(b).CompareTo(GetPriority(a)));
}
}
void RescanCustomTagContainers() {
foreach (var block in inventoryBlocks) {
string name = block.CustomName;
foreach (var customTag in customTags.Keys) {
string bracket = "[" + customTag + "]";
if (name.Contains(bracket)) {
if (!taggedContainers.ContainsKey(bracket))
taggedContainers[bracket] = new List<IMyTerminalBlock>();
if (!taggedContainers[bracket].Contains(block))
taggedContainers[bracket].Add(block);
}
}
}
foreach (var kv in taggedContainers) {
kv.Value.Sort((a, b) => GetPriority(b).CompareTo(GetPriority(a)));
}
}
bool IsLocked(IMyTerminalBlock block) {
string name = block.CustomName.ToLower();
foreach (var kw in LOCKED_KEYWORDS) { if (name.Contains(kw)) return true; }
return false;
}
bool IsHidden(IMyTerminalBlock block) {
string name = block.CustomName;
foreach (var kw in HIDDEN_KEYWORDS) { if (name.Contains(kw)) return true; }
return false;
}
bool IsInCustomTag(IMyTerminalBlock block) {
string name = block.CustomName;
foreach (var tagName in customTags.Keys) {
if (name.Contains("[" + tagName + "]")) return true;
}
return false;
}
int GetPriority(IMyTerminalBlock block) {
string name = block.CustomName;
int idx = name.IndexOf("[P:");
if (idx < 0) return 0;
string sub = name.Substring(idx + 3);
int end = sub.IndexOf(']');
if (end < 0) return 0;
int priority;
if (int.TryParse(sub.Substring(0, end), out priority)) return priority;
return 0;
}
void ReadCustomTags() {
customTags.Clear();
customNames.Clear();
customTagPriorities.Clear();
foreach (var panel in allPanels) {
if (!panel.CustomName.Contains(LCD_TAG_DEF)) continue;
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data) || !data.Contains("=")) continue;
string currentSection = null;
var lines = data.Split('\n');
foreach (var rawLine in lines) {
string line = rawLine.Trim();
if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
if (line.StartsWith("[") && line.EndsWith("]")) {
currentSection = line.Substring(1, line.Length - 2);
if (currentSection.StartsWith("Tag:")) {
string tagName = currentSection.Substring(4);
if (!customTags.ContainsKey(tagName))
customTags[tagName] = new Dictionary<MyItemType, double>();
}
continue;
}
if (currentSection == null || !line.Contains("=")) continue;
var parts = line.Split(new char[]{'='}, 2);
if (parts.Length != 2) continue;
string key = parts[0].Trim();
string val = parts[1].Trim();
if (currentSection == "Rename") {
if (!string.IsNullOrEmpty(val)) customNames[key] = val;
} else if (currentSection.StartsWith("Tag:")) {
string tagName = currentSection.Substring(4);
if (key.ToLower() == "priority") {
int pri;
if (int.TryParse(val, out pri))
customTagPriorities[tagName] = pri;
continue;
}
string valLower = val.ToLower();
double quota = -1;
bool include = false;
if (valLower == "true" || valLower == "all") {
include = true; quota = -1;
} else {
double parsed;
if (double.TryParse(val, out parsed) && parsed > 0) {
include = true; quota = parsed;
}
}
if (include) {
bool matched = false;
foreach (var dbKv in itemDb) {
if (dbKv.Value.displayName == key || dbKv.Key.SubtypeId == key) {
customTags[tagName][dbKv.Key] = quota;
matched = true;
break;
}
}
if (!matched && key.Contains("/")) {
int slash = key.IndexOf('/');
var itemType = new MyItemType(key.Substring(0, slash), key.Substring(slash + 1));
customTags[tagName][itemType] = quota;
}
}
}
}
}
foreach (var panel in allPanels) {
if (panel.CustomName.Contains(LCD_TAG_DEF)) continue;
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data) || !data.Contains("[Rename]")) continue;
bool inRename = false;
foreach (var rawLine in data.Split('\n')) {
string line = rawLine.Trim();
if (line == "[Rename]") { inRename = true; continue; }
if (line.StartsWith("[")) { inRename = false; continue; }
if (!inRename || line.StartsWith("#") || !line.Contains("=")) continue;
var parts = line.Split(new char[]{'='}, 2);
if (parts.Length == 2 && !string.IsNullOrEmpty(parts[1].Trim()))
customNames[parts[0].Trim()] = parts[1].Trim();
}
}
}
IEnumerator<bool> CountAllItems() {
globalCounts.Clear();
int n = 0;
foreach (var block in inventoryBlocks) {
if (IsHidden(block)) continue;
if (IsInCustomTag(block)) continue;
for (int invIdx = 0; invIdx < block.InventoryCount; invIdx++) {
var inv = block.GetInventory(invIdx);
var items = new List<MyInventoryItem>();
inv.GetItems(items);
foreach (var item in items) {
if (!itemDb.ContainsKey(item.Type)) {
itemDb[item.Type] = new ItemInfo(item.Type);
Log("New: "+item.Type.SubtypeId);
}
double amount = (double)item.Amount;
if (globalCounts.ContainsKey(item.Type))
globalCounts[item.Type] += amount;
else
globalCounts[item.Type] = amount;
}
}
if (++n % 4 == 0) yield return true;
}
}
double GetGlobalCount(MyItemType type) {
double val;
globalCounts.TryGetValue(type, out val);
return val;
}
double GetItemAmount(MyItemType type, IMyTerminalBlock block, int invIdx = 0) {
return (double)block.GetInventory(invIdx).GetItemAmount(type);
}
IEnumerator<bool> SortItems() {
int ops = 0;
foreach (var block in inventoryBlocks) {
if (block is IMyReactor || block is IMyGasGenerator) continue;
bool isTaggedContainer = false;
foreach (var kv in taggedContainers) {
if (kv.Value.Contains(block)) { isTaggedContainer = true; break; }
}
int invCount = block.InventoryCount;
for (int invIdx = 0; invIdx < invCount; invIdx++) {
if (block is IMyRefinery && invIdx == 0) continue; // skip refinery input, sort output
if (block is IMyAssembler) {
var asm = block as IMyAssembler;
// Determine which inventory is input vs output for current mode
bool isInput = (asm.Mode == MyAssemblerMode.Assembly && invIdx == 0)
|| (asm.Mode == MyAssemblerMode.Disassembly && invIdx == 1);
// Output (inv 1 in Assembly, inv 0 in Disassembly) always gets sorted
// Input only gets sorted when the queue is empty — no active crafting/disassembly
if (isInput && !asm.IsQueueEmpty) continue;
}
var inv = block.GetInventory(invIdx);
var items = new List<MyInventoryItem>();
inv.GetItems(items);
for (int i = items.Count - 1; i >= 0; i--) {
var item = items[i];
if (isTaggedContainer) {
string ownerTag = GetItemOwnerCustomTag(block, item.Type);
if (ownerTag != null) {
double quota;
if (customTags[ownerTag].TryGetValue(item.Type, out quota)) {
if (quota < 0) continue;
double totalInTag = GetCustomTagFillAmount(ownerTag, item.Type);
double excess = totalInTag - quota;
if (excess < 0.5) continue;
double toMove = Math.Min(excess, (double)item.Amount);
string builtinTag = null;
BUILTIN_CATEGORIES.TryGetValue(item.Type.TypeId, out builtinTag);
bool movedExcess = false;
if (builtinTag != null && taggedContainers.ContainsKey(builtinTag)) {
foreach (var dest in taggedContainers[builtinTag]) {
if (dest == block) continue;
var destInv = dest.GetInventory(0);
if (!inv.CanTransferItemTo(destInv, item.Type)) continue;
if (!destInv.CanItemsBeAdded(1, item.Type)) continue;
if (inv.TransferItemTo(destInv, i, null, true, (VRage.MyFixedPoint)toMove)) {
Log("Quota excess: " + GetDisplayName(item.Type) + " x" + FormatNumber(toMove) + " -> " + dest.CustomName);
movedExcess = true;
break;
}
}
}
if (!movedExcess) {
foreach (var dest in overflowContainers) {
if (dest == block) continue;
var destInv = dest.GetInventory(0);
if (!inv.CanTransferItemTo(destInv, item.Type)) continue;
if (inv.TransferItemTo(destInv, i, null, true, (VRage.MyFixedPoint)toMove)) {
Log("Quota excess: " + GetDisplayName(item.Type) + " x" + FormatNumber(toMove) + " -> " + dest.CustomName);
break;
}
}
}
ops++;
if (ops % 4 == 0) yield return true;
continue;
}
}
}
string destTag = GetDestinationTag(item.Type);
if (destTag == null) {
foreach (var dest in overflowContainers) {
if (dest == block) continue;
var destInv = dest.GetInventory(0);
if (!inv.CanTransferItemTo(destInv, item.Type)) continue;
if (!destInv.CanItemsBeAdded(1, item.Type)) continue;
if (inv.TransferItemTo(destInv, i, null, true)) {
Log("Unsorted " + item.Type.SubtypeId + " -> " + dest.CustomName);
break;
}
}
ops++;
if (ops % 4 == 0) yield return true;
continue;
}
if (isTaggedContainer && taggedContainers.ContainsKey(destTag) &&
taggedContainers[destTag].Contains(block)) continue;
double moveLimit = -1;
string rawDestTag = destTag.Length > 2 ? destTag.Substring(1, destTag.Length - 2) : "";
Dictionary<MyItemType, double> tagItemQuotas;
if (customTags.TryGetValue(rawDestTag, out tagItemQuotas)) {
double tagQuota;
if (tagItemQuotas.TryGetValue(item.Type, out tagQuota) && tagQuota >= 0) {
double current = GetCustomTagFillAmount(rawDestTag, item.Type);
moveLimit = tagQuota - current;
if (moveLimit <= 0) continue;
}
}
bool moved = false;
if (taggedContainers.ContainsKey(destTag)) {
foreach (var dest in taggedContainers[destTag]) {
if (dest == block) continue;
var destInv = dest.GetInventory(0);
if (!destInv.CanItemsBeAdded(1, item.Type)) continue;
if (!inv.CanTransferItemTo(destInv, item.Type)) continue;
if (moveLimit >= 0) {
double toMove = Math.Min(moveLimit, (double)item.Amount);
if (toMove <= 0) break;
if (inv.TransferItemTo(destInv, i, null, true, (VRage.MyFixedPoint)toMove)) {
moved = true; break;
}
} else {
if (inv.TransferItemTo(destInv, i, null, true)) {
moved = true; break;
}
}
}
}
if (!moved) {
foreach (var dest in overflowContainers) {
if (dest == block) continue;
var destInv = dest.GetInventory(0);
if (!inv.CanTransferItemTo(destInv, item.Type)) continue;
if (!destInv.CanItemsBeAdded(1, item.Type)) continue;
if (inv.TransferItemTo(destInv, i, null, true)) {
Log("Overflow " + item.Type.SubtypeId + " -> " + dest.CustomName);
break;
}
}
}
ops++;
if (ops % 4 == 0) yield return true;
}
}
}
}
string GetDestinationTag(MyItemType itemType) {
string bestTag = null;
int bestPri = -1;
foreach (var kv in customTags) {
double quota;
if (kv.Value.TryGetValue(itemType, out quota)) {
if (quota >= 0) {
double current = GetCustomTagFillAmount(kv.Key, itemType);
if (current >= quota) continue;
}
int pri = 0;
customTagPriorities.TryGetValue(kv.Key, out pri);
if (bestTag == null || pri > bestPri) {
bestTag = "[" + kv.Key + "]";
bestPri = pri;
}
}
}
if (bestTag != null) return bestTag;
string typeId = itemType.TypeId;
string tag;
if (BUILTIN_CATEGORIES.TryGetValue(typeId, out tag)) return tag;
return null;
}
double GetCustomTagFillAmount(string tagName, MyItemType itemType) {
string bracket = "[" + tagName + "]";
if (!taggedContainers.ContainsKey(bracket)) return 0;
double total = 0;
foreach (var block in taggedContainers[bracket])
total += GetItemAmount(itemType, block);
return total;
}
string GetItemOwnerCustomTag(IMyTerminalBlock block, MyItemType itemType) {
string bestTag = null;
int bestPri = -1;
foreach (var kv in customTags) {
if (!kv.Value.ContainsKey(itemType)) continue;
string bracket = "[" + kv.Key + "]";
if (!taggedContainers.ContainsKey(bracket)) continue;
if (!taggedContainers[bracket].Contains(block)) continue;
int pri = 0;
customTagPriorities.TryGetValue(kv.Key, out pri);
if (bestTag == null || pri > bestPri) {
bestTag = kv.Key;
bestPri = pri;
}
}
return bestTag;
}
void ManageReactors() {
MyItemType uraniumType = MyItemType.Parse(OB + T_INGOT + "/Uranium");
foreach (var reactor in reactors) {
if (!reactor.IsFunctional) continue;
if (!enablePowerManagement && !reactor.Enabled) continue;
reactor.UseConveyorSystem = false;
double target = reactor.CubeGrid.GridSize < 1f ? uraniumPerSmallReactor : uraniumPerLargeReactor;
double current = GetItemAmount(uraniumType, reactor);
if (current < target - 0.05) {
double needed = target - current;
var source = FindItemSource(uraniumType);
if (source != null) TransferItems(uraniumType, source, 0, reactor, 0, needed);
} else if (current > target + 0.05) {
double excess = current - target;
var dest = FindEmptyContainer(TAG_INGOT);
if (dest != null) TransferItems(uraniumType, reactor, 0, dest, 0, excess);
}
}
}
void ManageGenerators() {
MyItemType iceType = MyItemType.Parse(OB + T_ORE + "/Ice");
foreach (var gen in generators) {
if (!gen.IsFunctional || !gen.Enabled) continue;
gen.UseConveyorSystem = false;
double target = gen.CubeGrid.GridSize < 1f ? icePerSmallGenerator : icePerLargeGenerator;
double current = GetItemAmount(iceType, gen);
if (current < target * 0.8) {
double needed = target - current;
var source = FindItemSource(iceType);
if (source != null) TransferItems(iceType, source, 0, gen, 0, needed);
}
}
}
void ManageIrrigation() {
if (irrigationBlocks.Count == 0) return;
MyItemType iceType = MyItemType.Parse(OB + T_ORE + "/Ice");
foreach (var block in irrigationBlocks) {
var inv = block.GetInventory(0);
double current = (double)inv.GetItemAmount(iceType);
if (current < icePerIrrigation * 0.8) {
double needed = icePerIrrigation - current;
var source = FindItemSource(iceType);
if (source != null) TransferItems(iceType, source, 0, block, 0, needed);
}
}
}
void ManageBottles() {
MyItemType o2Bottle = new MyItemType(OB + T_OXYB, "OxygenBottle");
MyItemType h2Bottle = new MyItemType(OB + T_GASB, "HydrogenBottle");
foreach (var gen in generators) {
if (!gen.IsFunctional || !gen.Enabled) continue;
var inv = gen.GetInventory(0);
var items = new List<MyInventoryItem>();
inv.GetItems(items);
int o2Count = 0, h2Count = 0;
for (int i = items.Count - 1; i >= 0; i--) {
var item = items[i];
bool isO2 = (MyItemType)item.Type == o2Bottle;
bool isH2 = (MyItemType)item.Type == h2Bottle;
if (!isO2 && !isH2) continue;
if (isO2) o2Count++;
if (isH2) h2Count++;
bool isExtra = (isO2 && o2Count > 1) || (isH2 && h2Count > 1);
if (isExtra) {
var dest = FindEmptyContainer(TAG_BOTTLE);
if (dest != null) {
inv.TransferItemTo(dest.GetInventory(0), i, null, true);
if (isO2) o2Count--;
if (isH2) h2Count--;
}
}
}
if (o2Count == 0) FindAndInsertBottle(o2Bottle, inv);
if (h2Count == 0) FindAndInsertBottle(h2Bottle, inv);
}
}
IEnumerator<bool> ManageRefineries() {
if (refineries.Count == 0) yield break;
foreach (var refinery in refineries) {
if (!refinery.IsWorking) continue;
if (refinery.CustomName.Contains(TAG_MANUAL)) continue;
PullRefineryOutput(refinery);
yield return true;
}
}
void PullRefineryOutput(IMyRefinery refinery) {
var outInv = refinery.GetInventory(1);
var items = new List<MyInventoryItem>();
outInv.GetItems(items);
for (int i = items.Count - 1; i >= 0; i--) {
var item = items[i];
bool moved = false;
var destTag = GetDestinationTag(item.Type);
if (destTag != null && taggedContainers.ContainsKey(destTag)) {
foreach (var dest in taggedContainers[destTag]) {
var destInv = dest.GetInventory(0);
if (!outInv.CanTransferItemTo(destInv, item.Type)) continue;
if (!destInv.CanItemsBeAdded(1, item.Type)) continue;
if (outInv.TransferItemTo(destInv, i, null, true, item.Amount)) {
moved = true; break;
}
}
}
if (!moved) {
foreach (var dest in overflowContainers) {
var destInv = dest.GetInventory(0);
if (!outInv.CanTransferItemTo(destInv, item.Type)) continue;
if (!destInv.CanItemsBeAdded(1, item.Type)) continue;
if (outInv.TransferItemTo(destInv, i, null, true, item.Amount)) {
moved = true; break;
}
}
}
}
}
double ParseSmeltAmount(string str) {
str = str.Trim().ToUpper();
double mult = 1;
if (str.EndsWith("G")) { mult = 1000000000; str = str.Substring(0, str.Length - 1); }
else if (str.EndsWith("M")) { mult = 1000000; str = str.Substring(0, str.Length - 1); }
else if (str.EndsWith("K")) { mult = 1000; str = str.Substring(0, str.Length - 1); }
double val;
if (!double.TryParse(str.Trim(), out val)) return -1;
return val * mult;
}
void ReadSmeltTargets() {
ingotTargets.Clear();
ingotPriorities.Clear();
foreach (var panel in allPanels) {
string smeltFilter = GetLcdFilter(panel.CustomName, "[INV:Smelt");
if (smeltFilter == null) continue;
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data)) continue;
var lines = data.Split('\n');
foreach (var rawLine in lines) {
string line = rawLine.Trim();
if (line.StartsWith("[") || line.StartsWith("#") || !line.Contains("=")) continue;
var parts = line.Split(new char[]{'='}, 2);
if (parts.Length != 2) continue;
string ingotName = parts[0].Trim();
if (ingotName == "Font" || ingotName == "FontSize" || ingotName == "Padding") continue;
string valStr = parts[1].Trim();
int priority = 0;
int commaIdx = valStr.IndexOf(',');
if (commaIdx >= 0) {
int.TryParse(valStr.Substring(commaIdx + 1).Trim(), out priority);
valStr = valStr.Substring(0, commaIdx);
}
double target = ParseSmeltAmount(valStr);
if (target <= 0) continue;
MyItemType ingotType = new MyItemType(OB + T_INGOT, ingotName);
if (itemDb.ContainsKey(ingotType) || globalCounts.ContainsKey(ingotType)) {
ingotTargets[ingotType] = target;
if (priority > 0) ingotPriorities[ingotType] = priority;
} else {
foreach (var kv in itemDb) {
if (!kv.Key.TypeId.Contains(T_INGOT)) continue;
if (kv.Value.displayName == ingotName || kv.Key.SubtypeId == ingotName) {
ingotTargets[kv.Key] = target;
if (priority > 0) ingotPriorities[kv.Key] = priority;
break;
}
}
}
}
}
}
IEnumerator<bool> ManageOreCycling() {
if (refineries.Count == 0) yield break;
string[] oreNames = { "Iron", "Nickel", "Cobalt", "Silicon", "Silver",
"Gold", "Platinum", "Uranium", "Magnesium", "Stone", "Scrap" };
var allOres = new List<MyItemType>();
foreach (var name in oreNames) {
MyItemType oreType = new MyItemType(OB + T_ORE, name);
if (GetGlobalCount(oreType) > 0) allOres.Add(oreType);
}
var neededOres = new List<KeyValuePair<MyItemType, double>>();
var neededSet = new HashSet<string>();
foreach (var kv in ingotTargets) {
double current = GetGlobalCount(kv.Key);
if (current >= kv.Value) continue;
string oreSubtype = kv.Key.SubtypeId;
MyItemType oreType = new MyItemType(OB + T_ORE, oreSubtype);
if (GetGlobalCount(oreType) <= 0) continue;
double urgency = (kv.Value - current) / kv.Value;
neededOres.Add(new KeyValuePair<MyItemType, double>(oreType, urgency));
neededSet.Add(oreSubtype);
}
neededOres.Sort(CompareOresByPriority);
bool priorityMode = neededOres.Count > 0;
var topOres = new List<MyItemType>();
if (priorityMode) {
int topPri = 0;
MyItemType ingTop = new MyItemType(OB + T_INGOT, neededOres[0].Key.SubtypeId);
ingotPriorities.TryGetValue(ingTop, out topPri);
foreach (var no in neededOres) {
int pri = 0;
MyItemType ing = new MyItemType(OB + T_INGOT, no.Key.SubtypeId);
ingotPriorities.TryGetValue(ing, out pri);
if (pri == topPri && GetGlobalCount(no.Key) > 0)
topOres.Add(no.Key);
}
if (topOres.Count == 0) {
foreach (var no in neededOres) {
if (GetGlobalCount(no.Key) > 0) { topOres.Add(no.Key); break; }
}
}
}
var topOreSet = new HashSet<string>();
foreach (var o in topOres) topOreSet.Add(o.SubtypeId);
int assignIdx = 0;
foreach (var refinery in refineries) {
if (!refinery.IsWorking) continue;
if (refinery.CustomName.Contains(TAG_MANUAL)) continue;
PullRefineryOutput(refinery);
var inputInv = refinery.GetInventory(0);
var items = new List<MyInventoryItem>();
inputInv.GetItems(items);
refinery.UseConveyorSystem = false;
if (priorityMode && topOres.Count > 0) {
MyItemType assignedOre = topOres[assignIdx % topOres.Count];
assignIdx++;
for (int i = items.Count - 1; i >= 0; i--) {
if (!items[i].Type.TypeId.Contains(T_ORE)) continue;
if (items[i].Type.SubtypeId == assignedOre.SubtypeId) continue;
var dest = FindEmptyContainer(TAG_ORE);
if (dest != null)
inputInv.TransferItemTo(dest.GetInventory(0), i, null, true);
}
double rightAmount = 0;
var refreshed = new List<MyInventoryItem>();
inputInv.GetItems(refreshed);
foreach (var item in refreshed) {
if (item.Type.SubtypeId == assignedOre.SubtypeId)
rightAmount += (double)item.Amount;
}
if (rightAmount < orePerRefinery * 0.5)
FillRefineryOre(refinery, assignedOre, orePerRefinery - rightAmount);
} else if (allOres.Count > 0 && ingotTargets.Count == 0) {
double currentAmount = 0;
foreach (var item in items) {
if (item.Type.TypeId.Contains(T_ORE))
currentAmount += (double)item.Amount;
}
if (currentAmount < 100) {
MyItemType nextOre = allOres[oreRotationIndex % allOres.Count];
oreRotationIndex = (oreRotationIndex + 1) % allOres.Count;
FillRefineryOre(refinery, nextOre, orePerRefinery);
Log("Ore: cycled " + nextOre.SubtypeId + " -> " + refinery.CustomName);
}
}
yield return true;
}
}
void FillRefineryOre(IMyRefinery refinery, MyItemType oreType, double amount) {
if (taggedContainers.ContainsKey(TAG_ORE)) {
foreach (var block in taggedContainers[TAG_ORE]) {
double available = GetItemAmount(oreType, block);
if (available <= 0) continue;
double toMove = Math.Min(available, amount);
TransferItems(oreType, block, 0, refinery, 0, toMove);
amount -= toMove;
if (amount <= 0) return;
}
}
if (amount > 0) {
var source = FindItemSource(oreType, refinery);
if (source != null)
TransferItems(oreType, source, 0, refinery, 0, amount);
}
}
void ReadPowerConfig() {
foreach (var panel in allPanels) {
if (!panel.CustomName.Contains(LCD_POWER)) continue;
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data) || !data.Contains("=")) continue;
foreach (var rawLine in data.Split('\n')) {
string line = rawLine.Trim();
if (line.StartsWith("[") || line.StartsWith("#") || !line.Contains("=")) continue;
var parts = line.Split('=');
if (parts.Length != 2) continue;
string key = parts[0].Trim();
string val = parts[1].Trim();
if (key == "UpperThreshold") double.TryParse(val, out powerUpperThreshold);
else if (key == "LowerThreshold") double.TryParse(val, out powerLowerThreshold);
else if (key == "ManageReactors") managePowerReactors = val.ToLower() == "true";
else if (key == "ManageHydroEngines") managePowerHydroEngines = val.ToLower() == "true";
else if (key == "GasUpperThreshold") double.TryParse(val, out gasUpperThreshold);
else if (key == "GasLowerThreshold") double.TryParse(val, out gasLowerThreshold);
else if (key == "ManageGasGenerators") manageGasGenerators = val.ToLower() == "true";
else if (key == "IrrigationIce") double.TryParse(val, out icePerIrrigation);
}
}
}
void ManagePower() {
if (batteries.Count == 0) return;
float totalStored = 0, totalMax = 0;
foreach (var bat in batteries) {
totalStored += bat.CurrentStoredPower;
totalMax += bat.MaxStoredPower;
}
double chargePercent = totalMax > 0 ? (totalStored / totalMax) * 100 : 0;
if (!powerDischargeMode && chargePercent >= powerUpperThreshold) {
powerDischargeMode = true;
Log("Power: " + chargePercent.ToString("0.0") + "% >= " + powerUpperThreshold + "%, DISCHARGE mode");
} else if (powerDischargeMode && chargePercent <= powerLowerThreshold) {
powerDischargeMode = false;
Log("Power: " + chargePercent.ToString("0.0") + "% <= " + powerLowerThreshold + "%, NORMAL mode");
}
if (powerDischargeMode) {
if (managePowerReactors) {
foreach (var reactor in reactors) reactor.Enabled = false;
}
if (managePowerHydroEngines) {
foreach (var engine in hydrogenEngines)
(engine as IMyFunctionalBlock).Enabled = false;
}
foreach (var bat in batteries) bat.ChargeMode = ChargeMode.Discharge;
} else {
if (managePowerReactors) {
foreach (var reactor in reactors) reactor.Enabled = true;
}
if (managePowerHydroEngines) {
foreach (var engine in hydrogenEngines)
(engine as IMyFunctionalBlock).Enabled = true;
}
foreach (var bat in batteries) bat.ChargeMode = ChargeMode.Auto;
}
}
void ManageGas() {
if (gasTanks.Count == 0 || !manageGasGenerators) return;
double h2Total = 0; int h2Count = 0;
foreach (var tank in gasTanks) {
if (tank.BlockDefinition.SubtypeId.Contains("Hydrogen")) {
h2Total += tank.FilledRatio;
h2Count++;
}
}
if (h2Count == 0) return;
double h2Percent = (h2Total / h2Count) * 100;
if (!gasStockpileMode && h2Percent >= gasUpperThreshold) {
gasStockpileMode = true;
Log("Gas: H2 " + h2Percent.ToString("0.0") + "% >= " + gasUpperThreshold + "%, generators OFF");
} else if (gasStockpileMode && h2Percent <= gasLowerThreshold) {
gasStockpileMode = false;
Log("Gas: H2 " + h2Percent.ToString("0.0") + "% <= " + gasLowerThreshold + "%, generators ON");
}
foreach (var gen in generators) {
if (gen.CustomName.Contains(TAG_MANUAL)) continue;
gen.Enabled = !gasStockpileMode;
}
}
void ReadCraftTargets() {
craftTargets.Clear();
craftSources.Clear();
craftPriorities.Clear();
foreach (var panel in allPanels) {
string craftFilter = GetLcdFilter(panel.CustomName, "[INV:Craft");
if (craftFilter == null) continue;
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data)) continue;
foreach (var rawLine in data.Split('\n')) {
string line = rawLine.Trim();
if (line.StartsWith("[") || line.StartsWith("#") || line.StartsWith("~") || !line.Contains("=")) continue;
var eq = line.Split(new char[]{'='}, 2);
if (eq.Length != 2) continue;
string itemSpec = eq[0].Trim();
if (itemSpec == "Font" || itemSpec == "FontSize" || itemSpec == "Padding") continue;
// Parse value side: amount and optional ,P:N priority
string valStr = eq[1].Trim();
int priority = 0;
int commaIdx = valStr.IndexOf(",P:", StringComparison.OrdinalIgnoreCase);
if (commaIdx >= 0) {
int.TryParse(valStr.Substring(commaIdx + 3).Trim(), out priority);
valStr = valStr.Substring(0, commaIdx).Trim();
}
double target;
if (!double.TryParse(valStr, out target)) continue;
if (target <= 0) continue;
string lookupId = itemSpec;
string customName = null;
int colonIdx = itemSpec.IndexOf(':');
if (colonIdx > 0) {
lookupId = itemSpec.Substring(0, colonIdx).Trim();
customName = itemSpec.Substring(colonIdx + 1).Trim();
}
bool found = false;
foreach (var kv in itemDb) {
if (kv.Key.SubtypeId == lookupId || kv.Value.displayName == lookupId || kv.Value.displayName == itemSpec) {
craftTargets[kv.Key] = target;
craftSources[kv.Key] = panel.CustomName;
craftPriorities[kv.Key] = priority;
if (!string.IsNullOrEmpty(customName) && customName != kv.Value.displayName)
customNames[kv.Key.SubtypeId] = customName;
found = true;
break;
}
}
if (!found) {
if (lookupId.Contains("/")) {
var tp = lookupId.Split('/');
if (tp.Length == 2) {
var it = new MyItemType(tp[0], tp[1]);
craftTargets[it] = target;
craftSources[it] = panel.CustomName;
craftPriorities[it] = priority;
if (!string.IsNullOrEmpty(customName)) customNames[tp[1]] = customName;
}
} else {
string[] tryTypes = { OB+T_COMP, OB+T_AMMO, OB+T_GUN, OB+T_FOOD, OB+T_OXYB, OB+T_GASB, OB+T_PHYS };
bool matched = false;
foreach (var tid in tryTypes) {
var candidate = new MyItemType(tid, lookupId);
if (globalCounts.ContainsKey(candidate)) {
craftTargets[candidate] = target;
craftSources[candidate] = panel.CustomName;
craftPriorities[candidate] = priority;
matched = true;
break;
}
}
if (!matched) {
var fallbackType = new MyItemType(OB + T_COMP, lookupId);
craftTargets[fallbackType] = target;
craftSources[fallbackType] = panel.CustomName;
craftPriorities[fallbackType] = priority;
}
}
}
}
}
}
IEnumerator<bool> RunAutoCrafting() {
if (assemblers.Count == 0) yield break;
bool hasDeficit = false;
foreach (var kv in craftTargets) {
if (GetGlobalCount(kv.Key) < kv.Value) { hasDeficit = true; break; }
}
foreach (var asm in assemblers) {
if (!asm.IsWorking || asm.CustomName.Contains(TAG_MANUAL)) continue;
if (asm.Mode != MyAssemblerMode.Disassembly) continue;
if (asm.IsQueueEmpty) {
asm.Mode = MyAssemblerMode.Assembly;
} else if (hasDeficit && stasisTargets.Count == 0) {
asm.ClearQueue();
asm.Mode = MyAssemblerMode.Assembly;
Log("Reclaimed disassembler: " + asm.CustomName);
}
}
var usableAssemblers = new List<IMyAssembler>();
foreach (var asm in assemblers) {
if (asm.CustomName.Contains(TAG_MANUAL)) continue;
if (!asm.IsWorking) continue;
asm.CooperativeMode = false;
asm.Repeating = false;
asm.UseConveyorSystem = true;
if (asm.Mode == MyAssemblerMode.Assembly)
usableAssemblers.Add(asm);
}
if (usableAssemblers.Count == 0) yield break;
if (craftTargets.Count == 0) {
foreach (var asm in usableAssemblers) {
if (asm.IsQueueEmpty) continue;
asm.ClearQueue();
}
yield break;
}
// Build ordered list of items to craft, sorted by P: descending then deficit ratio
craftDbg = "asm=" + usableAssemblers.Count + "\n";
var craftEntries = new List<KeyValuePair<MyItemType, double>>();
foreach (var kv in craftTargets) {
double deficit = kv.Value - GetGlobalCount(kv.Key);
craftEntries.Add(new KeyValuePair<MyItemType, double>(kv.Key, deficit));
}
craftEntries.Sort(CompareCraftEntriesByPriority);
if (BudgetExceeded()) yield return true;
// Build shareMap (equal split across all assemblers) and priorityOrder (queue slot order)
var shareMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
var bpIdMap = new Dictionary<string, MyDefinitionId>(StringComparer.OrdinalIgnoreCase);
var priorityOrder = new List<string>();
foreach (var e in craftEntries) {
MyDefinitionId bpId;
if (!TryGetBlueprint(e.Key, out bpId)) {
if (e.Value > 0) {
string src = ""; craftSources.TryGetValue(e.Key, out src);
Warn(GetDisplayName(e.Key) + ": no assembler (from " + src + ")");
}
continue;
}
string bpKey = bpId.SubtypeId.ToString();
bpIdMap[bpKey] = bpId;
priorityOrder.Add(bpKey);
if (e.Value <= 0) {
shareMap[bpKey] = 0;
} else {
// Store total needed — we'll distribute per-assembler in the apply loop
shareMap[bpKey] = (int)e.Value;
int p = 0; craftPriorities.TryGetValue(e.Key, out p);
craftDbg += e.Key.SubtypeId + " P=" + p + " total=" + (int)e.Value + "\n";
}
}
// Apply to each assembler — clear and rebuild if queue order or amounts don't match
// Build per-assembler share map: distribute total evenly, last assembler gets remainder
// This ensures total crafted never exceeds the actual deficit
if (BudgetExceeded()) yield return true;
var perAsmShare = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
foreach (var kv in shareMap) {
if (kv.Value <= 0) { perAsmShare[kv.Key] = new int[usableAssemblers.Count]; continue; }
var shares = new int[usableAssemblers.Count];
int total = kv.Value;
int each = total / usableAssemblers.Count;
int rem = total % usableAssemblers.Count;
for (int i = 0; i < usableAssemblers.Count; i++) {
shares[i] = each + (i < rem ? 1 : 0);
}
perAsmShare[kv.Key] = shares;
}
for (int asmIdx = 0; asmIdx < usableAssemblers.Count; asmIdx++) {
var asm = usableAssemblers[asmIdx];
var aq = new List<MyProductionItem>();
asm.GetQueue(aq);
// Check if queue already matches this assembler's assigned shares
int expectedCount = 0;
foreach (var k in priorityOrder) { if (perAsmShare.ContainsKey(k) && perAsmShare[k][asmIdx] > 0) expectedCount++; }
bool ok = (aq.Count == expectedCount);
if (ok) {
int qi = 0;
foreach (var bpKey in priorityOrder) {
int want = perAsmShare.ContainsKey(bpKey) ? perAsmShare[bpKey][asmIdx] : 0;
if (want <= 0) continue;
if (qi >= aq.Count || aq[qi].BlueprintId.SubtypeId.ToString() != bpKey || (int)aq[qi].Amount != want) { ok = false; break; }
qi++;
}
}
if (!ok) {
asm.ClearQueue();
foreach (var bpKey in priorityOrder) {
int want = perAsmShare.ContainsKey(bpKey) ? perAsmShare[bpKey][asmIdx] : 0;
if (want <= 0) continue;
MyDefinitionId bpId; if (!bpIdMap.TryGetValue(bpKey, out bpId)) continue;
if (!asm.CanUseBlueprint(bpId)) continue;
asm.AddQueueItem(bpId, (MyFixedPoint)want);
}
}
yield return true;
}
}
void MergeCustomTagTargets() {
foreach (var tagKv in customTags) {
string tagName = tagKv.Key;
foreach (var itemKv in tagKv.Value) {
if (itemKv.Value <= 0) continue; // skip unlimited
// Ores cannot be crafted — skip to avoid assembler warnings
if (itemKv.Key.TypeId.Contains(T_ORE)) continue;
double quota = itemKv.Value;
double inTag = GetCustomTagFillAmount(tagName, itemKv.Key);
double deficit = quota - inTag;
if (deficit <= 0) continue;
if (craftTargets.ContainsKey(itemKv.Key))
craftTargets[itemKv.Key] += deficit;
else {
craftTargets[itemKv.Key] = deficit;
craftSources[itemKv.Key] = "[" + tagName + "]";
}
}
}
}
void MergeLoadoutCraftNeeds() {
foreach (var kv in loadoutCraftNeeds) {
double existing = craftTargets.ContainsKey(kv.Key) ? craftTargets[kv.Key] : 0;
if (kv.Value > existing)
craftTargets[kv.Key] = kv.Value;
}
}
void ReadStasisTargets() {
stasisTargets.Clear();
foreach (var panel in allPanels) {
if (!panel.CustomName.Contains("[INV:Status")) continue;
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data)) continue;
foreach (var rawLine in data.Split('\n')) {
string line = rawLine.Trim();
if (line.StartsWith("[") || line.StartsWith("#") || !line.Contains("=")) continue;
var parts = line.Split(new char[]{'='}, 2);
if (parts.Length != 2) continue;
string s = parts[0].Trim();
if (s == "Font" || s == "FontSize" || s == "Padding") continue;
double amt = ParseSmeltAmount(parts[1].Trim());
string id = s; string cn = null; int ci = s.IndexOf(':');
if (ci > 0) { id = s.Substring(0, ci).Trim(); cn = s.Substring(ci+1).Trim(); }
if (amt <= 0) continue;
bool found = false;
foreach (var kv in itemDb) {
if (kv.Key.SubtypeId == id || kv.Value.displayName == id) {
stasisTargets[kv.Key] = amt;
if (!string.IsNullOrEmpty(cn))
customNames[kv.Key.TypeId + "/" + kv.Key.SubtypeId] = cn;
found = true; break;
}
}
if (!found) {
if (!string.IsNullOrEmpty(cn)) customNames[id] = cn;
string[] tt = { OB+T_COMP, OB+T_AMMO, OB+T_GUN, OB+T_FOOD, OB+T_OXYB, OB+T_GASB, OB+T_PHYS };
foreach (var tid in tt) { var c = new MyItemType(tid, id); if (itemDb.ContainsKey(c)) { stasisTargets[c] = amt; break; } }
}
}
}
}
IEnumerator<bool> RunAutoDisassembly() {
if (assemblers.Count == 0) yield break;
// Build disShareMap: bpSubtypeId -> amount each dis-assembler should hold
var disShareMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
var disBpIdMap = new Dictionary<string, MyDefinitionId>(StringComparer.OrdinalIgnoreCase);
bool anyExcess = false;
foreach (var kv in stasisTargets) {
double current = GetGlobalCount(kv.Key);
double excess = current - kv.Value;
MyDefinitionId bpId;
if (!TryGetBlueprint(kv.Key, out bpId)) continue;
string bpKey = bpId.SubtypeId.ToString();
disBpIdMap[bpKey] = bpId;
if (excess >= 1) anyExcess = true;
// Store the raw excess amount — we'll divide by pool size once we know it
disShareMap[bpKey] = excess >= 1 ? (int)excess : 0;
}
// Determine how many assemblers crafting needs
bool craftingActive = false;
foreach (var kv in craftTargets) {
if (GetGlobalCount(kv.Key) < kv.Value) { craftingActive = true; break; }
}
// Build disassembly pool
var disPool = new List<IMyAssembler>();
foreach (var asm in assemblers) {
if (!asm.IsWorking || asm.CustomName.Contains(TAG_MANUAL)) continue;
if (asm.Mode == MyAssemblerMode.Disassembly) disPool.Add(asm);
}
if (!craftingActive) {
// No crafting — recruit all idle assembly-mode assemblers
foreach (var asm in assemblers) {
if (!asm.IsWorking || asm.CustomName.Contains(TAG_MANUAL) || asm.Mode == MyAssemblerMode.Disassembly) continue;
disPool.Add(asm);
}
} else if (disPool.Count == 0) {
// Crafting active — dedicate the last assembler in the list to disassembly
for (int i = assemblers.Count - 1; i >= 0; i--) {
var asm = assemblers[i];
if (!asm.IsWorking || asm.CustomName.Contains(TAG_MANUAL)) continue;
disPool.Add(asm);
break;
}
}
// If nothing to disassemble, release disassembly-mode assemblers back to assembly
if (!anyExcess) {
foreach (var asm in disPool) {
if (asm.Mode == MyAssemblerMode.Disassembly && asm.IsQueueEmpty)
asm.Mode = MyAssemblerMode.Assembly;
}
yield break;
}
if (disPool.Count == 0) yield break;
// Divide totals by pool size to get per-assembler shares
var finalShare = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
foreach (var kv in disShareMap) {
if (kv.Value <= 0) { finalShare[kv.Key] = 0; continue; }
int share = (int)Math.Ceiling((double)kv.Value / disPool.Count);
finalShare[kv.Key] = share;
}
// Apply to each assembler in the pool
foreach (var asm in disPool) {
asm.Mode = MyAssemblerMode.Disassembly;
var aq = new List<MyProductionItem>();
asm.GetQueue(aq);
// Check if already correct
bool ok = true;
var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach (var qi in aq) {
string k = qi.BlueprintId.SubtypeId.ToString();
int want; finalShare.TryGetValue(k, out want);
if ((int)qi.Amount != want || want == 0) { ok = false; break; }
seen.Add(k);
}
if (ok) {
foreach (var kv in finalShare) {
if (kv.Value > 0 && !seen.Contains(kv.Key)) { ok = false; break; }
}
}
if (!ok) {
asm.ClearQueue();
foreach (var kv in finalShare) {
if (kv.Value <= 0) continue;
MyDefinitionId bpId; if (!disBpIdMap.TryGetValue(kv.Key, out bpId)) continue;
asm.AddQueueItem(bpId, (MyFixedPoint)kv.Value);
}
}
yield return true;
}
}
bool TryGetBlueprint(MyItemType itemType, out MyDefinitionId blueprintId) {
blueprintId = default(MyDefinitionId);
// Check blueprintMap first — if it's already cached just return it
if (blueprintMap.ContainsKey(itemType.SubtypeId)) {
blueprintId = blueprintMap[itemType.SubtypeId];
return true;
}
// Not cached yet — probe assemblers that are in Assembly mode (CanUseBlueprint
// returns false in Disassembly mode, so we must use an Assembly-mode assembler)
var probeAsm = assemblers.Find(a => a.IsWorking && a.Mode == MyAssemblerMode.Assembly);
if (probeAsm == null) {
// No assembly-mode assembler available right now — fall back to any working assembler
// by temporarily checking with the map only (accept any parseable ID)
string[] suffixesFb = { "", "Component", "Magazine" };
string[] prefixesFb = { "", "Position0010_", "Position0020_", "Position0030_" };
foreach (var pre in prefixesFb) {
foreach (var suf in suffixesFb) {
MyDefinitionId cand;
if (MyDefinitionId.TryParse(BP_PREFIX + pre + itemType.SubtypeId + suf, out cand)) {
blueprintId = cand;
blueprintMap[itemType.SubtypeId] = cand;
return true;
}
}
}
return false;
}
string[] suffixes = { "", "Component", "Magazine" };
string[] prefixes = { "", "Position0010_", "Position0020_", "Position0030_" };
foreach (var prefix in prefixes) {
foreach (var suffix in suffixes) {
string bpName = BP_PREFIX + prefix + itemType.SubtypeId + suffix;
MyDefinitionId candidate;
if (!MyDefinitionId.TryParse(bpName, out candidate)) continue;
if (probeAsm.CanUseBlueprint(candidate)) {
blueprintId = candidate;
blueprintMap[itemType.SubtypeId] = candidate;
Log("Learned blueprint: " + itemType.SubtypeId);
return true;
}
}
}
return false;
}
void CancelQueuedBlueprint(List<IMyAssembler> asmList, MyDefinitionId blueprint, int amount) {
int toCancel = amount;
foreach (var asm in asmList) {
if (toCancel <= 0) break;
var queue = new List<MyProductionItem>();
asm.GetQueue(queue);
for (int i = queue.Count - 1; i >= 0; i--) {
if (toCancel <= 0) break;
if (queue[i].BlueprintId != blueprint) continue;
int qAmt = (int)queue[i].Amount;
int remove = Math.Min(qAmt, toCancel);
asm.RemoveQueueItem(i, (MyFixedPoint)remove);
toCancel -= remove;
}
}
}
void LearnFromAssemblers() {
foreach (var asm in learningAssemblers) {
if (!asm.IsWorking) continue;
long asmId = asm.EntityId;
var queue = new List<MyProductionItem>();
asm.GetQueue(queue);
MyDefinitionId currentBp = default(MyDefinitionId);
if (queue.Count > 0) {
currentBp = queue[0].BlueprintId;
lastLearnBp[asmId] = currentBp;
} else if (lastLearnBp.ContainsKey(asmId)) {
currentBp = lastLearnBp[asmId];
} else {
continue;
}
var outInv = asm.GetInventory(1);
var items = new List<MyInventoryItem>();
outInv.GetItems(items);
foreach (var item in items) {
string sub = item.Type.SubtypeId;
if (blueprintMap.ContainsKey(sub)) continue;
blueprintMap[sub] = currentBp;
Log("Learned: " + sub + " <- " + currentBp.SubtypeName);
if (!itemDb.ContainsKey(item.Type)) {
itemDb[item.Type] = new ItemInfo(item.Type);
}
AddLearnedToCraftPanel(sub);
}
}
}
void AddLearnedToCraftPanel(string subtypeId) {
string displayName = subtypeId;
foreach (var kv in itemDb) {
if (kv.Key.SubtypeId == subtypeId) { displayName = kv.Value.displayName; break; }
}
foreach (var panel in allPanels) {
if (GetLcdFilter(panel.CustomName, "[INV:Craft") == null) continue;
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data)) continue;
if (data.Contains(subtypeId + ":") || data.Contains(subtypeId + "=")) continue;
panel.CustomData = data.TrimEnd() + "\n" + subtypeId + ":" + displayName + "=0";
}
}
string ExtractLoadoutName(string name) {
int idx = name.IndexOf("[Loadout:");
if (idx < 0) return null;
int end = name.IndexOf(']', idx + 9);
if (end <= idx + 9) return null;
return name.Substring(idx + 9, end - idx - 9);
}
void ReadLoadoutProfiles() {
loadoutProfiles.Clear();
profileUraniumOverride.Clear();
profileIceOverride.Clear();
foreach (var panel in allPanels) {
string name = panel.CustomName;
if (!name.Contains(LCD_LOADOUT)) continue;
int start = name.IndexOf(LCD_LOADOUT) + LCD_LOADOUT.Length;
int end = name.IndexOf(']', start);
if (end < 0) continue;
string profileName = name.Substring(start, end - start);
string data = panel.CustomData;
if (string.IsNullOrWhiteSpace(data)) continue;
var profile = new Dictionary<MyItemType, double>();
foreach (var rawLine in data.Split('\n')) {
string line = rawLine.Trim();
if (line.StartsWith("[") || line.StartsWith("#") || !line.Contains("=")) continue;
var parts = line.Split(new char[]{'='}, 2);
if (parts.Length != 2) continue;
string itemName = parts[0].Trim();
if (itemName == "Font" || itemName == "FontSize" || itemName == "Padding") continue;
double amount;
if (!double.TryParse(parts[1].Trim(), out amount)) continue;
if (itemName == "ReactorUranium") { if (amount >= 0) profileUraniumOverride[profileName] = amount; continue; }
if (itemName == "GeneratorIce") { if (amount >= 0) profileIceOverride[profileName] = amount; continue; }
if (amount <= 0) continue;
bool found = false;
foreach (var kv in itemDb) {
if (kv.Value.displayName == itemName || kv.Key.SubtypeId == itemName) {
profile[kv.Key] = amount;
found = true;
break;
}
}
if (!found && itemName.Contains("/")) {
int slash = itemName.IndexOf('/');
var itemType = new MyItemType(itemName.Substring(0, slash), itemName.Substring(slash + 1));
profile[itemType] = amount;
}
}
loadoutProfiles[profileName] = profile;
}
}
IEnumerator<bool> ProcessLoadouts() {
loadoutCraftNeeds.Clear();
var toRemove = new List<long>();
foreach (var kv in loadoutPhase) {
bool found = false;
foreach (var c in connectors) {
if (c.EntityId == kv.Key && c.Status == MyShipConnectorStatus.Connected) { found = true; break; }
}
if (!found) toRemove.Add(kv.Key);
}
foreach (var id in toRemove) loadoutPhase.Remove(id);
foreach (var conn in connectors) {
if (conn.Status != MyShipConnectorStatus.Connected || conn.OtherConnector == null) continue;
string loadoutName = ExtractLoadoutName(conn.CustomName);
if (loadoutName == null)
loadoutName = ExtractLoadoutName(conn.OtherConnector.CustomName);
if (loadoutName == null) continue;
long cid = conn.EntityId;
bool isTagMode = loadoutName.Equals("Tags", StringComparison.OrdinalIgnoreCase);
Dictionary<MyItemType, double> profile = null;
if (!isTagMode) {
if (!loadoutProfiles.TryGetValue(loadoutName, out profile)) {
Warn("Loadout '" + loadoutName + "' not found");
continue;
}
}
int phase;
if (!loadoutPhase.TryGetValue(cid, out phase)) phase = LOAD_UNLOAD;
if (phase >= LOAD_DONE) continue;
var otherGrid = conn.OtherConnector.CubeGrid;
if (phase == LOAD_UNLOAD) {
var shipBlocks = new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(shipBlocks,
b => b.HasInventory && b.CubeGrid == otherGrid);
yield return true;
int ops = 0;
foreach (var shipBlock in shipBlocks) {
if (shipBlock is IMyReactor || shipBlock is IMyGasGenerator) continue;
for (int invIdx = 0; invIdx < shipBlock.InventoryCount; invIdx++) {
var srcInv = shipBlock.GetInventory(invIdx);
var items = new List<MyInventoryItem>();
srcInv.GetItems(items);
for (int i = items.Count - 1; i >= 0; i--) {
var itemType = (MyItemType)items[i].Type;
string destTag = GetDestinationTag(itemType);
IMyTerminalBlock dest = null;
if (destTag != null && taggedContainers.ContainsKey(destTag)) {
foreach (var d in taggedContainers[destTag]) {
if (HasSpace(d) && srcInv.CanTransferItemTo(d.GetInventory(0), itemType)) { dest = d; break; }
}
}
if (dest == null) {
foreach (var d in overflowContainers) {
if (HasSpace(d) && srcInv.CanTransferItemTo(d.GetInventory(0), itemType)) { dest = d; break; }
}
}
if (dest != null) srcInv.TransferItemTo(dest.GetInventory(0), i, null, true);
if (++ops % 3 == 0) yield return true;
}
}
}
Log("Loadout UNLOAD: " + ops + " items from " + conn.CustomName);
loadoutPhase[cid] = LOAD_STOCK;
yield return true;
continue;
}
if (phase == LOAD_STOCK) {
var shipBlocks = new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(shipBlocks,
b => b.HasInventory && b.CubeGrid == otherGrid);
yield return true;
if (isTagMode) {
int ops = 0;
foreach (var shipBlock in shipBlocks) {
string blockName = shipBlock.CustomName;
foreach (var tagKv in customTags) {
string bracket = "[" + tagKv.Key + "]";
if (!blockName.Contains(bracket)) continue;
foreach (var itemKv in tagKv.Value) {
double quota = itemKv.Value;
double current = GetItemAmount(itemKv.Key, shipBlock);
double needed = quota > 0 ? quota - current : 100000;
if (needed <= 0.5) continue;
var source = FindItemSource(itemKv.Key);
if (source != null) {
TransferItems(itemKv.Key, source, 0, shipBlock, 0, needed);
Log("Loadout: " + FormatNumber(needed) + "x " + GetDisplayName(itemKv.Key) + " -> " + shipBlock.CustomName);
}
if (++ops % 2 == 0) yield return true;
}
}
}
Log("Loadout TAG-SORT -> " + conn.CustomName);
loadoutPhase[cid]=LOAD_DONE;
} else {
var shipCargo = new List<IMyTerminalBlock>();
foreach (var sb in shipBlocks) {
if (sb is IMyCargoContainer) shipCargo.Add(sb);
}
bool ok=true;
foreach (var itemKv in profile) {
double current = 0;
foreach (var sb in shipBlocks)
current += GetItemAmount(itemKv.Key, sb);
double needed = itemKv.Value - current;
if (needed <= 0) continue;
double sent = 0;
foreach (var cargo in shipCargo) {
if (!HasSpace(cargo) || sent >= needed) continue;
var src = FindItemSource(itemKv.Key);
while (src != null && sent < needed) {
double m = TransferItems(itemKv.Key, src, 0, cargo, 0, needed - sent);
if (m <= 0) break; sent += m;
src = FindItemSource(itemKv.Key);
}
}
if (sent > 0) Log("Loadout: " + FormatNumber(sent) + "x " + GetDisplayName(itemKv.Key) + " -> ship");
double sn = needed - sent;
if (sn > 0.5) {
ok = false;
if (loadoutCraftNeeds.ContainsKey(itemKv.Key))
loadoutCraftNeeds[itemKv.Key] += sn;
else
loadoutCraftNeeds[itemKv.Key] = sn;
}
yield return true;
}
if (ok) {
Log("Loadout STOCK: '" + loadoutName + "' -> " + conn.CustomName);
loadoutPhase[cid] = LOAD_DONE;
} else {
Log("Loadout WAIT: '" + loadoutName + "' " + conn.CustomName);
}
}
yield return true;
}
}
}
IEnumerator<bool> FuelConnectedShips() {
var toRemove = new List<long>();
foreach (var kv in fuelPhase) {
bool found = false;
foreach (var c in connectors) {
if (c.EntityId == kv.Key && c.Status == MyShipConnectorStatus.Connected) { found = true; break; }
}
if (!found) toRemove.Add(kv.Key);
}
foreach (var id in toRemove) fuelPhase.Remove(id);
foreach (var conn in connectors) {
if (!conn.CustomName.Contains(TAG_FUEL)) continue;
long cid = conn.EntityId;
if (conn.Status != MyShipConnectorStatus.Connected || conn.OtherConnector == null) {
fuelPhase.Remove(cid);
continue;
}
int phase;
if (!fuelPhase.TryGetValue(cid, out phase)) phase = FUEL_DOCK;
if (phase >= FUEL_DONE) continue;
var otherGrid = conn.OtherConnector.CubeGrid;
bool noSort = conn.CustomName.Contains(TAG_NO_SORT) || conn.OtherConnector.CustomName.Contains(TAG_NO_SORT);
bool noManage = conn.CustomName.Contains(TAG_NO_MANAGE) || conn.OtherConnector.CustomName.Contains(TAG_NO_MANAGE);
bool skipCargo = noSort || noManage;
if (phase == FUEL_DOCK) {
var otherBatteries = new List<IMyBatteryBlock>();
GridTerminalSystem.GetBlocksOfType(otherBatteries, b => b.CubeGrid == otherGrid);
foreach (var bat in otherBatteries) bat.ChargeMode = ChargeMode.Recharge;
var otherTanks = new List<IMyGasTank>();
GridTerminalSystem.GetBlocksOfType(otherTanks, b => b.CubeGrid == otherGrid);
foreach (var tank in otherTanks) tank.Stockpile = true;
Log("Fuel DOCK: " + conn.CustomName + " (" + otherBatteries.Count + " bats, " + otherTanks.Count + " tanks)" + (skipCargo ? " [fuel only]" : ""));
fuelPhase[cid] = skipCargo ? FUEL_WAIT : FUEL_UNLOAD;
yield return true;
continue;
}
if (phase == FUEL_UNLOAD) {
bool hasLoadout = ExtractLoadoutName(conn.CustomName) != null;
if (!hasLoadout && conn.OtherConnector != null)
hasLoadout = ExtractLoadoutName(conn.OtherConnector.CustomName) != null;
if (hasLoadout) {
Log("Fuel UNLOAD: skipped (loadout handles) " + conn.CustomName);
fuelPhase[cid] = FUEL_STOCK;
yield return true;
continue;
}
var shipBlocks = new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(shipBlocks,
b => b.HasInventory && b.CubeGrid == otherGrid);
yield return true;
int ops = 0;
foreach (var shipBlock in shipBlocks) {
if (shipBlock is IMyReactor || shipBlock is IMyGasGenerator) continue;
for (int invIdx = 0; invIdx < shipBlock.InventoryCount; invIdx++) {
var srcInv = shipBlock.GetInventory(invIdx);
var items = new List<MyInventoryItem>();
srcInv.GetItems(items);
for (int i = items.Count - 1; i >= 0; i--) {
var itemType = (MyItemType)items[i].Type;
string destTag = GetDestinationTag(itemType);
IMyTerminalBlock dest = null;
if (destTag != null && taggedContainers.ContainsKey(destTag)) {
foreach (var d in taggedContainers[destTag]) {
if (HasSpace(d) && srcInv.CanTransferItemTo(d.GetInventory(0), itemType)) { dest = d; break; }
}
}
if (dest == null) {
foreach (var d in overflowContainers) {
if (HasSpace(d) && srcInv.CanTransferItemTo(d.GetInventory(0), itemType)) { dest = d; break; }
}
}
if (dest != null) srcInv.TransferItemTo(dest.GetInventory(0), i, null, true);
if (++ops % 3 == 0) yield return true;
}
}
}
Log("Fuel UNLOAD: " + ops + " items from " + conn.CustomName);
fuelPhase[cid] = FUEL_STOCK;
yield return true;
continue;
}
if (phase == FUEL_STOCK) {
MyItemType uraniumType = MyItemType.Parse(OB + T_INGOT + "/Uranium");
MyItemType iceType = MyItemType.Parse(OB + T_ORE + "/Ice");
string fuelProfileName = ExtractLoadoutName(conn.CustomName);
if (fuelProfileName == null && conn.OtherConnector != null)
fuelProfileName = ExtractLoadoutName(conn.OtherConnector.CustomName);
bool hasUranOverride = false, hasIceOverride = false;
double uranOverride = 0, iceOverride = 0;
if (fuelProfileName != null) {
hasUranOverride = profileUraniumOverride.TryGetValue(fuelProfileName, out uranOverride);
hasIceOverride = profileIceOverride.TryGetValue(fuelProfileName, out iceOverride);
}
var shipReactors = new List<IMyReactor>();
GridTerminalSystem.GetBlocksOfType(shipReactors, b => b.CubeGrid == otherGrid);
foreach (var reactor in shipReactors) {
double target = hasUranOverride ? uranOverride
: (reactor.CubeGrid.GridSize < 1f ? uraniumPerSmallReactor : uraniumPerLargeReactor);
double current = GetItemAmount(uraniumType, reactor);
if (current < target - 0.05) {
var source = FindItemSource(uraniumType);
if (source != null) TransferItems(uraniumType, source, 0, reactor, 0, target - current);
} else if (current > target + 0.05) {
var dest = FindEmptyContainer(TAG_INGOT);
if (dest != null) TransferItems(uraniumType, reactor, 0, dest, 0, current - target);
}
}
yield return true;
var shipGens = new List<IMyGasGenerator>();
GridTerminalSystem.GetBlocksOfType(shipGens, b => b.CubeGrid == otherGrid);
foreach (var gen in shipGens) {
double target = hasIceOverride ? iceOverride
: (gen.CubeGrid.GridSize < 1f ? icePerSmallGenerator : icePerLargeGenerator);
double current = GetItemAmount(iceType, gen);
if (current < target * 0.8) {
var source = FindItemSource(iceType);
if (source != null) TransferItems(iceType, source, 0, gen, 0, target - current);
} else if (current > target * 1.1) {
var dest = FindEmptyContainer(TAG_ORE);
if (dest != null) TransferItems(iceType, gen, 0, dest, 0, current - target);
}
}
yield return true;
Log("Fuel STOCK: " + shipReactors.Count + " reactors, " + shipGens.Count + " gens on " + conn.CustomName);
fuelPhase[cid] = FUEL_WAIT;
yield return true;
continue;
}
if (phase == FUEL_WAIT) {
var otherBatteries = new List<IMyBatteryBlock>();
GridTerminalSystem.GetBlocksOfType(otherBatteries, b => b.CubeGrid == otherGrid);
float batStored = 0, batMax = 0;
foreach (var bat in otherBatteries) { batStored += bat.CurrentStoredPower; batMax += bat.MaxStoredPower; }
double batPct = batMax > 0 ? (batStored / batMax) * 100 : 100;
var otherTanks = new List<IMyGasTank>();
GridTerminalSystem.GetBlocksOfType(otherTanks, b => b.CubeGrid == otherGrid);
double tankTotal = 0; int tankCount = 0;
foreach (var tank in otherTanks) { tankTotal += tank.FilledRatio; tankCount++; }
double tankPct = tankCount > 0 ? (tankTotal / tankCount) * 100 : 100;
if (batPct >= 98 && tankPct >= 98) {
Log("Fuel READY: " + conn.CustomName + " bat=" + batPct.ToString("0.0") + "% tank=" + tankPct.ToString("0.0") + "%");
fuelPhase[cid] = FUEL_RELEASE;
}
yield return true;
continue;
}
if (phase == FUEL_RELEASE) {
var otherBatteries = new List<IMyBatteryBlock>();
GridTerminalSystem.GetBlocksOfType(otherBatteries, b => b.CubeGrid == otherGrid);
foreach (var bat in otherBatteries) bat.ChargeMode = ChargeMode.Auto;
var otherTanks = new List<IMyGasTank>();
GridTerminalSystem.GetBlocksOfType(otherTanks, b => b.CubeGrid == otherGrid);
foreach (var tank in otherTanks) tank.Stockpile = false;
conn.Disconnect();
Log("Fuel RELEASE: " + conn.CustomName + " disconnected");
fuelPhase[cid] = FUEL_DONE;
yield return true;
}
}
}

void UpdateFillLevels() {
foreach (var kv in taggedContainers) {
foreach (var block in kv.Value) {
var inv = block.GetInventory(0);
float pct = (float)inv.CurrentVolume / (float)inv.MaxVolume * 100f;
string pctStr = "(" + pct.ToString("0.0") + "%)";
string name = block.CustomName;
int parenIdx = name.LastIndexOf('(');
if (parenIdx > 0 && name.EndsWith("%)")) {
name = name.Substring(0, parenIdx).TrimEnd();
}
string newName = name + " " + pctStr;
if (block.CustomName != newName)
block.CustomName = newName;
}
}
}
void Log(string msg) {
string entry = DateTime.UtcNow.ToString("HH:mm:ss") + " " + msg;
if (actionLog.Count > 0 && actionLog[0].Substring(9) == msg) return;
actionLog.Insert(0, entry);
if (actionLog.Count > 100) actionLog.RemoveAt(100);
}
void Warn(string msg) {
warnings.Add(msg);
}
string GetDisplayName(MyItemType type) {
string custom;
if (customNames.TryGetValue(type.TypeId + "/" + type.SubtypeId, out custom)) return custom;
ItemInfo info;
if (itemDb.TryGetValue(type, out info)) return info.displayName;
if (customNames.TryGetValue(type.SubtypeId, out custom)) return custom;
string sub = type.SubtypeId;
if (type.TypeId.Contains("Ingot")) return sub == "Stone" ? "Gravel" : sub + " Ingot";
if (type.TypeId.Contains("Ore") && !sub.Contains("Scrap")) return sub + " Ore";
return sub;
}
string FormatNumber(double val) {
double abs = Math.Abs(val);
if (abs >= 1000000000) return (val / 1000000000).ToString("0.#") + "G";
if (abs >= 1000000) return (val / 1000000).ToString("0.#") + "M";
if (abs >= 100000) return (val / 1000).ToString("0.#") + "K";
if (abs >= 10000) return (val / 1000).ToString("0.#") + "K";
return val.ToString("#,0");
}
string GetLcdFilter(string name, string tagPrefix) {
int idx = name.IndexOf(tagPrefix);
if (idx < 0) return null;
int after = idx + tagPrefix.Length;
if (after >= name.Length || name[after] == ']') return "";
if (name[after] == ':') {
int end = name.IndexOf(']', after);
if (end > after) return name.Substring(after + 1, end - after - 1).Trim();
}
return "";
}
IMyTerminalBlock FindItemSource(MyItemType type, IMyTerminalBlock exclude = null) {
foreach (var kv in taggedContainers) {
foreach (var block in kv.Value) {
if (block == exclude) continue;
if (block.GetInventory(0).FindItem(type) != null) return block;
}
}
foreach (var block in inventoryBlocks) {
if (block == exclude) continue;
if (block is IMyReactor || block is IMyGasGenerator) continue;
for (int i = 0; i < block.InventoryCount; i++) {
if (block.GetInventory(i).FindItem(type) != null) return block;
}
}
return null;
}
IMyTerminalBlock FindEmptyContainer(string preferredTag = null) {
if (preferredTag != null && taggedContainers.ContainsKey(preferredTag)) {
foreach (var block in taggedContainers[preferredTag]) {
if (HasSpace(block)) return block;
}
}
foreach (var block in overflowContainers) {
if (HasSpace(block)) return block;
}
return null;
}
bool HasSpace(IMyTerminalBlock block) {
var inv = block.GetInventory(0);
return (float)inv.CurrentVolume < (float)inv.MaxVolume * 0.98f;
}
double TransferItems(MyItemType type, IMyTerminalBlock from, int fromInv,
IMyTerminalBlock to, int toInv, double amount) {
var srcInv = from.GetInventory(fromInv);
var dstInv = to.GetInventory(toInv);
if (!srcInv.CanTransferItemTo(dstInv, type)) return 0;
var items = new List<MyInventoryItem>();
srcInv.GetItems(items);
double transferred = 0;
for (int i = items.Count - 1; i >= 0; i--) {
if ((MyItemType)items[i].Type != type) continue;
double available = (double)items[i].Amount;
double toMove = Math.Min(available, amount - transferred);
if (type.TypeId.Contains(T_INGOT) || type.TypeId.Contains(T_ORE))
toMove = Math.Ceiling(toMove * 100) / 100;
else
toMove = Math.Ceiling(toMove);
if (srcInv.TransferItemTo(dstInv, i, null, true, (VRage.MyFixedPoint)toMove)) {
transferred += toMove;
}
if (transferred >= amount) break;
}
return transferred;
}
bool FindAndInsertBottle(MyItemType bottleType, IMyInventory destInv) {
if (taggedContainers.ContainsKey(TAG_BOTTLE)) {
foreach (var block in taggedContainers[TAG_BOTTLE]) {
var srcInv = block.GetInventory(0);
var items = new List<MyInventoryItem>();
srcInv.GetItems(items);
for (int i = items.Count - 1; i >= 0; i--) {
if ((MyItemType)items[i].Type != bottleType) continue;
return srcInv.TransferItemTo(destInv, i, null, true, 1);
}
}
}
foreach (var block in inventoryBlocks) {
if (block is IMyGasGenerator) continue;
for (int inv = 0; inv < block.InventoryCount; inv++) {
var srcInv = block.GetInventory(inv);
var items = new List<MyInventoryItem>();
srcInv.GetItems(items);
for (int i = items.Count - 1; i >= 0; i--) {
if ((MyItemType)items[i].Type != bottleType) continue;
return srcInv.TransferItemTo(destInv, i, null, true, 1);
}
}
}
return false;
}
int CompareOresByPriority(KeyValuePair<MyItemType, double> a, KeyValuePair<MyItemType, double> b) {
int priA = 0, priB = 0;
MyItemType ingA = new MyItemType(OB + T_INGOT, a.Key.SubtypeId);
MyItemType ingB = new MyItemType(OB + T_INGOT, b.Key.SubtypeId);
ingotPriorities.TryGetValue(ingA, out priA);
ingotPriorities.TryGetValue(ingB, out priB);
if (priA != priB) return priB.CompareTo(priA);
return b.Value.CompareTo(a.Value);
}
int CompareCraftEntriesByPriority(KeyValuePair<MyItemType, double> a, KeyValuePair<MyItemType, double> b) {
int pa = 0, pb = 0;
craftPriorities.TryGetValue(a.Key, out pa);
craftPriorities.TryGetValue(b.Key, out pb);
if (pb != pa) return pb.CompareTo(pa);
double ta = craftTargets.ContainsKey(a.Key) ? craftTargets[a.Key] : 1;
double tb = craftTargets.ContainsKey(b.Key) ? craftTargets[b.Key] : 1;
double ra = ta > 0 ? a.Value / ta : 0;
double rb = tb > 0 ? b.Value / tb : 0;
return rb.CompareTo(ra);
}
class ItemInfo {
public MyItemType type;
public string displayName;
public string category;
public ItemInfo(MyItemType type) {
this.type = type;
this.category = "Other";
string sub = type.SubtypeId;
string name;
if (DISPLAY_NAMES.TryGetValue(sub, out name)) displayName = name;
else if (type.TypeId.Contains("Ore") && !sub.Contains("Scrap")) displayName = sub + " Ore";
else if (type.TypeId.Contains("Ingot")) displayName = sub == "Stone" ? "Gravel" : sub + " Ingot";
else {
var csb = new System.Text.StringBuilder();
for (int i = 0; i < sub.Length; i++) {
if (i > 0 && char.IsUpper(sub[i]) && i < sub.Length - 1
&& (!char.IsUpper(sub[i - 1]) || !char.IsUpper(sub[Math.Min(i + 1, sub.Length - 1)])))
csb.Append(' ');
csb.Append(sub[i]);
}
displayName = csb.ToString();
}
string tid = type.TypeId;
if (tid.Contains("Ore")) category = "Ores";
else if (tid.Contains("Ingot")) category = "Ingots";
else if (tid.Contains("Component")) category = "Components";
else if (tid.Contains("AmmoMagazine")) category = "Ammo";
else if (tid.Contains("OxygenContainer") || tid.Contains("GasContainer")) category = "Bottles";
else if (tid.Contains("PhysicalGunObject")) category = "Tools";
else if (tid.Contains("ConsumableItem")) category = "Food";
else if (tid.Contains("Datapad")) category = "Misc";
}
}

