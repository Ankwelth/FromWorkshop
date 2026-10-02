// C2's Natural Lighting
// ===================
// Version: 1.0.0
// Date: 2021-01-04

/*

Run this script with a temperature tag in the Custom Data of any light blocks on a grid and it will calculate and set
the appropriate color values for that color temperature.

Examples:
T:5500K
T:Coolwhite

The tag can also be added to a group name in brackets.

Examples:
Hangar Lights [T:Highpressuresodium]
Main Hallway Lights [T:5700K]

*/

static string TEMP_TAG = "T:"; //Customize the tag prefix here

static Dictionary<string, double> TEMPERATURE_PRESETS = new Dictionary<string, double>()
{
    {"Lowpressuresodium", 1700.0},
    {"Candlelight", 1850.0},
    {"Highpressuresodium", 2200.0},
    {"Incandescent", 2400.0},
    {"Sunrise", 2500.0},
    {"Softwhite", 2700.0},
    {"Warmwhite", 3000.0},
    {"Studio1", 3200.0},
    {"Studio2", 3350.0},
    {"Coolwhite", 5000.0},
    {"WarmDaylight", 5500.0},
    {"CoolDaylight", 6000.0},
    {"Xenon", 6200.0},
    {"Overcast", 6500.0},
    {"Shade", 7500.0},
    {"Bluesky", 10000.0}
};

string group_has_tag(string group_name)
{
    foreach(var p in group_name.Split(' '))
    {
        var part = p;
        if (part.StartsWith("[") && part.EndsWith("]"))
        {
            part = part.Remove(0, 1);
            part = part.Remove(part.Length - 1, 1);
            if (part.StartsWith(TEMP_TAG))
            {
                return part;
            }
        }
    }
    return null;
}

string parse_temprature_tag(string custom_data)
{
    string tag = null;
    foreach (var line in custom_data.Split('\n'))
    {
        if (line.StartsWith(TEMP_TAG)){
            tag = line;
            int index = tag.IndexOf(TEMP_TAG);
            tag = (index < 0) ? tag : tag.Remove(index, TEMP_TAG.Length);
        }
    }
    return tag;
}

double clamp(double input, double min, double max)
{
    input = Math.Min(input, max);
    input = Math.Max(input, min);

    return input;
}

Color temperature_string_to_color(string temperature)
{
    double kelvin = 6500.0;
    double r = 255.0;
    double g = 255.0;
    double b = 255.0;

    if (temperature.EndsWith("K"))
    {
        double.TryParse(temperature.Remove(temperature.Length - 1, 1), out kelvin);
    }
    else if (TEMPERATURE_PRESETS.ContainsKey(temperature))
    {
        kelvin = TEMPERATURE_PRESETS[temperature];
    }
    else
    {
        Echo("Invalid temperature " + temperature + ", defaulting to 6500K");
    }

    kelvin = kelvin / 100.0;

    if (kelvin <= 66.0)
    {
        r = 255.0;
        g = kelvin;
        g = 99.4708025861 * Math.Log(g) - 161.1195681661;
        g = clamp(g, 0.0, 255.0);
        if (kelvin <= 19.0)
        {
            b = 0.0;
        }
        else
        {
            b = kelvin - 10.0;
            b = 138.5177312231 * Math.Log(b) - 305.0447927307;
            b = clamp(b, 0.0, 255.0);
        }
    }
    else
    {
        r = kelvin - 60.0;
        r = 329.698727446 * (Math.Pow(r, -0.1332047592));
        r = clamp(r, 0.0, 255.0);
        g = kelvin - 60.0;
        g = 288.1221685283 * (Math.Pow(g, -0.0755148492));
        g = clamp(g, 0.0, 255.0);
        b = 255.0;
    }
    Color output = new Color((int)Math.Round(r), (int)Math.Round(g), (int)Math.Round(b));

    // Echo("R: " + output.R + " G: " + output.G + " B: " + output.B);

    return output;
}

void Main(string args)
{
	var all_lights = new List<IMyLightingBlock>();
    var groups = new List<IMyBlockGroup>();

    GridTerminalSystem.GetBlockGroups(groups);
	GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(all_lights);

	int totalLights = 0;

    foreach (var group in groups){
        var tag = group_has_tag(group.Name);
        if (tag != null)
        {
            tag = parse_temprature_tag(tag);
            var group_lights = new List<IMyLightingBlock>();
            group.GetBlocksOfType<IMyLightingBlock>(group_lights);

            Echo(group.Name);
            foreach (var light in group_lights)
            {
                Echo(light.CustomName + ": " + tag);
                light.Color = temperature_string_to_color(tag);
                totalLights++;
            }
        }
    }

	foreach (var light in all_lights) {
        var tag = parse_temprature_tag(light.CustomData);
        if (tag != null)
        {
            Echo(light.CustomName + ": " + tag);
            light.Color = temperature_string_to_color(tag);
            totalLights++;
        }
	}

	if (totalLights != 0) Echo("Found: " + totalLights);
}
