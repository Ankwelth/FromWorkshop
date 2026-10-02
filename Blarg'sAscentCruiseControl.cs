
/*
Blargmode's Ascent Cruise Control (version 1.7, 2023-09-12)

Tired of wasting fuel when leaving a gravity well? What you need is cruise control!

This script adjusts the thrust of your rear thrusters to the lowest possible without losing speed.


___/ Setup: \\__________

1. Install script.
2. Sit in flight seat and add the script to the toolbar: 'Run' and leave argument empty.

The script looks for an occupied flight seat every 5 seconds until it finds one, to determine what is forward. 
Once one is found, it stops looking and saves the seat internally. If the Wrong seat is stored, you can hop into
the correct one, access the Programmable block, type 'reset' as the argument, and press run.


___/ Usage: \\__________
Press the button you set up in step two to turn the cruise control on or off.


___/ Optional extras: \\__________

The script can show status on LCDs, both regular and corner.
It can also show if it's engaged or not using a light.

In either case, just add the tag #ACC to the name of the light/LCD.

You can have more than one light/LCD. 

Cruise Control can also be engaged via a button, a sensor, or any other action. 



___/ Settings: \\__________

Are found in Custom Data.
After changing one, press run with an empty argument field, or start the cruise control.

Some settings can also be changed via argument. 
Enter the argument and press run. Also works from the toolbar.

'95' or any other number set the target speed. (Keep it below the max speed though, the script
does not work efficiently at max speed).

'swap' switches between rear and bottom thrusters.

'on', 'off', and 'toggle' turns cruise control on, or off.



___/ Controlled descent: \\__________

Note, this is an unintended feature I found handy, so I kept it.

You can set the Target speed to a negative value. If you do, you have a controlled descent, backwards.
Be sure to take manual control when you get close to landing and be careful aligning the ship properly.


Warning: The following is an experimental feature. It can be dangerous to use with weak thrust, or if you
point the ship in the wrong direction.

As you're descending, the script will scale down your target speed in relation to your altitude. In theory 
you could land like that. But it's dangerous to do. It's intended to help slowing down when descending, not
to be full auto. Note that your altitude is measured at the center of mass, so the landing
might not be gentle.
And again, if your ship is too weak, or you've angled it wrong, or the terrain is weird, this could be your death.
In my testing target speeds exceeding -95m/s have been fatal.





















*/

public class Auxillary
{
    bool lastState = false;

    List<IMyLightingBlock> Lights = new List<IMyLightingBlock>();
    List<IMyTimerBlock> Timers = new List<IMyTimerBlock>();

    public void AddLight(IMyLightingBlock light)
    {
        Lights.Add(light);
        light.Enabled = false;
    }

    public void AddTimer(IMyTimerBlock timer)
    {
        Timers.Add(timer);
    }

    public void Update(bool state)
    {
        if (lastState == state) return;
        lastState = state;
        Timers.ForEach(x => x.Trigger());
        Lights.ForEach(x => x.Enabled = state);
    }

    public int CountTimers()
    {
        return Timers.Count;
    }

    public int CountLights()
    {
        return Timers.Count;
    }
}


class CruiseControl
{

    public bool Active { get; private set; } = false;
    public double TargetSpeed { get; private set; } // Target speed can be modified when closing in on a planet.
    public double Speed { get; private set; } = -1;
    public float ThrustOverride { get; private set; } = 1;
    public ThrustDirection ThrustDirection { get; private set; }
    public double Cutoff { get; private set; }
    public bool DisableAtGravTransition { get; private set; }
    public double GroundOffset { get; private set; }

    double specifiedTargetSpeed; // Target speed can be modified when closing in on a planet, so the original value is stored here.
    Dictionary<ThrustDirection, ThrusterSet> thrusters;
    IMyShipController controller;
    bool startedOutsideGravity;
    TimeSpan lastSpeedMeasurement = TimeSpan.MaxValue;

    public CruiseControl(Dictionary<ThrustDirection, ThrusterSet> thrusters, IMyShipController controller, Properties props)
    {
        this.thrusters = thrusters;
        this.TargetSpeed = props.TargetSpeed;
        this.specifiedTargetSpeed = props.TargetSpeed;
        this.ThrustDirection = props.ThrustDirection;
        this.Cutoff = props.Cutoff;
        this.DisableAtGravTransition = props.DisableAtGravTransition;
        this.GroundOffset = props.GroundOffset;
        this.controller = controller;
    }

    /// <summary>
    /// Needs to run continously to update thrusters.
    /// time is the script running time in ms. Like Arduinos millis()
    /// </summary>
    public void Loop(TimeSpan time)
    {
        if (!Active) return;
        double currentSpeed = CalcSpeed();
        double acceleration = 0;
        // Only make calculations when speed has been set.
        if (Speed != -1)
        {
            // Calculate thrust
            acceleration = Math.Round((currentSpeed - Speed) / (time - lastSpeedMeasurement).TotalSeconds, 3);
            double difference = TargetSpeed - currentSpeed;
            double errorMagnitude = Math.Abs(difference / TargetSpeed);
            if (difference < acceleration)
            {
                ThrustOverride -= (float)errorMagnitude;
            }
            else
            {
                ThrustOverride += (float)errorMagnitude;
            }
            if (ThrustOverride < 0) ThrustOverride = 0.00001f;
            else if (ThrustOverride > 1) ThrustOverride = 1;

            // Update thrusters
            foreach (var thruster in thrusters[ThrustDirection].Main)
            {
                if (thruster.MaxEffectiveThrust / thruster.MaxThrust <= Cutoff) thruster.ThrustOverridePercentage = 0;
                else thruster.ThrustOverridePercentage = ThrustOverride;
            }
        }

        // Check if it's time to disable
        if (DisableAtGravTransition)
        {
            if (!InGravity() && !startedOutsideGravity)
            {
                Disable();
            }
        }

        // Reduce target speed when closing in on the ground
        if (TargetSpeed < 0)
        {
            double height;
            if (controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out height))
            {
                double newTarget = -((height - GroundOffset) / 10.0);
                if (newTarget > -1) newTarget = -1;
                if (TargetSpeed < newTarget) TargetSpeed = newTarget;

                // Turn off if on the gruond
                if (height <= GroundOffset) Disable();
            }
        }

