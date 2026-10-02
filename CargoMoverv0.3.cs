/*
 * R e a d m e
 * -----------
 * 
 * Script to load cargo to ship containers from other grids containers then back off
 *  - periodically checks if ship is connected to a grid by a Connector
 *  - when ship is connected 
 *       - looks for an instruction flag on the new connection to see what needs to be done
 *       - lists all cargo containers
 *       - any inventory on same grid as Programable Block can be destination
 *            - or limited to only cargo containers by a setting
 *       - any container on different grid can be the source or limited to specific blocks
 *            - by way of flags
 *       - transfer as much cargo as possible
 *       - this is done only once 
 *            - resets when when all ship connectors are disconnected
 * 
 * MUST Recompile to load any changes in Custom Data
 * 
 * //----------------------------------------------------------------------------------
 * // Version Notes:
 * //
 * // v0.1 - Initial Build
 * //
 * // v0.2 - Added Timers to allow for delayed actions after a transfer in case of 
 * //          not filling the cargo hold completely and SAM would just hang and wait
 * //          til this condision was met
 * //
 * // v0.3 - Making the task of moving cargo be spread over a series of events to cut
 * //          down on the overhead of processing multiple drones at the same time.
 * //          After having muliple drones there is a noticable lag.  This should 
 * //          make it much less of a cpu hog when muliple drones are running around.
 * //----------------------------------------------------------------------------------
 * 
 * Config:
 * ----------------------------------------------------------
 * [S-ODCM]          - Start of script data block
 * ProcessSpeed      - Slow, Normal, Fast
 * 
 * [LCD]             - Config Section Header
 * OutputPanelName   - Panel Name or nothing
 * OutputSurface     - LCD Index for cockpits or 0
 * 
 * [Timer]           - Timer options to execute(MUST be befor Plans)
 * TimerId           - ID for the timer plan
 * ActionType        - ExecuteAlways, StopOnDisconnect
 * BlockType         - Timer, ProgrammableBlock
 * TimerDelay        - Time in seconds to wait before starting timer
 * BlockId           - Key for Plans to execute a timer
 * Command           - ProgramBlock to executes
 *     
 * [Plan]            - Config Section for each plan
 * ConnectorFlag     - Text on line of Custom Data field
 * CargoFrom         - Name of the group to move cargo from
 * CargoTo           - Name of the group to move cargo to
 * TimerID           - ID of the timer plan to execute when done
 * 
 * 
 * Sample Config:
 * ----------------------------------------------------------
 * [S-ODCM]
 * ProcessSpeed=Slow
 * [LCD]
 * OutputPanelName=CM - LCD Cargo Mover
 * OutputSurface=0
 * [Timer]
 * TimerId=SamMoveNext
 * ActionType=StopOnDisconnect
 * BlockType=ProgrammableBlock
 * TimerDelay=60
 * BlockId=CM - PB: SAM
 * Command=START NEXT
 * [Plan]
 * ConnectorFlag=MoveCargo
 * CargoFrom=MP - MoveThisCargo
 * CargoTo=CM - MyCargo
 * TimerId=SamMoveNext
 * [Plan]
 * ConnectorFlag=UnloadCargo
 * CargoFrom=CM - MyCargo
 * CargoTo=Base - MyCargo
 * TimerId=SamMoveNext
 * 
 */

//----------------------------------------------------------------------------------
// Script - On Drone Cargo Mover
//----------------------------------------------------------------------------------

private readonly string cdTag = "S-ODCM";
private const string version = "v0.3";



//----------------------------------------------------------------------------------
//Vars -- Should ONLY alter the values in the Custom Data
//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
//     -- Process Vars
//----------------------------------------------------------------------------------

//lists
private List<IMyTerminalBlock> toContainers = new List<IMyTerminalBlock>();
private List<IMyTerminalBlock> fromContainers = new List<IMyTerminalBlock>();
//        private List<IMyTerminalBlock> containersWithInventories;

private List<string> foundConnectorFlags = new List<string>();
private List<IMyShipConnector> shipConnectors = new List<IMyShipConnector>();

private Dictionary<string, ActionPlan> actionPlans = new Dictionary<string, ActionPlan>();
private Dictionary<string, TimerPlan> timerPlans = new Dictionary<string, TimerPlan>();
private Dictionary<string, Timer> timersAvailable = new Dictionary<string, Timer>();

private Dictionary<string, IMyShipConnector> cargoTransferConnectors;

private List<string> connectorFlags;
private List<string> timerFlags;
private List<MyInventoryItem> items;

private List<Timer> timersRunning = new List<Timer>();

//config items
private readonly MyIni dData = new MyIni();

//testing and moving vars
private readonly string thisGrid = "";

//pointer to our connector
private IMyShipConnector connector;

//var strings
private string connectorFlag;
private string cargoFrom;
private string cargoTo;
private string timerId;
private string flag;

