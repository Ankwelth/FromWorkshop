/*
 * Script Version 1.02
 * 
 * ****************************************************************************************************
 * En
 * 
 * Attention! Add all blocks related to the Airlock to one group. The oxygen tanks of airlock should not be connected to oxygen generators.
 * 
 * The register of names, parameters and arguments is not important!
 * 
 * The number of groups and the number of elements in a group can be any!
 * 
 * Explanations for "swap":
 * When the oxygen in the cylinder ends (for example, damage or accidental depressurization of the airlock room or the cylinders themselves)
 * swap supplies additional oxygen from the generator. You can add any quantity in the airlock.
 * 
 * Explanations for "ext":
 * It is used as an external pressure sensor and is designed to equalize the pressure in the airlock and the outside
 * (on different planets, different pressure). In SCADA, the CGA additionally displays whether it is possible to breathe outside the gateway.
 * It is advisable to add to all groups at once if the airlock goes into open space.
 * Usually no more than one is required, but for safety (in case of failure or destruction).
 * You can add any quantity behind the airlock (outside).
 * 
 * Explanations for "pres":
 * It is used as an internal pressure sensor and is designed to turn off the airlock during depressurization of a room.
 * In SCADA, the CGA additionally displays the oxygen level in the room or indicates that the room is depressurized.
 * It is advisable to add to all groups at once if the airlocks leave the same room.
 * Usually no more than one is required, but for safety (in case of failure or destruction).
 * You can add any quantity but within the same room with airlock.
 * 
 * ////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * Declare "AirlockSystems class_name = new AirlockSystems ();".
 * 
 * Attention! Now you need to declare a new instance of the class for each new gateway.
 * 
 * Examples:
 * 1) With the output of debugging information in the programmable block -> Echo($"{class_name.Run(this, "group_name", argument)}");
 * 2) Simple start -> class_name.Run(this, "group_name", argument);
 * 
 * "this" - required, "airlock group" - required, "ru" (Russian) - optional (English),
 * "arg" - for arguments "in"/"out" (Pressurize/Depressurization)
 * 
 * Required minimum for the airlock:
 * 1 - The inner door of the airlock with the mark "[in]";
 * 1 - Exterior airlock door with the mark "[out]";
 * 1 - Ventilation + 1 - Oxygen cylinder;
 * 
 * For full functionality, optionally, add any quantity:
 * Inside the airlock door with the mark "[in]";
 * Exterior airlock doors with the mark "[out]";
 * Ventilation + Oxygen cylinders;
 * Inner ventilation marked "[pres]" - monitoring the tightness of the room;
 * Swap ventilation with the mark "[swap]" + Oxygen cylinder (s) or Oxygen Generator (s);
 * Unconnected ventilation with the mark "[ext]";
 * Speakers;
 * Rotating Lights - Flashers;
 * Oxygen generators;
 * Indicators from any light sources for:
 * - Internal doors with the mark "[in]";
 * - External doors with the mark "[out]";
 * - All light sources without a mark - lighting of airlock
 * 
 * To display the status of the airlock, add any blocks with the ability to display text (Displayed in the main display).
 * In "Custom data" add, in any order, parameters via "", "," or from a new line: press, level, oxygen, oxy_all, in, out, 
 * scada_mda (for any LCD, on corner LCDs and the like, it makes no sense to use), scada_cga (only for LCD 1x2, on other LCDs it makes no sense to use);
 * 
 * Opening the airlock inward, start the program block with the argument "[in] [all]" - Everything, "[in] [Airlock_group]" - Only the group.
 * Opening the airlock out, start the program block with the argument "[out] [all]" - Everything, "[out] [Airlock_group]" - Only the group.
 * 
 * Enabled/Disabled airlock mode, start the program block with the argument "[@]".
 * 
 * Pressurize/Depressurize the airlock without opening doors - add to the argument "[$]".
 * 
 * ////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * << Advanced options >>
 * 
 * If the height of the screen is not enough to display all the information, then you need to add the label "[!main_screen_label_name]" to the screen name.
 * Install additional screen(s) (it is not necessary to add to the group), add a label to the name of the additional screen
 * "[!main_screen_label_name-sequential_screen_number]".
 * 
 * ****************************************************************************************************
 * Ru
 * 
 * Внимание! Все блоки относящиеся к шлюзу добавить в одну группу. Кислородные баллоны шлюза не должны быть соединены с генераторами кислорода.
 * 
 * Регистр имён, параметров и аргументов не важен!
 * 
 * Количество групп и количество элементов в группе может быть любым!
 * 
 * Пояснения к "swap":
 * Когда кислород в баллоне заканчивается (например повреждение или случайная разгерметизация комнаты шлюза или самих баллонов)
 * swap подаёт дополнительный кислород из генератора. Можно добавить любое количество в шлюзе.
 * 
 * Пояснения к "ext":
 * Используется как датчик наружного давления и предназначена для выравнивания давления в шлюзе и наружным пространством
 * (на разных планетах, разное давление). В SCADA CGA дополнительно отображает возможно ли дышать снаружи шлюза.
 * Желательно добавлять во все группы сразу, если шлюз выходит в открытое пространство. 
 * Обычно требуется не более одного, но для подстраховки (на случай выхода из строя или уничтожения).
 * Можно добавить любое количество за шлюзом (снаружи).
 * 
 * Пояснения к "pres":
 * Используется как датчик внутреннего давления и предназначена для отключения шлюза при разгерметизации помещения.
 * В SCADA CGA дополнительно отображает уровень кислорода в помещении либо показывает что помещение разгерметизировано.
 * Желательно добавлять во все группы сразу, если шлюзы выходят из одного помещения.
 * Обычно требуется не более одного, но для подстраховки (на случай выхода из строя или уничтожения).
 * Можно добавить любое количество но в пределах одного помещения со шлюзами.
 * 
 * ////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * Объявить "AirlockSystems имя_класса = new AirlockSystems();".
 * 
 * Внимание! Теперь нужно объявлять новый экземпляр класса для каждого нового шлюза.
 * 
 * Примеры:
 * 1) С выводом отладочной информации в программируемом блоке -> Echo($"{имя_класса.Run(this, "имя_группы", argument, "ru")}");
 * 2) Простой запуск -> имя_класса.Run(this, "имя_группы", argument, "ru");
 * 		
 * "this" - обязательно, "группа шлюза" - обязательно, "ru" (русский) - не обязательно (английский), 
 * "arg" - для аргументов "in"/"out" (герметизация/разгерметизация)
 * 
 * Необходимый минимум для шлюза:
 * 1 - Внутренняя дверь шлюза с отметкой "[in]";
 * 1 - Внешняя дверь шлюза с отметкой "[out]";
 * 1 - Вентиляция + 1 - Кислородный баллон;
 * 
 * Для полного функционала опционально добавить любое количество:
 * Внутренние двери шлюза с отметкой "[in]";
 * Внешние двери шлюза с отметкой "[out]";
 * Вентиляции + Кислородные баллоны;
 * Внутренняя вентиляция с пометкой «[pres]» - контроль герметичности помещения;
 * Вентиляции подкачки с отметкой "[swap]" + Кислородные баллон(ы) или Генератор(ы) кислорода;
 * Ни к чему не подключённая вентиляция с отметкой "[ext]";
 * Динамики;
 * Вращающиеся огни - мигалки;
 * Генераторы кислорода;
 * Индикаторы из любых источников света для:
 * - Внутренних дверей с отметкой "[in]";
 * - Внешних дверей с отметкой "[out]";
 * - Все источники света без отметки - освещение шлюза;
 * 
 * Для отображения состояния шлюза добавить любые блоки с возможностью отображать текст (Отображается в основном дисплее).
 * В "Свои данные" добавить, в любой последовательности, параметры через " ", "," или с новой строки: press, level, oxygen, oxy_all, in, out, 
 * scada_mda и scada_cga(для любых LCD, на угловых LCD и им подобным LCD не имеет смысла использовать)
 * 
 * Открытие шлюза внутрь, запускать программный блок с аргументом "[in] [all]" - Все, "[in] [Группа_шлюза]" - Только группа.
 * Открытие шлюза наружу, запускать программный блок с аргументом "[out] [all]" - Все, "[out] [Группа_шлюза]" - Только группа.
 * 
 * Включение/Выключение режима шлюза, запускать программный блок с аргументом "[@]".
 * 
 * Герметизация/Разгерметизация шлюза без открытия дверей - добавить к аргументу "[$]".
 * 
 * ////////////////////////////////////////////////////////////////////////////////////////////////////
 * 
 * << Дополнительные параметры >>
 * 
 * Если высоты экрана не достаточно для вывода всей информации, то к имени экрана нужно добавить метку "[!имя_метки_главного_экрана]".
 * Установить дополнительный экран(ы) (в группу добавлять не обязательно), к имени дополнительного экрана нужно добавить метку 
 * "[!имя_метки_главного_экрана-порядковый_номер_экрана]".
 * 
 */

AirlockSystems AS1 = new AirlockSystems();
AirlockSystems AS2 = new AirlockSystems();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string arg, UpdateType uType)
{
    Echo($"Script Speed: {Runtime.LastRunTimeMs,0:F3} ms\n");
    Echo($"{AS1.Run(this, "Airlock_1", arg, "ru", true)}");
    Echo($"{AS2.Run(this, "Airlock_2", arg, "ru", true)}");
}

/// <summary>
        /// In "Main" class_name.Run (this, "airlock group", arg, "ru"), "this" unchanged.
        /// В "Main" имя_класса.Run(this, "группа шлюза", arg, "ru"), "this" без изменений.
        /// </summary>
private class AirlockSystems
{
    private bool isAirlock = true, toggle = false, resumeSound = false;
    private string buf = "";

    private double LAUNCH_DELAY = 10;
    private int PROGRESS_LAUNCH = 0;

    private readonly char[] sb = { '(', ')', '|', '*', '*', ' ' };

