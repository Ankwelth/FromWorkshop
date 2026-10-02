/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

// This file contains your actual script.
//
// You can either keep all your code here, or you can create separate
// code files to make your program easier to navigate while coding.
//
// In order to add a new utility class, right-click on your project,
// select 'New' then 'Add Item...'. Now find the 'Space Engineers'
// category under 'Visual C# Items' on the left hand side, and select
// 'Utility Class' in the main area. Name it in the box below, and
// press OK. This utility class will be merged in with your code when
// deploying your final script.
//
// You can also simply create a new utility class manually, you don't
// have to use the template if you don't want to. Just do so the first
// time to see what a utility class looks like.
//
// Go to:
// https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
//
// to learn more about ingame scripts.
IEnumerator<bool> _stateMachine;

Factory factory = new Factory
{
    Assemblers = new List<Assembler>(),
    Blocks = new List<IMyTerminalBlock>(),
    Cargos = new List<Cargo>(),
    Connectors = new List<IMyShipConnector>(),
    Inventory = new List<InventoryItem>(),
    Reactors = new List<IMyReactor>(),
    Refineries = new List<IMyRefinery>()
};

InventoryItem xfrItem = new InventoryItem()
{
    Name = "",
    Quantity = 0,
    Type = ""
};

List<string> possibleTags = new List<string> { "Ore", "Component", "Ingot", "Tool", "Ammo", "Bottle" };

Cargo outputCargo = new Cargo()
{
    Tags = new List<string>()
};
MyIni _ini = new MyIni();
List<MyInventoryItem> contents = new List<MyInventoryItem>();
IMyInventory destination;
IMyInventory source;
MyFixedPoint destinationSpace;
int step = 0;
int cargoCounter = 0;
int assemblerCounter = 0;
string type;
string subtype;
Assembler currentAssembler = new Assembler();
string translatedType;
string translatedSubtype;
private bool hasMoreSteps;

Dictionary<string, string> translations = new Dictionary<string, string>()
{
    { "Welder4Item", "Elite Welder" },
    { "Welder3Item", "Proficient Welder" },
    { "Welder2Item", "Enhanced Welder" },
    { "WelderItem", "Welder" },
    { "Drill4Item", "Elite Drill" },
    { "Drill3Item", "Proficient Drill" },
    { "Drill2Item", "Enhanced Drill" },
    { "DrillItem", "Drill" },
    { "AngleGrinder4Item", "Elite Grinder" },
    { "AngleGrinder3Item", "Proficient Grinder" },
    { "AngleGrinder2Item", "Enhanced Grinder" },
    { "AngleGrinderItem", "Grinder" },
    { "KaelniumComponent", "Kaelnium Foil" },
    { "KaelniumIngot", "Kaelnium" },
    { "KaelsiliteComponent", "Kaelinium Plate" },
    { "PhysicalGunObject", "Tool" },
    { "AmmoMagazine", "Ammo" },
    { "NATO_5p56x45mm", "NATO 5.56x45mm" },
    { "OxygenContainerObject", "Bottle" }
};

Dictionary<MyInventoryItem, MyDefinitionId> assemblerRecipes = new Dictionary<MyInventoryItem, MyDefinitionId>()
{

};

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    _stateMachine = UpdateCargos();
    UpdateBlocks();
}

public void Save()
{
    _ini.Clear();
    foreach (var item in factory.Inventory.OrderBy(i => i.Type).ThenBy(i => i.Name))
    {
        _ini.Set("Quotas", $"{item.Type}/{item.Name}", (float)item.Quantity);
    }
    Me.CustomData = _ini.ToString();
}

public void Main(string argument, UpdateType updateSource)
{
    Echo($"Run Time: {Runtime.LastRunTimeMs:N4}ms");
    Echo("1");
    switch (step)
    {
        case 0:
            Echo($"Step {step + 1}: Updating Grid Info");
            UpdateBlocks();
            NextStep();
            break;

        case 1:
            Echo($"Step {step + 1}: Updating Cargos");
            Echo($"Updating Cargo {cargoCounter}/{factory.Cargos.Count}...");
            hasMoreSteps = _stateMachine.MoveNext();
            if (hasMoreSteps)
                Runtime.UpdateFrequency |= UpdateFrequency.Once;
            else
            {
                _stateMachine.Dispose();
                _stateMachine = UpdateAssemblers();
            }

            UpdateCargos();
            break;
        case 2:
            Echo($"Step {step + 1}: Updating Assemblers");
            Echo($"Updating Assembler {assemblerCounter}/{factory.Assemblers.Count}...");
            hasMoreSteps = _stateMachine.MoveNext();
            if (hasMoreSteps)
                Runtime.UpdateFrequency |= UpdateFrequency.Once;
            else
            {
                _stateMachine.Dispose();
                _stateMachine = UpdateCargos();
            }

            UpdateCargos();
            break;
        case 3:
            Echo($"Step {step + 1}: Updating Inventory");
            hasMoreSteps = _stateMachine.MoveNext();
            if (hasMoreSteps)
                Runtime.UpdateFrequency |= UpdateFrequency.Once;
            else
            {
                _stateMachine.Dispose();
                _stateMachine = UpdateCargos();
            }

            //UpdateInventory();
            break;
        case 4:
            Echo($"Step {step + 1}: Updating Autocraft Queue");
            //AutoCraft();
            break;
    }


    Echo(" ");
    Echo("Factory OS v1.1");
    Echo("Author: Orbitect");
    Echo("Issues: Orbitect#0042");
    ReadConfig();

}

