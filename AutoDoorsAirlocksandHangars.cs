/*
 * SMART DOOR & HANGAR SYSTEM v80 (REVERTED)
 * ---------------------------------------------------
 * - REVERTED: All logic returned to your original v80 state.
 * - FIXED: The "Ignore" keyword now correctly excludes doors.
 */

// =======================================================================================
//                                CONFIGURATION SECTION
// =======================================================================================

public float OXY_LOW = 0.05f; 
public float OXY_HIGH = 0.90f;
public bool RESTORE_PRESSURIZATION = true;

public string GROUP_TAG_HANGAR = "Hangar";
public string KEYWORD_HANGAR_INNER = "Inner"; 
public string KEYWORD_HANGAR_GATE = "Gate";   
public double HANGAR_CYCLE_TIME = 5.0;        

public string GROUP_TAG_AIRLOCK = "Airlock";      
public string KEYWORD_AIRLOCK_INNER = "Inner";    
public string KEYWORD_AIRLOCK_OUTER = "Outer";
public double AIRLOCK_CYCLE_TIME = 3.0; 

public string TAG_LCD_AIRLOCK = "Airlock Master"; 
public string TAG_LCD_HANGAR = "Hangar Master";   
public string TAG_LCD_MAIN = "Base Status";       
public bool SHOW_SPLIT_SCREEN = false; 

public Color COLOR_SAFE = new Color(0, 255, 0);        
public Color COLOR_VACUUM = new Color(0, 100, 255);    
public Color COLOR_CYCLING = new Color(255, 140, 0); 
public Color COLOR_CLOSED = new Color(255, 0, 0);      

public double OTHER_DOOR_TIME = 3.0; 
public string KEYWORD_IGNORE = "Ignore";      
public string GROUP_TAG_IGNORE = "Ignore Group";   
public string KEYWORD_SENSOR = "Sensor"; 

// =======================================================================================
//                                 END CONFIGURATION
// =======================================================================================

List<Airlock> airlocks = new List<Airlock>();
List<Hangar> hangars = new List<Hangar>();
List<SimpleDoor> otherDoors = new List<SimpleDoor>();
List<IMyTextPanel> lcdsAirlock = new List<IMyTextPanel>();
List<IMyTextPanel> lcdsHangar = new List<IMyTextPanel>();
List<IMyTextPanel> lcdsMain = new List<IMyTextPanel>();

StringBuilder debugLog = new StringBuilder();
StringBuilder sbAirlock = new StringBuilder();
StringBuilder sbHangar = new StringBuilder();
StringBuilder sbMain = new StringBuilder();

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10; 
    Scan();
}

public void Main(string arg, UpdateType source) {
    if (arg.ToLower() == "setup") Scan();
    sbAirlock.Clear(); sbHangar.Clear(); sbMain.Clear();

    foreach (var sys in airlocks) { sys.Run(Runtime.TimeSinceLastRun.TotalSeconds); sbAirlock.AppendLine(sys.GetStatusLine()); }
    foreach (var sys in hangars) { sys.Run(Runtime.TimeSinceLastRun.TotalSeconds); sbHangar.AppendLine(sys.GetStatusLine()); }

    int autoSensorCount = 0;
    foreach(var d in otherDoors) { d.Update(Runtime.TimeSinceLastRun.TotalSeconds); if(d.HasSensors) autoSensorCount++; }

    if (SHOW_SPLIT_SCREEN) {
        sbMain.AppendLine("AIRLOCKS                        HANGARS");
        sbMain.AppendLine("-----------------------------------------------------");
        int max = Math.Max(airlocks.Count, hangars.Count);
        for(int i=0; i<max; i++) {
            string left = (i < airlocks.Count) ? airlocks[i].GetShortStatus() : "";
            string right = (i < hangars.Count) ? hangars[i].GetShortStatus() : "";
            sbMain.AppendLine($"{left,-30} | {right}");
        }
    } else {
        sbMain.AppendLine("=== AIRLOCKS ==="); sbMain.Append(sbAirlock);
        sbMain.AppendLine("\n=== HANGARS ==="); sbMain.Append(sbHangar);
    }
    
    sbMain.AppendLine("\n-----------------------------------------------------");
    sbMain.AppendLine($"AUTO DOORS: {otherDoors.Count} Managed | {autoSensorCount} Sensor Linked");

    WriteToLcds(lcdsAirlock, sbAirlock, false);
    WriteToLcds(lcdsHangar, sbHangar, false);
    WriteToLcds(lcdsMain, sbMain, SHOW_SPLIT_SCREEN);

    Echo("=== DOOR MANAGER v80 ===");
    Echo(debugLog.ToString()); 
}

