// Damage&Alert Script v2.1 by MrHam5
// 2019.07.29.

// If you modify the lights, LCDs, or any part of your ship you need to reset the script.
// To reset the script run it with "reset" (leaving out quotes)
// Recompiling the script won't refresh the Hull block count!

// Add this string in the block's name you want to control (you can also edit it)
const string selectorString = "Alert";

// Set this to false, if you don't want the script to enable/disable
// the turrets and shield on your ship automatically
const bool autoToggle = true;

// Set the block group names here
const string aLightGroupName = "Alert Lights";
const string doorGroupName = "Blast Doors";
const string weaponGroupName = "Defense Turrets";

// Customize the Interior Light group system here
const string iLightGroupName = "Lights Interior";
const float bright = 3f;
const float dim = 1f;

Program()
{
    // Add your alerts below this line (use the provided format, and remove the // from the beginning of the line)
    //AlertHandler.AddCustomAlert("myalert", "My own alert", new Color(255, 255, 255), Blink.AlwaysOn, false, Doors.NoInteract, "", Systems.AllOffline);

    //#####################################################################//
    // DO NOT EDIT ANYTHING BELOW THIS (unless you know what you're doing) //
    //#####################################################################//

    Echo("Launching script...");
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    if (!Me.CustomName.Contains(selectorString) ||
        !myIni.TryParse(Me.CustomData) ||
        !myIni.ContainsSection("ScriptConfig") ||
        myIni.Get("ScriptConfig", "version").ToDouble() != version
        )
    {
        Setup();
    }

    hullBlockCount = myIni.Get("ScriptConfig", "hullBlockCount").ToInt32();
    CalculateHull();
    CalculateShield(true);
    ResetScript();

    Echo("Launch complete!");
}

public void Main(string argument, UpdateType updateSource)
{
    switch (updateSource)
    {
        case UpdateType.Terminal:
        case UpdateType.Trigger:
        case UpdateType.Script:
        case UpdateType.IGC:
        case UpdateType.Mod:
            UserRun(argument);
            break;

        case UpdateType.Once:
        case UpdateType.Update1:
        case UpdateType.Update10:
        case UpdateType.Update100:
            CalculationRun();
            break;
    }
}

void CalculationRun()
{
    CalculateHull();
    if (sMod != ShieldMod.None)
        CalculateShield();

    if (AlertHandler.ActiveAlert.BlinkMode == Blink.Slow)
    {
        if (DateTime.Now.Second % 2 == 0)
            foreach (var light in alertLights)
                light.Enabled = false;
        else
            foreach (var light in alertLights)
                light.Enabled = true;
    }
    else if (AlertHandler.ActiveAlert.BlinkMode == Blink.Fast)
    {
        if (DateTime.Now.Millisecond >= 500)
            foreach (var light in alertLights)
                light.Enabled = false;
        else
            foreach (var light in alertLights)
                light.Enabled = true;
    }

    if (sMod == ShieldMod.None)
        SurfaceHandler.PassData(hullPercent, null);
    else
        SurfaceHandler.PassData(hullPercent, shieldPercent, shieldEnabled);
    foreach (var screen in textSurfaces)
    {
        SurfaceHandler.DrawToScreen(screen.Surface, screen.Type);
    }

    Echo(EchoText());
}

void UserRun(string argument)
{
    switch (argument)
    {
        case "reset":
            CalculateHull(true);
            CalculateShield(true);
            ResetScript();
            break;
        case "toggle":
            switch (blastDoors[0].Status)
            {
                case DoorStatus.Open | DoorStatus.Opening:
                    autoDoorToggle = false;
                    foreach (IMyDoor door in blastDoors)
                    {
                        door.CloseDoor();
                    }
                    break;
                case DoorStatus.Closed | DoorStatus.Closing:
                    autoDoorToggle = true;
                    foreach (IMyDoor door in blastDoors)
                    {
                        door.OpenDoor();
                    }
                    break;
            }
            break;
        default:
            if (AlertHandler.IsValidAlert(argument))
                ChangeAlert(argument);
            break;
    }
}

void ChangeAlert(string argument, bool overRide = false)
{
    try
    {
        if (argument == AlertHandler.ActiveAlert.Argument && !overRide)
            return;

        AlertHandler.SetAlert(argument);
        AlertHandler.Alert alert = AlertHandler.ActiveAlert;

        foreach (IMyLightingBlock light in alertLights)
        {
            light.Color = alert.Color;
            light.Enabled = true;
        }

        if (autoDoorToggle)
            switch (alert.BlastDoors)
            {
                case Doors.Open:
                    foreach (var door in blastDoors)
                        door.OpenDoor();
                    break;
                case Doors.Close:
                    foreach (var door in blastDoors)
                        door.CloseDoor();
                    break;
            }

        if (alert.WarningSound != "")
            foreach (var speaker in soundBlocks)
            {
                speaker.SelectedSound = alert.WarningSound;
                speaker.Play();
            }
        else
            foreach (var speaker in soundBlocks)
                speaker.Stop();

        if (alert.DimLights)
            foreach (var light in interiorLights)
                light.SetValue("Intensity", dim);
        else
            foreach (var light in interiorLights)
                light.SetValue("Intensity", bright);

        if (autoToggle)
            switch (alert.ShipSystems)
            {
                case Systems.AllOffline:
                    foreach (var weapon in weapons)
                        weapon.Enabled = false;
                    foreach (var jammer in jammers)
                        jammer.Enabled = false;
                    if (sMod == ShieldMod.EnergyShield)
                        shieldBlock.Enabled = false;
                    else if (sMod == ShieldMod.DefenseShield)
                        shieldBlock.ApplyAction("DS-C_ToggleShield_Off");
                    break;
                case Systems.ShieldsOnline:
                    foreach (var weapon in weapons)
                        weapon.Enabled = false;
                    foreach (var jammer in jammers)
                        jammer.Enabled = true;
                    if (sMod == ShieldMod.EnergyShield)
                        shieldBlock.Enabled = true;
                    else if (sMod == ShieldMod.DefenseShield)
                        shieldBlock.ApplyAction("DS-C_ToggleShield_On");
                    break;
                case Systems.AllOnline:
                    foreach (var weapon in weapons)
                        weapon.Enabled = true;
                    foreach (var jammer in jammers)
                        jammer.Enabled = true;
                    if (sMod == ShieldMod.EnergyShield)
                        shieldBlock.Enabled = true;
                    else if (sMod == ShieldMod.DefenseShield)
                        shieldBlock.ApplyAction("DS-C_ToggleShield_On");
                    break;
                default:
                    break;
            }
    }
    catch (Exception)
    {
        ResetScript();
    }
}

