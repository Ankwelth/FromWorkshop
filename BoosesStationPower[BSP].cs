// Station Self-Power Manager — simple + grouped + connector-safe + debug
// v1.3.3
// - Local grid only (no control through connectors).
// - Optional groups for Batteries / Tanks / Engines / Reactors.
// - Hydrogen engines first; reactors only if engines can't raise charge within GRACE.
// - [BSP] in block name = ignored.
// - Battery% fix: counts all batteries regardless of ChargeMode.
// - Debug panel: shows what the script is doing.

const string SCRIPT = "Station Self-Power Manager v1.3.3";

// ---- Custom Data Keys (auto-populated once; then read-only) ----
const string CD_BAT_GROUP        = "BATTERY GROUP";        const string DEF_BAT_GROUP = "";
const string CD_TANK_GROUP       = "TANK GROUP";           const string DEF_TANK_GROUP = "";
const string CD_ENG_GROUP        = "ENGINE GROUP";         const string DEF_ENG_GROUP = "";
const string CD_REA_GROUP        = "REACTOR GROUP";        const string DEF_REA_GROUP = "";

const string CD_USE_ENGINES      = "USE ENGINES";          const bool   DEF_USE_ENGINES = true;
const string CD_USE_REACTORS     = "USE REACTORS";         const bool   DEF_USE_REACTORS = true;
const string CD_START_BAT_PCT    = "START BATTERY PERCENT";const float  DEF_START = 0.25f;
const string CD_STOP_BAT_PCT     = "STOP BATTERY PERCENT"; const float  DEF_STOP  = 0.50f;
const string CD_MIN_H2_PCT       = "MIN H2 PERCENT";       const float  DEF_MINH2 = 0.05f;
const string CD_ENGINE_GRACE_SEC = "ENGINE GRACE SECONDS"; const int    DEF_GRACE = 10;
const string CD_LOOP_SEC         = "LOOP PERIOD SECONDS";  const float  DEF_LOOP  = 2f;
const string CD_SCAN_SEC         = "SCAN PERIOD SECONDS";  const float  DEF_SCAN  = 10f;

// NEW: Debug + Displays
const string CD_DEBUG_ENABLED    = "DEBUG ENABLED";        const bool   DEF_DEBUG   = true;
const string CD_DEBUG_LEVEL      = "DEBUG LEVEL";          const int    DEF_DLVL    = 2;     // 1 basic, 2 verbose
const string CD_DISPLAY_TAG      = "DISPLAY TAG";          const string DEF_DTAG    = "StationPower";

const string OPT_OUT             = "[BSP]";

// ---- Internals ----
string _gBat,_gTank,_gEng,_gRea;
bool _useEng,_useRea,_dbg;
int  _dlvl;
string _dTag;
float _startPct,_stopPct,_minH2,_loopSec,_scanSec;
int _grace;

readonly List<IMyBatteryBlock>  _bats = new List<IMyBatteryBlock>();
readonly List<IMyGasTank>       _tanks= new List<IMyGasTank>();
readonly List<IMyPowerProducer> _engs = new List<IMyPowerProducer>();
readonly List<IMyPowerProducer> _reas = new List<IMyPowerProducer>();
readonly List<IMyTextSurface>   _surfaces = new List<IMyTextSurface>();

double _accLoop=0,_accScan=0;
DateTime _askedEnginesAt = DateTime.MinValue;
double _lastCharge = -1;
long _gridId;

// Debug ring buffer
const int LOG_MAX = 30;
readonly Queue<string> _log = new Queue<string>(LOG_MAX);
void Log(string msg){
    if(!_dbg) return;
    string line = $"{DateTime.Now:HH:mm:ss} {msg}";
    if(_log.Count >= LOG_MAX) _log.Dequeue();
    _log.Enqueue(line);
    Echo(line);
}

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update10; // ~6/s
    _gridId = Me.CubeGrid.EntityId;
    ReadConfig(true);
    ScanBlocks();
    ScanDisplays();
    Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    Log($"{SCRIPT} started.");
}

