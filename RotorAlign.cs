// ROTORALIGN
// https://steamcommunity.com/sharedfiles/filedetails/?id=2211458232
// custom built for PirateLaserBeam's tunnel-boring truck
// basically keep a rotor or hinge aligned so its 0 angle faces upward,
// according to the remote, regarding relative roll (so along remote forward axis, removing roll)
// default is align w natural gravity.  
// TODO if set remote control's CustomData field to name of a waypoint,
// will use that waypoint, or set to "waypoint" to use current active waypoint,
// or TODO to a copied-to-clipboard format gps coordinate string
// for Space Engineers in-game scripting for Programmable block
// public domain code by Sean L. Palmer

const string RAID = "RotorAlign\nby p3st|cIdE";

const string dfont = "Debug";
const float maxRotation = 4f; // main "speed" control; slow is more stable
const UpdateFrequency freq = UpdateFrequency.Update10; // seems to work better than running every frame, actually, but risks some lag

// requires a remote on same grid to measure local gravity
IMyRemoteControl FirstRemote()
{
  var remotes = new List<IMyRemoteControl>();
  GridTerminalSystem.GetBlocksOfType(remotes);
  foreach (var r in remotes) {
    if (r.CubeGrid != Me.CubeGrid) continue;
    if (!r.IsWorking) continue;
    return r;
  }
  Echo("No remote!");
  return null;
}
// goal is to allow either Rotor or Advanced Rotor or Hinge, guess I got the correct interface
IMyMotorStator FirstRotor() //IMyMechanicalConnectionBlock
{
  var parts = new List<IMyMotorStator>(); //IMyAttachableTopBlock>(); //IMyMotorRotor
  GridTerminalSystem.GetBlocksOfType(parts);
  foreach (var p in parts) {
    //if (!(p is IMyMotorRotor)) continue;
    //if (p.CubeGrid != Me.CubeGrid) continue;
    if (!p.IsAttached) continue; // hmm
    if (p.TopGrid != Me.CubeGrid) continue;
    //var rot = p.Base as IMyMotorStator;
    return p; //rot;
  }
  Echo("No attached rotor part!");
  return null;
}

static float CurrentAngle(IMyMotorStator r) // in radians
{
  return MathHelper.Clamp(r.Angle, r.LowerLimitRad, r.UpperLimitRad);  // cleanup since it's often slightly out of range
}
// simple PID to align rotor angle to specified angle over time
void SetAngle(IMyMotorStator rot, float radians)
{
  radians = MathHelper.Clamp(radians, rot.LowerLimitRad, rot.UpperLimitRad);
  float cur = CurrentAngle(rot);
  float delta = radians - cur;
  const float speed = 3f; 
  delta *= speed;
  float r = MathHelper.Clamp(delta, -maxRotation, maxRotation);
  rot.TargetVelocityRad = r;
}

void Stop() // failsafe shut off rotor, stop from rotating
{
  var rot = FirstRotor();
  if (rot != null) rot.TargetVelocityRad = 0f;
}

public void Main(string arg, UpdateType updateSource)
{
  try {
    Echo(RAID);
    var display = Me.GetSurface(0);
    if ((updateSource & (UpdateType.Update1|UpdateType.Update10|UpdateType.Update100)) == 0) {
      if (Runtime.UpdateFrequency != freq) 
        Runtime.UpdateFrequency = freq;
      else {
        Runtime.UpdateFrequency = UpdateFrequency.None;
        Stop();
        Echo("stop");
        display.DrawFrame().Dispose(); // clear screen
      }
    } else {
      Echo("align");
      var remote = FirstRemote();
      if (remote == null) return;
      var rot = FirstRotor();
      if (rot == null) return;
      Matrix worldToGrid = remote.WorldMatrix.GetOrientation();
      worldToGrid.TransposeRotationInPlace();
      Vector3 wp = Vector3.Transform(remote.Position, remote.WorldMatrix); // remote position in world coords
      Vector3 wd = remote.GetNaturalGravity(); //GetTotalGravity(); // gravity is a world space down direction
      var cd = remote.CustomData;
      if (cd != null) {
        if (cd != "") {
              cd = cd.ToLower().Trim();
              bool handled = false;
              Vector3 at = wp;
              /* 
              // or TODO set to "sun" to use direction to sun (for aligning solar panels),
              // nevermind, no reliable halfway-easy way to obtain direction to sun from in-game script
              if (cd == "sun") {
                Vector3 sunpos = new Vector3(); // FIXME get somehow
                wd = wp - sunpos;
                handled = true;
              }
              */
              // FIXME this sort of works, but currently may be backward or something because it's not quite right.
              if (cd == "waypoint" && !remote.CurrentWaypoint.IsEmpty()) {
                Echo("Current waypoint:\n" + remote.CurrentWaypoint.Name);
                at = remote.CurrentWaypoint.Coords;
                //wd = remote.CurrentWaypoint.Coords - wp;
                handled = true;
              }
              if (!handled) {
                Vector3 pp = new Vector3();
                string[] spl = cd.Split(':'); // split up trimmed-clipboard-style waypoint string of form "X:Y:Z" into separate strings
                if (spl.Length == 3) {
                  bool ok = 
                    float.TryParse(spl[0], out pp.X) &&
                    float.TryParse(spl[1], out pp.Y) &&
                    float.TryParse(spl[2], out pp.Z);
                  if (ok) {
                    Echo("Custom waypoint:\n" + pp.ToString());
                    at = pp;
                    //wd = wp - pp;
                    handled = true;
                  }
                }
              }
              if (handled) {// found something to aim at instead of away from gravity
                wd = at - wp;
                if (Vector3.Dot(wd, remote.WorldMatrix.GetOrientation().Up) > 0f)
                    wd = -wd; // if facing away from waypoint, use complementary angle
                //  wd = -wd;  // gravity goes opposite way of waypoints - I just flipped them all where calculated instead.
              }
        }
      }
      Vector3 lu = Vector3.TransformNormal(-wd, worldToGrid);
      lu = Vector3.Normalize(lu);
      // must map gravity vector to grid local space,
      // do asin to measure angular deviation
      float rad = -(float)Math.Asin(MathHelper.Clamp(lu.X, -1f, 1f)); // desired rotation angle in radians
      float cur = CurrentAngle(rot);
      SetAngle(rot, cur + rad);
      using (var frame = display.DrawFrame()) {
        var fz = display.SurfaceSize;
        const TextAlignment ctr = TextAlignment.CENTER; SpriteType txt = SpriteType.TEXT;
        float a = (cur - rad) / (float)Math.PI;
        frame.Add(new MySprite(txt,  "|", fz * new Vector2(MathHelper.Clamp(.5f  - a, 0f, 1f), .3f), Vector2.One, new Color(1f,.2f,.2f), dfont, ctr, 3f));
     }
    }
  } catch (Exception) {
    Stop(); throw;
  }
}

public Program()
{
  Runtime.UpdateFrequency = UpdateFrequency.Update10;
  var display = Me.GetSurface(0);
  display.ContentType = ContentType.SCRIPT;
  display.ScriptBackgroundColor = Color.Black;
  var display2 = Me.GetSurface(1);
  display2.ContentType = ContentType.TEXT_AND_IMAGE;
  display2.WriteText(RAID);
  display2.FontSize = 6f;
  display2.Alignment = TextAlignment.CENTER;
  display2.Font = dfont;
}