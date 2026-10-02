/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */


double timer = 0;
double closeDoorDelay = 1.5;


//  keep running and defaultItemList
public Program()
{

    //  keep running

    Echo("Program started!");

    Runtime.UpdateFrequency = UpdateFrequency.Update100;


}// end program


//  get name and amount from the collection, add the number of the same name
public void SearchThings(Dictionary<string, double> itemList)
{

    Echo("SearchThings started!");

    //  Build a itemListTemp and a cargocontainerList
    List<MyInventoryItem> itemCollectionTemp = new List<MyInventoryItem>();
    List<IMyCargoContainer> containerList = new List<IMyCargoContainer>();


    //  Fill a list with all CargoContainers in the system
    GridTerminalSystem.GetBlocksOfType(containerList);

    //  Get one specified container each time from the containerList
    foreach (IMyCargoContainer containerTemp in containerList)
    {

        //  Fill a list with all items in that specified container
        containerTemp.GetInventory().GetItems(itemCollectionTemp);

    }// end foreach

    //  Check itemCollection
    if (itemCollectionTemp == null)
    {
        Echo("itemList not found");
        //break;
    }
    Echo("get itemList");
    //  end check itemCollection


    //  get name and amount from the collection, add the number of the same name
    foreach (MyInventoryItem itemTemp in itemCollectionTemp)
    {
        Item item = new Item();
        item.name = itemTemp.Type.SubtypeId;
        item.amount = itemTemp.Amount.RawValue;

        double result = 0;
        if (itemList.TryGetValue(item.name, out result))
        {
            itemList[item.name] += item.amount;
        }
        else
        {
            itemList.Add(item.name, item.amount);
        }

    }// end foreach itemTemp


    //  check value
    int k = 1;
    foreach (KeyValuePair<string, double> dicTemp in itemList)
    {
        Echo(k + "_" + dicTemp.Key + "_" + dicTemp.Value);
        k++;
    }// end check value


}// end SearchThings


//  Separate the the itemList into different LCDs
public void LCDShow(int i, int j, Dictionary<string, double> itemList)
{

    Echo("LcdShow started!");

    //  Build a new value about lcdpanel
    IMyTextPanel panel;


    //  Clear all panels
    for (int a = 1; a <= i; a++)
    {
        for (int b = 1; b <= j; b++)
        {
            //  Build the name of a LCD
            StringBuilder lcdName = new StringBuilder();
            lcdName.Append("lcdInformation");
            lcdName.Append(a);
            lcdName.Append(b);

            //  get a panel
            panel = GridTerminalSystem.GetBlockWithName(lcdName.ToString()) as IMyTextPanel;
            panel.WriteText("", false);
            // contenttype
            panel.ContentType = ContentType.TEXT_AND_IMAGE;
            // fontsize & fontcolor
            panel.FontSize = (float)4.1;
            panel.FontColor = new Color(0, 130, 255);

        }
    }// end clear all panels

    //  build a list of keys(names)
    Dictionary<string, double>.KeyCollection keyCol = itemList.Keys;
    int k = 1;
    foreach (string key in keyCol)//    Get the upper boundary
    {
        k++;
    }
    string[] keyList = new string[k];// build a new empty list
    k = 0;
    foreach (string key in keyCol)//    assign the value to the list
    {
        keyList[k] = key;
        k++;
    }
    //  end build keylist

    //  select panel coordinate
    //  select row panel
    for (int a = 1; a <= i; a++)
    {
        //  select column panel
        for (int b = 1; b <= j; b++)
        {

            //  build the lcdname
            StringBuilder lcdName = new StringBuilder();
            lcdName.Append("lcdInformation");
            lcdName.Append(a);
            lcdName.Append(b);

            //  get a panel
            panel = GridTerminalSystem.GetBlockWithName(lcdName.ToString()) as IMyTextPanel;

            //  write information by row, each cycle writes 4 rows
            for (int x = 0; x <= 3; x++)
            {
                int y = 0;

                y = (a - 1) * 4 + x;

                if (y <= itemList.Count - 1)
                {

                    string name = keyList[y];
                    double amount = itemList[name];
                    CheckName(ref name, ref amount);
                    StringBuilder str = new StringBuilder();
                    str.Append(y);
                    str.Append(".");
                    str.Append(name);
                    str.Append("=");
                    amount = Math.Round(amount, 2);
                    str.Append(amount);
                    str.Append("\n");
                    Echo(str.ToString());
                    panel.WriteText(str, true);

                }
                else
                {
                    break;
                }

            }// end for x

            //Echo(panel.CustomName);

        }// end for b select column

    }// end for a select row

}// end Lcdshow


