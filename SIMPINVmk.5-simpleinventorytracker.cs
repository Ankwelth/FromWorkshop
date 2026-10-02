/* SIMPINV (simple inventory tracker) mk.5
 * CC-BY-SA, 2018-22 null inc.
 * https://steamcommunity.com/id/null_inc

  Displays on-grid ingots, ore, components, block weapons ammo, and gasses,
    as quota of their all-time maximum (persits through world save/load and recompile).

  EXAMPLE
  =======

   * you have 12000 units of iron ore.            "Or.Iron 12.0k =========="
   * you now put half of that through a refinery. "Or.Iron  6.0k =====....."
   * The refined ingots now show up as "In.Iron"
   * you store 24000 more iron ore.               "Or.Iron 30.0k =========="
   * you refine another 6000 iron ore             "Or.Iron 24.0k ========.."

  SETUP
  =====

   * Compile this script in a Programmable Block
   * Put "SIMPINV" in the name of LCD panels you want it to update
   * configure in the code below how often it should update, if it should count stuff on subgrids,
         if it should write to displays on subgrids, and which items to ignore

  RESET
  =====
 
   * To clear all stored amounts, run the progblock with argument "reset"
   * To clear individual items, put part of their name after "reset" (case-insensitive). Examples:
      "reset in.magn"    resets magnesium ingots
      "reset magn iron"  resets ore and ingots of magnesium and iron
      "reset AM. co."    resets all ammo and all components

   CHANGELOG
   ==========

     mk.5:
     * a timer block is no longer needed
     * font, font size, display mode are automatically adjusted
     * show new ammo types and stone-"ore"
     * hide new handheld and special items
     * separate control for whether to inventory, or display on docked grids
     * minor improvements to display layout
     * legacy reactor controls removed

    CONFIGURE
    =========*/

const bool inventoryDockedGrids = true;

const bool displayOnDockedGrids = false;

const int intervalSeconds = 10;

//items with these substrings (no type prefix) 
//  are never counted or displayed:
static readonly HashSet<string> ignoreItems = new HashSet<string>
{
  "AutomaticRifleGun_Mag_20rd",
  "ClangCola",
  "CosmicCoffee",
  "Datapad",
  "ElitePistolMagazine",
  "FullAutoPistolMagazine",
  "HydrogenBottle",
  "Medkit",
  "NATO_5p56x45mm",
  "OxygenBottle",
  "Powerkit",
  "PreciseAutomaticRifleGun_Mag_5rd",
  "RapidFireAutomaticRifleGun_Mag_50rd",
  "SemiAutoPistolMagazine",
  "UltimateAutomaticRifleGun_Mag_30rd",
  "ZoneChip",
};

//replace these item names
//the prefix is required for category sorting!
static Dictionary<string, string> renameItems = new Dictionary<string, string>
{
  { "Am.LargeCalib", "Am.Artillery" },
  { "Am.LargeRailg", "Am.Railgun_L" },
  { "Am.MediumCali", "Am.AssaultCan" },
  { "Am.NATO_25x18", "Am.Gatling" },
  { "Am.SmallRailg", "Am.Railgun_S" },
	{ "In.Stone", "In.Gravel"}
};

///////////////////////////////////////////////
// END OF SECTION INTENDED FOR CONFIGURATION //
///////////////////////////////////////////////


static int tenTicks = 0;

// add amount to dictionary if it exists, else add entry
void add2Dict(string name, long count, Dictionary<string, long> D)
{
  if(D.ContainsKey(name)) {
    D[name] += count;
  } else {
    D.Add(name, count);
  }
}

// put entry in dictionary, replace existing if there is one
void replaceDict(string name, long count, Dictionary<string, long> D)
{
  if(D.ContainsKey(name)) {
    D[name] = count;
  } else {
    D.Add(name, count);
  }
}

//serialize data for storage
string d2stor(Dictionary<string, long> d)
{
  var list = d.Keys.ToList();
  list.Sort();
  string s = "";
  foreach(var key in list)
    s = s + key + ":" + d[key] + "\n";
  return s;
}

// write SI suffixes
string kmg(long n)
{
  double d;
  var post = ' ';
  if (n>1000000000000) {
    d = n / 1000000000000d;
    post = 'T';
  } else if (n>1000000000) {
    d = n / 1000000000d;
    post = 'G';
  } else if (n> 1000000) {
    d = n / 1000000d;
    post = 'M';
  }  else if (n> 1000) {
    d = n / 1000d;
    post = 'k';
  } else {
   // return n.ToString() + "  ";
    return string.Format("{0,3}", n);
  }
  return string.Format("{0,5:F1}",d) + post;
}

