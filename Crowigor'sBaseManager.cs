// Crowigor's Base Manager
// -----------
// by Crowigor
// 
// Tags for custom data
// [CBM:GC] - Global Config
// [CBM:CI] - Custom Items
// [CBM:DC] - Display Config
// [CBM:IM] - Inventory Manager
// [CBM:SC] - Special Containers
// [CBM:IA] - Items Assembling
// [CBM:ID] - Items Disassembling
// [CBM:IC] - Items Collecting
// [CBM:SD] - Stop Drones
// [CBM:RM] - Refinery Manager
// [CBM:DS] - Display Script Status
// [CBM:DI] - Display Items Count
// [CBM:DL] - Display Items Limits
// [CBM:DV] - Display Blocks Volumes
// [CBM:DVR] - Display Blocks Volumes with Remained vector and timers
// 
// Blocks names contains
// CBM-SCAN - Blocks for scan new custom items
// CBM-PRINT - Display block for display all items to custom items strings
// 
public class BlocksManager{private readonly Dictionary<long,IMyTerminalBlock>_storage;private readonly Dictionary<
BlockType,List<long>>_storageByTypes;public enum BlockType{AirVent,Assembler,Battery,Cockpit,Collector,Connector,Container,
CryoChamber,Display,Drill,GasGenerator,GasPowerProducer,GasTank,GravityGenerator,Grinder,Piston,Projector,Reactor,Refinery,Rotor,
SafeZone,Sensor,Sorter,TerminalBlock,TextSurfaceProvider,Turret,Welder}public BlocksManager(IMyGridTerminalSystem grid,string
tag,string ignoreTag,List<BlockType>types=null,List<BlockType>ignoreTypes=null){_storage=new Dictionary<long,
IMyTerminalBlock>();_storageByTypes=new Dictionary<BlockType,List<long>>();foreach(BlockType type in Enum.GetValues(typeof(BlockType))){
if(type==BlockType.TerminalBlock){continue;}if(types!=null&&types.Count>0&&!types.Contains(type)){continue;}if(ignoreTypes
!=null&&ignoreTypes.Count>0&&ignoreTypes.Contains(type)){continue;}_storageByTypes[type]=new List<long>();}var blocks=new
List<IMyTerminalBlock>();grid.SearchBlocksOfName(tag,blocks);blocks.Sort((a,b)=>string.CompareOrdinal(a.CustomName,b.
CustomName));foreach(var block in blocks){if(!block.IsFunctional||block.CustomName.Contains(ignoreTag)||block.CustomData.Contains(
ignoreTag)){continue;}var addToStorage=false;foreach(var blockType in GetBlockTypes(block)){if(!_storageByTypes.ContainsKey(
blockType)){continue;}addToStorage=true;_storageByTypes[blockType].Add(block.EntityId);}if(addToStorage){_storage[block.EntityId]
=block;}}}public IMyTerminalBlock GetBlock(long selector,BlockType blockType=BlockType.TerminalBlock){if(!_storage.
ContainsKey(selector)){return null;}if(blockType!=BlockType.TerminalBlock&&!_storageByTypes[blockType].Contains(selector)){return
null;}return _storage[selector];}public List<IMyTerminalBlock>GetBlocks(BlockType blockType=BlockType.TerminalBlock){if(
blockType==BlockType.TerminalBlock){return _storage.Values.ToList();}var result=new List<IMyTerminalBlock>();if(!_storageByTypes.
ContainsKey(blockType)||_storageByTypes[blockType].Count==0){return result;}foreach(var entityId in _storageByTypes[blockType]){
IMyTerminalBlock value;if(_storage.TryGetValue(entityId,out value)){result.Add(value);}}return result;}public static List<BlockType>
GetBlockTypes(IMyTerminalBlock block){var result=new List<BlockType>();if(block is IMyAirVent){result.Add(BlockType.AirVent);}if(
block is IMyAssembler){result.Add(BlockType.Assembler);}if(block is IMyBatteryBlock){result.Add(BlockType.Battery);}if(block
is IMyCockpit){result.Add(BlockType.Cockpit);}if(block is IMyCollector){result.Add(BlockType.Collector);}if(block is
IMyShipConnector){result.Add(BlockType.Connector);}if(block is IMyCargoContainer){result.Add(BlockType.Container);}if(block is
IMyCryoChamber){result.Add(BlockType.CryoChamber);}if(block is IMyTextPanel){result.Add(BlockType.Display);}if(block is IMyShipDrill){
result.Add(BlockType.Drill);}if(block is IMyGasGenerator){result.Add(BlockType.GasGenerator);}if(block is IMyPowerProducer&&
block is IMyGasTank){result.Add(BlockType.GasPowerProducer);}if(block is IMyGasTank){result.Add(BlockType.GasTank);}if(block
is IMyGravityGenerator){result.Add(BlockType.GravityGenerator);}if(block is IMyShipGrinder){result.Add(BlockType.Grinder);
}if(block is IMyPistonBase){result.Add(BlockType.Piston);}if(block is IMyProjector){result.Add(BlockType.Projector);}if(
block is IMyReactor){result.Add(BlockType.Reactor);}if(block is IMyRefinery){result.Add(BlockType.Refinery);}if(block is
IMyMotorStator){result.Add(BlockType.Rotor);}if(block is IMySafeZoneBlock){result.Add(BlockType.SafeZone);}if(block is IMySensorBlock)
{result.Add(BlockType.Sensor);}if(block is IMyConveyorSorter){result.Add(BlockType.Sorter);}if(block is
IMyTextSurfaceProvider){result.Add(BlockType.TextSurfaceProvider);}if(block is IMyLargeConveyorTurretBase){result.Add(BlockType.Turret);}if(
block is IMyShipWelder){result.Add(BlockType.Welder);}return result;}public void Clear(){_storage.Clear();_storageByTypes.
Clear();}}private const string RootConfigSectionName="ROOT CONFIG SECTION",SectionIndexSeparator="::_";public class
ConfigObject{public readonly string Section;public readonly Dictionary<string,string>Data;public ConfigObject(string section,
Dictionary<string,string>data=null){Section=section;Data=data??new Dictionary<string,string>();}public string Get(string key,
string defaultValue=null){string value;return(Data.TryGetValue(key,out value))?value:defaultValue;}public void Set(string key,
string value=null){Data[key]=value;}public List<string>ToStringList(){var result=new List<string>();foreach(var entry in Data)
{var line=entry.Key;if(!string.IsNullOrEmpty(entry.Value)){line+="="+entry.Value;}result.Add(line);}return result;}public
string DataToString(){return string.Join("\n",ToStringList());}public static ConfigObject Parse(string section,string data="")
{if(string.IsNullOrEmpty(section)||!data.Contains(section)){return null;}var sections=ConfigsHelper.GetSections(data);
List<string>lines;if(!sections.TryGetValue(section,out lines)){return null;}var result=new ConfigObject(section);foreach(var
line in lines){var content=ConfigsHelper.ParseLine(line);if(!string.IsNullOrEmpty(content.Key)){result.Set(content.Key,
content.Value);}}return result;}}public static class ConfigsHelper{public static Dictionary<string,List<string>>GetSections(
string data,bool group=true){var result=new Dictionary<string,List<string>>();var lines=data.Split('\n');if(lines.Length==0){
return result;}var section=RootConfigSectionName;var index=1;result[section]=new List<string>();foreach(var line in data.Split
('\n')){var sectionContent=line.Trim();if(sectionContent.StartsWith("#")||sectionContent.StartsWith(";")){continue;}if(
sectionContent.StartsWith("[")&&sectionContent.EndsWith("]")){section=sectionContent.Substring(1,sectionContent.Length-2).Trim();if(!
group){section=AddSectionIndex(section,index);index++;}if(!result.ContainsKey(section)){result[section]=new List<string>();}
continue;}if(!string.IsNullOrEmpty(section)){result[section].Add(line);}}foreach(var key in result.Keys.ToList()){var list=
result[key];if(list.Count==0||!string.IsNullOrWhiteSpace(list[list.Count-1].Trim())){continue;}list.RemoveAt(list.Count-1);
result[key]=list;}return result;}public static string AddSectionIndex(string section,int index){return section+
SectionIndexSeparator+index;}public static string RemoveSectionIndex(string section){if(string.IsNullOrEmpty(section)){return section;}var
position=section.LastIndexOf(SectionIndexSeparator,StringComparison.Ordinal);if(position<0){return section;}return section.
Substring(0,position);}public static KeyValuePair<string,string>ParseLine(string line){var result=new KeyValuePair<string,string>
(null,null);var content=line.Trim();if(string.IsNullOrEmpty(content)){return result;}var lineParts=content.Split(new[]{
'='},2);var key=lineParts[0].Trim();string value=null;if(lineParts.Length==2){value=lineParts[1].Trim();}return new
KeyValuePair<string,string>(key,value);}public static ConfigObject Merge(string section,List<ConfigObject>configs){var result=new
ConfigObject(section);foreach(var config in configs){if(config==null||config.Data.Count==0){continue;}foreach(var entry in config.
Data){result.Set(entry.Key,entry.Value);}}return result;}public static string ToCustomData(ConfigObject config,string
customData=""){var result=new List<string>();var sections=GetSections(customData);if(config!=null&&!string.IsNullOrEmpty(config.
Section)){sections[config.Section]=config.ToStringList();}var firstSection=false;foreach(var section in sections){if(section.
Key!=RootConfigSectionName){if(!firstSection){firstSection=true;}else{result.Add("");}result.Add("["+section.Key+"]");}
result.AddRange(section.Value);}return string.Join("\n",result.ToArray());}}public class DisplayObject{public string Selector;
public long BlockSelector;public int SurfaceIndex,UpdateDelay,ListingDelay;int _updateCurrentTick,_listingCurrentTick,
_currentLine;private readonly List<List<MySprite>>_lines;private readonly List<string>_textLines;public DisplayObject(string
selector,long blockSelector,int surfaceIndex,int updateDelay=5,int listingDelay=5){Selector=selector;BlockSelector=blockSelector
;SurfaceIndex=surfaceIndex;UpdateDelay=updateDelay;_updateCurrentTick=updateDelay;ListingDelay=listingDelay;
_listingCurrentTick=0;_lines=new List<List<MySprite>>();_textLines=new List<string>();_currentLine=0;}public void Tick(){_updateCurrentTick
++;}public void TickReset(){_updateCurrentTick=0;}public bool NeedUpdate(){return _updateCurrentTick>=UpdateDelay;}public
void ListingTick(){_listingCurrentTick++;}public void ListingReset(){_listingCurrentTick=0;}public bool NeedListing(){return
_listingCurrentTick>=ListingDelay;}public void Listing(int limit){ListingReset();var current=_currentLine;var result=current+limit;if(
_lines.Count-1<result){result=0;}_currentLine=result;}public void AddLine(string text,params MySprite[]sprites){_textLines.Add
(text);_lines.Add(sprites.ToList());}public void AddLine(params MySprite[]sprites){var all=new List<string>();var left=
new List<string>();var center=new List<string>();var right=new List<string>();foreach(var sprite in sprites){if(sprite.
Alignment==TextAlignment.RIGHT){right.Add(sprite.Data);}else if(sprite.Alignment==TextAlignment.CENTER){center.Add(sprite.Data);}
else{left.Add(sprite.Data);}}if(left.Count>0){all.Add(string.Join(" ",left));}if(center.Count>0){all.Add(string.Join(" ",
center));}if(right.Count>0){all.Add(string.Join(" ",right));}AddLine(string.Join(" ",all.ToArray()),sprites);}public void
AddBlankLine(){AddLine("",BlankSprite());}public void AddTextLine(string text,TextAlignment alignment=TextAlignment.LEFT,Color?color
=null){AddLine(text,TextSprite(text,alignment,color));}public void AddCustomTextLine(string text){AddLine(ParseTextSprite
(text));}public List<List<MySprite>>GetLines(int limit=0){if(limit<=0){return _lines;}var start=_currentLine;var count=
Math.Min(limit,_lines.Count-start);return _lines.GetRange(start,count);}public string LinesToString(){return string.Join(
"\n",_textLines);}public void ClearLines(){_lines.Clear();_textLines.Clear();}public static MySprite BlankSprite(){return
TextSprite("");}public static MySprite TextSprite(string text,TextAlignment alignment=TextAlignment.LEFT,Color?color=null){return
new MySprite{Type=SpriteType.TEXT,Data=text,Alignment=alignment,Color=color};}public static MySprite ParseTextSprite(string
text=null){if(string.IsNullOrEmpty(text)){return BlankSprite();}var alignment=TextAlignment.LEFT;var pattern=
@"^(center|right|left)\s*";var match=System.Text.RegularExpressions.Regex.Match(text,pattern,System.Text.RegularExpressions.RegexOptions.
IgnoreCase);if(match.Success){switch(match.Value.Trim().ToLower()){case"center":alignment=TextAlignment.CENTER;break;case"right":
alignment=TextAlignment.RIGHT;break;case"left":alignment=TextAlignment.LEFT;break;}text=System.Text.RegularExpressions.Regex.
Replace(text,pattern,"",System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();}return TextSprite(text,alignment);}
public static RectangleF GetViewport(IMyTextSurface surface){return new RectangleF((surface.TextureSize-surface.SurfaceSize)/
2f,surface.SurfaceSize);}public static List<MySprite>GetSurfaceBorder(IMyTextSurface surface,float border=1f,float padding
=10f){var result=new List<MySprite>();if(border<=0){return result;}var viewport=GetViewport(surface);var outerRectSize=
new Vector2(viewport.Width-2*padding,viewport.Height-2*padding);var rectPosition=new Vector2(viewport.X+viewport.Width/2,
viewport.Y+viewport.Height/2);result.Add(new MySprite{Type=SpriteType.TEXTURE,Data="SquareSimple",Position=rectPosition,Size=
outerRectSize,Color=surface.ScriptForegroundColor,Alignment=TextAlignment.CENTER});var innerRectSize=new Vector2(outerRectSize.X-2*
border,outerRectSize.Y-2*border);result.Add(new MySprite{Type=SpriteType.TEXTURE,Data="SquareSimple",Position=rectPosition,
Size=innerRectSize,Color=surface.ScriptBackgroundColor,Alignment=TextAlignment.CENTER});return result;}public static List<
MySprite>GetSurfaceTitle(IMyTextSurface surface,string title="",string font="Debug",float fontSize=0.8f,float lineHeight=32f,
float border=1f,float padding=10f){var result=new List<MySprite>();if(string.IsNullOrEmpty(title)){return result;}var
viewport=GetViewport(surface);var size=new Vector2(title.Length*15*fontSize,lineHeight);var position=new Vector2(viewport.X+
viewport.Width/2,viewport.Y+lineHeight/2*fontSize);if(border>0){var rectPosition=new Vector2(position.X,viewport.Y+padding+
lineHeight/2);var outerRectSize=new Vector2(size.X+2*padding,lineHeight);var innerRectSize=new Vector2(outerRectSize.X-2*border,
lineHeight-2*border);result.Add(new MySprite{Type=SpriteType.TEXTURE,Data="SquareSimple",Position=rectPosition,Size=outerRectSize,
Color=surface.ScriptForegroundColor,Alignment=TextAlignment.CENTER});result.Add(new MySprite{Type=SpriteType.TEXTURE,Data=
"SquareSimple",Position=rectPosition,Size=innerRectSize,Color=surface.ScriptBackgroundColor,Alignment=TextAlignment.CENTER});}result.
Add(new MySprite{Type=SpriteType.TEXT,Data=title,Position=position,RotationOrScale=fontSize,Color=surface.
ScriptForegroundColor,FontId=font,Alignment=TextAlignment.CENTER});return result;}}public class InventoryHelper{public static MyFixedPoint
TransferToInventories(MyInventoryItem inventoryItem,IMyInventory sourceInventory,List<IMyInventory>destinationInventories,MyFixedPoint amount
=new MyFixedPoint()){var zero=MyFixedPoint.Zero;var result=inventoryItem.Amount;foreach(var destinationInventory in
destinationInventories){if(destinationInventory==sourceInventory){if(amount==zero){return zero;}var exist=destinationInventory.GetItemAmount(
inventoryItem.Type);if(amount<=exist){return zero;}result-=exist;continue;}var transfer=TransferItem(inventoryItem,sourceInventory,
destinationInventory,amount);if(transfer==null){continue;}if(transfer==zero){return zero;}result=(MyFixedPoint)transfer;}return result;}
public static MyFixedPoint?TransferItem(MyInventoryItem item,IMyInventory sourceInventory,IMyInventory destinationInventory,
MyFixedPoint amount=new MyFixedPoint()){var before=GetItemAmount(item,sourceInventory);var zero=MyFixedPoint.Zero;if(!
sourceInventory.CanTransferItemTo(destinationInventory,item.Type)||destinationInventory.IsFull){return null;}var transfer=(amount==zero
||before<=amount)?sourceInventory.TransferItemTo(destinationInventory,item):sourceInventory.TransferItemTo(
destinationInventory,item,amount);if(!transfer){return null;}var after=GetItemAmount(item,sourceInventory);if(amount==zero||amount==before){
return after;}if(amount>before){return after+(amount-before);}var result=(after>0)?before-after:before;return amount-result;}
public static MyFixedPoint GetItemAmount(MyInventoryItem item,IMyInventory inventory){var findItem=inventory.GetItemByID(item.
ItemId);return findItem?.Amount??MyFixedPoint.Zero;}public static MyFixedPoint?TransferFromInventories(MyItemType type,List<
IMyInventory>inventories,IMyInventory destinationInventory,MyFixedPoint amount){var zero=new MyFixedPoint();if(amount==zero||
inventories.Count==0){return null;}var current=destinationInventory.GetItemAmount(type);amount-=current;if(amount<=zero){return
amount;}foreach(var sourceInventory in inventories){var sourceInventoryItems=new List<MyInventoryItem>();sourceInventory.
GetItems(sourceInventoryItems,b=>b.Type==type);foreach(var inventoryItem in sourceInventoryItems){var transfer=TransferItem(
inventoryItem,sourceInventory,destinationInventory,amount);if(transfer!=null){amount=(MyFixedPoint)transfer;}if(amount<=zero){return
amount;}}}return amount;}public static MyFixedPoint?TransferFromInventories(MyItemType type,List<IMyInventory>inventories,
IMyInventory destinationInventory){if(inventories.Count==0){return null;}var before=destinationInventory.GetItemAmount(type);foreach
(var sourceInventory in inventories){var sourceInventoryItems=new List<MyInventoryItem>();sourceInventory.GetItems(
sourceInventoryItems,b=>b.Type==type);foreach(var inventoryItem in sourceInventoryItems){TransferItem(inventoryItem,sourceInventory,
destinationInventory);}}var after=destinationInventory.GetItemAmount(type);return after-before;}public static MyFixedPoint?
TransferFromBlocks(MyItemType type,List<IMyTerminalBlock>blocks,IMyInventory destinationInventory,MyFixedPoint amount){if(blocks.Count==0)
{return null;}var inventories=GetBlocksInventories(blocks);return TransferFromInventories(type,inventories,
destinationInventory,amount);}public static MyFixedPoint?TransferFromBlocks(MyItemType type,List<IMyTerminalBlock>blocks,IMyInventory
destinationInventory){if(blocks.Count==0){return null;}var inventories=GetBlocksInventories(blocks);return TransferFromInventories(type,
inventories,destinationInventory);}public static List<IMyInventory>GetBlocksInventories(List<IMyTerminalBlock>blocks){var
inventories=new List<IMyInventory>();foreach(var block in blocks){if(block==null){continue;}var inventoryCounts=block.
InventoryCount;if(inventoryCounts<=0){continue;}for(var i=0;i<inventoryCounts;i++){var inventory=block.GetInventory(i);if(inventory==
null){continue;}inventories.Add(inventory);}}return inventories;}}public class ItemsManager{public const string
CustomItemsPrefix="CM_CI",UnknownCustomItem="UNKNOWN_ITEM";private readonly Dictionary<string,ItemObject>_storage;private readonly
Dictionary<string,string>_aliases;public ItemsManager(List<ItemObject>customItems=null){List<ItemObject>items;if(customItems==null
||customItems.Count==0){items=ItemsDatabase.GetItems();}else{var dictionary=new Dictionary<string,ItemObject>();foreach(
var item in ItemsDatabase.GetItems()){dictionary[item.Selector]=item;}foreach(var item in customItems){if(!dictionary.
ContainsKey(item.Selector)){dictionary[item.Selector]=item;continue;}var update=dictionary[item.Selector];var needUpdate=false;if(
item.Name!=update.Name&&item.Name!=item.Type.SubtypeId){update.Name=item.Name;needUpdate=true;}if(item.Localization!=update.
Localization&&item.Localization!=item.Type.SubtypeId){update.Localization=item.Localization;needUpdate=true;}if(item.Blueprints.
Count>0){needUpdate=true;foreach(var keyPair in item.Blueprints){update.Blueprints[keyPair.Key]=keyPair.Value;}}if(!
needUpdate){continue;}update.UpdateAliases();dictionary[update.Selector]=update;}items=dictionary.Values.ToList();}_storage=new
Dictionary<string,ItemObject>();_aliases=new Dictionary<string,string>();foreach(var item in items){_storage[item.Selector]=item;
foreach(var alias in item.Aliases){_aliases[alias]=item.Selector;}}}public ItemObject GetItem(string key){var keyLower=key.
ToLower();string selector=null;if(_storage.ContainsKey(key)){selector=key;}else if(_aliases.ContainsKey(keyLower)){selector=
_aliases[keyLower];}return selector==null?null:_storage[selector];}public List<ItemObject>GetList(){return _storage.Values.
ToList();}public void UpdateItem(ItemObject item){if(item==null){return;}_storage[item.Selector]=item;}public void
ClearInventories(){foreach(var item in _storage.Values){item.ClearInventories();}}public void ClearAmounts(){foreach(var item in
_storage.Values){item.ClearAmount();}}public void TransferFromInventories(List<IMyInventory>inventories){foreach(var inventory
in inventories){TransferFromInventory(inventory);}}public void TransferFromInventory(IMyInventory inventory){var items=new
List<MyInventoryItem>();inventory.GetItems(items);foreach(var item in items){var find=GetItem(item.Type.ToString());find?.
Transfer(item,inventory);}}public static List<ItemObject>GetCustomItemsFromString(string input){var result=new List<ItemObject>(
);if(string.IsNullOrEmpty(input)||!input.Contains(CustomItemsPrefix)){return result;}var lines=input.Split('\n');foreach(
var line in lines){if(!line.StartsWith(CustomItemsPrefix)){continue;}var parts=line.Split(new[]{':'},6);if(parts.Length==1)
{continue;}var type=parts[1].Trim();if(string.IsNullOrEmpty(type)||type==UnknownCustomItem){continue;}MyItemType itemType
;try{itemType=MyItemType.Parse(type);}catch(Exception){continue;}type=itemType.ToString();var name=itemType.SubtypeId;if(
parts.Length>2&&!string.IsNullOrEmpty(parts[2].Trim())){name=parts[2].Trim();}var localization=itemType.SubtypeId;if(parts.
Length>3&&!string.IsNullOrEmpty(parts[3].Trim())){localization=parts[3].Trim();}Dictionary<string,string>blueprints=null;if(
parts.Length>4&&!string.IsNullOrEmpty(parts[4].Trim())){var blueprintId=parts[4].Trim();var blueprintRate="1";if(parts.Length
>5&&!string.IsNullOrEmpty(parts[5].Trim())){blueprintRate=parts[5].Trim();}blueprints=new Dictionary<string,string>{[
blueprintId]=blueprintRate};}result.Add(new ItemObject(name,localization,type,blueprints));}return result;}public static List<
string>ScanCustomItems(IMyGridTerminalSystem gridTerminalSystem,string blocksName="ScanItems",string title="[Custom Items]"){
var messages=new List<string>{"INFO - Starting Custom items scanner."};var blocks=new List<IMyTerminalBlock>();
gridTerminalSystem.SearchBlocksOfName(blocksName,blocks);if(blocks.Count==0){messages.Add("ERROR - Blocks with `"+blocksName+
"` not found!");return messages;}IMyTextPanel display=null;var manager=new ItemsManager();var itemsType=new Dictionary<string,
MyItemType>();var blueprintsType=new Dictionary<string,MyDefinitionId>();foreach(var block in blocks){if(block is
IMyCargoContainer){var inventory=block.GetInventory(0);if(inventory==null){continue;}var inventoryItems=new List<MyInventoryItem>();
inventory.GetItems(inventoryItems);foreach(var item in inventoryItems){var key=item.Type.ToString();if(manager.GetItem(key)==null
){itemsType[key]=item.Type;}}continue;}var assembler=block as IMyProductionBlock;if(assembler!=null){var queue=new List<
MyProductionItem>();assembler.GetQueue(queue);foreach(var item in queue){var key=item.BlueprintId.ToString();if(manager.GetItem(key)==
null){blueprintsType[key]=item.BlueprintId;}}continue;}var panel=block as IMyTextPanel;if(panel!=null){display=panel;}}if(
display==null){messages.Add("ERROR - Display with `"+blocksName+"` not found");return messages;}if(itemsType.Count==0){messages
.Add("WARNING - New Items not found");}else{messages.Add("INFO - Find "+itemsType.Count+" new Item(s)");}if(
blueprintsType.Count==0){messages.Add("WARNING - New Blueprints not found");}else{messages.Add("INFO - Find "+blueprintsType.Count+
" new Blueprint(s)");}if(blueprintsType.Count==0&&itemsType.Count==0){return messages;}var lines=new List<string>{title};var blueprintsFind
=new List<string>();if(itemsType.Count>0){foreach(var type in itemsType.Values){var subtype=type.SubtypeId;var
findBlueprints=false;foreach(var blueprint in blueprintsType.Keys){if(!blueprint.Contains(subtype)){continue;}findBlueprints=true;
blueprintsFind.Add(blueprint);lines.Add(string.Join(":",CustomItemsPrefix,type,subtype,subtype,blueprint,"1"));}if(!findBlueprints){
lines.Add(string.Join(":",CustomItemsPrefix,type,subtype,subtype));}}}if(blueprintsType.Count>0){foreach(var type in
blueprintsType.Values){var key=type.ToString();var subtype=type.SubtypeId.ToString();if(blueprintsFind.Contains(key)){continue;}lines.
Add(string.Join(":",CustomItemsPrefix,UnknownCustomItem,subtype,subtype,key,"1"));}}display.ContentType=ContentType.
TEXT_AND_IMAGE;display.WriteText(string.Join("\n",lines.ToArray()));messages.Add("SUCCESS - Result add to "+display.CustomName);return
messages;}public static List<string>PrintItems(ItemsManager manager,IMyGridTerminalSystem gridTerminalSystem,string blocksName=
"PrintItems",string title="[All Items]"){var messages=new List<string>{"INFO - Starting print manager items."};var blocks=new List<
IMyTerminalBlock>();gridTerminalSystem.SearchBlocksOfName(blocksName,blocks);if(blocks.Count==0){messages.Add("ERROR - Blocks with `"+
blocksName+"` not found!");return messages;}IMyTextPanel display=null;foreach(var block in blocks){var panel=block as IMyTextPanel
;if(panel!=null){display=panel;}}if(display==null){messages.Add("ERROR - Display with `"+blocksName+"` not found");return
messages;}var list=manager.GetList();messages.Add("INFO - Find "+list.Count+" items");var lines=new List<string>{title};foreach(
var itemObject in list){lines.AddRange(itemObject.ToStringList());}display.ContentType=ContentType.TEXT_AND_IMAGE;display.
WriteText(string.Join("\n",lines.ToArray()));messages.Add("SUCCESS - Result add to "+display.CustomName);return messages;}}public
class ItemObject{public readonly string Selector;public string Name,Localization;public MyItemType Type;public List<string>
Aliases;public readonly Dictionary<MyDefinitionId,MyFixedPoint>Blueprints;public List<IMyInventory>Inventories;public
ItemAmountsObject Amounts;public ItemObject(string name,string localization,string type,Dictionary<string,string>blueprints=null){
Selector=type.Replace("MyObjectBuilder_","");Name=name;Localization=localization;Type=MyItemType.Parse(type);Blueprints=new
Dictionary<MyDefinitionId,MyFixedPoint>();if(blueprints!=null){foreach(var entry in blueprints){var blueprint=MyDefinitionId.Parse
(entry.Key);Blueprints[blueprint]=MyFixedPoint.DeserializeString(entry.Value);}}UpdateAliases();ClearInventories();
ClearAmount();}public void UpdateAliases(){Aliases=new List<string>{Selector.ToLower(),Name.ToLower(),Localization.ToLower(),Type.
ToString().ToLower(),};if(Blueprints.Count<=0){return;}foreach(var blueprint in Blueprints.Keys){Aliases.Add(blueprint.ToString(
).ToLower());}}public bool IsCraftable(){return(Blueprints.Count>0);}public string Title(string language="local"){if(
language.ToLower()=="source"){return Name;}return Localization;}public void ClearAmount(){if(Amounts!=null){Amounts.Clear();}
else{Amounts=new ItemAmountsObject();}}public void ClearInventories(){if(Inventories!=null){Inventories.Clear();}else{
Inventories=new List<IMyInventory>();}}public void Transfer(MyInventoryItem inventoryItem,IMyInventory sourceInventory,MyFixedPoint
amount=new MyFixedPoint()){if(Inventories.Count==0){return;}InventoryHelper.TransferToInventories(inventoryItem,
sourceInventory,Inventories,amount);}public List<string>ToStringList(){var lines=new List<string>();if(Blueprints.Count==0){lines.Add(
string.Join(":",ItemsManager.CustomItemsPrefix,Type.ToString(),Name,Localization));return lines;}foreach(var blueprint in
Blueprints){lines.Add(string.Join(":",ItemsManager.CustomItemsPrefix,Type.ToString(),Name,Localization,blueprint.Key.ToString(),
blueprint.Value));}return lines;}}public class ItemAmountsObject{public bool IsNew;public MyFixedPoint Exist,Assembling,
AssemblingQuota,Disassembling,DisassemblingQuota;public ItemAmountsObject(){Clear();}public override string ToString(){var result=
ValueToString(Exist);if(AssemblingQuota>0||DisassemblingQuota>0){result+="/";MyFixedPoint quota=-1;if(AssemblingQuota>=0){quota=
AssemblingQuota;}if(DisassemblingQuota>=0){if(quota>=0&&DisassemblingQuota<quota){quota=DisassemblingQuota;}else if(quota<0){quota=
DisassemblingQuota;}}result+=ValueToString(quota);}if(Assembling<=0&&Disassembling<=0){return result;}result+=" (";if(Assembling>0){result
+="+"+ValueToString(Assembling);}if(Disassembling>0){result+="0"+ValueToString(Disassembling);}result+=")";return result;}
public void Clear(){IsNew=true;Exist=MyFixedPoint.Zero;Assembling=MyFixedPoint.Zero;AssemblingQuota=-1;Disassembling=
MyFixedPoint.Zero;DisassemblingQuota=-1;}public static string ValueToString(MyFixedPoint value){var result=(double)value;if(result>=
1000000){return(result/1000000.0).ToString("0.#")+"M";}if(result>=1000){return(result/1000.0).ToString("0.#")+"K";}return
result.ToString("0");}}public class ItemsDatabase{public static List<ItemObject>GetItems(){var items=new List<ItemObject>{new
ItemObject("S-10 Pistol Magazine","Магазин пистолета S-10","MyObjectBuilder_AmmoMagazine/SemiAutoPistolMagazine",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0010_SemiAutoPistolMagazine","1"}}),new ItemObject(
"S-20A Pistol Magazine","Магазин пистолета S-20A","MyObjectBuilder_AmmoMagazine/FullAutoPistolMagazine",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0020_FullAutoPistolMagazine","1"}}),new ItemObject("S-10E Pistol Magazine","Магазин пистолета S-10E",
"MyObjectBuilder_AmmoMagazine/ElitePistolMagazine",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0030_ElitePistolMagazine","1"}}),new
ItemObject("Flare Gun Magazine","Магазин для ракетницы","MyObjectBuilder_AmmoMagazine/FlareClip",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0051_FlareGunMagazine","1"}}),new ItemObject("Fireworks Blue","Фейерверк синий","MyObjectBuilder_AmmoMagazine/FireworksBoxBlue",new Dictionary
<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0060_FireworksBoxBlue","1"}}),new ItemObject(
"Fireworks Green","Фейерверк зеленый","MyObjectBuilder_AmmoMagazine/FireworksBoxGreen",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0061_FireworksBoxGreen","1"}}),new ItemObject("Fireworks Red","Фейерверк красный","MyObjectBuilder_AmmoMagazine/FireworksBoxRed",new Dictionary
<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0062_FireworksBoxRed","1"}}),new ItemObject(
"Fireworks Pink","Фейерверк розовый","MyObjectBuilder_AmmoMagazine/FireworksBoxPink",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0064_FireworksBoxPink","1"}}),new ItemObject("Fireworks Yellow","Фейерверк желтый","MyObjectBuilder_AmmoMagazine/FireworksBoxYellow",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0063_FireworksBoxYellow","1"}}),new ItemObject(
"Fireworks Rainbow","Фейерверк радужный","MyObjectBuilder_AmmoMagazine/FireworksBoxRainbow",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0065_FireworksBoxRainbow","1"}}),new ItemObject("MR-20 Rifle Magazine","Магазин винтовки MR-20",
"MyObjectBuilder_AmmoMagazine/AutomaticRifleGun_Mag_20rd",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0040_AutomaticRifleGun_Mag_20rd","1"}}),new
ItemObject("MR-50A Rifle Magazine","Магазин винтовки MR-50A","MyObjectBuilder_AmmoMagazine/RapidFireAutomaticRifleGun_Mag_50rd",
new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0050_RapidFireAutomaticRifleGun_Mag_50rd","1"}}
),new ItemObject("MR-8P Rifle Magazine","Магазин винтовки MR-8P",
"MyObjectBuilder_AmmoMagazine/PreciseAutomaticRifleGun_Mag_5rd",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0060_PreciseAutomaticRifleGun_Mag_5rd","1"}
}),new ItemObject("MR-30E Rifle Magazine","Магазин винтовки MR-30E",
"MyObjectBuilder_AmmoMagazine/UltimateAutomaticRifleGun_Mag_30rd",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0070_UltimateAutomaticRifleGun_Mag_30rd",
"1"}}),new ItemObject("5.56x45mm NATO magazine","Магазин 5.56x45мм НАТО","MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm"),new
ItemObject("Autocannon Magazine","Магазин автопушки","MyObjectBuilder_AmmoMagazine/AutocannonClip",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0090_AutocannonClip","1"}}),new ItemObject("Gatling Ammo Box","Боеприпасы 25x184 мм НАТО","MyObjectBuilder_AmmoMagazine/NATO_25x184mm",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0080_NATO_25x184mmMagazine","1"}}),new ItemObject("Rocket"
,"Ракета","MyObjectBuilder_AmmoMagazine/Missile200mm",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0100_Missile200mm","1"}}),new ItemObject("Artillery Shell","Артиллерийский снаряд","MyObjectBuilder_AmmoMagazine/LargeCalibreAmmo",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0120_LargeCalibreAmmo","1"}}),new ItemObject(
"Assault Cannon Shell","Снаряд штурмовой пушки","MyObjectBuilder_AmmoMagazine/MediumCalibreAmmo",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0110_MediumCalibreAmmo","1"}}),new ItemObject("Large Railgun Sabot","Крупный снаряд рельсотрона",
"MyObjectBuilder_AmmoMagazine/LargeRailgunAmmo",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0140_LargeRailgunAmmo","1"}}),new
ItemObject("Small Railgun Sabot","Малый снаряд рельсотрона","MyObjectBuilder_AmmoMagazine/SmallRailgunAmmo",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/Position0130_SmallRailgunAmmo","1"}}),new ItemObject("Construction Comp.",
"Строительные компоненты","MyObjectBuilder_Component/Construction",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/ConstructionComponent","1"}}),new ItemObject("Metal Grid","Компонент решётки","MyObjectBuilder_Component/MetalGrid",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/MetalGrid","1"}}),new ItemObject("Interior Plate","Внутренняя пластина",
"MyObjectBuilder_Component/InteriorPlate",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/InteriorPlate","1"}}),new ItemObject("Steel Plate",
"Стальная пластина","MyObjectBuilder_Component/SteelPlate",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/SteelPlate",
"1"}}),new ItemObject("Girder","Балка","MyObjectBuilder_Component/Girder",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/GirderComponent","1"}}),new ItemObject("Small Steel Tube","Малая трубка","MyObjectBuilder_Component/SmallTube",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/SmallTube","1"}}),new ItemObject("Large Steel Tube","Большая стальная труба",
"MyObjectBuilder_Component/LargeTube",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/LargeTube","1"}}),new ItemObject("Motor","Мотор",
"MyObjectBuilder_Component/Motor",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/MotorComponent","1"}}),new ItemObject("Display",
"Экран","MyObjectBuilder_Component/Display",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Display","1"}})
,new ItemObject("Bulletproof Glass","Бронированное стекло","MyObjectBuilder_Component/BulletproofGlass",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/BulletproofGlass","1"}}),new ItemObject("Superconductor","Сверхпроводник"
,"MyObjectBuilder_Component/Superconductor",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Superconductor","1"}}),new ItemObject("Computer","Компьютер","MyObjectBuilder_Component/Computer",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/ComputerComponent","1"}}),new ItemObject("Reactor Comp.","Компоненты реактора","MyObjectBuilder_Component/Reactor",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/ReactorComponent","1"}}),new ItemObject("Thruster Comp.",
"Детали ионного ускорителя","MyObjectBuilder_Component/Thrust",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/ThrustComponent"
,"1"}}),new ItemObject("Gravity Comp.","Компоненты гравитационного генератора",
"MyObjectBuilder_Component/GravityGenerator",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/GravityGeneratorComponent","1"}}),new ItemObject(
"Medical Comp.","Медицинские компоненты","MyObjectBuilder_Component/Medical",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/MedicalComponent","1"}}),new ItemObject("Radio-comm Comp.","Радиокомпоненты","MyObjectBuilder_Component/RadioCommunication",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/RadioCommunicationComponent","1"}}),new ItemObject(
"Detector Comp.","Компоненты детектора","MyObjectBuilder_Component/Detector",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/DetectorComponent","1"}}),new ItemObject("Explosives","Взрывчатка","MyObjectBuilder_Component/Explosives",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/ExplosivesComponent","1"}}),new ItemObject("Solar Cell","Солнечная ячейка","MyObjectBuilder_Component/SolarCell",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/SolarCell","1"}}),new ItemObject("Power Cell","Энергоячейка",
"MyObjectBuilder_Component/PowerCell",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/PowerCell","1"}}),new ItemObject("Parachute Canvas"
,"Полотно для парашютов","MyObjectBuilder_Component/Canvas",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0030_Canvas","1"}}),new ItemObject("Engineer Plushie","Мягкая игрушка инженера","MyObjectBuilder_Component/EngineerPlushie",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/EngineerPlushie","1"}}),new ItemObject("Sabiroid Plushie",
"Плюшевый сабироид","MyObjectBuilder_Component/SabiroidPlushie",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/SabiroidPlushie","1"}}),new ItemObject("Prototech Frame","Прототех-рама","MyObjectBuilder_Component/PrototechFrame",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/PrototechFrame","1"}}),new ItemObject("Prototech Panel","Прототех-панель"
,"MyObjectBuilder_Component/PrototechPanel",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/PrototechPanel","1"}}),new ItemObject("Prototech Capacitor","Прототех-конденсатор","MyObjectBuilder_Component/PrototechCapacitor",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/PrototechCapacitor","1"}}),new ItemObject(
"Prototech Propulsion Unit","Двигательный прототех-модуль","MyObjectBuilder_Component/PrototechPropulsionUnit",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/PrototechPropulsionUnit","1"}}),new ItemObject("Prototech Machinery","Прототех-механизм","MyObjectBuilder_Component/PrototechMachinery",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/PrototechMachinery","1"}}),new ItemObject("Prototech Circuitry",
"Прототех-схема","MyObjectBuilder_Component/PrototechCircuitry",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/PrototechCircuitry","1"}}),new ItemObject("Prototech Cooling Unit","Прототех-охладитель","MyObjectBuilder_Component/PrototechCoolingUnit",
new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/PrototechCoolingUnit","1"}}),new ItemObject("Zone Chip"
,"Ключ безопасности","MyObjectBuilder_Component/ZoneChip",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/ZoneChip","1"}}),new ItemObject("GoodAI Bot Feedback","Отзывы о ботах GoodAI",
"MyObjectBuilder_PhysicalGunObject/GoodAIRewardPunishmentTool"),new ItemObject("Stone","Камень","MyObjectBuilder_Ore/Stone"),new ItemObject("Ice","Лед","MyObjectBuilder_Ore/Ice"),new
ItemObject("Iron Ore","Железная руда","MyObjectBuilder_Ore/Iron"),new ItemObject("Nickel Ore","Никелевая руда",
"MyObjectBuilder_Ore/Nickel"),new ItemObject("Cobalt Ore","Кобальтовая руда","MyObjectBuilder_Ore/Cobalt"),new ItemObject("Magnesium Ore",
"Магниевая руда","MyObjectBuilder_Ore/Magnesium"),new ItemObject("Silicon Ore","Кремниевая руда","MyObjectBuilder_Ore/Silicon"),new
ItemObject("Silver Ore","Серебряная руда","MyObjectBuilder_Ore/Silver"),new ItemObject("Gold Ore","Золотая руда",
"MyObjectBuilder_Ore/Gold"),new ItemObject("Platinum Ore","Платиновая руда","MyObjectBuilder_Ore/Platinum"),new ItemObject("Uranium Ore",
"Урановая руда","MyObjectBuilder_Ore/Uranium"),new ItemObject("Scrap Metal","Металлолом","MyObjectBuilder_Ore/Scrap"),new ItemObject(
"Gravel","Гравий","MyObjectBuilder_Ingot/Stone",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/HydrogenBottlesRefill","0.9"},{"MyObjectBuilder_BlueprintDefinition/IceToOxygen","0.9"},{
"MyObjectBuilder_BlueprintDefinition/Position0010_StoneOreToIngotBasic","1.4"},{"MyObjectBuilder_BlueprintDefinition/StoneOreToIngot","14"},{
"MyObjectBuilder_BlueprintDefinition/StoneOreToIngot_Deconstruction","1"}}),new ItemObject("Iron Ingot","Железный слиток","MyObjectBuilder_Ingot/Iron",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/IronOreToIngot","0.7"},{"MyObjectBuilder_BlueprintDefinition/ScrapIngotToIronIngot","0.8"},{
"MyObjectBuilder_BlueprintDefinition/ScrapToIronIngot","0.8"}}),new ItemObject("Nickel Ingot","Никелевый слиток","MyObjectBuilder_Ingot/Nickel",new Dictionary<string,string>{
{"MyObjectBuilder_BlueprintDefinition/NickelOreToIngot","0.4"}}),new ItemObject("Cobalt Ingot","Кобальтовый слиток",
"MyObjectBuilder_Ingot/Cobalt",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/CobaltOreToIngot","0.3"}}),new ItemObject(
"Magnesium Powder","Магниевый слиток","MyObjectBuilder_Ingot/Magnesium",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/MagnesiumOreToIngot","0.007"}}),new ItemObject("Silicon Wafer","Кремниевая пластина","MyObjectBuilder_Ingot/Silicon",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/SiliconOreToIngot","0.7"}}),new ItemObject("Silver Ingot","Серебряный слиток",
"MyObjectBuilder_Ingot/Silver",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/SilverOreToIngot","0.1"}}),new ItemObject(
"Gold Ingot","Золотой слиток","MyObjectBuilder_Ingot/Gold",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/GoldOreToIngot","0.01"}}),new ItemObject("Platinum Ingot","Платиновый слиток","MyObjectBuilder_Ingot/Platinum",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/PlatinumOreToIngot","0.005"}}),new ItemObject("Uranium Ingot","Урановый слиток",
"MyObjectBuilder_Ingot/Uranium",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/UraniumOreToIngot","0.01"}}),new ItemObject(
"Prototech Scrap","Прототех-лом","MyObjectBuilder_Ingot/PrototechScrap",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/PrototechScrap","1"}}),new ItemObject("Welder","Сварщик","MyObjectBuilder_PhysicalGunObject/WelderItem",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0090_Welder","1"}}),new ItemObject("Enhanced Welder","Улучшенный сварщик","MyObjectBuilder_PhysicalGunObject/Welder2Item",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0100_Welder2","1"}}),new ItemObject("Proficient Welder",
"Продвинутый сварщик","MyObjectBuilder_PhysicalGunObject/Welder3Item",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0110_Welder3","1"}}),new ItemObject("Elite Welder","Элитный сварщик","MyObjectBuilder_PhysicalGunObject/Welder4Item",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0120_Welder4","1"}}),new ItemObject("Grinder","Резак",
"MyObjectBuilder_PhysicalGunObject/AngleGrinderItem",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0010_AngleGrinder","1"}}),new ItemObject(
"Enhanced Grinder","Улучшенная болгарка","MyObjectBuilder_PhysicalGunObject/AngleGrinder2Item",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0020_AngleGrinder2","1"}}),new ItemObject("Proficient Grinder","Продвинутая болгарка","MyObjectBuilder_PhysicalGunObject/AngleGrinder3Item"
,new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0030_AngleGrinder3","1"}}),new ItemObject(
"Elite Grinder","Элитная болгарка","MyObjectBuilder_PhysicalGunObject/AngleGrinder4Item",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0040_AngleGrinder4","1"}}),new ItemObject("Hand Drill","Ручной бур","MyObjectBuilder_PhysicalGunObject/HandDrillItem",new Dictionary<string
,string>{{"MyObjectBuilder_BlueprintDefinition/Position0050_HandDrill","1"}}),new ItemObject("Enhanced Hand Drill",
"Улучшенный ручной бур","MyObjectBuilder_PhysicalGunObject/HandDrill2Item",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0060_HandDrill2","1"}}),new ItemObject("Proficient Hand Drill","Продвинутый ручной бур",
"MyObjectBuilder_PhysicalGunObject/HandDrill3Item",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0070_HandDrill3","1"}}),new ItemObject(
"Elite Hand Drill","Элитный ручной бур","MyObjectBuilder_PhysicalGunObject/HandDrill4Item",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0080_HandDrill4","1"}}),new ItemObject("Flare Gun","Ракетница","MyObjectBuilder_PhysicalGunObject/FlareGunItem",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/Position0050_FlareGun","1"}}),new ItemObject("S-10 Pistol","Пистолет S-10",
"MyObjectBuilder_PhysicalGunObject/SemiAutoPistolItem",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0010_SemiAutoPistol","1"}}),new ItemObject(
"S-20A Pistol","Пистолет S-20A","MyObjectBuilder_PhysicalGunObject/FullAutoPistolItem",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0020_FullAutoPistol","1"}}),new ItemObject("S-10E Pistol","Пистолет S-10E","MyObjectBuilder_PhysicalGunObject/ElitePistolItem",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0030_EliteAutoPistol","1"}}),new ItemObject("MR-20 Rifle",
"Винтовка MR-20","MyObjectBuilder_PhysicalGunObject/AutomaticRifleItem",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0040_AutomaticRifle","1"}}),new ItemObject("MR-8P Rifle","Винтовка MR-8P","MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0060_PreciseAutomaticRifle","1"}}),new ItemObject(
"MR-50A Rifle","Винтовка MR-50A","MyObjectBuilder_PhysicalGunObject/RapidFireAutomaticRifleItem",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0050_RapidFireAutomaticRifle","1"}}),new ItemObject("MR-30E Rifle","Винтовка MR-30E","MyObjectBuilder_PhysicalGunObject/UltimateAutomaticRifleItem",
new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0070_UltimateAutomaticRifle","1"}}),new
ItemObject("RO-1 Rocket Launcher","Ракетница RO-1","MyObjectBuilder_PhysicalGunObject/BasicHandHeldLauncherItem",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0080_BasicHandHeldLauncher","1"}}),new ItemObject(
"PRO-1 Rocket Launcher","Ракетница PRO-1","MyObjectBuilder_PhysicalGunObject/AdvancedHandHeldLauncherItem",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0090_AdvancedHandHeldLauncher","1"}}),new ItemObject("Oxygen Bottle","Кислородный баллон","MyObjectBuilder_OxygenContainerObject/OxygenBottle",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0010_OxygenBottle","1"}}),new ItemObject("Hydrogen Bottle"
,"Водородный баллон","MyObjectBuilder_GasContainerObject/HydrogenBottle",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0020_HydrogenBottle","1"}}),new ItemObject("Medkit","Аптечка","MyObjectBuilder_ConsumableItem/Medkit",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0021_Medkit","1"}}),new ItemObject("Powerkit","Внешний аккумулятор","MyObjectBuilder_ConsumableItem/Powerkit",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/Position0022_Powerkit","1"}}),new ItemObject("Anti-Radiation Medkit",
"Противорадиационная аптечка","MyObjectBuilder_ConsumableItem/RadiationKit"),new ItemObject("Datapad","Инфопланшет","MyObjectBuilder_Datapad/Datapad"
,new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0040_Datapad","1"}}),new ItemObject(
"Package","Пакет","MyObjectBuilder_Package/Package"),new ItemObject("Space Credit","Космокредит",
"MyObjectBuilder_PhysicalObject/SpaceCredit"),new ItemObject("CubePlacer","CubePlacer","MyObjectBuilder_PhysicalGunObject/CubePlacerItem"),new ItemObject(
"Old Scrap Metal","Старый металлолом","MyObjectBuilder_Ingot/Scrap"),new ItemObject("Organic","Органика","MyObjectBuilder_Ore/Organic"),
new ItemObject("Clang Kola","Кланг-Кола","MyObjectBuilder_ConsumableItem/ClangCola"),new ItemObject("Cosmic Coffee",
"Космокофе","MyObjectBuilder_ConsumableItem/CosmicCoffee"),new ItemObject("Meal Pack (Kelp Crisp)","Паек (хрустящие водоросли)",
"MyObjectBuilder_ConsumableItem/MealPack_KelpCrisp",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0030_MealPack_KelpCrisp","1"}}),new
ItemObject("Meal Pack (Fruit Bar)","Паек (фруктовый батончик)","MyObjectBuilder_ConsumableItem/MealPack_FruitBar",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0040_MealPack_FruitBar","1"}}),new ItemObject(
"Meal Pack (Garden Slaw)","Паек (садовый салат)","MyObjectBuilder_ConsumableItem/MealPack_GardenSlaw",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0050_MealPack_GardenSlaw","1"}}),new ItemObject("Meal Pack (Red Pellets)","Паек (красные брикеты)",
"MyObjectBuilder_ConsumableItem/MealPack_RedPellets",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0060_MealPack_RedPellets","1"}}),new
ItemObject("Meal Pack (Chili)","Паек (перец чили)","MyObjectBuilder_ConsumableItem/MealPack_Chili",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0070_MealPack_Chili","1"}}),new ItemObject("Meal Pack (Ramen)","Паек (лапша)","MyObjectBuilder_ConsumableItem/MealPack_Ramen",new Dictionary
<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0090_MealPack_Ramen","1"}}),new ItemObject(
"Meal Pack (Flatbread)","Паек (лепешка)","MyObjectBuilder_ConsumableItem/MealPack_Flatbread",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0080_MealPack_Flatbread","1"}}),new ItemObject("Meal Pack (Fruit Pastry)","Паек (фруктовая выпечка)",
"MyObjectBuilder_ConsumableItem/MealPack_FruitPastry",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0100_MealPack_FruitPastry","1"}}),new
ItemObject("Meal Pack (Veggie Burger)","Паек (веганский бургер)","MyObjectBuilder_ConsumableItem/MealPack_VeggieBurger",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0110_MealPack_VeggieBurger","1"}}),new ItemObject(
"Meal Pack (Green Pellets)","Паек (зеленые брикеты)","MyObjectBuilder_ConsumableItem/MealPack_GreenPellets",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0130_MealPack_GreenPellets","1"}}),new ItemObject("Meal Pack (Curry)","Паек (карри)","MyObjectBuilder_ConsumableItem/MealPack_Curry",new Dictionary
<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0120_MealPack_Curry","1"}}),new ItemObject(
"Meal Pack (Dumplings)","Паек (клецки)","MyObjectBuilder_ConsumableItem/MealPack_Dumplings",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0140_MealPack_Dumplings","1"}}),new ItemObject("Meal Pack (Spaghetti)","Паек (спагетти)","MyObjectBuilder_ConsumableItem/MealPack_Spaghetti",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0150_MealPack_Spaghetti","1"}}),new ItemObject(
"Meal Pack (Lasagna)","Паек (лазанья)","MyObjectBuilder_ConsumableItem/MealPack_Lasagna",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0160_MealPack_Lasagna","1"}}),new ItemObject("Meal Pack (Burrito)","Паек (буррито)","MyObjectBuilder_ConsumableItem/MealPack_Burrito",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0170_MealPack_Burrito","1"}}),new ItemObject(
"Meal Pack (Frontier Stew)","Паек (приграничное рагу)","MyObjectBuilder_ConsumableItem/MealPack_FrontierStew",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0180_MealPack_FrontierStew","1"}}),new ItemObject("Meal Pack (Seared Sabiroid)","Паек (обжаренный сабироид)",
"MyObjectBuilder_ConsumableItem/MealPack_SearedSabiroid",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0190_MealPack_SearedSabiroid","1"}}),new
ItemObject("Meal Pack (Steak Dinner)","Паек (стейк на ужин)","MyObjectBuilder_ConsumableItem/MealPack_SteakDinner",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0200_MealPack_SteakDinner","1"}}),new ItemObject("Algae",
"Водоросли","MyObjectBuilder_PhysicalObject/Algae"),new ItemObject("Fruit","Фрукты","MyObjectBuilder_ConsumableItem/Fruit"),new
ItemObject("Grain","Злаки","MyObjectBuilder_PhysicalObject/Grain"),new ItemObject("Mushrooms","Грибы",
"MyObjectBuilder_ConsumableItem/Mushrooms"),new ItemObject("Vegetables","Овощи","MyObjectBuilder_ConsumableItem/Vegetables"),new ItemObject("Mammal Meat (Raw)",
"Мясо млекопитающего (сырое)","MyObjectBuilder_ConsumableItem/MammalMeatRaw"),new ItemObject("Mammal Meat (Cooked)",
"Мясо млекопитающего (обработанное)","MyObjectBuilder_ConsumableItem/MammalMeatCooked",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0010_CookMammalMeat","1"}}),new ItemObject("Insect Meat (Raw)","Мясо насекомого (сырое)","MyObjectBuilder_ConsumableItem/InsectMeatRaw"),new
ItemObject("Insect Meat (Cooked)","Мясо насекомого (обработанное)","MyObjectBuilder_ConsumableItem/InsectMeatCooked",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Position0020_CookSpiderMeat","1"}}),new ItemObject(
"Meal Pack (Unknown)","Паек (неизвестно)","MyObjectBuilder_ConsumableItem/MealPack_Unknown"),new ItemObject("Meal Pack (Food Paste)",
"Паек (пищевая паста)","MyObjectBuilder_ConsumableItem/MealPack_FoodPaste"),new ItemObject("Meal Pack (Synth Loaf)",
"Паек (синтетический хлеб)","MyObjectBuilder_ConsumableItem/MealPack_SynthLoaf"),new ItemObject("Meal Pack (Clang Crunchies)",
"Паек (хрустящий Кланг)","MyObjectBuilder_ConsumableItem/MealPack_ClangCrunchies"),new ItemObject("Meal Pack (Banana Beef)",
"Паек (банановый бифштекс)","MyObjectBuilder_ConsumableItem/MealPack_BananaBeef"),new ItemObject("Meal Pack (Hardtack)","Паек (сухари)",
"MyObjectBuilder_ConsumableItem/MealPack_Hardtack"),new ItemObject("Meal Pack (Expired Slop)","Паек (просрочка)","MyObjectBuilder_ConsumableItem/MealPack_ExpiredSlop"),
new ItemObject("Fruit Seeds","Семена фруктов","MyObjectBuilder_SeedItem/Fruit",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0010_Seeds_Fruit","1"}}),new ItemObject("Grain Seeds","Семена злаков","MyObjectBuilder_SeedItem/Grain",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0020_Seeds_Grain","1"}}),new ItemObject("Mushroom Spores","Споры грибов","MyObjectBuilder_SeedItem/Mushrooms",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/Position0040_Spores_Mushrooms","1"}}),new ItemObject("Vegetable Seeds",
"Семена овощей","MyObjectBuilder_SeedItem/Vegetables",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/Position0030_Seeds_Vegetables","1"}}),new ItemObject("Paint Chemicals","Paint Chemicals","MyObjectBuilder_AmmoMagazine/PaintGunMag",new Dictionary<
string,string>{{"MyObjectBuilder_BlueprintDefinition/Blueprint_PaintGunMag","1"}}),new ItemObject("Paint Gun","Paint Gun",
"MyObjectBuilder_PhysicalGunObject/PhysicalPaintGun",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/Blueprint_PaintGun","1"}}),new ItemObject(
"Admin Personal Shield","Админский персональный щит","MyObjectBuilder_GasContainerObject/CSEPersonalShield_Admin"),new ItemObject(
"Uranium Personal Shield","Урановый персональный щит","MyObjectBuilder_GasContainerObject/CSEPersonalShield_Rank5",new Dictionary<string,string>{
{"MyObjectBuilder_BlueprintDefinition/CSEPersonalShield_Rank5_Item","1"}}),new ItemObject("Platinum Personal Shield",
"Платиновый персональный щит","MyObjectBuilder_GasContainerObject/CSEPersonalShield_Rank4",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/CSEPersonalShield_Rank4_Item","1"}}),new ItemObject("Golden Personal Shield","Золотой персональный щит",
"MyObjectBuilder_GasContainerObject/CSEPersonalShield_Rank3",new Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/CSEPersonalShield_Rank3_Item","1"}}),new ItemObject
("Silver Personal Shield","Серебряный персональный щит","MyObjectBuilder_GasContainerObject/CSEPersonalShield_Rank2",new
Dictionary<string,string>{{"MyObjectBuilder_BlueprintDefinition/CSEPersonalShield_Rank2_Item","1"}}),new ItemObject(
"Cobalt Personal Shield","Кобальтовый персональный щит","MyObjectBuilder_GasContainerObject/CSEPersonalShield_Rank1",new Dictionary<string,
string>{{"MyObjectBuilder_BlueprintDefinition/CSEPersonalShield_Rank1_Item","1"}}),new ItemObject("Silicon Personal Shield",
"Кремниевый персональный щит","MyObjectBuilder_GasContainerObject/CSEPersonalShield_Rank0",new Dictionary<string,string>{{
"MyObjectBuilder_BlueprintDefinition/CSEPersonalShield_Rank0_Item","1"}}),new ItemObject("Field Emitter","Field Emitter","MyObjectBuilder_Component/ShieldComponent",new Dictionary<string
,string>{{"MyObjectBuilder_BlueprintDefinition/ShieldComponentBP","1"}})};return items;}}public static class
ConfigsSections{public const string GlobalConfig="CBM:GC",CustomItems="CBM:CI",DisplayConfig="CBM:DC",InventoryManager="CBM:IM",
SpecialContainer="CBM:SC",ItemsAssembling="CBM:IA",ItemsDisassembling="CBM:ID",RefineryManager="CBM:RM",ItemsCollecting="CBM:IC",
StopDrones="CBM:SD",DisplayStatus="CBM:DS",DisplayItems="CBM:DI",DisplayLimits="CBM:DL",DisplayVolumes="CBM:DV",
DisplayVolumesRemained="CBM:DVR";public static readonly List<string>Displays=new List<string>{DisplayConfig,DisplayStatus,DisplayItems,
DisplayLimits,DisplayVolumes,DisplayVolumesRemained};}private const string ScanPrefix="CBM-SCAN",PrintPrefix="CBM-PRINT";private
readonly TasksManager _tasks;BlocksManager _blocks;ItemsManager _items;Dictionary<string,DisplayObject>_displays;Dictionary<long
,VolumeObject>_volumes;Dictionary<string,List<string>>_messages;ConfigObject _globalConfig;double _time;public
 Program
