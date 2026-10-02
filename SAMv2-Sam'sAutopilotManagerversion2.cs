
// Sam's Autopilot Manager
public static string VERSION = "2.11.1";
//
// Owner: Sam (Magistrator)
//
// Documentation: http://steamcommunity.com/sharedfiles/filedetails/?id=1653875433
//
// Navigation modes: UNDOCKING -> [TAXIING] -> [ NAVIGATING | CONVERGING ] -> APPROACHING-> [TAXIING] -> DOCKING
//   NAVIGATING -> Used when obstacles are detected in the path.
//   CONVERGING -> Used when no obstacles are detected in the path.
//   TAXIING    -> Only during Path docking.
//

// Change the tag used to identify blocks
public static string TAG = "SAM";

// -------------------------------------------------------
// Update at your own peril.
// -------------------------------------------------------
private static float HORIZONT_CHECK_DISTANCE = 2000.0f; // How far the script will check for obstacles.

private static float MAX_SPEED = 95.0f; // Used for NAVIGATING and CONVERGING.
private static float APPROACHING_SPEED = 95.0f;
private static float TAXIING_SPEED = 10.0f;
private static float DOCKING_SPEED = 2.5f;

private static float APPROACH_DISTANCE = 500.0f; // Ship will start approach mode at this distance.
private static float TAXIING_DISTANCE = 10.0f; // How close will the ship get before starting the docking procedure.
private static float TAXIING_PANEL_DISTANCE = 5.0f; // When using Path-Docking, the distance from the panel.
private static float DOCK_DISTANCE = 5.0f; // Ship will start docking at this distance.
private static float UNDOCK_DISTANCE = 10.0f; // Ship will undock to this distance.

private static int LOG_MAX_LINES = 30;

// -------------------------------------------------------
// Avoid touching anything below this. Things will break.
// -------------------------------------------------------
private static string CHARGE_TARGET_GROUP_NAME = "Charge Target";
private static float DISTANCE_CHECK_TOLERANCE = 0.1f; // Script will assume the ship has reached the target position once the distance is lower than this.
private static double ROTATION_CHECK_TOLERANCE = 0.01; // Scrip will assume the ship has reached the target rotation once the rotation thresholds are lower than this.
private static float COLLISION_CORRECTION_ANGLE = (float)Math.PI / 7.5f;
private static float HORIZONT_CHECK_ANGLE_LIMIT = (float)Math.PI / 32.0f;
private static float HORIZONT_CHECK_ANGLE_STEP = (float)Math.PI / 75.0f;
private static float HORIZONT_MAX_UP_ANGLE = (float)Math.PI;
private static float COLLISION_DISABLE_RADIUS_MULTIPLIER = 2.0f;