//  Change some name into abbreviation
public void CheckName(ref string name, ref double amount)
{
    switch (name)
    {
        case "HydrogenBottle":
            //name = "H2Bottle";
            name = "氢气瓶";
            amount /= 1000000;
            break;
        case "OxygenBottle":
            //name = "O2Bottle";
            name = "氧气瓶";
            amount /= 1000000;
            break;
        case "NATO_5p56x45mm":
            name = "5.56x45";
            amount /= 1000000;
            break;
        case "Missile200mm":
            name = "SAM";
            amount /= 1000000;
            break;
        case "BulletproofGlass":
            name = "BPGlass";
            name = "防弹玻璃";
            amount /= 1000000;
            break;
        case "SpaceCredit":
            //name = "SC";
            name = "太空货币";
            amount /= 1000000;
            break;
        case "AutomaticRifleItem":
            name = "Rifle";
            amount /= 1000000;
            break;
        case "RadioCommunication":
            //name = "Radio";
            name = "无线零件";
            amount /= 1000000;
            break;
        case "InteriorPlate":
            //name = "InPlate";
            name = "内衬板";
            amount /= 1000000;
            break;
        case "Construction":
            //name = "Const.";
            name = "结构零件.";
            amount /= 1000000;
            break;
        case "SmallTube":
            //name = "S.Tube";
            name = "小钢管";
            amount /= 1000000;
            break;
        case "LargeTube":
            //name = "L.Tube";
            name = "大钢管";
            amount /= 1000000;
            break;
        case "SteelPlate":
            //name = "StPlate";
            name = "钢板";
            amount /= 1000000;
            break;
        case "AngleGrinderItem":
            //name = "Grinder";
            name = "角磨机";
            amount /= 1000000;
            break;
        case "Iron":
            //name = "Fe";
            name = "铁";
            amount /= 1000000;
            break;
        case "Silicon":
            //name = "Si";
            name = "硅";
            amount /= 1000000;
            break;
        case "Cobalt":
            //name = "Co";
            name = "钴";
            amount /= 1000000;
            break;
        case "Silver":
            //name = "Ag";
            name = "银";
            amount /= 1000000;
            break;
        case "Uranium":
            //name = "U";
            name = "铀";
            amount /= 1000000;
            break;
        case "Gold":
            //name = "Au";
            name = "金";
            amount /= 1000000;
            break;
        case "Nickel":
            //name = "Ni";
            name = "镍";
            amount /= 1000000;
            break;
        case "HandDrillItem":
            name = "HandDrill";
            amount /= 1000000;
            break;
        case "WelderItem":
            name = "Welder";
            amount /= 1000000;
            break;
        case "Magnesium":
            //name = "Mg";
            name = "镁";
            amount /= 1000000;
            break;
        case "MetalGrid":
            name = "金属网格";
            amount /= 1000000;
            break;
        case "Girder":
            name = "梁";
            amount /= 1000000;
            break;
        case "PowerCell":
            name = "动力电池";
            amount /= 1000000;
            break;
        case "Medical":
            name = "医疗零件";
            amount /= 1000000;
            break;
        default:
            amount /= 1000000;
            break;
        case "Superconductor":
            name = "超导体";
            amount /= 1000000;
            break;
        case "Computer":
            name = "计算机";
            amount /= 1000000;
            break;
        case "Ice":
            name = "冰";
            amount /= 1000000;
            break;
        case "Organic":
            name = "有机物";
            amount /= 1000000;
            break;
        case "Stone":
            name = "石头";
            amount /= 1000000;
            break;
        case "Motor":
            name = "电机";
            amount /= 1000000;
            break;
        case "Reactor":
            name = "反应堆零件";
            amount /= 1000000;
            break;
        case "Shale":
            name = "页岩";
            amount /= 1000000;
            break;
        case "ICPoint":
            name = "工业凭证";
            amount /= 1000000;
            break;
        case "XPGC":
            name = "工程经验点";
            amount /= 1000000;
            break;
        case "XPHT":
            name = "航天经验点";
            amount /= 1000000;
            break;
        case "XPWQ":
            name = "武器经验点";
            amount /= 1000000;
            break;
        case "Detector":
            name = "探测器零件";
            amount /= 1000000;
            break;
        case "Display":
            name = "显示器";
            amount /= 1000000;
            break;
        case "Heavyoil":
            name = "重油";
            amount /= 1000000;
            break;
        case "aa":
            name = "aa";
            amount /= 1000000;
            break;

            /*


Heavyoil


                     */
    }
}

public void DoorControl()
{

    List<IMyDoor> doorList = new List<IMyDoor>();

    GridTerminalSystem.GetBlocksOfType<IMyDoor>(doorList);

    IMyTextSurface panel = Me.GetSurface(0);

    foreach (IMyDoor door in doorList)
    {

        if (door.Status == DoorStatus.Open)
        {
            timer += Runtime.TimeSinceLastRun.TotalSeconds;
            panel.WriteText("Door has been opened!!", false);
            if (timer > closeDoorDelay)
            {
                door.CloseDoor();
                panel.WriteText("\n" + "Door has been closed!!", true);
            }
        }
        else
        {
            panel.WriteText("Door has been closed!!");
        }


    }

}

public void Power()
{

}

public struct Item
{
    public string name;
    public double amount;
}


//  XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
public void Main(string argument, UpdateType updateSource)
{

    Echo("main started!");

    List<Item> rawItemList = new List<Item>();
    Dictionary<string, double> itemList = new Dictionary<string, double>();

    SearchThings(itemList);
    LCDShow(8, 4, itemList);


    DoorControl();



}// end Main    XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX