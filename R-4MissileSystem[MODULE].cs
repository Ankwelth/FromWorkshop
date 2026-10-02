        /* Ракетная система Р-4 || Missile R-4 system, version 3.0
         * New Gen! Wow! (sarcasm)
         * -------------------------------------------------------------------------
         * Сделано в Н.С.К.С || Made in N.U.C.C
         * При уменьшении частоты вызова в 10 раз уменьшите переменные в 10 раз! 
         * If you reduce frequency in 10 times, reduce time variables in 10 times!
         * -------------------------------------------------------------------------
         */
        //
        // Строение ракеты || Missile specifications
        //
        static string MissileGroupName = "Р-4"; // Название группы блоков ракет || Missile blocks group name
        static string RCName = "УДР"; // Tag of RC of missile
        static string MergeName = "Стыковочный блок ракеты"; // Tag of merge of missile
        static string MassName = "Масса ракеты"; // Tag of artifical mass of missile
        static string AcbName = "Аккумулятор ракеты"; // Tag of missile batteries
        static string TankName = "Водородный бак ракеты"; // Tag of missile fuel tanks
        static string WarheadName = "Боеголовка ракеты"; // Tag of missile warheads
        static string OrientName = "Ориентир для ракет"; // Tag of orient name for ATGM mode
        static string LCDName = "Дисплей ракет "; // Name of LCD
                                                  // LCDName + 1 =  дисплей с классическим выводом информации, 2 - с новым, 3 - сообщения
                                                  // LCDName + 1 =  display with classic info, 2 - with new, 3 - messages
        static string AIName = "Блок ИИ ракеты"; // Tag of ai offensive and ai moving of missile
        static string ConnName = "Коннектор ракеты"; // Tag of connector of missile
        static string ThrustName = "Ускоритель ракеты"; // Tag of thruster
        static string IceCargoName = "Большой контейнер"; // Tag of container with ice or explosives
        static string IceGenName = "Генератор ракеты"; // Tag of H2O gen
        static string RotorName = "Ротор ракеты"; // Tag rotor name
        static string GyroName = "Гироскоп ракеты"; // Tag of Gyro
        static string GatlingName = "Отстреливатель"; // Name of launch gatling
        static string SensorName = "Сенсор детонатора ракеты"; // Tag of missile sensor (for detonator)
        static string LaunchSensorName = "Сенсор предохранителя пуска ракеты"; // Tag of missile sensor (for failsafe)
        static string ProjName = "Проектор ракеты"; // Tag of missile projector (if auto-builded)
        static string RadarName = "РЛС Изумруд-1"; // Название ПБ РЛС, если целеуказание не идёт напрямую
                                                   // Name of Radar PB if targeting not given directly
        static string TimerExplodeName = "ТАЙМЕРВЗРЫВ"; // Название таймера запускаемого на указанной дистанции до цели
                                                        // Starting at selected distance from target timer name 
        static string TimerStartName = "ТАЙМЕРСТАРТ"; // Название таймера на корабле запускаемого при пуске ракеты
                                                      // Starting on-ship as missile starts timer name
        //
        // Подготовка к взлету || Preparations for launch
        //
        static int StartDirection = 1;
        // Направление пуска двигателей старта || Direction of launch start thrusters 
        // 1 - перед, 2 - верх, 3 - низ, 4 - лево, 5 - право, 6 - назад
        // 1 - forward, 2 - upward, 3 - downward, 4 - left, 5 - right, 6 - backward
        //
        // Максимальные скорости в направлениях, допускающие пуск ракеты
        // Max speeds in directions allows launch of the missile
        // Up - Вверх
        // Down - Вниз
        // Left - Влево
        // Right - Вправо
        // Forward - Вперёд
        // Backward - Назад
        static bool LaunchBlock = true; // Включить блокировку безопасности?
                                        // Enable failsafe launch block?
        static float MaxUpSpeed = 40;
        static float MaxDownSpeed = 40;
        static float MaxLeftpSpeed = 40;
        static float MaxRightSpeed = 40;
        static float MaxForwardSpeed = 40;
        static float MaxBackwardSpeed = 40;

        static float SDist = 5; // Дистанция поиска блоков || Distance of search blocks
        static int TimeR = 150; // Время запуска снаряда || Launch time 
        static int TimeBefore = 1; // Тиков до начала запуска снаряда (после активации таймера старта)
                                     // Tics before launching shell (after launch start timer)
        static int Refresh1 = 150; // Сколько вызовов до удаления списка целей || Tics before of removing targets
        static int IceAmount = 2000; // Как много льда ложить в генератор (если есть)
                                        // How much ice to put in generator (if exists)
        bool CheckBuilding = false; // Проверяем на целостность построенной ракеты по проектору?
                                    // Check readiness of builded missile via projectors?
        static float MinFuel = 70F; // Минимальная заправка баков для пуска % || Min fuel for launch %
        static float MinCharge = 70F; // Минимальная зарядка батарей для пуска % || Min charge for launch %
        static int MaxBlocks = 2; // Сколько блоков может быть не достроено проектором
                                  // How much blocks can not be finished via projector
        //
        // Детонатор || Detonator
        //
        static float RiskMiss = 10; // Если промах будет этого размера и меньше в метрах, детонация произойдёт || If missile misses on that distance from target missile will detonate

        bool CompensateSize = true; // При подрыве боеголовки на расстоянии от центра от цели идет поправка на её размер || Try compensate size of target if HitPosition == null
        static bool ExplOnMiss = true; // Детонация боеголовки не будет прекращена при риске промаха || detonation will be not stopped if miss
        static bool UseTimers = true; // Если на снаряде есть таймер (с названием "ТАЙМЕРСТАРТ") он будет активирован. || If missile have timer with name "ТАЙМЕРСТАРТ" it will be activated
        
        static float TimerStartDist = 900; // Дистанция активации таймера || Distance of timer activation
        static float WDist = 1; // Дистанция подрыва БЧ || Distance of explostion
        static float WDist1 = 100; // Дистанция отсчёта БЧ || Distance of countdown for warhead
        //
        // Настройка ядерных боеголовок || Nuclear warheads settings
        //
        bool AutoExplode = true; // Автоподрыв ЯБЧ || Auto-detonation of nuke

        static float nuclear_distance = 60; // Дистанция автоподрыва || Auto-detonation distannce

        static string nuclear_core_name = "Коннектор ЯБЧ ракеты"; // Название коннектора с ЯБЧ || Nuclear warhead connector name
        static string nuclear_item_name = "SemiAutoPistolMagazine"; // Название взрывчатки для ЯБЧ || Name of explosive for nuke
        
        static int nuclear_item_count = 10000; // Число взрывчатки ЯБЧ || Count of explosives for nuke
        //
        // Режим ракеты с пушками || Missile with guns mode
        //
        static string Gun_name = "Пулемет ракеты"; // Тег пушек ракеты // Misisle guns tag
        static string Ammo_name = "NATO_25x184mm"; // Тег боеприпасов ракеты // Missile ammo tag
        
        bool FlyAsWeapon = true; // Если на ракете есть пушки, она летит как пушечная ракета
                                 // If missile has a guns it will be operate how a gun missile
                                 //NATO_25x184mm, Missile200mm
        static bool Creative = true; // Игнорировать наличие боеприпасов в пушках? Полезно для креатива
                                     // Ignore ammo amount in guns? Useful for creative game mode

        float WeaponSpeed = 400; // Скорость снаряда пушек
        float WeaponDistance = 800; // Дальность снаряда пушек
        float PrecisionRate = -2; // Точность пушек
        
        int AmmoCount = 2; // Число боеприпасов в каждой пушке ракеты || Ammo amount in every gun
        //
        // Наведение || Guidance
        //
        bool UseFlyTime = false; // Учитывать время полета до цели для расчета упреждения? Внимание: не рекомендуется для маневренных целей
                                 // Take into account the flight time to the target to calculate the lead vector? Warning: not recommended for fast targets
        static bool AlternateGuidSystems = false; // Использовать режим ПТРК при отсутсвии целеуказания?
                                                  // Use alternate ATGM guidance if no targets?
        static bool Turn = true; // Вращать ракету для компенсации боковых скоростей гасителем НИЗА блока ДУ?
                                 // Rotate missile for compensate side speeds with DOWNWARD dampeners?
        static bool ResetTurrets = true; // Если турель на ракете видит "лишнюю" цель, её целеуказание будет сброшено
                                         // If missile turret see not needed target his targeting will be reset
        static bool GravDemp = true; // Гасить гравитацию маршевыми двигателями? || Damp gravity by forward thrusters?
        bool RandomDirectionAttack = false; // Заходить на цель со случайного направления?  || Fly to target from random direction?
        bool ModuleTargeting = true; // Использовать блоки ИИ на ракетах для получения наведения на модули?
                                     // Use AI blocks on missiles to get guidance on modules?

        int ModuleTargeting_mode = 0; // Режим выбора модуля, 0 - случайный модуль, 1 - оружие, 2 - двигатели, 3 - энергия
                                      // Module selecting method, 0 - random module, 1 - weapon, 2 - engines, 3 - energy

        float ModuleTargeting_Distance_Deviation = 60.0F; // Максимальная дистанция от центра смещения модулей (защита от ложных целей)
                                                          // Maximum distance from the center of displacement of modules (protection against false targets)
        float ModuleTargeting_Distance = 2500; // Дистанция попытки захвата модуля цели || Distance to try lock-on target module
        float RandomDirectionDistance = 2000; // На какую высоту над целью выходить? || Which height for random direction use?
        float RandomDirectionCancelDistance = 2000; // На каком расстоянии до цели отменять маневр || Cancel maneuver distance to target
        float ATGMdist = 1.2F; // Множитель удаленности линии визирования ПТУР || Target point of ATGM will be on this distance from orient
        float MC = 55; // Коэффициент расстояния для ракеты (чем больше, тем сильнее) || Coeff of distance for missile (than more then more)
        float Modif = 1.3F; // Коэффициент мощности гироскопа (усилитель). || The power factor of the gyroscope (amplifier).
        float MaxxValue = 500; // Коэффициент маневрирования для упреждения как ракеты || Maneuvrablity coeff of missile
        float minTurnTime = 0.5f; // Минимальное время для разворота на цель (секунды) || Min rotating time (sec)
        float Time = 1; // Поправка на скорость работы скрипта || Script speed coeff
        //
        // Настройки самонаведения || Raycast settings
        //
        float C = 6.5f; // Делитель скорости (время в игровых тиках между вызовами скрипта || Speed divider (game tics between script runs)
        float Q = 10; // Коэфф. углубления при повторном сканировании (запас расстояния в случае деформации цели)
                      // Depth coeff for scanning (if target is deformated)
        static bool ScanAnyway = false; // Сканировать цель, даже если её видит радар?
                                        // Scan target even if radar sees it?
        static bool TryFind = false;    // Пытаться искать цель при её утере камерами ракеты? Нужно много камер
                                        // Try to find target again if its lost by cameras? Needs many cams
        static int refreshcams = 4; // Вызовов до пуска лучей || Runs before raycast
        //
        // Общие настройки || General settings
        //
        static bool ErwinRommel = false; // Экономить топливо? || Don't loose fuel?
        static bool ThrustStart = true; // Старт с двигателями || Start with thrusts
        static bool LowMode = false; // Уменьшить частоту вызова в 10 раз? (скажется на точности) || Reduce the frequency? (will affect on accuracy)
        static bool TurnOnMass = true; // Включать блоки массы на дистанции 400 метров от матер. корабля?
        static bool TurnOnMassWhileStart = false; // Включать блоки массы при старте? || Enable mass blocks while starting?
        static bool ENG = false; // English localisation?
        static bool GetSlower = true; // Замедляться, если задач нет? || Get a slower if no tasks?

        static int blocks_update_tick = 10; // Через сколько вызовов обновлять блоки || How much ticks needed before update blocks
        static int inventory_update_tick = 20; // Через сколько вызовов перемещать вещи || How much ticks before transfer items

        static float MaxSpeed = 100; // Максимальная скорость в мире || Max speed in world
        static float min_thrust = 0.2F; // Минимальная скорость двигателя (для регулятора топлива)
                                        // Minimal engine thrust (for fuel regulator)
        //
        // Импульсный двигатель || Pulse mass shift Drive
        //
        // Поддерживается только маршевый импульсный двигатель || Supported only forward pulse drive
        string Ingot_fuel_name = "Iron"; // Какие слитки грузить в контейнер, Iron = железные
                                         // Which ingots load in cargo? Iron = iron ingots
        string Onboard_cargo_tag = "Ящик ИД 1"; // Ящик ИД на самой ракете || Cargo on missile
        string Onboard_rotor_tag = "Ротор ИД 1"; // Ротор на самой ракете || Rotor on missile
        string Onrotor_cargo_tag = "Ящик ИД 2"; // Ящик ИД на втором роторе || Cargo on second rotor
        string Onrotor_rotor_tag = "Ротор ИД 2"; // Ротор ИД на первом роторе || Rotor on first rotor

        bool UsePulseDrive = true; // Использовать ИД, если есть? || Use pulse drive (if any)?

        int DistanceToRotor = 2; // Дистанция до начального ротора || Block dist to first rotor
        int PulseFreq = 1; // Частота импульсов || Pulse frequency

        float IngotCount = 999999999; // Число слитков, которое грузить в контейнеры. 999999999 - значит до отказа
                                      // How much ingots to load, 999999999 - means maximum possible count
        //
        // Декорации || Decorations
        //
        // Индикатор загрузки || Load indicator
        public string IsLoad(string load)
        {
            if (load == "|") return "/";
            if (load == "/") return "_";
            if (load == "_") return "<";
            if (load == "<") return "|";
            return "";
        }
        public string IsAllReady(bool Ready)
        {
            if (Ready) return Pixel(0, 7, 0).ToString();
            else return Pixel(7, 0, 0).ToString();
        }
        string load = "|";
        string load_pixel = "";
        //
        // Информация панели || LCD info
        // RU
        string str1 = "ЦП Ракет Р-4 ";
        string str2 = "Всего ракет: ";
        string str3 = "Из них запущено: ";
        string str4 = "Ракета ";
        string str5 = " в полёте";
        string str6 = " в процессе запуска";
        string str7 = " Дистанция до цели: ";
        string str8 = " Не видит цель!";
        string str9 = "ЭЭ: "; // Электроэнергия
        string str10 = "ЖТ: "; // Жидкое топливо
        string str11 = "СК: "; // Скорость
        string str12 = "РЦ: "; // Расстояние до цели
        string str13 = "УР - "; // Тег ракеты
        string str14 = "ЦУ: "; // Целеуказание (СМН или ПА)
        string str15 = "ОШ: "; // Угол отклонения (в градусах)
        string str16 = ""; // Линейка
        string str17 = "Всего: ";
        string str18 = "Запущено: ";
        string str19 = "Готовность: ";
        string str20 = "Целей: ";
        string str21 = "Ушло: ";
        string str22 = "ПУСК ЗАБЛОКИРОВАН: ОПАСНАЯ СКОРОСТЬ ПОЛЕТА";
        string str23 = "Не готова ракета ";

        string str24 = "Компьютер управления ракетами запущен: ";
        string str25 = "ракета была запущена: ";
        string str26 = "ракета была уничтожена: ";
        string str27 = "ракета сдетонировала: ";
        string str28 = "ракета не смогла взлететь: ";
        string str29 = "ракета потеряла подсветку цели: ";
        string str30 = "ракета перешла на самонаведение: ";
        string str31 = "ракета потеряла цель: ";
        string str32 = "ракета захватила смещение модуля цели: ";
        string str33 = "Сообщения ракетной системы Р-4:";
        string str34 = "ракета ведёт огонь по цели: ";
        // ENG
        string Estr1 = "Computer of R-4 missile system ";
        string Estr2 = "Total missiles: ";
        string Estr3 = "Launched: ";
        string Estr4 = "Missile ";
        string Estr5 = " in flight";
        string Estr6 = " is launching";
        string Estr7 = " Distance to target: ";
        string Estr8 = " Can't see target!";
        string Estr9 = "EE: "; // Energy
        string Estr10 = "H2: "; // Fuel
        string Estr11 = "SP: "; // Speed
        string Estr12 = "DT: "; // Distance to target
        string Estr13 = "MS - "; // Missile tag
        string Estr14 = "TM: "; // Guiding state (active or semi-active)
        string Estr15 = "ER: "; // Course error (deegres)
        string Estr16 = ""; // Line
        string Estr17 = "Total: ";
        string Estr18 = "In-flight: ";
        string Estr19 = "Readiness: ";
        string Estr20 = "Targets: ";
        string Estr21 = "Gone: ";
        string Estr22 = "LAUNCH IS BLOCKED: DANGEROUS SPEED";
        string Estr23 = "Can't launch a missile ";

        string Estr24 = "Missile computer is working: ";
        string Estr25 = "missile has been launched: ";
        string Estr26 = "missile was been destroyed: ";
        string Estr27 = "missile was been detonated: ";
        string Estr28 = "missile can't launch: ";
        string Estr29 = "missile lost semi-active guidance: ";
        string Estr30 = "missile uses on-missile radar guidance: ";
        string Estr31 = "missile lost target: ";
        string Estr32 = "missile locks target module offset: ";
        string Estr33 = "R-4 Missile System messages:";
        string Estr34 = "missile is shooting target: ";
        //
        /////////////////////////////////////////////////////////////////////////////////////////////////