//var bools
private bool doProcessPlansLoop = false;
private bool doTestConnectorsLoop = false;
private bool doTestConnectorFlagsLoop = false;
private bool doMoveCargoLoop = false;
private bool doMoveCargoLoop_Step_1 = false;
private bool doMoveCargoLoop_Step_2 = false;
private bool doMoveCargoLoop_Step_3 = false;
private bool wasConnected = false;
private bool isConnected = false;
private bool foundFlag = false;
private bool movedItems = false;
private bool tLoop = false;

//var ints
private int currentItemVolume = 0;
private int howManyPulled = 0;

private int foundConnectorFlagsIndex = 0;
private int shipConnectorsIndex = 0;
private int connectorFlagsIndex = 0;
private int fromContainersIndex = 0;
private int toContainersIndex = 0;
private int itemsIndex = 0;
private int timerCnt = 0;
private int tIdx = 0;

private IMyTerminalBlock fromContainer;
private IMyTerminalBlock toContainer;
private MyInventoryItem item;

//temp vars
private readonly string[] stringSeparators = new string[] { "\n[" };



//----------------------------------------------------------------------------------
//     -- Looping Vars
//----------------------------------------------------------------------------------

//logging info
private IMyTextSurface opanel;

//enums for controlling program
private ActionType.Action myAction;

private Task currentTask;

private string status = "";

//Lists
private Queue<Task> tasksToDo = new Queue<Task>();




//----------------------------------------------------------------------------------
//Init Functions
//----------------------------------------------------------------------------------

public Program()
{
    //setup pointer to the ship we are on
    thisGrid = Me.CubeGrid.ToString();

    Init();
}



//----------------------------------------------------------------------------------
//Main Loop Functions
//----------------------------------------------------------------------------------

public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & (UpdateType.Update100 | UpdateType.Terminal)) != 0 ||
        (updateSource & (UpdateType.Update10 | UpdateType.Terminal)) != 0 ||
        (updateSource & (UpdateType.Update1 | UpdateType.Terminal)) != 0 ||
        (updateSource & (UpdateType.Once | UpdateType.Terminal)) != 0)
    {
						try
						{
//                    int timerCnt = 0;
//                    string[] vars = argument.Split(' ');

            ProcessQueue();

            if (timersRunning.Count > 0) { ProcessTimers(ref timerCnt); }

            MakeOutput((connector != null && connector.Status ==
                MyShipConnectorStatus.Connected), toContainers.Count(),
                fromContainers.Count(), howManyPulled, timerCnt);
        }
        catch (Exception e)
        {
            // Dump the exception content to the
            Echo("An error occurred during script execution.");
            Echo($"Exception: {e}\n---");

            // Rethrow the exception to make the programmable block halt execution properly
            throw;
        }
    }
}

public IMyGridTerminalSystem GetGridTerminalSystem() => GridTerminalSystem;

private void ProcessQueue()
{
    //are we doing something?
    if (currentTask != null)
    {
        //keep going until done
        switch (currentTask.GetActionType())
        {
            case ActionType.Action.Setup:
                {
                    Echo("" + ActionType.Action.Setup.ToString());
                    DoSetup(ref currentTask);
                    break;
                }
            case ActionType.Action.TestConnectors:
                {
                    Echo("" + ActionType.Action.TestConnectors.ToString());
                    DoTestConnectors(ref currentTask);
                    break;
                }
            case ActionType.Action.TestConnectionFork:
                {
                    Echo("" + ActionType.Action.TestConnectionFork.ToString());
                    DoTestConnectionFork(ref currentTask);
                    break;
                }
            case ActionType.Action.TestConnectorFlags:
                {
                    Echo("" + ActionType.Action.TestConnectorFlags.ToString());
                    DoTestConnectorFlags(ref currentTask);
                    break;
                }
            case ActionType.Action.TestConnectorFlags_Step_1:
                {
                    DoTestConnectorFlags_Step_1(ref currentTask);
                    break;
                }
            case ActionType.Action.TestConnectorFlags_Step_2:
                {
                    DoTestConnectorFlags_Step_2(ref currentTask);
                    break;
                }
            case ActionType.Action.ProcessPlansFork:
                {
                    Echo("" + ActionType.Action.ProcessPlansFork.ToString());
                    DoProcessPlansFork(ref currentTask);
                    break;
                }
            case ActionType.Action.ProcessPlans:
                {
                    Echo("" + ActionType.Action.ProcessPlans.ToString());
                    DoProcessPlans(ref currentTask);
                    break;
                }
            case ActionType.Action.ProcessPlans_Step_1:
                {
                    DoProcessPlans_Step_1(ref currentTask);
                    break;
                }
            case ActionType.Action.ProcessPlans_Step_2:
                {
                    DoProcessPlans_Step_2(ref currentTask);
                    break;
                }
            case ActionType.Action.ProcessPlans_Step_3:
                {
                    DoProcessPlans_Step_3(ref currentTask);
                    break;
                }
            case ActionType.Action.MoveCargo:
                {
                    Echo("" + ActionType.Action.MoveCargo.ToString());
                    DoMoveCargo(ref currentTask);
                    break;
                }
            case ActionType.Action.MoveCargo_Step_1:
                {
                    DoMoveCargo_Step_1(ref currentTask);
                    break;
                }
            case ActionType.Action.MoveCargo_Step_2:
                {
                    DoMoveCargo_Step_2(ref currentTask);
                    break;
                }
            case ActionType.Action.MoveCargo_Step_3:
                {
                    DoMoveCargo_Step_3(ref currentTask);
                    break;
                }
            case ActionType.Action.ActivateTimer:
                {
                    Echo("" + ActionType.Action.ActivateTimer.ToString());
                    DoActivateTimer(ref currentTask);
                    break;
                }
            case ActionType.Action.Cleanup:
                {
                    Echo("" + ActionType.Action.Cleanup.ToString());
                    DoCleanup(ref currentTask);
                    break;
                }
            default:
                {
                    Echo("" + ActionType.Action.Init.ToString());
                    Init();
                    break;
                }
        }
    }

    //get the next item to do
    if (currentTask == null)
    {
        GetNextTask();
    }
}

