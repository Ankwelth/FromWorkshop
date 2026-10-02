// MASTER BASE MANAGER v10.5

// =======================================================================================

// --- DEBUG MODE ------------------------------------------------------------------------
// Set this to TRUE if the script isn't working as expected.
// [TRUE]  - Prints detected Generator Types to the [SystemLog] LCD.
//           - Lists all Actions/Properties of your Ore Tap block to its Custom Data.
// [FALSE] - Normal operation (Recommended for daily use).
bool DEBUG_MODE = false; 

// =======================================================================================
//   1. POWER & GAS GENERATION
// =======================================================================================

// --- GENERATOR TYPE ---
// [TRUE]  - Vanilla Behavior: One generator block produces BOTH Oxygen and Hydrogen.
//           (The script keeps the generator running if EITHER gas is needed).
// [FALSE] - Modded Behavior: You have separate blocks for O2 and H2 generation.
bool HYBRID_GENS = false;

// --- MODDED BLOCK TAGS ---
// (Only used if HYBRID_GENS is FALSE).
// The script looks for these text strings in a block's Name or Type to identify it.
string MOD_OXYGEN_TAG   = "Oxygen Generator";   
string MOD_HYDROGEN_TAG = "Hydrogen Generator"; 

// =======================================================================================
//   2. INVENTORY & LOGISTICS SUPPORT
// =======================================================================================

// --- LOGISTICS COMPATIBILITY ---
// [TRUE]  - Recommended for "Isy's Inventory Manager" or "TIM" users.
//           The script MONITORS Refineries/Assemblers but NEVER turns them off.
//           This prevents "flickering" issues where Isy tries to move items into a block 
//           at the exact moment this script tries to turn it off to save power.
// [FALSE] - Aggressive Power Saving. Turns off idle machines.
bool LOGISTICS_COMPATIBILITY = true;

// =======================================================================================
//   3. ORE TAP (MINING AUTOMATION)
// =======================================================================================
// Automatically toggles Drills/Lasers based on how much ore you have stored.
// HOW TO USE: Group your mining blocks and name the group "OreTap: [ResourceName]"
// Example Group Name: "OreTap: Iron"

// --- GROUP PREFIX ---
// The tag the script looks for in your Terminal Group names.
const string ORETAP_PREFIX = "OreTap:"; 

// --- SHUTOFF DELAY ---
// How long (in seconds) to wait after storage is full before turning off the miner.
// Useful to allow conveyor buffers to empty out.
const int SHUTOFF_DELAY = 10; 

// --- TARGET ACTION / PROPERTY ---
// The specific internal command used to turn your miner on/off.
// [Vanilla Drills]       -> Set to "" (Empty String). The script will toggle the block power.
// [ToolCore / 2cm Beam]  -> Set to "ToolCore_Shoot_Action".
//                           The script will automatically append "_On" or "_Off" to this.
string ORE_TAP_PROPERTY = "ToolCore_Shoot_Action"; 

// =======================================================================================
//   4. RESOURCE THRESHOLDS
// =======================================================================================
// Define when to STOP mining (Max) and when to START mining (Min).
// Format: { "ResourceName", new ResourceConfig(STOP_AMOUNT, START_AMOUNT) }
// Note: "ResourceName" must match the game's internal SubtypeID (e.g. "Stone", "Ice").

Dictionary<string, ResourceConfig> OreConfigs = new Dictionary<string, ResourceConfig>(StringComparer.OrdinalIgnoreCase) {
    // --- FUEL ---
    { "Ice",       new ResourceConfig(50000, 20000) }, // Stop at 50k, Restart at 20k
    { "Uranium",   new ResourceConfig(100000, 50000) }, 
    
    // --- COMMON ORES ---
    { "Stone",     new ResourceConfig(10000, 1000) }, 
    { "Iron",      new ResourceConfig(50000, 10000) },
    { "Nickel",    new ResourceConfig(10000, 2000) },
    { "Silicon",   new ResourceConfig(10000, 2000) },
    { "Cobalt",    new ResourceConfig(5000, 1000) },
    
    // --- PRECIOUS ORES ---
    { "Magnesium", new ResourceConfig(2000, 500) },
    { "Silver",    new ResourceConfig(2000, 500) },
    { "Gold",      new ResourceConfig(1000, 200) },
    { "Platinum",  new ResourceConfig(500, 100) }
};

