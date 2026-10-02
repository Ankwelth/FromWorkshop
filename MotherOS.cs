// |  Mother OS - v1.1.0 - 12 May 2026
// |  Agentluke
// |
// |  Docs
// |  https://lukejamesmorrison.github.io/mother-docs/
// |
// |  Discord
// |  https://discord.com/invite/PrrmBujmXQ
// 
A B;public
 Program(){B=new A(this){C="Mother OS",};B.D(new List<E>{new F(B),new G(B),new H(B),new I(B),new J(B)
,new K(B),new L(B),new M(B),new N(B),new O(B),new P(B),new Q(B),new R(B),new S(B),new T(B),new U(B),new V(B),new W(B),new
X(B),});}public void
 Save
()=>Storage=B.Y();public void
 Main
(string Z,UpdateType a)=>B.b(Z,a);
}
public class Q:c{d d;public Q(A B):base(B){}public override void o(){d=A.e<d>();f(new g(this));f(new h(this));f(new i(
this));j<IMyAirVent>(k=>k.Status,(l,m)=>n(l as IMyAirVent,m));}void n(IMyAirVent p,object q){var r=q as VentStatus?;if(r.
HasValue){if(r==VentStatus.Depressurized){t<s>(p);d.u(p,"onDepressurized");}else if(r==VentStatus.Depressurizing){t<v>(p);d.u(p,
"onDepressurizing");}else if(r==VentStatus.Pressurizing){t<w>(p);d.u(p,"onPressurizing");}else if(r==VentStatus.Pressurized){t<x>(p);d.u(p
,"onPressurized");}}}public void z(IMyAirVent y){y.Depressurize=false;}public void ª(IMyAirVent y){y.Depressurize=true;}
public void µ(IMyAirVent y){y.Depressurize=!y.Depressurize;}}public class h:º{Q À;public override string Á=>
"vent/depressurize";public h(Q Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string É=Ä.Å[0];List<IMyAirVent>Ë=À.
Ê<IMyAirVent>(É);if(Ë.Count==0)return Ì.Í(Î.Ï,É);Ë.ForEach(y=>À.ª(y));return Ì.Í(Î.Ð,É);}}}public class g:º{Q À;
public override string Á=>"vent/pressurize";public g(Q Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else
{string É=Ä.Å[0];List<IMyAirVent>Ë=À.Ê<IMyAirVent>(É);if(Ë.Count==0)return Ì.Í(Î.Ï,É);Ë.ForEach(y=>À.z(y));return Ì.Í(Î.Ò
,É);}}}public class i:º{Q À;public override string Á=>"vent/toggle";public i(Q Â){À=Â;}public override string Ñ(
Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string É=Ä.Å[0];List<IMyAirVent>Ë=À.Ê<IMyAirVent>(É);if(Ë.Count==0)return Ì.Í(Î.Ï,
É);Ë.ForEach(y=>À.µ(y));return Ì.Í(Î.Ó,É);}}}public class s:Ô{}public class v:Ô{}public class x:Ô{}public class w:Ô{}
public class M:c{public M(A B):base(B){}public override void o(){f(new Õ(this));f(new Ö(this));f(new Ø(this));f(new Ù(this));}
public void Û(IMyBatteryBlock Ú){Ú.ChargeMode=ChargeMode.Recharge;}public void Ü(IMyBatteryBlock Ú){Ú.ChargeMode=ChargeMode.
Discharge;}public void Ý(IMyBatteryBlock Ú){Ú.ChargeMode=ChargeMode.Auto;}public void Þ(IMyBatteryBlock Ú){switch(Ú.ChargeMode){
case ChargeMode.Auto:Ú.ChargeMode=ChargeMode.Recharge;break;case ChargeMode.Recharge:Ú.ChargeMode=ChargeMode.Discharge;break
;case ChargeMode.Discharge:Ú.ChargeMode=ChargeMode.Auto;break;}}}public class Ø:º{M À;public override string Á=>
"battery/auto";public Ø(M Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string ß=Ä.Å[0];List<IMyBatteryBlock
>à=À.Ê<IMyBatteryBlock>(ß);if(à.Count==0)return Ì.Í(Î.Ï,ß);à.ForEach(Ú=>À.Ý(Ú));return Ì.Í(Î.á,ß);}}}public class Õ:º{
M À;public override string Á=>"battery/charge";public Õ(M Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return
Æ.Ç.È;else{string ß=Ä.Å[0];List<IMyBatteryBlock>à=À.Ê<IMyBatteryBlock>(ß);if(à.Count==0)return Ì.Í(Î.Ï,ß);à.ForEach(Ú=>À.
Û(Ú));return Ì.Í(Î.â,ß);}}}public class Ö:º{M À;public override string Á=>"battery/discharge";public Ö(M Â){À=Â;
}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string ß=Ä.Å[0];List<IMyBatteryBlock>à=À.Ê<
IMyBatteryBlock>(ß);if(à.Count==0)return Ì.Í(Î.Ï,ß);à.ForEach(Ú=>À.Ü(Ú));return Ì.Í(Î.ã,ß);}}}public class Ù:º{M À;public
override string Á=>"battery/toggle";public Ù(M Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string ß=
Ä.Å[0];List<IMyBatteryBlock>à=À.Ê<IMyBatteryBlock>(ß);if(à.Count==0)return Ì.Í(Î.Ï,ß);à.ForEach(Ú=>À.Þ(Ú));return Ì.Í(Î.Ó
,ß);}}}public class U:c{d d;public IMyCockpit ä;public U(A B):base(B){}public override void o(){d=A.e<d>();f(new å(this))
;f(new æ(this));f(new ç(this));f(new è(this));j<IMyCockpit>(é=>é.IsUnderControl,(l,m)=>ê(l as IMyCockpit,m));ë();}void ë(
){var í=d.ì<IMyCockpit>();ä=í.FirstOrDefault(î=>î.IsMainCockpit)??í.FirstOrDefault();}void ê(IMyCockpit é,object q){var r
=q as bool?;if(r.Value){t<ï>(é);d.u(é,"onOccupied");}else{t<ð>(é);d.u(é,"onEmpty");}}public void ò(IMyCockpit é){ñ(é,!é.
HandBrake);}public void ñ(bool ó=true){if(ä==null)return;ä.HandBrake=ó;}public void ñ(IMyCockpit é,bool ó=true){é.HandBrake=ó;}
public void ô(bool ó=true){if(ä==null)return;ä.DampenersOverride=ó;}public void ô(IMyCockpit é,bool ó=true){é.
DampenersOverride=ó;}}public class æ:º{U À;public override string Á=>"dampeners/off";public æ(U Â){À=Â;}public override string Ñ
(Ã Ä){if(Ä.Å.Count==0){À.ä.DampenersOverride=false;return"Dampeners OFF";}else{string õ=Ä.Å[0];List<IMyCockpit>í=À.Ê<
IMyCockpit>(õ);if(í.Count==0)return Ì.Í(Î.Ï,õ);í.ForEach(é=>é.DampenersOverride=false);return$"Dampeners OFF for {õ}";}}}public
class å:º{U À;public override string Á=>"dampeners/on";public å(U Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count
==0){À.ä.DampenersOverride=true;return"Dampeners ON";}else{string õ=Ä.Å[0];List<IMyCockpit>í=À.Ê<IMyCockpit>(õ);if(í.Count
==0)return Ì.Í(Î.Ï,õ);í.ForEach(é=>é.DampenersOverride=true);return$"Dampeners ON for {õ}";}}}public class è:º{U
À;public override string Á=>"handbrake/off";public è(U Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0){À.ñ(false);
return"Handbrakes OFF";}else{string õ=Ä.Å[0];List<IMyCockpit>í=À.Ê<IMyCockpit>(õ);if(í.Count==0)return Ì.Í(Î.Ï,õ);í.ForEach(é
=>À.ñ(é,false));return$"Handbrakes OFF for {õ}";}}}public class ç:º{U À;public override string Á=>"handbrake/on";
public ç(U Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0){À.ñ();return"Handbrakes ON";}else{string õ=Ä.Å[0];List<
IMyCockpit>í=À.Ê<IMyCockpit>(õ);if(í.Count==0)return Ì.Í(Î.Ï,õ);í.ForEach(é=>À.ñ(é));return$"Handbrakes ON for {õ}";}}}public
class ð:Ô{}public class ï:Ô{}public class ö:º{J À;public override string Á=>"door/close";public ö(J Â){À=Â;}public
override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string ø=Ä.Å[0];List<IMyDoor>ù=À.Ê<IMyDoor>(ø);if(ù.Count==0)return Ì.Í
(Î.Ï,ø);ù.ForEach(ú=>À.û(ú));return Ì.Í(Î.Ð,ø);}}}public class ü:º{J À;public override string Á=>"door/open";
public ü(J Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string ø=Ä.Å[0];List<IMyDoor>ù=À.Ê<IMyDoor>
(ø);if(ù.Count==0)return Ì.Í(Î.Ï,ø);ù.ForEach(ú=>À.ý(ú));return Ì.Í(Î.Ò,ø);}}}public class þ:º{J À;public
override string Á=>"door/toggle";public þ(J Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string ø=Ä.Å
[0];List<IMyDoor>ù=À.Ê<IMyDoor>(ø);if(ù.Count==0)return Ì.Í(Î.Ï,ø);ù.ForEach(ú=>À.ÿ(ú));return Ì.Í(Î.Ó,ø);}}}public class
J:c{d d;public J(A B):base(B){}public override void o(){d=A.e<d>();f(new ü(this));f(new ö(this));f(new þ(this));j<IMyDoor
>(ú=>ú.Status,(l,m)=>Ā(l as IMyDoor,(DoorStatus)m));}void Ā(IMyDoor ú,DoorStatus r){if(r==DoorStatus.Open){t<ā>(ú);d.u(ú,
"onOpen");}else if(r==DoorStatus.Opening){t<Ă>(ú);d.u(ú,"onOpening");}else if(r==DoorStatus.Closed){t<ă>(ú);d.u(ú,"onClose");}
else if(r==DoorStatus.Closing){t<Ą>(ú);d.u(ú,"onClosing");}}public void ý(IMyDoor ú){ú.OpenDoor();}public void û(IMyDoor ú){
ú.CloseDoor();}public void ÿ(IMyDoor ú){if(ú.Status==DoorStatus.Open)û(ú);else ý(ú);}}public class ă:Ô{}public class Ą:Ô{
}public class ā:Ô{}public class Ă:Ô{}public class ą:º{I À;public override string Á=>"hinge/attach";public ą(I Â)
{À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string Ć=Ä.Å[0];List<IMyMotorStator>ć=À.Ê<IMyMotorStator
>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.Ĉ(l));return Ì.Í("Hinge attached: {0}",Ć);}}public class ĉ:º{I À;public override string Á=>"hinge/detach";public ĉ(I Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.
È;string Ć=Ä.Å[0];List<IMyMotorStator>ć=À.Ê<IMyMotorStator>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.Ċ(l));
return Ì.Í("Hinge detached: {0}",Ć);}}public class ċ:º{I À;public override string Á=>"hinge/lock";public ċ(I Â){À=Â;}
public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Č=Ä.Å[0];List<IMyMotorStator>č=À.Ê<IMyMotorStator>(Č);
if(č.Count==0)return Ì.Í(Î.Ï,Č);č.ForEach(Ď=>À.ď(Ď));return Ì.Í(Î.Đ,Č);}}}public class đ:º{I À;public override
string Á=>"hinge/reset";public đ(I Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Č=Ä.Å[0];
List<IMyMotorStator>č=À.Ê<IMyMotorStator>(Č);if(č.Count==0)return Ì.Í(Î.Ï,Č);č.ForEach(Ď=>À.Ē(Ď));return Ì.Í(Î.ē,Č);}}}
public class Ĕ:º{I À;public override string Á=>"hinge/rotate";public Ĕ(I Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å
.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Č=Ä.Å[0];string ĕ=Ä.Å[1];string ė=Ä.Ė("speed");float ę=!string.
IsNullOrEmpty(ė)?float.Parse(ė):I.Ę;bool Ĝ=Ě(Ä.ě);List<IMyMotorStator>č=À.Ê<IMyMotorStator>(Č);if(č.Count==0)return Ì.Í(Î.Ï,Č);float
Ğ=ĝ(ĕ,Ä.ě);float ğ=float.Parse(ĕ);float ġ=Ġ(ğ,č.Count,Ĝ);float Ģ=Ġ(Ğ,č.Count,Ĝ);č.ForEach(Ď=>{float ģ=Ğ==0?ġ:MathHelper.
ToDegrees(Ď.Angle)+Ģ;À.Ĥ(Ď,ģ,ę);});string ĥ=Ĝ?"shared ":"";return Ì.Í(Î.Ħ,Č,$"{ĥ}angle={ĕ}°");}return Æ.Ç.ħ;}}public class Ĩ:º{
I À;public override string Á=>"hinge/speed";public Ĩ(I Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count<2)return Æ.Ç.
È;string Č=Ä.Å[0];string ė=Ä.Å[1];List<IMyMotorStator>č=À.Ê<IMyMotorStator>(Č);if(č.Count==0)return Ì.Í(Î.Ï,Č);try{float
Ğ=ĝ(ė,Ä.ě);float ĩ=float.Parse(ė);bool Ī=Ä.Ė("free")=="true";bool Ĝ=Ě(Ä.ě);float ī=Ġ(ĩ,č.Count,Ĝ);float Ģ=Ġ(Ğ,č.Count,Ĝ);
foreach(var Ď in č){float Ĭ=Ğ==0?ī:Ď.TargetVelocityRPM+Ģ;À.ĭ(Ď,Ĭ,Ī);}string ĥ=Ĝ?"shared ":"";return Ì.Í(Î.Į,Č,$"{ĥ}speed={ė}");
}catch(ArgumentException){return Æ.Ç.ħ;}}}public class į:º{I À;public override string Á=>"hinge/unlock";public į
(I Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Č=Ä.Å[0];List<IMyMotorStator>č=À.Ê<
IMyMotorStator>(Č);if(č.Count==0)return Ì.Í(Î.Ï,Č);č.ForEach(Ď=>À.İ(Ď));return Ì.Í(Î.ı,Č);}}}public class I:Ĳ{public const float Ę=2f;
const float ĳ=0.12f;public I(A B):base(B){}public override void o(){base.o();f(new Ĵ(this,"hinge/ulimit",true));f(new Ĵ(this,
"hinge/llimit",false));f(new ċ(this));f(new į(this));f(new Ĕ(this));f(new đ(this));f(new Ĩ(this));f(new ą(this));f(new ĉ(this));}
public void Ē(IMyMotorStator Ď){Ĥ(Ď,0);}public void Ĥ(IMyMotorStator Ď,float ğ,float ę=Ę){ę=Math.Abs(ę);float Ķ=ĵ(Ď);ğ=
MathHelper.Clamp(ğ,-90,90);float ķ=ğ-Ķ;Ď.TargetVelocityRPM=ķ>0?ę:-ę;if(ķ>0){Ď.LowerLimitDeg=Ķ;Ď.UpperLimitDeg=ğ;}else{Ď.
LowerLimitDeg=ğ;Ď.UpperLimitDeg=Ķ;}Ď.RotorLock=false;d.u(Ď,"onMoving");A.e<ĸ>().Ĺ(Ď,l=>ĺ(l as IMyMotorStator,ğ),l=>Ļ(l as
IMyMotorStator,true));}float ĵ(IMyMotorStator Ď){float ļ=MathHelper.ToDegrees(Ď.Angle);if(ļ>180)ļ-=360;return ļ;}bool ĺ(IMyMotorStator
Ď,float ğ){float Ķ=ĵ(Ď);float ķ=Math.Abs(Ķ-ğ);return ķ<ĳ;}void Ļ(IMyMotorStator Ď,bool Ľ=true){ď(Ď,Ľ);d.u(Ď,"onStop");}}
public class ľ:º{N À;public override string Á=>"gear/lock";public ľ(N Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.
Count==0)return Æ.Ç.È;else{string Ŀ=Ä.Å[0];List<IMyLandingGear>ŀ=À.Ê<IMyLandingGear>(Ŀ);if(ŀ.Count==0)return Ì.Í(Î.Ï,Ŀ);ŀ.
ForEach(Ł=>À.ł(Ł));return Ì.Í(Î.Đ,Ŀ);}}}public class Ń:º{N À;public override string Á=>"gear/auto";public Ń(N Â){À=Â;}
public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ŀ=Ä.Å[0];List<IMyLandingGear>ŀ=À.Ê<IMyLandingGear>(Ŀ);
if(ŀ.Count==0)return Ì.Í(Î.Ï,Ŀ);if(Ä.Å.Count>=2){bool m=false;if(bool.TryParse(Ä.Å[1],out m)){ŀ.ForEach(Ł=>À.ń(Ł,m));
return Ì.Í(Î.Į,Ŀ,$"Autolock: {m}");}}}return Æ.Ç.ħ;}}public class Ņ:º{N À;public override string Á=>"gear/toggle";
public Ņ(N Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ŀ=Ä.Å[0];List<IMyLandingGear>ŀ=À.Ê<
IMyLandingGear>(Ŀ);if(ŀ.Count==0)return Ì.Í(Î.Ï,Ŀ);ŀ.ForEach(Ł=>À.ņ(Ł));return Ì.Í(Î.Ó,Ŀ);}}}public class Ň:º{N À;public
override string Á=>"gear/unlock";public Ň(N Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ŀ=Ä.Å
[0];List<IMyLandingGear>ŀ=À.Ê<IMyLandingGear>(Ŀ);if(ŀ.Count==0)return Ì.Í(Î.Ï,Ŀ);ŀ.ForEach(Ł=>À.ň(Ł));return Ì.Í(Î.ı,Ŀ);}
}}public class ŉ:Ô{}public class Ŋ:Ô{}public class ŋ:Ô{}public class N:c{d d;public N(A B):base(B){}public override void
o(){d=A.e<d>();f(new ľ(this));f(new Ň(this));f(new Ņ(this));f(new Ń(this));j<IMyLandingGear>(Ł=>Ł.LockMode,(l,m)=>Ō(l as
IMyLandingGear,(LandingGearMode)m));}void Ō(IMyLandingGear Ł,object q){if(Ł!=null){var r=q as LandingGearMode?;if(r.Value==
LandingGearMode.Locked){t<ŉ>(Ł);d.u(Ł,"onLock");}else if(r.Value==LandingGearMode.Unlocked){t<ŋ>(Ł);d.u(Ł,"onUnlock");}else if(r.Value
==LandingGearMode.ReadyToLock){t<Ŋ>(Ł);d.u(Ł,"onReady");}}}public void ł(IMyLandingGear Ł){Ł.Lock();}public void ň(
IMyLandingGear Ł){Ł.Unlock();}public void ņ(IMyLandingGear Ł){Ł.ToggleLock();}public void ń(IMyLandingGear Ł,bool m){Ł.AutoLock=m;}}
public class ō:º{K À;public override string Á=>"light/reset";public ō(K Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.
Count==0)return Æ.Ç.È;string Ŏ=Ä.Å[0];List<IMyLightingBlock>ŏ=À.Ê<IMyLightingBlock>(Ŏ);List<IMySearchlight>Ő=À.Ê<
IMySearchlight>(Ŏ);if(ŏ.Count==0)return Ì.Í(Î.Ï,Ŏ);ŏ.ForEach(ő=>À.Œ(ő));Ő.ForEach(ő=>À.Œ(ő));return Ì.Í(Î.ē,Ŏ);}}public class œ:º{
K À;public override string Á=>"light/blink";public œ(K Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç
.È;else if(Ä.Å.Count>=2){string Ŏ=Ä.Å[0];string Ŕ=Ä.Å[1];float ŕ;float ŗ=K.Ŗ;float ř=K.Ř;if(À.Ś.ContainsKey(Ŕ)){float[]ś=
Array.ConvertAll(À.Ś[Ŕ].Split(','),float.Parse);ŕ=ś[0];ŗ=ś[1];ř=ś[2];}else{ŕ=float.Parse(Ä.Å[1]);if(Ä.ě.ContainsKey("length")
)ŗ=float.Parse(Ä.ě["length"])*100;if(Ä.ě.ContainsKey("offset"))ř=float.Parse(Ä.ě["offset"])*100;}string Ŝ=
$"bi={ŕ}s, bl={ŗ}%, bo={ř}%";List<IMyLightingBlock>ŏ=À.Ê<IMyLightingBlock>(Ŏ);List<IMySearchlight>Ő=À.Ê<IMySearchlight>(Ŏ);if((ŏ.Count+Ő.Count)==0)
return Ì.Í(Î.Ï,Ŏ);ŏ.ForEach(ő=>{À.ŝ(ő,ŕ);À.Ş(ő,ŗ);À.ş(ő,ř);});Ő.ForEach(Š=>{À.ŝ(Š,ŕ);À.Ş(Š,ŗ);À.ş(Š,ř);});return Ì.Í(Î.Į,Ŏ,Ŝ);
}return Æ.Ç.ħ;}}public class š:º{K À;public override string Á=>"light/color";public š(K Â){À=Â;}public override
string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string Ŏ=Ä.Å[0];string Ţ=Ä.Å[1];List<IMyLightingBlock>ŏ=À.Ê<IMyLightingBlock>(Ŏ);
List<IMySearchlight>Ő=À.Ê<IMySearchlight>(Ŏ);if((ŏ.Count+Ő.Count)==0)return Ì.Í(Î.Ï,Ŏ);Color ť=ţ.Ť(Ţ);if(ť!=null){ŏ.ForEach(
ő=>À.Ŧ(ő,ť));Ő.ForEach(ő=>À.Ŧ(ő,ť));return Ì.Í(Î.Į,Ŏ,$"color={ť}");}return"Invalid color format.";}}public class ŧ:º{
K À;public override string Á=>"light/intensity";public ŧ(K Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return
Æ.Ç.È;else if(Ä.Å.Count>=2){string Ŏ=Ä.Å[0];string Ũ=Ä.Å[1];List<IMyLightingBlock>ŏ=À.Ê<IMyLightingBlock>(Ŏ);List<
IMySearchlight>Ő=À.Ê<IMySearchlight>(Ŏ);if((ŏ.Count+Ő.Count)==0)return Ì.Í(Î.Ï,Ŏ);float Ğ=ĝ(Ũ,Ä.ě);ŏ.ForEach(ő=>{float ũ=Ğ==0?float.
Parse(Ũ):(ő.Intensity/10)+Ğ;À.Ū(ő,ũ);});Ő.ForEach(ő=>{float ũ=Ğ==0?float.Parse(Ũ):(ő.Intensity/10)+Ğ;À.Ū(ő,ũ);});return Ì.Í(Î
.Į,Ŏ,$"brightness={Ũ}");}return"Invalid brightness format.";}}public class K:c{public Dictionary<string,string>Ś
=new Dictionary<string,string>(){{"off","0,0,0"},{"slow","3.0,30,0"},{"med","1.0,50,0"},{"fast","0.25,50,0"}};public
const float ū=0;public const float Ŗ=50;public const float Ř=0;public K(A B):base(B){}public override void o(){f(new ō(this))
;f(new š(this));f(new ŧ(this));f(new œ(this));}public void Ŧ(IMyLightingBlock ő,Color ť){ő.Color=ť;}public void Ŧ(
IMySearchlight ő,Color ť){ő.Color=ť;}public void Ū(IMyLightingBlock ő,float Ŭ){ő.Intensity=10*MathHelper.Clamp(Ŭ,0,1);}public void Ū(
IMySearchlight ő,float Ŭ){ő.Intensity=10*MathHelper.Clamp(Ŭ,0,1);}public void Œ(IMyLightingBlock ő){Ŧ(ő,Color.White);ŝ(ő,0);Ş(ő,0);ş(ő
,0);}public void Œ(IMySearchlight ő){Ŧ(ő,Color.White);ŝ(ő,0);Ş(ő,0);ş(ő,0);}public void ŝ(IMyLightingBlock ő,float ŭ){ő.
BlinkIntervalSeconds=ŭ;}public void ŝ(IMySearchlight ő,float ŭ){ő.BlinkInterval=ŭ;}public void Ş(IMyLightingBlock ő,float Ů){ő.BlinkLength=Ů
;}public void Ş(IMySearchlight ő,float Ů){ő.BlinkLength=Ů;}public void ş(IMyLightingBlock ő,float ů){ő.BlinkOffset=ů;}
public void ş(IMySearchlight ő,float ů){ő.BlinkOffset=ů;}}public class Ĵ:º{Ĳ À;string Ű;bool ű;
public override string Á=>Ű;public Ĵ(Ĳ Â,string Ų,bool ų){À=Â;Ű=Ų;ű=ų;}public override string Ñ(Ã Ä){if(Ä.Å.Count<2)return Æ.Ç
.È;string Ć=Ä.Å[0];string Ŵ=Ä.Å[1];List<IMyMotorStator>ŵ=À.Ê<IMyMotorStator>(Ć);if(ŵ.Count==0)return Ì.Í(Î.Ï,Ć);float Ŷ;
if(!float.TryParse(Ŵ,out Ŷ))return Æ.Ç.ħ;ŵ.ForEach(ŷ=>{if(ű)À.Ÿ(ŷ,Ŷ);else À.Ź(ŷ,Ŷ);});var ź=ű?"upperLimit":"lowerLimit";
return Ì.Í(Î.Į,Ć,$"{ź}={Ŵ}°");}}public class ż:º{Ż À;string Ű;bool ű;public override string Á=>Ű;
public ż(Ż Â,string Ų,bool ų){À=Â;Ű=Ų;ű=ų;}public override string Ñ(Ã Ä){if(Ä.Å.Count<2)return Æ.Ç.È;string Ć=Ä.Å[0];string Ŵ=
Ä.Å[1];List<IMyPistonBase>Ž=À.Ê<IMyPistonBase>(Ć);if(Ž.Count==0)return Ì.Í(Î.Ï,Ć);float Ŷ;if(!float.TryParse(Ŵ,out Ŷ))
return Æ.Ç.ħ;Ž.ForEach(ž=>{if(ű)À.Ÿ(ž,Ŷ);else À.Ź(ž,Ŷ);});string ź=ű?"upperLimit":"lowerLimit";return Ì.Í(Î.Į,Ć,$"{ź}={Ŵ}m");}
}public abstract class Ż:c{protected d d;protected Ż(A B):base(B){}public override void o(){d=A.e<d>();}public void Ĉ(
IMyMechanicalConnectionBlock ſ){ſ.Attach();}public void Ċ(IMyMechanicalConnectionBlock ſ){ſ.Detach();}public void Ÿ(IMyPistonBase ž,float ƀ){ž.
MaxLimit=ƀ;if(ž.MinLimit>ƀ)ž.MinLimit=ƀ;}public void Ź(IMyPistonBase ž,float ƀ){ž.MinLimit=ƀ;if(ž.MaxLimit<ƀ)ž.MaxLimit=ƀ;}}
public abstract class Ĳ:Ż{protected Ĳ(A B):base(B){}public void ď(IMyMotorStator ŷ,bool Ľ=true){ŷ.RotorLock=true;if(Ľ){ŷ.
TargetVelocityRPM=0;ŷ.UpperLimitDeg=0;ŷ.LowerLimitDeg=0;}}public void İ(IMyMotorStator ŷ){ŷ.RotorLock=false;}public void ĭ(IMyMotorStator
ŷ,float ę,bool Ī=false){ŷ.TargetVelocityRPM=ę;if(Ī){ŷ.RotorLock=false;ŷ.UpperLimitDeg=float.MaxValue;ŷ.LowerLimitDeg=
float.MinValue;}}public void Ÿ(IMyMotorStator ŷ,float ļ){ŷ.UpperLimitDeg=ļ;if(ŷ.LowerLimitDeg>ļ)ŷ.LowerLimitDeg=ļ;}public
void Ź(IMyMotorStator ŷ,float ļ){ŷ.LowerLimitDeg=ļ;if(ŷ.UpperLimitDeg<ļ)ŷ.UpperLimitDeg=ļ;}}public class Ɓ:º{H À;
public override string Á=>"piston/attach";public Ɓ(H Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string
Ć=Ä.Å[0];List<IMyPistonBase>ć=À.Ê<IMyPistonBase>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.Ĉ(l));return Ì.Í(
"Piston attached: {0}",Ć);}}public class Ƃ:º{H À;public override string Á=>"piston/detach";public Ƃ(H Â){À=Â;}public override string
Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string Ć=Ä.Å[0];List<IMyPistonBase>ć=À.Ê<IMyPistonBase>(Ć);if(ć.Count==0)return Ì.Í(Î
.Ï,Ć);ć.ForEach(l=>À.Ċ(l));return Ì.Í("Piston detached: {0}",Ć);}}public class ƃ:º{H À;public override string Á
=>"piston/reset";public ƃ(H Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string Ƅ=Ä.Å[0];List<
IMyPistonBase>Ž=À.Ê<IMyPistonBase>(Ƅ);if(Ž.Count==0)return Ì.Í(Î.Ï,Ƅ);Ž.ForEach(ž=>À.ƅ(ž));return Ì.Í(Î.ē,Ƅ);}}public class Ɔ:º{
H À;public override string Á=>"piston/distance";public Ɔ(H Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return
Æ.Ç.È;else if(Ä.Å.Count>=2){string Ƅ=Ä.Å[0];string Ƈ=Ä.Å[1];string ė=Ä.Ė("speed");float ę=!string.IsNullOrEmpty(ė)?float.
Parse(ė):H.Ę;bool Ĝ=Ě(Ä.ě);List<IMyPistonBase>Ž=À.Ê<IMyPistonBase>(Ƅ);if(Ž.Count==0)return Ì.Í(Î.Ï,Ƅ);float Ğ=ĝ(Ƈ,Ä.ě);float
ƈ=float.Parse(Ƈ);float Ɖ=Ġ(ƈ,Ž.Count,Ĝ);float Ɗ=Ġ(Ğ,Ž.Count,Ĝ);Ž.ForEach(ž=>{float Ƌ=Ğ==0?Ɖ:ž.CurrentPosition+Ɗ;À.ƌ(ž,Ƌ,ę
);});string ĥ=Ĝ?"shared ":"";return Ì.Í(Î.Į,Ƅ,$"{ĥ}distance={Ƈ}m");}return Æ.Ç.ħ;}}public class ƍ:º{H À;public
override string Á=>"piston/speed";public ƍ(H Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count<2)return Æ.Ç.È;string Ƅ=Ä.Å[0];
string ė=Ä.Å[1];bool Ĝ=Ě(Ä.ě);List<IMyPistonBase>Ž=À.Ê<IMyPistonBase>(Ƅ);if(Ž.Count==0)return Ì.Í(Î.Ï,Ƅ);float Ğ=ĝ(ė,Ä.ě);
float ĩ=float.Parse(ė);float Ǝ=Ġ(ĩ,Ž.Count,Ĝ);float Ɗ=Ġ(Ğ,Ž.Count,Ĝ);Ž.ForEach(ž=>{float Ĭ=Ğ==0?Ǝ:ž.Velocity+Ɗ;À.Ə(ž,Ĭ);});
string ĥ=Ĝ?"shared ":"";return Ì.Í(Î.Į,Ƅ,$"{ĥ}speed={ė}");}}public class Ɛ:º{H À;public override string Á=>
"piston/stop";public Ɛ(H Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string Ƅ=Ä.Å[0];List<IMyPistonBase>Ž=À.Ê<
IMyPistonBase>(Ƅ);if(Ž.Count==0)return Ì.Í(Î.Ï,Ƅ);Ž.ForEach(ž=>À.Ƒ(ž));return Ì.Í(Î.ƒ,Ƅ);}}public class Ɠ:Ô{}public class Ɣ:Ô{}public
class ƕ:Ô{}public class Ɩ:Ô{}public class H:Ż{public const float Ę=0.5f;public H(A B):base(B){}public override void o(){base.
o();f(new ƃ(this));f(new Ɔ(this));f(new Ɛ(this));f(new ƍ(this));f(new Ɓ(this));f(new Ƃ(this));f(new ż(this,
"piston/ulimit",true));f(new ż(this,"piston/llimit",false));j<IMyPistonBase>(ž=>ž.Status,(l,m)=>Ɨ(l as IMyPistonBase,(PistonStatus)m));
}void Ɨ(IMyPistonBase ž,PistonStatus r){if(r==PistonStatus.Extending){t<Ɣ>(ž);d.u(ž,"onExtending");}else if(r==
PistonStatus.Extended){t<Ɠ>(ž);d.u(ž,"onExtended");}else if(r==PistonStatus.Retracting){t<Ɩ>(ž);d.u(ž,"onRetracting");}else if(r==
PistonStatus.Retracted){t<ƕ>(ž);d.u(ž,"onRetracted");}}public void ƅ(IMyPistonBase ž)=>ƌ(ž,0);public void Ƒ(IMyPistonBase ž){Ə(ž,0);
}public void Ə(IMyPistonBase ž,float ę){ž.Velocity=ę;}public void ƌ(IMyPistonBase ž,float ƀ,float ę=Ę){float Ƙ=ž.Velocity
;float ƙ=ž.CurrentPosition;float ƚ=ę;if(ƀ==0)ž.Retract();if(ƙ<ƀ){ž.MaxLimit=ƀ;ž.Velocity=ƚ;}else if(ƙ>ƀ){ž.MinLimit=ƀ;ž.
Velocity=-ƚ;}A.e<ĸ>().Ĺ(ž,l=>ƛ(l as IMyPistonBase),l=>Ƒ(l as IMyPistonBase));}public bool ƛ(IMyPistonBase ž){return ž.Status==
PistonStatus.Extended||ž.Status==PistonStatus.Retracted;}public void Ɯ(IMyPistonBase ž){ž.Velocity=0;}}public class Ɲ:º{T À
;public override string Á=>"pb/run";public Ɲ(T Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if
(Ä.Å.Count>=2){string ƞ=Ä.Å[0];string Ɵ=string.Join(" ",Ä.Å.GetRange(1,Ä.Å.Count-1));List<IMyProgrammableBlock>Ơ=À.Ê<
IMyProgrammableBlock>(ƞ);if(Ơ.Count==0)return Ì.Í(Î.Ï,ƞ);Ơ.ForEach(ơ=>À.Ƣ(ơ,Ɵ));return Ì.Í(Î.ƣ,ƞ,Ɵ);}return Æ.Ç.ħ;}}public class T:c{public
T(A B):base(B){}public override void o(){f(new Ɲ(this));}public bool Ƣ(IMyProgrammableBlock Ƥ,string Z=""){return Ƥ.
TryRun(Z);}}public class ƥ:º{G À;public override string Á=>"rotor/attach";public ƥ(G Â){À=Â;}public override string Ñ
(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string Ć=Ä.Å[0];List<IMyMotorStator>ć=À.Ê<IMyMotorStator>(Ć);if(ć.Count==0)return Ì.Í(
Î.Ï,Ć);ć.ForEach(l=>À.Ĉ(l));return Ì.Í("Rotor attached: {0}",Ć);}}public class Ʀ:º{G À;public override string Á
=>"rotor/detach";public Ʀ(G Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string Ć=Ä.Å[0];List<
IMyMotorStator>ć=À.Ê<IMyMotorStator>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.Ċ(l));return Ì.Í("Rotor detached: {0}",Ć);}}
public class Ƨ:º{G À;public override string Á=>"rotor/lock";public Ƨ(G Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.
Count==0)return Æ.Ç.È;else{string ƨ=Ä.Å[0];List<IMyMotorStator>Ʃ=À.Ê<IMyMotorStator>(ƨ);if(Ʃ.Count==0)return Ì.Í(Î.Ï,ƨ);bool
ƪ=Ä.Ė("stop")!=null&&Ä.Ė("stop")!="false";Ʃ.ForEach(ƫ=>À.ď(ƫ,ƪ));return Ì.Í(Î.Đ,ƨ);}}}public class Ƭ:º{G À;
public override string Á=>"rotor/reset";public Ƭ(G Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{
string ƨ=Ä.Å[0];List<IMyMotorStator>Ʃ=À.Ê<IMyMotorStator>(ƨ);if(Ʃ.Count==0)return Ì.Í(Î.Ï,ƨ);Ʃ.ForEach(ƫ=>À.ƭ(ƫ));return Ì.Í(Î
.ē,ƨ);}}}public class Ʈ:º{G À;public override string Á=>"rotor/rotate";public Ʈ(G Â){À=Â;}public override string
Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string ƨ=Ä.Å[0];string Ư=Ä.Å[1];string ė=Ä.Ė("speed");float ę=!
string.IsNullOrEmpty(ė)?Math.Abs(float.Parse(ė)):G.Ę;bool Ĝ=Ě(Ä.ě);List<IMyMotorStator>Ʃ=À.Ê<IMyMotorStator>(ƨ);if(Ʃ.Count==0)
return Ì.Í(Î.Ï,ƨ);float Ğ=ĝ(Ư,Ä.ě);float ğ=float.Parse(Ư);float ư=Ġ(ğ,Ʃ.Count,Ĝ);float Ʊ=Ġ(Ğ,Ʃ.Count,Ĝ);Ʃ.ForEach(ƫ=>{float ģ=
Ğ==0?ư:MathHelper.ToDegrees(ƫ.Angle)+Ʊ;À.Ĥ(ƫ,ģ,ę);});string ĥ=Ĝ?"shared ":"";return Ì.Í(Î.Ħ,ƨ,$"{ĥ}angle={Ư}°");}return Ì
.Í(Æ.Ç.ħ);}}public class Ʋ:º{G À;public override string Á=>"rotor/speed";public Ʋ(G Â){À=Â;}public override
string Ñ(Ã Ä){if(Ä.Å.Count<2)return Æ.Ç.È;string ƨ=Ä.Å[0];string ė=Ä.Å[1];List<IMyMotorStator>Ʃ=À.Ê<IMyMotorStator>(ƨ);if(Ʃ.
Count==0)return Ì.Í(Î.Ï,ƨ);try{float Ğ=ĝ(ė,Ä.ě);float ĩ=float.Parse(ė);bool Ī=Ä.Ė("free")=="true";bool Ĝ=Ě(Ä.ě);float Ƴ=Ġ(ĩ,Ʃ
.Count,Ĝ);float Ʊ=Ġ(Ğ,Ʃ.Count,Ĝ);foreach(var ƫ in Ʃ){float Ĭ=Ğ==0?Ƴ:ƫ.TargetVelocityRPM+Ʊ;À.ĭ(ƫ,Ĭ,Ī);}string ĥ=Ĝ?
"shared ":"";return Ì.Í(Î.Į,ƨ,$"{ĥ}speed={ė}");}catch(ArgumentException){return Æ.Ç.ħ;}}}public class ƴ:º{G À;public
override string Á=>"rotor/unlock";public ƴ(G Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string ƨ=Ä.
Å[0];List<IMyMotorStator>Ʃ=À.Ê<IMyMotorStator>(ƨ);if(Ʃ.Count==0)return Ì.Í(Î.Ï,ƨ);Ʃ.ForEach(ƫ=>À.Ƶ(ƫ));return Ì.Í(Î.ı,ƨ);
}}}public class G:Ĳ{Dictionary<IMyMotorStator,float>ƶ=new Dictionary<IMyMotorStator,float>();public const float
Ę=2;const float ĳ=0.05f;public G(A B):base(B){}public override void o(){base.o();f(new Ʋ(this));f(new Ƨ(this));f(new ƴ(
this));f(new Ʈ(this));f(new Ƭ(this));f(new ƥ(this));f(new Ʀ(this));f(new Ĵ(this,"rotor/ulimit",true));f(new Ĵ(this,
"rotor/llimit",false));}public void ƭ(IMyMotorStator ƫ){Ĥ(ƫ,0);}public void Ĥ(IMyMotorStator ƫ,float Ʒ,float Ƹ=Ę){float Ķ=MathHelper.
ToDegrees(ƫ.Angle);if(!ƶ.ContainsKey(ƫ))ƶ[ƫ]=Ķ;Ʒ=ƹ.ƺ(Ʒ,-360,360);float ƻ=ƶ[ƫ];float ķ=Ƽ(Ʒ-ƻ);if(Ʒ==0||Math.Abs(Ʒ)==180)ķ=Ƽ(Ʒ-Ķ);
MyRotationDirection ƽ=ķ>0?MyRotationDirection.CW:MyRotationDirection.CCW;ƫ.RotateToAngle(ƽ,Ʒ,Math.Abs(Ƹ));ƫ.RotorLock=false;d.u(ƫ,
"onMoving");A.e<ĸ>().Ĺ(ƫ,l=>ƾ(l as IMyMotorStator),l=>ƿ(l as IMyMotorStator));ƶ[ƫ]=Ʒ;}float Ƽ(float ǀ){ǀ%=360f;if(ǀ>180f)ǀ-=360f;
if(ǀ<-180f)ǀ+=360f;return ǀ;}public bool ƾ(IMyMotorStator ƫ){float Ķ=MathHelper.ToDegrees(ƫ.Angle);float ğ=ƫ.
TargetVelocityRPM>=0?ƫ.UpperLimitDeg:ƫ.LowerLimitDeg;return Math.Abs(Ķ-ğ)<ĳ;}void ƿ(IMyMotorStator ƫ){ď(ƫ);d.u(ƫ,"onStop");}public void Ƶ
(IMyMotorStator ƫ){İ(ƫ);ƫ.UpperLimitDeg=float.MaxValue;ƫ.LowerLimitDeg=float.MinValue;}}public class ǁ:º{W À;
public override string Á=>"screen/print";public ǁ(W Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if
(Ä.Å.Count>=2){string Ć=Ä.Å[0];var ǂ=Ä.Å[1];string Ţ=Ä.Ė("color");string ǃ=Ä.Ė("size");float Ǆ;float.TryParse(ǃ,out Ǆ);
var ǆ=À.ǅ(Ć);ǆ.ForEach(l=>{if(Ǆ>0)À.Ǉ(l,ǂ,Ţ,Ǆ);else À.Ǉ(l,ǂ,Ţ);});return Ì.Í(Î.Į,Ć,$"color={Ţ}");}return Æ.Ç.ħ;}}public
class ǈ:º{W À;public override string Á=>"screen/bgcolor";public ǈ(W Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.
Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Ć=Ä.Å[0];string Ţ=Ä.Å[1];var ǆ=À.ǅ(Ć);ǆ.ForEach(l=>À.ǉ(l,Ţ));return Ì.Í(Î.
Į,Ć,$"color={Ţ}");}return Æ.Ç.ħ;}}public class Ǌ:º{W À;public override string Á=>"screen/color";public Ǌ(W Â){À=
Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Ć=Ä.Å[0];string Ţ=Ä.Å[1];var ǆ
=À.ǅ(Ć);ǆ.ForEach(l=>À.ǋ(l,Ţ));return Ì.Í(Î.Į,Ć,$"color={Ţ}");}return Æ.Ç.ħ;}}public class W:c{public W(A B):base(B){}
public override void o(){f(new ǁ(this));f(new Ǌ(this));f(new ǈ(this));}public void Ǉ(IMyTextSurface ǌ,string ǂ,string Ţ,float
Ǎ=0){Color ť=ţ.Ť(Ţ);if(ť!=null)ǌ.FontColor=ť;if(Ǎ>0)ǌ.FontSize=Ǎ;ǂ=ǂ.Replace(@"\n","\n");ǌ.WriteText(ǂ,false);}public
void ǋ(IMyTextSurface ǌ,string Ţ){Color ť=ţ.Ť(Ţ);if(ť!=null)ǌ.FontColor=ť;ǌ.ScriptForegroundColor=ť;}public void ǉ(
IMyTextSurface ǌ,string Ţ){Color ť=ţ.Ť(Ţ);if(ť!=null)ǌ.BackgroundColor=ť;ǌ.ScriptBackgroundColor=ť;}public List<IMyTextSurface>ǅ(
string ǎ){return A.e<Ǐ>()?.ǅ(ǎ);}}public class ǐ:Ô{}public class Ǒ:Ô{}public class R:c{Dictionary<IMySensorBlock,
MyDetectedEntityInfo>ǒ=new Dictionary<IMySensorBlock,MyDetectedEntityInfo>();d d;public R(A B):base(B){}public override void o(){d=A.e<d>();
j<IMySensorBlock>(Ǔ=>Ǔ.IsActive,(l,m)=>ǔ(l as IMySensorBlock,(bool)m));}void ǔ(IMySensorBlock Ǔ,object q){var r=q as bool
?;if(r.Value){t<Ǒ>(Ǔ);d.u(Ǔ,"onDetect");ǒ[Ǔ]=Ǔ.LastDetectedEntity;}else{t<ǐ>(Ǔ);d.u(Ǔ,"onClear");}}}public class Ǖ:º{
V À;public override string Á=>"sorter/drain";public Ǖ(V Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count<1)return Æ.Ç
.È;else{string ǖ=Ä.Å[0];bool ǘ=Ã.Ǘ(Ä.Å[1]);List<IMyConveyorSorter>Ǚ=À.Ê<IMyConveyorSorter>(ǖ);if(Ǚ.Count==0)return Ì.Í(Î.
Ï,ǖ);Ǚ.ForEach(ǚ=>À.Ǜ(ǚ,ǘ));return Ì.Í(Î.Į,ǖ,$"drainAll={ǘ}");}}}public class V:c{public V(A B):base(B){}public override
void o(){f(new Ǖ(this));}public void Ǜ(IMyConveyorSorter ǚ,bool ǜ){ǚ.DrainAll=ǜ;}}public class ǝ:º{P À;public
override string Á=>"sound/play";public ǝ(P Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ǟ=Ä.Å[
0];List<IMySoundBlock>ǟ=À.Ê<IMySoundBlock>(Ǟ);if(ǟ.Count==0)return Ì.Í(Î.Ï,Ǟ);if(Ä.Å.Count==1){ǟ.ForEach(Ǡ=>À.ǡ(Ǡ));
return Ì.Í(Î.Ǣ,Ǟ);}else if(Ä.Å.Count>=2){string ǣ=Ä?.Å[1];ǟ.ForEach(Ǡ=>{À.Ǥ(Ǡ,ǣ);À.ǡ(Ǡ);});return$"{Ì.Í(Î.Ǣ,Ǟ)}\nPlaying: {ǣ}"
;}return Æ.Ç.È;}}}public class ǥ:º{P À;public override string Á=>"sound/set";public ǥ(P Â){À=Â;}public override
string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Ǟ=Ä.Å[0];string ǣ=Ä.Å[1];List<IMySoundBlock>ǟ=À.Ê<
IMySoundBlock>(Ǟ);if(ǟ.Count==0)return Ì.Í(Î.Ï,Ǟ);ǟ.ForEach(Ǡ=>À.Ǥ(Ǡ,ǣ));return Ì.Í(Î.Į,Ǟ,$"sound={ǣ}");}return Æ.Ç.ħ;}}public class
Ǧ:º{P À;public override string Á=>"sound/stop";public Ǧ(P Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)
return Æ.Ç.È;else{string Ǟ=Ä.Å[0];List<IMySoundBlock>ǟ=À.Ê<IMySoundBlock>(Ǟ);if(ǟ.Count==0)return Ì.Í(Î.Ï,Ǟ);ǟ.ForEach(Ǡ=>À.ǧ(
Ǡ));return Ì.Í(Î.ƒ,Ǟ);}}}public class P:c{public P(A B):base(B){}public override void o(){f(new ǥ(this));f(new ǝ(this));f
(new Ǧ(this));}public void ǡ(IMySoundBlock Ǡ){Ǡ.Play();}public void ǧ(IMySoundBlock Ǡ){Ǡ.Stop();}public void Ǥ(
IMySoundBlock Ǡ,string Ǩ){Ǡ.SelectedSound=Ǩ;}}public class ǩ:º{O À;public override string Á=>"tank/share";public ǩ(O Â){À=Â;
}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ǫ=Ä.Å[0];List<IMyGasTank>ǫ=À.Ê<IMyGasTank>(Ǫ);if(
ǫ.Count==0)return Ì.Í(Î.Ï,Ǫ);ǫ.ForEach(Ǭ=>À.ǭ(Ǭ));return Ì.Í(Î.Ǯ,Ǫ);}}}public class ǯ:º{O À;public override
string Á=>"tank/stockpile";public ǯ(O Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ǫ=Ä.Å[0];
List<IMyGasTank>ǫ=À.Ê<IMyGasTank>(Ǫ);if(ǫ.Count==0)return Ì.Í(Î.Ï,Ǫ);ǫ.ForEach(Ǭ=>À.ǰ(Ǭ));return Ì.Í(Î.Ǳ,Ǫ);}}}public class
ǲ:º{O À;public override string Á=>"tank/toggle";public ǲ(O Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0
)return Æ.Ç.È;else{string Ǫ=Ä.Å[0];List<IMyGasTank>ǫ=À.Ê<IMyGasTank>(Ǫ);if(ǫ.Count==0)return Ì.Í(Î.Ï,Ǫ);ǫ.ForEach(Ǭ=>À.ǳ(
Ǭ));return Ì.Í(Î.Ó,Ǫ);}}}public class O:c{public O(A B):base(B){}public override void o(){f(new ǯ(this));f(new ǩ(this));f
(new ǲ(this));}void Ǵ(IMyGasTank Ǭ,object q){}public void ǰ(IMyGasTank Ǭ){Ǭ.Stockpile=true;}public void ǭ(IMyGasTank Ǭ){Ǭ
.Stockpile=false;}public void ǳ(IMyGasTank Ǭ){Ǭ.Stockpile=!Ǭ.Stockpile;}}public class ǵ:º{F À;public override
string Á=>"block/off";public ǵ(F Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ć=Ä.Å[0];List<
IMyFunctionalBlock>ć=À.Ê<IMyFunctionalBlock>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.Ƕ(l));return Ì.Í(Î.Ƿ,Ć);}}}public class Ǹ:º
{F À;public override string Á=>"block/on";public Ǹ(F Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)
return Æ.Ç.È;else{string Ć=Ä.Å[0];List<IMyFunctionalBlock>ć=À.Ê<IMyFunctionalBlock>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.
ForEach(l=>À.ǹ(l));return Ì.Í(Î.Ǻ,Ć);}}}public class ǻ:º{F À;public override string Á=>"block/toggle";public ǻ(F Â){À=
Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ć=Ä.Å[0];List<IMyFunctionalBlock>ć=À.Ê<
IMyFunctionalBlock>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.Ǽ(l));return Ì.Í(Î.Ó,Ć);}}}public class ǽ:º{F À;public
override string Á=>"tag/get";public ǽ(F Â){À=Â;}public override string Ñ(Ã Ä){string Ǿ=Ä.Å.FirstOrDefault();var ć=À.ǿ(Ǿ).OrderBy
(Ȁ=>Ȁ.CustomName).ToList();string ȁ=$"Found {ć.Count} blocks with tag: #{Ǿ}\n";return ȁ+string.Join("\n",ć.Select(Ȁ=>
$"- {Ȁ.CustomName}"));}}public class Ȃ:º{F À;public override string Á=>"block/actions";public Ȃ(F Â){À=Â;}public override string Ñ
(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ć=Ä.Å[0];List<IMyTerminalBlock>ć=À.Ê<IMyTerminalBlock>(Ć);if(ć.Count==0)
return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.ȃ(l));return"Actions printed to Log screen Text field.";}}}public class Ȅ:º{F À;
public override string Á=>"block/rename";public Ȅ(F Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count<2)return Æ.Ç.È;string Ć
=Ä.Å[0];string ȅ=Ä.Å[1];List<IMyTerminalBlock>ć=À.Ê<IMyTerminalBlock>(Ć);if(ć.Count==0)return Ì.Í(Î.Ï,Ć);ć.ForEach(l=>À.Ȇ
(l,ȅ));return$"Block renamed: {Ć} -> {ȅ}";}}public class ȇ:º{F À;public override string Á=>"block/action";public
ȇ(F Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Ć=Ä.Å[0];string Ȉ=Ä.Å
[1];List<string>ȉ=Ä.Å.GetRange(2,Ä.Å.Count-2);List<IMyTerminalBlock>ć=À.Ê<IMyTerminalBlock>(Ć);if(ć.Count==0)return Ì.Í(Î
.Ï,Ć);ć.ForEach(l=>À.Ȋ(l,Ȉ,ȉ));return Ì.Í(Î.ȋ,Ć,Ȉ);}return Ì.Í(Æ.Ç.ħ);}}public class Ȍ:º{F À;public override
string Á=>"block/config";public Ȍ(F Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count>=3){string ȍ=Ä.Å[0];string Ȏ=Ä.Å[1];
string ȏ=Ä.Å[2];var ć=À.Ê<IMyTerminalBlock>(ȍ);ć.ForEach(l=>{À.Ȑ(l,Ȏ,ȏ);});string ȁ=$"\"{Ȏ}={ȏ}\" set on {ć.Count} blocks:\n";
return ȁ+string.Join("\n",ć.Select(Ȁ=>$"- {Ȁ.CustomName}"));}return Æ.Ç.ħ;}}public class ȑ:º{F À;public override
string Á=>"tag/set";public ȑ(F Â){À=Â;}public override string Ñ(Ã Ä){string ȍ=Ä.Å[0];string Ǿ=Ä.Å[1];var ć=À.Ȓ(ȍ,Ǿ);string ȁ=
$"Tag \"{Ǿ}\" set on {ć.Count} blocks:\n";return ȁ+string.Join("\n",ć.Select(Ȁ=>$"- {Ȁ.CustomName}"));}}public class F:c{d d;public F(A B):base(B){}public
override void o(){d=A.e<d>();f(new Ǹ(this));f(new ǵ(this));f(new ǻ(this));f(new ȇ(this));f(new Ȃ(this));f(new ǽ(this));f(new ȑ(
this));f(new Ȍ(this));f(new Ȅ(this));}public void ǹ(IMyFunctionalBlock l){l.Enabled=true;d.u(l,"onOn");}public void Ƕ(
IMyFunctionalBlock l){l.Enabled=false;d.u(l,"onOff");}public void Ǽ(IMyFunctionalBlock l){if(l.Enabled)Ƕ(l);else ǹ(l);}public void Ȋ(
IMyTerminalBlock l,string ȓ,List<string>ȉ){List<TerminalActionParameter>Ȕ=new List<TerminalActionParameter>();float ȕ;foreach(var Ȗ in ȉ
){Ȕ.Add(float.TryParse(Ȗ,out ȕ)?TerminalActionParameter.Get(ȕ):TerminalActionParameter.Get(Ȗ));}l.ApplyAction(ȓ,Ȕ);}
public void ȃ(IMyTerminalBlock l){List<ITerminalAction>ȗ=new List<ITerminalAction>();l.GetActions(ȗ);A.e<Ș>()?.ș(String.Join(
", ",ȗ.Select(Ț=>Ț.Id)));}public HashSet<IMyTerminalBlock>ǿ(string Ǿ){if(d.ț.ContainsKey(Ǿ))return d.ț[Ǿ];else return new
HashSet<IMyTerminalBlock>();}public void Ȑ(IMyTerminalBlock l,string Ȏ,string ȏ){string[]Ȝ=Ȏ.Split('.');if(Ȝ.Length==2){d.ȝ[l].
Set(Ȝ[0],Ȝ[1],ȏ);l.CustomData=d.ȝ[l].ToString();}}public List<IMyTerminalBlock>Ȓ(string Ȟ,string Ǿ){List<IMyTerminalBlock>ć
=Ê<IMyTerminalBlock>(Ȟ);ć.ForEach(l=>d.ȟ(l,Ǿ));return ć;}public void Ȇ(IMyTerminalBlock l,string ȅ){l.CustomName=ȅ;}}
public class Ƞ:º{L À;public override string Á=>"thruster/thrust";public Ƞ(L Â){À=Â;}public override string Ñ(Ã Ä){if(
Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count==2){string ȡ=Ä.Å[0];string Ȣ=Ä.Å[1];string ȣ=Ȣ.EndsWith("N")?"N":"%";List<
IMyThrust>Ȥ=À.Ê<IMyThrust>(ȡ);if(Ȥ.Count==0)return Ì.Í(Î.Ï,ȡ);Ȥ.ForEach(ȥ=>À.Ȧ(ȥ,Ȣ));return Ì.Í(Î.Į,ȡ,$"thrust={Ȣ}{ȣ}");}return Æ
.Ç.ħ;}}public class L:c{List<IMyThrust>ȧ;public L(A B):base(B){}public override void o(){ȧ=A.e<d>().ì<IMyThrust>();f(new
Ƞ(this));}public void Ȧ(IMyThrust ȥ,string Ȩ){if(Ȩ.Contains("N")){float ȩ=float.Parse(Ȩ.Replace("N",""));ȥ.ThrustOverride
=ȩ;}else{float Ȫ=float.Parse(Ȩ.Replace("%",""))/100;ȥ.ThrustOverridePercentage=Ȫ;}}void ȫ(IMyThrust ȥ){ȥ.ThrustOverride=0
;}public void Ȭ(){ȧ.ForEach(ȥ=>ȫ(ȥ));}}public class ȭ:º{S À;public override string Á=>"timer/start";public ȭ(S Â
){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ȯ=Ä.Å[0];List<IMyTimerBlock>ȯ=À.Ê<
IMyTimerBlock>(Ȯ);if(ȯ.Count==0)return Ì.Í(Î.Ï,Ȯ);string Ȱ=Ä.Ė("delay");float ȱ=0;if(!string.IsNullOrEmpty(Ȱ)&&!float.TryParse(Ȱ,out
ȱ))return Ì.Í(Î.Ȳ,"delay");;ȯ.ForEach(ȳ=>{if(ȱ>0)À.ȴ(ȳ,ȱ);À.ȵ(ȳ);});return Ì.Í(Î.Ǣ,Ȯ);}}}public class ȶ:º{S À;
public override string Á=>"timer/stop";public ȶ(S Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{
string Ȯ=Ä.Å[0];List<IMyTimerBlock>ȯ=À.Ê<IMyTimerBlock>(Ȯ);if(ȯ.Count==0)return Ì.Í(Î.Ï,Ȯ);ȯ.ForEach(ȳ=>À.ȷ(ȳ));return Ì.Í(
$"Timer Stopped: {Ȯ}");}}}public class ȸ:º{S À;public override string Á=>"timer/trigger";public ȸ(S Â){À=Â;}public override string Ñ
(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ȯ=Ä.Å[0];List<IMyTimerBlock>ȯ=À.Ê<IMyTimerBlock>(Ȯ);if(ȯ.Count==0)return Ì
.Í(Î.Ï,Ȯ);ȯ.ForEach(ȳ=>À.ȹ(ȳ));return Ì.Í($"Timer Triggered: {Ȯ}");}}}public class S:c{public S(A B):base(B){}public
override void o(){f(new ȸ(this));f(new ȭ(this));f(new ȶ(this));}public void ȹ(IMyTimerBlock ȳ){ȳ.Trigger();}public void ȵ(
IMyTimerBlock ȳ){ȳ.StartCountdown();}public void ȷ(IMyTimerBlock ȳ){ȳ.StopCountdown();}public void ȴ(IMyTimerBlock ȳ,float ȱ){ȳ.
TriggerDelay=ȱ;}}public class Ⱥ:º{X À;public override string Á=>"wheel/friction";public Ⱥ(X Â){À=Â;}public override string
Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Ȼ=Ä.Å[0];string ȼ=Ä.Å[1];List<IMyMotorSuspension>Ƚ=À.Ê<
IMyMotorSuspension>(Ȼ);if(Ƚ.Count==0)return Ì.Í(Î.Ï,Ȼ);float ɀ=Ⱦ.ȿ<float>(ȼ.Replace("%",""));Ƚ.ForEach(Ɂ=>À.ɂ(Ɂ,ɀ));return Ì.Í(Î.Į,Ȼ,
$"friction={ɀ}%");}return Æ.Ç.ħ;}}public class Ƀ:º{X À;public override string Á=>"wheel/height";public Ƀ(X Â){À=Â;}public
override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Ȼ=Ä.Å[0];string Ʉ=Ä.Å[1];List<
IMyMotorSuspension>Ƚ=À.Ê<IMyMotorSuspension>(Ȼ);if(Ƚ.Count==0)return Ì.Í(Î.Ï,Ȼ);float Ğ=ĝ(Ʉ,Ä.ě);float Ʌ=float.Parse(Ʉ);Ƚ.ForEach(Ɂ=>{
float Ɇ=Ğ==0?Ʌ:Ɂ.Height+Ğ;À.ɇ(Ɂ,Ɇ);});return Ì.Í(Î.Į,Ȼ,$"height={Ʉ}m");}return Æ.Ç.ħ;}}public class Ɉ:º{X À;public
override string Á=>"wheel/power";public Ɉ(X Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count
>=2){string Ȼ=Ä.Å[0];string ɉ=Ä.Å[1];List<IMyMotorSuspension>Ƚ=À.Ê<IMyMotorSuspension>(Ȼ);if(Ƚ.Count==0)return Ì.Í(Î.Ï,Ȼ);
float Ɋ=Ⱦ.ȿ<float>(ɉ.Replace("%",""));Ƚ.ForEach(Ɂ=>À.ɋ(Ɂ,Ɋ));return Ì.Í(Î.Į,Ȼ,$"power={Ɋ}%");}return Æ.Ç.ħ;}}public class Ɍ:º
{X À;public override string Á=>"wheel/strength";public Ɍ(X Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0
)return Æ.Ç.È;else if(Ä.Å.Count>=2){string Ȼ=Ä.Å[0];string ɍ=Ä.Å[1];List<IMyMotorSuspension>Ƚ=À.Ê<IMyMotorSuspension>(Ȼ);
if(Ƚ.Count==0)return Ì.Í(Î.Ï,Ȼ);float Ɏ=Ⱦ.ȿ<float>(ɍ.Replace("%",""));Ƚ.ForEach(Ɂ=>À.ɏ(Ɂ,Ɏ));return Ì.Í(Î.Į,Ȼ,
$"strength={Ɏ}%");}return Æ.Ç.ħ;}}public class X:c{public X(A B):base(B){}public override void o(){f(new Ƀ(this));f(new Ɉ(this));f(new Ɍ
(this));f(new Ⱥ(this));}public void ɇ(IMyMotorSuspension Ɂ,float ɐ){Ɂ.Height=ɐ;}public void ɋ(IMyMotorSuspension Ɂ,float
Ɋ){Ɂ.Power=Ɋ;}public void ɏ(IMyMotorSuspension Ɂ,float Ɏ){Ɂ.Strength=Ɏ;}public void ɂ(IMyMotorSuspension Ɂ,float ɀ){Ɂ.
Friction=ɀ;}}public class ɑ:º{A A;public override string Á=>"boot";public ɑ(A B){A=B;}public override string Ñ(Ã Ä){A.ɒ
=true;return"Rebooting...";}}public abstract class º:ɓ{public abstract string Á{get;}public abstract string Ñ(Ã Ä);public
string ɔ()=>Á;protected float ĝ(string ɕ,Dictionary<string,string>ɖ){float ɗ;if(!float.TryParse(ɕ,out ɗ))throw new
ArgumentException("Invalid numerical value provided.");bool ɘ=ɖ.ContainsKey("add");bool ə=ɖ.ContainsKey("sub");float ȁ=0;if(ɘ)ȁ=ɗ;else if
(ə)ȁ=-ɗ;return ȁ;}protected float Ġ(float ɚ,int ɛ,bool ɜ){if(ɜ&&ɛ>1)return ɚ/ɛ;return ɚ;}protected bool Ě(Dictionary<
string,string>ɖ){return ɖ.ContainsKey("share");}}public interface ɓ{string ɔ();string Ñ(Ã Ä);}public class ɠ:º{A A;
public override string Á=>"purge";bool ɝ=false;List<string>ɞ=new List<string>();List<string>ɟ=new List<
string>();public ɠ(A B){A=B;}void ɤ(string Â){switch(Â){case"almanac":A.e<ɡ>().ɢ();ɟ.Add(Â);break;case"storage":A.e<ɣ>().ɢ();ɟ
.Add(Â);break;default:break;}}public override string Ñ(Ã Ä){ɞ.Clear();ɟ.Clear();foreach(var ɥ in Ä.ě){string ɦ=ɥ.Key;
string ȏ=ɥ.Value;if(ɦ=="force"&&(ȏ=="true"||ȏ=="1"))ɝ=true;}if(!ɝ)return"Run command with --force to purge";if(Ä.Å.Count==0)
return Æ.Ç.È;else{string ɧ=Ä.Å[0];List<string>ɨ=ɧ.Split(',').ToList();if(ɨ.Contains("*")){ɞ.Add("almanac");ɞ.Add("storage");}
if(ɨ.Contains("storage"))ɞ.Add("storage");if(ɨ.Contains("almanac"))ɞ.Add("almanac");ɞ.ForEach(Â=>ɤ(Â));return ɟ.Count==0?
"No modules purged":$"Purged {ɟ.Count} modules: {string.Join(", ",ɟ)}";}}}public class ɪ:º{A A;Random ɩ=new Random();
public override string Á=>"rename";public ɪ(A B){A=B;}public override string Ñ(Ã Ä){if(Ä.Å.Count<1)return Æ.Ç.È;string ȅ=Ä.Å[0
];bool ɫ=Ã.Ǘ(Ä.Ė("unique"));if(ɫ)ȅ=$"{ȅ}-{ɩ.Next(10000,99999)}";A.ɬ.CustomName=ȅ;A.Á=ȅ;var ɮ=A.e<ɭ>();ɮ.ɯ.Set("general",
"name",ȅ);A.ɰ.CustomData=ɮ.ɯ.ToString();var ɲ=A.e<ɱ>();ɲ.ɳ();ɲ.ɴ();return$"Grid name set to: {ȅ}";}}public class ĸ:ɵ{public
Dictionary<IMyTerminalBlock,ɶ>ɷ{get;}public struct ɶ{public Func<IMyTerminalBlock,bool>ɸ;public Action<IMyTerminalBlock>ɹ;public ɶ
(Func<IMyTerminalBlock,bool>ɺ,Action<IMyTerminalBlock>ɻ){ɸ=ɺ;ɹ=ɻ;}}public ĸ(A B):base(B){ɷ=new Dictionary<
IMyTerminalBlock,ɶ>();}public override void b(){var ɼ=new List<IMyTerminalBlock>();foreach(var ɽ in ɷ){IMyTerminalBlock l=ɽ.Key;ɶ ɾ=ɽ.
Value;if(ɾ.ɸ(l)){ɾ.ɹ?.Invoke(l);ɼ.Add(l);}}ɼ.ForEach(l=>ɿ(l));}public void Ĺ(IMyTerminalBlock l,Func<IMyTerminalBlock,bool>ʀ,
Action<IMyTerminalBlock>ʁ){if(!ɷ.ContainsKey(l))ɷ[l]=new ɶ(ʀ,ʁ);}public void ɿ(IMyTerminalBlock l){if(ɷ.ContainsKey(l))ɷ.
Remove(l);}}public class ɡ:ɵ{const double ʂ=300;public List<ʃ>ʄ=new List<ʃ>();public ɡ(A B):base(B){A=B;}public override void
o(){ʅ();var ʆ=A.e<ʆ>();ʆ.ʇ(ʈ,1);ʆ.ʇ(ʉ,30);}public void ʈ(){ʃ ʌ=ʊ($"{A.ʋ}");MatrixD ʎ=A.ʍ();Vector3D ʏ=ʎ.Translation;if(ʌ
==null){ʌ=new ʃ($"{A.ʋ}","grid",ʏ,A.ɬ.Speed);}Vector3D?ʐ=(Vector3D?)ʎ.Forward;Vector3D?ʑ=(Vector3D?)ʎ.Up;ʌ.ʒ(ʏ,A.ɬ.Speed,A
.Á,A.ʓ.Radius,ʐ,ʑ);ʔ(ʌ);}void ʅ(){string ʖ=A.e<ɣ>().ʕ("almanac")??"";if(ʖ=="")return;Dictionary<string,object>ʗ;try{ʗ=ʘ.ʙ
(ʖ);}catch{return;}foreach(var ʌ in ʗ){try{Dictionary<string,object>ʚ=(Dictionary<string,object>)ʌ.Value;ʃ ʜ=ʃ.ʛ(ʚ);if(ʝ(
ʜ))ʄ.Add(ʜ);}catch{}}}bool ʝ(ʃ ʌ){if(ʌ==null||string.IsNullOrEmpty(ʌ.ʋ))return false;if(ʌ.ʞ=="waypoint")return true;if(ʌ.
ʞ=="grid"&&ʌ.ʟ==0)return false;if(ʠ(ʌ))return false;return true;}bool ʠ(ʃ ʌ){if(ʌ.ʞ=="waypoint")return false;double ʢ=(
DateTime.Now-ʌ.ʡ).TotalSeconds;return ʢ>ʂ;}void ʉ(){int ʣ=ʄ.RemoveAll(ʌ=>ʌ.ʋ!=$"{A.ʋ}"&&ʠ(ʌ));if(ʣ>0)Y();}void Y(){var ʗ=new
Dictionary<string,object>();foreach(var ʌ in ʄ)ʗ[ʌ.ʋ]=ʌ;A.e<ɣ>().ʤ("almanac",ʘ.ʥ(ʗ));}public void ɢ(){ʄ.Clear();Y();}public ʃ ʴ(
string ʦ,long ʧ,string ǎ,Vector3D ʏ,float ę,HashSet<string>ʨ,bool ʩ,Vector3D?ʐ=null,Vector3D?ʑ=null){ʃ ʌ;ʃ ʪ=ʊ(ʦ);if(ʪ!=null){
ʪ.ʒ(ʏ,ę,ǎ,0,ʐ,ʑ);ʪ.ʟ=ʧ;ʪ.ʫ.UnionWith(ʨ);ʌ=ʪ;}else{ʌ=new ʃ(ʦ,"grid",ʏ,ę){ʟ=ʧ,ʬ=ǎ,ʫ=ʨ};if(ʐ.HasValue)ʌ.ʭ=ʐ.Value;if(ʑ.
HasValue)ʌ.ʮ=ʑ.Value;if(ʩ)ʌ.ʯ=ʃ.ʰ.ʱ;}if(!ʨ.Contains("*")&&ʌ.ʯ==ʃ.ʰ.ʲ)ʌ.ʯ=ʃ.ʰ.ʳ;ʔ(ʌ);return ʌ;}public ʃ ʊ(string ʵ){return ʄ.Find
(ʌ=>ʌ.ʋ==ʵ||ʌ.ʬ==ʵ);}public List<ʃ>ʷ(string ʶ){return ʄ.FindAll(ʌ=>ʌ.ʞ==ʶ);}public void ʔ(ʃ ʌ){ʃ ʪ=ʄ.Find(ʸ=>ʸ.ʋ==ʌ.ʋ);if
(ʪ==null)ʄ.Add(ʌ);else{if(ʌ.ʡ>ʪ.ʡ){ʄ.Remove(ʪ);ʄ.Add(ʌ);}}Y();}}public class ʃ:ʹ{public static Dictionary<string
,string>ʺ=new Dictionary<string,string>{{"grid","grid"},{"waypoint","waypoint"}};public enum ʰ{ʱ,ʳ,ʲ,ʻ}public string ʋ{
get;}public long ʟ{get;set;}public DateTime ʡ;public Vector3D ʼ;public Vector3D ʭ=Vector3D.Forward;public Vector3D ʮ=
Vector3D.Up;public float ʽ;public double ʾ;public string ʞ{get;set;}public string ʬ{get;set;}public ʰ ʯ{get;set;}public HashSet<
string>ʫ=new HashSet<string>();public ʃ(string ʿ,string ˀ,Vector3D ʏ,float ę=0){ʋ=ʿ;ʡ=DateTime.Now;ʼ=ʏ;ʽ=ę;ʞ=ˀ;ʯ=ʰ.ʲ;}public
void ʒ(Vector3D ʏ,float ę,string ˁ=null,double ˆ=0,Vector3D?ʐ=null,Vector3D?ʑ=null){ʼ=ʏ;ʽ=ę;ʡ=DateTime.Now;ʬ=ˁ??ʬ;ʾ=ˆ>0?ˆ:ʾ;
ʭ=ʐ??ʭ;ʮ=ʑ??ʮ;}public bool ˇ()=>ʯ==ʰ.ʳ;public bool ˈ()=>ʯ==ʰ.ʻ;public bool ˉ()=>ʯ==ʰ.ʲ;public bool ˊ()=>ʯ==ʰ.ʱ;public
string ˌ(){Dictionary<string,object>ˋ=new Dictionary<string,object>{{"Id",$"{ʋ}"},{"UnicastId",$"{ʟ}"},{"DisplayName",$"{ʬ}"},
{"UpdatedAt",$"{ʡ.Ticks}"},{"pos",$"{ʼ}"},{"LastKnownSpeed",$"{ʽ}"},{"EntityType",ʺ[ʞ]},{"SafeRadius",$"{ʾ}"},{"fx",
$"{ʭ.X}"},{"fy",$"{ʭ.Y}"},{"fz",$"{ʭ.Z}"},{"ux",$"{ʮ.X}"},{"uy",$"{ʮ.Y}"},{"uz",$"{ʮ.Z}"}};return ʘ.ʥ(ˋ);}public static ʃ ʛ(
Dictionary<string,object>ˍ){string[]ˎ=$"{ˍ["pos"]}".Split(' ');double ˏ=double.Parse(ˎ[0].Substring(2));double ː=double.Parse(ˎ[1]
.Substring(2));double ˑ=double.Parse(ˎ[2].Substring(2));string ˀ=$"{ˍ["EntityType"]}";ʃ ˠ=new ʃ($"{ˍ["Id"]}",ʺ[ˀ],new
Vector3D(ˏ,ː,ˑ),0);object ˡ;long ˢ;if(ˍ.TryGetValue("UnicastId",out ˡ)&&ˡ!=null&&long.TryParse(ˡ.ToString(),out ˢ)){ˠ.ʟ=ˢ;}
object ˣ;if(ˍ.TryGetValue("DisplayName",out ˣ)&&ˣ!=null&&!string.IsNullOrEmpty(ˣ.ToString())){ˠ.ʬ=ˣ.ToString();}double ˆ;
object ˤ;if(ˍ.TryGetValue("SafeRadius",out ˤ)&&ˤ!=null&&double.TryParse(ˤ.ToString(),out ˆ)){ˠ.ʾ=ˆ;}double ˬ,ˮ,Ͱ,ͱ,Ͳ,ͳ;object
ʹ,Ͷ,ͷ,ͺ,ͻ,ͼ;if(ˍ.TryGetValue("fx",out ʹ)&&double.TryParse($"{ʹ}",out ˬ)&&ˍ.TryGetValue("fy",out Ͷ)&&double.TryParse(
$"{Ͷ}",out ˮ)&&ˍ.TryGetValue("fz",out ͷ)&&double.TryParse($"{ͷ}",out Ͱ)){ˠ.ʭ=new Vector3D(ˬ,ˮ,Ͱ);}if(ˍ.TryGetValue("ux",out ͺ)
&&double.TryParse($"{ͺ}",out ͱ)&&ˍ.TryGetValue("uy",out ͻ)&&double.TryParse($"{ͻ}",out Ͳ)&&ˍ.TryGetValue("uz",out ͼ)&&
double.TryParse($"{ͼ}",out ͳ)){ˠ.ʮ=new Vector3D(ͱ,Ͳ,ͳ);}return ˠ;}public long ͽ()=>long.Parse(ʋ);}public abstract class Ώ:Ά,Έ{
public A A;Dictionary<IMyTerminalBlock,Func<IMyTerminalBlock,object>>Ή=new Dictionary<IMyTerminalBlock,Func<
IMyTerminalBlock,object>>();Dictionary<IMyTerminalBlock,Action<IMyTerminalBlock,object>>Ί=new Dictionary<
IMyTerminalBlock,Action<IMyTerminalBlock,object>>();public Dictionary<long,object>Ό=new Dictionary<long,object>();public List<ɓ
>Ύ=new List<ɓ>();public Ώ(A B){A=B;}public virtual void b(){}public virtual void o(){}public virtual IEnumerator<double>ΐ
(){o();yield return 0.0;}public virtual void Γ(Ô Α,object Β){}public virtual string Δ()=>$"{GetType()}";public virtual
void f(ɓ Ä){Ύ.Add(Ä);}protected void j<Ε>(Func<Ε,object>Ζ,Action<IMyTerminalBlock,object>Η)where Ε:class,IMyTerminalBlock{d
d=A.e<d>();foreach(var l in d.ì<Ε>()){Ή[l]=(Ȁ)=>Ζ(Ȁ as Ε);Ί[l]=Η;d.Θ(l,this);Ό[l.EntityId]=Ζ(l);}}public object Ι(
IMyTerminalBlock l){return Ή.ContainsKey(l)?Ή[l](l):null;}public bool Μ(IMyTerminalBlock l,object Κ){if(!Ή.ContainsKey(l))return false;
object Λ=Ή[l](l);return Κ==null||!Equals(Κ,Λ);}public void Ξ(IMyTerminalBlock l){if(!Ή.ContainsKey(l))return;long Ν=l.EntityId
;object Λ=Ή[l](l);if(!Ό.ContainsKey(Ν)||!Equals(Ό[Ν],Λ)){if(Ί.ContainsKey(l))Ί[l](l,Λ);Ό[Ν]=Λ;}}public List<Ε>Ê<Ε>(string
ǎ)where Ε:class,IMyTerminalBlock{return A.e<d>().Ê<Ε>(ǎ);}public Ε e<Ε>()where Ε:class,Ά{return A.e<Ε>();}public void Ρ<Ο
>()where Ο:Ô{A.e<Π>().Ρ<Ο>(this);}public void t(Ô Α,object Β){A.e<Π>().t(Α,Β);}public void t<Ο>(object Β=null)where Ο:Ô,
new(){t(new Ο(),Β);}public List<ɓ>Σ()=>Ύ;public void Ϊ(string Τ,Func<Υ,Φ>Χ){e<ɱ>().Ψ.Ω(Τ,Χ);}}public abstract class ɵ:Ώ,Ϋ{
public ɵ(A B):base(B){}}public abstract class c:Ώ,E{public c(A B):base(B){}}public class d:ɵ{ʆ ʆ;ɭ ɭ;Π Π;public HashSet<long>ά
=new HashSet<long>();List<IMyBlockGroup>έ=new List<IMyBlockGroup>();public List<IMyTerminalBlock>ή=new List<
IMyTerminalBlock>();public Dictionary<IMyTerminalBlock,MyIni>ȝ=new Dictionary<IMyTerminalBlock,MyIni>();public Dictionary<string,HashSet<IMyTerminalBlock>>ț=new Dictionary<string,HashSet<IMyTerminalBlock>>();Dictionary<
IMyTerminalBlock,Dictionary<string,string>>ί=new Dictionary<IMyTerminalBlock,Dictionary<string,string>>();Dictionary<long,
object>ΰ=new Dictionary<long,object>();Dictionary<IMyTerminalBlock,Έ>α=new Dictionary<IMyTerminalBlock,Έ>();int β=0;
const int γ=50;const string δ="general";const string ε="tags";const string ζ="hooks";bool η=false;public d(A B):base(B){A=B;}
public override IEnumerator<double>ΐ(){o();foreach(var ι in θ(A.ɬ))yield return ι;κ();ʆ.λ(μ());yield break;}public override
void o(){ɭ=A.e<ɭ>();Π=A.e<Π>();ʆ=A.e<ʆ>();Π.Ρ<ν>(this);Π.Ρ<ξ>(this);Π.Ρ<ο>(this);Π.Ρ<π>(this);Π.Ρ<ρ>(this);Π.Ρ<ς>(this);Π.Ρ<
σ>(this);}IEnumerable<double>μ(){τ();ʆ.λ(μ(),1);yield return 0;}public override void b(){υ();}void υ(){var φ=α.Keys.
ToList();int χ=φ.Count;var ψ=φ.Skip(β).Take(γ).ToList();foreach(var l in ψ){if(!α.ContainsKey(l))continue;Έ ω=α[l];object Κ=ΰ.
ContainsKey(l.EntityId)?ΰ[l.EntityId]:null;object Λ=ω.Ι(l);if(ω.Μ(l,Κ)){ω.Ξ(l);ΰ[l.EntityId]=Λ;}}β+=γ;if(β>=χ)β=0;}public override
void Γ(Ô Α,object Β){if(Α is ν||Α is ξ)ϊ();if(Α is ς||Α is σ)ϊ();if(Α is ο||Α is π)ϋ();if(Α is ρ)κ();}public
IMyTerminalBlock ȟ(IMyTerminalBlock l,string Ǿ){MyIni ύ=ό(l);string ώ=$"{ύ.Get(δ,ε)}";if(ώ=="")ώ=Ǿ;else if(!ώ.Contains(Ǿ))ώ+=$",{Ǿ}";ύ.
Set(δ,ε,ώ);l.CustomData=ύ.ToString();if(!ț.ContainsKey(Ǿ))ț[Ǿ]=new HashSet<IMyTerminalBlock>();ț[Ǿ].Add(l);return l;}public
void Θ(IMyTerminalBlock l,Έ ω){α[l]=ω;ΰ[l.EntityId]=ω.Ι(l);}public List<Ε>ì<Ε>(Func<Ε,bool>Ϗ=null)where Ε:class,
IMyTerminalBlock{List<Ε>ϐ=ή.OfType<Ε>().Where(l=>Ϗ==null||Ϗ(l)).ToList();return ϐ;}bool ϒ(IMyTerminalBlock l,MyIni ϑ){return ȝ.
ContainsKey(l)&&ϑ.ToString()!=ȝ[l].ToString();}void τ(){foreach(var l in ή){MyIni ύ=new MyIni();MyIniParseResult ϓ;if(!ύ.TryParse(l
.CustomData,out ϓ))continue;if(A.ϔ==A.ϕ.ϖ)ȝ[l]=ύ;else if(A.ϔ==A.ϕ.ϗ&&ϒ(l,ύ)){ȝ[l]=ύ;t<Ϙ>(l);if(l.EntityId==A.ʋ)t<ρ>(l);A.
ϙ($"Config changed: {l.CustomName}",false);}if(ύ.ToString()=="")Ϛ(l,ύ);if(ύ.ContainsSection(δ))ϛ(l,ύ);if(ύ.
ContainsSection(ζ))Ϝ(l,ύ);}}void Ϛ(IMyTerminalBlock l,MyIni ύ){foreach(var ɦ in ɭ.ϝ.Keys){string[]Ϟ=ɦ.Split('.');ύ.Set(Ϟ[0],Ϟ[1],ɭ.ϝ[ɦ]
);}l.CustomData=ύ.ToString();}void ϛ(IMyTerminalBlock l,MyIni ύ){string ώ=$"{ύ.Get(δ,ε)}";if(ώ=="")return;foreach(var Ǿ
in ώ.Split(',')){string ϟ=Ǿ.Trim();if(!ț.ContainsKey(ϟ))ț[ϟ]=new HashSet<IMyTerminalBlock>();ț[ϟ].Add(l);}}void Ϝ(
IMyTerminalBlock l,MyIni ύ){var Ϡ=new Dictionary<string,string>();List<MyIniKey>ϡ=new List<MyIniKey>();ύ.GetKeys(ζ,ϡ);foreach(var Ϣ in ϡ
){string ϣ=$"{ύ.Get(ζ,Ϣ.Name)}";string ϥ=Ϥ(l,ϣ);Ϡ[Ϣ.Name]=ϥ;}ί[l]=Ϡ;}string Ϥ(IMyTerminalBlock l,string ϣ){StringBuilder
Ϧ=new StringBuilder();int ϧ=0;while(ϧ<ϣ.Length){int Ϩ=ϣ.IndexOf("this",ϧ);if(Ϩ==-1){Ϧ.Append(ϣ.Substring(ϧ));break;}if(Ϩ>
0&&(ϣ[Ϩ-1]==' '||ϣ[Ϩ-1]=='=')&&(Ϩ+4==ϣ.Length||ϣ[Ϩ+4]==' '||ϣ[Ϩ+4]==';')){Ϧ.Append(ϣ.Substring(ϧ,Ϩ-ϧ));Ϧ.Append(
$"\"{l.CustomName}\"");}else{Ϧ.Append(ϣ.Substring(ϧ,Ϩ-ϧ+4));}ϧ=Ϩ+4;}return$"{Ϧ}";}void κ(){MyIni Ϫ=ɭ.ϩ;List<MyIniKey>ϡ=new List<MyIniKey>();Ϫ
.GetKeys(ζ,ϡ);foreach(var ɦ in ϡ){if(ɦ.Name.Contains(".")){string[]Ϟ=ɦ.Name.Split('.');string Ć=Ϟ[0].Trim('\"');string Ϣ=
Ϟ[1];foreach(var l in Ê<IMyTerminalBlock>(Ć)){if(!ί.ContainsKey(l))ί[l]=new Dictionary<string,string>();ί[l][Ϣ]=Ϫ.Get(ζ,ɦ
.Name).ToString();}}}}public void u(IMyTerminalBlock l,string Ϣ){if(ί.ContainsKey(l)&&ί[l].ContainsKey(Ϣ)){string ϫ=ί[l][
Ϣ];A.e<Æ>().Ϭ(ϫ);}}public MyIni ό(IMyTerminalBlock l){return ȝ.ContainsKey(l)?ȝ[l]:new MyIni();}public new List<Ε>Ê<Ε>(
string ǎ)where Ε:class,IMyTerminalBlock{List<Ε>ć=new List<Ε>();List<IMyBlockGroup>Ϯ=ϭ(ǎ);if(Ϯ.Count>0){foreach(var ϰ in Ϯ){
List<Ε>ϯ=new List<Ε>();ϰ.GetBlocksOfType(ϯ);ć.AddRange(ϯ.Where(l=>ά.Contains(l.CubeGrid.EntityId)));}}else if(ǎ.StartsWith(
"#")){if(ț.ContainsKey(ǎ.Substring(1))){ț[ǎ.Substring(1)]?.ToList().ForEach(l=>{if(l is Ε&&ά.Contains(l.CubeGrid.EntityId))
ć.Add(l as Ε);});}else{A.ϙ($"Tag not found: {ǎ}");return ć;}}else{Ε l=ϱ(ǎ)as Ε;if(l!=null&&ά.Contains(l.CubeGrid.EntityId
))ć.Add(l);}return ć;}public void ϊ(){έ.Clear();A.ϲ.GetBlockGroups(έ);}List<IMyBlockGroup>ϭ(string ϳ){return έ.Where(ϰ=>
string.Equals(ϰ.Name,ϳ,StringComparison.OrdinalIgnoreCase)).ToList();}IMyTerminalBlock ϱ(string ǎ)=>ή.FirstOrDefault(ˏ=>ˏ.
DisplayNameText==ǎ);void ϵ(){A.ϴ=ή.OfType<IMyShipController>().OrderByDescending(î=>î.IsMainCockpit).FirstOrDefault();}List<
IMyMechanicalConnectionBlock>Ϸ=new List<IMyMechanicalConnectionBlock>();Queue<IMyCubeGrid>ϸ=new Queue<IMyCubeGrid>();HashSet<long>
Ϲ=new HashSet<long>();Dictionary<long,HashSet<long>>Ϻ=new Dictionary<long,HashSet<long>>();const int ϻ=40;const int ϼ=500
;IEnumerable<double>θ(IMyCubeGrid Ͻ){Ͼ();ά.Clear();foreach(var ι in Ͽ(Ͻ,ά))yield return ι;foreach(var ι in Ѐ())yield
return ι;}void Ͼ(){Ϸ.Clear();Ϻ.Clear();A.ϲ.GetBlocksOfType(Ϸ);for(int Ё=0;Ё<Ϸ.Count;Ё++){var Ђ=Ϸ[Ё];var Ț=Ђ.CubeGrid;var Ȁ=Ђ.
TopGrid;if(Ț==null||Ȁ==null)continue;long Ѓ=Ț.EntityId,Є=Ȁ.EntityId;HashSet<long>Ѕ;if(!Ϻ.TryGetValue(Ѓ,out Ѕ))Ϻ[Ѓ]=Ѕ=new
HashSet<long>();Ѕ.Add(Є);HashSet<long>І;if(!Ϻ.TryGetValue(Є,out І))Ϻ[Є]=І=new HashSet<long>();І.Add(Ѓ);}}IMyCubeGrid Ј(long Ї){
for(int Ё=0;Ё<Ϸ.Count;Ё++){var Ȁ=Ϸ[Ё];if(Ȁ.CubeGrid!=null&&Ȁ.CubeGrid.EntityId==Ї)return Ȁ.CubeGrid;if(Ȁ.TopGrid!=null&&Ȁ.
TopGrid.EntityId==Ї)return Ȁ.TopGrid;}return null;}void Ћ(IMyTerminalBlock l){var Љ=new MyIni();MyIniParseResult Њ;if(!Љ.
TryParse(l.CustomData,out Њ))return;ȝ[l]=Љ;if(Љ.ToString().Length==0)Ϛ(l,Љ);if(Љ.ContainsSection(δ))ϛ(l,Љ);if(Љ.ContainsSection(
ζ))Ϝ(l,Љ);}IEnumerable<double>А(HashSet<long>Ќ){var Ѝ=new List<IMyTerminalBlock>();A.ϲ.GetBlocks(Ѝ);var Ў=Ѝ.Where(Ȁ=>Ќ.
Contains(Ȁ.CubeGrid.EntityId)).ToList();int Ϩ=0;while(Ϩ<Ў.Count){int Џ=Math.Min(ϼ,Ў.Count-Ϩ);for(int Ё=0;Ё<Џ;Ё++){var l=Ў[Ϩ+Ё];ή
.Add(l);Ћ(l);}Ϩ+=Џ;yield return 0;}}void В(HashSet<long>Ќ){ή.RemoveAll(Ȁ=>{if(!Ќ.Contains(Ȁ.CubeGrid.EntityId))return
false;ȝ.Remove(Ȁ);α.Remove(Ȁ);ΰ.Remove(Ȁ.EntityId);ί.Remove(Ȁ);foreach(var Б in ț.Values)Б.Remove(Ȁ);return true;});}
IEnumerable<double>Ͽ(IMyCubeGrid Ͻ,HashSet<long>ϓ){ϸ.Clear();Ϲ.Clear();ϸ.Enqueue(Ͻ);while(ϸ.Count>0){int Г=0;while(ϸ.Count>0&&Г<ϻ){
var Д=ϸ.Dequeue();long Е=Д.EntityId;if(Ϲ.Contains(Е)){Г++;continue;}Ϲ.Add(Е);ϓ.Add(Е);HashSet<long>Ж;if(Ϻ.TryGetValue(Е,out
Ж)){foreach(long З in Ж){var И=Ј(З);if(И!=null&&!Ϲ.Contains(З))ϸ.Enqueue(И);}}Г++;}yield return 0;}}IEnumerable<double>Ѐ(
){ț.Clear();ί.Clear();ȝ.Clear();ή.Clear();var Й=new List<IMyTerminalBlock>();A.ϲ.GetBlocks(Й);for(int Ё=0;Ё<Й.Count;Ё++){
var К=Й[Ё];if(ά.Contains(К.CubeGrid.EntityId))ή.Add(К);}ϵ();int Ϩ=0;while(Ϩ<ή.Count){int Џ=Math.Min(ϼ,ή.Count-Ϩ);for(int Ё=
0;Ё<Џ;Ё++)Ћ(ή[Ϩ+Ё]);Ϩ+=Џ;yield return 0;}ϊ();yield return 0;}public void ϋ(){if(η)return;η=true;ʆ.λ(Л());}public void О(
IMyCubeGrid М){if(М==null||η)return;if(ά.Contains(М.EntityId))return;η=true;ʆ.λ(Н(М));}public void Р(){if(η)return;η=true;ʆ.λ(П());
}IEnumerable<double>Н(IMyCubeGrid С){Ͼ();ϸ.Clear();Ϲ.Clear();var Т=new HashSet<long>();ϸ.Enqueue(С);while(ϸ.Count>0){int
Г=0;while(ϸ.Count>0&&Г<ϻ){var Д=ϸ.Dequeue();long Е=Д.EntityId;if(Ϲ.Contains(Е)){Г++;continue;}Ϲ.Add(Е);if(!ά.Contains(Е))
Т.Add(Е);HashSet<long>Ж;if(Ϻ.TryGetValue(Е,out Ж)){foreach(long З in Ж){if(ά.Contains(З))continue;var И=Ј(З);if(И!=null&&
!Ϲ.Contains(З))ϸ.Enqueue(И);}}Г++;}yield return 0;}if(Т.Count>0){foreach(var Е in Т)ά.Add(Е);foreach(var ι in А(Т))yield
return ι;}ϊ();η=false;yield return 0;}IEnumerable<double>П(){Ͼ();var У=new HashSet<long>();foreach(var ι in Ͽ(A.ɬ,У))yield
return ι;var Ф=new HashSet<long>(ά);Ф.ExceptWith(У);ά=У;if(Ф.Count>0)В(Ф);ϊ();η=false;yield return 0;}IEnumerable<double>Л(){Ͼ
();var Х=new HashSet<long>();foreach(var ι in Ͽ(A.ɬ,Х))yield return ι;var Ц=new HashSet<long>(Х);Ц.ExceptWith(ά);var Ф=
new HashSet<long>(ά);Ф.ExceptWith(Х);ά=Х;if(Ф.Count>0)В(Ф);if(Ц.Count>0){foreach(var ι in А(Ц))yield return ι;}ϊ();η=false;
yield return 0;}}public class Ϙ:Ô{}public interface Έ{object Ι(IMyTerminalBlock l);bool Μ(IMyTerminalBlock l,object Κ);void Ξ
(IMyTerminalBlock l);}public class ʆ:ɵ{class Ъ{public double Ч;public double Ш;public Action Щ;}List<Ъ>Ы=new
List<Ъ>();List<Ъ>Ь=new List<Ъ>();class Я{public IEnumerator<double>Э;public double Ю;}List<Я>а=new List<Я>
();MyGridProgram б;bool в=true;public ʆ(A B):base(B){б=B.б;б.Runtime.UpdateFrequency=UpdateFrequency.Update10;}
public void ƅ(){а.Clear();Ы.Clear();Ь.Clear();}public override void o(){ʇ(г,1);}public void ʇ(Action д,double ŭ=0){Ы.Add(new Ъ
{Щ=д,Ч=ŭ,Ш=ŭ});}public void е()=>Ы.Clear();public void з(Action д,double ж){Ь.Add(new Ъ{Щ=д,Ч=ж,Ш=ж});}public void λ(
IEnumerable<double>и,double ж=0){а.Add(new Я{Э=и.GetEnumerator(),Ю=ж});}public override void b(){double й=б.Runtime.
TimeSinceLastRun.TotalSeconds;foreach(var д in Ы){д.Ш-=й;if(д.Ш<=0){д.Щ.Invoke();д.Ш=д.Ч;}}for(int Ё=Ь.Count-1;Ё>=0;Ё--){var д=Ь[Ё];д.Ш
-=й;if(д.Ш<=0){д.Щ.Invoke();Ь.RemoveAt(Ё);}}for(int Ё=а.Count-1;Ё>=0;Ё--){var к=а[Ё];к.Ю-=й;if(к.Ю<=0){if(к.Э.MoveNext())к
.Ю=к.Э.Current;else{к.Э.Dispose();а.RemoveAt(Ё);}}}}public int л{get{return Ь.Count;}}public int м{get{return а.Count;}}
void г()=>в=!в;public string н()=>в?"/":"\\";}public static class Î{public const string Ï="Block not found: {0}";public
const string о="Invalid argument for block: {0}";public const string Ȳ="Invalid command option: {0}";public const string Į=
"Block updated: {0} -> {1}";public const string ē="Block resetting: {0}";public const string Ħ="Block moving: {0}";public const string Ǣ=
"Block started: {0}";public const string ƒ="Block stopped: {0}";public const string Đ="Block locked: {0}";public const string ı=
"Block unlocked: {0}";public const string Ò="Block open: {0}";public const string â="Block charging: {0}";public const string ã=
"Block discharging: {0}";public const string á="Block auto: {0}";public const string Ǻ="Block on: {0}";public const string Ƿ="Block off: {0}";
public const string Ð="Block closed: {0}";public const string Ó="Block toggled: {0}";public const string Ǳ=
"Block stockpiling: {0}";public const string Ǯ="Block sharing: {0}";public const string ȋ="Block action: {0} -> {1}";public const string ƣ=
"Block: {0} -> {1}";}public class Æ:ɵ{public static class Ç{public const string п="Command not found: {0}";public const string È=
"No arguments provided";public const string ħ="Invalid command format.";}ʆ ʆ;Ș Ș;ɱ р;public List<ɓ>с=new List<ɓ>();public Dictionary<long,HashSet<string>>т=new Dictionary<long,HashSet<string>>();public Dictionary<long,HashSet<string>>у=new
Dictionary<long,HashSet<string>>();public Æ(A B):base(B){}public override void o(){ʆ=A.e<ʆ>();Ș=A.e<Ș>();р=A.e<ɱ>();т.Clear();у.
Clear();f(new ф(this));Ϊ("command",х=>ц(х));Ϊ("localcmd",х=>ч(х));}Φ ц(Υ х){if(!р.ш)return null;string ъ=х.щ("Command").Trim(
);if(string.IsNullOrEmpty(ъ))return р.ы(х,Φ.ь.э);ю("REQ",х.я("OriginName"),ъ);var ѐ=new Ã(ъ);long ђ=ё(ѐ.Á);if(ђ==0)ђ=ѓ(ѐ.
Á);if(ђ!=0){р.є(ђ,ъ);return р.ы(х,Φ.ь.ѕ);}return і(х,ъ);}Φ ч(Υ х){string Ä=х.щ("Command").Trim();if(string.IsNullOrEmpty(
Ä))return р.ы(х,Φ.ь.э);ю("CREQ",х.я("OriginName"),Ä);return і(х,Ä);}Φ і(Υ х,string Ä){bool ї=Ϭ(Ä);var r=ї?Φ.ь.ѕ:Φ.ь.э;
return р.ы(х,r);}void ю(string ј,string љ,string Ä){Ș.ș($"{ј}: {љ}> {Ä}");A.ϙ($"{ј}: {љ}> {Ä}",false);}new public void f(ɓ Ä){
с.Add(Ä);}public bool Ϭ(string њ){if(њ.Length>0){њ=A.ћ(њ);ќ(new ѝ(њ));return true;}return false;}void ќ(ѝ ў){var Ѡ=ў.џ;if
(ʆ==null)ʆ=A.e<ʆ>();if(Ѡ=="self"||string.IsNullOrEmpty(Ѡ)){ѡ(ў);}else{ў.Ѣ(A.ѣ);var ѥ=$"> @{Ѡ} {ў.Ѥ}";if(Ѡ=="*")р.Ѧ(ў);
else р.ѧ(Ѡ,ў);Ș.ș(ѥ);A.ϙ(ѥ);}}void ѡ(ѝ и){if(и.Ѩ){foreach(var ϰ in и.ѩ)ʆ.λ(Ѫ(ϰ));}else{ʆ.λ(Ѫ(и.Ύ));}}IEnumerable<double>Ѫ(
List<Ã>ѫ){foreach(var Ä in ѫ){foreach(double ѭ in Ѭ(Ä))yield return ѭ;}}IEnumerable<double>Ѭ(Ã Ä){if(Ä.Á.ToLower()=="wait"&&
Ä.Å.Count>0){double ж;if(double.TryParse(Ä.Å[0],out ж)){A.ϙ($"> wait {ж}");yield return ж;}yield break;}string ѯ=Ѯ(Ä);if(
ѯ!=null){string Ѱ=A.ћ(ѯ);var и=new ѝ(Ѱ);if(и.Ѩ){foreach(var ϰ in и.ѩ)ʆ.λ(Ѫ(ϰ));}else{foreach(double ѭ in Ѫ(и.Ύ))yield
return ѭ;}yield break;}ѱ(Ä);yield return 0;}string Ѯ(Ã Ä){string њ=Ä.Ѳ;if(њ.StartsWith("_")){string ѳ=њ.Substring(1);if(A.ѣ.
ContainsKey(ѳ)){A.ϙ($"Executing local command: {ѳ}",false);return A.Ѵ(A.ѣ[ѳ],Ä.ě);}}if(!Ä.ѵ&&ё(Ä.Á)!=0)return null;if(A.ѣ.
ContainsKey(Ä.Á))return A.Ѵ(A.ѣ[Ä.Á],Ä.ě);string Ѷ="!"+Ä.Á;if(A.ѣ.ContainsKey(Ѷ))return A.Ѵ(A.ѣ[Ѷ],Ä.ě);return null;}void ѱ(Ã Ä){
string њ=Ä.Ѳ;if(!Ä.ѵ){long ѷ=ё(Ä.Á);if(ѷ!=0){р.є(ѷ,њ);return;}}foreach(ɓ Ѹ in с){if(Ѹ.ɔ()==Ä.Á){var ѥ="> "+њ;A.ϙ(ѥ);string ȁ=Ѹ
.Ñ(Ä);A.ϙ(ȁ,false);return;}}if(!Ä.ѵ){long ѹ=ѓ(Ä.Á);if(ѹ!=0){р.є(ѹ,њ);return;}}A.ϙ(Ì.Í(Ç.п,Ä.Ѳ),false);}public List<string
>ѻ(){var Ѻ=new List<string>(с.Count+A.ѣ.Count);for(int Ё=0;Ё<с.Count;Ё++)Ѻ.Add(с[Ё].ɔ());foreach(var ɦ in A.ѣ.Keys)Ѻ.Add(
ɦ);return Ѻ;}public void Ҁ(long Ѽ,List<string>ѫ){if(Ѽ==A.ʋ)return;var ѽ=new HashSet<string>();var Ѿ=new HashSet<string>()
;ѫ.ForEach(ѿ=>{if(ѿ.StartsWith("!"))Ѿ.Add(ѿ.Substring(1));else ѽ.Add(ѿ);});т[Ѽ]=ѽ;у[Ѽ]=Ѿ;}public long ѓ(string Ų){foreach
(var ɽ in т){if(ɽ.Value.Contains(Ų))return ɽ.Key;}return 0;}public long ё(string Ų){foreach(var ɽ in у)if(ɽ.Value.
Contains(Ų))return ɽ.Key;return 0;}}public class ф:º{Æ À;public ф(Æ Â){À=Â;}public override string Á=>"help";public
override string Ñ(Ã Ä){var ҁ=new StringBuilder();À.Ύ.ForEach(Ѹ=>{ҁ.Append(Ѹ.ɔ()).Append('\n');});return$"{ҁ}";}}public class Ã{
public string Ѳ;public string Á;public List<string>Å=new List<string>();public Dictionary<string,string>ě=new Dictionary<
string,string>();public bool Ҋ=false;public bool ҋ=false;public bool ѵ=false;public Ã(string њ){Ѳ=њ.Replace("\r","").Trim();Ҍ(
);}public string Ė(string ɦ){if(ě.ContainsKey(ɦ))return ě[ɦ];return"";}public static bool Ǘ(string ȏ){return ȏ?.Trim().
ToLower()=="true"||ȏ?.Trim()=="1";}void Ҍ(){foreach(string Ҏ in ҍ(Ѳ)){if(Ҏ.StartsWith("--")){string[]Ȝ=Ҏ.Split('=');string ɦ=Ȝ[
0].Substring(2);if(Ȝ.Length==2){ě.Add(ɦ,Ȝ[1]);}else{ě.Add(ɦ,"true");}}else{Å.Add(Ҏ);}}Á=Å[0];if(Á.StartsWith("!!")){ѵ=
true;Á=Á.Substring(2);}Å.RemoveAt(0);}public static List<string>ҍ(string ҏ){var Ґ=new List<string>();int Ё=0;while(Ё<ҏ.
Length){if(ҏ[Ё]==' '){Ё++;continue;}if(ҏ[Ё]=='"'){int ґ=ҏ.IndexOf('"',Ё+1);if(ґ==-1)ґ=ҏ.Length;Ґ.Add(ҏ.Substring(Ё+1,ґ-Ё-1));Ё
=ґ+1;}else{int ґ=ҏ.IndexOf(' ',Ё);if(ґ==-1)ґ=ҏ.Length;Ґ.Add(ҏ.Substring(Ё,ґ-Ё));Ё=ґ+1;}}return Ґ;}}public class ѝ{string
Ғ;public string џ="self";public string Ѥ="";public List<Ã>Ύ=new List<Ã>();public List<List<Ã>>ѩ=new List<List<Ã>>();
public bool Ѩ=>ѩ.Count>0;List<Ã>ғ=new List<Ã>();public ѝ(string и){Ғ=и.Trim();Ҕ();}void җ(string Ä){string[]ҕ=Ä.Split(' ');
string Җ=ҕ[0];if(Җ.StartsWith("@")){џ=Җ.Substring(1);Ғ=Ä.Substring(Җ.Length);}if(Җ=="*"){џ=Җ;Ғ=Ä.Substring(1);}}void Ҕ(){җ(Ғ);
if(Ҙ(Ғ)){ҙ(Ғ);}else{List<string>қ=Қ(Ғ);foreach(var њ in қ){if(!string.IsNullOrWhiteSpace(њ))Ύ.Add(new Ã(њ.Trim()));}}}
static bool Ҙ(string и){if(!и.Contains("{")||!и.Contains("}"))return false;int Ҝ=0;foreach(char î in и){if(î=='{')Ҝ++;else if(
î=='}')Ҝ--;else if(Ҝ==0&&!char.IsWhiteSpace(î))return false;}return true;}void ҙ(string и){List<Ã>ҝ=null;StringBuilder Ҟ=
new StringBuilder();bool ҟ=false;int Ҝ=0;for(int Ё=0;Ё<и.Length;Ё++){char î=и[Ё];if(î=='"')ҟ=!ҟ;if(!ҟ){if(î=='{'){Ҝ++;if(Ҝ
==1){ҝ=new List<Ã>();Ҟ.Clear();continue;}}else if(î=='}'){Ҝ--;if(Ҝ==0){string ѿ=Ҟ.ToString().Trim();if(!string.
IsNullOrWhiteSpace(ѿ))ҝ.Add(new Ã(ѿ));if(ҝ.Count>0)ѩ.Add(ҝ);ҝ=null;Ҟ.Clear();continue;}}else if(î==';'&&Ҝ==1){string ѿ=Ҟ.ToString().Trim()
;if(!string.IsNullOrWhiteSpace(ѿ))ҝ.Add(new Ã(ѿ));Ҟ.Clear();continue;}}if(Ҝ>0)Ҟ.Append(î);}}public ѝ Ѣ(Dictionary<string,
string>Ҡ){foreach(Ã Ä in Ύ){string Ң=ҡ(Ä,Ҡ);ғ.Add(new Ã(Ң));}var ҁ=new StringBuilder();foreach(Ã Ä in ғ)ҁ.Append(Ä.Ѳ).Append(
';');Ѥ=ҁ.ToString();return this;}static string ҡ(Ã Ä,Dictionary<string,string>Ҡ){string њ=Ä.Ѳ;Dictionary<string,string>Ҥ=ң(
Ҡ);њ=ҥ(њ,Ҥ);њ=Ҧ(њ);return њ;}List<string>Қ(string и){bool ҟ=false;int Ҝ=0;List<string>ѫ=new List<string>();StringBuilder
Ҟ=new StringBuilder();foreach(char î in и){if(î=='"')ҟ=!ҟ;if(!ҟ){if(î=='{')Ҝ++;else if(î=='}')Ҝ--;}if(î==';'&&!ҟ&&Ҝ==0){ѫ
.Add(Ҟ.ToString().Trim());Ҟ.Clear();}else{Ҟ.Append(î);}}if(Ҟ.Length>0)ѫ.Add(Ҟ.ToString().Trim());return ѫ;}static
Dictionary<string,string>ң(Dictionary<string,string>Ҡ){Dictionary<string,string>Ҥ=new Dictionary<string,string>();var Ҩ=Ҡ.Keys.
OrderByDescending(ҧ=>ҧ.Length).ToList();Ҩ.ForEach(ɦ=>{Ҥ[ɦ]=ҩ(Ҡ[ɦ],Ҥ);});return Ҥ;}static string ҩ(string ȏ,Dictionary<string,string>Ҥ){
StringBuilder ҁ=new StringBuilder(ȏ);bool Ҫ;do{Ҫ=false;foreach(var ɽ in Ҥ){string ɦ=ɽ.Key;string ҫ=ɽ.Value;if(Ҭ(ҁ.ToString(),ɦ)){ҁ.
Replace(ɦ,ҫ);Ҫ=true;}}}while(Ҫ);return$"{ҁ}";}static string ҥ(string Ä,Dictionary<string,string>Ҥ){StringBuilder ҁ=new
StringBuilder(Ä);foreach(var ɽ in Ҥ){if(Ҭ($"{ҁ}",ɽ.Key))ҁ.Replace(ɽ.Key,ɽ.Value);}return$"{ҁ}";}static bool Ҭ(string ҭ,string Ү){
return System.Text.RegularExpressions.Regex.IsMatch(ҭ,$@"\b{Ү}\b");}static string Ҧ(string Ä){while(Ä.Contains(";;"))Ä=Ä.
Replace(";;",";");return Ä.Trim(';');}}public class ү:º{ɭ À;public override string Á=>"var/set";public ү(ɭ Â){À=Â;}
public override string Ñ(Ã Ä){if(Ä.Å.Count<2)return Æ.Ç.È;string ǎ=Ä.Å[0];string ȏ=Ä.Å[1];bool Ұ=Ä.ě.ContainsKey("save");À.ұ(ǎ
,ȏ,Ұ);return Ұ?$"${ǎ} = \"{ȏ}\" (saved)":$"${ǎ} = \"{ȏ}\"";}}public class Ҳ:Ô{}public class ρ:Ô{}public class Ҵ{MyIni ҳ;public Ҵ(MyIni ɮ){ҳ=ɮ;}public MyIni b(){ҵ();return ҳ;}void ҵ(){if(ҳ.ContainsSection("Commands")){string Ҷ=ҳ.ToString(
);Ҷ=Ҷ.Replace("[Commands]","[commands]");ҳ.TryParse(Ҷ);}if(!ҳ.ContainsSection("channels")){string ҷ=ҳ.Get("security",
"passcodes").ToString();ҳ.Set("channels","default",ҷ);if(ҳ.ContainsSection("security"))ҳ.DeleteSection("security");}}}public class
ɭ:ɵ{Dictionary<string,string>Ҹ=new Dictionary<string,string>(){};public Dictionary<string,string>ϝ=new
Dictionary<string,string>(){};string[]ҹ=new string[]{"general","channels","variables","commands","hooks",};public
MyIni ϩ=new MyIni();public MyIni ɯ=>ϩ;public ɭ(A B):base(B){}void Һ(){MyIniParseResult ϓ;if(!ϩ.TryParse(A.ɰ.CustomData,
out ϓ))throw new Exception($"{ϓ}");}public override void o(){һ();A.ɰ.CustomData=$"{new Ҵ(ϩ).b()}";A.e<Π>()?.Ρ<ρ>(this);f(
new ү(this));}public void Ҽ(){һ();}void һ(){Һ();Ϛ();ҽ();Ҿ();A.ҿ=ʕ("general.debug").ToLower()=="true";var Ӏ=ʕ("general.name"
);A.Á=!string.IsNullOrEmpty(Ӏ)?Ӂ(Ӏ):A.ɬ.CustomName;}public override void Γ(Ô Α,object Β){if(Α is ρ)Ҽ();}public string ʕ(
string ӂ){List<string>Ӄ=new List<string>(ӂ.Split('.'));if(Ӄ.Count!=2)return$"";else{string ӄ=Ӄ[0];string ɦ=Ӄ[1];return
$"{ϩ.Get(ӄ,ɦ)}";}}string Ӂ(string ҏ){if(ҏ.Length>=2&&ҏ[0]=='"'&&ҏ[ҏ.Length-1]=='"')return ҏ.Substring(1,ҏ.Length-2);return ҏ;}void ҽ(){
A.Ӆ.Clear();var ӆ="variables";List<MyIniKey>ϡ=new List<MyIniKey>();ϩ.GetKeys(ӆ,ϡ);foreach(var ɦ in ϡ){string Ӈ=ɦ.Name;
string ӈ=$"{ϩ.Get(ӆ,Ӈ)}";if(Ӈ.StartsWith("$"))Ӈ=Ӈ.Substring(1);ӈ=Ӂ(ӈ);A.Ӆ[Ӈ]=ӈ;}}void Ҿ(){A.ѣ.Clear();List<MyIniKey>ϡ=new List
<MyIniKey>();ϩ.GetKeys("Commands",ϡ);foreach(var ɦ in ϡ){string Ų=ɦ.Name;string Ӊ=$"{ϩ.Get("Commands",Ų)}".Replace("\r",
"").Replace("\n"," ").Trim();Ӊ=System.Text.RegularExpressions.Regex.Replace(Ӊ,@"\s+"," ");Ӊ=Ӂ(Ӊ);A.ѣ[Ų]=Ӊ;}}public void ұ(
string ǎ,string ȏ,bool Ұ=false){A.Ӆ[ǎ]=ȏ;if(Ұ){ϩ.Set("variables",ǎ,ȏ);A.ɰ.CustomData=$"{ϩ}";}}void Ϛ(){foreach(string ӄ in ҹ)
if(!ϩ.ContainsSection(ӄ))ϩ.AddSection(ӄ);foreach(KeyValuePair<string,string>ӊ in Ҹ){string[]Ϟ=ӊ.Key.Split('.');string ӄ=Ϟ[
0];string ɦ=Ϟ[1];string ȏ=ӊ.Value;if(ϩ.Get(ӄ,ɦ).IsEmpty)ϩ.Set(ӄ,ɦ,ȏ);}A.ɰ.CustomData=$"{ϩ}";}}public class ӌ:º{Ӌ
À;public override string Á=>"connector/lock";public ӌ(Ӌ Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.
ħ;else{string Ӎ=Ä.Å[0];List<IMyShipConnector>ӎ=À.Ê<IMyShipConnector>(Ӎ);if(ӎ.Count==0)return Ì.Í(Î.Ï,Ӎ);ӎ.ForEach(k=>À.ӏ(
k));return Ì.Í(Î.Đ,Ӎ);}}}public class Ӑ:º{Ӌ À;public override string Á=>"connector/toggle";public Ӑ(Ӌ Â){À=Â;}
public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string Ӎ=Ä.Å[0];List<IMyShipConnector>ӎ=À.Ê<IMyShipConnector>(
Ӎ);if(ӎ.Count==0)return Ì.Í(Î.Ï,Ӎ);ӎ.ForEach(k=>À.ӑ(k));return Ì.Í(Î.Ó,Ӎ);}}}public class Ӓ:º{Ӌ À;public
override string Á=>"connector/unlock";public Ӓ(Ӌ Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else{string
Ӎ=Ä.Å[0];List<IMyShipConnector>ӎ=À.Ê<IMyShipConnector>(Ӎ);if(ӎ.Count==0)return Ì.Í(Î.Ï,Ӎ);ӎ.ForEach(k=>À.ӓ(k));return Ì.Í
(Î.ı,Ӎ);}}}public class Ӌ:ɵ{d d;public Ӌ(A B):base(B){}public override void o(){d=A.e<d>();f(new ӌ(this));f(new Ӓ(this));
f(new Ӑ(this));j<IMyShipConnector>(k=>k.Status,(l,m)=>Ӕ(l as IMyShipConnector,m));}protected void Ӕ(IMyShipConnector k,
object q){var r=q as MyShipConnectorStatus?;var ӕ=Ό.ContainsKey(k.EntityId)?Ό[k.EntityId]as MyShipConnectorStatus?:null;if(r==
MyShipConnectorStatus.Connected){t<ν>(k);d.u(k,"onLock");}else if((r==MyShipConnectorStatus.Connectable&&ӕ==MyShipConnectorStatus.Connected)
||r==MyShipConnectorStatus.Unconnected){t<ξ>(k);d.u(k,"onUnlock");}else if(r==MyShipConnectorStatus.Connectable){t<Ӗ>(k);d
.u(k,"onReady");}}public void ӏ(IMyShipConnector k){k.Connect();}public void ӓ(IMyShipConnector k){k.Disconnect();}public
void ӑ(IMyShipConnector k){if(k.Status==MyShipConnectorStatus.Connected)ӓ(k);else ӏ(k);}}public class ν:Ô{}public class Ӗ:Ô{
}public class ξ:Ô{}public class Ǐ:ɵ{public const string ӗ="LogView";ʆ ʆ;Ș Ș;d d;HashSet<IMyTextSurface>Ә=new
HashSet<IMyTextSurface>();List<IMyTextSurface>ә=new List<IMyTextSurface>();Dictionary<string,List<
IMyTextSurface>>Ӛ=new Dictionary<string,List<IMyTextSurface>>();public static float ӛ=1;public Ǐ(A B):base(B){}public override void o(
){ʆ=A.e<ʆ>();Ș=A.e<Ș>();d=A.e<d>();Ρ<Ϙ>();Ӝ();}public override void Γ(Ô Α,object Β){if(Α is Ϙ&&(Β is IMyTextPanel||Β is
IMyTextSurfaceProvider)){Ӝ();}}string Ӡ(string ӝ=""){return$" {A.C} - {ӝ}     ({ʆ.н()})\n"+
$" {A.Á} *{A.Ӟ}                                  {ӟ()}\n"+"------------------------------------------------------";}public string ӟ(){string ӡ=A.e<ĸ>().ɷ.Count()>0?"M":"   ";
string Ӣ=A.e<ɱ>().Ӣ.Count()>0?"C":"    ";string ӣ=$"{A.e<ɡ>()?.ʄ.Count()??0}";string ӥ=A.Ӥ?"A":"   ";string Ӧ=ʆ.л>0?"W":"   ";
return String.Join("  ",Ӧ,ӥ,Ӣ,ӡ,ӣ);}public List<IMyTextSurface>ǅ(string ǎ){var ӧ=ǎ.Split(':');var Ө=new List<IMyTextSurface>()
;if(ӧ.Length>1){string Ć=ӧ[0].Trim();int ө=int.Parse(ӧ[1].Trim());Ө=d.Ê<IMyTerminalBlock>(Ć).Where(l=>l is
IMyTextSurfaceProvider).Select(l=>((IMyTextSurfaceProvider)l).GetSurface(ө)).Where(Ӫ=>Ӫ!=null).ToList();}else{Ө=d.Ê<IMyTerminalBlock>(ǎ).Where
(l=>l is IMyTextSurface).Select(l=>(IMyTextSurface)l).ToList();}return Ө;}void ӭ(){d.ì<IMyTextPanel>()?.ForEach(ӫ=>Ӭ(ӫ));
}void Ӭ(IMyTextPanel ӫ){MyIni ɮ=d.ό(ӫ);foreach(Ӯ ɽ in ӯ.Ӱ(ɮ)){if(ɽ.ӱ!=0)continue;Ә.Add(ӫ);if(string.Equals(ɽ.Ӳ,ӗ,
StringComparison.OrdinalIgnoreCase)&&ӯ.ӳ(ɽ.Ӵ,A)){ӫ.ContentType=ContentType.TEXT_AND_IMAGE;ә.Add(ӫ);}ӵ(ɽ.Ӳ,ӫ);break;}}void ӷ(){var Ӷ=new
List<IMyTerminalBlock>();Ӷ.AddRange(d.ì<IMyCockpit>());Ӷ.AddRange(d.ì<IMyProgrammableBlock>());Ӷ.AddRange(d.ì<IMySoundBlock>
());Ӷ.ForEach(l=>ӷ(l));}void ӷ(IMyTerminalBlock l){IMyTextSurfaceProvider Ӹ=l as IMyTextSurfaceProvider;if(Ӹ==null)return
;MyIni ɮ=d.ό(l);foreach(Ӯ ɽ in ӯ.Ӱ(ɮ)){if(ɽ.ӱ>=Ӹ.SurfaceCount)continue;IMyTextSurface Ӫ=Ӹ.GetSurface(ɽ.ӱ);Ә.Add(Ӫ);if(
string.Equals(ɽ.Ӳ,ӗ,StringComparison.OrdinalIgnoreCase)&&ӯ.ӳ(ɽ.Ӵ,A)){Ӫ.ContentType=ContentType.TEXT_AND_IMAGE;ә.Add(Ӫ);}ӵ(ɽ.Ӳ,
Ӫ);}}public void Ӝ(){ӹ();ӭ();ӷ();}void ӹ(){Ә.Clear();ә.Clear();foreach(var Ӻ in Ӛ.Values)Ӻ.Clear();}public void Ӽ(){ӻ();}
string Ӿ(){string ӽ=string.Join("\n",Ș.ʄ);return Ӡ("LOG")+"\n"+ӽ;}void ӻ(){string ӽ=Ӿ();ә.ForEach(Ӫ=>Ӫ.WriteText($"{ӽ}",false)
);}public List<IMyTextSurface>Ԁ(string ӿ){string ɦ=ӿ.ToLower();if(Ӛ.ContainsKey(ɦ))return Ӛ[ɦ];return new List<
IMyTextSurface>();}void ӵ(string ӿ,IMyTextSurface Ӫ){string ɦ=ӿ.ToLower();if(!Ӛ.ContainsKey(ɦ))Ӛ[ɦ]=new List<IMyTextSurface>();Ӛ[ɦ].
Add(Ӫ);}}public struct Ӯ{public int ӱ;public string Ӳ;public string Ӵ;}public static class ӯ{const string ԁ="surfaces";
public static List<Ӯ>Ӱ(MyIni Ԃ){var ԃ=new List<Ӯ>();var ϡ=new List<MyIniKey>();Ԃ.GetKeys(ԁ,ϡ);foreach(MyIniKey ɦ in ϡ){int Ϩ;
if(!int.TryParse(ɦ.Name,out Ϩ))continue;string Ԅ=Ԃ.Get(ԁ,ɦ.Name).ToString().Trim();if(string.IsNullOrEmpty(Ԅ))continue;
List<string>Ґ=Ã.ҍ(Ԅ);string ӿ=Ґ[0];string Ȗ=Ґ.Count>1?Ґ[1]:null;ԃ.Add(new Ӯ{ӱ=Ϩ,Ӳ=ӿ,Ӵ=Ȗ});}return ԃ;}public static bool ӳ(
string ԅ,A B){if(string.IsNullOrEmpty(ԅ))return true;return string.Equals(ԅ,B.C,StringComparison.OrdinalIgnoreCase)||string.
Equals(ԅ,B.Ӟ,StringComparison.OrdinalIgnoreCase);}}public class Π:ɵ{public Π(A B):base(B){}Dictionary<Type,
HashSet<Ά>>Ԇ=new Dictionary<Type,HashSet<Ά>>();public void Ρ<Ο>(Ά Â)where Ο:Ô{var ԇ=typeof(Ο);if(!Ԇ.ContainsKey(ԇ))Ԇ[ԇ]=new
HashSet<Ά>();Ԇ[ԇ].Add(Â);}public bool Ԉ<Ο>(Ά Â){var ԇ=typeof(Ο);if(Ԇ.ContainsKey(ԇ))return Ԇ[ԇ].Contains(Â);return false;}
public void ԉ<Ο>(Ά Â)where Ο:Ô{var ԇ=typeof(Ο);if(Ԇ.ContainsKey(ԇ))Ԇ[ԇ].Remove(Â);}public new void t<Ο>(object Β=null)where Ο:
Ô,new(){t(new Ο(),Β);}public new void t(Ô Α,object Β=null){var ԇ=Α.GetType();if(Ԇ.ContainsKey(ԇ))Ԇ[ԇ].ToList().ForEach(Â
=>Â.Γ(Α,Β));}}public interface Ô{}public interface Ϋ:Ά{}public interface E:Ά{}public interface Ά{void o();IEnumerator<
double>ΐ();void b();void Γ(Ô Α,object Β);string Δ();List<ɓ>Σ();}public class Ԋ:º{A A;public override string Á=>"ping"
;public Ԋ(A B){A=B;}public override string Ñ(Ã Ä){A.e<ɱ>().ɴ();return"Pinging all grids";}}public class ԋ:Ô{}public class
Ԍ:Ô{}public class ԍ:Ô{}public abstract class ԓ{public Dictionary<string,object>Ԏ=new Dictionary<string,object>();public
Dictionary<string,object>ԏ=new Dictionary<string,object>();public HashSet<string>ʫ=new HashSet<string>();public string ʋ{get;}=Ԑ()
;public ԓ(Dictionary<string,object>ԑ,Dictionary<string,object>Ԓ){ԏ=ԑ;Ԏ=Ԓ;if(!Ԏ.ContainsKey("Id"))Ԏ["Id"]=Ԑ();}public
object Ԕ(string ɦ){object ȏ;return ԏ.TryGetValue(ɦ,out ȏ)?ȏ??"":"";}public string щ(string ɦ)=>$"{Ԕ(ɦ)}";public float ԕ(string
ɦ){float ȏ;float.TryParse(щ(ɦ),out ȏ);return ȏ;}public double Ԗ(string ɦ){double ȏ;double.TryParse(щ(ɦ),out ȏ);return ȏ;}
public object ԗ(string ɦ){object ȏ;return Ԏ.TryGetValue(ɦ,out ȏ)?ȏ??"":"";}public string я(string ɦ)=>$"{ԗ(ɦ)}";public float Ԙ
(string ɦ){float ȏ;float.TryParse(я(ɦ),out ȏ);return ȏ;}public double ԙ(string ɦ){double ȏ;double.TryParse(я(ɦ),out ȏ);
return ȏ;}public long Ԛ(string ɦ){long ȏ;long.TryParse(я(ɦ),out ȏ);return ȏ;}static string Ԑ(){long ԛ=DateTime.UtcNow.Ticks;
int Ԝ=new Random().Next(0,1000);return$"{ԛ}_{Ԝ}";}public virtual string ˌ(){string ԝ="header";string Ԟ="body";string ԟ=ʘ.ʥ(
Ԏ);string Ԡ=ʘ.ʥ(ԏ);return$"<{ԝ}>{ԟ}</{ԝ}>"+$"<{Ԟ}>{Ԡ}</{Ԟ}>";}public static string ԥ(string ǂ,string ԡ){string Ԣ=$"<{ԡ}>"
;string ԣ=$"</{ԡ}>";int ϧ=ǂ.IndexOf(Ԣ)+Ԣ.Length;int Ԥ=ǂ.IndexOf(ԣ);if(ϧ==-1||Ԥ==-1||ϧ>=Ԥ)return"";return ǂ.Substring(ϧ,Ԥ-
ϧ).Trim();}}public class ɱ:ɵ{class Ç{public const string Ԧ="Cannot de-serialize message.";public const string ԧ=
"No active request found for RespondingToId: {0}";}ʆ ʆ;Ș Ș;ɡ ɡ;Π Π;public Ψ Ψ;public Dictionary<string,Action<ԓ>>Ӣ=new Dictionary<string,Action<ԓ>>();const
string Ա=".construct";IMyUnicastListener Բ;List<IMyBroadcastListener>Գ=new List<IMyBroadcastListener>();public
Dictionary<string,string>ʫ=new Dictionary<string,string>();long Դ=0;public bool ш=>Ե()==A.ʋ;public ɱ(A B):base(B){Ψ=new Ψ();}
public override void o(){ʆ=A.e<ʆ>();Ș=A.e<Ș>();ɡ=A.e<ɡ>();Π=A.e<Π>();A.f(new Ԋ(A));Զ();Է();Π.Ρ<ρ>(this);Ψ.Ω("ping",х=>ы(х,Φ.ь.
Ը));Ψ.Ω("sync",х=>Թ(х));Ψ.Ω("almanac",х=>Ժ(х));ʆ.з(()=>ɳ(),0.5);ʆ.ʇ(ɳ,5);ʆ.ʇ(ɴ,2);}Φ Թ(Υ х){Ի(х);var Լ=A.e<Æ>().ѻ();var Խ
=ы(х,Φ.ь.Ը,new Dictionary<string,object>{{"Commands",string.Join(",",Լ)}});Խ.ʫ.Add(Ա);return Խ;}void Զ(){ʫ.Clear();var ɮ=
A.e<ɭ>();var ϡ=new List<MyIniKey>();ɮ.ɯ.GetKeys("channels",ϡ);ϡ.ForEach(ɦ=>{var ȏ=ɮ.ɯ.Get(ɦ.Section,ɦ.Name);ʫ[ɦ.Name]=
$"{ȏ}";});}void Է(){Բ=A.Ծ.UnicastListener;Բ.SetMessageCallback();IMyBroadcastListener Կ=A.Ծ.RegisterBroadcastListener(Ա);Կ.
SetMessageCallback();Գ.Add(Կ);foreach(var Հ in ʫ){IMyBroadcastListener Ձ=A.Ծ.RegisterBroadcastListener(Հ.Key);Ձ.SetMessageCallback();Գ.Add
(Ձ);}}public override void Γ(Ô Α,object Β){if(Α is ρ){Զ();Է();ʆ.з(ɳ,0);}}public void Մ(){while(Բ?.HasPendingMessage==true
)Ղ(Բ.AcceptMessage());Գ.ForEach(Ճ=>{while(Ճ?.HasPendingMessage==true)Ղ(Ճ.AcceptMessage());});Ӣ.Clear();}string Պ(
MyIGCMessage ǂ){string Յ=$"{ǂ.Data}";string Հ=ǂ.Tag;string Շ=Ն(Հ);if(Շ=="")return Յ;else return Ո.Չ(Յ,Շ);}public void Ղ(MyIGCMessage
ǂ){string Յ=$"{ǂ.Data}";Յ=Ո.Ջ(Յ)?Պ(ǂ):Յ;if(Յ.StartsWith("REQUEST::")){Ռ(Υ.Ս(Յ),ǂ.Tag,Վ=>Տ((Υ)Վ));}else if(Յ.StartsWith(
"RESPONSE::")){Ռ(Φ.Ս(Յ),ǂ.Tag,Վ=>Ր((Φ)Վ));}}void Ռ(ԓ Ց,string Ւ,Action<ԓ>ω){if(Ց!=null){Ց.ʫ.Add(Ւ);bool Փ=Ւ!=Ա;if(Փ){Ք(Ց);Օ(Ց);}ω(Ց)
;}else{Ș.Ֆ(Ç.Ԧ);}}ʃ Ք(ԓ ǂ){long ʧ=ǂ.Ԛ("OriginId");string ՙ=ǂ.я("GridId");string ʦ=!string.IsNullOrEmpty(ՙ)?ՙ:$"{ʧ}";
Vector3D?ʐ=null;Vector3D?ʑ=null;double ˬ,ˮ,Ͱ,ͱ,Ͳ,ͳ;if(double.TryParse(ǂ.я("Fx"),out ˬ)&&double.TryParse(ǂ.я("Fy"),out ˮ)&&double
.TryParse(ǂ.я("Fz"),out Ͱ)){ʐ=new Vector3D(ˬ,ˮ,Ͱ);}if(double.TryParse(ǂ.я("Ux"),out ͱ)&&double.TryParse(ǂ.я("Uy"),out Ͳ)
&&double.TryParse(ǂ.я("Uz"),out ͳ)){ʑ=new Vector3D(ͱ,Ͳ,ͳ);}return ɡ.ʴ(ʦ,ʧ,ǂ.я("OriginName"),new Vector3D(ǂ.Ԙ("X"),ǂ.Ԙ("Y")
,ǂ.Ԙ("Z")),ǂ.Ԙ("Speed"),ǂ.ʫ,ա(ʧ),ʐ,ʑ);}void Տ(Υ х){if(х==null)return;Π.t<Ԍ>();Φ Խ=Ψ.բ(х.я("Path"),х);if(Խ!=null)գ(Խ.Ԛ(
"TargetId"),Խ,null);}void Ր(Φ Խ){if(Խ==null)return;if(Խ.Ԏ.ContainsKey("RespondingToId")){string դ=Խ.я("RespondingToId");if(Ӣ.
ContainsKey(դ))Ӣ[դ]?.Invoke(Խ);}else Ș.Ֆ($"Response missing 'RespondingToId' header: {ʘ.ʥ(Խ.Ԏ)}");}public void գ(long ե,ԓ ǂ,Action<
ԓ>զ){Ӣ[$"{ǂ.Ԏ["Id"]}"]=զ;bool ї=false;var է=ǂ.ʫ.OrderBy(î=>î=="*").ToHashSet();if(է.Count==0)է.Add(Ա);foreach(string Հ in
է){string թ=Ո.ը(ǂ.ˌ(),Ն(Հ));ї=A.Ծ.SendUnicastMessage(ե,Հ,թ);if(ї)break;}if(ї)Π.t<ԍ>();else Π.t<ԋ>();}public void ժ(Υ х,
Action<ԓ>զ){Ӣ[х.ʋ]=զ;foreach(var Հ in ʫ){string թ=Ո.ը(х.ˌ(),Ն(Հ.Key));A.Ծ.SendBroadcastMessage(Հ.Key,թ);}Π.t<ԍ>();}public void
ѧ(string Ѡ,ѝ и){ʃ ʌ=ɡ.ʊ(Ѡ);if(ʌ==null){Ș.Ֆ($"Target '{Ѡ}' not found in Almanac.");return;}if(ʌ.ʞ!=ʃ.ʺ["grid"]){Ș.Ֆ(
$"Target '{Ѡ}' is not a grid (type: {ʌ.ʞ}).");return;}Υ х=ի(и.Ѥ).լ(ʌ);long խ=ʌ.ʟ!=0?ʌ.ʟ:ʌ.ͽ();գ(խ,х,null);}public void Ѧ(ѝ и){ɡ.ʷ("grid").ForEach(ծ=>ѧ(ծ.ʋ,и));}Υ ի(
string Ä){return կ("command",new Dictionary<string,object>{{"Command",Ä}});}Dictionary<string,object>յ(){MatrixD ʎ=A.ʍ();
Vector3D ձ=A.հ();return new Dictionary<string,object>{{"OriginId",$"{A.ʋ}"},{"GridId",$"{A.ղ}"},{"OriginName",A.Á},{"X",$"{ձ.X}"
},{"Y",$"{ձ.Y}"},{"Z",$"{ձ.Z}"},{"SafeRadius",$"{A.ʓ.Radius}"},{"Gravity",$"{A?.ճ()}"},{"Speed",$"{A?.մ()}"},{"Fx",
$"{ʎ.Forward.X}"},{"Fy",$"{ʎ.Forward.Y}"},{"Fz",$"{ʎ.Forward.Z}"},{"Ux",$"{ʎ.Up.X}"},{"Uy",$"{ʎ.Up.Y}"},{"Uz",$"{ʎ.Up.Z}"}};}public Υ կ(
string Τ,Dictionary<string,object>ն=null,Dictionary<string,object>շ=null){Dictionary<string,object>Ԓ=յ();Ԓ["Path"]=Τ;
Dictionary<string,object>ԑ=new Dictionary<string,object>();ո(Ԓ,շ);ո(ԑ,ն);return new Υ(ԑ,Ԓ);}public Φ ы(Υ х,Φ.ь չ,Dictionary<string
,object>ն=null,Dictionary<string,object>շ=null){Dictionary<string,object>պ=յ();Dictionary<string,object>ռ=new Dictionary<
string,object>(){{"Status",$"{Φ.ջ(չ)}"},{"TargetId",х.Ԏ["OriginId"]},{"TargetName",х.Ԏ["OriginName"]},{"RespondingToId",х.Ԏ[
"Id"]},};Dictionary<string,object>ս=new Dictionary<string,object>();ո(ռ,պ);ո(ռ,շ);ո(ս,ն);return new Φ(ս,ռ);}public void ɴ(){
if(!ш)return;var ʨ=ʫ.Keys.ToList();Υ х=կ("ping");х.ʫ=new HashSet<string>(ʨ);ժ(х,null);}public void ɳ(){var վ=A.e<Æ>().ѻ();
Υ х=կ("sync",new Dictionary<string,object>{{"Commands",string.Join(",",վ)}},new Dictionary<string,object>{{"OriginName",A
.Á}});տ(х,Խ=>ր(Խ));}void ր(ԓ Խ){Ի(Խ);Դ=0;}public void є(long խ,string Ä){Υ х=կ("localcmd",new Dictionary<string,object>{{
"Command",Ä}});A.Ծ.SendUnicastMessage(խ,Ա,х.ˌ());A.ϙ($"> @local {Ä}");}public void տ(Υ х,Action<ԓ>զ){Ӣ[х.ʋ]=զ;A.Ծ.
SendBroadcastMessage(Ա,х.ˌ(),TransmissionDistance.CurrentConstruct);}string Ն(string Հ){return ʫ.ContainsKey(Հ)?ʫ[Հ]:"";}void ո(Dictionary<
string,object>Ѡ,Dictionary<string,object>ԅ){if(ԅ==null)return;foreach(KeyValuePair<string,object>ɽ in ԅ)Ѡ[ɽ.Key]=ɽ.Value;}void
Ի(ԓ ǂ){long ʧ=ǂ.Ԛ("OriginId");string ց=ǂ.щ("Commands");if(!string.IsNullOrEmpty(ց)&&ʧ!=A.ʋ){var ѫ=new List<string>(ց.
Split(','));A.e<Æ>().Ҁ(ʧ,ѫ);}}long Ե(){if(Դ!=0)return Դ;long ւ=A.ʋ;var վ=A.e<Æ>().т;foreach(var Ѽ in վ.Keys)if(Ѽ<ւ)ւ=Ѽ;if(վ.
Count>0)Դ=ւ;return ւ;}Φ Ժ(Υ х){if(!ш)Ք(х);return null;}static string[]փ={"OriginId","GridId","OriginName","X","Y",
"Z","Speed","SafeRadius"};void Օ(ԓ ǂ){if(!ш)return;var Ԓ=new Dictionary<string,object>();foreach(string ɦ in փ)Ԓ[ɦ]=ǂ.я(ɦ);
Υ х=կ("almanac",null,Ԓ);A.Ծ.SendBroadcastMessage(Ա,х.ˌ(),TransmissionDistance.CurrentConstruct);}bool ա(long ʧ){return A.
e<d>().ì<IMyProgrammableBlock>().Any(ơ=>ơ.EntityId==ʧ);}}public class Υ:ԓ{public string ե;public string ք;public Υ(
Dictionary<string,object>ԑ,Dictionary<string,object>Ԓ):base(ԑ,Ԓ){}public Υ լ(ʃ ʌ){Ԏ["TargetId"]=$"{ʌ.ʋ}";Ԏ["TargetName"]=ʌ.ʬ??ʌ.ʋ;
ʫ=ʌ.ʫ;return this;}public override string ˌ()=>"REQUEST::"+base.ˌ();public static Υ Ս(string ǂ){ǂ=ǂ.Replace("REQUEST::",
"");string օ=ԥ(ǂ,"header");string ֆ=ԥ(ǂ,"body");return new Υ(ʘ.ʙ(ֆ),ʘ.ʙ(օ));}}public class Φ:ԓ{public enum ь{Ը=200,ѕ=201,և
=401,א=404,э=500,ב=600,ג=601,ד=602,ה=603,ו=604,}public Φ(Dictionary<string,object>ԑ,Dictionary<string,object>Ԓ):base(ԑ,Ԓ)
{}public override string ˌ()=>"RESPONSE::"+base.ˌ();public static Φ Ս(string ǂ){ǂ=ǂ.Replace("RESPONSE::","");string օ=ԥ(ǂ
,"header");string ֆ=ԥ(ǂ,"body");return new Φ(ʘ.ʙ(ֆ),ʘ.ʙ(օ));}static public int ջ(ь չ)=>(int)չ;}public class ט{public
string ז{get;}public Func<Υ,Φ>ח{get;}public ט(string Τ,Func<Υ,Φ>ω){ז=Τ;ח=ω;}}public class Ψ{public List<ט>י=new List<
ט>();public Φ բ(string Τ,Υ х){var Χ=י.FirstOrDefault(ʸ=>ʸ.ז==Τ);return Χ?.ח?.Invoke(х);}public void Ω(string Τ,Func<Υ,Φ>Χ
){י.Add(new ט(Τ,Χ));}}public class ך:º{ɣ À;public override string Á=>"get";public ך(ɣ Â){À=Â;}public override
string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;string ɦ=Ä.Å[0];string ȏ=À.ʕ(ɦ);return ȏ;}}public class כ:º{ɣ À;public
override string Á=>"set";public כ(ɣ Â){À=Â;}public override string Ñ(Ã Ä){if(Ä.Å.Count==0)return Æ.Ç.È;else if(Ä.Å.Count>=2){
string ɦ=Ä.Å[0];string ȏ=Ä.Å[1];À.ʤ(ɦ,ȏ);return$"{ɦ}={ȏ}";}return Æ.Ç.ħ;}}public class ɣ:ɵ{public string ל{get;set;}=
"";Dictionary<string,object>ם=new Dictionary<string,object>();bool מ=false;public ɣ(A B):base(B){ם=ʘ.ʙ(A.б.
Storage);}public override void o(){f(new כ(this));f(new ך(this));}public string ן(){ל=ʘ.ʥ(ם);מ=false;return ל;}public bool ɢ(){
ם.Clear();ל="";return מ=true;}public string ʕ(string ɦ){string ȏ="";if(ם.ContainsKey(ɦ))ȏ+=$"{ם[ɦ]}";return ȏ;}public
bool ʤ(string ɦ,string ȏ){ם[ɦ]=ȏ;מ=true;return מ;}}public class Ș:ɵ{const int נ=30;public List<string>ʄ{get;}=new List<
string>();public Ș(A B):base(B){}public void ș(string ʌ){ʔ(ʌ,"Info");}public void Ֆ(string ʌ){ʔ(ʌ,"Error");}void ʔ(string ʌ,
string ј=""){string ס=DateTime.Now.ToString("HH:mm:ss");string ע=ј!=""?"."+ј:"";if(ʄ.Count>נ)ʄ.RemoveAt(ʄ.Count-1);ʄ.Insert(0,
$"{ס}{ע} {ʌ}");}}public class ς:Ô{}public class σ:Ô{}public class ף:ɵ{d d;public ף(A B):base(B){}public override void o(){d=A.e<d>();
j<IMyMechanicalConnectionBlock>(ſ=>ſ.IsAttached,(l,m)=>פ(l as IMyMechanicalConnectionBlock,m));}protected void פ(
IMyMechanicalConnectionBlock ſ,object q){var ץ=q as bool?;var Κ=Ό.ContainsKey(ſ.EntityId)?Ό[ſ.EntityId]as bool?:null;if(ץ==true&&Κ!=true){t<ς>(ſ);d.
u(ſ,"onAttach");d.О(ſ.TopGrid);}else if(ץ==false&&Κ==true){t<σ>(ſ);d.u(ſ,"onDetach");d.Р();}}}public class ο:Ô{}public
class π:Ô{}public class צ:ɵ{d d;public צ(A B):base(B){}public override void o(){d=A.e<d>();j<IMyShipMergeBlock>(ק=>ק.State,(l
,m)=>ר(l as IMyShipMergeBlock,m));}protected void ר(IMyShipMergeBlock ק,object q){var r=q as MergeState?;var Κ=Ό.
ContainsKey(ק.EntityId)?Ό[ק.EntityId]as MergeState?:null;if(r.HasValue){switch(r){case MergeState.None:if(Κ==MergeState.Locked){t<π
>(ק);d.u(ק,"onUnmerge");d.ϋ();}break;case MergeState.Locked:if(Κ!=MergeState.Locked){t<ο>(ק);d.u(ק,"onMerge");d.ϋ();}
break;}}}public void ש(IMyShipMergeBlock ק){ק.Enabled=true;}public void ת(IMyShipMergeBlock ק){ק.Enabled=false;}public void װ
(IMyShipMergeBlock ק){if(ק.Enabled)ת(ק);else ש(ק);}}public static class Ⱦ{public static Ε ȿ<Ε>(string ȏ){return(Ε)System.
Convert.ChangeType(ȏ,typeof(Ε));}}public class Ո{static string ױ="##";public static bool Ջ(string ǂ)=>ǂ.StartsWith(ױ);
public static string ը(string ҏ,string Շ=""){if(Շ=="")return ҏ;else{var ײ=new char[ҏ.Length];for(int Ё=0;Ё<ҏ.Length;Ё++)ײ[Ё]=(
char)(ҏ[Ё]^Շ[Ё%Շ.Length]);return ױ+new string(ײ);}}public static string Չ(string ؠ,string Շ){ؠ=ؠ.Substring(ױ.Length);var ء=
new char[ؠ.Length];for(int Ё=0;Ё<ؠ.Length;Ё++)ء[Ё]=(char)(ؠ[Ё]^Շ[Ё%Շ.Length]);return new string(ء);}}public class أ:º{
آ À;public override string Á=>"clear";public أ(آ Â){À=Â;}public override string Ñ(Ã Ä){À.ؤ();return"";}}public class إ:
º{آ À;public override string Á=>"print";public إ(آ Â){À=Â;}public override string Ñ(Ã Ä){return Ä.Å[0];}}public
class آ:ɵ{ʆ ʆ;List<string>ئ=new List<string>();List<string>ا=new List<string>();public آ(A B):base(B){}
public override void o(){ʆ=A.e<ʆ>();Æ Æ=A.e<Æ>();Æ.f(new أ(this));Æ.f(new إ(this));}string ب="";public string ة()=>ب;public
void ت(string ǂ){ب+=ǂ+"\n";}public virtual string ج(){string ȁ="";string ث=(A.C??"").PadRight(15).Substring(0,15);ȁ+=
$" {ث}{ӟ()}   ({A.e<ʆ>().н()})\n"+$" {A.Á} *{A.Ӟ}\n"+$"------------------------------------------------------\n"+"";if(ب!="")ȁ+=$"{ة()}"+
$"------------------------------------------------------\n"+$"";return ȁ;}public void د(){string ح=$"{ج()}\n"+$"{String.Join("\n",ا.AsEnumerable().Reverse())}";خ(ح);ب="";}public
virtual string ӟ(){string ӡ=A.e<ĸ>().ɷ.Count()>0?"M":"   ";string Ӣ=A.e<ɱ>().Ӣ.Count()>0?"C":"    ";string ӣ=
$"{A.e<ɡ>().ʄ.Count()}";string ӥ=A.Ӥ?"A":"   ";string Ӧ=A.e<ʆ>().л>0?"W":"   ";int ذ=A.e<Æ>().т.Count;string ر=ذ>0?$"R{ذ}":"   ";return String.
Join("  ",Ӧ,ӥ,Ӣ,ӡ,ӣ,ر);}public void ϙ(string ǂ,bool ز=true){ئ.Add(ǂ);string س=ز&&ǂ.Length>37?ǂ.Substring(0,32)+"..."+ǂ.
Substring(ǂ.Length-5):ǂ;ا.Add(س);if(ا.Count>20)ا.RemoveRange(0,ا.Count-20);}public virtual void خ(string ǂ){A.б.Echo(ǂ);}public
bool ؤ(){ئ.Clear();ا.Clear();return ئ.Count==0&&ا.Count==0;}}public class A{public MyGridProgram б;public string C=
"Mother Program";public MyCommandLine ش;public bool Ӥ=false;public IMyCubeGrid ɬ;public IMyGridTerminalSystem ϲ;public
IMyIntergridCommunicationSystem Ծ;public IMyProgrammableBlock ɰ;public IMyShipController ϴ;public IMyGridProgramRuntimeInfo ص;public long ʋ;public
string Ӟ;public long ղ;public string Á;public BoundingSphereD ʓ;public enum ϕ{ض,ϖ,ϗ,ط,ظ,}public ϕ ϔ=ϕ.ض;public bool ҿ=false;
public Dictionary<string,Ά>ع=new Dictionary<string,Ά>();List<Ά>غ=new List<Ά>();public Dictionary<string,Ϋ>ػ=
new Dictionary<string,Ϋ>();public Dictionary<string,E>ؼ=new Dictionary<string,E>();public List<ɓ>Ύ=new List<ɓ>();public
Dictionary<string,string>ѣ=new Dictionary<string,string>();public Dictionary<string,string>Ӆ=new Dictionary<string,string>();
public bool ɒ=false;public A(MyGridProgram ؽ){ؾ(ؽ);}public void ؾ(MyGridProgram ؽ){б=ؽ;Ծ=б.IGC;ɰ=б.Me;ɬ=ɰ.CubeGrid;ϲ=б.
GridTerminalSystem;ص=б.Runtime;ʋ=Ծ.Me;Ӟ=$"{ʋ}".Substring($"{ʋ}".Length-5);ղ=ɬ.EntityId;Á=ɰ.CubeGrid.CustomName;ؿ();}public void ؿ(){List<Ϋ
>ɨ=new List<Ϋ>{new Ș(this),new ɭ(this),new ʆ(this),new Π(this),new Æ(this),new ɣ(this),new d(this),new ĸ(this),new ɡ(this
),new ɱ(this),new Ǐ(this),new آ(this),new Ӌ(this),new ף(this),new צ(this),};ɨ.ForEach(Â=>ـ(Â));}void ف(ϕ m){ϔ=m;}public
void o(){ف(ϕ.ϖ);e<ʆ>().ƅ();ϙ($"Booting {C}...");ق();ك();e<ʆ>().λ(ل());}IEnumerable<double>ل(){foreach(var ι in م())yield
return ι;ف(ϕ.ϗ);e<d>().u(ɰ,"onBoot");e<Π>().t<Ҳ>();ϙ($"{C} is online.");ϙ("Clearing console in 2 seconds...");ϙ(
"The Empire must grow.");e<ʆ>().з(()=>e<آ>()?.ؤ(),2.0);}IEnumerable<double>م(){int ن=غ.Count;for(int Ё=0;Ё<ن;Ё++){var Â=غ[Ё];ϙ(
$"Booting modules: ({Ё+1} / {ن})");var ه=Â.ΐ();while(ه.MoveNext())yield return ه.Current;Ҿ(Â.Σ());}ϙ("All modules booted.");}void ق(){double و=50;ʓ=new
BoundingSphereD(ɬ.WorldVolume.Center,ɬ.WorldVolume.Radius+و);}void ك(){f(new ɠ(this));f(new ɑ(this));f(new ɪ(this));}public void b(
string Z,UpdateType a){if(ɒ){ɒ=false;o();return;}if(ϔ==ϕ.ض)o();else if(ϔ==ϕ.ϖ){e<ʆ>().b();e<آ>()?.د();}else if(ϔ==ϕ.ϗ){if((a&(
UpdateType.Trigger|UpdateType.Terminal|UpdateType.Script))!=0){e<Æ>().Ϭ(Z);e<ʆ>().b();}else if(a==UpdateType.IGC)e<ɱ>().Մ();else{ى
();ي();}e<آ>().د();e<Ǐ>().Ӽ();}if(ҿ){e<آ>().ت("Complexity:  "+б.Runtime.CurrentInstructionCount.ToString()+"/50000");}}
void ى()=>ع.Values.ToList().ForEach(Â=>Â.b());void ي(){}public string Y()=>e<ɣ>()?.ן();public void ٮ(E Â){ؼ[Â.Δ()]=Â;ع[Â.Δ()
]=Â;غ.Add(Â);}public void D(List<E>ɨ){ɨ.ForEach(Â=>ٮ(Â));}public string Δ<Ε>()where Ε:Ά{foreach(var ɽ in ع)if(ɽ.Value is
Ε)return ɽ.Key;return typeof(Ε).Name;}public Ε e<Ε>()where Ε:class,Ά{var ٯ=Δ<Ε>();Ά Â;if(ع.TryGetValue(ٯ,out Â))return Â
as Ε;return null;}Ϋ ـ(Ϋ Â){ػ[Â.Δ()]=Â;ع[Â.Δ()]=Â;غ.Add(Â);return Â;}public void f(ɓ Ä){e<Æ>().f(Ä);}public void Ҿ(List<ɓ>ѫ
){ѫ.ForEach(Ä=>f(Ä));}public void ٳ(Action ٱ,double ٲ){e<ʆ>().з(ٱ,ٲ);}public void ϙ(string ǂ,bool ز=true){آ ٴ=e<آ>();if(ٴ
==null)б.Echo(ǂ);else ٴ.ϙ(ǂ,ز);e<Ș>()?.ș(ǂ);}public MatrixD ʍ(){return ϴ?.WorldMatrix??ɬ.WorldMatrix;}public Vector3D հ()
=>ɬ.GetPosition();public Vector3D ճ(){if(ϴ==null)return Vector3D.Zero;Vector3D ٵ=ϴ.GetArtificialGravity();if(ٵ.
LengthSquared()==0)ٵ=ϴ.GetNaturalGravity();return ٵ;}public double?մ()=>ϴ?.GetShipSpeed();public string ћ(string ҏ){if(Ӆ.Count==0)
return ҏ;var ٷ=Ӆ.OrderByDescending(ٶ=>ٶ.Key.Length);foreach(var ٸ in ٷ)ҏ=ҏ.Replace("$"+ٸ.Key,ٸ.Value);return ҏ;}public string
Ѵ(string ٹ,Dictionary<string,string>ɖ){if(ٹ.IndexOf("{{")==-1)return ћ(ٹ);var ϓ=new StringBuilder();int Ё=0;while(Ё<ٹ.
Length){int ٺ=ٹ.IndexOf("{{",Ё);if(ٺ==-1){ϓ.Append(ٹ,Ё,ٹ.Length-Ё);break;}if(ٺ>Ё)ϓ.Append(ٹ,Ё,ٺ-Ё);int ٻ=ٹ.IndexOf("}}",ٺ+2);
if(ٻ==-1){ϓ.Append(ٹ,ٺ,ٹ.Length-ٺ);break;}string ټ=ٹ.Substring(ٺ+2,ٻ-ٺ-2);string ٽ;string پ="";int ٿ=ټ.IndexOf(':');if(ٿ>=
0){ٽ=ټ.Substring(0,ٿ).Trim();پ=ټ.Substring(ٿ+1).Trim();}else{ٽ=ټ.Trim();}string ȏ;if(ɖ!=null&&ɖ.ContainsKey(ٽ))ȏ=ɖ[ٽ];
else ȏ=پ;ϓ.Append(ȏ);Ё=ٻ+2;}return ћ(ϓ.ToString());}}class ţ{public static Dictionary<string,string>ڀ=new
Dictionary<string,string>(){{"red","255,0,0"},{"green","0,255,0"},{"blue","0,0,255"},{"yellow","255,255,0"},{"cyan","0,255,255"},{
"magenta","255,0,255"},{"orange","255,165,0"},{"white","255,255,255"},{"black","0,0,0"}};public static Color ڃ(string ځ){string[]
ڂ=ځ.Split(',');return new Color(int.Parse(ڂ[0]),int.Parse(ڂ[1]),int.Parse(ڂ[2]));}public static Color چ(string ڄ){if(ڄ.
StartsWith("#"))ڄ=ڄ.Substring(1);int ʸ=څ(ڄ.Substring(0,2));int Д=څ(ڄ.Substring(2,2));int Ȁ=څ(ڄ.Substring(4,2));return new Color(ʸ,
Д,Ȁ);}static int څ(string ڇ){return int.Parse(ڇ,System.Globalization.NumberStyles.HexNumber);}public static Color ڈ(
string Ţ){string ځ;if(ڀ.TryGetValue(Ţ.ToLower(),out ځ))return ڃ(ځ);else return Color.White;}public static Color Ť(string Ţ){if
(Ţ.Contains(','))return ڃ(Ţ);else if(Ţ.StartsWith("#"))return چ(Ţ);else return ڈ(Ţ);}}public class ƹ{public static
BoundingBoxD ڎ(IEnumerable<Vector3D>ډ){if(!ډ.Any())return new BoundingBoxD(Vector3D.Zero,Vector3D.Zero);double ڊ=double.MaxValue;
double ڋ=double.MinValue;Vector3D ڌ=new Vector3D(ڊ,ڊ,ڊ);Vector3D ڍ=new Vector3D(ڋ,ڋ,ڋ);foreach(var ʏ in ډ){ڌ=Vector3D.Min(ڌ,ʏ)
;ڍ=Vector3D.Max(ڍ,ʏ);}return new BoundingBoxD(ڌ,ڍ);}public static Vector3D ڑ(Vector3D ڏ,BoundingBoxD ڐ){return new
Vector3D(MathHelper.Clamp(ڏ.X,ڐ.Min.X,ڐ.Max.X),MathHelper.Clamp(ڏ.Y,ڐ.Min.Y,ڐ.Max.Y),MathHelper.Clamp(ڏ.Z,ڐ.Min.Z,ڐ.Max.Z));}
public static Vector3D ڔ(BoundingSphereD ڒ,Vector3D ړ){Vector3D ƽ=Vector3D.Normalize(ڒ.Center-ړ);return ڒ.Center-(ƽ*ڒ.Radius);
}public static double ڕ(Vector3D Ț,Vector3D Ȁ){if(Vector3D.IsZero(Ț)||Vector3D.IsZero(Ȁ))return 0;return Math.Acos(
MathHelper.Clamp(Vector3D.Dot(Ț,Ȁ)/(Ț.Length()*Ȁ.Length()),-1,1));}public static void ڙ(Vector3D ږ,out Vector3D ڗ,out Vector3D ژ){
if(Math.Abs(ږ.X)>Math.Abs(ږ.Y))ڗ=new Vector3D(-ږ.Z,0,ږ.X);else ڗ=new Vector3D(0,ږ.Z,-ږ.Y);ڗ=Vector3D.Normalize(ڗ);ژ=
Vector3D.Normalize(Vector3D.Cross(ږ,ڗ));}public static float ƺ(float ļ,float ڌ,float ڍ){return Math.Max(ڌ,Math.Min(ڍ,ļ));}public
static Vector3D ڛ(string ښ){string[]Ȝ=ښ.Split(':');int ů=(Ȝ[0]=="GPS")?2:0;return new Vector3D(double.Parse(Ȝ[ů]),double.Parse
(Ȝ[ů+1]),double.Parse(Ȝ[ů+2]));}}public interface ʹ{string ˌ();}public static class Ì{public static string Í(string ǂ,
params object[]ڜ){return string.Format(ǂ,ڜ);}}public class ʘ{public static string ʥ(Dictionary<string,object>ˍ){var ҁ=new
StringBuilder();ҁ.Append("{");foreach(var ڞ in ˍ){ҁ.Append("\"").Append(ڝ(ڞ.Key)).Append("\":");var ڟ=ڞ.Value as ʹ;if(ڟ!=null)ҁ.
Append(ڟ.ˌ());else if(ڞ.Value is Dictionary<string,object>)ҁ.Append(ʥ((Dictionary<string,object>)ڞ.Value));else if(ڞ.Value is
List<object>)ҁ.Append(ڠ((List<object>)ڞ.Value));else if(ڞ.Value is string)ҁ.Append("\"").Append(ڝ((string)ڞ.Value)).Append(
"\"");else throw new InvalidOperationException("Unsupported value type: "+ڞ.Value?.GetType().Name);ҁ.Append(",");}if(ҁ.
Length>1)ҁ.Length--;ҁ.Append("}");return ҁ.ToString();}public static Dictionary<string,object>ʙ(string ӄ){var ڡ=new Dictionary
<string,object>();if(string.IsNullOrEmpty(ӄ)||ӄ[0]!='{'||ӄ[ӄ.Length-1]!='}')return ڡ;ӄ=ӄ.Substring(1,ӄ.Length-2);int Ů=ӄ.
Length;int Ё=0;while(Ё<Ů){int ڢ=ӄ.IndexOf('"',Ё);if(ڢ==-1)break;int ڣ=ӄ.IndexOf('"',ڢ+1);if(ڣ==-1)break;string ɦ=ڤ(ӄ.Substring
(ڢ+1,ڣ-ڢ-1));int ڥ=ӄ.IndexOf(':',ڣ)+1;if(ڥ==0)break;object ȏ;if(ӄ[ڥ]=='{'){int ڧ=ڦ(ӄ,ڥ,'{','}');if(ڧ==-1)break;string ڨ=ӄ
.Substring(ڥ,ڧ-ڥ+1);ȏ=ʙ(ڨ);Ё=ڧ+1;}else if(ӄ[ڥ]=='['){int ڧ=ڦ(ӄ,ڥ,'[',']');if(ڧ==-1)break;string ک=ӄ.Substring(ڥ,ڧ-ڥ+1);ȏ=
ڪ(ک);Ё=ڧ+1;}else if(ӄ[ڥ]=='"'){int ڧ=ӄ.IndexOf('"',ڥ+1);while(ڧ!=-1&&ӄ[ڧ-1]=='\\'){ڧ=ӄ.IndexOf('"',ڧ+1);}if(ڧ==-1)break;ȏ
=ڤ(ӄ.Substring(ڥ+1,ڧ-ڥ-1));Ё=ڧ+1;}else{int ڧ=ӄ.IndexOf(',',ڥ);if(ڧ==-1)ڧ=ӄ.Length;ȏ=ӄ.Substring(ڥ,ڧ-ڥ).Trim();Ё=ڧ;}ڡ.Add(
ɦ,ȏ);Ё=ӄ.IndexOf(',',Ё)+1;if(Ё==0)break;}return ڡ;}public static string ڠ(IEnumerable<object>Ӻ){var ҁ=new StringBuilder()
;ҁ.Append("[");foreach(var ګ in Ӻ){if(ګ is List<object>)ҁ.Append(ڠ((List<object>)ګ));else if(ګ is Dictionary<string,
object>)ҁ.Append(ʥ((Dictionary<string,object>)ګ));else if(ګ is string)ҁ.Append("\"").Append(ڝ((string)ګ)).Append("\"");else if
(ګ is ʹ)ҁ.Append(((ʹ)ګ).ˌ());else ҁ.Append("\"").Append(ڝ(ګ!=null?ګ.ToString():string.Empty)).Append("\"");ҁ.Append(",");
}if(ҁ.Length>1)ҁ.Length--;ҁ.Append("]");return ҁ.ToString();}public static List<object>ڪ(string ӄ){var Ӻ=new List<object>
();if(string.IsNullOrEmpty(ӄ)||ӄ[0]!='['||ӄ[ӄ.Length-1]!=']')return Ӻ;ӄ=ӄ.Substring(1,ӄ.Length-2);int Ů=ӄ.Length;int Ё=0;
while(Ё<Ů){char ڬ=ӄ[Ё];if(ڬ=='"'){int ڧ=ӄ.IndexOf('"',Ё+1);while(ڧ!=-1&&ӄ[ڧ-1]=='\\'){ڧ=ӄ.IndexOf('"',ڧ+1);}if(ڧ==-1)break;Ӻ.
Add(ڤ(ӄ.Substring(Ё+1,ڧ-Ё-1)));Ё=ڧ+1;}else if(ڬ=='{'){int ڧ=ڦ(ӄ,Ё,'{','}');if(ڧ==-1)break;string ڭ=ӄ.Substring(Ё,ڧ-Ё+1);Ӻ.
Add(ʙ(ڭ));Ё=ڧ+1;}else if(ڬ=='['){int ڧ=ڦ(ӄ,Ё,'[',']');if(ڧ==-1)break;string ک=ӄ.Substring(Ё,ڧ-Ё+1);Ӻ.Add(ڪ(ک));Ё=ڧ+1;}else
Ё++;if(Ё<Ů&&ӄ[Ё]==',')Ё++;}return Ӻ;}static int ڦ(string ӄ,int Ͻ,char ڮ,char گ){int ڰ=0;for(int Ё=Ͻ;Ё<ӄ.Length;Ё++){if(ӄ[
Ё]==ڮ)ڰ++;else if(ӄ[Ё]==گ)ڰ--;if(ڰ==0)return Ё;}return-1;}static string ڝ(string ڱ)=>ڱ.Replace("\\","\\\\").Replace("\"",
"\\\"");static string ڤ(string ڱ)=>ڱ.Replace("\\\"","\"").Replace("\\\\","\\");
