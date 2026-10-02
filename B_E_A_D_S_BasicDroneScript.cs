// ==============================================
//    MSC B.E.A.DS. (Basic Escort Ai Drone Setup) — DRONE BRAIN
// ==============================================
//    Made by Claude
//    Commissioned by mhwyoshi
// ==============================================
//
// SETUP INSTRUCTIONS (READ BEFORE RUNNING):
//
// Because Space Engineers has no way to tell apart
// multiple blocks of the same type (e.g. 5 Timer Blocks,
// 3 Event Controllers) by type alone, this script relies
// on Space Engineers' own DEFAULT sequential naming to
// tell them apart during first-time setup.
//
// BEFORE running Setup:
//   1. Place all required blocks (Timer Blocks, AI blocks,
//      Event Controllers, Action Relays, Camera, Broadcaster)
//      on the grid WITHOUT manually renaming them — leave
//      them on their default names (e.g. Timer Block,
//     , "Timer Block "...).
//   2. Make sure the ORDER you place same-type blocks in
//      matches the order listed in the BLOCK NAME MAP below.
//   3. Manually create these 4 Groups in the terminal, and drag
//      the matching blocks into each one yourself:
//        - Batteries          -> every Battery block on the grid
//        - Hydrogen Fuel      -> every Hydrogen Tank on the grid
//        - Hydrogen Thrusters -> every Hydrogen Thruster on the grid
//        - Ion Thrusters      -> every Ion Thruster on the grid
//      IMPORTANT: Space Engineers' scripting API does not allow a
//      script to add blocks to a group - only to READ what's
//      already in one. So Setup can VERIFY these groups exist and
//      report how many blocks are in each, but it can never
//      populate them for you. This has to be done by hand, once,
//      per ship (or once per design if you copy fighters via
//      Blueprint/Projector, since groups copy along with the
//      blueprint).
//   4. Wire those same 4 Groups into the 3 Event Controllers' own
//      condition/action slots (also a manual, one-time, in-terminal
//      step - the script can't do this part either):
//        Event Controller: Recharge Batteries
//          Condition slot -> Batteries group, "Stored power" <= 40%,
//                             AND gate (ALL batteries must be low)
//          Action slot    -> Timer Block: AI (Docking), Trigger Now
//        Event Controller: Refuel Hydrogen
//          Condition slot -> Hydrogen Fuel group, "Filled ratio" <= 50%,
//                             AND gate (ALL tanks must be low)
//          Action slot    -> Timer Block: AI (Docking), Trigger Now
//        Event Controller: Recharg & Refuel
//          Condition slot -> all Connectors, "Connected" state
//          Action slots (when TRUE / connected):
//            - Hydrogen Thrusters group -> Toggle Off
//            - Ion Thrusters group      -> Toggle Off
//            - Hydrogen Fuel group      -> Stockpile On
//            - Batteries group          -> Recharge On
//          Action slots (when FALSE / disconnected):
//            - Hydrogen Thrusters group -> Toggle On
//            - Ion Thrusters group      -> Toggle On
//            - Hydrogen Fuel group      -> Stockpile Off
//            - Batteries group          -> Automatic On
//
// TO RUN SETUP:
//   Run this Programmable Block with the argument:
//      Setup:PlayerName
//   Example:
//      Setup:Strife
//
//   This will:
//    - Rename all default-named blocks to their designated
//      script names, in placement order
//    - Assign the follow-target player to Custom Data
//      (Follow: PlayerName)
//    - Validate the player is currently online
//    - Confirm all required blocks were found
//    - Print a full confirmation report to Echo (visible in
//      this block's Detail Info panel / terminal)
//
// IF SETUP REPORTS MISSING BLOCKS:
//   Check "Error: Lacking [Block Name](s)" in Detail Info,
//   place/verify the missing block(s), and re-run Setup.
//
// IF SETUP REPORTS AN OFFLINE/INVALID PLAYER:
//   Check "Error: Player is offline" - confirm spelling and
//   that the player is currently online, then re-run Setup.
//
// Setup is SAFE TO RE-RUN. Blocks already renamed to their
// correct designated names will be skipped/left alone.
//
// OTHER ARGUMENTS (once Setup has run):
//   home_synchronization
//     Run this while the drone is DOCKED (Connector connected) to
//     manually re-sync its Home target. While connected, this grid's
//     GridTerminalSystem automatically extends to the docked grid too -
//     so the script searches it for a grid or Beacon matching the
//     "Home:" name in Custom Data, and remembers its position for use
//     after undocking. Normally this also runs automatically the moment
//     the Connector locks, but this argument lets you force it manually
//     (e.g. right after editing the Home: name while already docked).
//
//   reset_home
//     Clears the stored Home target position and blanks the "Home:"
//     line in Custom Data, so the ship can be set up fresh with a new
//     Home target. Type the new name into Home:, dock, then run
//     home_synchronization (or just redock, which triggers it
//     automatically).
//
//   list_ai_flight_properties
//     Diagnostic. Dumps every terminal property ID + type on the AI
//     Flight block to Detail Info (Echo), using the official technique
//     from Keen's own scripting wiki. Run this once in-game to find the
//     real property ID behind "Align to P.Gravity", then report it back
//     so the planetary docking check can read it directly by name
//     instead of searching for it every time.
//
//   Mothership true / Mothership false
//     Toggles this grid into/out of Mothership mode. A Mothership does
//     NOT run fighter AI setup at all - it purely broadcasts its own
//     docking-tagged connector positions/orientations over the Fleet
//     Position Coordinator channel so fighters can find and claim them.
//     No Setup needed for this role. Fighters that dock here will
//     ESCORT (follow) once launched.
//
//   Base Mode true / Base Mode false
//     Same broadcast-only role as Mothership, for a stationary base
//     instead of a moving carrier. Fighters that dock/launch from a
//     Base do NOT escort anything - once their launch playback
//     finishes, they instead enable AI Flight and AI Offensive directly
//     and go straight into attacking whatever nearby targets their AI
//     Offensive block is already configured (in its own terminal
//     panel) to attack. No Setup needed for this role either.
//
//   Mothership mode and Base mode status are both always shown in this
//   Programmable Block's Detail Info panel (Echo), so you can see at a
//   glance which role (if any) this grid is currently in.
//
//   CONNECTOR CUSTOM DATA - "docking" TAG (required for Mothership/Base):
//     A connector is only offered to fighters for docking if its OWN
//     Custom Data contains the word "docking" (case-insensitive)
//     anywhere in it - e.g. just type the word docking into that
//     connector's Custom Data field in its terminal panel. This lets a
//     Mothership/Base have other connectors (cargo, fuel transfer,
//     etc.) that fighters will never try to claim or fly into. Any
//     connector without "docking" in its Custom Data is simply ignored
//     by the broadcast.
//
//   RequestDock
//     Add this as a "Run Programmable Block" action (with this argument)
//     on the Timer Block: AI (Docking) toolbar. Begins the autonomous
//     sequence: claim the nearest free docking-tagged connector (see
//     above), yield to a closer rival fighter if one also claims it,
//     fly the two-leg approach via the Remote Control block, then
//     Connect().
//
//   RequestUndock
//     Add this as a "Run Programmable Block" action on the Timer Block:
//     AI Launch (Fighter Launch) toolbar. Disconnects and releases the
//     claimed connector so another fighter can use it.
//
//   REQUIRED ADDITIONAL BLOCK for autodock: a Remote Control block,
//   named "Remote Control: AI Docking Pilot" (or left default-named
//   before Setup, same rename convention as everything else.
//
//   CAMERA INTERIOR SCANNING: while flying normally (not in Mothership
//   or Base mode), a fighter continuously raycasts with Camera:
//   Targeting Sight (whatever it's currently pointed at, e.g. down a
//   hangar corridor) and broadcasts anything it hits to other fighters
//   on the same channel, so they're aware of what's nearby too. Note:
//   this is a single straight-line raycast in whatever direction the
//   camera currently faces, not a 360-degree scan - and right now the
//   script only shares this data between fighters, it does not yet
//   actively steer around what it detects. That's a good next piece to
//   test and build out together in-game.
//
//   FIGHTER'S OWN DOCKING CONNECTOR: same tagging convention as the
//   Mothership/Base side - type the word "docking" into the Custom
//   Data of whichever connector on THIS fighter should be used to
//   dock. If no connector at all exists on the fighter, the Detail
//   Info panel shows "Error: no connector of that type exists". If a
//   connector exists but none are tagged "docking", it shows "Error:
//   no connectors set to docking, you need at least one connector to
//   dock!"
//
//   PLANETARY DOCKING SAFETY CHECK: if the fighter's AI Flight block
//   has "Align to P.Gravity" enabled and the ship is in meaningful
//   planetary gravity, the autodock claim step skips any known
//   connector whose facing direction is too far from horizontal to
//   safely reach without exceeding what a horizon-locked ship can
//   pitch/roll to. If NO known connector is currently safe to reach,
//   Detail Info shows "Error: No suitable docking areas available for
//   docking!". This is a best-effort geometric heuristic (including
//   how the script detects Align to P.Gravity itself, since that
//   setting isn't available as a normal typed property) - worth
//   verifying/tuning together in-game.
//
//   HANGAR MODE (Custom Data): "Hangar Mode: true" or "Hangar Mode:
//   false" - auto-seeded to "false". Set to "true" if this drone needs
//   to know a physical hangar doorway location to dock (the original
//   single-chokepoint, furthest-fighter-goes-first queue system). Set
//   to "false" (the default) if this drone only uses the newer
//   connector-claim autodock system (Mothership/Base broadcasts) -
//   which doesn't need a hangar door concept at all, since it flies
//   directly to whichever specific connector it claimed. The Home:
//   sync (grid/Beacon lookup while docked) works the same either way,
//   regardless of this setting.
//
//   BASIC MODE (Custom Data): "Basic Mode: true" or "Basic Mode:
//   false" - auto-seeded to "false". Set to "true" to make this script
//   do NOTHING except watch Ammo Types - the fighter behaves exactly
//   as if it had no script running at all, relying entirely on its own
//   manually-configured Timer Block toolbars and AI block settings.
//   The only thing that still runs is the Ammo Types check, so weapon
//   presence/conveyor problems still get flagged while docked. Useful
//   for a fighter you want fully hand-controlled/manually-triggered,
//   without turning off ammo diagnostics.
//
//   AMMO TYPES (Custom Data): a block near the bottom of Custom Data,
//   auto-added the first time this script runs, listing every
//   supported ammo type set to "false" by default:
//     Rocket, Gatling Ammo Box, Assault Cannon Shell, Autocannon
//     Magazine, Artillery Shell, Large Railgun Sabot, Small Railgun
//     Sabot
//   Set any of these to "true" to have the script watch that ammo
//   type. Each label covers every weapon variant that uses that ammo
//   (e.g. Gatling Ammo Box covers Gatling Gun, Warfare Gatling Gun,
//   Gatling Turret, and Gatling Turret Type II; Rocket covers
//   Reloadable Rocket Launcher, Rocket Launcher (large grid), Rocket
//   Turret, and Rocket Turret Type II; Large/Small Railgun Sabot are
//   distinguished by the block's actual grid size). For each type
//   marked true: if no matching weapon block exists on the grid,
//   Detail Info shows "Error: weapon Missing (Type)". If matching
//   weapon(s) exist but none have a working conveyor connection to a
//   Cargo Container on the grid, it shows "Error: Ammo Type lacking
//   conveyoring (Type)". This is a diagnostic check only - actual ammo
//   restocking through the conveyor system is handled automatically by
//   the game itself, not by this script.
//
//   (No other arguments are required for normal operation - the Timer
//   Block toolbars drive the state machine. This script mainly watches
//   for Launch-complete -> Escort-On/Combat transition and handles
//   fleet dock/launch queue coordination.)
//
// ==============================================

