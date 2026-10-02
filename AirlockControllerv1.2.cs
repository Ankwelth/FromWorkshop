MyIni config = new MyIni();
IMyDoor innerDoor;
IMyDoor outerDoor;
IMyAirVent vent;
double maxDepressurizeTime;
double pressurizeThreshold;
public Program() {
    config.TryParse(Me.CustomData);
    innerDoor = GridTerminalSystem.GetBlockWithName(config.Get("airlock", "doorInner").ToString()) as IMyDoor;
    outerDoor = GridTerminalSystem.GetBlockWithName(config.Get("airlock", "doorOuter").ToString()) as IMyDoor;
    vent = GridTerminalSystem.GetBlockWithName(config.Get("airlock", "ventAirlock").ToString()) as IMyAirVent;
    maxDepressurizeTime = config.Get("airlock", "maxDepressurizeTime").ToDouble(10.0);
    pressurizeThreshold = Math.Min(1.0, config.Get("airlock", "pressurizeThreshold").ToDouble(0.99));
    Echo("Program compiled successfully");
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    Normalize();
    vent.Enabled = false;
}
string method = "normalize";
long depresTime = 0;
public void Main(string argument) {
    switch (argument) {
        case "pres": // request to pressurize airlock and open inner door
            method = "pressurize";
            Normalize();
            break;
        case "depres": // request to depressurize airlock and open outer door
            method = "depressurize";
            Normalize();
            break;
        case "normalize": // request to shut airlock doors
            method = "normalize";
            break;
        case "purge": // open both airlock doors
            method = "purging";
            break;
        default:
            Echo(method);
            switch (method) {
                case "normalize":
                    Normalize();
                    break;
                case "purging":
                    innerDoor.Enabled = true;
                    outerDoor.Enabled = true;
                    innerDoor.OpenDoor();
                    outerDoor.OpenDoor();
                    vent.Enabled = false;
                    vent.Depressurize = false;
                    if (innerDoor.Status == DoorStatus.Open && outerDoor.Status == DoorStatus.Open) {
                        method = "purge";
                    }
                    break;
                case "pressurize":
                    LockDoors();
                    if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Closed) {
                        vent.Depressurize = false;
                        vent.Enabled = true;
                        method = "pressurizing";
                    }
                    break;
                case "pressurizing":
                    if (vent.GetOxygenLevel() >= pressurizeThreshold) {
                        vent.Enabled = false;
                        innerDoor.Enabled = true;
                        innerDoor.OpenDoor();
                        method = "open";
                    }
                    break;
                case "depressurize":
                    LockDoors();
                    if (innerDoor.Status == DoorStatus.Closed && outerDoor.Status == DoorStatus.Closed) {
                        vent.Depressurize = true;
                        vent.Enabled = true;
                        method = "depressurizing";
                        depresTime = Runtime.LifetimeTicks;
                    }
                    break;
                case "depressurizing":
                    if (vent.GetOxygenLevel() < 0.01f || (maxDepressurizeTime >= 0 && Runtime.LifetimeTicks - depresTime >= maxDepressurizeTime * 60)) {
                        outerDoor.Enabled = true;
                        outerDoor.OpenDoor();
                        method = "open";
                    }
                    break;
            }
            break;
    }
}
private void LockDoors() {
    if (innerDoor.Status == DoorStatus.Closed) innerDoor.Enabled = false;
    else if (innerDoor.Status == DoorStatus.Open) {
        innerDoor.Enabled = true;
        innerDoor.CloseDoor();
    }
    if (outerDoor.Status == DoorStatus.Closed) outerDoor.Enabled = false;
    else if (outerDoor.Status == DoorStatus.Open) {
        outerDoor.Enabled = true;
        outerDoor.CloseDoor();
    }
}
private void Normalize() {
    LockDoors();
    vent.Enabled = false;
}