    /// <summary>
            /// Launch Airlock Systems (Запустить системы шлюзов)
            /// </summary>
            /// <param name="root">Mandatory keyword as a constant "this" (Обязательное ключевое слово как константа "this")</param>
            /// <param name="grASTag">Group Name (Имя группы)</param>
            /// <param name="arg">Argument - Optional (Аргумент - Необязательный)</param>
            /// <param name="ln">Language - Optional (Язык - Необязательный)</param>
            /// <param name="pref">Toogle On/Off prefixes (Включить/выключить префиксы)</param>
            /// <returns>Report (Отчёт)</returns>
    public string Run(Program root, string grASTag, string arg = "", string ln = "", bool pref = false)
    {
        double total_g = 0, total_o = 0;

        int wc = 0;

        var gAirlock = root.GridTerminalSystem.GetBlockGroupWithName(grASTag);

        if (gAirlock == null) return $"{Lng("error", ln)}";

        if (arg != "") buf = arg.ToLower();

        var all_Vents = new List<IMyAirVent>();
        root.GridTerminalSystem.GetBlocksOfType(all_Vents, a_v => !a_v.CustomName.ToLower().Contains("[ext]") & !a_v.CustomName.ToLower().Contains("[swap]"));

        var airlock_Lights = new List<IMyLightingBlock>();      //Light
        var airlock_Flashers = new List<IMyReflectorLight>();   //Flasher - Rotating Light
        var out_Indicators = new List<IMyLightingBlock>();      //[out]
        var in_Indicators = new List<IMyLightingBlock>();       //[in]
        var air_Speakers = new List<IMySoundBlock>();
        var air_Vents = new List<IMyAirVent>();
        var ext_Vents = new List<IMyAirVent>();                 //[ext]     // External pressure ()
        var pres_Vents = new List<IMyAirVent>();                //[pres]    // Internal pressure ()
        var swap_Vents = new List<IMyAirVent>();                //[swap]    // Pumps up oxygen if tanks are empty (Накачивает кислород, если баки пусты)
        var stor_Tanks = new List<IMyGasTank>();
        var gas_Gen = new List<IMyGasGenerator>();
        var out_Doors = new List<IMyDoor>();                    //[out]
        var in_Doors = new List<IMyDoor>();                     //[in]
        var stat_Info = new List<IMyTerminalBlock>();           //(pressure, level, oxygen, oxy_all, in, out, gasgen, gen_all, scada_mda, scada_cga, mode)

        gAirlock.GetBlocksOfType(airlock_Lights, light => !light.CustomName.ToLower().Contains("[out]")
            & !light.CustomName.ToLower().Contains("[in]") & light is IMyLightingBlock & light.IsFunctional
            & !(light is IMyReflectorLight & light.Mass < 50));
        gAirlock.GetBlocksOfType(airlock_Flashers, light => !light.CustomName.ToLower().Contains("[out]")
            & !light.CustomName.ToLower().Contains("[in]") & light is IMyReflectorLight & light.IsFunctional
            & light.BlockDefinition.SubtypeId.ToLower().Contains("rotatinglight"));
        gAirlock.GetBlocksOfType(out_Indicators, light => light.CustomName.ToLower().Contains("[out]")
            & light is IMyLightingBlock & light.IsFunctional);
        gAirlock.GetBlocksOfType(in_Indicators, light => light.CustomName.ToLower().Contains("[in]")
            & light is IMyLightingBlock & light.IsFunctional);
        gAirlock.GetBlocksOfType(air_Speakers, spk => spk is IMySoundBlock & spk.IsFunctional);
        gAirlock.GetBlocksOfType(air_Vents, vent => !vent.CustomName.ToLower().Contains("[swap]")
            & !vent.CustomName.ToLower().Contains("[pres]") & !vent.CustomName.ToLower().Contains("[ext]")
            & vent is IMyAirVent & vent.IsFunctional);
        gAirlock.GetBlocksOfType(ext_Vents, vent => vent.CustomName.ToLower().Contains("[ext]")
            & vent is IMyAirVent & vent.IsFunctional);
        gAirlock.GetBlocksOfType(pres_Vents, vent => vent.CustomName.ToLower().Contains("[pres]")
            & vent is IMyAirVent & vent.IsFunctional);
        gAirlock.GetBlocksOfType(swap_Vents, vent => vent.CustomName.ToLower().Contains("[swap]")
            & vent is IMyAirVent & vent.IsFunctional);
        gAirlock.GetBlocksOfType(stor_Tanks, tank => tank is IMyGasTank & tank.IsFunctional);
        gAirlock.GetBlocksOfType(gas_Gen, gen => gen is IMyGasGenerator & gen.IsFunctional);
        gAirlock.GetBlocksOfType(out_Doors, door => door.CustomName.ToLower().Contains("[out]")
            & door is IMyDoor & door.IsFunctional);
        gAirlock.GetBlocksOfType(in_Doors, door => door.CustomName.ToLower().Contains("[in]")
            & door is IMyDoor & door.IsFunctional);
        /// Временное решение
                /// gAirlock.GetBlocksOfType(stat_Info, info => info is IMyTextSurfaceProvider & info.IsFunctional);
                /// gAirlock.GetBlocksOfType(stat_Info, info => info is IMyTextPanel & info.IsFunctional);
                /// gAirlock.GetBlocksOfType(stat_Info, info => info is IMyTextSurface & info.IsFunctional);
        //gAirlock.GetBlocksOfType(stat_Info, info => (info is IMyTextSurface || info is IMyCockpit) & !(info is IMyCockpit) & info.IsFunctional);
        //gAirlock.GetBlocksOfType(stat_Info, info => (info is IMyTextSurface || info is IMyCockpit) & info.BlockDefinition.SubtypeId.ToLower().Contains("text") & info.IsFunctional);
        gAirlock.GetBlocksOfType(stat_Info, info => info is IMyTextSurface
            || info.BlockDefinition.SubtypeId.ToLower().Contains("atmblock") || info.BlockDefinition.SubtypeId.ToLower().Contains("medical")
            || info.BlockDefinition.SubtypeId.ToLower().Contains("turretcontrolblock") || info.BlockDefinition.SubtypeId.ToLower().Contains("scifibuttonpanel")
            || info.BlockDefinition.SubtypeId.ToLower().Contains("programmableblock") || info.BlockDefinition.SubtypeId.ToLower().Contains("cockpit")
            || info.BlockDefinition.SubtypeId.ToLower().Contains("scifibuttonterminal") || info.BlockDefinition.SubtypeId.ToLower().Contains("blockconsole")
            || info.BlockDefinition.SubtypeId.ToLower().Contains("survivalkit") || info.BlockDefinition.SubtypeId.ToLower().Contains("labequipment")
            & info.IsFunctional);

        /*-- Renaming Prefix --*/
        var ren_List = new List<IMyTerminalBlock>();
        gAirlock.GetBlocks(ren_List);

        if (pref)
        {
            foreach (var item in ren_List)
            {
                if (item.ShowInTerminal == true) item.ShowInTerminal = false;

                if (!item.CustomName.ToLower().Contains($"{sb[3]}{grASTag.ToLower()}{sb[4]}"))
                    item.CustomName = $"{sb[3]}{grASTag}{sb[4]}{sb[5]}{item.CustomName}";

                if (!item.CustomName.StartsWith($"{sb[0]}"))
                    item.CustomName = $"{sb[0]}{item.CustomName.Replace($"{sb[4]}{sb[5]}", $"{sb[4]}{sb[1]}{sb[5]}")}";
                else item.CustomName = item.CustomName.Replace($"{sb[4]}{sb[5]}", $"{sb[4]}{sb[1]}{sb[5]}");

                item.CustomName = item.CustomName.Replace($"{sb[1]} {sb[0]}", $"{sb[4]}{sb[2]}{sb[3]}");
                item.CustomName = item.CustomName.Replace($"{sb[3]}{sb[3]}", $"{sb[3]}");
                item.CustomName = item.CustomName.Replace($"{sb[4]}{sb[4]}", $"{sb[4]}");
            }
            ClearingPrefixes(root, ren_List, grASTag);
        }
        else ClearingPrefixes(root, ren_List, grASTag, true);
        /*== Renaming Prefix ==*/

        /*-- Ambient --*/
        float extPres = ext_Vents[0].GetOxygenLevel();

        /*float extPres = 0;
                float inPres = 0;

                foreach (var item in ext_Vents) extPres += Convert.ToSingle(Math.Round(item.GetOxygenLevel() / ext_Vents.Count, 2));
                extPres += extPres == 0 ? 0.01f : extPres == 1 ? -0.01f : 0;

                foreach (var item in pres_Vents) inPres += Convert.ToSingle(Math.Round(item.GetOxygenLevel() / pres_Vents.Count, 2));
                inPres += inPres == 0 ? 0.01f : inPres == 1 ? -0.01f : 0;

                if (extPres == inPres)
                {
                    foreach (var item in air_Vents) item.Enabled = false;
                    return $"«««=»»»\n{Lng("pres_diff", ln)}";
                }*/
        /*== Ambient ==*/
        Func<IMyAirVent, float> o_l_h = olh => System.Text.RegularExpressions.Regex.Match(olh.DetailedInfo.Split('\n')[2], @"[\d]*\.?[\d]+([eE][-+][\d]+)?").ToString() == ""
                    ? 0 : float.Parse(System.Text.RegularExpressions.Regex.Match(
                        olh.DetailedInfo.Split('\n')[2], @"[\d]*\.?[\d]+([eE][-+][\d]+)?").ToString()) / 100;

        Func<IMyAirVent, string> a_f_s = afs => new string('●', (int)(((float?)o_l_h(afs) ?? 0) * 42)).PadRight(42, '○');
        Func<IMyGasTank, string> o_f_s = ofs => new string('●', (int)((ofs?.FilledRatio ?? 0) * 42)).PadRight(42, '○');
        Func<IMyGasGenerator, string> g_f_s = gfs => new string('■', (int)(((double?)(gfs.GetInventory().CurrentVolume) /
            (double?)(gfs.GetInventory().MaxVolume) ?? 0) * 42)).PadRight(42, '∙');
        Func<IMyDoor, char> g_d = gd => "∙X«✓»₪"[gd == null ? 0 : gd.Enabled ? 2 + (int)gd.Status : 1];
        Func<IMyAirVent, char> a_s = a => "∙X☺↓☻↑"[a == null ? 0 : a.Enabled == false ? 1 : a.CanPressurize == false ? 2
            : o_l_h(a) < extPres ? 3 : a.Status == VentStatus.Depressurizing ? 4 : a.Status == VentStatus.Pressurized ? 5
            : a.Status == VentStatus.Pressurizing ? 6 : 7];
        Func<IMyAirVent, char> a_f = af => "∙ _▁▄█◙"[af == null ? 0 : o_l_h(af) < extPres ? 1 : o_l_h(af) < 0.25f ? 2
            : o_l_h(af) < 0.5f ? 3 : o_l_h(af) < 0.75 ? 4 : o_l_h(af) < 1 ? 5 : o_l_h(af) >= 1 ? 6 : 7];
        Func<IMyGasTank, char> o_f = af => "∙ _▁▄█◙"[af == null ? 0 : af.FilledRatio < extPres ? 1 : af.FilledRatio < 0.25f ? 2
            : af.FilledRatio < 0.5f ? 3 : af.FilledRatio < 0.75 ? 4 : af.FilledRatio < 1 ? 5 : af.FilledRatio == 1 ? 6 : 7];
        Func<IMyAirVent, char> p_f = pf => "∙↑↓→←"[pf == null ? 0 : pf.Status == VentStatus.Pressurizing ? 1 :
            pf.Status == VentStatus.Pressurized ? 1 : pf.Status == VentStatus.Depressurizing ? 2 : pf.Status == VentStatus.Depressurized ? 2 : 3];
        Func<IMyGasTank, char> change = c => "∙¤☼"[c == null ? 0 : c.Stockpile == true ? 1 : 2];

        /*-- SCADA --*/
        Func<IMyAirVent, string> scada_a_f_s = scada_afs => new string('●', (int)(((float?)o_l_h(scada_afs) ?? 0) * 7)).PadRight(7, '○');
        Func<IMyGasGenerator, char> g_f = gf => "X∅≈"[gf == null ? 0 : gf.IsWorking ? 2 : 1];
        Func<int, char> g_f_t = gft => "∅≈"[gft > 0 ? 1 : 0];

        Func<IMyAirVent, char> scada_v_s = scada_vs => "X≠≡"[scada_vs == null ? 0 : scada_vs.IsWorking ? 2 : 1];
        Func<IMyAirVent, char> scada_p_s = scada_ps => "X▲▼"[scada_ps == null ? 0 : scada_ps.Status == VentStatus.Pressurizing ? 2 :
            scada_ps.Status == VentStatus.Pressurized ? 2 : 1];
        Func<IMyDoor, char> scada_s_d_u = scada_sdu => "X╬╩"[scada_sdu == null ? 0 : scada_sdu.OpenRatio == 1 ? 2 : 1];
        Func<IMyDoor, char> scada_s_d_d = scada_sdd => "X╬╦"[scada_sdd == null ? 0 : scada_sdd.OpenRatio == 1 ? 2 : 1];
        /*== SCADA ==*/

        //------ Presets ------
        foreach (var item in air_Vents) item.Enabled = true;
        foreach (var item in ext_Vents) item.Enabled = true;
        foreach (var item in stor_Tanks)
        {
            item.Enabled = true;
            item.Stockpile = false;
            item.AutoRefillBottles = true;
        }
        foreach (var item in in_Indicators)
        {
            item.Enabled = true;
            item.Radius = 1f;
            item.Falloff = 3f;
            item.Intensity = 10f;
            item.BlinkLength = 50f;
            item.BlinkOffset = 0f;
            item.SetValueFloat("Offset", 0);
        }
        foreach (var item in out_Indicators)
        {
            item.Enabled = true;
            item.Radius = 1f;
            item.Falloff = 3f;
            item.Intensity = 10f;
            item.BlinkLength = 50f;
            item.BlinkOffset = 0f;
            item.SetValueFloat("Offset", 0);
        }
        foreach (var item in airlock_Lights)
        {
            item.Enabled = true;
            //item.Radius = 5f;
            item.Falloff = 3f;
            item.Intensity = 5f;
            item.BlinkLength = 50f;
            item.BlinkOffset = 0f;
            item.SetValueFloat("Offset", (item.Radius / 2));
        }
        foreach (var item in airlock_Flashers)
        {
            item.Enabled = false;
            item.Color = Color.Red;
            item.Radius = 1000f;
            item.Intensity = 100f;
            item.SetValueFloat("Offset", 10f);
            item.SetValueFloat("RotationSpeed", 0.05f);
        }
        foreach (IMyTextSurfaceProvider item in stat_Info)
        {
            item.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
            item.GetSurface(0).FontSize = 0.56f;
            item.GetSurface(0).Font = "Monospace";
            item.GetSurface(0).FontColor = new Color(255, 255, 0);
            item.GetSurface(0).Alignment = TextAlignment.LEFT;
            item.GetSurface(0).TextPadding = 0;
            item.GetSurface(0).BackgroundColor = new Color(0, 0, 50);
            if ((item as IMyTerminalBlock).CustomData == "")
                (item as IMyTerminalBlock).CustomData =
                    $"{Lng("help_params", ln)}\nlevel\n#pressure\n#oxygen\n#oxy_all\n#in\n#out\n#gasgen\n#gen_all\n#scada_mda\n#scada_cga\n#mode";
        }
        //====== Presets ======

        //Delay until all blocks are initialized
        var rUF = root.Runtime.UpdateFrequency;
        var tick = Math.Round(rUF == UpdateFrequency.Update1 ? 60 : rUF == UpdateFrequency.Update10 ? 6 : rUF == UpdateFrequency.Update100 ? 0.67 : 0);

        if (PROGRESS_LAUNCH < LAUNCH_DELAY * tick)
        {
            resumeSound = true;
            PROGRESS_LAUNCH++;
            var progress = PROGRESS_LAUNCH / (LAUNCH_DELAY * tick);
            foreach (var item in stat_Info)
            {
                if ((item as IMyTextSurfaceProvider).GetSurface(0).SurfaceSize.X == 1024)
                    (item as IMyTextSurfaceProvider).GetSurface(0).FontSize = (item as IMyTextSurfaceProvider).GetSurface(0).FontSize * 2;

                if (item is IMyTextPanel)
                {
                    (item as IMyTextPanel).Alignment = TextAlignment.CENTER;
                    (item as IMyTextPanel).WriteText($"{Lng("lod", ln)}: {Math.Floor(progress * 100)}%\n" +
                    $"[{new string(':', (int)(progress * 29)).PadRight(29, '.')}]");
                }
                else
                {
                    (item as IMyTextSurfaceProvider).GetSurface(0).Alignment = TextAlignment.CENTER;
                    (item as IMyTextSurfaceProvider).GetSurface(0).WriteText($"{Lng("lod", ln)}: {Math.Floor(progress * 100)}%\n" +
                    $"[{new string(':', (int)(progress * 29)).PadRight(29, '.')}]");
                }
            }
            return $"{Lng("lod", ln)}: {Math.Floor(progress * 100)}%\n[{new string(':', (int)(progress * 29)).PadRight(29, '.')}]";
        }

        // ------ Check For Need Airlock (Проверить, нужен ли шлюз) ------
        if (arg.Contains("[@]")) toggle = true; else if (toggle) { isAirlock = !isAirlock; toggle = false; }

        if (!isAirlock)
        {
            Sound(air_Speakers);
            resumeSound = true;

            foreach (var item in all_Vents) item.Enabled = false;
            foreach (var item in swap_Vents) item.Enabled = false;
            DoorStatusInd(in_Doors, in_Indicators, airlock_Lights);
            DoorStatusInd(out_Doors, out_Indicators, airlock_Lights);
            goto finish;
        }
        else
        {
            foreach (var room_Tightness in pres_Vents)
            {
                if (!room_Tightness.CanPressurize)
                {
                    foreach (var item in all_Vents) item.Enabled = false;
                    foreach (var item in airlock_Flashers) item.Enabled = true;

                    if (resumeSound) { Sound(air_Speakers, true, num: 4); resumeSound = false; }

                    DoorStatusInd(in_Doors, in_Indicators, airlock_Lights);
                    DoorStatusInd(out_Doors, out_Indicators, airlock_Lights);

                    goto finish;
                }
                else
                {
                    foreach (var item in all_Vents) item.Enabled = true;
                    foreach (var item in airlock_Flashers) item.Enabled = false;
                }
            }
        }
        // ====== C.F.N.A. ======

        foreach (var obj in in_Doors)
            switch (obj.Status)
            {
                case DoorStatus.Open:
                    foreach (var item in in_Indicators)
                    {
                        item.Color = Color.Lime;
                        item.BlinkIntervalSeconds = 0f;
                    }
                    break;
                case DoorStatus.Closed:
                    foreach (var item in in_Indicators)
                    {
                        foreach (var inj in air_Vents)
                            if (inj.GetOxygenLevel() > 0f)
                            {
                                item.BlinkIntervalSeconds = 1f;
                                item.Color = Color.Lime;
                            }
                            else
                            {
                                item.BlinkIntervalSeconds = 0f;
                                item.Color = Color.Red;
                            }
                    }
                    break;
                default:
                    foreach (var item in in_Indicators)
                    {
                        item.Color = Color.OrangeRed;
                        item.BlinkIntervalSeconds = 1f;
                    }
                    break;
            }

        foreach (var obj in out_Doors)
            switch (obj.Status)
            {
                case DoorStatus.Open:
                    foreach (var item in out_Indicators)
                    {
                        item.Color = Color.Lime;
                        item.BlinkIntervalSeconds = 0f;
                    }
                    break;
                case DoorStatus.Closed:
                    foreach (var item in out_Indicators)
                    {
                        foreach (var inj in air_Vents)
                            if (inj.GetOxygenLevel() > 0f)
                            {
                                item.BlinkIntervalSeconds = 0f;
                                item.Color = Color.Red;
                            }
                            else
                            {
                                item.BlinkIntervalSeconds = 1f;
                                item.Color = Color.Lime;
                            }
                    }
                    break;
                default:
                    foreach (var item in out_Indicators)
                    {
                        item.Color = Color.OrangeRed;
                        item.BlinkIntervalSeconds = 1f;
                    }
                    break;
            }

        foreach (var obj in air_Vents)
            switch (obj.Status)
            {
                case VentStatus.Pressurized:
                    foreach (var item in airlock_Lights) item.Color = Color.White;

                    Sound(air_Speakers);
                    resumeSound = true;
                    break;
                case VentStatus.Pressurizing:
                    if (obj.GetOxygenLevel() < 0.9)
                    {
                        foreach (var item in in_Indicators) { item.Color = Color.Red; item.BlinkIntervalSeconds = 1f; }
                        foreach (var item in out_Indicators) { item.Color = Color.Red; item.BlinkIntervalSeconds = 1f; }
                        foreach (var item in airlock_Lights) item.Color = new Color(0, 255, 128);

                        //if (resumeSound) { Sound(air_Speakers, true, num: 4); resumeSound = false; }
                        //if (!buf.Contains("[$]")) if (resumeSound) { Sound(air_Speakers, true, num: 4); resumeSound = false; }
                        if (!buf.Contains("[out]")) if (resumeSound) { Sound(air_Speakers, true, num: 4); resumeSound = false; }
                    }
                    break;
                default:
                    foreach (var inj in out_Doors)
                    {
                        if (obj.GetOxygenLevel() > extPres & inj.Status == DoorStatus.Closed)
                        {
                            foreach (var item in in_Indicators) { item.Color = Color.Red; item.BlinkIntervalSeconds = 1f; }
                            foreach (var item in out_Indicators) { item.Color = Color.Red; item.BlinkIntervalSeconds = 1f; }
                            foreach (var item in airlock_Lights) item.Color = new Color(0, 128, 255);

                            if (resumeSound) { Sound(air_Speakers, true, num: 4); resumeSound = false; }
                        }
                        else
                        {
                            foreach (var item in airlock_Lights) item.Color = Color.Yellow;
                            foreach (var item in in_Indicators) { item.Color = Color.Red; item.BlinkIntervalSeconds = 0f; }

                            Sound(air_Speakers);
                            resumeSound = true;
                        }
                    }
                    break;
            }

        double stor_Filled = 0;
        foreach (var obj in stor_Tanks)
        {
            stor_Filled += obj.FilledRatio;
            if (stor_Filled <= 0) foreach (var item in swap_Vents) item.Enabled = true;
            else foreach (var item in swap_Vents) item.Enabled = false;
        }

        foreach (var obj in air_Vents)
            if (obj.CanPressurize)
            {
                if (obj.GetOxygenLevel() > 0f) foreach (var item in out_Doors) item.Enabled = false;
                else foreach (var item in out_Doors) item.Enabled = true;

                if (obj.Depressurize)
                {
                    foreach (var item in in_Doors)
                    {
                        item.CloseDoor();
                        if (item.Status == DoorStatus.Closed) item.Enabled = false;
                    }
                }
            }
            else
            {
                foreach (var item in out_Doors) item.Enabled = true;
                foreach (var item in in_Doors)
                {
                    item.CloseDoor();
                    if (item.Status == DoorStatus.Closed) item.Enabled = false;
                }
            }

        if ((buf.Contains("[in]") & buf.Contains($"[{grASTag.ToLower()}]"))
            || (buf.Contains("[in]") & buf.Contains("[all]")))
        {
            foreach (var obj in out_Doors)
                if (obj.Status != DoorStatus.Closed)
                {
                    foreach (var item in air_Vents) item.Depressurize = true;
                    foreach (var item in out_Doors)
                    {
                        item.Enabled = true;
                        item.CloseDoor();
                    }
                }
                else
                {
                    foreach (var item in air_Vents)
                    {
                        item.Depressurize = false;

                        if (item.GetOxygenLevel() >= 0.99f)
                            foreach (var inj in in_Doors)
                            {
                                inj.Enabled = true;
                                if (!buf.Contains("[$]")) inj.OpenDoor();
                            }
                    }
                }
        }

        if ((buf.Contains("[out]") & buf.Contains($"[{grASTag.ToLower()}]"))
            || (buf.Contains("[out]")) & buf.Contains("[all]"))
        {
            foreach (var obj in in_Doors)
                if (obj.Status != DoorStatus.Closed)
                {
                    foreach (var item in air_Vents) item.Depressurize = false;
                    foreach (var item in in_Doors)
                    {
                        item.Enabled = true;
                        item.CloseDoor();
                    }
                }
                else
                {
                    foreach (var item in air_Vents)
                    {
                        if (item.GetOxygenLevel() <= extPres)
                        {
                            item.Enabled = false;
                            item.Depressurize = false;

                            foreach (var inj in out_Doors)
                            {
                                inj.Enabled = true;
                                if (!buf.Contains("[$]")) inj.OpenDoor();
                            }
                        }
                        else
                        {
                            item.Depressurize = true;
                            item.Enabled = true;
                        }
                    }
                }
        }

        finish:
        for (int i = 0; i < stat_Info.Count; i++)
        {
            string display_Output = "";
            string[] param = stat_Info[i].CustomData.Trim(' ').ToLower().Split(new char[] { ' ', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
            bool sc_o = true;

            for (int j = 0; j < param.Length; j++)
            {
                switch (param[j])
                {
                    case "_": display_Output += $"\n"; break;
                    case "mode":
                        display_Output += $"{ Lng("mod", ln)}: ";
                        if (isAirlock) display_Output += $"{Lng("mod_on", ln)}\n"; else display_Output += $"{Lng("mod_off", ln)}\n";
                        sc_o = false;
                        break;
                    case "pressure":
                        string pt = "";
                        foreach (var item in air_Vents)
                            if (item.GetOxygenLevel() < extPres)
                            {
                                foreach (var inj in out_Doors)
                                    if (inj.Status != DoorStatus.Closed)
                                        pt = $"[{a_s(item)}] {Lng("as", ln)}: {Lng("depressurized", ln)} ({Lng("off_herm", ln)})\n";
                                    else
                                        pt = $"[{a_s(item)}] {Lng("as", ln)}: {Lng("depressurized", ln)} ({Lng("on_herm", ln)})\n";
                            }
                            else pt = $"[{a_s(item)}] {Lng("as", ln)}: {Lng(item.Status.ToString(), ln)} ({Lng("on_herm", ln)})\n";
                        display_Output += pt;
                        sc_o = false;
                        break;
                    case "level":
                        string vtl = "";
                        foreach (var item in air_Vents) vtl = $"[{a_f(item)}] {Lng("afs", ln)}: " +
                                $"{Math.Round(o_l_h(item), 4) * 100}%\n[{p_f(item)}]├{a_f_s(item)}┤\n";
                        display_Output += vtl;
                        sc_o = false;
                        break;
                    case "oxygen":
                        total_o = 0;
                        string otl = "";
                        foreach (var item in stor_Tanks)
                        {
                            total_o += item.FilledRatio / stor_Tanks.Count;
                            otl = $"[{o_f(item)}] {Lng("otfs", ln)}: {Math.Round(total_o, 4) * 100}%\n[{change(item)}]" +
                                $"├{new string('●', (int)(total_o * 42)).PadRight(42, '○')}┤\n";
                        }
                        display_Output += otl;
                        sc_o = false;
                        break;
                    case "oxy_all":
                        foreach (var item in stor_Tanks) display_Output += $"[{o_f(item)}] {item.CustomName}: " +
                                $"{Math.Round(item.FilledRatio, 4) * 100}%\n[{change(item)}]├{o_f_s(item)}┤\n";
                        sc_o = false;
                        break;
                    case "gasgen":
                        string gil = "";
                        total_g = 0;
                        wc = 0;
                        foreach (var item in gas_Gen)
                        {
                            if (item.IsWorking) wc += 1;
                            total_g += ((double)(item.GetInventory().CurrentVolume) / (double)(item.GetInventory().MaxVolume)) / gas_Gen.Count;
                            gil = $"[{g_f_t(wc)}] {Lng("aiig", ln)}: {Math.Round(total_g, 4) * 100}%\n[{g_f_t(wc)}]" +
                                $"├{new string('■', (int)(total_g * 42)).PadRight(42, '∙')}┤\n";
                        }
                        display_Output += gil;
                        sc_o = false;
                        break;
                    case "gen_all":
                        foreach (var item in gas_Gen) display_Output += $"[{g_f(item)}] {item.CustomName}: " +
                                $"{Math.Round(((double)(item.GetInventory().CurrentVolume) / (double)(item.GetInventory().MaxVolume)), 4) * 100}%\n" +
                                $"[{g_f(item)}]├{g_f_s(item)}┤\n";
                        sc_o = false;
                        break;
                    case "in":
                        string idt = "";
                        foreach (var item in in_Doors) idt = $"[{g_d(item)}] {Lng("in_ads", ln)}: {Lng(item.Status.ToString(), ln)}\n";
                        display_Output += idt;
                        sc_o = false;
                        break;
                    case "out":
                        string odt = "";
                        foreach (var item in out_Doors) odt = $"[{g_d(item)}] {Lng("out_ads", ln)}: {Lng(item.Status.ToString(), ln)}\n";
                        display_Output += odt;
                        sc_o = false;
                        break;
                    case "scada_mda":
                        // ------ Screen Grab For SCADA_MDA ------
                        (stat_Info[i] as IMyTerminalBlock).CustomData =
                            $"{Lng("help_params", ln)}\n#level\n#pressure\n#oxygen\n#oxy_all\n#in\n#out\n#gasgen\n#gen_all\n#mode\nscada_mda\n#scada_cga";
                        // ====== Screen Grab For SCADA_MDA ======
                        string ot = new string('X', 5), gg_s = new string('X', 3), af = new string('X', 7);
                        char gg = 'X', pvs = 'X', svs = 'X', ps = 'X', di = 'X', du = 'X', sdu_i = 'X', sdd_i = 'X', sdu_o = 'X', sdd_o = 'X';

                        total_o = 0;
                        foreach (var item in stor_Tanks)
                        {
                            total_o += item.FilledRatio / stor_Tanks.Count;
                            ot = new string('●', (int)(total_o * 5)).PadRight(5, '○');
                        }
                        total_g = 0;
                        wc = 0;
                        foreach (var item in gas_Gen)
                        {
                            total_g += ((double)(item.GetInventory().CurrentVolume) / (double)(item.GetInventory().MaxVolume)) / gas_Gen.Count;
                            gg_s = new string('■', (int)(total_g * 3)).PadRight(3, '∙');
                            if (item.IsWorking) wc += 1;
                            gg = g_f_t(wc);
                        }
                        foreach (var item in air_Vents) { af = scada_a_f_s(item); pvs = scada_v_s(item); ps = scada_p_s(item); }
                        foreach (var item in swap_Vents) svs = scada_v_s(item);
                        foreach (var item in in_Doors) { di = g_d(item); sdu_i = scada_s_d_u(item); sdd_i = scada_s_d_d(item); }
                        foreach (var item in out_Doors) { du = g_d(item); sdu_o = scada_s_d_u(item); sdd_o = scada_s_d_d(item); }

                        display_Output +=
                            $" 	 	 	 	 	┌	─	─	─	─	─	─	─	─	─	─	┐	 	 	 	 	 	 	 	 	 	 	\n" +
                            $" 	 	 	 	 	│	 	 	 	┌	─	─	─	─	─	┐	│	╔	═	═	═	═	═	╗	 	 	 	\n" +
                            $" 	 	╔	═	╦	╪	╦	═	╦	╪	╦	═	╗	 	 	│	└	╫	{ot}				║	O	₂	 	\n" +
                            $" 	{du}║	 	║	↕	║	 	║	↓	║	 	║{di}	 	│	 	╚	═	═	═	═	═	╝	 	 	 	\n" +
                            $" 	 	{sdu_o} ╚{pvs}	╝{ps}	╚{svs}	╝	 	{sdu_i}  	│	 	 	 	 	 	 	 	 	 	 	 	\n" +
                            $" 	 	{sdd_o}  	 	 	 	 	 	 	 	{sdd_i}  	│	 	╔	═	═	═	╦	═	╗	H	₂	 	\n" +
                            $" 	∞	║	☺	{af}						☻	║	⌂	 	└	─	╫	{gg_s}		╠	{gg}╣	↕	 	 	\n" +
                            $" 	 	╚	═	═	═	═	═	═	═	═	═	╝	 	 	 	 	╚	═	═	═	╩	═	╝	O	₂	 	\n";
                        break;
                    case "scada_cga":
                        // ------ Screen Grab For SCADA_CGA ------
                        (stat_Info[i] as IMyTerminalBlock).CustomData =
                            $"{Lng("help_params", ln)}\n#level\n#pressure\n#oxygen\n#oxy_all\n#in\n#out\n#gasgen\n#gen_all\n#mode\n#scada_mda\nscada_cga";
                        // ====== Screen Grab For SCADA_CGA ======
                        Draw((stat_Info[i] as IMyTextSurfaceProvider), in_Doors, out_Doors, stor_Tanks, gas_Gen, air_Vents, swap_Vents, pres_Vents,
                            ext_Vents, isAirlock, ln, 0, true);
                        break;
                }
            }

            if (sc_o == true) (stat_Info[i] as IMyTextSurfaceProvider).GetSurface(0).FontSize =
                    (stat_Info[i] as IMyTextSurfaceProvider).GetSurface(0).FontSize * 1.8f;

            if ((stat_Info[i] as IMyTextSurfaceProvider).GetSurface(0).SurfaceSize.X == 1024)
                (stat_Info[i] as IMyTextSurfaceProvider).GetSurface(0).FontSize = (stat_Info[i] as IMyTextSurfaceProvider).GetSurface(0).FontSize * 2;

            TXT_LCD_Separator(root, (stat_Info[i] as IMyTextSurfaceProvider), display_Output);
        }

        return $"«««=»»»\n[{grASTag}]\n{Lng("lights", ln)}:\n{Lng("pi", ln)} - {airlock_Lights.Count}\n{Lng("fl", ln)} - {airlock_Flashers.Count}\n" +
            $"{Lng("odi", ln)} - {out_Indicators.Count}\n{Lng("idi", ln)} - {in_Indicators.Count}\n{Lng("spk", ln)}: {air_Speakers.Count}\n" +
            $"{Lng("av", ln)}:\n{Lng("bv", ln)} - {air_Vents.Count}\n{Lng("sv", ln)} - {swap_Vents.Count}\n{Lng("tanks", ln)}: {stor_Tanks.Count}\n" +
            $"{Lng("gen", ln)}: {gas_Gen.Count}\n{Lng("dors", ln)}:\n{Lng("id", ln)} - {in_Doors.Count}\n{Lng("od", ln)} - {out_Doors.Count}\n" +
            $"{Lng("mon", ln)}: {stat_Info.Count}\n{Lng("pres_vent", ln)}: {pres_Vents.Count}\n{Lng("ext_vent", ln)}: {ext_Vents.Count}\n";
    }

    /// <summary>
            /// Draw SCADA CGA
            /// </summary>
            /// <param name="surface">Surface Draw</param>
            /// <param name="in_Doors">List Inner Doors</param>
            /// <param name="out_Doors">List Exterior Doors</param>
            /// <param name="tanks_Gas">List Tanks</param>
            /// <param name="gas_Gen">List Gas Generators</param>
            /// <param name="air_Vents">List Airlock Vents</param>
            /// <param name="swap_Vents">List Swap Vents</param>
            /// <param name="pres_Vents">List Inner Vent - Sensors</param>
            /// <param name="ext_Vents">List External Vent - Sensors</param>
            /// <param name="isAirlock">Is The Airlock Active</param>
            /// <param name="ln">Language</param>
            /// <param name="sur">Screen Number</param>
            /// <param name="color_Brigh"> Brightness Of Color</param>
    private void Draw(IMyTextSurfaceProvider surface, List<IMyDoor> in_Doors, List<IMyDoor> out_Doors, List<IMyGasTank> tanks_Gas,
        List<IMyGasGenerator> gas_Gen, List<IMyAirVent> air_Vents, List<IMyAirVent> swap_Vents, List<IMyAirVent> pres_Vents, List<IMyAirVent> ext_Vents,
        bool isAirlock, string ln = "", int sur = 0, bool color_Brigh = false)
    {
        int c_b = color_Brigh ? 2 : 1;

        sur = sur < 0 || sur > surface.SurfaceCount - 1 ? 0 : sur;

        float scale = 1, bias_Y = 0;

        if (surface.GetSurface(sur).TextureSize.X == 512) { scale = 2; bias_Y = 250; }
        if (surface.GetSurface(sur).TextureSize.X == 256) { scale = 4; bias_Y = 250; }

        surface.GetSurface(sur).ContentType = ContentType.SCRIPT;
        surface.GetSurface(sur).Script = "";
        surface.GetSurface(sur).ScriptBackgroundColor = new Color(0, 0, 0);
        surface.GetSurface(sur).ScriptForegroundColor = new Color(255, 255, 0);

        double filRat_O = 0, vol_O = 0;
        foreach (var item in tanks_Gas)
        {
            filRat_O += item.FilledRatio;
            vol_O += Convert.ToDouble(
                System.Text.RegularExpressions.Regex.Match(item.DetailedInfo.Split('/')[1], @"[\d]*\.?[\d]+([eE][-+][\d]+)?").ToString());
        }

        bool gen_Toogle = false;
        foreach (var item in gas_Gen) if (item.Enabled) gen_Toogle = true;

        double filRat_H_O = 0, mas_H_O = 0;
        foreach (var item in gas_Gen)
        {
            filRat_H_O += (double)item.GetInventory().CurrentVolume / (double)item.GetInventory().MaxVolume;
            mas_H_O += Math.Round((double)item.GetInventory().CurrentMass, 2);
        }

        bool in_Enable = false, in_Opening = false, in_Closing = false;
        float in_Ratio = 0;
        foreach (var item in in_Doors)
        {
            if (item.Enabled) in_Enable = item.Enabled;
            if (item.Status == DoorStatus.Open) in_Ratio = item.OpenRatio;

            if (item.Status == DoorStatus.Opening || item.Status == DoorStatus.Closing)
            {
                in_Opening = item.Status == DoorStatus.Opening;
                in_Closing = item.Status == DoorStatus.Closing;
                in_Ratio = item.OpenRatio;
            }
        }

        bool out_Enable = false, out_Opening = false, out_Closing = false;
        float out_Ratio = 0;
        foreach (var item in out_Doors)
        {
            if (item.Enabled) out_Enable = item.Enabled;
            if (item.Status == DoorStatus.Open) out_Ratio = item.OpenRatio;

            if (item.Status == DoorStatus.Opening || item.Status == DoorStatus.Closing)
            {
                out_Opening = item.Status == DoorStatus.Opening;
                out_Closing = item.Status == DoorStatus.Closing;
                out_Ratio = item.OpenRatio;
            }
        }

        bool[] all_Active = new bool[] { false, false, false };
        Color[] av_Status = new Color[] { new Color(50 * c_b, 0 * c_b, 0 * c_b), new Color(50 * c_b, 0 * c_b, 0 * c_b),
            new Color(50 * c_b, 0 * c_b, 0 * c_b) };

        double oxy_Lev_R = 0, oxy_Lev_A = 0, oxy_Lev_E = 0;

        bool av_Herm = false;
        foreach (var item in air_Vents)
        {
            if (item.Enabled && item.CanPressurize)
            {
                all_Active[0] = true;
                av_Status[0] = item.Status == VentStatus.Pressurized || item.Status == VentStatus.Pressurizing ? new Color(0 * c_b, 50 * c_b, 0 * c_b) :
                    new Color(0 * c_b, 50 * c_b, 50 * c_b);
            }
            else if (item.Enabled && !item.CanPressurize) av_Status[0] = new Color(50 * c_b, 50 * c_b, 0 * c_b);

            if (item.Enabled) oxy_Lev_A += item.GetOxygenLevel();
            else
                oxy_Lev_A += System.Text.RegularExpressions.Regex.Match(item.DetailedInfo.Split('\n')[2], @"[\d]*\.?[\d]+([eE][-+][\d]+)?").ToString() == ""
                    ? -1 : Convert.ToDouble(System.Text.RegularExpressions.Regex.Match(
                        item.DetailedInfo.Split('\n')[2], @"[\d]*\.?[\d]+([eE][-+][\d]+)?").ToString()) / 100;

            if (item.CanPressurize) av_Herm = true;
        }

        foreach (var item in swap_Vents)
        {
            if (item.Enabled && item.CanPressurize)
            {
                all_Active[1] = true;
                av_Status[1] = item.Status == VentStatus.Pressurized || item.Status == VentStatus.Pressurizing ? new Color(0 * c_b, 50 * c_b, 0 * c_b) :
                    new Color(0 * c_b, 50 * c_b, 50 * c_b);
            }
            else if (item.Enabled && !item.CanPressurize) av_Status[1] = new Color(50 * c_b, 50 * c_b, 0 * c_b);
        }

        bool isHerm = false;
        foreach (var item in pres_Vents)
        {
            if (item.CanPressurize) isHerm = true;

            if (item.Enabled) oxy_Lev_R += item.GetOxygenLevel();
            else
                oxy_Lev_R += System.Text.RegularExpressions.Regex.Match(item.DetailedInfo.Split('\n')[2], @"[\d]*\.?[\d]+([eE][-+][\d]+)?").ToString() == ""
                    ? 0 : Convert.ToDouble(System.Text.RegularExpressions.Regex.Match(
                        item.DetailedInfo.Split('\n')[2], @"[\d]*\.?[\d]+([eE][-+][\d]+)?").ToString()) / 100;
        }

        foreach (var item in ext_Vents) oxy_Lev_E += Math.Round(item.GetOxygenLevel() / ext_Vents.Count, 2);

        // --- TRANSITION ROOM ---
        var armor = MySprite.CreateSprite("SquareSimple", new Vector2(750, 256 + bias_Y) / scale, new Vector2(450, 450) / scale);
        armor.Color = new Color(30 * c_b, 30 * c_b, 30 * c_b);

        var room_Space = MySprite.CreateSprite("SquareSimple", new Vector2(750, 256 + bias_Y) / scale, new Vector2(350, 350) / scale);
        room_Space.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        var doorway = MySprite.CreateSprite("SquareSimple", new Vector2(750, 256 + bias_Y) / scale, new Vector2(450, 100) / scale);
        doorway.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        var gauge_Room = MySprite.CreateSprite("SquareSimple", new Vector2(730, 256 + bias_Y) / scale, new Vector2(264, 42) / scale);
        gauge_Room.Color = new Color(0 * c_b, 25 * c_b, 50 * c_b);

        var blank_Gauge_R = MySprite.CreateSprite("SquareSimple", new Vector2(730, 256 + bias_Y) / scale, new Vector2(260, 38) / scale);
        blank_Gauge_R.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        var progress_Gauge = MySprite.CreateSprite("SquareSimple", new Vector2(604, 256 + bias_Y) / scale,
            new Vector2(Convert.ToSingle(Math.Abs(oxy_Lev_A / air_Vents.Count) * 252), 30) / scale);
        progress_Gauge.Color = (oxy_Lev_A / air_Vents.Count) > 0.8 ? new Color(0 * c_b, 50 * c_b, 0 * c_b) : (oxy_Lev_A / air_Vents.Count) >= 0.5 ?
            new Color(50, 50, 0) : (oxy_Lev_A / air_Vents.Count) < 0 ? new Color(50 * c_b, 25 * c_b, 0 * c_b) : new Color(50 * c_b, 0 * c_b, 0 * c_b);
        progress_Gauge.Alignment = TextAlignment.LEFT;

        string progress_Text = oxy_Lev_A > 0 & av_Herm ? $"{Lng("fil", ln)} {Math.Round(Math.Abs(oxy_Lev_A / air_Vents.Count) * 100, 1)}%" :
            $"{Lng("herm", ln)}";

        var shadow_Gauge = MySprite.CreateText(progress_Text, "Debug", new Color(0 * c_b, 0 * c_b, 255 * c_b), (float)1 / scale);
        shadow_Gauge.Position = new Vector2(731, 242 + bias_Y) / scale;

        var text_Gauge = MySprite.CreateText(progress_Text, "Debug", new Color(90 * c_b, 90 * c_b, 90 * c_b), (float)1 / scale);
        text_Gauge.Position = new Vector2(730, 241 + bias_Y) / scale;

        // --- OXYGEN TANK ---
        var shell_Oxy = MySprite.CreateSprite("SquareSimple", new Vector2(250, 130 + bias_Y) / scale, new Vector2(350, 200) / scale);
        shell_Oxy.Color = new Color(0 * c_b, 25 * c_b, 50 * c_b);

        var shell_Left = MySprite.CreateSprite("SemiCircle", new Vector2(80, 130 + bias_Y) / scale, new Vector2(200, 100) / scale);
        shell_Left.Color = new Color(0 * c_b, 25 * c_b, 50 * c_b);
        shell_Left.RotationOrScale = -1.5708f;

        var blank_Left = MySprite.CreateSprite("SemiCircle", new Vector2(80, 130 + bias_Y) / scale, new Vector2(180, 80) / scale);
        blank_Left.Color = new Color(1 * c_b, 1 * c_b, 3 * c_b);
        blank_Left.RotationOrScale = -1.5708f;

        var shell_Right = MySprite.CreateSprite("SemiCircle", new Vector2(420, 130 + bias_Y) / scale, new Vector2(200, 100) / scale);
        shell_Right.Color = new Color(0 * c_b, 25 * c_b, 50 * c_b);
        shell_Right.RotationOrScale = 1.5708f;

        var blank_Right = MySprite.CreateSprite("SemiCircle", new Vector2(420, 130 + bias_Y) / scale, new Vector2(180, 80) / scale);
        blank_Right.Color = new Color(1 * c_b, 1 * c_b, 3 * c_b);
        blank_Right.RotationOrScale = 1.5708f;

        var blank_Oxy = MySprite.CreateSprite("SquareSimple", new Vector2(250, 130 + bias_Y) / scale, new Vector2(350, 180) / scale);
        blank_Oxy.Color = new Color(1 * c_b, 1 * c_b, 3 * c_b);

        var tank_Oxy = MySprite.CreateSprite("IconOxygen", new Vector2(250, 92 + bias_Y) / scale, new Vector2(100, 100) / scale);
        tank_Oxy.Color = new Color(0 * c_b, 25 * c_b, 50 * c_b);

        var gauge_Oxy = MySprite.CreateSprite("SquareSimple", new Vector2(250, 175 + bias_Y) / scale, new Vector2(352, 42) / scale);
        gauge_Oxy.Color = new Color(0 * c_b, 25 * c_b, 50 * c_b);

        var blank_Gauge_O = MySprite.CreateSprite("SquareSimple", new Vector2(250, 175 + bias_Y) / scale, new Vector2(348, 38) / scale);
        blank_Gauge_O.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        var progress_Oxy = MySprite.CreateSprite("SquareSimple", new Vector2(80, 175 + bias_Y) / scale,
            new Vector2(Convert.ToSingle(filRat_O / tanks_Gas.Count * 340), 30) / scale);
        progress_Oxy.Color = (filRat_O / tanks_Gas.Count) > 0.99 ? new Color(0 * c_b, 25 * c_b, 50 * c_b) : (filRat_O / tanks_Gas.Count) >= 0.75 ?
            new Color(0 * c_b, 50 * c_b, 0 * c_b) : (filRat_O / tanks_Gas.Count) >= 0.5 ? new Color(50 * c_b, 50 * c_b, 0 * c_b) :
            (filRat_O / tanks_Gas.Count) >= 0.25 ? new Color(50 * c_b, 25 * c_b, 0 * c_b) : new Color(50 * c_b, 0 * c_b, 0 * c_b);
        progress_Oxy.Alignment = TextAlignment.LEFT;

        var shadow_Gauge_O = MySprite.CreateText($"{Lng("fil", ln)} {Math.Round((filRat_O / tanks_Gas.Count) * 100, 1)}%", "Debug",
            new Color(0 * c_b, 0 * c_b, 255 * c_b), (float)1 / scale);
        shadow_Gauge_O.Position = new Vector2(251, 161 + bias_Y) / scale;

        var text_Gauge_O = MySprite.CreateText($"{Lng("fil", ln)} {Math.Round((filRat_O / tanks_Gas.Count) * 100, 1)}%", "Debug",
            new Color(90 * c_b, 90 * c_b, 90 * c_b), (float)1 / scale);
        text_Gauge_O.Position = new Vector2(250, 160 + bias_Y) / scale;

        var text_Vol_O = MySprite.CreateText($"{Lng("vol", ln)}:\n{vol_O} {Lng("l", ln)}", "Debug", new Color(90 * c_b, 90 * c_b, 90 * c_b),
            (float)1 / scale);
        text_Vol_O.Position = new Vector2(130, 70 + bias_Y) / scale;

        var text_Qty_O = MySprite.CreateText($"{Lng("qua", ln)}:\n{tanks_Gas.Count}x", "Debug", new Color(90 * c_b, 90 * c_b, 90 * c_b),
            (float)1 / scale);
        text_Qty_O.Position = new Vector2(365, 70 + bias_Y) / scale;

        // --- ELECTROLYZER ---
        var border_L_T = MySprite.CreateSprite("RightTriangle", new Vector2(65, 295 + bias_Y) / scale, new Vector2(40, -40) / scale);
        border_L_T.Color = new Color(0 * c_b, 50 * c_b, 50 * c_b);

        var border_L_B = MySprite.CreateSprite("RightTriangle", new Vector2(65, 465 + bias_Y) / scale, new Vector2(40, 40) / scale);
        border_L_B.Color = new Color(0 * c_b, 50 * c_b, 50 * c_b);

        var border_R_B = MySprite.CreateSprite("RightTriangle", new Vector2(435, 465 + bias_Y) / scale, new Vector2(-40, 40) / scale);
        border_R_B.Color = new Color(0 * c_b, 50 * c_b, 50 * c_b);

        var border_R_T = MySprite.CreateSprite("RightTriangle", new Vector2(435, 295 + bias_Y) / scale, new Vector2(-40, -40) / scale);
        border_R_T.Color = new Color(0 * c_b, 50 * c_b, 50 * c_b);

        var shell_Gen = MySprite.CreateSprite("SquareSimple", new Vector2(250, 380 + bias_Y) / scale, new Vector2(400, 200) / scale);
        shell_Gen.Color = new Color(0 * c_b, 50 * c_b, 50 * c_b);

        var blank_Gen = MySprite.CreateSprite("SquareSimple", new Vector2(250, 380 + bias_Y) / scale, new Vector2(380, 180) / scale);
        blank_Gen.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        var ico_Gen = MySprite.CreateSprite("IconEnergy", new Vector2(410, 380 + bias_Y) / scale, new Vector2(100, 180) / scale);
        ico_Gen.Color = gen_Toogle ? (filRat_H_O != 0 ? new Color(0 * c_b, 50 * c_b, 50 * c_b) : new Color(50 * c_b, 50 * c_b, 0 * c_b)) :
            new Color(50 * c_b, 0 * c_b, 0 * c_b);

        var split_Gen = MySprite.CreateSprite("SquareSimple", new Vector2(375, 380 + bias_Y) / scale, new Vector2(10, 180) / scale);
        split_Gen.Color = new Color(0 * c_b, 50 * c_b, 50 * c_b);

        var gen_Hyd = MySprite.CreateSprite("IconHydrogen", new Vector2(100, 320 + bias_Y) + bias_Y / scale, new Vector2(60, 60) / scale);
        gen_Hyd.Color = new Color(0 * c_b, 25 * c_b, 0 * c_b);

        var shadow_Ice = MySprite.CreateText($"«   {Lng("ice", ln)}   »", "Debug", new Color(0 * c_b, 0 * c_b, 255 * c_b), (float)1.5 / scale);
        shadow_Ice.Position = new Vector2(216, 296 + bias_Y) / scale;

        var text_Ice = MySprite.CreateText($"«   {Lng("ice", ln)}   »", "Debug", new Color(0 * c_b, 90 * c_b, 90 * c_b), (float)1.5 / scale);
        text_Ice.Position = new Vector2(215, 295 + bias_Y) / scale;

        var gen_Oxy = MySprite.CreateSprite("IconOxygen", new Vector2(330, 320 + bias_Y) / scale, new Vector2(60, 60) / scale);
        gen_Oxy.Color = new Color(0 * c_b, 25 * c_b, 50 * c_b);

        var ico_El_H_O = MySprite.CreateSprite("IconEnergy", new Vector2(DateTime.Now.Second % 2 == 0 ? 165 : 265, 320 + bias_Y) / scale,
            new Vector2(40, 50) / scale);
        ico_El_H_O.Color = new Color(50 * c_b, 50 * c_b, 0 * c_b);

        var gauge_H_O = MySprite.CreateSprite("SquareSimple", new Vector2(215, 440 + bias_Y) / scale, new Vector2(280, 35) / scale);
        gauge_H_O.Color = new Color(0 * c_b, 50 * c_b, 50 * c_b);

        var blank_Gauge_H_O = MySprite.CreateSprite("SquareSimple", new Vector2(215, 440 + bias_Y) / scale, new Vector2(276, 31) / scale);
        blank_Gauge_H_O.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        var progress_Gen = MySprite.CreateSprite("SquareSimple", new Vector2(80, 440 + bias_Y) / scale,
            new Vector2(Convert.ToSingle(filRat_H_O / gas_Gen.Count * 270), 25) / scale);
        progress_Gen.Color = (filRat_H_O / gas_Gen.Count) > 0.99 ? new Color(0 * c_b, 50 * c_b, 50 * c_b) : (filRat_H_O / gas_Gen.Count) >= 0.75 ?
            new Color(0 * c_b, 50 * c_b, 0 * c_b) : (filRat_H_O / gas_Gen.Count) >= 0.5 ? new Color(50 * c_b, 50 * c_b, 0 * c_b) :
            (filRat_H_O / gas_Gen.Count) >= 0.25 ? new Color(50 * c_b, 25 * c_b, 0 * c_b) : new Color(50 * c_b, 0 * c_b, 0 * c_b);
        progress_Gen.Alignment = TextAlignment.LEFT;

        var shadow_Gauge_H_O = MySprite.CreateText($"{Lng("fil", ln)} {Math.Round((filRat_H_O / gas_Gen.Count) * 100, 1)}%", "Debug",
            new Color(0 * c_b, 0 * c_b, 255 * c_b), (float)0.85 / scale);
        shadow_Gauge_H_O.Position = new Vector2(221, 427 + bias_Y) / scale;

        var text_Gauge_H_O = MySprite.CreateText($"{Lng("fil", ln)} {Math.Round((filRat_H_O / gas_Gen.Count) * 100, 1)}%", "Debug",
            new Color(90 * c_b, 90 * c_b, 90 * c_b), (float)0.85 / scale);
        text_Gauge_H_O.Position = new Vector2(220, 426 + bias_Y) / scale;

        var text_Vol_H_O = MySprite.CreateText($"{Lng("mas", ln)}:\n{mas_H_O} {Lng("m", ln)}", "Debug",
            new Color(90 * c_b, 90 * c_b, 90 * c_b), (float)0.85 / scale);
        text_Vol_H_O.Position = new Vector2(150, 360 + bias_Y) / scale;

        var text_Qty_H_O = MySprite.CreateText($"{Lng("qua", ln)}:\n{gas_Gen.Count}x", "Debug", new Color(90 * c_b, 90 * c_b, 90 * c_b),
            (float)0.85 / scale);
        text_Qty_H_O.Position = new Vector2(290, 360 + bias_Y) / scale;

        // --- AIR VENT ---
        float y_AV = 0;

        var all_AV = new List<MySprite>();

        var label_AV = new string[]
        {
            $"{Lng("all_vent", ln)}:\n{air_Vents.Count}x",
            $"{Lng("swap_vent", ln)}:\n{swap_Vents.Count}x"
        };

        for (int type = 0; type < 1 + (swap_Vents.Count != 0 ? 1 : 0); type++)
        {
            var av_Pos = new float[,]       // [0] - Airlock - Air Vents
            {
                { 650, 155 + y_AV },        // [0] - body_AV, hole_AV, rim_AV, hub_AV, zenith_AV
                { 650, 135 + y_AV },        // [1] - blade_Fan_1
                { 650, 175 + y_AV },        // [2] - blade_Fan_2
                { 670, 155 + y_AV },        // [3] - blade_Fan_3
                { 630, 155 + y_AV },        // [4] - blade_Fan_4
                { 650, 105 + y_AV },        // [5] - pins_AV_T
                { 650, 185 + y_AV },        // [6] - pins_AV_B
                { 800, 110 + y_AV }         // [7] - text_AV
            };

            var body_AV = MySprite.CreateSprite("SquareTapered", new Vector2(av_Pos[0, 0], av_Pos[0, 1] + bias_Y) / scale,
                new Vector2(100, 100) / scale);
            body_AV.Color = new Color(10 * c_b, 30 * c_b, 50 * c_b);

            var hole_AV = MySprite.CreateSprite("Circle", new Vector2(av_Pos[0, 0], av_Pos[0, 1] + bias_Y) / scale, new Vector2(85, 85) / scale);
            hole_AV.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

            var rim_AV = MySprite.CreateSprite("Circle", new Vector2(av_Pos[0, 0], av_Pos[0, 1] + bias_Y) / scale, new Vector2(75, 75) / scale);
            rim_AV.Color = av_Status[type];

            var blade_Fan_1 = MySprite.CreateSprite("SemiCircle", new Vector2(all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[1, 0] :
                av_Pos[1, 0] + 15f : av_Pos[1, 0], (all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[1, 1] : av_Pos[1, 1] + 5f :
                av_Pos[1, 1]) + bias_Y) / scale, new Vector2(40, -40) / scale);
            blade_Fan_1.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);
            blade_Fan_1.RotationOrScale = all_Active[type] ? DateTime.Now.Second % 2 == 0 ? 1.5708f : 1.5708f + 0.785398f : 1.5708f;

            var blade_Fan_2 = MySprite.CreateSprite("SemiCircle", new Vector2(all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[2, 0] :
                av_Pos[2, 0] - 15f : av_Pos[2, 0], (all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[2, 1] : av_Pos[2, 1] - 5f :
                av_Pos[2, 1]) + bias_Y) / scale, new Vector2(40, 40) / scale);
            blade_Fan_2.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);
            blade_Fan_2.RotationOrScale = all_Active[type] ? DateTime.Now.Second % 2 == 0 ? 1.5708f : 1.5708f + 0.785398f : 1.5708f;

            var blade_Fan_3 = MySprite.CreateSprite("SemiCircle", new Vector2(all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[3, 0] :
                av_Pos[3, 0] - 5f : av_Pos[3, 0], (all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[3, 1] : av_Pos[3, 1] + 15f :
                av_Pos[3, 1]) + bias_Y) / scale, new Vector2(40, 40) / scale);
            blade_Fan_3.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);
            blade_Fan_3.RotationOrScale = all_Active[type] ? DateTime.Now.Second % 2 == 0 ? 0 : 0.785398f : 0;

            var blade_Fan_4 = MySprite.CreateSprite("SemiCircle", new Vector2(all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[4, 0] :
                av_Pos[4, 0] + 5f : av_Pos[4, 0], (all_Active[type] ? DateTime.Now.Second % 2 == 0 ? av_Pos[4, 1] : av_Pos[4, 1] - 15f :
                av_Pos[4, 1]) + bias_Y) / scale, new Vector2(40, -40) / scale);
            blade_Fan_4.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);
            blade_Fan_4.RotationOrScale = all_Active[type] ? DateTime.Now.Second % 2 == 0 ? 0 : 0.785398f : 0;