        // Update variables for next iteration
        Speed = currentSpeed;
        lastSpeedMeasurement = time;
    }

    public void Enable()
    {
        Active = true;
        TargetSpeed = specifiedTargetSpeed;
        if (TargetSpeed < 0) ThrustOverride = 1;
        thrusters[ThrustDirection].Reverse.ForEach(x => x.Enabled = false);
        thrusters[ThrustDirection].Main.ForEach(x => x.ThrustOverridePercentage = ThrustOverride);
        startedOutsideGravity = !InGravity();
    }

    public void Disable()
    {
        Active = false;
        TargetSpeed = specifiedTargetSpeed;
        thrusters[ThrustDirection].Reverse.ForEach(x => x.Enabled = true);
        thrusters[ThrustDirection].Main.ForEach(x => x.ThrustOverridePercentage = 0);
    }

    public void SwapThrustDirection()
    {
        SetThrustDirection(ThrustDirection == ThrustDirection.Rear ? ThrustDirection.Bottom : ThrustDirection.Rear);
    }

    // Setters

    public void SetTargetSpeed(double speed)
    {
        TargetSpeed = speed;
        specifiedTargetSpeed = speed;
    }

    public void SetThrustDirection(ThrustDirection direction)
    {
        // Don't update if the thrust direction isn't valid.
        if (!(direction == ThrustDirection.Rear || direction == ThrustDirection.Bottom)) return;
        // Reset current thrusters
        if (Active)
        {
            thrusters[ThrustDirection].Reverse.ForEach(x => x.Enabled = true);
            thrusters[ThrustDirection].Main.ForEach(x => x.ThrustOverridePercentage = 0);
        }
        ThrustDirection = direction;
        // Prepare next set of thrusters
        if (Active)
        {
            thrusters[direction].Reverse.ForEach(x => x.Enabled = false);
            thrusters[direction].Main.ForEach(x => x.ThrustOverridePercentage = ThrustOverride);
        }
    }

    public void SetCutoff(double cutoff)
    {
        Cutoff = cutoff;
    }

    public void SetDisableAtGravTransition(bool disable)
    {
        DisableAtGravTransition = disable;
    }

    public void SetGroundOffset(double offset)
    {
        GroundOffset = offset;
    }

    // Internal

    private double CalcSpeed()
    {
        var vel = controller.GetShipVelocities().LinearVelocity;
        double speed;
        if (ThrustDirection == ThrustDirection.Bottom)
        {
            speed = Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix)).Y;
        }
        else
        {
            speed = -Vector3D.TransformNormal(vel, MatrixD.Transpose(controller.WorldMatrix)).Z;
        }
        return speed;
    }

    private bool InGravity()
    {
        return controller.GetNaturalGravity().Length() > 0.0;
    }

    // Structs

    public struct ThrusterSet
    {
        public List<IMyThrust> Main { get; set; }
        public List<IMyThrust> Reverse { get; set; }
    }

    public struct Properties
    {
        public double TargetSpeed { get; set; }
        public ThrustDirection ThrustDirection { get; set; }
        public double Cutoff { get; set; }
        public bool DisableAtGravTransition { get; set; }
        public double GroundOffset { get; set; }
    }
}


class Display
{
    public const string font = "White"; // Can't use "Default" as MeasureStringInPixels doesn't work on it.

    Program P;
    IMyTextSurface surface;

    public Vector2 Size;
    public Vector2 TopLeft;
    public Vector2 BGSize;
    public Vector2 Center;
    public float SmallestSize;
    public float Width;
    public float FontSize;

    bool slim = false;

    public Display(IMyTextSurface surface, Program p)
    {
        P = p;
        this.surface = surface;
        surface.ContentType = ContentType.SCRIPT;
        surface.Script = "";

        if (surface.SurfaceSize.X > surface.SurfaceSize.Y)
        {
            BGSize = new Vector2(surface.SurfaceSize.X, surface.SurfaceSize.X);
            SmallestSize = surface.SurfaceSize.Y;
        }
        else
        {
            BGSize = new Vector2(surface.SurfaceSize.Y, surface.SurfaceSize.Y);
            SmallestSize = surface.SurfaceSize.X;
        }

        Width = surface.SurfaceSize.X;

        Size = surface.SurfaceSize;

        TopLeft = (surface.TextureSize - surface.SurfaceSize) * 0.5f;

        Center = surface.TextureSize * 0.5f;

        FontSize = surface.SurfaceSize.X * 0.004f; // 0.005 Debug

        // Super wide LCDs needs even smaller font size. 
        // There's only one vanilla LCD like this, the corner LCD.
        if (surface.SurfaceSize.X / surface.SurfaceSize.Y > 2)
        {
            FontSize = surface.SurfaceSize.X * 0.003f; // 0.002 Debug
            slim = true;
        }
    }

