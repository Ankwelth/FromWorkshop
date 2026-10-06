// Configuration variables
string airlockGroupName = "AirlockSystem";  // Partial match group name for airlocks
double airlockVentThresholdPercentage = 0.05;  // 5% vent threshold before enabling outer doors
double airlockDoorTimeout = 0.4;  // Timeout for automatically closing doors in seconds

// Class to represent a single airlock system
class AirlockSystem {
    public string name;
    public List<IMyDoor> innerDoors = new List<IMyDoor>();
    public List<IMyDoor> outerDoors = new List<IMyDoor>();
    public List<IMyAirVent> airVents = new List<IMyAirVent>();
    public DateTime doorCloseTime = DateTime.MinValue;
    public bool airlockCycleActive = false;

    public AirlockSystem(string name) {
        this.name = name;
    }
}

// List of all airlock systems
List<AirlockSystem> airlockSystems = new List<AirlockSystem>();

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10; // Updates every 10 ticks
    InitializeAirlockSystems();
}

// Initialize all airlock systems based on group name partial matches
void InitializeAirlockSystems() {
    var groups = new List<IMyBlockGroup>();
    GridTerminalSystem.GetBlockGroups(groups);

    foreach (var group in groups) {
        if (group.Name.Contains(airlockGroupName)) {
            var airlock = new AirlockSystem(group.Name);
            var blocks = new List<IMyTerminalBlock>();
            group.GetBlocks(blocks);

            foreach (var block in blocks) {
                var door = block as IMyDoor;
                var vent = block as IMyAirVent;

                if (door != null) {
                    if (block.CustomName.Contains("Inner")) {
                        airlock.innerDoors.Add(door);
                    } else if (block.CustomName.Contains("Outer")) {
                        airlock.outerDoors.Add(door);
                    }
                } else if (vent != null) {
                    airlock.airVents.Add(vent);
                }
            }
            airlockSystems.Add(airlock);
        }
    }
}

public void Save() { }

public void Main(string argument, UpdateType updateSource) {
    foreach (var airlock in airlockSystems) {
        if (airlock.airlockCycleActive) {
            ManageAirlockCycle(airlock);
        } else {
            CheckForOpenedDoors(airlock);
        }
    }
}

// Check if any doors (inner or outer) have been opened in a specific airlock system
void CheckForOpenedDoors(AirlockSystem airlock) {
    foreach (var innerDoor in airlock.innerDoors) {
        if (innerDoor.Status == DoorStatus.Open) {
            DisableDoors(airlock.outerDoors);
            airlock.doorCloseTime = DateTime.Now.AddSeconds(airlockDoorTimeout);
            airlock.airlockCycleActive = true;
            return;
        }
    }

    foreach (var outerDoor in airlock.outerDoors) {
        if (outerDoor.Status == DoorStatus.Open) {
            DisableDoors(airlock.innerDoors);
            airlock.doorCloseTime = DateTime.Now.AddSeconds(airlockDoorTimeout);
            airlock.airlockCycleActive = true;
            return;
        }
    }
}

// Manage the airlock cycle for a specific airlock system
void ManageAirlockCycle(AirlockSystem airlock) {
    if (DateTime.Now >= airlock.doorCloseTime) {
        CloseAllDoors(airlock);
        
        // Check if outer doors are closed to enable inner doors
        if (AreDoorsClosed(airlock.outerDoors)) {
            EnableDoors(airlock.innerDoors);  // Re-enable inner doors once outer doors are closed
            StartDepressurizationCycle(airlock);
        }
    }
}

// Close all doors in the specified airlock system
void CloseAllDoors(AirlockSystem airlock) {
    foreach (var door in airlock.innerDoors) {
        if (door.Status == DoorStatus.Open) {
            door.CloseDoor();
        }
    }
    foreach (var door in airlock.outerDoors) {
        if (door.Status == DoorStatus.Open) {
            door.CloseDoor();
        }
    }
}

// Check if all doors in a specific list are closed
bool AreDoorsClosed(List<IMyDoor> doors) {
    foreach (var door in doors) {
        if (door.Status != DoorStatus.Closed) {
            return false;
        }
    }
    return true;
}

// Start the depressurization cycle for a specific airlock system
void StartDepressurizationCycle(AirlockSystem airlock) {
    bool allVentsDepressurized = true;
    foreach (var vent in airlock.airVents) {
        if (vent.CanPressurize && vent.GetOxygenLevel() > airlockVentThresholdPercentage) {
            vent.Depressurize = true; // Depressurize if above threshold
            allVentsDepressurized = false;
        }
    }

    // Enable outer doors only if all vents are depressurized
    if (allVentsDepressurized) {
        EnableDoors(airlock.outerDoors);
        airlock.airlockCycleActive = false; // End the cycle
    }
}

// Disable doors in the specified list
void DisableDoors(List<IMyDoor> doors) {
    foreach (var door in doors) {
        door.Enabled = false; // Disable doors
    }
}

// Enable doors in the specified list
void EnableDoors(List<IMyDoor> doors) {
    foreach (var door in doors) {
        door.Enabled = true; // Enable doors
    }
}
