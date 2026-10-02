/*
 * Скрипт автоматического магазина от Dark_raven, обновлен Dissector`ом. Версия 2.1
 * 
 * Готов к работе сразу после загрузки, но с дефолтными параметрами.
 * При необходимости их можно подстроить, о чём ниже.
 * 
 * 1. Блоки магазинов.
 * Скрипт "работает" только с теми блоками магазинов,
 * у которых в имени есть специальный тег.
 * Можно добавить все теги в 1 магазин - так тоже работает.
 * Если какого-то тега не будет найдено - скрипт продолжит работу.
 * Для добавления новых блоков магазина необходимо рекомпилировать скрипт.
 * 
 * 2. Контейнеры.
 * По умолчанию скрипт использует все контейнеры текущей сетки, но
 * игнориует контейнеры с исключающим тегом.
 * Или можно указать группу блоков с контейнерами для работы,
 * тогда скрипт будет использовать только их.
 * Или можно пометить конкретные контейнеры специальных тегом для работы,
 * тогда скрипт будет использовать только их.
 * 
 * 4. Компоненты для торговли.
 * Торговля осуществляется с каждым товаром индивидуально согласно
 * настройкам ниже.
 * Цены на товары ограничены самой игрой и выйти за разрешенный рамки нельзя,
 * - товар просто не появится в магазине.
 * 
 * 5. Обновление торговых предложений.
 * Выкладка товаров проиходит сразу после компиляции и затем раз в час(по умолчанию).
 * Время можно настроить своё, но не рекомендуется слишком часто,
 * так как это может вызвать лаги.
 * В каждый подключенный магазин происходит вывод логов в текстовые данные блока.
 * При необходимости можно проверить его работу.
 * 
 */

// ============  ОБЯЗАТЕЛЬНЫЕ НАСТРОЙКИ ============
// Список тегов в имени блоков магазинов
string[] storeType = new string[6] {
    "Components",   // Тег блока магазина с компонентами
    "Ingots",       // Тег блока магазина слитков
    "Ores",         // Тег блока магазина руды
    "Tools",        // Тег блока магазина инструментов
    "Consumables",  // Тег блока магазина расходников (еда, напитки)
    "Seeds"         // Тег блока магазина семян
};

// ============ ОПЦИОНАЛЬНЫЕ НАСТРОЙКИ МАГАЗИНА ============
int timeRefresh = 3600; // Интервал для обновления товаров в магазине в секундах (3600 сек = 1 час)
const string kTagExclude = "Исключить";
const string kGroupContainersForTrade = "";
const string kTagContainerForTrade = "";

bool tradeComponents = true;
bool tradeIngots = true;
bool tradeOres = true;
bool tradeTools = true;
bool tradeConsumables = true;
bool tradeSeeds = true;

/* -------------------------------------------------------------
         * КОНФИГУРАЦИЯ ТОВАРОВ
         * -------------------------------------------------------------
         * MyItem(граница, цена_закупки, закупка_разрешена, цена_продажи, продажа_разрешена, режим)
         * Режимы: TradeModel.Storage, TradeModel.Shop, TradeModel.SellOnly
         */

// ========== КОМПОНЕНТЫ ==========
static internal Dictionary<string, MyItem> Components = new Dictionary<string, MyItem>()
{
    ["BulletproofGlass"] = new MyItem(235, 830, true, 3606, true, TradeModel.SellOnly),
    ["Canvas"] = new MyItem(10, 2500, true, 13709, true, TradeModel.SellOnly),
    ["Computer"] = new MyItem(400, 45, true, 167, true, TradeModel.SellOnly),
    ["Construction"] = new MyItem(1000, 430, true, 1923, true, TradeModel.SellOnly),
    ["Detector"] = new MyItem(30, 2236, true, 11638, true, TradeModel.SellOnly),
    ["Display"] = new MyItem(30, 381, true, 1442, true, TradeModel.SellOnly),
    ["Explosives"] = new MyItem(0, 33633, false, 168544, false, TradeModel.SellOnly),
    ["Girder"] = new MyItem(100, 360, true, 1442, true, TradeModel.SellOnly),
    ["GravityGenerator"] = new MyItem(0, 150000, false, 1750595, false, TradeModel.SellOnly),
    ["InteriorPlate"] = new MyItem(200, 154, true, 721, true, TradeModel.SellOnly),
    ["LargeTube"] = new MyItem(200, 1702, true, 8940, true, TradeModel.SellOnly),
    ["Medical"] = new MyItem(0, 40000, false, 194488, false, TradeModel.SellOnly),
    ["MetalGrid"] = new MyItem(300, 3265, true, 12495, true, TradeModel.SellOnly),
    ["Motor"] = new MyItem(150, 2008, true, 9759, true, TradeModel.SellOnly),
    ["PowerCell"] = new MyItem(50, 1078, true, 5380, true, TradeModel.SellOnly),
    ["RadioCommunication"] = new MyItem(30, 515, true, 3334, true, TradeModel.SellOnly),
    ["Reactor"] = new MyItem(0, 6410, false, 39421, false, TradeModel.SellOnly),
    ["SmallTube"] = new MyItem(300, 267, true, 1202, true, TradeModel.SellOnly),
    ["SolarCell"] = new MyItem(0, 641, false, 3361, false, TradeModel.SellOnly),
    ["SteelPlate"] = new MyItem(2500, 1236, true, 5048, true, TradeModel.SellOnly),
    ["Superconductor"] = new MyItem(0, 21354, false, 132429, false, TradeModel.SellOnly),
    ["Thrust"] = new MyItem(0, 41325, false, 162463, false, TradeModel.SellOnly),
    ["ZoneChip"] = new MyItem(0, 100000, false, 100500, false, TradeModel.SellOnly),
    ["EngineerPlushie"] = new MyItem(1, 30000, false, 30500, true, TradeModel.SellOnly),
    ["EngineerPlushieSE2"] = new MyItem(1, 30000, false, 30500, true, TradeModel.SellOnly),
    ["SabiroidPlushie"] = new MyItem(1, 30000, false, 30500, true, TradeModel.SellOnly),
    ["PrototechFrame"] = new MyItem(10, 100000, false, 105000, true, TradeModel.SellOnly),
    ["PrototechPanel"] = new MyItem(10, 323580, false, 325000, true, TradeModel.SellOnly),
    ["PrototechCapacitor"] = new MyItem(10, 635439, false, 640000, true, TradeModel.SellOnly),
    ["PrototechPropulsionUnit"] = new MyItem(10, 1460119, false, 1465000, true, TradeModel.SellOnly),
    ["PrototechMachinery"] = new MyItem(10, 353407, false, 355000, true, TradeModel.SellOnly),
    ["PrototechCircuitry"] = new MyItem(10, 664209, false, 670000, true, TradeModel.SellOnly),
    ["PrototechCoolingUnit"] = new MyItem(10, 1787887, false, 1800000, true, TradeModel.SellOnly),
};

