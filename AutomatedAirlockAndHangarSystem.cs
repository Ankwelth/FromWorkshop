const string TAG_AIRLOCK = "[ALOCK]";
const string TAG_HANGAR = "[HANGAR]";

Dictionary<string, Airlock> airlocks = new Dictionary<string, Airlock>();
Dictionary<string, Hangar> hangars = new Dictionary<string, Hangar>();

int uidCounter = 0;
int updateMode = 100;
public double blinkFrequency = 1.25;

public Program()
{
    LoadConfig();
    ClearAllLCDs();
    LoadAirlocks();
    LoadHangars();
}

public void Main(string argument, UpdateType updateSource)
{
    Echo("=== AIRLOCK SYSTEM ===");

    if (!string.IsNullOrWhiteSpace(argument))
        HandleCommand(argument);

    foreach (var a in airlocks.Values)
    {
        a.Update();
        Echo(a.GetDebug());
    }

    foreach (var h in hangars.Values)
    {
        h.Update();
        Echo(h.GetDebug());
    }
}

void HandleCommand(string arg)
{
    var parts = arg.Split(' ');
    
    if (parts.Length == 0) return;

    
    string cmd = parts[0].ToLower();
    // ================= PARSE FLAGS =================
    string side = null;
    string airlockId = null;
    bool lockdown = false;
    string airlockOpenTarget = null;
    string hangarAirlockOpenTarget = null;

    if (cmd == "reload")
    {
        airlocks.Clear();
        hangars.Clear();
        uidCounter = 0;
        LoadAirlocks();
        LoadHangars();
        return;
    }

    for (int i = 1; i < parts.Length; i++)
    {
        var p = parts[i];

        if (p.StartsWith("--side="))
            side = p.Substring(7).ToLower();

        else if (p.StartsWith("--airlock="))
            airlockId = p.Substring(10);

        else if (p == "--lockdown")
            lockdown = true;

        else if (p == "--open-interior")
            airlockOpenTarget = "interior";

        else if (p == "--open-exterior")
            airlockOpenTarget = "exterior";

        else if (p == "--open-inner")
            hangarAirlockOpenTarget = "inner";

        else if (p == "--open-access")
            hangarAirlockOpenTarget = "access";
    }

    // ================= EXTRACT GROUP NAME =================
    var nameParts = parts.Skip(1).TakeWhile(p => !p.StartsWith("--"));
    string groupName = string.Join(" ", nameParts).Trim();

    string airlockKey = FindGroupKey(airlocks, groupName, TAG_AIRLOCK);
    string hangarKey = FindGroupKey(hangars, groupName, TAG_HANGAR);

    if (cmd == "airlock")
    {
        if (airlockKey != null)
            airlocks[airlockKey].Toggle(airlockOpenTarget);
        return;
    }

    if (cmd == "hangar")
    {
        if (hangarKey != null)
        {
            var h = hangars[hangarKey];

            if (lockdown)
            {
                h.ToggleLockdown();
                return;
            }

            if (airlockId != null)
            {
                if (hangarAirlockOpenTarget != null)
                    h.OpenHangarAirlockDoor(airlockId, hangarAirlockOpenTarget);
                else
                    h.ToggleAirlock(airlockId);
                return;
            }

            int setIndex = -1;
            if (side == "port") setIndex = 0;
            else if (side == "starboard") setIndex = 1;

            h.ToggleSet(setIndex);
        }
        return;
    }

    if (cmd == "toggle")
    {
        if (airlockKey != null)
        {
            airlocks[airlockKey].Toggle(airlockOpenTarget);
            return;
        }

        if (hangarKey != null)
        {
            var h = hangars[hangarKey];

            if (lockdown)
            {
                h.ToggleLockdown();
                return;
            }

            if (airlockId != null)
            {
                if (hangarAirlockOpenTarget != null)
                    h.OpenHangarAirlockDoor(airlockId, hangarAirlockOpenTarget);
                else
                    h.ToggleAirlock(airlockId);
                return;
            }

            int setIndex = -1;
            if (side == "port") setIndex = 0;
            else if (side == "starboard") setIndex = 1;

            h.ToggleSet(setIndex);
            return;
        }
    }
}

string FindGroupKey<T>(Dictionary<string, T> dict, string groupName, string tag)
{
    if (string.IsNullOrWhiteSpace(groupName))
        return null;

    string normalizedInput = groupName.Trim();
    if (normalizedInput.StartsWith(tag, StringComparison.OrdinalIgnoreCase))
        normalizedInput = normalizedInput.Substring(tag.Length).Trim();

    foreach (var key in dict.Keys)
    {
        string normalizedKey = key;
        if (normalizedKey.StartsWith(tag, StringComparison.OrdinalIgnoreCase))
            normalizedKey = normalizedKey.Substring(tag.Length).Trim();

        if (normalizedKey.Equals(normalizedInput, StringComparison.OrdinalIgnoreCase))
            return key;
    }

    return null;
}

