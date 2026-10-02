// Этот скрипт предназначен для автоматической сортировки предметов по контейнерам.
// Это наследник скрипта .....

// Исправлено:
// Обрабатывается только сетка. Пристыкованные корабли не сортируются.
// Вызов замедлен до 1 раза в 6 секунд (500 тиков) чтобы не перегружать сервера на которых он работает.
// Кроме того оптимизирован сам скрипт с целью увеличения производительности.
// Добавлено условиепереполнения контейнера - если один заполнен то начинает заполняться следующий. 
// Исправлена  (не полностью) проблема с переименованием контейнеров. При удалении тэга все еще может 
// потребоваться перезагрузка мира, глюки с кэшированием самой игры. 
// Теперь обрабатываются все контейнеры а не только те, который имеют тэги.
// Тперь скрипт обрабатывает все соедржимое контейнера за 1 раз а не только первую позицию в списке.
// Добавлен вывод списка обрабатываемых блоков (терминал программного блока в панели блоков)


//  Скрипт использует список типов грузов (CargoTypes), включающий руду, компоненты, лёд и слитки.
//  Каждому типу груза соответствует свой контейнер. Скрипт автоматически перераспределяет предметы
//  из ассемблеров и очистительных заводов по соответствующим контейнерам.
 // Для работы скрипта требуется наличие контейнеров с названиями, соответствующими типам грузов в квадратных скобках.
// Скрипт умышленно не затрагивает пристыкованные корабли!!!



double elapsedTime = -1 ;

string[] CargoTypes =
        {
            "ORE",
            "COMPONENT",
            "STUFF",
            "ICE",
            "INGOT"
        };

        Dictionary <string, List<IMyTerminalBlock>> Cargos;
        List<IMyAssembler> Assemblers;
        List<IMyRefinery> Refineries;



        public Program()
        {
            Cargos = new Dictionary<string, List<IMyTerminalBlock>>();
            Assemblers = new List<IMyAssembler>();
            Refineries = new List<IMyRefinery>();

            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(Assemblers);
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(Refineries);

            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            foreach (string CargoType in CargoTypes)
            {
                //Cargos[CargoType].Clear();
                Cargos[CargoType] = new List<IMyTerminalBlock>();
                GridTerminalSystem.SearchBlocksOfName("[" + CargoType + "]", Cargos[CargoType]);
                Echo("Cargos " + CargoType +": " + Cargos[CargoType].Count);

            }


            Echo("Assemblers found: " + Assemblers.Count);
            Echo("Refineries found: " + Refineries.Count);            
        }

void Main()
{
    elapsedTime += Runtime.TimeSinceLastRun.TotalSeconds;

    if (elapsedTime >= 5 || elapsedTime == -1)
    {
        Echo("Running: " + elapsedTime + "s");
        //Echo("Cargos found: " + Cargos.Count);
        elapsedTime = 0; // Сбросить таймер

        foreach (string CargoType in CargoTypes)
            {
                Cargos[CargoType].Clear();            
                Cargos[CargoType] = new List<IMyTerminalBlock>();
                GridTerminalSystem.SearchBlocksOfName("[" + CargoType + "]", Cargos[CargoType]);
                Echo("Cargos " + CargoType +": " + Cargos[CargoType].Count);                
            }
        Echo("Assemblers found: " + Assemblers.Count);
        Echo("Refineries found: " + Refineries.Count);
        Assemblers.Clear();
        Refineries.Clear();
        GridTerminalSystem.GetBlocksOfType<IMyAssembler>(Assemblers);
        GridTerminalSystem.GetBlocksOfType<IMyRefinery>(Refineries);
        SortAssemberItems();
        SortRefineryItems();
        SortCargos();
  }
}



void SortAssemberItems()
{
    //Echo("Sorting Assembler Items...");
    if (Assemblers.Count > 0)
    {
        foreach (IMyAssembler Assembler in Assemblers)
        {
            if (Assembler.GetInventory(1).ItemCount > 0)
            {
                List<MyInventoryItem> Items = new List<MyInventoryItem>();
                Assembler.GetInventory(1).GetItems(Items);

                foreach (MyInventoryItem Item in Items)
                {
                    if (Cargos.ContainsKey("COMPONENT"))
                    {
                        foreach (IMyCargoContainer Cargo in Cargos["COMPONENT"])
                        {
                            // Проверка на принадлежность к одному конструкту
                            if (!Cargo.GetInventory(0).IsFull && Cargo.IsSameConstructAs(Assembler))
                            {
                                Assembler.GetInventory(1).TransferItemTo(Cargo.GetInventory(0), Item);
                                //break;
                            }
                        }
                    }
                }
            }
        }
    }
}

void SortRefineryItems()
{
    //Echo("Sorting Refiners Items...");

    if (Refineries.Count > 0)
    {
        foreach (IMyRefinery Refinery in Refineries)
        {
            if (Refinery.GetInventory(1).ItemCount > 0)
            {
                List<MyInventoryItem> Items = new List<MyInventoryItem>();
                Refinery.GetInventory(1).GetItems(Items);

                foreach (MyInventoryItem Item in Items)
                {
                    if (Cargos.ContainsKey("INGOT"))
                    {
                        foreach (IMyCargoContainer Cargo in Cargos["INGOT"])
                        {
                            // Проверка на принадлежность к одному конструкту
                            if (!Cargo.GetInventory(0).IsFull && Cargo.IsSameConstructAs(Refinery))
                            {
                                Refinery.GetInventory(1).TransferItemTo(Cargo.GetInventory(0), Item);
                                //break;
                            }
                        }
                    }
                }
            }
        }
    }
}

void SortCargos()
{
    List<IMyTerminalBlock> allContainers = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(allContainers, c => c.IsSameConstructAs(Me));

    foreach (IMyTerminalBlock block in allContainers)
    {
        var container = block as IMyCargoContainer;
        if (container == null)
            continue;

        List<MyInventoryItem> items = new List<MyInventoryItem>();
        container.GetInventory(0).GetItems(items);

        foreach (var item in items)
        {
            string itemFullType = item.Type.ToString();
            int firstStringPosition = itemFullType.IndexOf("_");
            int secondStringPosition = itemFullType.IndexOf("/");
            string itemType = itemFullType.Substring(firstStringPosition + 1, secondStringPosition - firstStringPosition - 1);

            if (itemFullType.IndexOf("Ice") != -1) itemType = "Ice";

            IMyCargoContainer destinationContainer = FindContainerForItem(itemType);
            if (destinationContainer != null)
            {
                container.GetInventory(0).TransferItemTo(destinationContainer.GetInventory(0), item);
            }
        }
    }
}

IMyCargoContainer FindContainerForItem(string itemType)
{
    List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("[" + itemType.ToUpper() + "]", containers, c => c is IMyCargoContainer);

    foreach (IMyCargoContainer container in containers)
    {
        if (container.IsSameConstructAs(Me) && !container.GetInventory(0).IsFull)
            return container;
    }

    return null;
}
