// ========================================
//         DUTC INVENTORY  v2.0.4
// ========================================
// Сортировка + равномерное распределение по контейнерам, автоназначение контейнеров,
// заправка баллонов (работает!), спецконтейнеры, автокрафт через Custom Data со
// спрайтовым экраном статуса, подача руды в очистители + балансировка руды + приоритет
// очереди, балансировка льда, балансировка урана, загрузка турелей (подгриды + пристыкованные
// корабли), подача в сборщики + очистка, спрайтовые LCD с автоскроллом, выбор главного
// экземпляра (станция бьёт корабль), Nanobot Build & Repair полностью автоматически.
//
// ОБРАТНАЯ СОВМЕСТИМОСТЬ С ISY'S INVENTORY MANAGER:
//   - Те же ключевые слова контейнеров: Ores/Ingots/Components/Tools/Ammo/Bottles
//   - Те же ключевые слова блоков: Special, Locked, Hidden, !manual
//   - Те же ключевые слова коннекторов: [No Sorting] и [No IIM]
//   - Теги LCD от IIM работают: IIM-main, IIM-inventory, IIM-warnings, IIM-actions
//     (также читает секции "@0 IIM-inventory" в Custom Data в стиле IIM)
//   - Строки спецконтейнеров IIM работают: Component/SteelPlate=100, 100M, 100L, All
//   - Формат текста LCD автокрафта IIM работает: "SteelPlate 123 = 5000"
//
// БЫСТРЫЙ СТАРТ
// 1. Вставь в программируемый блок. Запускается сам, аргумент не нужен.
// 2. Назови грузовые контейнеры: Ores / Ingots / Components / Tools / Ammo / Bottles
//    (или пусть скрипт назначит их сам). При useRussian=true используй русские имена.
// 3. Теги LCD (добавить в имя любого LCD или кабины):
//      DUTC-main  (или IIM-main)       = сводка статуса
//      DUTC-inv   (или IIM-inventory)  = списки предметов, фильтры в Custom Data
//      DUTC-warn  (или IIM-warnings)   = предупреждения
//      DUTC-act   (или IIM-actions)    = журнал действий
//      Autocrafting                    = экран запасов, правь нужные количества в CUSTOM DATA
//    Экраны кабины: DUTC-main:1 = второй экран, :2 = третий и т.д.
// 4. Аргументы (необязательно): pause / resume / reset
// 5. Несколько гридов с этим скриптом могут стыковаться безопасно: они выбирают ОДИН
//    активный менеджер автоматически (станция бьёт корабль, затем побеждает меньший id).
//    Остальные экземпляры уходят в ожидание, но сохраняют свои экраны и заправку машин.
//
// ---------------- НАСТРОЙКИ ----------------

// Язык интерфейса и Custom Data: true = русский, false = английский.
// Смена языка вступает в силу только после перезаливки скрипта (перекомпиляции PB).
// Custom Data НИКОГДА не перезаписывается, если она непустая. Шаблон создаётся
// только для пустой Custom Data — на текущем языке.
static bool useRussian = true;

// ==== Ключевые слова контейнеров ====
// Имена блоков-контейнеров, которые скрипт распознаёт как хранилища данного типа.
// Зависят от языка. Совместимость с IIM (английские имена) — при useRussian=false.
string ORE_KEY, INGOT_KEY, COMP_KEY, TOOL_KEY, AMMO_KEY, BOTTLE_KEY, FOOD_KEY;

// Предметы, которые всегда считаются едой, независимо от внутреннего типа мода
string[] foodItems = { "Algae", "Grain" };

// ==== Ключевые слова блоков (НЕ локализуются — совместимость с IIM) ====
string SPECIAL_KEY = "Special";
string HIDDEN_KEY = "Hidden";
string MANUAL_KEY = "!manual";
string[] lockedKeywords = { "Locked", "Control Station", "Control Seat", "Safe Zone" };

// ==== Ключевые слова коннекторов (НЕ локализуются) ====
string NO_SORT_CONNECTOR = "[No Sorting]";
string NO_IIM_CONNECTOR = "[No IIM]";

bool sortItems = true;              // главный выключатель сортировки
bool includeDockedGrids = true;     // тянуть предметы с пристыкованных кораблей (защити один через [No Sorting] / [No IIM])
bool autoAssignContainers = true;   // авто-тег грузового контейнера, когда тип отсутствует/переполнен
// блоки, которые НИКОГДА не должны использоваться как хранилище (не авто-назначаются, теги игнорируются),
// но всё равно опустошаются сортировкой. Совпадение по имени блока или сабтипу, например "Cargo Terminal"
string[] storageBlacklist = { };
// если не пусто, ТОЛЬКО блоки, совпадающие с одним из этих, могут быть авто-назначены как контейнеры типа
string[] storageWhitelist = { };
bool balanceTypeContainers = true;  // равномерно распределять предметы по контейнерам одного типа (9 инструментов / 3 ящика = по 3)
bool showFillLevel = true;          // показывать (xx%) в именах контейнеров типа

bool fillBottles = true;            // прогонять баллоны через бак с газом, чтобы они заправлялись
bool refillStoredBottles = true;    // ТАКЖЕ заправлять баллоны, уже лежащие в контейнере Bottles
int bottleRefillEvery = 30;         // каждые N циклов скрипта (~2с на цикл); прогоняются только контейнеры с НОВЫМИ баллонами
// только эти сабтипы прогоняются через баки — модовые предметы ТИПА "баллон"
// (пауэрбанки, щиты, краскопульты...) сортируются обычным образом и никогда не касаются баков
string[] tankableBottles = { "HydrogenBottle", "OxygenBottle" };

float UI_SCALE = 1.0f;              // множитель размера спрайтового экрана (крупнее текст = меньше строк на экран)

bool enableAutocrafting = true;
string CRAFT_KEY = "Autocrafting";  // ключевое слово в имени LCD для экрана(ов) автокрафта
double craftMargin = 0.05;          // 5% - крафтить, когда ниже wanted*(1-margin)

bool feedRefineries = true;         // держать очистители загруженными рудой
bool balanceRefineries = true;      // выравнивать каждую руду по всем очистителям
bool sortRefineryQueue = true;      // ставить приоритетную руду в начало

bool fillTurrets = true;            // держать турели + стационарные орудия заряженными (включая подгриды)
double magsPerTurret = 20;          // магазинов каждого принимаемого типа боеприпасов на орудие
// порядок переработки, верх = первый:
List<string> orePriority = new List<string>{ "Stone","Iron","Nickel","Cobalt","Silicon","Uranium","Silver","Gold","Platinum","Magnesium","Scrap" };

bool iceBalancing = true;           // распределять лёд по генераторам O2/H2
double iceBottleReserve = 240;      // литров, оставляемых свободными в каждом генераторе под баллоны

bool uraniumBalancing = true;       // держать фиксированное количество урана в каждом реакторе
double uraniumLarge = 100;          // на большой реактор
double uraniumSmall = 25;           // на малый реактор

bool assemblerCleanup = true;       // опустошать простаивающие сборщики обратно в груз
bool feedAssemblers = true;         // предварительно пополнять входы сборщиков слитками, нужными их очереди
double feedQueueDepth = 100;        // запас слитков на столько запланированных крафтов вперёд

bool excludeWelders = true;         // не опустошать сварщики (Build & Repair / Nanobot системы сами себя снабжают)
bool excludeGrinders = false;       // не опустошать болгарки
bool excludeDrills = false;         // не опустошать буры

string[] MAIN_TAGS = { "DUTC-main", "IIM-main" };
string[] INV_TAGS  = { "DUTC-inv", "IIM-inventory" };
string[] WARN_TAGS = { "DUTC-warn", "IIM-warnings" };
string[] ACT_TAGS  = { "DUTC-act", "IIM-actions" };

int maxMovesPerTick = 25;           // перемещений за прогон (производительность)
int scanChunk = 250;                // блоков за тик при сканировании (защита большой базы)

// ---------------- СЛОВАРИ ИМЁН (русский) ----------------

// Обратный словарь: русское имя -> английский SubtypeId (или "Тип/Подтип").
// Строится автоматически из ruNames при инициализации.
// В обратный словарь попадают:
//   - ВСЕ пары "Тип/Подтип" (Ore/*, Ingot/* и т.п.)
//   - имена предметов без слэша (SteelPlate, Motor, ...), КРОМЕ категорий
//     (Ore, Ingot, Component, Tool, Ammo, AmmoMagazine, Bottle, Food, Ice)
// При коллизии приоритет у пары со слэшем.
Dictionary<string, string> ruToEnNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

// Прямой словарь: английский SubtypeId или "Тип/Подтип" -> русское отображаемое имя.
Dictionary<string, string> ruNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
    { "Ore", "РУДА" }, { "Ingot", "СЛИТКИ" }, { "Component", "КОМПОНЕНТЫ" }, { "Tool", "ИНСТРУМЕНТЫ" }, { "Ammo", "БОЕПРИПАСЫ" }, { "AmmoMagazine", "БОЕПРИПАСЫ" }, { "Bottle", "БАЛЛОНЫ" }, { "Food", "ЕДА" }, { "Ice", "Лед" },
    { "Ore/Stone", "Камень" }, { "Ore/Iron", "Железная руда" }, { "Ore/Nickel", "Никелевая руда" }, { "Ore/Cobalt", "Кобальтовая руда" }, { "Ore/Magnesium", "Магниевая руда" }, { "Ore/Silicon", "Кремниевая руда" },
    { "Ore/Silver", "Серебряная руда" }, { "Ore/Gold", "Золотая руда" }, { "Ore/Platinum", "Платиновая руда" }, { "Ore/Uranium", "Урановая руда" }, { "Ore/Ice", "Лед" }, { "Ore/Scrap", "Металлолом" },
    { "Ingot/Stone", "Гравий" }, { "Ingot/Iron", "Железный слиток" }, { "Ingot/Nickel", "Никелевый слиток" }, { "Ingot/Cobalt", "Кобальтовый слиток" }, { "Ingot/Magnesium", "Магниевый слиток" }, { "Ingot/Silicon", "Кремниевая пластина" },
    { "Ingot/Silver", "Серебряный слиток" }, { "Ingot/Gold", "Золотой слиток" }, { "Ingot/Platinum", "Платиновый слиток" }, { "Ingot/Uranium", "Урановый слиток" }, { "Ingot/Caixirite_Raw_Compound", "Необр. кайксирит" }
};

// Локализованная строка: en для английского, ru для русского. Возвращает по useRussian.
string L(string en, string ru)
{
    return useRussian ? ru : en;
}
// ==== ТЕЛО СКРИПТА ====
const int ORE=0, INGOT=1, COMP=2, TOOL=3, AMMO=4, BOTTLE=5, FOOD=6, NCAT=7;
// Теги контейнеров на обоих языках — для распознавания имён блоков независимо от текущего языка.
// Заполняются один раз при загрузке класса (static readonly).
string[] catKeysEn = { "Ores", "Ingots", "Components", "Tools", "Ammo", "Bottles", "Food" };
string[] catKeysRu = { "Руда", "Слитки", "Компоненты", "Инструменты", "Боеприпасы", "Баллоны", "Еда" };
string[] catKeys;
List<IMyTerminalBlock>[] cats = new List<IMyTerminalBlock>[NCAT];
List<IMyTerminalBlock> allInv = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> specials = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> pendingSpecials = new List<IMyTerminalBlock>();
List<IMyCargoContainer> untagged = new List<IMyCargoContainer>();
List<IMyRefinery> refineries = new List<IMyRefinery>();
List<IMyAssembler> assemblers = new List<IMyAssembler>();
List<IMyAssembler> distinctAsm = new List<IMyAssembler>();
List<IMyGasGenerator> gens = new List<IMyGasGenerator>();
List<IMyReactor> reactors = new List<IMyReactor>();
List<IMyGasTank> tanks = new List<IMyGasTank>();
List<IMyUserControllableGun> guns = new List<IMyUserControllableGun>();
List<IMyTextPanel> craftLCDs = new List<IMyTextPanel>();
List<IMyTerminalBlock> mainHolders = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> invHolders = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> warnHolders = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> actHolders = new List<IMyTerminalBlock>();
Dictionary<MyItemType,double> stock = new Dictionary<MyItemType,double>();
Dictionary<string,MyItemType> byName = new Dictionary<string,MyItemType>();
Dictionary<MyDefinitionId,double> queued = new Dictionary<MyDefinitionId,double>();
Dictionary<string,MyDefinitionId> bpCache = new Dictionary<string,MyDefinitionId>();
HashSet<string> noBp = new HashSet<string>();
List<string> warnings = new List<string>();
HashSet<string> warnSet = new HashSet<string>();
List<string> actions = new List<string>();
Dictionary<int,int> scroll = new Dictionary<int,int>();
Dictionary<long,double> bottleMem = new Dictionary<long,double>();
List<IMyTerminalBlock> scanBlocks = new List<IMyTerminalBlock>();
List<IMyCubeGrid> noSortGrids = new List<IMyCubeGrid>();
int scanPos = 0;
Dictionary<long,bool> convCache = new Dictionary<long,bool>();
Dictionary<long,bool> reachCache = new Dictionary<long,bool>();
int reachBudget = 0;
int step = 0;
int sortIdx = 0;
int gunIdx = 0;
int moved = 0;
int tick = 0;
int cycles = 0;
bool paused = false;
bool standby = false;
string masterName = "";
string DUTC_MARKER = "[DUTC-INV-ACTIVE]";
string lastError = "";
string[] stepNames;