void Setup()
{
    Echo("Setup progressing...");

    if (!Me.CustomName.Contains(selectorString))
        Me.CustomName += " (" + selectorString + ")";

    myIni.TryParse(Me.CustomData);
    myIni.Set("ScriptConfig", "version", version);
    myIni.Set("ScriptConfig", "hullBlockCount", 0);
    myIni.Set("AlertScript", "display0", true);
    myIni.Set("AlertScript", "display1", false);

    Me.CustomData = myIni.ToString();
    CalculateHull(true);
}

void ResetScript()
{
    autoDoorToggle = true;

    alertLights.Clear();
    textSurfaces.Clear();
    blastDoors.Clear();
    soundBlocks.Clear();
    interiorLights.Clear();
    weapons.Clear();

    {// get alert lights
        List<IMyTerminalBlock> groupLights = new List<IMyTerminalBlock>();
        try { GridTerminalSystem.GetBlockGroupWithName(aLightGroupName).GetBlocks(groupLights); }
        catch { }

        GridTerminalSystem.GetBlocksOfType(alertLights, IsControlledBlock);
        foreach (var light in groupLights)
        {
            if (!alertLights.Contains(light as IMyLightingBlock))
            {
                alertLights.Add(light as IMyLightingBlock);
            }
        }
    }
    {// get interior lights
        try
        {
            var intLights = GridTerminalSystem.GetBlockGroupWithName(iLightGroupName);
            intLights.GetBlocksOfType(interiorLights);
        }
        catch { }
    }
    {// get blast doors
        List<IMyTerminalBlock> groupDoors = new List<IMyTerminalBlock>();
        try { GridTerminalSystem.GetBlockGroupWithName(doorGroupName).GetBlocks(groupDoors); }
        catch { }

        GridTerminalSystem.GetBlocksOfType(blastDoors, IsControlledBlock);
        foreach (var door in groupDoors)
        {
            if (!blastDoors.Contains(door as IMyDoor))
            {
                blastDoors.Add(door as IMyDoor);
            }
        }
    }
    {// get alert sound blocks
        GridTerminalSystem.GetBlocksOfType(soundBlocks, IsControlledBlock);
    }
    {// get weapon group
        var weaponGroup = GridTerminalSystem.GetBlockGroupWithName(weaponGroupName);
        List<IMyTerminalBlock> weaponsTemp = new List<IMyTerminalBlock>();
        try
        {
            weaponGroup.GetBlocks(weaponsTemp);
        }
        catch { }
        foreach (var block in weaponsTemp)
        {
            weapons.Add(block as IMyFunctionalBlock);
        }
    }
    {// get textBlocks
        List<IMyTerminalBlock> textBlocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(textBlocks, block => IsControlledBlock(block) && SurfaceHandler.IsSurface(block.BlockDefinition.SubtypeName));
        foreach (var block in textBlocks)
        {
            Surface templateSurface = SurfaceHandler.GetSurfaceById(block.BlockDefinition.SubtypeId);
            if (!myIni.TryParse(block.CustomData) || block.CustomData == "")
            {
                myIni.Clear();
                if (templateSurface.Type == BlockType.Single)
                {
                    myIni.Set("AlertScript", "display0", true);
                }
                else
                {
                    for (int i = 0; i < templateSurface.Screens.Length; i++)
                    {
                        myIni.Set("AlertScript", $"display{i}", false);
                    }
                }
                block.CustomData = myIni.ToString();
            }

            if (templateSurface.Type == BlockType.Single)
            {
                if (myIni.Get("AlertScript", "display0").ToBoolean())
                {
                    textSurfaces.Add(new Screen(block as IMyTextSurface, templateSurface.Screens[0]));
                }
            }
            else // provider
            {
                for (int i = 0; i < templateSurface.Screens.Length; i++)
                {
                    IMyTextSurfaceProvider provider = block as IMyTextSurfaceProvider;
                    if (myIni.Get("AlertScript", $"display{i}").ToBoolean())
                    {
                        textSurfaces.Add(new Screen(provider.GetSurface(i), templateSurface.Screens[i]));
                    }
                }
            }
        }
    }

    ChangeAlert(AlertHandler.DefaultAlertArg, overRide: true);
}