    public void DrawIssue() {
        using (var frame = surface.DrawFrame())
        {
            Color foreground = surface.ScriptForegroundColor;
            if (slim)
            {
                var fontSize = FontSize * 0.8f;
                var gapSize = surface.MeasureStringInPixels(new StringBuilder("M"), font, fontSize);
                AddKeyValue(frame, "Problem", "Check programmable block", fontSize, Center.Y - gapSize.Y * 1.2f, gapSize, foreground, false);
            } else {
                var fontSize = FontSize * 0.8f;
                var gapSize = surface.MeasureStringInPixels(new StringBuilder("M"), font, fontSize);
                AddText(frame, "Problem:\nCheck the\nprogramable\nblock.", fontSize, Center, foreground);
                // Script name
                var offset = TextHeight(FontSize * 0.5f);
                AddBanner(frame, "Ascent Cruise Control", FontSize * .5f, new Vector2(Center.X, TopLeft.Y + offset), foreground, Color.DarkSlateGray);
            }
        }
    }

    public void Draw(bool active, double speed, double target, double thrustOverride, ThrustDirection thrustDirection)
    {
        if (slim)
        {
            DrawSlim(active, speed, target, thrustOverride, thrustDirection);
        }
        else
        {
            DrawDefault(active, speed, target, thrustOverride, thrustDirection);
        }
    }

    public void DrawSlim(bool active, double speed, double target, double thrustOverride, ThrustDirection thrustDirection)
    {
        // TODO: Somehow adjust to resolution. I think there are several resolutions, but possible only 2 PPIs
        using (var frame = surface.DrawFrame())
        {
            // Using with of display as main measurement. Practically all LCDs are wider than they are tall. And the content is wider than tall.
            // This should fit well with the existing vanilla "scripts"


            Color foreground = surface.ScriptForegroundColor;

            var fontSize = FontSize * 0.8f;
            var gapSize = surface.MeasureStringInPixels(new StringBuilder("M"), font, fontSize);
            var veritcalOffset = gapSize.Y * 0.5f;

            var targetText = new StringBuilder(target.ToString("F0"));
            var targetSize = surface.MeasureStringInPixels(targetText, font, fontSize);
            var targetSprite = MySprite.CreateText(targetText.ToString(), font, foreground, fontSize);

            if (active)
            {
                var speedText = new StringBuilder(speed.ToString("F1") + " m/s");
                var speedSize = surface.MeasureStringInPixels(speedText, font, fontSize);
                var speedSprite = MySprite.CreateText(speedText.ToString(), font, foreground, fontSize);

                var overrideText = new StringBuilder(thrustOverride.ToString("P1"));
                var overrideSize = surface.MeasureStringInPixels(overrideText, font, fontSize);
                var overrideSprite = MySprite.CreateText(overrideText.ToString(), font, foreground, fontSize);

                var totalWidth = speedSize.X + targetSize.X + overrideSize.X + gapSize.X * 2;
                var speedPos = Center.X - totalWidth / 2 + speedSize.X / 2;
                var targetPos = gapSize.X + speedPos + speedSize.X / 2 + targetSize.X / 2;
                var overridePos = gapSize.X + targetPos + targetSize.X / 2 + overrideSize.X / 2;

                speedSprite.Position = new Vector2(speedPos, Center.Y - veritcalOffset);
                targetSprite.Position = new Vector2(targetPos, Center.Y - veritcalOffset);
                overrideSprite.Position = new Vector2(overridePos, Center.Y - veritcalOffset);

                frame.Add(speedSprite);
                frame.Add(targetSprite);
                frame.Add(overrideSprite);

                frame.Add(new MySprite(SpriteType.TEXTURE, "AH_TextBox", new Vector2(targetPos, Center.Y), targetSize * 1.2f, color: foreground));
            }
            else
            {
                var statusText = new StringBuilder("Ready");
                var statusSize = surface.MeasureStringInPixels(statusText, font, fontSize);
                var statusSprite = MySprite.CreateText(statusText.ToString(), font, foreground, fontSize);

                var totalWidth = statusSize.X + targetSize.X + gapSize.X;
                var statusPos = Center.X - totalWidth / 2 + statusSize.X / 2;
                var targetPos = gapSize.X + statusPos + statusSize.X / 2 + targetSize.X / 2;

                statusSprite.Position = new Vector2(statusPos, Center.Y - veritcalOffset);
                targetSprite.Position = new Vector2(targetPos, Center.Y - veritcalOffset);

                frame.Add(statusSprite);
                frame.Add(targetSprite);

                frame.Add(new MySprite(SpriteType.TEXTURE, "AH_TextBox", new Vector2(targetPos, Center.Y), targetSize * 1.2f, color: foreground));
            }

            if (Size.Y >= gapSize.Y * 4)
            {
                // Script name
                var offset = TextHeight(FontSize * 0.5f);
                //frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(Center.X, TopLeft.Y + offset), new Vector2(Width, FontSize), color: Color.Gray));
                //AddTextBox(frame, "Cruise Control", FontSize * .5f, new Vector2(Center.X, TopLeft.Y + offset), foreground);
                AddBanner(frame, "Ascent Cruise Control", FontSize * .5f, new Vector2(Center.X, TopLeft.Y + offset), foreground, Color.DarkSlateGray);
            }


        }
    }