public Program()
{
    // ==== Ключи контейнеров — зависят от языка ====
    ORE_KEY    = useRussian ? "Руда"        : "Ores";
    INGOT_KEY  = useRussian ? "Слитки"      : "Ingots";
    COMP_KEY   = useRussian ? "Компоненты"  : "Components";
    TOOL_KEY   = useRussian ? "Инструменты" : "Tools";
    AMMO_KEY   = useRussian ? "Боеприпасы"  : "Ammo";
    BOTTLE_KEY = useRussian ? "Баллоны"     : "Bottles";
    FOOD_KEY   = useRussian ? "Еда"         : "Food";
    
    catKeys = new[]{ ORE_KEY, INGOT_KEY, COMP_KEY, TOOL_KEY, AMMO_KEY, BOTTLE_KEY, FOOD_KEY };
    
    // ==== Имена шагов (для Echo и логов ошибок) ====
    stepNames = new[] {
        L("Scan","Сканирование"),
        L("Count","Подсчёт"),
        L("Assign","Назначение"),
        L("Sort","Сортировка"),
        L("Balance","Балансировка"),
        L("Special","Спецконтейнеры"),
        L("Craft","Крафт"),
        L("Refineries","Очистители"),
        L("Ice","Лёд"),
        L("Uranium","Уран"),
        L("Turrets","Турели"),
        L("Cleanup","Очистка"),
        L("Screens","Экраны")
    };
    
    for (int i = 0; i < NCAT; i++) cats[i] = new List<IMyTerminalBlock>();
    
    InitRecipes();
    InitRuNames();
    
    if (!string.IsNullOrEmpty(Storage))
    {
        var parts = Storage.Split('|');
        if (parts.Length >= 2)
        {
            foreach (var e in parts[0].Split(';'))
            {
                int gt = e.IndexOf('>');
                if (gt <= 0) continue;
                MyDefinitionId id;
                if (MyDefinitionId.TryParse(e.Substring(gt + 1), out id)) bpCache[e.Substring(0, gt)] = id;
            }
            foreach (var nm in parts[1].Split(';')) if (nm.Length > 0) noBp.Add(nm);
        }
    }
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

// Строит ruToEnNames из ruNames по правилу:
//   - все пары "Тип/Подтип" (Ore/*, Ingot/*, ...) идут в обратный словарь с высшим приоритетом
//   - имена без слэша (SteelPlate, Motor, ...) идут, КРОМЕ категорий
//   - при коллизии побеждает пара со слэшем
void InitRuNames()
{
    var categories = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
        "Ore","Ingot","Component","Tool","Ammo","AmmoMagazine","Bottle","Food","Ice"
    };
    
    // Проход 1: пары со слэшем (приоритет)
    foreach (var kv in ruNames)
    {
        if (kv.Key.IndexOf('/') < 0) continue;
        ruToEnNames[kv.Value] = kv.Key;
    }
    // Проход 2: имена без слэша (кроме категорий), не перезаписывая уже занятые
    foreach (var kv in ruNames)
    {
        if (kv.Key.IndexOf('/') >= 0) continue;
        if (categories.Contains(kv.Key)) continue;
        if (!ruToEnNames.ContainsKey(kv.Value)) ruToEnNames[kv.Value] = kv.Key;
    }
    
    // Ручные дополнения для оружия/инструментов/боеприпасов/баллонов
    ruNames["SemiAutoPistolItem"] = "S-10 Пистолет"; ruNames["ElitePistolItem"] = "S-10E Пистолет"; ruNames["FullAutoPistolItem"] = "S-20A Пистолет";
    ruNames["AutomaticRifleItem"] = "MR-20 Винтовка"; ruNames["PreciseAutomaticRifleItem"] = "MR-8P Винтовка"; ruNames["RapidFireAutomaticRifleItem"] = "MR-50A Винтовка"; ruNames["UltimateAutomaticRifleItem"] = "MR-30E Винтовка";
    ruNames["BasicHandHeldLauncherItem"] = "RO-1 Ракетница"; ruNames["AdvancedHandHeldLauncherItem"] = "PRO-1 Ракетница";
    ruNames["WelderItem"] = "Сварщик"; ruNames["Welder2Item"] = "* Улучшенный сварщик"; ruNames["Welder3Item"] = "** Продвинутый сварщик"; ruNames["Welder4Item"] = "*** Элитный сварщик";
    ruNames["AngleGrinderItem"] = "Болгарка"; ruNames["AngleGrinder2Item"] = "* Улучшенная болгарка"; ruNames["AngleGrinder3Item"] = "** Продвинутая болгарка"; ruNames["AngleGrinder4Item"] = "*** Элитная болгарка";
    ruNames["HandDrillItem"] = "Ручной бур"; ruNames["HandDrill2Item"] = "* Улучшенный ручной бур"; ruNames["HandDrill3Item"] = "** Продвинутый ручной бур"; ruNames["HandDrill4Item"] = "*** Элитный ручной бур";
    ruNames["Construction"] = "Стройкомпоненты"; ruNames["MetalGrid"] = "Компонент решётки"; ruNames["InteriorPlate"] = "Внутренняя пластина";
    ruNames["SteelPlate"] = "Стальная пластина"; ruNames["Girder"] = "Балка"; ruNames["SmallTube"] = "Малая труба"; ruNames["LargeTube"] = "Большая труба"; ruNames["Motor"] = "Мотор";
    ruNames["Display"] = "Экран"; ruNames["BulletproofGlass"] = "Бронестекло"; ruNames["Computer"] = "Компьютер"; ruNames["Reactor"] = "Компоненты реактора"; ruNames["Thrust"] = "Детали ускорителя";
    ruNames["GravityGenerator"] = "Гравикомпоненты"; ruNames["Medical"] = "Медкомпоненты"; ruNames["RadioCommunication"] = "Радиокомпоненты"; ruNames["Detector"] = "Компоненты детектора";
    ruNames["Explosives"] = "Взрывчатка"; ruNames["SolarCell"] = "Солнечная ячейка"; ruNames["PowerCell"] = "Энергоячейка"; ruNames["Superconductor"] = "Сверхпроводник";
    ruNames["Canvas"] = "Полотно парашюта"; ruNames["ZoneChip"] = "Чип"; ruNames["Medkit"] = "Аптечка"; ruNames["Powerkit"] = "Внешний аккумулятор";
    ruNames["ClangCola"] = "Кола"; ruNames["CosmicCoffee"] = "Кофе"; ruNames["SpaceCredit"] = "Кредиты"; ruNames["NATO_5p56x45mm"] = "5.56x45mm";
    ruNames["SemiAutoPistolMagazine"] = "S-10 Mag"; ruNames["ElitePistolMagazine"] = "S-10E Mag"; ruNames["FullAutoPistolMagazine"] = "S-20A Mag";
    ruNames["AutomaticRifleGun_Mag_20rd"] = "MR-20 Mag"; ruNames["PreciseAutomaticRifleGun_Mag_5rd"] = "MR-8P Mag"; ruNames["RapidFireAutomaticRifleGun_Mag_50rd"] = "MR-50A Mag"; ruNames["UltimateAutomaticRifleGun_Mag_30rd"] = "MR-30E Mag";
    ruNames["NATO_25x184mm"] = "Гатлинг патроны"; ruNames["Missile200mm"] = "200мм ракета"; ruNames["AutocannonClip"] = "М-н автопушки"; ruNames["MediumCalibreAmmo"] = "Снаряд ШП";
    ruNames["SmallRailgunAmmo"] = "МС Рельсотрон"; ruNames["LargeRailgunAmmo"] = "БС Рельсотрон"; ruNames["LargeCalibreAmmo"] = "АРТ Снаряд"; ruNames["FlareClip"] = "Flare Clip";
    ruNames["OxygenBottle"] = "Кислородные баллоны"; ruNames["HydrogenBottle"] = "Водородные баллоны";
    
    // Перестраиваем обратный словарь с учётом добавленного
    foreach (var kv in ruNames)
    {
        if (kv.Key.IndexOf('/') < 0) continue;
        ruToEnNames[kv.Value] = kv.Key;
    }
    foreach (var kv in ruNames)
    {
        if (kv.Key.IndexOf('/') >= 0) continue;
        if (categories.Contains(kv.Key)) continue;
        if (!ruToEnNames.ContainsKey(kv.Value)) ruToEnNames[kv.Value] = kv.Key;
    }
}

public void Save()
{
    var sb = new StringBuilder();
    foreach (var kv in bpCache) sb.Append(kv.Key).Append('>').Append(kv.Value.ToString()).Append(';');
    sb.Append('|');
    foreach (var nm in noBp) sb.Append(nm).Append(';');
    Storage = sb.ToString();
}

public void Main(string arg)
{
    arg = arg.Trim().ToLower();
    if (arg == "pause") paused = true;
    if (arg == "resume" || arg == "run") paused = false;
    if (arg == "reset") { bpCache.Clear(); noBp.Clear(); probePos.Clear(); convCache.Clear(); reachCache.Clear(); barCache.Clear(); Storage = ""; lastError = ""; }
    tick++;
    Echo("DUTC INVENTORY " + "|/-\\"[tick % 4]);
    Echo(L("Step: ","Шаг: ") + stepNames[step]);
    Echo(L("Inventories: ","Инвентарей: ") + allInv.Count + L("  Specials: ","  Спецконтейнеров: ") + specials.Count);
    Echo(L("Warnings: ","Предупреждений: ") + warnings.Count);
    if (standby) Echo(L("STANDBY - sorting handled by:\n","ОЖИДАНИЕ — сортировкой управляет:\n") + masterName);
    if (lastError != "") Echo(L("Last error: ","Последняя ошибка: ") + lastError);
    if (paused) { Echo(L("PAUSED - run with 'resume'","ПАУЗА — введите 'resume'")); return; }
    try
    {
        bool done = DoStep(step);
        if (done) step = (step + 1) % 13;
    }
    catch (Exception e)
    {
        lastError = stepNames[step] + ": " + e.Message;
        Warn(L("Script error in step ","Ошибка скрипта на шаге ") + stepNames[step]);
        step = (step + 1) % 13;
    }
}

