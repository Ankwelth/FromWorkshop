/* // Fancy LCD Sign Script Version 1.2
 * Refer to the workshop page for this script for a more detailed setup guide at:
 * https://steamcommunity.com/sharedfiles/filedetails/?id=2805529124
 * How to setup: (Basic)
 *    1. Place this script inside a programmable block.
 *  2. Make sure each LCD which you want to be a sign has the tag "FLCD" (Case sensitive) in its name.
 *  3. Run the script with argument "get_blocks".
 *  4. Open the custom data of each sign to configure it, the signs will auto update after a few seconds.
 *  5. Done! Each sign should now be ready!
 * Examples are at the bottom of the workshop page if you need it.
 * 
 * Made by Viruz
 */

// Script details
public readonly string S_VERSION = "1.1";
public readonly string S_NAME = "Fancy LCD Sign Script";

// All script functionality is handled by the "Script" field. The base program only handles arguments, outputting to the terminal and calling the script methods every cycle.
public MyCommandLine ArgsParser = new MyCommandLine();
public readonly ProgramManager Script = new ProgramManager();
public readonly SpriteData ProgramSpriteData = new SpriteData();
public readonly Dictionary<string, Action> ArgumentPairs;

public Program()
{
    // Set all new commands here, IMPORTANT: Make sure every key contains no uppercase letters.
    ArgumentPairs = new Dictionary<string, Action>()
    {
        { "get_blocks", Script.GetBlocks },
        { "update", Script.Update },
        { "reset", Script.SetTemplate }
    };

    // Link the script's functionality to the programmable block.
    Script.PBScript = this;
    Script.Update();
    Script.GetBlocks();

    // The script only needs to be ran periodically.
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument)
{
    // Show the details of the script. Uses constants from before.
    Echo($"// {S_NAME} Version {S_VERSION}");

    // Handle and call argument methods.
    if (ArgsParser.TryParse(argument))
    {
        Action action;
        if (ArgumentPairs.TryGetValue(ArgsParser.Argument(0).ToLower(), out action))
        {
            action.Invoke();
        }
    }

    // This is usually ran when the script is first loaded into the PB. However this is also ran if a player clears the custom data.
    if (string.IsNullOrWhiteSpace(Me.CustomData))
    {
        // Sets the default config template, the default values entered can be found in the ConfigDefaults class. (Annoying that static stuff can't be used :/)
        Script.SetTemplate();
    }

    if (Script.IsOperational)
    {
        // Perhaps in future versions, list the name of each sign which isn't working properly.

        int workingCount = 0;
        // Just to let someone know that the script is running and some stats.
        Echo($"Script is operational...\nBlock refresh in {Math.Round((Script.NextRefreshTime - DateTime.Now).TotalSeconds)} seconds.\n");

        // This method refreshes the data of each sign according to each signs own custom data, if the user modifies it, this method will update that sign to what the custom data says.
        // The method itself doesn't do anything until the counter for "NextRefreshTime" has been reached which will then reset "NextRefreshTime" to add whatever the refresh rate is set to.
        Script.TimerRefresh();

        // Redraw each sign per update to account for any changes.
        foreach (LCDSign sign in Script.Signs)
        {
            sign.Refresh();

            // Additionally, this loop will get how many of the signs are working and display the number.
            if (sign.IsOperational)
            {
                workingCount++;
            }
        }

        // This output includes:
        //   -The total number of signs
        //   -The number of working signs
        //   -The commands available (If the arguments are changed at all, the string has to be modified)
        //   -Some additional information.
        Echo($"// Statistics:\n" +
            $"  -Sign Count: {Script.Signs.Count}\n" +
            $"  -Working Signs: {workingCount}\n\n" +
            $"// Available Commands:\n" +
            $"  -get_blocks: Fetches the LCD blocks with the tag \"{Script.Tag}\" in their name.\n" +
            $"  -update: Updates the settings of the script, use if you have changed anything in the custom data.\n" +
            $"  -reset: Sets the custom data box of this programmable block to a default template.\n\n" +
            $"A list of available images can be found on the workshop page for this script.\n\n" +
            $"NOTE: You can also directly use the sprite data name if you need to. (E.g. \"AH_BoreSight\" works as well as \"Crosshair\")");
    }
}

// This class contains some of the default configuration values for the script or for individual signs (Again, can't use statics, so use readonly values :/)
public class ConfigDefaults
{
    public readonly string DEFAULT_FONT_FAMILY = "Monospace";
    public readonly double DEFAULT_FONT_SIZE = 2.0;
    public readonly string DEFAULT_BLOCK_TAG = "FLCD";
    public readonly int DEFAULT_REFRESH_RATE = 5;
}