// ========== СЛИТКИ ==========
static internal Dictionary<string, MyItem> Ingots = new Dictionary<string, MyItem>()
{
    ["Cobalt"] = new MyItem(1000, 1535, true, 1600, true, TradeModel.SellOnly),
    ["Gold"] = new MyItem(1000, 23355, true, 24000, true, TradeModel.SellOnly),
    ["Iron"] = new MyItem(1000, 150, true, 170, true, TradeModel.SellOnly),
    ["Magnesium"] = new MyItem(1000, 34054, true, 34500, true, TradeModel.SellOnly),
    ["Nickel"] = new MyItem(1000, 306, true, 310, true, TradeModel.SellOnly),
    ["Platinum"] = new MyItem(10, 122815, true, 123000, true, TradeModel.SellOnly),
    ["Silicon"] = new MyItem(1000, 173, true, 180, true, TradeModel.SellOnly),
    ["Silver"] = new MyItem(1000, 2585, true, 2600, true, TradeModel.SellOnly),
    ["Uranium"] = new MyItem(50, 80664, true, 80700, true, TradeModel.SellOnly),
};

// ========== РУДЫ ==========
static internal Dictionary<string, MyItem> Ores = new Dictionary<string, MyItem>()
{
    ["Cobalt"] = new MyItem(1000, 300, true, 310, true, TradeModel.SellOnly),
    ["Gold"] = new MyItem(1000, 210, true, 230, true, TradeModel.SellOnly),
    ["Stone"] = new MyItem(1000, 10, true, 11, true, TradeModel.SellOnly),
    ["Iron"] = new MyItem(1000, 105, true, 110, true, TradeModel.SellOnly),
    ["Magnesium"] = new MyItem(1000, 210, true, 212, true, TradeModel.SellOnly),
    ["Nickel"] = new MyItem(1000, 100, true, 105, true, TradeModel.SellOnly),
    ["Platinum"] = new MyItem(1000, 420, true, 435, true, TradeModel.SellOnly),
    ["Silicon"] = new MyItem(1000, 100, true, 110, true, TradeModel.SellOnly),
    ["Silver"] = new MyItem(1000, 210, true, 212, true, TradeModel.SellOnly),
    ["Uranium"] = new MyItem(1000, 350, true, 505, true, TradeModel.SellOnly),
    ["Ice"] = new MyItem(1000, 50, true, 51, true, TradeModel.SellOnly),
};

// ========== ИНСТРУМЕНТЫ, БАЛЛОНЫ, БОЕПРИПАСЫ ==========
static internal Dictionary<string, MyItem> Tools = new Dictionary<string, MyItem>()
{
    ["WelderItem"] = new MyItem(0, 2908, true, 2909, true, TradeModel.SellOnly),
    ["Welder2Item"] = new MyItem(1, 10518, true, 11000, true, TradeModel.SellOnly),
    ["Welder3Item"] = new MyItem(1, 32664, true, 35000, true, TradeModel.SellOnly),
    ["Welder4Item"] = new MyItem(2, 302438, true, 310000, true, TradeModel.SellOnly),
    ["AngleGrinderItem"] = new MyItem(2, 3433, true, 3500, true, TradeModel.SellOnly),
    ["AngleGrinder2Item"] = new MyItem(2, 10578, true, 11000, true, TradeModel.SellOnly),
    ["AngleGrinder3Item"] = new MyItem(2, 32821, true, 35000, true, TradeModel.SellOnly),
    ["AngleGrinder4Item"] = new MyItem(2, 302901, true, 310000, true, TradeModel.SellOnly),
    ["HandDrillItem"] = new MyItem(2, 4851, true, 5000, true, TradeModel.SellOnly),
    ["HandDrill2Item"] = new MyItem(2, 15155, true, 17000, true, TradeModel.SellOnly),
    ["HandDrill3Item"] = new MyItem(2, 44764, true, 45000, true, TradeModel.SellOnly),
    ["HandDrill4Item"] = new MyItem(2, 338084, true, 340000, true, TradeModel.SellOnly),
    ["FlareGunItem"] = new MyItem(1, 520, true, 550, true, TradeModel.SellOnly),
    ["SemiAutoPistolItem"] = new MyItem(1, 566, true, 600, true, TradeModel.SellOnly),
    ["FullAutoPistolItem"] = new MyItem(2, 3140, true, 3500, true, TradeModel.SellOnly),
    ["ElitePistolItem"] = new MyItem(2, 64155, true, 65000, true, TradeModel.SellOnly),
    ["AutomaticRifleItem"] = new MyItem(1, 4338, true, 5000, true, TradeModel.SellOnly),
    ["PreciseAutomaticRifleItem"] = new MyItem(1, 19148, true, 20000, true, TradeModel.SellOnly),
    ["RapidFireAutomaticRifleItem"] = new MyItem(1, 15233, true, 16000, true, TradeModel.SellOnly),
    ["UltimateAutomaticRifleItem"] = new MyItem(1, 111783, true, 115000, true, TradeModel.SellOnly),
    ["BasicHandHeldLauncherItem"] = new MyItem(1, 33807, true, 35000, true, TradeModel.SellOnly),
    ["AdvancedHandHeldLauncherItem"] = new MyItem(1, 275942, true, 280000, true, TradeModel.SellOnly),
};

static internal Dictionary<string, MyItem> Oxygen = new Dictionary<string, MyItem>()
{
    ["OxygenBottle"] = new MyItem(2, 68909, true, 70000, true, TradeModel.SellOnly)
};

static internal Dictionary<string, MyItem> Hydrogen = new Dictionary<string, MyItem>()
{
    ["HydrogenBottle"] = new MyItem(2, 68909, true, 70000, true, TradeModel.SellOnly)
};