public void UpdateBlocks()
{
    GridTerminalSystem.GetBlocks(factory.Blocks);
    factory.Cargos.Clear();
    factory.Assemblers.Clear();
    factory.Cargos = factory.Cargos.OrderBy(c => c.Capacity).ToList();

    foreach (var block in factory.Blocks.Where(b => b.GetType().Name == "MyCargoContainer").ToList())
    {
        factory.Cargos.Add(new Cargo { Block = (IMyTerminalBlock)block, Tags = new List<string>() });
    }

    foreach (var block in factory.Blocks.Where(b => b.GetType().Name == "MyAssembler").ToList())
    {
        factory.Assemblers.Add(new Assembler() { Block = (IMyProductionBlock)block });
    }

    foreach (var assembler in factory.Assemblers)
    {
        assembler.InputInventory = assembler.Block.InputInventory;
        assembler.OutputInventory = assembler.Block.OutputInventory;
    }

    foreach (var cargo in factory.Cargos)
    {
        foreach (var tag in possibleTags)
        {
            if (cargo.Block.CustomName.ToLower().Contains(tag.ToLower()))
            {
                cargo.Tags.Add(tag);
            }
        }
    }

    outputCargo = factory.Cargos.FirstOrDefault();
    if (outputCargo != null && !factory.Inventory.Any())
    {
        List<MyItemType> acceptedItems = new List<MyItemType>();
        outputCargo.Block.GetInventory().GetAcceptedItems(acceptedItems);
        foreach (var item in acceptedItems)
        {
            type = item.TypeId.Split('_')[1];
            if (translations.ContainsKey(type))
                translations.TryGetValue(type, out translatedType);
            else
                translatedType = type;


            subtype = item.SubtypeId;
            if (translations.ContainsKey(subtype))
                translations.TryGetValue(subtype, out translatedSubtype);
            else
                translatedSubtype = System.Text.RegularExpressions.Regex
                    .Replace(item.SubtypeId, @"([A-Z])", @" $1").Trim();
            factory.Inventory.Add(new InventoryItem(){Name = subtype, Type = type});
        }
    }

    GridTerminalSystem.GetBlocksOfType(factory.Connectors);
    GridTerminalSystem.GetBlocksOfType(factory.Reactors);
    GridTerminalSystem.GetBlocksOfType(factory.Refineries);
}

public void ReadConfig()
{
    MyIniParseResult result;
    if (!_ini.TryParse(Me.CustomData, out result))
        throw new Exception(result.ToString());
    foreach (var item in factory.Inventory)
    {
        item.Quantity = (MyFixedPoint)_ini.Get("Quotas", $"{item.Type}/{item.Name}").ToSingle();
    }
}

public IEnumerator<bool> UpdateCargos()
{
    foreach (var cargo in factory.Cargos)
    {
        foreach (var tag in possibleTags)
        {
            if (cargo.Block.CustomName.ToLower().Contains(tag.ToLower()))
            {
                cargo.Tags.Add(tag);
            }
        }
        cargo.Capacity = (MyFixedPoint)(((float)cargo.Block.GetInventory().CurrentVolume /
                                         (float)cargo.Block.GetInventory().MaxVolume) * 100);
        cargo.Block.CustomName =
            cargo.Block.CustomName.Split('(')[0] + $"({Math.Round((float)cargo.Capacity)}%)";
        cargo.Block.GetInventory().GetItems(contents);
        foreach (var thing in contents)
        {
            if (!cargo.Tags.Contains(GetType(thing)))
            {
                Sort(thing, cargo.Block.GetInventory());
            }

        }

        contents.Clear();
        cargoCounter++;
        yield return true;
    }

    NextStep();
    cargoCounter = 0;
}

public void NextStep()
{
    if (step < 2)
        step++;
    else
        step = 0;
}

public string GetType(MyInventoryItem thing)
{
    type = thing.Type.TypeId.Split('_')[1];
    if (translations.ContainsKey(type))
        translations.TryGetValue(type, out translatedType);
    else
        translatedType = type;
    return translatedType.ToString();
}

public string GetSubType(MyInventoryItem thing)
{
    subtype = thing.Type.SubtypeId;
    if (translations.ContainsKey(subtype))
        translations.TryGetValue(subtype, out translatedSubtype);
    else
        translatedSubtype = System.Text.RegularExpressions.Regex
            .Replace(thing.Type.SubtypeId, @"([A-Z])", @" $1").Trim();
    return translatedSubtype;
}

public IEnumerator<bool> UpdateAssemblers()
{
    foreach (var assembler in factory.Assemblers)
    {
        assembler.InputCapacity = (MyFixedPoint)(((float)assembler.Block.InputInventory.CurrentVolume /
                                                  (float)assembler.Block.InputInventory.MaxVolume) * 100);
        if (assembler.InputCapacity > 70)
        {
            contents.Clear();
            assembler.InputInventory.GetItems(contents);
            foreach (var thing in contents)
            {
                Sort(thing, assembler.InputInventory);
            }

            _stateMachine = null;
            Runtime.UpdateFrequency = UpdateFrequency.None;
            yield break;
        }

        contents.Clear();
        assembler.OutputInventory.GetItems(contents);
        foreach (var thing in contents)
        {
            Sort(thing, assembler.OutputInventory);
        }

        contents.Clear();
        assemblerCounter++;
        yield return true;
    }

    NextStep();
    assemblerCounter = 0;
}

