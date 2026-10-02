/*
 * Little Help
 *    pb
 *    bl, blk, gr, *bl*, *blk*       on off onoff
 *    gr-gr                               Group1_Name;Group2_Name
 *    gr-airlock                       $Time_1/$Time_2
 * gr-antenna, beacon          $Set_Radius/$Change_Radius
 * gr-batt                               mode auto recharge discharge
 * gr-conn_lock                     lock unlock toggle
 * gr-conn_collect                 on off toggle
 * gr-conn_throw                  on off toggle
 * gr-door                              open close toggle
 * gr-grav                              $Set_Grav/$Change_Grav $Set_Grav/set/$Set_Grav
 * gr-jump                             charge off toggle
 * gr-jump_dist                     $Set_Distance $Set_Distance/$Change_Dist
 * gr-light                              color=$R,$G,$B/radius=$Radius,$Intens/Blink=$Interval,$Length
 * gr-lgear_auto                    on off toggle
 * gr-lgear_lock                    lock unlock toggle
 * gr-piston_att                     attach detach toggle $Min_distance/$Max_distance
 * gr-piston_vel                    $Set_Velocity/$Change_Velocity $Set_Velocity/set/$Set_Velocity
 * gr-rotor_att                       attach detach toggle $Min_angle/$Max_angle
 * gr-rotor_lock                     lock unlock toggle
 * gr-rotor_vel                       $Set_Velocity/$Change_Velocity $Set_Velocity/set/$Set_Velocity
 * gr-sound                           play stop/$Sound/$Loop_Time
 * gr-sorter_drain                  on off toggle
 * sorter_list                         black white/$Ore_or_Ingot
 * gr-stockpile                      on off toggle
 * gr-timer                             trigger start stop toggle
 * gr-timer_delay                  $Set_Time/$Change_Time
 * gr-turret                             shoot tcharacters tstations tlargeships tsmallships tmeteors tmissiles tneutrals
 * gr-vent                              press depress toggle
 * gr-write                             screen=$Num/size=$FontSize/color=$R,$G,$B/background=$R,$G,$B/text=$Text  nltext=  addtext=  clear
 * gr-remote                          on off toggle
 * gr-waypoint                      GPS:$Name:$X:$Y:$Z
 * (all-)gr-show                     inv gas batt jump power
 * all-show                            mass/(cargo total)/$Max_mass grav/(art nat total) weight/$Max_weight
 * thrust                                 all atmo hydro ion/all left right up down forward backward
 * 
 */

/***   You should copy the example below to Custom Data section of your programmable block   ***

[Main]
CockpitPrefix=Cockpit
LCDPrefix = LCD

[Add]
Prefix=
Screen=0
DefaultMenu=0
WASDAutoDisable=true
SwitchThrusters=false
SwitchGyros=true
SwitchWheels=false
LCDAutoOff=false
WASDPrgOn=-1
WASDPrgOff=-1

[Color]
BackgroundColor=0,0,0
ItemBarColor=0,6,12
MainColor=10,160,255
SelItemBarColor=10,160,255
SelItemTextColor=0,0,0
OnColor=225,65,100
ArrowColor=155,225,255
ErrorMSGColor=225,65,100
HelpColor=155,225,255

[TopMenu]
0=Lights & Doors
1=Thrusrs & Batt

[0]
Light 1;bl;Corner Light;onoff
Light 2;bl;Corner Light 2;onoff
All Lights;gr;Lights;onoff
Door 1;door;Sliding Door;toggle
Door 2;door;Sliding Door 2;toggle
All Doors;gr-door;Doors;toggle
[1]
Brake Thrusters;thrust;all/backward;onoff
All Thrusters;thrust;all/all;onoff
Inv Vol;all-show;*;inv
Ship Mass;all-show;*;mass/total/1000000
All Batts;all-show;*;batt

[AutoAirlocks]
AutoAirlocks;gr;1.5;0

*** End of the Custom Data section ***/


const string Version = "1.5.1";

