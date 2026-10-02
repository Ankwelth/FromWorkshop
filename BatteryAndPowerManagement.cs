// BATTERY & POWER MANAGEMENT SCRIPT v6.2

// ======================================================================================= 
//                                 CONFIGURATION SECTION 
// ======================================================================================= 

string lcdNameTag = "[PowerStats]"; 
float PB_FONT_SIZE = 0.8f;  

bool autoControlGenerators = true; 
float H2_OFF_PERCENT = 0.95f;  
float uraniumEnergyDensity = 1.0f;

bool ENABLE_SOLAR_ALIGN = true; 
string rotorTag = "[SolarRotor]";  

bool SHUTDOWN_NON_ESSENTIAL = false;  

// -- ALERT SETTINGS -- 
float CRIT_URANIUM = 5.0f; 
float CRIT_H2 = 10.0f;     

// -- POWER TIERS (Battery %) -- 
float T1_ON = 0.80f; float T1_OFF = 0.95f; 
float T2_ON = 0.40f; float T2_OFF = 0.60f; 
float T3_ON = 0.15f; float T3_OFF = 0.30f; 

// ======================================================================================= 
//                                END OF CONFIGURATION 
// ======================================================================================= 

List<IMyTerminalBlock> gridBlocks = new List<IMyTerminalBlock>(); 
List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>(); 
List<IMyPowerProducer> reactors = new List<IMyPowerProducer>();  
List<IMyPowerProducer> engines = new List<IMyPowerProducer>(); 
List<IMyPowerProducer> auxGenerators = new List<IMyPowerProducer>();
List<IMySolarPanel> solars = new List<IMySolarPanel>(); 
List<IMyWindTurbine> winds = new List<IMyWindTurbine>(); 
List<IMyGasTank> h2Tanks = new List<IMyGasTank>(); 
List<IMyTextPanel> lcds = new List<IMyTextPanel>(); 
List<IMyMotorStator> solarRotors = new List<IMyMotorStator>(); 
List<IMyFunctionalBlock> production = new List<IMyFunctionalBlock>(); 

string activeTierName; 

public Program() { Runtime.UpdateFrequency = UpdateFrequency.Update100; } 

public void Main(string args, UpdateType updateSource) 
{ 
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(gridBlocks, b => b.IsSameConstructAs(Me)); 
    FilterAndAutoClassify(); 

    float batStored, batMaxStored, batInput, batOutput, batMaxOutput, batMaxInput; 
    ProcessBatteries(out batStored, out batMaxStored, out batInput, out batOutput, out batMaxOutput, out batMaxInput); 
    float batPercent = (batMaxStored > 0) ? batStored / batMaxStored : 0f; 
     
    double fReact, fCargo; CountUraniumSplit(out fReact, out fCargo); 
    double totalUranium = fReact + fCargo;
    double h2Cur, h2Max; GetHydrogenStats(out h2Cur, out h2Max); 
    double h2Pct = (h2Max > 0) ? (h2Cur / h2Max) * 100 : 0; 

    ManageAutoTiers(batPercent); 
    if (ENABLE_SOLAR_ALIGN) AlignSolar(); 

    float reactCur, reactMax, engCur, engMax, solarCur, solarPot, windCur, windPot, auxCur, auxMax; 
    ProcessGenList(reactors, out reactCur, out reactMax); 
    ProcessGenList(engines, out engCur, out engMax); 
    ProcessGenList(auxGenerators, out auxCur, out auxMax);
    ProcessRenewables(out solarCur, out solarPot, out windCur, out windPot); 

    float greenTotal = solarCur + windCur;
    float greenPercent = (batOutput > 0) ? Math.Min(100, (greenTotal / batOutput) * 100) : 100f; 

    StringBuilder sb = new StringBuilder(); 
    bool isCrit = (totalUranium < CRIT_URANIUM) || (h2Pct < CRIT_H2 && engines.Count > 0); 
     
    sb.AppendLine(isCrit ? "!!! FUEL CRITICAL !!!" : "=== POWER MANAGER v6.2 ==="); 
    sb.AppendLine($"\n>>> STATUS: {activeTierName} <<<"); 
    sb.AppendLine($" Battery: {(batPercent*100):0.0}% | Green: {greenPercent:0.0}%"); 

    // GREEN ENERGY
    if (solars.Count > 0 || winds.Count > 0) { 
        sb.AppendLine($" Solar ({solars.Count}) / Wind ({winds.Count})");
        sb.AppendLine($" Out: {FormatMW(greenTotal)} / {FormatMW(solarPot + windPot)}"); 
    } 

    // ENGINES
    if (engines.Count > 0) { 
        string engTime = GetH2Time(h2Cur, engCur);
        sb.AppendLine($"\n[ H2 ENGINES ({engines.Count}) ]"); 
        sb.AppendLine($" Out: {FormatMW(engCur)} / {FormatMW(engMax)}"); 
        sb.AppendLine($" H2: {h2Pct:0.0}% ({engTime})"); 
    } 

    // AUX POWER
    if (auxGenerators.Count > 0) {
        sb.AppendLine($"\n[ AUX POWER ({auxGenerators.Count}) ]");
        sb.AppendLine($" Out: {FormatMW(auxCur)} / {FormatMW(auxMax)}");
    }

    // REACTORS
    if (reactors.Count > 0) { 
        string rTime = GetReactTime(totalUranium, reactCur, reactMax);
        sb.AppendLine($"\n[ REACTORS ({reactors.Count}) ]"); 
        sb.AppendLine($" Out: {FormatMW(reactCur)} / {FormatMW(reactMax)}"); 
        sb.AppendLine($" Fuel: {totalUranium:N1}kg ({rTime})");
        sb.AppendLine($"   (React: {fReact:N1} | Cargo: {fCargo:N1})"); 
    } 

    // BATTERIES
    float net = batInput - batOutput; 
    sb.AppendLine($"\n[ BATTERY ARRAY ({batteries.Count}) ]"); 
    // RESTORED READOUT HERE
    sb.AppendLine($" Cap: {batStored:0.1} / {batMaxStored:0.1} MWh");
    sb.AppendLine($" Net: {(net >= 0 ? "+" : "")}{FormatMW(net)}"); 
    
    if(net < -0.001f) sb.AppendLine($" Draining: {FormatTime(batStored / Math.Abs(net))}"); 
    else if (net > 0.001f && batPercent < 0.99f) sb.AppendLine($" Charging: {FormatTime((batMaxStored - batStored) / net)}"); 
    else sb.AppendLine(" Status: Sustained");

    WriteOutput(sb.ToString(), isCrit); 
} 

