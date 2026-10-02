// === ORE SMELTING STATUS (C#6 / PB-safe) ===============================
// Scans all inventories on the same construct for MyObjectBuilder_Ore,
// tracks live rates (kg/min) with EMA smoothing, and shows per-ore ETAs.
// Display tag on panels/cockpits: [SMELT] or [SMELT:0.55] (fixed size).
//
// PB Custom Data (optional):
// [SMELT]
// FontSize=0.55
// PbFontSize=0.55
// MinFont=0.42
// AutoFit=true
// PageSeconds=6
// ======================================================================

// ---- Config -----------------------------------------------------------
const string SMELT_DISPLAY_TAG = "[SMELT]";
const bool   SAME_CONSTRUCT_ONLY = true;
const int    SURFACE_INDEX = 0;

const double SMELT_RATE_EPS = 1e-9;  // kg/s threshold
const double SMELT_EMA      = 0.20;  // smoothing factor

const double SMELT_FONT_LCD_DEF = 0.55;
const double SMELT_FONT_PB_DEF  = 0.55;
const double SMELT_MIN_FONT_DEF = 0.42;
const bool   SMELT_AUTOFIT_DEF  = true;
const double SMELT_PAGE_SEC_DEF = 6.0;

// ---- State ------------------------------------------------------------
readonly System.Text.StringBuilder _sb = new System.Text.StringBuilder(20000);
readonly System.Collections.Generic.Dictionary<string,double> _curOre =
  new System.Collections.Generic.Dictionary<string,double>(System.StringComparer.OrdinalIgnoreCase);
readonly System.Collections.Generic.Dictionary<string,SMELT_Tracker> _trackers =
  new System.Collections.Generic.Dictionary<string,SMELT_Tracker>(System.StringComparer.OrdinalIgnoreCase);

readonly System.Collections.Generic.List<IMyTerminalBlock> _invBlocks = new System.Collections.Generic.List<IMyTerminalBlock>();
readonly System.Collections.Generic.List<MyInventoryItem> _items = new System.Collections.Generic.List<MyInventoryItem>();
readonly System.Collections.Generic.List<IMyTextPanel> _panels = new System.Collections.Generic.List<IMyTextPanel>();
readonly System.Collections.Generic.List<IMyTerminalBlock> _surfBlocks = new System.Collections.Generic.List<IMyTerminalBlock>();

double _fontLCD = SMELT_FONT_LCD_DEF, _fontPB = SMELT_FONT_PB_DEF, _minFont = SMELT_MIN_FONT_DEF, _pageSec = SMELT_PAGE_SEC_DEF;
bool _autoFit = SMELT_AUTOFIT_DEF, _inited = false;
double _pageTimer = 0; int _page = 0;

// ---- Life cycle -------------------------------------------------------
public Program(){
  Runtime.UpdateFrequency = UpdateFrequency.Update100;
  SMELT_LoadCfg();
  SMELT_LoadState();
}
public void Save(){
  var s = new System.Text.StringBuilder();
  foreach(var kv in _trackers){
    var t = kv.Value;
    s.Append("ORE|").Append(kv.Key).Append("|").Append(t.Last).Append("|").Append(t.Rate).Append("\n");
  }
  Storage = s.ToString();
}

