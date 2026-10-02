// LEVELLER
// https://steamcommunity.com/sharedfiles/filedetails/?id=1672723291
// for Space Engineers in-game scripting for Programmable block
// public domain code by Sean L. Palmer

// really all this thing does atm is roll removal vs. gravity over time (anti-flip technology!)
// could also easily remove or limit pitch, as Gravity Aligner does; this is a much simpler script,
// more like how GA used to be originally, but without the input and mass kludges
const string LLID = " Leveller by p3st|cIdE";

const float sensRotation = 0.5f; // turn rate scale factor - tune to approximate game's ship controller sensitivity
const float antiRoll = 25f; // how quickly it tries to remove roll - too much can cause oscillation
const float maxRotation = (float)Math.PI * .5f; // max radians per second turn rate requested
// if slowed even further, it becomes less useful but more annoying, and if sped up, fights pilot control excessively.
// In fact it puts a hard limit on how far the vessel can roll by user input, as a side effect, if strong enough.
// If remove roll aggressively enough, it has the side effect of making yaw behave sensibly in most cases!
//const float antiPitch = 0f; // TODO?

void ShowDebug(string msg)
{
  var d = GridTerminalSystem.GetBlockWithName("debug") as IMyTextSurface;
  if (d != null) { d.WriteText(msg); } //d.ContentType = TEXT_AND_IMAGES; }
}

static string Pretty(Vector3 v)
{
  v = new Vector3(Math.Round(v.X, 2), Math.Round(v.Y, 2), Math.Round(v.Z, 2));
  return String.Format(" {0:f2}  {1:f2}  {2:f2}", v.X, v.Y, v.Z);
}

static string Pretty(Matrix m)
{
  m = Matrix.Transpose(m); // actually column-major
  return Pretty(m.Right) + "\n" + Pretty(m.Down) + "\n" + Pretty(m.Backward) + "\n";
}

System.Text.StringBuilder log = new System.Text.StringBuilder();

List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>(); // only the connected ones

IEnumerable<IMyShipController> AllControllers()
{
  foreach (var b in blocks) {
    var c = b as IMyShipController;
    if (c != null) yield return c;
  }
}

IEnumerable<IMyGyro> AllGyros()
{
  foreach (var b in blocks) {
    var g = b as IMyGyro;
    if (g != null) yield return g;
  }
}

// v is world control vector in radian/s (similar to angular momentum but for intended rotation)
static void TurnGyro(IMyGyro g, Vector3 w, bool overrideg)
{
  g.GyroOverride = overrideg;
  if (overrideg) {
    Matrix mworld = g.WorldMatrix.GetOrientation();
    mworld.TransposeRotationInPlace();
    
    w = Vector3.TransformNormal(w, ref mworld); // to gyro local
    w *= MathHelper.RadiansPerSecondToRPM;
    g.Pitch = w.X;
    g.Yaw   = w.Y;
    g.Roll  = w.Z;
  }
}

//Vector3 lastcontrol = new Vector3();
//DateTime lastupdate = DateTime.Now;

void OverrideTotalRotion(Vector3 r, bool overrideg)
{
  bool anyr = r.Dot(r) > 1e-3;
  bool nonzeror = anyr || overrideg;
  //var elapsed = (DateTime.Now - lastupdate).Seconds;
  // was attempting to help gyros fight ground contact and gravity forces; makes uncontrollable if tuned high enough for it to work, though.
  //if (overrideg && (r - lastcontrol).LengthSquared() < Math.Exp(-12f*elapsed)) // && elapsed < .1f)
  //  return; // just don't update yet
  //lastcontrol = r; lastupdate = DateTime.Now;
  foreach (var g in AllGyros())
    TurnGyro(g, r, nonzeror);
}

