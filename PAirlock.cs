// Necessary blocks for the script to work:
// 1) external door,
// 2) internal door,
// 3) external sensor (in front of the external door),
// 4) internal sensor (in front of the internal door),
// 5) airlock sensor (inside the airlock, between two doors),
// 6) airlock ventilation (between two doors).
//
// The names of the blocks in the strict order specified above, separated by commas,
// without spaces before the names, must be specified in the program block arguments field.
//
// The zones of outside sensors must reach the doors closely,
// the zone of the airlock sensor must be within the airlock.
// Gaps between zones are not acceptable.
//
// On servers that disable program blocks in the absence of the player,
// it is recommended to link the activation of the program block to a sensor configured to
// recognize the player and launch the program block with the specified arguments again.

// Необходимые блоки для работы скрипта:
// 1) внешняя дверь,
// 2) внутренняя дверь,
// 3) внешний сенсор (перед внешней дверью),
// 4) внутренний сенсор (перед внутренней дверью),
// 5) сенсор шлюза (внутри шлюза, между двумя дверьми),
// 6) вентиляция шлюза (между двумя дверьми).
//
// Названия блоков в строгом порядке, указанном выше, через запятую, без пробелов перед именами,
// должны быть заданы в поле аргументов программного блока.
//
// Зоны наружных сенсоров должны доходить до дверей вплотную,
// зона сенсора шлюза должна быть в пределах шлюза.
// Зазоры между зонами не допустимы.
//
// На серверах, которые отключают программные блоки в отсутствии игрока
// рекомендуется привязать включение программного блока к сенсору, настроенному на распознавание игрока и
// запускающему программный блок с заданными аргументами заново.

//== CONSTANTS FOR ADJUSTING. \ КОНСТАНТЫ ДЛЯ НАСТРОЙКИ. ==/
private const int iSyncAffectedBlocks = 50; // Cycles before update list of affected blocks. \ Циклы до обновления списка блоков.
private const int iAtmoLagCorrection = 6; // Cycles to consider lag in atmospheric exchange. \ Циклы для учёта лага обмена атмосферы.
private const int iCyclesToAbortPressurizeOperations = 20; // Cycles before abort preSs. ops. \ Циклы до отмены пресс. операций.

//== SCRIPT. \ СКРИПТ. ==//
private IMyDoor doorExternal;
private IMyDoor doorInternal;
private IMySensorBlock sensorExternal;
private IMySensorBlock sensorInternal;
private IMySensorBlock sensorAirlock;
private IMyAirVent airVent;
private string strLastArgument = "";
private List<string> args;
private const string strArgListErrorPref = "Argument list error, ";
private bool bLastSensorExternalSatate = false;
private bool bLastSensorInternalSatate = false;
private bool bLastSensorAirlockSatate = false;
private int iAtmoLag = iAtmoLagCorrection;
private float fLastOLevel = -1.0f;
private int iSyncAffectedBlocksCounter = 0;
private int iToAbortPO = iCyclesToAbortPressurizeOperations;
private readonly List<ChangeStateToDirection> lstDirections;
private Direction direction = Direction.IntToOutside;


// Направления.
enum Direction
{
    Unknown, ExtToOutside, OutsideToExt, ExtToAirlock, AirlockToExt, IntToAirlock, AirlockToInt, OutsideToInt, IntToOutside
}

// Варианты статуса сенсора.
enum Ss
{
    IGN, OFF, ON
}

// Варианты статуса двери.
enum Ds
{
    EOIC, IOEC, ANY
}

//= КЛАСС СТАТУСА СЕНСОРОВ. =//
class States
{
    public States() { }
    public States(Ss externalSensor, Ss airlockSensor, Ss internalSensor)
    {
        extS = externalSensor; airS = airlockSensor; intS = internalSensor;
    }
    public Ss extS;
    public Ss airS;
    public Ss intS;
}

//= КЛАСС ПЕРЕДАЧИ ОБЩЕГО СТАТУСА И СООТВ. НАПРАВЛЕНИЯ. =//
class ChangeStateToDirection
{
    public ChangeStateToDirection() { }
    public ChangeStateToDirection(States beforeState, States nowState, Ds doorsStates, Direction direction)
    {
        before = beforeState; now = nowState; dir = direction; doors = doorsStates;
    }
    public States before;
    public States now;
    public Ds doors;
    public Direction dir;
}

