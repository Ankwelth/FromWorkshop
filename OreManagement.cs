// ========== Auto Ore Manager ==========
// Ore you want to monitor ("Stone", "Iron", "Nickel", "Cobalt", "Silicon", "Silver", "Gold", "Platinum", "Uranium", "Magnesium", "Ice", "Scrap")
// Modded Ores work too, consult Mod Docs for ID
string ORE_NAME = "Ice";

// Min Stock (Enable blocks when under this value)
int ORE_MIN = 20000;

// Max Stock (Disable blocks when over this value)
int ORE_MAX = 100000;

// Tag of the blocks you want to toggle
string BLOCK_TAG = "Ice Drill";

// ================ Code ================

List<IMyCargoContainer> cargo;
List<IMyTerminalBlock> allBlocks;
List<IMyFunctionalBlock> targets;

IMyTextSurface screen;
string screenText;

bool isOn;

public Program()
{
    // Speed of Updates - No use being faster than 100 (Lower Faster)
    Runtime.UpdateFrequency = UpdateFrequency.Update100;

    screen = Me.GetSurface(0);
    screen.ContentType = ContentType.TEXT_AND_IMAGE;
    screen.FontSize = 0.8f;

    if (!bool.TryParse(Storage, out isOn))
        isOn = true;
}

public void Main(string argument, UpdateType updateSource)
{
    cargo = new List<IMyCargoContainer>();
    allBlocks = new List<IMyTerminalBlock>();
    targets = new List<IMyFunctionalBlock>();

    MyInventoryItem item;
    int oreAmount = 0;
    screenText = "";

    string searchString = "Ore/" + ORE_NAME;


    GridTerminalSystem.GetBlocksOfType(cargo);
    for (int i = 0; i < cargo.Count; i++)
    {
        for (int j = 0; j < cargo[i].GetInventory(0).ItemCount; j++)
        {
            item = cargo[i].GetInventory(0).GetItemAt(j).Value;

            if (item.ToString().IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                oreAmount += (int)item.Amount;
            }
        }
    }


    GridTerminalSystem.GetBlocksOfType(allBlocks, b => b.CustomName.Contains(BLOCK_TAG));
    for (int i = 0; i < allBlocks.Count; i++)
    {
        IMyFunctionalBlock fb = allBlocks[i] as IMyFunctionalBlock;
        if (fb != null)
            targets.Add(fb);
    }

    if (oreAmount >= ORE_MAX)
        isOn = false;
    else if (oreAmount <= ORE_MIN)
        isOn = true;

    Storage = isOn.ToString();

    string power = isOn ? "On" : "Off";

    // Turn targets on or off
    for (int i = 0; i < targets.Count; i++)
    {
        targets[i].ApplyAction("OnOff_" + power);
    }

    WriteText("========== Auto Ore Manager ==========");
    WriteText("Monitoring: " + ORE_NAME);
    WriteText("Stock: " + oreAmount + "  (Min " + ORE_MIN + " / Max " + ORE_MAX + ")");
    WriteText("Status: " + power);

    WriteText("\nControlled blocks (tag: \"" + BLOCK_TAG + "\"):");
    for (int i = 0; i < targets.Count; i++)
    {
        WriteText(targets[i].CustomName);
    }

    screen.WriteText(screenText);
}

public void WriteText(string x)
{
    Echo(x);
    screenText += x + "\n";
}