// =======================================================================================
//   5. LCD INFORMATION PANELS
// =======================================================================================
// Add these tags to the NAME of any LCD Panel to display that information.

string industryLcd = "[IndustryPowerStats]"; // Shows: Power Usage, Ore Amounts, Processing ETA
string gasLcd      = "[GasStats]";           // Shows: O2/H2 Tank Levels, Ice Amounts
string lsMasterLcd = "[MasterLifeSupport]";  // Shows: Status of all sealed rooms (Green/Red)
string logLcd      = "[SystemLog]";          // Shows: Script errors and debug info

// =======================================================================================
//   6. LIFE SUPPORT & ROOMS
// =======================================================================================
// Monitors Air Vents to detect breaches.
// SETUP: Group 1 Air Vent + 1 LCD. Name the group: "Life Support [Room Name]"

const string ROOM_TAG   = "Life Support"; // The tag to look for in Group Names.
const float ROOM_LOW_THRESH = 0.70f;      // < 70% Pressure = BREACHED (Red Alert)
const float ROOM_FULL_THRESH = 0.95f;     // > 95% Pressure = Standby (Saves O2)

// =======================================================================================
//   7. GAS AUTOMATION & DUMPING
// =======================================================================================
// Controls Generators based on tank levels to save Ice.

// --- GENERATOR ACTIVATION ---
const double O2_ON  = 0.60; const double O2_OFF = 0.80; // On at 60%, Off at 80%
const double H2_ON  = 0.70; const double H2_OFF = 0.95; // On at 70%, Off at 95%

// --- EMERGENCY DUMPING (Burner System) ---
// If Oxygen is full but we need Hydrogen, we must waste Oxygen to keep generators running.
// Setup: Name a Hydrogen Thruster "[O2 Burner]". It will fire into space to burn fuel.

const double BURN_START = 0.97;       // Start burning excess O2 at 97% capacity
const float BURN_STRENGTH = 0.15f;    // Burn intensity (15% Thrust)
const double BURN_MAX_START = 0.99;   // Panic Mode (99% capacity)
const float MAX_BURN_STRENGTH = 1.0f; // Panic intensity (100% Thrust)

// --- EMERGENCY VENTING ---
// Setup: Name a specific Air Vent "OverFlow Vent".
const string VENT_NAME  = "OverFlow Vent"; // Will be set to "Depressurize" if O2 > 99%
const string DOOR_NAME  = "OverFlow Door"; // Will be Opened if O2 > 99% (Use with caution!)

// =======================================================================================
//   8. ADVANCED TIMERS (Do Not Edit Unless Necessary)
// =======================================================================================

// How often (in seconds) the script re-scans the grid for new blocks.
int SCAN_INTERVAL = 30;     

// How long a machine stays "Spinning Down" before being turned off.
int coolDownSeconds = 30;        

// How long an assembler waits for materials before reporting "WAITING".
int stuckTimeoutSeconds = 45; 

// How long to force machines ON during a scan to ensure they are detected.
int wakeCycleSeconds = 15; 
const string TAG_IGNORE = "!Ignore"; // Add this to any block name to hide it from the script.


// ==========================================================
//              GLOBAL STATE (Do Not Edit Below)
// ==========================================================
public struct ResourceConfig {
    public double Max; public double Min;
    public ResourceConfig(double max, double min) { Max = max; Min = min; }
}

int runCount = 0; 
bool isDumping = false; bool isMaxBurn = false;
double lastTotalOre = 0; double avgOreCons = 0; string globalOreEta = "";
double lastTotalIce = 0; double avgIceCons = 0; string globalIceEta = "";
DateTime lastTick = DateTime.Now;
double curO2Pct = 0; double curH2Pct = 0;
StringBuilder statusLog = new StringBuilder();

