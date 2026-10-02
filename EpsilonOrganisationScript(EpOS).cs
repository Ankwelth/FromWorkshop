/*
 * R e a d m e
 * -----------
 */
class Config{
    // Configure the Script here


    // How fast should the SCript run
    public UpdateFrequency UpdateSpeed = UpdateFrequency.Update10;
    // Should docked Grids be scanned to
    public bool ScanConnectedGrids = true;
    // After how many Cycles should the Grid be scanned for changes (ie detectiong new Blocks / connected Grids)
    public int ScanInterval = 30;


    // --- GAS ---
    // How many empty O2 Gens should be displayed
    public int EmptyO2GensListLength = 6;
    // DIsplay the Amount of Ice available
    public bool ShowIce = true;
    // Tries to guess the max output of Oxygenfarms. Good for modded. Not yet implement.
    public bool detectMaxGasOutput = true;
    // A List of "unusual" Gasproduces, that have dynamic output
    public Dictionary<string, float> GasProducers = new Dictionary<string, float>()
    {
        {"LargeBlockOxygenFarm", 1.8f },
    };
    // How much Gas Processing should be done. Set to 0 to disable
    public int GasTicks = 1;

    // --- POWER ---
    // How many empty Reactors should be displayed
    public int EmptyReactorList = 6;
    // Show available Reactor Fuel
    public bool showFuel = true;
    // Tries to guess the max output of Solar Panels and Windturbines. Good for modded. Not yet implement.
    public bool detectMaxPowerOutput = true;
    // A List of "unusual" Powergenerators, that have dynamic output
    public Dictionary<string, float> PowerProducers = new Dictionary<string, float>()
    {
        {"LargeBlockWindTurbine", 0.4f },
        {"SmallBlockSolarPanel", 0.04f },
        {"LargeBlockSolarPanel", 0.16f },
    };
    // How much Power Processing should be done. Set to 0 to disable
    public int PowerTicks = 1;

    // --- ITEMS ---
    // Value from world Settings
    public int AssemlberEfficiency = 2;
    // Should the Autocrafting Screen of IsyInventory be used for Item-Quotas. Otherwise use the Custom-Data of Programmable Block to set Quotas and Recompile
    public bool UseIsy = true;
    // The Name of Isys Crafting Screen as defined in the scripts config
    public string IsyCraftingScreenName = "Autocrafting";
    // Should the Quota of Ingots be calculated based on Component-Quotas
    public bool CalculateIngotQuotas = true;
    // Multiplier for the Calculated Ingot Quota. Otherwise use the Custom-Data of Programmable Block to set Quotas and Recompile
    public int IngotCalcStockpile = 5;
    // Hide empty Items from Display
    public bool HideEmpty = true;
    // How much Uranium Ingots should be added to the Quota for Power
    public int UraniumPowerBonus = 1000;
    // Show incoing Ingots with more Detail
    public bool DetailedIngotOre = true;
    // Tags for Cargocontainers containing the respective Items
    public string TagComponent = "Components";
    public string TagIngot = "Ingots";
    public string TagOre = "Ore";
    public string TagAmmo = "Ammo";
    public string TagTool = "Tools";
    public string TagBottle = "Bottles";
    // How much Item Processing should be done. Set to 0 to disable
    public int ItemTicks = 4;

    // --- SECURITY ---
    // How often should Blocks be scanned for damage
    public int ScanDamageInterval = 10;
    // How many damaged BLocks should be displayed
    public int DamagedBlocksListLength = 10;
    // How many Empty Turrets should be displayd
    public int EmptyTurretsListLength = 5;
    // Weapons which Names contain any of these String will never show up in Empty Turrets. Good for Lasers.
    public List<string> NoAmmoWeapons = new List<string>() { "Laser" };
    // How much Security Processing should be done. Set to 0 to disable
    public int SecurityTicks = 1;


    // --- DISPLAYS ---
    public string DisplayGas = "EGAS";              // Display for the GAS Status
    public string DisplayPower = "EPOWER";          // Display for the POWER Status
    public string DisplayCargo = "ECARGO";          // Display for the Cargo Overview
    public string DisplayProduction = "EPROD";      // NOT YET USED
    public string DisplayIngots = "EINGOT";         // Display for Ingots (and incoming Ore)
    public string DisplayComponents = "ECOMP";      // Display for Component Overview
    public string DisplayAmmo = "EAMMO";            // Display for Ammo Overview
    public string DisplayTools = "ETOOLS";          // Display for Tools & Bottles Overview
    public string DisplaySecurity = "ESECURITY";    // Display for Security Overview (Only Block Inregrity so far)


