// MultiSoundBlock Script By PeteMetal v1.0
// --- User Configuration & Usage Guide ---
//
// =======================================================================================
// [MANUAL COMMAND ARGUMENTS]
// Run this Programmable Block from your Cockpit/Seat Hotbar or Timer Blocks with these:
// - "status" : Triggers a full pre-flight/battle checklist. Reports connection, reactor, 
//              thruster, weapon, and missile status, followed by resource shortage warnings 
//              or "All systems nominal". (Case-insensitive: "STATUS", "Status", "status")
//
// [CUSTOM MISSILE NAMING GUIDE (e.g., RDAV Missiles)]
// To ensure the "status" report accurately counts your custom missiles:
// 1. Only name the SHIP-SIDE launch/hardpoint Merge Blocks with the word "missile".
//    Examples: "Missile Bay 01", "Launch_Merge_Missile_12", "Hardpoint Missile Aft".
// 2. DO NOT include the word "missile" in the missile-side merge blocks to avoid double counting.
// 3. Standard fixed rocket launchers will also be counted if they contain "missile" in their name.
//
// [AUTOMATIC MONITORING SETUP]
// - Collision Avoidance Alert : Requires a forward-facing Camera with the MSB_TAG in its name.
// =======================================================================================
// MULTI-DIRECTIONAL LIDAR COLLISION SYSTEM (OPTIONAL)
// =======================================================================================
// You can install cameras in all 6 directions (Front, Rear, Left, Right, Up, Down).
// Simply include the MSB_TAG (e.g., "msb") in the camera's Custom Name.
// 
// The script dynamically calculates the ship's actual movement vector (velocity)
// and automatically selects the camera facing your current flight path. 
// Perfect for drifting, decoupling, or planetary belly-landings with dampeners off!
// =======================================================================================
// - Enemy Detection    : Requires an Any Light with the MSB_TAG in its name.
//                        Setup AI/Defense Block Actions and select the light you've tagged.: 
//                        -> Target Locked = Light ON
//                        -> Target Lost   = Light OFF
// =======================================================================================
// For Connectors: Tag your main ship docking connectors (e.g., "Connector Main MSB") so the script can track your docking status.
// You'll get announcement of Jumpdrives, Primary and Secondary when 1 or 2 of them are fully charged. Scenario: Primary for normal use, Secondary to escape for instance.
// TIP: Use external soundblocks with MSB_TAG and enough range so you'll be able to hear announcements in 3rd person view.

//USER DEFINITIONS
const string MSB_TAG = "msb";            // Tag used to identify system blocks. Case-insensitive ("msb", "MSB", "My_Msb_Block")
const double SOUND_DELAY = 0.8;          // Delay between consecutive sounds (in seconds)
const float LOW_PERCENT = 0.10f;         // 10% low limit for Hydrogen, Oxygen, and Power
const double SPEED_BUFFER = 15.0;        // Look-ahead time in seconds (Time to Collision)
const double IGNORE_SPEED = 10.0;        // Speeds below 10 m/s won't trigger collision alerts
const double LATERAL_THRESHOLD = 15.0;   // Meters you can pass an object without alerting

// Thresholds for cargo levels
const double THRESH_FULL = 0.95;
const double THRESH_HIGH = 0.75;
const double THRESH_MEDIUM = 0.50;
const double THRESH_LOW = 0.25;
bool Enable_Cargo_Sounds = true;         // Set false to mute cargo updates (good for small mining ships)

// Select your Rank: Captain=142, Lieutenant=143, Colonel=144, Commander=145, Soldier=146, Civilian=147
string myRank = "AQD_SB_Arc_145";        
//END OF USER DEFINITIONS


// =======================================================================================
Random rng = new Random();               // Randomizes greetings

// Global Variables
bool isFirstRun = true; // Follow the first runtime of the script
Dictionary<string, string> sounds = new Dictionary<string, string>();
Dictionary<string, bool> lastGroupStates = new Dictionary<string, bool>();
Queue<string> soundQueue = new Queue<string>();
double timer = 0;
string lastCargoLevel = "";
    
public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    SetupSounds();
}