static internal Dictionary<string, MyItem> Ammo = new Dictionary<string, MyItem>()
{
    ["SemiAutoPistolMagazine"] = new MyItem(20, 112, true, 150, true, TradeModel.SellOnly),
    ["FullAutoPistolMagazine"] = new MyItem(50, 8699, true, 9200, true, TradeModel.SellOnly),
    ["ElitePistolMagazine"] = new MyItem(50, 8163, true, 8653, true, TradeModel.SellOnly),
    ["FlareClip"] = new MyItem(10, 95, true, 100, true, TradeModel.SellOnly),
    ["FireworksBoxBlue"] = new MyItem(10, 45899, true, 46400, true, TradeModel.SellOnly),
    ["FireworksBoxGreen"] = new MyItem(10, 45899, true, 46400, true, TradeModel.SellOnly),
    ["FireworksBoxRed"] = new MyItem(10, 45899, true, 46400, true, TradeModel.SellOnly),
    ["FireworksBoxPink"] = new MyItem(10, 45899, true, 46400, true, TradeModel.SellOnly),
    ["FireworksBoxYellow"] = new MyItem(10, 45899, true, 46400, true, TradeModel.SellOnly),
    ["FireworksBoxRainbow"] = new MyItem(10, 45899, true, 46400, true, TradeModel.SellOnly),
    ["AutomaticRifleGun_Mag_20rd"] = new MyItem(10, 15113, true, 15600, true, TradeModel.SellOnly),
    ["RapidFireAutomaticRifleGun_Mag_50rd"] = new MyItem(10, 40220, true, 40720, true, TradeModel.SellOnly),
    ["PreciseAutomaticRifleGun_Mag_5rd"] = new MyItem(10, 15113, true, 15613, true, TradeModel.SellOnly),
    ["UltimateAutomaticRifleGun_Mag_30rd"] = new MyItem(10, 25185, true, 25685, true, TradeModel.SellOnly),
    ["AutocannonClip"] = new MyItem(10, 181002, true, 182002, true, TradeModel.SellOnly),
    ["Missile200mm"] = new MyItem(10, 170482, true, 171482, true, TradeModel.SellOnly),
    ["LargeCalibreAmmo"] = new MyItem(10, 581700, true, 590000, true, TradeModel.SellOnly),
    ["MediumCalibreAmmo"] = new MyItem(10, 96327, true, 100000, true, TradeModel.SellOnly),
    ["LargeRailgunAmmo"] = new MyItem(10, 158889, true, 165000, true, TradeModel.SellOnly),
    ["SmallRailgunAmmo"] = new MyItem(10, 26866, true, 30000, true, TradeModel.SellOnly),
    ["NATO_25x184mm"] = new MyItem(10, 314791, true, 320000, true, TradeModel.SellOnly),
    ["NATO_5p56x45mm"] = new MyItem(10, 96327, true, 100000, true, TradeModel.SellOnly),
};

// ========== РАСХОДНИКИ (ЕДА, НАПИТКИ) ==========
static internal Dictionary<string, MyItem> Consumables = new Dictionary<string, MyItem>()
{
    // Базовые (были)
    ["Medkit"] = new MyItem(10, 500, false, 600, true, TradeModel.SellOnly),
    ["Powerkit"] = new MyItem(10, 800, false, 950, true, TradeModel.SellOnly),
    ["RadiationKit"] = new MyItem(5, 1200, false, 1400, true, TradeModel.SellOnly),
    // Напитки
    ["ClangCola"] = new MyItem(0, 100, false, 105, true, TradeModel.SellOnly),
    ["CosmicCoffee"] = new MyItem(0, 100, false, 105, true, TradeModel.SellOnly),
    // Готовые рационы
    ["MealPack_KelpCrisp"] = new MyItem(0, 3491, false, 3900, true, TradeModel.SellOnly),
    ["MealPack_FruitBar"] = new MyItem(0, 8005, false, 8505, true, TradeModel.SellOnly),
    ["MealPack_GardenSlaw"] = new MyItem(0, 11567, false, 12000, true, TradeModel.SellOnly),
    ["MealPack_RedPellets"] = new MyItem(0, 11730, false, 12200, true, TradeModel.SellOnly),
    ["MealPack_Chili"] = new MyItem(0, 15369, false, 15800, true, TradeModel.SellOnly),
    ["MealPack_Ramen"] = new MyItem(0, 9588, false, 10000, true, TradeModel.SellOnly),
    ["MealPack_Flatbread"] = new MyItem(0, 11807, false, 12350, true, TradeModel.SellOnly),
    ["MealPack_FruitPastry"] = new MyItem(0, 16011, false, 16500, true, TradeModel.SellOnly),
    ["MealPack_VeggieBurger"] = new MyItem(0, 15365, false, 15800, true, TradeModel.SellOnly),
    ["MealPack_GreenPellets"] = new MyItem(0, 14294, false, 14750, true, TradeModel.SellOnly),
    ["MealPack_Curry"] = new MyItem(0, 15771, false, 16270, true, TradeModel.SellOnly),
    ["MealPack_Dumplings"] = new MyItem(0, 15605, false, 16100, true, TradeModel.SellOnly),
    ["MealPack_Spaghetti"] = new MyItem(0, 19646, false, 20100, true, TradeModel.SellOnly),
    ["MealPack_Lasagna"] = new MyItem(0, 22380, false, 22800, true, TradeModel.SellOnly),
    ["MealPack_Burrito"] = new MyItem(0, 26182, false, 26600, true, TradeModel.SellOnly),
    ["MealPack_FrontierStew"] = new MyItem(0, 21977, false, 22400, true, TradeModel.SellOnly),
    ["MealPack_SearedSabiroid"] = new MyItem(0, 21184, false, 21680, true, TradeModel.SellOnly),
    ["MealPack_SteakDinner"] = new MyItem(0, 26025, false, 26525, true, TradeModel.SellOnly),
    // Ингредиенты (съедобные)
    ["Fruit"] = new MyItem(0, 1240, false, 1300, true, TradeModel.SellOnly),
    ["Mushrooms"] = new MyItem(0, 1164, false, 1200, true, TradeModel.SellOnly),
    ["Vegetables"] = new MyItem(0, 1189, false, 1250, true, TradeModel.SellOnly),
    // Мясо
    ["MammalMeatRaw"] = new MyItem(0, 1027, false, 1100, true, TradeModel.SellOnly),
    ["MammalMeatCooked"] = new MyItem(0, 1027, false, 1100, true, TradeModel.SellOnly),
    ["InsectMeatRaw"] = new MyItem(0, 856, false, 950, true, TradeModel.SellOnly),
    ["InsectMeatCooked"] = new MyItem(0, 856, false, 950, true, TradeModel.SellOnly),
};