    // Item Storage

public ItemStorage Items=new ItemStorage();protected IMyTerminalBlock Block;public MyIni Ini=new MyIni();public
Dictionary<string,SubRoutine>AvailableRoutines=new Dictionary<string,SubRoutine>();public Config(IMyTerminalBlock
programmableBlock){Block=programmableBlock;LoadIni();}public void LoadIni(){if(Block.CustomData==null||!Ini.TryParse(Block.CustomData)||!
ValidateInit()){SaveIni();}foreach(MyIniKey key in GetSectionKeys("PowerProducers")){float value=(float)Ini.Get(key.Section,key.Name
).ToDouble();if(PowerProducers.ContainsKey(key.Name))PowerProducers[key.Name]=value;else PowerProducers.Add(key.Name,
value);}foreach(MyIniKey key in GetSectionKeys("GasProducers")){float value=(float)Ini.Get(key.Section,key.Name).ToDouble();
if(GasProducers.ContainsKey(key.Name))GasProducers[key.Name]=value;else GasProducers.Add(key.Name,value);}foreach(MyIniKey
key in GetSectionKeys("Quotas")){Items.Get(key.Name).TargetAmount=Ini.Get(key.Section,key.Name).ToInt32();}}public List<
MyIniKey>GetSectionKeys(string section){List<MyIniKey>result=new List<MyIniKey>();Ini.GetKeys(section,result);return result;}
private bool ValidateInit(){return Ini!=null&&Ini.ContainsSection("PowerProducers")&&Ini.ContainsSection("GasProducers")&&Ini.
ContainsSection("Quotas");}public void SaveIni(){foreach(Item i in Items.Items.Values){Ini.Set("Quotas",i.ItemId,i.TargetAmount);}
foreach(string s in PowerProducers.Keys){Ini.Set("PowerProducers",s,PowerProducers[s].ToString("N6"));}foreach(string s in
GasProducers.Keys){Ini.Set("GasProducers",s,GasProducers[s].ToString("N6"));}Block.CustomData=Ini.ToString();}}Log Logger=new Log();
Config Conf;IEnumerator<string>MainRoutine;List<SubRoutine>SubRoutines=new List<SubRoutine>();Program(){Conf=new Config(Me);
Runtime.UpdateFrequency=Conf.UpdateSpeed;MainRoutine=Run();}void Save(){}void Main(string argument,UpdateType updateSource){try
{Logger.Add(Runtime.LastRunTimeMs,MainRoutine.Current);Echo(Logger.Formated());MainRoutine.MoveNext();}catch(Exception e)
{Echo("Error - restarting "+e.Message);Echo(e.StackTrace);Runtime.UpdateFrequency=UpdateFrequency.None;}}IEnumerator<
string>Run(){List<IMyTerminalBlock>blockList=new List<IMyTerminalBlock>();Conf.AvailableRoutines.Add("gas",new GasRoutine(Conf
));Conf.AvailableRoutines.Add("power",new PowerRoutine(Conf));Conf.AvailableRoutines.Add("item",new ItemRoutine(Conf));
Conf.AvailableRoutines.Add("security",new SecurityRoutine(Conf));Conf.AvailableRoutines.Add("screen",new ScreenRoutine(Conf)
);SubRoutines.Add(Conf.AvailableRoutines["screen"]);for(int i=0;i<Conf.GasTicks;i++)SubRoutines.Add(Conf.
AvailableRoutines["gas"]);for(int i=0;i<Conf.PowerTicks;i++)SubRoutines.Add(Conf.AvailableRoutines["power"]);for(int i=0;i<Conf.
SecurityTicks;i++)SubRoutines.Add(Conf.AvailableRoutines["security"]);for(int i=0;i<Conf.ItemTicks;i++)SubRoutines.Add(Conf.
AvailableRoutines["item"]);int iteration=0;Random rnd=new Random();while(true){if(iteration++%Conf.ScanInterval==0){blockList.Clear();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blockList,t=>Conf.ScanConnectedGrids||t.IsSameConstructAs(Me));yield return
"Refreshed Blocklist";foreach(SubRoutine sub in Conf.AvailableRoutines.Values){sub.Refresh(Conf,blockList);yield return
"Refreshing Subroutine Blocks";}}SubRoutines=SubRoutines.OrderBy(item=>rnd.Next()).ToList();foreach(SubRoutine sub in SubRoutines){yield return sub.
Next();}}}abstract class AbstractStatusList<T>:List<T>{public string[]IncludedParts;public string[]ExcludedParts;public
AbstractStatusList(string[]includedParts,string[]excludedParts){IncludedParts=includedParts;ExcludedParts=excludedParts;}public
AbstractStatusList(){}public double MaxCapacity{get;private set;}public double CurrentCapacity{get;private set;}public double PastCapacity
{get;private set;}public double CurrentCapacityPercentage{get;private set;}public double PastCapacityPercentage{get;
private set;}public double CapacityChange{get;private set;}new public void Clear(){base.Clear();MaxCapacity=0;CurrentCapacity=0
;PastCapacity=0;CurrentCapacityPercentage=1;PastCapacityPercentage=1;}public void Load(List<IMyTerminalBlock>blocks){
Clear();this.AddRange(blocks.FindAll(block=>Matches(block)).Cast<T>());MaxCapacity=GetTotalMaxCapacity();}public bool
AddMatching(IMyTerminalBlock block){if(Matches(block)){Add((T)block);return true;}return false;}public void RefreshMax(){
MaxCapacity=GetTotalMaxCapacity();}protected virtual bool Matches(IMyTerminalBlock block){if(!(block is T))return false;if(
IncludedParts!=null&&IncludedParts.Length>0)foreach(string part in IncludedParts)if(!block.BlockDefinition.SubtypeId.Contains(part))
return false;if(ExcludedParts!=null&&ExcludedParts.Length>0)foreach(string part in ExcludedParts)if(block.BlockDefinition.
SubtypeId.Contains(part))return false;return true;}protected double GetTotalMaxCapacity(){double result=0;ForEach(block=>result+=
GetMaxCapacity(block));return result;}protected double GetTotalCapacity(){double result=0;ForEach(block=>result+=GetCapacity(block));
return result;}public void Refresh(){PastCapacity=CurrentCapacity;PastCapacityPercentage=CurrentCapacityPercentage;
CurrentCapacity=Count==0?0:GetTotalCapacity();CurrentCapacityPercentage=Count==0?1:CurrentCapacity/MaxCapacity;CapacityChange=Count==0?
0:CurrentCapacity-PastCapacity;if(double.IsNaN(CurrentCapacityPercentage)||double.IsInfinity(CurrentCapacityPercentage)){
CurrentCapacityPercentage=0;}}protected abstract double GetMaxCapacity(T block);protected abstract double GetCapacity(T block);}class CargoGroup{
public string Tag;public float MaxCapacity;List<IMyInventory>Inventories=new List<IMyInventory>();public CargoGroup(string tag
){Tag=tag;}public void Add(IMyTerminalBlock block){for(int i=0;i<block.InventoryCount;i++){Inventories.Add(block.
GetInventory(i));MaxCapacity+=(float)block.GetInventory(i).MaxVolume;}}public int InventoryCount{get{return Inventories.Count;}}
public float CurrentCapacity{get{float result=0;foreach(IMyInventory i in Inventories)result+=(float)i.CurrentVolume;return
result;}}public float Filled{get{return CurrentCapacity>0?CurrentCapacity/MaxCapacity:0;}}}class CycleTime{public DateTime
CycleStartTime=DateTime.Now;public long LastCycleTime;public int CycleNumber=0;public void NewCycle(){LastCycleTime=CurrentCycleTime;
CycleStartTime=DateTime.Now;CycleNumber++;}public long CurrentCycleTime{get{return(long)(DateTime.Now-CycleStartTime).TotalSeconds;}}}
class FormatUtils{public static string MaxLength(string input,int length){if(input==null)return null;return input.Substring(0
,Math.Min(length,input.Length));}public static string DistanceString(double amount){if(amount<1)return(amount*100d).
ToString("0cm");else if(amount<10)return amount.ToString("0.0m");else if(amount<10000)return amount.ToString("0m");else if(
amount<100000)return(amount/1000d).ToString("0.0")+"km";else return(amount/1000d).ToString("0")+"km";}public static string
AmountString(double amount){if(amount<10)return amount.ToString("0.0");else if(amount<1000)return amount.ToString("0");else if(
amount<10000)return(amount/1000f).ToString("0.0")+" K";else if(amount<1000000)return(amount/1000f).ToString("0")+" K";else if(
amount<10000000)return(amount/1000000f).ToString("0.0")+" M";else if(amount<1000000000)return(amount/1000000f).ToString("0")+
" M";else if(amount<10000000000)return(amount/1000000000f).ToString("0.0")+" B";else return(amount/1000000000f).ToString("0"
)+" B";}public static string PowerString(double amount){if(amount<0.001)return(amount*1000000f).ToString("0")+" W";else
if(amount<1)return(amount*1000f).ToString("0")+" KW";else if(amount<10f)return amount.ToString("0.0")+" MW";else if(amount
<1000f)return amount.ToString("0")+" MW";else if(amount<10000f)return(amount/1000f).ToString("0.0")+" GW";else if(amount<
1000000f)return(amount/1000f).ToString("0")+" GW";else if(amount<10000000f)return(amount/1000000f).ToString("0.0")+" TW";else
return(amount/1000000f).ToString("0")+" TW";}public static string GasString(double amount){if(amount<10)return amount.ToString
("0.0")+" L";else if(amount<1000)return amount.ToString("0")+" L";else if(amount<100000)return(amount/1000).ToString(
"0.0")+" KL";else if(amount<10000000)return(amount/1000).ToString("0")+" KL";else if(amount<1000000000)return(amount/1000000)
.ToString("0.0")+" ML";else if(amount<100000000000)return(amount/1000000).ToString("0")+" ML";else return(amount/
1000000000).ToString("0.0")+" GL";}public static string HoursToTime(double hours){if(double.IsNaN(hours)||double.IsInfinity(hours)
)return"forever";return SecondsToTime((long)(hours*3600));}public static string SecondsToTime(long seconds){int days=(int
)(seconds/86400);seconds-=(days*86400);int hours=(int)(seconds/3600);seconds-=(hours*3600);int minutes=(int)(seconds/60);
seconds-=(minutes*60);string result=String.Format("{0:00}:{1:00}",minutes,seconds);if(hours>0)result=String.Format("{0:00}:{1}"
,hours,result);if(days>365)result=String.Format("> 1y",days,result);else if(days>9)result=String.Format("{0}d",days,
result);else if(days>0)result=String.Format("{0}d {1}",days,result);return result;}public static string ToGPS(Vector3D vector,
string name){return ToGPS(vector,name,"#FFFFFFFF");}public static string ToGPS(Vector3D vector,string name,string color){
return"GPS:"+name+":"+vector.X+":"+vector.Y+":"+vector.Z+":"+color+":";}}class GasRoutine:SubRoutine{MyGasGeneratorList
GeneratorList=new MyGasGeneratorList();MyGasTankList OxygenTankList=new MyGasTankList(new string[]{"Oxygen"},null);MyGasTankList
HydrogenTankList=new MyGasTankList(new string[]{"Hydrogen"},null);MyVariableGasProducerList OxygenFarms;List<IMyGasGenerator>
EmptyGeneratorList=new List<IMyGasGenerator>();bool doUpdate=true;public GasRoutine(Config newConfig):base(newConfig){OxygenFarms=new
MyVariableGasProducerList(newConfig);}public override void RefreshLists(){doUpdate=true;}public override IEnumerator<string>Run(){CycleTimer.
NewCycle();List<IMyTerminalBlock>blockList=new List<IMyTerminalBlock>();Screen gscreen=InitScreen(Conf.DisplayGas,"Gas Overview"
);while(true){if(doUpdate){gscreen=InitScreen(Conf.DisplayGas,"Gas Overview");GeneratorList.Clear();OxygenTankList.Clear(
);HydrogenTankList.Clear();OxygenFarms.Clear();foreach(IMyTerminalBlock block in GlobalBlockList){if(GeneratorList.
AddMatching(block))continue;if(HydrogenTankList.AddMatching(block))continue;if(OxygenTankList.AddMatching(block))continue;if(
OxygenFarms.AddMatching(block))continue;}UpdateScreenTimeAll();yield return"Loaded "+GeneratorList.Count+" Generators\n"+"Loaded "+
OxygenTankList.Count+" O2 Tanks\n"+"Loaded "+HydrogenTankList.Count+" H2 Tanks\n"+"Loaded "+OxygenFarms.Count+" O2 Farms";
GeneratorList.RefreshMax();OxygenTankList.RefreshMax();HydrogenTankList.RefreshMax();OxygenFarms.RefreshMax();UpdateScreenTimeAll();
yield return"Refreshed Gas Levels";doUpdate=false;}if(OxygenFarms.Count>0){OxygenFarms.Refresh();UpdateScreenTimeAll();yield
return"Checked "+OxygenFarms.Count+" Farms";}if(OxygenTankList.Count>0){OxygenTankList.Refresh();UpdateScreenTimeAll();yield
return"Checked "+OxygenTankList.Count+" O2 Tanks";}if(HydrogenTankList.Count>0){HydrogenTankList.Refresh();UpdateScreenTimeAll
();yield return"Checked "+HydrogenTankList.Count+" H2 Tanks";}if(GeneratorList.Count>0){GeneratorList.Refresh();
EmptyGeneratorList.Clear();foreach(IMyGasGenerator generator in GeneratorList){if(generator.GetInventory(0).ItemCount==0){
EmptyGeneratorList.Add(generator);}}UpdateScreenTimeAll();yield return"Checked "+GeneratorList.Count+" O2 Generators";}CycleTimer.NewCycle
();gscreen.Clean();if(OxygenTankList.Count>0){AddTankLines(gscreen,"O2-Tanks",OxygenTankList,0.2,0.01);}if(
HydrogenTankList.Count>0){AddTankLines(gscreen,"H2-Tanks",HydrogenTankList,0.2,0.01);}if(OxygenFarms.Count>0){gscreen.AddFormat(
"{0} {1,3} $","O2-Farms",LCDUtils.WarningLCD(OxygenFarms.CurrentCapacityPercentage,0.1,0),OxygenFarms.Count);gscreen.AddFormat(
"{0} {1,8} / {2,8} § {3,3:0}%",OxygenFarms.CurrentCapacityPercentage.ToString(),LCDUtils.CreateColor(0,0,0),FormatUtils.GasString(OxygenFarms.
CurrentCapacity),FormatUtils.GasString(OxygenFarms.MaxCapacity),OxygenFarms.CurrentCapacityPercentage*100);}if(GeneratorList.Count>0){
char statusIcon=LCDUtils.INFO;if(EmptyGeneratorList.Count>0)statusIcon=LCDUtils.WARNING;gscreen.AddFormat("{0} {1,3} $",
"O2-Generators",statusIcon,GeneratorList.Count);for(int i=0;i<Math.Min(Conf.EmptyO2GensListLength,EmptyGeneratorList.Count);i++){
gscreen.Add(LCDUtils.CreateColor(0,0,0).ToString()+" - "+EmptyGeneratorList[i].DisplayNameText+" empty");}if(EmptyGeneratorList
.Count>Conf.EmptyO2GensListLength){gscreen.Add(LCDUtils.CreateColor(0,0,0).ToString()+" - "+(EmptyGeneratorList.Count-
Conf.EmptyO2GensListLength)+" others empty");}ItemRoutine itemRoutine=Conf.AvailableRoutines["item"]as ItemRoutine;if(
itemRoutine!=null&&Conf.ShowIce){itemRoutine.ItemLine(gscreen,Conf.Items.Get(ItemStorage.CATEGORY_ORE,"Ice"));}}UpdateScreenTimeAll
();yield return"Prepared "+gscreen.Count+" Screen Lines";}}private void AddTankLines(Screen sc,string name,MyGasTankList
source,double warning,double error){double mwh=Math.Abs(source.CapacityChange)/CycleTimer.LastCycleTime*3600;double hours=(
source.CurrentCapacity>source.PastCapacity?(source.MaxCapacity-source.CurrentCapacity):source.CurrentCapacity)/mwh;if(
CycleTimer.CycleNumber%2==0){sc.AddFormat("{0} {1,3} $ {2,9}/h",name,LCDUtils.StatusLed(source.CurrentCapacityPercentage*1.1,
source.CurrentCapacity/source.PastCapacity-1,warning,error),source.Count,(source.PastCapacity<=source.CurrentCapacity?'+':'-')
+" "+FormatUtils.GasString(mwh));}else{sc.AddFormat("{0} {1,3} $ {2}",name,LCDUtils.StatusLed(source.
CurrentCapacityPercentage*1.1,source.CurrentCapacity/source.PastCapacity-1,warning,error),source.Count,(source.PastCapacity<=source.
CurrentCapacity?'+':'-')+" "+FormatUtils.HoursToTime(hours));}sc.AddFormat("{0} {1,8} / {2,8} § {3,3:0}%",source.
CurrentCapacityPercentage.ToString(),LCDUtils.CreateColor(0,0,0).ToString(),FormatUtils.GasString(source.CurrentCapacity),FormatUtils.GasString(
source.MaxCapacity),source.CurrentCapacityPercentage*100);}}class ItemQuotaBlock{public IMyFunctionalBlock Block;public Item
Item;public int Quote;public string Oper;public bool Enabled{get{switch(Oper){case">":return Item.CurrentAmount>Quote;case
">=":return Item.CurrentAmount>=Quote;case"<":return Item.CurrentAmount<Quote;case"<=":return Item.CurrentAmount<=Quote;
default:return false;}}}public void Toggle(){this.Block.Enabled=Enabled;}}class ItemRoutine:SubRoutine{List<IMyInventory>
AllInventories=new List<IMyInventory>();List<CargoGroup>Cargos=new List<CargoGroup>();List<MyInventoryItem>Inv=new List<
MyInventoryItem>();List<Item>Items=new List<Item>();MyQuotaToggler Toggler;bool doUpdate=true;int refineries=0;int assembler=0;float
averageYield=1;public ItemRoutine(Config newConfig):base(newConfig){Toggler=new MyQuotaToggler(newConfig);}public override void
RefreshLists(){doUpdate=true;}public override IEnumerator<string>Run(){int c=0;while(true){if(doUpdate){Screen cargoScreen=
InitScreen(Conf.DisplayCargo,"Cargo Overview");Screen ingotScreen=InitScreen(Conf.DisplayIngots,"Ingot Overview");Screen
componentsScreen=InitScreen(Conf.DisplayComponents,"Component Overview");Screen ammoScreen=InitScreen(Conf.DisplayAmmo,"Ammo Overview");
Screen toolScreen=InitScreen(Conf.DisplayTools,"Equipment Overview");Screen productionScreen=InitScreen(Conf.DisplayProduction
,"Production Overview");int hits=0;c=0;Cargos.Clear();AllInventories.Clear();CargoGroup cgu=new CargoGroup("Other");
Cargos.Add(new CargoGroup(Conf.TagAmmo));Cargos.Add(new CargoGroup(Conf.TagBottle));Cargos.Add(new CargoGroup(Conf.
TagComponent));Cargos.Add(new CargoGroup(Conf.TagIngot));Cargos.Add(new CargoGroup(Conf.TagOre));Cargos.Add(new CargoGroup(Conf.
TagTool));Cargos.Add(cgu);int _refineries=0;int _assembler=0;float _yieldSum=0;foreach(IMyTerminalBlock b in GlobalBlockList){
hits=0;if(b.InventoryCount==0)continue;foreach(CargoGroup cg in Cargos){if(b.CustomName.Contains(cg.Tag)){hits++;cg.Add(b);}
}if(b is IMyAssembler){_assembler++;}else if(b is IMyRefinery){_refineries++;string status=(b as IMyRefinery).
DetailedInfo;string[]lines=status.Split('\n');foreach(string s in lines){if(s.StartsWith("Effectiveness")){string percent=s.Replace(
"Effectiveness: ","").Replace("%","");float eff=1;if(float.TryParse(percent,out eff)){_yieldSum+=eff/100f;}else{_yieldSum+=1;}break;}}}if
(hits==0){cgu.Add(b);}for(int i=0;i<b.InventoryCount;i++){AllInventories.Add(b.GetInventory(i));}if(++c%50==0){yield
return"Scanning for Inventories "+c+" / "+GlobalBlockList.Count;}}refineries=_refineries;assembler=_assembler;averageYield=
_yieldSum/_refineries;UpdateScreenTimeAll();yield return"Added "+AllInventories.Count+" Inventories";if(Conf.UseIsy){
IMyTerminalBlock panel=GlobalBlockList.Find(b=>b is IMyTextPanel&&b.CustomName.Contains(Conf.IsyCraftingScreenName));if(panel!=null){
string text=(panel as IMyTextPanel).GetText();string[]lines=text.Split('\n');string[]splits;int wanted;if(lines.Length>5){for(
int i=5;i<(lines.Length-9);i++){splits=lines[i].Trim().Split(' ');if(splits.Length<5)continue;string item=splits[0];if(!int
.TryParse(splits[splits.Length-1].Replace("A","").Replace("D","").Replace("P","").Replace("H","").Replace("I","").Trim(),
out wanted)){continue;}Item it=Conf.Items.GetIsy(item);if(it!=null){it.TargetAmount=wanted;}if(+i%20==0){
UpdateScreenTimeAll();yield return"Parsing Isy Lines "+i+" / "+(lines.Length-9);}}}}UpdateScreenTimeAll();yield return
"Parsed Isy Autocrafting";}if(Conf.CalculateIngotQuotas){Recipe all=Conf.Items.Sum();UpdateScreenTimeAll();yield return"Calculated Ingot Sums";
foreach(string key in Conf.Items.Ingots.Keys){Conf.Items.Get(ItemStorage.CATEGORY_INGOT,key).TargetAmount=all.GetValueOrDefault
(key,0)/Conf.AssemlberEfficiency*Conf.IngotCalcStockpile;}Conf.Items.Get(ItemStorage.CATEGORY_INGOT,"Uranium").
TargetAmount+=Conf.UraniumPowerBonus;UpdateScreenTimeAll();yield return"Calculated Ingot Quotas";}if(Conf.UseIsy||Conf.
CalculateIngotQuotas){Conf.SaveIni();UpdateScreenTimeAll();yield return"Updated CustomData";}IEnumerator<string>scanner=Toggler.ScanRun(
GlobalBlockList);while(scanner.MoveNext()){UpdateScreenTimeAll();yield return scanner.Current;}doUpdate=false;}c=0;int step=0;int
itemCount=Conf.Items.Items.Count;foreach(IMyInventory i in AllInventories){step+=25;if(step>=100){step=0;UpdateScreenTimeAll();
yield return"Scanning Cargo "+(++c)+" / "+AllInventories.Count;}Inv.Clear();i.GetItems(Inv);foreach(MyInventoryItem it in Inv
){Conf.Items.Get(it.Type.ToString()).CountAmount+=(float)it.Amount;step+=5;}if(step>=100){step=0;UpdateScreenTimeAll();
yield return"Scanning Cargo "+(++c)+" / "+AllInventories.Count;}}Conf.Items.Reset();yield return"Cargo Scan complete";if(
itemCount!=Conf.Items.Items.Count){Conf.SaveIni();UpdateScreenTimeAll();yield return"Saved new Items to Custom Data";}IEnumerator
<string>run=Toggler.ToggleRun();while(run.MoveNext()){UpdateScreenTimeAll();yield return run.Current;}CycleTimer.NewCycle
();Screen sc=Screens[Conf.DisplayCargo];sc.Clean();foreach(CargoGroup cg in Cargos){if(cg.InventoryCount==0&&Conf.
HideEmpty)continue;sc.AddFormat("{0} {1,3} {2,-10} § {3,3:0}%",cg.Filled.ToString(),LCDUtils.StatusLed((1-cg.Filled)*1.1f,0,0.5,
0.25),cg.InventoryCount,cg.Tag,cg.Filled*100);}sc.Add("");sc.AddFormat("[{0,3}] $ {1}x","Assembler",assembler,Conf.
AssemlberEfficiency);sc.AddFormat("[{0,3}] $ {1:0}%","Refineries",refineries,averageYield*100);UpdateScreenTimeAll();yield return
"Prepared "+sc.Count+" Cargo-Screen Lines";sc=Screens[Conf.DisplayIngots];Items.Clear();foreach(Item i in Conf.Items.Items.Values){
if(Conf.HideEmpty&&i.CurrentAmount==0&&i.PastAmount==0)continue;if(i.Screen==ItemStorage.CATEGORY_INGOT){Items.Add(i);}}
UpdateScreenTimeAll();yield return"Filtered "+Items.Count+" Ingot / Ore Items";sc.Clean();foreach(Item it in Items)ItemLine(sc,it,Conf.
Items.Ingots.GetValueOrDefault(it.Name,null));UpdateScreenTimeAll();yield return"Prepared "+sc.Count+" OreIngot-Screen Lines"
;sc=Screens[Conf.DisplayComponents];run=AddScreen(sc,ItemStorage.CATEGORY_COMPONENT);while(run.MoveNext()){
UpdateScreenTimeAll();yield return run.Current;}sc=Screens[Conf.DisplayAmmo];run=AddScreen(sc,ItemStorage.CATEGORY_AMMO);while(run.MoveNext
()){UpdateScreenTimeAll();yield return run.Current;}sc=Screens[Conf.DisplayTools];run=AddScreen(sc,ItemStorage.
CATEGORY_TOOL,ItemStorage.CATEGORY_OBOTTLE,ItemStorage.CATEGORY_HBOTTLE,ItemStorage.CATEGORY_DATAPAD,ItemStorage.CATEGORY_CONSUME);
while(run.MoveNext()){UpdateScreenTimeAll();yield return run.Current;}}}private IEnumerator<string>AddScreen(Screen List,
params string[]categories){Items.Clear();foreach(Item i in Conf.Items.Items.Values){if(Conf.HideEmpty&&i.CurrentAmount==0&&i.
PastAmount==0)continue;if(categories.Contains(i.Screen)){Items.Add(i);}}yield return"Filtered "+Items.Count+" Items for "+List.
Name;List.Clean();foreach(Item it in Items)ItemLine(List,it);yield return"Prepared "+List.Count+" Lines for "+List.Name;}
public void ItemLine(Screen list,Item item,List<Item>incoming=null){float amount=0;if(item.TargetAmount>0){if(incoming!=null&&
incoming.Count>0){foreach(Item it in incoming)amount+=it.CurrentAmount*it.RefinesInto.GetValueOrDefault(item.Name)*averageYield/
(it.affectedByAssemblerEfficiency?Conf.AssemlberEfficiency:1);list.AddFormat("{0} {1,-15} {2,6} / {3,6} § {4,3:0}%",item.
Fullfilled.ToString()+":"+(amount/item.TargetAmount).ToString(),LCDUtils.StatusLed(item.CurrentAmount/item.TargetAmount,item.
CurrentAmount/item.PastAmount-1),FormatUtils.MaxLength(item.DisplayName!=null?item.DisplayName:item.Name,15),FormatUtils.AmountString
(item.CurrentAmount),FormatUtils.AmountString(item.TargetAmount),Math.Min(999,item.Fullfilled*100));}else{list.AddFormat(
"{0} {1,-15} {2,6} / {3,6} § {4,3:0}%",item.Fullfilled.ToString(),LCDUtils.StatusLed(item.CurrentAmount/item.TargetAmount,item.CurrentAmount/item.PastAmount-1
),FormatUtils.MaxLength(item.DisplayName!=null?item.DisplayName:item.Name,15),FormatUtils.AmountString(item.CurrentAmount
),FormatUtils.AmountString(item.TargetAmount),Math.Min(999,item.Fullfilled*100));}}else{list.AddFormat("{0} $ {1,6} ---%"
,FormatUtils.MaxLength(item.DisplayName!=null?item.DisplayName:item.Name,15),LCDUtils.Arrow(item.CurrentAmount/item.
PastAmount-1),FormatUtils.AmountString(item.CurrentAmount));}if(incoming!=null&&amount>0){if(Conf.DetailedIngotOre){foreach(Item
it in incoming){if(it.CurrentAmount<1)continue;list.AddFormat("{0} \u2514\u2500 {1} {2} \u2248 {3} Ingots","",LCDUtils.
CreateColor(0,0,0),FormatUtils.AmountString(it.CurrentAmount),it.DisplayName,FormatUtils.AmountString(it.CurrentAmount*it.
RefinesInto.GetValueOrDefault(item.Name)*averageYield/(it.affectedByAssemblerEfficiency?Conf.AssemlberEfficiency:1)));}}else{list.
AddFormat("{0} \u2514\u2500 ~{1} Ingots incoming","",LCDUtils.CreateColor(0,0,0),FormatUtils.AmountString(amount));}}}}class
ItemStorage{public const string MYOB="MyObjectBuilder_";public const string CATEGORY_COMPONENT="Component";public const string
CATEGORY_ORE="Ore";public const string CATEGORY_INGOT="Ingot";public const string CATEGORY_AMMO="AmmoMagazine";public const string
CATEGORY_TOOL="PhysicalGunObject";public const string CATEGORY_PHYSICAL="PhysicalObject";public const string CATEGORY_DATAPAD=
"Datapad";public const string CATEGORY_CONSUME="ConsumableItem";public const string CATEGORY_INGREDIENTS="Ingredients";public
const string CATEGORY_OBOTTLE="OxygenContainerObject";public const string CATEGORY_HBOTTLE="GasContainerObject";public const
string CATEGORY_BOTTLE="Bottle";public const string CO="Cobalt";public const string AU="Gold";public const string FE="Iron";
public const string MG="Magnesium";public const string NI="Nickel";public const string PT="Platinum";public const string SI=
"Silicon";public const string AG="Silver";public const string ST="Stone";public const string UR="Uranium";public const string TI=
"DuraniumIngot";public const string TH="Thorium";public SortedList<string,Item>Items=new SortedList<string,Item>();public Dictionary<
string,List<Item>>Ingots=new Dictionary<string,List<Item>>();public ItemStorage(){Item change;AddIngot(CO);AddIngot(AU);
AddIngot(FE);AddIngot(MG);AddIngot(NI);AddIngot(PT);AddIngot(SI);AddIngot(AG);AddIngot(ST);AddIngot(UR);AddOre(CO,0.3f);AddOre(
AU,0.01f);AddOre(FE,0.7f);AddOre(MG,0.007f);AddOre(NI,0.1f);AddOre(PT,0.005f);AddOre(SI,0.7f);AddOre(AG,0.1f);AddOre(ST,
new Recipe(){{ST,0.014f},{FE,0.03f},{NI,0.0024f},{SI,0.004f}});AddOre(UR,0.01f);change=AddOre("Ice",new Recipe());change.
DisplayName="Ice";change=AddOre("Scrap",new Recipe(){{FE,0.8f}},"");change.DisplayName="Scrap";AddComponent("Construction",new
Recipe(){{FE,8}});AddComponent("Canvas",new Recipe(){{FE,2},{SI,35}});AddComponent("MetalGrid",new Recipe(){{FE,12},{CO,3},{NI
,5}});AddComponent("InteriorPlate",new Recipe(){{FE,3}});AddComponent("SteelPlate",new Recipe(){{FE,21}});AddComponent(
"Girder",new Recipe(){{FE,6}});AddComponent("SmallTube",new Recipe(){{FE,5}});AddComponent("LargeTube",new Recipe(){{FE,30}});
AddComponent("Motor",new Recipe(){{FE,20},{NI,5}});AddComponent("Display",new Recipe(){{FE,1},{SI,5}});AddComponent(
"BulletproofGlass",new Recipe(){{SI,15}});AddComponent("Computer",new Recipe(){{FE,0.5f},{SI,0.2f}});AddComponent("Reactor",new Recipe(){{
ST,20},{FE,15},{AG,5}});AddComponent("Thrust",new Recipe(){{FE,30},{CO,10},{AG,1},{PT,0.4f}});AddComponent(
"GravityGenerator",new Recipe(){{FE,600},{CO,220},{AG,5},{AU,10}});AddComponent("Medical",new Recipe(){{FE,60},{NI,70},{AG,20}});
AddComponent("RadioCommunication",new Recipe(){{FE,8},{SI,1}});AddComponent("Detector",new Recipe(){{FE,5},{NI,15}});AddComponent(
"Explosives",new Recipe(){{SI,0.5f},{MG,2}});AddComponent("SolarCell",new Recipe(){{NI,3},{SI,6}});AddComponent("PowerCell",new
Recipe(){{FE,10},{NI,2},{SI,1}});AddComponent("Superconductor",new Recipe(){{FE,10},{AU,2}});AddAmmo("NATO_5p56x45mm",new
Recipe());AddAmmo("NATO_25x184mm",new Recipe(){{FE,40},{NI,5},{MG,3}});AddAmmo("Missile200mm",new Recipe(){{FE,55},{NI,7},{SI,
0.2f},{MG,1.2f},{PT,0.04f},{UR,0.1f}});AddAmmo("SemiAutoPistolMagazine",new Recipe(){{FE,0.25f},{NI,0.05f},{MG,0.05f}});
AddAmmo("FullAutoPistolMagazine",new Recipe(){{FE,0.5f},{NI,0.1f},{MG,0.1f}});AddAmmo("ElitePistolMagazine",new Recipe(){{FE,
0.3f},{NI,0.1f},{MG,0.1f}});AddAmmo("AutomaticRifleGun_Mag_20rd",new Recipe(){{FE,0.8f},{NI,0.2f},{MG,0.15f}});AddAmmo(
"RapidFireAutomaticRifleGun_Mag_50rd",new Recipe(){{FE,2f},{NI,0.5f},{MG,0.4f}});AddAmmo("PreciseAutomaticRifleGun_Mag_5rd",new Recipe(){{FE,0.8f},{NI,0.2f},
{MG,0.15f}});AddAmmo("UltimateAutomaticRifleGun_Mag_30rd",new Recipe(){{FE,1.2f},{NI,0.4f},{MG,0.25f}});AddAmmo(
"MediumCalibreAmmo",new Recipe(){{FE,15f},{NI,2f},{MG,1.2f}});AddAmmo("LargeCalibreAmmo",new Recipe(){{FE,60f},{NI,8f},{MG,5f},{UR,0.1f}});
AddAmmo("LargeRailgunAmmo",new Recipe(){{FE,20f},{NI,3f},{SI,30},{UR,1}});AddAmmo("SmallRailgunAmmo",new Recipe(){{FE,4f},{NI,
0.5f},{SI,5},{UR,0.2f}});AddAmmo("AutocannonClip",new Recipe(){{FE,25f},{NI,0.3f},{MG,2.0f}});AddTool("AngleGrinderItem",new
Recipe(){{ST,5},{FE,3},{NI,1},{SI,1}});AddTool("AngleGrinder2Item",new Recipe(){{FE,3},{NI,1},{SI,6},{CO,2}});AddTool(
"AngleGrinder3Item",new Recipe(){{FE,3},{NI,1},{SI,2},{CO,1},{AG,2}});AddTool("AngleGrinder4Item",new Recipe(){{FE,3},{NI,1},{SI,2},{CO,1},
{PT,2}});AddTool("HandDrillItem",new Recipe(){{FE,20},{NI,3},{SI,3}});AddTool("HandDrill2Item",new Recipe(){{FE,30},{NI,3
},{SI,5}});AddTool("HandDrill3Item",new Recipe(){{FE,20},{NI,3},{SI,3},{AG,2}});AddTool("HandDrill4Item",new Recipe(){{FE
,20},{NI,3},{SI,3},{PT,2}});AddTool("WelderItem",new Recipe(){{ST,3},{FE,5},{NI,1}});AddTool("Welder2Item",new Recipe(){{
FE,5},{NI,1},{SI,2},{CO,0.2f}});AddTool("Welder3Item",new Recipe(){{FE,5},{NI,1},{CO,0.2f},{AG,2}});AddTool("Welder4Item",
new Recipe(){{FE,5},{NI,1},{CO,0.2f},{PT,2}});AddTool("AutomaticRifleItem",new Recipe(){{FE,3},{NI,1}});AddTool(
"PreciseAutomaticRifleItem",new Recipe(){{FE,3},{NI,1},{CO,5}});AddTool("RapidFireAutomaticRifleItem",new Recipe(){{FE,3},{NI,8}});AddTool(
"UltimateAutomaticRifleItem",new Recipe(){{FE,3},{NI,1},{AG,6},{PT,4}});AddTool("SemiAutoPistolItem",new Recipe(){{FE,1},{NI,0.3f}});AddTool(
"FullAutoPistolItem",new Recipe(){{FE,1.5f},{NI,0.5f}});AddTool("ElitePistolItem",new Recipe(){{FE,1},{NI,0.4f},{PT,0.5f},{AG,1}});AddTool(
"BasicHandHeldLauncherItem",new Recipe(){{FE,30},{NI,10},{CO,5}});AddTool("AdvancedHandHeldLauncherItem",new Recipe(){{FE,30},{NI,10},{CO,5},{PT,5}
});AddCraftable(CATEGORY_HBOTTLE,"HydrogenBottle",new Recipe(){{FE,80},{NI,30},{SI,10}});AddCraftable(CATEGORY_OBOTTLE,
"OxygenBottle",new Recipe(){{FE,80},{NI,30},{SI,10}});Add(CATEGORY_DATAPAD,"Datapad");Add(CATEGORY_CONSUME,"Medkit");Add(
CATEGORY_CONSUME,"Powerkit");Add(CATEGORY_CONSUME,"CosmicCoffee");Add(CATEGORY_CONSUME,"ClangCola");Add(CATEGORY_COMPONENT,"SpaceCredit"
);AddIngot(TH);AddOre(TH,0.004f);change=AddCraftable(CATEGORY_INGOT,"ThUFuelCell",new Recipe(){{TH,0.04f},{UR,0.02f}});
change.DisplayName="Fuel Cell";change=AddIngot(TI);change.DisplayName="Titanium";AddOre("Titanium",new Recipe(){{TI,0.004f}});
change=AddComponent("SuperComputer",new Recipe(){{FE,75},{SI,30},{TH,10},{AG,75},{AU,180}});change.DisplayName=
"Quantum Computer";AddComponent("ZoneChip",new Recipe(){{FE,200},{AU,50},{NI,200},{PT,40},{TH,50}});AddComponent("TitaniumPlate",new
Recipe(){{FE,500},{PT,30},{NI,200},{TI,10}});change=AddAmmo("Missile200mmHandheld",new Recipe(){{FE,55f},{NI,7f},{SI,0.2f},{UR
,0.1f},{PT,0.04f},{MG,1.2f}});change.DisplayName="Hand Missile";AddAmmo("LittleDavidMagazine",new Recipe(){{FE,700f},{MG,
255f}});AddAmmo("BBGunMagazine",new Recipe(){{FE,1800f},{MG,550f}});AddAmmo("DaveJrMagazine",new Recipe(){{FE,700f},{MG,255f
}});AddAmmo("SwarmMagazine",new Recipe(){{FE,500f},{MG,100f},{UR,3f},{TH,0.1f}});AddAmmo("Series900Magazine",new Recipe()
{{FE,400f},{MG,50f},{UR,3f},{TH,0.2f}});AddAmmo("Series900MagazineAP",new Recipe(){{FE,400f},{MG,50f},{UR,3f},{TH,0.2f}})
;AddAmmo("Series300Magazine",new Recipe(){{FE,200f},{MG,25f},{UR,3f},{TH,0.1f}});AddAmmo("Series300MagazineAP",new Recipe
(){{FE,25f},{MG,100f},{UR,3f},{TH,0.1f}});AddAmmo("PlasmaBallMag",new Recipe(){{FE,400f},{MG,200f},{UR,3f},{TH,0.2f}});
AddAmmo("AdenMagazine",new Recipe(){{FE,40f},{MG,5f},{NI,4f}});change=AddAmmo("BoforsMagazine",new Recipe(){{FE,50f},{MG,8f},{
NI,6f},{UR,0.02f}});change.DisplayName="Quad Cannon Magazine";AddAmmo("SgRocketSentryMagazine",new Recipe(){{FE,30f},{MG,
3f},{NI,2f}});change=AddAmmo("Vulcan20x102",new Recipe(){{FE,30f},{MG,3f},{NI,2f}});change.DisplayName=
"Heavy Gatling Magazine";AddAmmo("GatlingMagazine",new Recipe(){{FE,30f},{MG,3f},{NI,2f}});AddAmmo("SentryMagazine",new Recipe(){{FE,30f},{MG,3f
},{NI,2f}});AddAmmo("HeavyMissile",new Recipe(){{FE,1500f},{MG,250f},{UR,15f},{TH,0.5f}});AddAmmo("NukeMissile",new
Recipe(){{FE,2000f},{MG,500f},{UR,300f},{TH,2f}});AddAmmo("CIWSMagazine",new Recipe(){{FE,40f},{NI,4f},{MG,5f}});AddAmmo(
"SGSwarmMagazine",new Recipe(){{FE,500f},{MG,100f},{UR,3f},{TH,0.1f}});}public Item Add(Item item){if(Items.ContainsKey(item.ItemId))
return Items[item.ItemId];Items.Add(item.ItemId,item);return item;}public Item Add(string category,string subtype){return Add(
new Item(category,subtype));}public Item Add(string category,string subtype,string alias){return Add(new Item(category,
subtype){DisplayName=alias});}public Item AddCraftable(string category,string subtype,Recipe recipe,string isy=null){return Add
(new Item(category,subtype){CraftedWith=recipe,IsyName=isy==null?subtype:isy});}public Item AddComponent(string subtype,
Recipe recipe,string isy=null){Item it=AddCraftable(CATEGORY_COMPONENT,subtype,recipe,isy);return it;}public Item AddAmmo(
string subtype,Recipe recipe,string isy=null){return AddCraftable(CATEGORY_AMMO,subtype,recipe,isy);}public Item AddTool(
string subtype,Recipe recipe,string isy=null){return AddCraftable(CATEGORY_TOOL,subtype,recipe,isy);}public Item AddIngot(
string subtype){Ingots.Add(subtype,new List<Item>());return Add(CATEGORY_INGOT,subtype);}public Item AddOre(string subtype,
Recipe recipe,string suffix=" Ore"){Item it=Add(new Item(CATEGORY_ORE,subtype){RefinesInto=recipe,DisplayName=subtype+suffix})
;foreach(string key in recipe.Keys){if(Ingots.ContainsKey(key))Ingots[key].Add(it);}return it;}public Item AddOre(string
subtype,float rate){return AddOre(subtype,new Recipe(){{subtype,rate}});}public Recipe Sum(){Recipe result=new Recipe();foreach
(Item i in Items.Values){if(!i.Craftable)continue;foreach(string key in Ingots.Keys){result.AddOrPut(key,i.CraftedWith.
GetValueOrDefault(key,0f)*i.TargetAmount);}}return result;}public Item Get(string category,string name){return Get(ItemStorage.MYOB+
category+"/"+name);}public Item Get(string itemId){Item it;if(!Items.TryGetValue(itemId,out it)){it=new Item(itemId);Items.Add(
itemId,it);}return it;}public Item GetIsy(string item){foreach(Item i in Items.Values){if(item==i.IsyName)return i;}return
null;}public void Reset(){foreach(Item i in Items.Values){i.PastAmount=i.CurrentAmount;i.CurrentAmount=i.CountAmount;i.
CountAmount=0;}}public Item FindByName(string input){List<Item>holders;if(input.Contains('/')){string[]splits=input.Split('/');
holders=Items.Values.ToList().FindAll(it=>it.Category.ToLower().Contains(splits[0])&&it.Name.ToLower().Contains(splits[1]));}
else{holders=Items.Values.ToList().FindAll(it=>it.ItemId.ToLower().Contains(input));}if(holders.Count!=1)return null;return
holders.FirstOrDefault();}}class Item{public string ItemId;public string Category;public string Screen;public string Name;
public string DisplayName;public bool affectedByAssemblerEfficiency=false;public string IsyName;public Recipe CraftedWith=null
;public Recipe RefinesInto=null;public float PastAmount=0;public float CountAmount=0;public float CurrentAmount=0;public
float TargetAmount=0;public bool Craftable{get{return CraftedWith!=null&&TargetAmount>0;}}public float Fullfilled{get{return
CurrentAmount/TargetAmount;}}public float Increase{get{return CurrentAmount-PastAmount;}}public Item(string category,string name){
Category=category;Screen=category;Name=name;ItemId=ItemStorage.MYOB+Category+"/"+Name;DisplayName=generateDisplayName(Name);}
public Item(string category,string name,string screen){Category=category;Screen=screen;Name=name;ItemId=ItemStorage.MYOB+
Category+"/"+Name;DisplayName=generateDisplayName(Name);}public Item(string itemId){ItemId=itemId;string[]parts=itemId.Replace(
ItemStorage.MYOB,"").Split('/');Category=parts[0];Screen=parts[0];Name=parts[1];DisplayName=generateDisplayName(parts[1]);}private
string generateDisplayName(string name){return System.Text.RegularExpressions.Regex.Replace(name,"([a-z])([A-Z\\d+])","$1 $2")
;}}class Recipe:Dictionary<string,float>{internal void AddOrPut(string key,float v){if(ContainsKey(key))this[key]+=v;else
Add(key,v);}}class LCDUtils{public static char ARR_WHITE_RIGHT='\ue034';public static char ARR_RED_RIGHT='\ue035';public
static char ARR_GREEN_RIGHT='\ue036';public static char ARR_BLUE_RIGHT='\ue037';public static char ARR_YELLOW_RIGHT='\ue038';
public static char ARR_PINK_RIGHT='\ue039';public static char ARR_CYAN_RIGHT='\ue03a';public static char BIG_ARR_WHITE_RIGHT=
'\ue03b';public static char BIG_ARR_RED_RIGHT='\ue03c';public static char BIG_ARR_GREEN_RIGHT='\ue03d';public static char
BIG_ARR_BLUE_RIGHT='\ue03e';public static char BIG_ARR_YELLOW_RIGHT='\ue03f';public static char BIG_ARR_PINK_RIGHT='\ue040';public static
char BIG_ARR_CYAN_RIGHT='\ue041';public static char ARR_WHITE_LEFT='\ue042';public static char ARR_RED_LEFT='\ue043';public
static char ARR_GREEN_LEFT='\ue044';public static char ARR_BLUE_LEFT='\ue045';public static char ARR_YELLOW_LEFT='\ue046';
public static char ARR_PINK_LEFT='\ue047';public static char ARR_CYAN_LEFT='\ue048';public static char BIG_ARR_WHITE_LEFT=
'\ue049';public static char BIG_ARR_RED_LEFT='\ue050';public static char BIG_ARR_GREEN_LEFT='\ue051';public static char
BIG_ARR_BLUE_LEFT='\ue052';public static char BIG_ARR_YELLOW_LEFT='\ue053';public static char BIG_ARR_PINK_LEFT='\ue054';public static
char BIG_ARR_CYAN_LEFT='\ue055';public static char WARNING='\ue056';public static char INFO='\ue057';public static char
ERROR='\ue058';public static char DOTS_1='\ue030';public static char DOTS_2='\ue031';public static char DOTS_3='\ue032';
public static char DOTS_4='\ue033';public static char CHECK='\u2713';public static char CIRCLE='\u25cb';public static char
CIRCLE_FULL='\u25cf';public static char CreateColor(int r,int g,int b){return CreateColor8((int)Math.Round(r*(8f/255f)),(int)Math.
Round(g*(8f/255f)),(int)Math.Round(b*(8f/255f)));}public static char CreateColor8(int r,int g,int b){return(char)(0xE100+(
MathHelper.Clamp(r,0,7)<<6)+(MathHelper.Clamp(g,0,7)<<3)+MathHelper.Clamp(b,0,7));}public static void InitPanel(IMyTextPanel
surface,double fontSize=0){surface.ContentType=ContentType.TEXT_AND_IMAGE;surface.Font="Monospace";if(fontSize>0){surface.
FontSize=(float)fontSize;}}public static void InitPanel(IMyTextSurface surface,double fontSize=0){surface.ContentType=
ContentType.TEXT_AND_IMAGE;surface.Font="Monospace";if(fontSize>0){surface.FontSize=(float)fontSize;}}public static void InitPanel(
List<IMyTextPanel>panels,double fontSize=0){panels.ForEach(p=>InitPanel(p,fontSize));}public static void CopyLCD(
IMyTextPanel source,IMyTextPanel target){target.TextPadding=source.TextPadding;target.Font=source.Font;target.FontSize=source.
FontSize;target.FontColor=source.FontColor;target.BackgroundColor=source.BackgroundColor;target.WriteText(source.GetText());}
public static int PanelCharWidth(IMyTextSurface surface){return(int)(surface.SurfaceSize.X*(0.05f/surface.FontSize));}public
static string Filler(IMyTextSurface surface,int width=0){return new string('\u2500',width>0?width:PanelCharWidth(surface));}
public static char StatusLed(double percent,double change,double warning=0.95,double error=0.25){percent=MathHelper.Clamp(
percent,0,1);if(change<0){if(percent<error)return BIG_ARR_RED_LEFT;if(percent<warning)return BIG_ARR_YELLOW_LEFT;return
BIG_ARR_CYAN_LEFT;}else if(change>0){if(percent<error)return BIG_ARR_RED_RIGHT;if(percent<warning)return BIG_ARR_YELLOW_RIGHT;return
BIG_ARR_CYAN_RIGHT;}if(percent<error)return ERROR;if(percent<warning)return WARNING;return INFO;}public static char StatusLed(bool status)
{return status?CreateColor8(0,7,0):CreateColor8(7,0,0);}public static string Progressbar(int length,float percent){return
ProgressbarAlt(length,percent,0);}public static string ProgressbarAlt(int length,float percent,float buffer){if(length<=0)return"";
else if(length<=2){char c=' ';if(percent<0.25)c=' ';else if(percent<0.5)c=DOTS_1;else if(percent<0.75)c=DOTS_2;else if(
percent<1)c=DOTS_3;else if(percent>=1)c=DOTS_4;return c.ToString()+(length==2?" ":"");}else if(length==3){char c=' ';if(percent
<0.25)c=' ';else if(percent<0.5)c=DOTS_1;else if(percent<0.75)c=DOTS_2;else if(percent<1)c=DOTS_3;else if(percent>=1)c=
DOTS_4;return"\u2502"+c+"\u2502";}else if(length<4)return new string(' ',length);length-=2;float bufferPercent=percent+buffer;
float percentPerBlock=1f/length;string result="\u2502";for(int i=0;i<length;i++){float p=i*percentPerBlock;if(percent>=p){
result+='\u2588';continue;}if(bufferPercent>=p){result+='\u2591';continue;}result+='-';}return result+"\u2502";}public static
string Progressbar(int length,float current,float total){return Progressbar(length,current/total);}public static string Format
(int width,Line line){if(line.Holder!=null){int length=Math.Max(1,width-line.BaseText.Length+1);if(line.BaseText.Contains
("$")){return String.Format(line.BaseText.Replace("$","{0,-"+length+"}"),line.Holder);}else if(line.BaseText.Contains("§"
)){if(line.Holder.Contains(":")){string[]subs=line.Holder.Split(':');return line.BaseText.Replace("§",ProgressbarAlt(
length,float.Parse(subs[0]),float.Parse(subs[1])));}else{return line.BaseText.Replace("§",Progressbar(length,float.Parse(line.
Holder)));}}}else if(line.BaseText.Equals("-")){return Filler(null,width);}return line.BaseText;}internal static char
WarningLCD(bool warning){return warning?WARNING:INFO;}internal static char WarningLCD(double percentage,double warning=1,double
error=0.75){if(percentage<error)return ERROR;if(percentage<warning)return WARNING;return INFO;}internal static char Arrow(
float v){if(v<0)return ARR_WHITE_LEFT;if(v>0)return ARR_WHITE_RIGHT;return CIRCLE_FULL;}}struct Line{public string BaseText;
public string Holder;}class Log:List<LogM>{StringBuilder builder=new StringBuilder();public void Add(double ms,string message)
{if(Count==100)RemoveAt(Count-1);Insert(0,new LogM(){MS=ms,Message=message});}public string Formated(int count=10){
builder.Clear();builder.AppendFormat("AVG {0:00}: {1:0.000} ms\n\n",Count,AVG);for(int i=0;i<Math.Min(Count,count);i++)builder.
AppendFormat("[{0:0.000}] {1}\n",this[i].MS,this[i].Message);return builder.ToString();}public double AVG{get{if(Count==0)return 0;
double cur=0;foreach(LogM l in this)cur+=l.MS;return cur/Count;}}}struct LogM{public double MS;public string Message;}class
MyBatteryList:AbstractStatusList<IMyBatteryBlock>{public MyBatteryList():base(new string[]{"Battery"},null){}protected override
double GetCapacity(IMyBatteryBlock block){return block.CurrentStoredPower;}protected override double GetMaxCapacity(
IMyBatteryBlock block){return block.MaxStoredPower;}}class MyGasGeneratorList:AbstractStatusList<IMyGasGenerator>{protected override
double GetCapacity(IMyGasGenerator block){return block.GetInventory(0).ItemCount>0?1:0;}protected override double
GetMaxCapacity(IMyGasGenerator block){return 1;}}class MyGasTankList:AbstractStatusList<IMyGasTank>{public MyGasTankList(string[]
includedParts,string[]excludedParts):base(includedParts,excludedParts){}protected override double GetCapacity(IMyGasTank block){
return block.FilledRatio*block.Capacity;}protected override double GetMaxCapacity(IMyGasTank block){return block.Capacity;}}
class MyPowerProducerList<T>:AbstractStatusList<T>where T:IMyPowerProducer{public MyPowerProducerList(string[]includedParts,
string[]excludedParts):base(includedParts,excludedParts){}public MyPowerProducerList():base(){}protected override double
GetCapacity(T block){return block.CurrentOutput;}protected override double GetMaxCapacity(T block){return block.MaxOutput;}}class
MyQuotaToggler{Config Conf;List<ItemQuotaBlock>QuotaBlocks=new List<ItemQuotaBlock>();public MyQuotaToggler(Config conf){Conf=conf;}
public IEnumerator<string>ScanRun(List<IMyTerminalBlock>blocks){QuotaBlocks.Clear();int c=0;foreach(IMyTerminalBlock block in
blocks){if(!(block is IMyFunctionalBlock))continue;string name=block.CustomName.Trim();if(name.ToLower().Contains("iq:")){
string[]splits=name.Split(' ');for(int i=0;i<splits.Length;i++){string split=splits[i];string splitLowered=split.ToLower();if(
splitLowered.Contains("iq:")){string[]qsplit=splitLowered.Split(':');if(qsplit.Length!=4)continue;int quota;if(!int.TryParse(qsplit[
2],out quota))continue;Item holder=Conf.Items.FindByName(qsplit[3]);if(holder==null)continue;if(quota>holder.TargetAmount
)holder.TargetAmount=quota;ItemQuotaBlock qblock=new ItemQuotaBlock(){Block=block as IMyFunctionalBlock,Oper=qsplit[1],
Quote=quota,Item=holder};QuotaBlocks.Add(qblock);string newName;if(splitLowered.Contains("/")){newName=string.Format(
"IQ:{0}:{1}:{2}",qsplit[1],quota,holder.ItemId);}else{newName=string.Format("IQ:{0}:{1}:{2}",qsplit[1],quota,holder.Name);}if(!newName.
Equals(split)){block.CustomName=name.Replace(split,newName);}yield return string.Format("Added Quotablock {0}",block.
CustomName);break;}}}if(++c%50==0){yield return string.Format("Scanning for Quotablocks {0} / {1}",c,blocks.Count);}}}public
IEnumerator<string>ToggleRun(){int c=0;foreach(ItemQuotaBlock block in QuotaBlocks){block.Toggle();if(++c%25==0)yield return string
.Format("Set {0} to {1}",block.Block.CustomName,block.Block.Enabled);}}}class MyVariableGasProducerList:
AbstractStatusList<IMyOxygenFarm>{protected Config conf;public MyVariableGasProducerList(Config conf,string[]includedParts,string[]
excludedParts):base(includedParts,excludedParts){this.conf=conf;}public MyVariableGasProducerList(Config conf):base(){this.conf=conf;
}protected override double GetMaxCapacity(IMyOxygenFarm block){float currentOutput=1;float configOutput=conf.
PowerProducers.GetValueOrDefault(block.BlockDefinition.SubtypeName,-1);if(currentOutput>configOutput){if(conf.detectMaxPowerOutput){if
(configOutput==-1)conf.PowerProducers.Add(block.BlockDefinition.SubtypeName,currentOutput);else conf.PowerProducers[block
.BlockDefinition.SubtypeName]=currentOutput;conf.SaveIni();}configOutput=currentOutput;}return configOutput;}protected
override double GetCapacity(IMyOxygenFarm block){return block.GetOutput()*GetMaxCapacity(block);}}class
MyVariablePowerProducerList<T>:MyPowerProducerList<T>where T:IMyPowerProducer{protected Config conf;public MyVariablePowerProducerList(Config conf,
string[]includedParts,string[]excludedParts):base(includedParts,excludedParts){this.conf=conf;}public
MyVariablePowerProducerList(Config conf):base(){this.conf=conf;}protected override double GetMaxCapacity(T block){float currentOutput=(float)
GetCapacity(block);float configOutput=conf.PowerProducers.GetValueOrDefault(block.BlockDefinition.SubtypeName,-1);if(currentOutput>
configOutput){if(conf.detectMaxPowerOutput){if(configOutput==-1)conf.PowerProducers.Add(block.BlockDefinition.SubtypeName,
currentOutput);else conf.PowerProducers[block.BlockDefinition.SubtypeName]=currentOutput;conf.SaveIni();}configOutput=currentOutput;}
return configOutput;}}class PowerRoutine:SubRoutine{List<IMyReactor>EmptyUrReactorList=new List<IMyReactor>();List<IMyReactor>
EmptyThReactorList=new List<IMyReactor>();MyBatteryList BatteryList=new MyBatteryList();MyPowerProducerList<IMyPowerProducer>EngineList=
new MyPowerProducerList<IMyPowerProducer>(new string[]{"Hydro"},null);MyPowerProducerList<IMyReactor>UrReactorList=new
MyPowerProducerList<IMyReactor>(null,new string[]{"Fusion"});MyPowerProducerList<IMyReactor>ThReactorList=new MyPowerProducerList<
IMyReactor>(new string[]{"Fusion"},null);MyPowerProducerList<IMySolarPanel>SolarList;MyPowerProducerList<IMyPowerProducer>WindList
;bool doUpdate=true;public PowerRoutine(Config newConfig):base(newConfig){SolarList=new MyVariablePowerProducerList<
IMySolarPanel>(newConfig);WindList=new MyVariablePowerProducerList<IMyPowerProducer>(newConfig,new string[]{"Wind"},null);}public
override void RefreshLists(){doUpdate=true;}public override IEnumerator<string>Run(){CycleTimer.NewCycle();Screen pscreen=
InitScreen(Conf.DisplayPower,"Power Overview");while(true){if(doUpdate){pscreen=InitScreen(Conf.DisplayPower,"Power Overview");
BatteryList.Clear();UrReactorList.Clear();ThReactorList.Clear();EngineList.Clear();SolarList.Clear();WindList.Clear();foreach(
IMyTerminalBlock block in GlobalBlockList){if(BatteryList.AddMatching(block))continue;if(UrReactorList.AddMatching(block))continue;if(
ThReactorList.AddMatching(block))continue;if(EngineList.AddMatching(block))continue;if(SolarList.AddMatching(block))continue;if(
WindList.AddMatching(block))continue;}UpdateScreenTimeAll();yield return"Loaded "+BatteryList.Count+" Batteries"+"Loaded "+
UrReactorList.Count+" Ur Reactors"+"Loaded "+ThReactorList.Count+" Th Reactors"+"Loaded "+EngineList.Count+" Hydro Engines"+"Loaded "
+SolarList.Count+" Solars"+"Loaded "+WindList.Count+" Turbines";BatteryList.RefreshMax();UrReactorList.RefreshMax();
ThReactorList.RefreshMax();EngineList.RefreshMax();SolarList.RefreshMax();WindList.RefreshMax();UpdateScreenTimeAll();yield return
"Refreshed Power Levels";doUpdate=false;}if(BatteryList.Count>0){BatteryList.Refresh();UpdateScreenTimeAll();yield return"Checked "+BatteryList.
Count+" Batteries";}if(SolarList.Count>0){SolarList.Refresh();UpdateScreenTimeAll();yield return"Checked "+SolarList.Count+
" Solars";}if(WindList.Count>0){WindList.Refresh();UpdateScreenTimeAll();yield return"Checked "+WindList.Count+" Turbines";}if(
EngineList.Count>0){EngineList.Refresh();UpdateScreenTimeAll();yield return"Checked "+EngineList.Count+" Hydro Engines";}if(
UrReactorList.Count>0){UrReactorList.Refresh();EmptyUrReactorList.Clear();foreach(IMyReactor generator in UrReactorList){if(generator
.GetInventory(0).ItemCount==0){EmptyUrReactorList.Add(generator);}}UpdateScreenTimeAll();yield return"Checked "+
UrReactorList.Count+" Ur-Reactors";}if(ThReactorList.Count>0){ThReactorList.Refresh();EmptyThReactorList.Clear();foreach(IMyReactor
generator in ThReactorList){if(generator.GetInventory(0).ItemCount==0){EmptyThReactorList.Add(generator);}}UpdateScreenTimeAll();
yield return"Checked "+ThReactorList.Count+" Th-Reactors";}CycleTimer.NewCycle();pscreen.Clean();if(BatteryList.Count>0){
double mwh=Math.Abs(BatteryList.CapacityChange)/CycleTimer.LastCycleTime*3600;double hours=(BatteryList.CurrentCapacity>
BatteryList.PastCapacity?(BatteryList.MaxCapacity-BatteryList.CurrentCapacity):BatteryList.CurrentCapacity)/mwh;if(CycleTimer.
CycleNumber%2==0){pscreen.AddFormat("{0} {1,3} $ {2,9}/h","Batteries",LCDUtils.StatusLed(BatteryList.CurrentCapacityPercentage*1.1,
BatteryList.CurrentCapacity/BatteryList.PastCapacity-1,0.2,0.05),BatteryList.Count,(BatteryList.CapacityChange>=0?'+':'-')+" "+
FormatUtils.PowerString(mwh));}else{pscreen.AddFormat("{0} {1,3} $ {2}","Batteries",LCDUtils.StatusLed(BatteryList.
CurrentCapacityPercentage*1.1,BatteryList.CurrentCapacity/BatteryList.PastCapacity-1,0.2,0.05),BatteryList.Count,FormatUtils.HoursToTime(hours));
}pscreen.AddFormat("{0} {1,8} / {2,8} § {3,3:0}%",BatteryList.CurrentCapacityPercentage.ToString(),LCDUtils.CreateColor(0
,0,0),FormatUtils.PowerString(BatteryList.CurrentCapacity),FormatUtils.PowerString(BatteryList.MaxCapacity),BatteryList.
CurrentCapacityPercentage*100);}if(SolarList.Count>0){AddProducerLines(pscreen,"Solars",SolarList,LCDUtils.WarningLCD(SolarList.
CurrentCapacityPercentage,0.1,0));}if(WindList.Count>0){AddProducerLines(pscreen,"Turbines",WindList,LCDUtils.WarningLCD(WindList.
CurrentCapacityPercentage,0.1,0));}if(EngineList.Count>0){AddProducerLines(pscreen,"Hydro Engines",EngineList,LCDUtils.INFO);}if(UrReactorList.
Count>0){char icon=LCDUtils.INFO;if(EmptyUrReactorList.Count>0)icon=LCDUtils.WARNING;AddProducerLines(pscreen,"Ur Reactors",
UrReactorList,icon);for(int i=0;i<Math.Min(Conf.EmptyReactorList,EmptyUrReactorList.Count);i++){pscreen.Add(LCDUtils.CreateColor(0,0,
0).ToString()+" - "+EmptyUrReactorList[i].DisplayNameText+" empty");}if(EmptyUrReactorList.Count>Conf.EmptyReactorList){
pscreen.Add(LCDUtils.CreateColor(0,0,0).ToString()+" - "+(EmptyUrReactorList.Count-Conf.EmptyReactorList)+" others empty");}
ItemRoutine itemRoutine=Conf.AvailableRoutines["item"]as ItemRoutine;if(itemRoutine!=null&&Conf.ShowIce){itemRoutine.ItemLine(
pscreen,Conf.Items.Get(ItemStorage.CATEGORY_INGOT,"Uranium"));}}if(ThReactorList.Count>0){char icon=LCDUtils.INFO;if(
EmptyThReactorList.Count>0)icon=LCDUtils.WARNING;AddProducerLines(pscreen,"Th Reactors",ThReactorList,icon);for(int i=0;i<Math.Min(Conf.
EmptyReactorList,EmptyThReactorList.Count);i++){pscreen.Add(LCDUtils.CreateColor(0,0,0).ToString()+" - "+EmptyThReactorList[i].
DisplayNameText+" empty");}if(EmptyThReactorList.Count>Conf.EmptyReactorList){pscreen.Add(LCDUtils.CreateColor(0,0,0).ToString()+" - "+
(EmptyThReactorList.Count-Conf.EmptyReactorList)+" others empty");}ItemRoutine itemRoutine=Conf.AvailableRoutines["item"]
as ItemRoutine;if(itemRoutine!=null&&Conf.ShowIce){itemRoutine.ItemLine(pscreen,Conf.Items.Get(ItemStorage.CATEGORY_INGOT,
"ThUFuelCell"));}}UpdateScreenTimeAll();yield return"Prepared "+pscreen.Count+" Screen Lines";}}private void AddProducerLines<T>(
Screen sc,string name,AbstractStatusList<T>source,char icon){sc.AddFormat("{0} {1,3} $",name,icon,source.Count);sc.AddFormat(
"{0} {1,8} / {2,8} § {3,3:0}%",source.CurrentCapacityPercentage.ToString(),LCDUtils.CreateColor(0,0,0),FormatUtils.PowerString(source.CurrentCapacity)
,FormatUtils.PowerString(source.MaxCapacity),source.CurrentCapacityPercentage*100);}}class Screen{public string Name;
public string SurfaceTag="Screen";List<Line>Lines=new List<Line>();public bool AddHeader;public float Progress=-1;public long
RefreshSeconds=-1;public Screen(string surfaceTag,string name,bool addHeader=true){SurfaceTag=surfaceTag;Name=name;AddHeader=addHeader
;Add("Module loading...");}public void Clean(){Lines.Clear();}public void Add(Line line){Lines.Add(line);}public void Add
(string basicLine){Add(new Line{BaseText=basicLine});}public void Add(string basicLine,string holder){Add(new Line{
BaseText=basicLine,Holder=holder});}public void AddFormat(string format,string holder,params object[]args){Add(string.Format(
format,args),holder);}public void AppendToBuilder(StringBuilder builder,int width){if(AddHeader){string Time=RefreshSeconds>=0
?FormatUtils.SecondsToTime(RefreshSeconds):"";string ProgressBar=Progress>=0?" "+LCDUtils.Progressbar(3,Progress):"";
builder.Append(LCDUtils.Format(width+1,new Line{BaseText=string.Format("EpOS {0} $  {1}{2}\n",Name,Time,ProgressBar),Holder=""}
));builder.Append(LCDUtils.Format(width,new Line{BaseText="-"})).Append("\n");}foreach(Line l in Lines){builder.Append(
LCDUtils.Format(width,l));builder.Append("\n");}}public int Count{get{return Lines.Count;}}public void SetProgressTime(long
currentSeconds,long lastSeconds){RefreshSeconds=Math.Max(lastSeconds,currentSeconds);if(RefreshSeconds==0)Progress=1;else Progress=(
float)currentSeconds/(float)RefreshSeconds;}}class ScreenRoutine:SubRoutine{List<TaggedScreen>TaggedScreens=new List<
TaggedScreen>();Dictionary<string,Screen>AllScreens=new Dictionary<string,Screen>();bool doUpdate=true;public ScreenRoutine(Config
newConfig):base(newConfig){}public override void RefreshLists(){doUpdate=true;}public override IEnumerator<string>Run(){
StringBuilder builder=new StringBuilder();while(true){if(doUpdate){AllScreens.Clear();foreach(SubRoutine sr in Conf.AvailableRoutines
.Values){foreach(Screen s in sr.Screens.Values){if(!AllScreens.ContainsKey(s.SurfaceTag))AllScreens.Add(s.SurfaceTag,s);}
}yield return"Reloaded Routine Screens";int count=0;TaggedScreens.Clear();foreach(IMyTerminalBlock block in
GlobalBlockList){if(block is IMyTextPanel){TaggedScreen ts=new TaggedScreen{Panel=block as IMyTextPanel};string[]words=block.CustomName
.Split(' ');foreach(string s in AllScreens.Keys){foreach(string word in words){if(word.Contains(s)){ts.Tags.Add(s);}}}if(
ts.Tags.Count>0){LCDUtils.InitPanel(ts.Panel);TaggedScreens.Add(ts);}if(count++%5==0){yield return"Loading Screens   ...";
}}}doUpdate=false;yield return"Loaded "+TaggedScreens.Count+" Screens\n"+"Loaded "+AllScreens.Keys.Count+" Screentags";}
int update=0;foreach(TaggedScreen ts in TaggedScreens){builder.Clear();int width=LCDUtils.PanelCharWidth(ts.Panel);foreach(
string tag in ts.Tags){AllScreens[tag].AppendToBuilder(builder,width);builder.Append("\n");}ts.Panel.WriteText(builder);yield
return"Updated Screen "+(++update)+" / "+TaggedScreens.Count;}if(TaggedScreens.Count==0)yield return"No Screens found... idle"
;}}}class TaggedScreen{public IMyTextPanel Panel;public List<string>Tags=new List<string>();}class SecurityRoutine:
SubRoutine{List<IMyLargeTurretBase>Turrets=new List<IMyLargeTurretBase>();List<IMyLargeTurretBase>TurretsWithTarget=new List<
IMyLargeTurretBase>();List<IMyLargeTurretBase>TurretsOutOfAmmo=new List<IMyLargeTurretBase>();List<IMyTerminalBlock>DamagedBlocks=new List
<IMyTerminalBlock>();bool doUpdate=true;public SecurityRoutine(Config newConfig):base(newConfig){}public override void
RefreshLists(){doUpdate=true;}public override IEnumerator<string>Run(){int iteration=0;Screen sc=InitScreen(Conf.DisplaySecurity,
"Security Overview");while(true){if(doUpdate){sc=InitScreen(Conf.DisplaySecurity,"Security Overview");Turrets.Clear();foreach(
IMyTerminalBlock block in GlobalBlockList){if(block is IMyLargeTurretBase)Turrets.Add(block as IMyLargeTurretBase);}UpdateScreenTimeAll(
);yield return"Loaded "+Turrets.Count+" Turrets";doUpdate=false;}if(iteration++%Conf.ScanDamageInterval==0){sc.Progress=
0.0f;DamagedBlocks.Clear();int c=0;foreach(IMyTerminalBlock block in GlobalBlockList){if(!block.IsFunctional){DamagedBlocks.
Add(block);}if(++c%200==0){UpdateScreenTimeAll();yield return"Scanning for Damage "+c+" / "+GlobalBlockList.Count;}}
UpdateScreenTimeAll();yield return"Found "+DamagedBlocks.Count+" damaged Blocks";}if(Turrets.Count>0){TurretsWithTarget.Clear();
TurretsOutOfAmmo.Clear();int c=0;foreach(IMyLargeTurretBase turret in Turrets){if(turret.HasTarget){TurretsWithTarget.Add(turret);}if(!
isEnergyWeapon(turret)&&turret.GetInventory(0).ItemCount==0){TurretsOutOfAmmo.Add(turret);}if(++c%30==0){UpdateScreenTimeAll();yield
return"Scanning Turrets "+c+" / "+Turrets.Count;}}UpdateScreenTimeAll();yield return"Scanned "+Turrets.Count+" Turrets";}
CycleTimer.NewCycle();sc.Clean();sc.AddFormat("{0} Integrity {1,4} / {2,4} § {3,3:0}%",((GlobalBlockList.Count-DamagedBlocks.Count
)/(float)GlobalBlockList.Count).ToString(),LCDUtils.WarningLCD((GlobalBlockList.Count-DamagedBlocks.Count)/(float)
GlobalBlockList.Count),GlobalBlockList.Count-DamagedBlocks.Count,GlobalBlockList.Count,(float)(GlobalBlockList.Count-DamagedBlocks.
Count)/(float)GlobalBlockList.Count*100f);for(int i=0;i<Math.Min(Conf.DamagedBlocksListLength,DamagedBlocks.Count);i++){sc.
Add(LCDUtils.CreateColor(0,0,0).ToString()+" - "+DamagedBlocks[i].DisplayNameText+" damaged");}if(DamagedBlocks.Count>Conf.
DamagedBlocksListLength){sc.Add(LCDUtils.CreateColor(0,0,0).ToString()+" - "+(DamagedBlocks.Count-Conf.DamagedBlocksListLength)+" damaged");}sc
.Add("");if(Turrets.Count>0){char turretSign=LCDUtils.INFO;if(TurretsOutOfAmmo.Count>0)turretSign=LCDUtils.WARNING;if(
TurretsWithTarget.Count>0)turretSign=LCDUtils.ERROR;sc.AddFormat("{0} {1,3} Turrets",null,turretSign,Turrets.Count);sc.AddFormat(
"{0} {1,3} Engaged § {2,3:0}%",((float)TurretsWithTarget.Count/(float)Turrets.Count).ToString(),TurretsWithTarget.Count>0?LCDUtils.ERROR:LCDUtils.INFO
,TurretsWithTarget.Count,(float)TurretsWithTarget.Count/(float)Turrets.Count*100f);sc.AddFormat(
"{0} {1,3} Armed   § {2,3:0}%",((float)(Turrets.Count-TurretsOutOfAmmo.Count)/(float)Turrets.Count).ToString(),LCDUtils.WarningLCD(TurretsOutOfAmmo.
Count>0),(Turrets.Count-TurretsOutOfAmmo.Count),(float)(Turrets.Count-TurretsOutOfAmmo.Count)/(float)Turrets.Count*100f);for(
int i=0;i<Math.Min(Conf.EmptyTurretsListLength,TurretsOutOfAmmo.Count);i++){sc.Add(LCDUtils.CreateColor(0,0,0).ToString()+
" - "+TurretsOutOfAmmo[i].DisplayNameText+" empty");}if(TurretsOutOfAmmo.Count>Conf.EmptyTurretsListLength){sc.Add(LCDUtils.
CreateColor(0,0,0).ToString()+" - "+(TurretsOutOfAmmo.Count-Conf.EmptyTurretsListLength)+" others empty");}}UpdateScreenTimeAll();
yield return"Prepared "+sc.Count+" Status-Screen Lines";}}private bool isEnergyWeapon(IMyLargeTurretBase turret){foreach(
string tag in Conf.NoAmmoWeapons)if(turret.CustomName.Contains(tag))return true;return false;}}abstract class SubRoutine{
protected CycleTime CycleTimer=new CycleTime();protected Config Conf;protected IEnumerator<string>Routine;protected List<
IMyTerminalBlock>GlobalBlockList;public Dictionary<string,Screen>Screens=new Dictionary<string,Screen>();public float progress=0;public
SubRoutine(Config newConfig){Conf=newConfig;GlobalBlockList=new List<IMyTerminalBlock>();Routine=Run();}public void Refresh(Config
newConfig,List<IMyTerminalBlock>newBlocks){Conf=newConfig;GlobalBlockList=newBlocks;RefreshLists();}public abstract void
RefreshLists();public string Next(){try{Routine.MoveNext();return Routine.Current;}catch(Exception e){Routine=Run();return
"Error: Restarting. "+e.Message;}}public abstract IEnumerator<string>Run();protected Screen InitScreen(string screen,string name){if(!Screens
.ContainsKey(screen)){Screen sc=new Screen(screen,name);Screens.Add(screen,sc);return sc;}else{return Screens[screen];}}
protected void UpdateScreenTimeAll(){foreach(Screen sc in Screens.Values)sc.SetProgressTime(CycleTimer.CurrentCycleTime,
CycleTimer.LastCycleTime);}}