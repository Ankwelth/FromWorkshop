
/*  -= Telemetry =-

= How to use?
 1) Place a PB in your ship and load this code;
 2) Place a PB in your base and load this code;
 3) Go to the Custom Data in the PB of your base and set flag collector under Telemetry to true;
 4) Add the TAG [Telemetry] to a text panel in your base;
 5) Every ship telemetry with this PB will show up in the panel provided they are in antenna range;
 6) Add the TAG [VTelemetry] to a text panel or console block (projector) in your base;
*/

public static readonly string version = "Telemetry v1.2.0";

// What follows are the Custom Data settings and defaults that can be configured;
private void RegisterSettings(ref Section s) {
    s.RegisterBool("collector", false); // Set to true to enable this PB as a Telemetry collection node;
    s.RegisterString("visual_tag", "VTelemetry"); // Text pannels or projectors with this [tag] will display the visual telemetry;
    s.RegisterString("panel_tag", "Telemetry"); // Text pannels with this [tag] will display the telemetry;
    s.RegisterBool("subgrid_panels", false); // Set to true to enable this PB to write to pannels in other subgrids;
    s.RegisterInt("channel", 0); // Send telemetry via specific channels, use tag [TELEMETRY_0] in a text panel to get a specific channel;
    s.RegisterString("security_key", "TELEMETRY"); // Change to avoid other factions from seeing your telemetry;
}