// The main class which will handle the functionallity of the script, including getting configuration data.
public class ProgramManager
{
    public Program PBScript { get; set; }
    public readonly ConfigDefaults ConfigDefaults = new ConfigDefaults();
    public List<LCDSign> Signs { get; private set; }
    public string Tag { get; set; }
    public int RefreshRate { get; set; }
    public bool IsOperational { get; private set; }
    public string BlankCustomData { get; private set; }
    public DateTime NextRefreshTime { get; private set; } = DateTime.Now;

    public ProgramManager()
    {
        // Set the default values, at all times, every single configuration value has to explicitly not be null. Set them to the defaults.
        Tag = ConfigDefaults.DEFAULT_BLOCK_TAG;
        RefreshRate = ConfigDefaults.DEFAULT_REFRESH_RATE;

        // This looks like a mess but idk any other way to do it that is better so its fine.
        // The "blankCustomDataIni" is essentially a blank template per LCD sign which is automatically set when the LCD is loaded.
        string configSection = "Fancy LCD Script - Config";
        string[] sections = new string[] { "Fancy LCD Script - Left Image", "Fancy LCD Script - Right Image" };

        MyIni blankCustomDataIni = new MyIni();

        blankCustomDataIni.AddSection(configSection);
        blankCustomDataIni.Set(configSection, "Sign Text", "");
        blankCustomDataIni.SetComment(configSection, "Sign Text", " If you want to insert multiple lines, add a \"|\" character at the start of each newline, so for example:\n Sign Text=\n |Ship Exit\n |WARNING: DEPRESSURIZED");
        blankCustomDataIni.Set(configSection, "Font Family", ConfigDefaults.DEFAULT_FONT_FAMILY);
        blankCustomDataIni.Set(configSection, "Font Size", ConfigDefaults.DEFAULT_FONT_SIZE);
        blankCustomDataIni.Set(configSection, "Text Top Margin", "Auto");
        blankCustomDataIni.Set(configSection, "Use Grid Background", "False");
        blankCustomDataIni.Set(configSection, "Grid Colour", "Auto");

        // Set the configuration for the left and right image since they are both essentially the same except when drawing the frame.
        foreach (string section in sections)
        {
            blankCustomDataIni.AddSection(section);
            blankCustomDataIni.Set(section, "Sprite", "Arrow");
            blankCustomDataIni.Set(section, "Colour", "255 255 255");
            blankCustomDataIni.Set(section, "Size", "Auto");
            blankCustomDataIni.Set(section, "Rotation Angle (Degrees)", 0);
            blankCustomDataIni.Set(section, "Border (Pixels)", "10");
        }

        // Set the blank custom data as the just created template.
        BlankCustomData = blankCustomDataIni.ToString();
    }

    // Gets each block which contains the tag string, which is then added to the signs list. This method is also called when the player runs the programmable block with argument "get_blocks"
    public void GetBlocks()
    {
        Signs = GetDesignatedBlocks(Tag, false).Select(
        x => {
            LCDSign sign = new LCDSign(x, this);
            sign.UpdateData();

            return sign;
        }).ToList();
        IsOperational = true;
    }

    // Updates the configuration for the script, the configuration can be found in the custom data box of the programmable block.
    public void Update()
    {
        MyIni scriptSettings = new MyIni();
        IsOperational = false;
        // Use a single constant string for easy changes.
        const string INI_SETTINGS_SECTION_NAME = "Fancy LCD Script - Script Settings";

        // If the configuration could not be parsed, the IsOperational property is set to false, this means that the script will refuse to run until the player tries again.
        if (scriptSettings.TryParse(PBScript.Me.CustomData))
        {
            if (!scriptSettings.ContainsSection(INI_SETTINGS_SECTION_NAME))
            {
                return;
            }

            // Instead of the typical approach of throwing an exception when a value is missing or invalid, just set the values to their default. Much more convienent this way.
            string blockTag = scriptSettings.Get(INI_SETTINGS_SECTION_NAME, "Block Tag").ToString(ConfigDefaults.DEFAULT_BLOCK_TAG);
            int refreshRate = scriptSettings.Get(INI_SETTINGS_SECTION_NAME, "Refresh Rate (Seconds)").ToInt32(ConfigDefaults.DEFAULT_REFRESH_RATE);

            // Finally, set the properties/configuration of this script to the above values retrieved from the custom data box.
            Tag = blockTag == null ? ConfigDefaults.DEFAULT_BLOCK_TAG : blockTag;
            RefreshRate = refreshRate;
            IsOperational = true;
        }
        else
        {
            // This line isn't actually necessary as it is set to false by default and at the start of this method. But keep because its easier to read.
            IsOperational = false;
        }
    }

    // Generates a blank template for configuration using the default values set when the script is compiled in the ConfigSettings class. The custom data box of the programmable block is then set as this template.
    public void SetTemplate()
    {
        MyIni customDataIni = new MyIni();
        const string SECTION_NAME = "Fancy LCD Script - Script Settings";

        customDataIni.Set(SECTION_NAME, "Block Tag", ConfigDefaults.DEFAULT_BLOCK_TAG);
        customDataIni.Set(SECTION_NAME, "Refresh Rate (Seconds)", ConfigDefaults.DEFAULT_REFRESH_RATE);

        PBScript.Me.CustomData = customDataIni.ToString();
    }