void LoadConfig()
{
    var lines = Me.CustomData.Split('\n');

    foreach (var line in lines)
    {
        var l = line.Trim();

        if (l.StartsWith("UpdateMode="))
        {
            int parsedMode;
            int.TryParse(l.Substring(11), out parsedMode);
            if (parsedMode == 100)
                updateMode = 100;
            else
                updateMode = 10;
        }

        if (l.StartsWith("BlinkFrequency="))
        {
            int parsedFreq;
            int.TryParse(l.Substring(15), out parsedFreq);
            if (parsedFreq > 0)
                blinkFrequency = parsedFreq;

        }
    }

    switch (updateMode)
    {
        case 100:
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            break;
        case 10:
        default:
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            break;
    }
}

void LoadAirlocks()
{
    var groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);

    foreach (var g in groups)
    {
        if (!g.Name.Contains(TAG_AIRLOCK)) continue;

        var a = new Airlock(g, uidCounter++, this);

        if (a.Valid)
            airlocks[g.Name] = a;
    }
}

void LoadHangars()
{
    var groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);

    foreach (var g in groups)
    {

        if (!g.Name.Contains(TAG_HANGAR)) continue;

        var h = new Hangar(g, this);
        hangars[g.Name] = h;
    }
}

void ClearAllLCDs()
{
    var panels = new List<IMyTextSurfaceProvider>();
    GridTerminalSystem.GetBlocksOfType(panels);

    foreach (var p in panels)
    {
        for (int i = 0; i < p.SurfaceCount; i++)
        {
            var surface = p.GetSurface(i);
            surface.WriteText("", false);
        }
    }
}
// =================== AIRLOCK CLASS ===================
class Airlock
{
    public string Name;
    public int UID;
    public bool Valid = false;

    IMyDoor interior;
    IMyDoor exterior;
    IMyAirVent vent;
    IMyTextSurface lcd = null;
    IMyLightingBlock light = null;
    IMySoundBlock sound = null;

    MyGridProgram pg;

    enum State { Normal, Pressurizing, Depressurizing }
    State state = State.Normal;

    bool wantInteriorOpen = false;
    bool wantExteriorOpen = false;
    bool soundPlaying = false;

    int depressTicks = 0;
    int pressTicks = 0;
    const int maxDepressTicks = 100;
    const int maxPressTicks = 100;

    int toggleCooldown = 0;
    const int minToggleTicks = 2;

    int openTicks = 0;
    const int maxOpenTicks = 50;

    double blinkTime = 0;

    public Airlock(IMyBlockGroup group, int uid, MyGridProgram program)
    {
        Name = group.Name;
        UID = uid;
        pg = program;

        var blocks = new List<IMyTerminalBlock>();
        group.GetBlocks(blocks);

        var doors = blocks.OfType<IMyDoor>().ToList();
        var vents = blocks.OfType<IMyAirVent>().ToList();
        var lcds = blocks.OfType<IMyTextPanel>().ToList();
        var lights = blocks.OfType<IMyLightingBlock>().ToList();
        var sounds = blocks.OfType<IMySoundBlock>().ToList();

        interior = doors.FirstOrDefault(d => d.CustomName.Contains("Interior"));
        exterior = doors.FirstOrDefault(d => d.CustomName.Contains("Exterior"));

        if (vents.Count != 1)
        {
            pg.Echo($"[ERROR] {Name}: must have exactly 1 vent");
            return;
        }
        vent = vents[0];

        if (interior == null || exterior == null)
        {
            pg.Echo($"[ERROR] {Name}: missing Interior/Exterior doors");
            return;
        }

        // Optional LCD
        if (lcds.Count > 1)
            pg.Echo($"[WARN] {Name}: multiple LCDs, using first");
        if (lcds.Count >= 1)
            lcd = lcds[0];

        // Optional Light
        if (lights.Count > 1)
            pg.Echo($"[WARN] {Name}: multiple lights, using first");
        if (lights.Count >= 1)
            light = lights[0];

        if (sounds.Count > 0)
            sound = sounds[0];
        

        Valid = true;
    }

