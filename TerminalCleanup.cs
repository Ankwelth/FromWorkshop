/*
    --- Terminal Cleanup ---

    Hides grouped blocks, renames certain blocks to clearer versions,
    removes numbers from blocks (suffix), and sets rational defaults.

    Script configuration is stored in the custom data of the programmable block.
    Config is read at "Run" time and may be changed without recompilation.
    Defaults are set (if necessary) at compilation time.

    To reset to defaults simply clear/delete custom data.


    Version:    1.7.1

    Change log:
                1.7.1
                - Minor release,  a few more mod blocks added.
                - Fixed an ini formatting issue.

                1.7
                - Added DisableUseParkingOnConnector, and DisableUseParkingOnLandingGear
                -- These two new keys allow you to disable the "Use for Parking" feature
                -- on these blocks.   Remember Kids; Pressing P is Bad.

                {Snipped for brevity, see steam page for full changelog}
 
                1.0
                - Original Script by Gohia
                https://steamcommunity.com/sharedfiles/filedetails/?id=1336195185
*/


/// <summary>
        /// IDs of blocks which are controllable; blocks that have a control/view button,
        /// which should never be hidden from the terminal or toolbar.
        /// </summary>
private static readonly HashSet<string> ControllableBlockIds = new HashSet<string>
{
    "LargeCameraBlock",
    "LargeProjector",
    "LargeProgrammableBlock",
    "LargeBlockRemoteControl",
    "LargeInteriorTurret",
    "LargeGatlingTurret",
    "LargeMissileTurret",
    "LargeMissileLauncher",
    "LargeWarhead",
    "LargeJumpDrive",

    "SmallCameraBlock",
    "SmallProjector",
    "SmallProgrammableBlock",
    "SmallBlockRemoteControl",
    "SmallGatlingTurret",
    "SmallMissileTurret",
    "SmallWarhead"
};

/// <summary>
        /// IDs of blocks which are passive; blocks that have no special UI in the terminal,
        /// which can always be hidden.
        /// </summary>
private static readonly HashSet<string> PassiveBlockIds = new HashSet<string>
{
    //"LargeBlockLargeContainer",
    //"LargeBlockSmallContainer",
    "LargeEnergyModule",
    "LargeEffectivenessModule",
    "LargeProductivityModule",
    "ShieldCapacitor",
    "ShieldFluxCoil",
    "LargeBlockSpaceBall",
    "LargeBlockBed",
    "LargeBlockFrontLight",
    "SmallLight",
    "LargeBlockLight_1corner",
    "LargeBlockLight_2corner",
    "RotatingLightLarge",
    "ControlPanel",
    "LargeBlockSolarPanel",
    "VirtualMassLarge",
    "LargeDecoy",
    "LargeBlockBathroom",
    "LargeBlockBathroomOpen",
    "LargeBlockToilet",

    //"SmallBlockLargeContainer",
    //"SmallBlockMediumContainer",
    //"SmallBlockSmallContainer",
    "SmallBlockFrontLight",
    "SmallBlockSmallLight",
    "SmallBlockLight_1corner",
    "SmallBlockLight_2corner",
    "RotatingLightSmall",
    "SmallControlPanel",
    "SmallBlockSolarPanel",
    "VirtualMassSmall",
    "SmallDecoy"

    //
    // Mod Blocks
    //
};

/// <summary>
        /// IDs of blocks which have useless inventories that can be hidden.
        /// </summary>
private static readonly HashSet<string> HideInventoryBlockIds = new HashSet<string>
{
    //
    // Large Grid
    "OpenCockpitLarge",
    "CockpitOpen",
    "PassengerSeatLarge",
    "LargeBlockCockpitSeat",
    "LargeBlockCockpit",
    "LargeBlockCockpitIndustrial",
    "LargeBlockLockerRoom",
    "LargeBlockLockers",
    "LargeBlockLockerRoomCorner",
    "LargeBlockBed",
    "LargeBlockCryoChamber",
    "LargeShipGrinder",
    "LargeShipWelder",

    //
    // Small Grid
    "SmallBlockCockpitSeat",
    "SmallBlockCockpit",
    "SmallBlockCockpitIndustrial",
    "SmallShipGrinder",
    "SmallShipWelder",

    //
    // Mod blocks
    //

    //
    // Large Grid
    "LargeShipSmallShieldGeneratorBase",
    "LargeShipLargeShieldGeneratorBase",

    //
    // Small Grid
    "SmallShipSmallShieldGeneratorBase",
    "SmallShipLargeShieldGeneratorBase"
};

