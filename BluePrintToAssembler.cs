/*
 * ***BluePrint_To_Assembler***
 * 
 * Thanks for subscribing to this script!
 * ** Created by BlueLone
 * ** Version 1.0.7
 * 
 * Instructions:
 *     * Projector setup:
 *         - Add prefixKey to projector name to connect it to this script
 *         - In the customdata of the projector a series of textlines will appear (see instructions there !ONLY CHANGE VALUES!)
 *     * Assembler setup:
 *         - To enable sending to assemblers set toAssembler to true below
 *         - Add prefixKey to assmebler name to connect it to this script
 *             **Components will divide evenly between all assemblers with prefix**
 *     * LCD Setup:
 *         - To enable sending to LCD set toLCD to true below
 *         - Add prefixKey to lcd name to configure it for one projector
 *             **specific projector name can be enterd in lcd customdata (see instructions there)
 *         - Add prefixKeyLCDAll to LCD name to configure it to show the total components needed for all projectors connected to script
 *         **!! prefixKey OR prefixKeyLCDAll !! NOT BOTH !!**
 *     * Argument run:
 *         Run this program with arguments. Handy for button setup!
 *         - {projector name};{keyvalue heavy/toassembler};{Optional True/False}
 *             Description: With this argument you can change projector custom data values.
 *             Example arguments:
 *                 [BTA] Projector;heavy;true (will set type armor for projector to heavy)
 *                 [BTA] Projector 2;toassembler (will toggle if projector components must be sent to assemblers)
 *         - reset
 *             Description: Will reset all customdata values from projectors and lcds.
 */
// prefix that needs to be added to every projector, assembler or LCD you want to use with this mod
string prefixKey = "[BTA]";

// prefix that needs to be added to every lcd you want to use to see total component requirements of all projectors associated
string prefixKeyLCDAll = "[BTA-ALL]";

// Set to true if you want LCDs to show component requirements
bool toLCD = true;

// Set to true if you want to send the required components to associated assemblers
bool toAssembler = true;

// Set to true if you want to take current inventory into account
bool checkInventory = true;