void SetupSounds() {
    // --- AQD Voice Lines Library (001 - 151) ---
    sounds["Cmd_Confirmed"] = "AQD_SB_Arc_001";     // Command confirmed.
    sounds["Cmd_Rejected"] = "AQD_SB_Arc_002";      // Command rejected.
    sounds["Lockdown_On"] = "AQD_SB_Arc_003";       // Lockdown ... engaged.
    sounds["Lockdown_Off"] = "AQD_SB_Arc_004";      // Lockdown ... disengaged.
    sounds["Enemies_Sighted"] = "AQD_SB_Arc_005";   // Enemies sighted.
    sounds["Enemies_None"] = "AQD_SB_Arc_006";      // No enemies in range.
    sounds["Target_Acquired"] = "AQD_SB_Arc_007";   // Target ... acquired.
    sounds["Target_Lost"] = "AQD_SB_Arc_008";       // Target ... lost.
    sounds["Turrets_Engaging"] = "AQD_SB_Arc_009";  // Turrets ... engaging.
    sounds["Turrets_Active"] = "AQD_SB_Arc_010";    // Turrets ... activated.
    sounds["Turrets_Disabled"] = "AQD_SB_Arc_011";  // Turrets ... disabled.
    sounds["Seq_Startup"] = "AQD_SB_Arc_012";       // Initializing start-up sequence.
    sounds["Seq_Powerdown"] = "AQD_SB_Arc_013";     // Initializing power-down sequence.
    sounds["Sys_Startup"] = "AQD_SB_Arc_014";       // System starting up.
    sounds["Sys_Powerdown"] = "AQD_SB_Arc_015";     // System powering down.
    sounds["Power_On"] = "AQD_SB_Arc_016";          // Power ... on.
    sounds["Power_Off"] = "AQD_SB_Arc_017";         // Power ... off.
    sounds["Collision_Alert"] = "AQD_SB_Arc_018";   // Collision alert.
    sounds["Collision_Imminent"] = "AQD_SB_Arc_019"; // Collision imminent.
    sounds["Alert"] = "AQD_SB_Arc_020";             // Alert.
    sounds["Docking_Proceed"] = "AQD_SB_Arc_021";   // Proceed to your designated docking space.
    sounds["Intruder_Alert"] = "AQD_SB_Arc_022";    // Intruder alert.
    sounds["Gear_Lock"] = "AQD_SB_Arc_023";         // Landing gears ... locked.
    sounds["Gear_Unlock"] = "AQD_SB_Arc_024";       // Landing gears ... unlocked.
    sounds["Hull_Breach"] = "AQD_SB_Arc_025";       // Hull breach.
    sounds["Shields_Critical"] = "AQD_SB_Arc_026";  // Shields ... critical.
    sounds["Shields_Down"] = "AQD_SB_Arc_027";      // Shields ... down.
    sounds["Shields_Reinit"] = "AQD_SB_Arc_028";    // Shields reinitializing.
    sounds["Shields_Charging"] = "AQD_SB_Arc_029";  // Shields charging.
    sounds["Damage_Heavy"] = "AQD_SB_Arc_030";      // Sustaining heavy damage.
    sounds["Ammo_Low"] = "AQD_SB_Arc_031";          // Ammo ... low.
    sounds["Warning"] = "AQD_SB_Arc_032";           // Warning.
    sounds["Emotional_Support"] = "AQD_SB_Arc_033"; // Offering emotional support.
    sounds["Activated"] = "AQD_SB_Arc_034";         // Activated.
    sounds["Deactivated"] = "AQD_SB_Arc_035";       // Deactivated.
    sounds["Gravity_Entry"] = "AQD_SB_Arc_036";     // Entering planetary gravity.
    sounds["Gravity_Exit"] = "AQD_SB_Arc_037";      // Exiting planetary gravity.
    sounds["Jump_Charging"] = "AQD_SB_Arc_038";     // Jumpdrive charging.
    sounds["Jump_Complete"] = "AQD_SB_Arc_039";     // Jump complete.
    sounds["New_Contact"] = "AQD_SB_Arc_040";       // New contact detected.
    sounds["Power_Low"] = "AQD_SB_Arc_041";         // Power ... low.
    sounds["Hydro_Low"] = "AQD_SB_Arc_042";         // Hydrogen ... low.
    sounds["O2_Low"] = "AQD_SB_Arc_043";            // Oxygen ... low.
    sounds["Ore_Detected"] = "AQD_SB_Arc_044";      // Ore detected.
    sounds["Conn_Established"] = "AQD_SB_Arc_045";  // Connection established.
    sounds["Conn_Lost"] = "AQD_SB_Arc_046";         // Connection lost.
    sounds["Opening"] = "AQD_SB_Arc_047";           // Opening.
    sounds["Closing"] = "AQD_SB_Arc_048";           // Closing.
    sounds["Pressurizing"] = "AQD_SB_Arc_049";      // Pressurizing.
    sounds["Depressurizing"] = "AQD_SB_Arc_050";    // Depressurizing.
    sounds["Primary"] = "AQD_SB_Arc_051";           // Primary.
    sounds["Secondary"] = "AQD_SB_Arc_052";         // Secondary.
    sounds["Tertiary"] = "AQD_SB_Arc_053";          // Tertiary.
    sounds["On"] = "AQD_SB_Arc_054";                // On.
    sounds["Off"] = "AQD_SB_Arc_055";               // Off.
    sounds["Thrust_On"] = "AQD_SB_Arc_056";         // Thrusters ... engaged.
    sounds["Thrust_Off"] = "AQD_SB_Arc_057";        // Thrusters ... disabled.
    sounds["Lights_On"] = "AQD_SB_Arc_058";         // Lights ... on.
    sounds["Lights_Off"] = "AQD_SB_Arc_059";        // Lights ... off.
    sounds["Conn_Lock"] = "AQD_SB_Arc_060";         // Connector ... locked.
    sounds["Conn_Unlock"] = "AQD_SB_Arc_061";       // Connector ... unlocked.
    sounds["Greeting_1"] = "AQD_SB_Arc_062";        // Welcome aboard.
    sounds["Greeting_2"] = "AQD_SB_Arc_063";        // Welcome.
    sounds["Missiles_Armed"] = "AQD_SB_Arc_064";    // Missiles ... armed.
    sounds["Missile_Armed"] = "AQD_SB_Arc_065";     // Missile ... armed.
    sounds["Hangar_Open"] = "AQD_SB_Arc_066";       // Hangar doors opening.
    sounds["Hangar_Close"] = "AQD_SB_Arc_067";      // Hangar doors closing.
    sounds["Reloading"] = "AQD_SB_Arc_068";         // Reloading.
    sounds["Sys_Nominal"] = "AQD_SB_Arc_069";       // All systems nominal.
    sounds["Reactors_On"] = "AQD_SB_Arc_070";       // Reactors ... online.
    sounds["Reactors_Off"] = "AQD_SB_Arc_071";      // Reactors ... offline.
    sounds["Extending"] = "AQD_SB_Arc_072";         // Extending.
    sounds["Retracting"] = "AQD_SB_Arc_073";        // Retracting.
    sounds["Sensors_On"] = "AQD_SB_Arc_074";        // Sensors ... online.
    sounds["Sensors_Off"] = "AQD_SB_Arc_075";       // Sensors ... offline.
    sounds["Weapons_On"] = "AQD_SB_Arc_076";        // Weapons ... online.
    sounds["Weapons_Off"] = "AQD_SB_Arc_077";       // Weapons ... offline.
    sounds["Standby_On"] = "AQD_SB_Arc_078";        // Standby mode ... engaged.
    sounds["Standby_Off"] = "AQD_SB_Arc_079";       // Standby mode ... disengaged.
    sounds["Completed"] = "AQD_SB_Arc_080";         // Completed.
    sounds["Aborted"] = "AQD_SB_Arc_081";           // Aborted.
    sounds["Loc_Hangar"] = "AQD_SB_Arc_082";        // Hangar.
    sounds["Loc_Airlock"] = "AQD_SB_Arc_083";       // Airlock.
    sounds["Loc_Engine"] = "AQD_SB_Arc_084";        // Engine room.
    sounds["Loc_CargoBay"] = "AQD_SB_Arc_085";      // Cargo bay.
    sounds["Loc_Cryo"] = "AQD_SB_Arc_086";          // Cryo bay.
    sounds["Loc_Crew"] = "AQD_SB_Arc_087";          // Crew quarters.
    sounds["Loc_Bridge"] = "AQD_SB_Arc_088";        // Bridge.
    sounds["Dir_Forward"] = "AQD_SB_Arc_089";       // Forward.
    sounds["Dir_Aft"] = "AQD_SB_Arc_090";           // Aft.
    sounds["Dir_Port"] = "AQD_SB_Arc_091";          // Port.
    sounds["Dir_Starboard"] = "AQD_SB_Arc_092";     // Starbord.
    sounds["Dir_Upper"] = "AQD_SB_Arc_093";         // Upper.
    sounds["Dir_Lower"] = "AQD_SB_Arc_094";         // Lower.
    sounds["Num_1"] = "AQD_SB_Arc_095";             // One.
    sounds["Num_2"] = "AQD_SB_Arc_096";             // Two.
    sounds["Num_3"] = "AQD_SB_Arc_097";             // Three.
    sounds["Num_4"] = "AQD_SB_Arc_098";             // Four.
    sounds["Num_5"] = "AQD_SB_Arc_099";             // Five.
    sounds["Num_6"] = "AQD_SB_Arc_100";             // Six.
    sounds["Num_7"] = "AQD_SB_Arc_101";             // Seven.
    sounds["Num_8"] = "AQD_SB_Arc_102";             // Eight.
    sounds["Num_9"] = "AQD_SB_Arc_103";             // Nine.
    sounds["Num_10"] = "AQD_SB_Arc_104";            // Ten.
    sounds["Num_11"] = "AQD_SB_Arc_105";            // Eleven.
    sounds["Num_12"] = "AQD_SB_Arc_106";            // Twelve.
    sounds["Num_13"] = "AQD_SB_Arc_107";            // Thirteen.
    sounds["Num_14"] = "AQD_SB_Arc_108";            // Fourteen.
    sounds["Num_15"] = "AQD_SB_Arc_109";            // Fifteen.
    sounds["Num_16"] = "AQD_SB_Arc_110";            // Sixteen.
    sounds["Num_17"] = "AQD_SB_Arc_111";            // Seventeen.
    sounds["Num_18"] = "AQD_SB_Arc_112";            // Eighteen.
    sounds["Num_19"] = "AQD_SB_Arc_113";            // Nineteen.
    sounds["Num_20"] = "AQD_SB_Arc_114";            // Twenty.
    sounds["Num_30"] = "AQD_SB_Arc_115";            // Thirty.
    sounds["Num_40"] = "AQD_SB_Arc_116";            // Fourty.
    sounds["Num_50"] = "AQD_SB_Arc_117";            // Fifty.
    sounds["Num_60"] = "AQD_SB_Arc_118";            // Sixty.
    sounds["Num_70"] = "AQD_SB_Arc_119";            // Seventy.
    sounds["Num_80"] = "AQD_SB_Arc_120";            // Eighty.
    sounds["Num_90"] = "AQD_SB_Arc_121";            // Ninety.
    sounds["Num_Hundred"] = "AQD_SB_Arc_122";       // Hundred.
    sounds["Num_Thousand"] = "AQD_SB_Arc_123";      // Thousand.
    sounds["Num_Million"] = "AQD_SB_Arc_124";       // Million.
    sounds["Standby"] = "AQD_SB_Arc_125";           // Standby.
    sounds["Ready"] = "AQD_SB_Arc_126";             // Ready.
    sounds["Full"] = "AQD_SB_Arc_127";              // Full.
    sounds["Low"] = "AQD_SB_Arc_128";               // Low.
    sounds["Medium"] = "AQD_SB_Arc_129";            // Medium.
    sounds["High"] = "AQD_SB_Arc_130";              // High.
    sounds["Defcon"] = "AQD_SB_Arc_131";            // Defcon.
    sounds["Alert_Annc"] = "AQD_SB_Arc_132";        // Alert.
    sounds["Defcon_Red"] = "AQD_SB_Arc_133";        // Red.
    sounds["Defcon_Orange"] = "AQD_SB_Arc_134";     // Orange.
    sounds["Defcon_Yellow"] = "AQD_SB_Arc_135";     // Yellow.
    sounds["Defcon_Green"] = "AQD_SB_Arc_136";      // Green.
    sounds["Defcon_Blue"] = "AQD_SB_Arc_137";       // Blue.
    sounds["Defcon_Purple"] = "AQD_SB_Arc_138";     // Purple.
    sounds["Greeting_3"] = "AQD_SB_Arc_139";        // Hello.
    sounds["Greeting_4"] = "AQD_SB_Arc_140";        // Greetings.
    sounds["Goodbye"] = "AQD_SB_Arc_141";           // Goodbye.
    sounds["Rank_Captain"] = "AQD_SB_Arc_142";      // Captain.
    sounds["Rank_Lieutenant"] = "AQD_SB_Arc_143";   // Lieutenant.
    sounds["Rank_Colonel"] = "AQD_SB_Arc_144";      // Colonel.
    sounds["Rank_Commander"] = "AQD_SB_Arc_145";    // Commander.
    sounds["Rank_Soldier"] = "AQD_SB_Arc_146";      // Soldier.
    sounds["Rank_Civilian"] = "AQD_SB_Arc_147";     // Civilian.
    sounds["Merge"] = "AQD_SB_Arc_148";             // Merge.
    sounds["Warhead_Armed"] = "AQD_SB_Arc_149";     // Warhead... armed.
    sounds["Warhead_Disarmed"] = "AQD_SB_Arc_150";  // Warhead... disarmed.
    sounds["Countdown_Init"] = "AQD_SB_Arc_151";    // Count down sequence initiated.
}