// ============================================================
//  CONFIGURATION
// ============================================================

const string FLEET_CHANNEL = "Fleet Position Coordinator";
const double BATTERY_THRESHOLD = 0.40;      // not used directly by script (Event Controller handles it) - kept for reference
const double HYDROGEN_THRESHOLD = 0.50;     // same as above
const double FLEET_TIMEOUT_SECONDS = 10.0;  // how long to wait for a queued fighter before ignoring it
const double POSITION_PING_INTERVAL = 2.0;  // seconds between fleet position broadcasts

const string NAME_REMOTE_CONTROL   = "Remote Control: AI Docking Pilot";
const double APPROACH_OFFSET_METERS = 20.0;  // stand-off point before final connector approach
const double CONNECT_DISTANCE_METERS = 1.5;  // distance at which we attempt Connect()
const double CLAIM_SETTLE_SECONDS = 3.0;     // time to wait for closer claims before committing
const double CONNECTOR_BROADCAST_INTERVAL = 2.0; // seconds between Mothership connector broadcasts

// Designated block names (final names after Setup renames them)
const string NAME_AI_ESCORT       = "Basic AI (Escort)";
const string NAME_AI_FLIGHT       = "AI, Flight (Move)";
const string NAME_AI_COMBAT       = "AI Offensive (Combat)";
const string NAME_AI_REC_LAUNCH   = "AI Recorder (Launch)";
const string NAME_AI_REC_DOCKING  = "AI Recorder (Docking)";
const string NAME_CAMERA          = "Camera: Targeting Sight";

const string NAME_TIMER_LAUNCH    = "Timer Block: AI Launch (Fighter Launch)";
const string NAME_TIMER_DOCKING   = "Timer Block: AI (Docking)";
const string NAME_TIMER_ESCORT_ON = "Timer Block: AI Escort On";
const string NAME_TIMER_AI_OFF    = "Timer Block: AI Off";
const string NAME_TIMER_FOLLOW_ME = "Timer Block: Follow Me";

const string NAME_RELAY_LAUNCH    = "Action Relay: Launch Fighter";
const string NAME_RELAY_ESCORT_ON = "Action Relay: Strife AI Escort On";
const string NAME_RELAY_AI_OFF    = "Action Relay: Strife AI Off";
const string NAME_RELAY_DOCKING   = "Action Relay: Toggle Strife AI Docking";

const string NAME_EC_BATTERIES    = "Event Controller: Recharge Batteries";
const string NAME_EC_HYDROGEN     = "Event Controller: Refuel Hydrogen";
const string NAME_EC_RECHARGREFUEL= "Event Controller: Recharg & Refuel";

const string NAME_BROADCASTER     = "Drone Status Broadcaster"; // gets renamed to "Drone (<AntennaHUDName>)" post-setup

const string GROUP_BATTERIES      = "Batteries";
const string GROUP_HYDROGEN_FUEL  = "Hydrogen Fuel";
const string GROUP_HYDROGEN_THRUST= "Hydrogen Thrusters";
const string GROUP_ION_THRUST     = "Ion Thrusters";

const string CUSTOM_DATA_HOME_KEY = "Home"; // "Home: (Grid Name)" or "Home: (Beacon Name)" - resolved while docked
const string CUSTOM_DATA_HANGAR_MODE_KEY = "Hangar Mode"; // "true"/"false" - does this drone need to know a hangar door location to dock?
const string CUSTOM_DATA_BASIC_MODE_KEY = "Basic Mode"; // "true"/"false" - if true, script does nothing except Ammo Types checks; fighter behaves as if unscripted

const string AMMO_TYPES_HEADER = "Ammo Types";
static readonly string[] AmmoTypeNames = new string[]
{
    "Rocket",
    "Gatling Ammo Box",
    "Assault Cannon Shell",
    "Autocannon Magazine",
    "Artillery Shell",
    "Large Railgun Sabot",
    "Small Railgun Sabot"
};
const double AMMO_CHECK_INTERVAL = 5.0; // seconds between weapon presence/conveyor checks

// Broadcaster messages, indexed 1-6 to match design notes
static readonly string[] BroadcastMessages = new string[]
{
    "",                                              // index 0 unused
    "AI Refueling Toggled",                          // 1
    "AI Escort Enabled",                             // 2
    "Threat Detected: Targeting Bandit",              // 3
    "Threat Not Detected. Assuming Neutralized",      // 4
    "AI Disabled",                                    // 5
    "AI Launching Fighter"                            // 6
};

// ============================================================
//  STATE
// ============================================================

enum DroneState
{
    Idle,
    Launching,
    Escorting,
    Combat,
    Docking,
    Off
}

DroneState currentState = DroneState.Off;

// Block references (populated by FindBlocks / Setup)
IMyTimerBlock timerLaunch, timerDocking, timerEscortOn, timerAIOff, timerFollowMe;
IMyBasicMissionBlock aiEscort;
IMyFlightMovementBlock aiFlight;
IMyOffensiveCombatBlock aiCombat;
IMyPathRecorderBlock aiRecLaunch, aiRecDocking;
IMyCameraBlock camera;
IMyRadioAntenna fleetAntenna;       // Antenna acting as Drone Status Broadcaster + used for HUD name + IGC
List<IMyShipConnector> connectors = new List<IMyShipConnector>();

// Fleet coordination
IMyBroadcastListener fleetListener;
Vector3D? hangarDoorReference = null;
Vector3D? homeTargetPosition = null;
string lastHomeSyncResult = "";
bool wasConnected = false;
double lastPingTime = 0;
double runtimeSeconds = 0;
string lastCommandStatus = ""; // sticky - stays in Detail Info every tick until a new command overwrites it

// Sets a message that will keep showing in Detail Info every single tick
// (not just the tick the command ran on), until another command replaces
// it. Use this for one-off command confirmations instead of a plain Echo,
// since Echo's buffer normally clears every tick.
void SetStatus(string message)
{
    lastCommandStatus = message;
    Echo(message);
}

// Cache of last known positions/state of other fighters in the fleet
Dictionary<string, FleetShipInfo> fleetShips = new Dictionary<string, FleetShipInfo>();

// Camera-scanned surroundings reported by other fighters (interior/approach
// obstacle awareness) - one most-recent report per reporting fighter.
Dictionary<string, ScanReport> knownSurroundings = new Dictionary<string, ScanReport>();
double lastScanBroadcastTime = 0;
const double SCAN_BROADCAST_INTERVAL = 1.0; // seconds between camera scan broadcasts
const double SCAN_RANGE_METERS = 150.0;     // camera raycast range for interior scanning

class ScanReport
{
    public string SourceId;
    public string ObjectName;
    public Vector3D Position;
    public double LastSeen;
}

class FleetShipInfo
{
    public string Id;
    public double DistanceFromDoor;
    public string Status; // "waiting_dock", "docked", "waiting_launch", "launched"
    public double LastSeen;
}

// ---- Mothership / Autodock additions ----