int ref1 = 1;
bool IsLaunchBlocked(IMyRemoteControl shell)
{
if (!LaunchBlock) return false;
double speed;
speed = Vector3D.Dot(shell.WorldMatrix.Up, Vector3D.Normalize(shell.GetShipVelocities().LinearVelocity)) * shell.GetShipVelocities().LinearVelocity.Length();
if (speed > MaxUpSpeed) return true;
speed = Vector3D.Dot(shell.WorldMatrix.Down, Vector3D.Normalize(shell.GetShipVelocities().LinearVelocity)) * shell.GetShipVelocities().LinearVelocity.Length();
if (speed > MaxDownSpeed) return true;
speed = Vector3D.Dot(shell.WorldMatrix.Left, Vector3D.Normalize(shell.GetShipVelocities().LinearVelocity)) * shell.GetShipVelocities().LinearVelocity.Length();
if (speed > MaxLeftpSpeed) return true;
speed = Vector3D.Dot(shell.WorldMatrix.Right, Vector3D.Normalize(shell.GetShipVelocities().LinearVelocity)) * shell.GetShipVelocities().LinearVelocity.Length();
if (speed > MaxRightSpeed) return true;
speed = Vector3D.Dot(shell.WorldMatrix.Forward, Vector3D.Normalize(shell.GetShipVelocities().LinearVelocity)) * shell.GetShipVelocities().LinearVelocity.Length();
if (speed > MaxForwardSpeed) return true;
speed = Vector3D.Dot(shell.WorldMatrix.Backward, Vector3D.Normalize(shell.GetShipVelocities().LinearVelocity)) * shell.GetShipVelocities().LinearVelocity.Length();
if (speed > MaxBackwardSpeed) return true;

return false;
}
public static char Pixel(byte r, byte g, byte b)
{
return (char)(0xe100 + (r << 6) + (g << 3) + b);
}
public bool IsDisplay(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(LCDName)) return false;
IMyTextPanel lcd = block as IMyTextPanel;
if (lcd != null) return true;
return false;
}
public bool IsAI(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(AIName)) return false;
IMyOffensiveCombatBlock ai = block as IMyOffensiveCombatBlock;
if (ai != null) return true;
IMyFlightMovementBlock ai1 = block as IMyFlightMovementBlock;
if (ai1 != null) return true;
return false;
}
public bool IsMerge(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(MergeName)) return false;
IMyShipMergeBlock merge = block as IMyShipMergeBlock;
if (merge != null) return true;
return false;
}
public bool IsRemote(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(RCName)) return false;
IMyShipController RC = block as IMyShipController;
if (RC != null) return true;
return false;
}
public bool IsMass(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(MassName)) return false;
IMyVirtualMass mass = block as IMyVirtualMass;
if (mass != null) return true;
return false;
}
public bool IsAcb(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(AcbName)) return false;
IMyBatteryBlock bat = block as IMyBatteryBlock;
if (bat != null) return true;
return false;
}
public bool IsWarhead(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(WarheadName)) return false;
IMyWarhead warhead = block as IMyWarhead;
if (warhead != null) return true;
return false;
}
public bool IsThrust(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(ThrustName)) return false;
IMyThrust thrust = block as IMyThrust;
if (thrust != null) return true;
return false;
}
public bool IsTimer(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(TimerExplodeName)) return false;
IMyTimerBlock timer = block as IMyTimerBlock;
if (timer != null) return true;
return false;
}
public bool IsConnector(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(ConnName)) return false;
IMyShipConnector connector = block as IMyShipConnector;
if (connector != null) return true;
return false;
}
public bool IsNuclearCore(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(nuclear_core_name)) return false;
IMyShipConnector connector = block as IMyShipConnector;
if (connector != null) return true;
return false;
}
public bool IsGyro(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(GyroName)) return false;
IMyGyro gyro = block as IMyGyro;
if (gyro != null) return true;
return false;
}
public bool IsTank(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(TankName)) return false;
IMyGasTank tank = block as IMyGasTank;
if (tank != null) return true;
return false;
}
public bool IsContainer(IMyTerminalBlock block)
{
IMyCargoContainer cargo = block as IMyCargoContainer;
if (cargo != null) return true;
return false;
}
public bool IsSensor(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(SensorName) && !block.CustomName.Contains(LaunchSensorName)) return false;
IMySensorBlock sensor = block as IMySensorBlock;
if (sensor != null) return true;
return false;
}
public bool IsRotor (IMyTerminalBlock block)
{
if (!block.CustomName.Contains(RotorName)) return false;
IMyMotorStator rotor = block as IMyMotorStator;
if (rotor != null) return true;
return false;
}
public bool IsIceGen (IMyTerminalBlock block)
{
if (!block.CustomName.Contains(IceGenName)) return false;
IMyGasGenerator gen = block as IMyGasGenerator;
if (gen != null) return true;
return false;
}
public bool IsProjector(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(ProjName)) return false;
IMyProjector proj = block as IMyProjector;
if (proj != null) return true;
return false;
}
public bool IsWeapon(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(Gun_name)) return false;
IMyUserControllableGun gun = block as IMyUserControllableGun;
if (gun != null) return true;
return false;
}
public bool IsPulseDriveBlock(IMyTerminalBlock block)
{
if (!block.CustomName.Contains(Onboard_cargo_tag) && !block.CustomName.Contains(Onrotor_cargo_tag) && !block.CustomName.Contains(Onboard_rotor_tag) && !block.CustomName.Contains(Onrotor_rotor_tag)) return false;
IMyCargoContainer cargo = block as IMyCargoContainer;
IMyMotorAdvancedStator rotor = block as IMyMotorAdvancedStator;
if (cargo != null || rotor != null) return true;
return false;
}
bool Fire = false;
bool AllReady = true;
IMyTerminalBlock orient;
IMyProgrammableBlock radar;
IMyCargoContainer MainCargo;
IMyTimerBlock start_timer;
IMyBlockGroup MGroup;
List<IMyTerminalBlock> merges = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> RCS = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> mass = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> generators = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> bats = new List<IMyTerminalBlock>();
List<IMyRemoteControl> FiredRCS = new List<IMyRemoteControl>();
List<IMyTerminalBlock> Warheads = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Connectors = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Thrusts = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Timers = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Gyros = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Rotors = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> IceGens = new List<IMyTerminalBlock>();
List<IMyCameraBlock> Cameras = new List<IMyCameraBlock>();
List<IMyLargeTurretBase> Turrets = new List<IMyLargeTurretBase>();
List<IMySmallGatlingGun> gatlings = new List<IMySmallGatlingGun>();
List<IMyTerminalBlock> Sensors = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Nuclear_cores = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Projectors = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Tanks = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Guns = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> Pulse_Drive = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> LCDs = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> AI_blocks = new List<IMyTerminalBlock>();
List<long> extra_targets = new List<long>();
List<string> TargetsRC = new List<string>();
List<string> Times = new List<string>();
List<string> RandDirs = new List<string>();
List<string> Messages = new List<string>();
List<MyDetectedEntityInfo> Targets = new List<MyDetectedEntityInfo>();
List<long> Scanned_Targets = new List<long>();
MyDetectedEntityInfo Target;
double Refresh;
StringBuilder sb = new StringBuilder();
StringBuilder sb2 = new StringBuilder();
StringBuilder sb_messages = new StringBuilder();
Random rand = new Random();
string current_time;
int blc_upd = 0;
int inv_upd = 0;
int tbs = 0;
int gone_missiles = 0;
Program()
{
Messages.Add(str24 + current_time);
Refresh = Refresh1;
if (!LowMode && !GetSlower) Runtime.UpdateFrequency |= UpdateFrequency.Update1;
else Runtime.UpdateFrequency |= UpdateFrequency.Update100;
if (LowMode) { C /= 10; Time *= 10; }
if (ENG)
{
str1 = Estr1;
str2 = Estr2;
str3 = Estr3;
str4 = Estr4;
str5 = Estr5;
str6 = Estr6;
str7 = Estr7;
str8 = Estr8;
str9 = Estr9;
str10 = Estr10;
str11 = Estr11;
str12 = Estr12;
str13 = Estr13;
str14 = Estr14;
str15 = Estr15;
str16 = Estr16;
str17 = Estr17;
str18 = Estr18;
str19 = Estr19;
str20 = Estr20;
str21 = Estr21;
str22 = Estr22;
str23 = Estr23;
str24 = Estr24;
str25 = Estr25;
str26 = Estr26;
str27 = Estr27;
str28 = Estr28;
str29 = Estr29;
str30 = Estr30;
str31 = Estr31;
str32 = Estr32;
str33 = Estr33;
str34 = Estr34;
}
}
void Main(String args)
{
if (GetSlower)
{
if (Targets.Count < 1 && FiredRCS.Count < 1 && !Fire)
{
Runtime.UpdateFrequency |= UpdateFrequency.Update100;
}
else
{
if (!LowMode) Runtime.UpdateFrequency |= UpdateFrequency.Update1;
else Runtime.UpdateFrequency |= UpdateFrequency.Update10;
}
}
tbs--;
blc_upd--;
inv_upd--;
Refresh--;
ref1--;
load = IsLoad(load);
sb.Clear();
sb2.Clear();
sb_messages.Clear();
Scanned_Targets.Clear();
current_time = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
if (blc_upd < 1)
{
MGroup = GridTerminalSystem.GetBlockGroupWithName(MissileGroupName);
blc_upd = blocks_update_tick;
radar = GridTerminalSystem.GetBlockWithName(RadarName) as IMyProgrammableBlock;
orient = GridTerminalSystem.GetBlockWithName(OrientName);
start_timer = GridTerminalSystem.GetBlockWithName(TimerStartName) as IMyTimerBlock;
MainCargo = GridTerminalSystem.GetBlockWithName(IceCargoName) as IMyCargoContainer;
List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
if (MGroup != null)
{
MGroup.GetBlocks(blocks, IsRemote);
foreach (IMyTerminalBlock block in blocks) { if (!RCS.Contains(block)) RCS.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsMerge);
foreach (IMyTerminalBlock block in blocks) { if (!merges.Contains(block)) merges.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsMass);
foreach (IMyTerminalBlock block in blocks) { if (!mass.Contains(block)) mass.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsAcb);
foreach (IMyTerminalBlock block in blocks) { if (!bats.Contains(block)) bats.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsWarhead);
foreach (IMyTerminalBlock block in blocks) { if (!Warheads.Contains(block)) Warheads.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsConnector);
foreach (IMyTerminalBlock block in blocks) { if (!Connectors.Contains(block)) Connectors.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsTimer);
foreach (IMyTerminalBlock block in blocks) { if (!Timers.Contains(block)) Timers.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsThrust);
foreach (IMyTerminalBlock block in blocks) { if (!Thrusts.Contains(block)) Thrusts.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsGyro);
foreach (IMyTerminalBlock block in blocks) { if (!Gyros.Contains(block)) Gyros.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsIceGen);
foreach (IMyTerminalBlock block in blocks) { if (!IceGens.Contains(block)) IceGens.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsRotor);
foreach (IMyTerminalBlock block in blocks) { if (!Rotors.Contains(block)) Rotors.Add(block); }
blocks.Clear();
MGroup.GetBlocksOfType<IMyCameraBlock>(blocks);
foreach (IMyTerminalBlock block in blocks) { if (!Cameras.Contains((IMyCameraBlock)block)) Cameras.Add((IMyCameraBlock)block); }
blocks.Clear();
MGroup.GetBlocksOfType<IMyLargeTurretBase>(blocks);
foreach (IMyTerminalBlock block in blocks) { if (!Turrets.Contains((IMyLargeTurretBase)block)) Turrets.Add((IMyLargeTurretBase)block); }
blocks.Clear();
MGroup.GetBlocksOfType<IMySmallGatlingGun>(blocks);
foreach (IMyTerminalBlock block in blocks) { if (!gatlings.Contains((IMySmallGatlingGun)block)) gatlings.Add((IMySmallGatlingGun)block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsSensor);
foreach (IMyTerminalBlock block in blocks) { if (!Sensors.Contains(block)) Sensors.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsNuclearCore);
foreach (IMyTerminalBlock block in blocks) { if (!Nuclear_cores.Contains(block)) Nuclear_cores.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsProjector);
foreach (IMyTerminalBlock block in blocks) { if (!Projectors.Contains(block)) Projectors.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsTank);
foreach (IMyTerminalBlock block in blocks) { if (!Tanks.Contains(block)) Tanks.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsWeapon);
foreach (IMyTerminalBlock block in blocks) { if (!Guns.Contains(block)) Guns.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsPulseDriveBlock);
foreach (IMyTerminalBlock block in blocks) { if (!Pulse_Drive.Contains(block)) Pulse_Drive.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsDisplay);
foreach (IMyTerminalBlock block in blocks) { if (!LCDs.Contains(block)) LCDs.Add(block); }
blocks.Clear();
MGroup.GetBlocks(blocks, IsAI);
foreach (IMyTerminalBlock block in blocks) { if (!AI_blocks.Contains(block)) AI_blocks.Add(block); }
blocks.Clear();
}
}

for (int i = merges.Count - 1; i >= 0; i--) { IMyShipMergeBlock merge = merges[i] as IMyShipMergeBlock; if (merge.Closed || !merge.IsFunctional || merge == null) merges.RemoveAt(i); }
for (int i = RCS.Count - 1; i >= 0; i--) { IMyShipController merge = RCS[i] as IMyShipController; if (merge.Closed || !merge.IsFunctional || merge == null) { RCS.RemoveAt(i); Messages.Add((i + 1) + " " + str26 + current_time); } }
for (int i = mass.Count - 1; i >= 0; i--) { IMyVirtualMass merge = mass[i] as IMyVirtualMass; if (merge.Closed || !merge.IsFunctional || merge == null) mass.RemoveAt(i); }
for (int i = bats.Count - 1; i >= 0; i--) { IMyBatteryBlock merge = bats[i] as IMyBatteryBlock; if (merge.Closed || !merge.IsFunctional || merge == null) bats.RemoveAt(i); }
for (int i = FiredRCS.Count - 1; i >= 0; i--) { IMyRemoteControl merge = FiredRCS[i]; if (merge.Closed || !merge.IsFunctional || merge == null) FiredRCS.RemoveAt(i); }
for (int i = Warheads.Count - 1; i >= 0; i--) { IMyWarhead merge = Warheads[i] as IMyWarhead; if (merge.Closed || !merge.IsFunctional || merge == null) Warheads.RemoveAt(i); }
for (int i = Connectors.Count - 1; i >= 0; i--) { IMyShipConnector merge = Connectors[i] as IMyShipConnector; if (merge.Closed || !merge.IsFunctional || merge == null) Connectors.RemoveAt(i); }
for (int i = Timers.Count - 1; i >= 0; i--) { IMyTimerBlock merge = Timers[i] as IMyTimerBlock; if (merge.Closed || !merge.IsFunctional || merge == null) Timers.RemoveAt(i); }
for (int i = Thrusts.Count - 1; i >= 0; i--) { IMyThrust merge = Thrusts[i] as IMyThrust; if (merge.Closed || !merge.IsFunctional || merge == null) Thrusts.RemoveAt(i); }
for (int i = Gyros.Count - 1; i >= 0; i--) { IMyGyro merge = Gyros[i] as IMyGyro; if (merge.Closed || !merge.IsFunctional || merge == null) Gyros.RemoveAt(i); }
for (int i = Rotors.Count - 1; i >= 0; i--) { IMyMotorStator merge = Rotors[i] as IMyMotorStator; if (merge.Closed || !merge.IsFunctional || merge == null) Rotors.RemoveAt(i); }
for (int i = IceGens.Count - 1; i >= 0; i--) { IMyGasGenerator merge = IceGens[i] as IMyGasGenerator; if (merge.Closed || !merge.IsFunctional || merge == null) IceGens.RemoveAt(i); }
for (int i = Cameras.Count - 1; i >= 0; i--) { IMyCameraBlock merge = Cameras[i]; if (merge.Closed || !merge.IsFunctional || merge == null) Cameras.RemoveAt(i); }
for (int i = Turrets.Count - 1; i >= 0; i--) { IMyLargeTurretBase merge = Turrets[i]; if (merge.Closed || !merge.IsFunctional || merge == null) Turrets.RemoveAt(i); }
for (int i = Sensors.Count - 1; i >= 0; i--) { IMySensorBlock merge = Sensors[i] as IMySensorBlock; if (merge.Closed || !merge.IsFunctional || merge == null) Sensors.RemoveAt(i); }
for (int i = Nuclear_cores.Count - 1; i >= 0; i--) { IMyShipConnector merge = Nuclear_cores[i] as IMyShipConnector; if (merge.Closed || !merge.IsFunctional || merge == null) Nuclear_cores.RemoveAt(i); }
for (int i = Projectors.Count - 1; i >= 0; i--) { IMyProjector merge = Projectors[i] as IMyProjector; if (merge.Closed || !merge.IsFunctional || merge == null) Projectors.RemoveAt(i); }
for (int i = Tanks.Count - 1; i >= 0; i--) { IMyGasTank merge = Tanks[i] as IMyGasTank; if (merge.Closed || !merge.IsFunctional || merge == null) Tanks.RemoveAt(i); }
for (int i = Guns.Count - 1; i >= 0; i--) { IMyUserControllableGun merge = Guns[i] as IMyUserControllableGun; if (merge.Closed || !merge.IsFunctional || merge == null) Guns.RemoveAt(i); }
for (int i = Pulse_Drive.Count - 1; i >= 0; i--) { IMyTerminalBlock merge = Pulse_Drive[i]; if (merge.Closed || !merge.IsFunctional || merge == null) Pulse_Drive.RemoveAt(i); }
for (int i = LCDs.Count - 1; i >= 0; i--) { IMyTerminalBlock merge = LCDs[i]; if (merge.Closed || !merge.IsFunctional || merge == null) LCDs.RemoveAt(i); }
for (int i = AI_blocks.Count - 1; i >= 0; i--) { IMyTerminalBlock merge = AI_blocks[i]; if (merge.Closed || !merge.IsFunctional || merge == null) AI_blocks.RemoveAt(i); }


double dist;
if (LCDs.Count > 0) 
{
if (load_pixel == Pixel(0, 7, 0).ToString()) load_pixel = Pixel(7, 7, 0).ToString(); else if (load_pixel == Pixel(7, 7, 0).ToString()) load_pixel = Pixel(0, 4, 0).ToString(); else load_pixel = Pixel(0, 7, 0).ToString();
sb.Append(str1 + load); sb.AppendLine(); sb.Append(str2 + RCS.Count); sb.AppendLine(); sb.Append(str3 + FiredRCS.Count); sb.AppendLine();
sb2.AppendLine(str16); sb2.Append(str1 + load_pixel); sb2.AppendLine(); sb2.Append(str17 + RCS.Count + " " + str18 + FiredRCS.Count + " " + str19 + IsAllReady(AllReady) + " " + str20 + Targets.Count + " " + str21 + gone_missiles); sb2.AppendLine(); sb2.Append(str16);
}
bool blocked = false;
foreach (IMyRemoteControl RC in RCS)
{
if (FiredRCS.Contains(RC)) continue;
blocked = IsLaunchBlocked(RC);
if (blocked) break;
}
if (blocked)
{
sb.Append(str22);
sb.AppendLine();
sb2.Append(str22);
sb2.AppendLine();
}
if (RCS.Count > 0)
{
if (MainCargo != null && (IceGens.Count > 0 || Guns.Count > 0 || Nuclear_cores.Count > 0 || Pulse_Drive.Count > 0) && inv_upd < 1)
{
inv_upd = inventory_update_tick;
List<MyInventoryItem> items = new List<MyInventoryItem>();
MainCargo.GetInventory().GetItems(items);
foreach (MyInventoryItem item in items)
{
if (item.Type.SubtypeId.Equals(Ammo_name) && AmmoCount > 0 && Guns.Count > 0)
{
foreach (IMyUserControllableGun gun in Guns)
{
gun.Enabled = false;
if (MainCargo.GetInventory().IsConnectedTo(gun.GetInventory()))
{
List<MyInventoryItem> items1 = new List<MyInventoryItem>();
gun.GetInventory().GetItems(items1);
if (items1.Count > 0)
{
double amount = 0;
foreach (MyInventoryItem item1 in items1)
{
if (item1.Type.SubtypeId.Equals(Ammo_name)) amount += (double)item1.Amount;
}
MainCargo.GetInventory().TransferItemTo(gun.GetInventory(), item, (VRage.MyFixedPoint)(AmmoCount - amount));
}
else
{
MainCargo.GetInventory().TransferItemTo(gun.GetInventory(), item, (VRage.MyFixedPoint)AmmoCount);
}
}
}
}
if (item.Type.SubtypeId.Equals(Ingot_fuel_name) && Pulse_Drive.Count > 0)
{
foreach (IMyTerminalBlock block in Pulse_Drive)
{
if (!block.CustomName.Contains(Onboard_cargo_tag)) continue;
IMyCargoContainer cargo = block as IMyCargoContainer;
if (cargo == null) continue;

if (MainCargo.GetInventory().IsConnectedTo(cargo.GetInventory()))
{
List<MyInventoryItem> items1 = new List<MyInventoryItem>();
cargo.GetInventory().GetItems(items1);
if (items1.Count > 0)
{
double amount = 0;
foreach (MyInventoryItem item1 in items1)
{
if (item1.Type.SubtypeId.Equals(Ammo_name)) amount += (double)item1.Amount;
}
MainCargo.GetInventory().TransferItemTo(cargo.GetInventory(), item, (VRage.MyFixedPoint)(IngotCount - amount));
}
else
{
MainCargo.GetInventory().TransferItemTo(cargo.GetInventory(), item, (VRage.MyFixedPoint)IngotCount);
}
}
}
}
if (item.Type.SubtypeId.Equals("Ice") && IceAmount > 0 && IceGens.Count > 0)
{
foreach (IMyGasGenerator gen in IceGens)
{
gen.UseConveyorSystem = false;
gen.Enabled = false;
if (MainCargo.GetInventory().IsConnectedTo(gen.GetInventory()))
{
List<MyInventoryItem> items1 = new List<MyInventoryItem>();
gen.GetInventory().GetItems(items1);
if (items1.Count > 0)
{
double amount = 0;
foreach (MyInventoryItem item1 in items1)
{
if (item1.Type.SubtypeId.Equals("Ice")) amount += (double)item1.Amount;
}
MainCargo.GetInventory().TransferItemTo(gen.GetInventory(), item, (VRage.MyFixedPoint)(IceAmount - amount));
}
else
{
MainCargo.GetInventory().TransferItemTo(gen.GetInventory(), item, (VRage.MyFixedPoint)IceAmount);
}
}
}
}
if (Nuclear_cores.Count > 0)
{
if (item.Type.SubtypeId.Equals(nuclear_item_name))
{
foreach (IMyShipConnector nuclear_core in Nuclear_cores)
{
nuclear_core.Enabled = true;
if (MainCargo.GetInventory().IsConnectedTo(nuclear_core.GetInventory()))
{
List<MyInventoryItem> items1 = new List<MyInventoryItem>();
nuclear_core.GetInventory().GetItems(items1);
if (items1.Count > 0)
{
double amount = 0;
foreach (MyInventoryItem item1 in items1)
{
if (item1.Type.SubtypeId.Equals(nuclear_item_name)) amount += (double)item1.Amount; 
}
MainCargo.GetInventory().TransferItemTo(nuclear_core.GetInventory(), item, (VRage.MyFixedPoint)(nuclear_item_count - amount));
}
else
{
MainCargo.GetInventory().TransferItemTo(nuclear_core.GetInventory(), item, (VRage.MyFixedPoint)nuclear_item_count);
}
}
}
}
}
}
}
foreach (IMySmallGatlingGun gun in gatlings)
{
if (gun.CustomName == GatlingName) gun.Enabled = false;
}
for (int P = 0; P < RCS.Count; P++)
{
IMyRemoteControl RC = (IMyRemoteControl)RCS[P];
int MassCount = 0;
int ThrustCount = 0;
bool IsReady = true;
if (!FiredRCS.Contains(RC))
{
if (IsReady)
foreach (IMyBatteryBlock bat in bats)
{
Dist(bat, RC, out dist);
if (dist > SDist) continue;
if (!Fire && !FiredRCS.Contains(RC))bat.ChargeMode = ChargeMode.Recharge;
if (bat.CurrentStoredPower / bat.MaxStoredPower * 100 < MinCharge) IsReady = false;
}
if (IsReady)
foreach (IMyGasTank tank in Tanks)
{
Dist(tank, RC, out dist);
if (dist > SDist) continue;
if (!Fire && !FiredRCS.Contains(RC)) tank.Stockpile = true;
if (tank.FilledRatio * 100 < MinFuel) IsReady = false;
}
if (CheckBuilding)
foreach (IMyProjector proj in Projectors)
{
Dist(proj, RC, out dist);
if (dist <= SDist && proj.RemainingBlocks > MaxBlocks) { IsReady = false; }
}
if (!IsReady) AllReady = false;
if (IsReady)
foreach (IMySensorBlock sensor in Sensors)
{
if (!sensor.CustomName.Contains(LaunchSensorName)) continue;
Dist(sensor, RC, out dist);
if (!sensor.LastDetectedEntity.IsEmpty()) { IsReady = false; }
}
if (IsReady)
{
foreach (IMyArtificialMassBlock mas in mass)
{
Dist(mas, RC, out dist);
if (dist <= SDist) { mas.Enabled = false; MassCount++; }
}
foreach (IMyThrust mas in Thrusts)
{
Dist(mas, RC, out dist);
if (dist <= SDist) { mas.Enabled = false; ThrustCount++; }
}
foreach (IMyGravityGenerator gen in generators)
{
Dist(gen, RC, out dist);
if (dist <= SDist) gen.GravityAcceleration = 0;
}
foreach (IMyShipConnector conn in Connectors)
{
Dist(conn, RC, out dist);
if (dist <= SDist) { conn.Enabled = true; conn.Connect(); }
}
}
}
if (!IsReady) 
{
sb.Append(str23 + P);
sb.AppendLine();
sb2.Append(str23 + P);
sb2.AppendLine();
if (P == RCS.Count - 1) { Fire = false; break; }
continue; 
}

if (Fire && IsReady && (ThrustCount > 0 || UsePulseDrive) && (Target.EntityId != 0 || AlternateGuidSystems) && !FiredRCS.Contains(RC))
{
if (start_timer != null) start_timer.Trigger();
if (tbs > 0) continue;
foreach (IMyBatteryBlock bat in bats)
{
Dist(bat, RC, out dist);
if (dist <= SDist) bat.ChargeMode = ChargeMode.Discharge;
}
foreach (IMyGasTank tank in Tanks)
{
Dist(tank, RC, out dist);
if (dist <= SDist) tank.Stockpile = false;
}
if (TurnOnMassWhileStart)
foreach (IMyArtificialMassBlock mas in mass)
{
Dist(mas, RC, out dist);
if (dist <= SDist) { mas.Enabled = true; MassCount++; }
}
foreach (IMyMotorStator rotor in Rotors)
{
Dist(rotor, RC, out dist);
if (dist <= SDist) rotor.Detach();
}
foreach (IMyGasGenerator gen in IceGens)
{
Dist(gen, RC, out dist);
if (dist <= SDist) { gen.Enabled = true; gen.UseConveyorSystem = true; }
}
foreach (IMyShipMergeBlock merge in merges)
{
Dist(merge, RC, out dist);
if (dist <= SDist) merge.Enabled = false;
}
foreach (IMyArtificialMassBlock mas in mass)
{
Dist(mas, RC, out dist);
if (dist <= SDist) mas.Enabled = true;
}
foreach (IMyShipConnector conn in Connectors)
{
Dist(conn, RC, out dist);
if (dist <= SDist) { conn.Enabled = false; conn.Disconnect(); }
}
if (ThrustStart)
{
foreach (IMyThrust thrust in Thrusts)
{
Dist(thrust, RC, out dist);
if (StartDirection == 1 && dist <= SDist && thrust.WorldMatrix.Backward == RC.WorldMatrix.Forward) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 1; }
if (StartDirection == 2 && dist <= SDist && thrust.WorldMatrix.Backward == RC.WorldMatrix.Up) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 1; }
if (StartDirection == 3 && dist <= SDist && thrust.WorldMatrix.Backward == RC.WorldMatrix.Down) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 1; }
if (StartDirection == 4 && dist <= SDist && thrust.WorldMatrix.Backward == RC.WorldMatrix.Left) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 1; }
if (StartDirection == 5 && dist <= SDist && thrust.WorldMatrix.Backward == RC.WorldMatrix.Right) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 1; }
if (StartDirection == 6 && dist <= SDist && thrust.WorldMatrix.Backward == RC.WorldMatrix.Backward) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 1; }
}
}
if (UsePulseDrive)
{
PulseDrive_Prepare(Pulse_Drive, RC, DistanceToRotor);
}
foreach (IMySmallGatlingGun gun in gatlings)
{
if (gun.CustomName == GatlingName) 
{
Dist(RC, gun, out dist);
if (dist > SDist) continue;
gun.Enabled = true;
gun.ApplyAction("ShootOnce");
break; 
}
}
FiredRCS.Add(RC);
string TimeLaunch = RC.GetId().ToString() + "b" + TimeR;
Times.Add(TimeLaunch);
string IdTarget = "";
if (!Target.IsEmpty()) IdTarget = RC.GetId().ToString() + "b" + Target.EntityId + "b" + -1;
else IdTarget = RC.GetId().ToString() + "b" + 0 + "b" + -1;
if (extra_targets.Count > 0)
{
foreach (long extra_target in extra_targets)
{
bool exist = false;
foreach (MyDetectedEntityInfo target in Targets) if (target.EntityId == extra_target) { exist = true; break; }
if (exist)
{
IdTarget = RC.GetId().ToString() + "b" + extra_target + "b" + -1;
extra_targets.Remove(extra_target);
break;
}
}

}
TargetsRC.Add(IdTarget);
gone_missiles++;
if (!Messages.Contains((P + 1) + " " + str25 + current_time)) Messages.Add((P + 1) + " " + str25 + current_time);
Fire = false;
}
}
if (Targets.Count < 1 && !AlternateGuidSystems) Fire = false;
}
if (FiredRCS.Count > 0 && (Targets.Count > 0 || AlternateGuidSystems))
{
for (int M = 0; M < FiredRCS.Count; M++)
{
IMyRemoteControl RC = FiredRCS[M];
sb2.AppendLine();
sb2.Append(str13 + (M + 1) + ": ");
bool exist = false;
for (int i = Times.Count - 1; i >= 0 || exist; i--)
{
string tim = Times[i];
string[] timstring = tim.Split('b');
long rcid;
long.TryParse(timstring[0], out rcid);
if (RC.GetId() == rcid)
{
double time;
double.TryParse(timstring[1], out time);
time--;
tim = RC.GetId().ToString() + "b" + time.ToString();
if (time > 0) { Times[i] = tim; exist = true; }
else 
{ 
Times.RemoveAt(i); exist = false;
if (RandomDirectionAttack) RandDirs.Add(RC.GetId().ToString() + "b" + rand.Next(0, 6));
}
Echo(time.ToString());
Echo(exist.ToString());
break;
}
}
if (exist) { sb.Append(str4 + RC.EntityId + str6); sb2.Append(str6); sb.AppendLine(); continue; }

if (RC.IsSameConstructAs(Me)) { if (!Messages.Contains((M + 1) + " " + str28 + current_time)) Messages.Add((M + 1) + " " + str28 + current_time); FiredRCS.Remove(RC); break; }
char energy = Pixel(7, 0, 0);
char fuel = Pixel(7, 0, 0); 
char lock_mode = Pixel(0, 7, 0);
if (LCDs.Count > 0)
{
sb.Append(str4 + RC.EntityId + str5);
sb.AppendLine();
foreach (IMyBatteryBlock bat in bats)
{
if (!bat.IsSameConstructAs(RC)) continue;
energy = Pixel((byte)(0.07 * (100 - bat.CurrentStoredPower / bat.MaxStoredPower * 100)), (byte)(0.07 * (bat.CurrentStoredPower / bat.MaxStoredPower * 100)), 0);
break;
}
foreach (IMyGasTank tank in Tanks)
{
if (!tank.IsSameConstructAs(RC)) continue;
fuel = Pixel((byte)(0.07* (100 - tank.FilledRatio * 100)), (byte)(0.07 * (tank.FilledRatio * 100)), 0);
break;
}

sb2.Append(str9 + energy + " ");
sb2.Append(str10 + fuel + " ");
sb2.Append(str11 + (int)RC.GetShipSpeed() + " ");
}

if (Targets.Count > 0 || AlternateGuidSystems)
{
long id = 0;
string offset = "";
int offset_timer = 0;
int offset_index = 0;
for (int N = 0; N < TargetsRC.Count; N++)
{
string target = TargetsRC[N];
string[] oh = target.Split('b');
long idr;
long.TryParse(oh[0], out idr);
if (idr == RC.GetId())
{
long.TryParse(oh[1], out id);
offset = oh[2];
offset_index = N;
int.TryParse(oh[2], out offset_timer);
break;
}
}

MyDetectedEntityInfo Targed = new MyDetectedEntityInfo();
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == id) 
{
Targed = target;
bool See;
if (RC.CustomData.Length == 0) See = true;
else bool.TryParse(RC.CustomData, out See);
if (ref1 <= 0 && ((radar == null || !radar.CustomData.Contains(Targed.EntityId.ToString()) || ScanAnyway) && !Scanned_Targets.Contains(Targed.EntityId)))
{
if (!Messages.Contains((M + 1) + " " + str29 + current_time)) Messages.Add((M + 1) + " " + str29 + current_time);
lock_mode = Pixel(7, 0, 0);
for (int i = Cameras.Count - 1; i >= 0; i--)
{
IMyCameraBlock cam = Cameras[i];
if (cam.IsSameConstructAs(RC))
{
Vector3D dir;
if (Targed.HitPosition == null)
{
dir = Targed.Position + Vector3D.Normalize(Targed.Position - cam.GetPosition()) * Q + target.Velocity / C;
}
else
{
dir = (Vector3D)Targed.HitPosition + Vector3D.Normalize((Vector3D)Targed.HitPosition - cam.GetPosition()) * Q + Targed.Velocity / C;
}
cam.Enabled = true;
if (!cam.EnableRaycast) cam.EnableRaycast = true;
MyDetectedEntityInfo targ;
targ = cam.Raycast(dir);
if (targ.IsEmpty())
{
See = false;
RC.CustomData = "false";
if (!TryFind) continue;
for (int i1 = i; i1 >= 0; i1--)
{
Vector3D v = new Vector3D(Targed.Position.X + rand.Next(-10, 10), Targed.Position.Y + rand.Next(-10, 10), target.Position.Z + rand.Next(-10, 10));
v += target.Velocity / C;
cam = Cameras[i1];
if (!cam.CanScan(v)) continue;
targ = cam.Raycast(v);
if (targ.IsEmpty()) continue;
}
}
if (!targ.IsEmpty() && targ.EntityId == Targed.EntityId) 
{
if (!Messages.Contains((M + 1) + " " + str30 + current_time)) Messages.Add((M + 1) + " " + str30 + current_time);
Scanned_Targets.Add(targ.EntityId); 
Refresh = Refresh1; 
Targed = targ; 
Targets.Remove(target); 
Targets.Add(targ); 
See = true; RC.CustomData = "true";
lock_mode = Pixel(7, 7, 0);
break; 
}
}
}
}
if (!See) { sb.Append(str4 + str8); sb.AppendLine(); if (!Messages.Contains((M + 1) + " " + str31 + current_time)) Messages.Add((M + 1) + " " + str31 + current_time); }
for (int i = Turrets.Count - 1; i >= 0; i--)
{
IMyLargeTurretBase Turret = Turrets[i];
if (Turret.IsSameConstructAs(RC))
{
Turret.Enabled = true;
MyDetectedEntityInfo targ;
if (Turret.HasTarget) targ = Turret.GetTargetedEntity();
else continue;
if (!targ.IsEmpty() && targ.EntityId == target.EntityId) { Refresh = Refresh1; Targed = targ; Targets.Remove(target); Targets.Add(targ); break; }
else if (ResetTurrets) Turret.ResetTargetingToDefault();
}
}
break; 
}
}
if (LCDs.Count > 0) sb2.Append(str14 + lock_mode + " ");
if (Targets.Count > 0 || AlternateGuidSystems)
{
List<IMyThrust> BordThrust = new List<IMyThrust>();
foreach (IMyThrust thrust in Thrusts)
{
if (thrust.IsSameConstructAs(RC)) BordThrust.Add(thrust);
}
List<IMyUserControllableGun> BordGuns = new List<IMyUserControllableGun>();
foreach (IMyUserControllableGun gun in Guns)
{
if (gun.IsSameConstructAs(RC)) BordGuns.Add(gun);
}
Vector3D ShootVector;
Vector3D EnemyPos = new Vector3D();
Vector3D Direction = new Vector3D();
if (Targed.EntityId != 0)
{
EnemyPos = (Vector3D)Targed.HitPosition;
if (Targed.HitPosition == null) EnemyPos = Targed.Position;
if (ModuleTargeting && !offset.Contains(":"))
{
bool locked = false;
if (Direction.Length() < ModuleTargeting_Distance && offset_timer < 1)
{
foreach (IMyTerminalBlock block in AI_blocks)
{
if (!block.IsSameConstructAs(RC)) continue;
IMyOffensiveCombatBlock ai_offensive = block as IMyOffensiveCombatBlock;
if (ai_offensive == null) continue;
ai_offensive.Enabled = true;
ai_offensive.SelectedAttackPattern = 3;
ai_offensive.UpdateTargetInterval = 0;
if (offset_timer == -1)
{
ai_offensive.ApplyAction("ActivateBehavior_On", null);
offset_timer = ai_offensive.UpdateTargetInterval + 1;
if (LowMode) offset_timer *= 10;
else offset_timer *= 100;
ai_offensive.SetValue<long>("OffensiveCombatIntercept_GuidanceType", 0);
int targeting = ModuleTargeting_mode;
if (targeting == 0) { targeting = rand.Next(1, 3); }
if (targeting == 1) ai_offensive.ApplyAction("SetTargetingGroup_Weapons");
else
if (targeting == 2) ai_offensive.ApplyAction("SetTargetingGroup_Propulsion");
else
if (targeting == 3) ai_offensive.ApplyAction("SetTargetingGroup_PowerSystems");
else ai_offensive.SetValue<long>("OffensiveCombatIntercept_GuidanceType", rand.Next(0, 3));
ai_offensive.SetValueBool("OffensiveCombatIntercept_OverrideCollisionAvoidance", true);
}
if (offset_timer == 0)
{
if (ai_offensive.SearchEnemyComponent.FoundEnemyId == null || ai_offensive.SearchEnemyComponent.FoundEnemyId == 0)
{
offset_timer = ai_offensive.UpdateTargetInterval + 1;
if (LowMode) offset_timer *= 10;
else offset_timer *= 100;
}
}
break;
}
foreach (IMyTerminalBlock block in AI_blocks)
{
if (!block.IsSameConstructAs(RC)) continue;
IMyFlightMovementBlock ai_flight = block as IMyFlightMovementBlock;
if (ai_flight == null) continue;
ai_flight.Enabled = true;
ai_flight.MinimalAltitude = 0;
ai_flight.PrecisionMode = false;
ai_flight.SpeedLimit = 100;
ai_flight.AlignToPGravity = false;
ai_flight.CollisionAvoidance = false;
if (offset_timer == -1) ai_flight.ApplyAction("ActivateBehavior_Off", null);
if (offset_timer == 0)
{
ai_flight.ApplyAction("ActivateBehavior_On", null);
offset_timer = -2;
}
if (ai_flight.CurrentWaypoint != null)
{
IMyAutopilotWaypoint waypoint = ai_flight.CurrentWaypoint;
Vector3D offset_pos = new Vector3D(waypoint.Matrix.GetRow(3).X, waypoint.Matrix.GetRow(3).Y, waypoint.Matrix.GetRow(3).Z);
if (Vector3D.Distance(Targed.Position, offset_pos) < ModuleTargeting_Distance_Deviation)
{
offset_pos -= Targed.Position;
offset_pos = Vector3D.TransformNormal(offset_pos, MatrixD.Transpose(Targed.Orientation));
TargetsRC[offset_index] = RC.GetId().ToString() + "b" + Targed.EntityId + "b" + offset_pos.X + ":" + offset_pos.Y + ":" + offset_pos.Z;
Messages.Add((M + 1) + " " + str32 + current_time);
}
else TargetsRC[offset_index] = RC.GetId().ToString() + "b" + Targed.EntityId + "b" + EnemyPos.X + ":" + EnemyPos.Y + ":" + EnemyPos.Z;
ai_flight.ApplyAction("ActivateBehavior_Off", null);
locked = true;
}
break;
}
}
if (!locked && offset_timer > -2 && !offset.Contains(":"))
{
offset_timer--;
TargetsRC[offset_index] = RC.GetId().ToString() + "b" + Targed.EntityId + "b" + offset_timer;
}
}
else
{
if (ModuleTargeting && offset.Contains(":"))
{
double x = 0;
double y = 0;
double z = 0;
string[] off = offset.Split(':');
double.TryParse(off[0], out x);
double.TryParse(off[1], out y);
double.TryParse(off[2], out z);
EnemyPos += Vector3D.TransformNormal(new Vector3D(x, y, z), Targed.Orientation);
}
}
Vector3D OurPos = RC.GetPosition();
Direction = EnemyPos - OurPos;
Vector3D targetVelOrth = Vector3D.Dot(Targed.Velocity, Vector3D.Normalize(Direction)) * Vector3D.Normalize(Direction);
Vector3D targetVelTang = Vector3D.Reject(Targed.Velocity, Vector3D.Normalize(Direction));
double FlyTime = 1;
if (UseFlyTime) FlyTime = Direction.Length() / RC.GetShipSpeed();
if (targetVelTang.Length() > RC.GetShipSpeed())
{
ShootVector = Vector3D.Normalize(Targed.Velocity) * RC.GetShipSpeed() * Time * FlyTime;
}
else
{
double shotSpeedOrth = Math.Sqrt(RC.GetShipSpeed() * RC.GetShipSpeed() - targetVelTang.Length() * targetVelTang.Length());
Vector3D shotVelOrth = Vector3D.Normalize(Direction) * shotSpeedOrth;
ShootVector = (shotVelOrth + targetVelTang) * Time * FlyTime;
}
}
else
{
if (orient == null) ShootVector = (Me.CubeGrid.GetPosition() + Me.WorldMatrix.Forward * (Vector3D.Distance(RC.GetPosition(), Me.CubeGrid.GetPosition())) * ATGMdist) - RC.GetPosition() - RC.GetShipVelocities().LinearVelocity;
else ShootVector = (orient.GetPosition() + orient.WorldMatrix.Forward * (Vector3D.Distance(RC.GetPosition(), orient.GetPosition())) * ATGMdist) - RC.GetPosition() - RC.GetShipVelocities().LinearVelocity;
Direction = ShootVector;
Direction = new Vector3D(0, 10000, 0);
}
double FForward = Vector3D.Dot(RC.WorldMatrix.Forward, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100;
double RRight = Vector3D.Dot(RC.WorldMatrix.Right, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100;
double UUp = Vector3D.Dot(RC.WorldMatrix.Up, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100;
sb.Append(str7 + (int)Direction.Length());
sb.AppendLine();
sb2.Append(str12 + (int)Direction.Length() + " ");
if (minTurnTime > 0 && Targets.Count > 0)
{
// Относительная скорость (упрощённо)
double relSpeed = Math.Abs(RC.GetShipSpeed() - Targed.Velocity.Length());
// Минимальное время на корректировку (подбирается опытно) minTurnTime
// Минимальная дистанция = сколько ракета пролетит за это время
double minDistance = relSpeed * minTurnTime;
// Защита от деления на 0
if (minDistance < 1) minDistance = 1;
if (Direction.Length() > minDistance)
CustomReflect(RC.GetShipVelocities().LinearVelocity, ShootVector, out ShootVector);
}
else
if (!AlternateGuidSystems)
CustomReflect(RC.GetShipVelocities().LinearVelocity, ShootVector, out ShootVector);
if (UseTimers) TimerAct(RC, Direction, Targed);
if (BordGuns.Count > 0 && FlyAsWeapon)
{
bool Empty = true;
foreach (IMyTerminalBlock weapon in BordGuns)
{
List<MyInventoryItem> contitems = new List<MyInventoryItem>();
foreach (IMyUserControllableGun gun in BordGuns)
{
contitems.Clear();
if (gun.GetInventory().ItemCount > 0) Empty = false;
if (Creative) Empty = false;
if (!Empty && Direction.Length() <= WeaponDistance && FForward >= PrecisionRate) 
{ 
gun.Shoot = true;
if (!ENG) sb.Append(" СТРЕЛЯЕТ");
else sb.Append(" IS SHOOTING");
sb.AppendLine();
if (!Messages.Contains((M + 1) + " " + str34 + current_time)) Messages.Add((M + 1) + " " + str34 + current_time);
}
else gun.Shoot = false;
}
}
if (!Empty && Direction.Length() <= WeaponDistance)
{
ShootVector = EnemyPos + (Vector3D)Targed.Velocity * (Direction.Length() / WeaponSpeed) - RC.GetPosition();
ShootVector -= RC.GetShipVelocities().LinearVelocity * (Direction.Length() / WeaponSpeed);
}
}
if (RandomDirectionAttack && Targed.EntityId != 0)
{
for (int i = RandDirs.Count - 1; i >= 0; i--)
{
string tim = RandDirs[i];
string[] timstring = tim.Split('b');
long rcid;
long.TryParse(timstring[0], out rcid);
if (RC.GetId() == rcid)
{
if (Direction.Length() < RandomDirectionCancelDistance) { RandDirs.RemoveAt(i); break; }
int dir;
int.TryParse(timstring[1], out dir);
if (dir == 0) { RandDirs.RemoveAt(i); break; }
else if (dir == 1) { ShootVector = RC.GetPosition() + (EnemyPos + Targed.Orientation.Forward * RandomDirectionDistance);    }
else if (dir == 2) { ShootVector = RC.GetPosition() + (EnemyPos + Targed.Orientation.Backward * RandomDirectionDistance);  }
else if (dir == 3) { ShootVector = RC.GetPosition() + (EnemyPos + Targed.Orientation.Right * RandomDirectionDistance);   }
else if (dir == 4) { ShootVector = RC.GetPosition() + (EnemyPos + Targed.Orientation.Left * RandomDirectionDistance);    }
else if (dir == 5) { ShootVector = RC.GetPosition() + (EnemyPos + Targed.Orientation.Up * RandomDirectionDistance);  }
else if (dir == 6) { ShootVector = RC.GetPosition() + (EnemyPos + Targed.Orientation.Down * RandomDirectionDistance);   }
if (ShootVector.Length() < 100 || FForward < 0) { RandDirs.RemoveAt(i); break; }
CustomReflect(RC.GetShipVelocities().LinearVelocity, ShootVector, out ShootVector);
break;
}
}
}
if (LCDs.Count > 0) sb2.Append(str15 + (int)(Vector3D.Angle(Vector3D.Normalize(RC.GetShipVelocities().LinearVelocity), Vector3D.Normalize(ShootVector)) * 57.29577951308) + "' ");
List<IMyGyro> BordGyros = new List<IMyGyro>();
foreach (IMyGyro gyro in Gyros)
{
if (!BordGyros.Contains(gyro))
{
if (gyro.IsSameConstructAs(RC))
{
BordGyros.Add(gyro);
}
}
}
Vector3D forward = RC.WorldMatrix.Forward;
Vector3D dirNorm = Vector3D.Normalize(ShootVector);
double dot = Vector3D.Dot(RC.WorldMatrix.Forward, Vector3D.Normalize(ShootVector));
if (dot < -0.95)
{
Vector3D sideways = RC.WorldMatrix.Right;
Vector3D up = RC.WorldMatrix.Up;

// Выбираем, куда сильнее тянет цель
double sideDot = Vector3D.Dot(Vector3D.Normalize(ShootVector), sideways);
double upDot = Vector3D.Dot(Vector3D.Normalize(ShootVector), up);

// Создаём "пинок" в эту сторону
ShootVector += sideways * sideDot * 0.5 + up * upDot * 0.5;
}
foreach (IMyGyro gyro in BordGyros)
{
gyro.Enabled = true;
float gForward = 0;
float gRight = 0;
float gUp = 0;
Vector3D grav = new Vector3D();
double speed = RC.GetShipVelocities().LinearVelocity.Length();
double gravityCompensationFactor = 0.5 * grav.Length() / Math.Max(speed * speed, 1);

if (RC.GetNaturalGravity().Length() > 0) grav = RC.GetNaturalGravity();

// Поворачиваем ракеты, если гравитация включена или она выключена
if (!GravDemp || RC.GetNaturalGravity().Length() == 0)
{
gForward = (float)(Vector3D.Dot(gyro.WorldMatrix.Forward, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100);
gRight = (float)(Vector3D.Dot(gyro.WorldMatrix.Right, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100);
gUp = (float)(Vector3D.Dot(gyro.WorldMatrix.Up, Vector3D.Normalize(ShootVector)) / ShootVector.Length() * 100);
}
else
{
gForward = (float)(Vector3D.Dot(gyro.WorldMatrix.Forward, Vector3D.Normalize(ShootVector) + grav * gravityCompensationFactor) / ShootVector.Length() * 100);
gRight = (float)(Vector3D.Dot(gyro.WorldMatrix.Right, Vector3D.Normalize(ShootVector) + grav * gravityCompensationFactor) / ShootVector.Length() * 100);
gUp = (float)(Vector3D.Dot(gyro.WorldMatrix.Up, Vector3D.Normalize(ShootVector) + grav * gravityCompensationFactor) / ShootVector.Length() * 100);
}

gyro.GyroOverride = true;

double D = ShootVector.Length();
if (D > 1250) D = 1250;  // Сохраняем лимит для дальних целей
if (FlyAsWeapon && Direction.Length() < WeaponDistance) D = MC;
// Используем улучшенную систему для разных ориентаций гироскопов
if (gyro.WorldMatrix.Forward == RC.WorldMatrix.Forward)
{
gyro.Pitch = (float)(Math.Atan2(-gUp, gForward) / D * MC * Modif);
gyro.Yaw = (float)(Math.Atan2(gRight, gForward) / D * MC * Modif);
if (Turn) gyro.Roll = (float)(Math.Atan2(gRight, gUp) * 1.5); // Правильное вращение
}
else if (gyro.WorldMatrix.Backward == RC.WorldMatrix.Forward)
{
gyro.Pitch = (float)(Math.Atan2(gUp, -gForward) / D * MC * Modif);
gyro.Yaw = (float)(Math.Atan2(-gRight, -gForward) / D * MC * Modif);
if (Turn) gyro.Roll = (float)(Math.Atan2(-gRight, gUp) * 1.5); // Правильное вращение
}
else if (gyro.WorldMatrix.Right == RC.WorldMatrix.Forward)
{
if (Turn) gyro.Pitch = (float)(Math.Atan2(gForward, gUp) / D * MC * Modif);  // Проверка оси
gyro.Yaw = (float)(-Math.Atan2(gForward, gRight) / D * MC * Modif);
gyro.Roll = (float)(-Math.Atan2(gUp, gRight) * 1.5);  // Верный знак для оси
}
else if (gyro.WorldMatrix.Left == RC.WorldMatrix.Forward)
{
if (Turn) gyro.Pitch = (float)(Math.Atan2(-gForward, gUp) / D * MC * Modif);  // Правильное изменение
gyro.Yaw = (float)(Math.Atan2(gForward, -gRight) / D * MC * Modif);
gyro.Roll = (float)(Math.Atan2(gUp, -gRight) * 1.5);  // Корректное вращение
}
else if (gyro.WorldMatrix.Up == RC.WorldMatrix.Forward)
{
gyro.Pitch = (float)(Math.Atan2(gForward, gUp) / D * MC * Modif);  // Верный расчёт
if (Turn) gyro.Yaw = (float)(Math.Atan2(gRight, gForward) / D * MC * Modif);
gyro.Roll = (float)(Math.Atan2(gRight, gUp) * 1.5);  // Правильное вращение по оси
}
else if (gyro.WorldMatrix.Down == RC.WorldMatrix.Forward)
{
gyro.Pitch = (float)(-Math.Atan2(gForward, gUp) / D * MC * Modif);  // Исправляем знак
if (Turn) gyro.Yaw = (float)(-Math.Atan2(gRight, gForward) / D * MC * Modif);
gyro.Roll = (float)(-Math.Atan2(gRight, gUp) * 1.5);  // Верный расчёт для Roll
}
}
foreach (IMyArtificialMassBlock mas in mass)
{
if (mas.IsSameConstructAs(RC)) mas.Enabled = false;
}
if (UsePulseDrive)
{
PulseDrive_Pulse(ShootVector, Pulse_Drive, RC, PulseFreq, DistanceToRotor, FForward);
}
if (!ErwinRommel)
{
if (FForward > 0)
{
foreach (IMyThrust thrust in BordThrust)
{
if (thrust.WorldMatrix.Backward == RC.WorldMatrix.Forward) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 1; }
if (thrust.WorldMatrix.Backward != RC.WorldMatrix.Forward) { thrust.Enabled = true; thrust.ThrustOverridePercentage = 0; }
}
}
else
{
foreach (IMyThrust thrust in BordThrust)
{
thrust.Enabled = true;
thrust.ThrustOverridePercentage = 0;
}
}
}
else
{
float forward_coeff = (float)Vector3D.Dot(RC.WorldMatrix.Forward, Vector3D.Normalize(ShootVector)) + min_thrust;
foreach (IMyThrust thrust in BordThrust) RegulateThrust(thrust, RC, ShootVector, MaxSpeed, forward_coeff);
}
foreach (IMySensorBlock sensor in Sensors)
{
if (!sensor.IsSameConstructAs(RC) || !sensor.CustomName.Contains(SensorName)) continue;
if (!sensor.LastDetectedEntity.IsEmpty())
{
foreach (IMyWarhead warhead in Warheads)
if (warhead.IsSameConstructAs(RC)) { warhead.IsArmed = true; warhead.Detonate(); if (!Messages.Contains((M + 1) + " " + str27 + current_time)) Messages.Add((M + 1) + " " + str27 + current_time); }
}
}
if (CompensateSize)
{
double radius = (Targed.BoundingBox.Max - Targed.BoundingBox.Min).Length() / 2;
double centerDist = Vector3D.Distance(RC.GetPosition(), EnemyPos);
if (EnemyPos != Targed.Position) radius = 0;

if (centerDist <= radius + WDist)
{
foreach (IMyWarhead warhead in Warheads)
if (warhead.IsSameConstructAs(RC)) { warhead.IsArmed = true; warhead.Detonate(); if (!Messages.Contains((M + 1) + " " + str27 + current_time)) Messages.Add((M + 1) + " " + str27 + current_time); }
}
if (centerDist <= radius + nuclear_distance && AutoExplode)
{
foreach (IMyShipConnector nuclear_core in Nuclear_cores)
if (nuclear_core.IsSameConstructAs(RC))
{
nuclear_core.ThrowOut = true;
if (nuclear_core.GetInventory().ItemCount == 0)
{
foreach (IMyWarhead warhead in Warheads)
if (warhead.IsSameConstructAs(RC)) { warhead.IsArmed = true; warhead.Detonate(); if (!Messages.Contains((M + 1) + " " + str27 + current_time)) Messages.Add((M + 1) + " " + str27 + current_time); }
break;
}
}
}
}
else
if (AutoExplode && Direction.Length() < nuclear_distance)
{
foreach (IMyShipConnector nuclear_core in Nuclear_cores)
if (nuclear_core.IsSameConstructAs(RC))
{
nuclear_core.ThrowOut = true;
if (nuclear_core.GetInventory().ItemCount == 0)
{
foreach (IMyWarhead warhead in Warheads)
if (warhead.IsSameConstructAs(RC)) { warhead.IsArmed = true; warhead.Detonate(); if (!Messages.Contains(M + " " + str27 + current_time)) Messages.Add(M + " " + str27 + current_time); }
break;
}
}

}
foreach (IMyWarhead warhead in Warheads)
{
if (warhead.IsSameConstructAs(RC) && !warhead.IsCountingDown && Direction.Length() < WDist1 && (ExplOnMiss || (UUp < RiskMiss && UUp > -RiskMiss && RRight < RiskMiss && RRight > -RiskMiss))) { warhead.DetonationTime = (float)(Direction.Length() / (RC.GetShipVelocities().LinearVelocity - Targed.Velocity).Length()); warhead.StartCountdown(); }
else if (warhead.IsSameConstructAs(RC) && warhead.IsCountingDown && Direction.Length() > WDist1 && !ExplOnMiss) warhead.StopCountdown();
if (warhead.IsSameConstructAs(RC) && Direction.Length() < WDist) { warhead.IsArmed = true; warhead.Detonate(); if (!Messages.Contains(M + " " + str27 + current_time)) Messages.Add(M + " " + str27 + current_time); }
}
}

foreach (IMyArtificialMassBlock mas in mass)
{
if (mas.IsSameConstructAs(RC) && TurnOnMass && (Me.CubeGrid.GetPosition() - RC.GetPosition()).Length() <= 400) mas.Enabled = true;
}
}
}
}
if (ref1 <= 0) ref1 = refreshcams;
if (!ENG)
{
Echo("ЦП Р-4 " + load);
Echo("Сделано в Н.С.К.С");
Echo("Ракет доступно: " + RCS.Count);
Echo("Из них запущено: " + FiredRCS.Count);
Echo("Кол-во переданных целей: " + Targets.Count);
}
else
{
Echo("R-4 Missile System " + load);
Echo("Made in N.U.C.C ");
Echo("Total missiles: " + RCS.Count);
Echo("Launched: " + FiredRCS.Count);
Echo("Amount of transmited targets: " + Targets.Count);
}
if (!ENG) sb.Append("Ошибки:");
else sb.Append("Errors:");
sb.AppendLine();
if (!ENG) if (MainCargo == null) sb.Append("Не найден грузовой контейнер корабля");
else if (MainCargo == null) sb.Append("Ship Cargo container not found");
sb.AppendLine();
if (Target.EntityId == 0) { if (!ENG) sb.Append("Атакуемой цели нет"); else sb.Append("No target for attack"); if (AlternateGuidSystems) { if (!ENG) sb.Append(": РЕЖИМ ПТРК"); else sb.Append(": ATGM MODE"); } sb.AppendLine(); }
sb_messages.Clear();
sb_messages.Append(str33);
sb_messages.AppendLine();
for (int i = Messages.Count - 1; i >= 0; i--)
{
sb_messages.Append(Messages[i]);
sb_messages.AppendLine();
}
foreach (IMyTerminalBlock block in LCDs) 
{
if (block.CustomName.Contains("1")) { IMyTextPanel LCD = block as IMyTextPanel; LCD.Enabled = true; LCD.WriteText(sb.ToString()); LCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; continue; }
if (block.CustomName.Contains("2")) { IMyTextPanel LCD = block as IMyTextPanel; LCD.Enabled = true; LCD.WriteText(sb2.ToString()); LCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; LCD.Font = "Monospace"; continue; }
if (block.CustomName.Contains("3")) { IMyTextPanel LCD = block as IMyTextPanel; LCD.Enabled = true; LCD.WriteText(sb_messages.ToString()); LCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE; continue; }
}

/***************************/
switch (args)
{
case "FIRE": if ((Target.EntityId != 0 || AlternateGuidSystems) && RCS.Count > 0) { Fire = true; tbs = TimeBefore; } break; 
case "COMPENSATESIZE": CompensateSize = !CompensateSize; Messages.Add("CompensateSize: " + CompensateSize.ToString() + ": " + current_time); break;
case "CHECKBUILDING": CheckBuilding = !CheckBuilding; Messages.Add("CheckBuilding: " + CheckBuilding.ToString() + ": " + current_time); break;
case "USEWEAPON": FlyAsWeapon = !FlyAsWeapon; Messages.Add("FlyAsWeapon: " + FlyAsWeapon.ToString() + ": " + current_time); break;
case "USEFLYTIME": UseFlyTime = !UseFlyTime; Messages.Add("UseFlyTime: " + UseFlyTime.ToString() + ": " + current_time); break; 
case "USEPULSEDRIVE": UsePulseDrive = !UsePulseDrive; Messages.Add("UsePulseDrive: " + UsePulseDrive.ToString() + ": " + current_time); break;
case "CLEAR": Messages.Clear(); break;
case "MODULETARGETING": ModuleTargeting = !ModuleTargeting; Messages.Add("ModuleTargeting: " + ModuleTargeting.ToString() + ": " + current_time); break;
case "EXPL":
for (int i = 0; i < FiredRCS.Count; i++)
{
IMyRemoteControl RC = FiredRCS[i];
foreach (IMyWarhead warhead in Warheads)
{
if (warhead.IsSameConstructAs(RC)) { warhead.IsArmed = true; warhead.Detonate(); if (!Messages.Contains(i + " " + str27 + current_time)) Messages.Add(i + " " + str27 + current_time); }
}
}
; break;
}
if (args.Contains("MODULETARGETINGSELECT:"))
{
string[] s = args.Split(':');
int module = 0;
if (s.Length > 0) int.TryParse(s[1], out module);
if (module > 3) module = 3;
ModuleTargeting_mode = module;
Messages.Add("ModuleTargeting_Mode: " + ModuleTargeting_mode.ToString() + ": " + current_time);
}
if (args.Contains("ATTACK@"))
{
tbs = TimeBefore;
string[] s = args.Split('@');
long extra_target;
long.TryParse(s[1], out extra_target);
extra_targets.Add(extra_target);
Fire = true;
}
if (radar != null && radar.Enabled && radar.CustomData.Length > 1)
{
Targets.Clear();
int q = 0;
string[] rinfo = radar.CustomData.Split('@');
int.TryParse(rinfo[0], out q);

if (q > 0)
{
Refresh = Refresh1;
string[] info = rinfo[1].Split('b');
Vector3D Pos;
Vector3D.TryParse(info[0], out Pos);
Vector3D Veloc;
Vector3D.TryParse(info[1], out Veloc);
Vector3D min;
Vector3D max;
Vector3D.TryParse(info[2], out min);
Vector3D.TryParse(info[3], out max);
BoundingBoxD Box = new BoundingBoxD(min, max);
long id;
long.TryParse(info[4], out id);
MatrixD Orientation = new MatrixD();
if (info.Length > 5)
{
string[] or_s = info[6].Split(':');
Orientation = new MatrixD(double.Parse(or_s[0]), double.Parse(or_s[1]), double.Parse(or_s[2]), double.Parse(or_s[3]), double.Parse(or_s[4]), double.Parse(or_s[5]), double.Parse(or_s[6]), double.Parse(or_s[7]), double.Parse(or_s[8]), double.Parse(or_s[9]), double.Parse(or_s[10]), double.Parse(or_s[11]), double.Parse(or_s[12]), double.Parse(or_s[13]), double.Parse(or_s[14]), double.Parse(or_s[15]));
}
Target = new MyDetectedEntityInfo(id, "MainTarget", MyDetectedEntityType.LargeGrid, Pos, Orientation, Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);
if (Targets.Count > 0 && Target.EntityId != 0)
{
bool exist = false;
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == Target.EntityId) { exist = true; break; }
}
if (!exist) Targets.Add(Target);
if (exist)
{
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == id) { Targets.Remove(target); Targets.Add(Target); break; }
}
}
}
else if (Target.EntityId != 0) Targets.Add(Target);

