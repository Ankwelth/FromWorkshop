// TANK GAUGE
// http://steamcommunity.com/sharedfiles/filedetails/?id=2356296615
// Space Engineers in-game scripting for Programmable block
// public domain code by Sean L. Palmer

const string id = "Tank Gauge\n  by p3st|cIdE";

const string LCDName = "LCD Tank Gauge"; // will use PB screen if not found
const int LCDIndex = 0; // sub-index of screen to use, can change to use other of multiple screens

#if true
const string TankName = "Hydrogen";
const string TankSubtype = "Hydrogen";
// there is no distinct IMyHydrogenTank, it's a subtype of IMyGasTank
// it's either SmallHydrogenTank or LargeHydrogenTank but there may be mod-added tanks
#else
 // can make it an oxygen gauge easily enough by allowing empty string instead 
const string TankName = "Oxygen";
const string TankSubtype = "";
#endif
// TODO other mod block ids?

float maxTanks = 0f; // in case tanks get destroyed

bool SameGrid(IMyTerminalBlock t) { return t.CubeGrid == Me.CubeGrid; } //t => t.CubeGrid == Me.CubeGrid); //

float ComputeFill()
{
  var tankblocks = new List<IMyGasTank>(); //IMyTerminalBlock>();
  GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tankblocks, SameGrid);
  double fuel = 0; double tanks = 0;
  foreach (var tank in tankblocks) {
    if (!TankSubtype.Any() ? !tank.BlockDefinition.SubtypeId.Any()
      : tank.BlockDefinition.SubtypeId.Contains(TankSubtype)) { 
      var cap = tank.Capacity;
      fuel += tank.FilledRatio * cap; // gas fill percentage
      tanks += cap;
    }
  }
  if (MathHelper.IsValid(tanks))
    maxTanks = (float)Math.Max(maxTanks, tanks);
  var fill = (float)(fuel / maxTanks);
  if (maxTanks == 0) fill = 0; // avoid showing 100% filled if total capacity is zero
  fill = MathHelper.Clamp(fill, 0f, 1f);
  return fill;
}
// not presently trying to cache the list of tanks, causes too many problems

static Color Hue(double H)
{
  double h = Math.PI * H;
  return new Color(
    Math.Max(0f, (float)Math.Cos(h)),
    Math.Max(0f, (float)Math.Sin(h)),
    Math.Max(0f, (float)Math.Sin(h - Math.PI * .5f)));
}

const string dfont = "DEBUG";

void ShowBarGraph(MySpriteDrawFrame frame, Vector2 fz, float fraction, Color hue)
{
  string spr = "SquareSimple"; //"SquareHollow"; //"Circle"; //
  const int nsegs = 20; //int(Math.Round(frame.DisplaySize.X/frame.DisplaySize.Y * 12));
  // FIXME the offsets need tuned for Wide LCD or Corner LCD; this works for PB keyboard slot though
  // TODO surely there's a way to compute the precise positioning generically
  frame.Add(new MySprite(SpriteType.TEXTURE, "SquareHollow", new Vector2(.5f,.75f) * fz, 
    new Vector2(1f, .9f) * fz, hue * .3f, null, TextAlignment.CENTER, 0f));
  for (int i = (int)Math.Max(1f, Math.Round(fraction * nsegs)); --i >= 0; ) {
    float a = (float)(i / (double)nsegs);
    frame.Add(new MySprite(SpriteType.TEXTURE, spr, fz * new Vector2(a * .9f + .05f, .75f),
      new Vector2(.7f * fz.X / nsegs, fz.Y * .8f), hue * .7f, null, TextAlignment.CENTER, 0f));
  }
}

void ShowPieGraph(MySpriteDrawFrame frame, Vector2 fz, float fraction, Color hue)
{
  string spr = "SquareSimple"; //"Circle"; //"Triangle"; //
  const int nsegs = 36;
  frame.Add(new MySprite(SpriteType.TEXTURE, "CircleHollow", .5f * fz, 
    new Vector2(.83f * fz.Y), hue * .3f, null, TextAlignment.CENTER, 0f));
  for (int i = (int)Math.Max(1f, Math.Round(fraction * nsegs)); --i >= 0; ) {
    float a = (float)((.5f + i) / (double)nsegs);
    double r = 2f * Math.PI * a;
    frame.Add(new MySprite(SpriteType.TEXTURE, spr,
      new Vector2((float)Math.Sin(r), -(float)Math.Cos(r)) * .4f * fz.Y + .5f * fz, 
      new Vector2(.05f, .1f) * fz.Y, hue * .7f, null, TextAlignment.CENTER, (float)r));
  }
}
// FIXME idk why it doesn't center correctly