// ========== СЕМЕНА ==========
static internal Dictionary<string, MyItem> Seeds = new Dictionary<string, MyItem>()
{
    ["Fruit"] = new MyItem(0, 100, false, 110, true, TradeModel.SellOnly),
    ["Grain"] = new MyItem(0, 100, false, 110, true, TradeModel.SellOnly),
    ["Mushrooms"] = new MyItem(0, 100, false, 110, true, TradeModel.SellOnly),
    ["Vegetables"] = new MyItem(0, 100, false, 110, true, TradeModel.SellOnly),
};

// ============ КОНЕЦ НАСТРОЕК ============

MyAutoStore AutoStore;
readonly string[] arguments = new string[] {
    "магазин.разместить",
    "магазин.очистить",
    "магазин.список"
};

// Режим работы с товаром
public enum TradeModel : byte { Shop, Storage, SellOnly }
string oldCommand = "";

public Program()
{
    AutoStore = new MyAutoStore(ref tradeComponents, ref tradeIngots, ref tradeOres, ref tradeTools, ref tradeConsumables, ref tradeSeeds, timeRefresh);
    AutoStore.GetStoreBlock(GridTerminalSystem, Me.CubeGrid, ref storeType);
    CheckingSystem();
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    AvailableCommands();
}

public void Main(string arg, UpdateType updateSource)
{
    if (AutoStore.TimeCheckStore.IsOut())
    {
        if (Runtime.UpdateFrequency != UpdateFrequency.Update10) Runtime.UpdateFrequency = UpdateFrequency.Update10;
        AutoStore.StoreUpdate(GridTerminalSystem, Me.CubeGrid);
    }
    else if (Runtime.UpdateFrequency != UpdateFrequency.Update100) Runtime.UpdateFrequency = UpdateFrequency.Update100;

    if (arg != string.Empty) Arguments(arg);

    Echo($"=== Автоматический магазин ===");
    //Echo($"Выполнение {Runtime.LastRunTimeMs} мс");
    if (AutoStore.TimeCheckStore.Launched)
        Echo($"Обновление предложений \n* через {AutoStore.TimeCheckStore.RestTime}");

    AvailableCommands();
}

void CheckingSystem()
{
    Me.CustomData = "";

    if (AutoStore.StoreComp.Block != null)
        Me.CustomData += $"{AutoStore.StoreComp.Block.CustomName} подключен. Торговля: {tradeComponents}";
    else
        Me.CustomData += $"Магазин для {storeType[0]} не подключен.";

    if (AutoStore.StoreIng.Block != null)
        Me.CustomData += $"\n{AutoStore.StoreIng.Block.CustomName} подключен. Торговля: {tradeIngots}";
    else
        Me.CustomData += $"\nМагазин для {storeType[1]} не подключен.";

    if (AutoStore.StoreOre.Block != null)
        Me.CustomData += $"\n{AutoStore.StoreOre.Block.CustomName} подключен. Торговля: {tradeOres}";
    else
        Me.CustomData += $"\nМагазин для {storeType[2]} не подключен.";

    if (AutoStore.StoreTool.Block != null)
        Me.CustomData += $"\n{AutoStore.StoreTool.Block.CustomName} подключен. Торговля: {tradeTools}";
    else
        Me.CustomData += $"\nМагазин для {storeType[3]} не подключен.";

    if (AutoStore.StoreConsumables.Block != null)
        Me.CustomData += $"\n{AutoStore.StoreConsumables.Block.CustomName} подключен. Торговля: {tradeConsumables}";
    else
        Me.CustomData += $"\nМагазин для {storeType[4]} не подключен.";

    if (AutoStore.StoreSeeds.Block != null)
        Me.CustomData += $"\n{AutoStore.StoreSeeds.Block.CustomName} подключен. Торговля: {tradeSeeds}";
    else
        Me.CustomData += $"\nМагазин для {storeType[5]} не подключен.";

    Me.CustomData += $"\n\n* Добавьте нужным контейнерам тег '{kTagContainerForTrade}' в имя блока.";
    Me.CustomData += $"\n* Или создайте группу '{kGroupContainersForTrade}' с нужными контейнерами.";
}
void Arguments(string arg)
{
    if (arg.ToLower() == arguments[0])
    {
        AutoStore.TimeCheckStore.Stop();
        oldCommand = arguments[0];
        AvailableCommands();
    }
    else if (arg.ToLower() == arguments[1])
    {
        AutoStore.StoreComp.ClearAll();
        AutoStore.StoreIng.ClearAll();
        AutoStore.StoreOre.ClearAll();
        AutoStore.StoreTool.ClearAll();
        AutoStore.StoreConsumables.ClearAll();
        AutoStore.StoreSeeds.ClearAll();
        oldCommand = $"{arguments[1]}\n=> Очистка завершена";
        AvailableCommands();
    }
    else if (arg.ToLower() == arguments[2])
    {
        Me.CustomData = AutoStore.StoreComp.GetOrdersAndOffers();
        Me.CustomData += AutoStore.StoreIng.GetOrdersAndOffers();
        Me.CustomData += AutoStore.StoreOre.GetOrdersAndOffers();
        Me.CustomData += AutoStore.StoreTool.GetOrdersAndOffers();
        Me.CustomData += AutoStore.StoreConsumables.GetOrdersAndOffers();
        Me.CustomData += AutoStore.StoreSeeds.GetOrdersAndOffers();
        oldCommand = $"{arguments[2]}\nТовары из магазина выведены в данные ПБ";
        AvailableCommands();
    }
}
void AvailableCommands()
{
    string info = $"\nПред.аргумент: {oldCommand}\n\nВозможные аргументы:";
    foreach (var arg in arguments) { info += $"\n{arg}"; }
    Echo(info);
}

public class MyProductBlock
{
    List<MyStoreQueryItem> _storeItems = new List<MyStoreQueryItem>();
    internal IMyStoreBlock Block { get; set; } = null;
    internal bool Trading { get; set; } = true;
    internal StringBuilder TradeInfo { get; private set; } = new StringBuilder();

    internal MyProductBlock(bool trading) { Trading = trading; }
    internal MyProductBlock(IMyStoreBlock StoreBlock) { Block = StoreBlock; }
    internal MyProductBlock(IMyGridTerminalSystem TerminalSystem, IMyCubeGrid CubeGrid, string nameStore) { GetBlocks(TerminalSystem, CubeGrid, nameStore); }

