/*
 * Trailer Manager
 * ===============
 * 
 * There is no configuration to edit in this script. Configuration is set by means
 * of the Custom Data field in the trailer's hinges, the programmable block or in
 * any terminal block with a screen.
 * 
 * Hinge example:
 * --------------
 * 
 * [trailer]
 * front=true
 * name=Trailer Name
 * 
 * At least one hinge must have front set to "true". This is the hinge at the
 * front, by which the trailer is towed. If there is a hinge at the rear, change
 * front to "false" in that hinge's Custom Data. If there is a non-hitch hinge,
 * don't set this value.
 * 
 * The name of the trailer defaults to the name of the grid, if the name line is
 * missing.
 * 
 * Screen/Cockpit example:
 * -----------------------
 * 
 * [trailer]
 * display=0
 * scale=0.5
 * color=FF4500
 * 
 * Change the number to the screen that you wish to use; numbers start at 0, so a
 * five-screen cockpit has screens 0, 1, 2, 3 and 4.
 * 
 * Scale is a scaling factor; adjust this if the font size is wrong.
 * Color is optional, and allows the highlight color to be customised.
 * 
 * Programmable block example:
 * ---------------------------
 * 
 * [trailer]
 * autodeploy=true
 * mirror=true
 * 
 * Auto-deploy can be turned off by setting autodeploy to "false". This will stop
 * the script attempting to trigger the unpack or deploy function after a trailer
 * has been disconnected. Change to false if you don't want this sort of magic.
 * 
 * Mirroring causes the trailers' batteries, hydrogen tanks, engines & generators,
 * and parking brakes to mirror those in use on the towing vehicle. 
 * 
 * Timer example:
 * --------------
 * 
 * [trailer]
 * task=stow
 * 
 * The task setting can be "stow" or "pack", in which case it will be used to stow
 * a trailer for travel. It can also be set to "deploy" or "unpack", in which case
 * it will be used to deploy a travel for separate use in-position. If not set, or
 * if set to some other value, it will be treated like any other timer: its name
 * will be shown in the trailer's menu, where it can be triggered.
 * 
 * If you have a timer that is used to both deploy and stow a trailer, set task to
 * "toggle".
 * 
 * Arguments
 * ---------
 * 
 * brakes on      Apply handbrakes
 * brakes off     Release handbrakes
 * deploy         Deploy / Unpack trailer at rear
 * unpack           "
 * allpack	       Pack all trailers for travel
 * detach         Detach the rear-most trailer
 * hitch          Couple a new trailer to the rear
 * attach           "
 * connector      Switch connectors on rear trailer
 * weapons on     Activate turrets
 * weapons off    De-activate turrets
 * rebuild        Check trailer consist for changes
 * LegacyUpdate   Attempt to identify trailers (ISL)
 * 
 * up             Move up one line on screen
 * down           Move down one line on screen
 * apply          Select current line on screen
 * select           "
 * back           Go back one menu screen
 */

const string Version = "1.0.10";
List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
List<IMyMotorAdvancedStator> Hinges = new List<IMyMotorAdvancedStator>();
List<IMyAttachableTopBlock> HingeParts = new List<IMyAttachableTopBlock>();
Dictionary<IMyCubeGrid, Trailer> Trailers = new Dictionary<IMyCubeGrid, Trailer>();
Dictionary<IMyCubeGrid, Coupling> Couplings = new Dictionary<IMyCubeGrid, Coupling>();
List<Trailer> Consist = new List<Trailer>();
List<IMyCubeGrid> GridsFound = new List<IMyCubeGrid>();
const string Section = "trailer";
MyIni ini = new MyIni();
Trailer FirstTrailer;
List<ManagedDisplay> Displays = new List<ManagedDisplay>();
int selectedline = 0; // Menu selection position
Trailer selectedtrailer; // Selected trailer in menu (to recalculate selectedline in the event of a rebuild)
enum MenuOption { Top, AllTrailers, AllBatteries, AllHydrogen, Trailer, Config };
enum TimerTask { Menu, Stow, Deploy, Toggle };
MenuOption SelectedMenu = MenuOption.Top;
List<MenuItem> TopMenu = new List<MenuItem>();
List<MenuItem> AllTrailersMenu = new List<MenuItem>();
List<MenuItem> AllBatteriesMenu = new List<MenuItem>();
List<MenuItem> AllHydrogenMenu = new List<MenuItem>();
List<MenuItem> ConfigurationMenu = new List<MenuItem>();
IMyMotorAdvancedStator TractorHitch;
private bool UnidentifiedTrailer;

// Config settings
internal bool CfgAutoDeploy = true;
internal bool CfgMirror = true;

// Lists used for the mirror feature
private List<IMyBatteryBlock> Batteries = new List<IMyBatteryBlock>();
private List<IMyPowerProducer> Engines = new List<IMyPowerProducer>();
private List<IMyGasTank> HTanks = new List<IMyGasTank>();
private List<IMyGasGenerator> HGens = new List<IMyGasGenerator>();
private List<IMyUserControllableGun> Weapons = new List<IMyUserControllableGun>();
private IMyShipController Controller;

// Previous states for mirror feature (prevents continually applying changes)
ChargeMode PreviousChargeMode = ChargeMode.Auto;
bool PreviousBatteryEnabled = true;
bool PreviousStockpile = false;
bool PreviousGenerator = true;
bool PreviousEngines = true;
bool PreviousWeapons = false;
bool PreviousHandbrake = false;

public void ClearMirrorLists()
{
    Batteries.Clear();
    Engines.Clear();
    HTanks.Clear();
    HGens.Clear();
    Weapons.Clear();
    Controller = null;
}

// Methods for identifying the hydrogen blocks which lack unique interfaces.
// Many thanks to Vox Serico for these methods.

readonly MyDefinitionId
    _hydrogenEngineId = MyDefinitionId.Parse("MyObjectBuilder_HydrogenEngine/"),
    _hydrogenGasId = MyDefinitionId.Parse("MyObjectBuilder_GasProperties/Hydrogen"),
    _oxygenTankId = MyDefinitionId.Parse("MyObjectBuilder_OxygenTank/");

bool IsHydrogenEngine(IMyTerminalBlock block)
{
    return IsHydrogenEngine(block.BlockDefinition);
}
bool IsHydrogenEngine(MyDefinitionId blockId)
{
    return blockId.TypeId == _hydrogenEngineId.TypeId;
}
bool IsHydrogenTank(IMyTerminalBlock block)
{
    if (block.BlockDefinition.TypeId != _oxygenTankId.TypeId)
        return false;

    var resourceSink = block.Components.Get<MyResourceSinkComponent>();
    return resourceSink != null && resourceSink.AcceptedResources.Contains(_hydrogenGasId);
}

struct MenuItem
{
    public string Sprite;
    public float SpriteRotation;
    public Color SpriteColor;
    public Color TextColor;
    public string MenuText;
    public Action Action;
}

struct Feedback
{
    public string Message;
    public string Sprite;
    public float SpriteRotation;
    public Color BackgroundColor;
    public Color TextColor;
    public int duration;
}

public void AllTrailersBatteryCharge(ChargeMode chargeMode)
{
    foreach (var trailer in Consist)
        trailer.SetBatteryChargeMode(chargeMode);
}
public void AllTrailersDisableBattery()
{
    foreach (var trailer in Consist)
        trailer.DisableBattery();
}
public void AllTrailersHydrogenStockpileOn()
{
    foreach (var trailer in Consist)
        trailer.HydrogenTankStockpileOn();
}
public void AllTrailersHydrogenStockpileOff()
{
    foreach (var trailer in Consist)
        trailer.HydrogenTankStockpileOff();
}
public void AllTrailersEnginesOff()
{
    foreach (var trailer in Consist)
        trailer.EnginesOff();
}
public void AllTrailersEnginesOn()
{
    foreach (var trailer in Consist)
        trailer.EnginesOn();
}
public void AllTrailersGasGeneratorsOn()
{
    foreach (var trailer in Consist)
        trailer.GeneratorsOn();
}
public void AllTrailersGasGeneratorsOff()
{
    foreach (var trailer in Consist)
        trailer.GeneratorsOff();
}
public void AllTrailersWheelsOff()
{
    foreach (var trailer in Consist)
        trailer.WheelsOff();
}
public void AllTrailersHandbrakeOn()
{
    foreach (var trailer in Consist)
        trailer.HandbrakeOn();
}
public void AllTrailersHandbrakeOff()
{
    foreach (var trailer in Consist)
        trailer.HandbrakeOff();
}
public void AllTrailersDeploy()
{
    foreach (var trailer in Consist)
        trailer.Deploy();
}
public void AllTrailersStow()
{
    foreach (var trailer in Consist)
        trailer.Stow();
}
public void DeployLastTrailer()
{
    if (Consist.Count > 0) Consist[Consist.Count - 1].Deploy();
}
public void SwitchRearConnector()
{
    if (Consist.Count > 0) Consist[Consist.Count - 1].SwitchConnector();
}
public void DetachLastTrailer()
{
    if (Consist.Count > 0) Consist[Consist.Count - 1].Detach();
}
public void AttachLastTrailer()
{
    if (Consist.Count > 0)
        Consist[Consist.Count - 1].Attach();
    else
        if (null != TractorHitch)
            TractorHitch.Attach();
}
public void AllTrailersWeaponsLive()
{
    foreach (var trailer in Consist)
        trailer.WeaponsLive();
}
public void AllTrailersWeaponsSafe()
{
    foreach (var trailer in Consist)
        trailer.WeaponsSafe();
}

public bool? StateToMirror(IEnumerable<IMyFunctionalBlock> blocks)
{
    bool? ReturnState = null;
    foreach (var block in blocks)
    {
        if (null == ReturnState)
            ReturnState = block.Enabled;
        if (block.Enabled != ReturnState.Value)
            return null;
    }
    return ReturnState;
}

public ChargeMode? ChargeToMirror(IEnumerable<IMyBatteryBlock> batteries)
{
    ChargeMode? ReturnState = null;
    foreach (var battery in batteries)
    {
        if (null == ReturnState)
            ReturnState = battery.ChargeMode;
        if (battery.ChargeMode != ReturnState)
            return null;
    }
    return ReturnState;
}

public bool? StockpileToMirror(IEnumerable<IMyGasTank> tanks)
{
    bool? ReturnState = null;
    foreach (var tank in tanks)
    {
        if (null == ReturnState)
            ReturnState = tank.Stockpile;
        if (tank.Stockpile != ReturnState)
            return null;
    }
    return ReturnState;
}

public void Mirror()
{
    bool? enabledState = StateToMirror(Batteries);
    if (enabledState.HasValue)
    {
        if (!enabledState.Value && PreviousBatteryEnabled)
        {
            AllTrailersDisableBattery();
        }
        ChargeMode? chargeMode = ChargeToMirror(Batteries);
        if ((enabledState.Value && !PreviousBatteryEnabled) || (chargeMode.HasValue && chargeMode != PreviousChargeMode))
        {
            AllTrailersBatteryCharge(chargeMode.Value);
            PreviousChargeMode = chargeMode.Value;
        }
        PreviousBatteryEnabled = enabledState.Value;
    }
    enabledState = StateToMirror(Weapons);
    if (enabledState.HasValue)
    {
        if (enabledState.Value != PreviousWeapons)
            if (enabledState.Value)
                AllTrailersWeaponsLive();
            else
                AllTrailersWeaponsSafe();
        PreviousWeapons = enabledState.Value;
    }
    enabledState = StateToMirror(Engines);
    if (enabledState.HasValue)
    {
        if (enabledState.Value != PreviousEngines)
            if (enabledState.Value)
                AllTrailersEnginesOn();
            else
                AllTrailersEnginesOff();
        PreviousEngines = enabledState.Value;
    }
    enabledState = StateToMirror(HGens);
    if (enabledState.HasValue)
    {
        if (enabledState.Value != PreviousGenerator)
            if (enabledState.Value)
                AllTrailersGasGeneratorsOn();
            else
                AllTrailersGasGeneratorsOff();
        PreviousGenerator = enabledState.Value;
    }
    enabledState = StockpileToMirror(HTanks);
    if (enabledState.HasValue)
    {
        if (enabledState.Value != PreviousStockpile)
            if (enabledState.Value)
                AllTrailersHydrogenStockpileOn();
            else
                AllTrailersHydrogenStockpileOff();
        PreviousStockpile = enabledState.Value;
    }
    if (null != Controller)
    {
        if (Controller.HandBrake && !PreviousHandbrake)
        {
            AllTrailersHandbrakeOn();
            PreviousHandbrake = true;
        }
        else if (!Controller.HandBrake && PreviousHandbrake)
        {
            AllTrailersHandbrakeOff();
            PreviousHandbrake = false;
        }
    }
}

public void LegacyUpdate()
{
    GridTerminalSystem.GetBlocksOfType(Blocks, block => block.IsSameConstructAs(Me) && ((block is IMyMotorAdvancedStator) || (block is IMyTimerBlock)));
    Hinges = Blocks.OfType<IMyMotorAdvancedStator>().ToList();
    List<IMyTimerBlock> Timers = Blocks.OfType<IMyTimerBlock>().ToList();
    foreach (var hinge in Hinges)
    {
        ini.Clear();
        ini.TryParse(hinge.CustomData);
        if (!MyIni.HasSection(hinge.CustomData, Section))
        {
            if (hinge.CubeGrid == Me.CubeGrid && hinge.CustomName.ToLower().Contains("hitch"))
            {
                this.TractorHitch = hinge;
                ini.Set(Section, "hitch", true);
            }
            else if (hinge.CustomName.Contains("Solar")|| hinge.CustomName.Contains("Ramp"))
            {
                // Definitely want to ignore these guys
                ;
            }
            else
            {
                // A new trailer has been identified, set a name and allow BuildAll() to run
                if ((hinge.CustomName.ToLower().EndsWith("front") || hinge.CustomName.ToLower().EndsWith("steering")))
                {
                    ini.Set(Section, "front", true);
                    ini.Set(Section, "name", hinge.CubeGrid.CustomName);
                    UnidentifiedTrailer = false;
                }
                else if (hinge.CustomName.ToLower().EndsWith("rear"))
                {
                    ini.Set(Section, "front", false);
                }
            }
            hinge.CustomData = ini.ToString();
        }
    }
    foreach (var timer in Timers)
    {
        ini.Clear();
        ini.TryParse(timer.CustomData);
        if (!MyIni.HasSection(timer.CustomData, Section))
        {
            if (timer.CustomName.ToLower().Contains("unpack"))
                ini.Set(Section, "task", "deploy");
            else if (timer.CustomName.ToLower().Contains("pack"))
                ini.Set(Section, "task", "stow");
            else if (timer.CustomName.ToLower().Contains("trailer") || timer.CustomName.ToLower().Contains("hook/unhook"))
                ini.Set(Section, "task", "toggle");
        }
        timer.CustomData = ini.ToString();
    }
    BuildAll();
}

private void BuildConsist()
{
    Trailers.Clear();
    Blocks.Clear();
    Hinges.Clear();
    HingeParts.Clear();
    ClearMirrorLists();
    FirstTrailer = null;
    // Populate Blocks, excluding hinges, which will instead populate either Trailers or Hinges
    GridTerminalSystem.GetBlocksOfType(Blocks, block => {
        if(!block.IsSameConstructAs(Me))
            return false;
        IMyMotorAdvancedStator hinge = block as IMyMotorAdvancedStator;
        if (null != hinge && MyIni.HasSection(hinge.CustomData, Section) && ini.TryParse(hinge.CustomData))
        {
            if (ini.Get(Section, "front").ToBoolean())
            {
                Trailers.Add(hinge.CubeGrid, new Trailer(this, hinge,
                    ini.Get(Section, "ignore_hingelock").ToBoolean(),
                    ini.Get(Section, "hinge_action").ToBoolean(true),
                    ini.Get(Section, "ignore_handbrake").ToBoolean(),
                    ini.Get(Section, "handbrake_action").ToBoolean(true)));
            }
            else
                Hinges.Add(hinge);
            if (hinge.IsAttached)
                HingeParts.Add(hinge.Top);
        }
        return true;
    });
    // Iterate remaining blocks, now that all the Trailers have been found
    // First the hinges
    foreach (var hinge in Hinges)
    {
        if (MyIni.HasSection(hinge.CustomData, Section) && ini.TryParse(hinge.CustomData))
        {
            if (ini.ContainsKey(Section, "front") && !ini.Get(Section, "front").ToBoolean())
            {
                if (Trailers.ContainsKey(hinge.CubeGrid))
                    Trailers[hinge.CubeGrid].RearHitch = hinge; // Regardless if already set
                if (hinge.CubeGrid.Equals(Me.CubeGrid))
                    TractorHitch = hinge;
            }
            else if (hinge.CubeGrid.Equals(Me.CubeGrid) && ini.Get(Section, "hitch").ToBoolean())
            {
                TractorHitch = hinge;
            }
            else if (Trailers.ContainsKey(hinge.CubeGrid) && Trailers[hinge.CubeGrid].RearHitch == null)
            {
                Trailers[hinge.CubeGrid].RearHitch = hinge;
            }
        }
    }

    // Now everything else
    foreach (var block in Blocks)
    {
        // Add battery to its trailer
        IMyBatteryBlock Battery = block as IMyBatteryBlock;
        if (null != Battery)
        {
            if (Me.CubeGrid == Battery.CubeGrid)
                Batteries.Add(Battery);
            else if (Trailers.ContainsKey(Battery.CubeGrid))
                Trailers[Battery.CubeGrid].AddBattery(Battery);
        }
        // Add wheel suspension to its trailer
        IMyMotorSuspension Wheel = block as IMyMotorSuspension;
        if (null != Wheel)
        {
            if (Trailers.ContainsKey(Wheel.CubeGrid))
                Trailers[Wheel.CubeGrid].AddWheel(Wheel);
        }
        // Add hydrogen engine to its trailer
        IMyPowerProducer Engine = block as IMyPowerProducer;
        if (null != Engine)
        {
            if (IsHydrogenEngine(Engine))
                if (Me.CubeGrid == Engine.CubeGrid)
                    Engines.Add(Engine);
                else if (Trailers.ContainsKey(Engine.CubeGrid))
                    Trailers[Engine.CubeGrid].AddEngine(Engine);
        }
        // Add hydrogen tank to its trailer
        IMyGasTank Tank = block as IMyGasTank;
        if (null != Tank)
        {
            if (IsHydrogenTank(Tank))
                if (Me.CubeGrid == Tank.CubeGrid)
                    HTanks.Add(Tank);
                else if (Trailers.ContainsKey(Tank.CubeGrid))
                    Trailers[Tank.CubeGrid].AddHTank(Tank);
        }
        // Add O2/H2 generator to its trailer
        IMyGasGenerator Gen = block as IMyGasGenerator;
        if (null != Gen)
        {
            if (Me.CubeGrid == Gen.CubeGrid)
                HGens.Add(Gen);
            else if (Trailers.ContainsKey(Gen.CubeGrid))
                Trailers[Gen.CubeGrid].AddHGen(Gen);
        }
        // Add weapon to its trailer
        IMyUserControllableGun Weapon = block as IMyUserControllableGun;
        if (null != Weapon)
        {
            if (Me.CubeGrid == Weapon.CubeGrid)
                Weapons.Add(Weapon);
            else if (Trailers.ContainsKey(Weapon.CubeGrid))
                Trailers[Weapon.CubeGrid].AddWeapon(Weapon);
        }
        // Add connector to its trailer
        IMyShipConnector Connector = block as IMyShipConnector;
        if (null != Connector)
        {
            if (Trailers.ContainsKey(Connector.CubeGrid))
                Trailers[Connector.CubeGrid].AddConnector(Connector);
        }
        // Add antenna to its trailer
        IMyRadioAntenna Antenna = block as IMyRadioAntenna;
        if (null != Antenna)
        {
            if (Trailers.ContainsKey(Antenna.CubeGrid))
                Trailers[Antenna.CubeGrid].AddAntenna(Antenna);
        }
        // Get a controller for the handbrake
        IMyShipController Controller = block as IMyShipController;
        if (null != Controller)
        {
            if (Controller.CanControlShip)
                if (Me.CubeGrid == Controller.CubeGrid)
                    this.Controller = Controller;
                else if (Trailers.ContainsKey(Controller.CubeGrid))
                {
                    Trailers[Controller.CubeGrid].AddController(Controller);
                }
        }
        // Add timer to its trailer
        string taskname;
        IMyTimerBlock Timer = block as IMyTimerBlock;
        if (null != Timer && Trailers.ContainsKey(Timer.CubeGrid))
        {
            if (MyIni.HasSection(Timer.CustomData, Section) && ini.TryParse(Timer.CustomData))
            {
                taskname = ini.Get(Section, "task").ToString();
                switch (taskname)
                {
                    case "stow":
                    case "pack":
                        Trailers[Timer.CubeGrid].AddTimer(Timer, task: TimerTask.Stow);
                        break;
                    case "deploy":
                    case "unpack":
                        Trailers[Timer.CubeGrid].AddTimer(Timer, task: TimerTask.Deploy);
                        break;
                    case "toggle":
                        Trailers[Timer.CubeGrid].AddTimer(Timer, task: TimerTask.Toggle);
                        break;
                    default:
                        Trailers[Timer.CubeGrid].AddTimer(Timer, taskName: taskname.Length > 0 ? taskname : Timer.CustomName);
                        break;
                }
            }
        }
    }
    GridsFound.Clear();
    Couplings.Clear();
    // Find all grids with hinge parts on them (some of which will be all of the couplings)
    foreach (var part in HingeParts)
    {
        if (GridsFound.Contains(part.CubeGrid))
        {
            // This is the second hinge part
            Couplings[part.CubeGrid].AddPart(part);
        }
        else
        {
            // This is the first hinge part
            Couplings.Add(part.CubeGrid, new Coupling(part));
            GridsFound.Add(part.CubeGrid);
        }
    }
    // Now weed out the grids where a second hinge/rotor part wasn't found
    // Yes, some might have rotor parts, but that's not a problem
    // Just want to exclude any that would give us null values.
    foreach (var grid in Couplings.Keys.ToList())
    {
        if (!Couplings[grid].HasTwoParts())
            Couplings.Remove(grid);
    }
    // Hook up the first coupling to our tractor's tow hitch...
    IMyCubeGrid NextGrid;
    foreach (var coupling in Couplings.Values)
    {
        NextGrid = coupling.GetOtherGrid(Me.CubeGrid);
        if (null != NextGrid && Trailers.ContainsKey(NextGrid))
            {
            FirstTrailer = Trailers[NextGrid];
            TractorHitch = (IMyMotorAdvancedStator)coupling.GetOtherHinge(NextGrid);
            break;
        }
    }
    if (null == FirstTrailer && null != TractorHitch && TractorHitch.IsAttached)
    {
        ManagedDisplay.SetFeedback(new Feedback() { BackgroundColor = Color.Maroon, Sprite = "Danger", TextColor = Color.Yellow, duration = 8, Message = "Unsupported Trailer" });
    }
    // ...and connect the trailers to each other
    foreach (var trailer in Trailers.Values)
    {
        trailer.DetectNextTrailer();
    }
}

private void ArrangeTrailersIntoTrain(Trailer first)
{
    Consist.Clear();
    var trailer = first;
    while (trailer != null)
    {
        Consist.Add(trailer);
        trailer = trailer.NextTrailer;
    }
}

private void ReadConfig()
{
    ini.Clear();
    ini.TryParse(Me.CustomData);
    CfgAutoDeploy = ini.Get(Section,"autodeploy").ToBoolean();
    CfgMirror = ini.Get(Section,"mirror").ToBoolean();
}

private void WriteConfig()
{
    ini.Clear();
    ini.TryParse(Me.CustomData);
    ini.Set(Section, "autodeploy", CfgAutoDeploy);
    ini.Set(Section, "mirror", CfgMirror);
    Me.CustomData = ini.ToString();
}

public Program()
{
    ReadConfig();
    BuildAll();
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    Echo("Version "+Version);
}

private void ForceBuildAll()
{
    UnidentifiedTrailer = false;
    ReadConfig();
    BuildAll();
}

private void BuildAll()
{
    // If an unidentified trailer was found and hasn't been sorted, quit. This
    // isn't a cheap function.
    if (UnidentifiedTrailer) return;

    // BuildConsist populates Blocks, so we run that first.
    BuildConsist();
    FindDisplays();
    ArrangeTrailersIntoTrain(FirstTrailer);

    // Check whether we have something unidentified coupled to the end of our consist
    UnidentifiedTrailer = (Consist.Count > 0 && Consist[Consist.Count - 1].IsCoupled()) || (Consist.Count == 0 && null != TractorHitch && TractorHitch.IsAttached);

    BuildTopMenu();
    BuildAllTrailersMenu();
    BuildAllBatteriesMenu();
    BuildAllHydrogenMenu();
    BuildConfigurationMenu();

    RenderTopMenu();
    ActivateTopMenu();
}

