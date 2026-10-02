/*////////////////////////////////////////////////////////////////////////////////////////////////
NAME: TXC Automatic Solar Panels
DEPENDENCIES: none
AUTHOR: Xarniia
DATE: 2019/03/28 19:10
DESCRIPTION: No setup, no naming, just maximizing solar power by aligning solar panels.
    Arranges different predefined layout of solar panels without any setup needed.

    * Use 'recompile' button or 'init' as argument to scan the grid.
    * Arguments:    'start'    : starts the aligning again (e.g. after stop)
                    'stop'     : stops the aligning
                    'break X Y': toggles aligning of panels for X seconds after a break of Y seconds (to be used as argument of the timer block)

COPYRIGHT: This is free software released into the public domain.
            If you use or change it, please give a link back to this mod or name me as the author.
            Also, if you write new classes, feel free to include them in the original.
////////////////////////////////////////////////////////////////////////////////////////////////*/

//////////////////////////////// S E T T I N G S - feel free to edit ////////////////////////////////

// Blocks (rotors and solar panels) containing this string will not been handled.
private const string EXCLUDESTRING = "[TXC SolarExclude]";

// Cooldowns for a rotor or panel reset. For low simspeed this may need adjustment (bug in api).
private const double MILLISECONDSREVERSECOOLDOWN = 5000.0, GLOBALPANELRESETTIME = 30000.0;

// MINRELATIVEOUTPUT: Percentage of power panels should achieve.
// MINRELATIVEOUTPUTWITHLIMIT: Percentage of power when panels with limit should change the
// orientation. In space this can above zero.
// ROTORVELOCITY: Velocity of the rotors. If you increase this value you should reduce the ‘MILLISECONDSREVERSECOOLDOWN’.
// ROTORTORQUE: Torque and brake torque rotors use.
private const float MINRELATIVEOUTPUT = .99f, RELATIVEOUTPUTFORLIMITCHANGE = 0.0f, ROTORVELOCITY = 0.01f, ROTORTORQUE = 33600000.0f;

//////////////////////////////// C O D E - don't change! ////////////////////////////////

#region Code - don't change!

/// <summary>
/// Reverse action string for rotors
/// </summary>
private const string ACTION_ON = "OnOff_On", ACTION_REVERSE = "Reverse";

/// <summary>
/// Possible maximum output of solar panel blocks in MW.
/// </summary>
private const float SOLARMAXOUTPUTLARGE = 0.16f, SOLARMAXOUTPUTSMALL = 0.04f;

/// <summary>
/// List for holding arguments.
/// </summary>
private List<string> arguments = new List<string>();

private double BreakToggleTime = -1.0;

/// <summary>
/// global variables for arguments
/// </summary>
private bool IsStopped = false, IsBreakMode = false, IsBreak = false;

/// <summary>
/// Global list which holds all arrangeable panels.
/// </summary>
private List<TXC_AbstractPanel> List_Panels = new List<TXC_AbstractPanel>();

/// <summary>
/// Constructor
/// </summary>
public Program()
{
    // Configure this program to run the Main method every 100 update ticks
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

/// <summary>
/// Inits the program by scaning the grid for all panels of predefined shapes.
/// </summary>
private void Init()
{
    List_Panels.Clear();
    List<IMyMotorStator> rotors = new List<IMyMotorStator>();
    GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors, x => x.CubeGrid == Me.CubeGrid && !x.CustomName.Contains(EXCLUDESTRING) && x.TopGrid != null);

    foreach (IMyMotorStator r in rotors)
    {
        TXC_AbstractPanel p = TXC_IPanel.TryCreatePanel(GridTerminalSystem, r)
            ?? TXC_OPanel.TryCreatePanel(GridTerminalSystem, r)
            ?? TXC_UPanel.TryCreatePanel(GridTerminalSystem, r)
            ?? TXC_TPanel.TryCreatePanel(GridTerminalSystem, r);
        if (p == null || List_Panels.Contains(p))
            continue;
        List_Panels.Add(p);
    }
}