string txtGas = "";
string txtInd = "";
string txtLife = "";

List<IMyRefinery> refineries = new List<IMyRefinery>(); 
List<IMyAssembler> assemblers = new List<IMyAssembler>();
List<IMyGasGenerator> gasGens = new List<IMyGasGenerator>(); 
List<IMyTerminalBlock> inventoryBlocks = new List<IMyTerminalBlock>(); 
HashSet<long> ignoredIds = new HashSet<long>();
List<string> ignoredBlockNames = new List<string>();

Dictionary<long, int> idleTimers = new Dictionary<long, int>();
Dictionary<long, bool> lastState = new Dictionary<long, bool>(); 
Dictionary<long, double> lastAmts = new Dictionary<long, double>();
Dictionary<long, double> avgRates = new Dictionary<long, double>();
Dictionary<long, DateTime> lastChks = new Dictionary<long, DateTime>();
Dictionary<string, int> oreDelayTimers = new Dictionary<string, int>();
Dictionary<string, double> currentInvCounts = new Dictionary<string, double>();

List<MyInventoryItem> tempItems = new List<MyInventoryItem>();

public Program() { 
    Runtime.UpdateFrequency = UpdateFrequency.Update100; 
    Log("v10.5 Config Update Active"); 
    if(LOGISTICS_COMPATIBILITY) Log("Logistics Mode: ON");
    if(!string.IsNullOrEmpty(ORE_TAP_PROPERTY)) Log($"Target Action: '{ORE_TAP_PROPERTY}'");
} 

public void Main(string argument, UpdateType updateSource) { 
    try { 
        runCount++; 
        double secSinceScan = runCount * 0.166; 
        bool waking = (secSinceScan <= wakeCycleSeconds); 
        
        if (runCount == 1 || secSinceScan >= SCAN_INTERVAL) { 
            runCount = 1; 
            ScanGrid(); 
        } 
        
        CalculateInventoryTotals();
        
        RunGasLogic(waking); 
        string oreStatus = RunOreTapLogic(); 
        RunIndustry(waking, oreStatus);       
        RunRoomLogic(); 
        
        UpdatePBScreen(); 
        UpdateLog();
    } catch (Exception e) { 
        Echo("CRASH: " + e.Message);
        statusLog.Insert(0, "CRASH: " + e.Message + "\n");
    } 
} 

// ==========================================================
//                      CORE LOGIC
// ==========================================================

void CalculateInventoryTotals() {
    currentInvCounts.Clear();
    foreach(var b in inventoryBlocks) {
        if(b.Closed || b.InventoryCount == 0) continue; 
        for(int i=0; i < b.InventoryCount; i++) {
            tempItems.Clear(); 
            b.GetInventory(i).GetItems(tempItems);
            foreach(var item in tempItems) {
                string sub = item.Type.SubtypeId.ToString();
                if(!currentInvCounts.ContainsKey(sub)) currentInvCounts[sub] = 0;
                currentInvCounts[sub] += (double)item.Amount;
            }
        }
    }
}