void WriteToLcds(List<IMyTextPanel> panels, StringBuilder text, bool forceMono) {
    foreach(var p in panels) {
        p.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        if(forceMono) { p.Font = "Monospace"; p.FontSize = 0.6f; }
        p.WriteText(text);
    }
}

void Scan() {
    airlocks.Clear(); hangars.Clear(); otherDoors.Clear();
    lcdsAirlock.Clear(); lcdsHangar.Clear(); lcdsMain.Clear();
    debugLog.Clear();
    HashSet<long> managedIds = new HashSet<long>();

    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups, g => g.Name.IndexOf(GROUP_TAG_AIRLOCK, StringComparison.OrdinalIgnoreCase) >= 0);
    foreach (var g in groups) {
        Airlock a = new Airlock(this, g);
        if (a.IsValid) { 
            airlocks.Add(a); managedIds.Add(a.Inner.EntityId); managedIds.Add(a.Outer.EntityId);
            debugLog.AppendLine($"[OK] Airlock '{g.Name}' registered.");
        }
    }

    groups.Clear();
    GridTerminalSystem.GetBlockGroups(groups, g => g.Name.IndexOf(GROUP_TAG_HANGAR, StringComparison.OrdinalIgnoreCase) >= 0);
    foreach (var g in groups) {
        Hangar h = new Hangar(this, g, debugLog);
        if (h.IsValid) { 
            hangars.Add(h); 
            foreach(var set in h.AllGates) foreach(var d in set.Doors) managedIds.Add(d.EntityId);
        }
    }

    groups.Clear(); GridTerminalSystem.GetBlockGroups(groups, g => g.Name.IndexOf(GROUP_TAG_IGNORE, StringComparison.OrdinalIgnoreCase) >= 0);
    foreach (var g in groups) { List<IMyDoor> d = new List<IMyDoor>(); g.GetBlocksOfType(d); foreach(var x in d) managedIds.Add(x.EntityId); }

    List<IMySensorBlock> allSensors = new List<IMySensorBlock>(); GridTerminalSystem.GetBlocksOfType(allSensors);
    List<IMyDoor> allDoors = new List<IMyDoor>(); GridTerminalSystem.GetBlocksOfType(allDoors);
    foreach(var d in allDoors) {
        // This line checks for the Ignore keyword
        if (!managedIds.Contains(d.EntityId) && d.CustomName.IndexOf(KEYWORD_IGNORE, StringComparison.OrdinalIgnoreCase) < 0) {
            SimpleDoor sd = new SimpleDoor(d, OTHER_DOOR_TIME);
            foreach(var s in allSensors) if(s.CustomName.Contains(d.CustomName) || d.CustomName.Contains(s.CustomName)) sd.AddSensor(s);
            otherDoors.Add(sd);
        }
    }

    GridTerminalSystem.GetBlocksOfType(lcdsAirlock, b => b.CustomName.Contains(TAG_LCD_AIRLOCK));
    GridTerminalSystem.GetBlocksOfType(lcdsHangar, b => b.CustomName.Contains(TAG_LCD_HANGAR));
    GridTerminalSystem.GetBlocksOfType(lcdsMain, b => b.CustomName.Contains(TAG_LCD_MAIN));
}

public class SimpleDoor {
    IMyDoor Door; double MaxTime, Timer; List<IMySensorBlock> Sensors = new List<IMySensorBlock>();
    public bool HasSensors { get { return Sensors.Count > 0; } }
    public SimpleDoor(IMyDoor d, double time) { Door=d; MaxTime=time; }
    public void AddSensor(IMySensorBlock s) { Sensors.Add(s); }
    public void Update(double dt) {
        bool trig = false; foreach(var s in Sensors) if(s.IsActive) trig = true;
        if(trig) { if(Door.Status!=DoorStatus.Open && Door.Enabled) Door.OpenDoor(); Timer=0; }
        else if(Door.Status==DoorStatus.Open) { Timer+=dt; if(Timer>=MaxTime) { if(Door.Enabled) Door.CloseDoor(); Timer=0; } }
    }
}