   public void Toggle(string target = null)
    {
        if (!Valid) return;
        if (toggleCooldown > 0) return;

        toggleCooldown = minToggleTicks;

        bool interiorOpen =
        interior.Status == DoorStatus.Open ||
        interior.Status == DoorStatus.Opening;

        bool exteriorOpen =
            exterior.Status == DoorStatus.Open ||
            exterior.Status == DoorStatus.Opening;

        if (target == "interior" && interiorOpen && !exteriorOpen)
            return;

        if (target == "exterior" && exteriorOpen && !interiorOpen)
            return;
        
        interior.CloseDoor();
        exterior.CloseDoor();
        depressTicks = 0;

        // ================= EXPLICIT TARGET =================
        if (target == "interior")
        {
            state = State.Pressurizing;
            pressTicks = 0;
            wantInteriorOpen = true;
            wantExteriorOpen = false;
            return;
        }

        if (target == "exterior")
        {
            state = State.Depressurizing;
            depressTicks = 0;
            wantExteriorOpen = true;
            wantInteriorOpen = false;
            return;
        }

        // ================= DEFAULT TOGGLE =================
        if (state == State.Depressurizing)
        {
            state = State.Pressurizing;
            wantInteriorOpen = true;
            wantExteriorOpen = false;
        }
        else
        {
            state = State.Depressurizing;
            wantExteriorOpen = true;
            wantInteriorOpen = false;
        }
    }
    

    public void Update()
    {
        if (!Valid) return;

        if (toggleCooldown > 0) toggleCooldown--;

        float oxy = vent.GetOxygenLevel();

        blinkTime += pg.Runtime.TimeSinceLastRun.TotalSeconds;
        double freq = ((Program)pg).blinkFrequency;
        bool blink = ((int)(blinkTime * freq * 2)) % 2 == 0;
        // ================= DOOR INTENT =================

        
        if (interior.Status == DoorStatus.Opening)
        {
            state = State.Pressurizing;
            wantInteriorOpen = true;
            wantExteriorOpen = false;
        }

        if (exterior.Status == DoorStatus.Opening)
        {
            state = State.Depressurizing;
            wantExteriorOpen = true;
            wantInteriorOpen = false;
        }

        // ================= FAIL-SAFE =================
        bool interiorOpen = interior.Status == DoorStatus.Open || interior.Status == DoorStatus.Opening;
        bool exteriorOpen = exterior.Status == DoorStatus.Open || exterior.Status == DoorStatus.Opening;

        if (interiorOpen && exteriorOpen)
        {
            if (state == State.Pressurizing)
                exterior.CloseDoor();
            else if (state == State.Depressurizing)
                interior.CloseDoor();
        }

        // ================= STATE LOGIC =================
        switch (state)
        {
            case State.Pressurizing:
                vent.Depressurize = false;

                // Ensure exterior is closed first
                if (exterior.Status != DoorStatus.Closed)
                {
                    exterior.CloseDoor();
                    break;
                }

                if (oxy > 0.95f || pressTicks >= maxPressTicks)
                {
                    if (wantInteriorOpen)
                        interior.OpenDoor();

                    state = State.Normal;
                    pressTicks = 0;
                    StopSound();
                }
                else
                {
                    pressTicks++;
                    PlaySound("Alert 2");
                }
                break;

            case State.Depressurizing:
                if (interior.Status == DoorStatus.Closed &&
                    exterior.Status == DoorStatus.Closed)
                {
                    vent.Depressurize = true;
                    depressTicks++;

                    if (oxy <= 0.05f || depressTicks >= maxDepressTicks)
                    {
                        if (wantExteriorOpen)
                            exterior.OpenDoor();

                        StopSound();
                        state = State.Normal;
                        depressTicks = 0;
                    }
                    else
                    {
                        PlaySound("Alert 2");
                    }
                }
                break;
            
        }

        // ================= AUTO CLOSE =================
        bool anyFullyOpen =
        interior.Status == DoorStatus.Open ||
        exterior.Status == DoorStatus.Open;

        if (anyFullyOpen)
        {
            openTicks++;

            if (openTicks > maxOpenTicks)
            {
                interior.CloseDoor();
                exterior.CloseDoor();

                wantInteriorOpen = false;
                wantExteriorOpen = false;

                state = State.Normal;
                openTicks = 0;
            }
        }
        else
        {
            openTicks = 0;
        }
        // ================= IDLE STATE =================
        if (state == State.Normal &&
            interior.Status == DoorStatus.Closed &&
            exterior.Status == DoorStatus.Closed)
        {
            vent.Depressurize = false;
            StopSound();
        }

        UpdateLight(blink);
        UpdateLCD(oxy);
    }

