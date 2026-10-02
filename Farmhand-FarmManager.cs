// Farmhand v2.1.0
// Automated Farm Management
// By Duke Skyloafer | 2026-09-04
// 
// Full guide: https://steamcommunity.com/sharedfiles/filedetails/?id=3575736083
// 
// QUICK SETUP
// 
// 1. Add your blocks to groups:
//    - Farm Plots
//    - Irrigation Systems
//    - Air Vents
//    - Timer Blocks
//    - Broadcast Controllers
//    - Action Relays
// 
// 2. Add Display Screens: Put [FarmLCD] in the name of any LCD panel or multi-screen block (cockpit, control seat, etc.)
// 
// 3. Configure: Set the group name in your display block's custom data to match your farm group name
// 
// 4. Customize: Adjust settings through custom data on individual blocks
// 
// Configure all settings through custom data.
// 
string A;bool B=true;IEnumerator<C>J(){if(Me.CustomData!=A){B=true;}if(!B){yield return C.D;yield break;}A=Me.CustomData
;E.F();G=E.G;H=E.H;I=E.I;B=false;yield return C.D;}readonly K L;readonly M E;readonly string N="FarmLCD",O="PlotLCD",P=
"v2.1.0",Q="2026-09-04";readonly List<R>S=new List<R>();int T=0;public bool U{get;set;}public int H=30;int V;readonly Dictionary
<long,R>W=new Dictionary<long,R>();readonly Dictionary<long,X>Y=new Dictionary<long,X>();readonly Dictionary<long,Z>a=new
Dictionary<long,Z>();readonly List<b>c=new List<b>();bool d(){return U||V<=0;}void e(){V=H;U=false;}bool f=false;public
 Program
(){E=new M(Me,this);L=new K(GridTerminalSystem,this);Runtime.UpdateFrequency=UpdateFrequency.Update1;}public void
 Save
(){}public void
 Main