public class GateSet {
    public List<IMyDoor> Doors = new List<IMyDoor>();
    public List<IMySensorBlock> Sensors = new List<IMySensorBlock>();
    public List<IMyLightingBlock> Lights = new List<IMyLightingBlock>(); 
    public string ID;
    public bool IsOuter = false; 

    public bool WantsOpen() {
        foreach(var s in Sensors) if(s.IsActive) return true;
        foreach(var d in Doors) if(d.OpenRatio > 0) return true; 
        return false;
    }
    public bool AreSensorsActive() {
        foreach(var s in Sensors) if(s.IsActive) return true;
        return false;
    }
    public void Close() { foreach(var d in Doors) if(d.Status!=DoorStatus.Closed && d.Enabled) d.CloseDoor(); }
    public void Open() { foreach(var d in Doors) if(d.Status!=DoorStatus.Open && d.Enabled) d.OpenDoor(); }
    public void Enable(bool s) { foreach(var d in Doors) d.Enabled=s; }
    public bool IsClosed() { foreach(var d in Doors) if(d.Status!=DoorStatus.Closed) return false; return true; }
    public void UpdateLights(Color color, float blink) { foreach(var l in Lights) { l.Color = color; l.BlinkIntervalSeconds = blink; } }
}

public class Hangar {
    Program P; public string Name;
    public List<GateSet> AllGates = new List<GateSet>();
    GateSet ActiveGate = null; 
    List<IMyAirVent> Vents = new List<IMyAirVent>();
    List<IMyLightingBlock> GlobalLights = new List<IMyLightingBlock>(); 
    List<IMyTextPanel> Lcds = new List<IMyTextPanel>();
    
    enum State { Idle, Open_Wait, Closing, Cycling, Opening }
    State CurrentState = State.Idle;
    double Timer = 0; double LoiterTimer = 0; double CycleTimeout = 0; 
    bool EntityDetected = false; string StatusText = "Ready"; public bool IsValid = false;

    string GetIdentity(string name) {
        string n = name.ToLower();
        string[] keywords = { "hangar", "gate", "inner", "outer", "sensor", "door", "light", "airlock", "group" };
        foreach(var k in keywords) n = n.Replace(k, "");
        return n.Trim(); 
    }

    public Hangar(Program p, IMyBlockGroup group, StringBuilder debug) {
        P = p; Name = group.Name;
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>(); group.GetBlocks(blocks);
        foreach(var b in blocks) {
            if(b is IMyAirVent) Vents.Add((IMyAirVent)b); 
            else if(b is IMyTextPanel) Lcds.Add((IMyTextPanel)b);
        }

        List<IMySensorBlock> allSensors = new List<IMySensorBlock>();
        foreach(var b in blocks) if(b is IMySensorBlock) allSensors.Add((IMySensorBlock)b);

        foreach(var s in allSensors) {
            bool isGate = s.CustomName.ToLower().Contains(P.KEYWORD_HANGAR_GATE.ToLower());
            bool isInner = s.CustomName.ToLower().Contains(P.KEYWORD_HANGAR_INNER.ToLower());
            
            if(isGate || isInner) {
                GateSet set = new GateSet();
                set.Sensors.Add(s);
                set.IsOuter = isGate; 
                string coreID = GetIdentity(s.CustomName);
                if (coreID == "") coreID = "Main"; 
                set.ID = coreID;
                debug.AppendLine($"    > Found {(isGate?"Gate":"Inner")} ID: '{coreID}'");

                foreach(var b in blocks) {
                    if (GetIdentity(b.CustomName) == coreID) {
                        if(b is IMyDoor) set.Doors.Add((IMyDoor)b);
                        if(b is IMyLightingBlock) set.Lights.Add((IMyLightingBlock)b);
                    }
                }
                
                if(set.Doors.Count > 0) AllGates.Add(set);
                else debug.AppendLine($"      [!] No doors found matching ID '{coreID}'");
            }
        }

        foreach(var b in blocks) {
            if(b is IMyLightingBlock) {
                bool assigned = false;
                foreach(var set in AllGates) if(set.Lights.Contains((IMyLightingBlock)b)) assigned=true;
                if(!assigned) GlobalLights.Add((IMyLightingBlock)b);
            }
        }
        if (AllGates.Count > 0) IsValid = true;
    }
    
    public float GetOxygen() { if(Vents.Count==0)return 0f; float m=0f; foreach(var v in Vents)if(v.GetOxygenLevel()>m)m=v.GetOxygenLevel(); return m; }
    void SetVents(bool d) { foreach(var v in Vents) { v.Enabled=true; v.Depressurize=d; } }