/// <summary>
/// Main method.
/// </summary>
/// <param name="argument">Parameters passed from outside</param>
private void Main(string argument, UpdateType updateType)
{
    string output = string.Empty;

    #region commands

    // only if trggered directly or from button
    if ((updateType & (UpdateType.Trigger | UpdateType.Terminal)) != 0)
    {
        arguments = argument.Split(' ').ToList();

        // init or init argument
        int index = arguments.IndexOf("init");
        if (List_Panels.Count == 0 || index != -1)
        {
            ResetGlobals();
            Init();
            Echo(List_Panels.Count + " Panels created.");
            return;
        }

        // start argument
        index = arguments.IndexOf("start");
        if (index != -1)
        {
            ResetGlobals();
            output += "!!! Panels started !!!\n\n";
        }

        // stop argument
        index = arguments.IndexOf("stop");
        if (index != -1 || IsStopped)
        {
            ResetGlobals();
            IsStopped = true;
            output += "!!! Panels stopped !!!\n\n";
            foreach (TXC_AbstractPanel p in List_Panels)
            {
                p.Stop(ref output);
            }
            Echo(output);
            return;
        }

        // break argument
        index = arguments.IndexOf("break");
        if (index != -1 || IsBreakMode)
        {
            IsStopped = false;
            IsBreakMode = true;
            int run, pause;
            if (arguments.Count < index + 3 || !int.TryParse(arguments[index + 1], out run) || !int.TryParse(arguments[index + 2], out pause))
            {
                Echo("!!! Break argument not valid !!!\n\n     Stopping panels...");
                List_Panels.ForEach(x => x.Stop(ref output));
                return;
            }

            // timer setzen
            if (BreakToggleTime < 0.0)
            {
                IsBreak = !IsBreak;
                BreakToggleTime = (IsBreak ? pause : run) * 1000;
            }
            else
            {
                BreakToggleTime -= Runtime.TimeSinceLastRun.TotalMilliseconds;
            }

            if (IsBreak)
            {
                output += "!!! Panels paused !!!\n   " + (BreakToggleTime / 1000.0).ToString("F1") + " s left \n\n";
                foreach (TXC_AbstractPanel p in List_Panels)
                {
                    p.Stop(ref output);
                }
                Echo(output);
                return;
            }
            output += "!!! Panels arrange !!!\n   " + (BreakToggleTime / 1000.0).ToString("F1") + " s left \n\n";
        }
    }

    #endregion commands

    // else just arrage
    foreach (TXC_AbstractPanel p in List_Panels)
    {
        p.Arrange(ref output, Runtime.TimeSinceLastRun.TotalMilliseconds);
    }
    Echo(output);
}

/// <summary>
/// Resets global variables
/// </summary>
private void ResetGlobals()
{
    IsStopped = false;
    IsBreakMode = false;
    IsBreak = false;
    BreakToggleTime = -1.0;
}

/// <summary>
/// Basic abstract panel class.
/// </summary>
public abstract class TXC_AbstractPanel
{
    // Power W L/S
    protected static readonly float[] SOLARPANELOUTPUT = new float[] { SOLARMAXOUTPUTLARGE, SOLARMAXOUTPUTSMALL };

    protected readonly IMyMotorStator BaseRotor;
    protected readonly List<IMySolarPanel> SolarPanels;
    protected int BaseRotorTargetVelocityRadSign = 1;
    protected bool IsLimited = false, ReAligningToLimit = false;
    protected float LastRelativeMaxPower = 0.0f;
    protected double PanelGlobalReset = 0.0, BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;

    protected TXC_AbstractPanel(IMyMotorStator rotor, List<IMySolarPanel> solars)
    {
        BaseRotor = rotor;
        BaseRotor.TargetVelocityRad = 0.0f;
        BaseRotor.Torque = ROTORTORQUE;
        BaseRotor.BrakingTorque = ROTORTORQUE;
        BaseRotor.ApplyAction(ACTION_ON);
        IsLimited = !float.IsInfinity(BaseRotor.LowerLimitDeg) || !float.IsInfinity(BaseRotor.UpperLimitDeg);
        SolarPanels = solars;
        LastRelativeMaxPower = GetCurrentRelativMaxOutput();
    }

    public abstract void Arrange(ref string s, double millisecondsSinceLastRun);

    public abstract void Stop(ref string s);

    protected float GetCurrentRelativMaxOutput(List<IMySolarPanel> panels = null)
    {
        if ((panels ?? SolarPanels) == null)
            return 0.0f;
        float res = 0.0f;
        foreach (IMySolarPanel p in (panels ?? SolarPanels))
        {
            res = Math.Max(res, p.MaxOutput / SOLARPANELOUTPUT[(int)p.CubeGrid.GridSizeEnum]);
        }
        return res;
    }
}

