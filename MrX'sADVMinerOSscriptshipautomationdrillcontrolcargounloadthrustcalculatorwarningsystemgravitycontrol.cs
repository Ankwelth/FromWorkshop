// ============================================================
// Script Name: MrX's ADV Miner OS
// Author: Trey (with original by MrXSmiles420 + Friend Additions)
// Version: 1.0 (Trey CC BY-NC-SA Header Added)
// License: Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International (CC BY-NC-SA 4.0)
// ============================================================
//
// ==================== FEATURES (FULL) ====================
//
// 1. 6-Way Thrust-to-Weight Calculation
// 2. Dynamic Warning System (LCDs, cockpit surfaces, lights, sounds)
// 3. Current Down Axis Highlighting
// 4. Automated Drill Management (power off when overweight)
// 5. Crusher Automation (mass-dependent)
// 6. Manual Ore Unload ("UNLOAD" argument)
// 7. Fast Axis Updates (every tick, Update10)
// 8. Persistent Error Logging
// 9. Zero-G Safeguard (minimum acceleration for safety)
// 10. Orbit Flight Indicator (safe/not safe)
// 11. Configurable Thresholds & Presets
// 12. Full Integration of features without breaking functionality
//
// ==================== SETUP INSTRUCTIONS ====================
//
// 1. Paste this script into a Programmable Block (PB) and set Update10 frequency.
// 2. Assign a main cockpit (required for gravity and mass calculations).
// 3. Name LCDs or cockpit surfaces '[TWR]' to display TWR/status info.
// 4. Name lights '[TWR]' to show thresholds (Green/Yellow/Red).
// 5. Name sound blocks '[WARN]' for audible warnings at mid/max thresholds.
// 6. Drills monitored automatically; no manual setup required.
// 7. Name refineries '[CRUSHER]' for automatic mass-dependent operation.
// 8. Use "UNLOAD" argument (hotbar or PB) to transfer ores/ingots/ice to connected grid.
// 9. LCDs/cockpits display temporary "Unload Complete" message for 15 seconds.
// 10. Adjust thresholds using presets in the configurable section at top.
// 11. Only one preset should be active at a time to avoid conflicts.
//
// ==================== LICENSE ====================
//
// This work is licensed under the Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.
// You are free to:
// - Share — copy and redistribute the material in any medium or format
// - Adapt — remix, transform, and build upon the material
// Under the following terms:
// - Attribution — You must give appropriate credit, provide a link to the license, and indicate if changes were made.
// - NonCommercial — You may not use the material for commercial purposes.
// - ShareAlike — If you remix, transform, or build upon the material, you must distribute your contributions under the same license.
//
// Full License Text: https://creativecommons.org/licenses/by-nc-sa/4.0/
//
// ==================== DETAILED SETUP INSTRUCTIONS ====================
//
// 1. Paste script into a Programmable Block (PB) and set Update10 frequency.
// 2. Assign a main cockpit (required for gravity and mass calculations).
// 3. Name LCDs or cockpit surfaces '[TWR]' to display TWR and status info.
// 4. Name lights '[TWR]' to show threshold colors (Green = safe, Yellow = mid, Red = max).
// 5. Name sound blocks '[WARN]' for audible warnings at mid and max thresholds.
// 6. Drills are automatically monitored; no manual setup required.
// 7. Name refineries '[CRUSHER]' to enable automatic mass-dependent operation.
// 8. Use "UNLOAD" argument (via hotbar or PB) to transfer ores/ingots/ice to connected grid.
// 9. LCDs/cockpit screens display temporary "Unload Complete" message for 15 seconds.
// 10. Adjust thresholds using presets in the configurable section at top of script.
// 11. Make sure only one preset line is uncommented at a time to avoid conflicts.
//
// ==================== CONFIGURABLE SETTINGS ====================
//
// Base thresholds (increased by 2% over default):
double WarningThreshold = 0.744 * 1.02;   // Mid (Yellow)
double CutoffThreshold  = 0.8835 * 1.02;  // Max (Red)
double MinAccelZeroG = 0.05;              // Minimum acceleration in zero G (m/s²)
double MinOrbitAccel = 0.1;               // Minimum acceleration for safe orbital maneuvers

// Presets for reducing thresholds below default:
// Default: Mid=0.744, Max=0.8835
// -5%: Mid≈0.7068, Max≈0.8393
// -10%: Mid≈0.6696, Max≈0.7952
// -20%: Mid≈0.5952, Max≈0.7068
//
// How to toggle presets:
//   Uncomment only one of the lines below at a time. Do not leave multiple active.
//   This allows temporary or permanent reduction in thresholds for more conservative operation.
//
// WarningThreshold = 0.744; CutoffThreshold = 0.8835;           // Default
// WarningThreshold = 0.744 * 0.95; CutoffThreshold = 0.8835 * 0.95; // -5%
// WarningThreshold = 0.744 * 0.9;  CutoffThreshold = 0.8835 * 0.9;  // -10%
// WarningThreshold = 0.744 * 0.8;  CutoffThreshold = 0.8835 * 0.8;  // -20%