//= ФУНКЦИЯ ВКЛ.\ВЫКЛ. БЛОКА =//
bool ToggleBlock(IMyTerminalBlock block, bool bOn)
{
    List<ITerminalAction> lstActions = new List<ITerminalAction>();
    block.GetActions(lstActions);
    foreach (ITerminalAction action in lstActions)
    {
        string strName = action.Name.ToString();
        if (bOn && strName == "Toggle block On")
        {
            action.Apply(block);
            return true;
        }
        else if (!bOn && strName == "Toggle block Off")
        {
            action.Apply(block);
            return true;
        }
    }
    return false;
}

//= ФУНКЦИЯ УПРАВЛЕНИЯ ДВЕРЬМИ ПО СИТУАЦИИ. =//
void SetDoorState(ref IMyDoor door, bool bOpen)
{
    if (bOpen)
    {
        if ((door.Status == DoorStatus.Closed) || (door.Status == DoorStatus.Closing)) door.OpenDoor();
    }
    else
    {
        if ((door.Status == DoorStatus.Open) || (door.Status == DoorStatus.Opening)) door.CloseDoor();
    }
}

//= ФУНКЦИЯ ОПРЕДЕЛЕНИЯ НАПРАВЛЕНИЯ ДВИЖЕНИЯ ПО ОБЛАСТИ ШЛЮЗА. =//
Direction GetDirection(ref ChangeStateToDirection changes)
{
    Direction result = Direction.Unknown;
    foreach (ChangeStateToDirection stateTemplate in lstDirections)
    {
        bool bSame = false;
        if (stateTemplate.before.extS == Ss.IGN || stateTemplate.before.extS == changes.before.extS)
        {
            if (stateTemplate.before.airS == Ss.IGN || stateTemplate.before.airS == changes.before.airS)
            {
                if (stateTemplate.before.intS == Ss.IGN || stateTemplate.before.intS == changes.before.intS)
                {
                    bSame = true;
                }
            }
        }
        if (!bSame) continue;
        bSame = false;
        if (stateTemplate.now.extS == Ss.IGN || stateTemplate.now.extS == changes.now.extS)
        {
            if (stateTemplate.now.airS == Ss.IGN || stateTemplate.now.airS == changes.now.airS)
            {
                if (stateTemplate.now.intS == Ss.IGN || stateTemplate.now.intS == changes.now.intS)
                {
                    bSame = true;
                }
            }
        }
        if (!bSame) continue;
        bSame = false;
        if (stateTemplate.doors == Ds.ANY || stateTemplate.doors == changes.doors)
        {
            bSame = true;
        }
        if (!bSame) continue;
        return stateTemplate.dir;
    }
    return result;
}

