/*
 * V 1.1.1
 * 
 * Airlock groups are found by the pattern [Groupname] [airlockMarkerString] [interiorSuffix/exteriorSuffix]
 * for example:
 *   Cool Spacestation Airlock interior
 *   Cool Spacestation Airlock exterior
 * will result in the group Cool Spacestation with 2 doors
 *
 * every group needs atleast one interior and exterior door
 * 
 * Alternative: group the doors that should close automatic. The group is found with the pattern [Groupname] [managedGroupMarkerString]
 * for example:
 *   Close the damn Doors! AutoClosing
 * All Doors in that group close after [doorOpenTimeInSec] seconds when opend.
 * 
 * Airtight Hangar Doors and [Frosbite-DLC] Gates are supported, but closing and opening so slow that the default doorOpenTimeInSec has to be adjusted.
 */

private const string managedGroupMarkerString = "AutoClosing";
private const string airlockMarkerString = "airlock";
private const string interiorSuffix = "interior";
private const string exteriorSuffix = "exterior";

// the refreshrate of the Airlockgroups
// default 15sec
private const int setupRefreshInSec = 15;

// how long a door stays open
// default 5sec
private const int doorOpenTimeInSec = 5;

/*
 * Please do not change anything past this point
 */

private string internalManagedGroupMarkerString;
private string internalAirlockMarkerString;
private string internalInteriorSuffix;
private string internalExteriorSuffix;
private bool isSetup = false;
private bool isSettingUp = false;
private long setupTimeStamp;
private Dictionary<string, AirlockGroup> airlocks = new Dictionary<string, AirlockGroup>();
private Dictionary<string, ManagedGroup> managedDoors = new Dictionary<string, ManagedGroup>();

private TimeSpan setupRefresh = new TimeSpan(0, 0, 15);
private TimeSpan doorOpenTimeSpan = new TimeSpan(0, 0, 5);
private TimeSpan TimeSinceUpdate => new TimeSpan(DateTime.Now.Ticks - setupTimeStamp);

public Program()
{
  Runtime.UpdateFrequency = UpdateFrequency.Update10;
  internalManagedGroupMarkerString = string.IsNullOrWhiteSpace(managedGroupMarkerString) ? "autoclosing" : managedGroupMarkerString.ToLower();
  internalAirlockMarkerString = string.IsNullOrWhiteSpace(airlockMarkerString) ? "airlock" : airlockMarkerString.ToLower();
  internalInteriorSuffix = string.IsNullOrWhiteSpace(interiorSuffix) ? "interior" : interiorSuffix.ToLower();
  internalExteriorSuffix = string.IsNullOrWhiteSpace(exteriorSuffix) ? "exterior" : exteriorSuffix.ToLower();
  if (setupRefreshInSec > 0) setupRefresh = new TimeSpan(0, 0, setupRefreshInSec);
  if (doorOpenTimeInSec > 0) doorOpenTimeSpan = new TimeSpan(0, 0, doorOpenTimeInSec);
}

public void Main(string argument, UpdateType updateSource)
{
  if (isSettingUp)
    return;

  if (!this.isSetup || TimeSinceUpdate > this.setupRefresh)
  {
    isSettingUp = true;
    Setup();
    setupTimeStamp = DateTime.Now.Ticks;
    isSetup = true;
    isSettingUp = false;
    return;
  }

  UpdateDoors();
}

private void UpdateDoors()
{
  Echo($"time since update: {TimeSinceUpdate:g}");
  Echo(string.Empty);
  Echo("Airlocks:");
  foreach (var airlock in airlocks)
  {
    Echo($"{airlock.Key}: {airlock.Value.ToString()}");
    airlock.Value.Update();
  }

  Echo("Managed Groups:");
  foreach (var managedDoor in managedDoors)
  {
    Echo($"{managedDoor.Key}: {managedDoor.Value.ToString()}");
    managedDoor.Value.Update();
  }
}

private void Setup()
{
  var airlockIds = SetupAirlocks();
  SetupManagedGroups(airlockIds);
}

private List<long> SetupAirlocks()
{
  var doors = new List<IMyDoor>();
  GridTerminalSystem.GetBlocksOfType(
    doors,
    block => IsValidDoor(block) && block.CustomName.ToLower().Contains(internalAirlockMarkerString));

  var airlockIds = new List<long>();
  var oldDoors = new Dictionary<long, TimableDoor>();
  if (doors.Count == 0)
  {
    Echo("no Airlocks");
    return airlockIds;
  }

  foreach (var airlockGroup in airlocks.Values)
    foreach (var door in airlockGroup.AllDoors)
      oldDoors.Add(door.Id, door);

  airlocks.Clear();

  foreach (var door in doors)
  {
    var timableDoor = GetDoor(oldDoors, door);
    if (door.CustomName.ToLower().Contains(internalInteriorSuffix))
      AddAsInterior(timableDoor);
    else if (door.CustomName.ToLower().Contains(internalExteriorSuffix))
      AddAsExterior(timableDoor);
    else
      continue;

    airlockIds.Add(door.EntityId);
  }

  return airlockIds;
}