public void Save(){}

public void Main(string arg, UpdateType ut){
    // Commands
    if(!string.IsNullOrWhiteSpace(arg)){
        var a = arg.Trim().ToLower();
        if(a=="refresh"){ ReadConfig(false); ScanBlocks(); ScanDisplays(); Log("Refreshed config & rescan."); }
        else if(a=="debug on"){ _dbg = true; Log("Debug ON"); }
        else if(a=="debug off"){ Log("Debug OFF"); _dbg=false; }
        else if(a=="groups"){ DumpGroups(); }
    }

    _accLoop += Runtime.TimeSinceLastRun.TotalSeconds;
    _accScan += Runtime.TimeSinceLastRun.TotalSeconds;

    if(_accScan >= _scanSec){ _accScan=0; ScanBlocks(); }

    if(_accLoop < _loopSec){ Render(); return; }
    _accLoop = 0;

    if(_bats.Count==0){
        Log("No batteries managed. Check BATTERY GROUP or grid membership.");
        Render("No managed batteries. Set BATTERY GROUP or add batteries on this grid.");
        return;
    }

    // Metrics
    float batPct = BatteryChargePct(_bats);
    float h2Pct  = HydrogenPct(_tanks);
    double engOut = TotalOutput(_engs);
    double reaOut = TotalOutput(_reas);

    if(_dlvl>=2) Log($"Metrics: Bat {(batPct*100):0.0}%  H2 {(h2Pct*100):0.0}%  EngOut {engOut:0.00}MW  ReaOut {reaOut:0.00}MW");

    // Logic
    bool needHelp = batPct < _startPct;

    if(needHelp){
        bool enginesOK = _useEng && _engs.Count>0 && h2Pct >= _minH2;
        if(enginesOK){
            SetEnabled(_engs,true);
            if(_askedEnginesAt==DateTime.MinValue){ _askedEnginesAt = DateTime.Now; Log("Engines ON (low battery)."); }
        }else{
            if(_engs.Count==0) Log("No engines available (or all opted-out).");
            else if(h2Pct < _minH2) Log($"Engines skipped (H2 below MIN H2 {(_minH2*100):0.0}%).");
            _askedEnginesAt = DateTime.MinValue;
        }

        bool chargeImproving = (_lastCharge>=0 && batPct > _lastCharge + 0.0005f);
        bool graceElapsed    = (_askedEnginesAt!=DateTime.MinValue) && (DateTime.Now - _askedEnginesAt).TotalSeconds >= _grace;
        bool enginesWeak     = (engOut < 0.01);

        bool needReactors = _useRea && (!enginesOK || enginesWeak || (graceElapsed && !chargeImproving));
        if(needReactors){
            SetEnabled(_reas,true);
            Log($"Reactors ON ({(enginesWeak?"engines weak":"grace elapsed")}).");
        }
    }

    // Stop assistance
    if(batPct > _stopPct){
        if(_useRea && CountOn(_reas)>0){ SetEnabled(_reas,false); Log("Reactors OFF (battery above STOP)."); }
        if(_useEng && CountOn(_engs)>0){ SetEnabled(_engs,false); Log("Engines OFF (battery above STOP)."); }
        _askedEnginesAt = DateTime.MinValue;
    }

    _lastCharge = batPct;
    Render();
}

