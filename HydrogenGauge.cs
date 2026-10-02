// HYDROGEN GAUGE
// Space Engineers in-game scripting for Programmable block
// public domain code by Sean L. Palmer

// TODO sound block alarm when fuel low

const string LCDName = "LCD Hydrogen Gauge";

// TODO sound block alarm when fuel low
const string soundBlockName = "Sound Hydrogen Low";
const int warnPercent = 30;

float maxTanks; // in case tanks get destroyed
//int oldPercent = 0;

IMyTextPanel lcd;
//IMyTerminalBlock sound; //IMySoundBlock sound; // cast to IMySoundBlock breaks?!

string MakeGraph(int columns, int rows, float fill)
{
    var sb = ""; //var sb = new System.Text.StringBuilder();
    int cfilled = Math.Max(0, Math.Min(columns, (int)(fill * columns)));
    string row = new string('I', cfilled);// + new string(' ', columns - cfilled); // so the left is the FG (filled) and the right is the BG (empty)
    for (int j = rows; --j >= 0;) sb = sb + row + "\n"; //sb.AppendLine(row);
    return sb;//.ToString();
}

double Pips(IMyTextPanel screen)
{
    return (IsWide(screen) ? 2 : 1) * (8 * 73) / screen.GetValueFloat("FontSize");
}
public bool IsWide(IMyTextPanel screen)
{
    return screen.BlockDefinition.SubtypeId.Contains("Wide");
}
public int GraphicColumns(IMyTextPanel screen, int graphicCharPips)
{ // a standard LCD has 584 pips both ways, widescreen 2x width
    return (int)Math.Round(Pips(screen) / graphicCharPips);
}
public int GraphicRows(IMyTextPanel screen)
{   // a standard line is about 33 pips high
    return (int)Math.Round(584.0 / 33 / screen.GetValueFloat("FontSize"));
}

//System.Text.StringBuilder log = new System.Text.StringBuilder();
bool SameGrid(IMyTerminalBlock t) { return t.CubeGrid == Me.CubeGrid; }
// TODO group!
// there is no distinct IMyHydrogenTank, it's a subtype of IMyGasTank
void Main(string argument)
{
    string arglo = argument.ToLowerInvariant();
    if (arglo == "stop" || arglo == "reset") { // either Run with argument "reset"
        // or just recompile the script, same effect, but reset attempts to shut down gracefully
        Runtime.UpdateFrequency = UpdateFrequency.None;
        maxTanks = 0;
        //oldPercent = 0;
        if (lcd != null) 
            lcd.WriteText(""); //ClearImagesFromSelection();
        lcd = null;
        //if (sound != null) 
        //    sound.ApplyAction("Stop");
     //   sound = null;
        return; // seems good idea
    }
    if (arglo == "go") {
         Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }
    var log = "";
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(blocks, SameGrid); //t => t.CubeGrid == Me.CubeGrid); //
    double fuel = 0; double tanks = 0;
    for (int i = blocks.Count; --i >= 0; )//foreach (var b in blocks)
    {
        var b = blocks[i];
        var tank = b as IMyGasTank;
        // it's either SmallHydrogenTank or LargeHydrogenTank but there may be mod-added tanks
        //if (tank.BlockDefinition.SubtypeId.Substring(5, 8) == "Hydrogen") // only H2
        if (tank.BlockDefinition.SubtypeId.Contains("Hydrogen")) // only H2
        {
            var cap = tank.Capacity;
            fuel += tank.FilledRatio * cap; // gas fill percentage
            tanks += cap;
        }
    }
    maxTanks = (float)Math.Max(maxTanks, tanks);
    var fill = (float)(fuel / maxTanks);
    if (tanks == 0) fill = 0;
    log += "Hydrogen " + (fill * 100).ToString() + "%"; //fill.ToString("P1")); //log.AppendLine("Hydrogen " + fill.ToString("P1"));
    if (lcd == null) lcd = GridTerminalSystem.GetBlockWithName(LCDName) as IMyTextPanel;
    if (lcd != null && !lcd.IsWorking) lcd = null;
    if (lcd != null)
    {
        //if (textFontColor == null)
        //    textFontColor = lcd.GetProperty("FontColor").AsColor();
        //if (textBackgroundColor == null)
        //    textBackgroundColor = lcd.GetProperty("BackgroundColor").AsColor();
        int cols = GraphicColumns(lcd, 8);
        int rows = GraphicRows(lcd) - 5;
        string line = new string('¯', cols);
        log += "\n"; //log.AppendLine();
        log += line + "\n"; //log.AppendLine(line);
        log += MakeGraph(cols, rows, fill) + "\n"; //log.AppendLine(MakeGraph(cols, rows, fill));
        log += line + "\n"; //log.AppendLine(line);
        lcd.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        var ticks = DateTime.Now.Ticks;
        float blink = 1f - (float)Math.Abs(Math.Sin(ticks * 8e-7));
        int blinky = (int)(blink * 3);
        var fgcolor = fill <= .25 ? Color.Orange
            : fill <= .33 ? Color.Yellow
            : fill <= .5 ? Color.YellowGreen
            : fill <= .75 ? Color.GreenYellow
            : Color.Green;
        var bgcolor = fill <= .33 ? new Color(blinky, 0, 0, 255)
            : fill <= .66 ? new Color(2, 2, 0, 255) 
            : Color.Black;
        lcd.FontColor = fgcolor; //SetValue("FontColor", fgcolor);
        lcd.BackgroundColor = bgcolor; //lcd.SetValue("BackgroundColor", bgcolor);
        //if (textFontColor != null)
        //    textFontColor.SetValue(lcd, bgcolor);
        //lcd.SetValueFloat("FontSize", 2.5f));
        lcd.WriteText(log.ToString());
    }
    else Echo(log);//.ToString());
    //log.Clear();
}
