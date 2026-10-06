    #region CargoCapacityScriptPlus

    //------------------------------------ Config ------------------------------------//
    //================================================//

    /*
    Cargo Capacity Script +

    This is a script to monitor the capacity of any cargo containers, connectors or other storage blocks like Drills or Welders.

    This script can be used on any grid. It excludes any further connectet grids.

    Supports displaying to any nameable LCD Block and includs displaying to LCD's lnside Cockpit Blocks.

    Outputs the Cargo Level in "%" then Battery Power Level in "%" and at least Reaktor Fuel Level in "%"
    Includes a progressbar for every value.
    */

    //Name of your Cockpit Block -> Can be Changed
    readonly string CockpitName = "Industrial Cockpit";

    //Lcd Panel Number on your Cockpit Block -> Changed to desired Value
    //To See the Abount of found LCD's "LCD Screens in Cockpit: x" on your Cockpit Block look at the last entry on the Debug Display of your Programmable Block
    //Default value: 2 -> Is the second Screen on the Cockpit
    readonly int LcdPanelNumber = 2;

    //Disables/Enables Output to given Cocpit LCD -> "true" = Enabled Output, "false" = Disabled Output
    readonly bool DoCocpitLCDOut = true;

    //Name of your LCD Pannel -> Can be Changed
    readonly string LcdName = "LCD";

    //Disables/Enables Output to LCD Pannel -> "true" = Enabled Output, "false" = Disabled Output
    readonly bool DoLCDOut = true;

    //Disables/Enables Output for more detailed/atvanced Output to the LCD's -> "true" = Enabled Output, "false" = Disabled Output
    readonly bool DoAdvancedOutput = false;
    
    //Disables/Enables Output of current Uranium level from "m³" to "MWh" -> "true" = Enabled Output, "false" = Disabled Output
    readonly bool DoReaktorPowerInMWh = false;

    //Canges length of all Progressbars
    //Default length: 50 -> Number of characters between brackets
    readonly int barLength = 50;

    //-------------------- Symbols Used by Progressbar --------------------//
    //================================================//

    //Example Progressbar using default character set:

    //{||||||||||||||||||||||||||||||||||||||||||||||||||} -> Full Progressbar (100%)

    //{|||||||||||||||||||||||||'''''''''''''''''''''''''} -> 1/2 filled Progressbar (50%)

    //{''''''''''''''''''''''''''''''''''''''''''''''''''} -> Emty Progressbar (0%)


    //Canges the character used to display a full Progressbar
    //Default character: |
    readonly string BarFull = "|";

    //Canges the character used to display a empty Progressbar
    //Default character: '
    readonly string BarEmpty = "'";

    //Canges the character used at the beginning of The Progressbar
    //Default character: {
    readonly string BarStart = "{";

    //Canges the character used at the end of The Progressbar
    //Default character: }
    readonly string BarEnd = "}";
    
    //-------------------------------- End of Config --------------------------------//
    //================================================//

    string CapacityBar;
    string PowerBar;
    string UraniumBar;

    List<IMyFunctionalBlock> Tools;

    float MaxPower = 0.0f;
    float CurrentPower = 0.0f;

    float MaxCapacity = 0.0f;
    float CurrentCapacity = 0.0f;

    float MaxUranium = 0.0f;
    float CurrentUranium = 0.0f;

    float CapacityInPercent = 0.0f;
    float ChargeInPercent = 0.0f;
    float ReaktorPowerInPercent = 0.0f;

    string ComparativeReaktorPowerValues;
    string ComparativeCapacityValues;
    string ComparativeChargeValues;
    

    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }

    public static int MathClamp(int value, int min, int max)
    {
        return Math.Min(max, Math.Max(value, min));
    }


    public void PercentageBars()
    {
        CapacityBar = BarStart;
        var filledBars = (int)(barLength * (Math.Round(CurrentCapacity, 3) / MaxCapacity));
        var emtyBars = barLength - filledBars;

        for (int i = filledBars; i > 0; i--)
        {
            CapacityBar += BarFull;
        }

        for (int i = emtyBars; i > 0; i--)
        {
            CapacityBar += BarEmpty;
        }
        CapacityBar += BarEnd;


        PowerBar = BarStart;
        filledBars = (int)(barLength * (Math.Round(CurrentPower, 3) / MaxPower));
        emtyBars = barLength - filledBars;

        for (int i = filledBars; i > 0; i--)
        {
            PowerBar += BarFull;
        }

        for (int i = emtyBars; i > 0; i--)
        {
            PowerBar += BarEmpty;
        }
        PowerBar += BarEnd;


        UraniumBar = BarStart;
        filledBars = (int)(barLength * (Math.Round(CurrentUranium, 3) / MaxUranium));
        emtyBars = barLength - filledBars;

        for (int i = filledBars; i > 0; i--)
        {
            UraniumBar += BarFull;
        }

        for (int i = emtyBars; i > 0; i--)
        {
            UraniumBar += BarEmpty;
        }
        UraniumBar += BarEnd;
    }


    public void DetectShipTools()
    {
        List<IMyShipDrill> drills = new List<IMyShipDrill>();
        List<IMyShipWelder> welders = new List<IMyShipWelder>();
        List<IMyShipGrinder> grinders = new List<IMyShipGrinder>();
        Tools = new List<IMyFunctionalBlock>();
        GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills, block => block.IsSameConstructAs(Me));
        if (drills != null && drills.Count() > 0)
        {
            for (int i = 0; i < drills.Count(); i++)
            {
                Tools.Add((IMyFunctionalBlock)drills[i]);
            }
        }
        GridTerminalSystem.GetBlocksOfType<IMyShipWelder>(welders, block => block.IsSameConstructAs(Me));
        if (welders != null && welders.Count() > 0)
        {
            for (int i = 0; i < welders.Count(); i++)
            {
                Tools.Add((IMyFunctionalBlock)welders[i]);
            }
        }
        GridTerminalSystem.GetBlocksOfType<IMyShipGrinder>(grinders, block => block.IsSameConstructAs(Me));
        if (grinders != null && grinders.Count() > 0)
        {
            for (int i = 0; i < grinders.Count(); i++)
            {
                Tools.Add((IMyFunctionalBlock)grinders[i]);
            }
        }
    }


    public void Main(string argument)
    {
        MaxPower = 0;
        CurrentPower = 0;

        MaxCapacity = 0;
        CurrentCapacity = 0;

        MaxUranium = 0;
        CurrentUranium = 0;


        List<IMyCargoContainer> CargoContainers = new List<IMyCargoContainer>();
        GridTerminalSystem.GetBlocksOfType(CargoContainers, block => block.IsSameConstructAs(Me));

        List<IMyShipConnector> ShipConnectors = new List<IMyShipConnector>();
        GridTerminalSystem.GetBlocksOfType(ShipConnectors, block => block.IsSameConstructAs(Me));

        List<IMyBatteryBlock> Batteries = new List<IMyBatteryBlock>();
        GridTerminalSystem.GetBlocksOfType(Batteries, block => block.IsSameConstructAs(Me));

        List<IMyReactor> Reactors = new List<IMyReactor>();
        GridTerminalSystem.GetBlocksOfType(Reactors, block => block.IsSameConstructAs(Me));

        DetectShipTools();


        Echo($"Tools Count {Tools.Count()}");

        Echo($"Cargo Container Count {CargoContainers.Count()}");

        Echo($"Battery Count {Batteries.Count()}");

        Echo($"Reactor Count {Reactors.Count()}");

        Echo("");


        for (int i = 0; i < CargoContainers.Count; i++)
        {
            var inventory = CargoContainers[i].GetInventory(0);
            Echo($"Current Container Volume {i} : ");
            Echo(inventory.CurrentVolume.ToString() + "m³");
            MaxCapacity += (float)inventory.MaxVolume;
            CurrentCapacity += (float)inventory.CurrentVolume;
        }

        for (int i = 0; i < ShipConnectors.Count; i++)
        {
            var inventory = ShipConnectors[i].GetInventory(0);
            Echo($"Current Connector Volume {i} : ");
            Echo(inventory.CurrentVolume.ToString() + "m³");
            MaxCapacity += (float)inventory.MaxVolume;
            CurrentCapacity += (float)inventory.CurrentVolume;
        }

        for (int i = 0; i < Tools.Count; i++)
        {
            var inventory = Tools[i].GetInventory(0);
            Echo($"Current Tool Volume {i} : ");
            Echo(inventory.CurrentVolume.ToString() + "m³");
            MaxCapacity += (float)inventory.MaxVolume;
            CurrentCapacity += (float)inventory.CurrentVolume;
        }

        for (int i = 0; i < Batteries.Count; i++)
        {
            float MSP = Batteries[i].MaxStoredPower;
            float CSP = Batteries[i].CurrentStoredPower;
            MaxPower += MSP;
            CurrentPower += CSP;
            Echo($"Current Battery Charge {i} : ");
            Echo(CSP + "MWh");
        }

        for (int i = 0; i < Reactors.Count; i++)
        {
            var inventory = Reactors[i].GetInventory(0);
            Echo($"Current Uranium Volume {i} : ");
            Echo(inventory.CurrentVolume.ToString() + "m³");
            MaxUranium += (float)inventory.MaxVolume;
            CurrentUranium += (float)inventory.CurrentVolume;
        }


        float UraniuminMWh = CurrentUranium * 20000; // 1Kg Uranium = 0,05L = 1MWh Enegy -> fator for "m³" to "MWh" is: * 20000
        float UraniuminMWhMax = MaxUranium * 20000; // 1Kg Uranium = 0,05L = 1MWh Enegy -> fator for "m³" to "MWh" is: * 20000


        CapacityInPercent = CurrentCapacity / MaxCapacity * 100;

        ChargeInPercent = CurrentPower / MaxPower * 100;

        ReaktorPowerInPercent = CurrentUranium / MaxUranium * 100;

        string StorageFloat = String.Format("Cargo Load Level: {0:0.00} %", CapacityInPercent);

        string BatteryFloat = String.Format("Battery PW Level: {0:0.00} %", ChargeInPercent);

        string ReactorFloat = String.Format("Reactor Fuel Level: {0:0.00} %", ReaktorPowerInPercent);


        ComparativeCapacityValues = Math.Round(CurrentCapacity, 3) + "m³" + " / " + MaxCapacity + "m³";

        ComparativeChargeValues = Math.Round(CurrentPower, 3) + "MWh" + " / " + MaxPower + "MWh";

        if (DoReaktorPowerInMWh == true)
        {
            ComparativeReaktorPowerValues = Math.Round(UraniuminMWh, 3) + "MWh" + " / " + UraniuminMWhMax + "MWh";
        }
        else
        {   
            ComparativeReaktorPowerValues = Math.Round(CurrentUranium, 3) + "m³" + " / " + MaxUranium + "m³";
        }


        PercentageBars();

        string TextOutput =
            StorageFloat + "\n" + CapacityBar +
            "\n" + BatteryFloat + "\n" + PowerBar +
            "\n" + ReactorFloat + "\n" + UraniumBar;
            
        string AdvancedTextOutput =
            StorageFloat + "\n" + CapacityBar + "\n" + ComparativeCapacityValues +
            "\n" + BatteryFloat + "\n" + PowerBar + "\n" + ComparativeChargeValues +
            "\n" + ReactorFloat + "\n" + UraniumBar + "\n" + ComparativeReaktorPowerValues;

        
        Echo("");

        Echo("Current Capacity / Max Capacity");
        Echo(CurrentCapacity + "m³" + " / " + MaxCapacity + "m³");

        Echo("Current Power / Max Power");
        Echo(CurrentPower + "MWh" + " / " + MaxPower + "MWh");

        Echo("Current Uranium / Max Uranium");
        Echo(CurrentUranium + "m³" + " / " + MaxUranium + "m³");
        Echo(UraniuminMWh + "MWh" + " / " + UraniuminMWhMax + "MWh");

        Echo("");
        Echo(TextOutput);   

        Echo("");
        Echo(AdvancedTextOutput);


        var MyLCDPanel = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName(LcdName, MyLCDPanel, block => block.IsSameConstructAs(Me));
        
        for(int i = 0; i < MyLCDPanel.Count(); i++)
        {
            if (DoLCDOut == true || DoAdvancedOutput == false)
            {
                ((IMyTextSurfaceProvider)MyLCDPanel[i]).GetSurface(0).WriteText(TextOutput);
            }
            if (DoLCDOut == true || DoAdvancedOutput == true)
            {
                ((IMyTextSurfaceProvider)MyLCDPanel[i]).GetSurface(0).WriteText(AdvancedTextOutput);
            }
        }

        var MyCockpitBlock = new List<IMyTerminalBlock>();
        GridTerminalSystem.SearchBlocksOfName(CockpitName, MyCockpitBlock, block => block.IsSameConstructAs(Me));
        
        for(int i = 0; i < MyCockpitBlock.Count(); i++)
        {
            int LCDSurfaceCount = ((IMyTextSurfaceProvider)MyCockpitBlock[i]).SurfaceCount;

            int LcdPanelNumberClamped = MathClamp(LcdPanelNumber, 1, LCDSurfaceCount) - 1;

            Echo("");
            Echo("LCD Screens in Cockpit " + i + ": " + LCDSurfaceCount);
            Echo("LCD Screen selected: " + (LcdPanelNumberClamped + 1));
            
            if (DoCocpitLCDOut == true || DoAdvancedOutput == false)
            {
                ((IMyTextSurfaceProvider)MyCockpitBlock[i]).GetSurface(LcdPanelNumberClamped).WriteText(TextOutput);
            }
            if (DoCocpitLCDOut == true || DoAdvancedOutput == true)
            {
                ((IMyTextSurfaceProvider)MyCockpitBlock[i]).GetSurface(LcdPanelNumberClamped).WriteText(AdvancedTextOutput);
            }
        }
    }

    #endregion // CargoCapacityScriptPlus