    public void DrawDefault(bool active, double speed, double target, double thrustOverride, ThrustDirection thrustDirection)
    {
        // TODO: Somehow adjust to resolution. I think there are several resolutions, but possible only 2 PPIs
        using (var frame = surface.DrawFrame())
        {
            // Using with of display as main measurement. Practically all LCDs are wider than they are tall. And the content is wider than tall.
            // This should fit well with the existing vanilla "scripts"


            Color foreground = surface.ScriptForegroundColor;

            var fontSize = FontSize * 0.8f;
            var gapSize = surface.MeasureStringInPixels(new StringBuilder("M"), font, fontSize);
            var veritcalOffset = gapSize.Y * 0.5f;

            var targetText = new StringBuilder(target.ToString("F0"));
            var targetSize = surface.MeasureStringInPixels(targetText, font, fontSize);
            var targetSprite = MySprite.CreateText(targetText.ToString(), font, foreground, fontSize);

            if (active)
            {
                AddKeyValue(frame, "Status:", "Engaged", fontSize, Center.Y - gapSize.Y * 1.2f, gapSize, foreground, false);
                AddKeyValue(frame, "Target speed:", target.ToString("F0"), fontSize, Center.Y, gapSize, foreground, true);
                AddKeyValue(frame, "Speed:", speed.ToString("F1") + " m/s", fontSize, Center.Y + gapSize.Y * 1.2f, gapSize, foreground, false);
            }
            else
            {
                AddKeyValue(frame, "Status:", "Ready", fontSize, Center.Y - gapSize.Y * .7f, gapSize, foreground, true);
                AddKeyValue(frame, "Target speed:", target.ToString("F0"), fontSize, Center.Y + gapSize.Y * .7f, gapSize, foreground, true);
            }

            if (Size.Y >= gapSize.Y * 4)
            {
                // Script name
                var offset = TextHeight(FontSize * 0.5f);
                //frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(Center.X, TopLeft.Y + offset), new Vector2(Width, FontSize), color: Color.Gray));
                //AddTextBox(frame, "Cruise Control", FontSize * .5f, new Vector2(Center.X, TopLeft.Y + offset), foreground);
                AddBanner(frame, "Ascent Cruise Control", FontSize * .5f, new Vector2(Center.X, TopLeft.Y + offset), foreground, Color.DarkSlateGray);
            }


        }
    }

    void AddKeyValue(MySpriteDrawFrame frame, string key, string val, float fontSize, float height, Vector2 gapSize, Color color, bool box = false)
    {
        var keyText = new StringBuilder(key);
        var keySize = surface.MeasureStringInPixels(keyText, font, fontSize);
        var keySprite = MySprite.CreateText(keyText.ToString(), font, color, fontSize);

        var valText = new StringBuilder(val);
        var valSize = surface.MeasureStringInPixels(valText, font, fontSize);
        var valSprite = MySprite.CreateText(valText.ToString(), font, color, fontSize);

        var totalWidth = keySize.X + valSize.X + gapSize.X;
        var keyPos = Center.X - totalWidth / 2 + keySize.X / 2;
        var valPos = gapSize.X + keyPos + keySize.X / 2 + valSize.X / 2;

        keySprite.Position = new Vector2(keyPos, height - gapSize.Y / 2);
        valSprite.Position = new Vector2(valPos, height - gapSize.Y / 2);

        frame.Add(keySprite);
        frame.Add(valSprite);

        if (box) frame.Add(new MySprite(SpriteType.TEXTURE, "AH_TextBox", new Vector2(valPos, height), valSize * 1.2f, color: color));
    }

    void AddText(MySpriteDrawFrame frame, string str, float fontSize, Vector2 pos, Color color)
    {
        MySprite text = MySprite.CreateText(str, font, color, fontSize);
        var lines = str.Count(f => f == '\n') + 1;
        text.Position = new Vector2(pos.X, pos.Y - (TextHeight(fontSize, lines) * 0.5f));
        frame.Add(text);
    }

    void AddTextBox(MySpriteDrawFrame frame, string str, float fontSize, Vector2 pos, Color color)
    {
        MySprite text = MySprite.CreateText(str, font, color, fontSize);
        text.Position = new Vector2(pos.X, pos.Y - (TextHeight(fontSize) * 0.5f));
        frame.Add(text);
        Vector2 strSize = surface.MeasureStringInPixels(new StringBuilder(str), font, fontSize);
        frame.Add(new MySprite(SpriteType.TEXTURE, "AH_TextBox", pos, strSize * 1.2f, color: color));
    }

    void AddBanner(MySpriteDrawFrame frame, string str, float fontSize, Vector2 pos, Color textColor, Color bgColor)
    {
        MySprite text = MySprite.CreateText(str, font, textColor, fontSize);
        text.Position = new Vector2(pos.X, pos.Y - (TextHeight(fontSize) * 0.5f));
        Vector2 strSize = surface.MeasureStringInPixels(new StringBuilder(str), font, fontSize);
        frame.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", pos, strSize * 1.2f, color: bgColor));
        frame.Add(text);
    }

    float TextHeight(float scale, int lines = 1)
    {
        //Only for Debug font. Edit, seems to work fine with monospace too
        //Got 28.8f from Surface.MeasureStringInPixels(new StringBuilder("Text"), "Debug", 1f).Y;
        //But that didn't look right, even if it techniclay might be.
        //So trial and error using the UVChecker texure and aligning a 0 to it.
        return lines * scale * 30.6f;
    }
}