// ==================== BLOCK LISTS ====================
List<IMyThrust> Thrusters = new List<IMyThrust>();
List<IMyShipDrill> Drills = new List<IMyShipDrill>();
List<IMyTextPanel> LCDs = new List<IMyTextPanel>();
List<IMyCockpit> Cockpits = new List<IMyCockpit>();
List<IMyCockpit> DisplayCockpits = new List<IMyCockpit>();
List<IMySoundBlock> Sounds = new List<IMySoundBlock>();
List<IMyLightingBlock> Lights = new List<IMyLightingBlock>();
List<IMyRefinery> Crushers = new List<IMyRefinery>();
List<IMyCargoContainer> CargoContainers = new List<IMyCargoContainer>();

// ==================== INTERNAL STATE ====================
int lastBlockCount = -1;
int tickCounter = 0;
List<string> persistentErrors = new List<string>();
string unloadStatus = "";
double unloadStatusTime = 0;
double soundTimer = 0;
const string LCD_TAG = "[TWR]";
const string WARN_SOUND_TAG = "[WARN]";
const string UNLOAD_ARG = "UNLOAD";
const string CRUSHER_TAG = "[CRUSHER]";
const string CARGO_TAG = "Ores";

// ==================== PROGRAM ENTRY ====================
Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    CacheBlocksIfChanged();
    UpdateAllDisplays();
}

// ==================== MAIN LOOP ====================
void Main(string argument, UpdateType updateSource)
{
    try
    {
        Echo("PB Running: MrX's ADV Miner OS");

        if (argument != null && argument.Trim() != "")
        {
            string argUpper = argument.Trim().ToUpper();
            if (argUpper == UNLOAD_ARG)
            {
                UnloadMinerOres();
                unloadStatus = "Unload Complete";
                unloadStatusTime = 15; // seconds
            }
            else Echo("Unknown argument: " + argument);
        }

        CacheBlocksIfChanged();

        tickCounter++;
        if (tickCounter < 1) return;
        tickCounter = 0;

        UpdateAllDisplays();
        DisplayPersistentErrors();

        if (unloadStatusTime > 0) unloadStatusTime -= Runtime.TimeSinceLastRun.TotalSeconds;
        else unloadStatus = "";

        soundTimer += Runtime.TimeSinceLastRun.TotalSeconds;
    }
    catch (Exception e)
    {
        AddPersistentError("Error: " + e.Message);
    }
}

// ==================== CACHE BLOCKS ====================
void CacheBlocksIfChanged()
{
    List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(allBlocks);
    if (allBlocks.Count == lastBlockCount) return;
    lastBlockCount = allBlocks.Count;

    Thrusters.Clear(); Drills.Clear(); LCDs.Clear(); DisplayCockpits.Clear();
    Cockpits.Clear(); Lights.Clear(); Sounds.Clear(); Crushers.Clear(); CargoContainers.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyThrust>(Thrusters);
    GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(Drills);
    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(LCDs, l => l.CustomName != null && l.CustomName.Contains(LCD_TAG));
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(DisplayCockpits, c => c.CustomName != null && c.CustomName.Contains(LCD_TAG));
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(Cockpits);
    GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(Lights, l => l.CustomName != null && l.CustomName.Contains(LCD_TAG));
    GridTerminalSystem.GetBlocksOfType<IMySoundBlock>(Sounds, s => s.CustomName != null && s.CustomName.Contains(WARN_SOUND_TAG));
    GridTerminalSystem.GetBlocksOfType<IMyRefinery>(Crushers, r => r.CustomName != null && r.CustomName.Contains(CRUSHER_TAG));
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(CargoContainers);
}

