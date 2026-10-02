/*
 * -----------------------------------------------------------------------
 * 			VECTOR THRUST OS 22116
 * -----------------------------------------------------------------------
 * 
 * 
 * 
 * DO NOT TOUCH THE CODE BELOW
 * ALL MODIFICATIONS AND CONFITURATION IS ON CUSTOM DATA
 * 
 * 
 * IF YOU WANT TO MODIFY THE NORMAL CONFIGURATION, ALL THE CONFIGURATION IS HANDLED BY THE 
 * *CUSTOM DATA* OF THE PROGRAMABLE BLOCK THAT THE SCRIPT IS RUNNING
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * DO NOT TOUCH THE CODE BELOW
 * ALL MODIFICATIONS AND CONFITURATION IS ON CUSTOM DATA
 * 
 * 
 * IF YOU WANT TO MODIFY THE NORMAL CONFIGURATION, ALL THE CONFIGURATION IS HANDLED BY THE 
 * *CUSTOM DATA* OF THE PROGRAMABLE BLOCK THAT THE SCRIPT IS RUNNING
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * DO NOT TOUCH THE CODE BELOW
 * ALL MODIFICATIONS AND CONFITURATION IS ON CUSTOM DATA
 * 
 * 
 * IF YOU WANT TO MODIFY THE NORMAL CONFIGURATION, ALL THE CONFIGURATION IS HANDLED BY THE 
 * *CUSTOM DATA* OF THE PROGRAMABLE BLOCK THAT THE SCRIPT IS RUNNING
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * DO NOT TOUCH THE CODE BELOW
 * ALL MODIFICATIONS AND CONFITURATION IS ON CUSTOM DATA
 * 
 * 
 * IF YOU WANT TO MODIFY THE NORMAL CONFIGURATION, ALL THE CONFIGURATION IS HANDLED BY THE 
 * *CUSTOM DATA* OF THE PROGRAMABLE BLOCK THAT THE SCRIPT IS RUNNING
 */