private void GetNextTask()
{
    if (tasksToDo.Count > 0)
    {
        //we have more to do
        currentTask = tasksToDo.Dequeue();
        myAction = currentTask.GetActionType();
    }
}



//----------------------------------------------------------------------------------
//Testing Functions
//----------------------------------------------------------------------------------

private bool IsConnected(ref Task currentTask)
    {
    if (!(connector != null && connector.Status == MyShipConnectorStatus.Connected))
    {
        tasksToDo.Enqueue(new Task(ActionType.Action.Cleanup));
        currentTask = null;
        return false;
    }
				else { return true; }
}

private void DoTestConnectors(ref Task currentTask)
{
    if (!doTestConnectorsLoop)
    {
        if (timersRunning.Count == 0) { status = "Waiting . ."; }

        doTestConnectorsLoop = true;
        shipConnectorsIndex = 0;
    }

    //loop through the ships connector list
    if (shipConnectors.Count > shipConnectorsIndex)
    {
        connector = shipConnectors[shipConnectorsIndex];

        //if the ship is connected to something
        if (connector.Status == MyShipConnectorStatus.Connected)
        {
            //flag we are connected
            isConnected = true;
        }

        shipConnectorsIndex++;

        tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectors));
    }
    else
    {
        //update task phase
        //put task back in Q
        tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectionFork));
    }

    currentTask = null;
}

private void DoTestConnectionFork(ref Task currentTask)
{
    if ((isConnected) && (!wasConnected))
    {
        if (timersRunning.Count == 0) { status = "Waiting . . ."; }

        tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectorFlags));
    }
    else
    {
        //process the timers we have
        tasksToDo.Enqueue(new Task(ActionType.Action.Cleanup));
    }

    currentTask = null;
}

private void DoTestConnectorFlags(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    if (!doTestConnectorFlagsLoop)
    {
        status = "Initializing . . .";

        doTestConnectorFlagsLoop = true;
        shipConnectorsIndex = 0;

        //Lists
        toContainers = new List<IMyTerminalBlock>();
        fromContainers = new List<IMyTerminalBlock>();

        foundConnectorFlags = new List<string>();

        //test our connectors
        cargoTransferConnectors = new Dictionary<string, IMyShipConnector>();
    }

    //loop through the ships connector list
    if (shipConnectors.Count > shipConnectorsIndex)
    {
        connector = shipConnectors[shipConnectorsIndex];

        shipConnectorsIndex++;
        tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectorFlags_Step_1));

        connectorFlagsIndex = 0;
        flag = "";
    }
    else
    {
        tasksToDo.Enqueue(new Task(ActionType.Action.ProcessPlansFork));
    }

    currentTask = null;
}

private void DoTestConnectorFlags_Step_1(ref Task currentTask)
		{
    if (!IsConnected(ref currentTask)) { return; }

    if (connector.Status == MyShipConnectorStatus.Connected)
    {
        //see if the connecton has flag we know of
        if (connectorFlags.Count > connectorFlagsIndex)
        {
            flag = connectorFlags[connectorFlagsIndex];

            connectorFlagsIndex++;
            tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectorFlags_Step_2));
        }
						else
						{
            tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectorFlags));
        }
    }
				else
				{
        tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectorFlags));
    }

    currentTask = null;
}

private void DoTestConnectorFlags_Step_2(ref Task currentTask)
		{
    if (!IsConnected(ref currentTask)) { return; }

    if (connector.OtherConnector.CustomData.Contains(flag))
    {
        if (connector.CubeGrid.ToString().Equals(this.thisGrid))
        {
            cargoTransferConnectors.Add(flag, connector);
        }

        foundFlag = true;
        foundConnectorFlags.Add(flag);
    }

    currentTask = null;

    tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectorFlags_Step_1));
}



//----------------------------------------------------------------------------------
//Processing Functions
//----------------------------------------------------------------------------------

private void DoSetup(ref Task currentTask)
{
    if (timersRunning.Count == 0) { status = "Waiting ."; }

    SetupVars();
    currentTask = null;
    tasksToDo.Enqueue(new Task(ActionType.Action.TestConnectors));
}

