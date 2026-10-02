// HudDog - dogfight tracers, lead, autoaim, version 1.0.287 (c) cheerkin
// https://steamcommunity.com/sharedfiles/filedetails/?id=2645141226

const string Ver = "HD 1.1.287";
const float SP_LIMIT = 104.375f;

// how much surface the actual texture is (s/l transparent lcd)
const float LCD_TEXTURE_SCALE_L = 0.87f;
const float LCD_TEXTURE_SCALE_S = 0.87f;

// lower values make rotation more conservative, decrease if you experience excessive steering (pitch, yaw, roll)
// can be set via CustomData like this - command:set-grid-accel:1.2:0.5:2.5
static Vector3D GRID_ANGULAR_ACCELERATIONS = new Vector3D(0.5f, 0.5f, 1.02f);
static float RIPPLE_GYRO_BOOST = 1f;

float interfaceUpscaleLargeGrid = 1f;
float interfaceUpscaleSmallGrid = 1.15f;
int? CockpitScreenIndex = 0; // set to null or -1 to disable output to cockpit embedded screen

MyTuple<Color, Color> TRACER_COL = new MyTuple<Color, Color>(new Color(255, 125, 125), new Color(200, 30, 50, 30));
// main sprite color, main text color
MyTuple<Color, Color> HUD_COL_DEFAULT = new MyTuple<Color, Color>(new Color(r: 255, g: 195, b: 110, a: 80), new Color(r: 255, g: 195, b: 110, a: 30));
// bright mode sprite color, bright mode text color
MyTuple<Color, Color> HUD_COL_BRIGHT = new MyTuple<Color, Color>(new Color(r: 30, g: 255, b: 50, a: 200), new Color(r: 30, g: 195, b: 50, a: 100));

static class Variables
{
	static Dictionary<string, object> v = new Dictionary<string, object> {
		{ "rdc-vel", new Variable<float> { value = SP_LIMIT, parser = float.Parse } },
		{ "default-zero-range", new Variable<float> { value = 1000f, parser = float.Parse } }, // range for block-throw weapons
		{ "filtering-size", new Variable<float> { value = 10f, parser = float.Parse } },
		{ "funnel-base", new Variable<float> { value = 7f, parser = float.Parse } },
		{ "tick-avg-vel", new Variable<int> { value = 60, parser = int.Parse } }, // 30
		{ "projected-grid-forward-offset", new Variable<int> { value = 0, parser = int.Parse } },
		//{ "seat-pov-up-offset", new Variable<float> { value = 0f, parser = s => float.Parse(s) } }, // Obsolete: use cockpit custom data ini
		//{ "seat-pov-fw-offset", new Variable<float> { value = 0f, parser = s => float.Parse(s) } },
		{ "cast-bb-convex", new Variable<bool> { value = false, parser = s => s == "true" } },
		{ "double-tap-actions", new Variable<bool> { value = true, parser = s => s == "true" } },
		{ "skip-intro", new Variable<bool> { value = false, parser = s => s == "true" } },
		{ "tracking-clock", new Variable<int> { value = 5, parser = int.Parse } },
		{ "raycast-range", new Variable<float> { value = 3500f, parser = float.Parse } },
		{ "turret-ai-cone", new Variable<float> { value = 0.85f, parser = float.Parse } },
		{ "squared-offset-filter", new Variable<float> { value = 16f, parser = float.Parse } },

		{ "tracers-clock", new Variable<int> { value = 20, parser = int.Parse } }, // 0 to disable
		{ "ripple-precision-override", new Variable<float> { value = 0, parser = float.Parse } },
		{ "ripple-increment-interval", new Variable<int> { value = 60, parser = int.Parse } },
		{ "ripple-increment-interval-rail", new Variable<int> { value = 20, parser = int.Parse } },
		{ "ripple-gyro-boost", new Variable<float> { value = 4, parser = float.Parse } },
		{ "autodrop-timeout", new Variable<int> { value = 60, parser = int.Parse } },
		{ "zero-range", new Variable<float> { value = 600f, parser = float.Parse } } // max range for lockless funnel mode
	};
	public static void Set(string key, string value) { (v[key] as ISettable).Set(value); }
	public static void Set<T>(string key, T value) { (v[key] as ISettable).Set(value); }
	public static T Get<T>(string key) { return (v[key] as ISettable).Get<T>(); }
	public interface ISettable
	{
		void Set(string v);
		T1 Get<T1>();
		void Set<T1>(T1 v);
	}
	public class Variable<T> : ISettable
	{
		public T value;
		public Func<string, T> parser;
		public void Set(string v) { value = parser(v); }
		public void Set<T1>(T1 v) { value = (T)(object)v; }
		public T1 Get<T1>() { return (T1)(object)value; }
	}
}

public enum PovType : byte { Unknown = 0, LargeControlSeat = 1, CameraView = 3 }
PovType CurrentPovType;
public enum IndicationType : byte { SpriteApi = 0, Projector = 1 }
IndicationType CurrentIndicationType;

