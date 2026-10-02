// ======================
// Hangar Auto Airlock + PB Face LCD Indicator (Planet Depressurization Ignore)
// - "toggle": if doors closed -> (maybe) depressurize -> open; if open -> close -> pressurize
// - LCD: Green=pressurized, Yellow=transition, Red=depressurized
// - Planet mode: skips waiting for depressurization only (pressurization still active)
// - Toggle at runtime with args: planet_on / planet_off
// ======================

// === Config ===
const string DOOR_GROUP  = "BlueHangarDoors";
const string VENT_GROUP  = "BlueHangarVents";

// Runtime flag (NOT const to avoid unreachable-code warnings)
bool PlanetHasOxygen = true;

// Oxygen thresholds
const float PRESSURIZED_LEVEL    = 0.95f;
const float DEPRESSURIZED_LEVEL  = 0.02f;

// === State ===
enum HangarState { Idle, RequestOpen, Depressurizing, Opening, Open, RequestClose, Closing, Pressurizing }
HangarState _state = HangarState.Idle;

List<IMyDoor> _doors = new List<IMyDoor>();
List<IMyAirVent> _vents = new List<IMyAirVent>();

IMyTextSurface _lcd;

Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.None;

    GridTerminalSystem.GetBlockGroupWithName(DOOR_GROUP)?.GetBlocksOfType(_doors);
    GridTerminalSystem.GetBlockGroupWithName(VENT_GROUP)?.GetBlocksOfType(_vents);

    _lcd = Me.GetSurface(0);
    _lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    _lcd.TextPadding = 0f;
    _lcd.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;

    Echo(_doors.Count == 0 ? $"[WARN] No doors in '{DOOR_GROUP}'." : "");
    Echo(_vents.Count == 0 ? $"[WARN] No vents in '{VENT_GROUP}'." : "");

    // Optional: read default from Custom Data
    // Put line: PlanetHasOxygen=true
    var cd = Me.CustomData;
    if (!string.IsNullOrWhiteSpace(cd))
    {
        foreach (var line in cd.Split('\n'))
        {
            var t = line.Trim();
            if (t.StartsWith("PlanetHasOxygen=", StringComparison.OrdinalIgnoreCase))
            {
                var rhs = t.Substring("PlanetHasOxygen=".Length).Trim();
                bool parsed;
                if (bool.TryParse(rhs, out parsed)) PlanetHasOxygen = parsed;
            }
        }
    }
}

public void Save() {}

