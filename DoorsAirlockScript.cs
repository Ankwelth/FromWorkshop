// Auto-close doors + simple airlock logic
// - Any door: auto-closes 3s after fully open
// - Ignore doors with [M] or [EX] in CustomName
// - Airlocks: doors tagged [A1], [A2], ... etc
//   -> Only one door in each [A#] group can be open/opening/closing at a time

const double AUTO_CLOSE_TIME = 0.5; // seconds

Dictionary<long, double> doorTimers = new Dictionary<long, double>();
List<IMyDoor> doors = new List<IMyDoor>();
Dictionary<string, List<IMyDoor>> airlockGroups = new Dictionary<string, List<IMyDoor>>();

public Program()
{
    // Run regularly
    Runtime.UpdateFrequency = UpdateFrequency.Update10; // ~6 times per second
}

public void Main(string argument, UpdateType updateSource)
{
    double dt = Runtime.TimeSinceLastRun.TotalSeconds;
    if (dt <= 0) dt = 0.1667; // fallback for first run / lag spikes

    doors.Clear();
    airlockGroups.Clear();

    // Get all doors on this grid, except those manually excluded
    GridTerminalSystem.GetBlocksOfType(doors, d =>
        d.CubeGrid == Me.CubeGrid &&
        !d.CustomName.Contains("[M]") &&
        !d.CustomName.Contains("[EX]")
    );

    // Build airlock groups based on [A<number>] tags
    foreach (var door in doors)
    {
        string tag = GetAirlockTag(door.CustomName);
        if (tag == null)
            continue;

        List<IMyDoor> list;
        if (!airlockGroups.TryGetValue(tag, out list))
        {
            list = new List<IMyDoor>();
            airlockGroups[tag] = list;
        }
        list.Add(door);
    }

    // Auto-close logic for all included doors
    foreach (var door in doors)
    {
        long id = door.EntityId;
        double timeLeft;

        if (!doorTimers.TryGetValue(id, out timeLeft))
            timeLeft = -1; // -1 = no active timer

        var status = door.Status;

        // If fully closed, reset timer
        if (status == DoorStatus.Closed)
        {
            timeLeft = -1;
        }
        // If fully open and no timer yet, start one
        else if (status == DoorStatus.Open && timeLeft < 0)
        {
            timeLeft = AUTO_CLOSE_TIME;
        }

        // Count down if timer active
        if (timeLeft >= 0)
        {
            timeLeft -= dt;

            if (timeLeft <= 0)
            {
                // Time's up: close if still open/opening
                if (status == DoorStatus.Open || status == DoorStatus.Opening)
                    door.ApplyAction("Open_Off");

                timeLeft = -1;
            }
        }

        doorTimers[id] = timeLeft;
    }

    // Airlock mutual-exclusion logic
    foreach (var kvp in airlockGroups)
    {
        var group = kvp.Value;
        if (group.Count < 2)
            continue; // need at least 2 doors to act as an airlock

        IMyDoor primary = null;

        // Choose the first door that is not fully closed as the "active" door
        for (int i = 0; i < group.Count; i++)
        {
            var d = group[i];
            if (d.Status != DoorStatus.Closed)
            {
                primary = d;
                break;
            }
        }

        if (primary == null)
        {
            // All doors in this airlock are closed -> enable all
            for (int i = 0; i < group.Count; i++)
            {
                var d = group[i];
                if (!d.Enabled)
                    d.Enabled = true;
            }
        }
        else
        {
            // One door is active -> keep it enabled, disable the others
            for (int i = 0; i < group.Count; i++)
            {
                var d = group[i];

                if (d == primary)
                {
                    if (!d.Enabled)
                        d.Enabled = true;
                }
                else
                {
                    // Make sure "other" doors are closed & disabled
                    if (d.Status == DoorStatus.Open || d.Status == DoorStatus.Opening)
                        d.ApplyAction("Open_Off");

                    if (d.Enabled)
                        d.Enabled = false;
                }
            }
        }
    }
}

// Finds a tag of the form [A<number>] in the name, e.g. [A1], [A2], ...
string GetAirlockTag(string name)
{
    for (int i = 0; i < name.Length; i++)
    {
        if (name[i] == '[' && i + 2 < name.Length &&
            (name[i + 1] == 'A' || name[i + 1] == 'a'))
        {
            int j = i + 2;
            bool hasDigit = false;

            // Read digits
            while (j < name.Length && char.IsDigit(name[j]))
            {
                hasDigit = true;
                j++;
            }

            // Expect a closing ]
            if (!hasDigit || j >= name.Length || name[j] != ']')
                continue;

            // Return the whole token, e.g. "[A1]"
            return name.Substring(i, j - i + 1);
        }
    }

    return null;
}