public enum BallisticModelKind { Ballistic, Rocket, InertialGrid, BallisticCustom, FreeFall }
č Щ;á Ш;Ù Ч;Ã Ц;Ī Х;ĳ Ф;ɰ У;class Т{static Т С;Т(){}Action<string>Р;Dictionary<string,bool>П;Т(Dictionary<string,bool>Н,
Action<string>М){Р=М;П=Н;}public static Т О=>С;public static void Ŗ(Dictionary<string,bool>Н,Action<string>М){if(С==null)С=new
Т(Н,М);}public void Л(string ɷ,bool Ъ){if(П[ɷ]!=Ъ)з(ɷ);}public void з(string ɷ){П[ɷ]=!П[ɷ];Р(ɷ);}public bool ж(string ɷ){
return П[ɷ];}public ImmutableArray<MyTuple<string,string>>е(string д){return П.Select(Ʒ=>new MyTuple<string,string>(
$"{Ʒ.Key}: {(Ʒ.Value?"on":"off")}",$"[toggle:{Ʒ.Key}],[{д}]")).ToImmutableArray();}}bool г;б в;class б{Dictionary<string,Action<string[]>>Э;public б(
Dictionary<string,Action<string[]>>Э){this.Э=Э;}public void а(string Ć,string[]Ь){this.Э[Ć].Invoke(Ь);}}void Я(string Ю){ś.ŗ=Math.
Max(0.001,Runtime.TimeSinceLastRun.TotalSeconds);ś.Ř+=ś.ŗ;if(Runtime.TimeSinceLastRun.TotalSeconds>0)ś.G++;if(Ѭ>0&&ś.G%60==
0)њ();if(г&&string.IsNullOrEmpty(Ю)){г=false;Ю=string.Join(",",Me.CustomData.Trim('\n').Split(new[]{'\n'},
StringSplitOptions.RemoveEmptyEntries).Where(đ=>!đ.StartsWith("//")).Select(đ=>"["+đ+"]"));}if(!string.IsNullOrEmpty(Ю)&&Ю.Contains(":")){
var Э=Ю.Split(new[]{"],["},StringSplitOptions.RemoveEmptyEntries).Select(đ=>đ.Trim('[',']','\r')).ToList();foreach(var Á in
Э){string[]Ь=Á.Split(new[]{':'},StringSplitOptions.RemoveEmptyEntries);if(Ь[0]=="toggle"){var ƶ=Ь[1];if(Ь.Length>2)Т.О.Л(
ƶ,bool.Parse(Ь[2]));else Т.О.з(ƶ);}if(Ь[0]=="command"){this.в.а(Ь[1],Ь);}}}}void и(){}IMyProgrammableBlock Ы;static bool
й;void Ѣ(){ś.Ŗ(this);if(!string.IsNullOrEmpty(Me.CustomData))г=true;ύ=Me.CubeGrid.GridSizeEnum==MyCubeSize.Large?
interfaceUpscaleLargeGrid:interfaceUpscaleSmallGrid;List<IMyProgrammableBlock>ѡ=new List<IMyProgrammableBlock>();GridTerminalSystem.
GetBlocksOfType(ѡ,ƴ=>ƴ.CustomName.Contains("a-hud-svc")&&(ƴ.IsSameConstructAs(Me)));Ы=ѡ.FirstOrDefault();var ї=ф(Me);if(ї==null){
Runtime.UpdateFrequency=UpdateFrequency.None;Echo("Can't find hardware group containing this PB, stopping now.");}else{var Ѡ=
new List<IMyTerminalBlock>();ї.GetBlocks(Ѡ);var џ=ȹ<IMyShipController>(Ѡ);if(џ.Count==0){Runtime.UpdateFrequency=
UpdateFrequency.None;Echo("Need at least one cockpit or control station, stopping now.");}var ў=new Dictionary<string,Vector3D>();ў.Add
("LargeBlockCockpitSeat",new Vector3D(-0.00453,0.49836,0.18359));ў.Add("LargeBlockCockpitIndustrial",new Vector3D(0.00577
,0.08591,-0.18941));ў.Add("OpenCockpitLarge",new Vector3D(0,0.00818,-0.18584));ў.Add("LargeBlockStandingCockpit",new
Vector3D(0,0.46161,-0.68934));ў.Add("CockpitOpen",new Vector3D(0.01603,0.49966,0.43399));ў.Add("LargeBlockCockpit",new Vector3D(
-0.00453,0.59745,0.50189));ў.Add("SmallBlockCockpit",new Vector3D(-0.00453,0.46711,0.32022));ў.Add(
"SmallBlockFlushCockpit",new Vector3D(0,0.44617,0.22867));ў.Add("DBSmallBlockFighterCockpit",new Vector3D(0.00577,0.43444,0.73861));ў.Add(
"SmallBlockCockpitIndustrial",new Vector3D(0.00577,0.35507,0.01861));ў.Add("SmallBlockCapCockpit",new Vector3D(0,0.45322,0.12095));ў.Add(
"OpenCockpitSmall",new Vector3D(3E-05,0.49816,0.19997));ў.Add("RoverCockpit",new Vector3D(0,0.40906,0.06525));ў.Add("BuggyCockpit",new
Vector3D(0,0.33909,0.15938));ў.Add("SmallBlockStandingCockpit",new Vector3D(0,0.70495,-0.19538));ў.Add("SpeederCockpit",new
Vector3D(0,0.39432,-0.39239));ў.Add("SpeederCockpitCompact",new Vector3D(0,0.39432,-0.39239));ў.Add("PassengerSeatSmallNew",new
Vector3D(0,0.43426,-0.07315));ў.Add("PassengerSeatSmallOffset",new Vector3D(0.00011,0.43425,-0.07315));ў.Add("PassengerBench",
new Vector3D(-0.04347,0.44711,-0.00281));ў.Add("PassengerSeatLarge",new Vector3D(0,-0.08042,0.66043));ӫ=new Ӫ(џ,()=>ś.G,ў);
р=new п(GridTerminalSystem,ї,ѭ(),Me.CubeGrid);if(р.ч!=null)ծ=new խ(р.ч,ӫ,р.к.FirstOrDefault());if(р.о.Count>0)
CurrentIndicationType=IndicationType.SpriteApi;else if(р.н!=null)CurrentIndicationType=IndicationType.Projector;if(CurrentIndicationType!=
IndicationType.Projector&&р.н!=null)р.н.Enabled=false;Щ=new č();if(р.ë.Any())Ц=new Ã(р.ë);var ѝ=new List<IMyBlockGroup>();
GridTerminalSystem.GetBlockGroups(ѝ,º=>º.Name.Contains("hd-raycast"));var ќ=ѝ.SingleOrDefault();if(ќ!=null){Ш=new á(с<IMyCameraBlock>(ќ),
Me.CubeGrid);Ч=new Ù(с<IMyCameraBlock>(ќ),џ,Đ);}if(р.ģ.Any()){Func<MyDetectedEntityInfo,bool>ћ=º=>{var Ʊ=Variables.Get<
float>("filtering-size");return Ʊ<=0||Ʊ<=º.BoundingBox.Extents.Length();};Х=new Ī(р.ģ,Đ,ӫ.Ө,ћ);}У=new ɰ(IGC);Ф=new ĳ(У);ͳ=new
Ͳ(IGC,ʹ);var Ƥ=new List<Ƨ>{Щ,Ч,Ш,Х,Ц,Ф};Ƥ.RemoveAll(º=>º==null);Ɨ=new Ɩ(Ƥ,ӫ.Ө);Ǻ=new ȅ(GridTerminalSystem);if(Ǻ.ǚ(џ.First
().GetPosition())>0)Օ.AddLast(new қ(BallisticModelKind.FreeFall,0,SP_LIMIT,"Bomb"));Ǻ.ǝ+=р.ј;if(!Օ.Any())Օ.AddLast(new қ(
BallisticModelKind.InertialGrid,Variables.Get<float>("default-zero-range"),SP_LIMIT,"Inertial PMW"));if(р.о.Any()){foreach(var U in р.о)ϊ.
Add(U,new List<MySprite>());þ=new ý(IGC,"hud-dog",р.о.First().TextureSize);þ.Ě("MainScreen",()=>{return Ԧ();});}Ǔ();}Т.Ŗ(
new Dictionary<string,bool>{{"pip",true},{"fire",true},{"aim",true},{"360-aim",false},{"bright",false},{"pip-road",true},{
"damage-scan",true},{"angular-eval",true},{"aim-to-center",false},{"coax-ripple",true},{"raycast-burst",true},{"mouse-shifting",false
}},ɷ=>{switch(ɷ){case"log-message":break;case"mouse-shifting":var Á=ӫ.Ө();ϋ=Vector2.Zero;if(Á!=null){if(Т.О.ж(
"mouse-shifting"))Á.SetValueBool("ControlGyros",false);else Á.SetValueBool("ControlGyros",true);}break;}});в=new б(new Dictionary<string
,Action<string[]>>{{"set-value",(Ƙ)=>Variables.Set(Ƙ[2],Ƙ[3])},{"cycle-indication-type",(Ƙ)=>Ԁ()},{"next-model",(Ƙ)=>Ւ()}
,{"prev-model",(Ƙ)=>Ւ(true)},{"set-model",(Ƙ)=>Ր(Ƙ[2])},{"get-toggles",(Ƙ)=>{var д=string.Join(":",Ƙ.Take(3));IGC.
SendUnicastMessage(long.Parse(Ƙ[2]),$"menucommand.get-commands.reply:{д}",Т.О.е(д));}},{"color-hud-default",(Ƙ)=>HUD_COL_DEFAULT=new
MyTuple<Color,Color>(ƙ(Ƙ.Skip(2).ToArray()),ƙ(Ƙ.Skip(6).ToArray()))},{"color-hud-bright",(Ƙ)=>HUD_COL_DEFAULT=new MyTuple<Color
,Color>(ƙ(Ƙ.Skip(2).ToArray()),ƙ(Ƙ.Skip(6).ToArray()))},{"cockpit-output",(Ƙ)=>CockpitScreenIndex=int.Parse(Ƙ[2])},{
"fw-cast",(Ƙ)=>Ч?.ç(ӫ.Ө())},{"cast",(Ƙ)=>Ч?.è(ӫ.Ө())},{"clear-rc",(Ƙ)=>{Ч?.Ç();Ш?.Ç();}},{"remove-offset",(Ƙ)=>Ч?.È()},{"tom-src"
,(Ƙ)=>Đ?.ʢ()},{"gps",(Ƙ)=>Щ.â(ű.ū(Ƙ[2],Ƙ[3],Ƙ[4]))},{"static-cast",(Ƙ)=>Ш?.Ý(ӫ.Ө())},{"bind-targeters",(Ƙ)=>Ɨ.ƪ(Ƙ[2].
Split(',').ToList())},{"enable-inertial-pmw",(Ƙ)=>Օ.AddLast(new қ(BallisticModelKind.InertialGrid,Variables.Get<float>(
"default-zero-range"),SP_LIMIT,"Inertial PMW"))},{"set-grid-accel",(Ƙ)=>GRID_ANGULAR_ACCELERATIONS=new Vector3D(float.Parse(Ƙ[2]),float.
Parse(Ƙ[3]),float.Parse(Ƙ[4]))},{"drop",Ǻ.ǳ},{"cycle-bomb",(Ƙ)=>Ǻ.Ƕ()},{"set-selector",(Ƙ)=>Ɨ.ư(Ƙ[2])},});}ҹ ѭ(List<
MyDefinitionId>ѫ=null){var ї=ф(Me);var Ѫ=new List<IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(Ѫ,º=>º.Name.Contains("[hd-coax]")
||º.Name.Contains("[hd-custom]"));var ѩ=new List<Վ>();foreach(var Ѩ in Ѫ){var ѧ=new List<IMyTerminalBlock>();Ѩ.
GetBlocksOfType(ѧ);if(ѧ.Any()){ѩ.Add(new Վ(Ѩ,BallisticModelKind.BallisticCustom,ѧ));}}var Ѧ=new List<IMyUserControllableGun>();ї.
GetBlocksOfType(Ѧ);var ѥ=GridTerminalSystem.GetBlockGroupWithName("coaxial-weapons");if(ѥ!=null){var Ѥ=new List<IMyUserControllableGun>
();ѥ.GetBlocksOfType(Ѥ);Ѧ.AddRange(Ѥ);}if(ѫ!=null){var ѣ=new List<IMyTerminalBlock>();ї.GetBlocks(ѣ,º=>ѫ.Contains(º.
BlockDefinition));ś.œ($"Modded weapons subset: {ѣ.Count}");return new ҹ(ѣ,ѩ,Օ,Me.CubeGrid);}return new ҹ(Ѧ,ѩ,Օ,Me.CubeGrid);}int Ѭ=5;
void њ(){Ѭ--;var ё=Me.GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<string,Delegate>>().GetValue(Me);if(ё!=null){ś.œ(
"WcPbAPI is initialized");Ѭ=0;if(ё.ContainsKey("GetSortedThreatsByID")){ś.œ(
"WC detected, disabling ITgpTargeter and TurretAiTargeter instances.");Ɨ.ƕ.Clear();var ц=ё["GetSortedThreatsByID"]as Action<IMyTerminalBlock,IDictionary<long,MyDetectedEntityInfo>>;if(ц!=
null){ś.œ("Creating WcTargeter.");Ļ=new ĺ(ц,Me,ӫ.Ө);Ɨ.ƕ.Add(Ļ);}}if(ё.ContainsKey("GetCoreWeapons")){var ц=ё[
"GetCoreStaticLaunchers"]as Action<ICollection<MyDefinitionId>>;if(ц!=null){ś.œ(
"WC detected, recreating weapon face based on WC-supplied definitions");var х=new List<MyDefinitionId>();ц(х);р.ш=ѭ(х);}}}}IMyBlockGroup ф(IMyTerminalBlock у){List<IMyBlockGroup>т=new List<
IMyBlockGroup>();GridTerminalSystem.GetBlockGroups(т);return т.Where(õ=>{var ȧ=new List<IMyTerminalBlock>();õ.GetBlocksOfType(ȧ);
return ȧ.Contains(у);}).FirstOrDefault();}List<Ř>с<Ř>(IMyBlockGroup õ,Func<IMyTerminalBlock,bool>U=null)where Ř:class,
IMyTerminalBlock{List<Ř>ȧ=new List<Ř>();õ.GetBlocksOfType(ȧ,U);return ȧ;}Program(){Runtime.UpdateFrequency=UpdateFrequency.Update1;Ѣ();}
п р;class п{IMyGridTerminalSystem ǟ;public List<IMyTextPanel>о=new List<IMyTextPanel>();public IMyProjector н;public List
<IMyCameraBlock>м=new List<IMyCameraBlock>();public List<IMyLargeTurretBase>ģ=new List<IMyLargeTurretBase>();public List<
IMyTurretControlBlock>ë=new List<IMyTurretControlBlock>();public List<IMyGyro>л=new List<IMyGyro>();public IMyGyro ч;public List<
IMySoundBlock>к=new List<IMySoundBlock>();public ҹ ш;public List<IMyThrust>љ=new List<IMyThrust>();public п(IMyGridTerminalSystem Ǟ,
IMyBlockGroup ї,ҹ і,IMyCubeGrid ѕ){ǟ=Ǟ;ї.GetBlocksOfType(о);о.ForEach(đ=>{đ.ContentType=ContentType.SCRIPT;đ.Script="";đ.
ScriptBackgroundColor=Color.Transparent;});var ŕ=new List<IMyProjector>();ї.GetBlocksOfType(ŕ);н=ŕ.FirstOrDefault();if(н!=null)н.
ProjectionOffset=Vector3I.Zero;Ǟ.GetBlocksOfType(м);м.ForEach(º=>º.EnableRaycast=true);ї.GetBlocksOfType(ģ);ї.GetBlocksOfType(ë);ї.
GetBlocksOfType(к);ї.GetBlocksOfType(л);ч=л.FirstOrDefault();ш=і;ǟ.GetBlocksOfType(ѐ);var є=new List<IMyTerminalBlock>();foreach(var ѓ
in ш.ҳ)є.AddRange(ѓ.ҝ);ь.Add(new MyTuple<string,List<IMyTerminalBlock>>("WPN",є));ь.Add(new MyTuple<string,List<
IMyTerminalBlock>>("CAM",м.Cast<IMyTerminalBlock>().ToList()));ь.Add(new MyTuple<string,List<IMyTerminalBlock>>("LCD",о.Cast<
IMyTerminalBlock>().ToList()));if(ч!=null){ǟ.GetBlocksOfType(љ,º=>º.CubeGrid==ч.CubeGrid);ь.Add(new MyTuple<string,List<IMyTerminalBlock
>>("THR",љ.Cast<IMyTerminalBlock>().ToList()));}}public void ј(IMyCubeGrid ђ){ѐ.RemoveAll(º=>º.CubeGrid==ђ);}List<
IMyTerminalBlock>ѐ=new List<IMyTerminalBlock>();StringBuilder я=new StringBuilder();public string ю;public float э;List<MyTuple<string,
List<IMyTerminalBlock>>>ь=new List<MyTuple<string,List<IMyTerminalBlock>>>();public void ε(int G){я.Clear();if((G%30==0)&&ѐ.
Count>0){э=ъ(ѐ);}if(э>0f)я.AppendLine($"General dmg: {э*100:f0}%");if(ь.Count>0){var ы=ь[(G/60)%ь.Count];я.AppendLine(
$"        {ы.Item1}: {(ы.Item2.Count>0?ъ(ы.Item2)*100:0):f0}%");}if(ч!=null&&!ч.IsFunctional)я.AppendLine($"Gyro failure");ю=я.ToString();}float ъ(List<IMyTerminalBlock>ȧ){int щ=0;
foreach(var у in ȧ){if(у.Closed||!у.IsFunctional)щ++;}return(float)щ/ȧ.Count;}}ϫ К=new ϫ();class ϫ{class ω{public Vector3D ψ;
public Vector3D χ;public Vector3D φ;public float υ;public bool J=true;public bool τ;public float σ;public int ς;public ω ρ(){τ
=false;J=true;return this;}}Queue<ω>π=new Queue<ω>(200);Func<Vector3D>ο;public void ξ(Vector3D U,Vector3D ν,float μ,
Vector3D λ,Func<Vector3D>κ,float ι,float θ,float η=0){var ƴ=π.Count>0?π.Dequeue().ρ():new ω();ƴ.ψ=U;ƴ.χ=λ+ν*μ;ƴ.φ=U;ƴ.υ=ι;ƴ.σ=η;
ƴ.ς=ś.G+(int)(θ*60);ο=κ;ζ.Add(ƴ);}List<ω>ζ=new List<ω>();Random ʪ=new Random();public void ε(Vector3D Ξ,bool Λ){int Ϙ=0;
foreach(var ƴ in ζ){Ϙ++;if(ζ.Count-Ϙ>400){ƴ.J=false;if(π.Count<50)π.Enqueue(ƴ);continue;}var ϗ=(ƴ.ψ-ƴ.φ).Length();if(ś.G<ƴ.ς&&ϗ
<ƴ.υ){ƴ.ψ+=ƴ.χ/60f;var ʳ=ο.Invoke();var ũ=ʳ-ƴ.ψ;if(!ƴ.τ&&Λ&&ũ.Length()<10){ũ+=new Vector3D(1-ʪ.NextDouble()*2,1-ʪ.
NextDouble()*2,1-ʪ.NextDouble()*2)*0.3f;ƴ.τ=true;ƴ.χ/=5-ʪ.NextDouble()*3;ƴ.χ+=Vector3D.Reflect(ƴ.χ,Vector3D.Normalize(ũ));}ƴ.χ+=Ξ/
60f;if(ƴ.σ!=0){ϖ(ƴ,ƴ.σ);}}else{ƴ.J=false;if(π.Count<50)π.Enqueue(ƴ);}}ζ.RemoveAll(º=>!º.J);}void ϖ(ω ƴ,float ϑ){if(ƴ.χ.
LengthSquared()>ϑ*ϑ){ƴ.χ=Vector3D.Normalize(ƴ.χ)*ϑ;}}public Vector3D ϕ(Vector3D ň,Vector3D Ξ,Vector3D ϔ,Vector3D λ,double ϒ,float ϑ,
ref int ϐ){var Ϗ=ϒ*ϒ;var ƴ=new ω{ψ=ň,χ=λ};ϐ=0;while(true){ƴ.χ+=Ξ/60f;ϖ(ƴ,ϑ);ƴ.ψ+=ƴ.χ/60f;if(ϐ++>6000||(ϔ-ƴ.ψ).LengthSquared
()<Ϗ){return ƴ.ψ;}}}public IEnumerable<MyTuple<Vector3D,Vector3D>>ώ(){foreach(var ƴ in ζ){yield return new MyTuple<
Vector3D,Vector3D>(ƴ.ψ,ƴ.ψ-ƴ.χ/30);}}}static float ύ;Vector3D?ό;Vector2 ϋ;Dictionary<IMyTextPanel,List<MySprite>>ϊ=new
Dictionary<IMyTextPanel,List<MySprite>>();List<MyIGCMessage>ɦ=new List<MyIGCMessage>();void Main(string Ώ,UpdateType Ο){try{Я(Ώ);ӫ
.Ӊ();var Á=ӫ.Ө();var Ξ=Á?.GetNaturalGravity()??Vector3D.Zero;ɦ.Clear();while(IGC.UnicastListener.HasPendingMessage){var ċ
=IGC.UnicastListener.AcceptMessage();if(ċ.Tag=="muzzle-pos"){ό=((Vector3D)ċ.Data-Á.GetPosition());}else{ɦ.Add(ċ);}}try{У.
ɧ(ɦ);}catch(Exception ex){ś.œ(ex.ToString());throw;}if(Variables.Get<bool>("double-tap-actions")){if(ӫ.Ӕ("e")){Ւ();var Ν=
Փ.Value;р.ш.Ӝ(р.ш.ҳ.FirstOrDefault(õ=>õ.қ==Ν));ś.œ($"Model change from E double tap: {Ν.Հ}");if(Ν.Շ==BallisticModelKind.
InertialGrid){IGC.SendBroadcastMessage("rdc.muzzle-pos.request","",TransmissionDistance.CurrentConstruct);}}if(ӫ.Ӕ("q")){Đ.ʒ();}}if(
Т.О.ж("mouse-shifting")){var S=ӫ.Ӆ();ϋ+=S;}Vector3D ǀ=new Vector3D();CurrentPovType=PovType.Unknown;Ԅ(ӫ.ӧ(),ref ǀ,ref
CurrentPovType);var Μ=Փ?.Value.Շ;var Λ=false;if((CurrentPovType==PovType.Unknown)||(Á==null)){ś.Ŕ(
"Idle mode. Get in the cockpit, double tap E or fire configured weapon.");}else{bool Κ=Т.О.ж("pip");var Ι=Á.GetShipVelocities();var Θ=Ι.LinearVelocity-ӳ;ӳ=Ι.LinearVelocity;Vector3D Η;Á.
TryGetPlanetPosition(out Η);Vector3D?Ζ=null;if(Μ==BallisticModelKind.InertialGrid)Ζ=ό.HasValue?ό.Value+Á.GetPosition():Á.GetPosition();else
if(Μ==BallisticModelKind.FreeFall)Ζ=Ǻ.Ȼ();else if(р.ш.ҵ)Ζ=р.ш.Ȼ();Vector3D Ε;var ƍ=Vector3D.Zero;Vector3D?Δ=null;Vector3D?
ƌ=null;var Γ=0f;ʹ.Clear();var Β=Ɨ.ƭ(ś.G);if(Β!=null){Λ=true;Ε=Β.Ǿ.Value;ƍ=Β.M??Vector3D.Zero;ʹ.Add(new Ȁ{ā=Me.EntityId,ǿ=
"hd-selected",Ǿ=ϱ(Á,Ε),M=ƍ});}else{Γ=Փ?.Value.Կ??Variables.Get<float>("zero-range");double Α;var ΐ=Ζ??Á.GetPosition();if(Á.
TryGetPlanetElevation(MyPlanetElevation.Surface,out Α)&&Α<Γ){var Π=new BoundingSphereD(Η,(Η-ΐ).Length()-Α);var Ƃ=Π.Intersects(new RayD(ΐ,Á.
WorldMatrix.Forward));if(Ƃ.HasValue)Γ=Math.Min(Γ,(float)Ƃ.Value);}Ε=Á.WorldMatrix.Forward*Γ+ΐ;if(Т.О.ж("angular-eval"))ƍ=Ӱ(Γ,Ι.
AngularVelocity,Á.WorldMatrix);var γ=ƍ.Length();if(γ>SP_LIMIT)ƍ=Vector3D.Normalize(ƍ)*SP_LIMIT;Κ=true;}Vector3D?δ=ϱ(Á,Ε);Vector3D?β=
null;MyTuple<Vector3D,Vector3D,Vector3D>α=new MyTuple<Vector3D,Vector3D,Vector3D>();var ΰ=Т.О.ж("fire");Լ(Ι.LinearVelocity,Ε
);Vector3D?ί=null;Vector3D?ή=null;if(Ζ.HasValue&&Μ==BallisticModelKind.FreeFall){int έ=0;ί=Ԝ(Ξ,Λ,Ε,Η,Á,Ζ.Value,ref έ);if(
Λ&&ƍ.LengthSquared()>double.Epsilon&&έ>0)ή=Ε+ƍ*έ*ś.ŗ;}Ǻ.Ǭ(ί);MyTuple<double,double>?ά=null;string Ϋ="";if(Ζ.HasValue){var
Ϊ=Փ.Value;Vector3D Ω=Á.WorldMatrix.Forward*(Ε-Ζ.Value).Length()+Ζ.Value;var Ψ=new RayD(Ζ.Value,Á.WorldMatrix.Forward);if(
!Λ)Ε=Ω;if(ΰ&&Λ&&ί.HasValue){ά=Ǻ.Ǳ(Ζ.Value,ί.Value,ή??Ε);}double ń;bool Χ=Յ(Ϊ,Ψ,Ε,ƍ,Ι.LinearVelocity,Ξ,Η,ref ƌ,ref Δ,out ń
);if(Λ){if(ΰ&&Χ&&р.ш.ҵ){if(Δ.HasValue){var ª=Δ.Value-Ζ.Value;var Ó=ª.Length();р.ш.ӡ(ª/Ó,Ó,null);}else{var ª=ƌ.Value-Ζ.
Value;var Ó=ª.Length();р.ш.ӡ(Vector3D.Normalize(Ε-Ζ.Value),Ó,ª/Ó);}ά=new MyTuple<double,double>(р.ш.ҁ,р.ш.Ҋ);}if(Т.О.ж("aim")
&&(ծ!=null)){if(Χ){if(Δ.HasValue){var Φ=Vector3D.Normalize(Δ.Value-Ζ.Value);var Υ=Т.О.ж("360-aim")?-1:0.95;if(Vector3D.Dot
(Φ,Á.WorldMatrix.Forward)>Υ){ծ.զ();var Τ=Á.WorldMatrix;Τ.Translation=Ζ.Value;ծ.բ(Φ,Á.WorldMatrix.Up,Á.WorldMatrix,Á,true)
;}}else if(ƌ.HasValue){ծ.զ();var ª=Vector3D.Normalize(Ε-Ζ.Value);var Σ=MatrixD.CreateFromDir(Vector3D.Normalize(ƌ.Value-Ζ
.Value),Á.WorldMatrix.Up);if(!Σ.IsValid())Σ=Á.WorldMatrix;Σ.Translation=Ζ.Value;ծ.բ(ª,Á.WorldMatrix.Up,Σ,Á,true);}}else
if(ί.HasValue){var Ρ=Ξ.Normalized();var ϓ=Vector3D.Reject(ή??Ε-Ζ.Value,Á.WorldMatrix.Up);if(ϓ.LengthSquared()>100){var ł=ϓ
.Normalized();if(Vector3D.Dot(ł,Á.WorldMatrix.Forward)>0.8f){ծ.զ();ծ.բ(ł,-Ρ,Á.WorldMatrix,Á,false);}}}}}if(Κ){if(ƌ.
HasValue){β=ƌ.Value;}else if(Δ.HasValue){β=ԟ(Δ.Value,Ε,Ψ);}if(!Λ&&β.HasValue){α.Item1=β.Value;var ϙ=Ω;double Ѕ;var Ѓ=1/3f;ϙ=Á.
WorldMatrix.Forward*(Ε-Ζ.Value).Length()*(1-Ѓ)+Ζ.Value;var Ђ=ƍ*(1-Ѓ);Յ(Ϊ,Ψ,ϙ,Ђ,Ι.LinearVelocity,Ξ,Η,ref ƌ,ref Δ,out Ѕ);α.Item2=ƌ??ԟ
(Δ.Value,ϙ,Ψ);Ѓ=2/3f;ϙ=Á.WorldMatrix.Forward*(Ε-Ζ.Value).Length()*(1-Ѓ)+Ζ.Value;var Ё=ƍ*(1-Ѓ);Յ(Ϊ,Ψ,ϙ,Ё,Ι.LinearVelocity,
Ξ,Η,ref ƌ,ref Δ,out Ѕ);α.Item3=ƌ??ԟ(Δ.Value,ϙ,Ψ);}}else if(Δ.HasValue){β=Δ.Value;}}switch(CurrentIndicationType){case
IndicationType.Projector:if(р.н!=null&&β.HasValue){ҥ(β.Value,ǀ);}break;case IndicationType.SpriteApi:if(β.HasValue&&Ы?.Enabled==true&&
IGC.IsEndpointReachable(Ы.EntityId)){ϭ(Λ,β.Value,ǀ);}else{var Ѐ=Vector2.One*35f;Color ɇ,Ͽ;if(Т.О.ж("bright")){ɇ=
HUD_COL_BRIGHT.Item1;Ͽ=HUD_COL_BRIGHT.Item2;}else{ɇ=HUD_COL_DEFAULT.Item1;Ͽ=HUD_COL_DEFAULT.Item2;}IMyTextPanel Ͼ=null;foreach(var đ
in ϊ.Keys){Vector2 U;var Ͻ=Variables.Get<float>("funnel-base");Vector2 ϼ;if(Ӿ(Á.WorldMatrix.Translation+Á.WorldMatrix.
Forward*100f,đ,ǀ,out ϼ))Ͼ=đ;var Ɉ=ϊ[đ];if(Ζ.HasValue){if(Λ){if(Ӿ(Ε,đ,ǀ,out U)){var ȧ=Գ.Բ(ɇ,U);Ɉ.Add(ȧ.Item1);Ɉ.Add(ȧ.Item2);}if
(δ!=null){if(Ӿ(δ.Value,đ,ǀ,out U)){var ȧ=Գ.Բ(Color.OrangeRed,U);Ɉ.Add(ȧ.Item1);Ɉ.Add(ȧ.Item2);}}if(Κ){if(β.HasValue&&Ӿ(β.
Value,đ,ǀ,out U)){var ϻ=(Ի&&ծ?.է==true)?Color.Red:ɇ;MySprite ϸ=new MySprite(SpriteType.TEXTURE,"CircleHollow",size:Ѐ*0.5f,
color:ϻ);ϸ.Position=U;Ɉ.Add(ϸ);var Ó=(float)(Ε-ǀ).Length();Ȧ(1-Ó/600f,U,25f,3f,Ͽ,10,Ɉ);var É=MySprite.CreateText(
$"       {(Ε-đ.GetPosition()).Length():f1}"+$"\n       {ƍ.Length():f1} m/s","Debug",Ͽ,0.5f);U.X=đ.TextureSize.X*0.7f;É.Position=U;Ɉ.Add(É);if(!Β.ː.HasValue&&
Variables.Get<bool>("cast-bb-convex")){var ƿ=ǁ(Β.ː.Value,ǀ,đ);for(int Ʒ=0;Ʒ<ƿ.Count-1;Ʒ++){MySprite Ϻ;Ԓ(ƿ[Ʒ],ƿ[Ʒ+1],Ͽ,2f,out Ϻ);Ɉ
.Add(Ϻ);}MySprite Ϲ;Ԓ(ƿ.Last(),ƿ.First(),Ͽ,2f,out Ϲ);Ɉ.Add(Ϲ);}}}else{if(Ӿ(Ε,đ,ǀ,out U)){MySprite ϸ=new MySprite(
SpriteType.TEXTURE,"Cross",size:new Vector2(10f,10f),color:new Color(1f));ϸ.Position=U;Ɉ.Add(ϸ);}if(β.HasValue&&Ӿ(β.Value,đ,ǀ,out
U)){MySprite Є=MySprite.CreateText("x","Debug",new Color(0,200,0),0.3f);Є.Position=U;Ɉ.Add(Є);}else{if(đ==Ͼ&&β.HasValue){
Ӿ(β.Value,đ,ǀ,out U,true);var Ϸ=U-đ.TextureSize/2f;var Ʒ=Vector2.Normalize(Ϸ);var Ŝ=new Vector2(Ʒ.X,-Ʒ.Y);var ż=new
Vector2(0,1);float W=(float)(Math.Atan2(ż.Y,ż.X)-Math.Atan2(Ŝ.Y,Ŝ.X));MySprite И=new MySprite(SpriteType.TEXTURE,"Triangle",
size:new Vector2(15f,23f)*ύ,color:ɇ);И.RotationOrScale=W;var º=Math.Min(đ.TextureSize.X/2.2f,Math.Abs(Ϸ.X));var Ő=Math.Min(đ
.TextureSize.Y/2.2f,Math.Abs(Ϸ.Y));И.Position=đ.TextureSize/2f+new Vector2(Math.Sign(Ʒ.X)*º,Math.Sign(Ʒ.Y)*Ő);Ɉ.Add(И);}}
}}else{if(đ==Ͼ){var З=new MySprite(SpriteType.TEXTURE,"CircleHollow",size:Vector2.One*10f,color:ɇ);З.Position=ϼ;Ɉ.Add(З);
}MySprite Ж,Е;var ǧ=(Ζ.Value-Ε).Length();switch(Μ){case BallisticModelKind.Ballistic:case BallisticModelKind.Rocket:case
BallisticModelKind.BallisticCustom:{if(Т.О.ж("pip-road")){var Д=Ζ.Value+Á.WorldMatrix.Forward*600f;var Г=Vector3D.Normalize(Vector3D.Cross
(α.Item1-Д,α.Item1-Ζ.Value));var Й=Г*Ͻ/2f+α.Item1;var В=-Г*Ͻ/2f+α.Item1;var Б=Vector3D.Normalize(Vector3D.Cross(α.Item2-Д
,α.Item2-Ζ.Value));var А=Б*Ͻ/2f+α.Item2;var Џ=-Б*Ͻ/2f+α.Item2;var Ў=Vector3D.Normalize(Vector3D.Cross(α.Item3-Д,α.Item3-Ζ
.Value));var Ѝ=Ў*Ͻ/2f+α.Item3;var Ќ=-Ў*Ͻ/2f+α.Item3;Vector2 Ћ,Њ,Љ,Ј,Ї,І;Ӿ(Й,đ,ǀ,out Ћ,true);Ӿ(В,đ,ǀ,out Њ,true);Ӿ(А,đ,ǀ,
out Љ,true);Ӿ(Џ,đ,ǀ,out Ј,true);Ӿ(Ѝ,đ,ǀ,out Ї,true);Ӿ(Ќ,đ,ǀ,out І,true);MySprite ȶ;Ԓ(Ћ,Љ,ɇ,2f,out ȶ);Ɉ.Add(ȶ);Ԓ(Њ,Ј,ɇ,2f,
out ȶ);Ɉ.Add(ȶ);Ԓ(Љ,Ї,ɇ,2f,out ȶ);Ɉ.Add(ȶ);Ԓ(Ј,І,ɇ,2f,out ȶ);Ɉ.Add(ȶ);Е=MySprite.CreateText($"       {Γ:f0}","Debug",Ͽ,0.5f
*ύ);Е.Position=Ћ;Ɉ.Add(Е);}break;}case BallisticModelKind.InertialGrid:{if(Ӿ(α.Item3,đ,ǀ,out U)){Ж=new MySprite(
SpriteType.TEXTURE,"CircleHollow",size:Ѐ,color:ɇ);Ж.Position=U;Е=MySprite.CreateText($"       {ǧ/3f:f1}","Debug",Ͽ,0.5f*ύ);U.X=đ.
TextureSize.X*0.7f;Е.Position=U;Ɉ.Add(Ж);Ɉ.Add(Е);}if(Ӿ(α.Item2,đ,ǀ,out U)){Ж=new MySprite(SpriteType.TEXTURE,"CircleHollow",size:Ѐ
*0.66f,color:ɇ);Ж.Position=U;Е=MySprite.CreateText($"       {ǧ*2f/3f:f1}","Debug",Ͽ,0.5f*ύ);U.X=đ.TextureSize.X*0.7f;Е.
Position=U;Ɉ.Add(Ж);Ɉ.Add(Е);}if(Ӿ(α.Item1,đ,ǀ,out U)){Ж=new MySprite(SpriteType.TEXTURE,"CircleHollow",size:Ѐ*0.33f,color:ɇ);Ж.
Position=U;Е=MySprite.CreateText($"       {ǧ:f1}","Debug",Ͽ,0.5f*ύ);U.X=đ.TextureSize.X*0.7f;Е.Position=U;Ɉ.Add(Ж);Ɉ.Add(Е);}
break;}}}if(ί.HasValue){if(Ӿ(ί.Value,đ,ǀ,out U)){var Ɲ=new MySprite(SpriteType.TEXTURE,"CircleHollow",size:Vector2.One*35,
color:ɇ);Ɲ.Position=U;Ɉ.Add(Ɲ);}Ӿ(ί.Value,đ,ǀ,out U,true);Ӿ(ǀ+Á.WorldMatrix.Forward*100f,đ,ǀ,out ϼ,true);MySprite ȶ;Ԓ(ϼ,U,ɇ,
1f,out ȶ);Ɉ.Add(ȶ);if(ή.HasValue&&Ӿ(ή.Value,đ,ǀ,out U)){var ȧ=Գ.Բ(Color.OrangeRed,U);Ɉ.Add(ȧ.Item1);Ɉ.Add(ȧ.Item2);}}}
foreach(var ϩ in К.ώ()){Vector2 Ϩ,ϧ;if(Ӿ(ϩ.Item1,đ,ǀ,out Ϩ)&&Ӿ(ϩ.Item2,đ,ǀ,out ϧ)){MySprite Ϧ;Ԓ(Ϩ,ϧ,TRACER_COL.Item1,2f,out Ϧ);
Ɉ.Add(Ϧ);Ϧ.Color=TRACER_COL.Item2;Ϧ.Position=Ϧ.Position.Value+Vector2.One;Ϧ.Size=Ϧ.Size+Vector2.One*2;Ɉ.Add(Ϧ);}}if(Ч!=
null){var ϥ=Vector3D.Zero;var Ϥ=Vector3D.Zero;if(Ч.ì(Á,ref ϥ,ref Ϥ)){Vector2 ϣ,Ϣ;if(Ӿ(ϥ,đ,ǀ,out ϣ)&&Ӿ(Ϥ,đ,ǀ,out Ϣ)){var ϡ=
new MySprite(SpriteType.TEXTURE,"CircleHollow",size:(ϣ-Ϣ).Length()*Vector2.One,color:ɇ);ϡ.Position=ϣ;Ɉ.Add(ϡ);}}if(Ч.Õ.
Count>0){foreach(var Ϡ in Ч.Õ){Vector2 Ϟ;if(Ӿ(Ϡ,đ,ǀ,out Ϟ)){var Ǯ=new MySprite(SpriteType.TEXTURE,"SquareSimple",size:Vector2
.One*3f,color:new Color(0,60,20,60));Ǯ.Position=Ϟ;Ɉ.Add(Ǯ);}}}foreach(var ϟ in Ч.Ñ.Where(º=>º.Value.J)){Vector2 Ϟ;if(Ӿ(ϟ.
Value.N,đ,ǀ,out Ϟ)){var Ǯ=new MySprite(SpriteType.TEXTURE,"SquareHollow",size:Vector2.One*25f,color:new Color(100,60,20,60));
Ǯ.Position=Ϟ;Ɉ.Add(Ǯ);}}}if(Β?.ˑ!=null){foreach(var ʈ in Đ.ʉ(Β.ȁ,Β.ˑ.Value,Β.Ǿ.Value)){Vector2 ϝ;if(Ӿ(ʈ,đ,ǀ,out ϝ)){var Ϝ
=new MySprite(SpriteType.TEXTURE,"SquareHollow",size:Vector2.One*7f,color:Color.Red,rotation:ś.G/100f);Ϝ.Position=ϝ;Ɉ.Add
(Ϝ);}}}foreach(var ƞ in Ǻ.ɋ()){if(Ӿ(ƞ,đ,ǀ,out U)){var Ǯ=new MySprite(SpriteType.TEXTURE,"Circle",size:Vector2.One*5f,
color:Color.Green);Ǯ.Position=U;Ɉ.Add(Ǯ);}}if(đ==Ͼ){var ϛ=MySprite.CreateText(
$"FFz: {Variables.Get<float>("default-zero-range"):f0}","Debug",Ͽ,0.6f*ύ,TextAlignment.LEFT);ϛ.Position=new Vector2(đ.TextureSize.X*0.15f,đ.TextureSize.Y*0.15f);if(Λ){ϛ.Data+=
$"\nt-offsets: {Đ.ʎ(Β.ȁ)?.Count}";ϛ.Data+=$"\nT-ext: {Β.ː?.Extents.Length():f1}";}else ϛ.Data+=$"\nT-base: {Ͻ:f1}";Ɉ.Add(ϛ);float Ϫ=0.5f;float Ϛ=Ϫ;if(Т.О
.ж("aim")){ϛ.Data=$"CTRL TKVR";ϛ.Color=(Ի&&ծ?.է==true)?Color.Red:Ͽ;ϛ.Position=new Vector2(đ.TextureSize.X*0.15f,đ.
TextureSize.Y*Ϛ);Ɉ.Add(ϛ);}ϛ.Data=$"{Փ?.Value.Հ??"None"}";if(р.ш.Ҷ)ϛ.Data="*"+ϛ.Data;ϛ.Position=new Vector2(đ.TextureSize.X*0.15f,đ
.TextureSize.Y*(Ϛ+0.05f));ϛ.Color=Ͽ;Ɉ.Add(ϛ);if(ΰ){ϛ.Data=$"AUTO TRIG";if(ά!=null){var Ȓ=ά.Value.Item1;ϛ.Data+=
$": {(Ȓ>0?Ȓ.ToString("f1"):"X")} ({ά.Value.Item2}m)";}ϛ.Color=Ͽ;ϛ.Position=new Vector2(đ.TextureSize.X*0.15f,đ.TextureSize.Y*(Ϛ+0.1f));Ɉ.Add(ϛ);}if(!Ζ.HasValue){ϛ.Data=(Փ==
null)?"No weapon selected.\nDouble tap E or fire configured weapon.":$"No weapons found for type '{Փ.Value.Հ}'";ϛ.Color=Ի?
Color.Red:Ͽ;ϛ.Position=new Vector2(đ.TextureSize.X*0.15f,đ.TextureSize.Y*(Ϛ+0.15f));Ɉ.Add(ϛ);}else if(ί.HasValue)Ǻ.ɉ(Ɉ,ɇ,đ.
TextureSize.X*0.2f,new Vector2(đ.TextureSize.X*(0.5f-0.1f),đ.TextureSize.Y*0.15f),Á);if(Ǻ.ǜ>0){Ȧ((float)Ǻ.ǜ,ϼ,20,5,ɇ,15,Ɉ);}if(Т.О.
ж("damage-scan")){ϛ.Data=р.ю;ϛ.Position=new Vector2(đ.TextureSize.X*0.75f,đ.TextureSize.Y*0.2f);ϛ.RotationOrScale=0.5f*ύ;
ϛ.Color=Ͽ;Ɉ.Add(ϛ);Ȯ(new Vector2(đ.TextureSize.X*0.75f+50,đ.TextureSize.Y*0.15f),new Vector2(100,16),ɇ,Ͽ,р.э,Ɉ);}if(ś.G<
500&&!Variables.Get<bool>("skip-intro")){Ɉ.AddRange(ǔ.ǅ.Select(º=>Ɵ(ǔ.ǃ,ś.G,ref º)));if(ǔ.ǖ.Count>0)Ɉ.AddRange(ǔ.ǖ[(ś.G/6)%
ǔ.ǖ.Count]);}ϛ.Data=$"Tsrc: {Ɨ.Ɣ?.ļ}";if(Ч!=null){ϛ.Data+=Ч.Å();}ϛ.Position=new Vector2(đ.TextureSize.X*0.15f,đ.
TextureSize.Y*(Ϛ+0.35f));ϛ.RotationOrScale=0.5f*ύ;ϛ.Color=Ͽ;Ɉ.Add(ϛ);ϛ.Data=$"{Θ.Length()*60:f1} m/s^2";ϛ.Position=new Vector2(đ.
TextureSize.X*0.75f,đ.TextureSize.Y*(Ϛ+0.35f));Ɉ.Add(ϛ);}}foreach(var ʽ in Ɨ.Ɠ){Vector2 U;bool Y=false;foreach(var đ in ϊ.Keys){if(
Ӿ(ʽ.Ā,đ,ǀ,out U)){var ϵ=new MySprite(SpriteType.TEXTURE,"SquareHollow",size:new Vector2(40f,40f),color:ɇ);if(Β?.ȁ==ʽ.ā){ϵ
.Color=Color.Goldenrod;if(Β.ː.HasValue){Vector2 S;Ӿ(ʽ.Ā+Á.WorldMatrix.Right*Β.ː.Value.Extents.AbsMax(),đ,ǀ,out S,true);ϵ.
Size=(U-S).X*Vector2.One;}}ϵ.Position=U;ϊ[đ].Add(ϵ);Y=true;break;}}if(!Y&&Ͼ!=null){Ӿ(ʽ.Ā,Ͼ,ǀ,out U,true);if(U!=Vector2.Zero)
{var º=Ͼ.TextureSize/2;var ϴ=Vector2.Normalize(U-º);var S=Ͼ.TextureSize.Y/2;MySprite ȶ;Ԓ(º+ϴ*S*0.8f,º+ϴ*S*0.7f,ɇ,1,out ȶ)
;ϊ[Ͼ].Add(ȶ);}}}þ.ė(ɦ);foreach(var ϳ in ϊ){{using(var Ɉ=ϳ.Key.DrawFrame()){Ɉ.AddRange(ϳ.Value);ϳ.Value.Clear();}ϳ.Key.
ContentType=ContentType.TEXT_AND_IMAGE;ϳ.Key.ContentType=ContentType.SCRIPT;}}}break;}if(ś.G%60==0){Ի=!Ի;}if((Ы!=null)&&IGC.
IsEndpointReachable(Ы.EntityId)&&(Ы.Enabled)){Echo("Delegating output to "+Ы.EntityId);IGC.SendUnicastMessage(Ы.EntityId,"draw-text",new
MyTuple<string,Vector2,float>(Փ?.Value.Հ+(Т.О.ж("fire")?" AFR\n":"\n")+CurrentIndicationType+(Κ?" PIP":" LEAD"),new Vector2(0f,
1.3f/4f),0.3f*2));if(Ի){IGC.SendUnicastMessage(Ы.EntityId,"draw-text",new MyTuple<string,Vector2,float>("LOCK",new Vector2(
0f,1/4f),0.6f*2));}}if((Á!=null)&&CockpitScreenIndex.HasValue)ԉ(Á,CockpitScreenIndex.Value,
$"WType: {Փ?.Value.Հ}\nVel:{ƍ.Length():f2}"+$"\nIType: {CurrentIndicationType}\n{(Κ?"pip":"lead")}\nbright:{Т.О.ж("bright")}\nautofire:{Т.О.ж("fire")}");}ͳ.Ǭ(ɦ);ծ?
.ε();р.ш.ε();if(Т.О.ж("damage-scan"))р.ε(ś.G);if(Variables.Get<int>("tracers-clock")>0)К.ε(Á?.GetNaturalGravity()??
Vector3D.Zero,Λ);и();й=false;}catch(Exception ex){Runtime.UpdateFrequency=UpdateFrequency.None;ś.Ŕ(
"Script was stopped because of critical error\nContact me to get this fixed\nSee PB screen for details");var đ=Me.GetSurface(0);if(đ!=null){đ.WriteText(ex.ToString(),true);đ.ContentType=ContentType.TEXT_AND_IMAGE;}throw;}ϯ.
Enqueue(Runtime.LastRunTimeMs);if(ϯ.Count==100){double ϲ=0;foreach(var º in ϯ)ϲ+=º;Ϯ=ϲ/100f;Echo("100 runs avg "+Ϯ.ToString(
"f3")+" ms");ϯ.Dequeue();}Echo($"LastRunTimeMs: {Runtime.LastRunTimeMs:f3}");Echo(
$"CurrentInstructionCount: {Runtime.CurrentInstructionCount}");}Vector3D ϱ(IMyShipController Á,Vector3D Ε){if(Á==null||ϋ==Vector2.Zero)return Ε;var ϰ=Á.WorldMatrix;var ơ=(ϰ.
Translation-Ε).Length()/2000;return Ε+Á.WorldMatrix.Right*ϋ.Y*ơ+Á.WorldMatrix.Down*ϋ.X*ơ;}Queue<double>ϯ=new Queue<double>();double
Ϯ;void ϭ(bool Λ,Vector3D β,Vector3D ǀ){if(Λ){}IGC.SendUnicastMessage(Ы.EntityId,"draw-projection",new MyTuple<string,
Vector2,Vector3D,Vector3D,float,string>("Cross",new Vector2(10f,10f)*(р.ш.Ҷ?2.5f:1),β,ǀ,1f,""));}void ҥ(Vector3D β,Vector3D ǀ){
var Ԥ=Variables.Get<int>("projected-grid-forward-offset");var ԣ=new RayD(β,Vector3D.Normalize(ǀ-β));var Ԣ=new
BoundingSphereD(р.н.WorldMatrix.Translation,р.н.CubeGrid.GridSize*(50+Ԥ));var ԡ=ԣ.Intersects(Ԣ);if(ԡ!=null){var U=ԣ.Position+Vector3D.
Normalize(ԣ.Direction)*ԡ.Value;var Ԡ=Vector3D.TransformNormal(U-р.н.WorldMatrix.Translation,MatrixD.Transpose(р.н.WorldMatrix));
var ȿ=new Vector3I(-Ԡ/р.н.CubeGrid.GridSize);р.н.Enabled=true;р.н.ProjectionOffset=ȿ;р.н.UpdateOffsetAndRotation();}else{р.
н.Enabled=false;р.н.ProjectionOffset=Vector3I.Zero;}}Vector3D ԟ(Vector3D Δ,Vector3D Ε,RayD Ԟ){var ũ=Δ-Ԟ.Position;var Ó=ũ.
Length();var ԝ=Ԟ.Direction*Ó+Ԟ.Position;return ԝ-(-Ε+Δ);}Vector3D?Ԝ(Vector3D Ξ,bool Λ,Vector3D Ε,Vector3D Η,IMyShipController
Á,Vector3D ň,ref int ϐ){if(!Ξ.IsZero()){double Α;if(Λ){Α=(Ε-Η).Length();}else{Á.TryGetPlanetElevation(MyPlanetElevation.
Surface,out Α);Α=(ň-Η).Length()-Α;}var ſ=Á.GetShipVelocities().LinearVelocity;var ԥ=К.ϕ(ň,Ξ,Η,ſ,Α,SP_LIMIT,ref ϐ);return ԥ;}
return null;}void Լ(Vector3D Ժ,Vector3D Ε){var Թ=Variables.Get<int>("tracers-clock");var Ը=р.ш.ҍ()?.қ.Շ;if(Թ>0&&р.ш.Ҷ&&(Ը==
BallisticModelKind.Ballistic||Ը==BallisticModelKind.BallisticCustom)){if(ś.G%Թ==0){var Ѻ=р.ш.ҍ();if(Ѻ!=null){var є=Ѻ.ҝ;var Ѱ=Ѻ.қ;Vector3D
ƞ;Vector3D ν;if(є.Count>0){var Է=є[(ś.G/Թ)%є.Count];ƞ=Է.GetPosition();ν=Է.WorldMatrix.Forward;}else{ƞ=р.ш.Ȼ();ν=р.ш.Ҍ();}
var Զ=Ѱ.Կ/Ѱ.Ծ;К.ξ(ƞ,ν,Ѱ.Ծ,Ժ,()=>Ε,Ѱ.Կ*1.3f,Զ*2f);}}}}bool Ի;void Ե(){р.о.ForEach(U=>{U.WriteText("");U.DrawFrame().Dispose(
);});}class Գ{public static MyTuple<MySprite,MySprite>Բ(Color ɇ,Vector2 U){var Ǉ=new MySprite(SpriteType.TEXTURE,
"AH_BoreSight",size:Vector2.One*15f,color:ɇ,rotation:(float)Math.PI);Ǉ.Position=U+Vector2.UnitX*5f;var Ա=Ǉ;Ա.RotationOrScale=0;Ա.
Position=U-Vector2.UnitX*5f;return new MyTuple<MySprite,MySprite>(Ǉ,Ա);}}List<MySprite>ԧ=new List<MySprite>();List<MySprite>Ԧ(){
ԧ.Clear();Vector2 ϼ;var Á=ӫ.Ө();if(Á!=null){var ǀ=Á.GetPosition()+ӫ.ɔ(Á);Vector3D ԛ=ϊ.First().Key.WorldMatrix.Translation
;foreach(var U in ϊ){if(Ӿ(Á.WorldMatrix.Translation+Á.WorldMatrix.Forward*100f,U.Key,ǀ,out ϼ)){ԛ=U.Key.WorldMatrix.
Translation;}}foreach(var U in ϊ){bool Ԇ=U.Key.CubeGrid.GridSizeEnum==MyCubeSize.Large;var ɀ=Vector2.One*U.Key.TextureSize.X/(Ԇ?
2.5f*LCD_TEXTURE_SCALE_L:0.5f*LCD_TEXTURE_SCALE_S);var ȿ=(U.Key.WorldMatrix.Translation-ԛ)*ɀ.X;var ԅ=Vector3D.
TransformNormal(ȿ,MatrixD.Transpose(U.Key.WorldMatrix));ԧ.AddRange(U.Value.Select(º=>{var Ɲ=º;Ɲ.Position=new Vector2((float)(Ɲ.Position
.Value.X+ԅ.X),(float)(Ɲ.Position.Value.Y+ԅ.Y));return Ɲ;}));U.Value.Clear();}return ԧ;}return null;}void Ԅ(
IMyShipController Á,ref Vector3D ǀ,ref PovType ԃ){ǀ=new Vector3D();CurrentPovType=PovType.Unknown;var Ԃ=р.м.FirstOrDefault(X=>X.IsActive)
;if(Ԃ!=null){ԃ=PovType.CameraView;var ԁ=Ԃ.WorldMatrix;ǀ=ԁ.Translation+ԁ.Forward*0.2f;}else if(Á!=null){ǀ=Á.GetPosition()+
ӫ.ɔ(Á);ԃ=PovType.LargeControlSeat;}}void Ԁ(){var ӿ=((byte)CurrentIndicationType)+1;if(ӿ>1)ӿ=0;CurrentIndicationType=(
IndicationType)ӿ;if(CurrentIndicationType==IndicationType.Projector)Ե();else if(р.н!=null){р.н.ProjectionOffset=Vector3I.Zero;р.н.
Enabled=false;}}bool Ӿ(Vector3D ӽ,IMyTextPanel Ӽ,Vector3D ǀ,out Vector2 Ƭ,bool ӻ=false){var ƞ=ӽ;var Ӻ=ǀ;var ԇ=Ӽ.WorldMatrix;
bool Ԇ=Ӽ.CubeGrid.GridSizeEnum==MyCubeSize.Large;var ɀ=Vector2.One*Ӽ.TextureSize.X/(Ԇ?2.5f*LCD_TEXTURE_SCALE_L:0.5f*
LCD_TEXTURE_SCALE_S);var Ԛ=ԇ.Translation+ԇ.Forward*(Ԇ?2.386f:0.463f)/2f;var ԙ=new PlaneD(Ԛ,ԇ.Forward);var Ԙ=new RayD(ƞ,Vector3D.Normalize(Ӻ
-ƞ));double?Ƃ;Ԙ.Intersects(ref ԙ,out Ƃ);if(Ƃ.HasValue&&(Vector3D.Dot(Ԙ.Direction,-ԙ.Normal)>0)){float ԗ=Ӽ.TextureSize.X/
2f;float Ԗ=Ӽ.TextureSize.Y/2f;var ԕ=Ԙ.Position+Vector3D.Normalize(Ԙ.Direction)*Ƃ.Value;var Ԕ=Vector3D.TransformNormal(ԕ-ԇ.
Translation,MatrixD.Transpose(ԇ));Vector2 Á=new Vector2((float)Ԕ.X,(float)Ԕ.Y);Á*=ɀ;var ԓ=new RectangleF(Vector2.Zero,Ӽ.TextureSize
);var ʌ=new Vector2(ԗ+Á.X,Ԗ-Á.Y);if(ӻ||ԓ.Contains(ʌ)){Ƭ=ʌ;return true;}}Ƭ=Vector2.Zero;return false;}static void Ԓ(
Vector2 ԑ,Vector2 Ԑ,Color ԏ,float Ԏ,out MySprite ȶ){var ԍ=ԑ;var Ԍ=Ԑ;var ԋ=(Ԍ-ԍ);var Ԋ=new Vector2(0,1);float W=(float)(Math.
Atan2(Ԋ.Y,Ԋ.X)-Math.Atan2(ԋ.Y,ԋ.X));ȶ=new MySprite(SpriteType.TEXTURE,"SquareSimple",size:new Vector2(Ԏ,ԋ.Length()),color:ԏ);
ȶ.Position=ԍ+ԋ/2;ȶ.RotationOrScale=-W;}void ԉ(IMyShipController Á,int Դ,string Ԉ){var Խ=Á as IMyCockpit;IMyTextSurface Ӽ=
null;if(Խ!=null){if(Խ.SurfaceCount>Դ){Ӽ=Խ.GetSurface(Դ);}}if(Ӽ!=null){Ӽ.ContentType=ContentType.SCRIPT;Ӽ.Script="";using(var
Ɉ=Ӽ.DrawFrame()){float ԗ=Ӽ.TextureSize.X/2f;MySprite կ=MySprite.CreateText(Ԉ,"Debug",new Color(1f),0.6f,TextAlignment.
LEFT);կ.Position=new Vector2(Ӽ.TextureSize.X/4f,Ӽ.TextureSize.Y/4f);Ɉ.Add(կ);}}}խ ծ;class խ{Vector3D ѿ;IMyGyro լ;Ӫ ի;
IMySoundBlock ժ;public խ(IMyGyro թ,Ӫ ը,IMySoundBlock я){լ=թ;ժ=я;ի=ը;if(ժ!=null){ժ.SelectedSound="Alert 2";ժ.LoopPeriod=3600;ժ.Play();
ժ.Enabled=false;}}public bool է{get;private set;}public void զ(){է=true;}void ե(){if(!դ){լ.GyroOverride=true;դ=true;if(ժ
!=null)ժ.Enabled=true;}}bool դ=true;public void ε(){if(է)ե();else if(դ){լ.GyroOverride=false;դ=false;լ.SetValue("Pitch",0f
);լ.SetValue("Yaw",0f);լ.SetValue("Roll",0f);if(ժ!=null)ժ.Enabled=false;}է=false;}public double գ{get;private set;}public
void բ(Vector3D ª,Vector3D Ţ,MatrixD ա,IMyShipController Á,bool ՙ){var Ü=Vector3D.Zero;var Ʉ=MatrixD.Transpose(լ.WorldMatrix
);var ӭ=Vector3D.Rotate(Á.GetShipVelocities().AngularVelocity,Ʉ);var չ=Vector3D.Zero;if(ª!=Vector3D.Zero){if(ՙ)չ=Vector3D
.Rotate(ս(ª),Ʉ);ª=ª.Normalized();Ü=ű.Ũ(ª,ա,լ.WorldMatrix,Ţ,գ>0.87?1:0.2f);գ=Vector3D.Dot(ª,ա.Forward);}if(դ)ե(լ,Ü,ӭ,չ);}
Vector3D?վ;Vector3D ս(Vector3D ª){var ӟ=Vector3D.Zero;if(վ.HasValue){ӟ=Vector3D.Cross(ª,ª-վ.Value)/ª.LengthSquared()/ś.ŗ;}վ=ª;
return ӟ;}void ե(IMyGyro ռ,Vector3D ջ,Vector3D պ,Vector3D չ){var տ=ջ.Y;var ո=ջ.X;var ն=ջ.Z;var յ=60*RIPPLE_GYRO_BOOST;var Θ=
GRID_ANGULAR_ACCELERATIONS;var մ=2*Math.PI/60f;Func<double,double,double,double,double>ճ=(º,ղ,ձ,Ŝ)=>{var հ=º<0?ղ-ձ:ձ-ղ;հ=Math.Max(հ,0);var γ=Math.
Abs(º);double S;if(γ>հ*հ/(2*Ŝ))S=յ*Math.Sign(º)*Math.Max(Math.Min(γ,1),0.0002);else{S=-յ*Math.Sign(º)*Math.Max(Math.Min(γ,1
),0.0002);}return S-ձ/մ;};var շ=(float)ճ(տ,պ.Y,չ.Y,Θ.Y);var Ֆ=(float)ճ(ո,պ.X,չ.X,Θ.X);var Ս=(float)ճ(ն,պ.Z,չ.Z,Θ.Z);ռ.
SetValue("Pitch",-Ֆ);ռ.SetValue("Yaw",շ);ռ.SetValue("Roll",Ս);var Ն=ի.ӄ();if(Ն!=0)ռ.SetValue("Roll",Ն*10f);}}static bool Յ(қ Ϊ,
RayD Մ,Vector3D Ň,Vector3D ƍ,Vector3D λ,Vector3D Ξ,Vector3D Η,ref Vector3D?ƌ,ref Vector3D?Δ,out double ń){var Ζ=Մ.Position;
var ũ=Ň-Ζ;ń=0;var ņ=ƍ-λ;if(ņ.LengthSquared()<double.Epsilon&&Ξ==Vector3D.Zero){ƌ=Մ.Position+Մ.Direction*ũ.Length();Δ=Ň;
return true;}switch(Ϊ.Շ){case BallisticModelKind.BallisticCustom:case BallisticModelKind.Ballistic:Δ=ű.ŉ(Ζ,Ň,ƍ-λ,Ϊ.Ծ,ref ń);if
(!Ξ.IsZero())Δ=Δ.Value-Ξ*ń*ń/2f;return true;case BallisticModelKind.Rocket:var Ճ=Vector3D.Zero;if(ű.Ū(Ň,Մ.Direction,Մ.
Position,λ,ƍ,ref Ճ,ref ń)){Ճ-=ƍ*ń;ƌ=Ճ;return true;}break;case BallisticModelKind.InertialGrid:var ң=Variables.Get<float>(
"rdc-vel");double Ղ=ũ.Length()/ң;if((ƍ.Length()>0)||(λ.Length()>0)){Δ=ӎ(Ζ,λ,Ň,ƍ,ң,Ξ,Η);var Ձ=Vector3D.Reject(λ,ũ)*Ղ;Ձ*=0.587;Δ=Δ.
Value+Ձ;}else Δ=Ň;return true;}return false;}class қ{public string Հ;public float Կ=800f;public float Ծ;public
BallisticModelKind Շ;public қ(BallisticModelKind ѳ,float ѵ,float μ,string Ŭ){Շ=ѳ;Կ=ѵ;Ծ=μ;Հ=Ŭ;}}LinkedList<қ>Օ=new LinkedList<қ>();
LinkedListNode<қ>Ք;LinkedListNode<қ>Փ{get{return Ք;}set{if(Ք!=value&&value!=null)р.ш.Ӝ(р.ш.ҳ.FirstOrDefault(õ=>õ.қ==value.Value));Ք=
value;}}void Ւ(bool Ց=false){if(Օ.Any()){if(Փ==null)Փ=Ց?Օ.Last:Օ.First;else Փ=Ց?Փ.Previous??Օ.Last:Փ.Next??Օ.First;}}void Ր(
string Տ){var Ν=Օ.FirstOrDefault(º=>º.Հ.ToLower().Contains(Տ.ToLower()));if(Ν!=null)Փ=Օ.Find(Ν);}class Վ{public float Ծ{get;}
public float Կ{get;}public float Ҙ{get;}public string ǿ{get;}public bool Қ{get;}public IMyBlockGroup Ռ{get;}public float Ջ{get
;}public BallisticModelKind Պ{get;}public List<IMyTerminalBlock>Չ{get;}public Վ(IMyBlockGroup ү,BallisticModelKind Ո,List
<IMyTerminalBlock>ʼ){Ռ=ү;var Ƙ=ү.Name.Split('[').Select(º=>º.Trim(']')).ToArray();var ң=Ƙ.Skip(1).FirstOrDefault(º=>º.
Contains("v="));var S=Ƙ.Skip(1).FirstOrDefault(º=>º.Contains("r="));var Ң=Ƙ.Skip(1).FirstOrDefault(º=>º.Contains("d="));var ҡ=Ƙ.
Skip(1).FirstOrDefault(º=>º.Contains("fwO="));if(ң==null||S==null){var Ȓ=$"Custom group '{ү.Name}' definition failure. Expected v and r tags, e.g. 'Heavy Dakka [apck-custom][ripple][v=500][r=1000][d=5]'"
;ś.œ(Ȓ);throw new Exception(Ȓ);}if(Ң!=null)Ҙ=float.Parse(Ң.Split('=')[1]);else Ҙ=5;if(ҡ!=null)Ջ=float.Parse(ҡ.Split('=')[
1]);Կ=float.Parse(S.Split('=')[1]);Ծ=float.Parse(ң.Split('=')[1]);Պ=Ո;Қ=Ƙ.Any(º=>º=="ripple");ǿ=Ƙ[0];Չ=ʼ;}}class Ҡ{public
Vector3D ҟ;public Vector3D Ҟ;public List<IMyTerminalBlock>ҝ=new List<IMyTerminalBlock>();public string Ҝ;public қ қ;public bool
Қ;public int ҙ;public float Ҙ;public int җ;public int Җ;public Action<IMyTerminalBlock>ҕ;public Action<IMyTerminalBlock>Ҕ
;public Action<IMyTerminalBlock>Ҥ;public Action ғ;public Action Ҧ;}class ҹ{IMyEntity ҷ;public bool Ҷ{get;private set;}
public bool ҵ=>ӛ!=null;public float Ҵ;public List<Ҡ>ҳ;public ҹ(IEnumerable<IMyTerminalBlock>є,List<Վ>Ҳ,LinkedList<қ>Ѳ,
IMyEntity ұ){ҳ=new List<Ҡ>();var Ұ=ұ.WorldMatrix.Translation;var Ҹ=MatrixD.Transpose(ұ.WorldMatrix);ҷ=ұ;foreach(var ү in Ҳ){var ҭ
=ѷ(ү,Ѳ);var Ҭ=new List<IMyTerminalBlock>();ү.Ռ.GetBlocksOfType(Ҭ,º=>!(º is IMyTimerBlock)&&ү.Չ.Contains(º));ҭ.ҝ.AddRange(
Ҭ);ҭ.Қ=ү.Қ;var ҫ=new List<IMyTimerBlock>();ү.Ռ.GetBlocksOfType(ҫ,º=>ү.Չ.Contains(º));IMyTimerBlock Ҫ=null;IMyTimerBlock ҩ
=null;IMyTimerBlock Ҩ=null;foreach(var É in ҫ){var ҧ=ɐ(É);if(ҧ.Contains("ref"))Ҫ=É;if(ҧ.Contains("fire"))ҩ=É;if(ҧ.
Contains("cease"))Ҩ=É;}Ҫ=Ҫ??ҫ.FirstOrDefault();if(Ҫ!=null){ҭ.ғ=()=>{if(ҩ!=null)ҩ.Trigger();else Ҫ.Enabled=true;};ҭ.Ҧ=()=>{if(Ҩ!=
null)Ҩ.Trigger();else Ҫ.Enabled=false;};ҭ.Ҟ=Vector3D.Rotate(Ҫ.GetPosition()+Ҫ.WorldMatrix.Forward*ү.Ջ-Ұ,Ҹ);ҭ.ҟ=Vector3D.
Rotate(Ҫ.WorldMatrix.Forward,Ҹ);}ҳ.Add(ҭ);ś.œ($"Found new custom weapon group '{ү.ǿ}'	\nGun count: {Ҭ.Count}	\nRipple: "+
$"{ҭ.Қ}	\n[ref]: {Ҫ!=null}	\n[fire]: {ҩ!=null}	\n[cease]: {Ҩ!=null}");}foreach(var ґ in є){var Ѷ=ґ.BlockDefinition.TypeId+"/"+ґ.BlockDefinition.SubtypeName;if(!ҳ.Any(º=>º.Ҝ==Ѷ)){Ҡ Ѽ=null;
if(Ѷ.Contains("MyObjectBuilder_SmallGatlingGun"))Ѽ=ѷ(Ѷ,"Light ballistic",800f,800f,5,BallisticModelKind.Ballistic,Ѳ);else
if(Ѷ.Contains("SmallBlockMediumCalibreGun")){Ѽ=ѷ(Ѷ,"Heavy ballistic S",500f,1400f,2,BallisticModelKind.Ballistic,Ѳ,true);}
else if(Ѷ.Contains("LargeBlockLargeCalibreGun")){Ѽ=ѷ(Ѷ,"Heavy ballistic L",500f,2000f,2,BallisticModelKind.Ballistic,Ѳ,true)
;}else if(Ѷ.Contains("SmallRailgun")){Ѽ=ѷ(Ѷ,"Rail S",1000f,1400f,2,BallisticModelKind.Ballistic,Ѳ,true);Ѽ.ҙ=30;}else if(Ѷ
.Contains("LargeRailgun")){Ѽ=ѷ(Ѷ,"Rail L",2000f,2000f,2,BallisticModelKind.Ballistic,Ѳ,true);Ѽ.ҙ=120;}else if((Ѷ==
"MyObjectBuilder_SmallMissileLauncher/")||Ѷ.Contains("/LargeMissileLauncher")||Ѷ.Contains("/SmallRocketLauncherReload")){Ѽ=ѷ(Ѷ,"Rocket",200f,800f,5,
BallisticModelKind.Rocket,Ѳ,true);}if(Ѽ!=null){ҳ.Add(Ѽ);}}var Ѯ=ҳ.FirstOrDefault(º=>º.Ҝ==Ѷ);if(Ѯ!=null)Ѯ.ҝ.Add(ґ);else ś.œ(
$"Failed to parse weapon subtype: {Ѷ}, custom name: {ґ.CustomName}");}Vector3D ѻ;foreach(var Ѻ in ҳ){ѻ=Vector3D.Zero;if(Ѻ.ҝ.Count>0){ѯ(Ѻ);ѻ=Vector3D.Zero;Ѻ.ҝ.ForEach(º=>ѻ+=º.GetPosition()
);ѻ/=Ѻ.ҝ.Count;var ѹ=Ѻ.ҝ.OrderBy(º=>(ѻ-º.GetPosition()).Length()).First();Ѻ.Ҟ=Vector3D.Rotate(ѻ-Ұ,Ҹ);Ѻ.ҟ=Vector3D.Rotate(
ѹ.WorldMatrix.Forward,Ҹ);}if(Ѻ.Қ)Ѻ.ҝ=Ѻ.ҝ.OrderBy(º=>º.CustomName).ToList();if(Vector3D.IsZero(Ѻ.ҟ)){throw new Exception(
$"Critical error:  group '{Ѻ.Ҝ}' does not have any direction reference");}ś.œ($"Initialized group '{Ѻ.Ҝ}', guns found: {Ѻ.ҝ.Count}");}ҏ();}Ҡ ѷ(Վ Ѹ,LinkedList<қ>Ѳ){return ѷ(Ѹ.Ռ.Name,Ѹ.ǿ,Ѹ.Ծ,Ѹ.
Կ,Ѹ.Ҙ,Ѹ.Պ,Ѳ);}Ҡ ѷ(string Ѷ,string Ŭ,float μ,float ѵ,float Ѵ,BallisticModelKind ѳ,LinkedList<қ>Ѳ,bool ѱ=false){var Ѱ=Ѳ.
FirstOrDefault(º=>º.Հ==Ŭ);if(Ѱ==null){Ѱ=new қ(ѳ,ѵ,μ,Ŭ);Ѳ.AddLast(Ѱ);ś.œ($"Found new ballistic model '{Ŭ}'");}var õ=new Ҡ{Ҝ=Ѷ,қ=Ѱ,Ҙ=Ѵ,Қ
=ѱ};return õ;}void ѯ(Ҡ õ){if(õ.ҝ[0]is IMyUserControllableGun){ś.œ(
$"Setting up actions for a new vanilla weapon group '{õ.Ҝ}'");õ.ҕ=ƴ=>((IMyUserControllableGun)ƴ).Shoot=true;õ.Ҕ=ƴ=>((IMyUserControllableGun)ƴ).Shoot=false;õ.Ҥ=ƴ=>((
IMyUserControllableGun)ƴ).ShootOnce();}else{ś.œ($"Setting up actions for a new WC weapon group '{õ.Ҝ}'");õ.ҕ=ƴ=>ƴ.GetProperty("WC_Shoot").
AsBool().SetValue(ƴ,true);õ.Ҕ=ƴ=>ƴ.GetProperty("WC_Shoot").AsBool().SetValue(ƴ,false);õ.Ҥ=ƴ=>ƴ.GetProperty("WC_Shoot").AsBool(
).SetValue(ƴ,true);}}Dictionary<Vector3D,List<IMyUserControllableGun>>Ғ(List<IMyUserControllableGun>є){Dictionary<
Vector3D,List<IMyUserControllableGun>>Ƭ=new Dictionary<Vector3D,List<IMyUserControllableGun>>();foreach(var ґ in є){var Ґ=Ƭ.Keys
.FirstOrDefault(º=>Vector3D.Dot(º,ґ.WorldMatrix.Forward)>0.9);if(Ґ==Vector3D.Zero){Ƭ.Add(ґ.WorldMatrix.Forward,new List<
IMyUserControllableGun>());Ґ=ґ.WorldMatrix.Forward;}Ƭ[Ґ].Add(ґ);}return Ƭ;}public void ҏ(){Ҷ=true;Ҏ();}public void Ҏ(){if(Ҷ){foreach(var Ѻ in
ҳ){Ѻ.ҝ.ForEach(º=>Ѻ.Ҕ(º));Ѻ.Ҧ?.Invoke();}Ҷ=false;}}public Ҡ ҍ(){return ӛ;}public Vector3D Ҍ(){return Vector3D.Rotate(ӛ.ҟ,
ҷ.WorldMatrix);}public Vector3D Ȼ(){var ї=ӛ;if(ї.Қ&&Т.О.ж("coax-ripple")){var ҋ=ї.ҝ[ї.җ];if(ҋ.IsWorking)return ҋ.
WorldMatrix.Translation;}return Vector3D.TransformNormal(ї.Ҟ,ҷ.WorldMatrix)+ҷ.WorldMatrix.Translation;}public float Ҋ;public double
ҁ;public double Ҁ;double ѿ;bool Ѿ(Vector3D Ү,Vector3D ѽ,double Ó,float ѵ,float Ѵ,bool Ӟ){var ŕ=Variables.Get<float>(
"ripple-precision-override");if(ŕ>0)Ѵ=ŕ;var Ӥ=Vector3D.Dot(ѽ,Ү);Ӥ=Math.Min(Ӥ,1f);var ǭ=Math.Acos(Ӥ);var Ȓ=Math.Tan(ǭ)*Math.Max(100,Ó);Ҁ=Math.Abs(Ȓ-
ѿ);ѿ=Ȓ;Ҋ=Ѵ;ҁ=Math.Tan(ǭ)*Ó;return(Ó<ѵ)&&(ǭ<Math.PI/4f)&&(ҁ<Ѵ)&&(!Ӟ||Ҁ<0.15f);}public RayD?ӣ;public RayD?Ӣ;public void ӡ(
Vector3D ѽ,double Ó,Vector3D?Ӡ){var ї=ӛ;if(Т.О.ж("coax-ripple")&&ї.Қ){var ӟ=ї.ҝ[ї.җ];if(Ѿ(Ӡ??ӟ.WorldMatrix.Forward,ѽ,Ó,ї.қ.Կ,ї.Ҙ
,true)){if(!ӟ.IsWorking)ӥ(ї);else{if(!Ҷ){Ҷ=true;if(ї.ҙ==0)ї.ҕ(ӟ);if(ї.Җ==0)ї.Җ=ś.G;ӣ=new RayD(ӟ.GetPosition(),ѽ);Ӣ=new
RayD(ӟ.GetPosition(),Ӡ??ӟ.WorldMatrix.Forward);}ӹ=true;}}}else{var ň=Vector3D.Rotate(ї.Ҟ,ҷ.WorldMatrix)+ҷ.WorldMatrix.
Translation;var Ə=Vector3D.Rotate(ї.ҟ,ҷ.WorldMatrix);var Ӟ=ї.қ.Շ==BallisticModelKind.BallisticCustom;if(Ѿ(Ӡ??Ə,ѽ,Ó,ї.қ.Կ,ї.Ҙ,Ӟ)){if
(!Ҷ){ї.ҝ.ForEach(º=>ї.ҕ(º));ї.ғ?.Invoke();Ҷ=true;ӣ=new RayD(ň,ѽ);Ӣ=new RayD(ň,Ӡ??Ə);}ӹ=true;}}}Ҡ ӝ;public void Ӝ(Ҡ õ){ӝ=õ
;}Ҡ ӛ;public Action<Ҡ>Ӛ;void ӥ(Ҡ ї){ї.Ҕ(ї.ҝ[ї.җ]);Ҷ=false;ї.җ++;ї.Җ=0;if(ї.җ>ї.ҝ.Count-1)ї.җ=0;Ӛ?.Invoke(ї);}int?Ӧ;bool ӹ
;public void ε(){var ӷ=Variables.Get<int>("ripple-increment-interval-rail");if(ӛ?.Җ>0&&ӛ.ҙ>ӷ){if(Ӧ.HasValue){var Ӷ=ӛ.ҙ;
var ͼ=ś.G-Ӧ.Value;var ӵ=ͼ/ӷ;if(ӵ<ӛ.ҝ.Count){ӛ.Ҥ(ӛ.ҝ[ӵ]);}var Ӵ=MathHelper.Clamp((ͼ-Ӷ-5+ӷ)/ӷ,0,ӛ.ҝ.Count-1);ӛ.җ=Ӵ;if(ͼ>ӛ.ҝ.
Count*ӷ+Ӷ+10){ӛ.Җ=0;Ӧ=null;}}else{RIPPLE_GYRO_BOOST=Variables.Get<float>("ripple-gyro-boost");Ӧ=ś.G;}}else RIPPLE_GYRO_BOOST=
1;if((ӝ!=null)&&(ӝ!=ӛ)){Ӧ=null;ӛ=ӝ;ҏ();}foreach(var ї in ҳ){if(ї.ҙ<ӷ&&ї.Җ>0&&ś.G-ї.Җ>Variables.Get<int>(
"ripple-increment-interval")){ӥ(ї);}}if(!ӹ)Ҏ();ӹ=false;}}Vector3D ӳ;Vector3D?Ӳ;Queue<Vector3D>ӱ=new Queue<Vector3D>();Vector3D Ӱ(double Ӹ,Vector3D
ӯ,MatrixD ұ){var ӭ=Vector3D.TransformNormal(ӯ,MatrixD.Transpose(ұ));ӱ.Enqueue(ӭ);var Ӭ=Variables.Get<int>("tick-avg-vel")
;if(ӱ.Count==Ӭ){Vector3D ѹ=Vector3D.Zero;foreach(var º in ӱ)ѹ+=º;ӱ.Dequeue();ӭ=ѹ/Ӭ;}return Ӹ*Math.Sin(ӭ.X)*ұ.Up+Ӹ*Math.
Sin(ӭ.Y)*ұ.Left;}Ӫ ӫ;class Ӫ{public Vector2 ө=new Vector2();List<IMyShipController>џ;Func<int>Ҽ;public IMyShipController Ө(
){return Ӯ;}public IMyShipController ӧ(){return ә;}IMyShipController Ӯ;IMyShipController ә;Vector3 Ӑ;Vector2 Ӌ;float ӊ;
public void Ӊ(){Ӯ=null;ӈ=null;foreach(var Á in џ){if(Á.IsUnderControl){Ӯ=Á;if(!(Ӯ is IMyRemoteControl)){ә=Á;}break;}}if(Ӯ!=
null){Ӑ=Ӯ.MoveIndicator;Ӑ.X=-Ӑ.X;Ӑ.Z=-Ӑ.Z;}else Ӑ=Vector3.Zero;ӊ=Ӯ?.RollIndicator??0f;Ӌ=Ӯ?.RotationIndicator??Vector2.Zero;}
Vector3D?ӈ;public Vector3D ɔ(IMyShipController Á){if(!ӈ.HasValue){var ȿ=Vector3D.Zero;if(ҿ[Á]!=null)ȿ+=Vector3D.Rotate(ҿ[Á].
Value,Á.WorldMatrix);if(Ӏ.Ⱥ){Ӏ.ə((Vector3)Á.GetShipVelocities().LinearVelocity);Vector3 Ӈ;Ӏ.ƥ((float)ś.ŗ,out Ӈ);ȿ-=Ӈ;}ӈ=ȿ;}
return ӈ.Value;}public Vector3 ӆ(){return Ӑ;}public Vector2 Ӆ(){return Ӌ;}public float ӄ(){return ӊ;}class Ӄ{public string ӂ;
public int Ӂ;public int ȑ;}ɍ Ӏ=new ɍ();Dictionary<IMyShipController,Vector3D?>ҿ=new Dictionary<IMyShipController,Vector3D?>();
List<Ӄ>Ҿ;public Ӫ(List<IMyShipController>ҽ,Func<int>Ҽ,Dictionary<string,Vector3D>һ){џ=ҽ;this.Ҽ=Ҽ;foreach(var Ü in џ){if(!ҿ.
ContainsKey(Ü)){ҿ.Add(Ü,null);if(!string.IsNullOrEmpty(Ü.CustomData)){var Һ=new MyIni();MyIniParseResult ȡ;if(!string.IsNullOrEmpty
(Ü.CustomData)){if(!Һ.TryParse(Ü.CustomData,out ȡ))throw new Exception("CustomData ini fail");var U=Vector3D.Zero;U.X=Һ.
Get("hd-head-offset","X").ToSingle();U.Y=Һ.Get("hd-head-offset","Y").ToSingle();U.Z=Һ.Get("hd-head-offset","Z").ToSingle();
ҿ[Ü]=U;}}else if(һ.ContainsKey(Ü.BlockDefinition.SubtypeId)){ś.œ(
$"Found offset definition for cockpit subtype '{Ü.BlockDefinition.SubtypeId}'");ҿ[Ü]=һ[Ü.BlockDefinition.SubtypeId];}}}Ҿ=new List<Ӄ>();Ҿ.Add(new Ӄ{ӂ="spacebar"});Ҿ.Add(new Ӄ{ӂ="c"});Ҿ.Add(new Ӄ{ӂ=
"e"});Ҿ.Add(new Ӄ{ӂ="q"});Ҿ.Add(new Ӄ{ӂ="w"});Ҿ.Add(new Ӄ{ӂ="s"});Ҿ.Add(new Ӄ{ӂ="a"});Ҿ.Add(new Ӄ{ӂ="d"});}public bool ӗ(
string Ӓ){if(Ӯ!=null){bool Ӗ=false;if((Ӓ=="spacebar")&&(Ӑ.Y>0))Ӗ=true;if((Ӓ=="c")&&(Ӑ.Y<0))Ӗ=true;if((Ӓ=="e")&&(ӊ>0))Ӗ=true;if
((Ӓ=="q")&&(ӊ<0))Ӗ=true;if((Ӓ=="w")&&(Ӑ.Z<0))Ӗ=true;if((Ӓ=="s")&&(Ӑ.Z>0))Ӗ=true;if((Ӓ=="a")&&(Ӑ.X<0))Ӗ=true;if((Ӓ=="d")&&
(Ӑ.X>0))Ӗ=true;return Ӗ;}return false;}public bool ӕ(string Ӓ){var ӏ=Ҿ.First(Ҿ=>Ҿ.ӂ==Ӓ);if(ӗ(Ӓ)){if(ӏ.ȑ==0){ӏ.ȑ=1;ӏ.Ӂ=Ҽ()
;}if(ӏ.ȑ==2){ӏ.ȑ=0;}return false;}else{if((ӏ.ȑ==1)||(ӏ.ȑ==2)){ӏ.ȑ=0;return true;}}return false;}public bool Ӕ(string Ӓ){
return ӓ(Ӓ,Ҽ(),ӗ(Ӓ));}bool ӓ(string Ӓ,int Ә,bool ӑ){var ӏ=Ҿ.First(Ҿ=>Ҿ.ӂ==Ӓ);if(ӑ){if(ӏ.ȑ==0){ӏ.ȑ=1;ӏ.Ӂ=Ә;}if(ӏ.ȑ==2){ӏ.ȑ=0;
return true;}}else{if(Ә-ӏ.Ӂ<30){if(ӏ.ȑ==1){ӏ.ȑ=2;}}else{ӏ.ȑ=0;}}return false;}}static Vector3D ӎ(Vector3D ň,Vector3D ſ,
Vector3D ŋ,Vector3D ž,double Ņ,Vector3D Ξ,Vector3D Ӎ){Vector3D ӌ=ŋ;var ũ=ŋ-ň;double Ϭ=ũ.Length()/Ņ;Vector3D ō=Vector3D.Normalize
(ũ)*Ņ;if(Ξ.Length()>0){double º=0,Ő=0;var Ż=ň-Ӎ;var ź=ӌ-Ӎ;Ő=ź.Length()-Ż.Length();º=(ũ-Vector3D.Normalize(Ż)*Ő).Length();
var Ź=Ξ.Length();var Ÿ=Ņ;var ŷ=Math.Pow(Ÿ,2)/Ź;if(ŷ<ũ.Length()){return ӌ;}var Ŷ=Math.Atan((Math.Pow(Ÿ,2)+Math.Sqrt(Math.Pow
(Ÿ,4)-Ź*(Ź*Math.Pow(º,2)+2*Ő*Math.Pow(Ÿ,2))))/(Ź*º));var ŵ=Math.Atan((Math.Pow(Ÿ,2)-Math.Sqrt(Math.Pow(Ÿ,4)-Ź*(Ź*Math.Pow
(º,2)+2*Ő*Math.Pow(Ÿ,2))))/(Ź*º));var W=ŵ;var Ŵ=Vector3D.Normalize(ź)*(º*Math.Tan(W)-Ő);if(W!=Double.NaN){ӌ+=Ŵ;Ϭ=º/(Ÿ*
Math.Cos(W));ũ=ӌ-ň;ō=Vector3D.Normalize(ũ)*Ÿ*Math.Cos(W);}}ӌ=ű.ŉ(ň,ӌ,ž,ō);return ӌ;}static Ř ų<Ř>(object Ų)where Ř:struct{if
(Ų is Ř)return(Ř)Ų;return default(Ř);}static class ű{public static string Ű(params Vector3D[]ů){return string.Join(":",ů.
Select(ŭ=>string.Format("{0}:{1}:{2}",ŭ.X,ŭ.Y,ŭ.Z)));}public static string Ů(Vector3D ŭ,string Ŭ){return string.Format(
"GPS:{0}:{1}:{2}:{3}:",Ŭ,ŭ.X,ŭ.Y,ŭ.Z);}public static Vector3D ū(string º,string Ő,string ż){return new Vector3D(double.Parse(º),double.Parse(Ő
),double.Parse(ż));}public static bool Ū(Vector3D Ň,Vector3D Ə,Vector3D ň,Vector3D ſ,Vector3D ƍ,ref Vector3D ƌ,ref double
ń){ň+=Ə*3f;var ő=Ə*100;var Ľ=ő+ſ;var Ƌ=Vector3D.Reject(Ľ,Ə);var ŏ=Ľ-Ƌ;var Ɗ=(float)Math.Sqrt(200f*200f-Ƌ.LengthSquared())
-ŏ.Length();var Ɖ=Ɗ/600;var Ǝ=Ə*(ŏ.Length()+Ɗ/2f);var ƈ=ň+(Ǝ+Ƌ)*Ɖ;var Ƈ=Ľ+Ə*Ɗ;var Ɔ=Vector3D.Normalize(ƍ-ſ);var ƅ=
Vector3D.Cross(Ə,Ɔ);var Ƅ=new PlaneD(Ň,Vector3D.Cross(ƅ,Ɔ));var ƃ=new RayD(ƈ,Vector3D.Normalize(Ƈ));double?Ƃ;ƃ.Intersects(ref Ƅ,
out Ƃ);if(Ƃ.HasValue){ƌ=ƃ.Position+ƃ.Direction*Ƃ.Value;var Ɓ=Ƃ.Value/200;ń=Ɖ+Ɓ;return true;}return false;}public static
Vector3D ƀ(Vector3D ň,Vector3D ſ,Vector3D Ň,Vector3D ž){Vector3D ũ=Ň-ň;Vector3D ő=Vector3D.Normalize(ũ)*100;Vector3D Ľ=ő+ſ;
double ŏ=Vector3D.Dot(Vector3D.Normalize(ũ),Ľ);ŏ=Math.Min(ŏ,200);var Ŏ=(200-ŏ)*(200-ŏ)/(2*600);var Ó=ũ.Length();var ō=(Ŏ*(200+
ŏ)/2+(Ó-Ŏ)*200)/Ó;return ŉ(ň,Ň,ž-ſ,ō,ref Ó);}public static Vector3D ŉ(Vector3D Ō,Vector3D ŋ,Vector3D ņ,Vector3D Ŋ){double
Ņ=Vector3D.Dot(Vector3D.Normalize(ŋ-Ō),Ŋ);if(Ņ<30)Ņ=30;var ń=0d;return ŉ(Ō,ŋ,ņ,Ņ,ref ń);}public static Vector3D ŉ(
Vector3D ň,Vector3D Ň,Vector3D ņ,double Ņ,ref double ń){var Ń=Vector3D.Distance(ň,Ň);var ł=(Ň-ň).Normalized();var Ł=Ň;var ŀ=ņ.
Length();if(ŀ>float.Epsilon){var Ŀ=ņ/ŀ;var ľ=Math.PI-Math.Acos(Vector3D.Dot(ł,Ŀ));var Ő=ŀ*Math.Sin(ľ)/Ņ;if(Math.Abs(Ő)<=1){var
Œ=Math.Asin(Ő);var đ=Ń*Math.Sin(Œ)/Math.Sin(ľ+Œ);ń=đ/ŀ;Ł=Ň+Ŀ*đ;}else{ń=-1;}}else{ń=Ń/Ņ;}return Ł;}public static Vector3D
Ũ(Vector3D ŧ,MatrixD Ŧ,MatrixD ť,Vector3D Ť,float ţ){var Ţ=Ŧ.Up;var š=Vector3D.ProjectOnPlane(ref ŧ,ref Ţ);var Š=-(float)
Math.Atan2(Vector3D.Dot(Vector3D.Cross(Ŧ.Forward,š),Ţ),Vector3D.Dot(Ŧ.Forward,š));Ţ=Ŧ.Right;š=Vector3D.ProjectOnPlane(ref ŧ,
ref Ţ);var ş=-(float)Math.Atan2(Vector3D.Dot(Vector3D.Cross(Ŧ.Forward,š),Ţ),Vector3D.Dot(Ŧ.Forward,š));float Ş=0;if(Ť!=
Vector3D.Zero){Ţ=Ŧ.Forward;š=Vector3D.ProjectOnPlane(ref Ť,ref Ţ);š=š.Normalized();Ş=(float)Math.Atan2(Vector3D.Dot(Vector3D.
Cross(Ŧ.Up,š),Ţ),Vector3D.Dot(Ŧ.Up,š));}var Ü=new Vector3D(ş,Š,Ş*ţ);var ŝ=Vector3D.Rotate(Ü,Ŧ);var Ŝ=Vector3D.Rotate(ŝ,
MatrixD.Transpose(ť));return Ŝ;}}static class ś{static string Ś="";static Action<string>ř;static IMyTextSurface U;public static
double Ř;public static int G;public static double ŗ=1/60f;public static void Ŗ(Program ŕ){ř=ŕ.Echo;U=ŕ.Me.GetSurface(0);U.
ContentType=ContentType.TEXT_AND_IMAGE;U.WriteText("");}public static void Ŕ(string đ){if(Ś==""||đ.Contains(Ś))ř(đ);}public static
void œ(string đ){U.WriteText($"{G}: {đ}\n",true);}}Vector3D[]Ǆ=new Vector3D[8];Vector2[]ǂ=new Vector2[8];List<Vector2>ǁ(
BoundingBoxD ą,Vector3D ǀ,IMyTextPanel đ){ą.GetCorners(Ǆ);Ӿ(Ǆ[0],đ,ǀ,out ǂ[0]);Ӿ(Ǆ[1],đ,ǀ,out ǂ[1]);Ӿ(Ǆ[2],đ,ǀ,out ǂ[2]);Ӿ(Ǆ[3],đ,ǀ,
out ǂ[3]);Ӿ(Ǆ[4],đ,ǀ,out ǂ[4]);Ӿ(Ǆ[5],đ,ǀ,out ǂ[5]);Ӿ(Ǆ[6],đ,ǀ,out ǂ[6]);Ӿ(Ǆ[7],đ,ǀ,out ǂ[7]);var ƿ=ƾ.ƹ(ǂ.ToList());return
ƿ;}class ƾ{static double ƽ(Vector2 Ƽ,Vector2 ƻ,Vector2 ƺ){return(ƻ.X-Ƽ.X)*(ƺ.Y-Ƽ.Y)-(ƻ.Y-Ƽ.Y)*(ƺ.X-Ƽ.X);}public static
List<Vector2>ƹ(List<Vector2>Ƹ){if(Ƹ==null)return null;if(Ƹ.Count()<=1)return Ƹ;int Ʒ=Ƹ.Count(),ƶ=0;List<Vector2>Ƶ=new List<
Vector2>(new Vector2[2*Ʒ]);Ƹ.Sort((Ŝ,ƴ)=>Ŝ.X==ƴ.X?Ŝ.Y.CompareTo(ƴ.Y):Ŝ.X.CompareTo(ƴ.X));for(int Z=0;Z<Ʒ;++Z){while(ƶ>=2&&ƽ(Ƶ[ƶ
-2],Ƶ[ƶ-1],Ƹ[Z])<=0)ƶ--;Ƶ[ƶ++]=Ƹ[Z];}for(int Z=Ʒ-2,É=ƶ+1;Z>=0;Z--){while(ƶ>=É&&ƽ(Ƶ[ƶ-2],Ƶ[ƶ-1],Ƹ[Z])<=0)ƶ--;Ƶ[ƶ++]=Ƹ[Z];}
return Ƶ.Take(ƶ-1).ToList();}}struct Ƴ{public string Ă;public Vector2 ǃ;public Vector2 Ʋ;public List<MySprite>ǅ;public List<
List<MySprite>>ǖ;}Ƴ ǔ=new Ƴ(){Ă=ǐ,ǃ=new Vector2(512*0.5f,512*0.5f),Ʋ=new Vector2(512*0.45f,512*0.45f),ǅ=new List<MySprite>()
};void Ǔ(){var ǒ=Ǐ(ǐ,ǔ.ǃ,ǔ.Ʋ);if(ǒ.Count>0)ǔ.ǅ=ǒ[0].Item2;ǔ.ǖ=new List<List<MySprite>>();foreach(var Ǒ in ǒ.Skip(1)){ǔ.ǖ.
Add(Ǒ.Item2);}}const string ǐ="Sprites/default=ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.3359375,SizeY=0.1248103,PosX=0.4677796,PosY=0.3658402,Rotation=2.249999,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.2851563,SizeY=0.1059436,PosX=0.2666973,PosY=0.506598,Rotation=1.649999,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.09179688,SizeY=0.03410512,PosX=0.1527504,PosY=0.3502004,Rotation=1.549999,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.04101563,SizeY=0.01523846,PosX=0.1393449,PosY=0.5624543,Rotation=4.249998,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.04101563,SizeY=0.01523846,PosX=0.1192367,PosY=0.5982023,Rotation=4.499999,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.1640625,SizeY=0.02220203,PosX=0.6152402,PosY=-0.101118,Rotation=7.25001,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.06445313,SizeY=0.008722229,PosX=0.1616875,PosY=0.2720015,Rotation=12.50003,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.09765625,SizeY=0.0132155,PosX=0.1728586,PosY=0.1200726,Rotation=11.60003,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.06054688,SizeY=0.008193607,PosX=0.1795614,PosY=0.03070235,Rotation=12.75003,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.1054688,SizeY=0.01427274,PosX=0.213075,PosY=-0.7177719,Rotation=15.20004,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.04296875,SizeY=0.005814818,PosX=0.2979766,PosY=-0.7155376,Rotation=17.35002,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.1386719,SizeY=0.018766,PosX=0.4052207,PosY=-0.6105278,Rotation=16.05004,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.06054688,SizeY=0.008193607,PosX=0.4878883,PosY=-0.5032836,Rotation=17.70002,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.08203125,SizeY=0.01110102,PosX=0.5348074,PosY=-0.5300945,Rotation=17.90001,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.07226563,SizeY=0.009779467,PosX=0.6420516,PosY=-0.5881852,Rotation=18.8,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.07226563,SizeY=0.009779467,PosX=0.715782,PosY=-0.6328703,Rotation=18.05001,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.09570313,SizeY=0.01295119,PosX=0.7783411,PosY=-0.5971223,Rotation=17.30002,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.07617188,SizeY=0.01030809,PosX=0.7358903,PosY=-0.4608328,Rotation=17.60002,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.09765625,SizeY=0.0132155,PosX=0.680034,PosY=-0.3714629,Rotation=17.45002,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.09765625,SizeY=0.09202367,PosX=0.7381238,PosY=-0.2105961,Rotation=12.75003,Type=TEXTURE|ColorR=91,ColorG=164,ColorB=0,ColorA=255,Brush=RightTriangle,SizeX=0.1523438,SizeY=0.1435569,PosX=0.7269528,PosY=-0.02068478,Rotation=13.15003,Type=TEXTURE|ColorR=65,ColorG=91,ColorB=33,ColorA=255,Brush=RightTriangle,SizeX=0.08007813,SizeY=0.07545941,PosX=0.4722484,PosY=-0.04749572,Rotation=10.70002,Type=TEXTURE|ColorR=65,ColorG=91,ColorB=33,ColorA=255,Brush=RightTriangle,SizeX=0.08007813,SizeY=0.07545941,PosX=0.4476715,PosY=0.02400005,Rotation=11.35002,Type=TEXTURE|ColorR=65,ColorG=91,ColorB=33,ColorA=255,Brush=RightTriangle,SizeX=0.08007813,SizeY=0.07545941,PosX=0.3426617,PosY=-0.02068478,Rotation=13.05003,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.09375,SizeY=0.2898549,PosX=-0.2069641,PosY=0.5267071,Rotation=9.300016,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.08789063,SizeY=0.2717389,PosX=-0.3924071,PosY=0.3122188,Rotation=8.800014,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.09570313,SizeY=0.1189948,PosX=-0.5465703,PosY=-0.1971902,Rotation=10.80002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.1210938,SizeY=0.1505648,PosX=-0.5398676,PosY=-0.0363242,Rotation=10.50002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.01953125,SizeY=0.1443267,PosX=-0.454966,PosY=-0.326777,Rotation=3.399997,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.01953125,SizeY=0.1443267,PosX=-0.5242277,PosY=-0.4764715,Rotation=5.550002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.01953125,SizeY=0.1443267,PosX=-0.6135978,PosY=-0.5747789,Rotation=6.000004,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.0078125,SizeY=0.05773068,PosX=-0.6448774,PosY=-0.7133025,Rotation=6.450006,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.01171875,SizeY=0.08659603,PosX=-0.5778496,PosY=-0.6887257,Rotation=5.600002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.01953125,SizeY=0.1443267,PosX=-0.4482629,PosY=-0.6149953,Rotation=5.1,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.009765625,SizeY=0.07216336,PosX=-0.3119738,PosY=-0.6127611,Rotation=6.200005,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.0078125,SizeY=0.05773068,PosX=-0.2873973,PosY=-0.6216981,Rotation=6.650006,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.005859375,SizeY=0.04329801,PosX=-0.2717574,PosY=-0.6149953,Rotation=6.250005,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.005859375,SizeY=0.04329801,PosX=-0.2471805,PosY=-0.6284009,Rotation=6.450006,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.009765625,SizeY=0.07216336,PosX=-0.2091981,PosY=-0.6552119,Rotation=7.000008,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.005859375,SizeY=0.04329801,PosX=-0.1957926,PosY=-0.6552119,Rotation=6.800007,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.005859375,SizeY=0.04329801,PosX=-0.1555762,PosY=-0.6775544,Rotation=5.350001,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.001953125,SizeY=0.01443267,PosX=-0.1108914,PosY=-0.6909599,Rotation=6.600006,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.009765625,SizeY=0.07216336,PosX=-0.08631444,PosY=-0.7088339,Rotation=6.600006,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.0078125,SizeY=0.05773068,PosX=-0.02598983,PosY=-0.7401135,Rotation=5.600002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.0078125,SizeY=0.05773068,PosX=0.01199257,PosY=-0.7244737,Rotation=6.250005,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.0078125,SizeY=0.05773068,PosX=0.03656912,PosY=-0.7200052,Rotation=5.950004,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.0078125,SizeY=0.05773068,PosX=0.0678488,PosY=-0.7043654,Rotation=5.600002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.09375,SizeY=0.1393075,PosX=-0.2717574,PosY=-0.0184508,Rotation=10.55002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.04101563,SizeY=0.06094703,PosX=-0.3387852,PosY=0.1513523,Rotation=9.950019,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.04101563,SizeY=0.06094703,PosX=-0.2941,PosY=0.09102774,Rotation=10.20002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.0625,SizeY=0.09287168,PosX=-0.06844062,PosY=0.1424153,Rotation=9.200016,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.03710938,SizeY=0.05514256,PosX=-0.07514334,PosY=0.04857695,Rotation=8.000011,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.03710938,SizeY=0.05514256,PosX=-0.04386407,PosY=0.06645083,Rotation=9.400017,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.03125,SizeY=0.04643584,PosX=-0.0617379,PosY=-0.03632462,Rotation=11.05002,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.03320313,SizeY=0.1047101,PosX=-0.0595035,PosY=0.3524351,Rotation=15.80004,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.01757813,SizeY=0.05543474,PosX=-0.03269255,PosY=0.5870312,Rotation=18.8,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=RightTriangle,SizeX=0.01171875,SizeY=0.03695649,PosX=-0.00588125,PosY=0.6116081,Rotation=18.8,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.01953125,SizeY=0.01953125,PosX=0.01422691,PosY=0.6361848,Rotation=18.8,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.01953125,SizeY=0.01953125,PosX=0.04774058,PosY=0.6384192,Rotation=18.8,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.01953125,SizeY=0.01953125,PosX=0.09019136,PosY=0.6384192,Rotation=18.8,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.0234375,SizeY=0.0234375,PosX=0.07231724,PosY=0.2876418,Rotation=18.8,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.0234375,SizeY=0.0234375,PosX=0.09242535,PosY=0.2876418,Rotation=18.8,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.08789063,SizeY=0.03750901,PosX=-0.1265312,PosY=-0.1480371,Rotation=22.34995,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.09375,SizeY=0.04000961,PosX=0.286805,PosY=-0.1301629,Rotation=21.54996,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.05078125,SizeY=0.02167187,PosX=0.2711649,PosY=-0.0676043,Rotation=22.24995,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.05078125,SizeY=0.02167187,PosX=-0.1064234,PosY=-0.07877541,Rotation=21.69995,Type=TEXTURE|ColorR=9,ColorG=18,ColorB=18,ColorA=255,Brush=SquareSimple,SizeX=0.0546875,SizeY=0.02333895,PosX=-0.02599025,PosY=0.2273169,Rotation=26.79988,Type=TEXTURE|ColorR=0,ColorG=111,ColorB=144,ColorA=255,Brush=Hud Dog,SizeX=1,SizeY=1,PosX=0.08125436,PosY=-0.5055171,Rotation=0.00390625,Type=TEXT|ColorR=99,ColorG=221,ColorB=118,ColorA=255,Brush=Hud Dog,SizeX=1,SizeY=1,PosX=0.08795714,PosY=-0.4965804,Rotation=0.00390625,Type=TEXT\nSprites/frame-1.00=ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=-0.1131254,PosY=-0.1110004,Rotation=1.85,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=0.2733999,PosY=-0.09982932,Rotation=4.5,Type=TEXTURE\nSprites/frame-2.00=ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=-0.1131254,PosY=-0.1110004,Rotation=1.85,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=0.2733999,PosY=-0.09982932,Rotation=4.5,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=-0.1220625,PosY=-0.1020641,Rotation=8.250013,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=0.2823367,PosY=-0.09982967,Rotation=10.80002,Type=TEXTURE\nSprites/frame-3.00=ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=-0.1131254,PosY=-0.1110004,Rotation=1.85,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=0.2733999,PosY=-0.09982932,Rotation=4.5,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=-0.1220625,PosY=-0.1020641,Rotation=8.250013,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=0.2823367,PosY=-0.09982967,Rotation=10.80002,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=-0.1220625,PosY=-0.1154695,Rotation=14.50004,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=0.2756339,PosY=-0.1042981,Rotation=17.00003,Type=TEXTURE\nSprites/frame-4.00=ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=-0.1131254,PosY=-0.1110004,Rotation=1.85,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=0.2733999,PosY=-0.09982932,Rotation=4.5,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=-0.1220625,PosY=-0.1020641,Rotation=8.250013,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=0.2823367,PosY=-0.09982967,Rotation=10.80002,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=-0.1220625,PosY=-0.1154695,Rotation=14.50004,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=0.2756339,PosY=-0.1042981,Rotation=17.00003,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.07421875,SizeY=0.1882441,PosX=-0.1131254,PosY=-0.1065324,Rotation=20.79997,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.06445313,SizeY=0.1634751,PosX=0.2890395,PosY=-0.1110012,Rotation=23.44993,Type=TEXTURE\nSprites/frame-5.00=ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=-0.1131254,PosY=-0.1110004,Rotation=1.85,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.046875,SizeY=0.118891,PosX=0.2733999,PosY=-0.09982932,Rotation=4.5,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=-0.1220625,PosY=-0.1020641,Rotation=8.250013,Type=TEXTURE|ColorR=254,ColorG=0,ColorB=1,ColorA=5,Brush=Triangle,SizeX=0.05664063,SizeY=0.1436599,PosX=0.2823367,PosY=-0.09982967,Rotation=10.80002,Type=TEXTURE\nSprites/frame-6.00=\nSprites/frame-7.00="
;List<MyTuple<string,List<MySprite>>>Ǐ(string ą,Vector2 ƞ,Vector2 ǎ){var Ǎ=ą.Replace("Hud Dog",Ver);var Ƭ=new List<
MyTuple<string,List<MySprite>>>();var Ǖ=Ǎ.Split('\n').ToDictionary(đ=>đ.Split('=')[0],đ=>string.Join("=",đ.Split('=').Skip(1)))
;foreach(var ǋ in Ǖ.Where(ŭ=>ŭ.Key.Contains("Sprites"))){var Ǌ=ǋ.Key.Split('/')[1];if(!string.IsNullOrEmpty(ǋ.Value)){var
ǉ=new List<MySprite>();Ƭ.Add(new MyTuple<string,List<MySprite>>(Ǌ,ǉ));var ǈ=ǋ.Value.Split(new[]{'|'},StringSplitOptions.
RemoveEmptyEntries);foreach(string Ǉ in ǈ){var ǆ=Ǉ.Split(',').ToDictionary(đ=>đ.Split('=')[0],đ=>đ.Split('=')[1]);Color Á=new Color(byte.
Parse(ǆ["ColorR"]),byte.Parse(ǆ["ColorG"]),byte.Parse(ǆ["ColorB"]),byte.Parse(ǆ["ColorA"]));MySprite Ɲ;SpriteType ǌ;if(Enum.
TryParse(ǆ["Type"],out ǌ)&&(ǌ==SpriteType.TEXT)){Ɲ=MySprite.CreateText(ǆ["Brush"],"Debug",Á);}else{Vector2 Ʊ=new Vector2(float.
Parse(ǆ["SizeX"]),float.Parse(ǆ["SizeY"]));var ƫ=new Vector2(Ʊ.X>1?Ʊ.X:Ʊ.X*ǎ.X,Ʊ.Y>1?Ʊ.Y:Ʊ.Y*ǎ.Y);Ɲ=new MySprite(SpriteType.
TEXTURE,ǆ["Brush"],size:ƫ,color:Á);}var Ƣ=new Vector2(float.Parse(ǆ["PosX"])*ǎ.X/2f,float.Parse(ǆ["PosY"])*ǎ.Y/2f);Ɲ.Position=ƞ
+Ƣ;var ơ=float.Parse(ǆ["Rotation"]);if(ǌ==SpriteType.TEXT)Ɲ.RotationOrScale=ơ*(ǎ.Y);else Ɲ.RotationOrScale=ơ;ǉ.Add(Ɲ);}}}
return Ƭ;}float Ơ=0.001f;MySprite Ɵ(Vector2 ƞ,int G,ref MySprite Ɲ){if(G%400==0)Ơ=0.001f;if(Ơ>6.28)Ơ=0;if(Ơ!=0)Ơ+=0.001f;else
return Ɲ;float đ=(float)Math.Sin(Ơ);float Á=(float)Math.Cos(Ơ);var Ɯ=Ɲ.Position.Value;Ɯ-=ƞ;float ƛ=Ɯ.X*Á-Ɯ.Y*đ;float ƚ=Ɯ.X*đ+Ɯ
.Y*Á;Ɲ.Position=ƞ+new Vector2(ƛ,ƚ);return Ɲ;}Color ƙ(string[]Ƙ){return new Color(int.Parse(Ƙ[0]),int.Parse(Ƙ[1]),int.
Parse(Ƙ[2]),int.Parse(Ƙ[3]));}Ɩ Ɨ;class Ɩ{public List<Ƨ>ƕ;public Ƨ Ɣ{get;private set;}public HashSet<ă>Ɠ=new HashSet<ă>();
public long?ƒ;public string Ƒ;int Ɛ;Func<IMyEntity>Ĥ;Func<HashSet<ă>,ă?>ƣ;public Ɩ(List<Ƨ>Ƥ,Func<IMyEntity>Ģ){ƕ=Ƥ;Ĥ=Ģ;ư(
"First");}public void ư(string Ư){ƒ=null;if(Ư=="First"){Ƒ=Ư;ƣ=Ʈ=>Ʈ.First();}if(Ư=="Loop"){if(Ƒ==Ư)Ɛ++;Ƒ=Ư;ƣ=Ʈ=>{int º=Ɛ%Ʈ.Count
;int Z=0;foreach(var É in Ʈ)if(Z++==º)return É;return null;};}if(Ư=="Crosshair"){Ƒ=Ư;ƣ=Ʈ=>{var ċ=Ĥ().WorldMatrix;foreach(
var É in Ʈ)if(Vector3D.Dot((É.Ā-ċ.Translation).Normalized(),ċ.Forward)>0.994f)return É;return null;};}}public ˡ ƭ(int G){ˡ
Ƭ=null;Ɣ=null;Ɠ.Clear();foreach(var É in ƕ){É.ƥ(Ɠ,ƒ,G);if(ƒ.HasValue&&É.Ʀ.ȁ==ƒ&&É.Ʀ.ˈ){Ƭ=É.Ʀ;Ɣ=É;}}ś.Ŕ($"TargetId: {ƒ}");
if((ƒ==null||Ƭ==null)&&Ɠ.Count>0)ƒ=ƣ(Ɠ)?.ā;return Ƭ;}public void ƪ(List<string>Ʃ){List<Ƨ>ƨ=new List<Ƨ>();foreach(var Ŭ in
Ʃ){var É=ƕ.FirstOrDefault(º=>º.ļ==Ŭ);if(É!=null)ƨ.Add(É);}ƕ=ƨ;}}interface Ƨ{ˡ Ʀ{get;}void ƥ(HashSet<ă>Ð,long?Ï,int G);
string ļ{get;}}class č:Ƨ{public string ļ=>"GPStargeter";public ˡ Ʀ{get;}=new ˡ("",null);public void â(Vector3D U){Ʀ.ˉ(U);Ʀ.ȁ=1
;}public void ƥ(HashSet<ă>Ð,long?Ï,int G){if(Ʀ.ˈ)Ð.Add(new ă{ā=1,Ā=Ʀ.Ǿ.Value});}}class á:Ƨ{public string ļ=>
"RaycastSingleTargeter";public ˡ Ʀ{get;}=new ˡ("",null);List<IMyCameraBlock>Ø;IMyCubeGrid à;public á(List<IMyCameraBlock>ß,IMyCubeGrid Þ){Ø=ß;à
=Þ;}public void Ý(IMyShipController Ü){ȳ(Ü,Variables.Get<float>("raycast-range"),Ø,Û);}bool Û(MyDetectedEntityInfo Ú){if(
Ú.EntityId!=à.EntityId&&(à.GetPosition()-Ú.HitPosition.Value).Length()>50){Ʀ.ˉ(Ú.HitPosition.Value);Ʀ.ȁ=2;return true;}
return false;}public void ƥ(HashSet<ă>Ð,long?Ï,int G){if(Ʀ.ˈ)Ð.Add(new ă{ā=2,Ā=Ʀ.Ǿ.Value});}public void Ç(){Ʀ.Ͷ();}}class Ù:Ƨ{
public string ļ=>"RaycastTrackerTargeter";public ˡ Ʀ{get;}=new ˡ("",null);List<IMyCameraBlock>Ø;ď Ö;public List<Vector3D>Õ=new
List<Vector3D>();int ã;public string Ô;bool å;double ö;double ô;public double ó{get;private set;}IMyShipController ò;float ñ
=5f;int ð=20;int ï=1;public Ù(List<IMyCameraBlock>ß,List<IMyShipController>î,ď í){Ø=ß;Ø.ForEach(º=>º.EnableRaycast=true);
ò=î.First();Ö=í;for(int õ=1;õ<ð;õ++){ï+=õ*6;}}public bool ì(IMyTerminalBlock æ,ref Vector3D ê,ref Vector3D é){var Ó=
Variables.Get<float>("raycast-range");var Á=æ.WorldMatrix.Translation+æ.WorldMatrix.Forward*Ó;var V=ñ*0.866f;if(ö>ï*Ó){ê=æ.
WorldMatrix.Translation+æ.WorldMatrix.Forward*Ó;é=ê+æ.WorldMatrix.Right*V*ð;return true;}return false;}public void è(
IMyShipController Ü){Ô="";ȳ(Ü,Variables.Get<float>("raycast-range"),Ø,Û);}bool Û(MyDetectedEntityInfo Ú){if(Ú.EntityId==ò.CubeGrid.
EntityId||(ò.GetPosition()-Ú.HitPosition.Value).Length()<50){Ô="Too close";return false;}if(Ú.Type==MyDetectedEntityType.
SmallGrid||Ú.Type==MyDetectedEntityType.LargeGrid){if(!Ñ.ContainsKey(Ú.EntityId)){Ñ.Add(Ú.EntityId,new R(Ú,ś.G));Ô=
"Added new target";return true;}Ô="Same target";}else{Ô="Hit not a grid entity";}return false;}public void ç(IMyCubeBlock æ){if(æ==null)
return;Ô="";Õ.Clear();ã=ś.G;var Ó=Variables.Get<float>("raycast-range");var À=æ.GetPosition()+æ.WorldMatrix.Forward*Ó;var D=Ø.
FirstOrDefault(º=>º.CanScan(À));if(D!=null){Ó=ȭ(Ó,D,ò,true);var µ=D.Raycast(À);if(µ.HitPosition.HasValue&&(µ.EntityId!=æ.CubeGrid.
EntityId)){if(!Ñ.ContainsKey(µ.EntityId)){Ñ.Add(µ.EntityId,new R(µ,ś.G));}else{Ñ[µ.EntityId].F(µ,ś.G);}Ô=
"Base rc hit, added new local offset";var ª=Ñ[µ.EntityId];Ö.ʍ(µ.EntityId,µ.Orientation,µ.Position,µ.HitPosition.Value,false);Ö.ʡ();}else if(Т.О.ж(
"raycast-burst")&&(ö>ï*Ó)){int u=0;int o=0;int j=1;int Z=0;bool Y=false;foreach(var X in Ø){while(X.CanScan(Ó)){double W=60f/180f*Math.
PI*Z/j;var V=ñ*0.866f;var U=new Vector2((float)Math.Cos(W),(float)Math.Sin(W))*V*j;var S=X.Raycast(À+D.WorldMatrix.Up*U.X+
D.WorldMatrix.Left*U.Y);if(S.HitPosition.HasValue){if(!Ñ.ContainsKey(S.EntityId)){Ñ.Add(S.EntityId,new R(S,ś.G));u++;}
else{Ñ[S.EntityId].F(S,ś.G);o++;}Õ.Add(S.HitPosition.Value);}if(Z++>6*j){Z=0;j++;if(j>ð){Y=true;break;}}}if(Y){break;}}Ô=
$"Scan: added {u}, updated {o}, hits {Õ.Count}";}else{Ô=$"Base rc miss";}}}public class R{public MyDetectedEntityInfo Q{get;private set;}public Vector3D N;public
Vector3D M=>Q.Velocity;public int K;public bool J=true;public R(MyDetectedEntityInfo I,int G){F(I,G);}public void F(
MyDetectedEntityInfo I,int G){Q=I;N=I.Position;K=G;J=true;}public void Ò(int G){if(G-K>120)J=false;else N+=M/60f;}}public Dictionary<long,R>
Ñ=new Dictionary<long,R>();public void ƥ(HashSet<ă>Ð,long?Ï,int G){var Î=Variables.Get<int>("tracking-clock");long Í=0;
foreach(var ª in Ñ){var Ì=ª.Value;Ì.Ò(G);var Ë=Ì.N;if(Ì.J){if(ś.G%Î==0){var Ê=false;foreach(var X in Ø){if(X.CanScan(Ë)){Ê=true
;var S=X.Raycast(Ë+Vector3D.Normalize(Ë-X.GetPosition())*20f);if(S.HitPosition.HasValue&&S.EntityId==ª.Key){Ì.F(S,G);
break;}}}}}else{Í=ª.Key;}}if(Í!=0)Ñ.Remove(Í);foreach(var ª in Ñ){Ð.Add(new ă{ā=ª.Key,Ā=ª.Value.N});if(ª.Value.J&&ª.Key==Ï){
var É=ª.Value;Ʀ.ͺ(É.Q,Ö.ɔ(É.Q.EntityId,É.Q.Orientation,É.N,å));å=false;}}if(ś.G-ã>120)Õ.Clear();ö=0;foreach(var X in Ø){ö+=
X.AvailableScanRange;}if(ś.G%60==0){ó=ö-ô;ô=ö;}}public void È(){å=true;}public void Ç(){Õ.Clear();Ñ.Clear();}
StringBuilder Æ=new StringBuilder();public string Å(){Æ.Clear();Æ.Append($"\nRCRs: {ö/1000:f0}km");var Ä=ó/1000;Æ.Append(
$" ({Ä:#^;#\u00ac;#~})");if(Ä<0)Æ.Append($" {ö/Math.Abs(ó):f0}s");Æ.Append('\n').Append(Ô);return Æ.ToString();}}class Ã:Ƨ{public string ļ=>
"TurretControllerTargeter";public ˡ Ʀ{get;}=new ˡ("",null);List<IMyTurretControlBlock>Â;public Ã(List<IMyTurretControlBlock>ë){Â=ë;foreach(var Á
in ë){Á.Range=2000;Á.AIEnabled=true;}}public void ø(){foreach(var Á in Â)Á.ApplyAction("ForgetTarget");}public void ƥ(
HashSet<ă>Ð,long?Ï,int G){foreach(var Á in Â){if(Á.HasTarget){var ī=Á.GetTargetedEntity();if(!ī.IsEmpty()){Ð.Add(new ă{ā=ī.
EntityId,Ā=ī.Position});if(ī.EntityId==Ï)Ʀ.ͺ(ī,ī.Position);}}}}}class Ī:Ƨ{public string ļ=>"TurretAiTargeter";public ˡ Ʀ{get;}=
new ˡ("",null);MyDetectedEntityInfo ĩ;bool Ĩ;ď Ö;public int ħ;List<IMyLargeTurretBase>Ħ;Func<MyDetectedEntityInfo,bool>ĥ;
Func<IMyEntity>Ĥ;public Ī(List<IMyLargeTurretBase>ģ,ď í,Func<IMyEntity>Ģ,Func<MyDetectedEntityInfo,bool>ġ=null){Ħ=ģ;Ĥ=Ģ;Ö=í;
foreach(var Á in Ħ){Á.Range=5000;}ĥ=ġ;}public void ƥ(HashSet<ă>Ð,long?Ï,int G){bool Ġ=false;Vector3D Ĭ=Ĥ.Invoke()?.WorldMatrix.
Forward??Vector3D.Zero;foreach(var Ó in Ħ){if(!Ó.HasTarget)continue;var Z=Ó.GetTargetedEntity();if(!Z.IsEmpty()){if(Ġ){Ĩ=true;}
else if(Ĭ.IsZero()||Vector3D.Dot(Vector3D.Normalize(Z.Position-Ó.GetPosition()),Ĭ)>Variables.Get<float>("turret-ai-cone")){Ġ
=true;if(ĥ!=null)Ġ=ĥ(Z);if(!Ġ)ħ++;if(Ġ){Ð.Add(new ă{ā=Z.EntityId,Ā=Z.Position});Ĩ=true;ĩ=Z;if(!Т.О.ж("aim-to-center")){Ö.
ʍ(ĩ.EntityId,ĩ.Orientation,ĩ.Position,Z.HitPosition.Value,true);}}else{Ĩ=false;Ó.ResetTargetingToDefault();}}else Ó.
ResetTargetingToDefault();}}if(Ĩ&&ĩ.EntityId==Ï){var U=Т.О.ж("aim-to-center")?ĩ.Position:Ö.ɔ(ĩ.EntityId,ĩ.Orientation,ĩ.Position);Ʀ.ͺ(ĩ,U);}}}ĺ
Ļ;class ĺ:Ƨ{public string ļ=>"WcTargeter";public ˡ Ʀ{get;}=new ˡ("",null);Action<IMyTerminalBlock,IDictionary<long,
MyDetectedEntityInfo>>Ĺ;IMyProgrammableBlock ĸ;Func<IMyEntity>Ĥ;public ĺ(Action<IMyTerminalBlock,IDictionary<long,MyDetectedEntityInfo>>ķ,
IMyProgrammableBlock Ķ,Func<IMyEntity>Ģ){Ĺ=ķ;ĸ=Ķ;Ĥ=Ģ;}Dictionary<long,MyDetectedEntityInfo>ĵ=new Dictionary<long,MyDetectedEntityInfo>();
public void ƥ(HashSet<ă>Ð,long?Ï,int G){ĵ.Clear();try{Ĺ(ĸ,ĵ);}catch(Exception e){ś.œ(e.ToString());}var Ü=Ĥ();if(Ü!=null){
foreach(var É in ĵ.Values){var U=É.Position;Ð.Add(new ă{ā=É.EntityId,Ā=U});if(É.EntityId==Ï)Ʀ.ͺ(É,U);}}}}class ĳ:Ƨ{public
string ļ=>"DataLinkTargeter";public ˡ Ʀ{get;}=new ˡ("",null);ɰ Ĳ;ʠ ı;ʠ İ;long?į;public ĳ(ɰ Į){Ĳ=Į;ı=new ʠ(
"tgp.global.gridsense.offer",true,Į).ʕ(null);İ=new ʠ("tgp.local.gridsense.update",false,Į,É=>į.HasValue&&(É.ȁ==į));}HashSet<long>ĭ=new HashSet<long>
();private List<ă>Ĵ=new List<ă>();List<MyIGCMessage>ğ=new List<MyIGCMessage>();bool ĕ;public void ƥ(HashSet<ă>Ð,long?Ï,
int G){Ą();if(į==null){if(!ĕ){ı.ɵ();ĕ=true;}Ĳ.ʃ(ı,ref ğ);Ĵ.Clear();ĭ.Clear();foreach(var ċ in ğ){var Ó=ų<MyTuple<long,
Vector3D>>(ċ.Data);if(Ó.Item1!=0){if(ĭ.Add(Ó.Item1))Ĵ.Add(new ă{ā=Ó.Item1,Ā=Ó.Item2,Ă=ċ.Source});}else{var Ċ=ų<ImmutableArray<
MyTuple<long,Vector3D>>>(ċ.Data);foreach(var ĉ in Ċ)if(ĭ.Add(ĉ.Item1))Ĵ.Add(new ă{ā=ĉ.Item1,Ā=ĉ.Item2,Ă=ċ.Source});}}foreach(
var Ĉ in Ĵ)Ð.Add(Ĉ);foreach(var É in Ĵ){if(É.ā==Ï){ć(É.ā,É.Ă);break;}}}else if(İ.ʝ?.ˈ==true){if(ĕ){ı.ɱ();ĕ=false;}if(İ.ʝ.ȁ
==Ï){Ʀ.ˉ(İ.ʝ.Ǿ.Value);Ʀ.M=İ.ʝ.M;Ʀ.ː=İ.ʝ.ː;Ʀ.ˑ=İ.ʝ.ˑ;Ʀ.ȁ=İ.ʝ.ȁ;Ʀ.ˎ=İ.ʝ.ˎ;}}}void ć(long Ć,long ą){ś.œ(
$"Start focusing target {Ć}");į=Ć;İ.ɣ(Ć,ą);}void Ą(){if(į!=null&&İ.ʝ!=null){if(İ.ʝ.ˇ){ś.œ(
$"Target expired, dropping. Tick: {ś.G} TickStamp: {İ.ʝ.K}");į=null;İ.ɱ();}}}}struct ă:IEquatable<ă>{public long Ă;public long ā;public Vector3D Ā;public bool Equals(ă ÿ){return ā
==ÿ.ā;}public override int GetHashCode(){return ā.GetHashCode();}}ý þ;class ý{IMyIntergridCommunicationSystem ü;Dictionary
<string,Func<string>>û=new Dictionary<string,Func<string>>();Dictionary<string,Func<List<MySprite>>>ú=new Dictionary<
string,Func<List<MySprite>>>();string Č;Vector2 ù;public int Ď;public ý(IMyIntergridCommunicationSystem ĝ,string Ĝ,Vector2 ě){
ü=ĝ;Č=Ĝ;ù=ě;}public void Ě(string ę,Func<string>Ę){û[ę]=Ę;}public void Ě(string ę,Func<List<MySprite>>Ę){ú[ę]=Ę;}List<
MyTuple<int,string,Vector2,Vector2,float,Vector4>>Ċ=new List<MyTuple<int,string,Vector2,Vector2,float,Vector4>>();public void ė
(List<MyIGCMessage>Ğ){foreach(var Ė in Ğ){if(Ė.Tag.Contains("shell.get")){var Ó=(string)Ė.Data;if(û.ContainsKey(Ó)){var Ē
=û[Ó]();if(Ē!=null){ü.SendUnicastMessage(Ė.Source,"shell.text",new MyTuple<string,string>(Ó,Ē));Ď++;}}else if(ú.
ContainsKey(Ó)){var Ē=ú[Ó]();if(Ē!=null&&Ē.Any()){ü.SendUnicastMessage(Ė.Source,"shell.sprites",new MyTuple<string,ImmutableArray<
MyTuple<int,string,Vector2,Vector2,float,Vector4>>>(Ó,ē(Ē)));Ď++;}}}else if(Ė.Tag=="shell.options"){var Ĕ=û.Keys.Select(º=>new
MyTuple<string,byte>(º,2)).ToList();Ĕ.AddRange(ú.Keys.Select(º=>new MyTuple<string,byte>(º,1)));ü.SendUnicastMessage(Ė.Source,
"shell.options",new MyTuple<string,ImmutableArray<MyTuple<string,byte>>>(Č,Ĕ.ToImmutableArray()));}}ś.Ŕ(
$"Shell handler unicast counter: {Ď}");}public ImmutableArray<MyTuple<int,string,Vector2,Vector2,float,Vector4>>ē(IEnumerable<MySprite>Ē){Ċ.Clear();foreach(
var đ in Ē)Ċ.Add(new MyTuple<int,string,Vector2,Vector2,float,Vector4>(đ.Type==SpriteType.TEXT?(int)đ.Alignment:-1,đ.Data,đ
.Size.HasValue?new Vector2(đ.Size.Value.X/ù.X,đ.Size.Value.Y/ù.Y):Vector2.One,đ.Position.HasValue?new Vector2(đ.Position.
Value.X/(ù.X*0.5f)-1,đ.Position.Value.Y/(ù.Y*0.5f)-1):Vector2.Zero,đ.RotationOrScale,đ.Color??Color.White));return Ċ.
ToImmutableArray();}}ď Đ=new ď();class ď{Dictionary<long,List<Vector3D>>ä=new Dictionary<long,List<Vector3D>>();Dictionary<long,List<
Vector3D>>Ǘ=new Dictionary<long,List<Vector3D>>();Dictionary<long,List<Vector3D>>Ȇ;int ʑ;public int ʏ;public ď(){Ȇ=Ǘ;}public
List<Vector3D>ʎ(long Ć){if(Ȇ.ContainsKey(Ć))return Ȇ[Ć];else return null;}public bool ʍ(long Ć,MatrixD ʇ,Vector3D ʆ,Vector3D
ʌ,bool ʋ){List<Vector3D>ʊ;if(ʋ){if(!Ǘ.ContainsKey(Ć))Ǘ.Add(Ć,new List<Vector3D>());ʊ=Ǘ[Ć];}else{if(!ä.ContainsKey(Ć))ä.
Add(Ć,new List<Vector3D>());ʊ=ä[Ć];}var ȿ=Vector3D.TransformNormal(ʌ-ʆ,MatrixD.Transpose(ʇ));if(!ʋ||!ʊ.Any(º=>(ȿ-º).
LengthSquared()<Variables.Get<float>("squared-offset-filter"))){ʊ.Add(ȿ);return true;}else{if(ʋ&&(ś.G-ʑ>600)){ʑ=ś.G;ʊ.Clear();}}
return false;}public IEnumerable<Vector3D>ʉ(long Ć,MatrixD ʇ,Vector3D ʆ){if(Ȇ.ContainsKey(Ć))foreach(var ʈ in Ȇ[Ć])yield
return Vector3D.TransformNormal(ʈ,ʇ)+ʆ;}public Vector3D ɔ(long Ć,MatrixD ʇ,Vector3D ʆ,bool ʅ=false){if(Ȇ.ContainsKey(Ć)&&Ȇ[Ć].
Any()){var ʐ=Math.Min(Ȇ[Ć].Count-1,ʏ);if(ʅ){var Ƭ=Ȇ[Ć][Math.Max(ʐ-1,0)];Ȇ[Ć].RemoveAt(ʐ);return Vector3D.TransformNormal(Ƭ,
ʇ)+ʆ;}return Vector3D.TransformNormal(Ȇ[Ć][ʐ],ʇ)+ʆ;}else return ʆ;}public void ʒ(){if(Ȇ.Any()){var ʥ=Ȇ.First().Value;for(
int Z=0;Z<ʥ.Count;Z++){if(Z==ʏ){var ʣ=Z+1;if(ʣ>=ʥ.Count)ʣ=0;ʏ=ʣ;return;}}}else ʏ=0;}public void ʢ(){if(Ȇ==Ǘ)Ȇ=ä;else Ȇ=Ǘ;}
public void ʡ(){Ȇ=ä;}}class ʠ{public string ʟ;ˡ ʞ;public ˡ ʝ=>ʞ??(ʞ=new ˡ(ʟ+"--listener",60));public int ʜ;public Action<
MyIGCMessage,ˡ>ʛ;public long ʚ;public Func<ˡ,bool>ʤ;public bool ʙ;ɰ ʘ;public ʠ(string ɷ,bool ʗ,ɰ Į,Func<ˡ,bool>ʖ=null){ʙ=ʗ;ʟ=ɷ;ʤ=ʖ;ʛ
=(ą,ª)=>{var ĉ=(MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>)ą.Data;ª.Ί(ą.Source,ĉ,ś.G)
;};ʘ=Į;}public ʠ ʕ(Action<MyIGCMessage,ˡ>ʔ){ʛ=ʔ;return this;}int ʓ;Action ʄ;public void ɵ(){ʘ.ɾ(this);ʓ++;ʘ.ɼ(this);}
public void ɣ(long?ȃ,long ɴ){ʚ=ȃ??ʚ;ɵ();if(ɴ!=0){if(ʘ.ɨ.IsEndpointReachable(ɴ))ʘ.ɨ.SendUnicastMessage(ɴ,"apck.unicast.t+",new
MyTuple<string,long>(ʟ,ʚ));else{bool ɳ=false;ś.œ($"Endpoint {ɴ} is not reachable");}}else{if(!ʘ.ɽ(ʟ,ʚ))ʘ.ɹ(this);else ś.œ(
$"Already subscribed to unicast for DataId {ʚ}");}}public void ɲ(){ś.œ($"IgcL '{ʞ.ǿ}' Finalizer");while(ʓ>0)ɱ();}public void ɱ(){if(ʓ>0){ʘ.ɻ(ʟ);if(--ʓ==0){ś.œ(
$"	IgcL '{ʟ}' kill, clearing TT Data");ʞ=null;ʘ.ɿ(this);if(!ʘ.ɽ(ʟ,ʚ))ʄ?.Invoke();}}}}class ɰ{HashSet<ʠ>ɯ=new HashSet<ʠ>();class ɮ{public string ɭ;public int
ɬ;public List<MyIGCMessage>ɫ=new List<MyIGCMessage>();public IMyBroadcastListener ɪ;}Dictionary<string,ɮ>ɩ=new Dictionary
<string,ɮ>();public IMyIntergridCommunicationSystem ɨ;public ɰ(IMyIntergridCommunicationSystem ĝ){ɨ=ĝ;}public void ɧ(List
<MyIGCMessage>ɦ){foreach(var ɥ in ɩ.Values){ɥ.ɫ.Clear();if(ɥ.ɪ!=null){while(ɥ.ɪ.HasPendingMessage)ɥ.ɫ.Add(ɥ.ɪ.
AcceptMessage());}else{foreach(var ċ in ɦ){if(ċ.Tag==ɥ.ɭ)ɥ.ɫ.Add(ċ);}}ś.Ŕ($"CHNL:{ɥ.ɭ}({ɥ.ɫ.Count})");}foreach(var ɥ in ɯ){if(!ɩ.
ContainsKey(ɥ.ʟ))ś.Ŕ($"{ɥ.ʟ}(CHANNEL ERROR!)");else{if(ɥ.ʛ==null)continue;var ɤ=ɩ[ɥ.ʟ];foreach(var Ė in ɤ.ɫ){ś.Ŕ($"  {Ė.Source}");ɥ
.ʛ(Ė,ɥ.ʝ);if((ɥ.ʤ==null)||ɥ.ʤ(ɥ.ʝ)){ɥ.ʜ=ś.G;break;}}}}ɺ.Clear();foreach(var ɥ in ʀ){ɯ.Add(ɥ);}ʀ.Clear();foreach(var ɥ in
ʁ){ɯ.Remove(ɥ);}ʁ.Clear();}public void ʃ(ʠ ɥ,ref List<MyIGCMessage>ʂ){ʂ.Clear();if(ɩ.ContainsKey(ɥ.ʟ))ʂ.AddRange(ɩ[ɥ.ʟ].ɫ
);}List<ʠ>ʁ=new List<ʠ>();List<ʠ>ʀ=new List<ʠ>();public void ɿ(ʠ ɥ){ʁ.Add(ɥ);}public void ɾ(ʠ ɥ){ʀ.Add(ɥ);}public bool ɽ(
string ɷ,long ȃ){return ɯ.Any(º=>º.ʟ==ɷ&&º.ʚ==ȃ);}public void ɼ(ʠ ɥ){var ɷ=ɥ.ʟ;if(!ɩ.ContainsKey(ɷ)){ɩ.Add(ɷ,new ɮ(){ɭ=ɷ,ɪ=ɥ.ʙ
?ɨ.RegisterBroadcastListener(ɷ):null,ɬ=1});}else ɩ[ɷ].ɬ++;ś.œ($"Q {ɷ} enable, refs: {ɩ[ɷ].ɬ}, DataId: {ɥ.ʚ}");}public
void ɻ(string ɷ){if(ɩ.ContainsKey(ɷ)){ś.œ($"Q {ɷ} disable, refs: {ɩ[ɷ].ɬ-1}");if(--ɩ[ɷ].ɬ<=0){ś.œ($"	Q {ɷ} kill");if(ɩ[ɷ].ɪ
!=null)ɨ.DisableBroadcastListener(ɩ[ɷ].ɪ);else ɨ.SendBroadcastMessage("apck.unicast.closed",ɷ);ɩ.Remove(ɷ);}}}HashSet<
string>ɺ=new HashSet<string>();public void ɹ(ʠ ɥ){if(ɺ.Add(ɥ.ʟ+ɥ.ʚ))ɨ.SendBroadcastMessage($"apck.unicast.whohas",new MyTuple<
string,long>(ɥ.ʟ,ɥ.ʚ));}public void ɸ(string ɷ,byte ɶ,string ǌ,Vector3D U,Vector3D Ə,float S,string ˣ){ɨ.SendBroadcastMessage(
$"apck.unicast.whohas+predicate",new MyTuple<string,string,MyTuple<string,byte,Vector3D,Vector3D,float>>(ɷ,ˣ,new MyTuple<string,byte,Vector3D,Vector3D,
float>(ǌ,ɶ,U,Ə,S)));}}class ˡ{public long K;public string ǿ;public long ȁ;public Vector3D?Ǿ{get;private set;}public Vector3D?
M;public Vector3D?ˠ;public MatrixD?ˑ;public BoundingBoxD?ː;public int?ˏ;public MyDetectedEntityType?ˎ{get;set;}public
delegate void ˍ();public event ˍ ˌ;public long ˋ;public ˡ(string Ŭ,int?ˊ){ǿ=Ŭ;ˏ=ˊ;K=ś.G;}public void ˉ(Vector3D ƞ){Ǿ=ƞ;K=ś.G;}
public bool ˈ=>Ǿ.HasValue&&!ˇ;public bool ˇ=>ˏ.HasValue&&(ś.G-K>ˏ);public enum ˆ:byte{ˁ=1,ˢ=2,ˀ=4}bool ˤ(ˆ Ύ,ˆ Ό){return(Ύ&Ό)
==Ό;}public void Ί(long ɴ,MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>Ή,int Έ){var Ά=Ή.
Item1;ǿ=Ά.Item1;ȁ=Ά.Item2;ˎ=(MyDetectedEntityType)Ά.Item3;ˆ ͽ=(ˆ)Ά.Item4;var ͼ=Έ-K;var U=Ή.Item2;if(ˤ(ͽ,ˆ.ˁ)){var ͻ=Ή.Item3;
if(!M.HasValue)M=ͻ;if(ͼ>0)ˠ=(ͻ-M.Value)*60/ͼ;M=ͻ;U+=ͻ*ś.ŗ;}ˉ(U);if(ˤ(ͽ,ˆ.ˢ))ˑ=Ή.Item4;if(ˤ(ͽ,ˆ.ˀ))ː=Ή.Item5;ˋ=ɴ;}public
void ͺ(MyDetectedEntityInfo ī,Vector3D U){M=ī.Velocity;ː=ī.BoundingBox;ˑ=ī.Orientation;ȁ=ī.EntityId;ˎ=ī.Type;ˉ(U);}public
MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>Ǽ(){var ǻ=0|(M.HasValue?1:0)|(ˑ.HasValue?2:0)|(ː.
HasValue?4:0);var º=new MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>(new MyTuple<string,long,
byte,byte>(ǿ,ȁ,(byte)MyDetectedEntityType.LargeGrid,(byte)ǻ),Ǿ.Value,M??Vector3D.Zero,ˑ??MatrixD.Identity,ː??new
BoundingBoxD());return º;}public void Ͷ(){Ǿ=null;M=null;ˑ=null;ː=null;ˌ?.Invoke();}}List<Ȁ>ʹ=new List<Ȁ>();Ͳ ͳ;class Ͳ{
IMyIntergridCommunicationSystem ü;IMyBroadcastListener ͱ;IMyBroadcastListener Ͱ;IMyBroadcastListener ˮ;List<Ȁ>ˬ;public enum ͷ{ʿ,ʸ,ʮ,ʭ}public Ͳ(
IMyIntergridCommunicationSystem ĝ,List<Ȁ>ʬ){ü=ĝ;ͱ=ĝ.RegisterBroadcastListener("apck.unicast.closed");Ͱ=ĝ.RegisterBroadcastListener(
"apck.unicast.whohas");ˮ=ĝ.RegisterBroadcastListener("apck.unicast.whohas+predicate");ˬ=ʬ;Ŗ();}List<Ȁ>ʫ=new List<Ȁ>();Random ʪ=new Random();
int ʩ;List<MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>>ʨ=new List<MyTuple<MyTuple<string
,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>>();void Ŗ(){var ʧ=new ʻ();ʧ.ʹ=(ą,Ć)=>{if(!ʶ.ContainsKey(Ć))ʶ.Add
(Ć,new HashSet<long>());ʶ[Ć].Add(ą);};ʧ.ʺ=(ą,Ć)=>{if(Ć==0){foreach(var đ in ʶ)đ.Value.Remove(ą);}else ʶ[Ć].Remove(ą);};ʧ.
ɹ=(Ć)=>ˬ.Any(º=>º.ā==Ć);ʧ.ʾ=(ʦ,đ,U,Ə,S)=>{var ʯ=(ͷ)đ;ʫ.Clear();foreach(var É in ˬ){bool ʰ=É.Ǿ.HasValue&&ʴ(U,Ə,S,É.Ǿ.Value
);if(ʰ&&(ʦ==null||(É.ǽ.Value.Type.ToString()==ʦ)))ʫ.Add(É);}if(ʫ.Count>0){if(ʯ==ͷ.ʸ)return ʫ.OrderBy(º=>(U-º.Ǿ.Value).
LengthSquared()).First().ā;else{int Z=0;if(ʯ==ͷ.ʮ)Z=ʪ.Next(ʫ.Count);if(ʯ==ͷ.ʭ)Z=ʩ++%ʫ.Count;return ʫ[Z].ā;}}return-1;};ʧ.ʷ=()=>{ʨ.
Clear();foreach(var ª in ˬ){var ʽ=new Ȃ{ȁ=ª.ā,Ǿ=ª.Ǿ.Value};ü.SendBroadcastMessage("tgp.global.gridsense.offer",ʽ.Ǽ());if(ʶ.
ContainsKey(ª.ā)){var ʼ=ʶ[ª.ā];if(ʼ.Count>0){ś.Ŕ($"Tg: {ª.ā.ToString().Substring(0,4)}...: {ʼ.Count}");var ĉ=ª.Ǽ();foreach(var đ in
ʼ){ü.SendUnicastMessage(đ,"tgp.local.gridsense.update",ĉ);}}else ʶ.Remove(ª.ā);}ʨ.Add(ª.Ǽ());}if(ʨ.Count>0)ü.
SendBroadcastMessage("tgp.global.gridsense.batch",ʨ.ToImmutableArray());};ʵ.Add("tgp.local.gridsense.update",ʧ);}public void Ǭ(List<
MyIGCMessage>ʱ){ʲ(ʱ);}public class ʻ{public Action<long,long>ʺ;public Func<long,bool>ɹ;public Func<string,byte,Vector3D,Vector3D,
float,long>ʾ;public Action<long,long>ʹ;public Action ʷ;}Dictionary<long,HashSet<long>>ʶ=new Dictionary<long,HashSet<long>>();
Dictionary<string,ʻ>ʵ=new Dictionary<string,ʻ>();public static bool ʴ(Vector3D U,Vector3D Ə,float S,Vector3D ʳ){bool ƴ=false;if(Ə
!=Vector3D.Zero){if(Vector3D.Dot(Ə,Vector3D.Normalize(ʳ-U))>S)ƴ=true;}else{if(U!=Vector3D.Zero){if(Vector3D.
DistanceSquared(ʳ,U)<S*S)ƴ=true;}else ƴ=true;}return ƴ;}public void ʲ(List<MyIGCMessage>ʱ){while(ͱ.HasPendingMessage){var ċ=ͱ.
AcceptMessage();var Ȅ=(string)ċ.Data;if(ʵ.ContainsKey(Ȅ))ʵ[Ȅ].ʺ.Invoke(ċ.Source,0);}while(Ͱ.HasPendingMessage){var ċ=Ͱ.AcceptMessage(
);var Ó=(MyTuple<string,long>)ċ.Data;if(ʵ.ContainsKey(Ó.Item1)&&ʵ[Ó.Item1].ɹ.Invoke(Ó.Item2))ü.SendUnicastMessage(ċ.
Source,"apck.unicast.ihave",Ó);}while(ˮ.HasPendingMessage){var ċ=ˮ.AcceptMessage();var Ó=(MyTuple<string,string,MyTuple<string
,byte,Vector3D,Vector3D,float>>)ċ.Data;if(ʵ.ContainsKey(Ó.Item1)&&ʵ[Ó.Item1].ʾ!=null){ś.œ(
$"{Ó.Item1}: whohas with predicate");var Ć=ʵ[Ó.Item1].ʾ.Invoke(Ó.Item3.Item1,Ó.Item3.Item2,Ó.Item3.Item3,Ó.Item3.Item4,Ó.Item3.Item5);if(Ć>0){ś.œ(
$"Offering target {Ć} to {ċ.Source} with callback {Ó.Item2}");ü.SendUnicastMessage(ċ.Source,"apck.unicast.ihave+callback",new MyTuple<long,string>(Ć,Ó.Item2));}}}foreach(var ċ in ʱ
){if(ċ.Tag.Contains("apck.unicast.t")){var Ó=(MyTuple<string,long>)ċ.Data;var Ȅ=Ó.Item1;var ȃ=Ó.Item2;if(ʵ.ContainsKey(Ȅ)
){ś.œ($"{Ȅ}: {ċ.Tag} dataId {ȃ}");var đ=ʵ[Ȅ];if(ċ.Tag=="apck.unicast.t+")đ.ʹ.Invoke(ċ.Source,ȃ);else if(ċ.Tag==
"apck.unicast.t-")đ.ʺ.Invoke(ċ.Source,ȃ);}}}foreach(var º in ʵ)º.Value.ʷ();}public struct Ȃ{public long ȁ;public Vector3D Ǿ;public
MyTuple<long,Vector3D>Ǽ(){return new MyTuple<long,Vector3D>(ȁ,Ǿ);}}}struct Ȁ{public long ā;public string ǿ;public Vector3D?Ǿ;
public Vector3D?M;public MyDetectedEntityInfo?ǽ;public MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,
BoundingBoxD>Ǽ(){var ǻ=0|(M.HasValue?1:0)|(ǽ.HasValue?2:0)|(ǽ.HasValue?4:0);var º=new MyTuple<MyTuple<string,long,byte,byte>,
Vector3D,Vector3D,MatrixD,BoundingBoxD>(new MyTuple<string,long,byte,byte>(ǿ,ā,(byte)(ǽ?.Type??MyDetectedEntityType.LargeGrid),(
byte)ǻ),Ǿ.Value,M??Vector3D.Zero,ǽ?.Orientation??MatrixD.Identity,ǽ?.BoundingBox??new BoundingBoxD());return º;}}ȅ Ǻ;class ȅ
{class ǹ{public List<IMyWarhead>ȇ;public bool ȍ{get;set;}public Vector3D ȝ{get;set;}public Vector3D Ȝ{get;set;}public
IMyTerminalBlock ț{get;set;}public Vector3D Ț;public float ș=8;public float Ș=2.5f;public string ǿ;public List<IMyTerminalBlock>ȗ=new
List<IMyTerminalBlock>();public Vector3D Ȗ{get;set;}Dictionary<float,List<IMyTimerBlock>>ȕ=new Dictionary<float,List<
IMyTimerBlock>>();public void Ȕ(){(ț as IMyMotorStator)?.Detach();if(ț is IMyShipMergeBlock)((IMyShipMergeBlock)ț).Enabled=false;}
bool Ȟ(){if(ț is IMyShipMergeBlock)return((IMyShipMergeBlock)ț).IsConnected;if(ț is IMyMotorStator)return((IMyMotorStator)ț)
.IsAttached;return true;}void ȓ(){ȇ.ForEach(º=>º.Detonate());ś.œ($"Closing shot grid {ț.CubeGrid.DisplayName}");ȍ=true;}
enum ȑ{Ȑ,ȏ,Ȏ,ȍ}ȑ Ȍ=ȑ.Ȑ;bool ȋ;public bool Ȋ=>Ȍ==ȑ.Ȑ;public void ƥ(Vector3D?ȉ,ȅ Ȉ){switch(Ȍ){case ȑ.Ȑ:if(!Ȟ()){if(ȉ.HasValue)
{Ȍ=ȑ.ȏ;Ț=ȉ.Value;Ȗ=ț.GetPosition();}else Ȍ=ȑ.ȍ;Ȉ.Ƕ();}break;case ȑ.ȏ:Ȍ=ȑ.Ȏ;ȇ=ȗ.Where(ƴ=>ƴ is IMyWarhead&&ƴ.CubeGrid==ț.
CubeGrid).Cast<IMyWarhead>().ToList();var Ǫ=ȗ.Where(ƴ=>ƴ is IMyTimerBlock&&ƴ.CubeGrid==ț.CubeGrid).Cast<IMyTimerBlock>().ToList(
);ś.œ($"Init shot: timers {Ǫ.Count}, wh {ȇ.Count}, cube grid: {ț.CubeGrid.DisplayName}");foreach(var ǩ in Ǫ){foreach(var
Ǩ in ɐ(ǩ)){float ǧ;if(float.TryParse(Ǩ,out ǧ)){if(!ȕ.ContainsKey(ǧ))ȕ.Add(ǧ,new List<IMyTimerBlock>());ś.œ(
$"	-added timer {ǩ.CustomName} for d {ǧ}");ȕ[ǧ].Add(ǩ);}}}break;case ȑ.Ȏ:var Ǧ=ț.GetPosition();if(ȝ!=Vector3D.Zero&&Ǧ!=Vector3D.Zero){Vector3D ǥ=ȝ-Ǧ;ś.Ŕ(
$"V: {ǥ.Length()*60f:f2}");}ȝ=Ǧ;var Ǥ=(Ȗ-ț.GetPosition()).Length();if(!ȋ&&Ǥ>50){ȋ=true;ȇ.ForEach(º=>º.IsArmed=true);}var ǣ=Ț-ț.GetPosition();var
Ó=ǣ.Length();foreach(var É in ȕ){if(É.Value.Count>0&&É.Key>Ó){É.Value.ForEach(º=>º.Trigger());É.Value.Clear();}}var Ǣ=(Ȗ-
Ț).Length();bool ǡ=false;if(Ó<Ǣ*0.2){if(Ó>Ȝ.LengthSquared()+10)ǡ=true;}Ȝ=ǣ;if((Ǥ>50)&&((Ó<ș)||((Ǥ>Ǣ)&&ǡ))){ȓ();}if(Ǥ>5000
){ȓ();}if(Ȉ.ǜ==0)Ȉ.ǜ=Ó/Ǣ;break;case ȑ.ȍ:break;}}}List<ǹ>Ǡ=new List<ǹ>();IMyGridTerminalSystem ǟ;public ȅ(
IMyGridTerminalSystem Ǟ){ǟ=Ǟ;}public Action<IMyCubeGrid>ǝ;public double ǜ;ǹ Ǜ;public int ǚ(Vector3D Ǚ){var ǘ=new List<IMyTerminalBlock>();ǟ.
GetBlocksOfType(ǘ,º=>º.CustomName.Contains("[bomb]"));int ǫ=0;foreach(var Ǹ in ǘ.OrderByDescending(º=>Vector3D.Distance(º.GetPosition()
,Ǚ))){if((Ǹ is IMyShipMergeBlock||Ǹ is IMyMotorStator)&&!Ǡ.Any(º=>º.ț==Ǹ)){var Ƿ=new ǹ{ț=Ǹ};ǟ.GetBlocks(Ƿ.ȗ);Ǡ.Add(Ƿ);var
ǆ=ɏ(Ǹ);if(ǆ.ContainsKey("detonation"))Ƿ.ș=float.Parse(ǆ["detonation"]);if(ǆ.ContainsKey("name"))Ƿ.ǿ=ǆ["name"];if(ǆ.
ContainsKey("precision"))Ƿ.Ș=float.Parse(ǆ["precision"]);ś.œ($"Added managed shot root '{Ǹ.CustomName}' of type {Ǹ.GetType().Name}"
);ǫ++;Ǜ=Ǜ??Ƿ;}}return ǫ;}public void Ƕ(){if(Ǜ?.Ȋ!=true)Ǜ=null;int?ǵ=null;bool Ǵ=false;for(int Z=0;Z<Ǡ.Count;Z++){if(Ǡ[Z].
Ȋ){ǵ=ǵ??Z;if(Ǜ==null||Ǵ){Ǜ=Ǡ[Z];return;}if(Ǜ==Ǡ[Z]){Ǵ=true;}}}if(ǵ.HasValue)Ǜ=Ǡ[ǵ.Value];}public void ǳ(string[]Ƙ){var Z=
Ƙ[2];if(Z=="first"){Ǜ?.Ȕ();}else{foreach(var đ in Ǡ.Where(º=>º.Ȋ&&(Z=="*"||º.ț.CustomName.ToLower().Contains(Z.ToLower())
)))đ.Ȕ();}}int ǲ;public MyTuple<double,double>?Ǳ(Vector3D ň,Vector3D ƌ,Vector3D ǰ){if(Ǜ==null)return null;var ũ=ǰ-ň;var Ó
=ũ.Length();var ǯ=Vector3D.Normalize(ƌ-ň);var ŕ=Ǜ.Ș;var Ǯ=Vector3D.Dot(ǯ,ũ/Ó);if(Ǯ<0.9)return new MyTuple<double,double>(
-1,ŕ);var ǭ=Math.Acos(Ǯ);var Ȓ=Math.Tan(ǭ)*Ó;if(Ȓ<ŕ&&ǲ<ś.G){ś.œ(
$"Auto dropping, tPos: {ǰ.ToString("f2")}, origin: {ň.ToString("f2")}, pip: {ƌ.ToString("f2")}");Ǜ.Ȕ();ǲ=ś.G+Variables.Get<int>("autodrop-timeout");ś.œ($"Next drop possible at tick {ǲ}");}return new MyTuple<double,
double>(Ȓ,ŕ);}public void Ǭ(Vector3D?ȟ){ǜ=0;foreach(var Ƚ in Ǡ){Ƚ.ƥ(ȟ,this);if(Ƚ.ȍ)ǝ?.Invoke(Ƚ.ț.CubeGrid);}Ǡ.RemoveAll(S=>S.ȍ
);}List<Vector3D>Ɍ=new List<Vector3D>();public List<Vector3D>ɋ(){Ɍ.Clear();foreach(var Ɋ in Ǡ){if(!Ɋ.Ȋ)Ɍ.Add(Ɋ.ȝ);}return
Ɍ;}public void ɉ(List<MySprite>Ɉ,Color ɇ,float ƫ,Vector2 ƞ,IMyCubeBlock Ɇ){if(Ǜ?.ǿ!=null){var Ʌ=MySprite.CreateText(Ǜ?.ǿ,
"Debug",ɇ,0.6f*ύ,TextAlignment.CENTER);Ʌ.Position=new Vector2(ƞ.X+ƫ/2f,ƞ.Y-ƫ*0.28f);Ɉ.Add(Ʌ);}var Ʉ=MatrixD.Transpose(Ɇ.
WorldMatrix);var Ƀ=0d;var ɂ=0d;var Ɂ=double.MaxValue;foreach(var Ƚ in Ǡ){if(Ƚ.Ȋ){var ȼ=Vector3D.Rotate(Ƚ.ț.GetPosition()-Ɇ.
GetPosition(),Ʉ);Ƀ=Math.Max(Ƀ,Math.Abs(ȼ.X));ɂ=Math.Max(ɂ,Math.Abs(ȼ.Z));Ɂ=Math.Min(ȼ.Z,Ɂ);}}var ɀ=(float)(ƫ/Math.Max(Ƀ,ɂ))/2f;var
ȿ=new Vector2(0,-(float)Ɂ*ɀ);ȿ.Y-=ƫ/2f;ȿ+=Vector2.One*ƫ/2f;var Ⱦ=new MySprite(SpriteType.TEXTURE,"SquareSimple",color:ɇ);
foreach(var Ƚ in Ǡ){if(Ƚ.Ȋ){var ȼ=Vector3D.Rotate(Ƚ.ț.GetPosition()-Ɇ.GetPosition(),Ʉ);Ⱦ.Position=ƞ+new Vector2((float)ȼ.X,(
float)ȼ.Z)*ɀ+ȿ;Ⱦ.Size=Vector2.One*1.5f*ɀ;Ⱦ.Color=Ƚ==Ǜ?Color.Blue:ɇ;Ɉ.Add(Ⱦ);}}}public Vector3D?Ȼ(){return Ǜ?.ț.GetPosition();
}}class ɍ{public bool Ⱥ=true;Vector3 Ɏ;Vector3 ɢ;Vector3 ɠ;Vector3 ɟ;float ɞ=20f;float ɝ=1f;float ɜ=0.7f;float ɛ=2f;float
ɚ=0.5f;public void ə(Vector3 ɘ){Ɏ=ɘ;}public void ƥ(float ɗ,out Vector3 ɡ){var ɖ=Ɏ-ɢ;if(ɖ.LengthSquared()>ɛ*ɛ){ɖ.Normalize
();ɖ*=ɛ;}ɢ=Ɏ;ɟ+=ɖ*ɗ;var ɕ=-ɟ*ɞ/ɝ;ɠ+=ɕ*ɗ;ɟ+=ɠ*ɗ;ɠ*=ɜ;ɡ=ɔ(ɟ);}Vector3 ɔ(Vector3 ɓ){var ɒ=ɓ.Length();if(ɒ<=0.00001f)return ɓ
;var ɑ=ɚ*ɒ/(ɒ+2);return ɑ*ɓ/ɒ;}}static IEnumerable<string>ɐ(IMyTerminalBlock ƴ){return ƴ.CustomName.Trim().Split('[').
Select(U=>U.Trim(']'));}static Dictionary<string,string>ɏ(IMyTerminalBlock ƴ){return ɐ(ƴ).Where(º=>º.Contains('=')).
ToDictionary(º=>º.Split('=')[0],º=>º.Split('=')[1]);}static List<Ř>ȹ<Ř>(List<IMyTerminalBlock>Ʈ,string Ʒ=null)where Ř:class,
IMyTerminalBlock{var Ǒ=Ʈ.Where(ƴ=>ƴ is Ř&&((Ʒ==null)||ƴ.CustomName.Contains(Ʒ))).Cast<Ř>().ToList();return Ǒ;}static float ȭ(float Ȭ,
IMyCameraBlock D,IMyShipController Ü,bool ȫ){var Ó=Ȭ;double Ȫ;Vector3D ȩ;if(Ü.TryGetPlanetElevation(MyPlanetElevation.Surface,out Ȫ)){
Ü.TryGetPlanetPosition(out ȩ);var Ȩ=(Ü.GetPosition()-ȩ).Length();if(ȫ)Ȫ+=500;else Ȫ-=500;var ȧ=new BoundingSphereD(ȩ,Ȩ-Ȫ)
;var Ƃ=ȧ.Intersects(new RayD(D.GetPosition(),D.WorldMatrix.Forward));if(Ƃ.HasValue&&(Ƃ>0)&&Ƃ<Ó){ś.œ(
$"Cast distance adjusted from {Ó:f2} to {Ƃ.Value:f2} due to planet proximity");return(float)Ƃ.Value;}}return Ȭ;}void Ȧ(float ȥ,Vector2 ƞ,float é,float Ȥ,Color ȣ,int Ȣ,List<MySprite>ȡ){for(int Ʒ=0;Ʒ
<Ȣ;Ʒ++){if(Ʒ<ȥ*(Ȣ-2)){var Ƞ=new MySprite(SpriteType.TEXTURE,"Circle",color:ȣ);Ƞ.Position=ƞ+new Vector2((int)(é*Math.Cos(2
*3.14/(Ȣ-1)*Ʒ)),-(int)(é*Math.Sin(2*3.14/(Ȣ-1)*Ʒ)));Ƞ.Size=Vector2.One*Ȥ;ȡ.Add(Ƞ);}}}void Ȯ(Vector2 ƞ,Vector2 ƫ,Color ȣ,
Color ȷ,float ȥ,List<MySprite>ȡ){MySprite ȶ;var Ŝ=ƞ-ƫ*0.5f;var Á=ƞ+ƫ*0.5f;var ƴ=new Vector2(ƞ.X+ƫ.X*0.5f,ƞ.Y-ƫ.Y*0.5f);var Ó=
new Vector2(ƞ.X-ƫ.X*0.5f,ƞ.Y+ƫ.Y*0.5f);Ԓ(Ŝ,ƴ,ȣ,1f,out ȶ);ȡ.Add(ȶ);Ԓ(Á,ƴ,ȣ,1f,out ȶ);ȡ.Add(ȶ);Ԓ(Ó,Ŝ,ȣ,1f,out ȶ);ȡ.Add(ȶ);Ԓ(Ó
,Á,ȣ,1f,out ȶ);ȡ.Add(ȶ);var ȵ=new MySprite(SpriteType.TEXTURE,"SquareSimple",color:ȷ);ȵ.Position=ƞ;ȵ.Size=new Vector2(ƫ.X
*0.98f,ƫ.Y*0.9f);var ȸ=new Vector2(ƫ.X*0.98f,ƫ.Y*0.9f);var ȴ=ƞ;ȴ.X-=ȸ.X/2*(1-ȥ);ȸ.X*=ȥ;ȵ.Position=ȴ;ȵ.Size=ȸ;ȡ.Add(ȵ);}
static long ȳ(IMyShipController Ü,float Ȳ,List<IMyCameraBlock>ß,Func<MyDetectedEntityInfo,bool>ȱ){if(Ü==null)return 0;RayD Ȱ;
var ȯ=ß.FirstOrDefault(º=>º.IsActive);if(ȯ!=null)Ȱ=new RayD(ȯ.WorldMatrix.Translation,ȯ.WorldMatrix.Forward);else Ȱ=new
RayD(Ü.WorldMatrix.Translation,Ü.WorldMatrix.Forward);var À=Ȱ.Position+Ȱ.Direction*Ȳ;var D=ȯ?.CanScan(À)==true?ȯ:ß.
FirstOrDefault(º=>º.CanScan(À));Ȳ=ȭ(Ȳ,D,Ü,true);À=Ȱ.Position+Ȱ.Direction*Ȳ;if(D!=null){var Ž=D.WorldMatrix;var Ú=D.Raycast(À);if(!Ú.
IsEmpty()){return ȱ(Ú)?Ú.EntityId:0;}}return 0;}