void ManageController(IMyShipController c, ref Vector3 r)
{
  var vel = c.GetShipVelocities(); var wavel = vel.AngularVelocity;
  // I'm not sure the direction of their wavel matches up with our control vector orientation, must still confirm TODO
  var g = c.GetNaturalGravity();
  var lg = (float)g.Length(); // tracking some approximation of average gravity vector
  Matrix shipToWorld = c.WorldMatrix.GetOrientation();
  Matrix worldToShip = shipToWorld; worldToShip.TransposeRotationInPlace();
  var up = shipToWorld.Up; // in world space according to current controller, in case there's no gravity
  if (lg > 0) up = Vector3.Normalize(g / -lg);
  var localup = Vector3.TransformNormal(up, ref worldToShip); // to local space
  var localavel = Vector3.TransformNormal(wavel, ref worldToShip);
#if true
//  log.AppendLine(Pretty(shipToWorld));
//  log.AppendLine(" up:" + Pretty(up));
//  log.AppendLine(" lup:" + Pretty(localup));
//  log.AppendLine(" lavel:" + Pretty(localavel));
  log.AppendLine("roll: " + Math.Asin(localup.X).ToString("f2") + " radians");
#endif
  Vector3 view = new Vector3();
  if (c.CanControlShip) {
    float rc = shipToWorld.Up.Dot(up), rs = shipToWorld.Forward.Dot(up), rx = shipToWorld.Right.Dot(up);
    if (c.IsUnderControl) { // that's better!
      var aim = c.RotationIndicator;
      view = new Vector3(aim.X, +aim.Y, +c.RollIndicator);
      var yaw = view.Y;
      yaw *= rc; // just use less yaw when not level - yaw only works when approx level // working amazingly well for a very simple hack.
    }
    float inversion = (.54f - .46f * rc); // how inverted are we? doesn't go all the way to zero to allow some response when almost level, but too far causes overshoot
  #if true // ROLL REMOVAL OVER TIME
    view.Z += rx * inversion * antiRoll; // seems I need to factor Y alignment (rc) instead of X somehow for speed, mapping max rate to when completely inverted
  #endif // but over time only fixes it post-hoc
  #if true // COUNTER ROLL ANGULAR VELOCITY - if you display lavel you can tell it's a bit jittery though FIXME
    view.Z += localavel.Z * .125f; // actively oppose roll rotation, manage feedback, has a frame of lag
  #endif // actually does stabilize things
    view = Vector3.TransformNormal(view, ref shipToWorld);
    view *= sensRotation;
    r += view;
  }
  if (r.Dot(r) < 1e-3) r *= 0f;
}

void ManageVesselControl() 
{
  var r = new Vector3(); // total desired rotation in world radian/s
  foreach (var c in AllControllers()) 
    ManageController(c, ref r);
  //r = Vector3.Clamp(r, new Vector3(-maxRotation), new Vector3(maxRotation));
  if (r.Dot(r) > maxRotation * maxRotation) r *= maxRotation / r.Length();
  OverrideTotalRotion(r, true);
}

void StopOverrides() // failsafe shut off any overrides
{
  OverrideTotalRotion(new Vector3(), false);
}

// exec once per game frame while going
void Update()
{
  Echo(LLID + "\n going");
  blocks.Clear();
  GridTerminalSystem.GetBlocks(blocks);
  foreach (var b in blocks.AsEnumerable().Reverse()) {
    if (!b.IsSameConstructAs(Me))
      blocks.Remove(b);
  }
  ManageVesselControl();
#if true
  var logstr = log.ToString();
  if (logstr.Length == 0) logstr = LLID;
  ShowDebug(logstr);
  log.Clear();
#endif
}

void Stop() { Storage = null; Runtime.UpdateFrequency = UpdateFrequency.None; StopOverrides(); ShowDebug(LLID + "\n stopped"); }
void Go() { Storage = "go"; Runtime.UpdateFrequency = UpdateFrequency.Update1; }

bool Commands(string arg)
{
  if (arg == "toggle" || arg == "") {
    if (Runtime.UpdateFrequency != UpdateFrequency.None) arg = "stop";
    else arg = "go";
  }
  if (arg == "go") { Echo(LLID + "\n starting"); Go(); return true; }
  else if (arg == "stop") { Echo(LLID + "\n stopping"); Stop(); return true; }
  Echo(LLID + "\n commands:  go, stop"); return false;
}

public void Main(string arg, UpdateType updateSource)
{
  try
  {
    if (updateSource == UpdateType.Terminal || updateSource == UpdateType.Trigger) Commands(arg);
    else Update();
  }
  catch (Exception)
  {
    Stop(); throw;
  }
}

public Program()
{
  if (Storage != null) Runtime.UpdateFrequency = UpdateFrequency.Update1; // resume after reload or recompile
}
