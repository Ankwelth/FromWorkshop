// Displays graphical Bars for a grids resources
// Feel free to use it as you wish, modify it, do what you will.

// TODO:
// kinngrimm:       Fill percentage range based bar colours (edit range values and colours too)
// FatalStrings:    Cargo Container Fill Meter
// BilboBilly:      Time Left for batteries - Not really the purpose of the script but we will see

static string ScriptName = "Graphical Resource Bars";
static string[] DefaultCategory = { "Available Power", "Power Usage", "Gas Stockpiles", "Farming" };
static string[] DefaultBarTitle = { "Batteries:", "Wind Speed:", "Sun Exposure:", "Reactor Fuel:", "Wind Turbines:", "Solar Panels:", "Hydrogen:", "H2 Engines:", "Oxygen:", "Water:" };
static string[] ShowBar = { "ShowBatteries", "ShowWindSpeed", "ShowSunExposure", "ShowReactorFuel", "ShowWindTurbines", "ShowSolarPanels", "ShowHydrogen", "ShowHydrogenEngines", "ShowOxygen", "ShowWater" }; // Prefs for which Bars to show

string[] orders = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };// Used in shortening values. e.g. 1,000,000,000 becomes 1G (Giga)
string[] Ticker = { ".    ", "..   ", "...  ", ".... ", "....."};// This is the animation that tells you that the script is actually running
int TickFrame = 0; // Start the animation on the first frame

float TextTopOffset = 16f; // Text Co-ords are a few px out
float textOffset = 0f;

// Information Storage
float BatteryPercent = 0f;
float MaxBatteryPower = 0f;
float SolarPercent = 0f;
float TurbinePercent = 0f;
float MaxSolarOutput = 0f;
float MaxTurbineOutput = 0f;
float O2Percent = 0f;
float O2Capacity = 0f;
float H2Capacity = 0f;
float H2Percent = 0f;
float H2ECapacity = 0f;
float H2EPercent = 0f;
float ReactorPercent = 0f;
float WaterPercent = 0f;
float Exposure = 0f;
float WindSpeed = 0f;
float WindMax = 0f;

int ReactorCount = 0;
int H2Count = 0;
int H2ECount = 0;
int O2Count = 0;
int BatteryCount = 0;
int SolarPanelCount = 0;
int TurbineCount = 0;
int WaterCount = 0;
int barCount = 0;
int Displays = 0;

static int toLeft =  0;
static int toTop = 1;
static int toRight = 2;

static float[] ThemeOffset = {
    0f, 0f, 0f, 0f, // Default Theme (aka no theme)
    8f, 46f, 8f, 0f // Amiga 500
};

List<IMyTextSurface> _drawingSurface = new List<IMyTextSurface>();
List<RectangleF> _viewport = new List<RectangleF>();

public struct Prefs {
    public Prefs(bool b)
    {
        TopOffset = 16f;
        LeftOffset = 0f;
        RightOffset = 0f;
        WhitelistMode = false;
        ShowCategoryTitles = true;
        TextMode = false;
        ShowIcons = false; // TODO
        ShowTitles = true;
        BatteryColor = new Color(255, 0, 0, 255);
        SolarPanelsColor = new Color(255, 255, 0, 255);
        SunExposureColor = new Color(255, 128, 0, 255);
        ReactorFuelColor = new Color(0, 255, 128, 255);
        WindTurbineColor = new Color(0, 0, 255, 255);
        WindSpeedColor = new Color(0, 0, 128, 255);
        HydrogenColor = new Color(0, 255, 0, 255);
        HydrogenEngineColor = new Color(0, 128, 0, 255);
        OxygenColor = new Color(0, 128, 255, 255);
        WaterColor = new Color(0, 255, 255,255);

        GoodColor = new Color(0, 255, 0, 255);
        BadColor = new Color(255, 0, 0, 255);

        Category = new List<string>();
        for (int i = 0; i < DefaultCategory.Count(); i++) Category.Add(DefaultCategory[i]);

        BarTitle = new List<string>();
        ShowBar = new List<bool>();
        for (int i = 0; i < DefaultBarTitle.Count(); i++)
        {
            BarTitle.Add(DefaultBarTitle[i]);
            ShowBar.Add(true);
        }
        MaxTextWidth = 0;
        ScreenName = "Uninitialised";
        Theme = 0;
    }
    public float TopOffset { get; set; }
    public float LeftOffset { get; set; }
    public float RightOffset { get; set; }
    public bool WhitelistMode { get; set; }
    public bool ShowCategoryTitles { get; set; }
    public bool ShowIcons { get; set; }
    public bool ShowTitles { get; set; }
    public bool TextMode { get; set; }
    public Color BatteryColor { get; set; }
    public Color SolarPanelsColor { get; set; }
    public Color SunExposureColor { get; set; }
    public Color ReactorFuelColor { get; set; }
    public Color WindTurbineColor { get; set; }
    public Color WindSpeedColor { get; set; }
    public Color HydrogenColor { get; set; }
    public Color HydrogenEngineColor { get; set; }
    public Color OxygenColor { get; set; }
    public Color WaterColor { get; set; }
    public Color GoodColor { get; set; }
    public Color BadColor { get; set; }
    public List<bool> ShowBar { get; set; }
    public List<string> BarTitle { get; set; }
    public List<string> Category { get; set; }
    public float MaxTextWidth { get; set; }
    public string ScreenName { get; set; } // For debugging purposes
    public int Theme { get; set; }
}

List<Prefs> ScreenPrefs = new List<Prefs>();

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    Initialize();
}
public void Main(string argument, UpdateType updateSource) {
    if (argument == "Refresh") Initialize();
    TickFrame += 1; // Next animation frame
    if (TickFrame >= Ticker.Count()) TickFrame = 0; // Make sure it doesn't go over the number of frames that exists

    CalcValues();   // Do all of the calculations for the Bars to Display
    DrawBars();     // Draw The Bars (Or Text in TextMode)
    DebugText();    // Write the information into the programming block for debugging.
}