public void Main(string argument, UpdateType updateSource) {
    timer += Runtime.TimeSinceLastRun.TotalSeconds;

    // Turn off Combatlight on first runtime
    if (isFirstRun) {
        List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
        GridTerminalSystem.GetBlocksOfType(lights, l => l.CustomName.ToLower().Contains(MSB_TAG.ToLower()) && l.CubeGrid == Me.CubeGrid);
        if (lights.Count > 0) {
            lights[0].Enabled = false;
        }
        isFirstRun = false;
    }    
    // --- Parse Manual Command Arguments (Case-Insensitive & Trimmed) ---
    if (!string.IsNullOrEmpty(argument)) {
        string arg = argument.ToLower().Trim();
        if (arg == "status") {
            TriggerStatusReport();
        }
    }
    
    // Handle Sound Queue
    if (timer >= SOUND_DELAY && soundQueue.Count > 0) {
        if (PlaySound(soundQueue.Peek())) {
            soundQueue.Dequeue();
            timer = 0;
        }
    }
    
    // Run all background monitors
    CheckConnectorGroup();
    CheckLandingGearGroup();
    CheckThrusterGroup();
    CheckResourceAndPowerState();
    CheckCockpitInteraction();
    CheckCombatStatus();
    CheckCollision();
    CheckCargoLevels();
    CheckSystemPowerAndWeapons();
    CheckAmmoLevels();
    CheckJumpDriveSystems();
    CheckGravityEnvironment();
}