public void Main(string arg, UpdateType ut){
  if(!string.IsNullOrEmpty(arg) && arg.Trim().ToLower().Contains("reload")) SMELT_LoadCfg();

  double dt = System.Math.Max(0.0001, Runtime.TimeSinceLastRun.TotalSeconds);
  if(!_inited || dt>10){ foreach(var kv in _trackers) kv.Value.Reset(); _inited=true; }

  _pageTimer += dt; if(_pageTimer>=_pageSec){ _pageTimer=0; _page++; }

  // --- Scan ores across the construct ---
  _curOre.Clear();
  SMELT_ScanOres(_curOre);

  // --- Update per-ore trackers ---
  foreach(var kv in _curOre){
    SMELT_Tracker tr;
    if(!_trackers.TryGetValue(kv.Key, out tr)){ tr = new SMELT_Tracker(); _trackers[kv.Key]=tr; }
    tr.Update(kv.Value, dt);
  }

  // --- Build lines for UI ---
  var lines = new System.Collections.Generic.List<string>(200);
  lines.Add("ORE SMELTING STATUS");
  lines.Add("------------------------------------------");
  lines.Add("Grid: " + Me.CubeGrid.CustomName);
  lines.Add("");

  // Summary: total ore + overall ETA = max(ETA of ores being consumed)
  double totalKg = 0, overallSec = 0;
  foreach(var kv in _curOre) totalKg += kv.Value;

  foreach(var kv in _curOre){
    SMELT_Tracker tr;
    if(!_trackers.TryGetValue(kv.Key, out tr)) continue;
    double rps = tr.Rate;     // kg/s
    double amt = kv.Value;    // kg
    if(rps < -SMELT_RATE_EPS && amt>0){
      double sec = amt / (-rps);
      if(sec > overallSec) overallSec = sec;
    }
  }

  lines.Add("Summary");
  lines.Add("  Total ore: " + SMELT_Mass(totalKg));
  lines.Add("  Overall ETA (all done): " + (overallSec>0 ? SMELT_Time(overallSec) : "~0"));
  lines.Add("  Note: ETAs use live throughput.");
  lines.Add("");

  // Table header (fixed: removed invalid '>' from alignment)
  lines.Add(string.Format("{0,-12} {1,10}   {2,10}   {3,8}", "Ore", "Amount", "Rate(kg/m)", "ETA"));
  lines.Add("------------------------------------------");

  // Sort by amount desc
  var oreNames = new System.Collections.Generic.List<string>(_curOre.Keys);
  oreNames.Sort(delegate(string a, string b){
    double av = 0; _curOre.TryGetValue(a, out av);
    double bv = 0; _curOre.TryGetValue(b, out bv);
    int cmp = -av.CompareTo(bv);
    if(cmp!=0) return cmp;
    return string.Compare(a,b,System.StringComparison.OrdinalIgnoreCase);
  });

  for(int i=0;i<oreNames.Count;i++){
    string name = oreNames[i];
    double amt = 0; _curOre.TryGetValue(name, out amt);
    SMELT_Tracker tr; double rateS = 0;
    if(_trackers.TryGetValue(name, out tr)) rateS = tr.Rate; // kg/s
    double rateM = rateS * 60.0;
    string eta = "~0";
    if(System.Math.Abs(rateS) > SMELT_RATE_EPS){
      if(rateS < 0 && amt>0) eta = SMELT_Time(amt / (-rateS));
      else if(rateS > 0)     eta = "filling";
      else eta = "n/a";
    }
    string rateStr = SMELT_RateMin(rateM);
    // Row format (fixed: removed invalid '>' from alignment)
    lines.Add(string.Format("{0,-12} {1,10}   {2,10}   {3,8}", name, SMELT_MassTight(amt), rateStr, eta));
  }

  lines.Add("");
  lines.Add("Tip: tag any LCD/cockpit with [SMELT] (or [SMELT:0.55]).");
  lines.Add("ETAs are based on your live refinery throughput.");

  // --- Output: PB surface + tagged panels/surfaces ---
  var pb = Me.GetSurface(0);
  SMELT_RenderToSurface(pb, Me.CustomName, _fontPB, lines);

  _panels.Clear();
  GridTerminalSystem.GetBlocksOfType(_panels, p =>
    SMELT_Contains(p.CustomName, SMELT_DISPLAY_TAG.Substring(0,6)) &&
    (!SAME_CONSTRUCT_ONLY || p.IsSameConstructAs(Me)));
  for(int i=0;i<_panels.Count;i++){
    SMELT_RenderToSurface(_panels[i], _panels[i].CustomName, _fontLCD, lines);
  }

  _surfBlocks.Clear();
  GridTerminalSystem.GetBlocksOfType(_surfBlocks, b =>
    SMELT_Contains(b.CustomName, SMELT_DISPLAY_TAG.Substring(0,6)) &&
    (!SAME_CONSTRUCT_ONLY || b.IsSameConstructAs(Me)));
  for(int i=0;i<_surfBlocks.Count;i++){
    var blk = _surfBlocks[i];
    var sp = blk as IMyTextSurfaceProvider;
    if(sp==null || sp.SurfaceCount<=SURFACE_INDEX) continue;
    var srf = sp.GetSurface(SURFACE_INDEX);
    SMELT_RenderToSurface(srf, blk.CustomName, _fontLCD, lines);
  }
}

