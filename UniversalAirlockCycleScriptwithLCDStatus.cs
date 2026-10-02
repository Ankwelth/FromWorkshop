// Universal Airlock Cycle Handler with Status Display
// Author: sashiro

//=====================
// === CONFIGURATION ===

const string SAFE_DOOR_NAME = "Airlock Inner Door"; // Enabled when pressurized, disabled otherwise
const string EXPOSED_DOOR_NAME = "Airlock Outer Door"; // Enabled when depressurized, disabled otherwise
const string VENT_NAME = "Airlock Vent";
const string LCD_NAME = "Airlock LCD"; // Optional LCD
const string LIGHT1_NAME = "Airlock Light 1"; // Optional (either general or door light)
const string LIGHT2_NAME = "Airlock Light 2"; // Optional: Used only if two door lights exist (LIGHT1 for SAFE_DOOR, LIGHT2 for EXPOSED_DOOR)
const double DELAY_SECONDS = 3.0; // Artificial delay (in seconds) added before/after pressure cycle to extend airlock duration

// === END CONFIGURATION ===
//=====================

IMyDoor safeDoor;
IMyDoor exposedDoor;
IMyAirVent vent;
IMyTextPanel lcd;
IMyInteriorLight light1;
IMyInteriorLight light2;

bool pressurizing = false;

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.None;
}

void Init() {
    safeDoor = GridTerminalSystem.GetBlockWithName(SAFE_DOOR_NAME) as IMyDoor;
    exposedDoor = GridTerminalSystem.GetBlockWithName(EXPOSED_DOOR_NAME) as IMyDoor;
    vent = GridTerminalSystem.GetBlockWithName(VENT_NAME) as IMyAirVent;
    lcd = GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel;
    light1 = GridTerminalSystem.GetBlockWithName(LIGHT1_NAME) as IMyInteriorLight;
    light2 = GridTerminalSystem.GetBlockWithName(LIGHT2_NAME) as IMyInteriorLight;

    if (safeDoor == null || exposedDoor == null || vent == null)
        throw new Exception("Missing critical airlock component.");

    if (lcd != null) {
        lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    }
}

public void Main(string argument, UpdateType updateSource) {
    Init();

    if ((updateSource & UpdateType.Trigger) != 0) {
        safeDoor.OpenDoor(); exposedDoor.OpenDoor();
        safeDoor.CloseDoor(); exposedDoor.CloseDoor();

        Storage = "sealing";
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
        UpdateStatus("Closing doors...");
        return;
    }

    if ((updateSource & UpdateType.Update10) == 0) return;

    float pressure = vent.GetOxygenLevel();

    if (Storage == "sealing") {
        if (safeDoor.Status == DoorStatus.Closed && exposedDoor.Status == DoorStatus.Closed) {
            safeDoor.Enabled = false;
            exposedDoor.Enabled = false;

            // Start delay before pressure cycle
            long now = DateTime.Now.Ticks;
            Storage = "delay1|" + now;
            UpdateStatus("Doors sealed. Waiting...");
        } else {
            UpdateStatus("Waiting for doors to close...");
        }
        return;
    }

    // First delay after door close
    if (Storage.StartsWith("delay1|")) {
        long start = long.Parse(Storage.Split('|')[1]);
        if ((DateTime.Now.Ticks - start) / TimeSpan.TicksPerSecond >= DELAY_SECONDS) {
            pressurizing = pressure < 0.5f;
            vent.Depressurize = !pressurizing;

            Storage = pressurizing ? "pressurizing" : "depressurizing";
            UpdateStatus($"{(pressurizing ? "Pressurizing" : "Depressurizing")}...\n[----------] 0%");
        } else {
            UpdateStatus("Stabilizing airlock...");
        }
        return;
    }

    if (Storage == "pressurizing" || Storage == "depressurizing") {
        bool done = false;

        if (pressurizing && pressure >= 0.95f)
            done = true;
        else if (!pressurizing && pressure <= 0.05f)
            done = true;

        int barLength = 10;
        int filled = (int)Math.Round(pressure * barLength);
        string bar = new string('#', filled) + new string('-', barLength - filled);
        string percent = $"{(pressure * 100):0}%";

        UpdateStatus($"{(pressurizing ? "Pressurizing" : "Depressurizing")}...\n[{bar}] {percent}");

        if (done) {
            // Start post-cycle delay
            long now = DateTime.Now.Ticks;
            Storage = "delay2|" + now + "|" + (pressurizing ? "pressurized" : "depressurized");
            UpdateStatus("Stabilizing chamber...");
        }
        return;
    }

    // Final delay before re-enabling doors
    if (Storage.StartsWith("delay2|")) {
        var parts = Storage.Split('|');
        long start = long.Parse(parts[1]);
        bool isPressurized = parts[2] == "pressurized";

        if ((DateTime.Now.Ticks - start) / TimeSpan.TicksPerSecond >= DELAY_SECONDS) {
            if (isPressurized) {
                safeDoor.Enabled = true;
                exposedDoor.Enabled = false;
            } else {
                exposedDoor.Enabled = true;
                safeDoor.Enabled = false;
            }

            UpdateStatus($"Atmospheric cycle complete.\nStatus: {(isPressurized ? "Pressurized" : "Depressurized")}");
            Storage = "";
            Runtime.UpdateFrequency = UpdateFrequency.None;
        } else {
            UpdateStatus("Finalizing pressure...");
        }
    }
}

// Update LCD and light(s)
void UpdateStatus(string message) {
    Echo(message);

    if (lcd != null) {
        lcd.WriteText(message, false);
    }

    // If only one light is configured, use state-based coloring
    if (light1 != null && light2 == null) {
        Color color = Color.Black;

        if (message.Contains("Pressurized"))
            color = Color.Green;
        else if (message.Contains("Depressurized"))
            color = Color.Red;
        else
            color = Color.Red; // Default to red during transitions

        light1.Color = color;
    }

    // If two lights are configured, assign red/green depending on door states
    if (light1 != null && light2 != null) {
        bool safeOpen = safeDoor.Enabled;
        bool exposedOpen = exposedDoor.Enabled;

        light1.Color = safeOpen ? Color.Green : Color.Red;
        light2.Color = exposedOpen ? Color.Green : Color.Red;
    }
}