/// <summary>
        /// Blocks to be hidden from Toolbars
        /// </summary>
private static readonly HashSet<string> HideToolbarConfigBlockIds = new HashSet<string>
{
    "SmallLight",
    "SmallBlockSmallLight",

    //
    // Mod Blocks
    //

    //
    // Rebel Lights
    "LargeBlockFrontLight",
    "SmallBlockFrontLight",
    "LargeBlockLight_1corner",
    "LargeBlockLight_2corner",
    "SmallBlockLight_1corner",
    "SmallBlockLight_2corner",
    "LargeLightPanel",
    "SmallLightPanel",
    "SmallSearchlight",
    "LargeSearchlight",
    "Long_Interior_Light_Center",
    "Long_Interior_Light_SB",
    "Long_Interior_Light_Corner",
    "Long_Interior_Light_Double",
    "Long_Interior_Light_Diagonal",
    "Long_Interior_Light_Diagonal_Mirrored",
    "InteriorLightBulb",
    "SmallLightPole",
    "SmallLightPoleCorner",
    "SmallLightPoleDouble",
    "MediumLightPole",
    "LargeWorkLight",
    "LargeLightPole",
    "CleanSpotlight_LB",
    "CleanSpotlight_SB",
    "CleanSpotlightSlope_LB",
    "CleanSpotlightSlope_SB",
    "CleanSpotlightSlope2_LB",
    "CleanSpotlightSlope2_SB",
    "CleanSpotlightSlanted_LB",
    "CleanSpotlightSlanted_SB",
    "CleanSpotlightSlanted2_LB",
    "CleanSpotlightSlanted2_SB",
    "SlimSpotlight_LB",
    "SlimSpotlight_SB",
    "SmallFloodLight_LB",
    "SmallFloodLight_SB",
    "RoundInteriorLightOffset",
    "RoundInteriorLight_SB",
    "RoundInteriorLight",

    "InductionCharger",
};

/// <summary>
        /// Search & Replace
        /// </summary>