    internal void GetBlocks(IMyGridTerminalSystem TerminalSystem, IMyCubeGrid ThisCubeGrid, string tagStoreName)
    {
        List<IMyStoreBlock> temp = new List<IMyStoreBlock>();
        TerminalSystem.GetBlocksOfType(temp, x => x.CubeGrid == ThisCubeGrid && x.CustomName.ToLower().Contains(tagStoreName.ToLower()));
        foreach (var t in temp) { Block = t; break; }
    }

    internal void PlaceOfferingsAndSales(ref Dictionary<string, MyItem> ItemsForSaleBuy, string MyObjectBuilder_name, bool append = false)
    {
        if (!Trading || Block == null || !Block.IsWorking) return;
        _storeItems.Clear();
        if (!append) { TradeInfo.Clear(); TradeInfo.AppendLine($"Выкладка товаров осуществленна {DateTime.Now:g}"); }
        Block.GetPlayerStoreItems(_storeItems);
        foreach (var Item in ItemsForSaleBuy)
        {
            if (Item.Value.Mode == TradeModel.Storage)
            {
                if (Item.Value.AlowBuy && Item.Value.Amount < Item.Value.MaxAmount)
                    CreateOrder(ref MyObjectBuilder_name, Item.Key, Item.Value.BuyPrice, Item.Value.MaxAmount - Item.Value.Amount, true);
                else if (Item.Value.AlowSale && Item.Value.Amount > Item.Value.MaxAmount)
                    CreateOffer(ref MyObjectBuilder_name, Item.Key, Item.Value.SalePrice, Item.Value.Amount - Item.Value.MaxAmount, true);
            }
            else if (Item.Value.Mode == TradeModel.Shop)
            {
                if (Item.Value.AlowBuy && Item.Value.Amount < Item.Value.MaxAmount)
                    CreateOrder(ref MyObjectBuilder_name, Item.Key, Item.Value.BuyPrice, Item.Value.MaxAmount - Item.Value.Amount);
                if (Item.Value.AlowSale && Item.Value.Amount > 0)
                    CreateOffer(ref MyObjectBuilder_name, Item.Key, Item.Value.SalePrice, Item.Value.Amount);
            }
            else if (Item.Value.Mode == TradeModel.SellOnly)
            {
                if (Item.Value.AlowSale && Item.Value.Amount > 0)
                    CreateOffer(ref MyObjectBuilder_name, Item.Key, Item.Value.SalePrice, Item.Value.Amount, true);
            }
        }
        if (append) Block.CustomData += TradeInfo.ToString();
        else Block.CustomData = TradeInfo.ToString();
        _storeItems.Clear();
        TradeInfo.Clear();
    }

    void CreateOrder(ref string MyObjectBuilder_name, string itemName, int BuyPrice, int dif, bool agressiveRemove = false)
    {
        if (!IsPosted(MyObjectBuilder_name, itemName, BuyPrice, dif))
        {
            if (agressiveRemove) RemoveDuplicates(ref MyObjectBuilder_name, ref itemName, _storeItems);
            else RemoveDuplicates(ref MyObjectBuilder_name, ref itemName, _storeItems, BuyPrice);
            InsertOrder(MyObjectBuilder_name + "/" + itemName, dif, BuyPrice);
        }
        else TradeInfo.AppendLine($"[No update] Закупка {itemName} в кол-ве {dif}шт по цене {BuyPrice}кр. уже размещена");
    }

    void CreateOffer(ref string MyObjectBuilder_name, string itemName, int SalePrice, int dif, bool agressiveRemove = false)
    {
        if (!IsPosted(MyObjectBuilder_name, itemName, SalePrice, dif))
        {
            if (agressiveRemove) RemoveDuplicates(ref MyObjectBuilder_name, ref itemName, _storeItems);
            else RemoveDuplicates(ref MyObjectBuilder_name, ref itemName, _storeItems, SalePrice);
            InsertOffer(MyObjectBuilder_name + "/" + itemName, dif, SalePrice);
        }
        else TradeInfo.AppendLine($"[No update] Продажа {itemName} в кол-ве {dif}шт по цене {SalePrice}кр. уже размещена");
    }

    internal string GetOrdersAndOffers()
    {
        if (Block == null) return $"\n[Магазин не подключен]";
        _storeItems.Clear();
        TradeInfo.Clear();
        Block.GetPlayerStoreItems(_storeItems);
        TradeInfo.AppendLine($"\n{Block.CustomName} выложено {_storeItems.Count} товаров");
        foreach (var item in _storeItems) { TradeInfo.AppendLine($"\n{item.ItemId.SubtypeId} {item.Amount} шт по цене {item.PricePerUnit}"); }
        _storeItems.Clear();
        return TradeInfo.ToString();
    }

    internal void ClearAll()
    {
        if (Block == null) return;
        _storeItems.Clear();
        Block.GetPlayerStoreItems(_storeItems);
        foreach (var item in _storeItems) { Block.CancelStoreItem(item.Id); }
        Block.CustomData = $"Очистка магазина\n..удалено {_storeItems.Count} позиций";
        _storeItems.Clear();
    }

    void RemoveDuplicates(ref string TypeId, ref string SubtypeId, List<MyStoreQueryItem> storeItems, int price = 0)
    {
        if (price == 0) foreach (var item in storeItems) { if (item.ItemId.TypeIdString == TypeId && item.ItemId.SubtypeId == SubtypeId) RemoveItem(item.ItemId.SubtypeId, item.Amount, item.PricePerUnit, item.Id); }
        else foreach (var item in storeItems) { if (item.ItemId.TypeIdString == TypeId && item.ItemId.SubtypeId == SubtypeId && item.PricePerUnit == price) RemoveItem(item.ItemId.SubtypeId, item.Amount, item.PricePerUnit, item.Id); }
    }

    void RemoveItem(string SubtypeId, int Amount, int PricePerUnit, long Id)
    {
        TradeInfo.AppendLine($"[Remove] Товар {SubtypeId} {Amount} шт по цене {PricePerUnit} снят"); Block.CancelStoreItem(Id);
    }

    bool IsPosted(string TypeId, string SubtypeId, int price, int amount)
    {
        return _storeItems.Exists(x => x.ItemId.TypeIdString == TypeId && x.ItemId.SubtypeId == SubtypeId && x.PricePerUnit == price && x.Amount == amount);
    }

