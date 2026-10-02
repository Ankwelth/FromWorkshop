/*
 * 
 * R e a d m e
 * -----------
 * 
 * Version 1.0.1
 * 
 * Commands
 * updateconfig -- re-reads the Custom Data (or just recompile the script; which is often easier)
 * dock -- if the DockingPort value is set and within connector range, will dock and perform actions based on the [docking] config
 * undock -- if the DockingPort value is set, will undock and perform actions based on the [docking] config
 * 
 * ---
 * Configuration
 * 
 * Systems Manager uses the Custom Data of the block to manage settings. Below is the default configuration
 * 
 * I've included a comment to describe what each does
 * 
 * [hydrogen]
 * ManageHydrogenEngines=false ; only run hydrogen engines if needed, such as batteries low or maxing out power
 * ManageH2Generators=true ; will attempt to only run O2/H2 generators if tanks are low or you are directly running thrusters from the O2/H2 generator
 * HydrogenFillCutoff=0.9 ; if h2 tanks are at least this full, stop the o2/h2 generators
 * OxygenFillCutoff=0.2 ; similar for o2
 * MonitorVents=false ; whether vents should be used to detect low pressure. this is not always reliable due to some block types so it defaults to false
 * 
 * [docking]
 * ManageBatteries=false ; Set batteries to Recharge when docking and Auto when undocking
 * ManageDampers=true ; Enable/disable ship's inertial dampeners when docking and undocking
 * ManageTanks=false; If true, set tanks to Stockpile when docking
 * DockingPort= ; Connector to use for docking, if empty, no docking management
 * EnableOnDock= ; Comma separated list of groups to enable when docking (only on the local ship)
 * DisableOnDock= ; Comma separated list of groups to disable when docking (only on the local ship)
 * EnableOnUndock= ; Comma separated list of groups to enable when undocking (only on the local ship)
 * DisableOnUndock= ; Comma separated list of groups to disable when undocking (only on the local ship)
 * 
 * [airlocks] ; each line should be group_name=activation (so myairlock=0.6 will use the doors in myairlock group and automatically close them after 0.6 seconds)
 * autodetect=0.5 ; by default will attempt to detet and manage airlocks. Any group with "Airlock" in the name will be detected. delete this line if you do not want automatic airlock detection
 * 
 * [mirrors] ; block_name=group_name -- mirrors the power state of block_name to the blocks in group_name
 */

    public struct SysMgrConfig
    {
      //hydrogen
      public bool manageEngines;
      public bool manageH2Generators;
      public double h2Cutoff;
      public double o2Cutoff;
      public bool monitorVents;

      //docking
      public bool manageBatteries;
      public bool manageDampers;
      public bool manageTanks;
      public String DefaultDockingPort;
      public String[] EnableOnUndock;
      public String[] DisableOnUndock;
      public String[] EnableOnDock;
      public String[] DisableOnDock;

      //misc
      public String StatusLCD;
      public int StatusLCDPanel;

      //airlocks
      //public List<MyIniKey> airlockGroups;
      public Dictionary<MyIniKey, Double> airlockGroups;

      //mirror
      public Dictionary<MyIniKey, string> mirrors;

      //public SysMgrConfig(List<MyIniKey> airlockGroups) : this()
      public SysMgrConfig(Dictionary<MyIniKey, Double> airlockGroups, Dictionary<MyIniKey, string> mirrorGroups) : this()
      {
this.airlockGroups = airlockGroups;
this.mirrors = mirrorGroups;
      }

      public UpdateType updateSource;
    }

    String spinner = "*--";
    MyIni _ini = new MyIni();
    SysMgrConfig cfg = new SysMgrConfig();
    HydrogenManager hydrogenMgr;
    DockingManager dockingMgr;
    AirlockManager airlockMgr;
    MirrorManager mirrorMgr;

    void SetDefaultConfigMap()
    {
      String config = Me.CustomData;
      if (config == "")
      {
_ini.Set("hydrogen", "ManageHydrogenEngines", false);
_ini.Set("hydrogen", "ManageH2Generators", true);
_ini.Set("hydrogen", "HydrogenFillCutoff", 0.9);
_ini.Set("hydrogen", "OxygenFillCutoff", 0.2);
_ini.Set("hydrogen", "MonitorVents", false);

_ini.Set("docking", "ManageBatteries", false);
_ini.Set("docking", "ManageDampers", true);
_ini.Set("docking", "ManageTanks", false);
_ini.Set("docking", "DockingPort", "");
_ini.Set("docking", "EnableOnDock", "");
_ini.Set("docking", "DisableOnDock", "");
_ini.Set("docking", "EnableOnUndock", "");
_ini.Set("docking", "DisableOnUndock", "");

_ini.Set("misc", "StatusLCD", "");
_ini.Set("misc", "StatusLCDPanel", 0);

_ini.AddSection("airlocks");
_ini.Set("airlocks", "autodetect", 0.5);
_ini.AddSection("mirrors");
Me.CustomData = _ini.ToString();
      }
    }

    void UpdateConfigMap()
    {
      SetDefaultConfigMap();
      MyIniParseResult result;
      if (!_ini.TryParse(Me.CustomData, out result))
throw new Exception(result.ToString());

      if(!_ini.ContainsSection("hydrogen")) { _ini.AddSection("hydrogen");  }

      cfg.manageEngines = _ini.Get("hydrogen", "ManageHydrogenEngines").ToBoolean();
      cfg.manageH2Generators = _ini.Get("hydrogen", "ManageH2Generators").ToBoolean();
      cfg.h2Cutoff = _ini.Get("hydrogen", "HydrogenFillCutoff").ToDouble();
      cfg.o2Cutoff = _ini.Get("hydrogen", "OxygenFillCutoff").ToDouble();
      cfg.monitorVents = _ini.Get("hydrogen", "MonitorVents").ToBoolean();

      if (!_ini.ContainsSection("docking")) { _ini.AddSection("docking"); }
      cfg.manageBatteries = _ini.Get("docking", "ManageBatteries").ToBoolean();
      cfg.manageDampers = _ini.Get("docking", "ManageDampers").ToBoolean();
      cfg.manageTanks = _ini.Get("docking", "ManageTanks").ToBoolean();
      cfg.DefaultDockingPort = _ini.Get("docking", "DockingPort").ToString();
      if(cfg.DefaultDockingPort == "" ) { cfg.DefaultDockingPort = _ini.Get("docking", "DefaultDockingPort").ToString(); }
      cfg.EnableOnDock = _ini.Get("docking", "EnableOnDock").ToString().Split(',');
      cfg.DisableOnDock = _ini.Get("docking", "DisableOnDock").ToString().Split(',');
      cfg.EnableOnUndock = _ini.Get("docking", "EnableOnUndock").ToString().Split(',');
      cfg.DisableOnUndock = _ini.Get("docking", "DisableOnUndock").ToString().Split(',');

      if (!_ini.ContainsSection("misc")) { _ini.AddSection("misc"); }
      cfg.StatusLCD = _ini.Get("misc", "StatusLCD").ToString();
      cfg.StatusLCDPanel = _ini.Get("misc", "StatusLCDPanel").ToInt32();

      if (!_ini.ContainsSection("airlocks")) { _ini.AddSection("airlocks"); }
      cfg.airlockGroups = new Dictionary<MyIniKey, Double>();// new List<MyIniKey>();

      List<MyIniKey> airlocks = new List<MyIniKey>();
      _ini.GetKeys("airlocks", airlocks);
      foreach(MyIniKey airlock in airlocks) {
double openTime = _ini.Get("airlocks", airlock.Name).ToDouble(5.0);
cfg.airlockGroups[airlock] = openTime;
      }

      if (!_ini.ContainsSection("mirrors")) { _ini.AddSection("mirrors"); }
      cfg.mirrors = new Dictionary<MyIniKey, string>();// new List<MyIniKey>();

      List<MyIniKey> mirrors = new List<MyIniKey>();
      _ini.GetKeys("mirrors", mirrors);
      foreach (MyIniKey mirror in mirrors)
      {
string mirrorGroup = _ini.Get("mirrors", mirror.Name).ToString();
cfg.mirrors[mirror] = mirrorGroup;
      }
    }

    void SetDockCfg(String defaultPort)
    {
      MyIniParseResult result;
      if (!_ini.TryParse(Me.CustomData, out result))
throw new Exception(result.ToString());
      cfg.DefaultDockingPort = defaultPort;
      _ini.Set("docking", "DockingPort", defaultPort);
      Me.CustomData = _ini.ToString();
    }

    IMyTextSurface GetSurface(String surfaceName, int index = 0)
    {
      List<IMyTerminalBlock> matchingBlocks = new List<IMyTerminalBlock>();
      GridTerminalSystem.SearchBlocksOfName(surfaceName, matchingBlocks);
      IMyTextSurface textMessagePanel = null;

      foreach (IMyTerminalBlock block in matchingBlocks)
      {
if (block is IMyTextPanel)
  textMessagePanel = block as IMyTextSurface;
else if (block is IMyTextSurfaceProvider)
{
  index = (block as IMyTextSurfaceProvider).SurfaceCount > index ? index : 0;

  textMessagePanel = (block as IMyTextSurfaceProvider).GetSurface(index);
}
      }

      return textMessagePanel;
    }

    String GetStatus()
    {
      //TODO: Something that doesn't suck
      String state = "<Docking Manager>\n" + "  " + dockingMgr.ToString().Replace("\n", "\n  ") +
             "\n<Hydrogen Manager>\n" + "  " + hydrogenMgr.ToString().Replace("\n", "\n  ") +
             "\n<Airlock Manager>\n" + "  " + airlockMgr.ToString().Replace("\n","\n  ") +
             "\n<Mirror Manager>\n" + "  " + mirrorMgr.ToString().Replace("\n", "\n  ");

      return state;
    }

    void LCDStatus(String lcdName)
    {
      String status = GetStatus();
      IMyTextSurface surface = GetSurface(lcdName, cfg.StatusLCDPanel);
      if (surface == null)
return;

      surface.WriteText(status,false);
    }

    void ManageSystems(UpdateType updateSource)
    {
      String state;
      switch(spinner)
      {
case "*--":
  spinner = "-*-";
  break;
case "-*-":
  spinner = "--*";
  break;
default:
  spinner = "*--";
  break;
      }
      state = spinner + "\n";

      cfg.updateSource = updateSource;
      dockingMgr.Manage(cfg);
      hydrogenMgr.Manage(cfg);
      airlockMgr.Manage(cfg);
      mirrorMgr.Manage(cfg);

      state += GetStatus();

      Echo(state);
    }

    public Program()
    {
      UpdateConfigMap();
      hydrogenMgr = new HydrogenManager(this);
      dockingMgr = new DockingManager(this);
      airlockMgr = new AirlockManager(this);
      mirrorMgr = new MirrorManager(this);
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }

    public void Save()
    {
    }

    public void Main(string argument, UpdateType updateSource)
    {
      string arg = argument.ToUpper();
      if (arg == "")
      {
ManageSystems(updateSource);
      }
      else if (arg == "UPDATECONFIG")
      {
UpdateConfigMap();
      }
      else if (arg == "DOCK" && cfg.DefaultDockingPort != "")
      {
dockingMgr.Dock();
      }
      else if (arg == "UNDOCK" && cfg.DefaultDockingPort != "")
      {
dockingMgr.Undock();
      }
      else
      {
String[] cmdAndArgs = argument.Split('=');
if(cmdAndArgs.Count() > 1)
{
  switch(cmdAndArgs[0].ToUpper())
  {
    case "SET_DOCK":
      SetDockCfg(cmdAndArgs[1]);
      break;
    case "CLOSELOCK":
      airlockMgr.CloseLock(cmdAndArgs[1]);
      break;
  }
}
      }

      if(cfg.StatusLCD != "")
LCDStatus(cfg.StatusLCD);
    }

    public class AirlockManager : Manager
    {
      const int openSeconds = 5;
      Dictionary<String, double> doorStatus;
      public AirlockManager(Program program) : base(program)
      {
doorStatus = new Dictionary<string, double>();
      }

      protected bool ManageDoorGroup(String airlockGroupName, double doorOpenTime, UpdateType updateSource)
      {
bool state = false;
try
{
  int ticks;
  switch (updateSource)
  {
    case UpdateType.Update1:
      ticks = 1;
      break;
    case UpdateType.Update10:
      ticks = 10;
      break;
    case UpdateType.Update100:
      ticks = 100;
      break;
    default:
      ticks = 1;
      break;
  }
  List<IMyLightingBlock> doorStatusLights = new List<IMyLightingBlock>();
  List<IMyDoor> doors = new List<IMyDoor>(8);
  List<IMyDoor> closedDoors = new List<IMyDoor>(4);
  List<IMyDoor> openDoors = new List<IMyDoor>(4);

  IMyBlockGroup doorGroup = _program.GridTerminalSystem.GetBlockGroupWithName(airlockGroupName);
  doorGroup.GetBlocksOfType<IMyDoor>(doors, block => block.CubeGrid == _program.Me.CubeGrid);
  doorGroup.GetBlocksOfType<IMyLightingBlock>(doorStatusLights, block => block.CubeGrid == _program.Me.CubeGrid);
  closedDoors.Clear();
  openDoors.Clear();
  foreach (IMyDoor door in doors)
  {
    if (door.OpenRatio > 0)
      openDoors.Add(door);
    else
      closedDoors.Add(door);
  }

  if (!doorStatus.ContainsKey(airlockGroupName))
  {
    doorStatus.Add(airlockGroupName, 0);
  }
  // If more than 1 door is open; close all open doors
  // If only 1 door is open, disable the other doors
  // If no doors are open, enable all doors
  if (openDoors.Count > 1)
  {
    foreach (IMyDoor door in openDoors)
    {
      door.Enabled = true;
      door.CloseDoor();
    }
    doorStatus[airlockGroupName] = -1;
  }
  else if (openDoors.Count == 1)
  {
    state = true;
    foreach (IMyDoor door in closedDoors)
      door.Enabled = false;
    if (doorStatus[airlockGroupName] > 0)
    {
      //countdown

      doorStatus[airlockGroupName] = Math.Max(0, doorStatus[airlockGroupName] - ticks);
    }
    else
    {
      doorStatus[airlockGroupName] = doorOpenTime * 60;
    }
  }
  else
  {
    foreach (IMyDoor door in doors)
      door.Enabled = true;
    doorStatus[airlockGroupName] = -1;
  }

  if (doorStatus[airlockGroupName] == 0)
  {
    foreach (IMyDoor door in doors)
    {
      door.CloseDoor();
    }
    doorStatus[airlockGroupName] = -1;
  }

  if (doorStatusLights.Count > 0)
  {
    Color statusColor;
    if (state)
      statusColor = new Color(255, 0, 0, 1);
    else
      statusColor = new Color(0, 255, 0, 1);
    foreach (IMyLightingBlock light in doorStatusLights)
      light.Color = statusColor;
  }
}
catch(Exception exp)
{
  //_status.Add(exp.ToString());
}
return state;
      }

      public void CloseLock(String airlockGroupName)
      {
List<IMyDoor> doors = new List<IMyDoor>(8);
IMyBlockGroup doorGroup = _program.GridTerminalSystem.GetBlockGroupWithName(airlockGroupName);
doorGroup.GetBlocksOfType<IMyDoor>(doors, block => block.CubeGrid == _program.Me.CubeGrid);
foreach (IMyDoor door in doors)
{
  door.Enabled = true;
  door.CloseDoor();
}
      }

      protected void DetectAirlocks(SysMgrConfig cfg, double doorOpenTime)
      {
List<IMyBlockGroup> groupList = new List<IMyBlockGroup>();
_program.GridTerminalSystem.GetBlockGroups(groupList, group => group.Name.Contains("Airlock"));
foreach (IMyBlockGroup group in groupList) {
  MyIniKey key = new MyIniKey("airlocks", group.Name);
  IMyBlockGroup doorGroup = _program.GridTerminalSystem.GetBlockGroupWithName(group.Name);
  List<IMyDoor> doors = new List<IMyDoor>(8);
  doorGroup.GetBlocksOfType<IMyDoor>(doors, block => block.CubeGrid == _program.Me.CubeGrid);
  if (!cfg.airlockGroups.ContainsKey(key) && doors.Count > 0)
  {
    cfg.airlockGroups[new MyIniKey("airlocks", group.Name + " auto")] = doorOpenTime;
  }
}
      }

      new public void Manage(SysMgrConfig cfg)
      {
_status.Clear();
MyIniKey autoKey = new MyIniKey("airlocks","autodetect");
if(cfg.airlockGroups.ContainsKey(autoKey))
{
  try
  {
    double doorOpenTime = cfg.airlockGroups[autoKey];
    DetectAirlocks(cfg, doorOpenTime);
  } catch(Exception exp)
  { _status.Add(exp.ToString()); }
}

foreach (MyIniKey key in cfg.airlockGroups.Keys)
{
  if (key.Name == "autodetect")
    continue;
  try
  {
    String name = key.Name;
    if(name.Contains(" auto"))
    {
      name = name.Replace(" auto", "");
    }

    double doorOpenTime = cfg.airlockGroups[key];
    if (ManageDoorGroup(name, doorOpenTime, cfg.updateSource))
      _status.Add(key.Name + " Open (" + doorStatus[key.Name] + ")");
    else
      _status.Add(key.Name + " Sealed (" + doorOpenTime + ")");
  }
  catch (Exception exp)
  {
    _status.Add(exp.ToString());
  }
}
      }
    }

    public class DockingManager : Manager
    {
      enum DockingState
      {
Unknown,
None,
Docking,
Docked,
Undocking,
Undocked
      }
      DockingState _state;

      public DockingManager(Program program) : base(program)
      {
_state = DockingState.Unknown;
      }

      IMyShipConnector GetPort(String portName)
      {
IMyShipConnector dock = null;
List<IMyShipConnector> connectors = new List<IMyShipConnector>();
_program.GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors, connector => connector.CubeGrid == _program.Me.CubeGrid && connector.CustomName == portName);
if(connectors.Count() > 0)
  dock = connectors[0];
return dock;
      }

      void SetDampers(bool state)
      {
List<IMyTerminalBlock> cockpits = new List<IMyTerminalBlock>();
_program.GridTerminalSystem.GetBlocksOfType<IMyShipController>(cockpits, IsMyGrid);
for (int index = 0; index < cockpits.Count; index++)
  (cockpits[index] as IMyShipController).DampenersOverride = state;
      }

      void SetStockpile(bool state)
      {
List<IMyGasTank> tanks = new List<IMyGasTank>();
_program.GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, block => block.IsSameConstructAs(_program.Me));
foreach(var tank in tanks)
{
  tank.Stockpile = state;
}
      }
      void SetGroupPower(string groupName, bool state)
      {
List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
try
{
  _program.GridTerminalSystem.GetBlockGroupWithName(groupName).GetBlocks(blocks, IsMyGrid);
  for (int index = 0; index < blocks.Count; index++)
    blocks[index].SetValue("OnOff", state);
}
catch (System.Exception)
{ }
      }

      void SetBatteries(ChargeMode mode)
      {
List<IMyTerminalBlock> batteries = new List<IMyTerminalBlock>();
_program.GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batteries, IsMyGrid);
for (int index = 0; index < batteries.Count; index++)
  (batteries[index] as IMyBatteryBlock).ChargeMode = mode;
      }

      void DoPostDock(SysMgrConfig cfg)
      {
if(cfg.manageBatteries) { SetBatteries(ChargeMode.Recharge); }
if(cfg.manageDampers) { SetDampers(false); }
if (cfg.manageTanks) { SetStockpile(true); }

//enable systems
if (cfg.EnableOnDock != null)
  foreach (String managedSystem in cfg.EnableOnDock)
    SetGroupPower(managedSystem, true);
//disable systems
if (cfg.DisableOnDock != null)
  foreach (String managedSystem in cfg.DisableOnDock)
    SetGroupPower(managedSystem, false);
      }

      void DoConnect(IMyShipConnector connector)
      {
if (connector == null)
  return;

connector.Connect();
      }

      void DoPreUndock(SysMgrConfig cfg)
      {
IMyShipConnector dock = GetPort(cfg.DefaultDockingPort);
if (dock == null) {
  _errors.Add(cfg.DefaultDockingPort + " connector was not found. Undocking aborted.");
  return;
}
else if( dock.Status != MyShipConnectorStatus.Connected)
{
  _errors.Add(cfg.DefaultDockingPort + " connector not connected, undocking aborted.");
  return;
}


if (cfg.manageBatteries) { SetBatteries(ChargeMode.Auto); }

//enable systems
if (cfg.EnableOnUndock != null)
  foreach (String managedSystem in cfg.EnableOnUndock)
    SetGroupPower(managedSystem, true);
//disable systems
if (cfg.DisableOnUndock != null)
  foreach (String managedSystem in cfg.DisableOnUndock)
    SetGroupPower(managedSystem, false);
      }

      void DoDisconnect(IMyShipConnector connector, SysMgrConfig cfg)
      {
// TODO: Check if we're in space and moving, in which case don't enable dampeners
if (cfg.manageDampers) { SetDampers(true); }

if (cfg.manageTanks) { SetStockpile(false);  }

if (connector == null)
  return;

connector.Disconnect();
      }

      public void Dock()
      {
if(_state != DockingState.Docking || _state != DockingState.Docked)
  _state = DockingState.Docking;
      }

      public void Undock()
      {
if (_state != DockingState.Undocking || _state != DockingState.Undocked)
  _state = DockingState.Undocking;
      }

      new public void Manage(SysMgrConfig cfg)
      {
IMyShipConnector port = GetPort(cfg.DefaultDockingPort);
_status.Clear();
_errors.Clear();

if (cfg.DefaultDockingPort == "")
{
  _state = DockingState.None;
  _status.Add("Port: None");
}
else if (port == null)
{
  _status.Add("Port: " + cfg.DefaultDockingPort + " MISSING!");
  return;
}
else
  _status.Add("Port: " + cfg.DefaultDockingPort);

switch (_state)
{
  case DockingState.Unknown:
    _state = DockingState.None;
    _status.Add("Unknown");
    break;
  case DockingState.None:
    _status.Add("Idle");
    break;
  case DockingState.Docking:
    _status.Add("Docking");
    if (port.Status == MyShipConnectorStatus.Connectable)
      DoConnect(port);
    else if (port.Status == MyShipConnectorStatus.Connected)
      _state = DockingState.Docked;
    else
      _state = DockingState.None;
    //TODO: Add configuration to enable "sticky" connection request?
    // (similar to autolock on landing gear)
    break;
  case DockingState.Docked:
    _status.Add("Docked");
    DoPostDock(cfg);
    _state = DockingState.None;
    break;
  case DockingState.Undocking:
    _status.Add("Preparing to undock");
    DoPreUndock(cfg);
    _state = DockingState.Undocked;
    break;
  case DockingState.Undocked:
    _status.Add("Undocking");
    DoDisconnect(port, cfg);
    _state = DockingState.None;
    break;
}
      }
    }

    public class HydrogenManager : Manager
    {
      enum TankType
      {
Unknown,
Hydrogen,
Oxygen
      }

      protected struct PowerState
      {
public float totalUsed;
public float totalPossible;
public float engineUsed;
public float enginePossible;

public float batteryCharge;
public float batteryCap;
public int workingBatteries;
      }

      protected bool _hNeedsFilling;
      protected bool _o2NeedsFilling;
      protected bool _enginesNeedFuel;
      protected bool _hasHydrogenTanks;
      protected bool _hasOxygenTanks;
      protected double _powerThreshold;
      protected PowerState _powerState;
      protected bool _wantPower;
      protected bool _coolingDown;
      protected int _coolDownTicks;

      public HydrogenManager(Program program) : base(program)
      {
_enginesNeedFuel = false;
_hasHydrogenTanks = false;
_hasOxygenTanks = false;
_wantPower = false;
_powerThreshold = 0.6;
_coolingDown = false;
_coolDownTicks = 0;
      }

      TankType GetTankType(IMyGasTank tank)
      {
// This won't work if there are other types of tanks for mods
return tank.BlockDefinition.SubtypeId.Contains("Hydro") ? TankType.Hydrogen : TankType.Oxygen;
      }

      List<IMyGasTank> TanksOfType(TankType type, bool gridOnly = true)
      {
List<IMyGasTank> tanks = new List<IMyGasTank>();

_program.GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks, block => GetTankType(block) == type && (!gridOnly || block.IsSameConstructAs(_program.Me)));
return tanks;
      }

      bool PressureLow(SysMgrConfig cfg)
      {
List<IMyCockpit> cockpits = new List<IMyCockpit>();
_program.GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits, IsMyGrid);
bool requiresFilling = false;