/// <summary>
/// I Panel
/// </summary>
public class TXC_IPanel : TXC_AbstractPanel
{
    protected TXC_IPanel(IMyMotorStator rotor, List<IMySolarPanel> solars)
        : base(rotor, solars)
    {
    }

    public static TXC_AbstractPanel TryCreatePanel(IMyGridTerminalSystem GridTerminalSystem, IMyMotorStator rotor)
    {
        // no rotors on grid or pointing to this grid & solars found => i-Panel
        List<IMyMotorStator> rotors = new List<IMyMotorStator>();
        GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors, x => (x.CubeGrid == rotor.TopGrid || (x.CubeGrid == rotor.CubeGrid && x.TopGrid == rotor.TopGrid && x != rotor)) && !x.CustomName.Contains(EXCLUDESTRING));
        if (rotors.Count > 0)
            return null;

        List<IMySolarPanel> solars = new List<IMySolarPanel>();
        GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solars, x => x.CubeGrid == rotor.TopGrid && !x.CustomName.Contains(EXCLUDESTRING));
        if (solars.Count == 0)
            return null;

        return new TXC_IPanel(rotor, solars);
    }

    public override void Arrange(ref string output, double millisecondsSinceLastRun)
    {
        float current_p = GetCurrentRelativMaxOutput();
        if (BaseRotorReverseCooldown >= 0.0)
            BaseRotorReverseCooldown -= millisecondsSinceLastRun;
        output += (IsLimited ? "Limited " : string.Empty) + "IPanel " + BaseRotor.CustomName + " (" + BaseRotorReverseCooldown.ToString("F0") + ")\n  P: " + current_p.ToString("F3") + (current_p - LastRelativeMaxPower < 0.0 ? " - " : " + ") + Math.Abs(current_p - LastRelativeMaxPower).ToString("F3") + "\n\n";

        // Limit-Reset: align to farer limit
        if (ReAligningToLimit || IsLimited && current_p < RELATIVEOUTPUTFORLIMITCHANGE)
        {
            // start realign
            if (!ReAligningToLimit)
            {
                // the direction the panel later should go
                BaseRotorTargetVelocityRadSign = Math.Sign(2.0f * BaseRotor.Angle - BaseRotor.UpperLimitDeg + BaseRotor.UpperLimitDeg);
                BaseRotor.TargetVelocityRad = -BaseRotorTargetVelocityRadSign * ROTORVELOCITY;
                ReAligningToLimit = true;
            }
            else
            {
                // close is enought -> stopp
                if (BaseRotor.Angle - BaseRotor.LowerLimitDeg < 0.1 || BaseRotor.UpperLimitDeg - BaseRotor.Angle < 0.1)
                {
                    BaseRotor.TargetVelocityRad = 0.0f;
                }

                // make panel alignable again when treshold is reached.
                if (current_p > RELATIVEOUTPUTFORLIMITCHANGE)
                {
                    ReAligningToLimit = false;
                }
            }
            LastRelativeMaxPower = current_p;
            return;
        }

        // at least one panel must match the threshold (e.g. shadow) or rotor locked
        if (current_p > MINRELATIVEOUTPUT || LastRelativeMaxPower > MINRELATIVEOUTPUT || current_p == 0.0f)
        {
            // stop rotor
            if (BaseRotor.TargetVelocityRad != 0.0f)
            {
                BaseRotorTargetVelocityRadSign = Math.Sign(BaseRotor.TargetVelocityRad);
                BaseRotor.TargetVelocityRad = 0.0f;
            }
            LastRelativeMaxPower = current_p;
            return;
        }

        // Arrange start
        if (BaseRotor.TargetVelocityRad == 0.0f)
        {
            BaseRotor.TargetVelocityRad = BaseRotorTargetVelocityRadSign * ROTORVELOCITY;
            BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
        }
        // reverse
        else if (current_p < LastRelativeMaxPower && BaseRotorReverseCooldown <= 0.0f)
        {
            BaseRotor.ApplyAction(ACTION_REVERSE);
            BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
        }
        LastRelativeMaxPower = current_p;
    }

    public override void Stop(ref string output)
    {
        BaseRotor.TargetVelocityRad = 0.0f;
        output += (IsLimited ? "Limited " : string.Empty) + "IPanel " + BaseRotor.CustomName + " stopped.\n\n";
    }
}