public void ActivateAllBatteriesMenu()
{
    SelectedMenu = MenuOption.AllBatteries;
    selectedline = 0;
}
public void ActivateAllHydrogenMenu()
{
    SelectedMenu = MenuOption.AllHydrogen;
    selectedline = 0;
}
public void ActivateTopMenu()
{
    if (SelectedMenu == MenuOption.Config)
        selectedline = TopMenu.Count - 1;
    else if (SelectedMenu == MenuOption.Trailer)
        selectedline = Consist.IndexOf(selectedtrailer) + 1;
    else
        selectedline = 0;
    SelectedMenu = MenuOption.Top;
}
public void ActivateAllTrailersMenu()
{
    switch (SelectedMenu)
    {
        case MenuOption.AllBatteries:
            selectedline = 6;
            break;
        case MenuOption.AllHydrogen:
            selectedline = 7;
            break;
        default:
            selectedline = 0;
            break;
    }
    SelectedMenu = MenuOption.AllTrailers;
}
public void ActivateTrailerMenu()
{
    SelectedMenu = MenuOption.Trailer;
    selectedtrailer = Consist[selectedline - 1];
    selectedline = 0;
}
public void ActivateConfigurationMenu()
{
    SelectedMenu = MenuOption.Config;
    selectedline = 0;
}