string RunOreTapLogic() {
    StringBuilder sb = new StringBuilder();
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups, g => g.Name.StartsWith(ORETAP_PREFIX));

    if (groups.Count == 0) return ""; 

    sb.AppendLine("\n[ ORE CONTROLS ]");

    foreach(var g in groups) {
        string resName = g.Name.Replace(ORETAP_PREFIX, "").Trim();
        ResourceConfig cfg = OreConfigs.ContainsKey(resName) ? OreConfigs[resName] : new ResourceConfig(10000, 1000);
        double current = currentInvCounts.ContainsKey(resName) ? currentInvCounts[resName] : 0;
        
        bool stop = false;
        bool start = current <= cfg.Min;

        if (resName.Equals("Ice", StringComparison.OrdinalIgnoreCase)) {
             stop = current >= cfg.Max || curO2Pct >= 0.99 || curH2Pct >= 0.99;
        } else {
             stop = current >= cfg.Max;
        }

        if (!oreDelayTimers.ContainsKey(resName)) oreDelayTimers[resName] = 0;
        
        string statusTag = "[OK]";
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        g.GetBlocks(blocks);

        if(DEBUG_MODE && blocks.Count > 0) {
            StringBuilder dbg = new StringBuilder();
            dbg.AppendLine("--- AVAILABLE ACTIONS ---");
            List<ITerminalAction> acts = new List<ITerminalAction>();
            blocks[0].GetActions(acts);
            foreach(var a in acts) dbg.AppendLine($"Act: {a.Id}");
            blocks[0].CustomData = dbg.ToString();
        }

        if (stop) {
            oreDelayTimers[resName]++;
            int ticksNeeded = (int)(SHUTOFF_DELAY / 1.66);
            if (oreDelayTimers[resName] >= ticksNeeded) {
                ToggleBlocks(blocks, false); 
                statusTag = "[OFF]";
            } else {
                statusTag = $"[DLY:{(int)((ticksNeeded - oreDelayTimers[resName]) * 1.66)}s]";
            }
        } else {
            oreDelayTimers[resName] = 0;
            if (start) {
                ToggleBlocks(blocks, true); 
                statusTag = "[ON]";
            } else {
                bool isActive = false;
                if(blocks.Count > 0) isActive = CheckActiveState(blocks[0]);
                statusTag = isActive ? "[ON]" : "[OFF]";
            }
        }
        
        double pct = (cfg.Max > 0) ? (current / cfg.Max) * 100 : 0;
        sb.AppendLine($"{statusTag} {resName}: {FormatNumber(current)} ({pct:0}%)");
    }
    return sb.ToString();
}

bool CheckActiveState(IMyTerminalBlock b) {
    var func = b as IMyFunctionalBlock;
    if (func != null) return func.Enabled;
    return false;
}

void RunGasLogic(bool waking) {
    List<IMyGasTank> tanks = new List<IMyGasTank>(); 
    GridTerminalSystem.GetBlocksOfType(tanks); 
    List<IMyThrust> burners = new List<IMyThrust>(); 
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(burners, t => t.CustomName.ToUpper().Contains("O2")); 
    
    double tO2 = 0, cO2 = 0, tH2 = 0, cH2 = 0; 
    foreach (var tank in tanks) { 
        if (tank.BlockDefinition.SubtypeId.Contains("Hydro")) { 
            tH2 += tank.Capacity; cH2 += (tank.Capacity * tank.FilledRatio); 
        } else { 
            tO2 += tank.Capacity; cO2 += (tank.Capacity * tank.FilledRatio); 
        } 
    } 
    curO2Pct = tO2 > 0 ? cO2 / tO2 : 0; 
    curH2Pct = tH2 > 0 ? cH2 / tH2 : 0; 

    double timeDiff = (DateTime.Now - lastTick).TotalSeconds;
    if (timeDiff >= 1.0) {
        double curIce = currentInvCounts.ContainsKey("Ice") ? currentInvCounts["Ice"] : 0;
        double iceD = lastTotalIce - curIce;
        if (iceD > 0) avgIceCons = (avgIceCons == 0) ? iceD : (avgIceCons * 0.9) + (iceD * 0.1);
        if(avgIceCons > 0.01) globalIceEta = FormatTime(curIce / avgIceCons); else globalIceEta = "Stable";
        lastTotalIce = curIce; 
        lastTick = DateTime.Now;
    }

    foreach (var gen in gasGens) { 
        if (ignoredIds.Contains(gen.EntityId)) continue; 
        
        bool isO, isH;
        
        if (HYBRID_GENS) {
            isO = true; isH = true;
        } else {
            string name = gen.CustomName.ToUpper();
            string type = gen.BlockDefinition.SubtypeId.ToUpper();
            string oTag = MOD_OXYGEN_TAG.ToUpper();
            string hTag = MOD_HYDROGEN_TAG.ToUpper();

            isO = name.Contains(oTag) || type.Contains(oTag);
            isH = name.Contains(hTag) || type.Contains(hTag);

            if(!isO && !isH) { isO = true; isH = true; }
        }
        
        bool turnOn = false;
        
        if (isO && curO2Pct < O2_ON) turnOn = true;
        if (isH && curH2Pct < H2_ON) turnOn = true;
        
        if (gen.Enabled) {
            turnOn = true; 
            bool noTanks = (tO2 == 0 && tH2 == 0);
            bool satisfyO = !isO || (tO2 > 0 ? curO2Pct >= O2_OFF : !noTanks);
            bool satisfyH = !isH || (tH2 > 0 ? curH2Pct >= H2_OFF : !noTanks);
            if (satisfyO && satisfyH) turnOn = false;
        }
        SmartToggle(gen, turnOn || waking);
    } 

    isDumping = (curO2Pct >= BURN_START); 
    isMaxBurn = (curO2Pct >= BURN_MAX_START);
    float bVal = isMaxBurn ? MAX_BURN_STRENGTH : (isDumping ? BURN_STRENGTH : 0f);
    foreach(var t in burners) { t.Enabled = true; t.ThrustOverridePercentage = bVal; }
    if (burners.Count == 0) HandleFallbackHardware(isDumping);

    StringBuilder gsb = new StringBuilder();
    gsb.AppendLine("=== GAS SYSTEM STATUS ===");
    gsb.AppendLine($"O2: {(curO2Pct*100):0}% | H2: {(curH2Pct*100):0}%");
    gsb.AppendLine($"Ice: {FormatNumber(lastTotalIce)} | Fuel: {globalIceEta}");
    gsb.AppendLine("-------------------------");
    gsb.AppendLine("DUMP STATUS: " + (isDumping ? (isMaxBurn ? "MAX BURN" : "ACTIVE") : "OFF"));
    
    txtGas = gsb.ToString(); 
    UpdateLCD(gasLcd, txtGas);
}