(string g){if(!string.IsNullOrWhiteSpace(g)){h(g.Trim());}if(!i){j();}k();}void s(){T=T>=5?0:T+1;foreach(b m in L.l()){m.
n=T;for(int o=0;o<m.p.Count;o++){m.p[o].q(f);}for(int o=0;o<m.r.Count;o++){m.r[o].q(f);}}for(int o=0;o<S.Count;o++){S[o].
q(f);}}void t(){c.Clear();foreach(b m in L.l()){c.Add(m);}}void h(string g){switch(g.ToLower()){case"cleanup":u();break;
case"rescan":U=true;B=true;break;case"pause":i=true;break;case"resume":i=false;break;case"debug on":I=true;break;case
"debug off":I=false;break;}}void k(){Echo("Farmhand");Echo($"{P} | {Q}");if(i){Echo("[PAUSED]");}Echo($"Step {v+1}/{w.Length}: {x}"
);Echo($"{y} ticks/cycle");Echo($"Instr {Runtime.CurrentInstructionCount}/{Runtime.MaxInstructionCount}");Echo(
$"Peak {z} (limit {(int)(Runtime.MaxInstructionCount*G)})");if(I){Echo("");Echo($"Groups: {L.ª}");Echo($"PlotLCDs: {S.Count}");Echo($"Rescan in: {V}");}}void Î(){var À=µ.º(T,
"Farmhand",TextAlignment.LEFT);Á(À,true);Á($"{P} | {Q}",true);Á("",true);foreach(var Â in L.l()){Á($"Group: {Â.Ã}",true);if(Â.Ä.
Count>0){Á($"Farm Plots: {Â.Ä.Count}");}if(Â.Å.Count>0){Á($"Irrigation Systems: {Â.Å.Count}");}if(Â.Æ.Count>0){Á(
$"Water Tanks: {Â.Æ.Count}");}var Ç=Â.p.Count+Â.r.Count;if(Ç>0){Á($"LCD Screen Providers: {Ç}");}if(Â.È.Count>0){Á($"Air Vents: {Â.È.Count}");}if(Â
.É.Count>0){Á($"Solar Food Generators: {Â.É.Count}");}if(Â.Ê.Ë>0){Á($"Timers: {Â.Ê.Ë}");}if(Â.Ê.Ì>0){Á(
$"Action Relays: {Â.Ê.Ì}");}if(Â.Ê.Í>0){Á($"Broadcast Controllers: {Â.Ê.Í}");}}if(S.Count>0){Á($"Plot LCDs: {S.Count}");}}void u(){Echo(
"Starting custom data cleanup...");int Ï=0;var Ð=L.l();foreach(var Â in Ð){foreach(var Ñ in Â.Ä){Ñ.Ò();Ï++;}foreach(var Ñ in Â.Å){Ñ.Ò();Ï++;}foreach(var
Ñ in Â.Æ){Ñ.Ò();Ï++;}foreach(var Ñ in Â.p){Ñ.Ò();Ï++;}foreach(var Ñ in Â.r){Ñ.Ò();Ï++;}foreach(var Ñ in Â.È){Ñ.Ò();Ï++;}
foreach(var Ñ in Â.É){Ñ.Ò();Ï++;}foreach(var Ô in Â.Ê.Ó()){Ô.Ò();Ï++;}foreach(var Ö in Â.Ê.Õ()){Ö.Ò();Ï++;}foreach(var Ù in Â.Ê
.Ø()){Ù.Ò();Ï++;}}E.Ò();Ï++;Echo($"Custom data cleanup complete!");Echo($"Processed {Ï} blocks.");}IEnumerator<C>ß(){if(!
d()){yield return C.D;yield break;}IEnumerator<C>Û=Ú();while(Û.MoveNext()){yield return Û.Current;}yield return C.D;if(Ü(
))yield return C.Ý;Þ();e();t();yield return C.D;}IEnumerator<C>Ú(){var à=new List<X>();var á=new List<Z>();List<
IMyTerminalBlock>â=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName($"[{N}]",â);â.ForEach(Ñ=>{if(Z.ã(Ñ)){Z ä;if(!a.
TryGetValue(Ñ.EntityId,out ä)){ä=new Z(Ñ,this,f);a[Ñ.EntityId]=ä;}else{ä.å();}á.Add(ä);}else if(X.ã(Ñ as IMyFunctionalBlock)){X æ;
if(!Y.TryGetValue(Ñ.EntityId,out æ)){æ=new X(Ñ as IMyTextPanel,this,f);Y[Ñ.EntityId]=æ;}else{æ.å();}à.Add(æ);}});var é=à.
ConvertAll(ç=>ç.Ã()).FindAll(è=>!string.IsNullOrWhiteSpace(è)).Distinct().ToList();var ê=á.ConvertAll(ä=>ä.Ã()).FindAll(è=>!string
.IsNullOrWhiteSpace(è));é.AddRange(ê);é=é.Distinct().ToList();var ë=E.Ã();if(!string.IsNullOrWhiteSpace(ë)&&!é.Contains(ë
)){é.Add(ë);}L.ì(é);yield return C.D;foreach(var í in é){var î=à.FindAll(ç=>ç.Ã()==í);var ï=á.FindAll(ä=>ä.Ã()==í);L.ð(í,
î,ï);var m=L.ñ(í);m.M=E;yield return C.D;L.ò(í);yield return C.D;L.ó(í);yield return C.D;L.ô(í);yield return C.D;L.õ(í);
yield return C.D;L.ö(í);yield return C.D;L.ø(í);yield return C.D;L.ù(í);yield return C.D;L.ú(í);yield return C.D;}}void Þ(){S
.Clear();List<IMyTerminalBlock>û=new List<IMyTerminalBlock>();GridTerminalSystem.SearchBlocksOfName($"[{O}]",û);û.ForEach
(Ñ=>{if(R.ã(Ñ)){R ü;if(!W.TryGetValue(Ñ.EntityId,out ü)){ü=new R(Ñ as IMyTextPanel,this,f);W[Ñ.EntityId]=ü;}S.Add(ü);if(ü
.ý){ü.þ();}}});}public enum C{Ý,D}const int ÿ=999999;public float G=0.8f;public bool I;bool i;int v;string x="init";
public int Ā{get;private set;}public int y{get;private set;}public int z{get;private set;}static readonly string[]w={
"Discovery","Config","ScanPlots","ScanSupport","Commit","BuildText","RenderText","BuildSprites","FlushSprites","RenderPlotLCDs"};
IEnumerator<C>ā;bool Ü(){return Runtime.CurrentInstructionCount>Runtime.MaxInstructionCount*G;}void j(){Ā++;try{do{if(ā==null||!ā.
MoveNext()){ā=Ă();break;}}while(!Ü());}catch(Exception ă){Ą(ă);ā=Ă();}if(Runtime.CurrentInstructionCount>z){z=Runtime.
CurrentInstructionCount;}}IEnumerator<C>Ă(){while(true){ą.Ć=ą.Ć>=ÿ?0:ą.Ć+1;f=!f;if(V>H){V=H;}if(V>0){V--;}s();t();y=Ā;Ā=0;z=0;x="Preamble";v=-1
;yield return C.D;for(int o=0;o<w.Length;o++){v=o;x=w[o];IEnumerator<C>Ĉ=ć(o);while(true){bool ĉ;try{ĉ=Ĉ.MoveNext();}
catch(Exception ă){Ą(ă);break;}if(!ĉ){break;}yield return Ĉ.Current;}yield return C.D;}}}void Ą(Exception ă){Echo(
$"Error in step {v} {x}: {ă.Message}");}IEnumerator<C>ć(int o){switch(o){case 0:return ß();case 1:return J();case 2:return Ċ();case 3:return ċ();case 4:
return Č();case 5:return č();case 6:return Ď();case 7:return ď();case 8:return Đ();case 9:return đ();default:return Ē();}}
IEnumerator<C>Ē(){yield return C.D;}readonly Dictionary<long,List<MySprite>>ē=new Dictionary<long,List<MySprite>>();IEnumerator<C>č
(){Î();yield return C.D;for(int Ĕ=0;Ĕ<c.Count;Ĕ++){b Â=c[Ĕ];ĕ(Â);yield return C.D;IEnumerator<C>ė=Ė(Â);while(ė.MoveNext()
){yield return ė.Current;}yield return C.D;if(Ü())yield return C.Ý;}}void ĕ(b Â){Ę(Â.Ã,"Farmhand","Header",ę:true,T:T);Ę(
Â.Ã,"","Header",ę:true);}IEnumerator<C>Ė(b Â){var í=Â.Ã;var ě=Â.Ě;var Ĝ=new List<string>();var ĝ=new List<string>();var Ğ
=new List<string>();var ğ=new List<string>();var Ġ=new List<string>();if(ě.ġ>0){Ĝ.Add(
$"Dead Plants: {ě.ġ} ({string.Join(", ",ě.Ģ.Distinct())})");}if(ě.ģ>0){Ĝ.Add($"Available Plots: {ě.ģ}");}if(ě.Ĥ>0){Ĝ.Add($"Harvest Ready Plots: {ě.Ĥ}");}if(ě.ĥ>0f){Ĝ.Add(
$"Water Usage: {ě.ĥ:F1} L/min");}if(!string.IsNullOrWhiteSpace(ě.Ħ)){ĝ.Add(ě.Ħ);}if(Â.Å.Count>0){Ğ.Add($"Ice: {ě.ħ:P0} ({ě.Ĩ:F1} kg / {ě.ĩ:F1} kg)");}
else{ě.Ī.Add("No Working Irrigation Systems!");}var ī=new List<string>();if(Â.Æ.Count>0){ī.Add(
$"Water: {ě.Ĭ:P1} ({ě.ĭ:F1} L / {ě.Į:F1} L)");}if(Â.É.Count>0){ğ.Add($"Production Rate: {ě.į:F2} items/min");string İ;float Ĳ=ě.ı;if(Ĳ<60f){İ=$"{Ĳ:F1} sec";}else if
(Ĳ<3600f){İ=$"{Ĳ/60f:F1} min";}else{İ=$"{Ĳ/3600f:F1} hr";}ğ.Add($"Next Production: {İ}");}if(ě.ĳ.Count>0){foreach(
KeyValuePair<string,int>Ķ in ě.ĳ){int Ĵ;if(!ě.ĵ.TryGetValue(Ķ.Key,out Ĵ)){Ĵ=0;}float ķ;if(!ě.ĸ.TryGetValue(Ķ.Key,out ķ)){ķ=0f;}var Ĺ
=new List<string>();if(ķ>0f){Ĺ.Add($"{ķ:P1}");}if(Ĵ>0){Ĺ.Add($"{Ĵ} Ready");}Ġ.Add(
$"{Ķ.Key} ({Ķ.Value} Plot{(Ķ.Value==1?"":"s")}): {string.Join(", ",Ĺ)}");}}if(ě.Ī.Count>0){Ę(í,"Alerts","ShowAlerts",ę:true);for(int o=0;o<ě.Ī.Count;o++){Ę(í,ě.Ī[o],"ShowAlerts");yield return
C.D;}Ę(í,"","ShowAlerts");}if(Ĝ.Count>0){Ę(í,"Farm Plots","ShowFarmPlots",ę:true);for(int o=0;o<Ĝ.Count;o++){Ę(í,Ĝ[o],
"ShowFarmPlots");yield return C.D;}Ę(í,"","ShowFarmPlots");}if(ĝ.Count>0){Ę(í,"Atmosphere","ShowAtmosphere",ę:true);for(int o=0;o<ĝ.
Count;o++){Ę(í,ĝ[o],"ShowAtmosphere");yield return C.D;}Ę(í,"","ShowAtmosphere");}if(Ğ.Count>0){Ę(í,"Irrigation",
"ShowIrrigation",ę:true);for(int o=0;o<Ğ.Count;o++){Ę(í,Ğ[o],"ShowIrrigation");yield return C.D;}Ę(í,"","ShowIrrigation");}if(ī.Count>0)
{Ę(í,"Water Tanks","ShowWaterTanks",ę:true);for(int o=0;o<ī.Count;o++){Ę(í,ī[o],"ShowWaterTanks");yield return C.D;}Ę(í,
"","ShowWaterTanks");}if(ğ.Count>0){Ę(í,"Solar Food Generators","ShowSolarFoodGenerators",ę:true);for(int o=0;o<ğ.Count;o
++){Ę(í,ğ[o],"ShowSolarFoodGenerators");yield return C.D;}Ę(í,"","ShowSolarFoodGenerators");}if(Ġ.Count>0){Ę(í,
"Current Yield","ShowYield",ę:true);for(int o=0;o<Ġ.Count;o++){Ę(í,Ġ[o],"ShowYield");yield return C.D;}Ę(í,"","ShowYield");}}
IEnumerator<C>Ď(){E.ĺ();yield return C.D;for(int Ĕ=0;Ĕ<c.Count;Ĕ++){b Â=c[Ĕ];for(int o=0;o<Â.p.Count;o++){X ç=Â.p[o];if(!ç.Ļ()){ç.ļ
(Â);ç.ĺ();}if(Ü())yield return C.Ý;}for(int o=0;o<Â.r.Count;o++){Â.r[o].ļ(Â);Â.r[o].Ľ();if(Ü())yield return C.Ý;}yield
return C.D;}}IEnumerator<C>ď(){for(int Ĕ=0;Ĕ<c.Count;Ĕ++){b Â=c[Ĕ];for(int o=0;o<Â.p.Count;o++){X ç=Â.p[o];if(!ç.Ļ())continue;
ç.ļ(Â);long Ŀ=ç.ľ.EntityId;List<MySprite>ŀ;if(!ē.TryGetValue(Ŀ,out ŀ)){ŀ=new List<MySprite>();ē[Ŀ]=ŀ;}ŀ.Clear();
IEnumerator ł=ç.Ł(ŀ,f);while(ł.MoveNext()){yield return C.D;}yield return C.D;if(Ü())yield return C.Ý;}}}IEnumerator<C>Đ(){for(int
Ĕ=0;Ĕ<c.Count;Ĕ++){b Â=c[Ĕ];for(int o=0;o<Â.p.Count;o++){X ç=Â.p[o];if(!ç.Ļ())continue;List<MySprite>ŀ;if(ē.TryGetValue(ç
.ľ.EntityId,out ŀ)){ç.Ń(ŀ);}yield return C.D;if(Ü())yield return C.Ý;}}}IEnumerator<C>đ(){for(int o=0;o<S.Count;o++){S[o]
.ń(T,E);yield return C.D;if(Ü())yield return C.Ý;}}void Ę(string í,string Ņ,string ņ=null,bool ę=false,int T=0){var m=L.ñ
(í);m.p.ForEach(ç=>{ç.Ň(Ņ,ņ,ę,T);});m.r.ForEach(ä=>{ä.Ň(Ņ,ņ,ę,T);});}void Á(string Ņ,bool À=false){E.Ň(Ņ,À);}const int ň=
20;IEnumerator<C>Ċ(){int ŉ=0;for(int Ĕ=0;Ĕ<c.Count;Ĕ++){b Â=c[Ĕ];Ŋ ě=Â.ŋ;ě.Ō();if(Â.Ä.Count==0){continue;}var Ŏ=E.ō;var Ő=
E.ŏ;var Œ=E.ő;var Ŕ=E.œ;var Ŗ=E.ŕ;var Ř=E.ŗ;var Ś=E.ř;var Ŝ=E.ś;for(int o=0;o<Â.Ä.Count;o++){ŝ(Â.Ä[o],ě,Ŏ,Ő,Œ,Ŕ,Ŗ,Ř,Ś,Ŝ);
ŉ++;if(ŉ%ň==0)yield return C.D;if(Ü())yield return C.Ý;}}}void ŝ(Ş ş,Ŋ ě,float Ŏ,float Ő,Color Œ,Color Ŕ,Color Ŗ,Color Ř,
Color Ś,bool Ŝ){var š=ş.Š;var Ĵ=ş.Ţ;var Ť=ş.ţ();if(!ş.ť()){U=true;}if(Ť!=null){ě.ĥ+=Ť.Ŧ;}var Ũ=new ŧ();Ũ.ũ=ş.ũ();Ũ.Ū=ş.ū;Ũ.Ŭ=
ş.ŭ;Ũ.Ů=ş.ů;Ũ.Ű=Ť!=null;Ũ.ű=Ť!=null?Ť.ű:0f;Ũ.Ų=(float)ş.Ų;Ũ.ō=Ŏ;Ũ.ŏ=Ő;var Ŵ=Ş.ų(Ũ);if(Ŝ){switch(Ŵ.ŵ){case Ŷ.ŷ:ş.Ÿ(Œ);
break;case Ŷ.Ź:ş.Ÿ(Ŕ);break;case Ŷ.ź:ş.Ÿ(Ŗ);break;case Ŷ.Ż:ş.Ÿ(Ř);break;case Ŷ.ż:ş.Ÿ(Ś);break;}ş.Ž=Ŵ.ž;ş.ſ=Ŵ.ƀ;}bool Ɓ=Ũ.Ū&&Ũ
.Ŭ&&Ũ.Ű&&Ũ.ű<Ŏ;if(Ũ.Ū){if(Ũ.Ŭ){ě.Ƃ++;int ƃ;if(ě.ĳ.TryGetValue(š,out ƃ)){ě.ĳ[š]=ƃ+1;}else{ě.ĳ[š]=1;}if(Ũ.Ů){ě.Ĥ++;int Ƅ;if
(ě.ĵ.TryGetValue(š,out Ƅ)){ě.ĵ[š]=Ƅ+Ĵ;}else{ě.ĵ[š]=Ĵ;}}else if(Ɓ){ě.Ī.Add(
$"Health Low: {Ť.ű:P1} ({(string.IsNullOrEmpty(š)?"":š+", ")}{ş.ƅ})");ě.Ɔ++;}else if(Ť!=null){float Ƈ;if(!ě.ĸ.TryGetValue(š,out Ƈ)||(Ť.ƈ>Ƈ&&Ť.ƈ<1f)){ě.ĸ[š]=Ť.ƈ;}}}else{ě.ġ++;ě.Ģ.Add(Ť==
null||string.IsNullOrWhiteSpace(Ť.Ɖ)?"Unknown":Ť.Ɖ);}}else{ě.ģ+=ş.ģ;}if(Ŵ.ŵ==Ŷ.ż){ě.Ī.Add(
$"Water Low: {ş.Ų:P1} ({(string.IsNullOrEmpty(š)?"":š+", ")}{ş.ƅ})");ě.Ɗ++;}}IEnumerator<C>ċ(){int ŉ=0;for(int Ĕ=0;Ĕ<c.Count;Ĕ++){b Â=c[Ĕ];if(Â.Ä.Count==0)continue;Ŋ ě=Â.ŋ;if(Â.È.Count>0)
{var Ƌ=Â.È[0];ě.ƌ=Ƌ.ƌ;switch(Ƌ.ƍ){case VentStatus.Pressurizing:case VentStatus.Pressurized:ě.Ħ=$"Pressurized: {Ƌ.ƌ:P0}";ě
.Ǝ=true;break;case VentStatus.Depressurizing:case VentStatus.Depressurized:if(Ƌ.Ə){ě.Ħ=
$"Depressurized (Room is Air Tight): {Ƌ.ƌ:P0}";}else{ě.Ħ=$"Depressurized: {Ƌ.ƌ:P0}";}ě.Ǝ=false;break;}}if(Â.Å.Count>0){float Ɛ=0f;float Ƒ=0f;Â.Å.ForEach(ƒ=>{Ɛ+=ƒ.Ɠ;Ƒ
+=ƒ.Ɣ;});ě.ħ=Ƒ>0?Ɛ/Ƒ:0f;ě.Ĩ=Ɛ/0.37f;ě.ĩ=Ƒ/0.37f;}if(Â.Æ.Count>0){double ƕ=0.0;double Ɩ=0.0;Â.Æ.ForEach(Ɨ=>{ƕ+=Ɨ.Ɠ;Ɩ+=Ɨ.Ɣ;}
);ě.Ĭ=Ɩ>0?(float)(ƕ/Ɩ):0f;ě.ĭ=(float)ƕ;ě.Į=(float)Ɩ;}if(Â.É.Count>0){float Ƙ=0f;float ƙ=float.MaxValue;Â.É.ForEach(ƚ=>{Ƙ
+=ƚ.ƛ;if(ƚ.Ɯ<ƙ){ƙ=ƚ.Ɯ;}});ě.į=Ƙ;ě.ı=ƙ!=float.MaxValue?ƙ:0f;}ŉ++;if(ŉ%ň==0)yield return C.D;if(Ü())yield return C.Ý;}}
IEnumerator<C>Č(){for(int Ĕ=0;Ĕ<c.Count;Ĕ++){b Â=c[Ĕ];Ŋ Ɲ=Â.ŋ;Â.ŋ=Â.Ě;Â.Ě=Ɲ;if(Â.Ä.Count>0){Ê ƞ=Â.Ê;ƞ.Ɵ("OnCropDying",Ɲ.Ɔ>0);ƞ.Ɵ(
"OnWaterLow",Ɲ.Ɗ>0);if(Â.È.Count>0){ƞ.Ɵ("OnPressurized",Ɲ.Ǝ);}if(Â.Å.Count>0){ƞ.Ɵ("OnIceLow",Ɲ.ħ<E.Ơ);}if(Â.Æ.Count>0){ƞ.Ɵ(
"OnWaterTankLow",Ɲ.Ĭ<E.ơ);}else{ƞ.Ɵ("OnWaterTankLow",false);}ƞ.Ɵ("OnCropDead",Ɲ.ġ>0);ƞ.Ɵ("OnCropAvailable",Ɲ.ģ>0);ƞ.Ɵ("OnCropReady",Ɲ.Ĥ>
0);ƞ.Ɵ("OnAllCropsReady",Ɲ.Ƃ>0&&Ɲ.Ĥ==Ɲ.Ƃ);}yield return C.D;if(Ü())yield return C.Ý;}}
}
internal class ƨ:ą{private readonly IMyTransponder Ƣ;private readonly Dictionary<string,ƣ>Ƥ=new Dictionary<string,ƣ>(){{
"OnWaterLowTrue",new ƣ("On Water Low","0","Channel (1-100) to signal when any farm plots' water level is low. 0 = disabled")},{
"OnWaterLowFalse",new ƣ("On Water Not Low","0",
"Channel (1-100) to signal when all farm plots' water levels are no longer low. 0 = disabled")},{"OnWaterTankLowTrue",new ƣ("On Water Tank Low","0",
"Channel (1-100) to signal when water tanks' water level is low. 0 = disabled")},{"OnWaterTankLowFalse",new ƣ("On Water Tank Not Low","0",
"Channel (1-100) to signal when water tanks' water levels are no longer low. 0 = disabled")},{"OnIceLowTrue",new ƣ("On Ice Low","0",
"Channel (1-100) to signal when the irrigation systems are low on ice. 0 = disabled")},{"OnIceLowFalse",new ƣ("On Ice Not Low","0",
"Channel (1-100) to signal when the irrigation systems are no longer low on ice. 0 = disabled")},{"OnPressurizedTrue",new ƣ("On Pressurized","0",
"Channel (1-100) to signal when air vents are pressurized. 0 = disabled")},{"OnPressurizedFalse",new ƣ("On Depressurized","0",
"Channel (1-100) to signal when air vents are depressurizing. 0 = disabled")},{"OnCropReadyTrue",new ƣ("On Any Crop Ready","0",
"Channel (1-100) to signal when any farm plot crop is ready for harvest. 0 = disabled")},{"OnCropReadyFalse",new ƣ("On No Crops Ready","0",
"Channel (1-100) to signal when no farm plots are ready for harvest. 0 = disabled")},{"OnAllCropsReadyTrue",new ƣ("On All Crops Ready","0",
"Channel (1-100) to signal when all planted farm plots with crops are ready for harvest. 0 = disabled")},{"OnAllCropsReadyFalse",new ƣ("On Not All Crops Ready","0",
"Channel (1-100) to signal when you plant a crop while all other planted plots are ready for harvest. 0 = disabled")},{"OnCropDyingTrue",new ƣ("On Crop Dying","0",
"Channel (1-100) to signal when at least one farm plot's health is below threshold. 0 = disabled")},{"OnCropDyingFalse",new ƣ("On No Crops Dying","0",
"Channel (1-100) to signal when all farm plots' health are above threshold or all have died. 0 = disabled")},{"OnCropDeadTrue",new ƣ("On Crop Dead","0","Channel (1-100) to signal when a farm plot crop has died. 0 = disabled")}
,{"OnCropDeadFalse",new ƣ("On No Dead Crops","0",
"Channel (1-100) to signal when no farm plots have dead crops. 0 = disabled")},{"OnCropAvailableTrue",new ƣ("On Plot Empty","0",
"Channel (1-100) to signal when a farm plot crop is available for planting. 0 = disabled")},{"OnCropAvailableFalse",new ƣ("On No Plots Empty","0",
"Channel (1-100) to signal when no farm plots have crops available for planting. 0 = disabled")},};public override IMyTerminalBlock ľ=>Ƣ;protected override Dictionary<string,ƣ>ƥ=>Ƥ;public ƨ(IMyTransponder Ʀ,
MyGridProgram Ƨ):base(Ƨ){Ƣ=Ʀ;å();}public void Ƭ(string Ʃ){if(ũ()){int ƫ=ƪ(Ʃ);if(ƫ>0&&ƫ<=100){Ƣ.SendSignal(ƫ);}}}int ƪ(string Ʃ){try{F
();int ƫ=ƭ.Get(Ʈ,Ƥ[Ʃ].Ư).ToInt32(0);return(ƫ>=1&&ƫ<=100)?ƫ:0;}catch{return 0;}}public static bool ã(IMyTerminalBlock Ñ){
return ư(Ñ)&&Ñ is IMyTransponder;}}internal class Ƴ:ą{private readonly IMyAirVent Ʊ;public override IMyTerminalBlock ľ=>Ʊ;
protected override Dictionary<string,ƣ>ƥ=>null;public Ƴ(IMyAirVent Ʋ,MyGridProgram Ƨ):base(Ƨ){Ʊ=Ʋ;}public float ƌ=>ũ()?Ʊ.
GetOxygenLevel():0f;public VentStatus ƍ=>ũ()?Ʊ.Status:VentStatus.Depressurized;public bool Ə=>ũ()&&Ʊ.CanPressurize;public static bool
ã(IMyTerminalBlock Ñ){return Ñ is IMyAirVent&&ư(Ñ);}}internal struct ƣ{public string Ư{get;}public string ƴ{get;}public
string Ƶ{get;}public ƣ(string ƶ,string Ʒ,string Ƹ=null){Ư=ƶ;ƴ=Ʒ;Ƶ=Ƹ;}}internal abstract class ą{internal static int Ć;
protected readonly MyIni ƭ=new MyIni();protected readonly string Ʈ="Farmhand";protected readonly MyGridProgram ƹ;int ƺ=-1;string
ƻ;public abstract IMyTerminalBlock ľ{get;}protected abstract Dictionary<string,ƣ>ƥ{get;}protected ą(MyGridProgram Ƨ){ƹ=Ƨ;
}public bool ũ(){return ư(ľ)&&(!(ľ is IMyFunctionalBlock)||(ľ as IMyFunctionalBlock).Enabled);}public bool ť(){return ľ!=
null&&!ľ.Closed;}public string ƅ=>ư(ľ)?ľ.CustomName:"NOT VALID";protected static bool ư<Ƽ>(Ƽ Ñ)where Ƽ:class,
IMyTerminalBlock{if(Ñ is IMyFunctionalBlock){return Ñ!=null&&!Ñ.Closed&&(Ñ as IMyFunctionalBlock).Enabled&&(Ñ as IMyFunctionalBlock).
IsFunctional;}else{return Ñ!=null&&!Ñ.Closed;}}internal void å(){if(ƥ!=null&&ƥ.Count>0&&ũ()){F();foreach(KeyValuePair<string,ƣ>Ķ in
ƥ){ƭ.Set(Ʈ,Ķ.Value.Ư,ƭ.Get(Ʈ,Ķ.Value.Ư).ToString(Ķ.Value.ƴ));if(!string.IsNullOrEmpty(Ķ.Value.Ƶ)){ƭ.SetComment(Ʈ,Ķ.Value.
Ư,$"; {Ķ.Value.Ƶ}");}}ƭ.SetSectionComment(Ʈ,
"; For more detailed explanations of options, see the official guide on Steam");string ƽ=ƭ.ToString();if(ƽ!=ľ.CustomData){ľ.CustomData=ƽ;}ƻ=ƽ;}}public void F(){if(ƥ==null||ƥ.Count==0||!ũ()){return;}
if(ƺ==Ć){return;}ƺ=Ć;string ƾ=ľ.CustomData;if(ƾ==ƻ){return;}MyIniParseResult ƿ;if(!ƭ.TryParse(ƾ,out ƿ)){ƹ.Echo(
$"Cannot Parse Custom Data in: {ľ.CustomName}");}ƻ=ƾ;}public void Ò(){if(ƥ==null||ƥ.Count==0||!ũ()){return;}F();List<MyIniKey>ǀ=new List<MyIniKey>();ƭ.GetKeys(Ʈ,ǀ);
HashSet<string>ǁ=new HashSet<string>();foreach(KeyValuePair<string,ƣ>Ķ in ƥ){ǁ.Add(Ķ.Value.Ư);}bool ǂ=false;foreach(MyIniKey ǃ
in ǀ){if(!ǁ.Contains(ǃ.Name)){ƭ.Delete(Ʈ,ǃ.Name);ǂ=true;}}if(ǂ){ľ.CustomData=ƭ.ToString();}}protected string ǅ(string Ǆ,
string Ʒ=""){if(ƥ==null||!ƥ.ContainsKey(Ǆ)){return Ʒ;}F();return ƭ.Get(Ʈ,ƥ[Ǆ].Ư).ToString(Ʒ);}protected bool ǆ(string Ǆ,bool Ʒ
=false){if(ƥ==null||!ƥ.ContainsKey(Ǆ)){return Ʒ;}try{F();return ƭ.Get(Ʈ,ƥ[Ǆ].Ư).ToBoolean(Ʒ);}catch{return Ʒ;}}}internal
class Ǌ:ą{private readonly IMyBroadcastController Ǉ;private readonly IMyChatBroadcastControllerComponent ǈ;private readonly
Dictionary<string,ƣ>Ƥ=new Dictionary<string,ƣ>(){{"OnWaterLowTrue",new ƣ("On Water Low","",
"Message to send when any farm plots' water level is low. Leave empty to disable")},{"OnWaterLowFalse",new ƣ("On Water Not Low","",
"Message to send when all farm plots' water levels are no longer low. Leave empty to disable")},{"OnWaterTankLowTrue",new ƣ("On Water Tank Low","",
"Message to send when water tanks' water level is low. Leave empty to disable")},{"OnWaterTankLowFalse",new ƣ("On Water Tank Not Low","",
"Message to send when water tanks' water levels are no longer low. Leave empty to disable")},{"OnIceLowTrue",new ƣ("On Ice Low","",
"Message to send when the irrigation systems are low on ice. Leave empty to disable")},{"OnIceLowFalse",new ƣ("On Ice Not Low","",
"Message to send when the irrigation systems are no longer low on ice. Leave empty to disable")},{"OnPressurizedTrue",new ƣ("On Pressurized","",
"Message to send when air vents are pressurized. Leave empty to disable")},{"OnPressurizedFalse",new ƣ("On Depressurized","",
"Message to send when air vents are depressurizing. Leave empty to disable")},{"OnCropReadyTrue",new ƣ("On Any Crop Ready","",
"Message to send when any farm plot crop is ready for harvest. Leave empty to disable")},{"OnCropReadyFalse",new ƣ("On No Crops Ready","",
"Message to send when no farm plots are ready for harvest. Leave empty to disable")},{"OnAllCropsReadyTrue",new ƣ("On All Crops Ready","",
"Message to send when all planted farm plots with crops are ready for harvest. Leave empty to disable")},{"OnAllCropsReadyFalse",new ƣ("On Not All Crops Ready","",
"Message to send when you plant a crop while all other planted plots are ready for harvest. Leave empty to disable")},{"OnCropDyingTrue",new ƣ("On Crop Dying","",
"Message to send when at least one farm plot's health is below threshold. Leave empty to disable")},{"OnCropDyingFalse",new ƣ("On No Crops Dying","",
"Message to send when all farm plots' health are above threshold or all have died. Leave empty to disable")},{"OnCropDeadTrue",new ƣ("On Crop Dead","","Message to send when a farm plot crop has died. Leave empty to disable")},
{"OnCropDeadFalse",new ƣ("On No Dead Crops","",
"Message to send when no farm plots have dead crops. Leave empty to disable")},{"OnCropAvailableTrue",new ƣ("On Plot Empty","",
"Message to send when a farm plot crop is available for planting. Leave empty to disable")},{"OnCropAvailableFalse",new ƣ("On No Plots Empty","",
"Message to send when no farm plots have crops available for planting. Leave empty to disable")},};public override IMyTerminalBlock ľ=>Ǉ;protected override Dictionary<string,ƣ>ƥ=>Ƥ;public Ǌ(IMyBroadcastController Ñ
,MyGridProgram Ƨ):base(Ƨ){Ǉ=Ñ;foreach(MyComponentBase ǉ in Ǉ.Components){if(ǈ==null){ǈ=ǉ as
IMyChatBroadcastControllerComponent;}}å();}public void Ƭ(string Ʃ){if(ũ()&&ǈ!=null){string ǌ=ǋ(Ʃ);if(!string.IsNullOrEmpty(ǌ)){ǈ.SendMessage(ǌ);}}}string ǋ
(string Ʃ){try{F();string ǌ=ƭ.Get(Ʈ,Ƥ[Ʃ].Ư).ToString("");return ǌ??"";}catch{return"";}}public static bool ã(
IMyTerminalBlock Ñ){return(Ñ is IMyBroadcastController)&&ư(Ñ);}}internal class b{public string Ã{get;}public List<Ş>Ä{get;}public List<Ǎ
>Å{get;}public List<ǎ>Æ{get;}public List<X>p{get;}public List<Z>r{get;}public List<Ƴ>È{get;}public List<Ǐ>É{get;}public
List<Ǌ>ǐ{get;}public Ê Ê{get;}public Ŋ Ě{get;set;}public Ŋ ŋ{get;set;}public M M{get;set;}public int n{get;set;}public
readonly Dictionary<long,Ş>Ǒ=new Dictionary<long,Ş>();public readonly Dictionary<long,Ǎ>ǒ=new Dictionary<long,Ǎ>();public
readonly Dictionary<long,ǎ>Ǔ=new Dictionary<long,ǎ>();public readonly Dictionary<long,Ƴ>ǔ=new Dictionary<long,Ƴ>();public
readonly Dictionary<long,Ǐ>Ǖ=new Dictionary<long,Ǐ>();public b(string í){Ã=í;Ä=new List<Ş>();Å=new List<Ǎ>();Æ=new List<ǎ>();p=
new List<X>();r=new List<Z>();È=new List<Ƴ>();É=new List<Ǐ>();ǐ=new List<Ǌ>();Ê=new Ê();Ě=new Ŋ();ŋ=new Ŋ();}}internal
class K{readonly Dictionary<string,b>ǖ=new Dictionary<string,b>();readonly IMyGridTerminalSystem Ǘ;readonly Program Ƨ;
readonly List<long>ǘ=new List<long>();readonly List<IMyFunctionalBlock>Ǚ=new List<IMyFunctionalBlock>();readonly List<
IMyGasGenerator>ǚ=new List<IMyGasGenerator>();readonly List<IMyGasTank>Ǜ=new List<IMyGasTank>();readonly List<IMyAirVent>ǜ=new List<
IMyAirVent>();public K(IMyGridTerminalSystem Ǘ,Program Ƨ){this.Ǘ=Ǘ;this.Ƨ=Ƨ;}public b ñ(string í){if(!ǖ.ContainsKey(í)){ǖ[í]=new b
(í);}return ǖ[í];}public void ð(string í,List<X>à,List<Z>ǝ){var m=ñ(í);m.p.Clear();m.r.Clear();m.p.AddRange(à);m.r.
AddRange(ǝ);}void ǥ<Ǟ,ǟ>(List<Ǟ>Ǡ,List<ǟ>ǡ,Dictionary<long,ǟ>Ǣ,Func<Ǟ,ǟ>ǣ)where Ǟ:class,IMyTerminalBlock where ǟ:ą{ǡ.Clear();for
(int o=0;o<Ǡ.Count;o++){Ǟ Ñ=Ǡ[o];long Ŀ=Ñ.EntityId;ǟ Ǥ;if(!Ǣ.TryGetValue(Ŀ,out Ǥ)){Ǥ=ǣ(Ñ);Ǣ[Ŀ]=Ǥ;}else{Ǥ.å();}ǡ.Add(Ǥ);}
if(Ǣ.Count!=ǡ.Count){ǘ.Clear();foreach(KeyValuePair<long,ǟ>Ķ in Ǣ){if(!Ķ.Value.ť()){ǘ.Add(Ķ.Key);}}for(int o=0;o<ǘ.Count;o
++){Ǣ.Remove(ǘ[o]);}}}public void ò(string í){var m=ñ(í);IMyBlockGroup Ǧ=Ǘ.GetBlockGroupWithName(í);if(Ǧ==null){m.Ä.Clear(
);return;}Ǚ.Clear();Ǧ.GetBlocksOfType(Ǚ,Ñ=>Ş.ã(Ñ));ǥ(Ǚ,m.Ä,m.Ǒ,Ñ=>new Ş(Ñ,Ƨ));}public void ó(string í){var m=ñ(í);
IMyBlockGroup Ǧ=Ǘ.GetBlockGroupWithName(í);if(Ǧ==null){m.Å.Clear();return;}ǚ.Clear();Ǧ.GetBlocksOfType(ǚ,Ñ=>Ǎ.ã(Ñ));ǥ(ǚ,m.Å,m.ǒ,Ñ=>
new Ǎ(Ñ,Ƨ));}public void ô(string í){var m=ñ(í);IMyBlockGroup Ǧ=Ǘ.GetBlockGroupWithName(í);if(Ǧ==null){m.Æ.Clear();return;}
Ǜ.Clear();Ǧ.GetBlocksOfType(Ǜ,Ñ=>ǎ.ã(Ñ));ǥ(Ǜ,m.Æ,m.Ǔ,Ñ=>new ǎ(Ñ,Ƨ));}public void õ(string í){var m=ñ(í);IMyBlockGroup Ǧ=Ǘ
.GetBlockGroupWithName(í);if(Ǧ==null){m.È.Clear();return;}ǜ.Clear();Ǧ.GetBlocksOfType(ǜ,Ñ=>Ƴ.ã(Ñ));ǥ(ǜ,m.È,m.ǔ,Ñ=>new Ƴ(Ñ
,Ƨ));}public void ö(string í){var m=ñ(í);IMyBlockGroup Ǧ=Ǘ.GetBlockGroupWithName(í);if(Ǧ==null){m.É.Clear();return;}Ǚ.
Clear();Ǧ.GetBlocksOfType(Ǚ,Ñ=>Ǐ.ã(Ñ));ǥ(Ǚ,m.É,m.Ǖ,Ñ=>new Ǐ(Ñ,Ƨ));}public void ø(string í){var m=ñ(í);IMyBlockGroup Ǧ=Ǘ.
GetBlockGroupWithName(í);m.Ê.ǧ();List<IMyTimerBlock>Ǩ=new List<IMyTimerBlock>();Ǧ?.GetBlocksOfType(Ǩ,Ñ=>ǩ.ã(Ñ));Ǩ.ForEach(Ñ=>m.Ê.Ǫ(new ǩ(Ñ,Ƨ)
));}public void ù(string í){var m=ñ(í);IMyBlockGroup Ǧ=Ǘ.GetBlockGroupWithName(í);m.Ê.ǫ();List<IMyTransponder>Ǭ=new List<
IMyTransponder>();Ǧ?.GetBlocksOfType(Ǭ,Ñ=>ƨ.ã(Ñ));Ǭ.ForEach(Ñ=>m.Ê.ǭ(new ƨ(Ñ,Ƨ)));}public void ú(string í){var m=ñ(í);IMyBlockGroup Ǧ=
Ǘ.GetBlockGroupWithName(í);m.Ê.Ǯ();List<IMyBroadcastController>ǯ=new List<IMyBroadcastController>();Ǧ?.GetBlocksOfType(ǯ,
Ñ=>Ǌ.ã(Ñ));ǯ.ForEach(Ñ=>m.Ê.ǰ(new Ǌ(Ñ,Ƨ)));}public IEnumerable<b>l(){return ǖ.Values;}public int ª=>ǖ.Count;public void ì
(List<string>é){var Ǳ=ǖ.Keys.Where(ǃ=>!é.Contains(ǃ)).ToList();Ǳ.ForEach(ǃ=>ǖ.Remove(ǃ));}}public class ǳ{public float ƈ{
get;set;}=0f;public TimeSpan ǲ{get;set;}=TimeSpan.Zero;public float ű{get;set;}=0f;public float Ŧ{get;set;}=0f;public
string Ɖ{get;set;}=string.Empty;}public enum Ŷ{ŷ,Ź,ź,Ż,ż}public struct ŧ{public bool ũ,Ū,Ŭ,Ů,Ű;public float ű,Ų,ō,ŏ;}public
struct Ǵ{public Ŷ ŵ;public float ž,ƀ;}internal class Ş:ą{private readonly IMyFunctionalBlock ǵ;private readonly
IMyFarmPlotLogic Ƕ;private readonly IMyLightingComponent Ƿ;private readonly IMyResourceStorageComponent Ǹ;ǳ ǹ;int Ǻ=-1;public override
IMyTerminalBlock ľ=>ǵ;protected override Dictionary<string,ƣ>ƥ=>null;public Ş(IMyFunctionalBlock ş,MyGridProgram Ƨ):base(Ƨ){ǵ=ş;foreach(
MyComponentBase ǉ in ǵ.Components){if(Ƕ==null){Ƕ=ǉ as IMyFarmPlotLogic;}if(Ƿ==null){Ƿ=ǉ as IMyLightingComponent;}if(Ǹ==null){Ǹ=ǉ as
IMyResourceStorageComponent;}}}public bool ū=>ũ()&&Ƕ!=null&&Ƕ.IsPlantPlanted;public bool ŭ=>ũ()&&Ƕ!=null&&Ƕ.IsAlive;public bool ů=>ũ()&&Ƕ!=null&&Ƕ.
IsPlantFullyGrown;public string Š=>ũ()&&Ƕ!=null&&Ƕ.OutputItem!=null?Ƕ.OutputItem.SubtypeName:string.Empty;public string ǻ=>ũ()&&Ƕ!=null&&
Ƕ.OutputItem!=null?$"{Ƕ.OutputItem.TypeId.ToString()}/{Ƕ.OutputItem.SubtypeName}":string.Empty;public int Ţ=>ũ()&&Ƕ!=null
&&Ƕ.IsPlantPlanted&&Ƕ.IsAlive?Ƕ.OutputItemAmount:0;public int ģ=>ũ()&&Ƕ!=null&&(!Ƕ.IsPlantPlanted||!Ƕ.IsAlive)?Ƕ.
AmountOfSeedsRequired:0;public double Ų=>ũ()&&Ǹ!=null?Ǹ.FilledRatio:0f;public ǳ ţ(){if(Ǻ==ą.Ć){return ǹ;}Ǻ=ą.Ć;ǹ=Ƕ==null?null:Ǽ(Ƕ.
GetDetailedInfoWithoutRequiredInput());return ǹ;}public static ǳ Ǽ(string ǽ){if(string.IsNullOrWhiteSpace(ǽ)){return null;}var Ǿ=new ǳ();var ǿ=new List<
float>();string[]Ȁ=ǽ.Split('\n');foreach(string ȁ in Ȁ){string Ȃ=ȁ.Trim();if(string.IsNullOrWhiteSpace(Ȃ)){continue;}if(Ȃ.
StartsWith("[Color=")){int ȃ=Ȃ.IndexOf(']');if(ȃ>0){Ȃ=Ȃ.Substring(ȃ+1);}}if(Ȃ.EndsWith("[/Color]")){Ȃ=Ȃ.Substring(0,Ȃ.Length-8);}
int Ȅ=Ȃ.IndexOf(':');if(Ȅ<0){continue;}string ȅ=Ȃ.Substring(Ȅ+1).Trim();if(ȅ.EndsWith("%")){string Ȇ=ȅ.Substring(0,ȅ.Length
-1).Trim();float ȇ;if(float.TryParse(Ȇ,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.
InvariantCulture,out ȇ)){ǿ.Add(ȇ/100f);}}else{TimeSpan Ȉ;if(TimeSpan.TryParse(ȅ,out Ȉ)){Ǿ.ǲ=Ȉ;}else{int ȉ=ȅ.IndexOf(' ');if(ȉ>0){string
Ȋ=ȅ.Substring(0,ȉ).Trim();float ȋ;if(float.TryParse(Ȋ,System.Globalization.NumberStyles.Float,System.Globalization.
CultureInfo.InvariantCulture,out ȋ)){Ǿ.Ŧ=ȋ;}}else if(!string.IsNullOrEmpty(ȅ)){Ǿ.Ɖ=ȅ;}}}}if(ǿ.Count>0)Ǿ.ƈ=ǿ[0];if(ǿ.Count>1)Ǿ.ű=ǿ[1
];return Ǿ;}public static Ǵ ų(ŧ Ȍ){Ǵ ȍ;ȍ.ŵ=Ŷ.ŷ;ȍ.ž=0f;ȍ.ƀ=1f;bool Ɓ=Ȍ.Ū&&Ȍ.Ŭ&&Ȍ.Ű&&Ȍ.ű<Ȍ.ō;bool Ȏ=Ȍ.Ū&&Ȍ.Ů;if(Ȍ.Ū){if(!Ȍ.
Ŭ){ȍ.ŵ=Ŷ.Ż;}else if(Ȍ.Ů){ȍ.ŵ=Ŷ.ź;}else if(Ɓ){ȍ.ŵ=Ŷ.Ż;ȍ.ž=2f;ȍ.ƀ=50f;}else{ȍ.ŵ=Ŷ.Ź;}}if(Ȍ.ũ&&Ȍ.Ų<=Ȍ.ŏ&&!Ɓ&&!Ȏ){ȍ.ŵ=Ŷ.ż;ȍ.ž
=2f;ȍ.ƀ=50f;}else if(!Ɓ||Ȏ){ȍ.ž=0f;ȍ.ƀ=1f;}return ȍ;}public float Ž{get{return ũ()&&Ƿ!=null?Ƿ.BlinkIntervalSeconds:0f;}
set{if(ũ()&&Ƿ!=null){Ƿ.BlinkIntervalSeconds=value;}}}public float ſ{get{return ũ()&&Ƿ!=null?Ƿ.BlinkLength:0f;}set{if(ũ()&&Ƿ
!=null){Ƿ.BlinkLength=value;}}}public void Ÿ(Color ȏ){if(ũ()&&Ƿ!=null){Ƿ.Color=ȏ;}}public static bool ã(IMyTerminalBlock Ñ
){if(!ư(Ñ)){return false;}foreach(MyComponentBase ǉ in Ñ.Components){var Ȑ=ǉ as IMyFarmPlotLogic;if(Ȑ!=null){return true;
}}return false;}}internal class Ŋ{public int ģ{get;set;}public int ġ{get;set;}public int Ɔ{get;set;}public int Ɗ{get;set;
}public int Ĥ{get;set;}public float ĥ{get;set;}public List<string>Ģ{get;set;}public Dictionary<string,int>ĳ{get;set;}
public Dictionary<string,int>ĵ{get;set;}public Dictionary<string,float>ĸ{get;set;}public int Ƃ{get;set;}public bool Ǝ{get;set;
}public float ƌ{get;set;}public string Ħ{get;set;}public float ħ{get;set;}public float Ĩ{get;set;}public float ĩ{get;set;
}public float Ĭ{get;set;}public float ĭ{get;set;}public float Į{get;set;}public float į{get;set;}public float ı{get;set;}
public List<string>Ī{get;set;}public Ŋ(){Ģ=new List<string>();ĳ=new Dictionary<string,int>();ĵ=new Dictionary<string,int>();ĸ=
new Dictionary<string,float>();Ī=new List<string>();Ō();}public void Ō(){Ģ.Clear();ĳ.Clear();ĵ.Clear();ĸ.Clear();Ī.Clear();
ģ=0;ġ=0;Ɔ=0;Ɗ=0;Ĥ=0;ĥ=0f;Ƃ=0;Ǝ=false;ƌ=0f;Ħ="";ħ=0f;Ĩ=0f;ĩ=0f;Ĭ=0f;ĭ=0f;Į=0f;į=0f;ı=0f;}}internal class Ǎ:ą{private
readonly IMyGasGenerator ȑ;private readonly IMyInventory Ȓ;public override IMyTerminalBlock ľ=>ȑ;protected override Dictionary<
string,ƣ>ƥ=>null;public Ǎ(IMyGasGenerator ƒ,MyGridProgram Ƨ):base(Ƨ){ȑ=ƒ;foreach(MyComponentBase ǉ in ȑ.Components){if(Ȓ==null
){Ȓ=ǉ as IMyInventory;}}}public float Ɠ=>ũ()&&Ȓ!=null?(float)Ȓ.CurrentVolume:0f;public float Ɣ=>ũ()&&Ȓ!=null?(float)Ȓ.
MaxVolume:0f;public static bool ã(IMyTerminalBlock Ñ){if(!(Ñ is IMyGasGenerator)||!ư(Ñ)){return false;}foreach(MyComponentBase ǉ
in Ñ.Components){var ȓ=ǉ as MyResourceSourceComponent;if(ȓ!=null&&ȓ.ResourceTypes.Any(Ȕ=>Ȕ.SubtypeName=="Water")){return
true;}}return false;}}internal class X:ą{private readonly IMyTextPanel ȕ;protected readonly StringBuilder Ȗ=new
StringBuilder();b ȗ;bool Ș;private readonly Dictionary<string,ƣ>Ƥ=new Dictionary<string,ƣ>(){{"GroupName",new ƣ("Group Name","",
"Make sure all blocks you want to track are in the same group")},{"Header",new ƣ("Header","true","Shows the animated header on the screen")},{"Title",new ƣ("Title","",
"Optional custom title text displayed below the header")},{"ShowAlerts",new ƣ("Show Alerts","true","Shows information requiring attention")},{"ShowFarmPlots",new ƣ(
"Show Farm Plots","true","Shows farm plot information")},{"ShowAtmosphere",new ƣ("Show Atmosphere","true","Shows atmospheric information"
)},{"ShowIrrigation",new ƣ("Show Irrigation","true","Shows irrigation system status")},{"ShowWaterTanks",new ƣ(
"Show Water Tanks","true","Shows water tank (modded) status")},{"ShowSolarFoodGenerators",new ƣ("Show Solar Food Generators","true",
"Shows solar food generator status")},{"ShowYield",new ƣ("Show Yield","true","Shows current crop yield")},{"TextAlignment",new ƣ("Text Alignment","left",
"Text alignment on screen (left or center)")},{"GraphicalMode",new ƣ("Graphical Mode","false","Shows graphical UI instead of text (true/false)")},};public override
IMyTerminalBlock ľ=>ȕ;protected override Dictionary<string,ƣ>ƥ=>Ƥ;public X(IMyTextPanel æ,MyGridProgram Ƨ,bool f):base(Ƨ){ȕ=æ;Ș=f;å();}
internal void q(bool ȅ){Ș=ȅ;}public string Ã(){return ǅ("GroupName","");}string ș(){return ǅ("Title","");}public void Ň(string Ņ
,string ņ=null,bool ę=false,int T=0){if(ũ()&&ȕ!=null&&!Ļ()){if(ņ==null||Ț(ņ)){string ț=Ņ;if(ņ=="Title"){ț=ș();if(string.
IsNullOrEmpty(ț)){return;}}else if(ņ=="Header"&&ę&&!string.IsNullOrEmpty(Ņ)){var Ȝ=ș();var ȝ=string.IsNullOrEmpty(Ȝ)?Ņ:Ȝ;ț=µ.º(T,ȝ,Ȟ(
));}else if(!ę&&Ȟ()==TextAlignment.LEFT){ț="  "+Ņ;}Ȗ.AppendLine(ț);}}}public bool Ļ(){return ǆ("GraphicalMode",false);}
public void ĺ(){if(ũ()&&ȕ!=null){if(Ļ()){ȟ();}else{ȕ.ContentType=ContentType.TEXT_AND_IMAGE;ȕ.Alignment=Ȟ();ȕ.WriteText(Ȗ.
ToString(),false);}Ȗ.Clear();}}TextAlignment Ȟ(){try{F();string Ƞ=ƭ.Get(Ʈ,Ƥ["TextAlignment"].Ư).ToString("left");if(Ƞ.Equals(
"center",System.StringComparison.OrdinalIgnoreCase)){return TextAlignment.CENTER;}return TextAlignment.LEFT;}catch{return
TextAlignment.LEFT;}}public void ļ(b Â){ȗ=Â;}public void ȟ(){if(ũ()&&ȕ!=null&&Ļ()){var Ȣ=new ȡ(ȕ,ȗ,ș(),Ș);Ȣ.ȟ();}}public IEnumerator
Ł(List<MySprite>ȣ,bool f){if(!ũ()||ȕ==null||!Ļ())yield break;var Ȣ=new ȡ(ȕ,ȗ,ș(),f);Ȣ.Ȥ();IEnumerator ȥ=Ȣ.Ł(ȣ);while(ȥ.
MoveNext()){yield return null;}}public void Ń(List<MySprite>ł){if(!ũ()||ȕ==null||!Ļ())return;if(ł.Count==0)return;using(var Ȧ=ȕ.
DrawFrame()){for(int o=0;o<ł.Count;o++){Ȧ.Add(ł[o]);}}}public bool Ț(string ņ){return ǆ(ņ,false);}public static bool ã(
IMyTerminalBlock Ñ){return Ñ is IMyTextPanel&&ư(Ñ);}}internal class R:ą{private readonly IMyTextPanel ȕ;Ş ȧ,Ȩ;bool ȩ;private readonly
MyGridProgram Ȫ;private const float ȫ=512f,Ȭ=73f;public override IMyTerminalBlock ľ=>ȕ;protected override Dictionary<string,ƣ>ƥ=>null
;bool Ș;public bool ý=>ȩ;public R(IMyTextPanel æ,MyGridProgram Ƨ,bool f):base(Ƨ){ȕ=æ;Ȫ=Ƨ;Ș=f;ȭ();}internal void q(bool ȅ)
{Ș=ȅ;}void ȭ(){if(ȕ!=null){int Ȯ=(int)System.Math.Round(ȕ.SurfaceSize.X);int ȯ=(int)System.Math.Round(ȕ.SurfaceSize.Y);ȩ=
(Ȯ==ȫ&&ȯ==Ȭ);}else{ȩ=false;}}public void þ(){ȧ=null;Ȩ=null;if(!ȩ||!ũ()){return;}var Ȱ=ȕ.CubeGrid;var ȱ=ȕ.Position;var Ȳ=ȕ
.Orientation;var ȳ=Ȳ.Up;Vector3I ȴ=-1*Base6Directions.GetIntVector(ȳ);var ȵ=Ȳ.Forward;Vector3I ȶ=Base6Directions.
GetIntVector(ȵ);Vector3I ȷ=-ȶ;Vector3I[]ȸ=new Vector3I[]{ȱ+ȴ,ȱ+ȷ,ȱ+ȶ,};foreach(var ȹ in ȸ){var Ñ=Ȱ.GetCubeBlock(ȹ);if(Ñ!=null&&Ñ.
FatBlock!=null){var Ⱥ=Ñ.FatBlock as IMyTerminalBlock;if(Ⱥ!=null&&Ş.ã(Ⱥ as IMyFunctionalBlock)){ȧ=new Ş(Ⱥ as IMyFunctionalBlock,ƹ
);return;}}}if(ȧ==null&&Ȩ==null){var Ȼ=ȕ.BlockDefinition.SubtypeName.EndsWith("BlockCorner_LCD_1");var ȼ=Ȳ.Left;Vector3I
Ƚ=Ȼ?-1*Base6Directions.GetIntVector(ȼ):Base6Directions.GetIntVector(ȼ);Vector3I Ⱦ=-Ƚ;var ȿ=ȱ+Ƚ;var ɀ=Ȱ.GetCubeBlock(ȿ);if
(ɀ!=null&&ɀ.FatBlock!=null){var Ⱥ=ɀ.FatBlock as IMyTerminalBlock;if(Ⱥ!=null&&Ş.ã(Ⱥ as IMyFunctionalBlock)){ȧ=new Ş(Ⱥ as
IMyFunctionalBlock,ƹ);}}var Ɂ=ȱ+Ⱦ;var ɂ=Ȱ.GetCubeBlock(Ɂ);if(ɂ!=null&&ɂ.FatBlock!=null){var Ⱥ=ɂ.FatBlock as IMyTerminalBlock;if(Ⱥ!=null&&Ş
.ã(Ⱥ as IMyFunctionalBlock)){Ȩ=new Ş(Ⱥ as IMyFunctionalBlock,ƹ);}}}}public void ń(int T,M Ƀ){if(!ũ()||ȕ==null){return;}ȕ.
ContentType=VRage.Game.GUI.TextPanel.ContentType.SCRIPT;if(!ȩ){ȕ.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;ȕ.
Alignment=VRage.Game.GUI.TextPanel.TextAlignment.CENTER;ȕ.WriteText("Screen Size Unsupported\n\nExpected: 512 x 73",false);}else{
var Ȣ=new Ʉ(ȕ,ȧ,Ȩ,T,Ƀ,Ș);Ȣ.Ʌ();}}public static bool ã(IMyTerminalBlock Ñ){if(!(Ñ is IMyTextPanel)||!ư(Ñ)){return false;}var
Ɇ=Ñ as IMyTextPanel;if(!Ɇ.Enabled){return false;}return Ñ.CustomName.Contains("[PlotLCD]");}}internal class Ʉ{private
const int ɇ=3,Ɉ=30,ɉ=15,Ɋ=67,ɋ=2,Ɍ=0;private readonly IMyTextSurface ɍ;private readonly Ş ȧ,Ȩ;private readonly int Ɏ;private
readonly M ɏ;private readonly RectangleF ɐ;private readonly Vector2 ɑ;private readonly bool Ș;public Ʉ(IMyTextSurface ɒ,Ş ɓ,Ş ɔ,
int T,M Ƀ,bool f=false){ɍ=ɒ;ȧ=ɓ;Ȩ=ɔ;Ɏ=T;ɏ=Ƀ;Ș=f;ɑ=ɍ.TextureSize;ɐ=new RectangleF((ɑ-ɍ.SurfaceSize)/2f,ɍ.SurfaceSize);}
public void Ʌ(){using(var Ȧ=ɍ.DrawFrame()){if(Ș){Ȧ.Add(new MySprite());}bool ɕ=ȧ!=null;bool ɖ=Ȩ!=null;if(!ɕ&&!ɖ){ɗ(Ȧ);}else if
(ɕ&&ɖ){ɘ(Ȧ,ȧ,true);ɘ(Ȧ,Ȩ,false);}else if(ɕ){ə(Ȧ,ȧ);}else{ə(Ȧ,Ȩ);}}}void ɗ(MySpriteDrawFrame Ȧ){Ȧ.Add(new MySprite(){Type=
SpriteType.TEXT,Data="Farm Plot Not Found",Position=ɚ(ɐ.Width/2f,ɐ.Height/3),RotationOrScale=0.8f,Color=ɍ.ScriptForegroundColor,
Alignment=TextAlignment.CENTER,FontId="White",});}void ɘ(MySpriteDrawFrame Ȧ,Ş ş,bool ɛ){float ɜ=(ɐ.Width-Ɍ)/2f;float ɝ=ɛ?0f:(ɜ+Ɍ
);float ɞ=ɜ-(2*ɇ);float ɟ=ɐ.Height-(2*ɇ);float ɠ=ɐ.Height/2f;float ɡ=ɝ+ɇ+(Ɋ/2f);float ɢ=ɠ;float ɣ=ɝ+ɇ+Ɋ+(ɉ/2f);float ɤ=ɠ;
float ɥ=ɞ-Ɋ-ɉ;float ɦ=ɥ;float ɧ=ɝ+ɇ+Ɋ+ɉ+(ɦ/2f);float ɨ=ɠ;bool ɩ=(Ɏ%2)==1;var Ť=ş.ţ();var ɫ=µ.ɪ(ş,Ť,ɏ,ɩ);if(ş.ū){ɬ(Ȧ,ɡ,ɢ,ş);}
else{ɭ(Ȧ,ɡ,ɢ);}ɮ(Ȧ,ɣ,ɤ,ɟ,ɫ.ɯ,ş,ɉ);ɰ(Ȧ,ɧ,ɨ,ɦ,ɟ,ɫ.ƈ,ɫ.ɯ,ɫ.ɱ);}void ə(MySpriteDrawFrame Ȧ,Ş ş){float ɞ=ɐ.Width-(2*ɇ);float ɟ=ɐ.
Height-(2*ɇ);float ɠ=ɐ.Height/2f;float ɡ=ɇ+(Ɋ/2f);float ɢ=ɠ;float ɣ=ɇ+Ɋ+(Ɉ/2f);float ɤ=ɠ;float ɥ=ɞ-Ɋ-Ɉ;float ɦ=ɥ;float ɧ=ɇ+Ɋ+Ɉ
+(ɦ/2f);float ɨ=ɠ;bool ɩ=(Ɏ%2)==1;var Ť=ş.ţ();var ɫ=µ.ɪ(ş,Ť,ɏ,ɩ);if(ş.ū){ɬ(Ȧ,ɡ,ɢ,ş);}else{ɭ(Ȧ,ɡ,ɢ);}ɮ(Ȧ,ɣ,ɤ,ɟ,ɫ.ɯ,ş,Ɉ);ɰ(
Ȧ,ɧ,ɨ,ɦ,ɟ,ɫ.ƈ,ɫ.ɯ,ɫ.ɱ);}void ɬ(MySpriteDrawFrame Ȧ,float ɲ,float ɳ,Ş ş){Ȧ.Add(new MySprite(){Type=SpriteType.TEXTURE,Data
=µ.ɴ(ş.ǻ,ɍ),Position=ɚ(ɲ,ɳ),Size=new Vector2(Ɋ,Ɋ),Alignment=TextAlignment.CENTER,});}void ɭ(MySpriteDrawFrame Ȧ,float ɲ,
float ɳ){Ȧ.Add(new MySprite(){Type=SpriteType.TEXTURE,Data="Circle",Position=ɚ(ɲ,ɳ),Size=new Vector2(Ɋ*1/3,Ɋ*1/3),Color=ɏ.ő,
Alignment=TextAlignment.CENTER,});}void ɮ(MySpriteDrawFrame Ȧ,float ɲ,float ɳ,float ɵ,Color ɶ,Ş ş,int ɷ){Ȧ.Add(new MySprite(){
Type=SpriteType.TEXTURE,Data="SquareSimple",Position=ɚ(ɲ,ɳ),Size=new Vector2(ɷ,ɵ),Color=ɶ,Alignment=TextAlignment.CENTER,});
float ɸ=ɷ-(2*ɋ);float ɹ=ɵ-(2*ɋ);Ȧ.Add(new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",Position=ɚ(ɲ,ɳ),Size=new
Vector2(ɸ,ɹ),Color=ɍ.ScriptBackgroundColor,Alignment=TextAlignment.CENTER,});float ɺ=(float)ş.Ų;if(ɺ>0f){Color ɼ=ɻ(ş);float ɽ=ɸ
-(2*ɋ);float ɾ=ɹ-(2*ɋ);float ɿ=ɾ*ɺ;float ʀ=ɳ+(ɾ/2f)-(ɿ/2f);Ȧ.Add(new MySprite(){Type=SpriteType.TEXTURE,Data=
"SquareSimple",Position=ɚ(ɲ,ʀ),Size=new Vector2(ɽ,ɿ),Color=ɼ,Alignment=TextAlignment.CENTER,});}}void ɰ(MySpriteDrawFrame Ȧ,float ɲ,
float ɳ,float ʁ,float ɵ,float ķ,Color ɶ,Color ʂ){Ȧ.Add(new MySprite(){Type=SpriteType.TEXTURE,Data="SquareSimple",Position=ɚ(
ɲ,ɳ),Size=new Vector2(ʁ,ɵ),Color=ɶ,Alignment=TextAlignment.CENTER,});float ɸ=ʁ-(2*ɋ);float ɹ=ɵ-(2*ɋ);Ȧ.Add(new MySprite()
{Type=SpriteType.TEXTURE,Data="SquareSimple",Position=ɚ(ɲ,ɳ),Size=new Vector2(ɸ,ɹ),Color=ɍ.ScriptBackgroundColor,
Alignment=TextAlignment.CENTER,});if(ķ>0f){float ʃ=ɸ-(2*ɋ);float ɿ=ɹ-(2*ɋ);float ɽ=ʃ*ķ;float ʄ=ɲ-(ʃ/2f)+(ɽ/2f);Ȧ.Add(new MySprite
(){Type=SpriteType.TEXTURE,Data="SquareSimple",Position=ɚ(ʄ,ɳ),Size=new Vector2(ɽ,ɿ),Color=ʂ,Alignment=TextAlignment.
CENTER,});}}Vector2 ɚ(float ɲ,float ɳ){return new Vector2(ɲ,ɳ)+ɐ.Position;}Color ɻ(Ş ş){if(ɏ==null){return Color.Blue;}var Ť=ş
.ţ();var ɫ=µ.ɪ(ş,Ť,ɏ,false);return ɫ.ʅ;}}internal class M:ą{private readonly IMyProgrammableBlock ɏ;private readonly
IMyTextSurface ʆ;protected readonly StringBuilder Ȗ=new StringBuilder();Color?ʇ;Color?ʈ,ʉ,ʊ,ʋ;string ʌ;private readonly Dictionary<
string,ƣ>Ƥ=new Dictionary<string,ƣ>(){{"GroupName",new ƣ("Group Name","","Use only if you don't want to see status on an LCD")
},{"ControlFarmPlotLights",new ƣ("Control Farm Plot Lights","true",
"Enable automatic farm plot light control (set false if feature is buggy on your server)")},{"PlanterEmptyColor",new ƣ("Plot Empty Color","80,0,170","RGB color for empty farm plots (default: 80,0,170)")},{
"PlantedAliveColor",new ƣ("Plant Alive Color","255,255,255","RGB color for growing plants (default: 255,255,255)")},{"PlantedReadyColor",
new ƣ("Plant Ready Color","0,255,185","RGB color for ready-to-harvest plants (default: 0,255,185)")},{"PlantedDeadColor",
new ƣ("Plant Dead Color","255,0,25","RGB color for dead plants (default: 255,0,25)")},{"WaterLowColor",new ƣ(
"Water Low Color","0,65,255","RGB color for low water warning (default: 0,65,255)")},{"IceLowThreshold",new ƣ("Ice Low Threshold","0.2",
"Low ice percent threshold, between 0.0 and 1.0 (default: 0.2)")},{"WaterLowThreshold",new ƣ("Water Low Threshold","0.5",
"Low farm plot water percent threshold, between 0.0 and 1.0 (default: 0.5)")},{"WaterTankLowThreshold",new ƣ("Water Tank Low Threshold","0.2",
"Low water tank percent threshold, between 0.0 and 1.0 (default: 0.2)")},{"HealthLowThreshold",new ƣ("Health Low Threshold","1.0",
"Low health percent threshold, between 0.0 and 1.0 (default: 1.0)")},{"BudgetFraction",new ƣ("Budget Fraction","0.8",
"Fraction of the instruction allowance one run may use before yielding (0.1 to 0.95)")},{"RescanIntervalCycles",new ƣ("Rescan Interval Cycles","30","Cycles between periodic block discovery rescans")},{
"DebugLogging",new ƣ("Debug Logging","false","Show verbose pipeline detail on the programmable block screen")},};public override
IMyTerminalBlock ľ=>ɏ;protected override Dictionary<string,ƣ>ƥ=>Ƥ;public M(IMyProgrammableBlock Ƀ,MyGridProgram Ƨ):base(Ƨ){ɏ=Ƀ;ʆ=ɏ.
GetSurface(0);ʆ.ContentType=ContentType.TEXT_AND_IMAGE;å();}public string Ã(){F();return ƭ.Get(Ʈ,Ƥ["GroupName"].Ư).ToString("");}
void ʎ(){string ʍ=ľ.CustomData;if(ʌ!=ʍ){ʌ=ʍ;ʇ=null;ʈ=null;ʉ=null;ʊ=null;ʋ=null;}}public Color ő{get{ʎ();if(ʇ==null){F();ʇ=ʏ(
ƭ.Get(Ʈ,Ƥ["PlanterEmptyColor"].Ư).ToString(Ƥ["PlanterEmptyColor"].ƴ));}return ʇ.Value;}}public Color œ{get{ʎ();if(ʈ==null
){F();ʈ=ʏ(ƭ.Get(Ʈ,Ƥ["PlantedAliveColor"].Ư).ToString(Ƥ["PlantedAliveColor"].ƴ));}return ʈ.Value;}}public Color ŕ{get{ʎ();
if(ʉ==null){F();ʉ=ʏ(ƭ.Get(Ʈ,Ƥ["PlantedReadyColor"].Ư).ToString(Ƥ["PlantedReadyColor"].ƴ));}return ʉ.Value;}}public Color ŗ
{get{ʎ();if(ʊ==null){F();ʊ=ʏ(ƭ.Get(Ʈ,Ƥ["PlantedDeadColor"].Ư).ToString(Ƥ["PlantedDeadColor"].ƴ));}return ʊ.Value;}}public
Color ř{get{ʎ();if(ʋ==null){F();ʋ=ʏ(ƭ.Get(Ʈ,Ƥ["WaterLowColor"].Ư).ToString(Ƥ["WaterLowColor"].ƴ));}return ʋ.Value;}}public
float Ơ{get{F();var ʐ=ƭ.Get(Ʈ,Ƥ["IceLowThreshold"].Ư).ToString(Ƥ["IceLowThreshold"].ƴ);float ʑ;if(float.TryParse(ʐ,System.
Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out ʑ)){return Math.Max(0.0f,Math.Min(1.0f,ʑ));}
return 0.2f;}}public float ơ{get{F();var ʐ=ƭ.Get(Ʈ,Ƥ["WaterTankLowThreshold"].Ư).ToString(Ƥ["WaterTankLowThreshold"].ƴ);float
ʑ;if(float.TryParse(ʐ,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out ʑ)){
return Math.Max(0.0f,Math.Min(1.0f,ʑ));}return 0.2f;}}public float ŏ{get{F();var ʐ=ƭ.Get(Ʈ,Ƥ["WaterLowThreshold"].Ư).ToString(
Ƥ["WaterLowThreshold"].ƴ);float ʑ;if(float.TryParse(ʐ,System.Globalization.NumberStyles.Float,System.Globalization.
CultureInfo.InvariantCulture,out ʑ)){return Math.Max(0.0f,Math.Min(1.0f,ʑ));}return 0.2f;}}public float ō{get{F();var ʐ=ƭ.Get(Ʈ,Ƥ[
"HealthLowThreshold"].Ư).ToString(Ƥ["HealthLowThreshold"].ƴ);float ʑ;if(float.TryParse(ʐ,System.Globalization.NumberStyles.Float,System.
Globalization.CultureInfo.InvariantCulture,out ʑ)){return Math.Max(0.0f,Math.Min(1.0f,ʑ));}return 1.0f;}}public bool ś{get{return ǆ(
"ControlFarmPlotLights",true);}}public float G{get{F();var ƾ=ƭ.Get(Ʈ,Ƥ["BudgetFraction"].Ư).ToString(Ƥ["BudgetFraction"].ƴ);float ȅ;if(float.
TryParse(ƾ,System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out ȅ)){return Math.Max(
0.1f,Math.Min(0.95f,ȅ));}return 0.8f;}}public int H{get{F();var ƾ=ƭ.Get(Ʈ,Ƥ["RescanIntervalCycles"].Ư).ToString(Ƥ[
"RescanIntervalCycles"].ƴ);int ȅ;if(int.TryParse(ƾ,System.Globalization.NumberStyles.Integer,System.Globalization.CultureInfo.InvariantCulture
,out ȅ)){return ȅ<1?1:ȅ;}return 30;}}public bool I{get{return ǆ("DebugLogging",false);}}Color ʏ(string ʒ){var ʓ=ʒ.Split(
',');if(ʓ.Length==3){int Ȕ,Ĕ,ʔ;if(int.TryParse(ʓ[0].Trim(),out Ȕ)&&int.TryParse(ʓ[1].Trim(),out Ĕ)&&int.TryParse(ʓ[2].Trim(
),out ʔ)){return new Color(Ȕ,Ĕ,ʔ);}}return new Color(0,0,0);}public void Ň(string Ņ,bool À=false){if(ũ()&&ʆ!=null){if(À){
Ȗ.AppendLine(Ņ);}else{Ȗ.AppendLine("  "+Ņ);}}}public void ĺ(){if(ũ()&&ʆ!=null){ʆ.WriteText(Ȗ.ToString(),false);Ȗ.Clear();
}}}internal static class µ{internal struct ʕ{public float ƈ;public Color ɯ,ɱ,ʅ;}private static List<string>ʖ=null;public
static string ɴ(string ʗ,IMyTextSurface ɒ){if(string.IsNullOrEmpty(ʗ)){return string.Empty;}if(ʖ==null){ʖ=new List<string>();
if(ɒ!=null){ɒ.GetSprites(ʖ);}}if(!ʗ.StartsWith("MyObjectBuilder_")){return ʗ;}if(ʖ.Count==0){return ʗ;}var ʘ=ʗ.Replace(
"MyObjectBuilder_","ColorfulIcons_");if(ʖ.Contains(ʘ)){return ʘ;}return ʗ;}public static ʕ ɪ(Ş ʙ,ǳ Ť,M Ƀ,bool ɩ){var ƞ=new ʕ();if(Ƀ==null)
{ƞ.ɯ=Color.Green;ƞ.ɱ=Color.Green;ƞ.ʅ=Color.Blue;ƞ.ƈ=0f;return ƞ;}if(ʙ.ū&&ʙ.ŭ&&Ť!=null){ƞ.ƈ=Ť.ƈ;}bool ʚ=ʙ.ū&&ʙ.ŭ&&Ť!=null
&&Ť.ű<Ƀ.ō;bool ʛ=ʙ.ũ()&&ʙ.Ų<=Ƀ.ŏ&&!ʚ;Color ʜ;if(!ʙ.ū){ʜ=Ƀ.ő;}else if(!ʙ.ŭ){ʜ=Ƀ.ŗ;}else if(ʙ.ů){ʜ=Ƀ.ŕ;}else{ʜ=Ƀ.œ;}if(ʚ&&ɩ)
{ƞ.ɯ=Ƀ.ŗ;}else if(ʛ&&ɩ){ƞ.ɯ=Ƀ.ř;}else{ƞ.ɯ=ʜ;}if(ʙ.ū&&ʙ.ŭ){if(ʙ.ů){ƞ.ɱ=Ƀ.ŕ;}else if(ʚ){ƞ.ɱ=Ƀ.ŗ;}else if(ʛ&&ɩ){ƞ.ɱ=Ƀ.ř;}
else{ƞ.ɱ=Ƀ.œ;}}else{ƞ.ɱ=ʜ;}if(ʙ.Ų<=Ƀ.ŏ){ƞ.ʅ=Ƀ.ř;}else{ƞ.ʅ=ʜ;}return ƞ;}public static string º(int T,string ʝ="Farmhand",
TextAlignment Ƞ=TextAlignment.LEFT){var ʞ=new[]{"––•","–•–","•––"};var ʟ=new[]{"•––","–•–","––•"};var ʠ=T>2?T-3:T;return Ƞ==
TextAlignment.CENTER?$"{ʞ[ʠ%ʞ.Length]} {ʝ} {ʟ[ʠ%ʟ.Length]}":$"{ʝ} {ʟ[ʠ%ʟ.Length]}";}}internal class Ǐ:ą{private readonly
IMyFunctionalBlock Ǉ;private readonly IMySolarFoodGenerator ʡ;public override IMyTerminalBlock ľ=>Ǉ;protected override Dictionary<string,ƣ
>ƥ=>null;public float Ɯ=>ũ()&&ʡ!=null?ʡ.TimeRemainingUntilNextBatch:0f;public float ƛ=>ũ()&&ʡ!=null?ʡ.ItemsPerMinute:0f;
public Ǐ(IMyFunctionalBlock Ñ,MyGridProgram Ƨ):base(Ƨ){Ǉ=Ñ;foreach(MyComponentBase ǉ in Ǉ.Components){if(ʡ==null){ʡ=ǉ as
IMySolarFoodGenerator;}}}public static bool ã(IMyTerminalBlock Ñ){if(!(Ñ is IMyFunctionalBlock)||!ư(Ñ)){return false;}foreach(MyComponentBase
ǉ in Ñ.Components){var ʢ=ǉ as IMySolarFoodGenerator;if(ʢ!=null){return true;}}return false;}}internal class ʥ{public
IGrouping<string,Ş>ʣ{get;set;}public IGrouping<string,Ş>ʤ{get;set;}}internal struct ʰ{public int ʦ,ʧ,ʨ,ʩ,ʪ,ʫ,ʬ,ʭ;public float ʮ;
public bool ʯ;}internal class ȡ{private readonly IMyTextSurface ɍ;private readonly b ȗ;private readonly string ʱ;private
readonly RectangleF ɐ;private readonly Vector2 ɑ;private readonly bool ʲ;private readonly int ʳ,ʴ,ʵ,ʶ,ʷ,ʸ,ʹ,ʺ,ʻ,ʼ,ʽ,ʾ;private
readonly float ʿ;int ˀ=>ʹ+ʸ;private readonly bool Ș;List<ʥ>ˁ;int ˆ=-1;private const int ˇ=20;private readonly List<MySprite>ˈ=
new List<MySprite>();public ȡ(IMyTextSurface ɒ,b Â,string Ȝ="",bool f=false){ɍ=ɒ;ȗ=Â;ʱ=Ȝ;Ș=f;ɑ=ɍ.TextureSize;ɐ=new
RectangleF((ɑ-ɍ.SurfaceSize)/2f,ɍ.SurfaceSize);ʰ ˊ=ˉ((int)ɐ.Width,(int)ɐ.Height);ʲ=ˊ.ʯ;ʳ=ˊ.ʦ;ʵ=ˊ.ʧ;ʿ=ˊ.ʮ;ʴ=ʵ/2;ʾ=ʵ>0?ʵ+ʳ*4:ʳ;ʶ=ˊ.ʨ
;ʷ=ˊ.ʬ;ʸ=ˊ.ʩ;ʹ=ˊ.ʪ;ʺ=ˊ.ʫ;ʻ=ˊ.ʭ;int ˋ=(int)ɐ.Width-ʷ-ʶ-ʳ-ʳ;ʼ=(ˋ+ʳ)/(ʺ+ʳ);if(ʼ<1)ʼ=1;ʽ=ʼ/2-1;}private static ʰ ˉ(int ˌ,int
ˍ){float ˎ=Math.Min(1f,Math.Max(0.4f,ˌ/512f));var ˊ=new ʰ{ʦ=ˏ(6,ˎ),ʧ=ˏ(30,ˎ),ʮ=ˎ,ʨ=ˏ(50,ˎ),ʩ=ˏ(10,ˎ),ʪ=ˏ(40,ˎ),ʫ=ˏ(30,ˎ),
ʬ=ˏ(10,ˎ),ʭ=Math.Max(1,ˏ(2,ˎ)),};int ː=ˊ.ʪ+ˊ.ʩ;if(ˊ.ʧ+ˊ.ʦ*5+ː>ˍ){ˊ.ʧ=0;}int ˑ=ˍ-ˊ.ʦ*2;if(ː>ˑ){ˊ.ʪ=ˑ-ˊ.ʩ;ː=ˑ;}ˊ.ʨ=Math.Min
(ˊ.ʨ,ː);ˊ.ʯ=ˊ.ʪ>=ˊ.ʭ*4+1&&ˌ>=ˊ.ʬ+ˊ.ʨ+ˊ.ʦ*2+ˊ.ʫ;return ˊ;}private static int ˏ(int ˠ,float ˎ){return(int)Math.Round(ˠ*ˎ);}
Vector2 ɚ(float ɲ,float ɳ){return new Vector2(ɲ,ɳ)+ɐ.Position;}public void ȟ(){Ȥ();ˈ.Clear();IEnumerator ȥ=Ł(ˈ);while(ȥ.
MoveNext()){}if(ˈ.Count==0)return;using(var Ȧ=ɍ.DrawFrame()){for(int o=0;o<ˈ.Count;o++){Ȧ.Add(ˈ[o]);}}}public void Ȥ(){ɍ.
ContentType=ContentType.SCRIPT;ɍ.Script=string.Empty;}public IEnumerator Ł(List<MySprite>Ȧ){if(!ʲ){ˡ(Ȧ);yield break;}if(ȗ==null||ȗ.
Ä.Count==0){yield break;}if(Ș){Ȧ.Add(new MySprite());}ˢ(Ȧ);var ˤ=ˣ();int ˬ=ʾ;for(int Ȕ=0;Ȕ<ˤ.Count;Ȕ++){ʥ ˮ=ˤ[Ȕ];int Ͱ=0;
if(ˮ.ʣ!=null){List<Ş>ͱ=ˮ.ʣ.ToList();Ͱ=Ͳ(ͱ.Count);IEnumerator ʹ=ͳ(Ȧ,ͱ,ʷ,ˬ);while(ʹ.MoveNext()){yield return null;}}if(ˮ.ʤ!=
null){List<Ş>Ͷ=ˮ.ʤ.ToList();int ͷ=Ͳ(Ͷ.Count);if(ͷ>Ͱ){Ͱ=ͷ;}IEnumerator ͺ=ͳ(Ȧ,Ͷ,(int)ɐ.Width/2+ʷ,ˬ);while(ͺ.MoveNext()){yield
return null;}}ˬ+=Ͱ*(ʹ+ʳ*2)+ʳ*2;yield return null;}}int Ͳ(int ͻ){return(ͻ+ʼ-1)/ʼ;}List<ʥ>ˣ(){int ͼ=ȗ.Ä.Count;if(ˁ!=null&&ˆ==ͼ){
return ˁ;}ˆ=ͼ;var Ð=ȗ.Ä.GroupBy(ͽ=>ͽ.Š).ToList();var Ά=Ð.Where(Ĕ=>!string.IsNullOrEmpty(Ĕ.Key)).OrderByDescending(Ĕ=>Ĕ.Count()
).ToList();var Έ=Ð.Where(Ĕ=>string.IsNullOrEmpty(Ĕ.Key)).ToList();var Ή=Ά.Concat(Έ).ToList();var ˤ=new List<ʥ>();
IGrouping<string,Ş>Ί=null;foreach(var m in Ή){bool Ό=m.Count()<=ʽ;if(Ό){if(Ί==null){Ί=m;}else{ˤ.Add(new ʥ{ʣ=Ί,ʤ=m});Ί=null;}}else
{if(Ί!=null){ˤ.Add(new ʥ{ʣ=Ί});Ί=null;}ˤ.Add(new ʥ{ʣ=m});}}if(Ί!=null){ˤ.Add(new ʥ{ʣ=Ί});}ˁ=ˤ;return ˤ;}µ.ʕ ɪ(Ş ʙ,bool ɩ)
{var Ť=ʙ.ţ();return µ.ɪ(ʙ,Ť,ȗ.M,ɩ);}IEnumerator ͳ(List<MySprite>Ȧ,List<Ş>Ύ,int Ώ,int ΐ){Α(Ȧ,Ώ,ΐ,Ύ);bool ɩ=(ȗ.n%2)==1;int
Β=Ώ+ʶ+ʳ;for(int o=0;o<Ύ.Count;o++){var ʙ=Ύ[o];int Γ=o/ʼ;int Δ=o%ʼ;int ʹ=Β+Δ*(ʺ+ʳ);int Ε=ΐ+Γ*(ˀ+ʳ*2);var ɫ=ɪ(ʙ,ɩ);Ζ(Ȧ,ʹ,Ε,
ʙ,ɫ);if((o+1)%ˇ==0){yield return null;}}}void ˢ(List<MySprite>Ȧ){if(ʵ==0)return;var ȝ=string.IsNullOrEmpty(ʱ)?"Farmhand":
ʱ;string Η=µ.º(ȗ?.n??0,ȝ,TextAlignment.CENTER);Ȧ.Add(new MySprite(){Type=SpriteType.TEXT,Data=Η,Position=ɚ(ɐ.Width/2f,ʴ),
RotationOrScale=ʿ,Color=ɍ.ScriptForegroundColor,Alignment=TextAlignment.CENTER,FontId="White",});}void Α(List<MySprite>Ȧ,int Ώ,int ΐ,
List<Ş>Ύ){if(Ύ.Count>0){int Θ=ΐ+(ˀ-ʶ)/2;if(Ύ[0].ū){Ȧ.Add(new MySprite(){Type=SpriteType.TEXTURE,Data=µ.ɴ(Ύ[0].ǻ,ɍ),Position=
ɚ(Ώ+ʶ/2f,Θ+ʶ/2f),Size=new Vector2(ʶ,ʶ),Alignment=TextAlignment.CENTER,});}else{int Ι=ʶ/3;int Κ=(ʶ-Ι)/2;Λ(Ȧ,"Circle",Ώ+Κ+1
,Θ+Κ,Ι,Ι,ȗ.M.ő);}}}void Ζ(List<MySprite>Ȧ,int ʹ,int Ε,Ş ʙ,µ.ʕ ɫ){int Μ=ʻ;int Ν=ʹ+Μ;int Ξ=ʺ-2*Μ;int Ο=Ν+Μ;int ɽ=Ξ-2*Μ;Λ(Ȧ,
"SquareSimple",ʹ,Ε,ʺ,ˀ,ɫ.ɯ);int Π=Ε+Μ;int Ρ=ʹ-2*Μ;Λ(Ȧ,"SquareSimple",Ν,Π,Ξ,Ρ,ɍ.ScriptBackgroundColor);int Σ=Ρ-2*Μ;int Τ=(int)Math.
Round(Σ*ɫ.ƈ);if(Τ>0){Λ(Ȧ,"SquareSimple",Ο,Π+Μ+Σ-Τ,ɽ,Τ,ɫ.ɱ);}int Υ=Π+Ρ+Μ;int Φ=ʸ-Μ;Λ(Ȧ,"SquareSimple",Ν,Υ,Ξ,Φ,ɍ.
ScriptBackgroundColor);int Χ=(int)Math.Round(ɽ*(float)ʙ.Ų);if(Χ>0){Λ(Ȧ,"SquareSimple",Ο,Υ+Μ,Χ,Φ-2*Μ,ɫ.ʅ);}}void Λ(List<MySprite>Ȧ,string Ψ,
int ʹ,int Ε,int ˌ,int ˍ,Color ȏ){Ȧ.Add(new MySprite(){Type=SpriteType.TEXTURE,Data=Ψ,Position=ɚ(ʹ+ˌ/2f,Ε+ˍ/2f),Size=new
Vector2(ˌ,ˍ),Color=ȏ,Alignment=TextAlignment.CENTER,});}void ˡ(List<MySprite>Ȧ){Ȧ.Add(new MySprite(){Type=SpriteType.TEXT,Data=
"Screen Size Unsupported",Position=ɚ(ɐ.Width/2f,ʳ),RotationOrScale=0.7f,Color=ɍ.ScriptForegroundColor,Alignment=TextAlignment.CENTER,FontId=
"White",});string Ω=$"{(int)ɐ.Width} x {(int)ɐ.Height}";Ȧ.Add(new MySprite(){Type=SpriteType.TEXT,Data=Ω,Position=ɚ(ɐ.Width/2f,
ɐ.Height-20f-ʳ),RotationOrScale=0.5f,Color=ɍ.ScriptForegroundColor,Alignment=TextAlignment.CENTER,FontId="White",});}}
internal class Ê{private readonly Dictionary<string,bool>Ϊ=new Dictionary<string,bool>();private readonly List<ǩ>Ϋ=new List<ǩ>()
;private readonly List<ƨ>ά=new List<ƨ>();private readonly List<Ǌ>έ=new List<Ǌ>();public void Ǫ(ǩ Ô){Ϋ.Add(Ô);}public void
ǭ(ƨ ή){ά.Add(ή);}public void ǰ(Ǌ ί){έ.Add(ί);}public void ǧ(){Ϋ.Clear();}public void ǫ(){ά.Clear();}public void Ǯ(){έ.
Clear();}public void Ɵ(string ΰ,bool α){bool ǂ=β(ΰ,α);Ϊ[ΰ]=α;if(!ǂ){return;}string δ=γ(ΰ,α);foreach(ǩ Ô in Ϋ){Ô.Ƭ(δ);}foreach
(ƨ ή in ά){ή.Ƭ(δ);}foreach(Ǌ ί in έ){ί.Ƭ(δ);}}string γ(string ΰ,bool ȅ){return$"{ΰ}{(ȅ?"True":"False")}";}public bool β(
string ΰ,bool α){bool ε;if(!Ϊ.TryGetValue(ΰ,out ε)){return false;}return ε!=α;}public int Ë=>Ϋ.Count;public int Ì=>ά.Count;
public int Í=>έ.Count;public List<ǩ>Ó(){return Ϋ;}public List<ƨ>Õ(){return ά;}public List<Ǌ>Ø(){return έ;}}internal class Z:ą{
private readonly IMyTerminalBlock Ǉ;private readonly IMyTextSurfaceProvider ζ;protected readonly List<StringBuilder>Ȗ=new List<
StringBuilder>();b ȗ;bool Ș;private readonly Dictionary<string,ƣ>Ƥ=new Dictionary<string,ƣ>(){{"GroupName",new ƣ("Group Name","",
"Make sure all blocks you want to track are in the same group")},{"Header",new ƣ("Header","true","Shows the animated header on each screen")},{"Title",new ƣ("Title","",
"Optional custom title text displayed below the header")},{"ShowAlerts",new ƣ("Show Alerts","0","Shows information requiring attention (set index of screen, false to hide)")},
{"ShowAtmosphere",new ƣ("Show Atmosphere","0","Shows atmospheric information (set index of screen, false to hide)")},{
"ShowIrrigation",new ƣ("Show Irrigation","0","Shows irrigation system status (set index of screen, false to hide)")},{"ShowWaterTanks",
new ƣ("Show Water Tanks","0","Shows water tank (modded) status (set index of screen, false to hide)")},{
"ShowSolarFoodGenerators",new ƣ("Show Solar Food Generators","0","Shows solar food generator status (set index of screen, false to hide)")},{
"ShowYield",new ƣ("Show Yield","0","Shows current crop yield (set index of screen, false to hide)")},{"TextAlignment",new ƣ(
"Text Alignment","left","Text alignment on screen (left or center)")},{"GraphicalMode",new ƣ("Graphical Mode","false",
"Shows graphical UI instead of text (set index of screen, false to disable)")},};public override IMyTerminalBlock ľ=>Ǉ;protected override Dictionary<string,ƣ>ƥ=>Ƥ;public Z(IMyTerminalBlock Ñ,
MyGridProgram Ƨ,bool f):base(Ƨ){Ǉ=Ñ;ζ=Ñ as IMyTextSurfaceProvider;Ș=f;for(int o=0;o<ζ.SurfaceCount;o++){Ȗ.Add(new StringBuilder());}å
();}internal void q(bool ȅ){Ș=ȅ;}public string Ã(){return ǅ("GroupName","");}string ș(){return ǅ("Title","");}public void
Ň(string Ņ,string ņ=null,bool ę=false,int T=0){if(ũ()&&ζ!=null){string ț=Ņ;if(ņ=="Title"){ț=ș();if(string.IsNullOrEmpty(ț
)){return;}}else if(ņ=="Header"&&ę&&!string.IsNullOrEmpty(Ņ)){var Ȝ=ș();var ȝ=string.IsNullOrEmpty(Ȝ)?Ņ:Ȝ;ț=µ.º(T,ȝ,Ȟ());
}else if(!ę&&Ȟ()==TextAlignment.LEFT){ț="  "+Ņ;}if(ņ==null||(ņ=="Header"&&η())||ņ=="Title"){var ι=θ();ι.ForEach(κ=>{Ȗ[κ].
AppendLine(ț);});}else{var μ=λ(ņ);if(μ>=0){Ȗ[μ].AppendLine(ț);}}}}public int ξ(){try{F();string ν=ƭ.Get(Ʈ,Ƥ["GraphicalMode"].Ư).
ToString("false");if(ν!="false"){int μ;if(int.TryParse(ν,out μ)&&μ>=0){return μ;}}}catch{}return-1;}public void Ľ(){if(ũ()&&ζ!=
null){int ο=ξ();if(ο>=0&&ο<ζ.SurfaceCount){ȟ(ο);Ȗ[ο].Clear();}foreach(var κ in θ()){if(κ!=ο){var π=ζ.GetSurface(κ);π.
ContentType=ContentType.TEXT_AND_IMAGE;π.Alignment=Ȟ();π.WriteText(Ȗ[κ].ToString(),false);Ȗ[κ].Clear();}}}}TextAlignment Ȟ(){try{F(
);string Ƞ=ƭ.Get(Ʈ,Ƥ["TextAlignment"].Ư).ToString("left");if(Ƞ.Equals("center",System.StringComparison.OrdinalIgnoreCase)
){return TextAlignment.CENTER;}return TextAlignment.LEFT;}catch{return TextAlignment.LEFT;}}public void ļ(b Â){ȗ=Â;}void
ȟ(int μ){var π=ζ.GetSurface(μ);var Ȣ=new ȡ(π,ȗ,ș(),Ș);Ȣ.ȟ();}public int λ(string ņ){try{F();string ν=ƭ.Get(Ʈ,Ƥ[ņ].Ư).
ToString("false");if(ν!="false"){int μ;if(int.TryParse(ν,out μ)&&μ>=0){return μ;}}}catch{}return-1;}public List<int>θ(){F();List
<int>ι=new List<int>();int ο=ξ();foreach(var Ķ in Ƥ){if(Ķ.Key.StartsWith("Show")){var μ=λ(Ķ.Key);if(μ>=0&&μ!=ο&&!ι.
Contains(μ)){ι.Add(μ);}}}return ι;}bool η(){return ǆ("Header",false);}public static bool ã(IMyTerminalBlock Ñ){return!(Ñ is
IMyTextPanel)&&Ñ is IMyTextSurfaceProvider&&(Ñ as IMyTextSurfaceProvider).SurfaceCount>0&&ư(Ñ);}}internal class ǩ:ą{private readonly
IMyTimerBlock ρ;protected readonly StringBuilder Ȗ=new StringBuilder();private readonly Dictionary<string,ƣ>Ƥ=new Dictionary<string,ƣ
>(){{"TriggerNow",new ƣ("Trigger Immediately","true","Timer will trigger immediately instead of counting down")},{
"OnWaterLowTrue",new ƣ("On Water Low","false","Triggers when any farm plots' water level is low")},{"OnWaterLowFalse",new ƣ(
"On Water Not Low","false","Triggers when all farm plots' water levels are no longer low")},{"OnWaterTankLowTrue",new ƣ(
"On Water Tank Low","false","Triggers when water tanks' water level is low")},{"OnWaterTankLowFalse",new ƣ("On Water Tank Not Low","false",
"Triggers when water tanks' water levels are no longer low")},{"OnIceLowTrue",new ƣ("On Ice Low","false","Triggers when the irrigation systems are low on ice")},{"OnIceLowFalse",
new ƣ("On Ice Not Low","false","Triggers when the irrigation systems are no longer low on ice")},{"OnPressurizedTrue",new ƣ
("On Pressurized","false","Triggers when air vents are pressurized")},{"OnPressurizedFalse",new ƣ("On Depressurized",
"false","Triggers when air vents are depressurizing")},{"OnCropReadyTrue",new ƣ("On Any Crop Ready","false",
"Triggers when any farm plot crop is ready for harvest")},{"OnCropReadyFalse",new ƣ("On No Crops Ready","false","Triggers when no farm plots are ready for harvest")},{
"OnAllCropsReadyTrue",new ƣ("On All Crops Ready","false","Triggers when all planted farm plots with crops are ready for harvest")},{
"OnAllCropsReadyFalse",new ƣ("On Not All Crops Ready","false",
"Triggers when you plant a crop while all other planted plots are ready for harvest")},{"OnCropDyingTrue",new ƣ("On Crop Dying","false","Triggers when at least one farm plot's health is below threshold")}
,{"OnCropDyingFalse",new ƣ("On No Crops Dying","false",
"Triggers when all farm plots' health are above threshold or all have died")},{"OnCropDeadTrue",new ƣ("On Crop Dead","false","Triggers when a farm plot crop has died")},{"OnCropDeadFalse",new ƣ(
"On No Dead Crops","false","Triggers when no farm plots have dead crops")},{"OnCropAvailableTrue",new ƣ("On Plot Empty","false",
"Triggers when a farm plot crop is available for planting")},{"OnCropAvailableFalse",new ƣ("On No Plots Empty","false",
"Triggers when no farm plots have crops available for planting")},};public override IMyTerminalBlock ľ=>ρ;protected override Dictionary<string,ƣ>ƥ=>Ƥ;public ǩ(IMyTimerBlock ς,
MyGridProgram Ƨ):base(Ƨ){ρ=ς;å();}public void Ƭ(string Ʃ){if(ũ()&&σ(Ʃ)){if(σ("TriggerNow")){ρ.Trigger();}else{ρ.StartCountdown();}}}
bool σ(string Ʃ){try{F();return ƭ.Get(Ʈ,Ƥ[Ʃ].Ư).ToBoolean(false);}catch{return false;}}public static bool ã(IMyTerminalBlock
Ñ){return ư(Ñ)&&Ñ is IMyTimerBlock;}}internal class ǎ:ą{private readonly IMyGasTank τ;public override IMyTerminalBlock ľ
=>τ;protected override Dictionary<string,ƣ>ƥ=>null;public ǎ(IMyGasTank Ɨ,MyGridProgram Ƨ):base(Ƨ){τ=Ɨ;}public double Ɠ=>ũ(
)&&τ!=null?τ.FilledRatio*τ.Capacity:0.0;public double Ɣ=>ũ()&&τ!=null?τ.Capacity:0.0;public static bool ã(
IMyTerminalBlock Ñ){if(!(Ñ is IMyGasTank)||!ư(Ñ)){return false;}foreach(MyComponentBase ǉ in Ñ.Components){var ȓ=ǉ as
MyResourceSourceComponent;if(ȓ!=null&&ȓ.ResourceTypes.Any(Ȕ=>Ȕ.SubtypeName=="Water")){return true;}}return false;}