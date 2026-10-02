// === CONFIGURATION ===
const string LcdName = "HyperLCD";

// === STATE ===
int planetSelectionIndex = 0;
bool selectingFrom = true;
string selectedFrom = null;
string selectedTo = null;

IMyTextPanel lcd;

List<string> planetList = new List<string> {
    "Korriban", "Tatooine", "Naboo", "Crait", "Bespin", "Ilum",
    "Geonosis", "Mandalore", "Tython", "Mustafar", "Jakku", "Bastion",
    "Pyke", "Corellia", "Coruscant"
};


public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.None;
    lcd = GridTerminalSystem.GetBlockWithName(LcdName) as IMyTextPanel;
    lcd?.WriteText("Use arguments: UP, DOWN, ENTER\n");
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument == "UP" || argument == "DOWN" || argument == "ENTER")
        HandleInput(argument);
    else
        UpdateUI();
}

void HandleInput(string arg)
{
    if (arg == "UP")
    {
        planetSelectionIndex = (planetSelectionIndex - 1 + planetList.Count) % planetList.Count;
    }
    else if (arg == "DOWN")
    {
        planetSelectionIndex = (planetSelectionIndex + 1) % planetList.Count;
    }
    else if (arg == "ENTER")
    {
        if (selectingFrom)
        {
            selectedFrom = planetList[planetSelectionIndex];
            selectingFrom = false;
            planetSelectionIndex = 0;
        }
        else
        {
            selectedTo = planetList[planetSelectionIndex];
            DisplayRoute(GetRouteSteps(selectedFrom, selectedTo));
            selectingFrom = true;
            selectedFrom = null;
            selectedTo = null;
            planetSelectionIndex = 0;
            return;
        }
    }

    UpdateUI();
}

void UpdateUI()
{
    var sb = new StringBuilder();

    if (selectingFrom)
        sb.AppendLine("Select current planet:");
    else
        sb.AppendLine("Select destination planet:");

    for (int i = 0; i < planetList.Count; i++)
    {
        if (i == planetSelectionIndex)
            sb.Append("> ");
        else
            sb.Append("  ");
        sb.AppendLine(planetList[i]);
    }

    lcd.WriteText(sb.ToString());
}

void DisplayRoute(List<string> steps)
{
    var sb = new StringBuilder();
    sb.AppendLine("Calculated Route:\n");

    foreach (var step in steps)
    {
        sb.AppendLine(step);
    }

    lcd.WriteText(sb.ToString());
}
List<string> GetRouteSteps(string from, string to)
{
    var graph = new Dictionary<string, List<string>>();
    var gpsLookup = new Dictionary<string, string>();

    // Create the graph and gpsLookup dictionary from the RawGPS list
    foreach (var line in RawGPS)
    {
        var cleanLine = line.Trim();
        int start = cleanLine.IndexOf("GPS:") + 4;
        int end = cleanLine.IndexOf(":", start);
        if (start < 4 || end < 0) continue;

        string fullName = cleanLine.Substring(start, end - start);
        var parts = fullName.Split(new string[] { " - " }, StringSplitOptions.None);
        if (parts.Length != 2) continue;

        var p1 = parts[0].Trim();
        var p2 = parts[1].Trim();

        if (!graph.ContainsKey(p1)) graph[p1] = new List<string>();
        graph[p1].Add(p2);  // Only add the FROM -> TO direction

        string key = p1 + "->" + p2;
        if (!gpsLookup.ContainsKey(key)) gpsLookup[key] = cleanLine;
    }

    var queue = new Queue<List<string>>();
    var visited = new HashSet<string>();

    queue.Enqueue(new List<string> { from });
    visited.Add(from);

    // Perform a BFS (Breadth-First Search) to find the route
    while (queue.Count > 0)
    {
        var path = queue.Dequeue();
        var last = path[path.Count - 1];

        if (last == to)
        {
            var steps = new List<string>();
            // Retrieve the GPS coordinates for each step in the path
            for (int i = 0; i < path.Count - 1; i++)
            {
                string key = path[i] + "->" + path[i + 1];
                if (gpsLookup.ContainsKey(key))
                {
                    steps.Add(gpsLookup[key]);  // Add the GPS route to the steps list
                }
            }
            return steps;  // Return the list of GPS steps
        }

        foreach (var neighbor in graph[last])
        {
            if (!visited.Contains(neighbor))
            {
                visited.Add(neighbor);
                var newPath = new List<string>(path);
                newPath.Add(neighbor);
                queue.Enqueue(newPath);
            }
        }
    }

    return new List<string> { "No route found." };  // If no path found
}