bool DoStep(int s)
{
    if (standby && (s == 2 || s == 3 || s == 5 || s == 6)) return true;
    switch (s)
    {
        case 0: return Scan();
        case 1: return Count();
        case 2: return Assign();
        case 3: return Sort();
        case 4: return Balance();
        case 5: return SpecialFill();
        case 6: return Craft();
        case 7: return Refine();
        case 8: return Ice();
        case 9: return Uranium();
        case 10: return Turrets();
        case 11: return Cleanup();
        default: return Screens();
    }
}
bool Scan()
{
    if (scanPos == 0)
    {
        cycles++;
        warnings.Clear(); warnSet.Clear();
        if (cycles % 150 == 0) { convCache.Clear(); reachCache.Clear(); }
        if (!Me.CustomData.Contains(DUTC_MARKER)) Me.CustomData = DUTC_MARKER + "\n" + Me.CustomData;
        standby = false; masterName = "";
        var pbs = new List<IMyProgrammableBlock>();
        GridTerminalSystem.GetBlocksOfType(pbs, p => p != Me && p.IsWorking && p.CustomData.Contains(DUTC_MARKER));
        foreach (var p in pbs)
        {
            bool meStatic = Me.CubeGrid.IsStatic;
            bool otherStatic = p.CubeGrid.IsStatic;
            bool otherWins;
            if (otherStatic != meStatic) otherWins = otherStatic;
            else otherWins = p.EntityId < Me.EntityId;
            if (otherWins)
            {
                standby = true;
                masterName = p.CustomName + L(" on '"," на '") + p.CubeGrid.CustomName + "'";
                break;
            }
        }
        for (int i = 0; i < NCAT; i++) cats[i].Clear();
        allInv.Clear(); specials.Clear(); untagged.Clear();
        refineries.Clear(); assemblers.Clear(); gens.Clear(); reactors.Clear(); tanks.Clear(); guns.Clear();
        noSortGrids.Clear();
        var conns = new List<IMyShipConnector>();
        GridTerminalSystem.GetBlocksOfType(conns);
        foreach (var c in conns)
        {
            if (c.Status != MyShipConnectorStatus.Connected) continue;
            if (!c.CustomName.Contains(NO_SORT_CONNECTOR) && !c.CustomName.Contains(NO_IIM_CONNECTOR)) continue;
            var o = c.OtherConnector; if (o == null) continue;
            if (c.CubeGrid.IsSameConstructAs(Me.CubeGrid)) noSortGrids.Add(o.CubeGrid);
            else noSortGrids.Add(c.CubeGrid);
        }
        GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(scanBlocks, b => b.HasInventory);
    }
    int processed = 0;
    for (; scanPos < scanBlocks.Count; scanPos++)
    {
        if (processed >= scanChunk || Runtime.CurrentInstructionCount > 30000) return false;
        var b = scanBlocks[scanPos];
        processed++;
        string n = b.CustomName;
        bool same = b.IsSameConstructAs(Me);
        if (!same)
        {
            if (!includeDockedGrids) continue;
            bool skip = false;
            foreach (var g in noSortGrids) if (b.CubeGrid.IsSameConstructAs(g)) { skip = true; break; }
            if (skip) continue;
        }
        bool locked = false;
        foreach (var k in lockedKeywords) if (n.Contains(k)) { locked = true; break; }
        if (locked) continue;
        string def = b.BlockDefinition.TypeIdString;
        if (def.Contains("Parachute") || def.Contains("VendingMachine") || def.Contains("StoreBlock")) continue;
        bool manual = n.Contains(MANUAL_KEY);
        if (n.Contains(SPECIAL_KEY)) { specials.Add(b); if (b.CustomData.Trim().Length == 0) pendingSpecials.Add(b); continue; }
        allInv.Add(b);
        bool tagged = false;
        bool storageBlock = !(b is IMyProductionBlock) && !(b is IMyReactor) && !(b is IMyGasGenerator) && !(b is IMyGasTank);
        bool storBlack = MatchList(b, storageBlacklist);
        if (same && storageBlock && !storBlack)
            for (int c = 0; c < NCAT; c++)
                if (MatchesCat(n, c)) { cats[c].Add(b); tagged = true; }
                if (b is IMyRefinery) { if (!manual && same && b.IsWorking) refineries.Add((IMyRefinery)b); }
                else if (b is IMyAssembler) { if (!manual && same && b.IsWorking) assemblers.Add((IMyAssembler)b); }
                else if (b is IMyGasGenerator) { if (!manual && same && b.IsFunctional && ((IMyFunctionalBlock)b).Enabled) gens.Add((IMyGasGenerator)b); }
                else if (b is IMyReactor) { if (!manual && same && b.IsFunctional && ((IMyFunctionalBlock)b).Enabled) reactors.Add((IMyReactor)b); }
                else if (b is IMyGasTank) { if (!manual && same && b.IsWorking) tanks.Add((IMyGasTank)b); }
                else if (b is IMyUserControllableGun) { if (!manual && b.IsFunctional) guns.Add((IMyUserControllableGun)b); }
                else if (b is IMyCargoContainer && same && !tagged && !storBlack && !n.Contains(HIDDEN_KEY)
                    && (storageWhitelist.Length == 0 || MatchList(b, storageWhitelist))) untagged.Add((IMyCargoContainer)b);
    }
    scanPos = 0;
    distinctAsm.Clear();
    foreach (var a in assemblers)
    {
        bool dup = false;
        foreach (var d in distinctAsm) if (d.BlockDefinition.ToString() == a.BlockDefinition.ToString()) { dup = true; break; }
        if (!dup) distinctAsm.Add(a);
    }
    for (int c = 0; c < NCAT; c++) cats[c].Sort((a, z) => a.CustomName.CompareTo(z.CustomName));
    untagged.Sort((a, z) => ((double)z.GetInventory(0).MaxVolume).CompareTo((double)a.GetInventory(0).MaxVolume));
    craftLCDs.Clear();
    GridTerminalSystem.GetBlocksOfType(craftLCDs, p => p.IsSameConstructAs(Me) && p.CustomName.Contains(CRAFT_KEY));
    craftLCDs.Sort((a, z) => a.CustomName.CompareTo(z.CustomName));
    mainHolders.Clear(); invHolders.Clear(); warnHolders.Clear(); actHolders.Clear();
    var scr = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(scr, x => x.IsSameConstructAs(Me));
    foreach (var x in scr)
    {
        if (HasTag(x, MAIN_TAGS)) mainHolders.Add(x);
        if (HasTag(x, INV_TAGS)) invHolders.Add(x);
        if (HasTag(x, WARN_TAGS)) warnHolders.Add(x);
        if (HasTag(x, ACT_TAGS)) actHolders.Add(x);
    }
    return true;
}

// Сравнение имени блока с тегом категории — принимает оба языка.
bool MatchesCat(string blockName, int c)
{
    if (blockName.Contains(catKeysEn[c])) return true;
    if (blockName.Contains(catKeysRu[c])) return true;
    return false;
}

bool HasTag(IMyTerminalBlock b, string[] tags)
{
    foreach (var t in tags)
        if (b.CustomName.Contains(t) || b.CustomData.Contains(t)) return true;
        return false;
}

bool ContainsAny(string s, string[] tags)
{
    foreach (var t in tags) if (s.Contains(t)) return true;
    return false;
}

bool Count()
{
    stock.Clear(); byName.Clear();
    var items = new List<MyInventoryItem>();
    for (int pass = 0; pass < 2; pass++)
    {
        var list = pass == 0 ? allInv : specials;
        foreach (var b in list)
        {
            if (b.CustomName.Contains(HIDDEN_KEY)) continue;
            for (int q = 0; q < b.InventoryCount; q++)
            {
                items.Clear(); b.GetInventory(q).GetItems(items);
                foreach (var it in items)
                {
                    double a = (double)it.Amount;
                    if (stock.ContainsKey(it.Type)) stock[it.Type] += a; else stock[it.Type] = a;
                }
            }
        }
    }
    // Заполняем byName: английский SubtypeId, "Тип/Подтип" и русские алиасы
    foreach (var t in stock.Keys)
    {
        byName[t.SubtypeId] = t;
        byName[t.TypeId.Replace("MyObjectBuilder_", "") + "/" + t.SubtypeId] = t;
        if (useRussian)
        {
            string enKey = t.TypeId.Replace("MyObjectBuilder_", "") + "/" + t.SubtypeId;
            string ruName;
            // Приоритет: пара "Тип/Подтип", затем просто SubtypeId
            if (ruNames.TryGetValue(enKey, out ruName)) byName[ruName] = t;
            else if (ruNames.TryGetValue(t.SubtypeId, out ruName)) byName[ruName] = t;
        }
    }
    return true;
}

bool Assign()
{
    if (autoAssignContainers)
    {
        reachBudget = 3;
        for (int c = 0; c < NCAT; c++)
        {
            bool need = cats[c].Count == 0;
            if (!need)
            {
                need = true;
                foreach (var w in cats[c]) if (HasSpace(w.GetInventory(0))) { need = false; break; }
            }
            if (!need) continue;
            IMyCargoContainer box = null;
            foreach (var u in untagged) { if (Reachable(u)) { box = u; break; } }
            if (box == null) { Warn(L("No conveyor-connected cargo container free to assign for '","Нет свободного подключённого контейнера для '") + catKeys[c] + "'!"); continue; }
            untagged.Remove(box);
            box.CustomName = box.CustomName + " " + catKeys[c];
            cats[c].Add(box);
            Act(L("Assigned '","Назначен '") + box.CustomName + L("' for ","' для ") + catKeys[c]);
        }
    }
    if (showFillLevel)
    {
        for (int c = 0; c < NCAT; c++)
            foreach (var w in cats[c])
            {
                var inv = w.GetInventory(0);
                string nm = System.Text.RegularExpressions.Regex.Replace(w.CustomName, @"\s*\(\d+\.?\d*%\)", "");
                string want = nm + " (" + Pct((double)inv.CurrentVolume, (double)inv.MaxVolume) + ")";
                if (w.CustomName != want) w.CustomName = want;
            }
    }
    return true;
}
bool Sort()
{
    if (!sortItems) return true;
    moved = 0;
    var items = new List<MyInventoryItem>();
    for (; sortIdx < allInv.Count; sortIdx++)
    {
        if (moved >= maxMovesPerTick || Runtime.CurrentInstructionCount > 35000) return false;
        var b = allInv[sortIdx];
        if (b is IMyReactor) continue;
        if (b is IMyUserControllableGun) continue;
        if (b is IMyShipWelder && (excludeWelders || IsBaR(b))) continue;
        if (excludeGrinders && b is IMyShipGrinder) continue;
        if (excludeDrills && b is IMyShipDrill) continue;
        if (b.CustomName.Contains(HIDDEN_KEY)) continue;
        bool foreignGrid = !b.IsSameConstructAs(Me);
        var inv = b.GetInventory(SrcInvIndex(b));
        if (inv.ItemCount == 0) continue;
        items.Clear(); inv.GetItems(items);
        for (int i = items.Count - 1; i >= 0; i--)
        {
            var it = items[i];
            int c = Cat(it.Type);
            if (c < 0) continue;
            if (b is IMyGasGenerator && IsIce(it.Type)) continue;
            if (IsCat(b, c)) continue;
            if (foreignGrid && MatchesCat(b.CustomName, c)) continue; // respect docked ships' own type containers
            IMyInventory dst = null; string dstName = ""; long dstId = 0;
            if (c == BOTTLE && fillBottles && !(b is IMyGasTank) && Tankable(it.Type))
            {
                var tk = FindTank(it.Type);
                if (tk != null) { dst = tk.GetInventory(0); dstName = tk.CustomName; dstId = tk.EntityId; }
            }
            if (dst == null)
            {
                var w = DestFor(c, b);
                if (w == null) { Warn(L("All '","Все контейнеры '") + catKeys[c] + L("' containers are full!","' переполнены!")); continue; }
                dst = w.GetInventory(0); dstName = w.CustomName; dstId = w.EntityId;
            }
            if (!CanMove(inv, b.EntityId, dst, dstId, it.Type))
            {
                Warn(L("No conveyor path: '","Нет пути конвейера: '") + b.CustomName + L("' -> '","' -> '") + dstName + "'");
                continue;
            }
            if (inv.TransferItemTo(dst, i, null, true))
            {
                moved++;
                Act(ItemName(it.Type) + ": " + StripPct(b.CustomName) + " -> " + StripPct(dstName));
            }
            if (moved >= maxMovesPerTick) break;
        }
    }
    sortIdx = 0;
    return true;
}

bool Balance()
{
    if (!balanceTypeContainers) return true;
    var items = new List<MyInventoryItem>();
    for (int c = 0; c < NCAT; c++)
    {
        if (cats[c].Count < 2) continue;
        int n = cats[c].Count;
        var totals = new Dictionary<MyItemType, double>();
        foreach (var w in cats[c])
        {
            items.Clear(); w.GetInventory(0).GetItems(items);
            foreach (var it in items)
            {
                if (Cat(it.Type) != c) continue;
                double a; totals.TryGetValue(it.Type, out a);
                totals[it.Type] = a + (double)it.Amount;
            }
        }
        foreach (var kv in totals)
        {
            var t = kv.Key;
            double avg = kv.Value / n;
            double tol = Math.Max(1, avg * 0.05);
            foreach (var rich in cats[c])
            {
                var rInv = rich.GetInventory(0);
                double cur = (double)rInv.GetItemAmount(t);
                if (cur <= avg + tol) continue;
                foreach (var poor in cats[c])
                {
                    if (poor == rich) continue;
                    var pInv = poor.GetInventory(0);
                    double pc = (double)pInv.GetItemAmount(t);
                    if (pc >= avg - tol) continue;
                    if (!HasSpace(pInv)) continue;
                    if (Runtime.CurrentInstructionCount > 28000) return true;
                    double move = Math.Floor(Math.Min(cur - avg, avg - pc) + 0.001);
                    if (move < 1) continue;
                    cur -= PushFrom(rInv, t, move, pInv);
                    if (cur <= avg + tol) break;
                }
            }
            if (Runtime.CurrentInstructionCount > 30000) return true;
        }
    }
    return true;
}

