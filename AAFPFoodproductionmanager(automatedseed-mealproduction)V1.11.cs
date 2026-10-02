// R e a d m e
// -----------
// 
//  AAFP Food Production Queue Management V1.11 by Adomus
// 
// 
    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
        PrecalculateBars();
        InitializeItems();
    }

    public void Save()
    {
        Storage = Me.CustomData;
    }
    string ver = "V1.11";
    string container_tag = "[AAFP IntrepidParts]";
    string lcd_display_name = "[AAFP IntrepidParts G1 D1]";
    int lcd_display_index = 0; //used for devices with multiple screen panels (0+)               
    string lcd_display_name_2 = "[AAFP IntrepidParts G1 D2]";
    int lcd_display_index_2 = 0; //used for devices with multiple screen panels (0+)
    string lcd_display_name_3 = "[AAFP IntrepidParts G2 D1]";
    int lcd_display_index_3 = 0; //used for devices with multiple screen panels (0+)               
    string lcd_display_name_4 = "[AAFP IntrepidParts G2 D2]";
    int lcd_display_index_4 = 0; //used for devices with multiple screen panels (0+)
    string lcd_display_name_5 = "[AAFP IntrepidParts G3 D1]";
    int lcd_display_index_5 = 0; //used for devices with multiple screen panels (0+)               
    string lcd_display_name_6 = "[AAFP IntrepidParts G3 D2]";
    int lcd_display_index_6 = 0; //used for devices with multiple screen panels (0+)
    float fontzoom = 0.60f;
    float fontzoom_2 = 0.49f;
    //display settings

    string container_tag_applied = "";
    string lcd_display_name_applied = "";
    string lcd_display_name_applied_2 = "";
    string lcd_display_name_applied_3 = "";
    string lcd_display_name_applied_4 = "";
    string lcd_display_name_applied_5 = "";
    string lcd_display_name_applied_6 = "";

    string displaygroup_1 = "G1";
    string displaygroup_2 = "G2";
    string displaygroup_3 = "G3";

    string displayhalf_1 = "D1";
    string displayhalf_2 = "D2";




    string itemtype = "ConsumableItem";
    string itemtype_2 = "SeedItem";
    string itemtype_3 = "PhysicalObject";
   // string blueprinttype = "BlueprintDefinition";
    string itemname_1 = "CookMammalMeat";
    string itemname_2 = "CookSpiderMeat";
    string itemname_3 = "MealPack_KelpCrisp";
    string itemname_4 = "MealPack_FruitBar";
    string itemname_5 = "MealPack_GardenSlaw";
    string itemname_6 = "MealPack_RedPellets";
    string itemname_7 = "MealPack_Chili";
    string itemname_8 = "MealPack_Flatbread";
    string itemname_9 = "MealPack_Ramen";
    string itemname_10 = "MealPack_FruitPastry";
    string itemname_11 = "MealPack_VeggieBurger";
    string itemname_12 = "MealPack_Curry";
    string itemname_13 = "MealPack_GreenPellets";
    string itemname_14 = "MealPack_Dumplings";
    string itemname_15 = "MealPack_Spaghetti";
    string itemname_16 = "MealPack_Lasagna";
    string itemname_17 = "MealPack_Burrito";
    string itemname_18 = "MealPack_FrontierStew";
    string itemname_19 = "MealPack_SearedSabiroid";
    string itemname_20 = "MealPack_SteakDinner";
    string itemname_21 = "Fruit";
    string itemname_22 = "Grain";
    string itemname_23 = "Vegetables";
    string itemname_24 = "Mushrooms";
    string itemname_25 = "Fruit";
    string itemname_26 = "Grain";
    string itemname_27 = "Vegetables";
    string itemname_28 = "Mushrooms";
    string itemname_29 = "Algae";


    string itemname_1b = "Position0010_CookMammalMeat";
    string itemname_2b = "Position0020_CookSpiderMeat";
    string itemname_3b = "Position0030_MealPack_KelpCrisp";
    string itemname_4b = "Position0040_MealPack_FruitBar";
    string itemname_5b = "Position0050_MealPack_GardenSlaw";
    string itemname_6b = "Position0060_MealPack_RedPellets";
    string itemname_7b = "Position0070_MealPack_Chili";
    string itemname_8b = "Position0080_MealPack_Flatbread";
    string itemname_9b = "Position0090_MealPack_Ramen";
    string itemname_10b = "Position0100_MealPack_FruitPastry";
    string itemname_11b = "Position0110_MealPack_VeggieBurger";
    string itemname_12b = "Position0120_MealPack_Curry";
    string itemname_13b = "Position0130_MealPack_GreenPellets";
    string itemname_14b = "Position0140_MealPack_Dumplings";
    string itemname_15b = "Position0150_MealPack_Spaghetti";
    string itemname_16b = "Position0160_MealPack_Lasagna";
    string itemname_17b = "Position0170_MealPack_Burrito";
    string itemname_18b = "Position0180_MealPack_FrontierStew";
    string itemname_19b = "Position0190_MealPack_SearedSabiroid";
    string itemname_20b = "Position0200_MealPack_SteakDinner";
    string itemname_21b = "Position0010_Seeds_Fruit";
    string itemname_22b = "Position0020_Seeds_Grain";
    string itemname_23b = "Position0030_Seeds_Vegetables";
    string itemname_24b = "Position0040_Spores_Mushrooms";
    string itemname_25b = "Position0010_Seeds_Fruit";
    string itemname_26b = "Position0020_Seeds_Grain";
    string itemname_27b = "Position0030_Seeds_Vegetables";
    string itemname_28b = "Position0040_Spores_Mushrooms";
    string itemname_29b = "Algae";



    int state = 0;
    string spinner = "";
    List<string> complist = new List<string>();
    List<string> complist_b = new List<string>();
    List<ManagedItem> managedItems = new List<ManagedItem>();
    List<MyProductionItem> prod_list = new List<MyProductionItem>();
    List<MyProductionItem> temp_prod_list = new List<MyProductionItem>();
    List<int> queue_count = new List<int>();
    List<MyInventoryItem> item_list = new List<MyInventoryItem>();
    List<string> queue_item = new List<string>();
    List<string> barg = new List<string>();
    List<string> bargraph = new List<string>();
    bool setup_comp = false;
  //  string comp_rel = "";
    int item_count = 0;

    List<IMyTerminalBlock> display_all = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> display_tag_main = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> display_tag_2 = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> display_tag_3 = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> display_tag_4 = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> display_tag_5 = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> display_tag_6 = new List<IMyTerminalBlock>();
    List<IMyTextSurface> myTextSurfaces_d1 = new List<IMyTextSurface>();
    List<IMyTextSurface> myTextSurfaces_d2 = new List<IMyTextSurface>();
    IMyTextSurface display_surface_1;
    IMyTextSurface display_surface_2;
    IMyTextSurface display_surface_3;
    IMyTextSurface display_surface_4;
    IMyTextSurface display_surface_5;
    IMyTextSurface display_surface_6;
   // IMyProductionBlock CurrentAssembler;
 //   IMyCargoContainer currentcargocontainer;
    StringBuilder Custom_Data_Field = new StringBuilder();
    StringBuilder sbtext = new StringBuilder();
    StringBuilder sbtext2 = new StringBuilder();
    //StringBuilder sbtext3;
    List<IMyCargoContainer> cargos_all = new List<IMyCargoContainer>();
    List<IMyCargoContainer> cargos_tag = new List<IMyCargoContainer>();
    List<IMyProductionBlock> assemble_all = new List<IMyProductionBlock>();
    List<IMyProductionBlock> assemble_tag = new List<IMyProductionBlock>();
    List<IMyConveyorSorter> conveyors_all = new List<IMyConveyorSorter>();
    List<IMyConveyorSorter> conveyors_tag = new List<IMyConveyorSorter>();
    List<MyInventoryItem> items = new List<MyInventoryItem>();

    List<int> item_quota = new List<int>();
    List<int> assembler_queue_item = new List<int>();
    List<float> item_total = new List<float>();
    List<float> item_counter = new List<float>();
    List<float> queue_qty = new List<float>();
    List<string> compitem_type = new List<string>();

    IEnumerator<bool> listCoroutine;
    bool listgenerator_finished = false;
    double percent_list = 0.0;
    string runargument = "";
    string temprunargument = "";

    MyIni _ini = new MyIni();
    List<IMyMotorStator> rotors_all = new List<IMyMotorStator>();
    List<IMyMotorAdvancedStator> rotorAdvancedStators_all = new List<IMyMotorAdvancedStator>();
    List<IMyPistonBase> pistons_all = new List<IMyPistonBase>();
    IMyCubeGrid meCubeGrid;
    string dataold = "";
    string datanew = "";
    string displaytexttemp = "";
    StringBuilder sbtexttemp = new StringBuilder();
    StringBuilder nametemp = new StringBuilder();
    float percent_store_used = 0;
    float total_vol_used = 0;
    float max_total_vol = 0;
    int runTick = 0;
    string[] cachedBars = new string[22];
    private readonly string[] Spinners = { ".---", "-.--", "--.-", "---."};

    class ManagedItem
    {
        public string DisplayName;
        public string BlueprintSubtype;
        public MyItemType ItemType;
        public MyDefinitionId BlueprintId;
        public string ConfigTypeLabel;
        public int Quota;
        public float Total;
        public int QueueCount;
    }

    void PrecalculateBars()
    {
        for (int i = 0; i <= 20; i++)
            cachedBars[i] = "["+ new string('|', i) + new string('-', 20 - i) + "]";
    }

    void InitializeItems()
    {
        managedItems.Clear();
        string c = "ConsumableItem", p = "PhysicalObject", s = "SeedItem";

        // Main Food (Indices 0 - 23)
        AddItem("CookMammalMeat", "Position0010_CookMammalMeat", c);
        AddItem("CookSpiderMeat", "Position0020_CookSpiderMeat", c);
        AddItem("MealPack_KelpCrisp", "Position0030_MealPack_KelpCrisp", c);
        AddItem("MealPack_FruitBar", "Position0040_MealPack_FruitBar", c);
        AddItem("MealPack_GardenSlaw", "Position0050_MealPack_GardenSlaw", c);
        AddItem("MealPack_RedPellets", "Position0060_MealPack_RedPellets", c);
        AddItem("MealPack_Chili", "Position0070_MealPack_Chili", c);
        AddItem("MealPack_Flatbread", "Position0080_MealPack_Flatbread", c);
        AddItem("MealPack_Ramen", "Position0090_MealPack_Ramen", c);
        AddItem("MealPack_FruitPastry", "Position0100_MealPack_FruitPastry", c);
        AddItem("MealPack_VeggieBurger", "Position0110_MealPack_VeggieBurger", c);
        AddItem("MealPack_Curry", "Position0120_MealPack_Curry", c);
        AddItem("MealPack_GreenPellets", "Position0130_MealPack_GreenPellets", c);
        AddItem("MealPack_Dumplings", "Position0140_MealPack_Dumplings", c);
        AddItem("MealPack_Spaghetti", "Position0150_MealPack_Spaghetti", c);
        AddItem("MealPack_Lasagna", "Position0160_MealPack_Lasagna", c);
        AddItem("MealPack_Burrito", "Position0170_MealPack_Burrito", c);
        AddItem("MealPack_FrontierStew", "Position0180_MealPack_FrontierStew", c);
        AddItem("MealPack_SearedSabiroid", "Position0190_MealPack_SearedSabiroid", c);
        AddItem("MealPack_SteakDinner", "Position0200_MealPack_SteakDinner", c);

        // Grown/Harvested Raw Foods
        AddItem("Fruit", "Position0010_Seeds_Fruit", c);
        AddItem("Grain", "Position0020_Seeds_Grain", p);
        AddItem("Vegetables", "Position0030_Seeds_Vegetables", c);
        AddItem("Mushrooms", "Position0040_Spores_Mushrooms", c);

        // Seeds & Algae (Indices 24+)
        AddItem("Fruit", "Position0010_Seeds_Fruit", s);
        AddItem("Grain", "Position0020_Seeds_Grain", s);
        AddItem("Vegetables", "Position0030_Seeds_Vegetables", s);
        AddItem("Mushrooms", "Position0040_Spores_Mushrooms", s);
        AddItem("Algae", "Algae", p);
    }
    void AddItem(string name, string bp, string typeId)
    {
        managedItems.Add(new ManagedItem
        {
            DisplayName = name,
            BlueprintSubtype = bp,
            ItemType = new MyItemType("MyObjectBuilder_"+ typeId, name),
            BlueprintId = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/"+ bp),
            ConfigTypeLabel = typeId,
            Quota = 0,
            Total = 0,
            QueueCount = 0
        });
    }
    public void Main(string argument, UpdateType updateSource)
    {
        runTick++;
        // Check if a new argument was passed (manually via run or timer setup)
        if (!string.IsNullOrEmpty(argument))
        {
            runargument = argument;
            // --- Argument takes precedence for setup and override ---
            ParseAndApplyArguments(argument);
            Get_Custom_Data_Temp();
            if (argument != temprunargument)
            {
                update_custom_data(runargument);
            }
            else
            {
                Save();
            }

            // Force a full setup if arguments changed
            setup_comp = false;
        }

        if (!setup_comp)
        {
            setup_system();
        }


        //find cargo container, end if not found
        if (cargos_tag.Count <= 0)
        {
            Echo($"Containers with tag: '{container_tag}' not found.");
            return;
        }



        if (assemble_tag.Count <= 0)
        {
            Echo($"Assemblers with: '{container_tag}' not found.");
            return;
        }

        UpdateCustomData();

        sbtexttemp.Clear();
        sbtexttemp.Append(displaytexttemp).Append(" Food Queue Management ").Append(ver).Append(" Running...").Append('\n');
        sbtexttemp.Append("Food production #: ").Append(assemble_tag.Count).Append('\n');
        sbtexttemp.Append("Container #: ").Append(cargos_tag.Count).Append('\n');
        sbtexttemp.Append("Sorters #: ").Append(conveyors_tag.Count).Append('\n');
        if (state > 3)
        {
            state = 0;
        }

        sbtexttemp.Append(spinner).Append('\n');
        datanew = Me.CustomData;
        //reset inventory totals for array addition
            

        if (datanew != dataold)
        {
            Get_Custom_Data();
            dataold = Me.CustomData;
        }

        //coroutine list
        if (listCoroutine == null && !listgenerator_finished)
        {
            listCoroutine = Assemble_Control();
        }
        if (listCoroutine != null && !listgenerator_finished)
        {
            // Check the current yield value
            bool currentYield = listCoroutine.Current;

            // If the coroutine is finished, you can perform completion logic
            if (!listCoroutine.MoveNext())
            {
                // The coroutine has finished executing
                sbtexttemp.AppendLine("manufacture job complete.");
                listCoroutine?.Dispose();
                listCoroutine = null;
            }
            else
            {
                // Handle intermediate status if needed
                if (!currentYield)
                {
                    sbtexttemp.Append("Updating manufacture list... ").Append(Math.Round(percent_list, 1)).Append("%").Append('\n');
                    listCoroutine.MoveNext();
                }
            }

        }
        if (listgenerator_finished)
        {
            listgenerator_finished = false;
        }
        RenderDisplays();

        if (state > 3) state = 0;
        if (runTick % 2 == 0)
        {
            sbtexttemp.Clear();
            sbtexttemp.AppendLine($"AAFP Food Queue Management {ver} Running...");
            sbtexttemp.AppendLine($"Food Processor #: {assemble_tag.Count}");
            sbtexttemp.AppendLine($"Container #: {cargos_tag.Count} | Sorters #: {conveyors_tag.Count}");
            sbtexttemp.AppendLine(Spinners[state]);
            Echo(sbtexttemp.ToString());
        }
        state++;
        if(runTick > 60)
        {
            runTick = 0;
        }
    }

    private void ParseAndApplyArguments(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;

        string[] assemblertagdata = input.Split(',');

        if (assemblertagdata.Length >= 1 && !string.IsNullOrWhiteSpace(assemblertagdata[0]))
        {
            container_tag = assemblertagdata[0].Trim();
        }

        sbtexttemp.Append("Assembler Tag set to: ").Append(container_tag.Replace("[", "[[").Replace("]", "]]")).Append('\n');
    }

    void setup_system()
    {


        IMyGridTerminalSystem gts = GridTerminalSystem;
        MyDefinitionId steelPlateId = MyDefinitionId.Parse("MyObjectBuilder_BlueprintDefinition/SteelPlate");
        sbtext.Clear();
        sbtext2.Clear();
        ClearAllNonEmptyLists();
        display_all.Clear();
        display_tag_main.Clear();
        display_tag_2.Clear();
        display_tag_3.Clear();
        display_tag_4.Clear();
        display_tag_5.Clear();
        display_tag_6.Clear();
        myTextSurfaces_d1.Clear();
        myTextSurfaces_d2.Clear();
        bargraph.Clear();
        barg.Clear();
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        barg.Add("-");
        cargos_all.Clear();
        cargos_tag.Clear();
        conveyors_all.Clear();
        conveyors_tag.Clear();
        assemble_all.Clear();
        assemble_tag.Clear();
        prod_list.Clear();
        temp_prod_list.Clear();
        queue_count.Clear();
        queue_item.Clear();
        complist.Clear();
        complist_b.Clear();
        item_quota.Clear();
        item_total.Clear();
        item_counter.Clear();
        assembler_queue_item.Clear();
        queue_qty.Clear();
        item_list = new List<MyInventoryItem>();
        compitem_type.Clear();
        Custom_Data_Field = new StringBuilder();
        rotors_all = new List<IMyMotorStator>();
        rotorAdvancedStators_all = new List<IMyMotorAdvancedStator>();
        pistons_all = new List<IMyPistonBase>();
        complist.Add(itemname_1);
        complist_b.Add(itemname_1b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_2);
        complist_b.Add(itemname_2b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_3);
        complist_b.Add(itemname_3b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_4);
        complist_b.Add(itemname_4b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_5);
        complist_b.Add(itemname_5b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_6);
        complist_b.Add(itemname_6b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_7);
        complist_b.Add(itemname_7b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_8);
        complist_b.Add(itemname_8b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_9);
        complist_b.Add(itemname_9b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_10);
        complist_b.Add(itemname_10b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_11);
        complist_b.Add(itemname_11b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_12);
        complist_b.Add(itemname_12b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_13);
        complist_b.Add(itemname_13b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_14);
        complist_b.Add(itemname_14b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_15);
        complist_b.Add(itemname_15b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        bargraph.Add("");
        complist.Add(itemname_16);
        complist_b.Add(itemname_16b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_17);
        complist_b.Add(itemname_17b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_18);
        complist_b.Add(itemname_18b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_19);
        complist_b.Add(itemname_19b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_20);
        complist_b.Add(itemname_20b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_21);
        complist_b.Add(itemname_21b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_22);
        complist_b.Add(itemname_22b);
        compitem_type.Add(itemtype_3);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_23);
        complist_b.Add(itemname_23b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_24);
        complist_b.Add(itemname_24b);
        compitem_type.Add(itemtype);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_25);
        complist_b.Add(itemname_25b);
        compitem_type.Add(itemtype_2);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_26);
        complist_b.Add(itemname_26b);
        compitem_type.Add(itemtype_2);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_27);
        complist_b.Add(itemname_27b);
        compitem_type.Add(itemtype_2);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_28);
        complist_b.Add(itemname_28b);
        compitem_type.Add(itemtype_2);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        complist.Add(itemname_29);
        complist_b.Add(itemname_29b);
        compitem_type.Add(itemtype_3);
        item_quota.Add(0);
        item_total.Add(0);
        item_counter.Add(0);
        queue_count.Add(0);
        queue_qty.Add(0f);
        bargraph.Add("");
        nametemp.Clear();
        Generate_Custom_Data(runargument);
        //Me.CustomData = Custom_Data_Field.ToString();
        if (!string.IsNullOrEmpty(Storage) && !string.IsNullOrWhiteSpace(Storage))
        {
            Me.CustomData = Storage;
        }
        if (!string.IsNullOrEmpty(Me.CustomData) && !string.IsNullOrWhiteSpace(Me.CustomData))
        {
            Get_Custom_Data();
            ParseAndApplyArguments(runargument);
        }
        container_tag_applied = $"[AAFP {container_tag}]";
        lcd_display_name_applied = $"[AAFP {container_tag} {displaygroup_1} {displayhalf_1}]";
        lcd_display_name_applied_2 = $"[AAFP {container_tag} {displaygroup_1} {displayhalf_2}]";
        lcd_display_name_applied_3 = $"[AAFP {container_tag} {displaygroup_2} {displayhalf_1}]";
        lcd_display_name_applied_4 = $"[AAFP {container_tag} {displaygroup_2} {displayhalf_2}]";
        lcd_display_name_applied_5 = $"[AAFP {container_tag} {displaygroup_3} {displayhalf_1}]";
        lcd_display_name_applied_6 = $"[AAFP {container_tag} {displaygroup_3} {displayhalf_2}]";
        displaytexttemp = $"{container_tag_applied.Replace("[", "[[").Replace("]", "]]")}";
            
        //populate array with cargoblocks on grid(s) 
        bool blockfinder = false;
        gts.GetBlocksOfType<IMyMotorStator>(rotors_all, b => b.TopGrid == Me.CubeGrid);

        if (rotors_all.Count <= 0)
        {
            Echo("Rotor top grid not found, checking advanced rotors");
        }

        if (rotors_all.Count > 0)
        {
            if (rotors_all[0] != null)
            {
                meCubeGrid = rotors_all[0].CubeGrid;
                Echo("Local cubegrid found - rotor");
                blockfinder = true;
            }
        }

        gts.GetBlocksOfType<IMyMotorAdvancedStator>(rotorAdvancedStators_all, b => b.TopGrid == Me.CubeGrid);

        if (rotorAdvancedStators_all.Count <= 0)
        {
            Echo("Rotor top grid not found, checking advanced rotors");
        }
        if (rotorAdvancedStators_all.Count > 0)
        {
            if (rotorAdvancedStators_all[0] != null)
            {
                meCubeGrid = rotorAdvancedStators_all[0].CubeGrid;
                Echo("Local cubegrid found - advanced rotor/hinge");
                blockfinder = true;
            }
        }


        gts.GetBlocksOfType<IMyPistonBase>(pistons_all, b => b.TopGrid == Me.CubeGrid);

        if (pistons_all.Count <= 0)
        {
            Echo("Rotor top grid not found, checking pistons");
        }
        if (pistons_all.Count > 0)
        {
            if (pistons_all[0] != null)
            {

                meCubeGrid = pistons_all[0].CubeGrid;
                Echo("Local cubegrid found - piston");
                blockfinder = true;
            }
        }


        if (rotorAdvancedStators_all.Count == 0 && rotors_all.Count == 0 && pistons_all.Count == 0)
        {
            meCubeGrid = Me.CubeGrid;
            Echo("Local cubegrid found - PB");
            blockfinder = false;
        }

        rotors_all.Clear();
        rotorAdvancedStators_all.Clear();
        pistons_all.Clear();
        if (blockfinder)
        {
            gts.GetBlocksOfType<IMyCargoContainer>(cargos_all, b => b.CubeGrid == Me.CubeGrid);
            if (cargos_all.Count > 0)
            {
                for (int i = 0; i < cargos_all.Count; i++)
                {
                    //create new array from search array with containers matching tag
                    if (cargos_all[i].CustomName.Contains(container_tag))
                    {
                        cargos_tag.Add(cargos_all[i]);
                    }
                }

            }
            cargos_all.Clear();
        }
        gts.GetBlocksOfType<IMyCargoContainer>(cargos_all, b => b.CubeGrid == meCubeGrid);
        if (cargos_all.Count > 0)
        {
            for (int i = 0; i < cargos_all.Count; i++)
            {
                //create new array from search array with containers matching tag
                if (cargos_all[i].CustomName.Contains(container_tag))
                {
                    cargos_tag.Add(cargos_all[i]);
                }
            }

        }
        cargos_all.Clear();

        if (blockfinder)
        {
            gts.GetBlocksOfType<IMyConveyorSorter>(conveyors_all, b => b.CubeGrid == Me.CubeGrid);
            if (conveyors_all.Count > 0)
            {
                for (int i = 0; i < conveyors_all.Count; i++)
                {
                    //create new array from search array with conveyer sorters matching tag
                    if (conveyors_all[i].CustomName.Contains(container_tag))
                    {
                        conveyors_tag.Add(conveyors_all[i]);
                    }
                }

            }
            conveyors_all.Clear();
        }
        gts.GetBlocksOfType<IMyConveyorSorter>(conveyors_all, b => b.CubeGrid == meCubeGrid);
        if (conveyors_all.Count > 0)
        {
            for (int i = 0; i < conveyors_all.Count; i++)
            {
                //create new array from search array with conveyer sorters matching tag
                if (conveyors_all[i].CustomName.Contains(container_tag))
                {
                    conveyors_tag.Add(conveyors_all[i]);
                }
            }

        }
        conveyors_all.Clear();

        if (blockfinder)
        {
            gts.GetBlocksOfType<IMyProductionBlock>(assemble_all, b => b.CubeGrid == Me.CubeGrid);
            //find assemblers, end if not found
            if (assemble_all.Count > 0)
            {
                for (int i = 0; i < assemble_all.Count; i++)
                {
                    //create new array from search array with assemblers matching tag
                    if (assemble_all[i].CustomName.Contains(container_tag))
                    {
                        assemble_all[i].CustomName = $"AAFP Food Processor {container_tag_applied} {i + 1}";
                        assemble_tag.Add(assemble_all[i]);
                        assembler_queue_item.Add(0);
                    }
                }
            }
            assemble_all.Clear();
        }

        gts.GetBlocksOfType<IMyProductionBlock>(assemble_all, b => b.CubeGrid == meCubeGrid);
        //find assemblers, end if not found
        if (assemble_all.Count > 0)
        {
            for (int i = 0; i < assemble_all.Count; i++)
            {
                //create new array from search array with assemblers matching tag
                if (assemble_all[i].CustomName.Contains(container_tag))
                {
                    assemble_all[i].CustomName = $"AAFP Food Processor {container_tag_applied} {i + 1}";
                    assemble_tag.Add(assemble_all[i]);
                    assembler_queue_item.Add(0);
                }
            }
        }
        assemble_all.Clear();

        if (blockfinder)
        {
            gts.GetBlocksOfType<IMyTerminalBlock>(display_all, b => b.CubeGrid == Me.CubeGrid);
            if (display_all.Count > 0)
            {
                for (int i = 0; i < display_all.Count; i++)
                {
                    if (display_all[i].CustomName.Contains(lcd_display_name_applied))
                    {
                        display_tag_main.Add(display_all[i]);
                        myTextSurfaces_d1.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index));
                    }
                    if (display_all[i].CustomName.Contains(lcd_display_name_applied_2))
                    {
                        display_tag_2.Add(display_all[i]);
                        myTextSurfaces_d2.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index_2));
                    }
                    if (display_all[i].CustomName.Contains(lcd_display_name_applied_3))
                    {
                        display_tag_3.Add(display_all[i]);
                    }
                    if (display_all[i].CustomName.Contains(lcd_display_name_applied_4))
                    {
                        display_tag_4.Add(display_all[i]);
                    }
                    if (display_all[i].CustomName.Contains(lcd_display_name_applied_5))
                    {
                        display_tag_5.Add(display_all[i]);
                    }
                    if (display_all[i].CustomName.Contains(lcd_display_name_applied_6))
                    {
                        display_tag_6.Add(display_all[i]);
                    }
                }
            }
            display_all.Clear();
        }

        gts.GetBlocksOfType<IMyTerminalBlock>(display_all, b => b.CubeGrid == meCubeGrid);
        if (display_all.Count > 0)
        {
            for (int i = 0; i < display_all.Count; i++)
            {
                if (display_all[i].CustomName.Contains(lcd_display_name_applied))
                {
                    display_tag_main.Add(display_all[i]);
                    myTextSurfaces_d1.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index));
                }
                if (display_all[i].CustomName.Contains(lcd_display_name_applied_2))
                {
                    display_tag_2.Add(display_all[i]);
                    myTextSurfaces_d2.Add(((IMyTextSurfaceProvider)display_all[i]).GetSurface(lcd_display_index_2));
                }
                if (display_all[i].CustomName.Contains(lcd_display_name_applied_3))
                {
                    display_tag_3.Add(display_all[i]);
                }
                if (display_all[i].CustomName.Contains(lcd_display_name_applied_4))
                {
                    display_tag_4.Add(display_all[i]);
                }
                if (display_all[i].CustomName.Contains(lcd_display_name_applied_5))
                {
                    display_tag_5.Add(display_all[i]);
                }
                if (display_all[i].CustomName.Contains(lcd_display_name_applied_6))
                {
                    display_tag_6.Add(display_all[i]);
                }
            }
        }
        display_all.Clear();

        if (display_tag_main.Count > 0)
        {
            display_surface_1 = ((IMyTextSurfaceProvider)display_tag_main[0]).GetSurface(lcd_display_index);
            Echo($"LCD display: '{lcd_display_name.Replace("[", "[[").Replace("]", "]]")}' found.");
        }
        if (display_tag_2.Count > 0)
        {
            display_surface_2 = ((IMyTextSurfaceProvider)display_tag_2[0]).GetSurface(lcd_display_index_2);
            Echo($"LCD display: '{lcd_display_name_2.Replace("[", "[[").Replace("]", "]]")}' found.");
        }
        if (display_tag_3.Count > 0)
        {
            display_surface_3 = ((IMyTextSurfaceProvider)display_tag_3[0]).GetSurface(lcd_display_index_3);
            Echo($"LCD display: '{lcd_display_name_3.Replace("[", "[[").Replace("]", "]]")}' found.");
        }
        if (display_tag_4.Count > 0)
        {
            display_surface_4 = ((IMyTextSurfaceProvider)display_tag_4[0]).GetSurface(lcd_display_index_4);
            Echo($"LCD display: '{lcd_display_name_4.Replace("[", "[[").Replace("]", "]]")}' found.");
        }
        if (display_tag_5.Count > 0)
        {
            display_surface_5 = ((IMyTextSurfaceProvider)display_tag_5[0]).GetSurface(lcd_display_index_5);
            Echo($"LCD display: '{lcd_display_name_5.Replace("[", "[[").Replace("]", "]]")}' found.");
        }
        if (display_tag_6.Count > 0)
        {
            display_surface_6 = ((IMyTextSurfaceProvider)display_tag_6[0]).GetSurface(lcd_display_index_6);
            Echo($"LCD display: '{lcd_display_name_6.Replace("[", "[[").Replace("]", "]]")}' found.");
        }



        if (display_tag_main.Count <= 0 || ((IMyTextSurfaceProvider)display_tag_main[0]).GetSurface(lcd_display_index) == null)
        {
            Echo($"LCD display: '{lcd_display_name.Replace("[", "[[").Replace("]", "]]")}' not found.");
            //   return;
        }
        if (display_tag_2.Count <= 0 || ((IMyTextSurfaceProvider)display_tag_2[0]).GetSurface(lcd_display_index_2) == null)
        {
            Echo($"LCD display: '{lcd_display_name_2.Replace("[", "[[").Replace("]", "]]")}' not found.");
            //   return;
        }
        //find LCD display, end if not found
        if (display_tag_3.Count <= 0 || ((IMyTextSurfaceProvider)display_tag_3[0]).GetSurface(lcd_display_index_3) == null)
        {
            Echo($"LCD display: '{lcd_display_name_3.Replace("[", "[[").Replace("]", "]]")}' not found.");
            // return;
        }
        if (display_tag_4.Count <= 0 || ((IMyTextSurfaceProvider)display_tag_4[0]).GetSurface(lcd_display_index_4) == null)
        {
            Echo($"LCD display: '{lcd_display_name_4.Replace("[", "[[").Replace("]", "]]")}' not found.");
            // return;
        }
        if (display_tag_5.Count <= 0 || ((IMyTextSurfaceProvider)display_tag_5[0]).GetSurface(lcd_display_index_5) == null)
        {
            Echo($"LCD display: '{lcd_display_name_5.Replace("[", "[[").Replace("]", "]]")}' not found.");
            // return;
        }
        if (display_tag_6.Count <= 0 || ((IMyTextSurfaceProvider)display_tag_6[0]).GetSurface(lcd_display_index_6) == null)
        {
            Echo($"LCD display: '{lcd_display_name_6.Replace("[", "[[").Replace("]", "]]")}' not found.");
            // return;
        }
        Echo("Setup complete!");
        setup_comp = true;
        //presence check method
        if (myTextSurfaces_d1.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d1.Count; i++)
            {
                if (myTextSurfaces_d1[i] != null)
                {
                    if (myTextSurfaces_d1[i].ContentType != ContentType.TEXT_AND_IMAGE)
                    {
                        myTextSurfaces_d1[i].ContentType = ContentType.TEXT_AND_IMAGE;
                    }
                }
            }
        }
        if (myTextSurfaces_d2.Count > 0)
        {
            for (int i = 0; i < myTextSurfaces_d2.Count; i++)
            {
                if (myTextSurfaces_d2[i] != null)
                {
                    if (myTextSurfaces_d2[i].ContentType != ContentType.TEXT_AND_IMAGE)
                    {
                        myTextSurfaces_d2[i].ContentType = ContentType.TEXT_AND_IMAGE;
                    }
                }
            }
        }
        if (display_surface_1 != null)
        {
            if (display_surface_1.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                display_surface_1.ContentType = ContentType.TEXT_AND_IMAGE;
            }
        }
        if (display_surface_2 != null)
        {
            if (display_surface_2.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                display_surface_2.ContentType = ContentType.TEXT_AND_IMAGE;
            }
        }
        if (display_surface_3 != null)
        {
            if (display_surface_3.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                display_surface_3.ContentType = ContentType.TEXT_AND_IMAGE;
            }
        }
        if (display_surface_4 != null)
        {
            if (display_surface_4.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                display_surface_4.ContentType = ContentType.TEXT_AND_IMAGE;
            }
        }
        if (display_surface_5 != null)
        {
            if (display_surface_5.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                display_surface_5.ContentType = ContentType.TEXT_AND_IMAGE;
            }
        }
        if (display_surface_6 != null)
        {
            if (display_surface_6.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                display_surface_6.ContentType = ContentType.TEXT_AND_IMAGE;
            }
        }
    }
    void UpdateCustomData()
    {
        _ini.Clear();
        if (_ini.TryParse(Me.CustomData))
        {
            foreach (var item in managedItems)
            {
                string val = _ini.Get("quotamanager", $"{item.ConfigTypeLabel} {item.DisplayName}").ToString();
                int q = 0;
                int.TryParse(val, out q);
                item.Quota = q;
            }
        }
        else
        {
            _ini.Set("quotamanager", "runargument", container_tag);
            foreach (var item in managedItems)
                _ini.Set("quotamanager", $"{item.ConfigTypeLabel} {item.DisplayName}", item.Quota);
            Me.CustomData = _ini.ToString();
        }
    }

    void Generate_Custom_Data(string runargument_temp)
    {
        if (string.IsNullOrEmpty(runargument_temp) || string.IsNullOrWhiteSpace(runargument_temp))
        {
            runargument_temp = "UnamedParts";
        }

        _ini.Clear();
        _ini.Set("quotamanager", "runargument", runargument);
        if (complist.Count > 0)
        {
            for (int i = 0; i < complist.Count; i++)
            {
                _ini.Set("quotamanager", compitem_type[i]+ " "+ complist[i], item_quota[i]);

                //Custom_Data_Field.Append($"{complist[i]}:{item_quota[i]}:\n");
            }

        }
        Me.CustomData = _ini.ToString();
        _ini.Clear();

    }

    void Get_Custom_Data()
    {
        if (!string.IsNullOrEmpty(Me.CustomData) && !string.IsNullOrWhiteSpace(Me.CustomData))
        {
            var str = "";
            _ini.Clear();

            if (_ini.TryParse(Me.CustomData))
            {
                str = _ini.Get("quotamanager", "runargument").ToString().Trim();
                runargument = str;
                if (complist.Count > 0 && item_quota.Count > 0)
                {
                    for (int i = 0; i < complist.Count; i++)
                    {
                        str = _ini.Get("quotamanager", compitem_type[i] + " "+ complist[i]).ToString().Trim();
                        if (!int.TryParse(str, out item_count))
                        {
                            item_count = 0;
                        }
                        else
                        {
                            int.TryParse(str, out item_count);
                        }
                        item_quota[i] = item_count;
                    }
                }

            }
            _ini.Clear();
        }
    }

    void Get_Custom_Data_Temp()
    {
        if (!string.IsNullOrEmpty(Me.CustomData) && !string.IsNullOrWhiteSpace(Me.CustomData))
        {
            var str = "";
            _ini.Clear();

            if (_ini.TryParse(Me.CustomData))
            {
                str = _ini.Get("quotamanager", "runargument").ToString().Trim();
                temprunargument = str;
                if (complist.Count > 0 && item_quota.Count > 0)
                {
                    for (int i = 0; i < complist.Count; i++)
                    {
                        str = _ini.Get("quotamanager", compitem_type[i] + " "+ complist[i]).ToString().Trim();
                        if (!int.TryParse(str, out item_count))
                        {
                            item_count = 0;
                        }
                        else
                        {
                            int.TryParse(str, out item_count);
                        }
                        item_quota[i] = item_count;
                    }
                }

            }
            _ini.Clear();
        }
    }
    void renew_Custom_Data(string runargument_temp)
    {
        //_ini.Clear();
        _ini.Set("quotamanager", "runargument", runargument_temp);
        if (complist.Count > 0)
        {
            for (int i = 0; i < complist.Count; i++)
            {

                    _ini.Set("quotamanager", compitem_type[i] + " "+ complist[i], item_quota[i]);

                //Custom_Data_Field.Append($"{complist[i]}:{item_quota[i]}:\n");
            }

        }
        Me.CustomData = _ini.ToString();

    }

    void update_custom_data(string newarg)
    {
        if (!string.IsNullOrEmpty(Me.CustomData) && !string.IsNullOrWhiteSpace(Me.CustomData))
        {
            var str = "";
            _ini.Clear();

            if (_ini.TryParse(Me.CustomData))
            {
                if (complist.Count > 0 && item_quota.Count > 0)
                {
                    for (int i = 0; i < complist.Count; i++)
                    {
                        str = _ini.Get("quotamanager", compitem_type[i] + " "+ complist[i]).ToString().Trim();
                        if (!int.TryParse(str, out item_count))
                        {
                            item_count = 0;
                        }
                        else
                        {
                            int.TryParse(str, out item_count);
                        }
                        item_quota[i] = item_count;
                    }
                }

            }

            renew_Custom_Data(runargument);
            _ini.Clear();
            Save();
        }
    }

    IEnumerator<bool> Assemble_Control()
    {
        total_vol_used = 0; max_total_vol = 0;

        foreach (var cargo in cargos_tag)
        {
            total_vol_used += (float)cargo.GetInventory(0).CurrentVolume;
            max_total_vol += (float)cargo.GetInventory(0).MaxVolume;
        }
        percent_store_used = max_total_vol > 0 ? (total_vol_used / max_total_vol) * 100 : 0;

        for (int j = 0; j < managedItems.Count; j++)
        {
            var item = managedItems[j];
            item.Total = 0; item.QueueCount = 0;

            foreach (var assm in assemble_tag)
            {
                item.Total += (float)assm.GetInventory(1).GetItemAmount(item.ItemType);
                item.Total += (float)assm.GetInventory(0).GetItemAmount(item.ItemType);

                prod_list.Clear();
                assm.GetQueue(prod_list);
                foreach (var prod in prod_list)
                    if (prod.BlueprintId == item.BlueprintId)
                        item.QueueCount += (int)((float)prod.Amount.RawValue / 1000000f);
            }

            foreach (var sorter in conveyors_tag)
                item.Total += (float)sorter.GetInventory(0).GetItemAmount(item.ItemType);
            foreach (var cargo in cargos_tag)
                item.Total += (float)cargo.GetInventory(0).GetItemAmount(item.ItemType);

            int missing = item.Quota - ((int)item.Total + item.QueueCount);
            if (missing > 0 && assemble_tag.Count > 0)
            {
                int qty_per_assm = missing / assemble_tag.Count;
                int remainder = missing % assemble_tag.Count;

                for (int a = 0; a < assemble_tag.Count; a++)
                {
                    int toAdd = qty_per_assm + (a < remainder ? 1 : 0);
                    if (toAdd > 0)
                    {
                        assemble_tag[a].Enabled = true;
                        assemble_tag[a].AddQueueItem(item.BlueprintId, (MyFixedPoint)toAdd);
                    }
                }
            }
            else if (missing <= 0 && item.QueueCount > 0 && (int)item.Total >= item.Quota)
            {
                foreach (var assm in assemble_tag)
                    if (j < 20 || j > 23) assm.ClearQueue();
            }

            if (j % 4 == 0) yield return false;
        }

        listgenerator_finished = true;
        yield return true;
    }
    string GetBar(float val_in)
    {
        int bars = (int)(val_in * 20);
        if (bars < 0) bars = 0;
        if (bars > 20) bars = 20;
        return cachedBars[bars];
    }
    void RenderDisplays()
    {
        sbtext.Clear(); sbtext2.Clear();

        sbtext.AppendLine($"--- {container_tag} Food Inventory Status ---");
        sbtext.AppendLine("=======================================================");

        sbtext2.AppendLine($"--- {container_tag} Food Inventory Status ---");
        sbtext2.AppendLine("=================================================================");

        for (int i = 0; i < managedItems.Count; i++)
        {
            var item = managedItems[i];
            float ratio = item.Quota > 0 ? item.Total / item.Quota : 0;

            string prefix = (i >= 24 && i < 28) ? "Seed - ": "";
            string line = $"{GetBar(ratio)} | {prefix}{item.DisplayName} | Qty: {item.Total} - Quota: {item.Quota} - Queue: {item.QueueCount}\n";

            if (i < 24) sbtext.Append(line);
            else sbtext2.Append(line);
        }

        string usage = $"Storage capacity used: {Math.Round(percent_store_used)}%\n";
        sbtext.AppendLine("=======================================================").Append(usage);
        sbtext2.AppendLine("=================================================================").Append(usage);

        foreach (var surface in myTextSurfaces_d1)
        {
            if (surface.FontSize != fontzoom)
            {
                surface.FontSize = fontzoom;
            }
            if (surface.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                surface.ContentType = ContentType.TEXT_AND_IMAGE;
            }
            surface.WriteText(sbtext);
        }
        foreach (var surface in myTextSurfaces_d2)
        {
            if (surface.FontSize != fontzoom_2)
            {
                surface.FontSize = fontzoom_2;
            }
            if (surface.ContentType != ContentType.TEXT_AND_IMAGE)
            {
                surface.ContentType = ContentType.TEXT_AND_IMAGE;
            }
            surface.WriteText(sbtext2);
        }
    }
    public void ClearAllNonEmptyLists()
    {
        if (complist != null) complist.Clear();
        if (complist_b != null) complist_b.Clear();
        if (prod_list != null) prod_list.Clear();
        if (temp_prod_list != null) temp_prod_list.Clear();
        if (queue_count != null) queue_count.Clear();
        if (item_list != null) item_list.Clear();
        if (queue_item != null) queue_item.Clear();
        if (barg != null) barg.Clear();
        if (bargraph != null) bargraph.Clear();
        if (display_all != null) display_all.Clear();
        if (display_tag_main != null) display_tag_main.Clear();
        if (display_tag_2 != null) display_tag_2.Clear();
        if (display_tag_3 != null) display_tag_3.Clear();
        if (display_tag_4 != null) display_tag_4.Clear();
        if (display_tag_5 != null) display_tag_5.Clear();
        if (display_tag_6 != null) display_tag_6.Clear();
        if (cargos_all != null) cargos_all.Clear();
        if (cargos_tag != null) cargos_tag.Clear();
        if (assemble_all != null) assemble_all.Clear();
        if (assemble_tag != null) assemble_tag.Clear();
        if (conveyors_all != null) conveyors_all.Clear();
        if (conveyors_tag != null) conveyors_tag.Clear();
        if (item_quota != null) item_quota.Clear();
        if (assembler_queue_item != null) assembler_queue_item.Clear();
        if (item_total != null) item_total.Clear();
        if (item_counter != null) item_counter.Clear();
        if (queue_qty != null) queue_qty.Clear();
        if (compitem_type != null) compitem_type.Clear();
    }