public void AutoCraft()
{
    List<InventoryItem> toBeQueued = new List<InventoryItem>();
    //get the delta
    foreach (var item in factory.Inventory)
    {
        if (item.Quantity < item.Quantity)
        {
            //Queue the delta
            foreach (var assembler in factory.Assemblers.Where(a => a.Block.IsWorking))
            {
                if (factory.Assemblers.Count(a => a.Block.IsWorking) > 0)
                {
                    var delta = (float)(item.Quantity - item.Quantity).RawValue /
                                factory.Assemblers.Count(a => a.Block.IsWorking);
                    //assembler.Block.AddQueueItem(item.Blueprint, (MyFixedPoint)delta);
                }
            }
        }
    }

}

public IEnumerator<bool> UpdateInventory()
{
    foreach (var item in factory.Inventory)
    {
        foreach (var cargo in factory.Cargos)
        {

        }

        yield return true;
    }
}

public void Sort(MyInventoryItem thing, IMyInventory source)
{
    outputCargo = factory.Cargos.FirstOrDefault(c => c.Tags.Contains(GetType(thing)) && c.Capacity < 90);
    if (outputCargo != null)
    {
        destination = outputCargo.Block.GetInventory();
        destinationSpace = destination.MaxVolume * 1000 - destination.CurrentVolume * 1000;
        if (destinationSpace > thing.Amount)
        {
            if (!destination.TransferItemFrom(source, thing, destinationSpace))
            {
                Echo($"Failed to sort");

            }
            else
            {
                if (!destination.TransferItemFrom(source, thing, destinationSpace))
                {
                    Echo(
                        $"Failed to sort {GetSubType(thing)} into {factory.Blocks.First(b => b.EntityId.ToString() == source.Owner.Name)}");

                }
            }

        }
        else
        {
            Echo("Output Cargo is Null");
        }
    }
}

public class RunningSymbol
{
    char[] symbol;
    int index;
    public RunningSymbol()
    {
        symbol = new char[4] { '|', '/', '-', '\\' };
        index = 0;
    }
    public string Get()
    {
        index++;
        if (index > 3) { index = 0; }
        return symbol[index].ToString();
    }
}

internal class ShieldApi
{
    private IMyTerminalBlock _block;

    private readonly Func<IMyTerminalBlock, RayD, Vector3D?> _rayIntersectShield;
    private readonly Func<IMyTerminalBlock, LineD, Vector3D?> _lineIntersectShield;
    private readonly Func<IMyTerminalBlock, Vector3D, bool> _pointInShield;
    private readonly Func<IMyTerminalBlock, float> _getShieldPercent;
    private readonly Func<IMyTerminalBlock, int> _getShieldHeat;
    private readonly Func<IMyTerminalBlock, float> _getChargeRate;
    private readonly Func<IMyTerminalBlock, int> _hpToChargeRatio;
    private readonly Func<IMyTerminalBlock, float> _getMaxCharge;
    private readonly Func<IMyTerminalBlock, float> _getCharge;
    private readonly Func<IMyTerminalBlock, float> _getPowerUsed;
    private readonly Func<IMyTerminalBlock, float> _getPowerCap;
    private readonly Func<IMyTerminalBlock, float> _getMaxHpCap;
    private readonly Func<IMyTerminalBlock, bool> _isShieldUp;
    private readonly Func<IMyTerminalBlock, string> _shieldStatus;
    private readonly Func<IMyTerminalBlock, IMyEntity, bool, bool> _entityBypass;
    private readonly Func<IMyTerminalBlock, long, bool, bool> _entityBypassPb;
    // Fields below do not require SetActiveShield to be defined first.
    private readonly Func<IMyCubeGrid, bool> _gridHasShield;
    private readonly Func<IMyCubeGrid, bool> _gridShieldOnline;
    private readonly Func<IMyEntity, bool> _protectedByShield;
    private readonly Func<IMyEntity, IMyTerminalBlock> _getShieldBlock;
    private readonly Func<IMyTerminalBlock, bool> _isShieldBlock;
    private readonly Func<Vector3D, IMyTerminalBlock> _getClosestShield;
    private readonly Func<IMyTerminalBlock, Vector3D, double> _getDistanceToShield;
    private readonly Func<IMyTerminalBlock, Vector3D, Vector3D?> _getClosestShieldPoint;

    public void SetActiveShield(IMyTerminalBlock block) => _block = block; // AutoSet to TapiFrontend(block) if shield exists on grid.