void TriggerStatusReport() {
    bool conn = lastGroupStates.GetValueOrDefault("Conn", false);
    bool reactors = lastGroupStates.GetValueOrDefault("Reactors", false);
    bool thrust = lastGroupStates.GetValueOrDefault("Thrust", false);
    bool weapons = lastGroupStates.GetValueOrDefault("Weapons", false);
    
    soundQueue.Enqueue(sounds[conn ? "Conn_Lock" : "Conn_Unlock"]);
    soundQueue.Enqueue(sounds[reactors ? "Reactors_On" : "Reactors_Off"]);
    soundQueue.Enqueue(sounds[thrust ? "Thrust_On" : "Thrust_Off"]);
    soundQueue.Enqueue(sounds[weapons ? "Weapons_On" : "Weapons_Off"]);
    
    // EXPANDED: Now accepts Guns, Turrets, AND Merge Blocks containing the word "missile"
    List<IMyFunctionalBlock> missileBlocks = new List<IMyFunctionalBlock>();
    GridTerminalSystem.GetBlocksOfType(missileBlocks, b => b.IsSameConstructAs(Me) && 
        (b is IMyUserControllableGun || b is IMyLargeTurretBase || b is IMyShipMergeBlock) && 
        b.CustomName.ToLower().Contains("missile"));
        
    int activeMissiles = 0;
    foreach (var b in missileBlocks) {
        // If it's a weapon, check if it's enabled. 
        // If it's a Merge Block, check if it's currently powered on and holding a missile
        if (b.Enabled) {
            activeMissiles++;
        }
    }
    
    if (activeMissiles > 1) soundQueue.Enqueue(sounds["Missiles_Armed"]);
    else if (activeMissiles == 1) soundQueue.Enqueue(sounds["Missile_Armed"]);

    bool lowPower = lastGroupStates.GetValueOrDefault("LowPower", false);
    bool lowHydro = lastGroupStates.GetValueOrDefault("LowHydro", false);
    bool lowOxygen = lastGroupStates.GetValueOrDefault("LowOxygen", false);
    bool lowAmmo = lastGroupStates.GetValueOrDefault("LowAmmo", false);

    bool anyShortage = false;
    if (lowPower)  { soundQueue.Enqueue(sounds["Power_Low"]); anyShortage = true; }
    if (lowHydro)  { soundQueue.Enqueue(sounds["Hydro_Low"]); anyShortage = true; }
    if (lowOxygen) { soundQueue.Enqueue(sounds["O2_Low"]);    anyShortage = true; }
    if (lowAmmo)   { soundQueue.Enqueue(sounds["Ammo_Low"]);  anyShortage = true; }

    if (!anyShortage) {
        soundQueue.Enqueue(sounds["Sys_Nominal"]); 
    }
}