public class FixedWidthText
{
    private List<string> Text;

    public int Width
    {
        get;
        private set;
    }
    public int Lines
    {
        get {
            return Text.Count;
        }
    }
    public FixedWidthText(int width)
    {
        Text = new List<string>();
        Width = width;
    }
    public void Clear()
    {
        Text.Clear();
    }
    public void Append(string t)
    {
        Text[Text.Count - 1] += t;
    }
    public void AppendLine()
    {
        Text.Add("");
    }
    public void AppendLine(string t)
    {
        Text.Add(t);
    }
    public void Combine(List<string> input)
    {
        Text.AddRange(input);
    }
    public List<string> GetRaw()
    {
        return Text;
    }
    public override string ToString()
    {
        return GetText(Width);
    }
    public string GetText()
    {
        return GetText(Width);
    }
    public string GetText(int lineWidth)
    {
        string finalText = "";
        foreach (var line in Text)
        {
            string rest = line;
            if (rest.Length > lineWidth)
            {
                while (rest.Length > lineWidth)
                {
                    string part = rest.Substring(0, lineWidth);
                    rest = rest.Substring(lineWidth);
                    for (int i = part.Length - 1;
                    i > 0;
                    i--)
                    {
                        if (part[i] == ' ')
                        {
                            finalText += part.Substring(0, i) + "\n";
                            rest = part.Substring(i + 1) + rest;
                            break;
                        }
                    }
                }
            }
            finalText += rest + "\n";
        }
        return finalText;
    }
}