bool isMothership = false; // persisted via Storage, toggled by "Mothership true"/"Mothership false"
bool isBaseMode = false;   // persisted via Storage, toggled by "Base Mode true"/"Base Mode false"
IMyRemoteControl remoteControl; // fighters only - used for scripted autodock waypoints
string dockedAtRole = null; // "MOTHERSHIP" or "BASE" - remembered from whichever connector we last docked to

// Fighter-side: known connectors broadcast by the Mothership
Dictionary<string, MothershipConnectorInfo> knownConnectors = new Dictionary<string, MothershipConnectorInfo>();
string claimedConnectorId = null;
double claimStartTime = -1;
enum DockPhase { None, Claiming, Settling, ApproachingOffset, ApproachingConnector, Connected }
DockPhase dockPhase = DockPhase.None;

// Mothership-side: own connectors being broadcast
double lastConnectorBroadcastTime = 0;

class MothershipConnectorInfo
{
    public string ConnectorId;
    public string ShipId;
    public string Role; // "MOTHERSHIP" or "BASE"
    public Vector3D Position;
    public Vector3D Direction; // outward-facing normal, for approach offset
    public bool Occupied;
    public double LastSeen;
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    // Register fleet coordination listener
    fleetListener = IGC.RegisterBroadcastListener(FLEET_CHANNEL);
    fleetListener.SetMessageCallback();

    // Try to load saved hangar door reference + state from storage
    LoadState();

    // Attempt to locate blocks (won't rename anything unless Setup is run)
    FindBlocks(false, null);

    // Make sure Custom Data has a Follow: line to type a name into, even
    // before Setup or the first Main() tick has run.
    EnsureFollowLineExists();
}

// Writes a blank "Follow:" line into Custom Data if one doesn't already
// exist, so there's always an obvious place to type a player name in -
// whether or not Setup has ever been run.
void EnsureFollowLineExists()
{
    SeedCustomDataKeyIfMissing("Follow", "");
    SeedCustomDataKeyIfMissing(CUSTOM_DATA_HOME_KEY, "");
    SeedCustomDataKeyIfMissing(CUSTOM_DATA_HANGAR_MODE_KEY, "false");
    SeedCustomDataKeyIfMissing(CUSTOM_DATA_BASIC_MODE_KEY, "false");
    EnsureAmmoTypesBlockExists();
}

// Adds "Key: value" as its own blank-line-separated paragraph at the
// bottom of Custom Data, but only if that key doesn't already exist -
// existing values (yours) are never touched or overwritten. Used for all
// the mode/target keys (Follow, Home, Hangar Mode, Basic Mode) so each one
// is visually separated instead of all bunched together.
void SeedCustomDataKeyIfMissing(string key, string defaultValue)
{
    if (ParseCustomDataValue(Me.CustomData, key) != null) return;
    Me.CustomData = (Me.CustomData ?? "") + "\n" + key + ": " + defaultValue + "\n";
}

// Seeds an "Ammo Types" section at the bottom of Custom Data, one line per
// known ammo type, defaulted to "false" (opt-in) - set any of these to
// "true" to have the script watch that ammo type for missing weapon
// blocks/conveyor issues. Only added once; existing entries are never
// overwritten, so your own true/false choices always persist. A blank
// line separates the "Ammo Types" label from whatever came before it
// (Follow/Home/Hangar Mode), and another separates the label itself from
// the actual true/false entries underneath it.
void EnsureAmmoTypesBlockExists()
{
    if (Me.CustomData != null && Me.CustomData.IndexOf(AMMO_TYPES_HEADER, StringComparison.OrdinalIgnoreCase) >= 0)
        return; // already present - don't touch it

    StringBuilder block = new StringBuilder();
    block.AppendLine();
    block.AppendLine(AMMO_TYPES_HEADER);
    block.AppendLine();
    foreach (var name in AmmoTypeNames)
        block.AppendLine(name + ": false");

    Me.CustomData = (Me.CustomData ?? "") + block.ToString();
}

public void Main(string argument, UpdateType updateSource)
{
    runtimeSeconds += Runtime.TimeSinceLastRun.TotalSeconds;

    // ---- Handle explicit commands ----
    if (!string.IsNullOrWhiteSpace(argument))
    {
        if (argument.StartsWith("Setup", StringComparison.OrdinalIgnoreCase))
        {
            HandleSetup(argument);
            return;
        }

        if (argument.Equals("home_synchronization", StringComparison.OrdinalIgnoreCase) ||
            argument.Equals("home_sycrinization", StringComparison.OrdinalIgnoreCase))
        {
            HandleHomeSyncCommand();
            return;
        }

        if (argument.Equals("reset_home", StringComparison.OrdinalIgnoreCase))
        {
            HandleResetHomeCommand();
            return;
        }

        if (argument.Equals("Mothership true", StringComparison.OrdinalIgnoreCase))
        {
            isMothership = true;
            SaveState();
            SetStatus("Mothership mode ENABLED - this grid now broadcasts connector layout to fighters, and skips fighter-AI setup.");
            return;
        }

        if (argument.Equals("Mothership false", StringComparison.OrdinalIgnoreCase))
        {
            isMothership = false;
            SaveState();
            SetStatus("Mothership mode DISABLED - back to normal fighter setup/behavior.");
            return;
        }

        if (argument.Equals("Base Mode true", StringComparison.OrdinalIgnoreCase))
        {
            isBaseMode = true;
            SaveState();
            SetStatus("Base Mode ENABLED - this grid now broadcasts docking-tagged connectors to fighters, and skips fighter-AI setup.");
            return;
        }

        if (argument.Equals("Base Mode false", StringComparison.OrdinalIgnoreCase))
        {
            isBaseMode = false;
            SaveState();
            SetStatus("Base Mode DISABLED - back to normal fighter setup/behavior.");
            return;
        }

        if (argument.Equals("RequestDock", StringComparison.OrdinalIgnoreCase))
        {
            RequestDock();
            return;
        }

        if (argument.Equals("RequestUndock", StringComparison.OrdinalIgnoreCase))
        {
            RequestUndock();
            return;
        }

        if (argument.Equals("list_ai_flight_properties", StringComparison.OrdinalIgnoreCase))
        {
            ListAiFlightProperties();
            return;
        }
    }

    // ---- Handle incoming IGC fleet messages ----
    while (fleetListener.HasPendingMessage)
    {
        MyIGCMessage msg = fleetListener.AcceptMessage();
        HandleFleetMessage(msg);
    }

    // ---- Regular per-tick logic ----
    Echo("Mothership mode: " + (isMothership ? "ON" : "OFF") + " | Base mode: " + (isBaseMode ? "ON" : "OFF"));
    if (!string.IsNullOrEmpty(lastCommandStatus))
        Echo(lastCommandStatus); // keeps the last command's result visible every tick, not just the tick it ran on

    if (isMothership)
    {
        BroadcastCarrierConnectors("MOTHERSHIP");
    }
    else if (isBaseMode)
    {
        BroadcastCarrierConnectors("BASE");
    }
    else if (IsBasicModeEnabled())
    {
        // Basic Mode: behave as if this script weren't running at all -
        // the fighter relies entirely on its own manually-configured
        // Timer Block toolbars and AI block settings, same as before any
        // of this script existed. The one exception: still watch Ammo
        // Types so ammo restocking problems get flagged while docked.
        EnsureFollowLineExists();
        CheckAmmoTypes();
    }
    else
    {
        EnsureFollowLineExists();
        UpdateHangarDoorReference();
        CheckLaunchComplete();
        CheckFollowTarget();
        if (IsHangarModeEnabled())
        {
            BroadcastOwnPosition();
            PruneStaleFleetEntries();
        }
        PruneStaleConnectors();
        RunAutodockStateMachine();
        ScanInteriorAndBroadcast();
        CheckAmmoTypes();
    }

    SaveState();
}

// ============================================================
//  SETUP / BLOCK DISCOVERY / RENAME
// ============================================================

void HandleSetup(string argument)
{
    string playerName = null;
    int colonIndex = argument.IndexOf(':');
    if (colonIndex >= 0 && colonIndex < argument.Length - 1)
    {
        playerName = argument.Substring(colonIndex + 1).Trim();
    }

    StringBuilder report = new StringBuilder();
    report.AppendLine("Setup Complete:");

    List<string> missing = new List<string>();
    FindBlocks(true, report);

    // Validate required blocks
    CheckRequired(timerLaunch, "Timer Block: AI Launch (Fighter Launch)", missing);
    CheckRequired(timerDocking, "Timer Block: AI (Docking)", missing);
    CheckRequired(timerEscortOn, "Timer Block: AI Escort On", missing);
    CheckRequired(timerAIOff, "Timer Block: AI Off", missing);
    CheckRequired(timerFollowMe, "Timer Block: Follow Me", missing);
    CheckRequired(aiEscort, "Basic AI (Escort)", missing);
    CheckRequired(aiFlight, "AI, Flight (Move)", missing);
    CheckRequired(aiCombat, "AI Offensive (Combat)", missing);
    CheckRequired(aiRecLaunch, "AI Recorder (Launch)", missing);
    CheckRequired(aiRecDocking, "AI Recorder (Docking)", missing);
    CheckRequired(camera, "Camera: Targeting Sight", missing);
    CheckRequired(fleetAntenna, "Drone Status Broadcaster", missing);
    CheckRequired(remoteControl, "Remote Control: AI Docking Pilot", missing);

    if (connectors.Count == 0)
        missing.Add("Connector(s)");

    // Populate groups
    PopulateGroups(report);

    // Sync broadcaster name to antenna HUD name
    SyncBroadcasterName();

    // Handle player follow target
    if (!string.IsNullOrEmpty(playerName))
    {
        Me.CustomData = UpsertCustomDataLine(Me.CustomData, "Follow", playerName);
        report.AppendLine("Follow: " + playerName + " (saved - not automatically validated, see note in script header)");
    }
    else
    {
        // No player name given with Setup - seed a blank Follow: line in
        // Custom Data if one doesn't already exist, so it's obvious where
        // to type a player name in manually.
        if (ParseCustomDataValue(Me.CustomData, "Follow") == null)
        {
            Me.CustomData = UpsertCustomDataLine(Me.CustomData, "Follow", "");
            report.AppendLine("Follow: (blank) -> type a player name after \"Follow:\" in Custom Data");
        }
    }

    if (missing.Count > 0)
    {
        report.AppendLine();
        foreach (var m in missing)
            report.AppendLine("Error: Lacking " + m);
    }
    else
    {
        report.AppendLine();
        report.AppendLine("All Blocks Found");
    }

    SetStatus(report.ToString());
}