            var hub_AV = MySprite.CreateSprite("Circle", new Vector2(av_Pos[0, 0], av_Pos[0, 1] + bias_Y) / scale, new Vector2(17, 17) / scale);
            hub_AV.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

            var zenith_AV = MySprite.CreateSprite("Circle", new Vector2(av_Pos[0, 0], av_Pos[0, 1] + bias_Y) / scale, new Vector2(5, 5) / scale);
            zenith_AV.Color = new Color(255 * c_b, 255 * c_b, 255 * c_b);

            var pins_AV_T = MySprite.CreateText("¤              ¤", "Debug", new Color(0 * c_b, 0 * c_b, 90 * c_b), (float)0.7 / scale);
            pins_AV_T.Position = new Vector2(av_Pos[5, 0], av_Pos[5, 1] + bias_Y) / scale;

            var pins_AV_B = MySprite.CreateText("¤              ¤", "Debug", new Color(0 * c_b, 0 * c_b, 90 * c_b), (float)0.7 / scale);
            pins_AV_B.Position = new Vector2(av_Pos[6, 0], av_Pos[6, 1] + bias_Y) / scale;

            var text_AV = MySprite.CreateText(label_AV[type], "Debug", new Color(90 * c_b, 90 * c_b, 90 * c_b), (float)1 / scale);
            text_AV.Position = new Vector2(av_Pos[7, 0], av_Pos[7, 1] + bias_Y) / scale;