for (int i = 0; i < cockpits.Count && !requiresFilling; i++)
  requiresFilling = (cockpits[i].OxygenFilledRatio < 0.5 && cockpits[i].OxygenCapacity > 0.0 && cockpits[i].BlockDefinition.SubtypeId != "LargeBlockBed");

if (requiresFilling || !cfg.monitorVents)
  return requiresFilling;

List<IMyAirVent> vents = new List<IMyAirVent>();
_program.GridTerminalSystem.GetBlocksOfType<IMyAirVent>(vents, IsMyGrid);

for (int i = 0; i < vents.Count && !requiresFilling; i++)
  requiresFilling = vents[i].Enabled && vents[i].PressurizationEnabled && vents[i].CanPressurize && vents[i].Status == VentStatus.Pressurizing;

return requiresFilling;
      }

      bool TanksNeedFilling(TankType tankType, double ratio = 0.9)
      {
List<IMyGasTank> tanks = TanksOfType(tankType);
bool requiresFilling = false;
if (tankType == TankType.Hydrogen)
  _hasHydrogenTanks = tanks.Count > 0;
else if (tankType == TankType.Oxygen)
  _hasOxygenTanks = tanks.Count > 0;

for (int i = 0; i < tanks.Count && !requiresFilling; i++)
  requiresFilling = (tanks[i].FilledRatio <= ratio && tanks[i].Enabled);
return requiresFilling;
      }

      bool GeneratorHasIce(IMyGasGenerator generator)
      {
MyFixedPoint emptyMass = (MyFixedPoint)0.0;
return generator.GetInventory().CurrentMass > emptyMass;
      }

      PowerState GetPowerState()
      {
PowerState state = new PowerState();
List<IMyPowerProducer> powerSources = new List<IMyPowerProducer>();
_program.GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(powerSources, block => block.IsSameConstructAs(_program.Me));
for (int i = 0; i < powerSources.Count; i++)
{

  if (powerSources[i].BlockDefinition.SubtypeId.Contains("Hydro"))
  {
    state.engineUsed += powerSources[i].CurrentOutput;
    state.enginePossible += powerSources[i].MaxOutput;
  }

  if (powerSources[i].IsWorking)
  {
    state.totalUsed += powerSources[i].CurrentOutput;
    state.totalPossible += powerSources[i].MaxOutput;
    if(powerSources[i] is IMyBatteryBlock)
    {
      state.workingBatteries++;
      state.batteryCharge += (powerSources[i] as IMyBatteryBlock).CurrentStoredPower;
      state.batteryCap += (powerSources[i] as IMyBatteryBlock).MaxStoredPower;
    }
  }
}

return state;
      }

      double BatteryCharge()
      {
if (_powerState.workingBatteries == 0)
  return 1.0;

return _powerState.batteryCharge / _powerState.batteryCap;
      }

      /* Tries to enable/disable any hydrogen engines based on system load
       * - Missing any sort of feedback / understanding of how power usage changes
       * if the engines are disabled (e.g. solar panel req. output would drop if o2/h2 gen off, so wouldn't need engine)
       *
       * Fix will be to implement more of a power planning design, but there are cycles in that
       * so it isn't trivial
      */
      void AdjustEngines(SysMgrConfig cfg)
      {
_enginesNeedFuel = false;
if (!cfg.manageEngines)
  return;
double batteryCharge = BatteryCharge();

//bool needEngines = batteryCharge < 0.6 || _powerState.totalPossible < 0.01 || _powerState.totalPossible < _powerThreshold || ((_powerState.totalUsed / _powerState.totalPossible) > 0.9);
float fillLevel = 0;

/*Rules for enabling the engines
         * 1. Do batteries need to be charged
         * 2. Are we in some very low-power state
         * 3. Are we using more power than is available if we turn off the engines
         * 4. Would turning off the engines put our potential power below a fixed cutoff
         */
bool needEngines = batteryCharge < 0.6 ||
  _powerState.totalPossible < 0.01 ||
  _powerState.totalUsed >= (_powerState.totalPossible - _powerState.engineUsed) ||
  ((_powerState.totalUsed - _powerState.engineUsed)/ (_powerState.totalPossible)) > _powerThreshold ||
  _wantPower
  ;
_status.Add("Batteries: " + (batteryCharge * 100).ToString("n2") + "%");
if (needEngines)
{
  _coolDownTicks = 40;
} else
{
  if (_coolDownTicks > 0)
  {
    needEngines = true;
    _status.Add("Engine in cooldown: " + _coolDownTicks);
  }
  _coolDownTicks--;
}

List<IMyPowerProducer> engines = new List<IMyPowerProducer>();
_program.GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(engines, block => IsMyGrid(block) && block.BlockDefinition.SubtypeId.Contains("Hydro"));
for (int i = 0; i < engines.Count; i++)
{
  engines[i].Enabled = needEngines;
  try
  {
    fillLevel = float.Parse(engines[i].DetailedInfo.Split('\n')[3].Split(' ')[1].Split('%')[0]);
  } catch { fillLevel = 0; }

  _enginesNeedFuel |= engines[i].Enabled && engines[i].IsFunctional && fillLevel < 0.5;
}

 _status.Add("Hydro Engines Active: " + needEngines);
      }

      // Return true if we have hydrogen thrusters but no hydrogen tanks
      bool DirectThrusters()
      {
if (_hasHydrogenTanks)
  return false;

List<IMyThrust> thrusters = new List<IMyThrust>();
_program.GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, block => IsMyGrid(block) && block.BlockDefinition.SubtypeId.Contains("Hydro"));

