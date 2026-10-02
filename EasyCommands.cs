// Welcome to Easy Commands! Update the Custom Data of this Programmable Block to get started.
// 
// Having trouble?  You can change the updateFrequency below to slow down execution (Update100), or manually step through execution (None).
// 
// A full list of supported capabilities can be found at
// https://spaceengineers.merlinofmines.com/EasyCommands/
public class J:A<IMyCubeGrid>{public J(){B(C.D,E=>E.CustomName,(E,F)=>E.CustomName=F);G(C.H,E=>E.IsStatic);B(C.I,E=>E.
GridSizeEnum==MyCubeSize.Large?"large":"small");}public override string L(IMyCubeGrid K)=>K.CustomName;public override IEnumerable<
IMyCubeGrid>P<M>(List<M>N,Func<M,bool>O=null)=>N.Where(O??(E=>true)).Select(E=>((IMyTerminalBlock)E).CubeGrid).Distinct();}public
List<IMyTerminalBlock>R=Q<IMyTerminalBlock>();public T<S,List<Object>>U=new T<S,List<Object>>(),V=new T<S,List<Object>>();
public T<Type,ITerminalAction>W=new T<Type,ITerminalAction>();public T<Type,ITerminalProperty>X=new T<Type,ITerminalProperty>(
);public static class Ħ{static Dictionary<S,Y>Ċ=new Dictionary<S,Y>{{S.Z,new a()},{S.b,new c()},{S.d,new e()},{S.f,new g(
)},{S.h,new i()},{S.j,new k()},{S.l,new m()},{S.n,new o<IMyCockpit>()},{S.p,new q<IMyCollector>()},{S.r,new s()},{S.t,new
o<IMyCryoChamber>()},{S.u,new q<IMyDecoy>()},{S.v,new w()},{S.x,new y()},{S.z,new ª()},{S.µ,new q<IMyShipDrill>()},{S.º,
new À()},{S.Á,new Â<IMyPowerProducer>("Engine")},{S.Ã,new Ä()},{S.Å,new Æ()},{S.Ç,new È()},{S.É,new J()},{S.Ê,new q<
IMyShipGrinder>()},{S.Ë,new Ì<IMyUserControllableGun>()},{S.Í,new Î<IMyGyro>()},{S.Ï,new Ð()},{S.Ñ,new Ò(Ó("Hinge"))},{S.Ô,new Õ()},{S
.Ö,new Ø()},{S.Ù,new Ú()},{S.Û,new Ü()},{S.Ý,new Þ()},{S.ß,new à()},{S.á,new â()},{S.ã,new ä()},{S.å,new æ()},{S.ç,new Â<
IMyReactor>()},{S.è,new é()},{S.ê,new q<IMyRefinery>()},{S.ë,new Ò(E=>!Ó("Hinge")(E))},{S.ì,new Â<IMySolarPanel>()},{S.í,new î()},
{S.ï,new ð()},{S.ñ,new ò()},{S.ó,new ô()},{S.õ,new ö()},{S.ø,new ù()},{S.ú,new û<IMyTerminalBlock>()},{S.ü,new ý()},{S.þ,
new ÿ()},{S.Ā,new ā()},{S.Ă,new Â<IMyPowerProducer>("WindTurbine")},{S.ă,new Ą<IMyLargeTurretBase>()},{S.ą,new Ć()},{S.ć,
new Ĉ()},{S.ĉ,new q<IMyShipWelder>()}};public static Y č(S ċ){if(!Ċ.ContainsKey(ċ))throw new Č("Unsupported Block Type: "+ċ
);return Ċ[ċ];}public static List<Object>ď(S?ċ)=>ċ==null||ċ==S.x||ċ==S.É?Ċ[ċ??S.á].Ď(Q(á.Me)):null;public static List<
Object>ę(S ċ,string O=null){if(ċ==S.þ)return Ċ[ċ].Ď(Đ(),đ=>O?.Equals(đ.Ē??đ.ē)??true).Select(đ=>((Ĕ)đ).ĕ).Distinct().OrderBy(đ
=>đ==á.Ė).OfType<Object>().ToList();if(á.R.Count==0)á.GridTerminalSystem.GetBlocks(á.R);return á.U.ė(ċ,O,Ę=>Ċ[ċ].Ď(á.R,E=>
Ę?.Equals(E.CustomName)??true));}public static List<Object>ě(S ċ,String Ě)=>á.V.ė(ċ,Ě,Ę=>{var N=Q<IMyTerminalBlock>();á.
GridTerminalSystem.GetBlockGroupWithName(Ę)?.GetBlocks(N);return Ċ[ċ].Ď(N);});static List<Ĕ>Đ(){var Ğ=á.Ĝ.Select(đ=>đ.ĝ("async")).ToList()
;var Ġ=á.ğ.Skip(1).Select(đ=>đ.ĝ("queued")).ToList();var Ė=á.Ė.ĝ("current");var ġ=á.Ė;var ģ=á.Ĝ.Where(đ=>á.Ė==đ.Ģ).Select
(đ=>đ.ĝ("child")).ToList();return Ĥ(ĥ(Ė),á.Ĝ,á.ğ,Ğ,Ġ,ģ).ToList();}}public class ÿ:A<Ĕ>{public ÿ(){B(C.D,đ=>đ.Ē??đ.ē,(đ,Ę)
=>đ.Ē=Ę);G(C.ħ,đ=>false,(đ,E)=>{đ.Ĩ=new ĩ();if(đ==á.Ė)throw new Ī();});ī[Ĭ.ĭ]=C.ħ;}public override string L(Ĕ Į)=>Į.Ē??Į.ē
;}public class Ć:q<IMyTurretControlBlock>{public Ć(){į(C.İ,E=>E.Range,(E,F)=>E.Range=F,100f);ı(C.H,Ĳ(
"EnableTargetLocking",true));ı(C.ĳ,Ĳ("AI",true));G(C.Ĵ,E=>E.IsUnderControl);ĵ(C.Ķ,Ĭ.ķ,ĸ(Ĺ(E=>E.HasTarget?ĺ(E.GetTargetedEntity()):Vector3D.
Zero),Ĭ.ķ),ĸ(Ļ(E=>E.HasTarget),Ĭ.ĭ),ĸ(ļ(E=>E.GetTargetingGroup(),(E,F)=>E.SetTargetingGroup(F)),Ĭ.Ľ));ı(C.ľ,Ĳ(
"AngleDeviation",5f));G(C.Ŀ,E=>ŀ(E).Any(Ł=>Ł.IsShooting),(E,F)=>{foreach(IMyUserControllableGun ł in ŀ(E))ł.Shoot=F;});Ń(C.ń,Ņ.ņ,ĸ(Ň(E=>
E.VelocityMultiplierAzimuthRpm,(E,F)=>E.VelocityMultiplierAzimuthRpm=F),Ņ.ň,Ņ.ņ),ĸ(Ň(E=>E.VelocityMultiplierElevationRpm,
(E,F)=>E.VelocityMultiplierElevationRpm=F),Ņ.ŉ,Ņ.Ŋ));ī[Ĭ.ŋ]=C.İ;}IEnumerable<IMyUserControllableGun>ŀ(
IMyTurretControlBlock K){var Ō=Q<IMyFunctionalBlock>();K.GetTools(Ō);return Ō.OfType<IMyUserControllableGun>();}}public class Ą<ō>:Ì<
IMyLargeTurretBase>{public Ą(){į(C.İ,E=>E.Range,(E,F)=>E.Range=F,100f);ı(C.H,Ĳ("EnableTargetLocking",true));G(C.Ĵ,E=>E.IsUnderControl);G(C
.Ŏ,E=>E.EnableIdleRotation,(E,F)=>{E.EnableIdleRotation=F;E.SyncEnableIdleRotation();});ĵ(C.Ķ,Ĭ.ķ,ĸ(Ĺ(ŏ,Ő),Ĭ.ķ),ĸ(Ļ(E=>E.
HasTarget,(E,F)=>{if(!F)ő(E);}),Ĭ.ĭ),ĸ(ļ(E=>E.GetTargetingGroup(),(E,F)=>E.SetTargetingGroup(F)),Ĭ.Ľ));ı(Q(C.Ķ,C.ń),Ĺ(E=>E.
GetTargetedEntity().Velocity,(E,F)=>E.TrackTarget(ŏ(E),F)));į(C.ľ,E=>E.Azimuth*Œ,(E,F)=>{E.Azimuth=F*œ;E.SyncAzimuth();},5);į(C.Ŕ,E=>E.
Elevation*Œ,(E,F)=>{E.Elevation=F*œ;E.SyncElevation();},5);ī[Ĭ.ķ]=C.Ķ;ī[Ĭ.ŋ]=C.İ;}Vector3D ŏ(IMyLargeTurretBase ŕ)=>Ŗ(ŗ(ŕ,
"target")??"")??(ŕ.HasTarget?ĺ(ŕ.GetTargetedEntity()):Vector3D.Zero);void ő(IMyLargeTurretBase ŕ){bool Ř=ŕ.EnableIdleRotation;ŕ.
ResetTargetingToDefault();ř(ŕ,"target");ŕ.EnableIdleRotation=Ř;ŕ.SyncEnableIdleRotation();}void Ő(IMyLargeTurretBase ŕ,Vector3D Ś){ŕ.SetTarget(
Ś);ś(ŕ,"target",Ŝ(Ś));}}public class Ð:q<IMyHeatVent>{public Ð(){Ń(C.ŝ,Ņ.ŉ,ĸ(Ĳ("ColorMax",Color.Black),Ņ.ŉ),ĸ(Ĳ(
"ColorMin",Color.Black),Ņ.Ŋ));var Ş=Ĳ("Radius",1);ı(C.İ,Ş);ı(C.ş,Ş);ı(C.Š,Ĳ("Intensity",1));ı(C.š,Ĳ("Falloff",0.3));ı(C.Ţ,Ĳ(
"Offset",0.5));ı(C.ţ,Ĳ("PowerDependency",10));ī[Ĭ.ŝ]=C.ŝ;ī[Ĭ.ŋ]=C.ţ;Ť[Ņ.ŉ]=C.ŝ;}}public delegate object Ŧ(ITerminalProperty ť,
IMyTerminalBlock K);public delegate void Ū(ITerminalProperty ť,IMyTerminalBlock ŧ,Ũ ũ);public class ŭ{public Ŧ ū;public Ū Ŭ;}public
static ŭ Ů(Ŧ ū,Ū Ŭ)=>new ŭ{ū=ū,Ŭ=Ŭ};public class Ĳ<ō>:ů<ō>where ō:class,IMyTerminalBlock{public Ĳ(String Ű,Ũ ű):this(new Ų(Q(
new ų(Ű))),ű){}public Ĳ(Ų Ŵ,Ũ ű):base((E,ŵ)=>{var ť=Ŷ(E,Ŵ);return ŷ(Ÿ[ť.TypeName].ū(ť,E));},(E,ŵ,F)=>{var ť=Ŷ(E,Ŵ);Ÿ[ť.
TypeName].Ŭ(ť,E,F);},ű){}static Dictionary<String,ŭ>Ÿ=Ź(ź("StringBuilder",Ů((ŵ,E)=>E.GetValue<StringBuilder>(ŵ.Id).ToString(),(ŵ
,E,F)=>E.SetValue(ŵ.Id,new StringBuilder(Ż(F))))),ź("Boolean",Ů((ŵ,E)=>ŵ.AsBool().GetValue(E),(ŵ,E,F)=>ŵ.AsBool().
SetValue(E,ż(F)))),ź("Single",Ů((ŵ,E)=>ŵ.AsFloat().GetValue(E),(ŵ,E,F)=>ŵ.AsFloat().SetValue(E,Ž(F)))),ź("Int64",Ů((ŵ,E)=>(float
)ŵ.As<long>().GetValue(E),(ŵ,E,F)=>ŵ.As<long>().SetValue(E,(long)Ž(F)))),ź("Color",Ů((ŵ,E)=>ŵ.AsColor().GetValue(E),(ŵ,E,
F)=>ŵ.AsColor().SetValue(E,ž(F)))));}public static ITerminalProperty Ŷ(IMyTerminalBlock K,Ų Ŵ)=>á.X.ė(K.GetType(),Ŵ.ſ(),Ę
=>{var ť=K.GetProperty(Ę);if(ť==null)throw new Č(K.BlockDefinition.SubtypeName+" does not have property support for: "+Ę);
return ť;});public class û<ō>:A<ō>where ō:class,IMyTerminalBlock{public û(){ƀ(C.Ɓ,K=>K.GetPosition());B(C.D,E=>E.CustomName,(E
,F)=>E.CustomName=F);G(C.Ƃ,E=>E.ShowInTerminal,(E,F)=>E.ShowInTerminal=F);B(C.ƃ,E=>E.CustomData,(E,F)=>E.CustomData=F);B(
C.Ƅ,E=>E.DetailedInfo);ƅ(C.Ɔ,E=>{var Ƈ=Q<ITerminalProperty>();E.GetProperties(Ƈ);return ƈ(Ƈ.Select(ŵ=>Ɖ(ŵ.Id)));});ƅ(C.Ɗ,
E=>{var Ƌ=Q<ITerminalAction>();E.GetActions(Ƌ);return ƈ(Ƌ.Select(ŵ=>Ɖ(ŵ.Id)));});ı(C.ƌ,new ů<ō>((E,ŵ)=>ŵ.ƍ[0].Ǝ.ū(),(E,ŵ,
F)=>á.W.ė(E.GetType(),Ż(ŵ.ƍ[0].Ǝ.ū()),Ę=>E.GetActionWithName(Ę)).Apply(E),ŷ(0)));Ń(C.Ə,Ņ.Ɛ,ĸ(Ĺ(E=>E.WorldMatrix.Forward),
Ņ.Ɛ),ĸ(Ĺ(E=>E.WorldMatrix.Backward),Ņ.Ƒ),ĸ(Ĺ(E=>E.WorldMatrix.Up),Ņ.ŉ),ĸ(Ĺ(E=>E.WorldMatrix.Down),Ņ.Ŋ),ĸ(Ĺ(E=>E.
WorldMatrix.Left),Ņ.ň),ĸ(Ĺ(E=>E.WorldMatrix.Right),Ņ.ņ));var Ɣ=Ň(E=>ƒ(Ɠ(E)));ı(Q(C.ƕ,C.İ),Ɣ);ı(Q(C.Ɩ,C.İ),Ɣ);var Ƙ=Ļ(E=>Ɨ(Ɠ(E))==1)
;ı(C.ƕ,Ƙ);ı(C.ħ,Ƙ);ı(Q(C.ƕ,C.ħ),Ƙ);ı(Q(C.ƕ,C.I),Ň(E=>ƙ(Ɠ(E))));ı(Q(C.ƕ,C.ţ),Ň(E=>Ɨ(Ɠ(E))));var ƛ=ƚ(Ĭ.ŋ,ĸ(Ň(E=>ƒ(Ɠ(E))-ƙ(Ɠ
(E))),Ĭ.ŋ),ĸ(Ļ(E=>Ɨ(Ɠ(E))<1),Ĭ.ĭ));ı(C.Ɩ,ƛ);ı(Q(C.Ɩ,C.I),ƛ);ı(Q(C.Ɩ,C.ţ),Ň(E=>1-Ɨ(Ɠ(E))));ī[Ĭ.ķ]=C.Ɓ;ī[Ĭ.ĭ]=C.Ɯ;}
IMySlimBlock Ɠ(ō E)=>E.CubeGrid.GetCubeBlock(E.Position);float ƙ(IMySlimBlock K)=>K.BuildIntegrity-K.CurrentDamage;float ƒ(
IMySlimBlock K)=>K.MaxIntegrity;float Ɨ(IMySlimBlock K)=>ƙ(K)/ƒ(K);public override Ɲ<ō>ƞ(Ų ť){try{return base.ƞ(ť);}catch(Č){return
new Ĳ<ō>(ť,ŷ(1));}}public override string L(ō K)=>K.CustomName;public String ŗ(ō K,String Ɵ)=>Ơ(K).GetValueOrDefault(Ɵ,null
);public void ś(ō K,String Ɵ,String ũ){var ơ=Ơ(K);ơ[Ɵ]=ũ;Ƣ(K,ơ);}public void ř(ō K,String Ɵ){var ơ=Ơ(K);ơ.Remove(Ɵ);Ƣ(K,ơ
);}public void Ƣ(ō K,Dictionary<String,String>ƣ)=>K.CustomData=String.Join("\n",ƣ.Select(ŵ=>ŵ.Key+"="+ŵ.Value));public
Dictionary<String,String>Ơ(ō K)=>K.CustomData.Split(new[]{"\r\n","\r","\n"},StringSplitOptions.RemoveEmptyEntries).Select(Ƥ=>Ƥ.
Split('=')).ToDictionary(ƥ=>ƥ[0],ƥ=>ƥ[1]);public Ɲ<ō>Ĳ(String Ű,object ű)=>new Ĳ<ō>(Ű,ŷ(ű));}public class q<ō>:û<ō>where ō:
class,IMyFunctionalBlock{public q(){var Ʀ=Ļ(E=>E.Enabled,(E,F)=>E.Enabled=F);ı(C.Ɯ,Ʀ);ı(C.Ƨ,Ʀ);}}public class ƪ<ō>:q<ō>where
ō:class,IMyFunctionalBlock{Func<ō,bool>ƨ;public ƪ(Func<ō,bool>Ʃ){ƨ=Ʃ;}public override IEnumerable<ō>P<M>(List<M>N,Func<M,
bool>O=null)=>base.P(N,O).Where(ƨ);}public static Func<IMyFunctionalBlock,bool>Ó(string Ʃ)=>E=>Ʃ.Length==0||E.
BlockDefinition.SubtypeId.Contains(Ʃ);static Vector3D ĺ(MyDetectedEntityInfo ƫ)=>ƫ.HitPosition??ƫ.Position;public class a:q<IMyAirVent>
{public a(){G(C.ħ,E=>!Ƭ(E));G(C.ƭ,E=>Ƭ(E));G(C.Ʈ,E=>!E.Depressurize,(E,F)=>E.Depressurize=!F);į(C.ţ,E=>E.GetOxygenLevel()
);į(C.I,E=>E.GetOxygenLevel());ī[Ĭ.ŋ]=C.ţ;Ť[Ņ.ŉ]=C.ţ;}bool Ƭ(IMyAirVent E)=>(E.Status==VentStatus.Depressurizing||E.
Status==VentStatus.Pressurizing)&&E.GetOxygenLevel()>0.0001;}public class Ø:q<IMyLaserAntenna>{public Ø(){ƀ(C.Ķ,E=>E.
TargetCoords,(E,F)=>E.SetTargetCoords($"GPS:Target:{Ŝ(F)}:"));G(C.H,E=>E.IsPermanent,(E,F)=>E.IsPermanent=F);var Ư=Ň(E=>E.Range,(E,F
)=>E.Range=F,1000);ı(C.İ,Ư);ı(C.ş,Ư);G(C.ư,E=>E.Status==MyLaserAntennaStatus.Connected,(E,F)=>{if(F)E.Connect();});ī[Ĭ.ķ]
=C.Ķ;}}public class c:q<IMyRadioAntenna>{public c(){B(C.Ʊ,E=>E.HudText,(E,F)=>E.HudText=F);G(C.ư,E=>E.EnableBroadcasting,
(E,F)=>E.EnableBroadcasting=F);G(C.Ʈ,E=>E.EnableBroadcasting,(E,F)=>E.EnableBroadcasting=F);var Ş=Ň(E=>E.Radius,(E,F)=>E.
Radius=F,1000);ı(C.İ,Ş);ı(C.ş,Ş);G(C.Ƃ,E=>E.ShowShipName,(E,F)=>E.ShowShipName=F);ī[Ĭ.Ľ]=C.Ʊ;ī[Ĭ.ĭ]=C.ư;Ť[Ņ.ŉ]=C.İ;}}public
class g:q<IMyBatteryBlock>{public g(){G(C.Ʈ,E=>E.ChargeMode!=ChargeMode.Recharge,(E,F)=>E.ChargeMode=(F?ChargeMode.Auto:
ChargeMode.Recharge));G(C.ĳ,E=>E.ChargeMode==ChargeMode.Auto,(E,F)=>E.ChargeMode=(F?ChargeMode.Auto:ChargeMode.Recharge));į(C.İ,E
=>E.MaxStoredPower);į(C.ţ,E=>E.CurrentStoredPower/E.MaxStoredPower);į(C.Ʋ,E=>E.CurrentInput);į(C.Š,E=>E.CurrentOutput);į(C
.I,E=>E.CurrentStoredPower);ı(Q(C.Š,C.İ),Ň(E=>E.MaxOutput));ı(Q(C.Ʋ,C.İ),Ň(E=>E.MaxInput));ī[Ĭ.ŋ]=C.ţ;Ť[Ņ.ŉ]=C.ţ;}}public
class i:q<IMyBeacon>{public i(){B(C.Ʊ,E=>E.HudText,(E,F)=>E.HudText=F);var Ş=Ň(E=>E.Radius,(E,F)=>E.Radius=F,1000);ı(C.İ,Ş);ı
(C.ş,Ş);G(C.Ʈ,E=>E.Enabled,(E,F)=>E.Enabled=F);ī[Ĭ.Ľ]=C.Ʊ;ī[Ĭ.ŋ]=C.İ;Ť[Ņ.ŉ]=C.İ;}}public delegate Ũ Ƴ<ō>(ō K,Ų ť);public
delegate Ũ Ƶ<ō>(ō K,Ų ť,Ņ ƴ);public delegate void ƶ<ō>(ō K,Ų ť,Ũ ũ);public delegate void Ʒ<ō>(ō K,Ų ť,Ņ ƴ,Ũ ũ);public delegate
void Ƹ<ō>(ō K,Ų ť);public delegate void ƺ<ō>(ō K,Ų ť,Ũ ƹ);public delegate void ƻ<ō>(ō K,Ų ť,Ņ ƴ,Ũ ƹ);public delegate void Ƽ<
ō>(ō K,Ų ť,Ņ ƴ);public delegate void ƽ<ō>(ō K,Ų ť);public delegate M ƾ<ō,M>(ō K);public delegate void ƿ<ō,M>(ō K,M ũ);
public class Ɲ<ō>{public Ƴ<ō>ǀ;public Ƶ<ō>ǁ;public ƶ<ō>ǂ;public Ʒ<ō>ǃ;public Ƹ<ō>Ǆ;public ƺ<ō>ǅ;public ƻ<ō>ǆ;public Ƽ<ō>Ǉ;
public ƽ<ō>ǈ;}public class ů<ō>:Ɲ<ō>{public ů(Ƴ<ō>ǉ,ƶ<ō>Ǌ,Ũ ű){ǀ=ǉ;ǂ=Ǌ;ǁ=(E,ŵ,ơ)=>ǀ(E,ŵ);ǃ=(E,ŵ,ơ,F)=>ǂ(E,ŵ,F);Ǆ=(E,ŵ)=>ǅ(E,ŵ,
ű);ǅ=(E,ŵ,F)=>ǂ(E,ŵ,ǀ(E,ŵ).ǋ(ǌ(F,ŵ)));ǆ=(E,ŵ,ơ,F)=>ǃ(E,ŵ,ơ,ǁ(E,ŵ,ơ).ǋ(ǌ(F,ŵ)));Ǉ=(E,ŵ,ơ)=>ǃ(E,ŵ,ơ,ǁ(E,ŵ,ơ).ǋ(ǌ(ű,ŵ)));ǈ=(
E,ŵ)=>ǂ(E,ŵ,ǀ(E,ŵ).Ǎ());}Ũ ǌ(Ũ ŵ,Ų ť)=>ť.ǎ??true?ŵ:ŵ.Ǎ();}public class Ǒ<ō,M>:ů<ō>{public Ǒ(ƾ<ō,M>ū,ƿ<ō,M>Ŭ,Func<Ũ,M>Ǐ,M
ǐ):base((E,ŵ)=>ŷ(ū(E)),(E,ŵ,F)=>Ŭ(E,Ǐ(F)),ŷ(ǐ)){}}public delegate Ɲ<ō>ƞ<ō>(Ų ǒ);public class ǔ<ō>:Ɲ<ō>{public ǔ(ƞ<ō>Ǔ){ǀ=
(E,ŵ)=>Ǔ(ŵ).ǀ(E,ŵ);ǂ=(E,ŵ,F)=>Ǔ(ŵ).ǂ(E,ŵ,F);ǁ=(E,ŵ,ơ)=>Ǔ(ŵ).ǁ(E,ŵ,ơ);ǃ=(E,ŵ,ơ,F)=>Ǔ(ŵ).ǃ(E,ŵ,ơ,F);ǆ=(E,ŵ,ơ,F)=>Ǔ(ŵ).ǆ(E,ŵ
,ơ,F);ǅ=(E,ŵ,F)=>Ǔ(ŵ).ǅ(E,ŵ,F);Ǆ=(E,ŵ)=>Ǔ(ŵ).Ǆ(E,ŵ);Ǉ=(E,ŵ,ơ)=>Ǔ(ŵ).Ǉ(E,ŵ,ơ);}}public class ĸ<ō,M>{public Ɲ<ō>Ǖ;public M[
]ǖ;}public interface Y{Ų ǘ(Ĭ Ǘ);Ų ǘ(Ņ ƴ);Ņ Ǚ();List<Object>Ď<M>(List<M>N,Func<M,bool>O=null);String ǚ(Object K);Ũ Ǜ(
Object K,Ų ť);void ǜ(Object K,Ų ť);void ƺ(Object K,Ų ť);void ǝ(Object K,Ų ť);}public abstract class A<ō>:Y where ō:class{
public Dictionary<String,Ɲ<ō>>Ǟ=Ź<String,Ɲ<ō>>();public Dictionary<Ĭ,C>ī=Ź<Ĭ,C>();public Dictionary<Ņ,C>Ť=Ź<Ņ,C>();public Ņ ǟ=
Ņ.ŉ;public A(){B(C.D,E=>L(E));ī[Ĭ.Ľ]=C.D;}public string ǚ(object K)=>L((ō)K);public virtual Ɲ<ō>ƞ(Ų ť){var ǡ=Ǡ(ť);if(Ǟ.
ContainsKey(ǡ))return Ǟ[ǡ];throw new Č(typeof(ō).Name+" does not have property support for: "+ť.ſ());}public string Ǡ(Ų Ŵ)=>Ŵ.ƍ.
OrderBy(ŵ=>ŵ.Ǣ).Aggregate("",(ǣ,E)=>ǣ+E.Ǣ);public List<Object>Ď<M>(List<M>N,Func<M,bool>O=null)=>P(N,O).OfType<Object>().ToList
();public virtual IEnumerable<ō>P<M>(List<M>N,Func<M,bool>O=null)=>N.Where(O??(E=>true)).OfType<ō>();public abstract
string L(ō K);public Ņ Ǚ()=>ǟ;public Ų ǘ(Ņ ƴ)=>new Ų(Q(new ų(Ť.GetValueOrDefault(ƴ,Ť[ǟ])+"")));public Ų ǘ(Ĭ Ǘ)=>new Ų(Q(new ų(
ī.GetValueOrDefault(Ǘ,ī[Ĭ.Ľ])+"")));public Ũ Ǜ(object K,Ų ť){Ũ ũ=ť.ƴ.HasValue?ƞ(ť).ǁ((ō)K,ť,ť.ƴ.Value):ƞ(ť).ǀ((ō)K,ť);
return ť.Ǥ?ũ.Ǎ():ũ;}public void ǜ(Object K,Ų ť){if(ť.ǥ!=null){Ũ ũ=ť.ǥ.ū();if(ť.ƴ!=null){ƞ(ť).ǃ((ō)K,ť,ť.ƴ.Value,ũ);}else{ƞ(ť).
ǂ((ō)K,ť,ũ);}}else{ƞ(ť).Ǉ((ō)K,ť,ť.ƴ??ǟ);}}public void ƺ(Object K,Ų ť){if(ť.ǥ==null){ƞ(ť).Ǆ((ō)K,ť);}else{Ũ ũ=ť.ǥ.ū();if(
ť.ƴ!=null){ƞ(ť).ǆ((ō)K,ť,ť.ƴ.Value,ũ);}else{ƞ(ť).ǅ((ō)K,ť,ũ);}}}public void ǝ(Object K,Ų ť)=>ƞ(ť).ǈ((ō)K,ť);public void ı
(List<C>Ƈ,Ɲ<ō>Ǖ)=>Ǟ[Ǡ(new Ų(Ƈ.Select(ŵ=>new ų(ŵ+"")).ToList()))]=Ǖ;public void ı(C ť,Ɲ<ō>Ǖ)=>Ǟ[ť+""]=Ǖ;public void G(C ť,
ƾ<ō,bool>ǀ,ƿ<ō,bool>ǂ=null)=>ı(ť,Ļ(ǀ,ǂ));public void B(C ť,ƾ<ō,string>ǀ,ƿ<ō,string>ǂ=null)=>ı(ť,ļ(ǀ,ǂ));public void į(C ť
,ƾ<ō,float>ǀ,ƿ<ō,float>ǂ=null,float ű=0)=>ı(ť,Ň(ǀ,ǂ,ű));public void ƀ(C ť,ƾ<ō,Vector3D>ǀ,ƿ<ō,Vector3D>ǂ=null)=>ı(ť,Ĺ(ǀ,ǂ)
);public void ǧ(C ť,ƾ<ō,Color>ǀ,ƿ<ō,Color>ǂ=null)=>ı(ť,Ǧ(ǀ,ǂ));public void ƅ(C ť,ƾ<ō,Ǩ>ǀ,ƿ<ō,Ǩ>ǂ=null)=>ı(ť,ǩ(ǀ,ǂ));
public void Ń(C ť,Ņ ǟ,params ĸ<ō,Ņ>[]Ǫ)=>ı(ť,ǫ(ǟ,Ǫ));public void ĵ(C ť,Ĭ Ǭ,params ĸ<ō,Ĭ>[]Ǫ)=>ı(ť,ƚ(Ǭ,Ǫ));public Ɲ<ō>ǫ(Ņ ǟ,
params ĸ<ō,Ņ>[]Ǫ)=>ǭ(ǟ,ŵ=>ŵ.ƴ??ǟ,Ǫ);public Ɲ<ō>ƚ(Ĭ Ǭ,params ĸ<ō,Ĭ>[]Ǫ)=>ǭ(Ǭ,ŵ=>ŵ.ǥ?.ū().Ǯ??Ĭ.ǯ,Ǫ);public ĸ<ō,M>ĸ<M>(Ɲ<ō>ǰ,
params M[]Ǳ)=>new ĸ<ō,M>{Ǖ=ǰ,ǖ=Ǳ};Ɲ<ō>ǭ<M>(M ǲ,Func<Ų,M>ǳ,params ĸ<ō,M>[]Ǫ){var Ǵ=Ź<M,Ɲ<ō>>();foreach(ĸ<ō,M>Ǖ in Ǫ)foreach(M Ǘ
in Ǖ.ǖ)Ǵ.Add(Ǘ,Ǖ.Ǖ);return new ǔ<ō>(ŵ=>Ǵ.GetValueOrDefault(ǳ(ŵ),Ǵ[ǲ]));}public Ɲ<ō>Ļ(ƾ<ō,bool>ǀ,ƿ<ō,bool>ǂ=null)=>ǵ(ǀ,ǂ,ż,
true);public Ɲ<ō>ļ(ƾ<ō,string>ǀ,ƿ<ō,string>ǂ=null)=>ǵ(ǀ,ǂ,Ż,"");public Ɲ<ō>Ň(ƾ<ō,float>ǀ,ƿ<ō,float>ǂ=null,float ű=0)=>ǵ(ǀ,ǂ,
Ž,ű);public Ɲ<ō>Ĺ(ƾ<ō,Vector3D>ǀ,ƿ<ō,Vector3D>ǂ=null)=>ǵ(ǀ,ǂ,Ƕ,Vector3D.Zero);public Ɲ<ō>Ǧ(ƾ<ō,Color>ǀ,ƿ<ō,Color>ǂ=null)
=>ǵ(ǀ,ǂ,ž,new Color(10,10,10));public Ɲ<ō>ǩ(ƾ<ō,Ǩ>ǀ,ƿ<ō,Ǩ>ǂ=null)=>ǵ(ǀ,ǂ,Ƿ,ƈ());Ɲ<ō>ǵ<M>(ƾ<ō,M>ǀ,ƿ<ō,M>ǂ,Func<Ũ,M>Ǐ,M ű)=>
new Ǒ<ō,M>(ǀ,ǂ??((E,F)=>{}),Ǐ,ű);}public abstract class ǹ<ō>:A<ō>where ō:class{public override IEnumerable<ō>P<M>(List<M>N,
Func<M,bool>O=null)=>N.Where(O??(E=>true)).SelectMany(E=>Ǹ((IMyTerminalBlock)E));public abstract IEnumerable<ō>Ǹ(
IMyTerminalBlock K);}public class k:q<IMyCameraBlock>{public k(){G(C.Ŀ,E=>ŏ(E)!=Vector3D.Zero,(E,F)=>E.EnableRaycast=F);į(C.İ,Ǻ,(E,F)=>ś
(E,"Range",""+F),100);ı(Q(C.Ķ,C.ń),Ĺ(E=>Ŗ(ŗ(E,"Velocity"))??Vector3D.Zero));ƀ(C.Ķ,ŏ);ī[Ĭ.ķ]=C.Ķ;Ť[Ņ.ŉ]=C.İ;}public
Vector3D ŏ(IMyCameraBlock E){var ǻ=(double)Ǻ(E);E.EnableRaycast=true;if(E.CanScan(ǻ)){var Ǽ=E.Raycast(ǻ);if(Ǽ.IsEmpty()){ř(E,
"Target");}else{ś(E,"Target",Ŝ(ĺ(Ǽ)));ś(E,"Velocity",Ŝ(Ǽ.Velocity));}}return Ŗ(ŗ(E,"Target")??"")??Vector3D.Zero;}public float Ǻ
(IMyCameraBlock E)=>float.Parse(ŗ(E,"Range")??"1000");}public class m:ǹ<IMyInventory>{public m(){ı(C.ǽ,Ǿ);į(C.ţ,ǿ=>(float
)(ǿ.CurrentVolume.RawValue/(double)ǿ.MaxVolume.RawValue));į(C.İ,ǿ=>(float)ǿ.MaxVolume*1000);į(C.Š,ǿ=>(float)ǿ.
CurrentVolume*1000);į(C.Ȁ,ǿ=>(float)ǿ.CurrentMass);B(C.D,E=>ȁ(E).CustomName,(E,F)=>ȁ(E).CustomName=F);G(C.Ƃ,E=>ȁ(E).ShowInInventory,(
E,F)=>ȁ(E).ShowInInventory=F);ƅ(C.Ȃ,E=>{var ȃ=Q<MyInventoryItem>();E.GetItems(ȃ);return ƈ(ȃ.Select(Ǘ=>Ǘ.Type.TypeId+"."+Ǘ
.Type.SubtypeId).Distinct().Select(Ɖ));});ī[Ĭ.ŋ]=C.ţ;ī[Ĭ.Ľ]=C.D;ī[Ĭ.ĭ]=C.Ƃ;}public override string L(IMyInventory K)=>ȁ(K
).CustomName;public override IEnumerable<IMyInventory>Ǹ(IMyTerminalBlock K)=>Ȅ(0,K.InventoryCount).Select(K.GetInventory)
;Ɲ<IMyInventory>Ǿ=new Ɲ<IMyInventory>{ǀ=(E,ŵ)=>{var ȅ=Ż(ŵ.ƍ[0].Ǝ.ū());var Ȉ=á.Ȇ(á.ȇ(ȅ));double ȉ=0;var Ȋ=Q<
MyInventoryItem>();E.GetItems(Ȋ,Ȉ);Ȋ.ForEach(ȋ=>ȉ+=ȋ.Amount.RawValue);return ŷ((float)(ȉ/1000000));}};IMyTerminalBlock ȁ(IMyInventory Ȍ
)=>(IMyTerminalBlock)Ȍ.Owner;}public class e:q<IMyAssembler>{public e(){G(C.Ʈ,E=>E.Mode==MyAssemblerMode.Assembly,(E,F)=>
E.Mode=F?MyAssemblerMode.Assembly:MyAssemblerMode.Disassembly);G(C.ħ,E=>E.IsQueueEmpty,(E,F)=>{if(!F)E.ClearQueue();});G(
C.ƕ,E=>!E.IsQueueEmpty,(E,F)=>{if(!F)E.ClearQueue();});G(C.ĳ,E=>E.CooperativeMode,(E,F)=>E.CooperativeMode=F);ı(C.ȍ,new Ɲ
<IMyAssembler>(){ǀ=(E,ŵ)=>ŷ(E.Mode==MyAssemblerMode.Assembly&&Ȏ(E,ŵ)>=ȏ(ŵ,1f)),ǂ=(E,ŵ,F)=>{E.Mode=MyAssemblerMode.
Assembly;Ȑ(E,ŵ);}});ı(C.ȑ,new Ɲ<IMyAssembler>(){ǀ=(E,ŵ)=>ŷ(E.Mode==MyAssemblerMode.Disassembly&&Ȏ(E,ŵ)>=ȏ(ŵ,1f)),ǂ=(E,ŵ,F)=>{E.
Mode=MyAssemblerMode.Disassembly;Ȑ(E,ŵ);}});ı(C.ǽ,new Ɲ<IMyAssembler>(){ǀ=(E,F)=>ŷ(Ȏ(E,F)),ǂ=(E,ŵ,F)=>Ȑ(E,ŵ)});ƅ(C.Ȃ,E=>{var
Ȓ=Q<MyProductionItem>();E.GetQueue(Ȓ);return ƈ(Ȓ.Select(ȋ=>ȋ.BlueprintId.SubtypeId+"").Distinct().Select(Ɖ));});}ō ȏ<ō>(Ų
ǒ,ō ȓ){Object ũ=ǒ.ƍ.Where(ŵ=>ŵ.Ǝ!=null).Select(ŵ=>ŵ.Ǝ.ū()).Concat(Q(ǒ.ǥ?.ū()??ŷ(ȓ))).Concat(Q(ŷ(ȓ))).FirstOrDefault(F=>F.
Ǯ==ŷ(ȓ).Ǯ).ũ;return(ō)ũ;}float Ȏ(IMyAssembler E,Ų ŵ){var ȕ=á.Ȕ(ȏ(ŵ,"*"));var Ȓ=Q<MyProductionItem>();E.GetQueue(Ȓ);
MyFixedPoint ũ=Ȓ.Where(ȋ=>ȕ.Contains(ȋ.BlueprintId)).Select(ȋ=>ȋ.Amount).DefaultIfEmpty(MyFixedPoint.Zero).Aggregate((Ȗ,ȗ)=>Ȗ+ȗ);
return(float)ũ;}void Ȑ(IMyAssembler E,Ų ŵ){float Ș=ȏ(ŵ,1f);foreach(MyDefinitionId ș in á.Ȕ(ȏ(ŵ,"*"))){try{E.AddQueueItem(ș,(
MyFixedPoint)Ș);}catch(Exception){throw new Č("Unknown BlueprintId: "+ș.SubtypeId);}}}}public class Õ:q<IMyJumpDrive>{public Õ(){į(C
.Ƨ,E=>E.CurrentStoredPower);į(C.ţ,E=>E.CurrentStoredPower/E.MaxStoredPower);var Ț=Ļ(E=>E.Status==MyJumpDriveStatus.Ready)
;ı(C.ħ,Ț);ı(C.ț,Ț);G(C.Ʈ,E=>!E.Recharge,(E,F)=>E.Recharge=!F);var ȝ=ǫ(Ņ.Ȝ,ĸ(Ň(E=>E.JumpDistanceMeters,(E,F)=>E.
SetValueFloat("JumpDistance",100*(F-E.MinJumpDistanceMeters)/(E.MaxJumpDistanceMeters-E.MinJumpDistanceMeters))),Ņ.Ȝ),ĸ(Ň(E=>E.
MaxJumpDistanceMeters),Ņ.ŉ),ĸ(Ň(E=>E.MinJumpDistanceMeters),Ņ.Ŋ));ı(C.I,ȝ);ı(C.İ,ȝ);}}public class ý:q<IMyTimerBlock>{public ý(){G(C.Ŀ,E=>E.
IsCountingDown,(E,F)=>E.Trigger());G(C.Ȟ,E=>!E.Silent,(E,F)=>E.Silent=!F);į(C.İ,E=>E.TriggerDelay,(E,F)=>E.TriggerDelay=F,1);G(C.ȟ,E=>
E.IsCountingDown,(E,F)=>{if(F)E.StartCountdown();else E.StopCountdown();});}}public class î:q<IMyConveyorSorter>{public î
(){G(C.ĳ,(E)=>E.DrainAll,(E,F)=>E.DrainAll=F);}}public class Î<ō>:q<ō>where ō:class,IMyGyro{public Î(){var Ƞ=Ň(E=>E.
GyroPower,(E,F)=>E.GyroPower=F,0.1f);ı(C.İ,Ƞ);ı(C.Ƨ,Ƞ);G(C.ĳ,E=>!E.GyroOverride,(E,F)=>E.GyroOverride=!F);var Ȧ=ǫ(Ņ.Ȝ,ĸ(ƚ(Ĭ.ķ,ĸ(Ĺ
(E=>ȡ(-E.Pitch*Ȣ,E.Yaw*Ȣ,E.Roll*Ȣ),(E,F)=>{E.Pitch=ȣ*(float)-F.X;E.Yaw=ȣ*(float)F.Y;E.Roll=ȣ*(float)F.Z;}),Ĭ.ķ),ĸ(Ļ(E=>E.
GyroOverride,(E,F)=>E.GyroOverride=F),Ĭ.ĭ)),Ņ.Ȝ),ĸ(Ň(E=>Ȣ*-E.Pitch,(E,F)=>E.Pitch=ȣ*-F,5),Ņ.ŉ),ĸ(Ň(E=>Ȣ*E.Pitch,(E,F)=>E.Pitch=ȣ*F,5
),Ņ.Ŋ),ĸ(Ň(E=>Ȣ*-E.Yaw,(E,F)=>E.Yaw=ȣ*-F,5),Ņ.ň),ĸ(Ň(E=>Ȣ*E.Yaw,(E,F)=>E.Yaw=ȣ*F,5),Ņ.ņ),ĸ(Ň(E=>Ȣ*E.Roll,(E,F)=>E.Roll=ȣ*
F,5),Ņ.Ȥ),ĸ(Ň(E=>Ȣ*-E.Roll,(E,F)=>E.Roll=ȣ*-F,5),Ņ.ȥ));ı(C.ȧ,Ȧ);ı(C.Ŏ,Ȧ);ı(C.Ʋ,Ȧ);ī[Ĭ.ŋ]=C.Ƨ;ī[Ĭ.ķ]=C.ȧ;}}public class s:
À{public s(){var Ȩ=Ļ(E=>E.Status==MyShipConnectorStatus.Connected,(E,F)=>{if(F)E.Connect();else E.Disconnect();});ı(C.H,Ȩ
);ı(C.ư,Ȩ);į(C.ȩ,E=>E.PullStrength,(E,F)=>E.PullStrength=F,0.01f);var Ț=Ļ(E=>E.Status==MyShipConnectorStatus.Connectable)
;ı(C.ț,Ț);ı(Q(C.ț,C.H),Ț);ı(Q(C.ț,C.ư),Ț);ī[Ĭ.ŋ]=C.ȩ;}}public class À:q<IMyShipConnector>{public À(){G(C.ĳ,E=>E.ThrowOut,
(E,F)=>E.ThrowOut=F);G(C.Ʈ,E=>!E.CollectAll,(E,F)=>E.CollectAll=!F);}}public class ª:q<IMyDoor>{public ª(){G(C.Ȫ,(E)=>E.
Status!=DoorStatus.Closed,(E,F)=>{if(F)E.OpenDoor();else E.CloseDoor();});į(C.ţ,E=>E.OpenRatio);ī[Ĭ.ŋ]=C.ţ;Ť[Ņ.ŉ]=C.ţ;}}public
class Ä:q<IMyGasGenerator>{public Ä(){G(C.ĳ,(E)=>E.AutoRefill,(E,F)=>E.AutoRefill=F);}}public class ù:q<IMyGasTank>{public ù(
){G(C.Ʈ,E=>!E.Stockpile,(E,F)=>E.Stockpile=!F);G(C.ĳ,E=>E.AutoRefillBottles,(E,F)=>E.AutoRefillBottles=F);į(C.İ,E=>E.
Capacity);į(C.ţ,E=>(float)E.FilledRatio);į(C.I,E=>(float)(E.FilledRatio*E.Capacity));ī[Ĭ.ŋ]=C.ţ;Ť.Add(Ņ.ŉ,C.ţ);}}public class È:
q<IMyGravityGeneratorSphere>{public È(){var Ư=Ň(E=>E.Radius,(E,F)=>E.Radius=F,25);ı(C.İ,Ư);ı(C.ş,Ư);ı(C.I,Ư);į(C.ȩ,E=>E.
GravityAcceleration,(E,F)=>E.GravityAcceleration=F,0.25f);ī[Ĭ.ŋ]=C.ȩ;}}public class Æ:q<IMyGravityGenerator>{public Æ(){į(C.ȩ,E=>E.
GravityAcceleration,(E,F)=>E.GravityAcceleration=F,0.25f);var Ư=ǫ(Ņ.Ȝ,ĸ(ƚ(Ĭ.ķ,ĸ(Ĺ(E=>E.FieldSize,(E,F)=>E.FieldSize=F),Ĭ.ķ),ĸ(Ň(E=>E.
FieldSize.Length(),(E,F)=>E.FieldSize=ȡ(F,F,F),25),Ĭ.ŋ)),Ņ.Ȝ),ĸ(Ň(E=>E.FieldSize.Y,(E,F)=>E.FieldSize=ȡ(E.FieldSize.X,F,E.
FieldSize.Z)),Ņ.ŉ,Ņ.Ŋ),ĸ(Ň(E=>E.FieldSize.X,(E,F)=>E.FieldSize=ȡ(F,E.FieldSize.Y,E.FieldSize.Z)),Ņ.ň,Ņ.ņ),ĸ(Ň(E=>E.FieldSize.Z,(E
,F)=>E.FieldSize=ȡ(E.FieldSize.X,E.FieldSize.Y,F)),Ņ.Ɛ,Ņ.Ƒ));ı(C.İ,Ư);ı(C.I,Ư);ī[Ĭ.ŋ]=C.ȩ;ī[Ĭ.ķ]=C.İ;}}public class Ì<ō>:
q<ō>where ō:class,IMyUserControllableGun{public Ì(){G(C.Ŀ,(E)=>E.IsShooting,(E,F)=>E.Shoot=F);}}public class Ü:q<
IMyLandingGear>{public Ü(){G(C.ĳ,E=>E.AutoLock,(E,F)=>E.AutoLock=F);var ȫ=Ļ(E=>E.IsLocked,(E,F)=>{if(F)E.Lock();else E.Unlock();});ı(C
.H,ȫ);ı(C.ư,ȫ);var Ț=Ļ(E=>E.LockMode==LandingGearMode.ReadyToLock);ı(C.ț,Ț);ı(Q(C.ț,C.H),Ț);ı(Q(C.ț,C.ư),Ț);}}public
class ò:ƪ<IMyFunctionalBlock>{public ò():base(Ó("Searchlight")){ı(C.ŝ,Ĳ("Color",Color.Black));ı(C.ş,Ĳ("Radius",10));ı(C.İ,Ĳ(
"Range",100));ı(C.Ȭ,Ĳ("Blink Interval",0.1f));ı(C.I,Ĳ("Blink Lenght",0.1f));ı(C.Ţ,Ĳ("Offset",0.1f));ı(C.Š,Ĳ("Intensity",1f));ı(
C.š,Ĳ("Falloff",0.5f));ı(C.Ŏ,Ĳ("EnableIdleMovement",true));ı(C.H,Ĳ("EnableTargetLocking",true));ī[Ĭ.ŝ]=C.ŝ;ī[Ĭ.ŋ]=C.İ;Ť.
Add(Ņ.ŉ,C.İ);}}public class Ú:q<IMyLightingBlock>{public Ú(){ǧ(C.ŝ,E=>E.Color,(E,F)=>E.Color=F);var Ş=Ň(E=>E.Radius,(E,F)=>
E.Radius=F,3);ı(C.İ,Ş);ı(C.ş,Ş);į(C.Ȭ,E=>E.BlinkIntervalSeconds,(E,F)=>E.BlinkIntervalSeconds=F,0.1f);į(C.I,E=>E.
BlinkLength,(E,F)=>E.BlinkLength=F,0.1f);į(C.Ţ,E=>E.BlinkOffset,(E,F)=>E.BlinkOffset=F,0.1f);į(C.Š,E=>E.Intensity,(E,F)=>E.
Intensity=F,1f);į(C.š,E=>E.Falloff,(E,F)=>E.Falloff=F,0.5f);ī[Ĭ.ŝ]=C.ŝ;ī[Ĭ.ŋ]=C.İ;Ť.Add(Ņ.ŉ,C.İ);}}public class Þ:q<
IMyShipMergeBlock>{public Þ(){var ȭ=Ļ(E=>E.IsConnected,(E,F)=>E.Enabled=F);ı(C.H,ȭ);ı(C.ư,ȭ);}}public class w:q<IMyOreDetector>{public w(
){var Ư=Ĳ("Range",50);ı(C.İ,Ư);ı(C.ş,Ư);G(C.Ʈ,E=>E.BroadcastUsingAntennas,(E,F)=>E.BroadcastUsingAntennas=F);ī[Ĭ.ŋ]=C.İ;Ť
.Add(Ņ.ŉ,C.İ);}}public class à:q<IMyParachute>{public à(){var Ȯ=Ļ(E=>E.Status!=DoorStatus.Closed,(E,F)=>{if(F)E.OpenDoor(
);else E.CloseDoor();});ı(C.Ȫ,Ȯ);ı(C.Ŀ,Ȯ);ı(C.ĳ,Ĳ("AutoDeploy",true));ı(C.İ,Ĳ("AutoDeployHeight",500));į(C.ţ,E=>1-E.
OpenRatio);ƀ(C.ń,E=>E.GetVelocity());ƀ(C.ȩ,E=>E.GetTotalGravity());ı(Q(C.ȯ,C.ȩ),Ĺ(E=>E.GetNaturalGravity()));ı(Q(C.Ȱ,C.ȩ),Ĺ(E=>E.
GetArtificialGravity()));į(C.I,E=>{Vector3D?ȱ;return(float)(E.TryGetClosestPoint(out ȱ)?(ȱ.Value-E.GetPosition()).Length():-1);});ī[Ĭ.ŋ]=C.I
;Ť.Add(Ņ.ŉ,C.ţ);}}public class ä:q<IMyPistonBase>{public ä(){Ń(C.İ,Ņ.ŉ,ĸ(Ň(E=>E.MaxLimit,(E,F)=>E.MaxLimit=F,1),Ņ.ŉ,Ņ.Ɛ),
ĸ(Ň(E=>E.MinLimit,(E,F)=>E.MinLimit=F,1),Ņ.Ŋ,Ņ.Ƒ));ı(C.I,new Ȳ());į(C.ń,E=>E.Velocity,(E,F)=>E.Velocity=F,1);G(C.ư,E=>E.
IsAttached,(E,F)=>{if(F)E.Attach();else E.Detach();});ī[Ĭ.ŋ]=C.I;Ť[Ņ.ŉ]=C.I;Ť[Ņ.Ŋ]=C.I;}}public class Ȳ:Ǒ<IMyPistonBase,float>{
public Ȳ():base(E=>E.CurrentPosition,ȳ,Ž,1){Ǉ=(E,ŵ,ơ)=>{if(ơ==Ņ.ŉ)E.Extend();if(ơ==Ņ.Ŋ)E.Retract();};ǈ=(E,ŵ)=>E.Reverse();}}
static void ȳ(IMyPistonBase ȴ,float ũ){if(ȴ.CurrentPosition<ũ){ȴ.MaxLimit=ũ;ȴ.Extend();}else{ȴ.MinLimit=ũ;ȴ.Retract();}}public
class â:q<IMyProgrammableBlock>{public â(){G(C.ħ,K=>!K.IsRunning);B(C.Ʊ,K=>K.TerminalRunArgument);ĵ(C.ƭ,Ĭ.Ľ,ĸ(ļ(E=>E.
IsRunning.ToString(),(E,F)=>E.TryRun(F)),Ĭ.Ľ),ĸ(Ļ(E=>E.IsRunning,(E,F)=>{if(F)E.TryRun(E.TerminalRunArgument);else E.Enabled=
false;}),Ĭ.ĭ));ī[Ĭ.Ľ]=C.ƭ;}}public class æ:q<IMyProjector>{public æ(){G(C.ħ,E=>E.RemainingBlocks==0);į(C.ţ,E=>1-E.
RemainingBlocks/(float)E.TotalBlocks);G(C.Ƃ,E=>E.IsProjecting,(E,F)=>E.ShowOnlyBuildable=!F);ı(C.H,Ĳ("KeepProjection",""));ı(C.I,Ĳ(
"Scale",0.1f));Ń(C.Ŏ,Ņ.Ȝ,ĸ(Ĺ(E=>ȵ(E),(E,F)=>E.ProjectionRotation=ȡ(ȶ(F.X),ȶ(F.Y),ȶ(F.Z))),Ņ.Ȝ),ĸ(Ň(E=>ȵ(E).X,(E,F)=>ȷ(E,ȡ(0,1,1
),ȡ(F,0,0))),Ņ.ŉ),ĸ(Ň(E=>-ȵ(E).X,(E,F)=>ȷ(E,ȡ(0,1,1),ȡ(-F,0,0))),Ņ.Ŋ),ĸ(Ň(E=>ȵ(E).Y,(E,F)=>ȷ(E,ȡ(1,0,1),ȡ(0,F,0))),Ņ.ņ),ĸ
(Ň(E=>-ȵ(E).Y,(E,F)=>ȷ(E,ȡ(1,0,1),ȡ(0,-F,0))),Ņ.ň),ĸ(Ň(E=>ȵ(E).Z,(E,F)=>ȷ(E,ȡ(1,1,0),ȡ(0,0,F))),Ņ.Ȥ),ĸ(Ň(E=>-ȵ(E).Z,(E,F)
=>ȷ(E,ȡ(1,1,0),ȡ(0,0,-F))),Ņ.ȥ));Ń(C.Ţ,Ņ.Ȝ,ĸ(Ĺ(E=>ȸ(E),(E,F)=>E.ProjectionOffset=new Vector3I(F)),Ņ.Ȝ),ĸ(Ň(E=>ȸ(E).X,(E,F)
=>ȹ(E,ȡ(0,1,1),ȡ(F,0,0))),Ņ.ņ),ĸ(Ň(E=>-ȸ(E).X,(E,F)=>ȹ(E,ȡ(0,1,1),ȡ(-F,0,0))),Ņ.ň),ĸ(Ň(E=>ȸ(E).Y,(E,F)=>ȹ(E,ȡ(1,0,1),ȡ(0,F
,0))),Ņ.ŉ),ĸ(Ň(E=>-ȸ(E).Y,(E,F)=>ȹ(E,ȡ(1,0,1),ȡ(0,-F,0))),Ņ.Ŋ),ĸ(Ň(E=>ȸ(E).Z,(E,F)=>ȹ(E,ȡ(1,1,0),ȡ(0,0,F))),Ņ.Ɛ),ĸ(Ň(E=>-
ȸ(E).Z,(E,F)=>ȹ(E,ȡ(1,1,0),ȡ(0,0,-F))),Ņ.Ƒ));}void ȷ(IMyProjector Ⱥ,Vector3I Ȼ,Vector3I ȼ)=>Ⱥ.ProjectionRotation=ȵ(Ⱥ)*Ȼ+ȡ
(ȶ(ȼ.X),ȶ(ȼ.Y),ȶ(ȼ.Z));Vector3I ȵ(IMyProjector Ⱥ)=>Ⱥ.ProjectionRotation;void ȹ(IMyProjector Ⱥ,Vector3I Ȼ,Vector3I ȼ)=>Ⱥ.
ProjectionOffset=ȸ(Ⱥ)*Ȼ+ȼ;Vector3I ȸ(IMyProjector Ⱥ)=>Ⱥ.ProjectionOffset;Vector3I ȡ(float ǿ,float Ƚ,float Ⱦ)=>new Vector3I(ǿ,Ƚ,Ⱦ);float
ȶ(double ũ)=>(float)(ũ>2?2:ũ<-2?-2:ũ);}public class Ò:ƪ<IMyMotorStator>{public Ò(Func<IMyFunctionalBlock,bool>Ȉ):base(Ȉ){
ı(C.ľ,new ȿ());var ɀ=ǫ(Ņ.Ȝ,ĸ(Ļ(E=>E.UpperLimitDeg<361||E.LowerLimitDeg>-361,(E,F)=>{if(!F){E.UpperLimitDeg=361;E.
LowerLimitDeg=-361;};}),Ņ.Ȝ),ĸ(Ļ(E=>E.UpperLimitDeg<361,(E,F)=>{if(!F)E.UpperLimitDeg=361;}),Ņ.ŉ,Ņ.Ɛ,Ņ.Ȥ),ĸ(Ļ(E=>E.LowerLimitDeg>-361
,(E,F)=>{if(!F)E.LowerLimitDeg=-361;}),Ņ.Ŋ,Ņ.Ƒ,Ņ.ȥ));ĵ(C.İ,Ĭ.ŋ,ĸ(ǫ(Ņ.ŉ,ĸ(Ň(E=>E.UpperLimitDeg,(E,F)=>E.UpperLimitDeg=F,10
),Ņ.ŉ,Ņ.Ɛ,Ņ.Ȥ),ĸ(Ň(E=>E.LowerLimitDeg,(E,F)=>E.LowerLimitDeg=F,10),Ņ.Ŋ,Ņ.Ƒ,Ņ.ȥ)),Ĭ.ŋ),ĸ(ɀ,Ĭ.ĭ));ı(Q(C.İ,C.Ɯ),ɀ);į(C.ń,(E)
=>E.TargetVelocityRPM,(E,F)=>E.TargetVelocityRPM=F,1);į(C.I,(E)=>E.Displacement,(E,F)=>E.Displacement=F,0.1f);G(C.ư,E=>E.
IsAttached,(E,F)=>{if(F)E.Attach();else E.Detach();});G(C.H,E=>E.RotorLock,(E,F)=>E.RotorLock=F);į(C.ȩ,E=>E.Torque,(E,F)=>E.Torque
=F,1000);ī[Ĭ.ŋ]=C.ľ;Ť.Add(Ņ.ŉ,C.I);Ť.Add(Ņ.Ŋ,C.I);Ť.Add(Ņ.Ȥ,C.ľ);Ť.Add(Ņ.ȥ,C.ľ);ǟ=Ņ.Ȥ;}}public class ȿ:Ɲ<IMyMotorStator>{
public ȿ(){ǀ=(E,ŵ)=>ŷ(E.Angle*Œ);ǁ=(E,ŵ,ơ)=>ǀ(E,ŵ);ǂ=(E,ŵ,F)=>Ɂ(E,F);ǃ=(E,ŵ,ơ,F)=>Ɂ(E,F,ơ);ǆ=(E,ŵ,ơ,F)=>{if(ơ==Ņ.Ȥ||ơ==Ņ.ŉ)Ɂ(E
,ǀ(E,ŵ).ǋ(F),ơ);if(ơ==Ņ.ȥ||ơ==Ņ.Ŋ)Ɂ(E,ǀ(E,ŵ).ɂ(F),ơ);};ǅ=(E,ŵ,F)=>ǆ(E,ŵ,Ņ.Ȥ,F);Ǆ=(E,ŵ)=>ǅ(E,ŵ,ŷ(10));Ǉ=(E,ŵ,ơ)=>{if(ơ==Ņ.
Ȥ)E.TargetVelocityRPM=Math.Abs(E.TargetVelocityRPM);if(ơ==Ņ.ȥ)E.TargetVelocityRPM=-Math.Abs(E.TargetVelocityRPM);};ǈ=(E,ŵ
)=>E.TargetVelocityRPM*=-1;}}static void Ɂ(IMyMotorStator Ƀ,Ũ Ʉ){if(Ʉ.Ǯ!=Ĭ.ŋ){throw new Č(
"Cannot rotate rotor to non-numeric value: "+Ʉ);}float ũ=Ž(Ʉ);float Ɇ=Ʌ(ũ);if(Ƀ.Angle*Œ<ũ){Ƀ.UpperLimitDeg=Ɇ;Ƀ.TargetVelocityRPM=Math.Abs(Ƀ.TargetVelocityRPM);}else
{Ƀ.LowerLimitDeg=Ɇ;Ƀ.TargetVelocityRPM=-Math.Abs(Ƀ.TargetVelocityRPM);}}static void Ɂ(IMyMotorStator Ƀ,Ũ Ʉ,Ņ ƴ){if(Ʉ.Ǯ!=Ĭ
.ŋ){throw new Č("Cannot rotate rotor to non-numeric value: "+Ʉ);}float ũ=Ʌ(Ž(Ʉ));float ɇ=Ƀ.Angle*œ;switch(ƴ){case Ņ.Ȥ:if(
ũ<ɇ)ũ=Ʌ(ũ+360);Ƀ.UpperLimitDeg=ũ;Ƀ.TargetVelocityRPM=Math.Abs(Ƀ.TargetVelocityRPM);break;case Ņ.ȥ:if(ũ>ɇ)ũ=Ʌ(ũ-360);Ƀ.
LowerLimitDeg=ũ;Ƀ.TargetVelocityRPM=-Math.Abs(Ƀ.TargetVelocityRPM);break;default:Ɂ(Ƀ,Ʉ);break;}}static float Ʌ(float Ɉ){float ɉ=Ɉ;if(
ɉ>360)ɉ%=360;else if(ɉ<-360)ɉ=-((-ɉ)%360);return ɉ;}public class ô:q<IMySensorBlock>{public ô(){G(C.Ŀ,E=>E.IsActive);G(C.
Ȟ,E=>E.PlayProximitySound,(E,F)=>E.PlayProximitySound=F);ƀ(C.Ķ,E=>{var Ɋ=E.LastDetectedEntity;return Ɋ.IsEmpty()?Vector3D
.Zero:ĺ(Ɋ);});ı(Q(C.Ķ,C.ń),Ĺ(E=>{var Ɋ=E.LastDetectedEntity;return Ɋ.IsEmpty()?Vector3.Zero:Ɋ.Velocity;}));ī[Ĭ.ķ]=C.Ķ;}}
public class o<ō>:ɋ<ō>where ō:class,IMyCockpit{public o(){į(C.ţ,E=>E.OxygenFilledRatio);į(C.İ,E=>E.OxygenCapacity);ı(C.Ķ,Ĳ(
"TargetLocking",true));}}public class é:ɋ<IMyRemoteControl>{public é(){var Ʀ=Ļ(E=>E.IsAutoPilotEnabled,(E,F)=>E.SetAutoPilotEnabled(F))
;ı(C.ĳ,Ʀ);ı(C.ƭ,Ʀ);į(C.İ,E=>E.SpeedLimit,(E,F)=>E.SpeedLimit=F,10);ı(C.ư,Ĳ("DockingMode",true));ƀ(C.Ķ,E=>E.
CurrentWaypoint.Coords,(E,F)=>Ɍ(E,Ƿ(ŷ(F))));ƅ(C.ɍ,E=>{var Ɏ=Q<MyWaypointInfo>();E.GetWaypointInfo(Ɏ);return ƈ(Ɏ.Select(ɏ=>new ɐ(Ɖ(ɏ.
Name),Ɖ(ɏ.Coords))));},Ɍ);ī[Ĭ.ĭ]=C.ĳ;ī[Ĭ.ķ]=C.Ķ;ī[Ĭ.ɑ]=C.ɍ;}void Ɍ(IMyRemoteControl E,Ǩ Ɏ){E.ClearWaypoints();for(int ǿ=0;ǿ<
Ɏ.ɒ.Count;ǿ++){ɐ ũ=Ɏ.ɒ[ǿ];E.AddWaypoint(new MyWaypointInfo(ũ.ɓ()?ũ.ɔ():"Waypoint "+(ǿ+1),Ƕ(ũ.ū())));}}}public class ɋ<ō>:
û<ō>where ō:class,IMyShipController{public ɋ(){var ɕ=Ļ(E=>E.DampenersOverride,(E,F)=>E.DampenersOverride=F);var Ʀ=Ļ(E=>E.
IsMainCockpit,(E,F)=>E.IsMainCockpit=F);ı(C.Ɯ,Ʀ);ı(C.Ƨ,Ʀ);ı(C.ȧ,ɕ);ı(C.ĳ,ɕ);G(C.H,E=>E.HandBrake,(E,F)=>E.HandBrake=F);ƀ(C.ȩ,E=>E.
GetTotalGravity());ı(Q(C.ȯ,C.ȩ),Ĺ(E=>E.GetNaturalGravity()));ı(Q(C.Ȱ,C.ȩ),Ĺ(E=>E.GetArtificialGravity()));į(C.Ȁ,E=>E.CalculateShipMass(
).TotalMass);į(C.I,E=>ɖ(E,MyPlanetElevation.Surface));į(C.Ŕ,E=>ɖ(E,MyPlanetElevation.Sealevel));į(C.Ȁ,E=>E.
CalculateShipMass().TotalMass);G(C.Ĵ,E=>E.IsUnderControl);Ń(C.ń,Ņ.Ȝ,ĸ(ƚ(Ĭ.ķ,ĸ(Ĺ(E=>ɗ(E)),Ĭ.ķ),ĸ(Ň(E=>(float)ɗ(E).Length(),(E,F)=>(E as
IMyRemoteControl).SpeedLimit=F),Ĭ.ŋ)),Ņ.Ȝ),ĸ(Ň(E=>(float)ɗ(E).Y),Ņ.ŉ),ĸ(Ň(E=>(float)-ɗ(E).Y),Ņ.Ŋ),ĸ(Ň(E=>(float)-ɗ(E).X),Ņ.ň),ĸ(Ň(E=>(
float)ɗ(E).X),Ņ.ņ),ĸ(Ň(E=>(float)-ɗ(E).Z),Ņ.Ɛ),ĸ(Ň(E=>(float)ɗ(E).Z),Ņ.Ƒ));Ń(C.Ʋ,Ņ.Ȝ,ĸ(Ĺ(E=>E.MoveIndicator*new Vector3(1,1,-
1)),Ņ.Ȝ),ĸ(Ň(E=>E.MoveIndicator.Y),Ņ.ŉ),ĸ(Ň(E=>-E.MoveIndicator.Y),Ņ.Ŋ),ĸ(Ň(E=>-E.MoveIndicator.X),Ņ.ň),ĸ(Ň(E=>E.
MoveIndicator.X),Ņ.ņ),ĸ(Ň(E=>-E.MoveIndicator.Z),Ņ.Ɛ),ĸ(Ň(E=>E.MoveIndicator.Z),Ņ.Ƒ));Ń(C.Ŏ,Ņ.Ȝ,ĸ(Ĺ(E=>new Vector3D(E.
RotationIndicator/new Vector2(-9,9),E.RollIndicator)),Ņ.Ȝ),ĸ(Ň(E=>-E.RotationIndicator.X/9),Ņ.ŉ),ĸ(Ň(E=>E.RotationIndicator.X/9),Ņ.Ŋ),ĸ(Ň
(E=>-E.RotationIndicator.Y/9),Ņ.ň),ĸ(Ň(E=>E.RotationIndicator.Y/9),Ņ.ņ),ĸ(Ň(E=>-E.RollIndicator),Ņ.ȥ),ĸ(Ň(E=>E.
RollIndicator),Ņ.Ȥ));ī[Ĭ.ŋ]=C.ń;Ť[Ņ.ŉ]=C.ń;}Vector3D ɗ(ō K)=>Vector3D.TransformNormal(K.GetShipVelocities().LinearVelocity,MatrixD.
Transpose(K.WorldMatrix));float ɖ(ō K,MyPlanetElevation Ǘ){double ɘ;return(float)(K.TryGetPlanetElevation(Ǘ,out ɘ)?ɘ:-1);}}public
class ð:q<IMySoundBlock>{public ð(){į(C.Š,E=>E.Volume,(E,F)=>E.Volume=F,0.1f);var Ư=Ň(E=>E.Range,(E,F)=>E.Range=F,50);ı(C.İ,Ư
);ı(C.ş,Ư);į(C.I,E=>E.LoopPeriod,(E,F)=>E.LoopPeriod=F,10);ı(C.Ȟ,ƚ(Ĭ.Ľ,ĸ(Ļ((E)=>E.DetailedInfo.Contains("Loop timer"),(E,
F)=>{if(F)E.Play();else E.Stop();}),Ĭ.ĭ),ĸ(ļ(E=>E.SelectedSound,(E,F)=>E.SelectedSound=F),Ĭ.Ľ)));ƅ(C.ə,E=>{var ɚ=Q<string
>();E.GetSounds(ɚ);return ƈ(ɚ.Select(Ɖ));});ī[Ĭ.Ľ]=C.Ȟ;ī[Ĭ.ŋ]=C.Š;}}public class y:ǹ<IMyTextSurface>{public y(){var ɛ=Ň(E
=>E.FontSize,(E,F)=>E.FontSize=F,0.5f);var ɜ=Ǧ(E=>E.ContentType==ContentType.SCRIPT?E.ScriptForegroundColor:E.FontColor,(E
,F)=>{if(E.ContentType==ContentType.SCRIPT)E.ScriptForegroundColor=F;else E.FontColor=F;});B(C.D,E=>L(E));G(C.Ɯ,E=>E.
ContentType!=ContentType.NONE,(E,F)=>E.ContentType=F?ContentType.TEXT_AND_IMAGE:ContentType.NONE);B(C.Ʊ,E=>{var ɝ=new StringBuilder
();E.ReadText(ɝ);return ɝ.ToString();},(E,F)=>{E.ContentType=ContentType.TEXT_AND_IMAGE;E.WriteText(F);});B(C.Ȟ,E=>E.
CurrentlyShownImage??"",(E,F)=>ɞ(E,Ƿ(ŷ(F))));B(C.ƭ,E=>E.Script??"",(E,F)=>{E.ContentType=ContentType.SCRIPT;E.Script=F;});į(C.Ţ,E=>E.
TextPadding,(E,F)=>E.TextPadding=F,1);G(C.ţ,E=>E.PreserveAspectRatio,(E,F)=>E.PreserveAspectRatio=F);ƅ(C.ə,E=>{var ɟ=Q<string>();E.
GetSelectedImages(ɟ);return ƈ(ɟ.Select(Ɖ));},ɞ);B(C.Ɓ,E=>(E.Alignment+"").ToLower(),(E,F)=>E.Alignment=F=="center"?TextAlignment.CENTER:F
=="right"?TextAlignment.RIGHT:TextAlignment.LEFT);į(C.Ȭ,E=>E.ChangeInterval,(E,F)=>E.ChangeInterval=F,1);ı(C.I,ɛ);ı(C.ŝ,ɜ)
;ǧ(C.ɠ,E=>E.ContentType==ContentType.SCRIPT?E.ScriptBackgroundColor:E.BackgroundColor,(E,F)=>{if(E.ContentType==
ContentType.SCRIPT)E.ScriptBackgroundColor=F;else E.BackgroundColor=F;});ĵ(C.ɡ,Ĭ.Ľ,ĸ(ļ(E=>E.Font,(E,F)=>E.Font=F),Ĭ.Ľ),ĸ(ɛ,Ĭ.ŋ),ĸ(ɜ
,Ĭ.ŝ));ī[Ĭ.ĭ]=C.Ɯ;ī[Ĭ.Ľ]=C.Ʊ;ī[Ĭ.ŝ]=C.ŝ;ī[Ĭ.ŋ]=C.I;ī[Ĭ.ɑ]=C.ə;Ť[Ņ.ŉ]=C.Ʊ;}void ɞ(IMyTextSurface K,Ǩ ɟ){K.ContentType=
ContentType.TEXT_AND_IMAGE;K.ClearImagesFromSelection();K.AddImagesToSelection(ɟ.ɒ.Select(ǿ=>Ż(ǿ.ū())).ToList());}public override
string L(IMyTextSurface K)=>K.DisplayName;public override IEnumerable<IMyTextSurface>Ǹ(IMyTerminalBlock K)=>K is
IMyTextSurface?ĥ((IMyTextSurface)K):K is IMyTextSurfaceProvider?Ȅ(0,((IMyTextSurfaceProvider)K).SurfaceCount).Select(((
IMyTextSurfaceProvider)K).GetSurface):ɢ<IMyTextSurface>();}public class ā:q<IMyThrust>{public ā(){var ɣ=Ň(E=>E.CurrentThrust,(E,F)=>E.
ThrustOverride=F,5000);ı(C.I,ɣ);ı(C.Š,ɣ);į(C.İ,E=>E.MaxThrust,(E,F)=>E.ThrustOverride=F,5000);į(C.ţ,E=>E.ThrustOverridePercentage,(E,F
)=>E.ThrustOverridePercentage=F,0.1f);į(C.ȧ,E=>E.ThrustOverride,(E,F)=>E.ThrustOverride=F,5000);Ť[Ņ.ŉ]=C.İ;ī[Ĭ.ŋ]=C.I;}}
public class Ĉ:û<IMyWarhead>{public Ĉ(){G(C.Ŀ,E=>E.IsCountingDown,(E,F)=>E.Detonate());G(C.Ɯ,E=>E.IsArmed,(E,F)=>E.IsArmed=F);
į(C.İ,E=>E.DetonationTime,(E,F)=>E.DetonationTime=F,1);G(C.ȟ,E=>E.IsCountingDown,(E,F)=>{if(F)E.StartCountdown();else E.
StopCountdown();});}}public class ö:q<IMyMotorSuspension>{public ö(){į(C.I,E=>E.Height,(E,F)=>E.Height=F,0.1f);į(C.ľ,E=>(float)(-E.
SteerAngle*Œ),(E,F)=>{E.SteeringOverride=(float)(F*Math.PI/144);E.MaxSteerAngle=1;},.1f);G(C.H,E=>E.Brake,(E,F)=>E.Brake=F);G(C.ư,
E=>E.IsAttached,(E,F)=>{if(F)E.Attach();else E.Detach();});ı(Q(C.ń,C.İ),Ĳ("Speed Limit",5));ı(Q(C.ɤ,C.İ),Ň(E=>E.
MaxSteerAngle,(E,F)=>E.MaxSteerAngle=F,0.1f));į(C.ń,E=>E.PropulsionOverride,(E,F)=>E.PropulsionOverride=F,0.1f);ı(Q(C.ɥ,C.ń),Ļ(E=>E.
InvertPropulsion,(E,F)=>E.InvertPropulsion=F));ı(Q(C.ɥ,C.ɤ),Ļ(E=>E.InvertSteer,(E,F)=>E.InvertSteer=F));ı(Q(C.ɤ,C.ȧ),Ň(E=>E.
SteeringOverride,(E,F)=>E.SteeringOverride=F,0.1f));G(C.ɤ,E=>E.Steering,(E,F)=>E.Steering=F);į(C.ȩ,E=>E.Strength,(E,F)=>E.Strength=F,10)
;į(C.Ƨ,E=>E.Power,(E,F)=>E.Power=F,10);į(C.ţ,E=>E.Friction,(E,F)=>E.Friction=F,10);ī[Ĭ.ŋ]=C.I;Ť[Ņ.ŉ]=C.I;Ť[Ņ.Ŋ]=C.I;}}
public class Â<ō>:ƪ<ō>where ō:class,IMyPowerProducer{public Â(string Ʃ=""):base(Ó(Ʃ)){į(C.ţ,E=>E.CurrentOutput/E.MaxOutput);į(
C.İ,E=>E.MaxOutput);į(C.Š,E=>E.CurrentOutput);}}interface ɪ{void ɦ();bool Ŭ(object ŵ);bool ɧ();bool ɨ(object ŵ);bool ɩ(
object ŵ);}class ɰ<ō>:ɪ{ō ũ;public bool ɫ,ɬ,ɭ;public virtual ō ū()=>ũ;public virtual bool Ŭ(object ŵ){var ɮ=ũ==null&&ŵ is ō;ũ=
ɮ?(ō)ŵ:ũ;return ɮ;}public bool ɨ(object ɯ)=>ɫ&&Ŭ(ɯ);public bool ɩ(object ɯ)=>ɬ&&Ŭ(ɯ);public virtual bool ɧ()=>ũ!=null;
public virtual void ɦ()=>ũ=default(ō);}static ɰ<ō>ɱ<ō>()=>ɭ<ō>(false,true);static ɰ<ō>ɲ<ō>()=>ɭ<ō>(true,false);static ɰ<ō>ɳ<ō>
()=>ɭ<ō>(true,true);static ɰ<ō>ɭ<ō>(bool ɫ,bool ɬ)=>new ɰ<ō>{ɫ=ɫ,ɬ=ɬ};class ɴ<ō>:ɰ<ō>{public override bool ɧ()=>true;}
static ɴ<ō>ɶ<ō>()=>ɵ<ō>(false,true);static ɴ<ō>ɷ<ō>()=>ɵ<ō>(true,false);static ɴ<ō>ɸ<ō>()=>ɵ<ō>(true,true);static ɴ<ō>ɵ<ō>(
bool ɫ,bool ɬ)=>new ɴ<ō>{ɫ=ɫ,ɬ=ɬ};class ɹ<ō>:ɰ<List<ō>>{List<ō>Ǳ=Q<ō>();public override bool Ŭ(object ŵ){if(ŵ is ō)Ǳ.Add((ō)
ŵ);return ŵ is ō;}public override bool ɧ()=>!ɭ||Ǳ.Count>0;public override List<ō>ū()=>Ǳ;public override void ɦ()=>Ǳ.Clear
();}static ɹ<ō>ɻ<ō>(bool ɭ)=>ɺ<ō>(false,true,ɭ);static ɹ<ō>ɼ<ō>(bool ɭ)=>ɺ<ō>(true,true,ɭ);static ɹ<ō>ɺ<ō>(bool ɫ,bool ɬ,
bool ɭ)=>new ɹ<ō>{ɫ=ɫ,ɬ=ɬ,ɭ=ɭ};static bool ɾ(params ɪ[]ɽ)=>ɽ.All(ŵ=>ŵ.ɧ());static bool ɿ(params ɪ[]ɽ)=>ɽ.Any(ŵ=>ŵ.ɧ());
ILookup<Type,ʀ>ʁ;List<ʀ>ϯ=Q<ʀ>(new ʂ(),new ʃ(),ʄ(ʆ<ʅ>,ɱ<ʇ>(),ɶ<ʈ>(),ɶ<ʉ>(),(ŵ,O,ċ,ʊ)=>new ʋ(new ʌ(ʍ(ċ?.ũ,ʊ),new ʎ(O.ũ)))),ʄ(ʆ<ʅ
>,ɱ<ʏ>(),ɶ<ʈ>(),ɶ<ʉ>(),(ŵ,O,ċ,ʊ)=>new ʋ(new ʌ(ʍ(ċ?.ũ,ʊ),O.ũ))),ʐ(ʆ<ʇ>,ɱ<ʈ>(),ɶ<ʉ>(),(ŵ,ċ,ʊ)=>new ʋ(new ʌ(ʍ(ċ.ũ,ʊ),ŵ.ʑ?new
ʎ(ŵ.ũ):Ɖ(ŵ.ũ)))),new ʒ<ʇ>(ʓ(ʆ<ʇ>,ŵ=>ŵ.ʔ.Count>0&&ŵ.ʔ[0]is ʕ,ŵ=>ŵ.ʔ),ʖ(ʆ<ʇ>,ɶ<ʉ>(),(ŵ,Ł)=>ʗ<ʈ>(ŵ.ʔ)!=null,(ŵ,Ł)=>new ʘ(new
ʌ(ʍ(ʗ<ʈ>(ŵ.ʔ).ũ,Ł??ʗ<ʉ>(ŵ.ʔ)),ŵ.ʑ?new ʎ(ŵ.ũ):Ɖ(ŵ.ũ)))),ʓ(ʆ<ʇ>,ē=>á.ʙ.ContainsKey(ē.ũ),ē=>new ʚ(()=>ē.ũ)),ʓ(ʆ<ʇ>,Ę=>{Ũ Ʉ;ʛ
ʜ=Ę.ʑ?new ʎ(Ę.ũ):Ɖ(Ę.ũ);if(Ę.ʑ&&ʝ(Ę.ũ,out Ʉ))ʜ=Ɖ(Ʉ.ũ);return new ʏ(ʜ);})),ʓ(ʆ<ʕ>,ŵ=>ŵ.ʞ.Count>0,ŵ=>ŵ.ʞ),ʖ(ʆ<ʟ>,ɱ<ʠ>(),(ʡ,
ɺ)=>new ʟ(new ʢ(ʡ.ũ,ɺ.ũ))),ʖ(ʆ<ʠ>,ɲ<ʏ>(),(ɺ,ʜ)=>new ʟ(new ʢ(ʜ.ũ,ɺ.ũ))),ʖ(ʆ<ʟ>,ɲ<ʣ>(),(ʡ,ʤ)=>new ʥ(ʡ.ũ,ʤ.ũ)),ʐ(ʆ<ʦ>,ɶ<ʈ>()
,ɶ<ʉ>(),(ŵ,ċ,ʊ)=>new ʋ(new ʧ(ċ?.ũ?.ċ))),ʐ(ʆ<ʏ>,ɱ<ʈ>(),ɶ<ʉ>(),(ŵ,ċ,ʊ)=>new ʋ(new ʌ(ʍ(ċ.ũ,ʊ),ŵ.ũ))),ʐ(ʆ<ʟ>,ɱ<ʈ>(),ɶ<ʉ>(),(ŵ
,ċ,ʊ)=>new ʋ(new ʌ(ʍ(ċ.ũ,ʊ),ŵ.ũ))),ʖ(ʆ<ʈ>,ɶ<ʉ>(),(ċ,ʊ)=>new ʋ(new ʨ(ċ.ũ.ċ.Value))),ʖ(ʆ<ʩ>,ɱ<ʏ>(),(ŵ,ʪ)=>new ʫ(ʪ.ũ)),ʖ(ʆ<ʬ
>,ɳ<ʭ>(),(ŵ,ɫ)=>new ʬ((ǣ,E)=>!ŵ.ũ(ǣ,E))),ʖ(ʆ<ʬ>,ɱ<ʬ>(),(ŵ,ɬ)=>new ʬ(ɬ.ũ)),ʖ(ʆ<ʬ>,ɱ<ʮ>(),(ŵ,ɬ)=>ŵ),ʖ(ʆ<ʫ>,ɲ<ʋ>(),(ŵ,O)=>
new ʋ(new ʯ(O.ũ,ŵ.ũ))),ʖ(ʆ<ʠ>,ɲ<ʋ>(),(ɺ,O)=>new ʋ(new ʯ(O.ũ,new ʰ(ɺ.ũ)))),ʐ(ʆ<ʠ>,ɶ<ʠ>(),ɲ<ʠ>(),(ŵ,ɬ,ɫ)=>!ɫ.ɧ(),(ŵ,ɬ,ɫ)=>new
ʟ(new ʢ(ŵ.ũ,ɬ?.ũ??ʱ()))),ʓ(ʆ<ʲ>,ŵ=>Q<ʳ>()),ʖ(ʆ<ʏ>,ɲ<ʴ>(),(ē,ʵ)=>new ʚ(()=>Ż(ē.ũ.ū()),ʵ.ũ)),ʐ(ʆ<ʮ>,ɲ<ʶ>(),ɱ<ʶ>(),(ʷ,ʸ,ŵ)=>
ɾ(ʸ,ŵ)&&ʸ.ū().ũ==C.ț,(ʷ,ʸ,ŵ)=>Q<ʳ>(ʸ,ŵ)),ʓ(ʆ<ʶ>,ŵ=>new ʹ(new ų(ŵ.ũ+"",ŵ.ʺ).ʻ(ŵ.Ǥ))),ʖ(ʆ<ʼ>,ɲ<ʏ>(),(ŵ,F)=>new ʹ(new ų(ŵ.ũ+
"",ŵ.ʺ,F.ũ))),ʖ(ʆ<ʼ>,ɱ<ʏ>(),(ŵ,F)=>new ʹ(new ų(ŵ.ũ+"",ŵ.ʺ,F.ũ))),ʐ(ʆ<ʣ>,ɶ<ʽ>(),ɱ<ʏ>(),(ŵ,Ł,ē)=>ɾ(Ł,ē)&&(ē.ū().ũ is ʎ),(ŵ,Ł
,ē)=>new ʾ(((ʎ)ē.ũ).ũ,ŵ.ũ,Ł!=null)),ʖ(ʆ<ʿ>,ɱ<ʏ>(),(ŵ,ē)=>ē.ɧ()&&(ē.ū().ũ is ʎ),(ŵ,ē)=>new ˀ(((ʎ)ē.ũ).ũ,ŵ.ũ)),ʖ(ʆ<ˁ>,ɲ<ʏ>(
),(ŵ,ē)=>ē.ɧ()&&(ē.ū().ũ is ʎ),(ŵ,ē)=>new ˀ(((ʎ)ē.ũ).ũ,ŵ.ũ)),ʖ(ʆ<ʟ>,ɲ<ˆ>(),(ɺ,ˇ)=>new ʏ(new ˈ(ɺ.ũ,ˇ.ũ))),ʄ(ʆ<ʟ>,ɱ<ʬ>(),ɱ<
ʏ>(),ɷ<ˉ>(),(ɺ,ˊ,ũ,ˇ)=>new ʏ(new ˋ(ˇ?.ũ??á.ˌ,ɺ.ũ,ˊ.ũ,ũ.ũ))),ʓ(ʆ<ʟ>,ɺ=>new ʏ(ɺ.ũ)),new ʒ<ˍ>(ʓ(ʆ<ˍ>,ˎ=>new ˏ(ː.ˑ)),ʓ(ʆ<ˍ>,ˎ
=>new ˠ(ˡ.ˢ,3))),new ʒ<ˣ>(ʓ(ʆ<ˣ>,ˤ=>new ˠ(ˡ.ˬ,1)),ʓ(ʆ<ˣ>,ˤ=>new ˮ(ː.ˬ)),ʓ(ʆ<ˣ>,ˤ=>new ˏ(ː.ˬ))),new ʒ<Ͱ>(ʓ(ʆ<Ͱ>,ˤ=>new ˠ(ˡ.
ͱ,4)),ʓ(ʆ<Ͱ>,ˤ=>new ˮ(ː.ͱ)),ʓ(ʆ<Ͱ>,ˤ=>new ˏ(ː.ͱ))),ʖ(ʆ<ˮ>,ɲ<ʏ>(),(ŵ,Ͳ)=>new ʏ(new ͳ(ŵ.ũ,Ͳ.ũ))),ʖ(ʆ<ˏ>,ɱ<ʏ>(),(ŵ,Ͳ)=>new ʏ
(new ͳ(ŵ.ũ,Ͳ.ũ))),ʹ(ʆ<Ͷ>,ɲ<ʏ>(),ɱ<ʏ>(),ɱ<Ͷ>(),ɱ<ʏ>(),(ͷ,ͺ,ͻ,ͼ,ͽ)=>ɾ(ͺ,ͻ,ͽ)&&!(ͺ.ū().ũ is Ά||ͻ.ū().ũ is Ά||ͽ.ū().ũ is Ά),(
ͷ,ͺ,ͻ,ͼ,ͽ)=>new ʏ(new Ά{Έ=ͺ.ũ,Ή=ͻ.ũ,Ί=ͽ.ũ})),Ό(0),Ό(1),Ό(2),Ό(3),ʐ(ʆ<ʬ>,ɲ<ʏ>(),ɱ<ʏ>(),(ŵ,ɫ,ɬ)=>new ʏ(new Ύ(ɫ.ũ,ɬ.ũ,ŵ.ũ)))
,ʖ(ʆ<ʭ>,ɱ<ʏ>(),(ŵ,ɬ)=>new ʏ(new ͳ(ː.ˑ,ɬ.ũ))),ʖ(ʆ<Ώ>,ɱ<ʏ>(),(ŵ,ɬ)=>new ʏ(new ͳ(ː.ˑ,ɬ.ũ))),ʐ(ʆ<ΐ>,ɲ<ʏ>(),ɱ<ʏ>(),(ŵ,ɫ,ɬ)=>
new ʏ(new Α(ˡ.Β,ɫ.ũ,ɬ.ũ))),ʐ(ʆ<Γ>,ɲ<ʏ>(),ɱ<ʏ>(),(ŵ,ɫ,ɬ)=>new ʏ(new Α(ˡ.Δ,ɫ.ũ,ɬ.ũ))),Ό(4),ʐ(ʆ<Ε>,ɲ<ʏ>(),ɱ<ʏ>(),(Ζ,ɫ,ɬ)=>new
ʏ(new ɐ(ɫ.ũ,ɬ.ũ))),ʄ(ʆ<ΐ>,ɲ<Η>(),ɶ<Θ>(),ɱ<Η>(),(ŵ,ɫ,Ι,ɬ)=>new Η(á.Κ(ɫ.ũ,ɬ.ũ))),ʄ(ʆ<Γ>,ɲ<Η>(),ɶ<Θ>(),ɱ<Η>(),(ŵ,ɫ,Ι,ɬ)=>new
Η(á.Λ(ɫ.ũ,ɬ.ũ))),ʹ(ʆ<Θ>,ɱ<ʬ>(),ɻ<ʹ>(true),ɶ<Μ>(),ɶ<ʏ>(),(Ι,ŵ,Ν,Ξ,ʪ)=>ŵ.ɧ()&&ɿ(ʪ,Ν),(Ι,ŵ,Ν,Ξ,ʪ)=>Q<ʳ>(new Θ(),new Η(Ο(new
Ų(Ν.Select(F=>F.ũ).ToList()).Π(Ξ?.ũ),new Ρ(ŵ.ũ),ʪ?.ũ??Ɖ(true))))),ʐ(ʆ<Θ>,ɲ<ʋ>(),ɱ<Η>(),(ŵ,O,Σ)=>new ʋ(new Τ(O.ũ,Σ.ũ))),ʐ(
ʆ<ʹ>,ɶ<ˉ>(),ɶ<ʋ>(),(ŵ,ǣ,Ę)=>Υ(ǣ.ū(),Ę.ū()),(ŵ,ǣ,Ę)=>Q<ʳ>(ǣ,Ę,ŵ).Where(Φ=>Φ!=null).ToList()),ʄ(ʆ<ˆ>,ɳ<ʋ>(),ɼ<ʹ>(false),ɸ<Μ
>(),(ŵ,O,Ν,Ξ)=>new ʏ(new Χ(ŵ.ũ,O.ũ,new Ų(Ν.Select(F=>F.ũ).ToList()).Π(Ξ?.ũ)))),ʄ(ʆ<ʬ>,ɼ<ʹ>(true),ɸ<Μ>(),ɶ<ʏ>(),(ŵ,Ν,Ξ,ʪ)
=>ɿ(ʪ,Ν),(ŵ,Ν,Ξ,ʪ)=>new Η(Ο(new Ų(Ν.Select(F=>F.ũ).ToList()).Π(Ξ?.ũ),new Ρ(ŵ.ũ),ʪ?.ũ??Ɖ(true)))),ʐ(ʆ<Η>,ɷ<ˉ>(),ɲ<ʋ>(),(ŵ,ˇ
,O)=>new ʏ(new Ψ(ˇ?.ũ??á.ˌ,ŵ.ũ,O.ũ))),ʖ(ʆ<ˉ>,ɱ<ʋ>(),(ˇ,O)=>ˇ.ũ!=á.Ω&&O.ɧ(),(ˇ,O)=>O),ʖ(ʆ<Ϊ>,ɲ<ʏ>(),(ŵ,ʪ)=>new Ϋ(ʪ.ũ)),ʹ(ʆ
<ά>,ɲ<ʋ>(),ɱ<ʋ>(),ɱ<ʏ>(),ɶ<ʏ>(),(đ,έ,ή,ί,ΰ)=>new α(new β((đ.ũ?έ:ή).ũ,(đ.ũ?ή:έ).ũ,ί.ũ,ΰ?.ũ))),ʹ(ʆ<ά>,ɱ<ʋ>(),ɱ<ʋ>(),ɱ<ʏ>(),
ɶ<ʏ>(),(đ,έ,ή,ί,ΰ)=>new α(new β(έ.ũ,ή.ũ,ί.ũ,ΰ?.ũ))),ʓ(ʆ<Ͷ>,E=>new γ()),ʹ(ʆ<δ>,ɲ<ʏ>(),ɱ<ʏ>(),ɱ<γ>(),ɱ<ʏ>(),(ǿ,ε,ζ,η,θ)=>
new ʏ(new ι(){Σ=ε.ũ,ζ=ζ.ũ,θ=θ.ũ})),ʖ(ʆ<κ>,ɱ<ʏ>(),(ŵ,ʪ)=>new λ(ŵ.μ?new ͳ(ː.ˑ,ʪ.ũ):ʪ.ũ,ŵ.ν,ŵ.ξ)),ʐ(ʆ<ʹ>,ɼ<ʹ>(false),ɸ<Μ>(),(Ę
,ŵ,ơ)=>new ο(new Ų(ŵ.Select(F=>F.ũ).Concat(Q(Ę.ũ)).ToList()).Π(ơ?.ũ))),ʓ(ʆ<Μ>,ơ=>new ο(new Ų().Π(ơ.ũ))),new ʒ<ʘ>(ʖ(ʆ<ʘ>,ɼ
<ο>(true),(Ę,ŵ)=>new π(Ę.ũ,ŵ.Aggregate(new Ų(),(ǣ,E)=>ǣ.ρ(E.ũ)))),ʓ(ʆ<ʘ>,ŵ=>new ʏ(((ʌ)ŵ.ũ).O)),ʓ(ʆ<ʘ>,ŵ=>new ʋ(ŵ.ũ))),ʖ(ʆ
<ʋ>,ɼ<ο>(true),(Ę,ŵ)=>new π(Ę.ũ,ŵ.Aggregate(new Ų(),(ǣ,E)=>ǣ.ρ(E.ũ)))),ʓ(ʆ<ʮ>,ŵ=>Q<ʳ>()),ʖ(ʆ<ʋ>,ɲ<ʳ>(),(Ę,Φ)=>Q(Φ,new π(Ę
.ũ,new Ų()))),ʖ(ʆ<ʋ>,ɱ<ʳ>(),(Ę,Φ)=>Q(new π(Ę.ũ,new Ų()),Φ)),new ʒ<π>(ς(),ʓ(ʆ<π>,Ę=>new ʏ(new Χ(á.σ,Ę.O,Ę.Ŵ))),ʓ(ʆ<π>,Ę=>{
var ť=Ę.Ŵ;if(ť.ƴ==null)ť=ť.τ(Ɖ(true));return new α(new υ(Ę.O,(E,φ)=>E.ǜ(φ,ť.χ(E))));})),ʓ(ʆ<ψ>,E=>Q<ʳ>()),ʖ(ʆ<ʥ>,ɱ<ʏ>(),(ɺ,
ũ)=>new α(new ω(ɺ.ϊ,ũ.ũ,ɺ.ϋ))),ʖ(ʆ<ό>,ɱ<ʏ>(),(ŵ,ʪ)=>new α(new ύ(ʪ.ũ))),ʖ(ʆ<ώ>,ɶ<ʏ>(),(ŵ,Ϗ)=>new α(new ϐ(Ϗ?.ũ??Ɖ(0.01666f)
))),ʖ(ʆ<ʚ>,ɻ<ʏ>(false),(ŵ,ϑ)=>new α(new ϒ(ŵ.ϓ,ŵ.ϔ,ϑ.Select(F=>F.ũ).ToList()))),ʖ(ʆ<ʾ>,ɱ<ʏ>(),(ŵ,ʪ)=>new α(new ϕ(ŵ.ϖ,ʪ.ũ,ŵ
.ϋ,ŵ.ϗ))),ʖ(ʆ<ˀ>,ɶ<ʏ>(),(ǎ,ʜ)=>new α(new Ϙ(ǎ.ϖ,ǎ.ũ,ʜ?.ũ??Ɖ(1)))),ʖ(ʆ<ˁ>,ɱ<ʏ>(),(ŵ,ē)=>ē.ɧ()&&(ē.ū().ũ is ʎ),(ŵ,ē)=>new ˀ(
((ʎ)ē.ũ).ũ,ŵ.ũ)),ʐ(ʆ<ϙ>,ɱ<ʏ>(),ɱ<ʏ>(),(ŵ,Ϛ,ϛ)=>new α(new Ϝ(Ϛ.ũ,ϛ.ũ))),ʖ(ʆ<ϝ>,ɱ<ʏ>(),(ŵ,ʪ)=>new α(new Ϟ(ʪ.ũ,ŵ.ũ))),ʖ(ʆ<Ϋ>,
ɳ<α>(),(ŵ,ϟ)=>new α(new Ϡ(Q(ϟ.ũ),ŵ.ũ))),ʖ(ʆ<ϡ>,ɱ<α>(),(ŵ,ϟ)=>new α(new Ϣ{ϟ=ϟ.ũ,ϣ=ŵ.ũ})),ʖ(ʆ<Ϥ>,ɱ<α>(),(ŵ,ϟ)=>new α(new ϥ{
Ϧ=ϟ.ũ})),ʄ(ʆ<ϧ>,ɱ<ʏ>(),ɱ<ʏ>(),ɳ<α>(),(ǿ,ȋ,ɺ,ϟ)=>ɾ(ɺ,ϟ,ȋ)&&ȋ.ū().ũ is ʎ,(ǿ,ȋ,ɺ,ϟ)=>new α(new Ϩ(((ʎ)ȋ.ũ).ũ,ɺ.ũ,ϟ.ũ))),ʄ(ʆ<λ
>,ɳ<α>(),ɶ<ϩ>(),ɶ<α>(),(Σ,Ϫ,ϫ,Ϭ)=>{Ĩ ϭ=ϫ!=null?Ϭ.ũ:new ĩ();return new α(new Ϯ(Σ.ũ,Σ.ξ?ϭ:Ϫ.ũ,Σ.ξ?Ϫ.ũ:ϭ,Σ.ν));}));public
List<List<ʳ>>ϼ(List<ʳ>ϰ){var ϱ=Q<ʀ>();var ϲ=Q<List<ʳ>>();ϳ(ϱ,ϰ);int ϴ=0;while(ϴ<ϱ.Count){bool ϵ=false;bool Ϸ=false;ʀ ϸ=ϱ[ϴ];
for(int ǿ=ϰ.Count-1;ǿ>=0;ǿ--){if(ϸ.Ϲ(ϰ[ǿ])){List<ʳ>Ϻ;if(ϸ.ϻ(ϰ,ǿ,out Ϻ,ϲ)){ϳ(ϱ,Ϻ);Ϸ=true;break;}else ϵ=true;}}if(Ϸ){ϴ=0;
continue;}if(!ϵ)ϱ.RemoveAt(ϴ);else ϴ++;}return ϲ;}public void Ͽ(){for(int ǿ=0;ǿ<ϯ.Count;ǿ++)ϯ[ǿ].Ͻ=ǿ;ʁ=ϯ.ToLookup(ŵ=>ŵ.Ͼ());}
public ō Ё<ō>(List<ʳ>Ѐ)where ō:class,ʳ{var ϲ=Q(Ѐ);while(ϲ.Count>0){ϲ.AddRange(ϼ(ϲ[0]));if(ϲ[0].Count==1&&ϲ[0][0]is ō){return(ō
)ϲ[0][0];}else{ϲ.RemoveAt(0);}}return null;}void ϳ(List<ʀ>ɽ,List<ʳ>Ђ){ɽ.AddRange(Ђ.Select(đ=>đ.GetType()).SelectMany(đ=>ʁ
[đ]).Except(ɽ));ɽ.Sort();}static ō ʗ<ō>(List<ʳ>Ѐ)where ō:class,ʳ=>Ѐ.OfType<ō>().LastOrDefault();public List<Ѓ>Є=Q<Ѓ>();
public delegate bool Ѓ();bool З(){int Ѕ=0;if(String.IsNullOrWhiteSpace(á.Me.CustomData)){І("Welcome to EasyCommands!");І(
"Add Commands to Custom Data");return false;}else if(!á.Me.CustomData.Equals(Ї)){int Ј=1;Ї=á.Me.CustomData;Љ=Ї.Trim().Split(new[]{"\r\n","\r","\n"},
StringSplitOptions.None).SkipWhile(Ƥ=>{var Њ=string.IsNullOrWhiteSpace(Ƥ)||Ƥ.Trim().StartsWith("#");if(Њ)Ј++;return Њ;}).ToList();ʙ.Clear(
);Є.Clear();Ћ();if(!Љ[0].StartsWith(":")){Љ.Insert(0,":main");Ј--;}var Ќ=Ȅ(0,Љ.Count).Where(ǿ=>Љ[ǿ].StartsWith(":")).
Reverse();foreach(int ǿ in Ќ){var Ў=Ѝ(Љ[ǿ].Remove(0,1).Trim());var А=Ў[0].Џ;ʙ[А]=new Б(А,Ў.Skip(1).Select(đ=>đ.Џ).ToList());Є.
Add(new В(Љ.GetRange(ǿ+1,Љ.Count-ǿ-1),Ј+ǿ+1,Г=>{Є.Add(new Д(Г,0,true,ϟ=>{ʙ[А].ʵ=ϟ as Ϡ??new Ϡ(Q(ϟ));Е=А;}).Ж());}).Ж());Љ.
RemoveRange(ǿ,Љ.Count-ǿ);}Ѕ++;}while(Ѕ++<commandParseAmount&&Є.Count>0){if(Є[0]())Є.RemoveAt(0);}if(Є.Count>0)І("Parsing Script..."
);return Є.Count==0;}public class В{int И;List<String>Љ;List<Й>Г=Q<Й>();Action<List<Й>>К;public В(List<String>Л,int М,
Action<List<Й>>Н){И=М;Љ=Л;К=Н;}public Ѓ Ж()=>()=>{int ǿ=0;while(ǿ<á.commandParameterParseAmount&&Љ.Count>0){Й О=new Й(Љ[0],И++
);Љ.RemoveAt(0);ǿ+=О.ϰ.Count;if(О.ϰ.Count>0)Г.Add(О);}if(Љ.Count>0)return false;К(Г);return true;};}public class Д{List<Ĩ
>П=Q<Ĩ>();List<Й>Љ;int ʡ;bool Р;Action<Ĩ>К;public Д(List<Й>Л,int С,bool Т,Action<Ĩ>Н){Љ=Л;ʡ=С;Р=Т;К=Н;}public Ѓ Ж()=>()=>
{Ĩ ϟ;var Ф=У(out ϟ);if(Ф)К(ϟ);return Ф;};bool У(out Ĩ ϟ){ϟ=null;while(ʡ<Љ.Count-1){Й ϸ=Љ[ʡ];Й Х=Љ[ʡ+1];if(ϸ.Ц>Х.Ц)break;
if(ϸ.Ц<Х.Ц){á.Є.Insert(0,new Д(Љ,ʡ+1,true,Ч=>ϸ.ϰ.Add(new α(Ч))).Ж());return false;}if(Х.ϰ.Count>0&&Х.ϰ[0]is ϩ){ϸ.ϰ.Add(Х.ϰ
[0]);Х.ϰ.RemoveAt(0);á.Є.Insert(0,new Д(Љ,ʡ+1,false,Ч=>ϸ.ϰ.Add(new α(Ч))).Ж());return false;}if(!Р)break;П.Add(á.У(ϸ.ϰ,ϸ.
Ш));Љ.RemoveAt(ʡ);return false;}П.Add(á.У(Љ[ʡ].ϰ,Љ[ʡ].Ш));Љ.RemoveAt(ʡ);ϟ=П.Count>1?new Ϡ(П):П[0];return true;}}public Ĩ
У(String Щ,int Ш=0)=>У(Ъ(Ѝ(Щ)),Ш);Ĩ У(List<ʳ>Ѐ,int Ш){α ϟ=Ё<α>(Ѐ);if(ϟ==null)throw new Ы(
"Unable to parse command from command parameters at line number: "+Ш);return ϟ.ũ;}public class Й{public int Ц,Ш;public List<ʳ>ϰ;public Й(String ϟ,int Ƥ){Ц=ϟ.TakeWhile(Char.IsWhiteSpace).
Count();ϰ=á.Ъ(á.Ѝ(ϟ));Ш=Ƥ;}}Dictionary<String,List<ʳ>>Ь=Ź<string,List<ʳ>>();string[]Э=new[]{"(",")","[","]",",","*","/","!",
"^","..","%",">=","<=","==","&&","||","@","$","->","++","+=","--","-=","::"};string[]Ю=new[]{"<",">","=","&","|","-","+",
"?",":"},Я=new[]{"."};static Dictionary<ː,String>а=Ź(ź(ː.ͱ,"cast"),ź(ː.ˬ,"round"),ź(ː.ˑ,"negate"));static Dictionary<ˡ,
String>в=Ź(ź(ˡ.ͱ,"cast"),ź(ˡ.ˬ,"round"),ź(ˡ.б,"compare"));static Dictionary<Ĭ,String>г=Ź(ź(Ĭ.ĭ,"boolean"),ź(Ĭ.ŋ,"number"),ź(Ĭ.
Ľ,"string"),ź(Ĭ.ķ,"vector"),ź(Ĭ.ŝ,"color"),ź(Ĭ.ɑ,"list"));static Dictionary<string,д>ж=Ź(е("bool",ŵ=>ż(ŵ)),е("boolean",ŵ
=>ż(ŵ)),е("string",Ż),е("number",ŵ=>Ž(ŵ)),е("vector",ŵ=>Ƕ(ŵ)),е("color",ŵ=>ž(ŵ)),е("list",Ƿ));static Dictionary<string,
Color>з=Ź(ź("red",Color.Red),ź("blue",Color.Blue),ź("green",Color.Green),ź("orange",Color.Orange),ź("yellow",Color.Yellow),ź(
"white",Color.White),ź("black",Color.Black));delegate IEnumerable<ʺ>и(string Ę);delegate и й(и ŵ);и к=Ę=>ɢ<ʺ>();и л=Ę=>ĥ(new ʺ(
Ę,false,false)),м;public void Ҍ(){м=н(л,о(Ę=>!string.IsNullOrWhiteSpace(Ę)&&!Ę.Trim().StartsWith("#")),п('`'),п('\''),п(
'"',false),р(" : "," :: "),с(Э),т(),с(Ю),т(),с(Я));у(ф("in","the","than","turned","block","panel","chamber","drive","of",
"either","for","do","does","second","seconds","be","being","digits","digit"),new ʲ());у(ф("to","from","then"),new ʮ());у(ф(
"blocks","group","panels","chambers","drives"),new ʉ());у(ф("my","self","this"),new ʦ());у(ф("$"),new ʅ());х(ф("up","upward",
"upwards","upper"),Ņ.ŉ);х(ф("down","downward","downwards","lower"),Ņ.Ŋ);х(ф("left","lefthand"),Ņ.ň);х(ф("right","righthand"),Ņ.ņ)
;х(ф("forward","forwards","front"),Ņ.Ɛ);х(ф("backward","backwards","back"),Ņ.Ƒ);х(ф("clockwise","clock"),Ņ.Ȥ);х(ф(
"counterclockwise","counter","counterclock"),Ņ.ȥ);у(ф("bind","tie","link"),new ʣ(true));у(ф("move","go","tell","turn","rotate","set",
"assign","allocate","designate","apply"),new ʣ());у(ф("reverse","reversed"),new Ώ());у(ф("raise","extend"),new ʣ(),new Μ(Ņ.ŉ));у
(ф("retract"),new ʣ(),new Μ(Ņ.Ŋ));у(ф("increase","increment"),new ʿ());у(ф("decrease","decrement","reduce"),new ʿ(false))
;у(ф("++","+="),new ˁ());у(ф("--","-="),new ˁ(false));у(ф("global"),new ʽ());у(ф("by"),new ψ());у(ф("on","begin","true",
"start","started","resume","resumed"),new ʏ(Ɖ(true)));у(ф("off","terminate","cancel","end","false","stopped","halt","halted"),
new ʏ(Ɖ(false)));ц(ч(ш("height","length","level","size","period","scale")),C.I);ц(ш("angle","azimuth"),C.ľ);ц(ч(ш("speed",
"rate","pace"),ф("velocity","velocities")),C.ń);ц(ф("connect","attach","connected","attached","dock","docked","docking"),C.ư);
ц(ф("disconnect","detach","disconnected","detached","undock","undocked"),C.ư,false);ц(ф("lock","locked","locking",
"freeze","frozen","brake","braking","handbrake","permanent","static"),C.H);ц(ф("unlock","unlocked","unfreeze"),C.H,false);ц(ф(
"run","running","execute","executing","script"),C.ƭ);ц(ф("use","used","occupy","occupied","control","controlled"),C.Ĵ);ц(ф(
"unused","unoccupied","vacant","available"),C.Ĵ,false);ц(ф("done","complete","finished","finish","pressurized","depressurized"),
C.ħ);ц(ф("clear","wipe","erase"),C.ħ,false);ц(ф("open","opened"),C.Ȫ);ц(ф("close","closed","shut"),C.Ȫ,false);ц(ш("font")
,C.ɡ);ц(ш("text","message","argument"),C.Ʊ);ц(ч(ф("colors"),ш("foreground")),C.ŝ);щ(ф("color"),new ʶ(C.ŝ));ц(ш(
"background"),C.ɠ);ц(ф("power","powered"),C.Ƨ);ц(ф("enable","enabled","arm","armed"),C.Ɯ);ц(ф("disable","disabled","disarm",
"disarmed"),C.Ɯ,false);ц(ф("music","sound","song","track","image","play","playing","unsilence"),C.Ȟ);ц(ф("silence","silent",
"quiet"),C.Ȟ,false);ц(ф("sounds","songs","images","tracks"),C.ə);ц(ч(ш("volume","output"),ф("intensity","intensities")),C.Š);ц(
ч(ш("range","distance","limit","delay"),ф("capacity","capacities")),C.İ);ц(ф("radius","radii"),C.ş);ц(ш("interval"),C.Ȭ);
ц(ш("offset","padding"),C.Ţ);ц(ш("falloff"),C.š);ц(ф("trigger","triggered","detect","detected","trip","tripped","deploy",
"deployed","shoot","shooting","shot","detonate","fire","firing"),C.Ŀ);ц(ф("pressure","pressurize","pressurizing","supply",
"supplying","generate","generating","discharge","discharging","broadcast","broadcasting","assemble","assembling"),C.Ʈ);ц(ф(
"stockpile","stockpiling","depressurize","depressurizing","gather","gathering","intake","recharge","recharging","consume",
"consuming","collect","collecting","disassemble","disassembling"),C.Ʈ,false);ц(ч(ш("ratio","percentage","percent","completion"),ф(
"progress","progresses")),C.ţ);ц(ш("input","pilot","user"),C.Ʋ);ц(ш("roll","rollInput","rotation"),C.Ŏ);ц(ф("auto","autopilot",
"refill","drain","draining","cooperate","cooperating"),C.ĳ);ц(ч(ш("override","dampener"),ф("overridden")),C.ȧ);ц(ш("direction"),
C.Ə);ц(ш("position","location","alignment"),C.Ɓ);ц(ф("target","targeting","destination","waypoint","coords","coordinates"
),C.Ķ);ц(ф("waypoints","destinations"),C.ɍ);ц(ч(ш("strength","force","torque"),ф("gravity","gravities")),C.ȩ);ц(ф(
"natural","planet"),C.ȯ);ц(ф("artificial","fake"),C.Ȱ);ц(ш("countdown"),C.ȟ);ц(ш("name","label"),C.D);ц(ф("show","showing"),C.Ƃ);
ц(ф("hide","hiding"),C.Ƃ,false);ц(ф("properties","attributes"),C.Ɔ);ц(ф("actions"),C.Ɗ);ц(ф("types","blueprints"),C.Ȃ);ц(
ш("altitude","elevation"),C.Ŕ);ц(ф("weight","mass"),C.Ȁ);ц(ф("data","customdata"),C.ƃ);ц(ф("info","details",
"detailedinfo"),C.Ƅ);ц(ф("invert","inverted","inverting"),C.ɥ);ц(ф("steer","steering"),C.ɤ);ц(ф("able","ready"),C.ț);ц(ф("unable"),C.ț
,false);ц(ф("build","building","built"),C.ƕ);ц(ф("damage","damaged"),C.Ɩ);у(ш("amount"),new ʼ(C.ǽ));у(ф("property",
"attribute"),new ʼ(C.ъ));у(ф("action"),new ʼ(C.ƌ));у(ф("produce","producing","create","creating","make","making"),new ʼ(C.ȍ));у(ф(
"destroy","destroying","recycle","recycling"),new ʼ(C.ȑ));у(ф("times","iterations"),new Ϊ());у(ф("wait","hold"),new ώ());у(ф(
"call","gosub"),new ʴ(false));у(ф("goto"),new ʴ(true));у(ф("listen","channel","register","subscribe"),new ϝ(true));у(ф(
"forget","dismiss","ignore","deregister","unsubscribe"),new ϝ(false));у(ф("send"),new ϙ());у(ф("print","log","echo","write"),new
ό());у(ф("queue","schedule"),new ϡ(false));щ(ф("async","parallel"),new ϡ(true));у(ф("await","blocking"),new Ϥ());у(ф(
"transfer","give"),new ά(true));у(ф("take"),new ά(false));у(ф("->"),new Ε());у(ф("?"),new δ());у(ф("::"),new γ());у(ф(":"),new Ͷ()
);у(ф("each","every"),new ϧ());у(ф("if"),new κ(false,false,false));у(ф("unless"),new κ(true,false,false));у(ф("while"),
new κ(false,true,false));у(ф("until"),new κ(true,true,false));у(ф("when"),new κ(true,true,true));у(ф("else","otherwise"),
new ϩ());у(ф("that","which","whose"),new Θ());у(ф("less","<","below"),new ʬ((ǣ,E)=>ǣ.CompareTo(E)<0));у(ф("<="),new ʬ((ǣ,E)
=>ǣ.CompareTo(E)<=0));у(ф("is","are","equal","equals","=","=="),new ʬ((ǣ,E)=>ǣ.CompareTo(E)==0));у(ф(">="),new ʬ((ǣ,E)=>ǣ.
CompareTo(E)>=0));у(ф("greater",">","above","more"),new ʬ((ǣ,E)=>ǣ.CompareTo(E)>0));у(ф("contain","contains"),new ʬ((ǣ,E)=>ż(ы(ˡ.
ь,ǣ,E))));у(ф("any"),new ˉ((э,ю)=>ю>0));у(ф("all"),new ˉ(ˌ));у(ф("none"),new ˉ(Ω));у(ф("average","avg"),new ˆ((N,я)=>ѐ(N,
я).ё(ŷ(Math.Max(1,N.Count())))));у(ф("minimum","min"),new ˆ((N,я)=>N.Select(я).Min()??ŷ(0)));у(ф("maximum","max"),new ˆ((
N,я)=>N.Select(я).Max()??ŷ(0)));у(ф("count"),new ˆ((N,я)=>ŷ(N.Count())));щ(ф("number"),new ˆ((N,я)=>ŷ(N.Count())));у(ф(
"sum","total"),new ˆ(ѐ));у(ф("collection"),new ˆ((N,я)=>ŷ(ƈ(N.Select(E=>new ђ(я(E)))))));щ(ф("list"),new ˆ((N,я)=>ŷ(ƈ(N.
Select(E=>new ђ(я(E)))))));у(ф("("),new ѓ());у(ф(")"),new є());у(ф("and","&","&&","but","yet"),new ΐ());у(ф("or","|","||"),new
Γ());у(ф("not","!","stop"),new ʭ());у(ф("@"),new ʩ());ѕ(ф("absolute","abs"),ː.і);ѕ(ф("sqrt"),ː.ї);ѕ(ф("sin"),ː.ј);ѕ(ф(
"cosine","cos"),ː.љ);ѕ(ф("tangent","tan"),ː.њ);ѕ(ф("arcsin","asin"),ː.ћ);ѕ(ф("arccos","acos"),ː.ќ);ѕ(ф("arctan","atan"),ː.ѝ);ѕ(ф
("sort","sorted"),ː.ў);ѕ(ф("ln"),ː.џ);ѕ(ф("rand","random","randomize"),ː.Ѡ);ѕ(ф("shuffle","shuffled"),ː.ѡ);ѕ(ф("sign",
"quantize"),ː.Ѣ);ѣ(ф("tick","ticks"),ː.Ѥ);ѣ(ф("keys","indexes"),ː.ѥ);ѣ(ф("values"),ː.Ѧ);ѣ(ф("type"),ː.ѧ);у(ф("dot","."),new ˠ(ˡ.Ѩ,
0));ѩ(ф("pow","^","xor"),ˡ.Ѫ,1);ѩ(ф("multiply","*"),ˡ.ѫ,2);ѩ(ф("divide","/"),ˡ.Ѭ,2);ѩ(ф("mod","%"),ˡ.ѭ,2);ѩ(ф("split",
"separate","separated"),ˡ.Ѯ,2);ѩ(ф("join","joined"),ˡ.ѯ,2);ѩ(ф("plus","+"),ˡ.Ѱ,3);ѩ(ф("minus"),ˡ.ˢ,3);ѩ(ф(".."),ˡ.İ,4);у(ф("-"),
new ˍ());у(ф("round","rnd","rounded"),new ˣ());у(ф("as","cast","resolve","resolved"),new Ͱ());у(ф("["),new ѱ());у(ф("]"),
new Ѳ());у(ф(","),new ѳ());Ѵ(ф("restart","reset","reboot"),Į=>{á.Ћ();throw new ѵ(Ѷ.ѷ);});Ѵ(ф("repeat","loop","rerun",
"replay"),Į=>{Į.Ĩ=Į.Ĩ.Ѹ();return false;});Ѵ(ф("exit"),Į=>{á.Ћ();throw new ѵ(Ѷ.ѹ);});Ѵ(ф("pause"),Į=>{if(á.Ѻ!=Ѷ.ѻ)throw new ѵ(Ѷ.ѻ
);return true;});Ѵ(ф("break"),Į=>{Ѽ("break").ѽ();return false;});Ѵ(ф("continue"),Į=>{Ѽ("continue").Ѿ();return false;});Ѵ(
ф("return"),Į=>{ϒ Ҁ=Į.ѿ<ϒ>();Ĩ ҁ=new ĩ();if(Ҁ==null)Į.Ĩ=ҁ;else Ҁ.ʵ=ҁ;return false;});Ҋ(ф("piston"),S.ã);Ҋ(ф("light",
"spotlight"),S.Ù);Ҋ(ф("rotor"),S.ë);Ҋ(ф("hinge"),S.Ñ);Ҋ(ф("program","programmable"),S.á);Ҋ(ф("timer"),S.ü);Ҋ(ф("projector"),S.å);Ҋ(
ф("merge"),ф(),S.Ý);Ҋ(ф("connector"),S.r);Ҋ(ф("welder"),S.ĉ);Ҋ(ф("grinder"),S.Ê);Ҋ(ф("door","hangar","bay","gate"),S.z);Ҋ
(ф(),ф(),S.x,ш("display","screen","lcd"));Ҋ(ф("speaker","alarm","siren"),S.ï);Ҋ(ф("camera"),S.j);Ҋ(ф("sensor"),S.ó);Ҋ(ф(
"beacon"),S.h);Ҋ(ф("antenna"),S.b);Ҋ(ф("ship","rover","cockpit","seat","station","helm"),S.n);Ҋ(ф("cryo"),S.t);Ҋ(ф("drone",
"remote","robot"),S.è);Ҋ(ф("thruster"),S.Ā);Ҋ(ф("airvent","vent"),S.Z);Ҋ(ф("gun","railgun","cannon","autocannon","rocket",
"missile","launcher"),S.Ë);Ҋ(ф("turret"),S.ă);Ҋ(ф("generator"),S.Ã);Ҋ(ф("tank"),S.ø);Ҋ(ф("magnet"),ф("magnets","gears"),S.Û,ф(
"gear"));Ҋ(ф("battery"),ф("batteries"),S.f);Ҋ(ф("chute","parachute"),S.ß);Ҋ(ф("wheel"),ф("wheels"),S.õ,ф("suspension"));Ҋ(ф(
"detector"),S.v);Ҋ(ф("drill"),S.µ);Ҋ(ф("engine"),S.Á);Ҋ(ф("turbine"),S.Ă);Ҋ(ф("reactor"),S.ç);Ҋ(ф("solar"),S.ì);Ҋ(ф("sorter"),S.í)
;Ҋ(ф("gyro","gyroscope"),S.Í);Ҋ(ф("gravitygenerator"),S.Å);Ҋ(ф("gravitysphere"),S.Ç);Ҋ(ф("container"),ф("cargos",
"containers"),S.l,ф("cargo","inventory","inventories"));Ҋ(ф("warhead","bomb"),S.ć);Ҋ(ф("assembler"),S.d);Ҋ(ф("collector"),S.p);Ҋ(ф(
"ejector"),S.º);Ҋ(ф("decoy"),S.u);Ҋ(ф("jump","jumpdrive"),S.Ô);Ҋ(ф("laser","laserantenna"),S.Ö);Ҋ(ф("terminal"),S.ú);Ҋ(ф(
"refinery"),ф("refineries"),S.ê);Ҋ(ф("heatvent"),S.Ï);Ҋ(ф("searchlight"),S.ñ);Ҋ(ф("turretcontroller"),S.ą);Ҋ(ф(),ф(),S.É,ш("grid")
);Ҋ(ш("thread"),ф(),S.þ);ҋ(ф("can"),"is able");ҋ(ф("cannot"),"is not able");}String[]ф(params String[]ҍ)=>ҍ;String[]ч(
params String[][]ҍ)=>ҍ.Aggregate((ǣ,E)=>ǣ.Concat(E).ToArray());String[]ш(params String[]ҍ)=>ҍ.Concat(ҍ.Select(ɏ=>ɏ+"s")).
ToArray();void Ѵ(String[]ҍ,Ҏ ʵ){у(ҍ,new α(new ҏ{Ґ=ʵ}));}void ц(String[]ҍ,C ť,bool ґ=true)=>у(ҍ,new ʶ(ť,!ґ));void х(String[]ҍ,Ņ
ƴ){у(ҍ,new Μ(ƴ));}void ѕ(String[]ҍ,ː Ғ){у(ҍ,new ˏ(Ғ));а[Ғ]=ҍ[0];}void ѣ(String[]ҍ,ː Ғ){у(ҍ,new ˮ(Ғ));а[Ғ]=ҍ[0];}void ѩ(
String[]ҍ,ˡ Ғ,int ғ){у(ҍ,new ˠ(Ғ,ғ));в[Ғ]=ҍ[0];}void Ҋ(String[]Ҕ,S ċ)=>Ҋ(Ҕ,Ҕ.Select(E=>E+"s").ToArray(),ċ);void Ҋ(String[]Ҕ,
String[]ҕ,S ċ,String[]Җ=null){у(Ҕ,new ʈ(new җ{ċ=ċ,Ҙ=false}));у(ҕ,new ʈ(new җ{ċ=ċ,Ҙ=true}));у(Җ??new String[0],new ʈ(new җ{ċ=ċ}
));}void щ(String[]ҍ,params ʳ[]ϰ){foreach(String ҙ in ҍ)у(ф(ҙ),new ʇ(ҙ,false,new ʕ(ϰ)));}void у(String[]ҍ,params ʳ[]ϰ){
foreach(String ҙ in ҍ)Ь.Add(ҙ,ϰ.ToList());}void ҋ(String[]ҍ,string Қ){у(ҍ,á.Ъ(á.Ѝ(Қ)).ToArray());}public List<ʺ>Ѝ(String қ)=>м(
қ).ToList();List<ʳ>Ъ(List<ʺ>Ҝ)=>Ҝ.SelectMany(Ъ).ToList();List<ʳ>Ъ(ʺ ƥ){var ϰ=Q<ʳ>();if(ƥ.ҝ)ϰ.Add(new ʏ(Ɖ(ƥ.Џ)));else if(ƥ
.Ҟ)ϰ.Add(new ʇ(ƥ.Џ,false,Ъ(Ѝ(ƥ.ƥ)).ToArray()));else if(Ь.ContainsKey(ƥ.ƥ))ϰ.AddList(Ь[ƥ.ƥ]);else ϰ.Add(new ʇ(ƥ.Џ,true));ϰ
[0].ʺ=ƥ.Џ;return ϰ;}и н(и ҟ,params й[]Ҡ)=>н(ҟ,Ҡ.Select(Ł=>Ł));и н(и ҟ,IEnumerable<й>Ҡ)=>Ҡ.Reverse().Aggregate(ҟ??к,(ŵ,Ł)
=>Ł(ŵ));й о(Func<string,bool>ҡ)=>Ң=>ң=>ҡ(ң)?Ң(ң):к(ң);й п(char Ҥ,bool ҥ=true)=>Ң=>ң=>ң.Split(Ҥ).SelectMany((φ,ǿ)=>ǿ%2!=0?ĥ
(new ʺ(φ,true,ҥ)):Ң(φ));й р(string Ҧ,string ҧ)=>Ң=>ң=>Ң(ң.Replace(Ҧ,ҧ));й с(string[]Ҩ)=>Ң=>н(ң=>ң.Split(new[]{' '},
StringSplitOptions.RemoveEmptyEntries).SelectMany(ҩ=>Ҩ.Contains(ҩ)?л(ҩ):Ң(ҩ)),Ҩ.Select(Ҫ=>р(Ҫ,$" {Ҫ} ")));й т()=>Ң=>ң=>{Ũ ҫ;return ʝ(ң,out
ҫ)?л(ң):Ң(ң);};public static bool ʝ(String ƥ,out Ũ Ʉ){Ʉ=null;bool Ҭ;var ҭ=Ŗ(ƥ);Double Ү;var Ұ=ү(ƥ);if(bool.TryParse(ƥ,out
Ҭ))Ʉ=ŷ(Ҭ);if(Double.TryParse(ƥ,out Ү))Ʉ=ŷ(Ү);if(ҭ.HasValue)Ʉ=ŷ(ҭ.Value);if(Ұ.HasValue)Ʉ=ŷ(Ұ.Value);return Ʉ!=null;}public
class ʺ{public String ƥ,Џ;public bool Ҟ,ҝ;public ʺ(string ұ,bool Ҳ,bool ҳ){Ҟ=Ҳ;ҝ=ҳ;ƥ=ұ.ToLower();Џ=ұ;}public override string
ToString()=>ƥ;}public interface ʀ:IComparable<ʀ>{int Ͻ{get;set;}Type Ͼ();bool Ϲ(ʳ ŵ);bool ϻ(List<ʳ>ŵ,int ǿ,out List<ʳ>Ϻ,List<
List<ʳ>>ϲ);}public abstract class ҵ<ō>:ʀ where ō:class,ʳ{public int Ͻ{get;set;}public virtual Type Ͼ()=>typeof(ō);public int
CompareTo(ʀ Ҵ)=>Ͻ.CompareTo(Ҵ.Ͻ);public bool Ϲ(ʳ ŵ)=>ŵ is ō;public abstract bool ϻ(List<ʳ>ŵ,int ǿ,out List<ʳ>Ϻ,List<List<ʳ>>ϲ);}
public class ʂ:ҵ<ѓ>{public override bool ϻ(List<ʳ>ŵ,int ǿ,out List<ʳ>Ϻ,List<List<ʳ>>ϲ){Ϻ=null;for(int Ƚ=ǿ+1;Ƚ<ŵ.Count;Ƚ++){if(
ŵ[Ƚ]is ѓ)return false;else if(ŵ[Ƚ]is є){var Ҷ=Q(ŵ.GetRange(ǿ+1,Ƚ-(ǿ+1)));ŵ.RemoveRange(ǿ,Ƚ-ǿ+1);while(Ҷ.Count>0){Ϻ=Ҷ[0];Ҷ
.AddRange(á.ϼ(Ϻ));Ҷ.RemoveAt(0);if(Ϻ.Count==1)break;}foreach(var ҷ in Ҷ){ҷ.Insert(0,new ѓ());ҷ.Add(new є());var Ҹ=new
List<ʳ>(ŵ);Ҹ.InsertRange(ǿ,ҷ);ϲ.Add(Ҹ);}ŵ.InsertRange(ǿ,Ϻ);return true;}}throw new Ы(
"Missing Closing Parenthesis for Command");}}public class ʃ:ҵ<ѱ>{public override bool ϻ(List<ʳ>ŵ,int ǿ,out List<ʳ>Ϻ,List<List<ʳ>>ϲ){Ϻ=null;var ҹ=Q<ʛ>();int Һ=ǿ;
for(int Ƚ=Һ+1;Ƚ<ŵ.Count;Ƚ++){if(ŵ[Ƚ]is ѱ)return false;else if(ŵ[Ƚ]is ѳ){ҹ.Add(һ(ŵ,Һ,Ƚ));Һ=Ƚ;}else if(ŵ[Ƚ]is Ѳ){if(Ƚ>ǿ+1)ҹ.
Add(һ(ŵ,Һ,Ƚ));Ϻ=Q<ʳ>(new ʠ(Ɖ(ƈ(ҹ))));ŵ.RemoveRange(ǿ,Ƚ-ǿ+1);ŵ.InsertRange(ǿ,Ϻ);return true;}}throw new Ы(
"Missing Closing Bracket for List");}ʛ һ(List<ʳ>ŵ,int Һ,int Ҽ){var ǻ=ŵ.GetRange(Һ+1,Ҽ-(Һ+1));var ʜ=á.Ё<ҽ<ʛ>>(ǻ);if(ʜ==null)throw new Exception(
"List Index Values Must Resolve To a Variable");return ʜ.ũ;}}public class ʒ<ō>:ҵ<ō>where ō:class,ʳ{List<ҵ<ō>>ɽ;public ʒ(params ҵ<ō>[]ŵ){ɽ=Q(ŵ);}public override bool ϻ
(List<ʳ>ŵ,int ǿ,out List<ʳ>Ϻ,List<List<ʳ>>ϲ){Ϻ=null;var ҿ=ɽ.Where(Ҿ=>Ҿ.Ϲ(ŵ[ǿ])).ToList();var Ҹ=new List<ʳ>(ŵ);bool Ϸ=
false;foreach(ʀ Ҿ in ҿ){if(Ϸ){List<ʳ>ҫ;var Ӏ=new List<ʳ>(Ҹ);if(Ҿ.ϻ(Ӏ,ǿ,out ҫ,ϲ)){ϲ.Insert(0,Ӏ);}}else{Ϸ=Ҿ.ϻ(ŵ,ǿ,out Ϻ,ϲ);}}
return Ϸ;}}static Ӂ<ˠ>Ό(int ғ)=>ʄ(ʆ<ˠ>,ɲ<ʏ>(),ɶ<ʮ>(),ɱ<ʏ>(),(Ғ,ɫ,ӂ,ɬ)=>Ғ.ғ==ғ&&ɾ(ɫ,ɬ),(Ғ,ɫ,ӂ,ɬ)=>new ʏ(new Α(Ғ.ũ,ɫ.ũ,ɬ.ũ)));
static Ӂ<π>ς(){var Ӄ=ɼ<ʣ>(true);var ӄ=ɲ<ʿ>();var Ӆ=ɱ<ˁ>();var ӆ=ɳ<ʏ>();var Ӈ=ɳ<ο>();var ӈ=ɳ<Ώ>();var Ӊ=ɳ<ʭ>();var ӊ=ɱ<ψ>();var
ɽ=Q<ɪ>(Ӄ,ӄ,Ӆ,ӆ,Ӈ,ӈ,Ӊ,ӊ);Ӌ<π>ӌ=ŵ=>ɽ.Exists(ͺ=>ͺ.ɧ());Ӎ<π>ӑ=ŵ=>{Ų Ŵ=ŵ.Ŵ;if(Ӈ.ɧ())Ŵ=Ŵ.ρ(Ӈ.ū().ũ);ʛ ӎ=ӆ.ū()?.ũ??Ɖ(true);if(Ӊ.
ɧ())ӎ=new ͳ(ː.ˑ,ӎ);if(ɿ(Ӊ,ӆ))Ŵ=Ŵ.τ(ӎ);if(ɿ(ӄ,Ӆ,ӊ))Ŵ=Ŵ.ӏ(Ӆ.ū()?.ũ??ӄ.ū()?.ũ??true);Action<Y,Object>Ӑ;if(ӈ.ɧ())Ӑ=(E,φ)=>E.ǝ
(φ,Ŵ.χ(E));else if(ɿ(ӄ,Ӆ,ӊ))Ӑ=(E,φ)=>E.ƺ(φ,Ŵ.χ(E));else if(Ŵ.ƴ!=null)Ӑ=(E,φ)=>E.ǜ(φ,Ŵ.χ(E));else Ӑ=(E,φ)=>E.ǜ(φ,Ŵ.τ(ӎ).χ(
E));return new α(new υ(ŵ.O,Ӑ));};return new Ӂ<π>(ɽ,ӌ,ӑ);}class Ӂ<ō>:ҵ<ō>where ō:class,ʳ{List<ɪ>ɽ;Ӌ<ō>ӌ;Ӎ<ō>ӑ;public Ӂ(
List<ɪ>Ӓ,Ӌ<ō>ӓ,Ӎ<ō>Ӕ){ɽ=Ӓ;ӌ=ӓ;ӑ=Ӕ;}public override bool ϻ(List<ʳ>ŵ,int ǿ,out List<ʳ>Ϻ,List<List<ʳ>>ϲ){Ϻ=null;ɽ.ForEach(ӕ=>ӕ.
ɦ());int Ƚ=ǿ+1;while(Ƚ<ŵ.Count){if(ɽ.Exists(ӕ=>ӕ.ɩ(ŵ[Ƚ])))Ƚ++;else break;}int Ⱦ=ǿ;while(Ⱦ>0){if(ɽ.Exists(ӕ=>ӕ.ɨ(ŵ[Ⱦ-1])))
Ⱦ--;else break;}ō Ӗ=(ō)ŵ[ǿ];if(!ӌ(Ӗ))return false;var ӗ=ӑ(Ӗ);if(ӗ is ʳ)Ϻ=Q((ʳ)ӗ);else if(ӗ is List<ʳ>)Ϻ=(List<ʳ>)ӗ;else
throw new Exception("Final parameters must be CommandParameter");ŵ.RemoveRange(Ⱦ,Ƚ-Ⱦ);ŵ.InsertRange(Ⱦ,Ϻ);return true;}}static
Ӂ<ō>ʓ<ō>(Ә<ō>Ǘ,Ӎ<ō>ӑ)where ō:class,ʳ=>ʓ(Ǘ,ŵ=>true,ӑ);static Ӂ<ō>ʓ<ō>(Ә<ō>Ǘ,Ӌ<ō>ӌ,Ӎ<ō>ӑ)where ō:class,ʳ=>new Ӂ<ō>(Q<ɪ>(),ӌ
,ӑ);static Ӂ<ō>ʖ<ō,M>(Ә<ō>Ǘ,ɰ<M>ә,Ӛ<ō,M>ӑ)where ō:class,ʳ=>ʖ(Ǘ,ә,(ŵ,ǣ)=>ǣ.ɧ(),ӑ);static Ӂ<ō>ʖ<ō,M>(Ә<ō>Ǘ,ɰ<M>ә,ӛ<ō,M>ӌ,Ӛ<
ō,M>ӑ)where ō:class,ʳ=>new Ӂ<ō>(Q<ɪ>(ә),ŵ=>ӌ(ŵ,ә),ŵ=>ӑ(ŵ,ә.ū()));static Ӂ<ō>ʐ<ō,M,Ӝ>(Ә<ō>Ǘ,ɰ<M>ә,ɰ<Ӝ>F,ӝ<ō,M,Ӝ>ӑ)where ō:
class,ʳ=>ʐ(Ǘ,ә,F,(ŵ,ǣ,E)=>ɾ(ǣ,E),ӑ);static Ӂ<ō>ʐ<ō,M,Ӝ>(Ә<ō>Ǘ,ɰ<M>ә,ɰ<Ӝ>F,Ӟ<ō,M,Ӝ>ӌ,ӝ<ō,M,Ӝ>ӑ)where ō:class,ʳ=>new Ӂ<ō>(Q<ɪ>(
ә,F),ŵ=>ӌ(ŵ,ә,F),ŵ=>ӑ(ŵ,ә.ū(),F.ū()));static Ӂ<ō>ʄ<ō,M,Ӝ,ӟ>(Ә<ō>Ǘ,ɰ<M>ә,ɰ<Ӝ>F,ɰ<ӟ>ɏ,Ӡ<ō,M,Ӝ,ӟ>ӑ)where ō:class,ʳ=>ʄ(Ǘ,ә,F,
ɏ,(ŵ,ǣ,E,Φ)=>ɾ(ǣ,E,Φ),ӑ);static Ӂ<ō>ʄ<ō,M,Ӝ,ӟ>(Ә<ō>Ǘ,ɰ<M>ә,ɰ<Ӝ>F,ɰ<ӟ>ɏ,ӡ<ō,M,Ӝ,ӟ>ӌ,Ӡ<ō,M,Ӝ,ӟ>ӑ)where ō:class,ʳ=>new Ӂ<ō>(
Q<ɪ>(ә,F,ɏ),ŵ=>ӌ(ŵ,ә,F,ɏ),ŵ=>ӑ(ŵ,ә.ū(),F.ū(),ɏ.ū()));static Ӂ<ō>ʹ<ō,M,Ӝ,ӟ,Έ>(Ә<ō>Ǘ,ɰ<M>ә,ɰ<Ӝ>F,ɰ<ӟ>ɏ,ɰ<Έ>ͺ,Ӣ<ō,M,Ӝ,ӟ,Έ>ӑ)
where ō:class,ʳ=>ʹ(Ǘ,ә,F,ɏ,ͺ,(ŵ,ǣ,E,Φ,ơ)=>ɾ(ǣ,E,Φ,ơ),ӑ);static Ӂ<ō>ʹ<ō,M,Ӝ,ӟ,Έ>(Ә<ō>Ǘ,ɰ<M>ә,ɰ<Ӝ>F,ɰ<ӟ>ɏ,ɰ<Έ>ͺ,ӣ<ō,M,Ӝ,ӟ,Έ>ӌ,Ӣ
<ō,M,Ӝ,ӟ,Έ>ӑ)where ō:class,ʳ=>new Ӂ<ō>(Q<ɪ>(ә,F,ɏ,ͺ),ŵ=>ӌ(ŵ,ә,F,ɏ,ͺ),ŵ=>ӑ(ŵ,ә.ū(),F.ū(),ɏ.ū(),ͺ.ū()));delegate bool Ӌ<ō>(
ō đ);delegate bool ӛ<ō,M>(ō đ,ɰ<M>ǣ);delegate bool Ӟ<ō,M,Ӝ>(ō đ,ɰ<M>ǣ,ɰ<Ӝ>E);delegate bool ӡ<ō,M,Ӝ,ӟ>(ō đ,ɰ<M>ǣ,ɰ<Ӝ>E,ɰ<ӟ
>Φ);delegate bool ӣ<ō,M,Ӝ,ӟ,Έ>(ō đ,ɰ<M>ǣ,ɰ<Ӝ>E,ɰ<ӟ>Φ,ɰ<Έ>ơ);delegate object Ӎ<ō>(ō đ);delegate object Ӛ<ō,M>(ō đ,M ǣ);
delegate object ӝ<ō,M,Ӝ>(ō đ,M ǣ,Ӝ E);delegate object Ӡ<ō,M,Ӝ,ӟ>(ō đ,M ǣ,Ӝ E,ӟ Φ);delegate object Ӣ<ō,M,Ӝ,ӟ,Έ>(ō đ,M ǣ,Ӝ E,ӟ Φ,Έ
ơ);public interface ʳ{string ʺ{get;set;}}public abstract class Ӥ:ʳ{public string ʺ{get;set;}}public class ʩ:Ӥ{}public
class ʉ:Ӥ{}public class ʅ:Ӥ{}public class ʭ:Ӥ{}public class ΐ:Ӥ{}public class Γ:Ӥ{}public class ѓ:Ӥ{}public class є:Ӥ{}public
class ѱ:Ӥ{}public class ѳ:Ӥ{}public class Ѳ:Ӥ{}public class ϧ:Ӥ{}public class Ϊ:Ӥ{}public class Ώ:Ӥ{}public class ώ:Ӥ{}public
class ϙ:Ӥ{}public class ϩ:Ӥ{}public class ό:Ӥ{}public class ʦ:Ӥ{}public class ʽ:Ӥ{}public class ʲ:Ӥ{}public class Θ:Ӥ{}public
class Ε:Ӥ{}public class δ:Ӥ{}public class Ͷ:Ӥ{}public class γ:Ӥ{}public class ˍ:Ӥ{}public class ˣ:Ӥ{}public class Ͱ:Ӥ{}public
class ψ:Ӥ{}public class ʮ:Ӥ{}public class Ϥ:Ӥ{}public abstract class ҽ<ō>:Ӥ{public ō ũ;public ҽ(ō F){ũ=F;}}public class ϝ:ҽ<
bool>{public ϝ(bool F):base(F){}}public class ϡ:ҽ<bool>{public ϡ(bool ϣ):base(ϣ){}}public class ˏ:ҽ<ː>{public ˏ(ː ũ):base(ũ)
{}}public class ˮ:ҽ<ː>{public ˮ(ː ũ):base(ũ){}}public class ˠ:ҽ<ˡ>{public int ғ;public ˠ(ˡ ũ,int đ):base(ũ){ғ=đ;}}public
class ά:ҽ<bool>{public ά(bool F):base(F){}}public class ʣ:ҽ<bool>{public ʣ(bool ӥ=false):base(ӥ){}}public class ʿ:ҽ<bool>{
public ʿ(bool Ӧ=true):base(Ӧ){}}public class ˁ:ҽ<bool>{public ˁ(bool Ӧ=true):base(Ӧ){}}public class ʾ:Ӥ{public string ϖ;public
bool ϋ,ϗ;public ʾ(string ʜ,bool ӥ,bool ӧ){ϖ=ʜ;ϋ=ӥ;ϗ=ӧ;}}public class ʥ:Ӥ{public ʢ ϊ;public bool ϋ;public ʥ(ʢ ʜ,bool ӥ){ϊ=ʜ;ϋ
=ӥ;}}public class ˀ:ҽ<bool>{public string ϖ;public ˀ(string ʜ,bool Ӧ=true):base(Ӧ){ϖ=ʜ;}}public class ʏ:ҽ<ʛ>{public ʏ(ʛ ũ
):base(ũ){}}public class ʕ:Ӥ{public List<ʳ>ʞ;public ʕ(params ʳ[]Ө){ʞ=Ө.ToList();}}public class ʇ:ҽ<String>{public List<ʳ>
ʔ;public bool ʑ;public ʇ(String ũ,bool ө,params ʳ[]Ӫ):base(ũ){ʔ=Ӫ.ToList();ʑ=ө;}}public class Μ:ҽ<Ņ>{public Μ(Ņ ũ):base(ũ
){}}public class ʼ:ҽ<C>{public ʼ(C ũ):base(ũ){}}public class ʶ:ҽ<C>{public bool Ǥ;public ʶ(C ũ,bool ʻ=false):base(ũ){Ǥ=ʻ;
}}public class ʹ:ҽ<ų>{public ʹ(ų ũ):base(ũ){}}public class ο:ҽ<Ų>{public ο(Ų ũ):base(ũ){}}public class π:Ӥ{public ӫ O;
public Ų Ŵ;public π(ӫ Ӭ,Ų Ų){O=Ӭ;Ŵ=Ų;}}public class ʠ:ҽ<ʛ>{public ʠ(ʛ F):base(F){}}public class ʟ:ҽ<ʢ>{public ʟ(ʢ F):base(F){}
}public class ʫ:ҽ<ʛ>{public ʫ(ʛ ũ):base(ũ){}}public class ʴ:ҽ<bool>{public ʴ(bool ӭ):base(ӭ){}}public class ʚ:Ӥ{public
bool ϓ;public Ә<string>ϔ;public ʚ(Ә<string>Ӯ,bool ӭ=false){ϓ=ӭ;ϔ=Ӯ;}}public class κ:Ӥ{public bool μ,ν,ξ;public κ(bool Ǥ,bool
ӯ,bool Ӱ){μ=Ǥ;ν=ӯ;ξ=Ӱ;}}public class λ:ҽ<ʛ>{public bool ν,ξ;public λ(ʛ ũ,bool ӯ,bool Ӱ):base(ũ){ν=ӯ;ξ=Ӱ;}}public class Η:
ҽ<ӱ>{public Η(ӱ ũ):base(ũ){}}public class α:ҽ<Ĩ>{public α(Ĩ ũ):base(ũ){}}public class Ϋ:ҽ<ʛ>{public Ϋ(ʛ ũ):base(ũ){}}
public class ˉ:ҽ<Ӳ>{public ˉ(Ӳ ũ):base(ũ){}}public class ˆ:ҽ<ӳ>{public ˆ(ӳ ũ):base(ũ){}}public class ʬ:ҽ<Ρ>{public ʬ(Ρ ũ):base
(ũ){}}public class ʋ:ҽ<ӫ>{public ʋ(ӫ ũ):base(ũ){}}public class ʘ:ʋ{public ʘ(ʌ ũ):base(ũ){}}public class ʈ:ҽ<җ>{public ʈ(җ
ũ):base(ũ){}}public interface Ӵ{void ѽ();void Ѿ();}public abstract class Ĩ{public virtual void ӵ(){}public virtual Ĩ Ѹ()
=>this;public virtual Ĩ Ӷ(Func<Ĩ,bool>Ȉ)=>Ȉ(this)?this:null;public abstract bool ӷ();}public class Ϣ:Ĩ{public Ĩ ϟ;public
bool ϣ;public override bool ӷ(){Ĕ Į;if(ϣ){Į=new Ĕ(ϟ.Ѹ(),"Async","Unknown");Į.Ģ=á.Ė;á.Ӹ(Į);á.Ė.ѿ<ϥ>()?.ӹ?.Add(Į);}else{Į=new
Ĕ(ϟ.Ѹ(),"Queued","Unknown");á.Ӻ(Į);}if(ϟ is ϒ){Į.ӻ(((ϒ)ϟ).А());}Į.Ӽ=new Dictionary<string,ʛ>(á.ӽ().Ӽ);return true;}}
public class ύ:Ĩ{public ʛ ʜ;public ύ(ʛ F){ʜ=F;}public override bool ӷ(){Ӿ(Ż(ʜ.ū()));return true;}}public class ϒ:Ĩ{public bool
ϓ;public Ә<string>А;public List<ʛ>ӿ;public Ĩ ʵ;public ϒ(bool ӭ,Ә<string>ē,List<ʛ>Ѐ){ϓ=ӭ;А=ē;ӿ=Ѐ;ʵ=null;}public override
bool ӷ(){Ĕ Ė=á.ӽ();if(ʵ==null){var ē=А();Б Ӯ;if(!á.ʙ.TryGetValue(ē,out Ӯ))throw new Č("Invalid Function Name: "+ē);var ԁ=Ӯ.Ԁ
.Count;if(ӿ.Count!=ԁ)throw new Č($"Function {ē} expects {ԁ} parameters");ʵ=Ӯ.ʵ.Ѹ();for(int ǿ=0;ǿ<ԁ;ǿ++)Ė.Ӽ[Ӯ.Ԁ[ǿ]]=new ђ(
ӿ[ǿ].ū().Ԃ());if(ϓ){Ė.Ĩ=ʵ;Ė.ӻ(ē);}}return!ϓ&&ʵ.ӷ();}public override Ĩ Ѹ()=>new ϒ(ϓ,А,ӿ);public override void ӵ()=>ʵ=null;
public override Ĩ Ӷ(Func<Ĩ,bool>Ȉ)=>ʵ.Ӷ(Ȉ)??base.Ӷ(Ȉ);}public class ϕ:Ĩ{public String ϖ;public ʛ ʜ;public bool ϗ,ϋ;public ϕ(
string ē,ʛ ʪ,bool ӥ,bool ӧ){ϖ=ē;ʜ=ʪ;ϋ=ӥ;ϗ=ӧ;}public override bool ӷ(){ʛ ũ=ϋ?ʜ:new ђ(ʜ.ū().Ԃ());if(ϗ){á.ԃ(ϖ,ũ);}else{á.ӽ().Ӽ[ϖ]
=ũ;}return true;}}public class Ϙ:Ĩ{public String ϖ;public bool ǎ;public ʛ ʜ;public Ϙ(String Ԅ,bool Ǆ,ʛ ԅ){ϖ=Ԅ;ǎ=Ǆ;ʜ=ԅ;}
public override bool ӷ(){Ũ ű=ʜ.ū();if(!ǎ)ű=ű.Ǎ();ʛ Ɇ=new ђ(á.Ԇ(ϖ).ū().ǋ(ű));if(á.ӽ().Ӽ.ContainsKey(ϖ)){á.ӽ().Ӽ[ϖ]=Ɇ;}else{á.ԇ[
ϖ]=Ɇ;}return true;}}public class ω:Ĩ{public ʢ ɺ;public ʛ ũ;public bool ϋ;public ω(ʢ Ԉ,ʛ F,bool ӥ){ɺ=Ԉ;ũ=F;ϋ=ӥ;}public
override bool ӷ(){ɺ.Ŭ(ϋ?ũ:new ђ(ũ.ū().Ԃ()));return true;}}public delegate bool Ҏ(Ĕ Ė);public Ӵ Ѽ(string ԉ){Ӵ Ԋ=ӽ().ѿ<Ӵ>(ϟ=>(ϟ as
Ϯ)?.ν??true);if(Ԋ==null)throw new Č($"Invalid use of {ԉ} command");return Ԋ;}public class ҏ:Ĩ{public Ҏ Ґ;public override
bool ӷ()=>Ґ(á.Ė);}public class ϥ:Ĩ,Ӵ{public List<Ĕ>ӹ=Q<Ĕ>();public Ĩ Ϧ;bool ԋ=false;public override bool ӷ()=>(ԋ=ԋ||Ϧ.ӷ())&&
!á.Ĝ.Intersect(ӹ).Any();public override Ĩ Ӷ(Func<Ĩ,bool>Ȉ){return Ϧ.Ӷ(Ȉ)??base.Ӷ(Ȉ);}public override Ĩ Ѹ()=>new ϥ{Ϧ=Ϧ};
public override void ӵ(){ӹ.Clear();Ϧ.ӵ();ԋ=false;}public void ѽ(){ӵ();ԋ=true;}public void Ѿ(){ѽ();}}public class ϐ:Ĩ{public ʛ
Ԍ;double ԍ=-1;public ϐ(ʛ ʜ){Ԍ=ʜ;}public override Ĩ Ѹ()=>new ϐ(Ԍ);public override void ӵ(){ԍ=-1;}public override bool ӷ(){
if(ԍ<0){ԍ=Ž(Ԍ.ū())*1000;return false;}ԍ-=á.Runtime.TimeSinceLastRun.TotalMilliseconds;return ԍ<=5;}}public class Ϟ:Ĩ{
public ʛ ϛ;bool Ԏ;public Ϟ(ʛ F,bool ԏ){ϛ=F;Ԏ=ԏ;}public override bool ӷ(){var Ԑ=Ż(ϛ.ū());if(Ԏ)á.IGC.RegisterBroadcastListener(Ԑ
);else á.ԑ(Ԓ=>Ԓ.Tag==Ԑ,Ԓ=>á.IGC.DisableBroadcastListener(Ԓ));return true;}}public class Ϝ:Ĩ{public ʛ Ϛ,ϛ;public Ϝ(ʛ ԓ,ʛ Ԕ
){Ϛ=ԓ;ϛ=Ԕ;}public override bool ӷ(){á.IGC.SendBroadcastMessage(Ż(ϛ.ū()),Ż(Ϛ.ū()));return true;}}public class ĩ:Ĩ{public
override bool ӷ()=>true;}public class υ:Ĩ{public ӫ ԕ;public Action<Y,Object>Ӑ;public υ(ӫ Ԗ,Action<Y,Object>К){ԕ=Ԗ;Ӑ=К;}public
override bool ӷ(){Y Ǖ=Ħ.č(ԕ.ԗ());ԕ.Ԙ().ForEach(φ=>Ӑ(Ǖ,φ));return true;}}public class β:Ĩ{public ӫ ԙ,ʷ;public ʛ Ԛ,ԛ;public β(ӫ Ԝ,
ӫ ԝ,ʛ Ԟ,ʛ ԟ){ԙ=Ԝ;ʷ=ԝ;Ԛ=Ԟ;ԛ=ԟ;}public override bool ӷ(){if(ԙ.ԗ()!=S.l||ʷ.ԗ()!=S.l)throw new Č(
"Transfers can only be executed on cargo block types");var Ȉ=á.Ȇ(á.ȇ(Ż((ԛ??Ԛ).ū())));var Ȋ=Q<MyInventoryItem>();var Ԡ=ʷ.Ԙ().Cast<IMyInventory>().Where(ǿ=>!ǿ.IsFull).ToList()
;var ԡ=ԙ.Ԙ().Cast<IMyInventory>().Where(ǿ=>Ԡ.All(ʷ=>ǿ.Owner.EntityId!=ʷ.Owner.EntityId));MyFixedPoint Ԣ=MyFixedPoint.
MaxValue;if(ԛ!=null)Ԣ=(MyFixedPoint)Ž(Ԛ.ū());int ԣ=0;foreach(IMyInventory Ԥ in ԡ){Ԥ.GetItems(Ȋ,Ȉ);for(int ǿ=0;ǿ<Ԡ.Count;ǿ++){
foreach(MyInventoryItem ȋ in Ȋ){var ԥ=Ԡ[ǿ];var Ԧ=Ԥ.CurrentMass;Ԥ.TransferItemTo(ԥ,ȋ,Ԣ);Ԣ-=(Ԧ-Ԥ.CurrentMass);if(Ԣ<=MyFixedPoint.
Zero||++ԣ>=á.maxItemTransfers)return true;if(ԥ.IsFull){Ԡ.RemoveAt(ǿ--);break;}}}}return true;}}public class Ϯ:Ĩ,Ӵ{public ʛ Σ
;public bool ν,ԧ,Ա,Բ,Գ;public Ĩ Դ,Ե;public Ϯ(ʛ Զ,Ĩ Է,Ĩ ϭ,bool ӯ){Σ=Զ;Դ=Է;Ե=ϭ;ν=ӯ;}public override bool ӷ(){if(Գ){ӵ();
return true;}bool Թ=Ը();bool Ժ=Թ?Դ.ӷ():Ե.ӷ();Բ=!Ժ;if(Բ)return false;ӵ();return ν?!Թ:Ժ;}public override void ӵ(){Դ.ӵ();Ե.ӵ();ԧ=
false;Բ=false;Գ=false;}public override Ĩ Ѹ()=>new Ϯ(Σ,Դ.Ѹ(),Ե.Ѹ(),ν);bool Ը(){if((!Բ&&ν)||!ԧ){Ա=ż(Σ.ū());ԧ=true;}return Ա;}
public override Ĩ Ӷ(Func<Ĩ,bool>Ȉ)=>(Ա?Դ.Ӷ(Ȉ):Ե.Ӷ(Ȉ))??base.Ӷ(Ȉ);public void Ѿ(){ӵ();}public void ѽ(){Գ=true;}}public class Ϡ:
Ĩ{public List<Ĩ>Ի,Լ=null;public ʛ Խ;int Ծ;public Ϡ(List<Ĩ>Ի,int Կ=1):this(Ի,Ɖ(Կ)){}public Ϡ(List<Ĩ>Ө,ʛ Կ){Ի=Ө;Խ=Կ;}public
override bool ӷ(){if((Լ?.Count??0)==0){Լ=Ի.Select(Φ=>Φ.Ѹ()).ToList();if(Ծ==0)Ծ=(int)Math.Round(Ž(Խ.ū()));Ծ-=1;}while(Լ.Count>0){
if(Լ[0].ӷ()){Լ.RemoveAt(0);}else{break;}}if(Լ?.Count>0)return false;if(Ծ<=0)return true;ӵ();return false;}public override
void ӵ(){Լ=null;}public override Ĩ Ѹ()=>new Ϡ(Ի,Խ);public override Ĩ Ӷ(Func<Ĩ,bool>Ȉ)=>Լ[0].Ӷ(Ȉ)??base.Ӷ(Ȉ);}public class Ϩ:
Ĩ,Ӵ{public string Հ;public ʛ ɺ;public Ĩ ϟ;List<ʛ>Ձ=null;bool Ղ=true;public Ϩ(string Ճ,ʛ Մ,Ĩ Ĩ){Հ=Ճ;ɺ=Մ;ϟ=Ĩ;}public
override bool ӷ(){Ձ=Ձ??Ƿ(ɺ.ū()).Յ();if(Ղ&&Ձ.Count==0)return true;if(Ղ&&Ձ.Count>0){á.ӽ().Ӽ[Հ]=Ձ[0];Ձ.RemoveAt(0);Ղ=false;}if(!Ղ)Ղ
=ϟ.ӷ();if(Ղ)ϟ.ӵ();return Ղ&&((Ձ?.Count??0)==0);}public override void ӵ(){Ձ=null;Ղ=true;ϟ.ӵ();}public override Ĩ Ѹ()=>new
Ϩ(Հ,ɺ,ϟ.Ѹ());public override Ĩ Ӷ(Func<Ĩ,bool>Ȉ)=>ϟ.Ӷ(Ȉ)??base.Ӷ(Ȉ);public void ѽ(){Ղ=true;Ձ=Q<ʛ>();ϟ.ӵ();}public void Ѿ()
{Ղ=true;ϟ.ӵ();}}public interface ӫ{List<Object>Ԙ();S ԗ();}public class Τ:ӫ{public ӫ O;public ӱ Σ;public Τ(ӫ Ն,ӱ Շ){O=Ն;Σ=
Շ;}public S ԗ()=>O.ԗ();public List<object>Ԙ()=>O.Ԙ().Where(E=>Σ(E,O.ԗ())).ToList();}public class ʯ:ӫ{public ӫ O;public ʛ
ʡ;public ʯ(ӫ Ն,ʛ Ո){O=Ն;ʡ=Ո;}public S ԗ()=>O.ԗ();public List<Object>Ԙ(){var Չ=O.Ԙ();Y E=Ħ.č(ԗ());return Ƿ(ʡ.ū()).ɒ.Select
(F=>F.ū()).SelectMany(ŵ=>{if(ŵ.Ǯ==Ĭ.ŋ)return Չ.GetRange((int)Ž(ŵ),1);else if(ŵ.Ǯ==Ĭ.Ľ){var Ę=Ż(ŵ);return Չ.Where(ɯ=>Ę==E.
ǚ(ɯ));}else return ɢ<Object>();}).ToList();}}public class җ{public S?ċ;public bool?Ҙ;}public class ʌ:ӫ{public җ Պ;public
ʛ O;public ʌ(җ җ,ʛ Ӭ){Պ=җ;O=Ӭ;}public List<Object>Ԙ(){var Ջ=Ż(O.ū());var Ս=Ռ();var Վ=Ս.ċ.Value;var Չ=Q<Object>();if(!(Ս.Ҙ
??false))Չ=Ħ.ę(Վ,Ջ);if(Ս.Ҙ??Չ.Count==0)Չ=Ħ.ě(Վ,Ջ);return Չ;}public S ԗ()=>Ռ().ċ.Value;җ Ռ(){if(Պ.ċ!=null)return Պ;var Ջ=Ż(
O.ū());var Ѐ=á.Ъ(á.Ѝ(Ջ));var ċ=ʗ<ʈ>(Ѐ);if(ċ==null)throw new Č("Cannot parse block type from selector: "+Ջ);return ʍ(ċ.ũ,ʗ
<ʉ>(Ѐ));}}public static җ ʍ(җ O,ʉ Ҙ)=>new җ{ċ=O?.ċ,Ҙ=Ҙ!=null?true:O?.Ҙ};public class ʧ:ӫ{public S?ċ;public ʧ(S?Ǘ){ċ=Ǘ;}
public S ԗ()=>ċ??S.á;public List<object>Ԙ()=>Ħ.ď(ċ)??Ħ.ę(ԗ());}public class ʨ:ӫ{public S ċ;public ʨ(S Ǘ){ċ=Ǘ;}public S ԗ()=>ċ;
public List<object>Ԙ()=>Ħ.ę(ċ);}public delegate Ũ ӳ(IEnumerable<object>N,Func<object,Ũ>я);public ӳ ѐ=(N,я)=>N.Select(я).
Aggregate((Ũ)null,(ǣ,E)=>ǣ?.ǋ(E)??E)??ŷ(0),σ=(N,я)=>{var Տ=N.Select(я).DefaultIfEmpty(ŷ(ƈ()));return(Տ.Count()==1)?Տ.First():Տ.
All(F=>F.Ǯ==Ĭ.ŋ)?Տ.Aggregate((ǣ,E)=>ǣ.ǋ(E)):ŷ(ƈ(Տ.Select(F=>new ђ(F))));};public delegate bool Ӳ(int э,int ю);public Ӳ ˌ=(э
,ю)=>э==ю,Ω=(э,ю)=>ю==0;public class T<ō,M>{public Dictionary<MyTuple<ō,string>,M>Ր=Ź<MyTuple<ō,string>,M>();public M ė(ō
Ց,string Ւ,Func<string,M>Փ){var Ɵ=MyTuple.Create(Ց,Ւ);return Ր.ContainsKey(Ɵ)?Ր[Ɵ]:Ր[Ɵ]=Փ(Ւ);}public void ɦ()=>Ր.Clear();
}public class ѵ:Exception{public Ѷ Ѷ;public ѵ(Ѷ Ք){Ѷ=Ք;}}public class Ī:Exception{}public class Ы:Exception{public Ы(
string Օ):base(Օ){}}public class Č:Exception{public Č(string Օ):base(Օ){}}static Ǩ ƈ(IEnumerable<ʛ>Ǳ=null)=>new Ǩ{ɒ=(Ǳ??ɢ<ʛ>()
).Select(Ֆ).ToList()};public class Ǩ{public List<ɐ>ɒ;public List<ʛ>Յ()=>ɒ.ConvertAll(F=>(ʛ)F);public ʛ ū(Ũ Ɵ){switch(Ɵ.Ǯ)
{case Ĭ.ŋ:return ɒ[(int)Ž(Ɵ)];case Ĭ.Ľ:var ՙ=Ż(Ɵ);return ɒ.Where(F=>F.ɔ()==ՙ).Cast<ʛ>().DefaultIfEmpty(ʱ()).First();
default:throw new Č("Cannot lookup collection value for value: "+Ɵ.ũ);}}public void Ŭ(Ũ Ɵ,ʛ ũ){if(Ɵ.Ǯ==Ĭ.ŋ){ɒ[(int)Ž(Ɵ)]=Ֆ(ũ);}
else if(Ɵ.Ǯ==Ĭ.Ľ){var ՙ=Ż(Ɵ);ɐ ա=ɒ.Where(F=>F.ɔ()==ՙ).FirstOrDefault();if(ա==null)ɒ.Add(new ɐ(Ɖ(ՙ),ũ));else ա.բ=ũ;}else
throw new Č("Cannot set collection value by value: "+Ɵ.ũ);}public Ǩ Ĥ(Ǩ Ҵ){var գ=new HashSet<string>(Ҵ.ɒ.Where(Ⱦ=>Ⱦ.ɓ()).
Select(Ⱦ=>Ⱦ.ɔ()).Distinct());var դ=ɒ.Where(Ⱦ=>!Ⱦ.ɓ()||!գ.Contains(Ⱦ.ɔ()));return ƈ(դ.Concat(Ҵ.ɒ.Select(Ⱦ=>Ⱦ.Ԃ())));}public Ǩ զ
(Ǩ Ҵ){Ǩ Ҹ=ƈ(ɒ.Select(F=>new ɐ(F.ե,F.բ)));Ҵ.ɒ.ForEach(F=>Ҹ.Ŭ(F.ū(),null));return ƈ(Ҹ.ɒ.Where(Ⱦ=>Ⱦ.բ!=null));}public Ǩ է()
=>ƈ(ɒ.Where(F=>F.ɓ()).Select(F=>Ɖ(F.ɔ())));public Ǩ ը()=>ƈ(ɒ.Select(F=>F.բ));public Ǩ Ԃ()=>ƈ(ɒ.Select(Ⱦ=>Ⱦ.Ԃ()));public
String Ӿ()=>$"[{string.Join(",",ɒ.Select(Ⱦ=>Ⱦ.Ӿ()))}]";}public Dictionary<String,List<թ>>ժ=Ź<string,List<թ>>();public
Dictionary<String,MyDefinitionId>ի=Ź<string,MyDefinitionId>();public Func<String,MyDefinitionId>խ=լ;public void ր(){ծ(ф("ore"),կ()
);հ("Cobalt");հ("Nickel");հ("Gold");ծ(ф("ice"),կ("Ice"));հ("Iron");հ("Magnesium");հ("Platinum");հ("Silicon");հ("Silver");
հ("Uranium");ծ(ф("stone"),կ("Stone"));ծ(ф("scrap metal"),կ("Scrap"));ծ(ф("ingots"),ձ());ղ("Cobalt");ղ("Nickel");ղ("Gold")
;ծ(ф("gravel"),ձ("Stone"));ղ("Iron");ծ(ф("magnesium ingot","magnesium powder"),ձ("Magnesium"));ղ("Platinum");ծ(ф(
"silicon ingot","silicon wafer"),ձ("Silicon"));ղ("Silver");ղ("Uranium");ծ(ф("old scrap metal"),ձ("Scrap"));ծ(ф("ammo","ammunition"),ճ()
);մ(ф("missile ammo"),"Missile200mm");յ(ф("gatling ammo"),"NATO_25x184mmMagazine",ճ("NATO_25x184mm"));յ(ф("rifle ammo"),
"AutomaticRifleGun_Mag_20rd",ճ("NATO_5p56x45mm"),ճ("AutomaticRifleGun_Mag_20rd"));մ(ф("rapid rifle ammo"),"RapidFireAutomaticRifleGun_Mag_50rd");մ(ф
("precision rifle ammo"),"PreciseAutomaticRifleGun_Mag_5rd");մ(ф("elite rifle ammo"),"UltimateAutomaticRifleGun_Mag_30rd"
);մ(ф("pistol ammo"),"SemiAutoPistolMagazine");մ(ф("rapid pistol ammo"),"FullAutoPistolMagazine");մ(ф("elite pistol ammo"
),"ElitePistolMagazine");մ(ф("autocannon ammo"),"AutocannonClip");մ(ф("artillery ammo"),"LargeCalibreAmmo");մ(ф(
"cannon ammo"),"MediumCalibreAmmo");մ(ф("large railgun ammo"),"LargeRailgunAmmo");մ(ф("small railgun ammo"),"SmallRailgunAmmo");ծ(ф(
"components"),ն());շ(ф("bulletproof glass"),"BulletproofGlass","");շ(ф("canvas"),"Canvas","");շ(ф("computer"),"Computer");շ(ф(
"construction component"),"Construction");շ(ф("detector component"),"Detector");շ(ф("display"),"Display","");շ(ф("explosives"),"Explosives");շ(ф
("girder"),"Girder");շ(ф("gravity component"),"GravityGenerator");շ(ф("interior plate"),"InteriorPlate","");շ(ф(
"large steel tube"),"LargeTube","");շ(ф("medical component"),"Medical");շ(ф("metal grid"),"MetalGrid","");շ(ф("motor"),"Motor");շ(ф(
"power cell"),"PowerCell","");շ(ф("radio component"),"RadioCommunication");շ(ф("reactor component"),"Reactor");շ(ф(
"small steel tube"),"SmallTube","");շ(ф("solar cell"),"SolarCell","");շ(ф("steel plate"),"SteelPlate","");շ(ф("superconductor"),
"Superconductor","");շ(ф("thruster component"),"Thrust");շ(ф("zone chip"),"ZoneChip","");ծ(ф("tools"),ո("Grinder","Drill","Welder"));չ(
"grinder","AngleGrinder");չ("drill","HandDrill");չ("welder","Welder");ծ(ф("weapons"),ո("Rifle","Pistol","HandHeldLauncher"));պ(ф(
"rifle"),"AutomaticRifle");պ(ф("rapid rifle"),"RapidFireAutomaticRifle");պ(ф("precision rifle"),"PreciseAutomaticRifle");պ(ф(
"elite rifle"),"UltimateAutomaticRifle");պ(ф("pistol"),"SemiAutoPistol");պ(ф("rapid pistol"),"FullAutoPistol");յ(ф("elite pistol"),
"EliteAutoPistol",ջ("ElitePistolItem"));պ(ф("rocket launcher"),"BasicHandHeldLauncher");պ(ф("precision rocket launcher"),
"AdvancedHandHeldLauncher");թ ս=ռ("MyObjectBuilder_OxygenContainerObject","OxygenBottle");թ վ=ռ("MyObjectBuilder_GasContainerObject",
"HydrogenBottle");ծ(ф("bottles"),ս,վ);յ(ф("oxygen bottle"),"OxygenBottle",ս);յ(ф("hydrogen bottle"),"HydrogenBottle",վ);ծ(ф(
"consumables"),տ());ծ(ф("clang cola"),տ("ClangCola"));ծ(ф("cosmic coffee"),տ("CosmicCoffee"));ծ(ф("medkit"),տ("Medkit"));ծ(ф(
"powerkit"),տ("Powerkit"));ծ(ф("package"),ռ("MyObjectBuilder_Package","Package"));ծ(ф("datapad"),ռ("MyObjectBuilder_Datapad",
"Datapad"));ծ(ф("space credit"),ռ("MyObjectBuilder_PhysicalObject","SpaceCredit"));}void հ(String ց)=>ծ(ф(ց.ToLower()+" ore"),կ(ց
));void ղ(String ւ)=>ծ(ф(ւ.ToLower()+" ingot"),ձ(ւ));void մ(String[]ҍ,String փ)=>յ(ҍ,փ,ճ(փ));void պ(String[]ҍ,String ք)=>
յ(ҍ,ք,ջ(ք+"Item"));void շ(String[]ҍ,String օ,String ֆ="Component")=>յ(ҍ,օ+ֆ,ն(օ));void չ(String և,String א){յ(ф(և),א,ջ(א+
"Item"));յ(ф("enhanced "+և),א+"2",ջ(א+"2Item"));յ(ф("proficient "+և),א+"3",ջ(א+"3Item"));յ(ф("elite "+և),א+"4",ջ(א+"4Item"));}
void յ(String[]ҍ,String ב,params թ[]ג){ծ(ҍ,ג);ד(ҍ,ב);}void ד(string[]ҍ,string ב){var ה=խ(ב);foreach(String ҙ in ҍ)ի.Add(ҙ,ה)
;}void ծ(String[]ҍ,params թ[]ג){foreach(String ҙ in ҍ)ժ.Add(ҙ.ToLower(),ג.ToList());}public static MyDefinitionId լ(
string ב){MyDefinitionId Ӯ;MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition",ב,out Ӯ);return Ӯ;}public delegate
bool թ(MyInventoryItem ȋ);public List<թ>ȇ(String ȅ)=>ו(ȅ,ժ,ǿ=>Q(ז(ǿ.Split('.')))).SelectMany(ͺ=>ͺ).ToList();public List<
MyDefinitionId>Ȕ(String ȅ)=>ו(ȅ,ի,խ);List<ō>ו<ō>(string ȅ,Dictionary<string,ō>Ǳ,Func<string,ō>ח)=>ȅ.Split(',').Select(ǿ=>Ǳ.
GetValueOrDefault(ǿ.Trim().ToLower(),ח(ǿ))).ToList();public թ տ(String Ʃ=null)=>ռ("MyObjectBuilder_ConsumableItem",Ʃ);public թ ն(String Ʃ
=null)=>ռ("MyObjectBuilder_Component",Ʃ);public թ ճ(String Ʃ=null)=>ռ("MyObjectBuilder_AmmoMagazine",Ʃ);public թ ձ(String
Ʃ=null)=>ռ("MyObjectBuilder_Ingot",Ʃ);public թ կ(String Ʃ=null)=>ռ("MyObjectBuilder_Ore",Ʃ);public թ ջ(String Ʃ=null)=>ռ(
"MyObjectBuilder_PhysicalGunObject",Ʃ);public թ ո(params String[]ю)=>ǿ=>ǿ.Type.TypeId.Equals("MyObjectBuilder_PhysicalGunObject")&&ю.Any(Ę=>ǿ.Type.
SubtypeId.Contains(Ę));public թ ռ(String ט,String Ʃ=null)=>ǿ=>(string.IsNullOrEmpty(ט)||ǿ.Type.TypeId.Equals(ט))&&(string.
IsNullOrEmpty(Ʃ)||ǿ.Type.SubtypeId.Equals(Ʃ));public թ ז(string[]י)=>י.Count()==2?ռ(י[0],י[1]):ռ("",י[0]);public Func<MyInventoryItem
,bool>Ȇ(List<թ>ג)=>E=>ג.Any(ך=>ך(E));public delegate Ũ כ(Ũ ǣ);public delegate Ũ ל(Ũ ǣ,Ũ E);Dictionary<MyTuple<ː,Ĭ>,כ>ם=Ź<
MyTuple<ː,Ĭ>,כ>();Dictionary<MyTuple<ˡ,Ĭ,Ĭ>,ל>מ=Ź<MyTuple<ˡ,Ĭ,Ĭ>,ל>();public static Ũ ы(ː Ǘ,Ũ ǣ)=>á.ם.GetValueOrDefault(MyTuple
.Create(Ǘ,ǣ.Ǯ),ŵ=>{throw new Č($"Cannot perform operation: {а[Ǘ]} on type: {г[ŵ.Ǯ]}");})(ǣ);public static Ũ ы(ˡ Ǘ,Ũ ǣ,Ũ E
)=>á.מ.GetValueOrDefault(MyTuple.Create(Ǘ,ǣ.Ǯ,E.Ǯ),(ŵ,ן)=>{throw new Č(
$"Cannot perform operation: {в[Ǘ]} on types: {г[ŵ.Ǯ]}, {г[ן.Ǯ]}");})(ǣ,E);void ע<ō>(ː Ǘ,Func<ō,object>נ){foreach(Ĭ đ in ס(typeof(ō)))ם[MyTuple.Create(Ǘ,đ)]=ŵ=>ŷ(נ((ō)ŵ.ũ));}void ף<ō,M>
(ˡ Ǘ,Func<ō,M,object>נ){foreach(var Ⱦ in ס(typeof(ō)).SelectMany(đ=>ס(typeof(M)),(đ,ә)=>MyTuple.Create(Ǘ,đ,ә)))מ[Ⱦ]=(ŵ,ן)
=>ŷ(נ((ō)ŵ.ũ,(M)ן.ũ));}public void צ(){ע<Ǩ>(ː.Ѡ,ǣ=>ǣ.ū(ŷ(פ.Next(ǣ.ɒ.Count))).ū().ũ);ע<object>(ː.ͱ,ǣ=>ǣ);ע<string>(ː.ͱ,ǣ=>{
Ũ ץ;return ʝ(ǣ,out ץ)?ץ.ũ:ǣ;});ף<Ǩ,object>(ˡ.Ѱ,(ǣ,E)=>Ĥ(ǣ,E));ף<object,Ǩ>(ˡ.Ѱ,(ǣ,E)=>Ĥ(ǣ,E));ף<Ǩ,object>(ˡ.ˢ,(ǣ,E)=>ǣ.զ(Ƿ
(ŷ(E))));ף<float,float>(ˡ.İ,(ǣ,E)=>{var ǻ=Ȅ((int)Math.Min(ǣ,E),(int)(Math.Abs(E-ǣ)+1)).Select(ǿ=>Ɖ(ǿ));if(ǣ>E)ǻ=ǻ.Reverse
();return ƈ(ǻ);});ף<string,string>(ˡ.Ѯ,(ǣ,E)=>ƈ(ǣ.Split(ĥ(Ż(ŷ(E))).ToArray(),StringSplitOptions.None).Select(Ɖ)));ע<Ǩ>(ː.
ѥ,ǣ=>ǣ.է());ע<Ǩ>(ː.Ѧ,ǣ=>ǣ.ը());ע<Ǩ>(ː.ˑ,ǣ=>ƈ(ǣ.ɒ.Select(E=>E).Reverse()));ע<Ǩ>(ː.ў,ǣ=>ƈ(ǣ.ɒ.OrderBy(Ⱦ=>Ⱦ)));ע<Ǩ>(ː.ѡ,ǣ=>ƈ
(ǣ.ɒ.OrderBy(Ⱦ=>פ.Next())));ע<bool>(ː.ˑ,ǣ=>!ǣ);ף<bool,bool>(ˡ.Β,(ǣ,E)=>ǣ&&E);ף<bool,bool>(ˡ.Δ,(ǣ,E)=>ǣ||E);ף<bool,bool>(ˡ
.Ѫ,(ǣ,E)=>ǣ^E);ף<bool,bool>(ˡ.Ѱ,(ǣ,E)=>ǣ||E);ף<String,object>(ˡ.ь,(ǣ,E)=>ǣ.Contains(Ż(ŷ(E))));ף<Ǩ,object>(ˡ.ь,(ǣ,E)=>Ƿ(ŷ(
E)).ɒ.Select(F=>F.ū().ũ).Except(ǣ.ɒ.Select(F=>F.ū().ũ)).Count()==0);ף<bool,bool>(ˡ.б,(ǣ,E)=>ǣ.CompareTo(E));ף<string,
string>(ˡ.б,(ǣ,E)=>ǣ.CompareTo(E));ף<float,float>(ˡ.б,(ǣ,E)=>ǣ.CompareTo(E));ף<Vector3D,Vector3D>(ˡ.б,(ǣ,E)=>!ǣ.Equals(E));ף<
Color,Color>(ˡ.б,(ǣ,E)=>ǣ.PackedValue.CompareTo(E.PackedValue));ף<Vector3D,float>(ˡ.б,(ǣ,E)=>Ž(ŷ(ǣ.Length())).CompareTo(E));ף
<float,Vector3D>(ˡ.б,(ǣ,E)=>ǣ.CompareTo(Ž(ŷ(E.Length()))));ף<Ǩ,Ǩ>(ˡ.б,(ǣ,E)=>!Enumerable.SequenceEqual(ǣ.ɒ,E.ɒ));ע<float>
(ː.ˑ,ǣ=>-ǣ);ע<float>(ː.і,ǣ=>Math.Abs(ǣ));ע<float>(ː.ї,ǣ=>Math.Sqrt(ǣ));ע<float>(ː.ј,ǣ=>Math.Sin(ǣ));ע<float>(ː.љ,ǣ=>Math.
Cos(ǣ));ע<float>(ː.њ,ǣ=>Math.Tan(ǣ));ע<float>(ː.ћ,ǣ=>Math.Asin(ǣ));ע<float>(ː.ќ,ǣ=>Math.Acos(ǣ));ע<float>(ː.ѝ,ǣ=>Math.Atan(
ǣ));ע<float>(ː.ˬ,ǣ=>Math.Round(ǣ));ע<float>(ː.џ,ǣ=>Math.Log(ǣ));ע<Vector3D>(ː.і,ǣ=>ǣ.Length());ע<Vector3D>(ː.ї,ǣ=>Math.
Sqrt(ǣ.Length()));ע<float>(ː.Ѥ,ǣ=>ǣ/60);ע<float>(ː.Ѡ,ǣ=>פ.Next((int)ǣ));ע<float>(ː.Ѣ,ǣ=>Math.Sign(ǣ));ף<float,float>(ˡ.Ѱ,(ǣ,
E)=>ǣ+E);ף<float,float>(ˡ.ˢ,(ǣ,E)=>ǣ-E);ף<float,float>(ˡ.ѫ,(ǣ,E)=>ǣ*E);ף<float,float>(ˡ.Ѭ,(ǣ,E)=>ǣ/E);ף<float,float>(ˡ.ѭ,
(ǣ,E)=>ǣ%E);ף<float,float>(ˡ.Ѫ,(ǣ,E)=>Math.Pow(ǣ,E));ף<float,float>(ˡ.ˬ,(ǣ,E)=>Math.Round(ǣ,(int)E));ף<Vector3D,Vector3D>
(ˡ.Ѩ,(ǣ,E)=>ǣ.Dot(E));ף<Color,Vector3D>(ˡ.Ѩ,(ǣ,E)=>(ǣ.ToVector3()*255).Dot(E));ף<Vector3D,Vector3D>(ˡ.Ѫ,(ǣ,E)=>Math.Acos(
ǣ.Dot(E)/(ǣ.Length()*E.Length()))*Œ);ע<string>(ː.ˑ,ǣ=>new string(ǣ.Reverse().ToArray()));ע<object>(ː.ѧ,ǣ=>г[ŷ(ǣ).Ǯ]);ף<
string,object>(ˡ.Ѱ,(ǣ,E)=>ǣ+Ż(ŷ(E)));ף<object,string>(ˡ.Ѱ,(ǣ,E)=>Ż(ŷ(ǣ))+E);ף<string,string>(ˡ.ˢ,(ǣ,E)=>ǣ.Contains(E)?ǣ.Remove
(ǣ.IndexOf(E))+ǣ.Substring(ǣ.IndexOf(E)+E.Length):ǣ);ף<string,string>(ˡ.ѭ,(ǣ,E)=>ǣ.Replace(E,""));ף<string,float>(ˡ.ˢ,(ǣ,
E)=>E>=ǣ.Length?"":ǣ.Substring(0,(int)(ǣ.Length-E)));ף<object,string>(ˡ.ͱ,(ǣ,E)=>ж[E](ŷ(ǣ)));ף<Ǩ,string>(ˡ.ѯ,(ǣ,E)=>
string.Join(Ż(ŷ(E)),ǣ.ɒ.Select(F=>Ż(F.ū()))));ע<Vector3D>(ː.Ѣ,ǣ=>Vector3D.Sign(ǣ));ע<Vector3D>(ː.ˬ,ǣ=>Vector3D.Round(ǣ,0));ע<
Vector3D>(ː.ˑ,ǣ=>-ǣ);ף<Vector3D,Vector3D>(ˡ.Ѱ,(ǣ,E)=>ǣ+E);ף<Vector3D,Vector3D>(ˡ.ˢ,(ǣ,E)=>ǣ-E);ף<Vector3D,float>(ˡ.Ѱ,(ǣ,E)=>
Vector3D.Multiply(ǣ,(ǣ.Length()+E)/ǣ.Length()));ף<Vector3D,float>(ˡ.ˢ,(ǣ,E)=>Vector3D.Multiply(ǣ,(ǣ.Length()-E)/ǣ.Length()));ף<
Vector3D,Vector3D>(ˡ.ѫ,(ǣ,E)=>Vector3D.Cross(ǣ,E));ף<Vector3D,Vector3D>(ˡ.Ѭ,(ǣ,E)=>Vector3D.Divide(ǣ,E.Length()));ף<Vector3D,
float>(ˡ.ѫ,(ǣ,E)=>Vector3D.Multiply(ǣ,E));ף<Vector3D,float>(ˡ.Ѭ,(ǣ,E)=>Vector3D.Divide(ǣ,E));ף<float,Vector3D>(ˡ.ѫ,(ǣ,E)=>
Vector3D.Multiply(E,ǣ));ף<Vector3D,float>(ˡ.ˬ,(ǣ,E)=>Vector3D.Round(ǣ,(int)E));ף<Vector3D,Vector3D>(ˡ.ѭ,(ǣ,E)=>Vector3D.Reject(ǣ
,E));ע<Color>(ː.ˑ,ǣ=>new Color(255-ǣ.R,255-ǣ.G,255-ǣ.B));ף<Color,Color>(ˡ.Ѱ,(ǣ,E)=>ǣ+E);ף<Color,Color>(ˡ.ˢ,(ǣ,E)=>new
Color(ǣ.R-E.R,ǣ.G-E.G,ǣ.B-E.B));ף<Color,float>(ˡ.ѫ,(ǣ,E)=>Color.Multiply(ǣ,E));ף<float,Color>(ˡ.ѫ,(ǣ,E)=>Color.Multiply(E,ǣ))
;ף<Color,float>(ˡ.Ѭ,(ǣ,E)=>Color.Multiply(ǣ,1/E));}static Ǩ Ĥ(object ǣ,object E)=>Ƿ(ŷ(ǣ)).Ĥ(Ƿ(ŷ(E)));public delegate bool
ӱ(Object K,S ċ);public delegate bool Ρ(Ũ ǣ,Ũ E);public ӱ Κ(ӱ ǣ,ӱ E)=>new ӱ((K,ċ)=>ǣ(K,ċ)&&E(K,ċ));public ӱ Λ(ӱ ǣ,ӱ E)=>
new ӱ((K,ċ)=>ǣ(K,ċ)||E(K,ċ));public static ӱ Ο(Ų ť,Ρ ק,ʛ ר)=>(K,ċ)=>{Ũ ũ=ר.ū();Y Ǖ=Ħ.č(ċ);return ק(Ǖ.Ǜ(K,ť.τ(ר).χ(Ǖ,ũ.Ǯ)),ũ
);};public class Ũ:IComparable<Ũ>{public Ĭ Ǯ;public object ũ;public Ũ(Ĭ đ,object F){Ǯ=đ;ũ=F;}public Ũ ǋ(Ũ ŵ)=>ы(ˡ.Ѱ,this,
ŵ);public Ũ ɂ(Ũ ŵ)=>ы(ˡ.ˢ,this,ŵ);public Ũ ё(Ũ ŵ)=>ы(ˡ.Ѭ,this,ŵ);public int CompareTo(Ũ ŵ)=>Convert.ToInt32(Ž(ы(ˡ.б,this,
ŵ)));public Ũ Ǎ()=>ы(ː.ˑ,this);public Ũ Ԃ()=>ŷ((ũ as Ǩ)?.Ԃ()??ũ);}delegate Object д(Ũ ŵ);static KeyValuePair<ō,д>е<ō>(ō ש
,д ת)=>ź(ש,ת);static д װ(Ĭ Ǯ)=>ŵ=>{throw new Č($"Cannot convert {г[ŵ.Ǯ]} {Ż(ŵ)} to {г[Ǯ]}");};static Dictionary<Type,
Dictionary<Ĭ,д>>ؠ=Ź(ź(typeof(bool),Ź(е(Ĭ.ĭ,ŵ=>ŵ.ũ),е(Ĭ.ŋ,ŵ=>Ž(ŵ)!=0),е(Ĭ.Ľ,ŵ=>{Ũ Ʉ;return ʝ(Ż(ŵ),out Ʉ)&&ż(Ʉ);}),е(Ĭ.ǯ,װ(Ĭ.ĭ)))),ź
(typeof(float),Ź(е(Ĭ.ĭ,ŵ=>ż(ŵ)?1.0f:0.0f),е(Ĭ.ŋ,ŵ=>(float)ŵ.ũ),е(Ĭ.Ľ,ŵ=>float.Parse(Ż(ŵ))),е(Ĭ.ķ,ŵ=>(float)Ƕ(ŵ).Length())
,е(Ĭ.ǯ,װ(Ĭ.ŋ)))),ź(typeof(string),Ź(е(Ĭ.ŋ,ŵ=>Ž(ŵ).ToString(á.ԇ[ױ].ū().ũ+"")),е(Ĭ.ķ,ŵ=>Ŝ(Ƕ(ŵ))),е(Ĭ.ŝ,ŵ=>ײ(ž(ŵ))),е(Ĭ.ɑ,ŵ
=>Ƿ(ŵ).Ӿ()),е(Ĭ.ǯ,ŵ=>""+ŵ.ũ))),ź(typeof(Vector3D),Ź(е(Ĭ.Ľ,ŵ=>Ŗ(Ż(ŵ)).Value),е(Ĭ.ķ,ŵ=>ŵ.ũ),е(Ĭ.ŝ,ŵ=>ȡ(ž(ŵ).R,ž(ŵ).G,ž(ŵ).B)
),е(Ĭ.ǯ,װ(Ĭ.ķ)))),ź(typeof(Color),Ź(е(Ĭ.ŋ,ŵ=>new Color(Ž(ŵ))),е(Ĭ.Ľ,ŵ=>ү(Ż(ŵ)).Value),е(Ĭ.ķ,ŵ=>new Color((int)Ƕ(ŵ).X,(int
)Ƕ(ŵ).Y,(int)Ƕ(ŵ).Z)),е(Ĭ.ŝ,ŵ=>ŵ.ũ),е(Ĭ.ǯ,װ(Ĭ.ŝ)))),ź(typeof(Ǩ),Ź(е(Ĭ.ɑ,ŵ=>ŵ.ũ),е(Ĭ.ǯ,ŵ=>ƈ(ĥ(Ɖ(ŵ.ũ)))))));static
Dictionary<Type,Ĭ>ء=Ź(ź(typeof(bool),Ĭ.ĭ),ź(typeof(string),Ĭ.Ľ),ź(typeof(float),Ĭ.ŋ),ź(typeof(int),Ĭ.ŋ),ź(typeof(double),Ĭ.ŋ),ź(
typeof(Vector3D),Ĭ.ķ),ź(typeof(Color),Ĭ.ŝ),ź(typeof(Ǩ),Ĭ.ɑ));public static List<Ĭ>ס(Type Ǘ)=>Ǘ!=typeof(object)?Q(ء[Ǘ]):Q((Ĭ[])
Enum.GetValues(typeof(Ĭ)));public static Ũ ŷ(object ɯ)=>new Ũ(ء[ɯ.GetType()],(ɯ is double||ɯ is int)?Convert.ToSingle(ɯ):ɯ);
public static ō Ǐ<ō>(Ũ ŵ)=>(ō)ؠ[typeof(ō)].GetValueOrDefault(ŵ.Ǯ,ؠ[typeof(ō)][Ĭ.ǯ])(ŵ);public static bool ż(Ũ ŵ)=>Ǐ<bool>(ŵ);
public static float Ž(Ũ ŵ)=>Ǐ<float>(ŵ);public static string Ż(Ũ ŵ)=>Ǐ<string>(ŵ).Replace("\\n","\n");public static Vector3D Ƕ
(Ũ ŵ)=>Ǐ<Vector3D>(ŵ);public static Color ž(Ũ ŵ)=>Ǐ<Color>(ŵ);public static Ǩ Ƿ(Ũ ŵ)=>Ǐ<Ǩ>(ŵ);public static Color?ү(
String Ę)=>(Ę.StartsWith("#")&&Ę.Length==7)?new Color(آ(Ę.Substring(1,2)),آ(Ę.Substring(3,2)),آ(Ę.Substring(5,2))):(з.
ContainsKey(Ę.ToLower())?з[Ę.ToLower()]:(Color?)null);public static Vector3D?Ŗ(String Ę){var أ=Q<double>();foreach(string إ in Ę.
Split(':')){double ؤ;if(Double.TryParse(إ,out ؤ))أ.Add(ؤ);}return أ.Count()==3?ȡ(أ[0],أ[1],أ[2]):(Vector3D?)null;}static
string Ŝ(Vector3D ҭ)=>ҭ.X+":"+ҭ.Y+":"+ҭ.Z;static string ײ(Color Ұ)=>"#"+ئ(Ұ.R)+ئ(Ұ.G)+ئ(Ұ.B);static int آ(string ا)=>int.Parse
(ا.ToUpper(),System.Globalization.NumberStyles.AllowHexSpecifier);static string ئ(int ا)=>ا.ToString("X2");public class ų
{public String Ǣ,ب;public ʛ Ǝ;public bool Ǥ=false;public ų(string ť,string ҙ=null,ʛ ة=null){Ǣ=ť;ب=ҙ??ť;Ǝ=ة;}public ų ʻ(
bool ʻ){Ǥ=ʻ;return this;}public List<ų>χ(){var Ǳ=Q(this);if(Ǣ==C.ъ+""){Ǳ=Q<ų>();var ت=true;foreach(string Ę in Ż(Ǝ.ū()).
Split(' ')){var ϰ=á.Ь.GetValueOrDefault(Ę.ToLower(),Q<ʳ>());ʶ ť=ʗ<ʶ>(ϰ);if(ť!=null)Ǳ.Add(new ų(ť.ũ+"",Ę).ʻ(ť.Ǥ));else ت=false
;};if(!ت)Ǳ=Q(new ų(Ż(Ǝ.ū())));}return Ǳ;}}public class Ų{public List<ų>ƍ;public ʛ ǥ;public Ņ?ƴ;public bool?ǎ;public bool
Ǥ;public Ų(List<ų>Ǳ=null){ƍ=Ǳ??Q<ų>();}public Ų χ(Y Ǖ,Ĭ?ǲ=null){var ح=ث(ج(Ǖ,ǲ).ƍ);ح.ƍ=ح.ƍ.SelectMany(ŵ=>ŵ.χ()).ToList();
if(ح.ƍ.Any(ŵ=>ŵ.Ǥ)){ح=ح.τ(new ͳ(ː.ˑ,ǥ??Ɖ(true)));ح.Ǥ=true;}return ح;}Ų ج(Y خ,Ĭ?ǲ=null){if(ƍ.Count>0)return this;if(ƴ!=null
)return خ.ǘ(ƴ.Value);var Ǯ=ǥ?.ū().Ǯ??ǲ;return Ǯ!=null?خ.ǘ(Ǯ.Value):خ.ǘ(خ.Ǚ());}public Ų Π(Ņ?ƴ){Ų Ҹ=د();Ҹ.ƴ=ƴ;return Ҹ;}
public Ų ث(List<ų>Ƈ){Ų Ҹ=د();Ҹ.ƍ=Ƈ;return Ҹ;}public Ų τ(ʛ ǥ){Ų Ҹ=د();Ҹ.ǥ=ǥ;return Ҹ;}public Ų ӏ(bool ǎ){Ų Ҹ=د();Ҹ.ǎ=ǎ;return Ҹ
;}public Ų ρ(Ų ǒ){Ų Ҹ=د();Ҹ.ƍ.AddRange(ǒ.ƍ);Ҹ.ƴ=Ҹ.ƴ??ǒ.ƴ;return Ҹ;}Ų د()=>new Ų{ƍ=ƍ,ǥ=ǥ,ƴ=ƴ,ǎ=ǎ,Ǥ=Ǥ};public string ſ()=>
string.Join(" ",ƍ.Select(ŵ=>ŵ.ب));}public const float ȣ=(float)(Math.PI/30),Ȣ=(float)(30/Math.PI),Œ=(float)(180/Math.PI),œ=(
float)(Math.PI/180);public static ō ʆ<ō>()=>default(ō);public delegate ō Ә<ō>();public static List<ō>Q<ō>(params ō[]ذ)=>ذ.
ToList();public static Dictionary<ō,M>Ź<ō,M>(params KeyValuePair<ō,M>[]ذ)=>ذ.ToDictionary(φ=>φ.Key,φ=>φ.Value);public static
KeyValuePair<ō,M>ź<ō,M>(ō Ɵ,M ũ)=>new KeyValuePair<ō,M>(Ɵ,ũ);public static IEnumerable<int>Ȅ(int ر,int э)=>Enumerable.Range(ر,э);
public static IEnumerable<ō>ɢ<ō>()=>Enumerable.Empty<ō>();public static IEnumerable<ō>ĥ<ō>(ō ز)=>Enumerable.Repeat(ز,1);public
static IEnumerable<ō>Ĥ<ō>(params IEnumerable<ō>[]س)=>س.Aggregate((ǣ,E)=>ǣ.Concat(E));public static ʛ Ɖ(object ɯ)=>new ђ(ŷ(ɯ));
public static ʛ ʱ()=>Ɖ(ƈ());public static Vector3D ȡ(double ͺ,double ͻ,double ͽ)=>new Vector3D(ͺ,ͻ,ͽ);public static bool Υ(
params Object[]ش)=>ش.Any(ɯ=>ɯ!=null);
    public enum LogLevel { INFO, SCRIPT_ONLY }
    public enum S{Z,b,d,f,h,j,l,n,p,r,t,u,v,x,z,µ,º,Á,Ã,Å,Ç,É,Ê,Ë,Í,Ï,Ñ,Ô,Ö,Ù,Û,Ý,ß,ã,á,å,ç,ê,è,ë,ñ,ó,ì,í,ï,õ,ø,ú,þ,Ā,ü,
Ă,ă,ą,ć,ĉ}public enum C{ț,Ɗ,Ŕ,ľ,Ȱ,ĳ,ɠ,ƕ,ŝ,ħ,ư,ȟ,Ɩ,ƃ,Ə,Ɯ,š,ɡ,Ƅ,Ʋ,Ȭ,ɥ,I,H,ə,Ȟ,D,ȯ,Ţ,Ȫ,ȧ,Ɓ,Ƨ,Ɔ,ş,İ,ţ,Ŏ,ƭ,Ƃ,ɤ,ȩ,Ʈ,Ķ,Ʊ,Ŀ,Ȃ,Ĵ,ń
,Š,ɍ,Ȁ,ƌ,ǽ,ȍ,ȑ,ъ}public enum Ņ{ŉ,Ŋ,ň,ņ,Ɛ,Ƒ,Ȥ,ȥ,Ȝ}public enum Ѷ{ѷ,ѹ,ħ,ѻ}public enum Ĭ{ŋ,ĭ,Ľ,ķ,ŝ,ɑ,ǯ}public enum ˡ{Ѱ,ˢ,ѫ,Ѭ,
ѭ,Β,Δ,б,Ѩ,Ѫ,İ,ͱ,ь,Ѯ,ѯ,ˬ};public enum ː{ˑ,і,ї,ј,љ,њ,ћ,ќ,ѝ,ˬ,ѥ,Ѧ,Ѥ,ў,џ,Ѡ,ѡ,Ѣ,ͱ,ѧ};public interface ʛ{Ũ ū();}public class ђ:
ʛ{public Ũ Ʉ;public ђ(Ũ ص){Ʉ=ص;}public Ũ ū()=>Ʉ;}public class Ύ:ʛ{public ʛ ǣ,E;public Ρ ק;public Ύ(ʛ ɫ,ʛ ɬ,Ρ ض){ǣ=ɫ;E=ɬ;ק
=ض;}public Ũ ū()=>ŷ(ק(ǣ.ū(),E.ū()));}public class ι:ʛ{public ʛ Σ,ζ,θ;public Ũ ū()=>ż(Σ.ū())?ζ.ū():θ.ū();}public class Ά:ʛ
{public ʛ Έ,Ή,Ί;public Ũ ū(){if(Q(Έ,Ή,Ί).All(F=>F.ū().Ǯ==Ĭ.ŋ))return ŷ(ȡ(Ž(Έ.ū()),Ž(Ή.ū()),Ž(Ί.ū())));throw new Č(
"Invalid Variable in Vector");}}public class ͳ:ʛ{public ʛ ǣ;public ː Ғ;public ͳ(ː ط,ʛ F){Ғ=ط;ǣ=F;}public Ũ ū()=>ы(Ғ,ǣ.ū());}public class Α:ʛ{public
ʛ ǣ,E;public ˡ Ғ;public Α(ˡ ط,ʛ ɫ,ʛ ɬ){Ғ=ط;ǣ=ɫ;E=ɬ;}public Ũ ū()=>ы(Ғ,ǣ.ū(),E.ū());}public class ˋ:ʛ{public Ӳ ظ;public ʛ
ع,ר;public Ρ ק;public ˋ(Ӳ ˇ,ʛ ɺ,Ρ ض,ʛ ũ){ظ=ˇ;ع=ɺ;ק=ض;ר=ũ;}public Ũ ū(){var ɺ=Ƿ(ع.ū());return ŷ(ظ(ɺ.ɒ.Count,ɺ.ɒ.Count(F=>ק
(F.ū(),ר.ū()))));}}public class Ψ:ʛ{public Ӳ ظ;public ӱ غ;public ӫ ԕ;public Ψ(Ӳ ˇ,ӱ Σ,ӫ Ԗ){ظ=ˇ;غ=Σ;ԕ=Ԗ;}public Ũ ū(){var
N=ԕ.Ԙ();return ŷ(ظ(N.Count,N.Count(K=>غ(K,ԕ.ԗ()))));}}public class Χ:ʛ{public ӳ ػ;public ӫ ԕ;public Ų ť;public Χ(ӳ ؼ,ӫ Ԗ,
Ų ŵ){ػ=ؼ;ԕ=Ԗ;ť=ŵ;}public Ũ ū(){Y Ǖ=Ħ.č(ԕ.ԗ());if(ť.ƍ.Count==0)ť=ť.ث(Q(new ų(C.D+"")));Ų ŵ=ť.χ(Ǖ,Ĭ.Ľ);return ػ(ԕ.Ԙ(),E=>Ǖ.
Ǜ(E,ŵ));}}public class ʎ:ʛ{public String ũ;public ʎ(String F){ũ=F;}public Ũ ū(){try{return á.Ԇ(ũ).ū();}catch(Exception){
return ŷ(ũ);}}}public class ˈ:ʛ{public ʛ ع;public ӳ ػ;public ˈ(ʛ ɺ,ӳ ؼ){ع=ɺ;ػ=ؼ;}public Ũ ū()=>ػ(Ƿ(ع.ū()).ɒ,F=>((ʛ)F).ū());}
public class ʰ:ʛ{public ʛ ؽ;public ʰ(ʛ ʡ){ؽ=ʡ;}public Ũ ū(){Ǩ ɺ=Ƿ(ؽ.ū());if(ɺ.ɒ.Count==1){Ũ ؾ=ɺ.ū(ŷ(0)).ū();if(ؾ.Ǯ==Ĭ.ɑ)ɺ=Ƿ(ؾ)
;}return ŷ(ɺ);}}public class ʢ:ʛ{public ʛ ع,ʡ;public ʢ(ʛ ɺ,ʛ ǿ){ع=ɺ;ʡ=new ʰ(ǿ);}public Ũ ū(){var ɺ=Ƿ(ع.ū());var Ǳ=ؿ().
Select(ŵ=>ɺ.ū(ŵ)).ToList();if(Ǳ.Count==0)return ŷ(ɺ);return Ǳ.Count==1?Ǳ[0].ū():ŷ(ƈ(Ǳ));}public void Ŭ(ʛ ũ){var ɺ=Ƿ(ع.ū());var
ـ=ؿ();if(ـ.Count==0)ـ.AddRange(Ȅ(0,ɺ.ɒ.Count).Select(ǿ=>ŷ(ǿ)));ـ.ForEach(ʡ=>ɺ.Ŭ(ʡ,ũ));}List<Ũ>ؿ()=>Ƿ(ʡ.ū()).ɒ.Select(ǿ=>ǿ
.ū()).ToList();}public class ɐ:ʛ,IComparable<ɐ>,IEquatable<ɐ>{public ʛ ե,բ;public ɐ(ʛ Ɵ,ʛ ũ){ե=Ɵ;բ=ũ;}public bool ɓ()=>ե
!=null;public String ɔ()=>ե!=null?Ż(ե.ū()):null;public Ũ ū()=>բ.ū();public String Ӿ()=>(ɓ()?ف(ɔ())+"->":"")+ف(Ż(ū()));
public ɐ Ԃ()=>new ɐ(ե==null?null:Ɖ(ե.ū().Ԃ().ũ),Ɖ(բ.ū().Ԃ().ũ));String ف(String ũ)=>ũ.Contains(" ")?$"\"{ũ}\"":ũ;public bool
Equals(ɐ ʜ)=>ɔ()==ʜ.ɔ()&&ū().ũ.Equals(ʜ.ū().ũ);public int CompareTo(ɐ Ҵ)=>ū().CompareTo(Ҵ.ū());}public static ɐ Ֆ(ʛ ʜ)=>ʜ as ɐ
??new ɐ(null,ʜ);
    public UpdateFrequency updateFrequency = UpdateFrequency.Update1;
    public LogLevel logLevel = LogLevel.INFO;
    public int commandParseAmount = 1;
    public int commandParameterParseAmount = 100;
    public int maxAsyncThreads = 50;
    public int maxQueuedThreads = 50;
    public int maxItemTransfers = 10;
    public static string ױ="NUMBER_FORMAT";public static Program á;static void Ӿ(String ң){á?.Echo(ң);}static void І(
String ң){if(á?.logLevel!=LogLevel.SCRIPT_ONLY)Ӿ(ң);}public Ѷ Ѻ=Ѷ.ѹ;public Dictionary<string,Б>ʙ=Ź<string,Б>();public Ĕ Ė;
public Random פ=new Random();List<Ĕ>ğ=Q<Ĕ>();List<Ĕ>Ĝ=Q<Ĕ>();Dictionary<String,ʛ>ԇ=Ź<String,ʛ>();Dictionary<Ѷ,KeyValuePair<
String,bool>>ق=Ź(ź(Ѷ.ѷ,ź("Running",true)),ź(Ѷ.ѻ,ź("Paused",false)),ź(Ѷ.ѹ,ź("Stopped",false)),ź(Ѷ.ħ,ź("Complete",false)));
String Е;String Ї;List<String>Љ;public void Ћ(){ԑ(ك=>true,Ԓ=>{IGC.DisableBroadcastListener(Ԓ);while(Ԓ.HasPendingMessage)Ԓ.
AcceptMessage();});Ĝ.Clear();ğ.Clear();ԇ=Ź(ź("pi",Ɖ(Math.PI)),ź("e",Ɖ(Math.E)),ź("empty",ʱ()),ź("x",Ɖ(ȡ(1,0,0))),ź("y",Ɖ(ȡ(0,1,0))),ź
("z",Ɖ(ȡ(0,0,1))),ź("r",Ɖ(ȡ(1,0,0))),ź("g",Ɖ(ȡ(0,1,0))),ź("b",Ɖ(ȡ(0,0,1))),ź(ױ,Ɖ("#0.########")));}public void ԑ(Func<
IMyBroadcastListener,bool>Ȉ,Action<IMyBroadcastListener>К){var ل=Q<IMyBroadcastListener>();IGC.GetBroadcastListeners(ل,Ȉ);ل.ForEach(К);}
public Ĕ ӽ()=>Ė;public void Ӻ(Ĕ Į){ğ.Add(Į);if(ğ.Count>maxQueuedThreads)throw new Č(
$"Cannot have more than {maxQueuedThreads} queued commands");}public void Ӹ(Ĕ Į){Ĝ.Add(Į);if(Ĝ.Count>maxAsyncThreads)throw new Č(
$"Cannot have more than {maxAsyncThreads} concurrent async commands");}public void ԃ(String ϖ,ʛ ʜ){ԇ[ϖ]=ʜ;}public ʛ Ԇ(String ϖ){Ĕ Ė=ӽ();if(Ė.Ӽ.ContainsKey(ϖ)){return Ė.Ӽ[ϖ];}else if(ԇ.
ContainsKey(ϖ)){return ԇ[ϖ];}else{throw new Č("No Variable Exists for name: "+ϖ);}}public
 Program
(){try{á=this;Ҍ();Ͽ();צ();ր();Runtime.UpdateFrequency=updateFrequency;}finally{á=null;}}public void
 Main
(string م){try{á=this;if(!З()){ن(م);Ѻ=Ѷ.ѷ;return;}var ه=Q<MyIGCMessage>();ԑ(Ԓ=>Ԓ.HasPendingMessage,Ԓ=>ه.Add(Ԓ.
AcceptMessage()));ğ.InsertRange(0,ه.Select(Ϛ=>new Ĕ(У((String)Ϛ.Data),"Message",Ϛ.Tag)));ن(م);و();І(ق[Ѻ].Key);}catch(Exception φ){Ӿ(
$"{(φ is Ы?"Parsing Exception":φ is Č?"Runtime Exception":"Unknown Exception")} Occurred:");Ӿ(φ.Message);Ѻ=Ѷ.ѹ;}finally{Runtime.UpdateFrequency=ق[Ѻ].Value?updateFrequency:UpdateFrequency.None;á=null;}}void و(){
try{R.Clear();V.ɦ();U.ɦ();if(ğ.Count+Ĝ.Count==0){Б ى=ʙ[Е];ğ.Add(new Ĕ(ى.ʵ.Ѹ(),"Main",ى.А));}І("Queued Threads: "+ğ.Count);І
("Async Threads: "+Ĝ.Count);int ي=0;if(ğ.Count>0)ٮ(ğ,ref ي);ي=0;while(ي<Ĝ.Count)ٮ(Ĝ,ref ي);Ѻ=(ğ.Count+Ĝ.Count==0)?Ѷ.ħ:Ѷ.ѷ
;}catch(ѵ ٯ){Ѻ=ٯ.Ѷ;}}public void ٮ(List<Ĕ>ğ,ref int ʡ){Ė=ğ[ʡ];try{І(Ė.ǚ());if(Ė.Ĩ.ӷ())ğ.RemoveAt(ʡ);else ʡ++;}catch(Ī){ğ.
RemoveAt(ʡ);}}public void ن(String م){if(!String.IsNullOrEmpty(م))ğ.Insert(0,new Ĕ(У(م),"Request",م));}public class Ĕ{public Ĩ Ĩ
{get;set;}public Dictionary<String,ʛ>Ӽ=Ź<string,ʛ>();public string Ē,ē,ٱ;public Ĕ Ģ,ĕ;public ō ѿ<ō>(Func<Ĩ,bool>Ȉ=null)
where ō:class=>Ĩ.Ӷ(ϟ=>ϟ is ō&&(Ȉ==null||Ȉ(ϟ)))as ō;public Ĕ(Ĩ ϟ,string ٲ,string L,string ٳ=null,Ĕ ٴ=null){Ĩ=ϟ;ٱ=ٲ;ē=L;Ē=ٳ;ĕ=ٴ
??this;}public String ǚ()=>$"[{ٱ}] {Ē??ē}";public void ӻ(String Ę)=>ē=Ę;public Ĕ ĝ(string Ē)=>new Ĕ(Ĩ,ٱ,ē,Ē,this);}public
class Б{public String А;public Ϡ ʵ=null;public List<String>Ԁ;public Б(string ʵ,List<string>Ѐ){А=ʵ;Ԁ=Ѐ;}}