private void SetupVars()
{
    //reset the flags for this round of testing
    isConnected = false;
    foundFlag = false;

    doTestConnectorsLoop = false;
    doTestConnectorFlagsLoop = false;
    doProcessPlansLoop = false;
    doMoveCargoLoop = false;
    doMoveCargoLoop_Step_1 = false;
    doMoveCargoLoop_Step_2 = false;
    doMoveCargoLoop_Step_3 = false;
}

private void DoProcessPlansFork(ref Task currentTask)
		{
    if (!IsConnected(ref currentTask)) { return; }

    //if we have a vaild connection and we are only doing the first check
    if (foundFlag)
    {
        tasksToDo.Enqueue(new Task(ActionType.Action.ProcessPlans));
    }
				else
				{
        tasksToDo.Enqueue(new Task(ActionType.Action.Cleanup));
    }

    currentTask = null;
}

private void DoProcessPlans(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    if (!doProcessPlansLoop)
    {
        doProcessPlansLoop = true;

        status = "Processing . . .";

        //lists
//                containersWithInventories = new List<IMyTerminalBlock>();

        //vars
        foundConnectorFlagsIndex = 0;
    }

    //process each connector flag we found
    if (foundConnectorFlags.Count > foundConnectorFlagsIndex)
    {
        connectorFlag = foundConnectorFlags[foundConnectorFlagsIndex];

        tasksToDo.Enqueue(new Task(ActionType.Action.ProcessPlans_Step_1));

        foundConnectorFlagsIndex++;
    }
    else
    {
        tasksToDo.Enqueue(new Task(ActionType.Action.Cleanup));
    }

    currentTask = null;
}

private void DoProcessPlans_Step_1(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    //clear out lists
    toContainers.Clear();
    fromContainers.Clear();

    //load our actionPlan we need to use
    ActionPlan plan = actionPlans[connectorFlag];

    //parse the plan data
    cargoFrom = plan.GetCargoFrom();
    cargoTo = plan.GetCargoTo();
    timerId = plan.GetTimerId();

    tasksToDo.Enqueue(new Task(ActionType.Action.ProcessPlans_Step_2));

    currentTask = null;
}

private void DoProcessPlans_Step_2(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    // what containers do we use?
    if (cargoFrom.Trim().Length > 0 && cargoTo.Trim().Length > 0)
    {
        //get the to and from container groups
        try
        {
            GridTerminalSystem.GetBlockGroupWithName(cargoFrom).GetBlocksOfType<IMyTerminalBlock>(fromContainers);
        }
        catch (Exception e)
        {
            Echo("Missing CargoFrom: " + e.Message);
        }

        try
        {
            GridTerminalSystem.GetBlockGroupWithName(cargoTo).GetBlocksOfType<IMyTerminalBlock>(toContainers);
        }
        catch (Exception e)
        {
            Echo("Missing CargoTo: " + e.Message);
        }

        //filter out unusable blocks
        for (int i = fromContainers.Count - 1; i >= 0; i--)
        {
            if (!fromContainers[i].HasInventory)
            {
                fromContainers.RemoveAt(i);
            }
        }

        for (int i = toContainers.Count - 1; i >= 0; i--)
        {
            if (!toContainers[i].HasInventory)
            {
                toContainers.RemoveAt(i);
            }
        }
    }

    tasksToDo.Enqueue(new Task(ActionType.Action.ProcessPlans_Step_3));
    currentTask = null;
}

private void DoProcessPlans_Step_3(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    //if we have at least one item in each list
    if ((toContainers.Count > 0) && (fromContainers.Count > 0))
    {
        tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo));
    }
				else
				{
        tasksToDo.Enqueue(new Task(ActionType.Action.ProcessPlans));
    }

    currentTask = null;
}

private void DoMoveCargo(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    if (!doMoveCargoLoop)
    {
        //vars
        doMoveCargoLoop = true;

        currentItemVolume = 0;
        movedItems = false;

        fromContainersIndex = 0;
    }

    try
    {
        if (fromContainers.Count > fromContainersIndex)
        {
            fromContainer = fromContainers[fromContainersIndex];

            //testing for empty fromcontainer
            fromContainersIndex++;

            //do the loop again
            tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo_Step_1));
        }
        else
						{
            //move to next phase
            tasksToDo.Enqueue(new Task(ActionType.Action.ActivateTimer));
        }

        if (0 == howManyPulled && movedItems) { howManyPulled = 1; }
    }
    catch (Exception e)
    {
        EchoToLCD("ERROR: \n\n" + e.Message + "\n\n");
    }

    currentTask = null;
}

private void DoMoveCargo_Step_1(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    if (!doMoveCargoLoop_Step_1)
    {
        doMoveCargoLoop_Step_1 = true;
    }

    if (fromContainer.GetInventory(0).ItemCount > 0)
    {
        //for each item in the from container
        items = new List<MyInventoryItem>();
        fromContainer.GetInventory(0).GetItems(items);

        tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo_Step_2));
        doMoveCargoLoop_Step_2 = false;
    }
    else
    {
        status = "No Cargo to Move . . .";

        tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo));
    }

    currentTask = null;
}

