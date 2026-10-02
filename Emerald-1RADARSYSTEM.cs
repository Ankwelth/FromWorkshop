/*<------------------------------------------------------->
         * РАДИОЛОКАЦИОННАЯ СИСТЕМА <<Изумруд-1>>, версия 3.2 | RADAR SYSTEM <<Emerald-1>>, version 3.2
         * Сделано в Н.С.К.С. Androidом                        | Made in USSR-N.U.C.C by Android
         *                                                     |
         *                                                     |
         * 
         * Руководство пользователя || User Manual  https://steamcommunity.com/sharedfiles/filedetails/?id=2072454011
         */
        //
        // Названия блоков
        string RemName = "Блок ДУ"; // Название ДУ или кабины  || RemCon or cockpit name
        string Proj1n = "Проектор 1"; // Название обычного гол. прицела || Name of Projector of basic sight
        string Proj2n = "Проектор 2"; // Название гол. прицела при поиске цели || Name of Projector active sight
        string BTAG = "SOVIETRADARTRANSMIT"; // Тег для радиосвязи (соединяемые должны иметь один тег) || Broadcast tag 
        string CamName = "Камеры"; // Название группы камер || Cameras Group Name
        string TurretCamName = "Камеры турелей"; // Название группы камер для защиты от дружественного огня управляемых турелей || Name for group of turret cameras for save against friendly-fire
        string TurretsName = "Турели"; // Название группы турелей || Turrets Group Name
        string TurretsRName = "Турели для ракет"; // Название группы турелей для ракет || Turrets for missiles group name
        string TurretsIRName = "Турели для ИК-Ракет"; // Название группы турелей для ИК-Ракет || Name of group for IR-Missiles
        string AIBlockName = "Блок ИИ Нападения РЛС"; // Название блока наступательного ИИ || Name of offensive AI block
        string AIBlockName2 = "Блок ИИ Перемещения РЛС"; // Название блока перемещения ИИ || Name of moving AI block
        string LCDName = "Дисплей РЛС "; // + 1 - Название дисплеев графики || Name of graphical display
        string DinName = "Динамик РЛС"; // Название динамика тревоги РЛС || Name of alert sound block for radar
        string TalertName = "Динамик тревоги РЛС"; // Название таймера тревоги РЛС (если обнаружен враг) || Name of alert timer block for radar (if found any targets)
        // + 2 - Название дисплеев главной информации и списка целей || Name of basic information and targets list display
        // + 3 - Название дисплеев предупреждения || Name of warnings display
        // + 4 - Название дисплеев статуса ракет || Name of missile status display
        // + 5 - Название дисплеев статуса готовности ракет || Name of missile ready status display
        // + 6 - Название дисплеев для вывода координат захваченной цели || Name of  display for GPS coods
        // + 7 - Название дисплеев для вывода радиоприцела || Name of display for radio-sight
        // + 8 - Название дисплеев для вывода изменяемых настроек || Name of display for show settings what can be change by args
        // Например, Дисплей РЛС 1 || For example, Дисплей РЛС 1
        string AntName = "Антенна"; // Название антенны || Antenna name
        string SensName = "Сенсоры"; // Название группы сенсоров || Sensor group name
        string RLSName = "Изумруд-1"; // Название РЛС (декор) || Name of RADAR (decor)
        string BatsName = "Аккумуляторы ракет"; // Название группы батарей ракет || Name of Missiles Battery Group
        string TanksName = "Водородные баки ракет"; // Название топливных баков ракет || Name of Missiles Tanks Group
        string MissileRdavsPBName = "ПБ1"; // Название ПБ ракет от Rdav's || Name of PB with Rdav's PvP HM script
        string MissileWhipsPBName = "ПБ2"; // Название ПБ Launch Control для ракет Whip's  || Name of PB with Whip's launch control script
        string MissileLidatPBName = "ПБ3"; // Название ПБ Launch Control для ракет Alysius || Name of PB with Alysius launch control script
        string MissileR4PBName = "ПБ4"; // Название ПБ для ракет R-4 || Name of PB with R-4 script
        string R_FORWARDName = "R_FORWARD";
        string IR_FORWARDName = "IR_FORWARD #A#";
        string NotR_FORWARDName = "Турель";
        string ControlledTurretsN = "Турели контролируемые РЛС"; // Турели, контролируемые РЛС || Turrets, controlled by radar.
        string Connected_PB_tag = "ПБР"; // Тег в названии ПБ, которые нужно напрямую подключить к РЛС || Tag in names of PB, which need to directly connect 
        //
        // Названия ракет || Missiles Name
        string MissileName = "Р-4Р"; // R-4 System Name
        string Missile1Name = "Rdav's"; // Rdav's System Name
        string Missile2Name = "Whip's"; // Whip's System Name
        string Missile3Name = "Alysius"; // Alysius System Name
        //
        // Декорирование вида РЛС, цели. (у каждого символа в игре разная длина на дисплее, вы можете поломать изображение РЛС, если увеличите длину.)
        // Customization of RADAR graphic (each symbol in the game has a different length on the display, you can broke the radar graphic if you increase the length.)
        int resolution = 30; // Разрешение РЛС, чем больше, тем сильнее лаги || Radar resolution more => laggy
        float width_coeff = 1.2F; // Коэффицицент ширины изображения || Image width coeff
        float height_coeff = 1.0F; // Коэффициент высоты изображения || Image height coeff
        float thickness = 0.05F; // Толщина кругов || Circles thikness

        bool SmoothCorners = true; // Смягчить углы? || Smooth corners?
        bool DrawCircles = true; // Рисовать круги? || Draw circles?
        bool DrawCross = true; // Рисовать крест? || Draw cross? 

        bool Circle_1_3 = false; // Рисовать круг на 1/3 радиуса? || Draw circle on 1\3 of range?
        bool Circle_2_3 = false; // Рисовать круг на 2/3 радиуса? || Draw circle on 2\3 of range?
        bool Circle_3_3 = false; // Рисовать круг на 3/3 радиуса? || Draw circle on 3\3 of range?
        bool Circle_1_2 = true; // Рисовать круг на половине радиуса? || Draw circle on 1\2 of range?

        char center_target = '*'; // Символ центра || Center symbol
        char selected_target = 'X'; // Символ выбранной цели || Selected target symbol
        char enemy_target = 'E'; // Символ вражеской цели || Enemy target symbol
        char friendly_target = 'F'; // Символ дружеской цели  || Friend target symbol
        char owner_target = 'F'; // Символ собственной цели  || Owner target symbol
        char neutral_target = 'N'; // Символ нейтральной цели  || Neutral target symbol
        char unknown_target = 'U'; // Символ неизвестной цели  || Unknown target symbol
        char blind_space = ' '; // Символ пустого места  || Empty space symbol
        char circle_space = '#'; // Символ круга  || Circle space symbol
        char cross_space = '.'; // Символ креста  || Cross space symbol
        char edge_space = '+'; // Символ угла   || Edge space symbol
        //
        // Декорирование текста РЛС (русский)
        string Name = "РЛС ";
        string TargetsD = "Целей обнаружено: ";
        string TargetsN = "Целей не обнаружено.";
        string Mode = "Режим РЛС: ";
        string TargetedT = "<<ВЫБРАННАЯ ЦЕЛЬ>>";
        string TNam = "Название цели: ";
        string TSpe = "Скорость цели: ";
        string TDis = "Расстояние до цели: ";
        string TRel = "Принадлежность цели: ";
        string Alar = "ПРЕДУПРЕЖДЕНИЯ РЛС ";
        string AMode = "Автоматический запуск ракет: ";
        string TurrO = "Точный режим: ";
        //
        // Customization of radar information (ENG)
        string EName = "RADAR ";
        string ETargetsD = "Targets: ";
        string ETargetsN = "Targets not found.";
        string EMode = "RADAR MODE: ";
        string ETargetedT = "<<SELECTED TARGET>>";
        string ETNam = "Target name: ";
        string ETSpe = "Target speed: ";
        string ETDis = "Distance to target: ";
        string ETRel = "Target relationship: ";
        string EAlar = "RADAR WARNINGS ";
        string EAmode = "Automated launch of missiles: ";
        string ETurrO = "Percision mode: ";
        //
        string text_divider = "_________________________________________________";
        //
        // Настройки переменых
        // Variables Settings
        bool BroadcastEn = true;
        static bool UseTurretsForScan = true; // Позволяет сканировать камерами в направлении турели в которую смотрит игрок || Allows you to scan with cameras in the direction of the turret that the player is looking at 
        static double RemoveRate = 10; // Частота обновления целей по вещанию, не ставьте больше, чем вам нужно! || Broadcasting target cleaner don't set more than you need!
        static double MaxRange = 5000; // Максимальная дальность РЛС || Max radar distance
        static double BroadcastRange = 1000; // Дальность обмена информацией, не ставьте больше, чем вам нужно! || Target broadcast range, don't set more than you need!
        static bool Debug = false;
        static bool Auto = true; // Автозапуск скрипта || Script auto-launch
        static bool MouseSelector = false; // Выбор ближайшей к линии визирования цели, а не по команде || Select nearest to view line target
        static bool TargetUpd = true;
        static bool AllTurrets = true;
        bool AI_Scanning = true; // Позволяет искать цели с помощью блока ИИ на дальности до 2,5 км. Требует камер!
                                 // Allow to find targets via AI block on 2.5 km range. Cameras needed!
        bool FriendlyOff = false; // Запрещает захват дружественных целей || Prohibite to lock-on friendly targets
        static bool ENG = false; // English translation. Set true if needed
        static int Emode = 2; // Режим ракет Alysius, поддерживаются: 1,2,9,3
                              // Alysius missiles homing mode, supported: 1,2,9,3
        static bool AllInf = true; //Передавать все данные о цели? || Broadcast all target info?
        static bool UseTimersInstead = false; // использовать таймеры, вместо ПБ для пуска ракет? || Use timers instead PB for launch missiles?


        //
        // Захват целей
        // Targets lock-on
        static bool AdvAct = true; // Режим активного сканирования в случайном направлении, а не перед камерой
                                   // Active scanning mode with random angle raycast, not straight from camera
        static float Q = 5.5F; // Делитель скорости (для захвата цели) || Target speed divider (for target lock-on)
        static int ai_scan_freq = 40; // Частота поиска целей блоком ИИ || Scanning via AI-block frequency 
        int Frequency = 15; // Частота сканирования в активном режиме
        //
        // Баллистический прицел
        // Ballistic sight
        static double WeaponSpeed = 400;
        static double WeaponRange = 800;
        static bool BallGravity = true;
        //
        // Статус ракет
        // Missile Status 
        static bool StatusEnabled = true; // Отображать статус ракет или нет || Show missile status or not
        static bool EmergencyOff = true;
        static double EmergencyCharge = 50;
        static double EmergencyTanks = 50;
        static bool RdavEn = true;
        static bool WhipsEn = true;
        static bool AlysiusEn = true;
        static bool R4En = true;
        //
        // Автопуск ракет
        // Autolaunch missiles
        bool SAutoLaunch = false;
        bool LAutoLaunch = false;
        static double SAutoRange = 1000;
        static double LAutoRange = 1000;
        static bool ScanLaunch = false;
        static int MissilesCount = 1;
        static double IgnoredSize = 5;
        static double SRate = 10;
        static double LRate = 10;
        static int SAtype = 2; // 0-3
        static int LAtype = 2; // 0-3
        static bool IDNotType = false; // Идентифицировать размер корабля не по типу сетки? || Identificate ship size not by grid type?
        static double SizeS = 40; // Корабль размером выше этого значения считается большим || Ship have size more than this value identificated as big
        //
        // Контроль турелей
        // Turrets control
        static bool BlindCheck = true; // Осуществлять проверку на препятствие вслепую, без камер? Только свой корабль будет виден!
                                       // Try to check sight line without cameras? Only own ship will be checked
        static int BlindCheckRange = 200; // Дальность слепой проверки, если включена || Range of blind check, if enabled
        bool ControlTurr = false; // Контролировать указанные турели? || Control designated turrets?
        bool Gravity = true; // Учитывать гравитацию? Check for gravity?
        static double ShellSpeed = 400;
        static double TRange = 800;
        static bool OwnDamage = false;
        static double MaxCamDist = 10;
        static int BlindCheck_Start = 5; // Старт с какого числа блоков проверять || From what count of blocks start check
        //
        // Настройки фильтра
        // Filter settings
        static double minsizeS = 10; // minimal size for small grid (минимальный размер для малой сетки)
        static double minsizeL = 20; // minimal size for large grid (минимальный размер для большой сетки)
        static int mingrid = 0; // 0 - all (все), 1 - only small grid (только малая сетка), 2 - only large grid (только большая сетка)
        //
        // Поиск утерянных целей
        // Lost targets search
        static int count = 1; // Общее число попыток || Total repetitions count
        int rand_ray = 18; // Случайный угол между лучами || Random angle between rays
        int ray_parties = 3; // Число попыток поиска цели || Count of tries to catch lost target
        //
        /****************************************************************************************************/
