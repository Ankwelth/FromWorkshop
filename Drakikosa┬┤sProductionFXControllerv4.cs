// Drakikosa´s Production FX Controller v4
// =======================================
// Frei definierbare Gruppen für Raffinerien und Montageanlagen
// Mehrere Timer pro Zustand möglich
// Zweisprachig DE / EN
//
// Beispiel:
// working  -> [REF_ON]  / [REF_OFF]
// working2 -> [REF_ON2] / [REF_OFF2]
//
// Befehle:
// rescan
// status
// trigger ref 0 on
// trigger ref 0 off
// trigger asm 0 ready
// trigger asm 0 work
// trigger asm 0 wait

// ============================================================
// EINSTELLUNGEN
// ============================================================

const string LANGUAGE = "DE"; // DE oder EN

const bool ONLY_SAME_GRID = true;
const UpdateFrequency UPDATE_RATE = UpdateFrequency.Update100;

// Automatischer Rescan nach X Durchläufen.
// Bei Update100 sind 30 ca. 50 Sekunden.
const int AUTO_RESCAN_TICKS = 30;

// ============================================================
// GRUPPEN EINSTELLEN
// HIER ARBEITEST DU!
// ============================================================
//
// AddRefGroup:
// BlockTag, Timer wenn AN, Timer wenn AUS
//
// AddAsmGroup:
// BlockTag, Timer BEREIT, Timer ARBEITET, Timer WARTET
//
// Der BlockTag kommt in Name oder CustomData der Raffinerie / Montageanlage.
// Der TimerTag kommt in Name oder CustomData vom Timerblock.
//
// Beispiel:
// Raffinerie CustomData: working2
// Timer Name: Auspuff Start [REF_ON2]
// Timer Name: Auspuff Stop [REF_OFF2]

void ConfigureGroups()
{
    // -----------------------------
    // RAFFINERIEN
    // -----------------------------

    AddRefGroup("working",  "[REF_ON]",  "[REF_OFF]");
    AddRefGroup("working2", "[REF_ON2]", "[REF_OFF2]");
    AddRefGroup("working3", "[REF_ON3]", "[REF_OFF3]");

    // -----------------------------
    // MONTAGEANLAGEN
    // -----------------------------

    AddAsmGroup("montage",  "[ASM_READY]",  "[ASM_WORK]",  "[ASM_WAIT]");
    AddAsmGroup("montage2", "[ASM_READY2]", "[ASM_WORK2]", "[ASM_WAIT2]");
    AddAsmGroup("montage3", "[ASM_READY3]", "[ASM_WORK3]", "[ASM_WAIT3]");
}

// ============================================================
// AB HIER NICHTS ÄNDERN, WENN DU NICHT MUSST
// ============================================================

enum RefState
{
    Off,
    Working
}

enum AsmState
{
    Ready,
    Working,
    Waiting
}

class RefGroup
{
    public string Name;
    public string BlockTag;
    public string OnTimerTag;
    public string OffTimerTag;

    public List<IMyRefinery> Blocks = new List<IMyRefinery>();
    public List<IMyTimerBlock> OnTimers = new List<IMyTimerBlock>();
    public List<IMyTimerBlock> OffTimers = new List<IMyTimerBlock>();

    public RefState LastState = RefState.Off;
}

class AsmGroup
{
    public string Name;
    public string BlockTag;
    public string ReadyTimerTag;
    public string WorkTimerTag;
    public string WaitTimerTag;

    public List<IMyAssembler> Blocks = new List<IMyAssembler>();
    public List<IMyTimerBlock> ReadyTimers = new List<IMyTimerBlock>();
    public List<IMyTimerBlock> WorkTimers = new List<IMyTimerBlock>();
    public List<IMyTimerBlock> WaitTimers = new List<IMyTimerBlock>();

    public AsmState LastState = AsmState.Ready;
}

List<RefGroup> refGroups = new List<RefGroup>();
List<AsmGroup> asmGroups = new List<AsmGroup>();

List<IMyTimerBlock> allTimers = new List<IMyTimerBlock>();
List<MyProductionItem> queueBuffer = new List<MyProductionItem>();

int tick = 0;

public Program()
{
    Runtime.UpdateFrequency = UPDATE_RATE;

    LoadConfiguration();
    Rescan(true);
}

