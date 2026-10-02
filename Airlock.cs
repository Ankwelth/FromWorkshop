/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

SEUtils _seu;
Airlock[] airlocks;

public Program()
{
    _seu = new SEUtils(this);
    List<IMyDoor> allDoors = new List<IMyDoor>();
    GridTerminalSystem.GetBlocksOfType(allDoors, _seu.IsInGrid);
    Dictionary<string, IMyDoor> singleDoors = new Dictionary<string, IMyDoor>();
    List<Airlock> airlocks = new List<Airlock>();
    foreach (var door in allDoors)
    {
        if (!string.IsNullOrWhiteSpace(door.CustomData) && door.CustomData.Contains("airlock_"))
        {
            string id = door.CustomData.Replace("airlock_", "");
            if (singleDoors.ContainsKey(id))
            {
                Airlock newAirlock = new Airlock();
                newAirlock.doors = new IMyDoor[2];
                newAirlock.doors[0] = singleDoors[id];
                newAirlock.doors[1] = door;
                airlocks.Add(newAirlock);
                singleDoors.Remove(id);
            }
            else
            {
                singleDoors.Add(id, door);
            }
        }
    }
    this.airlocks = airlocks.ToArray();
    Echo("trauni's airlock script");
    Echo("Total doors: " + allDoors.Count);
    Echo("Single airlock doors: " + singleDoors.Count);
    Echo("Successfully recogized airlocks: " + this.airlocks.Length);
}

public void Save()
{

}

public void Main(string argument, UpdateType updateSource)
{
    if (!_seu.RuntimeUpdate(argument, updateSource)) return;
    for (int i = 0; i < airlocks.Length; i++)
    {
        if (!airlocks[i].running && airlocks[i].doors.Any(x => x.OpenRatio != 0))
        {
            airlocks[i].running = true;
            _seu.StartCoroutine(PerformAirlock(airlocks[i]));
        }
    }
}

public IEnumerator PerformAirlock(Airlock airlock)
{
    IMyDoor initialDoor = airlock.doors.First(x => x.OpenRatio != 0);
    IMyDoor otherDoor = airlock.doors.First(x => x.EntityId != initialDoor.EntityId);
    otherDoor.Enabled = false;
    yield return new WaitForConditionMet(() => initialDoor.OpenRatio == 1, 5000, -1, () => true);
    yield return new WaitForMilliseconds(2000);
    initialDoor.CloseDoor();
    yield return new WaitForConditionMet(() => { if (initialDoor.Status != DoorStatus.Closing) initialDoor.CloseDoor(); return initialDoor.OpenRatio == 0; }, 30000, 500, () => false);
    yield return new WaitForMilliseconds(100);
    initialDoor.Enabled = false;
    otherDoor.Enabled = true;
    otherDoor.OpenDoor();
    yield return new WaitForConditionMet(() => otherDoor.OpenRatio == 1, 5000, -1, () => true);
    yield return new WaitForMilliseconds(2000);
    otherDoor.CloseDoor();
    yield return new WaitForConditionMet(() => { if (otherDoor.Status != DoorStatus.Closing) otherDoor.CloseDoor(); return otherDoor.OpenRatio == 0; }, 30000, 500, () => false);
    initialDoor.Enabled = true;
    airlock.running = false;
}

public class SEUtils
{
    /// <summary>
            /// The PB that is executing this script
            /// </summary>
    public IMyProgrammableBlock CurrentProgrammableBlock;
    /// <summary>
            /// The CubeGrid the CurrentProgrammableBlock is located in
            /// </summary>
    public IMyCubeGrid CurrentCubeGrid;

    private IMyGridTerminalSystem GridTerminalSystem;
    private MyGridProgram CurrentMyGridProgram;
    private List<Action> invokeNextUpdateActions;
    private List<WaitingInvokeInfo> invokeTimeActions;
    private char[] icons = new char[] { '|', '/', '-', '\\' };
    private int iconIndex = 0;
    private DateTime start;
    private IMyTextSurface pbLcd;
    private string name = "";
    private bool setupDone = false;
    private Dictionary<int, IEnumerator> coroutines;
    private int coroutineCounter = 0;
    private UpdateFrequency updateFreq;

    /// <summary>
            /// Performs the setup, that is required for SEUtils to work
            /// </summary>
            /// <param name="scriptBaseClass">Your script class</param>
            /// <param name="updateFrequency">Your desired update frequency</param>
            /// <param name="statusDisplay">Whether SEUtils should display a simple status on the PB's screen</param>
            /// <param name="scriptName">The name of your script</param>
    public SEUtils(MyGridProgram scriptBaseClass, UpdateFrequency updateFrequency = UpdateFrequency.Update10, bool statusDisplay = true, string scriptName = "Script")
    {
        updateFreq = updateFrequency;
        coroutines = new Dictionary<int, IEnumerator>();
        invokeNextUpdateActions = new List<Action>();
        invokeTimeActions = new List<WaitingInvokeInfo>();
        CurrentMyGridProgram = scriptBaseClass;
        GridTerminalSystem = CurrentMyGridProgram.GridTerminalSystem;
        CurrentProgrammableBlock = CurrentMyGridProgram.Me;
        CurrentCubeGrid = CurrentProgrammableBlock.CubeGrid;

        name = scriptName;

        CurrentMyGridProgram.Runtime.UpdateFrequency = updateFreq;
        setupDone = true;

        if (statusDisplay)
        {
            pbLcd = CurrentProgrammableBlock.GetSurface(0);
            pbLcd.ContentType = ContentType.TEXT_AND_IMAGE;
            UpdatePBScreen();
        }

        start = DateTime.Now;
    }