    public void TimerRefresh()
    {
        // When the time elapsed since the last refresh has reached zero, call the UpdateSigns() method which will redraw every sign.
        if (DateTime.Now >= NextRefreshTime)
        {
            UpdateSigns();

            // Set the next point in time where this method will be called, this is the refresh rate gathered in the custom data configuration.
            NextRefreshTime = DateTime.Now.AddSeconds(RefreshRate);
        }
    }

    private void UpdateSigns()
    {
        // Pretty self explanatory, just update the configuration of each sign using the custom data section of every sign.
        foreach (LCDSign sign in Signs)
        {
            sign.UpdateData();
        }
    }

    // Old.
    public List<IMyTextPanel> GetDesignatedBlocks(string tag, bool startsWith)
    {
        // Gather each text panel on the grid using the GridTerminalSystem, then filter it using LINQ based on whether the name of each block contains the set tag.
        List<IMyTextPanel> gridLCDs = new List<IMyTextPanel>();
        PBScript.GridTerminalSystem.GetBlocksOfType(gridLCDs);

        // Now filter each block.
        List<IMyTextPanel> taggedLCDs = gridLCDs.Where(x => startsWith ? x.CustomName.StartsWith(tag) : x.CustomName.Contains(tag)).ToList();

        // Return the list, this may seem redundant as the "Signs" property can just be set directly since there isn't anything else which will use this method.
        // But just in case I decide to expand it, use this for now.
        return taggedLCDs;
    }
}

// Each LCDSign object represents one sign on the grid, this class provides methods to do things like draw the sprites and text and get the configuration data for the sign
public class LCDSign
{
    public IMyTextPanel Screen { get; }
    // Since statics aren't allowed, a reference to the manager is needed per sign.
    public ProgramManager Script { get; set; }
    // Configuration stuff for this sign.
    public string Text { get; set; }
    public string Font { get; set; }
    public float FontSize { get; set; }
    public float TextTopMargin { get; set; }
    public bool UseGridBackground { get; set; }
    public Color GridBackgroundColour { get; set; }
    // Holds the data per image for sprites. The reason this is separated from this class into its own struct is because it avoids repeating itself since theres 2 and also I may want to expand to include more images in future versions.
    public SignImage LeftSprite { get; private set; }
    public SignImage RightSprite { get; private set; }
    // Indicates whether this sign is operational. Pretty self explanatory.
    public bool IsOperational { get; private set; }

    public LCDSign(IMyTextPanel screen, ProgramManager script)
    {
        Screen = screen;
        Script = script;

        // Because the signs are gathered presumably when new signs, set the custom data to a blank template.
        if (Screen.CustomData == "")
        {
            FillCustomData();
        }
        else
        {
            // Since data is already available in the custom data section, attempt to parse it.
            UpdateData();
        }
    }

    // Simply sets the custom data section of the lcd to a blank template.
    private void FillCustomData() => Screen.CustomData = Script.BlankCustomData;

    // This method will draw the sprites such as the left and right images and the text aswell as (if included), the background.
    public void Refresh()
    {
        // Since this method is called often, might aswell check if the custom data is empty and fill it.
        if (string.IsNullOrWhiteSpace(Screen.CustomData))
        {
            Screen.CustomData = Script.BlankCustomData;
        }

        // ISSUE: Changing content type breaks signs on dedicated servers.
        // Screen.ContentType = ContentType.SCRIPT;
        // Screen.Script = "None";

        using (MySpriteDrawFrame frame = Screen.DrawFrame())
        {
            // Important, refer to: https://github.com/malware-dev/MDK-SE/wiki/Text-Panels-and-Drawing-Sprites
            RectangleF viewport = new RectangleF(
                (Screen.TextureSize - Screen.SurfaceSize) / 2f,
                Screen.SurfaceSize
            );

            // Set the size to auto size, and if the image has a manual size specified, use that.
            // This is worse for performance, but it shouldn't really matter too much.
            int baseSize = (int)((viewport.Width > viewport.Height ? viewport.Height : viewport.Width) * 0.8);
            Vector2 leftSize = new Vector2(baseSize, baseSize);
            Vector2 rightSize = new Vector2(baseSize, baseSize);

            // Auto sizing is indicated by whether the size Vector2 in the SignImage struct has a value, if null, auto size is used, and if not, use the value.
            if (LeftSprite.Size.HasValue)
            {
                leftSize = LeftSprite.Size.Value;
            }

            // Same with the other image...
            if (RightSprite.Size.HasValue)
            {
                rightSize = RightSprite.Size.Value;
            }

            // Create the image sprites and set the appropriate data.
            MySprite leftImageSprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Color = LeftSprite.Colour,
                RotationOrScale = Utilities.AnglesToRadians(LeftSprite.RotationAngle),
                Data = LeftSprite.Sprite,
                Size = leftSize,
                Position = new Vector2(viewport.X + LeftSprite.Border, viewport.Center.Y)
            };

            MySprite rightImageSprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Color = RightSprite.Colour,
                RotationOrScale = Utilities.AnglesToRadians(RightSprite.RotationAngle),
                Data = RightSprite.Sprite,
                Size = rightSize,
                Position = new Vector2((viewport.Right - RightSprite.Border) - rightSize.X, viewport.Center.Y)
            };