    public ShieldApi(IMyTerminalBlock block)
    {
        _block = block;
        var delegates = _block.GetProperty("DefenseSystemsPbAPI")?.As<Dictionary<string, Delegate>>().GetValue(_block);
        if (delegates == null) return;

        _rayIntersectShield = (Func<IMyTerminalBlock, RayD, Vector3D?>)delegates["RayIntersectShield"];
        _lineIntersectShield = (Func<IMyTerminalBlock, LineD, Vector3D?>)delegates["LineIntersectShield"];
        _pointInShield = (Func<IMyTerminalBlock, Vector3D, bool>)delegates["PointInShield"];
        _getShieldPercent = (Func<IMyTerminalBlock, float>)delegates["GetShieldPercent"];
        _getShieldHeat = (Func<IMyTerminalBlock, int>)delegates["GetShieldHeat"];
        _getChargeRate = (Func<IMyTerminalBlock, float>)delegates["GetChargeRate"];
        _hpToChargeRatio = (Func<IMyTerminalBlock, int>)delegates["HpToChargeRatio"];
        _getMaxCharge = (Func<IMyTerminalBlock, float>)delegates["GetMaxCharge"];
        _getCharge = (Func<IMyTerminalBlock, float>)delegates["GetCharge"];
        _getPowerUsed = (Func<IMyTerminalBlock, float>)delegates["GetPowerUsed"];
        _getPowerCap = (Func<IMyTerminalBlock, float>)delegates["GetPowerCap"];
        _getMaxHpCap = (Func<IMyTerminalBlock, float>)delegates["GetMaxHpCap"];
        _isShieldUp = (Func<IMyTerminalBlock, bool>)delegates["IsShieldUp"];
        _shieldStatus = (Func<IMyTerminalBlock, string>)delegates["ShieldStatus"];
        _entityBypass = (Func<IMyTerminalBlock, IMyEntity, bool, bool>)delegates["EntityBypass"];
        _entityBypassPb = (Func<IMyTerminalBlock, long, bool, bool>)delegates["EntityBypassPb"];
        _gridHasShield = (Func<IMyCubeGrid, bool>)delegates["GridHasShield"];
        _gridShieldOnline = (Func<IMyCubeGrid, bool>)delegates["GridShieldOnline"];
        _protectedByShield = (Func<IMyEntity, bool>)delegates["ProtectedByShield"];
        _getShieldBlock = (Func<IMyEntity, IMyTerminalBlock>)delegates["GetShieldBlock"];
        _isShieldBlock = (Func<IMyTerminalBlock, bool>)delegates["IsShieldBlock"];
        _getClosestShield = (Func<Vector3D, IMyTerminalBlock>)delegates["GetClosestShield"];
        _getDistanceToShield = (Func<IMyTerminalBlock, Vector3D, double>)delegates["GetDistanceToShield"];
        _getClosestShieldPoint = (Func<IMyTerminalBlock, Vector3D, Vector3D?>)delegates["GetClosestShieldPoint"];

        if (!IsShieldBlock()) _block = GetShieldBlock(_block.CubeGrid) ?? _block;
    }
    public Vector3D? RayIntersectShield(RayD ray) => _rayIntersectShield?.Invoke(_block, ray) ?? null;
    public Vector3D? LineIntersectShield(LineD line) => _lineIntersectShield?.Invoke(_block, line) ?? null;
    public bool PointInShield(Vector3D pos) => _pointInShield?.Invoke(_block, pos) ?? false;
    public float GetShieldPercent() => _getShieldPercent?.Invoke(_block) ?? -1;
    public int GetShieldHeat() => _getShieldHeat?.Invoke(_block) ?? -1;
    public float GetChargeRate() => _getChargeRate?.Invoke(_block) ?? -1;
    public float HpToChargeRatio() => _hpToChargeRatio?.Invoke(_block) ?? -1;
    public float GetMaxCharge() => _getMaxCharge?.Invoke(_block) ?? -1;
    public float GetCharge() => _getCharge?.Invoke(_block) ?? -1;
    public float GetPowerUsed() => _getPowerUsed?.Invoke(_block) ?? -1;
    public float GetPowerCap() => _getPowerCap?.Invoke(_block) ?? -1;
    public float GetMaxHpCap() => _getMaxHpCap?.Invoke(_block) ?? -1;
    public bool IsShieldUp() => _isShieldUp?.Invoke(_block) ?? false;
    public string ShieldStatus() => _shieldStatus?.Invoke(_block) ?? string.Empty;
    public bool EntityBypass(IMyEntity entity, bool remove = false) => _entityBypass?.Invoke(_block, entity, remove) ?? false;
    public bool EntityBypassPb(long entity, bool remove = false) => _entityBypassPb?.Invoke(_block, entity, remove) ?? false;
    public bool GridHasShield(IMyCubeGrid grid) => _gridHasShield?.Invoke(grid) ?? false;
    public bool GridShieldOnline(IMyCubeGrid grid) => _gridShieldOnline?.Invoke(grid) ?? false;
    public bool ProtectedByShield(IMyEntity entity) => _protectedByShield?.Invoke(entity) ?? false;
    public IMyTerminalBlock GetShieldBlock(IMyEntity entity) => _getShieldBlock?.Invoke(entity) ?? null;
    public bool IsShieldBlock() => _isShieldBlock?.Invoke(_block) ?? false;
    public IMyTerminalBlock GetClosestShield(Vector3D pos) => _getClosestShield?.Invoke(pos) ?? null;
    public double GetDistanceToShield(Vector3D pos) => _getDistanceToShield?.Invoke(_block, pos) ?? -1;
    public Vector3D? GetClosestShieldPoint(Vector3D pos) => _getClosestShieldPoint?.Invoke(_block, pos) ?? null;
}