(){_tasks=new TasksManager();_tasks.Add(TasksManager.InitializationTaskName,TaskInitialization,20,false);_tasks.Add(
TasksManager.CalculationTaskName,TaskCalculation,3);_tasks.Add("Inventory Manager",TaskInventoryManager,10);_tasks.Add(
"Assemblers Manager",TaskAssemblersManager,5,true,true);_tasks.Add("Assemblers Cleanup",TaskAssemblersCleanup,20);_tasks.Add(
"Refineries Manager",TaskRefineriesManager,9);_tasks.Add("Stop Drones",TaskStopDrones,6);_tasks.Add("Items Collecting",TaskItemsCollecting,7
);_tasks.Add("Displays Manager",TaskDisplaysManager,1,true,true);_tasks.Add("Display Status",TaskDisplayStatus,1,false);
TaskInitialization();Runtime.UpdateFrequency=UpdateFrequency.Update100;}public void
 Main
(string argument,UpdateType updateSource){if(string.IsNullOrEmpty(argument)){_tasks.Run();_time+=Runtime.TimeSinceLastRun
.TotalSeconds;return;}argument=argument.ToLower().Trim();if(argument=="restart"){TaskInitialization();_tasks.Restart();}
else if(argument.StartsWith("clear_queue")){ActionClearQueue(argument);}else if(argument.StartsWith("clear_inventories")){
ActionClearInventories(argument);}else if(argument.StartsWith("scan")){ActionScanItems();}else if(argument.StartsWith("print")){
ActionPrintItems();}}void TaskInitialization(){if(_messages==null){_messages=new Dictionary<string,List<string>>();}else{_messages.Clear
();}ConfigureMeDisplay();CheckGlobalConfig();var ignoreTypes=new List<BlocksManager.BlockType>{BlocksManager.BlockType.
AirVent,BlocksManager.BlockType.Battery,BlocksManager.BlockType.GasPowerProducer,BlocksManager.BlockType.GravityGenerator,
BlocksManager.BlockType.Piston,BlocksManager.BlockType.Projector,BlocksManager.BlockType.Rotor,BlocksManager.BlockType.Sensor};
_blocks=new BlocksManager(GridTerminalSystem,_globalConfig.Get("Tag"),_globalConfig.Get("Ignore"),null,ignoreTypes);if(_items==
null){_items=new ItemsManager(ItemsManager.GetCustomItemsFromString(Me.CustomData));}else{_items.ClearInventories();}if(
_displays==null){_displays=new Dictionary<string,DisplayObject>();}if(_volumes==null){_volumes=new Dictionary<long,VolumeObject>(
);}foreach(var terminalBlock in _blocks.GetBlocks(BlocksManager.BlockType.GasTank)){if(!terminalBlock.CustomData.Contains
(ConfigsSections.InventoryManager)){continue;}var config=ConfigObject.Parse(ConfigsSections.InventoryManager,
terminalBlock.CustomData);foreach(var key in config.Data.Keys.ToList()){var item=_items.GetItem(key);if(item==null){continue;}item.
Inventories.Add(terminalBlock.GetInventory(0));_items.UpdateItem(item);}}foreach(var terminalBlock in _blocks.GetBlocks(
BlocksManager.BlockType.Container)){if(!terminalBlock.CustomData.Contains(ConfigsSections.InventoryManager))continue;var config=
ConfigObject.Parse(ConfigsSections.InventoryManager,terminalBlock.CustomData);foreach(var key in config.Data.Keys.ToList()){var item
=_items.GetItem(key);if(item==null){continue;}item.Inventories.Add(terminalBlock.GetInventory(0));_items.UpdateItem(item)
;}}var displays=new Dictionary<string,DisplayObject>();foreach(var terminalBlock in _blocks.GetBlocks(BlocksManager.
BlockType.TextSurfaceProvider)){if(!ConfigsSections.Displays.Any(key=>terminalBlock.CustomData.Contains(key))){continue;}var
provider=terminalBlock as IMyTextSurfaceProvider;if(provider==null){continue;}var count=provider.SurfaceCount;if(count<=0){
continue;}for(var index=0;index<count;index++){var config=GetDisplayConfig(terminalBlock,index);var selector=terminalBlock.GetId
()+":"+index;var delay=5;if(GetDisplayConfigfSection(terminalBlock,ConfigsSections.DisplayConfig,index)!=null){delay=1;}
else if(GetDisplayConfigfSection(terminalBlock,ConfigsSections.DisplayVolumesRemained,index)!=null){delay=3;}var
listingDelay=int.Parse(config.Get("listingDelay"));DisplayObject display;if(_displays.ContainsKey(selector)){display=_displays[
selector];display.UpdateDelay=delay;display.ListingDelay=listingDelay;}else{display=new DisplayObject(selector,terminalBlock.
GetId(),index,delay,listingDelay);}displays[selector]=display;}}_displays=displays;var volumes=new Dictionary<long,
VolumeObject>();foreach(var terminalBlock in _blocks.GetBlocks()){VolumeObject.VolumeTypes volumeType;var blockType=BlocksManager.
GetBlockTypes(terminalBlock);if(blockType.Contains(BlocksManager.BlockType.Battery)){volumeType=VolumeObject.VolumeTypes.Battery;}
else if(blockType.Contains(BlocksManager.BlockType.GasTank)){volumeType=VolumeObject.VolumeTypes.Tank;}else if(blockType.
Contains(BlocksManager.BlockType.Container)){volumeType=VolumeObject.VolumeTypes.Container;}else if(blockType.Contains(
BlocksManager.BlockType.Collector)){volumeType=VolumeObject.VolumeTypes.Container;}else if(blockType.Contains(BlocksManager.BlockType
.Connector)){volumeType=VolumeObject.VolumeTypes.Container;}else if(blockType.Contains(BlocksManager.BlockType.Drill)){
volumeType=VolumeObject.VolumeTypes.Container;}else{continue;}var selector=terminalBlock.GetId();VolumeObject volume;if(_volumes.
ContainsKey(selector)){volume=_volumes[selector];volume.BlockName=terminalBlock.CustomName;}else{volume=new VolumeObject(selector,
volumeType,terminalBlock.CustomName);}volumes[selector]=volume;}_volumes=volumes;}void TaskCalculation(){_items.ClearAmounts();
foreach(var terminalBlock in _blocks.GetBlocks()){if(!terminalBlock.IsFunctional){continue;}if(terminalBlock.InventoryCount>0){
for(var i=0;i<terminalBlock.InventoryCount;i++){var inventory=terminalBlock.GetInventory(i);if(inventory==null){continue;}
var items=new List<MyInventoryItem>();inventory.GetItems(items);if(items.Count<=0){continue;}foreach(var item in items){var
find=_items.GetItem(item.Type.ToString());if(find==null){continue;}find.Amounts.Exist+=item.Amount;_items.UpdateItem(find);}
}}var assembler=terminalBlock as IMyAssembler;if(assembler==null)continue;{if(!assembler.IsWorking){continue;}if(!
assembler.IsQueueEmpty){var queue=new List<MyProductionItem>();assembler.GetQueue(queue);foreach(var item in queue){var find=
_items.GetItem(item.BlueprintId.ToString());if(find==null){continue;}var amount=item.Amount*find.Blueprints[item.BlueprintId];
if(assembler.Mode==MyAssemblerMode.Assembly){find.Amounts.Assembling+=amount;}else if(assembler.Mode==MyAssemblerMode.
Disassembly){find.Amounts.Disassembling+=amount;}}}if(assembler.CooperativeMode)continue;{if(assembler.Mode==MyAssemblerMode.
Assembly&&assembler.CustomData.Contains(ConfigsSections.ItemsAssembling)){var config=ConfigObject.Parse(ConfigsSections.
ItemsAssembling,assembler.CustomData);if(config==null){continue;}foreach(var entry in config.Data){var find=_items.GetItem(entry.Key);
if(find==null){continue;}if(find.Amounts.AssemblingQuota<0){find.Amounts.AssemblingQuota=0;}find.Amounts.AssemblingQuota+=
MyFixedPoint.DeserializeString(entry.Value);}}else if(assembler.Mode==MyAssemblerMode.Disassembly&&assembler.CustomData.Contains(
ConfigsSections.ItemsDisassembling)){var config=ConfigObject.Parse(ConfigsSections.ItemsDisassembling,assembler.CustomData);if(config==
null){continue;}foreach(var entry in config.Data){var find=_items.GetItem(entry.Key);if(find==null){continue;}if(find.
Amounts.DisassemblingQuota<0){find.Amounts.DisassemblingQuota=0;}find.Amounts.DisassemblingQuota+=MyFixedPoint.
DeserializeString(entry.Value);}}}}}foreach(var item in _items.GetList()){item.Amounts.IsNew=false;_items.UpdateItem(item);}foreach(var
volumeObject in _volumes.Values){var terminalBlock=_blocks.GetBlock(volumeObject.Selector);if(volumeObject.VolumeType==VolumeObject.
VolumeTypes.Battery){var block=terminalBlock as IMyBatteryBlock;volumeObject.SetValue(block.CurrentStoredPower,block.MaxStoredPower
,_time);}else if(volumeObject.VolumeType==VolumeObject.VolumeTypes.Tank){var block=terminalBlock as IMyGasTank;
volumeObject.SetValue(block.FilledRatio,(float)1,_time);}else if(volumeObject.VolumeType==VolumeObject.VolumeTypes.Container){var
inventory=terminalBlock.GetInventory(0);if(inventory==null){continue;}volumeObject.SetValue(inventory.CurrentVolume.ToIntSafe(),
inventory.MaxVolume.ToIntSafe(),_time);}}}void TaskInventoryManager(){foreach(var terminalBlock in _blocks.GetBlocks(
BlocksManager.BlockType.Container)){if(!terminalBlock.IsFunctional||!terminalBlock.CustomData.Contains(ConfigsSections.
SpecialContainer)){continue;}var container=terminalBlock as IMyCargoContainer;if(container==null){continue;}var config=ConfigObject.
Parse(ConfigsSections.SpecialContainer,container.CustomData);if(config==null){continue;}var inventory=container.GetInventory(
0);_items.TransferFromInventory(inventory);foreach(var entry in config.Data){var item=_items.GetItem(entry.Key);if(item==
null){continue;}var needle=MyFixedPoint.DeserializeString(entry.Value);if(needle==0){continue;}var current=inventory.
GetItemAmount(item.Type);if(current>=needle){continue;}var quantity=needle-current;InventoryHelper.TransferFromBlocks(item.Type,
_blocks.GetBlocks(),inventory,quantity);}}var inventories=new List<IMyInventory>();var ignoreBlockTypes=new List<BlocksManager.
BlockType>{BlocksManager.BlockType.Turret,BlocksManager.BlockType.Reactor,BlocksManager.BlockType.SafeZone};foreach(var
terminalBlock in _blocks.GetBlocks()){if(!terminalBlock.IsFunctional||terminalBlock.CustomData.Contains(ConfigsSections.
SpecialContainer)){continue;}var blockType=BlocksManager.GetBlockTypes(terminalBlock);if(blockType.Intersect(ignoreBlockTypes).Any()){
continue;}if(blockType.Contains(BlocksManager.BlockType.GasGenerator)){var inventory=terminalBlock.GetInventory(0);var items=new
List<MyInventoryItem>();inventory.GetItems(items);foreach(var item in items){var find=_items.GetItem(item.Type.ToString());
if(find!=null&&find.Selector!="Ore/Ice"){find.Transfer(item,inventory);}}}else if(terminalBlock is IMyAssembler){
inventories.Add(terminalBlock.GetInventory(1));}else if(terminalBlock is IMyRefinery){inventories.Add(terminalBlock.GetInventory(1)
);}else{for(var i=0;i<terminalBlock.InventoryCount;i++){inventories.Add(terminalBlock.GetInventory(i));}}}_items.
TransferFromInventories(inventories);}void TaskAssemblersManager(){foreach(var terminalBlock in _blocks.GetBlocks(BlocksManager.BlockType.
Assembler)){if(!terminalBlock.IsWorking){continue;}var assembler=(IMyAssembler)terminalBlock;if(!assembler.IsWorking){continue;}
if(!assembler.CooperativeMode){if(assembler.Mode==MyAssemblerMode.Assembly){var config=ConfigObject.Parse(ConfigsSections.
ItemsAssembling,assembler.CustomData);if(config!=null){foreach(var entry in config.Data){var item=_items.GetItem(entry.Key);if(item==
null||item.Blueprints.Count<=0){continue;}foreach(var blueprint in item.Blueprints){if(!assembler.CanUseBlueprint(blueprint.
Key)){continue;}var need=MyFixedPoint.DeserializeString(entry.Value);var total=item.Amounts.Exist+item.Amounts.Assembling;
if(total<need){var add=need-total;var calc=(int)Math.Ceiling((float)add/(float)blueprint.Value);var queue=MyFixedPoint.
DeserializeString(calc.ToString());assembler.Repeating=false;assembler.AddQueueItem(blueprint.Key,queue);item.Amounts.Assembling+=queue*
blueprint.Value;_items.UpdateItem(item);}break;}}}}else if(assembler.Mode==MyAssemblerMode.Disassembly){var config=ConfigObject.
Parse(ConfigsSections.ItemsDisassembling,assembler.CustomData);if(config!=null){foreach(var entry in config.Data){var item=
_items.GetItem(entry.Key);if(item==null||item.Blueprints.Count<=0){continue;}foreach(var blueprint in item.Blueprints){if(!
assembler.CanUseBlueprint(blueprint.Key)){continue;}var value=entry.Value??"0";var need=MyFixedPoint.DeserializeString(value);var
total=item.Amounts.Exist-item.Amounts.Disassembling;if(total>need){var add=total-need;var calc=(int)Math.Round((float)add/(
float)blueprint.Value,0);var queue=MyFixedPoint.DeserializeString(calc.ToString());assembler.Repeating=false;assembler.
AddQueueItem(blueprint.Key,queue);item.Amounts.Disassembling+=queue*blueprint.Value;_items.UpdateItem(item);}break;}}}}}if(assembler
.Mode==MyAssemblerMode.Disassembly){var queue=new List<MyProductionItem>();assembler.GetQueue(queue);const int max=5;var
current=0;foreach(var queueItem in queue){var item=_items.GetItem(queueItem.BlueprintId.ToString());if(item==null||item.
Blueprints.Count<=0){continue;}foreach(var blueprint in item.Blueprints){if(!assembler.CanUseBlueprint(blueprint.Key)){continue;}
MyFixedPoint quantity=queueItem.Amount*blueprint.Value;var transfer=InventoryHelper.TransferFromBlocks(item.Type,_blocks.GetBlocks()
,assembler.GetInventory(1),quantity);if(transfer>0){current++;}break;}if(current>=max){break;}}}}}void
TaskAssemblersCleanup(){var inventories=new List<IMyInventory>();foreach(var terminalBlock in _blocks.GetBlocks(BlocksManager.BlockType.
Assembler)){if(!terminalBlock.IsWorking)continue;inventories.Add(terminalBlock.GetInventory(0));}_items.TransferFromInventories(
inventories);}void TaskRefineriesManager(){var destinations=new List<IMyRefinery>();foreach(var terminalBlock in _blocks.GetBlocks(
BlocksManager.BlockType.Refinery)){if(!terminalBlock.IsWorking||!terminalBlock.CustomData.Contains(ConfigsSections.RefineryManager)){
continue;}var refinery=terminalBlock as IMyRefinery;if(refinery==null){continue;}destinations.Add(refinery);}if(destinations.
Count==0){return;}var inventories=InventoryHelper.GetBlocksInventories(_blocks.GetBlocks());foreach(var refinery in
destinations){var config=ConfigObject.Parse(ConfigsSections.RefineryManager,refinery.CustomData);if(config==null||config.Data.Keys.
Count==0){continue;}var destinationInventory=refinery.GetInventory(0);_items.TransferFromInventory(destinationInventory);
foreach(var key in config.Data.Keys){var item=_items.GetItem(key);if(item==null){continue;}InventoryHelper.
TransferFromInventories(item.Type,inventories,destinationInventory);if(destinationInventory.IsFull){break;}}}}void TaskStopDrones(){if(!
_messages.ContainsKey("Stop Drones")){_messages["Stop Drones"]=new List<string>();}else{_messages["Stop Drones"].Clear();}foreach
(var terminalBlock in _blocks.GetBlocks(BlocksManager.BlockType.Connector)){if(!terminalBlock.IsWorking||!terminalBlock.
CustomData.Contains(ConfigsSections.StopDrones)){continue;}var connector=(IMyShipConnector)terminalBlock;if(connector.Status!=
MyShipConnectorStatus.Connected){continue;}var config=ConfigObject.Parse(ConfigsSections.StopDrones,connector.CustomData);if(config==null){
continue;}var droneBlocksName=config.Get("DroneBlocksName");var baseContainersName=config.Get("BaseContainersName");string error
=null;if(string.IsNullOrEmpty(droneBlocksName)){error="Empty DroneBlocksName";}if(string.IsNullOrEmpty(baseContainersName
)){error="Empty BaseContainersName";}if(!string.IsNullOrEmpty(error)){_messages["Stop Drones"].Add(connector.CustomName+
":");_messages["Stop Drones"].Add("ERROR - "+error);continue;}var connectedBlocks=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(connectedBlocks,block=>block.CubeGrid==connector.OtherConnector.CubeGrid&&
droneBlocksName!=null&&block.CustomName.Contains(droneBlocksName));if(connectedBlocks.Count==0){_messages["Stop Drones"].Add(connector.
CustomName+":");_messages["Stop Drones"].Add("INFO - Can't find drones blocks `"+droneBlocksName+"`");continue;}var blockActive=
true;var volumeObject=new VolumeObject(_volumes.Values.ToList(),baseContainersName);var maxConfig=config.Get(
"BaseContainersMaxVolume",_globalConfig.Get("SD:BaseContainersMaxVolume","90%"));var percent=maxConfig.Contains("%");var max=(float)MyFixedPoint.
DeserializeString(maxConfig.Replace("%",""));if(percent){if(volumeObject.CurrentPercent>=max){blockActive=false;}}else if(volumeObject.
CurrentVolume>=max){blockActive=false;}foreach(var block in connectedBlocks){block.ApplyAction(blockActive?"OnOff_On":"OnOff_Off");}}
}void TaskItemsCollecting(){var itemsCollectingBlockTypes=new List<BlocksManager.BlockType>{BlocksManager.BlockType.
Cockpit,BlocksManager.BlockType.Collector,BlocksManager.BlockType.Connector,BlocksManager.BlockType.Container,BlocksManager.
BlockType.CryoChamber,BlocksManager.BlockType.Drill,BlocksManager.BlockType.Grinder,BlocksManager.BlockType.Sorter,BlocksManager.
BlockType.Welder};foreach(var terminalBlock in _blocks.GetBlocks(BlocksManager.BlockType.Connector)){if(!terminalBlock.IsWorking
||!terminalBlock.CustomData.Contains(ConfigsSections.ItemsCollecting)){continue;}var connector=(IMyShipConnector)
terminalBlock;if(connector.Status!=MyShipConnectorStatus.Connected){continue;}var connectedConnector=connector.OtherConnector;var
connectedBlocks=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(connectedBlocks,block=>block.CubeGrid
==connectedConnector.CubeGrid);var destinationInventory=connector.GetInventory(0);_items.TransferFromInventory(
destinationInventory);var config=ConfigObject.Parse(ConfigsSections.ItemsCollecting,connector.CustomData);var collectTypes=new List<
MyItemType>();var ignoreTypes=new List<MyItemType>();if(config!=null&&config.Data.Keys.Count>0){foreach(var key in config.Data.
Keys){var ignore=key.StartsWith("!");var itemKey=ignore?key.TrimStart('!'):key;var item=_items.GetItem(itemKey);if(item==
null){continue;}if(ignore){ignoreTypes.Add(item.Type);}else{collectTypes.Add(item.Type);}}}foreach(var block in
connectedBlocks){if(block.CustomName.Contains("!"+ConfigsSections.ItemsCollecting)||block.CustomData.Contains("!"+ConfigsSections.
ItemsCollecting)||!BlocksManager.GetBlockTypes(block).Intersect(itemsCollectingBlockTypes).Any()){continue;}for(var i=0;i<block.
InventoryCount;i++){var sourceInventory=block.GetInventory(i);var sourceItems=new List<MyInventoryItem>();sourceInventory.GetItems(
sourceItems);foreach(var item in sourceItems){if(collectTypes.Count!=0&&!collectTypes.Contains(item.Type)){continue;}if(ignoreTypes
.Count>0&&ignoreTypes.Contains(item.Type)){continue;}InventoryHelper.TransferItem(item,sourceInventory,
destinationInventory);_items.TransferFromInventory(destinationInventory);}}}}}void TaskDisplaysManager(){foreach(var displayObject in
_displays.Values.ToList()){var terminalBlock=_blocks.GetBlock(displayObject.BlockSelector,BlocksManager.BlockType.
TextSurfaceProvider);if(terminalBlock==null||!terminalBlock.IsWorking||!ConfigsSections.Displays.Any(key=>terminalBlock.CustomData.Contains
(key))){continue;}var config=GetDisplayConfig(terminalBlock,displayObject.SurfaceIndex);var language=config.Get(
"language");displayObject.Tick();if(displayObject.NeedUpdate()){displayObject.TickReset();displayObject.ClearLines();var
isDisplayStatus=false;if(displayObject.SurfaceIndex==0&&terminalBlock.CustomData.Contains(ConfigsSections.DisplayStatus+"]")){
isDisplayStatus=true;}else if(terminalBlock.CustomData.Contains(ConfigsSections.DisplayStatus+":"+displayObject.SurfaceIndex)){
isDisplayStatus=true;}if(isDisplayStatus){displayObject.AddTextLine("Runtime: "+SecondsToString(_time));foreach(var line in _tasks.
GetStatusText()){displayObject.AddTextLine(line);}foreach(var messageSection in _messages){if(messageSection.Value.Count==0){continue
;}displayObject.AddBlankLine();displayObject.AddTextLine(messageSection.Key+":");foreach(var message in messageSection.
Value.ToList()){displayObject.AddTextLine(message);}}}else{var configs=ConfigsHelper.GetSections(terminalBlock.CustomData,
false);foreach(var configSection in configs){var configKey=ConfigsHelper.RemoveSectionIndex(configSection.Key);if(
configSection.Value.Count==0||!ConfigsSections.Displays.Any(key=>key!=ConfigsSections.DisplayConfig&&configKey.Contains(key))){
continue;}var configIndexPosition=configKey.LastIndexOf(":",StringComparison.Ordinal);var configIndexString=configKey.Substring(
configIndexPosition+1);if(!configIndexString.All(char.IsDigit)){if(displayObject.SurfaceIndex==0){configIndexString="0";}else{continue;}}
else{configKey=configKey.Substring(0,configIndexPosition);}var configIndex=int.Parse(configIndexString);if(configIndex!=
displayObject.SurfaceIndex){continue;}foreach(var line in configSection.Value.ToList()){var clear=line.Trim();if(string.IsNullOrEmpty
(clear)){displayObject.AddBlankLine();continue;}if(configKey==ConfigsSections.DisplayItems){var item=_items.GetItem(clear
);if(item!=null){var label=item.Title(language);var text=item.Amounts.ToString();displayObject.AddLine(label+": "+text,
DisplayObject.TextSprite(label),DisplayObject.TextSprite(text,TextAlignment.RIGHT));continue;}}if(configKey==ConfigsSections.
DisplayLimits&&line.Contains("=")){var content=ConfigsHelper.ParseLine(line);if(!string.IsNullOrEmpty(content.Value)){var label=
content.Key;var exist=MyFixedPoint.DeserializeString("-1");var needle=MyFixedPoint.DeserializeString(content.Value);var item=
_items.GetItem(content.Key);if(item!=null){label=item.Title(language);exist=item.Amounts.Exist;}var text=ItemAmountsObject.
ValueToString(exist)+"/"+ItemAmountsObject.ValueToString(needle);Color?color=null;if(exist>needle){color=Color.Green;}else if(exist<
needle){color=Color.Red;}displayObject.AddLine(label+": "+text,DisplayObject.TextSprite(label),DisplayObject.TextSprite(text,
TextAlignment.RIGHT,color));continue;}}if((configKey==ConfigsSections.DisplayVolumes||configKey==ConfigsSections.
DisplayVolumesRemained)&&line.Contains("=")){var content=ConfigsHelper.ParseLine(line);if(!string.IsNullOrEmpty(content.Key)){var label=
content.Key;var blockName=content.Value.ToLower();var sumObject=new VolumeObject(_volumes.Values.ToList(),blockName);if(!
sumObject.IsValid){continue;}var text=sumObject.CurrentPercent+"%";Color?color=null;if(configKey==ConfigsSections.
DisplayVolumesRemained&&sumObject.RemainedVector!=VolumeObject.RemainedVectors.None){if(sumObject.RemainedVector==VolumeObject.RemainedVectors
.Plus){color=Color.Green;}else if(sumObject.RemainedVector==VolumeObject.RemainedVectors.Minus){color=Color.Red;}if(
sumObject.Remained>2){var time=SecondsToString(sumObject.Remained);text+=" (";if(sumObject.RemainedVector==VolumeObject.
RemainedVectors.Plus){text+="+";}else if(sumObject.RemainedVector==VolumeObject.RemainedVectors.Minus){text+="-";}text+=time+")";}}
displayObject.AddLine(label+": "+text,DisplayObject.TextSprite(label),DisplayObject.TextSprite(text,TextAlignment.RIGHT,color));
continue;}}displayObject.AddCustomTextLine(line);}}}}_displays[displayObject.Selector]=displayObject;var provider=terminalBlock
as IMyTextSurfaceProvider;var display=provider.GetSurface(displayObject.SurfaceIndex);if(display==null){continue;}if(
displayObject.GetLines().Count==0&&string.IsNullOrEmpty(config.Get("title"))){continue;}var frame=display.DrawFrame();var viewport=
DisplayObject.GetViewport(display);var padding=float.Parse(config.Get("padding"));var font=config.Get("font");var fontSize=float.
Parse(config.Get("fontSize"));var lineHeight=float.Parse(config.Get("lineHeight"));var border=float.Parse(config.Get("border"
));var positionLeft=viewport.X+padding;var positionRight=viewport.X+viewport.Width-padding;var positionCenter=viewport.X+
viewport.Width/2;var positionTop=viewport.Y+padding;var positionBottom=viewport.Y+viewport.Height-padding;if(border>0){foreach(
var sprite in DisplayObject.GetSurfaceBorder(display,border,padding)){frame.Add(sprite);}positionLeft=viewport.X+border+
padding*2;positionRight=viewport.X+viewport.Width-border-padding*2;positionTop=viewport.Y+border+padding*2;positionBottom=
viewport.Y+viewport.Height-border-padding*2;}var title=config.Get("title");if(!string.IsNullOrEmpty(title)){foreach(var sprite
in DisplayObject.GetSurfaceTitle(display,title,font,fontSize,lineHeight,border,padding)){frame.Add(sprite);}positionTop=
viewport.Y+lineHeight+padding*2;}var limit=(int)Math.Round((positionBottom-positionTop)/lineHeight);displayObject.ListingTick();
if(displayObject.NeedListing()){displayObject.Listing(limit);}foreach(var line in displayObject.GetLines(limit)){if(line.
Count==0){positionTop+=lineHeight;continue;}foreach(var sprite in line){var textSprite=sprite;textSprite.Size=new Vector2(
positionLeft,positionTop);textSprite.FontId=font;textSprite.RotationOrScale=fontSize;if(textSprite.Color==null){textSprite.Color=
display.ScriptForegroundColor;}textSprite.Position=new Vector2(positionLeft,positionTop);if(textSprite.Alignment==TextAlignment
.RIGHT){textSprite.Position=new Vector2(positionRight,positionTop);}else if(textSprite.Alignment==TextAlignment.CENTER){
textSprite.Position=new Vector2(positionCenter,positionTop);}frame.Add(textSprite);}positionTop+=lineHeight;}display.ContentType=
ContentType.SCRIPT;display.Script="";display.WriteText(displayObject.LinesToString());frame.Dispose();}}void TaskDisplayStatus(){
var displayData=new List<string>{{"= Crowigor's Base Manager ="},"Runtime: "+SecondsToString(_time)};displayData.AddRange(
_tasks.GetStatusText());if(_messages.Count>0){displayData.Add("");foreach(var entry in _messages){if(entry.Value.Count==0){
continue;}displayData.Add(entry.Key+": ");displayData.AddRange(entry.Value.Select(message=>"   "+message));}}Echo(string.Join(
"\n",displayData.ToArray()));}void ActionClearQueue(string argument=""){var blockName=argument.ToLower().Replace(
"clear_queue","").Trim();foreach(var terminalBlock in _blocks.GetBlocks(BlocksManager.BlockType.Assembler)){if(!string.IsNullOrEmpty(
argument)){var customName=terminalBlock.CustomName.ToLower();if(!customName.ToLower().Contains(blockName)){continue;}}var
assembler=terminalBlock as IMyAssembler;assembler?.ClearQueue();}}void ActionClearInventories(string argument=""){var blockName=
argument.ToLower().Replace("clear_inventories","").Trim();foreach(var terminalBlock in _blocks.GetBlocks()){if(!string.
IsNullOrEmpty(argument)){var customName=terminalBlock.CustomName.ToLower();if(!customName.Contains(blockName)){continue;}}for(var i=0
;i<terminalBlock.InventoryCount;i++){var inventory=terminalBlock.GetInventory(i);_items.TransferFromInventory(inventory);
}}}void ActionScanItems(){const string title="["+ConfigsSections.CustomItems+"]";var result=ItemsManager.ScanCustomItems(
GridTerminalSystem,ScanPrefix,title);if(_messages==null){_messages=new Dictionary<string,List<string>>();}_messages["Scan Result"]=result;
}void ActionPrintItems(){const string title="["+ConfigsSections.CustomItems+"]";var result=ItemsManager.PrintItems(_items
??new ItemsManager(),GridTerminalSystem,PrintPrefix,title);if(_messages==null){_messages=new Dictionary<string,List<string
>>();}_messages["Print Result"]=result;}void ConfigureMeDisplay(){var display=Me.GetSurface(0);display.ContentType=
ContentType.TEXT_AND_IMAGE;display.FontColor=new Color(255,180,0);display.FontSize=(float)0.9;display.TextPadding=(float)12.5;
display.Alignment=TextAlignment.CENTER;display.WriteText("Crowigor's Base Manager");display.ClearImagesFromSelection();display.
AddImageToSelection("LCD_Economy_Graph_2");}ConfigObject GetDisplayConfig(IMyTerminalBlock block,int index=0){var global=new ConfigObject(
ConfigsSections.DisplayConfig,new Dictionary<string,string>{{"title",""},{"font",_globalConfig.Get("DC:font","Debug")},{"fontSize",
_globalConfig.Get("DC:fontSize","0.8")},{"lineHeight",_globalConfig.Get("DC:lineHeight","32")},{"padding",_globalConfig.Get(
"DC:padding","10")},{"border",_globalConfig.Get("DC:border","1")},{"listingDelay",_globalConfig.Get("DC:listingDelay","10")},{
"language",_globalConfig.Get("DC:language","source")},});var section=GetDisplayConfigfSection(block,ConfigsSections.DisplayConfig,
index);if(section==null){return global;}var config=ConfigObject.Parse(section,block.CustomData);if(config==null){return
global;}return ConfigsHelper.Merge(ConfigsSections.DisplayConfig,new List<ConfigObject>{global,config});}string
GetDisplayConfigfSection(IMyTerminalBlock block,string section,int index=0){if(index==0&&block.CustomData.Contains(section+"]")){return section;
}if(block.CustomData.Contains(section+":"+index)){return section+":"+index;}return null;}void CheckGlobalConfig(){var
configDefault=new ConfigObject(ConfigsSections.GlobalConfig,new Dictionary<string,string>{{"Tag","[Base]"},{"Ignore","[!CBM]"},{
"SD:BaseContainersMaxVolume","90%"},{"DC:font","Debug"},{"DC:fontSize","0.8"},{"DC:lineHeight","32"},{"DC:padding","10"},{"DC:border","1"},{
"DC:listingDelay","5"},{"DC:language","source"}});var configCurrent=ConfigObject.Parse(ConfigsSections.GlobalConfig,Me.CustomData);var
configNew=ConfigsHelper.Merge(ConfigsSections.GlobalConfig,new List<ConfigObject>{configDefault,configCurrent});_globalConfig=
configNew;var customData=new List<string>{ConfigsHelper.ToCustomData(configNew)};var customItems=ConfigObject.Parse(
ConfigsSections.CustomItems,Me.CustomData);if(customItems!=null){customData.Add(ConfigsHelper.ToCustomData(customItems));}Me.CustomData
=string.Join("\n\n",customData.ToArray());}private static string SecondsToString(double seconds){var time=TimeSpan.
FromSeconds(seconds);return$"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";}void AddDebug(string message){if(
_messages==null){_messages=new Dictionary<string,List<string>>();}if(!_messages.ContainsKey("Debug")){_messages["Debug"]=new List
<string>();}_messages["Debug"].Add(message);}public class TasksManager{private readonly Dictionary<string,TaskObject>
_storage=new Dictionary<string,TaskObject>();public const string InitializationTaskName="Initialization",CalculationTaskName=
"Calculation";public void Add(string name,Action method,int delay=1,bool needInitialization=true,bool needCalculation=false){if(delay
<1){delay=1;}var task=new TaskObject(name,method,delay,needInitialization,needCalculation);_storage[task.Name]=task;}
public void Clear(){_storage.Clear();}public void Restart(){foreach(string key in new List<string>(_storage.Keys)){var task=
_storage[key];task.Status=TaskObject.Statuses.Wait;task.LastStatus=TaskObject.Statuses.Null;task.CurrentTick=task.Delay;
UpdateTask(task);}Run();}public void Run(){foreach(string key in new List<string>(_storage.Keys)){var task=_storage[key];if(task.
Status==TaskObject.Statuses.Progress){continue;}if(task.Status==TaskObject.Statuses.Success||task.Status==TaskObject.Statuses.
Error){task.CurrentTick=2;task.LastStatus=task.Status;task.Status=TaskObject.Statuses.Wait;UpdateTask(task);}if(task.Status!=
TaskObject.Statuses.Wait&&task.Status!=TaskObject.Statuses.Skip){continue;}if(task.CurrentTick<task.Delay){task.CurrentTick++;
UpdateTask(task);continue;}task.CurrentTick=1;task.Error=null;task.LastStatus=TaskObject.Statuses.Null;if((task.NeedInitialization
&&!CheckInitialization())||(task.NeedCalculation&&!CheckCalculation())){task.CurrentTick=task.Delay;task.Status=TaskObject
.Statuses.Skip;UpdateTask(task);continue;}task.Status=TaskObject.Statuses.Progress;UpdateTask(task);if(task.Method==null)
{task.Error="Method not found";task.Status=TaskObject.Statuses.Error;UpdateTask(task);continue;}try{task.Method();task.
Status=(task.Delay==1)?TaskObject.Statuses.Wait:TaskObject.Statuses.Success;UpdateTask(task);TaskObject value;if(task.Name==
InitializationTaskName&&_storage.TryGetValue(CalculationTaskName,out value)){value.LastStatus=TaskObject.Statuses.Skip;}}catch(Exception e){
task.Error=e.Message;task.Status=TaskObject.Statuses.Error;UpdateTask(task);}}}public List<string>GetStatusText(){var result
=new List<string>();foreach(var entry in _storage){var task=entry.Value;var text=task.Name+": ";if(task.Delay>1){text+=(
task.Status==TaskObject.Statuses.Wait)?task.LastStatus:task.Status;if(task.Status!=TaskObject.Statuses.Skip){text+=" ("+task
.CurrentTick+"/"+task.Delay+")";}}else{text+=(task.Status!=TaskObject.Statuses.Error)?TaskObject.Statuses.Success:task.
Status;}result.Add(text);if(task.Error!=null){result.Add(task.Error+"\n");}}return result;}bool CheckInitialization(){
TaskObject task;if(!_storage.TryGetValue(InitializationTaskName,out task)){return false;}bool result;if(task.Status==TaskObject.
Statuses.Success){result=true;}else if(task.Status==TaskObject.Statuses.Wait){result=(task.LastStatus==TaskObject.Statuses.
Success);}else{result=false;}return result;}bool CheckCalculation(){TaskObject task;if(!_storage.TryGetValue(
CalculationTaskName,out task)){return false;}bool result;if(task.Status==TaskObject.Statuses.Success){result=true;}else if(task.Status==
TaskObject.Statuses.Wait){result=(task.LastStatus==TaskObject.Statuses.Success);}else{result=false;}return result;}void UpdateTask
(TaskObject task){_storage[task.Name]=task;}}public class TaskObject{public readonly string Name;public Statuses Status,
LastStatus;public string Error;public readonly int Delay;public int CurrentTick;public readonly bool NeedInitialization,
NeedCalculation;public readonly Action Method;public enum Statuses{Wait,Progress,Error,Success,Skip,Null}public TaskObject(string name,
Action method,int delay=0,bool needInitialization=true,bool needCalculation=false){Name=name;Status=Statuses.Wait;LastStatus=
Statuses.Null;Error=null;Delay=delay;CurrentTick=delay;NeedInitialization=needInitialization;NeedCalculation=needCalculation;
Method=method;}}public class VolumeObject{public enum VolumeTypes{None,Battery,Container,Tank,Group}public enum
RemainedVectors{None,Plus,Minus,}public long Selector;public readonly VolumeTypes VolumeType;public string BlockName;public double
CurrentVolume,LastVolume,MaxVolume,CurrentTime,LastTime,Remained;public int CurrentPercent;public RemainedVectors RemainedVector;
public bool IsValid{get;}public VolumeObject(long selector,VolumeTypes volumeType,string blockName){Selector=selector;
VolumeType=volumeType;BlockName=blockName;RemainedVector=RemainedVectors.None;IsValid=true;}public VolumeObject(List<VolumeObject>
objects,string blockName="",VolumeTypes volumeType=VolumeTypes.Group){blockName=string.IsNullOrEmpty(blockName)?"":blockName.
ToLower();int count=0;double sumCurrentTime=0;double sumLastTime=0;double currentVolume=0;double lastVolume=0;double maxVolume=
0;foreach(var volumeObject in objects){if(volumeType!=VolumeTypes.Group&&volumeObject.VolumeType!=volumeType){continue;}
if(!string.IsNullOrEmpty(blockName)&&!volumeObject.BlockName.ToLower().Contains(blockName)){continue;}count++;
sumCurrentTime+=volumeObject.CurrentTime;sumLastTime+=volumeObject.LastTime;currentVolume+=volumeObject.CurrentVolume;lastVolume+=
volumeObject.LastVolume;maxVolume+=volumeObject.MaxVolume;}if(count==0){IsValid=false;return;}IsValid=true;CurrentVolume=lastVolume;
CurrentTime=count>0?sumLastTime/count:0;var currentTime=count>0?sumCurrentTime/count:0;SetValue(currentVolume,maxVolume,currentTime
);}public void SetValue(double current,double max,double currentTime){LastVolume=CurrentVolume;LastTime=CurrentTime;
CurrentVolume=current;MaxVolume=max;CurrentTime=currentTime;if(MaxVolume>0){var raw=CurrentVolume/MaxVolume*100.0;var rounded=(int)
Math.Round(raw,MidpointRounding.AwayFromZero);if(rounded<0){rounded=0;}else if(rounded>100){rounded=100;}CurrentPercent=
rounded;}else{CurrentPercent=0;}if(CurrentVolume>LastVolume){RemainedVector=RemainedVectors.Plus;}else if(CurrentVolume<
LastVolume){RemainedVector=RemainedVectors.Minus;}else{RemainedVector=RemainedVectors.None;}if(RemainedVector!=RemainedVectors.
None){var time=CurrentTime-LastTime;if(time<=0){Remained=0;return;}var volume=CurrentVolume-LastVolume;var rate=volume/time;
const double eps=1e-9;if(Math.Abs(rate)<eps){Remained=0;return;}if(RemainedVector==RemainedVectors.Plus){if(MaxVolume>0&&
CurrentVolume<MaxVolume){var left=MaxVolume-CurrentVolume;Remained=left>0?Math.Max(0,left/rate):0;}else{Remained=0;}}else{if(
CurrentVolume>0){Remained=Math.Max(0,CurrentVolume/Math.Abs(rate));}else{Remained=0;}}}}}