bool update = true;
public string IsTargetRelation(MyRelationsBetweenPlayerAndBlock relation)
{
if (relation == MyRelationsBetweenPlayerAndBlock.Enemies) return enemy_target.ToString();
if (relation == MyRelationsBetweenPlayerAndBlock.Friends) return friendly_target.ToString();
if (relation == MyRelationsBetweenPlayerAndBlock.Owner) return owner_target.ToString();
if (relation == MyRelationsBetweenPlayerAndBlock.Neutral) return neutral_target.ToString();
return unknown_target.ToString();
}
public string IsAIScanning(bool Mode)
{
if (AIBlock_move != null && AIBlock_offensive != null)
{
if (!ENG)
{
if (Mode) return "Сканирование целей блоком ИИ включено, до след. скана: " + ai_scan_freq_code;
else return "Сканирование целей блоком ИИ выключено";
}
else
{
if (Mode) return "Targets search with AI block enabled with, before next scan: " + ai_scan_freq_code;
else return "";
}
}
return "";
}
public string IsMode(bool Mode)
{
if (!ENG)
{
if (Mode == true)
{
if (!Scan)
{
return "Активный режим";
}
else
{
return "Активный режим, идёт захват цели";
}
}
if (Mode == false)
{
if (!Scan)
{
return "Пассивный режим";
}
else
{
return "Пассивный режим, идёт захват цели";
}
}
}
if (ENG)
{
if (Mode == true)
{
if (!Scan)
{
return "Active search mode";
}
else
{
return "Active search mode, scanning...";
}
}
if (Mode == false)
{
if (!Scan)
{
return "Passive search mode";
}
else
{
return "Passive search mode, scanning...";
}
}
}

return "";
}
public string IsMissiles(int Mode)
{
if (!ENG)
{
if (Mode == 0)
{
return "Выбраны ракеты " + MissileName;
}
if (Mode == 1)
{
return "Выбраны ракеты " + Missile1Name;
}
if (Mode == 2)
{
return "Выбраны ракеты " + Missile2Name;
}
if (Mode == 3)
{
return "Выбраны ракеты " + Missile3Name;
}
}
else
{
if (Mode == 0)
{
return "Selected Missiles " + MissileName;
}
if (Mode == 1)
{
return "Selected Missiles " + Missile1Name;
}
if (Mode == 2)
{
return "Selected Missiles " + Missile2Name;
}
if (Mode == 3)
{
return "Selected Missiles " + Missile3Name;
}
}
return "";
}
public string IsCharge(bool Mode)
{
if (!ENG)
{
if (Cameras.Count > 0)
{
double Charge = 0;
double MinCharge = double.MaxValue;
foreach (IMyCameraBlock cam in Cameras)
{
if (MinCharge > cam.AvailableScanRange) MinCharge = cam.AvailableScanRange;
Charge += cam.AvailableScanRange;
}
return ("Заряд камер (срзнач): " + (float)(Charge / Cameras.Count) + "; Мин. заряд: " + (float)MinCharge);
}
if (Mode && Cameras.Count <= 0)
{
return ("Камер нет, РЛС работает в пассивном режиме");
}
}
if (ENG)
{
if (Cameras.Count > 0)
{
double Charge = 0;
double MinCharge = double.MaxValue;
foreach (IMyCameraBlock cam in Cameras)
{
if (MinCharge > cam.AvailableScanRange) MinCharge = cam.AvailableScanRange;
Charge += cam.AvailableScanRange;
}
return ("Charge of cameras (med. val.): " + (float)(Charge / Cameras.Count) + "; Min. charge:" + (float)MinCharge);
}
if (Mode && Cameras.Count <= 0)
{
return ("No cameras, RADAR in passive scanning mode");
}
}
return "<---------------->";
}
Random rand = new Random();
int ai_scan_freq_code = 0;
public string IsAMode(bool small, bool large)
{
if (!ENG)
{
if (small || large)
{
return "Включён";
}
else return "Выключен";
}
else
{
if (small || large)
{
return "Online";
}
else return "Offline";
}
}
public bool IsShipControl(IMyTerminalBlock block)
{
IMyShipController rc = block as IMyShipController;
if (rc != null) return true;
return false;
}
public bool IsLCD(IMyTerminalBlock block)
{
IMyTextPanel lcd = block as IMyTextPanel;
if (lcd != null) return true;
return false;
}
IMyProjector Proj1;
IMyProjector Proj2;
IMyRadioAntenna ANT;
IMyBlockGroup Sensorsg;
IMyProgrammableBlock RPB;
IMyProgrammableBlock WPB;
IMyProgrammableBlock EPB;
IMyProgrammableBlock R4PB;
IMyOffensiveCombatBlock AIBlock_offensive;
IMyFlightMovementBlock AIBlock_move;
IMySoundBlock dyn;