bool SpecialFill()
{
    // Генерируем шаблоны Custom Data для пустых спецконтейнеров — теперь, когда stock заполнен.
    if (pendingSpecials.Count > 0)
    {
        foreach (var b in pendingSpecials) EnsureSpecialTemplate(b);
        pendingSpecials.Clear();
    }
    foreach (var b in specials)
    {
        var inv = b.GetInventory(0);
        
        // Собираем список распознанных имён и флаг «есть рабочие строки»
        var wanted = new HashSet<MyItemType>();
        bool anyWorkingLine = false;
        
        foreach (var raw in b.CustomData.Split('\n'))
        {
            var l = raw.Trim();
            if (l.Length == 0 || l.StartsWith("#") || l.StartsWith("@") || l.StartsWith("-")) continue;
            int eq = l.IndexOf('=');
            if (eq <= 0) continue;
            string name = l.Substring(0, eq).Trim();
            MyItemType t;
            if (!byName.TryGetValue(name, out t)) continue;   // нераспознанные игнорируем
            anyWorkingLine = true;
            string val = l.Substring(eq + 1).Trim().ToLower();
            double want = 0; string mode = "n";
            if (val.Contains("all")) { want = 1000000000; mode = "a"; }
            else
            {
                if (!double.TryParse(System.Text.RegularExpressions.Regex.Match(val, @"\d+\.?\d*").Value, out want)) continue;
                if (val.Contains("m")) mode = "m";
                else if (val.Contains("l")) mode = "l";
            }
            wanted.Add(t);
            double cur = (double)inv.GetItemAmount(t);
            if (mode != "l" && cur < want - 0.01)
            {
                if (!HasSpace(inv)) continue;
                double got = PullTo(inv, t, want - cur, b);
                if (mode != "a" && got < want - cur - 0.01)
                    Warn("'" + StripPct(b.CustomName) + L("' missing ","' не хватает ") + Math.Round(want - cur - got) + " " + ItemName(t));
            }
            else if ((mode == "n" || mode == "l") && cur > want + 0.01)
            {
                int c = Cat(t);
                if (c < 0) continue;
                var w = DestFor(c, b);
                if (w != null) PushFrom(inv, t, cur - want, w.GetInventory(0));
            }
        }
        
        // Строгий режим: если есть хоть одна рабочая строка — выкидываем всё лишнее
        if (!anyWorkingLine) continue;
        
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);
        for (int i = items.Count - 1; i >= 0; i--)
        {
            var it = items[i];
            if (wanted.Contains(it.Type)) continue;   // указан — оставляем
            int c = Cat(it.Type);
            if (c < 0) continue;                       // неизвестный тип — не трогаем
            var w = DestFor(c, b);
            if (w == null)
            {
                Warn(L("No room to dump ","Негде разместить ") + ItemName(it.Type) + L(" from special '"," из спецконтейнера '") + StripPct(b.CustomName) + "'");
                continue;
            }
            inv.TransferItemTo(w.GetInventory(0), i, null, true);
            Act(L("Dumped ","Выброшен ") + ItemName(it.Type) + L(" from "," из ") + StripPct(b.CustomName) + " -> " + StripPct(w.CustomName));
        }
    }
    return true;
}
string CRAFT_MARKER = "# DUTC Autocrafting";
Dictionary<MyDefinitionId,double> ourQueued = new Dictionary<MyDefinitionId,double>();
Dictionary<string,string> knownBp = new Dictionary<string,string>
{
    { "AQD_Comp_Concrete", "AQD_BP_StoneIngot_To_Concrete" }
};
Dictionary<string,int> probePos = new Dictionary<string,int>();
string[] vanillaCraft = { "SteelPlate","InteriorPlate","Construction","MetalGrid","SmallTube","LargeTube","Motor","Display","BulletproofGlass","Computer","Reactor","Thrust","GravityGenerator","Medical","RadioCommunication","Detector","Explosives","Girder","SolarCell","PowerCell","Superconductor","Canvas" };
int probeBudget = 0;
class CRow
{
    public string name = "";
    public double cur;
    public double want;
    public int state;   // 0 = normal, -1 = NoBP, -2 = ignored, -3 = still checking
    public double inQ;
}
List<CRow> craftRows = new List<CRow>();

bool Craft()
{
    if (!enableAutocrafting || craftLCDs.Count == 0) return true;
    if (assemblers.Count == 0) { Warn(L("Autocrafting: no usable assemblers!","Автокрафт: нет работающих сборщиков!")); return true; }
    probeBudget = 1;
    queued.Clear();
    var q = new List<MyProductionItem>();
    foreach (var a in assemblers)
    {
        q.Clear();
        try { a.GetQueue(q); } catch { continue; }
        foreach (var pi in q)
        {
            double amt; queued.TryGetValue(pi.BlueprintId, out amt);
            queued[pi.BlueprintId] = amt + (double)pi.Amount;
        }
    }
    var okeys = new List<MyDefinitionId>(ourQueued.Keys);
    foreach (var k in okeys)
    {
        double actual; queued.TryGetValue(k, out actual);
        if (ourQueued[k] > actual) ourQueued[k] = actual;
    }
    var master = craftLCDs[0];
    if (!master.CustomData.Contains(CRAFT_MARKER)) SetupCraftCD();
    var entries = new List<string[]>();
    var seen = new HashSet<string>();
    foreach (var p in craftLCDs)
    {
        foreach (var raw in p.CustomData.Split('\n'))
        {
            var l = raw.Trim();
            if (l.Length == 0 || l.StartsWith("#")) continue;
            int eq = l.IndexOf('=');
            if (eq <= 0) continue;
            string name = l.Substring(0, eq).Trim();
            string val = l.Substring(eq + 1).Trim();
            string bpOv = "";
            var mo = System.Text.RegularExpressions.Regex.Match(val, @"(?i)bp:([\w/]+)");
            if (mo.Success) { bpOv = mo.Groups[1].Value; val = val.Replace(mo.Value, ""); }
            double want;
            double.TryParse(System.Text.RegularExpressions.Regex.Match(val, @"\d+").Value, out want);
            string mods = System.Text.RegularExpressions.Regex.Replace(val, @"[\d\.\s]", "").ToUpper();
            if (name == "" || seen.Contains(name)) continue;
            seen.Add(name);
            entries.Add(new[] { name, want.ToString(), mods, bpOv });
        }
    }
    var newLines = new StringBuilder();
    foreach (var kv in stock)
    {
        var t = kv.Key;
        if (!t.TypeId.EndsWith("_Component") && !t.TypeId.EndsWith("_AmmoMagazine") && Cat(t) != FOOD && !t.TypeId.EndsWith("_PhysicalGunObject") && !t.TypeId.EndsWith("_PhysicalObject")) continue;
        string displayName = ItemName(t);
        if (seen.Contains(displayName)) continue;
        MyDefinitionId bp0;
        if (BpState(t.SubtypeId, out bp0, false) != 1) continue;
        seen.Add(displayName);
        entries.Add(new[] { displayName, "0", "", "" });
        newLines.Append(displayName + "=0\n");
    }
    if (newLines.Length > 0)
        master.CustomData = master.CustomData.TrimEnd('\n') + "\n" + newLines.ToString().TrimEnd('\n');
    entries.Sort((a, z) => a[0].CompareTo(z[0]));
    craftRows.Clear();
    foreach (var e in entries)
    {
        string name = e[0]; string mods = e[2];
        double want; double.TryParse(e[1], out want);
        MyItemType t;
        double cur = byName.TryGetValue(name, out t) ? CountOf(t) : 0;
        if (mods.Contains("I")) { craftRows.Add(new CRow { name = name, cur = cur, want = want, state = -2 }); continue; }
        if (e[3] != "" && !bpCache.ContainsKey(name))
        {
            MyDefinitionId ov;
            if (TestBp(e[3], out ov)) { bpCache[name] = ov; noBp.Remove(name); }
        }
        MyDefinitionId bp;
        // Разрешаем русское имя в английский SubtypeId для поиска чертежа
        string lookupName = name;
        if (useRussian)
        {
            string en;
            if (ruToEnNames.TryGetValue(name, out en))
            {
                int slash = en.IndexOf('/');
                lookupName = slash >= 0 ? en.Substring(slash + 1) : en;
            }
        }
        int st = BpState(lookupName, out bp, want > 0);
        if (st == 0 && want <= 0) { craftRows.Add(new CRow { name = name, cur = cur, want = want, state = 0 }); continue; }
        if (st == 0) { craftRows.Add(new CRow { name = name, cur = cur, want = want, state = -3 }); continue; }
        if (st == -1) { craftRows.Add(new CRow { name = name, cur = cur, want = want, state = -1 }); continue; }
        double inQ; queued.TryGetValue(bp, out inQ);
        if (want > 0 && cur < want * (1 - craftMargin))
        {
            double need = want - cur - inQ;
            if (need >= 1)
            {
                QueueBp(bp, need, mods.Contains("P"));
                queued[bp] = inQ + need;
                inQ += need;
                double om; ourQueued.TryGetValue(bp, out om);
                ourQueued[bp] = om + need;
                Act(L("Queued ","В очередь: ") + Math.Round(need) + " " + name);
            }
        }
        else if (cur >= want && inQ > 0)
        {
            double removed = RemoveBp(bp);
            inQ = Math.Max(0, inQ - removed);
            queued[bp] = inQ;
        }
        craftRows.Add(new CRow { name = name, cur = cur, want = want, state = 0, inQ = inQ });
    }
    DrawCraftScreens();
    return true;
}

void SetupCraftCD()
{
    var master = craftLCDs[0];
    var sbcd = new StringBuilder();
    sbcd.Append(CRAFT_MARKER + "\n");
    sbcd.Append(L(
        "# Edit the number = wanted stock. The screen only displays status.\n" +
        "# Modifiers after the number:  P = craft first (priority),  I = ignore\n" +
        "# Modded item stuck on NoBP? Force its blueprint: Name=100 BP:BlueprintSubtype\n" +
        "# New craftable items get added here automatically.\n",
        "# Правь число = нужный запас. Экран только показывает статус.\n" +
        "# Модификаторы после числа:  P = крафтить первым (приоритет),  I = игнорировать\n" +
        "# Модовый предмет застрял на 'нет чертежа'? Задай чертёж вручную: Имя=100 BP:SubtypeЧертежа\n" +
        "# Новые крафтабельные предметы добавляются сюда автоматически.\n"));
    var have = new HashSet<string>();
    foreach (var p in craftLCDs)
    {
        foreach (var raw in p.GetText().Split('\n'))
        {
            var l = raw.Trim();
            if (l.Length == 0 || l.StartsWith("=") || l.StartsWith("#") || l.StartsWith("DUTC") || l.StartsWith("Item")) continue;
            var tok = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tok.Length < 2) continue;
            string wantTok = null;
            for (int i = tok.Length - 1; i >= 1; i--)
            {
                if (tok[i].Contains("[") || tok[i].Contains("]")) continue;
                if (System.Text.RegularExpressions.Regex.IsMatch(tok[i], @"\d")) { wantTok = tok[i]; break; }
            }
            if (wantTok == null) continue;
            double w;
            double.TryParse(System.Text.RegularExpressions.Regex.Match(wantTok, @"\d+").Value, out w);
            string mods = System.Text.RegularExpressions.Regex.Replace(wantTok, @"[\d\.]", "").ToUpper().Replace("A", "").Replace("D", "").Replace("H", "");
            if (have.Contains(tok[0])) continue;
            have.Add(tok[0]);
            sbcd.Append(tok[0] + "=" + Math.Round(w) + mods + "\n");
        }
    }
    foreach (var v in vanillaCraft)
    {
        if (have.Contains(v)) continue;
        MyDefinitionId bp;
        if (BpState(v, out bp, false) != 1) continue;
        string displayV = useRussian && ruNames.ContainsKey(v) ? ruNames[v] : v;
        if (have.Contains(displayV)) continue;
        have.Add(displayV);
        sbcd.Append(displayV + "=0\n");
    }
    master.CustomData = sbcd.ToString().TrimEnd('\n') + (master.CustomData.Trim().Length > 0 ? "\n" + master.CustomData : "");
    Act(L("Autocrafting list written to Custom Data of ","Список автокрафта записан в Custom Data ") + StripPct(master.CustomName));
}

void DrawCraftScreens()
{
    int start = 0;
    for (int p = 0; p < craftLCDs.Count; p++)
    {
        bool last = p == craftLCDs.Count - 1;
        start += DrawCraftPanel(craftLCDs[p], p, start, last);
    }
}