void CalculateHull(bool reCount = false)
{
    List<IMyTerminalBlock> terminalBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(terminalBlocks, block => block.CubeGrid == Me.CubeGrid);
    if (reCount)
    {
        hullBlockCount = terminalBlocks.Count;
        myIni.TryParse(Me.CustomData);
        myIni.Set("ScriptConfig", "hullBlockCount", hullBlockCount);
        Me.CustomData = myIni.ToString();
    }
    hullPercent = (double)terminalBlocks.Count / hullBlockCount;
}

void CalculateShield(bool reset = false)
{
    if (reset)
    {
        List<IMyTerminalBlock> shieldGenerators = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName("Shield Generator:", shieldGenerators);

        if (shieldGenerators.Count > 0 && shieldGenerators[0].CustomName.Contains("Shield Generator:"))
        {
            // cython's energy shield mod
            sMod = ShieldMod.EnergyShield;
            shieldBlock = shieldGenerators[0] as IMyFunctionalBlock;
        }
        else
        {
            GridTerminalSystem.SearchBlocksOfName("Shield Control", shieldGenerators);

            // defense shield mod
            if (shieldGenerators.Count > 0 && shieldGenerators[0].CustomName.Contains("Shield Control"))
            {
                sMod = ShieldMod.DefenseShield;
                shieldBlock = shieldGenerators[0] as IMyFunctionalBlock;
                dShield = new DefenseShields(Me);
                if (!dShield.IsShieldBlock())
                {
                    sMod = ShieldMod.None;
                }
            }
            else
                sMod = ShieldMod.None;
        }

        if (sMod != ShieldMod.None)
        {
            List<IMyTerminalBlock> tempList = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("Transport Jammer", tempList, block => block.CubeGrid == Me.CubeGrid);
            jammers.Clear();
            foreach (var block in tempList)
                jammers.Add(block as IMyFunctionalBlock);
        }
    }

    try
    {
        switch (sMod)
        {
            case ShieldMod.EnergyShield:
                if (shieldBlock.Enabled == false)
                {
                    shieldEnabled = false;
                    shieldPercent = 0;
                }
                else
                {
                    shieldEnabled = true;
                    string[] tempStringArray = null;

                    tempStringArray = shieldBlock.CustomName.Substring(0, shieldBlock.CustomName.Length - 1).Split('(');
                    tempStringArray = tempStringArray[1].Split('/');
                    shieldPercent = Convert.ToDouble(tempStringArray[0]) / Convert.ToDouble(tempStringArray[1]);
                }
                break;
            case ShieldMod.DefenseShield:
                shieldPercent = dShield.GetCharge() / dShield.GetMaxCharge();
                if (dShield.IsShieldUp())
                    shieldEnabled = true;
                else
                {
                    shieldEnabled = false;
                    shieldPercent = 0;
                }

                break;
            case ShieldMod.None:
                break;
        }
    }
    catch (Exception)
    {
        CalculateShield(true);
    }
}

string EchoText()
{
    string symbolText;
    if (DateTime.Now.Second % 2 == 0)
        symbolText = "[#  ]";
    else
        symbolText = "[  #]";

    string echoText =
        "Red Alert Script Active " + symbolText +
        "\nCurrent alert: " + AlertHandler.ActiveAlert.Argument +
        "\n" +
        "\nTerminal blocks: " + hullBlockCount +
        "\nShield mod: " + sMod +
        "\n" +
        "\nAlert lights found: " + alertLights.Count +
        "\nInterior lights found: " + interiorLights.Count +
        "\nText surfaces: " + textSurfaces.Count +
        "\nBlast doors found: " + blastDoors.Count +
        "\nSoundblocks found: " + soundBlocks.Count +
        "\nWeapons found: " + weapons.Count;

    return echoText;
}

bool IsControlledBlock(IMyTerminalBlock block)
{
    return block.CubeGrid == Me.CubeGrid && block.CustomName.Contains(selectorString);
}

static class AlertHandler
{
    static List<Alert> Alerts = new List<Alert>();
    public static Alert ActiveAlert { get; private set; }
    public static string DefaultAlertArg { get; }

    static AlertHandler()
    {
        Alerts.Add(new Alert("normal", "Normal mode", new Color(150, 200, 255), Blink.AlwaysOn, false, Doors.Open, "", Systems.AllOffline));
        Alerts.Add(new Alert("yellow", "Yellow alert", new Color(255, 200, 50), Blink.Slow, false, Doors.Open, "SoundBlockAlert1", Systems.ShieldsOnline));
        Alerts.Add(new Alert("red", "Red alert", new Color(255, 15, 15), Blink.Slow, true, Doors.Close, "SoundBlockAlert2", Systems.AllOnline));
        Alerts.Add(new Alert("green", "Green alert", new Color(100, 255, 100), Blink.Slow, false, Doors.NoInteract, "", Systems.NoInteract));
        Alerts.Add(new Alert("blue", "Blue alert", new Color(0, 100, 255), Blink.Slow, false, Doors.NoInteract, "", Systems.AllOffline));
        Alerts.Add(new Alert("black", "Black alert", new Color(75, 0, 255), Blink.Slow, true, Doors.NoInteract, "", Systems.NoInteract));
        Alerts.Add(new Alert("grey", "Silent running", new Color(75, 75, 75), Blink.AlwaysOn, true, Doors.NoInteract, "", Systems.AllOffline));
        Alerts.Add(new Alert("intruder", "Intruder alert", new Color(255, 25, 0), Blink.Fast, false, Doors.Close, "SoundBlockAlert3", Systems.AllOnline));
        Alerts.Add(new Alert("engineering", "Failures detected", new Color(255, 150, 20), Blink.AlwaysOn, false, Doors.NoInteract, "", Systems.AllOffline));
        ActiveAlert = Alerts[0];
        DefaultAlertArg = Alerts[0].Argument;
    }