IMyTimerBlock Timer_Alert;
IMyTimerBlock RT;
IMyTimerBlock WT;
IMyTimerBlock ET;
IMyTimerBlock R4T;
IMyBlockGroup Turretg;
IMyBlockGroup CamerasG;
IMyBlockGroup gBats;
IMyBlockGroup gTanks;
StringBuilder sb = new StringBuilder();
List<string> RLSArray = new List<string>();
MyDetectedEntityInfo TargetedEnt = new MyDetectedEntityInfo();
List<Vector3D> TrTargets = new List<Vector3D>();
IMyBroadcastListener listener;
List<IMyLargeTurretBase> ControlledTurrets = new List<IMyLargeTurretBase>();
List<string> times = new List<string>();
List<string> targettimes = new List<string>();
List<IMyTerminalBlock> RCs = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> LCDs = new List<IMyTerminalBlock>();
List<MyDetectedEntityInfo> TargetsBroad = new List<MyDetectedEntityInfo>();
List<MyDetectedEntityInfo> LTargets = new List<MyDetectedEntityInfo>();
List<IMyBatteryBlock> Bats = new List<IMyBatteryBlock>();
List<IMyGasTank> Tanks = new List<IMyGasTank>();
List<IMyProgrammableBlock> DPBs = new List<IMyProgrammableBlock>();
List<IMyProgrammableBlock> CPBs = new List<IMyProgrammableBlock>();
List<IMyCameraBlock> usedcams = new List<IMyCameraBlock>();
List<IMyLargeTurretBase> IRTurrets = new List<IMyLargeTurretBase>();
List<IMyLargeTurretBase> RTurrets = new List<IMyLargeTurretBase>();
List<MyDetectedEntityInfo> ATTargets = new List<MyDetectedEntityInfo>();
List<MyDetectedEntityInfo> Targets4 = new List<MyDetectedEntityInfo>();
List<MyDetectedEntityInfo> Targets = new List<MyDetectedEntityInfo>();
List<IMySensorBlock> Sensors = new List<IMySensorBlock>();
List<IMyCameraBlock> Cameras = new List<IMyCameraBlock>();
List<IMyCameraBlock> TCameras = new List<IMyCameraBlock>();
List<IMyLargeTurretBase> Turrets = new List<IMyLargeTurretBase>();
List<MyDetectedEntityInfo> Targets0 = new List<MyDetectedEntityInfo>();
bool Checked = false;
bool finished = false;
bool ActiveMode = false;
bool HasAntenna = false;
bool Scan = false;
bool start = false;
bool offset = false;
bool lcd1 = false;
bool lcd2 = false;
bool lcd3 = false;
bool lcd4 = false;
bool lcd5 = false;
bool lcd6 = false;
bool lcd7 = false;
int MissileType = 0; // 0 - Р-4, 1 - рдав, 2 - вип, 3 - изи
int Frequency1 = 0;
int SelectedTarget = 0;
int sound_scan_freq = 3;
int sound_scan_freq_1 = 0;
bool HasBeenAlert = false;
Program()
{
if (Auto)
{
Runtime.UpdateFrequency |= UpdateFrequency.Update10;
}
if (BroadcastEn)
{
listener = IGC.RegisterBroadcastListener(BTAG);
listener.SetMessageCallback(BTAG);
}
}
void Main(String args)
{
ai_scan_freq_code--;
if (!update) Checked = true;
if (!Checked)
{
update = false;
if (StatusEnabled)
{
Tanks.Clear();
Bats.Clear();
}
List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
GridTerminalSystem.GetBlocks(blocks);
CPBs.Clear();
LCDs.Clear();
foreach (IMyTerminalBlock block in blocks)
{
if (block.CustomName.Contains(LCDName) && IsLCD(block)) LCDs.Add(block as IMyTextPanel);
if (block.CustomName.Contains(Connected_PB_tag)) CPBs.Add(block as IMyProgrammableBlock);
}
blocks.Clear();
ControlledTurrets.Clear();
IRTurrets.Clear();
Sensors.Clear();
Cameras.Clear();
TCameras.Clear();
Turrets.Clear();
RTurrets.Clear();
HasAntenna = false;
List<IMyRadioAntenna> ants = new List<IMyRadioAntenna>();
GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(ants);
if (ants.Count > 0) HasAntenna = true;
List<IMyLaserAntenna> ants1 = new List<IMyLaserAntenna>();
GridTerminalSystem.GetBlocksOfType<IMyLaserAntenna>(ants1);
if (ants1.Count > 0) HasAntenna = true;
Proj1 = GridTerminalSystem.GetBlockWithName(Proj1n) as IMyProjector;
Proj2 = GridTerminalSystem.GetBlockWithName(Proj2n) as IMyProjector;
ANT = GridTerminalSystem.GetBlockWithName(AntName) as IMyRadioAntenna;
Timer_Alert = GridTerminalSystem.GetBlockWithName(TalertName) as IMyTimerBlock;
GridTerminalSystem.SearchBlocksOfName(RemName, RCs, IsShipControl);
AIBlock_offensive = GridTerminalSystem.GetBlockWithName(AIBlockName) as IMyOffensiveCombatBlock;
AIBlock_move = GridTerminalSystem.GetBlockWithName(AIBlockName2) as IMyFlightMovementBlock;
dyn = GridTerminalSystem.GetBlockWithName(DinName) as IMySoundBlock;
if (AIBlock_offensive != null) AIBlock_offensive.Enabled = true;
if (AIBlock_move != null) AIBlock_move.Enabled = false;
if (!UseTimersInstead)
{
RPB = GridTerminalSystem.GetBlockWithName(MissileRdavsPBName) as IMyProgrammableBlock;
R4PB = GridTerminalSystem.GetBlockWithName(MissileR4PBName) as IMyProgrammableBlock;
WPB = GridTerminalSystem.GetBlockWithName(MissileWhipsPBName) as IMyProgrammableBlock;
EPB = GridTerminalSystem.GetBlockWithName(MissileLidatPBName) as IMyProgrammableBlock;
}
if (UseTimersInstead)
{
RT = GridTerminalSystem.GetBlockWithName(MissileRdavsPBName) as IMyTimerBlock;
WT = GridTerminalSystem.GetBlockWithName(MissileWhipsPBName) as IMyTimerBlock;
ET = GridTerminalSystem.GetBlockWithName(MissileLidatPBName) as IMyTimerBlock;
R4T = GridTerminalSystem.GetBlockWithName(MissileR4PBName) as IMyTimerBlock;
}
Turretg = GridTerminalSystem.GetBlockGroupWithName(TurretsName);
CamerasG = GridTerminalSystem.GetBlockGroupWithName(CamName);
Sensorsg = GridTerminalSystem.GetBlockGroupWithName(SensName);
if (StatusEnabled)
{
gBats = GridTerminalSystem.GetBlockGroupWithName(BatsName);
gTanks = GridTerminalSystem.GetBlockGroupWithName(TanksName);
}
if ((Debug || BroadcastEn) && ANT != null) ANT.Enabled = true;
if (CamerasG != null) CamerasG.GetBlocksOfType(Cameras);
if (Sensorsg != null) Sensorsg.GetBlocksOfType(Sensors);
if (Turretg != null) Turretg.GetBlocksOfType(Turrets);
Turretg = GridTerminalSystem.GetBlockGroupWithName(TurretsRName);
if (Turretg != null) Turretg.GetBlocksOfType(RTurrets);
Turretg = GridTerminalSystem.GetBlockGroupWithName(ControlledTurretsN);
if (Turretg != null) Turretg.GetBlocksOfType(ControlledTurrets);
Turretg = GridTerminalSystem.GetBlockGroupWithName(TurretsIRName);
if (Turretg != null) Turretg.GetBlocksOfType(IRTurrets);
if (AllTurrets)
{
Turrets.Clear();
GridTerminalSystem.GetBlocksOfType(Turrets);
}
CamerasG = GridTerminalSystem.GetBlockGroupWithName(TurretCamName);
if (CamerasG != null) CamerasG.GetBlocksOfType(TCameras);
if (StatusEnabled) { if (gBats != null) gBats.GetBlocksOfType(Bats); if (gTanks != null) gTanks.GetBlocksOfType(Tanks); }
foreach (IMyLargeTurretBase turret in Turrets)
{
if (!turret.Enabled)
{
Turrets.Remove(turret);
break;
}
}
foreach (IMyLargeTurretBase turret in RTurrets)
{
if (!turret.Enabled)
{
RTurrets.Remove(turret);
break;
}
}
foreach (IMyCameraBlock turret in Cameras)
{
if (!turret.Enabled)
{
Cameras.Remove(turret);
break;
}
}
foreach (IMySensorBlock turret in Sensors)
{
if (!turret.Enabled)
{
Sensors.Remove(turret);
break;
}
}
if (RCs.Count > 0)
{
if (LCDs.Count <= 0)
{
if (!ENG)
{
Echo("Дисплей РЛС не обнаружен, РЛС работает...");
}
else
{
Echo("Display of RADAR not found, RADAR is working...");
}
}
else
{
foreach (IMyTextPanel lcd in LCDs)
{
lcd.Enabled = true;
}
}
lcd1 = false;
lcd2 = false;
lcd3 = false;
lcd4 = false;
lcd5 = false;
lcd6 = false;
lcd7 = false;
foreach (IMyTextPanel lcd in LCDs)
{
lcd.Enabled = true;
if (lcd.CustomName == LCDName + "1") lcd1 = true;
if (lcd.CustomName == LCDName + "2") lcd2 = true;
if (lcd.CustomName == LCDName + "3") lcd3 = true;
if (lcd.CustomName == LCDName + "4") lcd4 = true;
if (lcd.CustomName == LCDName + "5") lcd5 = true;
if (lcd.CustomName == LCDName + "6") lcd6 = true;
if (lcd.CustomName == LCDName + "7") lcd7 = true;
}
if (!lcd1)
{
if (!ENG) Echo("Дисплей информации о цели не обнаружен, РЛС работает...");
else Echo("Display of RADAR info not found, RADAR is working...");
}
if (!lcd2)
{
if (!ENG) Echo("Дисплей вывода предупреждений не обнаружен, РЛС работает...");
else Echo("Display of RADAR warnings not found, RADAR is working...");
}
if (!lcd3)
{
if (!ENG) Echo("Дисплей статуса ракет не обнаружен, РЛС работает...");
else Echo("Display of missile status not found, RADAR is working...");
}
if (!lcd5 && StatusEnabled)
{
if (!ENG) Echo("Дисплей статуса готовности ракет не обнаружен, РЛС работает...");
else Echo("Display of ready missiles status not found, RADAR is working...");
}
if (ANT == null)
{
if (!ENG && (!HasAntenna && BroadcastEn)) Echo("Передатчик не обнаружен, радиосвязь невозможна, РЛС работает...");
if (ENG && (!HasAntenna && BroadcastEn)) Echo("Antenna not found, broadcasting impossible, RADAR is working...");
}
if (Turrets.Count <= 0)
{
if (!ENG) Echo("Турели не обнаружены, РЛС работает...");
else Echo("Turrets not found, RADAR is working...");
}
if (Cameras.Count <= 0)
{
if (!ENG) Echo("Камеры не обнаружены, РЛС работает...");
else Echo("Cameras not found, RADAR is working...");
}
if (Sensors.Count <= 0)
{
if (!ENG) Echo("Сенсоры не обнаружены, РЛС работает...");
else Echo("Sensors not found, RADAR is working...");
}
if (RPB != null) RPB.Enabled = true;
if (ControlledTurrets.Count <= 0 && ControlTurr)
{
if (!ENG) Echo("Турели для управления не найдены, РЛС работает...");
else Echo("Turrets for control not found, RADAR is working...");
}
if (Cameras.Count <= 0 && Turrets.Count <= 0 && Sensors.Count <= 0)
{
if (!ENG)
{
Echo("Ни камеры ни турели ни сенсоры не обнаружены, каким образом я должен работать?");
Checked = false;
}
else
{
Echo("Cameras, turrets and sensors not found, how i must work?");
Checked = false;
}
}
else
Checked = true;
}
else
{
if (!ENG)
{
Echo("Блок ДУ не обнаружен, работа невозможна");
Checked = false;
}
else
{
Echo("Remote Control not found, RADAR offline");
Checked = false;
}
}

}
if (RPB != null || WPB != null || EPB != null || R4PB != null || RT != null || R4T != null || WT != null || ET != null)
{
if ((RPB == null || RT == null) && MissileType == 1) MissileType++;
if ((WPB == null || WT == null) && MissileType == 2) MissileType++;
if ((EPB == null || ET == null) && MissileType == 3) MissileType = 0;
}
if (Checked)
{
if (!start)
{
if (StatusEnabled)
{
DPBs.Clear();
if (EmergencyOff)
{
if (RdavEn && RPB != null)
{
DPBs.Add(RPB);
}
if (WhipsEn && WPB != null)
{
DPBs.Add(WPB);
}
if (AlysiusEn && EPB != null)
{
DPBs.Add(EPB);
}
if (R4En && R4PB != null)
{
DPBs.Add(R4PB);
}
}
}

if (RPB != null || RT != null)
{
MissileType = 1;
}
else
{
if (WPB != null || WT != null)
{
MissileType = 2;
}
else
{
if (EPB != null || ET != null)
{
MissileType = 3;
}
else
{
MissileType = 0;
}
}
}
Frequency1 = Frequency;
for (int i = Cameras.Count - 1; i >= 0; i--)
{
IMyCameraBlock cam = Cameras[i];
cam.EnableRaycast = true;
}
if (ENG)
{
Name = EName;
TargetsD = ETargetsD;
TargetsN = ETargetsN;
Mode = EMode;
TargetedT = ETargetedT;
TNam = ETNam;
TSpe = ETSpe;
TDis = ETDis;
TRel = ETRel;
Alar = EAlar;
AMode = EAmode;
TurrO = ETurrO;
}
for (int i = 0; i < TCameras.Count; i++)
{
IMyCameraBlock cam = TCameras[i];
cam.EnableRaycast = true;
}
start = true;
}
//

// Поиск целей с помощью блока ИИ
if (AIBlock_offensive != null && AIBlock_move != null && AIBlock_offensive.IsFunctional && AIBlock_move.IsFunctional)
{
AIBlock_move.MinimalAltitude = 0;
AIBlock_move.PrecisionMode = false;
AIBlock_move.SpeedLimit = 100;
AIBlock_move.AlignToPGravity = false;
AIBlock_move.CollisionAvoidance = false;

AIBlock_offensive.UpdateTargetInterval = ai_scan_freq;
AIBlock_offensive.SearchEnemyComponent.TargetingLockOptions = MyGridTargetingRelationFiltering.Enemy;
AIBlock_offensive.SelectedAttackPattern = 3;
AIBlock_offensive.SetValue<long>("OffensiveCombatIntercept_GuidanceType", 0);
AIBlock_offensive.SetValueBool("OffensiveCombatIntercept_OverrideCollisionAvoidance", true);


if (ai_scan_freq_code < 1 && AI_Scanning)
{
if (AIBlock_move.Enabled)
{
if (Debug) Echo("AI_block_move_en");
if (AIBlock_move.CurrentWaypoint != null)
{
if (Debug) Echo("AI_get_guidance");
IMyAutopilotWaypoint waypoint = AIBlock_move.CurrentWaypoint;
Vector3D Pos = new Vector3D(waypoint.Matrix.GetRow(3).X, waypoint.Matrix.GetRow(3).Y, waypoint.Matrix.GetRow(3).Z);
//MyDetectedEntityInfo find_target = new MyDetectedEntityInfo(-1, "AI_Found_Ent", MyDetectedEntityType.Unknown, Pos, new MatrixD(), Pos, MyRelationsBetweenPlayerAndBlock.Enemies, new BoundingBoxD(), 0);
TrTargets.Add(Pos);
AIBlock_move.Enabled = false;
AIBlock_move.ApplyAction("ActivateBehavior_Off", null);
ai_scan_freq_code = ai_scan_freq;
}
AIBlock_offensive.Enabled = true;
AIBlock_offensive.ApplyAction("ActivateBehavior_On", null);
}
else
{
{
if (Debug) Echo("AI_attack_block_en");
AIBlock_move.Enabled = true;
AIBlock_move.ApplyAction("ActivateBehavior_On", null);
AIBlock_offensive.ApplyAction("ActivateBehavior_Off", null);
}
}
}
else
{
AIBlock_move.Enabled = false;
AIBlock_move.ApplyAction("ActivateBehavior_Off", null);
AIBlock_offensive.Enabled = false;
AIBlock_offensive.ApplyAction("ActivateBehavior_Off", null);
}

}
//

if (!finished)
{














float Angle = 0;
float Pitch = 0;
// скан переданных целей, которые содержат только позицию
if (TrTargets.Count > 0 && Cameras.Count > 0)
{
for (int i1 = 0; i1 < TrTargets.Count; i1++)
{
Vector3D target = TrTargets[i1];
for (int i = Cameras.Count - 1; i >= 0; i--)
{
IMyCameraBlock cam = Cameras[i];
if (!usedcams.Contains(cam) && cam.CanScan(target))
{
MyDetectedEntityInfo n;
n = cam.Raycast(target);
if (n.Name != "Планета" && n.Name != "Planet" && n.Name != Me.CubeGrid.CustomName && n.Type.ToString() != "Planet" && !n.IsEmpty() && ((n.Relationship.ToString() != "Owner" && n.Relationship.ToString() != "Friendly") || !FriendlyOff))
{
usedcams.Add(cam);
Targets.Add(n);
break;
}
else
{
continue;
}
}
else
{
continue;
}
}
}
}
TrTargets.Clear();
// обновление целей
if (Cameras.Count > 0 && TargetUpd)
{
if (LTargets.Count > 0)
{
for (int i = 0; i < LTargets.Count; i++)
{
MyDetectedEntityInfo target = LTargets[i];
Targets.Add(target);
}
LTargets.Clear();
}
if (Targets.Count > 0)
{
for (int i1 = 0; i1 < Targets.Count; i1++)
{
MyDetectedEntityInfo target = Targets[i1];
bool Contains = false;
foreach (MyDetectedEntityInfo target1 in Targets4)
{
if (target.EntityId == target1.EntityId)
{
Contains = true;
break;
}
}
if (!Contains)
{
bool exist = false;
Vector3D vector;
Vector3D dir = target.Position - Me.CubeGrid.GetPosition();
MyDetectedEntityInfo n = new MyDetectedEntityInfo();
for (int cam = Cameras.Count - 1; cam >= 0; cam--)
{
if (!offset || target.HitPosition == null) vector = target.Position + (Vector3D)target.Velocity / Q;
else vector = (Vector3D)target.HitPosition + Vector3D.Normalize((Vector3D)target.HitPosition - Cameras[cam].GetPosition()) * 10 + target.Velocity / Q;
if ((Cameras[cam].CanScan(vector) && dir.Length() <= MaxRange && Targets.Count != Targets4.Count) && ((target.Relationship.ToString() != "Owner" && target.Relationship.ToString() != "Friendly") || !FriendlyOff))
{
n = Cameras[cam].Raycast(vector);
if (n.EntityId == Me.CubeGrid.EntityId) continue;
usedcams.Add(Cameras[cam]);
foreach (MyDetectedEntityInfo targett in Targets)
{
if (n.EntityId == targett.EntityId)
{
exist = true;
break;
}
}
break;
}
}
if (exist)
{
Targets4.Add(n);
continue;
}
else
{
if (n.IsEmpty())
{
usedcams.Clear();
for (int count1 = count; count1 >= 0; count1--)
{
Random rand = new Random();
MyDetectedEntityInfo n1 = new MyDetectedEntityInfo();
for (int i = Cameras.Count - 1; i >= 0; i--)
{
IMyCameraBlock cam1 = Cameras[i];
Vector3D vector1 = target.Position;
if (cam1.CanScan(vector1) && !usedcams.Contains(cam1))
{
n1 = cam1.Raycast(vector1);
usedcams.Add(cam1);
break;
}
}
for (int z = ray_parties; z > 0; z--)
{
for (int i = Cameras.Count - 1; i >= 0; i--)
{
if (!n1.IsEmpty()) break;
IMyCameraBlock cam1 = Cameras[i];
Vector3D vector1 = new Vector3D(target.Position.X, target.Position.Y, target.Position.Z + rand.Next(-rand_ray, rand_ray));
if (cam1.CanScan(vector1) && !usedcams.Contains(cam1))
{
n1 = cam1.Raycast(vector1);
usedcams.Add(cam1);
break;
}
}
for (int i = Cameras.Count - 1; i >= 0; i--)
{
if (!n1.IsEmpty()) break;
IMyCameraBlock cam1 = Cameras[i];
Vector3D vector1 = new Vector3D(target.Position.X, target.Position.Y + rand.Next(-rand_ray, rand_ray), target.Position.Z);
if (cam1.CanScan(vector1) && !usedcams.Contains(cam1))
{
n1 = cam1.Raycast(vector1);
usedcams.Add(cam1);
break;
}
}
for (int i = Cameras.Count - 1; i >= 0; i--)
{
if (!n1.IsEmpty()) break;
IMyCameraBlock cam1 = Cameras[i];
Vector3D vector1 = new Vector3D(target.Position.X + rand.Next(-rand_ray, rand_ray), target.Position.Y, target.Position.Z);
if (cam1.CanScan(vector1) && !usedcams.Contains(cam1))
{
n1 = cam1.Raycast(vector1);
usedcams.Add(cam1);
break;
}
}
for (int i = Cameras.Count - 1; i >= 0; i--)
{
if (!n1.IsEmpty()) break;
IMyCameraBlock cam1 = Cameras[i];
Vector3D vector1 = new Vector3D(target.Position.X + rand.Next(-rand_ray, rand_ray), target.Position.Y + rand.Next(-rand_ray, rand_ray), target.Position.Z + rand.Next(-rand_ray, rand_ray));
if (cam1.CanScan(vector1) && !usedcams.Contains(cam1))
{
n1 = cam1.Raycast(vector1);
usedcams.Add(cam1);
break;
}
}
}
exist = false;
if (!n1.IsEmpty())
{
for (int i = 0; i < Targets.Count; i++)
{
MyDetectedEntityInfo targett = Targets[i];
if (n1.EntityId == targett.EntityId)
{
exist = true;
break;
}
}
if (exist)
{
Targets4.Add(n1);
break;
}
}
else
if (dyn != null && dyn.IsFunctional) { dyn.LoopPeriod = 3; dyn.Play(); }
}
}
//else break;
}
}
}
}
}
else
{
Targets.Clear();
}
if (Targets4.Count > 0)
{
Targets.Clear();
foreach (MyDetectedEntityInfo n in Targets4)
{
Targets.Add(n);
}
}
else
{
Targets.Clear();
}
// проверка переданных целей
if (TargetsBroad.Count > 0)
{
for (int i = 0; i < TargetsBroad.Count; i++)
{
MyDetectedEntityInfo n = TargetsBroad[i];
if (Targets.Count <= 0) { Targets.Add(n); AddAuto(n); }

if (Targets.Count > 0)
{
bool exist = false;
foreach (MyDetectedEntityInfo v in Targets)
{
if (n.EntityId == v.EntityId)
{
exist = true;
break;
}
}
if (!exist)
{
Targets.Add(n);
AddAuto(n);
}
}
}
}





for (int i = 0; i < RCs.Count; i++)
{
IMyShipController RC = (IMyShipController)RCs[i];
if (RC.IsSameConstructAs(Me))
{


// Скан
if (Scan && Cameras.Count > 0)
{
sound_scan_freq_1--;
if (dyn != null && dyn.IsFunctional && sound_scan_freq_1 < 1) { sound_scan_freq_1 = sound_scan_freq; dyn.Play(); }
for (int ca = Cameras.Count - 1; ca >= 0; ca--)
{
IMyCameraBlock cam = Cameras[ca];
if (cam.AvailableScanRange > MaxRange * 3 && !usedcams.Contains(cam))
{
if (Proj1 != null && Proj2 != null)
{
Proj1.Enabled = false;
Proj2.Enabled = true;
}
MyDetectedEntityInfo d;
Vector3D vector = RC.GetPosition() + RC.WorldMatrix.Forward * MaxRange;
if (Turrets.Count > 0 && UseTurretsForScan)
{
IMyLargeTurretBase Cturret = Turrets[0];
foreach (IMyLargeTurretBase turret in Turrets)
{
if (turret.IsUnderControl)
{

Vector3D turretDirection;
Vector3D.CreateFromAzimuthAndElevation(turret.Azimuth, turret.Elevation, out turretDirection);
turretDirection = Vector3D.TransformNormal(turretDirection, turret.WorldMatrix);

// Преобразуем вектор из локальной системы координат турели в мировую
vector = turret.WorldMatrix.Translation + Vector3D.Normalize(turretDirection) * MaxRange;
break;
}
}
}
if (cam.CanScan(vector)) d = cam.Raycast(vector);
else continue;
if (!d.IsEmpty() && d.Name != "Планета" && d.Name != "Planet" && d.Name != cam.CubeGrid.CustomName && d.Type.ToString() != "Planet" && ((d.Relationship.ToString() != "Owner" && d.Relationship.ToString() != "Friendly") || !FriendlyOff))
{
if ((d.BoundingBox.Size.Length() < minsizeS && d.Type == MyDetectedEntityType.SmallGrid) || (d.BoundingBox.Size.Length() < minsizeL && d.Type == MyDetectedEntityType.LargeGrid) || (d.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (d.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) continue;
if (Targets.Count <= 0)
{
if (ScanLaunch)
{
AddAuto(d);
}
Targets.Add(d);
Scan = false;
break;
}
if (Targets.Count > 0 && !Targets.Contains(d))
{
bool exist = false;
for (int i1 = 0; i1 < Targets.Count; i1++)
{
MyDetectedEntityInfo n = Targets[i1];
if (n.EntityId == d.EntityId) exist = true;
}
if (!exist)
{
if (ScanLaunch)
{
AddAuto(d);
}
Targets.Add(d);
Scan = false;
break;
}
}
if (offset && Targets.Count > 0)
{
for (int i1 = 0; i1 < Targets.Count; i1++)
{
if (Targets[i1].EntityId == d.EntityId) { Targets[i1] = d; break; }
}
Scan = false;
break;
}
}
}
}
}
if (!Scan)
{
if (Proj1 != null && Proj2 != null)
{
Proj1.Enabled = true;
Proj2.Enabled = false;
}
}
break;
}
}

// Активный режим поиска целей
if (Cameras.Count > 0 && ActiveMode)
{
//
Frequency--;
int q = Cameras.Count;
if (Frequency <= 0)
{
while (!finished)
{
q--;
Echo(q.ToString());
IMyCameraBlock n = Cameras[q];
if (!usedcams.Contains(Cameras[q]))
{
MyDetectedEntityInfo target = new MyDetectedEntityInfo();
if (!AdvAct)
{
target = n.Raycast(MaxRange, 0, 0);
}
else
{
Angle = rand.Next(-45, 45);
Pitch = rand.Next(-45, 45);
target = n.Raycast(MaxRange, Angle, Pitch);
}
if (!target.IsEmpty() && !Targets.Contains(target) && Targets.Count > 0)
{
for (int i1 = Targets.Count - 1; i1 >= 0; i1--)
{
MyDetectedEntityInfo i = Targets[i1];
if ((target.BoundingBox.Size.Length() < minsizeS && target.Type == MyDetectedEntityType.SmallGrid) || (target.BoundingBox.Size.Length() < minsizeL && target.Type == MyDetectedEntityType.LargeGrid) || (target.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (target.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) break;
if (i.EntityId != target.EntityId && target.Name != "Планета" && target.Name != Me.CubeGrid.CustomName && target.Name != "Planet" && target.Type.ToString() != "Planet")
{
if (i1 == 0)
{
Targets.Add(target);
AddAuto(target);
break;
}
}
else
{
break;
}

}

}
if (Targets.Count == 0)
{
if (!target.IsEmpty() && target.Name != "Планета" && target.Name != Me.CubeGrid.CustomName && target.Name != "Planet" && target.Type.ToString() != "Planet")
{
Targets.Add(target);
AddAuto(target);
}

}
if (q == 0)
{
if (Sensors.Count <= 0 && Turrets.Count <= 0)
{
finished = true;
}
else
{
break;
}
}
}
}
Frequency = Frequency1;
}
else
{
if (Sensors.Count <= 0 && Turrets.Count <= 0)
{
finished = true;
}
}
}
//
//
// Получение целей с турелей Targets0
if (Turrets.Count > 0)
{

int i = Turrets.Count;
while (!finished)
{
i--;
if (i < 0) { if (Sensors.Count <= 0) finished = true; break; }
Echo(i.ToString());
IMyLargeTurretBase n = Turrets[i];
if (!n.GetTargetedEntity().IsEmpty())
{
if (Targets.Count > 0)
{
MyDetectedEntityInfo target = n.GetTargetedEntity();
bool exist = false;
for (int i1 = 0; i1 < Targets.Count; i1++)
{
if (Targets[i1].EntityId == target.EntityId) { exist = true; break; }
}
if (exist)
{
if ((target.BoundingBox.Size.Length() < minsizeS && target.Type == MyDetectedEntityType.SmallGrid) || (target.BoundingBox.Size.Length() < minsizeL && target.Type == MyDetectedEntityType.LargeGrid) || (target.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (target.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) break;
if (!target.IsEmpty())
{
exist = false;
for (int i1 = 0; i1 < Targets0.Count; i1++)
{
if (Targets0[i1].EntityId == target.EntityId) { exist = true; break; }
}
if (!exist) Targets0.Add(target);
AddAuto(target);
continue;
}
}
else
{
if ((target.BoundingBox.Size.Length() < minsizeS && target.Type == MyDetectedEntityType.SmallGrid) || (target.BoundingBox.Size.Length() < minsizeL && target.Type == MyDetectedEntityType.LargeGrid) || (target.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (target.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) break;
Targets.Add(target);
Targets0.Add(target);
AddAuto(target);
continue;
}
}
if (Targets.Count <= 0)
{
MyDetectedEntityInfo target = n.GetTargetedEntity();
if ((target.BoundingBox.Size.Length() < minsizeS && target.Type == MyDetectedEntityType.SmallGrid) || (target.BoundingBox.Size.Length() < minsizeL && target.Type == MyDetectedEntityType.LargeGrid) || (target.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (target.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) continue;
if (target.IsEmpty() == false)
{
Targets.Add(target);
Targets0.Add(target);
AddAuto(target);
}

}
}
if (i == 0)
{
if (Sensors.Count <= 0)
{
finished = true;
break;
}
else
{
break;
}
}
}
}
//
// Получение целей с сенсоров Targets0
if (Sensors.Count > 0)
{
int i1 = Sensors.Count;
while (!finished)
{
i1--;
if (i1 < 0) { finished = true; break; }
List<MyDetectedEntityInfo> targets = new List<MyDetectedEntityInfo>();
Echo(i1.ToString());
IMySensorBlock n = Sensors[i1];
n.DetectedEntities(targets);
for (int i = 0; i < targets.Count; i++)
{
MyDetectedEntityInfo target = targets[i];
if (!Targets.Contains(target) && Targets.Count > 0)
{
bool exist = false;
foreach (MyDetectedEntityInfo z in Targets)
{
if (z.EntityId != target.EntityId && targets.Count > 0) { exist = true; break; }
}
if (!exist)
{
if ((target.BoundingBox.Size.Length() < minsizeS && target.Type == MyDetectedEntityType.SmallGrid) || (target.BoundingBox.Size.Length() < minsizeL && target.Type == MyDetectedEntityType.LargeGrid) || (target.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (target.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) continue;
Targets.Add(target);
Targets0.Add(target);
AddAuto(target);
break;
}
}
if (Targets.Count == 0)
{
if (target.IsEmpty() == false)
{
if ((target.BoundingBox.Size.Length() < minsizeS && target.Type == MyDetectedEntityType.SmallGrid) || (target.BoundingBox.Size.Length() < minsizeL && target.Type == MyDetectedEntityType.LargeGrid) || (target.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (target.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) continue;
Targets.Add(target);
Targets0.Add(target);
AddAuto(target);
}

}
}
if (i1 == 0)
{
finished = true;
}
}
}
//
if (Sensors.Count <= 0 && Turrets.Count <= 0 && !ActiveMode)
{
finished = true;
}
}
//
if (finished)
{
if (Targets.Count > 0)
{
if (!HasBeenAlert)
{
HasBeenAlert = true;
if (Timer_Alert != null && Timer_Alert.IsFunctional) Timer_Alert.Trigger();
}
}
else
{
HasBeenAlert = false;
}





if (!MouseSelector && Targets.Count > 0)
{
if (SelectedTarget < 0)
{
SelectedTarget = 0;
}
if (SelectedTarget > Targets.Count - 1)
{
SelectedTarget = 0;
}
if (TargetedEnt.IsEmpty()) TargetedEnt = Targets[SelectedTarget];
}
foreach (IMyShipController RC in RCs)
{
if (!RC.IsSameConstructAs(Me)) continue;
if (Targets.Count > 0)
{
if (MouseSelector)
{
if (Targets.Count > 1)
{
for (int z = Targets.Count - 1; z >= 1; z--)
{
Vector3D Direction2 = Targets[z].Position - RC.CubeGrid.GetPosition();
Vector3D NeedFDirection2 = RC.WorldMatrix.Forward - Vector3D.Normalize(Direction2);
Vector3D Direction21 = Targets[z - 1].Position - RC.CubeGrid.GetPosition();
Vector3D NeedFDirection21 = RC.WorldMatrix.Forward - Vector3D.Normalize(Direction21);
for (int q = Targets.Count - 1; q >= 0; q--)
{
if (NeedFDirection2.Length() <= NeedFDirection21.Length())
{
TargetedEnt = Targets[z];
}
else
{
TargetedEnt = Targets[z - 1];
}
}
}
}
if (Targets.Count == 1)
{
TargetedEnt = Targets[0];
}
}
if (!MouseSelector)
{
for (int i = 0; i < Targets.Count; i++)
{
MyDetectedEntityInfo n = Targets[i];
if (n.EntityId == TargetedEnt.EntityId)
{
TargetedEnt = n;
break;
}
}
}
}
foreach (IMyTextPanel LCD in LCDs)
{
if (LCD.CustomName != LCDName + "1") continue;
DrawRadar(LCD, Targets, resolution, LCD.GetPosition(), MaxRange, DrawCircles, SmoothCorners);
LCD.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}
}
//
//
foreach (IMyTextPanel lcd in LCDs)
{
if (lcd.CustomName == LCDName + "2")
{
sb.Clear();
if (Targets.Count > 0 && finished)
{
sb.Append(Name + RLSName);
sb.AppendLine();
sb.Append(TargetsD + Targets.Count);
sb.AppendLine();
sb.Append(Mode + IsMode(ActiveMode));
sb.AppendLine();
sb.Append(IsCharge(ActiveMode));
sb.AppendLine();
if (AIBlock_move != null && AIBlock_offensive != null)
{
sb.Append(IsAIScanning(AI_Scanning));
sb.AppendLine();
}
sb.AppendLine(text_divider.ToString());
sb.AppendLine();
sb.Append(AMode + IsAMode(SAutoLaunch, LAutoLaunch));
sb.AppendLine();
sb.Append(TurrO + IsAMode(offset, offset));
sb.AppendLine();
if (!TargetedEnt.IsEmpty())
{
sb.Append(TargetedT + " " + IsTargetRelation(TargetedEnt.Relationship));
sb.AppendLine();
Vector3D Dist1 = TargetedEnt.Position;
Vector3D Dist2 = Me.CubeGrid.GetPosition();
Vector3D Dist3 = Dist2 - Dist1;
sb.Append(TNam + TargetedEnt.Name);
sb.AppendLine();
sb.Append(TSpe + (float)TargetedEnt.Velocity.Length());
sb.AppendLine();
sb.Append(TDis + (float)Dist3.Length());
sb.AppendLine();
sb.Append(TRel + TargetedEnt.Relationship);
sb.AppendLine();
}
for (int i = Targets.Count - 1; i >= 0; i--)
{
MyDetectedEntityInfo n = Targets[i];
if (TargetedEnt.EntityId.Equals(n.EntityId))
{
if (i > 0)
{
i--;
n = Targets[i];
}
else break;
}
Vector3D Dist1 = n.Position;
Vector3D Dist2 = Me.CubeGrid.GetPosition();
Vector3D Dist3 = Dist2 - Dist1;
sb.Append(TNam + n.Name + " " + IsTargetRelation(n.Relationship));
sb.AppendLine();
sb.Append(TSpe + (float)n.Velocity.Length());
sb.AppendLine();
sb.Append(TDis + (float)Dist3.Length());
sb.AppendLine();
sb.Append(TRel + n.Relationship);
sb.AppendLine();
}
}
else
{
sb.Append(Name + RLSName);
sb.AppendLine();
sb.Append(TargetsN);
sb.AppendLine();
sb.Append(Mode + IsMode(ActiveMode));
sb.AppendLine();
sb.Append(IsCharge(ActiveMode));
sb.AppendLine();
sb.Append(IsAIScanning(AI_Scanning));
sb.AppendLine();
sb.AppendLine(text_divider.ToString());
sb.AppendLine();
sb.Append(AMode + IsAMode(SAutoLaunch, LAutoLaunch));
sb.AppendLine();
sb.Append(TurrO + IsAMode(offset, offset));
sb.AppendLine();
if (!ENG)
{
if (RPB != null || RT != null)
{
if (RTurrets.Count < 0)
{
sb.Append("Ракеты Rdav's подключены, но нет турелей");
sb.AppendLine();
}
if (RTurrets.Count > 0)
{
sb.Append("Ракеты Rdav's подключены");
sb.AppendLine();
}
}
if (WPB != null || WT != null)
{
if (RTurrets.Count < 0)
{
sb.Append("Ракеты Whip's подключены, но нет турели");
sb.AppendLine();
}
if (RTurrets.Count > 0)
{
sb.Append("Ракеты Whip's подключены");
sb.AppendLine();
}
}
if (EPB != null || ET != null)
{
if (!lcd5)
{
sb.Append("Ракеты Alysius подключены, но нет дисплея");
sb.AppendLine();
}
if (lcd5)
{
sb.Append("Ракеты Alysius подключены");
sb.AppendLine();
}
}
if (R4PB != null || R4T != null)
{
sb.Append("Ракеты R-4 подключены");
sb.AppendLine();
}
}
if (ENG)
{
if (RPB != null)
{
if (RTurrets.Count < 0)
{
sb.Append("Rdav's missiles connected, but no turrets");
sb.AppendLine();
}
if (RTurrets.Count > 0)
{
sb.Append("Rdav's missiles connected and ready");
sb.AppendLine();
}
}
if (WPB != null)
{
if (RTurrets.Count < 0)
{
sb.Append("Whip's missiles connected, but no turret");
sb.AppendLine();
}
if (RTurrets.Count > 0)
{
sb.Append("Whip's missiles connected");
sb.AppendLine();
}
}
if (EPB != null || ET != null)
{
if (!lcd5)
{
sb.Append("Alysius missiles connected, but no display");
sb.AppendLine();
}
if (lcd5)
{
sb.Append("Alysius missiles connected");
sb.AppendLine();
}
}
if (R4PB != null || R4T != null)
{
sb.Append("R-4 missiles is connnected");
sb.AppendLine();
}
}

}
lcd.WriteText(sb.ToString());
lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}
if (lcd.CustomName == LCDName + "3")
{
if (lcd != null)
{
sb.Clear();
sb.Append(Alar + RLSName);
sb.AppendLine();
if (Cameras.Count > 0 && Targets.Count > 0 && TargetUpd)
{
double Charge = 0;
foreach (IMyCameraBlock cam in Cameras)
{
Charge += cam.AvailableScanRange;
}
if (Charge / Cameras.Count <= MaxRange + 300)
{
if (!ENG)
{
sb.Append("Заряд камер близок к растрате, цели могут сорваться");
sb.AppendLine();
}
else
{
sb.Append("Charge of cameras near to zero, targets can be lost");
sb.AppendLine();
}
}
}
//
if (Cameras.Count > 0 && Cameras.Count <= MaxRange / 2000)
{
if (!ENG)
{
sb.Append("Камер недостаточно для эффективного сканирования");
sb.AppendLine();
sb.Append("на такую дистанцию. Увеличьте число камер.");
sb.AppendLine();
}
else
{
sb.Append("Not enough cameras for effective scanning");
sb.AppendLine();
sb.Append("on such distance. Increase amount of cameras");
sb.AppendLine();
}
}
if (Cameras.Count > 0 && Cameras.Count < Targets.Count && TargetUpd)
{
if (!ENG)
{
sb.Append("Камер недостаточно для ведения такого кол-ва целей");
sb.AppendLine();
}
else
{
sb.Append("Not enough cameras for lock-on this amount of targets");
sb.AppendLine();
}
}
if (Turrets.Count <= 0 && Cameras.Count > 0)
{
if (!ENG)
{
sb.Append("В РЛС отсутствуют турели при наличии камер.");
sb.AppendLine();
sb.Append("Турели позволят гарантированно обнаружить цель на малых");
sb.AppendLine();
sb.Append("расстояниях, а так же повысят надёжность ведения цели.");
sb.AppendLine();
}
else
{
sb.Append("Radar not have turrets.");
sb.AppendLine();
sb.Append("Turrets will help with detect targets on small distances");
sb.AppendLine();
sb.Append("and will increase the reliability");
sb.AppendLine();
}
}
if (ActiveMode)
{
if (!ENG)
{
sb.Append("Активный режим РЛС может вызывать лаги");
sb.AppendLine();
if (Frequency1 < 15)
{
sb.Append("Частота сканирования очень высокая, лучше уменьшить её");
sb.AppendLine();
}
}
else
{
sb.Append("Active search RADAR mode can cause lags");
sb.AppendLine();
if (Frequency1 < 15)
{
sb.Append("Scan frequency is too big, better to reduce it");
sb.AppendLine();
}
}
}
if (Targets.Count > 0 && TargetUpd && Cameras.Count > 0)
{
for (int i = 0; i < Targets.Count; i++)
{
MyDetectedEntityInfo target = Targets[i];
if (target.BoundingBox.Size.Length() <= 25)
{
if (!ENG)
{
sb.Append("Цель " + target.Name);
sb.AppendLine();
sb.Append("Имеет малые размеры, захват ненадёжен.");
sb.AppendLine();
}
if (ENG)
{
sb.Append("Target " + target.Name);
sb.AppendLine();
sb.Append("Has small size, lock-on unsafe.");
sb.AppendLine();
}
}
}
}
if (MouseSelector)
{
if (!ENG)
{
sb.Append("Включён автоматический приоритет целей.");
sb.AppendLine();
sb.Append("Уберите его, если хотите выбрать цель вне");
sb.AppendLine();
sb.Append("зависимости от своей ориентации.");
sb.AppendLine();
}
else
{
sb.Append("Auto priority of target selection online");
sb.AppendLine();
sb.Append("Turn off, if you want select target manually");
sb.AppendLine();
sb.Append("and without depend from your direction");
sb.AppendLine();
}
}
if (LAutoLaunch || SAutoLaunch)
{
if (!ENG)
{
sb.Append("Автозапуск ракет включён, вы НЕ можете");
sb.AppendLine();
sb.Append("выбирать цели вручную при атаке целей им!");
sb.AppendLine();
}
else
{
sb.Append("Autolaunch of missiles online, you CAN'T");
sb.AppendLine();
sb.Append("select target manually for attack them by it!");
sb.AppendLine();
}
if (Cameras.Count <= 0)
{
if (!ENG)
{
sb.Append("Автозапуск ракет включён, но нет камер. Убедитесь, что");
sb.AppendLine();
sb.Append("это вам не помешает");
sb.AppendLine();
}
else
{
sb.Append("Autolaunch of missiles online, but cameras not found. I hope you");
sb.AppendLine();
sb.Append("sure what it's can't broke your idea");
sb.AppendLine();
}
}
if ((!UseTimersInstead && (RPB == null && WPB == null && EPB == null && R4PB == null)) || (UseTimersInstead && (RT == null && WT == null && ET == null && R4T == null)))
{
if (!ENG)
{
sb.Append("Автозапуск ракет включён, но нет ни одной точки");
sb.AppendLine();
sb.Append("доступа к оным. Как и что я буду запускать?");
sb.AppendLine();
}
else
{
sb.Append("Autolaunch of missiles online, but not found any");
sb.AppendLine();
sb.Append("PB or timer for them. How and what i will launch?");
sb.AppendLine();
}
}
if (RTurrets.Count <= 0 && ((SAtype != 3 && Emode != 1 && SAtype != 0 && SAutoLaunch) || (LAtype != 3 && Emode != 1) && LAtype != 0 && LAutoLaunch))
{
if (!ENG)
{
sb.Append("Автозапуск ракет включён, но нет турелей для");
sb.AppendLine();
sb.Append("ведения атакуемой цели. Установите, если нужна");
sb.AppendLine();
}
else
{
sb.Append("Autolaunch enabled, but no turrets for guidance");
sb.AppendLine();
sb.Append("not found. Build them if you need!");
sb.AppendLine();
}
}
if (!lcd6 && ((SAutoLaunch && SAtype == 3 && Emode == 1) || (LAtype == 3 && Emode == 1 && LAutoLaunch)))
{
if (!ENG)
{
sb.Append("Автозапуск ракет включён, но нет дисплея для");
sb.AppendLine();
sb.Append("ведения атакуемой цели. Установите или измените режим ракеты!");
sb.AppendLine();
}
else
{
sb.Append("Autolaunch enabled, but no display for guidance");
sb.AppendLine();
sb.Append("not found. Build it or change launch mode!");
sb.AppendLine();
}
}
}
if (BroadcastEn && ANT != null)
{
if (!ENG)
{
sb.Append("Включено вещание. Противники будут видеть вас на дистанции");
sb.AppendLine();
sb.Append("вещания.");
sb.AppendLine();
if (!AllInf)
{
sb.Append("Кроме того, союзникам нужны камеры для подтверждения");
sb.AppendLine();
sb.Append("дислокации передаваемой цели");
sb.AppendLine();
}
}
else
{
sb.Append("Broadcast enabled. Enemies will see you on distance");
sb.AppendLine();
sb.Append("of broadcast.");
sb.AppendLine();
if (!AllInf)
{
sb.Append("By the way, allies must have cameras for accept");
sb.AppendLine();
sb.Append("transmited target dislocation");
sb.AppendLine();
}
}
}
if (ControlTurr && !BlindCheck && !OwnDamage && TCameras.Count <= ControlledTurrets.Count * 5)
{
if (!ENG)
{
sb.Append("Не хватает камер для текущего количества турелей.");
sb.AppendLine();
sb.Append("В связи с настройками турели не будет стрелять там");
sb.AppendLine();
sb.Append("где она не получила разрешение от камер");
sb.AppendLine();
}
else
{
sb.Append("Not enough cameras for such amount of turrets.");
sb.AppendLine();
sb.Append("In case of settings turrets will be not fire if");
sb.AppendLine();
sb.Append("they haven't confirm from cameras");
sb.AppendLine();
}
}
lcd.WriteText(sb.ToString());
lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}
}
}
if (!Targets.Contains(TargetedEnt) && !TargetedEnt.IsEmpty())
{
TargetedEnt = new MyDetectedEntityInfo();
}
if (ATTargets.Count > 0)
{
for (int i1 = 0; i1 < Targets.Count; i1++)
{
MyDetectedEntityInfo target = Targets[i1];
for (int i = ATTargets.Count - 1; i >= 0; i--)
{
MyDetectedEntityInfo target1 = ATTargets[i];
if (target.EntityId == target1.EntityId)
{
ATTargets.RemoveAt(i);
ATTargets.Insert(i, target);
}
}
}
}
foreach (IMyTextPanel lcd in LCDs)
{
if (lcd.CustomName != LCDName + "4") continue;
sb.Clear();
if (RTurrets.Count > 0)
{
foreach (IMyLargeTurretBase turret in RTurrets)
{
if (RTurrets.Count > 1)
{
foreach (IMyLargeTurretBase turret1 in RTurrets)
{
turret1.CustomName = NotR_FORWARDName;
}
}
if (((turret.HasTarget && turret.GetTargetedEntity().Equals(TargetedEnt) && !TargetedEnt.IsEmpty()) || turret.IsAimed) && MissileType != 1)
{
turret.CustomName = R_FORWARDName;
if (!ENG)
{
sb.Append("РАКЕТЫ ИМЕЮТ НАВЕДЕНИЕ");
sb.AppendLine();
}
break;
}
if (turret.HasTarget && turret.GetTargetedEntity().Equals(TargetedEnt) && !TargetedEnt.IsEmpty() && MissileType == 1 && RPB != null)
{
if (!ENG)
{
sb.Append("РАКЕТЫ ИМЕЮТ НАВЕДЕНИЕ");
sb.AppendLine();
}
break;
}
}
}
if (IRTurrets.Count > 0)
{
for (int i = 0; i < IRTurrets.Count; i++)
{
IMyLargeTurretBase turret = IRTurrets[i];
turret.CustomName = NotR_FORWARDName;
if (!TargetedEnt.IsEmpty())
{
if (turret.HasTarget && turret.GetTargetedEntity().EntityId == TargetedEnt.EntityId)
{
turret.CustomName = IR_FORWARDName;
if (!ENG)
{
sb.Append("ИК-ТУРЕЛИ ВИДЯТ ЦЕЛЬ");
sb.AppendLine();
}
else
{
sb.Append("IR-TURRETS HAVE GUIDANCE");
sb.AppendLine();
}
}
else
{
turret.ResetTargetingToDefault();
}
}
}
}
if (!ENG)
{
sb.Append(IsMissiles(MissileType));
sb.AppendLine();
}
if (ENG)
{
if (RTurrets.Count > 0)
{
for (int i = 0; i < RTurrets.Count; i++)
{
IMyLargeTurretBase turret = RTurrets[i];
if (RTurrets.Count > 1)
{
turret.CustomName = NotR_FORWARDName;
}
if ((turret.HasTarget && turret.GetTargetedEntity().Equals(TargetedEnt) && !TargetedEnt.IsEmpty()) || turret.IsAimed)
{
turret.CustomName = R_FORWARDName;
if (MissileType != 1)
{
sb.Append("TURRET-GUIDED MISSILES HAVE GUIDANCE");
sb.AppendLine();
}
break;
}
if (turret.HasTarget && turret.GetTargetedEntity().Equals(TargetedEnt) && !TargetedEnt.IsEmpty() && MissileType == 1 && RPB != null)
{
sb.Append("TURRET-GUIDED MISSILES HAVE GUIDANCE");
sb.AppendLine();
break;
}
}
}
sb.Append(IsMissiles(MissileType));
sb.AppendLine();
}
lcd.WriteText(sb.ToString()); lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}
foreach (IMyTextPanel lcd in LCDs)
{
if (lcd.CustomName != LCDName + "6") continue;

sb.Clear();
if (!TargetedEnt.IsEmpty())
{
sb.Append("GPS:" + RLSName + " #111:" + TargetedEnt.Position.X + ":" + TargetedEnt.Position.Y + ":" + TargetedEnt.Position.Z + ":");
sb.AppendLine();
}
lcd.WriteText(sb.ToString());
lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}
if ((RPB != null || WPB != null || EPB != null || RT != null || WT != null || ET != null) && RTurrets.Count > 0)
{
if (!TargetedEnt.IsEmpty())
{
foreach (IMyLargeTurretBase turret in RTurrets)
{
turret.SetTarget(TargetedEnt.Position + TargetedEnt.Velocity);
}
}
else
{
foreach (IMyLargeTurretBase turret in RTurrets)
{
turret.ResetTargetingToDefault();
}
}
}
if (StatusEnabled)
{
double TanksFull = 0;
double BatsCharge = 0;
if (Tanks.Count <= 0) TanksFull = 1000;
if (Bats.Count <= 0) BatsCharge = 1000;
if (Tanks.Count > 0)
{
foreach (IMyGasTank tank in Tanks)
{
TanksFull += tank.FilledRatio * 100 / Tanks.Count;
}
foreach (IMyGasTank tank in Tanks)
{
if (TanksFull < EmergencyTanks) tank.Stockpile = true;
if (TanksFull > EmergencyTanks) tank.Stockpile = false;
}
}
if (Bats.Count > 0)
{
foreach (IMyBatteryBlock bat in Bats)
{
BatsCharge += bat.CurrentStoredPower / bat.MaxStoredPower * 100 / Bats.Count;
}
foreach (IMyBatteryBlock bat in Bats)
{
if (BatsCharge < EmergencyCharge) bat.ChargeMode = ChargeMode.Recharge;
if (BatsCharge > EmergencyCharge) bat.ChargeMode = ChargeMode.Auto;
}

}
if (EmergencyOff)
{
foreach (IMyProgrammableBlock pb in DPBs)
{
if (BatsCharge < EmergencyCharge || TanksFull < EmergencyTanks)
{
pb.Enabled = false;
}
}
}
foreach (IMyTextPanel lcd in LCDs)
{
if (lcd.CustomName != LCDName + "5") continue;
if (StatusEnabled)
{
sb.Clear();
if (!ENG)
{
sb.Append("Статус готовности ракет:");
sb.AppendLine();
if (Bats.Count > 0) sb.Append("срзнч Заряда Батарей: " + BatsCharge);
sb.AppendLine();
if (Tanks.Count > 0) sb.Append("срзнч Заполненности баков: " + TanksFull);
sb.AppendLine();
if (EmergencyOff)
{
foreach (IMyProgrammableBlock pb in DPBs)
{
if (!pb.Enabled) { sb.Append("Ракеты не готовы к пуску"); break; }
if (pb.Enabled) { sb.Append("Ракеты готовы к пуску"); break; }
}
}
else
{
sb.Append("Ракеты готовы к пуску");
}
}
if (ENG)
{
sb.Append("Missile Status:");
sb.AppendLine();
if (Bats.Count > 0) sb.Append("Bat Charge%: " + BatsCharge);
sb.AppendLine();
if (Tanks.Count > 0) sb.Append("Tanks Fuel%: " + TanksFull);
sb.AppendLine();
if (EmergencyOff)
{
foreach (IMyProgrammableBlock pb in DPBs)
{
if (!pb.Enabled) { sb.Append("Missiles not ready"); break; }
if (pb.Enabled) { sb.Append("Missiles ready"); break; }
}
}
else
{
sb.Append("Missiles ready");
}
}
lcd.WriteText(sb.ToString());
lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}
}
}
if (BroadcastEn && !HasAntenna)
{
TargetsBroad.Clear();
targettimes.Clear();
}
if (BroadcastEn && HasAntenna)
{
//
for (int i = targettimes.Count - 1; i >= 0; i--)
{
string targed = targettimes[i];
string[] targ = targed.Split('b');
long id1;
double rate;
long.TryParse(targ[1], out id1);
double.TryParse(targ[0], out rate);
rate--;
if (rate > 0)
{
targettimes[i] = (rate.ToString() + "b" + id1.ToString() + "b");
}
if (rate <= 0)
{
for (int i1 = 0; i1 < TargetsBroad.Count; i1++)
{
MyDetectedEntityInfo target = TargetsBroad[i1];
if (target.EntityId == id1)
{
TargetsBroad.Remove(target);
targettimes.Remove(targed);
LTargets.Add(target);
break;
}
}
}
}
//
TrTargets.Clear();
if (listener != null)
{
if (TargetsBroad.Count > 0)
{
for (int i = Targets.Count - 1; i >= 0; i--)
{
MyDetectedEntityInfo target = Targets[i];
foreach (MyDetectedEntityInfo target1 in TargetsBroad)
{
if (target.EntityId == target1.EntityId) { Targets.Remove(target); }
}
}
}
if (Targets.Count > 0)
{
for (int i = 0; i < Targets.Count; i++)
{
MyDetectedEntityInfo target = Targets[i];
if (!AllInf)
{
IGC.SendBroadcastMessage(BTAG, target.Position + ((Vector3D)target.Velocity / 5) + "notinf", (TransmissionDistance)BroadcastRange);
}
else
{
string targetpos = target.Position.ToString() + "b";
string targetvel = target.Velocity.ToString() + "b";
string targetrelate = target.Relationship.ToString() + "b";
string targetsize = target.BoundingBox.Max.ToString() + "b";
string targetsize1 = target.BoundingBox.Min.ToString() + "b";
string targetname = target.Name.ToString() + "b";
string targetid = target.EntityId.ToString() + "b";
string targettype = target.Type.ToString() + "b";
string targets = targetpos + targetvel + targetrelate + targetsize + targetname + targetid + targettype + targetsize1;
IGC.SendBroadcastMessage(BTAG, targets, (TransmissionDistance)BroadcastRange);
}
}
}
if (RemoveRate <= 0)
{
TargetsBroad.Clear();
}
while (listener.HasPendingMessage)
{
if (args.ToString() == BTAG)
{
object TrTarget = listener.AcceptMessage().Data;
if (TrTarget != null)
{
string[] targetinfo = null;
if (!TrTarget.ToString().EndsWith("notinf"))
{
char b = 'b';
targetinfo = TrTarget.ToString().Split(b);
if (targetinfo != null)
{
Vector3D position;
Vector3D.TryParse(targetinfo[0], out position);
Vector3D veloc;
Vector3D.TryParse(targetinfo[1], out veloc);
string name = targetinfo[4];
long targetid;
long.TryParse(targetinfo[5], out targetid);
MyDetectedEntityType targettype;
MyDetectedEntityType.TryParse(targetinfo[6], out targettype);
Vector3D max;
Vector3D.TryParse(targetinfo[3], out max);
Vector3D min;
Vector3D.TryParse(targetinfo[7], out min);
MyDetectedEntityInfo target1 = new MyDetectedEntityInfo();
if (targetid != 0) { target1 = new MyDetectedEntityInfo(targetid, name, targettype, position, new MatrixD(), veloc, MyRelationsBetweenPlayerAndBlock.NoOwnership, new BoundingBoxD(min, max), new long()); }
bool exist2 = false;
bool exist = false;
foreach (MyDetectedEntityInfo target in TargetsBroad)
{
if (target.EntityId == targetid)
{
exist2 = true;
break;
}
}
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == targetid)
{
exist = true;
break;
}
}
if (targetid != 0)
{
if (!exist2 && !exist) TargetsBroad.Add(target1);
if (exist2 && !exist)
{
foreach (MyDetectedEntityInfo targetd in TargetsBroad)
{
if (targetd.EntityId == targetid)
{
TargetsBroad.Remove(targetd);
break;
}
}
TargetsBroad.Add(target1);
}
bool exist3 = false;
foreach (string target in targettimes)
{
string[] targ = target.Split('b');
long id1;
long.TryParse(targ[1], out id1);
if (id1 == targetid)
{
exist3 = true;
break;
}
}
if (RemoveRate > 0 && !exist2 && !exist3 && !exist)
targettimes.Add(RemoveRate.ToString() + "b" + targetid.ToString() + "b");
if (RemoveRate > 0 && exist2 && exist3 && !exist)
{
foreach (string targed in targettimes)
{
string[] targ = targed.Split('b');
long id1;
long.TryParse(targ[1], out id1);
if (targetid == id1)
{
targettimes.Remove(targed);
targettimes.Add(RemoveRate.ToString() + "b" + targetid.ToString() + "b");
break;
}
}
}
}

}
}
else
{
TrTarget = TrTarget.ToString().Remove(TrTarget.ToString().Length - 6, 6);
bool exist = false;
Vector3D TargetsCoord;
Vector3D.TryParse(TrTarget.ToString(), out TargetsCoord);
if (Targets.Count > 0)
{
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.Position == TargetsCoord)
{
exist = true;
break;
}
}
}
if (TargetsCoord != new Vector3D() && TargetsCoord != null && !exist) TrTargets.Add(TargetsCoord);
}
}
}
}
for (int i = 0; i < TargetsBroad.Count; i++)
{
MyDetectedEntityInfo n = TargetsBroad[i];
if (Targets.Count <= 0) { Targets.Add(n); AddAuto(n); }

if (Targets.Count > 0)
{
bool exist = false;
foreach (MyDetectedEntityInfo v in Targets)
{
if (n.EntityId == v.EntityId)
{
exist = true;
break;
}
}
if (!exist)
{
if ((n.BoundingBox.Size.Length() < minsizeS && n.Type == MyDetectedEntityType.SmallGrid) || (n.BoundingBox.Size.Length() < minsizeL && n.Type == MyDetectedEntityType.LargeGrid) || (n.Type == MyDetectedEntityType.LargeGrid && mingrid == 1) || (n.Type == MyDetectedEntityType.SmallGrid && mingrid == 2)) continue;
Targets.Add(n);
AddAuto(n);
}
}
}
}

}
foreach (IMyShipController RC in RCs)
{
if (!RC.IsSameConstructAs(Me)) continue;
foreach (IMyTextPanel lcd in LCDs)
{
if (lcd.CustomName != LCDName + "7") continue;
sb.Clear();
if (!TargetedEnt.IsEmpty())
{
string Q = "*";
if ((lcd.GetPosition() - (Vector3D)TargetedEnt.HitPosition).Length() <= WeaponRange) Q = "+";
//lcd.FontSize = (float)3.5;
string S1 = "----------------------------------";
string S2 = "----------------" + Q + "-----------------";
string S3 = "----------------------------------";
double NeedU = 0;
double NeedL = 0;
Vector3D Direction;
Vector3D ShootVector;
if (!offset || TargetedEnt.HitPosition == null) Direction = TargetedEnt.Position - RC.CubeGrid.GetPosition();
else Direction = (Vector3D)TargetedEnt.HitPosition - RC.CubeGrid.GetPosition();
if (Direction.Length() <= WeaponRange)
{
ShootVector = TargetedEnt.Position + (Vector3D)TargetedEnt.Velocity * (Direction.Length() / WeaponSpeed) - RC.GetPosition();
ShootVector -= RC.GetShipVelocities().LinearVelocity * (Direction.Length() / WeaponSpeed);
Direction += ShootVector;
if (RC.GetNaturalGravity().Length() > 0 && BallGravity) Direction -= (RC.GetNaturalGravity() * (Direction.Length() / WeaponSpeed));
}
NeedU = Vector3D.Dot(lcd.WorldMatrix.Up, Direction);
NeedL = Vector3D.Dot(lcd.WorldMatrix.Left, Direction);
if (NeedU > 10)
{
S1 = S1.Remove(17, 1);
S1 = S1.Insert(17, "^");
}
if (NeedU < 10)
{
S3 = S3.Remove(17, 1);
S3 = S3.Insert(17, "v");
}
if (NeedL > 10)
{
S2 = S2.Remove(16, 1);
S2 = S2.Insert(16, "<");
}
if (NeedL < 10)
{
S2 = S2.Remove(18, 1);
S2 = S2.Insert(18, ">");
}
sb.Append(S1);
sb.AppendLine();
sb.Append(S2);
sb.AppendLine();
sb.Append(S3);
sb.AppendLine();
}
else
{
sb.AppendLine();
if (!ENG)
{
sb.Append("------ЦЕЛЬ НЕ ВЫБРАНА------------");
}
else
{
sb.Append("-----TARGET NOT SELECTED---------");
}
sb.AppendLine();
}
lcd.WriteText(sb.ToString());
lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}

if (Targets.Count > 0 && !TargetedEnt.IsEmpty() && ControlTurr && ControlledTurrets.Count > 0)
{
for (int i1 = 0; i1 < ControlledTurrets.Count; i1++)
{
IMyLargeTurretBase turret = ControlledTurrets[i1];
double Distance = (TargetedEnt.Position - turret.GetPosition()).Length();
Vector3D vector;
Vector3D pos = TargetedEnt.Position;
if (offset && TargetedEnt.HitPosition != null) pos = (Vector3D)TargetedEnt.HitPosition;
if (Gravity)
{
vector = pos + ((TargetedEnt.Velocity - RC.GetShipVelocities().LinearVelocity) * (Distance / ShellSpeed)) + RC.GetShipVelocities().AngularVelocity - (RC.GetNaturalGravity() * (Distance / ShellSpeed));
}
else
{
vector = pos + (TargetedEnt.Velocity - RC.GetShipVelocities().LinearVelocity) * (Distance / ShellSpeed) + RC.GetShipVelocities().AngularVelocity;
}

turret.SetTarget(vector);
if (turret.IsAimed)
{
bool confirmed = false;
if (!BlindCheck)
{
if (TCameras.Count > 0 && Distance < TRange && !OwnDamage)
{
for (int i = TCameras.Count - 1; i >= 0; i--)
{
IMyCameraBlock camera = TCameras[i];
double dist = (camera.GetPosition() - turret.GetPosition()).Length();
if (camera != null && camera.CanScan(vector) && dist < MaxCamDist)
{
MyDetectedEntityInfo info = camera.Raycast(vector);
if (info.Relationship != MyRelationsBetweenPlayerAndBlock.Owner && info.EntityId != Me.CubeGrid.EntityId && ((info.Relationship != MyRelationsBetweenPlayerAndBlock.Friends && info.Relationship != MyRelationsBetweenPlayerAndBlock.FactionShare) || FriendlyOff)) confirmed = true;
}
}
}
}
else
{
confirmed = true;
int step = 1; // Шаг проверки
Vector3D dir;
// Создаем вектор на основе азимута и элевейшн
Vector3D.CreateFromAzimuthAndElevation(turret.Azimuth, turret.Elevation, out dir);
dir = Vector3D.TransformNormal(dir, turret.WorldMatrix);
// Преобразуем вектор из локальной системы координат турели в мировую
for (double distance = BlindCheck_Start; distance < BlindCheckRange; distance += step)
{
if (Debug) Echo("Checking Block at distance: " + distance);
// Расчет позиции вдоль линии визирования (мировые координаты)
Vector3D checkPosition = turret.GetPosition() + Vector3D.Normalize(dir);
// Преобразуем мировые координаты в локальные координаты относительно CubeGrid
Vector3D localPosition = Me.CubeGrid.WorldToGridInteger(checkPosition);
// Преобразуем в целочисленные координаты (которые используются в CubeExists)
Vector3I gridPosition = Vector3I.Round(localPosition);  // Округляем позицию до целого
if (Me.CubeGrid.CubeExists(gridPosition))
{
// Если блок существует на пути, прекращаем проверку
confirmed = false;
if (Debug) Echo("Block collision at position: " + gridPosition);
break;
}

}
}
if (Distance > TRange)
{
foreach (IMyCameraBlock cam in TCameras)
{
if (!Cameras.Contains(cam)) Cameras.Add(cam);
}
}
else
{
foreach (IMyCameraBlock cam in TCameras)
{
if (Cameras.Contains(cam)) Cameras.Remove(cam);
}
}
if ((confirmed || OwnDamage) && Distance < TRange)
{
turret.ApplyAction("Shoot_On", null);
}
else turret.ApplyAction("Shoot_Off", null);
}
else turret.ApplyAction("Shoot_Off", null);
}
}
break;
}

if (SAutoLaunch || LAutoLaunch)
{
if (Debug && ANT != null) ANT.HudText = times.Count.ToString();
if (times.Count > 0)
{
for (int i = times.Count - 1; i >= 0; i--)
{
string targed = times[i];
string[] targ = targed.Split('b');
long id1;
long.TryParse(targ[1], out id1);
double rate;
double.TryParse(targ[0], out rate);
rate--;
if (rate > 0)
{
times[i] = (rate.ToString() + "b" + id1.ToString() + "b");
}
if (rate == 1)
{
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == id1) { TargetedEnt = target; break; }
}
}
if (rate <= 0)
{
bool large = false;
foreach (MyDetectedEntityInfo target in Targets)
{
if (target.EntityId == id1) { TargetedEnt = target; if ((target.Type.ToString() == "LargeGrid" && !IDNotType) || (IDNotType && target.BoundingBox.Size.Length() > SizeS)) large = true; break; }
}
for (int z = MissilesCount; z > 0; z--)
{
double Distance = (Me.CubeGrid.GetPosition() - TargetedEnt.Position).Length();
if ((SAtype == 1 && !large && SAutoLaunch && SAutoRange >= Distance) || (LAtype == 1 && large && LAutoLaunch && LAutoRange >= Distance))
{
if (!UseTimersInstead && RPB != null) RPB.TryRun("Fire");
if (UseTimersInstead && RT != null) RT.Trigger();
}
if ((SAtype == 2 && !large && SAutoLaunch && SAutoRange >= Distance) || (LAtype == 2 && large && LAutoLaunch && LAutoRange >= Distance))
{
if (!UseTimersInstead && WPB != null) WPB.TryRun("fire");
if (UseTimersInstead && WT != null) WT.Trigger();
}
if ((SAtype == 3 && !large && SAutoLaunch && SAutoRange >= Distance) || (LAtype == 3 && large && LAutoLaunch && LAutoRange >= Distance))
{
if (!UseTimersInstead && EPB != null) EPB.TryRun("MODE:" + Emode);
if (UseTimersInstead && ET != null) ET.Trigger();
}
if ((SAtype == 0 && !large && SAutoLaunch && SAutoRange >= Distance) || (LAtype == 0 && large && LAutoLaunch && LAutoRange >= Distance))
{
if (!UseTimersInstead && EPB != null) R4PB.TryRun("FIRE");
if (UseTimersInstead && ET != null) R4T.Trigger();
}
}
if (large)
{
rate = LRate;
times[i] = ((rate + 2).ToString() + "b" + id1.ToString() + "b");
}
else
{
rate = SRate;
times[i] = ((rate + 2).ToString() + "b" + id1.ToString() + "b");
}
}
}
foreach (string targed in times)
{
string[] targ = targed.Split('b');
long id1;
long.TryParse(targ[1], out id1);
List<long> ids = new List<long>();
foreach (MyDetectedEntityInfo target in Targets)
{
ids.Add(target.EntityId);
}
if (ids.Count == Targets.Count)
{
if (!ids.Contains(id1))
{
times.Remove(targed);
break;
}
}
}
}
}
if (ControlTurr && ControlledTurrets.Count > 0)
{
foreach (IMyLargeTurretBase turret in ControlledTurrets)
{
if (TargetedEnt.IsEmpty() || !turret.IsAimed || Targets.Count <= 0)
turret.ApplyAction("Shoot_Off", null);
}
}


foreach (IMyTextPanel lcd in LCDs)
{
if (lcd.CustomName != LCDName + "8") continue;
sb.Clear();
sb.Append("BroadcastEn " + BroadcastEn.ToString());
sb.AppendLine();
sb.Append("ActiveMode " + ActiveMode.ToString());
sb.AppendLine();
sb.Append("Scan " + Scan.ToString());
sb.AppendLine();
sb.Append("MouseSelector " + MouseSelector.ToString());
sb.AppendLine();
sb.Append("SAutoLaunch " + SAutoLaunch.ToString());
sb.AppendLine();
sb.Append("LAutoLaunch " + LAutoLaunch.ToString());
sb.AppendLine();
sb.Append("FriendlyOff " + FriendlyOff.ToString());
sb.AppendLine();
lcd.WriteText(sb.ToString());
lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
}
sb.Clear();
foreach (IMyProgrammableBlock pb in CPBs)
{
if (Targets.Count > 0)
{
if (!offset || TargetedEnt.HitPosition == null) pb.TryRun(TargetedEnt.Position + "b" + (Vector3D)TargetedEnt.Velocity + "b" + TargetedEnt.BoundingBox.Min + "b" + TargetedEnt.BoundingBox.Max + "b" + TargetedEnt.EntityId + "b" + "TTARGET" + "b" + TargetedEnt.Orientation.M11 + ":" + TargetedEnt.Orientation.M12 + ":" + TargetedEnt.Orientation.M13 + ":" + TargetedEnt.Orientation.M14 + ":" + TargetedEnt.Orientation.M21 + ":" + TargetedEnt.Orientation.M22 + ":" + TargetedEnt.Orientation.M23 + ":" + TargetedEnt.Orientation.M24 + ":" + TargetedEnt.Orientation.M31 + ":" + TargetedEnt.Orientation.M32 + ":" + TargetedEnt.Orientation.M33 + ":" + TargetedEnt.Orientation.M34 + ":" + TargetedEnt.Orientation.M41 + ":" + TargetedEnt.Orientation.M42 + ":" + TargetedEnt.Orientation.M43 + ":" + TargetedEnt.Orientation.M44);
else pb.TryRun((Vector3D)TargetedEnt.HitPosition + "b" + (Vector3D)TargetedEnt.Velocity + "b" + TargetedEnt.BoundingBox.Min + "b" + TargetedEnt.BoundingBox.Max + "b" + TargetedEnt.EntityId + "b" + "TTARGET" + "b" + TargetedEnt.Orientation.M11 + ":" + TargetedEnt.Orientation.M12 + ":" + TargetedEnt.Orientation.M13 + ":" + TargetedEnt.Orientation.M14 + ":" + TargetedEnt.Orientation.M21 + ":" + TargetedEnt.Orientation.M22 + ":" + TargetedEnt.Orientation.M23 + ":" + TargetedEnt.Orientation.M24 + ":" + TargetedEnt.Orientation.M31 + ":" + TargetedEnt.Orientation.M32 + ":" + TargetedEnt.Orientation.M33 + ":" + TargetedEnt.Orientation.M34 + ":" + TargetedEnt.Orientation.M41 + ":" + TargetedEnt.Orientation.M42 + ":" + TargetedEnt.Orientation.M43 + ":" + TargetedEnt.Orientation.M44);
for (int i1 = 0; i1 < Targets.Count; i1++)
{
MyDetectedEntityInfo target = Targets[i1];
if (!offset || target.HitPosition == null) pb.TryRun(target.Position + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
else pb.TryRun((Vector3D)target.HitPosition + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
}
}
}
if (Targets.Count > 0)
{
sb.Append(Targets.Count);
sb.Append("@");
if (!offset || TargetedEnt.HitPosition == null) sb.Append(TargetedEnt.Position + "b" + (Vector3D)TargetedEnt.Velocity + "b" + TargetedEnt.BoundingBox.Min + "b" + TargetedEnt.BoundingBox.Max + "b" + TargetedEnt.EntityId + "b" + "TTARGET" + "b" + TargetedEnt.Orientation.M11 + ":" + TargetedEnt.Orientation.M12 + ":" + TargetedEnt.Orientation.M13 + ":" + TargetedEnt.Orientation.M14 + ":" + TargetedEnt.Orientation.M21 + ":" + TargetedEnt.Orientation.M22 + ":" + TargetedEnt.Orientation.M23 + ":" + TargetedEnt.Orientation.M24 + ":" + TargetedEnt.Orientation.M31 + ":" + TargetedEnt.Orientation.M32 + ":" + TargetedEnt.Orientation.M33 + ":" + TargetedEnt.Orientation.M34 + ":" + TargetedEnt.Orientation.M41 + ":" + TargetedEnt.Orientation.M42 + ":" + TargetedEnt.Orientation.M43 + ":" + TargetedEnt.Orientation.M44);
else sb.Append((Vector3D)TargetedEnt.HitPosition + "b" + (Vector3D)TargetedEnt.Velocity + "b" + TargetedEnt.BoundingBox.Min + "b" + TargetedEnt.BoundingBox.Max + "b" + TargetedEnt.EntityId + "b" + "TTARGET" + "b" + TargetedEnt.Orientation.M11 + ":" + TargetedEnt.Orientation.M12 + ":" + TargetedEnt.Orientation.M13 + ":" + TargetedEnt.Orientation.M14 + ":" + TargetedEnt.Orientation.M21 + ":" + TargetedEnt.Orientation.M22 + ":" + TargetedEnt.Orientation.M23 + ":" + TargetedEnt.Orientation.M24 + ":" + TargetedEnt.Orientation.M31 + ":" + TargetedEnt.Orientation.M32 + ":" + TargetedEnt.Orientation.M33 + ":" + TargetedEnt.Orientation.M34 + ":" + TargetedEnt.Orientation.M41 + ":" + TargetedEnt.Orientation.M42 + ":" + TargetedEnt.Orientation.M43 + ":" + TargetedEnt.Orientation.M44);
if (Targets.Count > 1) sb.Append("@");
for (int i1 = 0; i1 < Targets.Count; i1++)
{
MyDetectedEntityInfo target = Targets[i1];
if (TargetedEnt.EntityId == target.EntityId) continue;
if (!offset || target.HitPosition == null) sb.Append(target.Position + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
else sb.Append((Vector3D)target.HitPosition + "b" + (Vector3D)target.Velocity + "b" + target.BoundingBox.Min + "b" + target.BoundingBox.Max + "b" + target.EntityId + "b" + "TARGET" + "b" + target.Orientation.M11 + ":" + target.Orientation.M12 + ":" + target.Orientation.M13 + ":" + target.Orientation.M14 + ":" + target.Orientation.M21 + ":" + target.Orientation.M22 + ":" + target.Orientation.M23 + ":" + target.Orientation.M24 + ":" + target.Orientation.M31 + ":" + target.Orientation.M32 + ":" + target.Orientation.M33 + ":" + target.Orientation.M34 + ":" + target.Orientation.M41 + ":" + target.Orientation.M42 + ":" + target.Orientation.M43 + ":" + target.Orientation.M44);
if (i1 != Targets.Count - 1) sb.Append("@");
}
}
Me.CustomData = sb.ToString();
sb.Clear();
switch (args)
{
case "MODE": ActiveMode = !ActiveMode; break;
case "SCAN": Scan = !Scan; break;
case "TARGET":
if (!MouseSelector)
{
TargetedEnt = new MyDetectedEntityInfo();
if (SelectedTarget < Targets.Count - 1)
{
SelectedTarget++;
}
else
{
SelectedTarget = 0;
}
}
; break;

case "RESET":
Targets.Clear();
Targets4.Clear();
LTargets.Clear();
TargetedEnt = new MyDetectedEntityInfo();
break;
case "SELECT":
MissileType++;
if (MissileType == 4) MissileType = 0;
break;
// battle mode
case "LAUNCH":
if (RPB != null && MissileType == 1)
{
if (RPB != null && !StatusEnabled || !RdavEn) RPB.Enabled = true;
if (RPB != null && !UseTimersInstead) RPB.TryRun("Fire");
if (UseTimersInstead && RT != null) RT.Trigger();
}
if (MissileType == 2)
{
if (WPB != null && !StatusEnabled || !WhipsEn) WPB.Enabled = true;
if (WPB != null && !UseTimersInstead) WPB.TryRun("fire");
if (UseTimersInstead && WT != null) WT.Trigger();
}
if (EPB != null && MissileType == 3)
{
if (EPB != null && !StatusEnabled || !AlysiusEn) EPB.Enabled = true;
if (EPB != null && !UseTimersInstead) EPB.TryRun("MODE:" + Emode);
if (UseTimersInstead && ET != null) ET.Trigger();
}
if (R4PB != null && MissileType == 0)
{
if (R4PB != null && !StatusEnabled || !R4En) R4PB.Enabled = true;
if (R4PB != null && !UseTimersInstead) R4PB.TryRun("FIRE");
if (UseTimersInstead && R4T != null) R4T.Trigger();
}
; break;
//
case "BROADCAST": BroadcastEn = !BroadcastEn; break;
case "SMALLGRIDA": SAutoLaunch = !SAutoLaunch; break;
case "LARGEGRIDA": LAutoLaunch = !LAutoLaunch; break;
case "FRIENDLYFIRE": FriendlyOff = !FriendlyOff; break;
case "UPDATE": update = true; break;
case "OFFSET": offset = !offset; break;
case "AISCAN": AI_Scanning = !AI_Scanning; break;
case "CONTROLTURR": ControlTurr = !ControlTurr; break;
}
finished = false;
Checked = false;
if (TargetsBroad.Count > 0)
{
for (int i = Targets.Count - 1; i >= 0; i--)
{
MyDetectedEntityInfo target = Targets[i];
foreach (MyDetectedEntityInfo target1 in TargetsBroad)
{
if (target.EntityId == target1.EntityId) { Targets.Remove(target); }
}
}
}
}
}
sb.Clear();
usedcams.Clear();
Targets4.Clear();
for (int i = Targets0.Count - 1; i >= 0; i--)
{
MyDetectedEntityInfo target = Targets0[i];
Targets.Remove(target);
Targets4.Add(target);
}
Targets0.Clear();
}
public void AddAuto(MyDetectedEntityInfo d)
{
double Distance = (d.Position - Me.CubeGrid.GetPosition()).Length();
if (SAutoLaunch && d.BoundingBox.Size.Length() > IgnoredSize && ((d.Type.ToString() == "SmallGrid" && !IDNotType) || (IDNotType && d.BoundingBox.Size.Length() <= SizeS)))
{
string oh = SRate.ToString() + "b" + d.EntityId.ToString() + "b";
if (times.Count <= 0) times.Add(oh);
else
{
bool exist = false;
foreach (string targed in times)
{
string[] targ = targed.Split('b');
long id1;
long.TryParse(targ[1], out id1);
if (id1 == d.EntityId)
{
exist = true;
break;
}
}
if (!exist)
{
times.Add(oh);
}
}
}
if (LAutoLaunch && d.BoundingBox.Size.Length() > IgnoredSize && ((d.Type.ToString() == "LargeGrid" && !IDNotType) || (IDNotType && d.BoundingBox.Size.Length() > SizeS)))
{
string oh = LRate.ToString() + "b" + d.EntityId.ToString() + "b";
if (times.Count <= 0) times.Add(oh);
else
{
bool exist = false;
foreach (string targed in times)
{
string[] targ = targed.Split('b');
long id1;
long.TryParse(targ[1], out id1);
if (id1 == d.EntityId)
{
exist = true;
break;
}
}
if (!exist)
{
times.Add(oh);
}
}
}
}
public void DrawRadar(IMyTextPanel display, List<MyDetectedEntityInfo> targets, int resolution, Vector3D centerPosition, double maxRange, bool drawCircles, bool smoothEdges)
{
// Получаем размер экрана
int width = (int)(resolution * width_coeff);
int height = (int)(resolution * height_coeff);

char[,] radar_draw = new char[width, height];

// Буфер для графики
StringBuilder radarOutput = new StringBuilder();

// Среднее расстояние от центра экрана до максимальной дальности
double maxRangeScaled = maxRange / 2.0;  // Масштабируем до экрана

// Рисуем сглаженные края и круги, если необходимо
for (int y = 0; y < height; y++)
{
for (int x = 0; x < width; x++)
{
// Нормализуем координаты относительно центра
double dx = (x - width / 2.0) / (width / 2.0);
double dy = (y - height / 2.0) / (height / 2.0);
double distanceFromCenter = Math.Sqrt(dx * dx + dy * dy);

if (distanceFromCenter < thickness + 0.05F) { radar_draw[x, y] = center_target; continue; }
// Рисуем круги для дальности
if (DrawCross && (dy == 0 || dx == 0)) radar_draw[x, y] = cross_space; // Рисуем круг на краю экрана
else
if (drawCircles && Circle_1_3 && distanceFromCenter > 0.3 && distanceFromCenter < 0.3 + thickness) radar_draw[x, y] = circle_space; // Рисуем круг на краю экрана
else
if (drawCircles && Circle_2_3 && distanceFromCenter > 0.6 && distanceFromCenter < 0.6 + thickness) radar_draw[x, y] = circle_space; // Рисуем круг на краю экрана
else
if (drawCircles && Circle_3_3 && distanceFromCenter > 0.9 && distanceFromCenter < 0.9 + thickness) radar_draw[x, y] = circle_space; // Рисуем круг на краю экрана
else
if (drawCircles && Circle_1_2 && distanceFromCenter > 0.5 && distanceFromCenter < 0.5 + thickness) radar_draw[x, y] = circle_space; // Рисуем круг на краю экрана
else
if (smoothEdges && distanceFromCenter > 0.9) radar_draw[x, y] = edge_space;  // Мелкие объекты рисуем точкой
else radar_draw[x, y] = blind_space; // Оставляем пустое пространство для неактивного режима
}
//radarOutput.AppendLine();
}

// Перебираем все цели и отображаем их на экране
foreach (var target in targets)
{
// Рассчитываем расстояние и угол
Vector3D targetPosition = target.Position;
double distance = (centerPosition - targetPosition).Length();

// Проверяем, попадает ли цель в радиус действия
if (distance > maxRange) distance = maxRange;

// Рассчитываем угол по отношению к центру экрана
Vector3D directionToTarget = Vector3D.Normalize(targetPosition - centerPosition);
double azimuth; //Math.Atan2(directionToTarget.X, directionToTarget.Z); // Азимут
double elevation; //Math.Asin(directionToTarget.Y); // Элевейшн
azimuth = width / 2;
elevation = height / 2;
azimuth += Vector3D.Dot(display.WorldMatrix.Right, directionToTarget) * (width / 2 * (distance / maxRange));
//if (azimuth > 0) azimuth += width / 2;
//else Math.Abs(azimuth);
elevation += Vector3D.Dot(display.WorldMatrix.Backward, directionToTarget) * (height / 2 * (distance / maxRange));
if (Debug) Echo("a " + azimuth);
if (Debug) Echo("e " + elevation);
//if (elevation < 0) Math.Abs(elevation -= height / 2);

// Преобразуем угол в координаты экрана
int x = (int)azimuth; //(int)((azimuth / Math.PI) * 0.5 * width + 0.5 * width); // Преобразуем азимут в горизонтальную координату
int y = (int)elevation; //(int)((elevation / (Math.PI / 2)) * 0.5 * height + 0.5 * height); // Преобразуем элевейшн в вертикальную координату

// Учитываем, чтобы координаты не выходили за пределы экрана
x = Math.Max(0, Math.Min(x, width - 1));
y = Math.Max(0, Math.Min(y, height - 1));

// Записываем символ в буфер графики 
if (Debug) Echo("x " + x);
if (Debug) Echo("y " + y);
if (target.EntityId == TargetedEnt.EntityId) radar_draw[x, y] = selected_target; // Вставляем точку для цели
else
if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies) radar_draw[x, y] = enemy_target; // Вставляем точку для цели
else
if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Friends) radar_draw[x, y] = friendly_target; // Вставляем точку для цели
else
if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Neutral) radar_draw[x, y] = neutral_target; // Вставляем точку для цели
else
if (target.Relationship == MyRelationsBetweenPlayerAndBlock.Owner) radar_draw[x, y] = owner_target; // Вставляем точку для цели
else
radar_draw[x, y] = unknown_target; // Вставляем точку для цели

}

for (int Y = 0; Y < height; Y++)
{
for (int X = 0; X < width; X++) radarOutput.Append(radar_draw[X, Y]);
radarOutput.AppendLine();
}

// Рисуем изображение на дисплее
display.Font = "Monospace";
display.TextPadding = 0;
display.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
display.WriteText(radarOutput.ToString());
}
//