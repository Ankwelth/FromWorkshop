/* ============================================================================
 * TACTICAL SURVEY & PROSPECTING BEACON (AUXILIARY OS)
 * ============================================================================
 * DESCRIPTION:
 * A long-range survey tool for scout drones. When triggered, it broadcasts 
 * GPS markers over the "FLEET_TARGETING_NET" channel. These are captured 
 * by the Flagship Mainframe and added to its navigation list.
 *
 * SETUP: 
 * 1. Place on any drone/scout with an active Antenna.
 * 2. Set IGC_CHANNEL to "FLEET_TARGETING_NET".
 *
 * USE (HOTBAR ARGS):
 * - ORE TYPES : URANIUM, PLATINUM, GOLD, SILVER, COBALT, IRON, NICKEL, MAGNESIUM, ICE
 * - COMBAT    : HOSTILE
 * - NAVIGATION: SAFE
 * - CUSTOM POI: Type any text (e.g., "DERELICT", "ALPHA") to create a custom marker.
 * ============================================================================
 */

const string IGC_CHANNEL = "FLEET_TARGETING_NET";

public Program()
{
    // No update frequency needed. Runs on-demand via hotbar action.
}

public void Main(string arg, UpdateType updateSource)
{
    string cmd = string.IsNullOrEmpty(arg) ? "UNKNOWN" : arg.Trim().ToUpper();
    string color = "#FF00FF"; // Default Magenta
    string prefix = "SIG";    // Default Signature

    // --- CLASSIFICATION MATRIX ---
    switch (cmd)
    {
        case "URANIUM":   color = "#00FF00"; prefix = "U-235"; break;
        case "PLATINUM":  color = "#E5E4E2"; prefix = "PT";    break;
        case "GOLD":      color = "#FFD700"; prefix = "AU";    break;
        case "SILVER":    color = "#C0C0C0"; prefix = "AG";    break;
        case "COBALT":    color = "#0047AB"; prefix = "CO";    break;
        case "IRON":      color = "#D2691E"; prefix = "FE";    break;
        case "NICKEL":    color = "#727472"; prefix = "NI";    break;
        case "MAGNESIUM": color = "#8A2BE2"; prefix = "MG";    break;
        case "ICE":       color = "#00FFFF"; prefix = "H2O";   break;
        case "HOSTILE":   color = "#FF0000"; prefix = "TRG";   break;
        case "SAFE":      color = "#FFFFFF"; prefix = "NAV";   break;
        default:          color = "#FF00FF"; prefix = "POI";   break; // Custom POIs
    }

    Vector3D pos = Me.GetPosition();
    // If it's a custom command, use the raw input as the identifier
    string label = (cmd == "UNKNOWN" || cmd == "URANIUM" || cmd == "PLATINUM" || cmd == "GOLD" || cmd == "SILVER" || cmd == "COBALT" || cmd == "IRON" || cmd == "NICKEL" || cmd == "MAGNESIUM" || cmd == "ICE" || cmd == "HOSTILE" || cmd == "SAFE") ? cmd : arg.Trim();
    string targetName = $"{prefix} - {label}";
    
    // Generates the color-coded GPS String
    string gpsString = string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                       "GPS:{0}:{1:0.00}:{2:0.00}:{3:0.00}:{4}:", 
                       targetName, pos.X, pos.Y, pos.Z, color);
    
    IGC.SendBroadcastMessage(IGC_CHANNEL, gpsString);
    
    Echo($"Telemetry Sent:\n{targetName}\nHex Color: {color}");
}