public void Main(string argument, UpdateType updateSource)
{
    tick++;

    argument = (argument ?? "").Trim();

    if (argument.Equals("rescan", StringComparison.OrdinalIgnoreCase))
    {
        Rescan(true);
        return;
    }

    if (argument.Equals("status", StringComparison.OrdinalIgnoreCase))
    {
        PrintStatus();
        return;
    }

    if (argument.StartsWith("trigger ", StringComparison.OrdinalIgnoreCase))
    {
        ManualTrigger(argument);
        return;
    }

    UpdateRefGroups();
    UpdateAsmGroups();

    if (tick >= AUTO_RESCAN_TICKS)
    {
        tick = 0;
        Rescan(false);
    }

    PrintStatus();
}

// ============================================================
// KONFIGURATION
// ============================================================

void LoadConfiguration()
{
    refGroups.Clear();
    asmGroups.Clear();

    ConfigureGroups();
}

void AddRefGroup(string blockTag, string onTimerTag, string offTimerTag)
{
    RefGroup group = new RefGroup();

    group.Name = blockTag;
    group.BlockTag = blockTag;
    group.OnTimerTag = onTimerTag;
    group.OffTimerTag = offTimerTag;

    refGroups.Add(group);
}

void AddAsmGroup(string blockTag, string readyTimerTag, string workTimerTag, string waitTimerTag)
{
    AsmGroup group = new AsmGroup();

    group.Name = blockTag;
    group.BlockTag = blockTag;
    group.ReadyTimerTag = readyTimerTag;
    group.WorkTimerTag = workTimerTag;
    group.WaitTimerTag = waitTimerTag;

    asmGroups.Add(group);
}

// ============================================================
// SCAN
// ============================================================

void Rescan(bool echo)
{
    allTimers.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(
        allTimers,
        b =>
            b != null &&
            (!ONLY_SAME_GRID || b.CubeGrid == Me.CubeGrid)
    );

    for (int i = 0; i < refGroups.Count; i++)
    {
        ScanRefGroup(refGroups[i]);
    }

    for (int i = 0; i < asmGroups.Count; i++)
    {
        ScanAsmGroup(asmGroups[i]);
    }

    if (echo)
    {
        Echo(Txt("Scan abgeschlossen.", "Scan completed."));
        Echo("");
        PrintStatus();
    }
}

void ScanRefGroup(RefGroup group)
{
    group.Blocks.Clear();
    group.OnTimers.Clear();
    group.OffTimers.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyRefinery>(
        group.Blocks,
        b =>
            b != null &&
            (!ONLY_SAME_GRID || b.CubeGrid == Me.CubeGrid) &&
            HasTag(b, group.BlockTag)
    );

    for (int i = 0; i < allTimers.Count; i++)
    {
        IMyTimerBlock timer = allTimers[i];

        if (HasTag(timer, group.OnTimerTag))
        {
            group.OnTimers.Add(timer);
        }

        if (HasTag(timer, group.OffTimerTag))
        {
            group.OffTimers.Add(timer);
        }
    }

    group.LastState = GetRefState(group);
}

void ScanAsmGroup(AsmGroup group)
{
    group.Blocks.Clear();
    group.ReadyTimers.Clear();
    group.WorkTimers.Clear();
    group.WaitTimers.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(
        group.Blocks,
        b =>
            b != null &&
            (!ONLY_SAME_GRID || b.CubeGrid == Me.CubeGrid) &&
            HasTag(b, group.BlockTag)
    );

    for (int i = 0; i < allTimers.Count; i++)
    {
        IMyTimerBlock timer = allTimers[i];

        if (HasTag(timer, group.ReadyTimerTag))
        {
            group.ReadyTimers.Add(timer);
        }

        if (HasTag(timer, group.WorkTimerTag))
        {
            group.WorkTimers.Add(timer);
        }

        if (HasTag(timer, group.WaitTimerTag))
        {
            group.WaitTimers.Add(timer);
        }
    }

    group.LastState = GetAsmState(group);
}

// ============================================================
// UPDATE LOGIK
// ============================================================