   void UpdateLight(bool blink)
{
    if (light == null) return;

    bool interiorMoving =
        interior.Status == DoorStatus.Opening ||
        interior.Status == DoorStatus.Closing;

    bool exteriorMoving =
        exterior.Status == DoorStatus.Opening ||
        exterior.Status == DoorStatus.Closing;

    bool anyMoving = interiorMoving || exteriorMoving;

    bool pressureCycle =
        (state == State.Pressurizing ||
        state == State.Depressurizing) && !anyMoving;

    bool anyClosing = interior.Status == DoorStatus.Closing ||
        exterior.Status == DoorStatus.Closing;
    
    bool anyOpening = interior.Status == DoorStatus.Opening ||
        exterior.Status == DoorStatus.Opening;

    // ================= PRESSURE CYCLE =================
    // Blue blinking
    if (pressureCycle)
    {
        light.Color =
            blink
            ? new Color(0, 0, 255)
            : new Color(0, 0, 0);

        return;
    }

    // ================= DOOR MOVEMENT =================
    // Red blinking
    if (anyClosing)
    {
        light.Color =
            blink
            ? new Color(255, 0, 0)
            : new Color(0, 0, 0);

        return;
    }

     if (anyOpening)
    {
        light.Color =
            blink
            ? new Color(0, 255, 0)
            : new Color(0, 0, 0);

        return;
    }

    // ================= IDLE =================
    // White solid
    light.Color = new Color(255, 255, 255);
}

    void UpdateLCD(float oxy)
    {
        if (lcd == null) return;

        string text =
            $@"[{UID}] {Name}
            State: {state}
            O2: {(oxy*100f):0.0}%
            VentDepress: {vent.Depressurize}
            Interior: {interior.Status}
            Exterior: {exterior.Status}";

        lcd.WriteText(text, false);
    }

    void PlaySound(string cue)
    {
        if (sound == null) return;

        if (sound.SelectedSound != cue)
        {
            sound.SelectedSound = cue;
        }

        if (!soundPlaying)
        {
            sound.Play();
            soundPlaying = true;
        }
    }

    void StopSound()
    {
        if (sound == null) return;
        if (soundPlaying)
        {
            sound.Stop();
            soundPlaying = false;
        }
    }

    public string GetDebug()
    {
        return $"[{UID}] {Name} | {state}";
    }
}

// =================== HANGAR CLASS ===================
class Hangar
{
    public string Name;

    // ================= AIRLOCK SEGMENTS =================
    class AirlockSegment
    {
        public string id;

        public IMyDoor innerDoor;
        public IMyDoor accessDoor;
        public List<IMyAirVent> vents = new List<IMyAirVent>();

        public State state = State.Idle;
        public bool wantInner = false;
        public bool wantAccess = false;
        public bool lastWasInner = true;
        public bool pending = false;

        public int ticks = 0;
        public int pressTicks = 0;
        public int maxPressTicks = 100;
        public int inputCooldown = 0;

        public enum State
        {
            Idle,
            ClosingInner,
            ClosingAccess,
            Depressurizing,
            Pressurizing,
            OpeningInner,
            OpeningAccess
        }
    }

    List<AirlockSegment> airlocks = new List<AirlockSegment>();

    // ================= HANGAR =================
    List<IMyDoor> hangarDoors = new List<IMyDoor>();
    List<List<IMyDoor>> doorSets = new List<List<IMyDoor>>();

    List<IMyAirVent> hangarVents = new List<IMyAirVent>();

    IMyTextSurface lcd = null;

    enum State { Normal, Pressurizing, Depressurizing, Open }
    State targetState = State.Normal;

    int desiredSet = -1;

    int ticks = 0;
    int openTicks = 0;
    const int maxOpenTicks = 50;

    int pressurizeTicks = 0;
    const int maxPressurizeTicks = 100;

    int closedTicks = 0;
    const int maxClosedTicks = 15;

    bool lockdown = false;

    int toggleCooldown = 0;
    const int minToggleTicks = 2;

    MyGridProgram pg;