    private void CheckSetup()
    {
        if (!setupDone)
        {
            throw new Exception("Please execute the 'SEUtils.Setup' method in the script constructor");
        }
    }

    private void UpdatePBScreen()
    {
        string uptimeDisplay = "Uptime: " + (DateTime.Now - start).ToString();
        iconIndex++;
        if (iconIndex > icons.Length - 1)
        {
            iconIndex = 0;
        }

        pbLcd.WriteText(name + (!name.ToLower().Contains("script") ? " script" : "") + " is running " + icons[iconIndex] + "\n" + uptimeDisplay);
        Invoke(UpdatePBScreen, 1000);
    }

    private void WaitingCoroutineStep(WaitForConditionMet conditionChecker, int enumeratorId)
    {
        if (!coroutines.ContainsKey(enumeratorId))
        {
            return;
        }
        var enumerator = coroutines[enumeratorId];
        if (conditionChecker.condition())
        {
            CoroutineStep(enumeratorId);
        }
        else
        {
            if (conditionChecker.timeout != -1 && (DateTime.Now - conditionChecker.started).TotalMilliseconds >= conditionChecker.timeout)
            {
                if (conditionChecker.timeoutAction != null)
                {
                    if (conditionChecker.timeoutAction())
                    {
                        InvokeNextTick(() => CoroutineStep(enumeratorId));
                    }
                    else
                    {
                        StopCoroutine(enumeratorId);
                    }
                }
                else
                {
                    InvokeNextTick(() => CoroutineStep(enumeratorId));
                }
            }
            if (conditionChecker.checkInterval == -1)
            {
                InvokeNextTick(() => WaitingCoroutineStep(conditionChecker, enumeratorId));
            }
            else
            {
                Invoke(() => WaitingCoroutineStep(conditionChecker, enumeratorId), conditionChecker.checkInterval);
            }
        }
    }

    private void CoroutineStep(int enumeratorId)
    {
        if (!coroutines.ContainsKey(enumeratorId))
        {
            return;
        }
        var enumerator = coroutines[enumeratorId];
        if (enumerator.MoveNext())
        {
            var waitInstruction = enumerator.Current;

            if (waitInstruction is WaitForNextTick)
            {
                InvokeNextTick(() => CoroutineStep(enumeratorId));
            }
            else if (waitInstruction is WaitForMilliseconds)
            {
                var milliseconds = (waitInstruction as WaitForMilliseconds).milliseconds;
                Invoke(() => CoroutineStep(enumeratorId), milliseconds);
            }
            else if (waitInstruction is WaitForConditionMet)
            {
                InvokeNextTick(() => WaitingCoroutineStep(waitInstruction as WaitForConditionMet, enumeratorId));
            }
            else
            {
                CurrentMyGridProgram.Echo("Unknown coroutine waiting instruction");
            }
        }
        else
        {
            coroutines.Remove(enumeratorId);
        }
    }

    /// <summary>
            /// Starts the given coroutine and returns the id of the coroutine's instance
            /// </summary>
            /// <param name="coroutine">Coroutine to start</param>
            /// <returns>Id of the coroutine's instance</returns>
    public int StartCoroutine(IEnumerator coroutine)
    {
        int id = coroutineCounter;
        coroutineCounter++;
        coroutines.Add(id, coroutine);
        CoroutineStep(id);
        return id;
    }

    /// <summary>
            /// Stops a coroutine-instance if present and returns if the instance was found and stopping was successful
            /// </summary>
            /// <param name="coroutineInstanceId">The coroutine to stop</param>
            /// <returns>if the instance was found and stopping was successful</returns>
    public bool StopCoroutine(int coroutineInstanceId)
    {
        return coroutines.Remove(coroutineInstanceId);
    }

    /// <summary>
            /// Checks wheter the coroutine with the given id is running
            /// </summary>
            /// <param name="coroutineInstanceId">The coroutine to check</param>
            /// <returns>if the instance was found</returns>
    public bool CheckCoroutineRunning(int coroutineInstanceId)
    {
        return coroutines.ContainsKey(coroutineInstanceId);
    }


    /// <summary>
            /// Checks if the given block is on the same grid as the current PB
            /// </summary>
            /// <param name="block">Block to check the grid on</param>
            /// <returns>if the block is on the same grid as the current PB</returns>
    public bool IsInGrid(IMyTerminalBlock block)
    {
        CheckSetup();
        return block.CubeGrid == CurrentCubeGrid;
    }