    public void Run(double dt) {
        if(!IsValid) return;
        float oxy = GetOxygen();

        switch (CurrentState) {
            case State.Idle:
                if (oxy <= P.OXY_LOW) { StatusText="VACUUM"; SetVisuals(P.COLOR_VACUUM, "VACUUM"); } 
                else { StatusText="PRESSURIZED"; SetVisuals(P.COLOR_SAFE, "PRESSURIZED"); }
                foreach(var g in AllGates) g.Enable(true);
                
                GateSet requestingGate = null;
                foreach(var g in AllGates) if(g.WantsOpen()) requestingGate = g;

                if (requestingGate != null) {
                    LoiterTimer += dt;
                    bool manual = requestingGate.Doors.Any(d=>d.OpenRatio>0);
                    if (!manual && LoiterTimer < 1.5) { StatusText = "DETECTING..."; return; }
                    
                    ActiveGate = requestingGate;
                    LoiterTimer = 0;
                    
                    bool targetVacuum = ActiveGate.IsOuter;
                    bool safe = targetVacuum ? (oxy <= P.OXY_LOW) : (oxy > P.OXY_LOW); 

                    bool conflict = false;
                    foreach(var g in AllGates) {
                        if(g != ActiveGate && !g.IsClosed()) {
                            if(ActiveGate.IsOuter || g.IsOuter) conflict = true;
                        }
                    }

                    if (!safe || conflict) { CurrentState = State.Closing; } 
                    else { SetVents(targetVacuum); ActiveGate.Open(); Timer = 0; EntityDetected = false; CurrentState = State.Open_Wait; }
                } else { LoiterTimer = 0; ActiveGate = null; }
                break;

            case State.Open_Wait:
                bool active = (ActiveGate != null && ActiveGate.AreSensorsActive() && !ActiveGate.IsClosed());
                if (active) { Timer = 0; EntityDetected = true; StatusText = "CLEAR AREA"; } 
                else {
                    if (EntityDetected) { Timer += (dt * 3); StatusText = $"FAST CLOSE {P.HANGAR_CYCLE_TIME - Timer:0.0}s"; } 
                    else { Timer += dt; StatusText = $"WAIT {P.HANGAR_CYCLE_TIME - Timer:0.0}s"; }
                }
                
                foreach(var g in AllGates) {
                    if(g != ActiveGate) {
                        if(ActiveGate.IsOuter || g.IsOuter) g.Close();
                    }
                }
                
                if(Timer >= P.HANGAR_CYCLE_TIME) CurrentState = State.Closing;
                break;

            case State.Closing:
                StatusText="SEALING"; 
                foreach(var g in AllGates) g.Close();
                
                bool allClosed = true; foreach(var g in AllGates) if(!g.IsClosed()) allClosed = false;
                if(allClosed) { 
                    if (ActiveGate == null) { CurrentState = State.Idle; break; }
                    bool targetVacuum = ActiveGate.IsOuter;
                    
                    bool needsCycle = (targetVacuum && oxy > P.OXY_LOW) || (!targetVacuum && oxy <= P.OXY_LOW);

                    if (needsCycle) { CurrentState = State.Cycling; CycleTimeout = 0; } 
                    else { CurrentState = State.Idle; if (P.RESTORE_PRESSURIZATION) SetVents(false); }
                }
                break;

            case State.Cycling:
                foreach(var g in AllGates) g.Close();
                CycleTimeout += dt; bool stall = (CycleTimeout > 15.0); bool done = false;
                bool targetVac = (ActiveGate != null && ActiveGate.IsOuter);
                if(targetVac) { StatusText="DEPRESSURIZING"; SetVisuals(P.COLOR_CYCLING, $"CYCLING {oxy:P0}"); SetVents(true); if(oxy <= P.OXY_LOW || stall) done=true; }
                else { StatusText="PRESSURIZING"; SetVisuals(P.COLOR_CYCLING, $"CYCLING {oxy:P0}"); SetVents(false); if(oxy >= P.OXY_HIGH || stall) done=true; }
                if(done) CurrentState = State.Opening;
                break;

            case State.Opening:
                StatusText="OPENING";
                bool isVac = (ActiveGate != null && ActiveGate.IsOuter);
                if(isVac) SetVisuals(P.COLOR_VACUUM, "LAUNCH READY"); else SetVisuals(P.COLOR_SAFE, "BASE OPEN"); 
                if(ActiveGate != null) ActiveGate.Open();
                Timer=0; EntityDetected = false; CurrentState = State.Open_Wait;
                break;
        }
    }