public void Main(string argument, UpdateType updateSource)
{
    // Runtime toggles
    if (!string.IsNullOrWhiteSpace(argument))
    {
        if (argument.Equals("toggle", StringComparison.OrdinalIgnoreCase)) HandleToggle();
        else if (argument.Equals("open", StringComparison.OrdinalIgnoreCase)) RequestOpen();
        else if (argument.Equals("close", StringComparison.OrdinalIgnoreCase)) RequestClose();
        else if (argument.Equals("planet_on", StringComparison.OrdinalIgnoreCase)) PlanetHasOxygen = true;
        else if (argument.Equals("planet_off", StringComparison.OrdinalIgnoreCase)) PlanetHasOxygen = false;
    }

    switch (_state)
    {
        case HangarState.Idle:
            Runtime.UpdateFrequency = UpdateFrequency.None;
            break;

        case HangarState.RequestOpen:
            if (AreDoorsOpen())
            {
                _state = HangarState.Open;
                break;
            }
            if (IsPressurized() && !PlanetHasOxygen)
            {
                SetVentsDepressurize(true);
                _state = HangarState.Depressurizing;
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            else
            {
                _state = HangarState.Opening;
                OpenDoors();
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            break;

        case HangarState.Depressurizing:
            if (IsDepressurized() || PlanetHasOxygen)
            {
                _state = HangarState.Opening;
                OpenDoors();
            }
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            break;

        case HangarState.Opening:
            if (AreDoorsOpen())
            {
                _state = HangarState.Open;
                SetVentsDepressurize(true); // keep vents dumping while open
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
            else
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            break;

        case HangarState.Open:
            Runtime.UpdateFrequency = UpdateFrequency.None;
            break;

        case HangarState.RequestClose:
            if (AreDoorsClosed())
            {
                _state = HangarState.Pressurizing;
                SetVentsDepressurize(false);
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            else
            {
                _state = HangarState.Closing;
                CloseDoors();
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            break;

        case HangarState.Closing:
            if (AreDoorsClosed())
            {
                _state = HangarState.Pressurizing;
                SetVentsDepressurize(false);
            }
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            break;

        case HangarState.Pressurizing:
            if (IsPressurized())
            {
                _state = HangarState.Idle;
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
            else
            {
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
            }
            break;
    }

    EchoStatus();
    UpdateLCD();
}

// ===== Commands =====
void HandleToggle()
{
    if (AreDoorsOpen()) RequestClose();
    else                RequestOpen();
}
void RequestOpen()
{
    _state = HangarState.RequestOpen;
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
void RequestClose()
{
    _state = HangarState.RequestClose;
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

// ===== Helpers =====
bool AreDoorsClosed()
{
    if (_doors.Count == 0) return false;
    foreach (var d in _doors)
        if (d.Status != DoorStatus.Closed && d.OpenRatio > 0f) return false;
    return true;
}

bool AreDoorsOpen()
{
    if (_doors.Count == 0) return false;
    foreach (var d in _doors)
        if (d.OpenRatio < 0.99f && d.Status != DoorStatus.Open) return false;
    return true;
}

void OpenDoors()
{
    foreach (var d in _doors) d.OpenDoor();
}

void CloseDoors()
{
    foreach (var d in _doors) d.CloseDoor();
}

void SetVentsDepressurize(bool on)
{
    // In planet mode we still allow this call, but we won't *wait* on O2 to drop.
    foreach (var v in _vents)
        if (v.IsFunctional) v.Depressurize = on;
}

bool IsPressurized()
{
    if (_vents.Count == 0) return false;
    foreach (var v in _vents)
    {
        if (!v.IsFunctional) return false;
        if (!v.CanPressurize) return false;
        if (v.GetOxygenLevel() < PRESSURIZED_LEVEL) return false;
    }
    return true;
}

bool IsDepressurized()
{
    if (_vents.Count == 0) return true;
    foreach (var v in _vents)
    {
        if (!v.IsFunctional) return false;
        if (v.GetOxygenLevel() > DEPRESSURIZED_LEVEL) return false;
    }
    return true;
}

void EchoStatus()
{
    Echo($"State:   {_state}");
    Echo($"Doors:   {(AreDoorsOpen() ? "OPEN" : AreDoorsClosed() ? "CLOSED" : "MOVING")}");
    float minO2 = 1f, maxO2 = 0f;
    foreach (var v in _vents)
    {
        var o2 = v.GetOxygenLevel();
        if (o2 < minO2) minO2 = o2;
        if (o2 > maxO2) maxO2 = o2;
    }
    Echo($"O2:      {minO2:0.00} – {maxO2:0.00}");
    if (_vents.Count > 0) Echo($"Vents:   {(_vents[0].Depressurize ? "DEPRESSURIZING" : "PRESSURIZING/HOLD")}");
    Echo($"PlanetHasOxygen: {PlanetHasOxygen}");
}

// ===== PB Face LCD (Solid Color Panel) =====
void UpdateLCD()
{
    if (_lcd == null) return;

    _lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    _lcd.TextPadding = 0f;
    _lcd.FontSize = 10f;
    _lcd.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
    _lcd.Font = "Monospace";
    _lcd.FontColor = new VRageMath.Color(0,0,0);

    bool isTransition =
        _state == HangarState.Depressurizing ||
        _state == HangarState.Opening ||
        _state == HangarState.Closing ||
        _state == HangarState.Pressurizing;

    VRageMath.Color bg =
        isTransition                 ? new VRageMath.Color(255, 255, 0) : // Yellow
        IsPressurized()              ? new VRageMath.Color(0, 255, 0)   : // Green
        IsDepressurized()            ? new VRageMath.Color(255, 0, 0)   : // Red
                                       new VRageMath.Color(255, 255, 0);  // Fallback

    _lcd.BackgroundColor = bg;
    _lcd.WriteText("");
}