private void SetupManagedGroups(List<long> airlockIds)
{
  List<IMyDoor> doors = new List<IMyDoor>();
  Dictionary<long, TimableDoor> oldDoors = new Dictionary<long, TimableDoor>();
  var blockGroups = new List<IMyBlockGroup>();
  GridTerminalSystem.GetBlockGroups(blockGroups,
    blockGroup =>
    blockGroup.Name.ToLower().Contains(internalManagedGroupMarkerString));

  if (blockGroups.Count == 0)
  {
    Echo("no Groups");
    return;
  }

  oldDoors.Clear();
  foreach (var managedDoor in managedDoors.Values)
    foreach (var door in managedDoor.Doors)
      oldDoors.Add(door.Id, door);
  managedDoors.Clear();
  ManagedGroup managedGroup;
  foreach (var blockGroup in blockGroups)
  {
    blockGroup.GetBlocksOfType(
      doors,
      block => IsValidDoor(block) && !airlockIds.Contains(block.EntityId));
    if (doors.Count == 0) continue;

    managedGroup = new ManagedGroup();
    foreach (var door in doors)
    {
      var timableDoor = GetDoor(oldDoors, door);
      managedGroup.Doors.Add(timableDoor);
    }

    managedDoors.Add(blockGroup.Name.Replace(" ", string.Empty), managedGroup);
    doors.Clear();
  }
}

private bool IsValidDoor(IMyDoor block)
{
  return block.CubeGrid == Me.CubeGrid // only own grid
    &&
    block is IMyDoor; // only doors
}

private TimableDoor GetDoor(Dictionary<long, TimableDoor> oldDoors, IMyDoor door)
{
  TimableDoor timableDoor;
  if (oldDoors.ContainsKey(door.EntityId))
    timableDoor = oldDoors[door.EntityId];
  else
    timableDoor = new TimableDoor(door, doorOpenTimeSpan);
  return timableDoor;
}

private void AddAsInterior(TimableDoor door) => GetAirlockGroup(door.Name).AddAsInterior(door);

private void AddAsExterior(TimableDoor door) => GetAirlockGroup(door.Name).AddAsExterior(door);

private AirlockGroup GetAirlockGroup(string doorName)
{
  var name = doorName.ToLower().Replace(" ", string.Empty);
  var groupName = name.Substring(0, name.IndexOf(internalAirlockMarkerString));
  if (!airlocks.ContainsKey(groupName))
  {
    airlocks.Add(groupName, new AirlockGroup());
  }

  return airlocks[groupName];
}

public class TimableDoor
{
  private IMyDoor door;
  private bool oldIsOpen;
  private long opendTimestamp;
  private TimeSpan doorOpenTimeSpan = new TimeSpan(0, 0, 5);

  public TimableDoor(IMyDoor door, TimeSpan doorOpenTimeSpan)
  {
    this.door = door;
    this.doorOpenTimeSpan = doorOpenTimeSpan;
    oldIsOpen = IsCurrentlyOpen;
    if (IsCurrentlyOpen)
      opendTimestamp = DateTime.Now.Ticks;
  }

  public long Id => door.EntityId;
  public string Name => door.CustomName;
  public bool IsCurrentlyOpen => door.Status != DoorStatus.Closed;

  public void Update()
  {
    if (!door.IsWorking) return;

    if (!IsCurrentlyOpen)
    {
      oldIsOpen = false;
      return;
    }

    if (oldIsOpen)
    {
      // was open for longer than x sec
      if (doorOpenTimeSpan < new TimeSpan(DateTime.Now.Ticks - opendTimestamp))
        Close();
    }
    else
    {
      oldIsOpen = true;
      opendTimestamp = DateTime.Now.Ticks;
    }
  }

  public void Unlock()
  {
    if (!door.IsWorking) door.Enabled = true;
  }

  public void Close()
  {
    if (!door.IsWorking)
      door.Enabled = true;

    door.CloseDoor();
  }

  public void CloseOrLock()
  {
    if (!IsCurrentlyOpen)
    {
      door.Enabled = false;
      return;
    }

    Close();
  }
}

public class AirlockGroup
{
  private List<TimableDoor> interiorList = new List<TimableDoor>();
  private List<TimableDoor> exteriorList = new List<TimableDoor>();

  public List<TimableDoor> AllDoors
  {
    get
    {
      var list = new List<TimableDoor>();
      list.AddRange(interiorList);
      list.AddRange(exteriorList);
      return list;
    }
  }

  public void AddAsInterior(TimableDoor door) => interiorList.Add(door);

  public void AddAsExterior(TimableDoor door) => exteriorList.Add(door);

  public void Update()
  {
    if (interiorList.Count == 0 || exteriorList.Count == 0)
      return;

    var interiorOpen = false;
    var exteriorOpen = false;

    foreach (var door in interiorList)
    {
      door.Update();
      if (!door.IsCurrentlyOpen) continue;

      interiorOpen = true;
    }

    foreach (var door in exteriorList)
    {
      door.Update();
      if (!door.IsCurrentlyOpen) continue;

      exteriorOpen = true;
    }

    if (interiorOpen && exteriorOpen)
    {
      CloseAllDoors();
      return;
    }

    if (interiorOpen)
      CloseOrLock(exteriorList);
    else
      Unlock(exteriorList);

    if (exteriorOpen)
      CloseOrLock(interiorList);
    else
      Unlock(interiorList);
  }

  private void Unlock(List<TimableDoor> doorList) => doorList.ForEach(door => door.Unlock());

  private void CloseOrLock(List<TimableDoor> doorList) => doorList.ForEach(door => door.CloseOrLock());

  private void CloseAllDoors()
  {
    interiorList.ForEach(door => door.Close());
    exteriorList.ForEach(door => door.Close());
  }

  public override string ToString() => $"interior: {interiorList.Count} | exterior: {exteriorList.Count}";
}

public class ManagedGroup
{
  public List<TimableDoor> Doors { get; private set; } = new List<TimableDoor>();

  public void Update()
  {
    foreach (var door in Doors)
      door.Update();
  }

  public override string ToString() => $"Count: {Doors.Count}";
}