    public static void AddCustomAlert(string argument, string displayText, Color color, Blink blinkMode, bool dimLights, Doors doors, string sound, Systems systems)
    {
        Alerts.Add(new Alert(argument, displayText, color, blinkMode, dimLights, doors, sound, systems));
    }

    public static bool IsValidAlert(string argument)
    {
        for (int i = 0; i < Alerts.Count; i++)
        {
            if (argument == Alerts[i].Argument)
            {
                return true;
            }
        }
        return false;
    }

    public static void SetAlert(string argument)
    {
        for (int i = 0; i < Alerts.Count; i++)
        {
            if (argument == Alerts[i].Argument)
            {
                ActiveAlert = Alerts[i];
            }
        }
    }

    public struct Alert
    {
        public string Argument { get; }
        public string DisplayText { get; }
        public Color Color { get; }
        public Blink BlinkMode { get; }
        public bool DimLights { get; }
        public Doors BlastDoors { get; }
        public string WarningSound { get; }
        public Systems ShipSystems { get; }

        public Alert(string argument, string name, Color lightColor, Blink blinkMode, bool dimLights, Doors closeBlastDoors, string sound, Systems systems)
        {
            Argument = argument;
            DisplayText = name;
            Color = lightColor;
            BlinkMode = blinkMode;
            DimLights = dimLights;
            BlastDoors = closeBlastDoors;
            WarningSound = sound;
            ShipSystems = systems;
        }
    }
}

static class SurfaceHandler
{
    static readonly List<Surface> surfaces = new List<Surface>();
    static int hullPercent = 0;
    static int shieldPercent = 0;
    static bool hasShield = false;
    static bool shieldEnabled = false;
    static Vector2 ZeroPoint;
    static Color HullColor
    {
        get
        {
            if (hullPercent >= 25)
                return hullColorH;
            else
                return hullColorL;
        }
    }
    static Color ShieldColor
    {
        get
        {
            if (!shieldEnabled || shieldPercent >= 25)
                return shieldColorH;
            else
                return shieldColorL;
        }
    }
    static readonly Color hullColorH = new Color(60, 255, 95);
    static readonly Color hullColorL = new Color(255, 50, 50);
    static readonly Color shieldColorH = new Color(85, 210, 255);
    static readonly Color shieldColorL = new Color(255, 100, 50);