private static readonly Dictionary<string, string> SearchReplaceDict = new Dictionary<string, string>
{
    //
    // Utility Blocks

    { "Programmable Block",                 "CPU" },
    { "Timer Block",                        "Timer" },

    //
    // LCDs
    { "Wide LCD Panel",                     "LCD Panel, Wide" },
    { "Corner LCD Top",                     "LCD, Corner, Top" },
    { "Corner LCD Bottom",                  "LCD, Corner, Bottom" },

    //
    // Lights
    { "Interior Light",                     "Light, Interior" },

    //
    // Mechanics
    { "Advanced Rotor",                     "Rotor, Advanced" },
    { "Sliding Door",                       "Door, Sliding" },

    //
    // Thrusters

    { "Ion Thrusters",                      "Ion Thruster" },
    { "Large Ion Thruster",                 "Ion Thruster, Large" },
    { "Atmospheric Thrusters",              "Atmospheric Thruster" },
    { "Large Atmospheric Thruster",         "Atmospheric Thruster, Large" },
    { "Hydrogen Thrusters",                 "Hydrogen Thruster" },
    { "Large Hydrogen Thruster",            "Hydrogen Thruster, Large" },

    //
    // Weapons

    { "Reloadable Rocket Launcher",         "Rocket Launcher, Reloadable" },

    //
    // Cargo Containers

    { "Large Cargo Container",              "Cargo Container, Large" },
    { "Medium Cargo Container",             "Cargo Container, Medium" },
    { "Small Cargo Container",              "Cargo Container, Small" },

    //
    // Reactors

    { "Large Reactor",                      "Reactor, Large" },
    { "Small Reactor",                      "Reactor, Small" },

    //
    // DLC Items
    //

    // DLC - Heavy Industry
    { "Industrial Assembler",               "Assembler" },
    { "Industrial Refinery",                "Refinery" },
    { "Industrial Hydrogen Tank",           "Hydrogen Tank" },
    { "Industrial Cockpit",                 "Cockpit" },
    { "Large Industrial Cargo Container",   "Cargo Container, Large" },
    { "Industrial Hydrogen Thruster",       "Hydrogen Thruster" },
    { "Industrial Large Hydrogen Thruster", "Hydrogen Thruster, Large" },
    { "Industrial Conveyor Sorter",         "Conveyor Sorter" },
    { "Large Magnetic Plate",               "Magnetic Plate, Large" },

    // DLC - Wasteland
    { "Offset Light",                       "Light, Offset" },
    { "Offset Spotlight",                   "Spotlight, Offset" },
    { "Offroad Wheel Suspension 7x7 Left",  "Wheel Suspension 7x7 Left" },
    { "Offroad Wheel Suspension 5x5 Left",  "Wheel Suspension 5x5 Left" },
    { "Offroad Wheel Suspension 3x3 Left",  "Wheel Suspension 3x3 Left" },
    { "Offroad Wheel Suspension 1x1 Left",  "Wheel Suspension 1x1 Left" },
    { "Offroad Wheel Suspension 7x7 Right", "Wheel Suspension 7x7 Right" },
    { "Offroad Wheel Suspension 5x5 Right", "Wheel Suspension 5x5 Right" },
    { "Offroad Wheel Suspension 3x3 Right", "Wheel Suspension 3x3 Right" },
    { "Offroad Wheel Suspension 1x1 Right", "Wheel Suspension 1x1 Right" },

    // DLC - Sparks of the Future
    { "Sci-Fi Control Panel",               "Control Panel" },
    { "Sci-Fi Ion Thrusters",               "Ion Thruster" },
    { "Sci-Fi Large Ion Thruster",          "Ion Thruster, Large" },
    { "Sci-Fi Atmospheric Thrusters",       "Atmospheric Thruster" },
    { "Sci-Fi Large Atmospheric Thruster",  "Atmospheric Thruster, Large" },
    { "Sci-Fi LCD Panel 5x5",               "LCD Panel, 5x5" },

    // DLC - Warfare 2
    { "Warfare Battery",                    "Battery"},
    { "Small Warfare Reactor",              "Reactor, Small" },
    { "Large Warfare Reactor",              "Reactor, Large" },
    { "Warfare Ion Thruster",               "Ion Thruster" },
    { "Large Warfare Ion Thruster",         "Ion Thruster, Large" },

    // Automaton
    { "Automaton Sensor",                   "Sensor" },
    { "Automaton Programmable Block",       "CPU" },
    { "Automaton CPU",                      "CPU" },
    { "Automaton Timer Block",              "Timer" },
    { "Automaton Timer",                    "Timer" },
    { "Top Mounted Camera",                 "Camera, Top Mounted" },

    // DLC - Decoration Pack #1
    { "Corner Couch",                       "Couch, Corner" },
    { "Armory Lockers",                     "Lockers, Armory" },

    // DLC - Decoration Pack #2
    { "Transparent LCD",                    "LCD, Transparent" },

    // DLC - Decoration Pack #3
    { "Twin-blade Wind Turbine",            "Wind Turbine, Twin-blade" },
    { "Colorable Solar Panel",              "Solar Panel, Colorable" },
    { "Colorable Solar Panel Slope Left",   "Solar Panel, Colorable, Slope Left" },
    { "Colorable Solar Panel Slope Right",  "Solar Panel, Colorable, Slope Right" },
    { "Round Beacon",                       "Beacon, Round" },
    { "Inset Cryo Room",                    "Cryo Chamber, Inset" },
    { "Holo LCD",                           "LCD, Holo" },
    { "Corner Medical Room",                "Medical Room, Corner" },
    { "Inset LCD Panel",                    "LCD Panel, Inset" },
    { "Sloped LCD Panel",                   "LCD Panel, Sloped" },
    { "Curved LCD Panel",                   "LCD Panel, Curved" },
    { "Inset Couch",                        "Couch, Inset" },
    { "Inset Bed",                          "Bed, Inset" },
    { "Inset Button Panel",                 "Button Panel, Inset" },
    { "Inset Bookshelf",                    "Bookshelf, Inset" },
    { "Inset Kitchen",                      "Kitchen, Inset" },
    { "Cab Cockpit",                        "Cockpit, Cab" },

    //
    // Mod Blocks
    //

    { "Compact Antenna",                    "Antenna, Compact" },

    { "BuildAndRepairSystem",               "Build & Repair System" },
    { "DrillSystem",                        "Drill & Fill System" },

    { "Azimuth Block Ion Thruster",         "Ion Thruster" },
    { "Half Azimuth Cargo Storage Tank",    "Cargo Container" },
    { "Azimuth Fusion Reactor (6 Port)",    "Reactor, Fusion" },
    { "Azimuth Fusion Reactor (2 Port)",    "Reactor, Fusion" },

    { "Gyroscope 1x1",                      "Gyroscope" },
    { "Gyroscope 3x3",                      "Gyroscope" },
    { "Gyroscope 5x5",                      "Gyroscope" },

    { "Clean Spotlight",                    "Spotlight" },
    { "Projector Spotlight Slope",          "Spotlight, Slope" },
    { "Projector Spotlight Slanted",        "Spotlight, Slanted" },
    { "Projector Spotlight",                "Spotlight" },

    { "Docking Camera",                     "Camera, Docking" },

    { "Bottom Mounted Camera",              "Camera, Bottom Mounted" },

    { "Compact Shield Emitter",             "Shield Emitter, Compact" },
    { "Ship Shield Emitter",                "Shield Emitter" },
    { "Station Shield Emitter",             "Shield Emitter, Station" },

    { "Small Shield Generator",             "Shield Generator, Small" },
    { "Large Shield Generator",             "Shield Generator, Large" },

    { "Small Oxygen Tank",                  "Oxygen Tank, Small" },
    { "Tiny Oxygen Tank",                   "Oxygen Tank, Tiny" },

    { "Giant Battery",                      "Battery, Giant" },
    { "Huge Battery",                       "Battery, Huge" },
    { "Large Battery",                      "Battery, Large" },
    { "Massive Battery",                    "Battery, Massive" },

    { "Multi-Junction Solar Array",         "Solar Array, Multi-Junction" },
    { "Dual-Junction Solar Array",          "Solar Array, Dual-Junction" },
    { "Nanocrystaline Solar Array",         "Solar Array, Nanocrystaline" },

    { "Stackable Wind Turbine",             "Wind Turbine, Stackable" },

    { "Armored Connector",                  "Connector, Armored" },
    { "Armored Conveyor",                   "Conveyor, Armored" },

    { "1x1 Piston",                         "Piston, 1x1" },
    { "1x5 Piston",                         "Piston, 1x5" },
    { "1x7 Piston",                         "Piston, 1x7" },

    { "1x1 Advanced Rotor DualHead",        "Rotor, Advanced, 1x1, Dual Head" },
    { "1x1 Rotor, Advanced DualHead 1",     "Rotor, Advanced, 1x1, Dual Head" },

    { "Control Panel Pedestal",             "Control Panel, Pedestal" },
    { "Single Button Panel",                "Button Panel, Single" },

    { "Upgradable O2/H2 Generator",         "O2/H2 Generator, Upgradable" }
};