private void BuildTopMenu()
{
    TopMenu.Clear();
    TopMenu.Add(new MenuItem() { MenuText = "All Trailers...", TextColor = Color.White, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_20.dds", SpriteColor = Color.White, SpriteRotation = (float)(0.5f * Math.PI), Action = ActivateAllTrailersMenu });
    foreach (var trailer in Consist)
        TopMenu.Add(new MenuItem() { MenuText = trailer.Name, TextColor = Color.Gray, Sprite = "AH_BoreSight", SpriteColor = Color.White, Action = ActivateTrailerMenu });
    TopMenu.Add(new MenuItem() { MenuText = "Configuration...", TextColor = Color.White, Sprite = "Construction", SpriteColor = Color.White, Action = ActivateConfigurationMenu });
}

private void BuildAllBatteriesMenu()
{
    AllBatteriesMenu.Clear();
    AllBatteriesMenu.Add(new MenuItem() { MenuText = "Back", TextColor = Color.Gray, Sprite = "AH_PullUp", SpriteColor = Color.White, SpriteRotation = (float)(1.5f * Math.PI), Action = ActivateAllTrailersMenu });
    AllBatteriesMenu.Add(new MenuItem() { MenuText = "All batteries recharge", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.Yellow, Action = () => AllTrailersBatteryCharge(ChargeMode.Recharge) });
    AllBatteriesMenu.Add(new MenuItem() { MenuText = "All batteries auto", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.Green, Action = () => AllTrailersBatteryCharge(ChargeMode.Auto) });
    AllBatteriesMenu.Add(new MenuItem() { MenuText = "All batteries discharge", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.Cyan, Action = () => AllTrailersBatteryCharge(ChargeMode.Discharge) });
    AllBatteriesMenu.Add(new MenuItem() { MenuText = "All batteries off", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.DarkRed, Action = AllTrailersDisableBattery });
}

private void BuildAllTrailersMenu()
{
    AllTrailersMenu.Clear();
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Back", TextColor = Color.Gray, Sprite = "AH_PullUp", SpriteColor = Color.White, SpriteRotation = (float)(1.5f * Math.PI), Action = ActivateTopMenu });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Pack all trailers", Sprite = "Arrow", TextColor = Color.Gray, SpriteColor = Color.YellowGreen, Action = AllTrailersStow });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Unpack rearmost trailer", Sprite = "Arrow", SpriteColor = Color.Green, SpriteRotation = (float)Math.PI, TextColor = Color.Gray, Action = DeployLastTrailer });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Detach rearmost trailer", Sprite = "Cross", SpriteColor = Color.Red, SpriteRotation = (float)Math.PI, TextColor = Color.Gray, Action = DetachLastTrailer });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Handbrake On", TextColor = Color.Gray, SpriteColor = Color.Green, Action = AllTrailersHandbrakeOn, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds" });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Handbrake Off", TextColor = Color.Gray, SpriteColor = Color.Yellow, Action = AllTrailersHandbrakeOff, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds" });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Batteries...", TextColor = Color.White, SpriteColor = Color.White, Action = ActivateAllBatteriesMenu, Sprite = "IconEnergy" });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Hydrogen...", TextColor = Color.White, SpriteColor = Color.White, Action = ActivateAllHydrogenMenu, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_27.dds" });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Weapons Live", TextColor = Color.Gray, SpriteColor = Color.Green, Action = AllTrailersWeaponsLive, Sprite = "MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem" });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Weapons Safe", TextColor = Color.Gray, SpriteColor = Color.Red, Action = AllTrailersWeaponsSafe, Sprite = "MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem" });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Switch Rear Connector", TextColor = Color.Gray, SpriteColor = Color.Yellow, Action = SwitchRearConnector, Sprite = "CircleHollow" });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Unpack all trailers", Sprite = "Arrow", SpriteColor = Color.Green, SpriteRotation = (float)Math.PI, TextColor = Color.Gray, Action = AllTrailersDeploy });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "Attach another trailer", Sprite = "Textures\\FactionLogo\\Traders\\TraderIcon_2.dds", TextColor = Color.Gray, SpriteColor = Color.YellowGreen, Action = AttachLastTrailer });
    AllTrailersMenu.Add(new MenuItem() { MenuText = "De-power wheels", TextColor = Color.Gray, SpriteColor = Color.Red, Action = AllTrailersWheelsOff, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds" });
}

private void BuildAllHydrogenMenu()
{
    AllHydrogenMenu.Clear();
    AllHydrogenMenu.Add(new MenuItem() { MenuText = "Back", TextColor = Color.Gray, Sprite = "AH_PullUp", SpriteColor = Color.White, SpriteRotation = (float)(1.5f * Math.PI), Action = ActivateAllTrailersMenu });
    AllHydrogenMenu.Add(new MenuItem() { MenuText = "Engines on", TextColor = Color.Gray, SpriteColor = Color.Green, Action = AllTrailersEnginesOn, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_27.dds" });
    AllHydrogenMenu.Add(new MenuItem() { MenuText = "Engines off", TextColor = Color.Gray, SpriteColor = Color.Red, Action = AllTrailersEnginesOff, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_27.dds" });
    AllHydrogenMenu.Add(new MenuItem() { MenuText = "H Tank Stockpile on", TextColor = Color.Gray, SpriteColor = Color.Cyan, Action = AllTrailersHydrogenStockpileOn, Sprite = "MyObjectBuilder_GasContainerObject/HydrogenBottle" });
    AllHydrogenMenu.Add(new MenuItem() { MenuText = "H Tank Stockpile off", TextColor = Color.Gray, SpriteColor = Color.Green, Action = AllTrailersHydrogenStockpileOff, Sprite = "MyObjectBuilder_GasContainerObject/HydrogenBottle" });
    AllHydrogenMenu.Add(new MenuItem() { MenuText = "Generators on", TextColor = Color.Gray, SpriteColor = Color.Green, Action = AllTrailersGasGeneratorsOn, Sprite = "MyObjectBuilder_Ore/Ice" });
    AllHydrogenMenu.Add(new MenuItem() { MenuText = "Generators off", TextColor = Color.Gray, SpriteColor = Color.Red, Action = AllTrailersGasGeneratorsOff, Sprite = "MyObjectBuilder_Ore/Ice" });
}

private void BuildConfigurationMenu()
{
    ConfigurationMenu.Clear();
    ConfigurationMenu.Add(new MenuItem() { MenuText = "Back", TextColor = Color.Gray, Sprite = "AH_PullUp", SpriteColor = Color.White, SpriteRotation = (float)(1.5f * Math.PI), Action = ActivateTopMenu });
    ConfigurationMenu.Add(new MenuItem() { MenuText = "Toggle AutoDeploy", TextColor = CfgAutoDeploy ? Color.Gray : Color.DarkGray, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_33.dds", SpriteColor = CfgAutoDeploy?Color.Green:Color.Red, Action = ToggleAutoDeploy });
    ConfigurationMenu.Add(new MenuItem() { MenuText = "Toggle Mirroring", TextColor = CfgAutoDeploy ? Color.Gray : Color.DarkGray, Sprite = "Textures\\FactionLogo\\Traders\\TraderIcon_2.dds", SpriteColor = CfgMirror?Color.Green:Color.Red, Action = ToggleMirror });
    ConfigurationMenu.Add(new MenuItem() { MenuText = "Rebuild consist", TextColor = Color.Gray, Sprite = "Textures\\FactionLogo\\Builders\\BuilderIcon_16.dds", SpriteColor = Color.Cyan, Action = ForceBuildAll });
    ConfigurationMenu.Add(new MenuItem() { MenuText = "Detect trailer", TextColor = Color.Gray, Sprite = "Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds", SpriteColor = Color.OrangeRed, Action = LegacyUpdate });
}

private void ToggleAutoDeploy()
{
    CfgAutoDeploy = !CfgAutoDeploy;
    WriteConfig();
    BuildConfigurationMenu();
}

private void ToggleMirror()
{
    CfgMirror = !CfgMirror;
    WriteConfig();
    BuildConfigurationMenu();
}

private void FindDisplays()
{
    Displays.Clear();
    foreach (IMyTerminalBlock TextSurfaceProvider in Blocks.OfType<IMyTextSurfaceProvider>())
    {
        if (((IMyTextSurfaceProvider)TextSurfaceProvider).SurfaceCount > 0 && (MyIni.HasSection(TextSurfaceProvider.CustomData, Section)))
        {
            ini.TryParse(TextSurfaceProvider.CustomData);
            var displayNumber = ini.Get(Section, "display").ToUInt16();
            if (displayNumber < ((IMyTextSurfaceProvider)TextSurfaceProvider).SurfaceCount || ((IMyTextSurfaceProvider)TextSurfaceProvider).SurfaceCount == 0)
            {
                var display = ((IMyTextSurfaceProvider)TextSurfaceProvider).GetSurface(ini.Get(Section, "display").ToInt16());
                float scale = ini.Get(Section, "scale").ToSingle(1.0f);
                string DefaultColor = "FF4500";
                string ColorStr = ini.Get(Section, "color").ToString(DefaultColor);
                if (ColorStr.Length < 6)
                    ColorStr = DefaultColor;
                Color color = new Color() {
                    R = byte.Parse(ColorStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                    G = byte.Parse(ColorStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                    B = byte.Parse(ColorStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber),
                    A = 255
                };
                Displays.Add(new ManagedDisplay(display, scale, color));
            }
            else
            {
                Echo("Warning: " + TextSurfaceProvider.CustomName + " doesn't have a display number " + ini.Get(Section, "display").ToString());
            }
        }
    }
}

public void RenderTopMenu()
{
    // The main menu
    foreach (var display in Displays)
        display.RenderMenu(selectedline, TopMenu);
}

public void RenderAllTrailersMenu()
{
    // Menu with functions for all trailers
    foreach (var display in Displays)
        display.RenderMenu(selectedline, AllTrailersMenu);
}

public void RenderAllBatteriesMenu()
{
    // Menu with battery charge functions for all trailers
    foreach (var display in Displays)
        display.RenderMenu(selectedline, AllBatteriesMenu);
}

public void RenderAllHydrogenMenu()
{
    // Menu with battery charge functions for all trailers
    foreach (var display in Displays)
        display.RenderMenu(selectedline, AllHydrogenMenu);
}

public void RenderTrailerMenu()
{
    // Menu specific to a trailer
    selectedtrailer.BuildMenu(ActivateTopMenu);
    foreach (var display in Displays)
        display.RenderMenu(selectedline, selectedtrailer.Menu);
}

public void RenderConfigurationMenu()
{
    // The config menu
    foreach (var display in Displays)
        display.RenderMenu(selectedline, ConfigurationMenu);
}

public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & (UpdateType.Terminal | UpdateType.Trigger)) != 0)
    {
        switch (argument.ToLower())
        {
            case "legacyupdate":
                LegacyUpdate();
                break;
            case "brakes on":
                AllTrailersHandbrakeOn();
                break;
            case "brakes off":
                AllTrailersHandbrakeOff();
                break;
            case "deploy":
            case "unpack":
                DeployLastTrailer();
                break;
            case "detach":
                DetachLastTrailer();
                break;
            case "hitch":
            case "attach":
                AttachLastTrailer();
                break;
            case "connector":
                SwitchRearConnector();
                break;
            case "allpack":
                AllTrailersStow();
                break;
            case "weapons on":
                AllTrailersWeaponsLive();
                break;
            case "weapons off":
                AllTrailersWeaponsSafe();
                break;
            case "rebuild":
                ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.DarkCyan, TextColor = Color.White, Message = "Rebuilding Train", Sprite = "Screen_LoadingBar", duration = 4 });
                ForceBuildAll();
                break;
            case "up":
                if (selectedline > 0)
                    --selectedline;
                break;
            case "down":
                if (SelectedMenu == MenuOption.Top && selectedline < TopMenu.Count - 1)
                    ++selectedline;
                if (SelectedMenu == MenuOption.AllTrailers && selectedline < AllTrailersMenu.Count - 1)
                    ++selectedline;
                if (SelectedMenu == MenuOption.AllBatteries && selectedline < AllBatteriesMenu.Count - 1)
                    ++selectedline;
                if (SelectedMenu == MenuOption.AllHydrogen && selectedline < AllHydrogenMenu.Count - 1)
                    ++selectedline;
                if (SelectedMenu == MenuOption.Trailer && selectedline < selectedtrailer.Menu.Count - 1)
                    ++selectedline;
                if (SelectedMenu == MenuOption.Config && selectedline < ConfigurationMenu.Count - 1)
                    ++selectedline;
                break;
            case "back":
                if (SelectedMenu == MenuOption.AllBatteries || SelectedMenu == MenuOption.AllHydrogen)
                    ActivateAllTrailersMenu();
                else
                    ActivateTopMenu();
                break;
            case "apply":
            case "select":
                switch (SelectedMenu)
                {
                    case MenuOption.Top:
                        TopMenu[selectedline].Action();
                        break;
                    case MenuOption.AllTrailers:
                        AllTrailersMenu[selectedline].Action();
                        break;
                    case MenuOption.Trailer:
                        selectedtrailer.Menu[selectedline].Action();
                        break;
                    case MenuOption.Config:
                        ConfigurationMenu[selectedline].Action();
                        break;
                    case MenuOption.AllBatteries:
                        AllBatteriesMenu[selectedline].Action();
                        break;
                    case MenuOption.AllHydrogen:
                        AllHydrogenMenu[selectedline].Action();
                        break;
                    default:
                        selectedline = 0;
                        SelectedMenu = MenuOption.Top;
                        break;
                }
                break;
            default:
                break;
        }
    }

    if ((updateSource & (UpdateType.Update10)) != 0)
    {
        ManagedDisplay.FeedbackTick();
        RefreshConsist();
    }

    switch (SelectedMenu)
    {
        case MenuOption.Top:
            RenderTopMenu();
            break;
        case MenuOption.AllTrailers:
            RenderAllTrailersMenu();
            break;
        case MenuOption.AllBatteries:
            RenderAllBatteriesMenu();
            break;
        case MenuOption.AllHydrogen:
            RenderAllHydrogenMenu();
            break;
        case MenuOption.Trailer:
            RenderTrailerMenu();
            break;
        case MenuOption.Config:
            RenderConfigurationMenu();
            break;
        default:
            break;
    }
}

private void RefreshConsist()
{
    // Deal with unexpectedly detached or attached first trailers
    if (null != TractorHitch)
        if (!TractorHitch.IsAttached && Consist.Count > 0)
        {
            // Something was attached and now it isn't
            FirstTrailer.Deploy();
            FirstTrailer = null;
            Consist.Clear();
            BuildTopMenu();
            ActivateTopMenu();
            ManagedDisplay.SetFeedback(new Feedback() { BackgroundColor = Color.Red, Sprite = "Danger", TextColor = Color.Yellow, duration = 8, Message = "Trailer detached" });
        }
        else if (TractorHitch.IsAttached && Consist.Count == 0)
        {
            // Nothing was attached, but now something is
            BuildAll();
            if (UnidentifiedTrailer)
                ManagedDisplay.SetFeedback(new Feedback() { BackgroundColor = Color.Maroon, Sprite = "Danger", TextColor = Color.Yellow, duration = 8, Message = "Unknown trailer!" });
            else
                ManagedDisplay.SetFeedback(new Feedback() { BackgroundColor = Color.Green, Sprite = "Danger", TextColor = Color.Yellow, duration = 8, Message = "Trailer found" });
        }
    // Deal with unexpectedly detached or attached subsequent trailers
    for (int i = 0; i < Consist.Count - 1; ++i)
    {
        if (!Consist[i].IsCoupled())
        {
            if (CfgAutoDeploy)
                Consist[i].NextTrailer.Deploy();
            Consist[i].NextTrailer = null;
            ArrangeTrailersIntoTrain(FirstTrailer);
            BuildTopMenu();
            ActivateTopMenu();
            ManagedDisplay.SetFeedback(new Feedback() { BackgroundColor = Color.Red, Sprite = "Danger", TextColor = Color.Yellow, duration = 8, Message = "Trailer detached" });
        }
    }
    if (Consist.Count > 0 && Consist[Consist.Count - 1].IsCoupled())
    {
        BuildAll();
        if (UnidentifiedTrailer)
            ManagedDisplay.SetFeedback(new Feedback() { BackgroundColor = Color.Maroon, Sprite = "Danger", TextColor = Color.Yellow, duration = 8, Message = "Unknown trailer!" });
        else
            ManagedDisplay.SetFeedback(new Feedback() { BackgroundColor = Color.Green, Sprite = "Danger", TextColor = Color.Yellow, duration = 8, Message = "Trailer found" });
    }
    // Mirror vehicle state in trailers
    if (CfgMirror)
        Mirror();
}

class Coupling
{
    private IMyAttachableTopBlock A, B;

    public Coupling(IMyAttachableTopBlock part)
    {
        this.A = part;
    }

    public void AddPart(IMyAttachableTopBlock part)
    {
        this.B = part;
    }

    public bool HasTwoParts()
    {
        return null != B && !A.Equals(B);
    }

    public bool ContainsPart(IMyAttachableTopBlock part)
    {
        return (A.Equals(part) || B.Equals(part));
    }

    public IMyCubeGrid GetOtherGrid(IMyCubeGrid grid)
    {
        if (null == B) return null;
        if (grid.Equals(A.Base.CubeGrid) && B.IsAttached)
            return B.Base.CubeGrid;
        if (grid.Equals(B.Base.CubeGrid) && A.IsAttached)
            return A.Base.CubeGrid;
        return null;
    }
    public IMyMechanicalConnectionBlock GetOtherHinge(IMyCubeGrid grid)
    {
        if (null == B) return null;
        if (grid.Equals(A.Base.CubeGrid) && B.IsAttached)
            return B.Base;
        if (grid.Equals(B.Base.CubeGrid) && A.IsAttached)
            return A.Base;
        return null;
    }

}

class ManagedDisplay
{
    private IMyTextSurface surface;
    private RectangleF viewport;
    private MySpriteDrawFrame frame;
    private float StartHeight = 5f;
    private float HeadingHeight = 35f;
    private float LineHeight = 40f;
    private float BodyBeginsHeight = 65f; // StartHeight + HeadingHeight + 25;
    private float HeadingFontSize = 2.0f;
    private float RegularFontSize = 1.5f;
    private Vector2 Position;
    private Vector2 CursorDrawPosition;
    private int WindowSize;         // Number of lines shown on screen at once after heading
    private int WindowPosition = 0; // Number of lines scrolled away
    private int CursorMenuPosition; // Position of cursor within window
    static Program.Feedback Feedback;
    private float Scale;
    private Color HighlightColor;

    public ManagedDisplay(IMyTextSurface surface, float scale = 1.0f, Color highlightColor = new Color())
    {
        this.surface = surface;
        this.Scale = scale;
        this.HighlightColor = highlightColor;

        // Scale everything!
        StartHeight *= scale;
        HeadingHeight *= scale;
        LineHeight *= scale;
        BodyBeginsHeight *= scale;
        HeadingFontSize *= scale;
        RegularFontSize *= scale;

        surface.ContentType = ContentType.SCRIPT;
        surface.Script = "";
        surface.ScriptBackgroundColor = Color.Black;
        viewport = new RectangleF((surface.TextureSize - surface.SurfaceSize) / 2f, surface.SurfaceSize);
        WindowSize = ((int)((viewport.Height - BodyBeginsHeight - 10 * scale) / LineHeight));
    }

    public static void FeedbackTick()
    {
        --Feedback.duration;
    }

    public static void SetFeedback(Program.Feedback feedback)
    {
        Feedback = feedback;
    }

    private void ShowFeedback()
    {
        if (Feedback.duration > 0)
        {
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "SquareSimple",
                Position = new Vector2(0, viewport.Y + LineHeight),
                Color = Feedback.BackgroundColor,
                Size = new Vector2(viewport.Width, LineHeight * 2)
            });
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = Feedback.Sprite,
                Position = new Vector2(LineHeight, viewport.Y + LineHeight * 1.25f),
                Color = Feedback.TextColor,
                Alignment = TextAlignment.CENTER /* Center the text on the position */,
                Size = new Vector2(LineHeight, LineHeight),
                FontId = "White"
            });
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = Feedback.Message,
                Position = new Vector2(2 * LineHeight, viewport.Y + HeadingHeight / 2),
                RotationOrScale = HeadingFontSize,
                Color = Feedback.TextColor,
                Alignment = TextAlignment.LEFT /* Center the text on the position */,
                FontId = "White"
            });
        }
    }

    private void DrawCursor()
    {
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "SquareSimple",
            Position = CursorDrawPosition,
            Color = HighlightColor,
            Size = new Vector2(viewport.Width, LineHeight)
        });
    }

    private void AddHeading(int menuLength)
    {
        Position = new Vector2(viewport.Width / 2f - LineHeight, StartHeight) + viewport.Position;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Trailer Manager",
            Position = Position,
            RotationOrScale = HeadingFontSize,
            Color = Color.White,
            Alignment = TextAlignment.CENTER /* Center the text on the position */,
            FontId = "White"
        });
        Position = new Vector2(viewport.Width - 2 * LineHeight, LineHeight) + viewport.Position;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "AH_BoreSight",
            Color = (WindowPosition > 0) ? HighlightColor : Color.Black.Alpha(0),
            RotationOrScale = 1.5f * (float)Math.PI,
            Size = new Vector2(LineHeight, LineHeight),
            Position = Position,
        });
        Position += new Vector2(LineHeight, 0);
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "AH_BoreSight",
            Color = (WindowPosition + WindowSize < menuLength) ? HighlightColor : Color.Black.Alpha(0),
            RotationOrScale = 0.5f * (float)Math.PI,
            Size = new Vector2(LineHeight, LineHeight),
            Position = Position,
        });
        Position = new Vector2(viewport.Width / 2f - LineHeight, StartHeight + HeadingHeight) + viewport.Position;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "----------------------------",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.CENTER,
            FontId = "White"
        });
    }

    private void AddMenuItem(MenuItem menuItem)
    {
        AddMenuItem(
            menuText: menuItem.MenuText,
            sprite: menuItem.Sprite,
            spriteRotation: menuItem.SpriteRotation,
            spriteColor: menuItem.SpriteColor,
            textColor: menuItem.TextColor
            );
    }

    private void AddMenuItem(string menuText, string sprite = "SquareSimple", float spriteRotation = 0, Color? spriteColor = null, Color? textColor = null)
    {
        if (null == spriteColor)
            spriteColor = Color.White;
        float SpriteOffset = 25f * Scale;
        Position += new Vector2(0, LineHeight);
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = sprite,
            Position = Position + new Vector2(0, SpriteOffset),
            RotationOrScale = spriteRotation,
            Size = new Vector2(LineHeight, LineHeight),
            Color = spriteColor ?? Color.White,
        });
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = menuText,
            Position = Position + new Vector2(LineHeight * 1.2f, 0),
            RotationOrScale = RegularFontSize,
            Color = textColor ?? Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
    }

    internal void RenderMenu(int selectedline, List<MenuItem> menuItems)
    {
        SetWindowPosition(selectedline);
        frame = surface.DrawFrame();
        CursorDrawPosition = new Vector2(0, BodyBeginsHeight + LineHeight + LineHeight * CursorMenuPosition) + viewport.Position;
        DrawCursor();
        AddHeading(menuItems.Count);
        Position.X = surface.TextPadding;
        int renderLineCount = 0;
        foreach (var menuItem in menuItems)
        {
            if (renderLineCount >= WindowPosition && renderLineCount < WindowPosition + WindowSize)
                AddMenuItem(menuItem);
            ++renderLineCount;
        }
        ShowFeedback();
        frame.Dispose();
    }

    private void SetWindowPosition(int selectedline)
    {
        CursorMenuPosition = selectedline - WindowPosition;
        if (CursorMenuPosition < 0)
        {
            CursorMenuPosition = 0;
            WindowPosition = selectedline;
        }
        if (CursorMenuPosition >= WindowSize)
        {
            CursorMenuPosition = WindowSize - 1;
            WindowPosition = selectedline - (WindowSize - 1);
        }
    }
}