    void SetVisuals(Color c, string mainStatus) { 
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"--- {Name.ToUpper()} ---");
        sb.AppendLine($"STATE:    {mainStatus}");
        sb.AppendLine($"PRESSURE: {GetOxygen():P1}");
        sb.AppendLine("------------------");
        foreach(var g in AllGates) sb.AppendLine($"{(g.IsOuter?"GATE":"INNER")} {g.ID.ToUpper()}: {GetDoorStateString(g)}");
        foreach(var lcd in Lcds) { lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; lcd.WriteText(sb.ToString()); lcd.FontColor=c; } 
        foreach(var l in GlobalLights) { l.Color=c; l.BlinkIntervalSeconds=0; } 
        foreach(var g in AllGates) {
            if (g.IsClosed()) g.UpdateLights(P.COLOR_CLOSED, 0f); 
            else if (g.Doors[0].Status == DoorStatus.Open) g.UpdateLights(P.COLOR_SAFE, 0f); 
            else g.UpdateLights(P.COLOR_CYCLING, 1.0f); 
        }
    }
    string GetDoorStateString(GateSet g) {
        if (g.IsClosed()) return "CLOSED";
        foreach(var d in g.Doors) {
            if (d.Status == DoorStatus.Opening) return "OPENING >>";
            if (d.Status == DoorStatus.Closing) return "CLOSING <<";
        }
        return "OPEN";
    }
    public string GetStatusLine() { return $"{Name}: {StatusText} ({GetOxygen():P0})"; }
    public string GetShortStatus() { return $"{Name}: {StatusText}"; }
}

public class Airlock {
    Program P; public string Name; public IMyDoor Inner, Outer;
    public List<IMySensorBlock> InnerSensors = new List<IMySensorBlock>();
    public List<IMySensorBlock> OuterSensors = new List<IMySensorBlock>();
    List<IMyAirVent> Vents = new List<IMyAirVent>();
    List<IMyLightingBlock> Lights = new List<IMyLightingBlock>();
    List<IMyTextPanel> Lcds = new List<IMyTextPanel>();
    enum State { Idle, Entry_Wait, Entry_Closing, Cycling, Exit_Opening, Exit_Wait, Exit_Closing }
    State CurrentState = State.Idle;
    bool GoingToSpace = false; double Timer = 0; string StatusText = "Init"; public bool IsValid = false;

