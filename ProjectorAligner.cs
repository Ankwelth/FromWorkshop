/*
 * Projector Aligner
 * -----------------
 * 
 * All configuration is done through Custom Data. Simply create a heading
 * in the custom data of any block that you wish to use with this script:
 * 
 * [projector]
 * 
 * You may add cockpits, flight seats, remote controls and projectors.
 * By default all blocks will be in the "default" projector group. You can
 * change this by specifying a group name. For example:
 * 
 * [projector]
 * group=Ship Printer
 * 
 * To make use of a display, specify a display number and (optionally) a
 * scale and a color. For example:
 * 
 * [projector]
 * display=0
 * scale=0.5
 * color=00CED1
 * 
 * Change the number to the screen that you wish to use; numbers start at 0, so a
 * five-screen cockpit has screens 0, 1, 2, 3 and 4.
 * Color should be entered as six hexadecimal digits representing red, green and blue.
 * 
 * Additionally, you can specify details for a specific display using
 * numbered headings. This allows you to configure the display for more
 * than one group on a multi-display block, such as a cockpit, or to
 * display the output on a cockpit without using it for control. For example:
 * 
 * [projector_display0]
 * scale=0.8
 * color=F07020
 * group=Second Printer
 * 
 * Arguments
 * ---------
 * 
 * up             Move up one line on screen
 * down           Move down one line on screen
 * apply          Enter Menu / Select current projector from Menu
 * select           "
 * build          Re-read the Custom Data of blocks, and reconfigure
 * rebuild          "
 * save           Store the current alignment values in Custom Data
 * load           Recover the alignment values from Custom Data
 * 
 * Follow the argument with a group name if other than "default". Use quotes if the
 * group name contains spaces or begins with a hyphen.
 */

string Version = "Version 1.2.2";
List<IMyTerminalBlock>Blocks = new List<IMyTerminalBlock>();
List<ProjectorController> ProjectorControllers = new List<ProjectorController>();
Dictionary<string, ProjectorGroup> ProjectorGroups = new Dictionary<string, ProjectorGroup>();
MyIni ini = new MyIni();
static string iniSection = "projector";
static string DisplaySectionPrefix = iniSection + "_display";
MyCommandLine commandLine = new MyCommandLine();
private List<string> SectionNames = new List<string>();
StringBuilder SectionCandidateName = new StringBuilder();

struct MenuItem
{
    public string Sprite;
    public float SpriteRotation;
    public Color SpriteColor;
    public Color TextColor;
    public string MenuText;
    public Action Action;
}

public void Build()
{
    StringComparison ignoreCase = StringComparison.InvariantCultureIgnoreCase;
    IMyProjector projector = null;
    IMyShipController controller = null;
    IMyTextSurfaceProvider provider = null;
    string groupName;
    ProjectorControllers.Clear();
    ProjectorGroups.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Blocks, block => {
        if (!block.IsSameConstructAs(Me))
            return false;
        ini.TryParse(block.CustomData);
        ini.GetSections(SectionNames);
        foreach (var section in SectionNames)
        {
            Echo(String.Format("Block: {0}",block.CustomName));
            groupName = ini.Get(section, "group").ToString("default");
            projector = block as IMyProjector;
            if (null != projector && section.Equals(iniSection))
            {
                Echo(String.Format("Projector: {0}",groupName));
                if (!ProjectorGroups.ContainsKey(groupName))
                {
                    ProjectorGroups.Add(groupName, new ProjectorGroup(groupName, this));
                }
                ProjectorGroups[groupName].Add(projector);
            }
            provider = block as IMyTextSurfaceProvider;
            if (null != provider && (section.Equals(iniSection) || section.StartsWith(DisplaySectionPrefix)))
            {
                Echo(String.Format("Provider: {0}", groupName));
                if (!ProjectorGroups.ContainsKey(groupName))
                {
                    ProjectorGroups.Add(groupName, new ProjectorGroup(groupName, this));
                }
                int surfacenumber = -1;
                for (int displayNumber = 0; displayNumber < provider.SurfaceCount; ++displayNumber)
                {
                    SectionCandidateName.Clear();
                    SectionCandidateName.Append(DisplaySectionPrefix).Append(displayNumber.ToString());
                    if (section.Equals(SectionCandidateName.ToString(), ignoreCase))
                    {
                        surfacenumber = displayNumber;
                        Echo(String.Format("Display: {0}", surfacenumber));
                        addDisplay(provider, groupName, surfacenumber, section);
                    }
                }
                if (section == iniSection)
                {
                    surfacenumber = ini.Get(section, "display").ToInt16(-1);
                    Echo(String.Format("Display: {0}", surfacenumber));
                    addDisplay(provider, groupName, surfacenumber, section);
                }
            }
            controller = block as IMyShipController;
            if (null != controller && section.Equals(iniSection))
                {
                Echo(String.Format("Controller: {0}",groupName));
                ProjectorController projectorController = new ProjectorController(controller, this);
                if (!ProjectorGroups.ContainsKey(groupName))
                    ProjectorGroups.Add(groupName, new ProjectorGroup(groupName, this));
                projectorController.projectorGroup = ProjectorGroups[groupName];
                ProjectorControllers.Add(projectorController);
            }
        }
        return false;
    });
}

