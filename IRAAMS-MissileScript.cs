/* 
I.R.A.A.M.S. - Iron Reign Advanced Automated Missile System.
By Chrithpy.
 
This is a missile script designed to work primarily with the Missile Guidance Block mod by Alysius, which compliments their Large
Torpedo Block mod V2.
Instructions on the Steam Page.
Enjoy ;) */

/* UPDATE 1.2
Added dampener controls so that missiles on subgrids dont fight against the movement of the main grid. You will need to add a 
Remote Control block named "Missile Remote Block" to the subgrid for this to work properly.
Added safety command to prevent accidental launch of missiles. */
 
//====== ADJUSTABLE SETTINGS =============================================================================
// After changing settings, make sure to Recompile the script before using.
 
// FIRE_LIMIT settings limit how many missiles launch in a single instant. 0= no limit, 1= one at a time, 2= two at a time, etc...
// (These settings ensure that all missiles dont launch at once and collide with each other)
int BLOCK_FIRE_LIMIT=2;
int ALL_FIRE_LIMIT=2;
int GROUP_FIRE_LIMIT=1;
 
//SALVO_LIMIT settings limit how many missiles fire from a launch press. 0= no limit, 1= one per press, etc...
// (These settings are good to maintain control over how many missiles are being launched)
int BLOCK_SALVO_LIMIT=0;
int ALL_SALVO_LIMIT=0;
int GROUP_SALVO_LIMIT=0;
 
//DELAY settings adjust the timing of the critical functions, change these to make a launch cycle quicker or slower. 
double DOOR_OPEN_DELAY=2.0;
double MISSILE_FIRE_DELAY=0.5;
double DOOR_CLOSE_DELAY=1.0;
double DOOR_SAFETY_DELAY=0.5;
 
//LAUNCH_ORDER determines the order that missiles fire in, 0=normal, 1=reverse, 2=random.
int LAUNCH_ORDER=0;
 
//RELOAD settings affect the "Missile Welders", how long they turn on for and how long they activate after the launch cycle is complete
int AUTO_RELOAD=5; //(0 = Disabled)
double RELOAD_DELAY=2.0;
 
//====== DO NOT CHANGE ANYTHING BELOW THIS LINE ============================================================
 
Dictionary<string, List<IMyTerminalBlock>> Targets = new Dictionary<string, List<IMyTerminalBlock>>();
Dictionary<string, int[]> State = new Dictionary<string, int[]>();
List<IMyTerminalBlock> A = new List<IMyTerminalBlock>();
int AI=0, AT=0, AS=0, AF=0;
int reloadDelayRemaining = 0;
int reloadTimeRemaining = 0;
bool isSafetyEnabled = false; // Master safety flag (False = Armed / Default function)
 
public Program() { Runtime.UpdateFrequency = UpdateFrequency.Update10; }
 
public void Main(string arg, UpdateType t)
{
    arg = arg.ToLower();
    
    // Safety toggle argument
    if (arg == "safety")
    {
        isSafetyEnabled = !isSafetyEnabled;
        if (isSafetyEnabled) Echo("IRAAMS SYSTEM STATE: SAFE");
        else Echo("IRAAMS SYSTEM STATE: ARMED");
        return;
    }

    // If safety is engaged, print a warning to the PB terminal and halt launch processing
    if (isSafetyEnabled)
    {
        Echo("CRITICAL: Launch command ignored. System Safety is ACTIVE.");
        SetVlsDampeners(false);
        return;
    }
    
    foreach(var key in new List<string>(State.Keys)) RunSequence(key);
    RunAll();
    ManageReloading();
 
    var M = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(M, x => x.CustomName.Contains("Missile Computer"));
 
    // Automatically turn dampeners OFF if no missile sequences are actively processing
    if (State.Count == 0 && AS == 0)
    {
        SetVlsDampeners(false);
    }
 
    if (arg == "abort") { Abort(); return; }
    if (arg == "lock") { foreach (var x in M) x.ApplyAction("Adn.ActionLockOnTarget"); return; }
    if (arg == "launchall") { StartAll(M); return; }
 
    if (arg.StartsWith("launch"))
    {
        string id = arg.Substring(6).ToUpper();
        if (id.Length >= 1 && id.Length <= 3) StartSequence(id, M);
    }
}
 
