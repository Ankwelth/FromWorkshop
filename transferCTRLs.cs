/*
 *   R e a d m e
 *   -----------
 * V2.5.5 - Hotfix
 * 
 * First, Thanks for choosing my script when it comes to ambigous movements. Moving on,  
 * The aim of this script is to be able to transfer user control from controllers and 
 * apply them to objects that can not inherently take wasd controls. 
 * 
 * That said, a tag must be provided in order for objects to pick up on groupings. Default is [ctrl]
 * and each new addition requires the reload of the pb. Sorry, its not autolcd material yet.
 * 
 * The cockpit you want to control things from must have [ctrl] in its name, and a number in custom data
 * defining which group you want to control
 * 
 * Inside the items custom data you wish to transfer control to, create 3 items seperated however (space is reliable).
 * 1.) The first should be an integer underlining the grouping number
 * 2.) The second should be the control direction relative to controllers orientation
 * 3.) a boolean (true or false) which inverts the directional schema. 
 * 
 * Once all items are in place, reload and have a blast.
 * Currently this script can be used on rotors, pistons, thrusters, projectors, grav gens, 
 * and wheels (buggy and WIP)
 * 
 * If you need a reminder of what controls what, 
 * fb = forward/back
 * lr = left/right
 * ud = up/down
 * yaw = mouse movement
 * pit = mouse movement
 * rol = roll left/roll right
 * 
 */


string nameTag = "[ctrl]"; // The identifying name tag that go in custom names
public string customTag = "[tcj5]"; // The indetifying data tag that goes in custom data
bool useScriptFace = true; // Uses a script face over the text version
////////////////////////////////////////////////////////

public int? groupShift; // For choosing which argument to process
LinkedList<Controller> cocks; // All cockpits
LinkedList<BlockHandleI> proxyblocks;
List<Wheel> wheels; // This list is to hold refference to wheels
Dictionary<int, WheelAvg> groupAvgs; // Wheel averages for each individual group

///////////////////