const string ˍ="dampeners";const string ˌ="cruise";const string ˋ="gear";const string ˊ="applytags";const string ˉ=
"applytagsall";const string ˈ="removetags";Ѧ ˇ;Ѧ ˆ;double ˁ=1.0/60.0;Vector3D ʅ=new Vector3D();bool ˀ=false;bool ʿ=false;bool ʾ=false;
bool ʽ=false;bool ʼ=false;bool ˎ=false;bool ʻ=false;bool ˏ=false;bool ͼ=false;double ͺ=0;double ͷ=0;double Ͷ=0;double ʹ=0;
float ͳ=0;string Ͳ="";StringBuilder ͱ=new StringBuilder();StringBuilder ͻ=new StringBuilder();StringBuilder Ͱ=new
StringBuilder();MyShipMass ˬ;Ѧ ˤ;Ѧ ˣ;Ѧ ˢ;Ѧ ˡ;Ѧ ˠ;Ѧ ˑ;bool ː=false;bool ˮ=true;string ʺ="VT:";string ʯ="VTLCD";const float ʣ=1f;const
float ʢ=1.5f;const float ʡ=0.1f;const float ʠ=9.81f;const float ʟ=0.1f*ʠ;bool ʞ=true;const string ʝ="c.damping";const string
ʜ="c.cubesizemode";const string ʛ="c.sprint";const string ʚ="c.thrusts";bool ʙ=false;bool ʘ=false;bool ʗ=false;bool ʖ=
false;double ʕ=0;StringBuilder ʤ=new StringBuilder();double ʔ=0;bool ʦ=false;ʇ ʹ=null;List<ʇ>ʷ=new List<ʇ>();List<
IMyShipController>ʶ=new List<IMyShipController>();List<IMyShipController>ʵ=new List<IMyShipController>();List<ʇ>ʴ=new List<ʇ>();List<ɗ>ʳ=
new List<ɗ>();List<IMyThrust>ʲ=new List<IMyThrust>();List<IMyTextPanel>ʱ=new List<IMyTextPanel>();List<IMyShipConnector>ʸ=
new List<IMyShipConnector>();List<IMyLandingGear>ʰ=new List<IMyLandingGear>();List<IMyGasTank>ʮ=new List<IMyGasTank>();List
<IMyTerminalBlock>ʭ=new List<IMyTerminalBlock>();List<List<ɗ>>ʬ=new List<List<ɗ>>();List<V>ʫ=new List<V>();List<IMyThrust
>ʪ=new List<IMyThrust>();List<IMyMotorStator>ʩ=new List<IMyMotorStator>();List<IMyBatteryBlock>ʨ=new List<IMyBatteryBlock
>();List<IMyBatteryBlock>ʥ=new List<IMyBatteryBlock>();List<IMyThrust>ͽ=new List<IMyThrust>();List<IMyMotorStator>Δ=new
List<IMyMotorStator>();List<ʇ>ε=new List<ʇ>();List<IMyTextPanel>γ=new List<IMyTextPanel>();List<IMyMotorStator>β=new List<
IMyMotorStator>();List<IMyThrust>α=new List<IMyThrust>();List<IMyTerminalBlock>ΰ=new List<IMyTerminalBlock>();List<double>ί=new List<
double>();bool ή=false;bool έ=true;Vector3D ά=Vector3D.Zero;double Ϋ=0;bool Ϊ=true;string Ω="|VT|";bool Ψ=false;bool Ŀ=true;
float Χ=0;bool Φ=true;Dictionary<string,object>δ=null;bool Υ=false;bool ζ=false;float τ=0;int ſ=0;bool ς=true;bool ρ=false;ĕ
π;Vector3D ο=Vector3D.Zero;Program(){Ͱ.AppendLine("Program() Start");ξ();Τ=new ѡ(this,100,500);ˇ=new Ѧ(this,H(),true);ˆ=
new Ѧ(this,M(),true);ˤ=new Ѧ(this,ğ(),true);ˣ=new Ѧ(this,ť(),true);ˢ=new Ѧ(this,Ų(),true);ˡ=new Ѧ(this,ů(),true);ˠ=new Ѧ(
this,Œ(),true);ˑ=new Ѧ(this,Ν(),true);π=new ĕ(ʫ,this);ç();if(!ʻ)Runtime.UpdateFrequency=UpdateFrequency.Update1;Echo(Ͱ.
ToString());Ͱ.AppendLine("--VTOS Started--");}void Save(){string σ=string.Join(";",string.Join(":",Ω,Ŀ),Ѳ,ѻ,ː);Storage=σ;}void ξ
(){string[]μ=Storage.Split(new string[]{";"},StringSplitOptions.RemoveEmptyEntries);if(μ.Length>0){string[]λ=μ[0].Split(
new string[]{":"},StringSplitOptions.RemoveEmptyEntries);if(λ.Length==2){Ͳ=λ[0];Ŀ=bool.Parse(λ[1]);}if(μ.Length>=2)Ѳ=bool.
Parse(μ[1]);if(μ.Length>=3){int Ɉ=int.Parse(μ[2]);if(Ѽ.Count-1<Ɉ)Ɉ=Ѽ.Count-1;ѻ=Ɉ;}if(μ.Length>=4){ː=bool.Parse(μ[3]);;}}}bool
κ=false;bool ι=false;Vector3D θ=Vector3D.Zero;int Ý=60;float η=0;bool ν=false;ѡ Τ;void Main(string Β){Β=Β.ToLower();Τ.Ǵ()
;Ѐ();if(!ρ||Β.Length>0||κ){MyShipVelocities Α=ʹ.ɫ.GetShipVelocities();ά=Α.LinearVelocity;Ϋ=ά.Length();if(!ρ||Β.Length>0){
bool ΐ=ʹ.ɫ.DampenersOverride;ˀ=ΐ!=ˏ;ˏ=ΐ;ʅ=İ(Β,ʿ);ͷ=ʅ.Length();ν=ͷ==0&&Ϋ<ѽ;в();Ч();}}Ю(Β.Length>0);ŭ();if(Ы(Β))return;float Ώ
=ˬ.PhysicalMass;ο=ʹ.ɫ.GetNaturalGravity();ͳ=(float)ο.Length();bool Ύ=Math.Abs(Χ-ͳ)>0.05f;ͺ=Χ=ͳ;if(ͳ<ʟ)ͳ=ʠ;Vector3D Ό=Ώ*ο;
if(ˮ){Vector3D Ί=Vector3D.Zero;if(ʅ!=Vector3D.Zero){if(ƈ.ƶ(ʅ,ά)<0){Ί+=ɪ.ɹ(ά,ʅ.Ǎ());}Ί+=ɪ.ɵ(ά,ʅ.Ǎ());}else{Ί+=ά;}if(ː){if(ʭ
.Count>0&&!ʽ){ʽ=true;foreach(IMyFunctionalBlock I in ʭ)I.Enabled=false;}foreach(ʇ Ŭ in ʴ){if((Ĺ()&&Ŭ!=ʹ)||!Ŭ.ɫ.
IsUnderControl)continue;if(ƈ.ƶ(Ί,Ŭ.ɫ.WorldMatrix.Forward)>0||ҙ){Ί-=ɪ.ɹ(Ί,Ŭ.ɫ.WorldMatrix.Forward);}if(ҙ){Ό-=ɪ.ɹ(Ό,Ŭ.ɫ.WorldMatrix.
Forward);}}}else if(!ː&&ʽ){ʽ=false;ʭ.ForEach(I=>(I as IMyFunctionalBlock).Enabled=true);}ː=Ϊ||(ͺ!=0&&Ϋ==0)||κ||ʭ.ƺ()||ı||ʿ||ʾ||
ˇ.с?ː:ʭ.All(Q=>!(Q as IMyFunctionalBlock).Enabled);ʅ-=Ί*ʡ;}θ=ά;ʅ*=Ώ*(float)Ͷ;Vector3D ɓ=-ʅ+Ό;Vector3D Ή=Vector3D.Zero;
foreach(IMyThrust W in ʲ){Ή+=W.WorldMatrix.Backward*W.CurrentThrust;}double Έ=Ή.Length();ɓ+=Ή;Ε=ɓ.Length();У(ref ɓ,Ώ);Σ=ɓ;Ο=Ύ;Π
=Έ;ˑ.н();Ϊ=false;}List<double>Γ=new List<double>();double Ά=0;double Ε=0;Vector3D Σ=Vector3D.Zero;double Π=0;bool Ο=false
;List<int>Ξ=new List<int>{1,1};IEnumerable<double>Ν(){while(true){Vector3D ɓ=Σ;double Μ=0;double Λ=0;double Ρ=0;int Κ=ʬ.
Count;List<List<ɗ>>Ι=new List<List<ɗ>>(ʬ);for(int Ä=0;Ä<Κ;Ä++){List<ɗ>Ɉ=Ι[Ä];int Θ=Ɉ.Count;if(Θ<=0)continue;int Η=Ä+1;int Ť=Ɉ
.Count(Q=>Q.ɒ>0).Ƹ(1,Θ);Vector3D Ζ=ɪ.ɵ(ɓ,Ɉ[0].Ř.ɫ.WorldMatrix.Up);if(Ɉ[0].Ř.ɾ&&Vector3D.Dot(Ζ,Ɉ[0].Ř.ɫ.WorldMatrix.Left)
<=0){Ζ=ɪ.ɵ(Ζ,Ɉ[0].Ř.ɫ.WorldMatrix.Right);}if(Η<Κ&&Γ[Η]>0&&Γ[Ä]>=Ζ.Length()){ɗ ʧ=ʬ[Η][0];Vector3D ʓ=ɪ.ɵ(Ζ,ʧ.Ř.ɫ.WorldMatrix
.Up);if(ʧ.Ř.ɾ){bool ɂ=Vector3D.Dot(ʓ,ʧ.Ř.ɫ.WorldMatrix.Left)>0;if(!ɂ)ʓ=ɪ.ɵ(ʓ,ʧ.Ř.ɫ.WorldMatrix.Right);}ʓ*=0.15;ʓ=ʓ.Ƹ(0.01
,Γ[Η]);Ζ-=ʓ;}Γ[Ä]=0;Ζ/=Ť;int ɛ=(Ɉ.Count/Ξ[0]).Ƹ(1,Ɉ.Count);List<ɗ>ɚ=new List<ɗ>(Ɉ);for(int Ŗ=0;Ŗ<Θ;Ŗ++){ɗ y=ɚ[Ŗ];bool ə=ɛ
!=Θ;if(!В(y.Ř.ɫ))continue;if(!y.ɖ.ƺ()&&(Ο||!y.Ⱥ())){y.ȴ();}double ɘ=y.Ȼ();Γ[Ä]+=ɘ;if(ɘ<=0)ɘ=0.01;else Ρ+=ɘ;y.ɓ=Ζ.Ƹ(0.01,ɘ)
;y.ɨ();ɓ-=y.ɓ;Λ+=ɘ*1.595;Μ+=y.Ř.ɿ;if(ə){if(Ŗ+1%ɛ==0){yield return 0.016;}}}}if(Ρ!=0){Ά=Ρ;Ά+=Π;Ά/=ˬ.PhysicalMass;}Μ/=ʳ.
Count;Λ+=Π;ʔ=Μ;ʕ=Λ;yield return 0.016;}}class ɗ{Program U;public ʁ Ř;public List<ɇ>ɖ;public List<ɇ>ɕ;public List<ɇ>ɔ;public
Vector3D ɓ=Vector3D.Zero;public float ɒ=0;public int ɑ=0;public Vector3D ɜ=Vector3D.Zero;Ѧ ɐ;double ɞ=0;public ɗ(ʁ Ř,Program Ĝ){
this.U=Ĝ;this.Ř=Ř;this.ɖ=new List<ɇ>();this.ɕ=new List<ɇ>();this.ɔ=new List<ɇ>();ɐ=new Ѧ(Ĝ,ɧ(),true);}public void ɨ(){ɞ=Ř.ʄ(
ɓ);double ɦ=MathHelper.Clamp(U.қ,0,1);ɐ.н();}IEnumerable<double>ɧ(){while(true){double ɦ=MathHelper.Clamp(U.қ,0,1);double
ɥ=((((ɞ+1)*(1+ɦ)/2)-ɦ)*(((ɞ+1)*(1+ɦ))/2)).Ƹ(0,1);List<ɇ>ɩ=new List<ɇ>(ɔ);int ɛ=(ɩ.Count/U.Ξ[1]).Ƹ(1,ɩ.Count);for(int Ä=0;
Ä<ɩ.Count;Ä++){ɇ Ɔ=ɩ[Ä];bool ə=ɛ!=ɩ.Count;if(ə){ɥ=((((ɞ+1)*(1+ɦ)/2)-ɦ)*(((ɞ+1)*(1+ɦ))/2)).Ƹ(0,1);}Vector3D ɣ=(ɥ*ɓ*Ɔ.ɫ.
MaxEffectiveThrust/ɒ);bool ɢ=ɣ.LengthSquared()<0.001f||(U.ͺ==0&&ɞ<0.85);if(!U.Φ||ɢ){Ɔ.ʂ(0);if(Ɔ.Ƀ)Ɔ.ɫ.Enabled=Ɔ.Ƀ=false;Ɔ.ɀ=!U.Φ||ɢ;}else{
Ɔ.ʂ(ɣ);if(!Ɔ.Ƀ)Ɔ.ɫ.Enabled=Ɔ.Ƀ=true;Ɔ.ɀ=false;}if(ə&&Ä+1%ɛ==0){yield return 0.016;}}yield return 0.016;}}public void ɡ(){
bool ɠ=false;IMyMotorStator ɟ=Ř.ɫ;MatrixD ɤ=ɟ.WorldMatrix;Vector3D ɏ=ɤ.Up;foreach(List<ɗ>Ɉ in U.ʬ){if(Ɉ.ƺ())continue;
IMyMotorStator Ⱦ=Ɉ[0].Ř.ɫ;MatrixD Ƚ=Ⱦ.WorldMatrix;Vector3D ȼ=Ƚ.Up;if(Vector3D.Dot(ɏ,ȼ).ƾ()>0.9){if(Ř.ɾ.Equals(Ɉ[0].Ř.ɾ)){if((Ř.ɾ&&
Vector3D.Dot(ɤ.Left,Ƚ.Left)>0.9)||!Ř.ɾ){if(!Ɉ.Contains(this))Ɉ.Add(this);ɠ=true;break;}}}}if(!ɠ){U.ʬ.Add(new List<ɗ>());U.ʬ[U.ʬ.
Count-1].Add(this);U.Γ.Add(0);}}public double Ȼ(){ɒ=0;foreach(ɇ W in ɔ){ɒ+=W.ɫ.MaxEffectiveThrust;}return ɒ;}public bool Ⱥ(){
bool ȹ=false;foreach(ɇ ȸ in ɖ){bool ȷ=((U.ͺ==0||!ȸ.Ʌ||U.Ϊ)&&ȸ.ɫ.IsFunctional)||(ȸ.Ʌ&&ȸ.ɫ.Ƈ());if(ɕ.Contains(ȸ)){bool ȶ=ȸ.Ɇ&&
!ȸ.ɫ.Enabled&&!ȸ.ɀ;if((!ȷ||ȶ)){if(ȶ)ȸ.Ɇ=false;ɕ.Remove(ȸ);ȹ=true;}}else{bool ȵ=!ȸ.Ɇ&&ȸ.ɫ.Enabled;if(ȷ&&(ȵ||(ȸ.Ɇ&&!ɕ.
Contains(ȸ)))){ɕ.Add(ȸ);ȹ=true;ȸ.Ɇ=true;}}}return!ȹ;}public void ȴ(){Vector3D ȳ=Vector3D.Zero;Vector3D Ȳ=Vector3D.Zero;Vector3I
ȱ=Vector3I.Zero;Base6Directions.Direction ȿ=Ř.ɫ.Top.Orientation.Up;foreach(ɇ W in ɕ){Base6Directions.Direction Ɂ=W.ɫ.
Orientation.Forward;if(!(Ɂ==ȿ||Ɂ==Base6Directions.GetFlippedDirection(ȿ))){var Ɏ=Base6Directions.GetVector(Ɂ);if(Ɏ.X<0||Ɏ.Y<0||Ɏ.Z<
0){Ȳ+=Base6Directions.GetVector(Ɂ)*W.ɫ.MaxEffectiveThrust;}else{ȳ+=Base6Directions.GetVector(Ɂ)*W.ɫ.MaxEffectiveThrust;}}
}double ƌ=Math.Max(ȳ.Z,Math.Max(ȳ.X,ȳ.Y));double Ɓ=Math.Min(Ȳ.Z,Math.Min(Ȳ.X,Ȳ.Y));double Ɍ;if(ƌ>-1*Ɓ){Ɍ=ƌ;}else{Ɍ=Ɓ;}
float ɋ=0.1f;if(Math.Abs(Ɍ-ȳ.X)<ɋ){ȱ.X=1;}else if(Math.Abs(Ɍ-ȳ.Y)<ɋ){ȱ.Y=1;}else if(Math.Abs(Ɍ-ȳ.Z)<ɋ){ȱ.Z=1;}else if(Math.
Abs(Ɍ-Ȳ.X)<ɋ){ȱ.X=-1;}else if(Math.Abs(Ɍ-Ȳ.Y)<ɋ){ȱ.Y=-1;}else if(Math.Abs(Ɍ-Ȳ.Z)<ɋ){ȱ.Z=-1;}else{return;}if(!ɕ.ƺ())Ř.ʀ=(
Vector3D)ȱ;Base6Directions.Direction ɉ=Base6Directions.GetDirection(ȱ);Ɋ(ɉ);}public void Ɋ(Base6Directions.Direction?ɉ=null,bool
ɍ=false){if(!ɍ){foreach(ɇ W in ɖ){if(!W.Ɇ)W.ɫ.Enabled=false;}ɔ.Clear();}foreach(ɇ W in ɕ){Base6Directions.Direction Ɂ=W.ɫ
.Orientation.Forward;if((ɉ==Ɂ||ɍ)&&((W.ɫ.MaxEffectiveThrust!=0&&W.ɫ.Enabled)||(!U.ʿ&&!W.ɫ.Enabled))){W.ɫ.Enabled=true;W.Ɇ
=true;if(!ɔ.Contains(W))ɔ.Add(W);}}}}class ɇ:ɬ<IMyThrust>{public bool Ɇ;public bool Ʌ;float Ʉ=0;public bool Ƀ;public bool
ɀ=true;public ɇ(IMyThrust Ɔ,Program Ĝ):base(Ɔ,Ĝ){this.Ɇ=false;this.ɫ.Enabled=true;Ƀ=true;Ʌ=ɫ.BlockDefinition.SubtypeId.
Contains("Hydrogen");}public void ʂ(Vector3D ʃ){ʂ(ʃ.Length());}public void ʂ(double ɣ){if(ɣ>ɫ.MaxThrust){ɣ=ɫ.MaxThrust;}else if(
ɣ<0){ɣ=0;}float ł=(float)(ɣ*ɫ.MaxThrust/ɫ.MaxEffectiveThrust);if((ł!=0&&(ł.ƾ()-Ʉ.ƾ()).ƾ().ƃ(4)/ɫ.MaxThrust<0.0075)||(ł==0
&&Ʉ==0)){return;}ɫ.ThrustOverride=Ʉ=ł;}}class ʁ:ɬ<IMyMotorStator>{public Vector3D ʀ=Vector3D.Zero;public double ɿ=0;public
bool ɾ{get;}и ɽ;int ɼ=0;float ɻ=0;public ʁ(IMyMotorStator Ř,Program Ĝ):base(Ř,Ĝ){U=Ĝ;ɽ=new и(4,0,0,1.0/60.0);ɾ=ɫ.
BlockDefinition.SubtypeId.Contains("Hinge");}public double ʄ(Vector3D ɓ){Vector3D ʅ=ɓ.Ǎ();Vector3D ʒ=Vector3D.TransformNormal(ʀ,ɫ.Top.
CubeGrid.WorldMatrix);double ʐ=U.ѽ*U.ȁ;double ɞ=ɪ.ɝ(ʅ,ʒ);double ʏ=ɞ*100;double Ȧ=Math.Acos(ɞ)*2;Vector3D ʎ=Vector3D.Cross(ʅ,ʒ);Ȧ
*=Math.Sign(Vector3D.Dot(ʎ,ɫ.WorldMatrix.Up));if(ɓ.Length()<ʐ&&U.Φ){if(((U.ͺ==0&&U.ˮ)||(U.ͺ!=0))&&U.Φ&&Math.Abs(ʏ-ɿ)<=U.ғ
&&ʏ<90){ɽ.з+=U.Ҟ[0];}else if(ʏ>98||Math.Abs(ʏ-ɿ)>U.ғ){ɽ.з=U.Ҟ[0];}}else if(!U.Φ){ɽ.з=U.Ҟ[1];}else{ɽ.з=U.Ҟ[2];}ɿ=ʏ;float ǅ=
(float)ɽ.ф(Ȧ);if(ɾ){if(U.җ)U.Б($"- {ʏ.Ƽ(4)} - {ɼ}");if(ʏ<=-99.89){ɼ++;if(ɼ>10)ǅ=-ǅ;}else if(ɼ>10&&ʏ>-98.5){ɼ=0;}}if((ǅ.ƾ(
)-ɻ.ƾ()).ƾ()<0.03||(ɻ==0&&ǅ==0)){return ɞ;}ɫ.TargetVelocityRad=ɻ=ǅ;return ɞ;}public bool ʍ(Vector3D ʑ,Vector3D ʌ){
Vector3D Ȧ=Vector3D.Cross(ʑ,ʌ);double ʊ=Vector3D.Dot(Ȧ,ɫ.WorldMatrix.Up);return ʊ>=0;}public double ʉ(Vector3D Ƶ,Vector3D I){
double ʈ=Vector3D.Dot(Ƶ,I);return ʈ/I.Length();}}class ʇ:ɬ<IMyShipController>{public bool ʆ;public List<IMyThrust>ʋ=new List<
IMyThrust>();public List<IMyThrust>ɺ=new List<IMyThrust>();public ʇ(IMyShipController ɰ,Program Ĝ):base(ɰ,Ĝ){ʆ=ɰ.
DampenersOverride;}public void ɯ(bool ł){ʆ=ł;ɫ.DampenersOverride=ł;}}interface ɮ{IMyTerminalBlock ɫ{get;set;}string ɭ{get;}}abstract
class ɬ<Ś>:ɮ where Ś:class,IMyTerminalBlock{public Ś ɫ{get;set;}public Program U;public ɬ(Ś Ġ,Program U){this.U=U;ɫ=Ġ;}
IMyTerminalBlock ɮ.ɫ{get{return ɫ;}set{ɫ=(Ś)value;}}public string ɭ=>ɫ.CustomName;}static class ɪ{public static Vector3D ɱ(Vector3D Ƶ){
if(Vector3D.IsZero(Ƶ))return Vector3D.Zero;if(Vector3D.IsUnit(ref Ƶ))return Ƶ;return Vector3D.Normalize(Ƶ);}public static
Vector3D ɲ(Vector3D Ƶ,Vector3D I,double ɸ=1){Vector3D ɷ=ɹ(Ƶ,I);Vector3D ɶ=Ƶ-ɷ;return ɷ-ɶ*ɸ;}public static Vector3D ɵ(Vector3D Ƶ,
Vector3D I){if(Vector3D.IsZero(Ƶ)||Vector3D.IsZero(I))return Vector3D.Zero;return Ƶ-Ƶ.Dot(I)/I.LengthSquared()*I;}public static
Vector3D ɹ(Vector3D Ƶ,Vector3D I){if(Vector3D.IsZero(Ƶ)||Vector3D.IsZero(I))return Vector3D.Zero;if(Vector3D.IsUnit(ref I))
return Ƶ.Dot(I)*I;return Ƶ.Dot(I)/I.LengthSquared()*I;}public static double ɴ(Vector3D Ƶ,Vector3D I){if(Vector3D.IsZero(Ƶ)||
Vector3D.IsZero(I))return 0;if(Vector3D.IsUnit(ref I))return Ƶ.Dot(I);return Ƶ.Dot(I)/I.Length();}public static double ɳ(
Vector3D Ƶ,Vector3D I){if(Vector3D.IsZero(Ƶ)||Vector3D.IsZero(I))return 0;else return Math.Acos(MathHelper.Clamp(Ƶ.Dot(I)/Math.
Sqrt(Ƶ.LengthSquared()*I.LengthSquared()),-1,1));}public static double ɝ(Vector3D Ƶ,Vector3D I){if(Vector3D.IsZero(Ƶ)||
Vector3D.IsZero(I))return 0;else return MathHelper.Clamp(Ƶ.Dot(I)/Math.Sqrt(Ƶ.LengthSquared()*I.LengthSquared()),-1,1);}public
static bool Ѣ(Vector3D Ƶ,Vector3D I,double Ѡ){double ʈ=Vector3D.Dot(Ƶ,I);double ƻ=Ƶ.LengthSquared()*I.LengthSquared()*Ѡ*Math.
Abs(Ѡ);return Math.Abs(ʈ)*ʈ>ƻ;}}class џ{private bool ў;private double ѝ;private double ќ;public double ћ{get;private set;}
public double њ{get;private set;}public џ(int љ){ѝ=2.0/(љ+1);}public void ј(double ї){if(!ў){ћ=ї;њ=0;ќ=ћ;ў=true;return;}ћ=((ї-
ќ)*ѝ)+ќ;њ=ћ-ќ;ќ=ћ;}}class ѡ{public double і{get;private set;}public double ѣ{get;private set;}public double ѱ{get;private
set;}public bool Ѱ{get;private set;}public bool ѯ{get;private set;}public џ Ѯ;public int ѭ=0;int Ѭ=0;public int ѫ;Program U
;public ѡ(Program U,int ѩ,int Ѩ){this.U=U;ѫ=Ѩ;Ѯ=new џ(ѩ);}public void ѧ(int Ä){Ѯ=new џ(Ä);}public void Ǵ(){ѱ=U.Runtime.
LastRunTimeMs;Ѯ.ј(ѱ);ѣ=Ѯ.ћ;if(ѭ%ѫ==0)і=ѣ;else if(U.Runtime.LastRunTimeMs>і)і=ѱ;if(ѭ>=ѫ){ѭ=0;U.Ͱ.Clear();}if(U.Runtime.UpdateFrequency
==UpdateFrequency.Update1){ѭ++;Ѭ++;}else if(U.Runtime.UpdateFrequency==UpdateFrequency.Update10){ѭ+=10;Ѭ+=10;}else{ѭ+=100;
Ѭ+=100;}if(Ѱ)ѯ=true;else if(ѯ)ѯ=false;Ѱ=Ѭ>=U.ҕ;if(Ѱ)Ѭ=0;}}class Ѧ{public Program ѥ;public bool Ѥ{get;set;}public bool Ѫ{
get;private set;}public IEnumerable<double>ѕ;public double ь{get;private set;}private IEnumerator<double>т;public bool с{
get;set;}public Ѧ(Program Ĝ,IEnumerable<double>р=null,bool п=false){ѥ=Ĝ;ѕ=р;Ѥ=п;if(Ѥ){о();}}public void о(){с=false;л(ѕ);}
public void н(){if(т==null)return;ь-=ѥ.Runtime.TimeSinceLastRun.TotalSeconds;if(ь>0)return;bool м=т.MoveNext();if(м){ь=т.
Current;if(ь<=-0.5)м=false;}if(!м){if(Ѥ)л(ѕ);else л(null);}}private void л(IEnumerable<double>к){Ѫ=false;ь=0;т?.Dispose();т=
null;if(к!=null){Ѫ=true;т=к.GetEnumerator();}}public bool й(bool Ŕ){while(!с){н();if(Ŕ)return Ŕ;}с=false;return с;}}class и{
public double з{get;set;}=0;public double у{get;set;}=0;public double ж{get;set;}=0;public double х{get;private set;}double є=
0;double ђ=0;double ё=0;double ѐ=0;bool я=true;public и(double ю,double э,double ѓ,double ш){з=ю;у=э;ж=ѓ;є=ш;ђ=1/є;}
protected virtual double ы(double ъ,double щ,double ш){return щ+ъ*ш;}public double ф(double ʻ){double ч=(ʻ-ѐ)*ђ;if(я){ч=0;я=false
;}ё=ы(ʻ,ё,є);ѐ=ʻ;х=з*ʻ+у*ё+ж*ч;return х;}public double ф(double ʻ,double ш){if(ш!=є){є=ш;ђ=1/є;}return ф(ʻ);}public
virtual void ҥ(){ё=0;ѐ=0;я=true;}}class Ҥ:и{public double ң{get;set;}public Ҥ(double ю,double э,double ѓ,double ш,double Ң):
base(ю,э,ѓ,ш){ң=Ң;}protected override double ы(double ъ,double щ,double ш){return щ*(1.0-ң)+ъ*ш;}}class ҡ:и{public double Ҡ{
get;set;}public double ҟ{get;set;}public ҡ(double ю,double э,double ѓ,double ш,double ҫ,double Ҫ):base(ю,э,ѓ,ш){Ҡ=Ҫ;ҟ=ҫ;}
protected override double ы(double ъ,double щ,double ш){щ+=ъ*ш;return Math.Min(Ҡ,Math.Max(щ,ҟ));}}class ҩ:и{Queue<double>Ҩ=new
Queue<double>();public int Ҭ{get;set;}=0;public ҩ(double ю,double э,double ѓ,double ш,int Ҧ):base(ю,э,ѓ,ш){Ҭ=Ҧ;}protected
override double ы(double ъ,double щ,double ш){if(Ҩ.Count==Ҭ)Ҩ.Dequeue();Ҩ.Enqueue(ъ*ш);return Ҩ.Sum();}public override void ҥ(){
base.ҥ();Ҩ.Clear();}}string ҧ="VT";List<double>Ҟ=new List<double>{0.1,1,4};float ғ=6f;double ҁ=0.5;double Ҁ=0.01;double ѿ=1;
double Ѿ=0.15;double ѽ=1;List<double>Ѽ=new List<double>{15,50,100};int ѻ=0;double Ѻ=0;bool ѹ=true;bool Ѹ=true;string ѷ=
"Backup";bool Ѷ=true;bool ѵ=false;bool Ѵ=false;bool ѳ=false;bool Ҋ=true;bool Ѳ=false;List<double>ҋ=new List<double>{-1,-1,0};
bool ҝ=false;double қ=0.0000000001;string[]Қ=new string[]{"|","|"};bool ҙ=false;int Ҙ=1;bool җ=false;int Җ=0;int ҕ=10;bool Ҝ
=true;bool Ҕ=false;MyIni ǈ=new MyIni();MyIni Ғ=new MyIni();const string ґ="--------Vector Thrust 2 Settings--------";
const string Ґ="--Rotor Calibration Settings--";const string ҏ="--Acceleration Settings--";const string Ҏ=
"--Parking Settings--";const string ҍ="--Advanced Settings--";const string Ҍ="--Whip's Artificial Horizon Redux Settings--";const string ц=
"--Config Settings--";const string е="Name Tag";const string ϖ="Tag Surround Char(s)";const string ϴ=
"(Min/Thrust_Off/Max) Rotor Correction Aggressivity Level";const string ϳ="Margin No. For Error Correction";const string ϲ="Velocity To Turn On/Off VectorThrusters";const string
ϱ="Velocity To Turn On/Off VectorThrusters In Cruise";const string ϰ="Velocity To Trigger Presision Mode";const string ϯ=
"Accelerations (Total Thrust %)";const string Ϯ="Turn Off Thrusters On Park";const string ϭ="Set Batteries/Tanks to Recharge/Stockpile On Park";const
string Ϭ="Assign Backup Batteries With Surname";const string ϫ="Automatically Adds Small Batteries As Backup";const string Ϫ=
"Run Script Each 100 Frames When Parked";const string ϩ="Add Automatically Same Grid Connectors";const string Ϩ="Add Automatically Same Grid Landing Gears";
const string ϧ="Activate Park Mode If Parked In Static Grid or Ground";const string Ϧ=
"Direction Of Thrusters On Park (Vector X,Y,Z)";const string ϵ="Override Adjustment of Direction Of Thrusters in Gravity";const string ϥ=
"Gather Data Only From Main Cockpit";const string Ϸ="Cruise Mode Act Like Plane";const string Њ="Frames Per Operation: Task Splitter";const string Ј=
"Show Metrics";const string Ї="Skip Frames";const string І="Frames Where The Script Won't Print";const string Ѕ=
"Set Park Blocks/Normal Thrusters to Default (Recompile)";const string Є="Lines In The Sides";const string Ѓ="Reverse Velocity and Backward Arrow Indicator";const string Ђ=
"Sky Color (Gravity)";const string Љ="Horizon Line (Default: None)";const string Ё="Elevation Lines (Gravity)";const string Ͽ=
"DAMP, CRUISE, PARK Off Indicator";const string Ͼ="DAMP, CRUISE, PARK On Indicator";const string Ͻ="Forward Arrow Color";const string ϼ=
"DAMP, CRUISE, PARK Background";const string ϻ="Velocity Indicator Sensitivity";const string Ϻ="Dampeners Arrow Trigger Multiplier";const string Ϲ=
"Allow Control Module Mod";const string ϸ="Split Vector Thrusters / Individual Thrusters Tasks Frames";void Ѐ(){if(Me.CustomData.Equals(ϓ)&&ϓ.
Length>0)return;if(!Ϊ)Ͱ.ǒ("\n  >Configuration Edit Detected\n");ǈ.Clear();Ϥ();if(ǈ.TryParse(Me.CustomData)){ҧ=ǈ.Get(ґ,е).
ToString(ҧ);if(string.IsNullOrEmpty(ҧ)){ҧ="VT";}ʺ=$"{ҧ}:";ʯ=$"{ҧ}LCD";string ό=ǈ.Get(ґ,ϖ).ToString();int Ϟ=ό.Length;if(Ϟ==1){Қ=
new string[]{ό,ό};}else if(Ϟ>1&&Ϟ%2==0){string ϒ=ό.Substring(0,(int)(ό.Length/2));string ϑ=ό.Substring((int)(ό.Length/2),(
int)(ό.Length/2));Қ=new string[]{ϒ,ϑ};}else{Қ=new string[]{"|","|"};}И();var ϐ=ǈ.ǉ<double>(Ґ,ϴ);if(ϐ.Count==3)Х<double>(ref
Ҟ,ϐ);ғ=ǈ.Get(Ґ,ϳ).ToSingle(ғ);ѽ=ǈ.Get(Ґ,ϰ).ToDouble(ѽ);ϐ=new List<double>(ǈ.ǉ<double>(Ґ,ϲ));if(ϐ.Count==2){ҁ=ϐ[0];Ҁ=ϐ[1];
}ϐ=new List<double>(ǈ.ǉ<double>(Ґ,ϱ));if(ϐ.Count==2){ѿ=ϐ[0];Ѿ=ϐ[1];}ϐ=new List<double>(ǈ.ǉ<double>(ҏ,ϯ));if(ϐ.Count>1&&ϐ.
All(Q=>Q>0))Х<double>(ref Ѽ,ϐ);if(ѻ>Ѽ.Count-1)ѻ=Ѽ.Count-1;ѹ=ǈ.Get(Ҏ,Ϯ).ToBoolean(ѹ);Ѹ=ǈ.Get(Ҏ,ϭ).ToBoolean(Ѹ);ѷ=ǈ.Get(Ҏ,Ϭ).
ToString(ѷ);Ѷ=ǈ.Get(Ҏ,ϫ).ToBoolean(Ѷ);Ѵ=ǈ.Get(Ҏ,ϩ).ToBoolean(Ѵ);ѳ=ǈ.Get(Ҏ,Ϩ).ToBoolean(ѳ);Ҋ=ǈ.Get(Ҏ,ϧ).ToBoolean(Ҋ);ѵ=ǈ.Get(Ҏ,Ϫ)
.ToBoolean(ѵ);ϐ=new List<double>(ǈ.ǉ<double>(Ҏ,Ϧ));if(ϐ.Count==3)Х<double>(ref ҋ,ϐ);ҝ=ǈ.Get(Ҏ,ϵ).ToBoolean(ҝ);π.Ĕ=Ϝ(Ҍ,Є,ǈ
,π.Ĕ);π.ē=Ϝ(Ҍ,Ѓ,ǈ,π.ē);π.Ē=Ϝ(Ҍ,Ђ,ǈ,π.Ē);π.đ=Ϝ(Ҍ,Љ,ǈ,π.đ);π.Đ=Ϝ(Ҍ,Ё,ǈ,π.Đ);π.û=Ϝ(Ҍ,Ͻ,ǈ,π.û);π.Ė=Ϝ(Ҍ,Ͼ,ǈ,π.Ė);π.ď=Ϝ(Ҍ,Ͽ,ǈ,π
.ď);π.ï=Ϝ(Ҍ,ϼ,ǈ,π.ï);π.á=ǈ.Get(Ҍ,ϻ).ToSingle(π.á);π.à=ǈ.Get(Ҍ,Ϻ).ToSingle(π.à);Ҕ=ǈ.Get(ҍ,ϥ).ToBoolean(Ҕ);ҙ=ǈ.Get(ҍ,Ϸ).
ToBoolean(ҙ);Ҙ=ǈ.Get(ҍ,Њ).ToInt32(Ҙ);if(Ҙ<=0){Ҙ=1;}ώ=Ҙ*ˁ;ҕ=ǈ.Get(ҍ,І).ToInt32(ҕ);var Ϗ=new List<int>(ǈ.ǉ<int>(ҍ,ϸ));if(Ϗ.Count==2
&&Ϗ.All(Q=>Q>0))Х<int>(ref Ξ,Ϗ);Җ=ǈ.Get(ҍ,Ї).ToInt32(Җ);Ҝ=ǈ.Get(ҍ,Ѕ).ToBoolean(Ҝ);җ=ǈ.Get(ц,Ј).ToBoolean(җ);ʞ=ǈ.Get(ц,Ϲ).
ToBoolean(ʞ);}ύ();υ(ǈ.ToString());}double ώ=0;void ύ(){ǈ.Set(ґ,е,ҧ);ǈ.SetSectionComment(ґ," For more info, check Advanced I & II sections in the guide:\n https://steamcommunity.com/sharedfiles/filedetails/?id=2861711651\n "
);string ό=Қ[0].Equals(Қ[1])?Қ[0]:Қ[0]+Қ[1];ǈ.Set(ґ,ϖ,ό);string ϋ=String.Join(" ; ",Ҟ);ǈ.Set(Ґ,ϴ,ϋ);ǈ.Set(Ґ,ϳ,ғ);ǈ.Set(Ґ,
ϰ,ѽ);string ϊ=String.Join(" ; ",new double[]{ҁ,Ҁ});ǈ.Set(Ґ,ϲ,ϊ);string ω=String.Join(" ; ",new double[]{ѿ,Ѿ});ǈ.Set(Ґ,ϱ,ω
);string ψ=String.Join(" ; ",Ѽ);ǈ.Set(ҏ,ϯ,ψ);ǈ.Set(Ҏ,Ϯ,ѹ);ǈ.Set(Ҏ,ϭ,Ѹ);ǈ.Set(Ҏ,Ϭ,ѷ);ǈ.Set(Ҏ,ϫ,Ѷ);ǈ.Set(Ҏ,ϩ,Ѵ);ǈ.
SetComment(Ҏ,ϩ,"\n");ǈ.Set(Ҏ,Ϩ,ѳ);ǈ.Set(Ҏ,ϧ,Ҋ);ǈ.SetComment(Ҏ,ϧ,"\n");ǈ.Set(Ҏ,Ϫ,ѵ);string χ=String.Join(" ; ",ҋ);ǈ.Set(Ҏ,Ϧ,χ);ǈ.
SetComment(Ҏ,Ϧ,"\n");ǈ.Set(Ҏ,ϵ,ҝ);ϟ(Ҍ,Є,ǈ,π.Ĕ);ϟ(Ҍ,Ѓ,ǈ,π.ē);ϟ(Ҍ,Ђ,ǈ,π.Ē);ϟ(Ҍ,Љ,ǈ,π.đ);ϟ(Ҍ,Ё,ǈ,π.Đ);ϟ(Ҍ,Ͻ,ǈ,π.û);ϟ(Ҍ,Ͼ,ǈ,π.Ė);ϟ(Ҍ,Ͽ
,ǈ,π.ď);ϟ(Ҍ,ϼ,ǈ,π.ï);ǈ.Set(Ҍ,ϻ,π.á);ǈ.Set(Ҍ,Ϻ,π.à);ǈ.Set(ҍ,Ϸ,ҙ);ǈ.Set(ҍ,Њ,Ҙ);ǈ.Set(ҍ,І,ҕ);string φ=String.Join(" ; ",Ξ);ǈ
.Set(ҍ,ϸ,φ);ǈ.Set(ҍ,Ї,Җ);ǈ.Set(ҍ,Ѕ,Ҝ);ǈ.Set(ц,Ј,җ);ǈ.Set(ц,Ϲ,ʞ);}string ϓ="";void υ(string ϕ){if(ϕ!=Me.CustomData)Me.
CustomData=ϕ;try{if(!Me.CustomData.Contains($"\n---\n{ʺ}0"))Me.CustomData=Me.CustomData.Replace(Me.CustomData.ǂ("\n---\n","0")[0],
ʺ);}catch{if(!Ϊ)Ͱ.ǒ("No tag found textSufaceKeyword\n");}if(!Me.CustomData.Contains($"\n---\n{ʺ}0"))Me.CustomData+=
$"\n---\n{ʺ}0";ϓ=Me.CustomData;}void Ϥ(){if(Ϊ&&Ғ.TryParse(Me.CustomData)){ύ();List<MyIniKey>ϣ=new List<MyIniKey>();List<MyIniKey>Ϣ=new
List<MyIniKey>();Ғ.GetKeys(ϣ);ǈ.GetKeys(Ϣ);foreach(MyIniKey ϡ in ϣ){foreach(MyIniKey Ϡ in Ϣ){if(ϡ.Equals(Ϡ))ǈ.Set(Ϡ,Ғ.Get(ϡ)
.ToString());}}υ(ǈ.ToString());}Ғ.Clear();}static void ϟ(string ϛ,string Ϛ,MyIni ϙ,Color Ƅ){string ϝ=string.Format(
"{0}, {1}, {2}, {3}",Ƅ.R,Ƅ.G,Ƅ.B,Ƅ.A);ϙ.Set(ϛ,Ϛ,ϝ);}static Color Ϝ(string ϛ,string Ϛ,MyIni ϙ,Color?Ϙ=null){string ϗ=ϙ.Get(ϛ,Ϛ).ToString(
"null");string[]ϔ=ϗ.Split(',');int ű,Ɉ,I,Ƶ;if(ϔ.Length!=4){if(Ϙ.HasValue)return Ϙ.Value;else return Color.Transparent;}int.
TryParse(ϔ[0].Trim(),out ű);int.TryParse(ϔ[1].Trim(),out Ɉ);int.TryParse(ϔ[2].Trim(),out I);bool Ц=int.TryParse(ϔ[3].Trim(),out
Ƶ);if(!Ц)Ƶ=255;ű=MathHelper.Clamp(ű,0,255);Ɉ=MathHelper.Clamp(Ɉ,0,255);I=MathHelper.Clamp(I,0,255);Ƶ=MathHelper.Clamp(Ƶ,0
,255);return new Color(ű,Ɉ,I,Ƶ);}void Х<Ś>(ref List<Ś>Į,List<Ś>Ф){Į=new List<Ś>(Ф);}void У(ref Vector3D ɓ,float Т){if(Φ)
return;ʇ Ť=ʹ??ʴ[0];List<double>С=ҋ;Vector3D Р;Vector3D П=ҝ?Vector3D.Zero:ɓ;Vector3D О=ҝ?Vector3D.Zero:ɓ-ά;Р=(Ť.ɫ.WorldMatrix.
Forward*С[0]+Ť.ɫ.WorldMatrix.Up*С[1]+Ť.ɫ.WorldMatrix.Right*С[2])*ʠ;ɓ=ˮ?Р*Т+П:О+Р;ʼ=true;}double ȁ=0;void Ч(){ȁ=ʟ*ˬ.PhysicalMass
;double Н=Ѿ*ȁ;double Ш=ѿ*ȁ;if(ͷ!=0||(ˀ&&ˮ)||(!ː&&Ϋ>ҁ)||(ː&&Ε>Ш)||(κ&&ͺ!=0&&Ϋ!=0)){Φ=true;κ=false;}if(ͷ==0){bool д=ͺ!=0&&Ϋ
==0&&!ʿ;if((ͺ==0&&((!ː&&Ϋ<Ҁ)||((ː||!ˮ)&&Ε<Н)))||!(!ʿ||!ʾ)||д){Φ=false;if(д)κ=true;}}}double г=0;void в(){double б=ͳ*ʣ;
double а=ʕ/ˬ.BaseMass;Ѻ=а*Ѽ[ѻ]/100;г=Ά*Ѽ[ѻ]/100;ʹ=а*Ѽ[Ѽ.Count-1]/100;double Я=ʢ*б;bool Ŕ=ͷ==0&&!ː&&ˮ&&Ϋ>ѽ&&Ѻ>Я;Ͷ=ͷ!=0||Ŕ?Ѻ:Я;η
=!Φ||ν?(float)г.Ƽ(2):(float)((ά-θ)*Ý).Length();}void Ю(bool ȁ){if(!Τ.Ѱ&&!ȁ)return;Echo(ͱ.ToString());ͱ.Clear();π.Ǵ(ȁ);ͻ.
Clear();string Э=ʹ!=null?ʹ.ɫ.CustomName:"DEAD";if(җ){string Ð=$" {Τ.ѱ.Ƽ(3)}   {Τ.ѣ.Ƽ(3)}";ͱ.AppendLine(Ð);ͻ.AppendLine(Ð);}ͱ.
AppendLine("VT OS\n22116\n");if(Ŀ)ͱ.AppendLine("WARNING, TAGS ARE NOT APPLIED\nAPPLY THEM WITH \"applytags\"\n");if(ĸ<=ŋ)ͱ.
AppendLine($" > Thrusters Total Precision: {ʔ.Ƽ(1)}%");ͱ.AppendLine($" > Main/Ref Controller:\n  {Э}");ͱ.AppendLine(
$" > Runtime (MS):\n  {Runtime.LastRunTimeMs.Ƽ(3)} / Avg: {Τ.ѣ.Ƽ(3)} / Max: {Τ.і.Ƽ(3)}");if(Җ>0)ͱ.AppendLine($" > Skipping {Җ} Frames");ͱ.Append(ʤ);if(ͼ)ͱ.AppendLine(
"CAN'T FLY A STATION, RUNNING WITH LESS RUNTIME.");if(җ){StringBuilder Ь=new StringBuilder($"\n > Metrics:\n  Total VectorThrusters: {ʳ.Count}\n");Ь.AppendLine(
$"  Main/Ref Cont: {Э}");Ь.AppendLine($"  Parked: {ρ}/{ς}");Ь.AppendLine($"  ThrustOn: {Φ}");ͱ.Append(Ь);ͻ.Append(Ь);ͱ.Append(
$"\n > Log [{Τ.ѭ}/{Τ.ѫ}]:\n").Append(Ͱ);}}bool C=false;bool Ы(string Β){bool Ъ=Β.Contains(ˊ)||Β.Contains(ˌ)||Β.Contains(ˈ);bool Щ=false;if(!ͼ){ˤ.н()
;Щ=ō();Щ=Ŋ()||Щ;if(!ː)Щ=ķ()||Щ;if(Ъ)Й(Β);}if(ʻ){М();return true;}else if(ͼ){ʦ=ʦ||ʩ.All(Q=>Q.TargetVelocityRPM==0)&&ʪ.All(
Q=>!Q.Enabled&&Q.ThrustOverridePercentage==0);Ќ();return true;}return Щ;}void М(){if(ͺ==0){ʪ.ForEach(ª=>ª.ǎ());Ͱ.
AppendLine("0G Detected -> Braking Thrusters");}ʩ.ForEach(Ð=>Ð.ǎ());Ͱ.AppendLine("Braking Rotors");if(π!=null)π.ǝ();Echo(Ͱ.
ToString());Ũ(4);}bool В(IMyMotorStator Ð){return Ð!=null&&Ð.Top!=null&&GridTerminalSystem.CanAccess(Ð);}void Б(string ǫ,bool А=
true,params object[]Џ){if(!Τ.Ѱ)return;StringBuilder ǅ=Џ.Length!=0?new StringBuilder().Append(string.Join(ǫ,Џ)):new
StringBuilder(ǫ);ͻ.Append(ǅ+"\n");if(А)ͱ.Append(ǅ+"\n");}void Ў<Ś>(ref List<Ś>Ѝ){Ѝ=Ѝ.Distinct().ToList();}bool ǐ(IMyTerminalBlock Ġ){
return Ġ.CubeGrid==Me.CubeGrid;}void Ќ(){foreach(ɗ Ŏ in ʳ){foreach(ɇ W in Ŏ.ɖ){W.ɫ.ǎ();W.ɫ.Enabled=false;}Ŏ.Ř.ɫ.ǎ();}}ʇ Ћ(){if
(ʹ.ɫ.IsWorking)return ʹ;foreach(ʇ Ŭ in ʴ){if(Ŭ.ɫ.IsWorking){return Ŭ;}}return null;}void Л(bool К=true){Ŧ();έ=true;if(К)ˤ
.н();}void Й(string Β){ˎ=Β.Contains(ˉ);this.Ψ=Β.Contains(ˊ)||ˎ;this.Ŀ=(!this.Ψ&&this.Ŀ);if(this.Ψ){Е(Me);}else if(Β.
Contains(ˈ))И(true,false);Л();ˎ=false;this.Ψ=false;}void И(bool ȁ=false,bool З=true){Ω=Қ[0]+ҧ+Қ[1];bool Æ=Ͳ.Length>0;bool Å=!Ω.
Equals(Ͳ)&&Me.CustomName.Contains(Ͳ);bool w=Ŀ&&Me.CustomName.Contains(Ͳ);if(Æ&&(Å||w||ȁ)){if(З)Ͱ.ǒ(
" -Cleaning Tags To Prevent Future Errors, just in case\n");else Ͱ.ǒ(" -Removing Tags\n");List<IMyTerminalBlock>Í=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocksOfType<
IMyTerminalBlock>(Í);foreach(IMyTerminalBlock Ġ in Í)Г(Ġ);}this.Ŀ=!Ж(Me);Ͳ=Ω;}bool Ж(IMyTerminalBlock Ġ){return Ġ.CustomName.Contains(Ω)
;}void Е(IMyTerminalBlock Ġ){string Д=Ġ.CustomName;if(!Д.Contains(Ω)){Ͱ.ǒ("Adding tag:"+Ġ.CustomName+"\n");Ġ.CustomName=Ω
+" "+Д;}}void Г(IMyTerminalBlock Ġ){string Ȱ=Ġ.CustomName;Ġ.CustomName=Ω==Ͳ?Ġ.CustomName.Replace(Ω,"").Trim():Ġ.
CustomName.Replace(Ͳ,"").Trim();if(!Ȱ.Equals(Ġ.CustomName)&&!ʻ)Ͱ.ǒ($" > Removing Tag: {Ġ.CustomName} \n");}void ç(){Ͱ.AppendLine(
"Init() Start");Echo("Init() Start");Ѐ();Echo("Config() End");И();Echo("ManageTag() End");ŀ();Echo("InitControllers() End");έ=true;if(
ʹ!=null){ˬ=ʹ.ɫ.CalculateShipMass();τ=ˬ.BaseMass;}Echo("Checking Mass End");Л();Echo("OneRunMainChecker() End");Ͱ.
AppendLine("Init "+(ʻ?"Failed":"Completed Sucessfully"));}void ŀ(){bool Ŀ=this.Ŀ||this.Ψ;List<IMyShipController>Í=new List<
IMyShipController>();GridTerminalSystem.GetBlocksOfType<IMyShipController>(Í);List<ʇ>ľ=new List<ʇ>();foreach(IMyShipController Ľ in Í){ʶ.
Add(Ľ);ľ.Add(new ʇ(Ľ,this));}ʷ=ľ;StringBuilder ļ=new StringBuilder();foreach(ʇ ġ in ʷ){bool Ļ=true;StringBuilder ĺ=new
StringBuilder(ġ.ɫ.CustomName+"\n");if(!ġ.ɫ.CanControlShip){ĺ.AppendLine("  CanControlShip not set\n");Ļ=false;}if(!ġ.ɫ.
ControlThrusters){ĺ.AppendLine("  Can't ControlThrusters\n");Ļ=false;}if(!(Ŀ||Ж(ġ.ɫ))){ĺ.AppendLine("  Doesn't match my tag\n");Ļ=false;
}if(Ļ){Ń(ġ.ɫ);ġ.ʆ=ġ.ɫ.DampenersOverride;ʴ.Add(ġ);ʵ.Add(ġ.ɫ);if(this.Ψ){Е(ġ.ɫ);}}else{ļ.Append(ĺ);}}if(Í.Count==0){ļ.
AppendLine("No Controller Found.\nEither for missing tag, not working or removed.");}if(ʴ.Count==0){Ͱ.ǒ(
"ERROR: no usable ship controller found. Reason: \n");Ͱ.ǒ(ļ.ToString());И(true);ʻ=true;return;}else if(ʴ.Count>0){foreach(ʇ ġ in ʴ){if(ġ.ɫ.IsUnderControl){ʹ=ġ;break;}}if(ʹ
==null){ʹ=ʴ[0];}}return;}bool Ĺ(){return Ҕ&&ʹ!=null&&ʹ.ɫ.IsUnderControl;}double ĸ=0;bool ķ(){bool Ķ=ͺ==0;bool ĵ=!ʿ&&ʾ;bool
Ł=ʿ&&ʾ;bool Ĵ=(Ķ||Ł)&&ĸ>ŋ&&ʼ&&!Φ&&ͷ==0&&!ˀ;if(!Φ&&ʔ.Ƽ(1)==100&&ĸ<=ŋ)ĸ+=Runtime.TimeSinceLastRun.TotalSeconds;else if(Φ)ĸ=
0;if(Ĵ||ρ){if(Τ.Ѱ)ͱ.AppendLine("\nEverything stopped, performance mode.\n");ʦ=ʦ||ʩ.All(Q=>Q.TargetVelocityRPM==0)&&ʪ.All(
Q=>!Q.Enabled&&Q.ThrustOverridePercentage==0);if(!ʦ)Ќ();return true;}else if((ʦ&&ʼ)||ĵ){ʼ=ʦ=false;foreach(ɗ Ŏ in ʳ)Ŏ.Ɋ(ɍ:
true);}return ʦ;}bool ō(){if(Җ>0&&Τ.Ѱ){ͱ.AppendLine($"--SkipFrame[{Җ}]--");ͱ.AppendLine($" >Skipped: {ſ}");ͱ.AppendLine(
$" >Remaining: {Җ-ſ}");}if(!Ϊ&&Җ>0&&Җ>ſ){ſ++;return true;}else if(Җ>0&&ſ>=Җ)ſ=0;return false;}bool Ō=false;const double ŋ=0.25;bool Ŋ(){ι=!ʸ.
ƺ()||!ʰ.ƺ();if(!ι&&!Ō)return false;bool ŉ=false;if(ς){ζ=ʸ.Any(Q=>Q.Status==MyShipConnectorStatus.Connected);ʿ=(ʰ.Any(Q=>Q
.IsLocked)||ζ)&&(Ѳ||(κ&&Ҋ));}else{bool ň=(ʰ.Any(Q=>Q.IsLocked)||ʸ.Any(Q=>Q.Status==MyShipConnectorStatus.Connected))&&(Ѳ
||(κ&&Ҋ&&Ϋ==0))&&!ˀ;ŉ=ň!=ʿ;ʿ=ň;}ς=!ʿ&&!ʾ;if(ς)return false;bool Ň=ʿ&&ʾ&&ʼ;bool ņ=ʔ.Ƽ(1)==100&&ĸ>ŋ;ρ=Ň&&ņ;bool Ņ=Ň&&!ņ;bool
ń=ʿ&&!ʾ;bool ĵ=!ʿ&&ʾ;if(ń||(ĵ&&ˇ.с)||ŉ){ŧ();}if(ρ||(ĵ&&!ˇ.с)){ˇ.н();}if(ς)Ō=false;return ρ;}bool Ń(IMyTerminalBlock Ġ){if
(!(Ġ is IMyTextSurfaceProvider))return false;IMyTextSurfaceProvider ĭ=(IMyTextSurfaceProvider)Ġ;bool Ĳ=true;if(Ġ.
CustomData.Length==0){return false;}bool[]Ī=new bool[ĭ.SurfaceCount];for(int Ä=0;Ä<Ī.Length;Ä++){Ī[Ä]=false;}int ĩ=0;while(ĩ>=0){
string Ĩ=Ġ.CustomData;int ħ=Ĩ.IndexOf(ʺ,ĩ);if(ħ<0){Ĳ=ĩ!=0;break;}int Ħ=Ĩ.IndexOf("\n",ħ);ĩ=Ħ;string ĥ;if(Ħ<0){ĥ=Ĩ.Substring(ħ+
ʺ.Length);}else{ĥ=Ĩ.Substring(ħ+ʺ.Length,Ħ-(ħ+ʺ.Length));}int Ĥ;if(Int32.TryParse(ĥ,out Ĥ)){if(Ĥ>=0&&Ĥ<ĭ.SurfaceCount){Ī[
Ĥ]=true;}else{string ģ;if(Ħ<0){ģ=Ĩ.Substring(ħ);}else{ģ=Ĩ.Substring(ħ,Ħ-(ħ));}ʤ.Append($"\nDisplay number out of range: {Ĥ}\nshould be: 0 <= num < {ĭ.SurfaceCount}\non line: ({ģ})\nin block: {Ġ.CustomName}\n"
);}}else{string ģ;if(Ħ<0){ģ=Ĩ.Substring(ħ);}else{ģ=Ĩ.Substring(ħ,Ħ-(ħ));}ʤ.Append(
$"\nDisplay number invalid: {ĥ}\non line: ({ģ})\nin block: {Ġ.CustomName}\n");}}for(int Ä=0;Ä<Ī.Length;Ä++){if(Ī[Ä]&&!this.ʫ.Any(Q=>Q.R.Equals(ĭ.GetSurface(Ä)))){this.ʫ.Add(new V(ĭ.GetSurface(Ä),
this));}else if(!Ī[Ä]){List<V>Ģ=ʫ.FindAll(Q=>Q.R.Equals(ĭ.GetSurface(Ä)));foreach(V ġ in Ģ){ĳ(ġ);}}}return Ĳ;}bool ī(
IMyTerminalBlock Ġ){if(!(Ġ is IMyTextSurfaceProvider))return false;IMyTextSurfaceProvider ĭ=(IMyTextSurfaceProvider)Ġ;for(int Ä=0;Ä<ĭ.
SurfaceCount;Ä++){if(ʫ.Any(Q=>Q.R.Equals(ĭ.GetSurface(Ä)))){List<V>Ģ=ʫ.FindAll(Q=>Q.R.Equals(ĭ.GetSurface(Ä)));foreach(V ġ in Ģ){ĳ(ġ
);}}}return true;}void ĳ(V R){if(this.ʫ.Any(Q=>Q.R.Equals(R))){this.ʫ.Remove(R);R.R.ContentType=ContentType.NONE;R.R.
WriteText("",false);}}void ĳ(IMyTextPanel R){if(this.ʫ.Any(Q=>Q.R.Equals(R))){List<V>Ģ=ʫ.FindAll(Q=>Q.R.Equals(R));foreach(V ġ in
Ģ){ĳ(ġ);}R.ContentType=ContentType.NONE;R.WriteText("",false);}}bool ı=false;Vector3D İ(string į,bool Ĭ=false){Vector3D ŏ
=Vector3D.Zero;if(ʞ){if(Ϊ)try{δ=Me.GetValue<Dictionary<string,object>>("ControlModule.Inputs");Me.SetValue<string>(
"ControlModule.AddInput",ʝ);Me.SetValue<string>("ControlModule.AddInput",ʜ);Me.SetValue<string>("ControlModule.AddInput",ʛ);Me.SetValue<string>(
"ControlModule.AddInput",ʚ);Me.SetValue<bool>("ControlModule.RunOnInput",true);Me.SetValue<int>("ControlModule.InputState",1);Me.SetValue<float>
("ControlModule.RepeatDelay",0.016f);}catch{ʞ=false;}if(δ!=null){if(!ʙ&&δ.ContainsKey(ʝ)){ˮ=!ˮ;ʙ=true;}else if(ʙ&&!δ.
ContainsKey(ʝ)){ʙ=false;}if(!ʘ&&δ.ContainsKey(ʜ)){ː=!ː;ʘ=true;}else if(ʘ&&!δ.ContainsKey(ʜ)){ʘ=false;}if(!ʗ&&δ.ContainsKey(ʛ)){if(ѻ
==Ѽ.Count-1)ѻ=0;else ѻ++;ʗ=true;}else if(ʗ&&!δ.ContainsKey(ʛ)){ʗ=false;}if(!ʖ&&δ.ContainsKey(ʚ)){Ѳ=!Ѳ;ʖ=true;}else if(ʖ&&!
δ.ContainsKey(ʚ)){ʖ=false;}}}bool ř=false;if(į.Length>0){if(į.Contains(ˍ)){ˮ=!ˮ;ř=true;}else if(į.Contains(ˌ)&&!ʿ){ː=!ː;ı
=ː;}else if(į.Contains(ˋ)){if(ѻ==Ѽ.Count-1)ѻ=0;else ѻ++;}else if(į.Contains("park")){Ѳ=!Ѳ;Ō=true;}}if(ʲ.Count!=0){if(Ĺ())
{if(ř){ʹ.ɫ.DampenersOverride=ˮ;}else{ˮ=ʹ.ɫ.DampenersOverride;}}else{if(ř){foreach(ʇ Ŭ in ʴ){Ŭ.ɯ(ˮ);}}else{ˮ=ǐ(ʹ.ɫ)?ʹ.ɫ.
DampenersOverride:ˮ;foreach(ʇ Ŭ in ʴ){Ŭ.ɫ.DampenersOverride=ˮ;Ŭ.ɯ(ˮ);}}}}if(Ĭ)return ŏ;if(Ĺ()){ŏ=ʹ.ɫ.ƴ();}else{foreach(ʇ Ŭ in ʴ){if(Ŭ.ɫ.
IsUnderControl){ŏ+=Ŭ.ɫ.ƴ();}}}return ŏ;}void ŭ(){if(Ϊ)return;ʇ Ŭ=Ћ();if(Ŭ==null){Ͱ.ǒ(
"  -No cockpit registered, checking mainController\n");if(!GridTerminalSystem.CanAccess(ʹ.ɫ)){ʹ=null;foreach(ʇ Ť in ʴ){if(GridTerminalSystem.CanAccess(Ť.ɫ)){ʹ=Ť;break;}}}if(
ʹ==null){ʻ=true;Ͱ.ǒ("ERROR, ANY CONTROLLERS FOUND - SHUTTING DOWN");И(true);return;}}else if(!Ψ){ˬ=Ŭ.ɫ.CalculateShipMass(
);float ū=ˬ.BaseMass;if(ū<0.001f){Ͱ.ǒ("  -Can't fly a Station\n");ͼ=true;Ũ(2);return;}else if(ͼ){ͼ=ʦ=false;foreach(ɗ Ŏ in
ʳ)Ŏ.Ɋ(ɍ:true);Ũ(0);}if(this.τ==ū)return;this.τ=ū;}Л(false);}bool Ū(bool ũ,bool C){if(ũ&&ʿ&&!ˇ.с){ˇ.с=true;Ũ(ѵ&&ͺ==0&&!C?2
:1);}else if(!ʿ){ζ=ʾ=false;}if(έ&&!this.C&&ρ){Ũ();this.C=true;}else if(this.C&&!έ&&ρ){Ũ(ѵ&&ͺ==0?2:1);this.C=false;}return
true;}void Ũ(int Ŏ=0){switch(Ŏ){case 0:Runtime.UpdateFrequency=UpdateFrequency.Update1;break;case 1:Runtime.UpdateFrequency=
UpdateFrequency.Update10;break;case 2:Runtime.UpdateFrequency=UpdateFrequency.Update100;break;case 3:Runtime.UpdateFrequency=
UpdateFrequency.Once;break;case 4:Runtime.UpdateFrequency=UpdateFrequency.None;break;};}void ŧ(){ˆ.о();ˇ.о();ˇ.с=false;if(ʿ&&!ʾ)ʾ=true;
else if(!ʿ&&ʾ)Ũ();Φ=!ʿ;if(!ʿ)κ=false;}void Ŧ(){if(έ)Ͱ.ǒ("Checking Blocks Again");ˣ.о();ˢ.о();ˡ.о();ˠ.о();ˤ.о();}IEnumerable<
double>ť(){while(true){if(έ)Ͱ.ǒ($"  Getting Screens => new:{γ.Count}\n");if(γ.Any()){this.ʱ.AddRange(γ);Ў(ref ʱ);γ.Clear();if(
ή)yield return ώ;}if(Me.SurfaceCount>0){Ń(Me);}foreach(IMyTextPanel Ů in this.ʱ){bool Æ=ʫ.Any(Q=>Q.R.Equals(Ů));bool Å=Ů.
IsWorking;bool w=Ů.CustomName.ToLower().Contains(ʯ.ToLower());bool X=Ů.Closed;if(!Æ&&Å&&w)ʫ.Add(new V(Ů,this));else if(Æ&&(!Å||!w
||X)){List<V>Ģ=ʫ.FindAll(Q=>Q.R.Equals(Ů));foreach(V ġ in Ģ){ʫ.Remove(ġ);}}if(ή)yield return ώ;}if(έ){if(ή)yield return ώ;
Ͱ.ǒ($"  ->Done. Total Screens {ʱ.Count} => Total Surfaces:{ʫ.Count}\n");Ў(ref ʫ);}π.ß=ʫ;ˣ.с=true;yield return ώ;}}
IEnumerable<double>Ų(){while(true){bool Ŀ=this.Ŀ||this.Ψ;if(this.ε.Count>0){this.ʷ.AddRange(ε);Ў(ref ʷ);ε.Clear();if(ή)yield return
ώ;}StringBuilder ļ=new StringBuilder();foreach(ʇ ġ in this.ʷ){bool Ļ=true;StringBuilder ĺ=new StringBuilder(ġ.ɫ.
CustomName+"\n");if(!ġ.ɫ.CanControlShip){ĺ.AppendLine("  CanControlShip not set\n");Ļ=false;}if(!ġ.ɫ.ControlThrusters){ĺ.
AppendLine("  Can't ControlThrusters\n");Ļ=false;}if(!Ŀ&&!Ж(ġ.ɫ)){ĺ.AppendLine("  Doesn't match my tag\n");Ļ=false;}if(Ļ){Ń(ġ.ɫ);
List<IMyThrust>Ű=new List<IMyThrust>();if(ή)yield return ώ;GridTerminalSystem.GetBlocksOfType(Ű,Q=>ġ.ɫ.ǐ(Q)&&!ġ.ʋ.Contains(Q
));if(ή)yield return ώ;ġ.ʋ=ġ.ʋ.Concat(Ű).ToList();if(ή)yield return ώ;Ű.RemoveAll(Q=>Q.Orientation.Forward!=ġ.ɫ.
Orientation.Forward);if(ή)yield return ώ;ġ.ɺ=ġ.ɺ.Concat(Ű).ToList();if(ή)yield return ώ;ġ.ʆ=ġ.ʋ.Count>0?ġ.ɫ.DampenersOverride:ˮ;if(
!ʴ.Contains(ġ)){ʴ.Add(ġ);ʵ.Add(ġ.ɫ);}if(this.Ψ){Е(ġ.ɫ);}if(ή)yield return ώ;}else{ī(ġ.ɫ);ʴ.Remove(ġ);ʵ.Remove(ġ.ɫ);ļ.
Append(ĺ);}}if(ή)yield return ώ;if(ʷ.Count==0)ļ.AppendLine(
"Any Controller Found.\nEither for missing tag, not working or removed.");if(ʴ.Count==0){Ͱ.ǒ("ERROR: no usable ship controller found. Reason: \n");Ͱ.ǒ(ļ.ToString());И(true);yield return ώ;}
else if(ʴ.Count>0){foreach(ʇ ġ in ʴ){if(ġ.ɫ.IsUnderControl){ʹ=ġ;break;}if(ή)yield return ώ;}if(ʹ==null){ʹ=ʴ[0];}}ˢ.с=true;
yield return ώ;}}IEnumerable<double>ů(){while(true){bool Ŀ=this.Ψ||this.Ŀ;Ͱ.ǒ("  >Getting Rotors\n");foreach(IMyTerminalBlock
ű in ʩ){if(this.Ψ){Е(ű);}if(ή)yield return ώ;}foreach(IMyTerminalBlock ª in ʪ){if(this.Ψ){Е(ª);}if(ή)yield return ώ;}Δ.
AddRange(β);if(ή)yield return ώ;ͽ.AddRange(α);if(ή)yield return ώ;Ў(ref Δ);if(ή)yield return ώ;Ў(ref ͽ);if(ή)yield return ώ;
foreach(IMyMotorStator š in Δ){if(this.Ψ){Е(š);}if(š.Top!=null&&(Ŀ||Ж(š))&&š.TopGrid!=Me.CubeGrid){ʁ Ř=new ʁ(š,this);this.ʳ.Add
(new ɗ(Ř,this));ʩ.Add(š);}else{Г(š);}if(ή)yield return ώ;}Ͱ.ǒ("  >Getting Thrusters\n");for(int Ä=this.ʳ.Count-1;Ä>=0;Ä--
){IMyMotorStator ŗ=this.ʳ[Ä].Ř.ɫ;for(int Ŗ=ͽ.Count-1;Ŗ>=0;Ŗ--){bool ŕ=false;if(Ŀ||Ж(ͽ[Ŗ])){bool Ŕ=ͽ[Ŗ].CubeGrid==this.ʳ[Ä
].Ř.ɫ.TopGrid;bool Å=ʳ[Ä].ɖ.Any(Q=>Q.ɫ==ͽ[Ŗ]);if(Ŕ&&this.Ψ){Е(ͽ[Ŗ]);}if(Ŕ&&!Å){if(Ϊ){ͽ[Ŗ].ThrustOverridePercentage=0;ͽ[Ŗ]
.Enabled=true;}ŕ=true;α.Remove(ͽ[Ŗ]);this.ʳ[Ä].ɖ.Add(new ɇ(ͽ[Ŗ],this));ʪ.Add(ͽ[Ŗ]);ͽ.RemoveAt(Ŗ);}}if(!ŕ&&!α.Contains(ͽ[Ŗ
]))α.Add(ͽ[Ŗ]);if(ή)yield return ώ;}if(this.ʳ[Ä].ɖ.Count==0){if(!β.Contains(ŗ))β.Add(ŗ);ʩ.Remove(ŗ);Г(ŗ);this.ʳ.RemoveAt(
Ä);}else{if(Ϊ){ŗ.ǎ();ŗ.RotorLock=false;ŗ.Enabled=true;}β.Remove(ŗ);this.ʳ[Ä].Ⱥ();this.ʳ[Ä].ȴ();this.ʳ[Ä].ɡ();}if(ή)yield
return ώ;}Ͱ.ǒ("  >Grouping VTThrs\n");if(ʬ.Count==0){Ͱ.ǒ("  > [ERROR] => Any Vector Thrusters Found!\n");ʻ=true;И(true);if(ή)
yield return ώ;}for(int Ä=0;Ä<ʬ.Count;Ä++){ʬ[Ä]=ʬ[Ä].OrderByDescending(œ=>œ.ɖ.Sum(Q=>Q.ɫ.MaxEffectiveThrust)).ToList();if(ή)
yield return ώ;}Γ=new List<double>(ʬ.Count);Γ.AddRange(Enumerable.Repeat(0.0,ʬ.Count));if(ή)yield return ώ;ͽ.Clear();Δ.Clear(
);ˡ.с=true;yield return ώ;}}IEnumerable<double>Œ(){while(true){foreach(IMyShipConnector ő in Š){if(((Ѵ&&ǐ(ő))||Ж(ő))&&!ʸ.
Contains(ő)){yield return ώ;ʸ.Add(ő);yield return ώ;Ͱ.ǒ($"New CON: {ő.CustomName}\n");yield return ώ;}yield return ώ;}foreach(
IMyLandingGear Ő in ş){if(((ѳ&&ǐ(Ő))||Ж(Ő))&&!ʰ.Contains(Ő)){yield return ώ;ʰ.Add(Ő);yield return ώ;Ͱ.ǒ(
$"New LanGear: {Ő.CustomName}\n");yield return ώ;}yield return ώ;}for(int Ä=ʥ.Count-1;Ä>=0;Ä--){IMyBatteryBlock I=ʥ[Ä];yield return ώ;if(Ж(I)){yield
return ώ;Ͱ.ǒ($"Filtered Bat: {I.CustomName}\n");yield return ώ;ʥ.RemoveAt(Ä);yield return ώ;if(!ʨ.Contains(I))ʨ.Add(I);yield
return ώ;}yield return ώ;}for(int Ä=ʨ.Count-1;Ä>=0;Ä--){IMyBatteryBlock I=ʨ[Ä];yield return ώ;if(!Ж(I)){yield return ώ;Ͱ.ǒ(
$"Filtered TagBat: {I.CustomName}\n");yield return ώ;ʨ.RemoveAt(Ä);yield return ώ;if(ǐ(I)&&!ʥ.Contains(I))ʥ.Add(I);else if(!Ş.Contains(I))Ş.Add(I);yield
return ώ;}yield return ώ;}for(int Ä=Ş.Count-1;Ä>=0;Ä--){IMyBatteryBlock I=Ş[Ä];yield return ώ;if(Ж(I)){yield return ώ;Ͱ.ǒ(
$"Added TagBat: {I.CustomName}\n");yield return ώ;Ş.RemoveAt(Ä);yield return ώ;if(!ʨ.Contains(I))ʨ.Add(I);yield return ώ;}yield return ώ;}for(int Ä=ʸ.
Count-1;Ä>=0;Ä--){IMyShipConnector Ť=ʸ[Ä];yield return ώ;bool È=Ж(Ť);yield return ώ;if((!Ѵ&&!È)||(Ѵ&&!È&&!ǐ(Ť))){yield return
ώ;Ͱ.ǒ($"Filtered Con: {Ť.CustomName}\n");yield return ώ;ʸ.RemoveAt(Ä);yield return ώ;Ō=true;yield return ώ;}yield return
ώ;}for(int Ä=ʰ.Count-1;Ä>=0;Ä--){IMyLandingGear ţ=ʰ[Ä];yield return ώ;if((ѳ&&!Ж(ţ)&&!ǐ(ţ))||(!ѳ&&!Ж(ţ))){yield return ώ;Ͱ
.ǒ($"Filtered LanGear: {ţ.CustomName}\n");yield return ώ;ʰ.RemoveAt(Ä);yield return ώ;Ō=true;yield return ώ;}yield return
ώ;}for(int Ä=0;Ä<ʬ.Count;Ä++){ʬ[Ä]=ʬ[Ä].OrderByDescending(œ=>œ.ɒ).ToList();yield return ώ;}ˠ.с=true;yield return ώ;}}List
<IMySoundBlock>Ţ=new List<IMySoundBlock>();List<IMyShipConnector>Š=new List<IMyShipConnector>();List<IMyLandingGear>ş=new
List<IMyLandingGear>();List<IMyBatteryBlock>Ş=new List<IMyBatteryBlock>();int ŝ=0;List<IMyTerminalBlock>Ŝ=new List<
IMyTerminalBlock>();void ś<Ś>(IMyTerminalBlock Ġ,ref List<Ś>Į){if(!Ŝ.Contains(Ġ))Ŝ.Add(Ġ);if(!Į.Contains((Ś)Ġ))Į.Add((Ś)Ġ);ŝ++;}
IEnumerable<double>ğ(){while(true){ή=((!Ϊ||(Ϊ&&ʻ))&&!Ψ);if(ή)yield return ώ;if(!έ){if(ˢ.й(ή))yield return ώ;if(ˣ.й(ή))yield return
ώ;if(ˠ.й(ή))yield return ώ;Ͱ.ǒ(" -Everything seems normal.");continue;}if(!Ϊ)Ͱ.ǒ("  -Mass is different\n");List<
IMyTerminalBlock>S=new List<IMyTerminalBlock>();GridTerminalSystem.GetBlocks(S);if(!Ψ&&S.Count.Equals(ŝ)){έ=false;yield return ώ;
continue;}if(!Ϊ)Ͱ.ǒ("  -New blocks detected\n");List<IMyTerminalBlock>Â=new List<IMyTerminalBlock>(ʪ).Concat(ʲ).Concat(ʩ).Concat
(ʶ).ToList();bool Á=Â.Any(Q=>!GridTerminalSystem.CanAccess(Q));if(Á){foreach(IMyTerminalBlock I in Â){if(!
GridTerminalSystem.CanAccess(I)){Ŝ.Remove(I);ŝ--;if(I is IMyShipController){IMyShipController À=(IMyShipController)I;ʵ.Remove(À);ʶ.Remove(
À);ī(À);ʴ.Remove(ʴ.Find(Q=>Q.ɫ.Equals(À)));ʷ.Remove(ʷ.Find(Q=>Q.ɫ.Equals(À)));if(ʴ.ƺ()){Ͱ.ǒ(
$"ERROR -> Any Usable Controller Found, Shutting Down");И(true);ʻ=true;yield return ώ;}ʹ=ʴ[0];}else if(I is IMyThrust){IMyThrust ª=(IMyThrust)I;α.Remove(ª);ʭ.Remove(ª);bool º
=!ʲ.ƺ();ʲ.Remove(ª);if(ʲ.ƺ()&&º){ˮ=true;}ʪ.Remove(ª);}else{ʩ.Remove((IMyMotorStator)I);β.Remove((IMyMotorStator)I);}}}}
List<IMyTerminalBlock>µ=new List<IMyTerminalBlock>(S).FindAll(Q=>Q is IMyThrust||Q is IMyMotorStator||Q is IMyShipController
).Except(ʲ).Except(ʩ).Except(ʪ).Except(ʶ).ToList();if(Ψ||!µ.ƺ()){foreach(IMyTerminalBlock I in µ){if(I is
IMyShipController){ś(I,ref ʶ);ε.Add(new ʇ((IMyShipController)I,this));}else if(I is IMyThrust){if(!ǐ(I))ś(I,ref ͽ);else{IMyThrust ª=(
IMyThrust)I;ś(I,ref ʲ);if(I.Orientation.Forward==ʹ.ɫ.Orientation.Forward){ʭ.Add(ª);}if(!Ϊ&&Ҝ)(I as IMyFunctionalBlock).Enabled=
true;}}else{ś(I,ref Δ);}}ˢ.й(false);ˡ.й(false);}if(ή)yield return ώ;foreach(ɗ y in ʳ){y.ɖ.RemoveAll(Q=>!ʪ.Contains(Q.ɫ));y.ɔ
.RemoveAll(Q=>!y.ɖ.Contains(Q));y.ɕ.RemoveAll(Q=>!y.ɖ.Contains(Q));if(ή)yield return ώ;}for(int Ä=ʳ.Count-1;Ä>=0;Ä--){ɗ y
=ʳ[Ä];IMyMotorStator Ð=y.Ř.ɫ;if(!ʩ.Contains(Ð)||Ð.Top==null||y.ɖ.ƺ()){Ð.ǎ();ʳ.RemoveAt(Ä);if(!β.Contains(Ð))β.Add(Ð);}if(
ή)yield return ώ;}foreach(List<ɗ>Ï in ʬ){Ï.RemoveAll(Q=>!ʳ.Contains(Q)||Q.ɖ.Count<1);if(ή)yield return ώ;}ʬ.RemoveAll(Q=>
Q.Count<1);if(ή)yield return ώ;if(ή)yield return ώ;for(int Ä=Ŝ.Count-1;Ä>=0;Ä--){IMyTerminalBlock I=Ŝ[Ä];bool Î=ˎ&&(I is
IMyBatteryBlock||I is IMyGasTank||I is IMyLandingGear||I is IMyShipConnector);bool Ñ=I is IMyShipController||ʪ.Contains(I)||I is
IMyMotorStator;if(!GridTerminalSystem.CanAccess(I)){Ŝ.RemoveAt(Ä);if(I is IMyLandingGear){ş.Remove((IMyLandingGear)I);ʰ.Remove((
IMyLandingGear)I);}else if(I is IMyShipConnector){Š.Remove((IMyShipConnector)I);ʸ.Remove((IMyShipConnector)I);}else if(I is IMyGasTank
){ʮ.Remove((IMyGasTank)I);}else if(I is IMyBatteryBlock){Ş.Remove((IMyBatteryBlock)I);ʨ.Remove((IMyBatteryBlock)I);ʥ.
Remove((IMyBatteryBlock)I);}else if(I is IMyTextPanel){ʱ.Remove((IMyTextPanel)I);ĳ((IMyTextPanel)I);}else if(I is
IMySoundBlock){Ţ.Remove((IMySoundBlock)I);}}else if(Ψ&&(Î||Ñ)){Е(I);if(Ѷ&&I.BlockDefinition.SubtypeId.Contains("SmallBattery")&&!I.
CustomName.Contains(ѷ)){I.CustomName+=" "+ѷ;}}if(ή)yield return ώ;};List<IMyTerminalBlock>Í=new List<IMyTerminalBlock>(S).Except(Ŝ
).ToList();if(ή)yield return ώ;foreach(IMyTerminalBlock I in Í){if(GridTerminalSystem.CanAccess(I)){bool Ë=I is
IMyLandingGear;bool Ê=I is IMyShipConnector;bool É=ǐ(I);bool È=Ж(I);bool Ç=É||È;if(I is IMyTextPanel){ś(I,ref γ);}else if(Ê){if(ˎ)Е(I)
;bool Æ=Ѵ&&Ç;bool Å=!Ѵ&&È;bool Ì=Æ||Å;ś(I,ref Š);if(Ì&&!ʸ.Contains(I))ʸ.Add((IMyShipConnector)I);}else if(Ë){if(ˎ)Е(I);
bool w=ѳ&&Ç;bool X=!ѳ&&È;bool P=w||X;ś(I,ref ş);if(P&&!ʰ.Contains(I))ʰ.Add((IMyLandingGear)I);}else if(I is IMyGasTank&&(È||
ˎ||É)){if(ˎ)Е(I);ś(I,ref ʮ);if(È&&Ҝ)(I as IMyGasTank).Stockpile=false;}else if(I is IMyBatteryBlock){IMyBatteryBlock O=(
IMyBatteryBlock)I;if(ˎ){Е(I);if(Ѷ&&I.BlockDefinition.SubtypeId.Contains("SmallBattery")&&!I.CustomName.Contains(ѷ)){I.CustomName+=" "+ѷ
;}}if(Ϊ&&(È||É)&&Ҝ)O.ChargeMode=ChargeMode.Auto;if(È)ś(I,ref ʨ);else if(É)ś(I,ref ʥ);else ś(I,ref Ş);}else if(I is
IMySoundBlock&&È){IMySoundBlock N=(IMySoundBlock)I;N.LoopPeriod=1;N.SelectedSound="Alert 2";ś(I,ref Ţ);}}if(ή)yield return ώ;}if(!Ϊ&&
ˠ.й(ή))yield return ώ;if(ˣ.й(ή))yield return ώ;Ў(ref ʶ);if(ή)yield return ώ;Ў(ref ʳ);if(ή)yield return ώ;Ў(ref ʲ);if(ή)
yield return ώ;Ў(ref ʪ);if(ή)yield return ώ;Ў(ref ʩ);if(ή)yield return ώ;Ў(ref ʵ);if(ή)yield return ώ;Ў(ref ʴ);if(ή)yield
return ώ;έ=false;ŝ=S.Count;yield return ώ;}}IEnumerable<double>M(){while(true){ί.Clear();if(ΰ.Count>0){double L=0;double K=0;
double J=0;foreach(IMyPowerProducer I in ΰ){K+=I.CurrentOutput;if(I is IMyBatteryBlock){L+=(I as IMyBatteryBlock).CurrentInput
;J+=(I as IMyBatteryBlock).CurrentStoredPower/(I as IMyBatteryBlock).MaxStoredPower;}K-=I.MaxOutput;yield return ώ;}L/=L
!=0?ΰ.Count:1;K/=K!=0?ΰ.Count:1;J*=J!=0?(100/ΰ.Count):1;ί=new List<double>{L,K,J.Ƽ(0)};yield return ώ;}ˆ.с=true;yield
return ώ;}}IEnumerable<double>H(){List<IMyBatteryBlock>G=new List<IMyBatteryBlock>();List<IMyBatteryBlock>F=new List<
IMyBatteryBlock>();bool E=false;bool D=false;bool C=false;while(true){bool B=ѹ&&!ʲ.ƺ();if(B&&(!E||!ʿ)){ʲ.ForEach(Q=>Q.Enabled=!ʿ);E=
true;}if((ʥ.Count+ʨ.Count<2)||!Ѹ||!ζ){C=Ū(true,C);yield return ώ;continue;}if(G.ƺ()&&F.ƺ()){List<IMyBatteryBlock>A=new List<
IMyBatteryBlock>(ʨ).Concat(ʥ).ToList();if(ʿ)yield return ώ;F=A.FindAll(Q=>Q.CustomName.Contains(ѷ)||(Ŀ&&Ѷ&&Q.BlockDefinition.SubtypeId.
Contains("SmallBattery")));if(ʿ)yield return ώ;if(!F.ƺ()&&A.SequenceEqual(F)){G=new List<IMyBatteryBlock>(F);F=new List<
IMyBatteryBlock>{G[0]};G.RemoveAt(0);}else if(!F.ƺ()){G=A.Except(F).ToList();}else if(F.ƺ()&&ʨ.Count>ʥ.Count){F=new List<
IMyBatteryBlock>(ʥ);if(ʥ.ƺ()){F=new List<IMyBatteryBlock>{ʨ[0]};}G=new List<IMyBatteryBlock>(ʨ).Except(F).ToList();}else if(F.ƺ()){F.
Add(ʥ.ƺ()?ʨ[0]:ʥ[0]);G=G.Concat(ʥ).Concat(ʨ).Except(F).ToList();}if(!ʿ){if(!G.ƺ())G[0].ChargeMode=ChargeMode.Auto;if(!ʮ.ƺ()
)ʮ[0].Stockpile=false;}yield return ώ;}List<IMyPowerProducer>u=new List<IMyPowerProducer>();GridTerminalSystem.
GetBlocksOfType(u,Q=>!G.Contains(Q)&&!F.Contains(Q));yield return ώ;List<double>m=new List<double>();List<double>k=new List<double>();
yield return ώ;if(!D||(ˇ.с&&ʿ)){if(ʿ){ΰ=new List<IMyTerminalBlock>(u);while(!ˆ.с){ˆ.н();yield return ώ;}ˆ.с=false;D=true;k=
new List<double>(ί);}yield return ώ;ΰ=new List<IMyTerminalBlock>(F);while(!ˆ.с){ˆ.н();yield return ώ;}ˆ.с=false;m=new List<
double>(ί);yield return ώ;}bool h=!m.ƺ()&&m[2]<2.5;bool f=!m.ƺ()&&m[2]>25;yield return ώ;bool Z=G.All(Q=>Q.ChargeMode==
ChargeMode.Recharge);Υ=(k.ƺ()&&D)||(!k.ƺ()&&k[1]==0)||h;bool q=ʿ&&ζ&&D&&((!Z&&!Υ)||(Z&&Υ))&&!G.ƺ();bool Y=Υ&&!k.ƺ()&&k[1]!=0&&f;
yield return ώ;if(!((!ʿ&&ζ)||q||Y)){C=Ū(D,C);yield return ώ;continue;}foreach(IMyGasTank W in ʮ){W.Stockpile=!Υ&&ʿ&&W.
FilledRatio!=1;yield return ώ;}if(ʿ&&!Υ){foreach(IMyBatteryBlock I in F){I.ChargeMode=ʿ&&!Υ?ChargeMode.Auto:ChargeMode.Recharge;
yield return ώ;}foreach(IMyBatteryBlock I in G){I.ChargeMode=ʿ&&!Υ?ChargeMode.Recharge:ChargeMode.Auto;yield return ώ;}}else
if(!ʿ||Υ){foreach(IMyBatteryBlock I in G){I.ChargeMode=ʿ&&!Υ?ChargeMode.Recharge:ChargeMode.Auto;yield return ώ;}foreach(
IMyBatteryBlock I in F){I.ChargeMode=ʿ&&!Υ?ChargeMode.Auto:ChargeMode.Recharge;yield return ώ;}}C=Ū(D,C);yield return ώ;}}class V{
Program U;public IMyTextSurface R{get;}public RectangleF Ò{get;}public Vector2 å{get;}public Vector2 č{get;}public Vector2 ċ{
get;}public float Ċ{get;}public Vector2 ĉ{get;}public Vector2 Ĉ{get;}public float ć{get;}public float Ć{get;}public float ą
{get;}public float Ą{get;}public float ă{get;}public float Ă{get;}public float ā{get;}public float Ā{get;}public float ÿ{
get;}public float þ{get;}public List<Vector2>ý{get;}public Vector2 Č{get;}public Vector2 ü{get;}public Vector2 Ď{get;}
public Vector2 Ğ{get;}public V(IMyTextSurface R,Program Ĝ){this.R=R;this.U=Ĝ;Ò=new RectangleF((R.TextureSize-R.SurfaceSize)/2f
,R.SurfaceSize);å=R.TextureSize;č=å*0.5f;ċ=R.SurfaceSize-12f;Ċ=Math.Min(ċ.X,ċ.Y);ĉ=new Vector2(Ċ,Ċ);Ĉ=(å+ċ)*0.5f/512f;ć=
Math.Min(Ĉ.X,Ĉ.Y);Ć=1.45f;ą=Ò.Y+Ò.Height-Ć*ć*20;Ą=ą-Ć*ć*25;ă=Math.Min(č.X,č.Y);Ă=200/2*ć*Ć+140/2*ć*1.5f;ā=74*ć*Ć;ý=new List<
Vector2>{new Vector2(Ò.X+(Ò.Width*0.5f)-ā,ą)};ý.Add(new Vector2(ý[0].X+Ă,ą));ý.Add(new Vector2(Ò.X+(Ò.Width*0.5f),Ą));float[]ě=
new float[]{Ò.Width*0.15f,Ò.Height*0.25f};float[]Ě=new float[]{Ò.Width*0.85f,Ò.Height*0.25f};float[]ę=new float[]{Ò.Width*
0.15f,Ò.Height*0.4f};float[]Ę=new float[]{Ò.Width*0.225f,Ò.Height*0.4f};float[]ĝ=new float[]{Ò.Width*0.75f,Ò.Height*0.4f};
float[]ė=new float[]{Ò.Width*0.225f,Ò.Height*0.60f};ý.Add(new Vector2(Ò.X+ě[0],Ò.Y+ě[1]));ý.Add(new Vector2(Ò.X+Ě[0],Ò.Y+Ě[1]
));ý.Add(new Vector2(Ò.X+ę[0],Ò.Y+ę[1]));ý.Add(new Vector2(Ò.X+Ę[0],Ò.Y+Ę[1]));ý.Add(new Vector2(Ò.X+ĝ[0],Ò.Y+ĝ[1]));ý.
Add(new Vector2(Ò.X+ė[0],Ò.Y+ė[1]));Č=new Vector2(č.X,č.Y-ć*50);ü=new Vector2(č.X,č.Y+25f*ć+3f);Ď=new Vector2(č.X,Ò.Y+12f*ć
);Ğ=new Vector2(Ò.X+(Ò.Width*0.5f),Ò.Y+(Ò.Height*0.5f)-ć*50);Ā=ć*3.5f;ÿ=ć*2f;þ=ć*3;}}class ĕ{Program U;public Color Ĕ{get
;set;}=new Color(150,150,150);public Color ē{get;set;}=new Color(150,0,0);public Color Ē{get;set;}=new Color(10,30,50);
public Color đ{get;set;}=new Color(0,0,0);public Color Đ{get;set;}=new Color(150,150,150);public Color ď{get;set;}=new Color(
99,99,99);public Color Ė{get;set;}=new Color(255,40,40);public Color û{get;set;}=new Color(0,175,0,150);public Color ï{get
;set;}=Color.Black;public Color ã{get;set;}=Color.Black;public Color â{get;set;}=Color.Black;public float á{get;set;}=1;
public float à{get;set;}=0.5f;public List<V>ß{get;set;}Color Þ=new Color(10,10,10);const int Ý=6;float Ü=0;float Û=0;float Ú=1
;float Ù=0;float Ø=0;float Ö=0;float Õ=0;float Ô=0;float ä=0;const float Ó=1.3f;const float æ=8f;const float ú=24f;const
float ø=0.6f;const float ö=5f;const float õ=0.8f;const float ô=1f/MathHelper.PiOver2;double ó=0;double ò=0;double ñ=0;double
ù=0;double ð=5;Vector3D î=new Vector3D(0,-1,0);Vector3D í;Vector2 ì=Vector2.Zero;Vector2 ë;Vector2 ê;Vector2 é;Vector2 è;
Vector2 Ã;Vector2 ų=new Vector2(64,64);Vector2 Ƥ=new Vector2(175,32);Vector2 Ȃ=new Vector2(24,48);Vector2 Ȁ=new Vector2(32,4);
bool ǿ=false;bool Ǿ=false;bool ǽ=false;bool Ǽ=false;bool ǻ=false;bool Ǻ=false;int ǹ=0;int Ǹ=4;string[]Ƿ=new string[3];ƒ<
double>Ƕ=new ƒ<double>(5);public ĕ(List<V>ǵ,Program U){ß=ǵ;this.U=U;}public void Ǵ(bool ȁ){ǳ(ȁ);ȋ(ȁ);}void ǳ(bool ȁ){if(ǹ>Ǹ&&!
ȁ)return;if((U.ͷ==0&&!U.ν)||U.ͷ!=0){Vector3D ȍ=U.ά;Ô=(float)ȍ.Normalize();Vector3D Ȍ=Vector3D.Rotate(ȍ,MatrixD.Transpose(
U.ʹ.ɫ.WorldMatrix));ì.X=(float)Math.Asin(MathHelper.Clamp(Ȍ.X,-1,1))*ô;ì.Y=(float)Math.Asin(MathHelper.Clamp(-Ȍ.Y,-1,1))*
ô;Ǿ=Ȍ.Z>1e-3;}else{Ô=0;ì=Vector2.Zero;Ǿ=false;}ǿ=U.ͺ!=0;if(ǿ){Ț(U.ʹ.ɫ,Ý);}}void ȋ(bool ȁ){foreach(V ǜ in ß){
IMyTextSurface ġ=ǜ.R;bool Ɣ=U.Ƃ(ġ);bool Ŕ=U.ͻ.Length==0&&((U.ρ&&U.ˇ.с)||U.ͼ||(U.κ&&!U.ʿ&&U.ĸ>ŋ)||(U.ͺ==0&&U.ĸ>ŋ&&!U.ʿ&&!U.κ));bool Æ=Ŕ
&&ǹ>Ǹ;if(Æ&&!Ɣ&&!ȁ){return;}if(Ŕ&&!ȁ)ǹ++;else ǹ=0;RectangleF Ȋ=ǜ.Ò;float ć=ǜ.ć;float Ȉ=Ǿ?-1:1;Vector2 ȇ=ǜ.ĉ*ì*Ȉ*á;double Ȇ
=ȇ.Length();Vector2 ȅ=ǜ.č+ȇ;float Ȅ=ǜ.Ċ/2*à;ǽ=Ȇ>ǜ.Ċ/2||!U.ˮ&&Ȇ>Ȅ||((ȅ-ǜ.č).Y>0&&ȅ.Y>ǜ.Ą);using(var ſ=ġ.DrawFrame()){if(!U
.ʿ&&!U.κ){if(ǿ)ƭ(ſ,ǜ.č,ć,ǜ.Ċ);if(ǽ){Ƨ(U.ʹ.ɫ);ƀ(ſ,ǜ.č,ǜ.Ċ*0.5f,ć);}Ǫ(ſ,new Vector2(0,ǜ.č.Y),new Vector2(ǜ.č.X-64*ć,ǜ.č.Y),
ö*ć,Ĕ);Ǫ(ſ,new Vector2(ǜ.č.X+64*ć,ǜ.č.Y),new Vector2(ǜ.č.X*2f,ǜ.č.Y),ö*ć,Ĕ);Vector2 ȃ=ų*ć;MySprite ȉ=new MySprite(
SpriteType.TEXTURE,"AH_BoreSight",size:ȃ*1.2f,position:ǜ.č+Vector2.UnitY*ȃ*0.5f,color:Ĕ){RotationOrScale=-MathHelper.PiOver2};ſ.
Add(ȉ);if(!ǽ){MySprite ǲ=new MySprite(SpriteType.TEXTURE,"AH_VelocityVector",size:ȃ,color:!Ǿ?û:ē){Position=ȅ};ſ.Add(ǲ);if(Ǿ
){Vector2 Ǭ=Ȁ*ć;MySprite Ǣ=new MySprite(SpriteType.TEXTURE,"SquareSimple",size:Ǭ,color:ē){Position=ǲ.Position,
RotationOrScale=MathHelper.PiOver4};ſ.Add(Ǣ);Ǣ.RotationOrScale+=MathHelper.PiOver2;ſ.Add(Ǣ);}}}float ǡ=(float)(U.Ѻ/U.ʹ*100);float Ǡ=ć*ǜ
.Ć;Ȩ(ſ,ǜ.ý[0],ǡ,Ǡ,Ǳ:true,Ȓ:ã);Ȯ(ſ,ǜ.ý[1],$"{Math.Round(U.η,2)} m/s²",Ǡ,Ȓ:ï);ȓ(ſ,ǜ.ý[2],Ǡ,Ȓ:â);int[]ǟ;float Ǟ;if(U.ʹ.ɫ.
BlockDefinition.ToString().Contains("FighterCockpit")&&(U.ʹ.ɫ as IMyTextSurfaceProvider).GetSurface(0).Equals(ġ)){ǟ=new int[]{6,7,8};Ǟ=
1.1f;}else{ǟ=new int[]{3,4,5};Ǟ=1;}Ȯ(ſ,ǜ.ý[ǟ[0]],$"CRUISE",ć*ǜ.Ć/Ǟ,80,U.ː?Ė:ď,ï);Ȯ(ſ,ǜ.ý[ǟ[1]],$"PARK",ć*ǜ.Ć/Ǟ,80,U.Ѳ?Ė:ď,ï)
;Ȯ(ſ,ǜ.ý[ǟ[2]],$"DAMP",ć*ǜ.Ć/Ǟ,80,U.ˮ?Ė:ď,ï);if(U.ʿ||U.ʾ||U.κ||U.ͼ){if(U.ρ&&U.ˇ.с){Ǯ("PARKED",ſ,ǜ.Ğ,ǜ.Ā);}else if(U.ρ&&!U
.ˇ.с){Ǯ("ASSIGNING",ſ,ǜ.ü,ǜ.ÿ);ț(ſ,ǜ.Č,ǜ.ÿ,-0.5f);}else if(!U.ʿ&&U.ʾ&&!U.ˇ.с){Ǯ("UNPARKING",ſ,ǜ.ü,ǜ.ÿ);ț(ſ,ǜ.Č,ǜ.ÿ,1.2f);
}if(((U.ʿ&&U.ʾ)||U.κ)&&U.ʼ&&(U.ʔ.Ƽ(1)!=100||U.ĸ<=ŋ)){if(U.ͼ){Ǯ("STATION MODE",ſ,ǜ.Ğ,ǜ.Ā);}else{Ǯ("PARKING",ſ,ǜ.ü,ǜ.ÿ);ț(ſ
,ǜ.Č,ǜ.ÿ,0.5f);}}else if(U.κ&&!U.ʿ){Ǯ("(NOT) PARKED",ſ,ǜ.Ğ,ǜ.þ);}}if(U.ͻ.Length>0)Ǯ(U.ͻ.ToString(),ſ,ǜ.Ď,ć);ſ.Dispose();}
}}public void ǝ(){foreach(V ǜ in ß){IMyTextSurface ġ=ǜ.R;U.Ƃ(ġ,new Color(0,0,65,255));using(var ſ=ġ.DrawFrame()){ǘ(ſ,ǜ.č,
ǜ.ċ,ǜ.ć);}}}public Vector2 Ǜ(Vector2 ǚ,float Ž,float Ǚ=0){return new Vector2(ǚ.X,ǚ.Y+(16.5f+Ǚ)*Ž);}public void ǘ(
MySpriteDrawFrame ſ,Vector2 ǣ,Vector2 Ǘ,float Ž=1f){Vector2 ǚ=new Vector2(ǣ.X-Ǘ.X*0.5f,ǣ.Y-Ǘ.Y*0.5f);float ǰ=10f;float ǯ=0.54f;ſ.Add(new
MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data=
"A problem has been detected and Vector Thrust OS has been shut down to",Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž);ſ.Add(new MySprite(){Type=
SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data="prevent damage to your Grid.",Position=ǚ,Color=new Color(255,255,255,255),
FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ);ſ.Add(new MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data=
"The problem seems to be caused by the following reason: REASON.SYS",Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ);ſ.Add(new MySprite(){Type=
SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data="REASON_REASON_REASON_REASON",Position=ǚ,Color=new Color(255,255,255,255),FontId
="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ);ſ.Add(new MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data=
"If this is the first time you've seen this Stop error screen, recompile your",Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž);ſ.Add(new MySprite(){Type=
SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data="Programmable Block. If this screen appears again, follow these steps:",Position
=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ);ſ.Add(new MySprite(){Type=SpriteType.
TEXT,Alignment=TextAlignment.LEFT,Data="Check to make sure any new blocks and PB are properly installed.",Position=ǚ,Color=
new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž);ſ.Add(new MySprite(){Type=SpriteType.TEXT,
Alignment=TextAlignment.LEFT,Data="If this is a new installation, ask your grid or script manufacturer for any",Position=ǚ,Color=
new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž);ſ.Add(new MySprite(){Type=SpriteType.TEXT,
Alignment=TextAlignment.LEFT,Data="Vector Thrust OS updates you might need.",Position=ǚ,Color=new Color(255,255,255,255),FontId=
"Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ);ſ.Add(new MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data=
"If problems continue, disable or remove any newly installed blocks or scripts. ",Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž);ſ.Add(new MySprite(){Type=
SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data="Disable CUSTOM_DATA options such as nametag or surrounds. If you need to",
Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž);ſ.Add(new MySprite(){Type=SpriteType.
TEXT,Alignment=TextAlignment.LEFT,Data="Log to remove or disable something, go to the Custom Data of PB, Change Show",
Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž);ſ.Add(new MySprite(){Type=SpriteType.
TEXT,Alignment=TextAlignment.LEFT,Data="Metrics, and then Recompile the Script.",Position=ǚ,Color=new Color(255,255,255,255)
,FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ);ſ.Add(new MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,
Data="Technical information:",Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ);ſ.
Add(new MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data=
"*** STOP: 0x0000050 (0xFD3094C2, 0x00000001, 0xFBFE7617, 0x00000000)",Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});ǚ=Ǜ(ǚ,Ž,ǰ+10);ſ.Add(new MySprite(){
Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,Data=
"*** REASON.SYS -- Address VTOS0822 base at VTOS2022, DateStamp vant666p2",Position=ǚ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=ǯ*Ž});}public void Ǯ(string Ʈ,
MySpriteDrawFrame ſ,Vector2 Ǧ,float Ž=1f,bool Ǳ=true){TextAlignment ǭ=Ǳ?TextAlignment.CENTER:TextAlignment.LEFT;MySprite ǫ=new MySprite()
{Type=SpriteType.TEXT,Alignment=ǭ,Data=Ʈ,Position=Ǧ,Color=new Color(255,255,255,255),FontId="Debug",RotationOrScale=1f*Ž}
;ſ.Add(ǫ);}MySprite Ǫ(Vector2 ǩ,Vector2 Ǩ,float ǧ,Color Ƅ){Vector2 Ǧ=0.5f*(ǩ+Ǩ);Vector2 Ǥ=ǩ-Ǩ;float Ȏ=Ǥ.Length();if(Ȏ>0)Ǥ
/=Ȏ;Vector2 ƙ=new Vector2(Ȏ,ǧ);float Ȧ=(float)Math.Acos(Vector2.Dot(Ǥ,Vector2.UnitX));Ȧ*=Math.Sign(Vector2.Dot(Ǥ,Vector2.
UnitY));MySprite ȥ=MySprite.CreateSprite("SquareSimple",Ǧ,ƙ);ȥ.RotationOrScale=Ȧ;ȥ.Color=Ƅ;return ȥ;}void Ǫ(MySpriteDrawFrame
ſ,Vector2 ǩ,Vector2 Ǩ,float ǧ,Color Ƅ){Vector2 Ǧ=0.5f*(ǩ+Ǩ);Vector2 Ǥ=ǩ-Ǩ;float Ȏ=Ǥ.Length();if(Ȏ>0)Ǥ/=Ȏ;Vector2 ƙ=new
Vector2(Ȏ,ǧ);float Ȧ=(float)Math.Acos(Vector2.Dot(Ǥ,Vector2.UnitX));Ȧ*=Math.Sign(Vector2.Dot(Ǥ,Vector2.UnitY));MySprite ȥ=
MySprite.CreateSprite("SquareSimple",Ǧ,ƙ);ȥ.RotationOrScale=Ȧ;ȥ.Color=Ƅ;ſ.Add(ȥ);}void Ȥ(MySpriteDrawFrame ſ,Vector2 Ǧ,Vector2 ȣ
,Vector2 ȧ,double ȯ,Color Ƅ,Color ȭ){if(Math.Abs(ȧ.LengthSquared()-1)<MathHelper.EPSILON)ȧ.Normalize();ȣ.Y*=(float)Math.
Sqrt(1-ȯ*ȯ);Vector2 Ȭ=Vector2.One*ȣ.X;Ȭ.Y*=(float)Math.Abs(ȯ);float Ȧ=(float)Math.Acos(Vector2.Dot(ȧ,-Vector2.UnitY));Ȧ*=
Math.Sign(Vector2.Dot(ȧ,Vector2.UnitX));Vector2 ȫ=Ǧ+ȧ*ȣ.Y*0.5f;MySprite Ȫ=MySprite.CreateSprite("Circle",Ǧ,Ȭ);Ȫ.Color=Ǿ?Ƅ:ȭ;
Ȫ.RotationOrScale=Ȧ;MySprite ȩ=MySprite.CreateSprite("Triangle",ȫ,ȣ);ȩ.Color=Ƅ;ȩ.RotationOrScale=Ȧ;ſ.Add(ȩ);ſ.Add(Ȫ);}
public void Ȯ(MySpriteDrawFrame ſ,Vector2 ǣ,string Ʈ,float Ž=1f,float ǧ=140f,Color?Ƅ=null,Color?Ȓ=null){Ƅ=Ƅ??Color.White;if(Ȓ
!=null){ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=ǣ,Size=
new Vector2(ǧ,25f)*Ž,Color=Ȓ,RotationOrScale=0f});}ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.
CENTER,Data="SquareHollow",Position=ǣ,Size=new Vector2(ǧ,25f)*Ž,Color=Ƅ,RotationOrScale=0f});ſ.Add(new MySprite(){Type=
SpriteType.TEXT,Alignment=TextAlignment.CENTER,Data=Ʈ,Position=new Vector2(ǣ.X,ǣ.Y-Ž*12.5f),Color=Ƅ,FontId="Debug",RotationOrScale
=0.75f*Ž});}void Ȩ(MySpriteDrawFrame ſ,Vector2 ǣ,float Ȣ,float Ž=1f,bool Ǳ=true,Color?Ƅ=null,Color?ȑ=null,Color?Ȓ=null){
Vector2 ǚ=Ǳ?new Vector2(ǣ.X-(200f*Ž/2.25f),ǣ.Y):ǣ;TextAlignment Ȕ=Ǳ?TextAlignment.LEFT:TextAlignment.CENTER;Ȣ=MathHelper.Clamp(
Ȣ,0,100);Ȣ=(Ȣ*180)/100;Ƅ=Ƅ??Color.White;ȑ=ȑ??new Color(0,255,255,255);if(Ȓ!=null){ſ.Add(new MySprite(){Type=SpriteType.
TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareSimple",Position=ǣ,Size=new Vector2(200f,25f)*Ž,Color=Ȓ,RotationOrScale=0f})
;}ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=Ȕ,Data="SquareSimple",Position=ǚ,Size=new Vector2(Ȣ,20f)*Ž,Color
=ȑ,RotationOrScale=0f});ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="SquareHollow",
Position=ǣ,Size=new Vector2(200f,25f)*Ž,Color=Ƅ,RotationOrScale=0f});}void ȓ(MySpriteDrawFrame ſ,Vector2 ǣ,float Ž=1f,Color?Ƅ=
null,Color?Ȓ=null,Color?ȑ=null){float ǧ=200f;float Ȑ=-100+75*0.5f;Ƅ=Ƅ??Color.White;Ȓ=Ȓ??Color.Black;ȑ=ȑ??new Color(0,255,0,
255);ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.LEFT,Data="SquareSimple",Position=new Vector2(Ȑ,
0f)*Ž+ǣ,Size=new Vector2(ǧ+Ž*12,25f)*Ž,Color=Color.Black,RotationOrScale=0f});ſ.Add(new MySprite(){Type=SpriteType.TEXTURE
,Alignment=TextAlignment.LEFT,Data="SquareHollow",Position=new Vector2(Ȑ,0f)*Ž+ǣ,Size=new Vector2(ǧ+Ž*12,25f)*Ž,Color=Ƅ,
RotationOrScale=0f});float ȕ=58f;float ȏ=61f;int Ȗ=3;int ȡ=U.Ѽ.Count;int Ƞ=U.ѻ+1;int ȟ=ȡ-Ƞ;float Ȟ=ȕ*Ȗ/ȡ;float ȝ=ȏ*Ȗ/ȡ;float Ȝ=Ȑ+ǧ-ȝ;
for(int Ä=0;Ä<ȡ;Ä++){ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.LEFT,Data="SquareSimple",Position
=new Vector2(Ȝ,0f)*Ž+ǣ,Size=new Vector2(Ȟ,20f)*Ž,Color=ȟ!=0?Color.Black:ȑ,RotationOrScale=0f});ſ.Add(new MySprite(){Type=
SpriteType.TEXTURE,Alignment=TextAlignment.LEFT,Data="SquareHollow",Position=new Vector2(Ȝ,0f)*Ž+ǣ,Size=new Vector2(Ȟ,25)*Ž,Color=
Ƅ,RotationOrScale=0f});if(ȟ!=0)ȟ--;Ȝ-=ȝ;}ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.LEFT,Data=
"SquareSimple",Position=new Vector2(-100-75*0.5f,0f)*Ž+ǣ,Size=new Vector2(75f,25f)*Ž,Color=Ȓ,RotationOrScale=0f});ſ.Add(new MySprite()
{Type=SpriteType.TEXTURE,Alignment=TextAlignment.LEFT,Data="SquareHollow",Position=new Vector2(-100-75*0.5f,0f)*Ž+ǣ,Size=
new Vector2(75f,25f)*Ž,Color=Ƅ,RotationOrScale=0f});ſ.Add(new MySprite(){Type=SpriteType.TEXT,Alignment=TextAlignment.LEFT,
Data="GEAR",Position=new Vector2(-125,-13f)*Ž+ǣ,Color=Ƅ,FontId="Debug",RotationOrScale=0.8f*Ž});}public void ț(
MySpriteDrawFrame ſ,Vector2 ǣ,float Ž=1f,float Ô=1){Vector2 ƙ=new Vector2(25f,25f)*Ž;ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,
Alignment=TextAlignment.CENTER,Data="Screen_LoadingBar",Position=ǣ,Size=ƙ,Color=new Color(255,255,255,255),RotationOrScale=Ü});ſ.
Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.CENTER,Data="Screen_LoadingBar",Position=ǣ,Size=ƙ*2,
Color=new Color(255,255,255,255),RotationOrScale=Û});ſ.Add(new MySprite(){Type=SpriteType.TEXTURE,Alignment=TextAlignment.
CENTER,Data="Screen_LoadingBar",Position=ǣ,Size=ƙ*3.5f,Color=new Color(255,255,255,255),RotationOrScale=Ú});Ô*=0.0125f;Ü+=Ô*3-
MathHelper.TwoPi;Û-=Ô*3+MathHelper.TwoPi;Ú+=Ô*3-MathHelper.TwoPi;if(Ü+2100<10)Ü=Û=Ú=0;}void Ț(IMyShipController Ʀ,double ș){
Vector3D Ș=-U.ο;Vector3D ȗ=Vector3D.Cross(Ș,Ʀ.WorldMatrix.Forward);Vector3D ǥ=Vector3D.Cross(ȗ,Ș);var ǖ=Vector3D.Rotate(Ș,
MatrixD.Transpose(Ʀ.WorldMatrix));var ƅ=new Vector3D(ǖ.X,ǖ.Y,0);Ù=(float)ɪ.ɳ(ƅ,Vector3D.Up)*Math.Sign(Vector3D.Dot(Vector3D.
Right,ƅ));Ø=(float)ɪ.ɳ(ǥ,Ʀ.WorldMatrix.Forward)*Math.Sign(Vector3D.Dot(Ș,Ʀ.WorldMatrix.Forward));Ö=MyMath.FastCos(Ù);Õ=MyMath
.FastSin(Ù);double Ƣ;Ʀ.TryGetPlanetElevation(MyPlanetElevation.Surface,out Ƣ);ò=Ƣ;Ʀ.TryGetPlanetElevation(
MyPlanetElevation.Sealevel,out Ƣ);ò=Ƣ;Ƕ.ƍ((ñ-ò)*ș);double ơ=0;for(int Ä=0;Ä<Ƕ.Ƒ;++Ä){ơ+=Ƕ.Ɗ();}double Ơ=ơ/Ƕ.Ƒ;double Ɵ=ò/(Ơ);ä=(float)(Ɵ/
ð);Ǽ=Ơ>0&&Ô>10&&Ɵ<=ð;if(ǻ!=Ǽ)Ǻ=true;else Ǻ=!Ǻ;ǻ=Ǽ;ñ=ò;Vector3D ƞ=Vector3D.Cross(U.ο,î);Vector3D Ɲ=Vector3D.Cross(ƞ,U.ο);
Vector3D Ɯ=ɪ.ɵ(Ʀ.WorldMatrix.Forward,U.ο);ù=MathHelper.ToDegrees(ɪ.ɳ(Ɯ,Ɲ));if(Vector3D.Dot(Ʀ.WorldMatrix.Forward,ƞ)<0)ù=360-ù;ó=
ɪ.ɴ(U.ά,-U.ο);}void ƛ(MySpriteDrawFrame ſ,Vector2 ƚ,Vector2 ƙ,float Ƙ,float Ɨ,float Ž,bool ƣ){float Ɩ=MathHelper.
ToRadians(-Ɨ)/MathHelper.PiOver2;string ƥ=Ɩ<=0?"AH_GravityHudPositiveDegrees":"AH_GravityHudNegativeDegrees";Vector2 Ʋ=ƙ*Ž;
MySprite Ʊ=new MySprite(SpriteType.TEXTURE,ƥ,color:Đ,size:Ʋ){RotationOrScale=Ù+(Ɩ<=0?MathHelper.Pi:0),Position=ƚ+(Ɩ+Ƙ)*Ã};ſ.Add(
Ʊ);if(!ƣ)return;Vector2 ư=new Vector2(Ö,Õ)*(Ʋ.X+48f*Ž)*0.5f;Vector2 Ư=Vector2.UnitY*-24f*Ž*(Ɩ<=0?0:1);MySprite Ʈ=MySprite
.CreateText($"{Ɨ}","Debug",Đ);Ʈ.RotationOrScale=õ*Ž;Ʈ.Position=Ʊ.Position+ư+Ư;ſ.Add(Ʈ);Ʈ.Position=Ʊ.Position-ư+Ư;ſ.Add(Ʈ)
;}void ƭ(MySpriteDrawFrame ſ,Vector2 č,float Ž,float Ċ){Vector2 ƫ=č*6f;è.Y=ƫ.Y*0.5f*(1-Ö);è.X=ƫ.Y*0.5f*(Õ);Ã.Y=Ö*Ċ*0.5f;Ã
.X=-Õ*Ċ*0.5f;float Ɩ=Ø/MathHelper.PiOver2;MySprite ƪ=new MySprite(SpriteType.TEXTURE,"SquareSimple",color:Ē,size:ƫ){
RotationOrScale=Ù};Vector2 Ʃ=č+new Vector2(0,-ƫ.Y*0.5f);ƪ.Position=Ʃ+è+Ã*Ɩ;ſ.Add(ƪ);MySprite ƨ=new MySprite(SpriteType.TEXTURE,
"SquareSimple",color:đ,size:new Vector2(ƫ.X,ö*Ž)){RotationOrScale=Ù,Position=č+Ã*Ɩ,};ſ.Add(ƨ);for(int Ä=-90;Ä<=90;Ä+=30){if(Ä==0)
continue;ƛ(ſ,č,Ƥ,Ɩ,Ä,Ž,true);}}void Ƨ(IMyShipController Ʀ){MatrixD Ƭ=MatrixD.Transpose(Ʀ.WorldMatrix);Vector3D ƕ=Vector3D.Rotate
(U.ά/U.Ϋ,Ƭ);ë.X=(float)(ƕ.X)*ø;ë.Y=(float)(-ƕ.Y)*ø;ê=Vector2.SignNonZero(ë);if(!Vector2.IsZero(ref ë,MathHelper.EPSILON))
é=Vector2.Normalize(ë);Ƿ[0]=ŵ(ƕ.Z);double ƌ=í.Max();double Ɓ=í.Min();}void ƀ(MySpriteDrawFrame ſ,Vector2 č,float ž,float
Ž){float ż=Ž*Ó;float Ż=Ž*æ;float ź=Ž*ú;Vector2 Ź=Ž*Ȃ;Vector2 Ÿ=č+ë*ž;MySprite ŷ=Ǫ(č,Ÿ,Ż,!Ǿ?û:ē);MySprite Ŷ=MySprite.
CreateText("DIR","Debug",!Ǿ?û:ē,ż,TextAlignment.CENTER);Ŷ.Position=Ÿ+ź*ê-Vector2.UnitY*Ź.Y;Ȥ(ſ,Ÿ,Ȃ*Ž,é,í.X,!Ǿ?û:ē,Þ);ſ.Add(ŷ);ſ.
Add(Ŷ);}string ŵ(double Ŵ){return Ŵ<0?"CircleHollow":"Circle";}}bool Ƃ(IMyTextSurface R,Color?Ƅ=null){bool Ɣ=false;Ƅ=Ƅ??new
Color(0,0,0,255);if(true||Ƅ!=R.ScriptBackgroundColor||R.ContentType!=ContentType.SCRIPT||R.Script.Length!=0){R.
ScriptBackgroundColor=(Color)Ƅ;R.ContentType=ContentType.SCRIPT;R.Script="";}return Ɣ;}class ƒ<Ś>{public int Ƒ;Ś[]Ɛ=null;int Ə=0;int Ǝ=0;
public ƒ(int Ɠ){if(Ɠ<1)throw new Exception($"Capacity of CircularBuffer ({Ɠ}) can not be less than 1");Ƒ=Ɠ;Ɛ=new Ś[Ƒ];}public
void ƍ(Ś Ƌ){Ɛ[Ə]=Ƌ;Ə=++Ə%Ƒ;}public Ś Ɗ(){Ś ł=Ɛ[Ǝ];Ǝ=++Ǝ%Ƒ;return ł;}public Ś Ɖ(){return Ɛ[Ǝ];}}
}static class ƈ{public static bool Ƈ(this IMyThrust Ɔ){return Ɔ.IsWorking||(!Ɔ.IsWorking&&(!Ɔ.Enabled||!Ɔ.IsFunctional));
}public static double ƃ(this double Ŏ,int ƹ){double Ǌ=Math.Pow(10,ƹ);return Math.Truncate(Ŏ*Ǌ)/Ǌ;}public static float ƃ(
this float Ŏ,int ƹ){double Ǌ=Math.Pow(10,ƹ);return(float)(Math.Truncate(Ŏ*Ǌ)/Ǌ);}public static List<Ś>ǉ<Ś>(this MyIni ǈ,
string Ǉ,string ǆ){List<Ś>ǅ=new List<Ś>();try{string Ǆ=ǈ.Get(Ǉ,ǆ).ToString();string[]ǃ=Ǆ.Split(new string[]{";"},
StringSplitOptions.RemoveEmptyEntries);foreach(string W in ǃ){ǅ.Add((Ś)Convert.ChangeType(W,typeof(Ś)));}}catch{}return ǅ;}public static
List<string>ǂ(this string ǁ,string ǋ,string ǀ=""){if(ǀ.Equals(""))ǀ=ǋ;return ǁ.Split(new string[]{ǋ,ǀ},StringSplitOptions.
RemoveEmptyEntries).Where(ǌ=>ǁ.Contains(ǋ+ǌ+ǀ)).ToList();}public static double Ƹ(this double ł,double Ɓ,double ƌ){return MathHelper.Clamp(
ł,Ɓ,ƌ);}public static Vector3D ǔ(this Vector3D Ǔ,double ł=1){return Ǔ.Ǎ()*ł;}public static StringBuilder ǒ(this
StringBuilder Ǖ,string Ǒ){if(Ǖ.ƺ()&&Ǒ!=null&&Ǒ.ƺ()){Ǖ.Replace(Ǒ,"");}Ǖ.Append(Ǒ);return Ǖ;}public static bool ǐ(this IMyTerminalBlock
I,IMyTerminalBlock Ǐ)=>I.CubeGrid==Ǐ.CubeGrid;public static void ǎ(this IMyMotorStator Ř)=>Ř.TargetVelocityRPM=0;public
static void ǎ(this IMyThrust Ɔ)=>Ɔ.ThrustOverridePercentage=0;public static Vector3D Ǎ(this Vector3D ƽ){if(Vector3D.IsZero(ƽ))
return Vector3D.Zero;if(Vector3D.IsUnit(ref ƽ))return ƽ;return Vector3D.Normalize(ƽ);}public static Vector3D Ƹ(this Vector3D Ʒ
,double Ɓ,double ƌ)=>Ʒ.Ǎ()*Ʒ.Length().Ƹ(Ɓ,ƌ);public static int Ƹ(this int Ʒ,int Ɓ,int ƌ)=>MathHelper.Clamp(Ʒ,Ɓ,ƌ);public
static double ƶ(this Vector3D Ƶ,Vector3D I){return Vector3D.Dot(Ƶ,I);}public static Vector3D ƴ(this IMyShipController Ŭ){
return Vector3D.TransformNormal(Ŭ.MoveIndicator,Ŭ.WorldMatrix);}public static float Ƴ(this float U,float Ŏ){return(float)Math.
Pow(U,Ŏ);}public static double Ƴ(this double U,double Ŏ)=>Math.Pow(U,Ŏ);public static bool ƺ<Ś>(this List<Ś>Į)=>Į.Count==0;
public static bool ƺ(this string ƿ)=>ƿ.Length==0;public static bool ƺ(this StringBuilder ƿ)=>ƿ.Length==0;public static double
ƾ(this double ƹ){return Math.Abs(ƹ);}public static float ƾ(this float ƹ){return Math.Abs(ƹ);}public static Vector3D Ƽ(
this Vector3D ƽ,int ƻ=0){return Vector3D.Round(ƽ,ƻ);}public static double Ƽ(this double ł,int ƻ=0){return Math.Round(ł,ƻ);}
public static float Ƽ(this float ł,int ƻ=0){return(float)Math.Round(ł,ƻ);}