    // ================= INIT =================
    public Hangar(IMyBlockGroup group, MyGridProgram program)
    {
        pg = program;
        Name = group.Name;

        var blocks = new List<IMyTerminalBlock>();
        group.GetBlocks(blocks);

        var allDoors = blocks.OfType<IMyDoor>().ToList();
        var allVents = blocks.OfType<IMyAirVent>().ToList();

        var innerMap = new Dictionary<string, List<IMyDoor>>();
        var accessMap = new Dictionary<string, List<IMyDoor>>();
        var ventMap = new Dictionary<string, List<IMyAirVent>>();

        foreach (var d in allDoors)
        {
            string name = d.CustomName;

            if (name.Contains("ShipInterior"))
            {
                string key = ExtractIndex(name);
                if (!innerMap.ContainsKey(key)) innerMap[key] = new List<IMyDoor>();
                innerMap[key].Add(d);
            }
            else if (name.Contains("HangarAccess"))
            {
                string key = ExtractIndex(name);
                if (!accessMap.ContainsKey(key)) accessMap[key] = new List<IMyDoor>();
                accessMap[key].Add(d);
            }
            else
            {
                hangarDoors.Add(d);
            }
        }

        foreach (var v in allVents)
        {
            if (v.CustomName.Contains("AirlockVent"))
            {
                string key = ExtractIndex(v.CustomName);
                if (!ventMap.ContainsKey(key)) ventMap[key] = new List<IMyAirVent>();
                ventMap[key].Add(v);
            }
            else
            {
                hangarVents.Add(v);
            }
        }

        foreach (var key in accessMap.Keys.Union(innerMap.Keys).OrderBy(k => k))
        {
            var seg = new AirlockSegment();
            seg.id = key;

            // ================= INNER DOOR (optional) =================
            if (innerMap.ContainsKey(key) && innerMap[key].Count > 0)
                seg.innerDoor = innerMap[key][0];

            // ================= ACCESS DOOR (optional) =================
            if (accessMap.ContainsKey(key) && accessMap[key].Count > 0)
                seg.accessDoor = accessMap[key][0];

            // ================= VENTS (optional, can be multiple) =================
            if (ventMap.ContainsKey(key))
                seg.vents = ventMap[key];

            // ================= VALIDATION =================
            // Only add segment if it has at least ONE door
            if (seg.innerDoor != null || seg.accessDoor != null)
            {
                airlocks.Add(seg);
            }
            else
            {
                WriteDebug($"[INIT] Skipping airlock {key} (no doors found)");
            }
        }

        var port = hangarDoors.Where(d => d.CustomName.Contains("Port")).ToList();
        var star = hangarDoors.Where(d => d.CustomName.Contains("Starboard")).ToList();

        if (port.Count > 0) doorSets.Add(port);
        if (star.Count > 0) doorSets.Add(star);

        var lcds = blocks.OfType<IMyTextPanel>().ToList();
        if (lcds.Count > 0)
        {
            lcd = lcds[0];
            lcd.WriteText("", false); 
        }
        else
        {
            lcd = null; 
        }

        WriteDebug($"[INIT] {Name} | Airlocks={airlocks.Count} | Sets={doorSets.Count}");
    }

    string ExtractIndex(string name)
    {
        var parts = name.Split(' ');
        return parts[parts.Length - 1];
    }

    // ================= COMMANDS =================

   public void ToggleSet(int setIndex)
{
    if (lockdown) return;

    if (setIndex < -1 || setIndex >= doorSets.Count)
        return;

    bool targetOpen;

    if (setIndex == -1)
        targetOpen = hangarDoors.Any(d => d.Status != DoorStatus.Closed);
    else
        targetOpen = doorSets[setIndex].Any(d => d.Status != DoorStatus.Closed);

    // cancel current transition
    if (targetState == State.Depressurizing ||
        targetState == State.Pressurizing)
    {
        targetState = State.Normal;
    }

    if (targetOpen)
    {
        if (setIndex == -1)
            hangarDoors.ForEach(d => d.CloseDoor());
        else
            doorSets[setIndex].ForEach(d => d.CloseDoor());

        targetState = State.Pressurizing;
        pressurizeTicks = 0;

        WriteDebug($"[HANGAR] Closing {(setIndex == -1 ? "ALL" : setIndex.ToString())}");
    }
    else
    {
        desiredSet = setIndex;
        targetState = State.Depressurizing;

        ticks = 0;
        openTicks = 0;

        WriteDebug($"[HANGAR] Opening {(setIndex == -1 ? "ALL" : setIndex.ToString())}");
    }
}

   public void ToggleAirlock(string id)
{
   
    var a = airlocks.FirstOrDefault(x => x.id == id);
    if (a == null) return;

     if (a.inputCooldown > 0)
    {
        WriteDebug($"[AIRLOCK {id}] INPUT BLOCKED {a.inputCooldown}");
        return;
    }

    a.inputCooldown = 1;

    bool hangarBusy =
    targetState == State.Depressurizing || targetState == State.Pressurizing ;
    // =========================
    // CASE 1: already has intent => reverse it (NOT cancel)
    // =========================
    if (a.wantInner || a.wantAccess)
    {
        if (a.wantInner)
        {
            a.wantInner = false;
            a.wantAccess = true;
        }
        else
        {
            a.wantAccess = false;
            a.wantInner = true;
        }

        WriteDebug($"[AIRLOCK {id}] FLIP INTENT");
        return;
    }

    // =========================
    // CASE 2: no intent => choose direction
    // =========================
    if (a.lastWasInner)
    {
        a.wantAccess = true;
        a.lastWasInner = false;
    }
    else
    {
        a.wantInner = true;
        a.lastWasInner = true;
    }

    WriteDebug($"[AIRLOCK {id}] NEW INTENT inner {a.wantInner} access {a.wantAccess}" );
}