// ---------------- Rendering ----------------
void Render(string overrideMessage=null){
    var sb = new StringBuilder();
    sb.AppendLine(SCRIPT);
    sb.AppendLine($"Time: {DateTime.Now:HH:mm:ss}");
    sb.AppendLine();

    if(overrideMessage==null){
        float bat = BatteryChargePct(_bats);
        float h2  = HydrogenPct(_tanks);
        double eO = TotalOutput(_engs);
        double rO = TotalOutput(_reas);

        sb.AppendLine($"Batteries : {(bat*100):0.0}%");
        sb.AppendLine($"Hydrogen  : {(h2*100):0.0}%");
        sb.AppendLine($"Engines   : {_engs.Count} (ON {CountOn(_engs)}) {eO:0.0} MW");
        sb.AppendLine($"Reactors  : {_reas.Count} (ON {CountOn(_reas)}) {rO:0.0} MW");
        sb.AppendLine();
        sb.AppendLine($"START: {(_startPct*100):0}%   STOP: {(_stopPct*100):0}%   MIN H₂: {(_minH2*100):0}%   GRACE: {_grace}s");
        sb.AppendLine();
    }else{
        sb.AppendLine(overrideMessage);
        sb.AppendLine();
    }

    if(_dbg){
        sb.AppendLine("[ Debug ]");
        if(_dlvl>=1){
            sb.AppendLine($"BATTERY GROUP : {(_gBat==""?"<auto-scan>":_gBat)}  -> {_bats.Count}");
            sb.AppendLine($"TANK GROUP    : {(_gTank==""?"<auto-scan>":_gTank)} -> {_tanks.Count}");
            sb.AppendLine($"ENGINE GROUP  : {(_gEng==""?"<auto-scan>":_gEng)}  -> {_engs.Count}");
            sb.AppendLine($"REACTOR GROUP : {(_gRea==""?"<auto-scan>":_gRea)}  -> {_reas.Count}");
        }
        sb.AppendLine();
        foreach(var line in _log) sb.AppendLine(line);
    }

    var text = sb.ToString();
    Me.GetSurface(0).WriteText(text, false);
    foreach(var s in _surfaces){ try{ s.WriteText(text,false);}catch{} }
}

// ---------------- Scanning ----------------
bool Local(IMyTerminalBlock b){
    return b.CubeGrid?.EntityId == _gridId; // strict: same physical grid only (never across connectors)
}
bool Managed(IMyTerminalBlock b){
    return b != null && b.CustomName.IndexOf(OPT_OUT, StringComparison.OrdinalIgnoreCase) < 0;
}

void ScanBlocks(){
    _bats.Clear(); _tanks.Clear(); _engs.Clear(); _reas.Clear();

    GetGroupOrScan(_gBat, _bats,  (IMyBatteryBlock b)=> Local(b) && Managed(b));
    GetGroupOrScan(_gTank,_tanks, (IMyGasTank t)=> Local(t)&&Managed(t)&&t.IsFunctional&&
        t.BlockDefinition.SubtypeId.ToString().IndexOf("Hydrogen",StringComparison.OrdinalIgnoreCase)>=0);
    GetGroupOrScan(_gEng, _engs,  (IMyPowerProducer p)=> Local(p)&&Managed(p)&&p.IsFunctional&&
        p.BlockDefinition.TypeIdString.IndexOf("HydrogenEngine",StringComparison.OrdinalIgnoreCase)>=0);
    GetGroupOrScan(_gRea, _reas,  (IMyPowerProducer p)=> Local(p)&&Managed(p)&&p.IsFunctional&&(p is IMyReactor));

    if(_dlvl>=1){
        Log($"Scan -> Bats:{_bats.Count} Tanks:{_tanks.Count} Eng:{_engs.Count} Rea:{_reas.Count}");
    }
}

void ScanDisplays(){
    _surfaces.Clear();
    GridTerminalSystem.GetBlocksOfType(_surfaces, s=>{
        var tb=s as IMyTerminalBlock; if(tb==null) return false;
        if(!Local(tb) || !Managed(tb)) return false;
        if(tb.CustomName.IndexOf(_dTag, StringComparison.OrdinalIgnoreCase)>=0){
            s.ContentType=ContentType.TEXT_AND_IMAGE; return true;
        }
        return false;
    });
}