/// <summary>
/// O-Panel
/// </summary>
public class TXC_OPanel : TXC_AbstractPanel
{
    protected readonly IMyMotorStator RollRotor;

    protected TXC_OPanel(IMyMotorStator rotor, IMyMotorStator rollrotor, List<IMySolarPanel> solars)
        : base(rotor, solars)
    {
        RollRotor = rollrotor;
        RollRotor.TargetVelocityRad = 0.0f;
        RollRotor.Torque = 0.0f;
        RollRotor.BrakingTorque = 0.0f;
        RollRotor.ApplyAction(ACTION_ON);

        // rotor already checked
        IsLimited |= !float.IsInfinity(rollrotor.LowerLimitDeg) || !float.IsInfinity(rollrotor.UpperLimitDeg);
    }

    public static bool operator !=(TXC_OPanel P1, TXC_OPanel P2)
    {
        return !(P1 == P2);
    }

    public static bool operator ==(TXC_OPanel P1, TXC_OPanel P2)
    {
        if (P1.SolarPanels[0].CubeGrid == P2.SolarPanels[0].CubeGrid &&
            ((P1.BaseRotor == P2.BaseRotor && P1.RollRotor == P2.RollRotor) ||
            (P1.BaseRotor == P2.RollRotor && P1.RollRotor == P2.BaseRotor)))
            return true;

        return false;
    }

    public static TXC_AbstractPanel TryCreatePanel(IMyGridTerminalSystem GridTerminalSystem, IMyMotorStator rotor)
    {
        // no rotor on attached grid & solars found => o-Panel
        List<IMyMotorStator> rotors = new List<IMyMotorStator>();
        GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors, x => x.CubeGrid == rotor.TopGrid && !x.CustomName.Contains(EXCLUDESTRING));
        if (rotors.Count > 0)
            return null;

        // two rotors pointing to solar panels
        GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors, x => x.CubeGrid == rotor.CubeGrid && x.TopGrid == rotor.TopGrid && x != rotor && !x.CustomName.Contains(EXCLUDESTRING));
        if (rotors.Count != 1)
            return null;

        List<IMySolarPanel> solars = new List<IMySolarPanel>();
        GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solars, x => x.CubeGrid == rotor.TopGrid && !x.CustomName.Contains(EXCLUDESTRING));
        if (solars.Count == 0)
            return null;

        return new TXC_OPanel(rotor, rotors[0], solars);
    }

    public override void Arrange(ref string output, double millisecondsSinceLastRun)
    {
        float current_p = GetCurrentRelativMaxOutput();
        if (BaseRotorReverseCooldown >= 0.0)
            BaseRotorReverseCooldown -= millisecondsSinceLastRun;
        output += (IsLimited ? "Limited " : string.Empty) + "OPanel " + BaseRotor.CustomName + " (" + BaseRotorReverseCooldown.ToString("F0") + ")\n  P: " + current_p.ToString("F3") + (current_p - LastRelativeMaxPower < 0.0 ? " - " : " + ") + Math.Abs(current_p - LastRelativeMaxPower).ToString("F3") + "\n\n";
        // at least one panel must match the threshold (e.g. shadow) or rotor locked
        if (current_p > MINRELATIVEOUTPUT || LastRelativeMaxPower > MINRELATIVEOUTPUT || current_p == 0.0)
        {
            // stop rotor
            if (BaseRotor.TargetVelocityRad != 0.0f)
            {
                BaseRotorTargetVelocityRadSign = Math.Sign(BaseRotor.TargetVelocityRad);
                BaseRotor.TargetVelocityRad = 0.0f;
            }
            LastRelativeMaxPower = current_p;
            return;
        }

        // Arrange start
        if (BaseRotor.TargetVelocityRad == 0.0f)
        {
            // take care on the roll rotor
            RollRotor.TargetVelocityRad = 0.0f;
            RollRotor.Torque = 0.0f;
            RollRotor.BrakingTorque = 0.0f;

            // also the base rotor may need new initation because of the construction of
            // equal O-panels
            BaseRotor.Torque = ROTORTORQUE;
            BaseRotor.BrakingTorque = ROTORTORQUE;
            BaseRotor.TargetVelocityRad = BaseRotorTargetVelocityRadSign * ROTORVELOCITY;
            BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
        }
        // reverse
        else if (current_p < LastRelativeMaxPower && BaseRotorReverseCooldown <= 0.0f)
        {
            BaseRotor.ApplyAction(ACTION_REVERSE);
            BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
        }
        LastRelativeMaxPower = current_p;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TXC_OPanel))
            return false;

        return this == obj as TXC_OPanel;
    }

    public override int GetHashCode()
    {
        return this.GetHashCode();
    }

    public override void Stop(ref string output)
    {
        BaseRotor.TargetVelocityRad = 0.0f;
        RollRotor.TargetVelocityRad = 0.0f;
        output += (IsLimited ? "Limited " : string.Empty) + "OPanel " + BaseRotor.CustomName + " stopped.\n\n";
    }
}