void CheckRequired(IMyTerminalBlock block, string name, List<string> missing)
{
    if (block == null) missing.Add(name);
}

// Finds blocks either by their final designated name (normal operation)
// or by default sequential naming + renames them (Setup mode).
void FindBlocks(bool renameMode, StringBuilder report)
{
    // ---- Timer Blocks ----
    List<IMyTimerBlock> timers = new List<IMyTimerBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(timers, b => b.CubeGrid == Me.CubeGrid);
    timerLaunch    = ResolveTypedBlock(timers, NAME_TIMER_LAUNCH, "Timer Block", 0, renameMode, report);
    timerDocking   = ResolveTypedBlock(timers, NAME_TIMER_DOCKING, "Timer Block", 1, renameMode, report);
    timerEscortOn  = ResolveTypedBlock(timers, NAME_TIMER_ESCORT_ON, "Timer Block", 2, renameMode, report);
    timerAIOff     = ResolveTypedBlock(timers, NAME_TIMER_AI_OFF, "Timer Block", 3, renameMode, report);
    timerFollowMe  = ResolveTypedBlock(timers, NAME_TIMER_FOLLOW_ME, "Timer Block", 4, renameMode, report);

    // ---- AI Blocks (using confirmed real interfaces, not guessed subtypes) ----
    List<IMyBasicMissionBlock> escortBlocks = new List<IMyBasicMissionBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBasicMissionBlock>(escortBlocks, b => b.CubeGrid == Me.CubeGrid);
    aiEscort = ResolveTypedBlock(escortBlocks, NAME_AI_ESCORT, "AI Basic", 0, renameMode, report);

    List<IMyFlightMovementBlock> flightBlocks = new List<IMyFlightMovementBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyFlightMovementBlock>(flightBlocks, b => b.CubeGrid == Me.CubeGrid);
    aiFlight = ResolveTypedBlock(flightBlocks, NAME_AI_FLIGHT, "AI Flight", 0, renameMode, report);

    List<IMyOffensiveCombatBlock> combatBlocks = new List<IMyOffensiveCombatBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyOffensiveCombatBlock>(combatBlocks, b => b.CubeGrid == Me.CubeGrid);
    aiCombat = ResolveTypedBlock(combatBlocks, NAME_AI_COMBAT, "AI Offensive", 0, renameMode, report);

    List<IMyPathRecorderBlock> recorderBlocks = new List<IMyPathRecorderBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyPathRecorderBlock>(recorderBlocks, b => b.CubeGrid == Me.CubeGrid);
    aiRecLaunch  = ResolveTypedBlock(recorderBlocks, NAME_AI_REC_LAUNCH, "AI Recorder", 0, renameMode, report);
    aiRecDocking = ResolveTypedBlock(recorderBlocks, NAME_AI_REC_DOCKING, "AI Recorder", 1, renameMode, report);

    // ---- Camera ----
    List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyCameraBlock>(cameras, b => b.CubeGrid == Me.CubeGrid);
    camera = ResolveTypedBlock(cameras, NAME_CAMERA, "Camera", 0, renameMode, report);

    // ---- Broadcaster (Antenna) ----
    List<IMyRadioAntenna> antennas = new List<IMyRadioAntenna>();
    GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(antennas, b => b.CubeGrid == Me.CubeGrid);
    if (antennas.Count > 0)
    {
        fleetAntenna = antennas[0];
        if (renameMode && !antennas[0].CustomName.Equals(NAME_BROADCASTER) && !antennas[0].CustomName.StartsWith("Drone ("))
        {
            antennas[0].CustomName = NAME_BROADCASTER;
            report?.AppendLine(antennas[0].CustomName + " -> " + NAME_BROADCASTER);
        }
    }

    // ---- Connectors ----
    connectors.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, b => b.CubeGrid == Me.CubeGrid);

    // ---- Remote Control (fighters only - used for autodock waypoints) ----
    List<IMyRemoteControl> remoteControls = new List<IMyRemoteControl>();
    GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(remoteControls, b => b.CubeGrid == Me.CubeGrid);
    remoteControl = ResolveTypedBlock(remoteControls, NAME_REMOTE_CONTROL, "Remote Control", 0, renameMode, report);
}

// Generic resolver: finds a block by its exact final name first (normal
// operation), or falls back to matching the Nth block of this type whose
// name still starts with the given default-name prefix (Setup/rename mode).
// This replaces guessing at subtype strings - T here is the CONFIRMED real
// interface for each AI block (IMyBasicMissionBlock, IMyFlightMovementBlock,
// IMyOffensiveCombatBlock, IMyPathRecorderBlock).
T ResolveTypedBlock<T>(List<T> candidates, string finalName, string defaultNamePrefix, int index, bool renameMode, StringBuilder report) where T : class, IMyTerminalBlock
{
    foreach (var c in candidates)
        if (c.CustomName == finalName) return c;

    if (!renameMode) return null;

    List<T> defaultNamed = candidates.FindAll(c => c.CustomName.StartsWith(defaultNamePrefix));
    defaultNamed.Sort((a, b) => a.EntityId.CompareTo(b.EntityId));
    if (index < defaultNamed.Count)
    {
        string oldName = defaultNamed[index].CustomName;
        defaultNamed[index].CustomName = finalName;
        report?.AppendLine(oldName + " -> " + finalName);
        return defaultNamed[index];
    }
    return null;
}

void PopulateGroups(StringBuilder report)
{
    // NOTE: The Space Engineers ingame scripting API only allows READING
    // group membership (GetBlocks/GetBlocksOfType) - there is no AddBlock
    // method available to scripts. Groups must be populated manually in
    // the terminal (drag blocks into the group), matching the setup
    // instructions at the top of this script. This method verifies what's
    // already in each group and reports counts/gaps instead of writing to them.

    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, b => b.CubeGrid == Me.CubeGrid);

    List<IMyGasTank> allTanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(allTanks, b => b.CubeGrid == Me.CubeGrid);
    List<IMyGasTank> hydrogenTanks = allTanks.FindAll(t =>
        t.BlockDefinition.SubtypeId.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0);

    List<IMyThrust> allThrusters = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(allThrusters, b => b.CubeGrid == Me.CubeGrid);
    List<IMyThrust> hydrogenThrusters = allThrusters.FindAll(t =>
        t.BlockDefinition.SubtypeId.IndexOf("Hydrogen", StringComparison.OrdinalIgnoreCase) >= 0);
    List<IMyThrust> ionThrusters = allThrusters.FindAll(t =>
        t.BlockDefinition.SubtypeId.IndexOf("Ion", StringComparison.OrdinalIgnoreCase) >= 0 ||
        t.BlockDefinition.SubtypeId.IndexOf("Atmospheric", StringComparison.OrdinalIgnoreCase) >= 0);

    ReportGroupStatus(GROUP_BATTERIES, batteries.Count, report);
    ReportGroupStatus(GROUP_HYDROGEN_FUEL, hydrogenTanks.Count, report);
    ReportGroupStatus(GROUP_HYDROGEN_THRUST, hydrogenThrusters.Count, report);
    ReportGroupStatus(GROUP_ION_THRUST, ionThrusters.Count, report);
}

void ReportGroupStatus(string groupName, int expectedCount, StringBuilder report)
{
    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName(groupName);
    if (group == null)
    {
        report?.AppendLine("Error: Lacking Group \"" + groupName + "\" (create it manually in the terminal)");
        return;
    }
    List<IMyTerminalBlock> members = new List<IMyTerminalBlock>();
    group.GetBlocks(members);
    report?.AppendLine("Group \"" + groupName + "\": " + members.Count + " block(s) found on grid (" +
        expectedCount + " expected of matching type) - add manually in terminal if counts don't match");
}

void SyncBroadcasterName()
{
    if (fleetAntenna == null) return;
    string hudName = fleetAntenna.CustomName;
    string desiredName = "Drone (" + hudName + ")";
    if (fleetAntenna.CustomName != desiredName)
    {
        // Note: we rename the antenna itself to reflect broadcaster naming convention
        // since the Broadcaster IS the antenna in this setup.
        fleetAntenna.CustomName = desiredName;
    }
}