void RunIndustry(bool waking, string oreStatus) { 
    double pwr = 0; 
    double totalOre = 0; 
    List<string> listWorking = new List<string>(); 
    List<string> listIdle = new List<string>(); 
    int nextCheck = SCAN_INTERVAL - (int)(runCount * 0.166); 
    
    foreach(var kvp in currentInvCounts) {
        if(kvp.Key.EndsWith("Ore") || kvp.Key == "Stone" || kvp.Key == "Scrap") totalOre += kvp.Value;
    }
    
    foreach(var r in refineries) { 
        if (ignoredIds.Contains(r.EntityId)) continue; 
        double cur = (double)r.InputInventory.CurrentMass; 
        bool hasInput = r.InputInventory.ItemCount > 0;
        bool hasOutput = r.OutputInventory.ItemCount > 0;
        
        if (hasInput || hasOutput) { 
            idleTimers[r.EntityId] = 0; 
            SmartToggle(r, true); 
            pwr += GetPower(r); 
            if(hasInput) {
                var item = r.InputInventory.GetItemAt(0);
                string itemName = item.HasValue ? item.Value.Type.SubtypeId.ToString() : "???";
                listWorking.Add($"{Truncate(r.CustomName, 10)} ({itemName})"); 
            } else {
                listWorking.Add($"{Truncate(r.CustomName, 10)} [Output Full]"); 
            }
        } else { 
            if (!idleTimers.ContainsKey(r.EntityId)) idleTimers[r.EntityId] = 0; 
            idleTimers[r.EntityId]++; 
            int secIdle = (int)(idleTimers[r.EntityId] * 1.66); 
            if (secIdle < coolDownSeconds) { 
                SmartToggle(r, true); 
                pwr += GetPower(r); 
                listWorking.Add($"{Truncate(r.CustomName, 10)} [Cooling]"); 
            } else { 
                if(LOGISTICS_COMPATIBILITY) { SmartToggle(r, true); listIdle.Add($"{Truncate(r.CustomName, 12)} [Idle]"); } 
                else { SmartToggle(r, waking); listIdle.Add($"{Truncate(r.CustomName, 12)} [OFF]"); }
            } 
        } 
    } 
    
    foreach(var a in assemblers) { 
        if (ignoredIds.Contains(a.EntityId)) continue; 
        if (!idleTimers.ContainsKey(a.EntityId)) idleTimers[a.EntityId] = 0; 
        List<MyProductionItem> q = new List<MyProductionItem>(); a.GetQueue(q); 
        double qAmt = 0; foreach(var i in q) qAmt += (double)i.Amount; 
        bool hasOutput = a.OutputInventory.ItemCount > 0;

        if ((a.IsProducing && qAmt > 0) || hasOutput) { 
            idleTimers[a.EntityId] = 0; SmartToggle(a, true); pwr += GetPower(a); 
            if (q.Count > 0) listWorking.Add($"{Truncate(a.CustomName, 10)} ({q[0].BlueprintId.SubtypeName})"); 
            else listWorking.Add($"{Truncate(a.CustomName, 10)} [Output Full]");
        } else { 
            idleTimers[a.EntityId]++; int secIdle = (int)(idleTimers[a.EntityId] * 1.66); 
            if (secIdle < coolDownSeconds) { SmartToggle(a, true); listWorking.Add($"{Truncate(a.CustomName, 10)} [Standby]"); } 
            else if (!a.IsQueueEmpty && secIdle < stuckTimeoutSeconds) { SmartToggle(a, true); listWorking.Add($"{Truncate(a.CustomName, 10)} [WAITING]"); } 
            else { 
                if(LOGISTICS_COMPATIBILITY) { SmartToggle(a, true); listIdle.Add($"{Truncate(a.CustomName, 12)} [Idle]"); } 
                else { SmartToggle(a, waking); listIdle.Add($"{Truncate(a.CustomName, 12)} [OFF]"); }
            } 
        } 
    } 
    
    double oreD = lastTotalOre - totalOre; 
    if (oreD > 0) avgOreCons = (avgOreCons == 0) ? oreD : (avgOreCons * 0.95) + (oreD * 0.05); 
    lastTotalOre = totalOre; 
    
    if (avgOreCons > 0.01) globalOreEta = FormatTime(totalOre / avgOreCons * SCAN_INTERVAL); else globalOreEta = "Done";
    
    StringBuilder sb = new StringBuilder(); 
    if (waking && !LOGISTICS_COMPATIBILITY) sb.AppendLine("!!! GRID WAKE CYCLE ACTIVE !!!\n[||||||||||||||||||||]"); 
    else sb.AppendLine($"--- INDUSTRY (Scan: {nextCheck}s) ---"); 
    
    sb.AppendLine($"Pwr: {pwr:0.1}MW | Ore: {FormatNumber(totalOre)}\nETA: {globalOreEta}"); 
    
    if (!string.IsNullOrEmpty(oreStatus)) sb.Append(oreStatus);

    if (listWorking.Count > 0) { sb.AppendLine("\n[ WORKING ]"); foreach(var s in listWorking) sb.AppendLine("> " + s); } 
    if (listIdle.Count > 0) { sb.AppendLine("\n[ STANDBY ]"); foreach(var s in listIdle) sb.AppendLine("z " + s); } 
    
    txtInd = sb.ToString(); 
    UpdateLCD(industryLcd, txtInd); 
}