int DrawCraftPanel(IMyTextSurface s, int index, int start, bool last)
{
    var f = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f * UI_SCALE;
    string hd = L("DUTC AUTOCRAFTING","DUTC АВТОКРАФТ") + (craftLCDs.Count > 1 ? " " + (index + 1) + "/" + craftLCDs.Count : "");
    f.Add(Txt(hd, off + new Vector2(W * 0.5f, 10f * sc), 1.05f * sc, UI_DIM));
    int okC = 0, craftC = 0, lowC = 0, nobpC = 0, chkC = 0;
    foreach (var r in craftRows)
    {
        if (r.state == -1) nobpC++;
        else if (r.state == -3) chkC++;
        else if (r.state == 0 && r.want > 0)
        {
            if (r.cur >= r.want * (1 - craftMargin)) okC++;
            else if (r.inQ > 0) craftC++;
            else lowC++;
        }
    }
    string counts = okC + L(" ok   "," ок   ") + craftC + L(" crafting   "," крафтится   ") + lowC + L(" low   "," мало   ") + nobpC + L(" noBP"," нет чертежа");
    if (chkC > 0) counts += "   " + chkC + L(" checking"," проверка");
    counts += L("   |   edit Custom Data","   |   правь Custom Data");
    f.Add(Txt(counts, off + new Vector2(W * 0.5f, 38f * sc), 0.55f * sc, UI_DIM));
    f.Add(Box(off + new Vector2(W * 0.5f, 64f * sc), new Vector2(W * 0.9f, 2f * sc), UI_FRAME));
    float rowPx = 26f * sc;
    float yStart = 77f * sc;
    int maxRows = Math.Max(1, (int)((H - 18f * sc - yStart) / rowPx));
    int total = craftRows.Count;
    int offR = start;
    if (last)
    {
        int remaining = total - start;
        if (remaining > maxRows)
        {
            int key = s.GetHashCode();
            int so; scroll.TryGetValue(key, out so);
            int maxOff = remaining - maxRows;
            if (so > maxOff || so < 0) so = 0;
            scroll[key] = so + 2 > maxOff ? (so >= maxOff ? 0 : maxOff) : so + 2;
            offR = start + so;
            string ind = (so > 0 ? "^" + so + " " : "") + (maxOff - so > 0 ? "v" + (maxOff - so) : "");
            if (ind != "") f.Add(Txt(ind, off + new Vector2(W * 0.05f, 10f * sc), 0.55f * sc, UI_DIM, TextAlignment.LEFT));
        }
    }
    int maxChars = (int)(W * 0.42f / (10.5f * sc));
    float y = yStart;
    int drawn = 0;
    for (int i = offR; i < total && drawn < maxRows; i++)
    {
        var r = craftRows[i];
        Color col; string valTxt; double frac = -1;
        if (r.state == -2) { col = UI_DIM; valTxt = L("ignored","игнор"); }
        else if (r.state == -3) { col = UI_DIM; valTxt = Num(r.cur) + L("  checking..","  проверка.."); }
        else if (r.state == -1) { col = UI_BAD; valTxt = Num(r.cur) + L("  NoBP","  нет чертежа"); }
        else
        {
            frac = r.want > 0 ? Math.Min(1, r.cur / r.want) : -1;
            col = r.want <= 0 ? UI_DIM : r.cur >= r.want * (1 - craftMargin) ? UI_GOOD : r.inQ > 0 ? UI_WARNC : UI_BAD;
            valTxt = Num(r.cur) + " / " + Num(r.want) + (r.inQ > 0 ? " +" + Math.Round(r.inQ) : "");
        }
        f.Add(Txt(TruncS(r.name, maxChars), off + new Vector2(W * 0.05f, y), 0.6f * sc, UI_TEXT, TextAlignment.LEFT));
        f.Add(Txt(valTxt, off + new Vector2(W * 0.70f, y), 0.55f * sc, col, TextAlignment.RIGHT));
        if (frac >= 0) DrawBar(f, off + new Vector2(W * 0.845f, y + 8f * sc), W * 0.25f, 10f * sc, frac, col);
        y += rowPx; drawn++;
    }
    if (total == 0) f.Add(Txt(L("No craftable items known yet","Нет доступных чертежей"), off + new Vector2(W * 0.5f, H * 0.45f), 0.7f * sc, UI_DIM));
    f.Dispose();
    return last ? Math.Max(0, total - start) : maxRows;
}

void QueueBp(MyDefinitionId bp, double need, bool prio)
{
    var usable = new List<IMyAssembler>();
    foreach (var a in assemblers)
    {
        try
        {
            if (!a.CanUseBlueprint(bp)) continue;
            if (a.Mode == MyAssemblerMode.Disassembly)
            {
                if (a.IsQueueEmpty) a.Mode = MyAssemblerMode.Assembly; else continue;
            }
            usable.Add(a);
        }
        catch { }
    }
    if (usable.Count == 0) { Warn(L("No assembler can craft ","Ни один сборщик не умеет делать ") + bp.SubtypeName); return; }
    double chunk = Math.Ceiling(need / usable.Count);
    foreach (var a in usable)
    {
        double amt = Math.Min(chunk, need);
        if (amt < 1) break;
        try
        {
            if (prio) a.InsertQueueItem(0, bp, (VRage.MyFixedPoint)amt);
            else a.AddQueueItem(bp, (VRage.MyFixedPoint)amt);
        }
        catch (Exception ex)
        {
            Warn(L("Assembler '","Сборщик '") + a.CustomName + L("' failed to queue ","' не принял в очередь ") + bp.SubtypeName + ": " + ex.Message);
        }
        need -= amt;
    }
}

double RemoveBp(MyDefinitionId bp)
{
    double allow; ourQueued.TryGetValue(bp, out allow);
    if (allow <= 0) return 0;
    double removed = 0;
    var q = new List<MyProductionItem>();
    foreach (var a in assemblers)
    {
        if (allow <= 0) break;
        q.Clear();
        try { a.GetQueue(q); } catch { continue; }
        for (int i = q.Count - 1; i >= 0 && allow > 0; i--)
        {
            if (q[i].BlueprintId != bp) continue;
            double take = Math.Min(allow, (double)q[i].Amount);
            try { a.RemoveQueueItem(i, (VRage.MyFixedPoint)take); } catch { continue; }
            allow -= take; removed += take;
        }
    }
    ourQueued[bp] = 0;
    return removed;
}

int BpState(string name, out MyDefinitionId bp, bool deep)
{
    bp = new MyDefinitionId();
    MyDefinitionId cached;
    if (bpCache.TryGetValue(name, out cached)) { bp = cached; return 1; }
    if (noBp.Contains(name)) return -1;
    if (assemblers.Count == 0) return 0;
    string mapped;
    if (knownBp.TryGetValue(name, out mapped) && TestBp(mapped, out bp)) { bpCache[name] = bp; return 1; }
    string core = name;
    string typePrefix = "";
    int slash = name.IndexOf('/');
    if (slash > 0) { typePrefix = name.Substring(0, slash); core = name.Substring(slash + 1); }
    bool isSeed = typePrefix == "SeedItem" || typePrefix == "Seed" || typePrefix == "Seeds";
    string baseN = core.Replace("Item", "");
    int lastUnder = baseN.LastIndexOf('_');
    string trimmed = lastUnder > 0 ? baseN.Substring(0, lastUnder) : baseN;
    var candidates = new HashSet<string>();
    if (isSeed)
    {
        candidates.Add("Seeds_" + core); candidates.Add("Seed_" + core); candidates.Add("Spores_" + core);
        candidates.Add("Seeds_" + baseN); candidates.Add("Spores_" + baseN);
        candidates.Add(core + "_Seed"); candidates.Add(core + "_Seeds");
    }
    if (typePrefix != "") candidates.Add(typePrefix + "_" + core);
    candidates.Add(core); candidates.Add(baseN); candidates.Add(trimmed);
    candidates.Add(core + "Component"); candidates.Add(baseN + "Component");
    candidates.Add(core + "Magazine"); candidates.Add(baseN + "Magazine");
    candidates.Add(core + "Item"); candidates.Add(baseN + "Item");
    candidates.Add(core + "_ApexSurvivalAdditions"); candidates.Add(baseN + "_ApexSurvivalAdditions");
    foreach (var s in candidates)
        if (TestBp(s, out bp)) { bpCache[name] = bp; return 1; }
        if (!deep) return 0;
        if (probeBudget <= 0 || Runtime.CurrentInstructionCount > 28000) return 0;
        probeBudget--;
    var candList = new List<string>(candidates);
    int pos; probePos.TryGetValue(name, out pos);
    int idx = 0;
    for (int ci = 0; ci < candList.Count; ci++)
    {
        for (int i = 1; i <= 35; i++)
        {
            string p2 = i.ToString("00");
            for (int v = 0; v < 3; v++)
            {
                if (idx++ < pos) continue;
                string sub = v == 0 ? p2 + candList[ci] : v == 1 ? p2 + "_" + candList[ci] : "Position" + p2 + "_" + candList[ci];
                if (TestBp(sub, out bp)) { probePos.Remove(name); bpCache[name] = bp; return 1; }
                if (Runtime.CurrentInstructionCount > 20000) { probePos[name] = idx; return 0; }
            }
        }
    }
    for (int n = 1; n <= 400; n = n < 30 ? n + 1 : n + 5)
    {
        string pre = "Position" + n.ToString("0000") + "_";
        for (int ci = 0; ci < candList.Count; ci++)
        {
            if (idx++ < pos) continue;
            if (TestBp(pre + candList[ci], out bp)) { probePos.Remove(name); bpCache[name] = bp; return 1; }
            if (Runtime.CurrentInstructionCount > 22000) { probePos[name] = idx; return 0; }
        }
    }
    probePos.Remove(name);
    noBp.Add(name);
    return -1;
}

bool TestBp(string sub, out MyDefinitionId id)
{
    id = new MyDefinitionId();
    for (int attempt = 0; attempt < 2; attempt++)
    {
        try
        {
            MyDefinitionId t;
            if (!MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + sub, out t)) return false;
            foreach (var a in distinctAsm)
                if (a.CanUseBlueprint(t)) { id = t; return true; }
                return false;
        }
        catch { }
    }
    return false;
}bool Refine()
{
    if (refineries.Count == 0) return true;
    foreach (var r in refineries)
    {
        var inv = r.GetInventory(0);
        if (feedRefineries && (double)inv.CurrentVolume < (double)inv.MaxVolume * 0.4)
        {
            double freeL = ((double)inv.MaxVolume - (double)inv.CurrentVolume) * 1000.0;
            foreach (var ore in orePriority)
            {
                var t = MyItemType.MakeOre(ore);
                if (CountOf(t) <= 0) continue;
                if (!inv.CanItemsBeAdded(1, t)) continue;
                if (PullTo(inv, t, freeL / 0.37, r) > 0) break;
            }
        }
        if (sortRefineryQueue)
        {
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            if (items.Count > 1)
            {
                int best = int.MaxValue, bi = -1;
                for (int i = 0; i < items.Count; i++)
                {
                    int pr = orePriority.IndexOf(items[i].Type.SubtypeId);
                    if (pr < 0) pr = 999;
                    if (pr < best) { best = pr; bi = i; }
                }
                if (bi > 0) inv.TransferItemTo(inv, bi, 0, true);
            }
        }
    }
    if (balanceRefineries && refineries.Count > 1)
    {
        foreach (var ore in orePriority)
        {
            var t = MyItemType.MakeOre(ore);
            double tot = 0;
            foreach (var rf in refineries) tot += (double)rf.GetInventory(0).GetItemAmount(t);
            if (tot < 200) continue;
            double avg = tot / refineries.Count;
            foreach (var rich in refineries)
            {
                double cur = (double)rich.GetInventory(0).GetItemAmount(t);
                if (cur <= avg + 100) continue;
                foreach (var poor in refineries)
                {
                    if (poor == rich) continue;
                    double pc = (double)poor.GetInventory(0).GetItemAmount(t);
                    if (pc >= avg - 100) continue;
                    double move = Math.Min(cur - avg, avg - pc);
                    cur -= PushFrom(rich.GetInventory(0), t, move, poor.GetInventory(0));
                    if (cur <= avg + 100) break;
                }
            }
            if (Runtime.CurrentInstructionCount > 30000) break;
        }
    }
    return true;
}