    public void OpenHangarAirlockDoor(string id, string target)
    {
        var a = airlocks.FirstOrDefault(x => x.id == id);
        if (a == null)
        {
            WriteDebug($"[ERROR] Airlock {id} not found");
            return;
        }

        float hangarO2 = hangarVents.Count > 0
            ? hangarVents.Min(v => v.GetOxygenLevel())
            : 1f;

        bool hangarPressurized = hangarO2 > 0.8f;

        bool hangarBusy =
    targetState == State.Depressurizing || targetState == State.Pressurizing ;

        if (target == "inner")
        {
            a.wantInner = true;
            a.wantAccess = false;
            a.lastWasInner = true;
             WriteDebug($"[AIRLOCK {id}] Opening {target}");
        }
        else
        {
            a.wantAccess = true;
            a.wantInner = false;
            a.lastWasInner = false;
            WriteDebug($"[AIRLOCK {id}] Opening {target}");
        }

        if (hangarBusy)
        {
            a.pending = true;
            WriteDebug($"[AIRLOCK {id}] QUEUED {target}");
            return;
        }

        if (hangarPressurized)
        {
            a.wantInner = (target == "inner");
            a.wantAccess = (target == "access");
            WriteDebug($"[AIRLOCK {id}] OPEN {target.ToUpper()} (SAFE)");
            return;
        }

        if (target == "inner")
            a.state = AirlockSegment.State.ClosingAccess;
        else
            a.state = AirlockSegment.State.ClosingInner;

        
        WriteDebug($"[AIRLOCK {id}] CYCLE → {target.ToUpper()}");
    }

    public void ToggleLockdown()
    {
        lockdown = !lockdown;

        if (lockdown)
        {
            hangarDoors.ForEach(d => d.CloseDoor());

            foreach (var a in airlocks)
            {
                a.innerDoor.CloseDoor();
                a.accessDoor.CloseDoor();
            }

            hangarVents.ForEach(v => v.Depressurize = false);

            WriteDebug("[LOCKDOWN] ENABLED");
        }
        else
        {
            WriteDebug("[LOCKDOWN] DISABLED");
        }
    }