/// <summary>
        /// https://github.com/sstixrud/WeaponCore/blob/master/Data/Scripts/WeaponCore/Api/WeaponCorePbApi.cs
        /// </summary>
public class WcPbApi
{
    private Action<ICollection<MyDefinitionId>> _getCoreWeapons;
    private Action<ICollection<MyDefinitionId>> _getCoreStaticLaunchers;
    private Action<ICollection<MyDefinitionId>> _getCoreTurrets;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, IDictionary<string, int>, bool> _getBlockWeaponMap;
    private Func<long, MyTuple<bool, int, int>> _getProjectilesLockedOn;

    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, IDictionary<MyDetectedEntityInfo, float>>
        _getSortedThreats;

    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock,
        ICollection<Sandbox.ModAPI.Ingame.MyDetectedEntityInfo>> _getObstructions;

    private Func<long, int, MyDetectedEntityInfo> _getAiFocus;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, bool> _setAiFocus;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, MyDetectedEntityInfo> _getWeaponTarget;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int> _setWeaponTarget;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, bool, int> _fireWeaponOnce;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, bool, bool, int> _toggleWeaponFire;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, bool, bool, bool> _isWeaponReadyToFire;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, float> _getMaxWeaponRange;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, ICollection<string>, int, bool> _getTurretTargetTypes;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, ICollection<string>, int> _setTurretTargetTypes;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, float> _setBlockTrackingRange;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, bool> _isTargetAligned;

    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, MyTuple<bool, Vector3D?>>
        _isTargetAlignedExtended;

    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, bool> _canShootTarget;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, int, Vector3D?> _getPredictedTargetPos;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, float> _getHeatLevel;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, float> _currentPowerConsumption;
    private Func<MyDefinitionId, float> _getMaxPower;
    private Func<long, bool> _hasGridAi;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, bool> _hasCoreWeapon;
    private Func<long, float> _getOptimalDps;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, string> _getActiveAmmo;
    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, string> _setActiveAmmo;

    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Action<long, int, ulong, long, Vector3D, bool>>
        _monitorProjectile;

    private Action<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Action<long, int, ulong, long, Vector3D, bool>>
        _unMonitorProjectile;

    private Func<ulong, MyTuple<Vector3D, Vector3D, float, float, long, string>> _getProjectileState;
    private Func<long, float> _getConstructEffectiveDps;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long> _getPlayerController;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Matrix> _getWeaponAzimuthMatrix;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Matrix> _getWeaponElevationMatrix;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, bool, bool, bool> _isTargetValid;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, MyTuple<Vector3D, Vector3D>> _getWeaponScope;
    private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, MyTuple<bool, bool>> _isInRange;

    public bool Activate(Sandbox.ModAPI.Ingame.IMyTerminalBlock pbBlock)
    {
        var dict = pbBlock.GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<string, Delegate>>()
            .GetValue(pbBlock);
        if (dict == null) throw new Exception("WcPbAPI failed to activate");
        return ApiAssign(dict);
    }

    public bool ApiAssign(IReadOnlyDictionary<string, Delegate> delegates)
    {
        if (delegates == null)
            return false;

        AssignMethod(delegates, "GetCoreWeapons", ref _getCoreWeapons);
        AssignMethod(delegates, "GetCoreStaticLaunchers", ref _getCoreStaticLaunchers);
        AssignMethod(delegates, "GetCoreTurrets", ref _getCoreTurrets);
        AssignMethod(delegates, "GetBlockWeaponMap", ref _getBlockWeaponMap);
        AssignMethod(delegates, "GetProjectilesLockedOn", ref _getProjectilesLockedOn);
        AssignMethod(delegates, "GetSortedThreats", ref _getSortedThreats);
        AssignMethod(delegates, "GetObstructions", ref _getObstructions);
        AssignMethod(delegates, "GetAiFocus", ref _getAiFocus);
        AssignMethod(delegates, "SetAiFocus", ref _setAiFocus);
        AssignMethod(delegates, "GetWeaponTarget", ref _getWeaponTarget);
        AssignMethod(delegates, "SetWeaponTarget", ref _setWeaponTarget);
        AssignMethod(delegates, "FireWeaponOnce", ref _fireWeaponOnce);
        AssignMethod(delegates, "ToggleWeaponFire", ref _toggleWeaponFire);
        AssignMethod(delegates, "IsWeaponReadyToFire", ref _isWeaponReadyToFire);
        AssignMethod(delegates, "GetMaxWeaponRange", ref _getMaxWeaponRange);
        AssignMethod(delegates, "GetTurretTargetTypes", ref _getTurretTargetTypes);
        AssignMethod(delegates, "SetTurretTargetTypes", ref _setTurretTargetTypes);
        AssignMethod(delegates, "SetBlockTrackingRange", ref _setBlockTrackingRange);
        AssignMethod(delegates, "IsTargetAligned", ref _isTargetAligned);
        AssignMethod(delegates, "IsTargetAlignedExtended", ref _isTargetAlignedExtended);
        AssignMethod(delegates, "CanShootTarget", ref _canShootTarget);
        AssignMethod(delegates, "GetPredictedTargetPosition", ref _getPredictedTargetPos);
        AssignMethod(delegates, "GetHeatLevel", ref _getHeatLevel);
        AssignMethod(delegates, "GetCurrentPower", ref _currentPowerConsumption);
        AssignMethod(delegates, "GetMaxPower", ref _getMaxPower);
        AssignMethod(delegates, "HasGridAi", ref _hasGridAi);
        AssignMethod(delegates, "HasCoreWeapon", ref _hasCoreWeapon);
        AssignMethod(delegates, "GetOptimalDps", ref _getOptimalDps);
        AssignMethod(delegates, "GetActiveAmmo", ref _getActiveAmmo);
        AssignMethod(delegates, "SetActiveAmmo", ref _setActiveAmmo);
        AssignMethod(delegates, "MonitorProjectile", ref _monitorProjectile);
        AssignMethod(delegates, "UnMonitorProjectile", ref _unMonitorProjectile);
        AssignMethod(delegates, "GetProjectileState", ref _getProjectileState);
        AssignMethod(delegates, "GetConstructEffectiveDps", ref _getConstructEffectiveDps);
        AssignMethod(delegates, "GetPlayerController", ref _getPlayerController);
        AssignMethod(delegates, "GetWeaponAzimuthMatrix", ref _getWeaponAzimuthMatrix);
        AssignMethod(delegates, "GetWeaponElevationMatrix", ref _getWeaponElevationMatrix);
        AssignMethod(delegates, "IsTargetValid", ref _isTargetValid);
        AssignMethod(delegates, "GetWeaponScope", ref _getWeaponScope);
        AssignMethod(delegates, "IsInRange", ref _isInRange);
        return true;
    }