private void addDisplay(IMyTextSurfaceProvider provider, string groupName, int surfacenumber, string section)
{
    if (surfacenumber >= 0 && provider.SurfaceCount > surfacenumber)
    {
        string DefaultColor = "00CED1";
        float scale = ini.Get(section, "scale").ToSingle(1.0f);
        string ColorStr = ini.Get(section, "color").ToString(DefaultColor);
        if (ColorStr.Length < 6)
            ColorStr = DefaultColor;
        string R = ColorStr.Substring(0, 2);
        string G = ColorStr.Substring(2, 2);
        string B = ColorStr.Substring(4, 2);
        Color color = new Color() { R = byte.Parse(R, System.Globalization.NumberStyles.HexNumber), G = byte.Parse(G, System.Globalization.NumberStyles.HexNumber), B = byte.Parse(B, System.Globalization.NumberStyles.HexNumber), A = 255 };
        ProjectorGroups[groupName].Add(new ManagedDisplay(provider.GetSurface(surfacenumber), scale, color));
    }
}

public Program()
{
    Echo(Version);
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    Build();
}

public void Main(string argument, UpdateType updateSource)
{
    foreach (var projectorController in ProjectorControllers)
        projectorController.UpdateKeys();
    if (commandLine.TryParse(argument))
    {
        string groupName = commandLine.Argument(1) ?? "default";
        switch (commandLine.Argument(0))
        {
            case "up":
                if (ProjectorGroups.ContainsKey(groupName))
                    ProjectorGroups[groupName].Up();
                break;
            case "down":
                if (ProjectorGroups.ContainsKey(groupName))
                    ProjectorGroups[groupName].Down();
                break;
            case "apply":
            case "select":
                if (ProjectorGroups.ContainsKey(groupName))
                {
                    ProjectorGroup group = ProjectorGroups[groupName];
                    if (!group.DisplayStatus)
                    {
                        group.Select();
                        group.UpdateDisplays();
                    }
                    group.DisplayStatus = !group.DisplayStatus;
                }
                break;
            case "load":
                if (ProjectorGroups.ContainsKey(groupName))
                    ProjectorGroups[groupName].LoadAlignment();
                break;
            case "save":
                if (ProjectorGroups.ContainsKey(groupName))
                    ProjectorGroups[groupName].SaveAlignment();
                break;
            case "build":
            case "rebuild":
                Build();
                break;
            default:
                break;
        }
    }
    if ((updateSource & UpdateType.Update100) != 0)
    {
        foreach (var projectorGroup in ProjectorGroups.Values)
            projectorGroup.UpdateDisplays();
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
        Position = new Vector2(viewport.Width / 2f, StartHeight) + viewport.Position;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Projector Status",
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
        Position = new Vector2(viewport.Width / 2f, StartHeight + HeadingHeight) + viewport.Position;
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
        AddMenuItem("Save alignment", "LCD_Emote_Happy", 0,Color.Yellow);
        AddMenuItem("Restore alignment", "LCD_Emote_Neutral", 0,Color.Green);
        AddMenuItem("Toggle projector power", "LCD_Emote_Dead", 0,Color.Red);
        frame.Dispose();
    }

    internal void RenderProjectorStatus(IMyProjector projector, string group)
    {
        frame = surface.DrawFrame();
        CursorDrawPosition = new Vector2(0, BodyBeginsHeight + LineHeight + LineHeight * CursorMenuPosition) + viewport.Position;
        AddHeading(7);
        // Projector group & name
        Position.X = viewport.Width / 2f - LineHeight;
        Position += new Vector2(0, LineHeight);
        float ColumnWidth = viewport.Width / 4;

        if (null == projector)
        {
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Group: " + group + "  Projector: NOT CONFIGURED!",
                Position = Position,
                RotationOrScale = RegularFontSize / 2,
                Color = Color.Gray,
                Alignment = TextAlignment.CENTER,
                FontId = "White"
            });
            Position += new Vector2(0, LineHeight * 3);
            frame.Add(new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "Danger",
                Position = Position,
                Size = new Vector2(LineHeight * 3, LineHeight * 3),
                Color = Color.Red,
            })
            ;
            frame.Dispose();
            return;
        }

        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Group: " + group + "  Projector: " + projector.CustomName,
            Position = Position,
            RotationOrScale = RegularFontSize / 2,
            Color = Color.Gray,
            Alignment = TextAlignment.CENTER,
            FontId = "White"
        });
        // Headings
        Position.X = surface.TextPadding;
        Position += new Vector2(0, LineHeight);
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Offset",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.White,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth * 2;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Rotation",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.White,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        // Data line 1 of 3
        Position.X = surface.TextPadding;
        Position += new Vector2(0, LineHeight);
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Horiz:",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = projector.ProjectionOffset.X.ToString(),
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Pitch:",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = projector.ProjectionRotation.Y.ToString(),
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        // Data line 2 of 3
        Position.X = surface.TextPadding;
        Position += new Vector2(0, LineHeight);
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Vert:",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = projector.ProjectionOffset.Y.ToString(),
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Yaw:",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = projector.ProjectionRotation.X.ToString(),
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        // Data line 3 of 3
        Position.X = surface.TextPadding;
        Position += new Vector2(0, LineHeight);
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Forward:",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = projector.ProjectionOffset.Z.ToString(),
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = "Roll:",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        Position.X += ColumnWidth;
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = projector.ProjectionRotation.Z.ToString(),
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = HighlightColor,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
        // Blocks remaining
        Position.X = surface.TextPadding;
        Position += new Vector2(0, LineHeight * 1.5f);
        frame.Add(new MySprite()
        {
            Type = SpriteType.TEXT,
            Data = $"Blocks remaining: {projector.RemainingBlocks} (total {projector.TotalBlocks})",
            Position = Position,
            RotationOrScale = RegularFontSize,
            Color = Color.Gray,
            Alignment = TextAlignment.LEFT,
            FontId = "White"
        });
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