    static SurfaceHandler()
    {
        /* Control Station */
        surfaces.Add(new Surface("LargeBlockCockpit", 1));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Large);
        /* Flight Seat */
        surfaces.Add(new Surface("CockpitOpen", 1));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Wide);
        /* Cockpit */
        surfaces.Add(new Surface("LargeBlockCockpitSeat", 6));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(2, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(3, ScreenType.Keyboard);
        surfaces[surfaces.Count - 1].SetScreen(4, ScreenType.Tiny);
        surfaces[surfaces.Count - 1].SetScreen(5, ScreenType.Tiny);
        /* Industrial Cockpit */
        surfaces.Add(new Surface("LargeBlockCockpitIndustrial", 6));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(2, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(3, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(4, ScreenType.Keyboard);
        surfaces[surfaces.Count - 1].SetScreen(5, ScreenType.Tiny);
        /* Console */
        surfaces.Add(new Surface("LargeBlockConsole", 4));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Large);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(2, ScreenType.Tiny);
        surfaces[surfaces.Count - 1].SetScreen(3, ScreenType.Keyboard);
        /* Programmable Block */
        surfaces.Add(new Surface("LargeProgrammableBlock", 2));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Medium);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Keyboard);
        /* Store Block */
        surfaces.Add(new Surface("StoreBlock", 2));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Keyboard);
        /* Contracts Block */
        surfaces.Add(new Surface("ContractBlock", 2));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Keyboard);
        /* Safe Zone Block */
        surfaces.Add(new Surface("SafeZoneBlock", 2));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Keyboard);
        /* LCD Panel */
        surfaces.Add(new Surface("LargeLCDPanel", ScreenType.Large));
        /* Text Panel */
        surfaces.Add(new Surface("LargeTextPanel", ScreenType.Large));
        /* Wide LCD Panel */
        surfaces.Add(new Surface("LargeLCDPanelWide", ScreenType.Large));
        /* Corner LCD Top */
        surfaces.Add(new Surface("LargeBlockCorner_LCD_1", ScreenType.Corner));
        /* Corner LCD Bottom */
        surfaces.Add(new Surface("LargeBlockCorner_LCD_2", ScreenType.Corner));
        /* Corner LCD Flat Top */
        surfaces.Add(new Surface("LargeBlockCorner_LCD_Flat_1", ScreenType.Corner));
        /* Corner LCD Flat Bottom */
        surfaces.Add(new Surface("LargeBlockCorner_LCD_Flat_2", ScreenType.Corner));
		/* Inset LCD Panel */
        surfaces.Add(new Surface("LargeFullBlockLCDPanel", ScreenType.Large));
		/* Diagonal LCD Panel */
        surfaces.Add(new Surface("LargeDiagonalLCDPanel", ScreenType.Large));
		/* Curved LCD Panel */
        surfaces.Add(new Surface("LargeCurvedLCDPanel", ScreenType.Large));
		/* Holo LCD */
        surfaces.Add(new Surface("HoloLCDLarge", ScreenType.Large));
		/* Transparent LCD */
        surfaces.Add(new Surface("TransparentLCDLarge", ScreenType.Large));
		/* LargeLCDPanel5x3 */
        surfaces.Add(new Surface("LargeLCDPanel5x3", ScreenType.Large));
		/* LargeLCDPanel3x3 */
        surfaces.Add(new Surface("LargeLCDPanel3x3", ScreenType.Large));
		/* LargeLCDPanel5x5 */
        surfaces.Add(new Surface("LargeLCDPanel5x5", ScreenType.Large));

        /* Cockpit (small) */
        surfaces.Add(new Surface("SmallBlockCockpit", 4));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Medium);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Medium);
        surfaces[surfaces.Count - 1].SetScreen(2, ScreenType.Medium);
        surfaces[surfaces.Count - 1].SetScreen(3, ScreenType.Small);
        /* Industrial Cockpit (small) */
        surfaces.Add(new Surface("SmallBlockCockpitIndustrial", 5));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Medium);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Medium);
        surfaces[surfaces.Count - 1].SetScreen(2, ScreenType.Medium);
        surfaces[surfaces.Count - 1].SetScreen(3, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(4, ScreenType.Tiny);
        /* Fighter Cockpit */
        surfaces.Add(new Surface("DBSmallBlockFighterCockpit", 6));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Tiny);
        surfaces[surfaces.Count - 1].SetScreen(2, ScreenType.Tiny);
        surfaces[surfaces.Count - 1].SetScreen(3, ScreenType.Keyboard);
        surfaces[surfaces.Count - 1].SetScreen(4, ScreenType.Tiny);
        surfaces[surfaces.Count - 1].SetScreen(5, ScreenType.Tiny);
        /* Programmable Block (small) */
        surfaces.Add(new Surface("SmallProgrammableBlock", 2));
        surfaces[surfaces.Count - 1].SetScreen(0, ScreenType.Small);
        surfaces[surfaces.Count - 1].SetScreen(1, ScreenType.Keyboard);
        /* LCD Panel (small) */
        surfaces.Add(new Surface("SmallLCDPanel", ScreenType.Large));
        /* Text Panel (small) */
        surfaces.Add(new Surface("SmallTextPanel", ScreenType.Medium));
        /* Wide LCD Panel (small) */
        surfaces.Add(new Surface("SmallLCDPanelWide", ScreenType.Large));
        /* Corner LCD Top (small) */
        surfaces.Add(new Surface("SmallBlockCorner_LCD_1", ScreenType.Small));
        /* Corner LCD Bottom (small) */
        surfaces.Add(new Surface("SmallBlockCorner_LCD_2", ScreenType.Small));
        /* Corner LCD Flat Top (small) */
        surfaces.Add(new Surface("SmallBlockCorner_LCD_Flat_1", ScreenType.Small));
        /* Corner LCD Flat Bottom (small) */
        surfaces.Add(new Surface("SmallBlockCorner_LCD_Flat_2", ScreenType.Small));
    }

    public static bool IsSurface(string id)
    {
        for (int i = 0; i < surfaces.Count; i++)
        {
            if (surfaces[i].Id == id)
            {
                return true;
            }
        }
        return false;
    }

    public static Surface GetSurfaceById(string id)
    {
        for (int i = 0; i < surfaces.Count; i++)
        {
            if (surfaces[i].Id == id)
            {
                return surfaces[i];
            }
        }
        return null;
    }

    public static void PassData(double hull, double? shield, bool enabled = true)
    {
        hullPercent = Convert.ToInt32(hull * 100);
        if (shield == null)
        {
            hasShield = false;
            shieldEnabled = false;
            shieldPercent = 0;
        }
        else
        {
            hasShield = true;
            shieldEnabled = enabled;
            shieldPercent = Convert.ToInt32(shield * 100);
        }
    }

    public static void DrawToScreen(IMyTextSurface surface, ScreenType screenType)
    {
        surface.ContentType = ContentType.SCRIPT;
        surface.ScriptBackgroundColor = new Color(0, 0, 0);

        ZeroPoint = new Vector2(0, surface.TextureSize.Y / 2 - surface.SurfaceSize.Y / 2);
        using (var frame = surface.DrawFrame())
        {
            if (screenType == ScreenType.Large || screenType == ScreenType.Medium)
            {
                Vector2 position = new Vector2(surface.SurfaceSize.X * 0.5f, surface.SurfaceSize.Y * 0.55f);
                Vector2 size = new Vector2(surface.SurfaceSize.X * 0.6f, surface.SurfaceSize.Y * 0.05f);
                AddBarToFrame(frame, position, size, hullPercent, HullColor);
                position.Y = surface.SurfaceSize.Y * 0.7f;
                AddBarToFrame(frame, position, size, shieldPercent, ShieldColor);

                position = new Vector2(surface.SurfaceSize.X * 0.1f, surface.SurfaceSize.Y * 0.55f);
                AddTextToFrame(frame, position, "H", HullColor, 1.5f);
                position = new Vector2(surface.SurfaceSize.X * 0.9f, surface.SurfaceSize.Y * 0.55f);
                AddTextToFrame(frame, position, hullPercent.ToString(), HullColor, 1.5f);
                position = new Vector2(surface.SurfaceSize.X * 0.1f, surface.SurfaceSize.Y * 0.7f);
                AddTextToFrame(frame, position, "S", ShieldColor, 1.5f);
                position = new Vector2(surface.SurfaceSize.X * 0.9f, surface.SurfaceSize.Y * 0.7f);
                AddTextToFrame(frame, position, ShieldPercent(), ShieldColor, 1.5f);

                position = new Vector2(surface.SurfaceSize.X * 0.5f, surface.SurfaceSize.Y * 0.3f);
                AddTextToFrame(frame, position, AlertHandler.ActiveAlert.DisplayText, AlertHandler.ActiveAlert.Color, 2.5f);

                AddWaveAnimationToFrame(frame, surface.SurfaceSize);
            }
            else if (screenType == ScreenType.Small)
            {
                Vector2 position = new Vector2(surface.SurfaceSize.X * 0.5f, surface.SurfaceSize.Y * 0.55f);
                Vector2 size = new Vector2(surface.SurfaceSize.X * 0.6f, surface.SurfaceSize.Y * 0.075f);
                AddBarToFrame(frame, position, size, hullPercent, HullColor);
                position.Y = surface.SurfaceSize.Y * 0.725f;
                AddBarToFrame(frame, position, size, shieldPercent, ShieldColor);

                position = new Vector2(surface.SurfaceSize.X * 0.1f, surface.SurfaceSize.Y * 0.55f);
                AddTextToFrame(frame, position, "H", HullColor, 1f);
                position = new Vector2(surface.SurfaceSize.X * 0.9f, surface.SurfaceSize.Y * 0.55f);
                AddTextToFrame(frame, position, hullPercent.ToString(), HullColor, 1f);
                position = new Vector2(surface.SurfaceSize.X * 0.1f, surface.SurfaceSize.Y * 0.725f);
                AddTextToFrame(frame, position, "S", ShieldColor, 1f);
                position = new Vector2(surface.SurfaceSize.X * 0.9f, surface.SurfaceSize.Y * 0.725f);
                AddTextToFrame(frame, position, ShieldPercent(), ShieldColor, 1f);

                position = new Vector2(surface.SurfaceSize.X * 0.5f, surface.SurfaceSize.Y * 0.3f);
                AddTextToFrame(frame, position, AlertHandler.ActiveAlert.DisplayText, AlertHandler.ActiveAlert.Color, 1.25f);

                AddWaveAnimationToFrame(frame, surface.SurfaceSize);
            }
            else if (screenType == ScreenType.Keyboard)
            {
                Vector2 position = new Vector2(surface.SurfaceSize.X * 0.5f, surface.SurfaceSize.Y * 0.5f);
                Vector2 size = new Vector2(surface.SurfaceSize.X * 0.9f, surface.SurfaceSize.Y * 0.1f);
                AddBlinkingSquareToFrame(frame, position, size);

                position.Y = surface.SurfaceSize.Y * 0.20f;
                AddTextToFrame(frame, position, AlertHandler.ActiveAlert.DisplayText, AlertHandler.ActiveAlert.Color, 1.35f);

                position = new Vector2(surface.SurfaceSize.X * 0.3f, surface.SurfaceSize.Y * 0.80f);
                AddTextToFrame(frame, position, "H: " + hullPercent, HullColor, 1.15f);
                position.X = surface.SurfaceSize.X * 0.7f;
                AddTextToFrame(frame, position, "S: " + ShieldPercent(), ShieldColor, 1.15f);
            }
            else if (screenType == ScreenType.Tiny)
            {
                Vector2 position = surface.SurfaceSize / 2f;
                Vector2 size = surface.SurfaceSize * 0.85f;
                AddBlinkingSquareToFrame(frame, position, size);
            }
            else if (screenType == ScreenType.Corner)
            {
                Vector2 position = new Vector2(surface.SurfaceSize.X * 0.045f, surface.SurfaceSize.Y * 0.5f);
                Vector2 size = new Vector2(surface.SurfaceSize.X * 0.05f, surface.SurfaceSize.Y * 0.9f);
                AddBlinkingSquareToFrame(frame, position, size);
                position.X = surface.SurfaceSize.X * 0.955f;
                AddBlinkingSquareToFrame(frame, position, size);

                position = new Vector2(surface.SurfaceSize.X * 0.5f, surface.SurfaceSize.Y * 0.3f);
                size = new Vector2(surface.SurfaceSize.X * 0.6f, surface.SurfaceSize.Y * 0.2f);
                AddBarToFrame(frame, position, size, hullPercent, HullColor);
                position.Y = surface.SurfaceSize.Y * 0.7f;
                AddBarToFrame(frame, position, size, shieldPercent, ShieldColor);

                position = new Vector2(surface.SurfaceSize.X * 0.135f, surface.SurfaceSize.Y * 0.3f);
                AddTextToFrame(frame, position, "H", HullColor, 1.0f);
                position = new Vector2(surface.SurfaceSize.X * 0.865f, surface.SurfaceSize.Y * 0.3f);
                AddTextToFrame(frame, position, hullPercent.ToString(), HullColor, 1.0f);
                position = new Vector2(surface.SurfaceSize.X * 0.135f, surface.SurfaceSize.Y * 0.7f);
                AddTextToFrame(frame, position, "S", ShieldColor, 1.0f);
                position = new Vector2(surface.SurfaceSize.X * 0.865f, surface.SurfaceSize.Y * 0.7f);
                AddTextToFrame(frame, position, ShieldPercent(), ShieldColor, 1.0f);
            }
            else if (screenType == ScreenType.Wide)
            {
                Vector2 position = new Vector2(surface.SurfaceSize.X * 0.045f, surface.SurfaceSize.Y * 0.5f);
                Vector2 size = new Vector2(surface.SurfaceSize.X * 0.05f, surface.SurfaceSize.Y * 0.85f);
                AddBlinkingSquareToFrame(frame, position, size);
                position.X = surface.SurfaceSize.X * 0.955f;
                AddBlinkingSquareToFrame(frame, position, size);

                position = new Vector2(surface.SurfaceSize.X * 0.5f, surface.SurfaceSize.Y * 0.3f);
                size = new Vector2(surface.SurfaceSize.X * 0.6f, surface.SurfaceSize.Y * 0.2f);
                AddBarToFrame(frame, position, size, hullPercent, HullColor);
                position.Y = surface.SurfaceSize.Y * 0.7f;
                AddBarToFrame(frame, position, size, shieldPercent, ShieldColor);

                position = new Vector2(surface.SurfaceSize.X * 0.135f, surface.SurfaceSize.Y * 0.3f);
                AddTextToFrame(frame, position, "H", HullColor, 1.15f);
                position = new Vector2(surface.SurfaceSize.X * 0.865f, surface.SurfaceSize.Y * 0.3f);
                AddTextToFrame(frame, position, hullPercent.ToString(), HullColor, 1.15f);
                position = new Vector2(surface.SurfaceSize.X * 0.135f, surface.SurfaceSize.Y * 0.7f);
                AddTextToFrame(frame, position, "S", ShieldColor, 1.15f);
                position = new Vector2(surface.SurfaceSize.X * 0.865f, surface.SurfaceSize.Y * 0.7f);
                AddTextToFrame(frame, position, ShieldPercent(), ShieldColor, 1.15f);
            }
        }
    }

    static void AddBlinkingSquareToFrame(MySpriteDrawFrame frame, Vector2 position, Vector2 size)
    {
        MySprite rectangle = new MySprite(SpriteType.TEXTURE, "SquareSimple")
        {
            Position = ZeroPoint + position,
            Size = size
        };
        if (AlertHandler.ActiveAlert.BlinkMode == Blink.Slow)
        {
            if (DateTime.Now.Second % 2 == 0)
                rectangle.Color = Darken(AlertHandler.ActiveAlert.Color);
            else
                rectangle.Color = AlertHandler.ActiveAlert.Color;
        }
        else if (AlertHandler.ActiveAlert.BlinkMode == Blink.Fast)
        {
            if (DateTime.Now.Millisecond >= 500)
                rectangle.Color = Darken(AlertHandler.ActiveAlert.Color);
            else
                rectangle.Color = AlertHandler.ActiveAlert.Color;
        }
        else
            rectangle.Color = AlertHandler.ActiveAlert.Color;

        frame.Add(rectangle);
    }

    static void AddTextToFrame(MySpriteDrawFrame frame, Vector2 position, string txt, Color color, float scale)
    {
        MySprite text = MySprite.CreateText(txt, "Debug", color, scale);
        text.Position = ZeroPoint + new Vector2(position.X, position.Y - scale * 15.5f);
        frame.Add(text);
    }

    static void AddBarToFrame(MySpriteDrawFrame frame, Vector2 position, Vector2 size, int percent, Color color)
    {
        MySprite bar = new MySprite(SpriteType.TEXTURE, "SquareSimple");
        bar.Position = ZeroPoint + position;
        bar.Size = size;
        bar.Color = Darken(color);
        frame.Add(bar);

        bar.Position = ZeroPoint + new Vector2(position.X - size.X * ((100 - percent) / 100f) / 2, position.Y);
        bar.Size = new Vector2(size.X * (percent / 100f), size.Y);
        bar.Color = color;
        frame.Add(bar);
    }

    static void AddWaveAnimationToFrame(MySpriteDrawFrame frame, Vector2 surfaceSize)
    {
        MySprite rectangle = new MySprite(SpriteType.TEXTURE, "SquareSimple");
        if (AlertHandler.ActiveAlert.BlinkMode == Blink.AlwaysOn)
        {
            rectangle.Size = new Vector2(surfaceSize.X * 0.955f, surfaceSize.Y * 0.08f);
            rectangle.Color = AlertHandler.ActiveAlert.Color;
            rectangle.Position = ZeroPoint + new Vector2(surfaceSize.X * 0.5f, surfaceSize.Y * 0.0625f);
            frame.Add(rectangle);
            rectangle.Position = ZeroPoint + new Vector2(surfaceSize.X * 0.5f, surfaceSize.Y * 0.9375f);
            frame.Add(rectangle);
            return;
        }

        int state = State();
        for (int i = 0; i < 4; i++)
        {
            float xCoordinate = surfaceSize.X * 0.5625f + surfaceSize.X * i * 0.125f;
            rectangle.Size = new Vector2(surfaceSize.X * 0.08f, surfaceSize.Y * 0.08f);
            if (i == state)
                rectangle.Color = AlertHandler.ActiveAlert.Color;
            else
                rectangle.Color = Darken(AlertHandler.ActiveAlert.Color);

            rectangle.Position = ZeroPoint + new Vector2(xCoordinate, surfaceSize.Y * 0.0625f);
            frame.Add(rectangle);
            rectangle.Position = ZeroPoint + new Vector2(surfaceSize.X - xCoordinate, surfaceSize.Y * 0.0625f);
            frame.Add(rectangle);
            rectangle.Position = ZeroPoint + new Vector2(xCoordinate, surfaceSize.Y * 0.9375f);
            frame.Add(rectangle);
            rectangle.Position = ZeroPoint + new Vector2(surfaceSize.X - xCoordinate, surfaceSize.Y * 0.9375f);
            frame.Add(rectangle);
        }
    }

    static string ShieldPercent()
    {
        if (hasShield)
            if (shieldEnabled)
                return shieldPercent.ToString();
            else
                return "OFF";
        else
            return "---";
    }

    static int State()
    {
        if (AlertHandler.ActiveAlert.BlinkMode == Blink.Slow)
        {
            if (DateTime.Now.Second % 2 == 0)
                if (DateTime.Now.Millisecond < 400)
                    return 0;
                else if (DateTime.Now.Millisecond < 800)
                    return 1;
                else
                    return 2;
            else
                if (DateTime.Now.Millisecond < 200)
                return 2;
            else if (DateTime.Now.Millisecond < 600)
                return 3;
            else
                return 4;
        }
        else
        {
            if (DateTime.Now.Millisecond < 200)
                return 0;
            else if (DateTime.Now.Millisecond < 400)
                return 1;
            else if (DateTime.Now.Millisecond < 600)
                return 2;
            else if (DateTime.Now.Millisecond < 800)
                return 3;
            else
                return 4;
        }
    }

    static Color Darken(Color c)
    {
        return new Color(Convert.ToInt32(c.R * 0.1f), Convert.ToInt32(c.G * 0.1f), Convert.ToInt32(c.B * 0.1f));
    }
}