            all_AV.Add(body_AV);
            all_AV.Add(hole_AV);
            all_AV.Add(rim_AV);
            all_AV.Add(blade_Fan_1);
            all_AV.Add(blade_Fan_2);
            all_AV.Add(blade_Fan_3);
            all_AV.Add(blade_Fan_4);
            all_AV.Add(hub_AV);
            all_AV.Add(zenith_AV);
            all_AV.Add(pins_AV_T);
            all_AV.Add(pins_AV_B);
            all_AV.Add(text_AV);

            y_AV += 200;
        }

        // --- DOORS ---
        var in_Door_Closed = MySprite.CreateSprite("SquareSimple", new Vector2(550, 256 + bias_Y) / scale, new Vector2(30, 100) / scale);
        in_Door_Closed.Color = in_Opening ? new Color(50 * c_b, 50 * c_b, 0 * c_b) : in_Closing ? new Color(50 * c_b, 25 * c_b, 0 * c_b) : in_Enable ?
            new Color(0 * c_b, 50 * c_b, 0 * c_b) : new Color(50 * c_b, 0 * c_b, 0 * c_b);

        var out_Door_Cloded = MySprite.CreateSprite("SquareSimple", new Vector2(950, 256 + bias_Y) / scale, new Vector2(30, 100) / scale);
        out_Door_Cloded.Color = out_Opening ? new Color(50 * c_b, 50 * c_b, 0 * c_b) : out_Closing ? new Color(50 * c_b, 25 * c_b, 0 * c_b) :
            out_Enable ? new Color(0 * c_b, 50 * c_b, 0 * c_b) : new Color(50 * c_b, 0 * c_b, 0 * c_b);

        var in_Door_Anim = MySprite.CreateSprite("SquareSimple", new Vector2(550, 256 + bias_Y) / scale, new Vector2(30, -in_Ratio * 100) / scale);
        in_Door_Anim.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        var out_Door_Anim = MySprite.CreateSprite("SquareSimple", new Vector2(950, 256 + bias_Y) / scale, new Vector2(30, -out_Ratio * 100) / scale);
        out_Door_Anim.Color = new Color(0 * c_b, 0 * c_b, 0 * c_b);

        // --- ICONS ---
        var in_Oxy = MySprite.CreateSprite(pres_Vents.Count != 0 ? "IconOxygen" : "Cross", new Vector2(500, 253 + bias_Y) / scale,
            new Vector2(50, 50) / scale);
        in_Oxy.Color = isHerm ? (oxy_Lev_R / pres_Vents.Count >= 0.8 ? new Color(0 * c_b, 50 * c_b, 0 * c_b) : oxy_Lev_R / pres_Vents.Count >= 0.5 ?
            new Color(50 * c_b, 50 * c_b, 0 * c_b) : new Color(50 * c_b, 0 * c_b, 0 * c_b)) : new Color(100 * c_b, 50 * c_b, 0 * c_b);

        var airlock_Oxy = MySprite.CreateSprite("IconOxygen", new Vector2(900, 253 + bias_Y) / scale,
            new Vector2(50, 50) / scale);
        airlock_Oxy.Color = isAirlock ? (oxy_Lev_A / air_Vents.Count >= 0.8 ? new Color(0 * c_b, 50 * c_b, 0 * c_b) : oxy_Lev_A / air_Vents.Count >= 0.5 ?
            new Color(50 * c_b, 50 * c_b, 0 * c_b) : oxy_Lev_A / air_Vents.Count < 0 ? new Color(100 * c_b, 50 * c_b, 0 * c_b) :
            new Color(50 * c_b, 0 * c_b, 0 * c_b)) : new Color(100 * c_b, 50 * c_b, 0 * c_b);

        var out_Oxy = MySprite.CreateSprite(ext_Vents.Count != 0 ? "IconOxygen" : "Cross", new Vector2(1000, 253 + bias_Y) / scale,
            new Vector2(50, 50) / scale);
        out_Oxy.Color = oxy_Lev_E >= 0.8 ? new Color(0 * c_b, 50 * c_b, 0 * c_b) : oxy_Lev_E >= 0.5 ? new Color(50 * c_b, 50 * c_b, 0 * c_b) :
            new Color(50 * c_b, 0 * c_b, 0 * c_b);

        int x_Co = 580, y_Co = 155;

        var all_Conv = new List<MySprite>();

        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var conv_Bord = MySprite.CreateSprite("SquareSimple", new Vector2(x_Co, y_Co + bias_Y) / scale, new Vector2(50, 30) / scale);
                conv_Bord.Color = new Color(10 * c_b, 10 * c_b, 10 * c_b);

                var conv_Body = MySprite.CreateSprite("SquareSimple", new Vector2(x_Co, y_Co + bias_Y) / scale, new Vector2(50, 20) / scale);
                conv_Body.Color = new Color(20 * c_b, 20 * c_b, 20 * c_b);

                var conv_Joint_R = MySprite.CreateSprite("SquareSimple", new Vector2(x_Co + 20, y_Co + bias_Y) / scale, new Vector2(10, 40) / scale);
                conv_Joint_R.Color = new Color(30 * c_b, 20 * c_b, 0 * c_b);

                var conv_Joint_L = MySprite.CreateSprite("SquareSimple", new Vector2(x_Co - 20, y_Co + bias_Y) / scale, new Vector2(10, 40) / scale);
                conv_Joint_L.Color = new Color(30 * c_b, 20 * c_b, 0 * c_b);

                var conv_Ind_T = MySprite.CreateSprite("SquareSimple", new Vector2(x_Co, (y_Co - 12) + bias_Y) / scale, new Vector2(10, 3) / scale);
                conv_Ind_T.Color = new Color(0 * c_b, 50 * c_b, 0 * c_b);

                var conv_Ind_B = MySprite.CreateSprite("SquareSimple", new Vector2(x_Co, (y_Co + 13) + bias_Y) / scale, new Vector2(10, 3) / scale);
                conv_Ind_B.Color = new Color(0 * c_b, 50 * c_b, 0 * c_b);

                var conv_Txt = MySprite.CreateText("><", "Debug", new Color(10 * c_b, 10 * c_b, 10 * c_b), (float)0.8 / scale);
                conv_Txt.Position = new Vector2(x_Co, (y_Co - 14) + bias_Y) / scale;

                all_Conv.Add(conv_Bord);
                all_Conv.Add(conv_Body);
                all_Conv.Add(conv_Joint_R);
                all_Conv.Add(conv_Joint_L);
                all_Conv.Add(conv_Ind_T);
                all_Conv.Add(conv_Ind_B);
                all_Conv.Add(conv_Txt);

                x_Co -= 40;
            }
            x_Co = 580;
            y_Co += 200;
        }

        using (var frame = surface.GetSurface(sur).DrawFrame())
        {
            // --- TRANSITION ROOM ---
            frame.Add(armor);
            frame.Add(room_Space);
            frame.Add(doorway);
            frame.Add(gauge_Room);
            frame.Add(blank_Gauge_R);
            frame.Add(progress_Gauge);
            frame.Add(shadow_Gauge);
            frame.Add(text_Gauge);
            // --- CONVEYOR
            frame.AddRange(all_Conv);
            // --- ICONS ---
            frame.Add(in_Oxy);
            frame.Add(airlock_Oxy);
            frame.Add(out_Oxy);
            // --- DOORS ---
            frame.Add(in_Door_Closed);
            frame.Add(out_Door_Cloded);
            frame.Add(in_Door_Anim);
            frame.Add(out_Door_Anim);
            // --- ELECTROLYZER ---
            if (gas_Gen.Count != 0)
            {
                frame.Add(border_L_T);
                frame.Add(border_L_B);
                frame.Add(border_R_B);
                frame.Add(border_R_T);
                frame.Add(shell_Gen);
                frame.Add(blank_Gen);
                frame.Add(ico_Gen);
                frame.Add(split_Gen);
                frame.Add(gen_Hyd);
                frame.Add(shadow_Ice);
                frame.Add(text_Ice);
                frame.Add(gen_Oxy);
                if (gen_Toogle & filRat_H_O != 0) frame.Add(ico_El_H_O);
                frame.Add(gauge_H_O);
                frame.Add(blank_Gauge_H_O);
                frame.Add(progress_Gen);
                frame.Add(shadow_Gauge_H_O);
                frame.Add(text_Gauge_H_O);
                frame.Add(text_Vol_H_O);
                frame.Add(text_Qty_H_O);
            }
            // --- OXYGEN TANK ---
            frame.Add(shell_Oxy);
            frame.Add(shell_Left);
            frame.Add(blank_Left);
            frame.Add(shell_Right);
            frame.Add(blank_Right);
            frame.Add(blank_Oxy);
            frame.Add(tank_Oxy);
            frame.Add(gauge_Oxy);
            frame.Add(blank_Gauge_O);
            frame.Add(progress_Oxy);
            frame.Add(shadow_Gauge_O);
            frame.Add(text_Gauge_O);
            frame.Add(text_Qty_O);
            frame.Add(text_Vol_O);
            // --- AIR VENT ---
            frame.AddRange(all_AV);
        }
    }

    /// <summary>
            /// Text Separator For Multiple LCDs
            /// </summary>
            /// <param name="root">Constant "root"</param>
            /// <param name="main_Surface">Main Surface</param>
            /// <param name="input_Text">Input Text</param>
    private void TXT_LCD_Separator(Program root, IMyTextSurfaceProvider main_Surface, string input_Text)
    {
        var sur_Parts = new List<IMyTextSurfaceProvider>();

        var text_Wrap = new List<StringBuilder>();
        text_Wrap.Add(new StringBuilder());

        var main_Mark = System.Text.RegularExpressions.Regex.Match((main_Surface as IMyTerminalBlock).CustomName.ToLower(), @"(?<=\[!).+?(?=])");

        root.GridTerminalSystem.GetBlocksOfType(sur_Parts, parts => (parts as IMyTerminalBlock).IsFunctional
                & (parts as IMyTerminalBlock).CustomName.ToLower().Contains($"[!{main_Mark}-"));

        sur_Parts = sur_Parts.OrderBy(
            x => System.Text.RegularExpressions.Regex.Match((x as IMyTerminalBlock).CustomName.ToLower(), @"-\d+\]").ToString()).ToList();

        foreach (var item in sur_Parts)
        {
            item.GetSurface(0).WriteText("");
            item.GetSurface(0).ContentType = main_Surface.GetSurface(0).ContentType;
            item.GetSurface(0).FontSize = main_Surface.GetSurface(0).FontSize;
            item.GetSurface(0).Font = main_Surface.GetSurface(0).Font;
            item.GetSurface(0).FontColor = main_Surface.GetSurface(0).FontColor;
            item.GetSurface(0).Alignment = main_Surface.GetSurface(0).Alignment;
            item.GetSurface(0).TextPadding = main_Surface.GetSurface(0).TextPadding;
            item.GetSurface(0).BackgroundColor = main_Surface.GetSurface(0).BackgroundColor;
            if (main_Surface.GetSurface(0).CurrentlyShownImage != null)
                item.GetSurface(0).AddImageToSelection(main_Surface.GetSurface(0).CurrentlyShownImage);
            else
                item.GetSurface(0).ClearImagesFromSelection();
        }

        var scr_Y = main_Surface.GetSurface(0).SurfaceSize.Y;

        var frag = 0;
        double txt_Y = 0;

        for (int i = 0; i < input_Text.Split('\n').Length; i++)
        {
            txt_Y = main_Surface.GetSurface(0).MeasureStringInPixels(text_Wrap[frag],
                main_Surface.GetSurface(0).Font, main_Surface.GetSurface(0).FontSize).Y - (main_Surface.GetSurface(0).FontSize * 3);

            if (txt_Y <= scr_Y) text_Wrap[frag].AppendLine(input_Text.Split('\n')[i]); else { text_Wrap.Add(new StringBuilder()); frag++; i--; }
        }

        main_Surface.GetSurface(0).WriteText(text_Wrap[0]);

        if (sur_Parts.Count > 0)
            if (frag > 0)
                for (int i = 0; i < frag; i++)
                {
                    if (sur_Parts.Count > i) sur_Parts[i].GetSurface(0).WriteText(text_Wrap[i + 1]);
                }
    }

    /// <summary>
            /// Door Status Indication (Индикация состояния двери)
            /// </summary>
            /// <param name="list_Doors">List Doors (Список дверей)</param>
            /// <param name="list_Indicators">List Indicators (Список индикаторов)</param>
    private void DoorStatusInd(List<IMyDoor> list_Doors, List<IMyLightingBlock> list_Indicators, List<IMyLightingBlock> airlock_Lights)
    {
        foreach (var item in airlock_Lights) item.Color = Color.Orange;

        foreach (var obj in list_Doors)
        {
            obj.Enabled = true;

            switch (obj.Status)
            {
                case DoorStatus.Closed:
                    {
                        foreach (var item in list_Indicators)
                        {
                            item.Enabled = true;
                            item.Color = Color.Red;
                            item.BlinkIntervalSeconds = 0f;
                        }
                        break;
                    }

                case DoorStatus.Open:
                    {
                        foreach (var item in list_Indicators)
                        {
                            item.Enabled = true;
                            item.Color = Color.Lime;
                            item.BlinkIntervalSeconds = 0f;
                        }
                        break;
                    }
                case DoorStatus.Opening:
                    {
                        foreach (var item in list_Indicators)
                        {
                            item.Enabled = true;
                            item.Color = Color.Yellow;
                            item.BlinkIntervalSeconds = 1f;
                        }
                        break;
                    }
                case DoorStatus.Closing:
                    {
                        foreach (var item in list_Indicators)
                        {
                            item.Enabled = true;
                            item.Color = Color.Orange;
                            item.BlinkIntervalSeconds = 1f;
                        }
                        break;
                    }
            }
        }
    }

    /// <summary>
            /// Sound block switch (Переключатель звукового блока)
            /// </summary>
            /// <param name="sound_blocks">Sound block name (Название звукового блока)</param>
            /// <param name="on_off">On / off position - Optional (Положение вкл / выкл - Необязательный)</param>
            /// <param name="sound">The name of the sound from the list "optional" - Optional (Название звука из списка - Необязательный)</param>
            /// <param name="num">Number of sound from the list "optional" - Optional (Номер звука из списка - Необязательный)</param>
    private void Sound(List<IMySoundBlock> sound_blocks, bool on_off = false, string sound = "", int num = 0)
    {
        var sounds = new List<string>();

        if (on_off == false) foreach (var item in sound_blocks) item.Stop();
        else
        {
            foreach (var item in sound_blocks)
            {
                item.LoopPeriod = 1800f;
                item.GetSounds(sounds);
                if (sound == "") item.SelectedSound = sounds[num]; else item.SelectedSound = sound;
                item.Play();
            }
        }
    }

    /// <summary>
            /// Clearing block names from prefixes (Очистка названий блоков от префиксов)
            /// </summary>
            /// <param name="root">Constant "root"</param>
            /// <param name="ren_List">List of blocks in a group with prefixes (Список блоков в группе с префиксами)</param>
            /// <param name="grASTag">Group Name (Имя группы)</param>
            /// <param name="enabled_Pref">On/Off prefix (Префикс вкл/выкл)</param>
    private void ClearingPrefixes(Program root, List<IMyTerminalBlock> ren_List, string grASTag, bool enabled_Pref = false)
    {
        var ren_Clean_List = new List<IMyTerminalBlock>();
        root.GridTerminalSystem.GetBlocks(ren_Clean_List);

        foreach (var item in ren_Clean_List)
        {
            if (enabled_Pref)
            {
                item.CustomName = item.CustomName.Replace($"{sb[3]}{grASTag}{sb[4]}", "");
                item.CustomName = item.CustomName.Replace($"{sb[0]}{sb[1]}{sb[5]}", "");
                item.CustomName = item.CustomName.Replace($"{sb[0]}{sb[2]}{sb[1]}{sb[5]}", "");
                item.CustomName = item.CustomName.Replace($"{sb[0]}{sb[2]}", $"{sb[0]}");
                item.CustomName = item.CustomName.Replace($"{sb[2]}{sb[1]}", $"{sb[1]}");
            }
            else if (!ren_List.Contains(item))
            {
                item.CustomName = item.CustomName.Replace($"{sb[3]}{grASTag}{sb[4]}", "");
                item.CustomName = item.CustomName.Replace($"{sb[0]}{sb[1]}{sb[5]}", "");
                item.CustomName = item.CustomName.Replace($"{sb[0]}{sb[2]}{sb[1]}{sb[5]}", "");
                item.CustomName = item.CustomName.Replace($"{sb[0]}{sb[2]}", $"{sb[0]}");
                item.CustomName = item.CustomName.Replace($"{sb[2]}{sb[1]}", $"{sb[1]}");
            }
        }
    }

    /// <summary>
            /// Language selection (Выбор языка)
            /// </summary>
            /// <param name="id_String">Short String Name (Краткое строковое имя)</param>
            /// <param name="code_Language">Language code "format ru / en / etc..." - Optional (Код языка "формат ru / en / ... и т. д." - Необязательный)</param>
    private string Lng(string id_String, string code_Language)
    {
        string str = "";

        switch (code_Language.ToLower())
        {
            case "ru":
                switch (id_String.ToLower())
                {
                    //Airlock Doors
                    case "opening": str = "Открытие"; break;
                    case "open": str = "Открыто"; break;
                    case "closing": str = "Закрытие"; break;
                    case "closed": str = "Закрыто"; break;

                    case "in_ads": str = "Состояние внутренних дверей шлюза"; break;
                    case "out_ads": str = "Состояние внешних дверей шлюза"; break;

                    //Airlock
                    case "depressurized": str = "Вакуум"; break;
                    case "depressurizing": str = "Сброс"; break;
                    case "pressurized": str = "Под давлением"; break;
                    case "pressurizing": str = "Нагнетание"; break;

                    case "on_herm": str = "Герметично"; break;
                    case "off_herm": str = "Не герметично"; break;

                    case "afs": str = "Шкала наполнения воздухом"; break;
                    case "otfs": str = "Заполненность кислородных баков"; break;

                    case "aiig": str = "Количество льда в генераторах"; break;

                    case "as": str = "Состояние шлюза"; break;

                    case "pres_diff": str = "Разницы между внутренним и внешним давлением обнаружено не было."; break;

                    //Notice
                    case "error": str = "Группа не существует!"; break;
                    case "lod": str = "Запуск"; break;

                    //Report
                    case "lights": str = "Свет"; break;
                    case "fl": str = "Вращающиеся огни (Мигалки)"; break;
                    case "pi": str = "Индикаторы давления"; break;
                    case "odi": str = "Индикаторы наружных дверей"; break;
                    case "idi": str = "Индикаторы внутренних дверей"; break;

                    case "spk": str = "Динамики"; break;

                    case "av": str = "Вентиляция"; break;
                    case "bv": str = "Основная вентиляция"; break;
                    case "sv": str = "Вентиляция подкачки"; break;

                    case "tanks": str = "Кислородные баллоны"; break;
                    case "gen": str = "Генераторы кислорода"; break;

                    case "dors": str = "Двери"; break;
                    case "id": str = "Внутренние двери"; break;
                    case "od": str = "Наружные двери"; break;

                    case "mon": str = "Мониторы"; break;

                    //Help
                    case "help_params": str = "Удалите комментарий «#» перед желаемой опцией. («_» - Пустая строка)"; break;

                    //Mode
                    case "mod": str = "Режим воздушного шлюза"; break;
                    case "mod_on": str = "Включен"; break;
                    case "mod_off": str = "Выключен"; break;

                    //SCADA
                    case "fil": str = "ЗАПОЛНЕНИЕ"; break;

                    case "vol": str = "ОБЪЕМ"; break;
                    case "l": str = "л"; break;

                    case "mas": str = "МАССА"; break;
                    case "m": str = "кг"; break;

                    case "qua": str = "КОЛИЧЕСТВО"; break;

                    case "ice": str = "ЛЁД"; break;

                    case "herm": str = "НЕ ГЕРМЕТИЧНО"; break;

                    case "all_vent": str = "ОБЩАЯ\nВЕНТИЛЯЦИЯ"; break;
                    case "swap_vent": str = "ВЕНТИЛЯЦИЯ\nПОДКАЧКИ"; break;

                    //Sensors
                    case "pres_vent": str = "Датчики внутреннего давления"; break;
                    case "ext_vent": str = "Датчики внешнего давления"; break;
                }
                break;
            default:
                switch (id_String.ToLower())
                {
                    //Airlock Doors
                    case "opening": str = "Opening"; break;
                    case "open": str = "Open"; break;
                    case "closing": str = "Closing"; break;
                    case "closed": str = "Closed"; break;

                    case "in_ads": str = "Inner Airlock Doors Status"; break;
                    case "out_ads": str = "Outher Airlock Doors Status"; break;

                    //Airlock
                    case "depressurized": str = "Vacuum"; break;
                    case "depressurizing": str = "Depressurizing"; break;
                    case "pressurized": str = "Pressurized"; break;
                    case "pressurizing": str = "Pressurizing"; break;

                    case "on_herm": str = "Hermetically"; break;
                    case "off_herm": str = "Not Hermetically"; break;

                    case "afs": str = "Air Filling Scale"; break;
                    case "otfs": str = "Oxygen Tanks Fullness"; break;

                    case "aiig": str = "Amount Ice In Generators"; break;

                    case "as": str = "Airlock Status"; break;

                    case "pres_diff": str = "No difference was found between internal and external pressure."; break;

                    //Notice
                    case "error": str = "Group does not exist!"; break;
                    case "lod": str = "Launch"; break;

                    //Report
                    case "lights": str = "Lights"; break;
                    case "fl": str = "Rotating Lights (Flashers)"; break;
                    case "pi": str = "Pressure Indicators"; break;
                    case "odi": str = "Outher Door Indicators"; break;
                    case "idi": str = "Inner Door Indicators"; break;

                    case "spk": str = "Speakers"; break;

                    case "av": str = "Air Vent"; break;
                    case "bv": str = "Basic Vent"; break;
                    case "sv": str = "Swap Vent"; break;

                    case "tanks": str = "Oxygen Tanks"; break;
                    case "gen": str = "Gas Generators"; break;

                    case "dors": str = "Doors"; break;
                    case "id": str = "Inner Doors"; break;
                    case "od": str = "Outher Doors"; break;

                    case "mon": str = "Monitors"; break;

                    //Help
                    case "help_params": str = "Delete the comment \"#\" before the desired option. (\"_\" - Empty line)"; break;

                    //Mode
                    case "mod": str = "Airlock mode"; break;
                    case "mod_on": str = "Enabled"; break;
                    case "mod_off": str = "Disabled"; break;

                    //SCADA
                    case "fil": str = "FILLING"; break;

                    case "vol": str = "VOLUME"; break;
                    case "l": str = "L"; break;

                    case "mas": str = "MASS"; break;
                    case "m": str = "kg"; break;

                    case "qua": str = "QUANTITY"; break;

                    case "ice": str = "ICE"; break;

                    case "herm": str = "NOT HERMETICALLY"; break;

                    case "all_vent": str = "GENERAL\nVENTILATION"; break;
                    case "swap_vent": str = "SWAP\nVENTILATION"; break;

                    //Sensors
                    case "pres_vent": str = "Internal Pressure Sensors"; break;
                    case "ext_vent": str = "External Pressure Sensors"; break;
                }
                break;
        }
        return str;
    }
}