// GPS ROUTES
List<string> RawGPS = new List<string> {
    "GPS:Geonosis - Bastion:358115.85:336831.98:2673998.39:#FF75C9F1:Bastion Bypass:",
    "GPS:Bastion - Geonosis:3508966.75:-1201581.74:1896224.06:#FF75C9F1:Bastion Bypass:",
    "GPS:Bastion - Crait:3508719.1:-1198626.04:1897598.07:#FF75C9F1:Bastion Bypass:",
    "GPS:Crait - Bastion:1491861.56:-2579564.88:278598.48:#FF75C9F1:Bastion Bypass:",
    "GPS:Coruscant - Corellia:-520095.85:-727089.58:319269.84:#FF75C9F1:Beskar Smuggleing Lane:",
    "GPS:Mandalore - Corellia:-835416.71:1399865.94:-953457.73:#FF75C9F1:Beskar Smuggleing Lane:",
    "GPS:Corellia - Mandalore:-899117.14:420909.2:482760.13:#FF75C9F1:Beskar Smuggleing Lane:",
    "GPS:Corellia - Coruscant:-898825.41:417857.56:481782.01:#FF75C9F1:Beskar Smuggleing Lane:",
    "GPS:Tython - Jakku:230887.75:-204709.07:74132.18:#FF75C9F1:Jedi Passage:",
    "GPS:Tython - Coruscant:229524.71:-206022.94:76598.21:#FF75C9F1:Jedi Passage:",
    "GPS:Coruscant - Tython:-502056.61:-765549.55:311520.83:#FF75C9F1:Jedi Passage:",
    "GPS:Coruscant Ilum:-504436.54:-767967.79:311437.21:#FF75C9F1:Jedi Passage:",
    "GPS:Ilum - Coruscant:-189912.5:-1594532.34:-1453673.16:#FF75C9F1:Jedi Passage:",
    "GPS:Jakku - Tython:874966.3:394471.15:-1021384.82:#FF75C9F1:Jedi Passage:",
    "GPS:Korriban - Mandalore:805503.28:1825407.63:1609854.72:#FF75C9F1:Korriban Run:",
    "GPS:Mandalore - Korriban:-784729.41:1432764.83:-931674.16:#FF75C9F1:Korriban Run:",
    "GPS:Mustafar - Bespin:1888427.07:-1803564.79:-4058927.9:#FF75C9F1:Main Way:",
    "GPS:Pyke - Mustafar:-574431.07:1595465.77:-2937275.03:#FF75C9F1:Main Way:",
    "GPS:Pyke Mandalore:-577214.77:1596344.71:-2935526.7:#FF75C9F1:Main Way:",
    "GPS:Mandalore - Pyke:-926329.81:1444528.25:-929547.7:#FF75C9F1:Main Way:",
    "GPS:Mandalore - Geonosis:-930075.12:1446243.46:-929963.29:#FF75C9F1:Main Way:",
    "GPS:Tatooine - Geonosis:-186621.84:-512009.81:2392753.36:#FF75C9F1:Main Way:",
    "GPS:Crait - Naboo:1469121.3:-2570558.12:292679.14:#FF75C9F1:Main Way:",
    "GPS:Geonosis - Tatooine:300109.84:333713.13:2626809.71:#FF75C9F1:Main Way:",
    "GPS:Geonosis - Mandalore:303808.94:335083.32:2626023.1:#FF75C9F1:Main Way:",
    "GPS:Naboo - Tatooine:997517.21:-2538519.3:1532538.54:#FF75C9F1:Main Way:",
    "GPS:Naboo - Crait:996232.11:-2540244.5:1530412.49:#FF75C9F1:Main Way:",
    "GPS:Mustafar - Pyke:1890602.87:-1802885.53:-4060924.41:#FF75C9F1:Main Way:",
    "GPS:Bespin - Mustafar:1949637.83:-2225698.16:-2092660.75:#FF75C9F1:Main Way:",
    "GPS:Bespin - Crait:1948398.07:-2225651.83:-2089913.87:#FF75C9F1:Main Way:",
    "GPS:Crait - Bespin:1470075.22:-2571397.47:289925.39:#FF75C9F1:Main Way:",
    "GPS:Tatooine - Naboo:-184292.4:-513967.03:2392387.82:#FF75C9F1:Main Way:",
    "GPS:Coruscant - Naboo:-487169.03:-784911.75:335554.91:#FF75C9F1:Naboo Lane:",
    "GPS:Naboo - Coruscant:920327.84:-2526377.02:1523343.52:#FF75C9F1:Naboo Lane:"
};