// ---- Scanning ---------------------------------------------------------
void SMELT_ScanOres(System.Collections.Generic.Dictionary<string,double> into){
  _invBlocks.Clear();
  GridTerminalSystem.GetBlocksOfType(_invBlocks, b => b.InventoryCount>0 && (!SAME_CONSTRUCT_ONLY || b.IsSameConstructAs(Me)));
  for(int bi=0; bi<_invBlocks.Count; bi++){
    var b = _invBlocks[bi];
    for(int ii=0; ii<b.InventoryCount; ii++){
      var inv = b.GetInventory(ii); if(inv==null) continue;
      _items.Clear(); inv.GetItems(_items);
      for(int k=0;k<_items.Count;k++){
        var it = _items[k];
        if(it.Type.TypeId == "MyObjectBuilder_Ore"){
          string ore = it.Type.SubtypeId; if(string.IsNullOrEmpty(ore)) ore="Ore";
          double cur = 0; into.TryGetValue(ore, out cur);
          cur += SMELT_ToDouble(it.Amount);
          into[ore] = cur;
        }
      }
    }
  }
}

// ---- Rendering / Auto-fit / Pagination --------------------------------
void SMELT_RenderToSurface(IMyTextSurface srf, string name, double baseFont, System.Collections.Generic.List<string> lines){
  double font = SMELT_ParseSize(name, baseFont);
  bool forced = SMELT_HasExplicitSize(name);

  if(!forced && _autoFit){
    double f = font;
    while(f > _minFont && SMELT_LinesCapacity(srf, f) < 12) f -= 0.02;
    font = f < _minFont ? _minFont : f;
  }

  srf.ContentType = ContentType.TEXT_AND_IMAGE;
  srf.Font = "Monospace";
  srf.FontSize = (float)SMELT_Clamp(font, 0.3, 2.0);
  srf.Alignment = TextAlignment.LEFT;
  srf.TextPadding = 0.3f;

  int cap = System.Math.Max(8, SMELT_LinesCapacity(srf, (double)srf.FontSize));
  int pages = (lines.Count + cap - 1) / cap; if(pages<1) pages=1;
  int pi = _page % pages; int st = pi*cap; int en = System.Math.Min(lines.Count, st+cap);

  _sb.Clear();
  for(int i=st;i<en;i++) _sb.AppendLine(lines[i]);
  _sb.AppendLine();
  _sb.Append("Font ").Append(((double)srf.FontSize).ToString("0.00")).Append("  ");
  _sb.Append("Panel tag: [SMELT] / [SMELT:0.55]  PB arg: reload");
  srf.WriteText(_sb.ToString(), false);
}

int SMELT_LinesCapacity(IMyTextSurface s, double f){
  var v = s.MeasureStringInPixels(new System.Text.StringBuilder("A"), s.Font, (float)f);
  float h = v.Y <= 0 ? 20f : v.Y * 1.05f;
  var sz = s.SurfaceSize;
  int lines = (int)(sz.Y / h);
  if(lines < 6) lines = 6;
  return lines;
}

// ---- Helpers ----------------------------------------------------------
class SMELT_Tracker{
  public double Last = double.NaN; // kg
  public double Rate = 0.0;        // kg/s EMA
  public void Update(double cur, double dt){
    if(double.IsNaN(Last) || dt<=0){ Last = cur; return; }
    double inst = (cur - Last) / dt;
    if(System.Math.Abs(Rate) < SMELT_RATE_EPS) Rate = inst;
    else Rate = SMELT_EMA*inst + (1.0 - SMELT_EMA)*Rate;
    Last = cur;
  }
  public void Reset(){ Rate = 0.0; }
}

void SMELT_LoadCfg(){
  _fontLCD = SMELT_FONT_LCD_DEF; _fontPB = SMELT_FONT_PB_DEF; _minFont = SMELT_MIN_FONT_DEF; _autoFit = SMELT_AUTOFIT_DEF; _pageSec = SMELT_PAGE_SEC_DEF;
  string cd = Me.CustomData; if(string.IsNullOrWhiteSpace(cd)) return;
  var L = cd.Split(new char[]{'\n','\r'}, System.StringSplitOptions.RemoveEmptyEntries);
  bool inSec=false;
  for(int i=0;i<L.Length;i++){
    string raw = L[i].Trim();
    if(raw.StartsWith("[") && raw.EndsWith("]")){ inSec = raw.ToUpper()=="[SMELT]"; continue; }
    if(!inSec) continue;
    int eq = raw.IndexOf('='); if(eq<=0) continue;
    string key = raw.Substring(0,eq).Trim();
    string val = raw.Substring(eq+1).Trim();
    double num;
    if(SMELT_KeyEq(key,"FontSize")     && SMELT_TryNum(val,out num)) _fontLCD = SMELT_Clamp(num,0.3,2.0);
    else if(SMELT_KeyEq(key,"PbFontSize") && SMELT_TryNum(val,out num)) _fontPB = SMELT_Clamp(num,0.3,2.0);
    else if(SMELT_KeyEq(key,"MinFont")    && SMELT_TryNum(val,out num)) _minFont = SMELT_Clamp(num,0.3,2.0);
    else if(SMELT_KeyEq(key,"AutoFit"))   _autoFit = SMELT_ParseBool(val,true);
    else if(SMELT_KeyEq(key,"PageSeconds")&& SMELT_TryNum(val,out num)) _pageSec = System.Math.Max(2,num);
  }
}