void StartSequence(string id, List<IMyTerminalBlock> M)
{
    Targets[id] = new List<IMyTerminalBlock>();
    if (id.Length == 1) foreach (var x in M) { if (GetGroup(x).StartsWith(id)) Targets[id].Add(x); }
    else if (id.Length == 2) foreach (var x in M) { if (GetGroup(x) == id) Targets[id].Add(x); }
    else if (id.Length == 3) foreach (var x in M) { if (x.CustomName.EndsWith(id)) Targets[id].Add(x); }
 
    Prepare(Targets[id]);
    State[id] = new int[] { 0, 0, 0, 0 };
 
    string pGroup = id.Length == 3 ? id.Substring(0, 2) : id;
    if (id.Length == 1) { SetBlockDoors(id, true); State[id][2] = 1; }
    else if (DoorsOpen(pGroup)) State[id][2] = 2;
    else { if (id.Length == 2) SetDoors(id, true); else SetDoors(pGroup, true); State[id][2] = 1; }
}
 
void RunSequence(string id)
{
    if (!State.ContainsKey(id) || State[id][2] == 0) return;
    State[id][1]++;
 
    int fireLimit = id.Length == 1 ? BLOCK_FIRE_LIMIT : (id.Length == 2 ? GROUP_FIRE_LIMIT : 1);
    int salvoLimit = id.Length == 1 ? BLOCK_SALVO_LIMIT : (id.Length == 2 ? GROUP_SALVO_LIMIT : 0);
 
    if (State[id][2] == 1)
    {
        if (State[id][1] < (DOOR_OPEN_DELAY + DOOR_SAFETY_DELAY) * 10) return;
        State[id][1] = 0; State[id][2] = 2;
    }
    if (State[id][2] == 2)
    {
        if (State[id][1] < MISSILE_FIRE_DELAY * 10) return;
        State[id][1] = 0;
 
        int f = 0;
        while ((fireLimit == 0 || f < fireLimit) && State[id][0] < Targets[id].Count)
        {
            if (salvoLimit > 0 && State[id][3] >= salvoLimit) break;
            
            // Turn dampeners ON on the remote block to prepare for separation
            SetVlsDampeners(true);
            
            Targets[id][State[id][0]].ApplyAction("Adn.ActionLaunchMissile");
            State[id][0]++; State[id][3]++; f++;
        }
        if (State[id][0] >= Targets[id].Count) State[id][2] = 3;
    }
    if (State[id][2] == 3)
    {
        if (State[id][1] < DOOR_CLOSE_DELAY * 10) return;
        if (id.Length == 1) SetBlockDoors(id, false);
        else SetDoors(id.Length == 3 ? id.Substring(0, 2) : id, false);
        
        TriggerReload();
        State.Remove(id); Targets.Remove(id);
    }
}
 
void StartAll(List<IMyTerminalBlock> M)
{
    A.Clear(); foreach (var x in M) A.Add(x);
    Prepare(A);
    for (int i = 0; i < 10; i++) SetBlockDoors(i.ToString(), true);
    AI = AT = AF = 0;
 
    bool o = true;
    for (int i = 0; i < 10; i++)
        for (char c = 'A'; c <= 'Z'; c++) if (!DoorsOpen(i + c.ToString())) o = false;
    AS = o ? 2 : 1;
}
 
void RunAll()
{
    if (AS == 0) return;
    AT++;
    if (AS == 1 && AT >= (DOOR_OPEN_DELAY + DOOR_SAFETY_DELAY) * 10) { AT = 0; AS = 2; }
    if (AS == 2 && AT >= MISSILE_FIRE_DELAY * 10)
    {
        AT = 0; int f = 0;
        while (f < ALL_FIRE_LIMIT && AI < A.Count)
        {
            if (ALL_SALVO_LIMIT > 0 && AF >= ALL_SALVO_LIMIT) break;
            
            // Turn dampeners ON on the remote block to prepare for separation
            SetVlsDampeners(true);
            
            A[AI].ApplyAction("Adn.ActionLaunchMissile");
            AI++; AF++; f++;
        }
        if (AI >= A.Count) AS = 3;
    }
    if (AS == 3 && AT >= DOOR_CLOSE_DELAY * 10)
    {
        for (int i = 0; i < 10; i++) SetBlockDoors(i.ToString(), false);
        TriggerReload();
        AS = 0;
    }
}
 
