

/*
        *     Version: Feb. 19th 2024 - 07:40 am CET
        *
        *     ===== HOW TO =========================================================================================
        *     ======================================================================================================
        *
        *     1.) For each room, make a group(called like you want to name your room) containing LCD/s & AirVent/s
        *
        *     2.) In the "Custom Data" of the Programmable Block, write each groupname in one line.
        *
        *     3.) Load this Script, optionally edit settings in the Settings Section and compile(Click "OK") it.
        *
        *     4.) Have Fun!
        *
        *     ======================================================================================================
        *     ======================================================================================================
        *     Sidenote: You can also first compile it and then write the Group-/Room-Names in CustomData. (bc of ez initialization #whoCares)
        */

// =========================================== SETTINGS ==================================================
//   ======================================================================================================

// === Overview LCD TAG ===
//   (Optional - But still very nice)
//     Add this TAG to some LCD !!!OUTISDE FROM A ROOM GROUP!!! and Write Room-Group-Names into the LCDs Custom Data
//       (max. 5 per Tagged LCD)

const string OverViewTag = "[OxyOv]"; //Don't remove the ""

// ======================================================================================================

// === LCD Brightness ( 0 - 255 ) ===

const byte Brightness = 50;
const byte Brightness_Holo_Transparent = 255;

// ======================================================================================================

//   ========================================= SETTINGS END ================================================


/*     ===== INFO ===== [ Not very important for common usage ]
                *
                *     -| If you add new LCDs to a group, recompile the script
                *
                *     -| The Script is only updating a Rooms LCDs when it's Pressure changed, with a small threshold.
                *
                *     -| Should be pretty Server-Friendly. Maybe "infinite" Groups could be bad for Server-Admins.
                *        If you're running a Script-Whitelist on your server and need modification - Contact me :)
                *
                *     -| Not functional Blocks getting ignored by the script, but also be used automatically again if you repair them. Just for performance.
                *
                *     -| When removing a block, it get deleted automatically internally. Just for performance.
                *
                *     -| If you find some bad bug, feel free to contact me, aswell would be cool if you can rate this script on the workshop.
                *
                */



// ========== DO NOT CHANGE ANYTHING BELOW HERE, UNLESS YOU KNOW WHAT YOU'RE DOING! ==========






bool initialized = false;
List<string> PbCdLines = new List<string>();
List<Room> Rooms = new List<Room>();
List<OverView> Overviews = new List<OverView>();
string feedback = string.Empty;
List<IMyTerminalBlock> containOvTag = new List<IMyTerminalBlock>();
List<IMyTextPanel> ovTps = new List<IMyTextPanel>();

public bool Initialize()
{
    List<string> errors = new List<string>();
    List<string> successes = new List<string>();

    PbCdLines = Me.CustomData.Split('\n')
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToArray().Distinct().ToList();

    string newCd = string.Join("\n", PbCdLines);
    Me.CustomData = newCd;

    foreach (string line in PbCdLines)
    {
        IMyBlockGroup tmpBg = GridTerminalSystem.GetBlockGroupWithName(line);
        if (tmpBg == null)
        {
            errors.Add($"-| Cannot find group {line}");
            continue;
        }
        else
        {
            Room tmpRoom = new Room(tmpBg, this);
            if (tmpRoom.Lcds.Count >= 1 && tmpRoom.Vents.Count >= 1)
            {
                Rooms.Add(tmpRoom);
                successes.Add($"-| Added group {tmpRoom.Name}");
            }
            else
            {
                if (tmpRoom.Lcds.Count < 1)
                {
                    errors.Add($"-| {line} contains no LCDs!");
                }
                if (tmpRoom.Vents.Count < 1)
                {
                    errors.Add($"-| {line} contains no Vents!");
                }
                continue;
            }
        }
    }
    if (errors.Count > 0)
    {
        feedback = "Fix Errors below:\n";
        feedback += string.Join("\n", errors.ToArray());
        return false;
    }
    else if (successes.Count >= 1 && errors.Count == 0)
    {
        //Before returning true, doing OverViewInitialization (cuz we have only existing valid groups then in PBs CustomData)
        GridTerminalSystem.SearchBlocksOfName(OverViewTag, containOvTag);
        if (containOvTag.Count >= 1)
        {
            foreach (IMyTerminalBlock tb in containOvTag)
            {
                if (tb is IMyTextPanel && !tb.BlockDefinition.SubtypeId.Contains("Corner"))
                {
                    ovTps.Add(tb as IMyTextPanel);
                }
            }
        }
        if (ovTps.Count >= 1)
        {
            foreach (IMyTextPanel tp in ovTps)
            {
                OverView tmpOv = new OverView(tp, this);
                if (tmpOv.ValidateCustomData())
                {
                    Overviews.Add(tmpOv);
                }
            }
        }

        ovTps.Sort((x, y) => x.CustomName.CompareTo(y.CustomName));
        // OverView /////////////////////////////////////////////////////////////////////////////
        feedback = "Initialization completed:\n";
        feedback += string.Join("\n", successes.ToArray());

        return true;
    }
    else
    {
        feedback = "Write Room/Groupnames in\n\"Custom Data\"";
        return false;
    }
}