private void DoMoveCargo_Step_2(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    if (!doMoveCargoLoop_Step_2)
    {
        doMoveCargoLoop_Step_2 = true;
        itemsIndex = 0;
    }

    if (items.Count > itemsIndex)
    {
        item = items[itemsIndex];

        //loop again
        itemsIndex++;
        tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo_Step_3));
        doMoveCargoLoop_Step_3 = false;
    }
    else
    {
        //do prior loop
        tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo_Step_1));
    }

    currentTask = null;
}

private void DoMoveCargo_Step_3(ref Task currentTask)
{
    if (!IsConnected(ref currentTask)) { return; }

    if (!doMoveCargoLoop_Step_3)
    {
        doMoveCargoLoop_Step_3 = true;
        toContainersIndex = 0;
    }

    //for each tocontainer
    if (toContainers.Count > toContainersIndex)
    {
        toContainer = toContainers[toContainersIndex];

        //testing for full tocontainer
        if (!toContainer.GetInventory(0).IsFull)
        {
            status = "Moving Cargo . . .";

            //if the tocontainer has more volume then the items take up
            if ((toContainer.GetInventory(0).MaxVolume
                - toContainer.GetInventory(0).CurrentVolume)
                > item.Amount * item.Type.GetItemInfo().Volume)
            {
                currentItemVolume = 0;

                //calc amount moving
                currentItemVolume = (int)(item.Amount *
                    item.Type.GetItemInfo().Volume);

                //move the amount
                if (fromContainer.GetInventory(0).TransferItemTo(
                    toContainer.GetInventory(0), item))
                {
                    movedItems = true;

                    //update var with moved amount
                    howManyPulled += currentItemVolume;

                    //do prior loop
                    tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo_Step_2));
                    return;
                }
            }
            else
            {
                currentItemVolume = 0;

                //calc how much of the item we can transfer
                currentItemVolume = (int)((toContainer.GetInventory(0).MaxVolume -
                    toContainer.GetInventory(0).CurrentVolume) *
                    (MyFixedPoint)(1 / item.Type.GetItemInfo().Volume));

                //try to transfer the calced amount of the item
                if (fromContainer.GetInventory(0).TransferItemTo(
                    toContainer.GetInventory(0), item,
                    (toContainer.GetInventory(0).MaxVolume -
                    toContainer.GetInventory(0).CurrentVolume) *
                    (MyFixedPoint)(1 / item.Type.GetItemInfo().Volume)))
                {
                    movedItems = true;
                    howManyPulled += currentItemVolume;
                }
            }
        }

        toContainersIndex++;

        //dp the loop agian
        tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo_Step_3));
    }
    else
    {
        status = "Container Full . . .";

        //do prior loop
        tasksToDo.Enqueue(new Task(ActionType.Action.MoveCargo_Step_2));
    }

    currentTask = null;
}

private void DoActivateTimer(ref Task currentTask)
		{
    if (!IsConnected(ref currentTask)) { return; }

    //execute timer as needed
    if (timerId.Length > 0 && timerPlans.ContainsKey(timerId))
    {
        status = "Setting up Timer . . .";

        //get timer plan
        TimerPlan timerPlan = timerPlans[timerId];

        //should move this to the init and store the timer under the timerplan
        //then just add/remove from the timers list
        Timer timer = timersAvailable[timerId];
        timer.SetConnector(cargoTransferConnectors[connectorFlag]);

        //queue up timer
        timersRunning.Add(timer);
    }

    tasksToDo.Enqueue(new Task(ActionType.Action.Cleanup));
    currentTask = null;
       }

private void DoCleanup(ref Task currentTask)
{
    //flag if we moved cargo
    wasConnected = isConnected;
    currentTask = null;

    tasksToDo.Enqueue(new Task(ActionType.Action.Setup));
}

private void ProcessTimers(ref int timerCnt)
{
    if(!tLoop)
				{
        tLoop = true;

        tIdx = timersRunning.Count - 1;
    }

    if(tIdx >= 0)
    {
//                if (!IsConnected(ref currentTask)) { return; }

        status = "Waiting on Timer . . .";

        Timer timer = timersRunning[tIdx];

        if (timer.Check() || !IsConnected(ref currentTask))
        {
            switch (timer.GetTimerType())
            {
                case TimerPlan.TimerType.StopOnDisconnect:
                    {
                        if (timer.GetConnector() != null && timer.GetConnector().Status == MyShipConnectorStatus.Connected)
                        {
                            status = "Executing Timer . . .";

                            timersRunning.Remove(timer);
                            timer.Execute();
                        }
                        else
                        {
                            status = "Disconnected before Timer . . .";
                            timersRunning.Remove(timer);
                        }
                        break;
                    }
                default:
                    {
                        status = "Executing Timer . . .";

                        timer.Execute();
                        timersRunning.Remove(timer);
                        break;
                    }
            }
        }

        if (timer.GetTimerDelayLeft() >= 0)
        {
            timerCnt = timer.GetTimerDelayLeft();
        }
        else
						{
            timerCnt = 0;
						}

        tIdx--;
    }
				else
				{
        toContainers.Clear();
        fromContainers.Clear();
        howManyPulled = 0;

        tLoop = false;  //reset for next check
    }
}