    void InsertOrder(string itemTypeSubtype, int amount, int price)
    {
        long orderId = 0;
        MyDefinitionId definitionId;
        if (MyDefinitionId.TryParse(itemTypeSubtype, out definitionId))
            Block.InsertOrder(new MyStoreItemDataSimple(definitionId, amount, price), out orderId);
        else
            TradeInfo.AppendLine($"ОШИБКА! [MyDefinitionId] НЕ создан. Вероятно указан не правильный itemType");
        if (orderId != 0) TradeInfo.AppendLine($"[Create] Закупка [{itemTypeSubtype}] {amount}шт по цене {price}кр");
        else TradeInfo.AppendLine($"ОШИБКА! Закупка [{itemTypeSubtype}] не удалась! Проверьте имя товара");
    }

    void InsertOffer(string itemTypeSubtype, int amount, int price)
    {
        long orderId = 0;
        MyDefinitionId definitionId;
        if (MyDefinitionId.TryParse(itemTypeSubtype, out definitionId))
            Block.InsertOffer(new MyStoreItemDataSimple(definitionId, amount, price), out orderId);
        else
            TradeInfo.AppendLine($"ОШИБКА! Объект [MyDefinitionId] НЕ создан. Вероятно указан не правильный itemType/ & Subtype");
        if (orderId != 0) TradeInfo.AppendLine($"[Create] Продажа [{itemTypeSubtype}] {amount}шт по цене {price}кр");
        else TradeInfo.AppendLine($"ОШИБКА! Продажа [{itemTypeSubtype}] не удалась! Проверьте имя и цену[{price}]");
    }
}

public class MyAutoStore
{
    int _invenoryCounter = 0, _storeCount = 0;
    internal MyProductBlock StoreComp { get; private set; }
    internal MyProductBlock StoreIng { get; private set; }
    internal MyProductBlock StoreOre { get; private set; }
    internal MyProductBlock StoreTool { get; private set; }
    internal MyProductBlock StoreConsumables { get; private set; }
    internal MyProductBlock StoreSeeds { get; private set; }
    internal Timer TimeCheckStore;
    List<IMyCargoContainer> _containers = new List<IMyCargoContainer>();

    string _infoComponents = "", _infoIngOre = "", _infoTools = "", _infoConsumables = "", _infoSeeds = "";
    internal List<IMyCargoContainer> Containers { get { return _containers; } }
    internal string InfoComponents { get { return _infoComponents; } }
    internal string InfoIngOre { get { return _infoIngOre; } }
    internal string InfoTools { get { return _infoTools; } }
    internal string InfoConsumables { get { return _infoConsumables; } }
    internal string InfoSeeds { get { return _infoSeeds; } }
    internal string Warning { get; private set; } = "";

    internal MyAutoStore(ref bool tradeComponents, ref bool tradeIngots, ref bool tradeOres, ref bool tradeTools, ref bool tradeConsumables, ref bool tradeSeeds, int secondsForUpdate = 3600)
    {
        StoreComp = new MyProductBlock(tradeComponents);
        StoreIng = new MyProductBlock(tradeIngots);
        StoreOre = new MyProductBlock(tradeOres);
        StoreTool = new MyProductBlock(tradeTools);
        StoreConsumables = new MyProductBlock(tradeConsumables);
        StoreSeeds = new MyProductBlock(tradeSeeds);
        TimeCheckStore = new Timer(secondsForUpdate, false);
    }

    internal void GetStoreBlock(IMyGridTerminalSystem TerminalSystem, IMyCubeGrid cubeGrid, ref string[] storeTags)
    {
        List<IMyStoreBlock> AllStoreBlock = new List<IMyStoreBlock>();
        TerminalSystem.GetBlocksOfType(AllStoreBlock, x => x.CubeGrid == cubeGrid);
        foreach (var thisBlock in AllStoreBlock)
        {
            string nameLower = thisBlock.CustomName.ToLower();
            if (nameLower.Contains(storeTags[0].ToLower())) StoreComp.Block = thisBlock;
            if (nameLower.Contains(storeTags[1].ToLower())) StoreIng.Block = thisBlock;
            if (nameLower.Contains(storeTags[2].ToLower())) StoreOre.Block = thisBlock;
            if (nameLower.Contains(storeTags[3].ToLower())) StoreTool.Block = thisBlock;
            if (storeTags.Length > 4 && nameLower.Contains(storeTags[4].ToLower())) StoreConsumables.Block = thisBlock;
            if (storeTags.Length > 5 && nameLower.Contains(storeTags[5].ToLower())) StoreSeeds.Block = thisBlock;
        }
    }

    internal void StoreUpdate(IMyGridTerminalSystem terminalSystem, IMyCubeGrid cubeGrid)
    {
        if (_containers.Count == 0 || _storeCount == 0) GetCargoBlocks(terminalSystem, cubeGrid);
        PlaceOffers();
    }


    void GetCargoBlocks(IMyGridTerminalSystem terminalSystem, IMyCubeGrid cubeGrid)
    {
        Warning = "";
        _containers.Clear();
        if (kGroupContainersForTrade != string.Empty)
        {
            var groupCargo = terminalSystem.GetBlockGroupWithName(kGroupContainersForTrade);
            if (groupCargo != null) groupCargo.GetBlocksOfType<IMyCargoContainer>(_containers);
        }
        else if (kTagContainerForTrade != string.Empty)
            terminalSystem.GetBlocksOfType(_containers, x => x.CubeGrid == cubeGrid && x.CustomName.ToLower().Contains(kTagContainerForTrade.ToLower()));
        if (_containers.Count == 0) terminalSystem.GetBlocksOfType(_containers, x => x.CubeGrid == cubeGrid && !x.CustomName.ToLower().Contains(kTagExclude.ToLower()));
    }