private void RegisterCommands(ref CmdHandler c) {
    c.NewCmd("SCALE", "Saves data to PB Storage", ToggleScale);
    c.NewCmd("SAVE", "Saves data to PB Storage", SaveCmd);
    c.NewCmd("CLEAR", "Clears existing data", ResetCmd);
    c.NewCmd("ORBIT", "Dumps orbit GPS in CustomData", CalculateOrbit);
}
static string visual_tag="VTelemetry";static string panel_tag="Telemetry";static string security_key="TELEMETRY";static
int channel=0;static bool subgrid_panels=false;void ToggleScale(ref string arg){Interface.ToggleScale();Interface.Print(
this,ref recordManager);}void ResetCmd(ref string arg){recordManager.records.Clear();}void SaveCmd(ref string arg){Save();}
void CalculateOrbit(ref string arg){double altitude;if(!double.TryParse(arg,out altitude)){Logger.Err(
"Unable to parse altitude in meters. Example: \"ORBIT 11000\"");return;}Vector3D orbit;try{orbit=GetOrbit(this,altitude);}catch(Exception e){Logger.Err("Unable to calculate orbit: "+
e.Message);return;}Logger.Info("Orbit set in Custom Data");settingsSection.RegisterString("orbit",GenerateGPS("orbit",
orbit));return;}void UpdateAdvertise(ref string _){if(!collector){new Record().GetData(this).Advertise(this);}}void
UpdateCollector(ref string _){string summary=string.Format("{0} {1} {2}\n",Animation.Curve(),version,Animation.Curve());Animation.Curve
();if(collector){try{recordManager.CollectRecords(this);summary+=recordManager.Stats();recordManager.WriteToPannels(this)
;Interface.Print(this,ref recordManager);}catch(Exception e){Logger.Err("Collector: "+e.Message);}}Echo(summary+"\n"+
Logger.PrintBuffer()+"\n"+arbiter.Stats());}static bool collector=false;void UpdateSettings(ref string _){Me.CustomData=
settings.UpdateSettings(Me.CustomData);if(subgrid_panels!=settingsSection.GetBool("subgrid_panels")){subgrid_panels=!
subgrid_panels;Logger.Info("Writing to subgrid panels "+(subgrid_panels?"enabled":"disabled"));}if(security_key!=settingsSection.
GetString("security_key")){IGC.DisableBroadcastListener(IGC.RegisterBroadcastListener(security_key));security_key=settingsSection
.GetString("security_key");IGC.RegisterBroadcastListener(security_key);Logger.Info("Security key changed to: "+
security_key);}if(collector!=settingsSection.GetBool("collector")){collector=!collector;Logger.Info("Collector mode "+(collector?
"enabled":"disabled"));var listener=IGC.RegisterBroadcastListener(security_key);if(!collector){IGC.DisableBroadcastListener(
listener);}}if(channel!=settingsSection.GetInt("channel")){channel=settingsSection.GetInt("channel");Logger.Info(
"Channel changed to: "+channel.ToString());}if(panel_tag!=settingsSection.GetString("panel_tag")){panel_tag=settingsSection.GetString(
"panel_tag");Logger.Info("Pannel tag changed to: "+panel_tag);}if(visual_tag!=settingsSection.GetString("visual_tag")){visual_tag=
settingsSection.GetString("visual_tag");Logger.Info("Visual pannel tag changed to: "+visual_tag);}}void Main(string argument,UpdateType
updateSource){try{arbiter.HandleUpdate(updateSource,ref argument);}catch(Exception exception){Echo("Main exception: "+exception.
Message);}}Settings settings;Section settingsSection;Arbiter arbiter;RecordManager recordManager;CmdHandler cmdHandler;string
empty_string="";Program(){settings=new Settings();settingsSection=settings.NewSection("Telemetry");RegisterSettings(ref
settingsSection);UpdateSettings(ref empty_string);recordManager=new RecordManager();cmdHandler=new CmdHandler();RegisterCommands(ref
cmdHandler);Load();arbiter=new Arbiter(this);arbiter.RegisterMiliSecond(750,UpdateCollector);arbiter.RegisterSecond(3,
UpdateSettings);arbiter.RegisterSecond(5,UpdateAdvertise);arbiter.RegisterDefault(cmdHandler.HandleCmd);Logger.Info("Started...");}
void Save(){try{this.Storage=recordManager.ToSerial();}catch(Exception e){Logger.Err("Saving error: "+e.Message);}}void Load
(){if(this.Storage==""){return;}try{recordManager.FromSerial(this.Storage);}catch(Exception e){this.Storage="";Logger.Err
("Loading error: "+e.Message);}}static class Animation{private static string[]ROTATOR=new string[]{"|","/","-","\\"};
private static int rotatorCount=0;private static string[]SCORE=new string[]{"_","-","¯","-"};private static int scoreCount=0;
private static string[]CURVE=new string[]{">","<"};private static int curveCount=0;public static string Rotator(){if(++
rotatorCount>ROTATOR.Length-1){rotatorCount=0;}return ROTATOR[rotatorCount];}public static string Score(){if(++scoreCount>SCORE.
Length-1){scoreCount=0;}return SCORE[scoreCount];}public static string Curve(){if(++curveCount>CURVE.Length-1){curveCount=0;}
return CURVE[curveCount];}}class Arbiter{private float MAX_INSTRUCTION_QUOTA=0.85f;private int instructionOverruns=0;public
delegate void Updater(ref string msg);public LoadStats loadStats=new LoadStats();private Program program;private struct
UpdaterInfo{public string Name;public Updater updater;}private Dictionary<UpdateType,UpdaterInfo>updateRegister=new Dictionary<
UpdateType,UpdaterInfo>();private Dictionary<Rater,Updater>updateRater=new Dictionary<Rater,Updater>();public Arbiter(Program p){
program=p;}public void RegisterDefault(Updater updater){var defaultUpdate=UpdateType.Terminal|UpdateType.Mod|UpdateType.Script|
UpdateType.Trigger;updateRegister[defaultUpdate]=new UpdaterInfo(){Name="Default",updater=updater};}public void Register(
UpdateType updateType,Updater updater){updateRegister[updateType]=new UpdaterInfo(){Name=updateType.ToString(),updater=updater};
switch(updateType){case UpdateType.Update1:program.Runtime.UpdateFrequency|=UpdateFrequency.Update1;break;case UpdateType.
Update10:program.Runtime.UpdateFrequency|=UpdateFrequency.Update10;break;case UpdateType.Update100:program.Runtime.
UpdateFrequency|=UpdateFrequency.Update100;break;}}public void RegisterMiliSecond(int updateMiliSeconds,Updater updater){updateRater[
new Rater(TimeSpan.FromMilliseconds(updateMiliSeconds))]=updater;program.Runtime.UpdateFrequency|=UpdateFrequency.Update1;}
public void RegisterSecond(int updateSeconds,Updater updater){updateRater[new Rater(TimeSpan.FromSeconds(updateSeconds))]=
updater;program.Runtime.UpdateFrequency|=UpdateFrequency.Update1;}private float InstructionQuotaUsage(float starting=0){return(
float)(program.Runtime.CurrentInstructionCount-starting)/(float)program.Runtime.MaxInstructionCount;}public bool
InstructionOveruse(){var check=InstructionQuotaUsage()>=MAX_INSTRUCTION_QUOTA;if(check){++instructionOverruns;}return check;}public void
InnerHandleUpdate(UpdateType updateType,ref string msg){foreach(var pair in updateRegister){if((pair.Key&updateType)!=0){Run(pair.Value.
updater,ref msg,pair.Value.Name);}if(InstructionOveruse()){return;}}if(updateRater.Count!=0){foreach(var rater in updateRater){
if(rater.Key.Elapsed()){Run(rater.Value,ref msg,rater.Key.Name);}if(InstructionOveruse()){return;}}}}public void
HandleUpdate(UpdateType updateType,ref string msg){InnerHandleUpdate(updateType,ref msg);loadStats.Measure(InstructionQuotaUsage(),
"Total");}private void Run(Updater updater,ref string msg,string name){var cic=program.Runtime.CurrentInstructionCount;try{
updater(ref msg);}catch(Exception e){Logger.Err(string.Format("Run({0}): {1}",updater.Method.Name,e.Message));}loadStats.
Measure(InstructionQuotaUsage(cic),name);}public string Stats(){return loadStats+"Overruns: "+instructionOverruns;}}delegate
void CmdAction(ref string arg);class Cmd{public string help;public CmdAction cmdAction;public Cmd(string h,CmdAction a){help
=h;cmdAction=a;}}class CmdHandler{public static char[]CMD_SEPARATOR=new char[]{' '};public Dictionary<string,Cmd>cmdDict;
public CmdHandler(){cmdDict=new Dictionary<string,Cmd>();}public void NewCmd(string cmd,string help,CmdAction action){cmdDict[
cmd]=new Cmd(help,action);}public void HandleCmd(ref string arg){arg=arg.Trim();if(arg==""){return;}string[]parts=arg.Trim(
).Split(CMD_SEPARATOR,2);string arg0=parts[0].ToUpper();string arg1=parts.ElementAtOrDefault(1);arg1=String.IsNullOrEmpty
(arg1)?"":arg1;if(!cmdDict.ContainsKey(arg0)){return;}cmdDict[arg0].cmdAction(ref arg1);Logger.Info("Executed: "+arg0);}}
class PbDefenseWrapper{private IMyTerminalBlock _block;public bool noDefenseData;private Func<IMyTerminalBlock,float>
_getShieldPercent;private Func<IMyEntity,IMyTerminalBlock>_getShieldBlock;private Func<IMyTerminalBlock,bool>_isShieldBlock;public void
SetActiveShield(IMyTerminalBlock block)=>_block=block;public PbDefenseWrapper(IMyTerminalBlock block){_block=block;var delegates=_block
.GetProperty("DefenseSystemsPbAPI")?.As<Dictionary<string,Delegate>>().GetValue(_block);if(delegates==null){noDefenseData
=true;return;}_getShieldPercent=(Func<IMyTerminalBlock,float>)delegates["GetShieldPercent"];_getShieldBlock=(Func<
IMyEntity,IMyTerminalBlock>)delegates["GetShieldBlock"];_isShieldBlock=(Func<IMyTerminalBlock,bool>)delegates["IsShieldBlock"];if
(!IsShieldBlock())_block=GetShieldBlock(_block.CubeGrid)??_block;}public float GetShieldPercent()=>_getShieldPercent?.
Invoke(_block)??-1;public IMyTerminalBlock GetShieldBlock(IMyEntity entity)=>_getShieldBlock?.Invoke(entity)??null;public bool
IsShieldBlock()=>_isShieldBlock?.Invoke(_block)??false;}static class Interface{private static Color bgColor=Color.Black;private
static Color fgColor=Color.Cyan.Alpha(0.1f);private static Color fgActiveColor=Color.Red.Alpha(0.1f);private static float
dot_size=0.25f;private static Color dotColor=Color.DarkGreen;private static Color textColor=Color.DarkGreen;private static float
text_size=0.75f;private static class Sprite{private static MySprite sprite_dot=new MySprite(){Type=SpriteType.TEXT,Data="•",
Alignment=TextAlignment.CENTER,FontId="White"};private static Vector2 sprite_dot_offset=new Vector2(0.0f,31.0f/2.0f);private
static Vector2 sprite_dot_size=new Vector2(41.0f,31.0f);public static MySprite Dot(Vector2 pos,Color color,float scale=1.0f){
sprite_dot.Color=color;sprite_dot.RotationOrScale=scale;sprite_dot.Size=scale*sprite_dot_size;sprite_dot.Position=pos-scale*
sprite_dot_offset;return sprite_dot;}private static MySprite sprite_cross=new MySprite(){Type=SpriteType.TEXT,Data="X",Alignment=
TextAlignment.CENTER,FontId="White"};private static Vector2 sprite_cross_offset=new Vector2(0.0f,35.0f/2.0f);private static Vector2
sprite_cross_size=new Vector2(45.0f,35.0f);public static MySprite Cross(Vector2 pos,Color color,float scale=1.0f){sprite_cross.Color=
color;sprite_cross.RotationOrScale=scale;sprite_cross.Size=scale*sprite_cross_size;sprite_cross.Position=pos-scale*
sprite_cross_offset;return sprite_cross;}private static MySprite sprite_text=new MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.
LEFT,FontId="White"};public static MySprite Text(string text,Vector2 pos,Color color,Vector2 size,float scale=1.0f){
sprite_text.Data=text;sprite_text.Color=color;sprite_text.RotationOrScale=scale;sprite_text.Size=size;sprite_text.Position=pos;
return sprite_text;}private static MySprite sprite_circle_hollow=new MySprite(){Type=SpriteType.TEXTURE,Data="CircleHollow",
Alignment=TextAlignment.CENTER,};public static MySprite CircleHollow(Vector2 pos,float size,Color color){sprite_circle_hollow.
Color=color;sprite_circle_hollow.Size=new Vector2(size);sprite_circle_hollow.Position=pos;return sprite_circle_hollow;}
private static MySprite sprite_circle=new MySprite(){Type=SpriteType.TEXTURE,Data="Circle",Alignment=TextAlignment.CENTER,};
public static MySprite Circle(Vector2 pos,float size,Color color){sprite_circle.Color=color;sprite_circle.Size=new Vector2(
size);sprite_circle.Position=pos;return sprite_circle;}}public static float scale=1.0f;private static List<float>
scale_levels=new List<float>(){0.1f,1.0f,10.0f,100.0f,1000.0f};public static void ToggleScale(){var index=scale_levels.IndexOf(scale
);if(--index<0){scale=scale_levels.Last();}else{scale=scale_levels[index];}}public static void Print(Program p,ref
RecordManager rm){var tagRegex=TagMatchRegex(Program.visual_tag);List<IMyTextPanel>panels=new List<IMyTextPanel>();p.
GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels);foreach(IMyTextPanel panel in panels){if(!tagRegex.IsMatch(panel.CustomName)){
continue;}Print(panel,ref rm,panel.GetPosition(),panel.WorldMatrix.Down,panel.WorldMatrix.Right);}List<IMyProjector>projectors=
new List<IMyProjector>();p.GridTerminalSystem.GetBlocksOfType<IMyProjector>(projectors);foreach(IMyProjector projector in
projectors){if(!tagRegex.IsMatch(projector.CustomName)){continue;}Print(projector.GetSurface(0),ref rm,projector.GetPosition(),
projector.WorldMatrix.Left,projector.WorldMatrix.Backward);}}private static void Print(IMyTextSurface surface,ref RecordManager
rm,Vector3D thisPos,Vector3D thisUp,Vector3D thisRight){surface.ContentType=ContentType.SCRIPT;surface.Script="";surface.
ScriptBackgroundColor=Color.Black;surface.ScriptForegroundColor=Color.Black;RectangleF viewport=new RectangleF((surface.TextureSize-surface.
SurfaceSize)/2.0f,surface.SurfaceSize);MySpriteDrawFrame frame=surface.DrawFrame();var maxSize=(float)Math.Sqrt(Math.Pow(viewport.
Size.X,2.0f)+Math.Pow(viewport.Size.Y,2.0f));var legend=false;var legend_location=0.0f;var circle_diamater_scale_step=200.0f
;for(float circle_size_diameter=(float)Math.Ceiling(maxSize/circle_diamater_scale_step)*circle_diamater_scale_step;
circle_size_diameter>0;circle_size_diameter-=circle_diamater_scale_step){var setColor=fgColor;if(!legend){if(circle_size_diameter<viewport.
Width){legend_location=circle_size_diameter;legend=true;setColor=fgActiveColor;}}frame.Add(Sprite.CircleHollow(viewport.
Center,circle_size_diameter,setColor));frame.Add(Sprite.Circle(viewport.Center,circle_size_diameter-1,bgColor));}var show=
string.Format("\n {0}",ToMetric(scale*legend_location/2.0f));frame.Add(Sprite.Text(show,new Vector2(viewport.Center.X-
legend_location/2.0f,viewport.Center.Y-45.0f*0.75f),fgActiveColor,viewport.Size,0.75f));frame.Add(Sprite.Dot(viewport.Center,
fgActiveColor,0.25f));foreach(var record in rm.records.Values){var relativePos=record.position-thisPos;var v2pos=new Vector2((float)
Vector3D.Dot(relativePos,thisRight),(float)Vector3D.Dot(relativePos,thisUp));var text=string.Format("{0}\n{1}",record.name,
ToMetric((float)relativePos.Length()));var drawingPos=(v2pos/scale)+viewport.Center;frame.Add(Sprite.Cross(drawingPos,dotColor,
dot_size));frame.Add(Sprite.Text(text,drawingPos,textColor.Alpha(0.1f),viewport.Size,text_size));}frame.Dispose();}}class
LoadStats{private class InstructionLoad{public float lowest;public float average;public float highest;public int calls;public
InstructionLoad(float currentLoad){highest=average=lowest=currentLoad;calls=1;}public void Measure(float currentLoad){lowest=Math.Min(
lowest,currentLoad);highest=Math.Max(highest,currentLoad);average=0.9f*average+0.1f*currentLoad;++calls;}}private Dictionary<
string,InstructionLoad>instructionStats=new Dictionary<string,InstructionLoad>();public void Measure(float currentLoad,string
updateType){if(!instructionStats.ContainsKey(updateType)){instructionStats[updateType]=new InstructionLoad(currentLoad);return;}
instructionStats[updateType].Measure(currentLoad);}public override string ToString(){var str="%Min - %Avg - %Max :Source\n";foreach(var
load in instructionStats){str+=String.Format("{1:F0} - {2:F0} - {3:F0} :{0}\n",load.Key,load.Value.lowest*100.0f,load.Value.
average*100.0f,load.Value.highest*100.0f);}return str;}}static partial class Logger{public enum LogType{I,W,E,D}private class
LogEntry{public string entry;public int count;public LogType logType;}private static int MAX_LOG_ENTRIES=8;private static List<
LogEntry>logger=new List<LogEntry>();public static void Log(string line,LogType logType){if(logger.Count>=1&&line==logger[0].
entry&&logType==logger[0].logType){++logger[0].count;return;}logger.Insert(0,new LogEntry{entry=line,count=1,logType=logType}
);if(logger.Count()>MAX_LOG_ENTRIES){logger.RemoveAt(logger.Count()-1);}}public static void Clear(){logger.Clear();}
public static void D(string line){Log(line,LogType.D);}public static void Info(string line){Log(line,LogType.I);}public static
void Warn(string line){Log(line,LogType.W);}public static void Err(string line){Log(line,LogType.E);}public static string
PrintBuffer(){var str="";foreach(var logEntry in logger){str+=logEntry.logType.ToString()+": "+(logEntry.count!=1?"("+logEntry.
count.ToString()+") ":"")+logEntry.entry+"\n";}return str;}}class Rater{private TimeSpan interval;private DateTime
last_update=DateTime.MinValue;public string Name;public Rater(TimeSpan interval){this.interval=interval;this.Name="Update"+((
interval.TotalSeconds>=1.0)?(interval.TotalSeconds.ToString()+"s"):(interval.TotalMilliseconds.ToString()+"ms"));}public bool
Elapsed(){if(DateTime.UtcNow-this.last_update<interval){return false;}last_update=DateTime.UtcNow;return true;}}class Record:
Serialize{public long grid_id=0;public string name="";public float battery_charge=0.0f;public float hydrogen_level=0.0f;public
float speed=0.0f;public int altitude=0;public Vector3D position=new Vector3D();public string docked_grid="";public DateTime
updated=DateTime.UtcNow;public float shield_percent=-1.0f;public int channel=0;public float oxygen_level=0.0f;public Record
GetData(Program p){GetGridData(p);GetDefenseData(p);GetBatteryCharge(p);GetTankLevels(p);GetRCData(p);GetConnectedGrid(p);
return this;}public void Pack(){Pack(grid_id);Pack(name);Pack(battery_charge);Pack(hydrogen_level);Pack(speed);Pack(altitude);
Pack(position);Pack(docked_grid);Pack(updated);Pack(shield_percent);Pack(channel);Pack(oxygen_level);}public string ToSerial
(){InitPack();Pack();return FinishPack();}public void Unpack(){Unpack(ref grid_id);Unpack(ref name);Unpack(ref
battery_charge);Unpack(ref hydrogen_level);Unpack(ref speed);Unpack(ref altitude);Unpack(ref position);Unpack(ref docked_grid);Unpack(
ref updated);try{Unpack(ref shield_percent);Unpack(ref channel);Unpack(ref oxygen_level);}catch{Logger.Warn(
"Unable to decode some data. Update ship scripts to the latest version.");}}public Record FromSerial(string s){InitUnpack(s);Unpack();if(!FinishUnpack()){Logger.Warn(
"Some data not decoded. Update script to the latest version.");}return this;}public void Advertise(Program p){p.IGC.SendBroadcastMessage<string>(Program.security_key,this.ToSerial()
);}private void GetGridData(Program p){this.grid_id=p.Me.CubeGrid.EntityId;this.name=p.Me.CubeGrid.CustomName;this.
channel=Program.channel;}private void GetDefenseData(Program p){var shield=new PbDefenseWrapper(p.Me);if(shield.noDefenseData){
return;}shield_percent=shield.GetShieldPercent();}private void GetBatteryCharge(Program p){float charge=-1.0f;List<
IMyBatteryBlock>batteries=new List<IMyBatteryBlock>();p.GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries);foreach(
IMyBatteryBlock battery in batteries){if(!battery.IsSameConstructAs(p.Me)){continue;}charge=(charge==-1.0f)?(battery.CurrentStoredPower
/battery.MaxStoredPower):((charge+(battery.CurrentStoredPower/battery.MaxStoredPower))/2.0f);}battery_charge=charge;}
private void GetTankLevels(Program p){float h_level=-1.0f;float o_level=-1.0f;List<IMyGasTank>tanks=new List<IMyGasTank>();p.
GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks);foreach(IMyGasTank tank in tanks){if(!tank.IsSameConstructAs(p.Me)){continue;}if(
tank.BlockDefinition.SubtypeId.Contains("Hydro")){h_level=(h_level==-1.0f)?(float)tank.FilledRatio:(h_level+(float)tank.
FilledRatio)/2.0f;}else{o_level=(o_level==-1.0f)?(float)tank.FilledRatio:(o_level+(float)tank.FilledRatio)/2.0f;}}this.
hydrogen_level=h_level;this.oxygen_level=o_level;}private void GetRCData(Program p){List<IMyShipController>shipControllers=new List<
IMyShipController>();p.GridTerminalSystem.GetBlocksOfType<IMyShipController>(shipControllers);foreach(IMyShipController shipController in
shipControllers){if(!shipController.IsSameConstructAs(p.Me)){continue;}double elevation;if(shipController.TryGetPlanetElevation(
MyPlanetElevation.Surface,out elevation)){altitude=(int)elevation;}speed=(float)shipController.GetShipVelocities().LinearVelocity.Length(
);position=shipController.GetPosition();return;}}private void GetConnectedGrid(Program p){List<IMyShipConnector>
connectors=new List<IMyShipConnector>();p.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);foreach(
IMyShipConnector connector in connectors){if(!connector.IsSameConstructAs(p.Me)){continue;}if(connector.Status!=MyShipConnectorStatus.
Connected){continue;}docked_grid=connector.OtherConnector.CubeGrid.CustomName;return;}docked_grid="";}private string space="   ";
private List<string>list_data=new List<string>();private void AddParam(string s,float f){if(f!=-1.0){list_data.Add(string.
Format(s,f));}}public override string ToString(){list_data.Clear();AddParam("B:{0:P0}",battery_charge);AddParam("H:{0:P0}",
hydrogen_level);AddParam("O:{0:P0}",oxygen_level);AddParam("S:{0:F0}",shield_percent);var s=string.Format("{0}\n {1}\n",name,string.
Join(space,list_data));if(docked_grid!=""){s+=string.Format(" dock: {0}\n",docked_grid);}else{list_data.Clear();AddParam(
"v:{0:F1} m/s",speed);AddParam("a:{0:F1} m",altitude);s+=string.Format(" {0}\n",string.Join(space,list_data));}s+=string.Format(
" updated: {0} ago\n",TimeSpanToString((DateTime.UtcNow-updated).Duration()));return s;}}class RecordManager:Serialize{public Dictionary<long
,Record>records=new Dictionary<long,Record>();public string Stats(){if(records.Count==0){return"No records.\n";}if(
records.Count==1){return records.Values.First().ToString();}HashSet<int>channels=new HashSet<int>();DateTime oldest=DateTime.
MaxValue;DateTime newest=DateTime.MinValue;foreach(var record in records.Values){channels.Add(record.channel);oldest=(oldest<
record.updated)?oldest:record.updated;newest=(newest>record.updated)?newest:record.updated;}return string.Format("Stats --\n"+
" entries: {0}\n"+" channels: {1}\n"+" newest: {2} ago\n"+" oldest: {3} ago\n",records.Count(),string.Join(", ",channels),
TimeSpanToString((DateTime.UtcNow-newest).Duration()),TimeSpanToString((DateTime.UtcNow-oldest).Duration()));}public void CollectRecords
(Program p){IMyBroadcastListener listener=p.IGC.RegisterBroadcastListener(Program.security_key);while(listener.
HasPendingMessage){var igcData=listener.AcceptMessage();try{Record r=new Record().FromSerial((string)igcData.Data);records[r.grid_id]=r;}
catch(Exception exception){Logger.Err("Receive record: "+exception.Message);}}}public string GetChannelRecords(int channel){
string sum="";string gps="";foreach(var record in records.Values){if(channel==-1||record.channel==channel){sum+=record.
ToString();gps+=GenerateGPS(record.name,record.position)+"\n";}}return sum+"\n"+gps;}private Dictionary<int,string>
channel_buffer=new Dictionary<int,string>();public void WriteToPannels(Program p){channel_buffer.Clear();var tagRegex=
TagNumberMatchRegex(Program.panel_tag);List<IMyTextPanel>panels=new List<IMyTextPanel>();p.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>
(panels);foreach(IMyTextPanel panel in panels){if(!Program.subgrid_panels&&!panel.IsSameConstructAs(p.Me)){continue;}
System.Text.RegularExpressions.Match match=tagRegex.Match(panel.CustomName);if(!match.Success){continue;}panel.ContentType=
ContentType.TEXT_AND_IMAGE;panel.ClearImagesFromSelection();var ch=-1;if(match.Groups[3].Value!=""){ch=int.Parse(match.Groups[3].
Value);}if(!channel_buffer.ContainsKey(ch)){channel_buffer[ch]=GetChannelRecords(ch);}panel.WriteText(channel_buffer[ch]);}}
public string ToSerial(){InitPack();Pack(records.Count);foreach(KeyValuePair<long,Record>p in records){Pack(p.Key);p.Value.
serialized=this.serialized;p.Value.Pack();}return FinishPack();}public void FromSerial(string s){InitUnpack(s);int count=0;Unpack(
ref count);for(var i=0;i<count;i++){long key=0;Unpack(ref key);Record record=new Record();record.serialized=this.serialized
;record.Unpack();records[key]=record;}}}class Serialize{private string[]separator=new string[]{"\n"};public Queue<string>
serialized=new Queue<string>();public Serialize InitPack(){serialized.Clear();return this;}public string FinishPack(){return
string.Join(separator[0],serialized);}public Serialize InitUnpack(string str){serialized=new Queue<string>(str.Split(separator
,StringSplitOptions.None));return this;}public bool FinishUnpack(){return serialized.Count==0;}public Serialize Pack(
string str){serialized.Enqueue(str.Replace(separator[0]," "));return this;}public Serialize Pack(int val){serialized.Enqueue(
val.ToString());return this;}public Serialize Pack(long val){serialized.Enqueue(val.ToString());return this;}public
Serialize Pack(float val){serialized.Enqueue(val.ToString());return this;}public Serialize Pack(double val){serialized.Enqueue(
val.ToString());return this;}public Serialize Pack(bool val){serialized.Enqueue(val?"1":"0");return this;}public Serialize
Pack(VRage.Game.MyCubeSize val){Pack((int)val);return this;}public Serialize Pack(Vector3D val){Pack(val.X);Pack(val.Y);Pack
(val.Z);return this;}public Serialize Pack(List<Vector3D>val){Pack(val.Count);foreach(Vector3D v in val){Pack(v);}return
this;}public Serialize Pack(List<int>val){Pack(val.Count);foreach(int v in val){Pack(v);}return this;}public Serialize Pack(
DateTime t){Pack(t.ToBinary());return this;}public Serialize Unpack(ref string val){val=serialized.Dequeue();return this;}public
Serialize Unpack(ref int val){val=int.Parse(serialized.Dequeue());return this;}public Serialize Unpack(ref long val){val=long.
Parse(serialized.Dequeue());return this;}public Serialize Unpack(ref float val){val=float.Parse(serialized.Dequeue());return
this;}public Serialize Unpack(ref double val){val=double.Parse(serialized.Dequeue());return this;}public Serialize Unpack(
ref bool val){val=serialized.Dequeue()=="1";return this;}public Serialize Unpack(ref MyCubeSize val){val=(MyCubeSize)int.
Parse(serialized.Dequeue());return this;}public Serialize Unpack(ref Vector3D val){Unpack(ref val.X).Unpack(ref val.Y).Unpack
(ref val.Z);return this;}public Serialize Unpack(ref List<Vector3D>list){int count=0;Unpack(ref count);for(int i=0;i<
count;i++){Vector3D v3d=new Vector3D();Unpack(ref v3d);list.Add(v3d);}return this;}public Serialize Unpack(ref List<int>val){
int count=0;Unpack(ref count);for(int i=0;i<count;i++){int integer=0;Unpack(ref integer);val.Add(integer);}return this;}
public Serialize Unpack(ref DateTime t){t=DateTime.FromBinary(long.Parse(serialized.Dequeue()));return this;}}class Settings{
public MyIni ini=new MyIni();public List<Section>sections=new List<Section>();public Settings(){}public Section NewSection(
string name){var s=new Section(name);sections.Add(s);return s;}public string UpdateSettings(string iniString){ini.Clear();ini.
TryParse(iniString);foreach(var s in this.sections){s.UpdateSection(ref ini);}return ini.ToString();}}class Setting{public enum
Content{INT,FLOAT,STRING,BOOL};public object setting;public Content content;public void IniSet(ref MyIni ini,string sectionName
,string keyName){switch(this.content){case Setting.Content.BOOL:ini.Set(sectionName,keyName,(bool)this.setting);break;
case Setting.Content.INT:ini.Set(sectionName,keyName,(int)this.setting);break;case Setting.Content.FLOAT:ini.Set(sectionName
,keyName,(float)this.setting);break;case Setting.Content.STRING:ini.Set(sectionName,keyName,(string)this.setting);break;}
}public bool IniGet(ref MyIni ini,string sectionName,string keyName){switch(this.content){case Setting.Content.BOOL:bool
boolVal;if(!bool.TryParse(ini.Get(sectionName,keyName).ToString(),out boolVal)){return false;}setting=boolVal;break;case
Setting.Content.INT:int intVal;if(!int.TryParse(ini.Get(sectionName,keyName).ToString(),out intVal)){return false;}setting=
intVal;break;case Setting.Content.FLOAT:float floatVal;if(!float.TryParse(ini.Get(sectionName,keyName).ToString(),out floatVal
)){return false;}setting=floatVal;break;case Setting.Content.STRING:setting=ini.Get(sectionName,keyName).ToString();break
;}return true;}}class Section{public Dictionary<string,Setting>settings=new Dictionary<string,Setting>();public string
sectionName="";public Section(string sectionName){this.sectionName=sectionName;}public void RegisterString(string name,string
defaultValue){settings[name]=new Setting{content=Setting.Content.STRING,setting=defaultValue};}public void RegisterFloat(string name
,float defaultValue){settings[name]=new Setting{content=Setting.Content.FLOAT,setting=defaultValue};}public void
RegisterBool(string name,bool defaultValue){settings[name]=new Setting{content=Setting.Content.BOOL,setting=defaultValue};}public
void RegisterInt(string name,int defaultValue){settings[name]=new Setting{content=Setting.Content.INT,setting=defaultValue};
}public string GetString(string name){return(string)settings[name].setting;}public float GetFloat(string name){return(
float)settings[name].setting;}public bool GetBool(string name){return(bool)settings[name].setting;}public int GetInt(string
name){return(int)settings[name].setting;}public void UpdateSection(ref MyIni ini){if(!ini.ContainsSection(this.sectionName))
{SaveSection(ref ini);}else{LoadSection(ref ini);}}public void SaveSection(ref MyIni ini){foreach(var pair in settings){
pair.Value.IniSet(ref ini,sectionName,pair.Key);}}public void LoadSection(ref MyIni ini){List<MyIniKey>iniKeys=new List<
MyIniKey>();ini.GetKeys(sectionName,iniKeys);foreach(var invalidKey in iniKeys.Where(k=>!settings.Keys.Contains(k.Name))){ini.
Delete(invalidKey);}foreach(var pair in settings){if(ini.ContainsKey(sectionName,pair.Key)){if(pair.Value.IniGet(ref ini,
sectionName,pair.Key)){continue;}}settings[pair.Key].IniSet(ref ini,sectionName,pair.Key);}}}static System.Text.RegularExpressions.
Regex tagFormatRegex=new System.Text.RegularExpressions.Regex("\\[([^\\[\\]]+)\\]");static System.Text.RegularExpressions.
Regex TagMatchRegex(string tags){return new System.Text.RegularExpressions.Regex(@"\[("+tags.Replace(",","|")+@")\]");}static
System.Text.RegularExpressions.Regex TagNumberMatchRegex(string tags){return new System.Text.RegularExpressions.Regex(@"\[("+
tags.Replace(",","|")+@")(_(\d+))?\]");}static System.Text.RegularExpressions.Regex TagWordMatchRegex(string tags){return
new System.Text.RegularExpressions.Regex(@"\[("+tags.Replace(",","|")+@")(_([a-zA-Z]+))?\]");}static System.Text.
RegularExpressions.Regex TagActionMatchRegex(string tags){return new System.Text.RegularExpressions.Regex("\\[("+tags.Replace(",","|")+
")_([vxyzVXYZ])\\]");}static string ToMetric(float n){if(n>1000000000.0f){return string.Format("{0:F1}Gm",n/1000000000.0f);}else if(n>
1000000.0f){return string.Format("{0:F1}Mm",n/1000000.0f);}else if(n>1000.0f){return string.Format("{0:F1}km",n/1000.0f);}return
string.Format("{0:F0}m",n);}static string TimeSpanToString(TimeSpan ts){string s;if(ts.TotalMinutes<1.0){s=String.Format(
"{0}s",ts.Seconds);}else if(ts.TotalHours<1.0){s=String.Format("{0}m:{1:D2}s",ts.Minutes,ts.Seconds);}else{s=String.Format(
"{0}h:{1:D2}m:{2:D2}s",(int)ts.TotalHours,ts.Minutes,ts.Seconds);}return s;}static string GenerateGPS(string name,Vector3D v,string color=
"FF00FF"){return string.Format("GPS:{0}:{1}:{2}:{3}:#{4}:",name,v.X,v.Y,v.Z,color);}static Vector3D GetOrbit(Program p,double
altitude){List<IMyShipController>shipControllers=new List<IMyShipController>();p.GridTerminalSystem.GetBlocksOfType<
IMyShipController>(shipControllers);foreach(IMyShipController shipController in shipControllers){if(!shipController.IsSameConstructAs(p.
Me)){continue;}Vector3D planetCenter;if(!shipController.TryGetPlanetPosition(out planetCenter)){throw new Exception(
"Not in gravity?");}double currentAltitude;if(!shipController.TryGetPlanetElevation(MyPlanetElevation.Surface,out currentAltitude)){throw
new Exception("Not in gravity?");}Vector3D currentPosition=shipController.GetPosition();return(altitude-currentAltitude)*
Vector3D.Normalize(currentPosition-planetCenter)+currentPosition;}throw new Exception("No RCs or Cockpits?");}