//----------------------------------------------------------------------------------
//Load Config and ActionPlan Functions
//----------------------------------------------------------------------------------

private void Init()
		{
				//just in case an error happens before we have a terminal screen
				SetupLCD(Me.CustomName);

    //write example config if there is no config
    if (Me.CustomData.Trim().Length == 0 ||
        !Me.CustomData.Trim().ToLower().Contains(cdTag.ToLower()))
    {
        WriteConfig();
    }

    //setup clean vars
    SetupVars();

    //load the config info
    LoadConfig();

    shipConnectors.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(shipConnectors, (x => x.CubeGrid.ToString() == thisGrid));

    //setup the loop timer
    this.tasksToDo.Enqueue(new Task(ActionType.Action.Setup));
}

private void LoadConfig()
{
    //vars
    int pCnt = 0;
    int tCnt = 0;

    //get custom data for this block
    string cdData = Me.CustomData.Trim();

    //need to parse cdData into sections
    string[] sections = cdData.ToString().Split(stringSeparators, StringSplitOptions.None);

    for (int i = 0; i < sections.Length; i++)
    {
        if (sections[i].ToLower().StartsWith("plan]"))
        {
            sections[i] = pCnt + sections[i];
            pCnt += 1;
        }
        if (sections[i].ToLower().StartsWith("timer]"))
        {
            sections[i] = tCnt + sections[i];
            tCnt += 1;
        }
    }

    if (dData.TryParse(String.Join("\n[", sections.ToArray()).Trim()))
    {
        //vars
        TimerPlan.TimerType timerType = TimerPlan.TimerType.StopOnDisconnect;
        TimerPlan.BlockType blockType = TimerPlan.BlockType.Timer;

        List<string> sectionNames = new List<string>();
        connectorFlags = new List<string>();
        timerFlags = new List<string>();

        actionPlans.Clear();
        timerPlans.Clear();

        string connectorFlag = "";
        string cargoFrom = "";
        string cargoTo = "";
        string blockId = "";
        string timerId = "";
        string command = "";

        int timerDelay = 0;

        dData.GetSections(sectionNames);

        //process each section
        foreach (string section in sectionNames)
        {
            //Main section
            if (section.Trim().ToLower().EndsWith(this.cdTag.ToLower()))
								{
                switch(dData.Get(section, "ProcessSpeed").ToString("Slow").Trim().ToLower())
										{
                    case "debug":
														{
                            Runtime.UpdateFrequency = UpdateFrequency.Once;
                            break;
														}
                    case "fast":
														{
                            Runtime.UpdateFrequency = UpdateFrequency.Update1;
                            break;
														}
                    case "normal":
														{
                            Runtime.UpdateFrequency = UpdateFrequency.Update10;
                            break;
                        }
                    default:
														{
                            Runtime.UpdateFrequency = UpdateFrequency.Update100;
                            break;
                        }
                }
            }
            //we have the LCD section
            else if (section.Trim().ToLower().EndsWith("lcd"))
            {
                //load the display vars for later use
                SetupLCD(
                dData.Get(section, "OutputPanelName").ToString(Me.CustomName).Trim(),
                dData.Get(section, "OutputSurface").ToInt32(0)
                );
            }
            //we have a Timer section
            else if (section.Trim().ToLower().EndsWith("timer"))
            {
                timerId = dData.Get(section, "TimerId").ToString(timerId).Trim();

                //get the timer data to make a timerplan
                timerType = (dData.Get(section, "ActionType").ToString(
                            timerType.ToString()).Trim().ToLower().Equals(
                    TimerPlan.TimerType.StopOnDisconnect.ToString().ToLower()
                    )) ? TimerPlan.TimerType.StopOnDisconnect : TimerPlan.TimerType.ExecuteAlways;

                blockType = (dData.Get(section, "BlockType").ToString(
                            blockType.ToString()).Trim().ToLower().Equals(
                    TimerPlan.BlockType.Timer.ToString().ToLower()
                    )) ? TimerPlan.BlockType.Timer : TimerPlan.BlockType.ProgrammableBlock;

                blockId = dData.Get(section, "BlockId").ToString(blockId).Trim();

                command = dData.Get(section, "Command").ToString(command).Trim();

                timerDelay = dData.Get(section, "TimerDelay").ToInt32(timerDelay);

                //if we dont have a timerId or if we already have one
                if (blockId.Length != 0 || !timerFlags.Contains(blockId))
                {
                    timerFlags.Add(blockId);
                }

                //make and add timer to the list
                if (!timerPlans.ContainsKey(timerId))
                {
                    timerPlans.Add(timerId, new TimerPlan(timerType, blockType, timerDelay,
                        blockId, command));

                    timersAvailable.Add(timerId, new Timer(this, null, timerId, timerType,
                        blockType, timerDelay, blockId, command));
                }
            }
            //we have a Plan section
            else if (section.Trim().ToLower().EndsWith("plan"))
            {
                //get all the plan data to make a actionplan
                connectorFlag = dData.Get(section, "ConnectorFlag").ToString(connectorFlag).Trim();
                cargoFrom = dData.Get(section, "CargoFrom").ToString(cargoFrom).Trim();
                cargoTo = dData.Get(section, "CargoTo").ToString(cargoTo).Trim();
                timerId = dData.Get(section, "TimerId").ToString(timerId).Trim();

                //if we dont have a connectorFlag or if we already have one
                if (connectorFlag.Length != 0 || !connectorFlags.Contains(connectorFlag))
                {
                    connectorFlags.Add(connectorFlag);
                }

                //make and add plan to the list
                if (!actionPlans.ContainsKey(connectorFlag))
                {
                    actionPlans.Add(connectorFlag, new ActionPlan(cargoFrom, cargoTo, timerId));
                }
            }
        }
    }
    sections = null;
}