    private void AssignMethod<T>(IReadOnlyDictionary<string, Delegate> delegates, string name, ref T field)
        where T : class
    {
        if (delegates == null)
        {
            field = null;
            return;
        }

        Delegate del;
        if (!delegates.TryGetValue(name, out del))
            throw new Exception($"{GetType().Name} :: Couldn't find {name} delegate of type {typeof(T)}");

        field = del as T;
        if (field == null)
            throw new Exception(
                $"{GetType().Name} :: Delegate {name} is not type {typeof(T)}, instead it's: {del.GetType()}");
    }

    public void GetAllCoreWeapons(ICollection<MyDefinitionId> collection) =>
        _getCoreWeapons?.Invoke(collection);

    public void GetAllCoreStaticLaunchers(ICollection<MyDefinitionId> collection) =>
        _getCoreStaticLaunchers?.Invoke(collection);

    public void GetAllCoreTurrets(ICollection<MyDefinitionId> collection) =>
        _getCoreTurrets?.Invoke(collection);

    public bool GetBlockWeaponMap(Sandbox.ModAPI.Ingame.IMyTerminalBlock weaponBlock,
        IDictionary<string, int> collection) =>
        _getBlockWeaponMap?.Invoke(weaponBlock, collection) ?? false;

    public MyTuple<bool, int, int> GetProjectilesLockedOn(long victim) =>
        _getProjectilesLockedOn?.Invoke(victim) ?? new MyTuple<bool, int, int>();

    public void GetSortedThreats(Sandbox.ModAPI.Ingame.IMyTerminalBlock pBlock,
        IDictionary<MyDetectedEntityInfo, float> collection) =>
        _getSortedThreats?.Invoke(pBlock, collection);

    public void GetObstructions(Sandbox.ModAPI.Ingame.IMyTerminalBlock pBlock,
        ICollection<Sandbox.ModAPI.Ingame.MyDetectedEntityInfo> collection) =>
        _getObstructions?.Invoke(pBlock, collection);

    public MyDetectedEntityInfo? GetAiFocus(long shooter, int priority = 0) =>
        _getAiFocus?.Invoke(shooter, priority);

    public bool SetAiFocus(Sandbox.ModAPI.Ingame.IMyTerminalBlock pBlock, long target, int priority = 0) =>
        _setAiFocus?.Invoke(pBlock, target, priority) ?? false;

    public MyDetectedEntityInfo? GetWeaponTarget(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon,
        int weaponId = 0) =>
        _getWeaponTarget?.Invoke(weapon, weaponId);

    public void SetWeaponTarget(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long target, int weaponId = 0) =>
        _setWeaponTarget?.Invoke(weapon, target, weaponId);

    public void FireWeaponOnce(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, bool allWeapons = true,
        int weaponId = 0) =>
        _fireWeaponOnce?.Invoke(weapon, allWeapons, weaponId);

    public void ToggleWeaponFire(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, bool on, bool allWeapons,
        int weaponId = 0) =>
        _toggleWeaponFire?.Invoke(weapon, on, allWeapons, weaponId);

    public bool IsWeaponReadyToFire(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId = 0,
        bool anyWeaponReady = true,
        bool shootReady = false) =>
        _isWeaponReadyToFire?.Invoke(weapon, weaponId, anyWeaponReady, shootReady) ?? false;