// ============================================================
//  PLAYER / FOLLOW VALIDATION
// ============================================================

// IMPORTANT LIMITATION: Space Engineers' in-game Programmable Block scripts
// have NO reliable way to validate a typed player name against anything -
// there's no player list (GetPlayers/IMyPlayer aren't available to scripts),
// and block/grid ownership only exposes numeric IDs (OwnerId, BigOwners,
// SmallOwners) with no confirmed name-lookup function available to scripts
// either. So "Follow: PlayerName" is treated as informational/trusted input
// only - the script reads whatever's typed there and hands it to Basic AI
// (Escort)'s own Follow Player behavior, without trying to verify it.
// Double-check spelling manually; the script can't catch a typo for you.

string GetFollowTarget()
{
    return ParseCustomDataValue(Me.CustomData, "Follow");
}

string lastFollowValue = null;
string lastFollowStatus = "";

// Reads the Follow: line from Custom Data and echoes it whenever it
// changes. No validation is performed - see the note above IsPlayerOnline
// removal for why the script can't verify a typed name.
void CheckFollowTarget()
{
    string followName = GetFollowTarget();
    if (followName == lastFollowValue) return;
    lastFollowValue = followName;

    lastFollowStatus = string.IsNullOrWhiteSpace(followName)
        ? "Follow: (blank) -> type a player name after \"Follow:\" in Custom Data"
        : "Follow: " + followName + " (double-check spelling - not automatically validated)";
    Echo(lastFollowStatus);
}

