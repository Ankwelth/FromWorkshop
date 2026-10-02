/*#################################################################################################
Hydrogen Units Gauge Overlay (HUGO) v1
Script written by RaVen, adapted for use on a MP server
For setup, add the following tags to blocks for the system to pick them up
 - Small thrusters [HS]
 - Large thrusters [HL]
 - Hydrogen tanks [HT]
 - Output LCDs [H]
*/
//#################################################################################################

float hydrogenMaxConsumptionLarge = 4820F; // Should be the max consumption for a single large thruster. This shouldn't need to change if your ship is large grid.
float hydrogenMaxConsumptionSmall = 803.34F; // Should be the max consumption for a single small thruster. This shouldn't need to change if your ship is large grid.


public void Main(string argument, UpdateType updateSource)
{
    try
    {
        switch (updateSource)
        {
            case UpdateType.Terminal:
            case UpdateType.Trigger:
                OS(argument);
                break;
            case UpdateType.Update10:
                clock.Tick();

                if (clock.oncePer())
                {
                    GridUpdate();
                }

                if (clock.everyOther())
                {
                    ThrusterUpdate();
                    LCDUpdate();
                }

                break;
        }
    }
    catch (Exception e) { Echo(e + ""); }
}

#region Grid Routines

void GridUpdate()
{
    List<IMyTerminalBlock> lcdBlocks = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> thrustBlocks = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> tankBlocks = new List<IMyTerminalBlock>();
    largeThrustersList.Clear();
    smallThrustersList.Clear();
    tankList.Clear();
    lcdList.Clear();

    GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrustBlocks, thisGrid);
    foreach (IMyThrust thrust in thrustBlocks)
    {
        if (thrust.CustomName.Contains(largeTag))
        {
            largeThrustersList.Add(thrust);
            continue;
        }
        if (thrust.CustomName.Contains(smallTag))
        {
            smallThrustersList.Add(thrust);
            continue;
        }
    }

    GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tankBlocks, thisGrid);
    foreach (IMyGasTank tank in tankBlocks)
    {
        if (tank.CustomName.Contains(tankTag))
        {
            tankList.Add(tank);
        }
    }

    GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcdBlocks, thisGrid);
    foreach (IMyTextPanel lcd in lcdBlocks)
    {
        if (lcd.CustomName.Contains(lcdTag))
        {
            lcdList.Add(lcd);
            lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        }
    }
}

void LCDUpdate()
{
    foreach(IMyTextPanel lcd in lcdList)
    {
        if (lcd == null) { continue; }
        lcd.WriteText(hydrogenText, false);
    }
}

void ThrusterUpdate()
{
    float hydrogenCurrentConsumption = 0F;

    foreach (IMyThrust thruster in smallThrustersList)
    {
        if (thruster == null) { continue; }
        if (thruster.CurrentThrust == 0F) { hydrogenCurrentConsumption += 0; }
        else
        {
            float currentThrustMult = thruster.CurrentThrust / thruster.MaxThrust;
            hydrogenCurrentConsumption += hydrogenMaxConsumptionSmall * currentThrustMult;
        }
    }

    foreach (IMyThrust thruster in largeThrustersList)
    {
        if (thruster == null) { continue; }
        if (thruster.CurrentThrust == 0F) { hydrogenCurrentConsumption += 0; }
        else
        {
            float currentThrustMult = thruster.CurrentThrust / thruster.MaxThrust;
            hydrogenCurrentConsumption += hydrogenMaxConsumptionLarge * currentThrustMult;
        }
    }

    hydrogenText.Clear();
    hydrogenText.AppendLine("Hydrogen Consumption: ");
    hydrogenCurrentConsumption = (float)Math.Round(hydrogenCurrentConsumption,2);
    hydrogenText.AppendLine(hydrogenCurrentConsumption.ToString() + " L/S");

    float totalRemainingHydrogen = 0;
    foreach (IMyGasTank gasTank in tankList)
    {
        float remainingHydrogen = gasTank.Capacity * ((float)gasTank.FilledRatio);
        totalRemainingHydrogen += remainingHydrogen;
    }
    hydrogenText.AppendLine("Remaining Hydrogen: ");
    hydrogenText.AppendLine(fancyNumber(totalRemainingHydrogen));

    float remainingSeconds = totalRemainingHydrogen / hydrogenCurrentConsumption;
    if (remainingSeconds > 0 && remainingSeconds < 999999999)
    {
        TimeSpan timeRemaining = TimeSpan.FromSeconds(remainingSeconds);
        hydrogenText.AppendLine("Time Until Depleted:");
        hydrogenText.AppendLine(timeRemaining.ToString(@"hh\:mm\:ss"));
    }
}

#endregion

#region Statics

string fancyNumber(float input)
{
    string output = "";
    int result = 0;
    result = (int)Math.Round(input);
    output = result.ToString() + " L";
    return output;
}

void OS(string input)
{
    switch(input)
    {
        case "reset":
            GridUpdate();
            break;
        case "test":
            Echo("Daisy daisy...");
            break;
        default:
            break;
    }
}

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;

    clock = new Clock();

    smallThrustersList = new List<IMyTerminalBlock>();
    largeThrustersList = new List<IMyTerminalBlock>();
    tankList = new List<IMyTerminalBlock>();
    lcdList = new List<IMyTerminalBlock>();

    hydrogenText = new StringBuilder();

    GridUpdate();
    Echo("System Online");
}

bool thisGrid(IMyTerminalBlock block)
{
    return block.CubeGrid == Me.CubeGrid;
}

public class Clock
{
    public int Mil;
    public int Sec;
    public int Bar;

    public void Tick()
    {
        ++Mil;
        if (Mil >= 6)
        {
            Sec++;
            Mil = 0;
            ++Bar;
            if (Bar >= 10) { Bar = 0; }
        }
        if (Sec >= 10)
        {
            Sec = 0;
        }
    }

    public bool oncePer()
    {
        if (Mil == 0 && Sec == 1) { return true; }
        return false;
    }

    public bool everyOther()
    {
        if(Mil == 1 || Mil == 3 || Mil == 5) { return true; }
        return false;
    }
}

#endregion

#region Variables
Clock clock;

List<IMyTerminalBlock> smallThrustersList;
List<IMyTerminalBlock> largeThrustersList;
List<IMyTerminalBlock> tankList;
List<IMyTerminalBlock> lcdList;

StringBuilder hydrogenText;

string smallTag = "[HS]";
string largeTag = "[HL]";
string tankTag = "[HT]";
string lcdTag = "[H]";

#endregion