class Surface
{
    public string Id;
    public BlockType Type;
    public ScreenType[] Screens;

    public Surface(string block, int numOfScreens)
    {
        Id = block;
        Type = BlockType.Provider;
        Screens = new ScreenType[numOfScreens];
    }

    public Surface(string block, ScreenType screenType)
    {
        Id = block;
        Type = BlockType.Single;
        Screens = new ScreenType[1];
        Screens[0] = screenType;
    }

    public void SetScreen(int num, ScreenType screenType)
    {
        Screens[num] = screenType;
    }
}

struct Screen
{
    public IMyTextSurface Surface { get; }
    public ScreenType Type { get; }

    public Screen(IMyTextSurface ts, ScreenType t)
    {
        Surface = ts;
        Type = t;
    }
}

class DefenseShields
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

    public DefenseShields(IMyTerminalBlock block)
    {
        _block = block;
        var delegates = _block.GetProperty("DefenseSystemsPbAPI")?.As<Dictionary<string, Delegate>>().GetValue(_block);
        if (delegates == null)
            return;

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
        _gridHasShield = (Func<IMyCubeGrid, bool>)delegates["GridHasShield"];
        _gridShieldOnline = (Func<IMyCubeGrid, bool>)delegates["GridShieldOnline"];
        _protectedByShield = (Func<IMyEntity, bool>)delegates["ProtectedByShield"];
        _getShieldBlock = (Func<IMyEntity, IMyTerminalBlock>)delegates["GetShieldBlock"];
        _isShieldBlock = (Func<IMyTerminalBlock, bool>)delegates["IsShieldBlock"];
        _getClosestShield = (Func<Vector3D, IMyTerminalBlock>)delegates["GetClosestShield"];
        _getDistanceToShield = (Func<IMyTerminalBlock, Vector3D, double>)delegates["GetDistanceToShield"];
        _getClosestShieldPoint = (Func<IMyTerminalBlock, Vector3D, Vector3D?>)delegates["GetClosestShieldPoint"];

        if (!IsShieldBlock())
            _block = GetShieldBlock(_block.CubeGrid) ?? _block;
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
    public bool GridHasShield(IMyCubeGrid grid) => _gridHasShield?.Invoke(grid) ?? false;
    public bool GridShieldOnline(IMyCubeGrid grid) => _gridShieldOnline?.Invoke(grid) ?? false;
    public bool ProtectedByShield(IMyEntity entity) => _protectedByShield?.Invoke(entity) ?? false;
    public IMyTerminalBlock GetShieldBlock(IMyEntity entity) => _getShieldBlock?.Invoke(entity) ?? null;
    public bool IsShieldBlock() => _isShieldBlock?.Invoke(_block) ?? false;
    public IMyTerminalBlock GetClosestShield(Vector3D pos) => _getClosestShield?.Invoke(pos) ?? null;
    public double GetDistanceToShield(Vector3D pos) => _getDistanceToShield?.Invoke(_block, pos) ?? -1;
    public Vector3D? GetClosestShieldPoint(Vector3D pos) => _getClosestShieldPoint?.Invoke(_block, pos) ?? null;
}