    void PlaceOffers()
    {
        if (_containers.Count == 0) { Warning = "Размещение отменено. Нет конейнеров"; return; }
        if (!SortingContentsInventories()) return;
        switch (_storeCount)
        {
            case 0:
                StoreComp.PlaceOfferingsAndSales(ref Components, "MyObjectBuilder_Component");
                break;
            case 1:
                StoreIng.PlaceOfferingsAndSales(ref Ingots, "MyObjectBuilder_Ingot");
                break;
            case 2:
                StoreOre.PlaceOfferingsAndSales(ref Ores, "MyObjectBuilder_Ore");
                break;
            case 3:
                StoreTool.PlaceOfferingsAndSales(ref Tools, "MyObjectBuilder_PhysicalGunObject");
                StoreTool.PlaceOfferingsAndSales(ref Oxygen, "MyObjectBuilder_OxygenContainerObject", true);
                StoreTool.PlaceOfferingsAndSales(ref Hydrogen, "MyObjectBuilder_GasContainerObject", true);
                StoreTool.PlaceOfferingsAndSales(ref Ammo, "MyObjectBuilder_AmmoMagazine", true);
                break;
            case 4:
                StoreConsumables.PlaceOfferingsAndSales(ref Consumables, "MyObjectBuilder_ConsumableItem");
                break;
            case 5:
                StoreSeeds.PlaceOfferingsAndSales(ref Seeds, "MyObjectBuilder_SeedItem");
                break;
            default:
                _storeCount = 0;
                _containers.Clear();
                TimeCheckStore.Start();
                return;
        }
        _storeCount++;
    }

    bool SortingContentsInventories()
    {
        if (_storeCount > 0) return true;
        if (_invenoryCounter == 0) SetAmountZero();
        if (_invenoryCounter < _containers.Count)
        {
            List<MyInventoryItem> items = new List<MyInventoryItem>();
            _containers[_invenoryCounter].GetInventory().GetItems(items);
            SortingItems(ref items);
            _invenoryCounter++;
            return false;
        }
        else
        {
            CreateListInfo(ref Components, "КОМПОНЕНТЫ", ref _infoComponents);
            MergeIngOreListInfo();
            CreateListInfo(ref Tools, "ИНСТРУМЕНТЫ", ref _infoTools);
            AppendListInfo(ref Oxygen, "БАЛЛОНЫ", ref _infoTools);
            AppendListInfo(ref Hydrogen, "", ref _infoTools);
            AppendListInfo(ref Ammo, "БОЕПРИПАСЫ", ref _infoTools);
            CreateListInfo(ref Consumables, "РАСХОДНИКИ", ref _infoConsumables);
            CreateListInfo(ref Seeds, "СЕМЕНА", ref _infoSeeds);
            _invenoryCounter = 0;
            return true;
        }
    }

    void SetAmountZero()
    {
        foreach (var item in Components) { item.Value.Amount = 0; }
        foreach (var item in Ingots) { item.Value.Amount = 0; }
        foreach (var item in Ores) { item.Value.Amount = 0; }
        foreach (var item in Tools) { item.Value.Amount = 0; }
        foreach (var item in Oxygen) { item.Value.Amount = 0; }
        foreach (var item in Hydrogen) { item.Value.Amount = 0; }
        foreach (var item in Ammo) { item.Value.Amount = 0; }
        foreach (var item in Consumables) { item.Value.Amount = 0; }
        foreach (var item in Seeds) { item.Value.Amount = 0; }
    }

