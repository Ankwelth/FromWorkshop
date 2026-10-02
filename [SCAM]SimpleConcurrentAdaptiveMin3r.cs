

string Ver = "0.9.308";

static bool WholeAirspaceLocking = false;
static long DbgIgc = 0;
static bool IsLargeGrid;
static double Dt = 1 / 60f;
static float MAX_SP = 104.38f;
const float G = 9.81f;
const string DockHostTag = "docka-min3r";
const string ForwardGyroTag = "forward-gyro";
bool ClearDocksOnReload = false;

static float StoppingPowerQuotient = 0.5f;
static bool MaxBrakeInProximity = true;
static bool MaxAccelInProximity = false;
static bool MoreRejectDampening = true;

static string LOCK_NAME_GeneralSection = "general";
//static string LOCK_NAME_ForceFinishSection = "general";
static string LOCK_NAME_ForceFinishSection = "force-finish";
const string INERT_CMD = "command:pillock-mode:Inert";

Action<IMyTextPanel> outputPanelInitializer = x =>
{
	x.ContentType = ContentType.TEXT_AND_IMAGE;
};

Action<IMyTextPanel> logPanelInitializer = x =>
{
	x.ContentType = ContentType.TEXT_AND_IMAGE;
	x.FontColor = new Color(r: 0, g: 255, b: 116);
	x.FontSize = 0.65f;
};