// --- CORE LOGIC ---

void FilterAndAutoClassify() { 
    batteries.Clear(); reactors.Clear(); engines.Clear(); solars.Clear(); winds.Clear();  
    h2Tanks.Clear(); lcds.Clear(); solarRotors.Clear(); production.Clear(); auxGenerators.Clear();
    foreach(var b in gridBlocks) { 
        if(b is IMyBatteryBlock) batteries.Add(b as IMyBatteryBlock); 
        else if(b is IMySolarPanel) solars.Add(b as IMySolarPanel); 
        else if(b is IMyWindTurbine) winds.Add(b as IMyWindTurbine); 
        else if(b is IMyGasTank && b.BlockDefinition.SubtypeId.Contains("Hydrogen")) h2Tanks.Add(b as IMyGasTank); 
        else if(b is IMyMotorStator && (b.CustomName.Contains(rotorTag) || b.BlockDefinition.ToString().Contains("Solar"))) solarRotors.Add(b as IMyMotorStator); 
        else if(b is IMyRefinery || b is IMyAssembler) production.Add(b as IMyFunctionalBlock); 
        else if(b is IMyPowerProducer) { 
            var gen = b as IMyPowerProducer; string type = gen.BlockDefinition.ToString();
            if(type.Contains("Reactor")) reactors.Add(gen);
            else if(type.Contains("Engine")) engines.Add(gen);
            else auxGenerators.Add(gen);
        } 
        if(b is IMyTextPanel && b.CustomName.Contains(lcdNameTag)) lcds.Add(b as IMyTextPanel); 
    } 
} 

void ManageAutoTiers(float batPct) { 
    if (!autoControlGenerators) { activeTierName = "MANUAL"; return; }
    bool t3_req = batPct < (IsTierActive(3) ? T3_OFF : T3_ON);
    bool t2_req = batPct < (IsTierActive(2) ? T2_OFF : T2_ON);
    bool t1_req = batPct < (IsTierActive(1) ? T1_OFF : T1_ON);
    if (t3_req) { SetGenState(reactors, true); SetGenState(engines, true); SetGenState(auxGenerators, true); activeTierName = "TIER 3 (EMG)"; } 
    else if (t2_req) { SetGenState(reactors, false); SetGenState(engines, true); SetGenState(auxGenerators, true); activeTierName = "TIER 2 (BACKUP)"; } 
    else if (t1_req) { SetGenState(reactors, false); SetGenState(engines, false); SetGenState(auxGenerators, true); activeTierName = "TIER 1 (MAIN)"; } 
    else { SetGenState(reactors, false); SetGenState(engines, false); SetGenState(auxGenerators, false); activeTierName = "IDLE (GREEN)"; } 
    if (batPct >= H2_OFF_PERCENT) foreach(var e in engines) e.Enabled = false; 
    if (SHUTDOWN_NON_ESSENTIAL) { bool ps = batPct > T3_ON; foreach(var p in production) if(p.Enabled != ps) p.Enabled = ps; }
} 