// ============================================================================
//   No User Servicable Parts Below This Point!
// ============================================================================

private readonly string scriptVersion = "1.7.1";

private static readonly System.Text.RegularExpressions.Regex suffixExpression = new System.Text.RegularExpressions.Regex(@"\s\d+$", System.Text.RegularExpressions.RegexOptions.Compiled);

private readonly List<IMyTerminalBlock> blocksCache = new List<IMyTerminalBlock>();
private readonly List<IMyBlockGroup> groupsCache = new List<IMyBlockGroup>();
private readonly Queue<IMyTerminalBlock> blockQueue = new Queue<IMyTerminalBlock>();
private readonly IOHandler ioHandler;

private readonly MyIni configINI;
private static readonly string sectionKey = "Terminal Cleanup";

int blocksTotal = 0;
int blocksRemaining = 0;
int currentBlock = 0;

// Settings (From config)
private bool ProcessSubGrids;
private bool StripSuffixGroupedBlocks;
private bool StripSuffixAllBlocks;
private bool HideGroupedBlocks;
private bool HidePassiveBlocks;
private bool HideUselessInventories;
private bool HideToolbarConfigs;
private bool TurretsDisableTargetNeutral;
private bool TurretsMaxRange;
private bool TurretsDisableTargetCharacter;
private bool TurretsEnableTargetMissiles;
private bool SetAntennaBeaconRangeMax;
private bool SetOreDetectorRangeMax;
private bool DisableUseParkingOnConnector;
private bool DisableUseParkingOnLandingGear;

/// <summary>
        /// Generates the config
        /// </summary>
        /// <param name="forceRegenerate">If true, configuration is forced back to defaults.</param>
private void CreateConfig(bool forceRegenerate = false)
{
    // This call is used to attempt to preserve "other" content
    // that may preexist in custom data.  That data will then be appended
    // to the customdata after the new ini data is inserted
    configINI.TryParse(Me.CustomData);

    if (forceRegenerate || Me.CustomData.Trim().Length == 0) {
        if (!configINI.ContainsSection(sectionKey)) {
            configINI.AddSection(sectionKey);
        }

        configINI.Set(sectionKey, "ProcessSubGrids", false);

        configINI.Set(sectionKey, "StripSuffixGroupedBlocks", false);
        configINI.Set(sectionKey, "StripSuffixAllBlocks", false);

        configINI.Set(sectionKey, "HideGroupedBlocks", true);
        configINI.Set(sectionKey, "HidePassiveBlocks", true);
        configINI.Set(sectionKey, "HideToolbarConfigs", true);
        configINI.Set(sectionKey, "HideUselessInventories", true);

        configINI.Set(sectionKey, "TurretsDisableTargetNeutral", true);
        configINI.Set(sectionKey, "TurretsSetMaxRange", true);
        configINI.Set(sectionKey, "TurretsDisableTargetCharacter", true);
        configINI.Set(sectionKey, "TurretsEnableTargetMissiles", true);

        configINI.Set(sectionKey, "SetAntennaBeaconRangeMax", true);

        configINI.Set(sectionKey, "SetOreDetectorRangeMax", true);

        configINI.Set(sectionKey, "DisableUseParkingOnConnector", true);
        configINI.Set(sectionKey, "DisableUseParkingOnLandingGear", true);

        Me.CustomData = configINI.ToString();
    }
}

