// --- CONFIGURATION ---
const string CONNECTOR_NAME = "Landing Pad Connector";
readonly string[] LIGHT_NAMES = {
    "Landing Pad Lght A",
    "Landing Pad Lght B",
    "Landing Pad Lght C",
    "Landing Pad Lght D"
};

// Colors (Red, Green, Blue)
readonly Color COLOR_LOCKED = new Color(0, 255, 0);    // Solid Green
readonly Color COLOR_UNLOCKED = new Color(255, 0, 0);  // Warning Red
// ---------------------

List<IMyLightingBlock> padsLights = new List<IMyLightingBlock>();
IMyShipConnector connector;

public Program()
{
    // Check the connector status automatically every 10 ticks (~6 times a second)
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    // Fetch and verify connector
    connector = GridTerminalSystem.GetBlockWithName(CONNECTOR_NAME) as IMyShipConnector;
    if (connector == null)
    {
        Echo($"Error: Connector '{CONNECTOR_NAME}' not found!");
        return;
    }

    // Fetch and verify lights
    padsLights.Clear();
    foreach (var name in LIGHT_NAMES)
    {
        var light = GridTerminalSystem.GetBlockWithName(name) as IMyLightingBlock;
        if (light != null)
        {
            padsLights.Add(light);
        }
    }

    if (padsLights.Count == 0)
    {
        Echo("Error: No landing pad lights found!");
        return;
    }

    // Determine lock state and apply updates
    if (connector.Status == MyShipConnectorStatus.Connected)
    {
        // LOCKED STATE -> Solid Green
        foreach (var light in padsLights)
        {
            light.Color = COLOR_LOCKED;
            light.BlinkIntervalSeconds = 0f; // Solid on, no blinking
            light.BlinkLength = 0f;
        }
        Echo("[PAD STATUS] SECURED\nLights: GREEN");
    }
    else
    {
        // UNLOCKED STATE -> Red (Ready / Warning)
        foreach (var light in padsLights)
        {
            light.Color = COLOR_UNLOCKED;
            
            // OPTIONAL: If you want them to blink when unlocked, 
            // uncomment the two lines below by removing the '//'
            // light.BlinkIntervalSeconds = 1.0f; 
            // light.BlinkLength = 50f;
        }
        Echo("[PAD STATUS] OPEN / UNLOCKED\nLights: RED");
    }
}