static class Variables
{
	static Dictionary<string, object> v = new Dictionary<string, object> {
		{ "depth-limit", new Variable<float> { value = 80, parser = s => float.Parse(s) } },
		{ "max-generations", new Variable<int> { value = 7, parser = s => int.Parse(s) } },
		{ "circular-pattern-shaft-radius", new Variable<float> { value = 3.6f, parser = s => float.Parse(s) } },
		{ "echelon-offset", new Variable<float> { value = 12f, parser = s => float.Parse(s) } },
		{ "getAbove-altitude", new Variable<float> { value = 20, parser = s => float.Parse(s) } },
		{ "skip-depth", new Variable<float> { value = 0, parser = s => float.Parse(s) } },
		{ "ct-raycast-range", new Variable<float> { value = 1000, parser = s => float.Parse(s) } },
		{ "preferred-container", new Variable<string> { value = "", parser = s => s } },
		{ "group-constraint", new Variable<string> { value = "general", parser = s => s } },
		{ "logger-char-limit", new Variable<int> { value = 5000, parser = s => int.Parse(s) } },
		{ "cargo-full-factor", new Variable<float> { value = 0.8f, parser = s => float.Parse(s) } },
		{ "battery-low-factor", new Variable<float> { value = 0.2f, parser = s => float.Parse(s) } },
		{ "battery-full-factor", new Variable<float> { value = 0.8f, parser = s => float.Parse(s) } },
		{ "gas-low-factor", new Variable<float> { value = 0.2f, parser = s => float.Parse(s) } },
		{ "speed-clear", new Variable<float> { value = 2f, parser = s => float.Parse(s) } },
		{ "speed-drill", new Variable<float> { value = 0.6f, parser = s => float.Parse(s) } },
		{ "roll-power-factor", new Variable<float> { value = 1f, parser = s => float.Parse(s) } },
		// apck
		{ "ggen-tag", new Variable<string> { value = "", parser = s => s } },
		{ "hold-thrust-on-rotation", new Variable<bool> { value = true, parser = s => s == "true" } },
		{ "amp", new Variable<bool> { value = false, parser = s => s == "true" } }
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
class κ{static κ ſ;κ(){}Action<string>ι;Dictionary<string,bool>θ;κ(Dictionary<string,bool>λ,Action<string>μ){ι=μ;θ=λ;}
public static κ ƅ=>ſ;public static void ģ(Dictionary<string,bool>λ,Action<string>μ){if(ſ==null)ſ=new κ(λ,μ);}public void ω(
string ğ,bool ψ){if(θ[ğ]!=ψ)χ(ğ);}public void χ(string ğ){θ[ğ]=!θ[ğ];ι(ğ);}public bool φ(string ğ){return θ[ğ];}public
ImmutableArray<MyTuple<string,string>>υ(){return θ.Select(Ť=>new MyTuple<string,string>("Toggle "+Ť.Key+(Ť.Value?" (off)":" (on)"),
"toggle:"+Ť.Key)).ToImmutableArray();}}bool τ;ς σ;class ς{Dictionary<string,Action<string[]>>ρ;public ς(Dictionary<string,Action<
string[]>>ρ){this.ρ=ρ;}public void π(string Ȇ,string[]ο){this.ρ[Ȇ].Invoke(ο);}}static int ξ;void ν(string ϊ){ξ++;Echo(
"Run count: "+ξ);if(τ&&string.IsNullOrEmpty(ϊ)){τ=false;ϊ=string.Join(",",Me.CustomData.Trim('\n').Split(new[]{'\n'},
StringSplitOptions.RemoveEmptyEntries).Where(ƃ=>!ƃ.StartsWith("//")).Select(ƃ=>"["+ƃ+"]"));}if(!string.IsNullOrEmpty(ϊ)&&ϊ.Contains(":")){
var ρ=ϊ.Split(new[]{"],["},StringSplitOptions.RemoveEmptyEntries).Select(ƃ=>ƃ.Trim('[',']')).ToList();foreach(var á in ρ){
string[]ο=á.Split(new[]{':'},StringSplitOptions.RemoveEmptyEntries);if(ο[0]=="command"){try{this.σ.π(ο[1],ο);}catch(Exception
ex){Ϋ($"Run command '{ο[1]}' failed.\n{ex}");}}if(ο[0]=="toggle"){κ.ƅ.χ(ο[1]);Ϋ(
$"Switching '{ο[1]}' to state '{κ.ƅ.φ(ο[1])}'");}}}}void Ƃ(){ƀ.ƅ.Ƙ();У.Ƃ();}IMyProgrammableBlock ϒ;void ϑ(){if(!string.IsNullOrEmpty(Me.CustomData))τ=true;У.ģ(Echo,
GridTerminalSystem,Me);κ.ģ(new Dictionary<string,bool>{{"adaptive-mining",false},{"adjust-entry-by-elevation",true},{"log-message",false},
{"show-pstate",false},{"suppress-transition-control",false},{"suppress-gyro-control",false},{"damp-when-idle",true},{
"ignore-user-thruster",false},{"cc",true}},ğ=>{switch(ğ){case"log-message":var ϐ=Ё?.Ҟ;if(ϐ!=null)ϐ.CustomData="";break;}});ě.Add("docking",new
Ĕ(1,"docking"));stateWrapper=new StateWrapper(ƃ=>Storage=ƃ);if(!stateWrapper.TryLoad(Storage)){У.Ƌ(
"State load failed, clearing Storage now");stateWrapper.Save();Runtime.UpdateFrequency=UpdateFrequency.None;}GridTerminalSystem.GetBlocksOfType(ͺ,á=>á.
IsSameConstructAs(Me));ͺ.ForEach(á=>á.EnableRaycast=true);IsLargeGrid=Me.CubeGrid.GridSizeEnum==MyCubeSize.Large;this.σ=new ς(new
Dictionary<string,Action<string[]>>{{"set-value",(ˀ)=>Variables.Set(ˀ[2],ˀ[3])},{"add-panel",(ˀ)=>{List<IMyTextPanel>â=new List<
IMyTextPanel>();GridTerminalSystem.GetBlocksOfType(â,é=>é.IsSameConstructAs(Me)&&é.CustomName.Contains(ˀ[2]));var ë=â.FirstOrDefault
();if(ë!=null){У.Ƈ($"Added {ë.CustomName} as GUI panel");outputPanelInitializer(ë);Ъ=ë;}}},{"add-gui-controller",(ˀ)=>{
List<IMyShipController>â=new List<IMyShipController>();GridTerminalSystem.GetBlocksOfType(â,é=>é.IsSameConstructAs(Me)&&é.
CustomName.Contains(ˀ[2]));Ш=â.FirstOrDefault();if(Ш!=null)У.Ƈ($"Added {Ш.CustomName} as GUI controller");}},{"add-logger",(ˀ)=>{
List<IMyTextPanel>â=new List<IMyTextPanel>();GridTerminalSystem.GetBlocksOfType(â,é=>é.IsSameConstructAs(Me)&&é.CustomName.
Contains(ˀ[2]));var ë=â.FirstOrDefault();if(ë!=null){logPanelInitializer(ë);У.Ƅ(ë);У.Ƈ("Added logger: "+ë.CustomName);}}},{
"create-task",(ˀ)=>Ё?.Κ()},{"mine",(ˀ)=>Ё?.Ҷ()},{"skip",(ˀ)=>Ё?.ҵ()},{"set-role",(ˀ)=>ϗ(ˀ[2])},{"low-update-rate",(ˀ)=>Runtime.
UpdateFrequency=UpdateFrequency.Update10},{"create-task-raycast",(ˀ)=>ʹ(ˀ)},{"create-task-gps",(ˀ)=>ˇ(ˀ)},{"force-finish",(ˀ)=>Ё?.Ҍ()},
{"recall",(ˀ)=>ː?.Η()},{"static-dock",(ˀ)=>Ё?.Ҵ(ˀ)},{"set-state",(ˀ)=>Ё?.Ĉ(ˀ[2])},{"halt",(ˀ)=>Ё?.ј()},{
"clear-storage-state",(ˀ)=>stateWrapper?.ClearPersistentState()},{"save",(ˀ)=>stateWrapper?.Save()},{"static-dock-gps",(ˀ)=>{if((Ё!=null)&&(Ё
.Ҟ!=null)){Ё.Ҟ.CustomData="GPS:static-dock:"+(stateWrapper.PState.StaticDockOverride.HasValue?ѕ.х(stateWrapper.PState.
StaticDockOverride.Value):"-")+":";}}},{"dispatch",(ˀ)=>Ё?.Ѡ()},{"global",(ˀ)=>{var ο=ˀ.Skip(2).ToArray();IGC.SendBroadcastMessage(
"miners.command",string.Join(":",ο),TransmissionDistance.TransmissionDistanceMax);Ϋ("broadcasting global "+string.Join(":",ο));σ.π(ο[1],
ο);}},{"get-toggles",(ˀ)=>{IGC.SendUnicastMessage(long.Parse(ˀ[2]),
$"menucommand.get-commands.reply:{string.Join(":",ˀ.Take(3))}",κ.ƅ.υ());}},});}void ϗ(string ϖ){Role ϕ;if(Enum.TryParse(ϖ,out ϕ)){CurrentRole=ϕ;У.Ƈ("Assigned role: "+ϕ);if(ϕ==Role.
Dispatcher){ː=new ˏ(IGC,stateWrapper);var ϔ=new List<IMyShipConnector>();GridTerminalSystem.GetBlocksOfType(ϔ,á=>á.
IsSameConstructAs(Me)&&á.CustomName.Contains(DockHostTag));if(ClearDocksOnReload)ϔ.ForEach(ƭ=>ƭ.CustomData="");ʫ=new ʨ(ϔ,stateWrapper.
PState,GridTerminalSystem);if(stateWrapper.PState.ShaftStates.Count>0){var ϓ=stateWrapper.PState.ShaftStates;ː.Κ(stateWrapper.
PState.shaftRadius.Value,stateWrapper.PState.corePoint.Value,stateWrapper.PState.miningPlaneNormal.Value,stateWrapper.PState.
MaxGenerations,stateWrapper.PState.CurrentTaskGroup);for(int Ť=0;Ť<ː.Ε.Ό.Count;Ť++){ː.Ε.Ό[Ť].ß=(ShaftState)ϓ[Ť];}stateWrapper.PState.
ShaftStates=ː.Ε.Ό.Select(é=>(byte)é.ß).ToList();У.Ƈ($"Restored task from pstate, shaft count: {ϓ.Count}");}ώ("miners",
"dispatcher-change");}else{var â=new List<IMyProgrammableBlock>();GridTerminalSystem.GetBlocksOfType(â,Ϗ=>Ϗ.CustomName.Contains("core")&&Ϗ.
IsSameConstructAs(Me)&&Ϗ.Enabled);ϒ=â.FirstOrDefault();Ё=new ѫ(ϕ,GridTerminalSystem,IGC,stateWrapper,Ŏ,Me);if(ϒ!=null){Ё.ћ(ϒ);}else{Ɩ=new
ƕ(Me,stateWrapper.PState,GridTerminalSystem,IGC,Ŏ);Ё.ћ(Ɩ);Ё.ѿ=new ς(new Dictionary<string,Action<string[]>>{{"create-wp",
(ˀ)=>ǉ(ˀ)},{"set-sp-limit",(ˀ)=>{if(Ɩ.Ƒ?.đ=="Deserialized Behavior")Ɩ.Ƒ.ð=float.Parse(ˀ[2]);}},{"pillock-mode",(ˀ)=>Ɩ?.Ĉ(
ˀ[2])},{"request-docking",(ˀ)=>{У.Ƈ("Embedded lone mode is not supported");}},{"request-depart",(ˀ)=>{У.Ƈ(
"Embedded lone mode is not supported");}}});}ƀ.ƅ.ƞ(3000).Ɯ(()=>{if(Ё.Ɣ.OtherConnector!=null)Ё.ѽ(INERT_CMD,U=>U.Ċ(ApckState.Inert));if(!string.IsNullOrEmpty(
stateWrapper.PState.lastAPckCommand))Ё.ѽ(stateWrapper.PState.lastAPckCommand);});if(ϕ==Role.Lone){Ё.Ѫ=new ˏ(IGC,stateWrapper);}if(ϕ
==Role.Agent){ƀ.ƅ.ƚ(()=>!Ё.ѧ.HasValue).ƞ(1000).Ɯ(()=>ώ("miners.handshake",Variables.Get<string>("group-constraint")));}if(
stateWrapper.PState.miningEntryPoint.HasValue){Ё.ҷ();}}}}static void ύ<Ŧ>(Ŧ ό,IList<Ŧ>á)where Ŧ:class{if((ό!=null)&&!á.Contains(ό))á
.Add(ό);}void ώ<Ŧ>(string ŵ,Ŧ γ){var ϋ=IGC.RegisterBroadcastListener(ŵ);IGC.SendBroadcastMessage(ϋ.Tag,γ,
TransmissionDistance.TransmissionDistanceMax);}void Ϋ(string ˉ){У.Ƈ(ˉ);}
public enum MinerState : byte
{
	Disabled = 0, Idle, GoingToEntry, Drilling, GettingOutTheShaft, GoingToUnload, WaitingForDocking,
	Docking, ReturningToShaft, WaitingForLockInShaft, ChangingShaft, Maintenance, ForceFinish
}

public enum ShaftState { Planned, InProgress, Complete, Cancelled }

Role CurrentRole;
public enum Role : byte { None = 0, Dispatcher, Agent, Lone }

public enum ApckState
{
	Inert, Standby, Formation, DockingAwait, DockingFinal, Brake, CwpTask
}

StateWrapper stateWrapper;
public class StateWrapper
{
	public PersistentState PState { get; private set; }

	public void ClearPersistentState()
	{
		var currentState = PState;
		PState = new PersistentState();
		PState.StaticDockOverride = currentState.StaticDockOverride;
		PState.LifetimeAcceptedTasks = currentState.LifetimeAcceptedTasks;
		PState.LifetimeOperationTime = currentState.LifetimeOperationTime;
		PState.LifetimeWentToMaintenance = currentState.LifetimeWentToMaintenance;
		PState.LifetimeOreAmount = currentState.LifetimeOreAmount;
		PState.LifetimeYield = currentState.LifetimeYield;
	}

	Action<string> stateSaver;
	public StateWrapper(Action<string> stateSaver)
	{
		this.stateSaver = stateSaver;
	}

	public void Save()
	{
		try
		{
			PState.Save(stateSaver);
		}
		catch (Exception ex)
		{
			У.Ƈ("State save failed.");
			У.Ƈ(ex.ToString());
		}
	}

	public bool TryLoad(string serialized)
	{
		PState = new PersistentState();
		try
		{
			PState.Load(serialized);
			return true;
		}
		catch (Exception ex)
		{
			У.Ƈ("State load failed.");
			У.Ƈ(ex.ToString());
		}
		return false;
	}
}

public class PersistentState
{
	public int LifetimeOperationTime = 0;
	public int LifetimeAcceptedTasks = 0;
	public int LifetimeWentToMaintenance = 0;
	public float LifetimeOreAmount = 0;
	public float LifetimeYield = 0;

	// cleared by specific command
	public Vector3D? StaticDockOverride { get; set; }

	// cleared by clear-storage-state (task-dependent)
	public MinerState MinerState = MinerState.Idle;
	public Vector3D? miningPlaneNormal;
	public Vector3D? getAbovePt;
	public Vector3D? miningEntryPoint;
	public Vector3D? corePoint;
	public float? shaftRadius;

	public float? maxDepth;
	public Vector3D? currentWp;
	public float? skipDepth;

	public float? lastFoundOreDepth;
	public float CurrentJobMaxShaftYield;

	public float? minFoundOreDepth;
	public float? maxFoundOreDepth;
	public float? prevTickValCount = 0;

	public int? CurrentShaftId;
	public List<byte> ShaftStates = new List<byte>();
	public int MaxGenerations;
	public string CurrentTaskGroup;

	public string lastAPckCommand;
	// banned directions?

	T ParseValue<T>(Dictionary<string, string> values, string key)
	{
		string res;
		if (values.TryGetValue(key, out res) && !string.IsNullOrEmpty(res))
		{
			if (typeof(T) == typeof(String))
				return (T)(object)res;
			else if (typeof(T) == typeof(int))
				return (T)(object)int.Parse(res);
			else if (typeof(T) == typeof(int?))
				return (T)(object)int.Parse(res);
			else if (typeof(T) == typeof(float))
				return (T)(object)float.Parse(res);
			else if (typeof(T) == typeof(float?))
				return (T)(object)float.Parse(res);
			else if (typeof(T) == typeof(long?))
				return (T)(object)long.Parse(res);
			else if (typeof(T) == typeof(Vector3D?))
			{
				var d = res.Split(':');
				return (T)(object)new Vector3D(double.Parse(d[0]), double.Parse(d[1]), double.Parse(d[2]));
			}
			else if (typeof(T) == typeof(List<byte>))
			{
				var d = res.Split(':');
				return (T)(object)d.Select(x => byte.Parse(x)).ToList();
			}
			else if (typeof(T) == typeof(MinerState))
			{
				return (T)Enum.Parse(typeof(MinerState), res);
			}
		}
		return default(T);
	}

	public PersistentState Load(string storage)
	{
		if (!string.IsNullOrEmpty(storage))
		{
			У.Ƌ(storage);

			var values = storage.Split('\n').ToDictionary(s => s.Split('=')[0], s => string.Join("=", s.Split('=').Skip(1)));

			LifetimeAcceptedTasks = ParseValue<int>(values, "LifetimeAcceptedTasks");
			LifetimeOperationTime = ParseValue<int>(values, "LifetimeOperationTime");
			LifetimeWentToMaintenance = ParseValue<int>(values, "LifetimeWentToMaintenance");
			LifetimeOreAmount = ParseValue<float>(values, "LifetimeOreAmount");
			LifetimeYield = ParseValue<float>(values, "LifetimeYield");

			StaticDockOverride = ParseValue<Vector3D?>(values, "StaticDockOverride");
			MinerState = ParseValue<MinerState>(values, "MinerState");
			miningPlaneNormal = ParseValue<Vector3D?>(values, "miningPlaneNormal");
			getAbovePt = ParseValue<Vector3D?>(values, "getAbovePt");
			miningEntryPoint = ParseValue<Vector3D?>(values, "miningEntryPoint");
			corePoint = ParseValue<Vector3D?>(values, "corePoint");
			shaftRadius = ParseValue<float?>(values, "shaftRadius");

			maxDepth = ParseValue<float?>(values, "maxDepth");
			currentWp = ParseValue<Vector3D?>(values, "currentWp");
			skipDepth = ParseValue<float?>(values, "skipDepth");

			lastFoundOreDepth = ParseValue<float?>(values, "lastFoundOreDepth");
			CurrentJobMaxShaftYield = ParseValue<float>(values, "CurrentJobMaxShaftYield");

			minFoundOreDepth = ParseValue<float?>(values, "minFoundOreDepth");
			maxFoundOreDepth = ParseValue<float?>(values, "maxFoundOreDepth");

			CurrentShaftId = ParseValue<int?>(values, "CurrentShaftId");
			MaxGenerations = ParseValue<int>(values, "MaxGenerations");
			CurrentTaskGroup = ParseValue<string>(values, "CurrentTaskGroup");

			lastAPckCommand = ParseValue<string>(values, "lastAPckCommand");

			ShaftStates = ParseValue<List<byte>>(values, "ShaftStates") ?? new List<byte>();
		}
		return this;
	}

	public void Save(Action<string> store)
	{
		store(Serialize());
	}

	string Serialize()
	{
		string[] pairs = new string[]
		{
			"LifetimeAcceptedTasks=" + LifetimeAcceptedTasks,
			"LifetimeOperationTime=" + LifetimeOperationTime,
			"LifetimeWentToMaintenance=" + LifetimeWentToMaintenance,
			"LifetimeOreAmount=" + LifetimeOreAmount,
			"LifetimeYield=" + LifetimeYield,
			"StaticDockOverride=" + (StaticDockOverride.HasValue ? ѕ.х(StaticDockOverride.Value) : ""),
			"MinerState=" + MinerState,
			"miningPlaneNormal=" + (miningPlaneNormal.HasValue ? ѕ.х(miningPlaneNormal.Value) : ""),
			"getAbovePt=" + (getAbovePt.HasValue ? ѕ.х(getAbovePt.Value) : ""),
			"miningEntryPoint=" + (miningEntryPoint.HasValue ? ѕ.х(miningEntryPoint.Value) : ""),
			"corePoint=" + (corePoint.HasValue ? ѕ.х(corePoint.Value) : ""),
			"shaftRadius=" + shaftRadius,
			"maxDepth=" + maxDepth,
			"currentWp=" +  (currentWp.HasValue ? ѕ.х(currentWp.Value) : ""),
			"skipDepth=" + skipDepth,
			"lastFoundOreDepth=" + lastFoundOreDepth,
			"CurrentJobMaxShaftYield=" + CurrentJobMaxShaftYield,
			"minFoundOreDepth=" + minFoundOreDepth,
			"maxFoundOreDepth=" + maxFoundOreDepth,
			"CurrentShaftId=" + CurrentShaftId ?? "",
			"MaxGenerations=" + MaxGenerations,
			"CurrentTaskGroup=" + CurrentTaskGroup,
			"ShaftStates=" + string.Join(":", ShaftStates),
			"lastAPckCommand=" + lastAPckCommand
		};
		return string.Join("\n", pairs);
	}

	public override string ToString()
	{
		return Serialize();
	}
}
void Save(){stateWrapper.Save();}Program(){Runtime.UpdateFrequency=UpdateFrequency.Update1;ϑ();}List<MyIGCMessage>ˍ=new
List<MyIGCMessage>();void Main(string ˌ,UpdateType ˋ){ˍ.Clear();while(IGC.UnicastListener.HasPendingMessage){ˍ.Add(IGC.
UnicastListener.AcceptMessage());}var ˊ=IGC.RegisterBroadcastListener("miners.command");if(ˊ.HasPendingMessage){var ˉ=ˊ.AcceptMessage()
;ˌ=ˉ.Data.ToString();Ϋ("Got miners.command: "+ˌ);}ν(ˌ);foreach(var ȉ in ˍ){if(ȉ.Tag=="apck.ntv.update"){var ĥ=(MyTuple<
MyTuple<string,long,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>)ȉ.Data;var ã=ĥ.Item1.Item1;Ġ(ã,ĥ);if(Ё?.ќ!=null){
IGC.SendUnicastMessage(Ё.ќ.EntityId,"apck.ntv.update",ĥ);}}else if(ȉ.Tag=="apck.depart.complete"){ʫ?.ȇ(ȉ.Source.ToString())
;}else if(ȉ.Tag=="apck.depart.request"){ʫ.ȅ(ȉ.Source,(Vector3D)ȉ.Data,true);}else if(ȉ.Tag=="apck.docking.request"){ʫ.ȅ(ȉ
.Source,(Vector3D)ȉ.Data);}else if(ȉ.Tag=="apck.depart.complete"){if(Ё?.ѧ!=null)IGC.SendUnicastMessage(Ё.ѧ.Value,
"apck.depart.complete","");}else if(ȉ.Tag=="apck.docking.approach"||ȉ.Tag=="apck.depart.approach"){if(Ё?.ќ!=null){IGC.SendUnicastMessage(Ё.ќ.
EntityId,ȉ.Tag,(ImmutableArray<Vector3D>)ȉ.Data);}else{if(ȉ.Tag.Contains("depart")){var ţ=new ɻ("fin",Ɩ.Ƒ);ţ.ɸ=1;ţ.ʹ=()=>IGC.
SendUnicastMessage(ȉ.Source,"apck.depart.complete","");Ɩ.ǉ(ţ);}Ɩ.Ɣ.Disconnect();var ˎ=(ImmutableArray<Vector3D>)ȉ.Data;if(ˎ.Length>0){
foreach(var ë in ˎ){Func<Vector3D>ʪ=()=>Vector3D.Transform(ë,Ŏ("docking").č.Value);var D=new ƻ(){đ="r",î=true,ƹ=é=>ʪ(),ư=()=>Ɩ.
Ɣ.GetPosition(),Ƴ=Ʋ=>Ɩ.Ɣ.GetPosition()-Ŏ("docking").č.Value.Forward*10000,Ʊ=()=>Ɩ.Ɣ.WorldMatrix};Ɩ.ǉ(ɻ.ʴ("r",ʪ,D));}}}}}У
.Ƌ($"Version: {Ver}");У.Ƌ("Min3r role: "+CurrentRole);if(CurrentRole==Role.Dispatcher){У.Ƌ(ː.ToString());ː.Φ(ˍ);if(ș!=
null){foreach(var ƃ in ː.ͻ)IGC.SendUnicastMessage(ƃ.Ȩ,"report.request","");}ș?.ɦ(ː);ʫ.ǈ(IGC,ξ);if(Ъ!=null){if(Ш!=null){if(ș
==null){ș=new Ș(Ъ,ː,stateWrapper);ː.η=ș.ɉ;ș.ɗ=Ȇ=>ː.ҍ(Ȇ);if(ː.Ε!=null)ː.η.Invoke(ː.Ε);}else ș.ǈ(Ъ,Ш);}}}else if((
CurrentRole==Role.Agent)||(CurrentRole==Role.Lone)){Ё.ǈ(ˍ);У.Ƌ("Min3r state: "+Ё.Ѵ());У.Ƌ("Static dock override: "+(stateWrapper.
PState.StaticDockOverride.HasValue?"ON":"OFF"));У.Ƌ("Dispatcher: "+Ё.ѧ);У.Ƌ("Echelon: "+Ё.Ω);У.Ƌ("HoldingLock: "+Ё.Ϊ);У.Ƌ(
"WaitedSection: "+Ё.Ѧ);У.Ƌ($"Estimated shaft radius: {Variables.Get<float>("circular-pattern-shaft-radius"):f2}");У.Ƌ(
"LifetimeAcceptedTasks: "+stateWrapper.PState.LifetimeAcceptedTasks);У.Ƌ("LifetimeOreAmount: "+Ц(stateWrapper.PState.LifetimeOreAmount));У.Ƌ(
"LifetimeOperationTime: "+TimeSpan.FromSeconds(stateWrapper.PState.LifetimeOperationTime).ToString());У.Ƌ("LifetimeWentToMaintenance: "+
stateWrapper.PState.LifetimeWentToMaintenance);if(Ɩ!=null){if(Ɩ.Š.M!=Vector3D.Zero)Ȼ("agent-dest",Ɩ.Š.M,"");if(Ɩ.Š.K!=Vector3D.Zero)
Ȼ("agent-vel",Ɩ.Š.K,Ɩ.Š.ø);}if(Ъ!=null){Ы($"Version: {Ver}");Ы(
$"LifetimeAcceptedTasks: {stateWrapper.PState.LifetimeAcceptedTasks}");Ы($"LifetimeOreAmount: {Ц(stateWrapper.PState.LifetimeOreAmount)}");Ы(
$"LifetimeOperationTime: {TimeSpan.FromSeconds(stateWrapper.PState.LifetimeOperationTime)}");Ы($"LifetimeWentToMaintenance: {stateWrapper.PState.LifetimeWentToMaintenance}");Ы("\n");Ы(
$"CurrentJobMaxShaftYield: {Ц(stateWrapper.PState.CurrentJobMaxShaftYield)}");Ы($"CurrentShaftYield: "+Ё?.ѩ?.Ϯ());Ы(Ё?.ѩ?.ToString());Ч();}}if(κ.ƅ.φ("show-pstate"))У.Ƌ(stateWrapper.PState.ToString
());Ƃ();ĝ();if(DbgIgc!=0)Ⱥ(DbgIgc);Dt=Math.Max(0.001,Runtime.TimeSinceLastRun.TotalSeconds);У.Ŧ+=Dt;ˈ=Math.Max(ˈ,Runtime.
CurrentInstructionCount);У.Ƌ($"InstructionCount (Max): {Runtime.CurrentInstructionCount} ({ˈ})");У.Ƌ(
$"Processed in {Runtime.LastRunTimeMs:f3} ms");}int ˈ;void ˇ(string[]ˆ){if(ː!=null){var ʼ=ˆ.Skip(2).ToArray();var ę=new Vector3D(double.Parse(ʼ[0]),double.Parse(ʼ[1]
),double.Parse(ʼ[2]));Vector3D Ť;if(ʼ.Length>3){Ť=Vector3D.Normalize(ę-new Vector3D(double.Parse(ʼ[3]),double.Parse(ʼ[4])
,double.Parse(ʼ[5])));}else{if(Ш==null){У.Ƈ(
"WARNING: the normal was not supplied and there is no Control Station available to check if we are in gravity");Ť=-ʫ.Ǿ();У.Ƈ("Using 'first dock connector Backward' as a normal");}else{Vector3D ˮ;if(Ш.TryGetPlanetPosition(out ˮ)){Ť
=Vector3D.Normalize(ˮ-ę);У.Ƈ("Using mining-center-to-planet-center direction as a normal because we are in gravity");}
else{Ť=-ʫ.Ǿ();У.Ƈ("Using 'first dock connector Backward' as a normal");}}}var á=Variables.Get<string>("group-constraint");if
(!string.IsNullOrEmpty(á)){ː.Κ(Variables.Get<float>("circular-pattern-shaft-radius"),ę,Ť,Variables.Get<int>(
"max-generations"),á);ː.έ(á);}else У.Ƈ(
"To use this mode specify group-constraint value and make sure you have intended circular-pattern-shaft-radius");}else У.Ƈ("GPStaskHandler is intended for Dispatcher role");}List<IMyCameraBlock>ͺ=new List<IMyCameraBlock>();Vector3D
?ͷ;Vector3D?Ͷ;void ʹ(string[]ˆ){var ͳ=ͺ.Where(á=>á.IsActive).FirstOrDefault();if(ͳ!=null){ͳ.CustomData="";var ę=ͳ.
GetPosition()+ͳ.WorldMatrix.Forward*Variables.Get<float>("ct-raycast-range");ͳ.CustomData+="GPS:dir0:"+ѕ.х(ę)+":\n";Ϋ(
$"RaycastTaskHandler tries to raycast point GPS:create-task base point:{ѕ.х(ę)}:");if(ͳ.CanScan(ę)){var Ͳ=ͳ.Raycast(ę);if(!Ͳ.IsEmpty()){ͷ=Ͳ.HitPosition.Value;Ϋ(
$"GPS:Raycasted base point:{ѕ.х(Ͳ.HitPosition.Value)}:");ͳ.CustomData+="GPS:castedSurfacePoint:"+ѕ.х(ͷ.Value)+":\n";IMyShipController ͱ=Ё?.Ă??Ш;Vector3D ˮ;if((ͱ!=null)&&ͱ.
TryGetPlanetPosition(out ˮ)){Ͷ=Vector3D.Normalize(ˮ-ͷ.Value);У.Ƈ(
"Using mining-center-to-planet-center direction as a normal because we are in gravity");}else{var Ͱ=ͷ.Value-ͳ.GetPosition();var ˬ=Vector3D.Normalize(Vector3D.CalculatePerpendicularVector(Ͱ));var ˤ=ͷ.Value+ˬ
*Math.Min(10,Ͱ.Length());var ˣ=ͷ.Value+Vector3D.Normalize(Vector3D.Cross(ˬ,Ͱ))*Math.Min(20,Ͱ.Length());var ˢ=ˤ+Vector3D.
Normalize(ˤ-ͳ.GetPosition())*500;var ˡ=ˣ+Vector3D.Normalize(ˣ-ͳ.GetPosition())*500;ͳ.CustomData+="GPS:target1:"+ѕ.х(ˢ)+":\n";if(ͳ
.CanScan(ˢ)){var ˠ=ͳ.Raycast(ˢ);if(!ˠ.IsEmpty()){Ϋ($"GPS:Raycasted aux point 1:{ѕ.х(ˠ.HitPosition.Value)}:");ͳ.CustomData
+="GPS:cast1:"+ѕ.х(ˠ.HitPosition.Value)+":\n";ͳ.CustomData+="GPS:target2:"+ѕ.х(ˡ)+":\n";if(ͳ.CanScan(ˡ)){var ˑ=ͳ.Raycast(ˡ
);if(!ˑ.IsEmpty()){Ϋ($"GPS:Raycasted aux point 2:{ѕ.х(ˑ.HitPosition.Value)}:");ͳ.CustomData+="GPS:cast2:"+ѕ.х(ˑ.
HitPosition.Value)+":";Ͷ=-Vector3D.Normalize(Vector3D.Cross(ˠ.HitPosition.Value-ͷ.Value,ˑ.HitPosition.Value-ͷ.Value));}}}}}if(Ͷ.
HasValue&&ͷ.HasValue){У.Ƈ("Successfully got mining center and mining normal");if(ː!=null){var á=Variables.Get<string>(
"group-constraint");if(!string.IsNullOrEmpty(á)){ː.έ(á);ː.Κ(Variables.Get<float>("circular-pattern-shaft-radius"),ͷ.Value-Ͷ.Value*10,Ͷ.
Value,Variables.Get<int>("max-generations"),á);}else Ϋ(
"To use this mode specify group-constraint value and make sure you have intended circular-pattern-shaft-radius");}else if(Ё!=null){if(Ё.Ѫ!=null){ː.Κ(Variables.Get<float>("circular-pattern-shaft-radius"),ͷ.Value-Ͷ.Value*10,Ͷ.Value,
Variables.Get<int>("max-generations"),"LocalDispatcher");Ё.Ҷ();}else if(Ё.ѧ.HasValue)IGC.SendUnicastMessage(Ё.ѧ.Value,
"create-task",new MyTuple<float,Vector3D,Vector3D>(Variables.Get<float>("circular-pattern-shaft-radius"),ͷ.Value-Ͷ.Value*10,Ͷ.Value))
;}}else{У.Ƈ($"RaycastTaskHandler failed to get castedNormal or castedSurfacePoint");}}}else{У.Ƈ($"RaycastTaskHandler couldn't raycast initial position. Camera '{ͳ.CustomName}' had {ͳ.AvailableScanRange} AvailableScanRange"
);}}else{throw new Exception($"No active cam, {ͺ.Count} known");}}ˏ ː;class ˏ{public List<ά>ͻ=new List<ά>();Dictionary<
string,Queue<long>>ͼ=new Dictionary<string,Queue<long>>();public Action<Δ>η;public class ά{public long Ȩ;public string Ϊ;
public float Ω;public string Ψ;public Ȳ Χ;}IMyIntergridCommunicationSystem X;StateWrapper ȑ;public ˏ(
IMyIntergridCommunicationSystem Ā,StateWrapper ȑ){X=Ā;this.ȑ=ȑ;}void Ϋ(string ˉ){У.Ƈ(ˉ);}public void Φ(List<MyIGCMessage>ˍ){var Υ=X.
RegisterBroadcastListener("miners");while(Υ.HasPendingMessage){var ˉ=Υ.AcceptMessage();if(ˉ.Data!=null){if(ˉ.Data.ToString().Contains(
"common-airspace-ask-for-lock")){var Τ=ˉ.Data.ToString().Split(':')[1];if(!ͻ.Any(ƃ=>ƃ.Ϊ==Τ&&ƃ.Ȩ!=ˉ.Source)){ͻ.First(ƃ=>ƃ.Ȩ==ˉ.Source).Ϊ=Τ;X.
SendUnicastMessage(ˉ.Source,"miners","common-airspace-lock-granted:"+Τ);Ϋ(Τ+" granted to "+ˉ.Source);}else{if(!ͼ.ContainsKey(Τ))ͼ.Add(Τ,
new Queue<long>());if(!ͼ[Τ].Contains(ˉ.Source))ͼ[Τ].Enqueue(ˉ.Source);Ϋ(
"commonSpaceLockOwner rejected, added to requests queue: "+ˉ.Source);}}if(ˉ.Data.ToString().Contains("common-airspace-lock-released")){var Τ=ˉ.Data.ToString().Split(':')[1];Ϋ(
"(Dispatcher) received lock-released notification "+Τ+" from "+ˉ.Source);ͻ.Single(ƃ=>ƃ.Ȩ==ˉ.Source).Ϊ="";if(ͼ.ContainsKey(Τ)&&(ͼ[Τ].Count>0)){var Ȇ=ͼ[Τ].Dequeue();X.
SendUnicastMessage(Ȇ,"miners","common-airspace-lock-granted:"+Τ);ͻ.First(ƃ=>ƃ.Ȩ==Ȇ).Ϊ=Τ;Ϋ(Τ+" common-airspace-lock-granted to "+Ȇ);}}}}var
Σ=X.RegisterBroadcastListener("miners.handshake");while(Σ.HasPendingMessage){var ˉ=Σ.AcceptMessage();if(ˉ.Data is string)
{var γ=(string)ˉ.Data;Ϋ($"Initiated handshake by {ˉ.Source}, group tag: {γ}");ά ζ;if(!ͻ.Any(ƃ=>ƃ.Ȩ==ˉ.Source)){ζ=new ά{Ȩ=
ˉ.Source,Ω=(ͻ.Count+1)*Variables.Get<float>("echelon-offset")+10f,Ψ=γ};ͻ.Add(ζ);ζ.Χ=new Ȳ(){Ȩ=ζ.Ȩ,ȩ=Color.White};}else{ζ=
ͻ.Single(ƃ=>ƃ.Ȩ==ˉ.Source);ζ.Ψ=γ;}X.SendUnicastMessage(ˉ.Source,"miners.handshake.reply",X.Me);X.SendUnicastMessage(ˉ.
Source,"miners.echelon",ζ.Ω);if(ȑ.PState.miningPlaneNormal.HasValue){X.SendUnicastMessage(ˉ.Source,"miners.normal",ȑ.PState.
miningPlaneNormal.Value);}var ε=new string[]{"skip-depth","depth-limit","getAbove-altitude"};ƀ.ƅ.ƞ(500).Ɯ(()=>{foreach(var Ĝ in ε){Ϋ(
$"Propagating set-value:'{Ĝ}' to {ˉ.Source}");X.SendUnicastMessage(ˉ.Source,"set-value",$"{Ĝ}:{Variables.Get<float>(Ĝ)}");}});}}var δ=X.RegisterBroadcastListener(
"miners.report");while(δ.HasPendingMessage){var ˉ=δ.AcceptMessage();var γ=(MyTuple<long,MatrixD,Vector4,ImmutableArray<MyTuple<string,
string>>>)ˉ.Data;var β=ͻ.FirstOrDefault(ƃ=>ƃ.Ȩ==ˉ.Source);if(β!=null)β.Χ.Ȧ(γ);}foreach(var ˉ in ˍ){if(ˉ.Tag=="create-task"){
var γ=(MyTuple<float,Vector3D,Vector3D>)ˉ.Data;Ϋ("Got new mining task from agent");var β=ͻ.First(ƃ=>ƃ.Ȩ==ˉ.Source);β.Ϊ=
LOCK_NAME_GeneralSection;Κ(γ.Item1,γ.Item2,γ.Item3,Variables.Get<int>("max-generations"),β.Ψ);έ(β.Ψ);}if(ˉ.Tag.Contains("request-new")){if(ˉ.Tag
=="shaft-complete-request-new"){ѯ((int)ˉ.Data);У.Ƈ($"Shaft {ˉ.Data} complete");}Vector3D?Ξ=Vector3D.Zero;Vector3D?α=
Vector3D.Zero;int ΰ=0;if((Ε!=null)&&ѭ(ref Ξ,ref α,ref ΰ)){X.SendUnicastMessage(ˉ.Source,"miners.assign-shaft",new MyTuple<int,
Vector3D,Vector3D>(ΰ,Ξ.Value,α.Value));У.Ƈ($"AssignNewShaft with id {ΰ} sent");}else{X.SendUnicastMessage(ˉ.Source,"command",
"force-finish");}}if(ˉ.Tag=="ban-direction"){Ѯ((int)ˉ.Data);}}foreach(var ƃ in ͻ){У.Ƌ(ƃ.Ȩ+": echelon = "+ƃ.Ω+" lock: "+ƃ.Ϊ);}}public
void ί(){var Ŵ=ȑ.PState.CurrentTaskGroup;if(!string.IsNullOrEmpty(Ŵ)){Ϋ($"Broadcasting task resume for mining group '{Ŵ}'");
foreach(var ƃ in ͻ.Where(é=>é.Ψ==Ŵ)){X.SendUnicastMessage(ƃ.Ȩ,"miners.resume",ȑ.PState.miningPlaneNormal.Value);}}}public void
ή(){Ϋ($"Broadcasting global Halt & Clear state");X.SendBroadcastMessage("miners.command","command:halt");}public void έ(
string Ρ){Ϋ($"Preparing start for mining group '{Ρ}'");X.SendBroadcastMessage("miners.command","command:clear-storage-state");
ƀ.ƅ.Ɨ();ȑ.PState.LifetimeAcceptedTasks++;ƀ.ƅ.ƞ(500).Ɯ(()=>{foreach(var ƃ in ͻ.Where(é=>é.Ψ==Ρ)){X.SendUnicastMessage(ƃ.Ȩ,
"miners.normal",ȑ.PState.miningPlaneNormal.Value);}});ƀ.ƅ.ƞ(1000).Ɯ(()=>{Ϋ($"Broadcasting start for mining group '{Ρ}'");foreach(var ƃ
in ͻ.Where(é=>é.Ψ==Ρ)){X.SendUnicastMessage(ƃ.Ȩ,"command","mine");}});}public void Η(){X.SendBroadcastMessage(
"miners.command","command:force-finish");Ϋ($"Broadcasting Recall");}public void Ζ(){X.SendBroadcastMessage("miners.command",
"command:dispatch");ͼ.Clear();ͻ.ForEach(é=>é.Ϊ="");Ϋ($"WARNING! Purging Locks, green light for everybody...");}public Δ Ε;public class Δ{
public float Γ{get;private set;}public Vector3D Β{get;private set;}public Vector3D Α{get;private set;}public Vector3D Θ{get;
private set;}public Vector3D ΐ{get;private set;}public string Ύ{get;private set;}public List<Μ>Ό;public Δ(int Ί,float Ή,
Vector3D Έ,Vector3D Ά,string ͽ){Γ=Ή;Ύ=ͽ;Β=Ά;Α=Έ;Θ=Vector3D.Normalize(Vector3D.Cross(Έ,Ά));ΐ=Vector3D.Cross(Θ,Ά);Ό=new List<Μ>{
new Μ()};int Ȇ=1;while(Ȥ.Ɇ(++Ȇ)<=Ί){var ę=Ȥ.Ⱦ(Ȇ);Ό.Add(new Μ{Λ=ę*Γ,Ȩ=Ȇ-1});}}public void Ώ(int Ȇ){var ň=Ό.First(é=>é.Ȩ==Ȇ).
Λ;foreach(var ʻ in Ό){if(Vector2.Dot(Vector2.Normalize(ʻ.Λ),Vector2.Normalize(ň))>.8f)ʻ.ß=ShaftState.Cancelled;}}public
void Π(int Ȇ,ShaftState ñ){var ʻ=Ό.First(é=>é.Ȩ==Ȇ);var ʓ=ʻ.ß;if(ʓ==ShaftState.Cancelled&&ʓ==ñ)ñ=ShaftState.Planned;ʻ.ß=ñ;}
public bool Ο(ref Vector3D?Ξ,ref Vector3D?Ν,ref int Ȇ){var ʻ=Ό.FirstOrDefault(é=>é.ß==ShaftState.Planned);if(ʻ!=null){Ξ=Α+Θ*ʻ.
Λ.X+ΐ*ʻ.Λ.Y;Ν=Ξ.Value-Β*Variables.Get<float>("getAbove-altitude");Ȇ=ʻ.Ȩ;ʻ.ß=ShaftState.InProgress;return true;}return
false;}public class Μ{public ShaftState ß=ShaftState.Planned;public Vector2 Λ;public int Ȩ;}}public void Κ(float Â,Vector3D Α
,Vector3D Β,int Ί,string ͽ){Ε=new Δ(Ί,Â,Α,Β,ͽ);η?.Invoke(Ε);ȑ.ClearPersistentState();ȑ.PState.corePoint=Α;ȑ.PState.
shaftRadius=Â;ȑ.PState.miningPlaneNormal=Β;ȑ.PState.MaxGenerations=Ί;ȑ.PState.ShaftStates=Ε.Ό.Select(é=>(byte)é.ß).ToList();ȑ.
PState.CurrentTaskGroup=ͽ;Ϋ($"Creating task...");Ϋ(ѕ.Э("min3r.task.P",Α,Color.Red));Ϋ(ѕ.Э("min3r.task.Np",Α-Β,Color.Red));Ϋ(
$"shaftRadius: {Â}");Ϋ($"maxGenerations: {Ί}");Ϋ($"shafts: {Ε.Ό.Count}");Ϋ($"groupConstraint: {Ε.Ύ}");Ϋ($"Task created");}public void ҍ(int
Ȇ){var ƃ=ShaftState.Cancelled;Ε?.Π(Ȇ,ƃ);ȑ.PState.ShaftStates[Ȇ]=(byte)ƃ;η?.Invoke(Ε);}public void ѯ(int Ȇ){var ƃ=
ShaftState.Complete;Ε?.Π(Ȇ,ShaftState.Complete);ȑ.PState.ShaftStates[Ȇ]=(byte)ƃ;η?.Invoke(Ε);}public void Ѯ(int Ȇ){Ε?.Ώ(Ȇ);η?.
Invoke(Ε);}public bool ѭ(ref Vector3D?Ξ,ref Vector3D?Ν,ref int Ȇ){Ϋ($"CurrentTask.RequestShaft");bool Ȱ=Ε.Ο(ref Ξ,ref Ν,ref Ȇ)
;ȑ.PState.ShaftStates[Ȇ]=(byte)ShaftState.InProgress;η?.Invoke(Ε);return Ȱ;}StringBuilder ζ=new StringBuilder();public
override string ToString(){ζ.Clear();ζ.AppendLine($"CircularPattern radius: {ȑ.PState.shaftRadius:f2}");ζ.AppendLine($" ");ζ.
AppendLine($"Total subordinates: {ͻ.Count}");ζ.AppendLine($"Lock queue: {ͼ.Count}");ζ.AppendLine(
$"LifetimeAcceptedTasks: {ȑ.PState.LifetimeAcceptedTasks}");return ζ.ToString();}}ѫ Ё;class ѫ{public ˏ Ѫ{get;set;}è Ů;public Є ѩ{get;private set;}public Role Ѩ;public long?ѧ;
public float?Ω;public string Ϊ="";public string Ѧ="";public bool Ѭ;public bool ѥ;bool?Ѱ;public bool Ѷ{set{if(Ѱ!=value){ҝ.
ForEach(ƭ=>ƭ.TerrainClearingMode=value);ѽ($"command:set-sp-limit:{Variables.Get<float>(value?"speed-clear":"speed-drill")}");Ѱ=
value;}}}public Vector3D ѵ(){if(!ѳ.miningPlaneNormal.HasValue){var Ѥ=Ă.GetNaturalGravity();if(Ѥ==Vector3D.Zero)throw new
Exception("Need either natural gravity or miningPlaneNormal");else return Vector3D.Normalize(Ѥ);}return ѳ.miningPlaneNormal.Value
;}public MinerState Ѵ(){return ѳ.MinerState;}public PersistentState ѳ{get{return ȑ.PState;}}Func<string,Ĕ>Ѳ;StateWrapper
ȑ;public ѫ(Role ϖ,IMyGridTerminalSystem Ũ,IMyIntergridCommunicationSystem Ā,StateWrapper ȑ,Func<string,Ĕ>Ŏ,
IMyTerminalBlock Ţ){Ѳ=Ŏ;this.Ѩ=ϖ;this.Ũ=Ũ;X=Ā;this.ȑ=ȑ;Ҟ=ї<IMyGyro>(â=>â.CustomName.Contains(ForwardGyroTag)&&â.IsSameConstructAs(Ţ));Ă=
ї<IMyRemoteControl>(â=>â.IsSameConstructAs(Ţ));Ɣ=ї<IMyShipConnector>(â=>â.IsSameConstructAs(Ţ));Ũ.GetBlocksOfType(ҝ,ƭ=>ƭ.
IsSameConstructAs(Ţ));Ũ.GetBlocksOfType(ҟ,ƭ=>ƭ.IsSameConstructAs(Ţ)&&ƭ.HasInventory&&((ƭ is IMyCargoContainer)||(ƭ is IMyShipDrill)||(ƭ
is IMyShipConnector)));Ũ.GetBlocksOfType(ҡ,â=>â.IsSameConstructAs(Ţ));Ũ.GetBlocksOfType(Ҝ,â=>â.IsSameConstructAs(Ţ));List<
IMyTimerBlock>å=new List<IMyTimerBlock>();Ũ.GetBlocksOfType(å,â=>â.IsSameConstructAs(Ţ));Ů=new è(å);float ѱ=0;float ř=Ţ.CubeGrid.
GridSizeEnum==MyCubeSize.Large?2f:1.5f;foreach(var ƭ in ҝ){var Â=Vector3D.Reject(ƭ.GetPosition()-Ҟ.GetPosition(),Ҟ.WorldMatrix.
Forward).Length();ѱ=(float)Math.Max(Â+ř,ѱ);}Variables.Set("circular-pattern-shaft-radius",ѱ);var Ǐ=new List<IMyRadioAntenna>();
Ũ.GetBlocksOfType(Ǐ,â=>â.IsSameConstructAs(Ţ));ý=Ǐ.FirstOrDefault();var ѝ=new List<IMyLightingBlock>();Ũ.GetBlocksOfType(
ѝ,â=>â.IsSameConstructAs(Ţ));Ҧ=ѝ.FirstOrDefault();Ũ.GetBlocksOfType(Ҹ,â=>â.IsSameConstructAs(Ţ));if(Ɣ.OtherConnector==
null){Ҩ(false);}}public void ћ(IMyProgrammableBlock ќ){this.ќ=ќ;}public void ћ(ƕ ǒ){Ҡ=ǒ;}public MinerState њ{get;private set
;}public void Ċ(MinerState Ć){Ů.ä(Ѵ()+".OnExit");Ϋ("SetState: "+Ѵ()+"=>"+Ć);Ů.ä(Ć+".OnEnter");њ=ѳ.MinerState;ѳ.MinerState
=Ć;if((Ć==MinerState.Disabled)||(Ć==MinerState.Idle)){ҝ.ForEach(ƭ=>ƭ.Enabled=false);ѽ(INERT_CMD,U=>U.Ċ(ApckState.Inert));
}}public void ј(){Ҏ(1,1);ѽ("command:pillock-mode:Disabled",U=>U.Š.Ċ(à.ß.Þ));ҝ.ForEach(ƭ=>ƭ.Enabled=false);ȑ.
ClearPersistentState();}public void Ĉ(string ć){MinerState Ć;if(Enum.TryParse(ć,out Ć))Ċ(Ć);}public Ŧ ї<Ŧ>(Func<IMyTerminalBlock,bool>љ)
where Ŧ:class{var ў=new List<IMyTerminalBlock>();Ũ.GetBlocksOfType(ў,â=>((â is Ŧ)&&љ(â)));return ў.First()as Ŧ;}public void Κ
(){var Ѥ=Ă.GetNaturalGravity();if(Ѥ!=Vector3D.Zero)ѳ.miningPlaneNormal=Vector3D.Normalize(Ѥ);else ѳ.miningPlaneNormal=Ҟ.
WorldMatrix.Forward;double À;if(Ă.TryGetPlanetElevation(MyPlanetElevation.Surface,out À))ѳ.miningEntryPoint=Ҟ.WorldMatrix.
Translation+ѳ.miningPlaneNormal.Value*(À-5);else ѳ.miningEntryPoint=Ҟ.WorldMatrix.Translation;if(Ѩ==Role.Agent){if(ѧ.HasValue){X.
SendUnicastMessage(ѧ.Value,"create-task",new MyTuple<float,Vector3D,Vector3D>(Variables.Get<float>("circular-pattern-shaft-radius"),ѳ.
miningEntryPoint.Value,ѳ.miningPlaneNormal.Value));}}else if(Ѩ==Role.Lone){var Ͼ=ѳ.miningEntryPoint.Value;Ѫ.Κ(Variables.Get<float>(
"circular-pattern-shaft-radius"),Ͼ,ѳ.miningPlaneNormal.Value,Variables.Get<int>("max-generations"),"LocalDispatcher");ѳ.getAbovePt=Ͼ-ѳ.
miningPlaneNormal.Value*Variables.Get<float>("getAbove-altitude");ѳ.miningEntryPoint=Ͼ;}}public void ǈ(List<MyIGCMessage>ˍ){У.Ƌ(Ҡ!=null?
"Embedded APck":ќ.CustomName);Ҡ?.ǈ(ξ,У.Ƌ);if((ѩ!=null)&&(!Ѭ)){if((Ѩ!=Role.Agent)||(ѧ.HasValue))ѩ.Ѕ(ѳ.MinerState);}var Ļ=ѩ;var Υ=X.
RegisterBroadcastListener("miners");foreach(var ˉ in ˍ){if(!ˉ.Tag.Contains("set-vectors"))Ҥ(ˉ,false);if((ˉ.Tag=="miners.assign-shaft")&&(ˉ.Data
is MyTuple<int,Vector3D,Vector3D>)&&(Ѩ==Role.Agent)){var γ=(MyTuple<int,Vector3D,Vector3D>)ˉ.Data;if(Ļ!=null){Ļ.Ϻ(γ.Item1,
γ.Item2,γ.Item3);Ϋ("Got new ShaftVectors");Ѡ();}}if(ˉ.Tag=="miners.handshake.reply"){Ϋ("Received reply from dispatcher "+
ˉ.Source);ѧ=ˉ.Source;}if(ˉ.Tag=="miners.echelon"){Ϋ("Was assigned an echelon of "+ˉ.Data);Ω=(float)ˉ.Data;}if(ˉ.Tag==
"miners.normal"){var Ά=(Vector3D)ˉ.Data;Ϋ("Was assigned a normal of "+Ά);ѳ.miningPlaneNormal=Ά;}if(ˉ.Tag=="miners.resume"){var Ά=(
Vector3D)ˉ.Data;Ϋ("Received resume command. Clearing state, running MineCommandHandler, assigned a normal of "+Ά);ȑ.
ClearPersistentState();ѳ.miningPlaneNormal=Ά;Ҷ();}if(ˉ.Tag=="command"){if(ˉ.Data.ToString()=="force-finish")Ҍ();if(ˉ.Data.ToString()=="mine"
)Ҷ();}if(ˉ.Tag=="set-value"){var ˀ=((string)ˉ.Data).Split(':');Ϋ($"Set value '{ˀ[0]}' to '{ˀ[1]}'");Variables.Set(ˀ[0],ˀ[
1]);}if(ˉ.Data.ToString().Contains("common-airspace-lock-granted")){var Τ=ˉ.Data.ToString().Split(':')[1];if(!string.
IsNullOrEmpty(Ϊ)&&(Ϊ!=Τ)){Ϋ($"{Τ} common-airspace-lock hides current ObtainedLock {Ϊ}!");}Ϊ=Τ;Ϋ(Τ+" common-airspace-lock-granted");if
(Ѧ==Τ)Ѡ();}if(ˉ.Tag=="report.request"){var ϝ=new Ȳ();ϝ.Ȩ=X.Me;ϝ.Ȫ=Ҟ.WorldMatrix;ϝ.ȩ=Ҧ?.Color??Color.White;ѩ?.Ϟ(ϝ,ѳ.
MinerState);X.SendBroadcastMessage("miners.report",ϝ.ȥ());}}while(Υ.HasPendingMessage){var ˉ=Υ.AcceptMessage();Ҥ(ˉ,false);if(ˉ.
Data!=null){if(ˉ.Data.ToString().Contains("common-airspace-lock-released")){var Τ=ˉ.Data.ToString().Split(':')[1];if(Ѩ==Role
.Agent){Ϋ("(Agent) received lock-released notification "+Τ+" from "+ˉ.Source);}}if(Ѩ==Role.Agent){if(ˉ.Data.ToString()==
"dispatcher-change"){ѧ=null;ƀ.ƅ.ƚ(()=>!ѧ.HasValue).ƞ(1000).Ɯ(()=>ώ("miners.handshake",Variables.Get<string>("group-constraint")));}}}}}
Queue<Action<ѫ>>ѣ=new Queue<Action<ѫ>>();public void Ѣ(string Τ,Action<ѫ>ѡ){Ѭ=true;if(!string.IsNullOrEmpty(Τ))Ѧ=Τ;ѣ.Enqueue(
ѡ);Ϋ("WaitForDispatch section \""+Τ+"\", callback chain: "+ѣ.Count);}public void Ѡ(){Ѭ=false;Ѧ="";var џ=ѣ.Count;if(џ>0){Ϋ
("Dispatching, callback chain: "+џ);var Ȋ=ѣ.Dequeue();Ȋ.Invoke(this);}else Ϋ("WARNING: empty Dispatch()");}public void ώ<
Ŧ>(string ŵ,Ŧ γ){X.SendBroadcastMessage(ŵ,γ,TransmissionDistance.TransmissionDistanceMax);Ҥ(γ,true);}public void ҥ<Ŧ>(
string ŵ,Ŧ γ){if(ѧ.HasValue)X.SendUnicastMessage(ѧ.Value,ŵ,γ);}public void Ϋ(object ˉ){У.Ƈ($"MinerController -> {ˉ}");}public
void Ҥ(object ˉ,bool ң){string γ=ˉ.GetType().Name;if(ˉ is string)γ=(string)ˉ;else if((ˉ is ImmutableArray<Vector3D>)||(ˉ is
Vector3D))γ="some vector(s)";if(κ.ƅ.φ("log-message")){if(!ң)У.Ƈ($"MinerController MSG-IN -> {γ}");else У.Ƈ(
$"MinerController MSG-OUT -> {γ}");}}public Action Ң;public IMyProgrammableBlock ќ;ƕ Ҡ;public IMyGridTerminalSystem Ũ;public
IMyIntergridCommunicationSystem X;public IMyRemoteControl Ă;public List<IMyTerminalBlock>ҟ=new List<IMyTerminalBlock>();public IMyTerminalBlock Ҟ;
public List<IMyShipDrill>ҝ=new List<IMyShipDrill>();public IMyShipConnector Ɣ;public IMyRadioAntenna ý;public List<
IMyBatteryBlock>ҡ=new List<IMyBatteryBlock>();public List<IMyGasTank>Ҝ=new List<IMyGasTank>();public IMyLightingBlock Ҧ;public List<
IMyTerminalBlock>Ҹ=new List<IMyTerminalBlock>();public void ҷ(){ѩ=new Є(this);ѩ.Ϛ=DateTime.Now;}public void Ҷ(){ѩ=new Є(this);ѩ.Ϛ=
DateTime.Now;ѳ.LifetimeAcceptedTasks++;ѳ.maxDepth=Variables.Get<float>("depth-limit");ѳ.skipDepth=Variables.Get<float>(
"skip-depth");if(!Ҋ()){ѩ.Ѐ();}}public void ҵ(){if(ѩ!=null){ѩ.Ͻ();}}public void Ҵ(string[]ˆ){if((ˆ.Length>2)&&(ˆ[2]=="clear"))ѳ.
StaticDockOverride=null;else ѳ.StaticDockOverride=Ҟ.WorldMatrix.Translation;}public Vector3D ҳ(Vector3D ň){if(Ω.HasValue){return ň-ѵ()*Ω.
Value;}return ň;}public Vector3D ҳ(Vector3D ň,Vector3D Ά){if(Ω.HasValue){return ň-Ά*Ω.Value;}return ň;}public bool Ҳ(bool ұ){
if(ѳ.StaticDockOverride.HasValue){string Ұ="command:create-wp:Name=StaticDock,Ng=Forward:"+ѕ.х(ѳ.StaticDockOverride.Value)
;if(!WholeAirspaceLocking)Ҁ(LOCK_NAME_GeneralSection);if(Ω.HasValue){Ұ=
"command:create-wp:Name=StaticDock.echelon,Ng=Forward:"+ѕ.х(ҳ(ѳ.StaticDockOverride.Value))+":"+Ұ;}if(ұ){if(ѳ.getAbovePt.HasValue)ѽ(
"command:create-wp:Name=StaticDock.getAbovePt,Ng=Forward,SpeedLimit="+Variables.Get<float>("speed-clear")+":"+ѕ.х(ҳ(ѳ.getAbovePt.Value))+":"+Ұ);else{var ү=ѳ.StaticDockOverride.Value;
Vector3D Ǎ;Ă.TryGetPlanetPosition(out Ǎ);var Ү=ү-Ǎ;var ҭ=(Ҟ.GetPosition()-Ǎ);var Ў=Vector3D.Normalize(ҭ);var Ҭ=Ү.Length()>ҭ.
Length()?Ү:ҭ;var ҫ=Ǎ+Ў*(Ҭ.Length()+100f);ѽ("command:create-wp:Name=StaticDock.approachP,Ng=Forward:"+ѕ.х(ҫ)+":"+Ұ);}}else{ѽ(Ұ)
;}return true;}return false;}public void Ҫ(){bool ҩ=Ѵ()==MinerState.ForceFinish;if(ѳ.StaticDockOverride.HasValue){if(Ҳ(ҩ)
){if(!WholeAirspaceLocking)Ҁ(LOCK_NAME_GeneralSection);if(!ҩ)Ċ(MinerState.Docking);}}else{if(ѧ.HasValue){if(!
WholeAirspaceLocking)Ҁ(LOCK_NAME_GeneralSection);Ң?.Invoke();X.SendUnicastMessage(ѧ.Value,"apck.docking.request",Ɣ.GetPosition());Ċ(
MinerState.WaitingForDocking);}else{if(!ҩ){Ċ(MinerState.Docking);ѽ("command:request-docking");}else{ѽ(
"[command:create-wp:Name=AutoDock.getAbovePt,Ng=Forward,SpeedLimit="+Variables.Get<float>("speed-clear")+":"+ѕ.х(ҳ(ѳ.getAbovePt.HasValue?ѳ.getAbovePt.Value:(Ҟ.GetPosition()+Ҟ.WorldMatrix.
Up*50)))+":command:request-docking]");}}}}void Ҩ(bool ҧ){if(ҧ){ҡ.ForEach(â=>â.ChargeMode=ChargeMode.Recharge);Ҝ.ForEach(â
=>â.Stockpile=true);}else{ҡ.ForEach(â=>â.ChargeMode=ChargeMode.Auto);Ҝ.ForEach(â=>â.Stockpile=false);}}public void Ҍ(){if(
Ɣ.Status==MyShipConnectorStatus.Connected){Ҩ(true);ѳ.lastAPckCommand="";Ċ(MinerState.Disabled);}else{ҝ.ForEach(ҋ=>ҋ.
Enabled=false);Ѧ="";ѣ.Clear();ҁ(LOCK_NAME_ForceFinishSection,(ϼ)=>{if(ѳ.MinerState==MinerState.ForceFinish||ѳ.MinerState==
MinerState.Docking||Ɣ.Status==MyShipConnectorStatus.Connected){Ċ(MinerState.ForceFinish);Ϋ(
"Started force-finish callback during docking in progress!");Ҁ(LOCK_NAME_ForceFinishSection);return;}Ċ(MinerState.ForceFinish);ѩ=ѩ??new Є(this);Ҫ();});}}public bool Ҋ(){if(Ɣ.
Status==MyShipConnectorStatus.Connected){if(ѳ.getAbovePt.HasValue){Ċ(MinerState.Docking);return true;}else{if(Ѩ==Role.Agent){ҥ
("request-new","");Ѣ("",ϼ=>{ϼ.Ċ(MinerState.Docking);});return true;}}}return false;}public void ҁ(string Τ,Action<ѫ>Ǉ){if
(Ѩ==Role.Agent){ώ("miners","common-airspace-ask-for-lock:"+Τ);Ѣ(Τ,Ǉ);}else{Ǉ(this);}}public void Ҁ(string Τ){if(Ϊ==Τ){Ϊ=
null;if(Ѩ==Role.Agent){ώ("miners","common-airspace-lock-released:"+Τ);Ϋ($"Released lock: {Τ}");}}else{Ϋ(
"Tried to release non-owned lock section "+Τ);}}public ς ѿ;public void ѽ(string ƛ,Action<ƕ>Ѽ=null){if(ƛ!=INERT_CMD)ѳ.lastAPckCommand=ƛ;У.Ƈ("CommandAutoPillock: "+
ƛ);if(Ҡ!=null){if(Ѽ!=null){Ѽ(Ҡ);}else{var ѻ=ƛ.Split(new[]{"],["},StringSplitOptions.RemoveEmptyEntries).Select(ƃ=>ƃ.Trim(
'[',']')).ToList();foreach(var ĺ in ѻ){string[]ο=ĺ.Split(new[]{':'},StringSplitOptions.RemoveEmptyEntries);if(ο[0]==
"command"){ѿ.π(ο[1],ο);}}}}else{if(X.IsEndpointReachable(ќ.EntityId)){X.SendUnicastMessage(ќ.EntityId,"apck.command",ƛ);}else{
throw new Exception($"APck {ќ.EntityId} is not reachable");}}}DateTime Ѻ;bool ѹ(float Ѹ,float Ѿ){var ѷ=DateTime.Now;if((ѷ-Ѻ).
TotalSeconds>60){Ѻ=ѷ;return Ҏ(Ѹ,Ѿ);}return true;}bool Ҏ(float Ѹ,float Ѿ){Ҹ.ForEach(é=>ҕ(é,Ҙ(é),њ!=MinerState.ForceFinish));if(Ҹ.Any(
â=>!â.IsFunctional)){if(ý!=null)ý.CustomName=ý.CubeGrid.CustomName+"> Damaged. Fix me asap!";Ҹ.Where(â=>!â.IsFunctional).
ToList().ForEach(â=>У.Ƈ($"{â.CustomName} is damaged or destroyed"));return false;}float қ=0;float Қ=0;foreach(var â in ҡ){Қ+=â
.MaxStoredPower;қ+=â.CurrentStoredPower;}double ҙ=0;foreach(var â in Ҝ){ҙ+=â.FilledRatio;}if(Ҝ.Any()&&(ҙ/Ҝ.Count<Ѿ)){if(ý
!=null)ý.CustomName=$"{ý.CubeGrid.CustomName}> Maintenance. Gas level: {ҙ/Ҝ.Count:f2}/{Ѿ:f2}";return false;}else if(қ/Қ<Ѹ)
{if(ý!=null)ý.CustomName=$"{ý.CubeGrid.CustomName}> Maintenance. Charge level: {қ/Қ:f2}/{Ѹ:f2}";return false;}else{return
true;}}float Ҙ(IMyTerminalBlock җ){IMySlimBlock Җ=җ.CubeGrid.GetCubeBlock(җ.Position);if(Җ!=null)return(Җ.BuildIntegrity-Җ.
CurrentDamage)/Җ.MaxIntegrity;else return 1f;}void ҕ(IMyTerminalBlock ҏ,float Ҕ,bool ғ){string ã=ҏ.CustomName;if((Ҕ<1f)&&(!ғ||!ҏ.
IsFunctional)){if(!(ҏ is IMyRadioAntenna)&&!(ҏ is IMyBeacon)){ҏ.SetValue("ShowOnHUD",true);}string Ғ;if(ã.Contains("||")){string ґ=
@"(?<=DAMAGED: )(?<label>\d+)(?=%)";System.Text.RegularExpressions.Regex Â=new System.Text.RegularExpressions.Regex(ґ);Ғ=Â.Replace(ã,delegate(System.Text.
RegularExpressions.Match ȉ){return(Ҕ*100).ToString("F0");});}else{Ғ=string.Format("{0} || DAMAGED: {1}%",ã,Ҕ.ToString("F0"));Ϋ(
$"{ã} was damaged. Showing on HUD.");}ҏ.CustomName=Ғ;}else{Ґ(ҏ);}}void Ґ(IMyTerminalBlock ҏ){if(ҏ.CustomName.Contains("||")){string ã=ҏ.CustomName;ҏ.
CustomName=ã.Split('|')[0].Trim();if(!(ҏ is IMyRadioAntenna)&&!(ҏ is IMyBeacon)){ҏ.SetValue("ShowOnHUD",false);}Ϋ(
$"{ҏ.CustomName} was fixed.");}}public class Є{protected ѫ á;bool Ѓ(double Ђ){return(!á.ѳ.currentWp.HasValue||(á.ѳ.currentWp.Value-á.Ҟ.WorldMatrix.
Translation).Length()<=Ђ);}public Є(ѫ Ё){á=Ё;}public void Ѐ(){if(á.Ѩ==Role.Agent){á.ҥ("request-new","");á.Ѣ("",ϼ=>{á.ҁ(
LOCK_NAME_GeneralSection,é=>{é.Ċ(MinerState.ChangingShaft);é.ҝ.ForEach(ƭ=>ƭ.Enabled=false);var Ϡ=-15;var ň=é.ҳ(á.ѳ.miningEntryPoint.Value+á.ѵ()*
Ϡ);var Ͽ=$"command:create-wp:Name=ChangingShaft,Ng=Forward,UpNormal=1;0;0,"+$"AimNormal={ѕ.х(á.ѵ()).Replace(':',';')}"+
$":{ѕ.х(ň)}";á.ѽ(Ͽ);á.ѳ.currentWp=ň;});});}else if(á.Ѩ==Role.Lone){á.ѳ.maxDepth=Variables.Get<float>("depth-limit");á.ѳ.skipDepth=
Variables.Get<float>("skip-depth");á.Ċ(MinerState.GoingToEntry);á.ѳ.currentWp=á.ѳ.miningEntryPoint;var Ͽ=
$"command:create-wp:Name=drill entry,Ng=Forward,"+$"AimNormal={ѕ.х(á.ѵ()).Replace(':',';')}"+$":{ѕ.х(á.ѳ.miningEntryPoint.Value)}";á.ѽ(Ͽ);}}public void Ͻ(){if(á.ѳ.
CurrentJobMaxShaftYield<ϳ+ϵ-ϴ)á.ѳ.CurrentJobMaxShaftYield=ϳ+ϵ-ϴ;if(κ.ƅ.φ("adaptive-mining")){if(!ϙ.HasValue||((ϳ+ϵ-ϴ)/á.ѳ.
CurrentJobMaxShaftYield<0.5f)){if(á.Ѩ==Role.Agent){á.ҥ("ban-direction",á.ѳ.CurrentShaftId.Value);}else{á.Ѫ.Ѯ(á.ѳ.CurrentShaftId.Value);}}}Ϸ();ϙ
=null;var Ϡ=-15;var ň=á.ѳ.miningEntryPoint.Value+á.ѵ()*Ϡ;if(á.Ѩ==Role.Agent){á.ҥ("shaft-complete-request-new",á.ѳ.
CurrentShaftId.Value);á.Ѣ("",ϼ=>{á.ҁ(LOCK_NAME_GeneralSection,é=>{é.Ċ(MinerState.ChangingShaft);é.ҝ.ForEach(ƭ=>ƭ.Enabled=false);é.ѽ(
"command:create-wp:Name=ChangingShaft,Ng=Forward:"+ѕ.х(ň));á.ѳ.currentWp=ň;});});}else if(á.Ѩ==Role.Lone){int ϻ=0;if(á.Ѫ.ѭ(ref á.ѳ.miningEntryPoint,ref á.ѳ.getAbovePt,ref
ϻ)){á.Ċ(MinerState.ChangingShaft);á.ҝ.ForEach(ƭ=>ƭ.Enabled=false);á.ѳ.CurrentShaftId=ϻ;á.ѽ(
"command:create-wp:Name=ChangingShaft,Ng=Forward:"+ѕ.х(ň));á.ѳ.currentWp=ň;}}}public void Ϻ(int Ȇ,Vector3D Ͼ,Vector3D Ϲ){á.ѳ.miningEntryPoint=Ͼ;á.ѳ.getAbovePt=Ϲ;á.ѳ.
CurrentShaftId=Ȇ;}public void Ѕ(MinerState ñ){if(ñ==MinerState.GoingToEntry){if(Ѓ(0.5f)){á.Ҁ(LOCK_NAME_GeneralSection);á.ҝ.ForEach(ƭ=>
ƭ.Enabled=true);á.Ċ(MinerState.Drilling);á.ѽ("command:create-wp:Name=drill,Ng=Forward,PosDirectionOverride=Forward"+
",AimNormal="+ѕ.х(á.ѵ()).Replace(':',';')+",UpNormal=1;0;0,SpeedLimit="+Variables.Get<float>("speed-drill")+":0:0:0");}}if(ñ==
MinerState.Drilling){Ϝ=(float)(á.Ҟ.WorldMatrix.Translation-á.ѳ.miningEntryPoint.Value).Length();У.Ƌ(
$"Depth: current: {Ϝ:f1} skip: {á.ѳ.skipDepth:f1}");if(á.ѳ.maxDepth.HasValue&&(Ϝ>á.ѳ.maxDepth.Value)||!á.ѹ(Variables.Get<float>("battery-low-factor"),Variables.Get<float>
("gas-low-factor"))){Ϥ();}if((!á.ѳ.skipDepth.HasValue)||(Ϝ>á.ѳ.skipDepth)){á.Ѷ=false;if(ϲ()){ϙ=Math.Max(Ϝ,ϙ??0);if((!ϟ.
HasValue)||(ϟ>Ϝ))ϟ=Ϝ;if((!ϥ.HasValue)||(ϥ<Ϝ))ϥ=Ϝ;if(κ.ƅ.φ("adaptive-mining")){á.ѳ.skipDepth=ϟ.Value-2f;á.ѳ.maxDepth=ϥ.Value+2f;}
}else{if(ϙ.HasValue&&(Ϝ-ϙ>2)){Ϥ();}}if(ϫ()){Ϥ();}}else{á.Ѷ=true;}}if((ñ==MinerState.GettingOutTheShaft)||(ñ==MinerState.
WaitingForLockInShaft)){if(Ѓ(0.5f)){if(ϫ()||!á.Ҏ(Variables.Get<float>("battery-low-factor"),Variables.Get<float>("gas-low-factor"))){á.ҁ(
LOCK_NAME_GeneralSection,ϼ=>{ϼ.Ċ(MinerState.GoingToUnload);ϼ.ҝ.ForEach(ƭ=>ƭ.Enabled=false);var ň=á.ҳ(á.ѳ.getAbovePt.Value);ϼ.ѽ(
"command:create-wp:Name=GoingToUnload,Ng=Forward:"+ѕ.х(ň));á.ѳ.currentWp=ň;});}else{Ͻ();}}}if(ñ==MinerState.ChangingShaft){if(Ѓ(0.5f)){var Ϡ=-15;var ň=á.ѳ.
miningEntryPoint.Value+á.ѵ()*Ϡ;á.ҳ(ň);á.ѽ("command:create-wp:Name=GoingToEntry (ChangingShaft),Ng=Forward:"+ѕ.х(ň));á.ѳ.currentWp=ň;á.Ċ(
MinerState.ReturningToShaft);}}if(ñ==MinerState.ReturningToShaft){if(Ѓ(1)){á.ҁ(LOCK_NAME_GeneralSection,ϼ=>{ϼ.Ċ(MinerState.
GoingToEntry);á.ҝ.ForEach(ƭ=>ƭ.Enabled=true);var Ξ=$"command:create-wp:Name=drill entry,Ng=Forward,UpNormal=1;0;0,AimNormal="+
$"{ѕ.х(á.ѵ()).Replace(':',';')}:";double À;if(κ.ƅ.φ("adjust-entry-by-elevation")&&á.Ă.TryGetPlanetElevation(MyPlanetElevation.Surface,out À)){Vector3D Ǎ;
á.Ă.TryGetPlanetPosition(out Ǎ);var Ў=Vector3D.Normalize(á.ѳ.miningEntryPoint.Value-Ǎ);var Ɓ=(á.Ҟ.WorldMatrix.Translation
-Ǎ).Length()-À+5f;var Ѝ=Ǎ+Ў*Ɓ;ϼ.ѽ(Ξ+ѕ.х(Ѝ));á.ѳ.currentWp=Ѝ;}else{ϼ.ѽ(Ξ+ѕ.х(á.ѳ.miningEntryPoint.Value));á.ѳ.currentWp=á.
ѳ.miningEntryPoint;}});}}if(ñ==MinerState.GoingToUnload){if(Ѓ(0.5f)){á.Ҫ();}}if(ñ==MinerState.WaitingForDocking){var Ќ=á.
Ѳ("docking");if(Ќ.ď.HasValue){if(á.њ==MinerState.ForceFinish){Vector3D Ћ;if(á.ѳ.getAbovePt.HasValue){Ћ=á.ҳ(á.ѳ.getAbovePt
.Value);}else{var Њ=Ќ.ď.Value-á.Ɣ.GetPosition();var Ť=Ќ.č.Value.Backward;var Љ=Vector3D.ProjectOnVector(ref Њ,ref Ť);Ћ=á.
ҳ(á.Ɣ.GetPosition()+Љ,Ť);}á.ѽ("command:create-wp:Name=ForceFinish.getAbovePt,SpeedLimit="+Variables.Get<float>(
"speed-clear")+",Ng=Forward:"+ѕ.х(Ћ)+":command:create-wp:Name=ForceFinish.dock-echelon,Ng=Forward,TransformChannel=docking:"+ѕ.х(
Vector3D.Transform(á.ҳ(Ќ.ď.Value,Ќ.č.Value.Backward)-Ќ.č.Value.Backward*Variables.Get<float>("getAbove-altitude"),MatrixD.Invert
(Ќ.č.Value)))+":command:pillock-mode:DockingFinal");á.Ċ(MinerState.ForceFinish);}else{á.ѽ(
"command:create-wp:Name=DynamicDock.echelon,Ng=Forward,AimNormal="+ѕ.х(á.ѵ()).Replace(':',';')+",TransformChannel=docking:"+ѕ.х(Vector3D.Transform(á.ҳ(Ќ.ď.Value,Ќ.č.Value.Backward)-Ќ.č.
Value.Backward*Variables.Get<float>("getAbove-altitude"),MatrixD.Invert(Ќ.č.Value)))+":command:pillock-mode:DockingFinal");á.
Ċ(MinerState.Docking);}}}if(ñ==MinerState.Docking){if(á.Ɣ.Status==MyShipConnectorStatus.Connected){if(!á.ѥ){á.ѥ=true;У.Ƈ(
"Regular docking handled");á.ѽ("command:pillock-mode:Disabled");á.Ă.DampenersOverride=false;á.Ɣ.OtherConnector.CustomData="";á.Ң?.Invoke();á.Ҩ(
true);if(á.Ϊ==LOCK_NAME_GeneralSection)á.Ҁ(LOCK_NAME_GeneralSection);}У.Ƌ("Docking: Connected");if(!Ј()){У.Ƌ(
"Docking: still have items");}else{if(á.Ҏ(Variables.Get<float>("battery-low-factor"),Variables.Get<float>("gas-low-factor"))){á.ҁ(
LOCK_NAME_GeneralSection,ϼ=>{á.Ҩ(false);ϸ();Ϩ(á.Ɣ.OtherConnector);á.Ɣ.Disconnect();á.Ċ(MinerState.ReturningToShaft);});}else{á.Ċ(MinerState.
Maintenance);á.ѳ.LifetimeWentToMaintenance++;ƀ.ƅ.ƞ(10000).ƚ(()=>á.Ѵ()==MinerState.Maintenance).Ɯ(()=>{if(á.Ҏ(Variables.Get<float>(
"battery-full-factor"),0.99f)){á.Ċ(MinerState.Docking);}});}}}else{if(á.ѥ)á.ѥ=false;if(á.ѳ.StaticDockOverride.HasValue)á.Ɣ.Connect();}}if(ñ==
MinerState.Maintenance){if((á.њ!=MinerState.Docking)&&(á.Ɣ.Status==MyShipConnectorStatus.Connected)){á.ѽ(
"command:pillock-mode:Disabled");ƀ.ƅ.ƞ(10000).ƚ(()=>á.Ѵ()==MinerState.Maintenance).Ɯ(()=>{if(á.Ҏ(Variables.Get<float>("battery-full-factor"),0.99f)){á.
Ċ(MinerState.Docking);}});}}if(ñ==MinerState.ForceFinish){if(á.Ɣ.Status==MyShipConnectorStatus.Connected){if(!á.ѥ){á.ѥ=
true;У.Ƈ("ForceFinish docking handled");á.ѽ("command:pillock-mode:Disabled");á.Ă.DampenersOverride=false;á.Ɣ.OtherConnector.
CustomData="";á.Ң?.Invoke();á.Ҩ(true);á.Ϊ=LOCK_NAME_ForceFinishSection;á.Ҁ(LOCK_NAME_ForceFinishSection);á.Ϊ=
LOCK_NAME_GeneralSection;á.Ҁ(LOCK_NAME_GeneralSection);}if(!Ј()){У.Ƌ("ForceFinish: still have items");}else{á.Ċ(MinerState.Disabled);ϸ();á.ѳ.
LifetimeOperationTime+=(int)(DateTime.Now-Ϛ).TotalSeconds;á.ȑ.Save();á.ѩ=null;}}else{if(á.ѥ)á.ѥ=false;if(á.ѳ.StaticDockOverride.HasValue)á.Ɣ.
Connect();}}}bool Ј(){var Ї=new List<IMyCargoContainer>();á.Ũ.GetBlocksOfType(Ї,â=>â.IsSameConstructAs(á.Ɣ.OtherConnector)&&â.
HasInventory&&â.IsFunctional&&(â is IMyCargoContainer));var І=á.ҟ.Select(á=>á.GetInventory()).Where(ĺ=>ĺ.ItemCount>0);if(І.Any()){У.
Ƌ("Docking: still have items");foreach(var Ϙ in І){var ϣ=new List<MyInventoryItem>();Ϙ.GetItems(ϣ);for(int Ť=0;Ť<ϣ.Count;
Ť++){var Ϣ=ϣ[Ť];IMyInventory ϡ;var Ȉ=Variables.Get<string>("preferred-container");if(!string.IsNullOrEmpty(Ȉ))ϡ=Ї.Where(é
=>é.CustomName.Contains(Ȉ)).Select(á=>á.GetInventory()).FirstOrDefault();else ϡ=Ї.Select(á=>á.GetInventory()).Where(ĺ=>ĺ.
CanItemsBeAdded((MyFixedPoint)(1f),Ϣ.Type)).OrderBy(ĺ=>(float)ĺ.CurrentVolume).FirstOrDefault();if(ϡ!=null){if(!Ϙ.TransferItemTo(ϡ,ϣ[Ť]
)){У.Ƌ("Docking: failing to transfer from "+(Ϙ.Owner as IMyTerminalBlock).CustomName+" to "+(ϡ.Owner as IMyTerminalBlock)
.CustomName);}}}}return false;}return true;}void Ϥ(){Ϝ=0;if(á.Ѩ==Role.Agent){á.Ċ(MinerState.WaitingForLockInShaft);var Ϡ=
Math.Min(8,(á.Ҟ.WorldMatrix.Translation-á.ѳ.miningEntryPoint.Value).Length());var ň=á.ѳ.miningEntryPoint.Value+á.ѵ()*Ϡ;á.ѽ(
"command:create-wp:Name=WaitingForLockInShaft,Ng=Forward"+",AimNormal="+ѕ.х(á.ѵ()).Replace(':',';')+",UpNormal=1;0;0,SpeedLimit="+Variables.Get<float>("speed-clear")+":"+ѕ.х(ň))
;á.ѳ.currentWp=ň;}else if(á.Ѩ==Role.Lone){á.Ċ(MinerState.GettingOutTheShaft);á.ѽ(
"command:create-wp:Name=GettingOutTheShaft,Ng=Forward,UpNormal=1;0;0,SpeedLimit="+Variables.Get<float>("speed-clear")+":"+ѕ.х(á.ѳ.miningEntryPoint.Value));á.ѳ.currentWp=á.ѳ.miningEntryPoint;}}public
void Ϟ(Ȳ ϝ,MinerState ñ){var â=ImmutableArray.CreateBuilder<MyTuple<string,string>>(10);â.Add(new MyTuple<string,string>(
"State",ñ.ToString()));â.Add(new MyTuple<string,string>("Adaptive\nmode",κ.ƅ.φ("adaptive-mining")?"Y":"N"));â.Add(new MyTuple<
string,string>("Session\nore mined",ϛ.ToString("f2")));â.Add(new MyTuple<string,string>("Last found\nore depth",(ϙ??0f).
ToString("f2")));â.Add(new MyTuple<string,string>("Cargo\nfullness",ϭ.ToString("f2")));â.Add(new MyTuple<string,string>(
"Current\ndepth",Ϝ.ToString("f2")));â.Add(new MyTuple<string,string>("Lock\nrequested",á.Ѧ));â.Add(new MyTuple<string,string>(
"Lock\nowned",á.Ϊ));ϝ.ȧ=â.ToImmutableArray();}StringBuilder ζ=new StringBuilder();public override string ToString(){ζ.Clear();ζ.
AppendFormat("session uptime: {0}\n",(Ϛ==default(DateTime)?"-":(DateTime.Now-Ϛ).ToString()));ζ.AppendFormat(
"session ore mass: {0}\n",ϛ);ζ.AppendFormat("cargoFullness: {0:f2}\n",ϭ);ζ.AppendFormat("cargoMass: {0:f2}\n",Ϭ);ζ.AppendFormat(
"cargoYield: {0:f2}\n",ϳ);ζ.AppendFormat("lastFoundOreDepth: {0}\n",ϙ.HasValue?ϙ.Value.ToString("f2"):"-");ζ.AppendFormat(
"minFoundOreDepth: {0}\n",ϟ.HasValue?ϟ.Value.ToString("f2"):"-");ζ.AppendFormat("maxFoundOreDepth: {0}\n",ϥ.HasValue?ϥ.Value.ToString("f2"):"-");
ζ.AppendFormat("shaft id: {0}\n",á.ѳ.CurrentShaftId??-1);return ζ.ToString();}float Ϝ;public float ϛ;public DateTime Ϛ;
float?ϙ;float?ϟ{get{return á.ѳ.minFoundOreDepth;}set{á.ѳ.minFoundOreDepth=value;}}float?ϥ{get{return á.ѳ.maxFoundOreDepth;}
set{á.ѳ.maxFoundOreDepth=value;}}public float Ϯ(){return ϳ+ϵ-ϴ;}void ϸ(){ϛ+=Ϭ;á.ѳ.LifetimeOreAmount+=Ϭ;á.ѳ.LifetimeYield+=ϳ
;ϵ+=ϳ-ϴ;ϴ=0;ϳ=0;}void Ϸ(){ϴ=ϳ;ϵ=0;}float ϵ=0;float ϴ=0;float ϳ=0;bool ϲ(){float ϱ=0;for(int ĺ=0;ĺ<á.ҟ.Count;ĺ++){var ɨ=á.
ҟ[ĺ].GetInventory(0);if(ɨ==null)continue;List<MyInventoryItem>ϣ=new List<MyInventoryItem>();ɨ.GetItems(ϣ);ϣ.Where(ϰ=>ϰ.
Type.ToString().Contains("Ore")&&!ϰ.Type.ToString().Contains("Stone")).ToList().ForEach(é=>ϱ+=(float)é.Amount);}bool ϯ=false
;if((ϳ>0)&&(ϱ>ϳ)){ϯ=true;}ϳ=ϱ;return ϯ;}float ϭ;float Ϭ;bool ϫ(){float Ϫ=0;float ϩ=0;Ϭ=0;for(int ĺ=0;ĺ<á.ҟ.Count;ĺ++){var
ɨ=á.ҟ[ĺ].GetInventory(0);if(ɨ==null)continue;Ϫ+=(float)ɨ.MaxVolume;ϩ+=(float)ɨ.CurrentVolume;Ϭ+=(float)ɨ.CurrentMass;}ϭ=ϩ
/Ϫ;return ϭ>=Variables.Get<float>("cargo-full-factor");}void Ϩ(IMyShipConnector ϧ){string Ϧ=
"command:create-wp:Name=drill getAbovePt,Ng=Forward,AimNormal="+ѕ.х(á.ѵ()).Replace(':',';')+":"+ѕ.х(á.ҳ(á.ѳ.getAbovePt.Value));var Џ=á.ҳ(ϧ.WorldMatrix.Translation,ϧ.WorldMatrix.
Backward)-ϧ.WorldMatrix.Backward*Variables.Get<float>("getAbove-altitude");var А=
"[command:pillock-mode:Disabled],[command:create-wp:Name=Dock.Echelon,Ng=Forward:"+ѕ.х(Џ)+":"+Ϧ+"]";á.ѽ(А);á.ѳ.currentWp=á.ҳ(á.ѳ.getAbovePt.Value);}}}static class ѕ{public static string х(params
Vector3D[]у){return string.Join(":",у.Select(Ĝ=>string.Format("{0}:{1}:{2}",Ĝ.X,Ĝ.Y,Ĝ.Z)));}public static string т(MatrixD с){
StringBuilder ζ=new StringBuilder();for(int ĺ=0;ĺ<4;ĺ++){for(int Ļ=0;Ļ<4;Ļ++){ζ.Append(с[ĺ,Ļ]+":");}}return ζ.ToString().TrimEnd(':')
;}public static Vector3D р(MatrixD п,Vector3D о,BoundingSphereD ф){RayD Â=new RayD(п.Translation,Vector3D.Normalize(о-п.
Translation));double?н=Â.Intersects(ф);if(н.HasValue){var м=Vector3D.Normalize(ф.Center-п.Translation);if(ф.Contains(п.Translation)
==ContainmentType.Contains)return ф.Center-м*ф.Radius;var л=Vector3D.Cross(Â.Direction,м);Vector3D к;if(л.Length()<double.
Epsilon)м.CalculatePerpendicularVector(out к);else к=Vector3D.Cross(л,-м);return ф.Center+Vector3D.Normalize(к)*ф.Radius;}
return о;}public static Vector3D й(Vector3D и,MatrixD з,MatrixD ж,Vector3D е,ref Vector3D д){var Ã=з.Up;var г=Vector3D.
ProjectOnPlane(ref и,ref Ã);var ц=-(float)Math.Atan2(Vector3D.Dot(Vector3D.Cross(з.Forward,г),Ã),Vector3D.Dot(з.Forward,г));Ã=з.Right;
г=Vector3D.ProjectOnPlane(ref и,ref Ã);var і=-(float)Math.Atan2(Vector3D.Dot(Vector3D.Cross(з.Forward,г),Ã),Vector3D.Dot(
з.Forward,г));float є=0;if((е!=Vector3D.Zero)&&(Math.Abs(ц)<.05f)&&(Math.Abs(і)<.05f)){Ã=з.Forward;г=Vector3D.
ProjectOnPlane(ref е,ref Ã);є=-(float)Math.Atan2(Vector3D.Dot(Vector3D.Cross(з.Down,г),Ã),Vector3D.Dot(з.Up,г));}var ѓ=new Vector3D(і,
ц,є*Variables.Get<float>("roll-power-factor"));д=new Vector3D(Math.Abs(ѓ.X),Math.Abs(ѓ.Y),Math.Abs(ѓ.Z));var ђ=Vector3D.
TransformNormal(ѓ,з);var Ȋ=Vector3D.TransformNormal(ђ,MatrixD.Transpose(ж));Ȋ.X*=-1;return Ȋ;}public static void ё(IMyGyro ч,Vector3 в,
Vector3D Б){float ѐ=в.Y;float я=в.X;float Ǻ=в.Z;var ю=IsLargeGrid?30:60;var э=new Vector3D(1.92f,1.92f,1.92f);Func<double,double
,double,double>ь=(é,Ĝ,Ȋ)=>{var ы=Math.Abs(é);double Â;if(ы>(Ĝ*Ĝ*1.7)/(2*Ȋ))Â=ю*Math.Sign(é)*Math.Max(Math.Min(ы,1),0.002)
;else{Â=-ю*Math.Sign(é)*Math.Max(Math.Min(ы,1),0.002);}return Â*0.6;};var ъ=(float)ь(ѐ,Б.Y,э.Y);var щ=(float)ь(я,Б.X,э.X)
;var ш=(float)ь(Ǻ,Б.Z,э.Z);ч.SetValue("Pitch",щ);ч.SetValue("Yaw",ъ);ч.SetValue("Roll",ш);}public static void ɲ(IMyGyro ч
,Vector3 в,Vector3D Р,Vector3D Б){if(Variables.Get<bool>("amp")){var О=5f;var Н=2f;Func<double,double,double>М=(é,ƭ)=>é*(
Math.Exp(-ƭ*Н)+0.8)*О/2;Р/=Math.PI/180f;if((Р.X<2)&&(в.X>0.017))в.X=(float)М(в.X,Р.X);if((Р.Y<2)&&(в.Y>0.017))в.Y=(float)М(в
.Y,Р.Y);if((Р.Z<2)&&(в.Z>0.017))в.Z=(float)М(в.Z,Р.Z);}ё(ч,в,Б);}public static Vector3D И(Vector3D Л,Vector3D К,Vector3D
Ù,Vector3D Ï,Vector3D П,bool Д){double Е=Vector3D.Dot(Vector3D.Normalize(Ù-Л),П);if(Е<30)Е=30;return И(Л,К,Ù,Ï,Е,Д);}
public static Vector3D И(Vector3D З,Vector3D Ж,Vector3D Ù,Vector3D Ï,double Е,bool Д){double Г=Vector3D.Distance(З,Ù);Vector3D
В=Ù-З;Vector3D Й=Vector3D.Normalize(В);Vector3D С=Ù;Vector3D Щ;if(Д){var б=Vector3D.Reject(Ж,Й);Ï-=б;}if(Ï.Length()>float
.Epsilon){Щ=Vector3D.Normalize(Ï);var Я=Math.PI-Math.Acos(Vector3D.Dot(Й,Щ));var ȿ=(Ï.Length()*Math.Sin(Я))/Е;if(Math.Abs
(ȿ)<=1){var Ю=Math.Asin(ȿ);var ƃ=Г*Math.Sin(Ю)/Math.Sin(Я+Ю);С=Ù+Щ*ƃ;}}return С;}public static string Э(string ã,Vector3D
ë,Color á){return$"GPS:{ã}:{ë.X}:{ë.Y}:{ë.Z}:#{á.R:X02}{á.G:X02}{á.B:X02}:";}}StringBuilder Ь=new StringBuilder();void Ы(
string а){if(Ъ!=null){Ь.AppendLine(а);}}IMyTextPanel Ъ;IMyShipController Ш;void Ч(){if(Ь.Length>0){var ƃ=Ь.ToString();Ь.Clear(
);Ъ?.WriteText(ƃ);}}string Ц(float ψ,string Х=""){string Ф;if(Math.Abs(ψ)>=1000000){if(!string.IsNullOrEmpty(Х))Ф=string.
Format("{0:0.##} M{1}",ψ/1000000,Х);else Ф=string.Format("{0:0.##}M",ψ/1000000);}else if(Math.Abs(ψ)>=1000){if(!string.
IsNullOrEmpty(Х))Ф=string.Format("{0:0.##} k{1}",ψ/1000,Х);else Ф=string.Format("{0:0.##}k",ψ/1000);}else{if(!string.IsNullOrEmpty(Х)
)Ф=string.Format("{0:0.##} {1}",ψ,Х);else Ф=string.Format("{0:0.##}",ψ);}return Ф;}static class У{static string Т="";
static Action<string>ĉ;static IMyTextSurface ë;static IMyTextSurface Ι;public static double Ŧ;public static void ģ(Action<
string>F,IMyGridTerminalSystem Ŵ,IMyProgrammableBlock Ţ){ĉ=F;ë=Ţ.GetSurface(0);ë.ContentType=ContentType.TEXT_AND_IMAGE;ë.
WriteText("");}public static void Ƌ(string ƃ){if((Т=="")||(ƃ.Contains(Т)))ĉ(ƃ);}static string Ɗ="";public static void Ɖ(string ƃ)
{Ɗ+=ƃ+"\n";}static List<string>ƈ=new List<string>();public static void Ƈ(string ƃ){ë.WriteText($"{Ŧ:f2}: {ƃ}\n",true);if(
Ι!=null){ƈ.Add(ƃ);}}public static void Ɔ(){Ι?.WriteText("");ƈ.Clear();}public static void Ƅ(IMyTextSurface ƃ){Ι=ƃ;}public
static void Ƃ(){if(!string.IsNullOrEmpty(Ɗ)){var Ɓ=ɿ.ŷ.Where(é=>é.IsUnderControl).FirstOrDefault()as IMyTextSurfaceProvider;if
((Ɓ!=null)&&(Ɓ.SurfaceCount>0))Ɓ.GetSurface(0).WriteText(Ɗ);Ɗ="";}if(ƈ.Any()){if(Ι!=null){ƈ.Reverse();var Ŗ=string.Join(
"\n",ƈ)+"\n"+Ι.GetText();var U=Variables.Get<int>("logger-char-limit");if(Ŗ.Length>U)Ŗ=Ŗ.Substring(0,U-1);Ι.WriteText(
$"{Ŧ:f2}: {Ŗ}");}ƈ.Clear();}}}class ƀ{static ƀ ſ=new ƀ();ƀ(){}public static ƀ ƅ{get{ſ.Ɵ=0;ſ.ƙ=null;return ſ;}}class ž{public DateTime
ƍ;public Action Ƣ;public Func<bool>ƙ;public long ơ;}Queue<ž>Ơ=new Queue<ž>();long Ɵ;Func<bool>ƙ;public ƀ ƞ(int Ɲ){this.Ɵ
+=Ɲ;return this;}public ƀ Ɯ(Action ƛ){Ơ.Enqueue(new ž{ƍ=DateTime.Now.AddMilliseconds(Ɵ),Ƣ=ƛ,ƙ=ƙ,ơ=Ɵ});return this;}public
ƀ ƚ(Func<bool>ƙ){this.ƙ=ƙ;return this;}public void Ƙ(){if(Ơ.Count>0){У.Ƌ("Scheduled actions count:"+Ơ.Count);var á=Ơ.Peek
();if(á.ƍ<DateTime.Now){if(á.ƙ!=null){if(á.ƙ.Invoke()){á.Ƣ.Invoke();á.ƍ=DateTime.Now.AddMilliseconds(á.ơ);}else{Ơ.Dequeue
();}}else{á.Ƣ.Invoke();Ơ.Dequeue();}}}}public void Ɨ(){Ơ.Clear();Ɵ=0;ƙ=null;}}ƕ Ɩ;class ƕ{public IMyShipConnector Ɣ;
public List<IMyWarhead>Ɠ;IMyRadioAntenna ý;Ǖ ƒ;public ƻ Ƒ;public Func<string,Ĕ>Ɛ;public IMyGyro Ə;public IMyGridTerminalSystem
Ǝ;public IMyIntergridCommunicationSystem Ž;public IMyRemoteControl ů;public à Š;public è Ů;public IMyProgrammableBlock ŭ;
public IMyProgrammableBlock Ŭ;HashSet<IMyTerminalBlock>ū=new HashSet<IMyTerminalBlock>();Ŧ Ū<Ŧ>(string ã,List<IMyTerminalBlock
>ť,bool ũ=false)where Ŧ:class,IMyTerminalBlock{Ŧ Â;У.Ƌ("Looking for "+ã);var ţ=ť.Where(â=>â is Ŧ&&â.CustomName.Contains(ã
)).Cast<Ŧ>().ToList();Â=ũ?ţ.Single():ţ.FirstOrDefault();if(Â!=null)ū.Add(Â);return Â;}List<Ŧ>ŧ<Ŧ>(List<IMyTerminalBlock>ť
,string Ť=null)where Ŧ:class,IMyTerminalBlock{var ţ=ť.Where(â=>â is Ŧ&&((Ť==null)||(â.CustomName==Ť))).Cast<Ŧ>().ToList()
;foreach(var â in ţ)ū.Add(â);return ţ;}public ƕ(IMyProgrammableBlock Ţ,PersistentState š,IMyGridTerminalSystem Ũ,
IMyIntergridCommunicationSystem Ā,Func<string,Ĕ>Ŷ){Ǝ=Ũ;Ɛ=Ŷ;Ž=Ā;Func<IMyTerminalBlock,bool>ţ=â=>â.IsSameConstructAs(Ţ);var ż=new List<IMyTerminalBlock>(
);Ũ.GetBlocks(ż);ż=ż.Where(â=>ţ(â)).ToList();Ż(ż);}public void Ż(List<IMyTerminalBlock>ź){var ţ=ź;У.Ƈ("subset: "+ź.Count)
;ŭ=Ū<IMyProgrammableBlock>("a-thrust-provider",ţ);var Ź=ŧ<IMyMotorStator>(ţ);var Ÿ=new List<IMyProgrammableBlock>();Ǝ.
GetBlocksOfType(Ÿ,Ļ=>Ź.Any(é=>(é.Top!=null)&&é.Top.CubeGrid==Ļ.CubeGrid));Ŭ=Ū<IMyProgrammableBlock>("a-tgp",ţ)??Ÿ.FirstOrDefault(é=>é.
CustomName.Contains("a-tgp"));Ə=Ū<IMyGyro>(ForwardGyroTag,ţ,true);var ŷ=ŧ<IMyShipController>(ţ);ɿ.ģ(ŷ);ý=ŧ<IMyRadioAntenna>(ţ).
FirstOrDefault();Ɣ=ŧ<IMyShipConnector>(ţ).First();Ɠ=ŧ<IMyWarhead>(ţ);ů=ŧ<IMyRemoteControl>(ţ).First();ů.CustomData="";var ü=ŧ<
IMyTimerBlock>(ţ);Ů=new è(ü);Ų=new List<IMyTerminalBlock>();Ų.AddRange(ŧ<IMyThrust>(ţ));Ų.AddRange(ŧ<IMyArtificialMassBlock>(ţ));
string ŵ=Variables.Get<string>("ggen-tag");if(!string.IsNullOrEmpty(ŵ)){var Ŵ=new List<IMyGravityGenerator>();var ų=Ǝ.
GetBlockGroupWithName(ŵ);if(ų!=null)ų.GetBlocksOfType(Ŵ,â=>ţ.Contains(â));foreach(var â in Ŵ)ū.Add(â);Ų.AddRange(Ŵ);}else Ų.AddRange(ŧ<
IMyGravityGenerator>(ţ));Š=new à(ů,Ů,Ž,ŭ,Ə,ý,Ű,this,true);ƒ=new Ǖ(this,Ɛ);Ċ(ApckState.Standby);Š.Ċ(à.ß.æ);}List<IMyTerminalBlock>Ų;ʍ ç;int
ű;public ʍ Ű(){if(ç==null)ç=new ʍ(Ə,Ų);else if((ĵ!=ű)&&(ĵ%60==0)){ű=ĵ;if(Ų.Any(é=>!é.IsFunctional)){Ų.RemoveAll(é=>!é.
IsFunctional);ç=new ʍ(Ə,Ų);}}if(Ų.Any(é=>é is IMyThrust&&(é as IMyThrust)?.MaxEffectiveThrust!=(é as IMyThrust)?.MaxThrust))ç.ʝ();
return ç;}public Vector3D?ǎ;public Vector3D?Ǎ;public bool ǌ(){if(Š.A!=null){if(Ǎ==null){Vector3D Ì;if(ů.TryGetPlanetPosition(
out Ì)){Ǎ=Ì;return true;}}return Ǎ.HasValue;}return false;}public void Ĉ(string ć){ApckState ƃ;if(Enum.TryParse(ć,out ƃ)){Ċ
(ƃ);}}public void Ċ(ApckState ƃ){if(ƒ.Ĉ(ƃ)){Ƒ=ƒ.ƪ();if(ý!=null)ý.CustomName=
$"{Ə.CubeGrid.CustomName}> {ƒ.ƥ().Ư} / {ǅ?.Value?.đ}";}}public void ǋ(ApckState Ʀ,ƻ â){ƒ.ƫ(Ʀ,â);}public ƻ Ǌ(ApckState Ʀ){return ƒ.Ƨ(Ʀ).ò;}public void ǉ(ɻ Ŗ){У.Ƈ("CreateWP "+
Ŗ.đ);ǘ(Ŗ);}public void ǈ(int ĵ,Action<string>ĉ){this.ĵ=ĵ;Ǆ();var Ǉ=ǅ?.Value;ƻ D;if((Ǉ!=null)&&(ƒ.ƥ().Ư==Ǉ.ʢ))D=Ǉ.ò??Ǌ(Ǉ.ʢ
);else D=Ƒ;Š.I(ĵ,ĉ,D);}LinkedList<ɻ>ǆ=new LinkedList<ɻ>();LinkedListNode<ɻ>ǅ;int ĵ;void Ǆ(){if(ǅ!=null){var Ŗ=ǅ.Value;if(
Ŗ.ʺ(ĵ,Š)){У.Ƈ($"TFin {Ŗ.đ}");Ǘ();}}}public ɻ Ǚ(){return ǅ?.Value;}public void ǘ(ɻ Ŗ){ǆ.AddFirst(Ŗ);У.Ƈ(
$"Added {Ŗ.đ}, total: {ǆ.Count}");ǅ=ǆ.First;if(ǅ.Next==null)ǅ.Value.ʷ=ƒ.ƥ().Ư;Ŗ.ģ(Š,ĵ);Ċ(Ŗ.ʢ);}public void Ǘ(){var ǖ=ǅ;var á=ǖ.Value;á.ʹ?.Invoke();if(ǖ.
Next!=null){á=ǖ.Next.Value;ǅ=ǖ.Next;á.ģ(Š,ĵ);ǆ.Remove(ǖ);}else{ǅ=null;ǆ.Clear();Ċ(ǖ.Value.ʷ);}}}class Ǖ{ƕ ǔ;Dictionary<
ApckState,ƣ>Ǔ=new Dictionary<ApckState,ƣ>();public Ǖ(ƕ ǒ,Func<string,Ĕ>Ŏ){ǔ=ǒ;var Ǒ=ǔ.Š;var ǐ=new ƻ{đ="Default"};foreach(var ƃ in
Enum.GetValues(typeof(ApckState))){Ǔ.Add((ApckState)ƃ,new ƣ((ApckState)ƃ,ǐ));}Ǔ[ApckState.Standby].ò=new ƻ{đ="Standby"};Ƥ=Ǔ[
ApckState.Standby];Ǔ[ApckState.Formation].ò=new ƻ{đ="follow formation",ƺ=false,í=()=>Ŏ("wingman"),Ƴ=(Ʋ)=>Ǒ.P.GetPosition()+Ŏ(
"wingman").č.Value.Forward*5000,ƹ=ë=>{var Ǐ=new BoundingSphereD(Ŏ("wingman").č.Value.Translation,30);return ѕ.р(Ǒ.P.WorldMatrix,ë
,Ǐ);},ư=()=>Ǒ.P.GetPosition()};Ǔ[ApckState.Brake].ò=new ƻ{đ="reverse",î=true,ƹ=Ʋ=>Ǒ.Ő(-150),Ƴ=(Ʋ)=>Ǒ.Ő(1),ï=true};Ǔ[
ApckState.DockingAwait].ò=new ƻ{đ="awaiting docking",ƺ=false,î=true,í=()=>Ŏ("wingman"),Ƴ=Ʋ=>Ǒ.P.GetPosition()+Ǒ.P.WorldMatrix.
Forward,ƹ=Ʋ=>Ŏ("wingman").ď.HasValue?Ŏ("wingman").ď.Value:Ǒ.P.GetPosition(),ƶ=(ƭ,Ƭ,á,Ô,U)=>{if(Ŏ("docking").ď.HasValue&&(ǔ.Ɣ!=
null)){U.Ċ(ApckState.DockingFinal);}}};Ǔ[ApckState.DockingFinal].ò=new ƻ{Ƴ=Ʋ=>ǔ.Ɣ.GetPosition()-Ŏ("docking").č.Value.Forward
*10000,ƹ=ë=>ë+Ŏ("docking").č.Value.Forward*(IsLargeGrid?1.25f:0.5f),Ʊ=()=>ǔ.Ɣ.WorldMatrix,ư=()=>ǔ.Ɣ.GetPosition(),í=()=>Ŏ
("docking"),ƺ=false,Ƶ=()=>Ǒ.Û,ƶ=(ƭ,Ƭ,á,Ô,U)=>{if((ƭ<20)&&(á.õ.Length()<0.8)&&(ǔ.Ɣ!=null)){ǔ.Ɣ.Connect();if(ǔ.Ɣ.Status==
MyShipConnectorStatus.Connected){U.Ċ(ApckState.Inert);ǔ.Ɣ.OtherConnector.CustomData="";á.Z.DampenersOverride=false;}}}};Ǔ[ApckState.Inert].ǂ=
ƃ=>ǒ.Š.Ċ(à.ß.Ý);Ǔ[ApckState.Inert].ǃ=ƃ=>ǒ.Š.Ċ(à.ß.æ);}public void ƫ(ApckState Ʀ,ƻ D){Ǔ[Ʀ].ò=D;}public ƻ ƪ(){return Ƥ.ò;}
public bool Ĉ(ApckState Ʀ){if(Ʀ==Ƥ.Ư)return true;var Ŗ=Ƨ(Ʀ);if(Ŗ!=null){var á=Ʃ.FirstOrDefault(é=>é.ƾ.Ư==Ƥ.Ư||é.ƽ.Ư==Ƥ.Ư||é.ƽ.
Ư==Ʀ||é.ƾ.Ư==Ʀ);if(á!=null){if(!(á.ƾ==Ƥ&&á.ƽ==Ŗ)){return false;}}var ƨ=Ƥ;У.Ƈ($"{ƨ.Ư} -> {Ŗ.Ư}");Ƥ=Ŗ;ƨ.ǃ?.Invoke(Ƥ.Ư);á?.Ƽ
?.Invoke();Ŗ.ǂ?.Invoke(Ŗ.Ư);return true;}return false;}public ƣ Ƨ(ApckState Ʀ){return Ǔ[Ʀ];}public ƣ ƥ(){return Ƥ;}ƣ Ƥ;
List<ƿ>Ʃ=new List<ƿ>();public class ƣ{public ApckState Ư;public Action<ApckState>ǃ;public Action<ApckState>ǂ;public ƻ ò;
public ƣ(ApckState Ʀ,ƻ â,Action<ApckState>ǁ=null,Action<ApckState>ǀ=null){ò=â;Ư=Ʀ;ǃ=ǁ;ǂ=ǀ;}}public class ƿ{public ƣ ƾ;public ƣ
ƽ;public Action Ƽ;}}class ƻ{public string đ="Default";public bool ƺ=true;public Func<Vector3D,Vector3D>ƹ{get;set;}public
Func<Vector3D,Vector3D>Ƹ{get;set;}public Func<Vector3D>Ʒ{get;set;}public Action<double,double,à,ƻ,ƕ>ƶ{get;set;}public Func<
Vector3D>Ƶ;public bool ƴ=false;public Func<Vector3D,Vector3D>Ƴ=(Ʋ)=>Ʋ;public Func<MatrixD>Ʊ;public Func<Vector3D>ư;public bool Ø
;public float?ð;public bool Á=false;public bool ï=false;public bool î;public Func<Ĕ>í;public static ƻ ì(Vector3D ë,string
ã,Func<Vector3D>ê=null){return new ƻ(){đ=ã,î=true,ƹ=é=>ë,Ƴ=é=>ê?.Invoke()??ë};}}class è{Dictionary<string,IMyTimerBlock>å
=new Dictionary<string,IMyTimerBlock>();List<IMyTimerBlock>ç;public è(List<IMyTimerBlock>å){ç=å;}public bool ä(string ã){
IMyTimerBlock â;if(!å.TryGetValue(ã,out â)){â=ç.FirstOrDefault(á=>á.CustomName.Contains(ã));if(â!=null)å.Add(ã,â);else return false;}
â.GetActionWithName("TriggerNow").Apply(â);return true;}}class à{public enum ß{Þ=0,Ý,æ}public bool Ü;ß ñ=ß.Þ;public void
Ċ(ß Ć){if(Ć==ß.æ)ą();else if(Ć==ß.Ý)Ą(false);else if(Ć==ß.Þ)Ą();ñ=Ć;}public void Ĉ(string ć){ß Ć;if(Enum.TryParse(ć,out Ć
))Ċ(Ć);}void ą(){Z.DampenersOverride=false;}void Ą(bool ă=true){Q.GyroOverride=false;V().ʄ();Z.DampenersOverride=ă;}
public à(IMyRemoteControl Ă,è ā,IMyIntergridCommunicationSystem Ā,IMyProgrammableBlock ÿ,IMyGyro þ,IMyTerminalBlock ý,Func<ʍ>ü
,ƕ û,bool ú){Z=Ă;X=Ā;Q=þ;µ=ý;Y=ā;W=ÿ;V=ü;U=û;Ü=ú;}Vector3D ù;public string ø;public Vector3D ö{get;private set;}public
Vector3D õ{get;private set;}Vector3D ô=Vector3D.Zero;public double ó{get;private set;}public ƻ ò{get;private set;}public
Vector3D Û{get{return Z.GetShipVelocities().LinearVelocity;}}Vector3D?º;public Vector3D?A{get{return(º!=Vector3D.Zero)?º:null;}}
Vector3D ª{get;set;}public Vector3D w{get;set;}public IMyRemoteControl Z;public è Y{get;private set;}
IMyIntergridCommunicationSystem X;IMyProgrammableBlock W;Func<ʍ>V;ƕ U;int S;IMyGyro Q;IMyTerminalBlock µ;public IMyTerminalBlock P{get{return Q;}}
public Vector3D N;public Vector3D M;public Vector3D L;public Vector3D K;public Vector3D J;public void I(int H,Action<string>F,
ƻ D){var B=H-S;S=H;if(B>0)w=(Û-ª)*60f/B;ª=Û;º=Z.GetNaturalGravity();ò=D;MyPlanetElevation O=new MyPlanetElevation();
double À;Z.TryGetPlanetElevation(O,out À);Vector3D Ì;Z.TryGetPlanetPosition(out Ì);Func<Vector3D>Ú=null;bool Ø=false;float?Ö=
null;Ĕ Õ=null;switch(ñ){case ß.Þ:return;case ß.æ:try{var Ô=D;if(Ô==null){Ċ(ß.Þ);return;}if(!Ü&&!Ô.ƴ)return;var Ó=Vector3D.
TransformNormal(Z.GetShipVelocities().AngularVelocity,MatrixD.Transpose(P.WorldMatrix));Ó=new Vector3D(Math.Abs(Ó.X),Math.Abs(Ó.Y),Math
.Abs(Ó.Z));var Ò=(Ó-ô)/Dt;ô=Ó;Ú=Ô.Ƶ;var Ñ=Ô.Ƴ;Ø=Ô.Ø;Ö=Ô.ð;if(Ô.í!=null)Õ=Ô.í();if(Ô.î||((Õ!=null)&&Õ.ď.HasValue)){
Vector3D Ð;Vector3D?Ï=null;if((Õ!=null)&&(Õ.ď.HasValue)){Ð=Õ.ď.Value;if(Ï.IsValid())Ï=Õ.Û;else У.Ƈ("Ivalid targetVelocity");}
else Ð=Vector3D.Zero;if(Ô.Ƹ!=null)Ð=Ô.Ƹ(Ð);var Î=(Ô.ư!=null)?Ô.ư():P.GetPosition();if((Ú!=null)&&(Ï.HasValue)&&(Ï.Value.
Length()>0)){Vector3D Ù=Ð;Vector3D Í=ѕ.И(Î,Û,Ù,Ï.Value,Ú(),Ô.Á);if((Ð-Í).Length()<2500){Ð=Í;}M=Í;}N=Ð;if(Ô.ƹ!=null)K=Ô.ƹ(Ð);
else K=Ð;double Ë=(Ð-Î).Length();double Ê=(K-Î).Length();if(DbgIgc!=0)ø=$"origD: {Ë:f1}\nshiftD: {Ê:f1}";Ô.ƶ?.Invoke(Ë,Ê,
this,Ô,U);Q.GyroOverride=true;J=Ð;if(Ñ!=null)J=Ñ(J);if((Variables.Get<bool>("hold-thrust-on-rotation")&&(ù.Length()>1))||κ.ƅ
.φ("suppress-transition-control")){У.Ƌ($"prev cv: {ù.Length():f2} HOLD");ċ(P.WorldMatrix.Translation,P.WorldMatrix.
Translation,false,null,null,false);}else{У.Ƌ($"prev cv: {ù.Length():f2} OK");ċ(Î,K,Ø,Ï,Ö,Ô.ï);}var É=(Ô.Ʊ!=null)?Ô.Ʊ():P.
WorldMatrix;if(!Ü&&(L!=P.WorldMatrix.Translation))J=L;Vector3D È=Vector3D.Zero;var Ç=J-P.WorldMatrix.Translation;if(Ç!=Vector3D.
Zero){var Æ=Vector3D.Normalize(Ç);var Å=MatrixD.CreateFromDir(Æ);Vector3D Ä=Vector3D.Zero;var Ã=Ô.Ʒ?.Invoke()??Vector3D.Zero
;È=ѕ.й(Æ,É,P.WorldMatrix,Ã,ref Ä);Ä.Z=0;õ=Ä;ö=õ-ù;ù=õ;ó=Vector3D.Dot(Æ,É.Forward);}if(!κ.ƅ.φ("suppress-gyro-control"))ѕ.ɲ
(Q,(Vector3)È,ö,ô);}else{Q.GyroOverride=false;if(κ.ƅ.φ("damp-when-idle"))ċ(P.WorldMatrix.Translation,P.WorldMatrix.
Translation,false,null,0,false);else ċ(P.WorldMatrix.Translation,P.WorldMatrix.Translation,false,null,null,false);}}catch(Exception
ex){µ.CustomName+="HC Exception! See remcon cdata or PB screen";var Â=Z;var ĉ=
$"HC EPIC FAIL\nNTV:{Õ?.đ}\nBehavior:{ò.đ}\n{ex}";Â.CustomData+=ĉ;У.Ƈ(ĉ);Ċ(ß.Þ);throw ex;}finally{У.Ƃ();}break;}}void ċ(Vector3D Ş,Vector3D Ō,bool Ø,Vector3D?Ŋ,float?Ö,
bool ŉ){L=Ō;if(ñ!=ß.æ)return;var ň=Ō;var Ň=Q.WorldMatrix;Ň.Translation=Ş;var Ç=Ō-Ň.Translation;var ņ=MatrixD.Transpose(Ň);
var Ņ=Vector3D.TransformNormal(Û,ņ);var ń=Ņ;if(!Ü&&(Ç!=Vector3D.Zero)){V().ɷ().ɱ(1f*Math.Max(0.2f,ó));if(Û!=Vector3D.Zero)L
=Vector3D.Normalize(Vector3D.Reflect(Û,Ç))+Ş+Vector3D.Normalize(Ç)*Û.Length()*0.5f;return;}float ŋ=Z.CalculateShipMass().
PhysicalMass;BoundingBoxD Ń=V().ʠ(ŋ);if(Ń.Volume==0)return;Vector3D ł=Vector3D.Zero;if(A!=null){ł=Vector3D.TransformNormal(A.Value,ņ
);Ń+=-ł;}Vector3D Ł=Vector3D.Zero;Vector3D ŀ=Vector3D.Zero;Vector3D Ŀ=new Vector3D();if(Ç.Length()>double.Epsilon){
Vector3D ľ=Vector3D.TransformNormal(Ç,ņ);RayD Ľ=new RayD(-ľ*(MaxAccelInProximity?1000:1),Vector3D.Normalize(ľ));RayD ļ=new RayD(
ľ*(MaxBrakeInProximity?1000:1),Vector3D.Normalize(-ľ));var Ļ=ļ.Intersects(Ń);var ĺ=Ľ.Intersects(Ń);if(!Ļ.HasValue||!ĺ.
HasValue)throw new InvalidOperationException("Not enough thrust to compensate for gravity");var Ĺ=ļ.Position+(Vector3D.Normalize
(ļ.Direction)*Ļ.Value);var Ð=Ľ.Position+(Vector3D.Normalize(Ľ.Direction)*ĺ.Value);var ĸ=Ĺ.Length();Vector3D ō=Vector3D.
Reject(Ņ,Vector3D.Normalize(ľ));if(Ŋ.HasValue){var ş=Vector3D.TransformNormal(Ŋ.Value,ņ);ń=Ņ-ş;ō=Vector3D.Reject(ń,Vector3D.
Normalize(ľ));}else{ń-=ō;}var ŝ=Vector3D.Dot(ń,Vector3D.Normalize(ľ));bool Ŝ=ŝ>0;bool ś=true;var Ś=Math.Pow(Math.Max(0,ŝ),2)/(2*ĸ
*StoppingPowerQuotient);var ř=Ç.Length()-Ś;if(DbgIgc!=0){ø+=$"\nSTP: {Ś:f2}\nRelSP: {ŝ:f2}";}if(Ŝ){if(Ś>Ç.Length())ś=
false;else if(MoreRejectDampening)ō/=Dt;}if(ŉ||ś){if(Ö.HasValue&&(Vector3D.Dot(Vector3D.Normalize(Ç),Û)>=Ö)){Ŀ=Ĺ;Ŀ*=(ŝ-Ö.
Value)/ĸ;}else Ŀ=Ð;}else Ŀ=Ĺ;if(ś){var Ř=Vector3D.Dot(Vector3D.Normalize(Ç),Û);if(Ř>MAX_SP-0.001){Ŀ=Vector3D.Zero;}}ŀ=Ŀ;Ł=ō;
if(ō.IsValid())Ŀ+=ō;}else if(Ö.HasValue&&(Ö==0)){Ŀ+=Ņ/(MoreRejectDampening?Dt:1);}if(A!=null){Ŀ+=ł;}Ŀ-=ɿ.ɽ(P.WorldMatrix)*
1000;if(Ŀ!=Vector3D.Zero){L=Vector3D.TransformNormal(Ŀ,Ň)+Ş;if(DbgIgc!=0){var ŗ=new List<MyTuple<Vector3D,Vector3D,Vector4>>
();var á=Color.SeaGreen;á.A=40;ŗ.Add(new MyTuple<Vector3D,Vector3D,Vector4>(Ş,L,á));var Ŗ=new MyTuple<string,Vector2,
Vector3D,Vector3D,float,string>("Circle",Vector2.One*4,L,Vector3D.Zero,1f,Ŀ.Length().ToString("f2"));X.SendUnicastMessage(DbgIgc
,"draw-projection",Ŗ);á=Color.Blue;á.A=40;ŗ.Add(new MyTuple<Vector3D,Vector3D,Vector4>(Ş,Vector3D.TransformNormal(Ł,Ň)+Ş,
á));á=Color.Red;á.A=40;ŗ.Add(new MyTuple<Vector3D,Vector3D,Vector4>(Ş,Vector3D.TransformNormal(ŀ,Ň)+Ş,á));var ŕ=new RayD(
Ŀ*1000,Vector3D.Normalize(-Ŀ));var ĺ=ŕ.Intersects(Ń);if(ĺ.HasValue){var Œ=ŕ.Position+(Vector3D.Normalize(ŕ.Direction)*ĺ.
Value);var ę=Vector3D.TransformNormal(Œ,Ň)+Ş;var ő=new MyTuple<string,Vector2,Vector3D,Vector3D,float,string>("Circle",
Vector2.One*4,ę,Vector3D.Zero,1f,Œ.Length().ToString("f2"));X.SendUnicastMessage(DbgIgc,"draw-projection",ő);}var Ŕ=new RayD(-Ŀ
*1000,Vector3D.Normalize(Ŀ));var œ=Ŕ.Intersects(Ń);if(œ.HasValue){var Œ=Ŕ.Position+(Vector3D.Normalize(Ŕ.Direction)*œ.
Value);var ę=Vector3D.TransformNormal(Œ,Ň)+Ş;var ő=new MyTuple<string,Vector2,Vector3D,Vector3D,float,string>("Circle",
Vector2.One*4,ę,Vector3D.Zero,1f,Œ.Length().ToString("f2"));X.SendUnicastMessage(DbgIgc,"draw-projection",ő);}X.
SendUnicastMessage(DbgIgc,"draw-lines",ŗ.ToImmutableArray());}}Ŀ.Y*=-1;V().ɲ(Ŀ,ŋ);}public Vector3D Ő(float ŏ){return P.GetPosition()+P.
WorldMatrix.Forward*ŏ;}}Ĕ Ŏ(string ğ){Ĕ Â;if(ě.TryGetValue(ğ,out Â))return Â;throw new InvalidOperationException("No TV named "+ğ);
}void ķ(string ğ,Ĕ Č){if(!ě.ContainsKey(ğ))ě.Add(ğ,Č);else ě[ğ]=Č;ȸ.ȶ++;}void Ġ(string ğ,MyTuple<MyTuple<string,long,long
,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>Ğ){ě[ğ].Ħ(Ğ,ξ);ȸ.ȶ++;}void ĝ(){foreach(var é in ě.Values.Where(Ĝ=>Ĝ.ď.
HasValue)){У.Ƌ(é.đ+((é.ď.Value==Vector3D.Zero)?" Zero!":" OK"));é.ı(ξ);}}Dictionary<string,Ĕ>ě=new Dictionary<string,Ĕ>();struct
Ě{public Vector3D?ę;public Vector3D?Ę;public MatrixD?ġ;public BoundingBoxD?ė;public MyDetectedEntityType?ĕ;}class Ĕ{
public long ē;int Ē;public string đ;public long Đ;public Vector3D?ď{get;private set;}public Vector3D?Û;public Vector3D?Ď;
public MatrixD?č;public BoundingBoxD?Ė;public int?Ģ=60;public MyDetectedEntityType?Į{get;set;}public delegate void Ķ();public
event Ķ Ĵ;public Ĕ(int Ē,string ã){this.Ē=Ē;đ=ã;}public void ĳ(Vector3D ę,long Ĳ){ď=ę;ē=Ĳ;}public void ı(int İ){if((ē!=0)&&Ģ.
HasValue&&(İ-ē>Ģ.Value))ʏ();}public void į(int ĵ,int Ē){if((Û.HasValue)&&(Û.Value.Length()>double.Epsilon)&&(ĵ-ē)>0){ď+=Û*(ĵ-ē)*
Ē/60;ȸ.ƌ++;}}public enum ĭ:byte{Ĭ=1,ī=2,Ī=4}bool ĩ(ĭ Ĩ,ĭ ħ){return(Ĩ&ħ)==ħ;}public void Ħ(MyTuple<MyTuple<string,long,
long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>ĥ,int Ĥ){var Ʈ=ĥ.Item1;Đ=Ʈ.Item2;Į=(MyDetectedEntityType)Ʈ.Item4;ĭ ǚ=
(ĭ)Ʈ.Item5;ĳ(ĥ.Item2,Ĥ);if(ĩ(ǚ,ĭ.Ĭ)){var ʬ=ĥ.Item3;if(!Û.HasValue)Û=ʬ;Ď=(ʬ-Û.Value)*60/Ē;Û=ʬ;}if(ĩ(ǚ,ĭ.ī))č=ĥ.Item4;if(ĩ(
ǚ,ĭ.Ī))Ė=ĥ.Item5;ȸ.ȶ++;}public static Ĕ ʔ(MyTuple<MyTuple<string,long,long,byte,byte>,Vector3D,Vector3D,MatrixD,
BoundingBoxD>ĥ,Func<string[],Ě>ʒ,int Ĥ){var Ŗ=new Ĕ(1,ĥ.Item1.Item1);Ŗ.Ħ(ĥ,Ĥ);return Ŗ;}public MyTuple<MyTuple<string,long,long,byte
,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>ʑ(){var ʐ=0|(Û.HasValue?1:0)|(č.HasValue?2:0)|(Ė.HasValue?4:0);var é=new
MyTuple<MyTuple<string,long,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>(new MyTuple<string,long,long,byte,byte>(đ,Đ
,DateTime.Now.Ticks,(byte)MyDetectedEntityType.LargeGrid,(byte)ʐ),ď.Value,Û??Vector3D.Zero,č??MatrixD.Identity,Ė??new
BoundingBoxD());return é;}public void ʏ(){ď=null;Û=null;č=null;Ė=null;var ʎ=Ĵ;if(ʎ!=null)Ĵ();ȸ.ȵ++;}}class ʍ{List<ʂ>ʓ;List<ʂ>ʌ;List<
ʂ>ʋ;List<ʂ>ʊ;List<ʂ>ʉ;List<ʂ>ʈ;List<ʂ>ʇ;public double[]ʆ=new double[6];bool ʅ;public void ʄ(){if(!ʅ){ʌ.ForEach(Ȋ=>Ȋ.ʀ());
ʋ.ForEach(Ȋ=>Ȋ.ʀ());ʊ.ForEach(Ȋ=>Ȋ.ʀ());ʉ.ForEach(Ȋ=>Ȋ.ʀ());ʈ.ForEach(Ȋ=>Ȋ.ʀ());ʇ.ForEach(Ȋ=>Ȋ.ʀ());ʅ=true;}}public
BoundingBoxD ʠ(float ŋ){Vector3D ʟ=new Vector3D(-ʆ[5],-ʆ[3],-ʆ[1])/ŋ;Vector3D ʞ=new Vector3D(ʆ[4],ʆ[2],ʆ[0])/ŋ;return new
BoundingBoxD(ʟ,ʞ);}public void ʝ(){ʆ[0]=ɷ().ɯ();ʆ[1]=ɩ().ɯ();ʆ[2]=ɶ().ɯ();ʆ[3]=ɵ().ɯ();ʆ[4]=ɳ().ɯ();ʆ[5]=ɴ().ɯ();}public ʍ(
IMyTerminalBlock ʜ,List<IMyTerminalBlock>ʛ){MatrixD ʚ=ʜ.WorldMatrix;Func<Vector3D,List<ʂ>>ʙ=ʕ=>{var Â=ʛ.Where(â=>â is IMyThrust&&ʕ==â.
WorldMatrix.Forward).Select(é=>é as IMyThrust).ToList();return Â.Select(Ŗ=>new ʃ(Ŗ)).Cast<ʂ>().ToList();};ʌ=ʙ(ʚ.Backward);ʋ=ʙ(ʚ.
Forward);ʊ=ʙ(ʚ.Down);ʉ=ʙ(ʚ.Up);ʈ=ʙ(ʚ.Left);ʇ=ʙ(ʚ.Right);var ʘ=ʛ.Where(â=>â is IMyArtificialMassBlock).Cast<
IMyArtificialMassBlock>().ToList();var ʗ=ʛ.Where(â=>â is IMyGravityGenerator).Cast<IMyGravityGenerator>().ToList();Func<Vector3D,bool,List<ʂ>>
ʖ=(ʕ,ɨ)=>{var Ŵ=ʗ.Where(â=>ʕ==â.WorldMatrix.Up);return Ŵ.Select(ë=>new ɬ(ë,ʘ,ɨ)).Cast<ʂ>().ToList();};ʌ.AddRange(ʖ(ʚ.
Forward,true));ʋ.AddRange(ʖ(ʚ.Forward,false));ʌ.AddRange(ʖ(ʚ.Backward,false));ʋ.AddRange(ʖ(ʚ.Backward,true));ʊ.AddRange(ʖ(ʚ.Up,
true));ʉ.AddRange(ʖ(ʚ.Up,false));ʊ.AddRange(ʖ(ʚ.Down,false));ʉ.AddRange(ʖ(ʚ.Down,true));ʈ.AddRange(ʖ(ʚ.Right,true));ʇ.
AddRange(ʖ(ʚ.Right,false));ʈ.AddRange(ʖ(ʚ.Left,false));ʇ.AddRange(ʖ(ʚ.Left,true));ʝ();}public ʍ ɷ(){ʓ=ʌ;return this;}public ʍ ɩ(
){ʓ=ʋ;return this;}public ʍ ɶ(){ʓ=ʊ;return this;}public ʍ ɵ(){ʓ=ʉ;return this;}public ʍ ɴ(){ʓ=ʈ;return this;}public ʍ ɳ()
{ʓ=ʇ;return this;}public void ɲ(Vector3D Ĝ,float ŋ){ʅ=false;Func<ʂ,bool>ɰ=Ȋ=>!(Ȋ is ɬ);ɩ().ɱ(-Ĝ.Z/ʆ[1]*ŋ);ɷ().ɱ(Ĝ.Z/ʆ[0]*
ŋ);ɵ().ɱ(-Ĝ.Y/ʆ[3]*ŋ);ɶ().ɱ(Ĝ.Y/ʆ[2]*ŋ);ɴ().ɱ(-Ĝ.X/ʆ[5]*ŋ);ɳ().ɱ(Ĝ.X/ʆ[4]*ŋ);}public bool ɱ(double Ơ,Func<ʂ,bool>ɰ=null){
if(ʓ!=null){Ơ=Math.Min(1,Math.Abs(Ơ))*Math.Sign(Ơ);foreach(var ɭ in ɰ==null?ʓ:ʓ.Where(ɰ)){ɭ.ɱ(Ơ);}}ʓ=null;return true;}
public float ɯ(){float ɮ=0;if(ʓ!=null){foreach(var ɭ in ʓ){ɮ+=ɭ.ʁ();}}ʓ=null;return ɮ;}}class ɬ:ʂ{IMyGravityGenerator Ŵ;List<
IMyArtificialMassBlock>ɫ;bool ɪ;public ɬ(IMyGravityGenerator Ŵ,List<IMyArtificialMassBlock>ɫ,bool ɪ){this.Ŵ=Ŵ;this.ɫ=ɫ;this.ɪ=ɪ;}public void ɱ
(double ɾ){if(ɾ>=0)Ŵ.GravityAcceleration=(float)(ɪ?-ɾ:ɾ)*G;}public void ʀ(){Ŵ.GravityAcceleration=0;}public float ʁ(){
return ɫ.Count*50000*G;}}class ʃ:ʂ{IMyThrust Ŗ;public ʃ(IMyThrust Ŗ){this.Ŗ=Ŗ;}public void ɱ(double ɾ){if(ɾ<=0)Ŗ.
ThrustOverride=0.00000001f;else Ŗ.ThrustOverride=(float)ɾ*Ŗ.MaxThrust;}public void ʀ(){Ŗ.ThrustOverride=0;Ŗ.Enabled=true;}public float
ʁ(){return Ŗ.MaxEffectiveThrust;}}interface ʂ{void ɱ(double ɾ);float ʁ();void ʀ();}static class ɿ{public static List<
IMyShipController>ŷ;public static void ģ(List<IMyShipController>á){if(ŷ==null)ŷ=á;}public static Vector3 ɽ(MatrixD ɼ){Vector3 Ȱ=new
Vector3();if(κ.ƅ.φ("ignore-user-thruster"))return Ȱ;var á=ŷ.Where(é=>é.IsUnderControl).FirstOrDefault();if(á!=null&&(á.
MoveIndicator!=Vector3.Zero))return(Vector3)Vector3D.TransformNormal(á.MoveIndicator,ɼ*MatrixD.Transpose(á.WorldMatrix));return Ȱ;}}
class ɻ{public string đ;public Vector3D?ɺ;public Func<Vector3D>ɹ;double?ʡ;public int?ɸ;public ApckState ʢ=ApckState.CwpTask;
public ƻ ò;public Action ʹ;int ʸ;public ApckState ʷ;public ɻ(string ã,ƻ D,int?ʶ=null){ò=D;đ=ã;ɸ=ʶ;}public ɻ(string ã,ApckState
Ʀ,int?ʶ=null){ʢ=Ʀ;đ=ã;ɸ=ʶ;}public void ģ(à Š,int ĵ){if(ʸ==0)ʸ=ĵ;Š.Y.ä(đ+".OnStart");}public static ɻ ʵ(string ã,Vector3D
ë,ƻ D){var Ŗ=new ɻ(ã,D);Ŗ.ʡ=0.5;Ŗ.ɺ=ë;return Ŗ;}public static ɻ ʴ(string ã,Func<Vector3D>ʳ,ƻ D){var Ŗ=new ɻ(ã,D);Ŗ.ʡ=0.5;
Ŗ.ɹ=ʳ;return Ŗ;}public bool ʺ(int ĵ,à Š){if(ɸ.HasValue&&(ĵ-ʸ>ɸ)){return true;}if(ʡ.HasValue){Vector3D ë;var ǖ=ò.ư?.Invoke
()??Š.P.GetPosition();if(ɹ!=null)ë=ɹ();else ë=ɺ.Value;if((ǖ-ë).Length()<ʡ)return true;}return false;}}ɻ ˁ;void ǉ(string[]
ˀ){ʩ();var ʿ=Ɩ;var Ǒ=ʿ.Š;var ʾ=Ǒ.P.GetPosition();var ʽ=ˀ[2].Split(',').ToDictionary(ƃ=>ƃ.Split('=')[0],ƃ=>ƃ.Split('=')[1]
);var D=new ƻ(){đ="Deserialized Behavior",î=true,ƺ=false};ˁ=new ɻ("twp",D);float ʉ=1;var ʼ=ˀ.Take(6).Skip(1).ToArray();
var ę=new Vector3D(double.Parse(ʼ[2]),double.Parse(ʼ[3]),double.Parse(ʼ[4]));Func<Vector3D,Vector3D>ʻ=ë=>ę;ˁ.ɺ=ę;Vector3D?Ť
=null;if(ʽ.ContainsKey("AimNormal")){var Ĝ=ʽ["AimNormal"].Split(';');Ť=new Vector3D(double.Parse(Ĝ[0]),double.Parse(Ĝ[1])
,double.Parse(Ĝ[2]));}if(ʽ.ContainsKey("UpNormal")){var Ĝ=ʽ["UpNormal"].Split(';');var Ã=Vector3D.Normalize(new Vector3D(
double.Parse(Ĝ[0]),double.Parse(Ĝ[1]),double.Parse(Ĝ[2])));D.Ʒ=()=>Ã;}if(ʽ.ContainsKey("Name"))ˁ.đ=ʽ["Name"];if(ʽ.ContainsKey(
"FlyThrough"))D.ï=true;if(ʽ.ContainsKey("SpeedLimit"))D.ð=float.Parse(ʽ["SpeedLimit"]);if(ʽ.ContainsKey("TriggerDistance"))ʉ=float.
Parse(ʽ["TriggerDistance"]);if(ʽ.ContainsKey("PosDirectionOverride")&&(ʽ["PosDirectionOverride"]=="Forward")){if(Ť.HasValue){
ʻ=ë=>ʾ+Ť.Value*((Ǒ.P.GetPosition()-ʾ).Length()+5);}else ʻ=ë=>Ǒ.Ő(50000);}if(ˀ.Length>6){D.ƶ=(ƭ,ʲ,Š,Ô,U)=>{if(ʲ<ʉ){ʩ();Ё.ѿ
.π(ˀ[7],ˀ.Skip(6).ToArray());}};}if(ʽ.ContainsKey("Ng")){Func<MatrixD>ʕ=()=>Ǒ.P.WorldMatrix;if(ʽ["Ng"]=="Down")D.Ʊ=()=>
MatrixD.CreateFromDir(Ǒ.P.WorldMatrix.Down,Ǒ.P.WorldMatrix.Forward);if(ʿ.ǌ()&&!ʽ.ContainsKey("IgG")){D.Ƴ=ë=>ʿ.Ǎ.Value;D.ƹ=ë=>
Vector3D.Normalize(ʻ(ë)-ʿ.Ǎ.Value)*(ʾ-ʿ.Ǎ.Value).Length()+ʿ.Ǎ.Value;}else{if(Ť.HasValue){D.Ƴ=ë=>Ǒ.P.GetPosition()+Ť.Value*1000;}
else D.Ƴ=ë=>Ǒ.P.GetPosition()+(D.Ʊ??ʕ)().Forward*1000;}}if(ʽ.ContainsKey("TransformChannel")){Func<Vector3D,Vector3D>ʪ=ë=>
Vector3D.Transform(ę,Ŏ(ʽ["TransformChannel"]).č.Value);D.Ƹ=ʪ;ˁ.ɹ=()=>Vector3D.Transform(ę,Ŏ(ʽ["TransformChannel"]).č.Value);D.Á=
true;D.í=()=>Ŏ(ʽ["TransformChannel"]);D.Ƶ=()=>Ǒ.Û;D.ƹ=null;}else D.ƹ=ʻ;ˁ.ʢ=ApckState.CwpTask;ʿ.ǋ(ˁ.ʢ,D);ʿ.ǘ(ˁ);}void ʩ(){if(
ˁ!=null){if(Ɩ.Ǚ()==ˁ)Ɩ.Ǘ();ˁ=null;}}ʨ ʫ;class ʨ{List<IMyShipConnector>ʦ;Dictionary<IMyShipConnector,Vector3D>ʥ=new
Dictionary<IMyShipConnector,Vector3D>();public ʨ(List<IMyShipConnector>ʤ,PersistentState š,IMyGridTerminalSystem Ũ){ʦ=ʤ;ʦ.ForEach(
é=>ʥ.Add(é,é.GetPosition()));}public void ʣ(List<IMyTerminalBlock>â){foreach(var é in â){var ʧ=é.CustomName.Split('/')[1]
;var ț=ʧ.ToCharArray();var ʭ=new Ȟ();ʭ.ț=ț;ʭ.Ț=é.Position;ȟ.Add(ʭ);}}void ʱ(Ȟ Ť){var á=string.Concat(Ť.ț);У.Ƈ("checkin "+
á);foreach(var ʰ in ȟ.Where(é=>é.ȝ==null&&é.Ȝ.Count==0&&(é!=Ť)&&é.ț.Any(Ŗ=>á.Contains(Ŗ)))){Ť.Ȝ.Add(ʰ);ʰ.ȝ=Ť;ʱ(ʰ);}}
public void ǈ(IMyIntergridCommunicationSystem ĺ,int Ŗ){У.Ƌ("Navmesh: "+ȟ.Count);var ŗ=new List<MyTuple<Vector3D,Vector3D,
Vector4>>();foreach(var Ť in ȟ.Where(é=>é.ȝ!=null)){var ų=ʦ.First().CubeGrid;ŗ.Add(new MyTuple<Vector3D,Vector3D,Vector4>(ų.
GridIntegerToWorld(Ť.Ț),ų.GridIntegerToWorld(Ť.ȝ.Ț),Color.SeaGreen.ToVector4()));}ĺ.SendUnicastMessage(DbgIgc,"draw-lines",ŗ.
ToImmutableArray());foreach(var ƃ in Ȃ)У.Ƌ(ƃ+" awaits docking");foreach(var ƃ in ȁ)У.Ƌ(ƃ+" awaits dep");if(Ȃ.Any()){var ʯ=ʦ.
FirstOrDefault(ƭ=>(string.IsNullOrEmpty(ƭ.CustomData)||(ƭ.CustomData==Ȃ.Peek().ToString()))&&(ƭ.Status==MyShipConnectorStatus.
Unconnected));if(ʯ!=null){var Ȇ=Ȃ.Dequeue();ʯ.CustomData=Ȇ.ToString();var ɨ=MatrixD.Transpose(ʯ.WorldMatrix);var Ȋ=Ȁ(ȃ[Ȇ]).Reverse(
).Select(é=>Vector3D.TransformNormal(é-ʯ.GetPosition(),ɨ)).ToImmutableArray();У.Ƈ($"Sent {Ȋ.Length}-node approach path");
ĺ.SendUnicastMessage(Ȇ,"apck.docking.approach",Ȋ);}}if(ȁ.Any()){foreach(var ƃ in ȁ){У.Ƌ(ƃ+" awaits departure");}var Â=ȁ.
Peek();var ʮ=ʦ.FirstOrDefault(ƭ=>ƭ.CustomData==Â.ToString());if(ʮ!=null){ȁ.Dequeue();var ɨ=MatrixD.Transpose(ʮ.WorldMatrix);
var Ȋ=Ȁ(ȃ[Â]).Select(é=>Vector3D.TransformNormal(é-ʮ.GetPosition(),ɨ)).ToImmutableArray();У.Ƈ(
$"Sent {Ȋ.Length}-node departure path");ĺ.SendUnicastMessage(Â,"apck.depart.approach",Ȋ);}}foreach(var ƭ in ʦ.Where(ƭ=>!string.IsNullOrEmpty(ƭ.CustomData))){
long Ȇ;if(long.TryParse(ƭ.CustomData,out Ȇ)){У.Ƌ($"Channeling DV to {Ȇ}");var é=new Ĕ(1,"docking");var ȉ=ƭ.WorldMatrix;é.ĳ(ȉ
.Translation+ȉ.Forward*(ƭ.CubeGrid.GridSizeEnum==MyCubeSize.Large?1.25:0.5),Ŗ);if(ʥ[ƭ]!=Vector3D.Zero)é.Û=(ƭ.GetPosition(
)-ʥ[ƭ])/Dt;ʥ[ƭ]=ƭ.GetPosition();é.č=ȉ;var Ȉ=é.ʑ();ĺ.SendUnicastMessage(Ȇ,"apck.ntv.update",Ȉ);}}}public void ȇ(string Ȇ){
ʦ.First(é=>é.CustomData==Ȇ).CustomData="";}public void ȅ(long Ȇ,Vector3D ƭ,bool Ȅ=false){if(Ȅ){if(!ȁ.Contains(Ȇ))ȁ.
Enqueue(Ȇ);}else{if(!Ȃ.Contains(Ȇ))Ȃ.Enqueue(Ȇ);}ȃ[Ȇ]=ƭ;}Dictionary<long,Vector3D>ȃ=new Dictionary<long,Vector3D>();Queue<long>
Ȃ=new Queue<long>();Queue<long>ȁ=new Queue<long>();public IEnumerable<Vector3D>Ȁ(Vector3D ǿ){var ų=ʦ.First().CubeGrid;var
Ť=ȟ.Where(é=>é.Ȝ.Count==0).OrderBy(é=>(ų.GridIntegerToWorld(é.Ț)-ǿ).LengthSquared()).FirstOrDefault();if(Ť!=null){var á=Ť
;do{yield return ų.GridIntegerToWorld(á.Ț);á=á.ȝ;}while(á!=null);}}public Vector3D Ǿ(){return ʦ.First().WorldMatrix.
Forward;}public List<IMyShipConnector>ȋ(){return ʦ;}public List<Ȟ>ȟ=new List<Ȟ>();public class Ȟ{public Ȟ ȝ;public List<Ȟ>Ȝ=new
List<Ȟ>();public char[]ț;public Vector3I Ț;}}Ș ș;class Ș{Vector2 ȗ;List<ɥ>Ȗ=new List<ɥ>();ˏ ȕ;StateWrapper Ȕ;Vector2 ȓ;
public Ș(IMyTextSurface ë,ˏ Ȓ,StateWrapper ȑ){ȕ=Ȓ;Ȕ=ȑ;ȓ=ë.TextureSize;float Ȑ=0.18f;Vector2 Ǫ=new Vector2(85,40);float ȏ=-
0.967f;var Ȏ=ǫ(ë,Ǫ,new Vector2(ȏ+Ȑ,0.85f),"Recall",Color.Black);Ȏ.ɜ=Ǜ=>{Ȓ.Η();};Ǟ(Ȏ,
"Finish work (broadcast command:force-finish)");Ȗ.Add(Ȏ);var ȍ=ǫ(ë,Ǫ,new Vector2(ȏ+Ȑ*2,0.85f),"Resume",Color.Black);ȍ.ɜ=Ǜ=>{Ȓ.ί();};Ǟ(ȍ,
"Resume work (broadcast 'miners.resume' message)");Ȗ.Add(ȍ);var Ȍ=ǫ(ë,Ǫ,new Vector2(ȏ+Ȑ*3,0.85f),"Clear state",Color.Black);Ȍ.ɜ=Ǜ=>{ȑ?.ClearPersistentState();};Ǟ(Ȍ,
"Clear Dispatcher state");Ȗ.Add(Ȍ);var ǯ=ǫ(ë,Ǫ,new Vector2(ȏ+Ȑ*4,0.85f),"Clear log",Color.Black);ǯ.ɜ=Ǜ=>{У.Ɔ();};Ȗ.Add(ǯ);var ǭ=ǫ(ë,Ǫ,new
Vector2(ȏ+Ȑ*5,0.85f),"Purge locks",Color.Black);ǭ.ɜ=Ǜ=>{Ȓ.Ζ();};Ǟ(ǭ,"Clear lock ownership. Last resort in case of deadlock");Ȗ.
Add(ǭ);var Ǭ=ǫ(ë,Ǫ,new Vector2(ȏ+Ȑ*6,0.85f),"EMRG HALT",Color.Black);Ǭ.ɜ=Ǜ=>{Ȓ.ή();};Ǟ(Ǭ,
"Halt all activity, restore overrides, release control, clear states");Ȗ.Add(Ǭ);Ɏ=new MySprite(SpriteType.TEXT,"",new Vector2(ȓ.X/1.2f,ȓ.Y*0.9f),null,Color.White,"Debug",TextAlignment.
CENTER,0.5f);Ɍ=new MySprite(SpriteType.TEXT,"",Ȏ.ɤ-Vector2.UnitY*17,null,Color.White,"Debug",TextAlignment.LEFT,0.5f);ɋ=new
MySprite(SpriteType.TEXT,"No active task",new Vector2(ȓ.X/1.2f,ȓ.Y/20f),null,Color.White,"Debug",TextAlignment.CENTER,0.5f);}ɥ ǫ
(IMyTextSurface ë,Vector2 Ǫ,Vector2 ǩ,string Ǩ,Color?ǧ=null){var Ǯ=ë.TextureSize;var Ǧ=new MySprite(SpriteType.TEXTURE,
"SquareSimple",new Vector2(0,0),Ǫ,Color.CornflowerBlue);var Ǥ=Vector2.Zero;if(Ǫ.Y>1)Ǥ.Y=-ë.MeasureStringInPixels(new StringBuilder(Ǩ),
"Debug",0.5f).Y/Ǫ.Y;var ǣ=new MySprite(SpriteType.TEXT,Ǩ,Ǥ,Vector2.One,Color.White,"Debug",TextAlignment.CENTER,0.5f);var Ǣ=new
List<MySprite>(){Ǧ,ǣ};var ǡ=new ɥ(Ǣ,Ǫ,ǩ,Ǯ);if(ǧ!=null){ǡ.ɟ=()=>{ǡ.ȯ(ǟ=>{var Ǡ=ǟ;Ǡ.Color=ǧ;Ǡ.Size=Ǫ*1.05f;return ǟ.Type==
SpriteType.TEXTURE?Ǡ:ǟ;});};ǡ.ɞ=()=>{ǡ.ȯ(ǟ=>ǟ.Type==SpriteType.TEXTURE?Ǧ:ǟ);};}return ǡ;}void Ǟ(ɥ ǝ,string ǜ){ǝ.ɟ+=()=>Ɍ.Data=ǜ;ǝ.
ɞ+=()=>Ɍ.Data="";}bool ǥ;public void ǈ(IMyTextPanel Ƕ,IMyShipController ǽ){bool Ǽ=true;Vector2 Â=Vector2.Zero;bool ǻ=
false;if(ǽ.IsUnderControl&&κ.ƅ.φ("cc")){Â=ǽ.RotationIndicator;var Ǻ=ǽ.RollIndicator;var ǹ=(Ǻ>0);if(!ǹ&&ǥ)ǻ=true;ǥ=ǹ;}if(Â.
LengthSquared()>0||ǻ){Ǽ=true;ȗ.X+=Â.Y;ȗ.Y+=Â.X;ȗ=Vector2.Clamp(ȗ,-Ƕ.TextureSize/2,Ƕ.TextureSize/2);}var Ǹ=ȗ+Ƕ.TextureSize/2;if(Ǽ){
using(var Ǵ=Ƕ.DrawFrame()){ɧ(Ǵ);foreach(var ǝ in Ȗ.Where(é=>é.ɚ).Union(ɏ)){if(ǝ.ȡ(Ǹ)){if(ǻ)ǝ.ɜ?.Invoke(Ǹ);}}foreach(var ǝ in
Ȗ.Where(é=>é.ɚ).Union(ɏ)){Ǵ.AddRange(ǝ.Ȯ());}Ǵ.Add(Ɏ);Ǵ.Add(Ɍ);Ǵ.Add(ɋ);ǵ(Ǵ);var Ƿ=new MySprite(SpriteType.TEXTURE,
"Triangle",Ǹ,new Vector2(7f,10f),Color.White);Ƿ.RotationOrScale=6f;Ǵ.Add(Ƿ);}if(Â.LengthSquared()>0){Ƕ.ContentType=ContentType.
TEXT_AND_IMAGE;Ƕ.ContentType=ContentType.SCRIPT;}}}void ǵ(MySpriteDrawFrame Ǵ){var ŗ=new Vector2(ȓ.X/1.2f,ȓ.Y/2f);foreach(var ǳ in ȕ.ͻ
){var ę=ǳ.Χ.Ȫ.Translation;var Ǉ=ȕ.Ε;if(Ǉ!=null){var ǲ=Vector3D.Transform(ę,Ɋ);float Ǳ=3.5f;var ǰ=ŗ+new Vector2((float)ǲ.Y
,(float)ǲ.X)*Ǳ;var Ǫ=Vector2.One*Ǳ*Ǉ.Γ*2;var Ǧ=new MySprite(SpriteType.TEXTURE,"AH_BoreSight",ǰ+new Vector2(0,5),Ǫ*0.8f,ǳ
.Χ.ȩ);Ǧ.RotationOrScale=(float)Math.PI/2f;var Ƞ=new MySprite(SpriteType.TEXTURE,
"Textures\\FactionLogo\\Miners\\MinerIcon_3.dds",ǰ,Ǫ*1.2f,Color.Black);Ǵ.Add(Ƞ);Ǵ.Add(Ǧ);}}}void ɧ(MySpriteDrawFrame Ǵ){bool ɖ=false;int ɕ=0,ɔ=30;foreach(var ɓ in ȕ.ͻ){
if(!ɓ.Χ.ȧ.IsDefault){int ɒ=0,ɑ=100,Ȑ=75;if(!ɖ){foreach(var ɐ in ɓ.Χ.ȧ){Ǵ.Add(new MySprite(SpriteType.TEXTURE,
"SquareSimple",new Vector2(ɑ+ɒ,ɔ),new Vector2(Ȑ-5,40),Color.Black));Ǵ.Add(new MySprite(SpriteType.TEXT,ɐ.Item1,new Vector2(ɑ+ɒ,ɔ-16),
null,Color.White,"Debug",TextAlignment.CENTER,0.5f));ɒ+=Ȑ;}ɖ=true;ɕ+=40;}ɒ=0;foreach(var ɐ in ɓ.Χ.ȧ){Ǵ.Add(new MySprite(
SpriteType.TEXT,ɐ.Item2,new Vector2(ɑ+ɒ,ɔ+ɕ),null,ɓ.Χ.ȩ,"Debug",TextAlignment.CENTER,0.5f));ɒ+=Ȑ;}ɕ+=40;}}}List<ɥ>ɏ=new List<ɥ>();
public Action<int>ɗ;MySprite Ɏ;MySprite Ɍ;MySprite ɋ;MatrixD Ɋ;internal void ɉ(ˏ.Δ Ɉ){Ɋ=MatrixD.Invert(MatrixD.CreateWorld(Ɉ.Α
,Ɉ.Β,Ɉ.Θ));ɏ=new List<ɥ>();Vector2 ɇ=new Vector2(ȓ.X/1.2f,ȓ.Y/2f);float Ǳ=3.5f;Vector2 Ǫ=Vector2.One*Ǳ*Ɉ.Γ*1.6f;foreach(
var Ŗ in Ɉ.Ό){var ę=ɇ+Ŗ.Λ*Ǳ;Color ɍ=Color.White;if(Ŗ.ß==ShaftState.Planned)ɍ=Color.CornflowerBlue;else if(Ŗ.ß==ShaftState.
Complete)ɍ=Color.Darken(Color.CornflowerBlue,0.4f);else if(Ŗ.ß==ShaftState.InProgress)ɍ=Color.Lighten(Color.CornflowerBlue,0.2f)
;else if(Ŗ.ß==ShaftState.Cancelled)ɍ=Color.DarkSlateGray;var Ǧ=new MySprite(SpriteType.TEXTURE,"Circle",new Vector2(0,0),
Ǫ,ɍ);var Ǣ=new List<MySprite>(){Ǧ};var ǡ=new ɥ(Ǣ,Ǫ,ę,ȓ);var ǧ=Color.Red;ǡ.ɝ=ë=>Ɏ.Data=$"id: {Ŗ.Ȩ}, {Ŗ.ß}";ǡ.ɟ=()=>{ǡ.ȯ(ǟ
=>{var Ǡ=ǟ;Ǡ.Color=ǧ;Ǡ.Size=Ǫ*1.05f;return ǟ.Type==SpriteType.TEXTURE?Ǡ:ǟ;});};ǡ.ɞ=()=>{ǡ.ȯ(ǟ=>ǟ.Type==SpriteType.TEXTURE?
Ǧ:ǟ);Ɏ.Data="Hover over shaft for more info,\n tap E to cancel it";};ǡ.ɜ=é=>ɗ?.Invoke(Ŗ.Ȩ);ɏ.Add(ǡ);}}public void ɦ(ˏ ƭ){
if(ƭ?.Ε!=null)ɋ.Data=$"Kind: HexSpiral\nShafts: {ƭ.Ε.Ό.Count}\nRadius: {ƭ.Ε.Γ:f2}\n"+$"Group: {ƭ.Ε.Ύ}";}class ɥ{public
Vector2 ɤ,ɣ,ɢ;public List<MySprite>ɡ;public Vector2 ɠ;public Action ɟ{get;set;}public Action ɞ{get;set;}public Action<Vector2>ɝ
{get;set;}public Action<Vector2>ɜ{get;set;}public bool ɛ{get;set;}public bool ɚ=true;Vector2 ə;public ɥ(List<MySprite>Ǣ,
Vector2 ɘ,Vector2 ǩ,Vector2 ȳ){ɡ=Ǣ;if(Math.Abs(ǩ.X)>1)ǩ.X=ǩ.X/(ȳ.X*0.5f)-1;if(Math.Abs(ǩ.Y)>1)ǩ.Y=1-ǩ.Y/(ȳ.Y*0.5f);ə=ȳ;ɠ=new
Vector2(ɘ.X>1?ɘ.X:ɘ.X*ə.X,ɘ.Y>1?ɘ.Y:ɘ.Y*ə.Y);ɢ=ȳ/2f*(Vector2.One+ǩ);ɤ=ɢ-ɠ/2f;ɣ=ɢ+ɠ/2f;}public bool ȡ(Vector2 ȱ){bool Ȱ=(ȱ.X>ɤ.X
)&&(ȱ.X<ɣ.X)&&(ȱ.Y>ɤ.Y)&&(ȱ.Y<ɣ.Y);if(Ȱ){if(!ɛ){ɟ?.Invoke();}ɛ=true;ɝ?.Invoke(ȱ);}else{if(ɛ){ɞ?.Invoke();}ɛ=false;}return
Ȱ;}public void ȯ(Func<MySprite,MySprite>ţ){for(int Ť=0;Ť<ɡ.Count;Ť++){ɡ[Ť]=ţ(ɡ[Ť]);}}public IEnumerable<MySprite>Ȯ(){
foreach(var é in ɡ){var ȭ=ɠ;var Ȭ=é;Ȭ.Position=ɢ+ɠ/2f*é.Position;var ȫ=é.Size.Value;Ȭ.Size=new Vector2(ȫ.X>1?ȫ.X:ȫ.X*ȭ.X,ȫ.Y>1?
ȫ.Y:ȫ.Y*ȭ.Y);yield return Ȭ;}}}}class Ȳ{public MatrixD Ȫ;public Color ȩ;public long Ȩ;public ImmutableArray<MyTuple<
string,string>>ȧ;public void Ȧ(MyTuple<long,MatrixD,Vector4,ImmutableArray<MyTuple<string,string>>>Ğ){Ȩ=Ğ.Item1;Ȫ=Ğ.Item2;ȩ=Ğ.
Item3;ȧ=Ğ.Item4;}public MyTuple<long,MatrixD,Vector4,ImmutableArray<MyTuple<string,string>>>ȥ(){var Ğ=new MyTuple<long,
MatrixD,Vector4,ImmutableArray<MyTuple<string,string>>>();Ğ.Item1=Ȩ;Ğ.Item2=Ȫ;Ğ.Item3=ȩ.ToVector4();Ğ.Item4=ȧ;return Ğ;}}class
Ȥ{static int[]ȣ={1,1,0,-1,-1,0};static int Ȣ(int ĺ)=>ȣ[(ĺ-1)%6];public static int ȴ(int Ť)=>1+3*Ť*(Ť-1);public static int
Ɇ(int ĺ)=>(int)(Math.Ceiling((3+Math.Sqrt(12*ĺ-3))/6));static Vector2 Ʌ(int Ʉ){if(Ʉ==1){return Vector2.Zero;}int Ƀ=Ɇ(Ʉ)-1
;int ɂ=Ʉ-ȴ(Ƀ);int Ɂ;var ɀ=Math.DivRem(ɂ-1,Ƀ,out Ɂ);Ɂ+=1;var é=Ƀ*Ȣ(ɀ+1)+Ɂ*Ȣ(ɀ+3);var ȿ=Ƀ*Ȣ(ɀ+2)+Ɂ*Ȣ(ɀ+4);return new
Vector2(é,ȿ);}public static Vector2 Ⱦ(int ĺ){var Ƚ=Ʌ(ĺ);Ƚ.Y/=2;Ƚ.X-=Ƚ.Y;Ƚ.Y*=(float)Math.Sqrt(3);return(float)Math.Sqrt(3)*Ƚ;}}
List<MyTuple<string,Vector3D,ImmutableArray<string>>>ȼ=new List<MyTuple<string,Vector3D,ImmutableArray<string>>>();void Ȼ(
string ŵ,Vector3D ë,params string[]ƃ){ȼ.Add(new MyTuple<string,Vector3D,ImmutableArray<string>>(ŵ,ë,ƃ.ToImmutableArray()));}
void Ⱥ(long ȹ){IGC.SendUnicastMessage(ȹ,"hud.apck.proj",ȼ.ToImmutableArray());ȼ.Clear();}static ȷ ȸ;struct ȷ{public int ȶ;
public int ƌ;public int ȵ;}