/// <summary>
        /// Loads the config
        /// </summary>
private void LoadConfig()
{
    if (!configINI.TryParse(Me.CustomData) || !configINI.ContainsSection(sectionKey)) {
        CreateConfig(true);
    }

    ProcessSubGrids = configINI.Get(sectionKey, "ProcessSubGrids").ToBoolean();

    StripSuffixGroupedBlocks = configINI.Get(sectionKey, "StripSuffixGroupedBlocks").ToBoolean();
    StripSuffixAllBlocks = configINI.Get(sectionKey, "StripSuffixAllBlocks").ToBoolean();

    HideGroupedBlocks = configINI.Get(sectionKey, "HideGroupedBlocks").ToBoolean();
    HidePassiveBlocks = configINI.Get(sectionKey, "HidePassiveBlocks").ToBoolean();
    HideToolbarConfigs = configINI.Get(sectionKey, "HideToolbarConfigs").ToBoolean();
    HideUselessInventories = configINI.Get(sectionKey, "HideUselessInventories").ToBoolean();

    TurretsDisableTargetNeutral = configINI.Get(sectionKey, "TurretsDisableTargetNeutral").ToBoolean();
    TurretsMaxRange = configINI.Get(sectionKey, "TurretsSetMaxRange").ToBoolean();
    TurretsDisableTargetCharacter = configINI.Get(sectionKey, "TurretsDisableTargetCharacter").ToBoolean();
    TurretsEnableTargetMissiles = configINI.Get(sectionKey, "TurretsEnableTargetMissiles").ToBoolean();

    SetAntennaBeaconRangeMax = configINI.Get(sectionKey, "SetAntennaBeaconRangeMax").ToBoolean();
    SetOreDetectorRangeMax = configINI.Get(sectionKey, "SetOreDetectorRangeMax").ToBoolean();

    DisableUseParkingOnConnector = configINI.Get(sectionKey, "DisableUseParkingOnConnector").ToBoolean();
    DisableUseParkingOnLandingGear = configINI.Get(sectionKey, "DisableUseParkingOnLandingGear").ToBoolean();
}

/// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
public Program()
{
    ioHandler = new IOHandler(this, false);
    configINI = new MyIni();

    Me.GetSurface(0).ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
    Me.GetSurface(0).Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
    Me.GetSurface(0).FontSize = 0.5f;
    Me.GetSurface(0).WriteText("");

    ioHandler.AddTextSurface(Me.GetSurface(0));

    CreateConfig();

    // Do not run on compilation
    Runtime.UpdateFrequency = UpdateFrequency.None;
}

/// <summary>
        /// The meat of the script,  this will process a single block at a time
        /// </summary>
        /// <param name="block">The block to process</param>