    void SortingItems(ref List<MyInventoryItem> items)
    {
        foreach (var item in items)
        {
            if (item.Type.TypeId == "MyObjectBuilder_Component" && Components.ContainsKey(item.Type.SubtypeId))
                Components[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_Ingot" && Ingots.ContainsKey(item.Type.SubtypeId))
                Ingots[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_Ore" && Ores.ContainsKey(item.Type.SubtypeId))
                Ores[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_PhysicalGunObject" && Tools.ContainsKey(item.Type.SubtypeId))
                Tools[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_OxygenContainerObject" && Oxygen.ContainsKey(item.Type.SubtypeId))
                Oxygen[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_GasContainerObject" && Hydrogen.ContainsKey(item.Type.SubtypeId))
                Hydrogen[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_AmmoMagazine" && Ammo.ContainsKey(item.Type.SubtypeId))
                Ammo[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_ConsumableItem" && Consumables.ContainsKey(item.Type.SubtypeId))
                Consumables[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else if (item.Type.TypeId == "MyObjectBuilder_SeedItem" && Seeds.ContainsKey(item.Type.SubtypeId))
                Seeds[item.Type.SubtypeId].Amount += item.Amount.ToIntSafe();
            else
                Warning += $"\n[{item.Type.TypeId}/{item.Type.SubtypeId}] отсутствует в словарях";
        }
    }

    void MergeIngOreListInfo()
    {
        _infoIngOre = $"\n=== СЛИТКИ / РУДЫ ===";
        foreach (var item in Ingots)
        {
            _infoIngOre += $"\n{TranslateName_components(item.Key)} : {item.Value.Amount} кг";
            if (Ores.ContainsKey(item.Key)) _infoIngOre += $" ( {Math.Round((double)Ores[item.Key].Amount / 1000)} т руды )";
        }
    }

    void AppendListInfo(ref Dictionary<string, MyItem> DictItems, string header, ref string info)
    {
        if (header != "") info += $"\n=== {header} ===";
        WriteItemsListInfo(ref DictItems, ref info);
    }

    void CreateListInfo(ref Dictionary<string, MyItem> DictItems, string header, ref string info)
    {
        info = $"\n=== {header} ===";
        WriteItemsListInfo(ref DictItems, ref info);
    }

    void WriteItemsListInfo(ref Dictionary<string, MyItem> DictItems, ref string info)
    {
        foreach (var Item in DictItems)
        { info += $"\n{TranslateName_components(Item.Key)} : {Item.Value.Amount}"; }
    }

    string TranslateName_components(string name)
    {
        switch (name)
        {
            case "BulletproofGlass": return "Бронированное стекло";
            case "Canvas": return "Парашют";
            case "Computer": return "Компьютеры";
            case "Construction": return "Строительные компоненты";
            case "Detector": return "Компоненты детектора";
            case "Display": return "Экран";
            case "Explosives": return "Взрывчатка";
            case "Girder": return "Балка";
            case "GravityGenerator": return "Компоненты грав. генератора";
            case "InteriorPlate": return "Внутренняя пластина";
            case "LargeTube": return "Большая стальная труба";
            case "Medical": return "Медицинские компоненты";
            case "MetalGrid": return "Компоненты решетки";
            case "Motor": return "Мотор";
            case "PowerCell": return "Энергоячейка";
            case "RadioCommunication": return "Радиокомпоненты";
            case "Reactor": return "Компоненты реактора";
            case "SmallTube": return "Малая трубка";
            case "SolarCell": return "Солнечная панель";
            case "SteelPlate": return "Стальная пластина";
            case "Superconductor": return "Сверхпроводник";
            case "Thrust": return "Детали ионного двигателя";
            case "ZoneChip": return "Ключ безопасности";
            case "Cobalt": return "Кобальт";
            case "Gold": return "Золото";
            case "Stone": return "Камень";
            case "Iron": return "Железо";
            case "Magnesium": return "Магний";
            case "Nickel": return "Никель";
            case "Platinum": return "Платина";
            case "Silicon": return "Кремний";
            case "Silver": return "Серебро";
            case "Uranium": return "Уран";
            case "Ice": return "Лёд";
            case "UltimateAutomaticRifleItem": return "Продвинутая винтовка";
            case "AngleGrinder4Item": return "Элитная болгарка";
            case "HandDrill4Item": return "Элитный ручной бур";
            case "Welder4Item": return "Элитный сварщик";
            case "RapidFireAutomaticRifleItem": return "Скорострельная автоматическая винтовка";
            case "PreciseAutomaticRifleItem": return "Точная винтовка";
            case "OxygenBottle": return "Кислородный баллон";
            case "HydrogenBottle": return "Водородный баллон";
            case "Missile200mm": return "Ракета 200мм";
            case "NATO_25x184mm": return "Боеприпасы 25х184";
            case "NATO_5p56x45mm": return "Магазин 5.56х45mm";
            // Расходники
            case "Medkit": return "Медицинский набор";
            case "Powerkit": return "Энергетический набор";
            case "RadiationKit": return "Противорадиационный набор";
            case "ClangCola": return "Кланг-Кола";
            case "CosmicCoffee": return "Космический кофе";
            case "MealPack_KelpCrisp": return "Хрустящая ламинария";
            case "MealPack_FruitBar": return "Фруктовый батончик";
            case "MealPack_GardenSlaw": return "Огородный салат";
            case "MealPack_RedPellets": return "Красные гранулы";
            case "MealPack_Chili": return "Чили";
            case "MealPack_Ramen": return "Лапша";
            case "MealPack_Flatbread": return "Лепешка";
            case "MealPack_FruitPastry": return "Фруктовая выпечка";
            case "MealPack_VeggieBurger": return "Вегетарианский бургер";
            case "MealPack_GreenPellets": return "Зеленые гранулы";
            case "MealPack_Curry": return "Карри";
            case "MealPack_Dumplings": return "Пельмени";
            case "MealPack_Spaghetti": return "Спагетти";
            case "MealPack_Lasagna": return "Лазанья";
            case "MealPack_Burrito": return "Буррито";
            case "MealPack_FrontierStew": return "Походное рагу";
            case "MealPack_SearedSabiroid": return "Жареный сабироид";
            case "MealPack_SteakDinner": return "Стейк-ужин";
            case "Fruit": return "Фрукты";
            case "Mushrooms": return "Грибы";
            case "Vegetables": return "Овощи";
            case "MammalMeatRaw": return "Сырое мясо млекопитающего";
            case "MammalMeatCooked": return "Приготовленное мясо млекопитающего";
            case "InsectMeatRaw": return "Сырое мясо насекомого";
            case "InsectMeatCooked": return "Приготовленное мясо насекомого";
            case "MealPack_Unknown": return "Неизвестный рацион";
            case "MealPack_FoodPaste": return "Пищевая паста";
            case "MealPack_SynthLoaf": return "Синтетический батон";
            case "MealPack_ClangCrunchies": return "Хрустящие Кланга";
            case "MealPack_BananaBeef": return "Банан-говядина";
            case "MealPack_Hardtack": return "Галета";
            case "MealPack_ExpiredSlop": return "Просроченная похлебка";
            // Семена
            case "Fruit_Seed": return "Семена фруктов";
            case "Grain_Seed": return "Семена зерновых";
            case "Mushrooms_Seed": return "Споры грибов";
            case "Vegetables_Seed": return "Семена овощей";
            default: return name;
        }
    }
}

public class MyItem
{
    internal int BuyPrice { get; set; }
    internal int SalePrice { get; set; }
    internal int Amount { get; set; } = 0;
    internal int MaxAmount { get; set; }
    internal bool AlowSale { get; set; }
    internal bool AlowBuy { get; set; }
    public TradeModel Mode { get; set; }

    public MyItem(int MaxAmount = 0, int BuyPrice = 1, bool AlowBuy = false, int SalePrice = 2, bool AlowSale = false, TradeModel storeMode = TradeModel.Storage)
    {
        this.MaxAmount = MaxAmount;
        this.BuyPrice = BuyPrice;
        this.SalePrice = SalePrice;
        this.AlowBuy = AlowBuy;
        this.AlowSale = AlowSale;
        Mode = storeMode;
    }
}

public class Timer
{
    DateTime _stopTime;
    bool _pause = false;
    int _countdown;
    /// <summary>
            /// Остаток времени таймера
            /// </summary>
    internal string RestTime
    {
        get
        {
            if (Launched)
            {
                var lastTime = _stopTime - DateTime.Now;
                return lastTime.ToString("hh\\:mm\\:ss");
            }
            return _countdown.ToString();
        }
    }
    internal bool Pause { get { return _pause; } set { if (Launched) _pause = true; } }
    /// <summary>
            /// Состояние таймера [Не обновляет сам таймер]
            /// </summary>
    internal bool Launched { get; private set; } = false;
    /// <summary>
            /// Длительность отсчёта (в секундах)
            /// </summary>
    internal int Countdown { get { return _countdown; } set { _countdown = value; } }
    /// <summary>
            /// Таймер обратного отсчёта
            /// </summary>
    internal Timer(int Seconds, bool start) { Countdown = Seconds; if (start) Start(); }
    /// <summary>
            /// Запуск таймера
            /// </summary>
    internal void Start() { Launched = true; _pause = false; _stopTime = DateTime.Now.AddSeconds(Countdown); }
    /// <summary>
            /// Остановка таймера и сброс отсчёта
            /// </summary>
    internal void Stop() { Launched = false; _pause = false; }
    /// <summary>
            /// Проверяет таймер и если он вышел - вернёт ЕДИНОЖДЫ true и остановит отсчёт
            /// </summary>
    internal bool IsFinsh()
    {
        if (Launched && !_pause)
        {
            if (DateTime.Now >= _stopTime) { Stop(); return true; }
        }
        return false;
    }
    /// <summary>
            /// Проверяет остановлен ли таймер и возвращает true если остановлен
            /// </summary>
            /// <returns></returns>
    internal bool IsOut()
    {
        if (_pause) return false;
        IsFinsh(); return !Launched;
    }
}