private void WriteConfig()
{
    EchoToLCD("Printed default Config to CD of " + Me.CustomName);
    string cData = "";

    if (Me.CustomData.Trim().Length == 0)
        //start off clean
        cData = $"[{cdTag}]\n";
    else
        //add to whatever is already there
        cData += $"[{cdTag}]\n";

    cData += "ProcessSpeed=Slow\n";
    cData += "[LCD]\n";
    cData += "OutputPanelName={Me.CustomName}\n";
    cData += "OutputSurface=0\n";
    cData += "[Timer]\n";
    cData += "TimerId=\n";
    cData += "ActionType=\n";
    cData += "BlockType=\n";
    cData += "TimerDelay=\n";
    cData += "BlockId=\n";
    cData += "Command=\n";
    cData += "[Plan]\n";
    cData += "ConnectorFlag=LoadCargo\n";
    cData += "CargoFrom=\n";
    cData += "CargoTo=TakeThis\n";
    cData += "TimerId=\n";
    cData += "[Plan]\n";
    cData += "ConnectorFlag=UnloadCargo\n";
    cData += "CargoFrom=TakeThis\n";
    cData += "CargoTo=PutHere\n";
    cData += "TimerId=\n";
    Me.CustomData = cData;
}

public void SetupLCD(string panelName, int panelSurface = 0)
{
    if (panelName.Length > 0)
    {
        try
        {
            opanel = (GridTerminalSystem.GetBlockWithName(panelName) as IMyTextSurfaceProvider).GetSurface(panelSurface);
        }
        catch (Exception e)
        {
            Echo("ERROR: No Output Screen Found\n\n" + e.Message);
        }
    }
}



//----------------------------------------------------------------------------------
//LCD Functions
//----------------------------------------------------------------------------------

public void MakeOutput(bool isconnected, int toContainersCnt,
    int fromContainersCnt, int howManyPulled, int timerCnt)
{
    ClearLCD();
    //print what we did
    EchoToLCD( "Status: " + status + "\n\n"
        + ((isconnected) ? "Connector: Connected" : "Connector: Not Connected")
        + "\nTO Containers: " + toContainersCnt
        + "\nFROM Containers: " + fromContainersCnt
        + "\nVolume Moved: " + howManyPulled
        + ((timerCnt != 0) ? "\nTimer In: " + timerCnt : "")
        );
}

public void EchoToLCD(string text)
{
    // Append the text and a newline to the logging LCD
    // A nice little C# trick here:
    // - The ?. after dpanel means "call only if dpanel is not null".
    opanel?.WriteText("" + text + "\n", true);
}

public void ClearLCD()
		{
    opanel?.WriteText("");
}

}
	//----------------------------------------------------------------------------------
	//Task Classes Needed
	//
	// - used to hold the actions that are done by the main script
	//----------------------------------------------------------------------------------

	class ActionType
	{
			public enum Action { Init, Setup, TestConnectors, TestConnectionFork,
					TestConnectorFlags, TestConnectorFlags_Step_1, TestConnectorFlags_Step_2,
					ProcessPlansFork,	ProcessPlans,	ProcessPlans_Step_1, ProcessPlans_Step_2,
					ProcessPlans_Step_3, MoveCargo, MoveCargo_Step_1, MoveCargo_Step_2,
					MoveCargo_Step_3,	ActivateTimer, Cleanup };
	}


	class Task
	{
			private readonly ActionType.Action actionType = ActionType.Action.Init;

			public Task(ActionType.Action actionType)
			{
					this.actionType = actionType;
			}

			public ActionType.Action GetActionType() => this.actionType;
	}



	//----------------------------------------------------------------------------------
	//Class ActionPlan
	//
	// - used to hold the actions that are done by the main script
	//----------------------------------------------------------------------------------

	class ActionPlan
{
    private readonly string cargoFrom = "";
    private readonly string cargoTo = "";
    private readonly string timerId = "";

    public ActionPlan(string cargoFrom, string cargoTo, string timerId)
    {
        this.cargoFrom = cargoFrom;
        this.cargoTo = cargoTo;
        this.timerId = timerId;
    }

    public string GetCargoFrom() => this.cargoFrom;

    public string GetCargoTo() => this.cargoTo;