private void ProcessBlock(IMyTerminalBlock block)
{
    string origname;
    string tempname;
    string name;
    string blockId;

    //
    // Time to Do the Work
    //

    MyIni tempINI = new MyIni();
    bool Local_StripSuffix;

    tempINI.Clear();

    name = block.CustomName;
    blockId = block.BlockDefinition.SubtypeId;

    Local_StripSuffix = false;

    //
    // Local block settings handler
    //

    if (tempINI.TryParse(block.CustomData) && tempINI.ContainsSection("Terminal Cleanup")) {
        if (tempINI.Get("Terminal Cleanup", "PreserveName").ToBoolean(false)) {
            return;
        }

        if (tempINI.Get("Terminal Cleanup", "StripSuffix").ToBoolean(false)) {
            Local_StripSuffix = true;
        }
    }

    //
    // Manipulate Block Names
    //

    name = name.Trim();
    origname = name;
    tempname = name;

    //
    // For each block, remove number suffix.
    // That is trailing numbers that are immediately
    // preceded by whitespace,  this prevents
    // the removal of custom numbers (#1, X-1, etc)
    if (StripSuffixAllBlocks || Local_StripSuffix) {
        name = suffixExpression.Replace(name, "");

        if (name != tempname) {
            ioHandler.Echo($"{origname} : Suffix removed");
        }
    }

    //
    // Apply renaming rules
    foreach (KeyValuePair<string, string> kv in SearchReplaceDict) {
        name = name.Replace(kv.Key, kv.Value);
    }


    name = name.Trim();
    block.CustomName = name;

    //
    // If name has changed,  notify the user
    if (name != tempname) {
        ioHandler.Echo($"{origname} : renamed to '{name}'");
    }


    // --------
    // - /End Manipulate Block Names
    // --------

    // --------
    // - Show / Hide Blocks
    // --------

    //
    // Hide blocks with useless inventories.
    if (HideUselessInventories && HideInventoryBlockIds.Contains(blockId)) {
        if (block.ShowInInventory) {
            block.ShowInInventory = false;
            ioHandler.Echo($"{origname} : Disabled Show in Inventory");
        }
    }

    //
    // Hide passive blocks.
    if (HidePassiveBlocks && PassiveBlockIds.Contains(blockId)) {
        if (block.ShowInTerminal) {
            block.ShowInTerminal = false;
            ioHandler.Echo($"{origname} : Disabled Show in Terminal");
        }

        if (block.ShowInToolbarConfig) {
            block.ShowInToolbarConfig = false;
            ioHandler.Echo($"{origname} : Disabled Show in Toolbar");
        }
    }

    //
    // Hide blocks in toolbar config
    if (HideToolbarConfigs && HideToolbarConfigBlockIds.Contains(blockId)) {
        if (block.ShowInToolbarConfig) {
            block.ShowInToolbarConfig = false;
            ioHandler.Echo($"{origname} : Disabled Show in Toolbar");
        }
    }

    //
    // Set block-specific default settings
    //

    if (block is IMyLargeTurretBase) {
        if (TurretsDisableTargetNeutral) {
            if (block.GetValueBool("TargetNeutrals")) {
                block.SetValueBool("TargetNeutrals", false);
                ioHandler.Echo($"{origname} : Turret - Disabled Target Neutrals");
            }
        }

        if (TurretsDisableTargetCharacter) {
            if (block.GetValueBool("TargetCharacters")) {
                block.SetValueBool("TargetCharacters", false);
                ioHandler.Echo($"{origname} : Turret - Disabled Target Characters");
            }

        }

        if (TurretsMaxRange) {
            if (!block.GetValueFloat("Range").Equals(float.MaxValue)) {
                block.SetValueFloat("Range", float.MaxValue);
                ioHandler.Echo($"{origname} : Turret - Set Range to Maximum");
            }
        }
    }

    else if (block is IMyLargeGatlingTurret || block is IMyLargeInteriorTurret) {
        if (TurretsEnableTargetMissiles) {
            if (!block.GetValueBool("TargetMissiles")) {
                block.SetValueBool("TargetMissiles", true);
                ioHandler.Echo($"{origname} : Turret - Enabled Target Missiles");
            }
        }
    }

    else if (block is IMyRadioAntenna || block is IMyBeacon) {
        if (SetAntennaBeaconRangeMax) {
            if (!block.GetValueFloat("Radius").Equals(float.MaxValue)) {
                block.SetValueFloat("Radius", float.MaxValue);

                if (block is IMyRadioAntenna) {
                    ioHandler.Echo($"{origname} : Antenna - Set Radius to Maximum", block.Name);
                }
                else if (block is IMyBeacon) {
                    ioHandler.Echo($"{origname} : Beacon - Set Radius to Maximum");
                }
            }
        }
    }

    else if (block is IMyOreDetector && SetOreDetectorRangeMax) {
        if (!block.GetValueFloat("Range").Equals(float.MaxValue)) {
            block.SetValueFloat("Range", float.MaxValue);
            ioHandler.Echo($"{origname} : Ore Detector - Set Range to Maximum");
        }
    }

    else if (block is IMyShipConnector && DisableUseParkingOnConnector) {
        (block as IMyShipConnector).IsParkingEnabled = false;
    }

    else if (block is IMyLandingGear && DisableUseParkingOnLandingGear) {
        (block as IMyLandingGear).IsParkingEnabled = false;
    }

    //
    // Block Groups
    //

    //
    // Process grouped blocks.
    foreach (var group in GridTerminalSystem.Groups(groupsCache)) {
        foreach (var b in group.Blocks(blocksCache, ProcessSubGrids ? null : Me.CubeGrid)) {
            if (HideGroupedBlocks && !ControllableBlockIds.Contains(b.BlockDefinition.SubtypeId)) {
                b.ShowInTerminal = false;
                b.ShowInToolbarConfig = false;
            }

            if (StripSuffixGroupedBlocks) {
                b.CustomName = suffixExpression.Replace(b.CustomName, "");
            }

            //
            // Because the block is in a group,  hide it in toolbar config
            b.ShowInToolbarConfig = false;
        }
    }
}