private class OverView
{
    public List<Room> OvRooms { get; set; }
    public IMyTextPanel LCD { get; }
    private Program Pr;
    List<double> LastPressures = new List<double>();

    public OverView(IMyTextPanel textPanel, Program prog)
    {
        Pr = prog;
        LCD = textPanel;
        OvRooms = new List<Room>();

    }
    private void OvLcdInit()
    {
        string type = LCD.BlockDefinition.SubtypeId;

        LCD.Font = "Monospace";
        LCD.SetValue("alignment", (Int64)2);
        // LCD.ShowPublicTextOnScreen();

        if (type.Contains("Holo") || type.Contains("Transparent"))
        {
            LCD.FontColor = new Color(Brightness_Holo_Transparent, Brightness_Holo_Transparent, Brightness_Holo_Transparent);
        }
        else
        {
            LCD.FontColor = new Color(Brightness, Brightness, Brightness);
        }



        LCD.ContentType = ContentType.TEXT_AND_IMAGE;
        LCD.TextPadding = 0F;



        if (type.Contains("TextPanel") || type.Contains("5x3")) // 5=0.93 8| 4=1.18 | 3=1.6 | <=2=2.1
        {
            if (OvRooms.Count == 4)
            {
                LCD.FontSize = 0.707F;
            }
            else if (OvRooms.Count == 3)
            {
                LCD.FontSize = 0.96F;
            }
            else if (OvRooms.Count <= 2)
            {
                LCD.FontSize = 1.069F;
            }
            else
            {
                LCD.FontSize = 0.55F;
            }
        }
        else
        {
            if (type.Contains("Wide") && OvRooms.Count <= 4) // 5=0.93 8| 4=1.18 | 3=1.6 | <=2=2.1
            {
                if (OvRooms.Count == 4)
                {
                    LCD.FontSize = 1.18F;
                }
                else if (OvRooms.Count == 3)
                {
                    LCD.FontSize = 1.6F;
                }
                else if (OvRooms.Count <= 2)
                {
                    LCD.FontSize = 2.1F;
                }
                else
                {
                    LCD.FontSize = 0.938F;
                }
            }
            else
            {

                LCD.FontSize = 0.938F;
            }


        }

    }

    public bool ValidateCustomData()
    {
        List<string> cdLines = LCD.CustomData.Split('\n')
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray().Distinct().ToList();

        LCD.CustomData = String.Join("\n", cdLines);

        if (cdLines.Count > 5)
        {
            LCD.Font = "Red";
            LCD.WriteText($"\n\n\nMaximum 5 Rooms per LCD\n\nRemove atleast {cdLines.Count - 5} Rooms from Custom Data", false);
            return false;
        }
        else if (cdLines.Count == 0)
        {
            LCD.Font = "Green";
            LCD.WriteText($"\n\n\nAdd maximum 5 Rooms names\n you want to show on this\nLCD into it's Custom Data\n\nRecompile Script then", false);
            return false;
        }
        else
        {
            foreach (string line in cdLines)
            {
                if (!Pr.PbCdLines.Contains(line))
                {
                    LCD.Font = "Red";
                    LCD.WriteText($"\n\n\nOne or more Rooms in\nLCDs Custom Data do\nnot exist in the Script\n\nCheck them again\n&\nrecompile the Script", false);
                    return false;
                }
                else
                {
                    foreach (Room room in Pr.Rooms)
                    {
                        if (room.Name == line)
                        {
                            OvRooms.Add(room);
                            break;
                        }
                    }
                }
            }
        }

        OvLcdInit();

        return true;
    }

    public bool firstRun = true;



    public void WriteOvToLCD()
    {
        foreach (Room room in OvRooms)
        {
            if (room.pressureChanged || firstRun)
            {
                string output = string.Empty;
                foreach (Room r in OvRooms)
                {
                    output += $"{r.ShortName}\n{r.BarLine}{r.PercLine}\n\n";
                }
                LCD.WriteText(output, false);
                firstRun = false;
                return;
            }
        }
    }
}

private class Room
{
    public string Name { get; }
    public string ShortName { get; }
    public List<IMyAirVent> Vents { get; }
    public List<IMyTextPanel> Lcds { get; }
    public bool pressureChanged = false;

    private double lastPressure = 0.00;
    private double pressure = 0.00;
    public string BarLine = string.Empty;
    public string PercLine = string.Empty;
    private Program pr;


    public Room(IMyBlockGroup group, Program prog)
    {
        Name = group.Name;
        ShortName = TruncateName(group.Name, 26);
        Vents = new List<IMyAirVent>();
        Lcds = new List<IMyTextPanel>();

        group.GetBlocksOfType<IMyAirVent>(Vents);
        group.GetBlocksOfType<IMyTextPanel>(Lcds);

        pr = prog;

        UpdatePressure();
        lastPressure = pressure;
    }

    private static bool IsClosed(IMyTerminalBlock block)
    {
        return block.WorldMatrix == MatrixD.Identity;
    }

