#region ReactorManager

// Version
const string VERSION = "1.2.1";

// Patch Notes
// 1.2.1 - Fix surface settings reapplied every tick; fix false alert on first run
// 1.2.0 - Support any text surface (cockpits, seats, wide LCDs, etc.); fix LCD subtype filter
// 1.1.3 - Low fuel alert only when reactors on or battery low; check all grid inventories
// 1.1.2 - Fix reactor status check to consider all reactors, not just the first
// 1.1.1 - Reuse inventory list instead of allocating per reactor per tick
// 1.1.0 - Refactored alert light state to single struct; fixed save/restore for lights added/removed from group mid-alert
// 1.0.0 - Initial release

// Settings
string lcdName = "[POWER]";                    // LCDs must contain this tag
string lightGroupName = "Power Alert Lights";  // Group name for alert lights
float BTOn = 50f;                              // Reactors ON ≤ this %
float BTOff = 95f;                             // Reactors OFF ≥ this %
float lowUraniumThreshold = 20f;               // Yellow if uranium < this
float blinkDuration = 10f;                     // Blink time in seconds
// End Settings

List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
List<IMyReactor> reactors = new List<IMyReactor>();
List<IMyTextSurfaceProvider> displayBlocks = new List<IMyTextSurfaceProvider>();
List<IMyTextSurface> surfaces = new List<IMyTextSurface>();
List<IMyLightingBlock> alertLights = new List<IMyLightingBlock>();
List<IMyTerminalBlock> inventoryBlocks = new List<IMyTerminalBlock>();
static readonly MyItemType uraniumType = MyItemType.MakeIngot("Uranium");

struct LightState {
public IMyLightingBlock Block;
public Color Color;
public float Intensity;
public float Radius;
public float BlinkIntervalSeconds;
public float BlinkLength;
public float BlinkOffset;
public bool Enabled;
}

Dictionary<long, LightState> preAlertStates = new Dictionary<long, LightState>();

// Alert state tracking
bool prevReactorsOn = false;
bool prevInAlert = false;
float blinkTimer = 0f;
bool isRedBlink = false;
const float tickTime = 1f / 6f;  // Update10 ≈ 6 ticks/sec
bool firstRun = true;