#region variables
enum BlockType { Single, Provider }
enum ScreenType { Large, Medium, Small, Keyboard, Tiny, Corner, Wide }
enum Doors { Open, NoInteract, Close }
enum Blink { AlwaysOn, Slow, Fast }
enum Systems { AllOffline, ShieldsOnline, AllOnline, NoInteract }
enum ShieldMod { EnergyShield, DefenseShield, None }

int hullBlockCount = 0;
double hullPercent = 0;
double shieldPercent = 0;
bool shieldEnabled = true;
bool autoDoorToggle = true;
readonly double version = 2.1d;
readonly MyIni myIni = new MyIni();

IMyFunctionalBlock shieldBlock;
DefenseShields dShield;
ShieldMod sMod;

readonly List<IMyLightingBlock> alertLights = new List<IMyLightingBlock>();
readonly List<IMyLightingBlock> interiorLights = new List<IMyLightingBlock>();
readonly List<Screen> textSurfaces = new List<Screen>();
readonly List<IMyDoor> blastDoors = new List<IMyDoor>();
readonly List<IMySoundBlock> soundBlocks = new List<IMySoundBlock>();
readonly List<IMyFunctionalBlock> weapons = new List<IMyFunctionalBlock>();
readonly List<IMyFunctionalBlock> jammers = new List<IMyFunctionalBlock>();
#endregion