class Trailer
{
    private IMyMotorAdvancedStator ForwardHitch;
    public IMyMotorAdvancedStator RearHitch
    {
        get;
        set;
    }
    private IMyCubeGrid Grid;
    public string Name;
    public Trailer NextTrailer;
    private Program program;
    private List<IMyBatteryBlock> Batteries = new List<IMyBatteryBlock>();
    private List<IMyMotorSuspension> Wheels = new List<IMyMotorSuspension>();
    private List<IMyPowerProducer> Engines = new List<IMyPowerProducer>();
    private List<IMyGasTank> HTanks = new List<IMyGasTank>();
    private List<IMyGasGenerator> HGens = new List<IMyGasGenerator>();
    private List<TimerWithTaskName> TimersWithTask = new List<TimerWithTaskName>();
    private List<IMyUserControllableGun> Weapons = new List<IMyUserControllableGun>();
    private List<IMyShipConnector> Connectors = new List<IMyShipConnector>();
    private List<IMyRadioAntenna> Antennae = new List<IMyRadioAntenna>();
    private IMyShipController controller;
    private IMyTimerBlock StowTimer, DeployTimer;
    private bool IgnoreHingeLock;
    private bool IgnoreHandbrake;
    private bool HingeAction = true;
    private bool HandbrakeAction = true;