/// <summary>
        /// Program Entrypoint,  this is the function called by Space Engineers
        /// </summary>
        /// <param name="argument">The arguments passed to us to use</param>
        /// <param name="updateSource">The reason/how we have been called</param>
public void Main(string argument, UpdateType updateSource)
{
    if ((updateSource & UpdateType.Update1) != 0) {
        if (blocksRemaining == 0) {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            return;
        }

        IMyTerminalBlock block = blockQueue.Dequeue();

        ProcessBlock(block);

        blocksRemaining -= 1;
        currentBlock += 1;

        Echo($"Terminal Cleanup v{scriptVersion}");
        Echo("-------------------------");
        Echo($"Processing...[[{currentBlock} / {blocksTotal}]]");
    }
    else if ((updateSource & UpdateType.Terminal) != 0) {
        LoadConfig();

        // build our block cache
        if (ProcessSubGrids) {
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocksCache, block => block.IsSameConstructAs(Me));
        }
        else {
            GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocksCache, block => block.CubeGrid.Equals(Me.CubeGrid));
        }

        if (argument.Trim().ToUpper().StartsWith("RESTORE_DEFAULT")) {
            foreach (IMyTerminalBlock block in blocksCache) {
                if (block.DefinitionDisplayNameText.Length != 0) {
                    ioHandler.Echo($"Restored {block.CustomName} name back to {block.DefinitionDisplayNameText}");


                    //block.CustomName = block.DefinitionDisplayNameText;
                    block.CustomName = $"{block.DisplayNameText} {block.NumberInGrid}";
                    block.ShowInTerminal = true;
                    block.ShowInInventory = true;
                    block.ShowInToolbarConfig = true;
                }
            }

            return;
        }
        else if (argument.Trim().ToUpper().StartsWith("RESET_CONFIG")) {
            CreateConfig(true);

            return;
        }

        foreach (var block in blocksCache) {
            blockQueue.Enqueue(block);
        }

        blocksTotal = blockQueue.Count;
        blocksRemaining = blocksTotal;
        currentBlock = 0;

        Runtime.UpdateFrequency |= UpdateFrequency.Update1;
    }
}

}
/// <summary>
    /// Class containing extension methods for the following classes<br/>
    /// <br/>
    /// <see cref="IMyGridTerminalSystem">GridTerminalSystem</see>
    /// </summary>
public static class Extensions
{
    /// <summary>
        /// </summary>
        /// <param name="cache">An existing List to make use of (will be cleared!)</param>
        /// <param name="grid">The grid to filter against, or NULL to search all</param>
        /// <param name="collect">The filter function</param>
        /// <returns>A list of <typeparamref name="T"/></returns>
    public static List<T> Blocks<T>(this IMyGridTerminalSystem gts, List<T> cache = null, IMyCubeGrid grid = null, Func<T, bool> collect = null) where T : class, IMyCubeBlock
    {
        List<T> blocks;

        if (cache != null) {
            cache.Clear();
            blocks = cache;
        }
        else {
            blocks = new List<T>();
        }

        if (grid == null) {
            gts.GetBlocksOfType(blocks, collect);
        }
        else {
            gts.GetBlocksOfType(blocks, block => block.CubeGrid.Equals(grid) && (collect?.Invoke(block) ?? true));
        }

        return blocks;
    }

    /// <summary>
        /// </summary>
        /// <param name="cache">An existing List to make use of (will be cleared!)</param>
        /// <param name="grid">The grid to filter against, or NULL to search all</param>
        /// <param name="collect">The filter function</param>
        /// <returns>A list of <typeparamref name="T"/></returns>
    public static List<T> Blocks<T>(this IMyBlockGroup group, List<T> cache = null, IMyCubeGrid grid = null, Func<T, bool> collect = null) where T : class, IMyCubeBlock
    {
        List<T> blocks;

        if (cache != null) {
            cache.Clear();
            blocks = cache;
        }
        else {
            blocks = new List<T>();
        }

        if (grid == null) {
            group.GetBlocksOfType(blocks, collect);
        }
        else {
            group.GetBlocksOfType(blocks, block => block.CubeGrid.Equals(grid) && (collect?.Invoke(block) ?? true));
        }

        return blocks;
    }