// Group helper: try exact first, then case-insensitive
IMyBlockGroup TryGetGroup(string name){
    if(string.IsNullOrWhiteSpace(name)) return null;
    var g = GridTerminalSystem.GetBlockGroupWithName(name);
    if(g!=null) return g;
    // case-insensitive fallback search
    var groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);
    foreach(var gg in groups){
        if(string.Equals(gg.Name, name, StringComparison.OrdinalIgnoreCase)) return gg;
    }
    return null;
}

// Fill list by group or by scan; logs what happened
void GetGroupOrScan<T>(string group, List<T> into, Func<T,bool> pred) where T: class, IMyTerminalBlock {
    if(!string.IsNullOrWhiteSpace(group)){
        var g = TryGetGroup(group);
        if(g!=null){
            var tmp = new List<T>(); g.GetBlocksOfType(tmp, pred);
            into.AddRange(tmp);
            Log($"Group '{group}' found -> {into.Count} {typeof(T).Name.Replace("IMy","")}");
            if(into.Count==0) Log($"WARNING: Group '{group}' has 0 matching blocks on THIS grid or all opted-out.");
            return;
        }else{
            Log($"Group '{group}' NOT found. Scanning local grid instead.");
            SuggestGroups();
        }
    }
    GridTerminalSystem.GetBlocksOfType(into, pred);
    Log($"Scan local grid -> {into.Count} {typeof(T).Name.Replace("IMy","")}");
}

// Show some available group names to help diagnose typos
void SuggestGroups(){
    var names = new List<string>();
    var all = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(all);
    foreach(var g in all) names.Add(g.Name);
    if(names.Count==0){ Log("No block groups exist on this grid."); return; }
    int show = Math.Min(8, names.Count);
    var sb = new StringBuilder("Groups on grid: ");
    for(int i=0;i<show;i++){ if(i>0) sb.Append(" | "); sb.Append(names[i]); }
    if(names.Count>show) sb.Append(" | ...");
    Log(sb.ToString());
}

// Dump all groups on command
void DumpGroups(){
    var all = new List<IMyBlockGroup>(); GridTerminalSystem.GetBlockGroups(all);
    if(all.Count==0){ Log("No groups on grid."); return; }
    Log($"Groups ({all.Count}):");
    int i=0; foreach(var g in all){ Log($"  {++i}. {g.Name}"); }
}

// ---------------- Metrics ----------------
float BatteryChargePct(List<IMyBatteryBlock> list){
    double cur=0,max=0;
    foreach(var b in list){ cur+=b.CurrentStoredPower; max+=b.MaxStoredPower; }
    return (max>0)?(float)(cur/max):0f;
}
float HydrogenPct(List<IMyGasTank> list){
    if(list.Count==0) return 0f;
    double fill=0,cap=0;
    foreach(var t in list){ fill+=t.Capacity*t.FilledRatio; cap+=t.Capacity; }
    return (cap>0)?(float)(fill/cap):0f;
}
double TotalOutput(List<IMyPowerProducer> list){ double s=0; foreach(var p in list) s+=p.CurrentOutput; return s; }
int CountOn(List<IMyPowerProducer> list){ int n=0; foreach(var p in list) if(p.Enabled) n++; return n; }
void SetEnabled(List<IMyPowerProducer> list,bool on){ foreach(var p in list){ if(!p.IsFunctional) continue; p.Enabled=on; } }