    private struct TimerWithTaskName
    {
        public IMyTimerBlock Timer;
        public string TaskName;
    }

    internal List<MenuItem> Menu = new List<MenuItem>();

    public Trailer(Program program, IMyMotorAdvancedStator forwardHitch, bool ignoreHingeLock, bool hingeAction, bool ignoreHandbrake, bool handbrakeAction)
    {
        Grid = forwardHitch.CubeGrid;
        this.program = program;
        IgnoreHingeLock = ignoreHingeLock;
        IgnoreHandbrake = ignoreHandbrake;
        HingeAction = hingeAction;
        HandbrakeAction = handbrakeAction;
        if (program.ini.TryParse(forwardHitch.CustomData))
        {
            this.Name = program.ini.Get(Program.Section, "name").ToString();
            if (null == this.Name || this.Name.Length == 0)
                this.Name = forwardHitch.CubeGrid.CustomName;
        }
        else
        {
            this.Name = forwardHitch.CubeGrid.CustomName;
        }
        this.ForwardHitch = forwardHitch;
    }

    public bool IsCoupled()
    {
        return null != RearHitch && RearHitch.IsAttached;
    }

    public void Attach()
    {
        if (null != RearHitch && !RearHitch.IsAttached)
            RearHitch.Attach();
    }

    public void AddBattery(IMyBatteryBlock battery)
    {
        Batteries.Add(battery);
    }
    public void AddWheel(IMyMotorSuspension wheel)
    {
        Wheels.Add(wheel);
    }
    public void AddEngine(IMyPowerProducer engine)
    {
        Engines.Add(engine);
    }
    public void AddHTank(IMyGasTank tank)
    {
        HTanks.Add(tank);
    }
    public void AddHGen(IMyGasGenerator generator)
    {
        HGens.Add(generator);
    }
    public void AddAntenna(IMyRadioAntenna antenna)
    {
        Antennae.Add(antenna);
    }
    public void AddController(IMyShipController controller)
    {
        this.controller = controller;
    }
    public void AddTimer(IMyTimerBlock timer, TimerTask task = TimerTask.Menu, string taskName = "")
    {
        // Pack is used to flag when a timer is for stowing (packing) or deploying (unpacking)
        if (task == TimerTask.Stow)
            StowTimer = timer;
        else if (task == TimerTask.Deploy)
            DeployTimer = timer;
        else if (task == TimerTask.Toggle)
        {
            DeployTimer = timer;
            StowTimer = timer;
        }
        else
        {
            TimersWithTask.Add(new TimerWithTaskName { Timer = timer, TaskName = taskName });
        }
    }
    public void AddWeapon(IMyUserControllableGun Weapon)
    {
        Weapons.Add(Weapon);
    }
    public void AddConnector(IMyShipConnector connector)
    {
        Connectors.Add(connector);
    }