    public void UpdatePressure()
    {
        pressure = 0.00;
        List<double> percents = new List<double>();
        foreach (IMyAirVent vent in Vents)
        {
            if (IsClosed(vent))
            {
                Vents.Remove(vent);
                return;
            }

            percents.Add(vent.GetOxygenLevel() * 100);
        }

        if (percents != null && percents.Count >= 1)
        {
            pressure = percents.Max();
        }
        else
        {
            pressure = 0.00;
        }
    }

    private string CenterPercent(double perc)
    {
        double percent = perc;
        double.TryParse($"{percent:0.00}", out percent);

        if (percent < 10)
        {
            return $"  {percent:0.00}%";
        }
        else if (percent < 100)
        {
            return $" {percent:0.00}%";
        }
        else
        {
            return $"{percent:0.00}%";
        }
    }

    private string LoadingBar(double percentage)
    {
        string bar = "";
        double perc = percentage;
        double.TryParse($"{perc:0.00}", out perc);
        string[] bars = new string[10] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " " };
        for (int i = 1; i <= 10; i++)
        {
            if (perc >= 10 && perc >= i * 10)
            {
                bar += bars[i - 1]; //gardient
            }
            else if (perc < i * 10 && perc >= 0.05)
            {
                bar += " "; //orange
            }
            else if (perc < 0.05)
            {
                bar += " "; //red
            }
        }
        bar = bar.Substring(0, bar.Length - 1);
        return bar += "\n";
    }

    private string TruncateName(string str, int maxChars)
    {
        return str.Length <= maxChars ? str : str.Substring(0, maxChars - 3) + "...";
    }

    public void LcdSettingsInit()
    {
        foreach (IMyTextPanel panel in Lcds)
        {
            string type = panel.BlockDefinition.SubtypeId;

            panel.Font = "Monospace";
            panel.SetValue("alignment", (Int64)2);

            // panel.ShowPublicTextOnScreen(); // obsolete
            panel.ContentType = ContentType.TEXT_AND_IMAGE;
            panel.TextPadding = 0F;

            if (type.Contains("Holo") || type.Contains("Transparent"))
            {
                panel.FontColor = new Color(Brightness_Holo_Transparent, Brightness_Holo_Transparent, Brightness_Holo_Transparent);
            }
            else
            {
                panel.FontColor = new Color(Brightness, Brightness, Brightness);
            }

            if (type.Contains("Wide"))
            {
                panel.FontSize = 2.0F;
            }
            else if (type.Contains("TextPanel") || type.Contains("5x3"))
            {
                panel.FontSize = 0.707F;
            }
            else if (type.Contains("TextPanel") || type.Contains("Corner"))
            {
                panel.FontSize = 4.36F;
            }
            else
            {
                panel.FontSize = 1.0F;
            }
        }
    }

    private bool firstRun = true;

    public void WriteToLCDs()
    {
        UpdatePressure();
        BarLine = LoadingBar(pressure);
        PercLine = CenterPercent(pressure);

        if (lastPressure - pressure >= 0.005 || lastPressure - pressure <= -0.005 || firstRun)
        {
            foreach (IMyTextPanel panel in Lcds)
            {
                if (IsClosed(panel))
                {
                    Lcds.Remove(panel);
                    continue;
                }
                if (!panel.IsFunctional)
                {
                    continue;
                }
                string type = panel.BlockDefinition.SubtypeId;
                if (type.Contains("Wide"))
                {
                    panel.WriteText($"\n{ShortName}\n\n========= Oxygen =========\n\n{BarLine}{BarLine}{PercLine}\n==========================\n", false); //last line for alignbug fix
                }

                else if (type.Contains("Corner"))
                {
                    if (type.Contains("Large"))
                        panel.WriteText($"{BarLine}{BarLine}{BarLine}{BarLine}{BarLine}{BarLine}", false);
                    else
                        panel.WriteText($"{BarLine}{BarLine}{BarLine}{BarLine}{BarLine}{BarLine}{BarLine}{BarLine}", false);
                }
                else if (type.Contains("LCDPanel") || type.Contains("TextPanel") || type.Contains("TransparentLCD"))
                {
                    panel.WriteText($"\n\n\n{ShortName}\n\n\n========= Oxygen =========\n\n\n{BarLine}{BarLine}\n{PercLine}\n\n==========================\n", false);
                }
            }
            pressureChanged = true;
        }
        else
        {
            pressureChanged = false;
        }

        lastPressure = pressure;
        firstRun = false;
    }
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    if (!initialized)
    {
        initialized = Initialize();
        Echo(feedback);
        if (initialized)
        {
            foreach (Room room in Rooms)
            {
                room.LcdSettingsInit();
            }
        }
        return;
    }

    foreach (Room room in Rooms)
    {
        room.WriteToLCDs();
    }

    if (Overviews.Count >= 1)
    {
        foreach (OverView ov in Overviews)
        {
            if (ov.OvRooms.Count >= 1)
            {
                ov.WriteOvToLCD();
            }
        }
    }
}