            float topMargin = TextTopMargin == -1 ? viewport.Center.Y - (Screen.MeasureStringInPixels(new StringBuilder(Text), Font, FontSize).Y / 2) : TextTopMargin;

            // Add the text bit.
            MySprite text = new MySprite()
            {
                Type = SpriteType.TEXT,
                FontId = Font,
                Data = Text,
                RotationOrScale = FontSize,
                Alignment = TextAlignment.CENTER,
                Color = Screen.ScriptForegroundColor,
                Position = new Vector2(viewport.Center.X, topMargin)
            };

            // Add the grid background if specified.
            // TODO: Perhaps add multiple background choices
            if (UseGridBackground)
            {
                MySprite background = new MySprite()
                {
                    Type = SpriteType.TEXTURE,
                    Color = GridBackgroundColour,
                    Data = "Grid",
                    Size = new Vector2(viewport.Width > viewport.Height ? viewport.Width : viewport.Height),
                    Position = new Vector2(0, 0)
                };

                frame.Add(background);
            }

            // Finally, add each sprite, and dispose of the frame.
            frame.Add(leftImageSprite);
            frame.Add(rightImageSprite);
            frame.Add(text);
        }
    }

    public void UpdateData()
    {
        MyIni ini = new MyIni();

        // Parse the custom data of the LCD. If unsuccessful, the sign won't be redrawn.
        if (ini.TryParse(Screen.CustomData))
        {
            // Using an array for each section means that it is easy to modify.
            string[] sections = new string[] { "Fancy LCD Script - Config", "Fancy LCD Script - Left Image", "Fancy LCD Script - Right Image" };

            // Check that each section is atleast in the custom data. If not, there is no point in continuing.
            foreach (string section in sections)
            {
                if (!ini.ContainsSection(section))
                {
                    IsOperational = false;
                    return;
                }
            }

            // This is set to true here since instead of throwing an error when checking each value, I simply substitue with the default value.
            // This makes it less prone to exceptions and just easier to manage.
            IsOperational = true;

            // Gather the standard configuration values, the specific values aren't important here. Mainly just get them and set the sign properties to them.
            string textValue = ini.Get(sections[0], "Sign Text").ToString();
            string fontValue = ini.Get(sections[0], "Font Family").ToString(Script.ConfigDefaults.DEFAULT_FONT_FAMILY);
            double textMargin = ini.Get(sections[0], "Text Top Margin").ToDouble(defaultValue: -1);
            double fontSize = ini.Get(sections[0], "Font Size").ToDouble(Script.ConfigDefaults.DEFAULT_FONT_SIZE);
            bool useGridBackground = ini.Get(sections[0], "Use Grid Background").ToBoolean(false);

            Color gridColour;
            // To explain this method, this method parses a standard text representation of an RGB colour, namely 3 integers separated by spaces such as: 153 50 2 which is brown.
            // The method is also smarter than just parsing the raw string, so basically non number/non whitespace characters will be filtered out and only the numbers will be processed.
            // This essentialy means that something such as (153, 50, 2) or (R: 153, G: 50, B: 2) is still valid as all of the unwanted characters will just not be counted.
            Utilities.TryGetRGBColourFromString(ini.Get(sections[0], "Grid Colour").ToString(), out gridColour);

            // Get the data for each image.
            SignImage left = ParseImageConfigData(ini, sections[1]);
            SignImage right = ParseImageConfigData(ini, sections[2]);

            // Set the data for the LCDSign to the values just retrived.
            Text = textValue;
            Font = fontValue;
            FontSize = (float)fontSize;
            TextTopMargin = (float)textMargin;
            UseGridBackground = useGridBackground;
            GridBackgroundColour = gridColour;

            LeftSprite = left;
            RightSprite = right;

            Screen.CustomData = ini.ToString();
        }
        else
        {
            IsOperational = false;
        }
    }

    private SignImage ParseImageConfigData(MyIni ini, string section)
    {
        // Get the data for one image. Since all the images in a sign are the same, this method can be used.
        string spriteValue = Script.PBScript.ProgramSpriteData.GetDataNameFromNickName(ini.Get(section, "Sprite").ToString());
        string colourValue = ini.Get(section, "Colour").ToString();
        Color colour;
        int rotationAngle;
        Vector2? size;
        int border = ini.Get(section, "Border (Pixels)").ToInt32(10);

        if (!Utilities.TryGetRGBColourFromString(colourValue, out colour))
        {
            if (colourValue.ToLower() == "auto")
            {
                colour = Color.White;
            }
        }

        // Similar to the "TryGetRGBColourFromString()" method except only 2 numbers are needed this time.
        Utilities.TryGetSizeFromString(ini.Get(section, "Size").ToString(), out size);

        if (!int.TryParse(ini.Get(section, "Rotation Angle (Degrees)").ToString().Replace("°", ""), out rotationAngle))
        {
            rotationAngle = 0;
        }

        return new SignImage(spriteValue, rotationAngle, size, border, colour);
    }
}