void Abort()
{
    foreach (var id in new List<string>(State.Keys))
    {
        if (id.Length == 1) SetBlockDoors(id, false);
        else SetDoors(id.Length == 3 ? id.Substring(0, 2) : id, false);
    }
    State.Clear(); Targets.Clear(); A.Clear();
    AI = AT = AS = AF = 0;
    for (int i = 0; i < 10; i++) SetBlockDoors(i.ToString(), false);
    
    SetWelders(false);
    reloadDelayRemaining = 0;
    reloadTimeRemaining = 0;
    
    SetVlsDampeners(false);
}
 
bool DoorsOpen(string id)
{
    var a = GridTerminalSystem.GetBlockWithName("VLS Door " + id + "-1") as IMyDoor;
    var b = GridTerminalSystem.GetBlockWithName("VLS Door " + id + "-2") as IMyDoor;
    return a != null && b != null && a.OpenRatio > 0.9f && b.OpenRatio > 0.9f;
}
 
void SetBlockDoors(string id, bool o) { foreach (char c in "ABCDEFGHIJKLMNOPQRSTUVWXYZ") SetDoors(id + c.ToString(), o); }
 
void SetDoors(string id, bool o)
{
    var a = GridTerminalSystem.GetBlockWithName("VLS Door " + id + "-1") as IMyDoor;
    var b = GridTerminalSystem.GetBlockWithName("VLS Door " + id + "-2") as IMyDoor;
    if (a != null) { if (o) a.OpenDoor(); else a.CloseDoor(); }
    if (b != null) { if (o) b.OpenDoor(); else b.CloseDoor(); }
}
 
string GetGroup(IMyTerminalBlock x)
{
    string n = x.CustomName;
    for (int i = 0; i < n.Length - 1; i++)
        if (char.IsDigit(n[i]) && char.IsLetter(n[i + 1])) return "" + n[i] + char.ToUpper(n[i + 1]);
    return "";
}
 
void Prepare(List<IMyTerminalBlock> L)
{
    if (LAUNCH_ORDER == 1) L.Reverse();
    if (LAUNCH_ORDER == 2) { Random r = new Random(); for (int i = L.Count - 1; i > 0; i--) { int j = r.Next(i + 1); var t = L[i]; L[i] = L[j]; L[j] = t; } }
}
 
void TriggerReload()
{
    if (AUTO_RELOAD > 0)
    {
        reloadDelayRemaining = (int)(RELOAD_DELAY * 10);
        reloadTimeRemaining = AUTO_RELOAD * 10;
    }
}
 
void ManageReloading()
{
    if (reloadDelayRemaining > 0)
    {
        reloadDelayRemaining--;
        if (reloadDelayRemaining <= 0)
        {
            SetWelders(true);
        }
        return;
    }
 
    if (reloadTimeRemaining > 0)
    {
        reloadTimeRemaining--;
        if (reloadTimeRemaining <= 0)
        {
            SetWelders(false);
        }
    }
}
 
void SetWelders(bool active)
{
    var group = GridTerminalSystem.GetBlockGroupWithName("Missile Welders");
    if (group == null) return;
    
    var welders = new List<IMyShipWelder>();
    group.GetBlocksOfType(welders);
    foreach (var w in welders)
    {
        if (w != null) w.Enabled = active;
    }
}
 
void SetVlsDampeners(bool active)
{
    var ctrl = GridTerminalSystem.GetBlockWithName("Missile Remote Block") as IMyShipController;
    if (ctrl != null)
    {
        ctrl.DampenersOverride = active;
    }
}