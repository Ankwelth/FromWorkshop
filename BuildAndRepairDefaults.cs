/*
    --- Build & Repair Defaults ---

    Version
            1.1

    Changelog
            1.1 - Added ini configuration
                -- 'Ignore"
                -- 'SkipColors'
                -- 'SkipOffsets'
               
            1.0 - Initial Version


    Script Arguments (Case Insensitive)
        SKIPCOLORS
                - Skip setting GrindColor globally

        SKIPOFFSETS
                - Do not reset Offset globally
 

    INI Configuration
        Ignore
                - Set to True to ignore this block when setting defaults

        SkipColors
                - Set to True to not set GrindColor for this block

        SkipOffsets
                - Set to True to not reset Offset for this block


    Description
        This script sets *my* defaults for every B&R block
        it finds on the grid.

        Renames block from "BuildAndRepairSystem" to "Build & Repair System"
        Sets Search Mode to Fly
        Sets Work Mode to Grind before Weld
        Disables Ignore Color
        Sets grind color to hot pink (330, 100, 80)
        Enable push component immediately
        Disable Collect Ore
        Resets Offset to {0, 0, 0}
        Turns off Show Area
        and Finally sets Sound Volume to 5%


        Note: any number of arguments can be used at the same time,  seperate by a space

*/

/// <summary>
        ///     The search modes supported by the block
        /// </summary>
        /// <remarks>
        ///     This is the for Build & Repair Mod
        /// </remarks>
private enum SearchModes
{
    /// <summary>
            /// Walk Mode
            /// </summary>
    Grids = 0x0001,

    /// <summary>
            /// Fly Mode
            /// </summary>
    BoundingBox = 0x0002
}

/// <summary>
        ///     The work modes supported by the block
        /// </summary>
        /// <remarks>
        ///     This is the for Build & Repair Mod
        /// </remarks>
private enum WorkModes
{
    /// <summary>
            ///     Grind only if nothing to weld
            /// </summary>
    WeldBeforeGrind = 0x0001,

    /// <summary>
            ///     Weld only if nothing to grind
            /// </summary>
    GrindBeforeWeld = 0x0002,

    /// <summary>
            ///     Grind only if nothing to weld or
            ///     build waiting for missing items
            /// </summary>
    GrindIfWeldGetStuck = 0x0004,

    /// <summary>
            ///     Only welding is allowed
            /// </summary>
    WeldOnly = 0x0008,

    /// <summary>
            ///     Only grinding is allowed
            /// </summary>
    GrindOnly = 0x0010
}

private readonly List<IMyTerminalBlock> blocksCache = new List<IMyTerminalBlock>();
private readonly MyIni IniHandler;
private readonly string SectionName = "B&R Defaults";

/// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
public Program()
{
    IniHandler = new MyIni();
    Runtime.UpdateFrequency = UpdateFrequency.None;
}

/// <summary>
        /// Main Entrypoint,  This is what is called by the game.
        /// </summary>
        /// <param name="argument">The argument string</param>
        /// <param name="updateSource">The update source</param>
public void Main(string argument, UpdateType updateSource)
{
    Echo("Build & Repair Defaults");

    bool doColors = true;
    bool doOffsets = true;

    if (( updateSource & UpdateType.Terminal ) != 0) {

        if (argument.Trim().ToUpper().Contains("SKIPOFFSETS")) {
            doOffsets = false;
        }

        if (argument.Trim().ToUpper().Contains("SKIPCOLORS")) {
            doColors = false;
        }

        GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocksCache, block => block.IsSameConstructAs(Me));

        string blockId;

        foreach (var block in blocksCache) {
            blockId = block.BlockDefinition.SubtypeId;

            if (blockId.Contains("NanobotBuildAndRepairSystem")) {
                IniHandler.Clear();

                if (!IniHandler.TryParse(block.CustomData) || !IniHandler.ContainsSection(SectionName)) {
                    IniHandler.AddSection(SectionName);
                    IniHandler.Set(SectionName, "Ignore", false);
                    IniHandler.SetComment(SectionName, "Ignore", "Set to True to skip setting Defaults for this block.");
                    IniHandler.Set(SectionName, "SkipColors", false);
                    IniHandler.SetComment(SectionName, "SkipColors", "Set to True to Skip setting GrindColor for this block");
                    IniHandler.Set(SectionName, "SkipOffsets", false);
                    IniHandler.SetComment(SectionName, "SkipOffsets", "Set to True to Skip resetting Offsets to {0,0,0} for this block");

                    block.CustomData = IniHandler.ToString();
                }

                // Skip B&R blocks that have a key 'ignore' set to 'true'
                if (IniHandler.ContainsKey(SectionName, "Ignore") && IniHandler.Get(SectionName, "Ignore").ToBoolean()) {
                    Echo($"Skipping block '{block.CustomName}', Ignoring");
                    continue;
                }

                doColors = doColors && !IniHandler.Get(SectionName, "SkipColors").ToBoolean();
                doOffsets = doOffsets && !IniHandler.Get(SectionName, "SkipOffsets").ToBoolean();

                //
                //
                //

                Echo($"Working on block '{block.CustomName}'");

                // Fixup the name
                block.CustomName = block.CustomName.Replace("BuildAndRepairSystem", "Build & Repair System");

                // Disable the Ignore Color
                block.SetValueBool("BuildAndRepair.UseIgnoreColor", false);

                // Enable the Grind Color
                block.SetValueBool("BuildAndRepair.UseGrindColor", true);

                if (doColors) {
                    // Set GrindColor to Hot Pink!
                    block.SetValue("BuildAndRepair.GrindColor", new Vector3(330.0f, 100.0f, 100.0f));
                }

                // Set Grind Not Owned, Grind Neutrals, and Grind Smallest Grid First
                block.SetValueBool("BuildAndRepair.GrindJanitorNotOwned", true);
                block.SetValueBool("BuildAndRepair.GrindJanitorNeutrals", true);
                block.SetValueBool("BuildAndRepair.GrindSmallestGridFirst", true);

                // Set to Push Components Immediately
                block.SetValueBool("BuildAndRepair.PushComponentImmediately", true);

                if (doOffsets) {
                    // Reset Area Offsets back to Zero
                    block.SetValue("BuildAndRepair.AreaOffsetLeftRight", 0.0f);
                    block.SetValue("BuildAndRepair.AreaOffsetUpDown", 0.0f);
                    block.SetValue("BuildAndRepair.AreaOffsetFrontBack", 0.0f);
                }

                // Set Search Mode to Fly
                block.SetValue<long>("BuildAndRepair.Mode", (long)SearchModes.BoundingBox);

                // Set WorkMod to Grind Before Weld
                block.SetValue<long>("BuildAndRepair.WorkMode", (long)WorkModes.GrindBeforeWeld);

                // Enable collect ore
                Action<int, bool> setCollectEnabled = block.GetValue<Action<int, bool>>("BuildAndRepair.SetCollectEnabled");
                setCollectEnabled(4, false);

                // Disable Show Area
                block.SetValueBool("BuildAndRepair.ShowArea", false);

                // Set Sound Volume to 5%
                block.SetValueFloat("BuildAndRepair.SoundVolume", 5f);
            }
        }
    }
}