/// <summary>
/// T Panel
/// </summary>
public class TXC_TPanel : TXC_AbstractPanel
{
    protected readonly List<TPanelChild> TPanelChilds;
    protected bool BaseRotorReversed = false;

    protected TXC_TPanel(IMyMotorStator baserotor, List<TPanelChild> childs)
        : base(baserotor, null)
    {
        TPanelChilds = childs;
        TPanelChilds.ForEach(delegate (TPanelChild x)
        {
            x.PanelRotor.TargetVelocityRad = 0.0f;
            x.PanelRotor.Torque = ROTORTORQUE;
            x.PanelRotor.BrakingTorque = ROTORTORQUE;
            x.PanelRotor.ApplyAction(ACTION_ON);
            x.LastRelativeMaxPower = GetCurrentRelativMaxOutput(x.SolarPanels);
        });
    }

    public static TXC_AbstractPanel TryCreatePanel(IMyGridTerminalSystem GridTerminalSystem, IMyMotorStator rotor)
    {
        // min 1 rotor with different TopGrids
        List<IMyMotorStator> rotors = new List<IMyMotorStator>();
        GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors, x => x.CubeGrid == rotor.TopGrid && !x.CustomName.Contains(EXCLUDESTRING) && x.TopGrid != null);
        if (rotors.Count == 0 || rotors.Exists(x => rotors.Exists(y => y != x && y.TopGrid == x.TopGrid)))
            return null;

        List<TPanelChild> childs = new List<TPanelChild>();

        // For each rotor
        foreach (IMyMotorStator r in rotors)
        {
            //solars found there => T - Panel
            List<IMySolarPanel> solars = new List<IMySolarPanel>();
            GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solars, x => x.CubeGrid == r.TopGrid && !x.CustomName.Contains(EXCLUDESTRING));