void RunRoomLogic() { 
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>(); 
    GridTerminalSystem.GetBlockGroups(groups); 
    StringBuilder masterLS = new StringBuilder(); 
    masterLS.AppendLine("=== MASTER LIFE SUPPORT ==="); 
    foreach (var g in groups) { 
        if (!g.Name.Contains(ROOM_TAG) || g.Name.Contains(TAG_IGNORE)) continue; 
        List<IMyAirVent> v = new List<IMyAirVent>(); List<IMyTextPanel> p = new List<IMyTextPanel>(); 
        g.GetBlocksOfType(v); g.GetBlocksOfType(p); 
        if (v.Count == 0) continue; 
        bool s = v[0].CanPressurize; float pr = v[0].GetOxygenLevel(); 
        bool f = s && (pr < ROOM_LOW_THRESH); if (pr >= ROOM_FULL_THRESH) f = false; 
        for (int i = 1; i < v.Count; i++) v[i].Enabled = f; 
        string rn = g.Name.Replace(ROOM_TAG, "").Trim(); 
        masterLS.AppendLine($"{(s ? "[OK]" : "[!!]")} {Truncate(rn, 12)}: {(s ? (pr*100).ToString("0")+"%" : "BREACH")}"); 
        string statusText = s ? "SEALED" : "BREACHED"; string o2Text = (pr * 100).ToString("0.0") + "%"; Color statusColor = s ? (pr < 0.8f ? Color.Yellow : Color.Green) : Color.Red;
        foreach (var x in p) { x.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; x.FontSize = 1.2f; x.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER; x.FontColor = statusColor; x.WriteText($"--- ROOM: {rn} ---\nStatus: {statusText}\nO2 Level: {o2Text}"); }
    } 
    txtLife = masterLS.ToString(); 
    UpdateLCD(lsMasterLcd, txtLife); 
}