void ShowTankGraph(IMyTextSurface lcd, float fraction, string pct)
{
  lcd.ContentType = ContentType.SCRIPT; //TEXT_AND_IMAGE; //
  lcd.Script = null;
  lcd.Alignment = TextAlignment.CENTER;
  var hue = Hue(fraction * .6);
  lcd.ScriptBackgroundColor = hue * .04f;
  var fz = lcd.SurfaceSize;
  using (var frame = lcd.DrawFrame()) { //MySpriteDrawFrame
    if (lcd.SurfaceSize.X >= 1.6f*lcd.SurfaceSize.Y)  //false) //true) //IsWide(surf)) //
      ShowBarGraph(frame, fz, fraction, hue);
    else
      ShowPieGraph(frame, fz, fraction, hue);
    frame.Add(new MySprite(SpriteType.TEXT,  TankName, fz * new Vector2(.5f, .30f), 
      fz, Color.Lerp(hue, Color.White, .5f), dfont, TextAlignment.CENTER, 1f));
    frame.Add(new MySprite(SpriteType.TEXT,  pct, fz * new Vector2(.5f, .47f), 
      fz, Color.Lerp(hue, Color.White, .25f), dfont, TextAlignment.CENTER, 2f)); //CLIP_RECT//
  }
}
// future proofing for handling multiple screens
IEnumerable<IMyTextSurface> FindLCDs()
{
  IMyTextPanel panel = GridTerminalSystem.GetBlockWithName(LCDName) as IMyTextPanel;
  if (panel != null && !panel.IsWorking) panel = null;
  IMyTextSurfaceProvider surfs = panel as IMyTextSurfaceProvider;
  if (surfs != null && surfs.SurfaceCount > 0 
    && LCDIndex >= 0 && LCDIndex < surfs.SurfaceCount) {
    var lcd = surfs.GetSurface(LCDIndex); //Math.Min(lcdIndex, surfs.SurfaceCount - 1));
    if (lcd != null) {
      yield return lcd;
      yield break;
    }
  }
  var pb = Me as IMyTextSurfaceProvider;
  if (pb != null && pb.SurfaceCount > 0) {
    var pbs = pb.GetSurface(Math.Min(pb.SurfaceCount-1, LCDIndex));
    if (pbs != null) {
      Echo("fallback to PB screen");
      yield return pbs;
      yield break;
    }
  }
  Echo("no LCD screen found to display on");
  yield break;
}

void ClearLCD(IMyTextSurface lcd)
{
  if (lcd == null) return;
  //lcd.WriteText(""); //ClearImagesFromSelection();
  lcd.ContentType = ContentType.NONE;
}

void ShowFillGraph(IMyTextSurface lcd, float fraction)
{
  float percent = (int)Math.Round(100 * fraction);
  string pct = percent.ToString() + '%'; //fraction.ToString("P1")); //
  Echo(pct);
  ShowTankGraph(lcd, fraction, pct);
}

void ShowFill(float fill)
{
  foreach (var lcd in FindLCDs()) {
    ShowFillGraph(lcd, fill);
  }
}

void Toggle()
{
  if (Runtime.UpdateFrequency != UpdateFrequency.None) { 
    Runtime.UpdateFrequency = UpdateFrequency.None;
    maxTanks = 0f;
    foreach (var lcd in FindLCDs())
      ClearLCD(lcd);
    Echo("stopped");
  } else { 
    Runtime.UpdateFrequency = UpdateFrequency.Update10; //Update1; //
  }
}

void Main(string argument, UpdateType updateSource)
{
  Echo(id);
  switch (updateSource) {
    // when run by self-tick, update display
    case UpdateType.Update1:
    case UpdateType.Update10:
    case UpdateType.Update100:
      ShowFill(ComputeFill());
      break;
    // when run by timer or manually, toggle state, which triggers re-assessment of max capacity
    case UpdateType.IGC:
    case UpdateType.Terminal:
    case UpdateType.Trigger:
      Toggle();
      break;
  }
}