public Program()
{
Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
// Grab blocks
GridTerminalSystem.GetBlocksOfType(batteries, b => b.CubeGrid == Me.CubeGrid);
GridTerminalSystem.GetBlocksOfType(reactors, b => b.CubeGrid == Me.CubeGrid);
string lcdTag = lcdName.TrimEnd(']');
GridTerminalSystem.GetBlocksOfType(displayBlocks, b => ((IMyTerminalBlock)b).CustomName.Contains(lcdTag));
surfaces.Clear();
foreach (var block in displayBlocks)
{
    string name = ((IMyTerminalBlock)block).CustomName;
    int tagStart = name.IndexOf(lcdTag);
    int afterTag = tagStart + lcdTag.Length;
    int surfaceIndex = 0;
    if (afterTag < name.Length && name[afterTag] == ':')
    {
        int closeBracket = name.IndexOf(']', afterTag);
        if (closeBracket > afterTag + 1)
            int.TryParse(name.Substring(afterTag + 1, closeBracket - afterTag - 1), out surfaceIndex);
    }
    if (surfaceIndex >= 0 && surfaceIndex < block.SurfaceCount)
        surfaces.Add(block.GetSurface(surfaceIndex));
}

// Battery calculations
float maxStored = 0f;
float currentStored = 0f;
float currentOutput = 0f;
float currentInput = 0f;
bool anyBatteryEnabled = false;

foreach (var bat in batteries)
{
    maxStored += bat.MaxStoredPower;
    currentStored += bat.CurrentStoredPower;
    currentOutput += bat.CurrentOutput;
    currentInput += bat.CurrentInput;
    if (bat.Enabled) anyBatteryEnabled = true;
}

float batteryPercent = maxStored > 0 ? currentStored / maxStored * 100f : 0f;

// Runtime estimate
string timeRemaining = "N/A";
float netPower = currentOutput - currentInput;
if (maxStored > 0 && netPower > 0.01f)
{
    float hoursLeft = currentStored / netPower;
    timeRemaining = hoursLeft >= 1 ? $"{hoursLeft:0.0} h" : $"{hoursLeft * 60:0} min";
}
else if (currentInput > currentOutput + 0.01f)
{
    timeRemaining = "Charging";
}

// Uranium on grid
GridTerminalSystem.GetBlocksOfType(inventoryBlocks, b => b.CubeGrid == Me.CubeGrid && b.HasInventory);
double totalUranium = 0;
foreach (var block in inventoryBlocks)
{
    for (int i = 0; i < block.InventoryCount; i++)
        totalUranium += (double)block.GetInventory(i).GetItemAmount(uraniumType);
}
inventoryBlocks.Clear();

// Reactor control
bool forceOn = (batteries.Count == 0 || !anyBatteryEnabled);
bool shouldTurnOn = (batteryPercent <= BTOn) && totalUranium > 0;
bool shouldTurnOff = (batteryPercent >= BTOff) || totalUranium == 0;

foreach (var r in reactors)
{
    if (forceOn) r.Enabled = true;
    else if (shouldTurnOn) r.Enabled = true;
    else if (shouldTurnOff) r.Enabled = false;
}

bool reactorsOn = reactors.Count > 0 && reactors.Any(r => r.Enabled);
bool lowFuel = totalUranium < lowUraniumThreshold && (reactorsOn || batteryPercent <= BTOn);

// Light group
IMyBlockGroup lightGroup = GridTerminalSystem.GetBlockGroupWithName(lightGroupName);
alertLights.Clear();
if (lightGroup != null) lightGroup.GetBlocksOfType(alertLights);

// Reactor flip detection (skip first tick to avoid false alert on startup)
if (!firstRun && reactorsOn != prevReactorsOn)
{
    isRedBlink = reactorsOn;
    blinkTimer = blinkDuration;
}
if (blinkTimer > 0)
{
    blinkTimer -= tickTime;
    if (blinkTimer <= 0) blinkTimer = 0;
}

bool inAlert = lowFuel || blinkTimer > 0;
bool alertJustStarted = inAlert && !prevInAlert;

// === ALERT START: Save current state and force lights on for alert ===
if (alertJustStarted)
{
    preAlertStates.Clear();

    foreach (var light in alertLights)
    {
        preAlertStates[light.EntityId] = new LightState {
            Block = light,
            Color = light.Color,
            Intensity = light.Intensity,
            Radius = light.Radius,
            BlinkIntervalSeconds = light.BlinkIntervalSeconds,
            BlinkLength = light.BlinkLength,
            BlinkOffset = light.BlinkOffset,
            Enabled = light.Enabled
        };

        light.Enabled = true;
    }
}

// === DURING ALERT: Apply visual changes only (do NOT touch Enabled) ===
if (inAlert)
{
    foreach (var light in alertLights)
    {
        // Late save: light added to group mid-alert
        if (!preAlertStates.ContainsKey(light.EntityId))
        {
            preAlertStates[light.EntityId] = new LightState {
                Block = light,
                Color = light.Color,
                Intensity = light.Intensity,
                Radius = light.Radius,
                BlinkIntervalSeconds = light.BlinkIntervalSeconds,
                BlinkLength = light.BlinkLength,
                BlinkOffset = light.BlinkOffset,
                Enabled = light.Enabled
            };
            light.Enabled = true;
        }

        light.BlinkIntervalSeconds = 0;  // Disable built-in blink

        if (lowFuel)
        {
            light.Color = new VRageMath.Color(255, 255, 0);  // Yellow
        }
        else
        {
            // Red/Green blink
            int ticksElapsed = (int)((blinkDuration - blinkTimer) / tickTime);
            bool onPhase = (ticksElapsed / 3) % 2 == 0;
            light.Color = onPhase
                ? (isRedBlink ? new VRageMath.Color(255, 0, 0) : new VRageMath.Color(0, 255, 0))
                : new VRageMath.Color(0, 0, 0);
        }
    }
}
// === ALERT ENDED: Restore exact pre-alert state ===
else if (prevInAlert && !inAlert && preAlertStates.Count > 0)
{
    foreach (var state in preAlertStates.Values)
    {
        state.Block.Color = state.Color;
        state.Block.Intensity = state.Intensity;
        state.Block.Radius = state.Radius;
        state.Block.BlinkIntervalSeconds = state.BlinkIntervalSeconds;
        state.Block.BlinkLength = state.BlinkLength;
        state.Block.BlinkOffset = state.BlinkOffset;
        state.Block.Enabled = state.Enabled;
    }
    preAlertStates.Clear();
}
// === NO ALERT ACTIVE: Do nothing to lights (full player control) ===

prevInAlert = inAlert;
prevReactorsOn = reactorsOn;

// Display
string reactorStatus = reactorsOn ? "On" : "Off";
string displayText =
$"=== Power Management v{VERSION} ===\n" +
$"Batteries: {batteries.Count}\n" +
$"Reactors : {reactors.Count}\n" +
$"Reactor  : {reactorStatus}\n" +
$"Stored   : {batteryPercent:0.0}%\n" +
$"Runtime  : {timeRemaining}\n" +
$"Uranium  : {totalUranium:0.##} ingots";

if (lowFuel) displayText += "\n>>> FUEL LOW! <<<";

var pbSurface = Me.GetSurface(0);
if (firstRun)
{
    pbSurface.ContentType = ContentType.TEXT_AND_IMAGE;
    pbSurface.FontSize = 1f;
    pbSurface.TextPadding = 5f;
    pbSurface.Alignment = TextAlignment.CENTER;
}
pbSurface.WriteText(displayText);

Echo(displayText);

foreach (var surface in surfaces)
{
    if (firstRun)
    {
        surface.ContentType = ContentType.TEXT_AND_IMAGE;
        surface.FontSize = 1.2f;
    }
    surface.WriteText(displayText);
}

firstRun = false;

// Cleanup
batteries.Clear();
reactors.Clear();
displayBlocks.Clear();
surfaces.Clear();
alertLights.Clear();
}

#endregion // ReactorManager