// An SignImage stores the data for an image on one of the signs. This doesn't need to have anything fancy with its own methods, only store data.
public struct SignImage
{
    public string Sprite;
    public double RotationAngle;
    public Vector2? Size;
    public int Border;
    public Color Colour;

    public SignImage(string sprite, double rotationAngle, Vector2? size, int border, Color colour)
    {
        Sprite = sprite;
        RotationAngle = rotationAngle;
        Size = size;
        Border = border;
        Colour = colour;
    }
}

// A (once static) class which is dedicated to nicknames for sprites.
public class SpriteData
{
    // All of the sprite listings can be found at: https://github.com/malware-dev/MDK-SE/issues/187
    // This provides translations for nicknames for alot of sprites which have weird or complex nicknames, I tried my best to summarise most sprites into a short phrase or word.
    private Dictionary<string, string> nickToDataNamePairs = new Dictionary<string, string>()
    {
        { "cross", "Cross" },
        { "arrow", "Arrow" },
        { "warning", "Danger" },
        { "stop", "No Entry" },
        { "construction", "Construction" },
        { "rectangle", "SquareSimple" },
        { "circle", "Circle" },
        { "circle hollow", "HollowCircle" },
        { "hollow arrow", "AH_PullUp" },
        { "hollow box", "AH_Textbox" },
        { "trinity", @"Textures\FactionLogo\Builders\BuilderIcon_2.dds" },
        { "left bracket", "DecorativeBracketLeft" },
        { "right bracket", "DecorativeBracketRight" },
        { "hydrogen", "IconHydrogen" },
        { "oxygen", "IconOxygen" },
        { "energy", "IconEnergy" },
        { "radioactive", @"Textures\FactionLogo\Others\OtherIcon_19.dds" },
        { "triangle", "Triangle" },
        { "crosshair", "AH_BoreSight" },
        { "target crosshair", "AH_VelocityVector" },
        { "sc coin", "LCD_Economy_SingleCoin" },
        { "shopping trolley", "StoreBlock2" },
        { "cubes", @"Textures\FactionLogo\Builders\BuilderIcon_1.dds" },
        { "atomic", @"Textures\FactionLogo\Others\OtherIcon_18.dds" },
        { "atomic 2", @"Textures\FactionLogo\Builders\BuilderIcon_13.dds" },
        { "rocket", @"Textures\FactionLogo\Builders\BuilderIcon_14.dds" },
        { "construction tools", @"Textures\FactionLogo\Builders\BuilderIcon_15.dds" },
        { "tool", @"Textures\FactionLogo\Builders\BuilderIcon_16.dds" },
        { "faction icon crown", @"Textures\FactionLogo\Builders\BuilderIcon_10.dds" },
        { "faction icon triangle", @"Textures\FactionLogo\Others\OtherIcon_12.dds" },
        { "power tools", @"Textures\FactionLogo\Builders\BuilderIcon_4.dds" },
        { "shield", @"Textures\FactionLogo\Builders\BuilderIcon_5.dds"},
        { "gears", @"Textures\FactionLogo\Builders\BuilderIcon_7.dds" },
        { "helmet", @"Textures\FactionLogo\Builders\BuilderIcon_8.dds" },
        { "crown", @"Textures\FactionLogo\Builders\BuilderIcon_9.dds" },
        { "drill 1", @"Textures\FactionLogo\Miners\MinerIcon_1.dds" },
        { "drill 2", @"Textures\FactionLogo\Miners\MinerIcon_2.dds" },
        { "drill 3", @"Textures\FactionLogo\Miners\MinerIcon_3.dds" },
        { "drill 4", @"Textures\FactionLogo\Miners\MinerIcon_4.dds" },
        { "weapon emblem", @"Textures\FactionLogo\Others\OtherIcon_1.dds" },
        { "tool emblem", @"Textures\FactionLogo\Others\OtherIcon_10.dds" },
        { "space engineer", @"Textures\FactionLogo\Others\OtherIcon_11.dds" },
        { "crowned space engineer", @"Textures\FactionLogo\Others\OtherIcon_17.dds" },
        { "construction crane", @"Textures\FactionLogo\Others\OtherIcon_2.dds" },
        { "other faction icon 1", @"Textures\FactionLogo\Others\OtherIcon_20.dds" },
        { "other faction icon 2", @"Textures\FactionLogo\Others\OtherIcon_21.dds" },
        { "other faction icon 3", @"Textures\FactionLogo\Others\OtherIcon_28.dds" },
        { "other faction icon 4", @"Textures\FactionLogo\Others\OtherIcon_29.dds" },
        { "other faction icon 5", @"Textures\FactionLogo\Others\OtherIcon_7.dds" },
        { "other faction icon 6", @"Textures\FactionLogo\Builders\BuilderIcon_3.dds" },
        { "other faction icon 7", @"Textures\FactionLogo\Builders\BuilderIcon_6.dds" },
        { "other faction icon 8", @"Textures\FactionLogo\Builders\BuilderIcon_12.dds" },
        { "gear", @"Textures\FactionLogo\Others\OtherIcon_22.dds" },
        { "mushroom cloud", @"Textures\FactionLogo\Others\OtherIcon_23.dds" },
        { "solar system", @"Textures\FactionLogo\Others\OtherIcon_24.dds" },
        { "compass", @"Textures\FactionLogo\Others\OtherIcon_26.dds" },
        { "compass 2", @"Textures\FactionLogo\Others\OtherIcon_30.dds" },
        { "energy 2", @"Textures\FactionLogo\Others\OtherIcon_27.dds" },
        { "skull 1", @"Textures\FactionLogo\Others\OtherIcon_3.dds" },
        { "skull 2", @"Textures\FactionLogo\Others\OtherIcon_31.dds" },
        { "hexagons", @"Textures\FactionLogo\Others\OtherIcon_32.dds" },
        { "rocket 2", @"Textures\FactionLogo\Others\OtherIcon_33.dds"  },
        { "fist", @"Textures\FactionLogo\Others\OtherIcon_4.dds" },
        { "rocket orbit", @"Textures\FactionLogo\Others\OtherIcon_6.dds" },
        { "triangle pattern", @"Textures\FactionLogo\Others\OtherIcon_8.dds" },
        { "helmet glasses", @"Textures\FactionLogo\Others\OtherIcon_9.dds" },
        { "pirate", @"Textures\FactionLogo\PirateIcon.dds" },
        { "spider", @"Textures\FactionLogo\Spiders.dds" },
        { "flags", @"Textures\FactionLogo\Traders\TraderIcon_1.dds" },
        { "business", @"Textures\FactionLogo\Traders\TraderIcon_2.dds" },
        { "money", @"Textures\FactionLogo\Traders\TraderIcon_3.dds" },
        { "money 2", @"Textures\FactionLogo\Traders\TraderIcon_4.dds" },
        { "space engineer tie", @"Textures\FactionLogo\Traders\TraderIcon_5.dds" },
        { "magazine 1", @"MyObjectBuilder_AmmoMagazine/AutocannonClip" },
        { "magazine 2", "MyObjectBuilder_AmmoMagazine/AutomaticRifleGun_Mag_20rd" },
        { "magazine 3", "MyObjectBuilder_AmmoMagazine/ElitePistolMagazine" },
        { "magazine 4", "MyObjectBuilder_AmmoMagazine/FullAutoPistolMagazine" },
        { "magazine 5", "MyObjectBuilder_AmmoMagazine/NATO_5p56x45mm" },
        { "magazine 6", "MyObjectBuilder_AmmoMagazine/PreciseAutomaticRifleGun_Mag_5rd" },
        { "magazine 7", "MyObjectBuilder_AmmoMagazine/RapidFireAutomaticRifleGun_Mag_50rd" },
        { "magazine 8", "MyObjectBuilder_AmmoMagazine/SemiAutoPistolMagazine" },
        { "magazine 9", "MyObjectBuilder_AmmoMagazine/UltimateAutomaticRifleGun_Mag_30rd" },
        { "shell 1", "MyObjectBuilder_AmmoMagazine/LargeCalibreAmmo" },
        { "shell 2", "MyObjectBuilder_AmmoMagazine/MediumCalibreAmmo" },
        { "large sabot", "MyObjectBuilder_AmmoMagazine/LargeRailgunAmmo" },
        { "small sabot", "MyObjectBuilder_AmmoMagazine/SmallRailgunAmmo" },
        { "missile", "MyObjectBuilder_AmmoMagazine/Missile200mm" },
        { "ammo box", "MyObjectBuilder_AmmoMagazine/NATO_25x184mm" },
        { "bulletproof glass", "MyObjectBuilder_Component/BulletproofGlass" },
        { "canvas", "MyObjectBuilder_Component/Canvas" },
        { "computer", "MyObjectBuilder_Component/Computer" },
        { "construction component", "MyObjectBuilder_Component/Construction" },
        { "detector component", "MyObjectBuilder_Component/Detector" },
        { "display", "MyObjectBuilder_Component/Display" },
        { "explosives", "MyObjectBuilder_Component/Explosives" },
        { "girder", "MyObjectBuilder_Component/Girder" },
        { "gravity component", "MyObjectBuilder_Component/GravityGenerator" },
        { "interior plate", "MyObjectBuilder_Component/InteriorPlate" },
        { "large steel tube", "MyObjectBuilder_Component/LargeTube" },
        { "medical componment", "MyObjectBuilder_Component/Medical" },
        { "metal grid", "MyObjectBuilder_Component/MetalGrid" },
        { "motor", "MyObjectBuilder_Component/Motor" },
        { "power cell", "MyObjectBuilder_Component/PowerCell" },
        { "radio-comm comp", "MyObjectBuilder_Component/RadioCommunication" },
        { "reactor component", "MyObjectBuilder_Component/Reactor" },
        { "small steel tube", "MyObjectBuilder_Component/SmallTube" },
        { "solar cell", "MyObjectBuilder_Component/SolarCell" },
        { "steel plate", "MyObjectBuilder_Component/SteelPlate" },
        { "superconductor", "MyObjectBuilder_Component/Superconductor" },
        { "thruster component", "MyObjectBuilder_Component/Thrust" },
        { "zone chip", "MyObjectBuilder_Component/ZoneChip" },
        { "clang cola", "MyObjectBuilder_ConsumableItem/ClangCola" },
        { "cosmic coffee", "MyObjectBuilder_ConsumableItem/CosmicCoffee" },
        { "medkit", "MyObjectBuilder_ConsumableItem/Medkit" },
        { "powerkit", "MyObjectBuilder_ConsumableItem/Powerkit" },
        { "datapad", "MyObjectBuilder_Datapad/Datapad" },
        { "hydrogen bottle", "MyObjectBuilder_GasContainerObject/HydrogenBottle" },
        { "oxygen bottle", "MyObjectBuilder_OxygenContainerObject/OxygenBottle" },
        { "cobalt ingot", "MyObjectBuilder_Ingot/Cobalt" },
        { "gold ingot", "MyObjectBuilder_Ingot/Gold" },
        { "iron ingot", "MyObjectBuilder_Ingot/Iron" },
        { "magnesium ingot", "MyObjectBuilder_Ingot/Magnesiu" },
        { "nickel ingot", "MyObjectBuilder_Ingot/Nickel" },
        { "platinum ingot", "MyObjectBuilder_Ingot/Platinum" },
        { "scrap ingot", "MyObjectBuilder_Ingot/Scrap" },
        { "silicon ingot", "MyObjectBuilder_Ingot/Silicon" },
        { "silver ingot", "MyObjectBuilder_Ingot/Silver" },
        { "stone ingot", "MyObjectBuilder_Ingot/Stone" },
        { "uranium ingot", "MyObjectBuilder_Ingot/Uranium" },
        { "stone", "MyObjectBuilder_Ore/Stone" },
        { "organic", "MyObjectBuilder_Ore/Organic" },
        { "scrap", "MyObjectBuilder_Ore/Scrap" },
        { "cobalt ore", "MyObjectBuilder_Ore/Cobalt" },
        { "gold ore", "MyObjectBuilder_Ore/Gold" },
        { "ice ore", "MyObjectBuilder_Ore/Ice" },
        { "iron ore", "MyObjectBuilder_Ore/Iron" },
        { "magnesium ore", "MyObjectBuilder_Ore/Magnesium" },
        { "nickel ore", "MyObjectBuilder_Ore/Nickel" },
        { "platinum ore", "MyObjectBuilder_Ore/Platinum" },
        { "silicon ore", "MyObjectBuilder_Ore/Silicon" },
        { "silver ore", "MyObjectBuilder_Ore/Silver" },
        { "uranium ore", "MyObjectBuilder_Ore/Uranium" },
        { "package", "MyObjectBuilder_Package/Package" },
        { "rocket launcher 1", "MyObjectBuilder_PhysicalGunObject/BasicHandHeldLauncherItem" },
        { "rocket launcher 2", "MyObjectBuilder_PhysicalGunObject/AdvancedHandHeldLauncherItem" },
        { "welder", "MyObjectBuilder_PhysicalGunObject/WelderItem" },
        { "enhanced welder", "MyObjectBuilder_PhysicalGunObject/Welder2Item" },
        { "proficent welder", "MyObjectBuilder_PhysicalGunObject/Welder3Item" },
        { "elite welder", "MyObjectBuilder_PhysicalGunObject/Welder4Item" },
        { "grinder", "MyObjectBuilder_PhysicalGunObject/AngleGrinderItem" },
        { "enhanced grinder", "MyObjectBuilder_PhysicalGunObject/AngleGrinder2Item" },
        { "proficent grinder", "MyObjectBuilder_PhysicalGunObject/AngleGrinder3Item" },
        { "elite grinder", "MyObjectBuilder_PhysicalGunObject/AngleGrinder4Item" },
        { "drill", "MyObjectBuilder_PhysicalGunObject/HandDrillItem" },
        { "enhanced drill", "MyObjectBuilder_PhysicalGunObject/HandDrill2Item" },
        { "proficent drill", "MyObjectBuilder_PhysicalGunObject/HandDrill3Item" },
        { "elite drill", "MyObjectBuilder_PhysicalGunObject/HandDrill4Item" },
        { "rifle 1", "MyObjectBuilder_PhysicalGunObject/AutomaticRifleItem" },
        { "rifle 2", "MyObjectBuilder_PhysicalGunObject/PreciseAutomaticRifleItem" },
        { "rifle 4", "MyObjectBuilder_PhysicalGunObject/UltimateAutomaticRifleItem" },
        { "rifle 3", "MyObjectBuilder_PhysicalGunObject/RapidFireAutomaticRifleItem" },
        { "pistol 1", "MyObjectBuilder_PhysicalGunObject/ElitePistolItem" },
        { "pistol 2", "MyObjectBuilder_PhysicalGunObject/FullAutoPistolItem" },
        { "pistol 3", "MyObjectBuilder_PhysicalGunObject/SemiAutoPistolItem" },
        { "space credits", "MyObjectBuilder_PhysicalObject/SpaceCredit" },
        { "thumbs up", "MyObjectBuilder_PhysicalGunObject/GoodAIRewardPunishmentTool" },
    };