bool Turrets()
{
    if (!fillTurrets || guns.Count == 0) { gunIdx = 0; return true; }
    var ammoTypes = new List<MyItemType>();
    foreach (var kv in stock)
        if (kv.Key.TypeId.EndsWith("_AmmoMagazine")) ammoTypes.Add(kv.Key);
        if (ammoTypes.Count == 0) { gunIdx = 0; return true; }
        for (; gunIdx < guns.Count; gunIdx++)
        {
            if (Runtime.CurrentInstructionCount > 25000) return false;
            var g = guns[gunIdx];
            if (g.Closed) continue;
            var inv = g.GetInventory(0);
            foreach (var t in ammoTypes)
            {
                if (Runtime.CurrentInstructionCount > 30000) return false;
                if (!inv.CanItemsBeAdded(1, t)) continue;
                double cur = (double)inv.GetItemAmount(t);
                if (cur >= magsPerTurret - 0.01) continue;
                PullTo(inv, t, magsPerTurret - cur, g);
            }
        }
        gunIdx = 0;
        return true;
}

bool Ice()
{
    if (!iceBalancing || gens.Count == 0) return true;
    var iceTypes = new List<MyItemType>();
    foreach (var kv in stock) if (IsIce(kv.Key)) iceTypes.Add(kv.Key);
    if (iceTypes.Count == 0) return true;
    foreach (var g in gens) g.UseConveyorSystem = false;
    foreach (var ice in iceTypes)
    {
        double share = CountOf(ice) / gens.Count;
        foreach (var g in gens)
        {
            var inv = g.GetInventory(0);
            double cur = (double)inv.GetItemAmount(ice);
            double freeL = ((double)inv.MaxVolume - (double)inv.CurrentVolume) * 1000.0;
            double maxHold = cur + Math.Max(0, freeL - iceBottleReserve) / 0.37;
            double target = Math.Min(share, maxHold);
            if (cur > target + 100)
            {
                var w = DestFor(ORE, g);
                if (w != null) PushFrom(inv, ice, cur - target, w.GetInventory(0));
            }
            else if (cur < target - 100)
            {
                PullTo(inv, ice, target - cur, g);
            }
            if (Runtime.CurrentInstructionCount > 30000) return true;
        }
    }
    return true;
}

bool Uranium()
{
    if (!uraniumBalancing || reactors.Count == 0) return true;
    var ur = MyItemType.MakeIngot("Uranium");
    foreach (var r in reactors)
    {
        r.UseConveyorSystem = false;
        var inv = r.GetInventory(0);
        double target = r.CubeGrid.GridSize > 0.6f ? uraniumLarge : uraniumSmall;
        double cur = (double)inv.GetItemAmount(ur);
        if (cur > target + 1)
        {
            var w = DestFor(INGOT, r);
            if (w != null) PushFrom(inv, ur, cur - target, w.GetInventory(0));
        }
        else if (cur < target - 1)
        {
            PullTo(inv, ur, target - cur, r);
        }
    }
    return true;
}

string[] ingotNames = { "Iron","Nickel","Cobalt","Silicon","Silver","Gold","Platinum","Magnesium","Stone" };
Dictionary<string,double[]> recipes = new Dictionary<string,double[]>();
void InitRecipes()
{
    recipes["SteelPlate"] = new double[]{21,0,0,0,0,0,0,0,0};
    recipes["InteriorPlate"] = new double[]{3.5,0,0,0,0,0,0,0,0};
    recipes["ConstructionComponent"] = new double[]{8,0,0,0,0,0,0,0,0};
    recipes["MetalGrid"] = new double[]{12,5,3,0,0,0,0,0,0};
    recipes["SmallTube"] = new double[]{5,0,0,0,0,0,0,0,0};
    recipes["LargeTube"] = new double[]{30,0,0,0,0,0,0,0,0};
    recipes["MotorComponent"] = new double[]{20,5,0,0,0,0,0,0,0};
    recipes["ComputerComponent"] = new double[]{0.5,0,0,0.2,0,0,0,0,0};
    recipes["Display"] = new double[]{1,0,0,5,0,0,0,0,0};
    recipes["BulletproofGlass"] = new double[]{0,0,0,15,0,0,0,0,0};
    recipes["GirderComponent"] = new double[]{6,0,0,0,0,0,0,0,0};
    recipes["PowerCell"] = new double[]{10,2,0,1,0,0,0,0,0};
    recipes["SolarCell"] = new double[]{0,3,0,6,0,0,0,0,0};
    recipes["RadioCommunicationComponent"] = new double[]{8,0,0,1,0,0,0,0,0};
    recipes["DetectorComponent"] = new double[]{5,15,0,0,0,0,0,0,0};
    recipes["MedicalComponent"] = new double[]{60,70,0,0,20,0,0,0,0};
    recipes["ReactorComponent"] = new double[]{15,0,0,0,5,0,0,0,20};
    recipes["ThrustComponent"] = new double[]{30,0,10,0,0,1,0.4,0,0};
    recipes["GravityGeneratorComponent"] = new double[]{600,0,220,0,5,10,0,0,0};
    recipes["Superconductor"] = new double[]{10,0,0,0,0,2,0,0,0};
    recipes["ExplosivesComponent"] = new double[]{0,0,0,0.5,0,0,0,2,0};
    recipes["NATO_25x184mmMagazine"] = new double[]{40,2,0,0,0,0,0,3,0};
}

void FeedAssemblers()
{
    if (!feedAssemblers) return;
    var q = new List<MyProductionItem>();
    foreach (var a in assemblers)
    {
        if (a.Mode == MyAssemblerMode.Disassembly || a.IsQueueEmpty) continue;
        a.UseConveyorSystem = true;
        var inv = a.GetInventory(0);
        if (!HasSpace(inv)) continue;
        if (Runtime.CurrentInstructionCount > 28000) return;
        var need = new Dictionary<string,double>();
        q.Clear();
        try { a.GetQueue(q); } catch { continue; }
        double depth = feedQueueDepth;
        foreach (var pi in q)
        {
            if (depth <= 0) break;
            double amt = Math.Min((double)pi.Amount, depth);
            depth -= amt;
            double[] costs;
            if (!recipes.TryGetValue(pi.BlueprintId.SubtypeName, out costs)) continue;
            for (int i = 0; i < costs.Length; i++)
            {
                if (costs[i] <= 0) continue;
                double v; need.TryGetValue(ingotNames[i], out v);
                need[ingotNames[i]] = v + costs[i] * amt;
            }
        }
        foreach (var kv in need)
        {
            if (Runtime.CurrentInstructionCount > 28000) return;
            var t = MyItemType.MakeIngot(kv.Key);
            double have = (double)inv.GetItemAmount(t);
            if (have >= kv.Value - 0.5) continue;
            PullTo(inv, t, kv.Value - have, a);
        }
    }
}

bool Cleanup()
{
    FeedAssemblers();
    if (assemblerCleanup)
    {
        foreach (var a in assemblers)
        {
            if (!a.IsQueueEmpty) continue;
            var inv = a.GetInventory(0);
            if (inv.ItemCount == 0) continue;
            var w = DestFor(INGOT, a);
            if (w == null) continue;
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            for (int i = items.Count - 1; i >= 0; i--) inv.TransferItemTo(w.GetInventory(0), i, null, true);
            Act(L("Cleaned up ","Очищен ") + StripPct(a.CustomName));
        }
    }
    if (fillBottles && refillStoredBottles && cycles % bottleRefillEvery == 0) BottleTopUp();
    return true;
}