// ==================== UPDATE DISPLAYS ====================
void UpdateAllDisplays()
{
    IMyCockpit mainCockpit = null;
    for (int i = 0; i < Cockpits.Count; i++)
    {
        if (Cockpits[i] != null && Cockpits[i].IsMainCockpit)
        {
            mainCockpit = Cockpits[i];
            break;
        }
    }

    if (mainCockpit == null)
    {
        AddPersistentError("ERROR: No main cockpit set!");
        return;
    }

    double gravity = mainCockpit.GetNaturalGravity().Length();
    if (gravity <= 0.01) gravity = 0.0;
    double MinAccel = gravity > 0 ? Math.Max(gravity * 0.25, 0.1) : MinAccelZeroG;

    double f = 0, b = 0, l = 0, r = 0, u = 0, d = 0;

    for (int i = 0; i < Thrusters.Count; i++)
    {
        IMyThrust t = Thrusters[i];
        if (t == null || !t.IsFunctional) continue;
        Vector3D thrDir = Vector3D.TransformNormal(t.WorldMatrix.Forward, MatrixD.Transpose(mainCockpit.WorldMatrix));
        double thrust = t.MaxEffectiveThrust;

        f += Math.Max(0, thrDir.Z) * thrust;
        b += Math.Max(0, -thrDir.Z) * thrust;
        r += Math.Max(0, thrDir.X) * thrust;
        l += Math.Max(0, -thrDir.X) * thrust;
        u += Math.Max(0, thrDir.Y) * thrust;
        d += Math.Max(0, -thrDir.Y) * thrust;
    }

    double shipMass = mainCockpit.CalculateShipMass().TotalMass;
    double g = Math.Max(gravity, 0.01);
    double maxF = f / g, maxB = b / g, maxL = l / g, maxR = r / g, maxU = u / g, maxD = d / g;

    bool hasAccel = HasManeuverability(f, b, l, r, u, d, shipMass, MinAccel);

    string downAxis = GetCurrentDownAxis(mainCockpit);
    double downThrust = 0;
    switch (downAxis)
    {
        case "Forward": downThrust = maxF; break;
        case "Backward": downThrust = maxB; break;
        case "Left": downThrust = maxL; break;
        case "Right": downThrust = maxR; break;
        case "Up": downThrust = maxU; break;
        case "Down": downThrust = maxD; break;
    }

    bool overThreshold = shipMass > downThrust * CutoffThreshold || !hasAccel;
    bool warnThreshold = shipMass > downThrust * WarningThreshold && !overThreshold;

    // Drill power management
    if (overThreshold) ToggleDrillsOff();

    // Crusher management
    if (overThreshold) EnableCrushers();
    else DisableCrushers();

    // Warning sounds
    if (overThreshold && soundTimer >= 3) { PlayWarnings(); soundTimer = 0; }
    else if (warnThreshold && soundTimer >= 10) { PlayWarnings(); soundTimer = 0; }
    if (!warnThreshold && !overThreshold) StopWarnings();

    string results = "=== Thrust-to-Weight ===\n";
    results += "Ship Mass: " + shipMass.ToString("0,0") + " kg\n";
    results += "Forward: " + maxF.ToString("0,0") + " kg\n";
    results += "Backward: " + maxB.ToString("0,0") + " kg\n";
    results += "Left: " + maxL.ToString("0,0") + " kg\n";
    results += "Right: " + maxR.ToString("0,0") + " kg\n";
    results += "Up: " + maxU.ToString("0,0") + " kg\n";
    results += "Down: " + maxD.ToString("0,0") + " kg\n";
    results += "\nGravity: " + gravity.ToString("0.00") + " m/s²\n";
    results += "Down Axis: " + downAxis + "\n";

    if (overThreshold) results += "[INTAKE DISABLED]";
    else if (warnThreshold) results += "[WARNING]";
    else results += "[OK]";

    if (unloadStatusTime > 0) results += "\n" + unloadStatus;

    // ==================== ORBIT FLIGHT SAFETY ====================
    double maxAccel = Math.Max(Math.Max(maxF, maxB), Math.Max(maxL, Math.Max(maxR, Math.Max(maxU, maxD)))) / shipMass;
    string orbitStatus = "Orbit Flight: ";
    orbitStatus += (maxAccel >= MinOrbitAccel) ? "(Safe)" : "(Not Safe)";
    results += "\n" + orbitStatus;

    // ==================== UPDATE LCD & LIGHT COLORS ====================
    UpdateLCDColors(overThreshold, warnThreshold);
    UpdateLightColors(overThreshold, warnThreshold);

    for (int i = 0; i < LCDs.Count; i++)
        if (LCDs[i] != null && LCDs[i].IsFunctional) LCDs[i].WriteText(results, false);

    for (int i = 0; i < DisplayCockpits.Count; i++)
        if (DisplayCockpits[i] != null && DisplayCockpits[i].IsFunctional)
            DisplayCockpits[i].GetSurface(0).WriteText(results, false);
}

// ==================== HELPER FUNCTIONS ====================
bool HasManeuverability(double f, double b, double l, double r, double u, double d, double mass, double MinAccel)
{
    if (mass <= 0) return true;
    return f / mass >= MinAccel && b / mass >= MinAccel &&
           l / mass >= MinAccel && r / mass >= MinAccel &&
           u / mass >= MinAccel && d / mass >= MinAccel;
}