void CheckAmmoLevels() {
    List<IMyUserControllableGun> guns = new List<IMyUserControllableGun>();
    GridTerminalSystem.GetBlocksOfType(guns, g => g.IsSameConstructAs(Me));
    
    bool isLow = false;
    foreach (var g in guns) {
        if (g.Enabled) {
            var inv = g.GetInventory(0);
            if (inv != null) {
                if ((float)inv.CurrentVolume / (float)inv.MaxVolume <= LOW_PERCENT) {
                    isLow = true;
                    break;
                }
            }
        }
    }
    UpdateGroupState("LowAmmo", isLow, "Ammo_Low", null);
}

void CheckJumpDriveSystems() {
    List<IMyJumpDrive> drives = new List<IMyJumpDrive>();
    GridTerminalSystem.GetBlocksOfType(drives, d => d.CubeGrid == Me.CubeGrid);
    
    int totalDrives = drives.Count;
    if (totalDrives == 0) return;

    int readyDrives = 0;
    int chargingDrives = 0;

    foreach (var d in drives) {
        if (d.Status == MyJumpDriveStatus.Ready) readyDrives++;
        else if (d.Status == MyJumpDriveStatus.Charging) chargingDrives++;
    }

    bool primaryCharging = (readyDrives == 0 && chargingDrives > 0);
    bool primaryReady = (readyDrives >= 1);
    bool secondaryCharging = (totalDrives > 1 && readyDrives > 0 && readyDrives < totalDrives && chargingDrives > 0);
    bool secondaryReady = (totalDrives > 1 && readyDrives == totalDrives);

    if (primaryCharging && !lastGroupStates.GetValueOrDefault("PrimCharge", false)) {
        soundQueue.Enqueue(sounds["Primary"]);
        soundQueue.Enqueue(sounds["Jump_Charging"]);
    }
    lastGroupStates["PrimCharge"] = primaryCharging;

    if (primaryReady && !lastGroupStates.GetValueOrDefault("PrimReady", false)) {
        soundQueue.Enqueue(sounds["Primary"]);
        soundQueue.Enqueue(sounds["Ready"]);
    }
    lastGroupStates["PrimReady"] = primaryReady;

    if (secondaryCharging && !lastGroupStates.GetValueOrDefault("SecCharge", false)) {
        soundQueue.Enqueue(sounds["Secondary"]);
        soundQueue.Enqueue(sounds["Jump_Charging"]);
    }
    lastGroupStates["SecCharge"] = secondaryCharging;

    if (secondaryReady && !lastGroupStates.GetValueOrDefault("SecReady", false)) {
        soundQueue.Enqueue(sounds["Secondary"]);
        soundQueue.Enqueue(sounds["Ready"]);
    }
    lastGroupStates["SecReady"] = secondaryReady;
}