void BottleTopUp()
{
    foreach (var w in cats[BOTTLE])
    {
        var inv = w.GetInventory(0);
        var items = new List<MyInventoryItem>();
        inv.GetItems(items);
        double count = 0;
        foreach (var it in items) if (Cat(it.Type) == BOTTLE && Tankable(it.Type)) count += (double)it.Amount;
        double last; bottleMem.TryGetValue(w.EntityId, out last);
        if (count <= last + 0.01) { bottleMem[w.EntityId] = count; continue; }
        bottleMem[w.EntityId] = count;
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (Cat(items[i].Type) != BOTTLE) continue;
            if (!Tankable(items[i].Type)) continue;
            var tk = FindTank(items[i].Type);
            if (tk == null) continue;
            if (!CanMove(inv, w.EntityId, tk.GetInventory(0), tk.EntityId, items[i].Type)) continue;
            inv.TransferItemTo(tk.GetInventory(0), i, null, true);
        }
    }
}
Color UI_BG = new Color(8, 10, 16);
Color UI_FRAME = new Color(95, 105, 125);
Color UI_TEXT = new Color(210, 220, 235);
Color UI_DIM = new Color(110, 120, 140);
Color UI_GOOD = new Color(80, 220, 120);
Color UI_WARNC = new Color(255, 190, 60);
Color UI_BAD = new Color(255, 80, 70);
Color[] catColors = {
    new Color(200, 140, 80),   // Руда
    new Color(170, 190, 210),  // Слитки
    new Color(100, 160, 255),  // Компоненты
    new Color(255, 190, 60),   // Инструменты
    new Color(255, 110, 90),   // Боеприпасы
    new Color(80, 180, 255),   // Баллоны
    new Color(150, 210, 110)   // Еда
};
class UiRow
{
    public int kind;          // 0 = centered header, 1 = item row with bar, 2 = plain text
    public string name = "";
    public string val = "";
    public double frac = -1;  // <0 = no bar
    public Color col;
}
bool Screens()
{
    foreach (var b in mainHolders) { var s = SurfaceFor(b, MAIN_TAGS); if (s != null) DrawMain(s); }
    foreach (var b in warnHolders) { var s = SurfaceFor(b, WARN_TAGS); if (s != null) DrawList(s, L("WARNINGS","ПРЕДУПРЕЖДЕНИЯ"), WarnRows(), true); }
    foreach (var b in actHolders) { var s = SurfaceFor(b, ACT_TAGS); if (s != null) DrawList(s, L("ACTIONS","ДЕЙСТВИЯ"), ActRows(), true); }
    foreach (var b in invHolders) WriteInv(b);
    return true;
}
void DrawMain(IMyTextSurface s)
{
    var f = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f * UI_SCALE;
    f.Add(Txt(L("DUTC INVENTORY","DUTC ИНВЕНТАРЬ"), off + new Vector2(W * 0.5f, 10f * sc), 1.15f * sc, UI_DIM));
    f.Add(Txt(("" + "|/-\\"[tick % 4]), off + new Vector2(W * 0.95f, 10f * sc), 1.0f * sc, UI_DIM, TextAlignment.RIGHT));
    float y = 64f * sc;
    for (int c = 0; c < NCAT; c++)
    {
        double cur = 0, max = 0;
        foreach (var w in cats[c])
        {
            var inv = w.GetInventory(0);
            cur += (double)inv.CurrentVolume; max += (double)inv.MaxVolume;
        }
        double fr = max > 0 ? cur / max : 0;
        Color col = fr > 0.9 ? UI_BAD : catColors[c];
        f.Add(Txt(catKeys[c] + " x" + cats[c].Count, off + new Vector2(W * 0.05f, y), 0.7f * sc, UI_TEXT, TextAlignment.LEFT));
        DrawBar(f, off + new Vector2(W * 0.60f, y + 10f * sc), W * 0.34f, 13f * sc, fr, col);
        f.Add(Txt(Pct(cur, max), off + new Vector2(W * 0.96f, y + 1f * sc), 0.6f * sc, col, TextAlignment.RIGHT));
        y += 31f * sc;
    }
    f.Add(Box(off + new Vector2(W * 0.5f, y + 2f * sc), new Vector2(W * 0.9f, 2f * sc), UI_FRAME));
    y += 14f * sc;
    f.Add(Txt(L("Refineries ","Очистители ") + refineries.Count, off + new Vector2(W * 0.05f, y), 0.62f * sc, UI_DIM, TextAlignment.LEFT));
    f.Add(Txt(L("Assemblers ","Сборщики ") + assemblers.Count, off + new Vector2(W * 0.95f, y), 0.62f * sc, UI_DIM, TextAlignment.RIGHT));
    y += 24f * sc;
    f.Add(Txt(L("O2/H2 Gens ","Генераторы O2/H2 ") + gens.Count, off + new Vector2(W * 0.05f, y), 0.62f * sc, UI_DIM, TextAlignment.LEFT));
    f.Add(Txt(L("Reactors ","Реакторы ") + reactors.Count, off + new Vector2(W * 0.95f, y), 0.62f * sc, UI_DIM, TextAlignment.RIGHT));
    y += 24f * sc;
    f.Add(Txt(L("Tanks ","Баки ") + tanks.Count + L("   Specials ","   Спецконтейнеры ") + specials.Count, off + new Vector2(W * 0.05f, y), 0.62f * sc, UI_DIM, TextAlignment.LEFT));
    f.Add(Txt(L("Turrets ","Турели ") + guns.Count, off + new Vector2(W * 0.95f, y), 0.62f * sc, UI_DIM, TextAlignment.RIGHT));
    y += 28f * sc;
    double qTot = 0; foreach (var kv in queued) qTot += kv.Value;
    f.Add(Txt(L("QUEUE ","ОЧЕРЕДЬ ") + Math.Round(qTot), off + new Vector2(W * 0.05f, y), 0.7f * sc, qTot > 0 ? UI_WARNC : UI_DIM, TextAlignment.LEFT));
    f.Add(Txt(L("WARNINGS ","ПРЕДУПРЕЖДЕНИЯ ") + warnings.Count, off + new Vector2(W * 0.95f, y), 0.7f * sc, warnings.Count > 0 ? UI_BAD : UI_GOOD, TextAlignment.RIGHT));
    y += 27f * sc;
    f.Add(Box(off + new Vector2(W * 0.5f, y), new Vector2(W * 0.9f, 2f * sc), UI_FRAME));
    y += 10f * sc;
    int maxChars = (int)(W * 0.9f / (10.5f * sc));
    foreach (var a in actions)
    {
        if (y > H - 24f * sc) break;
        f.Add(Txt(TruncS(a, maxChars), off + new Vector2(W * 0.05f, y), 0.52f * sc, UI_DIM, TextAlignment.LEFT));
        y += 21f * sc;
    }
    if (actions.Count == 0 && y <= H - 24f * sc)
        f.Add(Txt(L("- no actions yet -","- действий пока нет -"), off + new Vector2(W * 0.05f, y), 0.52f * sc, UI_DIM, TextAlignment.LEFT));
    f.Dispose();
}
void DrawList(IMyTextSurface s, string title, List<UiRow> rows, bool scrollOn)
{
    var f = Begin(s);
    Vector2 size = s.SurfaceSize, off = (s.TextureSize - size) * 0.5f;
    float W = size.X, H = size.Y, sc = Math.Min(W, H) / 512f * UI_SCALE;
    f.Add(Txt("DUTC " + title, off + new Vector2(W * 0.5f, 10f * sc), 1.05f * sc, UI_DIM));
    f.Add(Box(off + new Vector2(W * 0.5f, 46f * sc), new Vector2(W * 0.9f, 2f * sc), UI_FRAME));
    float rowPx = 26f * sc;
    float yStart = 59f * sc;
    int maxRows = Math.Max(1, (int)((H - 18f * sc - yStart) / rowPx));
    int total = rows.Count;
    int offR = 0;
    if (scrollOn && total > maxRows)
    {
        int key = s.GetHashCode();
        scroll.TryGetValue(key, out offR);
        int maxOff = total - maxRows;
        if (offR > maxOff || offR < 0) offR = 0;
        scroll[key] = offR + 2 > maxOff ? (offR >= maxOff ? 0 : maxOff) : offR + 2;
    }
    int maxChars = (int)(W * 0.55f / (10.5f * sc));
    int maxCharsWide = (int)(W * 0.9f / (10.5f * sc));
    float y = yStart;
    for (int i = offR; i < total && i < offR + maxRows; i++)
    {
        var r = rows[i];
        if (r.kind == 0)
        {
            f.Add(Txt(r.name, off + new Vector2(W * 0.5f, y), 0.72f * sc, r.col));
        }
        else if (r.kind == 1)
        {
            f.Add(Txt(TruncS(r.name, maxChars), off + new Vector2(W * 0.05f, y), 0.6f * sc, UI_TEXT, TextAlignment.LEFT));
            f.Add(Txt(r.val, off + new Vector2(W * 0.66f, y), 0.6f * sc, r.col, TextAlignment.RIGHT));
            if (r.frac >= 0)
                DrawBar(f, off + new Vector2(W * 0.825f, y + 8f * sc), W * 0.25f, 10f * sc, r.frac, r.col);
        }
        else
        {
            f.Add(Txt(TruncS(r.name, maxCharsWide), off + new Vector2(W * 0.05f, y), 0.55f * sc, r.col, TextAlignment.LEFT));
        }
        y += rowPx;
    }
    if (total > maxRows)
    {
        string ind = (offR > 0 ? "^" + offR + " " : "") + (total - offR - maxRows > 0 ? "v" + (total - offR - maxRows) : "");
        if (ind != "") f.Add(Txt(ind, off + new Vector2(W * 0.05f, 10f * sc), 0.55f * sc, UI_DIM, TextAlignment.LEFT));
    }
    if (total == 0) f.Add(Txt(L("- nothing to show -","- нечего показать -"), off + new Vector2(W * 0.5f, H * 0.45f), 0.8f * sc, UI_DIM));
    f.Dispose();
}
List<UiRow> WarnRows()
{
    var r = new List<UiRow>();
    for (int i = 0; i < warnings.Count; i++)
        r.Add(new UiRow { kind = 2, name = (i + 1) + ". " + warnings[i].Replace("\n", " "), col = UI_TEXT });
    if (warnings.Count == 0) r.Add(new UiRow { kind = 2, name = L("No problems detected","Проблем не обнаружено"), col = UI_GOOD });
    return r;
}
List<UiRow> ActRows()
{
    var r = new List<UiRow>();
    foreach (var a in actions) r.Add(new UiRow { kind = 2, name = a, col = UI_DIM });
    if (actions.Count == 0) r.Add(new UiRow { kind = 2, name = L("Nothing moved yet","Пока ничего не перемещалось"), col = UI_DIM });
    return r;
}
void WriteInv(IMyTerminalBlock b)
{
    var prov = b as IMyTextSurfaceProvider;
    var secIdx = new List<int>();
    var secLines = new List<List<string>>();
    int cur = -1;
    foreach (var raw in b.CustomData.Split('\n'))
    {
        var l = raw.Trim();
        if (l.StartsWith("@"))
        {
            cur = -1;
            var m = System.Text.RegularExpressions.Regex.Match(l, @"^@(\d+)");
            if (m.Success && ContainsAny(l, INV_TAGS))
            {
                int ix; int.TryParse(m.Groups[1].Value, out ix);
                secIdx.Add(ix); secLines.Add(new List<string>()); cur = secIdx.Count - 1;
            }
            continue;
        }
        if (cur >= 0) secLines[cur].Add(raw);
    }
    if (secIdx.Count == 0)
    {
        var s = SurfaceFor(b, INV_TAGS);
        if (s == null) return;
        if (b.CustomData.Trim().Length == 0)
            b.CustomData = useRussian
            ? "# Экран DUTC-inv\n# Один фильтр на строку (тип, имя предмета или регулярка):\n#   Ore   Ingot   Component   AmmoMagazine\n#   SteelPlate   Iron   NATO\n# Опции после фильтра:\n#   <число> = максимум шкалы   noBar   hideEmpty   hideType   noHeading   noScroll\nComponent\n"
            : "# DUTC-inv screen\n# One filter per line (type, item name or regex):\n#   Ore   Ingot   Component   AmmoMagazine\n#   SteelPlate   Iron   NATO\n# Options after the filter:\n#   <number> = bar max   noBar   hideEmpty   hideType   noHeading   noScroll\nComponent\n";
        RenderInv(s, b.CustomData.Split('\n'));
        return;
    }
    for (int i = 0; i < secIdx.Count; i++)
    {
        IMyTextSurface s = null;
        if (prov != null && secIdx[i] < prov.SurfaceCount) s = prov.GetSurface(secIdx[i]);
        else s = b as IMyTextSurface;
        if (s == null) continue;
        RenderInv(s, secLines[i].ToArray());
    }
}
void RenderInv(IMyTextSurface s, string[] lines)
{
    bool noScroll = false;
    var rows = new List<UiRow>();
    foreach (var raw in lines)
    {
        var l = raw.Trim();
        if (l.Length == 0 || l.StartsWith("#")) continue;
        string low = l.ToLower();
        if (low.Contains("noscroll")) noScroll = true;
        if (low.StartsWith("echoc")) { rows.Add(new UiRow { kind = 0, name = l.Substring(5).Trim(), col = UI_DIM }); continue; }
        if (low.StartsWith("echor")) { rows.Add(new UiRow { kind = 2, name = l.Substring(5).Trim(), col = UI_TEXT }); continue; }
        if (low.StartsWith("echo")) { rows.Add(new UiRow { kind = 2, name = l.Substring(4).Trim(), col = UI_TEXT }); continue; }
        if (low.Contains("=") && !low.Contains(" ")) continue;
        var tok = l.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string filt = tok[0].ToLower();
        double barMax = 0; bool noBar = false, hideEmpty = false, hideType = false, noHeading = false;
        for (int i = 1; i < tok.Length; i++)
        {
            string o = tok[i].ToLower();
            if (o == "nobar") noBar = true;
            else if (o == "hideempty") hideEmpty = true;
            else if (o == "hidetype") hideType = true;
            else if (o == "noheading") noHeading = true;
            else if (o == "noscroll") noScroll = true;
            else if (o == "singleline") { }
            else double.TryParse(o, out barMax);
        }
        // Русский фильтр -> английский эквивалент для Regex.IsMatch по TypeId/SubtypeId
        string filtEn = filt;
        if (useRussian)
        {
            string en;
            if (ruToEnNames.TryGetValue(filt, out en)) filtEn = en.ToLower();
        }
        var matches = new List<KeyValuePair<MyItemType, double>>();
        foreach (var kv in stock) if (MatchItem(kv.Key, filtEn) || MatchItem(kv.Key, filt)) matches.Add(kv);
        matches.Sort((a, z) => z.Value.CompareTo(a.Value));
        if (!noHeading) rows.Add(new UiRow { kind = 0, name = FilterLabel(tok[0]), col = UI_DIM });
        if (matches.Count == 0) { rows.Add(new UiRow { kind = 2, name = L("- nothing found -","- ничего не найдено -"), col = UI_DIM }); continue; }
        double relMax = barMax;
        if (relMax <= 0) foreach (var kv in matches) if (kv.Value > relMax) relMax = kv.Value;
        foreach (var kv in matches)
        {
            if (hideEmpty && kv.Value < 1) continue;
            string nm = ItemName(kv.Key);
            if (kv.Key.ToString().EndsWith("Ingot/Stone")) nm = useRussian ? "Гравий" : "Gravel";
            if (!hideType) nm += " (" + kv.Key.TypeId.Replace("MyObjectBuilder_", "").Substring(0, 2) + ")";
            int cc = Cat(kv.Key);
            rows.Add(new UiRow
            {
                kind = 1,
                name = nm,
                val = Num(kv.Value),
                     frac = noBar ? -1 : (relMax > 0 ? kv.Value / relMax : 0),
                     col = cc >= 0 ? catColors[cc] : UI_TEXT
            });
        }
    }
    if (rows.Count == 0)
    {
        rows.Add(new UiRow { kind = 0, name = L("NO FILTERS SET","ФИЛЬТРЫ НЕ ЗАДАНЫ"), col = UI_WARNC });
        rows.Add(new UiRow { kind = 2, name = L("Open this LCD's Custom Data (K)","Открой Custom Data этого LCD (K)"), col = UI_TEXT });
        rows.Add(new UiRow { kind = 2, name = L("and add one filter per line:","и добавь по одному фильтру в строке:"), col = UI_TEXT });
        rows.Add(new UiRow { kind = 2, name = "  " + (useRussian ? "Компоненты" : "Component"), col = UI_DIM });
        rows.Add(new UiRow { kind = 2, name = "  " + (useRussian ? "Слитки" : "Ingot"), col = UI_DIM });
        rows.Add(new UiRow { kind = 2, name = "  " + (useRussian ? "Руда hideEmpty" : "Ore hideEmpty"), col = UI_DIM });
        rows.Add(new UiRow { kind = 2, name = "  " + (useRussian ? "Стальная пластина 5000" : "SteelPlate 5000"), col = UI_DIM });
        rows.Add(new UiRow { kind = 2, name = L("Or clear Custom Data completely","Или полностью очисти Custom Data"), col = UI_TEXT });
        rows.Add(new UiRow { kind = 2, name = L("for an automatic template.","для автошаблона."), col = UI_TEXT });
    }
    DrawList(s, L("ITEM STOCK","СКЛАД"), rows, !noScroll);
}
// Отображаемое имя фильтра: для русского — ищем перевод "Тип" или "Тип/Подтип"
string FilterLabel(string filt)
{
    if (useRussian)
    {
        string r;
        if (ruNames.TryGetValue(filt, out r)) return r;
        string en;
        if (ruToEnNames.TryGetValue(filt, out en))
        {
            if (ruNames.TryGetValue(en, out r)) return r;
        }
    }
    return filt.ToUpper();
}
bool MatchItem(MyItemType t, string filt)
{
    string s = (t.TypeId + "/" + t.SubtypeId).ToLower();
    try { return System.Text.RegularExpressions.Regex.IsMatch(s, filt); }
    catch { return s.Contains(filt); }
}
IMyTextSurface SurfaceFor(IMyTerminalBlock b, string[] tags)
{
    int idx = 0; bool found = false;
    foreach (var tag in tags)
    {
        int p = b.CustomName.IndexOf(tag + ":");
        if (p >= 0)
        {
            int.TryParse(System.Text.RegularExpressions.Regex.Match(b.CustomName.Substring(p + tag.Length + 1), @"^\d+").Value, out idx);
            found = true; break;
        }
        var m = System.Text.RegularExpressions.Regex.Match(b.CustomData, @"@(\d+)[^\n]*" + tag);
        if (m.Success) { int.TryParse(m.Groups[1].Value, out idx); found = true; break; }
        if (b.CustomName.Contains(tag)) { found = true; break; }
    }
    if (!found) return null;
    var prov = b as IMyTextSurfaceProvider;
    if (prov != null && idx < prov.SurfaceCount) return prov.GetSurface(idx);
    return b as IMyTextSurface;
}
MySpriteDrawFrame Begin(IMyTextSurface s)
{
    s.ContentType = ContentType.SCRIPT;
    s.Script = "";
    var f = s.DrawFrame();
    f.Add(Box((s.TextureSize - s.SurfaceSize) * 0.5f + s.SurfaceSize * 0.5f, s.SurfaceSize, UI_BG));
    return f;
}
MySprite Box(Vector2 pos, Vector2 size, Color c)
{
    return new MySprite() { Type = SpriteType.TEXTURE, Data = "SquareSimple",
        Position = pos, Size = size, Color = c, Alignment = TextAlignment.CENTER };
}
MySprite Txt(string text, Vector2 pos, float scale, Color c, TextAlignment al = TextAlignment.CENTER)
{
    return new MySprite() { Type = SpriteType.TEXT, Data = text, Position = pos,
        RotationOrScale = scale, Color = c, Alignment = al, FontId = "White" };
}
void DrawBar(MySpriteDrawFrame f, Vector2 center, float w, float h, double frac, Color col)
{
    float t = 2f;
    f.Add(Box(center, new Vector2(w + t * 2, h + t * 2), UI_FRAME));
    f.Add(Box(center, new Vector2(w, h), UI_BG));
    float fw = (float)(w * Math.Max(0, Math.Min(1, frac)));
    if (fw > 1) f.Add(Box(center + new Vector2(-w * 0.5f + fw * 0.5f, 0), new Vector2(fw, h), col));
}
string TruncS(string t, int max)
{
    return max > 2 && t.Length > max ? t.Substring(0, max - 2) + ".." : t;
}
// Локализованное имя предмета для отображения в Act/Warn/списках.
// При useRussian=true пытается найти русский перевод: сначала "Тип/Подтип", затем SubtypeId.
// При useRussian=false возвращает SubtypeId как есть.
string ItemName(MyItemType t)
{
    if (useRussian)
    {
        string enKey = t.TypeId.Replace("MyObjectBuilder_", "") + "/" + t.SubtypeId;
        string r;
        if (ruNames.TryGetValue(enKey, out r)) return r;
        if (ruNames.TryGetValue(t.SubtypeId, out r)) return r;
    }
    return t.SubtypeId;
}
int SrcInvIndex(IMyTerminalBlock b) { return b is IMyProductionBlock ? 1 : 0; }
int Cat(MyItemType t)
{
    foreach (var s in foodItems) if (t.SubtypeId == s) return FOOD;
    string ty = t.TypeId;
    if (ty.EndsWith("_Ore")) return ORE;
    if (ty.EndsWith("_Ingot")) return INGOT;
    if (ty.EndsWith("_Component")) return COMP;
    if (ty.EndsWith("_AmmoMagazine")) return AMMO;
    if (ty.EndsWith("_OxygenContainerObject") || ty.EndsWith("_GasContainerObject")) return BOTTLE;
    if (ty.EndsWith("_ConsumableItem") || ty.EndsWith("_Ingredient") || ty.EndsWith("_IngredientItem") || ty.EndsWith("_SeedItem") || ty.EndsWith("_Seed") || ty.EndsWith("_Food") || ty.EndsWith("_FoodItem")) return FOOD;
    if (ty.EndsWith("_PhysicalGunObject") || ty.EndsWith("_PhysicalObject") || ty.EndsWith("_Datapad")) return TOOL;
    return -1;
}
bool IsCat(IMyTerminalBlock b, int c) { return cats[c].Contains(b); }
bool IsIce(MyItemType t) { return t.TypeId.EndsWith("_Ore") && t.SubtypeId.IndexOf("Ice", StringComparison.OrdinalIgnoreCase) >= 0; }
bool MatchList(IMyTerminalBlock b, string[] list)
{
    foreach (var s in list)
    {
        if (s == null || s == "") continue;
        if (b.CustomName.Contains(s) || b.BlockDefinition.SubtypeId.Contains(s)) return true;
    }
    return false;
}
bool HasSpace(IMyInventory inv) { return (double)inv.CurrentVolume < (double)inv.MaxVolume * 0.98; }
bool CanMove(IMyInventory src, long srcId, IMyInventory dst, long dstId, MyItemType t)
{
    long key = srcId * 397 ^ dstId;
    bool ok;
    if (convCache.TryGetValue(key, out ok)) return ok;
    try { ok = src.CanTransferItemTo(dst, t); } catch { ok = false; }
    convCache[key] = ok;
    return ok;
}
bool Reachable(IMyTerminalBlock b)
{
    bool v;
    if (reachCache.TryGetValue(b.EntityId, out v)) return v;
    if (reachBudget <= 0) return false;
    reachBudget--;
    IMyInventory refInv = null;
    for (int c = 0; c < NCAT; c++)
    {
        if (cats[c].Count > 0 && cats[c][0] != b) { refInv = cats[c][0].GetInventory(0); break; }
    }
    if (refInv == null) refInv = Me.GetInventory(0);
    try { v = b.GetInventory(0).IsConnectedTo(refInv); }
    catch { v = true; }
    reachCache[b.EntityId] = v;
    return v;
}
IMyTerminalBlock DestFor(int c, IMyTerminalBlock exclude)
{
    foreach (var w in cats[c])
    {
        if (w == exclude) continue;
        if (HasSpace(w.GetInventory(0))) return w;
    }
    return null;
}
bool Tankable(MyItemType t)
{
    foreach (var s in tankableBottles) if (t.SubtypeId == s) return true;
    return false;
}
IMyGasTank FindTank(MyItemType t)
{
    bool hyd = t.TypeId.Contains("GasContainerObject");
    foreach (var tk in tanks)
    {
        if (tk.BlockDefinition.SubtypeId.Contains("Hydrogen") != hyd) continue;
        if (tk.FilledRatio < 0.01) continue;
        var inv = tk.GetInventory(0);
        if ((double)inv.MaxVolume - (double)inv.CurrentVolume < 0.13) continue;
        tk.AutoRefillBottles = true;
        return tk;
    }
    return null;
}
double PullTo(IMyInventory dst, MyItemType t, double amount, IMyTerminalBlock exclude)
{
    double before = (double)dst.GetItemAmount(t);
    double got = 0;
    long dstId = dst.Owner != null ? dst.Owner.EntityId : 0;
    int c = Cat(t);
    var items = new List<MyInventoryItem>();
    for (int pass = 0; pass < 2; pass++)
    {
        var list = (pass == 0 && c >= 0) ? cats[c] : allInv;
        foreach (var b in list)
        {
            if (Runtime.CurrentInstructionCount > 42000) return got;
            if (b == exclude) continue;
            if (pass == 1 && c >= 0 && IsCat(b, c)) continue;
            if (b is IMyReactor) continue;
            if (b is IMyGasGenerator && IsIce(t)) continue;
            if (b is IMyUserControllableGun) continue;
            if (b is IMyShipWelder && (excludeWelders || IsBaR(b))) continue;
            if (excludeGrinders && b is IMyShipGrinder) continue;
            if (excludeDrills && b is IMyShipDrill) continue;
            if (b.CustomName.Contains(HIDDEN_KEY)) continue;
            if (c >= 0 && !b.IsSameConstructAs(Me) && MatchesCat(b.CustomName, c)) continue;
            var inv = b.GetInventory(SrcInvIndex(b));
            if ((double)inv.GetItemAmount(t) <= 0) continue;
            if (!CanMove(inv, b.EntityId, dst, dstId, t)) continue;
            items.Clear(); inv.GetItems(items);
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (!items[i].Type.Equals(t)) continue;
                double take = Math.Min(amount - got, (double)items[i].Amount);
                if (take <= 0) break;
                inv.TransferItemTo(dst, i, null, true, (VRage.MyFixedPoint)take);
                got = (double)dst.GetItemAmount(t) - before;
                if (got >= amount - 0.01) return got;
                if (((double)dst.MaxVolume - (double)dst.CurrentVolume) * 1000.0 < 5) return got;
            }
        }
        if (c < 0) break;
    }
    return got;
}
double PushFrom(IMyInventory src, MyItemType t, double amount, IMyInventory dst)
{
    double before = (double)dst.GetItemAmount(t);
    double done = 0;
    var items = new List<MyInventoryItem>();
    src.GetItems(items);
    for (int i = items.Count - 1; i >= 0; i--)
    {
        if (!items[i].Type.Equals(t)) continue;
        double take = Math.Min(amount - done, (double)items[i].Amount);
        if (take <= 0) break;
        src.TransferItemTo(dst, i, null, true, (VRage.MyFixedPoint)take);
        done = (double)dst.GetItemAmount(t) - before;
        if (done >= amount - 0.01) break;
    }
    return done;
}
double CountOf(MyItemType t) { double v; stock.TryGetValue(t, out v); return v; }
Dictionary<long,bool> barCache = new Dictionary<long,bool>();
bool IsBaR(IMyTerminalBlock b)
{
    bool v;
    if (barCache.TryGetValue(b.EntityId, out v)) return v;
    v = false;
    try { b.GetValueBool("BuildAndRepair.ScriptControlled"); v = true; } catch { }
    barCache[b.EntityId] = v;
    return v;
}
void EnsureSpecialTemplate(IMyTerminalBlock b)
{
    // НЕ трогаем непустую Custom Data — игрок сам решит, когда очистить.
    if (b.CustomData.Trim().Length > 0) return;
    b.CustomData = useRussian
    ? "# DUTC Спецконтейнер\n" +
    "# Одна строка на предмет: Имя=Количество\n" +
    "# Режимы (совместимо с IIM):\n" +
    "#   Имя=100   держать ровно 100, лишнее убирать\n" +
    "#   Имя=100M  держать минимум 100, излишек игнорировать\n" +
    "#   Имя=100L  никогда не пополнять, только убирать сверх 100\n" +
    "#   Имя=All   хранить всё, что сможет получить\n" +
    "# ВАЖНО: всё, что НЕ указано в этом списке, будет выброшено в общий сток.\n" +
    "# Если Custom Data пуста (только комментарии) — контейнер не трогается вообще.\n" +
    "Стальная пластина=0\n"
    : "# DUTC Special Container\n" +
    "# One line per item: Name=Amount\n" +
    "# Modes (IIM compatible):\n" +
    "#   Name=100   keep exactly 100, remove excess\n" +
    "#   Name=100M  keep at least 100, ignore excess\n" +
    "#   Name=100L  never fill, only remove above 100\n" +
    "#   Name=All   store everything it can get\n" +
    "# IMPORTANT: anything NOT listed here will be dumped to general storage.\n" +
    "# If Custom Data is empty (comments only) - the container is left alone.\n" +
    "# Ambiguous names: prefix the type, e.g. Ore/Stone=100\n" +
    "SteelPlate=0\n";
}
void Warn(string m) { if (warnSet.Add(m)) warnings.Add(m); }
void Act(string m)
{
    actions.Insert(0, DateTime.Now.ToString("HH:mm:ss") + " " + m);
    if (actions.Count > 25) actions.RemoveAt(25);
}
string StripPct(string n)
{
    return System.Text.RegularExpressions.Regex.Replace(n, @"\s*\(\d+\.?\d*%\)", "");
}
string Bar(int w, double f)
{
    int fi = (int)Math.Round(Math.Max(0, Math.Min(1, f)) * w);
    return "[" + new string('I', fi) + new string('.', w - fi) + "]";
}
string Pct(double cur, double max)
{
    if (max <= 0) return "0%";
    return Math.Round(cur / max * 100, 1) + "%";
}
string Num(double v)
{
    if (v >= 1000000) return Math.Round(v / 1000000, 1) + "M";
    if (v >= 10000) return Math.Round(v / 1000, 1) + "k";
    return Math.Round(v).ToString();
}