    public float GetMaxWeaponRange(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getMaxWeaponRange?.Invoke(weapon, weaponId) ?? 0f;

    public bool GetTurretTargetTypes(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, IList<string> collection,
        int weaponId = 0) =>
        _getTurretTargetTypes?.Invoke(weapon, collection, weaponId) ?? false;

    public void SetTurretTargetTypes(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, IList<string> collection,
        int weaponId = 0) =>
        _setTurretTargetTypes?.Invoke(weapon, collection, weaponId);

    public void SetBlockTrackingRange(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, float range) =>
        _setBlockTrackingRange?.Invoke(weapon, range);

    public bool IsTargetAligned(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
        _isTargetAligned?.Invoke(weapon, targetEnt, weaponId) ?? false;

    public MyTuple<bool, Vector3D?> IsTargetAlignedExtended(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon,
        long targetEnt, int weaponId) =>
        _isTargetAlignedExtended?.Invoke(weapon, targetEnt, weaponId) ?? new MyTuple<bool, Vector3D?>();

    public bool CanShootTarget(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
        _canShootTarget?.Invoke(weapon, targetEnt, weaponId) ?? false;

    public Vector3D? GetPredictedTargetPosition(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetEnt,
        int weaponId) =>
        _getPredictedTargetPos?.Invoke(weapon, targetEnt, weaponId) ?? null;

    public float GetHeatLevel(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) =>
        _getHeatLevel?.Invoke(weapon) ?? 0f;

    public float GetCurrentPower(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) =>
        _currentPowerConsumption?.Invoke(weapon) ?? 0f;

    public float GetMaxPower(MyDefinitionId weaponDef) => _getMaxPower?.Invoke(weaponDef) ?? 0f;
    public bool HasGridAi(long entity) => _hasGridAi?.Invoke(entity) ?? false;

    public bool HasCoreWeapon(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) =>
        _hasCoreWeapon?.Invoke(weapon) ?? false;

    public float GetOptimalDps(long entity) => _getOptimalDps?.Invoke(entity) ?? 0f;

    public string GetActiveAmmo(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getActiveAmmo?.Invoke(weapon, weaponId) ?? null;

    public void SetActiveAmmo(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId, string ammoType) =>
        _setActiveAmmo?.Invoke(weapon, weaponId, ammoType);

    public void MonitorProjectileCallback(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId,
        Action<long, int, ulong, long, Vector3D, bool> action) =>
        _monitorProjectile?.Invoke(weapon, weaponId, action);

    public void UnMonitorProjectileCallback(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId,
        Action<long, int, ulong, long, Vector3D, bool> action) =>
        _unMonitorProjectile?.Invoke(weapon, weaponId, action);

    public MyTuple<Vector3D, Vector3D, float, float, long, string> GetProjectileState(ulong projectileId) =>
        _getProjectileState?.Invoke(projectileId) ??
        new MyTuple<Vector3D, Vector3D, float, float, long, string>();

    public float GetConstructEffectiveDps(long entity) => _getConstructEffectiveDps?.Invoke(entity) ?? 0f;

    public long GetPlayerController(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon) =>
        _getPlayerController?.Invoke(weapon) ?? -1;

    public Matrix GetWeaponAzimuthMatrix(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getWeaponAzimuthMatrix?.Invoke(weapon, weaponId) ?? Matrix.Zero;

    public Matrix GetWeaponElevationMatrix(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
        _getWeaponElevationMatrix?.Invoke(weapon, weaponId) ?? Matrix.Zero;

    public bool IsTargetValid(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetId, bool onlyThreats,
        bool checkRelations) =>
        _isTargetValid?.Invoke(weapon, targetId, onlyThreats, checkRelations) ?? false;

    public MyTuple<Vector3D, Vector3D> GetWeaponScope(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon,
        int weaponId) =>
        _getWeaponScope?.Invoke(weapon, weaponId) ?? new MyTuple<Vector3D, Vector3D>();

    // terminalBlock, Threat, Other, Something
    public MyTuple<bool, bool> IsInRange(Sandbox.ModAPI.Ingame.IMyTerminalBlock block) =>
        _isInRange?.Invoke(block) ?? new MyTuple<bool, bool>();
}

}
class Assembler
{
    public MyFixedPoint InputCapacity { get; set; }
    public IMyInventory InputInventory { get; set; }
    public IMyInventory OutputInventory { get; set; }
    public IMyProductionBlock Block { get; set; }
}

class Cargo
{
    public IMyTerminalBlock Block { get; set; }
    public List<string> Tags { get; set; }
    public MyFixedPoint Capacity { get; set; }
}

class Factory
{
    public List<Assembler> Assemblers { get; set; }
    public List<IMyTerminalBlock> Blocks { get; set; }
    public List<Cargo> Cargos { get; set; }
    public List<IMyShipConnector> Connectors { get; set; }
    public List<InventoryItem> Inventory { get; set; }
    public List<IMyReactor> Reactors { get; set; }
    public List<IMyRefinery> Refineries { get; set; }
}

class InventoryItem
{
    public string Name { get; set; }
    public MyFixedPoint Quantity { get; set; }
    public string Type { get; set; }

}

class PID
{
    double _kP = 0;
    double _kI = 0;
    double _kD = 0;
    double _integralDecayRatio = 0;
    double _lowerBound = 0;
    double _upperBound = 0;
    double _timeStep = 0;
    double _inverseTimeStep = 0;
    double _errorSum = 0;
    double _lastError = 0;
    bool _firstRun = true;
    bool _integralDecay = false;
    public double Value { get; private set; }

    public PID(double kP, double kI, double kD, double lowerBound, double upperBound, double timeStep)
    {
        _kP = kP;
        _kI = kI;
        _kD = kD;
        _lowerBound = lowerBound;
        _upperBound = upperBound;
        _timeStep = timeStep;
        _inverseTimeStep = 1 / _timeStep;
        _integralDecay = false;
    }

    public PID(double kP, double kI, double kD, double integralDecayRatio, double timeStep)
    {
        _kP = kP;
        _kI = kI;
        _kD = kD;
        _timeStep = timeStep;
        _inverseTimeStep = 1 / _timeStep;
        _integralDecayRatio = integralDecayRatio;
        _integralDecay = true;
    }

    public double Control(double error)
    {
        //Compute derivative term
        var errorDerivative = (error - _lastError) * _inverseTimeStep;

        if (_firstRun)
        {
            errorDerivative = 0;
            _firstRun = false;
        }

        //Compute integral term
        if (!_integralDecay)
        {
            _errorSum += error * _timeStep;

            //Clamp integral term
            if (_errorSum > _upperBound)
                _errorSum = _upperBound;
            else if (_errorSum < _lowerBound)
                _errorSum = _lowerBound;
        }
        else
        {
            _errorSum = _errorSum * (1.0 - _integralDecayRatio) + error * _timeStep;
        }

        //Store this error as last error
        _lastError = error;

        //Construct output
        this.Value = _kP * error + _kI * _errorSum + _kD * errorDerivative;
        return this.Value;
    }

    public double Control(double error, double timeStep)
    {
        _timeStep = timeStep;
        _inverseTimeStep = 1 / _timeStep;
        return Control(error);
    }

    public void Reset()
    {
        _errorSum = 0;
        _lastError = 0;
        _firstRun = true;
    }
}

public static class VectorHelpers
{
    /// <summary>
        ///  Normalizes a vector only if it is non-zero and non-unit
        /// </summary>
    public static Vector3D SafeNormalize(Vector3D a)
    {
        if (Vector3D.IsZero(a))
            return Vector3D.Zero;

        if (Vector3D.IsUnit(ref a))
            return a;

        return Vector3D.Normalize(a);
    }

    /// <summary>
        /// Reflects vector a over vector b with an optional rejection factor
        /// </summary>
    public static Vector3D Reflection(Vector3D a, Vector3D b, double rejectionFactor = 1) //reflect a over b
    {
        Vector3D project_a = Projection(a, b);
        Vector3D reject_a = a - project_a;
        return project_a - reject_a * rejectionFactor;
    }

    /// <summary>
        /// Rejects vector a on vector b
        /// </summary>
    public static Vector3D Rejection(Vector3D a, Vector3D b) //reject a on b
    {
        if (Vector3D.IsZero(a) || Vector3D.IsZero(b))
            return Vector3D.Zero;

        return a - a.Dot(b) / b.LengthSquared() * b;
    }

    /// <summary>
        /// Projects vector a onto vector b
        /// </summary>
    public static Vector3D Projection(Vector3D a, Vector3D b)
    {
        if (Vector3D.IsZero(a) || Vector3D.IsZero(b))
            return Vector3D.Zero;

        if (Vector3D.IsUnit(ref b))
            return a.Dot(b) * b;

        return a.Dot(b) / b.LengthSquared() * b;
    }

    /// <summary>
        /// Scalar projection of a onto b
        /// </summary>
    public static double ScalarProjection(Vector3D a, Vector3D b)
    {
        if (Vector3D.IsZero(a) || Vector3D.IsZero(b))
            return 0;

        if (Vector3D.IsUnit(ref b))
            return a.Dot(b);

        return a.Dot(b) / b.Length();
    }

    /// <summary>
        /// Computes angle between 2 vectors in radians.
        /// </summary>
    public static double AngleBetween(Vector3D a, Vector3D b)
    {
        if (Vector3D.IsZero(a) || Vector3D.IsZero(b))
            return 0;
        else
            return Math.Acos(MathHelper.Clamp(a.Dot(b) / Math.Sqrt(a.LengthSquared() * b.LengthSquared()), -1, 1));
    }

    /// <summary>
        /// Computes cosine of the angle between 2 vectors.
        /// </summary>
    public static double CosBetween(Vector3D a, Vector3D b)
    {
        if (Vector3D.IsZero(a) || Vector3D.IsZero(b))
            return 0;
        else
            return MathHelper.Clamp(a.Dot(b) / Math.Sqrt(a.LengthSquared() * b.LengthSquared()), -1, 1);
    }

    /// <summary>
        /// Returns if the normalized dot product between two vectors is greater than the tolerance.
        /// This is helpful for determining if two vectors are "more parallel" than the tolerance.
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="tolerance">Cosine of maximum angle</param>
        /// <returns></returns>
    public static bool IsDotProductWithinTolerance(Vector3D a, Vector3D b, double tolerance)
    {
        double dot = Vector3D.Dot(a, b);
        double num = a.LengthSquared() * b.LengthSquared() * tolerance * Math.Abs(tolerance);
        return Math.Abs(dot) * dot > num;
    }