void SetGenState(List<IMyPowerProducer> list, bool state) { foreach(var g in list) if(g.Enabled != state) g.Enabled = state; }
bool IsTierActive(int tier) {
    if (tier == 3) return reactors.Any(r => r.Enabled);
    if (tier == 2) return engines.Any(e => e.Enabled);
    if (tier == 1) return auxGenerators.Any(a => a.Enabled);
    return false;
}

// --- HELPERS ---
string GetH2Time(double liters, float current) {
    if (liters <= 0) return "N/A";
    float totalBurn = 0;
    foreach(var e in engines) totalBurn += (e.CubeGrid.GridSizeEnum == MyCubeSize.Large ? 500f : 80f);
    float hours = (float)(liters / ((current > 0.01f ? current : totalBurn) * 3600));
    return (current > 0.01f) ? FormatTime(hours) : "Est." + FormatTime(hours);
}

string GetReactTime(double fuel, float cur, float max) {
    if (fuel <= 0) return "NO FUEL";
    float burnMW = (cur > 0.01f) ? cur : max;
    float hours = (float)(fuel / (burnMW / uraniumEnergyDensity));
    return (cur > 0.01f) ? FormatTime(hours) : "Est." + FormatTime(hours);
}

void AlignSolar() { if (GridTerminalSystem.GetBlockWithName("Solar Rotor Controller") != null) return; foreach(var r in solarRotors) { float cm = 0; foreach(var s in solars) if(s.IsSameConstructAs(r)) cm += s.MaxOutput; if (cm < (solars.Count * 0.12f)) r.TargetVelocityRPM = 0.1f; else r.TargetVelocityRPM = 0f; } }
void CountUraniumSplit(out double r, out double c) { r=0; c=0; foreach(var b in gridBlocks) { if(b.HasInventory) { var inv = b.GetInventory(0); var items = new List<MyInventoryItem>(); inv.GetItems(items); double t=0; foreach(var i in items) { string sid=i.Type.SubtypeId; if(sid.Contains("Uranium")&&!sid.Contains("Ore")) t+=(double)i.Amount; } if(b is IMyPowerProducer && b.BlockDefinition.ToString().Contains("Reactor")) r+=t; else c+=t; } } } 
void GetHydrogenStats(out double c, out double m) { c=0; m=0; foreach(var t in h2Tanks) { c+=(t.Capacity*t.FilledRatio); m+=t.Capacity; } } 
void ProcessBatteries(out float s, out float m, out float i, out float o, out float mo, out float mi) { s=0; m=0; i=0; o=0; mo=0; mi=0; foreach(var b in batteries) { if(!b.IsWorking) continue; s+=b.CurrentStoredPower; m+=b.MaxStoredPower; i+=b.CurrentInput; o+=b.CurrentOutput; mo+=b.MaxOutput; mi+=b.MaxInput; } } 
void ProcessGenList(List<IMyPowerProducer> l, out float c, out float m) { c=0; m=0; foreach(var g in l) { if(g.IsWorking && g.Enabled) c+=g.CurrentOutput; m+=g.MaxOutput; } } 
void ProcessRenewables(out float sc, out float sp, out float wc, out float wp) { sc=0; sp=0; wc=0; wp=0; foreach(var s in solars) if(s.IsWorking) { sc+=s.CurrentOutput; sp+=s.MaxOutput; } foreach(var w in winds) if(w.IsWorking) { wc+=w.CurrentOutput; wp+=w.MaxOutput; } } 
string FormatMW(float mw) { return mw < 1.0f ? (mw*1000).ToString("0")+"kW" : mw.ToString("0.00")+"MW"; } 
string FormatTime(float h) { if(float.IsInfinity(h)||h<=0) return "-"; if(h>24) return "> 1 Day"; TimeSpan t=TimeSpan.FromHours(h); return $"{t.Hours}h {t.Minutes}m"; } 
void WriteOutput(string o, bool crit) { Color c = crit ? new Color(255, 0, 0) : new Color(255, 255, 255); foreach(var l in lcds) { l.WriteText(o); l.FontColor = c; } var s = Me.GetSurface(0); s.FontSize = PB_FONT_SIZE; s.WriteText(o); s.FontColor = c; }