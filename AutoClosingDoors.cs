/*
 * R e a d m e
 * -----------
 *     This script can be used as-is, and it is simple to understand and modify. Simply upload it to a Programmable block and compile. (No timer block needed)
 *     It automatically closes all the doors on the local grid, after a specified period (default 3 secs) has elapsed from the moment the door was opened.
 *      
 *     If you want to increase / decrease the close delay, pass a 3rd parameter of type int, to the static method 'DoorManager.Initialize' (or just change the hard-coded value). The default is 3 seconds. Eg: `DoorManager.Initialize(this, tickEventManager, 5);` This will set the close delay to 5 seconds.
 *     If you want the script to auto-close doors on connected grids as well, then pass a 4th parameter of type bool, to the static method 'DoorManager.Initialize' (or just change the hard-coded value). The default is false. Eg: `DoorManager.Initialize(this, tickEventManager, 3, true);` This will also auto-close doors on connected grids.
 *     If you want a door to be ignored by this script, add the text '-ignore' to the custom data of the door block.
 *     And that is all there is to it. Enjoy your auto-closing doors!
 *      
 *     Author: Stuyvenstein
 *      
 *     Thanks to malware-dev for the awesome MDK!
 */

// TickEventManager is an event-driven class used within DoorManager to handle updating of door blocks (every 100 ticks) and auto-closing of doors (runs every 10 ticks)
private readonly TickEventManager tickEventManager;
public Program()
{
    tickEventManager = new TickEventManager();
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    DoorManager.Initialize(this, tickEventManager);
}

public void Main(string argument)
{
    tickEventManager.Tick();
}

public class DoorManager
{
    private static List<AutoDoor> _autoDoors = new List<AutoDoor>();
    private static int _closeDelaySeconds;
    private static Func<IMyDoor, bool> _doorfilter;
    const string _ignoreFlag = "-ignore";
    private static MyGridProgram _program;

    public static void Initialize(MyGridProgram program, TickEventManager tickEventManager, int CloseDelaySeconds = 3, bool AffectConnectedGrids = false)
    {
        _program = program;
        // Initialize filter to exclude doors that aren't working, and doors with the -ignore flag in their custom data field
        _doorfilter = f => f.IsWorking == true && !f.CustomData.ToLower().Contains(_ignoreFlag);
        // Amend filter if AffectConnectedGrids flag is set
        if (AffectConnectedGrids) _doorfilter = f => _doorfilter(f) && f.CubeGrid == _program.Me.CubeGrid;
        _closeDelaySeconds = CloseDelaySeconds;
        tickEventManager.OnEventTick100 += UpdateDoorList;
        tickEventManager.OnEventTick10 += UpdateDoors;
        UpdateDoorList();
    }

    // Function that updates the list of doors to be auto-closed
    private static void UpdateDoorList()
    {
        List<IMyDoor> allGridDoors = new List<IMyDoor>();
        _program.GridTerminalSystem.GetBlocksOfType(allGridDoors, _doorfilter);
        _autoDoors.RemoveAll(d => d.doorRef == null || !_doorfilter(d.doorRef));
        _autoDoors.AddRange(allGridDoors
            .Where(d => !_autoDoors
            .Select(a => a.doorRef)
            .Contains(d))
            .Select(d => new AutoDoor{doorRef = d}));
    }

    // Function to handle the actual auto-closing of affected doors
    public static void UpdateDoors()
    {
        foreach (AutoDoor autoDoor in _autoDoors)
        {
            //Find open doors that aren't yet flagged for auto-closing
            if (autoDoor.doorRef.Status == DoorStatus.Open && !autoDoor.IsTiming)
            {
                //Flag door for auto-closing
                autoDoor.IsTiming = true;
                autoDoor.TimeOpened = DateTime.Now;
            }

            //Check if opened door has reached or passed the door close delay period, and closes it if true
            if (autoDoor.doorRef.Status == DoorStatus.Open && autoDoor.IsTiming)
            {
                if (DateTime.Now.Subtract(autoDoor.TimeOpened).Seconds >= _closeDelaySeconds)
                {
                    autoDoor.IsTiming = false;
                    autoDoor.doorRef.CloseDoor();
                }
            }

            //Handle manually closed doors
            if (autoDoor.doorRef.Status == (DoorStatus.Closed | DoorStatus.Closing) && autoDoor.IsTiming) autoDoor.IsTiming = false;
        }

    }

    public class AutoDoor
    {
        public IMyDoor doorRef;
        public bool IsTiming = false;
        public DateTime TimeOpened;
    }

}

public class TickEventManager
{
    public delegate void OnTickEvent();
    public event OnTickEvent OnEventTick;
    public event OnTickEvent OnEventTick10;
    public event OnTickEvent OnEventTick100;
    private int tickCount100 = 0;
    private int tickCount10 = 0;

    public void Tick()
    {
        OnEventTick?.Invoke();
        tickCount10++;
        tickCount100++;
        if (tickCount10 >= 10)
        {
            OnEventTick10?.Invoke();
            tickCount10 = 0;
        }
        if (tickCount100 >= 100)
        {
            OnEventTick100?.Invoke();
            tickCount100 = 0;
        }
    }
}