    // Method which will retrive a value from the dictionary above, this should be the only way values are retrieved from the above dictionary, this makes the dictionary case insensitive.
    // Using this method also means that if a direct data name (such as "AH_VelocityVector") which isn't a key in the dictionary is entered it will still be a valid sprite.
    public string GetDataNameFromNickName(string nickname)
    {
        string key = nickname.ToLower();
        if (nickToDataNamePairs.ContainsKey(key))
        {
            return nickToDataNamePairs[key];
        }
        else
        {
            return nickname;
        }
    }
}

// Provides basic utilites.
public static class Utilities
{
    public static float AnglesToRadians(double degrees) => (float)((float)degrees * Math.PI / 180);
    // Attempts to parse a given string into a VRageMath.Color object. Will filter any letter characters. E.g. 255 100 25. If the string is one number, every colour channel will be set to that colour.
    public static bool TryGetRGBColourFromString(string colourString, out Color colour)
    {
        // Set because C# doesn't allow returning before setting an out reference.
        colour = Color.White;

        int[] channels;
        // Separated into its own method because I can use it for other multi number objects such as Vectors.
        // If the value is a valid colour, process it, otherwise, check if its a single number and if not, just set as white.
        if (GetSeparatedNumberValues(colourString, 3, out channels))
        {
            colour = new Color()
            {
                R = (byte)channels[0],
                G = (byte)channels[1],
                B = (byte)channels[2],
                A = 255
            };

            return true;
        }
        else
        {
            // Attempt to parse the string as a single number, if not, then the string cannot be understood and the default colour of white will be set.
            int result;

            if (int.TryParse(colourString, out result))
            {
                colour = new Color()
                {
                    R = (byte)result,
                    G = (byte)result,
                    B = (byte)result,
                    A = 255
                };

                return true;
            }

            return false;
        }
    }

