/* DonDepresor Cargo monitor

This script allows you to monitor a cargo container for a certain amount of inventory items.
When all of the listed items are stored in the container, a timer will be triggered.
When none of the listed items are stored in the container, another timer will be triggered.

Setup:
=====

- Setup the script filling the names of the Cargo Container and the Trigger Blocks.
- Edit this Programable Block's Custom Data to setup the required items and their amounts. See below for more help on this.
  Each line contains the item name, a colon, and the desired amount. Invalid lines will be ignored.

Running:
=======

- Set autostart = true to start the script automatically.
- Start the script manually by running with argument 'start'.
- Stop the script by running with argument 'stop'.
- Reload the Custom Data list by running with argument 'reload'. You can reload without restarting.
- Print a list of the current contents of the cargo container by running with argument 'list'
    (do it while the script is stopped, or the next update won't let you read anything).
- Manually check the cargo container by running with argument 'check' (this works even when the script is stopped).

Tips:
====

To know the names of your required items, just add a few of them to the container and run the script with the 'list' argument.
The script will print a list with the names of the items and their current amount, like this:

Contents of Cargo Container:
- Construction: 100
- InteriorPlate: 100
- SteelPlate: 100
- SmallTube: 100
- Motor: 100

Now you can fill this Programable Block's Custom Data with your required items and amounts, like this:

Construction: 2500
InteriorPlate: 200
SteelPlate: 5000
SmallTube: 650
Motor: 200

Test your setup with autostart = false. Run 'list', 'reload' and 'check' until you are satisfied with
the results. Then set autostart = true.

*/
public Program() {

    // Configure your script here:

    // Whether to start automatically:
    bool autostart = false;

    // Cargo container to check:
    string cargoContainerName = "Cargo Container";

    // Timer to trigger when full:
    string fullTimerName = "Timer 1";

    // Timer to trigger when empty:
    string emptyTimerName = "Timer 2";

    // -- End of configuration --

    cargo = GridTerminalSystem.GetBlockWithName(cargoContainerName) as IMyCargoContainer;
    if(cargo==null) Echo("Error: Container not found with name "+ cargoContainerName);
    fullTimer = GridTerminalSystem.GetBlockWithName(fullTimerName) as IMyTimerBlock;
    if(fullTimer==null) Echo("Error: Timer not found with name "+ fullTimerName);
    emptyTimer = GridTerminalSystem.GetBlockWithName(emptyTimerName) as IMyTimerBlock;
    if(emptyTimer==null) Echo("Error: Timer not found with name "+ emptyTimerName);

    Runtime.UpdateFrequency = UpdateFrequency.None;
    if(autostart) {
        this.Main("start", UpdateType.Once);
    }
}

private Hashtable requiredItems = new Hashtable();
private IMyCargoContainer cargo;
private IMyTimerBlock fullTimer;
private IMyTimerBlock emptyTimer;
private bool wasFull = false;
private bool wasEmpty = false;

public void Main(string argument, UpdateType updateSource) {
    if(isReady()) {
        bool doStart = String.Equals("start", argument, StringComparison.OrdinalIgnoreCase);
        bool doReload = String.Equals("reload", argument, StringComparison.OrdinalIgnoreCase);
        bool doStop = String.Equals("stop", argument, StringComparison.OrdinalIgnoreCase);
        bool doList = String.Equals("list", argument, StringComparison.OrdinalIgnoreCase);
        bool doForceCheck = String.Equals("check", argument, StringComparison.OrdinalIgnoreCase);
        if(doStop) {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            Echo("Stopped");
        }
        if(doStart || doReload) {
            populateList();
        }
        if(doStart) {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            Echo("Started");
        }
        if(doList) {
            list();
        }
        checkList(doForceCheck);
    } else {
        Echo("Error: script is not configured yet");
    }
}

private bool isReady() {
    return cargo!=null && fullTimer!=null && emptyTimer!=null;
}

private void populateList() {
    string data = Me.CustomData.Trim();
    requiredItems.Clear();
    int lines = 0, read = 0, errors = 0;
    if(data.Length>0) {
        foreach(string line in data.Split('\n')) {
            string[] parts = line.Split(':');
            if(parts.Length > 1) {
                try {
                    requiredItems.Add(parts[0].Trim(), int.Parse(parts[1].Trim()));
                    read++;
                } catch(Exception e) {
                    errors++;
                }
            }
            lines++;
        }
    }
    Echo("List loaded. Text lines: "+ lines +", items read: "+ read +", errors: "+ errors);
}

private void checkList(bool force = false) {
    if(requiredItems.Count>0) {
        Hashtable missingItems = (Hashtable)requiredItems.Clone();
        List<MyInventoryItem> items = new List<MyInventoryItem>();
        try {
            cargo.GetInventory().GetItems(items);
        } catch(Exception e) {
            Echo("Error loading inventory from container: "+ e.Message);
        }
        foreach(MyInventoryItem item in items) {
            string name = item.Type.SubtypeId;
            int amount = (int)(item.Amount.RawValue / 1000000);
            if(missingItems.ContainsKey(name)) {
                int missing = (int)(missingItems[name]);
                missingItems[name] = missing - amount;
            }
        }
        bool isFull = true, isEmpty = true;
        foreach(string key in requiredItems.Keys) {
            int amountRequired = (int)(requiredItems[key]);
            int amountMissing = (int)(missingItems[key]);
            isFull = isFull && amountMissing <= 0;
            isEmpty = isEmpty && amountMissing >= amountRequired;
        }
        if(isFull) {
            if(!wasFull || force) {
                Echo("Container now has all of the required items");
                try {
                    fullTimer.Trigger();
                } catch(Exception e) {
                    Echo("Error triggering full Timer: "+ e.Message);
                }
            } else {
                Echo("Container still has all of the required items");
            }
        } else if(isEmpty) {
            if(!wasEmpty || force) {
                Echo("Container now has none of the required items");
                try {
                    emptyTimer.Trigger();
                } catch(Exception e) {
                    Echo("Error triggering empty Timer: "+ e.Message);
                }
            } else {
                Echo("Container still has none of the required items");
            }
        } else {
            Echo("Container has some of the required items");
        }
        wasFull = isFull;
        wasEmpty = isEmpty;
    }
}

private void list() {
    Echo("Contents of "+ cargo.CustomName +":");
    List<MyInventoryItem> items = new List<MyInventoryItem>();
    try {
        cargo.GetInventory().GetItems(items);
    } catch(Exception e) {
        Echo("Error loading inventory from Container: "+ e.Message);
    }
    foreach(MyInventoryItem item in items) {
        Echo("-"+ item.Type.SubtypeId +": "+ (item.Amount.RawValue / 1000000));
    }
}