public enum ID
{
    Tag, TargetSpeed, DisableAtGravTransition, SelectThrusters, ThrustEffectivnessCutoff, GroundOffset
};


    public string ScriptName = "Blarg's Ascent Cruise Control";

    TimeSpan Time = TimeSpan.Zero; // Keep track of script running time, like Arduinos millis().
    bool initialized = false;
    TimeSpan nextSetupTry = TimeSpan.MinValue;

    IMyShipController Controller;
    Dictionary<ID, Setting> settings;
    SettingsParser settingsParser;
    CruiseControl cruiseControl;
    Display programmableBlockDisplay;
    List<Display> displays;
    Auxillary auxillary;

    FixedWidthText Problems = new FixedWidthText(40);

    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }

    public void Main(string argument, UpdateType updateType)
    {
        if (Runtime.TimeSinceLastRun.TotalMilliseconds < 1 && updateType != UpdateType.Trigger && updateType != UpdateType.Terminal) return;
        Time += Runtime.TimeSinceLastRun;


        // Check if updateType is either Trigger or Terminal
        if ((updateType & (UpdateType.Trigger | UpdateType.Terminal)) != 0)
        {
            Input(argument, updateType);
        }
        // Check if updateType is the 10 tick interval
        if ((updateType & UpdateType.Update10) != 0)
        {
            LoopLogic();
            Present();
        }
    }

    void LoopLogic()
    {
        if (!initialized)
        {
            if (Time < nextSetupTry) return;
            initialized = Initialize();
            if (initialized)
            {
                Problems.Clear();
            }
            else
            {
                nextSetupTry = Time + TimeSpan.FromSeconds(5);
                return;
            }
        }
        cruiseControl.Loop(Time);
        auxillary.Update(cruiseControl.Active);
    }

    void Present()
    {
        UpdateDetailedInfo();
        if (initialized) {
            programmableBlockDisplay?.Draw(cruiseControl.Active, cruiseControl.Speed, cruiseControl.TargetSpeed, cruiseControl.ThrustOverride, cruiseControl.ThrustDirection);
            displays.ForEach(x => x.Draw(cruiseControl.Active, cruiseControl.Speed, cruiseControl.TargetSpeed, cruiseControl.ThrustOverride, cruiseControl.ThrustDirection));
        } else {
            programmableBlockDisplay?.DrawIssue();
            displays.ForEach(x => x.DrawIssue());
        }
    }

    void Input(string argument, UpdateType updateType)
    {
        if (!initialized) return;

        var parts = argument.ToLower().Split(' ');
        foreach (var part in parts)
        {
            if (ParseInputNumber(argument))
            {
                continue;
            }
            else if (part == "off")
            {
                if (cruiseControl.Active) cruiseControl.Disable();
                continue;
            }
            else if (part == "on")
            {
                if (!cruiseControl.Active) EnableCC();
                continue;
            }
            else if (part == "toggle" || (part == "" && updateType == UpdateType.Trigger))
            {
                if (cruiseControl.Active) cruiseControl.Disable();
                else EnableCC();
                continue;
            }
            else if (part == "swap")
            {
                cruiseControl.SwapThrustDirection();
                settings[ID.SelectThrusters] = settings[ID.SelectThrusters].WithValue(cruiseControl.ThrustDirection);
                settingsParser.Print();
                continue;
            }
            else if (part == "reset") {
                initialized = false;
                nextSetupTry = Time + TimeSpan.FromSeconds(1);
                Storage = "";
            }
            else if (part == "") {
                UpdateSettings();
            }
        }
    }

    void EnableCC()
    {
        if (cruiseControl.Active) return;
        UpdateSettings();
        cruiseControl.Enable();
    }

    void UpdateSettings() {
        settingsParser.Parse(this);
        cruiseControl.SetTargetSpeed((double)settings[ID.TargetSpeed].Value);
        cruiseControl.SetCutoff((double)settings[ID.ThrustEffectivnessCutoff].Value / 100);
        cruiseControl.SetDisableAtGravTransition((bool)settings[ID.DisableAtGravTransition].Value);
        var dir = (ThrustDirection)settings[ID.SelectThrusters].Value;
        if (dir != ThrustDirection.Auto) cruiseControl.SetThrustDirection(dir); // TODO somehow remember which thrusters has the most thrust.
    }

    bool ParseInputNumber(string text)
    {
        double parsed = 0;
        if (double.TryParse(text, out parsed))
        {
            settings[ID.TargetSpeed] = settings[ID.TargetSpeed].WithValue(parsed);
            cruiseControl.SetTargetSpeed(parsed);
            settingsParser.Print();
            return true;
        }
        return false;
    }

    bool Initialize()
    {
        Problems.Clear();
        // Setting this instead of returning immediately allow us to check 
        // several initialization steps before returning, potentially
        // highlighting multiple issues.
        bool abort = false;

        // Controller
        if (Controller == null)
        {
            if (!InitializeController()) abort = true;
        }

        // Settings
        settings = new Dictionary<ID, Setting>
        {
            { ID.Tag, new Setting("Tag", "#ACC") },
            { ID.TargetSpeed, new Setting("Target Speed", (double)95) },
            { ID.DisableAtGravTransition, new Setting("Disable when exiting gravity", true) },
            { ID.SelectThrusters, new Setting("Select thrusters (Auto, Rear, or Bottom)", ThrustDirection.Auto) },
            { ID.ThrustEffectivnessCutoff, new Setting("Thrust effectiveness cutoff (%)", 5.0) },
        };
        settingsParser = new SettingsParser(settings, Me, ScriptName);
        if (!settingsParser.Parse(this)) abort = true;

        // Displays, lights, and timers
        if (programmableBlockDisplay == null) programmableBlockDisplay = new Display(Me.GetSurface(0), this);
        displays = new List<Display>();
        auxillary = new Auxillary();
        var blocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(blocks);
        blocks.ForEach(block => ProcessBlocks(block));


        if (abort) return false;

        // Cruise control
        if (!InitializeCruiseControl())
        {
            return false;
        }
        return true;
    }

    void ProcessBlocks(IMyTerminalBlock block) {
        if (block is IMyTextSurfaceProvider) ProcessDisplay(block);
        if (block is IMyLightingBlock) ProcessLights(block as IMyLightingBlock);
        if (block is IMyTimerBlock) ProcessTimers(block as IMyTimerBlock);
    }

    void ProcessLights(IMyLightingBlock block) {
        string tag = (string)settings[ID.Tag].Value;
        if (!block.CustomName.Contains(tag)) return;

        auxillary.AddLight(block);
        
    }

    void ProcessTimers(IMyTimerBlock block) {
        string tag = (string)settings[ID.Tag].Value;
        if (!block.CustomName.Contains(tag)) return;

        auxillary.AddTimer(block);
    }

    void ProcessDisplay(IMyTerminalBlock block)
    {
        if (block as IMyTextSurfaceProvider == null) return;
        string tag = (string)settings[ID.Tag].Value;

        if (!block.CustomName.Contains(tag)) return;

        int screennr = 0;
        var parts = block.CustomName.Split('@');
        if (parts.Length > 1)
        {
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].EndsWith(tag) && parts.Length > i + 1)
                {
                    int.TryParse(new string(parts[i + 1].TakeWhile(char.IsDigit).ToArray()), out screennr);
                }
            }
        }
        IMyTextSurface surface = (block as IMyTextSurfaceProvider).GetSurface(screennr);
        if (surface == null) return; // TODO Spotlights are surface providers bu has no surfaces... This prevents errors with that.
        displays.Add(new Display(surface, this));
    }

    bool InitializeController()
    {
        var controllers = new List<IMyShipController>();
        long id = 0;
        if (long.TryParse(Storage, out id))
        {
            GridTerminalSystem.GetBlocksOfType(controllers, x => x.EntityId == id);
            if (controllers.Count > 0)
            {
                Controller = controllers[0];
                return true;
            }
        }
        GridTerminalSystem.GetBlocksOfType(controllers, x => ControllerMeetsConditions(x));
        if (controllers.Count > 0)
        {
            Controller = controllers[0];
            Storage = controllers[0].EntityId.ToString();
            return true;
        }
        Problems.AppendLine("Can't find a control seat/cockpit. Sit in one for up to 10 seconds (to determine what's forward).");
        return false;
    }

    bool InitializeCruiseControl()
    {
        // Gather thrusters
        var shipOrientation = Controller.Orientation;
        float rearThrust = 0;
        float bottomThrust = 0;
        var thrusters = new List<IMyThrust>();
        var backwards = new List<IMyThrust>();
        var forwards = new List<IMyThrust>();
        var upwards = new List<IMyThrust>();
        var downwards = new List<IMyThrust>();
        GridTerminalSystem.GetBlocksOfType(thrusters, x => x.CubeGrid == Me.CubeGrid);
        foreach (var item in thrusters)
        {
            if (item.Orientation.Forward == shipOrientation.Forward)
            {
                backwards.Add(item);
            }
            else if (item.Orientation.Forward == Base6Directions.GetOppositeDirection(shipOrientation.Forward))
            {
                forwards.Add(item);
                rearThrust += item.MaxThrust;
            }
            if (item.Orientation.Forward == shipOrientation.Up)
            {
                upwards.Add(item);
            }
            else if (item.Orientation.Forward == Base6Directions.GetOppositeDirection(shipOrientation.Up))
            {
                downwards.Add(item);
                bottomThrust += item.MaxThrust;
            }
        }

        // Detect auto thrust direction
        var thrustDirection = (ThrustDirection)settings[ID.SelectThrusters].Value;
        if (thrustDirection == ThrustDirection.Auto)
        {
            if (rearThrust >= bottomThrust) thrustDirection = ThrustDirection.Rear;
            else thrustDirection = ThrustDirection.Bottom;
        }

        // Check that we have thrusters
        if (thrustDirection == ThrustDirection.Rear && forwards.Count == 0)
        {
            Problems.AppendLine("No thrusters found at the rear.");
            return false;
        }
        else if (downwards.Count == 0)
        {
            Problems.AppendLine("No thrusters found at the bottom.");
            return false;
        }

        // Setup cruise control
        // Thruster data structure
        var thrustDict = new Dictionary<ThrustDirection, CruiseControl.ThrusterSet>
        {
            {
                ThrustDirection.Rear,
                new CruiseControl.ThrusterSet
                {
                    Main = forwards,
                    Reverse = backwards
                }
            },
            {
                ThrustDirection.Bottom,
                new CruiseControl.ThrusterSet
                {
                    Main = downwards,
                    Reverse = upwards
                }
            }
        };
        // Cruise control properties
        var properties = new CruiseControl.Properties
        {
            TargetSpeed = (double)settings[ID.TargetSpeed].Value,
            ThrustDirection = thrustDirection,
            Cutoff = (double)settings[ID.ThrustEffectivnessCutoff].Value / 100,
            DisableAtGravTransition = (bool)settings[ID.DisableAtGravTransition].Value,
            GroundOffset = 0,
        };
        // Initialize class
        cruiseControl = new CruiseControl(thrustDict, Controller, properties);
        return true;
    }

    bool ControllerMeetsConditions(IMyShipController controller)
    {
        if (controller is IMyCryoChamber) return false;
        if (controller.CubeGrid != Me.CubeGrid) return false;
        if (!controller.IsUnderControl) return false;
        return true;
    }

    void UpdateDetailedInfo()
    {
        var info = new FixedWidthText(70);
        info.Clear();
        info.AppendLine(ScriptName);
        info.AppendLine();
        if (settingsParser.Problem)
        {
            info.Combine(settingsParser.Problems.GetRaw());
            info.AppendLine();
        }
        if (Problems.Lines > 0)
        {
            info.AppendLine("Problems:");
            info.Combine(Problems.GetRaw());
            info.AppendLine();
        }
        if (cruiseControl != null && cruiseControl.Active)
        {
            info.AppendLine("Status: Engaged");
            info.AppendLine($"Target speed: {Math.Round(cruiseControl.TargetSpeed, 1)}m/s");
            info.AppendLine($"Current speed: {Math.Round(cruiseControl.Speed, 1).ToString("n1")}m/s");
            info.AppendLine($"Thrust override: {(cruiseControl.ThrustOverride * 100).ToString("n1")}%");
            info.AppendLine();
            info.AppendLine("Press toolbar button again to disengage.");
            info.AppendLine();
            if ((bool)settings[ID.DisableAtGravTransition].Value) info.AppendLine("Will automatically disengage.");
            else info.AppendLine("Manual disengage only.");
            info.AppendLine($"Using {cruiseControl.ThrustDirection.ToString()} thrusters."); // TODO
        }
        else if (!initialized)
        {
            info.AppendLine("Status: NOT Ready");
        }
        else
        {
            info.AppendLine("Status: ready");
            info.AppendLine($"Target speed: {Math.Round(cruiseControl.TargetSpeed, 1)}m/s"); // Uncomment when cc is implemented
            info.AppendLine();
            info.AppendLine("Click 'Custom Data' for settings.");
            info.AppendLine();
            if ((bool)settings[ID.DisableAtGravTransition].Value) info.AppendLine("Will automatically disengage.");
            else info.AppendLine("Manual disengage only.");
            info.AppendLine($"Using {cruiseControl.ThrustDirection.ToString()} thrusters.");
            info.AppendLine($"Connected LCDs: {displays.Count}");
            info.AppendLine($"Connected Lights: {auxillary.CountLights()}");
            info.AppendLine($"Connected Timers: {auxillary.CountTimers()}");
        }
        Echo(info.ToString());
    }


