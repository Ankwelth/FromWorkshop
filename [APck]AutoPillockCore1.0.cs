string Ver = "1.1.310";

// (c)AutoPillock by cheerkin
// WS link: https://steamcommunity.com/sharedfiles/filedetails/?id=3158053231

static bool DesignMode;

static Vector3D GRID_ANGULAR_ACCELERATIONS = new Vector3D(0.5f, 0.5f, 1.02f);

static long? DIAG_IGC;
const float G = 9.81f;

const string GGEN_GR_TAG = "";

static float PMW_FF_REACTION_R = 5000;
class ӳ{public float Ӵ=104.38f;public float ӵ=100;public float Ӷ=1;
	Dictionary<string, Ӳ> v = new Dictionary<string, Ӳ> {
		{ "wb-range-override", ӷ<float>.ʕ(0) },
		{ "wb-precision-override", ӷ<float>.ʕ(0) },
		{ "hold-thrust-on-rotation", new ӷ<bool> { ӣ = false, Ӹ = (val, s) => s == "true" } },
		{ "torpedo-fuse-offset", ӷ<float>.ʕ(-0.5f) },
		{ "roll-power-factor", ӷ<float>.ʕ(0.2f) },
		{ "sp-limit", ӷ<float>.ʕ(104.38f) },
		{ "cc-gain", ӷ<float>.ʕ(100) },
		{ "dpath-speed-limit", ӷ<float>.ʕ(30) },
		{ "capital-behavior", new ӷ<bool> { ӣ = false, Ӹ = (value, s) => s == "true" } },
		{ "ripple-increment-interval", ӷ<int>.ʕ(20) },
		{ "ripple-increment-interval-rail", ӷ<int>.ʕ(180) },
		{ "filtering-size", ӷ<float>.ʕ(6) },
		{ "awareness-range", ӷ<float>.ʕ(3000) },
		{ "tg-autolink-range", ӷ<float>.ʕ(500) },
		{ "wb-model-cycle-timeout", ӷ<int>.ʕ(60) },
		{ "diag-igc", new ӷ<long?> { ӣ = null, Ӹ = (val, s) => { var r = long.Parse(s); DIAG_IGC = r; return r; } } },
		{ "tv-mult",  ӷ<float>.ʕ(1) },
		{ "custom-val", ӷ<float>.ʕ(0) } // for user set in conditionals
	};
	public void ӯ<M>(string ȱ,Action<M>Ӱ){((ӷ<M>)v[ȱ]).ӻ=Ӱ;}public void Ӣ(string ȱ,string ӣ){v[ȱ].Ӣ(ӣ);}public void Ӣ<M>(
string ȱ,M ӣ){((ӷ<M>)v[ȱ]).ӣ=ӣ;}public M ӱ<M>(string ȱ){return((ӷ<M>)v[ȱ]).ӣ;}public interface Ӳ{void Ӣ(string ȳ);}public
class ӷ<M>:Ӳ{public M ӣ;public Func<M,string,M>Ӹ;public void Ӣ(string ȳ){var ɪ=ӣ;ӣ=Ӹ(ӣ,ȳ);if(!ӣ.Equals(ɪ))ӻ?.Invoke(ӣ);}
public static ӷ<M>ʕ(M Ȳ,Func<M,string,M>C=null){var ȳ=new ӷ<M>{ӣ=Ȳ,Ӹ=C};if(C==null){if(typeof(M)==typeof(float))(ȳ as ӷ<float>
).Ӹ=Ӻ;if(typeof(M)==typeof(int))(ȳ as ӷ<int>).Ӹ=ӹ;}return ȳ;}public Action<M>ӻ;}static int ӹ(int ɪ,string Ç){return Ç.
StartsWith("$")?ɪ+int.Parse(Ç.TrimStart('$')):int.Parse(Ç);}static float Ӻ(float ɪ,string Ç){return Ç.StartsWith("$")?ɪ+float.
Parse(Ç.TrimStart('$')):float.Parse(Ç);}}
public enum TS { Closest, Random, Loop }
public enum RK { Ignore, FreeFire, Attack }
public enum RS { None, FreeFiring, Attacking }
public enum PS { Disabled = 0, Inert, Aim, WP }
public enum TD { None = 0, Vtol, Rover, Other }
class Ӯ{Dictionary<string,bool>ӭ=new Dictionary<string,bool>{{"suppress-transition-control",false},{
"wingman-circle-rotation",false},{"damp-when-idle",true},{"ignore-user-thruster",false},{"coax-ripple",true},{"suppress-gyro-control",false},{
"aim-to-center",false},{"avoid-carrier",true},{"freefall-target-filter",false},{"wb-snipe-range",false},{"wb-jab",false},{
"patrol-after-response",false},{"allow-1t-up-shifter",false},{"log",false},{"echo",false},{"thr-opt",false}};public void Ӣ(string ȱ,bool ӣ){ӭ[ȱ
]=ӣ;}public void Ӥ(string ȱ){ӭ[ȱ]=!ӭ[ȱ];}public bool ӥ(string ȱ){return ӭ[ȱ];}public ImmutableArray<MyTuple<string,string
>>Ӧ(string ӧ){return ӭ.Select(Ĥ=>new MyTuple<string,string>($"{Ĥ.Key}: {(Ĥ.Value?"on":"off")}",$"[toggle:{Ĥ.Key}],[{ӧ}]")
).ToImmutableArray();}public void Ө(string ȱ,خ J){var Ą=J.M.ӥ(ȱ);switch(ȱ){case"suppress-transition-control":if(!Ą)J.ƺ.Ͼ(
);break;case"log":J.ռ=Ą?new ө(ʅ.ʉ):null;break;case"echo":J.ց=Ą?new ө(ʅ.ʈ):null;break;}}}class ө{StringBuilder ӫ=new
StringBuilder();Action<string>Ӫ;public ө(Action<string>ӡ){Ӫ=ӡ;}public void ә(string Ì){ӫ.AppendLine(Ì);}public void Ӛ(){var Ƃ=ӫ.
Length;if(Ƃ>0){Ӫ(ӫ.ToString(0,Ƃ-1));ӫ.Clear();}}}const string ӛ="apck.dpath.complete";const string Ӝ="apck.command";const
string ӝ="tgp.local.gridsense.update";const string Ӟ="apck.docking.update";const string ӟ="tgp.global.gridsense.offer";Program
(){Runtime.UpdateFrequency=UpdateFrequency.Update100;ý=GridTerminalSystem;ą=new Ć(IGC,new Dictionary<string,Action<string
[],خ>>{{"set-output",(ǌ,J)=>{IMyTextSurface Ơ;var Ӡ=J.ر.Ʀ.FirstOrDefault(Ç=>Ç.CustomName.Contains(ǌ[2])&&(Ç is
IMyTextSurfaceProvider))as IMyTextSurfaceProvider;Ơ=Ӡ?.GetSurface(int.Parse(ǌ[3]));if(Ơ==null)Ơ=J.צ<IMyTextPanel>(Ç=>Ç.IsSameConstructAs(Me)&&
Ç.CustomName.Contains(ǌ[2])).FirstOrDefault();if(Ơ!=null){ʅ.ҹ=(Ì)=>Ơ.WriteText(Ì+"\n",true);ʅ.ʋ=()=>Ơ.WriteText("");Ơ.
ContentType=ContentType.TEXT_AND_IMAGE;}}},{"next",(ǌ,J)=>J.ƃ.Ҋ()},{"create-task",(ǌ,J)=>Ռ(ǌ,J)},{"inject-task",(ǌ,J)=>Ռ(ǌ,J,true)}
,{"inject-task-inherit-target",(ǌ,J)=>Ռ(ǌ,J,true,true)},{"remove-task",(ǌ,J)=>J.ƃ.ҡ(int.Parse(ǌ[2]))},{"clear-tasks",(ǌ,J
)=>J.ƃ.Ҋ(true)},{"infer-task",(ǌ,J)=>Պ(ǌ)},{"default-task",(ǌ,J)=>{ӈ.ƃ.ԑ(()=>փ(ǌ,ӈ,null));ʡ.ʥ=string.Join(":",ǌ.Skip(2));
}},{"remove-default-task",(ǌ,J)=>{ӈ.ƃ.ԑ(null);ʡ.ʥ=null;}},{"signal",(ǌ,J)=>J.ƃ.Ҟ()},{"repeat",(ǌ,J)=>{var Ä=new List<
string>(Ս);Ս.Clear();Ց(Ä,true);}},{"save-queue",(ǌ,J)=>ʡ.ʦ=Ս},{"exec-queue",(ǌ,J)=>Ց(ʡ.ʦ,true)},{"jab",(ǌ,J)=>J.ƃ.Ѳ(new ځ(ӈ,
null,null))},{"jab2",(ǌ,J)=>J.ƃ.Ѳ(new ځ(ӈ,null,ӈ.س.FirstOrDefault()))},{"thrust-delegation",(ǌ,J)=>{J.ח?.TryRun(
$"command:set-mode:{ǌ[2]}");TD Ӭ;J.ו.ĩ=Enum.TryParse(ǌ[2],out Ӭ)?Ӭ:TD.Other;}},{"pc-state",(ǌ,J)=>ɀ<PS>(ǌ[2],Ç=>J.ו.Ĭ(Ç))},{"recycle",(ǌ,J)=>ć()},
{"set-value",(ǌ,J)=>J.ذ.Ӣ(ǌ[2],ǌ[3])},{"set-tag",(ǌ,J)=>J.ō=ǌ[2]},{"clear-state",(ǌ,J)=>{ʡ=new ʢ(Ì=>Storage=Ì);Save();}},
{"clear-defs",(ǌ,J)=>Ĉ()},{"clear-navs",(ǌ,J)=>œ?.ŀ()},{"request-docking",(ǌ,J)=>J.ƃ.Ң()},{"request-depart",(ǌ,J)=>{var Ȫ
=J.ض.WorldMatrix;var Ú=new ٳ(Ȫ.Translation-Ȫ.Forward*100,ӈ,10,false);J.ƃ.ѵ(Ú);}},{"cancel-current-route",(ǌ,J)=>œ?.ń()},{
"start-su",(ǌ,J)=>{var X=ǌ[2];var ԧ=ǌ.Length>3?long.Parse(ǌ[3]):(long?)null;if(X=="first"){var Ա=J.Ư.FirstOrDefault(Ç=>!Ç.թ);if(Ա
!=null)J.Ʃ(Ա,ԧ);}else{foreach(var F in J.Ư.Where(Ç=>!Ç.թ&&((X=="all")||(Ç.ō==X))))J.Ʃ(F,ԧ);}}},{"refresh-su",(ǌ,J)=>J.ם()}
,{"query-target",(ǌ,J)=>{TS Ơ=0;var Բ=string.Join(":",ǌ.Skip(4));Dictionary<string,string>Ő;var C=J.ɷ;if(ȼ(3,ǌ,out Ő)){
var Գ=Ⱦ<string>(Ő,"TargetSelection");if(Գ!=null)Enum.TryParse(Գ,out Ơ);var Դ=Ⱦ<float?>(Ő,"R");var Ȥ=Ⱦ<string>(Ő,"Pos");if(Ȥ
!=null){var ȳ=Ȥ.Split(';');C=new Vector3D(double.Parse(ȳ[0]),double.Parse(ȳ[1]),double.Parse(ȳ[2]));}var Զ=Ⱦ<string>(Ő,
"LocationFilter");var Ե=Ⱦ<string>(Ő,"TypeFilter");if(Զ=="Cone"&&Դ.HasValue)J.ג.հ(ǌ[2],(byte)Ơ,Ե,J.ɷ,J.և.Forward,Դ.Value,Բ);else if(Զ==
"Sphere")J.ג.հ(ǌ[2],(byte)Ơ,Ե,C,Vector3D.Zero,Դ??J.ذ.ӱ<float>("awareness-range"),Բ);else J.ג.հ(ǌ[2],(byte)Ơ,Ե,Vector3D.Zero,
Vector3D.Zero,0,Բ);}}},{"tmc",(ǌ,J)=>J.י?.TryRun("q:"+string.Join(":",ǌ.Skip(2)))},{"timer",(ǌ,J)=>J.ז.ă(ǌ[2])},{"d-path-add",(ǌ
,J)=>{var Ä=DIAG_IGC??ɞ.ɠ;if(Ä!=0){J.ђ.K($"apck.dpath.add:{ǌ[2]}",ӈ.ض.WorldMatrix,Ä);}}},{"d-path-clear",(ǌ,J)=>{if(ɞ.ɠ!=
0)J.ђ.K(Ӝ,"command:clear-navs",ɞ.ɠ);}},{"replace-behavior-task",(ǌ,J)=>J.ƃ.ԇ(ǌ[3],ǌ[4],true,ǌ[2])},{
"replace-behavior-current",(ǌ,J)=>J.ƃ.ԇ(ǌ[2],ǌ[3],true)},{"chain-behavior-task",(ǌ,J)=>J.ƃ.ԇ(ǌ[3],ǌ[4],false,ǌ[2])},{"chain-behavior-current",(ǌ,J
)=>J.ƃ.ԇ(ǌ[2],ǌ[3],false)},{"set-response",(ǌ,J)=>J.ƃ.ы(ǌ[2])},{"set-targeting-strategy",(ǌ,J)=>ɀ<TS>(ǌ[2],Ç=>ʵ.ʭ=Ç)},{
"detonate",(ǌ,J)=>{int ѝ=0;foreach(var Ċ in J.ع){if(ѝ++<2)ʑ.ʕ(()=>Ċ.Detonate(),0).ʙ(ʌ);else Ċ.Detonate();}}},{"set-response-ovr",(
ǌ,J)=>J.ƃ.ъ=string.Join(":",ǌ.Skip(2))},{"w-mod-value",(ǌ,J)=>ɞ.Ȱ(ǌ[2],ǌ[3])},{"wb-cycle-face",(ǌ,J)=>J.Ŷ.ȁ(ǌ.Length>2?
int.Parse(ǌ[2]):-1)},{"add-condition",(ǌ,J)=>J.ƃ.Ԍ.Ѧ(ǌ)},{"cmdr-draw-pos",(ǌ,J)=>J.ђ.A(J.ɷ,ǌ[2],ǌ[3],int.Parse(ǌ[4]),ǌ[5])}
,{"cmdr-draw-targetable",(ǌ,J)=>J.ђ.A(J.ɷ,ǌ[2],ǌ[3],int.Parse(ǌ[4]),ǌ[5],true)},{"delay-cmd",(ǌ,J)=>ʑ.ʕ(()=>J.ђ.Â(string.
Join(":",ǌ.Skip(3)),J),int.Parse(ǌ[2])).ʙ(ʌ)},{"get-toggles",(ǌ,J)=>{var ӧ=string.Join(":",ǌ.Take(3));J.ђ.K(
$"menucommand.get-commands.reply:{ӧ}",J.M.Ӧ(ӧ),long.Parse(ǌ[2]));}},{"get-storage",(ǌ,J)=>{var Q=long.Parse(ǌ[2]);DIAG_IGC=Q;J.ђ.K("diag.get-storage.reply",
Storage,Q);}}});}class ԋ{خ ņ;public э Ԍ;Func<ڈ>ԍ;ڈ Ԏ;bool ԏ;public long?Ԑ;public ԋ(خ J){ņ=J;ԏ=!J.Ʊ;Ԟ();Ԍ=new э(J);}public void
ԑ(Func<ڈ>Ä){ԍ=Ä;if(Ä!=null){Ԏ=ԍ();ѵ(Ԏ);}}int Ԋ;void ӽ(ڈ Ä){Ԋ++;Ä.ژ(Ԋ);}Dictionary<string,Func<ӆ,Ӊ?,Vector3D>>Ӿ;public
class ӿ{public bool Ԁ;public List<Ӊ>ԁ=new List<Ӊ>();public void Ԃ(ӿ ԃ){Ԁ=ԃ.Ԁ;ԁ=new List<Ӊ>(ԃ.ԁ);}}Dictionary<string,ӿ>Ԅ=new
Dictionary<string,ӿ>();Dictionary<string,ӿ>ԅ=new Dictionary<string,ӿ>();Dictionary<string,ӿ>Ԇ=new Dictionary<string,ӿ>();public
void ԇ(string Ç,string Ԉ,bool ԉ,string Ӽ=null){ӿ ſ;if(Ӽ!=null){var Ԓ=(Ç=="aim")?ԅ:(Ç=="move")?Ԅ:Ԇ;if(!Ԓ.ContainsKey(Ӽ))Ԓ.Add
(Ӽ,new ӿ());ſ=Ԓ[Ӽ];}else ſ=(Ç=="aim")?Ԛ:(Ç=="move")?ԓ:ԙ;ſ.Ԁ=ԉ;ſ.ԁ.Clear();if(Ԉ!="reset")ԛ(ſ,Ԉ);}void ԛ(ӿ ſ,string ǫ){
foreach(var Y in ǫ.Split(',')){var Ԝ=Y.Split('/');var ԝ=new Ӊ{M=Ӿ[Ԝ[0]]};if(Ԝ.Length>1)ԝ.ӄ=float.Parse(Ԝ[1]);ſ.ԁ.Add(ԝ);}}void
Ԟ(){Ӿ=new Dictionary<string,Func<ӆ,Ӊ?,Vector3D>>();Ӿ.Add("AimRestrictPlane",ҿ.Ӑ);Ӿ.Add("BallisticSolver",ҿ.ӗ);Ӿ.Add(
"SwayTargetXYZ",ҿ.ң);Ӿ.Add("SwayX",ҿ.һ);Ӿ.Add("SwayY",ҿ.ҽ);Ӿ.Add("CircleFw100",ҿ.ڵ);Ӿ.Add("CircleTgPlane",ҿ.ҷ);Ӿ.Add("Empty",ҿ.ڇ);Ӿ.Add
("NegGravity",ҿ.ҭ);Ӿ.Add("NegGravityRejectTilt",ҿ.Ү);Ӿ.Add("Closer",ҿ.Ҳ);Ӿ.Add("Higher",ҿ.Ҭ);Ӿ.Add("FwRel",ҿ.Ҥ);Ӿ.Add(
"CqbShifter",ҿ.Ӄ);Ӿ.Add("OrbitPoint",ҿ.Ӏ);Ӿ.Add("AngularOffsetX",ҿ.Ҹ);Ӿ.Add("AngularOffsetY",ҿ.Ҿ);Ӿ.Add("AngularOffsetV",ҿ.ҳ);Ӿ.Add(
"AngularOffsetNg",ҿ.Ҵ);}LinkedList<ڈ>ԟ=new LinkedList<ڈ>();LinkedListNode<ڈ>Ԡ;List<MyTuple<int,string,Vector3D>>Ԧ=new List<MyTuple<int,
string,Vector3D>>(10);public void ԡ(){var Ԣ=ņ.ו;if(!ņ.Ʊ){var ԣ="        ";var Ц=new MyTuple<string,byte,int,long>(ņ.ō,0,ņ.Ʋ,ņ.
î.CubeGrid.EntityId);var Ԥ=new MyTuple<MyTuple<string,byte,int,long>,Vector3D,Vector3D,Vector3D,string>(Ц,ņ.ɷ,Ԣ.Ĵ,Ԣ.η,
$"{ԣ}V': {Ԣ.α.Length():f1}\n{ԣ}{ѳ?.ǜ}({ԟ.Count})");ņ.ב.SendBroadcastMessage("apck.report",Ԥ);}if(ԏ){Ԧ.Clear();if(ԟ.Count>0){Ԧ.Add(new MyTuple<int,string,Vector3D>(-1,
"agent",ņ.ɷ));ņ.ց?.ә("Task queue:");var ԥ=ņ.ɷ;foreach(var Ä in ԟ){ņ.ց?.ә($"{Ä.Ɲ}-{Ä.ʀ}");if(Ä.ɷ.HasValue)ԥ=Ä.ɷ.Value;Ԧ.Add(new
MyTuple<int,string,Vector3D>(Ä.ʀ,Ä.Ɲ,Ä.ɷ??ԥ));}ņ.ב.SendBroadcastMessage("captain-commander.task-data",Ԧ.ToImmutableArray());}}ғ
();if(щ!=RS.None)Ԍ.Ӆ(ѕ,щ==RS.FreeFiring?"FreeFiring":"Attacking");if(Ԡ!=null){var Ä=Ԡ.Value;Ԍ.Ӆ(Ä.ڒ.ه??Ä.ڒ.و,Ä.Ɲ);if(Ä.ڍ(
ʅ.ƅ,ņ.ו)){ņ.ռ?.ә($"TFin {Ä.Ɲ}-{Ä.ʀ}");Ҋ();}}Ԙ(ҝ());}ӿ Ԛ=new ӿ();ӿ ԓ=new ӿ();ӿ ԙ=new ӿ();Vector3D Ԕ(ӆ Ô,Ӊ?Ċ,ӿ ſ){foreach(
var ƣ in ſ.ԁ)Ô.ʂ=ƣ.M(Ô,ƣ);return Ô.ʂ;}Vector3D ԕ(ӆ Ô,Ӊ?Ċ){return Ԕ(Ô,Ċ,Ԛ);}Vector3D Ԗ(ӆ Ô,Ӊ?Ċ){return Ԕ(Ô,Ċ,ԓ);}Vector3D ԗ(
ӆ Ô,Ӊ?Ċ){return Ԕ(Ô,Ċ,ԙ);}void Ԙ(ػ Ą){var Ў=new ӆ{ӈ=ņ,ӊ=Ą};var ή=Ą.ـ?.Invoke()??ņ.ɷ;var ѡ=Ą.ه;var Ѷ=Ą.ؽ;var ѷ=Ą.ؿ;if(щ==
RS.FreeFiring){Ą.ه=ѕ;Ą.ؽ=ѓ;}if(Ą.و?.Ά.HasValue==true){Ў.ʂ=Ą.و.Ά.Value;Ў.Ӈ=Ą.و.Ĵ??Vector3D.Zero;if(ԓ.ԁ.Count>0){if(!ԓ.Ԁ&&Ą.
ؼ!=null)Ў.ʂ=Ą.ؼ(Ў,null);ņ.ו.η=Ԗ(Ў,null);}else ņ.ו.η=Ą.ؼ?.Invoke(Ў,null)??Ў.ʂ;if(щ==RS.FreeFiring)ņ.ו.η=є(Ў,null);Ą.ى=ņ.ו.
η;double Ѹ=(ņ.ו.η-ή).Length();if(Ą.م){if(ņ.Ū()&&(Ѹ>(ņ.ذ.Ӵ>200?8000:3000))){if(Ѽ(ή,Ą)){if(!ѻ.HasValue){ѻ=Ą.ك;ņ.ռ?.ә(
"PCN starts");}Ą.ك=false;}else{if(ѻ.HasValue){Ą.ك=ѻ.Value;ņ.ռ?.ә("PCN ends");}ѻ=null;}}else{if(ѻ.HasValue){Ą.ك=ѻ.Value;ņ.ռ?.ә(
"PCN ends");}ѻ=null;}}if(Ą.ن)Ą.ى=ҿ.ډ(Ą.ى.Value,ņ,Ą);}else if(Ą.ي)Ą.ى=Ą.ؼ?.Invoke(Ў,null);bool ѹ=false;var Ѻ=Vector3D.Zero;if(ņ.ו.ī
&&Ą.ى.HasValue&&ņ.ו.Ķ==PS.WP){ѹ=true;Ѻ=ҿ.څ(Ą.ى.Value,ņ,Ą);}if(ѹ&&(Ԛ.ԁ.Count==0)){Ą.ζ=Ѻ;}else{if(Ą.ه?.Ά.HasValue==true){Ў.ʂ
=Ą.ه.Ά.Value;Ў.Ӈ=Ą.ه.Ĵ??Vector3D.Zero;if(Ԛ.ԁ.Count>0){if(!Ԛ.Ԁ&&Ą.ؽ!=null)Ў.ʂ=Ą.ؽ(Ў,null);Ў.ʂ=ԕ(Ў,null);}else Ў.ʂ=Ą.ؽ?.
Invoke(Ў,null)??Ў.ʂ;Ą.ζ=Ў.ʂ;}else if(Ą.ي)Ą.ζ=Ą.ؽ?.Invoke(Ў,null);}Ў.ʂ=Ą.ى??ņ.ɷ;if(ԙ.ԁ.Count>0){if(!ԙ.Ԁ&&Ą.ؾ!=null)Ў.ʂ=ņ.ɷ+Ą.ؾ(
Ў,null);Ą.ٮ=ԗ(Ў,null)-ņ.ɷ;}else{Ą.ٮ=Ą.ؾ?.Invoke(Ў,null)??Vector3D.Zero;}Ą.غ=Ą.ؿ?.Invoke();Ą.ه=ѡ;Ą.ؽ=Ѷ;Ą.ؿ=ѷ;}bool?ѻ;bool
Ѽ(Vector3D ή,ػ Ą){var ѽ=ņ.ŵ.Value;var Ѿ=ѽ-ή;if(Vector3D.Dot(Ѿ.Normalized(),ņ.ו.ķ)>0.99){var Ç=ņ.ו.η;var ѿ=Ç-ѽ;var з=Ѿ.
Length();var Ҁ=(Ç-ή).Normalized();var ҁ=Vector3D.Dot((ѽ-Ç).Normalized(),Ҁ);ņ.ց?.ә(
$"Circumnav.alt: {з}\nCircumnav.d: {(Ç-ή).Length()}\nCircumnav.dot: {ҁ}");if(ҁ<0.6){var а=new BoundingSphereD(ѽ,з);if(а.Contains(Ç)==ContainmentType.Contains){var Ȝ=ҿ.ڋ(Ç,ņ,Ą);ņ.ց?.ә(
"PCN above");Ą.ى=Ȝ;return true;}else{а.Radius-=100;var д=new RayD(ή,Ҁ);var Ѭ=а.Intersects(д);if(Ѭ.HasValue){var м=Vector3D.Cross(-д
.Direction,ѿ);var ѭ=Vector3D.Cross(ѿ,м);var Ş=ѿ.Length();var Ѯ=з*з/Ş;var ѯ=Math.Asin(з/Ş);var Ѱ=Math.Cos(ѯ)*з;var ѱ=ѽ+Ѯ*ѿ
/Ş+Ѱ*ѭ.Normalized();var Ȝ=ҿ.ڋ(ѱ,ņ,Ą);ņ.ց?.ә($"Outer: PCN tangent");Ą.ى=Ȝ;return true;}}}}return false;}public void Ѳ(ڈ Ä)
{ņ.ռ?.ә("CreateWP "+Ä.ǜ);ѫ(Ä);}public ڈ ѳ=>Ԡ?.Value;public ͻ Ѵ=>ѳ?.ڒ.و;public void ѵ(ڈ Ä){ӽ(Ä);ԟ.AddLast(Ä);ņ.ռ?.ә(
$"Added {Ä.ǜ}, total: {ԟ.Count}");if(Ԡ==null){Ԡ=ԟ.First;қ(Ä);}else if(Ԡ.Value.ڐ&&!Ԡ.Value.ڑ.HasValue)Ҋ();}public void ѫ(ڈ Ä){ӽ(Ä);ԟ.AddFirst(Ä);ņ.ռ?.ә(
$"Added and activated {Ä.ǜ}, total: {ԟ.Count}");Ԡ=ԟ.First;қ(Ä);}public void Ҋ(bool ҙ=false){if(Ԡ==null)return;var Қ=Ԡ;var Ô=Қ.Value;Ô.ړ?.Invoke();Ԛ.ԁ.Clear();ԓ.ԁ.
Clear();ԙ.ԁ.Clear();ѻ=null;ņ.ז.ă(Ô.Ɲ+".OnComplete");if(ј)ņ.ד.ʻ();if(!ҙ&&Қ.Next!=null){if(Қ==Ԡ){Ô=Қ.Next.Value;Ԡ=Қ.Next;қ(Ô);}
ԟ.Remove(Қ);}else{ԟ.Clear();Ԡ=null;if((Ԏ!=null)&&(Ô!=Ԏ)){Ԏ=ԍ();ѵ(Ԏ);}}}void қ(ڈ Ä){Ä.ժ(ņ,ʅ.ƅ);if(ԅ.ContainsKey(Ä.Ɲ)&&ԅ[Ä.
Ɲ].ԁ.Count>0){ņ.ռ?.ә($"Bh aim override: {Ä.Ɲ}");Ԛ.Ԃ(ԅ[Ä.Ɲ]);}if(Ԅ.ContainsKey(Ä.Ɲ)&&Ԅ[Ä.Ɲ].ԁ.Count>0){ņ.ռ?.ә(
$"Bh move override: {Ä.Ɲ}");ԓ.Ԃ(Ԅ[Ä.Ɲ]);}if(Ԇ.ContainsKey(Ä.Ɲ)&&Ԇ[Ä.Ɲ].ԁ.Count>0){ņ.ռ?.ә($"Bh up norm override: {Ä.Ɲ}");ԙ.Ԃ(Ԇ[Ä.Ɲ]);}}ػ Ҝ=new ػ{ǜ=
"Standby"};public ػ ҝ(){var Ҕ=Ԡ?.Value;return Ҕ!=null?Ҕ.ڒ:Ҝ;}public void Ҟ(){if(ѳ is ڏ)Ҋ();}public void ҡ(int Q){var ҟ=ԟ.
FirstOrDefault(Ç=>Ç.ʀ==Q);if(ҟ!=null){if(ѳ==ҟ)Ҋ();else ԟ.Remove(ҟ);}}public void Ҡ(bool Ķ=true){ņ.ת.ForEach(Ą=>Ą.ChargeMode=Ķ?
ChargeMode.Recharge:ChargeMode.Auto);ņ.ק.ForEach(Ą=>Ą.Stockpile=Ķ);ņ.ו.Ĭ(Ķ?PS.Inert:PS.WP);}public void Ң(){if(ņ.ض!=null){if(ņ.Ʊ)ѵ
(new ڣ(ņ));else if(ņ.ɞ.ɠ!=0)ѵ(new ڣ(ņ,ņ.ɞ.ɠ));}}public void Ҙ(long ҋ){Җ=false;ņ.ђ.K(ӛ,"",ҋ);}bool Җ;public void Ҍ(ͻ ҍ=
null){if(ņ.ض?.Status==MyShipConnectorStatus.Connected){Ҡ(false);var Ҏ=ņ.ض.OtherConnector;var ҏ=new ڏ(ņ,60);ҏ.ړ=()=>{ņ.ռ?.ә(
"OtherConnector undick wait complete");var п=ҍ?.Ά??ņ.ض.GetPosition()-ņ.ض.WorldMatrix.Forward*100;if(Ҏ.CustomName.Contains("dock-host")){if(Җ==false){Ҏ.
CustomData=ņ.Ʊ?ņ.Ƴ:ņ.ב.Me.ToString();Җ=true;if(ņ.Ʊ)ņ.د.œ.Ğ(ņ.Ƴ,п,"",true);else ņ.ב.SendBroadcastMessage("apck.depart.request",new
MyTuple<long,Vector3D>(Ҏ.EntityId,п));ņ.ռ?.ә($"Waiting for depart dpath");ņ.ו.Ĭ(PS.Inert);ņ.ƃ.ѫ(new ڏ(ņ,()=>Җ==false));}}else{ņ
.ռ?.ә($"Undock");ņ.ض.Disconnect();}};ņ.ƃ.ѫ(ҏ);}}public void Ґ(ͻ ȶ){ѕ=ȶ;ї=ȶ.ͽ;}public void ґ(long Q,long Ý){і=ņ.Ű(ӝ,Q,Ý,
true);ѕ=і.ɰ;і.ժ();ї=Q;}void Ғ(){щ=RS.None;ѓ=null;є=null;ѕ=null;ї=null;і=null;ņ.ռ?.ә("Combat Response Ends");ņ.Ŷ.ȅ();if(ј)ņ.ד
.ʻ();}void ғ(){if(ѕ?.Й==true){if(ш==RK.FreeFire){if(щ==RS.None){ņ.ռ?.ә("Response: FreeFiring");щ=RS.FreeFiring;ѓ=ҿ.ӗ;є=ҿ.
ڵ;}}else if(ш==RK.Attack){if(щ==RS.None){щ=RS.Attacking;ņ.ռ?.ә("Response: Attacking");if(ņ.ך!=null)ņ.ב.SendUnicastMessage
(ņ.ך.EntityId,"apck-encounter",new MyTuple<long,Vector3D>(ѕ.ͽ,ѕ.Ά.Value));if(ъ!=null){foreach(var Ã in ъ.Split('|')){ņ.ђ.
Â(Ã.Replace("{id}",ѕ.ͽ.ToString()),ņ);}}else{var Ҕ=new ٸ(ņ,ѕ);if(ņ.M.ӥ("patrol-after-response")&&!ԟ.Any()){var ҕ=ņ.ɷ;Ҕ.ړ=
()=>{var җ=new ٳ(ҕ,ņ,null,true);җ.ڑ=60*40;җ.ǜ="Search";var Ȅ=new Ӊ{ӄ=500};җ.ڒ.ؼ=(Ô,Ċ)=>ҿ.Ӏ(Ô,Ȅ);ѵ(new ٳ(ҕ+ҿ.ҫ(new ӆ{ӈ=ņ})
*200,ņ,20,false));ѵ(җ);};}ѫ(Ҕ);}}}}else if(щ!=RS.None)Ғ();}Func<ӆ,Ӊ?,Vector3D>ѓ;Func<ӆ,Ӊ?,Vector3D>є;ͻ ѕ;ڦ і;long?ї;
public bool ј=>!ї.HasValue&&(ш!=RK.Ignore)&&(Ԡ==null||Ԡ.Value.ڌ);List<ɾ>љ=new List<ɾ>();public void њ(ɾ Ç){var ǘ=(Ç.ʂ-ņ.ɷ).
Length();ņ.ց?.ә($"consider tg: {ǘ:f2}");if((ш==RK.Attack&&ǘ<ņ.ذ.ӱ<float>("awareness-range"))||(ш==RK.FreeFire&&ǘ<ņ.Ŷ.Ɗ)){љ.Add
(Ç);}}Random ћ=new Random();public void ќ(int ѝ,TS Ơ){if(љ.Count>0){if(Ơ==TS.Closest)ņ.ד.ʯ(љ.OrderBy(Ç=>(ņ.ɷ-Ç.ʂ).
LengthSquared()).First(),ņ);else{int X=0;if(Ơ==TS.Random)X=ћ.Next(љ.Count);if(Ơ==TS.Loop)X=ѝ%љ.Count;ņ.ד.ʯ(љ[X],ņ);}}љ.Clear();}
public Vector3D?ў(Vector3D C){var ё=ņ.օ.FirstOrDefault(Ô=>Ô.CanScan(C));if(ё!=null){var ǘ=ё.Raycast(C);if(ǘ.HitPosition.
HasValue&&ǘ.EntityId!=ё.CubeGrid.EntityId)return ǘ.HitPosition;}return null;}public RK ш=RK.FreeFire;public RS щ=RS.None;public
string ъ;public void ы(string ь){var ɪ=ш;Enum.TryParse(ь,out ш);if(ш!=ɪ){щ=RS.None;ї=null;if(ш!=RK.Ignore)ņ.ד.ʻ();else Ғ();}}}
class э{Dictionary<string,List<ѐ>>ю=new Dictionary<string,List<ѐ>>();Random я=new Random();خ ņ;public э(خ J){ņ=J;}class ѐ{
public List<Func<ͻ,bool>>ч=new List<Func<ͻ,bool>>();public List<string>ђ=new List<string>();public int џ;public int Ѣ;public
int ʘ;public string ѣ;public bool Ѥ(ͻ ȶ)=>ч.All(Ç=>Ç(ȶ));}char[]ѥ=new char[]{'<','=','>'};public void Ѧ(string[]ѧ){int ˇ;
int Ũ=0;int X=3;var Y=ѧ[2];if(int.TryParse(ѧ[3],out ˇ)){X++;if(int.TryParse(ѧ[4],out Ũ))X++;}else ˇ=300;var Ѩ=string.Join(
":",ѧ.Skip(X+1));var ѩ=new ѐ();ѩ.ђ.AddRange(Ѩ.Split('|'));ѩ.ѣ=ѧ[X];foreach(var Ѫ in ѧ[X].Split(',','&')){var ʌ=Ѫ.IndexOfAny
(ѥ);var ȱ=Ѫ.Substring(0,ʌ);var ȍ=Ѫ.Substring(ʌ,1);var Ȳ=Ѫ.Substring(ʌ+1,Ѫ.Length-ʌ-1);Func<double,double,bool>Ѡ=(Ş,Ą)=>Ş
==Ą;if(ȍ=="<")Ѡ=(Ş,Ą)=>Ş<Ą;if(ȍ==">")Ѡ=(Ş,Ą)=>Ş>Ą;float ƣ=0;float.TryParse(Ȳ,out ƣ);if(ȱ=="targetType")ѩ.ч.Add(ȶ=>ȶ.Ύ==(
MyDetectedEntityType)Enum.Parse(typeof(MyDetectedEntityType),Ȳ));if(ȱ=="rnd")ѩ.ч.Add(ȶ=>Ѡ(я.NextDouble(),ƣ));if(ȱ=="targetSize")ѩ.ч.Add(ȶ=>Ѡ
(ȶ.Ί.Value.Extents.Length(),ƣ));if(ȱ=="distance")ѩ.ч.Add(ȶ=>ȶ.Ά.HasValue&&Ѡ((ņ.ɷ-ȶ.Ά.Value).Length(),ƣ));if(ȱ==
"rc-distance")ѩ.ч.Add(ȶ=>{var ǘ=ņ.ƃ.ў(ņ.ɷ+(ȶ.Ά.Value-ņ.ɷ).Normalized()*(ƣ+1));return ǘ.HasValue&&Ѡ((ņ.ɷ-ǘ.Value).Length(),ƣ);});if(ȱ
=="dot")ѩ.ч.Add(ȶ=>ȶ.Ά.HasValue&&Ѡ(ņ.ו.ĳ,ƣ));if(ȱ=="targetVelocity")ѩ.ч.Add(ȶ=>ȶ.Ĵ.HasValue&&Ѡ(ȶ.Ĵ.Value.Length(),ƣ));if(ȱ
=="blockEnabled"){var Ą=ņ.ז.Ă.FirstOrDefault(Ç=>Ç.CustomName.Contains(Ȳ));if(Ą!=null)ѩ.ч.Add(ȶ=>Ą.Enabled);}if(ȱ==
"taskElapsedTicks")ѩ.ч.Add(ȶ=>ņ.ƃ.ѳ!=null&&ʅ.ƅ-ņ.ƃ.ѳ.ڕ>ƣ);if(ȱ=="custom-val")ѩ.ч.Add(ȶ=>Ѡ(ņ.ذ.ӱ<float>(ȱ),ƣ));if(ȱ=="toggle")ѩ.ч.Add(ȶ=>ņ.
M.ӥ(Ȳ));if(ȱ=="alt")ѩ.ч.Add(ȶ=>ņ.ו.ί.HasValue&&Ѡ(ņ.ו.ί.Value,ƣ));if(ȱ=="ng")ѩ.ч.Add(ȶ=>Ѡ(ņ.ו.Ȣ?.Length()??0,ƣ));if(ȱ==
"climb-rate")ѩ.ч.Add(ȶ=>ņ.ו.Ȣ.HasValue&&Ѡ(Vector3D.Dot(ņ.ו.Ĵ,-ņ.ו.ķ),ƣ));}ѩ.џ=ˇ;ѩ.Ѣ=Ũ;if(!ю.ContainsKey(Y))ю.Add(Y,new List<ѐ>());ю[
Y].Add(ѩ);}public void Ӆ(ͻ ȶ,string ò){if(ю.ContainsKey(ò))foreach(var ѩ in ю[ò]){if(ʅ.ƅ-ѩ.ʘ>ѩ.џ){ѩ.ʘ=ʅ.ƅ;if(ѩ.Ѥ(ȶ)){ѩ.ʘ
+=ѩ.Ѣ;ņ.ռ?.ә($"Predicate '{ѩ.ѣ}' passed, adding cooldown ({ѩ.Ѣ})");foreach(var Ã in ѩ.ђ){var Ô=Ã;if(!Ô.Contains(
"query-target"))Ô=Ô.Replace("{id}",ȶ.ͽ.ToString());ņ.ђ.Â(Ô,ņ);}}}}}}struct ӆ{public Vector3D ʂ;public Vector3D Ӈ;public خ ӈ;public ػ ӊ
;}struct Ӊ{public float?ӄ;public Func<ӆ,Ӊ?,Vector3D>M;}class ҿ{public static Vector3D Ӏ(ӆ Ў,Ӊ?Ċ){var J=Ў.ӈ;var Ç=Ў.ʂ;var
ǘ=Ċ?.ӄ??500d;var Ӂ=ҫ(Ў);var т=J.ɷ-Ç;var N=т.Length();т/=N;var Ô=J.ɷ;var ӂ=J.ذ.ӵ;var ҁ=Vector3D.Dot(Ӂ,т);if(ҁ>0.7||ҁ<-0.4)
{var Э=Math.Sign(ҁ)*Vector3D.Cross(Vector3D.Cross(Ӂ,т),т).Normalized();Ô+=Э*ӂ*0.5;}var ƶ=к.л(т,Ӂ).Normalized();Ô+=ƶ*ӂ;var
Ζ=((N<ǘ)?1:-1)*т*Math.Min(ӂ*1.5,Math.Abs(N-ǘ)*2);Ô+=Ζ;return Ô;}public static Vector3D Ӄ(ӆ Ô,Ӊ?Ċ){var J=Ô.ӈ;var Ũ=J.Ŷ.ƍ;
var Ȅ=new Ӊ{ӄ=Ũ};var Ç=Ô.ʂ;var ȶ=Ç-J.ɷ;var N=ȶ.Length();if(N>Ũ*1.3){var Χ=ҫ(Ô);var ӕ=Vector3D.Cross(Χ,ȶ/N).Normalized();var
Ә=new Random((int)J.î.EntityId);var п=Ç+Χ*(Ә.NextDouble()+0.2)+ӕ*(1-Ә.NextDouble());к.о(п,J.ɷ,new BoundingSphereD(Ç,Ũ),
ref Ç);Ô.ʂ=Ç;}else{Ô.ʂ=Ӏ(Ô,Ȅ);Ô.ʂ=ڊ(Ô.ʂ,ҫ(Ô)*J.ذ.ӵ*0.5);}Ô.ʂ=Ӗ(Ô,Ȅ);return Ô.ʂ;}public static Vector3D Ӗ(ӆ Ô,Ӊ?Ċ){var ſ=Ô.ӈ
.ו.ί;if(ſ<100){return Ô.ʂ-Ô.ӈ.ו.ķ*(2-ſ.Value/100)*Ô.ӈ.ذ.ӵ;}return Ô.ʂ;}public static Vector3D ӗ(ӆ Ô,Ӊ?Ċ){var J=Ô.ӈ;var Ӌ=
J.ו.Ĵ;var Ӕ=J.ɷ;J.Ŷ.ǻ(Ô.ʂ);var ȇ=J.Ŷ.ȡ(Ô.ʂ,Ô.Ӈ,Ӌ,J.ו.Ȣ??Vector3D.Zero);var ӌ=J.Ŷ.Ƞ();if(ӌ.Direction!=Vector3D.Zero){var Χ
=J.և.Up;var Ӎ=MatrixD.CreateFromDir(ӌ.Direction,Vector3D.ArePerpendicular(ref ӌ.Direction,ref Χ)?J.և.Up:J.և.Forward);Ӎ.
Translation=ӌ.Position;Ô.ӊ.ؿ=()=>Ӎ;}else Ô.ӊ.ؿ=null;if(J.ɞ.ɡ.ɰ.Й){var ӎ=new RayD(Ӕ,(ȇ-Ӕ).Normalized());var ӏ=J.ɞ.ɡ.ɰ.Ί.Value;var а=
new BoundingSphereD(ӏ.Min,ӏ.Max.X);if(!ӎ.Intersects(а).HasValue)J.Ŷ.ĉ(ȇ);}else J.Ŷ.ĉ(ȇ);return ȇ;}public static Vector3D Ӑ(
ӆ Ô,Ӊ?Ċ){var J=Ô.ӈ;if(!J.Ū())return Ô.ʂ;var ӑ=J.ɷ;var Ĥ=J.ו.ķ;var Ӓ=Ô.ʂ-ӑ;var ӓ=Vector3D.ProjectOnPlane(ref Ӓ,ref Ĥ);var
D=J.և.Forward;var ҁ=Vector3D.Dot(Ӓ.Normalized(),D);if(ҁ<0)ӓ=Vector3D.ProjectOnPlane(ref ӓ,ref D);return ӑ+ӓ;}public
static Vector3D Ȣ(ӆ Ô,Ӊ?Ċ){return Ô.ӈ.Ū()?-Ô.ӈ.ו.ķ:Vector3D.Zero;}public static Vector3D ҭ(ӆ Ô,Ӊ?Ċ){return Ô.ӈ.ɷ+Ȣ(Ô,Ċ)*(Ċ?.ӄ
??1f);}public static Vector3D Ү(ӆ Ô,Ӊ?Ċ){return Ô.ӈ.ɷ+ү(Ô,Ċ);}public static Vector3D ү(ӆ Ô,Ӊ?Ċ){var J=Ô.ӈ;var Ұ=Ċ?.ӄ??1f;
if(J.Ū()){var ұ=J.ו.Ȣ.Value;if(J.ו.Ĵ.LengthSquared()>0&&Ô.ʂ!=J.ɷ){var ˈ=Vector3D.Reject(J.ו.Ĵ,(Ô.ʂ-J.ɷ).Normalized())/5f*Ұ
;return-(ұ+ˈ).Normalized();}return-ұ;}return Vector3D.Zero;}public static Vector3D Ҳ(ӆ Ô,Ӊ?Ċ){return Ô.ʂ-(Ô.ʂ-Ô.ӈ.ɷ).
Normalized()*(Ċ?.ӄ??1f);}public static Vector3D Ҭ(ӆ Ô,Ӊ?Ċ){return Ô.ӈ.Ū()?Ô.ʂ-Ô.ӈ.ו.ķ*(Ċ?.ӄ??1f):Ô.ʂ;}public static Vector3D Ҥ(ӆ Ô
,Ӊ?Ċ){var ҥ=Ô.ӊ.ه;if(ҥ?.Ή!=null){var Ҧ=ҥ.Ή.Value;var ҧ=Ô.ӈ.ɷ-Ô.ʂ;var D=Ҧ.Forward;var Ҩ=Vector3D.ProjectOnVector(ref ҧ,ref
D);return Ҧ.Translation+Ҩ+Ҧ.Forward*(Ċ?.ӄ??1000);}return Ô.ʂ;}public static Vector3D ҩ(Vector3D Ҫ,float Э,خ J){return J.Ū
()?Ҫ-J.ו.ķ*Э:Ҫ;}public static Vector3D ҫ(ӆ Ô){return Ô.ӈ.Ū()?-Ô.ӈ.ו.ķ:(Ô.Ӈ.LengthSquared()>20*20?Ô.Ӈ.Normalized():
Vector3D.UnitY);}public static Vector3D ң(ӆ Ô,Ӊ?Ċ){var ҥ=Ô.ӊ.ه;if(ҥ?.Ή.HasValue==true){var ȶ=(Ô.ʂ-Ô.ӈ.ɷ).Normalized();var Ĥ=ҥ.Ή.
Value.Left;if(Math.Abs(Vector3D.Dot(ȶ,ҥ.Ή.Value.Up))<Math.Abs(Vector3D.Dot(ȶ,Ĥ)))Ĥ=ҥ.Ή.Value.Up;if(Math.Abs(Vector3D.Dot(ȶ,ҥ.
Ή.Value.Forward))<Math.Abs(Vector3D.Dot(ȶ,Ĥ)))Ĥ=ҥ.Ή.Value.Forward;if(Ċ?.ӄ==null&&ҥ.Ί.HasValue)Ċ=new Ӊ{ӄ=(float)ҥ.Ί.Value.
HalfExtents.Length()/2f};return Ô.ʂ+Һ(Ĥ,Ċ);}return Ô.ʂ;}static Vector3D Һ(Vector3D Ĥ,Ӊ?Ċ){return Ĥ*Math.Sin(ʅ.M/3)*(Ċ?.ӄ??30);}
public static Vector3D һ(ӆ Ô,Ӊ?Ċ){return Ô.ʂ+Һ(Ô.ӈ.և.Left,Ċ);}public static Vector3D ҽ(ӆ Ô,Ӊ?Ċ){return Ô.ʂ+Һ(Ô.ӈ.և.Up,Ċ);}
static Vector3D Ҽ(خ J,Vector3D C,float Ş,Vector3D Ĥ){return C+Ĥ*(C-J.ɷ).Length()*Math.Tan(Ş);}public static Vector3D Ҿ(ӆ Ô,Ӊ?Ċ
){return Ҽ(Ô.ӈ,Ô.ʂ,Ċ?.ӄ??0,Ô.ӈ.և.Up);}public static Vector3D Ҹ(ӆ Ô,Ӊ?Ċ){return Ҽ(Ô.ӈ,Ô.ʂ,Ċ?.ӄ??0,Ô.ӈ.և.Left);}public
static Vector3D ҳ(ӆ Ô,Ӊ?Ċ){return Ô.Ӈ==Vector3D.Zero?Ô.ʂ:Ҽ(Ô.ӈ,Ô.ʂ,Ċ?.ӄ??0,Ô.Ӈ.Normalized());}public static Vector3D Ҵ(ӆ Ô,Ӊ?Ċ
){return!Ô.ӈ.Ū()?Ô.ʂ:Ҽ(Ô.ӈ,Ô.ʂ,Ċ?.ӄ??0,Ô.ӈ.ו.ķ);}static Vector3D ҵ(Vector3D C,Vector3D ȗ,float Ҷ){var ϯ=Vector3D.
CalculatePerpendicularVector(ȗ);var Ç=Vector3D.Cross(ȗ,ϯ);return C+Ç*Ҷ*Math.Cos(Math.PI*(ʅ.M/2f))+ϯ*Ҷ*Math.Sin(Math.PI*(ʅ.M/2f));}public static
Vector3D ҷ(ӆ Ô,Ӊ?Ċ){return ҵ(Ô.ʂ,(Ô.ʂ-Ô.ӈ.ɷ).Normalized(),Ċ?.ӄ??30);}public static Vector3D ڵ(ӆ Ô,Ӊ?Ċ){var ȶ=Ô.ʂ-Ô.ӈ.ɷ;var N=ȶ.
Length();var ȗ=ȶ/N;var Ҷ=Ċ?.ӄ??30;if(N>50)return ҵ(Ô.ӈ.ɷ+ȗ*100,ȗ,Ҷ);return Ô.ʂ;}public static Vector3D ڊ(Vector3D Ǽ,Vector3D Ӂ
){return Ǽ+Ӂ*Math.Sin(ʅ.M/2);}public static Vector3D ڋ(Vector3D Ç,خ J,ػ Ą){var ұ=J.ו.ķ;var ȶ=Ç-J.ɷ;var Ь=Vector3D.
ProjectOnPlane(ref ȶ,ref ұ);J.ց?.ә($"bearing: {Ь.Length()}");var Ѐ=555;if(J.ו.ί<Ѐ){var ڀ=(Ѐ-J.ו.ί.Value)/Ѐ;return J.ɷ-ұ*1000*ڀ+Ь.
Normalized()*500;}return J.ɷ+Ь;}public static Vector3D ډ(Vector3D Ç,خ J,ػ Ą){if(J.ɞ.ɡ.ɰ.Й&&J.M.ӥ("avoid-carrier")){var ӏ=J.ɞ.ɡ.ɰ.Ί
.Value;var а=new BoundingSphereD(ӏ.Min,ӏ.Max.X);return к.Ю(Ç,J.ɷ,а);}return Ç;}public static Vector3D څ(Vector3D Ç,خ J,ػ
Ą){if(!J.M.ӥ("allow-1t-up-shifter"))Ą.ؾ=null;var ή=Ą.ـ?.Invoke()??J.ɷ;var چ=(Ą.ؿ?.Invoke()??J.և).Translation;return چ-J.ו
.ˣ(Ç-ή,Ą.ف??J.ذ.Ӵ,Ą.ك,Ą.و?.Ĵ??Vector3D.Zero);}public static Vector3D ڇ(ӆ Ô,Ӊ?Ċ){return Ô.ʂ;}}abstract class ڈ{public
string ǜ;public string Ɲ{get;protected set;}public int ʀ{get;private set;}public bool ڌ;public bool ڐ;public Vector3D?ɷ;public
int?ڑ;public ػ ڒ;public Action ړ;public Action ڔ;public int ڕ;bool ږ;protected خ ņ;protected ڈ(string ò,خ J,bool ڗ=true){Ɲ=
ò;ņ=J;ǜ=ò;ڒ=new ػ();ږ=ڗ;}public void ژ(int Q){ʀ=Q;}public virtual void ڙ(ͻ ښ,ͻ ڛ){ڒ.ه=ښ;ڒ.و=ڛ;}public abstract bool ɵ(int
ƅ,Ĩ ו);public bool ڍ(int ƅ,Ĩ ו){if(ڑ.HasValue&&(ƅ-ڕ>ڑ))return true;return ɵ(ƅ,ו);}bool ڎ;public void ժ(خ J,int ƅ){if(J.ض?
.Status==MyShipConnectorStatus.Connected&&ږ){J.ռ?.ә($"{ǜ}: forcing depart");J.ƃ.Ҍ(ڒ.و);return;}if(J.ق?.IsLocked==true){J.
ق.Unlock();J.ו.Ĭ(PS.WP);}if(ڕ==0)ڕ=ƅ;J.ז.ă(Ɲ+".OnStart");if(!ڎ){ڎ=true;J.ռ?.ә($"Starting task {Ɲ}-{ʀ}");ڔ?.Invoke();}else
J.ռ?.ә($"Resuming task {Ɲ}-{ʀ}");}}class ڏ:ڈ{Func<bool>Қ;public ڏ(خ J,int ʖ=0):base(ʖ==0?"wait-for-signal":$"wait-{ʖ}t",J
,false){if(ʖ!=0)ڑ=ʖ;ڒ.ǜ="Wait";}public ڏ(خ J,Func<bool>Ƶ,int ʖ=0):base("wait-for-condition",J,false){if(ʖ!=0)ڑ=ʖ;ڒ.ǜ=
"Wait";Қ=Ƶ;}public override bool ɵ(int ƅ,Ĩ ו){return ڑ.HasValue&&(ڑ.Value<(ƅ-ڕ))||(Қ?.Invoke()==true);}}class ٽ:ڈ{public ٽ(
Action Ş):base("exec",null,false){ړ=Ş;}public override bool ɵ(int ƅ,Ĩ ו){return true;}}class ٳ:ڈ{double?ٴ;public ٳ(Vector3D C,
خ J,double?ب,bool ό):base("move",J){if(ب!=null)ٴ=ب*ب;ڒ.ن=true;ڌ=true;var ȶ=new ͻ(ǜ);ڔ=()=>ȶ.Α(C);ɷ=C;ڒ.م=true;ڒ.و=ȶ;ڒ.ه=ȶ
;ڒ.ك=ό;if(ό&&(ب??1)==1)ٴ=400;if(ب<0)ٴ=null;ڒ.ؾ=ҿ.Ȣ;}public override bool ɵ(int ƅ,Ĩ ו){if(!ٴ.HasValue)return false;var C=ڒ
.و.Ά.Value;var ȶ=ڒ.ـ?.Invoke()??ņ.ɷ-C;if(ו.ĩ==TD.Rover)ȶ=Vector3D.Reject(ȶ,ņ.և.Up);var ʛ=ȶ.LengthSquared();if(ʛ<100)ڒ.ه=
null;return ʛ<ٴ;}}class ٲ:ٳ{Vector3D ȹ;Vector3D Қ;public ٲ(Vector3D?Ƭ,خ J,Vector3D?Ĥ):base(Vector3D.Zero,J,0,false){Ɲ="land"
;ڌ=true;ڔ=()=>{if(!Ƭ.HasValue){if(J.Ū()){var Э=J.ו.ί.Value;Қ=J.ق.GetPosition()+J.ו.ķ*Э;}else J.ƃ.Ҋ();}else Қ=Ƭ.Value;ɷ=Қ;
if(!Ĥ.HasValue){if(J.Ū())ȹ=J.ו.ķ;else J.ƃ.Ҋ();}else ȹ=Ĥ.Value;var ϲ=Қ-J.ɷ;var N=ϲ.Length();if(N>500&&!J.ו.ī){J.ƃ.ѫ(J.ذ.ӱ<
bool>("capital-behavior")?new ٳ(Қ-ϲ/N*400-ȹ*N/4f,J,50,false):new ٳ(Қ-ϲ/N*400-ȹ*N/4f,J,5,true));}};ڒ.ي=true;ڒ.ؾ=null;ڒ.ؽ=(Ô,Ċ
)=>J.ɷ+ȹ*1000;ڒ.ؼ=(Ô,Ċ)=>Қ;ڒ.ـ=()=>J.ق.GetPosition()+J.ق.WorldMatrix.Down*(ʄ(J.ق)?3.2:0.55);ڒ.ؿ=()=>{var Ú=J.ו.ī?J.և:
MatrixD.CreateFromDir(J.ق.WorldMatrix.Down);Ú.Translation=J.ɷ;return Ú;};}public override bool ɵ(int ƅ,Ĩ ו){if(ņ.ق.IsLocked){ņ.
ו.Ĭ(PS.Inert);return true;}ņ.ق.Lock();return false;}}class ٯ:ڈ{Vector3D?ٱ;Vector3D ٵ;double ٹ;float ٺ;public ٯ(خ J,float
ٻ=0,Vector3D?ټ=null):base("cruise-fw",J){ņ=J;ڐ=true;ڒ.ل=true;ڌ=true;ٺ=ٻ;ٱ=ټ;ڒ.ه=new ͻ(ǜ);ڒ.ه.Α(J.ɷ);ڒ.و=new ͻ(ǜ);ڒ.و.Α(J.
ɷ);ڔ=()=>{if(J.Ū())ٹ=(ņ.ŵ.Value-ņ.ɷ).Length();ٵ=J.ɷ;};ڒ.ؾ=ҿ.Ȣ;ڒ.ك=true;}public override bool ɵ(int ƅ,Ĩ ו){var پ=ņ.ɷ;var ڃ
=ņ.ذ.ӵ*10;if(ņ.Ū()){var D=Vector3D.Reject(ņ.և.Forward,ņ.ו.ķ);if(!D.IsZero())پ+=D.Normalized()*ڃ;}else پ+=(ٱ??ņ.և.Forward)
*ڃ;var ٿ=پ;if(ņ.ו.ί.HasValue){if(ņ.ו.ί<ٺ){var ڀ=(ٺ-ņ.ו.ί.Value)/ٺ;ٿ+=-ņ.ו.ķ*ڃ*ڀ;ņ.ց?.ә($"diff: {ڀ}");}else{var Э=(ņ.ŵ.
Value-ņ.ɷ).Length();if(Э>ٹ){var ڀ=Math.Min(Э-ٹ,ڃ);ٿ+=ņ.ו.ķ*ڃ*Math.Sqrt(ڀ)/150;}}ņ.ց?.ә(
$"elevCap: {ٹ:f1}\nelev: {(ņ.ŵ.Value-ņ.ɷ).Length():f1}");}ڒ.و.Α(ٿ);ڒ.ه.Α(پ);return false;}}class ځ:ڈ{float ڂ;IMyFunctionalBlock ڄ;public ځ(خ J,ڦ N,IMyFunctionalBlock Ą):base(
"jab",J){var ٶ=N?.ɰ??new ͻ("dumb jab");ڔ+=()=>{Vector3D C=J.ו.χ(ڂ);if(Ą!=null){ڒ.ؿ=()=>{var Ú=ʄ(Ą)?MatrixD.CreateFromDir(Ą.
WorldMatrix.Right,Ą.WorldMatrix.Up):MatrixD.CreateFromDir(Ą.WorldMatrix.Up,Ą.WorldMatrix.Right);Ú.Translation=Ą.WorldMatrix.
Translation;return Ú;};C=Ą.WorldMatrix.Translation+ڒ.ؿ().Forward*ڂ;}if(!ٶ.Ά.HasValue){ٶ.Α(C);}};ڄ=Ą;ڒ.و=ٶ;ڒ.ه=ٶ;ڂ=1000;ڒ.ك=true;}
int ٷ;public override bool ɵ(int ƅ,Ĩ ו){if(ڒ.ه.Й){if(ו.ĳ>0.9){var Ú=Vector3D.Dot((ڒ.ؿ?.Invoke()??ņ.և).Forward,ו.Ĵ);var N=(ڒ
.ه.Ά.Value-ņ.ɷ).Length();if((ٷ==0)&&(Ú>ņ.ذ.Ӵ-0.2)&&ņ.Ŷ.Ț((float)(N/Ú)+ņ.ذ.ӱ<float>("torpedo-fuse-offset"),ڒ.ه.ͽ,ڄ)){ٷ=1;ڕ
=ƅ;ڒ.ؼ=(Ô,Ċ)=>Ô.ӈ.ɷ+(ڒ.ؿ?.Invoke()??ņ.և).Forward*(-150);ņ.ռ?.ә($"jabe release at tick {ڕ}");}}}if((ٷ==0)&&(ƅ-ڕ>120))
return true;return(ٷ==1)&&(ƅ-ڕ>60);}}class ٸ:ڈ{void Ȃ(ͻ ȶ,خ J){ڒ.ن=true;ņ=J;ڒ.ه=ڒ.و=ȶ;ڒ.ك=false;ڒ.ؾ=ҿ.ү;ڒ.ؽ=ҿ.ӗ;ړ+=()=>J.Ŷ.ȅ()
;}public ٸ(خ J,Vector3D C):base("attack",J){var ȶ=new ͻ("dumb attack");Ȃ(ȶ,J);ڔ=()=>{ȶ.Α(C);ڒ.ؼ=(Ô,Ċ)=>{var Ũ=J.Ŷ.ƍ;var Ç
=Ô.ʂ;Ô.ʂ=ҿ.ҩ(Ô.ʂ,Ũ/2,J);var ǘ=Ç==Ô.ʂ?Ũ:Ũ*0.86f;var Ȅ=new Ӊ{ӄ=ǘ};Ô.ʂ=ҿ.Ӏ(Ô,Ȅ);Ô.ʂ=ҿ.Ӗ(Ô,Ȅ);return ҿ.ڊ(Ô.ʂ,ҿ.ҫ(Ô)*0.5*ņ.ذ.ӵ
);};};}public ٸ(خ J,ͻ ȶ):base("attack",J){Ȃ(ȶ,J);ڒ.ؼ=ҿ.Ӄ;}public ٸ(خ J,ڦ N):base("attack",J){ڔ=N.ժ;ړ=()=>J.Ŵ(N);Ȃ(N.ɰ,J);
ڒ.ؼ=ҿ.Ӄ;}public override bool ɵ(int ƅ,Ĩ ו){return ڒ.ه.К;}}class ڶ:ڈ{List<Vector3D>ڷ;int ڸ;Vector3D Қ;float ڱ;public ڶ(خ J
,List<Vector3D>Ê,bool É,long ڲ):base("dpath",J,false){ڷ=Ê;var ڳ=J.Ű(Ӟ,ڲ,ڲ);ڔ=()=>ڳ.ժ();ړ=()=>{J.Ŵ(ڳ);if(É)J.ƃ.Ҙ(ڲ);};Ȃ(ڳ.
ɰ);}public ڶ(خ J,List<Vector3D>Ê,bool É):base("dpath",J,false){ڷ=Ê;Ȃ(new ͻ("su-do"));if(É){ړ=()=>{J.ƃ.Ҙ(-1);J.د.œ.ľ(J.Ƴ);
};}}void Ȃ(ͻ ڴ){ڱ=ņ.ذ.ӱ<float>("dpath-speed-limit");ڒ.ه=ڴ;ڒ.ؽ=(Ô,Ċ)=>ņ.ض.GetPosition()-ڴ.Ή.Value.Forward*10000;ڒ.ؿ=()=>ņ.
ו.ī?ņ.և:ņ.ض.WorldMatrix;ڒ.ـ=()=>ņ.ض.GetPosition();ڒ.ؾ=(Ô,Ċ)=>ڴ.Ή?.Up??Vector3D.Zero;ڒ.و=ڴ;ڒ.ؼ=(Ô,Ċ)=>Қ;ڔ+=()=>{ņ.ض.
Disconnect();ņ.ו.Ĭ(PS.WP);};}public override bool ɵ(int ƅ,Ĩ ו){var Ȫ=ڒ.و.Ή;if(Ȫ.HasValue){Қ=Vector3D.Transform(ڷ[ڸ],Ȫ.Value);if((ņ
.ض.GetPosition()-Қ).LengthSquared()<3){if(ڱ>0)ڒ.ف=ڱ;if(++ڸ==ڷ.Count)return true;}}else Қ=ņ.ɷ;return false;}}class ڻ:ڈ{
Vector3D?Қ;Vector3D?ڼ;MatrixD?ڽ;Vector3D?ھ;IMySensorBlock ڿ;public ڻ(خ J,IMySensorBlock Ì,Vector3D?Ĥ):base("follow-playa",J){ڿ=Ì
;ņ=J;ھ=Ĥ;Ȃ(new ͻ("sensor"));}public ڻ(خ J,long Q,Vector3D?C=null):base("follow",J){ņ=J;Қ=C;var Ƃ=J.Ű("apck-position",Q,Q)
;Ȃ(Ƃ.ɰ);ڔ=Ƃ.ժ;ړ=()=>J.Ŵ(Ƃ);}void Ȃ(ͻ ȶ){ڌ=true;ڐ=true;ڒ.ǜ="follow";ڒ.ؼ=(Ô,Ċ)=>ڽ.HasValue?Vector3D.Transform(ڼ.Value,ڽ.
Value):Ô.ӈ.ɷ;ڒ.ؽ=(Ô,Ċ)=>ڽ.HasValue?Ô.ӈ.ɷ+(ھ.HasValue?Vector3D.Rotate(ھ.Value,ڽ.Value):ڽ.Value.Forward):Ô.ӈ.ו.χ(100);ڒ.ؾ=(Ô,Ċ)
=>ڽ.HasValue?ڽ.Value.Up:Ô.ӈ.և.Up;ڒ.و=ڒ.ه=ȶ;}public override bool ɵ(int ƅ,Ĩ ו){if(ڿ!=null){var ڹ=ڿ.LastDetectedEntity;if(!ڹ
.IsEmpty()){ڒ.و.Α(ڹ.Position);ڒ.و.Ĵ=ڹ.Velocity;var Ȫ=ڹ.Orientation;Ȫ.Translation=ڹ.Position;ڒ.و.Ή=Ȫ;ڒ.ه=ڒ.و;}}var ή=ڒ.ـ?.
Invoke()??ņ.î.GetPosition();if(ڒ.و.Ή.HasValue){ڽ=ڒ.و.Ή;if(!ڼ.HasValue)ڼ=Қ??Vector3D.Rotate(ή-ڽ.Value.Translation,MatrixD.
Transpose(ڽ.Value));}else ڽ=null;return false;}}class ں:ڈ{public ں(خ J):base("wingman",J){var ȶ=J.Ʊ?new ͻ("su-w"):J.ɞ.ɡ.ɰ;ڐ=true;
ڒ.ن=true;ڌ=true;ڒ.ه=ڒ.و=ȶ;ڒ.ؽ=(Ô,Ċ)=>Ô.ʂ+ȶ.Ή.Value.Forward*5000;ڒ.ؾ=(Ô,Ċ)=>ȶ.Ή?.Up??Vector3D.Zero;}public override bool ɵ
(int ƅ,Ĩ ו){return false;}}class ڣ:ڈ{public ڣ(خ J,Vector3D C,Vector3D Χ):base("docking",J){var ȶ=new ͻ("dumb dock");ȶ.Α(C
);var Ú=MatrixD.CreateFromDir(Χ);Ú.Translation=C;ȶ.Ή=Ú;Ȃ(ȶ);}public ڣ(خ J,long ƪ):base("docking",J){var N=J.Ű(Ӟ,ƪ,ƪ);Ȃ(N.
ɰ);ڔ=()=>{N.ժ();ņ.ռ?.ә($"Sending docking request to {ƪ}");J.ђ.K("apck.docking.request",new MyTuple<Vector3D,string>(J.ض.
GetPosition(),J.ō),ƪ);};ړ=()=>{J.ђ.K(ӛ,J.ض.GetPosition(),ƪ);J.Ŵ(N);};}public ڣ(خ J):base("docking",J){var ڤ=new ͻ("su-do");Ȃ(ڤ);ڔ=(
)=>J.د.œ.Ğ(J.Ƴ,J.ض.GetPosition(),J.ō);ړ=()=>J.د.œ.ľ(J.Ƴ);}void Ȃ(ͻ ȶ){ڒ.و=ڒ.ه=ȶ;ڒ.ؽ=(Ô,Ċ)=>ņ.ض.GetPosition()-ȶ.Ή.Value.
Forward;var ڡ=Ⱦ<float?>(Г(ņ.ض),"offset")??(ʄ(ņ.ض)?1.25f:0.5f);ڒ.ؼ=(Ô,Ċ)=>Ô.ʂ+ȶ.Ή.Value.Forward*ڡ;ڒ.ؿ=()=>ņ.ו.ī?ņ.և:ņ.ض.
WorldMatrix;ڒ.ـ=()=>ņ.ض.GetPosition();ڒ.ؾ=(Ô,Ċ)=>ȶ.Ή?.Up??Vector3D.Zero;}public override bool ɵ(int ƅ,Ĩ ו){if(ڒ.و.Й){var N=(ڒ.ـ()-ڒ
.و.Ά.Value).LengthSquared();if((N<20)&&(ו.ĳ>0.8)&&(ņ.ض!=null)){ņ.ض.Connect();if(ņ.ض.Status==MyShipConnectorStatus.
Connected){ņ.ו.Ĭ(PS.Inert);return true;}}}return false;}}class ڝ:ڈ{public ڝ(خ J):base("maintenance",J,false){ڐ=true;ڔ=()=>J.ƃ.Ҡ()
;ړ=()=>J.ƃ.Ҡ(false);}public override bool ɵ(int ƅ,Ĩ ו){return false;}}class ڞ:ڈ{Dictionary<string,long>ڟ;Dictionary<
string,long>ڠ=new Dictionary<string,long>();List<IMyInventory>ڜ;List<IMyInventory>ڢ;public ڞ(خ J,Dictionary<string,string>Ő):
base("cargo",J,false){ڔ=()=>{if(J.ض.OtherConnector!=null){ڪ(true);ڪ(false);ڟ=new Dictionary<string,long>();foreach(var ک in
Ő){int ȳ;if(int.TryParse(ک.Value,out ȳ))ڟ.Add(ک.Key,ȳ);}foreach(var N in ڟ)ņ.ռ?.ә($"Desired: {N.Key}: {N.Value}");}};}
void ڪ(bool ګ){var ڬ=ņ.ض.GetInventory(0);var Ï=ņ.צ<IMyCargoContainer>(Ą=>Ą.IsSameConstructAs(ګ?ņ.ض:ņ.ض.OtherConnector)&&Ą.
HasInventory);var ڭ=Ï.Select(Ç=>Ç.GetInventory(0)).Where(Ç=>Ç.IsConnectedTo(ڬ)).OrderByDescending(Ç=>(int)Ç.MaxVolume).ToList();if(ګ
)ڜ=ڭ;else ڢ=ڭ;}List<MyInventoryItem>ڮ=new List<MyInventoryItem>();void گ(bool ڰ,string B,int ڨ){var Ý=ڰ?ڜ:ڢ;var п=ڰ?ڢ:ڜ;
foreach(var N in Ý){ڮ.Clear();N.GetItems(ڮ);foreach(var Ç in ڮ){if(Ç.Type.ToString().Contains(B)){if(ڰ)ڨ*=-1;if(ڨ>0){if(N.
TransferItemTo(п.OrderByDescending(ſ=>(int)ſ.MaxVolume).First(),Ç,ڨ)){ņ.ռ?.ә($"{(ڰ?"Pushed":"Pulled")} {Ç.Type}: {ڨ}");return;}}}}}}
public override bool ɵ(int ƅ,Ĩ ו){ڠ.Clear();foreach(var N in ڟ)ڠ.Add(N.Key,0);foreach(var N in ڜ){var ڥ=new List<
MyInventoryItem>();N.GetItems(ڥ);foreach(var Ç in ڥ){var B=ڠ.Keys.FirstOrDefault(Y=>Ç.Type.ToString().Contains(Y));if(B!=null)ڠ[B]+=Ç.
Amount.RawValue/1000000;}}bool ǘ=false;foreach(var N in ڟ){var ڀ=N.Value-ڠ[N.Key];if(ڀ!=0){ǘ=true;گ(ڀ<0,N.Key,(int)ڀ);}}if(ǘ)ņ
.ց?.ә("Working with cargo");return!ǘ;}}class ڦ{public string ڧ;public ͻ ɰ;public long է;public long?ɿ;public خ ӈ;public
int ը;public bool թ;public void ժ(){ӈ.ג.ծ(this);}}class ի{HashSet<string>լ=new HashSet<string>{ӟ};Dictionary<string,Action<
MyIGCMessage,Dictionary<long,ڦ>>>խ;public ڦ Ű(string ű,long Q,long?Ų,bool ų=false){if(!դ.ContainsKey(ű)){դ.Add(ű,new Օ{Ֆ=ű});դ[ű].գ=
խ.ContainsKey(ű);}var ʌ=դ[ű];if(!ʌ.բ.ContainsKey(Q)){var Ä=new ڦ{ɰ=new ͻ("Empty"){Ό=60},է=Q,ɿ=Ų,ӈ=ʸ,ڧ=ű};ʌ.բ.Add(Q,Ä);if(
ų)Ä.ժ();}ʌ.բ[Q].ը++;ʸ.ռ?.ә($"Get token for data {ʌ.բ[Q].է}, refs: {ʌ.բ[Q].ը}");return ʌ.բ[Q];}public void ծ(ڦ Ä){if(!Ä.թ)
{Ä.թ=true;ʸ.ռ?.ә($"Token start {Ä.ڧ}/{Ä.ɿ}/{Ä.է}");var ű=Ä.ڧ;if(լ.Contains(ű)){դ[ű].ա=ʸ.ב.RegisterBroadcastListener(ű);}
else{if(Ä.ɿ.HasValue)ն(Ä.ɿ.Value,ű,Ä.է);else ʸ.ב.SendBroadcastMessage("apck.unicast.whohas",new MyTuple<string,long>(ű,Ä.է))
;}}}public void զ(string ű,long Q,long Ý){var Ä=դ[ű].բ[Q];Ä.ɿ=Ý;ն(Ý,ű,Q);}public void Ք(ڦ Ä){ʸ.ռ?.ә(
$"Dispose token for data {Ä.է}");var ű=Ä.ڧ;var ʌ=դ[ű];if(--Ä.ը==0){ʌ.բ.Remove(Ä.է);if(Ä.ɿ.HasValue)ʸ.ב.SendUnicastMessage(Ä.ɿ.Value,"apck.unicast.t-",
new MyTuple<string,long>(ű,Ä.է));}ʸ.ռ?.ә($"New ref ctr: {Ä.ը}");if(ʌ.բ.Count<1){if(ʌ.ա!=null)ʸ.ב.DisableBroadcastListener(ʌ
.ա);else ʸ.ב.SendBroadcastMessage("apck.unicast.closed",ű);դ.Remove(ű);}}class Օ{public string Ֆ;public List<MyIGCMessage
>ՙ=new List<MyIGCMessage>();public IMyBroadcastListener ա;public Dictionary<long,ڦ>բ=new Dictionary<long,ڦ>();public bool
գ;}Dictionary<string,Օ>դ=new Dictionary<string,Օ>();خ ʸ;public ի(خ ե){ʸ=ե;խ=new Dictionary<string,Action<MyIGCMessage,
Dictionary<long,ڦ>>>{{ӝ,(Ú,ո)=>{var ɼ=ȯ<MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>>(Ú.Data);if
(ɼ.Item1.Item2!=0){var ȱ=ɼ.Item1.Item2;if(ո.ContainsKey(ȱ))ո[ȱ].ɰ.Ш(Ú.Source,ɼ,ʅ.ƅ);}else{var Ü=ȯ<ImmutableArray<MyTuple<
MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,BoundingBoxD>>>(Ú.Data);foreach(var N in Ü){var ȱ=N.Item1.Item2;if(ո.
ContainsKey(ȱ))ո[ȱ].ɰ.Ш(Ú.Source,N,ʅ.ƅ);}}}},{"apck-position",(Ú,ո)=>{var ɼ=(MyTuple<MatrixD,Vector3D>)Ú.Data;if(ո.ContainsKey(Ú.
Source)){var ȶ=ո[Ú.Source].ɰ;ȶ.Α(ɼ.Item1.Translation);ȶ.Ή=ɼ.Item1;ȶ.Ĵ=ɼ.Item2;ȶ.ͽ=Ú.Source;ȶ.Ώ=Ú.Source;}}}};խ.Add(Ӟ,խ[ӝ]);խ.
Add("apck-wingman",խ[ӝ]);}public void ԡ(List<MyIGCMessage>Ö){foreach(var ʌ in դ.Values){ʌ.ՙ.Clear();int ѝ=0;if(ʌ.ա!=null){
while(ʌ.ա.HasPendingMessage){ѝ++;if(ʌ.գ)խ[ʌ.Ֆ](ʌ.ա.AcceptMessage(),ʌ.բ);else ʌ.ՙ.Add(ʌ.ա.AcceptMessage());}}else{foreach(var
Ú in Ö){if(Ú.Tag==ʌ.Ֆ){ѝ++;if(ʌ.գ)խ[ʌ.Ֆ](Ú,ʌ.բ);else ʌ.ՙ.Add(Ú);}}}ʸ.ց?.ә(
$"{(ʌ.գ?"(t*)":"")}:{ʌ.Ֆ} m:{ѝ} tok: {ʌ.բ.Count}");}foreach(var չ in կ)ʸ.ב.SendUnicastMessage(չ.Key,"apck.unicast.t+.batch",չ.Value.ToImmutableArray());կ.Clear();}public
void պ(string ű,ref List<MyIGCMessage>շ){շ.Clear();if(դ.ContainsKey(ű))շ.AddRange(դ[ű].ՙ);}Dictionary<long,HashSet<MyTuple<
string,long>>>կ=new Dictionary<long,HashSet<MyTuple<string,long>>>();public void ն(long Ý,string ű,long Q){if(!կ.ContainsKey(Ý
))կ.Add(Ý,new HashSet<MyTuple<string,long>>());կ[Ý].Add(new MyTuple<string,long>(ű,Q));}public void հ(string ȱ,byte ձ,
string ղ,Vector3D C,Vector3D ƶ,float ǘ,string ճ){ʸ.ב.SendBroadcastMessage($"apck.unicast.whohas+predicate",new MyTuple<string,
string,MyTuple<string,byte,Vector3D,Vector3D,float>>(ȱ,ճ,new MyTuple<string,byte,Vector3D,Vector3D,float>(ղ,ձ,C,ƶ,ǘ)));}
HashSet<long>մ=new HashSet<long>();public void ɂ(Ծ Ƀ,خ յ,bool œ){var Ȥ=new Ղ();Ȥ.Յ=(Ý,Q)=>մ.Add(Ý);Ȥ.Ճ=(Ý,Q)=>մ.Remove(Ý);Ȥ.Մ=(
Q)=>ʸ.ב.Me==Q;Ȥ.Ն=(ɺ,Ì,C,ƶ,ǘ)=>{bool ɻ=Ծ.Ի(C,ƶ,ǘ,յ.ɷ);if(ɻ&&(ɺ==null||(ɺ=="Dockable"&&œ)||(յ.ō==ɺ)))return ʸ.ב.Me;return-
1;};Ȥ.Հ=()=>{foreach(var Ì in մ){var N=new MyTuple<MatrixD,Vector3D>(յ.î.WorldMatrix,յ.ו.Ĵ);ʸ.ב.SendUnicastMessage(Ì,
"apck-position",N);}};Ƀ.Ժ.Add("apck-position",Ȥ);}}Ծ Ձ;class Ղ{public Action<long,long>Ճ;public Func<long,bool>Մ;public Func<string,
byte,Vector3D,Vector3D,float,long>Ն;public Action<long,long>Յ;public Action Հ;}class Ծ{IMyBroadcastListener Է;
IMyBroadcastListener Ը;IMyBroadcastListener Թ;IMyIntergridCommunicationSystem γ;public Ծ(IMyIntergridCommunicationSystem X){γ=X;Է=X.
RegisterBroadcastListener("apck.unicast.closed");Ը=X.RegisterBroadcastListener("apck.unicast.whohas");Թ=X.RegisterBroadcastListener(
"apck.unicast.whohas+predicate");}public Dictionary<string,Ղ>Ժ=new Dictionary<string,Ղ>();public static bool Ի(Vector3D C,Vector3D ƶ,float ǘ,Vector3D Լ
){bool Ą=false;if(ƶ!=Vector3D.Zero){if(Vector3D.Dot(ƶ,(Լ-C).Normalized())>ǘ)Ą=true;}else{if(C!=Vector3D.Zero){if(Vector3D
.DistanceSquared(Լ,C)<ǘ*ǘ)Ą=true;}else Ą=true;}return Ą;}public void Խ(List<MyIGCMessage>ʽ){while(Է.HasPendingMessage){
var Ú=Է.AcceptMessage();var Կ=(string)Ú.Data;if(Ժ.ContainsKey(Կ))Ժ[Կ].Ճ.Invoke(Ú.Source,0);}while(Ը.HasPendingMessage){var
Ú=Ը.AcceptMessage();var N=(MyTuple<string,long>)Ú.Data;if(Ժ.ContainsKey(N.Item1)&&Ժ[N.Item1].Մ.Invoke(N.Item2))γ.
SendUnicastMessage(Ú.Source,"apck.unicast.ihave",N);}while(Թ.HasPendingMessage){var Ú=Թ.AcceptMessage();var N=(MyTuple<string,string,
MyTuple<string,byte,Vector3D,Vector3D,float>>)Ú.Data;if(Ժ.ContainsKey(N.Item1)&&Ժ[N.Item1].Ն!=null){var Q=Ժ[N.Item1].Ն.Invoke(N
.Item3.Item1,N.Item3.Item2,N.Item3.Item3,N.Item3.Item4,N.Item3.Item5);if(Q>0)γ.SendUnicastMessage(Ú.Source,
"apck.unicast.ihave+callback",new MyTuple<long,string>(Q,N.Item2));}}foreach(var Ú in ʽ){if(Ú.Tag=="apck.unicast.t+.batch"){foreach(var Վ in(
ImmutableArray<MyTuple<string,long>>)Ú.Data)Տ(false,Ú.Source,Վ);}else if(Ú.Tag.Contains("apck.unicast.t")){var N=(MyTuple<string,long>
)Ú.Data;if(Ú.Tag=="apck.unicast.t+")Տ(false,Ú.Source,N);if(Ú.Tag=="apck.unicast.t-")Տ(true,Ú.Source,N);}}foreach(var Ç in
Ժ)Ç.Value.Հ();}void Տ(bool Փ,long Ý,MyTuple<string,long>N){var Կ=N.Item1;var Ր=N.Item2;if(Ժ.ContainsKey(Կ)){if(Փ)Ժ[Կ].Ճ.
Invoke(Ý,Ր);else Ժ[Կ].Յ.Invoke(Ý,Ր);}}}void Ց(List<string>ʌ,bool Ւ){foreach(var Ä in ʌ){if(Ւ&&Ä.Contains("signal"))continue;Ռ(
Ä.Split(new[]{':'},StringSplitOptions.RemoveEmptyEntries),ӈ);Ւ=false;}}List<string>Ս=new List<string>();ڈ Շ=new ڏ(null);
void Ռ(string[]ǌ,خ J,bool Ո=false,bool Չ=false){var Q=Չ?J.ƃ.Ѵ?.ͽ:null;var Ä=փ(ǌ,J,Q);if(Ո)J.ƃ.ѫ(Ä);else{J.ƃ.ѵ(Ä);Ս.Add(
string.Join(":",ǌ));}}void Պ(string[]ǌ){var Ջ=ǌ[2];if(Ջ=="dock"&&(ӈ.ض?.OtherConnector!=null)){var Ҏ=ӈ.ض.OtherConnector;var Ú=Ҏ
.WorldMatrix;var Ĥ=Ú.Forward;var C=Ú.Translation+Ú.Forward*(Ҏ.CubeGrid.GridSizeEnum==MyCubeSize.Large?1.25:0.5);Ռ(
$"command:create-task:dock:AimNormal={Ĥ.X};{Ĥ.Y};{Ĥ.Z}:{Д(C)}".Split(':'),ӈ);}if(Ջ=="land"&&(ӈ.ق!=null)){var C=ӈ.ق.GetPosition()+ӈ.ق.WorldMatrix.Down*(ʄ(ӈ.ق)?3.2:0.55);var Ĥ=ӈ.ق.
WorldMatrix.Down;var Ã=$"command:create-task:land:AimNormal={Ĥ.X};{Ĥ.Y};{Ĥ.Z}:{Д(C)}";Ռ(Ã.Split(':'),ӈ);}if(Ջ=="move"){var Ã=
$"command:create-task:move:{Д(ӈ.ɷ)}";Ռ(Ã.Split(':'),ӈ);}}ڈ փ(string[]ǌ,خ J,long?ƪ){try{J.ռ?.ә($"Parsing task: {string.Join(":",ǌ)}");ڈ Ҕ;var أ=ǌ[2];
Dictionary<string,string>ȿ;Vector3D?Ȥ=null;int ؤ=3;if(ȼ(ؤ,ǌ,out ȿ))ؤ=4;if(ǌ.Length>4){var إ=ǌ.Take(ؤ+3).Skip(ؤ).ToArray();Ȥ=new
Vector3D(double.Parse(إ[0]),double.Parse(إ[1]),double.Parse(إ[2]));}var ئ=Ⱦ<string>(ȿ,"Name");var Q=Ⱦ<long?>(ȿ,"TargetId")??J.ƃ.
Ԑ??-1;var ا=Ⱦ<int?>(ȿ,"Ticks");var ب=Ⱦ<long?>(ȿ,"Proximity");var ˬ=Ⱦ<string>(ȿ,"FlyThrough");var ة=Ⱦ<string>(ȿ,
"Circumnavigate");Vector3D?Ĥ=null;var ت=Ⱦ<string>(ȿ,"AimNormal");if(ت!=null){var ȳ=ت.Split(';');Ĥ=new Vector3D(double.Parse(ȳ[0]),double
.Parse(ȳ[1]),double.Parse(ȳ[2]));}float?ˤ=Ⱦ<float?>(ȿ,"SpeedLimit");switch(أ){case"orbit":var ء=Ⱦ<int?>(ȿ,"R")??500;var Ô
=Ȥ??J.ɷ+J.և.Forward*ء;Ҕ=new ٳ(Ô,J,null,true);var Ȅ=new Ӊ{ӄ=ء};Ҕ.ڒ.ؼ=(ѩ,Ċ)=>ҿ.Ӏ(ѩ,Ȅ);break;case"wait":Ҕ=new ڏ(J,ا??0);
break;case"wait-for-signal":Ҕ=new ڏ(J);Ս.Clear();break;case"exec":Ҕ=new ٽ(()=>J.ђ.Â(Ⱦ<string>(ȿ,"FollowUp").Replace(';',':').
Replace('/','='),J));break;case"cruise-fw":Ҕ=new ٯ(J,Ⱦ<float?>(ȿ,"R")??0,Ĥ);break;case"move":Ҕ=new ٳ(Ȥ.Value,J,ب??1,ˬ=="true");
break;case"move-rel":Ҕ=new ٳ(Vector3D.Zero,J,ب??1,ˬ=="true");Ҕ.ڔ=()=>{var װ=Ⱦ<string>(ȿ,"RemapNG")=="true"&&J.ו.Ȣ.HasValue;
var ױ=J.î.WorldMatrix;if(װ){var D=Math.Abs(Vector3D.Dot(J.ו.ķ,ױ.Left))>.99?ױ.Forward:Vector3D.Cross(J.ו.ķ,ױ.Left);ױ=MatrixD
.CreateFromDir(D,-J.ו.ķ);ױ.Translation=J.ɷ;}var C=Vector3D.Transform(Ȥ.Value,ױ);Ҕ.ڒ.و.Α(C);Ҕ.ɷ=C;var Ş=J.î.WorldMatrix.
Forward;Ҕ.ڒ.ؽ=(ѩ,Ċ)=>J.ɷ+Ş*100;};Ҕ.ɷ=null;break;case"jab":Ҕ=new ځ(J,Q!=-1?J.Ű(ӝ,Q,null):null,null);break;case"attack":if(Ȥ.
HasValue)Ҕ=new ٸ(J,Ȥ.Value);else if(ʵ.ʷ.ɹ&&ʵ.ʷ.ɰ.ͽ==Q)Ҕ=new ٸ(J,ʵ.ʷ.ɰ);else Ҕ=new ٸ(J,J.Ű(ӝ,Q,null));break;case"follow":Ҕ=Q!=-1?
new ڻ(J,Q,Ȥ):new ڻ(J,(IMySensorBlock)J.א.GetBlockWithName("sensor-apck"),Ĥ);break;case"wingman":Ҕ=new ں(J);break;case"dock"
:if(J.ض!=null)Ҕ=Q!=-1?new ڣ(J,Q):new ڣ(J,Ȥ.Value,Ĥ.Value);else Ҕ=Շ;break;case"maintenance":Ҕ=new ڝ(J);break;case"cargo":Ҕ
=J.ض!=null?new ڞ(J,ȿ):Շ;break;case"land":Ҕ=J.ق!=null?new ٲ(Ȥ,J,Ĥ):Շ;break;default:Ҕ=Շ;break;}Ҕ.ǜ=ئ??أ;Ҕ.ڑ=ا??Ҕ.ڑ;Ҕ.ڒ.ف=ˤ
??Ҕ.ڒ.ف;if(ˬ!=null)Ҕ.ڒ.ك=ˬ=="true";if(ة!=null)Ҕ.ڒ.م=ة=="true";return Ҕ;}catch(Exception ex){ʅ.ʊ($"{ex}");}return null;}
static Vector3I[]ײ={new Vector3I(1,0,0),new Vector3I(-1,0,0),new Vector3I(0,1,0),new Vector3I(0,-1,0),new Vector3I(0,0,1),new
Vector3I(0,0,-1)};static M ؠ<M>(IMyCubeBlock Ą,Func<M,bool>Ƶ=null)where M:class,IMyTerminalBlock{foreach(var آ in ײ){var ث=Ą.
CubeGrid.GetCubeBlock(Ą.Position+آ)?.FatBlock as M;if((ث!=null)&&(ث!=Ą)&&(Ƶ==null||Ƶ(ث)))return ث;}return null;}class ػ{public
string ǜ="Default";public Func<ӆ,Ӊ?,Vector3D>ؼ;public Func<ӆ,Ӊ?,Vector3D>ؽ;public Func<ӆ,Ӊ?,Vector3D>ؾ;public Func<MatrixD>ؿ;
public Func<Vector3D>ـ;public float?ف;public bool ك;public bool ي;public bool ل;public bool م;public bool ن;public ͻ ه;public
ͻ و;public Vector3D?ى;public Vector3D?ζ;public Vector3D ٮ;public MatrixD?غ;}Dictionary<string,خ>ج=new Dictionary<string,خ
>();HashSet<خ>ظ=new HashSet<خ>();HashSet<خ>ح=new HashSet<خ>();خ ӈ;class خ{public bool թ;public Program د;public Ć ђ;
public ӳ ذ=new ӳ();public Ӯ M=new Ӯ();public Ɇ ر;public List<IMyFunctionalBlock>ز;public List<IMyFunctionalBlock>س;public
IMyFunctionalBlock ش;public IMyFunctionalBlock ص;public IMyShipConnector ض;public List<IMyShipConnector>ط;public List<IMyWarhead>ع;public
IMyLandingGear ق;public List<IMyBatteryBlock>ת;public List<IMyGasTank>ק;public List<IMyLargeTurretBase>ք;public List<IMyCameraBlock>օ;
public IMyRadioAntenna ֆ;public IMyGyro î;public MatrixD և=>î.WorldMatrix;public IMyGridTerminalSystem א;public
IMyIntergridCommunicationSystem ב;public ի ג;public ɟ ɞ;public ʶ ד;public IMyRemoteControl ה;public Ĩ ו;public Ā ז;public IMyProgrammableBlock ח;public
IMyProgrammableBlock ט;public IMyProgrammableBlock ך;public IMyProgrammableBlock י;public List<IMyProgrammableBlock>ւ;public ө ց;public ө ռ;
HashSet<IMyTerminalBlock>ս=new HashSet<IMyTerminalBlock>();public Vector3D ɷ{get{return և.Translation;}}M վ<M>(string ï,List<
IMyTerminalBlock>ջ,bool տ=false)where M:class,IMyTerminalBlock{M ǘ;ց?.ә($"Looking for {(տ?"single":"")} '{ï}' ({typeof(M).Name})");var ƣ
=(ջ?.Where(Ą=>Ą is M).Cast<M>()??צ<M>(Ç=>Ç.CubeGrid==د.Me.CubeGrid)).Where(Ą=>Ą.CustomName.Contains(ï));ǘ=տ?ƣ.Single():ƣ.
FirstOrDefault();if(ǘ!=null)ս.Add(ǘ);return ǘ;}List<M>ր<M>(List<IMyTerminalBlock>ջ,string Ĥ=null)where M:class,IMyTerminalBlock{var ƣ=
(ջ?.Where(Ą=>Ą is M).Cast<M>()??צ<M>(Ç=>Ç.CubeGrid==د.Me.CubeGrid)).Where(Ą=>(Ĥ==null)||Ą.CustomName.Contains(Ĥ)).ToList(
);foreach(var Ą in ƣ)ս.Add(Ą);return ƣ;}public List<M>צ<M>(Func<M,bool>Ƶ=null)where M:class,IMyTerminalBlock{var Ç=new
List<M>();א.GetBlocksOfType(Ç,Ƶ);return Ç;}void ר<M>(List<M>ש,List<IMyCubeGrid>ƥ,string Ĥ=null)where M:class,
IMyTerminalBlock{ש.AddRange(צ<M>(Ç=>ƥ.Contains(Ç.CubeGrid)&&(Ĥ==null||Ç.CustomName.Contains(Ĥ))));}public خ(ʢ ŏ,Program Ў){var Ő=Г(Ў.Me)
;Ʋ=Ⱦ<int?>(Ő,"rank")??-1;ō=Ⱦ<string>(Ő,"tag")??"";ג=new ի(this);د=Ў;א=Ў.ý;ב=Ў.IGC;ד=Ў.ʵ;ђ=Ў.ą;ɞ=new ɟ(120,150,Ʋ,this);var
ץ=new HashSet<Vector3I>(ŏ.ʣ);var Ǣ=new List<IMyTerminalBlock>();bool פ=!ץ.Any()||DesignMode;ŷ=new Ÿ(د.Me,ץ);if(!פ){א.
GetBlocksOfType(Ǣ,Ą=>ŷ.Ɓ(Ą));}else{ռ=new ө(ʅ.ʉ);Ǣ=null;}ơ(Ǣ);if(פ){ŷ.Ź.Clear();ս.Remove(Ў.Me);if(ص!=null){var כ=new Ÿ(ص,new HashSet<
Vector3I>());כ.ƀ(ս.ToArray());ŏ.ʣ=כ.Ź;ص.CustomData=ŏ.ToString();}else ŷ.ƀ(ս.ToArray());ŏ.ʣ=ŷ.Ź;ʅ.ʉ($"Saving defs: {ŏ.ʣ.Count}");
ŏ.ʝ();}ռ?.Ӛ();ռ=null;}int?ל;public void ם(){if(!ל.HasValue&&ز.Any())ל=0;}void מ(){if(ל.HasValue){ן(ز[ל.Value]);ל++;if(ל==
ز.Count)ל=null;}}void ן(IMyFunctionalBlock נ){var ȍ=ؠ<IMyFunctionalBlock>(נ,Ç=>Ç.CustomName.Contains("sub-base"));if(ȍ!=
null&&(ȍ!=ص)&&Ư.All(Ç=>Ç.ص!=ȍ)&&د.ظ.All(Ç=>Ç.ص!=ȍ)){var ס=new ʢ(null);ס.ʨ(ȍ.CustomData);var ע=new Ÿ(ȍ,ס.ʣ);ռ?.ә("sub defs: "
+ס.ʣ.Count);Func<IMyTerminalBlock,bool>ף=ע.Ɓ;var Ǣ=new List<IMyTerminalBlock>();א.GetBlocks(Ǣ);Ǣ=Ǣ.Where(Ą=>ף(Ą)).ToList(
);ռ?.ә("sub blocks: "+Ǣ.Count);try{var F=new خ(this);F.ش=נ;F.ơ(Ǣ);F.ض?.Connect();F.ƃ.Ҡ();د.ظ.Add(F);F.ō=Ⱦ<string>(Г(נ),
"tag")??"";}catch(Exception ex){ʅ.ʊ($"Sub init failure.\n{ex}");}}}public void Ʃ(خ Ç,long?ƪ,bool ƫ=false){Ç.թ=true;ڈ Ä=null;Ç
.ƃ.Ԑ=Ç.ƃ.Ԑ??ƪ;if(!ƫ){var Ƭ=Ç.ش;foreach(var B in В(Ƭ)){if(B=="jab")Ä=new ځ(this,null,Ƭ);}}Action ƭ=()=>{Ç.ص.Enabled=false;
ʑ.ʕ(()=>{ռ?.ә($"sub-unit[{Ư.IndexOf(Ç)}] starts");Ç.ƃ.Ҡ(false);var Ë=Ç.ה.CustomData.Trim('\n').Split(new[]{'\n'},
StringSplitOptions.RemoveEmptyEntries).Where(Ì=>!Ì.StartsWith("//"));foreach(var Ã in Ë)ђ.Â(Ã,Ç);},1).ʙ(د.ʌ);};if(Ä!=null){ƃ.ѫ(new ٽ(ƭ));ƃ
.ѫ(new ٽ(()=>ƃ.ѫ(Ä)));}else ƭ();}public void Ʈ(خ Ç){Ç.Ů();د.ح.Add(Ç);}public List<خ>Ư=new List<خ>();public خ ư;public
bool Ʊ;public int Ʋ;public string ō;public string Ƴ;public خ(خ C){ư=C;א=ư.א;ב=ư.ב;ד=ư.ד;ђ=ư.ђ;ɞ=ư.ɞ;ג=ư.ג;د=ư.د;Ʊ=true;}
public void ơ(List<IMyTerminalBlock>Ƣ){var ƣ=Ƣ;var Ƥ=ր<IMyMechanicalConnectionBlock>(ƣ);var ƥ=Ƥ.Where(Ç=>Ç.Top!=null).Select(Ç
=>Ç.Top.CubeGrid).ToList();ռ?.ә($"Friend subgrids: {ƥ.Count}");ւ=ր<IMyProgrammableBlock>(ƣ);ר(ւ,ƥ);ռ?.ә($"PBs: {ւ.Count}")
;if(!Ʊ){foreach(var Ą in ւ)ђ.K("apck-handshake","",Ą.EntityId);}î=վ<IMyGyro>("forward-gyro",ƣ,true);var Ʀ=ր<
IMyShipController>(ƣ);ر=new Ɇ(Ʀ,M);var Ƨ=new List<IMyRadioAntenna>();Ƨ=ր<IMyRadioAntenna>(ƣ);ר(Ƨ,ƥ);ֆ=Ƨ.FirstOrDefault();ق=ր<
IMyLandingGear>(ƣ).FirstOrDefault();ط=צ<IMyShipConnector>(Ç=>Ç.IsSameConstructAs(î)&&Ç.CustomName.Contains("dock-host"));ط.ForEach(N=>
N.CustomData="");ض=վ<IMyShipConnector>("docka",ƣ);ת=ր<IMyBatteryBlock>(ƣ);ק=ր<IMyGasTank>(ƣ);ք=ր<IMyLargeTurretBase>(ƣ,
"x-designator");օ=ր<IMyCameraBlock>(ƣ);ר(օ,ƥ);var ƨ=צ<IMyCameraBlock>(Ç=>Ç.IsSameConstructAs(î));ƨ.ForEach(Ç=>Ç.EnableRaycast=true);ز=
ր<IMyFunctionalBlock>(ƣ,"sub-unit");ר(ز,ƥ,"sub-unit");ص=վ<IMyFunctionalBlock>("sub-base",ƣ);س=ր<IMyFunctionalBlock>(ƣ,
"dumb-unit");ع=ր<IMyWarhead>(ƣ);ר(ع,ƥ);ה=ր<IMyRemoteControl>(ƣ).First();var Ơ=ր<IMyTimerBlock>(ƣ);ר(Ơ,ƥ);ז=new Ā(Ơ);ƹ=new List<
IMyTerminalBlock>();ƹ.AddRange(ր<IMyThrust>(ƣ));ƹ.AddRange(ր<IMyArtificialMassBlock>(ƣ));if(!string.IsNullOrEmpty(GGEN_GR_TAG)){var õ=
new List<IMyGravityGenerator>();var ß=א.GetBlockGroupWithName(GGEN_GR_TAG);if(ß!=null)ß.GetBlocksOfType(õ,Ą=>ƣ==null||ƣ.
Contains(Ą));foreach(var Ą in õ)ս.Add(Ą);ƹ.AddRange(õ);}else ƹ.AddRange(ր<IMyGravityGenerator>(ƣ));var ƾ=ր<
IMyUserControllableGun>(ƣ);ר(ƾ,ƥ);ƺ=new Џ(this);ו=new Ĩ(this);List<Vector3D>ƿ=new List<Vector3D>();var ǀ=new List<IMyBlockGroup>();א.
GetBlockGroups(ǀ,Ç=>Ç.Name.Contains("[apck-custom]"));var ǁ=new List<Ǜ>();var ǂ=new List<IMyTimerBlock>();var Ǆ=new List<
IMyUserControllableGun>();foreach(var ǈ in ǀ){var ǅ=new List<IMyTerminalBlock>();ǈ.GetBlocksOfType(ǂ);ǅ.AddRange(Ơ.Where(Ç=>ǂ.Contains(Ç)));ǈ.
GetBlocksOfType(Ǆ);ǅ.AddRange(ƾ.Where(Ç=>Ǆ.Contains(Ç)));if(ǅ.Any()){ǁ.Add(new Ǜ(ǈ,ƒ.Ɠ,ǅ));ƿ.Add(ǅ.First().WorldMatrix.Forward);ƾ.
RemoveAll(Ç=>ǅ.Contains(Ç));}}ǀ.Clear();var ǆ=new List<IMyUserControllableGun>();א.GetBlockGroups(ǀ,Ç=>Ç.Name.Contains(
"[apck-fixed]"));var Ǉ=ǀ.FirstOrDefault();if(Ǉ==null)ǆ.AddRange(ƾ.Where(Ç=>!(Ç is IMyLargeTurretBase)));else Ǉ.GetBlocksOfType(ǆ,Ç=>ƾ.
Contains(Ç));Ŷ=new Ɔ(this);var ƴ=MatrixD.Identity;var Ƽ=new[]{ƴ.Forward,ƴ.Backward,ƴ.Up,ƴ.Down,ƴ.Left,ƴ.Right};foreach(var N in
Ƽ){Func<Vector3D,Vector3D,bool>Ƶ=(Ç,ƶ)=>Vector3D.Dot(Ç,Vector3D.Rotate(ƶ,î.WorldMatrix))>0.75;var Ʒ=ǆ.Where(Ċ=>Ƶ(Ċ.
WorldMatrix.Forward,N)).ToList();var Ƹ=ƿ.Where(Ç=>Ƶ(Ç,N));if(Ʒ.Any()||Ƹ.Any()){Ŷ.Ǝ(Ʒ,ǁ,î,N);}}Ŷ.Ȃ();ד.ʷ.ɴ(this);ƃ=new ԋ(this);î.
GyroOverride=false;if(ض?.Status==MyShipConnectorStatus.Connected)ƃ.ѫ(new ڝ(this));else ו.Ĭ(PS.WP);ذ.ӯ<float>("sp-limit",Ç=>ذ.Ӵ=Ç);ذ.
ӯ<float>("cc-gain",Ç=>ذ.ӵ=Ç);ذ.ӯ<long?>("diag-igc",Ç=>DIAG_IGC=Ç);ذ.ӯ<float>("tv-mult",Ç=>ذ.Ӷ=Ç);}public List<
IMyTerminalBlock>ƹ;public Џ ƺ;int ƻ;void ƽ(){var ǃ=false;var Ɵ=false;ƺ.ɖ(ref ǃ,ref Ɵ);if(ǃ&&ʅ.ƅ-ƻ>120){ƻ=ʅ.ƅ;ƹ.RemoveAll(Ç=>!Ç.IsWorking
||!Ç.IsFunctional);ƺ=new Џ(this);}else if(Ɵ&&ʅ.ƅ-ƻ>10){ƻ=ʅ.ƅ;ƺ.ɑ();}}Ÿ ŷ;class Ÿ{public HashSet<Vector3I>Ź;
IMyTerminalBlock ź;MatrixD Ż;public Ÿ(IMyTerminalBlock ż,HashSet<Vector3I>N){ź=ż;Ż=MatrixD.Transpose(ź.WorldMatrix);Ź=N;}public Vector3I
Ž(IMyCubeBlock Ç){var ž=Vector3D.Rotate(Ç.Position-ź.Position,ź.CubeGrid.WorldMatrix);var ſ=Vector3D.Rotate(ž,Ż);return
Vector3I.Round(ſ);}public void ƀ(params IMyTerminalBlock[]Ą){foreach(var Ç in Ą){var Ƃ=Ž(Ç);Ź.Add(Ƃ);}}public bool Ɓ(
IMyTerminalBlock Ą){return Ą.CubeGrid==ź.CubeGrid&&Ź.Contains(Ž(Ą));}}public Vector3D?ŵ;public bool Ū(){if(ו.Ȣ!=null){if(ŵ==null){
Vector3D ū;if(ה.TryGetPlanetPosition(out ū)){ŵ=ū;return true;}}return ŵ.HasValue;}return false;}public void Ť(){try{ƃ.ԡ();}catch
(Exception ex){ʅ.ʊ($"Cap.HandleTick failure, task: {ƃ.ѳ?.ǜ}\n{ex}");}ػ Ŭ=ƃ.ҝ();ƽ();ו.κ(Ŭ);Ŷ.ē();מ();ռ?.Ӛ();ց?.Ӛ();}bool ŭ
;public void Ů(){if(!ŭ){ŭ=true;ռ?.ә("~Cap Finalizer");foreach(var Ä in ů.Keys.ToList())while(ů[Ä]-->0)ג.Ք(Ä);}}Dictionary
<ڦ,int>ů=new Dictionary<ڦ,int>();public ڦ Ű(string ű,long Q,long?Ų,bool ų=false){var Ä=ג.Ű(ű,Q,Ų,ų);if(!ů.ContainsKey(Ä))
ů.Add(Ä,0);ů[Ä]++;return Ä;}public void Ŵ(ڦ Ä){if(ů[Ä]-->0)ג.Ք(Ä);}public Ɔ Ŷ;public ԋ ƃ;}public enum ƒ{Ɠ=1,Ɣ=2,ƕ=3,Ɩ}
class Ɨ{public string Ƙ;public float ƙ=800f;public float ƚ;public ƒ Ɲ;public Ɨ(ƒ ò,float ñ,float ð,string ï){Ɲ=ò;ƙ=ñ;ƚ=ð;Ƙ=ï;
}}class ƛ{public Ǎ Ɯ;public int ƞ;public int Ƒ;int Ƅ;public bool Ɛ(int ƅ){if(!Ɯ.ǒ)return false;if(ƞ>Ɯ.ǐ.Count){Ƅ=ƅ+Ƒ;ƞ=0;
return true;}if(Ƅ>0){if(Ƅ>ƅ)return true;Ƅ=0;}return false;}}class Ɔ{public List<Ǥ>Ƈ=new List<Ǥ>();LinkedList<Ɨ>ƈ=new
LinkedList<Ɨ>();Ǥ Ɖ;خ ņ;public float Ɗ;public float Ƌ;float ƌ=float.MaxValue;public float ƍ=500;public Ɔ(خ J){ņ=J;}public void Ǝ(
List<IMyUserControllableGun>Ə,List<Ǜ>Ź,IMyTerminalBlock D,Vector3D ǉ){Ƈ.Add(new Ǥ(Ə,Ź,ƈ,D,ǉ,ņ));}List<ƛ>Ǚ=new List<ƛ>();
public void Ȃ(){foreach(var ȃ in Ƈ){ȃ.Đ=õ=>Ǚ.First(Ç=>Ç.Ɯ==õ).ƞ++;foreach(var õ in ȃ.ǯ){Ƌ=Math.Max(Ƌ,õ.Ɨ.ƙ);ƌ=Math.Min(ƌ,õ.Ɨ.ƙ
);var Ȅ=new ƛ(){Ɯ=õ};Ǚ.Add(Ȅ);if(õ.Ɨ.Ƙ=="Heavy ballistic S")Ȅ.Ƒ=6*60;if(õ.Ɨ.Ƙ=="Heavy ballistic L")Ȅ.Ƒ=12*60;if(õ.Ɨ.Ƙ==
"Rail S")Ȅ.Ƒ=20*60;if(õ.Ɨ.Ƙ=="Rail L")Ȅ.Ƒ=60*60;if(õ.Ɨ.Ƙ=="Rocket")Ȅ.Ƒ=10*60;}if(ņ.ք.Any())ƌ=Math.Min(ƌ,ʄ(ņ.î)?700:550);ƍ=ƌ;Ɗ=Ƌ;
if(ņ.ز.Any()||ņ.س.Any())Ɗ=PMW_FF_REACTION_R;}Ɖ=Ƈ.FirstOrDefault();}public void ȅ(){Ƈ.ForEach(Ç=>Ç.ö());}public Ǥ Ȇ(){
return Ɖ;}public void ĉ(Vector3D ȇ){var Ȉ=Ȇ();if(Ȉ!=null){Ȉ.ĉ(ȇ);}}Vector3D ȉ;public void Ȋ(Vector3D Ĥ){if(ȉ!=Ĥ){var ȋ=Ƈ.
FirstOrDefault(Ç=>Ç.ǰ==Ĥ);foreach(var Ǿ in Ƈ){ņ.ռ?.ә($"considering face normal {Ǿ.ǰ} vs {Ĥ}");}Ɖ=ȋ??Ɖ;}ȉ=Ĥ;}public void ȁ(int Ȁ){if(Ȁ
!=-1)Ɖ=Ƈ.ElementAt(Ȁ);else{var X=Ƈ.IndexOf(Ɖ);if(++X>Ƈ.Count-1)Ɖ=Ƈ.FirstOrDefault();else Ɖ=Ƈ.ElementAt(X);}ņ.ռ?.ә(
$"cycled face to {Ɖ.ǰ}");ȉ=Ɖ.ǰ;}int Ǻ;public void ǻ(Vector3D Ǽ){var ǽ=ņ.ذ.ӱ<float>("wb-range-override");if(ǽ>0)ƍ=ǽ;else ƍ=(ņ.M.ӥ(
"wb-snipe-range")?Ƌ:ƌ)-50;var Ǿ=Ɖ;if(Ǿ!=null){var ñ=(Ǽ-ņ.ɷ).Length();var õ=Ǿ.ú();var ǘ=Ǚ.First(Ç=>Ç.Ɯ==õ);if(ǘ.Ɛ(ʅ.ƅ)||((õ.Ɨ.ƙ<ñ)&&(Ƌ>ñ)
))ǿ(Ǿ);else{if(!õ.ǒ&&(ʅ.ƅ>Ǻ+ņ.ذ.ӱ<int>("wb-model-cycle-timeout"))){Ǻ=ʅ.ƅ;ǿ(Ǿ);}}}Ɖ=Ǿ;}void ǿ(Ǥ ƣ){var õ=ƣ.ú();if(õ!=null)
{var ĺ=ƣ.ǯ.IndexOf(õ);for(int X=ĺ+1;;X++){if(X==ƣ.ǯ.Count)X=0;var ǹ=ƣ.ǯ[X];var ǘ=Ǚ.First(Ç=>Ç.Ɯ==ǹ);if((X==ĺ)||!ǹ.ǒ||!ǘ.Ɛ
(ʅ.ƅ)){ƣ.Ĕ(ǹ.Ɨ);break;}}}}public bool Ț(float Ȕ,long ƪ,IMyFunctionalBlock ț=null){bool Ȝ=false;foreach(var Ú in ņ.س.Where
(Ç=>(ț==null)||(Ç==ț))){var ȝ=ؠ<IMyShipMergeBlock>(Ú);if(ȝ!=null){Ȝ=true;var ȟ=ؠ<IMyTimerBlock>(ȝ);if(ȟ!=null){ȟ.
TriggerDelay=Ȕ;ȟ.StartCountdown();}ȝ.ApplyAction("OnOff_Off");ņ.ռ?.ә($"torp away");var F=ņ.Ư.FirstOrDefault(Ç=>Ç.ص==ȝ&&!Ç.թ);if(F!=
null){ņ.Ʃ(F,ƪ,true);}}}return Ȝ;}public void ē(){foreach(var Ǿ in Ƈ){Ǿ.ē();}}public RayD Ƞ(){var ȍ=Ɖ?.ì();if(ȍ.HasValue){
return new RayD(ȍ.Value,Ɖ.û());}return new RayD();}public Vector3D ȡ(Vector3D Ǽ,Vector3D ȑ,Vector3D Ȓ,Vector3D Ȍ){if(Ɖ!=null){
var Ș=Ɖ.Č();if(Ș!=null){var ȍ=Ɖ.ì();if(ȍ!=null){var ȇ=Ȏ(Ș,ȍ.Value,Ǽ,ȑ,Ȓ,Ȍ);return ȇ;}}}return Ǽ;}Vector3D Ȏ(Ɨ ȏ,Vector3D Ȑ,
Vector3D Ǽ,Vector3D ȑ,Vector3D Ȓ,Vector3D Ȍ){Vector3D ȓ=Ǽ-Ȑ;double Ȕ=0;var ȕ=ȑ-Ȓ;if(ȕ.LengthSquared()<double.Epsilon&&Ȍ==
Vector3D.Zero)return Ǽ;switch(ȏ.Ɲ){case ƒ.Ɩ:case ƒ.Ɠ:var Ȗ=к.е(Ȑ,Ȓ,Ǽ,ȑ,ȏ.ƚ,ref Ȕ);if(!Ȍ.IsZero())Ȗ-=Ȍ*Ȕ*Ȕ/2f;return Ȗ;case ƒ.Ɣ:
var N=ȓ.Length();var ȗ=ȓ/N;Vector3D ș=ȗ*100;Vector3D Ȟ=ș+Ȓ;double Ǹ=Vector3D.Dot(ȗ,Ȟ);Ǹ=Math.Min(Ǹ,200);var ǲ=(200-Ǹ)*(200-
Ǹ)/(2*600);var ǚ=(ǲ*(200+Ǹ)/2+(N-ǲ)*200)/N;return к.е(Ȑ,Ȓ,Ǽ,ȑ,ǚ,ref Ȕ);case ƒ.ƕ:return Ǽ;default:return Ǽ;}}}class Ǜ{
public float ƚ{get;}public float ƙ{get;}public float Ǔ{get;}public string ǜ{get;}public bool ǒ{get;}public IMyBlockGroup ǝ{get
;}public float Ǟ{get;}public ƒ ƒ{get;}public List<IMyTerminalBlock>ǟ{get;}public Ǜ(IMyBlockGroup Ǡ,ƒ ǡ,List<
IMyTerminalBlock>Ǣ){ǝ=Ǡ;var Ő=Г(null,Ǡ.Name);var ǣ=Ⱦ<float?>(Ő,"v");var ǘ=Ⱦ<float?>(Ő,"r");var ǖ=Ⱦ<float?>(Ő,"d");var ǋ=Ⱦ<float?>(Ő,
"fwO");if(ǣ==null||ǘ==null){var í=$"Custom group '{Ǡ.Name}' definition failure. Expected v and r tags, e.g. 'Heavy Dakka [apck-custom][ripple][v=500][r=1000][d=5]'"
;ʅ.ʉ(í);throw new Exception(í);}Ǔ=ǖ??5;Ǟ=ǋ??0;ƙ=ǘ??0;ƚ=ǣ??0;ƒ=ǡ;var ǌ=Ǡ.Name.Split('[').Select(Ç=>Ç.Trim(']')).ToArray();
ǒ=ǌ.Any(Ç=>Ç=="ripple");ǜ=ǌ[0];ǟ=Ǣ;}}class Ǎ{public IMyTerminalBlock ǎ;public Vector3D Ǐ;public List<
IMyUserControllableGun>ǐ=new List<IMyUserControllableGun>();public string Ǒ;public Ɨ Ɨ;public bool ǒ;public float Ǔ;public int ǔ;public int Ǖ;
public Action Ǌ;public Action Ǘ;}class Ǥ{IMyTerminalBlock ǭ;خ ņ;public bool Ǯ{get;private set;}public List<Ǎ>ǯ;public Vector3D
ǰ{get;private set;}public Ǥ(List<IMyUserControllableGun>Ə,List<Ǜ>Ǳ,LinkedList<Ɨ>ó,IMyTerminalBlock Ƿ,Vector3D ǉ,خ J){ņ=J;
ǯ=new List<Ǎ>();var ǳ=Ƿ.WorldMatrix.Translation;var Ǵ=MatrixD.Transpose(Ƿ.WorldMatrix);foreach(var Ǡ in Ǳ){var ǵ=ũ(Ǡ,ó);
var Ƕ=new List<IMyUserControllableGun>();Ǡ.ǝ.GetBlocksOfType(Ƕ,Ç=>Ǡ.ǟ.Contains(Ç));ǵ.ǐ.AddRange(Ƕ);ǵ.ǒ=Ǡ.ǒ;J.ռ?.ә(
$"Added custom weapon group '{Ǡ.ǜ}', gun count: {Ƕ.Count}, ripple: {ǵ.ǒ}");var Ǭ=new List<IMyTimerBlock>();Ǡ.ǝ.GetBlocksOfType(Ǭ,Ç=>Ǡ.ǟ.Contains(Ç));IMyTimerBlock Ǫ=null;IMyTimerBlock ǥ=null;
IMyTimerBlock Ǧ=null;foreach(var Ä in Ǭ){foreach(var B in В(Ä)){if(B=="ref")Ǫ=Ä;if(B=="fire")ǥ=Ä;if(B=="cease")Ǧ=Ä;}}Ǫ=Ǫ??Ǭ.
FirstOrDefault();if(Ǫ!=null){ǵ.Ǌ=()=>{if(ǥ!=null)ǥ.Trigger();else Ǫ.Enabled=true;};ǵ.Ǘ=()=>{if(Ǧ!=null)Ǧ.Trigger();else Ǫ.Enabled=
false;};ǵ.ǎ=Ǫ;var ǧ=Ǫ.GetPosition()+Ǫ.WorldMatrix.Forward*Ǡ.Ǟ-ǳ;ǵ.Ǐ=Vector3D.Rotate(ǧ,Ǵ);J.ռ?.ә("Added custom trigger");}}
foreach(var å in Ə){var Î=å.BlockDefinition.TypeId+"/"+å.BlockDefinition.SubtypeName;if(!ǯ.Any(Ç=>Ç.Ǒ==Î)){if(Î.Contains(
"MyObjectBuilder_SmallGatlingGun"))ũ(Î,"Light ballistic",800f,800f,5,ƒ.Ɠ,ó);else if(Î.Contains("SmallBlockMediumCalibreGun")){ũ(Î,"Heavy ballistic S",
500f,1400f,2,ƒ.Ɠ,ó).ǒ=true;}else if(Î.Contains("LargeBlockLargeCalibreGun")){ũ(Î,"Heavy ballistic L",500f,2000f,2,ƒ.Ɠ,ó).ǒ=
true;}else if(Î.Contains("SmallRailgun")){ũ(Î,"Rail S",1000f,1400f,2,ƒ.Ɠ,ó).ǒ=true;}else if(Î.Contains("LargeRailgun")){ũ(Î,
"Rail L",2000f,2000f,2,ƒ.Ɠ,ó).ǒ=true;}else if(Î.Contains("SmallMissileLauncher")){ũ(Î,"Rocket",200f,800f,10,ƒ.Ɣ,ó).ǒ=true;}}var
Ǩ=ǯ.FirstOrDefault(Ç=>Ç.Ǒ==Î);if(Ǩ!=null)Ǩ.ǐ.Add((IMyUserControllableGun)å);else ʅ.ʊ(
$"Failed to parse weapon subtype: {Î}, custom name: {å.CustomName}");}Vector3D ǩ;foreach(var ù in ǯ){ǩ=Vector3D.Zero;if(ù.ǐ.Count>0){ǩ=Vector3D.Zero;ù.ǐ.ForEach(Ç=>ǩ+=Ç.GetPosition());ǩ/=
ù.ǐ.Count;ù.ǎ=ù.ǐ.OrderBy(Ç=>(ǩ-Ç.GetPosition()).Length()).First();ù.Ǐ=Vector3D.Rotate(ǩ-ǳ,Ǵ);}J.ռ?.ә(
$"Group {ù.Ɨ.Ƙ}: {ù.ǐ.Count}");}ǭ=Ƿ;ǰ=ǉ;Ǯ=true;ċ=ǯ.FirstOrDefault()?.Ɨ;}Ǎ ũ(Ǜ ǫ,LinkedList<Ɨ>ó){return ũ(ǫ.ǝ.Name,ǫ.ǜ,ǫ.ƚ,ǫ.ƙ,ǫ.Ǔ,ǫ.ƒ,ó);}Ǎ ũ(string
Î,string ï,float ð,float ñ,float ç,ƒ ò,LinkedList<Ɨ>ó){var ô=ó.FirstOrDefault(Ç=>Ç.Ƙ==ï);if(ô==null){ô=new Ɨ(ò,ñ,ð,ï);ó.
AddLast(ô);}var õ=new Ǎ(){Ǒ=Î,Ɨ=ô,Ǔ=ç};ǯ.Add(õ);return õ;}public void ö(){Ǯ=true;ø();}public void ø(){if(Ǯ){foreach(var ù in ǯ)
{ù.Ǘ?.Invoke();foreach(var õ in ù.ǐ)õ.Shoot=false;}Ǯ=false;}}public Ǎ ú(){if(ċ!=null)return ǯ.FirstOrDefault(Ç=>Ç.Ɨ==ċ);
return null;}public Vector3D û(){var ß=ú();if(ß.ǎ!=null&&ß.ǎ.GetPosition().IsZero())ß.ǎ=ß.ǐ.FirstOrDefault(Ç=>Ç.IsWorking);
return ß.ǎ?.WorldMatrix.Forward??Vector3D.Zero;}public Vector3D?ì(){if(ċ!=null)foreach(var ß in ǯ.Where(Ç=>Ç.Ɨ==ċ)){if(!ņ.M.ӥ(
"coax-ripple")||!ß.ǒ)return Vector3D.Transform(ß.Ǐ,ǭ.WorldMatrix);var à=ß.ǐ[ß.ǔ];if(à.IsWorking)return à.WorldMatrix.Translation;đ(ß)
;return null;}return null;}public float á;public double â;double ã;bool ä(IMyTerminalBlock å,Vector3D æ,float ç,bool è){
var é=ņ.ذ.ӱ<float>("wb-precision-override");if(é>0)ç=é;var ê=å.GetPosition();var N=(ê-æ).Length();var ë=Vector3D.Dot((æ-ê)/
N,å.WorldMatrix.Forward);var Þ=Math.Acos(Math.Min(ë,1f));var í=Math.Tan(Þ)*Math.Max(100,N);var ü=Math.Abs(í-ã);ã=í;á=ç;â=
Math.Tan(Þ)*N;return(N<ċ.ƙ)&&(Þ<Math.PI/4f)&&(â<ç)&&(!è||ü<0.5f);}public void ĉ(Vector3D æ){if(ċ!=null)foreach(var ß in ǯ.
Where(Ç=>Ç.Ɨ==ċ)){if(ņ.M.ӥ("coax-ripple")&&ß.ǒ){var Ċ=ß.ǐ[ß.ǔ];if(ä(Ċ,æ,ß.Ǔ,true)){if(!Ċ.IsWorking)đ(ß);else{if(!Ǯ){Ċ.Shoot=
true;Ǯ=true;if(ß.Ǖ==0)ß.Ǖ=ʅ.ƅ;}ņ.ց?.ә($"{ß.Ɨ.Ƙ}: ripple ({ß.ǔ}/{ß.ǐ.Count})");Ē=true;}}}else{if(ä(ß.ǎ,æ,ß.Ǔ,false)){if(!Ǯ){ß
.Ǌ?.Invoke();foreach(var õ in ß.ǐ)õ.Shoot=true;Ǯ=true;}ņ.ց?.ә($"{ß.Ɨ.Ƙ}: firing salvo");Ē=true;}}if(!Ē)ņ.ց?.ә(
$"{ß.Ɨ.Ƙ} ({ß.ǔ}) AimError: {â:f2}");}}Ɨ ċ;public Ɨ Č(){return ċ;}Ɨ Ď;public void Ĕ(Ɨ ď){Ď=ď;}public Action<Ǎ>Đ;void đ(Ǎ ß){ß.ǐ[ß.ǔ].Shoot=false;ß.ǔ++;ß.Ǖ=
0;Ǯ=false;if(ß.ǔ>ß.ǐ.Count-1)ß.ǔ=0;Đ?.Invoke(ß);}bool Ē;public void ē(){if((Ď!=null)&&(Ď!=ċ)){if(ǯ.Any(Ç=>Ç.Ɨ==Ď)){ċ=Ď;Ǯ=
true;ø();}else ċ=null;Ď=null;}foreach(var ß in ǯ){if(ß.Ǖ>0){if(ʅ.ƅ-ß.Ǖ>(ß.Ɨ.Ƙ.Contains("Rail")?ņ.ذ.ӱ<int>(
"ripple-increment-interval-rail"):ņ.ذ.ӱ<int>("ripple-increment-interval"))){đ(ß);}}}if(!Ē)ø();Ē=false;}}void Ĉ(){ʡ.ʣ.Clear();Save();ć();}
IMyGridTerminalSystem ý;void ć(){ʅ.ƅ=0;ʌ=new Queue<ʑ>();ʡ=new ʢ((Ì)=>Storage=Ì);ʡ.ʨ(Storage);ʵ=new ʶ();ӈ=new خ(ʡ,this);ӈ.ռ?.ә(
$"IGC id: {IGC.Me}");ɞ=ӈ.ɞ;ج.Clear();ӈ.ם();ӈ.թ=true;ʵ.Ȃ(ӈ);if(ӈ.ط.Any())œ=new ő(ӈ,ʡ,ý);Ձ=new Ծ(IGC);ʵ.ɂ(Ձ);ӈ.ג.ɂ(Ձ,ӈ,œ!=null);ӈ.ɞ.ɂ(Ձ);if(!
string.IsNullOrEmpty(Me.CustomData)||!string.IsNullOrEmpty(Storage))ÿ=true;}void Save(){ʡ?.ʝ();}bool þ;bool ÿ;class Ā{
Dictionary<string,IMyTimerBlock>ā=new Dictionary<string,IMyTimerBlock>();public List<IMyTimerBlock>Ă{get;}public Ā(List<
IMyTimerBlock>ā){Ă=ā;}public void ă(string ï){IMyTimerBlock Ą;if(!ā.TryGetValue(ï,out Ą)){Ą=Ă.FirstOrDefault(Ô=>Ô.CustomName.Contains
(ï));if(Ą!=null)ā.Add(ï,Ą);}Ą?.Trigger();}}Ć ą;class Ć{Dictionary<string,Action<string[],خ>>Ë;
IMyIntergridCommunicationSystem č;public Ć(IMyIntergridCommunicationSystem X,Dictionary<string,Action<string[],خ>>j){Ë=j;č=X;}public void ª(string Q,
string[]I,خ J){Ë[Q].Invoke(I,J);}public void µ(string º,long Q=0){if(Q!=0)À(º,Q);else À(º);}void À(string Á,params long[]O){K(
Ӝ,Á,O);}public void Â(string Ã,خ J){J.ռ?.ә($"Got apck cmd: {Ã}");Ã=Ã.Replace("{me}",J.ב.Me.ToString());var I=Ã.Split(':')
;var Å=I[0];if(Å.StartsWith("<")){var Ä=Å.Trim('<','>');if(!J.ō.Contains(Ä))return;else I=I.Skip(1).ToArray();}Å=I[0];if(
Å=="toggle"){var Y=I[1];if(I.Length>2)J.M.Ӣ(Y,bool.Parse(I[2]));else J.M.Ӥ(Y);J.M.Ө(Y,J);return;}if(Å=="command"){ª(I[1],
I,J);return;}int?X=null;string B=null;if(Å.Contains("]")){X=int.Parse(Å.Split('[').Select(C=>C.Trim(']')).Skip(1).First()
);Å=Å.Split('[').First();}if(Å.Contains(">")){B=Å.Split('<').Select(C=>C.Trim('>')).Skip(1).First();Å=Å.Split('<').First(
);}var D=string.Join(":",I.Skip(1).ToArray());if(B!=null)D=$"<{B}>:{D}";if(Å=="bc")À(D);if(Å=="w"){if(X.HasValue&&(J.ɞ.ɢ.
Count>X))À(D,J.ɞ.ɢ.ElementAt(X.Value));else À(D,J.ɞ.ɢ.ToArray());}if(Å=="su"){if(X.HasValue&&(J.Ư.Count>X))ª(I[2],I.Skip(1).
ToArray(),J.Ư[X.Value]);else foreach(var F in J.Ư)ª(I[2],I.Skip(1).ToArray(),F);}if(Å=="recursive"){H(I,J);}if(Å=="p"&&(J.ɞ.ɠ!=
0))À(D,J.ɞ.ɠ);}void H(string[]I,خ J){ª(I[2],I.Skip(1).ToArray(),J);foreach(var F in J.Ư){H(I,F);}}public void K<M>(string
B,M N,params long[]O){if(!O.Any()){Ж.р++;č.SendBroadcastMessage(B,N);}else{foreach(var Q in O){Ж.р++;č.SendUnicastMessage
(Q,B,N);}}}public void A(Vector3D C,string Æ,string Ï,int Ð,string Ñ,bool Ò=false){var Ó=Ï.Trim('#');var Ô=new Color(Õ(Ó,
0),Õ(Ó,2),Õ(Ó,4));č.SendBroadcastMessage(Ò?"cmdr.persist-targetable":"cmdr.persist-projection",new MyTuple<string,Vector2
,Vector3D,Vector4,string>(Æ,Vector2.One*Ð,C,Ô.ToVector4(),Ñ));}int Õ(string Ç,int X){return int.Parse(Ç.Substring(X,2),
System.Globalization.NumberStyles.HexNumber);}}List<string>Ü=new List<string>();List<MyIGCMessage>Ö=new List<MyIGCMessage>();
void Main(string Ø){if(!þ){try{ʅ.Ȃ(this);ć();þ=true;Runtime.UpdateFrequency=UpdateFrequency.Update1;}catch(Exception ex){
Runtime.UpdateFrequency=UpdateFrequency.None;Echo(ex.ToString());if(Ø=="command:clear-state")Storage="";else if(Ø==
"command:clear-defs"){Ĉ();Runtime.UpdateFrequency=UpdateFrequency.Update1;}return;}}ʅ.ʋ?.Invoke();ʅ.ҹ?.Invoke(
$"Subordinates: {ӈ?.ɞ.ɢ.Count}");ʅ.ƅ++;ʅ.ʆ=Math.Max(0.001,Runtime.TimeSinceLastRun.TotalSeconds);ʅ.M+=ʅ.ʆ;if(DIAG_IGC.HasValue)Echo("WARNING! DbgIgc");
Echo($"AutoPillock Core v.{Ver}");ӈ.ց?.ә($"Dockable: {ӈ.ض!=null} Carrier: {œ!=null}\nDesignators: {ʵ.ʷ.ɯ.Count}");ӈ.ց?.ә(
"Services found:");ӈ.ց?.ә($"TGP: {ӈ.ך!=null} TP: {ӈ.ח!=null}");ӈ.ց?.ә($"TMC: {ӈ.י!=null} Osvc: {ӈ.ט!=null}");Ü.Clear();Ö.Clear();while(
IGC.UnicastListener.HasPendingMessage){Ö.Add(IGC.UnicastListener.AcceptMessage());}ӈ.ց?.ә($"Receiving unicasts: {Ö.Count}")
;try{ӈ.ג.ԡ(Ö);}catch(Exception ex){ʅ.ʊ($"{ex}");throw;}var Ù=IGC.RegisterBroadcastListener(Ӝ);while(Ù.HasPendingMessage){
var Ú=Ù.AcceptMessage();Ü.AddRange(Ʉ(Ú.Data.ToString()));}var Û=false;foreach(var Ú in Ö){var Ý=Ú.Source;var B=Ú.Tag;Ж.Ъ++;
if(B==Ӝ)Ü.AddRange(Ʉ(Ú.Data.ToString()));else{if(B=="apck-handshake-reply"){if((string)Ú.Data=="TGP")ӈ.ך=ӈ.ւ.First(Ç=>Ç.
EntityId==Ú.Source);if((string)Ú.Data=="TP"){ӈ.ח=ӈ.ւ.First(Ç=>Ç.EntityId==Ú.Source);ӈ.ח.TryRun(
$"[command:set-mode:{ӈ.ו.ĩ}],[toggle:vtol-hybrid:{ӈ.ו.Ī==3}]");}if((string)Ú.Data=="OutputSvc")ӈ.ט=ӈ.ւ.First(Ç=>Ç.EntityId==Ú.Source);if((string)Ú.Data=="TMC")ӈ.י=ӈ.ւ.First(Ç=>Ç.
EntityId==Ú.Source);}if(B=="apck.tac.addchild"){ɞ.ɢ.Add(Ý);}else if(B=="tp.force-report"){ӈ.ו.ι=(Vector3D)Ú.Data;}else if(B==ӛ){
œ.ľ(Ý.ToString());}else if(B=="apck.dpath.add:node"){œ?.ļ((MatrixD)Ú.Data);}else if(B=="apck.dpath.add:entry"){œ?.ļ((
MatrixD)Ú.Data,true);}else if(B=="diag.set-storage"){Storage=(string)Ú.Data;ć();}else if(B=="apck.docking.request"){var È=(
MyTuple<Vector3D,string>)Ú.Data;œ?.Ğ(Ý.ToString(),È.Item1,È.Item2);}else if(B.Contains("dpath.exec")){var É=Ú.Tag.Contains(
"depart");var Ê=(ImmutableArray<Vector3D>)Ú.Data;ӈ.ƃ.ѫ(new ڶ(ӈ,Ê.Reverse().ToList(),É,Ú.Source));}else if(!Û){if(B==
"apck.unicast.ihave"){Û=true;var N=(MyTuple<string,long>)Ú.Data;ӈ.ג.զ(N.Item1,N.Item2,Ú.Source);}else if(B=="apck.unicast.ihave+callback"){Û
=true;var N=(MyTuple<long,string>)Ú.Data;ӈ.ђ.Â(N.Item2.Replace("{id}",N.Item1.ToString()),ӈ);}}}}ɞ.Ť();œ?.Ť();if(ÿ&&
string.IsNullOrEmpty(Ø)){ÿ=false;var Ë=Me.CustomData.Trim('\n').Split(new[]{'\n'},StringSplitOptions.RemoveEmptyEntries).Where
(Ì=>!Ì.StartsWith("//")).Select(Ì=>"["+Ì+"]").ToList();Ø=string.Join(",",Ë);}if(!string.IsNullOrEmpty(Ø)&&Ø.Contains(":")
){Ж.Ъ++;Ü.AddRange(Ʉ(Ø));}foreach(var Ã in Ü){ӈ.ђ.Â(Ã,ӈ);}ɞ.Ȯ();ʵ.ɵ(Ö);Ձ.Խ(Ö);ӈ.Ť();var Í=ج.Count;if(Í>0)Ř.AppendLine(
$"Units: {Í}");var Ä=ʅ.ƅ%10;int X=0;foreach(var J in ج.Values){if(Í<5||X++%10==Ä){if(!J.î.IsFunctional||!J.ה.IsFunctional||J.î.Closed
||J.ה.Closed)J.ư.Ʈ(J);else{try{J.Ť();}catch(Exception ex){ʅ.ʊ($"SubUnit failure, removing.\n{ex}");J.ư.Ʈ(J);}}}}if(ظ.Count
>0){foreach(var J in ظ){J.Ƴ=$"su-{J.ص.EntityId}-{J.ش.GetHashCode()}-{ʅ.ƅ}";ج.Add(J.Ƴ,J);J.ư.Ư.Add(J);J.ם();ӈ.ռ?.ә(
$"SU add: '{J.Ƴ}'");}ظ.Clear();ӈ.ך?.TryRun("command:refresh-designators");}if(ح.Count>0){var Ô=ج.Count;foreach(var J in ح){ج.Remove(J.Ƴ);J
.ư.Ư.Remove(J);}ӈ.ռ?.ә($"SU remove: {Ô}->{ج.Count}");ح.Clear();}var Ŕ=ӈ.ט;if((Ŕ!=null)&&IGC.IsEndpointReachable(Ŕ.
EntityId)&&(Ŕ.Enabled)){var ŕ=Ŕ.EntityId;ӈ.ց?.ә("Delegating output to "+Ŕ.EntityId);Ř.AppendLine(ӈ.M.ӥ("damp-when-idle")?"DMP":
"INR");Ř.AppendLine((ӈ.ו.ĩ!=0)&&(ӈ.ח!=null)?"THR":"HC");ŝ(ŕ,Ř.ToString(),new Vector2(0f,3.8f/4),1f);Ř.Clear();Ř.AppendLine(ӈ.
ƃ.щ==RS.Attacking?"TURR REACT":"");ŝ(ŕ,Ř.ToString(),new Vector2(0f,1.15f),0.6f);Ř.Clear();ś(ŕ);}else if(DIAG_IGC.HasValue
)ś(DIAG_IGC.Value);if(ӈ.ց!=null){Ř.AppendFormat("Processed in {0:f3} ms\n",Runtime.LastRunTimeMs).AppendFormat(
"sentMsgCount: {0}\n",Ж.р).AppendFormat("receivedCmdCount: {0}\n",Ж.Ъ).AppendFormat("parseVectorsCount: {0}\n",Ж.С).AppendFormat(
"TacNode.Parent: {0}\n",ɞ.ɠ).AppendFormat("TacNode.Children: {0}\n",ɞ.ɢ.Count).AppendFormat("Logged errors: {0}\n",ʅ.ʇ);ӈ.ց?.ә(Ř.ToString());}Ř
.Clear();ʐ();ŗ.Enqueue(Runtime.LastRunTimeMs);if(ŗ.Count==100){double Ŗ=0;foreach(var Ç in ŗ)Ŗ+=Ç;Echo($"100 runs avg {(Ŗ/100f).ToString("f3")} ms, instr.: {(float)Runtime.CurrentInstructionCount/Runtime.MaxInstructionCount*100:f1}%"
);ŗ.Dequeue();}}Queue<double>ŗ=new Queue<double>();StringBuilder Ř=new StringBuilder();List<MyTuple<string,Vector3D,
ImmutableArray<string>>>ř=new List<MyTuple<string,Vector3D,ImmutableArray<string>>>();void Ś(string B,Vector3D C,params string[]Ì){ř.
Add(new MyTuple<string,Vector3D,ImmutableArray<string>>(B,C,Ì.ToImmutableArray()));}void ś(long Ŝ){IGC.SendUnicastMessage(Ŝ
,"hud.apck.proj",ř.ToImmutableArray());ř.Clear();}void ŝ(long Ş,string Ä,Vector2 C,float Ð){IGC.SendUnicastMessage(Ş,
"draw-text",new MyTuple<string,Vector2,float>(Ä,C,Ð));}ő œ;class ő{خ ņ;List<Ł>Ň;string ň;IMyBroadcastListener ŉ;Dictionary<
IMyShipConnector,ŋ>Ŋ;class ŋ{public Vector3D Ō;public string ō;public float Ŏ;}public ő(خ J,ʢ ŏ,IMyGridTerminalSystem ý){ņ=J;Ň=ŏ.ʤ;Ŋ=new
Dictionary<IMyShipConnector,ŋ>();foreach(var N in J.ط){var Ő=Г(N);Ŋ.Add(N,new ŋ{Ō=N.GetPosition(),ō=Ⱦ<string>(Ő,"tag"),Ŏ=Ⱦ<float?>
(Ő,"offset")??(ʄ(N)?1.25f:0.5f)});}ŉ=J.ב.RegisterBroadcastListener("apck.depart.request");foreach(var Ņ in Ň){J.ռ?.ә(
$"node: {Ņ.Ń}");}}ͻ Œ=new ͻ("docking");List<Vector3D>ş;public void Ť(){ņ.ց?.ә("Navmesh: "+Ň.Count);while(ŉ.HasPendingMessage){var Ú=ŉ.
AcceptMessage();var N=(MyTuple<long,Vector3D>)Ú.Data;if(ņ.ط.Any(Ç=>Ç.EntityId==N.Item1))Ğ(Ú.Source.ToString(),N.Item2,"",true);}ņ.ց?.
ә($"Dpath lock: {ň}");foreach(var Ì in ġ)ņ.ց?.ә($"{Ì} awaits docking");foreach(var Ì in Ģ)ņ.ց?.ә($"{Ì} awaits departure")
;if(Ģ.Count>0&&ň==null){var Q=Ģ.Peek();var ť=ņ.ط.FirstOrDefault(N=>N.CustomData==Q);if(ť!=null){Ģ.Dequeue();if(Ň.Any(Ç=>Ç
.ł)){var Ţ=MatrixD.Invert(ť.WorldMatrix);ş=ė(ť,ğ[Q]).Select(Ç=>Vector3D.Transform(Ç.Ń,ņ.î.WorldMatrix*Ţ)).ToList();ņ.ռ?.ә
($"Sent {ş.Count}-node departure path");ň=Q;}else ş=new List<Vector3D>{Vector3D.Forward*20};ţ(Q,true);}}string Ŧ=null;if(
ġ.Count>0&&ň==null)Ŧ=ġ.Peek();bool ŧ=false;foreach(var N in ņ.ط){var Ċ=Ŋ[N];var Ũ=N.CustomData;if(string.IsNullOrEmpty(Ũ)
||(Ũ==Ŧ)){if((Ŧ!=null)&&(N.Status==MyShipConnectorStatus.Unconnected))ŧ=true;}else{var Ä=Œ;if(ņ.د.ج.ContainsKey(Ũ)){var F=
ņ.د.ج[Ũ];if(F.ƃ.Ѵ?.ǜ=="su-do")Ä=F.ƃ.Ѵ;}var Ú=N.WorldMatrix;var C=Ú.Translation+Ú.Forward*Ŋ[N].Ŏ;if(Ċ.Ō!=Vector3D.Zero){
var Ç=N.GetPosition()-Ċ.Ō;Ä.Ĵ=Ç/ʅ.ʆ;}Ċ.Ō=N.GetPosition();Ä.Α(C);Ä.ͽ=ņ.ב.Me;Ä.Ή=Ú;long Q;if(Ä==Œ&&long.TryParse(N.CustomData
,out Q)){ņ.ց?.ә($"Channeling DV to {Q}");ņ.ב.SendUnicastMessage(Q,Ӟ,Ä.И());}}}if(ŧ){var Q=ġ.Dequeue();var Ę=ğ[Q];var ę=
Vector3D.Rotate(Ę-ņ.î.WorldMatrix.Translation,MatrixD.Transpose(ņ.î.WorldMatrix));var Š=Ň.Where(Ç=>Ç.ł).OrderBy(Ç=>(Ç.Ń-ę).
LengthSquared()).FirstOrDefault();if(Š!=null)Ę=Vector3D.Transform(Š.Ń,ņ.î.WorldMatrix);var š=ņ.ط.Where(N=>(string.IsNullOrEmpty(N.
CustomData)||(N.CustomData==Ŧ))&&(N.Status==MyShipConnectorStatus.Unconnected)&&(Ŋ[N].ō==null||(Ġ[Q]!=""&&Ŋ[N].ō.Contains(Ġ[Q]))))
.OrderBy(N=>Vector3D.DistanceSquared(Ę,N.GetPosition())).First();š.CustomData=Ŧ;ņ.ռ?.ә(
$"{Q} assigned to connector {š.CustomName}");try{if(Š!=null){var Ţ=MatrixD.Invert(š.WorldMatrix);ş=Ħ(Š,š).Select(Ç=>Vector3D.Transform(Ç.Ń,ņ.î.WorldMatrix*Ţ)).
ToList();ņ.ռ?.ә($"Sent {ş.Count}-node approach path");ň=Q;}else ş=new List<Vector3D>{Vector3D.Forward*20};ţ(Q);}catch(
Exception ex){ʅ.ʊ($"{ex}");throw;}}}void ţ(string Q,bool É=false){if(ņ.د.ج.ContainsKey(Q)){var F=ņ.د.ج[Q];ş.Reverse();F.ƃ.ѫ(new ڶ
(F,ş,É));}else ņ.ђ.K($"apck.dpath.exec.{(É?"depart":"docking")}",ş.ToImmutableArray(),long.Parse(Q));}public void ń(){if(
ň!=null){ľ(ň);}}public void ľ(string Q){ņ.ռ?.ә($"{Q} RouteComplete");if(ň==Q)ň=null;ņ.ط.First(Ç=>Ç.CustomData==Q).
CustomData="";}public void Ğ(string Q,Vector3D N,string B,bool É=false){ņ.ռ?.ә($"{Q} requests {(É?"depart":"docking")}");if(É){if(
!Ģ.Contains(Q))Ģ.Enqueue(Q);}else{if(!ġ.Contains(Q))ġ.Enqueue(Q);}ğ[Q]=N;Ġ[Q]=B;}Dictionary<string,Vector3D>ğ=new
Dictionary<string,Vector3D>();Dictionary<string,string>Ġ=new Dictionary<string,string>();Queue<string>ġ=new Queue<string>();Queue<
string>Ģ=new Queue<string>();IEnumerable<Ł>ģ(Dictionary<Ł,Ł>ĝ,Ł Ĥ){yield return Ĥ;var Ô=ĝ[Ĥ];while(Ô!=null){yield return Ô;Ô=ĝ
[Ô];}}List<Ł>Ħ(Ł ĥ,IMyShipConnector N){var Ě=Vector3D.Rotate(N.GetPosition()-ņ.î.WorldMatrix.Translation,MatrixD.
Transpose(ņ.î.WorldMatrix));var Ė=Ň.OrderBy(Ç=>(Ç.Ń-Ě).LengthSquared()).FirstOrDefault();return Ĝ(ĥ,Ė);}List<Ł>ė(IMyShipConnector
N,Vector3D Ę){var ę=Vector3D.Rotate(N.GetPosition()-ņ.î.WorldMatrix.Translation,MatrixD.Transpose(ņ.î.WorldMatrix));var Ě
=Vector3D.Rotate(Ę-ņ.î.WorldMatrix.Translation,MatrixD.Transpose(ņ.î.WorldMatrix));var ě=Ň.OrderBy(Ç=>(Ç.Ń-ę).
LengthSquared()).FirstOrDefault();var Ė=Ň.Where(Ç=>Ç.ł).OrderBy(Ç=>(Ç.Ń-Ě).LengthSquared()).FirstOrDefault();return Ĝ(ě,Ė);}List<Ł>Ĝ(
Ł ě,Ł Ė){var ĕ=new HashSet<Ł>();ĕ.Add(ě);var ĝ=new Dictionary<Ł,Ł>();var ħ=new Dictionary<Ł,double>();var Ĺ=new
Dictionary<Ł,double>();foreach(var Ĥ in Ň){Ĺ.Add(Ĥ,double.MaxValue);ħ.Add(Ĥ,double.MaxValue);ĝ.Add(Ĥ,null);}ħ[ě]=0;Ĺ[ě]=0;while(ĕ.
Count>0){var ĺ=ĕ.OrderBy(Ç=>Ĺ[Ç]).FirstOrDefault();if(ĺ==Ė){return ģ(ĝ,ĺ).ToList();}ĕ.Remove(ĺ);foreach(var Ĥ in ĺ.ĸ){var Ļ=ħ
[ĺ]+Ĥ.Item2;if(Ļ<ħ[Ĥ.Item1]){ĝ[Ĥ.Item1]=ĺ;ħ[Ĥ.Item1]=Ļ;Ĺ[Ĥ.Item1]=Ļ;if(!ĕ.Contains(Ĥ.Item1)){ĕ.Add(Ĥ.Item1);}}}}ʅ.ʊ(
$"aStar Failure");return null;}public void ļ(MatrixD Ľ,bool ĥ=false){try{var Ĥ=new Ł(){Ń=Vector3D.Rotate(Ľ.Translation-ņ.î.GetPosition()
,MatrixD.Transpose(ņ.î.WorldMatrix)),ł=ĥ};var Ŀ=Ň.Where(Ç=>Ç.ł).OrderBy(Ç=>(Ç.Ń-Ĥ.Ń).LengthSquared()).FirstOrDefault()??Ň
.OrderBy(Ç=>(Ç.Ń-Ĥ.Ń).LengthSquared()).FirstOrDefault();if(Ŀ!=null){Ŀ.ĸ.Add(new MyTuple<Ł,double>(Ĥ,(Ĥ.Ń-Ŀ.Ń).Length()));
Ĥ.ĸ.Add(new MyTuple<Ł,double>(Ŀ,(Ĥ.Ń-Ŀ.Ń).Length()));}Ň.Add(Ĥ);}catch(Exception ex){ʅ.ʊ($"{ex}");}}public void ŀ(){Ň.
Clear();}}class Ł{public bool ł;public Vector3D Ń;public HashSet<MyTuple<Ł,double>>ĸ=new HashSet<MyTuple<Ł,double>>();}class
Ĩ{public PS Ķ;public TD ĩ;public int Ī;public bool ī=>Ī==1&&ĩ==TD.None;public void Ĭ(PS ĭ){if(ĭ==PS.WP)Į();else if(ĭ==PS.
Inert)į(false);else if(ĭ==PS.Disabled||ĭ==PS.Aim)į();Ķ=ĭ;}void Į(){β.DampenersOverride=false;}void į(bool İ=true){J.î.
GyroOverride=false;J.ƺ.Ͼ();β.DampenersOverride=İ;}public Ĩ(خ ı){β=ı.ה;γ=ı.ב;J=ı;var Ĳ=ı.ƺ.Ͽ(1);J.ռ?.ә(
$"Total thrust force BB: {Ĳ}, vol: {Ĳ.Volume}");if(Ĳ.Volume>0)Ī=3;else if(Ĳ.Max.Z>0){Ī=1;J.ر.Ʀ.ForEach(Ç=>Ç.SetValueBool("ControlGyros",false));}J.ռ?.ә(
$"ThrustDim: {Ī}");}public double ĳ{get;private set;}public Vector3D Ĵ=>β.GetShipVelocities().LinearVelocity;Vector3D?ĵ;public double î;
public Vector3D ķ;public Vector3D?Ȣ{get{if(ĵ!=null)return ĵ;var ȳ=β.GetNaturalGravity();if(ȳ!=Vector3D.Zero){î=ȳ.Length();ķ=ȳ/
î;ĵ=ȳ;}else J.ŵ=null;return ĵ;}}public double?ί{get{double ſ;if(β.TryGetPlanetElevation(MyPlanetElevation.Surface,out ſ))
return ſ;return null;}}Vector3D ΰ{get;set;}public Vector3D α{get;set;}public IMyRemoteControl β;
IMyIntergridCommunicationSystem γ;خ J;int δ;double ε;public Vector3D ζ;public Vector3D η;bool θ;public Vector3D ι;public void κ(ػ Ŭ){var μ=ʅ.ƅ-δ;δ=ʅ.ƅ;
if(μ>0)ε=μ/60f;else return;α=(Ĵ-ΰ)/ε;ΰ=Ĵ;η=Vector3D.Zero;θ=false;switch(Ķ){case PS.Disabled:return;case PS.Aim:case PS.WP:
try{var λ=Ŭ;var ȶ=Vector3D.Zero;if(Ķ==PS.WP){if(λ.ى.HasValue){var ή=λ.ـ?.Invoke()??J.ɷ;bool Υ=!J.M.ӥ(
"suppress-gyro-control")&&J.ذ.ӱ<bool>("hold-thrust-on-rotation");if((Υ&&(ĳ>0.2))||J.M.ӥ("suppress-transition-control")){ά(ή,ή,null,null,false);
}else{ά(ή,λ.ى.Value,λ.و?.Ĵ,λ.ف,λ.ك);}}else{var C=J.ɷ;var ˇ=J.M.ӥ("damp-when-idle")?(float?)0:null;if(ī)ȶ=-ˣ(Vector3D.Zero
,ˇ,false,Vector3D.Zero);else ά(C,C,null,ˇ,false);}}if(ĩ==TD.Rover)λ.ζ=null;if(λ.ζ.HasValue){ζ=λ.ζ.Value;ȶ=ζ-(λ.غ??J.և).
Translation;}if(ȶ!=Vector3D.Zero){Φ(ȶ,λ.ٮ,λ.غ??J.և,!ī&&!λ.ل);if(ī)J.ƺ.ɒ.ɫ(ϊ);}if(θ!=J.î.GyroOverride){J.î.GyroOverride=θ;}}catch(
Exception ex){if(J.ֆ!=null)J.ֆ.CustomName+="HC Exception! See PB screen log";var ſ=$"HC EPIC FAIL\nBehavior:{Ŭ.ǜ}\n{ex}";ʅ.ʉ(ſ);Ĭ
(PS.Disabled);throw;}break;}ϊ=0;ĵ=null;ι=Vector3D.Zero;}void Φ(Vector3D ȶ,Vector3D Χ,MatrixD Ψ,bool Ω){var Ϊ=Vector3D.
Zero;var Ţ=MatrixD.Transpose(J.և);var Ȭ=Vector3D.Rotate(β.GetShipVelocities().AngularVelocity,Ţ);var Ϋ=Vector3D.Zero;if(ȶ!=
Vector3D.Zero){if(Ω)Ϋ=Vector3D.Rotate(φ(ȶ),Ţ);ȶ=ȶ.Normalized();Ϊ=к.ϱ(ȶ,Ψ,J.և,Χ,ĳ>0.87?1:J.ذ.ӱ<float>("roll-power-factor"));ĳ=
Vector3D.Dot(ȶ,Ψ.Forward);}θ=!J.M.ӥ("suppress-gyro-control");if(θ){if(ī)Ϊ.Z=-J.ر.ɇ?.RotationIndicator.Y??Ϊ.Z;к.Ɍ(J.î,Ϊ,Ȭ,Ϋ);}}
void ά(Vector3D έ,Vector3D Τ,Vector3D?ȑ,float?ν,bool ό){if(Ķ!=PS.WP)return;var ˮ=(ȑ??Vector3D.Zero)*J.ذ.Ӷ;var ύ=false;if(ĩ!=
TD.None){if(!γ.IsEndpointReachable(J.ח?.EntityId??-1))J.ց?.ә("TP error");else{J.ց?.ә("Thrust delegation");ύ=(ĩ==TD.Vtol)&&
(Ī==3);var ώ=ν??-1;if(ύ&&(Ĵ.LengthSquared()<1))ώ=-1;J.ב.SendUnicastMessage(J.ח.EntityId,"thrust",new MyTuple<Vector3D,
Vector3D,Vector3D,bool,float>(έ,Τ,ˮ,ό,ώ));if(!ύ)return;}}if(Ī<3)return;var ɍ=β.CalculateShipMass().PhysicalMass;if(ɍ==0)return;
var Ϗ=J.ƺ.Ͽ(ɍ);var ˁ=J.և;ˁ.Translation=έ;var ȓ=Τ-ˁ.Translation;var ϑ=MatrixD.Transpose(ˁ);var ϙ=Vector3D.Rotate(Ĵ,ϑ);var ϒ=
ϙ;var ϓ=Vector3D.Zero;if(Ȣ!=null){ϓ=Vector3D.Rotate(Ȣ.Value,ϑ);if(!ύ)Ϗ+=-ϓ;}var ϔ=Vector3D.Zero;var ϕ=Vector3D.Zero;var ϖ
=Vector3D.Zero;var ϗ=new Vector3D();var Ϙ=J.ر.Ɂ(ref ˁ);if(ύ){var Ϛ=Vector3D.Rotate(ι,ϑ)/ɍ;J.ց?.ә(
$"localExtA: {Ϛ.ToString("f2")}");ϗ-=Ϛ;}var ϋ=ȓ.Length();if(ϋ>double.Epsilon){var ξ=Vector3D.Rotate(ȓ,ϑ);var ω=ξ.Normalized();ϔ=Vector3D.Reject(ϙ,ω);if(
ˮ!=Vector3D.Zero){ϒ-=Vector3D.Rotate(ˮ,ϑ);ϔ=Vector3D.Reject(ϒ,ω);}else{ϒ-=ϔ;}var ο=Vector3D.Dot(ϒ,ω);var ΐ=new RayD(
Vector3D.Zero,ω);double π,ρ;if(!Ϗ.Intersect(ref ΐ,out π,out ρ)){ʅ.ʉ(
$"Not enough thrust to compensate for gravity - zero is outide of acc BB. Mass: {ɍ}");return;}ϖ=ΐ.Direction*π;ϕ=ΐ.Direction*ρ;var ς=ϕ.Length();double ͷ=ν??J.ذ.Ӵ;if(!ό)ͷ=Math.Min(ͷ,Math.Sqrt(2*ς*0.92*ϋ));
var ː=ͷ-ο;if(ː>0){ϗ=ː<10?ϖ*ː/10:ϖ;}else ϗ=ϕ;}else if(ν==0){var σ=Ϗ.Extents.Max();ϔ=Ϙ!=Vector3D.Zero?Vector3D.Reject(ϙ,Ϙ):ϙ;
var τ=ϔ.Length();if(τ>σ*ε)ϔ=ϔ/τ*σ;}if(ϔ.IsValid())ϗ+=ϔ;if(Ȣ!=null)ϗ+=ϓ;if(!J.Ʊ)ϗ-=Ϙ*1000;try{J.ƺ.Ɍ(ϗ,ɍ);}catch{ʅ.ʉ(
"SetOverride failure");ʅ.ʉ($"reject{ϔ}");ʅ.ʉ($"Dt{ε:f5}");ʅ.ʉ($"reject/Dt{ϔ/ε}");ʅ.ʉ($"reversePoint{ϕ}");ʅ.ʉ($"point{ϖ}");ʅ.ʉ(
$"overrideVector: {ϗ}, mass: {ɍ}, accCap: {Ϗ}");throw;}}Vector3D?υ;Vector3D φ(Vector3D ȶ){var Ċ=Vector3D.Zero;if(υ.HasValue){Ċ=Vector3D.Cross(ȶ,ȶ-υ.Value)/ȶ.
LengthSquared()/ε;}υ=ȶ;return Ċ;}public Vector3D χ(double ψ){return J.ɷ+J.և.Forward*ψ;}double ϊ;enum ϐ{Σ,Ι,ˠ,ˡ,ˢ}public Vector3D ˣ(
Vector3D ȓ,float?ˤ,bool ˬ,Vector3D ˮ){var Ͱ=ϐ.Ι;var ͱ=Ĵ;var Ơ=J.ƺ;var Ͳ=Ơ.ɒ.Ϲ()/J.ה.CalculateShipMass().PhysicalMass;var ͳ=Ͳ;var
ʹ=Vector3D.Zero;var Ͷ=Vector3D.Zero;var ȕ=ͱ;var ͷ=ˤ??J.ذ.Ӵ;var ˑ=new BoundingSphereD(Vector3D.Zero,Ͳ);var ˏ=Vector3D.Zero
;var ˀ=J.ر.ɇ?.MoveIndicator;if(ˀ==Vector3.Zero)ˀ=null;if(ͷ==0)Ͱ=ϐ.Σ;if(Ȣ!=null){var Ȍ=Ȣ.Value;ʹ=Ȍ;ˑ.Center+=Ȍ;if(Ͳ>J.ו.î)
ͳ=(float)Math.Sqrt(Math.Max(0,Ͳ*Ͳ-J.ו.î*J.ו.î));if(ˀ.HasValue){Ͱ=ϐ.Ι;var ˁ=MatrixD.CreateFromDir(к.л(J.և.Left,ķ),-ķ);var
ˆ=ˀ.Value*100f;ˆ.Y=-Math.Sign(ˆ.Y)*(float)J.ו.î*0.99f;ˏ+=-Vector3D.Rotate(ˆ,ˁ);}}if(!ˀ.HasValue&&ȓ!=Vector3D.Zero){Ͱ=ϐ.Ι;
var N=ȓ.Length();var ȗ=ȓ/N;ȕ=ͱ-ˮ*J.ذ.Ӷ;if(ȕ==Vector3D.Zero)ˏ=ȗ;else{var ˇ=Math.Max(Vector3D.Dot(ȕ,ȗ),0);var ˈ=Vector3D.
Reject(ȕ,ȗ);var ˉ=ˈ.Length();var ˊ=ˈ/ˉ;Ͱ=ϐ.ˠ;if(!ˬ){var ˋ=N;var ˌ=Ȣ.HasValue?î*0.99:9f;var ˍ=ˌ*0.5;var ˎ=Math.Sqrt(2*ˍ*ˋ);ͷ=(
float)Math.Min(ͷ,ˎ);if(Ȣ!=null){if(N<20)ͷ/=2;}else{if(N<20)ͷ/=4;else if(N<80)ͷ/=2;}var í=ȗ*ͷ-ȕ;if(ˇ<=0)í=ȗ*ͷ-ˈ;ˏ=í;var ʿ=
Vector3D.Reject(í,ȗ);var ː=í-ʿ;ˏ=ː+ʿ/2;Vector3D.ClampToSphere(ref ˏ,ˌ);if(N<1){Ͱ=ϐ.Σ;ˏ=Vector3D.Zero;}}else{var ͺ=Math.Min(1,ˉ/
50)*0.5f;var Β=(ͷ-ˇ)/ͷ;ˏ=(-ˊ*ͺ+ȗ*Β)*Ͳ;}}}if((Ͱ==ϐ.Σ)&&ȕ!=Vector3D.Zero){if(Ȣ==null){ˏ=-ȕ;}else{var ˇ=ȕ.Length();var Γ=Math
.Min(ͳ,ˇ);var Δ=ȕ/ˇ;var Ε=Vector3D.Reject(Δ,ķ);var Ζ=Δ-Ε;var Η=Vector3D.Dot(ȕ,-ķ);var Θ=(Η>0&&Η<J.ו.î*3)?Math.Min(J.ו.î,ˇ
):Γ;ˏ+=-Ε*(ˇ>8?Γ:Γ/2);ˏ+=-Ζ*Θ;}}var Κ=Vector3D.Zero;if(ˏ!=Vector3D.Zero){Κ=-Ρ(ˏ,ˑ);}if(Ȣ.HasValue){Κ+=Ȣ.Value;var Ε=
Vector3D.ProjectOnPlane(ref Κ,ref ķ);ʹ=Κ-Ε;}bool Λ=true;var Μ=î/Ͳ;var Ν=Vector3D.Dot(J.î.WorldMatrix.Backward,ķ);if(Ν<Μ)Λ=false;
var Ξ=Κ.Normalized();ϊ=0;if(Κ!=Vector3D.Zero){var ƶ=ˏ.Normalized();if(Ȣ!=null&&Λ){var Ο=ʹ.Length();var Π=ʹ/Ο;if(Vector3D.
Dot(J.î.WorldMatrix.Backward,Π)>0)ϊ=Ο/Vector3D.Dot(J.î.WorldMatrix.Backward,Π)/Ͳ;}else{if(ˬ||Vector3D.Dot(J.î.WorldMatrix.
Backward,-ƶ)>0.95)ϊ=Math.Max(0,Vector3D.Dot(J.î.WorldMatrix.Backward,Κ))/Ͳ;}}return Κ==Vector3D.Zero?Κ:Ξ;}Vector3D Ρ(Vector3D ˏ,
BoundingSphereD ˑ){if(ˑ.Contains(Vector3D.Zero)==ContainmentType.Contains){var ΐ=new RayD(ˏ,-ˏ.Normalized());if(ˑ.Contains(ˏ)==
ContainmentType.Disjoint){var N=ΐ.Intersects(ˑ);J.ց?.ә($"d {N}");var Ȝ=ΐ.Position+ΐ.Direction*N.Value;return Ȝ;}return ˏ;}return
Vector3D.Zero;}}class ͻ{public long ͼ;public string ǜ;public long ͽ;public Vector3D?Ά{get;private set;}public Vector3D?Ĵ;public
Vector3D?Έ;public MatrixD?Ή;public BoundingBoxD?Ί;public int?Ό;public MyDetectedEntityType?Ύ{get;set;}public long Ώ;public ͻ(
string ï){ǜ=ï;}public void Α(Vector3D Ȥ){Ά=Ȥ;ͼ=ʅ.ƅ;}public bool Й=>Ά.HasValue&&!К;public bool К=>Ό.HasValue&&(ʅ.ƅ-ͼ>Ό);public
enum Л:byte{М=1,Н=2,О=4}bool П(Л Р,Л Т){return(Р&Т)==Т;}public void Ш(long У,MyTuple<MyTuple<string,long,byte,byte>,Vector3D
,Vector3D,MatrixD,BoundingBoxD>Ф,int Х){var Ц=Ф.Item1;ǜ=Ц.Item1;ͽ=Ц.Item2;Ύ=(MyDetectedEntityType)Ц.Item3;Л Ч=(Л)Ц.Item4;
var μ=Х-ͼ;var C=Ф.Item2;if(П(Ч,Л.М)){var Щ=Ф.Item3;if(!Ĵ.HasValue)Ĵ=Щ;if(μ>0)Έ=(Щ-Ĵ.Value)*60/μ;Ĵ=Щ;C+=Щ*ʅ.ʆ;}Α(C);if(П(Ч,Л
.Н))Ή=Ф.Item4;if(П(Ч,Л.О))Ί=Ф.Item5;Ώ=У;Ж.С++;}public MyTuple<MyTuple<string,long,byte,byte>,Vector3D,Vector3D,MatrixD,
BoundingBoxD>И(){var Б=0|(Ĵ.HasValue?1:0)|(Ή.HasValue?2:0)|(Ί.HasValue?4:0);var Ç=new MyTuple<MyTuple<string,long,byte,byte>,
Vector3D,Vector3D,MatrixD,BoundingBoxD>(new MyTuple<string,long,byte,byte>(ǜ,ͽ,(byte)MyDetectedEntityType.LargeGrid,(byte)Б),Ά.
Value,Ĵ??Vector3D.Zero,Ή??MatrixD.Identity,Ί??new BoundingBoxD());return Ç;}}static IEnumerable<string>В(IMyTerminalBlock Ą,
string Ì=null){return(Ì??Ą.CustomName).Trim().Split('[').Select(C=>C.Trim(']'));}static Dictionary<string,string>Г(
IMyTerminalBlock Ą,string Ì=null){return В(Ą,Ì).Where(Ç=>Ç.Contains('=')).ToDictionary(Ç=>Ç.Split('=')[0],Ç=>Ç.Split('=')[1]);}static
string Д(params Vector3D[]Е){return string.Join(":",Е.Select(ȳ=>$"{ȳ.X}:{ȳ.Y}:{ȳ.Z}"));}static З Ж;struct З{public int С;
public int Ъ;public int р;}static class к{public static Vector3D л(Vector3D м,Vector3D н){return Math.Abs(м.Dot(н))>0.999?
Vector3D.CalculatePerpendicularVector(н):Vector3D.Cross(м,н);}public static bool о(Vector3D п,Vector3D C,BoundingSphereD а,ref
Vector3D с){var Ô=а.Center;if(а.Contains(C)==ContainmentType.Contains){с=C+(C-Ô).Normalized()*а.Radius*1.1;return false;}var т=
new RayD(п,(C-п).Normalized());if(т.Intersects(а).HasValue){var у=п==Ô?Vector3D.CalculatePerpendicularVector(т.Direction)-Ô
:п-Ô;var Ş=у.Length();var ф=л(т.Direction,у/Ş).Normalized();var х=Vector3D.Cross(-т.Direction,ф).Normalized();с=Ô+х*а.
Radius;return true;}с=п;return false;}static Vector3D ц(Vector3D Ç,Vector3D Я,BoundingSphereD й,double Ы){var з=(й.Center-Я).
Length();var Ĥ=(й.Center-Я)/з;var ȶ=Ç-Я;var Ь=Vector3D.ProjectOnPlane(ref ȶ,ref Ĥ);var Э=з-й.Radius;if(Э<Ы)return Я-Ĥ*Ы+Ь.
Normalized()*500;return Я+Ь;}public static Vector3D Ю(Vector3D Ç,Vector3D Я,BoundingSphereD а){var б=(а.Center-Я).Normalized();if(
а.Contains(Я)==ContainmentType.Contains)return а.Center-б*а.Radius*1.1;if(а.Contains(Ç)==ContainmentType.Contains)Ç=а.
Center+(Ç-а.Center).Normalized()*а.Radius*1.1;var в=Ç;var г=Ç;var д=(Ç-Я).Normalized();RayD ǘ;if(Vector3D.Dot(д,б)>0)ǘ=new
RayD(Ç,-д);else ǘ=new RayD(Я,д);if(ǘ.Intersects(а).HasValue){if((Я-а.Center).Length()>а.Radius*1.2){о(Я,Ç,а,ref в);return в;
}о(Ç,Я,а,ref г);return ц(г,Я,а,а.Radius+10);}return Ç;}public static Vector3D е(Vector3D ж,Vector3D и,Vector3D А,Vector3D
ϩ,double Ї,ref double Ȕ){var Ϫ=Vector3D.Distance(ж,А);var ȗ=(А-ж).Normalized();var ϫ=А;ϩ-=и;var Ϭ=ϩ.Length();if(Ϭ>float.
Epsilon){var ϭ=ϩ/Ϭ;var Ϯ=Math.PI-Math.Acos(Vector3D.Dot(ȗ,ϭ));var ϯ=Ϭ*Math.Sin(Ϯ)/Ї;if(Math.Abs(ϯ)<=1){var ϰ=Math.Asin(ϯ);var Ì
=Ϫ*Math.Sin(ϰ)/Math.Sin(Ϯ+ϰ);Ȕ=Ì/Ϭ;ϫ=А+ϭ*Ì;}else{Ȕ=-1;}}else{Ȕ=Ϫ/Ї;}return ϫ;}public static Vector3D ϱ(Vector3D ϲ,MatrixD
ϳ,MatrixD ϴ,Vector3D ϵ,float Ϸ){var Χ=ϳ.Up;var Ϩ=Vector3D.ProjectOnPlane(ref ϲ,ref Χ);var Ϧ=-(float)Math.Atan2(Vector3D.
Dot(Vector3D.Cross(ϳ.Forward,Ϩ),Χ),Vector3D.Dot(ϳ.Forward,Ϩ));Χ=ϳ.Right;Ϩ=Vector3D.ProjectOnPlane(ref ϲ,ref Χ);var Ϝ=-(
float)Math.Atan2(Vector3D.Dot(Vector3D.Cross(ϳ.Forward,Ϩ),Χ),Vector3D.Dot(ϳ.Forward,Ϩ));float ϝ=0;if(ϵ!=Vector3D.Zero){Χ=ϳ.
Forward;Ϩ=Vector3D.ProjectOnPlane(ref ϵ,ref Χ);Ϩ=Ϩ.Normalized();ϝ=(float)Math.Atan2(Vector3D.Dot(Vector3D.Cross(ϳ.Up,Ϩ),Χ),
Vector3D.Dot(ϳ.Up,Ϩ));}var Ϊ=new Vector3D(Ϝ,Ϧ,ϝ*Ϸ);var Ϟ=Vector3D.Rotate(Ϊ,ϳ);var Ş=Vector3D.Rotate(Ϟ,MatrixD.Transpose(ϴ));
return Ş;}public static void Ɍ(IMyGyro ϟ,Vector3D Ϡ,Vector3D ϡ,Vector3D Ϋ){var Ϣ=Ϡ.Y;var ϣ=Ϡ.X;var Ϥ=Ϡ.Z;var ϥ=ϟ.CubeGrid.
GridSizeEnum==MyCubeSize.Large?30:60;var ʏ=GRID_ANGULAR_ACCELERATIONS;var ϛ=0.1047f;Func<double,double,double,double,double>ϧ=(Ç,Ђ,Ѓ
,Ş)=>{var Є=Ç<0?Ђ-Ѓ:Ѓ-Ђ;Є=Math.Max(Є,0);var σ=Math.Abs(Ç);double ǘ;if(σ>Є*Є/(2*Ş))ǘ=ϥ*Math.Sign(Ç)*Math.Max(Math.Min(σ,1)
,0.0002);else{ǘ=-ϥ*Math.Sign(Ç)*Math.Max(Math.Min(σ,1),0.0002);}return ǘ-Ѓ/ϛ;};var Ѕ=(float)ϧ(Ϣ,ϡ.Y,Ϋ.Y,ʏ.Y);var І=(float
)ϧ(ϣ,ϡ.X,Ϋ.X,ʏ.X);var Ј=(float)ϧ(Ϥ,ϡ.Z,Ϋ.Z,ʏ.Z);ϟ.SetValue("Pitch",-І);ϟ.SetValue("Yaw",Ѕ);ϟ.SetValue("Roll",Ј);}}class Џ
{public class Љ{List<ɩ>Њ;public bool Ћ;public bool Ќ;private Џ Ѝ;public Љ(Џ Ў,List<ɩ>ɕ){Ѝ=Ў;Њ=ɕ;}public void Ё(List<ɩ>ɕ){
Њ.AddRange(ɕ);}public void Ů(){Њ.ForEach(Ş=>Ş.Ů());}public void ɫ(double ʌ){Ќ=false;Ћ=false;ʌ=Math.Min(1,Math.Abs(ʌ))*
Math.Sign(ʌ);if(Ѝ.ɘ.Ʊ||!Ѝ.ɘ.M.ӥ("thr-opt")){foreach(var ϸ in Њ){ϸ.ɫ(ʌ);ϸ.ɖ(ref Ћ,ref Ќ);}}else{for(int X=0;X<Њ.Count;X++){if
(X%5==ʅ.ƅ%5){Њ[X].ɫ(ʌ);Њ[X].ɖ(ref Ћ,ref Ќ);}}}}public float Ϲ(){float Ϻ=0;foreach(var ϸ in Њ)Ϻ+=ϸ.ɛ();if(Ѝ.ϼ&&(Ϻ==0||
double.IsNaN(Ϻ)))Ϻ=1000000;return Ϻ;}}double[]ϻ=new double[6];bool ϼ;bool Ͻ;public void Ͼ(){if(!Ͻ){foreach(var ƣ in ɓ)ƣ.Ů();Ͻ=
true;}}public BoundingBoxD Ͽ(float ɍ){Vector3D Ѐ=new Vector3D(-ϻ[5],-ϻ[2],-ϻ[1])/ɍ;Vector3D ʾ=new Vector3D(ϻ[4],ϻ[3],ϻ[0])/ɍ
;return new BoundingBoxD(Ѐ,ʾ);}public void ɑ(){for(int X=0;X<6;X++){ϻ[X]=ɓ[X].Ϲ();}}public Љ ɒ=>ɓ[0];Љ[]ɓ=new Љ[6];Љ ɔ(
List<ɩ>ɕ){return new Љ(this,ɕ);}public void ɖ(ref bool ǃ,ref bool Ɵ){foreach(var ɗ in ɓ){ǃ|=ɗ.Ћ;Ɵ|=ɗ.Ќ;}}خ ɘ;public Џ(خ ə){ɘ
=ə;var Ȫ=ə.î.WorldMatrix;Func<Vector3D,List<ɩ>>ɚ=D=>{var ǘ=ɘ.ƹ.Where(Ą=>Ą is IMyThrust&&D==Ą.WorldMatrix.Forward).Select(
Ç=>Ç as IMyThrust).ToList();return ǘ.Select(Ä=>new ɥ(Ä)).Cast<ɩ>().ToList();};ɓ[0]=ɔ(ɚ(Ȫ.Backward));ɓ[1]=ɔ(ɚ(Ȫ.Forward));
ɓ[2]=ɔ(ɚ(Ȫ.Down));ɓ[3]=ɔ(ɚ(Ȫ.Up));ɓ[4]=ɔ(ɚ(Ȫ.Right));ɓ[5]=ɔ(ɚ(Ȫ.Left));var ɐ=ɘ.ƹ.Where(Ą=>Ą is IMyArtificialMassBlock).
Cast<IMyArtificialMassBlock>().ToList();var Ɋ=ɘ.ƹ.Where(Ą=>Ą is IMyGravityGenerator).Cast<IMyGravityGenerator>().ToList();
Func<Vector3D,bool,List<ɩ>>ɋ=(D,Ţ)=>{var õ=Ɋ.Where(Ą=>D==Ą.WorldMatrix.Up);return õ.Select(C=>new Ɏ(C,ɐ,Ţ)).Cast<ɩ>().ToList
();};ɓ[0].Ё(ɋ(Ȫ.Forward,true));ɓ[1].Ё(ɋ(Ȫ.Forward,false));ɓ[0].Ё(ɋ(Ȫ.Backward,false));ɓ[1].Ё(ɋ(Ȫ.Backward,true));ɓ[2].Ё(ɋ
(Ȫ.Up,true));ɓ[3].Ё(ɋ(Ȫ.Up,false));ɓ[2].Ё(ɋ(Ȫ.Down,false));ɓ[3].Ё(ɋ(Ȫ.Down,true));ɓ[5].Ё(ɋ(Ȫ.Right,true));ɓ[4].Ё(ɋ(Ȫ.
Right,false));ɓ[5].Ё(ɋ(Ȫ.Left,false));ɓ[4].Ё(ɋ(Ȫ.Left,true));ɑ();ϼ=true;}public void Ɍ(Vector3D ȳ,float ɍ){Ͻ=false;ɓ[1].ɫ(-ȳ.
Z/ϻ[1]*ɍ);ɓ[0].ɫ(ȳ.Z/ϻ[0]*ɍ);ɓ[2].ɫ(-ȳ.Y/ϻ[2]*ɍ);ɓ[3].ɫ(ȳ.Y/ϻ[3]*ɍ);ɓ[5].ɫ(-ȳ.X/ϻ[5]*ɍ);ɓ[4].ɫ(ȳ.X/ϻ[4]*ɍ);}}class Ɏ:ɩ{
IMyGravityGenerator õ;List<IMyArtificialMassBlock>ɏ;bool ɉ;public Ɏ(IMyGravityGenerator õ,List<IMyArtificialMassBlock>ɏ,bool ɉ){this.õ=õ;
this.ɏ=ɏ;this.ɉ=ɉ;}public void ɫ(double ɤ){if(ɤ>=0)õ.GravityAcceleration=(float)(ɉ?-ɤ:ɤ)*G;}public void Ů(){õ.
GravityAcceleration=0;}public float ɛ(){return ɏ.Count*50000*G;}public void ɖ(ref bool ɜ,ref bool ɝ){ɜ|=!õ.IsFunctional||õ.GetPosition().
IsZero();}}class ɥ:ɩ{IMyThrust Ä;public ɥ(IMyThrust Ä){this.Ä=Ä;ɨ=Ä.MaxEffectiveThrust;}double ɪ;public void ɫ(double ɤ){if(ɤ
<=0&&Ä.ThrustOverride>=0.001)Ä.ThrustOverride=0.00000001f;else if(Math.Abs(ɤ-ɪ)>0.0000001)Ä.ThrustOverride=(float)ɤ*Ä.
MaxThrust;ɪ=ɤ;}public void Ů(){Ä.ThrustOverride=0;ɪ=0;Ä.Enabled=true;}public float ɛ(){var ɧ=Ä.MaxEffectiveThrust;ɨ=ɧ;return ɧ;}
float ɨ;public void ɖ(ref bool ɜ,ref bool ɝ){ɜ|=!Ä.IsFunctional||Ä.GetPosition().IsZero();var ɧ=Ä.MaxEffectiveThrust;if(Math.
Abs(ɨ-ɧ)>0.0001){ɝ=true;ɨ=ɧ;}}}interface ɩ{void ɫ(double ɤ);float ɛ();void Ů();void ɖ(ref bool ɜ,ref bool ɝ);}ɟ ɞ;class ɟ{
public long ɠ;public ڦ ɡ;public HashSet<long>ɢ=new HashSet<long>();خ ņ;int Ì;int ſ;int ǘ;Dictionary<long,MyTuple<int,Vector3D>
>Ç=new Dictionary<long,MyTuple<int,Vector3D>>();bool ƣ;Dictionary<string,int>ɣ=new Dictionary<string,int>();public ɟ(int
ɦ,int Ɉ,int Ʌ,خ J){Ì=ɦ;ſ=Ɉ;ǘ=Ʌ;ņ=J;ɣ["base-fw"]=100;ɣ["base-up"]=30;ɣ["interval"]=50;ɣ["echelon"]=20;ɣ["circle"]=350;ɡ=J.
Ű("apck-wingman",J.ב.Me,null);}public void Ȱ(string ȱ,string Ȳ){int ȳ;if(int.TryParse(Ȳ,out ȳ)&&ɣ.ContainsKey(ȱ))ɣ[ȱ]=Ȳ.
Contains('-')||Ȳ.Contains('+')?ɣ[ȱ]+ȳ:ȳ;}public void Ť(){if(!ƣ){var ű=ņ.ב.RegisterBroadcastListener("apck.report");if(ʅ.ƅ>Ì){if(
ʅ.ƅ<ſ){while(ű.HasPendingMessage){Ж.Ъ++;var Ú=ű.AcceptMessage();var N=(MyTuple<MyTuple<string,byte,int,long>,Vector3D,
Vector3D,Vector3D,string>)Ú.Data;if(!Ç.ContainsKey(Ú.Source))Ç.Add(Ú.Source,new MyTuple<int,Vector3D>(N.Item1.Item3,N.Item2));}}
else{var ȴ=Ç.Where(Ä=>Ä.Value.Item1==ǘ-1).OrderBy(Ä=>(Ä.Value.Item2-ņ.ɷ).LengthSquared());if(ȴ.Any()){var ȵ=ȴ.First();if((ȵ.
Value.Item2-ņ.ɷ).Length()<ņ.ذ.ӱ<float>("tg-autolink-range")){ɠ=ȴ.First().Key;ɡ.ɿ=ɠ;ɡ.ժ();ņ.ђ.K("apck.tac.addchild",0,ɠ);}}ņ.ב
.DisableBroadcastListener(ű);ƣ=true;}}}}public ͻ ȶ=new ͻ("wingman");int ȷ;int ȹ;List<خ>ȸ=new List<خ>();public void Ȯ(){ȷ=
Ⱥ.Count;ȹ=0;ȶ.Ĵ=ņ.ו.Ĵ;ȶ.Ή=ņ.և;ȶ.Ί=new BoundingBoxD(ņ.î.CubeGrid.WorldVolume.Center,new Vector3D(ņ.î.CubeGrid.WorldVolume.
Radius,0,0));ȸ.Clear();foreach(var F in ņ.د.ج.Values)if(F.ƃ.ѳ?.Ɲ=="wingman")ȸ.Add(F);ȷ+=ȸ.Count;foreach(var F in ȸ){var Ȥ=Ȧ();
var ȥ=F.ƃ.Ѵ;ȥ.Α(Ȥ);ȥ.Ĵ=ȶ.Ĵ;ȥ.Ή=ȶ.Ή;ȥ.Ί=ȶ.Ί;}}Vector3D Ȧ(){var Ċ=ņ.և;var ȧ=Ċ.Translation+Ċ.Forward*ɣ["base-fw"]+Ċ.Up*ɣ[
"base-up"];double Ȩ=2*Math.PI/ȷ;Vector3D Ȥ;if(ņ.M.ӥ("wingman-circle-rotation"))Ȥ=ȩ(ȧ,ref Ċ,ɣ["circle"],Ȩ*ȹ++);else{ȹ=(ȹ>0)?ȹ*(-1)
:(Math.Abs(ȹ)+1);Ȥ=ȧ+Ċ.Left*ȹ*ɣ["interval"]+Ċ.Up*Math.Abs(ȹ)*ɣ["echelon"];}return Ȥ;}Vector3D ȩ(Vector3D Ç,ref MatrixD Ȫ,
double ǘ,double ȫ){var Ȭ=30/(800f/ǘ);return Ç+Ȫ.Right*Math.Cos(Math.PI*(ʅ.M/Ȭ)+ȫ)*ǘ+Ȫ.Up*Math.Sin(Math.PI*(ʅ.M/Ȭ)+ȫ)*ǘ;}void ȭ
(){foreach(var ȣ in Ⱥ){var Ȥ=Ȧ();ȶ.Α(Ȥ);ȶ.ͽ=ȣ;ņ.ב.SendUnicastMessage(ȣ,"apck-wingman",ȶ.И());}}HashSet<long>Ⱥ=new HashSet
<long>();public void ɂ(Ծ Ƀ){var Ȥ=new Ղ();Ȥ.Յ=(Ý,Q)=>Ⱥ.Add(Ý);Ȥ.Ճ=(Ý,Q)=>Ⱥ.Remove(Ý);Ȥ.Հ=ȭ;Ƀ.Ժ.Add("apck-wingman",Ȥ);}}
List<string>Ʉ(string º){return º.Split(new[]{"],["},StringSplitOptions.RemoveEmptyEntries).Select(Ì=>Ì.Trim('[',']')).ToList
();}class Ɇ{public List<IMyShipController>Ʀ;Ӯ Œ;public Ɇ(List<IMyShipController>Ô,Ӯ Ä){Œ=Ä;Ʀ=Ô;}public IMyShipController
ɇ=>Ʀ.Find(Ç=>Ç.IsUnderControl);public Vector3 Ɂ(ref MatrixD Ȼ){var Ȝ=new Vector3();if(Œ.ӥ("ignore-user-thruster"))return
Ȝ;if(ɇ!=null&&ɇ.MoveIndicator!=Vector3.Zero)return Vector3.TransformNormal(ɇ.MoveIndicator,Ȼ*MatrixD.Transpose(ɇ.
WorldMatrix));return Ȝ;}}static bool ȼ(int Ƚ,string[]ǌ,out Dictionary<string,string>Ő){if((ǌ.Length>Ƚ)&&ǌ[3].Contains("=")){Ő=ǌ[Ƚ].
Split(',').ToDictionary(Ì=>Ì.Split('=')[0],Ì=>Ì.Split('=')[1]);return true;}Ő=null;return false;}static M Ⱦ<M>(Dictionary<
string,string>ȿ,string ȱ){string Ȝ;if((ȿ!=null)&&ȿ.TryGetValue(ȱ,out Ȝ)&&!string.IsNullOrEmpty(Ȝ)){if(typeof(M)==typeof(string
))return(M)(object)Ȝ;else if(typeof(M)==typeof(int?))return(M)(object)int.Parse(Ȝ);else if(typeof(M)==typeof(long?))
return(M)(object)long.Parse(Ȝ);else if(typeof(M)==typeof(float?))return(M)(object)float.Parse(Ȝ);}return default(M);}static
void ɀ<M>(string Ì,Action<M>Ç){Ç((M)Enum.Parse(typeof(M),Ì));}static M ȯ<M>(object Ľ)where M:struct{if(Ľ is M)return(M)Ľ;
return default(M);}ʢ ʡ;class ʢ{public HashSet<Vector3I>ʣ=new HashSet<Vector3I>();public List<Ł>ʤ=new List<Ł>();Action<string>ě
;public string ʥ;public List<string>ʦ=new List<string>();public ʢ(Action<string>ʧ){ě=ʧ;}public ʢ ʨ(string ʩ){if(!string.
IsNullOrEmpty(ʩ)){var ȿ=ʩ.Split('\n').ToDictionary(Ì=>Ì.Split('=')[0],Ì=>string.Join("=",Ì.Split('=').Skip(1)));var ſ=Ⱦ<string>(ȿ,
"defs");if(ſ!=null){var ʜ=ſ.Split(new[]{'|'},StringSplitOptions.RemoveEmptyEntries);foreach(var N in ʜ){var Ç=N.Split(':');ʣ.
Add(new Vector3I(int.Parse(Ç[0]),int.Parse(Ç[1]),int.Parse(Ç[2])));}}ſ=Ⱦ<string>(ȿ,"navs-nodes");if(ſ!=null){var ʜ=ſ.Split(
new[]{'|'},StringSplitOptions.RemoveEmptyEntries);ʤ=new List<Ł>(ʜ.Length);foreach(var N in ʜ){var Ç=N.Split(':');var C=new
Vector3D(double.Parse(Ç[0]),double.Parse(Ç[1]),double.Parse(Ç[2]));ʤ.Add(new Ł(){Ń=C});}ſ=Ⱦ<string>(ȿ,"navs-links");if(ſ!=null){
foreach(var N in ſ.Split(new[]{'|'},StringSplitOptions.RemoveEmptyEntries)){var Ç=N.Split(':');var Ş=ʤ.ElementAt(int.Parse(Ç[0]
));var Ą=ʤ.ElementAt(int.Parse(Ç[1]));var ʛ=(Ş.Ń-Ą.Ń).Length();Ş.ĸ.Add(new MyTuple<Ł,double>(Ą,ʛ));Ą.ĸ.Add(new MyTuple<Ł,
double>(Ş,ʛ));}}ſ=Ⱦ<string>(ȿ,"navs-entries");if(ſ!=null){foreach(var X in ſ.Split(new[]{'|'},StringSplitOptions.
RemoveEmptyEntries)){ʤ[int.Parse(X)].ł=true;}}}ſ=Ⱦ<string>(ȿ,"saved-task-q");if(ſ!=null){var ʜ=ſ.Split(new[]{'|'},StringSplitOptions.
RemoveEmptyEntries);ʦ.AddRange(ʜ);}ʥ=Ⱦ<string>(ȿ,"default-task");}return this;}public void ʝ(){ě(ʳ());}string ʞ(){var ʜ=new List<string>()
;foreach(var N in ʣ){ʜ.Add(string.Join(":",string.Format("{0}:{1}:{2}",N.X,N.Y,N.Z)));}return string.Join("|",ʜ);}string
ʟ(){var ʜ=new List<string>();var ʚ=new List<string>();var ʠ=new List<string>();foreach(var Ç in ʤ){ʜ.Add(string.Join(":",
string.Format("{0}:{1}:{2}",Ç.Ń.X,Ç.Ń.Y,Ç.Ń.Z)));if(Ç.ł){ʠ.Add(ʤ.IndexOf(Ç).ToString());}foreach(var Ą in Ç.ĸ){ʚ.Add(string.
Join(":",string.Format("{0}:{1}",ʤ.IndexOf(Ç),ʤ.IndexOf(Ą.Item1))));}}return
$"navs-nodes={string.Join("|",ʜ)}\nnavs-links={string.Join("|",ʚ)}\nnavs-entries={string.Join(" | ",ʠ)}";}string ʳ(){string[]ʴ=new string[]{"defs="+ʞ(),"ts="+DateTime.Now.ToShortDateString(),"default-task="+ʥ,"saved-task-q="
+string.Join("|",ʦ),ʟ()};return string.Join("\n",ʴ);}public override string ToString(){return ʳ();}}ʶ ʵ;class ʶ{public ʁ
ʷ=new ʁ();خ ʸ;ڦ ʹ;public void Ȃ(خ J){ʸ=J;}bool ʺ;public void ʻ(){if(!ʺ){ʹ=ʸ.Ű(ӟ,-1,null);ʹ.ժ();ʺ=true;}}int ʼ;public void
ɵ(List<MyIGCMessage>ʽ){ʷ.ɵ();ʸ.ג.պ(ӟ,ref ʪ);ʫ.Clear();ʲ.Clear();foreach(var Ú in ʪ){var N=ȯ<MyTuple<long,Vector3D>>(Ú.
Data);if(N.Item1!=0){if(ʲ.Add(N.Item1))ʫ.Add(new ɾ{ʀ=N.Item1,ʂ=N.Item2,ɿ=Ú.Source});}else{var Ü=ȯ<ImmutableArray<MyTuple<
long,Vector3D>>>(Ú.Data);foreach(var ɼ in Ü)if(ʲ.Add(ɼ.Item1))ʫ.Add(new ɾ{ʀ=ɼ.Item1,ʂ=ɼ.Item2,ɿ=Ú.Source});}}ʼ=0;ʬ(ʸ);
foreach(var J in ʸ.د.ج.Values)ʬ(J);ʸ.ց?.ә($"interested: {ʼ}");if(ʼ==0){if(ʺ){ʸ.Ŵ(ʹ);ʺ=false;}}else ʻ();}HashSet<long>ʲ=new
HashSet<long>();List<MyIGCMessage>ʪ=new List<MyIGCMessage>();List<ɾ>ʫ=new List<ɾ>();void ʬ(خ J){J.ƃ.ќ(ʮ,ʭ);if(J.թ&&J.ƃ.ј){ʼ++;
if(ʷ.ɹ)J.ƃ.Ґ(ʷ.ɰ);else foreach(var Ä in ʫ)J.ƃ.њ(Ä);}}public TS ʭ=TS.Loop;int ʮ;public void ʯ(ɾ Ä,خ J){J.ƃ.ґ(Ä.ʀ,Ä.ɿ);ʮ++;}
Dictionary<long,HashSet<long>>ʰ=new Dictionary<long,HashSet<long>>();public void ɂ(Ծ Ƀ){var ʱ=new Ղ();ʱ.Յ=(Ý,Q)=>{if(!ʰ.
ContainsKey(Q))ʰ.Add(Q,new HashSet<long>());ʰ[Q].Add(Ý);};ʱ.Ճ=(Ý,Q)=>{if(Q==0){foreach(var Ì in ʰ)Ì.Value.Remove(Ý);}else if(ʰ.
ContainsKey(Q))ʰ[Q].Remove(Ý);};ʱ.Մ=(Q)=>ʷ.ɹ&&ʷ.ɰ.ͽ==Q;ʱ.Ն=(ɺ,Ì,C,ƶ,ǘ)=>{var ɻ=ʷ.ɹ&&Ծ.Ի(C,ƶ,ǘ,ʷ.ɷ);if(ɻ&&(ɺ==null||(ɺ==ʷ.ɰ.Ύ.
ToString())))return ʷ.ɰ.ͽ;return-1;};ʱ.Հ=()=>{var ȶ=ʷ.ɰ;if(ʷ.ɹ&&ȶ.Ά.HasValue){var ɼ=ȶ.И();var ɽ=new MyTuple<long,Vector3D>(ȶ.ͽ,ȶ
.Ά.Value);ʸ.ב.SendBroadcastMessage(ӟ,ɽ);if(ʰ.ContainsKey(ȶ.ͽ)){var Ǣ=ʰ[ȶ.ͽ];if(Ǣ.Count>0){foreach(var Ì in Ǣ)ʸ.ב.
SendUnicastMessage(Ì,ӝ,ɼ);}else ʰ.Remove(ȶ.ͽ);}ʸ.ב.SendBroadcastMessage("tgp.global.gridsense.update",ɼ);}};Ƀ.Ժ.Add(ӝ,ʱ);}}struct ɾ{public
long ɿ;public long ʀ;public Vector3D ʂ;}class ʁ{public bool ɹ=>ɱ;public Vector3D ɷ;public Vector3D ɭ;public bool ɮ=true;
public HashSet<IMyLargeTurretBase>ɯ=new HashSet<IMyLargeTurretBase>();public ͻ ɰ=new ͻ("tdesignator"){Ό=12};bool ɱ;
MyDetectedEntityInfo ɲ;Func<MyDetectedEntityInfo,bool>ɳ;خ ņ;public void ɴ(خ J){ņ=ņ??J;foreach(var Ô in J.ք)ɯ.Add(Ô);ɳ=ʍ;}public void ɵ(){
bool ɶ=false;ɱ=false;if(!ɮ)return;foreach(var N in ɯ){if(N.HasTarget){var X=N.GetTargetedEntity();if(ɶ)ɱ=true;else{ɶ=true;if
(ɳ!=null)ɶ=ɳ(X);if(ɶ){ɱ=true;ɭ=X.Velocity;ɲ=X;ɷ=ņ.M.ӥ("aim-to-center")?X.BoundingBox.Center:X.HitPosition.Value;}else{ɱ=
false;N.ResetTargetingToDefault();}}}}if(ɱ){ɰ.Α(ɷ);ɰ.Ĵ=ɭ;ɰ.Ί=ɲ.BoundingBox;ɰ.Ή=ɲ.Orientation;ɰ.ͽ=ɲ.EntityId;ɰ.Ύ=ɲ.Type;}}long
ɬ;Vector3D ɸ;int ʃ;bool ʍ(MyDetectedEntityInfo Ç){var ʎ=ņ.ذ.ӱ<float>("filtering-size");var Ȝ=(ʎ<=0||ʎ<=Ç.BoundingBox.
Extents.Length());if(ņ.M.ӥ("freefall-target-filter")){if(ɬ==Ç.EntityId){if(Ç.Velocity.LengthSquared()>0){var ʏ=(Ç.Velocity-ɸ)*
60;if((ʏ==Vector3D.Zero)||(ņ.ו.Ȣ==ʏ))ʃ++;if(ʃ>10)Ȝ=false;}}else{ɬ=Ç.EntityId;ɸ=Ç.Velocity;ʃ=0;}}return Ȝ;}}Queue<ʑ>ʌ;void
ʐ(){if(ʌ.Count>0){var Ô=ʌ.Peek();if(Ô.ʘ<ʅ.ƅ){Ô.ʒ.Invoke();if(Ô.ʓ?.Invoke()==true){Ô.ʘ=ʅ.ƅ+Ô.ʔ;}else ʌ.Dequeue();}}}class
ʑ{public int ʘ;public Action ʒ;public Func<bool>ʓ;public int ʔ;public static ʑ ʕ(Action Ã,int ʖ,Func<bool>ʗ=null){return
new ʑ{ʒ=Ã,ʔ=ʖ,ʓ=ʗ};}public void ʙ(Queue<ʑ>ʌ){ʘ=ʅ.ƅ+ʔ;ʌ.Enqueue(this);}}static bool ʄ(IMyTerminalBlock Ą)=>Ą.CubeGrid.
GridSizeEnum==MyCubeSize.Large;static class ʅ{static Action<string>ſ;static IMyTextSurface C;public static double M;public static
int ƅ;public static double ʆ=1/60f;public static int ʇ;public static void Ȃ(Program é){ſ=é.Echo;C=é.Me.GetSurface(0);C.
ContentType=ContentType.TEXT_AND_IMAGE;C.WriteText("");}public static void ʈ(string Ì){ſ(Ì);}public static void ʉ(string Ì){C.
WriteText($"{ƅ}: {Ì}\n",true);}public static void ʊ(string Ì){ʇ++;ʉ(Ì);}public static Action<string>ҹ;public static Action ʋ;}