    /// <summary>
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="collect"></param>
        /// <returns>A list of IMyBlockGroups</returns>
    public static List<IMyBlockGroup> Groups(this IMyGridTerminalSystem gts, List<IMyBlockGroup> cache = null, Func<IMyBlockGroup, bool> collect = null)
    {
        List<IMyBlockGroup> groups;

        if (cache != null) {
            cache.Clear();
            groups = cache;
        }
        else {
            groups = new List<IMyBlockGroup>();
        }

        gts.GetBlockGroups(groups, collect);

        return groups;
    }
}

/// <summary>
    /// Class used to write text to various outputs.
    /// Supports Terminal, and anything that provides a TextSurface, TextSurfaceProvider, or TextPanel
    /// </summary>
    /// <remarks>
    /// Can output a log to CustomData
    /// </remarks>
public class IOHandler
{
    private readonly MyGridProgram parent;
    private readonly List<IMyTextPanel> panelList;
    private readonly List<IMyTextSurface> surfaceList;
    private readonly bool echoToCustomData;

    /// <summary>
        /// Initializes a new instance of the <see cref="IOHandler"/> class.
        /// </summary>
        /// <param name="parentProgram">The MyGridProgram that is running us</param>
        /// <param name="echoToCustomData">If true, log to CustomData as well</param>
    public IOHandler(MyGridProgram parentProgram, bool echoToCustomData = false)
    {
        parent = parentProgram;

        panelList = new List<IMyTextPanel>();
        surfaceList = new List<IMyTextSurface>();

        this.echoToCustomData = echoToCustomData;

        if (echoToCustomData) {
            parent.Me.CustomData = "";
            parent.Me.CustomData += "=====\n";
            parent.Me.CustomData += $"{DateTime.Now.ToString("HH:mm:ss")} - Logging Started\n";
            parent.Me.CustomData += "=====\n";
        }
    }

    /// <summary>
        /// Outputs text to the various targets
        /// </summary>
        /// <param name="message">The message.</param>
    public void Echo(string message, bool suppressTerminal = false)
    {
        if (!suppressTerminal) {
            // this prevents the dreaded yellow text in terminal
            // '[' and ']' are used for color codes
            string filteredMessage = message.Replace("[", "[[").Replace("]", "]]");

            parent.Echo(filteredMessage);
        }

        if (panelList.Count > 0) {
            foreach (IMyTextPanel panel in panelList) {
                if (panel.IsWorking) {
                    panel.WriteText(message + "\n", true);
                }
            }
        }

        if (surfaceList.Count > 0) {
            foreach (IMyTextSurface surface in surfaceList) {
                surface.WriteText(message + "\n", true);
            }
        }

        if (echoToCustomData) {
            parent.Me.CustomData += $"{DateTime.Now.ToString("HH:mm:ss")} - {message}\n";
        }
    }

    /// <summary>
        /// Outputs formatted text to the various targets
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
    public void Echo(string format, params object[] args)
    {
        this.Echo(string.Format(format, args));
    }

    /// <summary>
        /// Adds a TextPanel to Output List
        /// </summary>
        /// <param name="panel"></param>
    public void AddTextPanel(IMyTextPanel panel)
    {
        panelList.Add(panel);

        panel.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        panel.WriteText("");
    }

    /// <summary>
        /// Adds a TextSurface to Output List
        /// </summary>
        /// <param name="surface"></param>
    public void AddTextSurface(IMyTextSurface surface)
    {
        surfaceList.Add(surface);

        surface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        surface.WriteText("");
    }

    /// <summary>
        /// Adds a TextSurfaceProvider to Output List
        /// </summary>
        /// <remarks>
        /// This is mostly used for cockpits, or other blocks with multiple TextSurface's
        /// </remarks>
        /// <param name="provider"></param>
        /// <param name="index">Zero based index of target surface</param>
    public void AddTextSurfaceProvider(IMyTextSurfaceProvider provider, int index = 0)
    {
        if (index < 0) {
            this.Echo("Error - AddTextSurfaceProvider : Argument '{0}' was Out of Range, Expected >= '0', requested '{1}'", "index", index);
        }
        else if (index < provider.SurfaceCount) {
            AddTextSurface(provider.GetSurface(index));
        }
        else {
            this.Echo("Error - AddTextSurfaceProvider : Argument '{0}' was Out of Range, Expected <= '{1]', requested '{2}'", "index", provider.SurfaceCount - 1, index);
        }
    }