// percent. 
string pct(int x, int w) {
   string ret = "";
   int b = x * w / 100;
   for(int i = 0; i < b; i ++)
    ret += "=";
   for(int i = b; i < w; i ++)
    ret += ".";
   return ret;
}

// serialize dictionary
string d2string(Dictionary<string, long> d, Dictionary<string, long> dold, Dictionary <string, string> delta)
{
  var list = dold.Keys.ToList();
  list.Sort();
  string s = "";

  foreach(var key in list) {
    long n = 0;
    s += key.PadRight(14);
    long dk = 0;
    if (d.ContainsKey(key)) {
      dk = d[key];
      if(dold[key] == 0)
        n = 100;
      else
        n = d[key] * 100 / dold[key] ;
    }
    string dstr = " ";
    if (delta.ContainsKey(key))
      dstr = delta[key];
    s += kmg(dk).PadRight(7) + pct((int)n, 10) + " " + dstr + "\n";
  }
  return s;
}

// one column display with indent
string col1(string bla) {
  string[] lines = bla.Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
  string ret = "";
  foreach(string s in lines)
   ret += " " + s + "\n";
  return ret;
}

// two column display
string col2(string bla) {
  string ret = "";
  List<string> L = new List<string>();
  List<string> R = new List<string>();
  string[] lines = bla.Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

  var lastL = ' ';
  var lastR = ' ';

  //split in columns by prefix;
  //  insert a blank line when prefix changes
  foreach(string s in lines) {
    if(s[0] == 'C' || s[0] == 'A') {
      if(lastR != s[0] && R.Count != 0) {
        R.Add("");
      }
      R.Add(s);
      lastR = s[0];
    } else {
      if(lastL != s[0] && L.Count != 0) {
        L.Add("");
      }
      L.Add(s);
      lastL = s[0];
    } 
  }

  //put the columns together
  int max = L.Count;
  if (R.Count > max) max = R.Count;

  for (int fu = 0; fu < max; fu++) {
    string ret1 = "";
    string ret2 = "";
    if(fu < L.Count) {
      ret1 = L[fu];
    }
    if(fu < R.Count) {
      ret2= R[fu];
    }
    int w = 37;
    ret += ret1.PadRight(w) + ret2.PadRight(w) + "\n";
  }
  return ret;
}

// set all values in dict. 2 to their key's max of dict. 1 and 2
void max2(Dictionary<string, long> one, Dictionary<string, long> two)
{
  foreach(var key in one.Keys)
    if ( ! two.ContainsKey(key) || one[key] > two[key])
        two[key] = one[key];
}