//= ОСНОВНАЯ ИНИЦИАЛИЗАЦИЯ. =//
Program()
        {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    lstDirections = new List<ChangeStateToDirection>();
    // ЗАХОДЫ И ВЫХОДЫ НА ОБЛАСТЬ ШЛЮЗА.
    // Заход снаружи на внешний.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.OFF, Ss.OFF, Ss.OFF),
        new States(Ss.ON, Ss.OFF, Ss.OFF),
        Ds.ANY,
        Direction.OutsideToExt));
    // Выход с внешнего наружу.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.ON, Ss.OFF, Ss.OFF),
        new States(Ss.OFF, Ss.OFF, Ss.OFF),
        Ds.ANY,
        Direction.ExtToOutside));
    // Внешний уступает вход внутреннему.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.ON, Ss.OFF, Ss.ON),
        new States(Ss.OFF, Ss.OFF, Ss.ON),
        Ds.EOIC,
        Direction.OutsideToInt));
    // Заход снаружи на внутренний.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.OFF, Ss.OFF, Ss.OFF),
        new States(Ss.OFF, Ss.OFF, Ss.ON),
        Ds.ANY,
        Direction.OutsideToInt));
    // Выход с внутреннего наружу.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.OFF, Ss.OFF, Ss.ON),
        new States(Ss.OFF, Ss.OFF, Ss.OFF),
        Ds.ANY,
        Direction.IntToOutside));
    // Внутренний уступает вход внешнему.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.ON, Ss.OFF, Ss.ON),
        new States(Ss.ON, Ss.OFF, Ss.OFF),
        Ds.IOEC,
        Direction.OutsideToExt));
    // ВНЕШНИЕ ПЕРЕМЕЩЕНИЯ.
    // Заход снаружи на внешний.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.OFF, Ss.OFF, Ss.IGN),
        new States(Ss.ON, Ss.OFF, Ss.IGN),
        Ds.EOIC,
        Direction.OutsideToExt));
    // Выход с внешнего наружу.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.ON, Ss.OFF, Ss.IGN),
        new States(Ss.OFF, Ss.OFF, Ss.IGN),
        Ds.EOIC,
        Direction.ExtToOutside));
    // Заход снаружи на внутренний.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.IGN, Ss.OFF, Ss.OFF),
        new States(Ss.IGN, Ss.OFF, Ss.ON),
        Ds.IOEC,
        Direction.OutsideToInt));
    // Выход с внутреннего наружу.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.IGN, Ss.OFF, Ss.ON),
        new States(Ss.IGN, Ss.OFF, Ss.OFF),
        Ds.IOEC,
        Direction.IntToOutside));
    // ВНУТРЕННИЕ ПЕРЕМЕЩЕНИЯ.
    // Заход с внешнего в шлюз.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.ON, Ss.IGN, Ss.IGN),
        new States(Ss.OFF, Ss.ON, Ss.IGN),
        Ds.EOIC,
        Direction.ExtToAirlock));
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.ON, Ss.OFF, Ss.IGN),
        new States(Ss.IGN, Ss.ON, Ss.IGN),
        Ds.EOIC,
        Direction.ExtToAirlock));
    // Выход со шлюза на внешний.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.IGN, Ss.ON, Ss.IGN),
        new States(Ss.ON, Ss.OFF, Ss.IGN),
        Ds.EOIC,
        Direction.AirlockToExt));
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.OFF, Ss.ON, Ss.IGN),
        new States(Ss.ON, Ss.IGN, Ss.IGN),
        Ds.EOIC,
        Direction.AirlockToExt));
    // Заход с внутреннего в шлюз.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.IGN, Ss.IGN, Ss.ON),
        new States(Ss.IGN, Ss.ON, Ss.OFF),
        Ds.IOEC,
        Direction.IntToAirlock));
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.IGN, Ss.OFF, Ss.ON),
        new States(Ss.IGN, Ss.ON, Ss.IGN),
        Ds.IOEC,
        Direction.IntToAirlock));
    // Выход со шлюза на внутренний.
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.IGN, Ss.ON, Ss.IGN),
        new States(Ss.IGN, Ss.OFF, Ss.ON),
        Ds.IOEC,
        Direction.AirlockToInt));
    lstDirections.Add(new ChangeStateToDirection(
        new States(Ss.IGN, Ss.ON, Ss.OFF),
        new States(Ss.IGN, Ss.IGN, Ss.ON),
        Ds.IOEC,
        Direction.AirlockToInt));
}