            if (solars.Count != 0)
                childs.Add(new TPanelChild(r, solars));
        }

        if (childs.Count == 0)
            return null;

        return new TXC_TPanel(rotor, childs);
    }

    public override void Arrange(ref string output, double millisecondsSinceLastRun)
    {
        if (PanelGlobalReset > 0.0)
            PanelGlobalReset = PanelGlobalReset - millisecondsSinceLastRun == 0.0 ? -1.0 : PanelGlobalReset - millisecondsSinceLastRun;
        if (BaseRotorReverseCooldown >= 0.0)
            BaseRotorReverseCooldown -= millisecondsSinceLastRun;
        output += (IsLimited ? "Limited " : string.Empty) + "TPanel " + BaseRotor.CustomName + " (" + PanelGlobalReset.ToString("F0") + " - " + BaseRotorReversed + ")\n";
        float deltaP;
        for (int i = 0; i != TPanelChilds.Count; i++)
        {
            if (TPanelChilds[i].PanelRotorReverseCooldown >= 0.0)
                TPanelChilds[i].PanelRotorReverseCooldown -= millisecondsSinceLastRun;
            TPanelChilds[i].CurrentPowerVariable = GetCurrentRelativMaxOutput(TPanelChilds[i].SolarPanels);
            deltaP = TPanelChilds[i].CurrentPowerVariable - TPanelChilds[i].LastRelativeMaxPower;

            output += "  C" + i + " (" + TPanelChilds[i].PanelRotor.CustomName + "): " + TPanelChilds[i].CurrentPowerVariable.ToString("F3") + (deltaP < 0.0 ? " - " : " + ") + Math.Abs(deltaP).ToString("F3") + "(" + TPanelChilds[i].PanelRotorReversed + ")\n";
        }
        output += "\n";

        // all panels must match the threshold (e.g. shadow) or rotor locked
        if (!TPanelChilds.Exists(x => x.CurrentPowerVariable < MINRELATIVEOUTPUT || x.LastRelativeMaxPower < MINRELATIVEOUTPUT || x.CurrentPowerVariable != 0.0f) || PanelGlobalReset < 0.0)
        {
            // stop rotors
            if (BaseRotor.TargetVelocityRad != 0.0f)
            {
                BaseRotorTargetVelocityRadSign = Math.Sign(BaseRotor.TargetVelocityRad);
                BaseRotor.TargetVelocityRad = 0.0f;
            }
            TPanelChilds.ForEach(delegate (TPanelChild x)
            {
                if (x.PanelRotor.TargetVelocityRad != 0.0f)
                {
                    x.PanelRotorTargetVelocityRadSign = Math.Sign(x.PanelRotor.TargetVelocityRad);
                    x.PanelRotor.TargetVelocityRad = 0.0f;
                }
                x.PanelRotorReversed = false;
                x.PanelRotorClose = false;
                x.LastRelativeMaxPower = x.CurrentPowerVariable;
            });
            BaseRotorReversed = false;
            PanelGlobalReset = 0.0;
            return;
        }

        // Arrange start from default with all PanelRotor
        if (BaseRotor.TargetVelocityRad == 0.0f && !BaseRotorReversed && !TPanelChilds.Exists(x => x.PanelRotor.TargetVelocityRad != 0.0f || x.PanelRotorReversed))
        {
            TPanelChilds.ForEach(delegate (TPanelChild x)
            {
                x.PanelRotor.TargetVelocityRad = BaseRotorTargetVelocityRadSign * ROTORVELOCITY;
                x.PanelRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
            });
        }
        // reverse or base alignement
        else
        {
            TPanelChilds.ForEach(delegate (TPanelChild x)
            {
                // reverse i-rotor
                if (x.CurrentPowerVariable < x.LastRelativeMaxPower && x.PanelRotorReverseCooldown <= 0.0f || x.CurrentPowerVariable == 0.0 && x.LastRelativeMaxPower == 0.0)
                {
                    // reverse the panel rotor only once
                    if (!x.PanelRotorReversed)
                    {
                        x.PanelRotor.ApplyAction(ACTION_REVERSE);
                        // if already close, no need for long cooldown
                        x.PanelRotorReverseCooldown = x.PanelRotorClose ? 0.2 * MILLISECONDSREVERSECOOLDOWN : MILLISECONDSREVERSECOOLDOWN;
                        x.PanelRotorReversed = true;
                    }
                    // stop panel rotor
                    else
                    {
                        // the rotor should follow the sun until all panels have found their maximum
                        x.PanelRotorClose = true;
                        // already close, no need for long cooldown
                        x.PanelRotorReverseCooldown = 0.2 * MILLISECONDSREVERSECOOLDOWN;
                        x.PanelRotorReversed = false;
                    }
                }
            });

            // start base rotor
            if (BaseRotor.TargetVelocityRad == 0.0f && !TPanelChilds.Exists(x => !x.PanelRotorClose && x.CurrentPowerVariable != 0.0f))
            {
                // stop all panels!
                TPanelChilds.ForEach(delegate (TPanelChild x)
                {
                    if (x.PanelRotor.TargetVelocityRad == 0.0f)
                        return;
                    x.PanelRotorTargetVelocityRadSign = Math.Sign(x.PanelRotor.TargetVelocityRad);
                    x.PanelRotor.TargetVelocityRad = 0.0f;
                });

                BaseRotor.TargetVelocityRad = BaseRotorTargetVelocityRadSign * ROTORVELOCITY;
                BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
            }
            // reverse base rotor only once
            else if (BaseRotor.TargetVelocityRad != 0.0f && BaseRotorReverseCooldown <= 0.0f)
            {
                // sum delta of each panel
                deltaP = 0.0f;
                foreach (TPanelChild x in TPanelChilds)
                    deltaP += x.CurrentPowerVariable - x.LastRelativeMaxPower;

                // lesser power, reverse or stop
                if (deltaP < 0)
                {
                    if (!BaseRotorReversed)
                    {
                        BaseRotor.ApplyAction(ACTION_REVERSE);
                        BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
                        PanelGlobalReset = GLOBALPANELRESETTIME;
                        BaseRotorReversed = true;
                    }
                    else
                    {
                        BaseRotorTargetVelocityRadSign = Math.Sign(BaseRotor.TargetVelocityRad);
                        BaseRotor.TargetVelocityRad = 0.0f;
                        BaseRotorReversed = false;
                        TPanelChilds.ForEach(delegate (TPanelChild x)
                        {
                            x.PanelRotorReversed = false;
                            x.PanelRotorClose = false;
                        });
                    }
                }
            }
        }
        TPanelChilds.ForEach(delegate (TPanelChild x)
        {
            x.LastRelativeMaxPower = x.CurrentPowerVariable;
        });
    }

    public override void Stop(ref string output)
    {
        BaseRotor.TargetVelocityRad = 0.0f;
        TPanelChilds.ForEach(x => x.PanelRotor.TargetVelocityRad = 0.0f);
        output += (IsLimited ? "Limited " : string.Empty) + "TPanel " + BaseRotor.CustomName + " stopped.\n\n";
    }

    protected class TPanelChild
    {
        public readonly IMyMotorStator PanelRotor;
        public readonly List<IMySolarPanel> SolarPanels;
        public float CurrentPowerVariable;
        public float LastRelativeMaxPower;
        public double PanelRotorReverseCooldown;
        public bool PanelRotorReversed, PanelRotorClose;
        public int PanelRotorTargetVelocityRadSign;

        public TPanelChild(IMyMotorStator childrotor, List<IMySolarPanel> childsolarpanels)
        {
            PanelRotor = childrotor;
            SolarPanels = childsolarpanels;
            LastRelativeMaxPower = 0.0f;
            PanelRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
            PanelRotorReversed = false;
            PanelRotorClose = false;
            PanelRotorTargetVelocityRadSign = 1;
            CurrentPowerVariable = 0f;
        }
    }
}

