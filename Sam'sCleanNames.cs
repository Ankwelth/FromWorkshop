
// Sam's Clean Names
public static string VERSION = "0.3.0";
/* Owner: Sam (Magistrator)

https://steamcommunity.com/sharedfiles/filedetails/?id=2426657588

== Usage ==
Run with:
 - "clean" to rename all blocks in the same construct;
 - "clean all" to rename all blocks in all constructs;
 - "clean tags" to clean keywords and [bracketed] tags;
 - "clean simple" to omit ship name;
 A combination of all above can be used.


The ending block name format will be:
 ShipName - Canonical Block Name # <keywords> [tags]
  where:
        # will only be added if there are more than 1 same blocks in the grid;
        <keywords> are known keywords that the custom name contains, see KEYWORDS_TO_KEEP further down;
        [tags] is anything enclosed in brackets like [SAM ADVERTISE] [PAM] [LCD] etc;


Examples with 'clean':

    before: Assembler  !manual
    after: ShipName - Assembler 1 !manual

    before: Programable block 2 [SAM ADVERTISE]
    after: ShipName - *PB* 2 [SAM ADVERTISE]


Overriding block names can be done in the following list: */
public static Dictionary<string, string> RENAMES = new Dictionary<string, string> {
    // {"before", "after" },
    {"Programmable block","*PB*" },
    {"Remote Control","*RC*" },
    {"Large Cargo Container","Cargo Large" },
    {"Medium Cargo Container","Cargo Medium" },
    {"Small Cargo Container","Cargo Small" },
};

// If Custom Names contain the following words they will be kept in the new name:
public static List<string> KEYWORDS_TO_KEEP = new List<string> {
    "Autocrafting",
    "!manual",
    "Ores",
    "Ingots",
    "Tools",
    "Components",
    "Special",
};

