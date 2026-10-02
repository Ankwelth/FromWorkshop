double _serverMultiplier = 3.00;
string _lcdName = "Refiner Lcd";
int _textPaddingRight = 15;
int _textPaddingLeft = 15;
int _artificialTime;
int _artificialTimeMax = 10; // 16.6 seconds roughly

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    _artificialTime = _artificialTimeMax;
}

public void Main(string argument, UpdateType updateSource)
{
    
    _artificialTime--;
    
    if(_artificialTime  == 0){
        _artificialTime = _artificialTimeMax;
    }else{
        return;
    }

    var separator = $"{"-".PadRight(_textPaddingLeft + _textPaddingRight, '-')}";

    var lcd = GridTerminalSystem.GetBlockWithName(_lcdName) as IMyTextPanel;
    
    var output = "Refining Time (Current Grid)\n"+ separator + "\n";
    
    var oreQuantities = new Dictionary<string, int>() {
        {"Stone", 0},
        {"Scrap", 0},
        {"Iron", 0}, 
        {"Nickel", 0}, 
        {"Cobalt", 0}, 
        {"Magnesium", 0}, 
        {"Silicon", 0}, 
        {"Silver", 0}, 
        {"Gold", 0}, 
        {"Platinum", 0}, 
        {"Uranium", 0}, 
    };
    
    var oreSpeed = new Dictionary<string, double>() {
        {"Stone", 0.008},
        {"Scrap", 0.031},
        {"Iron", 0.038}, 
        {"Nickel", 0.508}, 
        {"Cobalt", 2.308}, 
        {"Magnesium", 0.385}, 
        {"Silicon", 0.462}, 
        {"Silver", 0.769}, 
        {"Gold", 0.308}, 
        {"Platinum", 2.308}, 
        {"Uranium", 3.077}, 
    };
    
    var oreRefineTime = new Dictionary<string, int>() {
        {"Stone", 0},
        {"Scrap", 0},
        {"Iron", 0}, 
        {"Nickel", 0}, 
        {"Cobalt", 0}, 
        {"Magnesium", 0}, 
        {"Silicon", 0}, 
        {"Silver", 0}, 
        {"Gold", 0}, 
        {"Platinum", 0}, 
        {"Uranium", 0}, 
    };
    
    
    var inventoryBlocks = new List<IMyTerminalBlock>();
    var refineryCount = 0;
    
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(
        inventoryBlocks, 
        block => ((block is IMyCargoContainer || block is IMyRefinery) && block.CubeGrid == Me.CubeGrid)
    );
    
    foreach (var block in inventoryBlocks.Where(block => block.InventoryCount != 0)) {
        var items = new List<MyInventoryItem>();
        if (block is IMyRefinery) {
            refineryCount++;
            var inputItems = new List<MyInventoryItem>();
            ((IMyRefinery) block).InputInventory.GetItems(inputItems);
            
            items = inputItems.ToList();
            
        } else {
            block.GetInventory().GetItems(items);
        }



        foreach (var item in items) {

            var subtype = item.Type.TypeId.Split('_')[1];
            if (subtype != "Ore") {
                continue;
            }

            var itemName = item.Type.SubtypeId;
            
            if(itemName!="Ice"){
                oreQuantities[itemName] = oreQuantities.GetValueOrDefault(itemName) + (int) item.Amount;
            }
        }
    }

    foreach (var item in oreQuantities)
    {
        oreRefineTime[item.Key] = (int)(((double) oreQuantities[item.Key] * oreSpeed[item.Key] / (double)refineryCount / _serverMultiplier));
    }
    
    var ores = oreRefineTime.OrderByDescending(pair => pair.Value).ToList();
    
    var totalRefiningTime = 0;
    
    for (int i = 0; i < ores.Count; i++) {
        var ore = ores[i];

        var refiningTime = new TimeSpan(0,0,oreRefineTime[ore.Key]);
        totalRefiningTime+=(int) refiningTime.TotalSeconds;
        if(ore.Value>0)
        {
            output+= $"{(ore.Key+":").PadRight(_textPaddingLeft)}{refiningTime.ToString().PadLeft(_textPaddingRight)}\n";
        }
    }
    
    if(totalRefiningTime>0)
    {
        output+=separator+"\n";
        var totalTimeSpan = new TimeSpan(0, 0, totalRefiningTime);
        output+=$"{("Total Time:".PadRight(_textPaddingLeft))}{totalTimeSpan.ToString().PadLeft(_textPaddingRight)}\n";
    }else{
        output+="Nothing to refine\n";
    }
    
    Echo(output);
    lcd.WriteText(output);
    
}