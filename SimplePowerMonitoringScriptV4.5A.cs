// R e a d m e
// -----------
// 
// Power monitoring script V4.5A
// 
// Hope this finds some use. 
// 
// Have fun!
// 
// Adomus
// 
// 
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    ManageFirstLoad(Me);
}

// rename tags here
string h_gen_tag = "MBGenR";
string t_gen_tag = "MBGenR";
string r_gen_tag = "MBGenR";
string solar_panel_tag = "Mars Base";
string battery_tag = "Mars Base";
string power_tag = "Power";
string rotor_tag = "Mars Base";

// display settings
string lcd_display_name = "[Mars Base Power D1]";
int lcd_display_index = 0;
string lcd_display_name_2 = "[Mars Base Power D2]";
int lcd_display_index_2 = 0;
string lcd_display_name_3 = "[Mars Base Power D3]";
int lcd_display_index_3 = 0;

// item presence
bool battery = true;
bool solar = true;
bool wind = true;
bool hydro = false;
bool reactor = false;
bool rotors = false;

// power variables
float stored_power_total;
float max_power_total;
float current_power_total;
float in_current_power_total;

float total_power_a;
float total_power_gen;
float total_power_gen_w;
float total_power_gen_r;

List<IMyBatteryBlock> batteries_tag = new List<IMyBatteryBlock>();
List<IMySolarPanel> spanels_a = new List<IMySolarPanel>();
List<IMyPowerProducer> generators_tag_h = new List<IMyPowerProducer>();
List<IMyPowerProducer> generators_tag_t = new List<IMyPowerProducer>();
List<IMyPowerProducer> generators_tag_r = new List<IMyPowerProducer>();

List<IMyTextSurface> myTextSurfaces_d1 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d2 = new List<IMyTextSurface>();
List<IMyTextSurface> myTextSurfaces_d3 = new List<IMyTextSurface>();

StringBuilder sbtext = new StringBuilder();
StringBuilder sbtext_2nd = new StringBuilder();
string version = "V4.5";

float battery_medium = 1.0f;
bool setup_complete = false;
int state = 0;
IEnumerator<bool> listCoroutine;
bool listgenerator_finished = true;

double percent_list_generator_h = 0.0;
double percent_list_solar = 0.0;
double percent_list_generator_w = 0.0;
double percent_list_generator_r = 0.0;
double percent_list_battery = 0.0;
float percent_battery_power = 0f;

List<string> bargraph = new List<string> { ""};

MyIni _Ini = new MyIni();
StringBuilder sbtexttemp = new StringBuilder();
int runTick = 0;

void ManageFirstLoad(IMyTerminalBlock block)
{
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        WriteStorageData(block);
    }
    LoadStorageData(block.CustomData);
}

void LoadStorageData(string input)
{
    _Ini.Clear();
    if (_Ini.TryParse(input))
    {
        var str = "";
        str = _Ini.Get("PowerStatusConfig", "enablebattery").ToString();
        if (!bool.TryParse(str, out battery)) battery = true;

        str = _Ini.Get("PowerStatusConfig", "enablesolar").ToString();
        if (!bool.TryParse(str, out solar)) solar = true;

        str = _Ini.Get("PowerStatusConfig", "enablewind").ToString();
        if (!bool.TryParse(str, out wind)) wind = true;

        str = _Ini.Get("PowerStatusConfig", "enablehydro").ToString();
        if (!bool.TryParse(str, out hydro)) hydro = false;

        str = _Ini.Get("PowerStatusConfig", "enablereactor").ToString();
        if (!bool.TryParse(str, out reactor)) reactor = false;

        str = _Ini.Get("PowerStatusConfig", "enablerotor").ToString();
        if (!bool.TryParse(str, out rotors)) rotors = false;

        h_gen_tag = _Ini.Get("PowerStatusConfig", "hydrogentag").ToString(h_gen_tag);
        t_gen_tag = _Ini.Get("PowerStatusConfig", "turbinetag").ToString(t_gen_tag);
        r_gen_tag = _Ini.Get("PowerStatusConfig", "reactortag").ToString(r_gen_tag);
        solar_panel_tag = _Ini.Get("PowerStatusConfig", "solarpaneltag").ToString(solar_panel_tag);
        battery_tag = _Ini.Get("PowerStatusConfig", "batterytag").ToString(battery_tag);
        power_tag = _Ini.Get("PowerStatusConfig", "powertag").ToString(power_tag);
        rotor_tag = _Ini.Get("PowerStatusConfig", "rotortag").ToString(rotor_tag);

        lcd_display_name = _Ini.Get("PowerStatusConfig", "lcd1tag").ToString(lcd_display_name);
        str = _Ini.Get("PowerStatusConfig", "lcd1surfaceindex").ToString();
        if (!int.TryParse(str, out lcd_display_index)) lcd_display_index = 0;

        lcd_display_name_2 = _Ini.Get("PowerStatusConfig", "lcd2tag").ToString(lcd_display_name_2);
        str = _Ini.Get("PowerStatusConfig", "lcd2surfaceindex").ToString();
        if (!int.TryParse(str, out lcd_display_index_2)) lcd_display_index_2 = 0;

        lcd_display_name_3 = _Ini.Get("PowerStatusConfig", "lcd3tag").ToString(lcd_display_name_3);
        str = _Ini.Get("PowerStatusConfig", "lcd3surfaceindex").ToString();
        if (!int.TryParse(str, out lcd_display_index_3)) lcd_display_index_3 = 0;
    }
    _Ini.Clear();
    sbtexttemp.AppendLine("Data loaded from pb customdata");
}

void WriteStorageData(IMyTerminalBlock block)
{
    _Ini.Clear();
    _Ini.TryParse(block.CustomData); // Pre-load to preserve other data

    _Ini.Set("PowerStatusConfig", "enablebattery", battery);
    _Ini.Set("PowerStatusConfig", "enablesolar", solar);
    _Ini.Set("PowerStatusConfig", "enablewind", wind);
    _Ini.Set("PowerStatusConfig", "enablehydro", hydro);
    _Ini.Set("PowerStatusConfig", "enablereactor", reactor);
    _Ini.Set("PowerStatusConfig", "enablerotor", rotors);
    _Ini.Set("PowerStatusConfig", "hydrogentag", h_gen_tag);
    _Ini.Set("PowerStatusConfig", "turbinetag", t_gen_tag);
    _Ini.Set("PowerStatusConfig", "reactortag", r_gen_tag);
    _Ini.Set("PowerStatusConfig", "solarpaneltag", solar_panel_tag);
    _Ini.Set("PowerStatusConfig", "batterytag", battery_tag);
    _Ini.Set("PowerStatusConfig", "powertag", power_tag);
    _Ini.Set("PowerStatusConfig", "rotortag", rotor_tag);
    _Ini.Set("PowerStatusConfig", "lcd1tag", lcd_display_name);
    _Ini.Set("PowerStatusConfig", "lcd1surfaceindex", lcd_display_index);
    _Ini.Set("PowerStatusConfig", "lcd2tag", lcd_display_name_2);
    _Ini.Set("PowerStatusConfig", "lcd2surfaceindex", lcd_display_index_2);
    _Ini.Set("PowerStatusConfig", "lcd3tag", lcd_display_name_3);
    _Ini.Set("PowerStatusConfig", "lcd3surfaceindex", lcd_display_index_3);

    block.CustomData = _Ini.ToString();
    _Ini.Clear();
    Echo("Data saved to pb customdata");
}

void Setup_System()
{
    IMyGridTerminalSystem gts = GridTerminalSystem;
    sbtext.Clear();
    sbtext_2nd.Clear();
    myTextSurfaces_d1.Clear();
    myTextSurfaces_d2.Clear();
    myTextSurfaces_d3.Clear();
    batteries_tag.Clear();
    spanels_a.Clear();
    generators_tag_h.Clear();
    generators_tag_t.Clear();
    generators_tag_r.Clear();

    List<IMyTerminalBlock> display_all = new List<IMyTerminalBlock>();
    gts.GetBlocksOfType(display_all, b => b.IsSameConstructAs(Me));

    foreach (var disp in display_all)
    {
        if (disp.CustomName.Contains(lcd_display_name))
            myTextSurfaces_d1.Add(((IMyTextSurfaceProvider)disp).GetSurface(lcd_display_index));
        if (disp.CustomName.Contains(lcd_display_name_2))
            myTextSurfaces_d2.Add(((IMyTextSurfaceProvider)disp).GetSurface(lcd_display_index_2));
        if (disp.CustomName.Contains(lcd_display_name_3))
            myTextSurfaces_d3.Add(((IMyTextSurfaceProvider)disp).GetSurface(lcd_display_index_3));
    }

    if (battery)
    {
        List<IMyBatteryBlock> batteries_all = new List<IMyBatteryBlock>();
        gts.GetBlocksOfType(batteries_all, b => b.IsSameConstructAs(Me));
        for (int i = 0; i < batteries_all.Count; i++)
        {
            var b = batteries_all[i];
            string size = b.MaxStoredPower > battery_medium ? "Large": (b.MaxStoredPower < battery_medium ? "Small": "");
            b.CustomName = $"{size} Battery {i + 1} [{battery_tag}]".Trim();
            batteries_tag.Add(b);
        }
    }

    if (solar)
    {
        List<IMySolarPanel> spanels_all = new List<IMySolarPanel>();
        gts.GetBlocksOfType(spanels_all, b => b.IsSameConstructAs(Me));
        for (int i = 0; i < spanels_all.Count; i++)
        {
            var s = spanels_all[i];
            s.CustomName = $"Solar Panel {i + 1} [{solar_panel_tag}] [{power_tag}]";
            spanels_a.Add(s);
        }
    }

    if (hydro || wind || reactor)
    {
        List<IMyPowerProducer> generators_all = new List<IMyPowerProducer>();
        gts.GetBlocksOfType(generators_all, b => b.IsSameConstructAs(Me));

        foreach (var gen in generators_all)
        {
            if (wind && gen.BlockDefinition.SubtypeId.Contains("WindTurbine"))
            {
                gen.CustomName = $"Wind Turbine {generators_tag_t.Count + 1} [{t_gen_tag}] [{battery_tag}]";
                generators_tag_t.Add(gen);
            }
            else if (reactor && gen.BlockDefinition.SubtypeId.Contains("Reactor"))
            {
                gen.CustomName = $"Reactor {generators_tag_r.Count + 1} [{r_gen_tag}] [{battery_tag}]";
                generators_tag_r.Add(gen);
            }
            else if (hydro && gen.BlockDefinition.SubtypeId.Contains("HydrogenEngine"))
            {
                gen.CustomName = $"Hydrogen Engine {generators_tag_h.Count + 1} [{h_gen_tag}] [{battery_tag}]";
                generators_tag_h.Add(gen);
            }
        }
    }

    FormatDisplays(myTextSurfaces_d1);
    FormatDisplays(myTextSurfaces_d2);
    FormatDisplays(myTextSurfaces_d3);

    Echo("Initialization complete!");
    setup_complete = true;
}

void FormatDisplays(List<IMyTextSurface> surfaces)
{
    foreach (var surface in surfaces)
    {
        if (surface != null && surface.ContentType != ContentType.TEXT_AND_IMAGE)
        {
            surface.ContentType = ContentType.TEXT_AND_IMAGE;
            surface.Alignment = TextAlignment.CENTER;
            surface.FontSize = 1.263f;
        }
    }
}

public void Main(string argument)
{
    runTick++;
    if (argument == "init"|| !setup_complete)
    {
        setup_complete = false;
        sbtexttemp.AppendLine("Initializing..");
        LoadStorageData(Me.CustomData);
        Setup_System();
    }

    string[] spinners = { ".---", "-.--", "--.-", "---."};
    if (state > 3) state = 0;
    string spinner = spinners[state];

    sbtexttemp.Append("Power Status display ").Append(version).Append(" Running ").Append(spinner).Append('\n');

    if (listgenerator_finished)
    {
        listgenerator_finished = false;
    }

    if (listCoroutine == null && !listgenerator_finished)
    {
        listCoroutine = Inventory_Scan();
    }

    if (listCoroutine != null && !listgenerator_finished)
    {
        if (!listCoroutine.MoveNext())
        {
            sbtexttemp.AppendLine("Inventory list complete.");
            listCoroutine.Dispose();
            listCoroutine = null;
            listgenerator_finished = true;
        }
        else if (!listCoroutine.Current)
        {
            if (battery) sbtexttemp.Append("Updating battery list... ").Append(Math.Round(percent_list_battery, 1)).Append("%\n");
            if (solar) sbtexttemp.Append("Updating solar panel list... ").Append(Math.Round(percent_list_solar, 1)).Append("%\n");
            if (hydro) sbtexttemp.Append("Updating hydrogen gen list... ").Append(Math.Round(percent_list_generator_h, 1)).Append("%\n");
            if (wind) sbtexttemp.Append("Updating wind turbine list... ").Append(Math.Round(percent_list_generator_w, 1)).Append("%\n");
            if (reactor) sbtexttemp.Append("Updating reactor list... ").Append(Math.Round(percent_list_generator_r, 1)).Append("%\n");
        }
    }

    sbtext.Clear();
    sbtext.Append("--- ").Append(battery_tag).Append(" Battery Power Status ---\n");
    sbtext.Append("============================\n");
    sbtext.Append("| Stored Power  | ").Append(Math.Round(stored_power_total, 2)).Append(" MWh\n");
    sbtext.Append("| Current Output | ").Append(Math.Round(current_power_total, 2)).Append(" MW\n");
    sbtext.Append("| Current Input   | ").Append(Math.Round(in_current_power_total, 2)).Append(" MW\n");
    sbtext.Append("============================\n");
    sbtext.Append("| Battery Count  | ").Append(batteries_tag.Count).Append('\n');
    sbtext.Append("Battery Charge: ").Append(Math.Round(percent_battery_power)).Append("%\n");

    sbtext_2nd.Clear();
    sbtext_2nd.Append("--- ").Append(battery_tag).Append($" Battery Charge {Math.Round(percent_battery_power)} % ---\n");
    sbtext_2nd.Append("============================\n\n\n");

    barmaker(percent_battery_power / 100f, 0);
    sbtext_2nd.Append(bargraph[0]).Append("\n\n");

    float charge_drain_val = in_current_power_total - current_power_total;

    if (charge_drain_val != 0 && percent_battery_power < 100.0f)
    {
        float time_remain = charge_drain_val < 0
            ? stored_power_total / -charge_drain_val
            : (max_power_total - stored_power_total) / charge_drain_val;

        int total_hours = (int)time_remain;
        int days = total_hours / 24;
        int hours = total_hours % 24;
        int minutes = (int)((time_remain - total_hours) * 60f);

        sbtext_2nd.Append(charge_drain_val < 0 ? "Power out: ": "Power in: ")
                  .Append(Math.Round(Math.Abs(charge_drain_val), 2)).Append(" MW\n");
        sbtext_2nd.Append(charge_drain_val < 0 ? "Charge empty:": "Charge full:");

        if (days > 0) sbtext_2nd.Append(" ").Append(days).Append(" d");
        if (hours > 0) sbtext_2nd.Append(" ").Append(hours).Append(" h");
        if (minutes > 0) sbtext_2nd.Append(" ").Append(minutes).Append(" m");
        sbtext_2nd.Append('\n');
    }

    if (solar)
    {
        sbtext.Append("\n--- ").Append(solar_panel_tag).Append(" Solar Power Status ---\n");
        sbtext.Append("============================\n");
        sbtext.Append("| Total Solar     | ").Append(Math.Round(total_power_a, 2)).Append(" MW\n");
        sbtext.Append("| Total panel # | ").Append(spanels_a.Count).Append('\n');
    }
    if (hydro)
    {
        sbtext.Append("\n--- ").Append(h_gen_tag).Append(" Generator Power Status ---\n");
        sbtext.Append("============================\n");
        sbtext.Append("| Total gen. pwr | ").Append(Math.Round(total_power_gen, 2)).Append(" MW\n");
        sbtext.Append("| Total gen. #     | ").Append(generators_tag_h.Count).Append('\n');
    }
    if (wind)
    {
        sbtext.Append("\n--- ").Append(t_gen_tag).Append(" Generator Power Status ---\n");
        sbtext.Append("============================\n");
        sbtext.Append("| Total gen. pwr | ").Append(Math.Round(total_power_gen_w, 2)).Append(" MW\n");
        sbtext.Append("| Total gen. #     | ").Append(generators_tag_t.Count).Append('\n');
    }
    if (reactor)
    {
        sbtext.Append("\n--- ").Append(r_gen_tag).Append(" Generator Power Status ---\n");
        sbtext.Append("============================\n");
        sbtext.Append("| Total gen. pwr | ").Append(Math.Round(total_power_gen_r, 2)).Append(" MW\n");
        sbtext.Append("| Total gen. #     | ").Append(generators_tag_r.Count).Append('\n');
    }

    if (listgenerator_finished)
    {
        WriteToDisplays(myTextSurfaces_d1, sbtext_2nd);
        WriteToDisplays(myTextSurfaces_d2, sbtext_2nd);
        WriteToDisplays(myTextSurfaces_d3, sbtext);
    }

    if (runTick % 2 == 0) Echo(sbtexttemp.ToString());
    sbtexttemp.Clear();
    state++;
    if (runTick > 60) runTick = 0;
}

void WriteToDisplays(List<IMyTextSurface> displays, StringBuilder text)
{
    for (int i = 0; i < displays.Count; i++)
    {
        displays[i]?.WriteText(text);
    }
}

IEnumerator<bool> Inventory_Scan()
{
    stored_power_total = 0;
    max_power_total = 0;
    current_power_total = 0;
    in_current_power_total = 0;
    percent_battery_power = 0;

    if (battery && batteries_tag.Count > 0)
    {
        for (int i = 0; i < batteries_tag.Count; i++)
        {
            if (batteries_tag[i] != null && batteries_tag[i].IsFunctional)
            {
                stored_power_total += batteries_tag[i].CurrentStoredPower;
                max_power_total += batteries_tag[i].MaxStoredPower;
                current_power_total += batteries_tag[i].CurrentOutput;
                in_current_power_total += batteries_tag[i].CurrentInput;
            }

            percent_list_battery = batteries_tag.Count == 1 ? 100.0 : ((double)i / (batteries_tag.Count - 1)) * 100;

            // Yield dynamically based on CPU cycle limits instead of hard limit
            if (Runtime.CurrentInstructionCount > 100) yield return false;
        }
        if (max_power_total > 0) percent_battery_power = (stored_power_total / max_power_total) * 100;
    }

    total_power_a = 0;
    if (solar && spanels_a.Count > 0)
    {
        for (int j = 0; j < spanels_a.Count; j++)
        {
            if (spanels_a[j] != null && spanels_a[j].IsFunctional)
            {
                total_power_a += spanels_a[j].CurrentOutput;
            }

            percent_list_solar = spanels_a.Count == 1 ? 100.0 : ((double)j / (spanels_a.Count - 1)) * 100;
            if (Runtime.CurrentInstructionCount > 100) yield return false;
        }
    }

    total_power_gen = 0;
    if (hydro && generators_tag_h.Count > 0)
    {
        for (int k = 0; k < generators_tag_h.Count; k++)
        {
            if (generators_tag_h[k] != null && generators_tag_h[k].IsFunctional)
            {
                total_power_gen += generators_tag_h[k].CurrentOutput;
            }
            percent_list_generator_h = generators_tag_h.Count == 1 ? 100.0 : ((double)k / (generators_tag_h.Count - 1)) * 100;
            if (Runtime.CurrentInstructionCount > 100) yield return false;
        }
    }

    total_power_gen_w = 0;
    if (wind && generators_tag_t.Count > 0)
    {
        for (int k = 0; k < generators_tag_t.Count; k++)
        {
            if (generators_tag_t[k] != null && generators_tag_t[k].IsFunctional)
            {
                total_power_gen_w += generators_tag_t[k].CurrentOutput;
            }
            percent_list_generator_w = generators_tag_t.Count == 1 ? 100.0 : ((double)k / (generators_tag_t.Count - 1)) * 100;
            if (Runtime.CurrentInstructionCount > 100) yield return false;
        }
    }

    total_power_gen_r = 0;
    if (reactor && generators_tag_r.Count > 0)
    {
        for (int k = 0; k < generators_tag_r.Count; k++)
        {
            if (generators_tag_r[k] != null && generators_tag_r[k].IsFunctional)
            {
                total_power_gen_r += generators_tag_r[k].CurrentOutput;
            }
            percent_list_generator_r = generators_tag_r.Count == 1 ? 100.0 : ((double)k / (generators_tag_r.Count - 1)) * 100;
            if (Runtime.CurrentInstructionCount > 100) yield return false;
        }
    }

    yield return true;
}

void barmaker(float val_in, int indexer)
{
    int filled = (int)Math.Max(0, Math.Min(20, Math.Round(val_in * 20)));
    int empty = 20 - filled;

    // Rapid string allocation bypassing massive iteration chains
    bargraph[indexer] = $"[{new string('|', filled)}{new string('-', empty)}]";
}