    // ================= UPDATE HANGAR =================
    public void Update()
    {
        float hangarO2 = hangarVents.Count > 0
            ? hangarVents.Min(v => v.GetOxygenLevel())
            : 1f;

        bool hangarClosed = hangarDoors.All(d => d.Status == DoorStatus.Closed);
        bool accessClosed = airlocks.All(a => a.accessDoor.Status == DoorStatus.Closed);
        bool innerClosed = airlocks.All(a => a.innerDoor.Status == DoorStatus.Closed);
        bool anyAirlockActive = airlocks.Any(a => a.state != AirlockSegment.State.Idle);
        bool anyHangarOpen = hangarDoors.Any(d => d.Status == DoorStatus.Open);

        WriteDebug($"[Hangar.Update] State={targetState}, O2={hangarO2:P0}, ticks={ticks}, closed={hangarClosed}/{accessClosed}/{innerClosed}");

        // ================= DEPRESSURIZE =================
        if (targetState == State.Depressurizing)
        {
            if (!accessClosed)
            {
                foreach (var a in airlocks)
                    a.accessDoor.CloseDoor();

                WriteDebug("[DEPRESSURIZE] Waiting for access doors to close");
                return;
            }
            if (!innerClosed)
            {
                foreach (var a in airlocks)
                    a.innerDoor.CloseDoor();

                WriteDebug("[DEPRESSURIZE] Waiting for inner doors to close");
                return;
            }

            if (!hangarClosed)
            {
                hangarDoors.ForEach(d => d.CloseDoor());
                WriteDebug("[DEPRESSURIZE] Waiting for hangar doors to close");
                return;
            }
            hangarVents.ForEach(v => v.Depressurize = true);
            ticks++;
            WriteDebug($"[DEPRESSURIZE] Active, O2={hangarO2:P0}, ticks={ticks}");

            if (hangarO2 <= 0.05f)
            {
                OpenDesiredSet();
                targetState = State.Open;
                ticks = 0;
                WriteDebug("[DEPRESSURIZE] Complete, transitioning to OPEN");
            }
            return;
        }

        // ================= PRESSURIZE =================
        else if (targetState == State.Pressurizing)
        {
            if (anyHangarOpen)
            {
                hangarDoors.ForEach(d => d.CloseDoor());
                return;
            }

            foreach (var a in airlocks)
                foreach (var v in a.vents)
                    v.Depressurize = false;

            hangarVents.ForEach(v => v.Depressurize = false);
            pressurizeTicks++;

            if (hangarO2 >= 0.98f || pressurizeTicks >= maxPressurizeTicks)
            {
                targetState = State.Normal;
                pressurizeTicks = 0;
            }
        }

        // ================= OPEN =================
        else if (targetState == State.Open)
        {
            // Maintain open state while doors are open
        }

        // ================= AUTO CLOSE =================
        if (anyHangarOpen)
        {
            openTicks++;

            if (openTicks >= maxOpenTicks)
            {
                hangarDoors.ForEach(d => d.CloseDoor());
                targetState = State.Pressurizing;
                pressurizeTicks = 0;
                openTicks = 0;

                WriteDebug("[AUTO] Closing hangar");
            }
        }
        else
        {
            openTicks = 0;
        }

        // allow airlock override of hangar transitions
        if (airlocks.Any(a => a.state != AirlockSegment.State.Idle))
        {
            closedTicks = 0;
        }

        // ================= AUTO PRESSURIZE =================
        if (hangarClosed && accessClosed && !anyAirlockActive)
        {
            closedTicks++;
            if (closedTicks >= maxClosedTicks && targetState == State.Normal)
            {
                targetState = State.Pressurizing;
                pressurizeTicks = 0;
                ticks = 0;

                WriteDebug("[AUTO] Pressurizing");
            }
        }
        else
        {
            closedTicks = 0;
        }

        // ================= STATE SELF-HEAL =================
        if (targetState != State.Normal)
        {
            bool hangarStable =
                hangarDoors.All(d => d.Status == DoorStatus.Closed) &&
                hangarVents.Min(v => v.GetOxygenLevel()) > 0.95f;

            bool noAirlockActivity =
                !airlocks.Any(a => a.state != AirlockSegment.State.Idle);

            if (hangarStable && noAirlockActivity)
            {
                targetState = State.Normal;
            }
        }

        // ================= AIRLOCK =================
        UpdateAirlocks(hangarO2);

        // ================= DEBUG =================
        WriteDebug(
            $"State={targetState} | " +
            $"HangarClosed={hangarClosed} | " +
            $"AccessClosed={accessClosed} | " +
            $"O2={(hangarO2 * 100f):0}%"
        );
    }

