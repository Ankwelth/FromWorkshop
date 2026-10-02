// Runway Chase Lights Script
// Erstellt von Throlon
// Frei nutzbar
// bei Fragen einfach kontaktieren


// =====================================================================
//                ++++++++++++ CONFIG / KONFIG ++++++++++++
// =====================================================================
// EN: Lights name prefix (blocks must be consecutively numbered at the end)
// DE: Namens-Präfix der Lichter (am Ende fortlaufend nummeriert)
const string NAME_PREFIX = "Runway Light";

// EN: Step time in milliseconds (lower = faster; min 1)
// DE: Schrittzeit in Millisekunden (kleiner = schneller; min 1)
int STEP_MS   = 30;

// EN: Lit lamps per head (>=1) — the "tail"/chaser length
// DE: Anzahl gleichzeitig leuchtender Lampen pro Head (>=1) — "Schweif"
int TAIL_LEN  = 2;

// EN: Parallel chasers (“heads”) running in one direction (1..lamp count)
// DE: Anzahl paralleler Lauflichter/„Heads“ in eine Richtung (1..Anzahl Lichter)
int HEADS     = 3;

// EN: Start running automatically (true) or start paused (false)
// DE: Automatisch starten (true) oder pausiert starten (false)
bool RUNNING  = true;


// =====================================================================
//          ++++++++++++ CODE – DO NOT EDIT / NICHT ÄNDERN ++++++++++++
// =====================================================================
List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
List<int> headPos = new List<int>();
HashSet<int> currentlyOn = new HashSet<int>();
double accMs = 0;
bool running = true;

public Program() {
    running = RUNNING;
    Runtime.UpdateFrequency = UpdateFrequency.Update1; // ~60 Hz
    Rebuild();
    EchoStatus();
}

public void Main(string argument, UpdateType updateSource) {
    if (!running || lights.Count == 0 || headPos.Count == 0) return;

    accMs += Runtime.TimeSinceLastRun.TotalMilliseconds;
    if (accMs < STEP_MS) return;

    int steps = (int)(accMs / STEP_MS);
    accMs -= steps * STEP_MS;

    for (int s = 0; s < steps; s++) {
        // Advance all heads one step
        // Alle Heads einen Schritt weiter
        for (int h = 0; h < headPos.Count; h++) {
            int p = headPos[h] + 1;
            if (p >= lights.Count) p = 0;
            headPos[h] = p;
        }

        // Build target set (union of all heads + tails)
        // Zielmenge (Vereinigung aller Heads + Schweife)
        HashSet<int> targetOn = new HashSet<int>();
        for (int h = 0; h < headPos.Count; h++) {
            int head = headPos[h];
            for (int k = 0; k < TAIL_LEN; k++) {
                int j = head - k;
                if (j < 0) j += lights.Count;
                targetOn.Add(j);
            }
        }

        // Diff OFF
        foreach (int i in currentlyOn) {
            if (!targetOn.Contains(i)) SafeSet(lights[i], false);
        }
        // Diff ON
        foreach (int i in targetOn) {
            if (!currentlyOn.Contains(i)) SafeSet(lights[i], true);
        }

        currentlyOn = targetOn;
    }
}

// ----------------- Helpers -----------------
void SafeSet(IMyLightingBlock l, bool on) {
    if (l == null || l.Closed) return;
    if (l.Enabled != on) l.Enabled = on;
}

void Rebuild() {
    lights.Clear();
    currentlyOn.Clear();
    headPos.Clear();

    // Collect lights on same grid, with correct prefix, and valid trailing number
    // Lichter sammeln: gleiche Grid, richtiger Präfix, gültige Endzahl
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(lights, delegate(IMyLightingBlock l) {
        if (l == null || l.Closed) return false;
        if (l.CubeGrid != Me.CubeGrid) return false;
        if (!l.CustomName.StartsWith(NAME_PREFIX)) return false;
        int n = ExtractTrailingNumber(l.CustomName);
        return n != int.MinValue;
    });

    // Sort ascending by trailing number
    // Sortierung nach Zahl am Namensende (aufsteigend)
    lights.Sort(delegate (IMyLightingBlock a, IMyLightingBlock b) {
        int na = ExtractTrailingNumber(a.CustomName);
        int nb = ExtractTrailingNumber(b.CustomName);
        return na.CompareTo(nb);
    });

    // Disable native blinking; all off
    // Eigen-Blinken aus; alles aus
    for (int i = 0; i < lights.Count; i++) {
        IMyLightingBlock l = lights[i];
        if (l == null || l.Closed) continue;
        try {
            l.BlinkIntervalSeconds = 0f;
            l.BlinkLength = 50f;
            l.BlinkOffset = 0f;
        } catch { }
        l.Enabled = false;
    }

    InitHeads();
    Runtime.UpdateFrequency = lights.Count > 0 ? UpdateFrequency.Update1 : UpdateFrequency.None;
}

void InitHeads() {
    headPos.Clear();
    if (lights.Count == 0) return;

    // Clamp heads
    // HEADS begrenzen
    if (HEADS < 1) HEADS = 1;
    if (HEADS > lights.Count) HEADS = lights.Count;

    int spacing = lights.Count / HEADS;
    if (spacing < 1) spacing = 1;

    // Distribute start positions evenly; start at -1 because we step +1 first
    // Startpositionen gleichmäßig; -1, da der erste Tick +1 macht
    for (int h = 0; h < HEADS; h++) {
        int pos = (h * spacing) - 1;
        while (pos < 0) pos += lights.Count;
        headPos.Add(pos % lights.Count);
    }

    // Ensure all off after (re)build
    // Beim Neuaufbau alles aus
    currentlyOn.Clear();
    for (int i = 0; i < lights.Count; i++) SafeSet(lights[i], false);
}

int ExtractTrailingNumber(string s) {
    if (s == null || s.Length == 0) return int.MinValue;
    int num = 0;
    int mul = 1;
    bool has = false;
    for (int i = s.Length - 1; i >= 0; i--) {
        char c = s[i];
        if (c >= '0' && c <= '9') {
            has = true;
            num += (c - '0') * mul;
            mul *= 10;
        } else if (has) break;
    }
    return has ? num : int.MinValue;
}

void EchoStatus() {
    Echo("Runway Chase Lights");
    Echo("Lights: " + lights.Count);
    Echo("speed(ms): " + STEP_MS);
    Echo("tail: " + TAIL_LEN);
    Echo("heads: " + HEADS);
    Echo("running: " + (running ? "yes" : "no"));
}