// ---------------- Config ----------------
void ReadConfig(bool init){
    var cd = Me.CustomData ?? "";

    _gBat   = GetString(cd,CD_BAT_GROUP,DEF_BAT_GROUP,init);
    _gTank  = GetString(cd,CD_TANK_GROUP,DEF_TANK_GROUP,init);
    _gEng   = GetString(cd,CD_ENG_GROUP,DEF_ENG_GROUP,init);
    _gRea   = GetString(cd,CD_REA_GROUP,DEF_REA_GROUP,init);

    _useEng = GetBool(cd,CD_USE_ENGINES,DEF_USE_ENGINES,init);
    _useRea = GetBool(cd,CD_USE_REACTORS,DEF_USE_REACTORS,init);

    _startPct = Clamp01(GetFloat(cd,CD_START_BAT_PCT,DEF_START,init));
    _stopPct  = Clamp01(GetFloat(cd,CD_STOP_BAT_PCT, DEF_STOP, init));
    _minH2    = Clamp01(GetFloat(cd,CD_MIN_H2_PCT,   DEF_MINH2,init));
    _grace    = Math.Max(0,(int)GetFloat(cd,CD_ENGINE_GRACE_SEC,DEF_GRACE,init));

    _loopSec  = Math.Max(0.2f,GetFloat(cd,CD_LOOP_SEC,DEF_LOOP,init));
    _scanSec  = Math.Max(0.5f,GetFloat(cd,CD_SCAN_SEC,DEF_SCAN,init));

    _dbg      = GetBool(cd,CD_DEBUG_ENABLED,DEF_DEBUG,init);
    _dlvl     = Math.Max(1,(int)GetFloat(cd,CD_DEBUG_LEVEL,DEF_DLVL,init));
    _dTag     = GetString(cd,CD_DISPLAY_TAG,DEF_DTAG,init);

    // On first run, write a tidy header with quotes ready for group names
    if(init){
        Me.CustomData =
$@"# ===== Station Self-Power Manager Settings =====
# Fill group names inside quotes ("")
# Leave "" to auto-scan this grid

{CD_BAT_GROUP}: ""{_gBat}""
{CD_TANK_GROUP}: ""{_gTank}""
{CD_ENG_GROUP}: ""{_gEng}""
{CD_REA_GROUP}: ""{_gRea}""

{CD_USE_ENGINES}: {_useEng}
{CD_USE_REACTORS}: {_useRea}
{CD_START_BAT_PCT}: {_startPct:0.##}
{CD_STOP_BAT_PCT}: {_stopPct:0.##}
{CD_MIN_H2_PCT}: {_minH2:0.##}
{CD_ENGINE_GRACE_SEC}: {_grace}
{CD_LOOP_SEC}: {_loopSec:0.##}
{CD_SCAN_SEC}: {_scanSec:0.##}

{CD_DEBUG_ENABLED}: {_dbg}
{CD_DEBUG_LEVEL}: {_dlvl}
{CD_DISPLAY_TAG}: {_dTag}
";
    }
}

string GetString(string src,string key,string def,bool init){
    var v=ReadLine(src,key);
    if(v==null){ if(init) Append($"{key}: \"{def}\""); return def; }
    v=v.Trim();
    if(v.Length>=2 && v[0]=='\"' && v[v.Length-1]=='\"') v=v.Substring(1,v.Length-2);
    return v;
}
bool GetBool(string src,string key,bool def,bool init){
    var v=ReadLine(src,key); bool b;
    if(v==null || !bool.TryParse(v,out b)){ if(init) Append($"{key}: {def}"); return def; }
    return b;
}
float GetFloat(string src,string key,float def,bool init){
    var v=ReadLine(src,key); float f;
    if(v==null || !float.TryParse(v,out f)){ if(init) Append($"{key}: {def:0.##}"); return def; }
    return f;
}
string ReadLine(string src,string key){
    foreach(var line in (src??"").Split('\n')){
        if(line.TrimStart().StartsWith(key, StringComparison.OrdinalIgnoreCase)){
            int i=line.IndexOf(':'); if(i>=0 && i+1<line.Length) return line.Substring(i+1).Trim();
        }
    }
    return null;
}
void Append(string line){
    if(!string.IsNullOrEmpty(Me.CustomData) && !Me.CustomData.EndsWith("\n")) Me.CustomData += "\n";
    Me.CustomData += line + "\n";
}
float Clamp01(float x){ return x<0?0:(x>1?1:x); }