void CheckSystemPowerAndWeapons() {
    List<IMyReactor> reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(reactors, r => r.CubeGrid == Me.CubeGrid);
    bool reactorsActive = false;
    foreach (var r in reactors) if (r.Enabled) reactorsActive = true;
    UpdateGroupState("Reactors", reactorsActive, "Reactors_On", "Reactors_Off");

    List<IMyLargeTurretBase> turrets = new List<IMyLargeTurretBase>();
    GridTerminalSystem.GetBlocksOfType(turrets, t => t.IsSameConstructAs(Me));
    
    bool weaponsArmed = false;
    foreach (var t in turrets) {
        if (t.Enabled) {
            weaponsArmed = true; 
            break;
        }
    }
    UpdateGroupState("Weapons", weaponsArmed, "Weapons_On", "Weapons_Off");
}

void CheckGravityEnvironment() {
    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.CubeGrid == Me.CubeGrid);
    bool inGravity = false;
    foreach (var c in controllers) {
        if (c.GetNaturalGravity().Length() > 0) inGravity = true;
    }
    UpdateGroupState("Gravity", inGravity, "Gravity_Entry", "Gravity_Exit");
}

void CheckCargoLevels() {
    if (!Enable_Cargo_Sounds) return;
    double fillPercent = GetCargoFillLevel();
    string currentLevel = "";

    if (fillPercent >= THRESH_FULL) currentLevel = "Full";
    else if (fillPercent >= THRESH_HIGH) currentLevel = "High";
    else if (fillPercent >= THRESH_MEDIUM) currentLevel = "Medium";
    else if (fillPercent >= THRESH_LOW) currentLevel = "Low";

    if (currentLevel != lastCargoLevel && !string.IsNullOrEmpty(currentLevel)) {
        if (IsLevelHigher(currentLevel, lastCargoLevel)) {
            soundQueue.Enqueue(sounds["Loc_CargoBay"]); 
            
            if (currentLevel == "Low") {
                soundQueue.Enqueue(sounds["Low"]);      
                soundQueue.Enqueue(sounds["Num_20"]);   
                soundQueue.Enqueue(sounds["Num_5"]);    
            }
            else if (currentLevel == "Medium") {
                soundQueue.Enqueue(sounds["Medium"]);   
                soundQueue.Enqueue(sounds["Num_50"]);   
            }
            else if (currentLevel == "High") {
                soundQueue.Enqueue(sounds["High"]);     
                soundQueue.Enqueue(sounds["Num_70"]);   
                soundQueue.Enqueue(sounds["Num_5"]);    
            }
            else if (currentLevel == "Full") {
                soundQueue.Enqueue(sounds["Full"]);     
                soundQueue.Enqueue(sounds["Docking_Proceed"]); 
            }
        }
    }
    lastCargoLevel = currentLevel;
}

double GetCargoFillLevel() {
    List<IMyCargoContainer> containers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(containers, c => c.CubeGrid == Me.CubeGrid);
    MyFixedPoint cur = 0, max = 0;
    foreach (var c in containers) {
        var inv = c.GetInventory();
        cur += inv.CurrentVolume; max += inv.MaxVolume;
    }
    return max == 0 ? 0 : (double)cur / (double)max;
}

bool IsLevelHigher(string current, string last) {
    Dictionary<string, int> priority = new Dictionary<string, int> {
        {"", 0}, {"Low", 1}, {"Medium", 2}, {"High", 3}, {"Full", 4}
    };
    int c = priority.ContainsKey(current) ? priority[current] : 0;
    int l = priority.ContainsKey(last) ? priority[last] : 0;
    return c > l;
}