public void CalcBatteries() {
    MaxBatteryPower = 0f;
    float CurrentStoredPower = 0f;
    List<IMyTerminalBlock> BatteryList = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(BatteryList, b => b.CubeGrid == Me.CubeGrid);
    BatteryCount = BatteryList.Count;
    if (BatteryCount > 0)
    {
        foreach (IMyBatteryBlock Battery in BatteryList)
        {
            MaxBatteryPower += Battery.MaxStoredPower;
            CurrentStoredPower += Battery.CurrentStoredPower;
        }
    }
    BatteryPercent = (float)Math.Round(CurrentStoredPower / MaxBatteryPower * 100f, 0);
}
public void CalcGasTanks() {
    H2Count = 0;
    O2Count = 0;
    H2Percent = 0;
    H2Capacity = 0;
    O2Percent = 0;
    O2Capacity = 0;
    List<IMyGasTank> GasTankList = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(GasTankList, t => t.CubeGrid == Me.CubeGrid);
    if (GasTankList.Count > 0)
    {
        foreach (IMyGasTank gasTank in GasTankList)
        {
            if (gasTank.BlockDefinition.SubtypeId.Contains("Hydrogen"))
            {
                H2Count++;
                H2Percent += (float)gasTank.FilledRatio;
                H2Capacity += (float)gasTank.Capacity;
            }
            else // if(gasTank.BlockDefinition.SubtypeId.Contains("Oxygen"))
            {
                O2Count++;
                O2Percent += (float)gasTank.FilledRatio;
                O2Capacity += (float)gasTank.Capacity;
            }
        }
        H2Percent /= H2Count;
        H2Percent = (float)Math.Round(H2Percent * 100f);
        if (float.IsNaN(H2Percent)) H2Percent = 0f;

        O2Percent /= O2Count;
        O2Percent = (float)Math.Round(O2Percent * 100f);
        if (float.IsNaN(O2Percent)) O2Percent = 0f;
    }
}
public void CalcH2Engines() {
    H2ECount = 0;
    H2ECapacity = 0;
    H2EPercent = 0;
    List<IMyPowerProducer> PowerProducerList = new List<IMyPowerProducer>();
    GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(PowerProducerList, t => t.CubeGrid == Me.CubeGrid);
    if (PowerProducerList.Count > 0)
    {
        foreach (IMyPowerProducer powerProducer in PowerProducerList)
        {
            if (powerProducer.BlockDefinition.SubtypeId.Contains("HydrogenEngine"))
            {
                H2ECount++;
                H2ECapacity += 100000; // Gotta write a function to grab this from the block
                float thisFillRatio = GetFillRatio(powerProducer);
                if (thisFillRatio != 666.0 && thisFillRatio != 256.0)
                {
                    H2EPercent += thisFillRatio;
                }
            }
        }
        H2EPercent /= H2ECount;
        H2EPercent = (float)Math.Round(H2EPercent * 100f);
        if (float.IsNaN(H2EPercent)) H2EPercent = 0f;
    }
}
public void CalcReactors() {
    ReactorPercent = 0f;
    ReactorCount = 0;
    float TotalReactorCapacity = 0f;
    float TotalFuel = 0f;
    List<IMyReactor> reactorList = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactorList, t => t.CubeGrid == Me.CubeGrid);
    if (reactorList.Count > 0)
    {
        IMyInventory inventory;
        float MaxVolume = 0f;
        foreach (IMyReactor reactor in reactorList)
        {
            ReactorCount++;
            inventory = reactor.GetInventory(0);
            MaxVolume = (float)inventory.MaxVolume.RawValue;
            TotalReactorCapacity += MaxVolume;
            TotalFuel += reactor.GetInventory(0).VolumeFillFactor * MaxVolume;
        }
        ReactorPercent = (TotalFuel / TotalReactorCapacity) * 100f;
        ReactorPercent = (float)Math.Round(ReactorPercent);
        if (float.IsNaN(ReactorPercent)) ReactorPercent = 0f;
    }
}
public void CalcSolarPanels() {
    MaxSolarOutput = 0f;
    float SolarOutput = 0f;
    SolarPercent = 0f; Exposure = 0f;
    List<IMyTerminalBlock> SolarPanelList = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(SolarPanelList, s => s.CubeGrid.GridSizeEnum == Me.CubeGrid.GridSizeEnum);
    SolarPanelCount = SolarPanelList.Count;
    if (SolarPanelCount > 0)
    {
        foreach (IMySolarPanel SolarPanel in SolarPanelList)
        {
            MaxSolarOutput += SolarPanel.MaxOutput;
            SolarOutput += SolarPanel.CurrentOutput;
        }
    }
    MaxSolarOutput = (float)Math.Round(MaxSolarOutput, 0);
    var MaxMax = (float)Math.Round(SolarPanelList.Count * 0.160f, 0); // 160kW Max
    Exposure = (float)Math.Round(MaxSolarOutput / MaxMax * 100f, 0);
    if (float.IsNaN(Exposure))
        Exposure = 0f;
    SolarPercent = (float)Math.Round(SolarOutput / MaxSolarOutput * 100f, 0);
    if (float.IsNaN(SolarPercent))
        SolarPercent = 0f;
}
public void CalcTurbines() {
    MaxTurbineOutput = 0f;
    float TurbineOutput = 0f;
    List<IMyWindTurbine> TurbineList = new List<IMyWindTurbine>();
    GridTerminalSystem.GetBlocksOfType<IMyWindTurbine>(TurbineList, t => t.CubeGrid == Me.CubeGrid);
    TurbineCount = TurbineList.Count;
    if (TurbineCount > 0)
    {
        foreach (IMyWindTurbine Turbine in TurbineList)
        {
            MaxTurbineOutput += Turbine.MaxOutput;
            TurbineOutput += Turbine.CurrentOutput;
        }
        WindMax = TurbineList.Count * 0.4f;
        WindSpeed = (float)Math.Round(MaxTurbineOutput / WindMax * 100f, 0);
        TurbinePercent = (float)Math.Round(TurbineOutput / MaxTurbineOutput * 100f, 0);
        if (float.IsNaN(TurbinePercent)) TurbinePercent = 0f;
        if (float.IsNaN(WindSpeed)) WindSpeed = 0f;
    }
}
public void CalcValues() {
    CalcBatteries();
    CalcSolarPanels();
    CalcReactors();
    CalcTurbines();
    CalcH2Engines();
    CalcGasTanks();
    CalcWater();
}
public void CalcWater() {
    WaterPercent = 0f;
    WaterCount = 0;
    float TotalWaterCapacity = 0f;
    float TotalIce = 0f;
    
    List<IMyFunctionalBlock> waterList = new List<IMyFunctionalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyFunctionalBlock>(waterList, t => t.CubeGrid == Me.CubeGrid && t.BlockDefinition.SubtypeId == "IrrigationSystem");
    if (waterList.Count > 0)
    {
        IMyInventory inventory;
        float MaxVolume = 0f;
        foreach (IMyFunctionalBlock waterBlock in waterList)
        {
            WaterCount++;
            inventory = waterBlock.GetInventory(0);
            MaxVolume = (float)inventory.MaxVolume.RawValue;
            TotalWaterCapacity += MaxVolume;
            TotalIce += waterBlock.GetInventory(0).VolumeFillFactor * MaxVolume;
        }
        WaterPercent = (TotalIce / TotalWaterCapacity) * 100f;
        WaterPercent = (float)Math.Round(WaterPercent);
        if (float.IsNaN(WaterPercent)) WaterPercent = 0f;
    }
}
public void DebugText() {
    //Echo("Displays: " + Displays);
    int bc = 0;
    Echo(Ticker[TickFrame]);
    Echo("--- " + DefaultCategory[0] + " ---\n");
    Echo(DefaultBarTitle[bc++] + BatteryCount + " @ " + BatteryPercent + "% of " + MaxBatteryPower + " MWh");
    Echo(DefaultBarTitle[bc++] + WindSpeed + "%");
    Echo(DefaultBarTitle[bc++] + Exposure + "%");
    Echo(DefaultBarTitle[bc++] + ReactorPercent + "% by volume");
    Echo("\n--- " + DefaultCategory[1] + " ---\n");
    Echo(DefaultBarTitle[bc++] + TurbineCount + " @ " + TurbinePercent + "% of " + WindMax + " MWh");
    Echo(DefaultBarTitle[bc++] + SolarPanelCount + " @ " + SolarPercent + "% of " + MaxSolarOutput + " MWh");
    Echo("\n--- " + DefaultCategory[2] + " ---\n");
    int groups = NumberOrders((int)H2Capacity);
    float reduced = H2Capacity / (float)Math.Pow(1000, groups);
    Echo(DefaultBarTitle[bc++] + H2Count + " @ " + H2Percent + "% of " + reduced + " " + orders[groups] + "L");
    groups = NumberOrders((int)H2ECapacity);
    reduced = H2ECapacity / (float)Math.Pow(1000, groups);
    Echo(DefaultBarTitle[bc++] + H2ECount + " @ " + H2EPercent + "% of " + reduced + " " + orders[groups] + "L");
    groups = NumberOrders((int)O2Capacity);
    reduced = O2Capacity / (float)Math.Pow(1000, groups);
    Echo(DefaultBarTitle[bc++] + O2Count + " @ " + O2Percent + "% of " + reduced + " " + orders[groups] + "L");
    Echo("\n--- " + DefaultCategory[3] + " ---\n");
    Echo(DefaultBarTitle[bc++] + WaterCount + " @ " + WaterPercent + "%");
}
public void DrawBars() {
    for (int i = 0; i < _drawingSurface.Count(); i++)
    {
        //Echo("Display: " + ScreenPrefs[i].ScreenName);
        barCount = 0;
        if (ScreenPrefs[i].TextMode)
        {
            StringBuilder OutputText = new StringBuilder();
            OutputText.Append(Ticker[TickFrame] + "\n");

            if (ScreenPrefs[i].ShowCategoryTitles && (ScreenPrefs[i].ShowBar[0] || ScreenPrefs[i].ShowBar[1] || ScreenPrefs[i].ShowBar[2] || ScreenPrefs[i].ShowBar[3])) OutputText.Append("\n--- " + ScreenPrefs[i].Category[0] + " ---\n\n");

            if (ScreenPrefs[i].ShowBar[0]) OutputText.Append(ScreenPrefs[i].BarTitle[0] + BatteryCount + " @ " + BatteryPercent + "% of " + MaxBatteryPower + " MWh\n");
            if (ScreenPrefs[i].ShowBar[1]) OutputText.Append(ScreenPrefs[i].BarTitle[1] + WindSpeed + "%\n");
            if (ScreenPrefs[i].ShowBar[2]) OutputText.Append(ScreenPrefs[i].BarTitle[2] + Exposure + "%\n");
            if (ScreenPrefs[i].ShowBar[3]) OutputText.Append(ScreenPrefs[i].BarTitle[3] + ReactorCount + " @ " + ReactorPercent + "% by volume\n");

            if (ScreenPrefs[i].ShowCategoryTitles && (ScreenPrefs[i].ShowBar[4] || ScreenPrefs[i].ShowBar[5])) OutputText.Append("\n--- " + ScreenPrefs[i].Category[1] + " ---\n\n");

            if (ScreenPrefs[i].ShowBar[4]) OutputText.Append(ScreenPrefs[i].BarTitle[4] + TurbineCount + " @ " + TurbinePercent + "% of " + WindMax + " MWh\n");
            if (ScreenPrefs[i].ShowBar[5]) OutputText.Append(ScreenPrefs[i].BarTitle[5] + SolarPanelCount + " @ " + SolarPercent + "% of " + MaxSolarOutput + " MWh\n");

            if (ScreenPrefs[i].ShowCategoryTitles && (ScreenPrefs[i].ShowBar[6] || ScreenPrefs[i].ShowBar[7] || ScreenPrefs[i].ShowBar[8])) OutputText.Append("\n--- " + ScreenPrefs[i].Category[2] + " ---\n\n");

            int groups_ = NumberOrders((int)H2Capacity);
            float reduced_ = H2Capacity / (float)Math.Pow(1000, groups_);

            if (ScreenPrefs[i].ShowBar[6]) OutputText.Append(ScreenPrefs[i].BarTitle[6] + H2Count + " @ " + H2Percent + "% of " + reduced_ + " " + orders[groups_] + "L\n");

            groups_ = NumberOrders((int)H2ECapacity);
            reduced_ = H2ECapacity / (float)Math.Pow(1000, groups_);

            if (ScreenPrefs[i].ShowBar[7]) OutputText.Append(ScreenPrefs[i].BarTitle[7] + H2ECount + " @ " + H2EPercent + "% of " + reduced_ + " " + orders[groups_] + "L\n");

            groups_ = NumberOrders((int)O2Capacity);
            reduced_ = O2Capacity / (float)Math.Pow(1000, groups_);

            if (ScreenPrefs[i].ShowBar[8]) OutputText.Append(ScreenPrefs[i].BarTitle[8] + O2Count + " @ " + O2Percent + "% of " + reduced_ + " " + orders[groups_] + "L\n");

            _drawingSurface[i].WriteText(OutputText.ToString());
        }
        else
        {
            var frame = _drawingSurface[i].DrawFrame();

            SetUpTheme(ref frame, i);
            
            if (ScreenPrefs[i].ShowCategoryTitles && (ScreenPrefs[i].ShowBar[0] || ScreenPrefs[i].ShowBar[1] || ScreenPrefs[i].ShowBar[2] || ScreenPrefs[i].ShowBar[3])) DrawSeparator(barCount++, ref frame, ScreenPrefs[i].Category[0], _drawingSurface[i].ScriptForegroundColor, i);

            if (ScreenPrefs[i].ShowBar[0]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[0], BatteryPercent, ScreenPrefs[i].BatteryColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);
            if (ScreenPrefs[i].ShowBar[1]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[1], WindSpeed, ScreenPrefs[i].WindSpeedColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);
            if (ScreenPrefs[i].ShowBar[2]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[2], Exposure, ScreenPrefs[i].SunExposureColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);
            if (ScreenPrefs[i].ShowBar[3]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[3], ReactorPercent, ScreenPrefs[i].ReactorFuelColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);

            if (ScreenPrefs[i].ShowCategoryTitles && (ScreenPrefs[i].ShowBar[4] || ScreenPrefs[i].ShowBar[5])) DrawSeparator(barCount++, ref frame, ScreenPrefs[i].Category[1], _drawingSurface[i].ScriptForegroundColor, i);

            if (ScreenPrefs[i].ShowBar[4]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[4], TurbinePercent, ScreenPrefs[i].WindTurbineColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);
            if (ScreenPrefs[i].ShowBar[5]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[5], SolarPercent, ScreenPrefs[i].SolarPanelsColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);

            if (ScreenPrefs[i].ShowCategoryTitles && (ScreenPrefs[i].ShowBar[6] || ScreenPrefs[i].ShowBar[7] || ScreenPrefs[i].ShowBar[8])) DrawSeparator(barCount++, ref frame, ScreenPrefs[i].Category[2], _drawingSurface[i].ScriptForegroundColor, i);

            if (ScreenPrefs[i].ShowBar[6]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[6], H2Percent, ScreenPrefs[i].HydrogenColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);
            if (ScreenPrefs[i].ShowBar[7]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[7], H2EPercent, ScreenPrefs[i].HydrogenEngineColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);
            if (ScreenPrefs[i].ShowBar[8]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[8], O2Percent, ScreenPrefs[i].OxygenColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);

            if (ScreenPrefs[i].ShowCategoryTitles && (ScreenPrefs[i].ShowBar[9])) DrawSeparator(barCount++, ref frame, ScreenPrefs[i].Category[3], _drawingSurface[i].ScriptForegroundColor, i);
            
            if (ScreenPrefs[i].ShowBar[9]) DrawProgressBar(barCount++, ref frame, ScreenPrefs[i].BarTitle[9], WaterPercent, ScreenPrefs[i].WaterColor, i, ScreenPrefs[i].MaxTextWidth, ScreenPrefs[i].ShowIcons, ScreenPrefs[i].ShowTitles);
  
            WrapUpTheme(ref frame, i);
            
            frame.Dispose();
        }
    }
}
public void DrawColoredBox(Vector2 pos, Vector2 size, Color color, Color border, int thickness, int screenIndex, ref MySpriteDrawFrame frame) {
    var sprite = new MySprite();
    if(thickness > 0)
    {
        // Outline Bar
        sprite = new MySprite()
        {
            Type = SpriteType.TEXTURE,
            Data = "White screen",
            Position = pos,
            Size = size,
            Color = border,
            Alignment = TextAlignment.LEFT
        };
        frame.Add(sprite);
    }
    // Foreground
    sprite = new MySprite()
    {
        Type = SpriteType.TEXTURE,
        Data = "White screen",
        Position = pos + new Vector2(thickness, 0f),
        Size = size - new Vector2(2f * thickness, 2f * thickness),
        Color = color,
        Alignment = TextAlignment.LEFT
    };
    frame.Add(sprite);
}
public void DrawProgressBar(int index, ref MySpriteDrawFrame frame, string title, float percent, Color color, int screenIndex, float maxTextLength, bool titleIsIcon = false, bool showTitles = true) {
    if (percent < 0f || float.IsInfinity(percent)) percent = 0f;
    if (!showTitles) { if (titleIsIcon) maxTextLength = 0f; else maxTextLength = -10f; }
    float overflow = 0f;
    float LeftOffset = ScreenPrefs[screenIndex].LeftOffset + textOffset + maxTextLength + GetThemeOffset(screenIndex, toLeft);
    // Outline Bar
    float barSize = (_viewport[screenIndex].Width - (LeftOffset + ScreenPrefs[screenIndex].RightOffset + GetThemeOffset(screenIndex, toRight)) - 2f);
    float barLeft = LeftOffset;
    float percentLeft = LeftOffset + (barSize / 2f);
    DrawColoredBox(new Vector2(LeftOffset, (30f * index) + ScreenPrefs[screenIndex].TopOffset + GetThemeOffset(screenIndex, toTop) - 5f) + _viewport[screenIndex].Position, new Vector2(barSize + 2f, 20f + 2f), _drawingSurface[screenIndex].ScriptBackgroundColor.Alpha(1f), _drawingSurface[screenIndex].ScriptForegroundColor.Alpha(1f), 1, screenIndex, ref frame);
    // Percentage Bar
    float percentText = percent;
    if(percent > 0f)
    {
        if (percent > 100f)
        {
            string tempString = percent.ToString("0");
            overflow = float.Parse(tempString.Substring(tempString.Length - 2));
            percent = 100f;
        }
        barSize = (_viewport[screenIndex].Width - (LeftOffset + ScreenPrefs[screenIndex].RightOffset  + GetThemeOffset(screenIndex, toRight) )) * ((percent) / 100f); // 200 is 100 to make it between 0 - 1 and  2 because it's a centre value so half the size
        DrawColoredBox(new Vector2(LeftOffset + 1f, (30f * index) + ScreenPrefs[screenIndex].TopOffset + GetThemeOffset(screenIndex, toTop) - 5f) + _viewport[screenIndex].Position, new Vector2(barSize - 2f, 20f), (ScreenPrefs[screenIndex].Theme == 1 ? new Color(255, 136, 0, 255) : color.Alpha(1f)), new Color(0,0,0,0), 0, screenIndex, ref frame);
        // Overflow (percentages higher than 100%)
        if (overflow > 0f)
        {
            barSize = (_viewport[screenIndex].Width - (LeftOffset + ScreenPrefs[screenIndex].RightOffset + GetThemeOffset(screenIndex, toRight))) * (overflow / 100f); // 200 is 100 to make it between 0 - 1 and  2 because it's a centre value so half the size
            Color shade = (ScreenPrefs[screenIndex].Theme == 1 ? new Color(127, 68, 0, 255) : new Color(color.R / 2, color.G / 2, color.B / 2));
            DrawColoredBox(new Vector2(LeftOffset + 1f, (30f * index) + ScreenPrefs[screenIndex].TopOffset + GetThemeOffset(screenIndex, toTop) - 5f) + _viewport[screenIndex].Position, new Vector2(barSize, 20f), shade, shade, 0, screenIndex, ref frame);
        }
    }
    // Percentage Text
    DrawText(new Vector2(percentLeft, (30f * index) + ScreenPrefs[screenIndex].TopOffset - TextTopOffset + GetThemeOffset(screenIndex, toTop)) + _viewport[screenIndex].Position, percentText + "%", 0.7f, _drawingSurface[screenIndex].ScriptForegroundColor.Alpha(1f), TextAlignment.CENTER, screenIndex, ref frame);
    if (showTitles)
    {
        if (!titleIsIcon)
        {
            DrawText(new Vector2(ScreenPrefs[screenIndex].LeftOffset + textOffset + GetThemeOffset(screenIndex, toLeft), (30f * index) + ScreenPrefs[screenIndex].TopOffset - TextTopOffset + GetThemeOffset(screenIndex, toTop)) + _viewport[screenIndex].Position, title, 0.7f, _drawingSurface[screenIndex].ScriptForegroundColor.Alpha(1f), TextAlignment.LEFT, screenIndex, ref frame);
        }
    }
}
public void DrawSeparator(int index, ref MySpriteDrawFrame frame, string title, Color color, int screenIndex) {
    // Title Text
    float xpos = (_viewport[screenIndex].Width - (ScreenPrefs[screenIndex].LeftOffset + ScreenPrefs[screenIndex].RightOffset)) / 2f + ScreenPrefs[screenIndex].LeftOffset;
    float ypos = ((30f * index) + ScreenPrefs[screenIndex].TopOffset + GetThemeOffset(screenIndex, toTop)) - TextTopOffset;
    DrawText(new Vector2(xpos, ypos), title, 0.7f, color, TextAlignment.CENTER, screenIndex, ref frame);
}
public void DrawText(Vector2 position, string text, float scale, Color color, TextAlignment alignment, int screenIndex, ref MySpriteDrawFrame frame) {
    var sprite = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = text,
        Position = position + new Vector2(0f, scale),
        RotationOrScale = scale,
        Color = color,
        Alignment = alignment,
        FontId = "White"
    };
    frame.Add(sprite);
}
public void DrawTurbineInfo(int index, ref MySpriteDrawFrame frame, string title, Color OnColor, Color OffColor, int screenIndex, float maxTextLength, bool Enabled, bool Functional, bool Working, bool titleIsIcon = false) {
    float leftOffset = textOffset + maxTextLength + 10f;
    // Title Text
    var sprite = new MySprite()
    {
        Type = SpriteType.TEXT,
        Data = title,
        Position = new Vector2(textOffset, (30f * index) + ScreenPrefs[screenIndex].TopOffset - TextTopOffset) + _viewport[screenIndex].Position,
        RotationOrScale = 0.8f,
        Color = _drawingSurface[screenIndex].ScriptForegroundColor.Alpha(1f),
        Alignment = TextAlignment.LEFT,
        FontId = "White"
    };
    frame.Add(sprite);
    int DotIndex = 0;
    Color white = new Color(255, 255, 255, 255);
    DrawColoredBox(new Vector2(leftOffset + (44 * DotIndex++), (30f * index) + ScreenPrefs[screenIndex].TopOffset) + _viewport[screenIndex].Position, new Vector2(20f + 2f, 20f + 2f), (Enabled ? OnColor : OffColor), white, 1, screenIndex, ref frame);
    DrawColoredBox(new Vector2(leftOffset + (44 * DotIndex++), (30f * index) + ScreenPrefs[screenIndex].TopOffset) + _viewport[screenIndex].Position, new Vector2(20f + 2f, 20f + 2f), (Functional ? OnColor : OffColor), white, 1, screenIndex, ref frame);
    DrawColoredBox(new Vector2(leftOffset + (44 * DotIndex++), (30f * index) + ScreenPrefs[screenIndex].TopOffset) + _viewport[screenIndex].Position, new Vector2(20f + 2f, 20f + 2f), (Working ? OnColor : OffColor), white, 1, screenIndex, ref frame);
}
public float GetFillRatio(IMyTerminalBlock Block) {
    if (Block.BlockDefinition.SubtypeId.Contains("HydrogenEngine"))
    {
        foreach (string line in (Block.DetailedInfo.Split('\n')))
        {
            if (line.Contains("Filled:"))
            {
                return float.Parse((line.Split(' ')[1]).Split('%')[0]) / 100f;
            }
        }
        return 255f; // Error Code for not finding ration in text
    }
    return 666f; // Error Code for when it's not even a H2 Engine
}
public float GetThemeOffset(int screenIndex, int offset) {
    float theOffset = ThemeOffset[(ScreenPrefs[screenIndex].Theme * 4) + offset];
    //Echo("to: (" + screenIndex + "): " + theOffset);
    return theOffset;
}
public void Initialize() {
    // Get the screens
    // Look for blocks with our data
    MyIni _ini = new MyIni();
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    // Only Blocks with screens AND our ini Data
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, block => block.CubeGrid == Me.CubeGrid && MyIni.HasSection(block.CustomData, ScriptName) && ((block as IMyTextSurfaceProvider)?.SurfaceCount ?? 0) >= 1);
    Displays = blocks.Count;
    int i = 0;
    if (blocks.Count > 0)
    {
        foreach (IMyTerminalBlock OutputBlock in blocks)
        {
            if (OutputBlock != null)
            {
                _ini = new MyIni();
                MyIniParseResult result;
                if (!_ini.TryParse(OutputBlock.CustomData, out result))
                    throw new Exception(result.ToString());
                // Get the value from the CustomData
                int ScreenNumber = (int)_ini.Get(ScriptName, "ScreenNumber").ToUInt64(); // Defaults to zero
                Prefs SPrefs = new Prefs(true);
                SPrefs.ScreenName = OutputBlock.CustomName;

                // Get True/False Values
                bool b;
                if (_ini.Get(ScriptName, "WhitelistMode").TryGetBoolean(out b)) { SPrefs.WhitelistMode = b; }
                if(SPrefs.WhitelistMode)
                {
                    for (int sb = 0; sb < SPrefs.ShowBar.Count(); sb++)
                        SPrefs.ShowBar[sb] = false;
                }
                // Grab all of the Preferences for which bars to show
                for (int sb = 0; sb < ShowBar.Count(); sb++)
                    if (_ini.Get(ScriptName, ShowBar[sb]).TryGetBoolean(out b)) SPrefs.ShowBar[sb] = b;

                if (_ini.Get(ScriptName, "ShowCategoryTitles").TryGetBoolean(out b)) { SPrefs.ShowCategoryTitles = b; }
                if (_ini.Get(ScriptName, "ShowIcons").TryGetBoolean(out b)) { SPrefs.ShowIcons = b; }
                if (_ini.Get(ScriptName, "ShowTitles").TryGetBoolean(out b)) { SPrefs.ShowTitles = b; }
                if (_ini.Get(ScriptName, "TextMode").TryGetBoolean(out b)) { SPrefs.TextMode = b; }

                // Get String Values
                string n;
                if (_ini.Get(ScriptName, "AvailablePowerName").TryGetString(out n)) SPrefs.Category[0] = n;

                if (_ini.Get(ScriptName, "BatteryName").TryGetString(out n)) SPrefs.BarTitle[0] = n;
                if (_ini.Get(ScriptName, "WindSpeedName").TryGetString(out n)) SPrefs.BarTitle[1] = n;
                if (_ini.Get(ScriptName, "SunExposureName").TryGetString(out n)) SPrefs.BarTitle[2] = n;
                if (_ini.Get(ScriptName, "ReactorsName").TryGetString(out n)) SPrefs.BarTitle[3] = n;

                if (_ini.Get(ScriptName, "PowerUsageName").TryGetString(out n)) SPrefs.Category[1] = n;

                if (_ini.Get(ScriptName, "WindTurbinesName").TryGetString(out n)) SPrefs.BarTitle[4] = n;
                if (_ini.Get(ScriptName, "SolarPanelsName").TryGetString(out n)) SPrefs.BarTitle[5] = n;

                if (_ini.Get(ScriptName, "GasStockpilesName").TryGetString(out n)) SPrefs.Category[2] = n;

                if (_ini.Get(ScriptName, "HydrogenName").TryGetString(out n)) SPrefs.BarTitle[6] = n;
                if (_ini.Get(ScriptName, "HydrogenEngineName").TryGetString(out n)) SPrefs.BarTitle[8] = n;
                if (_ini.Get(ScriptName, "OxygenName").TryGetString(out n)) SPrefs.BarTitle[9] = n;

                // Get Colour Code Values
                string ColorString;
                Color oc;
                if (_ini.Get(ScriptName, "BatteryColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.BatteryColor = oc; }
                if (_ini.Get(ScriptName, "WindSpeedColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.WindSpeedColor = oc; }
                if (_ini.Get(ScriptName, "SunExposureColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.SunExposureColor = oc; }
                if (_ini.Get(ScriptName, "ReactorFuelColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.ReactorFuelColor = oc; }
                if (_ini.Get(ScriptName, "WindTurbineColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.WindTurbineColor = oc; }
                if (_ini.Get(ScriptName, "SolarPanelColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.SolarPanelsColor = oc; }
                if (_ini.Get(ScriptName, "HydrogenColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.HydrogenColor = oc; }
                if (_ini.Get(ScriptName, "HydrogenEngineColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.HydrogenEngineColor = oc; }
                if (_ini.Get(ScriptName, "OxygenColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.OxygenColor = oc; }
                if (_ini.Get(ScriptName, "WaterColor").TryGetString(out ColorString)) { MakeColor(ColorString, out oc); SPrefs.WaterColor = oc; }

                // Get Floating point values
                float fv;
                if (_ini.Get(ScriptName, "TopOffset").TryGetSingle(out fv)) { SPrefs.TopOffset = fv; }
                if (_ini.Get(ScriptName, "LeftOffset").TryGetSingle(out fv)) { SPrefs.LeftOffset = fv; }
                if (_ini.Get(ScriptName, "RightOffset").TryGetSingle(out fv)) { SPrefs.RightOffset = fv; }
                
                // Get Intager values
                int iv;
                if (_ini.Get(ScriptName, "Theme").TryGetInt32(out iv)) SPrefs.Theme = iv;

                // Set up the surface to take our output
                _drawingSurface.Add((OutputBlock as IMyTextSurfaceProvider).GetSurface(ScreenNumber));
                i = _drawingSurface.Count - 1;
                _viewport.Add(new RectangleF((_drawingSurface[i].TextureSize - _drawingSurface[i].SurfaceSize) / 2f, _drawingSurface[i].SurfaceSize));
                if (SPrefs.TextMode)
                {
                    _drawingSurface[i].ContentType = ContentType.TEXT_AND_IMAGE;
                }
                else
                {
                    _drawingSurface[i].ContentType = ContentType.SCRIPT;
                    _drawingSurface[i].Script = ""; // None of the built in scripts, use this instead
                }
                // TODO: if the screen is not text mode then get the max title width based on which bars are wanted
                if (!SPrefs.TextMode)
                {
                    float BarWidth = 0;
                    for (int s = 0; s < SPrefs.BarTitle.Count(); s++)
                    {
                        if (SPrefs.ShowBar[s])
                        {
                            BarWidth = _drawingSurface[i].MeasureStringInPixels(new StringBuilder(SPrefs.BarTitle[s]), "White", 0.8f).X;
                            if (BarWidth > SPrefs.MaxTextWidth)
                                SPrefs.MaxTextWidth = BarWidth;
                        }
                    }
                }
                ScreenPrefs.Add(SPrefs); // each screen has it's own settings
            }
        }
    }
}
public void MakeColor(string ColorString, out Color oc) {
    string[] RGB;
    RGB = ColorString.Split(',');
    oc = new Color(Int32.Parse(RGB[0]), Int32.Parse(RGB[1]), Int32.Parse(RGB[2]));
}
public int NumberOrders(int value) {
    Stack<int> q = new Stack<int>();
    do
    {
        q.Push(value % 1000);
        value /= 1000;
    } while (value > 0);
    return q.ToArray().Count() - 1;
}
public void SetUpTheme(ref MySpriteDrawFrame frame, int screenIndex) {
    switch(ScreenPrefs[screenIndex].Theme)
    {
        case 0:
        break;
        case 1:
            string windowTitle = "Kiasanth";
            float windowTitleWidth = _drawingSurface[screenIndex].MeasureStringInPixels(new StringBuilder(windowTitle), "White", 0.7f).X;
            Color white = new Color(255, 255, 255, 255);
            Color blue = new Color(0, 85, 170,255);
            Color black = new Color(0, 0, 0,255);
            _drawingSurface[screenIndex].ScriptBackgroundColor = blue;
            _drawingSurface[screenIndex].ScriptForegroundColor = white;
            // Screen
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset, ScreenPrefs[screenIndex].TopOffset - 5f) + _viewport[screenIndex].Position, new Vector2(_viewport[screenIndex].Width - (ScreenPrefs[screenIndex].LeftOffset + ScreenPrefs[screenIndex].RightOffset) - 53f, 20f), white, white, 0, screenIndex, ref frame); // Screen Title Bar
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 51f, ScreenPrefs[screenIndex].TopOffset - 5f) + _viewport[screenIndex].Position, new Vector2(22f, 20f), white, white, 0, screenIndex, ref frame); // Screen Back Button
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 51f + 2f, ScreenPrefs[screenIndex].TopOffset - 5f - 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), white, blue, 2, screenIndex, ref frame); // Hollow box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 51f + 6f, ScreenPrefs[screenIndex].TopOffset - 5f + 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), black, blue, 0, screenIndex, ref frame); // Black box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 27f, ScreenPrefs[screenIndex].TopOffset - 5f) + _viewport[screenIndex].Position, new Vector2(22f, 20f), white, white, 0, screenIndex, ref frame); // Screen Front Button
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 27f + 2f, ScreenPrefs[screenIndex].TopOffset - 5f - 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), black, blue, 0, screenIndex, ref frame); // Black box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 27f + 6f, ScreenPrefs[screenIndex].TopOffset - 5f + 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), white, blue, 2, screenIndex, ref frame); // Hollow box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 3f, ScreenPrefs[screenIndex].TopOffset - 5f) + _viewport[screenIndex].Position, new Vector2(3f, 20f), white, white, 0, screenIndex, ref frame); // Little extra bar thing
            DrawText(new Vector2(10f + textOffset + ScreenPrefs[screenIndex].LeftOffset, ScreenPrefs[screenIndex].TopOffset - TextTopOffset) + _viewport[screenIndex].Position, "Amiga Workbench 1.3.2", 0.7f, blue, TextAlignment.LEFT, screenIndex, ref frame);
            // Window            
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset, ScreenPrefs[screenIndex].TopOffset + 18f) + _viewport[screenIndex].Position, new Vector2(_viewport[screenIndex].Width - (ScreenPrefs[screenIndex].LeftOffset + ScreenPrefs[screenIndex].RightOffset) - 53f, 20f), white, white, 0, screenIndex, ref frame); // Screen Title Bar
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset + 4f, ScreenPrefs[screenIndex].TopOffset + 18f) + _viewport[screenIndex].Position, new Vector2(22f, 24f), white, blue, 2, screenIndex, ref frame); // Close Button
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset + 8f, ScreenPrefs[screenIndex].TopOffset + 18f) + _viewport[screenIndex].Position, new Vector2(14f, 14f), white, blue, 2, screenIndex, ref frame); // Close Box
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset + 13f, ScreenPrefs[screenIndex].TopOffset + 18f) + _viewport[screenIndex].Position, new Vector2(4f, 4f), black, blue, 0, screenIndex, ref frame); // Close Dot
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 51f, ScreenPrefs[screenIndex].TopOffset + 18f) + _viewport[screenIndex].Position, new Vector2(22f, 20f), white, blue, 0, screenIndex, ref frame); // Screen Back Button
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 51f + 2f, ScreenPrefs[screenIndex].TopOffset + 18f - 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), white, blue, 2, screenIndex, ref frame); // Hollow box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 51f + 6f, ScreenPrefs[screenIndex].TopOffset + 18f + 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), black, blue, 0, screenIndex, ref frame); // Black box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 27f, ScreenPrefs[screenIndex].TopOffset + 18f) + _viewport[screenIndex].Position, new Vector2(22f, 20f), white, blue, 0, screenIndex, ref frame); // Screen Front Button
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 27f + 2f, ScreenPrefs[screenIndex].TopOffset + 18f - 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), black, blue, 0, screenIndex, ref frame); // Black box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 27f + 6f, ScreenPrefs[screenIndex].TopOffset + 18f + 2f) + _viewport[screenIndex].Position, new Vector2(14f, 12f), white, blue, 2, screenIndex, ref frame); // Hollow box
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 3f, ScreenPrefs[screenIndex].TopOffset + 18f) + _viewport[screenIndex].Position, new Vector2(3f, 20f), white, white, 0, screenIndex, ref frame); // Little extra bar thing
            DrawText(new Vector2(30f + textOffset + ScreenPrefs[screenIndex].LeftOffset, ScreenPrefs[screenIndex].TopOffset - TextTopOffset + 22f) + _viewport[screenIndex].Position, "Kiasanth", 0.7f, blue, TextAlignment.LEFT, screenIndex, ref frame);
            float lineGap = 34f + windowTitleWidth;
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset + lineGap, ScreenPrefs[screenIndex].TopOffset + 14f) + _viewport[screenIndex].Position, new Vector2(_viewport[screenIndex].Width - (ScreenPrefs[screenIndex].LeftOffset + ScreenPrefs[screenIndex].RightOffset) - lineGap - 57f, 4f), blue, white, 0, screenIndex, ref frame); // drag bar line 1
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset + lineGap, ScreenPrefs[screenIndex].TopOffset + 22f) + _viewport[screenIndex].Position, new Vector2(_viewport[screenIndex].Width - (ScreenPrefs[screenIndex].LeftOffset + ScreenPrefs[screenIndex].RightOffset) - lineGap - 57f, 4f), blue, white, 0, screenIndex, ref frame); // drag bar line 1
        break;
    }
}
public void WrapUpTheme(ref MySpriteDrawFrame frame, int screenIndex) {
    switch(ScreenPrefs[screenIndex].Theme)
    {
        case 0:
        break;
        case 1:
            Color white = new Color(255, 255, 255, 255);
            Color blue = new Color(0, 85, 170,255);
            float borderHeight = (barCount * 30f + 2f);
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset, ScreenPrefs[screenIndex].TopOffset + (borderHeight / 2f) + 24f) + _viewport[screenIndex].Position, new Vector2(2f, borderHeight), white, white, 0, screenIndex, ref frame); // Left Window Border
            DrawColoredBox(new Vector2(ScreenPrefs[screenIndex].LeftOffset, ScreenPrefs[screenIndex].TopOffset + borderHeight + 24f) + _viewport[screenIndex].Position, new Vector2(_viewport[screenIndex].Width - (ScreenPrefs[screenIndex].LeftOffset + ScreenPrefs[screenIndex].RightOffset), 2f), white, white, 0, screenIndex, ref frame); // Left Window Border
            DrawColoredBox(new Vector2(_viewport[screenIndex].Width - ScreenPrefs[screenIndex].RightOffset - 2f, ScreenPrefs[screenIndex].TopOffset + (borderHeight / 2f) + 24f) + _viewport[screenIndex].Position, new Vector2(2f, borderHeight), white, white, 0, screenIndex, ref frame); // Right Window Border
        break;
    }
}
// LargeBlockFarmPlot
// IrrigationSystem on/off status