class ProjectorController
{
    private const int Key_Down = 1;
    private const int Key_Up = 2;
    private const int Key_Left = 4;
    private const int Key_Right = 8;
    private const int Key_E = 16;
    private const int Key_Q = 32;
    private const int Key_D = 64;
    private const int Key_A = 128;
    private const int Key_Space = 256;
    private const int Key_C = 512;
    private const int Key_S = 1024;
    private const int Key_W = 2048;

    private Program program;

    private IMyShipController controller;
    public ProjectorGroup projectorGroup
    {
        get; set;
    }
    private int previousKeys;

    public ProjectorController(IMyShipController controller, Program program)
    {
        this.controller = controller;
        this.program = program;
    }

    public int GetKeysPressed()
    {
        if (null == controller || !controller.IsUnderControl)
            return 0;
        const float RotationThreshold = 8.0f;
        const float RollThreshold = 0.5f;
        const float MoveThreshold = 0.5f;
        int keysPressed = 0;
        if (controller.RotationIndicator.X > RotationThreshold)
            keysPressed |= Key_Down;
        if (controller.RotationIndicator.X < -RotationThreshold)
            keysPressed |= Key_Up;
        if (controller.RotationIndicator.Y > RotationThreshold)
            keysPressed |= Key_Left;
        if (controller.RotationIndicator.Y < -RotationThreshold)
            keysPressed |= Key_Right;
        if (controller.RollIndicator > RollThreshold)
            keysPressed |= Key_E;
        if (controller.RollIndicator < -RollThreshold)
            keysPressed |= Key_Q;
        if (controller.MoveIndicator.X > MoveThreshold)
            keysPressed |= Key_D;
        if (controller.MoveIndicator.X < -MoveThreshold)
            keysPressed |= Key_A;
        if (controller.MoveIndicator.Y > MoveThreshold)
            keysPressed |= Key_Space;
        if (controller.MoveIndicator.Y < -MoveThreshold)
            keysPressed |= Key_C;
        if (controller.MoveIndicator.Z > MoveThreshold)
            keysPressed |= Key_S;
        if (controller.MoveIndicator.Z < -MoveThreshold)
            keysPressed |= Key_W;
        int retVal = (previousKeys | keysPressed) ^ previousKeys;
        previousKeys = keysPressed;
        program.Runtime.UpdateFrequency |= UpdateFrequency.Once;
        return retVal;
    }