void CheckCombatStatus() {
    IMyInteriorLight signalLight = null;
    List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
    //Tag matching is fully case-insensitive from configuration down to block name
    GridTerminalSystem.GetBlocksOfType(lights, l => l.CustomName.ToLower().Contains(MSB_TAG.ToLower()) && l.CubeGrid == Me.CubeGrid);
    
    if (lights.Count > 0) signalLight = lights[0];

    if (signalLight == null) {
        Echo("!!! SETUP ERROR !!!");
        Echo($"Required block missing: {MSB_TAG}");
        Echo("----------------------------------");
        Echo("HOW TO FIX:");
        Echo("1. Create an Interior Light.");
        Echo($"2. Name it with: {MSB_TAG}");
        Echo("3. Go to AI Block 'Setup Actions'.");
        Echo("- Target Locked -> Light ON");
        Echo("- Target Lost -> Light OFF");
        return;
    }

    bool currentIsLocked = signalLight.Enabled;
    UpdateGroupState("State_Locked", currentIsLocked, "Enemies_Sighted", "Enemies_None");

    IMyTextPanel debugLcd = null;
    List<IMyTextPanel> lcds = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(lcds, l => l.CustomName.ToLower().Contains(MSB_TAG.ToLower()) && l.CubeGrid == Me.CubeGrid);
    if (lcds.Count > 0) debugLcd = lcds[0];
    if (debugLcd != null) {
        string status = currentIsLocked ? ">>> TARGET LOCKED <<<" : "Scanning for targets...";
        debugLcd.WriteText($"[AI RADAR SYSTEM]\nStatus: {status}\nSignal Light: OK");
    }

    Echo("System: Active");
    Echo($"Target Locked: {currentIsLocked}");
}

void CheckCollision() {
    List<IMyCameraBlock> allCameras = new List<IMyCameraBlock>();
    GridTerminalSystem.GetBlocksOfType(allCameras, c => c.CustomName.ToLower().Contains(MSB_TAG.ToLower()) && c.IsSameConstructAs(Me));
    
    if (allCameras.Count == 0 || IsDocked()) { ResetCollisionStates(); return; }

    // Keep ALL cameras pre-charging their lasers constantly so they are instantly ready to scan in 360°
    foreach (var c in allCameras) {
        c.EnableRaycast = true;
    }

    Vector3D velocityVec = Me.CubeGrid.LinearVelocity;
    double speed = velocityVec.Length();

    if (speed < IGNORE_SPEED) { ResetCollisionStates(); return; }

    Vector3D dir = Vector3D.Normalize(velocityVec);
    
    // Highest Dot Product selection (Picks the camera pointing closest to movement vector)
    IMyCameraBlock activeCamera = null;
    double highestDot = -1.0;

    foreach (var c in allCameras) {
        double dot = dir.Dot(c.WorldMatrix.Forward);
        if (dot > highestDot) {
            highestDot = dot;
            activeCamera = c;
        }
    }

    if (activeCamera == null || highestDot < 0.5) { ResetCollisionStates(); return; }

    double scanDist = Math.Max(50, Math.Min(6000, speed * SPEED_BUFFER));

    if (activeCamera.CanScan(scanDist)) {
        MyDetectedEntityInfo info = activeCamera.Raycast((float)scanDist);
        if (!info.IsEmpty() && info.HitPosition.HasValue) {
            Vector3D toHit = info.HitPosition.Value - activeCamera.GetPosition();
            double latDist = (info.HitPosition.Value - (activeCamera.GetPosition() + dir * toHit.Dot(dir))).Length();

            // Safety 
            // If drifting sideways, the ship is WIDE (its length is the hazard profile). 
            // Increase the allowed lateral distance to 60 meters for side cameras.
            double currentThreshold = LATERAL_THRESHOLD;
            string camName = activeCamera.CustomName.ToLower();
            if (camName.Contains("left") || camName.Contains("right")) {
                currentThreshold = 60.0; // 60 meter safety bubble for sideways drifting
            }

            if (latDist > currentThreshold) { ResetCollisionStates(); return; }

            double ttc = toHit.Length() / speed;
            Echo($"Lidar ({activeCamera.CustomName}): Distance {toHit.Length():0}m, TTC: {ttc:0.0}s");

            if (ttc < 6.0) { 
                UpdateGroupState("CollImminent", true, "Collision_Imminent", null); 
                UpdateGroupState("CollAlert", false, null, null); 
            }
            else if (ttc < 12.0) { 
                UpdateGroupState("CollAlert", true, "Collision_Alert", null); 
                UpdateGroupState("CollImminent", false, null, null); 
            }
        } else { ResetCollisionStates(); }
    }
}

void ResetCollisionStates() {
    UpdateGroupState("CollAlert", false, null, null);
    UpdateGroupState("CollImminent", false, null, null);
}

bool IsDocked() {
    List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    
    // Only monitor the main ship's tagged docking connectors
    GridTerminalSystem.GetBlocksOfType(connectors, c => c.CubeGrid == Me.CubeGrid && c.CustomName.ToLower().Contains(MSB_TAG.ToLower()));
    foreach (var c in connectors) if (c.Status == MyShipConnectorStatus.Connected) return true;
    
    List<IMyLandingGear> gears = new List<IMyLandingGear>();
    // Only monitor landing gears on the main ship hull
    GridTerminalSystem.GetBlocksOfType(gears, g => g.CubeGrid == Me.CubeGrid);
    foreach (var g in gears) if (g.IsLocked) return true;
    return false;
}