void UpdatePBScreen() { 
    IMyTextSurface s = Me.GetSurface(0); 
    s.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; 
    s.FontSize = 0.45f; 
    
    StringBuilder pbOut = new StringBuilder();
    pbOut.AppendLine("== MASTER BASE MANAGER [v10.5] =="); 
    pbOut.AppendLine("");
    pbOut.Append(txtGas);
    pbOut.AppendLine("");
    pbOut.Append(txtInd);
    pbOut.AppendLine("");
    pbOut.Append(txtLife);

    s.WriteText(pbOut.ToString()); 
}

// ==========================================================
//                      HELPER FUNCTIONS
// ==========================================================
void ToggleBlocks(List<IMyTerminalBlock> blocks, bool enable) {
    foreach (var b in blocks) {
        bool actionTriggered = false;
        
        // 1. Try Custom Property (Configured string)
        if (!string.IsNullOrEmpty(ORE_TAP_PROPERTY)) {
             // A. Try as a Direct Property (e.g., "DrainAll")
             var prop = b.GetProperty(ORE_TAP_PROPERTY);
             if (prop != null && prop.TypeName == "Boolean") {
                 b.SetValueBool(ORE_TAP_PROPERTY, enable);
                 actionTriggered = true;
             } 
             
             // B. Try as an Action (e.g., "ToolCore_Shoot_Action" -> "_On" / "_Off")
             if (!actionTriggered) {
                 string suffix = enable ? "_On" : "_Off";
                 var action = b.GetActionWithName(ORE_TAP_PROPERTY + suffix);
                 if (action != null) {
                     action.Apply(b);
                     actionTriggered = true;
                 }
             }

             // If triggered via Property or Action, ensure Main Power is ON for Isy
             if (actionTriggered) {
                 var f = b as IMyFunctionalBlock;
                 if (f != null && !f.Enabled) f.Enabled = true; 
             }
        }
        
        // 2. Fallback to Main Power (Standard Blocks)
        if (!actionTriggered) {
             var func = b as IMyFunctionalBlock;
             if (func != null && func.Enabled != enable) func.Enabled = enable;
        }
    }
}
void SmartToggle(IMyFunctionalBlock b, bool on) { 
    if (lastState.ContainsKey(b.EntityId) && b.Enabled != lastState[b.EntityId]) { lastState[b.EntityId] = b.Enabled; return; } 
    b.Enabled = on; lastState[b.EntityId] = on; 
}
void ScanGrid() { 
    refineries.Clear(); assemblers.Clear(); gasGens.Clear(); 
    inventoryBlocks.Clear(); 
    ignoredIds.Clear(); ignoredBlockNames.Clear(); 
    
    List<IMyTerminalBlock> all = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType(all, b => b.IsSameConstructAs(Me)); 
    List<IMyBlockGroup> grps = new List<IMyBlockGroup>(); 
    GridTerminalSystem.GetBlockGroups(grps); 
    
    foreach(var g in grps) {
        if(g.Name.Contains(TAG_IGNORE)) { 
            List<IMyTerminalBlock> gb = new List<IMyTerminalBlock>(); 
            g.GetBlocks(gb); 
            foreach(var b in gb) { 
                ignoredIds.Add(b.EntityId); 
                if (!ignoredBlockNames.Contains(b.CustomName)) ignoredBlockNames.Add(b.CustomName); 
            } 
        } 
    }
    
    foreach(var b in all) { 
        if (b.CustomName.Contains(TAG_IGNORE)) { 
            ignoredIds.Add(b.EntityId); 
            if (!ignoredBlockNames.Contains(b.CustomName)) ignoredBlockNames.Add(b.CustomName); 
        } 
        
        if (b is IMyRefinery) refineries.Add(b as IMyRefinery); 
        else if (b is IMyAssembler) assemblers.Add(b as IMyAssembler); 
        else if (b is IMyGasGenerator) gasGens.Add(b as IMyGasGenerator); 
        
        if (b.HasInventory) inventoryBlocks.Add(b);
    } 
}
string GetETA(long id, double amt) { 
    if(!lastAmts.ContainsKey(id)) { lastAmts[id] = amt; avgRates[id] = 0; lastChks[id] = DateTime.Now; return ""; } 
    double diff = lastAmts[id] - amt; double time = (DateTime.Now - lastChks[id]).TotalSeconds; 
    if(diff > 0 && time > 0) avgRates[id] = (avgRates[id] <= 0) ? diff/time : (avgRates[id] * 0.95) + (diff/time * 0.05); lastAmts[id] = amt; lastChks[id] = DateTime.Now; 
    
    if(avgRates[id] > 0.001) return "~" + FormatTime(amt/avgRates[id]); else return "Done"; 
}
double GetPower(IMyTerminalBlock b) { 
    try {
        string s = b.DetailedInfo; 
        int i = s.IndexOf("Input:"); 
        if(i == -1) return 0; 
        double v; 
        if(double.TryParse(s.Substring(i+6).Trim().Split(' ')[0], out v)) 
            return s.Contains("kW") ? v/1000 : v; 
        return 0; 
    } catch { return 0; }
}
void UpdateLCD(string tag, string txt) { List<IMyTextPanel> p = new List<IMyTextPanel>(); GridTerminalSystem.GetBlocksOfType(p, x => x.CustomName.Contains(tag)); foreach(var x in p) { x.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; x.WriteText(txt); } }
void Log(string msg) { statusLog.Insert(0, DateTime.Now.ToString("HH:mm:ss") + ": " + msg + "\n"); if (statusLog.Length > 2000) statusLog.Length = 2000; }
void UpdateLog() { UpdateLCD(logLcd, "=== SYSTEM UPDATES ===\n" + statusLog.ToString()); }
string FormatTime(double s) { 
    if (double.IsNaN(s) || double.IsInfinity(s) || s > 315360000) return "Stable"; 
    TimeSpan t = TimeSpan.FromSeconds(s); 
    return t.TotalHours >= 1 ? t.ToString(@"hh\h\ mm\m") : t.ToString(@"mm\m\ ss\s"); 
}
string FormatNumber(double n) { if (n >= 1000000) return (n / 1000000).ToString("0.0") + "M"; if (n >= 1000) return (n / 1000).ToString("0.0") + "k"; return n.ToString("0"); }
void HandleFallbackHardware(bool dumping) { List<IMyAirVent> v = new List<IMyAirVent>(); GridTerminalSystem.GetBlocksOfType(v, x => x.CustomName == VENT_NAME); List<IMyDoor> d = new List<IMyDoor>(); GridTerminalSystem.GetBlocksOfType(d, x => x.CustomName == DOOR_NAME); foreach(var x in v) x.Depressurize = dumping; foreach(var x in d) { if(dumping && v.Count > 0 && v[0].GetOxygenLevel() >= 0.9) x.OpenDoor(); else x.CloseDoor(); } }
string Truncate(string s, int len) { return (s.Length <= len) ? s : s.Substring(0, len); }