    // Sets a Vector2 value from a string which contains 2 numbers.
    public static bool TryGetSizeFromString(string sizeString, out Vector2? size)
    {
        int[] dimensions;

        if (GetSeparatedNumberValues(sizeString, 2, out dimensions))
        {
            size = new Vector2(dimensions[0], dimensions[1]);
            return true;
        }
        else
        {
            // Attempt to parse the string as a single number, this will mean that the "size" will be a square.
            int result;
            if (int.TryParse(sizeString, out result))
            {
                size = new Vector2(value: result);

                return true;
            }

            // Nothing else which can be checked, so simply set as null and return false to indicate it wasn't successfully converted.
            size = null;

            return false;
        }
    }

    // What the conversion from string methods which are above use to extract the numbers out of the string.
    private static bool GetSeparatedNumberValues(string totalValue, int length, out int[] values)
    {
        // Create the array which contain the values.
        values = new int[length];
        // Discards cannot be used in C# 6.0 so just use an unused discard variable.
        int discard;

        // Filter the string given to only contain number characters or spaces. (LINQ can be used since a string implements IEnumerable)
        string filtered = new string(totalValue.Where(x => char.IsNumber(x) || char.IsWhiteSpace(x)).ToArray());
        string[] stringChannels = filtered.Split(' ').Where(x => int.TryParse(x, out discard)).ToArray();

        // Check if the length desired is the length of the array.
        if (stringChannels.Length != length)
        {
            return false;
        }

        for (int i = 0; i < length; i++)
        {
            int channel;
            if (int.TryParse(stringChannels[i], out channel))
            {
                if (channel >= 0 && channel <= 255)
                {
                    values[i] = (byte)channel;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    // Not used in script, only for developing, still useful.
    public static string ConvertStringToTitleCase(string str)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower()).Replace("\n", " ");
    }
}