//Pixy[] pixies; // An array of pixies for handling transferCTRLs data
//Spriter spriter; // Spriter for easily writing sprites to displayz
/** construct it */
public Program()
{

    wheels = new List<Wheel>();
    cocks = new LinkedList<Controller>();
    proxyblocks = new LinkedList<BlockHandleI>(); // New list of block handle interfaces
    groupAvgs = new Dictionary<int, WheelAvg>(); // Initialize dictionary of averages
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(null, buildList);
    //groupShift = 0;
    //buildIt();
    //pixies = new Pixy[2]; // 2 elements will be stored in pixies; The transfer controls data + blocks it controls
    //pixies[0] = new Pixy(0, 0, 0, 0, false); // This pixy will be responsible for the transfer controls data
    //pixies[1] = new Pixy(0, 40, -10, 0, false); // Pixy is responsible for the block it controls

    //spriter = new Spriter();

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

/**
 * Collect function which should help in reducing allocation BS.
 */
public bool buildList(IMyTerminalBlock block)
{
    // If the block has the nametag or custom tag in its data
    if (block.CustomName.Contains(nameTag) || block.CustomData.Contains(customTag))
    {
        if (block is IMyShipController)
        {
            //Echo("Block count raw " + ((IMyTextSurfaceProvider)block).SurfaceCount);
            Controller check = new Controller((IMyShipController)block, customTag, useScriptFace);
            cocks.AddLast(check);
            // If we do not contain the key
            if (!groupAvgs.ContainsKey(check.getGroup()))
            {
                groupAvgs.Add(check.getGroup(), new WheelAvg()); // Use the zero vector to initialize
            }



            //Echo("block count surface = " + check.getSurfaces().SurfaceCount);
            //Echo("Block count check = " + check.getDisplay());
        }
        // Pistons
        if (block is IMyPistonBase)
        {
            proxyblocks.AddLast(new Piston((IMyPistonBase)block, customTag)); // Add to proxy blocks
        }
        // Motor stators
        if (block is IMyMotorStator)
        {
            proxyblocks.AddLast(new Rotor((IMyMotorStator)block, customTag)); // Add to proxy blocks
        }
        // Thrusters
        if (block is IMyThrust)
        {
            proxyblocks.AddLast(new Thruster((IMyThrust)block, customTag));
        }
        // Grav gens
        if (block is IMyGravityGeneratorBase)
        {

            proxyblocks.AddLast(new GravGen((IMyGravityGeneratorBase)block, customTag));
        }
        // Projectors
        if (block is IMyProjector)
        {
            proxyblocks.AddLast(new Projector((IMyProjector)block, customTag));
        }
        // Wheels
        if (block is IMyMotorSuspension)
        {
            Wheel wheel = new Wheel((IMyMotorSuspension)block, customTag);
            Vector3I pos = wheel.getBlock().getGridPos(Me.CubeGrid); // Grab position
            int group = wheel.getGroup();
            proxyblocks.AddLast(wheel);
            wheels.Add(wheel);
            // If we do not contain a group for the wheel we are adding
            if (!groupAvgs.ContainsKey(wheel.getGroup()))
            {
                // Add in a new average
                groupAvgs.Add(group, new WheelAvg(pos, 1)); // Get position relative to
            }
            // The key already exists and as a result we must update the respective average for this groups wheel position
            else
            {
                //groupAvgs[wheel.getGroup()].runningAvg(wheel.getBlock().getGridPos(Me.CubeGrid)); // Running average variant
                groupAvgs[group].addSum(pos); // Add to the sum
            }
        }
        // Lights
        if (block is IMyLightingBlock)
        {
            proxyblocks.AddLast(new Light((IMyLightingBlock)block, customTag));
        }
    }
    return false;
}

/** Checks to see if the block is still existing */
public bool Closed(IMyTerminalBlock block, IMyGridTerminalSystem script)
{

    if (block == null || block.WorldMatrix == MatrixD.Identity) return true;
    return !(script.GetBlockWithId(block.EntityId) == block);

}
/** save */
public void Save()
{

}
/**
         * Class responsible for computing the averages of a wheel relative to a group
         * and during run time
         */
class WheelAvg
{
    private Vector3I sum; // The summation of all vectors accounted for thus far
    private int count; // The number of elements computed so far
    /** Wheel avg default constructor */
    public WheelAvg() : this(Vector3I.Zero, 0) { }
    public WheelAvg(Vector3I sum) : this(sum, 1) { } // First element
    /** Wheel avg with control */
    public WheelAvg(Vector3I sum, int count)
    {
        this.sum = sum;
        this.count = count;
    }


    /** Add the next grid positional data and increment the count */
    public void addSum(Vector3I next)
    {
        sum += next;
        count++;
    }
    /** Remove from the current sum and decrement the count */
    public void remSum(Vector3I curr)
    {
        sum -= curr;
        count--;
    }
    /** Compute the average */
    public Vector3I getAvg()
    {
        return (count > 0) ? sum / count : Vector3I.Zero;
    }

    /** Compute the average with a relevant cube grid in mind */
    public Vector3I getAvg(IMyCubeGrid grid)
    {
        return grid.WorldToGridInteger(sum) / count;
    }
}
/*public void runner()
        {
            method(() => case1("Hey", 1));
        }

        public void case1(String check, int item)
        {
            Echo(check + ": " + item);
        }
        public void method(Action runCase)
        {
            runCase();
        }*/


// build with string
private void buildIt()
{
    if (Me.CustomData != null && Me.CustomData.Contains("tag=")) { this.nameTag = Me.CustomData.Substring(3); }
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    //init
    wheels = new List<Wheel>();
    cocks = new LinkedList<Controller>();

    proxyblocks = new LinkedList<BlockHandleI>(); // New list of block handle interfaces
    GridTerminalSystem.GetBlocks(blocks); // Get all the blocks
    // Build the lists
    foreach (IMyTerminalBlock block in blocks)
    {
        // Same grid check
        if (block.CustomName.Contains(nameTag))
        {
            if (block is IMyShipController) { cocks.AddLast(new Controller((IMyShipController)block, customTag, useScriptFace)); }
            // Pistons
            if (block is IMyPistonBase)
            {
                proxyblocks.AddLast(new Piston((IMyPistonBase)block, customTag)); // Add to proxy blocks
            }
            // Motor stators
            if (block is IMyMotorStator)
            {
                proxyblocks.AddLast(new Rotor((IMyMotorStator)block, customTag)); // Add to proxy blocks
            }
            // Thrusters
            if (block is IMyThrust)
            {
                proxyblocks.AddLast(new Thruster((IMyThrust)block, customTag));
            }
            // Grav gens
            if (block is IMyGravityGeneratorBase)
            {

                proxyblocks.AddLast(new GravGen((IMyGravityGeneratorBase)block, customTag));
            }
            // Projectors
            if (block is IMyProjector)
            {
                proxyblocks.AddLast(new Projector((IMyProjector)block, customTag));
            }
            // Wheels
            if (block is IMyMotorSuspension)
            {
                Wheel wheel = new Wheel((IMyMotorSuspension)block, customTag);
                proxyblocks.AddLast(wheel);
            }


        }
    }
    Echo("Recompiled.");
}

public K Method<K>(Func<K> enact)
{
    return enact();
}

public void Method(Action enact)
{
    enact();
}

public int someMethod(int ele1, int ele2)
{
    return ele1 + ele2;
}

public void someMethod(int ele1)
{
    Echo("Fuick you");
}
public int pass = 0;
/** The main argument */
public void Main(string argument, UpdateType updateSource)
{
    //Method(() => someMethod(1));
    //Func<int> c1 = () => someMethod(5, 2);
    //Action c2 = () => someMethod(2);
    //QueryResponse check = new QueryResponse("DO YOU WANT SOME CHEESE", 10);
    //check.runQuery<int>(cocks.First.Value.getCock(), false, c1, c2, 10);
    Echo("TransferCTRLS currently active and running\n\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\");
    runArgs(argument); // Process argument commands
    //Echo(cocks.ElementAt(0).listActs());
    workIt2(cocks); // Run the primary script

    //findGroups(); // uncomment if we should worry about updating groupings
}
/** control for running arguments */
public void runArgs(string arg)
{
    int attempt = 0;
    if (int.TryParse(arg, out attempt))
    {
        groupShift = attempt; // Readjust group ordering
        Echo("GS = " + groupShift);
    }
}






/** Secondary using the new classes */
public void workIt2(LinkedList<Controller> ctrls)
{
    // No controllers are detected
    if (ctrls == null) { return; }
    //bool needBuild = false;
    int cockLength = ctrls.Count; // Counts
    int proxLength = proxyblocks.Count; // ^
    LinkedListNode<Controller> ctrlN = ctrls.First; // Refference to the heads
    //Screen surfer; // A surface thats parsed during iteration
    StringBuilder build = new StringBuilder();
    // Cycle through the ship controllers
    for (int i = 0; i < cockLength && ctrlN != null; i++)
    {
        //Echo("tse");
        // TODO: Manual strinbuilder size will complicate string outs
        //StringBuilder builder = new StringBuilder(1000); // Build a string builder
        Controller cock = ctrlN.Value; // Get actual controller
        IMyShipController ctrl = cock.getCock();
        // Following code helps to make sure everything is properly delinked
        if (ctrl.Closed(this)) { ctrlN = delink<Controller>(ctrls, ctrlN); cockLength--; //Echo("Delink");
            continue; }
        if (!ctrl.IsUnderControl) { ctrlN = ctrlN.Next; continue; } // Skip to next node if controller isnt being used
        //surfer = cock.getSurface(); // Pass the default surface this cockpit is configured with to surfer
        int group = (ctrl.IsMainCockpit && groupShift != null) ? (int)groupShift : cock.getGroup();
        //PixyDust header = new TextWriter("TransferCTRLS link established with group " + group + ":\n", true); // Build header
        //pixies[0].setDust(header); // Set dust for the header
        // Parse group as either the shift or custom data read
        // Vector3D wheelAvg = avgWheels(wheels, group);
        //Echo("Currently Controlled = " + ctrl.CustomName + " for group " + group);
        Vector3I avg = (groupAvgs.ContainsKey(group)) ? groupAvgs[group].getAvg() : Vector3I.Zero;
        Vector3 moves = getMoves(ctrl); // Get the postional params of a controller
        Vector3I rolls = getRolls(ctrl); // Get rolls
        LinkedListNode<BlockHandleI> proxyN = proxyblocks.First; // THIS MUST BE REINSTATED EACH NEW LOOP
        int count = 0;
        for (int n = 0; n < proxLength && proxyN != null; n++)
        {
            BlockHandleI proxy = proxyN.Value; // Parse
            // Set build desire to true and skip if blocks broken
            if (proxy.closed(this)) { proxyN = delink<BlockHandleI>(proxyblocks, proxyN); proxLength--; continue; }
            proxy.enact(dir.lr, moves.X, group); // It doesnt matter what axis we pass, so long as it matches the one contained!
            proxy.enact(dir.ud, moves.Y, group); // ^
            proxy.enact(dir.fb, moves.Z, group); // ^
            proxy.enact(dir.pit, rolls.X, group);
            proxy.enact(dir.yaw, rolls.Y, group);
            proxy.enact(dir.rol, rolls.Z, group);
            proxy.enact(ctrl, group); // Sometimes, we want to use all aspects of the controller, like for projector blocks
            proxy.enact(Me, ctrl, group, avg); // This currently only has an effect on wheel average
            //Echo(proxy.getDebug());
            if (proxy.getGroup() == group)
            {
                count++;
                //builder.Append(proxy.getTermBlock().CustomName + ", ");
            }
            //TODO: Echo(proxy.getDebug());
            proxyN = proxyN.Next;
        }
        build.Append("Group " + group + " = " + count + " blocks controlled\n");
        //string val = builder.ToString();
        //Echo("New line position calculated at " + val.LastIndexOf("\n", val.Length - 1));
        //TextWriter checkers = new TextWriter(builder, true);
        // Operate on the surface should it exist
        /*if (surfer != null)
                {
                    checkers.setCutoff(surfer.getScreen().SurfaceSize * 3/2); // Set cutoff to be twice the surface size

                    pixies[1].setDust(checkers);

                    spriter.flush(surfer, pixies);
                    Echo("Debug = " + pixies[1].getDebug());
                }*/
        ctrlN = ctrlN.Next; // Next node
    }
    // If we found items to control
    if (build.Length > 0)
    {
        Echo("Control List;\n");
        Echo(build.ToString());
    }
    // Default echo case
    else
    {
        Echo("No blocks are currently being controlled. If you think this is an error, check for any spelling mistakes and mishaps in custom data");
    }
}




/** Delinks a node safely */
public LinkedListNode<T> delink<T>(LinkedList<T> list, LinkedListNode<T> node)
{
    LinkedListNode<T> next = node.Next; // Pass old
    list.Remove(node); // Delete    old
    return next;
}






/**
         * Controller class for holding, handling, and managing controllers and any screens they may have
         */
public class Controller
{
    private readonly int group; // Holds the group number that this controller controls
    private int groupActual; // This group is the shifted value, and the one that is read
    private readonly IMyShipController cock; // Ship Controller
    private int display; // The default display transferCTRLS will use
    //private readonly Screen[] displays; // The displays this controller can access
    // TODO: private Vector3D wheelAvg; // In theory, each controller will be assigned to one group. When this happens, a wheel average can then be calculated
    // TODO: private bool updatedAvg; // This is to signify to other elements that this component needs its average updated.
    // And stored straight to the controller to persist between runs
    public Controller(IMyShipController pass, String tag, bool useScriptFace)
    {
        cock = pass; // Pass in the controller actual
        //displays = Screen.genScreens((IMyTextSurfaceProvider)cock); // Generate displays against screens
        StringBuilder check = new StringBuilder(cock.CustomData);
        check.RemoveTag(tag); // Remove tag
        String[] args = check.split(); // Split with whitespace
        group = -1; // Default
        display = 0; // ^
        // Parse out the display num
        if (args.Length >= 2)
        {

            int.TryParse(args[1], out display); // Attempt to process display
        }
        // Parse out group number
        if (args.Length >= 1)
        {
            int.TryParse(args[0], out group); // Attempt to process index
            groupActual = group; // Read in group actual
        }

        // If the display we want to use is greater than the number of surfaces
        //if (display >= (displays.Length))
        {
            display = 0; // Use the default surface

        }
        //foreach(Screen screen in displays)
        {
            //screen.cleanSurface();
        }


        //getSurface().primeSurface(useScriptFace); // Prime it for reading


    }

    /** Gets the ships controller */
    public IMyShipController getCock()
    {
        return cock;
    }
    /** Gets the group number */
    public int getGroup()
    {
        return groupActual;
    }
    /** Check if under control */
    public bool isUnderControl()
    {
        return cock.IsUnderControl;
    }
    /** Check if this is a main cockpit */
    public bool isMainCockpit()
    {
        return cock.IsMainCockpit;
    }
    /** Setgroup using the default value */
    public void setGroup() { setGroup(group); }
    /** Setgroup will use the group variable as the default if none is specified */
    public void setGroup(int value)
    {
        groupActual = value; // Changing the group will also change
    }


    /** Gets the surfaces provider */
    public IMyTextSurfaceProvider getSurfaces()
    {
        return (IMyTextSurfaceProvider)cock;
    }
    /** Use built in variable */
    /*public Screen getSurface()
            {
                return getSurface(display);
            }*/
    /** Pass in magic */
    /*public Screen getSurface(int disp)
            {
                if (disp > displays.Length)
                {
                    return displays[0];
                }
                return displays[disp];
            }*/
    /** CLosed method */
    public bool Closed(Program prog)
    {
        return cock.Closed(prog);
    }
    /** Gets the frame of the display*/
    /*public MySpriteDrawFrame getFrame()
            {
                return getFrame(display);
            }*/
    /** Gets the draw FRAME Of the specific dispaly */
    /*public MySpriteDrawFrame getFrame(int disp)
            {
                return getSurface(disp).DrawFrame();
            }*/

    public int getDisplay()
    {
        return display;
    }




}





///////////////////////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////////////////////////
/** Gets the position relative of the reference block to the entire list*/
public Vector3D avgWheels(List<Wheel> wheels, int group)
{
    int wheelCount = (wheels.Count > 0) ? 0 : 1; // Counts the number of wheels
    Vector3D sumPos = Vector3D.Zero; // The positional sum
    // For all the wheels of the ship
    foreach (Wheel myWheel in wheels)
    {
        // Only add positions of wheels on the same level
        if (myWheel.getGroup() == group)
        {
            sumPos += myWheel.getBlock().GetPosition(); // Add pos
            wheelCount++; // Inc count
        }
    }
    if (wheelCount == 0) { wheelCount = 1; } // Avoid divide by zero errors
    return sumPos / wheelCount; // The average = the sum pos and count of wheels in same group
}





/** Get the moves */
private Vector3 getMoves(IMyShipController ctrl)
{
    return ctrl.MoveIndicator;
}
/** calculate rolls */
private Vector3I getRolls(IMyShipController ctrl)
{
    Vector2I normieRolls = new Vector2I((int)ctrl.RotationIndicator.X, (int)ctrl.RotationIndicator.Y);
    Vector3I rolls = new Vector3I(
        Math.Sign(normieRolls.X), Math.Sign(normieRolls.Y),
        ctrl.RollIndicator);
    return rolls;
}

//////////////////////////////////////////////////////////////////////////////////////
// Find and return a list of all blocks that are being controlled.

/*public void buildRelation(IMyTerminalBlock block, IMyMotorStator rote)
        {
            if rote.
        }*/

//public bool trueRelation(IMyTerminalBlock block, IMyMotorStator cock);



// Debugging
private void debug(string check)
{
    Echo(check);
    Me.CustomName = check;
}
/////////////////////////////////////
// Boolean for comparing grids
// Returns true if the same grid
private bool sameGrid(IMyTerminalBlock block)
{
    return block.CubeGrid == Me.CubeGrid;
}



///////////////////////////////////////////////////////////////////////////////
// Junkyard
////////////////////////////////
public class Node<T>
{
    private T data; // Data
    private Node<T> next; // Next
    private Node<T> prev; // Previous
    // default to init with default
    public Node() : this(default(T)) { }
    // Init with data, pass to sll build
    public Node(T data) : this(data, null) { }
    // SLL Init to dll
    public Node(T data, Node<T> next) : this(null, data, next) { }
    // DLL init
    public Node(Node<T> prev, T data, Node<T> next)
    {
        setData(data);
        setNext(next);
        setPrev(prev);
    }
    // Data set
    public void setData(T data) { this.data = data; }
    // next set
    public void setNext(Node<T> next) { this.next = next; }
    // Prev set
    public void setPrev(Node<T> prev) { this.prev = prev; }
    // Get data
    public T getData() { return data; }
    // Get next
    public Node<T> getNext() { return next; }
    // Get prev
    public Node<T> getPrev() { return prev; }
    // Object equals
    public override bool Equals(object check)
    {
        // Not null and same class
        if (check != null)
        {
            // Check is a node
            if (check is Node<T>)
            {
                Node<T> temp = (Node<T>)check;
                return temp.Equals(this);
            }
            // Check is object
            if (check is T)
            {
                T temp = (T)check;
                return temp.Equals(data);
            }
        }
        return false;
    }
    /** Check data equivalence */
    public bool Equals(Node<T> check)
    {
        return check != null && this.Equals(check.getData());
    }
    /** Check data direct equivalence */
    public bool Equals(T check)
    {
        return check != null && check.Equals(data);
    }

    public override int GetHashCode()
    {
        var hashCode = 1674795341;
        hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(data);
        hashCode = hashCode * -1521134295 + EqualityComparer<Node<T>>.Default.GetHashCode(next);
        hashCode = hashCode * -1521134295 + EqualityComparer<Node<T>>.Default.GetHashCode(prev);
        return hashCode;
    }
}

public class DLL<T> : IEnumerable<T>
{
    private int count; // Count
    private Node<T> head; // Head Refference
    private Node<T> tail; // Tail refference points to end node thats doubly linked to allow tail adjust
    // New sll
    public DLL()
    {
        count = 0;
        head = null;
        tail = null;
    }
    /** Removing node */
    public void remove(Node<T> node)
    {
        if (node == null) { return; }
        Node<T> prev = node.getPrev(); // Store old refferences
        Node<T> next = node.getNext(); // ^
        if (node.Equals(head))
        {
            head.setNext(node.getNext());
            head.setPrev(null);
        }
        else if (node.Equals(tail))
        {
            tail.setPrev(node.getPrev());
            tail.setNext(null);
        }
        else
        {
            prev.setNext(next);
            next.setPrev(prev);
        }
        count--; // Decrement
        //return node;
    }
    /** Data param to node param for tail ref */
    public void Add(T data) { Add(new Node<T>(data)); }
    /** Insert at tail */
    public void Add(Node<T> node)
    {
        node.setPrev(tail);
        node.setNext(null); // Just in case
        tail.setNext(node);
        tail = node; // Update tail refference
        count++;
    }
    /** data param to node param for head ref */
    public void AddHead(T data) { AddHead(new Node<T>(data)); }
    /** insert head */
    public void AddHead(Node<T> node)
    {
        node.setNext(head);
        node.setPrev(null); // Just in case
        head.setPrev(node);
        head = node; // Update head refference
        count++;
    }
    /** Head ref */
    public Node<T> getHead()
    {
        return head;
    }
    /** Tail ref */
    public Node<T> getTail()
    {
        return tail;
    }
    /** go find data */
    public bool contains(Node<T> data)
    {
        // Dont work on false
        if (data == null) { return false; }
        Node<T> temp = head;
        for (int i = 0; i < count && !temp.Equals(data); i++)
        {
            temp = temp.getNext(); // Next
        }
        // Return data results
        return data.Equals(temp);
    }
    // Data form
    public bool contains(T data) { return contains(new Node<T>(data)); }


    /** Get node at */
    public Node<T> getNodeAt(uint pos)
    {
        if (pos > count) { return null; }
        // Work from tail to middle
        Node<T> temp = null;
        // work from behind
        if (pos > count / 2)
        {
            temp = tail;
            // Need to shift pos by one to account for indexing at 0
            for (int i = count - 1; i > count / 2 && pos != i; i--)
            {
                temp = temp.getPrev();
            }
        }
        // Work from front
        if (pos <= count / 2)
        {
            temp = head;
            // No shift needed?
            for (int i = 0; i < count / 2 && pos != i; i++)
            {
                temp = temp.getNext();
            }
        }
        return temp;
    }
    /** Element at */
    public T getEleAt(uint pos)
    {
        if (pos > count) { return default(T); }
        return getNodeAt(pos).getData();
    }
    /** Get count */
    public int getCount()
    {
        return count;
    }
    /** Build lists statically */
    static public List<Type> toList(DLL<Type> list)
    {
        List<Type> result = new List<Type>();
        Node<Type> temp = list.getHead();
        for (int i = 0; i < list.getCount() && temp != null; i++)
        {
            result.Add(temp.getData());
            temp = temp.getNext();
        }
        return result;
    }

    public List<T> toList(DLL<T> list)
    {
        List<T> result = new List<T>();
        Node<T> temp = list.getHead();
        for (int i = 0; i < list.getCount() && temp != null; i++)
        {
            result.Add(temp.getData());
            temp = temp.getNext();
        }
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T data in toList(this))
        {
            yield return data;
        }

    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}

/** Defines the interface for typical block handles.
 * This allows us to build more informative and isolated lists, but still allow us
 * to synchronously act upon all custom blocks
 */
public interface BlockHandleI
{
    /** Gets the block of the respective type */
    //T getBlock();
    /** Returns the terminal block representation */
    IMyTerminalBlock getTermBlock();
    /** Gets the grouping of the block */
    int getGroup();
    /** Gets the type of the block */
    Type getType();
    /** Gets the axis */
    dir? getAxis();
    /** Performs the respective actions stored here.
             * axis = the axial direction we are to act on
             * move = the intensity of that direction
             */
    void enact(dir axis, float move, int group);
    /** This version of enact commands will attempt to utilize all aspects of the
             * A controller, such as in the case of needing move indicators, roll, and rotation indicators
             */
    void enact(IMyShipController ctrl, int group);
    /** Runs an enaction with a passing list */
    void enact(IMyShipController ctrl, int group, List<Object> list);
    /** Runs an enaction with a vector3D */
    void enact(IMyProgrammableBlock Me, IMyShipController ctrl, int group, Vector3D vect);
    /** Loads the block from data */
    string[] buildFromData(string customTag);
    /** Checks if block direction is flipped */
    bool isFlipped();
    /** GEt custom data */
    string getData();
    /** Gets debug info so it can be ported to echo outs*/
    string getDebug();
    /** Sets debug info so it can be seen in echo outs */
    void setDebug(string debug);
    /** check to see if the element is closed */
    bool closed(MyGridProgram script);
}
/** Axial directions and rotational inputs */
public enum dir
{
    lr, ud, fb, pit, yaw, rol
}

/** Attempts to find the dir based on string */
public static dir? GetDir(string check)
{
    if (check.Equals(dir.fb.ToString()))
    {
        return dir.fb;
    }
    if (check.Equals(dir.lr.ToString()))
    {
        return dir.lr;
    }
    if (check.Equals(dir.ud.ToString()))
    {
        return dir.ud;
    }
    if (check.Equals(dir.pit.ToString()))
    {
        return dir.pit;
    }
    if (check.Equals(dir.yaw.ToString()))
    {
        return dir.yaw;
    }
    if (check.Equals(dir.rol.ToString()))
    {
        return dir.rol;
    }
    return null;
}

/** The generic class of a block handle */
//public class BlockHandle<T> : BlockHandleI<IMyTerminalBlock>
public abstract class BlockHandle<T> : BlockHandleI where T : IMyTerminalBlock
{
    private T block; // The block thats stored here
    private int group; // The group this block belongs to
    private dir? axis; // The axis in question
    private bool flip; // The condition that this elements directional input is flipped
    private string customTag; // Custom tag for rebuilding if necessary
    private string debug; // Holds debugging information if needed to echo out stuffs
    /** Constructor that initializes control schemas */
    public BlockHandle(T block, string customTag)
    {
        this.block = block;
        this.customTag = customTag;
        string[] data = buildFromData(customTag);
        group = -1; // Default
        if (data.Length < 3)
        {
            axis = null;
            flip = false;
            return;
        }
        //group = (int.TryParse(temp2[0], out group)) ? group : -1;
        int.TryParse(data[0], out group);
        axis = GetDir(data[1]);
        flip = (bool.TryParse(data[2], out flip)) ? flip : false;
    }
    /** Builds elements from custom data */
    public string[] buildFromData(string customTag)
    {
        StringBuilder temp = new StringBuilder(block.CustomData);
        temp.RemoveTag(customTag);
        return temp.split(); // Split on whitespace
    }

    /** Enact does nothing on the generic version */
    public virtual void enact(dir axis, float move, int group)
    {
    }
    /** ^ */
    public virtual void enact(IMyShipController ctrl, int group)
    {
    }
    // This will attempt to parse out the various versions of enact
    public virtual void enact(IMyShipController ctrl, int group, MemorySafeList<Object> list)
    {
        if (list is MemorySafeList<BlockHandle<T>>)
        {
            enact(ctrl, group, list as MemorySafeList<BlockHandle<T>>);
        }
    }
    /** To be overrided */
    public void enact(IMyShipController ctrl, int group, List<BlockHandle<T>> list) { }
    /** To be overrided */
    public virtual void enact(IMyProgrammableBlock Me, IMyShipController ctrl, int group, Vector3D vect) { }
    /** Gets axis */
    public dir? getAxis()
    {
        return axis;
    }
    /** Gets blocks */
    public T getBlock()
    {
        return block;
    }
    /** Gets group order */
    public int getGroup()
    {
        return group;
    }
    /** Returns terminal version */
    public IMyTerminalBlock getTermBlock()
    {
        return (IMyTerminalBlock)block;
    }
    /** Get the type of the block stored */
    public Type getType()
    {
        return block.GetType();
    }
    /** Checks to see if we are the same axis and group */
    public bool sameControl(dir axis, int group)
    {
        return axis.Equals(getAxis()) && group == getGroup();
    }
    /** Return flip status */
    public bool isFlipped()
    {
        return flip;
    }
    /** Gets the blocks custom data */
    public string getData()
    {
        return block.CustomData;
    }

    /** Gets the blocks debug info */
    public string getDebug()
    {
        return debug;
    }
    /** sets debugging */
    public void setDebug(string msg)
    {
        debug = msg;
    }
    /** Checks if the grids closed */
    public bool closed(MyGridProgram script)
    {
        return (block == null || block.WorldMatrix == MatrixD.Identity || !(script.GridTerminalSystem.GetBlockWithId(block.EntityId) == getTermBlock()));
    }

}
///////////////////////////////////////////////////////////////////////////////////////////
// Below are some custom classes that are of type BlockHandleI
// Each documenting a special instance of enact which defines how this particular
// Block should behave when called. This allows new classes to be defined in a streamlined manner.
/** Rotos conditional actions */
public class Rotor : BlockHandle<IMyMotorStator>
{
    // Use base cast
    public Rotor(IMyMotorStator rotor, string customTag) : base(rotor, customTag) { }
    /** Override parent to use this version */
    public override void enact(dir axis, float move, int group)
    {
        bool isFlipped = base.isFlipped(); // Get flip status
        // If we arent the same axis as movement, or we arent on the same group
        if (!sameControl(axis, group)) { return; }
        IMyMotorStator rotor = getBlock(); // grab the block
        rotor.Enabled = move != 0;  // Turn on if we are moving
        // Rotors should rotate clockwise on positive, counter on neggative
        // Pos veloc = clock
        // Set clockwise
        if (move > 0 && isFlipped)
        {
            rotor.TargetVelocityRad = -1 * Math.Abs(rotor.TargetVelocityRad);
        }
        else if (move > 0)
        {
            rotor.TargetVelocityRad = Math.Abs(rotor.TargetVelocityRad);
        }
        // Left controls
        else if (move < 0 && isFlipped)
        {
            rotor.TargetVelocityRad = Math.Abs(rotor.TargetVelocityRad);
        }
        else if (move < 0)
        {
            rotor.TargetVelocityRad = -1 * Math.Abs(rotor.TargetVelocityRad);
        }
        // Turn off to save power and lock movement
        else
        {
            rotor.Enabled = false;

        }
    }
}
/** Pistons conditional actions */
public class Piston : BlockHandle<IMyPistonBase>
{
    // Use base cast for the piston
    public Piston(IMyPistonBase piston, string customTag) : base(piston, customTag) { }
    /** Override parent to use this version */
    public override void enact(dir axis, float move, int group)
    {
        bool isFlipped = base.isFlipped(); // Get flip status
        // If we arent the same axis as movement, or we arent on the same group
        if (!sameControl(axis, group)) { return; }
        IMyPistonBase piston = getBlock();
        piston.Enabled = move != 0; // Turn on
        // Left = -1, Right = 1
        if (move > 0 && isFlipped)
        {
            piston.Retract();
        }
        // extend
        else if (move > 0)
        {
            piston.Extend();
        }
        // Left controls
        else if (move < 0 && isFlipped)
        {
            piston.Extend();
        }
        else if (move < 0)
        {
            piston.Retract();
        }
        // Turn off to save power and lock movement
        else
        {
            piston.Enabled = false;
        }
    }
}

/** Defines thrusters*/
public class Thruster : BlockHandle<IMyThrust>
{
    /** constructor */
    public Thruster(IMyThrust thrust, string customTag) : base(thrust, customTag) { }
    /** Enactions override */
    public override void enact(dir axis, float move, int group)
    {
        IMyThrust thrust = getBlock(); // Get thrust
        bool isFlipped = base.isFlipped(); // Get flip
        if (!sameControl(axis, group)) { return; } // Dont work on bad cases
        // Thrusters should always be enabled, but not mandated by the script.

        // Move is forward but we are flipped
        if (move > 0 && isFlipped)
        {
            thrust.ThrustOverridePercentage = 0.001f; // minimal activation
        }
        // Move is forward
        else if (move > 0)
        {
            thrust.ThrustOverridePercentage = 1; // Max activation
        }
        // Move is backwards but flipped
        else if (move < 0 && isFlipped)
        {
            thrust.ThrustOverridePercentage = 1; // Max act
        }
        // Move is backwards
        else if (move < 0)
        {
            thrust.ThrustOverridePercentage = 0.001f; // Minimal activation
        }
        // Turn percentage to 0 activating dampeners
        else
        {
            thrust.ThrustOverride = 0;
        }
    }
}
/** Grav gen controls */
public class GravGen : BlockHandle<IMyGravityGeneratorBase>
{

    /** constructor */
    public GravGen(IMyGravityGeneratorBase grav, string customTag) : base(grav, customTag) { }
    /** Enaction */
    public override void enact(dir axis, float move, int group)
    {
        bool isFlipped = base.isFlipped();
        // Check ctrls
        if (!base.sameControl(axis, group)) { return; }
        IMyGravityGeneratorBase grav = getBlock();
        grav.Enabled = move != 0; // Turn on relative to movement vector
        // Forward and flipped = reverse
        if (move > 0 && isFlipped)
        {
            grav.GravityAcceleration = int.MinValue;
        }
        // Forward
        else if (move > 0)
        {
            grav.GravityAcceleration = int.MaxValue;
        }
        // Backwards and flipped = forward
        else if (move < 0 && isFlipped)
        {
            grav.GravityAcceleration = int.MaxValue;
        }
        // Backswards
        else if (move < 0)
        {
            grav.GravityAcceleration = int.MinValue;
        }
        // turn off
        else
        {
            grav.Enabled = false;
        }
    }
}

/** Projector controls */
public class Projector : BlockHandle<IMyProjector>
{
    /** constructor */
    public Projector(IMyProjector proj, string customTag) : base(proj, customTag) { }
    /** enact with all vectors */
    public override void enact(IMyShipController ctrl, int group)
    {
        // God damnit keen, fix your normalizations
        Vector2I normieRolls = new Vector2I((int)ctrl.RotationIndicator.X, (int)ctrl.RotationIndicator.Y);
        IMyProjector proj = getBlock();
        if (getGroup() != group) { return; }
        Vector3I rolls = new Vector3I(
            //(normieRolls.X != 0) ? Math.Sign(normieRolls.X) : 0,
            //(normieRolls.Y != 0) ? Math.Sign(normieRolls.Y) : 0,
            Math.Sign(normieRolls.X), Math.Sign(normieRolls.Y),
            ctrl.RollIndicator);
        //Vector3I rolls = new Vector3(normieRolls, ctrl.RollIndicator); // Stores everything to rolls
        // Builds a new moves
        proj.ProjectionOffset = new Vector3I(ctrl.MoveIndicator.X + proj.ProjectionOffset.X,
            ctrl.MoveIndicator.Y + proj.ProjectionOffset.Y,
            ctrl.MoveIndicator.Z + proj.ProjectionOffset.Z);
        // Projector rolls are a bit wonky, since they only take -2 to 2. (But for some reason take float values???)
        //setDebug("Projection = " + (((int)(proj.ProjectionRotation.X + 2 + rolls.X) % 5) - 2));
        //setDebug("Projection = " + rolls + " rotes = " + ctrl.RotationIndicator);
        proj.ProjectionRotation = new Vector3I(
            ((int)(proj.ProjectionRotation.X + 2 + rolls.X) % 5) - 2,
            ((int)(proj.ProjectionRotation.Y + 2 + rolls.Y) % 5) - 2,
            ((int)(proj.ProjectionRotation.Z + 2 + rolls.Z) % 5) - 2
            );


        //setDebug("Projection = " + proj.ProjectionRotation);
    }
}
/** Wheel handles */
public class Wheel : BlockHandle<IMyMotorSuspension>
{
    private double str; // Strength before
    /** Constructor */
    public Wheel(IMyMotorSuspension wheel, string customTag) : base(wheel, customTag)
    {
        str = wheel.Strength;
    }
    /** Enaction with all vectors */
    public override void enact(IMyProgrammableBlock Me, IMyShipController ctrl, int group, Vector3D avgWheels)
    {
        setDebug("Yo avgwheels = ");
        setDebug(getDebug() + avgWheels.ToString());

        if (getGroup() != group)
        {
            // Dont work on values that are already 0
            if (getOverProp() == 0 && getOverSteer() == 0)
            {
                return;
            }
            setOverProp(0);
            setOverSteer(0);
            return;
        }
        IMyMotorSuspension wheel = getBlock();
        Vector3D ctrlPos = ctrl.GetPosition();
        Vector3D wheelPos = wheel.GetPosition();
        Vector3 moves = ctrl.MoveIndicator;
        // Brakes are true if we are pressing space, or the cockpit has handbrake set
        bool brakes = moves.Y > 0 || ctrl.HandBrake;
        float strength = wheel.Strength; // Temporairly holds strength
        // Velocity is used to calculate braking
        Vector3D velocity = Vector3D.TransformNormal(ctrl.GetShipVelocities().LinearVelocity, MatrixD.Transpose(ctrl.WorldMatrix)) * 0.1f;
        setDebug("Pos: " + wheel.GetPosition() + "| WorldFord: " + ctrl.WorldMatrix.Forward);
        setDebug("Avg: " + avgWheels + "\nPos: " + wheel.getGridPos(ctrl.CubeGrid) + "\nCubeFord: " + ctrl.WorldMatrix.Forward);
        // Store the multiplier of the controller
        float steerDir = Math.Sign(Math.Round(Vector3D.Dot(wheel.WorldMatrix.Forward, ctrl.WorldMatrix.Up), 2))
              * Math.Sign(Vector3D.Dot(wheel.GetPosition() - avgWheels,
                //new Vector3D(ctrl.CubeGrid.WorldToGridInteger(ctrl.WorldMatrix.Forward))));
                ctrl.WorldMatrix.Forward));
        Math.Sign(Vector3D.Dot(wheel.GetPosition() - avgWheels, ctrl.WorldMatrix.Forward));
        // the WorldToGridInt with world matrix forward converts the grids forward. Using just worldMatrix.forward will alternate inputs relative to user direction
        Math.Sign(Vector3D.Dot(wheel.GetPosition(), ctrl.WorldMatrix.Forward));
        // Store the propulsion multiplier of the controller
        int propDir = -Math.Sign(Math.Round(Vector3D.Dot(wheel.WorldMatrix.Up, ctrl.WorldMatrix.Right), 2));

        // Was originally if braked, use propDir * velocity.Z,
        float propVal = (brakes) ? (float)(propDir * velocity.Z) : propDir * wheel.Power * -moves.Z; // Calculates override propulsion
        // If we had hit brakes because we hit space or the handbrake was toggled on
        //int steerInv = 1;
        // Im struggling to figure out what ctrl schema is what
        setDebug("Avg: " + avgWheels + "\nPos: " + wheel.getGridPos(ctrl.CubeGrid) + "\nDir: " + propDir + "\nCubeFord: " + ctrl.WorldMatrix.Forward);

        float steerVal = steerDir * moves.X * 100;
        // We hit duck so we jump!
        if (moves.Y < 0)
        {
            // Increase by 25%
            wheel.Strength = 0; // Lower to rise!
            wheel.Strength = (float)(.25f * strength + wheel.Strength);
        }
        //ctrl.HandBrake = moves.Y > 0 || ctrl.HandBrake;
        // If we are using any move inputs
        if (moves.AbsMax() > 0)
        {

            setOverProp(propVal);
            setOverSteer(steerVal);
        }
        else
        {
            setOverProp(0);
            setOverSteer(0);
        }
        wheel.InvertSteer = true;
        //wheel.InvertSteer = (propDir < 0) ? true : false;
        // Restore to normal str
        wheel.Strength = strength; // Pass old val back
    }
    /** Sets propulsion override */
    public void setOverProp(float val) { getBlock().SetValue("Propulsion override", val); }
    /** Sets steering override */
    public void setOverSteer(float val) { getBlock().SetValue("Steer override", val); }
    /** Get over prop */
    public float getOverProp() { return getBlock().GetValue<Single>("Propulsion override"); }
    /** Get over steer */
    public float getOverSteer() { return getBlock().GetValue<Single>("Steer override"); }
}

/** Should encompass all lighting blocks */
public class Light : BlockHandle<IMyLightingBlock>
{
    /** Constructor */
    public Light(IMyLightingBlock block, string customTag) : base(block, customTag)
    {
        // Nothing to do.
    }
    /** Light blocks should activate when input is supplied regardless of direction.
             * The only difference is that isFlipped dictates if the light should turn on, or turn off
             * when the thing is activated. The assumption should then become that isFlipped will instead
             * turn off lights when the movement is detected.
             */
    public override void enact(dir axis, float move, int group)
    {
        IMyLightingBlock light = getBlock(); // Im seriously wondering if I should just make it protected.
        // But then again, its easy being able to work with methods as opposed to all the variables. Keeps things organized.
        bool isFlipped = base.isFlipped(); // Get flip
        if (!sameControl(axis, group)) { return; } // Dont work on bad cases

        // Stationary case
        if (move == 0)
        {
            light.Enabled = isFlipped; // If we are flipped, we want it on. If we arent, we want it off
        }
        else
        {
            light.Enabled = !isFlipped; // Use the reverse case for changing status.
        }
    }
}

/** This class is responsible for debugging classes that dont have access to the echo method directly.
         * It is to this end Echo Debugging can be made possible in a simple and easy manner
         */
public class Debug
{
    private string debug; // Debugging information to be echoed
    /** Nothing to do for this constructor. All work will be done with the getter and setters */
    public Debug() { }
    /** Get the debug data */
    public string getDebug() { return debug; }
    /** Set the debug data */
    public void setDebug(string data) { debug = data; }
    /** Add to the debug data, prepend with newline */
    public void addDebug(string data)
    {
        debug = debug + "\n" + data;
    }
}



/**
         * This library is responsible for building a query/response system in order to coordinate system changes to a platform
         * The original use case extended from the transferCTRLS dillemma of a user passing input requesting a system change regarding
         * their target block, but information about the block or person is forgoed. The appending of an identifier to a command is
         * the simple case, but in the long run we lose out on the ability to expand to unaccounted for user cases.
         *
         * Remember, the GUI HUD (hotbar) for a cockpit can be given blocks with specific commands in order to function. However, if a script
         * were to rely on the idea of "pass identifier to command" and leave that as an executable action in said hotbar, this identifier
         * would have to be re-defined for each and every new edition that would need to make use of this feature. In the long run this means that
         * an end user will have to know more about the blocks they are activating, add more to the command they would originally be sending, and as
         * a result is prone to errors both in spelling or identification.
         *
         * An alternate idea recently came to light. Suppose a command was sent out to change, but instead of appending the identifier, we bank on the
         * blocks own ability to toggle on/off some aspect of itself as the response? For example, the HUD switch is often left in the exact same state because
         * its often defined to a build or only used if someone needs to make repairs. So what we can do is that when a query goes out, a person can send their
         * response in the form of a toggle (DILLEMMA*). Now when we are tallying which blocks should be changed, we already have a value set to the block that can be
         * checked for the users response, apply the actions appropriately, and restore the block back to the inital state. We avoid the issues addressed previously regarding
         * error and deployment, but we also have the bonus of having sweeping changes take effect for everyone who wants the new changes. To elaborate on this perk, say someone
         * queried for new changes to take effect. Another person using the system upon receiving the prompt who may not have asked for changes but may still want to use them
         * anyways. This could work wonders in systems where a compliancy register is needed, like a mission briefing or a security activation requiring all personel response.
         *
         * /////////////////////////////////////////////////////////////////////////////////////
         *
         * (*DILLEMMA: Do we implement toggle affirmation, where yes is a toggle, and no is a no toggle, or do we record the previous
         * (states, set the default response to no by deactivating the switch, and then have the end user switch to the state they want?
         * (Or do we make use of two different switches where their individual control corresponds to yes and no?
         *
         * (RESOLUTION: The second implementation runs an issue where the state change can not be recorded if the default state happens to be the one the user wants
         * (so indefinite prompts would fail in this case. However the use of two different switches resolves this as each individual switch can be monitored as the response.
         * (However the issue here is that we potentially, and likely severely limit our options of blocks able to respond to this request as not all blocks have the same switch
         * (controls. For example, all blocks will have a HUD switch, but not all blocks will have a conveyor system switch. This is why the the toggle implementatino would shine
         * (as only one switch control would have to be used, where the default response is set to the on/off state.
         *
         * A concern for actual implementation is how do we go about activations? Do we host the loop here or do we host it outside of? Technically we may run the query/response
         * across a list but we may lose the ability for custom messages. Another loss is in the realm of efficiency where the process of querying may not be a defined function
         * and as a result we would have to loop through the entire list in order to tally who does what. And in the process of this querying, we should also execute and apply actions
         * to the corresponding blocks. This means that functions and methods should be passed INTO this class in order to execute.
         *
         * For sake of complexity and usecase the above ideas I wont really bother dealing with at the moment. The knowledge to actually do the passing of functions is currently lost
         * to me, and is not as important. Implementing the timer toggle seems to be the best and easily manageable task. Relook into yields and counters
         */














/**
         * A query/response class should serve as a way to tally a block. It is to only hold the decision prompt and quite possibly the actions it should execute on the conditions
         * that the reading switch is using.
         *
         * What counts as the object making the choice must be an IMyTerminalBlock. This is the only way we can verify the integrity.
         * Generic arguments T and K refer to the types that a particular case happens. A query response is ussually responsible for a specific type and a
         * specific return class if at all valid. Ussually, K is void, but there may be cases where we are required to return k for whatever reason.
         */
public class QR
{
    private readonly IMyProgrammableBlock Me; // Pass back readonly
    private String query; // The query of an item
    //private Func<T, K> truthCase; // The truth case
    //private Func<T, K> falseCase; // The false ^
    //TODO: private Response response; // The response case
    private float limit; // The amount of time a person has to answer a query
    private float timePassed; // The amount of time passed
    // This dictionary both acts as a "HasSeen" and a "restoreDefaults"
    // in the use of loops, a block is expected to reappear, and assuming no duplicates, will have appeared exactly twice within 2 iterations.
    // Thus, when we see our first duplicate block, we can lock down the ability to add any more elements to the dictionary.
    // When a user makes a decision, this can be immediately queried, restore the default, and removed from the dictionary.
    // This prevents us from having to requery the element again as a user has already chosen what they wanted to have happen.
    private Dictionary<IMyTerminalBlock, bool> defaults; // ^
    private bool hasPassed; // holds the check to see if the dictionary has been iterated once

    /** Constructor */
    public QR(IMyProgrammableBlock pass, String question, float limit)
    {
        Me = pass; // Pass back PB for data manipulation
        query = question; // Store the question
        hasPassed = false;
        timePassed = 0;
        this.limit = limit;
        defaults = new Dictionary<IMyTerminalBlock, bool>(); // Init
    }
    /** This method holds the priming logic and dictionary control so its not repeated in the varying runQuery methods */
    private void queryPrimer(IMyTerminalBlock block, bool initBool)
    {
        // If the dictionary does not contain the passed key, or we have not asked for this key already
        if (!hasPassed && !defaults.ContainsKey(block))
        {
            defaults.Add(block, block.ShowOnHUD); // Add the block to the dictionary with the hud switch acting as control
            block.ShowOnHUD = initBool; // Set the hud to init bool

        }
        // If the dictionary does contain the key
        else
        {
            hasPassed = true; // It is assumed we have passed once and thus we no longer need to iterate, so by setting to true we lock from adding new entries

        }
    }
    /** Sets responses so its viewable on a screen relative to the configuration passed */
    private void setResponse(IMyTerminalBlock block, String query)
    {
        // Handles with an IMyTextSurface provider
        if (block is IMyTextSurfaceProvider)
        {
            IMyTextSurface cock = ((IMyTextSurfaceProvider)block).GetSurface(0);
            cock.WriteText(query); // Write text to screen
            cock.Alignment = TextAlignment.LEFT;
            cock.ContentType = ContentType.SCRIPT;

        }
    }

    /**
             * The following methods are designed around the following principle; there may be instances where we are running non-void methods
             * as well as methods that are indeed void. What we actually do with the value can be ascertained by the compiler itself and not of
             * our own violition. This allows us to call runQuery against a variety of actions or fully defined delegates while still using the same namespace.
             *
             * Invoking them would be as follows;
             * Action act = () => someMethodVoid(param1, param2); // Where act stores the defined action
             * Func<K> del = () => someMethodReturn(param1, param2); // Where del stores a returning function of type k
             *
             * This method is where both methods are return types
             */
    public void runQuery<K>(IMyTerminalBlock block, bool initBool, Func<K> truthCase, Func<K> falseCase, int secondsPassed)
    //public void runQuery(IMyTerminalBlock block, bool initBool, Action<>)
    {
        queryPrimer(block, initBool); // Query primer
        // If the initial boolean has been changed by the 'sHUD' switch
        if (initBool != block.ShowOnHUD)
        {
            // We perform runcase with the truthCase and false case swapped
            runCase(falseCase, truthCase, initBool);
            // Because we have already decided which case to run, we can flush the query from the block
            flushQuery(block);

        }
        // We have surpassed the time limit, so execute the casehoods
        else if (timePassed >= limit)
        {
            // We perform RunCase with a 1:1 execution as documented by runCase
            runCase(truthCase, falseCase, initBool);
            flushQuery(block); // Flush the query
        }
        // No decision has been made, so increment the time passed
        timePassed += secondsPassed;
    }
    /** runQuery version where false case is an Action. See documentation of full delegate method for uncompressed form */
    public void runQuery<K>(IMyTerminalBlock block, bool initBool, Func<K> truthCase, Action falseCase, int secondsPassed)
    {
        queryPrimer(block, initBool);
        if (initBool != block.ShowOnHUD) { runCase(falseCase, truthCase, initBool); flushQuery(block); }
        else if (timePassed >= limit) { runCase(truthCase, falseCase, initBool); flushQuery(block); }
        timePassed += secondsPassed;
    }

    /** runQuery version where true case is an Action. See documentation of full delegate method for uncompressed form */
    public void runQuery<K>(IMyTerminalBlock block, bool initBool, Action truthCase, Func<K> falseCase, int secondsPassed)
    {
        queryPrimer(block, initBool);
        if (initBool != block.ShowOnHUD) { runCase(falseCase, truthCase, initBool); flushQuery(block); }
        else if (timePassed >= limit) { runCase(truthCase, falseCase, initBool); flushQuery(block); }
        timePassed += secondsPassed;
    }

    /** runQuery version where both cases are an Action. See documentation of full delegate method for uncompressed form */
    public void runQuery(IMyTerminalBlock block, bool initBool, Action truthCase, Action falseCase, int secondsPassed)
    {
        queryPrimer(block, initBool);
        if (initBool != block.ShowOnHUD) { runCase(falseCase, truthCase, initBool); flushQuery(block); }
        else if (timePassed >= limit) { runCase(truthCase, falseCase, initBool); flushQuery(block); }
        timePassed += secondsPassed;
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /** Runs the passed cases relative to meeting the passed initial bool condition. Used exclusively for the above.
             * We have exactly two conditions to execute on; when the switch has been toggled, or when the switch remains the same.
             * We already check when the intial boolean has been flipped due to the outside conditional. So, we can change the parameters around
             * such that the cases are flipped for each of these conditions.
             *
             * IMPORTANT:
             * This will be written in the context of the initial boolean being false, and we are executing a false case
             * This version returns the value of either the truth case or the false case
             */
    private K runCase<K>(Func<K> truthCase, Func<K> falseCase, bool initBool)
    {
        // If the default case was false, we execute the false case
        if (!initBool)
        {
            return falseCase();
        }
        // The default case was true, we execute the truth case
        else
        {
            return truthCase();
        }
    }
    /** Runs case where the K returned is only the truth case */
    private K runCase<K>(Func<K> truthCase, Action falseCase, bool initBool)
    {
        // If the default case was false, we execute the false case
        if (!initBool) { falseCase(); return default(K); } // Return class default
        // The default case was true, we execute the truth case
        else { return truthCase(); } // Send truth case back
    }
    /** Runs case where K is returned on false case */
    private K runCase<K>(Action truthCase, Func<K> falseCase, bool initBool)
    {
        if (!initBool) { return falseCase(); } // Send false case back
        else { truthCase(); return default(K); } // Run truthcase and send back default
    }
    /** Runs case where no value needs to be returned*/
    private void runCase(Action truthCase, Action falseCase, bool initBool)
    {
        if (!initBool) { falseCase(); }
        else { truthCase(); }
    }
    ////////////////////////////////////////////////////////////////////////////////////
    /** Overload of the above where we instead
            private void runCase(Action truthCase, Action falseCase, bool initBool)
            {

            }
            /** Garbage collection to remove from dictionary */
    public void flushQuery(IMyTerminalBlock block)
    {
        // Do not work on null lists or blocks
        if (defaults == null || block == null) { return; }
        // If our dictionary contains the key
        if (defaults.ContainsKey(block))
        {
            block.ShowOnHUD = defaults[block]; // Restore default value
            defaults.Remove(block); // Remove the block from the dictionary
        }
    }
}
/** A QRBlock is basically an IMyTerminalBlock but with additional checks/conditions such that
        public class QRBlock
        {

        }

        /** Response type signifies what kind of response the q/r system will be using.
         * NO/YES
         */
enum Query
{
    prompt2, // Uses 2 switches to determine truth/false
    timer, // Uses a timer and 1 switch to figure out control option
    timerToggle // Uses a timer but instead operates on a toggle basis.
}
/**
         * Response signifies the natrue of a queries answer. For example, we may not want to perform our query case
         * until a sufficient number of responses have been generated.
         */
enum Response
{

}

}
static public class Extensions
{
    public static readonly MyDefinitionId Electricity = MyDefinitionId.Parse("MyObjectBuilder_GasProperties/Electricity");
    public static readonly char[] whitespace = { ' ', ',', '.', ';', '\t', '\'', '|', '\n', '\r' };
    // size value of 1000d coverts to kw.
    static public double CurrentPowerUse(this IMyTerminalBlock terminalBlock, double size) =>
        ((terminalBlock.Components.Get<MyResourceSinkComponent>() != null) ? size * terminalBlock.Components.Get<MyResourceSinkComponent>().CurrentInputByType(Electricity) : 0);
    static public double MaxPowerUse(this IMyTerminalBlock terminalBlock, double size) => size * terminalBlock.Components.Get<MyResourceSinkComponent>().MaxRequiredInputByType(Electricity);
    static public float OptimalMaxOutput(this IMyPowerProducer powerBlock) => powerBlock.Components.Get<MyResourceSourceComponent>().DefinedOutput;
    //static public float MaxPutAct(this IMyPowerProducer powerBlock, IMyGridTerminalSystem script) => (!Closed(powerBlock, script) && powerBlock.IsWorking) ? OptimalMaxOutput(powerBlock): 0;
    static public float MaxPutAct(this IMyPowerProducer powerBlock) => (powerBlock.IsWorking) ? OptimalMaxOutput(powerBlock) : 0;
    static public bool IsShooting(this IMyUserControllableGun gun) => gun.Enabled && (gun.IsShooting || gun.GetValue<bool>("Shoot"));
    static public bool Closed(this IMyTerminalBlock block, MyGridProgram script) => (block == null || block.WorldMatrix == MatrixD.Identity || !(script.GridTerminalSystem.GetBlockWithId(block.EntityId) == block));
    static public void setBrake(this IMyShipController ctrl, bool flip)
    {
        if (ctrl.HandBrake != flip)
        {
            ctrl.SetValueBool("HandBrake", flip);
        }
    }
    /** Converts the update frequency to seconds */
    /*static public float TicksToSecs(this UpdateFrequency tick)
        {
            UpdateFrequency.
            switch (tick)
            {
                case UpdateFrequency.Once:
                case UpdateFrequency.Update1:
                    return 0.0167f;
                case UpdateFrequency.Update10:
                    return 0.167f;
                case UpdateFrequency.Update100:
                    return 1.67f;
                default:
                    return 0;
            }
        }*/

    /** Ref grid should be the cube grid refference pulled from "Me" */
    static public Vector3I getGridPos(this IMyTerminalBlock block, IMyCubeGrid refGrid)
    {
        return refGrid.WorldToGridInteger(block.GetPosition());
    }

    /** The running average for a wheel/Multi-vectored approach
        * Like its one dimensional counterpart, a running average on a vector would entail
        * getting the avg value on all 3 dimensions, which entails treating each axis
        * as their own individual average.
        */
    static public Vector3D runningAvg(Vector3D priorAvg, Vector3D current, int count)
    {
        return priorAvg + (current - priorAvg) / count;
    }
    /** Using algebra and the same elements, we can reverse the above equation to find
         * the average if we were to remove 1 element. EQ = (n * m_(n-1) - a_n)/(n-1)
         */
    static public Vector3D fleeingAvg(Vector3D currAvg, Vector3D current, int count)
    {
        return (count * currAvg - current) / (count - 1);
    }


    /** list the properties of a terminal block in a string */
    static public string listProps(this IMyTerminalBlock block)
    {
        StringBuilder build = new StringBuilder();
        List<ITerminalProperty> props = new List<ITerminalProperty>();
        block.GetProperties(props);
        foreach (ITerminalProperty prop in props)
        {
            build.Append(prop.Id).Append(" - ").AppendLine(prop.TypeName);
        }
        return build.ToString();
    }
    /** List the actions of a terminal block in a string */
    static public string listActs(this IMyTerminalBlock block)
    {
        StringBuilder build = new StringBuilder();
        List<ITerminalAction> acts = new List<ITerminalAction>();
        block.GetActions(acts);
        foreach (ITerminalAction act in acts)
        {
            build.Append(act.Id).Append(" - ").AppendLine(act.Name.ToString());
        }
        return build.ToString();
    }
    /** Calculates where new lines should be inserted */
    static public int newLinePos(this IMyTextSurface surf, StringBuilder build)
    {
        // The number of pixels to chars is calculated below.
        return (int)(surf.SurfaceSize.X / surf.getPixRate(build)); // Calculates the number of chars can fit in this x-surface
    }
    /** Calculates the number of pixels to chars */
    static public float getPixRate(this IMyTextSurface surf, StringBuilder build)
    {
        return (surf.MeasureStringInPixels(build, surf.Font, surf.FontSize).X / build.Length); // Calculates the number of pixels to chars
    }

    //static public bool Closed2(this object thing) => thing.
    static public string[] split(this StringBuilder build)
    {
        return build.ToString().Split(whitespace);
    }
    /** Removes a tag from the string builder */
    static public void RemoveTag(this StringBuilder build, String tag)
    {
        String data = build.ToString();

        if (data.Contains(tag))
        {
            build.Remove(0, data.IndexOf(tag) + 6); // Removes everything from start up to end
            int pos = build.ToString().IndexOf('\n'); // Get the index of the next new line char
            build.Remove(pos, build.Length - pos); // Remove from the position of newline to end, isolating the data
        }
    }
    /**
         * Primes, cleans, prepares a surface for writing
         */
    static public void primeSurface(this IMyTextSurface surf, bool useScriptFace)
    {

        surf.ContentType = (useScriptFace) ? ContentType.SCRIPT : ContentType.TEXT_AND_IMAGE;
        // IF we are using scripts and still using script face
        if (useScriptFace)
        {
            surf.Script = "None"; // Set script to none
            //surf.ClearImagesFromSelection(); // Remove all images
            surf.ScriptBackgroundColor = surf.ScriptBackgroundColor;
        }
        surf.Alignment = TextAlignment.LEFT; // Set left Alignments
    }

    /** Prints the stringbuilder to the scripts surface */
    static public void printScript(this IMyTextSurface surf, StringBuilder build)
    {
        surf.printScript(build, new Vector2(0)); // Use 0 offset
    }
    /** Prints the stringbuilder to the surface relative to offset */
    static public void printScript(this IMyTextSurface surf, StringBuilder build, Vector2 offset)
    {
        MySpriteDrawFrame frame = surf.DrawFrame();
        build.insertNewLine(surf); // Append in new lines relative to surface
        MySprite sprite = MySprite.CreateText(build.ToString(), surf.Font, surf.ScriptForegroundColor, surf.FontSize);
        sprite.Alignment = TextAlignment.LEFT;
        sprite.RotationOrScale = 0.75f;
        var screenCorner = (surf.TextureSize - surf.SurfaceSize) * 0.5f;
        sprite.Position = screenCorner + new Vector2(16) + offset;
        frame.Add(sprite);

    }




    /** Inserts new lines into stringbuilders relative to surface */
    static public void insertNewLine(this StringBuilder build, IMyTextSurface surf)
    {
        int pixels = (int)surf.MeasureStringInPixels(build, surf.Font, surf.FontSize).X;
        // Out of bounds X
        if (pixels > surf.SurfaceSize.X)
        {
            int cycles = (int)pixels / (int)surf.SurfaceSize.X;
            int cutoff = surf.newLinePos(build);
            for (int i = 0; i < cycles; i++)
            {
                int step = (cutoff * (i + 1));
                int doot = (step > build.Length) ? build.Length : step; // Dont go over, use either step or length
                if (build.ToString().Substring(cutoff * i, cutoff).Contains("\n")) { continue; } // Dont insert new lines if they already exist in the frame

                // Use either step or length
                build.Insert(doot, "\n", 1);
            }
        }
    }
    /** calculates the corner of the text surface */
    static public Vector2 getCorner(this IMyTextSurface space)
    {
        return (space.TextureSize - space.SurfaceSize) * 0.5f;
    }
    /** Measures a string in pixels by avoiding unescessary allocation and storage of a string builder */
    static public Vector2 MeasureStringInPixels(this IMyTextSurface surf, String build, String font, float scale)
    {
        return surf.MeasureStringInPixels(new StringBuilder(build), font, scale);
    }
    /** Grabs the edges of a bounding box as more bounding box edges */
    static public Line[] getEdges(this BoundingBox2 box)
    {
        Vector2[] corners = box.GetCorners();
        // Starting from the top right to top left, moving counter clockwise.
        return new Line[]
        {
            new Line(corners[0], corners[1]),
            new Line(corners[1], corners[2]),
            new Line(corners[2], corners[3]),
            new Line(corners[3], corners[0])
        };
    }
    /** extends bounding box containment check to include the line type */
    static public ContainmentType Contains(this BoundingBox2 box, Line check)
    {
        // If both our points are contained
        if (box.Contains(check.P1()) == ContainmentType.Contains && box.Contains(check.P2()) == ContainmentType.Contains)
        {
            return ContainmentType.Contains;
        }
        // If both our points are disjoin
        else if (box.Contains(check.P1()) == ContainmentType.Disjoint && box.Contains(check.P2()) == ContainmentType.Disjoint)
        {
            return ContainmentType.Disjoint;
        }
        // We are in between. Intersection
        else
        {
            return ContainmentType.Intersects;
        }

    }
    /** Computes the diagonal distance of a bounding box 2*/
    static public float Diagonal(this BoundingBox2 box)
    {
        return Vector2.Distance(box.Min, box.Max); // Compute vector2 2 float distance
    }
}




/** This class is responsible for holding 2 vector2's **/
public class Line
{
    private Vector2 p1; // Point 1
    private Vector2 p2; // Point 2
    /** constructor with two points */
    public Line(Vector2 one, Vector2 two)
    {
        p1 = one;
        p2 = two;
    }
    /** Constructor using only 1 point */
    public Line(Vector2 p) : this(p, p) { }
    // Default constructor
    public Line() { }
    // Setter & getter for p1 and p2
    public Vector2 P1() => p1;
    public void P1(Vector2 use) => p1 = use;
    public Vector2 P2() => p2;
    public void P2(Vector2 use) => p2 = use;
    // grabs distance
    public float distance()
    {
        return Vector2.Distance(p1, p2);
    }
    /** Compares the distance to another passed line */
    public int compareDistance(Line check) => compareDistance(check.distance());
    /** Compare the distance via float */
    public int compareDistance(float check)
    {
        // If our distance > checks distance
        if (distance() > check)
        {
            return 1;
        }
        // If our distance < checks distance
        if (distance() < check)
        {
            return -1;
        }
        // else our distance = checks distance
        return 0;
    }