/// <summary>
/// U Panel
/// </summary>
public class TXC_UPanel : TXC_AbstractPanel
{
    protected readonly IMyMotorStator PanelRotor, PanelRollRotor;
    protected bool BaseRotorReversed = false, PanelRotorReversed = false;
    protected double PanelRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
    protected int PanelRotorTargetVelocityRadSign = 1;

    protected TXC_UPanel(IMyMotorStator baserotor, List<IMySolarPanel> solars, IMyMotorStator[] toprotors)
        : base(baserotor, solars)
    {
        PanelRotor = toprotors[0];
        PanelRotor.TargetVelocityRad = 0.0f;
        PanelRotor.Torque = ROTORTORQUE;
        PanelRotor.BrakingTorque = ROTORTORQUE;
        PanelRotor.ApplyAction(ACTION_ON);

        PanelRollRotor = toprotors[1];
        PanelRollRotor.TargetVelocityRad = 0.0f;
        PanelRollRotor.Torque = 0.0f;
        PanelRollRotor.BrakingTorque = 0.0f;
        PanelRollRotor.ApplyAction(ACTION_ON);

        IsLimited |= !float.IsInfinity(PanelRotor.LowerLimitDeg) || !float.IsInfinity(PanelRotor.UpperLimitDeg) || !float.IsInfinity(PanelRollRotor.LowerLimitDeg) || !float.IsInfinity(PanelRollRotor.UpperLimitDeg);
    }

    public static TXC_AbstractPanel TryCreatePanel(IMyGridTerminalSystem GridTerminalSystem, IMyMotorStator rotor)
    {
        // 2 rotors with same TopGrid & solars found there => U-Panel
        List<IMyMotorStator> rotors = new List<IMyMotorStator>();
        GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors, x => x.CubeGrid == rotor.TopGrid && !x.CustomName.Contains(EXCLUDESTRING) && x.TopGrid != null);
        if (rotors.Count != 2 || rotors[0].TopGrid != rotors[1].TopGrid)
            return null;

        List<IMySolarPanel> solars = new List<IMySolarPanel>();
        GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(solars, x => x.CubeGrid == rotors[0].TopGrid && !x.CustomName.Contains(EXCLUDESTRING));
        if (solars.Count == 0)
            return null;