    /// <summary>
            /// Invokes the given action to be executed in the next game tick
            /// </summary>
            /// <param name="action">Action to invoke</param>
    public void InvokeNextTick(Action action)
    {
        CheckSetup();
        CurrentMyGridProgram.Runtime.UpdateFrequency = UpdateFrequency.Update1 | updateFreq;
        invokeNextUpdateActions.Add(action);
    }

    /// <summary>
            /// Invokes the given action to be executed after the given milliseconds passed
            /// </summary>
            /// <param name="action">Action to execute</param>
            /// <param name="milliseconds">Milliseconds to wait</param>
    public void Invoke(Action action, int milliseconds)
    {
        CheckSetup();
        invokeTimeActions.Add(new WaitingInvokeInfo(action, DateTime.Now.AddMilliseconds(milliseconds)));
    }

    /// <summary>
            /// It is necessary that you call this method in your Main method at the beginning.
            /// Only execute your script's code if this method returns true.
            /// </summary>
            /// <param name="argument">The parameter 'argument' that is passed to your Main method</param>
            /// <param name="updateSource">The parameter 'updateSource' that is passed to your Main method</param>
            /// <returns>If you should execute your code</returns>
    public bool RuntimeUpdate(string argument, UpdateType updateSource)
    {
        string finished = "Start";
        try
        {
            CheckSetup();
            if ((updateSource & (UpdateType.Update1 | UpdateType.Update10 | UpdateType.Update100)) != 0)
            {
                var nextUp = invokeNextUpdateActions.ToArray();
                invokeNextUpdateActions.Clear();
                foreach (var item in nextUp)
                {
                    item();
                }
                finished = "Executing 'next tick invokes'";
                var actions = invokeTimeActions.Where(x => DateTime.Now >= x.datetime).Select(x => x.action).ToList();
                foreach (var item in actions)
                {
                    item();
                }
                invokeTimeActions = invokeTimeActions.Where(x => DateTime.Now < x.datetime).ToList();
                if (invokeTimeActions.Any(x => (DateTime.Now - x.datetime).TotalMilliseconds < 10 * (1000 / 60)))
                {
                    CurrentMyGridProgram.Runtime.UpdateFrequency = UpdateFrequency.Update1 | updateFreq;
                }
                finished = "Executing 'time invokes'";
            }

            if ((updateSource & (UpdateType.Trigger | UpdateType.Terminal | UpdateType.Script | UpdateType.IGC | UpdateType.Once | (UpdateType)(((int)updateFreq) * 32))) != 0) // updateFrequency to UpdateType
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            pbLcd.ContentType = ContentType.TEXT_AND_IMAGE;
            pbLcd.WriteText("Exception in SEUtils caught after " +  finished + ": " + ex.Message + "\n" + ex.StackTrace);
            CurrentMyGridProgram.Echo(ex.GetType() + " in SEUtils caught after " +  finished + ": " + ex.Message + "\n" + ex.StackTrace);
            throw ex;
        }
    }
}

private class WaitingInvokeInfo
{
    public Action action;
    public DateTime datetime;

    public WaitingInvokeInfo(Action action, DateTime datetime)
    {
        this.action = action;
        this.datetime = datetime;
    }
}

/// <summary>
        /// Waits for the next game tick and continues the coroutine
        /// </summary>
public class WaitForNextTick
{

}

/// <summary>
        /// Waits for the given milliseconds to pass
        /// </summary>
public class WaitForMilliseconds
{
    public int milliseconds;

    public WaitForMilliseconds(int milliseconds)
    {
        this.milliseconds = milliseconds;
    }
}

/// <summary>
        /// Waits at least for the next game tick, after that waits for the given condition to be true
        /// If timeout is specified, after the timeout passed and there is a timeoutAction specified, the timeoutAction will be executed, if timeoutAction returns true, coroutine continues, otherwise terminates
        /// </summary>
public class WaitForConditionMet
{
    public Func<bool> condition;

    /// <summary>
            /// Waits for the given condition to be true, but waits at least for the next game tick
            /// </summary>
            /// <param name="action">Action that evaluates your condition</param>
            /// <param name="timeoutMilliseconds">Timeout; -1 for none; Coroutine continues after timeout has passed, if no timeoutAction is passed</param>
            /// <param name="checkIntervalMilliseconds">Delay to wait between the condition checks; -1 for none</param>
            /// <param name="timeoutFunc">Func to execute when timeout passed; If null, coroutine continues after timeout; If func returns true, coroutine continues, otherwise terminates</param>
    public WaitForConditionMet(Func<bool> action, int timeoutMilliseconds = -1, int checkIntervalMilliseconds = -1, Func<bool> timeoutFunc = null)
    {
        condition = action;
        timeout = timeoutMilliseconds;
        checkInterval = checkIntervalMilliseconds;
        this.timeoutAction = timeoutFunc;
        started = DateTime.Now;
    }

    public Func<bool> timeoutAction { get; set; }
    public DateTime started { get; set; }
    public int checkInterval { get; set; }
    public int timeout { get; set; }
}

}

public class Airlock
{
    public IMyDoor[] doors;
    public bool running = false;