    public Airlock(Program p, IMyBlockGroup g) {
        P = p; Name = g.Name;
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>(); g.GetBlocks(blocks);
        foreach (var b in blocks) {
            string ln = b.CustomName.ToLower();
            if (b is IMyDoor) { 
                if (ln.Contains(P.KEYWORD_AIRLOCK_INNER.ToLower())) Inner = (IMyDoor)b; 
                else if (ln.Contains(P.KEYWORD_AIRLOCK_OUTER.ToLower())) Outer = (IMyDoor)b; 
            }
            else if(b is IMySensorBlock && ln.Contains(P.KEYWORD_SENSOR.ToLower())) { 
                if(ln.Contains(P.KEYWORD_AIRLOCK_INNER.ToLower())) InnerSensors.Add((IMySensorBlock)b); 
                else if(ln.Contains(P.KEYWORD_AIRLOCK_OUTER.ToLower())) OuterSensors.Add((IMySensorBlock)b); 
            }
            else if (b is IMyAirVent) Vents.Add((IMyAirVent)b); 
            else if (b is IMyLightingBlock) Lights.Add((IMyLightingBlock)b); 
            else if (b is IMyTextPanel) Lcds.Add((IMyTextPanel)b);
        }
        IsValid = (Inner != null && Outer != null);
    }
    public float GetOxygen() { if(Vents.Count == 0) return 0f; float max=0f; foreach(var v in Vents) if(v.GetOxygenLevel()>max) max=v.GetOxygenLevel(); return max; }
    bool SensorsActive(List<IMySensorBlock> sensors) { foreach(var s in sensors) if(s.IsActive) return true; return false; }
    public void Run(double dt) {
        if (!IsValid) return;
        float oxy = GetOxygen();
        switch (CurrentState) {
            case State.Idle:
                if (oxy <= P.OXY_LOW) { StatusText="VACUUM"; SetVisuals(P.COLOR_VACUUM, "VACUUM"); } else { StatusText="PRESSURIZED"; SetVisuals(P.COLOR_SAFE, "PRESSURIZED"); }
                if(!Inner.Enabled) Inner.Enabled=true; if(!Outer.Enabled) Outer.Enabled=true;
                bool reqInner = SensorsActive(InnerSensors) || Inner.OpenRatio > 0;
                bool reqOuter = SensorsActive(OuterSensors) || Outer.OpenRatio > 0;
                if (reqInner) { 
                    if(oxy < P.OXY_HIGH && Vents.Count > 0) { GoingToSpace=false; CurrentState = State.Cycling; } 
                    else { Inner.OpenDoor(); GoingToSpace=true; Timer=0; CurrentState=State.Entry_Wait; }
                } else if (reqOuter) { 
                    if(oxy > P.OXY_LOW && Vents.Count > 0) { GoingToSpace=true; CurrentState = State.Cycling; } 
                    else { Outer.OpenDoor(); GoingToSpace=false; Timer=0; CurrentState=State.Entry_Wait; }
                }
                break;
            case State.Entry_Wait:
                if ((GoingToSpace && SensorsActive(InnerSensors)) || (!GoingToSpace && SensorsActive(OuterSensors))) Timer = 0; else Timer += dt;
                StatusText = $"WAIT {P.AIRLOCK_CYCLE_TIME - Timer:0.0}s";
                if (GoingToSpace) EnsureClosed(Outer); else EnsureClosed(Inner);
                if (Timer >= P.AIRLOCK_CYCLE_TIME) CurrentState = State.Entry_Closing;
                break;
            case State.Entry_Closing:
                StatusText = "CLOSING"; EnsureClosed(Inner); EnsureClosed(Outer);
                if (Inner.Status == DoorStatus.Closed && Outer.Status == DoorStatus.Closed) CurrentState = State.Cycling;
                break;
            case State.Cycling:
                EnsureClosed(Inner); EnsureClosed(Outer); bool done = false;
                if (GoingToSpace) { StatusText = "DEPRESSURIZING"; SetVisuals(P.COLOR_CYCLING, $"CYCLING {oxy:P0}"); SetVents(true); if (oxy <= P.OXY_LOW) done = true; }
                else { StatusText = "PRESSURIZING"; SetVisuals(P.COLOR_CYCLING, $"CYCLING {oxy:P0}"); SetVents(false); if (oxy >= P.OXY_HIGH) done = true; }
                if (done) CurrentState = State.Exit_Opening;
                break;
            case State.Exit_Opening:
                StatusText = "OPENING";
                if (GoingToSpace) { SetVisuals(P.COLOR_VACUUM, "EXIT OPEN"); if(!Outer.Enabled) Outer.Enabled=true; Outer.OpenDoor(); }
                else { SetVisuals(P.COLOR_SAFE, "ENTRY OPEN"); if(!Inner.Enabled) Inner.Enabled=true; Inner.OpenDoor(); }
                Timer = 0; CurrentState = State.Exit_Wait;
                break;
            case State.Exit_Wait:
                if ((GoingToSpace && SensorsActive(OuterSensors)) || (!GoingToSpace && SensorsActive(InnerSensors))) Timer = 0; else Timer += dt;
                StatusText = $"OPEN {P.AIRLOCK_CYCLE_TIME - Timer:0.0}s";
                if (Timer >= P.AIRLOCK_CYCLE_TIME) CurrentState = State.Exit_Closing;
                break;
            case State.Exit_Closing:
                StatusText = "CLOSING"; EnsureClosed(Inner); EnsureClosed(Outer);
                if (Inner.Status == DoorStatus.Closed && Outer.Status == DoorStatus.Closed) CurrentState = State.Idle;
                break;
        }
    }
    void EnsureClosed(IMyDoor d) { if (d.Status != DoorStatus.Closed) { d.Enabled=true; d.CloseDoor(); } }
    void SetVents(bool drain) { foreach(var v in Vents) { v.Enabled=true; v.Depressurize=drain; } }
    void SetVisuals(Color c, string t) { foreach(var l in Lights) l.Color=c; foreach(var lcd in Lcds) { lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; lcd.WriteText(t); lcd.FontColor=c; } }
    public string GetStatusLine() { return $"{Name}: {StatusText} ({GetOxygen():P0})"; }
    public string GetShortStatus() { return $"{Name}: {StatusText}"; }
}