return thrusters.Count > 0;
      }

      void AdjustGenerators(SysMgrConfig cfg)
      {
double extraPower = 0;
List<IMyGasGenerator> generators = new List<IMyGasGenerator>();
_program.GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(generators, block => IsMyGrid(block));

if (generators.Count <= 0)
  return;

_hNeedsFilling = TanksNeedFilling(TankType.Hydrogen, cfg.h2Cutoff);
_o2NeedsFilling = (TanksNeedFilling(TankType.Oxygen, cfg.o2Cutoff) || PressureLow(cfg));
bool directToThrusters = DirectThrusters();
bool shouldEnable = false;
bool hasPower = false;
bool fillingH2 = false;
bool fillingO2 = false;

_wantPower = false;
for (int i = 0; i < generators.Count; i++)
{
  shouldEnable = (directToThrusters || _o2NeedsFilling || _hNeedsFilling || _enginesNeedFuel) &&
    GeneratorHasIce(generators[i]);
  extraPower += !generators[i].Enabled && shouldEnable? 0.1 : 0;
  hasPower = (_powerState.totalUsed + extraPower < _powerState.totalPossible);

  // Direct connections to thrusters ignore any power budget because there is no hydrogen storage
  // and it is assumed that the thrusters are most important and the pilot will save power elsewhere
  generators[i].Enabled = directToThrusters || (hasPower && shouldEnable);

  _wantPower |= shouldEnable && !hasPower;

  fillingH2 |= _hNeedsFilling && generators[i].Enabled;
  fillingO2 |= _o2NeedsFilling && generators[i].Enabled;
}

_status.Add("Direct to Thrusters: " + directToThrusters);
_status.Add("Hydrogen Filling: " + fillingH2);
_status.Add("Oxygen Filling: " + fillingO2);
      }

      new public void Manage(SysMgrConfig cfg)
      {
_status.Clear();
_powerState = GetPowerState();
if (cfg.manageEngines) { AdjustEngines(cfg); }
if(cfg.manageH2Generators) { AdjustGenerators(cfg); }
      }
    }

    public class Manager
    {
      protected Program _program;
      protected List<String> _status;
      protected List<String> _errors;
      public Manager(Program program)
      {
_program = program;
_status = new List<String>();
_errors = new List<String>();
      }

      protected bool IsMyGrid(IMyTerminalBlock block)
      {
return block.IsSameConstructAs(_program.Me);
//return block.CubeGrid == _program.Me.CubeGrid;
      }

      public void Manage(SysMgrConfig cfg) { }

      new public String ToString()
      {
return string.Join("\n", _status) + (_errors.Count() > 0 ? "\n\nErrors: \n" + string.Join("\n", _errors) : "");
      }
    }

    public class MirrorManager : Manager
    {
      Dictionary<string, string> mirrors;
      public MirrorManager(Program program) : base(program)
      {
mirrors = new Dictionary<string, string>();
      }

      new public void Manage(SysMgrConfig cfg)
      {
_status.Clear();
foreach (MyIniKey key in cfg.mirrors.Keys)
{
  string groupName = cfg.mirrors[key];
  if (groupName == "")
  {
    continue;
  }

  _status.Add("Mirroring power of " + key.Name + " to " + groupName);
  List<IMyTerminalBlock> list = new List<IMyTerminalBlock>();
  _program.GridTerminalSystem.SearchBlocksOfName(key.Name, list, block => block.IsSameConstructAs(_program.Me) && (block as IMyFunctionalBlock) != null);
  if (list.Count > 0) {
    List<IMyFunctionalBlock> blocks = new List<IMyFunctionalBlock>();
    bool powerState = (list[0] as IMyFunctionalBlock).Enabled;
    try
    {
      IMyBlockGroup group = _program.GridTerminalSystem.GetBlockGroupWithName(groupName);
      group.GetBlocksOfType<IMyFunctionalBlock>(blocks, block => block.CubeGrid == _program.Me.CubeGrid);
      foreach (IMyFunctionalBlock block in blocks)
      {
        block.Enabled = powerState;
      }
    }
    catch (Exception) { }
  }
}
      }
    }