// load and parse (unserialize) storage
Program() {
  Echo("Program()");
  Runtime.UpdateFrequency = UpdateFrequency.Update10;
  maxitems = new Dictionary<string, long>();
  long d;
  string[] lines = Storage.Split( new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
  foreach(string s in lines) {
    string[] sub = s.Split( new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
    if(sub.Count() > 1 &&  long.TryParse(sub[1], out d)) {
        maxitems[sub[0]] = d;
    }
  }
}

Dictionary<string, long> maxitems;  //what we had at max
Dictionary<string, long> previtems; //what we had before now

string trunc(string s, int n)
{
  if(string.IsNullOrEmpty(s)) return s;
  if(s.Length <= n) return s;
  return s.Substring(0,n);
}

// add one inventory to a dictionary
void addInventoriesToDictionary(IMyTerminalBlock b, Dictionary<string, long> items)
{
  if (! b.HasInventory)
    return;
  int n = 0;
  while(true) {
    var it = new List<MyInventoryItem>();
    try {
      b.GetInventory(n++).GetItems(it);
    } catch {
      return;
    }
    foreach (MyInventoryItem item in it) {
      string s = item.ToString();
      var bla = "MyObjectBuilder_";
      s = s.Substring(s.LastIndexOf(bla) + bla.Length);
      if (s[0] != 'P' ) {
        var ts = item.Type.ToString();
        var tss = ts.Split(new char []{'/'});  //Postfix

        if (ignoreItems.Contains(tss[1]) == false) {
          ts = trunc(tss[1], 10);
          s = s.Substring(0,2) + "." + ts;  //Prefix

          string rewrite;
          if (renameItems.TryGetValue(s, out rewrite)) {
            s = rewrite;
          }
          add2Dict(s, (long)item.Amount, items);
        }
      }
    } //foreach item
  } //while true
}

//return one symbol if a value in "cur" is greater, another when smaller
Dictionary <string, string> getDelta(Dictionary<string, long> cur, Dictionary<string, long> prev)
{
  string up = "▲";
  string down = "▼";
  Dictionary<string, string> ret = new Dictionary<string, string>();
  if (prev == null) {
    foreach(var k in cur.Keys)
      ret[k] = up;
    return ret;
  }
  foreach(var k in cur.Keys)
  {
    if (! prev.ContainsKey(k) || prev[k] < cur[k])
      ret[k] = up;
    else if (prev[k] > cur[k])
      ret[k] = down;
    else
      ret[k] = " ";
  }
  foreach(var k in prev.Keys)
    if (! cur.ContainsKey(k))
      ret[k] = down;
  return ret;
}

// find all inventories and pass them to addition
void inventory()
{
  Dictionary<string, long> items = new Dictionary<string, long>();
  var B = new List<IMyTerminalBlock>();
  GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(B, b=> inventoryDockedGrids || Me.CubeGrid == b.CubeGrid);
  foreach (var b in B) {
      addInventoriesToDictionary(b, items);
  }

  var T = new List<IMyGasTank>();
  GridTerminalSystem.GetBlocksOfType<IMyGasTank>(T, b=> inventoryDockedGrids || Me.CubeGrid == b.CubeGrid);
  double o2 = 0;
  double h2 = 0;
  foreach (var t in T) {
    double liter = t.FilledRatio * t.Capacity;
    if (t.DetailedInfo.Contains("Oxygen"))
      o2 += liter;
    else if (t.DetailedInfo.Contains("Hydrogen"))
      h2 += liter;
    else
      Echo("I don't know what kind of tank this is: " + t.DetailedInfo);
  }
  if (o2 > 0)
    add2Dict("  Oxygen", (long)o2, items);
  if (h2 > 0)
    add2Dict("  Hydrogen", (long)h2, items);

  max2(items, maxitems);
  Dictionary<string, string> delta = getDelta(items, previtems);
  previtems = items;
  string output = d2string(items, maxitems, delta);
  display(output);
  Echo(output);
}

const string font = "Monospace"; //always

void display(string s)
{
  List<IMyTextPanel> panels = new List<IMyTextPanel>();
  GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels,
    b => (displayOnDockedGrids || Me.CubeGrid == b.CubeGrid) && b.CustomName.ToUpper().Contains("SIMPINV"));
  
  foreach (var panel in panels) {

    string output = col2(s);

    var dim = panel.MeasureStringInPixels(new StringBuilder(output), font, 1.0f);
    float scale = 0.33f;
    if (dim.X == 0 || dim.Y == 0) {
      Echo("can not get font size");
    } else {
      var surf = panel.SurfaceSize;
      scale = surf.X / dim.X;
      float yfac = surf.Y / dim.Y;
      if(yfac < scale)
        scale = yfac;
    }

    panel.FontSize = scale;
    panel.Font = font;
    panel.ContentType = ContentType.TEXT_AND_IMAGE;
    panel.WriteText(col2(s));
  }
}

bool reset = false;

void Main(string argument, UpdateType updateType)  
{
  reset = false;

  // the script runs every 10 ticks (6/s) but we do not update every time
  if ((updateType & UpdateType.Update10) != 0) {
    if (tenTicks % (6 * intervalSeconds) != 0) {
      tenTicks++;
      return;
    } else {
      tenTicks++;
    }
  } else {
    //when the script was triggered externally, we will update on next tick
    tenTicks = 0;
  }

  string arg = argument.ToUpper().Trim();
  if (arg == "RESET") {
    Echo("RESET");
    Storage = "";
    reset = true;
    display("SIMPINV" + updateType.ToString());
    maxitems = new Dictionary<string, long>();
    return;
  } else if (arg.StartsWith("RESET")) {
    arg = arg.Remove(0, 5).Trim();
    string[] resetItems = arg.Split(null);
    List<string> rmKeys = maxitems.Where(kvp => resetItems.Any(kvp.Key.ToUpper().Contains)).Select(kvp => kvp.Key).ToList();
    foreach (string rm in rmKeys) {
      Echo("rm " + rm);
      maxitems.Remove(rm);
    }
    Storage = d2stor(maxitems);
    reset = false;
  } else {
    reset = false;
  }
  inventory();
}

// call persistor
void Save()
{
  if (reset)
    Storage = "";
  else
    Storage = d2stor(maxitems);
}