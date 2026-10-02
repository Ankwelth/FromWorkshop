/*
 * Script Version 0.7
 * 
 * En
 * 
 * The script activates the specified timer when receiving damage or sawning (or not complited) of the specified blocks (groups of blocks).
 * 
 * The timer name is set exactly case insensitive (uncomment / comment out one of the penultimate lines for case-sensitive / case-insensitive).
 * After the name of the timer, enter the parts of the words contained in the names of the necessary blocks or groups.
 * 
 * Ru
 * 
 * Скрипт активирует заданный таймер при получении урона или распиле (недостройки) указанных блоков (групп блоков).
 * 
 * Имя таймера устанавливается точно без учета регистра (раскомментируйте / закомментируйте одну из предпоследних строк для учета регистра / без учета регистра).
 * После имени таймера введите части слов, содержащиеся в названиях необходимых блоков или групп.
 */

IntegDamagContr IDC_1 = new IntegDamagContr();
IntegDamagContr IDC_2 = new IntegDamagContr();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    /*через запятую можно добавлять сколько угодно частей имён блоков и груп*/
    IDC_1.Run(this, "timer1", "lights", "battery", "tanks", "sound");
    /*для каждого таймера создавать новый класс*/
    IDC_2.Run(this, "timer2", "ions");

    // DEBUG
    //Echo($"{IDC_1.Run(this, "timer1", "lights", "battery", "tanks", "sound")}");
    //Echo($"{IDC_2.Run(this, "timer2", "ions")}");
}

/// <summary>
        /// Integrity & Damage Control
        /// </summary>
private class IntegDamagContr
{
    /// <summary>
            /// Activate timer when damage or grinding blocks in list or group. (Активация таймера при повреждении блока в списке или группе.)
            /// </summary>
            /// <param name="root">Required parameter "this". (Обязательный параметр "this".)</param>
            /// <param name="timerName">Timer name. (Имя таймера.)</param>
            /// <param name="objectsName">List of words contained in blocks and/or groups. (Список слов содержащихся в блоках и/или группах.)</param>
            /// <returns>Output debugging information. (Вывод отладочной информации.)</returns>
    public string Run(Program root, string timerName, params string[] objectsName)
    {
        var term_Blocks = new List<IMyTerminalBlock>();
        var all_Blocks = new List<IMyTerminalBlock>();

        var term_Groups = new List<IMyBlockGroup>();
        var all_Groups = new List<IMyBlockGroup>();

        var timers = new List<IMyTimerBlock>();

        var act = false;

        foreach (var item in objectsName)
        {
            // --- By Names ---//
            root.GridTerminalSystem.GetBlocksOfType(term_Blocks, s => s.CustomName.ToLower().Contains($"{item.ToLower()}"));
            all_Blocks.AddRange(term_Blocks);

            // --- By Groups ---//
            root.GridTerminalSystem.GetBlockGroups(term_Groups, g => g.Name.ToLower().Contains($"{item.ToLower()}"));
            all_Groups.AddRange(term_Groups);

            foreach (var inj in all_Groups)
            {
                inj.GetBlocks(term_Blocks);
                all_Blocks.AddRange(term_Blocks);
            }
        }

        var blocks = all_Blocks.GroupBy(b => b.GetId()).Select(b => b.First());

        var TXT_DBG = $"*** {timerName} ***\n------------------\nGroups - {all_Groups.Count}\nBlocks - {blocks.Count()}" +
            $"\n------------------\nIncomplete or Damage Blocks:";

        foreach (var item in blocks)
        {
            var max = item.CubeGrid.GetCubeBlock(item.Position).MaxIntegrity;
            var bld = item.CubeGrid.GetCubeBlock(item.Position).BuildIntegrity * 100 / max;
            var dmg = (max - item.CubeGrid.GetCubeBlock(item.Position).CurrentDamage) * 100 / max;

            var itg = -100 + bld + dmg;

            if (itg < 100) act = true;

            if (itg < 100) TXT_DBG += $"\n\n<=[{item.CustomName}]=>\nBuild - {Math.Truncate(bld)}%\nDamage - {Math.Truncate(dmg)}%\n" +
                    $"Integrity - {Math.Truncate(itg)}%";
        }

        // DEBUG
        root.Echo(TXT_DBG.Contains("<=[") ?
            TXT_DBG += "\n////////////////////////////////////\n" : TXT_DBG += " None\n////////////////////////////////////\n");

        root.GridTerminalSystem.GetBlocksOfType(timers, t => t.CustomName.ToLower() == timerName.ToLower());

        if (act) foreach (var item in timers) item.Trigger(); // Для точного имени таймера без учёта регистра

        //if (act > 0) (root.GridTerminalSystem.GetBlockWithName(timerName) as IMyTimerBlock).Trigger(); // Для точного имени таймера с учётом регистра

        return TXT_DBG;
    }
}