void UpdateRefGroups()
{
    for (int i = 0; i < refGroups.Count; i++)
    {
        RefGroup group = refGroups[i];
        RefState currentState = GetRefState(group);

        if (currentState == group.LastState)
        {
            continue;
        }

        if (currentState == RefState.Working)
        {
            TriggerTimers(group.OnTimers, Txt("Raffinerie Gruppe arbeitet: ", "Refinery group working: ") + group.Name);
        }
        else
        {
            TriggerTimers(group.OffTimers, Txt("Raffinerie Gruppe aus: ", "Refinery group off: ") + group.Name);
        }

        group.LastState = currentState;
    }
}

void UpdateAsmGroups()
{
    for (int i = 0; i < asmGroups.Count; i++)
    {
        AsmGroup group = asmGroups[i];
        AsmState currentState = GetAsmState(group);

        if (currentState == group.LastState)
        {
            continue;
        }

        if (currentState == AsmState.Working)
        {
            TriggerTimers(group.WorkTimers, Txt("Montage Gruppe arbeitet: ", "Assembler group working: ") + group.Name);
        }
        else if (currentState == AsmState.Waiting)
        {
            TriggerTimers(group.WaitTimers, Txt("Montage Gruppe wartet: ", "Assembler group waiting: ") + group.Name);
        }
        else
        {
            TriggerTimers(group.ReadyTimers, Txt("Montage Gruppe bereit: ", "Assembler group ready: ") + group.Name);
        }

        group.LastState = currentState;
    }
}

// ============================================================
// STATUS ERMITTELN
// ============================================================

RefState GetRefState(RefGroup group)
{
    for (int i = group.Blocks.Count - 1; i >= 0; i--)
    {
        IMyRefinery block = group.Blocks[i];

        if (block == null || block.Closed)
        {
            group.Blocks.RemoveAt(i);
            continue;
        }

        if (block.IsProducing)
        {
            return RefState.Working;
        }
    }

    return RefState.Off;
}

AsmState GetAsmState(AsmGroup group)
{
    bool hasQueue = false;

    for (int i = group.Blocks.Count - 1; i >= 0; i--)
    {
        IMyAssembler block = group.Blocks[i];

        if (block == null || block.Closed)
        {
            group.Blocks.RemoveAt(i);
            continue;
        }

        if (block.IsProducing)
        {
            return AsmState.Working;
        }

        queueBuffer.Clear();
        block.GetQueue(queueBuffer);

        if (queueBuffer.Count > 0)
        {
            hasQueue = true;
        }
    }

    if (hasQueue)
    {
        return AsmState.Waiting;
    }

    return AsmState.Ready;
}

// ============================================================
// MANUELLE BEFEHLE
// ============================================================
//
// trigger ref 0 on
// trigger ref 0 off
//
// trigger asm 0 ready
// trigger asm 0 work
// trigger asm 0 wait
//
// Die Zahl ist die Gruppen-Nummer aus der Statusanzeige.

void ManualTrigger(string argument)
{
    string[] parts = argument.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length < 4)
    {
        Echo(Txt("Befehl falsch.", "Wrong command."));
        Echo("trigger ref 0 on");
        Echo("trigger asm 0 work");
        return;
    }

    string type = parts[1].ToLower();
    int index = 0;

    if (!int.TryParse(parts[2], out index))
    {
        Echo(Txt("Gruppen-Nummer ungültig.", "Invalid group number."));
        return;
    }

    string action = parts[3].ToLower();

    if (type == "ref")
    {
        if (index < 0 || index >= refGroups.Count)
        {
            Echo(Txt("Raffinerie-Gruppe nicht gefunden.", "Refinery group not found."));
            return;
        }

        RefGroup group = refGroups[index];

        if (action == "on")
        {
            TriggerTimers(group.OnTimers, "MANUAL REF ON: " + group.Name);
            return;
        }

        if (action == "off")
        {
            TriggerTimers(group.OffTimers, "MANUAL REF OFF: " + group.Name);
            return;
        }
    }

    if (type == "asm")
    {
        if (index < 0 || index >= asmGroups.Count)
        {
            Echo(Txt("Montage-Gruppe nicht gefunden.", "Assembler group not found."));
            return;
        }

        AsmGroup group = asmGroups[index];

        if (action == "ready")
        {
            TriggerTimers(group.ReadyTimers, "MANUAL ASM READY: " + group.Name);
            return;
        }

        if (action == "work")
        {
            TriggerTimers(group.WorkTimers, "MANUAL ASM WORK: " + group.Name);
            return;
        }

        if (action == "wait")
        {
            TriggerTimers(group.WaitTimers, "MANUAL ASM WAIT: " + group.Name);
            return;
        }
    }

    Echo(Txt("Unbekannter Trigger-Befehl.", "Unknown trigger command."));
}