    public void SetBatteryChargeMode(ChargeMode chargeMode)
    {
        foreach (var battery in Batteries)
        {
            battery.Enabled = true;
            battery.ChargeMode = chargeMode;
        }
        Color ChargeColor = Color.Green;
        if (chargeMode == ChargeMode.Recharge)
            ChargeColor = Color.Yellow;
        if (chargeMode == ChargeMode.Discharge)
            ChargeColor = Color.Cyan;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = ChargeColor, Message = chargeMode.ToString(), Sprite = "IconEnergy", duration = 4 });
    }
    public void DisableBattery()
    {
        foreach (var battery in Batteries)
        {
            battery.Enabled = false;
        }
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.DarkRed, Message = "Batteries off", Sprite = "IconEnergy", duration = 4 });
    }

    public void HydrogenTankStockpileOn()
    {
        foreach (var tank in HTanks)
            tank.Stockpile = true;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Cyan, Message = "Stockpile H On", Sprite = "IconHydrogen", duration = 4 });
    }
    public void HydrogenTankStockpileOff()
    {
        foreach (var tank in HTanks)
            tank.Stockpile = false;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Green, Message = "Stockpile H Off", Sprite = "IconHydrogen", duration = 4 });
    }

    public void EnginesOn()
    {
        foreach (var engine in Engines)
            engine.Enabled = true;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Green, Message = "Engines On", Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_27.dds", duration = 4 });
    }
    public void EnginesOff()
    {
        foreach (var engine in Engines)
            engine.Enabled = false;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Yellow, Message = "Engines Off", Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_27.dds", duration = 4 });
    }

    public void GeneratorsOn()
    {
        foreach (var gen in HGens)
            gen.Enabled = true;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Green, Message = "O2/H2 Gen On", Sprite = "MyObjectBuilder_Ore/Ice", duration = 4 });
    }
    public void GeneratorsOff()
    {
        foreach (var gen in HGens)
            gen.Enabled = false;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Yellow, Message = "O2/H2 Gen Off", Sprite = "MyObjectBuilder_Ore/Ice", duration = 4 });
    }
    public void AntennaeOn()
    {
        foreach (var antenna in Antennae)
            antenna.Enabled = true;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Green, Message = "Antenna On", Sprite = "Textures\\FactionLogo\\Builders\\BuilderIcon_3.dds", duration = 4 });
    }
    public void AntennaeOff()
    {
        foreach (var antenna in Antennae)
            antenna.Enabled = false;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Red, Message = "Antenna Off", Sprite = "Textures\\FactionLogo\\Builders\\BuilderIcon_3.dds", duration = 4 });
    }
    public void WheelsOff()
    {
        foreach (var Wheel in Wheels)
            Wheel.Enabled = false;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Blue, Message = "Wheels powered off", Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds", duration = 4 });
    }
    public void HandbrakeOn()
    {
        if (null != controller)
            controller.HandBrake = true;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Green, Message = "Handbrake engaged", Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds", duration = 4 });
    }
    public void HandbrakeOff()
    {
        if (null != controller)
            controller.HandBrake = false;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Yellow, Message = "Handbrake disengaged", Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds", duration = 4 });
    }
    public void Deploy()
    {
        if (null != DeployTimer)
        {
            if (null != controller && !IgnoreHandbrake)
                    controller.HandBrake = !HandbrakeAction;
            if (!IgnoreHingeLock)
                ForwardHitch.RotorLock = !HingeAction;
            // Trigger the timer, which will run in the next frame
            DeployTimer.Trigger();
        }
        else
        {
            if (null != controller && !IgnoreHandbrake)
                    controller.HandBrake = true;
            if (!IgnoreHingeLock)
                ForwardHitch.RotorLock = true;
        }
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Yellow, Message = "Trailer Deployed", Sprite = "Arrow", duration = 4 });
    }
    public void Stow()
    {
        if (null != StowTimer)
        {
            // Put handbrakes and rotor lock on, assuming that the Timer will toggle these
            if (null != controller && !IgnoreHandbrake)
                controller.HandBrake = HandbrakeAction;
            if (!IgnoreHingeLock)
                ForwardHitch.RotorLock = HingeAction;
            // Trigger the timer, which will run in the next frame
            StowTimer.Trigger();
        }
        else
        {
            if (null != controller && !IgnoreHandbrake)
                    controller.HandBrake = false;
            if (!IgnoreHingeLock)
                ForwardHitch.RotorLock = false;
        }
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.GreenYellow, Message = "Trailer Stowed", Sprite = "Arrow", SpriteRotation = (float)Math.PI, duration = 4 });
    }
    public void Detach()
    {
        if (program.CfgAutoDeploy)
            Deploy();
        // Get the grid that's towing me
        IMyCubeGrid TowingGrid = program.Couplings[ForwardHitch.TopGrid].GetOtherGrid(Grid);

        // Get the thing that's towing me to detach me
        program.Couplings[ForwardHitch.TopGrid].GetOtherHinge(Grid).Detach();

        // Now remove me and all subsequent trailers from the Consist
        if (TowingGrid == program.Me.CubeGrid)
        {
            // I'm being towed by the tractor vehicle
            program.FirstTrailer = null;
        }
        else
        {
            // I'm being towed by some trailer, let's find it
            program.Trailers[program.Couplings[ForwardHitch.TopGrid].GetOtherGrid(Grid)].NextTrailer = null;
        }
        // Now whatever is towing me, has forgotten me. Rebuild the Consist.
        program.ArrangeTrailersIntoTrain(program.FirstTrailer);
        program.BuildTopMenu();
        program.ActivateTopMenu();
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Maroon, TextColor = Color.Yellow, Message = "Detached", Sprite = "Cross", duration = 4 });
    }
    public void WeaponsLive()
    {
        foreach (var Weapon in Weapons)
            Weapon.Enabled = true;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Maroon, TextColor = Color.Green, Message = "Weapons Live", Sprite = "MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem", duration = 4 });
    }
    public void WeaponsSafe()
    {
        foreach (var Weapon in Weapons)
            Weapon.Enabled = false;
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.SaddleBrown, TextColor = Color.Red, Message = "Weapons Safe", Sprite = "MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem", duration = 4 });
    }
    public void SwitchConnector()
    {
        foreach (var Connector in Connectors)
            Connector.ToggleConnect();
        ManagedDisplay.SetFeedback(new Feedback { BackgroundColor = Color.Black, TextColor = Color.Yellow, Message = "Switch Connector", Sprite = "CircleHollow", duration = 4 });
    }

    public IMyCubeGrid GetGrid()
    {
        return this.Grid;
    }

    public bool DetectNextTrailer()
    {
        IMyCubeGrid NextGrid;
        foreach (var coupling in program.Couplings.Values)
        {
            if (null != RearHitch && RearHitch.IsAttached && coupling.ContainsPart(RearHitch.Top))
            {
                NextGrid = coupling.GetOtherGrid(this.Grid);
                if (null != NextGrid && program.Trailers.ContainsKey(NextGrid))
                {
                    NextTrailer = program.Trailers[NextGrid];
                    return true;
                }
            }
        }
        return false;
    }

    public void BuildMenu(Action BackMenuAction)
    {
        Menu.Clear();
        Menu.Add(new MenuItem() { MenuText = Name, TextColor = Color.White, Sprite = "AH_PullUp", SpriteColor = Color.White, SpriteRotation = (float)(1.5f * Math.PI), Action = BackMenuAction });
        Menu.Add(new MenuItem() { MenuText = "Unpack / Deploy", Sprite = "Arrow", SpriteColor = Color.Green, SpriteRotation = (float)Math.PI, TextColor = Color.Gray, Action = Deploy });
        Menu.Add(new MenuItem() { MenuText = "Pack / Stow for travel", Sprite = "Arrow", SpriteColor = Color.Green, TextColor = Color.Gray, Action = Stow });
        Menu.Add(new MenuItem() { MenuText = "Detach this trailer", Sprite = "Cross", SpriteColor = Color.Red, TextColor = Color.Gray, Action = Detach });
        if (null != RearHitch && !RearHitch.IsAttached)
            Menu.Add(new MenuItem() { MenuText = "Attach another trailer", Sprite = "Textures\\FactionLogo\\Traders\\TraderIcon_2.dds", TextColor = Color.Gray, SpriteColor = Color.YellowGreen, Action = RearHitch.Attach });
        if (Batteries.Count > 0)
        {
            Menu.Add(new MenuItem() { MenuText = "Batteries recharge", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.Yellow, Action = () => SetBatteryChargeMode(ChargeMode.Recharge) });
            Menu.Add(new MenuItem() { MenuText = "Batteries auto", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.Green, Action = () => SetBatteryChargeMode(ChargeMode.Auto) });
            Menu.Add(new MenuItem() { MenuText = "Batteries discharge", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.Cyan, Action = () => SetBatteryChargeMode(ChargeMode.Discharge) });
            Menu.Add(new MenuItem() { MenuText = "Batteries off", TextColor = Color.Gray, Sprite = "IconEnergy", SpriteColor = Color.DarkRed, Action = DisableBattery });
        }
        if (Engines.Count > 0)
        {
            Menu.Add(new MenuItem() { MenuText = "Engines on", TextColor = Color.Gray, SpriteColor = Color.Green, Action = EnginesOn, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_27.dds" });
            Menu.Add(new MenuItem() { MenuText = "Engines off", TextColor = Color.Gray, SpriteColor = Color.Red, Action = EnginesOff, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_27.dds" });
        }
        if (HTanks.Count > 0)
        {
            Menu.Add(new MenuItem() { MenuText = "H Tank Stockpile on", TextColor = Color.Gray, SpriteColor = Color.Cyan, Action = HydrogenTankStockpileOn, Sprite = "MyObjectBuilder_GasContainerObject/HydrogenBottle" });
            Menu.Add(new MenuItem() { MenuText = "H Tank Stockpile off", TextColor = Color.Gray, SpriteColor = Color.Green, Action = HydrogenTankStockpileOff, Sprite = "MyObjectBuilder_GasContainerObject/HydrogenBottle" });
        }
        if (HGens.Count > 0)
        {
            Menu.Add(new MenuItem() { MenuText = "Generators on", TextColor = Color.Gray, SpriteColor = Color.Green, Action = GeneratorsOn, Sprite = "MyObjectBuilder_Ore/Ice" });
            Menu.Add(new MenuItem() { MenuText = "Generators off", TextColor = Color.Gray, SpriteColor = Color.Red, Action = GeneratorsOff, Sprite = "MyObjectBuilder_Ore/Ice" });
        }
        if (Connectors.Count>0)
        {
            Menu.Add(new MenuItem() { MenuText = "Switch Connector(s)", TextColor = Color.Gray, SpriteColor = Color.Yellow, Action = SwitchConnector, Sprite = "CircleHollow" });
        }
        if (Antennae.Count > 0)
        {
            Menu.Add(new MenuItem() { MenuText = "Antenna on", TextColor = Color.Gray, SpriteColor = Color.Green, Action = AntennaeOn, Sprite = "Textures\\FactionLogo\\Builders\\BuilderIcon_3.dds" });
            Menu.Add(new MenuItem() { MenuText = "Antenna off", TextColor = Color.Gray, SpriteColor = Color.Red, Action = AntennaeOff, Sprite = "Textures\\FactionLogo\\Builders\\BuilderIcon_3.dds" });
        }
        if (TimersWithTask.Count > 0)
        {
            foreach (var TimerWithTask in TimersWithTask)
            {
                Menu.Add(new MenuItem() { MenuText = TimerWithTask.TaskName, TextColor = Color.Gray, SpriteColor = Color.Blue, Action = TimerWithTask.Timer.Trigger, Sprite = "Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds" });
            }
        }
        if (Weapons.Count > 0)
        {
            Menu.Add(new MenuItem() { MenuText = "Weapons Live", TextColor = Color.Gray, SpriteColor = Color.Green, Action = WeaponsLive, Sprite = "MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem" });
            Menu.Add(new MenuItem() { MenuText = "Weapons Safe", TextColor = Color.Gray, SpriteColor = Color.Red, Action = WeaponsSafe, Sprite = "MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem" });
        }
        if (null != controller)
        {
            if (controller.HandBrake)
                Menu.Add(new MenuItem() { MenuText = "Disengage Handbrake", TextColor = Color.Gray, SpriteColor = Color.Yellow, Action = HandbrakeOff, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds" });
            else
                Menu.Add(new MenuItem() { MenuText = "Engage Handbrake", TextColor = Color.Gray, SpriteColor = Color.Green, Action = HandbrakeOn, Sprite = "Textures\\FactionLogo\\Others\\OtherIcon_22.dds" });
        }
    }
}