//= ОСНОВНОЙ ЦИКЛ. =//
void Main(string strArgument)
{
    if (iSyncAffectedBlocksCounter == 0)
    {
        if (strArgument != "") strLastArgument = strArgument;
        args = strLastArgument.Split(',').ToList();
        if (args.Count != 6) // Только 6 агрументов.
        {
            Echo(strArgListErrorPref + "need 6 args.");
            Echo("Current arguments count: " + args.Count);
            Echo("Current args string: " + strLastArgument);
            return;
        }
        // Проверка существования заданных блоков в гриде.
        bool bWrongArgs = false;
        doorExternal = GridTerminalSystem.GetBlockWithName(args[0]) as IMyDoor;
        if (doorExternal == null)
        {
            Echo(strArgListErrorPref + "can`t get external door block.");
            bWrongArgs = true;
        }
        doorInternal = GridTerminalSystem.GetBlockWithName(args[1]) as IMyDoor;
        if (doorInternal == null)
        {
            Echo(strArgListErrorPref + "can`t get internal door block.");
            bWrongArgs = true;
        }
        sensorExternal = GridTerminalSystem.GetBlockWithName(args[2]) as IMySensorBlock;
        if (sensorExternal == null)
        {
            Echo(strArgListErrorPref + "can`t get external sensor block.");
            bWrongArgs = true;
        }
        sensorInternal = GridTerminalSystem.GetBlockWithName(args[3]) as IMySensorBlock;
        if (sensorInternal == null)
        {
            Echo(strArgListErrorPref + "can`t get internal sensor block.");
            bWrongArgs = true;
        }
        sensorAirlock = GridTerminalSystem.GetBlockWithName(args[4]) as IMySensorBlock;
        if (sensorAirlock == null)
        {
            Echo(strArgListErrorPref + "can`t get airlock sensor block.");
            bWrongArgs = true;
        }
        airVent = GridTerminalSystem.GetBlockWithName(args[5]) as IMyAirVent;
        if (airVent == null)
        {
            Echo(strArgListErrorPref + "can`t get airvent block.");
            bWrongArgs = true;
        }
        if (bWrongArgs) return;
        iSyncAffectedBlocksCounter = iSyncAffectedBlocks;
        Echo("Affected blocks sync OK.");
    }
    else iSyncAffectedBlocksCounter--;

    // Инициализация.
    if (fLastOLevel == -1.0f)
    {
        fLastOLevel = (float)Math.Round(airVent.GetOxygenLevel() * 100.0f);
        ToggleBlock(doorInternal, true);
        ToggleBlock(doorExternal, true);
        ToggleBlock(sensorExternal, true);
        ToggleBlock(sensorInternal, true);
        ToggleBlock(sensorAirlock, true);
        ToggleBlock(airVent, true);
        SetDoorState(ref doorExternal, false);
        SetDoorState(ref doorInternal, false);
    }
    bool bExtSensor = sensorExternal.IsActive;
    bool bAirLockSensor = sensorAirlock.IsActive;
    bool bIntSensor = sensorInternal.IsActive;
    bool bVentState = airVent.PressurizationEnabled;

    // ОПРЕДЕЛЕНИЕ НАПРАВЛЕНИЙ.
    // Если есть изменения в сенсорах с прошлого цикла - определение нового направления.
    if ((bLastSensorExternalSatate != bExtSensor) ||
        (bLastSensorAirlockSatate != bAirLockSensor) ||
        (bLastSensorInternalSatate != bIntSensor))
    {
        ChangeStateToDirection states = new ChangeStateToDirection();
        states.before = new States();
        states.now = new States();
        if (bLastSensorExternalSatate) states.before.extS = Ss.ON; else states.before.extS = Ss.OFF;
        if (bLastSensorAirlockSatate) states.before.airS = Ss.ON; else states.before.airS = Ss.OFF;
        if (bLastSensorInternalSatate) states.before.intS = Ss.ON; else states.before.intS = Ss.OFF;
        if (bExtSensor) states.now.extS = Ss.ON; else states.now.extS = Ss.OFF;
        if (bAirLockSensor) states.now.airS = Ss.ON; else states.now.airS = Ss.OFF;
        if (bIntSensor) states.now.intS = Ss.ON; else states.now.intS = Ss.OFF;

        // Упрощённые статусы конкретных дверей.
        bool bIntDoorOveralStatus, bExtDoorOveralStatus;
        if ((doorInternal.Status == DoorStatus.Closed) || (doorInternal.Status == DoorStatus.Closing)) bIntDoorOveralStatus = false;
        else bIntDoorOveralStatus = true;
        if ((doorExternal.Status == DoorStatus.Closed) || (doorExternal.Status == DoorStatus.Closing)) bExtDoorOveralStatus = false;
        else bExtDoorOveralStatus = true;

        // Статус состояния дверей шлюза.
        if (bExtDoorOveralStatus && !bIntDoorOveralStatus) states.doors = Ds.EOIC;
        else if (!bExtDoorOveralStatus && bIntDoorOveralStatus) states.doors = Ds.IOEC;
        else states.doors = Ds.ANY;
        //
        Direction newDirection = GetDirection(ref states);
        if (newDirection != Direction.Unknown) direction = newDirection;
    }

    // Работа по направлениям.
    switch (direction)
    {
        case Direction.OutsideToExt:
            Echo("Direction: OutsideToExt");
            SetDoorState(ref doorInternal, false);
            airVent.Depressurize = true;
            break;
        case Direction.OutsideToInt:
            Echo("Direction: OutsideToInt");
            SetDoorState(ref doorExternal, false);
            airVent.Depressurize = false;
            break;
        case Direction.ExtToAirlock:
            Echo("Direction: ExtToAirlock");
            SetDoorState(ref doorExternal, false);
            airVent.Depressurize = false;
            break;
        case Direction.IntToAirlock:
            Echo("Direction: IntToAirlock");
            SetDoorState(ref doorInternal, false);
            airVent.Depressurize = true;
            break;
        case Direction.AirlockToExt:
            Echo("Direction: AirlockToExt"); // Просто чтобы не Unknown.
            break;
        case Direction.AirlockToInt:
            Echo("Direction: AirlockToInt"); // Просто чтобы не Unknown.
            break;
        case Direction.ExtToOutside:
            Echo("Direction: ExtToOutside");
            SetDoorState(ref doorExternal, false);
            break;
        case Direction.IntToOutside:
            Echo("Direction: IntToOutside");
            SetDoorState(ref doorInternal, false);
            break;
        default:
            Echo("Direction: Unknown");
            break;
    }

    // Реакция на давление в шлюзе.
    float fOLevel = (float)Math.Round(airVent.GetOxygenLevel() * 100.0f); // Умножение на 100 для округления.
    if ((direction != Direction.ExtToOutside) && (direction != Direction.IntToOutside))
    {
        // Прерывание операций в шлюзе по достижении максимального времени ожидания завершения.
        bool bToDepressurize = true;
        if (airVent.Depressurize && (fOLevel > 1.0f))
        {
            iToAbortPO--;
            bToDepressurize = true;
        }
        else if (!airVent.Depressurize && (fOLevel < 99.0f))
        {
            iToAbortPO--;
            bToDepressurize = false;
        }
        else iToAbortPO = iCyclesToAbortPressurizeOperations;
        if (iToAbortPO == 0)
        {
            if (bToDepressurize)
            {
                Echo("Forced external door open.");
                if ((doorInternal.Status != DoorStatus.Open) || (doorInternal.Status != DoorStatus.Opening))
                    SetDoorState(ref doorExternal, true);
            }
            else
            {
                Echo("Forced internal door open.");
                if ((doorExternal.Status != DoorStatus.Open) || (doorExternal.Status != DoorStatus.Opening))
                    SetDoorState(ref doorInternal, true);
            }
            iToAbortPO = iCyclesToAbortPressurizeOperations;
        }

        // Работа с текущим уровнем кислорода.
        if (fLastOLevel != fOLevel)
        {
            iAtmoLag = iAtmoLagCorrection;
            Echo("Pressure unstable.");
        }
        if (iAtmoLag == 0)
        {
            if ((fOLevel > 99.0f) && (direction != Direction.IntToAirlock) && (direction != Direction.OutsideToExt))
            {
                Echo("Pressurized.");
                if ((direction == Direction.OutsideToInt) || (direction == Direction.ExtToAirlock) &&
                    (doorExternal.Status != DoorStatus.Open) || (doorExternal.Status != DoorStatus.Opening))
                    SetDoorState(ref doorInternal, true);
            }
            else if ((fOLevel < 1.0f) && (direction != Direction.ExtToAirlock) && (direction != Direction.OutsideToInt))
            {
                Echo("Depressurized.");
                if ((direction == Direction.OutsideToExt) || (direction == Direction.IntToAirlock) &&
                    (doorInternal.Status != DoorStatus.Open) || (doorInternal.Status != DoorStatus.Opening))
                    SetDoorState(ref doorExternal, true);
            }
        }
        else
        {
            iAtmoLag--;
            Echo("Waiting for pressure stabilization.");
        }
    }

    // Завершение цикла.
    fLastOLevel = fOLevel;
    bLastSensorExternalSatate = bExtSensor;
    bLastSensorInternalSatate = bIntSensor;
    bLastSensorAirlockSatate = bAirLockSensor;
    Echo("Ok.");
}