void CheckResourceAndPowerState() {
    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    GridTerminalSystem.GetBlocksOfType(batteries, b => b.CubeGrid == Me.CubeGrid);
    float currentPower = 0, maxPower = 0; bool anyProviding = false;
    foreach(var b in batteries) {
        if (b.Enabled && b.ChargeMode != ChargeMode.Recharge) anyProviding = true;
        currentPower += b.CurrentStoredPower; maxPower += b.MaxStoredPower;
    }
    UpdateGroupState("MainPower", anyProviding, "Power_On", "Power_Off");
    if (maxPower > 0 && anyProviding) UpdateGroupState("LowPower", (currentPower / maxPower) <= LOW_PERCENT, "Power_Low", null);

    List<IMyGasTank> tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(tanks, t => t.CubeGrid == Me.CubeGrid);
    
    double curH = 0, maxH = 0;
    double curO = 0, maxO = 0;

    foreach(var t in tanks) {
        string subtype = t.BlockDefinition.SubtypeId.ToLower();
        if (subtype.Contains("hydrogen")) {
            curH += (t.Capacity * t.FilledRatio); 
            maxH += t.Capacity;
        } else {
            curO += (t.Capacity * t.FilledRatio); 
            maxO += t.Capacity;
        }
    }

    if (maxH > 0) UpdateGroupState("LowHydro", (curH / maxH) <= LOW_PERCENT, "Hydro_Low", null);
    if (maxO > 0) UpdateGroupState("LowOxygen", (curO / maxO) <= LOW_PERCENT, "O2_Low", null);
}

void CheckConnectorGroup() {
    List<IMyShipConnector> blocks = new List<IMyShipConnector>();
    
    // STRICT FILTER: Only checks connectors on the main ship hull (Me.CubeGrid) AND requires the MSB_TAG
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.CubeGrid == Me.CubeGrid && b.CustomName.ToLower().Contains(MSB_TAG.ToLower()));
    
    bool connected = false;
    foreach (var b in blocks) if (b.Status == MyShipConnectorStatus.Connected) connected = true;
    UpdateGroupState("Conn", connected, "Conn_Lock", "Conn_Unlock");
}

void CheckLandingGearGroup() {
    List<IMyLandingGear> blocks = new List<IMyLandingGear>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.IsSameConstructAs(Me));
    bool locked = false;
    foreach (var b in blocks) if (b.IsLocked) locked = true;
    UpdateGroupState("Gear", locked, "Gear_Lock", "Gear_Unlock");
}

void CheckThrusterGroup() {
    List<IMyThrust> blocks = new List<IMyThrust>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.IsSameConstructAs(Me));
    bool anyOn = false;
    foreach (var b in blocks) if (b.Enabled) anyOn = true;
    UpdateGroupState("Thrust", anyOn, "Thrust_On", "Thrust_Off");
}

void CheckCockpitInteraction() {
    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, c => c.CubeGrid == Me.CubeGrid);

    bool ownerIsPilot = false;
    foreach (var c in controllers) {
        if (c.IsUnderControl) {
            ownerIsPilot = true;
            break;
        }
    }

    if (!lastGroupStates.ContainsKey("Player")) { 
        lastGroupStates["Player"] = ownerIsPilot; 
        return; 
    }

    if (ownerIsPilot != lastGroupStates["Player"]) {
        if (ownerIsPilot) {
            soundQueue.Enqueue(sounds["Greeting_" + rng.Next(1, 5)]);
            soundQueue.Enqueue(myRank); 
        } else {
            soundQueue.Enqueue(sounds["Goodbye"]);
        }
        lastGroupStates["Player"] = ownerIsPilot;
    }
}

void UpdateGroupState(string key, bool state, string sOn, string sOff) {
    if (!lastGroupStates.ContainsKey(key)) { lastGroupStates[key] = state; return; }
    if (state != lastGroupStates[key]) {
        string sKey = state ? sOn : sOff;
        if (sKey != null && sounds.ContainsKey(sKey)) {
            if (!soundQueue.Contains(sounds[sKey])) soundQueue.Enqueue(sounds[sKey]);
        }
        lastGroupStates[key] = state;
    }
}

bool PlaySound(string soundId) {
    List<IMySoundBlock> blocks = new List<IMySoundBlock>();
    GridTerminalSystem.GetBlocksOfType(blocks, b => b.CustomName.ToLower().Contains(MSB_TAG.ToLower()) && b.CubeGrid == Me.CubeGrid);
    foreach (var sb in blocks) { sb.SelectedSound = soundId; sb.Play(); }
    return blocks.Count > 0;
}