    // ================= AIRLOCK LOGIC =================
void UpdateAirlocks(float hangarO2)
{
    for (int i = 0; i < airlocks.Count; i++)
    {
        
        var a = airlocks[i];

        if (a.inputCooldown > 0)
            a.inputCooldown--;
        // =====================================================
        // READ STATE
        // =====================================================
        float airlockO2 = 1f;
        if (a.vents.Count > 0)
            airlockO2 = a.vents.Min(v => v.GetOxygenLevel());
        


        bool anyHangarDoorOpen = hangarDoors.Any(d =>
            d.Status != DoorStatus.Closed);

        bool hangarSafe = !anyHangarDoorOpen && hangarO2 > 0.9f;

        bool innerClosed = a.innerDoor.Status == DoorStatus.Closed;
        bool accessClosed = a.accessDoor.Status == DoorStatus.Closed;

        bool innerOpening = a.innerDoor.Status == DoorStatus.Opening;
        bool accessOpening = a.accessDoor.Status == DoorStatus.Opening;

        bool innerOpen =
            a.innerDoor.Status == DoorStatus.Open ||
            a.innerDoor.Status == DoorStatus.Opening;

        bool accessOpen =
            a.accessDoor.Status == DoorStatus.Open ||
            a.accessDoor.Status == DoorStatus.Opening;

        bool hangarBusy =
    targetState == State.Depressurizing || targetState == State.Pressurizing ;

        if (a.wantInner && a.wantAccess){
            a.wantInner = true;
            a.wantAccess = false;
        }   
           
        

        if (hangarBusy)
        {
            // Don't kill FSM, just prevent NEW transitions
            if (a.state == AirlockSegment.State.Idle)
            {
                a.wantInner = false;
                a.wantAccess = false;
            }
        }
        if (!hangarBusy && a.pending)
        {
            a.pending = false;

            // re-apply last intent
            if (a.lastWasInner)
            {
                a.wantInner = true;
                a.wantAccess = false;
            }
            else
            {
                a.wantAccess = true;
                a.wantInner = false;
            }

            WriteDebug($"[AIRLOCK {a.id}] EXECUTING PENDING");
        }
        // FAIL SAFE
        if (innerOpen && accessOpen)
        {
            bool innerMoving = a.innerDoor.Status == DoorStatus.Opening;
            bool accessMoving = a.accessDoor.Status == DoorStatus.Opening;

            // PRIORITY 1: respect movement direction
            if (innerMoving && !accessMoving)
            {
                a.accessDoor.CloseDoor();
                continue;
            }

            if (accessMoving && !innerMoving)
            {
                a.innerDoor.CloseDoor();
                continue;
            }

            // PRIORITY 2: use ACTIVE intent ONLY (never reset it here)
            if (a.wantInner)
            {
                a.accessDoor.CloseDoor();
                continue;
            }

            if (a.wantAccess)
            {
                a.innerDoor.CloseDoor();
                continue;
            }

            // PRIORITY 3: fallback to FSM (only if no intent)
            if (a.state == AirlockSegment.State.Pressurizing)
                a.accessDoor.CloseDoor();
            else if (a.state == AirlockSegment.State.Depressurizing)
                a.innerDoor.CloseDoor();

             // final fallback
            a.accessDoor.CloseDoor();
            continue;
        }

        // =====================================================
        // SAFE MODE (NO FSM, DIRECT CONTROL)
        // =====================================================
        if (hangarSafe)
        {
            foreach (var v in a.vents)
                v.Depressurize = false;

            if (!a.wantInner && !a.wantAccess)
            {
                a.state = AirlockSegment.State.Idle;
                continue;
            }

            // INNER REQUEST
            if (a.wantInner)
            {
                if (!accessClosed)
                {
                    a.accessDoor.CloseDoor();
                    continue;
                }

                if (innerClosed)
                    a.innerDoor.OpenDoor();

                if (innerOpen && accessClosed)
                    a.wantInner = false;

                a.state = AirlockSegment.State.Idle;
                continue;
            }

            // ACCESS REQUEST
            if (a.wantAccess)
            {
                if (!innerClosed)
                {
                    a.innerDoor.CloseDoor();
                    continue;
                }

                if (accessClosed)
                    a.accessDoor.OpenDoor();

                if (accessOpen && innerClosed)
                    a.wantAccess = false;

                a.state = AirlockSegment.State.Idle;
                continue;
            }
        }

        // =====================================================
        // UNSAFE MODE (FSM)
        // =====================================================
        switch (a.state)
        {
            case AirlockSegment.State.Idle:

                if (a.wantInner)
                    a.state = AirlockSegment.State.ClosingAccess;

                else if (a.wantAccess)
                    a.state = AirlockSegment.State.ClosingInner;

                break;

            case AirlockSegment.State.ClosingInner:

                if (!innerClosed)
                    a.innerDoor.CloseDoor();

                if (innerClosed)
                    a.state = AirlockSegment.State.Depressurizing;

                break;

            case AirlockSegment.State.ClosingAccess:
                if (!accessClosed)
                    a.accessDoor.CloseDoor();

                if (accessClosed)
                {
                    a.state = AirlockSegment.State.Pressurizing;
                    a.pressTicks = 0;
                }
                break;

            case AirlockSegment.State.Depressurizing:

                foreach (var v in a.vents)
                    v.Depressurize = true;

                if (airlockO2 < 0.05f)
                    a.state = AirlockSegment.State.OpeningAccess;

                break;

            case AirlockSegment.State.Pressurizing:
                foreach (var v in a.vents)
                    v.Depressurize = false;

                a.pressTicks++;
                if (airlockO2 > 0.95f || a.vents.Count == 0)
                {
                    a.state = AirlockSegment.State.OpeningInner;
                    a.pressTicks = 0;
                }
                break;

            case AirlockSegment.State.OpeningAccess:

                if (a.accessDoor.Status == DoorStatus.Closed)
                    a.accessDoor.OpenDoor();

                a.state = AirlockSegment.State.Idle;
                break;

            case AirlockSegment.State.OpeningInner:

                if (a.innerDoor.Status == DoorStatus.Closed)
                    a.innerDoor.OpenDoor();

                a.state = AirlockSegment.State.Idle;
                break;
        }
    }
    }
    void OpenDesiredSet()
    {
        if (desiredSet == -1)
        {
            hangarDoors.ForEach(d => d.OpenDoor());
        }
        else if (desiredSet < doorSets.Count)
        {
            doorSets[desiredSet].ForEach(d => d.OpenDoor());
        }

        desiredSet = -1;
        openTicks = 0;
    }

    void WriteDebug(string text)
    {
        pg.Echo(text);
        if (lcd != null)
            lcd.WriteText(text + "\n", true);
    }

    public string GetDebug()
    {
        return $"[HANGAR] {Name} | State={targetState}";
    }
}