    public void UpdateKeys()
    {
        int keys = GetKeysPressed();
        if (keys == 0)
            return;
        var projector = projectorGroup.CurrentProjector;
        if (null == projector)
            return;
        if ((keys & Key_Right) == Key_Right)
        {
            projector.ProjectionRotation = new Vector3I(projector.ProjectionRotation.X >= 3 ? 0 : projector.ProjectionRotation.X + 1, projector.ProjectionRotation.Y, projector.ProjectionRotation.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_Left) == Key_Left)
        {
            projector.ProjectionRotation = new Vector3I(projector.ProjectionRotation.X == 0 ? 3 : projector.ProjectionRotation.X - 1, projector.ProjectionRotation.Y, projector.ProjectionRotation.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_Up) == Key_Up)
        {
            projector.ProjectionRotation = new Vector3I(projector.ProjectionRotation.X, projector.ProjectionRotation.Y >= 3 ? 0 : projector.ProjectionRotation.Y + 1, projector.ProjectionRotation.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_Down) == Key_Down)
        {
            projector.ProjectionRotation = new Vector3I(projector.ProjectionRotation.X, projector.ProjectionRotation.Y == 0 ? 3 : projector.ProjectionRotation.Y - 1, projector.ProjectionRotation.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_E) == Key_E)
        {
            projector.ProjectionRotation = new Vector3I(projector.ProjectionRotation.X, projector.ProjectionRotation.Y, projector.ProjectionRotation.Z >= 3 ? 0 : projector.ProjectionRotation.Z + 1);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_Q) == Key_Q)
            {
            projector.ProjectionRotation = new Vector3I(projector.ProjectionRotation.X, projector.ProjectionRotation.Y, projector.ProjectionRotation.Z == 0 ? 3 : projector.ProjectionRotation.Z - 1);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_A) == Key_A)
        {
            projector.ProjectionOffset = new Vector3I(projector.ProjectionOffset.X + 1, projector.ProjectionOffset.Y, projector.ProjectionOffset.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_D) == Key_D)
        {
            projector.ProjectionOffset = new Vector3I(projector.ProjectionOffset.X - 1, projector.ProjectionOffset.Y, projector.ProjectionOffset.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_Space) == Key_Space)
        {
            projector.ProjectionOffset = new Vector3I(projector.ProjectionOffset.X, projector.ProjectionOffset.Y - 1, projector.ProjectionOffset.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_C) == Key_C)
        {
            projector.ProjectionOffset = new Vector3I(projector.ProjectionOffset.X, projector.ProjectionOffset.Y + 1, projector.ProjectionOffset.Z);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_S) == Key_S)
        {
            projector.ProjectionOffset = new Vector3I(projector.ProjectionOffset.X, projector.ProjectionOffset.Y, projector.ProjectionOffset.Z - 1);
            projector.UpdateOffsetAndRotation();
        }
        if ((keys & Key_W) == Key_W)
        {
            projector.ProjectionOffset = new Vector3I(projector.ProjectionOffset.X, projector.ProjectionOffset.Y, projector.ProjectionOffset.Z + 1);
            projector.UpdateOffsetAndRotation();
        }
        projectorGroup.UpdateDisplays();
    }
}

class ProjectorGroup
{
    private Program program;
    private List<IMyProjector> projectors = new List<IMyProjector>();
    private List<ManagedDisplay> displays = new List<ManagedDisplay>();
    private List<MenuItem> ProjectorMenu = new List<MenuItem>();
    private int SelectedLine = 0;
    private int SelectedProjector = 0;
    private int UILines = 3; // Number of additional menu lines
    public bool DisplayStatus
    {
        get
        {
            return displayStatus;
        }
        set
        {
            displayStatus = value;
            UpdateDisplays();
        }
    }
    public string CustomName
    {
        get;
        set;
    }
    private IMyProjector currentProjector;
    private bool displayStatus;

    public ProjectorGroup(string customName,Program program)
    {
        this.CustomName = customName;
        this.program = program;
        DisplayStatus = true;
    }