private static double GYRO_GAIN = 1.0;
private static double GYRO_MAX_ANGULAR_VELOCITY = Math.PI;
private static float GUIDANCE_MIN_AIM_DISTANCE = 0.5f;
private static float DISTANCE_TO_GROUND_IGNORE_PLANET = 1.2f * HORIZONT_CHECK_DISTANCE;
private static int DOCK_ATTEMPTS = 5;
private static string ADVERT_ID = "SAMv2";
private static string ADVERT_ID_VER = "SAMv2V";
private static string STORAGE_VERSION = "deadbeef";
private static string CMD_TAG = TAG + "CMD";
private static string CMD_RES_TAG = TAG + "CMDRES";
private static string LEADER_TAG = TAG + "LEADER";
static float Ƀ=0.0000001f;static double Ʉ=0.166666f;static double Ʌ=1.66666f;static class Ɇ{public enum ɇ{Ɉ,ɉ,Ɋ};public
static Vector3D ɋ;public static MyDetectedEntityInfo Ɍ;public static ɇ ɍ(ref Vector3D Ɏ,bool ɏ){foreach(IMyCameraBlock ɐ in ğ.
Ĥ){if(!ɐ.CanScan(Ɏ)){continue;}Ɍ=ɐ.Raycast(Ɏ);if(Ɍ.IsEmpty()){return ɇ.ɉ;}if(Ɍ.EntityId==ğ.Ġ.CubeGrid.EntityId){continue;
}switch(Ɍ.Type){case MyDetectedEntityType.Planet:if(ɏ){return ɇ.ɉ;}goto case MyDetectedEntityType.SmallGrid;case
MyDetectedEntityType.Asteroid:case MyDetectedEntityType.LargeGrid:case MyDetectedEntityType.SmallGrid:ɋ=Ɍ.HitPosition.Value;return ɇ.Ɋ;
default:return ɇ.ɉ;}}return ɇ.Ɉ;}}static class ɑ{public static bool ɒ;public static Vector3D ź;public static Vector3D ɓ;public
static double ɔ;public static Vector3D ɕ;public static bool ɖ;public static Vector3D ɗ=new Vector3D();public static bool ɘ;
public static double ə;public static double Ǔ;public static float ɚ;public static Vector3D ɂ;public static Vector3D ɛ;public
static Vector3D ɜ;public static Vector3D ɴ;public static Vector3D ɵ;public static Vector3D ɶ;public static Vector3D ɳ;public
static Vector3D ɷ;public static MatrixD ɸ;public static Vector3D ɹ;public static Vector3D ɺ;public static Vector3D ɻ;private
static Dictionary<Base6Directions.Direction,double>ɼ=new Dictionary<Base6Directions.Direction,double>(){{Base6Directions.
Direction.Backward,0},{Base6Directions.Direction.Down,0},{Base6Directions.Direction.Forward,0},{Base6Directions.Direction.Left,0}
,{Base6Directions.Direction.Right,0},{Base6Directions.Direction.Up,0},};public static double ɽ(Vector3D ɾ){var ɿ=Ţ.đ.
WorldMatrix.GetClosestDirection(-ɾ);return ɼ.Where(ş=>ş.Value!=0&&ş.Key!=ɿ).Min(ş=>ş.Value);}public static void ʈ(){foreach(
Base6Directions.Direction ɾ in ɼ.Keys.ToList()){ɼ[ɾ]=0;}foreach(IMyThrust ȍ in ğ.Ĭ){ȍ.Enabled=true;if(!ȍ.IsWorking){continue;}ɼ[ȍ.
Orientation.Forward]+=ȍ.MaxEffectiveThrust;}ɹ=Ţ.đ.CubeGrid.WorldMatrix.GetDirectionVector(Base6Directions.Direction.Forward);ɺ=Ţ.đ.
CubeGrid.WorldMatrix.GetDirectionVector(Base6Directions.Direction.Up);ɻ=Ţ.đ.CubeGrid.WorldMatrix.GetDirectionVector(
Base6Directions.Direction.Left);ɚ=Ţ.đ.CalculateShipMass().PhysicalMass;ź=Ţ.đ.CenterOfMass;ɸ=Ţ.đ.WorldMatrix.GetOrientation();Ǔ=Ţ.đ.
CubeGrid.WorldVolume.Radius;ɴ=Ţ.đ.WorldMatrix.Forward;ɵ=Ţ.đ.WorldMatrix.Backward;ɳ=Ţ.đ.WorldMatrix.Right;ɷ=Ţ.đ.WorldMatrix.Left;
ɜ=Ţ.đ.WorldMatrix.Up;ɶ=Ţ.đ.WorldMatrix.Down;ɓ=Ţ.đ.GetShipVelocities().LinearVelocity;ɔ=Vector3D.Dot(ɓ,ɜ);ɖ=Ţ.đ.
TryGetPlanetPosition(out ɗ);ɕ=Ţ.đ.GetNaturalGravity();ɘ=ɕ.Length()>=0.5;if(ɘ){Ţ.đ.TryGetPlanetElevation(MyPlanetElevation.Surface,out ə);ɛ=
Vector3D.Normalize(ɕ);ɂ=-1*ɛ;}else{ə=DISTANCE_TO_GROUND_IGNORE_PLANET;ɛ=ɶ;ɂ=ɜ;}}}static class ʀ{public static float ʁ=0.0f;
public static bool Ɍ=false;private static bool ɏ;private static Vector3D ʂ;private static float ʃ=1.0f;private static float ž=
1.0f,ʄ=1.0f,ʅ=1.0f;public static Vector3D?ʆ(float ʇ,Vector3D ɴ,Vector3D ɳ){ʂ=ɑ.ź+Math.Min(ʇ,HORIZONT_CHECK_DISTANCE)*
Vector3D.Transform(ɴ,Quaternion.CreateFromAxisAngle(ɳ,ʁ));ɏ=ɑ.ə>=DISTANCE_TO_GROUND_IGNORE_PLANET;if(Ɍ){switch(Ɇ.ɍ(ref ʂ,ɏ)){
case Ɇ.ɇ.Ɋ:ʁ+=HORIZONT_CHECK_ANGLE_STEP*ž;ž=Math.Min(10.0f,ž+1.0f);ʄ=1.0f;ʅ=1.0f;ʁ=(float)Math.Min(HORIZONT_MAX_UP_ANGLE,ʁ);
return Vector3D.Transform(ɴ,Quaternion.CreateFromAxisAngle(ɳ,ʁ));case Ɇ.ɇ.ɉ:ʁ-=HORIZONT_CHECK_ANGLE_STEP*ʄ;ʄ=Math.Min(10.0f,ʄ+
1.0f);ž=1.0f;ʅ=1.0f;if(ʁ<-HORIZONT_CHECK_ANGLE_LIMIT){Ɍ=false;ʁ=0.0f;ž=ʄ=ʅ=1.0f;return Vector3D.Zero;}return Vector3D.
Transform(ɴ,Quaternion.CreateFromAxisAngle(ɳ,ʁ));}}else{switch(Ɇ.ɍ(ref ʂ,ɏ)){case Ɇ.ɇ.Ɋ:Ɍ=true;ž=ʄ=ʅ=1.0f;return Vector3D.
Transform(ɴ,Quaternion.CreateFromAxisAngle(ɳ,ʁ));case Ɇ.ɇ.ɉ:ž=ʄ=1.0f;ʁ+=ʃ*ʅ*HORIZONT_CHECK_ANGLE_STEP;ʅ=Math.Min(10.0f,ʅ+1.0f);if
(Math.Abs(ʁ)>HORIZONT_CHECK_ANGLE_LIMIT){ʁ=Math.Min(HORIZONT_CHECK_ANGLE_LIMIT,Math.Max(ʁ,-HORIZONT_CHECK_ANGLE_LIMIT));ʃ
*=-1.0f;ʅ=1.0f;}return Vector3D.Zero;}}return null;}}static class ɝ{private static Vector3D ɱ=new Vector3D();private
static Vector3D ɞ=new Vector3D();private static Vector3D ɟ=new Vector3D();private static float ɠ=MAX_SPEED;public static void
Ǟ(Ƃ Ȝ){ɱ=Ȝ.ƃ.ź;ɞ=Ȝ.ƃ.Ž;ɟ=Ȝ.ƃ.ž;ɠ=Ȝ.Ƅ;}public static void ɡ(){foreach(IMyGyro Ɂ in ğ.ī){Ɂ.GyroOverride=false;}foreach(
IMyThrust ȍ in ğ.Ĭ){ȍ.ThrustOverride=0;}}public static void Å(){ɝ.ɧ();ɝ.ɲ();ɝ.ȉ();}public static bool ǲ(){return Math.Abs(ɯ)<=
ROTATION_CHECK_TOLERANCE&&Math.Abs(ɮ)<=ROTATION_CHECK_TOLERANCE&&Math.Abs(ɰ)<=ROTATION_CHECK_TOLERANCE&&ɦ<=DISTANCE_CHECK_TOLERANCE;}private
static Vector3D ɢ,ɣ,ɤ,ɜ,ɥ;private static float ɦ;private static void ɧ(){ɣ=ɱ-ɑ.ź;ɦ=(float)ɣ.Length();ɢ=Vector3D.Normalize(ɣ);
if(ɞ!=Vector3D.Zero){ɤ=ɑ.ź+ɞ*ɑ.Ǔ;}else{ɥ=(ɦ>GUIDANCE_MIN_AIM_DISTANCE)?ɢ:ɑ.ɴ;if(ɑ.ɘ&&!ɑ.ɒ){ɤ=ɑ.ź+Vector3D.Normalize(
Vector3D.ProjectOnPlane(ref ɥ,ref ɑ.ɂ))*ɑ.Ǔ;}else{ɤ=ɑ.ź+ɥ*ɑ.Ǔ;}}if(ɑ.ɘ&&!ɑ.ɒ){ɜ=(ɟ==Vector3D.Zero)?ɑ.ɂ:ɟ;}else{ɜ=(ɟ==Vector3D.
Zero)?Vector3D.Cross(ɥ,ɑ.ɷ):ɟ;}}private static Quaternion ɨ;private static Vector3D ƀ,ɩ,ɪ,ɫ,ɬ,ɭ;private static double ɮ,ɯ,ɰ;
private static void ɲ(){if(ğ.ī.Count==0){return;}ƀ=Vector3D.Normalize(ɤ-ɑ.ź);ɨ=Quaternion.Inverse(Quaternion.
CreateFromForwardUp(ɑ.ɴ,ɑ.ɜ));ɩ=Vector3D.Transform(ƀ,ɨ);Vector3D.GetAzimuthAndElevation(ɩ,out ɮ,out ɯ);ɬ=Vector3D.ProjectOnPlane(ref ɜ,ref
ƀ);ɬ.Normalize();ɭ=Vector3D.Cross(ƀ,ɬ);ɭ.Normalize();ɰ=Vector3D.Dot(ɑ.ɜ,ɭ);ɪ=Vector3.Transform((new Vector3D(ɯ,ɮ,ɰ)),ɑ.ɸ)
;foreach(IMyGyro Ɂ in ğ.ī){ɫ=Vector3.Transform(ɪ,Matrix.Transpose(Ɂ.WorldMatrix.GetOrientation()));Ɂ.Pitch=(float)
MathHelper.Clamp((-ɫ.X*GYRO_GAIN),-GYRO_MAX_ANGULAR_VELOCITY,GYRO_MAX_ANGULAR_VELOCITY);Ɂ.Yaw=(float)MathHelper.Clamp(((-ɫ.Y)*
GYRO_GAIN),-GYRO_MAX_ANGULAR_VELOCITY,GYRO_MAX_ANGULAR_VELOCITY);Ɂ.Roll=(float)MathHelper.Clamp(((-ɫ.Z)*GYRO_GAIN),-
GYRO_MAX_ANGULAR_VELOCITY,GYRO_MAX_ANGULAR_VELOCITY);Ɂ.GyroOverride=true;}}private static float ȁ,Ȁ,Ȃ,ȃ;private static Vector3D Ȅ,ȅ,Ȇ;private
static double ȇ,Ȉ;private static void ȉ(){ȇ=ɑ.ɽ(ɢ);var Ȋ=-110.1f*ɑ.ɚ/ȇ+203.3f;var ȋ=2.5*ɑ.ɚ/ȇ+2.9;var Ȍ=Math.Min(1.0,Math.Pow(
(1.0/Ȋ)*ɦ,1.0/ȋ));Ȉ=Ȍ*Math.Sqrt(2.0f*ȇ*ɦ/ɑ.ɚ);ȅ=ɑ.ɓ/Ʉ;Ȇ=Math.Min(ɠ,Ȉ)*ɢ/Ʉ;Ȅ=ɑ.ɚ*(-Ȇ+ȅ)+ɑ.ɚ*ɑ.ɕ;ȁ=(float)Vector3D.Dot(Ȅ,ɑ.
ɹ);Ȁ=(float)Vector3D.Dot(Ȅ,ɑ.ɺ);Ȃ=(float)Vector3D.Dot(Ȅ,ɑ.ɻ);foreach(IMyThrust ȍ in ğ.Ĭ){if(!ȍ.IsWorking){ȍ.
ThrustOverridePercentage=0;continue;}switch(ȍ.Orientation.Forward){case Base6Directions.Direction.Forward:ȍ.ThrustOverridePercentage=((ȁ<0)?Ƀ:(ɝ
.Ȏ(ref ȁ,ȍ.MaxEffectiveThrust)));break;case Base6Directions.Direction.Backward:ȍ.ThrustOverridePercentage=((ȁ>0)?Ƀ:(ɝ.Ȏ(
ref ȁ,ȍ.MaxEffectiveThrust)));break;case Base6Directions.Direction.Up:ȍ.ThrustOverridePercentage=((Ȁ<0)?Ƀ:(ɝ.Ȏ(ref Ȁ,ȍ.
MaxEffectiveThrust)));break;case Base6Directions.Direction.Down:ȍ.ThrustOverridePercentage=((Ȁ>0)?Ƀ:(ɝ.Ȏ(ref Ȁ,ȍ.MaxEffectiveThrust)));
break;case Base6Directions.Direction.Left:ȍ.ThrustOverridePercentage=((Ȃ<0)?Ƀ:(ɝ.Ȏ(ref Ȃ,ȍ.MaxEffectiveThrust)));break;case
Base6Directions.Direction.Right:ȍ.ThrustOverridePercentage=((Ȃ>0)?Ƀ:(ɝ.Ȏ(ref Ȃ,ȍ.MaxEffectiveThrust)));break;}}}private static float Ȏ(
ref float ȏ,float Ȑ){ȃ=Math.Min(Math.Abs(ȏ),Ȑ);ȏ=(ȏ>0.0f)?(ȏ-ȃ):(ȏ+ȃ);return Math.Max(ȃ/Ȑ,Ƀ);}}static class ȑ{public static
List<Ƃ>Ȓ=new List<Ƃ>{};public static string ȓ(){if(Ȓ.Count==0){return"";}return Ȓ[0].Ŧ();}private static Vector3D?Ȕ;private
static Vector3D ȕ,Ȗ,ȗ,Ș,ș,Ț;private static void ț(){switch(Ȓ[0].P){case Ƃ.ƅ.Ɖ:if((Ȓ[0].ƃ.ź-ɑ.ź).Length()<APPROACH_DISTANCE){Ȓ[
0].P=Ƃ.ƅ.ƍ;Ȓ[0].Ƅ=APPROACHING_SPEED;}goto case Ƃ.ƅ.ƍ;case Ƃ.ƅ.ƍ:if((Ȓ[0].ƃ.ź-ɑ.ź).Length()<
COLLISION_DISABLE_RADIUS_MULTIPLIER*ɑ.Ǔ){return;}break;case Ƃ.ƅ.Ɗ:break;case Ƃ.ƅ.Ǝ:if(ǳ.Ǵ.ź!=Vector3D.Zero){Ȓ[0].ƃ.ź=ǳ.Ǵ.ź+ǳ.ǵ.Z*ɑ.Ǔ*ǳ.Ǵ.ž+ǳ.ǵ.X*ɑ.Ǔ*ǳ.Ǵ.Ǒ+
ǳ.ǵ.Y*ɑ.Ǔ*Vector3D.Cross(ǳ.Ǵ.Ǒ,ǳ.Ǵ.ž)+2.0*Ʌ*Vector3D.Dot(ǳ.Ǵ.ǒ,ǳ.Ǵ.Ǒ)*ǳ.Ǵ.Ǒ+0.5*Ʌ*Vector3D.Dot(ǳ.Ǵ.ǒ,ǳ.Ǵ.ž)*ǳ.Ǵ.ž;return;
}Ȓ[0].ƃ.ź=ɑ.ź;return;default:return;}ȕ=Ȓ[Ȓ[0].P==Ƃ.ƅ.Ɗ?1:0].ƃ.ź;Ȗ=ȕ-ɑ.ź;ȗ=Vector3D.Normalize(Ȗ);Ț=Vector3D.Normalize(
Vector3D.Cross(ȗ,ɑ.ɂ));Ȕ=ʀ.ʆ((float)Ȗ.Length(),ȗ,Ț);if(!Ȕ.HasValue){return;}if(Vector3D.IsZero(Ȕ.Value)){if(Ȓ[0].P==Ƃ.ƅ.Ɗ){Ȓ.
RemoveAt(0);}return;}Ș=Vector3D.Transform(Ȕ.Value,Quaternion.CreateFromAxisAngle(Ț,COLLISION_CORRECTION_ANGLE));ș=ɑ.ź+Math.Min(
HORIZONT_CHECK_DISTANCE,(ɑ.ź-Ȓ.Last().ƃ.ź).Length())*Ș;if(Ȓ[0].P==Ƃ.ƅ.Ɗ){Ȓ[0].ƃ.ź=ș;}else{Ȓ.Insert(0,new Ƃ(new ż(ș,Vector3D.Zero,Vector3D.Zero)
,MAX_SPEED,Ƃ.ƅ.Ɗ));}}public static void Å(){if(Ȓ.Count==0){return;}ɑ.ʈ();ț();ɝ.Ǟ(Ȓ.ElementAt(0));ɝ.Å();if(Ȓ[0].P==Ƃ.ƅ.Ə){
if((Ȓ[0].ƃ.ź-ɑ.ź).Length()<APPROACH_DISTANCE){Ȓ.Clear();ɝ.ɡ();}}if(ɝ.ǲ()){if(Ȓ[0].P==Ƃ.ƅ.Ǝ){return;}Ȓ.RemoveAt(0);if(Ȓ.
Count!=0){return;}ɝ.ɡ();}}public static void ǰ(Ƃ Ȝ){Ȓ.Insert(0,Ȝ);}public static void ǰ(ż N,float Ż,Ƃ.ƅ Ź){ǰ(new Ƃ(N,Ż,Ź));}
public static void ǰ(Vector3D Æ,Vector3D ŷ,Vector3D Ÿ,float Ż,Ƃ.ƅ Ź){ǰ(new ż(Æ,ŷ,Ÿ),Ż,Ź);}public static void Ǳ(){ɝ.ɡ();Ȓ.Clear
();}public static bool ǲ(){return Ȓ.Count==0;}}static class ǳ{public static ǐ Ǵ;public static Vector3D ǵ;private static ǐ
Ƕ;private static string Ƿ="";private static double Ĕ;public static void Ǹ(string g){if(!ʲ.ǧ(ğ.Ġ.EntityId,"Follow",ref Ƿ))
{return;}ã.ǅ(g);Ƕ=ã.Ǐ();if(Ƕ.ö!=Ƿ){return;}Ǵ=Ƕ;if(ʲ.ǧ(ğ.Ġ.EntityId,"FollowFront",ref Ƿ)&&double.TryParse(Ƿ,out Ĕ)){ǵ.X=Ĕ;
}else{ǵ.X=10.0;}if(ʲ.ǧ(ğ.Ġ.EntityId,"FollowUp",ref Ƿ)&&double.TryParse(Ƿ,out Ĕ)){ǵ.Z=Ĕ;}else{ǵ.Z=10.0;}if(ʲ.ǧ(ğ.Ġ.
EntityId,"FollowRight",ref Ƿ)&&double.TryParse(Ƿ,out Ĕ)){ǵ.Y=Ĕ;}else{ǵ.Y=10.0;}}public static void ǹ(Program Æ){if(!ʲ.ʾ(ğ.Ġ.
EntityId,"LEADER")){return;}if(Ţ.đ==null){û.Ñ("No Remote Control");return;}Ƕ.ź=Ţ.đ.CenterOfMass;if(!ʲ.ǧ(ğ.Ġ.EntityId,"Name",ref
Ƿ)){Ƿ=ğ.Ġ.CubeGrid.CustomName;}Ƕ.ö=Ƿ;Ƕ.Ǔ=ğ.Ġ.CubeGrid.WorldVolume.Radius;Ƕ.ǒ=Ţ.đ.GetShipVelocities().LinearVelocity;Ƕ.ž=Ţ
.đ.WorldMatrix.Up;Ƕ.Ǒ=Ţ.đ.WorldMatrix.Forward;ã.ç();ã.h(Ƕ);Æ.IGC.SendBroadcastMessage<string>(LEADER_TAG,ã.å);}}static
class Ǻ{public static bool Ö=false;public static Ŵ ǻ;public enum Ǽ{ǽ,Ǿ,ǿ};public static Ǽ ǯ=Ǽ.ǽ;public static long ȝ=long.
MaxValue;public static long Ȟ=TimeSpan.FromSeconds(10.0).Ticks;public static void Ȭ(){ȝ=DateTime.Now.Ticks;}public static void ȭ
(){Ö=true;ǻ=null;}public static void ȭ(Ŵ Ȯ){Ö=true;ǻ=Ȯ;ȿ.ȩ(false);ȿ.ɀ(true);}public static void ȯ(){Ö=false;ǻ=null;}
private static bool Ȱ;private static bool ȱ;public static void Å(){if(!Ö||β.Ά){return;}if(ǯ==Ǽ.ǽ){ȯ();return;}if(ǻ!=null&&ǻ.ƶ!=
Ŵ.ƭ.ƴ){if(ǻ.ƶ==Ŵ.ƭ.Ʈ){if(DateTime.Now.Ticks-ȝ<Ȟ){return;}}else{if(Ť.ľ()==null&&ǻ.Ʃ!="Manual"){return;}ȱ=true;switch(ǻ.ƶ){
case Ŵ.ƭ.ư:case Ŵ.ƭ.Ʋ:if(!ȿ.ȟ()){ȱ=false;}break;case Ŵ.ƭ.Ʊ:case Ŵ.ƭ.Ƴ:if(!ȿ.ȡ()){ȿ.Ȣ(true);ȱ=false;break;}ȿ.Ȣ(false);break;}
Ȱ=true;switch(ǻ.ƶ){case Ŵ.ƭ.Ư:case Ŵ.ƭ.Ʋ:case Ŵ.ƭ.Ƴ:if(!ȿ.ȥ()){ȿ.ȩ(true);Ȱ=false;return;}ȿ.ȩ(false);break;case Ŵ.ƭ.Ƶ:if(!
ȿ.Ȧ()){ȿ.Ȫ(true);Ȱ=false;return;}ȿ.Ȫ(false);break;}if(!ȱ||!Ȱ){return;}}}κ.ʜ(G.W.Y);if(ǯ==Ǽ.Ǿ&&κ.μ==0){ȯ();û.é(
"Dock list finished, stopping autopilot.");return;}if(ǻ==null){û.é("Just a waypoint, navigating to next dock.");}else{switch(ǻ.ƶ){case Ŵ.ƭ.Ʈ:û.é(
"Wait time expired, resuming navigation.");break;case Ŵ.ƭ.ư:û.é("Cargo loaded, resuming navigation.");break;case Ŵ.ƭ.Ʊ:û.é("Cargo unloaded, resuming navigation."
);break;case Ŵ.ƭ.Ư:û.é("Charged, resuming navigation.");break;case Ŵ.ƭ.Ʋ:û.é(
"Charged and cargo loaded, resuming navigation.");break;case Ŵ.ƭ.Ƴ:û.é("Cargo unloaded, resuming navigation.");break;case Ŵ.ƭ.ƴ:break;case Ŵ.ƭ.Ƶ:û.é(
"Charge low, resuming navitation.");break;}}β.ˤ();}private static string Ȳ;private static bool ȳ;private static string ȴ;private static List<KeyValuePair<
ǀ,Ŵ>>ȵ=new List<KeyValuePair<ǀ,Ŵ>>{};private static List<ǀ>ȶ=new List<ǀ>{};private static List<ǀ>ȷ=new List<ǀ>{};private
static string ª;public static void ȸ(Program Æ,string r){ã.ǅ(r);Ȳ=ȹ(ã.ǭ());if(Ȳ==""){return;}Æ.IGC.SendBroadcastMessage<string
>(CMD_RES_TAG,Ȳ);}public static string ȹ(ƕ Ê){if(!ʲ.ǧ(ğ.Ġ.EntityId,"Name",ref ȴ)){ȴ=ğ.Ġ.CubeGrid.CustomName;}if(Ê.Ɨ!=ȴ){
return"";}if(Ê.Ɩ<0||Ê.Ɩ>Ě.ÿ.Count-1){return"Received a command that does not exist: "+Ê;}û.é("Remote command received.");ª=Ě.ÿ
[Ê.Ɩ];if(ª.Contains("start")){β.ˤ();return"Start success.";}else if(ª.Contains("stop")){β.Ǳ();return"Stop success.";}ȵ.
Clear();ȶ.Clear();foreach(ǀ Ⱥ in Ê.Ƙ){ȳ=false;foreach(Ŵ Ɓ in κ.Ψ.ToArray()){if(Ⱥ.ƒ==Vector3D.Zero){if(Ⱥ.Ɛ==Ɓ.ƫ){if(Ⱥ.ƣ==""||Ⱥ
.ƣ==Ɓ.Ʃ){ȵ.Add(new KeyValuePair<ǀ,Ŵ>(Ⱥ,Ɓ));ȳ=true;break;}}}else{if(Ɓ.Ʃ=="Manual"&&Ɓ.ƃ.ź==Ⱥ.ƒ){ȵ.Add(new KeyValuePair<ǀ,Ŵ>
(Ⱥ,Ɓ));ȳ=true;break;}}}if(!ȳ){if(Ⱥ.ƒ!=Vector3D.Zero){ȵ.Add(new KeyValuePair<ǀ,Ŵ>(Ⱥ,κ.ʘ(Ⱥ.Ƒ,Ⱥ.ƒ)));}else{ȶ.Add(Ⱥ);}}}if(ȶ.
Count!=0){return"Not found: "+ȶ.Count.ToString();}β.Ǳ();κ.Ν.Clear();κ.μ=0;foreach(KeyValuePair<ǀ,Ŵ>Ȼ in ȵ){Ȼ.Value.ƶ=Ȼ.Key.ƥ;
κ.Ν.Add(Ȼ.Value);}κ.ʛ();if(ª.Contains("step")){Ƚ(Ǽ.ǽ);}else if(ª.Contains("run")){Ƚ(Ǽ.Ǿ);}else if(ª.Contains("loop")){Ƚ(Ǽ
.ǿ);}if(!ª.Contains("conf")){β.ˤ();}return"Command executed";}private static string ȼ;private static void Ƚ(Ǽ Ⱦ){ǯ=Ⱦ;ȼ=ğ.
Ġ.CustomName;if(Ⱦ==Ǽ.Ǿ){ȼ=ȼ.Replace(" LOOP","");ȼ=ȼ.Replace("["+TAG,"["+TAG+" LIST");ʲ.ʹ(ğ.Ġ.EntityId,"LIST","");ʲ.Ğ(ğ.Ġ.
EntityId,"LOOP");}else if(Ⱦ==Ǽ.ǿ){ȼ=ȼ.Replace(" LIST","");ȼ=ȼ.Replace("["+TAG,"["+TAG+" LOOP");ʲ.ʹ(ğ.Ġ.EntityId,"LOOP","");ʲ.Ğ(ğ
.Ġ.EntityId,"LIST");}else{ȼ=ȼ.Replace(" LIST","");ȼ=ȼ.Replace(" LOOP","");ʲ.Ğ(ğ.Ġ.EntityId,"LOOP");ʲ.Ğ(ğ.Ġ.EntityId,
"LIST");}ğ.Ġ.CustomName=ȼ;}}static class ȿ{private static string Ē;private static float ē;public static void ɀ(bool ȣ){Ţ.đ.
DampenersOverride=ȣ;}private static List<IMyGasTank>ȫ=new List<IMyGasTank>();public static bool ȟ(){ȫ.Clear();foreach(IMyGasTank ċ in ğ.ı
){if(!ʲ.ʾ(ċ.EntityId,"CARGO")){continue;}ȫ.Add(ċ);ċ.Stockpile=true;}foreach(IMyGasTank ċ in ȫ){ē=95.0f;if(ʲ.ǧ(ċ.EntityId,
"Full",ref Ē)){if(!float.TryParse(Ē,out ē)){ē=95.0f;}}if(ċ.FilledRatio<ē/100.0f){return false;}}foreach(IMyCargoContainer Ċ in
ğ.İ){ē=90f;if(ʲ.ǧ(Ċ.EntityId,"Full",ref Ē)){if(!float.TryParse(Ē,out ē)){ē=90f;}}IMyInventory Ƞ=Ċ.GetInventory();if(Ƞ.
CurrentVolume.RawValue<(Ƞ.MaxVolume.RawValue*ē/100.0f)){return false;}}return true;}public static bool ȡ(){ȫ.Clear();foreach(
IMyGasTank ċ in ğ.ı){if(!ʲ.ʾ(ċ.EntityId,"CARGO")){continue;}ȫ.Add(ċ);ċ.Stockpile=false;}foreach(IMyGasTank ċ in ȫ){ē=0.0f;if(ʲ.ǧ(ċ
.EntityId,"Empty",ref Ē)){if(!float.TryParse(Ē,out ē)){ē=0.0f;}}if(ċ.FilledRatio>ē/100.0f){return false;}}foreach(
IMyCargoContainer Ċ in ğ.İ){ē=0.0f;if(ʲ.ǧ(Ċ.EntityId,"Empty",ref Ē)){if(!float.TryParse(Ē,out ē)){ē=0.0f;}}IMyInventory Ƞ=Ċ.GetInventory(
);if(Ƞ.CurrentVolume.RawValue>(Ƞ.MaxVolume.RawValue*ē/100.0f)){return false;}}return true;}public static void Ȣ(bool ȣ){
foreach(IMyGasTank ċ in ğ.ı){if(ʲ.ʾ(ċ.EntityId,"CARGO")){foreach(IMyGasTank Ȥ in ğ.ĳ){Ȥ.Stockpile=ȣ;}return;}}}public static
bool ȥ(){foreach(IMyGasTank ċ in ğ.ı){if(ʲ.ʾ(ċ.EntityId,"CARGO")){continue;}ē=95.0f;ċ.Stockpile=true;if(ʲ.ǧ(ċ.EntityId,
"Full",ref Ē)){if(!float.TryParse(Ē,out ē)){ē=95.0f;}}if(ċ.FilledRatio<ē/100.0f){return false;}}foreach(IMyBatteryBlock ĉ in ğ
.į){ē=95f;if(ʲ.ǧ(ĉ.EntityId,"Full",ref Ē)){if(!float.TryParse(Ē,out ē)){ē=95f;}}if(ĉ.CurrentStoredPower<(ĉ.MaxStoredPower
*ē/100.0f)){return false;}}û.é("Everything is charged!");return true;}public static bool Ȧ(){foreach(IMyBatteryBlock ĉ in
ğ.į){ē=25.0f;if(ʲ.ǧ(ĉ.EntityId,"Empty",ref Ē)){if(!float.TryParse(Ē,out ē)){ē=25.0f;}}if(ĉ.CurrentStoredPower<(ĉ.
MaxStoredPower*ē/100.0f)){return true;}}return false;}private static bool ȧ,Ȩ;public static void ȩ(bool ȣ){foreach(IMyBatteryBlock ĉ
in ğ.į){ȧ=ʲ.ʾ(ĉ.EntityId,"FORCE");ĉ.ChargeMode=ȣ&&ȧ?ChargeMode.Recharge:ChargeMode.Auto;}foreach(IMyGasTank ċ in ğ.ı){if(ʲ
.ʾ(ċ.EntityId,"CARGO")){continue;}ȧ=ʲ.ʾ(ċ.EntityId,"FORCE");ċ.Stockpile=ȣ&&ȧ;}}public static void Ȫ(bool ȣ){foreach(
IMyBatteryBlock ĉ in ğ.į){Ȩ=ʲ.ʾ(ĉ.EntityId,"FORCE");ĉ.ChargeMode=ȣ&&Ȩ?ChargeMode.Discharge:ChargeMode.Auto;}foreach(IMyBatteryBlock ĉ
in ğ.Ĳ){ĉ.ChargeMode=ȣ?ChargeMode.Recharge:ChargeMode.Auto;}}}static class β{public static bool Ά=false;public static List
<Ŵ>Ȯ=new List<Ŵ>();public static void Å(){if(!Ά){return;}if(!Ţ.ţ()){if(!Ǚ.õ(Ǚ.ǚ.ǜ)){û.Ñ("No Remote Control!");}Ǚ.Ǟ(Ǚ.ǚ.ǜ)
;Ǳ();return;}if(ȑ.ǲ()){if(Ȯ.Count!=0&&Ȯ[0].ƨ!=0){if(Ȯ[0].ƶ==Ŵ.ƭ.ƴ){Ȯ.Clear();û.é("Hop successful!");Ǻ.Ȭ();Ά=false;return;
}Δ();Ȯ.Clear();return;}û.é("Navigation successful!");Ŕ.Ŝ(Ŕ.ŕ.ŗ);Ǻ.Ȭ();Ť.ĸ();Ά=false;return;}ȑ.Å();}private static
Quaternion Έ,Ή,Ί;private static Vector3D Ό,Ύ,Ώ,ΐ,ž,Α,ƀ,Β;private static IMyShipConnector Ľ;private static bool ņ;private static
float Γ;private static void Δ(){Ľ=Ť.Ň(Ȯ[0]);if(Ľ==null){û.Ð("No connectors available!");return;}ɑ.ʈ();Ό=ɑ.ź-Ľ.GetPosition();
if(Math.Abs(Vector3D.Dot(Ȯ[0].ƃ.Ž,ɑ.ɂ))<0.5f){ž=ɑ.ɂ;Α=Ľ.WorldMatrix.GetDirectionVector(Ľ.WorldMatrix.GetClosestDirection(ž
));Α=(Α==Ľ.WorldMatrix.Forward||Α==Ľ.WorldMatrix.Backward)?Ľ.WorldMatrix.Up:Α;}else{ž=Ȯ[0].ƃ.ž;Α=Ľ.WorldMatrix.Up;}ņ=ʲ.ʾ(
Ľ.EntityId,"REV");Έ=Quaternion.Inverse(Quaternion.CreateFromForwardUp(ņ?Ľ.WorldMatrix.Backward:Ľ.WorldMatrix.Forward,Α));
Ή=Quaternion.CreateFromForwardUp(-Ȯ[0].ƃ.Ž,ž);Ί=Ή*Έ;Ύ=Vector3D.Transform(Ό,Ί);ΐ=Vector3D.Transform(Ţ.đ.WorldMatrix.
Forward,Ί);Ώ=Vector3D.Transform(Ţ.đ.WorldMatrix.Up,Ί);Γ=(Ȯ[0].Ƨ==VRage.Game.MyCubeSize.Large)?2.6f/2.0f:0.5f;Γ+=(Ľ.CubeGrid.
GridSizeEnum==VRage.Game.MyCubeSize.Large)?2.6f/2.0f:0.5f;Ζ=Ȯ[0].ƃ.ź+Ύ+(Γ*Ȯ[0].ƃ.Ž);ȑ.ǰ(Ζ,ΐ,Ώ,DOCKING_SPEED,Ƃ.ƅ.Ƈ);Ζ=Ȯ[0].ƃ.ź+Ύ+((
DOCK_DISTANCE+Γ)*Ȯ[0].ƃ.Ž);ȑ.ǰ(Ζ,ΐ,Ώ,TAXIING_SPEED,Ƃ.ƅ.ƌ);Ȯ[0].Ʀ.Reverse();foreach(ſ Ε in Ȯ[0].Ʀ){Ζ=Ε.ź+(Ε.ƀ*(TAXIING_PANEL_DISTANCE+
ɑ.Ǔ));ȑ.ǰ(Ζ,Vector3D.Zero,Vector3D.Zero,TAXIING_SPEED,Ƃ.ƅ.ƌ);}Ȯ[0].Ʀ.Reverse();}private static Vector3D Ζ,Η;private
static Ŵ Θ;private static Ƃ.ƅ Ι;private static void Κ(Ŵ Ȯ){ɑ.ʈ();Ι=(Ȯ.ƶ==Ŵ.ƭ.ƴ)?Ƃ.ƅ.Ə:Ƃ.ƅ.Ɖ;if(Ȯ.ƪ==0){ȑ.ǰ(Ȯ.ƃ,MAX_SPEED,Ƃ.ƅ.Ɔ
);ȑ.ǰ(Ȯ.ƃ.ź,Vector3D.Zero,Vector3D.Zero,MAX_SPEED,Ι);Ζ=Ȯ.ƃ.ź;}else{if(Ȯ.Ʀ.Count==0){Ζ=Ȯ.ƃ.ź+((TAXIING_DISTANCE+ɑ.Ǔ)*Ȯ.ƃ.Ž
);}else{Ζ=Ȯ.Ʀ[0].ź+((TAXIING_DISTANCE+ɑ.Ǔ)*Ȯ.Ʀ[0].ƀ);}ȑ.ǰ(Ζ,Vector3D.Zero,Vector3D.Zero,MAX_SPEED,Ι);}if(ɑ.ɓ.Length()>=
2.0f){return;}Θ=Ť.Ņ();ƀ=Vector3D.Normalize(Ζ-ɑ.ź);Β=Vector3D.ProjectOnPlane(ref ƀ,ref ɑ.ɂ);if(Θ==null){ȑ.ǰ(ɑ.ź,Β,ɑ.ɂ,
MAX_SPEED,Ƃ.ƅ.Ɔ);return;}if(Θ.Ʀ.Count>0){foreach(ſ Ε in Θ.Ʀ){Ζ=Ε.ź+(Ε.ƀ*(TAXIING_PANEL_DISTANCE+ɑ.Ǔ));ȑ.ǰ(Ζ,Vector3D.Zero,
Vector3D.Zero,TAXIING_SPEED,Ƃ.ƅ.ƌ);}}Η=Θ.ƃ.Ž;Η*=(ɑ.Ǔ+UNDOCK_DISTANCE);Η+=ɑ.ź;ȑ.ǰ(Η,Β,ɑ.ɂ,DOCKING_SPEED,Ƃ.ƅ.Ɔ);ȑ.ǰ(Η,ɑ.ɴ,ɑ.ɜ,
DOCKING_SPEED,Ƃ.ƅ.ƈ);}public static void ˤ(){ˤ(κ.Ρ());}public static void Λ(){ż N=new ż(new Vector3D(1,1,1),Vector3D.Zero,Vector3D.
Zero);Ƃ Æ=new Ƃ(N,MAX_SPEED,Ƃ.ƅ.Ǝ);ˤ(Æ);}public static void ˤ(Ŵ Ɓ){if(Ɓ==null){return;}if(!Ţ.ť()){return;}Ǳ();û.é(
"Navigating to "+"["+Ɓ.Ʃ+"] "+Ɓ.ƫ);Ȯ.Add(Ɓ);Κ(Ɓ);Ά=true;Ǻ.ȭ(Ɓ);Ŕ.Ŝ(Ŕ.ŕ.Ř);}public static void ˤ(Ƃ Ȝ){if(!Ţ.ť()){return;}if(Ȝ.ƃ.ź==
Vector3D.Zero){û.Ñ("Invalid GPS format!");return;}Ǳ();û.é("Navigating to coordinates");ȑ.ǰ(Ȝ);Ά=true;Ǻ.ȭ();}public static void Ǳ
(){ȑ.Ǳ();Ȯ.Clear();Ά=false;Ǻ.ȯ();}public static void ˬ(){if(Ά){Ǳ();return;}ˤ();}}IMyBroadcastListener ˮ;
IMyBroadcastListener Ͱ;IMyBroadcastListener ͱ;IMyBroadcastListener Ͳ;bool ͳ=false;ƙ ʹ=new ƙ();Program(){try{if(this.Ͷ()){û.é(
"Loaded previous session");}}catch(Exception exception){û.Ð("Unable to load previous session: "+exception.Message);Storage="";}Runtime.
UpdateFrequency=UpdateFrequency.Update100|UpdateFrequency.Update10|UpdateFrequency.Once;ˮ=IGC.RegisterBroadcastListener(TAG);ˮ.
SetMessageCallback(TAG);Ͱ=IGC.RegisterBroadcastListener(CMD_TAG);Ͱ.SetMessageCallback(CMD_TAG);ͱ=IGC.RegisterBroadcastListener(CMD_RES_TAG
);ͱ.SetMessageCallback(CMD_RES_TAG);Ͳ=IGC.RegisterBroadcastListener(LEADER_TAG);Ͳ.SetMessageCallback(LEADER_TAG);}bool Ͷ(
){if(Storage.Length!=0){û.é("Loading session size: "+Storage.Length);if(ͷ.Ͷ(Storage)){return true;}û.Ð(
"Unable to Load previous session due to different version");}return false;}void Save(){try{string è=ͳ?"":ͷ.ͺ();û.é("Saving session size: "+è.Length);Storage=è;}catch(Exception e)
{û.é("Failed to save: "+e.Message);}}static class ͷ{public static string ͺ(){try{ã.ç();ã.h(STORAGE_VERSION);ã.h(κ.λ);ã.h(
κ.Ψ);ã.h(β.Ȯ);}catch(Exception e){û.é("Error1");throw e;}try{List<int>J=new List<int>();foreach(Ŵ Ɓ in κ.Ν){J.Add(κ.Ψ.
IndexOf(Ɓ));}ã.h(J);ã.h(ʀ.ʁ);ã.h(ʀ.Ɍ);ã.h(κ.μ);ã.h(κ.ο);ã.h(κ.ν);}catch(Exception e){û.é("Error2");throw e;}try{ã.h(κ.Ϊ);ã.h(β.
Ά);ã.h(ȑ.Ȓ);ã.h(Ǻ.Ö);if(Ǻ.Ö){ã.h(Ǻ.ǻ);}ã.h(Ǻ.ǯ);}catch(Exception e){û.é("Error3");throw e;}return ã.å;}public static bool
Ͷ(string è){ã.ǅ(è);if(STORAGE_VERSION!=ã.ǆ()){return false;}κ.λ=ã.Ǉ();κ.Ψ=ã.ǥ();β.Ȯ=ã.ǥ();List<int>J=ã.Ǣ();κ.Ν.Clear();
foreach(int Ƹ in J){κ.Ν.Add(κ.Ψ[Ƹ]);}κ.Ξ.Clear();foreach(Ŵ Ɓ in κ.Ψ){if(Ɓ.ƨ==0){continue;}κ.Ξ[Ɓ.ƪ]=Ɓ;if(β.Ȯ.Count!=0&&β.Ȯ[0].ƪ
==Ɓ.ƪ){β.Ȯ.Clear();β.Ȯ.Add(Ɓ);}}ʀ.ʁ=ã.ǉ();ʀ.Ɍ=ã.ǋ();κ.μ=ã.Ǉ();κ.ο=ã.Ǉ();κ.ν=ã.Ǉ();κ.Ϊ=ã.Ǉ();β.Ά=ã.ǋ();ȑ.Ȓ=ã.Ǩ();if(ã.æ.
Count==0){return true;}Ǻ.Ö=ã.ǋ();if(Ǻ.Ö){Ǻ.ǻ=ã.Ǥ();}Ǻ.ǯ=ã.Ǡ();return true;}}void ͻ(ref string ª){string[]ǘ=ª.Trim().Split(' '
);ǘ.DefaultIfEmpty("");string ͼ=ǘ.ElementAtOrDefault(0).ToUpper();string ˣ=ǘ.ElementAtOrDefault(1);ˣ=ˣ??"";try{switch(ͼ){
case"PREV":G.ë(G.W.X);break;case"NEXT":G.ë(G.W.Y);break;case"SELECT":G.ë(G.W.Z);break;case"ADD":switch(ˣ.ToUpper()){case
"STANCE":G.ë(G.W.Ï,String.Join(" ",ǘ.Skip(2).ToArray()));break;case"ORBIT":G.ë(G.W.ê);break;default:G.ë(G.W.A,String.Join(" ",ǘ.
Skip(1).ToArray()));break;}break;case"REMOVE":G.ë(G.W.Î);break;case"SCREEN":G.V();break;case"FOLLOW":β.Λ();break;case"START"
:switch(ˣ.ToUpper()){case"PREV":Ŕ.ð();κ.ʜ(G.W.X);β.ˤ();break;case"NEXT":Ŕ.ð();κ.ʜ(G.W.Y);β.ˤ();break;case"":Ŕ.ð();β.ˤ();
break;default:Ŕ.ð();β.ˤ(Ƃ.ŧ(String.Join(" ",ǘ.Skip(1).ToArray())));break;}break;case"GO":switch(ˣ.ToUpper()){case"":û.Ñ(
"There is no where to GO.");break;default:Ŕ.ð();β.ˤ(κ.Ο(String.Join(" ",ǘ.Skip(1).ToArray())));break;}break;case"TOGGLE":β.ˬ();Ŕ.ð();break;case
"STOP":β.Ǳ();Ŕ.ð();break;case"SAVE":Save();break;case"LOAD":Ͷ();break;case"CLEARLOG":û.ð();break;case"CLEARSTORAGE":ͳ=true;
break;case"TEST":Ŕ.Ŝ(Ŕ.ŕ.Ŗ);break;default:û.Ñ("Unknown command ->"+ͼ+"<-");break;}}catch(Exception exception){û.Ñ(
"Command exception -< "+ª+" >-< "+exception.Message+" >-");}}static class ͽ{public delegate void Μ(ref string g);public static void Ϋ(ref
UpdateType ά,UpdateType έ,Μ ή,ref string ί){if((ά&έ)==0){return;}ή(ref ί);ά&=~έ;}public static void ΰ(ref UpdateType ά,Μ ή,ref
string ί){Ϋ(ref ά,ά,ή,ref ί);}public static void α(Program Æ){try{Æ.Echo("SAMv"+VERSION+"\n"+G.U()+"\n"+Æ.ʹ.Ƣ());}catch(
Exception e){Æ.Echo(e.Message);}}}void Main(string ί,UpdateType γ){var ξ=γ;try{ͽ.Ϋ(ref γ,UpdateType.Once,this.δ,ref ί);ͽ.Ϋ(ref γ,
UpdateType.Update100,this.ι,ref ί);ͽ.Ϋ(ref γ,UpdateType.IGC,this.η,ref ί);ͽ.Ϋ(ref γ,UpdateType.Update10,this.θ,ref ί);ͽ.ΰ(ref γ,
this.ͻ,ref ί);ʹ.Ɵ((float)Runtime.CurrentInstructionCount/(float)Runtime.MaxInstructionCount,ξ);}catch(Exception exception){û
.Ñ("Main exception: "+exception.Message);Echo("Main exception: "+exception.Message);}}void δ(ref string ε){try{this.Œ();}
catch(Exception exception){û.Ñ("Once ScanGrid exception: "+exception.Message);}}MyIGCMessage ζ;void η(ref string g){if(g==TAG
){while(ˮ.HasPendingMessage){ζ=ˮ.AcceptMessage();try{ʙ.ʏ((string)ζ.Data);}catch(Exception exception){û.Ñ(
"Antenna Docks.Listen exception: "+exception.Message);}}}else if(g==LEADER_TAG){while(Ͳ.HasPendingMessage){ζ=Ͳ.AcceptMessage();try{ǳ.Ǹ((string)ζ.Data);}
catch(Exception exception){û.Ñ("Antenna Follower.ProcessLeaderMsg exception: "+exception.Message);}}}else if(g==CMD_TAG){
while(Ͱ.HasPendingMessage){ζ=Ͱ.AcceptMessage();try{Ǻ.ȸ(this,(string)ζ.Data);}catch(Exception exception){û.Ñ(
"Antenna Commander.ProcessCmd exception: "+exception.Message);}}}else if(g==CMD_RES_TAG){while(ͱ.HasPendingMessage){ζ=ͱ.AcceptMessage();try{Ě.Í((string)ζ.Data);}
catch(Exception exception){û.Ñ("Antenna Terminal.ProcessResponse exception: "+exception.Message);}}}}void θ(ref string ε){try
{G.Q();}catch(Exception exception){û.Ñ("Update10 Pannels.Print exception: "+exception.Message);}try{Û.ß();}catch(
Exception exception){û.Ñ("Update10 Animation.Run exception: "+exception.Message);}try{β.Å();}catch(Exception exception){û.Ñ(
"Update10 Pilot.Tick exception: "+exception.Message);}if(Me.CustomName.Contains("DEBUG")){this.Ù();}ͽ.α(this);}void ι(ref string ε){try{this.Œ();}catch(
Exception exception){û.Ñ("Update100 ScanGrid exception: "+exception.Message);}try{ʙ.ǹ(this);}catch(Exception exception){û.Ñ(
"Update100 Docks.Advertise exception: "+exception.Message);}try{ǳ.ǹ(this);}catch(Exception exception){û.Ñ("Update100 Leader.Advertise exception: "+exception.
Message);}try{Ť.Ĺ();}catch(Exception exception){û.Ñ("Update100 ConnectorControl.CheckConnect exception: "+exception.Message);}
try{this.Š();}catch(Exception exception){û.Ñ("Update100 SendSignals exception: "+exception.Message);}try{Ǻ.Å();}catch(
Exception exception){û.Ñ("Update100 Commander.Tick exception: "+exception.Message);}try{Ě.Å(this);}catch(Exception exception){û.Ñ
("Update100 Terminal.Tick exception: "+exception.Message);}}static class κ{public static int λ=0;public static int μ=0,ν=
0;public static int ο=0,Ϊ=0;public static List<Ŵ>Ν=new List<Ŵ>();public static List<Ŵ>Ψ=new List<Ŵ>();public static
Dictionary<long,Ŵ>Ξ=new Dictionary<long,Ŵ>();public static Ŵ Ο(long Ĝ){for(int Ƹ=0;Ƹ<Ψ.Count;Ƹ++){if(Ψ[Ƹ].ƪ==Ĝ){return Ψ[Ƹ];}}
return null;}public static Ŵ Ο(string Π){for(int Ƹ=0;Ƹ<Ψ.Count;Ƹ++){if(Ψ[Ƹ].ƫ==Π){return Ψ[Ƹ];}}throw new Exception(
"Connector ->"+Π+"<- was not found.");}public static Ŵ Ρ(){if(Ν.Count==0){return null;}return Ν[μ];}private static void Σ(){if(μ<0||μ
>=Ν.Count){return;}Ν[μ].Ʒ();}private static void Τ(){if(ο<0||ο>=Ψ.Count){return;}Ȯ=Ψ[ο];if(Ν.Contains(Ȯ)){Ν.Remove(Ȯ);}
else{Ν.Add(Ȯ);}ʛ();}private static void ê(){if(Ţ.đ==null){û.Ñ("No Remote Control");return;}Vector3D Υ=Ţ.đ.GetNaturalGravity(
);if(Υ==Vector3D.Zero){û.Ñ("No Gravity detected");return;}Vector3D Ô=Ţ.đ.CenterOfMass;Vector3D Ž=Ţ.đ.WorldMatrix.Forward;
Vector3D ž=Ţ.đ.WorldMatrix.Up;Vector3D Ζ=(-45000.0*Vector3D.Normalize(Υ))+Ô+(1000.0*Ž);string Π="Orbit";Ŵ Ɓ=Ŵ.Ŷ(Ζ,Ž,ž,Π);û.Ò(
"Orbit",ref Ζ);Ψ.Add(Ɓ);Ψ.Sort();ʛ();}private static void Φ(bool ƃ,string í){if(Ţ.đ==null){û.Ñ("No Remote Control");return;}
Vector3D Ô=Ţ.đ.CenterOfMass;Vector3D Ž=ƃ?Ţ.đ.WorldMatrix.Forward:Vector3D.Zero;Vector3D ž=ƃ?Ţ.đ.WorldMatrix.Up:Vector3D.Zero;
string Π=ǔ.Ǖ(ƃ,λ);λ++;bool Χ=false;IMyShipConnector Ľ=Ť.ļ();if(Ľ!=null){ž=Ľ.WorldMatrix.Up;Ô=Ľ.GetPosition();Ž=Vector3D.
Normalize(Ľ.OtherConnector.GetPosition()-Ľ.GetPosition());Π=Ľ.CustomName.Trim();}if(í!=""){string Ω;ǖ F=new ǖ(í);if(F.Ǘ){Ľ=null;Ô
=F.Ô;Ω=F.ö;Χ=true;}else{Ω=í;}Ω=Ω.Trim();if(Ω==""){û.Ñ("Invalid Dock name");}else{Π=Ω;}}Ŵ Ɓ=Ŵ.Ŷ(Ô,Ž,ž,Π);if(Χ){û.é(
"Added new GPS location: "+Π);}else if(í!=""){û.é("Added current GPS location: "+Π);}else{û.é("New connected Dock: "+Π);}if(Ľ!=null){Ɓ.ƪ=Ľ.
EntityId;Ɓ.Ƨ=Ľ.CubeGrid.GridSizeEnum;Ɓ.ƨ=Ľ.CubeGrid.EntityId;Ɓ.Ʃ=Ľ.CubeGrid.CustomName;Ɓ.ƿ();}Ψ.Add(Ɓ);Ψ.Sort();ʛ();}public
static Ŵ ʘ(string ö,Vector3D Ô){Ŵ Ɓ=Ŵ.Ŷ(Ô,Vector3D.Zero,Vector3D.Zero,ö);Ψ.Add(Ɓ);Ψ.Sort();ʛ();return Ɓ;}private static void ʚ
(){if(ο<0||ο>=Ψ.Count){return;}Ȯ=Ψ[ο];if(Ȯ.ƨ!=0&&Ȯ.ǁ()){return;}Ν.Remove(Ȯ);Ψ.Remove(Ȯ);Ξ.Remove(Ȯ.ƪ);ʛ();}public static
void ʛ(){if(μ<0){μ=Ν.Count-1;}if(μ>=Ν.Count){μ=0;}if(μ<ν){ν=μ;}if(μ>=ν+ʡ){ν=μ-ʡ+1;}if(ο<0){ο=Ψ.Count-1;}if(ο>=Ψ.Count){ο=0;}
if(ο<Ϊ){Ϊ=ο;}if(ο>=Ϊ+ʤ){Ϊ=ο-ʤ+1;}}public static void ʜ(G.W ì){ʜ(ì,"");}public static void ʜ(G.W ì,string í){switch(ì){case
G.W.X:--μ;break;case G.W.Y:++μ;break;case G.W.Z:Σ();break;case G.W.A:Φ(false,í);break;case G.W.Ï:Φ(true,í);break;case G.W
.Î:break;}ʛ();}public static void ʝ(G.W ì){ʝ(ì,"");}public static void ʝ(G.W ì,string í){switch(ì){case G.W.X:--ο;break;
case G.W.Y:++ο;break;case G.W.Z:Τ();break;case G.W.A:Φ(false,í);break;case G.W.ê:ê();break;case G.W.Ï:Φ(true,í);break;case G
.W.Î:ʚ();break;}ʛ();}private static Ŵ Ȯ;private static string è,ʞ,ʟ;private static int ʠ;private static int ʡ=11;public
static string ʢ(){è=Û.à()+" SAMv2 ";ʞ=ȑ.ȓ();if(ʞ!=""){if(Ǻ.ǻ!=null){return è+ʞ+"\n   to ["+Ǻ.ǻ.Ʃ+"] "+Ǻ.ǻ.ƫ;}return è+ʞ+
"\n   to GPS marker";}if(Ǻ.Ö){return è+"waiting...";}if(!β.Ά){return è+"disabled";}return è+"";}public static string ʣ(bool Ö){è=ñ.õ(
"Navigation",Ö);ʞ=ȑ.ȓ();ʟ=Ǻ.Ö?"waiting...":"disabled";è+="   "+((ʞ==""&&!β.Ά)?ʟ:ʞ)+"\n";if(Ν.Count()==0){return è+
"\n - No docks selected.\n   Use Configuration\n   screen to select\n   them.";}è+=(ν>0)?"     /\\/\\/\\\n":"     ------\n";for(int Ƹ=0;Ƹ<Ν.Count;++Ƹ){if(Ƹ<ν||Ƹ>=ν+ʡ){continue;}Ȯ=Ν[Ƹ];è+=((μ==Ƹ)?
" >":"  ")+(Ȯ.ǁ()?"":"? ");if(Ȯ.ƶ!=Ŵ.ƭ.Ʈ){è+="{"+Ȯ.ƹ()+"}";}è+="["+Ȯ.Ʃ+"] "+Ȯ.ƫ+"\n";}è+=(ν+ʡ<Ν.Count)?"     \\/\\/\\/\n":
"     ------\n";return è;}private static int ʤ=12;public static string ʥ(bool Ö){è=ñ.õ("Configuration",Ö);if(Ψ.Count()==0){return è+
"\n - No available docks\n   to configure.";}è+=(Ϊ>0)?"     /\\/\\/\\\n":"     ------\n";for(int Ƹ=0;Ƹ<Ψ.Count;Ƹ++){if(Ƹ<Ϊ||Ƹ>=Ϊ+ʤ){continue;}Ȯ=Ψ[Ƹ];ʠ=Ν.IndexOf(Ȯ)
+1;è+=(ʠ!=0)?ʠ.ToString().PadLeft(2,' '):"  ";è+=((ο==Ƹ)?" >":"  ");è+=(Ȯ.ǁ()?" ":" ? ")+"["+Ȯ.Ʃ+"] "+Ȯ.ƫ+"\n";}è+=(Ϊ+ʤ<Ψ
.Count)?"     \\/\\/\\/\n":"     ------\n";return è;}}static class ʙ{private static string ʗ;private static string ʉ;
private static List<ſ>Ʀ=new List<ſ>();private static string ʊ(){ã.ç();ã.h(ADVERT_ID);if(!ʲ.ǧ(ğ.Ġ.EntityId,"Name",ref ʗ)){ʗ=ğ.Ġ.
CubeGrid.CustomName;}ã.h(ğ.Ġ.CubeGrid.EntityId);ã.h(ʗ);ã.h(ğ.Ġ.CubeGrid.GridSizeEnum);ã.h(ğ.Ĩ.Count());foreach(IMyShipConnector
Ľ in ğ.Ĩ){if(ʲ.ʾ(Ľ.EntityId,"MAIN")){continue;}ã.h(Ľ.EntityId);if(!ʲ.ǧ(Ľ.EntityId,"Name",ref ʗ)){ʗ=Ľ.CustomName.Trim();}ã
.h(ʗ);ã.h(Ľ.GetPosition());if(ʲ.ʾ(Ľ.EntityId,"REV")){ã.h(Ľ.WorldMatrix.Backward);}else{ã.h(Ľ.WorldMatrix.Forward);}ã.h(Ľ.
WorldMatrix.Up);Ʀ.Clear();foreach(IMyTextPanel R in ğ.Ī){if(!ʲ.ǧ(R.EntityId,"Name",ref ʉ)){continue;}if(ʉ!=ʗ){continue;}Ʀ.Add(new ſ
(R.GetPosition(),-R.WorldMatrix.Forward));}ã.h(Ʀ);}return ã.å;}public static void ǹ(Program Æ){if(!ʲ.ʾ(ğ.Ġ.EntityId,
"ADVERTISE")){return;}if(ğ.Ĩ.Count()==0||ğ.Ĩ.Count()==ğ.ĩ.Count()){return;}ʊ();Æ.IGC.SendBroadcastMessage<string>(TAG,ã.å);}private
static long ƨ;private static long ƪ;private static string Ʃ;private static VRage.Game.MyCubeSize ʋ;private static int ǡ;
private static Ŵ Ȯ;private static bool ʌ;private static string ʍ;private static bool ʎ;public static void ʏ(string ʐ){ã.ǅ(ʐ);ʍ=
ã.ǆ();if(ʍ==ADVERT_ID_VER){ʎ=true;}if(!ʎ&&ʍ!=ADVERT_ID){return;}if(ʎ){ã.ǈ();}ƨ=ã.ǈ();Ʃ=ã.ǆ();if(!ʎ){ʋ=ã.ǌ();}ǡ=ã.Ǉ();for(
int Ƹ=0;Ƹ<ǡ;++Ƹ){ƪ=ã.ǈ();if(κ.Ξ.ContainsKey(ƪ)){Ȯ=κ.Ξ[ƪ];ʌ=true;}else{Ȯ=new Ŵ();ʌ=false;}Ȯ.ƿ();Ȯ.ƫ=ã.ǆ();Ȯ.ƃ=new ż(ã.ǂ(),ã.
ǂ(),ã.ǂ());Ȯ.Ƭ=DateTime.Now.Ticks;Ȯ.ƪ=ƪ;Ȯ.ƨ=ƨ;Ȯ.Ʃ=Ʃ;if(ʎ){Ȯ.Ƨ=ã.ǌ();}else{Ȯ.Ƨ=ʋ;}Ȯ.Ʀ=ã.Ǫ();Ȯ.Ƽ(Ȯ.ƃ.ź);if(!ʌ){κ.Ξ[ƪ]=Ȯ;κ.Ψ
.Add(Ȯ);κ.Ψ.Sort();}}}}static class ʑ{public static System.Text.RegularExpressions.Regex ʒ=new System.Text.
RegularExpressions.Regex("\\s*"+TAG+"\\.([a-zA-Z0-9]*)([:=]{1}([\\S]*))?",System.Text.RegularExpressions.RegexOptions.IgnoreCase);private
static System.Text.RegularExpressions.Match ȳ;private static char[]ʓ=new char[]{'\n'};private static char[]ʔ=new char[]{':',
'='};private static string[]B;private static string ʕ;private static string ʖ;private static string ĝ;private static string
ʦ;private static bool ʧ;private static long Ĝ;private static bool ˀ;private static string ˁ;public static bool ˆ(ref
IMyTerminalBlock đ,ref Ů ʵ){B=đ.CustomData.Split(ʓ);ʦ="";ʧ=false;ˀ=false;Ĝ=đ.EntityId;foreach(string q in B){ˁ=q.Trim();if(ˁ==""){
continue;}ȳ=ʒ.Match(ˁ);ˀ=ȳ.Success||ˀ;if(ȳ.Groups.Count==4){if(ȳ.Groups[1].Value!=""){if(ȳ.Groups[3].Value!=""){ʖ=ʵ.Ų(ȳ.Groups[1
].Value);if(ʖ!=""){ĝ=ȳ.Groups[3].Value;ʦ+=TAG+"."+ʖ+"="+ĝ+"\n";ʲ.ʹ(Ĝ,ʖ,ĝ);continue;}}else{ʕ=ȳ.Groups[1].Value.ToUpper();
if(ʵ.Ű.Contains(ʕ)){if(ʧ){ʦ+=ˁ+"\n";continue;}ʧ=true;}else if(!ʵ.ů.Contains(ʕ)){ʦ+=ˁ+"\n";continue;}ʲ.ʹ(Ĝ,ʕ,"");ʦ+=TAG+"."
+ʕ+"\n";continue;}}ʦ+=TAG+".\n";continue;}else{ʦ+=ˁ+"\n";}}if(ˀ){đ.CustomData=ʦ;}return ˀ;}}static class ˇ{private static
char[]ʔ=new char[]{':','='};private static System.Text.RegularExpressions.Regex ˈ=new System.Text.RegularExpressions.Regex(
"\\[("+TAG+"[\\s\\S]*)\\]",System.Text.RegularExpressions.RegexOptions.IgnoreCase);private static string ˉ=TAG+"\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*(\\S*)\\s*"
;private static System.Text.RegularExpressions.Regex ˊ=new System.Text.RegularExpressions.Regex(ˉ,System.Text.
RegularExpressions.RegexOptions.IgnoreCase);private static System.Text.RegularExpressions.Match ˋ;private static System.Text.
RegularExpressions.Match ȳ;private static string ʦ;private static string ˌ;private static string ˍ;private static bool ˎ;private static
string[]ˏ;private static string ʖ;private static long Ĝ;public static bool ˆ(ref IMyTerminalBlock đ,ref Ů ʵ){ˋ=ˈ.Match(đ.
CustomName);if(!ˋ.Success){return false;}ȳ=ˊ.Match(ˋ.Groups[1].Value);if(!ȳ.Success){return false;}Ĝ=đ.EntityId;ˎ=false;ʦ="["+TAG;
for(int Ƹ=1;Ƹ<ȳ.Groups.Count;++Ƹ){ˌ=ȳ.Groups[Ƹ].Value;if(ˌ==""){break;}ˍ=ˌ.ToUpper();if(ʵ.Ű.Contains(ˍ)){if(ˎ){continue;}ˎ=
true;ʦ+=" "+ˍ;ʲ.ʹ(Ĝ,ˍ,"");continue;}if(ʵ.ů.Contains(ˍ)){ʦ+=" "+ˍ;ʲ.ʹ(Ĝ,ˍ,"");continue;}ˏ=ˌ.Split(ʔ);if(ˏ.Count()>1){ʖ=ʵ.Ų(ˏ[
0]);if(ʖ!=""){ʲ.ʹ(Ĝ,ʖ,ˏ[1]);ʦ+=" "+ʖ+"="+ˏ[1];continue;}ʦ+=" "+ʖ.ToLower()+"="+ˏ[1];continue;}ʦ+=" "+ˌ.ToLower();}ʦ+="]";
đ.CustomName=đ.CustomName.Replace(ˋ.Groups[0].Value,ʦ);return true;}}static class ː{private static string[]ˑ=new string[]
{};private static string[]ˠ=new string[]{"IGNORE"};private static string[]ˢ=new string[]{"Name"};private static string[]ˡ
=new string[]{"DEBUG","ADVERTISE","NODAMPENERS","IGNOREGRAVITY","LEADER"};private static string[]ʿ=new string[]{"LIST",
"LOOP"};private static string[]ʽ=new string[]{"Follow","FollowFront","FollowUp","FollowRight","Name","Speed","Wait",
"TaxiingDistance","ApproachDistance","DockDistance","UndockDistance","DockingSpeed","TaxiingSpeed","MaxSpeed","Aggro","ConvergingSpeed",
"MassExcess","TaxiingPanelDistance","ApproachingSpeed","EffectiveThrust"};private static string[]ʨ=new string[]{"OVR"};private
static string[]ʩ=new string[]{"LOG","NAV","CONF","DATA","STAT"};private static string[]ʪ=new string[]{"OVR"};public static
string[]ʫ=new string[]{"Panel0","Panel1","Panel2","Panel3","Panel4","Panel5","Panel6","Panel7","Panel8","Panel9"};private
static string[]ʬ=new string[]{"REV","MAIN"};private static string[]ʭ=new string[]{"DOCKED","NAVIGATED"};public static string[]
ʮ=new string[]{"Full","Empty"};private static string[]ʯ=new string[]{"FORCE"};private static string[]ʰ=new string[]{
"FORCE","CARGO"};public static Ů ė=new Ů(ref ˡ,ref ʿ,ref ʽ);public static Dictionary<Type,Ů>ʱ=new Dictionary<Type,Ů>{{typeof(
IMyProgrammableBlock),ė},{typeof(IMyRemoteControl),new Ů(ref ˑ,ref ˑ,ref ˑ)},{typeof(IMyCameraBlock),new Ů(ref ˑ,ref ˑ,ref ˑ)},{typeof(
IMyRadioAntenna),new Ů(ref ˑ,ref ˑ,ref ˑ)},{typeof(IMyLaserAntenna),new Ů(ref ˑ,ref ˑ,ref ˑ)},{typeof(IMyShipConnector),new Ů(ref ʬ,ref
ˑ,ref ˢ)},{typeof(IMyTextPanel),new Ů(ref ʨ,ref ʩ,ref ˢ)},{typeof(IMyCockpit),new Ů(ref ʪ,ref ˑ,ref ʫ)},{typeof(
IMyTimerBlock),new Ů(ref ʭ,ref ˑ,ref ˑ)},{typeof(IMyBatteryBlock),new Ů(ref ʯ,ref ˑ,ref ʮ)},{typeof(IMyGasTank),new Ů(ref ʰ,ref ˑ,ref
ʮ)},{typeof(IMyCargoContainer),new Ů(ref ˑ,ref ˑ,ref ʮ)},{typeof(IMyThrust),new Ů(ref ˠ,ref ˑ,ref ˑ)},};}static class ʲ{
public static bool ʳ(ref IMyTerminalBlock đ,Type P){return ʴ(ref đ,ː.ʱ[P]);}public static bool ʴ(ref IMyTerminalBlock đ,Ů ʵ){
bool ʶ=ˇ.ˆ(ref đ,ref ʵ);bool ʷ=ʑ.ˆ(ref đ,ref ʵ);return ʶ||ʷ;}private static Dictionary<long,Dictionary<string,string>>ʸ=new
Dictionary<long,Dictionary<string,string>>();public static void ʹ(long Ĝ,string ʺ,string ĝ){if(ĝ==null){ĝ="";}if(ʸ.ContainsKey(Ĝ))
{ʸ[Ĝ][ʺ]=ĝ;}else{ʸ[Ĝ]=new Dictionary<string,string>{{ʺ,ĝ}};}}public static void ʻ(){foreach(KeyValuePair<long,Dictionary<
string,string>>ʼ in ʸ){ʼ.Value.Clear();}}public static bool ʾ(long Ĝ,string ö){if(!ʸ.ContainsKey(Ĝ)){return false;}if(!ʸ[Ĝ].
ContainsKey(ö)){return false;}return true;}public static bool ǧ(long Ĝ,string ö,ref string ĝ){if(!ʾ(Ĝ,ö)){return false;}ĝ=ʸ[Ĝ][ö];
return true;}public static void Ğ(long Ĝ,string ö){if(!ʸ.ContainsKey(Ĝ)){return;}ʸ[Ĝ].Remove(ö);}}static class ğ{public static
IMyProgrammableBlock Ġ;public static Dictionary<string,ũ>ġ=new Dictionary<string,ũ>();public static List<IMyTerminalBlock>Ģ=new List<
IMyTerminalBlock>();public static List<IMyRemoteControl>ģ=new List<IMyRemoteControl>();public static List<IMyCameraBlock>Ĥ=new List<
IMyCameraBlock>();public static List<IMyRadioAntenna>ĥ=new List<IMyRadioAntenna>();public static List<IMyLaserAntenna>Ħ=new List<
IMyLaserAntenna>();public static List<IMyProgrammableBlock>ħ=new List<IMyProgrammableBlock>();public static List<IMyShipConnector>Ĩ=new
List<IMyShipConnector>();public static List<IMyShipConnector>ĩ=new List<IMyShipConnector>();public static List<IMyTextPanel>
Ī=new List<IMyTextPanel>();public static List<IMyGyro>ī=new List<IMyGyro>();public static List<IMyThrust>Ĭ=new List<
IMyThrust>();public static List<IMyTimerBlock>ĭ=new List<IMyTimerBlock>();public static List<IMyCockpit>Į=new List<IMyCockpit>();
public static List<IMyBatteryBlock>į=new List<IMyBatteryBlock>();public static List<IMyCargoContainer>İ=new List<
IMyCargoContainer>();public static List<IMyGasTank>ı=new List<IMyGasTank>();public static List<IMyBatteryBlock>Ĳ=new List<IMyBatteryBlock
>();public static List<IMyGasTank>ĳ=new List<IMyGasTank>();public static IMyTerminalBlock ĵ;public static
IMyRemoteControl Ĵ;public static IMyCameraBlock ě;public static IMyRadioAntenna ę;public static IMyLaserAntenna ā;public static
IMyProgrammableBlock Ă;public static IMyShipConnector ă;public static IMyTextPanel Ą;public static IMyGyro ą;public static IMyThrust Ć;
public static IMyTimerBlock ć;public static IMyCockpit Ĉ;public static IMyBatteryBlock ĉ;public static IMyCargoContainer Ċ;
public static IMyGasTank ċ;public static void ð(){foreach(string Č in ġ.Keys){ġ[Č].Ŭ();}Ģ.Clear();ģ.Clear();Ĥ.Clear();ĥ.Clear(
);Ħ.Clear();ħ.Clear();Ĩ.Clear();ĩ.Clear();Ī.Clear();ī.Clear();Ĭ.Clear();ĭ.Clear();Į.Clear();į.Clear();İ.Clear();ı.Clear()
;}public static void č(string Č){if(ġ.ContainsKey(Č)){ġ[Č].ū++;}else{ġ[Č]=new ũ();}}private static int Ď;public static
void ď(){foreach(string Č in ġ.Keys){Ď=ġ[Č].ŭ();if(Ď>0){û.é(String.Format("Found {0}x {1}",Ď,Č));}else if(Ď<0){û.é(String.
Format("Lost {0}x {1}",-Ď,Č));}}}public static bool Đ(IMyTerminalBlock đ){if((Ĵ=đ as IMyRemoteControl)!=null){if(!ʲ.ʳ(ref đ,
typeof(IMyRemoteControl))){return false;}ģ.Add(Ĵ);}else if((ě=đ as IMyCameraBlock)!=null){if(!ʲ.ʳ(ref đ,typeof(IMyCameraBlock)
)){return false;}Ĥ.Add(ě);}else if((ę=đ as IMyRadioAntenna)!=null){ĥ.Add(ę);}else if((ā=đ as IMyLaserAntenna)!=null){Ħ.
Add(ā);}else if((Ă=đ as IMyProgrammableBlock)!=null){if(!ʲ.ʳ(ref đ,typeof(IMyProgrammableBlock))){return false;}ħ.Add(Ă);}
else if((ă=đ as IMyShipConnector)!=null){if(!ʲ.ʳ(ref đ,typeof(IMyShipConnector))){return false;}Ĩ.Add(ă);if(ʲ.ʾ(ă.EntityId,
"MAIN")){ĩ.Add(ă);ğ.č("MAIN "+đ.DefinitionDisplayNameText);}}else if((Ą=đ as IMyTextPanel)!=null){if(!ʲ.ʳ(ref đ,typeof(
IMyTextPanel))){return false;}Ī.Add(Ą);}else if((ą=đ as IMyGyro)!=null){ī.Add(ą);}else if((Ć=đ as IMyThrust)!=null){if(ʲ.ʳ(ref đ,
typeof(IMyThrust))){if(ʲ.ʾ(đ.EntityId,"IGNORE")){return false;}}Ĭ.Add(Ć);}else if((ć=đ as IMyTimerBlock)!=null){if(!ʲ.ʳ(ref đ,
typeof(IMyTimerBlock))){return false;}ĭ.Add(ć);}else if((Ĉ=đ as IMyCockpit)!=null){if(!ʲ.ʳ(ref đ,typeof(IMyCockpit))){return
false;}Į.Add(Ĉ);}else if((ĉ=đ as IMyBatteryBlock)!=null){if(!ʲ.ʳ(ref đ,typeof(IMyBatteryBlock))){return false;}į.Add(ĉ);}else
if((Ċ=đ as IMyCargoContainer)!=null){if(!ʲ.ʳ(ref đ,typeof(IMyCargoContainer))){return false;}İ.Add(Ċ);}else if((ċ=đ as
IMyGasTank)!=null){if(!ʲ.ʳ(ref đ,typeof(IMyGasTank))){return false;}ı.Add(ċ);}else{return false;}return true;}private static
string Ē;private static float ē;private static double Ĕ;private static bool ĕ;public static void Ė(IMyProgrammableBlock ė){Ġ=ė
;ĵ=ė as IMyTerminalBlock;if(!ʲ.ʴ(ref ĵ,ː.ė)){ė.CustomName+=" ["+TAG+"]";return;}if(ʲ.ǧ(ĵ.EntityId,"Speed",ref Ē)){if(
float.TryParse(Ē,out ē)){if(MAX_SPEED!=ē){MAX_SPEED=ē;û.é("Maximum speed changed to "+MAX_SPEED);}}}if(ʲ.ǧ(ĵ.EntityId,
"MaxSpeed",ref Ē)){if(float.TryParse(Ē,out ē)){if(MAX_SPEED!=ē){MAX_SPEED=ē;û.é("Maximum speed changed to "+MAX_SPEED);}}}if(ʲ.ǧ(ĵ
.EntityId,"DockingSpeed",ref Ē)){if(float.TryParse(Ē,out ē)){if(DOCKING_SPEED!=ē){DOCKING_SPEED=ē;û.é(
"Docking speed changed to "+DOCKING_SPEED);}}}if(ʲ.ǧ(ĵ.EntityId,"TaxiingSpeed",ref Ē)){if(float.TryParse(Ē,out ē)){if(TAXIING_SPEED!=ē){
TAXIING_SPEED=ē;û.é("Taxiing speed changed to "+TAXIING_SPEED);}}}if(ʲ.ǧ(ĵ.EntityId,"TaxiingDistance",ref Ē)){if(float.TryParse(Ē,out
ē)){if(TAXIING_DISTANCE!=(float)ē){TAXIING_DISTANCE=(float)ē;û.é("Taxiing distance changed to "+TAXIING_DISTANCE);}}}if(ʲ
.ǧ(ĵ.EntityId,"TaxiingPanelDistance",ref Ē)){if(float.TryParse(Ē,out ē)){if(TAXIING_PANEL_DISTANCE!=(float)ē){
TAXIING_PANEL_DISTANCE=(float)ē;û.é("Taxiing panel distance changed to "+TAXIING_PANEL_DISTANCE);}}}if(ʲ.ǧ(ĵ.EntityId,"ApproachDistance",ref Ē
)){if(float.TryParse(Ē,out ē)){if(APPROACH_DISTANCE!=(float)ē){APPROACH_DISTANCE=(float)ē;û.é(
"Approach distance changed to "+APPROACH_DISTANCE);}}}if(ʲ.ǧ(ĵ.EntityId,"ApproachingSpeed",ref Ē)){if(float.TryParse(Ē,out ē)){if(APPROACHING_SPEED!=ē)
{APPROACHING_SPEED=ē;û.é("Approaching speed changed to "+APPROACHING_SPEED);}}}if(ʲ.ǧ(ĵ.EntityId,"DockDistance",ref Ē)){
if(float.TryParse(Ē,out ē)){if(DOCK_DISTANCE!=(float)ē){DOCK_DISTANCE=(float)ē;û.é("Docking distance changed to "+
DOCK_DISTANCE);}}}if(ʲ.ǧ(ĵ.EntityId,"UndockDistance",ref Ē)){if(float.TryParse(Ē,out ē)){if(UNDOCK_DISTANCE!=(float)ē){
UNDOCK_DISTANCE=(float)ē;û.é("Undocking distance changed to "+UNDOCK_DISTANCE);}}}if(ʲ.ǧ(ĵ.EntityId,"Wait",ref Ē)){if(Double.TryParse(Ē
,out Ĕ)){Ǻ.Ȟ=TimeSpan.FromSeconds(Ĕ).Ticks;}}ĕ=ʲ.ʾ(ĵ.EntityId,"IGNOREGRAVITY");if(ĕ!=ɑ.ɒ){ɑ.ɒ=ĕ;û.é("Ship orientation "+(
ĕ?"ignoring":"using")+" gravity for alignment.");}if(ʲ.ʾ(ĵ.EntityId,"LIST")){Ǻ.ǯ=Ǻ.Ǽ.Ǿ;}else if(ʲ.ʾ(ĵ.EntityId,"LOOP")){Ǻ
.ǯ=Ǻ.Ǽ.ǿ;}else{Ǻ.ǯ=Ǻ.Ǽ.ǽ;Ǻ.Ö=false;}}private static int Ę,Ā;private static int Ķ(IMyThrust ķ,IMyThrust Ŏ){Ę=Ā=0;if(ķ.
DefinitionDisplayNameText.Contains("Hydrogen ")){Ę+=4;}else if(ķ.DefinitionDisplayNameText.Contains("Ion ")){Ę+=2;}if(ķ.DefinitionDisplayNameText
.Contains("Large ")){Ę+=1;}if(Ŏ.DefinitionDisplayNameText.Contains("Hydrogen ")){Ā+=4;}else if(Ŏ.
DefinitionDisplayNameText.Contains("Ion ")){Ā+=2;}if(Ŏ.DefinitionDisplayNameText.Contains("Large ")){Ā+=1;}return Ę-Ā;}public static void ŏ(){if(
ģ.Count()==1){Ţ.đ=ģ[0];Ǚ.Ä(Ǚ.ǚ.ǜ);Ǚ.Ä(Ǚ.ǚ.Ǜ);return;};Ţ.đ=null;if(!Ǚ.õ(Ǚ.ǚ.Ǜ)&&ģ.Count()>1){Ǚ.Ǟ(Ǚ.ǚ.Ǜ);û.Ñ(
"Too many remote controllers");}}public static void Ő(){foreach(IMyCameraBlock ě in Ĥ){if(!ě.EnableRaycast){ě.EnableRaycast=true;}}}public static
void ő(){Ĭ.Sort(Ķ);}}void Œ(){ʲ.ʻ();ğ.ð();ğ.Ė(Me);this.GridTerminalSystem.GetBlocks(ğ.Ģ);foreach(IMyTerminalBlock đ in ğ.Ģ){
if(!đ.IsSameConstructAs(Me)){continue;}if(đ.EntityId==Me.EntityId){continue;}if(ğ.Đ(đ)){ğ.č(đ.DefinitionDisplayNameText);}
}ğ.ő();ğ.Ő();ğ.ŏ();ğ.ď();try{var œ=this.GridTerminalSystem.GetBlockGroupWithName(CHARGE_TARGET_GROUP_NAME);if(œ!=null){œ.
GetBlocksOfType<IMyBatteryBlock>(ğ.Ĳ);œ.GetBlocksOfType<IMyGasTank>(ğ.ĳ);}}catch(Exception exception){û.Ñ(
"Update100 ScanGrid blockGroup exception: "+exception.Message);}}static class Ŕ{public enum ŕ{Ŗ,ŗ,Ř,ř};public static Dictionary<ŕ,int>Ö=new Dictionary<ŕ,int>();
private static HashSet<ŕ>Ś=new HashSet<ŕ>{};private static int ś=10;public static void Ŝ(ŕ ŝ){Ö[ŝ]=ś;}public static void Ş(){
foreach(KeyValuePair<ŕ,int>ş in Ö){Ś.Add(ş.Key);}foreach(ŕ ŝ in Ś){if(--Ö[ŝ]<1){Ö.Remove(ŝ);}}Ś.Clear();}public static void ð()
{Ö.Clear();}}void Š(){if(Ŕ.Ö.Count==0){return;}foreach(IMyTimerBlock š in ğ.ĭ){if(ʲ.ʾ(š.EntityId,"DOCKED")&&Ŕ.Ö.
ContainsKey(Ŕ.ŕ.Ŗ)){Ŕ.Ö[Ŕ.ŕ.Ŗ]=0;û.é("Timer triggered due to Docking accomplished");š.StartCountdown();}if(ʲ.ʾ(š.EntityId,
"UNDOCKED")&&Ŕ.Ö.ContainsKey(Ŕ.ŕ.ř)){Ŕ.Ö[Ŕ.ŕ.ř]=0;û.é("Timer triggered due to Undocking sequence");š.StartCountdown();}if(ʲ.ʾ(š.
EntityId,"NAVIGATED")&&Ŕ.Ö.ContainsKey(Ŕ.ŕ.ŗ)){Ŕ.Ö[Ŕ.ŕ.ŗ]=0;û.é("Timer triggered due to Navigation finished");š.StartCountdown()
;}if(ʲ.ʾ(š.EntityId,"STARTED")&&Ŕ.Ö.ContainsKey(Ŕ.ŕ.Ř)){Ŕ.Ö[Ŕ.ŕ.Ř]=0;û.é("Timer triggered due to Navigation started");š.
StartCountdown();}}Ŕ.Ş();}static class Ţ{public static IMyRemoteControl đ=null;public static bool ţ(){return đ!=null;}public static
bool ť(){if(ţ()){return true;}û.Ñ("No Remote Control!");return false;}}static class Ť{private static int ō=0;private static
List<IMyShipConnector>Ō=null;public static void ĸ(){ō=DOCK_ATTEMPTS;ĺ();}public static void Ĺ(){if(ō==0){return;}if(0==--ō){
û.é("Failed to dock!");}else{ĺ();}}private static void ĺ(){if(!ŀ()){return;}ō=0;û.é("Docking successful!");Ŕ.Ŝ(Ŕ.ŕ.Ŗ);if(
!ʲ.ʾ(ğ.Ġ.EntityId,"NODAMPENERS")){ȿ.ɀ(false);}}private static List<IMyShipConnector>Ļ(){if(ğ.ĩ.Count==0){return ğ.Ĩ;}
return ğ.ĩ;}public static IMyShipConnector ļ(){Ō=Ļ();foreach(IMyShipConnector Ľ in Ō){if(Ľ.Status==MyShipConnectorStatus.
Connected){return Ľ.OtherConnector;}}return null;}public static IMyShipConnector ľ(){Ō=Ļ();foreach(IMyShipConnector Ľ in Ō){if(Ľ.
Status==MyShipConnectorStatus.Connected){return Ľ;}}return null;}private static bool Ŀ;private static bool ŀ(){Ō=Ļ();Ŀ=false;
foreach(IMyShipConnector Ľ in Ō){Ľ.Connect();if(Ľ.Status==MyShipConnectorStatus.Connected){Ŀ=true;}}return Ŀ;}private static
Vector3D Ł;public static Vector3D ł(){Ō=Ļ();Ł=Vector3D.Zero;foreach(IMyShipConnector Ľ in Ō){if(Ľ.Status==MyShipConnectorStatus.
Connected){Ľ.Disconnect();Ł=-Ľ.WorldMatrix.Forward;}}return Ł;}private static Ŵ Ń;private static Ŵ ń;public static Ŵ Ņ(){Ō=Ļ();Ń=
null;foreach(IMyShipConnector Ľ in Ō){if(Ľ.Status==MyShipConnectorStatus.Connected){ń=κ.Ο(Ľ.OtherConnector.EntityId);if(ń!=
null){Ń=ń;}else{Ń=Ŵ.Ŷ(Ľ.OtherConnector.GetPosition(),Ľ.OtherConnector.WorldMatrix.Forward,Ľ.OtherConnector.WorldMatrix.Up,
"D");}Ŕ.Ŝ(Ŕ.ŕ.ř);Ľ.Disconnect();}}return Ń;}private static bool ņ;public static IMyShipConnector Ň(Ŵ ň){Ō=Ļ();if(Math.Abs(
Vector3D.Dot(ň.ƃ.Ž,Ţ.đ.WorldMatrix.Up))<0.5f){foreach(IMyShipConnector Ľ in Ō){ņ=ʲ.ʾ(Ľ.EntityId,"REV");if(Math.Abs(Vector3D.Dot(
ņ?Ľ.WorldMatrix.Backward:Ľ.WorldMatrix.Forward,Ţ.đ.WorldMatrix.Up))<0.5f){return Ľ;}}}else{foreach(IMyShipConnector Ľ in
Ō){ņ=ʲ.ʾ(Ľ.EntityId,"REV");if(Vector3D.Dot(ņ?Ľ.WorldMatrix.Backward:Ľ.WorldMatrix.Forward,-ň.ƃ.Ž)>0.5f){return Ľ;}}}
foreach(IMyShipConnector Ľ in Ō){return Ľ;}return null;}}static class ŉ{public static void Ŋ(long ŋ,IMyTextSurface T){if(ʲ.ʾ(ŋ,
"OVR")){return;}if(ʲ.ʾ(ŋ,"STAT")){T.FontSize=2.5f;T.Font="Monospace";T.TextPadding=0.0f;return;}T.Font="Monospace";T.
TextPadding=0.0f;if(T.SurfaceSize.Y<512){T.FontSize=0.6f;return;}T.FontSize=1.180f;}}static class Ě{public static List<string>ÿ=new
List<string>{"step","run","loop","step conf","run conf","loop conf","start","stop"};private static string e="SAMv2 cmd# ";
private static System.Text.RegularExpressions.Regex j=new System.Text.RegularExpressions.Regex("^"+e+"\\s*([\\S ]+)\\s*$");
private static System.Text.RegularExpressions.Regex k=new System.Text.RegularExpressions.Regex(
"^(\\{(\\S+)\\}){0,1}(\\[(\\S+)\\]){0,1}(\\S+)$");private static System.Text.RegularExpressions.Regex n=new System.Text.RegularExpressions.Regex(
@"^(\{(\S+)\}){0,1}(GPS:[\S\s]+)$");private static IMyTextSurface L;private static IMyTextSurface o;private static string q;private static string[]r,t,z;
private static int ª;private static List<string>µ=new List<string>{};private static System.Text.RegularExpressions.Match º,À,Á;
private static string Â="SAMv2 "+VERSION+" Terminal\n please write your command in the keyboard.";private static string Ã=Â;
private static System.Text.StringBuilder I=new System.Text.StringBuilder();public static void Ä(){L.ContentType=VRage.Game.GUI.
TextPanel.ContentType.TEXT_AND_IMAGE;L.WriteText(Ã);L.FontSize=0.6f;L.Font="Monospace";L.FontColor=Color.Green;L.BackgroundColor=
Color.Black;L.TextPadding=0.0f;o.ContentType=VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;o.WriteText(e);o.FontSize=
2.0f;o.Font="Monospace";o.FontColor=Color.Green;o.BackgroundColor=Color.Black;o.TextPadding=0.0f;}public static void Å(
Program Æ){if(ğ.Ġ==null){return;}L=ğ.Ġ.GetSurface(0);o=ğ.Ġ.GetSurface(1);if(L==null||o==null){return;}if(o.ContentType!=VRage.
Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE){Ä();return;}I.Clear();o.ReadText(I);t=I.ToString().Split(';');ª=-2;foreach(
string Ç in t){r=Ç.Split('\n');if(r.Length==0){Ä();return;}if(r[0]==e){return;}µ.Clear();foreach(string È in r){q=È.Trim();if(
q!=""){µ.Add(q);}}r=µ.ToArray();if(ª==-2){º=j.Match(r[0]);if(!º.Success){Ã="Invalid command. Please try again.";Ä();
return;}ª=ÿ.FindIndex(É=>É.ToLower()==º.Groups[1].Value);}else{z=new string[r.Length+1];z[0]="";Array.Copy(r,0,z,1,r.Length);r
=z;}if(ª==-1){Ã="Invalid command: "+º.Groups[1].Value+"\n\nAvailable commands are:\n "+string.Join("\n  ",ÿ);Ä();return;}
if(r.Length==1){Ã="Command must be followed by the ship name.\nExample:\n loop\n ShipName";Ä();return;}Ê.Ɨ=r[1];if(c(r.
Skip(2).ToArray())){Ì(Æ,ª);}}Ä();}private static ƕ Ê=new ƕ();private static Ŵ.ƭ Ë;private static void Ì(Program Æ,int r){Ê.Ɩ
=r;Ã=Ǻ.ȹ(Ê);if(Ã!=""){return;}ã.ç();ã.h(Ê);Æ.IGC.SendBroadcastMessage<string>(CMD_TAG,ã.å);Ã=
"Command sent.\n Will only be successful if acknowledged...";}public static void Í(string g){Ã=g;Ä();}public static bool c(string[]B){Ê.Ƙ.Clear();foreach(string C in B){À=k.Match(C
);Á=n.Match(C);if(!À.Success&&!À.Success){Ã="Invalid navigation format:\n"+C+
"\nUse:\n {Action}[Grid]DockName\nor:\n {Action}DockName\nor:\n DockName\nor {Action}GPS:...";return false;}var E=À.Groups[2].Value;if(Á.Success){E=Á.Groups[2].Value;}Ë=Ŵ.ƺ(E)|Ŵ.ƺ(E);if(Ë==Ŵ.ƭ.Ʈ&&E!=""){Ã=
"Invalid Action:\n"+E+"\n\nUse one of:\n Charge,Charge&Load,Charge&Unload,\n Load,Unload;";return false;}if(Á.Success){var F=new ǖ(Á.Groups
[3].Value);if(!F.Ǘ){Ã="Invalid GPS format;";return false;}Ê.Ƙ.Add(new ǀ(Ë,"","",F.ö,F.Ô));}else{Ê.Ƙ.Add(new ǀ(Ë,À.Groups[
4].Value,À.Groups[5].Value,"",Vector3D.Zero));}}return true;}}static class G{private static List<string>H=new List<string
>{"LOG","NAV","CONF","STAT","DATA"};private static Dictionary<string,string>I=new Dictionary<string,string>();private
static Queue<string>J=new Queue<string>(new List<string>{"NAV","CONF"});private static string K,L;private static void M(){
foreach(string N in H){I[N]="";}}private static void O(string P){K=I[P];if(K!=""){return;}L=J.Peek();switch(P){case"LOG":try{K=
û.Õ(L=="LOG");}catch(Exception exception){û.Ñ("PrintBuffer LOG exception: "+exception.Message);}break;case"CONF":try{K=κ.
ʥ(L=="CONF");}catch(Exception exception){û.Ñ("PrintBuffer CONF exception: "+exception.Message);}break;case"NAV":try{K=κ.ʣ
(L=="NAV");}catch(Exception exception){û.Ñ("PrintBuffer NAV exception: "+exception.Message);}break;case"STAT":try{K=κ.ʢ()
;}catch(Exception exception){û.Ñ("PrintBuffer STAT exception: "+exception.Message);}break;case"DATA":K=î.I;break;}}public
static void Q(){if(ğ.Ī.Count()==0&&ğ.Į.Count()==0){return;}M();foreach(IMyTextPanel R in ğ.Ī){if(ʲ.ʾ(R.EntityId,"Name")){
continue;}K="";foreach(string P in H){if(ʲ.ʾ(R.EntityId,P)){O(P);break;}}if(K==""){O(J.Peek());}R.ContentType=VRage.Game.GUI.
TextPanel.ContentType.TEXT_AND_IMAGE;ŉ.Ŋ(R.EntityId,R);R.WriteText(K);}foreach(IMyCockpit S in ğ.Į){string L="";for(int R=0;R<S.
SurfaceCount&&R<ː.ʫ.Length;++R){if(!ʲ.ǧ(S.EntityId,ː.ʫ[R],ref L)){continue;}IMyTextSurface T=S.GetSurface(R);T.ContentType=VRage.
Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;switch(L.ToUpper()){case"LOG":O("LOG");break;case"NAV":O("NAV");break;case
"CONF":O("CONF");break;case"STAT":O("STAT");break;case"DATA":O("DATA");break;default:O(J.Peek());break;}T.WriteText(K);ŉ.Ŋ(S.
EntityId,T);}}}public static string U(){if(I.Count()==0){M();}O(J.Peek());return K;}public static void V(){J.Enqueue(J.Dequeue()
);}public enum W{X,Y,Z,A,Î,Ï,ê};public static void ë(W ì){ë(ì,"");}public static void ë(W ì,string í){switch(J.Peek()){
case"NAV":κ.ʜ(ì,í);break;case"CONF":κ.ʝ(ì,í);break;case"LOG":break;}}}static class î{public static string I="-";private
static int ï=0;public static void Q(string N){I+=N+"\n";}public static void ð(){ï++;if(ï>=10000000){ï=0;}I="Clears: "+ï.
ToString()+"\n";}}static class ñ{private static int ò=28;private static int ó=1;private static Dictionary<string,string>ô=new
Dictionary<string,string>();public static string õ(string ö,bool Ö,bool ø=false){if(!ô.ContainsKey(ö)){var ù=(ø?(" SAMv"+Program.
VERSION):"")+(" "+ö+" ");var ú=Ö?'=':'-';ù+=new String(ú,ò-ù.Length-(ø?Program.VERSION.Length:0)-ó);ô[ö]=ù+"\n";}return Û.à()+ô
[ö];}}static class û{private static List<string>ü=new List<string>();private static string è;public static void ý(string
q){ü.Insert(0,q);if(ü.Count()>LOG_MAX_LINES){ü.RemoveAt(ü.Count()-1);}}public static void ð(){ü.Clear();}public static
void þ(string q){ý("D: "+q);}public static void é(string q){ý("I: "+q);}public static void Ð(string q){ý("W: "+q);}public
static void Ñ(string q){ý("E: "+q);}public static void Ò(string Ó,ref Vector3D Ô){ý("GPS:"+Ó+":"+Ô.X.ToString("F2")+":"+Ô.Y.
ToString("F2")+":"+Ô.Z.ToString("F2")+":FFFFFF:");}public static string Õ(bool Ö){è=ñ.õ("Logger",Ö,true);foreach(string q in ü){
è+=q+"\n";}return è;}}string Ø;void Ù(){List<IMyTextPanel>Ú=new List<IMyTextPanel>();GridTerminalSystem.GetBlocksOfType<
IMyTextPanel>(Ú);if(Ú.Count()==0){return;}Û.á();Ø=û.Õ(false);foreach(IMyTextPanel R in Ú){if(!R.CustomName.Contains("LOG")){continue
;}R.FontSize=1.180f;R.Font="Monospace";R.TextPadding=0.0f;R.WriteText(Ø);}}static class Û{private static string[]Ü=new
string[]{"|","/","-","\\"};private static int Ý=0;private static int Þ=0;public static void ß(){if(++Ý>Ü.Length-1){Ý=0;}}
public static string à(){return Ü[Ý];}public static void á(){if(++Þ>Ü.Length-1){Þ=0;}}public static string â(){return Ü[Þ];}}
static class ã{public static string[]ä=new string[]{"\n"};public static string å;public static Queue<string>æ;public static
void ç(){å="";}public static void h(string è){å+=è.Replace(ä[0]," ")+ä[0];}public static void h(int Ǆ){å+=Ǆ.ToString()+ä[0];
}public static void h(long Ǆ){å+=Ǆ.ToString()+ä[0];}public static void h(float Ǆ){å+=Ǆ.ToString()+ä[0];}public static
void h(double Ǆ){å+=Ǆ.ToString()+ä[0];}public static void h(bool Ǆ){å+=(Ǆ?"1":"0")+ä[0];}public static void h(VRage.Game.
MyCubeSize Ǆ){h((int)Ǆ);}public static void h(Ŵ.ƭ Ǆ){h((int)Ǆ);}public static void h(Vector3D Ǆ){h(Ǆ.X);h(Ǆ.Y);h(Ǆ.Z);}public
static void h(List<Vector3D>Ǆ){h(Ǆ.Count);foreach(Vector3D ǃ in Ǆ){h(ǃ);}}public static void h(List<int>Ǆ){h(Ǆ.Count);foreach(
int ǃ in Ǆ){h(ǃ);}}public static void h(ż Ǆ){h(Ǆ.ź);h(Ǆ.Ž);h(Ǆ.ž);}public static void h(Ŵ Ǆ){h(Ǆ.ƃ);h(Ǆ.Ʀ);h(Ǆ.Ƨ);h(Ǆ.ƨ);h(
Ǆ.Ʃ);h(Ǆ.ƪ);h(Ǆ.ƫ);h(Ǆ.Ƭ);h(Ǆ.ƶ);}public static void h(List<Ŵ>Ǆ){h(Ǆ.Count);foreach(Ŵ ǃ in Ǆ){h(ǃ);}}public static void h
(Ƃ Ǆ){h(Ǆ.ƃ);h(Ǆ.Ƅ);h((int)Ǆ.P);}public static void h(List<Ƃ>Ǆ){h(Ǆ.Count);foreach(Ƃ ǃ in Ǆ){h(ǃ);}}public static void h(
ſ Ǆ){h(Ǆ.ź);h(Ǆ.ƀ);}public static void h(List<ſ>Ǆ){h(Ǆ.Count);foreach(ſ ǃ in Ǆ){h(ǃ);}}public static void h(ǀ Ǆ){h(Ǆ.ƥ);h
(Ǆ.ƣ);h(Ǆ.Ɛ);h(Ǆ.Ƒ);h(Ǆ.ƒ);}public static void h(List<ǀ>Ǆ){h(Ǆ.Count);foreach(ǀ ǃ in Ǆ){h(ǃ);}}public static void h(ƕ Ǆ){
h(Ǆ.Ɩ);h(Ǆ.Ɨ);h(Ǆ.Ƙ);}public static void h(Ǻ.Ǽ Ǆ){h((int)Ǆ);}public static void h(ǐ Ǆ){h(Ǆ.ö);h(Ǆ.ź);h(Ǆ.Ǒ);h(Ǆ.ž);h(Ǆ.ǒ)
;h(Ǆ.Ǔ);}public static void ǅ(string è){æ=new Queue<string>(è.Split(ä,StringSplitOptions.None));}public static string ǆ()
{return æ.Dequeue();}public static int Ǉ(){return int.Parse(æ.Dequeue());}public static long ǈ(){return long.Parse(æ.
Dequeue());}public static float ǉ(){return float.Parse(æ.Dequeue());}public static double Ǌ(){return double.Parse(æ.Dequeue());
}public static bool ǋ(){return æ.Dequeue()=="1";}public static VRage.Game.MyCubeSize ǌ(){return(VRage.Game.MyCubeSize)Ǉ()
;}public static Ŵ.ƭ Ǎ(){return(Ŵ.ƭ)Ǉ();}public static Vector3D ǂ(){return new Vector3D(Ǌ(),Ǌ(),Ǌ());}public static List<
Vector3D>ǎ(){List<Vector3D>Ǆ=new List<Vector3D>();int ǡ=Ǉ();for(int Ƹ=0;Ƹ<ǡ;Ƹ++){Ǆ.Add(ǂ());}return Ǆ;}public static List<int>Ǣ(
){List<int>Ǆ=new List<int>();int ǡ=Ǉ();for(int Ƹ=0;Ƹ<ǡ;Ƹ++){Ǆ.Add(Ǉ());}return Ǆ;}public static ż ǣ(){return new ż(ǂ(),ǂ(
),ǂ());}public static Ŵ Ǥ(){Ŵ Ǆ=new Ŵ();Ǆ.ƃ=ǣ();Ǆ.Ʀ=Ǫ();Ǆ.Ƨ=ǌ();Ǆ.ƨ=ǈ();Ǆ.Ʃ=ǆ();Ǆ.ƪ=ǈ();Ǆ.ƫ=ǆ();Ǆ.Ƭ=ǈ();Ǆ.ƶ=Ǎ();return Ǆ;
}public static List<Ŵ>ǥ(){List<Ŵ>Ǆ=new List<Ŵ>();int ǡ=Ǉ();for(int Ƹ=0;Ƹ<ǡ;Ƹ++){Ǆ.Add(Ǥ());}return Ǆ;}public static Ƃ Ǧ()
{return new Ƃ(ǣ(),ǉ(),(Ƃ.ƅ)Ǉ());}public static List<Ƃ>Ǩ(){List<Ƃ>Ǆ=new List<Ƃ>();int ǡ=Ǉ();for(int Ƹ=0;Ƹ<ǡ;Ƹ++){Ǆ.Add(Ǧ()
);}return Ǆ;}public static ſ ǩ(){return new ſ(ǂ(),ǂ());}public static List<ſ>Ǫ(){List<ſ>Ǆ=new List<ſ>();int ǡ=Ǉ();for(int
Ƹ=0;Ƹ<ǡ;Ƹ++){Ǆ.Add(ǩ());}return Ǆ;}public static ǀ ǫ(){return new ǀ((Ŵ.ƭ)Ǉ(),ǆ(),ǆ(),ǆ(),ǂ());}public static List<ǀ>Ǭ(){
List<ǀ>Ǆ=new List<ǀ>();int ǡ=Ǉ();for(int Ƹ=0;Ƹ<ǡ;Ƹ++){Ǆ.Add(ǫ());}return Ǆ;}public static ƕ ǭ(){ƕ Ǯ=new ƕ();Ǯ.Ɩ=Ǉ();Ǯ.Ɨ=ǆ();
Ǯ.Ƙ=Ǭ();return Ǯ;}public static Ǻ.Ǽ Ǡ(){return(Ǻ.Ǽ)Ǉ();}public static ǐ Ǐ(){ǐ ǟ=new ǐ{ö=ǆ(),ź=ǂ(),Ǒ=ǂ(),ž=ǂ(),ǒ=ǂ(),Ǔ=ǉ()
};return ǟ;}}struct ǐ{public string ö;public Vector3D ź;public Vector3D Ǒ;public Vector3D ž;public Vector3D ǒ;public
double Ǔ;}static class ǔ{public static string Ǖ(bool ƃ,int Ô){return(ƃ?"Ori ":"Pos ")+(++Ô).ToString("D2");}}class ǖ{public
string ö;public Vector3D Ô;public bool Ǘ=false;private string[]ǘ;public ǖ(string F){ǘ=F.Split(':');if(ǘ.Length!=6&&ǘ.Length!=7
){return;}try{ö=ǘ[1];Ô=new Vector3D(double.Parse(ǘ[2]),double.Parse(ǘ[3]),double.Parse(ǘ[4]));}catch{return;}Ǘ=true;}}
static class Ǚ{public enum ǚ{Ǜ,ǜ};private static Dictionary<ǚ,bool>ǝ=new Dictionary<ǚ,bool>{};public static void Ǟ(ǚ P){ǝ[P]=
true;}public static void Ä(ǚ P){ǝ[P]=false;}public static bool õ(ǚ P){if(!ǝ.ContainsKey(P)){return false;}return ǝ[P];}}
class ż{public Vector3D ź;public Vector3D Ž;public Vector3D ž;public ż(Vector3D Æ,Vector3D ŷ,Vector3D Ÿ){this.ź=Æ;this.Ž=ŷ;
this.ž=Ÿ;}}class ſ{public Vector3D ź;public Vector3D ƀ;public ſ(Vector3D Æ,Vector3D Ɓ){this.ź=Æ;this.ƀ=Ɓ;}}class Ƃ{public ż
ƃ;public float Ƅ;public enum ƅ{Ɔ,Ƈ,ƈ,Ɖ,Ɗ,Ƌ,ƌ,ƍ,Ǝ,Ə};public ƅ P;public Ƃ(ż N,float Ż,ƅ Ź){ƃ=N;Ƅ=Ż;P=Ź;}public string Ŧ(){
switch(this.P){case ƅ.Ɔ:return"aligning...";case ƅ.Ƈ:return"docking...";case ƅ.ƈ:return"undocking...";case ƅ.ƍ:return
"approaching...";case ƅ.Ɖ:return"converging...";case ƅ.Ɗ:return"navigating...";case ƅ.ƌ:return"taxiing...";case ƅ.Ǝ:return"following..."
;case ƅ.Ə:return"hopping...";}return"Testing...";}public static Ƃ ŧ(string Ũ){ǖ F=new ǖ(Ũ);return new Ƃ(new ż(F.Ô,
Vector3D.Zero,Vector3D.Zero),MAX_SPEED,ƅ.Ɖ);}}class ũ{public int Ū;public int ū;public ũ(){this.Ū=0;this.ū=1;}public void Ŭ(){
this.Ū=this.ū;this.ū=0;}public int ŭ(){return ū-Ū;}}class Ů{public string[]ů;public string[]Ű;public string[]ű;public Ů(ref
string[]ů,ref string[]Ű,ref string[]ű){this.ů=ů;this.Ű=Ű;this.ű=ű;}public string Ų(string ų){foreach(string É in ű){if(É.
ToLower()==ų.ToLower()){return É;}}return"";}}class Ŵ:IComparable<Ŵ>{private static long ŵ=TimeSpan.FromSeconds(60.0).Ticks;
public static Ŵ Ŷ(Vector3D Æ,Vector3D ŷ,Vector3D Ÿ,string ö){Ŵ Ɓ=new Ŵ();Ɓ.ƃ=new ż(Æ,ŷ,Ÿ);Ɓ.Ʃ="Manual";Ɓ.ƫ=ö;return Ɓ;}public
ż ƃ;public List<ſ>Ʀ=new List<ſ>();public VRage.Game.MyCubeSize Ƨ;public long ƨ=0;public string Ʃ="";public long ƪ=0;
public string ƫ="";public long Ƭ=0;public enum ƭ{Ʈ,Ư,ư,Ʊ,Ʋ,Ƴ,ƴ,Ƶ};public ƭ ƶ=ƭ.Ʈ;public void Ʒ(){int Ƹ=(int)ƶ;if(++Ƹ==Enum.
GetNames(typeof(ƭ)).Length){Ƹ=0;}ƶ=(ƭ)Ƹ;}public string ƹ(){switch(ƶ){case ƭ.Ʈ:return"None";case ƭ.Ư:return"Charge";case ƭ.ư:
return"Load";case ƭ.Ʊ:return"Unload";case ƭ.Ʋ:return"Charge&Load";case ƭ.Ƴ:return"Charge&Unload";case ƭ.ƴ:return"Hop";case ƭ.Ƶ
:return"Discharge";}return"";}public static ƭ ƺ(string ö){switch(ö.ToLower()){case"charge":return ƭ.Ư;case"load":return ƭ
.ư;case"unload":return ƭ.Ʊ;case"charge&load":return ƭ.Ʋ;case"charge&unload":return ƭ.Ƴ;case"hop":return ƭ.ƴ;case
"discharge":return ƭ.Ƶ;}return ƭ.Ʈ;}public int CompareTo(Ŵ ƻ){if(this.ƨ!=ƻ.ƨ){return(ƻ.ƨ<this.ƨ)?1:-1;}if(this.ƪ!=ƻ.ƪ){return(ƻ.ƪ<
this.ƪ)?1:-1;}return this.ƫ.CompareTo(ƻ.ƫ);}public void Ƽ(Vector3D ƽ){Ʀ.Sort(delegate(ſ É,ſ ƾ){return(int)(Vector3D.Distance
(ƽ,ƾ.ź)-Vector3D.Distance(ƽ,É.ź));});}public void ƿ(){this.Ƭ=DateTime.Now.Ticks;}public bool ǁ(){if(Ƭ==0){return true;}
return(DateTime.Now.Ticks-Ƭ)<ŵ;}}class ǀ{public Ŵ.ƭ ƥ;public string ƣ;public string Ɛ;public string Ƒ;public Vector3D ƒ;public
ǀ(Ŵ.ƭ Ɠ,string Ɣ,string Ľ,string ö,Vector3D Ô){this.ƥ=Ɠ;this.ƣ=Ɣ;this.Ɛ=Ľ;this.Ƒ=ö;this.ƒ=Ô;}}class ƕ{public int Ɩ;public
string Ɨ;public List<ǀ>Ƙ=new List<ǀ>{};}class ƙ{private class ƚ{public float ƛ;public float Ɯ;public float Ɲ;public ƚ(float ƞ)
{Ɲ=Ɯ=ƛ=ƞ;}public void Ɵ(float ƞ){ƛ=Math.Min(ƛ,ƞ);Ɲ=Math.Max(Ɲ,ƞ);Ɯ=0.9f*Ɯ+0.1f*ƞ;}}private Dictionary<UpdateType,ƚ>Ơ=new
Dictionary<UpdateType,ƚ>();public void Ɵ(float ƞ,UpdateType ơ){if(!Ơ.ContainsKey(ơ)){Ơ[ơ]=new ƚ(ƞ);return;}Ơ[ơ].Ɵ(ƞ);}public
string Ƣ(){var è="Load: Min/Avg/Max\n";foreach(var Ƥ in Ơ){è+=String.Format("{1:P1} {2:P1} {3:P1} :{0}\n",Ƥ.Key,Ƥ.Value.ƛ,Ƥ.
Value.Ɯ,Ƥ.Value.Ɲ);}return è;}}