public struct Setting
{
    public string Text;
    public object Value;
    public int SpaceAbove;

    public Setting(string text, object value, int spaceAbove = 0)
    {
        Text = text;
        Value = value;
        SpaceAbove = spaceAbove;
    }

    public Setting WithValue(object newValue)
    {
        return new Setting(Text, newValue, SpaceAbove);
    }
}


public class SettingsParser
{

    Dictionary<ID, Setting> settings;
    IMyTerminalBlock block;
    string scriptName;

    public bool Problem { get; private set; } = false;
    public FixedWidthText Problems { get; private set; } = new FixedWidthText(40);

    public SettingsParser(Dictionary<ID, Setting> settings, IMyTerminalBlock block, string scriptName)
    {
        this.settings = settings;
        this.block = block;
        this.scriptName = scriptName;
    }

    public bool Parse(Program program)
    {
        Problems.Clear();
        Problem = false;
        Problems.AppendLine("Problem with settings:");

        string text = block.CustomData;
        var lines = text.Split('\n');
        bool inSettings = true;
        for (int i = 0; i < lines.Length; i++)
        {
            if (!inSettings)
            {
                if (lines[i].Contains(scriptName + " Settings")) inSettings = true;
                continue;
            }
            if (lines[i].StartsWith(" \t "))
            {
                // Invisible way of ending the settings section
                inSettings = false;
                continue;
            }
            var keys = new List<ID>(settings.Keys);
            foreach (var key in keys)
            {
                ParseKey(key, lines[i], program);
            }
        }

        Problems.AppendLine("______________________________");
        Problems.AppendLine();

        Print();
        return !Problem;
    }