    public IMyProjector CurrentProjector
    {
        get
        {
            return currentProjector;
        }
        set
        {
            if (projectors.Contains(value))
            {
                this.currentProjector = value;
                foreach (var projector in projectors)
                {
                    if (projector.Equals(value))
                    {
                        projector.Enabled = true;
                    }
                    else
                    {
                        projector.Enabled = false;
                    }
                }
            }
            else
            {
                this.currentProjector = null;
            }
        }
    }

    public void Add(IMyProjector projector)
    {
        if (projectors.Count == 0 || projector.Enabled)
        {
            this.currentProjector = projector;
            this.SelectedLine = projectors.Count;
        }
        if (!projectors.Contains(projector))
        {
            projectors.Add(projector);
        }
        ProjectorMenu.Add(new MenuItem() {
            Sprite = "Construction",
            SpriteColor = Color.Yellow,
            TextColor = Color.White,
            MenuText = projector.CustomName,
            Action = () => { currentProjector = projector; }
        });
    }
    public void Add(ManagedDisplay surface)
    {
        if (!displays.Contains(surface))
        {
            displays.Add(surface);
        }
    }
    public void Clear()
    {
        projectors.Clear();
        displays.Clear();
    }
    public void Select()
    {
        this.Select(SelectedLine);
    }
    public void Select(int menuIndex)
    {
        if (menuIndex < 0 || menuIndex > projectors.Count + UILines - 1)
            return;
        if (menuIndex > projectors.Count - 1)
        {
            switch (menuIndex - projectors.Count)
            {
                case 0:
                    SaveAlignment();
                    SelectedLine = SelectedProjector;
                    break;
                case 1:
                    LoadAlignment();
                    SelectedLine = SelectedProjector;
                    break;
                case 2:
                default:
                    TogglePower();
                    SelectedLine = SelectedProjector;
                    break;
            }
        }
        else
        {
            SelectedProjector = SelectedLine;
            CurrentProjector = projectors[SelectedProjector];
            SelectedLine = menuIndex;
        }
    }
    public void Up()
    {
        if (SelectedLine > 0)
            --SelectedLine;
        if (DisplayStatus)
            Select();
        UpdateDisplays();
    }
    public void Down()
    {
        ++SelectedLine;
        if (DisplayStatus)
        {
            if (SelectedLine >= projectors.Count)
                --SelectedLine;
            Select();
        }
        else
        {
            if (SelectedLine >= projectors.Count + UILines)
                --SelectedLine;
        }
        UpdateDisplays();
    }
    public void UpdateDisplays()
    {
        foreach (var display in displays)
        {
            if (DisplayStatus)
                display.RenderProjectorStatus(currentProjector, CustomName);
            else
                display.RenderMenu(SelectedLine, ProjectorMenu);
        }
    }

    internal void LoadAlignment()
    {
        var ini = program.ini;
        var iniSection = Program.iniSection;
        ini.TryParse(CurrentProjector.CustomData);
        Vector3I ProjectionRotation = new Vector3I(
            ini.Get(iniSection, "RotationX").ToInt32(),
            ini.Get(iniSection, "RotationY").ToInt32(),
            ini.Get(iniSection, "RotationZ").ToInt32()
        );
        Vector3I ProjectionOffset = new Vector3I(
            ini.Get(iniSection, "OffsetX").ToInt32(),
            ini.Get(iniSection, "OffsetY").ToInt32(),
            ini.Get(iniSection, "OffsetZ").ToInt32()
        );
        CurrentProjector.ProjectionRotation = ProjectionRotation;
        CurrentProjector.ProjectionOffset = ProjectionOffset;
        CurrentProjector.UpdateOffsetAndRotation();
    }

    internal void SaveAlignment()
    {
        MyIni ini = program.ini;
        ini.TryParse(CurrentProjector.CustomData);
        ini.Set(Program.iniSection, "RotationX", CurrentProjector.ProjectionRotation.X);
        ini.Set(Program.iniSection, "RotationY", CurrentProjector.ProjectionRotation.Y);
        ini.Set(Program.iniSection, "RotationZ", CurrentProjector.ProjectionRotation.Z);
        ini.Set(Program.iniSection, "OffsetX", CurrentProjector.ProjectionOffset.X);
        ini.Set(Program.iniSection, "OffsetY", CurrentProjector.ProjectionOffset.Y);
        ini.Set(Program.iniSection, "OffsetZ", CurrentProjector.ProjectionOffset.Z);
        CurrentProjector.CustomData = ini.ToString();
    }
    internal void TogglePower()
    {
        CurrentProjector.Enabled = !CurrentProjector.Enabled;
    }
}