// ============================================================
// TIMER
// ============================================================

void TriggerTimers(List<IMyTimerBlock> timers, string message)
{
    int triggered = 0;

    for (int i = timers.Count - 1; i >= 0; i--)
    {
        IMyTimerBlock timer = timers[i];

        if (timer == null || timer.Closed)
        {
            timers.RemoveAt(i);
            continue;
        }

        timer.Trigger();
        triggered++;
    }

    Echo(message);
    Echo(Txt("Timer ausgelöst: ", "Timers triggered: ") + triggered);
}

// ============================================================
// AUSGABE
// ============================================================

void PrintStatus()
{
    Echo("=== Drakikosa Production FX v4 ===");
    Echo(Txt("Sprache: ", "Language: ") + LANGUAGE);
    Echo(Txt("Grid: ", "Grid: ") + (ONLY_SAME_GRID ? Txt("Nur gleiches Grid", "Same grid only") : Txt("Alle verbundenen Grids", "All connected grids")));
    Echo("");

    Echo("=== " + Txt("Raffinerie-Gruppen", "Refinery groups") + " ===");

    for (int i = 0; i < refGroups.Count; i++)
    {
        RefGroup group = refGroups[i];
        RefState state = GetRefState(group);

        Echo("#" + i + " " + group.Name + " | " + RefStateText(state));
        Echo("  BlockTag: " + group.BlockTag);
        Echo("  " + group.OnTimerTag + ": " + group.OnTimers.Count);
        Echo("  " + group.OffTimerTag + ": " + group.OffTimers.Count);
        Echo("  " + Txt("Blöcke: ", "Blocks: ") + group.Blocks.Count);
    }

    Echo("");
    Echo("=== " + Txt("Montage-Gruppen", "Assembler groups") + " ===");

    for (int i = 0; i < asmGroups.Count; i++)
    {
        AsmGroup group = asmGroups[i];
        AsmState state = GetAsmState(group);

        Echo("#" + i + " " + group.Name + " | " + AsmStateText(state));
        Echo("  BlockTag: " + group.BlockTag);
        Echo("  " + group.ReadyTimerTag + ": " + group.ReadyTimers.Count);
        Echo("  " + group.WorkTimerTag + ": " + group.WorkTimers.Count);
        Echo("  " + group.WaitTimerTag + ": " + group.WaitTimers.Count);
        Echo("  " + Txt("Blöcke: ", "Blocks: ") + group.Blocks.Count);
    }

    Echo("");
    Echo(Txt("Befehle:", "Commands:"));
    Echo("rescan | status");
    Echo("trigger ref 0 on");
    Echo("trigger ref 0 off");
    Echo("trigger asm 0 ready");
    Echo("trigger asm 0 work");
    Echo("trigger asm 0 wait");
}

string RefStateText(RefState state)
{
    if (state == RefState.Working)
    {
        return Txt("ARBEITET", "WORKING");
    }

    return Txt("AUS", "OFF");
}

string AsmStateText(AsmState state)
{
    if (state == AsmState.Working)
    {
        return Txt("ARBEITET", "WORKING");
    }

    if (state == AsmState.Waiting)
    {
        return Txt("WARTET", "WAITING");
    }

    return Txt("BEREIT", "READY");
}

// ============================================================
// HELFER
// ============================================================

bool HasTag(IMyTerminalBlock block, string tag)
{
    if (block == null) return false;

    return ContainsIgnoreCase(block.CustomName, tag) ||
           ContainsIgnoreCase(block.CustomData, tag);
}

bool ContainsIgnoreCase(string text, string value)
{
    if (string.IsNullOrWhiteSpace(text)) return false;
    if (string.IsNullOrWhiteSpace(value)) return false;

    return text.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
}

string Txt(string de, string en)
{
    if ((LANGUAGE ?? "").Trim().ToUpper() == "EN")
    {
        return en;
    }

    return de;
}