        return new TXC_UPanel(rotor, solars, rotors.ToArray());
    }

    public override void Arrange(ref string output, double millisecondsSinceLastRun)
    {
        float current_p = GetCurrentRelativMaxOutput();
        if (BaseRotorReverseCooldown >= 0.0)
            BaseRotorReverseCooldown -= millisecondsSinceLastRun;
        if (PanelRotorReverseCooldown >= 0.0)
            PanelRotorReverseCooldown -= millisecondsSinceLastRun;
        if (PanelGlobalReset > 0.0)
            PanelGlobalReset = PanelGlobalReset - millisecondsSinceLastRun == 0.0 ? -1.0 : PanelGlobalReset - millisecondsSinceLastRun;

        output += ((IsLimited ? "Limited " : string.Empty) + "UPanel " + BaseRotor.CustomName + " (" + PanelGlobalReset.ToString("F0") + ")\n  P: " + current_p.ToString("F3") + (current_p - LastRelativeMaxPower < 0.0 ? " - " : " + ") + Math.Abs(current_p - LastRelativeMaxPower).ToString("F3") + "\n\n");
        // at least one panel must match the threshold (e.g. shadow) or rotor locked
        if (current_p > MINRELATIVEOUTPUT || LastRelativeMaxPower > MINRELATIVEOUTPUT || current_p == 0.0 || PanelGlobalReset < 0.0)
        {
            // stop rotors
            if (BaseRotor.TargetVelocityRad != 0.0f)
            {
                BaseRotorTargetVelocityRadSign = Math.Sign(BaseRotor.TargetVelocityRad);
                BaseRotor.TargetVelocityRad = 0.0f;
            }
            if (PanelRotor.TargetVelocityRad != 0.0f)
            {
                PanelRotorTargetVelocityRadSign = Math.Sign(PanelRotor.TargetVelocityRad);
                PanelRotor.TargetVelocityRad = 0.0f;
            }
            LastRelativeMaxPower = current_p;
            BaseRotorReversed = false;
            PanelRotorReversed = false;
            PanelGlobalReset = 0.0;
            return;
        }

        // Arrange start with PanelRotor
        if (PanelRotor.TargetVelocityRad == 0.0f && BaseRotor.TargetVelocityRad == 0.0f)
        {
            // take care of the roll rotor
            PanelRollRotor.TargetVelocityRad = 0.0f;
            PanelRollRotor.Torque = 0.0f;
            PanelRollRotor.BrakingTorque = 0.0f;

            PanelRotor.TargetVelocityRad = BaseRotorTargetVelocityRadSign * ROTORVELOCITY;
            PanelRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
        }
        // reverse or base alignement
        else if (current_p < LastRelativeMaxPower && PanelRotorReverseCooldown <= 0.0f || BaseRotor.TargetVelocityRad != 0.0f)
        {
            // reverse the panel rotor only once
            if (!PanelRotorReversed)
            {
                PanelRotor.ApplyAction(ACTION_REVERSE);
                PanelRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
                PanelRotorReversed = true;
            }
            // start aligning base rotor
            else
            {
                // stop panel rotor
                if (PanelRotor.TargetVelocityRad != 0.0f)
                {
                    PanelRotorTargetVelocityRadSign = Math.Sign(PanelRotor.TargetVelocityRad);
                    PanelRotor.TargetVelocityRad = 0.0f;
                }

                // start base rotor
                if (BaseRotor.TargetVelocityRad == 0.0f)
                {
                    BaseRotor.TargetVelocityRad = BaseRotorTargetVelocityRadSign * ROTORVELOCITY;
                    BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
                }
                // reverse base rotor only once
                else if (current_p < LastRelativeMaxPower && BaseRotorReverseCooldown <= 0.0f)
                {
                    if (!BaseRotorReversed)
                    {
                        BaseRotor.ApplyAction(ACTION_REVERSE);
                        BaseRotorReverseCooldown = MILLISECONDSREVERSECOOLDOWN;
                        PanelGlobalReset = GLOBALPANELRESETTIME;
                        BaseRotorReversed = true;
                    }
                    else
                    {
                        BaseRotorTargetVelocityRadSign = Math.Sign(BaseRotor.TargetVelocityRad);
                        BaseRotor.TargetVelocityRad = 0.0f;
                        BaseRotorReversed = false;
                        PanelRotorReversed = false;
                    }
                }
            }
        }
        LastRelativeMaxPower = current_p;
    }

    public override void Stop(ref string output)
    {
        BaseRotor.TargetVelocityRad = 0.0f;
        PanelRotor.TargetVelocityRad = 0.0f;
        PanelRollRotor.TargetVelocityRad = 0.0f;
        output += (IsLimited ? "Limited " : string.Empty) + "UPanel " + BaseRotor.CustomName + " stopped.\n\n";
    }
}

#endregion Code - don't change!