    public string GetTimerId() => this.timerId;
}



	//----------------------------------------------------------------------------------
	//Class TimerPlan
	//
	// - used to hold the timer data used to execute a timer after
	//   completing an action plan by the main script
	//----------------------------------------------------------------------------------

	class TimerPlan
	{
			public enum TimerType { StopOnDisconnect, ExecuteAlways };
			public enum BlockType { Timer, ProgrammableBlock };

			private readonly TimerType timerType = TimerType.StopOnDisconnect;
			private readonly BlockType blockType = BlockType.Timer;
			private readonly string blockId = "";
			private readonly string command = "";
			private readonly int delay = 0;

			public TimerPlan(TimerType timerType, BlockType blockType, int delay,
					string blockId, string command)
			{
					this.timerType = timerType;
					this.blockType = blockType;
					this.delay = delay;
					this.blockId = blockId;
					this.command = command;
			}

			public TimerType GetTimerType() => this.timerType;

			public BlockType GetBlockType() => this.blockType;

			public int GetDelay() => this.delay;

			public string GetBlockId() => this.blockId;

			public string GetCommand() => this.command;
	}


	class Timer
	{
			private readonly string timerId;
			private readonly TimerPlan.TimerType actionType;
			private readonly TimerPlan.BlockType blockType;
			private readonly string blockId;
			private readonly string command;
			private readonly double delay;

			private Double elapsedMillisecs;

			private DateTime startTime = new DateTime(0);
			private DateTime endTime = new DateTime(0);

			private IMyShipConnector xferConnector;
			private readonly IMyGridTerminalSystem gridTerminalSystem;
			private readonly Program program;

			public Timer(Program program, IMyShipConnector xferConnector,
					string timerId, TimerPlan.TimerType actionType, TimerPlan.BlockType blockType,
					int delay, string blockId, string command = "")
			{
					this.program = program;
					this.xferConnector = xferConnector;
					this.timerId = timerId;
					this.actionType = actionType;
					this.blockType = blockType;
					this.delay = (double)delay;
					this.blockId = blockId;
					this.command = command;

					gridTerminalSystem = program.GetGridTerminalSystem();
			}

			public string GetTimerId() => this.timerId;

			public int GetTimerDelayLeft()
			{
					return (int)(((delay * 1000) - (endTime - startTime).TotalMilliseconds) / 1000);
			}

			public TimerPlan.TimerType GetTimerType() => this.actionType;

			public IMyShipConnector GetConnector() => this.xferConnector;

			public void SetConnector(IMyShipConnector xferConnector)
			{
					this.xferConnector = xferConnector;
			}

			public string GetBlockId() => this.blockId;

			public bool Check()
			{
					if (startTime != DateTime.MinValue)
					{
							//we started tracking time
							endTime = DateTime.Now;

							elapsedMillisecs = (endTime - startTime).TotalMilliseconds;

							//has enough time gone by?
							if (elapsedMillisecs >= (delay * 1000))
							{
									startTime = DateTime.MinValue;
									return true;
							}
					}
					else
					{
							//start tracking time
							startTime = DateTime.Now;
					}
					return false;
			}

			public bool Execute()
			{
					switch (this.blockType)
					{
							case TimerPlan.BlockType.ProgrammableBlock:
									{
											return (ProgrammableBlockRun(blockId, command));
									}
							case TimerPlan.BlockType.Timer:
									{
											return (TimerBlockTrg(blockId));
									}
							default:
									{
											return false;
									}
					}
			}

			private bool TimerBlockTrg(string blockName)
			{
					try
					{
							IMyTimerBlock block = gridTerminalSystem.GetBlockWithName(blockName) as IMyTimerBlock;
							if (block != null)
							{
									if (!BlockIsWorking(block))
									{
											return false;
									}

									block.Trigger();
									return true;
							}
					}
					catch (Exception e)
					{
							program.EchoToLCD("There was a problem with the block called " + blockName + "\n" + e.Message);
					}
					return false;
			}

			private bool ProgrammableBlockRun(string blockName, string command = "")
			{
					try
					{
							IMyProgrammableBlock block = gridTerminalSystem.GetBlockWithName(blockName) as IMyProgrammableBlock;
							if (block != null)
							{
									if (!BlockIsWorking(block))
									{
											return false;
									}

									if (!block.TryRun(command))
									{
											//a false return means it was not executed
											program.EchoToLCD($"WARNING: Command was sent ... but '{blockName}' was busy!\nCommand might not have been executed.");
									}
									return true;
							}
							else
							{
									program.EchoToLCD($"ERROR: Recieved '{blockName}' run with '{command}',\nhowever that Block is not recognised as a Programmable-Block!");
							}
					}
					catch (Exception e)
					{
							program.EchoToLCD("There was a problem with the block called" + blockName + "\n" + e.Message);
					}
					return false;
			}

			private bool BlockIsWorking(IMyTerminalBlock block, bool working = true)
			{
					if (working)
					{
							if (!block.IsWorking)
							{
									program.EchoToLCD($"ERROR: '{block.CustomName}' is not in a workable State!");
									return false;
							}
					}
					else
					{
							if (!block.IsFunctional)
							{
									program.EchoToLCD($"ERROR: '{block.CustomName}' is not in a functional State!");
									return false;
							}
					}
					return true;
			}