// This script will also automatically hide the following blocks from the terminal.
public static List<string> HIDDEN = new List<string> {
    "ButtonPanelLarge",
    "ButtonPanelSmall",
    "LargeSciFiButtonPanel",
    "LargeBlockLandingGear",
    "SmallBlockLandingGear",
    "LargeBlockBatteryBlock",
    "SmallBlockBatteryBlock",
    "LargeGatlingTurret",
    "SmallGatlingTurret",
    "SmallGatlingGun",
    "SmallShipWelder",
    "LargeShipWelder",
    "AirtightHangarDoor",
    "LargeBlockFrontLight",
    "SmallBlockFrontLight",
    "LargeBlockConveyorSorter",
    "SmallBlockConveyorSorter",
    "LargeBlockLargeContainer",
    "SmallBlockLargeContainer",
    "SmallBlockMediumContainer",
    "LargeBlockSmallContainer",
    "SmallBlockSmallContainer",
    "LargeBlockLockerRoom",
    "LargeBlockLockerRoomCorner",
    "VirtualMassLarge",
    "VirtualMassSmall",
    "LargeBlockSmallAtmosphericThrust",
    "SmallBlockSmallAtmosphericThrust",
    "LargeBlockBathroom",
    "LargeBlockBed",
    "LargeBlockCouchCorner",
    "LargeBlockCorner_LCD_2",
    "SmallBlockCorner_LCD_2",
    "LargeBlockCorner_LCD_Flat_2",
    "SmallBlockCorner_LCD_Flat_2",
    "LargeBlockCorner_LCD_Flat_1",
    "SmallBlockCorner_LCD_Flat_1",
    "LargeBlockCorner_LCD_1",
    "SmallBlockCorner_LCD_1",
    "LargeBlockLight_1corner",
    "SmallBlockLight_1corner",
    "LargeBlockLight_2corner",
    "SmallBlockLight_2corner",
    "LargeBlockCouch",
    "LargeCoverWall",
    "LargeBlockCryoChamber",
    "SmallBlockCryoChamber",
    "LargeDecoy",
    "SmallDecoy",
    "LargeBlockDeskCorner",
    "LargeBlockDesk",
    "Freight1",
    "Freight2",
    "Freight3",
    "LargeBlockGyro",
    "SmallBlockGyro",
    "LargeHydrogenTank",
    "SmallHydrogenTank",
    "LargeBlockSmallHydrogenThrust",
    "SmallBlockSmallHydrogenThrust",
    "SmallLight",
    "SmallBlockSmallLight",
    "LargeBlockSmallThrust",
    "SmallBlockSmallThrust",
    "LargeBlockKitchen",
    "LabEquipment",
    "LargeBlockLargeAtmosphericThrust",
    "SmallBlockLargeAtmosphericThrust",
    "LargeBlockLargeHydrogenThrust",
    "SmallBlockLargeHydrogenThrust",
    "LargeBlockLargeThrust",
    "SmallBlockLargeThrust",
    "LargeLCDPanel",
    "SmallLCDPanel",
    "LargeBlockLockers",
    "LargeBlockOffsetDoor",
    "OffsetLight",
    "LargeBlockOxygenFarm",
    "OxygenTank",
    "OxygenTankSmall",
    "PassengerSeatLarge",
    "PassengerSeatSmall",
    "LargeBlockPlanters",
    "LargeEnergyModule",
    "RotatingLightLarge",
    "RotatingLightSmall",
    "LargeBlockSmallAtmosphericThrustSciFi",
    "SmallBlockSmallAtmosphericThrustSciFi",
    "LargeBlockSciFiTerminal",
    "LargeSciFiButtonPanel",
    "LargeBlockSmallThrustSciFi",
    "SmallBlockSmallThrustSciFi",
    "LargeBlockLargeAtmosphericThrustSciFi",
    "SmallBlockLargeAtmosphericThrustSciFi",
    "LargeBlockLargeThrustSciFi",
    "SmallBlockLargeThrustSciFi",
    "LargeLCDPanel3x3",
    "LargeLCDPanel5x3",
    "LargeLCDPanel5x5",
    "LargeSciFiButtonTerminal",
    "SmallSideDoor",
    "LargeBlockSensor",
    "SmallBlockSensor",
    "LargeBlockSlideDoor",
    "Shower",
    "SmallBlockSmallBatteryBlock",
    "LargeHydrogenTankSmall",
    "SmallHydrogenTankSmall",
    "LargeBlockSolarPanel",
    "SmallBlockSolarPanel",
    "LargeBlockSoundBlock",
    "SmallBlockSoundBlock",
    "LargeProductivityModule",
    "GravityGeneratorSphere",
    "StorageShelf1",
    "StorageShelf2",
    "StorageShelf3",
    "LargeTextPanel",
    "SmallTextPanel",
    "TimerBlockLarge",
    "TimerBlockSmall",
    "LargeBlockBathroomOpen",
    "LargeBlockToilet",
    "TransparentLCDLarge",
    "TransparentLCDSmall",
    "LargeLCDPanelWide",
    "SmallLCDPanelWide",
    "LargeBlockWindTurbine",
    "LargeEffectivenessModule",
};
class BlockEvaluater{private Dictionary<String,int>counts=new Dictionary<String,int>();private Dictionary<String,int>
totalCounts=new Dictionary<String,int>();private Dictionary<long,string>customNames=new Dictionary<long,string>();private
Dictionary<long,string>subTypeIDs=new Dictionary<long,string>();private Dictionary<long,List<string>>tagsPerID=new Dictionary<long
,List<string>>();private Dictionary<long,List<string>>keywordsPerID=new Dictionary<long,List<string>>();private MyIni ini
=new MyIni();private static System.Text.RegularExpressions.Regex bracketRegex=new System.Text.RegularExpressions.Regex(
"\\[([!?:=\\.\\-_ a-zA-Z0-9]+)\\]");public BlockEvaluater(ref List<IMyTerminalBlock>blocks){foreach(IMyTerminalBlock block in blocks){this.StoreTags(block
);this.StoreKeywords(block);var subTypeID=block.BlockDefinition.SubtypeId;this.customNames[block.EntityId]=RENAMES.
GetValueOrDefault(block.DefinitionDisplayNameText,block.DefinitionDisplayNameText);this.subTypeIDs[block.EntityId]=subTypeID;if(!this.
totalCounts.ContainsKey(subTypeID)){this.totalCounts[subTypeID]=1;}else{this.totalCounts[subTypeID]=this.totalCounts[subTypeID]+1;}
}}private void StoreTags(IMyTerminalBlock block){foreach(System.Text.RegularExpressions.Match match in bracketRegex.
Matches(block.CustomName)){for(var i=1;i<match.Groups.Count;++i){if(!this.tagsPerID.ContainsKey(block.EntityId)){this.tagsPerID
[block.EntityId]=new List<string>();}this.tagsPerID[block.EntityId].Add("["+match.Groups[i].Value+"]");}}}private void
StoreKeywords(IMyTerminalBlock block){foreach(var keyword in KEYWORDS_TO_KEEP){if(block.CustomName.Contains(" "+keyword)){if(!this.
keywordsPerID.ContainsKey(block.EntityId)){this.keywordsPerID[block.EntityId]=new List<string>();}this.keywordsPerID[block.EntityId].
Add(keyword);}}}private string ContainsNameOverride(string cd){this.ini.Clear();if(!this.ini.TryParse(cd,"CleanNames")){
return"";}var val=this.ini.Get("CleanNames","name");return val.ToString();}private string GetCount(string type){if(this.
totalCounts[type]==1){return"";}if(!this.counts.ContainsKey(type)){this.counts[type]=1;}else{this.counts[type]=this.counts[type]+1;
}if(this.totalCounts[type]>=10){return" "+this.counts[type].ToString("D2");}return" "+this.counts[type].ToString();}
private string GetKeywords(long id){if(!this.keywordsPerID.ContainsKey(id)){return"";}var str="";foreach(var keyword in this.
keywordsPerID[id]){str+=" "+keyword;}return str;}private string GetTags(long id){if(!this.tagsPerID.ContainsKey(id)){return"";}var
str="";foreach(var tag in this.tagsPerID[id]){str+=" "+tag;}return str;}public string GetName(IMyTerminalBlock block,bool
noTags){return this.customNames[block.EntityId]+this.GetCount(this.subTypeIDs[block.EntityId])+(noTags?"":this.GetKeywords(
block.EntityId)+this.GetTags(block.EntityId));}public void Clear(){this.counts.Clear();this.totalCounts.Clear();this.
customNames.Clear();this.subTypeIDs.Clear();foreach(var key in this.tagsPerID.Keys){this.tagsPerID[key].Clear();}this.tagsPerID.
Clear();foreach(var key in this.keywordsPerID.Keys){this.keywordsPerID[key].Clear();}this.keywordsPerID.Clear();}}void Clean(
bool everything,bool simple,bool cleanTags){List<IMyTerminalBlock>allBlocks=new List<IMyTerminalBlock>();List<
IMyTerminalBlock>goodBlocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(allBlocks);foreach(IMyTerminalBlock block in
allBlocks){if(!everything&&!block.IsSameConstructAs(Me)){continue;}goodBlocks.Add(block);}BlockEvaluater counter=new
BlockEvaluater(ref goodBlocks);foreach(IMyTerminalBlock block in goodBlocks){block.CustomName=(simple?"":(block.CubeGrid.CustomName+
" - "))+counter.GetName(block,cleanTags);if(HIDDEN.Contains(block.BlockDefinition.SubtypeId)){block.ShowInTerminal=false;}}
counter.Clear();}void HandleCommand(ref string command){string[]parts=command.Trim().Split(' ');parts.DefaultIfEmpty("");string
arg0=parts.ElementAtOrDefault(0).ToUpper();string arg1=parts.ElementAtOrDefault(1);string arg2=parts.ElementAtOrDefault(2);
string arg3=parts.ElementAtOrDefault(3);arg1=arg1??"";arg2=arg2??"";arg3=arg3??"";try{switch(arg0){case"CLEAN":var everything=
arg1.ToUpper()=="ALL"||arg2.ToUpper()=="ALL"||arg3.ToUpper()=="ALL";var simple=arg1.ToUpper()=="SIMPLE"||arg2.ToUpper()==
"SIMPLE"||arg3.ToUpper()=="SIMPLE";var tags=arg1.ToUpper()=="TAGS"||arg2.ToUpper()=="TAGS"||arg3.ToUpper()=="TAGS";Clean(
everything,simple,tags);break;default:Echo("Nothing to do...");break;}}catch(Exception e){Echo("Command exception -< "+command+
" >-< "+e.Message+" >-");}}Program(){}static class MainHelper{public delegate void Updater(ref string msg);public static string
lastException="";public static void TimedRunIf(ref UpdateType update,UpdateType what,Updater run,ref string argument){if((update&what
)==0){return;}TimeStats.Start(what.ToString());run(ref argument);update&=~what;TimeStats.Stop(what.ToString());}public
static void TimedRunDefault(ref UpdateType update,Updater run,ref string argument){TimedRunIf(ref update,update,run,ref
argument);}public static void WriteStats(Program p){string str=String.Format("CleanNames v{0}\n",VERSION);str+=TimeStats.Results
();str+=String.Format("Load:{0:F3}%\n",100.0*(double)p.Runtime.CurrentInstructionCount/(double)p.Runtime.
MaxInstructionCount);str+=lastException;p.Echo(str);}}void Main(string argument,UpdateType updateSource){try{MainHelper.TimedRunDefault(ref
updateSource,this.HandleCommand,ref argument);MainHelper.WriteStats(this);}catch(Exception e){Echo("Main exception: "+e.Message);}}
static class TimeStats{public static Dictionary<string,DateTime>start=new Dictionary<string,DateTime>();public static
Dictionary<string,TimeSpan>stats=new Dictionary<string,TimeSpan>();public static void Start(string key){start[key]=DateTime.Now;}
public static void Stop(string key){stats[key]=DateTime.Now-start[key];}public static string Results(){string str="";foreach(
KeyValuePair<string,TimeSpan>stat in stats){str+=String.Format("{0}: {1:F4}ms\n",stat.Key,stat.Value.TotalMilliseconds);}return str;
}}