ǣ ʬ;ǥ ʫ;const string ʪ="DEBUG";bool ʩ=false;List<Ə>ʨ;List<ě>[]ʭ,ʧ;List<ē>[]ʥ,ʤ;List<string>ʣ;List<ē>ʢ,ʡ,ʠ,ʦ,ʟ,ʮ,ˀ,ʾ;
string ʽ,ʼ,ʻ,ʺ,ʹ;int ʸ,ʷ,ʶ,ʵ,ʴ,ʳ,ʲ,ʱ,ʰ,ʯ,ʞ,ʓ;bool ʃ,ʑ,ʐ,ʏ,ʎ;Dictionary<int,string>ʍ=new Dictionary<int,string>(3){{0,"auto"},{
1,"recharge"},{2,"discharge"}};Dictionary<string,int>ʌ=new Dictionary<string,int>(4){{"all",0},{"atmo",1},{"hydro",2},{
"ion",3}};Dictionary<string,int>ʒ=new Dictionary<string,int>(7){{"all",0},{"left",2},{"right",1},{"up",4},{"down",3},{
"forward",6},{"backward",5}};const int ʋ=9;const float ʉ=0.9f;const float ʈ=1.1f;const float ʇ=1.4f;TimeSpan ʆ;const int ʅ=9;
const UpdateFrequency ʄ=UpdateFrequency.Update10;public enum ʊ{Ȁ,Ǡ,ʝ,œ,ʛ,ʚ,ʙ,ʘ,Ǭ,ʜ,ʗ};public enum ʖ{Ȁ,ʕ,Ŭ,Ň,N,ʿ}bool ˁ,ί,Ν,Λ,
Κ,Ι,Θ;MyShipMass Η;double Ζ,Μ,Ε;IMyTextSurface Γ;IMyProgrammableBlock Β;List<ǥ.ż>Ț;List<IMyTextPanel>Α,ΐ;List<ă>Ώ,Δ;List<
IMyTerminalBlock>Ύ,Ξ;IMyBlockGroup ΰ,ή;IMyTerminalBlock έ;List<IMyThrust>[,]ά;List<IMyThrust>Ϋ;List<IMyInventory>Ϊ;List<IMyJumpDrive>Ω;
List<IMyBatteryBlock>Ψ;List<IMyPowerProducer>Χ;List<IMyGasTank>Φ;List<MyInventoryItemFilter>Υ;enum Τ{Ȁ,Σ,Ρ,Π,Ο,Ό,ˤ,ˆ,ˢ,ˡ,ˠ,ˑ
,ː,ˏ,ˣ,ˎ,ˌ,ˋ,ˊ,ˉ,ˈ,ˇ,ˍ,ˬ,ͷ,Ί,Έ,Ά,ͽ,ͼ,ͻ,ͺ,Ή}enum ɞ{Ͷ,ʹ,ͳ,Ͳ}List<Τ>ͱ;Dictionary<string,ě.Ě>Ͱ;static IMyGridTerminalSystem ˮ
;ŷ.ǂ ʔ;float ʂ=0;bool ȫ=false;Color Ʉ=new Color(0,0,0);Color Ƀ=new Color(0,6,12);Color ɂ=new Color(10,160,255);Color Ɂ=
new Color(10,160,255);Color ɀ=new Color(0,0,0);Color ȿ=new Color(225,65,100);Color Ʌ=new Color(155,225,255);Color Ⱦ=new
Color(225,65,100);Color ȼ=new Color(155,225,255);Vector2 Ȼ=new Vector2(250,38);Vector2 Ⱥ=new Vector2(0,26);Vector2 ȹ=new
Vector2(364,38);Vector2 ȸ=new Vector2(8,74);Vector2 ȷ=new Vector2(40,34);Vector2 Ƚ=new Vector2(414,2);Vector2 ȶ=new Vector2(52,
28);Vector2 ɇ=new Vector2(433,19);Vector2 ɘ=new Vector2(110,34);Vector2 ɗ=new Vector2(380,2);Vector2 ɖ=new Vector2(434,2);
Vector2 ɕ=new Vector2(22,12);Vector2 ɔ=new Vector2(436,19);Vector2 ɓ=new Vector2(200,2);Vector2 ɒ=new Vector2(392,4);Vector2 ɑ=
new Vector2(712,4);Vector2 ɐ=new Vector2(400,58);Vector2 ɏ=new Vector2(-400,58);Vector2 Ɏ=new Vector2(80,80);Vector2 ɍ=new
Vector2(120,120);Vector2 Ɍ=new Vector2(196,62);Vector2 ɋ=new Vector2(196,494);Vector2 Ɋ=new Vector2(32,32);Vector2 ɉ=new
Vector2(-24,24);Vector2 Ɉ=new Vector2(520,454);Vector2 ȵ=new Vector2(520,484);Vector2 ȩ=new Vector2(200,46);Vector2 Ȝ=new
Vector2(360,19);Vector2 ȧ=new Vector2(0,-22);Vector2 Ȧ=new Vector2(110,0);Vector2 ȥ=new Vector2(44,44);Vector2 Ȥ=new Vector2(16
,18);Vector2 ȣ=new Vector2(30,30);Vector2 Ȣ=new Vector2(374,18);Vector2 Ȩ=new Vector2(496,18);Vector2 ȡ=new Vector2(-16,
58);Vector2 Ƞ=new Vector2(16,58);const int ȟ=256;const int Ȟ=46;Program(){š();}void Main(string ȝ){if(ʩ){ʆ=ʆ+Runtime.
TimeSinceLastRun;Ȫ(ȝ);ʫ.Ǩ(ref ά,ref ʹ);switch(ʫ.ǽ(ref ʹ)){case ǥ.ȇ.Ų:ɩ();break;case ǥ.ȇ.ț:ɨ();break;case ǥ.ȇ.ǅ:ɮ();break;case ǥ.ȇ.ǃ:ʁ();
break;case ǥ.ȇ.Ƽ:ɿ();break;case ǥ.ȇ.Ǆ:ɾ();break;case ǥ.ȇ.ș:ɽ();break;}if(ʱ<1){Ȳ();Ȱ();}for(int Ë=0;Ë<ʨ.Count;Ë++){ʨ[Ë].Ū(ʆ);}
Ə.Ũ(ʨ);if(ʱ>ʅ)ʱ=-1;ʱ++;if(ȫ){ʂ+=(float)Runtime.TimeSinceLastRun.TotalSeconds;ʔ.Ƨ(ʂ);}}else{š();}Ţ();}void Ȫ(string Ȯ){
string[]ȴ=Ȯ.Split(';');switch(ȴ[0]){case"up":ɩ();break;case"down":ɨ();break;case"left":ɮ();break;case"right":ʁ();break;case
"prev":ɿ();break;case"next":ɾ();break;case"enter":ɽ();break;case"wasd":ʫ.ǭ(ǥ.ȗ.Ȕ,ĸ(ȴ),ref ʹ);break;case"wasd_on":ʫ.ǭ(ǥ.ȗ.Ȗ,ĸ(ȴ
),ref ʹ);break;case"wasd_off":ʫ.ǭ(ǥ.ȗ.ȕ,ĸ(ȴ),ref ʹ);break;case"run":if(ȴ.Length>3){ě û=new ě(ȴ[0],ȴ[1],ȴ[2],ȴ[3],Ͱ[ȴ[1]])
;ɼ(û);}else ʺ="Error, cmd too short: "+Ȯ;break;case"stop":Ə.Ŧ(ʨ);break;case"setup":š();break;}}void Ȳ(){ί=!ί;ͱ.Clear();if
(Ν)Η=Ț[0].ƀ.CalculateShipMass();if(Λ)Ζ=Ț[0].ƀ.GetNaturalGravity().Length();if(Κ)Μ=Ț[0].ƀ.GetArtificialGravity().Length();
if(Ι)Ε=Ț[0].ƀ.GetTotalGravity().Length();for(int Ë=0;Ë<ʭ[ʳ].Count;Ë++){Τ ȱ=Τ.Ȁ;ɚ(ʭ[ʳ][Ë],ref ȱ,ref ʭ[ʳ][Ë].ç);ͱ.Add(ȱ);}}
void Ȱ(){int ȯ=0;foreach(ǥ.ż ȳ in Ț){IMyTextSurface ķ;ķ=(ȳ.ƀ as IMyTextSurfaceProvider)?.GetSurface(ʸ);if(ķ!=null){var ȭ=ķ.
DrawFrame();Ȭ(ref ȭ,Ώ[ȯ],ʭ[ʳ],ͱ,ȯ);ȭ.Dispose();ȯ++;}}if(Θ){ContentType ź=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;if(ʫ
.Ƅ)ź=VRage.Game.GUI.TextPanel.ContentType.SCRIPT;foreach(IMyTextPanel ķ in ΐ)ķ.ContentType=ź;}if(ˁ){ȯ=0;foreach(
IMyTextPanel ķ in Α){var ȭ=ķ.DrawFrame();Ȭ(ref ȭ,Δ[ȯ],ʭ[ʳ],ͱ);ȭ.Dispose();ȯ++;}}}void Ȭ(ref MySpriteDrawFrame ȭ,ă ə,List<ě>ʀ,List<Τ>
ɵ,int ɴ=-99){MySprite ɳ;ɳ=new MySprite(SpriteType.TEXTURE,"SquareSimple",position:ə.ü.Center,color:Ʉ);ȭ.Add(ɳ);Vector2 ɲ=
Ⱥ;Vector2 ɱ=ɲ;ɲ.X=ɲ.X-ȟ*ʳ;for(int Ë=0;Ë<ʷ;Ë++){if(Ë==ʳ){ȭ.Add(ˀ[25].ĝ(ə.ú,ə.ø+ə.ú*ɲ));ȭ.Add(ʠ[Ë].ĝ(ə.ú,ə.ø+ə.ú*ɲ));}else{
ȭ.Add(ˀ[24].ĝ(ə.ú,ə.ø+ə.ú*ɲ));ȭ.Add(ʢ[Ë].ĝ(ə.ú,ə.ø+ə.ú*ɲ));}ɲ.X=ɲ.X+ȟ;}if(ʷ>1){Vector2 ɰ=Vector2.Zero;if(ʳ>0){ȭ.Add(ˀ[17]
.ĝ(ə.ú,ə.ø+ə.ú*ȡ));ɰ.X=-14;ɰ.Y=0;ȭ.Add(ˀ[20].ĝ(ə.ú,ə.ø+ə.ú*(ȡ+ɰ)));ɰ.X=0;ɰ.Y=-14;ȭ.Add(ˀ[14].ĝ(ə.ú,ə.ø+ə.ú*(ȡ+ɰ)));}if(ʳ+
1<ʷ){ȭ.Add(ˀ[17].ĝ(ə.ú,ə.ø+ə.ú*Ƞ));ɰ.X=14;ɰ.Y=0;ȭ.Add(ˀ[21].ĝ(ə.ú,ə.ø+ə.ú*(Ƞ+ɰ)));ɰ.X=0;ɰ.Y=-14;ȭ.Add(ˀ[15].ĝ(ə.ú,ə.ø+ə.ú
*(Ƞ+ɰ)));}}ɲ=ȸ;for(int Ë=ʵ;Ë<Math.Min(ʭ[ʳ].Count+ʵ,ʋ+ʵ);Ë++){if(Ë==ʶ){ȭ.Add(ˀ[27].ĝ(ə.ú,ə.ù+ə.ú*ɲ));ȭ.Add(ʤ[ʳ][Ë].ĝ(ə.ú,ə
.ù+ə.ú*ɲ));ɱ=ɲ;}else{ȭ.Add(ˀ[26].ĝ(ə.ú,ə.ù+ə.ú*ɲ));ȭ.Add(ʥ[ʳ][Ë].ĝ(ə.ú,ə.ù+ə.ú*ɲ));}int ɯ=(int)ɵ[Ë];switch(ʀ[Ë].ç.ę){case
ʖ.ʕ:ȭ.Add(ˀ[28].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+Ƚ)));if(ɯ==1)ȭ.Add(ʮ[ɯ-1].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+ɇ)));break;case ʖ.Ŭ:ȭ.Add(ˀ[29].ĝ(ə.ú,ə.ù+ə.ú
*(ɲ+ɗ)));if(ɯ==2)ȭ.Add(ʮ[ɯ-1].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+ɔ)));else if(ɯ>2)ȭ.Add(ʮ[ɯ-1].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+ɖ)));break;case ʖ.N:ȭ.Add(ˀ
[29].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+ɗ)));ʟ.Clear();T(ʀ[Ë].ç.Ĕ,ref ʟ,ɂ,ʈ);ȭ.Add(ʟ[0].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+ɖ)));break;case ʖ.Ň:ȭ.Add(ˀ[29].ĝ(
ə.ú,ə.ù+ə.ú*(ɲ+ɗ)));ʟ.Clear();if(Ë==ʶ&&ʀ[Ë].ç.Ė){ȭ.Add(ˀ[20].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+Ȣ)));ȭ.Add(ˀ[21].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+Ȩ)));}if(
!ʀ[Ë].ç.ĕ){T(ʀ[Ë].ç.Ę.ToString(ʀ[Ë].ç.ė),ref ʟ,ɂ,ʈ);}else{if(ʀ[Ë].ç.ą){É(ʀ[Ë].ç.ò,ʀ[Ë].ç.ò,ʀ[Ë].ç.Ĕ,ʀ[Ë].ç.ė,Ë,ref ʟ,"%")
;}else{string ɭ="";string ɬ=ʀ[Ë].ç.ė;float ɫ=Ņ(ʀ[Ë].ç.Ę,ref ɬ,ref ɭ);É(ɫ,ʀ[Ë].ç.ò,ʀ[Ë].ç.Ĕ,ɬ,Ë,ref ʟ,ɭ);}ȭ.Add(ʟ[1].ĝ(ə.ú
,ə.ù+ə.ú*(ɲ+ɓ)));}ȭ.Add(ʟ[0].ĝ(ə.ú,ə.ù+ə.ú*(ɲ+ɖ)));break;}ɲ.Y=ɲ.Y+Ȟ;}ȭ.Add(ˀ[5].ĝ(ə.ú,ə.ø+ə.ú*ɐ));ȭ.Add(ˀ[5].ĝ(ə.ú,ə.ø+ə.
ú*ɏ));if(ʵ>0){ȭ.Add(ˀ[7].ĝ(ə.ú,ə.ù+ə.ú*Ɍ));ȭ.Add(ˀ[2].ĝ(ə.ú,ə.ù+ə.ú*Ɍ));}if(ʭ[ʳ].Count>ʋ-1&&ʵ!=ʴ){ȭ.Add(ˀ[6].ĝ(ə.ú,ə.ù+ə.
ú*ɋ));ȭ.Add(ˀ[1].ĝ(ə.ú,ə.ù+ə.ú*ɋ));}if(ɴ>=0){if(Ț[ɴ].Ƅ&&ί)ȭ.Add(ˀ[30].ĝ(ə.ú,ə.ö+ə.ú*ɉ));}if(ʎ){Vector2 ɪ=ə.ù+ə.ú*Ȝ;ȭ.Add(
ˀ[31].ĝ(ə.ú,ɪ+ə.ú*ɱ));ȭ.Add(ˀ[32].ĝ(ə.ú,ɪ+ə.ú*ɱ));ȭ.Add(ˀ[33].ĝ(ə.ú,ɪ+ə.ú*ɱ));ȭ.Add(ˀ[22].ĝ(ə.ú,ɪ+ə.ú*(ɱ-Ȧ)));ȭ.Add(ˀ[23]
.ĝ(ə.ú,ɪ+ə.ú*(ɱ+Ȧ)));ȭ.Add(ˀ[34+ʰ].ĝ(ə.ú,ɪ+ə.ú*(ɱ+ȧ)));}ʾ.Clear();T(ʺ,ref ʾ,Ⱦ,ʉ,TextAlignment.LEFT);ȭ.Add(ʾ[0].ĝ(ə.ú,ə.ù+
ə.ú*ȵ));T(ʹ,ref ʾ,ɂ,ʉ,TextAlignment.LEFT);ȭ.Add(ʾ[1].ĝ(ə.ú,ə.ù+ə.ú*Ɉ));}void ɩ(){ɣ();ʎ=false;ʶ--;if(ʶ<ʴ)--ʵ;if(ʶ<0)ʶ=0;if
(ʵ<0)ʵ=0;}void ɨ(){ɣ();ʎ=false;ʶ++;if(ʶ-ʵ>ʋ-1)++ʵ;if(ʶ>ʭ[ʳ].Count-1)ʶ=ʭ[ʳ].Count-1;if(ʵ>ʴ)ʵ=ʴ;}void ɮ(){ɣ();if(ʎ){ʰ--;if(
ʰ<0)ʰ=2;}else{Τ ȱ=Τ.Ȁ;ě.Ě ɧ=new ě.Ě();switch(ʭ[ʳ][ʶ].ñ){case ʊ.ʝ:ɜ(ʭ[ʳ][ʶ].ê,ʻ+ʭ[ʳ][ʶ].é,ʭ[ʳ][ʶ].è,ref ȱ,ref ɧ,ɞ.ͳ);break
;case ʊ.ʛ:ɤ(ʭ[ʳ][ʶ].ê,ʻ+ʭ[ʳ][ʶ].é,ʭ[ʳ][ʶ].è,ref ȱ,ref ɧ,ɞ.ͳ);break;}}}void ʁ(){ɣ();if(ʎ){ʰ++;if(ʰ>2)ʰ=0;}else{Τ ȱ=Τ.Ȁ;ě.Ě
ɧ=new ě.Ě();switch(ʭ[ʳ][ʶ].ñ){case ʊ.ʝ:ɜ(ʭ[ʳ][ʶ].ê,ʻ+ʭ[ʳ][ʶ].é,ʭ[ʳ][ʶ].è,ref ȱ,ref ɧ,ɞ.Ͳ);break;case ʊ.ʛ:ɤ(ʭ[ʳ][ʶ].ê,ʻ+ʭ[
ʳ][ʶ].é,ʭ[ʳ][ʶ].è,ref ȱ,ref ɧ,ɞ.Ͳ);break;}}}void ɿ(){ɣ();ʎ=false;ʳ--;ʶ=0;ʵ=0;if(ʳ<0)ʳ=0;ʴ=Math.Max(ʭ[ʳ].Count-ʋ,0);}void
ɾ(){ɣ();ʎ=false;ʳ++;ʶ=0;ʵ=0;if(ʳ>ʷ-1)ʳ=ʷ-1;ʴ=Math.Max(ʭ[ʳ].Count-ʋ,0);}void ɽ(){ɣ();ɼ(ʭ[ʳ][ʶ]);}void ɼ(ě ɶ){if(ɶ.ê=="run"
){if(ɶ.è=="0")ɻ(ɶ.é,false,true);else{if(!Ə.ů(ʨ,ɶ.é))ʨ.Add(new Ə(ʆ,ɶ.é,TimeSpan.FromSeconds(0),TimeSpan.FromSeconds(
Convert.ToSingle(ɶ.è)),false,ɻ));}}else{ɷ(ɶ);if(ɶ.æ>=0)ɻ(ɶ.æ.ToString(),false,true);}}void ɻ(string ɺ,bool ɹ,bool ɸ){if(ɸ){try{
int L;if(Int32.TryParse(ɺ,out L)){for(int Ë=0;Ë<ʧ[L].Count;Ë++){if(ʧ[L][Ë].ê=="run")ɼ(ʧ[L][Ë]);else ɷ(ʧ[L][Ë]);}}}catch{ʺ=
"Error run Prg: "+ɺ;}}}void ɷ(ě ɶ){Τ ȱ=Τ.Ȁ;ě.Ě ɧ=new ě.Ě();ɚ(ɶ,ref ȱ,ref ɧ,ɞ.ʹ);}void ɣ(){ʺ="";ʱ=-1;}void ɚ(ě ɢ,ref Τ ɡ,ref ě.Ě ā,ɞ ɞ=ɞ.Ͷ
){switch(ɢ.ñ){case ʊ.Ǡ:ɠ(ɢ.ê,ʻ+ɢ.é,ɢ.è,ref ɡ,ɞ);break;case ʊ.ʝ:ɜ(ɢ.ê,ʻ+ɢ.é,ɢ.è,ref ɡ,ref ā,ɞ);break;case ʊ.œ:ɥ(ɢ.ê,ʻ+ɢ.é,
ɢ.è,ref ɡ,ɞ);break;case ʊ.ʛ:ɤ(ɢ.ê,ʻ+ɢ.é,ɢ.è,ref ɡ,ref ā,ɞ);break;case ʊ.ʚ:ϖ(ʻ+ɢ.é,ɢ.è,ref ɡ,ref ā,ɞ);break;case ʊ.Ǭ:ϑ(ɢ.é
,ɢ.è,ref ɡ,ref ā,ɞ);break;case ʊ.ʙ:ϔ(ʻ+ɢ.é,ɢ.è,ref ɡ,ɞ);break;case ʊ.ʜ:ϒ(ɢ.é,ref ɡ);break;case ʊ.ʘ:ϓ(ɢ.ê,ɢ.é,ɢ.è,ref ɡ,ɞ)
;break;case ʊ.ʗ:ς(ɢ.ê,ɢ.é,ɢ.è,ref ɡ,ref ā);break;default:ʺ="Error in command in ["+ʳ.ToString()+"]: "+ɢ.ê;break;}}void ɠ(
string ȴ,string ɛ,string Ɇ,ref Τ ɟ,ɞ ɞ=ɞ.Ͷ){bool ɝ=true;if(ȴ=="bl")ɝ=false;if(Ĩ(ref έ,ɛ,ɝ)){if(ɞ==ɞ.ʹ){switch(Ɇ){case"on":(έ
as IMyFunctionalBlock).Enabled=true;break;case"off":(έ as IMyFunctionalBlock).Enabled=false;break;case"onoff":(έ as
IMyFunctionalBlock).Enabled=!(έ as IMyFunctionalBlock).Enabled;break;default:ʺ="Error in action: "+Ɇ;break;}}if((έ as IMyFunctionalBlock).
Enabled)ɟ=Τ.Σ;}else if(ɞ==ɞ.ʹ)ʺ="Error Finding Block: "+ɛ;}void ɜ(string ȴ,string ɛ,string Ɇ,ref Τ ɟ,ref ě.Ě Ň,ɞ ɞ=ɞ.Ͷ){if(Ĩ(
ref έ,ɛ,true)){switch(ȴ){case"antenna":Ň.Ę=Ϯ(έ as IMyRadioAntenna,Ɇ,ɞ);break;case"beacon":Ň.Ę=ϭ(έ as IMyBeacon,Ɇ,ɞ);break;
case"batt":if(ʎ&&ɞ==ɞ.ʹ){ʎ=false;Ɇ=ʍ[ʰ];}ɟ=Ϭ(έ as IMyBatteryBlock,Ɇ,ɞ);break;case"door":ɟ=ϝ(έ as IMyDoor,Ɇ,ɞ);break;case
"piston_att":ɟ=Ϝ(έ as IMyPistonBase,Ɇ,ɞ);break;case"piston_vel":Ň.Ę=ϙ(έ as IMyPistonBase,Ɇ,ɞ);break;case"rotor_att":ɟ=Ϡ(έ as
IMyMotorStator,Ɇ,ɞ);break;case"rotor_lock":ɟ=ϡ(έ as IMyMotorStator,Ɇ,ɞ);break;case"rotor_vel":Ň.Ę=ϟ(έ as IMyMotorStator,Ɇ,ɞ);break;
case"conn_lock":ɟ=Ϟ(έ as IMyShipConnector,Ɇ,ɞ);break;case"conn_collect":ɟ=ν(έ as IMyShipConnector,Ɇ,ɞ);break;case
"conn_throw":ɟ=μ(έ as IMyShipConnector,Ɇ,ɞ);break;case"grav":Ň.Ę=λ(έ as IMyGravityGeneratorBase,Ɇ,ɞ);break;case"jump":ɟ=κ(έ as
IMyJumpDrive,Ɇ,ɞ);break;case"jump_dist":ρ(έ as IMyJumpDrive,Ɇ,ref Ň,ɞ);break;case"light":Ň.Ĕ=ξ(έ as IMyLightingBlock,Ɇ,ɞ);break;case
"lgear_auto":ɟ=π(έ as IMyLandingGear,Ɇ,ɞ);break;case"lgear_lock":ɟ=ο(έ as IMyLandingGear,Ɇ,ɞ);break;case"sorter_drain":ɟ=δ(έ as
IMyConveyorSorter,Ɇ,ɞ);break;case"sorter_list":ɟ=α(έ as IMyConveyorSorter,Ɇ,ɞ);break;case"sound":Ň.Ĕ=θ(έ as IMySoundBlock,Ɇ,ɞ);break;case
"stockpile":ɟ=η(έ as IMyGasTank,Ɇ,ɞ);break;case"timer":ɟ=ζ(έ as IMyTimerBlock,Ɇ,ɞ);break;case"timer_delay":Ň.Ę=ϋ(έ as IMyTimerBlock
,Ɇ,ɞ);break;case"turret":ɟ=ϊ(έ as IMyLargeTurretBase,Ɇ,ɞ);break;case"vent":ɟ=ω(έ as IMyAirVent,Ɇ,ɞ);break;case"remote":ɟ=
ψ(έ as IMyRemoteControl,Ɇ,ɞ);break;case"waypoint":Ň.Ĕ=ό(έ as IMyRemoteControl,Ɇ,ɞ);break;case"write":Ň.Ĕ=ώ(έ,Ɇ,ɞ);break;
case"show":τ(έ,Ɇ,ref Ň,ɞ);break;}}else if(ɞ==ɞ.ʹ)ʺ="Error Finding Block: "+ɛ;}void ɥ(string ȴ,string ɛ,string Ɇ,ref Τ ɟ,ɞ ɞ=
ɞ.Ͷ){if(ŕ(ref ΰ,ɛ)){ΰ.GetBlocks(Ύ);if(ɞ==ɞ.ʹ){bool ɦ=false;if((Ύ[0]as IMyFunctionalBlock).Enabled)ɦ=true;switch(Ɇ){case
"on":foreach(IMyTerminalBlock Ǡ in Ύ){(Ǡ as IMyFunctionalBlock).Enabled=true;}break;case"off":foreach(IMyTerminalBlock Ǡ in
Ύ){(Ǡ as IMyFunctionalBlock).Enabled=false;}break;case"onoff":if(ɦ)foreach(IMyTerminalBlock Ǡ in Ύ){(Ǡ as
IMyFunctionalBlock).Enabled=false;}else foreach(IMyTerminalBlock Ǡ in Ύ){(Ǡ as IMyFunctionalBlock).Enabled=true;}break;default:ʺ=
"Error in action: "+Ɇ;break;}}if((Ύ[0]as IMyFunctionalBlock).Enabled)ɟ=Τ.Σ;}else if(ɞ==ɞ.ʹ)ʺ="Error Finding Group: "+ɛ;}void ɤ(string ȴ,
string ɛ,string Ɇ,ref Τ ɟ,ref ě.Ě Ň,ɞ ɞ=ɞ.Ͷ){if(ŕ(ref ΰ,ɛ)){switch(ȴ){case"gr-airlock":ΰ.GetBlocksOfType<IMyDoor>(Ύ);if(ϕ(Ύ,ȴ,
ɛ,1)){ɟ=Ϙ(ɛ,Ύ[0]as IMyDoor,Ύ[1]as IMyDoor,Ɇ,ɞ);}break;case"gr-antenna":ΰ.GetBlocksOfType<IMyRadioAntenna>(Ύ);if(ϕ(Ύ,ȴ,ɛ))
{foreach(IMyTerminalBlock ô in Ύ)Ϯ(ô as IMyRadioAntenna,Ɇ,ɞ);Ň.Ę=Ϯ(Ύ[0]as IMyRadioAntenna,Ɇ);}break;case"gr-beacon":ΰ.
GetBlocksOfType<IMyBeacon>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ϭ(ô as IMyBeacon,Ɇ,ɞ);Ň.Ę=ϭ(Ύ[0]as IMyBeacon,Ɇ);}break;case
"gr-batt":ΰ.GetBlocksOfType<IMyBatteryBlock>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){if(ʎ&&ɞ==ɞ.ʹ){ʎ=false;Ɇ=ʍ[ʰ];}foreach(IMyTerminalBlock ô in Ύ)Ϭ(ô as
IMyBatteryBlock,Ɇ,ɞ);ɟ=Ϭ(Ύ[0]as IMyBatteryBlock,Ɇ);}break;case"gr-door":ΰ.GetBlocksOfType<IMyDoor>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(
IMyTerminalBlock ô in Ύ)ɟ=ϝ(ô as IMyDoor,Ɇ,ɞ);ɟ=ϝ(Ύ[0]as IMyDoor,Ɇ);}break;case"gr-piston_att":ΰ.GetBlocksOfType<IMyPistonBase>(Ύ);if(ϕ(
Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)Ϝ(ô as IMyPistonBase,Ɇ,ɞ);ɟ=Ϝ(Ύ[0]as IMyPistonBase,Ɇ);}break;case"gr-piston_vel":
ΰ.GetBlocksOfType<IMyPistonBase>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ϙ(ô as IMyPistonBase,Ɇ,ɞ);Ň.Ę=ϙ(Ύ[0]as
IMyPistonBase,Ɇ);}break;case"gr-rotor_att":ΰ.GetBlocksOfType<IMyMotorStator>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)Ϡ(ô as
IMyMotorStator,Ɇ,ɞ);ɟ=Ϡ(Ύ[0]as IMyMotorStator,Ɇ);}break;case"gr-rotor_lock":ΰ.GetBlocksOfType<IMyMotorStator>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(
IMyTerminalBlock ô in Ύ)ϡ(ô as IMyMotorStator,Ɇ,ɞ);ɟ=ϡ(Ύ[0]as IMyMotorStator,Ɇ);}break;case"gr-rotor_vel":ΰ.GetBlocksOfType<
IMyMotorStator>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ϟ(ô as IMyMotorStator,Ɇ,ɞ);Ň.Ę=ϟ(Ύ[0]as IMyMotorStator,Ɇ);}break;case
"gr-conn_lock":ΰ.GetBlocksOfType<IMyShipConnector>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)Ϟ(ô as IMyShipConnector,Ɇ,ɞ);ɟ=Ϟ(Ύ[
0]as IMyShipConnector,Ɇ);}break;case"gr-conn_collect":ΰ.GetBlocksOfType<IMyShipConnector>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(
IMyTerminalBlock ô in Ύ)ν(ô as IMyShipConnector,Ɇ,ɞ);ɟ=ν(Ύ[0]as IMyShipConnector,Ɇ);}break;case"gr-conn_throw":ΰ.GetBlocksOfType<
IMyShipConnector>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)μ(ô as IMyShipConnector,Ɇ,ɞ);ɟ=μ(Ύ[0]as IMyShipConnector,Ɇ);}break;
case"gr-grav":ΰ.GetBlocksOfType<IMyGravityGeneratorBase>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)λ(ô as
IMyGravityGeneratorBase,Ɇ,ɞ);Ň.Ę=λ(Ύ[0]as IMyGravityGeneratorBase,Ɇ);}break;case"gr-jump":ΰ.GetBlocksOfType<IMyJumpDrive>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){
foreach(IMyTerminalBlock ô in Ύ)κ(ô as IMyJumpDrive,Ɇ,ɞ);ɟ=κ(Ύ[0]as IMyJumpDrive,Ɇ);}break;case"gr-jump_dist":ΰ.GetBlocksOfType
<IMyJumpDrive>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ρ(ô as IMyJumpDrive,Ɇ,ref Ň,ɞ);ρ(Ύ[0]as IMyJumpDrive,Ɇ,ref
Ň);}break;case"gr-light":ΰ.GetBlocksOfType<IMyLightingBlock>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ξ(ô as
IMyLightingBlock,Ɇ,ɞ);Ň.Ĕ=ξ(Ύ[0]as IMyLightingBlock,Ɇ);}break;case"gr-lgear_auto":ΰ.GetBlocksOfType<IMyLandingGear>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){
foreach(IMyTerminalBlock ô in Ύ)π(ô as IMyLandingGear,Ɇ,ɞ);ɟ=π(Ύ[0]as IMyLandingGear,Ɇ);}break;case"gr-lgear_lock":ΰ.
GetBlocksOfType<IMyLandingGear>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ο(ô as IMyLandingGear,Ɇ,ɞ);ɟ=ο(Ύ[0]as IMyLandingGear,Ɇ)
;}break;case"gr-sorter_drain":ΰ.GetBlocksOfType<IMyConveyorSorter>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)δ(ô as
IMyConveyorSorter,Ɇ,ɞ);ɟ=δ(Ύ[0]as IMyConveyorSorter,Ɇ);}break;case"gr-sorter_list":ΰ.GetBlocksOfType<IMyConveyorSorter>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){
foreach(IMyTerminalBlock ô in Ύ)α(ô as IMyConveyorSorter,Ɇ,ɞ);ɟ=α(Ύ[0]as IMyConveyorSorter,Ɇ);}break;case"gr-sound":ΰ.
GetBlocksOfType<IMySoundBlock>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)θ(ô as IMySoundBlock,Ɇ,ɞ);Ň.Ĕ=θ(Ύ[0]as IMySoundBlock,Ɇ);
}break;case"gr-stockpile":ΰ.GetBlocksOfType<IMyGasTank>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)η(ô as IMyGasTank
,Ɇ,ɞ);ɟ=η(Ύ[0]as IMyGasTank,Ɇ);}break;case"gr-timer":ΰ.GetBlocksOfType<IMyTimerBlock>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(
IMyTerminalBlock ô in Ύ)ζ(ô as IMyTimerBlock,Ɇ,ɞ);ɟ=ζ(Ύ[0]as IMyTimerBlock,Ɇ);}break;case"gr-timer_delay":ΰ.GetBlocksOfType<
IMyTimerBlock>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ϋ(ô as IMyTimerBlock,Ɇ,ɞ);Ň.Ę=ϋ(Ύ[0]as IMyTimerBlock,Ɇ);}break;case
"gr-turret":ΰ.GetBlocksOfType<IMyLargeTurretBase>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ϊ(ô as IMyLargeTurretBase,Ɇ,ɞ);ɟ=
ϊ(Ύ[0]as IMyLargeTurretBase,Ɇ);}break;case"gr-vent":ΰ.GetBlocksOfType<IMyAirVent>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(
IMyTerminalBlock ô in Ύ)ω(ô as IMyAirVent,Ɇ,ɞ);ɟ=ω(Ύ[0]as IMyAirVent,Ɇ);}break;case"gr-remote":ΰ.GetBlocksOfType<IMyRemoteControl>(Ύ);if
(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ψ(ô as IMyRemoteControl,Ɇ,ɞ);ɟ=ψ(Ύ[0]as IMyRemoteControl,Ɇ);}break;case
"gr-waypoint":ΰ.GetBlocksOfType<IMyRemoteControl>(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ό(ô as IMyRemoteControl,Ɇ,ɞ);Ň.Ĕ=ό(
Ύ[0]as IMyRemoteControl,Ɇ);}break;case"gr-write":ΰ.GetBlocks(Ύ);if(ϕ(Ύ,ȴ,ɛ)){foreach(IMyTerminalBlock ô in Ύ)ώ(ô,Ɇ,ɞ);Ň.Ĕ
=ώ(Ύ[0],Ɇ);}break;case"gr-show":ΰ.GetBlocksOfType<IMyTerminalBlock>(Ύ);if(ϕ(Ύ,ȴ,ɛ))σ(Ύ,Ɇ,ref Ň,ɞ);break;}}else if(ɞ==ɞ.ʹ)
ʺ="Error Finding Group: "+ɛ;}void ϖ(string ɛ,string Ɇ,ref Τ ɟ,ref ě.Ě Ň,ɞ ɞ=ɞ.Ͷ){if(ŕ(ref ΰ,ɛ)){ΰ.GetBlocks(Ύ);if(ŕ(ref ή
,ʻ+Ɇ)){ή.GetBlocks(Ξ);ɟ=ϯ(ɞ);}}else if(ɞ==ɞ.ʹ)ʺ="Error Finding Groups: "+ɛ+" - "+Ɇ;}void ϔ(string ɛ,string γ,ref Τ ɟ,ɞ ɞ=
ɞ.Ͷ){if(ɞ==ɞ.ʹ){try{Β=GridTerminalSystem.GetBlockWithName(ɛ)as IMyProgrammableBlock;Β.TryRun(γ);ʹ=ɛ+" - "+γ;}catch{ʺ=
"Error run PB: "+ɛ+" with arg: "+γ;}}try{Β=GridTerminalSystem.GetBlockWithName(ɛ)as IMyProgrammableBlock;if(Β.Enabled)ɟ=Τ.ˍ;}catch{ʺ=
"Error PB: "+ɛ+" not found ";}}void ϒ(string ɺ,ref Τ ɟ){if(ʨ.Count==1)ɟ=Τ.ː;else if(ʨ.Count==2)ɟ=Τ.ˏ;else if(ʨ.Count>2)ɟ=Τ.ˬ;if(Ə.ů(
ʨ,ɺ))ɟ=Τ.Ρ;}void ϓ(string ȴ,string ɛ,string Ɇ,ref Τ ɟ,ɞ ɞ=ɞ.Ͷ){bool ϐ=true;if(ȴ=="*blk*")ϐ=false;if(Œ(ref Ύ,ɛ)){if(ɞ==ɞ.ʹ
){foreach(IMyTerminalBlock ô in Ύ){if(ϐ||ô.IsSameConstructAs(Me)){switch(Ɇ){case"on":(ô as IMyFunctionalBlock).Enabled=
true;break;case"off":(ô as IMyFunctionalBlock).Enabled=false;break;case"onoff":(ô as IMyFunctionalBlock).Enabled=!(ô as
IMyFunctionalBlock).Enabled;break;default:ʺ="Error in action: "+Ɇ;break;}}}}if(ϐ||Ύ[0].IsSameConstructAs(Me))if((Ύ[0]as IMyFunctionalBlock
).Enabled)ɟ=Τ.Σ;}}void ϑ(string ɛ,string Ɇ,ref Τ ɟ,ref ě.Ě Ň,ɞ ɞ=ɞ.Ͷ){if(Ϋ.Count>0){string[]Ĵ=ɛ.Split('/');try{int ŋ=ʌ[Ĵ[
0]];int ŏ=ʒ[Ĵ[1]];if(ά[ŋ,ŏ].Count>0){if(ɞ==ɞ.ʹ){bool ɦ=false;if((ά[ŋ,ŏ][0]as IMyFunctionalBlock).Enabled)ɦ=true;switch(Ɇ)
{case"on":foreach(IMyThrust Ǡ in ά[ŋ,ŏ]){(Ǡ as IMyFunctionalBlock).Enabled=true;}break;case"off":foreach(IMyThrust Ǡ in ά
[ŋ,ŏ]){(Ǡ as IMyFunctionalBlock).Enabled=false;}break;case"onoff":if(ɦ)foreach(IMyThrust Ǡ in ά[ŋ,ŏ]){(Ǡ as
IMyFunctionalBlock).Enabled=false;}else foreach(IMyThrust Ǡ in ά[ŋ,ŏ]){(Ǡ as IMyFunctionalBlock).Enabled=true;}break;default:ʺ=
"Error in action: "+Ɇ;break;}}if((ά[ŋ,ŏ][0]as IMyFunctionalBlock).Enabled)ɟ=Τ.Σ;}}catch{ʺ="Error in command: "+ɛ;}}else if(ɞ==ɞ.ʹ)ʺ=
"Thrusters not found";}bool ϕ(List<IMyTerminalBlock>Ġ,string ź,string ǈ,int ϗ=0){if(Ġ.Count>ϗ)return true;else{ʺ="Error, "+ź+" in group "+ǈ+
" not found";return false;}}Τ Ϙ(string Ĵ,IMyDoor Ϥ,IMyDoor Ϣ,string γ,ɞ β=ɞ.Ͷ){Τ ϩ;bool Ϩ=true;if(Ϥ.Status==DoorStatus.Closed||Ϥ.
Status==DoorStatus.Closing){ϩ=Τ.ˈ;if(Ϣ.Status==DoorStatus.Closed||Ϣ.Status==DoorStatus.Closing)ϩ=Τ.Ο;Ϩ=false;}else if(Ϣ.Status
==DoorStatus.Closed||Ϣ.Status==DoorStatus.Closing)ϩ=Τ.ˇ;else ϩ=Τ.Π;if(β==ɞ.ʹ){string[]Ɣ=γ.Split('/');if(Ɣ.Length>1){if(!Ə.
ů(ʨ,Ĵ))ʨ.Add(new Ə(ʆ,Ĵ,TimeSpan.FromSeconds(Convert.ToSingle(Ɣ[0])),TimeSpan.FromSeconds(Convert.ToSingle(Ɣ[1])),Ϩ,Ϫ));}
else ʺ="Error in action: "+γ;}return ϩ;}void Ϫ(string ɛ,bool ϧ,bool Ϧ){if(ŕ(ref ΰ,ɛ)){ΰ.GetBlocksOfType<IMyDoor>(Ύ);if(ϕ(Ύ,
"","",1)){if(!Ϧ)ϥ(Ύ[0]as IMyDoor,Ύ[1]as IMyDoor);else ϣ(Ύ[0]as IMyDoor,Ύ[1]as IMyDoor,ϧ);}}}void ϥ(IMyDoor Ϥ,IMyDoor Ϣ){Ϥ.
Enabled=true;Ϥ.CloseDoor();Ϣ.Enabled=true;Ϣ.CloseDoor();}void ϣ(IMyDoor Ϥ,IMyDoor Ϣ,bool ϫ){if(!ϫ){Ϥ.Enabled=true;Ϥ.OpenDoor();
Ϣ.Enabled=true;Ϣ.CloseDoor();Ϣ.Enabled=false;}else{Ϥ.Enabled=true;Ϥ.CloseDoor();Ϥ.Enabled=false;Ϣ.Enabled=true;Ϣ.OpenDoor
();}}Τ ϯ(ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){if((Ύ[0]as IMyFunctionalBlock).Enabled){foreach(IMyTerminalBlock ô in Ύ){(ô as
IMyFunctionalBlock).Enabled=false;}foreach(IMyTerminalBlock ô in Ξ){(ô as IMyFunctionalBlock).Enabled=true;}}else{foreach(IMyTerminalBlock
ô in Ύ){(ô as IMyFunctionalBlock).Enabled=true;}foreach(IMyTerminalBlock ô in Ξ){(ô as IMyFunctionalBlock).Enabled=false;
}}}if((Ύ[0]as IMyFunctionalBlock).Enabled)return Τ.ː;if((Ξ[0]as IMyFunctionalBlock).Enabled)return Τ.ˏ;return Τ.Ȁ;}float
Ϯ(IMyRadioAntenna ô,string γ,ɞ β=ɞ.Ͷ){if(β!=ɞ.Ͷ){string[]Ɣ=γ.Split('/');try{switch(β){case ɞ.ʹ:ô.Radius=Convert.ToSingle(
Ɣ[0]);break;case ɞ.ͳ:ô.Radius-=Convert.ToSingle(Ɣ[1]);break;case ɞ.Ͳ:ô.Radius+=Convert.ToSingle(Ɣ[1]);break;}}catch{ʺ=
"Error in action: "+γ;}}return ô.Radius;}float ϭ(IMyBeacon ô,string γ,ɞ β=ɞ.Ͷ){if(β!=ɞ.Ͷ){string[]Ɣ=γ.Split('/');try{switch(β){case ɞ.ʹ:ô.
Radius=Convert.ToSingle(Ɣ[0]);break;case ɞ.ͳ:ô.Radius-=Convert.ToSingle(Ɣ[1]);break;case ɞ.Ͳ:ô.Radius+=Convert.ToSingle(Ɣ[1]);
break;}}catch{ʺ="Error in action: "+γ;}}return ô.Radius;}Τ Ϭ(IMyBatteryBlock ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case
"auto":ô.ChargeMode=ChargeMode.Auto;break;case"recharge":ô.ChargeMode=ChargeMode.Recharge;break;case"discharge":ô.ChargeMode=
ChargeMode.Discharge;break;case"mode":if(!ʎ){ʰ=(int)ô.ChargeMode;ʎ=true;}break;}}switch(ô.ChargeMode){case ChargeMode.Auto:return
Τ.Ό;case ChargeMode.Recharge:return Τ.ˤ;case ChargeMode.Discharge:return Τ.ˆ;default:return Τ.Ȁ;}}Τ ϝ(IMyDoor ô,string γ,
ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case"open":ô.OpenDoor();break;case"close":ô.CloseDoor();break;case"toggle":ô.ToggleDoor();
break;default:ʺ="Error in action: "+γ;break;}}if(ô.Status==DoorStatus.Closed)return Τ.Ο;else return Τ.Π;}Τ Ϝ(IMyPistonBase ô,
string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){string[]Ɣ=γ.Split('/');switch(Ɣ[0]){case"attach":ô.Attach();break;case"detach":ô.Detach();break;
case"toggle":if(ô.IsAttached)ô.Detach();else ô.Attach();break;default:if(Ɣ.Length>1){float ϛ,Ϛ;if(float.TryParse(Ɣ[0],out ϛ)
&&float.TryParse(Ɣ[1],out Ϛ)){ô.MinLimit=ϛ;ô.MaxLimit=Ϛ;}else ʺ="Error in action: "+γ;}else ʺ="Error in action: "+γ;break;
}}if(ô.IsAttached)return Τ.ˢ;else return Τ.ˡ;}float ϙ(IMyPistonBase ô,string γ,ɞ β=ɞ.Ͷ){if(β!=ɞ.Ͷ){string[]Ɣ=γ.Split('/')
;try{switch(β){case ɞ.ʹ:if(Ɣ[0]=="reverse")ô.Velocity=-1*ô.Velocity;else ô.Velocity=Convert.ToSingle(Ɣ[0]);break;case ɞ.ͳ
:if(Ɣ[1]=="set")ô.Velocity=-Convert.ToSingle(Ɣ[2]);else ô.Velocity-=Convert.ToSingle(Ɣ[1]);break;case ɞ.Ͳ:if(Ɣ[1]=="set")
ô.Velocity=Convert.ToSingle(Ɣ[2]);else ô.Velocity+=Convert.ToSingle(Ɣ[1]);break;}}catch{ʺ="Error in action: "+γ;}}return
ô.Velocity;}Τ Ϡ(IMyMotorStator ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){string[]Ɣ=γ.Split('/');switch(Ɣ[0]){case"attach":ô.Attach()
;break;case"detach":ô.Detach();break;case"toggle":if(ô.IsAttached)ô.Detach();else ô.Attach();break;default:if(Ɣ.Length>1)
{float ϛ,Ϛ;if(float.TryParse(Ɣ[0],out ϛ)&&float.TryParse(Ɣ[1],out Ϛ)){ô.LowerLimitDeg=ϛ;ô.UpperLimitDeg=Ϛ;}else ʺ=
"Error in action: "+γ;}else ʺ="Error in action: "+γ;break;}}if(ô.IsAttached)return Τ.ˢ;else return Τ.ˡ;}Τ ϡ(IMyMotorStator ô,string γ,ɞ β=ɞ
.Ͷ){if(β==ɞ.ʹ){switch(γ){case"lock":ô.RotorLock=true;break;case"unlock":ô.RotorLock=false;break;case"toggle":ô.RotorLock=
!ô.RotorLock;break;default:ʺ="Error in action: "+γ;break;}}if(ô.RotorLock)return Τ.ˠ;else return Τ.ˑ;}float ϟ(
IMyMotorStator ô,string γ,ɞ β=ɞ.Ͷ){if(β!=ɞ.Ͷ){string[]Ɣ=γ.Split('/');try{switch(β){case ɞ.ʹ:if(Ɣ[0]=="reverse")ô.TargetVelocityRPM=-1*
ô.TargetVelocityRPM;else ô.TargetVelocityRPM=Convert.ToSingle(Ɣ[0]);break;case ɞ.ͳ:if(Ɣ[1]=="set")ô.TargetVelocityRPM=-
Convert.ToSingle(Ɣ[2]);else ô.TargetVelocityRPM-=Convert.ToSingle(Ɣ[1]);break;case ɞ.Ͳ:if(Ɣ[1]=="set")ô.TargetVelocityRPM=
Convert.ToSingle(Ɣ[2]);else ô.TargetVelocityRPM+=Convert.ToSingle(Ɣ[1]);break;}}catch{ʺ="Error in action: "+γ;}}return ô.
TargetVelocityRPM;}Τ Ϟ(IMyShipConnector ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case"lock":ô.Connect();break;case"unlock":ô.Disconnect()
;break;case"toggle":ô.ToggleConnect();break;default:ʺ="Error in action: "+γ;break;}}switch(ô.Status){case
MyShipConnectorStatus.Connectable:return Τ.ˌ;case MyShipConnectorStatus.Connected:return Τ.ˠ;case MyShipConnectorStatus.Unconnected:return Τ.
ˑ;}return Τ.Ȁ;}Τ ν(IMyShipConnector ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case"on":ô.CollectAll=true;break;case"off":ô
.CollectAll=false;break;case"toggle":ô.CollectAll=!ô.CollectAll;break;}}if(ô.CollectAll)return Τ.ˣ;else return Τ.Ȁ;}Τ μ(
IMyShipConnector ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case"on":ô.ThrowOut=true;break;case"off":ô.ThrowOut=false;break;case"toggle":ô
.ThrowOut=!ô.ThrowOut;break;default:ʺ="Error in action: "+γ;break;}}if(ô.ThrowOut)return Τ.ˎ;else return Τ.Ȁ;}float λ(
IMyGravityGeneratorBase ô,string γ,ɞ β=ɞ.Ͷ){if(β!=ɞ.Ͷ){string[]Ɣ=γ.Split('/');try{switch(β){case ɞ.ʹ:ô.GravityAcceleration=Convert.ToSingle(Ɣ[0
]);break;case ɞ.ͳ:if(Ɣ[1]=="set")ô.GravityAcceleration=-Convert.ToSingle(Ɣ[2]);else ô.GravityAcceleration-=Convert.
ToSingle(Ɣ[1]);break;case ɞ.Ͳ:if(Ɣ[1]=="set")ô.GravityAcceleration=Convert.ToSingle(Ɣ[2]);else ô.GravityAcceleration+=Convert.
ToSingle(Ɣ[1]);break;}}catch{ʺ="Error in action: "+γ;}}return ô.GravityAcceleration;}Τ κ(IMyJumpDrive ô,string γ,ɞ β=ɞ.Ͷ){if(β==
ɞ.ʹ){switch(γ){case"on":ô.ApplyAction("Recharge_On");break;case"off":ô.ApplyAction("Recharge_Off");break;case"toggle":ô.
ApplyAction("Recharge");break;default:ʺ="Error in action: "+γ;break;}}if(ô.GetValue<bool>("Recharge"))return Τ.ˋ;else return Τ.Ȁ;}
void ρ(IMyJumpDrive ô,string γ,ref ě.Ě ģ,ɞ β=ɞ.Ͷ){if(β!=ɞ.Ͷ){string[]Ɣ=γ.Split('/');try{switch(β){case ɞ.ʹ:ô.SetValue<float>
("JumpDistance",Convert.ToSingle(Ɣ[0]));break;case ɞ.ͳ:if(Ɣ.Length>1)ô.SetValue<float>("JumpDistance",ô.GetValue<float>(
"JumpDistance")-Convert.ToSingle(Ɣ[1]));else ô.ApplyAction("DecreaseJumpDistance");break;case ɞ.Ͳ:if(Ɣ.Length>1)ô.SetValue<float>(
"JumpDistance",ô.GetValue<float>("JumpDistance")+Convert.ToSingle(Ɣ[1]));else ô.ApplyAction("IncreaseJumpDistance");break;}}catch{ʺ=
"Error in action: "+γ;}}Ν=true;ģ.Ę=ł(ģ.ò);ģ.ò=ô.GetValue<float>("JumpDistance");}Τ π(IMyLandingGear ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ
){case"on":ô.AutoLock=true;break;case"off":ô.AutoLock=false;break;case"toggle":ô.AutoLock=!ô.AutoLock;break;default:ʺ=
"Error in action: "+γ;break;}}if(ô.AutoLock)return Τ.Ό;else return Τ.Ȁ;}Τ ο(IMyLandingGear ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case
"lock":ô.Lock();break;case"unlock":ô.Unlock();break;case"toggle":ô.ToggleLock();break;default:ʺ="Error in action: "+γ;break;}}
switch(ô.LockMode){case LandingGearMode.ReadyToLock:return Τ.ˌ;case LandingGearMode.Locked:return Τ.ˠ;case LandingGearMode.
Unlocked:return Τ.ˑ;}return Τ.Ȁ;}string ξ(IMyLightingBlock ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){string[]Ɣ=γ.Split('/');try{for(int Ë=0
;Ë<Ɣ.Length;Ë++){if(ã(Ɣ[Ë],@"radius=[\d.]+,[\d.]+")){System.Text.RegularExpressions.Match Ä=á(Ɣ[Ë],@"[\d.]+,[\d.]+");
string[]Ò=Ä.Value.Split(',');float Ø,Ñ;if(float.TryParse(Ò[0],out Ø)&&float.TryParse(Ò[1],out Ñ)){ô.Radius=Ø;ô.Intensity=Ñ;}
else{ʺ="Error in radius,intensity: "+Ɣ[Ë];}}else if(ã(Ɣ[Ë],@"blink=[\d.]+,\d+")){System.Text.RegularExpressions.Match Ä=á(Ɣ[
Ë],@"[\d.]+,\d+");string[]Ò=Ä.Value.Split(',');float Ø;byte Ñ;if(float.TryParse(Ò[0],out Ø)&&byte.TryParse(Ò[1],out Ñ)){ô
.BlinkIntervalSeconds=Ø;ô.BlinkLength=Ñ;}else{ʺ="Error in blink: "+Ɣ[Ë];}}else ô.Color=Ù(Ɣ[Ë],"color=",ô.Color,ref ʺ);}}
catch{ʺ="Error in action: "+γ;}}string ε="";if(ô.BlinkIntervalSeconds>0&&ô.BlinkLength>0&&ô.BlinkLength<100)ε="*";return(ô.
Color.R/27).ToString()+(ô.Color.G/27).ToString()+(ô.Color.B/27).ToString()+ε;}Τ δ(IMyConveyorSorter ô,string γ,ɞ β=ɞ.Ͷ){if(β
==ɞ.ʹ){switch(γ){case"on":ô.DrainAll=true;break;case"off":ô.DrainAll=false;break;case"toggle":ô.DrainAll=!ô.DrainAll;break
;default:ʺ="Error in action: "+γ;break;}}if(ô.DrainAll)return Τ.ͽ;else return Τ.Ȁ;}Τ α(IMyConveyorSorter ô,string γ,ɞ β=ɞ
.Ͷ){if(β==ɞ.ʹ){string[]Ɣ=γ.Split('/');MyDefinitionId Ɛ=new MyDefinitionId();Υ.Clear();if(Ɣ.Length>1){string[]ι=Ɣ[1].Split
(',');for(int Ë=0;Ë<ι.Length;Ë++){Echo("i = "+Ë.ToString());Echo(ι[Ë]);ι[Ë]=ι[Ë].Replace("-","/");Echo(ι[Ë]);ι[Ë]=
"MyObjectBuilder_"+ι[Ë];Echo(ι[Ë]);MyDefinitionId.TryParse(ι[Ë],out Ɛ);Υ.Add(Ɛ);}}if(Ɣ[0]=="black")ô.SetFilter(MyConveyorSorterMode.
Blacklist,Υ);else if(Ɣ[0]=="white")ô.SetFilter(MyConveyorSorterMode.Whitelist,Υ);else ʺ="Error in action: "+γ;}if(ô.Mode==
MyConveyorSorterMode.Blacklist)return Τ.ͼ;else return Τ.ͻ;}string θ(IMySoundBlock ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){string[]Ɣ=γ.Split('/');try{
if(Ɣ.Length>2)ô.LoopPeriod=Convert.ToSingle(Ɣ[2]);if(Ɣ.Length>1)ô.SelectedSound=Ɣ[1];if(Ɣ[0]=="play")ô.Play();if(Ɣ[0]==
"stop")ô.Stop();}catch{ʺ="Error in action: "+γ;}}string ε=ô.SelectedSound;if(ε.Length>6)ε=ε.Remove(0,ε.Length-6);return ε;}Τ η
(IMyGasTank ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case"on":ô.Stockpile=true;break;case"off":ô.Stockpile=false;break;
case"toggle":ô.Stockpile=!ô.Stockpile;break;default:ʺ="Error in action: "+γ;break;}}if(ô.Stockpile)return Τ.Ά;else return Τ.
Ȁ;}Τ ζ(IMyTimerBlock ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){case"trigger":ô.Trigger();break;case"start":ô.
StartCountdown();break;case"stop":ô.StopCountdown();break;case"toggle":if(ô.IsCountingDown)ô.StopCountdown();else ô.StartCountdown();
break;default:ʺ="Error in action: "+γ;break;}}if(ô.IsCountingDown)return Τ.Ρ;else return Τ.Ȁ;}float ϋ(IMyTimerBlock ô,string
γ,ɞ β=ɞ.Ͷ){if(β!=ɞ.Ͷ){string[]Ɣ=γ.Split('/');try{switch(β){case ɞ.ʹ:ô.TriggerDelay=Convert.ToSingle(Ɣ[0]);break;case ɞ.ͳ:
ô.TriggerDelay=ô.TriggerDelay-Convert.ToSingle(Ɣ[1]);break;case ɞ.Ͳ:ô.TriggerDelay=ô.TriggerDelay+Convert.ToSingle(Ɣ[1]);
break;}}catch{ʺ="Error in action: "+γ;}}return ô.TriggerDelay;}Τ ϊ(IMyLargeTurretBase ô,string γ,ɞ β=ɞ.Ͷ){switch(γ){case
"shoot":if(β==ɞ.ʹ)ô.ApplyAction("Shoot");if(ô.GetValue<bool>("Shoot"))return Τ.ˊ;break;case"tcharacters":if(β==ɞ.ʹ)ô.
ApplyAction("TargetCharacters");if(ô.GetValue<bool>("TargetCharacters"))return Τ.ˉ;break;case"tstations":if(β==ɞ.ʹ)ô.ApplyAction(
"TargetStations");if(ô.GetValue<bool>("TargetStations"))return Τ.ˉ;break;case"tlargeships":if(β==ɞ.ʹ)ô.ApplyAction("TargetLargeShips");
if(ô.GetValue<bool>("TargetLargeShips"))return Τ.ˉ;break;case"tsmallships":if(β==ɞ.ʹ)ô.ApplyAction("TargetSmallShips");if(
ô.GetValue<bool>("TargetSmallShips"))return Τ.ˉ;break;case"tmeteors":if(β==ɞ.ʹ)ô.ApplyAction("TargetMeteors");if(ô.
GetValue<bool>("TargetMeteors"))return Τ.ˉ;break;case"tmissiles":if(β==ɞ.ʹ)ô.ApplyAction("TargetMissiles");if(ô.GetValue<bool>(
"TargetMissiles"))return Τ.ˉ;break;case"tneutrals":if(β==ɞ.ʹ)ô.ApplyAction("TargetNeutrals");if(ô.GetValue<bool>("TargetNeutrals"))
return Τ.ˉ;break;default:ʺ="Error in action: "+γ;break;}return Τ.Ȁ;}Τ ω(IMyAirVent ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){switch(γ){
case"press":ô.Depressurize=false;break;case"depress":ô.Depressurize=true;break;case"toggle":ô.Depressurize=!ô.Depressurize;
break;default:ʺ="Error in action: "+γ;break;}}if(!ô.CanPressurize)return Τ.ͺ;switch(ô.Status){case VentStatus.Pressurized:
return Τ.ͷ;case VentStatus.Pressurizing:return Τ.Ί;}return Τ.Έ;}Τ ψ(IMyRemoteControl ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){ô.
FlightMode=FlightMode.OneWay;switch(γ){case"on":ô.SetAutoPilotEnabled(true);break;case"off":ô.SetAutoPilotEnabled(false);break;
case"toggle":ô.SetAutoPilotEnabled(!ô.IsAutoPilotEnabled);break;default:ʺ="Error in action: "+γ;break;}}if(ô.
IsAutoPilotEnabled)return Τ.Ρ;else return Τ.Ή;}string ό(IMyRemoteControl ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ.ʹ){string[]Ɣ=γ.Split(':');try{ô.
ClearWaypoints();double Ļ=Convert.ToDouble(Ɣ[2]);double Ƹ=Convert.ToDouble(Ɣ[3]);double ƹ=Convert.ToDouble(Ɣ[4]);ô.AddWaypoint(new
Vector3D(Ļ,Ƹ,ƹ),Ɣ[1]);}catch{ʺ="Error in action: "+γ;}}string ε="";List<MyWaypointInfo>Ϗ=new List<MyWaypointInfo>();ô.
GetWaypointInfo(Ϗ);if(Ϗ.Count>0)ε=Ϗ[0].Name;if(ε.Length>5)ε=ε.Remove(6);return ε;}string ώ(IMyTerminalBlock ô,string γ,ɞ β=ɞ.Ͷ){if(β==ɞ
.ʹ){string[]Ɣ=γ.Split('/');try{IMyTextSurface ķ;int ύ=0;Ù(Ɣ[0],"screen=",out ύ,ref ʺ);ķ=(ô as IMyTextSurfaceProvider)?.
GetSurface(ύ);if(ķ!=null)ķ.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;else{ʺ="Error, LCD surface not found";}
for(int Ë=0;Ë<Ɣ.Length;Ë++){if(ã(Ɣ[Ë],"clear"))ķ.WriteText(" ");else if(ã(Ɣ[Ë],@"addtext=[^\/;]+")){System.Text.
RegularExpressions.Match Ä=á(Ɣ[Ë],@"(?<=\=).[^\/;]+");ķ.WriteText(Ä.Value,true);}else if(ã(Ɣ[Ë],@"nltext=[^\/;]+")){System.Text.
RegularExpressions.Match Ä=á(Ɣ[Ë],@"(?<=\=).[^\/;]+");ķ.WriteText("\n"+Ä.Value,true);}else if(ã(Ɣ[Ë],@"text=[^\/;]+")){System.Text.
RegularExpressions.Match Ä=á(Ɣ[Ë],@"(?<=\=).[^\/;]+");ķ.WriteText(Ä.Value);}else{ķ.FontSize=Ù(Ɣ[Ë],"size=",ķ.FontSize,ref ʺ);ķ.FontColor=Ù
(Ɣ[Ë],"color=",ķ.FontColor,ref ʺ);ķ.BackgroundColor=Ù(Ɣ[Ë],"background=",ķ.BackgroundColor,ref ʺ);}}}catch{ʺ=
"Error in action: "+γ;}}return"txt";}void τ(IMyTerminalBlock ô,string γ,ref ě.Ě ģ,ɞ β=ɞ.Ͷ){switch(γ){case"batt":ģ.Ĕ="<";ģ.ą=true;χ(ô,ref ģ)
;break;case"inv":ģ.ą=true;į(ô,ref ģ);break;case"power":ģ.Ĕ=">";Ĭ(ô,ref ģ);break;case"gas":ģ.Ĕ="<";ģ.ą=true;Ĥ(ô,ref ģ);
break;case"jump":ģ.Ĕ="+";ģ.ą=true;Ħ(ô,ref ģ);break;default:ʺ="Error in action: "+γ;break;}}void σ(List<IMyTerminalBlock>ô,
string γ,ref ě.Ě ģ,ɞ β=ɞ.Ͷ){switch(γ){case"batt":ģ.Ĕ="<";ģ.ą=true;χ(ô,ref ģ);break;case"inv":ģ.Ĕ=">";ģ.ą=true;į(ô,ref ģ);break
;case"power":ģ.Ĕ=">";Ĭ(ô,ref ģ);break;case"gas":ģ.Ĕ="<";ģ.ą=true;Ĥ(ô,ref ģ);break;case"jump":ģ.Ĕ="+";ģ.ą=true;Ħ(ô,ref ģ);
break;default:ʺ="Error in action: "+γ;break;}}void ς(string ȴ,string ɛ,string Ɇ,ref Τ ɟ,ref ě.Ě ģ){string[]Ɣ=Ɇ.Split('/');try
{switch(Ɣ[0]){case"batt":ģ.Ĕ="<";ģ.ą=true;if(Ψ.Count>0)χ(Ψ,ref ģ);break;case"inv":ģ.Ĕ=">";ģ.ą=true;if(Ϊ.Count>0)į(Ϊ,ref ģ
);break;case"power":ģ.Ĕ=">";if(Χ.Count>0)Ĭ(Χ,ref ģ);break;case"gas":ģ.Ĕ="<";ģ.ą=true;if(Φ.Count>0)Ĥ(Φ,ref ģ);break;case
"jump":ģ.Ĕ="+";ģ.ą=true;if(Ω.Count>0)Ħ(Ω,ref ģ);break;case"mass":ģ.Ĕ=">";Ν=true;switch(Ɣ[1]){case"cargo":ģ.Ę=Η.PhysicalMass-Η.
BaseMass;ģ.ò=ò(Η.PhysicalMass-Η.BaseMass,float.Parse(Ɣ[2]));break;case"total":ģ.Ę=Η.PhysicalMass;ģ.ò=ò(Η.PhysicalMass,float.
Parse(Ɣ[2]));break;}break;case"grav":switch(Ɣ[1]){case"art":Κ=true;ģ.Ę=(float)Μ;ģ.ò=ò(Μ,9.8d);break;case"nat":Λ=true;ģ.Ę=(
float)Ζ;ģ.ò=ò(Ζ,9.8d);break;case"total":Ι=true;ģ.Ę=(float)Ε;ģ.ò=ò(Ε,9.8d);break;}break;case"weight":ģ.Ĕ=">";Ν=true;Λ=true;ģ.Ę
=(float)Ζ*Η.PhysicalMass;ģ.ò=ò(Ζ*Η.PhysicalMass,float.Parse(Ɣ[1]));break;default:ʺ="Error in command: "+Ɇ;break;}}catch{ʺ
="Error in action: "+Ɇ;}}void χ(IMyTerminalBlock ô,ref ě.Ě ģ){if(ô is IMyBatteryBlock){IMyBatteryBlock Ü=ô as
IMyBatteryBlock;ģ.ò=ò(Ü.CurrentStoredPower,Ü.MaxStoredPower);}}void χ(List<IMyTerminalBlock>Ġ,ref ě.Ě ģ){float φ=0;float υ=0;foreach(
IMyTerminalBlock ô in Ġ){if(ô is IMyBatteryBlock){IMyBatteryBlock Ü=ô as IMyBatteryBlock;φ+=Ü.MaxStoredPower;υ+=Ü.CurrentStoredPower;}}ģ
.ò=ò(υ,φ);}void χ(List<IMyBatteryBlock>Ġ,ref ě.Ě ģ){float φ=0;float υ=0;foreach(IMyBatteryBlock Ü in Ġ){φ+=Ü.
MaxStoredPower;υ+=Ü.CurrentStoredPower;}ģ.ò=ò(υ,φ);}void į(IMyTerminalBlock ô,ref ě.Ě ģ){if(ô.HasInventory){IMyInventory İ=ô.
GetInventory(0);ģ.ò=ò((float)İ.CurrentVolume,(float)İ.MaxVolume);}}void į(List<IMyTerminalBlock>Ġ,ref ě.Ě ģ){VRage.MyFixedPoint Į=0;
VRage.MyFixedPoint ĭ=0;foreach(IMyTerminalBlock ô in Ġ){if(ô.HasInventory){IMyInventory İ=ô.GetInventory(0);Į+=İ.MaxVolume;ĭ
+=İ.CurrentVolume;}}ģ.ò=ò((float)ĭ,(float)Į);}void į(List<IMyInventory>Ġ,ref ě.Ě ģ){VRage.MyFixedPoint Į=0;VRage.
MyFixedPoint ĭ=0;foreach(IMyInventory ô in Ġ){Į+=ô.MaxVolume;ĭ+=ô.CurrentVolume;}ģ.ò=ò((float)ĭ,(float)Į);}void Ĭ(IMyTerminalBlock ô
,ref ě.Ě ģ){if(ô is IMyPowerProducer){IMyPowerProducer Ü=ô as IMyPowerProducer;ģ.Ę=Ü.CurrentOutput;ģ.ò=ò(Ü.CurrentOutput,
Ü.MaxOutput);}}void Ĭ(List<IMyTerminalBlock>Ġ,ref ě.Ě ģ){float ĳ=0;float Ĳ=0;foreach(IMyTerminalBlock ô in Ġ){if(ô is
IMyPowerProducer){IMyPowerProducer Ü=ô as IMyPowerProducer;Ĳ+=Ü.CurrentOutput;ĳ+=Ü.MaxOutput;}}ģ.Ę=Ĳ;ģ.ò=ò(Ĳ,ĳ);}void Ĭ(List<
IMyPowerProducer>Ġ,ref ě.Ě ģ){float ĳ=0;float Ĳ=0;foreach(IMyPowerProducer Ü in Ġ){Ĳ+=Ü.CurrentOutput;ĳ+=Ü.MaxOutput;}ģ.Ę=Ĳ;ģ.ò=ò(Ĳ,ĳ);}
void Ĥ(IMyTerminalBlock ô,ref ě.Ě ģ){if(ô is IMyGasTank)ģ.ò=(float)(ô as IMyGasTank).FilledRatio*100;if(ô is IMyCockpit||ô
is IMyCryoChamber)ģ.ò=(ô as IMyCockpit).OxygenFilledRatio*100;}void Ĥ(List<IMyTerminalBlock>Ġ,ref ě.Ě ģ){double Ģ=0;double
ġ=0;foreach(IMyTerminalBlock ô in Ġ){if(ô is IMyGasTank){IMyGasTank Ü=ô as IMyGasTank;double ĥ=Ü.Capacity;Ģ+=ĥ;ġ+=ĥ*Ü.
FilledRatio;}if(ô is IMyCockpit){IMyCockpit Ü=ô as IMyCockpit;double ĥ=Ü.OxygenCapacity;Ģ+=ĥ;ġ+=ĥ*Ü.OxygenFilledRatio;}}ģ.ò=ò((
float)ġ,(float)Ģ);}void Ĥ(List<IMyGasTank>Ġ,ref ě.Ě ģ){double Ģ=0;double ġ=0;foreach(IMyGasTank Ü in Ġ){double ĥ=Ü.Capacity;Ģ
+=ĥ;ġ+=ĥ*Ü.FilledRatio;}ģ.ò=ò((float)ġ,(float)Ģ);}void Ħ(IMyTerminalBlock ô,ref ě.Ě ģ){if(ô is IMyJumpDrive){IMyJumpDrive
Ü=ô as IMyJumpDrive;ģ.ò=ò(Ü.CurrentStoredPower,Ü.MaxStoredPower);}}void Ħ(List<IMyTerminalBlock>ī,ref ě.Ě ģ){float Ī=0;
float ĩ=0;foreach(IMyTerminalBlock ô in ī){if(ô is IMyJumpDrive){Ī+=(ô as IMyJumpDrive).MaxStoredPower;ĩ+=(ô as IMyJumpDrive)
.CurrentStoredPower;}}ģ.ò=ò(ĩ,Ī);}void Ħ(List<IMyJumpDrive>Ġ,ref ě.Ě ģ){float Ī=0;float ĩ=0;foreach(IMyJumpDrive Ü in Ġ){
Ī+=Ü.MaxStoredPower;ĩ+=Ü.CurrentStoredPower;}ģ.ò=ò(ĩ,Ī);}bool Ĩ(ref IMyTerminalBlock ô,string Ĵ,bool ţ){try{ô=
GridTerminalSystem.GetBlockWithName(Ĵ);if(ô==null)return false;if(ô.IsSameConstructAs(Me)||!ţ)return true;else return false;}catch{return
false;}}bool ŕ(ref IMyBlockGroup œ,string Ĵ){try{œ=GridTerminalSystem.GetBlockGroupWithName(Ĵ);if(œ==null)return false;else
return true;}catch{return false;}}bool Œ(ref List<IMyTerminalBlock>Ġ,string Ĵ){try{GridTerminalSystem.SearchBlocksOfName(Ĵ,Ġ);
if(Ġ.Count<1)return false;return true;}catch{return false;}}void ő(){Ϋ=new List<IMyThrust>();List<IMyThrust>Ŕ=new List<
IMyThrust>();GridTerminalSystem.GetBlocksOfType<IMyThrust>(Ŕ);foreach(IMyThrust Ő in Ŕ){if(Ő.IsSameConstructAs(Me))Ϋ.Add(Ő);}ά=
new List<IMyThrust>[4,7];for(int Ë=0;Ë<4;Ë++)for(int Ŏ=0;Ŏ<7;Ŏ++)ά[Ë,Ŏ]=new List<IMyThrust>();foreach(IMyThrust ō in Ϋ){
string Ō=ō.BlockDefinition.ToString();int ŋ;if(Ō.Contains("Atmo"))ŋ=1;else if(Ō.Contains("Hydro"))ŋ=2;else ŋ=3;Vector3I Ŋ=ō.
GridThrustDirection;int ŏ;if(Ŋ.Equals(Vector3I.Left))ŏ=1;else if(Ŋ.Equals(Vector3I.Right))ŏ=2;else if(Ŋ.Equals(Vector3I.Up))ŏ=3;else if(Ŋ.
Equals(Vector3I.Down))ŏ=4;else if(Ŋ.Equals(Vector3I.Forward))ŏ=5;else if(Ŋ.Equals(Vector3I.Backward))ŏ=6;else ŏ=-1;if(ŏ>=0){ά[
ŋ,ŏ].Add(ō);ά[0,ŏ].Add(ō);}ά[ŋ,0].Add(ō);ά[0,0].Add(ō);}List<IMyTerminalBlock>ŉ=new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocks(ŉ);Ω=new List<IMyJumpDrive>();Ψ=new List<IMyBatteryBlock>();Ϊ=new List<IMyInventory>();Φ=new List<IMyGasTank>
();Χ=new List<IMyPowerProducer>();foreach(IMyTerminalBlock ô in ŉ){if(ô is IMyJumpDrive&&ô.IsSameConstructAs(Me))Ω.Add(ô
as IMyJumpDrive);if(ô is IMyBatteryBlock&&ô.IsSameConstructAs(Me))Ψ.Add(ô as IMyBatteryBlock);if(ô.HasInventory&&ô.
IsSameConstructAs(Me))Ϊ.Add(ô.GetInventory(0));if(ô is IMyGasTank&&ô.IsSameConstructAs(Me)){string Ō=ô.BlockDefinition.ToString();if(Ō.
Contains("HydrogenTank"))Φ.Add(ô as IMyGasTank);}if(ô is IMyPowerProducer&&ô.IsSameConstructAs(Me))Χ.Add(ô as IMyPowerProducer);
}}void Ţ(){if(ʫ.Ƅ)Echo("WASD");if(ʺ!="")Echo(ʺ);}void š(){Echo("Running setup ...");ˮ=GridTerminalSystem;ʩ=true;int Š=-1;
ʬ=new ǣ(Me);Ͱ=new Dictionary<string,ě.Ě>(){{"bl",new ě.Ě(ʖ.ʕ)},{"blk",new ě.Ě(ʖ.ʕ)},{"*bl*",new ě.Ě(ʖ.ʕ)},{"*blk*",new ě.
Ě(ʖ.ʕ)},{"gr",new ě.Ě(ʖ.ʕ)},{"gr-gr",new ě.Ě(ʖ.ʕ)},{"thrust",new ě.Ě(ʖ.ʕ)},{"sound",new ě.Ě(ʖ.N)},{"gr-sound",new ě.Ě(ʖ.N
)},{"light",new ě.Ě(ʖ.N)},{"gr-light",new ě.Ě(ʖ.N)},{"write",new ě.Ě(ʖ.N)},{"gr-write",new ě.Ě(ʖ.N)},{"waypoint",new ě.Ě(
ʖ.N)},{"gr-waypoint",new ě.Ě(ʖ.N)},{"batt",new ě.Ě(ʖ.Ŭ)},{"gr-batt",new ě.Ě(ʖ.Ŭ)},{"door",new ě.Ě(ʖ.Ŭ)},{"gr-door",new ě.
Ě(ʖ.Ŭ)},{"jump",new ě.Ě(ʖ.Ŭ)},{"gr-jump",new ě.Ě(ʖ.Ŭ)},{"piston_att",new ě.Ě(ʖ.Ŭ)},{"gr-piston_att",new ě.Ě(ʖ.Ŭ)},{
"rotor_att",new ě.Ě(ʖ.Ŭ)},{"gr-rotor_att",new ě.Ě(ʖ.Ŭ)},{"rotor_lock",new ě.Ě(ʖ.Ŭ)},{"gr-rotor_lock",new ě.Ě(ʖ.Ŭ)},{"conn_lock",new
ě.Ě(ʖ.Ŭ)},{"gr-conn_lock",new ě.Ě(ʖ.Ŭ)},{"conn_collect",new ě.Ě(ʖ.Ŭ)},{"gr-conn_collect",new ě.Ě(ʖ.Ŭ)},{"conn_throw",new
ě.Ě(ʖ.Ŭ)},{"gr-conn_throw",new ě.Ě(ʖ.Ŭ)},{"lgear_auto",new ě.Ě(ʖ.Ŭ)},{"gr-lgear_auto",new ě.Ě(ʖ.Ŭ)},{"lgear_lock",new ě.Ě
(ʖ.Ŭ)},{"gr-lgear_lock",new ě.Ě(ʖ.Ŭ)},{"sorter_drain",new ě.Ě(ʖ.Ŭ)},{"gr-sorter_drain",new ě.Ě(ʖ.Ŭ)},{"sorter_list",new ě
.Ě(ʖ.Ŭ)},{"gr-sorter_list",new ě.Ě(ʖ.Ŭ)},{"stockpile",new ě.Ě(ʖ.Ŭ)},{"gr-stockpile",new ě.Ě(ʖ.Ŭ)},{"timer",new ě.Ě(ʖ.Ŭ)},
{"gr-timer",new ě.Ě(ʖ.Ŭ)},{"turret",new ě.Ě(ʖ.Ŭ)},{"gr-turret",new ě.Ě(ʖ.Ŭ)},{"vent",new ě.Ě(ʖ.Ŭ)},{"gr-vent",new ě.Ě(ʖ.Ŭ
)},{"remote",new ě.Ě(ʖ.Ŭ)},{"gr-remote",new ě.Ě(ʖ.Ŭ)},{"gr-airlock",new ě.Ě(ʖ.Ŭ)},{"pb",new ě.Ě(ʖ.Ŭ)},{"run",new ě.Ě(ʖ.Ŭ)
},{"antenna",new ě.Ě(ʖ.Ň,"F0",true)},{"gr-antenna",new ě.Ě(ʖ.Ň,"F0",true)},{"beacon",new ě.Ě(ʖ.Ň,"F0",true)},{"gr-beacon"
,new ě.Ě(ʖ.Ň,"F0",true)},{"piston_vel",new ě.Ě(ʖ.Ň,"F2",true)},{"gr-piston_vel",new ě.Ě(ʖ.Ň,"F2",true)},{"rotor_vel",new
ě.Ě(ʖ.Ň,"F1",true)},{"gr-rotor_vel",new ě.Ě(ʖ.Ň,"F1",true)},{"grav",new ě.Ě(ʖ.Ň,"F1",true)},{"gr-grav",new ě.Ě(ʖ.Ň,"F1",
true)},{"timer_delay",new ě.Ě(ʖ.Ň,"F1",true)},{"gr-timer_delay",new ě.Ě(ʖ.Ň,"F1",true)},{"jump_dist",new ě.Ě(ʖ.Ň,"F0",true,
true,"")},{"gr-jump_dist",new ě.Ě(ʖ.Ň,"F0",true,true,"")},{"show",new ě.Ě(ʖ.Ň,"F1",false,true,">")},{"gr-show",new ě.Ě(ʖ.Ň,
"F1",false,true,">")},{"all-show",new ě.Ě(ʖ.Ň,"F1",false,true,">")},};const string ş="Used default ";const string Ş=
" in Custom Data";if(ʬ.Ƴ("Main","CockpitPrefix",ref ʽ)&&ʬ.Ƴ("Main","LCDPrefix",ref ʼ)){}else{Echo(
"Error!!! in Custom Data [Main] section");ʩ=false;}if(!ʬ.Ƴ("Add","Prefix",ref ʻ,"",false))Echo(ş+"[Add] 'Prefix'"+Ş);if(!ʬ.Ƴ("Add","Screen",ref ʸ,0))Echo(ş+
"[Add] 'Screen'"+Ş);if(!ʬ.Ƴ("Add","DefaultMenu",ref ʲ,0))Echo(ş+"[Add] 'DefaultMenu'"+Ş);if(!ʬ.Ƴ("Add","WASDAutoDisable",ref ʃ,true))
Echo(ş+"[Add] 'WASDAutoDisable'"+Ş);if(!ʬ.Ƴ("Add","SwitchThrusters",ref ʑ,false))Echo(ş+"[Add] 'SwitchThrusters'"+Ş);if(!ʬ.Ƴ
("Add","SwitchGyros",ref ʐ,true))Echo(ş+"[Add] 'SwitchGyros'"+Ş);if(!ʬ.Ƴ("Add","SwitchWheels",ref ʏ,false))Echo(ş+
"[Add] 'SwitchWheels'"+Ş);if(!ʬ.Ƴ("Add","WASDPrgOn",ref ʞ,-1))Echo(ş+"[Add] 'WASDPrgOn'"+Ş);if(!ʬ.Ƴ("Add","WASDPrgOff",ref ʓ,-1))Echo(ş+
"[Add] 'WASDPrgOff'"+Ş);if(!ʬ.Ƴ("Add","LCDAutoOff",ref Θ,false))Echo(ş+"[Add] 'LCDAutoOff'"+Ş);if(!ʬ.Ƴ("Color","BackgroundColor",ref Ʉ,Ʉ))
Echo(ş+"[Color] 'BackgroundColor'"+Ş);if(!ʬ.Ƴ("Color","ItemBarColor",ref Ƀ,Ƀ))Echo(ş+"[Color] 'ItemBarColor'"+Ş);if(!ʬ.Ƴ(
"Color","MainColor",ref ɂ,ɂ))Echo(ş+"[Color] 'MainColor'"+Ş);if(!ʬ.Ƴ("Color","SelItemBarColor",ref Ɂ,Ɂ))Echo(ş+
"[Color] 'SelItemBarColor'"+Ş);if(!ʬ.Ƴ("Color","SelItemTextColor",ref ɀ,ɀ))Echo(ş+"[Color] 'SelItemTextColor'"+Ş);if(!ʬ.Ƴ("Color","OnColor",ref ȿ,ȿ
))Echo(ş+"[Color] 'OnColor'"+Ş);if(!ʬ.Ƴ("Color","ArrowColor",ref Ʌ,Ʌ))Echo(ş+"[Color] 'ArrowColor'"+Ş);if(!ʬ.Ƴ("Color",
"ErrorMSGColor",ref Ⱦ,Ⱦ))Echo(ş+"[Color] 'ErrorMSGColor'"+Ş);if(!ʬ.Ƴ("Color","HelpColor",ref ȼ,ȼ))Echo(ş+"[Color] 'HelpColor'"+Ş);ʣ=new
List<string>();ʢ=new List<ē>();ʡ=new List<ē>();ʠ=new List<ē>();ʦ=new List<ē>();int Ë=0;bool ŝ;do{string Ŝ="";if(ʬ.Ƴ(
"TopMenu",Ë.ToString(),ref Ŝ)){ŝ=true;ʣ.Add(Ŝ);}else ŝ=false;Ë++;}while(ŝ);ʷ=Ë-1;if(ʷ<1){Echo(
"Error!!! in Custom Data [TopMenu] section");ʩ=false;}Ï(ʣ,ref ʢ,ɂ,TextAlignment.CENTER,ʇ);Ï(ʣ,ref ʠ,ɀ,TextAlignment.CENTER,ʇ);Echo("Total "+ʷ.ToString()+
" items found in TopMenu");List<string>[]ś=new List<string>[ʷ];ʭ=new List<ě>[ʷ];ʥ=new List<ē>[ʷ];ʤ=new List<ē>[ʷ];for(Ë=0;Ë<ʷ;Ë++){ś[Ë]=new List<
string>();ʭ[Ë]=new List<ě>();ʥ[Ë]=new List<ē>();ʤ[Ë]=new List<ē>();if(ʬ.Ƴ(Ë.ToString(),ref ś[Ë])){Š=Ķ(ś[Ë]);if(Š>=0){Echo(
"Error!!! in: ["+Ë.ToString()+"], line: "+(Š+1).ToString());ʩ=false;return;}Ï(ś[Ë],ref ʥ[Ë],ɂ,TextAlignment.LEFT,ʇ);Ï(ś[Ë],ref ʤ[Ë],ɀ,
TextAlignment.LEFT,ʇ);for(int Ŏ=0;Ŏ<ś[Ë].Count;Ŏ++){string[]Ś=ś[Ë][Ŏ].Split(';');try{if(Ś.Length>4)ʭ[Ë].Add(new ě(Ś[0],Ś[1],Ś[2],Ś[3]
,Ͱ[Ś[1]],Ś[4]));else ʭ[Ë].Add(new ě(Ś[0],Ś[1],Ś[2],Ś[3],Ͱ[Ś[1]]));}catch{Echo("Error!!! in menu ["+Ë.ToString()+
"], line: "+Ŏ.ToString()+" command: "+Ś[1]);ʩ=false;}}}else{Echo("Error!!! in Custom Data ["+Ë.ToString()+"] section");ʩ=false;}}
Echo("..and "+Ë+" menus was found");Ë=0;string ř="";List<string>Ř=new List<string>();while(ʬ.ǚ(Ë.ToString()+@"\D{2,}",ref ř)
){Ř.Add(ř);Ë++;}ʯ=Ë;if(ʯ>0){List<string>[]ŗ=new List<string>[ʯ];ʧ=new List<ě>[ʯ];for(Ë=0;Ë<ʯ;Ë++){ŗ[Ë]=new List<string>()
;ʧ[Ë]=new List<ě>();if(ʬ.Ƴ(Ř[Ë],ref ŗ[Ë])){Š=Ķ(ŗ[Ë]);if(Š>=0){Echo("Error!!! in: ["+Ř[Ë]+"], line: "+(Š+1).ToString());ʩ=
false;return;}for(int Ŏ=0;Ŏ<ŗ[Ë].Count;Ŏ++){string[]Ŗ=ŗ[Ë][Ŏ].Split(';');try{ʧ[Ë].Add(new ě(Ŗ[0],Ŗ[1],Ŗ[2],Ŗ[3],Ͱ[Ŗ[1]]));}
catch{Echo("Error!!! in menu ["+Ř[Ë]+"], line: "+Ŏ.ToString()+" command: "+Ŗ[1]);ʩ=false;}}}else{Echo(
"Error!!! in Custom Data ["+Ř[Ë]+"] section");ʩ=false;}}Echo("Found "+ʯ.ToString()+" sub-programs");}const string ň="AutoAirlocks";var ľ=new List<
string>();string ĵ="";if(ʬ.Ƴ(ň,ref ľ)){ȫ=true;Š=Ķ(ľ);if(Š>=0){Echo("Error!!! in: ["+ň+"], line: "+(Š+1).ToString());ȫ=false;}
if(ȫ)ʔ=ŷ.ǂ.Ƶ(ʬ,ʻ,ľ,ref ĵ);if(ʔ.ǁ.Count<1)ȫ=false;Echo(ĵ);}ˀ=new List<ē>();Ý(ref ˀ,ɒ,ɂ);Ý(ref ˀ,Ɏ,Ʌ,"AH_BoreSight",1);Ý(ref
ˀ,Ɏ,Ʌ,"AH_BoreSight",3);Ý(ref ˀ,Ɏ,Ʌ,"AH_BoreSight",0);Ý(ref ˀ,Ɏ,Ʌ,"AH_BoreSight",2);Ý(ref ˀ,ɑ,ɂ);Ý(ref ˀ,ɍ,Ʉ,
"AH_BoreSight",1);Ý(ref ˀ,ɍ,Ʉ,"AH_BoreSight",3);Ý(ref ˀ,ɍ,Ʉ,"AH_BoreSight",0);Ý(ref ˀ,ɍ,Ʉ,"AH_BoreSight",2);T("W",ref ˀ,Ʉ,ʉ);T("S",ref
ˀ,Ʉ,ʉ);T("A",ref ˀ,Ʉ,ʉ);T("D",ref ˀ,Ʉ,ʉ);T("Q",ref ˀ,Ʉ,ʉ);T("E",ref ˀ,Ʉ,ʉ);T("Space",ref ˀ,Ʉ,ʉ);Ý(ref ˀ,Ȥ,ȼ,
"SquareSimple",0);Ý(ref ˀ,ȣ,ȼ,"AH_BoreSight",3);Ý(ref ˀ,ȣ,ȼ,"AH_BoreSight",1);Ý(ref ˀ,ȣ,ȼ,"AH_BoreSight",2);Ý(ref ˀ,ȣ,ȼ,"AH_BoreSight"
,0);Ý(ref ˀ,ȥ,ȼ,"AH_BoreSight",2);Ý(ref ˀ,ȥ,ȼ,"AH_BoreSight",0);Ý(ref ˀ,Ȼ,Ƀ);Ý(ref ˀ,Ȼ,Ɂ);Þ(ref ˀ,ȹ,Ƀ);Þ(ref ˀ,ȹ,Ɂ);Þ(ref
ˀ,ȷ,ɂ,"AH_TextBox");Þ(ref ˀ,ɘ,ɂ,"AH_TextBox");Ý(ref ˀ,Ɋ,ȿ,"Triangle",2);Ý(ref ˀ,ȩ*1.45f,Ʉ,"SquareSimple");Ý(ref ˀ,ȩ*1.4f,
ȿ,"AH_TextBox");Ý(ref ˀ,ȩ,ɂ,"AH_TextBox");T("Auto",ref ˀ,ɂ,ʇ);T("Recharge",ref ˀ,ȿ,ʇ);T("Discharge",ref ˀ,ɂ,ʇ);ʮ=new List
<ē>();Ý(ref ʮ,ȶ,ȿ,"IconEnergy");Ý(ref ʮ,ɕ,ȿ,"Triangle",1);T("Open",ref ʮ,ȿ,ʈ);T("Close",ref ʮ,ɂ,ʈ);T("Auto",ref ʮ,ɂ,ʈ);T(
"Rech",ref ʮ,ȿ,ʈ);T("Disch",ref ʮ,ɂ,ʈ);T("Attach",ref ʮ,ɂ,ʈ);T("Detach",ref ʮ,ȿ,ʈ);T("Lock",ref ʮ,ȿ,ʈ);T("Unlock",ref ʮ,ɂ,ʈ);T
("1",ref ʮ,ɂ,ʈ);T("2",ref ʮ,ɂ,ʈ);T("Collect",ref ʮ,ɂ,ʈ);T("Throw",ref ʮ,ȿ,ʈ);T("Ready",ref ʮ,ɂ,ʈ);T("Charge",ref ʮ,ɂ,ʈ);T
("Shoot",ref ʮ,ȿ,ʈ);T("Yes",ref ʮ,ɂ,ʈ);T("<<<",ref ʮ,ɂ,ʈ);T(">>>",ref ʮ,ɂ,ʈ);T("PB",ref ʮ,ɂ,ʈ);T("> 2",ref ʮ,ɂ,ʈ);T("OK",
ref ʮ,ɂ,ʈ);T("Press",ref ʮ,ȿ,ʈ);T("No Air",ref ʮ,ȿ,ʈ);T("Stock",ref ʮ,ȿ,ʈ);T("Drain",ref ʮ,ȿ,ʈ);T("Black",ref ʮ,ɂ,ʈ);T(
"White",ref ʮ,ɂ,ʈ);T("Leak",ref ʮ,ȿ,ʈ);T("Off",ref ʮ,ɂ,ʈ);ʟ=new List<ē>();ʟ.Clear();ʾ=new List<ē>();ʾ.Clear();ő();Echo("Found "
+Ϋ.Count.ToString()+" thrusters");Echo("Found "+Ω.Count.ToString()+" jumps, "+Ψ.Count.ToString()+" batts, "+Ϊ.Count.
ToString()+" invs, "+Φ.Count.ToString()+" H2 tanks, "+Χ.Count.ToString()+" powers");if(ȫ)Echo("+ "+ľ.Count.ToString()+
" auto airlocks");List<IMyShipController>ļ=new List<IMyShipController>();Ț=new List<ǥ.ż>();Ώ=new List<ă>();try{GridTerminalSystem.
GetBlocksOfType<IMyShipController>(ļ);foreach(IMyShipController ĺ in ļ){if(ĺ.CustomName.Contains(ʽ)){System.Text.RegularExpressions.
Match Ä=System.Text.RegularExpressions.Regex.Match(ĺ.CustomName,@"\d+");int Ļ;if(Ä.Success&&Int32.TryParse(Ä.Value,out Ļ))Ț.
Add(new ǥ.ż(ĺ,Ļ));else Ț.Add(new ǥ.ż(ĺ,-1));}}if(Ț.Count<1){Echo("Error!!! Cockpits with '"+ʽ+"' not found");ʩ=false;}
foreach(ǥ.ż ĺ in Ț){IMyTextSurface ķ;ķ=(ĺ.ƀ as IMyTextSurfaceProvider)?.GetSurface(ʸ);if(ķ!=null){ķ.ContentType=VRage.Game.GUI.
TextPanel.ContentType.SCRIPT;ķ.Script="";ķ.BackgroundColor=Ʉ;bool Ľ=false;if(ĺ.ƀ.BlockDefinition.ToString()==
"MyObjectBuilder_Cockpit/DBSmallBlockFighterCockpit"&&ʸ==0)Ľ=true;Ώ.Add(new ă((ķ.TextureSize-ķ.SurfaceSize)/2f,ķ.SurfaceSize,Ľ));}}ʫ=new ǥ(this,Ț,ʄ,ά,ʃ,ʑ,ʐ,ʏ,true,ɻ,ʞ,ʓ);
Echo("Cockpits Count: "+Ț.Count.ToString());}catch{Echo("Error!!! Cockpits not found");ʩ=false;}if(ʼ!=""){List<IMyTextPanel>
Ĺ=new List<IMyTextPanel>();Α=new List<IMyTextPanel>();ΐ=new List<IMyTextPanel>();Δ=new List<ă>();try{GridTerminalSystem.
GetBlocksOfType<IMyTextPanel>(Ĺ);foreach(IMyTextPanel ķ in Ĺ){if(ķ.CustomName.Contains(ʼ)){Α.Add(ķ);if(ķ.BlockDefinition.ToString().
Contains("Transparent"))ΐ.Add(ķ);}}if(Α.Count>0){if(ΐ.Count<1)Θ=false;foreach(IMyTextPanel ķ in Α){ķ.ContentType=VRage.Game.GUI.
TextPanel.ContentType.SCRIPT;ķ.Script="";ķ.BackgroundColor=Ʉ;Δ.Add(new ă((ķ.TextureSize-ķ.SurfaceSize)/2f,ķ.SurfaceSize));}ˁ=true
;Echo("LCDS count: "+Α.Count.ToString());}else{ˁ=false;Echo("LCDS with '"+ʼ+"' not found");}}catch{ˁ=false;Echo(
"LCDS not found");}}if(ʩ){Ύ=new List<IMyTerminalBlock>();Ξ=new List<IMyTerminalBlock>();Υ=new List<MyInventoryItemFilter>();ʶ=ʵ=ʳ=ʱ=0;if
(ʲ>=0&&ʲ<ʷ)ʳ=ʲ;ʴ=Math.Max(ś[ʳ].Count-ʋ,0);ʺ=ʹ="";ί=false;ʎ=false;ŀ();ʨ=new List<Ə>();ͱ=new List<Τ>();Γ=Me.GetSurface(0);
if(Γ!=null){Γ.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;Γ.FontSize=1.5f;Γ.WriteText(
"Space Defence Systems\n");Γ.WriteText("  Operational Script\n",true);Γ.WriteText("    v."+Version+"\n",true);Γ.WriteText("by DEeM0NX\n\n\n",true
);}Runtime.UpdateFrequency=ʄ;Echo("Setup complete.");}}int Ķ(List<string>Í){for(int Ë=0;Ë<Í.Count;Ë++){string[]N=Í[Ë].
Split(';');if(N.Length<4)return Ë;}return-1;}int ĸ(string[]Ŀ){if(Ŀ.Length>1){int Ļ;if(Int32.TryParse(Ŀ[1],out Ļ)){if(Ļ>=0)
return Ļ;else return-1;}else return-1;}else return-1;}float ò(float Ň,float ņ){if(ņ==0)return-1;return(float)Math.Round((Ň/ņ)*
100,1);}float ò(double Ň,double ņ){if(ņ==0)return-1;return(float)Math.Round((Ň/ņ)*100,1);}float Ņ(float ħ,ref string ń,ref
string Ń){if(ħ>9999999){ń="F0";Ń="M";return(float)Math.Round(ħ/1000000);}else if(ħ>999999){ń="F1";Ń="M";return(float)Math.
Round(ħ/100000)/10;}else if(ħ>9999){ń="F0";Ń="K";return(float)Math.Round(ħ/1000);}else if(ħ>999){ń="F1";Ń="K";return(float)
Math.Round(ħ/100)/10;}else if(ħ>9){ń="F1";Ń="";return ħ;}else{ń="F2";return ħ;}}float ł(float Ł){if(Η.TotalMass<=1250000)
return 2000*Ω.Count*Ł/100;else return(2500000000*Ω.Count*Ł)/(Η.TotalMass*100);}void ŀ(){Ν=false;Λ=false;Κ=false;Ι=false;}void
Ù(string Ö,string Õ,out int ħ,ref string Ó){ħ=0;if(ã(Ö,Õ+@"\d")){System.Text.RegularExpressions.Match Ä=á(Ö,@"\d");if(!
int.TryParse(Ä.Value,out ħ)){Ó="Error in int: "+Ö;}}}float Ù(string Ö,string Õ,float Ô,ref string Ó){if(ã(Ö,Õ+@"[\d.]+")){
System.Text.RegularExpressions.Match Ä=á(Ö,@"[\d.]+");float Ú;if(float.TryParse(Ä.Value,out Ú))return Ú;else{Ó=
"Error in float: "+Ö;}}return Ô;}Color Ù(string Ö,string Õ,Color Ô,ref string Ó){if(ã(Ö,Õ+@"\d+,\d+,\d+")){System.Text.RegularExpressions.
Match Ä=á(Ö,@"\d+,\d+,\d+");string[]Ò=Ä.Value.Split(',');byte Ø,Ñ,Ü;if(Byte.TryParse(Ò[0],out Ø)&&Byte.TryParse(Ò[1],out Ñ)&&
Byte.TryParse(Ò[2],out Ü))return new Color(Ø,Ñ,Ü);else Ó="Error in color: "+Ö;}return Ô;}bool ã(string à,string ß){if(System
.Text.RegularExpressions.Regex.IsMatch(à,ß,System.Text.RegularExpressions.RegexOptions.IgnoreCase))return true;else
return false;}System.Text.RegularExpressions.Match á(string à,string ß){return System.Text.RegularExpressions.Regex.Match(à,ß,
System.Text.RegularExpressions.RegexOptions.IgnoreCase);}void Þ(ref List<ē>J,Vector2 Û,Color I,string Ð="SquareSimple"){ē B;
Vector2 Î=new Vector2(Û.X/2f,Û.Y/2f);B=new ē(Ð,Û,Î,0,I);J.Add(B);}void Ý(ref List<ē>J,Vector2 Û,Color I,string Ð="SquareSimple"
,int o=0){ē B;float Z=(float)(o*Math.PI/2d);B=new ē(Ð,Û,Vector2.Zero,Z,I);J.Add(B);}void T(string N,ref List<ē>J,Color I,
float G,TextAlignment H=TextAlignment.CENTER){ē B;B=new ē(N,ʪ,G,Vector2.Zero,I,H);J.Add(B);}void M(float L,ref List<ē>J,Color
I,TextAlignment H,float G){L=Math.Min(L,100);L=Math.Max(L,0);ē B;string N="IIIIIIIIIIIIIIIIIIII";int Ê=(int)(L/5);N=N.
Substring(20-Ê);B=new ē(N,ʪ,G,Vector2.Zero,I,H);J.Add(B);}void Ï(List<string>Í,ref List<ē>Ì,Color I,TextAlignment H,float G){ē B;
Vector2 Î=new Vector2(0,-22);if(H==TextAlignment.LEFT){Î.X=10f;Î.Y=-3f;}for(int Ë=0;Ë<Í.Count;Ë++){string[]N=Í[Ë].Split(';');B=
new ē(N[0],ʪ,G,Î,I,H);Ì.Add(B);}}void É(float È,float Ç,string Æ,string Å,int â,ref List<ē>ä,string ğ){if((Æ==">"&&Ç>80)||(
Æ=="<"&&Ç<20)||(Æ=="+"&&Ç<100))T(È.ToString(Å)+ğ,ref ä,ȿ,ʈ);else T(È.ToString(Å)+ğ,ref ä,ɂ,ʈ);if(â==ʶ)M(Ç,ref ä,ɀ,
TextAlignment.LEFT,ʈ);else M(Ç,ref ä,ɂ,TextAlignment.LEFT,ʈ);}struct ē{string đ;Vector2 Đ;Vector2 ď;float Ď;Color č;string Č;string Ē
;float ċ;bool ĉ;TextAlignment Ĉ;public ē(string ć,Vector2 Û,Vector2 Ć,float Ċ,Color I){đ=ć;Đ=Û;ď=Ć;Ď=Ċ;č=I;ĉ=false;Č="";Ē
="";ċ=0f;Ĉ=TextAlignment.CENTER;}public ē(string N,string Ğ,float Ĝ,Vector2 Ć,Color I,TextAlignment H=TextAlignment.
CENTER){Ē=N;Č=Ğ;ċ=Ĝ;ď=Ć;Ď=Ĝ;č=I;ĉ=true;Ĉ=H;đ="";Đ=Vector2.Zero;}public MySprite ĝ(float Ĝ,Vector2 Î){if(!ĉ)return new MySprite
(SpriteType.TEXTURE,đ,Î+ď*Ĝ,Đ*Ĝ,č,rotation:Ď);else return new MySprite(SpriteType.TEXT,Ē,Î+ď*Ĝ,null,č,Č,rotation:Ď*Ĝ,
alignment:Ĉ);}}class ě{public struct Ě{public ʖ ę;public float Ę;public string ė;public bool Ė;public bool ĕ;public string Ĕ;
public bool ą;public float ò;public Ě(ʖ ð,string Å="",bool ï=false,bool î=false,string í="",bool ì=false){ę=ð;Ę=-1;ė=Å;Ė=ï;ĕ=î
;Ĕ=í;ą=ì;ò=-1;}}public ʊ ñ;public string ë;public string ê;public string é;public string è;public Ě ç;public int æ;
private IReadOnlyList<string>å=new List<string>(){"antenna","beacon","batt","conn_lock","conn_collect","conn_throw","door",
"grav","jump","jump_dist","light","lgear_auto","lgear_lock","piston_att","piston_vel","rotor_att","rotor_lock","rotor_vel",
"sorter_drain","sorter_list","sound","stockpile","timer","timer_delay","turret","vent","write","airlock","show","remote","waypoint"};
public ě(string ó,string û,string Ą,string Ă,Ě ā,string Ā="-1"){ë=ó;ê=û;é=Ą;è=Ă;ñ=ÿ(û);ç=ā;if(!int.TryParse(Ā,out æ))æ=-1;}
private ʊ ÿ(string þ){if(þ=="bl"||þ=="blk")return ʊ.Ǡ;if(þ=="all-show")return ʊ.ʗ;if(þ=="thrust")return ʊ.Ǭ;if(þ=="pb")return ʊ
.ʙ;if(þ=="run")return ʊ.ʜ;if(þ=="*bl*"||þ=="*blk*")return ʊ.ʘ;string[]ý=þ.Split('-');if(å.Contains(ý[0]))return ʊ.ʝ;if(ý[
0]=="gr"){if(ý.Length<2)return ʊ.œ;else if(å.Contains(ý[1]))return ʊ.ʛ;else if(ý[1]=="gr")return ʊ.ʚ;}return ʊ.Ȁ;}}struct
ă{public RectangleF ü;public float ú;public Vector2 ù;public Vector2 ø;public Vector2 ö;public ă(Vector2 õ,Vector2 Û,bool
Ť=false){ü=new RectangleF(õ,Û);Vector2 Ĝ=ü.Size/512f;ú=Math.Min(Ĝ.X,Ĝ.Y);ù=ü.Center-ü.Size/2f;if(Ť){ú=ú/1.1f;ù.X=ù.X+ü.
Size.X/4.5f;ù.Y=ù.Y+ü.Size.Y/11f;}ø=ü.Center;ø.Y=ù.Y;ö=ù;ö.X=ù.X+ü.Size.X;if(Ť){ö.X=ø.X+ü.Size.X/5f;ö.Y=ù.Y-ü.Size.Y/11f;}
float Ǥ=Ĝ.X/Ĝ.Y;if(Ǥ>2)ù.X=ü.Size.Y*(Ǥ-2f)/2f;}}class ǣ{string Ǣ;IMyTerminalBlock ǡ;public ǣ(IMyTerminalBlock Ǡ){ǡ=Ǡ;Ǣ=ǡ.
CustomData;}public bool Ƴ(string ǒ,string ƛ,ref string ǌ,string Ǖ="",bool ǔ=true){string Ŝ="";if(ǖ(ǒ,ƛ,ref Ŝ,ǔ)){ǌ=Ŝ;return true;}
else{ǌ=Ǖ;return false;}}public bool Ƴ(string ǒ,string ƛ,ref bool ǌ,bool Ǖ=false,bool ǔ=true){string Ŝ="";if(ǖ(ǒ,ƛ,ref Ŝ,ǔ))
return bool.TryParse(Ŝ,out ǌ);else{ǌ=Ǖ;return false;}}public bool Ƴ(string ǒ,string ƛ,ref int ǌ,int Ǖ=0,bool ǔ=true){string Ŝ=
"";if(ǖ(ǒ,ƛ,ref Ŝ,ǔ))return int.TryParse(Ŝ,out ǌ);else{ǌ=Ǖ;return false;}}public bool Ƴ(string ǒ,string ƛ,ref float ǌ,
float Ǖ=0,bool ǔ=true){string Ŝ="";if(ǖ(ǒ,ƛ,ref Ŝ,ǔ))return float.TryParse(Ŝ,out ǌ);else{ǌ=Ǖ;return false;}}public bool Ƴ(
string ǒ,string ƛ,ref Color ǌ,Color Ǖ,bool ǔ=true){string Ŝ="";if(ǖ(ǒ,ƛ,ref Ŝ,ǔ)){string[]Ò=Ŝ.Split(',');byte Ø,Ñ,Ü;if(Byte.
TryParse(Ò[0],out Ø)&&Byte.TryParse(Ò[1],out Ñ)&&Byte.TryParse(Ò[2],out Ü)){ǌ.R=Ø;ǌ.G=Ñ;ǌ.B=Ü;return true;}else{ǌ=Ǖ;return false
;}}else return false;}public bool Ƴ(string ǒ,ref List<string>Ǒ){Ǣ=ǡ.CustomData;var ǐ=Ǣ.Split('\n');string Ǐ="";bool ǎ=
false;foreach(string Ǎ in ǐ){string Ǔ=Ǎ.Trim();if(Ǔ.StartsWith("[")&&Ǔ.EndsWith("]")){Ǐ=Ǔ;}else if(Ǐ.Equals($"[{ǒ}]")&&Ǔ.Trim
()!=""){Ǒ.Add(Ǔ);ǎ=true;}}return ǎ;}private bool ǖ(string ň,string ǟ,ref string ǝ,bool ǜ){Ǣ=ǡ.CustomData;var ǐ=Ǣ.Split(
'\n');string Ǐ="";foreach(string Ǎ in ǐ){string Ǔ=Ǎ.Trim();if(Ǔ.StartsWith("[")&&Ǔ.EndsWith("]")){Ǐ=Ǔ;}else if(Ǐ.Equals(
$"[{ň}]")){string[]Ǟ=Ǎ.Split(new[]{'='},2);if(Ǟ.Length>=1&&Ǟ[0]==ǟ){if(Ǟ.Length>=2){if(ǜ)ǝ=Ǟ[1].Trim();else ǝ=Ǟ[1];return true;}
else return false;}}}return false;}public string Ǜ(){Ǣ=ǡ.CustomData;return Ǣ;}public bool ǚ(string Ǚ,ref string ǘ){Ǣ=ǡ.
CustomData;var ǐ=Ǣ.Split('\n');foreach(string Ǎ in ǐ){string Ǔ=Ǎ.Trim();if(Ǔ.StartsWith("[")&&Ǔ.EndsWith("]")&&System.Text.
RegularExpressions.Regex.IsMatch(Ǔ,Ǚ,System.Text.RegularExpressions.RegexOptions.IgnoreCase)){ǘ=Ǔ.Substring(1,Ǔ.Length-2);return true;}}
return false;}}class ǥ{static private Program Ǧ;private List<ż>Ț;private UpdateFrequency ȏ;private const double Ȏ=0.00001;
private int ȍ,Ȍ,ȋ,Ȋ;private bool ȉ=false;public bool Ƅ{get{return ȉ;}}private bool Ȉ,Ȇ,ȅ,Ȅ,ȃ,Ȃ;public bool ȁ;public enum ȇ{Ȁ,Ų,
ț,ǅ,ǃ,Ƽ,Ǆ,ș,Ș}public enum ȗ{Ȗ,ȕ,Ȕ}private Action<string,bool,bool>ȓ;public ǥ(Program Ȓ,List<ż>ȑ,UpdateFrequency Ȑ,List<
IMyThrust>[,]Ǭ,bool ǿ=false,bool Ǵ=true,bool ǧ=true,bool ǲ=true,bool Ǳ=true,Action<string,bool,bool>ǰ=null,int ǯ=-1,int Ǯ=-1){Ǧ=Ȓ
;Ț=ȑ;ȏ=Ȑ;if(!Ǵ)Ȃ=true;else Ȃ=false;Ȇ=ǿ;ȅ=Ǵ;Ȅ=ǧ;ȃ=ǲ;Ȉ=Ǳ;ȓ=ǰ;ȋ=ǯ;Ȋ=Ǯ;}public void ǭ(ȗ ǳ,int L,ref string ǫ){foreach(ż Ž in
Ț){if(Ž.ƀ.IsUnderControl){if(L==-1||L==Ž.Ż){switch(ǳ){case ȗ.Ȗ:Ž.Ƅ=true;break;case ȗ.ȕ:Ž.Ƅ=false;break;case ȗ.Ȕ:Ž.Ƅ=!Ž.Ƅ;
break;}}if(ȅ)ſ(!Ž.Ƅ);if(Ȅ)Ž.ƀ.SetValue<bool>("ControlGyros",!Ž.Ƅ);if(ȃ)Ž.ƀ.ControlWheels=!Ž.Ƅ;}}ȉ=Ǫ();}private bool Ǫ(){bool
ǩ=false;foreach(ż Ž in Ț)if(Ž.Ƅ)ǩ=true;return ǩ;}public void Ǩ(ref List<IMyThrust>[,]Ǭ,ref string ǫ){int ǻ=0;int Ǿ=0;
foreach(ż Ž in Ț){if(Ž.ƀ.IsUnderControl)ǻ++;else if(Ȇ){Ž.Ƅ=false;if(ȅ)Ž.ƀ.ControlThrusters=true;if(ȃ)Ž.ƀ.ControlWheels=true;if(
Ȅ)Ž.ƀ.SetValue<bool>("ControlGyros",true);}if(Ž.Ƅ)Ǿ++;}ȉ=Ǫ();if(Ȃ&&(Ǿ>Ȍ)){if(Ǿ>=ǻ)Ǽ(true,ref Ǭ);else{foreach(ż Ž in Ț){if
(Ž.Ƅ)Ž.ƀ.ControlThrusters=false;else Ž.ƀ.ControlThrusters=true;}}}if(Ȃ&&(Ǿ<Ȍ)){if(ǻ==0||(Ǿ<ǻ))Ǽ(false,ref Ǭ);foreach(ż Ž
in Ț){if(Ž.Ƅ)Ž.ƀ.ControlThrusters=false;else Ž.ƀ.ControlThrusters=true;}}ǫ="ctrl="+ǻ.ToString()+" ;wasd="+Ǿ.ToString()+
"; aThr="+ȁ;if(ȉ)Ǧ.Runtime.UpdateFrequency=UpdateFrequency.Update1;else Ǧ.Runtime.UpdateFrequency=ʄ;ȍ=ǻ;Ȍ=Ǿ;}public ȇ ǽ(ref
string ǫ){ȇ Ŝ=ȇ.Ȁ;if(ȉ){foreach(ż Ž in Ț){if(Ž.Ƅ){Ž.Ɠ=ȇ.Ȁ;if(Ž.ƀ.MoveIndicator.GetDim(0)<-Ȏ)Ž.ƕ=ȇ.ǅ;else if(Ž.ƀ.MoveIndicator.
GetDim(0)>Ȏ)Ž.ƕ=ȇ.ǃ;else if(Ž.ƀ.MoveIndicator.GetDim(1)<-Ȏ)Ž.ƕ=ȇ.Ș;else if(Ž.ƀ.MoveIndicator.GetDim(1)>Ȏ)Ž.ƕ=ȇ.ș;else if(Ž.ƀ.
MoveIndicator.GetDim(2)<-Ȏ)Ž.ƕ=ȇ.Ų;else if(Ž.ƀ.MoveIndicator.GetDim(2)>Ȏ)Ž.ƕ=ȇ.ț;if(Ž.ƀ.RollIndicator<-Ȏ)Ž.ƕ=ȇ.Ƽ;else if(Ž.ƀ.
RollIndicator>Ȏ)Ž.ƕ=ȇ.Ǆ;if(Ȉ){if(Math.Abs(Ž.ƀ.MoveIndicator.GetDim(0)+Ž.ƀ.MoveIndicator.GetDim(1)+Ž.ƀ.MoveIndicator.GetDim(2)+Ž.ƀ.
RollIndicator)<Ȏ){Ž.Ɠ=Ž.ƕ;Ž.ƕ=ȇ.Ȁ;}if(Ž.ƒ>40){Ž.Ɠ=Ž.ƕ;Ž.ƒ=32;}}else{Ž.Ɠ=Ž.ƕ;Ž.ƕ=0;}if(Ž.ƕ==ȇ.Ȁ)Ž.ƒ=0;else Ž.ƒ++;if(Ž.Ɠ>Ŝ)Ŝ=Ž.Ɠ;}}}
return Ŝ;}private void Ǽ(bool ž,ref List<IMyThrust>[,]Ǻ){ȁ=ž;if(ž){Vector3D ǹ=Ț[0].ƀ.GetNaturalGravity();if(ǹ.Length()>0.001){
Vector3D Ǹ=ǹ*Ț[0].ƀ.CalculateShipMass().PhysicalMass;Vector3D Ƿ=Ț[0].ƀ.WorldMatrix.Forward;Vector3D Ƕ=Ț[0].ƀ.WorldMatrix.Left;
Vector3D ǵ=Ț[0].ƀ.WorldMatrix.Up;float Ǘ=(float)Ƿ.Dot(Ǹ);float ǋ=(float)Ƕ.Dot(Ǹ);float Ű=(float)ǵ.Dot(Ǹ);Ƃ(Ǻ[0,1],ǋ/ƃ(Ǻ[0,1]));Ƃ
(Ǻ[0,2],-ǋ/ƃ(Ǻ[0,2]));Ƃ(Ǻ[0,3],Ű/ƃ(Ǻ[0,3]));Ƃ(Ǻ[0,4],-Ű/ƃ(Ǻ[0,4]));Ƃ(Ǻ[0,5],Ǘ/ƃ(Ǻ[0,5]));Ƃ(Ǻ[0,6],-Ǘ/ƃ(Ǻ[0,6]));if(ȋ>=0)ȓ
?.Invoke(ȋ.ToString(),true,true);}else ſ(false);}else{foreach(IMyThrust Ő in Ǻ[0,0]){Ő.ThrustOverride=0;}if(Ȋ>=0)ȓ?.
Invoke(Ȋ.ToString(),true,true);ſ(true);}}private void Ƃ(List<IMyThrust>ō,float Ɓ){if(Ɓ>0)foreach(IMyThrust Ő in ō){Ő.
ThrustOverridePercentage=Ɓ;}else foreach(IMyThrust Ő in ō){Ő.ThrustOverride=1;}}private float ƃ(List<IMyThrust>ō){float Å=0;foreach(IMyThrust Ő
in ō){Å+=Ő.MaxEffectiveThrust;}return Å;}private void ſ(bool ž){foreach(ż Ž in Ț){Ž.ƀ.ControlThrusters=ž;}}public class ż{
public IMyShipController ƀ;public int Ż;public bool Ƅ=false;public ȇ ƕ=ȇ.Ȁ;public ȇ Ɠ=ȇ.Ȁ;public int ƒ=0;public ż(
IMyShipController Ƒ,int Ɛ){ƀ=Ƒ;Ż=Ɛ;}}}class Ə{private TimeSpan Ǝ,ƍ;private string ƌ;private bool Ƌ=false;private Action<string,bool,bool>
Ɗ;private enum Ɖ{ƈ,Ƈ,Ɔ}private Ɖ ƅ=Ɖ.ƈ;public Ə(TimeSpan ũ,string ť,TimeSpan Ů,TimeSpan ŭ,bool Ŭ,Action<string,bool,bool>
ū){ƌ=ť;Ƌ=Ŭ;Ǝ=ũ+Ů;ƍ=Ǝ+ŭ;Ɗ=ū;}public void Ū(TimeSpan ũ){if(ũ>=Ǝ&&ƅ==Ɖ.ƈ){Ɗ?.Invoke(ƌ,Ƌ,false);ƅ=Ɖ.Ƈ;}if(ũ>=ƍ&&ƅ==Ɖ.Ƈ){Ɗ?.
Invoke(ƌ,Ƌ,true);ƅ=Ɖ.Ɔ;}}static public bool ů(List<Ə>ŧ,string ť){for(int Ë=0;Ë<ŧ.Count();Ë++){if(ŧ[Ë].ƌ==ť)return true;}return
false;}static public void Ũ(List<Ə>ŧ){for(int Ë=ŧ.Count()-1;Ë>=0;Ë--){if(ŧ[Ë].ƅ==Ɖ.Ɔ){ŧ[Ë]=null;ŧ.RemoveAt(Ë);}}}static
public void Ŧ(List<Ə>ŧ){for(int Ë=ŧ.Count()-1;Ë>=0;Ë--){ŧ[Ë]=null;ŧ.RemoveAt(Ë);}}}class ŷ{public string Ŵ;ǉ Ë,Ź;public
IMySensorBlock Ñ;public IMyAirVent Å;ƻ Ÿ;float ŏ;ǉ ź,Ü;string Ŷ;public static ŷ ŵ(string Ŵ,List<ǉ>ų,List<IMySensorBlock>Ų,List<
IMyAirVent>ű){var Ɣ=$"Error at Airlock '{Ŵ}':\n";var Ɩ=Ɣ.Length;if((ų?.Count??0)!=2)Ɣ+=
$" MUST have 2 doors, but found {ų?.Count??0}\n";if((Ų?.Count??0)>1)Ɣ+=$" MUST have 0-1 Sensors, but found {Ų?.Count??0}\n";if((ű?.Count??0)>1)Ɣ+=
$" MUST have 0-1 Vents, but found {ű?.Count??0}'\n";var Ʀ=new ŷ(Ŵ);if(Ɣ.Length==Ɩ){Ʀ.Ë=ų[0];Ʀ.Ź=ų[1];if(Ų?.Any()??false)Ʀ.Ñ=Ų[0];if(ű?.Any()??false)Ʀ.Å=ű[0];return Ʀ;}if(ų
!=null)foreach(ǉ ė in ų)Ɣ+=$"  Door '{ė.ǈ.CustomName}'\n";if(Ų!=null)foreach(IMySensorBlock ƿ in Ų)Ɣ+=
$"  Sensor '{ƿ.CustomName}'\n";if(ű!=null)foreach(IMyAirVent ƾ in ű)Ɣ+=$"  Vent '{ƾ.CustomName}'\n";Ʀ.Ŷ=Ɣ;return Ʀ;}public ŷ(string ƽ){Ŵ=ƽ;}public
void Ƽ(float ı){ƻ ǀ=Ÿ;if(Ÿ==ƻ.ƹ){Ë.ǈ.Enabled=Ź.ǈ.Enabled=true;if(Ǌ(Ñ)&&Ñ.IsActive){if(Ø(Ë)){Ü=Ë;ź=Ź;Ÿ=ƻ.Ʒ;}else if(Ø(Ź)){Ü=Ź
;ź=Ë;Ÿ=ƻ.Ʒ;}}else{if(Ø(Ë))Æ(Ë,Ź,ı);else if(Ø(Ź))Æ(Ź,Ë,ı);}}else if(Ÿ==ƻ.Ƹ){if(Ü.ǈ.Status==DoorStatus.Closed)Ü.ǈ.Enabled=
false;else if(Ü.ǈ.Status!=DoorStatus.Closing)Ü.ǈ.CloseDoor();if(ŏ<=ı||(Ǌ(Ñ)&&Ñ.IsActive))ź.ǈ.CloseDoor();if(ź.ǈ.Status==
DoorStatus.Closed)Ÿ=ƻ.Ļ;}else if(Ÿ==ƻ.Ļ){if(Ǌ(Ñ)&&!Ñ.IsActive)Ÿ=ƻ.ƹ;else{ŏ=ı+Ü.Ǉ;Ÿ=ƻ.Ʒ;}}else if(Ÿ==ƻ.Ʒ){Ü.ǈ.Enabled=true;if(ź.ǈ.
Status!=DoorStatus.Closed){Æ(ź,Ü,ı);}else if(ŏ<=ı||(Ǌ(Å)&&Å.GetOxygenLevel()<0.0001)||Ø(Ü)){if(!Ø(Ü))Ü.ǈ.OpenDoor();ź.ǈ.
Enabled=false;ŏ=ı+Ü.ǆ;Ÿ=ƻ.ƶ;}}else if(Ÿ==ƻ.ƶ){ź.ǈ.CloseDoor();if(ŏ<=ı||(Ǌ(Ñ)&&!Ñ.IsActive))Ü.ǈ.CloseDoor();if(Ü.ǈ.Status==
DoorStatus.Closed){ź.ǈ.Enabled=true;Ÿ=ƻ.ƹ;}}else if(Ÿ==ƻ.ƺ){Ë.ǈ.Enabled=Ź.ǈ.Enabled=true;Ë.ǈ.CloseDoor();Ź.ǈ.CloseDoor();if(Ǌ(Å))Å
.Depressurize=true;Ÿ=ƻ.ƹ;}if(Ÿ!=ǀ)Ƽ(ı);}enum ƻ{ƺ,ƹ,Ƹ,Ļ,Ʒ,ƶ,}void Æ(ǉ ŋ,ǉ Ú,float ı){ź=ŋ;Ü=Ú;Ü.ǈ.CloseDoor();ŏ=ı+ź.ǆ;Ÿ=ƻ.Ƹ
;}bool Ø(ǉ ė){return ė.ǈ.Status==DoorStatus.Opening||ė.ǈ.Status==DoorStatus.Open;}bool Ǌ(IMyCubeBlock Ç){return Ç!=null&&
!Ç.Closed&&Ç.IsFunctional&&Ç.IsWorking;}public class ǉ{public IMyDoor ǈ;public float Ǉ,ǆ;public string Ʃ;public ǉ(IMyDoor
ƴ,string ǅ,float Ǆ,float ǃ){ǈ=ƴ;Ʃ=ǅ;ǆ=Ǆ;Ǉ=ǃ;}}public class ǂ{public List<ŷ>ǁ;public static ǂ Ƶ(ǣ Ƥ,string Ɨ,List<string>Ƣ
,ref string ơ){var Ơ=new List<IMyDoor>();var Ɵ=new List<IMySensorBlock>();var ƞ=new List<IMyAirVent>();var Ɲ=new
Dictionary<string,List<ŷ.ǉ>>();var ƣ=new Dictionary<string,List<IMySensorBlock>>();var Ɯ=new Dictionary<string,List<IMyAirVent>>()
;var ƚ=new Random();for(int Ë=0;Ë<Ƣ.Count;Ë++){string[]ƙ=Ƣ[Ë].Split(';');var Ĵ=Ɨ+ƙ[0];var Ƙ=ƙ[1];if(Ƙ=="gr"){
IMyBlockGroup œ;try{œ=ˮ.GetBlockGroupWithName(Ĵ);œ.GetBlocksOfType(Ơ);œ.GetBlocksOfType(Ɵ);œ.GetBlocksOfType(ƞ);}catch{ơ+=
$"Error: Group {Ĵ} not found\n";}}else{ơ+=$"Error in [AutoAirlocks] line: {Ë+1}\n";continue;}string ƛ=ƚ.Next().ToString()+Ĵ;float ƥ,ƫ;if(Ơ.Count>0&&
float.TryParse(ƙ[2],out ƥ)&&float.TryParse(ƙ[3],out ƫ)){foreach(IMyDoor ƴ in Ơ){if(!Ɲ.ContainsKey(ƛ))Ɲ[ƛ]=new List<ŷ.ǉ>();Ɲ[ƛ
].Add(new ŷ.ǉ(ƴ,ƛ,ƥ,ƫ));}}else{ơ+=$"Error in [AutoAirlocks] line: {Ë+1}\n";continue;}foreach(IMySensorBlock Ʋ in Ɵ){if(!ƣ
.ContainsKey(ƛ))ƣ[ƛ]=new List<IMySensorBlock>();ƣ[ƛ].Add(Ʋ);}foreach(IMyAirVent Ʊ in ƞ){if(!Ɯ.ContainsKey(ƛ))Ɯ[ƛ]=new
List<IMyAirVent>();Ɯ[ƛ].Add(Ʊ);}}var ư=Ɲ.Keys.Concat(ƣ.Keys).Concat(Ɯ.Keys).Distinct().ToList();var Ư=new List<ŷ>();foreach(
var Ʈ in ư){var ƭ=ŷ.ŵ(Ʈ,Ƴ(Ɲ,Ʈ),Ƴ(ƣ,Ʈ),Ƴ(Ɯ,Ʈ));if(ƭ.Ŷ==null)Ư.Add(ƭ);else ơ+=ƭ.Ŷ;}return new ǂ(Ư);}static ű Ƴ<Ƭ,ű>(
Dictionary<Ƭ,ű>ƪ,Ƭ Ʃ)where ű:class{if(ƪ.ContainsKey(Ʃ))return ƪ[Ʃ];else return null;}public ǂ(List<ŷ>ƨ){ǁ=ƨ;}public void Ƨ(float ı
){foreach(var Ʀ in ǁ)Ʀ.Ƽ(ı);}}}