void SMELT_LoadState(){
  if(string.IsNullOrWhiteSpace(Storage)) return;
  try{
    var L = Storage.Split(new char[]{'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
    for(int i=0;i<L.Length;i++){
      var p = L[i].Split('|'); if(p.Length!=4) continue;
      if(p[0] != "ORE") continue;
      string name = p[1];
      double last=0, rate=0;
      double.TryParse(p[2], out last);
      double.TryParse(p[3], out rate);
      SMELT_Tracker tr;
      if(!_trackers.TryGetValue(name, out tr)){ tr=new SMELT_Tracker(); _trackers[name]=tr; }
      tr.Last = last; tr.Rate = rate;
    }
  }catch{}
}

bool SMELT_KeyEq(string a,string b){ return string.Equals(a,b,System.StringComparison.OrdinalIgnoreCase); }
bool SMELT_Contains(string s,string q){ return !string.IsNullOrEmpty(s) && s.IndexOf(q,System.StringComparison.OrdinalIgnoreCase)>=0; }
double SMELT_ToDouble(MyFixedPoint fp){ return (double)fp; }

string SMELT_Mass(double kg){
  if(kg >= 1000000) return (kg/1000000).ToString("0.00")+"kt";
  if(kg >= 1000)    return (kg/1000).ToString("0.00")+"t";
  return kg.ToString("0")+"kg";
}
string SMELT_MassTight(double kg){
  if(kg >= 1000000) return (kg/1000000).ToString("0.0")+"kt";
  if(kg >= 1000)    return (kg/1000).ToString("0.0")+"t";
  return kg.ToString("0");
}
string SMELT_RateMin(double kgm){
  double a = System.Math.Abs(kgm);
  string sign = kgm<0 ? "-" : (kgm>0?"+":"~");
  if(a >= 1000000) return sign+(a/1000000).ToString("0.0")+"M";
  if(a >= 1000)    return sign+(a/1000).ToString("0.0")+"k";
  return sign+a.ToString("0");
}
string SMELT_Time(double s){
  if(double.IsInfinity(s) || double.IsNaN(s)) return "n/a";
  if(s < 60) return s.ToString("0")+"s";
  double m = s/60; if(m < 60) return m.ToString("0")+"m";
  double h = m/60; if(h < 48) return h.ToString("0.0")+"h";
  double d = h/24; return d.ToString("0.0")+"d";
}

double SMELT_Clamp(double v,double lo,double hi){ if(v<lo) return lo; if(v>hi) return hi; return v; }
bool SMELT_TryNum(string s,out double d){ if(double.TryParse(s,out d)) return true; s=s.Replace(',','.'); return double.TryParse(s,out d); }
bool SMELT_ParseBool(string s,bool def){ if(string.IsNullOrEmpty(s)) return def; s=s.Trim().ToLower(); if(s=="true"||s=="1"||s=="yes"||s=="y") return true; if(s=="false"||s=="0"||s=="no"||s=="n") return false; return def; }
double SMELT_ParseSize(string n,double def){
  int i=n.IndexOf(SMELT_DISPLAY_TAG.Substring(0,6), System.StringComparison.OrdinalIgnoreCase);
  if(i<0) return def;
  int colon = n.IndexOf(':', i);
  if(colon>=0){
    int end = n.IndexOf(']', colon+1);
    if(end>colon+1){
      double v; if(SMELT_TryNum(n.Substring(colon+1,end-(colon+1)), out v)) return SMELT_Clamp(v,0.3,2.0);
    }
  }
  return def;
}
bool SMELT_HasExplicitSize(string n){
  int i=n.IndexOf(SMELT_DISPLAY_TAG.Substring(0,6), System.StringComparison.OrdinalIgnoreCase);
  if(i<0) return false;
  int colon = n.IndexOf(':', i);
  return colon>=0;
}