    public void Print()
    {
        var CustomData = new FixedWidthText(70);
        CustomData.AppendLine(scriptName + " Settings");
        CustomData.AppendLine("----------------------------------------------------------------------");
        CustomData.AppendLine("To change settings: Edit the value after the colon, then press run with no argument. Or engage cruise control.");
        CustomData.AppendLine("----------------------------------------------------------------------");
        CustomData.AppendLine();
        foreach (var setting in settings.Values)
        {
            for (int i = 0; i < setting.SpaceAbove; i++)
            {
                CustomData.AppendLine();
            }
            CustomData.AppendLine(setting.Text + ": " + SettingToString(setting.Value));
        }
        CustomData.AppendLine(" \t ");
        CustomData.AppendLine();
        CustomData.AppendLine();
        CustomData.AppendLine();
        CustomData.AppendLine("More info:");
        CustomData.AppendLine("----------------------------------------------------------------------");
        CustomData.AppendLine("The script can show status using lights and LCDs.");
        CustomData.AppendLine("To set them up, add the tag above to the names of the LCDs/lights.");
        CustomData.AppendLine("Supports both regular and corner LCDs.");
        CustomData.AppendLine("After adding tags, press run with no argument to update");
        CustomData.AppendLine();
        CustomData.AppendLine("----------------------------------------------------------------------");
        CustomData.AppendLine("If the script has gotten what's forward wrong:");
        CustomData.AppendLine("Sit in a cockpit/flight seat facing the desired forward direction, make sure only one seat is in use, and send the reset command.");
        CustomData.AppendLine("(The reset command is sent by typing 'reset' as the argument and pressing run).");
        CustomData.AppendLine();
        CustomData.AppendLine("----------------------------------------------------------------------");
        CustomData.AppendLine("Arguments you can run from the toolbar/button/sensor/etc..");
        CustomData.AppendLine("(All of which can be used even while cruise control is engaged).");
        CustomData.AppendLine("(Minus the brackets).");
        CustomData.AppendLine("  [] (empty) Toggles cruise control on or off.");
        CustomData.AppendLine("  [on] Toggles cruise control on.");
        CustomData.AppendLine("  [off] Toggles cruise control off.");
        CustomData.AppendLine("  [95] (any number, including negative) Sets a new target speed.");
        CustomData.AppendLine("  [swap] Switches between rear and bottom thrusters.");
        block.CustomData = CustomData.GetText();
    }

    private string SettingToString(object input)
    {
        if (input is bool)
        {
            return (bool)input ? "yes" : "no";
        }
        if (input is Color)
        {
            var color = (Color)input;
            return "R:" + color.R + ", G:" + color.G + ", B:" + color.B;
        }
        return input.ToString();
    }

    private void ParseKey(ID key, string line, Program program)
    {
        if (!line.StartsWith(settings[key].Text)) return;
        var parts = line.Split(new char[] { ':' }, 2);
        string value = (parts[1] ?? "").Trim();
        if (string.IsNullOrEmpty(value))
        {
            SettingsProblemIllegible(key);
            return;
        }
        if (settings[key].Value is bool)
        {
            value = value.ToLower();
            if (value == "yes") settings[key] = settings[key].WithValue(true);
            else if (value == "no") settings[key] = settings[key].WithValue(false);
            else {
                SettingsProblemIllegible(key, "Has to be yes or no");
            }
            return;
        }
        if (settings[key].Value is int)
        {
            int parsed = 0;
            if (int.TryParse(value, out parsed)) settings[key] = settings[key].WithValue(parsed);
            else {
                SettingsProblemIllegible(key, "Has to be a whole number");
            }
            return;
        }
        if (settings[key].Value is double)
        {
            double parsed = 0;
            if (double.TryParse(value, out parsed)) settings[key] = settings[key].WithValue(parsed);
            else {
                SettingsProblemIllegible(key, "Has to be a number");
            }
            return;
        }
        if (settings[key].Value is string)
        {
            if (!string.IsNullOrWhiteSpace(value)) settings[key] = settings[key].WithValue(value);
            else {
                SettingsProblemIllegible(key, "Has to be text");
            }
            return;
        }
        if (settings[key].Value is ThrustDirection)
        {
            ThrustDirection td;
            if (Enum.TryParse<ThrustDirection>(value, true, out td)) settings[key] = settings[key].WithValue(td);
            else
            {
                
                string directions = string.Join(", ", Enum.GetNames(typeof(ThrustDirection)));
                SettingsProblemIllegible(key, $"Must be one of these: {directions}.");
            }
        }
    }

    private void SettingsProblemIllegible(ID key, string additionalInfo = "")
    {
        Problem = true;
        Problems.AppendLine();
        Problems.AppendLine("Did not understand setting. Using default or previous value.");
        Problems.AppendLine("> " + settings[key].Text);
        if (additionalInfo != "") Problems.AppendLine("> " + additionalInfo);
    }
}


public enum ThrustDirection
{
    Auto, Rear, Bottom
};

