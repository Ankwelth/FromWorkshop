        /*
        * Simple air vent controls V1.2 by Adomus
        * 
        */


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
        }
        
        string airlock_light_name = "[AL EL]";
        string airlock_vent_name = "[AL EL]";
        float safe_oxygen = 0.05f;
        float o_low = 0.10f;
        float o_med = 0.50f;
        float o_hi = 0.90f;
        bool airlockmode = true;
        string ver = "V1.2";

        IMyLightingBlock airlocklight_actual;
        IMyAirVent airlock_vent_actual;

        Color Colorone = new Color(0, 255, 0);
        Color Colortwo = new Color(255, 255, 0);
        Color Colorthree = new Color(255, 0, 0);

        float oxygen_val = 0.0f;
        string f0 = "OnOff_Off";
        string f1 = "OnOff_On";

        List<IMyLightingBlock> lighting_all;
        List<IMyLightingBlock> lighting_tag;
        List<IMyAirVent> vents_all;
        List<IMyAirVent> vents_tag;
        bool setup_complete = false;



        public void Main(string argument, UpdateType updateSource)
        {
            IMyGridTerminalSystem gts = GridTerminalSystem as IMyGridTerminalSystem;

            if (!setup_complete)
            {
                lighting_all = new List<IMyLightingBlock>();
                lighting_tag = new List<IMyLightingBlock>();
                gts.GetBlocksOfType<IMyLightingBlock>(lighting_all);
                for (int i = 0; i < lighting_all.Count; i++)
                {
                    if (lighting_all[i].CustomName.Contains(airlock_light_name) == true)
                    {
                        lighting_tag.Add(lighting_all[i]);
                    }
                }
                vents_all = new List<IMyAirVent>();
                vents_tag = new List<IMyAirVent>();
                gts.GetBlocksOfType<IMyAirVent>(vents_all);
                for (int i = 0; i < vents_all.Count; i++)
                {
                    if (vents_all[i].CustomName.Contains(airlock_vent_name) == true)
                    {
                        vents_tag.Add(vents_all[i]);
                    }
                }
                setup_complete = true;
                Echo("Setup complete!");
            }

            if (lighting_tag.Count <= 0|| lighting_tag[0]==null)
            {
                Echo($"light block with tag: {airlock_light_name} not found");
                return;
            }
            airlocklight_actual = lighting_tag[0];


            if (vents_tag.Count <= 0|| vents_tag[0]==null)
            {
                Echo($"vent block with tag: {airlock_vent_name} not found");
                return;
            }
            airlock_vent_actual = vents_tag[0];
            if (vents_tag.Count > 0)
            {
                oxygen_val = airlock_vent_actual.GetOxygenLevel();
            }
            if (vents_tag.Count > 0)
            {
                Echo($"Adomus' Airlock Safe Script: {ver}");
                Echo($"Running vent control: {airlock_vent_name}...");
                Echo($"O2 Level: {oxygen_val.ToString()}");
            }
            if (oxygen_val < safe_oxygen && airlockmode)
            {
                airlocklight_actual.SetValue("Color", Colorone);
                airlocklight_actual.ApplyAction(f1);
            }
            if (oxygen_val >= safe_oxygen && airlockmode)
            {
                airlocklight_actual.SetValue("Color", Colorthree);
                airlocklight_actual.ApplyAction(f1);
            }
            if (!airlock_vent_actual.Enabled && airlockmode)
            {
                airlocklight_actual.ApplyAction(f0);
            }

            if (oxygen_val <= o_low && !airlockmode)
            {
                airlocklight_actual.SetValue("Color", Colorthree);
                airlocklight_actual.ApplyAction(f1);
            }
            if (oxygen_val > o_low && oxygen_val <= o_med && !airlockmode)
            {
                airlocklight_actual.SetValue("Color", Colortwo);
                airlocklight_actual.ApplyAction(f1);
            }
            if (oxygen_val > o_med && oxygen_val < o_hi && !airlockmode)
            {
                airlocklight_actual.SetValue("Color", Colortwo);
                airlocklight_actual.ApplyAction(f1);
            }

            if (oxygen_val >= o_hi && !airlockmode)
            {
                airlocklight_actual.SetValue("Color", Colorone);
                airlocklight_actual.ApplyAction(f1);
            }
            if (!airlock_vent_actual.Enabled && airlockmode)
            {
                airlocklight_actual.ApplyAction(f0);
            }

        }