string GetCurrentDownAxis(IMyCockpit cockpit)
{
    Vector3D gravDir = cockpit.GetNaturalGravity();
    Vector3D worldDown = gravDir.LengthSquared() > 0.01 ? Vector3D.Normalize(gravDir) : -cockpit.WorldMatrix.Forward;
    Vector3D localDown = Vector3D.TransformNormal(worldDown, MatrixD.Transpose(cockpit.WorldMatrix));

    double absX = Math.Abs(localDown.X);
    double absY = Math.Abs(localDown.Y);
    double absZ = Math.Abs(localDown.Z);

    if (absX > absY && absX > absZ) return localDown.X > 0 ? "Right" : "Left";
    if (absY > absX && absY > absZ) return localDown.Y > 0 ? "Up" : "Down";
    return localDown.Z > 0 ? "Forward" : "Backward";
}

void UpdateLCDColors(bool over, bool warn)
{
    for (int i = 0; i < LCDs.Count; i++)
    {
        var lcd = LCDs[i];
        if (lcd == null || !lcd.IsFunctional) continue;
        lcd.BackgroundColor = Color.Black;
        if (over) lcd.FontColor = Color.Red;
        else if (warn) lcd.FontColor = Color.Yellow;
        else lcd.FontColor = Color.Green;
    }
}

void UpdateLightColors(bool over, bool warn)
{
    for (int i = 0; i < Lights.Count; i++)
    {
        var l = Lights[i];
        if (l == null || !l.IsFunctional) continue;
        if (over) l.Color = Color.Red;
        else if (warn) l.Color = Color.Yellow;
        else l.Color = Color.Green;
    }
}

// ==================== DRILLS / CRUSHERS / WARNINGS ====================
void ToggleDrillsOff() { for (int i = 0; i < Drills.Count; i++) if (Drills[i] != null) Drills[i].Enabled = false; }
void EnableCrushers() { for (int i = 0; i < Crushers.Count; i++) if (Crushers[i] != null) Crushers[i].Enabled = true; }
void DisableCrushers() { for (int i = 0; i < Crushers.Count; i++) if (Crushers[i] != null) Crushers[i].Enabled = false; }
void PlayWarnings() { for (int i = 0; i < Sounds.Count; i++) if (Sounds[i] != null && Sounds[i].IsFunctional) Sounds[i].Play(); }
void StopWarnings() { for (int i = 0; i < Sounds.Count; i++) if (Sounds[i] != null) Sounds[i].Stop(); }

// ==================== PERSISTENT ERRORS ====================
void AddPersistentError(string msg) { if (!persistentErrors.Contains(msg)) persistentErrors.Add(msg); }
void DisplayPersistentErrors() { for (int i = 0; i < persistentErrors.Count; i++) Echo(persistentErrors[i]); }

// ==================== UNLOAD ====================
void UnloadMinerOres()
{
    List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    GridTerminalSystem.GetBlocksOfType(connectors);

    IMyShipConnector myConnector = null;
    for (int i = 0; i < connectors.Count; i++)
        if (connectors[i].Status == MyShipConnectorStatus.Connected && connectors[i].OtherConnector != null)
        { myConnector = connectors[i]; break; }

    if (myConnector == null) { AddPersistentError("No connected grid."); return; }

    IMyCubeGrid targetGrid = myConnector.OtherConnector.CubeGrid;
    List<IMyCargoContainer> oreContainers = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlocksOfType(oreContainers, c => c.CubeGrid == targetGrid && c.CustomName.Contains(CARGO_TAG));

    if (oreContainers.Count == 0)
        GridTerminalSystem.GetBlocksOfType(oreContainers, c => c.CubeGrid == targetGrid);

    if (oreContainers.Count == 0) { AddPersistentError("No cargo containers on target."); return; }

    List<IMyTerminalBlock> shipBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(shipBlocks, b => b.HasInventory);

    int oreIndex = 0;
    for (int i = 0; i < shipBlocks.Count; i++)
        for (int j = 0; j < shipBlocks[i].InventoryCount; j++)
            TransferItemsFromMiner(shipBlocks[i].GetInventory(j), oreContainers, ref oreIndex);
}

void TransferItemsFromMiner(IMyInventory sourceInv, List<IMyCargoContainer> oreContainers, ref int oreIndex)
{
    if (oreContainers == null || oreContainers.Count == 0) return;
    List<MyInventoryItem> items = new List<MyInventoryItem>();
    sourceInv.GetItems(items);

    for (int i = items.Count - 1; i >= 0; i--)
    {
        MyInventoryItem item = items[i];
        string typeId = item.Type.TypeId.ToLower();
        if (!typeId.Contains("ore") && !typeId.Contains("ingot") && !typeId.Contains("ice")) continue;

        IMyCargoContainer target = oreContainers[oreIndex % oreContainers.Count];
        oreIndex++;
        IMyInventory targetInv = target.GetInventory(0);
        if (targetInv != null) sourceInv.TransferItemTo(targetInv, i, (int?)item.Amount, true);
    }
}