//---------------------------------------------FOR ADVANCED USERS----------------------------------------
// DO NOT CHANGE EXISTING VALUES!
// if u have custom components with component subtypeid names different then their blueprint subtypeid names, add them here
Dictionary<string, string> ComponentToBlueprint = new Dictionary<string, string>(){
    {"Construction","ConstructionComponent"},
    {"Girder","GirderComponent"},
    {"Motor","MotorComponent"},
    {"Computer","ComputerComponent"},
    {"Reactor","ReactorComponent"},
    {"Thrust","ThrustComponent"},
    {"GravityGenerator","GravityGeneratorComponent"},
    {"Medical","MedicalComponent"},
    {"RadioCommunication","RadioCommunicationComponent"},
    {"Detector","DetectorComponent"},
    {"Explosives","ExplosivesComponent"}
    //,{ComponentSubtypeId,BlueprintSubtypeId}
};
// if custom components is present add new line here as example
Dictionary<int, string> ComponentsKeys = new Dictionary<int, string>(){
    {1,"SteelPlate"},
    {2,"ConstructionComponent"},
    {3,"InteriorPlate"},
    {4,"SmallTube"},
    {5,"LargeTube"},
    {6,"MotorComponent"},
    {7,"GirderComponent"},
    {8,"Display"},
    {9,"ComputerComponent"},
    {10,"Superconductor"},
    {11,"MetalGrid"},
    {12,"BulletproofGlass"},
    {13,"ReactorComponent"},
    {14,"ThrustComponent"},
    {15,"GravityGeneratorComponent"},
    {16,"MedicalComponent"},
    {17,"RadioCommunicationComponent"},
    {18,"DetectorComponent"},
    {19,"ExplosivesComponent"},
    {20,"SolarCell"},
    {21,"PowerCell"}
    //,{22,BlueprintSubtypeId}
};
// if custom blocks are present in your game u can add them here (all in lower cap)
// this should be {"gridsizePrefix_BlockComponentName",new Dictionary<int,int>() {{ComponentKey, count}}}
// gridsizePrefix: small/large, ComponentKey from list above, count number of components
// example is a block called Freezer on a small grid and it needs 15 steelplate and 50 construction components
Dictionary<string, Dictionary<int, int>> customBlocks = new Dictionary<string, Dictionary<int, int>>()
{
    //{"small_freezer",new Dictionary<int,int>(){{1,15},{2,50}}}
};
//----------------------------------------------------------------------------------------------------------------------------------
//---------------------------------------------DO NOT EDIT BEYOND THIS LINE----------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------
string ĝ ="\\";string Ĝ="";Dictionary<string,Dictionary<int,int>>ě=null;Program(){Echo("Initializing script...");Runtime.
UpdateFrequency=UpdateFrequency.Update100;}void Save(){}void Main(string Ě,UpdateType ę){ħ();try{List<IMyTerminalBlock>r=new List<
IMyTerminalBlock>();GridTerminalSystem.GetBlocks(r);Ĝ+="GridBlockSize:"+r.Count;if((ę&(UpdateType.Trigger|UpdateType.Terminal|UpdateType
.Script))!=0){if(!string.IsNullOrEmpty(Ě))Ħ(Ě,r);else return;}List<IMyProjector>w=Ô(r);Ĝ+="\nViableProjectors:"+w.Count;
Echo("Found Projectors :"+w.Count);if(w.Count<1)return;List<IMyAssembler>z=null;if(toAssembler){z=v(r);Ĝ+="\nViableAssemblers:"+z.
Count;Echo("Found Assemblers :"+z.Count);}else{Echo("AssemblerQueue disabled || enable in script");}List<IMyTextPanel>n=null;
List<IMyTextPanel>ĭ=null;if(toLCD){n=s(r);Ĝ+="\nViableLCD:"+n.Count;Echo("Found LCD's :"+n.Count);ĭ=n.FindAll(Č=>Č.CustomName.
Contains(prefixKeyLCDAll));Ĝ+="\nViableLCDAll:"+ĭ.Count;}else{Echo("LCD disabled || enable in script");}Dictionary<string,int>Ĭ=null;if(checkInventory)Ĭ=ł(
r);Dictionary<string,int>ī=new Dictionary<string,int>();Dictionary<string,int>Ī=new Dictionary<string,int>();foreach(
IMyProjector t in w){Echo("---------------------\nReading: "+t.CustomName);bool ă=false;Boolean.TryParse(ĳ(t.CustomData,ď.Ö[
"typevar"]),out ă);Echo("Armor Type: "+(ă?"Heavy":"Light"));Dictionary<string,int>Ć=ĵ(t);Ĝ+="\nBlockSizeBlueprint{"+t.CustomName+
"}:"+Ć.Sum(Ø=>Ø.Value);Dictionary<string,int>Ă=č(Ć,(t as IMyCubeBlock).BlockDefinition.ToString().Split('/')[1].Contains(
"Large")?"large_":"small_",ă);if(toLCD&&n.Count>0){List<IMyTextPanel>ĩ=q(n,t);if(ĩ.Count>0)foreach(IMyTextPanel Æ in ĩ)Ŋ(Æ,Ă,Ĭ,
false,t.CustomName);}Ī=Ŏ(Ă,Ī);if(toAssembler){bool Ĩ=false;Boolean.TryParse(ĳ(t.CustomData,ď.Ö["workingvar"]),out Ĩ);if(Ĩ){Echo(
"Components already added...");continue;}Ŀ(t,ď.Ö["workingvar"],true.ToString());ī=Ŏ(Ă,ī);}else{Echo(
"Components are not being send to assembler(s)...");continue;}}if(toLCD&&Ī.Count>0&&ĭ.Count>0)foreach(IMyTextPanel Æ in ĭ)Ŋ(Æ,Ī,Ĭ);if(ī.Count>0)if(toAssembler&&z!=null&&z.Count>0){if(checkInventory
)ī=Ō(Ĭ,ī);Ĕ(z,ī);}}catch(Exception e){Me.CustomData=Ĝ+"\nErrorMsg:"+e.Message+"\n"+e.StackTrace;if(e.InnerException!=null
)Me.CustomData+="\nInnerErrorMsg:"+e.InnerException.Message+"\n"+e.InnerException.StackTrace;}}void ħ(){switch(ĝ){case
"\\":ĝ="|";break;case"|":ĝ="/";break;case"/":ĝ="\\";break;}Ĝ="";Echo("Running...."+ĝ);if(ě==null){ě=new Dictionary<string,
Dictionary<int,int>>();ě.Add("large_lightarmorblock",new Dictionary<int,int>(){{1,25}});ě.Add("small_lightarmorblock",new
Dictionary<int,int>(){{1,1}});ě.Add("large_heavyarmorblock",new Dictionary<int,int>(){{1,150},{11,50}});ě.Add(
"small_heavyarmorblock",new Dictionary<int,int>(){{1,5},{11,2}});ě.Add("large_lightarmorpanel",new Dictionary<int,int>(){{1,5}});ě.Add(
"large_lightarmorpanelside",new Dictionary<int,int>(){{1,3}});ě.Add("large_lightarmorpanelslope",new Dictionary<int,int>(){{1,6}});ě.Add(
"large_lightarmorhalfpanel",new Dictionary<int,int>(){{1,3}});ě.Add("large_lightarmorquarterpanel",new Dictionary<int,int>(){{1,2}});ě.Add(
"large_lightarmorpanel2x1slopebase",new Dictionary<int,int>(){{1,5}});ě.Add("large_lightarmorpanel2x1slopetip",new Dictionary<int,int>(){{1,5}});ě.Add(
"large_lightarmorpanel2x1baseright",new Dictionary<int,int>(){{1,5}});ě.Add("large_lightarmorpanel2x1tipright",new Dictionary<int,int>(){{1,3}});ě.Add(
"large_lightarmorpanel2x1baseleft",new Dictionary<int,int>(){{1,5}});ě.Add("large_lightarmorpanel2x1tipleft",new Dictionary<int,int>(){{1,3}});ě.Add(
"large_lightarmorpanelhalfslope",new Dictionary<int,int>(){{1,4}});ě.Add("large_lightarmorhalfpanel2x1baseright",new Dictionary<int,int>(){{1,3}});ě.Add
("large_lightarmorhalfpanel2x1tipright",new Dictionary<int,int>(){{1,3}});ě.Add("large_lightarmorhalfpanel2x1baseleft",
new Dictionary<int,int>(){{1,3}});ě.Add("large_lightarmorhalfpanel2x1tipleft",new Dictionary<int,int>(){{1,3}});ě.Add(
"large_heavyarmorpanel",new Dictionary<int,int>(){{1,15},{11,5}});ě.Add("large_heavyarmorpanelside",new Dictionary<int,int>(){{1,8},{11,3}});ě.
Add("large_heavyarmorpanelslope",new Dictionary<int,int>(){{1,21},{11,7}});ě.Add("large_heavyarmorhalfpanel",new Dictionary
<int,int>(){{1,8},{11,3}});ě.Add("large_heavyarmorquarterpanel",new Dictionary<int,int>(){{1,5},{11,2}});ě.Add(
"large_heavyarmorpanel2x1slopebase",new Dictionary<int,int>(){{1,18},{11,6}});ě.Add("large_heavyarmorpanel2x1slopetip",new Dictionary<int,int>(){{1,18},{11
,6}});ě.Add("large_heavyarmorpanel2x1baseright",new Dictionary<int,int>(){{1,12},{11,4}});ě.Add(
"large_heavyarmorpanel2x1tipright",new Dictionary<int,int>(){{1,6},{11,2}});ě.Add("large_heavyarmorpanel2x1baseleft",new Dictionary<int,int>(){{1,12},{11,
4}});ě.Add("large_heavyarmorpanel2x1tipleft",new Dictionary<int,int>(){{1,6},{11,2}});ě.Add(
"large_heavyarmorpanelhalfslope",new Dictionary<int,int>(){{1,9},{11,3}});ě.Add("large_heavyarmorhalfpanel2x1baseright",new Dictionary<int,int>(){{1,9},
{11,3}});ě.Add("large_heavyarmorhalfpanel2x1tipright",new Dictionary<int,int>(){{1,9},{11,3}});ě.Add(
"large_heavyarmorhalfpanel2x1baseleft",new Dictionary<int,int>(){{1,9},{11,3}});ě.Add("large_heavyarmorhalfpanel2x1tipleft",new Dictionary<int,int>(){{1,9},{
11,3}});ě.Add("small_lightarmorpanel",new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorpanelside",new Dictionary<
int,int>(){{1,1}});ě.Add("small_lightarmorpanelslope",new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorhalfpanel",
new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorquarterpanel",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_lightarmorpanel2x1slopebase",new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorpanel2x1slopetip",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_lightarmorpanel2x1baseright",new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorpanel2x1tipright",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_lightarmorpanel2x1baseleft",new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorpanel2x1tipleft",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_lightarmorpanelhalfslope",new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorhalfpanel2x1baseright",new Dictionary<int,int>(){{1,1}});ě.Add
("small_lightarmorhalfpanel2x1tipright",new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorhalfpanel2x1baseleft",
new Dictionary<int,int>(){{1,1}});ě.Add("small_lightarmorhalfpanel2x1tipleft",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_heavyarmorpanel",new Dictionary<int,int>(){{1,3},{11,1}});ě.Add("small_heavyarmorpanelside",new Dictionary<int,int>(){{1,2},{11,1}});ě.
Add("small_heavyarmorpanelslope",new Dictionary<int,int>(){{1,3},{11,1}});ě.Add("small_heavyarmorhalfpanel",new Dictionary<
int,int>(){{1,2},{11,1}});ě.Add("small_heavyarmorquarterpanel",new Dictionary<int,int>(){{1,2},{11,1}});ě.Add(
"small_heavyarmorpanel2x1slopebase",new Dictionary<int,int>(){{1,3},{11,1}});ě.Add("small_heavyarmorpanel2x1slopetip",new Dictionary<int,int>(){{1,3},{11,1
}});ě.Add("small_heavyarmorpanel2x1baseright",new Dictionary<int,int>(){{1,3},{11,1}});ě.Add(
"small_heavyarmorpanel2x1tipright",new Dictionary<int,int>(){{1,2},{11,1}});ě.Add("small_heavyarmorpanel2x1baseleft",new Dictionary<int,int>(){{1,3},{11,1
}});ě.Add("small_heavyarmorpanel2x1tipleft",new Dictionary<int,int>(){{1,2},{11,1}});ě.Add(
"small_heavyarmorpanelhalfslope",new Dictionary<int,int>(){{1,2},{11,1}});ě.Add("small_heavyarmorhalfpanel2x1baseright",new Dictionary<int,int>(){{1,2},
{11,1}});ě.Add("small_heavyarmorhalfpanel2x1tipright",new Dictionary<int,int>(){{1,2},{11,1}});ě.Add(
"small_heavyarmorhalfpanel2x1baseleft",new Dictionary<int,int>(){{1,2},{11,1}});ě.Add("small_heavyarmorhalfpanel2x1tipleft",new Dictionary<int,int>(){{1,2},{
11,1}});ě.Add("small_programmableblock",new Dictionary<int,int>(){{1,2},{2,2},{5,2},{6,1},{8,1},{9,2}});ě.Add(
"large_projector",new Dictionary<int,int>(){{1,21},{2,4},{5,2},{6,1},{9,2}});ě.Add("small_projector",new Dictionary<int,int>(){{1,2},{2,2
},{5,2},{6,1},{9,2}});ě.Add("small_sensor",new Dictionary<int,int>(){{3,5},{2,5},{9,6},{17,4},{18,6},{1,2}});ě.Add(
"large_sensor",new Dictionary<int,int>(){{3,5},{2,5},{9,6},{17,4},{18,6},{1,2}});ě.Add("large_targetdummy",new Dictionary<int,int>(){{
1,15},{4,10},{6,2},{9,4},{8,1}});ě.Add("small_soundblock",new Dictionary<int,int>(){{3,4},{2,6},{9,3}});ě.Add(
"large_soundblock",new Dictionary<int,int>(){{3,4},{2,6},{9,3}});ě.Add("large_buttonpanel",new Dictionary<int,int>(){{3,10},{2,20},{9,20}}
);ě.Add("small_buttonpanel",new Dictionary<int,int>(){{3,2},{2,2},{9,1}});ě.Add("large_timerblock",new Dictionary<int,int
>(){{3,6},{2,30},{9,5}});ě.Add("small_timerblock",new Dictionary<int,int>(){{3,2},{2,3},{9,1}});ě.Add(
"large_programmableblock",new Dictionary<int,int>(){{1,21},{2,4},{5,2},{6,1},{8,1},{9,2}});ě.Add("large_customturretcontroller",new Dictionary<
int,int>(){{3,20},{2,30},{18,20},{6,4},{8,6},{9,20},{1,20}});ě.Add("small_customturretcontroller",new Dictionary<int,int>()
{{3,4},{2,10},{18,4},{6,2},{8,1},{9,10},{1,4}});ě.Add("large_antenna",new Dictionary<int,int>(){{1,80},{5,40},{4,60},{2,
30},{9,8},{17,40}});ě.Add("large_beacon",new Dictionary<int,int>(){{1,80},{2,30},{5,20},{9,10},{17,40}});ě.Add(
"small_beacon",new Dictionary<int,int>(){{1,2},{2,1},{4,1},{9,1},{17,4}});ě.Add("small_antenna",new Dictionary<int,int>(){{1,1},{4,1},
{2,2},{9,1},{17,4}});ě.Add("large_remotecontrol",new Dictionary<int,int>(){{3,10},{2,10},{6,1},{9,15}});ě.Add(
"small_remotecontrol",new Dictionary<int,int>(){{3,2},{2,1},{6,1},{9,1}});ě.Add("large_laserantenna",new Dictionary<int,int>(){{1,50},{2,40},
{6,16},{18,30},{17,20},{10,100},{9,50},{12,4}});ě.Add("small_laserantenna",new Dictionary<int,int>(){{1,10},{4,10},{2,10}
,{6,5},{17,5},{10,10},{9,30},{12,2}});ě.Add("large_controlpanel",new Dictionary<int,int>(){{1,1},{2,1},{9,1},{8,1}});ě.
Add("small_controlpanel",new Dictionary<int,int>(){{1,1},{2,1},{9,1},{8,1}});ě.Add("large_controlstations",new Dictionary<
int,int>(){{3,20},{2,20},{6,2},{9,100},{8,10}});ě.Add("large_cockpit",new Dictionary<int,int>(){{1,30},{2,20},{6,1},{8,8},{
9,100},{12,60}});ě.Add("small_cockpit",new Dictionary<int,int>(){{1,10},{2,10},{6,1},{8,5},{9,15},{12,30}});ě.Add(
"small_fightercockpit",new Dictionary<int,int>(){{2,20},{6,1},{1,20},{11,10},{3,15},{8,4},{9,20},{12,40}});ě.Add("large_flightseat",new
Dictionary<int,int>(){{3,20},{2,20},{6,2},{9,100},{8,4}});ě.Add("small_rovercockpit",new Dictionary<int,int>(){{3,30},{2,25},{6,2}
,{9,20},{8,4}});ě.Add("large_gyroscope",new Dictionary<int,int>(){{1,600},{2,40},{5,4},{11,50},{6,4},{9,5}});ě.Add(
"small_gyroscope",new Dictionary<int,int>(){{1,25},{2,5},{5,1},{6,2},{9,3}});ě.Add("small_controlseat",new Dictionary<int,int>(){{3,20},{
2,20},{6,1},{9,15},{8,2}});ě.Add("large_controlseat",new Dictionary<int,int>(){{3,30},{2,30},{6,2},{9,100},{8,6}});ě.Add(
"large_desk",new Dictionary<int,int>(){{3,30},{2,30}});ě.Add("large_deskcorner",new Dictionary<int,int>(){{3,20},{2,20}});ě.Add(
"large_deskchairless",new Dictionary<int,int>(){{3,30},{2,30}});ě.Add("large_deskchairlesscorner",new Dictionary<int,int>(){{3,20},{2,20}});ě
.Add("large_kitchen",new Dictionary<int,int>(){{3,20},{2,30},{5,6},{6,6},{12,4}});ě.Add("large_bed",new Dictionary<int,
int>(){{3,30},{2,30},{4,8},{12,10}});ě.Add("large_armory",new Dictionary<int,int>(){{3,30},{2,30},{8,4},{12,10}});ě.Add(
"large_armorylockers",new Dictionary<int,int>(){{3,25},{2,30},{8,4},{12,10}});ě.Add("large_planters",new Dictionary<int,int>(){{3,10},{2,20},
{4,8},{12,8}});ě.Add("large_couch",new Dictionary<int,int>(){{3,30},{2,30}});ě.Add("large_couchcorner",new Dictionary<int
,int>(){{3,35},{2,35}});ě.Add("large_lockers",new Dictionary<int,int>(){{3,20},{2,20},{8,3},{9,2}});ě.Add(
"large_bathroomopen",new Dictionary<int,int>(){{3,30},{2,30},{4,8},{6,4},{5,2}});ě.Add("large_bathroom",new Dictionary<int,int>(){{3,30},{2,
40},{4,8},{6,4},{5,2}});ě.Add("large_toilet",new Dictionary<int,int>(){{3,10},{2,15},{4,2},{6,2},{5,1}});ě.Add(
"large_consoleblock",new Dictionary<int,int>(){{3,20},{2,30},{9,8},{8,10}});ě.Add("small_cockpitindustrial",new Dictionary<int,int>(){{1,10}
,{2,20},{11,10},{6,2},{8,6},{9,20},{12,60},{4,10}});ě.Add("large_cockpitindustrial",new Dictionary<int,int>(){{1,20},{2,
30},{11,15},{6,2},{8,10},{9,60},{12,80},{4,10}});ě.Add("large_fooddispenser",new Dictionary<int,int>(){{3,20},{2,10},{6,4}
,{8,10},{9,10}});ě.Add("large_jukebox",new Dictionary<int,int>(){{3,15},{2,10},{9,4},{8,4}});ě.Add("large_labequipment",
new Dictionary<int,int>(){{3,15},{2,15},{6,1},{12,4}});ě.Add("large_shower",new Dictionary<int,int>(){{3,20},{2,20},{4,12},
{12,8}});ě.Add("large_windowwall",new Dictionary<int,int>(){{1,8},{2,10},{12,10}});ě.Add("large_windowwallleft",new
Dictionary<int,int>(){{1,10},{2,10},{12,8}});ě.Add("large_windowwallright",new Dictionary<int,int>(){{1,10},{2,10},{12,8}});ě.Add(
"large_medicalstation",new Dictionary<int,int>(){{3,15},{2,15},{6,2},{16,1},{8,2}});ě.Add("large_transparentlcd",new Dictionary<int,int>(){{2,
8},{9,6},{8,10},{12,10}});ě.Add("small_transparentlcd",new Dictionary<int,int>(){{2,4},{9,4},{8,3},{12,1}});ě.Add(
"large_gratedcatwalk",new Dictionary<int,int>(){{2,16},{7,4},{4,20}});ě.Add("large_gratedcatwalkcorner",new Dictionary<int,int>(){{2,24},{7,4
},{4,32}});ě.Add("large_gratedcatwalkstraight",new Dictionary<int,int>(){{2,24},{7,4},{4,32}});ě.Add(
"large_gratedcatwalkwall",new Dictionary<int,int>(){{2,20},{7,4},{4,26}});ě.Add("large_gratedcatwalkrailingend",new Dictionary<int,int>(){{2,28},
{7,4},{4,38}});ě.Add("large_gratedcatwalkrailinghalfright",new Dictionary<int,int>(){{2,28},{7,4},{4,36}});ě.Add(
"large_gratedcatwalkrailinghalfleft",new Dictionary<int,int>(){{2,28},{7,4},{4,36}});ě.Add("large_gratedstairs",new Dictionary<int,int>(){{2,22},{4,12},{3,
16}});ě.Add("large_gratedhalfstairs",new Dictionary<int,int>(){{2,20},{4,6},{3,8}});ě.Add("large_gratedhalfstairsmirrored"
,new Dictionary<int,int>(){{2,20},{4,6},{3,8}});ě.Add("large_railingstraight",new Dictionary<int,int>(){{2,8},{4,6}});ě.
Add("large_railingdouble",new Dictionary<int,int>(){{2,16},{4,12}});ě.Add("large_railingcorner",new Dictionary<int,int>(){{
2,16},{4,12}});ě.Add("large_railingdiagonal",new Dictionary<int,int>(){{2,12},{4,9}});ě.Add("large_railinghalfright",new
Dictionary<int,int>(){{2,8},{4,4}});ě.Add("large_railinghalfleft",new Dictionary<int,int>(){{2,8},{4,4}});ě.Add(
"large_rotatinglight",new Dictionary<int,int>(){{2,3},{6,1}});ě.Add("small_rotatinglight",new Dictionary<int,int>(){{2,3},{6,1}});ě.Add(
"large_freight1",new Dictionary<int,int>(){{3,6},{2,8}});ě.Add("large_freight2",new Dictionary<int,int>(){{3,12},{2,16}});ě.Add(
"large_freight3",new Dictionary<int,int>(){{3,18},{2,24}});ě.Add("large_door",new Dictionary<int,int>(){{3,10},{2,40},{4,4},{6,2},{8,1},
{9,2},{1,8}});ě.Add("small_door",new Dictionary<int,int>(){{3,8},{2,30},{4,4},{6,2},{8,1},{9,2},{1,6}});ě.Add(
"large_airtighthangardoor",new Dictionary<int,int>(){{1,350},{2,40},{4,40},{6,16},{9,2}});ě.Add("large_slidingdoor",new Dictionary<int,int>(){{1,
20},{2,40},{4,4},{6,4},{8,1},{9,2},{12,15}});ě.Add("large_blastdoor",new Dictionary<int,int>(){{1,140}});ě.Add(
"large_blastdoorcorner",new Dictionary<int,int>(){{1,120}});ě.Add("large_blastdoorcornerinverted",new Dictionary<int,int>(){{1,135}});ě.Add(
"large_blastdooredge",new Dictionary<int,int>(){{1,130}});ě.Add("small_blastdoor",new Dictionary<int,int>(){{1,5}});ě.Add(
"small_blastdoorcorner",new Dictionary<int,int>(){{1,5}});ě.Add("small_blastdoorcornerinverted",new Dictionary<int,int>(){{1,5}});ě.Add(
"small_blastdooredge",new Dictionary<int,int>(){{1,5}});ě.Add("large_store",new Dictionary<int,int>(){{1,30},{2,20},{6,6},{8,4},{9,10}});ě.
Add("large_safezone",new Dictionary<int,int>(){{1,800},{2,180},{15,10},{-1,5},{11,80},{9,120}});ě.Add("large_contract",new
Dictionary<int,int>(){{1,30},{2,20},{6,6},{8,4},{9,10}});ě.Add("large_vendingmachine",new Dictionary<int,int>(){{3,20},{2,10},{6,4
},{8,4},{9,10}});ě.Add("large_atm",new Dictionary<int,int>(){{1,20},{2,20},{6,2},{9,10},{8,4}});ě.Add("large_battery",new
Dictionary<int,int>(){{1,80},{2,30},{21,80},{9,25}});ě.Add("small_battery",new Dictionary<int,int>(){{1,25},{2,5},{21,20},{9,2}});
ě.Add("small_smallbattery",new Dictionary<int,int>(){{1,4},{2,2},{21,2},{9,2}});ě.Add("small_smallreactor",new Dictionary
<int,int>(){{1,3},{2,10},{11,2},{5,1},{13,3},{6,1},{9,10}});ě.Add("small_largereactor",new Dictionary<int,int>(){{1,60},{
2,9},{11,9},{5,3},{13,95},{6,5},{9,25}});ě.Add("large_smallreactor",new Dictionary<int,int>(){{1,80},{2,40},{11,4},{5,8},
{13,100},{6,6},{9,25}});ě.Add("large_largereactor",new Dictionary<int,int>(){{1,1000},{2,70},{11,40},{5,40},{10,100},{13,
2000},{6,20},{9,75}});ě.Add("large_hydrogenengine",new Dictionary<int,int>(){{1,100},{2,70},{5,12},{4,20},{6,12},{9,4},{21,1
}});ě.Add("small_hydrogenengine",new Dictionary<int,int>(){{1,30},{2,20},{5,4},{4,6},{6,4},{9,1},{21,1}});ě.Add(
"large_windturbine",new Dictionary<int,int>(){{3,40},{6,8},{2,20},{7,24},{9,2}});ě.Add("large_solarpanel",new Dictionary<int,int>(){{1,4},{
2,14},{7,12},{9,4},{20,32},{12,4}});ě.Add("small_solarpanel",new Dictionary<int,int>(){{1,2},{2,2},{7,4},{9,1},{20,8},{12
,1}});ě.Add("large_monolith",new Dictionary<int,int>(){{1,130},{10,130}});ě.Add("large_stereolith",new Dictionary<int,int
>(){{1,130},{10,130}});ě.Add("small_deadastronaut",new Dictionary<int,int>(){{1,13},{10,13}});ě.Add("large_deadastronaut"
,new Dictionary<int,int>(){{1,13},{10,13}});ě.Add("large_antennadish",new Dictionary<int,int>(){{2,40},{7,120},{1,80},{9,
8},{17,40}});ě.Add("large_gate",new Dictionary<int,int>(){{1,800},{2,100},{4,100},{6,20},{9,10}});ě.Add(
"large_offsetdoor",new Dictionary<int,int>(){{1,25},{2,35},{4,4},{6,4},{8,1},{9,2},{12,6}});ě.Add("large_deadbody01",new Dictionary<int,
int>(){{12,1},{17,1},{8,1}});ě.Add("large_deadbody02",new Dictionary<int,int>(){{12,1},{17,1},{8,1}});ě.Add(
"large_deadbody03",new Dictionary<int,int>(){{12,1},{17,1},{8,1}});ě.Add("large_deadbody04",new Dictionary<int,int>(){{12,1},{17,1},{8,1}}
);ě.Add("large_deadbody05",new Dictionary<int,int>(){{12,1},{17,1},{8,1}});ě.Add("large_deadbody06",new Dictionary<int,
int>(){{12,1},{17,1},{8,1}});ě.Add("large_gravitygenerator",new Dictionary<int,int>(){{1,150},{15,6},{2,60},{5,4},{6,6},{9,
40}});ě.Add("large_gravitygeneratorsphere",new Dictionary<int,int>(){{1,150},{15,6},{2,60},{5,4},{6,6},{9,40}});ě.Add(
"large_virtualmass",new Dictionary<int,int>(){{1,90},{10,20},{2,30},{9,20},{15,9}});ě.Add("small_virtualmass",new Dictionary<int,int>(){{1,
3},{10,2},{2,2},{9,2},{15,1}});ě.Add("large_spaceball",new Dictionary<int,int>(){{1,225},{2,30},{9,20},{15,3}});ě.Add(
"small_spaceball",new Dictionary<int,int>(){{1,70},{2,10},{9,7},{15,1}});ě.Add("large_magneticplatelarge",new Dictionary<int,int>(){{1,
450},{2,60},{6,20}});ě.Add("small_magneticplatelarge",new Dictionary<int,int>(){{1,6},{2,15},{6,3}});ě.Add(
"large_largeindustrialcargocontainer",new Dictionary<int,int>(){{3,360},{2,80},{11,24},{4,60},{6,20},{8,1},{9,8}});ě.Add("large_verticalbuttonpanel",new
Dictionary<int,int>(){{3,5},{2,10},{9,5}});ě.Add("small_verticalbuttonpanel",new Dictionary<int,int>(){{3,5},{2,10},{9,5}});ě.Add(
"large_conveyorpipe",new Dictionary<int,int>(){{3,14},{2,20},{4,12},{6,6}});ě.Add("large_conveyorpipecurved",new Dictionary<int,int>(){{3,14
},{2,20},{4,12},{6,6}});ě.Add("large_conveyorpipejunction",new Dictionary<int,int>(){{3,20},{2,30},{4,20},{6,6}});ě.Add(
"large_conveyorpipecross",new Dictionary<int,int>(){{3,18},{2,20},{4,16},{6,6}});ě.Add("large_conveyorpipeflanged",new Dictionary<int,int>(){{3,
14},{2,20},{4,12},{6,6}});ě.Add("large_conveyorpipeend",new Dictionary<int,int>(){{3,14},{2,20},{4,12},{6,6}});ě.Add(
"large_industrialhydrogentank",new Dictionary<int,int>(){{1,280},{5,80},{4,60},{9,8},{2,40}});ě.Add("large_industrialassembler",new Dictionary<int,int
>(){{1,140},{2,80},{6,20},{8,10},{11,10},{9,160}});ě.Add("large_industrialrefinery",new Dictionary<int,int>(){{1,1200},{2
,40},{5,20},{6,16},{11,20},{9,20}});ě.Add("large_cylindricalcolumn",new Dictionary<int,int>(){{3,25},{2,10}});ě.Add(
"small_cylindricalcolumn",new Dictionary<int,int>(){{3,5},{2,3}});ě.Add("large_industrialconveyorsorter",new Dictionary<int,int>(){{3,50},{2,120}
,{4,50},{9,20},{6,2}});ě.Add("large_beamblock",new Dictionary<int,int>(){{1,25}});ě.Add("large_beamblockslope",new
Dictionary<int,int>(){{1,13}});ě.Add("large_beamblockround",new Dictionary<int,int>(){{1,13}});ě.Add("large_beamblock2x1base",new
Dictionary<int,int>(){{1,19}});ě.Add("large_beamblock2x1tip",new Dictionary<int,int>(){{1,7}});ě.Add("large_beamblockhalf",new
Dictionary<int,int>(){{1,12}});ě.Add("large_beamblockhalfslope",new Dictionary<int,int>(){{1,7}});ě.Add("large_beamblockend",new
Dictionary<int,int>(){{1,25}});ě.Add("large_beamblockjunction",new Dictionary<int,int>(){{1,25}});ě.Add("large_beamblocktjunction"
,new Dictionary<int,int>(){{1,25}});ě.Add("small_beamblock",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_beamblockslope",new Dictionary<int,int>(){{1,1}});ě.Add("small_beamblockround",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_beamblock2x1base",new Dictionary<int,int>(){{1,1}});ě.Add("small_beamblock2x1tip",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_beamblockhalf",new Dictionary<int,int>(){{1,1}});ě.Add("small_beamblockhalfslope",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_beamblockend",new Dictionary<int,int>(){{1,1}});ě.Add("small_beamblockjunction",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_beamblocktjunction",new Dictionary<int,int>(){{1,1}});ě.Add("large_industriallargehydrogenthruster",new Dictionary<int,int>(){{1,150},{2,
180},{11,250},{5,40}});ě.Add("large_industrialhydrogenthruster",new Dictionary<int,int>(){{1,25},{2,60},{11,40},{5,8}});ě.
Add("small_industriallargehydrogenthruster",new Dictionary<int,int>(){{1,30},{2,30},{11,22},{5,10}});ě.Add(
"small_industrialhydrogenthruster",new Dictionary<int,int>(){{1,7},{2,15},{11,4},{5,2}});ě.Add("large_passage",new Dictionary<int,int>(){{3,74},{2,20},{4,
48}});ě.Add("large_passage2",new Dictionary<int,int>(){{3,74},{2,20},{4,48}});ě.Add("large_passage2side",new Dictionary<
int,int>(){{3,50},{2,14},{4,32}});ě.Add("large_stairs",new Dictionary<int,int>(){{3,50},{2,30}});ě.Add("large_ramp",new
Dictionary<int,int>(){{3,70},{2,16}});ě.Add("large_steelcatwalk",new Dictionary<int,int>(){{3,27},{2,5},{4,20}});ě.Add(
"large_steelcatwalktwosides",new Dictionary<int,int>(){{3,32},{2,7},{4,25}});ě.Add("large_steelcatwalkcorner",new Dictionary<int,int>(){{3,32},{2,7}
,{4,25}});ě.Add("large_steelcatwalkplate",new Dictionary<int,int>(){{3,23},{2,7},{4,17}});ě.Add("large_fullcoverwall",new
Dictionary<int,int>(){{1,4},{2,10}});ě.Add("large_halfcoverwall",new Dictionary<int,int>(){{1,2},{2,6}});ě.Add(
"large_interiorwall",new Dictionary<int,int>(){{3,25},{2,10}});ě.Add("large_interiorpillar",new Dictionary<int,int>(){{3,25},{2,10},{4,4}});
ě.Add("large_passengerseat",new Dictionary<int,int>(){{3,20},{2,20}});ě.Add("small_passengerseat",new Dictionary<int,int>
(){{3,20},{2,20}});ě.Add("small_passengerseatoffset",new Dictionary<int,int>(){{3,20},{2,20}});ě.Add("large_ladder",new
Dictionary<int,int>(){{3,10},{2,20},{4,10}});ě.Add("small_ladder",new Dictionary<int,int>(){{3,10},{2,20},{4,10}});ě.Add(
"small_textpanel",new Dictionary<int,int>(){{3,1},{2,4},{9,4},{8,3},{12,1}});ě.Add("small_lcdpanelwide",new Dictionary<int,int>(){{3,1},{
2,8},{9,8},{8,6},{12,2}});ě.Add("small_lcdpanel",new Dictionary<int,int>(){{3,1},{2,4},{9,4},{8,3},{12,2}});ě.Add(
"large_cornerlcdtop",new Dictionary<int,int>(){{2,5},{9,3},{8,1}});ě.Add("large_cornerlcdbottom",new Dictionary<int,int>(){{2,5},{9,3},{8,1}
});ě.Add("large_cornerlcdflattop",new Dictionary<int,int>(){{2,5},{9,3},{8,1}});ě.Add("large_cornerlcdflatbottom",new
Dictionary<int,int>(){{2,5},{9,3},{8,1}});ě.Add("small_cornerlcdtop",new Dictionary<int,int>(){{2,3},{9,2},{8,1}});ě.Add(
"small_cornerlcdbottom",new Dictionary<int,int>(){{2,3},{9,2},{8,1}});ě.Add("small_cornerlcdflattop",new Dictionary<int,int>(){{2,3},{9,2},{8,1
}});ě.Add("small_cornerlcdflatbottom",new Dictionary<int,int>(){{2,3},{9,2},{8,1}});ě.Add("large_textpanel",new
Dictionary<int,int>(){{3,1},{2,6},{9,6},{8,10},{12,2}});ě.Add("large_lcdpanel",new Dictionary<int,int>(){{3,1},{2,6},{9,6},{8,10},
{12,6}});ě.Add("large_lcdpanelwide",new Dictionary<int,int>(){{3,2},{2,12},{9,12},{8,20},{12,12}});ě.Add(
"large_spotlight",new Dictionary<int,int>(){{1,8},{5,2},{3,20},{2,15},{12,4}});ě.Add("small_spotlight",new Dictionary<int,int>(){{1,1},{5
,1},{3,1},{2,1},{12,2}});ě.Add("large_interiorlight",new Dictionary<int,int>(){{2,2}});ě.Add("small_interiorlight",new
Dictionary<int,int>(){{2,2}});ě.Add("large_cornerlight",new Dictionary<int,int>(){{2,3}});ě.Add("large_cornerlightdouble",new
Dictionary<int,int>(){{2,6}});ě.Add("small_cornerlight",new Dictionary<int,int>(){{2,2}});ě.Add("small_cornerlightdouble",new
Dictionary<int,int>(){{2,4}});ě.Add("small_oxygentank",new Dictionary<int,int>(){{1,16},{5,8},{4,10},{9,8},{2,10}});ě.Add(
"large_oxygentank",new Dictionary<int,int>(){{1,80},{5,40},{4,60},{9,8},{2,40}});ě.Add("large_hydrogentank",new Dictionary<int,int>(){{1,
280},{5,80},{4,60},{9,8},{2,40}});ě.Add("large_hydrogentanksmall",new Dictionary<int,int>(){{1,80},{5,40},{4,60},{9,8},{2,
40}});ě.Add("small_hydrogentank",new Dictionary<int,int>(){{1,40},{5,20},{4,30},{9,4},{2,20}});ě.Add(
"small_hydrogentanksmall",new Dictionary<int,int>(){{1,3},{5,1},{4,2},{9,4},{2,2}});ě.Add("large_airvent",new Dictionary<int,int>(){{1,45},{2,20}
,{6,10},{9,5}});ě.Add("small_airvent",new Dictionary<int,int>(){{1,8},{2,10},{6,2},{9,5}});ě.Add(
"small_smallcargocontainer",new Dictionary<int,int>(){{3,3},{2,1},{9,1},{6,1},{8,1}});ě.Add("small_mediumcargocontainer",new Dictionary<int,int>(){
{3,30},{2,10},{9,4},{6,4},{8,1}});ě.Add("small_largecargocontainer",new Dictionary<int,int>(){{3,75},{2,25},{9,6},{6,8},{
8,1}});ě.Add("large_smallcargocontainer",new Dictionary<int,int>(){{3,40},{2,40},{11,4},{4,20},{6,4},{8,1},{9,2}});ě.Add(
"large_largecargocontainer",new Dictionary<int,int>(){{3,360},{2,80},{11,24},{4,60},{6,20},{8,1},{9,8}});ě.Add("small_smallconveyor",new Dictionary
<int,int>(){{3,4},{2,4},{6,1}});ě.Add("large_conveyorjunction",new Dictionary<int,int>(){{3,20},{2,30},{4,20},{6,6}});ě.
Add("large_collector",new Dictionary<int,int>(){{1,45},{2,50},{4,12},{6,8},{8,4},{9,10}});ě.Add("small_collector",new
Dictionary<int,int>(){{1,35},{2,35},{4,12},{6,8},{8,2},{9,8}});ě.Add("large_connector",new Dictionary<int,int>(){{1,150},{2,40},{4
,12},{6,8},{9,20}});ě.Add("small_ejector",new Dictionary<int,int>(){{1,7},{2,4},{4,2},{6,1},{9,4}});ě.Add(
"small_connector",new Dictionary<int,int>(){{1,21},{2,12},{4,6},{6,6},{9,6}});ě.Add("large_conveyortube",new Dictionary<int,int>(){{3,14}
,{2,20},{4,12},{6,6}});ě.Add("small_smallconveyortube",new Dictionary<int,int>(){{3,1},{6,1},{2,1}});ě.Add(
"small_mediumconveyortube",new Dictionary<int,int>(){{3,10},{2,20},{4,10},{6,6}});ě.Add("small_conveyorframe",new Dictionary<int,int>(){{3,5},{2,
12},{4,5},{6,2}});ě.Add("large_curvedconveyortube",new Dictionary<int,int>(){{3,14},{2,20},{4,12},{6,6}});ě.Add(
"small_smallcurvedconveyortube",new Dictionary<int,int>(){{3,1},{6,1},{2,1}});ě.Add("small_curvedconveyortube",new Dictionary<int,int>(){{3,7},{2,20},{
4,10},{6,6}});ě.Add("small_conveyorjunction",new Dictionary<int,int>(){{3,15},{2,20},{4,15},{6,2}});ě.Add(
"large_conveyorsorter",new Dictionary<int,int>(){{3,50},{2,120},{4,50},{9,20},{6,2}});ě.Add("small_conveyorsorter",new Dictionary<int,int>(){{
3,5},{2,12},{4,5},{9,5},{6,2}});ě.Add("small_smallconveyorsorter",new Dictionary<int,int>(){{3,5},{2,12},{4,5},{9,5},{6,2
}});ě.Add("large_piston",new Dictionary<int,int>(){{1,25},{2,10},{5,12},{6,4},{9,2}});ě.Add("large_pistontop",new
Dictionary<int,int>(){{1,10},{5,8}});ě.Add("small_piston",new Dictionary<int,int>(){{1,8},{2,4},{4,4},{6,2},{9,1},{5,2}});ě.Add(
"small_pistontop",new Dictionary<int,int>(){{1,4},{5,2}});ě.Add("large_rotor",new Dictionary<int,int>(){{1,45},{2,10},{5,10},{6,4},{9,2}}
);ě.Add("large_rotorpart",new Dictionary<int,int>(){{1,30},{5,6}});ě.Add("small_rotor",new Dictionary<int,int>(){{1,17},{
2,5},{4,7},{6,1},{9,1}});ě.Add("small_rotorpart",new Dictionary<int,int>(){{1,12},{4,6}});ě.Add("large_advancedrotor",new
Dictionary<int,int>(){{1,57},{2,18},{5,18},{6,4},{9,2}});ě.Add("large_advancedrotorpart",new Dictionary<int,int>(){{1,30},{5,10}})
;ě.Add("small_advancedrotor",new Dictionary<int,int>(){{1,44},{2,11},{4,1},{6,1},{9,1},{5,13}});ě.Add("large_hinge",new
Dictionary<int,int>(){{1,16},{2,10},{5,4},{6,4},{9,2}});ě.Add("large_largehingehead",new Dictionary<int,int>(){{1,12},{5,4},{2,8}}
);ě.Add("small_hingepart3x3",new Dictionary<int,int>(){{1,10},{2,6},{5,2},{6,2},{9,2}});ě.Add("small_mediumhingehead",new
Dictionary<int,int>(){{1,6},{5,2},{2,4}});ě.Add("small_hinge",new Dictionary<int,int>(){{1,6},{2,4},{5,1},{6,2},{9,2}});ě.Add(
"small_smallhingehead",new Dictionary<int,int>(){{1,3},{5,1},{2,2}});ě.Add("large_medicalroom",new Dictionary<int,int>(){{3,240},{2,80},{11,60
},{4,20},{5,5},{8,10},{9,10},{16,15}});ě.Add("large_cryochamber",new Dictionary<int,int>(){{3,40},{2,20},{6,8},{8,8},{16,
3},{9,30},{12,10}});ě.Add("small_cryochamber",new Dictionary<int,int>(){{3,20},{2,10},{6,4},{8,4},{16,3},{9,15},{12,5}});
ě.Add("large_refinery",new Dictionary<int,int>(){{1,1200},{2,40},{5,20},{6,16},{11,20},{9,20}});ě.Add(
"large_basicrefinery",new Dictionary<int,int>(){{1,120},{2,20},{6,10},{9,10}});ě.Add("large_o2h2generator",new Dictionary<int,int>(){{1,120},
{2,5},{5,2},{6,4},{9,5}});ě.Add("small_o2h2generator",new Dictionary<int,int>(){{1,8},{2,8},{5,2},{6,1},{9,3}});ě.Add(
"large_assembler",new Dictionary<int,int>(){{1,140},{2,80},{6,20},{8,10},{11,10},{9,160}});ě.Add("large_basicassembler",new Dictionary<
int,int>(){{1,80},{2,40},{6,10},{8,4},{9,80}});ě.Add("large_survivalkit",new Dictionary<int,int>(){{1,30},{2,2},{16,3},{6,4
},{8,1},{9,5}});ě.Add("small_survivalkit",new Dictionary<int,int>(){{1,6},{2,2},{16,3},{6,4},{8,1},{9,5}});ě.Add(
"large_oxygenfarm",new Dictionary<int,int>(){{1,40},{12,100},{5,20},{4,10},{2,20},{9,20}});ě.Add("large_speedmodule",new Dictionary<int,
int>(){{1,100},{2,40},{4,20},{9,60},{6,4}});ě.Add("large_yieldmodule",new Dictionary<int,int>(){{1,100},{2,50},{4,15},{10,
20},{6,4}});ě.Add("large_powerefficiencymodule",new Dictionary<int,int>(){{1,100},{2,40},{4,20},{21,20},{6,4}});ě.Add(
"small_exhaustpipe",new Dictionary<int,int>(){{1,2},{2,1},{4,2},{6,2}});ě.Add("large_exhaustpipe",new Dictionary<int,int>(){{1,15},{2,10},{
5,2},{6,4}});ě.Add("small_buggycockpit",new Dictionary<int,int>(){{3,30},{2,25},{6,2},{9,20},{8,4}});ě.Add(
"large_viewport1",new Dictionary<int,int>(){{1,10},{2,10},{12,8}});ě.Add("large_viewport2",new Dictionary<int,int>(){{1,10},{2,10},{12,8}
});ě.Add("large_offroadwheelsuspension3x3left",new Dictionary<int,int>(){{1,95},{2,65},{5,26},{4,12},{6,6}});ě.Add(
"large_offroadwheelsuspension5x5left",new Dictionary<int,int>(){{1,200},{2,110},{5,50},{4,30},{6,20}});ě.Add("large_offroadwheelsuspension1x1left",new
Dictionary<int,int>(){{1,55},{2,45},{5,16},{4,12},{6,6}});ě.Add("small_offroadwheelsuspension3x3left",new Dictionary<int,int>(){{1
,16},{2,22},{4,2},{6,1},{5,3}});ě.Add("small_offroadwheelsuspension5x5left",new Dictionary<int,int>(){{1,31},{2,37},{4,4}
,{6,2},{5,5}});ě.Add("small_offroadwheelsuspension1x1left",new Dictionary<int,int>(){{1,10},{2,12},{4,2},{6,1},{5,1}});ě.
Add("large_offroadwheelsuspension3x3right",new Dictionary<int,int>(){{1,95},{2,65},{5,26},{4,12},{6,6}});ě.Add(
"large_offroadwheelsuspension5x5right",new Dictionary<int,int>(){{1,200},{2,110},{5,50},{4,30},{6,20}});ě.Add("large_offroadwheelsuspension1x1right",new
Dictionary<int,int>(){{1,55},{2,45},{5,16},{4,12},{6,6}});ě.Add("small_offroadwheelsuspension3x3right",new Dictionary<int,int>(){{
1,16},{2,22},{4,2},{6,1},{5,3}});ě.Add("small_offroadwheelsuspension5x5right",new Dictionary<int,int>(){{1,31},{2,37},{4,
4},{6,2},{5,5}});ě.Add("small_offroadwheelsuspension1x1right",new Dictionary<int,int>(){{1,10},{2,12},{4,2},{6,1},{5,1}})
;ě.Add("small_offroadwheel1x1",new Dictionary<int,int>(){{1,2},{2,5},{5,1}});ě.Add("small_offroadwheel3x3",new Dictionary
<int,int>(){{1,8},{2,15},{5,3}});ě.Add("small_offroadwheel5x5",new Dictionary<int,int>(){{1,15},{2,25},{5,5}});ě.Add(
"large_offroadwheel1x1",new Dictionary<int,int>(){{1,30},{2,30},{5,10}});ě.Add("large_offroadwheel3x3",new Dictionary<int,int>(){{1,70},{2,50},
{5,20}});ě.Add("large_offroadwheel5x5",new Dictionary<int,int>(){{1,130},{2,70},{5,30}});ě.Add("small_offsetlight",new
Dictionary<int,int>(){{2,2}});ě.Add("small_offsetspotlight",new Dictionary<int,int>(){{2,2},{12,1}});ě.Add("small_barredwindow",
new Dictionary<int,int>(){{7,1},{2,4}});ě.Add("small_barredwindowslope",new Dictionary<int,int>(){{7,1},{2,4}});ě.Add(
"small_barredwindowside",new Dictionary<int,int>(){{7,1},{2,4}});ě.Add("small_barredwindowface",new Dictionary<int,int>(){{7,1},{2,4}});ě.Add(
"large_storageshelf1",new Dictionary<int,int>(){{7,10},{1,50},{2,50},{4,50},{3,50}});ě.Add("large_storageshelf2",new Dictionary<int,int>(){{7
,30},{21,20},{6,20},{11,20}});ě.Add("large_storageshelf3",new Dictionary<int,int>(){{7,10},{13,10},{10,10},{14,10},{15,2}
});ě.Add("large_scifilcdpanel5x5",new Dictionary<int,int>(){{3,25},{2,150},{9,25},{8,250},{12,150}});ě.Add(
"large_scifilcdpanel5x3",new Dictionary<int,int>(){{3,15},{2,90},{9,15},{8,150},{12,90}});ě.Add("large_scifilcdpanel3x3",new Dictionary<int,int>
(){{3,10},{2,50},{9,10},{8,90},{12,50}});ě.Add("large_scifiinteriorwall",new Dictionary<int,int>(){{3,25},{2,10}});ě.Add(
"large_neontubes1",new Dictionary<int,int>(){{3,6},{4,6},{2,2}});ě.Add("large_neontubes2",new Dictionary<int,int>(){{3,6},{4,6},{2,2}});ě.
Add("large_neontubescorner",new Dictionary<int,int>(){{3,6},{4,6},{2,2}});ě.Add("large_neontubesup",new Dictionary<int,int>
(){{3,12},{4,12},{2,4}});ě.Add("large_neontubesdown",new Dictionary<int,int>(){{3,3},{4,3},{2,1}});ě.Add(
"large_neontubesend1",new Dictionary<int,int>(){{3,6},{4,6},{2,2}});ě.Add("large_neontubesend2",new Dictionary<int,int>(){{3,10},{4,6},{2,4}}
);ě.Add("large_neontubesdown2",new Dictionary<int,int>(){{3,9},{4,9},{2,3}});ě.Add("large_neontubesushape",new Dictionary
<int,int>(){{3,18},{4,18},{2,6}});ě.Add("large_scificontrolpanel",new Dictionary<int,int>(){{2,4},{9,2},{8,4},{3,2}});ě.
Add("large_scifionebuttonterminal",new Dictionary<int,int>(){{3,5},{2,10},{9,4},{8,4}});ě.Add("small_scifislidingdoor",new
Dictionary<int,int>(){{3,10},{2,26},{12,4},{6,2},{8,1},{9,2},{1,8}});ě.Add("large_scifibarcounter",new Dictionary<int,int>(){{3,16
},{2,10},{6,1},{12,6}});ě.Add("large_scifibarcountercorner",new Dictionary<int,int>(){{3,24},{2,14},{6,2},{12,10}});ě.Add
("large_scififourbuttonpanel",new Dictionary<int,int>(){{3,10},{2,20},{9,20},{8,5}});ě.Add("small_scifiionthruster",new
Dictionary<int,int>(){{1,2},{2,2},{5,1},{14,1}});ě.Add("small_scifilargeionthruster",new Dictionary<int,int>(){{1,5},{2,2},{5,5},{
14,12}});ě.Add("large_scifiionthruster",new Dictionary<int,int>(){{1,25},{2,60},{5,8},{14,80}});ě.Add(
"large_scifilargeionthruster",new Dictionary<int,int>(){{1,150},{2,100},{5,40},{14,960}});ě.Add("large_scifilargeatmosphericthruster",new Dictionary<
int,int>(){{1,230},{2,60},{5,50},{11,40},{6,1100}});ě.Add("large_scifiatmosphericthruster",new Dictionary<int,int>(){{1,35}
,{2,50},{5,8},{11,10},{6,110}});ě.Add("small_scifilargeatmosphericthruster",new Dictionary<int,int>(){{1,20},{2,30},{5,4}
,{11,8},{6,90}});ě.Add("small_scifiatmosphericthruster",new Dictionary<int,int>(){{1,3},{2,22},{5,1},{11,1},{6,18}});ě.
Add("large_lettera",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterb",new Dictionary<int,int>(){{1,4}});ě.Add(
"large_letterc",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterd",new Dictionary<int,int>(){{1,4}});ě.Add("large_lettere",new
Dictionary<int,int>(){{1,4}});ě.Add("large_letterf",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterg",new Dictionary<int,int
>(){{1,4}});ě.Add("large_letterh",new Dictionary<int,int>(){{1,4}});ě.Add("large_letteri",new Dictionary<int,int>(){{1,4}
});ě.Add("large_letterj",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterk",new Dictionary<int,int>(){{1,4}});ě.Add(
"large_letterl",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterm",new Dictionary<int,int>(){{1,4}});ě.Add("large_lettern",new
Dictionary<int,int>(){{1,4}});ě.Add("large_lettero",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterp",new Dictionary<int,int
>(){{1,4}});ě.Add("large_letterq",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterr",new Dictionary<int,int>(){{1,4}
});ě.Add("large_letters",new Dictionary<int,int>(){{1,4}});ě.Add("large_lettert",new Dictionary<int,int>(){{1,4}});ě.Add(
"large_letteru",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterv",new Dictionary<int,int>(){{1,4}});ě.Add("large_letterw",new
Dictionary<int,int>(){{1,4}});ě.Add("large_letterx",new Dictionary<int,int>(){{1,4}});ě.Add("large_lettery",new Dictionary<int,int
>(){{1,4}});ě.Add("large_letterz",new Dictionary<int,int>(){{1,4}});ě.Add("small_lettera",new Dictionary<int,int>(){{1,1}
});ě.Add("small_letterb",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterc",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_letterd",new Dictionary<int,int>(){{1,1}});ě.Add("small_lettere",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterf",new
Dictionary<int,int>(){{1,1}});ě.Add("small_letterg",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterh",new Dictionary<int,int
>(){{1,1}});ě.Add("small_letteri",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterj",new Dictionary<int,int>(){{1,1}
});ě.Add("small_letterk",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterl",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_letterm",new Dictionary<int,int>(){{1,1}});ě.Add("small_lettern",new Dictionary<int,int>(){{1,1}});ě.Add("small_lettero",new
Dictionary<int,int>(){{1,1}});ě.Add("small_letterp",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterq",new Dictionary<int,int
>(){{1,1}});ě.Add("small_letterr",new Dictionary<int,int>(){{1,1}});ě.Add("small_letters",new Dictionary<int,int>(){{1,1}
});ě.Add("small_lettert",new Dictionary<int,int>(){{1,1}});ě.Add("small_letteru",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_letterv",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterw",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterx",new
Dictionary<int,int>(){{1,1}});ě.Add("small_lettery",new Dictionary<int,int>(){{1,1}});ě.Add("small_letterz",new Dictionary<int,int
>(){{1,1}});ě.Add("large_number0",new Dictionary<int,int>(){{1,4}});ě.Add("large_number1",new Dictionary<int,int>(){{1,4}
});ě.Add("large_number2",new Dictionary<int,int>(){{1,4}});ě.Add("large_number3",new Dictionary<int,int>(){{1,4}});ě.Add(
"large_number4",new Dictionary<int,int>(){{1,4}});ě.Add("large_number5",new Dictionary<int,int>(){{1,4}});ě.Add("large_number6",new
Dictionary<int,int>(){{1,4}});ě.Add("large_number7",new Dictionary<int,int>(){{1,4}});ě.Add("large_number8",new Dictionary<int,int
>(){{1,4}});ě.Add("large_number9",new Dictionary<int,int>(){{1,4}});ě.Add("small_number0",new Dictionary<int,int>(){{1,1}
});ě.Add("small_number1",new Dictionary<int,int>(){{1,1}});ě.Add("small_number2",new Dictionary<int,int>(){{1,1}});ě.Add(
"small_number3",new Dictionary<int,int>(){{1,1}});ě.Add("small_number4",new Dictionary<int,int>(){{1,1}});ě.Add("small_number5",new
Dictionary<int,int>(){{1,1}});ě.Add("small_number6",new Dictionary<int,int>(){{1,1}});ě.Add("small_number7",new Dictionary<int,int
>(){{1,1}});ě.Add("small_number8",new Dictionary<int,int>(){{1,1}});ě.Add("small_number9",new Dictionary<int,int>(){{1,1}
});ě.Add("large_hyphen",new Dictionary<int,int>(){{1,4}});ě.Add("large_underscore",new Dictionary<int,int>(){{1,4}});ě.
Add("large_dot",new Dictionary<int,int>(){{1,4}});ě.Add("large_apostrophe",new Dictionary<int,int>(){{1,4}});ě.Add(
"large_and",new Dictionary<int,int>(){{1,4}});ě.Add("large_colon",new Dictionary<int,int>(){{1,4}});ě.Add("large_exclamationmark",
new Dictionary<int,int>(){{1,4}});ě.Add("large_questionmark",new Dictionary<int,int>(){{1,4}});ě.Add("small_hyphen",new
Dictionary<int,int>(){{1,1}});ě.Add("small_underscore",new Dictionary<int,int>(){{1,1}});ě.Add("small_dot",new Dictionary<int,int>
(){{1,1}});ě.Add("small_apostrophe",new Dictionary<int,int>(){{1,1}});ě.Add("small_and",new Dictionary<int,int>(){{1,1}})
;ě.Add("small_colon",new Dictionary<int,int>(){{1,1}});ě.Add("small_exclamationmark",new Dictionary<int,int>(){{1,1}});ě.
Add("small_questionmark",new Dictionary<int,int>(){{1,1}});ě.Add("small_ionthruster",new Dictionary<int,int>(){{1,2},{2,2},
{5,1},{14,1}});ě.Add("small_largeionthruster",new Dictionary<int,int>(){{1,5},{2,2},{5,5},{14,12}});ě.Add(
"large_ionthruster",new Dictionary<int,int>(){{1,25},{2,60},{5,8},{14,80}});ě.Add("large_largeionthruster",new Dictionary<int,int>(){{1,150
},{2,100},{5,40},{14,960}});ě.Add("large_largehydrogenthruster",new Dictionary<int,int>(){{1,150},{2,180},{11,250},{5,40}
});ě.Add("large_hydrogenthruster",new Dictionary<int,int>(){{1,25},{2,60},{11,40},{5,8}});ě.Add(
"small_largehydrogenthruster",new Dictionary<int,int>(){{1,30},{2,30},{11,22},{5,10}});ě.Add("small_hydrogenthruster",new Dictionary<int,int>(){{1,7}
,{2,15},{11,4},{5,2}});ě.Add("large_largeatmosphericthruster",new Dictionary<int,int>(){{1,230},{2,60},{5,50},{11,40},{6,
1100}});ě.Add("large_atmosphericthruster",new Dictionary<int,int>(){{1,35},{2,50},{5,8},{11,10},{6,110}});ě.Add(
"small_largeatmosphericthruster",new Dictionary<int,int>(){{1,20},{2,30},{5,4},{11,8},{6,90}});ě.Add("small_atmosphericthruster",new Dictionary<int,int>
(){{1,3},{2,22},{5,1},{11,1},{6,18}});ě.Add("small_drill",new Dictionary<int,int>(){{1,32},{2,30},{5,4},{6,1},{9,1}});ě.
Add("large_drill",new Dictionary<int,int>(){{1,300},{2,40},{5,12},{6,5},{9,5}});ě.Add("large_grinder",new Dictionary<int,
int>(){{1,20},{2,30},{5,1},{6,4},{9,2}});ě.Add("small_grinder",new Dictionary<int,int>(){{1,12},{2,17},{4,4},{6,4},{9,2}});
ě.Add("large_welder",new Dictionary<int,int>(){{1,20},{2,30},{5,1},{6,2},{9,2}});ě.Add("small_welder",new Dictionary<int,
int>(){{1,12},{2,17},{4,6},{6,2},{9,2}});ě.Add("large_oredetector",new Dictionary<int,int>(){{1,50},{2,40},{6,5},{9,25},{18
,20}});ě.Add("small_oredetector",new Dictionary<int,int>(){{1,3},{2,2},{6,1},{9,1},{18,1}});ě.Add("large_landinggear",new
Dictionary<int,int>(){{1,150},{2,20},{6,6}});ě.Add("small_landinggear",new Dictionary<int,int>(){{1,2},{2,5},{6,1}});ě.Add(
"large_magneticplatesmall",new Dictionary<int,int>(){{1,15},{2,3},{6,1}});ě.Add("small_magneticplatesmall",new Dictionary<int,int>(){{1,2},{2,1},{
6,1}});ě.Add("large_jumpdrive",new Dictionary<int,int>(){{1,60},{11,50},{15,20},{18,20},{21,120},{10,1000},{9,300},{2,40}
});ě.Add("small_camera",new Dictionary<int,int>(){{1,2},{9,3}});ě.Add("large_camera",new Dictionary<int,int>(){{1,2},{9,3
}});ě.Add("large_mergeblock",new Dictionary<int,int>(){{1,12},{2,15},{6,2},{5,6},{9,2}});ě.Add("small_mergeblock",new
Dictionary<int,int>(){{1,4},{2,5},{6,1},{4,2},{9,1}});ě.Add("small_mergeblocksmall",new Dictionary<int,int>(){{1,2},{2,3},{6,1},{4
,1},{9,1}});ě.Add("large_parachutehatch",new Dictionary<int,int>(){{1,9},{2,25},{4,5},{6,3},{9,2}});ě.Add(
"small_parachutehatch",new Dictionary<int,int>(){{1,2},{2,2},{4,1},{6,1},{9,1}});ě.Add("large_weaponrack",new Dictionary<int,int>(){{3,30},{2,
20}});ě.Add("small_weaponrack",new Dictionary<int,int>(){{3,3},{2,3}});ě.Add("large_firecover",new Dictionary<int,int>(){{
1,4},{2,10}});ě.Add("large_firecovercorner",new Dictionary<int,int>(){{1,8},{2,20}});ě.Add("large_halfwindow",new
Dictionary<int,int>(){{7,4},{1,10},{12,10}});ě.Add("large_halfwindowinv",new Dictionary<int,int>(){{7,4},{1,10},{12,10}});ě.Add(
"large_halfwindowcorner",new Dictionary<int,int>(){{7,8},{1,20},{12,20}});ě.Add("large_halfwindowcornerinv",new Dictionary<int,int>(){{7,8},{1,
20},{12,20}});ě.Add("large_embrasure",new Dictionary<int,int>(){{1,30},{2,20},{11,10}});ě.Add("large_passage3",new
Dictionary<int,int>(){{3,74},{2,20},{4,48}});ě.Add("large_passage3wall",new Dictionary<int,int>(){{3,50},{2,14},{4,32}});ě.Add(
"large_passage3cross",new Dictionary<int,int>(){{3,35},{2,10},{4,25}});ě.Add("large_passage3frame",new Dictionary<int,int>(){{3,35},{2,10},{4
,25}});ě.Add("large_passage3light",new Dictionary<int,int>(){{3,74},{2,20},{4,48}});ě.Add("large_passagescificorner",new
Dictionary<int,int>(){{3,74},{2,20},{4,48}});ě.Add("large_passagescifitjunction",new Dictionary<int,int>(){{3,55},{2,16},{4,38}});
ě.Add("large_passagescifiwindow",new Dictionary<int,int>(){{3,60},{2,16},{12,16},{4,38}});ě.Add("large_bridgewindowslope"
,new Dictionary<int,int>(){{7,8},{1,5},{3,10},{12,25}});ě.Add("large_bridgewindowface",new Dictionary<int,int>(){{7,8},{1
,2},{3,4},{12,18}});ě.Add("small_passengerbench",new Dictionary<int,int>(){{3,20},{2,20}});ě.Add("large_lightpanel",new
Dictionary<int,int>(){{2,10},{3,5}});ě.Add("small_lightpanel",new Dictionary<int,int>(){{2,2},{3,1}});ě.Add(
"large_smallwarfarereactor",new Dictionary<int,int>(){{1,80},{2,40},{11,4},{5,8},{13,100},{6,6},{9,25}});ě.Add("large_largewarfarereactor",new
Dictionary<int,int>(){{1,1000},{2,70},{11,40},{5,40},{10,100},{13,2000},{6,20},{9,75}});ě.Add("small_smallwarfarereactor",new
Dictionary<int,int>(){{1,3},{2,10},{11,2},{5,1},{13,3},{6,1},{9,10}});ě.Add("small_largewarfarereactor",new Dictionary<int,int>(){
{1,60},{2,9},{11,9},{5,3},{13,95},{6,5},{9,25}});ě.Add("large_warfarehangardoor",new Dictionary<int,int>(){{1,350},{2,40}
,{4,40},{6,16},{9,2}});ě.Add("large_warfarehangardoorwindowed",new Dictionary<int,int>(){{1,350},{2,40},{4,40},{6,16},{9,
2}});ě.Add("large_warfarehangardoor2",new Dictionary<int,int>(){{1,350},{2,40},{4,40},{6,16},{9,2}});ě.Add(
"small_warfarerocketlauncher",new Dictionary<int,int>(){{1,4},{2,2},{11,1},{5,4},{6,1},{9,1}});ě.Add("small_warfaregatlinggun",new Dictionary<int,int
>(){{1,4},{2,1},{11,2},{4,6},{6,1},{9,1}});ě.Add("large_warfarebattery",new Dictionary<int,int>(){{1,80},{2,30},{21,80},{
9,25}});ě.Add("small_warfarebattery",new Dictionary<int,int>(){{1,25},{2,5},{21,20},{9,2}});ě.Add(
"large_slidinghatchdoor",new Dictionary<int,int>(){{1,40},{2,50},{4,10},{6,4},{8,2},{9,2},{12,10}});ě.Add("large_halfslidinghatchdoor",new
Dictionary<int,int>(){{1,30},{2,50},{4,10},{6,4},{8,2},{9,2},{12,10}});ě.Add("small_searchlight",new Dictionary<int,int>(){{1,1},{
2,3},{5,1},{6,2},{9,5},{12,2}});ě.Add("large_searchlight",new Dictionary<int,int>(){{1,5},{2,20},{5,2},{6,4},{9,5},{12,4}
});ě.Add("large_heatvent",new Dictionary<int,int>(){{1,25},{2,20},{5,10},{6,5}});ě.Add("small_heatvent",new Dictionary<
int,int>(){{1,2},{2,1},{5,1},{6,1}});ě.Add("small_helm",new Dictionary<int,int>(){{3,20},{2,20},{6,1},{9,20},{8,2}});ě.Add(
"large_helm",new Dictionary<int,int>(){{3,20},{2,20},{6,1},{9,20},{8,2}});ě.Add("small_warfareionthruster",new Dictionary<int,int>()
{{1,2},{2,2},{5,1},{14,1}});ě.Add("small_largewarfareionthruster",new Dictionary<int,int>(){{1,5},{2,2},{5,5},{14,12}});ě
.Add("large_warfareionthruster",new Dictionary<int,int>(){{1,25},{2,60},{5,8},{14,80}});ě.Add(
"large_largewarfareionthruster",new Dictionary<int,int>(){{1,150},{2,100},{5,40},{14,960}});ě.Add("large_warhead",new Dictionary<int,int>(){{1,20},{7,
24},{2,12},{4,12},{9,2},{19,6}});ě.Add("small_warhead",new Dictionary<int,int>(){{1,4},{7,1},{2,1},{4,2},{9,1},{19,2}});ě.
Add("large_decoy",new Dictionary<int,int>(){{1,30},{2,10},{9,10},{17,1},{5,2}});ě.Add("small_decoy",new Dictionary<int,int>
(){{1,2},{2,1},{9,1},{17,1},{4,2}});ě.Add("large_gatlingturret",new Dictionary<int,int>(){{1,40},{2,40},{11,15},{4,6},{6,
8},{9,10}});ě.Add("small_gatlingturret",new Dictionary<int,int>(){{1,15},{2,30},{11,5},{4,6},{6,4},{9,10}});ě.Add(
"large_missileturret",new Dictionary<int,int>(){{1,40},{2,50},{11,15},{5,6},{6,16},{9,10}});ě.Add("small_missileturret",new Dictionary<int,
int>(){{1,15},{2,40},{11,5},{5,2},{6,8},{9,10}});ě.Add("large_interiorturret",new Dictionary<int,int>(){{3,6},{2,20},{4,1},
{6,2},{9,5},{1,4}});ě.Add("small_rocketlauncher",new Dictionary<int,int>(){{1,4},{2,2},{11,1},{5,4},{6,1},{9,1}});ě.Add(
"large_rocketlauncher",new Dictionary<int,int>(){{1,35},{2,8},{11,30},{5,25},{6,6},{9,4}});ě.Add("small_reloadablerocketlauncher",new
Dictionary<int,int>(){{4,50},{3,50},{2,24},{5,8},{11,10},{6,4},{9,2},{1,8}});ě.Add("small_gatlinggun",new Dictionary<int,int>(){{1
,4},{2,1},{11,2},{4,6},{6,1},{9,1}});ě.Add("small_autocannon",new Dictionary<int,int>(){{1,6},{2,2},{11,2},{4,2},{6,1},{9
,1}});ě.Add("small_assaultcannon",new Dictionary<int,int>(){{1,25},{2,10},{11,5},{5,10},{9,1}});ě.Add("large_artillery",
new Dictionary<int,int>(){{1,250},{2,20},{11,20},{5,20},{9,5}});ě.Add("large_railgun",new Dictionary<int,int>(){{1,350},{2,
150},{10,150},{5,60},{21,100},{9,100}});ě.Add("small_railgun",new Dictionary<int,int>(){{1,25},{2,20},{10,20},{5,6},{21,10}
,{9,20}});ě.Add("large_artilleryturret",new Dictionary<int,int>(){{1,450},{2,400},{11,50},{5,40},{6,30},{9,20}});ě.Add(
"large_assaultcannonturret",new Dictionary<int,int>(){{1,300},{2,280},{11,30},{5,30},{6,20},{9,20}});ě.Add("small_assaultcannonturret",new
Dictionary<int,int>(){{1,50},{2,100},{11,10},{5,6},{6,10},{9,20}});ě.Add("small_autocannonturret",new Dictionary<int,int>(){{1,20}
,{2,40},{11,6},{4,4},{6,4},{9,10}});ě.Add("large_wheelsuspension3x3left",new Dictionary<int,int>(){{1,95},{2,65},{5,26},{
4,12},{6,6}});ě.Add("large_wheelsuspension5x5left",new Dictionary<int,int>(){{1,200},{2,110},{5,50},{4,30},{6,20}});ě.Add
("large_wheelsuspension1x1left",new Dictionary<int,int>(){{1,55},{2,45},{5,16},{4,12},{6,6}});ě.Add(
"small_wheelsuspension3x3left",new Dictionary<int,int>(){{1,16},{2,22},{4,2},{6,1},{5,3}});ě.Add("small_wheelsuspension5x5left",new Dictionary<int,int
>(){{1,31},{2,37},{4,4},{6,2},{5,5}});ě.Add("small_wheelsuspension1x1left",new Dictionary<int,int>(){{1,10},{2,12},{4,2},
{6,1},{5,1}});ě.Add("large_wheelsuspension3x3right",new Dictionary<int,int>(){{1,95},{2,65},{5,26},{4,12},{6,6}});ě.Add(
"large_wheelsuspension5x5right",new Dictionary<int,int>(){{1,200},{2,110},{5,50},{4,30},{6,20}});ě.Add("large_wheelsuspension1x1right",new Dictionary<
int,int>(){{1,55},{2,45},{5,16},{4,12},{6,6}});ě.Add("small_wheelsuspension3x3right",new Dictionary<int,int>(){{1,16},{2,22
},{4,2},{6,1},{5,3}});ě.Add("small_wheelsuspension5x5right",new Dictionary<int,int>(){{1,31},{2,37},{4,4},{6,2},{5,5}});ě
.Add("small_wheelsuspension1x1right",new Dictionary<int,int>(){{1,10},{2,12},{4,2},{6,1},{5,1}});ě.Add("small_wheel1x1",
new Dictionary<int,int>(){{1,2},{2,5},{5,1}});ě.Add("small_wheel3x3",new Dictionary<int,int>(){{1,8},{2,15},{5,3}});ě.Add(
"small_wheel5x5",new Dictionary<int,int>(){{1,15},{2,25},{5,5}});ě.Add("large_wheel1x1",new Dictionary<int,int>(){{1,30},{2,30},{5,10}})
;ě.Add("large_wheel3x3",new Dictionary<int,int>(){{1,70},{2,50},{5,20}});ě.Add("large_wheel5x5",new Dictionary<int,int>()
{{1,130},{2,70},{5,30}});ě.Add("large_verticalwindow",new Dictionary<int,int>(){{3,12},{2,8},{4,4}});ě.Add(
"large_diagonalwindow",new Dictionary<int,int>(){{3,16},{2,12},{4,6}});ě.Add("large_window1x2slope",new Dictionary<int,int>(){{7,16},{12,55}})
;ě.Add("large_window1x2faceinv",new Dictionary<int,int>(){{7,15},{12,40}});ě.Add("large_window1x2face",new Dictionary<int
,int>(){{7,15},{12,40}});ě.Add("large_window1x2sideleft",new Dictionary<int,int>(){{7,13},{12,26}});ě.Add(
"large_window1x2sideleftinv",new Dictionary<int,int>(){{7,13},{12,26}});ě.Add("large_window1x2sideright",new Dictionary<int,int>(){{7,13},{12,26}});
ě.Add("large_window1x2siderightinv",new Dictionary<int,int>(){{7,13},{12,26}});ě.Add("large_window1x1slope",new
Dictionary<int,int>(){{7,12},{12,35}});ě.Add("large_window1x1face",new Dictionary<int,int>(){{7,11},{12,24}});ě.Add(
"large_window1x1side",new Dictionary<int,int>(){{7,9},{12,17}});ě.Add("large_window1x1sideinv",new Dictionary<int,int>(){{7,9},{12,17}});ě.
Add("large_window1x1faceinv",new Dictionary<int,int>(){{7,11},{12,24}});ě.Add("large_window1x2flat",new Dictionary<int,int>
(){{7,15},{12,50}});ě.Add("large_window1x2flatinv",new Dictionary<int,int>(){{7,15},{12,50}});ě.Add("large_window1x1flat"
,new Dictionary<int,int>(){{7,10},{12,25}});ě.Add("large_window1x1flatinv",new Dictionary<int,int>(){{7,10},{12,25}});ě.
Add("large_window3x3flat",new Dictionary<int,int>(){{7,40},{12,196}});ě.Add("large_window3x3flatinv",new Dictionary<int,int
>(){{7,40},{12,196}});ě.Add("large_window2x3flat",new Dictionary<int,int>(){{7,25},{12,140}});ě.Add(
"large_window2x3flatinv",new Dictionary<int,int>(){{7,25},{12,140}});ě.Add("small_window1x2slope",new Dictionary<int,int>(){{7,1},{12,3}});ě.Add
("small_window1x2faceinv",new Dictionary<int,int>(){{7,1},{12,3}});ě.Add("small_window1x2face",new Dictionary<int,int>(){
{7,1},{12,3}});ě.Add("small_window1x2sideleft",new Dictionary<int,int>(){{7,1},{12,3}});ě.Add(
"small_window1x2sideleftinv",new Dictionary<int,int>(){{7,1},{12,3}});ě.Add("small_window1x2sideright",new Dictionary<int,int>(){{7,1},{12,3}});ě.
Add("small_window1x2siderightinv",new Dictionary<int,int>(){{7,1},{12,3}});ě.Add("small_window1x1slope",new Dictionary<int,
int>(){{7,1},{12,2}});ě.Add("small_window1x1face",new Dictionary<int,int>(){{7,1},{12,2}});ě.Add("small_window1x1side",new
Dictionary<int,int>(){{7,1},{12,2}});ě.Add("small_window1x1sideinv",new Dictionary<int,int>(){{7,1},{12,2}});ě.Add(
"small_window1x1faceinv",new Dictionary<int,int>(){{7,1},{12,2}});ě.Add("small_window1x2flat",new Dictionary<int,int>(){{7,1},{12,3}});ě.Add(
"small_window1x2flatinv",new Dictionary<int,int>(){{7,1},{12,3}});ě.Add("small_window1x1flat",new Dictionary<int,int>(){{7,1},{12,2}});ě.Add(
"small_window1x1flatinv",new Dictionary<int,int>(){{7,1},{12,2}});ě.Add("small_window3x3flat",new Dictionary<int,int>(){{7,3},{12,12}});ě.Add(
"small_window3x3flatinv",new Dictionary<int,int>(){{7,3},{12,12}});ě.Add("small_window2x3flat",new Dictionary<int,int>(){{7,2},{12,8}});ě.Add(
"small_window2x3flatinv",new Dictionary<int,int>(){{7,2},{12,8}});}}void Ħ(string Ě,List<IMyTerminalBlock>r){string[]Į=Ě.Split(';');if(Į.Length
<=0){Echo("Not enough arguments...");return;}if(Į[0].ToString().ToLower()=="reset"){Ê(Ô(r),null);var Ę=s(r);È(Ę.FindAll(Č
=>Č.CustomName.Contains(prefixKeyLCDAll)),false,null);È(Ę.FindAll(Č=>Č.CustomName.Contains(prefixKey)),true,null);}else if(Į.Length>1){
IMyProjector ċ=Ô(r).Find(Ċ=>Ċ.CustomName.Trim().Equals(Į[0].Trim()));if(ċ==null)return;bool ĉ=false;bool Ĉ=false;if(Į.Length>=3){Ĉ=
Boolean.TryParse(Į[2],out ĉ);}string ā="";if(Į[1].ToLower().Trim()=="heavy")ā="typevar";else if(Į[1].ToLower().Trim()==
"toassembler")ā="workingvar";else return;if(Ĉ)Ŀ(ċ,ď.Ö[ā],ĉ.ToString());else{bool ć=Boolean.TryParse(ĳ(ċ.CustomData,ď.Ö[ā]),out ĉ);ĉ=ć
?!ĉ:false;Ŀ(ċ,ď.Ö[ā],ĉ.ToString());}}else{Echo("Either not enough arguments or wrong argument...");return;}}Dictionary<
string,int>č(Dictionary<string,int>Ć,string Ą,bool ă){Dictionary<string,int>Ă=new Dictionary<string,int>();foreach(string ā in
Ć.Keys){Dictionary<int,int>Ā=null;Ā=ą(ā,Ą,ă);if(Ā==null)continue;Dictionary<string,int>ÿ=new Dictionary<string,int>();
foreach(int þ in Ā.Keys){ÿ.Add(ComponentsKeys[þ],Ā[þ]*Ć[ā]);}Ă=Ŏ(ÿ,Ă);}return Ă;}Dictionary<int,int>ą(string ý,string Ą,bool ă){Dictionary<
int,int>Ā=null;string ė=Ķ(ý.Replace(" ","").ToLower());if(!ě.TryGetValue(Ą+ė,out Ā)){if(ė.Equals("armorblocks")){if(!ă)Ā=ě[
Ą+"lightarmorblock"];else Ā=ě[Ą+"heavyarmorblock"];}else{Dictionary<string,Dictionary<int,int>>Ė=ě.Where(ĕ=>ĕ.Key.
StartsWith(Ą)&&ĺ(Ą,ý,ĕ.Key)).ToDictionary(ĕ=>ĕ.Key,ĕ=>ĕ.Value);if(Ė.Keys.Count<1){Ė=customBlocks.Where(ĕ=>ĕ.Key.StartsWith(Ą)&&ĺ(Ą,ý,ĕ.Key)).
ToDictionary(ĕ=>ĕ.Key,ĕ=>ĕ.Value);if(Ė.Keys.Count<1){Echo("Error: Did not find "+ý);return null;}if(Ė.Keys.Count>1){Echo(
"Error: Found multiple custom keys with "+ý);return null;}}if(Ė.Keys.Count>1){Echo("Error: Found multiple keys with "+ý);return null;}Ā=Ė[Ė.Keys.First()];}}
return Ā;}void Ĕ(List<IMyAssembler>z,Dictionary<string,int>Ă){foreach(IMyAssembler ē in z){foreach(String ā in Ă.Keys){double
Ē=Decimal.ToDouble(Math.Ceiling(Convert.ToDecimal(Ă[ā]/z.Count)));đ(ā,Ē,ē);}}Echo(
"Componenets added to assembler queues...");}void đ(String Đ,double Ď,IMyAssembler ē){MyDefinitionId ķ=new MyDefinitionId();if(MyDefinitionId.TryParse(
"MyObjectBuilder_BlueprintDefinition/"+Đ,out ķ)){ē.AddQueueItem(ķ,Ď);}else{Echo("Something went wrong parsing definition for "+Đ);}}void Ŋ(IMyTextPanel Æ,
Dictionary<string,int>Ă,Dictionary<string,int>Ĭ,bool ŉ=true,string ň=""){Ä Ň=new Ä();if(!Ň.H(Æ)){Echo(Ň.Á);return;}Ň.I();string ņ=
Ň.ù(ŉ?ď.Ö["lcdfronttitleall"]:(ď.Ö["lcdfronttitle"]+" - "+ň));StringBuilder Å=new StringBuilder();Å.AppendLine();foreach(
string ā in Ă.Keys){Å.AppendLine(Ň.è(ā,checkInventory?((Ĭ!=null&&Ĭ.ContainsKey(ā)?Ĭ[ā]:0)+"/"+Ă[ā]):Ă[ā].ToString()));if(checkInventory)Å.AppendLine(Ň.ï
(Ĭ!=null&&Ĭ.ContainsKey(ā)?Ĭ[ā]:0,Ă[ā]));}string İ=Å.ToString();int ń=Int32.Parse(ĳ(Æ.CustomData,ď.Ö["lcdloopoffsetvar"])
);Ň.Q(ń);İ=Ň.à(İ,1,ņ);string Ń=İ.Split(new string[]{"&&"},StringSplitOptions.None)[1];Ŀ(Æ,ď.Ö["lcdloopoffsetvar"],Ń);Ň.E(
İ.Split(new string[]{"&&"},StringSplitOptions.None)[0]);}Dictionary<string,int>ł(List<IMyTerminalBlock>r){Dictionary<
string,int>Ă=new Dictionary<string,int>();foreach(IMyTerminalBlock Ľ in r){if(Ľ.HasInventory){for(int Ņ=0;Ņ<Ľ.InventoryCount;Ņ
++){var Ł=Ľ.GetInventory(Ņ);Ă=ŋ(Ă,Ł);}}}return Ă;}Dictionary<string,int>ŋ(Dictionary<string,int>Ă,IMyInventory Ł){
Dictionary<string,int>ŕ=new Dictionary<string,int>();List<MyInventoryItem>Ŕ=new List<MyInventoryItem>();Ł.GetItems(Ŕ);for(int f=0;
f<Ŕ.Count;f++){var ĕ=Ŕ[f];if(ĕ.Amount>0){if(!ĕ.Type.TypeId.Equals("MyObjectBuilder_Component"))continue;string œ;if(!ComponentToBlueprint.
TryGetValue(ĕ.Type.SubtypeId,out œ))œ=ĕ.Type.SubtypeId;if(ŕ.ContainsKey(œ))ŕ[œ]+=ĕ.Amount.ToIntSafe();else ŕ.Add(œ,ĕ.Amount.
ToIntSafe());}}return Ŏ(ŕ,Ă);}bool ĺ(string Ą,string Œ,string ĉ){string[]ő=Œ.Trim().ToLower().Split(' ');bool Ő=true;foreach(
string ŏ in ő){if(!string.IsNullOrEmpty(Ķ(ŏ.Trim().ToLower()))){if(!ĉ.Replace(Ą,"").Contains(Ķ(ŏ))){Ő=false;break;}}}return Ő;
}Dictionary<string,int>Ŏ(Dictionary<string,int>ō,Dictionary<string,int>ŀ){foreach(string Ċ in ō.Keys){if(ŀ.Keys.Contains(
Ċ))ŀ[Ċ]+=ō[Ċ];else ŀ.Add(Ċ,ō[Ċ]);}return ŀ;}Dictionary<string,int>Ō(Dictionary<string,int>ō,Dictionary<string,int>ŀ){for(
int f=0;f<ŀ.Keys.Count;f++){string Ċ=ŀ.Keys.ElementAt(f);if(ō.Keys.Contains(Ċ)){if(ŀ[Ċ]-ō[Ċ]<=0)ŀ.Remove(Ċ);else ŀ[Ċ]-=ō[Ċ]
;}}return ŀ;}string Ķ(string ĉ){return ĉ.Replace("/","").Replace(".","").Replace("-","");}Dictionary<string,int>ĵ(
IMyProjector t){Dictionary<string,int>Ć=new Dictionary<string,int>();string[]Ĵ=t.DetailedInfo.Split(new string[]{"\r\n","\n"},
StringSplitOptions.None);for(var f=5;f<Ĵ.Length;f++){Ć[Ĵ[f].Split(':')[0].Trim()]=Convert.ToInt32(Ĵ[f].Split(':')[1].Trim());}return Ć;}
string ĳ(string İ,string ā){ā=ā.ToLower();string[]į=İ.ToLower().Split(new string[]{"\n"},StringSplitOptions.RemoveEmptyEntries
);string Ĳ=į.FirstOrDefault(Č=>Č.StartsWith(ā));if(!string.IsNullOrEmpty(Ĳ)&&Ĳ.Contains("="))return Ĳ.Split('=')[1];else
return"";}bool ı(string İ,string ā){ā=ā.ToLower();string[]į=İ.ToLower().Split(new string[]{"\n"},StringSplitOptions.
RemoveEmptyEntries);string Ĳ=į.FirstOrDefault(Č=>Č.StartsWith(ā));return!string.IsNullOrEmpty(Ĳ);}void Ŀ(IMyTerminalBlock Ľ,string ā,
string ĉ){if(Ľ.CustomData.Contains(ā)){string[]į=Ľ.CustomData.Split(new string[]{"\n"},StringSplitOptions.None);int Ĺ=Array.
FindIndex(į,Č=>Č.ToString().StartsWith(ā));į[Ĺ]=ā+ĉ;StringBuilder Å=new StringBuilder();for(var f=0;f<į.Length;f++){Å.Append(į[f]
).Append("\n");}Ľ.CustomData=Å.ToString();}}bool ľ(IMyTerminalBlock Ľ,string ļ,string Ļ,string ĺ){if(!Ľ.CustomData.
Contains(string.IsNullOrEmpty(ĺ)?ļ:ĺ)){string[]į=Ľ.CustomData.Split(new string[]{"\n"},StringSplitOptions.RemoveEmptyEntries);
int Ĺ=-1;if(string.IsNullOrEmpty(Ļ))Ĺ=0;else if(Ľ.CustomData.Contains(Ļ))Ĺ=Array.FindIndex(į,Č=>Č.ToString().StartsWith(Ļ))
+1;if(Ĺ!=-1){List<string>ĸ=į.ToList();ĸ.Insert(Ĺ,ļ);StringBuilder Å=new StringBuilder();for(var f=0;f<ĸ.Count;f++){Å.
Append(ĸ[f]).Append("\n");}Ľ.CustomData=Å.ToString();return true;}return false;}return true;}List<IMyProjector>Ô(List<
IMyTerminalBlock>r){List<IMyProjector>y=r.OfType<IMyProjector>().ToList();List<IMyProjector>w=new List<IMyProjector>();for(var f=0;f<y.
Count;f++){if(y[f].Enabled&&y[f].CustomName.Contains(prefixKey)){Ê(null,y[f]);w.Add(y[f]as IMyProjector);}}return w;}List<
IMyAssembler>v(List<IMyTerminalBlock>r){List<IMyAssembler>u=r.OfType<IMyAssembler>().ToList();List<IMyAssembler>z=new List<
IMyAssembler>();for(var f=0;f<u.Count;f++){if(u[f].Enabled&&u[f].CustomName.Contains(prefixKey)){z.Add(u[f]as IMyAssembler);}}return z;}List
<IMyTextPanel>s(List<IMyTerminalBlock>r){List<IMyTextPanel>o=r.OfType<IMyTextPanel>().ToList();List<IMyTextPanel>n=new
List<IMyTextPanel>();for(var f=0;f<o.Count;f++){if(o[f].Enabled&&(o[f].CustomName.Contains(prefixKey)||o[f].CustomName.Contains(prefixKeyLCDAll)))
{È(null,o[f].CustomName.Contains(prefixKey),o[f]);if(o[f].CustomName.Contains(prefixKey)&&string.IsNullOrEmpty(ĳ(o[f].CustomData,ď.Ö[
"lcdprojectorvar"])))o[f].WriteText(ď.Ö["nolcdprojector"]);n.Add(o[f]as IMyTextPanel);}else if(!o[f].CustomName.Contains(prefixKey)&&!o[f].
CustomName.Contains(prefixKeyLCDAll)&&o[f].CustomData.StartsWith(ď.Ö["intro"].Trim()))o[f].CustomData="";}return n;}List<IMyTextPanel>q(List<
IMyTextPanel>o,IMyProjector t){List<IMyTextPanel>n=new List<IMyTextPanel>();for(var f=0;f<o.Count;f++){if(o[f].CustomName.Contains(prefixKey
)&&ĳ(o[f].CustomData,ď.Ö["lcdprojectorvar"]).ToLower().Trim()==t.CustomName.Trim().ToLower()){n.Add(o[f]as IMyTextPanel);
}}return n;}void Ê(List<IMyProjector>w,IMyProjector t){if(t!=null){ľ(t,ď.Ö["intro"],"","");ľ(t,ď.Ö["workingintro"]+"\n"+ď
.Ö["workingvar"]+"True\n\n",ď.Ö["intro"],ď.Ö["workingvar"]);ľ(t,ď.Ö["typeintro"]+"\n"+ď.Ö["typevar"]+"False\n\n",ď.Ö[
"workingvar"],ď.Ö["typevar"]);}else for(var f=0;f<w.Count;f++){StringBuilder Å=new StringBuilder();Å.Append(ď.Ö["intro"]);Å.Append(ď
.Ö["workingintro"]);Å.Append(ď.Ö["workingvar"]).Append("True\n\n");Å.Append(ď.Ö["typeintro"]);Å.Append(ď.Ö["typevar"]).
Append("False\n");w[f].CustomData=Å.ToString();}}void È(List<IMyTextPanel>n,bool Ç,IMyTextPanel Æ){if(Æ!=null){ľ(Æ,ď.Ö["intro"
],"","");ľ(Æ,ď.Ö["lcdloopoffsetintro"]+"\n"+ď.Ö["lcdloopoffsetvar"]+"0\n\n",ď.Ö["intro"],ď.Ö["lcdloopoffsetvar"]);if(Ç)ľ(
Æ,ď.Ö["lcdprojectorintro"]+"\n"+ď.Ö["lcdprojectorvar"]+"\n\n",ď.Ö["lcdloopoffsetvar"],ď.Ö["lcdprojectorvar"]);}else for(
var f=0;f<n.Count;f++){StringBuilder Å=new StringBuilder();Å.Append(ď.Ö["intro"]);Å.AppendLine(ď.Ö["lcdloopoffsetintro"]);Å
.Append(ď.Ö["lcdloopoffsetvar"]).Append("0\n\n");if(Ç){Å.AppendLine(ď.Ö["lcdprojectorintro"]);Å.Append(ď.Ö[
"lcdprojectorvar"]).Append("\n\n");}n[f].CustomData=Å.ToString();}}
}class Ä{public IMyTerminalBlock Ã;public string Â="";public string Á="";string À="";public string º="lcd";public
Dictionary<char,float>µ=new Dictionary<char,float>();public int ª=0;public float m=0;int R=0;int A=0;public int P=17;bool O=true;
bool N=true;string M;bool L=false;public Ä(string K="default"){if(K=="default")A=93;R=A;b(K);}public void Q(int J){ª=J;}
public bool H(IMyTerminalBlock G){bool F=false;if(º=="lcd"){if(Ã==null||À!=Ã.CustomName)F=true;}else{if(Ã==null)F=true;}if(F){
Ã=G;if(Ã==null){Á="Supplied Terminal Block is Empty";return false;}if(!Ã.BlockDefinition.ToString().Contains("TextPanel")
)º="block";À=Ã.CustomName;return true;}else return true;}public void E(string D){if(º.ToLower()=="lcd"){if(((IMyTextPanel
)Ã).ContentType!=ContentType.TEXT_AND_IMAGE)((IMyTextPanel)Ã).ContentType=ContentType.TEXT_AND_IMAGE;((IMyTextPanel)Ã).
WriteText(D);}else{Ã.CustomName=D;C(true);}}public void C(bool B){if(B){Ã.ShowOnHUD=true;}else{Ã.ShowOnHUD=false;}}public void I(
){Â="";if(º=="lcd"){if(M!=((IMyTextPanel)Ã).Font){M=((IMyTextPanel)Ã).Font;if(M=="Monospace"){A=26;R=A;P=17;b(M);}else{A=
93;R=A;P=17;b(M);}L=true;}if(m!=((IMyTextPanel)Ã).FontSize||L){m=((IMyTextPanel)Ã).FontSize;float S=(float)R;if(Ã.
BlockDefinition.ToString().Contains("LargeLCDPanelWide"))S=S*2.010752f;S=S/m;A=(int)S;P=(int)(17.6/m);L=false;}}else{if(m==0){A=93;m=1f
;P=17;}}Â+=A.ToString()+"\n";}void b(string K){if(K=="Monospace"){µ.Clear();k("\n",0f);k(
"AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz",1f);k("1234567890",1f);k("!@#$%^&*()<>?,./[]{}\\|-=_+`~;':\" ",1f);k("ƒ«°",1f);}else{µ.Clear();k("\n",0f);k("'|",1f);k(
" !`Iijl",1.29166f);k("(),.:;[]{}1ft",1.43076f);k("\"-r",1.57627f);k("*",1.72222f);k("\\°",1.86f);k("/",2.16279f);k("«Lvx_ƒ",
2.325f);k("?7Jcz",2.44736f);k("3FKTabdeghknopqsuy",2.58333f);k("+<>=^~E",2.73529f);k("#0245689CXZ",2.90625f);k("$&GHPUVY",3f);
k("ABDNOQRS",3.20689f);k("%",3.57692f);k("@",3.72f);k("M",3.875f);k("mw",4.04347f);k("W",4.65f);}}public void k(string h,
float g){for(int f=0;f<h.Length;f++)µ.Add(h[f],g);}public string e(float a,float Z){float X=a/Z;if(a>Z)X=1;return Math.Round(
(X*100),1).ToString("F1")+"%";}public string d(float a,float Z=-1,float Y=1){float X=0;if(Z==-1f)X=a;else X=a/Z;float W=Ò
("[]");float V=((float)A)*Y-W;int U=(int)(V/Ò("|"));int É=(int)(X*U);if(É==0&&X>0)É=1;string T="[";for(int f=0;f<U;f++){
if(f<É)T+="|";else T+="'";}T+="]";return T;}public string ï(float a,float Z=-1,float Y=1){float X=0;if(Z==-1f)X=a;else if(
a>Z)X=1;else X=a/Z;string î=Math.Round((X*100),1).ToString("F1")+"%";float í=Ò(" 100.0%");float W=Ò("[]");float V=((float
)A)*Y-í-W;int U=(int)(V/Ò("|"));int É=(int)(X*U);if(É==0&&X>0)É=1;string T="[";for(int f=0;f<U;f++){if(f<É)T+="|";else T
+="'";}T+="]";float ì=Ò(T);float ë=Ò(î);float ê=((float)A)*Y-ì-ë;int é=(int)(ê/Ò(" "));for(int f=0;f<é;f++)T+=" ";T+=î;
return T;}public string è(string ð,string ü,string ú="left"){float ì=Ò(ü);float ë=Ò(ð);float ê=((float)A)-ì-ë;if(ê>0){int é=(
int)(ê/Ò(" "));for(int f=0;f<é;f++)ð+=" ";}else{if(ú=="left"){ð=ô(ð,((float)A-ì)/A);}else ü=ô(ü,((float)A-ë)/A);}return ð+ü
;}public string ù(string ø){float ö=Ò(ø);float ê=((float)A)-ö;if(ê>0){int é=(int)(ê/Ò(" "));for(int f=0;f<é/2;f++)ø=" "+ø
;}else{ø=ô(ø,((float)A-ö)/A);}return ø;}public string õ(string D){if(O)D=ä(D);if(N)D=à(D);return D;}public string ô(
string ó,float ò=1,bool ñ=false,int û=3){float ç=0;string å="";if(û!=0){for(int f=0;f<û;f++)å+=".";}ç=(int)Ò(å);int Ó=0;if(ñ)Ó
=Ñ(ó,(int)(((float)(A))*ò-ç));else Ó=Ñ(ó,(int)(((float)(A))*ò-ç),false);ó=ó.Substring(0,Ó);return ó+å;}public float Ò(
string D){float Î=0;for(int f=0;f<D.Length;f++){Î+=µ[D[f]];}return Î;}public int Ñ(string D,int A,bool Ð=true){int Ï=0;float Î
=0;for(int f=0;f<D.Length;f++){Î+=µ[D[f]];if(Î<A)Ï=f;}int Í=D.Substring(0,Ï).LastIndexOf(" ");if(Í==-1||!Ð)Í=Ï;return Í;}
public string Ì(string D,int A){string Ë="";string Õ="";int æ=Ñ(D,A);Ë+=D.Substring(0,æ)+"\n";Õ+=D.Substring(æ).Trim();if(Ò(Õ)
>A)Ë+=Ì(Õ,A)+"\n";else Ë+=Õ+"\n";return Ë;}public string ä(string D){Â="";string ã="";int â=D.IndexOf("\n");if(â==-1){if(
Ò(D)>A){ã+=Ì(D,A);}else ã+=D+"\n";}else{string[]á=D.Split('\n');for(int f=0;f<á.Length-1;f++){if(Ò(á[f])>A){ã+=Ì(á[f],A);
}else ã+=á[f]+"\n";}}return ã;}public string à(string D,int ß=1,string Þ=null,string Ý=null){int Ü=P;string Û="";string[]
Ú=D.Split('\n');int Ù=Ú.Length;int Ø=0;if(Þ!=null){Û+=Þ+"\n";Ü=Ü-1;}if(Ý!=null)Ü=Ü-1;if(Ü<Ù){for(int f=0;f<Ü;f++){if(f+ª<
Ù){Û+=Ú[f+ª]+"\n";Ø=0;}else{Û+=Ú[Ø]+"\n";Ø++;}}}else{Û=Û+D;}if(Ý!=null)Û+=Ý+"\n";ª+=ß;if((Ù)-1<ª)ª=0;else if(ª<0)ª=Ù-1;
return Û+"&&"+ª;}}static class ď{public static Dictionary<string,string>Ö=new Dictionary<string,string>(){{"intro",
"//***BluePrint_To_Assembler***//"},{"workingintro","//Change this variable to false to add blueprint components to assembler\n//queue"},{"workingvar",
"Components_Added_To_Queue="},{"typeintro","//Change this variable to true to count for heavy armor blocks in the blueprint\n// If set to False it will count light armor blocks"
},{"typevar","Armor_Block_Type_Heavy="},{"lcdprojectorintro",
"//Here you can enter the name of the projector you want to see the components of"},{"lcdprojectorvar","Name_Projector="},{"lcdfronttitleall","Blueprints Requirements Total"},{"lcdfronttitle","BR"},{
"lcdloopoffsetintro","//THIS VALUE CONTROLS FLOW OF SCROLL, DO NOT CHANGE"},{"lcdloopoffsetvar","Scrolloop_Offset="},{"nolcdprojector",
"This LCD has no projector assigned."},};