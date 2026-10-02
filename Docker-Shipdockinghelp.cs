/*
 * A script that:
 * * Automatically locks/connects/merges landing gears/connectors/merge blocks when requesting to dock.
 * * Optionally disables certain blocks when docked, and enables them again when undocking.
 * * Optionally sets batteries to recharge and tanks to stockpile when docked, and sets them back to their previous mode when undocking.
 *
 * Run the script with 'dock' to enable docking mode. When docking requirements have been met, the ship will dock by locking/connecting/merging.
 * When no additional arguments are given, the ship will dock when any connector is in range of another connector to connect to.
 * Change the docking requirements with the following flags:
 *
 * * -landingAny  : Dock when any landing gear can be locked.
 * * -landingAny  : Dock when all landing gears can be locked
 * * -connectorAny: Dock when any connector can be connected.
 * * -connectorAll: Dock when all connectors can be connected.
 * * -mergeAny    : Dock when any merge block can be merged.
 * * -mergeAll    : Dock when all merge blocks can be merged.
 *
 * For example:
 *
 * * 'dock -landingAny' will dock the ship when any landing gear can be locked.
 * * 'dock -landingAll' will dock the ship when all landing gears can be locked.
 * * 'dock -landingAll -connectorAny' will dock the ship when all landing gears can be locked, and any connector can be connected.
 *
 * By default, ships will not recharge batteries nor stockpile their tanks. To enable this, add the -refuel flag.
 *
 * For example:
 *
 * * 'dock -landingAll -connectorAny -refuel' will dock the ship when all landing gears can be locked, and any connector can be
 *                                            connected, and will set all batteries to recharge, and all tanks to stockpile.
 *
 * To undock, run the script with 'undock'.
 *
 * The script can be configured by changing the Custom Data of the programming block, after which a recompile of the script is required.
 */

    private readonly List<IMyShipConnector> _connectorsCache = new List<IMyShipConnector>();
    private readonly List<IMyLandingGear> _landingGearsCache = new List<IMyLandingGear>();
    private readonly List<IMyShipMergeBlock> _mergesCache = new List<IMyShipMergeBlock>();

    private readonly List<IMyShipController> _shipControllersCache = new List<IMyShipController>();
    private readonly List<IMyThrust> _thrustersCache = new List<IMyThrust>();
    private readonly List<IMyGyro> _gyroscopesCache = new List<IMyGyro>();

    private readonly List<IMySmallGatlingGun> _gatlingGunsCache = new List<IMySmallGatlingGun>();
    private readonly List<IMyLargeGatlingTurret> _gatlingTurretsCache = new List<IMyLargeGatlingTurret>();
    private readonly List<IMySmallMissileLauncher> _missileLaunchersCache = new List<IMySmallMissileLauncher>();
    private readonly List<IMySmallMissileLauncherReload> _missileLaunchersReloadCache =
      new List<IMySmallMissileLauncherReload>();
    private readonly List<IMyLargeMissileTurret> _missileTurretsCache = new List<IMyLargeMissileTurret>();

    private readonly List<IMyRadioAntenna> _radioAntennasCache = new List<IMyRadioAntenna>();
    private readonly List<IMyBeacon> _beaconsCache = new List<IMyBeacon>();
    private readonly List<IMyOreDetector> _oreDetectorsCache = new List<IMyOreDetector>();

    private readonly List<IMyBatteryBlock> _batteriesCache = new List<IMyBatteryBlock>();
    private readonly List<IMyGasTank> _gasTanksCache = new List<IMyGasTank>();

    private readonly List<IMySoundBlock> _soundBlockCache = new List<IMySoundBlock>();
    private int _nextSoundBlock;

    private readonly MyCommandLine _commandLine = new MyCommandLine();


    private MyIni _settingsIni = new MyIni();

    private bool _manageLandingGears;
    private bool _manageConnectors;
    private bool _manageMergeBlocks;

    private bool _manageDampeners;
    private bool _manageHandbrake;
    private bool _manageThrusters;
    private bool _manageGyroscopes;

    private bool _manageGatlingGuns;
    private bool _manageGatlingTurrets;
    private bool _manageMissileLaunchers;
    private bool _manageMissileTurrets;

    private bool _manageAntennas;
    private bool _manageBeacons;
    private bool _manageOreDetectors;

    private bool _manageBatteries;
    private bool _manageHydrogenTanks;

    private bool _manageOxygenTanks;

    private bool _manageSoundBlocks;

    private bool _debugPrint;


    private MyIni _storageIni = new MyIni();

    private bool _inDockingProcedure = false;
    private DockingMode _dockingMode = DockingMode.None;
    private bool _refuel = false;


    public Program() {
      ReloadSettings();
      ReloadStorage();
    }

    public void Save() {
      SaveStorage();
    }

    public void Main(string argument, UpdateType updateSource) {
      try {
if(updateSource == UpdateType.Update1 || updateSource == UpdateType.Update10 ||
   updateSource == UpdateType.Update100) {
  DockIfAble();
} else {
  if(!_commandLine.TryParse(argument)) {
    Error($"Cannot parse input argument {argument}");
    return;
  }
  var command = _commandLine.Argument(0);
  if(command == null) {
    Error("No command was given");
    return;
  }
  if(command == "dock") {
    var dockingMode = DockingMode.None;
    if(_commandLine.Switch("landing") || _commandLine.Switch("landingAny"))
      dockingMode |= DockingMode.LandingAny;
    if(_commandLine.Switch("landingAll")) dockingMode |= DockingMode.LandingAll;
    if(_commandLine.Switch("connector") || _commandLine.Switch("connectorAny"))
      dockingMode |= DockingMode.ConnectorAny;
    if(_commandLine.Switch("connectorAll")) dockingMode |= DockingMode.ConnectorAll;
    if(_commandLine.Switch("merge") || _commandLine.Switch("mergeAny")) dockingMode |= DockingMode.MergeAny;
    if(_commandLine.Switch("mergeAll")) dockingMode |= DockingMode.MergeAll;
    if(dockingMode == DockingMode.None) dockingMode = DockingMode.ConnectorAny;
    var refuel = _commandLine.Switch("refuel");
    Debug($"Running RequestDock({dockingMode}, {refuel})");
    RequestDock(dockingMode, refuel);
  } else if(command == "undock") {
    Debug("Running Undock()");
    Undock();
  } else if(command == "debugPrintStorage") {
    Debug("Running Echo(Storage)");
    Echo(Storage);
  } else if(command == "debugSaveStorage") {
    Debug("Running SaveStorage()");
    SaveStorage();
  } else if(command == "debugResetStorage") {
    Debug("Running ResetStorage()");
    ResetStorage();
  } else {
    Error($"Unknown command: {argument}");
  }
}
      } catch(Exception e) {
Error($"Exception: {e}");
      }
    }


    private void ReloadSettings() {
      MyIniParseResult result;
      if(!_settingsIni.TryParse(Me.CustomData, out result)) {
throw new Exception(
  $"ERROR: Could not parse INI settings from custom data. Please check and/or clear the custom data of this programming block. Error was: {result}");
      }

      _manageLandingGears = GetSetting("Block", "ManageLandingGears");
      _manageConnectors = GetSetting("Block", "ManageConnectors");
      _manageMergeBlocks = GetSetting("Block", "ManageMergeBlocks", false);

      _manageDampeners = GetSetting("Grid", "ManageDampeners");
      _manageHandbrake = GetSetting("Grid", "ManageHandbrake");
      _manageThrusters = GetSetting("Block", "ManageThrusters");
      _manageGyroscopes = GetSetting("Block", "ManageGyroscopes");

      _manageGatlingGuns = GetSetting("Block", "ManageGatlingGuns");
      _manageGatlingTurrets = GetSetting("Block", "ManageGatlingTurrets", false);
      _manageMissileLaunchers = GetSetting("Block", "ManageMissileLaunchers");
      _manageMissileTurrets = GetSetting("Block", "ManageMissileTurrets", false);

      _manageAntennas = GetSetting("Block", "ManageAntennas");
      _manageBeacons = GetSetting("Block", "ManageBeacons");
      _manageOreDetectors = GetSetting("Block", "ManageOreDetectors");

      _manageBatteries = GetSetting("Block", "ManageBatteries");
      _manageHydrogenTanks = GetSetting("Block", "ManageHydrogenTanks");

      _manageOxygenTanks = GetSetting("Block", "ManageOxygenTanks");

      _manageSoundBlocks = GetSetting("Block", "ManageSoundBlocks", false);

      _debugPrint = GetSetting("Debug", "Print", false);

      Me.CustomData = _settingsIni.ToString();
    }

    private bool GetSetting(string section, string name, bool @default = true) {
      return _settingsIni.GetAndSetIfNotFound(new MyIniKey($"Docker.{section}", name), @default);
    }


    private void ReloadStorage() {
      MyIniParseResult result;
      if(!_storageIni.TryParse(Storage, out result)) {
Error($"Could not parse INI settings from script storage, resetting script storage. Error was: {result}");
      }

      _inDockingProcedure = GetStorage("Dock", "InDockingProcedure", false);
      _dockingMode = (DockingMode) GetStorage("Dock", "DockingMode", (int) DockingMode.None);
      _refuel = GetStorage("Dock", "Refuel", false);
    }

    private void ResetStorage() {
      _storageIni.Clear();
      SaveStorage();
    }

    private void SaveStorage() {
      SetStorage("Dock", "InDockingProcedure", _inDockingProcedure);
      SetStorage("Dock", "DockingMode", (int) _dockingMode);
      SetStorage("Dock", "Refuel", _refuel);
      Storage = _storageIni.ToString();
    }

    private bool GetStorage(string section, string name, bool @default = true) {
      var value = _storageIni.GetOrDefault(new MyIniKey(section, name), @default);
      Debug($"Getting storage [{section}] {name} = {value}");
      return value;
    }

    private int GetStorage(string section, string name, int @default = 0) {
      var value = _storageIni.GetOrDefault(new MyIniKey(section, name), @default);
      Debug($"Getting storage [{section}] {name} = {value}");
      return value;
    }

    private void SetStorage(string section, string name, bool value) {
      Debug($"Setting storage [{section}] {name} = {value}");
      _storageIni.Set(new MyIniKey(section, name), value);
    }

    private void SetStorage(string section, string name, int value) {
      Debug($"Setting storage [{section}] {name} = {value}");
      _storageIni.Set(new MyIniKey(section, name), value);
    }


    private bool DockCheck(DockingMode dockingMode, Func<IMyLandingGear, bool> landingGearFunc,
      Func<IMyShipConnector, bool> connectorFunc, bool echo) {
      if(dockingMode == DockingMode.None) {
return false;
      }

      var grid = Me.CubeGrid;
      var system = GridTerminalSystem;
      if(_manageLandingGears && dockingMode.IsLanding()) {
var blocks = system.Blocks(_landingGearsCache, grid);
if(dockingMode.CheckMask(DockingMode.LandingAll) && blocks.Any(landingGearFunc)) {
  if(echo) Info("Cannot dock yet, SOME landing gears are NOT ready to lock");
  return false;
}
if(dockingMode.CheckMask(DockingMode.LandingAny) && blocks.All(landingGearFunc)) {
  if(echo) Info("Cannot dock yet, ALL landing gears are NOT ready to lock");
  return false;
}
      }
      if(_manageConnectors && dockingMode.IsConnecting()) {
var blocks = system.Blocks(_connectorsCache, grid);
if(dockingMode.CheckMask(DockingMode.ConnectorAll) && blocks.Any(connectorFunc)) {
  if(echo) Info("Cannot dock yet, SOME connectors are NOT connectable");
  return false;
}
if(dockingMode.CheckMask(DockingMode.ConnectorAny) && blocks.All(connectorFunc)) {
  if(echo) Info("Cannot dock yet, ALL connectors are NOT connectable");
  return false;
}
      }
      if(_manageMergeBlocks && dockingMode.IsMerging()) {
var blocks = system.Blocks(_mergesCache, grid);
if(dockingMode.CheckMask(DockingMode.MergeAll) && blocks.Any(b => !b.IsFunctional || !b.IsConnected)) {
  if(echo) Info("Cannot dock yet, SOME merge blocks are NOT connected");
  return false;
}
if(dockingMode.CheckMask(DockingMode.MergeAny) && blocks.All(b => !b.IsFunctional || !b.IsConnected)) {
  if(echo) Info("Cannot dock yet, ALL merge blocks are NOT connected");
  return false;
}
      }
      return true;
    }

    private bool CanDock(DockingMode dockingMode) {
      return DockCheck(dockingMode,
b => !b.IsFunctional || b.LockMode == LandingGearMode.Unlocked,
b => !b.IsFunctional || b.Status == MyShipConnectorStatus.Unconnected,
true
      );
    }

    private bool IsDocked(DockingMode dockingMode) {
      return DockCheck(dockingMode,
b => !b.IsFunctional || b.LockMode != LandingGearMode.Locked,
b => !b.IsFunctional || b.Status != MyShipConnectorStatus.Connected,
false
      );
    }

    private void RequestDock(DockingMode dockingMode, bool refuel) {
      if(refuel && !dockingMode.SupportsRefuel()) {
Error($"cannot dock and refuel with docking mode {dockingMode}, it does not support refueling");
return;
      }

      // Prevent restarting docking sequence when docked. Also check with previous docking mode to prevent restarting
      // the docking sequence while docked with previous mode.
      if(IsDocked(dockingMode) || IsDocked(_dockingMode)) {
if(refuel != _refuel) {
  // Update refuel status if it was different.
  _refuel = refuel;
  if(refuel) EnableRefuel();
  else DisableRefuel();
}
return; // Already docked, do nothing.
      }

      _dockingMode = dockingMode;
      _refuel = refuel;

      ManageAll(BlockAction.RequestDock);
      EnableDockingProcedure();

      PlaySound("VL: Command confirmed.");
    }

    private void DockIfAble() {
      Info("Attempting to dock...");
      if(!CanDock(_dockingMode)) return;
      Dock();
      if(!IsDocked(_dockingMode)) return;
      Info("Successfully docked");
      AfterDocked();
      DisableDockingProcedure();
    }

    private void Dock() {
      ManageAll(BlockAction.Dock);
    }

    private void AfterDocked() {
      ManageAll(BlockAction.AfterDocked);
      if(_refuel) {
EnableRefuel();
      } else {
DisableRefuel();
      }
      if(_dockingMode.IsMerging()) {
PlaySound("VL: Lockdown ... enganged.	");
      } else if(_dockingMode.IsConnecting()) {
PlaySound("VL: Connector ... locked.");
      } else if(_dockingMode.IsLanding()) {
PlaySound("VL: Landing gears ... locked.");
      }
    }

    private void EnableRefuel() {
      ManageAll(BlockAction.EnableRefuel);
      PlaySound("VL: On.");
    }

    private void DisableRefuel() {
      ManageAll(BlockAction.DisableRefuel);
      PlaySound("VL: Off.");
    }

    private void Undock() {
      if(_inDockingProcedure) {
PlaySound("VL: Aborted.");
      } else {
if(IsDocked(_dockingMode)) {
  if(_dockingMode.IsMerging()) {
    PlaySound("VL: Lockdown ... disengaged.	");
  } else if(_dockingMode.IsConnecting()) {
    PlaySound("VL: Connector ... unlocked.");
  } else if(_dockingMode.IsLanding()) {
    PlaySound("VL: Landing gears ... unlocked.");
  }
}
      }

      ManageAll(BlockAction.Undock);
      DisableDockingProcedure(); // Disable docking procedure if one was still in progress.
      _dockingMode = DockingMode.None;
      _refuel = false;
    }


    private void EnableDockingProcedure() {
      _inDockingProcedure = true;
      Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }

    private void DisableDockingProcedure() {
      Runtime.UpdateFrequency = UpdateFrequency.None;
      _inDockingProcedure = false;
    }


    private void ManageConnectors(BlockAction action) {
      if(!_manageConnectors) return;
      foreach(var block in GridTerminalSystem.Blocks(_connectorsCache, Me.CubeGrid)) {
if(block.BlockDefinition.SubtypeId.Contains("ConnectorSmall")) continue; // Skip ejectors
if(action == BlockAction.RequestDock) {
  block.Enabled = true;
} else if(action == BlockAction.Dock && _dockingMode.IsConnecting()) {
  block.Connect();
} else if(action == BlockAction.Undock) {
  block.Enabled = false;
  block.Disconnect();
}
      }
    }

    private void ManageLandingGears(BlockAction action) {
      if(!_manageLandingGears) return;
      foreach(var block in GridTerminalSystem.Blocks(_landingGearsCache, Me.CubeGrid)) {
if(action == BlockAction.RequestDock) {
  SetStorage("Block.AutoLock", block.EntityId.ToString(), block.AutoLock);
  block.Enabled = true;
  block.AutoLock = false;
} else if(action == BlockAction.Dock && _dockingMode.IsLanding()) {
  block.Lock();
} else if(action == BlockAction.Undock) {
  block.Enabled = true;
  block.Unlock();
  block.AutoLock = GetStorage("Block.AutoLock", block.EntityId.ToString(), false);
}
      }
    }

    private void ManageMergeBlocks(BlockAction action) {
      if(!_manageMergeBlocks) return;
      foreach(var block in GridTerminalSystem.Blocks(_mergesCache, Me.CubeGrid)) {
if(action == BlockAction.RequestDock && _dockingMode.IsMerging()) {
  block.Enabled = true;
} else if(action == BlockAction.Undock) {
  block.Enabled = false;
}
      }
    }


    private void ManageDampeners(BlockAction action) {
      if(!_manageDampeners) return;
      var shipController = GridTerminalSystem.Blocks(_shipControllersCache, Me.CubeGrid).FirstOrDefault();
      if(shipController == null) return;
      if(action == BlockAction.RequestDock) {
SetStorage("Grid", "Dampeners", shipController.DampenersOverride);
      } else if(action == BlockAction.Dock) {
shipController.DampenersOverride = false;
      } else if(action == BlockAction.Undock) {
shipController.DampenersOverride = GetStorage("Grid", "Dampeners", true);
      }
    }

    private void ManageHandbrake(BlockAction action) {
      if(!_manageHandbrake) return;
      var shipController = GridTerminalSystem.Blocks(_shipControllersCache, Me.CubeGrid).FirstOrDefault();
      if(shipController == null) return;
      if(action == BlockAction.RequestDock) {
SetStorage("Grid", "Handbrake", shipController.HandBrake);
      } else if(action == BlockAction.Dock) {
shipController.HandBrake = true;
      } else if(action == BlockAction.Undock) {
shipController.HandBrake = GetStorage("Grid", "Handbrake", true);
      }
    }

    private void ManageThrusters(BlockAction action) {
      ManageFunctionalBlocks(action, _manageThrusters, _thrustersCache);
    }

    private void ManageGyroscopes(BlockAction action) {
      ManageFunctionalBlocks(action, _manageGyroscopes, _gyroscopesCache);
    }


    private void ManageGatlingGuns(BlockAction action) {
      ManageFunctionalBlocks(action, _manageGatlingGuns, _gatlingGunsCache);
    }

    private void ManageGatlingTurrets(BlockAction action) {
      ManageFunctionalBlocks(action, _manageGatlingTurrets, _gatlingTurretsCache);
    }

    private void ManageMissileLaunchers(BlockAction action) {
      ManageFunctionalBlocks(action, _manageMissileLaunchers, _missileLaunchersCache);
      ManageFunctionalBlocks(action, _manageMissileLaunchers, _missileLaunchersReloadCache);
    }

    private void ManageMissileTurrets(BlockAction action) {
      ManageFunctionalBlocks(action, _manageMissileTurrets, _missileTurretsCache);
    }


    private void ManageAntennas(BlockAction action) {
      ManageFunctionalBlocks(action, _manageAntennas, _radioAntennasCache);
    }

    private void ManageBeacons(BlockAction action) {
      ManageFunctionalBlocks(action, _manageBeacons, _beaconsCache);
    }

    private void ManageOreDetectors(BlockAction action) {
      ManageFunctionalBlocks(action, _manageOreDetectors, _oreDetectorsCache);
    }


    private void ManageBatteries(BlockAction action) {
      if(!_manageBatteries) return;
      foreach(var block in GridTerminalSystem.Blocks(_batteriesCache, Me.CubeGrid)) {
if(action == BlockAction.RequestDock) {
  SetStorage("Block.Enabled", block.EntityId.ToString(), block.Enabled);
  SetStorage("Block.ChargeMode", block.EntityId.ToString(), (int) block.ChargeMode);
} else if(action == BlockAction.EnableRefuel) {
  block.ChargeMode = ChargeMode.Recharge;
  block.Enabled = true;
} else if(action == BlockAction.DisableRefuel) {
  // Disable battery to prevent it from draining, if docking method would support refueling.
  if(_dockingMode.SupportsRefuel()) block.Enabled = false;
} else if(action == BlockAction.Undock) {
  block.Enabled = GetStorage("Block.Enabled", block.EntityId.ToString(), true);
  block.ChargeMode =
    (ChargeMode) GetStorage("Block.ChargeMode", block.EntityId.ToString(), (int) ChargeMode.Discharge);
}
      }
    }

    private void ManageGasTanks(BlockAction action, bool shouldManage, string containsSubtypeId) {
      if(!shouldManage) return;
      foreach(var block in GridTerminalSystem.Blocks(_gasTanksCache, Me.CubeGrid)) {
if(!block.BlockDefinition.SubtypeId.Contains(containsSubtypeId)) continue;
if(action == BlockAction.RequestDock) {
  SetStorage("Block.Enabled", block.EntityId.ToString(), block.Enabled);
  SetStorage("Block.Stockpile", block.EntityId.ToString(), block.Stockpile);
} else if(action == BlockAction.EnableRefuel) {
  block.Stockpile = true;
  block.Enabled = true;
} else if(action == BlockAction.DisableRefuel) {
  // Disable gas tank to prevent it from draining, if docking method would support refueling.
  if(_dockingMode.SupportsRefuel()) block.Enabled = false;
} else if(action == BlockAction.Undock) {
  block.Enabled = GetStorage("Block.Enabled", block.EntityId.ToString(), true);
  block.Stockpile = GetStorage("Block.Stockpile", block.EntityId.ToString(), false);
}
      }
    }


    private void ManageDockingBlocks(BlockAction action) {
      ManageLandingGears(action); // Manage landing gears first; they do not affect grid merging.
      ManageConnectors(action);
      ManageMergeBlocks(action);
    }

    private void ManagePowerBlocks(BlockAction action) {
      ManageBatteries(action);
      ManageGasTanks(action, _manageHydrogenTanks, "Hydrogen");
    }

    private void ManageAll(BlockAction action) {
      if(action == BlockAction.Undock) {
ManagePowerBlocks(action); // When undocking, first restore power before disengaging docking blocks.
      } else {
ManageDockingBlocks(action);
      }

      ManageDampeners(action);
      ManageHandbrake(action);
      ManageThrusters(action);
      ManageGyroscopes(action);

      ManageGatlingGuns(action);
      ManageGatlingTurrets(action);
      ManageMissileLaunchers(action);
      ManageMissileTurrets(action);

      ManageAntennas(action);
      ManageBeacons(action);
      ManageOreDetectors(action);

      ManageGasTanks(action, _manageOxygenTanks, "Oxygen");

      if(action == BlockAction.Undock) {
ManageDockingBlocks(action); // When undocking, disengage docking blocks at the very end.
      } else {
ManagePowerBlocks(action);
      }
    }


    private enum BlockAction {
      RequestDock,
      Dock,
      AfterDocked,
      EnableRefuel,
      DisableRefuel,
      Undock
    }

    private void ManageFunctionalBlocks<T>(BlockAction action, bool shouldManage, List<T> cache)
      where T : class, IMyFunctionalBlock {
      if(!shouldManage) return;
      foreach(var block in GridTerminalSystem.Blocks(cache, Me.CubeGrid)) {
if(action == BlockAction.RequestDock) {
  SetStorage("Block.Enabled", block.EntityId.ToString(), block.Enabled);
} else if(action == BlockAction.AfterDocked) {
  block.Enabled = false;
} else if(action == BlockAction.Undock) {
  block.Enabled = GetStorage("Block.Enabled", block.EntityId.ToString(), true);
}
      }
    }


    private void PlaySound(string name) {
      if(!_manageSoundBlocks) return;
      var soundBlocks = GridTerminalSystem.Blocks(_soundBlockCache, Me.CubeGrid);
      if(soundBlocks.Count == 0) {
_nextSoundBlock = -1;
      } else if(_nextSoundBlock >= soundBlocks.Count - 1) {
_nextSoundBlock = 0;
      } else {
++_nextSoundBlock;
      }
      if(_nextSoundBlock != -1) {
var block = soundBlocks[_nextSoundBlock];
block.SelectedSound = name;
block.Play();
      }
    }


    private void Error(string str) {
      Echo($"ERROR: {str}");
      PlaySound("VL: Alert.	");
    }

    private void Info(string str) {
      Echo(str);
    }

    private void Debug(string str) {
      if(_debugPrint) {
Echo(str);
      }
    }

}


  [Flags]
  public enum DockingMode {
None = 0,
// ReSharper disable once ShiftExpressionRealShiftCountIsZero
LandingAny = 1 << 0,
LandingAll = 1 << 1,
ConnectorAny = 1 << 2,
ConnectorAll = 1 << 3,
MergeAny = 1 << 4,
MergeAll = 1 << 5,
  }


  static class DockingModeExtensions {
public static bool CheckMask(this DockingMode dockingMode, DockingMode mask) {
  return (dockingMode & mask) != 0;
}

public static bool IsLanding(this DockingMode dockingMode) {
  return dockingMode.CheckMask(DockingMode.LandingAny | DockingMode.LandingAll);
}

public static bool IsConnecting(this DockingMode dockingMode) {
  return dockingMode.CheckMask(DockingMode.ConnectorAny | DockingMode.ConnectorAll);
}

public static bool IsMerging(this DockingMode dockingMode) {
  return dockingMode.CheckMask(DockingMode.MergeAny | DockingMode.MergeAll);
}

public static bool SupportsRefuel(this DockingMode dockingMode) {
  return dockingMode.IsConnecting() || dockingMode.IsMerging();
}
  }

  public static class Extensions {
public static System.Text.RegularExpressions.Regex CreateRegex(string pattern) {
  return new System.Text.RegularExpressions.Regex(pattern,
    System.Text.RegularExpressions.RegexOptions.Compiled);
}

public static List<T> Blocks<T>(
  this IMyGridTerminalSystem system,
  List<T> cache = null,
  IMyCubeGrid grid = null,
  Func<T, bool> filter = null
) where T : class, IMyCubeBlock {
  List<T> blocks;
  if(cache != null) {
    cache.Clear();
    blocks = cache;
  } else {
    blocks = new List<T>();
  }

  if(grid == null) {
    system.GetBlocksOfType(blocks, filter);
  } else {
    system.GetBlocksOfType(blocks, block => block.CubeGrid.Equals(grid) && (filter?.Invoke(block) ?? true));
  }

  return blocks;
}

public static List<T> BlocksWithTag<T>(
  this IMyGridTerminalSystem system,
  string tag,
  List<T> cache = null,
  IMyCubeGrid grid = null
) where T : class, IMyTerminalBlock {
  return system.Blocks(cache, grid, block => block.CustomName.Contains(tag));
}

public static List<T> Blocks<T>(
  this IMyBlockGroup group,
  List<T> cache = null,
  IMyCubeGrid grid = null,
  Func<T, bool> filter = null
) where T : class, IMyCubeBlock {
  List<T> blocks;
  if(cache != null) {
    cache.Clear();
    blocks = cache;
  } else {
    blocks = new List<T>();
  }

  if(grid == null) {
    group.GetBlocksOfType(blocks, filter);
  } else {
    group.GetBlocksOfType(blocks, block => block.CubeGrid.Equals(grid) && (filter?.Invoke(block) ?? true));
  }

  return blocks;
}

public static List<T> BlocksWithTag<T>(
  this IMyBlockGroup group,
  string tag,
  List<T> cache = null,
  IMyCubeGrid grid = null
) where T : class, IMyTerminalBlock {
  return group.Blocks(cache, grid, block => block.CustomName.Contains(tag));
}

public static List<IMyBlockGroup> Groups(
  this IMyGridTerminalSystem system,
  List<IMyBlockGroup> cache = null,
  Func<IMyBlockGroup, bool> filter = null
) {
  List<IMyBlockGroup> groups;
  if(cache != null) {
    cache.Clear();
    groups = cache;
  } else {
    groups = new List<IMyBlockGroup>();
  }

  system.GetBlockGroups(groups, filter);
  return groups;
}

public static List<IMyBlockGroup> GroupsWithTag(
  this IMyGridTerminalSystem system,
  string tag,
  List<IMyBlockGroup> cache = null
) {
  return system.Groups(cache, group => group.Name.Contains(tag));
}


public static bool GetOrDefault(this MyIni ini, MyIniKey key, bool @default = false) {
  var value = ini.Get(key);
  if(!value.IsEmpty) return value.ToBoolean();
  return @default;
}

public static int GetOrDefault(this MyIni ini, MyIniKey key, int @default = 0) {
  var value = ini.Get(key);
  if(!value.IsEmpty) return value.ToInt32(@default);
  return @default;
}

public static string GetOrDefault(this MyIni ini, MyIniKey key, string @default = "") {
  var value = ini.Get(key);
  if(!value.IsEmpty) return value.ToString();
  return @default;
}


public static bool GetAndSetIfNotFound(this MyIni ini, MyIniKey key, bool @default = false) {
  var value = ini.Get(key);
  if(!value.IsEmpty) return value.ToBoolean();
  ini.Set(key, @default);
  return @default;
}

public static int GetAndSetIfNotFound(this MyIni ini, MyIniKey key, int @default = 0) {
  var value = ini.Get(key);
  if(!value.IsEmpty) return value.ToInt32();
  ini.Set(key, @default);
  return @default;
}

public static float GetAndSetIfNotFound(this MyIni ini, MyIniKey key, float @default = 0.0f) {
  var value = ini.Get(key);
  if(!value.IsEmpty) return value.ToSingle();
  ini.Set(key, @default);
  return @default;
}

public static string GetAndSetIfNotFound(this MyIni ini, MyIniKey key, string @default = "") {
  var value = ini.Get(key);
  if(!value.IsEmpty) return value.ToString();
  ini.Set(key, @default);
  return @default;
}


public static void SetIfNotFound(this MyIni ini, MyIniKey key, bool value) {
  var existingValue = ini.Get(key);
  if(!existingValue.IsEmpty) return;
  ini.Set(key, value);
}