string ParseCustomDataValue(string customData, string key)
{
    if (string.IsNullOrEmpty(customData)) return null;
    string[] lines = customData.Split('\n');
    foreach (var line in lines)
    {
        string trimmed = line.Trim();
        if (trimmed.StartsWith(key + ":", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed.Substring(key.Length + 1).Trim();
        }
    }
    return null;
}

string UpsertCustomDataLine(string customData, string key, string value)
{
    List<string> lines = new List<string>();
    if (!string.IsNullOrEmpty(customData))
        lines.AddRange(customData.Split('\n'));

    bool found = false;
    for (int i = 0; i < lines.Count; i++)
    {
        if (lines[i].Trim().StartsWith(key + ":", StringComparison.OrdinalIgnoreCase))
        {
            lines[i] = key + ": " + value;
            found = true;
            break;
        }
    }
    if (!found)
        lines.Add(key + ": " + value);

    return string.Join("\n", lines);
}

// ============================================================
//  STATE TRANSITIONS
// ============================================================

// Detects AI Recorder (Launch) finishing playback and transitions the
// fighter into either Escort mode (docked at a Mothership) or Combat/Patrol
// mode (docked at a Base - no following, just engage nearby targets).
bool launchWasPlaying = false;
void CheckLaunchComplete()
{
    if (aiRecLaunch == null) return;

    bool isPlaying = IsRecorderPlaying(aiRecLaunch);

    if (launchWasPlaying && !isPlaying)
    {
        if (dockedAtRole == "BASE")
        {
            // Base-launched fighters skip Escort entirely - just engage
            // whatever their AI Offensive block is already configured to
            // attack. IMyFunctionalBlock.Enabled is standard, confirmed
            // API - this just flips the blocks on, it doesn't touch any
            // of their in-game-configured targeting settings.
            if (aiFlight != null) aiFlight.Enabled = true;
            if (aiCombat != null) aiCombat.Enabled = true;
            currentState = DroneState.Combat;
        }
        else if (timerEscortOn != null)
        {
            // Mothership-launched (or unknown/legacy) fighters escort as before.
            timerEscortOn.Trigger();
            currentState = DroneState.Escorting;
        }
    }

    if (isPlaying)
        currentState = DroneState.Launching;

    launchWasPlaying = isPlaying;
}

bool IsRecorderPlaying(IMyTerminalBlock recorder)
{
    // AI Recorder blocks expose playback state via their DetailedInfo /
    // custom properties depending on game version. This checks DetailedInfo
    // text for a "Playing" indicator as a version-tolerant fallback.
    string info = recorder.DetailedInfo;
    if (string.IsNullOrEmpty(info)) return false;
    return info.IndexOf("Playing", StringComparison.OrdinalIgnoreCase) >= 0;
}

// ============================================================
//  FLEET COORDINATION (decentralized, via IGC)
// ============================================================

// Only tracks/uses a hangar door reference position when "Hangar Mode:
// true" is set in Custom Data - a drone using the newer connector-claim
// autodock system (Mothership/Base broadcasts) doesn't need to know where
// a physical hangar doorway is, since it's flying directly to a specific
// claimed connector instead of queuing through a single chokepoint.
bool IsHangarModeEnabled()
{
    string value = ParseCustomDataValue(Me.CustomData, CUSTOM_DATA_HANGAR_MODE_KEY);
    return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
}

// Basic Mode: makes the fighter behave as if this script weren't running
// at all - no Follow/Escort automation, no fleet coordination, no
// autodock, nothing except still watching Ammo Types (see the Basic Mode
// branch in Main()).
bool IsBasicModeEnabled()
{
    string value = ParseCustomDataValue(Me.CustomData, CUSTOM_DATA_BASIC_MODE_KEY);
    return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
}

void UpdateHangarDoorReference()
{
    bool isConnected = false;
    Vector3D connectorPos = Vector3D.Zero;
    foreach (var c in connectors)
    {
        if (c.Status == MyShipConnectorStatus.Connected)
        {
            isConnected = true;
            connectorPos = c.GetPosition();
            break;
        }
    }

    if (isConnected)
    {
        if (IsHangarModeEnabled())
            hangarDoorReference = connectorPos;

        SyncHomeTarget(); // Home: sync runs regardless of Hangar Mode - it's a separate feature
    }

    wasConnected = isConnected;
}

// While physically connected (Connector locked), GridTerminalSystem
// automatically extends to include the other grid's blocks too - this is
// the same mechanism as rotor/piston-connected grids, and is confirmed
// vanilla behavior (not IGC, no companion script needed on the other side).
// So while docked, we can directly search for a grid or Beacon matching
// the Home: name in Custom Data, grab its position, and remember it for
// use after undocking.
void SyncHomeTarget()
{
    string homeName = ParseCustomDataValue(Me.CustomData, CUSTOM_DATA_HOME_KEY);
    if (string.IsNullOrWhiteSpace(homeName))
    {
        lastHomeSyncResult = "Home: (blank) -> type a grid or Beacon name after \"Home:\" in Custom Data";
        SetStatus(lastHomeSyncResult);
        return;
    }

    // 1) Try matching a grid name first (any block whose CubeGrid.CustomName matches)
    List<IMyTerminalBlock> allReachable = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(allReachable); // no grid filter - includes docked grid while connected

    HashSet<long> seenGrids = new HashSet<long>();
    foreach (var b in allReachable)
    {
        if (seenGrids.Contains(b.CubeGrid.EntityId)) continue;
        seenGrids.Add(b.CubeGrid.EntityId);

        if (string.Equals(b.CubeGrid.CustomName, homeName, StringComparison.OrdinalIgnoreCase))
        {
            homeTargetPosition = b.CubeGrid.GetPosition();
            lastHomeSyncResult = "Home: " + homeName + " -> found (matched grid name)";
            SetStatus(lastHomeSyncResult);
            return;
        }
    }

    // 2) Fall back to matching a Beacon block's name
    List<IMyBeacon> beacons = new List<IMyBeacon>();
    GridTerminalSystem.GetBlocksOfType<IMyBeacon>(beacons); // no grid filter - includes docked grid while connected
    foreach (var beacon in beacons)
    {
        if (string.Equals(beacon.CustomName, homeName, StringComparison.OrdinalIgnoreCase))
        {
            homeTargetPosition = beacon.GetPosition();
            lastHomeSyncResult = "Home: " + homeName + " -> found (matched Beacon name)";
            SetStatus(lastHomeSyncResult);
            return;
        }
    }

    lastHomeSyncResult = "Home: " + homeName + " -> Error: no matching grid or Beacon found while docked";
    SetStatus(lastHomeSyncResult);
}

// Manual trigger for "home_synchronization" argument - runs the same
// docked-only sync as the automatic connector check, but on demand (e.g.
// run this on any/all AI drone grids while they're connected, to force a
// re-sync without waiting for the connector state to change).
void HandleHomeSyncCommand()
{
    bool isConnected = false;
    foreach (var c in connectors)
    {
        if (c.Status == MyShipConnectorStatus.Connected)
        {
            isConnected = true;
            break;
        }
    }

    if (!isConnected)
    {
        SetStatus("Error: home_synchronization requires the drone to be currently docked (Connector connected)");
        return;
    }

    SyncHomeTarget();
}

// Manual trigger for "reset_home" argument - clears both the stored Home
// target position and the Home: name in Custom Data, so the drone/ship can
// be set up fresh with a new Home target.
void HandleResetHomeCommand()
{
    homeTargetPosition = null;
    Me.CustomData = UpsertCustomDataLine(Me.CustomData, CUSTOM_DATA_HOME_KEY, "");
    SetStatus("Home reset - type a new grid or Beacon name after \"Home:\" in Custom Data, then dock and run home_synchronization");
    SaveState();
}

void BroadcastOwnPosition()
{
    if (hangarDoorReference == null) return; // no reference yet - sit out of ordering (per design)
    if (runtimeSeconds - lastPingTime < POSITION_PING_INTERVAL) return;

    lastPingTime = runtimeSeconds;

    double distance = Vector3D.Distance(Me.CubeGrid.GetPosition(), hangarDoorReference.Value);
    string myId = Me.CubeGrid.EntityId.ToString();
    string status = StateToStatus(currentState);

    string payload = "FIGHTER|" + myId + "|" + distance.ToString("F1") + "|" + status;
    IGC.SendBroadcastMessage(FLEET_CHANNEL, payload);
}

// Dispatches incoming Fleet Position Coordinator messages by type prefix:
// FIGHTER   - another fighter's distance-from-door ping (existing queue logic)
// CONNECTOR - a Mothership broadcasting one of its connectors
// CLAIM     - a fighter claiming a specific connector
// RELEASE   - a fighter releasing a claim (lost the race, or finished docking)
void HandleFleetMessage(MyIGCMessage msg)
{
    if (msg.Tag != FLEET_CHANNEL) return;
    string data = msg.As<string>();
    if (string.IsNullOrEmpty(data)) return;

    string[] parts = data.Split('|');
    if (parts.Length < 1) return;

    switch (parts[0])
    {
        case "FIGHTER": HandleFighterPing(parts); break;
        case "CONNECTOR": HandleConnectorBroadcast(parts); break;
        case "CLAIM": HandleClaimMessage(parts); break;
        case "RELEASE": HandleReleaseMessage(parts); break;
        case "SCAN": HandleScanMessage(parts); break;
    }
}

void HandleFighterPing(string[] parts)
{
    if (parts.Length < 4) return;
    string id = parts[1];
    double dist;
    if (!double.TryParse(parts[2], out dist)) return;
    string status = parts[3];

    if (id == Me.CubeGrid.EntityId.ToString()) return; // ignore own broadcast

    if (!fleetShips.ContainsKey(id))
        fleetShips[id] = new FleetShipInfo();

    fleetShips[id].Id = id;
    fleetShips[id].DistanceFromDoor = dist;
    fleetShips[id].Status = status;
    fleetShips[id].LastSeen = runtimeSeconds;
}

void PruneStaleFleetEntries()
{
    List<string> stale = new List<string>();
    foreach (var kvp in fleetShips)
    {
        if (runtimeSeconds - kvp.Value.LastSeen > FLEET_TIMEOUT_SECONDS)
            stale.Add(kvp.Key);
    }
    // Stale entries are simply removed (i.e. "ignored" per design decision -
    // a silent fighter no longer blocks the queue).
    foreach (var id in stale)
        fleetShips.Remove(id);
}

// Returns true if this fighter is clear to proceed with docking/launching
// based on the furthest-from-door-goes-first rule.
bool IsClearToProceed(bool isDocking)
{
    if (hangarDoorReference == null) return true; // no reference yet - proceed unblocked (per design)

    double myDistance = Vector3D.Distance(Me.CubeGrid.GetPosition(), hangarDoorReference.Value);
    string waitStatus = isDocking ? "waiting_dock" : "waiting_launch";

    foreach (var ship in fleetShips.Values)
    {
        if (ship.Status == waitStatus && ship.DistanceFromDoor > myDistance)
        {
            // A fighter further from the door is still waiting - we go after it.
            return false;
        }
    }
    return true;
}

string StateToStatus(DroneState state)
{
    switch (state)
    {
        case DroneState.Docking: return "waiting_dock";
        case DroneState.Launching: return "waiting_launch";
        default: return "idle";
    }
}

// ============================================================
//  MOTHERSHIP ROLE (broadcast-only, no Setup/renaming needed)
// ============================================================

// Broadcasts every DOCKING-ELIGIBLE connector on this grid: its ID,
// position, outward-facing direction (for the approach offset), and
// whether it's currently occupied. A connector only counts as
// docking-eligible if its own Custom Data contains the word "docking"
// (case-insensitive) - this lets a Mothership/Base have other connectors
// (cargo, fuel, etc.) that fighters should never try to claim. Runs
// continuously once "Mothership true" or "Base Mode true" has been set -
// no Setup required, since this role doesn't rename anything, it just
// reads connectors directly by type.
void BroadcastCarrierConnectors(string role)
{
    if (runtimeSeconds - lastConnectorBroadcastTime < CONNECTOR_BROADCAST_INTERVAL) return;
    lastConnectorBroadcastTime = runtimeSeconds;

    List<IMyShipConnector> myConnectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(myConnectors, b => b.CubeGrid == Me.CubeGrid);

    string shipId = Me.CubeGrid.EntityId.ToString();
    int broadcastCount = 0;
    foreach (var c in myConnectors)
    {
        if (c.CustomData == null || c.CustomData.IndexOf("docking", StringComparison.OrdinalIgnoreCase) < 0)
            continue; // not tagged for fighter docking - skip (e.g. cargo/fuel connectors)

        string connectorId = c.EntityId.ToString();
        Vector3D pos = c.GetPosition();
        Vector3D dir = c.WorldMatrix.Forward; // outward-facing normal of the connector
        bool occupied = c.Status == MyShipConnectorStatus.Connected;

        string payload = "CONNECTOR|" + role + "|" + shipId + "|" + connectorId + "|" +
            pos.X + ";" + pos.Y + ";" + pos.Z + "|" +
            dir.X + ";" + dir.Y + ";" + dir.Z + "|" +
            (occupied ? "1" : "0");
        IGC.SendBroadcastMessage(FLEET_CHANNEL, payload);
        broadcastCount++;
    }

    Echo(role + " mode: broadcasting " + broadcastCount + " docking-tagged connector(s) on " + FLEET_CHANNEL);
}

// ============================================================
//  CAMERA INTERIOR SCANNING (fighter-to-fighter surroundings awareness)
// ============================================================

// Uses the Camera's raycast (standard, well-documented API - EnableRaycast,
// CanScan, Raycast) to scan whatever the camera is pointed at (e.g. down a
// carrier/base's interior hangar corridor) and broadcasts what it sees to
// other fighters on the Fleet Position Coordinator channel, so a fighter
// navigating the same tight interior space can be aware of what another
// fighter has already spotted nearby (obstacles, other ships, etc.).
void ScanInteriorAndBroadcast()
{
    if (camera == null) return;
    if (runtimeSeconds - lastScanBroadcastTime < SCAN_BROADCAST_INTERVAL) return;
    lastScanBroadcastTime = runtimeSeconds;

    camera.EnableRaycast = true;
    if (!camera.CanScan(SCAN_RANGE_METERS)) return;

    MyDetectedEntityInfo hit = camera.Raycast(SCAN_RANGE_METERS);
    if (hit.IsEmpty()) return;

    string myId = Me.CubeGrid.EntityId.ToString();
    string name = string.IsNullOrEmpty(hit.Name) ? "Unknown" : hit.Name;
    Vector3D pos = hit.Position;

    string payload = "SCAN|" + myId + "|" + name + "|" + pos.X + ";" + pos.Y + ";" + pos.Z;
    IGC.SendBroadcastMessage(FLEET_CHANNEL, payload);
}

void HandleScanMessage(string[] parts)
{
    if (parts.Length < 4) return;
    string sourceId = parts[1];
    if (sourceId == Me.CubeGrid.EntityId.ToString()) return; // ignore our own scan broadcast

    string name = parts[2];
    Vector3D pos = ParseVector(parts[3]);

    if (!knownSurroundings.ContainsKey(sourceId))
        knownSurroundings[sourceId] = new ScanReport { SourceId = sourceId };

    knownSurroundings[sourceId].ObjectName = name;
    knownSurroundings[sourceId].Position = pos;
    knownSurroundings[sourceId].LastSeen = runtimeSeconds;
}

void HandleConnectorBroadcast(string[] parts)
{
    if (parts.Length < 7) return;
    string role = parts[1];
    string shipId = parts[2];
    string connectorId = parts[3];
    Vector3D pos = ParseVector(parts[4]);
    Vector3D dir = ParseVector(parts[5]);
    bool occupied = parts[6] == "1";

    if (!knownConnectors.ContainsKey(connectorId))
        knownConnectors[connectorId] = new MothershipConnectorInfo { ConnectorId = connectorId, ShipId = shipId };

    var info = knownConnectors[connectorId];
    info.Role = role;
    info.Position = pos;
    info.Direction = dir;
    info.Occupied = occupied;
    info.LastSeen = runtimeSeconds;
}

// Tracks the closest currently-known claim on each connector, so we can
// tell if a rival fighter is closer than us and yield the connector to them.
Dictionary<string, double> closestClaimDistance = new Dictionary<string, double>();
Dictionary<string, string> closestClaimOwner = new Dictionary<string, string>();

void HandleClaimMessage(string[] parts)
{
    if (parts.Length < 4) return;
    string fighterId = parts[1];
    string connectorId = parts[2];
    double dist;
    if (!double.TryParse(parts[3], out dist)) return;

    if (fighterId == Me.CubeGrid.EntityId.ToString()) return; // ignore our own claim broadcast

    double existing;
    if (!closestClaimDistance.TryGetValue(connectorId, out existing) || dist < existing)
    {
        closestClaimDistance[connectorId] = dist;
        closestClaimOwner[connectorId] = fighterId;
    }
}

void HandleReleaseMessage(string[] parts)
{
    if (parts.Length < 3) return;
    string connectorId = parts[2];
    closestClaimDistance.Remove(connectorId);
    closestClaimOwner.Remove(connectorId);
}

void PruneStaleConnectors()
{
    List<string> stale = new List<string>();
    foreach (var kvp in knownConnectors)
        if (runtimeSeconds - kvp.Value.LastSeen > FLEET_TIMEOUT_SECONDS)
            stale.Add(kvp.Key);
    foreach (var id in stale)
        knownConnectors.Remove(id);

    List<string> staleScans = new List<string>();
    foreach (var kvp in knownSurroundings)
        if (runtimeSeconds - kvp.Value.LastSeen > FLEET_TIMEOUT_SECONDS)
            staleScans.Add(kvp.Key);
    foreach (var id in staleScans)
        knownSurroundings.Remove(id);
}

Vector3D ParseVector(string s)
{
    string[] p = s.Split(';');
    double x = 0, y = 0, z = 0;
    if (p.Length == 3)
    {
        double.TryParse(p[0], out x);
        double.TryParse(p[1], out y);
        double.TryParse(p[2], out z);
    }
    return new Vector3D(x, y, z);
}

// Call this (e.g. via a "RequestDock" argument added to the AI (Docking)
// Timer Block's toolbar as a "Run Programmable Block" action) to begin the
// autonomous claim + approach + connect sequence.
void RequestDock()
{
    if (dockPhase != DockPhase.None) return; // already in progress
    dockPhase = DockPhase.Claiming;
}

// Finds this fighter's own docking connector - a connector must have the
// word "docking" (case-insensitive) in its own Custom Data, same tagging
// convention used on the Mothership/Base side. Returns null if none found.
IMyShipConnector GetDockingConnector()
{
    foreach (var c in connectors)
    {
        if (c.CustomData != null && c.CustomData.IndexOf("docking", StringComparison.OrdinalIgnoreCase) >= 0)
            return c;
    }
    return null;
}

// Call this (e.g. via a "RequestUndock" argument added to the AI Launch
// Timer Block's toolbar) to disconnect and release the connector.
void RequestUndock()
{
    IMyShipConnector dockingConnector = GetDockingConnector();
    if (dockingConnector == null) return;
    dockingConnector.Disconnect();

    if (claimedConnectorId != null)
    {
        string myId = Me.CubeGrid.EntityId.ToString();
        IGC.SendBroadcastMessage(FLEET_CHANNEL, "RELEASE|" + myId + "|" + claimedConnectorId);
        claimedConnectorId = null;
    }
    dockPhase = DockPhase.None;
    if (remoteControl != null) remoteControl.SetAutoPilotEnabled(false);
}

// ============================================================
//  AMMO TYPES (weapon presence + conveyor connectivity checks)
// ============================================================

// Parses the "Ammo Types:" block in Custom Data, returning the set of
// weapon type names currently marked "true".
List<string> GetEnabledAmmoTypes()
{
    List<string> enabled = new List<string>();
    if (Me.CustomData == null) return enabled;

    string[] lines = Me.CustomData.Split('\n');
    bool inBlock = false;
    foreach (var rawLine in lines)
    {
        string line = rawLine.Trim();
        if (line.Equals(AMMO_TYPES_HEADER, StringComparison.OrdinalIgnoreCase))
        {
            inBlock = true;
            continue;
        }
        if (!inBlock) continue;

        int colon = line.IndexOf(':');
        if (colon < 0) continue;
        string name = line.Substring(0, colon).Trim();
        string value = line.Substring(colon + 1).Trim();

        foreach (var known in AmmoTypeNames)
        {
            if (string.Equals(name, known, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
            {
                enabled.Add(known);
            }
        }
    }
    return enabled;
}

// Maps each Ammo Type label to the keyword(s) that actually appear in the
// matching weapon blocks' DefinitionDisplayNameText - these labels describe
// the AMMO (Magazine/Ammo Box/Shell/Sabot), not the weapon name itself, so
// they don't literally appear in the weapon's own display name and can't
// be substring-matched directly. Large/Small Railgun Sabot additionally
// filter by the block's actual CubeGrid.GridSizeEnum, since "Railgun"
// alone appears in both the large and small grid variants' names.
bool MatchesAmmoCategory(IMyUserControllableGun w, string category)
{
    string name = w.DefinitionDisplayNameText;
    if (string.IsNullOrEmpty(name)) return false;

    switch (category)
    {
        case "Rocket":
            // Covers Reloadable Rocket Launcher, Rocket Launcher (large
            // grid), Rocket Turret, and Rocket Turret Type II - all
            // contain "Rocket".
            return name.IndexOf("Rocket", StringComparison.OrdinalIgnoreCase) >= 0;

        case "Gatling Ammo Box":
            // Covers Gatling Gun, Warfare Gatling Gun, Gatling Turret,
            // and Gatling Turret Type II - all contain "Gatling".
            return name.IndexOf("Gatling", StringComparison.OrdinalIgnoreCase) >= 0;

        case "Assault Cannon Shell":
            return name.IndexOf("Assault Cannon", StringComparison.OrdinalIgnoreCase) >= 0;

        case "Autocannon Magazine":
            // Covers Autocannon and Autocannon Turret.
            return name.IndexOf("Autocannon", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   name.IndexOf("Auto Cannon", StringComparison.OrdinalIgnoreCase) >= 0;

        case "Artillery Shell":
            // Covers Artillery and Artillery Turret.
            return name.IndexOf("Artillery", StringComparison.OrdinalIgnoreCase) >= 0;

        case "Large Railgun Sabot":
            return name.IndexOf("Railgun", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   w.CubeGrid.GridSizeEnum == MyCubeSize.Large;

        case "Small Railgun Sabot":
            return name.IndexOf("Railgun", StringComparison.OrdinalIgnoreCase) >= 0 &&
                   w.CubeGrid.GridSizeEnum == MyCubeSize.Small;

        default:
            return name.IndexOf(category, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}

double lastAmmoCheckTime = 0;
// For each Ammo Type marked "true": confirms at least one matching weapon
// block exists on the grid (via MatchesAmmoCategory above), and that at
// least one of them has a working conveyor connection to another
// inventory on the grid (e.g. a cargo container) using the confirmed
// IMyInventory.IsConnectedTo API.
void CheckAmmoTypes()
{
    if (runtimeSeconds - lastAmmoCheckTime < AMMO_CHECK_INTERVAL) return;
    lastAmmoCheckTime = runtimeSeconds;

    List<string> enabledTypes = GetEnabledAmmoTypes();
    if (enabledTypes.Count == 0) return;

    List<IMyUserControllableGun> allWeapons = new List<IMyUserControllableGun>();
    GridTerminalSystem.GetBlocksOfType<IMyUserControllableGun>(allWeapons, b => b.CubeGrid == Me.CubeGrid);

    List<IMyCargoContainer> cargoContainers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainers, b => b.CubeGrid == Me.CubeGrid);

    foreach (var typeName in enabledTypes)
    {
        List<IMyUserControllableGun> matches = new List<IMyUserControllableGun>();
        foreach (var w in allWeapons)
        {
            if (MatchesAmmoCategory(w, typeName))
                matches.Add(w);
        }

        if (matches.Count == 0)
        {
            Echo("Error: weapon Missing (" + typeName + ")");
            continue;
        }

        if (cargoContainers.Count == 0)
        {
            continue; // nothing to check conveyor connectivity against - skip silently
        }

        bool anyConnected = false;
        foreach (var w in matches)
        {
            if (!w.HasInventory) continue;
            IMyInventory weaponInv = w.GetInventory(0);
            foreach (var cargo in cargoContainers)
            {
                if (!cargo.HasInventory) continue;
                if (weaponInv.IsConnectedTo(cargo.GetInventory(0)))
                {
                    anyConnected = true;
                    break;
                }
            }
            if (anyConnected) break;
        }

        if (!anyConnected)
            Echo("Error: Ammo Type lacking conveyoring (" + typeName + ")");
    }
}

// ============================================================
//  PLANETARY DOCKING SUITABILITY (gravity + Align to P.Gravity)
// ============================================================

// IMyShipController.GetNaturalGravity() is a confirmed, documented method -
// Remote Control implements IMyShipController, so this is solid ground.
const double GRAVITY_THRESHOLD = 0.1; // roughly 10% of a G - below this, treat as "not in meaningful gravity"
const double UNSAFE_CONNECTOR_ANGLE_DEGREES = 60.0; // how far from horizontal a connector can point before we call it unsafe under Align to P.Gravity

// The AI Flight block's "Align to P.Gravity" setting isn't exposed as a
// typed C# property - like other block-specific toggles, it can only be
// read through the generic Terminal Property system (GetProperties() /
// GetValue<T>), which IS a real, documented mechanism, but the exact
// property ID string for this particular setting isn't confirmed from
// available docs. Rather than guess a string that might silently return
// nothing, this searches the block's actual properties for anything whose
// ID/name mentions both "align" and "gravity", and reports what it found
// via Echo so we can confirm the right one together in-game.
bool? lastAlignToGravityReading = null;
string lastAlignPropertySearchResult = "";

// Diagnostic: dumps every terminal property ID + type on the AI Flight
// block to Echo, using the exact technique documented on the official
// Space Engineers wiki's scripting reference. Run this once in-game
// (argument: list_ai_flight_properties) and look for the real ID behind
// "Align to P.Gravity" - then tell me what it is and I'll swap
// TryGetAlignToGravity() to read it directly instead of searching by name
// every time, which will be both more reliable and cheaper to run.
void ListAiFlightProperties()
{
    if (aiFlight == null)
    {
        SetStatus("Error: AI Flight block not found - run Setup first");
        return;
    }

    List<ITerminalProperty> props = new List<ITerminalProperty>();
    aiFlight.GetProperties(props);

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("AI Flight terminal properties (" + props.Count + " total):");
    foreach (var p in props)
        sb.AppendLine(p.Id + " : " + p.TypeName);

    SetStatus(sb.ToString());
}
bool? TryGetAlignToGravity()
{
    if (aiFlight == null)
    {
        lastAlignToGravityReading = null;
        return null;
    }

    List<ITerminalProperty> props = new List<ITerminalProperty>();
    aiFlight.GetProperties(props);

    foreach (var p in props)
    {
        if (p.Id.IndexOf("align", StringComparison.OrdinalIgnoreCase) >= 0 &&
            p.Id.IndexOf("gravity", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            var boolProp = p.As<bool>();
            if (boolProp != null)
            {
                bool value = boolProp.GetValue(aiFlight);
                lastAlignPropertySearchResult = "Found property \"" + p.Id + "\" = " + value + " (verify this is really Align to P.Gravity in-game)";
                lastAlignToGravityReading = value;
                return value;
            }
        }
    }

    lastAlignPropertySearchResult = "Could not find an Align-to-Gravity property on AI Flight - skipping planetary docking angle check";
    lastAlignToGravityReading = null;
    return null;
}

// Returns false only when we're confident docking to this connector would
// be unsafe: meaningful natural gravity present, Align to P.Gravity
// confirmed on, and the connector's facing direction points too far from
// horizontal (i.e. the fighter would need to pitch/roll beyond what a
// horizon-locked ship can do to line up with it). If gravity is negligible,
// or we can't confirm Align to P.Gravity's state, this does NOT block
// docking - it only blocks when the check is confident, per the geometry
// described. This heuristic should be tuned/verified in-game.
bool IsSuitableForPlanetaryDocking(MothershipConnectorInfo target)
{
    if (remoteControl == null) return true;

    Vector3D gravity = remoteControl.GetNaturalGravity();
    if (gravity.Length() < GRAVITY_THRESHOLD) return true; // not in meaningful gravity - no restriction

    bool? alignOn = TryGetAlignToGravity();
    Echo("Align to P.Gravity reading: " + (lastAlignToGravityReading.HasValue ? lastAlignToGravityReading.Value.ToString() : "unknown") + " - " + lastAlignPropertySearchResult);
    if (alignOn != true) return true; // not confirmed on - don't block based on an unconfirmed reading

    Vector3D down = Vector3D.Normalize(gravity);
    Vector3D connectorDir = Vector3D.Normalize(target.Direction);

    double dot = Vector3D.Dot(connectorDir, down); // -1 = points straight up, +1 = points straight down
    double angleFromHorizontalDegrees = 90.0 - (Math.Acos(MathHelper.Clamp(dot, -1.0, 1.0)) * (180.0 / Math.PI));

    return Math.Abs(angleFromHorizontalDegrees) <= UNSAFE_CONNECTOR_ANGLE_DEGREES;
}


// Drives the fighter through: pick nearest free connector -> claim it ->
// wait briefly to see if a closer fighter also claims it -> fly to an
// offset stand-off point along the connector's facing direction -> fly to
// the connector itself -> Connect().
void RunAutodockStateMachine()
{
    if (dockPhase == DockPhase.None) return;
    if (remoteControl == null) return;

    if (connectors.Count == 0)
    {
        Echo("Error: no connector of that type exists");
        return;
    }

    IMyShipConnector dockingConnector = GetDockingConnector();
    if (dockingConnector == null)
    {
        Echo("Error: no connectors set to docking, you need at least one connector to dock!");
        return;
    }

    Vector3D myPos = Me.CubeGrid.GetPosition();

    switch (dockPhase)
    {
        case DockPhase.Claiming:
        {
            string best = null;
            double bestDist = double.MaxValue;
            foreach (var kvp in knownConnectors)
            {
                if (kvp.Value.Occupied) continue;
                if (!IsSuitableForPlanetaryDocking(kvp.Value)) continue;
                double d = Vector3D.Distance(myPos, kvp.Value.Position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = kvp.Key;
                }
            }

            if (best == null)
            {
                if (knownConnectors.Count > 0)
                    Echo("Error: No suitable docking areas available for docking!");
                else
                    Echo("Autodock: no free connectors currently known - waiting");
                return; // stay in Claiming, try again next tick
            }

            claimedConnectorId = best;
            string myId = Me.CubeGrid.EntityId.ToString();
            IGC.SendBroadcastMessage(FLEET_CHANNEL, "CLAIM|" + myId + "|" + best + "|" + bestDist.ToString("F1"));
            claimStartTime = runtimeSeconds;
            dockPhase = DockPhase.Settling;
            break;
        }

        case DockPhase.Settling:
        {
            if (runtimeSeconds - claimStartTime < CLAIM_SETTLE_SECONDS) return; // keep waiting

            // Did a closer fighter also claim this connector?
            double myDist = Vector3D.Distance(myPos, knownConnectors[claimedConnectorId].Position);
            double rivalDist;
            if (closestClaimDistance.TryGetValue(claimedConnectorId, out rivalDist) && rivalDist < myDist)
            {
                // We lost the race - release and go back to claiming a different one
                IGC.SendBroadcastMessage(FLEET_CHANNEL, "RELEASE|" + Me.CubeGrid.EntityId.ToString() + "|" + claimedConnectorId);
                claimedConnectorId = null;
                dockPhase = DockPhase.Claiming;
                return;
            }

            // We won (or no rival) - begin the approach
            var target = knownConnectors[claimedConnectorId];
            Vector3D offsetPoint = target.Position + target.Direction * APPROACH_OFFSET_METERS;

            remoteControl.ClearWaypoints();
            remoteControl.AddWaypoint(offsetPoint, "Approach Offset");
            remoteControl.AddWaypoint(target.Position, "Connector");
            remoteControl.SetAutoPilotEnabled(true);
            dockPhase = DockPhase.ApproachingOffset;
            break;
        }

        case DockPhase.ApproachingOffset:
        {
            var target = knownConnectors[claimedConnectorId];
            Vector3D offsetPoint = target.Position + target.Direction * APPROACH_OFFSET_METERS;
            if (Vector3D.Distance(myPos, offsetPoint) < APPROACH_OFFSET_METERS * 0.5)
                dockPhase = DockPhase.ApproachingConnector; // autopilot will continue to next waypoint on its own
            break;
        }

        case DockPhase.ApproachingConnector:
        {
            var target = knownConnectors[claimedConnectorId];
            if (Vector3D.Distance(myPos, target.Position) < CONNECT_DISTANCE_METERS)
            {
                dockingConnector.Connect();
                remoteControl.SetAutoPilotEnabled(false);
                dockedAtRole = target.Role; // remember whether this was a Mothership or a Base for launch behavior later
                if (timerDocking != null) timerDocking.Trigger();
                dockPhase = DockPhase.Connected;
            }
            break;
        }

        case DockPhase.Connected:
            // Docking sequence (refuel/recharge/AI-off toolbar chain) takes
            // over from here via the existing Timer Block: AI (Docking).
            dockPhase = DockPhase.None;
            break;
    }
}

// ============================================================
//  PERSISTENCE
// ============================================================

// Persists the hangar-door reference, Home target position, Mothership
// flag, Base Mode flag, and the role we last docked at, so all survive a
// script recompile / world reload:
// "hdX;hdY;hdZ|homeX;homeY;homeZ|mothership(0/1)|baseMode(0/1)|dockedAtRole"
void SaveState()
{
    string hangarPart = hangarDoorReference != null
        ? hangarDoorReference.Value.X + ";" + hangarDoorReference.Value.Y + ";" + hangarDoorReference.Value.Z
        : "none";
    string homePart = homeTargetPosition != null
        ? homeTargetPosition.Value.X + ";" + homeTargetPosition.Value.Y + ";" + homeTargetPosition.Value.Z
        : "none";
    string mothershipPart = isMothership ? "1" : "0";
    string baseModePart = isBaseMode ? "1" : "0";
    string dockedAtRolePart = dockedAtRole ?? "none";
    Storage = hangarPart + "|" + homePart + "|" + mothershipPart + "|" + baseModePart + "|" + dockedAtRolePart;
}

void LoadState()
{
    if (string.IsNullOrEmpty(Storage)) return;
    string[] sections = Storage.Split('|');
    if (sections.Length < 1) return;

    hangarDoorReference = ParseStoredVector(sections[0]);
    if (sections.Length >= 2)
        homeTargetPosition = ParseStoredVector(sections[1]);
    if (sections.Length >= 3)
        isMothership = sections[2] == "1";
    if (sections.Length >= 4)
        isBaseMode = sections[3] == "1";
    if (sections.Length >= 5)
        dockedAtRole = sections[4] == "none" ? null : sections[4];
}

Vector3D? ParseStoredVector(string section)
{
    if (string.IsNullOrEmpty(section) || section == "none") return null;
    string[] parts = section.Split(';');
    if (parts.Length != 3) return null;
    double x, y, z;
    if (double.TryParse(parts[0], out x) && double.TryParse(parts[1], out y) && double.TryParse(parts[2], out z))
        return new Vector3D(x, y, z);
    return null;
}