for (int i = 2; i <= q; i++)
{
info = rinfo[i].Split('b');
Vector3D.TryParse(info[0], out Pos);
Vector3D.TryParse(info[1], out Veloc);
Vector3D.TryParse(info[2], out min);
Vector3D.TryParse(info[3], out max);
Box = new BoundingBoxD(min, max);
long.TryParse(info[4], out id);
Orientation = new MatrixD();
if (info.Length > 5)
{
string[] or_s = info[6].Split(':');
Orientation = new MatrixD(double.Parse(or_s[0]), double.Parse(or_s[1]), double.Parse(or_s[2]), double.Parse(or_s[3]), double.Parse(or_s[4]), double.Parse(or_s[5]), double.Parse(or_s[6]), double.Parse(or_s[7]), double.Parse(or_s[8]), double.Parse(or_s[9]), double.Parse(or_s[10]), double.Parse(or_s[11]), double.Parse(or_s[12]), double.Parse(or_s[13]), double.Parse(or_s[14]), double.Parse(or_s[15]));
}
MyDetectedEntityInfo Darget = new MyDetectedEntityInfo(id, "OtherTarget", MyDetectedEntityType.LargeGrid, Pos, Orientation, Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);
if (Targets.Count > 0 && Darget.EntityId != 0)
{
bool exist = false;
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == Darget.EntityId) { exist = true; break; }
}
if (!exist) Targets.Add(Darget);
if (exist)
{
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == id) { Targets.Remove(target); Targets.Add(Darget); break; }
}
}
}
else if (Darget.EntityId != 0) Targets.Add(Darget);
}
}
}
if (args.EndsWith("TTARGET") && !args.EndsWith("TARGET"))
{
string[] info = args.Split('b');
Vector3D Pos;
Vector3D.TryParse(info[0], out Pos);
Vector3D Veloc;
Vector3D.TryParse(info[1], out Veloc);
Vector3D min;
Vector3D max;
Vector3D.TryParse(info[2], out min);
Vector3D.TryParse(info[3], out max);
BoundingBoxD Box = new BoundingBoxD(min, max);
long id;
long.TryParse(info[4], out id);
MatrixD Orientation = new MatrixD();
if (info.Length > 5)
{
string[] or_s = info[6].Split(':');
Orientation = new MatrixD(double.Parse(or_s[0]), double.Parse(or_s[1]), double.Parse(or_s[2]), double.Parse(or_s[3]), double.Parse(or_s[4]), double.Parse(or_s[5]), double.Parse(or_s[6]), double.Parse(or_s[7]), double.Parse(or_s[8]), double.Parse(or_s[9]), double.Parse(or_s[10]), double.Parse(or_s[11]), double.Parse(or_s[12]), double.Parse(or_s[13]), double.Parse(or_s[14]), double.Parse(or_s[15]));
}
Target = new MyDetectedEntityInfo(id, "TargetedEnt", MyDetectedEntityType.LargeGrid, Pos, Orientation, Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);
if (Target.EntityId != 0) Refresh = Refresh1;
if (Targets.Count > 0 && Target.EntityId != 0)
{
bool exist = false;
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == Target.EntityId) { exist = true; if (exist) { Targets.Remove(target); Targets.Add(Target); } break; }
}
if (!exist) Targets.Add(Target);
}
else if (Target.EntityId != 0) Targets.Add(Target);
}
if (args.EndsWith("TARGET") && !args.EndsWith("TTARGET"))
{
string[] info = args.Split('b');
Vector3D Pos;
Vector3D.TryParse(info[0], out Pos);
Vector3D Veloc;
Vector3D.TryParse(info[1], out Veloc);
Vector3D min;
Vector3D max;
Vector3D.TryParse(info[2], out min);
Vector3D.TryParse(info[3], out max);
BoundingBoxD Box = new BoundingBoxD(min, max);
long id;
long.TryParse(info[4], out id);
MatrixD Orientation = new MatrixD();
if (info.Length > 5)
{
string[] or_s = info[6].Split(':');
Orientation = new MatrixD(double.Parse(or_s[0]), double.Parse(or_s[1]), double.Parse(or_s[2]), double.Parse(or_s[3]), double.Parse(or_s[4]), double.Parse(or_s[5]), double.Parse(or_s[6]), double.Parse(or_s[7]), double.Parse(or_s[8]), double.Parse(or_s[9]), double.Parse(or_s[10]), double.Parse(or_s[11]), double.Parse(or_s[12]), double.Parse(or_s[13]), double.Parse(or_s[14]), double.Parse(or_s[15]));
}
MyDetectedEntityInfo Darget = new MyDetectedEntityInfo(id, "TargetedEnt", MyDetectedEntityType.LargeGrid, Pos, Orientation, Veloc, MyRelationsBetweenPlayerAndBlock.Enemies, Box, 0);

if (Targets.Count > 0 && Darget.EntityId != 0)
{
bool exist = false;
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == Darget.EntityId) { exist = true; if (exist) { Targets.Remove(target); Targets.Add(Darget); } break; }
}
if (!exist) Targets.Add(Darget);
}
else if (Darget.EntityId != 0) Targets.Add(Darget);
}
if (Refresh <= 0) { Refresh = Refresh1; Targets.Clear(); Target = new MyDetectedEntityInfo(); }
}
void Dist(IMyTerminalBlock block, IMyTerminalBlock block1, out double Distance)
{
if (!block.IsSameConstructAs(block1)) { Distance = 1000000000;  return; }
Distance = (block.GetPosition() - block1.GetPosition()).Length();
}
void TimerAct(IMyRemoteControl RC, Vector3D Direction, MyDetectedEntityInfo Targed)
{

if (CompensateSize)
{
double radius = (Targed.BoundingBox.Max - Targed.BoundingBox.Min).Length() / 2;
double centerDist = Vector3D.Distance(RC.GetPosition(), Targed.Position);
if (Targed.HitPosition != Targed.Position) radius = 0;

if (centerDist <= radius + TimerStartDist)
{
foreach (IMyTimerBlock timer in Timers)
{
if (timer.IsSameConstructAs(RC))
{
timer.Enabled = true;
timer.Trigger();
break;
}
}
}
}
else
if (TimerStartDist >= Direction.Length())
{
foreach (IMyTimerBlock timer in Timers)
{
if (timer.IsSameConstructAs(RC))
{
timer.Enabled = true;
timer.Trigger();
break;
}
}
}
}
void CustomReflect(Vector3D velocity, Vector3D direction, out Vector3D adjustedDirection)
{
Vector3D directionNorm = Vector3D.Normalize(direction);
// Боковая составляющая (занос) — перпендикуляр к цели
Vector3D lateral = Vector3D.Reject(velocity, directionNorm);
// Дистанция до цели — чем ближе, тем важнее точность
double distance = direction.Length();
// Масштаб для коррекции — плавный, но ощутимый
double correctionFactor = MathHelper.Clamp(MaxxValue / distance, 0.1, 2.5);
// Итоговый ShootVector: выравниваем курс, убирая занос
adjustedDirection = direction - lateral * correctionFactor;
// Если летим совсем не в ту сторону — разворачиваем
if (Vector3D.Dot(velocity, directionNorm) < 0)
{
adjustedDirection = directionNorm * velocity.Length(); // прямой курс
}
}
void RegulateThrust(IMyThrust thruster, IMyRemoteControl RCS, Vector3D targetDirection, double targetSpeed, float forward)
{
// Рассчитываем угол между текущей скоростью и нужным направлением
double angle = Vector3D.Angle(Vector3D.Normalize(targetDirection), Vector3D.Normalize(RCS.GetShipVelocities().LinearVelocity));
// Если угол слишком мал (меньше определенного порога, например 1°), выключаем двигатель
if (thruster.WorldMatrix.Backward == RCS.WorldMatrix.Forward)
{
if (angle < MathHelper.ToRadians(3) && RCS.GetShipSpeed() > targetSpeed - 1 && RCS.GetNaturalGravity().Length() < 0.05)
{
thruster.ThrustOverridePercentage = 0; // Выключаем двигатель
}
else
{
// Иначе, продолжаем работать как обычно, или можно выставить нормальную тягу
thruster.ThrustOverridePercentage = 1 * forward; // Используем полную тягу (или другую логику)
}
}
}
void PulseDrive_Prepare(List<IMyTerminalBlock> cargo_and_rotors, IMyShipController RC, int distance_to_rotor)
{
foreach (IMyTerminalBlock block in cargo_and_rotors)
{
IMyMotorAdvancedStator rotor = block as IMyMotorAdvancedStator;
float amount = 0;
if (rotor == null || !rotor.IsSameConstructAs(RC) || (!rotor.CustomName.Contains(Onboard_rotor_tag) && !rotor.CustomName.Contains(Onrotor_rotor_tag))) continue;
rotor.Torque = 0;
rotor.TargetVelocityRPM = 0;
rotor.RotorLock = true;
rotor.Displacement = 0.11F;
if (!rotor.CustomName.Contains(Onboard_rotor_tag)) continue;
foreach (IMyTerminalBlock block1 in cargo_and_rotors)
{
IMyCargoContainer cargo_stator = block1 as IMyCargoContainer;
if (cargo_stator == null || !cargo_stator.IsSameConstructAs(RC) || !cargo_stator.CustomName.Contains(Onboard_cargo_tag) || Vector3D.Distance(cargo_stator.GetPosition(), rotor.GetPosition()) > distance_to_rotor) continue;
foreach (IMyTerminalBlock block2 in cargo_and_rotors)
{
IMyCargoContainer cargo_rotor = block2 as IMyCargoContainer;
if (cargo_rotor == null || !cargo_rotor.IsSameConstructAs(RC) || !cargo_rotor.CustomName.Contains(Onrotor_cargo_tag) || Vector3D.Distance(cargo_rotor.GetPosition(), rotor.GetPosition()) > distance_to_rotor) continue;

if (cargo_rotor.GetInventory().ItemCount > 0)
cargo_rotor.GetInventory().TransferItemTo(cargo_stator.GetInventory(), (MyInventoryItem)cargo_rotor.GetInventory().GetItemAt(0), 999999999);

break;
}
if (cargo_stator.GetInventory().ItemCount > 0)
{
MyInventoryItem item = (MyInventoryItem)cargo_stator.GetInventory().GetItemAt(0);
amount = (float)item.Amount;
}
break;
}
rotor.CustomData = "0@" + amount.ToString();
}
}
void PulseDrive_Pulse(Vector3D MoveVector, List<IMyTerminalBlock> cargo_and_rotors, IMyShipController RC, int pulse_freq, int distance_to_rotor, double FForward)
{
// Если нужно перемещение по команде пилота, то скармливаем MoveIndicator
foreach (IMyTerminalBlock block in cargo_and_rotors)
{
IMyMotorAdvancedStator rotor = block as IMyMotorAdvancedStator;
if (rotor == null || !rotor.CustomName.Contains(Onboard_rotor_tag) || !rotor.IsSameConstructAs(RC)) continue;
IMyCargoContainer cargo_stator = block as IMyCargoContainer;
IMyCargoContainer cargo_rotor = block as IMyCargoContainer;
IMyMotorAdvancedStator top_rotor = block as IMyMotorAdvancedStator;
foreach (IMyTerminalBlock block1 in cargo_and_rotors)
{
IMyMotorAdvancedStator dat_rotor = block as IMyMotorAdvancedStator;
if (dat_rotor == null || !dat_rotor.IsSameConstructAs(RC) || !dat_rotor.CustomName.Contains(Onrotor_rotor_tag) || Vector3D.Distance(dat_rotor.GetPosition(), rotor.GetPosition()) > distance_to_rotor) continue;
top_rotor = dat_rotor;
break;
}
foreach (IMyTerminalBlock block1 in cargo_and_rotors)
{
IMyCargoContainer cargo = block1 as IMyCargoContainer;
if (cargo == null || !cargo.IsSameConstructAs(RC) || !cargo.CustomName.Contains(Onrotor_cargo_tag) || Vector3D.Distance(cargo.GetPosition(), rotor.GetPosition()) > distance_to_rotor) continue;
cargo_rotor = cargo;
break;
}
foreach (IMyTerminalBlock block1 in cargo_and_rotors)
{
IMyCargoContainer cargo = block1 as IMyCargoContainer;
if (cargo == null || !cargo.IsSameConstructAs(RC) || !cargo.CustomName.Contains(Onboard_cargo_tag) || Vector3D.Distance(cargo.GetPosition(), rotor.GetPosition()) > distance_to_rotor) continue;
cargo_stator = cargo;
break;
}
if (cargo_stator == null || cargo_rotor == null) continue;
float amount;
int time_to_pulse;
string[] custom_data = rotor.CustomData.Split('@');
float.TryParse(custom_data[1], out amount);
int.TryParse(custom_data[0], out time_to_pulse);
if (time_to_pulse < 1)
{
if (rotor.WorldMatrix.Down == RC.WorldMatrix.Forward)
{
if (FForward > 0)
{
if (rotor.Displacement == 0.11F)
{
if (cargo_rotor.GetInventory().ItemCount > 0) cargo_rotor.GetInventory().TransferItemTo(cargo_stator.GetInventory(), (MyInventoryItem)cargo_rotor.GetInventory().GetItemAt(0), 999999999);
rotor.Displacement = -0.11F;
top_rotor.Displacement = -0.11F;
}
else
{
if (cargo_stator.GetInventory().ItemCount > 0) cargo_stator.GetInventory().TransferItemTo(cargo_rotor.GetInventory(), (MyInventoryItem)cargo_stator.GetInventory().GetItemAt(0), (VRage.MyFixedPoint)amount);
rotor.Displacement = 0.11F;
top_rotor.Displacement = 0.11F;
}
}
else
{
if (rotor.Displacement == -0.11F)
{
if (cargo_rotor.GetInventory().ItemCount > 0) cargo_rotor.GetInventory().TransferItemTo(cargo_stator.GetInventory(), (MyInventoryItem)cargo_rotor.GetInventory().GetItemAt(0), 999999999);
rotor.Displacement = 0.11F;
top_rotor.Displacement = 0.11F;
}
else
{
if (cargo_stator.GetInventory().ItemCount > 0) cargo_stator.GetInventory().TransferItemTo(cargo_rotor.GetInventory(), (MyInventoryItem)cargo_stator.GetInventory().GetItemAt(0), (VRage.MyFixedPoint)amount);
rotor.Displacement = -0.11F;
top_rotor.Displacement = -0.11F;
}
}
}
else
{
if (FForward < 0)
{
if (rotor.Displacement == 0.11F)
{
if (cargo_rotor.GetInventory().ItemCount > 0) cargo_rotor.GetInventory().TransferItemTo(cargo_stator.GetInventory(), (MyInventoryItem)cargo_rotor.GetInventory().GetItemAt(0), 999999999);
rotor.Displacement = -0.11F;
top_rotor.Displacement = -0.11F;
}
else
{
if (cargo_stator.GetInventory().ItemCount > 0) cargo_stator.GetInventory().TransferItemTo(cargo_rotor.GetInventory(), (MyInventoryItem)cargo_stator.GetInventory().GetItemAt(0), (VRage.MyFixedPoint)amount);
rotor.Displacement = 0.11F;
top_rotor.Displacement = 0.11F;
}
}
else
{
if (rotor.Displacement == -0.11F)
{
if (cargo_rotor.GetInventory().ItemCount > 0) cargo_rotor.GetInventory().TransferItemTo(cargo_stator.GetInventory(), (MyInventoryItem)cargo_rotor.GetInventory().GetItemAt(0), 999999999);
rotor.Displacement = 0.11F;
top_rotor.Displacement = 0.11F;
}
else
{
if (cargo_stator.GetInventory().ItemCount > 0) cargo_stator.GetInventory().TransferItemTo(cargo_rotor.GetInventory(), (MyInventoryItem)cargo_stator.GetInventory().GetItemAt(0), (VRage.MyFixedPoint)amount);
rotor.Displacement = -0.11F;
top_rotor.Displacement = -0.11F;
}
}
}
time_to_pulse = pulse_freq;
}
time_to_pulse--;
rotor.CustomData = time_to_pulse.ToString() + "@" + amount.ToString();
}
}
//