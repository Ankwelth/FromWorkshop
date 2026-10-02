// R e a d m e
// -----------
// 
//         Simple air vent controls by Adomus V2.9
//       
// 

        /*
        Simple air vent controls by Adomus
        */


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Save()
        {
        }

string optional_name_ref_tag = "[Intrepid]";
string airlock_light_name_main = "[Intrepid Rear Hab Main]";
string airlock_light_name_front = "[Intrepid Rear Hab Front]";
string airlock_light_name_rear = "[Intrepid Rear Hab Rear]";
string airlock_vent_name_main = "[Intrepid Rear Hab Vent]";
string sensor_name_front = "[Intrepid Rear Hab Front]";
string sensor_name_rear = "[Intrepid Rear Hab Rear]";
string door_name_front = "[Intrepid Rear Hab Front]";
string door_name_rear = "[Intrepid Rear Hab Rear]";
string reference_gas_tank_name = "[Intrepid Vent Tank Ref 1]";
string tank_full_light_name = "[Intrepid Rear Hab Override]";

        bool En_sensor_front = true;
        bool En_sensor_rear = true;
        bool En_door_front = true;
        bool En_door_rear = true;
        bool En_light_main = true;
        bool En_light_front = true;
        bool En_light_rear = true;
        bool En_Interlock = true;
        bool En_Override_controls = true;
        bool En_Tank_full_light = true;

        float safe_oxygen = 0.05f;
        float o_low = 0.01f;
        float o_med = 0.50f;
        float o_hi = 0.90f;
        double tank_full_ratio = 0.99;

        bool airlockmode = true;

        string ver = "V2.9";

        IMyLightingBlock airlocklight_actual_main;
        IMyLightingBlock airlocklight_actual_front;
        IMyLightingBlock airlocklight_actual_rear;
        IMyLightingBlock airlocklight_tank_actual_full;
        
        IMyAirVent airlock_vent_actual;

        IMyGasTank gas_tank_ref_actual;

        Color Colorone = new Color(0, 255, 0);
        Color Colortwo = new Color(255, 255, 0);
        Color Colorthree = new Color(255, 0, 0);

        float oxygen_val = 0.0f;

        List<IMyLightingBlock> lighting_all;
        List<IMyLightingBlock> lighting_tag_main;
        List<IMyLightingBlock> lighting_tag_front;
        List<IMyLightingBlock> lighting_tag_rear;
        List<IMyLightingBlock> lighting_tag_full;

        List<IMyAirVent> vents_all;
        List<IMyAirVent> vents_tag;        
        List<IMySensorBlock> sensors_all;
        List<IMySensorBlock> sensors_tag_front;
        List<IMySensorBlock> sensors_tag_rear;
        List<IMyDoor> doors_all;
        List<IMyDoor> doors_tag_front;
        List<IMyDoor> doors_tag_rear;

        List<IMyGasTank> gas_tank_all;
        List<IMyGasTank> gas_tank_ref_tag;
        
        List<bool> closedcnt_front;
        List<bool> closedcnt_rear;
        List<bool> closingcnt_front;
        List<bool> closingcnt_rear;
        List<bool> openingcnt_front;
        List<bool> openingcnt_rear;
        bool setup_complete = false;

        bool is_working = false;
        bool is_overriding = false;
        int closed_count_front = 0;
        int closed_count_rear = 0;
        int closing_count_front = 0;
        int closing_count_rear = 0;
        int opening_count_front = 0;
        int opening_count_rear = 0;



        public void Main(string argument, UpdateType updateSource)
        {
            IMyGridTerminalSystem gts = GridTerminalSystem as IMyGridTerminalSystem;

            if (!setup_complete)
            {
                lighting_all = new List<IMyLightingBlock>();
                lighting_tag_main = new List<IMyLightingBlock>();
                lighting_tag_front = new List<IMyLightingBlock>();
                lighting_tag_rear = new List<IMyLightingBlock>();
                if (En_Tank_full_light)
                {
                    lighting_tag_full = new List<IMyLightingBlock>();
                }
                gts.GetBlocksOfType<IMyLightingBlock>(lighting_all);
                if (lighting_all.Count > 0) {
                    for (int i = 0; i < lighting_all.Count; i++)
                    {
                        if (lighting_all[i].CustomName.Contains(airlock_light_name_main))
                        {
                            if (!lighting_all[i].CustomName.Equals($"Airlock Door Light {optional_name_ref_tag} {airlock_light_name_main}"))
                            {
                                lighting_all[i].CustomName = $"Airlock Door Light {optional_name_ref_tag} {airlock_light_name_main}";
                            }
                            lighting_tag_main.Add(lighting_all[i]);
                        }
                        if (En_light_front)
                        {
                            if (lighting_all[i].CustomName.Contains(airlock_light_name_front))
                            {
                                if (!lighting_all[i].CustomName.Equals($"Airlock Door Light {optional_name_ref_tag} {airlock_light_name_front}"))
                                {
                                    lighting_all[i].CustomName = $"Airlock Door Light {optional_name_ref_tag} {airlock_light_name_front}";
                                }
                                lighting_tag_front.Add(lighting_all[i]);
                            }
                        }
                        if (En_light_rear)
                        {
                            if (lighting_all[i].CustomName.Contains(airlock_light_name_rear))
                            {
                                if (!lighting_all[i].CustomName.Equals($"Airlock Door Light {optional_name_ref_tag} {airlock_light_name_rear}"))
                                {
                                    lighting_all[i].CustomName = $"Airlock Door Light {optional_name_ref_tag} {airlock_light_name_rear}";
                                }
                                lighting_tag_rear.Add(lighting_all[i]);
                            }
                        }
                        if (En_Tank_full_light)
                        {
                            if (lighting_all[i].CustomName.Contains(tank_full_light_name))
                            {
                                if (!lighting_all[i].CustomName.Equals($"Tank Full Control Light {optional_name_ref_tag} {tank_full_light_name}"))
                                {
                                    lighting_all[i].CustomName = $"Tank Full Control Light {optional_name_ref_tag} {tank_full_light_name}";
                                }
                                lighting_tag_full.Add(lighting_all[i]);
                            }
                        }
                    }
                }
                lighting_all.Clear();
                vents_all = new List<IMyAirVent>();
                vents_tag = new List<IMyAirVent>();
                gts.GetBlocksOfType<IMyAirVent>(vents_all);
                if (vents_all.Count > 0)
                {
                    for (int i = 0; i < vents_all.Count; i++)
                    {
                        if (vents_all[i].CustomName.Contains(airlock_vent_name_main))
                        {
                            if (!vents_all[i].CustomName.Equals($"Airlock Vent {optional_name_ref_tag} {airlock_vent_name_main}"))
                            {
                                vents_all[i].CustomName = $"Airlock Vent {optional_name_ref_tag} {airlock_vent_name_main}";
                            }
                            vents_tag.Add(vents_all[i]);
                        }
                    }
                }
                vents_all.Clear();
                if (En_door_front || En_door_rear)
                {
                    doors_all = new List<IMyDoor>();
                    doors_tag_front = new List<IMyDoor>();
                    doors_tag_rear = new List<IMyDoor>();
                    closedcnt_front = new List<bool>();
                    closedcnt_rear = new List<bool>();
                    closingcnt_front = new List<bool>();
                    closingcnt_rear = new List<bool>();
                    openingcnt_front = new List<bool>();
                    openingcnt_rear = new List<bool>();

                    gts.GetBlocksOfType<IMyDoor>(doors_all);
                    if (doors_all.Count > 0)
                    {
                        for (int i = 0; i < doors_all.Count; i++)
                        {
                            if (En_door_front)
                            {
                                if (doors_all[i].CustomName.Contains(door_name_front) == true)
                                {
                                    if (!doors_all[i].CustomName.Equals($"Airlock Door {optional_name_ref_tag} {door_name_front}"))
                                    {
                                        doors_all[i].CustomName = $"Airlock Door {optional_name_ref_tag} {door_name_front}";
                                    }
                                    doors_tag_front.Add(doors_all[i]);
                                    closedcnt_front.Add(false);
                                    closingcnt_front.Add(false);
                                    openingcnt_front.Add(false);
                                }
                            }
                            if (En_door_rear)
                            {
                                if (doors_all[i].CustomName.Contains(door_name_rear) == true)
                                {
                                    if (!doors_all[i].CustomName.Equals($"Airlock Door {optional_name_ref_tag} {door_name_rear}"))
                                    {
                                        doors_all[i].CustomName = $"Airlock Door {optional_name_ref_tag} {door_name_rear}";
                                    }
                                    doors_tag_rear.Add(doors_all[i]);
                                    closedcnt_rear.Add(false);
                                    closingcnt_rear.Add(false);
                                    openingcnt_rear.Add(false);
                                }
                            }
                        }
                    }                  
                }
                doors_all.Clear();
                if (En_sensor_front || En_sensor_rear)
                {
                    sensors_all = new List<IMySensorBlock>();
                    sensors_tag_front = new List<IMySensorBlock>();
                    sensors_tag_rear = new List<IMySensorBlock>();
                    gts.GetBlocksOfType<IMySensorBlock>(sensors_all);
                    if (sensors_all.Count > 0)
                    {
                        for (int i = 0; i < sensors_all.Count; i++)
                        {
                            if (En_sensor_front)
                            {
                                if (sensors_all[i].CustomName.Contains(sensor_name_front) == true)
                                {
                                    if (!sensors_all[i].CustomName.Equals($"Airlock Sensor {optional_name_ref_tag} {sensor_name_front}"))
                                    {
                                        sensors_all[i].CustomName = $"Airlock Sensor {optional_name_ref_tag} {sensor_name_front}";
                                    }
                                    sensors_tag_front.Add(sensors_all[i]);
                                }
                            }
                            if (En_sensor_rear)
                            {
                                if (sensors_all[i].CustomName.Contains(sensor_name_rear) == true)
                                {
                                    if (!sensors_all[i].CustomName.Equals($"Airlock Sensor {optional_name_ref_tag} {sensor_name_rear}"))
                                    {
                                        sensors_all[i].CustomName = $"Airlock Sensor {optional_name_ref_tag} {sensor_name_rear}";
                                    }
                                    sensors_tag_rear.Add(sensors_all[i]);
                                }
                            }
                        }
                    }
                }
                sensors_all.Clear();
                if (En_Override_controls)
                {
                    gas_tank_all = new List<IMyGasTank>();
                    gas_tank_ref_tag = new List<IMyGasTank>();
                    gts.GetBlocksOfType<IMyGasTank>(gas_tank_all);
                    if (gas_tank_all.Count > 0)
                    {
                        for (int i = 0; i < gas_tank_all.Count; i++)
                        {
                            if (gas_tank_all[i].CustomName.Contains(reference_gas_tank_name))
                            {
                                gas_tank_ref_tag.Add(gas_tank_all[i]);
                            }
                        }
                    }
                }                
                gas_tank_all.Clear();
                setup_complete = true;
                Echo("Setup complete!");
            }

            if (lighting_tag_main.Count <= 0 && En_light_main)
            {
                Echo($"Indication light main block with tag: {airlock_light_name_main} not found");
                return;
            }
            if (En_light_main)
            {
                airlocklight_actual_main = lighting_tag_main[0];
            }

            if (lighting_tag_front.Count <= 0 && En_light_front)
            {
                Echo($"Indication light front block with tag: {airlock_light_name_front} not found");
                return;
            }
            if (En_light_front)
            {
                airlocklight_actual_front = lighting_tag_front[0];
            }
            if (lighting_tag_rear.Count <= 0 && En_light_rear)
            {
                Echo($"Indication light rear block with tag: {airlock_light_name_rear} not found");
                return;
            }
            if (En_light_rear)
            {
                airlocklight_actual_rear = lighting_tag_rear[0];
            }
            if (En_Tank_full_light)
            {
                if (lighting_tag_full.Count <= 0)
                {
                    Echo($"Tank full indicaition light block with tag: {tank_full_light_name} not found");
                    return;
                }

                if (lighting_tag_full.Count > 0)
                {
                    airlocklight_tank_actual_full = lighting_tag_full[0];
                }
            }

            if (vents_tag.Count <= 0 || vents_tag[0] == null)
            {
                Echo($"Air vent block with tag: {airlock_vent_name_main} not found");
                return;
            }
            if (vents_tag.Count > 0)
            {
                airlock_vent_actual = vents_tag[0];
            }
            if (En_door_front)
            {
                if (doors_tag_front.Count <= 0)
                {
                    Echo($"Door front block with tag: {door_name_front} not found");
                    return;
                }
            }
            if (En_door_rear)
            {
                if (doors_tag_rear.Count <= 0)
                {
                    Echo($"Door rear block with tag: {door_name_rear} not found");
                    return;
                }
            }
            if (En_sensor_front)
            {
                if (sensors_tag_front.Count <= 0)
                {
                    Echo($"Sesnsor front block with tag: {sensor_name_front} not found");
                    return;
                }
            }

            if (En_sensor_rear)
            {
                if (sensors_tag_rear.Count <= 0)
                {
                    Echo($"Sesnsor rear block with tag: {sensor_name_rear} not found");
                    return;
                }
            }
            if (En_Override_controls)
            {
                if (gas_tank_ref_tag.Count <= 0)
                {
                    Echo($"Exhaust control reference gas tank block with tag: {reference_gas_tank_name} not found");
                    return;
                }

                if (gas_tank_ref_tag.Count > 0)
                {
                    gas_tank_ref_actual = gas_tank_ref_tag[0];
                }
            }
            //vent logic
            if (vents_tag.Count > 0 && airlock_vent_actual != null)
            {
                oxygen_val = airlock_vent_actual.GetOxygenLevel();
                if (airlockmode && !airlock_vent_actual.Depressurize)
                {
                    airlock_vent_actual.Depressurize = true;
                }
                if (!airlockmode && airlock_vent_actual.Depressurize)
                {
                    airlock_vent_actual.Depressurize = false;
                }
            }
            if (vents_tag.Count > 0)
            {
                Echo($"Adomus' Airlock Safe Script: {ver}");
                Echo($"Running vent control: {optional_name_ref_tag} {airlock_vent_name_main} ...");
                Echo($"Airlock mode: {airlockmode}");
                Echo($"Airvent: {airlock_vent_name_main} - O2 Level: {oxygen_val.ToString()}");
                Echo($"Interlock Enabled: {En_Interlock} - Running: {is_working}");
                Echo($"Override Control Enabled: {En_Override_controls} - Override: {is_overriding}");
                if (En_Override_controls)
                {
                    Echo($"Tank reference level: {Math.Round(gas_tank_ref_actual.FilledRatio,2)}");
                    Echo($"Tank full level: {Math.Round(tank_full_ratio, 2)}");
                }
                if (En_Tank_full_light)
                {
                    Echo($"Tank full: {airlocklight_tank_actual_full.Enabled}");
                }
                if (En_sensor_front)
                {
                    Echo($"Sensor front: {sensor_name_front} : {sensors_tag_front[0].IsActive}");
                }
                if (En_door_front)
                {
                    Echo($"Doors front: {door_name_front} - Closed: #{closed_count_front} - O/N:#{opening_count_front} - C/N:#{closing_count_front}");
                }
                if (En_sensor_rear)
                {
                    Echo($"Sensor rear: {sensor_name_rear} : {sensors_tag_rear[0].IsActive}");
                }
                if (En_door_front)
                {
                    Echo($"Doors rear: {door_name_rear} - Closed: #{closed_count_rear} - O/N:#{opening_count_rear} - C/N:#{closing_count_rear}");
                }
            }
//            Echo("Halpu");

            if (oxygen_val < safe_oxygen && airlockmode && !(closing_count_front > 0 || closing_count_rear > 0 || opening_count_front > 0 || opening_count_rear > 0))
            {
                if (En_light_main)
                {
                    airlocklight_actual_main.SetValue("Color", Colorone);
                    airlocklight_actual_main.Enabled = true;
                }                
                if (En_light_front)
                {
                    airlocklight_actual_front.SetValue("Color", Colorone);
                    airlocklight_actual_front.Enabled = true;
                }
                if (En_light_rear)
                {
                    airlocklight_actual_rear.SetValue("Color", Colorone);
                    airlocklight_actual_rear.Enabled = true;
                }
                is_working = false;
                if (En_Override_controls)
                {
                    is_overriding = false;
                    if (En_Tank_full_light)
                    {
                        airlocklight_tank_actual_full.Enabled = false;
                    }
                }
                if (!En_Override_controls)
                {
                    is_overriding = false;
                }
            }

            if (oxygen_val >= safe_oxygen && airlockmode || (closing_count_front > 0 || closing_count_rear > 0 || opening_count_front > 0 || opening_count_rear > 0))
            {
                if (En_light_main)
                {
                    airlocklight_actual_main.SetValue("Color", Colorthree);
                    airlocklight_actual_main.Enabled = true;
                }
                if (En_light_front)
                {
                    airlocklight_actual_front.SetValue("Color", Colorthree);
                    airlocklight_actual_front.Enabled = true;
                }
                if (En_light_rear)
                {
                    airlocklight_actual_rear.SetValue("Color", Colorthree);
                    airlocklight_actual_rear.Enabled = true;
                }
                is_working = true;

                if (En_Override_controls)
                {
                    if (gas_tank_ref_actual != null)
                    {
                        if (Math.Round(gas_tank_ref_actual.FilledRatio, 2) >= Math.Round(tank_full_ratio, 2) && oxygen_val >= safe_oxygen && airlockmode)
                        {
                            if (En_Tank_full_light && airlocklight_tank_actual_full != null)
                            {
                                airlocklight_tank_actual_full.Enabled = true;
                            }
                            is_overriding = true;
                        }
                        if (Math.Round(gas_tank_ref_actual.FilledRatio, 2) < Math.Round(tank_full_ratio, 2) && oxygen_val >= safe_oxygen && airlockmode)
                        {
                            if (En_Tank_full_light && airlocklight_tank_actual_full != null)
                            {
                                airlocklight_tank_actual_full.Enabled = false;
                            }
                            is_overriding = false;
                        }
                    }
                    if (gas_tank_ref_actual == null)
                    {
                        is_overriding = false;
                        if (En_Tank_full_light && airlocklight_tank_actual_full != null)
                        {
                            airlocklight_tank_actual_full.Enabled = false;
                        }
                    }
                    if (!En_Override_controls)
                    {
                        is_overriding = false;
                    }
                }

            }            
            if (!airlock_vent_actual.Enabled && airlockmode)
            {
                airlocklight_actual_main.Enabled = false;
                is_working = false;
            }

            if (oxygen_val <= o_low && !airlockmode)
            {
                if (En_light_main)
                {
                    airlocklight_actual_main.SetValue("Color", Colorthree);
                    airlocklight_actual_main.Enabled = true;
                }
                if (En_light_front)
                {
                    airlocklight_actual_front.SetValue("Color", Colorthree);
                    airlocklight_actual_front.Enabled = true;
                }
                if (En_light_rear)
                {
                    airlocklight_actual_rear.SetValue("Color", Colorthree);
                    airlocklight_actual_rear.Enabled = true;
                }
                is_working = true;
            }
            if (oxygen_val > o_low && oxygen_val <= o_med && !airlockmode)
            {
                if (En_light_main)
                {
                    airlocklight_actual_main.SetValue("Color", Colortwo);
                    airlocklight_actual_main.Enabled = true;
                }
                if (En_light_front)
                {
                    airlocklight_actual_front.SetValue("Color", Colortwo);
                    airlocklight_actual_front.Enabled = true;
                }
                if (En_light_rear)
                {
                    airlocklight_actual_rear.SetValue("Color", Colortwo);
                    airlocklight_actual_rear.Enabled = true;
                }
                is_working = false;
            }
            if (oxygen_val > o_med && oxygen_val < o_hi && !airlockmode)
            {
                if (En_light_main)
                {
                    airlocklight_actual_main.SetValue("Color", Colortwo);
                    airlocklight_actual_main.Enabled = true;
                }
                if (En_light_front)
                {
                    airlocklight_actual_front.SetValue("Color", Colortwo);
                    airlocklight_actual_front.Enabled = true;
                }
                if (En_light_rear)
                {
                    airlocklight_actual_rear.SetValue("Color", Colortwo);
                    airlocklight_actual_rear.Enabled = true;
                }
                is_working = false;
            }

            if (oxygen_val >= o_hi && !airlockmode)
            {
                if (En_light_main)
                {
                    airlocklight_actual_main.SetValue("Color", Colorone);
                    airlocklight_actual_main.Enabled = true;
                }
                if (En_light_front)
                {
                    airlocklight_actual_front.SetValue("Color", Colorone);
                    airlocklight_actual_front.Enabled = true;
                }
                if (En_light_rear)
                {
                    airlocklight_actual_rear.SetValue("Color", Colorone);
                    airlocklight_actual_rear.Enabled = true;
                }
                is_working = false;
            }
            if (!airlock_vent_actual.Enabled && airlockmode)
            {
                airlocklight_actual_main.Enabled = true;
                is_working = false;
            }


            //sensor logic 1 (door 1)
            if (En_sensor_front && En_door_front)
            {
                if (En_Interlock)
                {
                    for (int i = 0; i < doors_tag_front.Count; i++)
                    {
                        if (doors_tag_front[i] != null)
                        {
                            closedcnt_front[i] = doors_tag_front[i].Status == DoorStatus.Closed;
                            closingcnt_front[i] = doors_tag_front[i].Status == DoorStatus.Closing;
                            openingcnt_front[i] = doors_tag_front[i].Status == DoorStatus.Opening;
                        }
                    }
                    closed_count_front = CntTrueVls(closedcnt_front);
                    closing_count_front = CntTrueVls(closingcnt_front);
                    opening_count_front = CntTrueVls(openingcnt_front);
                    if (is_working && sensors_tag_front[0].IsActive && !is_overriding)
                    {
                        if (closed_count_front < doors_tag_front.Count)
                        {

                            for (int i = 0; i < doors_tag_front.Count; i++)
                            {
                                if (doors_tag_front[i] != null)
                                {
                                    if (doors_tag_front[i].Status != DoorStatus.Closed && doors_tag_front[i].Status != DoorStatus.Closing)
                                    {
                                        doors_tag_front[i].CloseDoor();
                                    }

                                }
                            }
                        }
                        for (int i = 0; i < doors_tag_front.Count; i++)
                        {
                            if (doors_tag_front[i] != null)
                            {
                                closedcnt_front[i] = doors_tag_front[i].Status == DoorStatus.Closed;
                                closingcnt_front[i] = doors_tag_front[i].Status == DoorStatus.Closing;
                                openingcnt_front[i] = doors_tag_front[i].Status == DoorStatus.Opening;
                            }
                        }
                        closed_count_front = CntTrueVls(closedcnt_front);
                        closing_count_front = CntTrueVls(closingcnt_front);
                        opening_count_front = CntTrueVls(openingcnt_front);

                        if (closed_count_front >= doors_tag_front.Count)
                        {
                            for (int i = 0; i < doors_tag_front.Count; i++)
                            {
                                if (doors_tag_front[i] != null)
                                {
                                    if (doors_tag_front[i].Enabled && !is_overriding)
                                    {
                                        doors_tag_front[i].Enabled = false;
                                    }
                                }
                            }
                        }
                    }

                    if (sensors_tag_front[0].IsActive && !is_working || sensors_tag_front[0].IsActive && is_working && is_overriding)
                    {

                        for (int i = 0; i < doors_tag_front.Count; i++)
                        {
                            if (doors_tag_front[i] != null)
                            {
                                if (!doors_tag_front[i].Enabled)
                                {
                                    doors_tag_front[i].Enabled = true;
                                }
                                if (doors_tag_front[i].Status != DoorStatus.Open && doors_tag_front[i].Status != DoorStatus.Opening)
                                {
                                    doors_tag_front[i].OpenDoor();
                                }
                            }
                        }
                    }

                    if (!sensors_tag_front[0].IsActive)
                    {
                        for (int i = 0; i < doors_tag_front.Count; i++)
                        {
                            if (doors_tag_front[i] != null)
                            {
                                if (!doors_tag_front[i].Enabled)
                                {
                                    doors_tag_front[i].Enabled = true;
                                }
                                if (doors_tag_front[i].Status != DoorStatus.Closed && doors_tag_front[i].Status != DoorStatus.Closing)
                                {
                                    doors_tag_front[i].CloseDoor();
                                }
                            }
                        }
                    }
                }
                //nointerlock
                if (!sensors_tag_front[0].IsActive && !En_Interlock)
                {
                    for (int i = 0; i < doors_tag_front.Count; i++)
                    {
                        if (doors_tag_front[i] != null)
                        {
                            if (!doors_tag_front[i].Enabled)
                            {
                                doors_tag_front[i].Enabled = true;
                            }
                            if (doors_tag_front[i].Status != DoorStatus.Open && doors_tag_front[i].Status != DoorStatus.Opening)
                            {
                                doors_tag_front[i].OpenDoor();
                            }
                        }
                    }
                }

                if (sensors_tag_front[0].IsActive && !En_Interlock)
                {
                    for (int i = 0; i < doors_tag_front.Count; i++)
                    {
                        if (doors_tag_front[i] != null)
                        {
                            if (!doors_tag_front[i].Enabled)
                            {
                                doors_tag_front[i].Enabled = true;
                            }
                            if (doors_tag_front[i].Closed)
                            {
                                doors_tag_front[i].OpenDoor();
                            }
                        }
                    }
                }
            }            

            //sensor logic 2 (door 2)
            if (En_sensor_rear && En_door_rear)
            {
                if (En_Interlock)
                {
                    for (int i = 0; i < doors_tag_rear.Count; i++)
                    {
                        if (doors_tag_rear[i] != null)
                        {
                            closedcnt_rear[i] = doors_tag_rear[i].Status == DoorStatus.Closed;
                            closingcnt_rear[i] = doors_tag_rear[i].Status == DoorStatus.Closing;
                            openingcnt_rear[i] = doors_tag_rear[i].Status == DoorStatus.Opening;
                        }
                    }
                    closed_count_rear = CntTrueVls(closedcnt_rear);
                    closing_count_rear = CntTrueVls(closingcnt_rear);
                    opening_count_rear = CntTrueVls(openingcnt_rear);                    
                    if (is_working && sensors_tag_rear[0].IsActive && !is_overriding)
                    {
                        if (closed_count_rear < doors_tag_rear.Count)
                        {
                            for (int i = 0; i < doors_tag_rear.Count; i++)
                            {
                                if (doors_tag_rear[i] != null)
                                {
                                    if (doors_tag_rear[i].Status != DoorStatus.Closed && doors_tag_rear[i].Status != DoorStatus.Closing)
                                    {
                                        doors_tag_rear[i].CloseDoor();
                                    }
                                }
                            }
                        }                        
                        for (int i = 0; i < doors_tag_rear.Count; i++)
                        {
                            if (doors_tag_rear[i] != null)
                            {
                                closedcnt_rear[i] = doors_tag_rear[i].Status == DoorStatus.Closed;
                                closingcnt_rear[i] = doors_tag_rear[i].Status == DoorStatus.Closing;
                                openingcnt_rear[i] = doors_tag_rear[i].Status == DoorStatus.Opening;
                            }
                        }                       
                        closed_count_rear = CntTrueVls(closedcnt_rear);
                        closing_count_rear = CntTrueVls(closingcnt_rear);
                        opening_count_rear = CntTrueVls(openingcnt_rear);                        
                        if (closed_count_rear >= doors_tag_rear.Count)
                        {
                            for (int i = 0; i < doors_tag_rear.Count; i++)
                            {
                                if (doors_tag_rear[i] != null)
                                {
                                    if (doors_tag_rear[i].Enabled && !is_overriding)
                                    {
                                        doors_tag_rear[i].Enabled = false;
                                    }
                                }
                            }
                        }
                    }
                    
                    if (sensors_tag_rear[0].IsActive && !is_working || sensors_tag_rear[0].IsActive && is_working && is_overriding)
                    {

                        for (int i = 0; i < doors_tag_rear.Count; i++)
                        {
                            if (doors_tag_rear[i] != null)
                            {
                                if (!doors_tag_rear[i].Enabled)
                                {
                                    doors_tag_rear[i].Enabled = true;
                                }
                                if (doors_tag_rear[i].Status != DoorStatus.Open && doors_tag_rear[i].Status != DoorStatus.Opening)
                                {
                                    doors_tag_rear[i].OpenDoor();
                                }
                            }
                        }
                    }

                    if (!sensors_tag_rear[0].IsActive)
                    {
                        for (int i = 0; i < doors_tag_rear.Count; i++)
                        {
                            if (doors_tag_rear[i] != null)
                            {
                                if (!doors_tag_rear[i].Enabled)
                                {
                                    doors_tag_rear[i].Enabled = true;
                                }
                                if (doors_tag_rear[i].Status != DoorStatus.Closed && doors_tag_rear[i].Status != DoorStatus.Closing)
                                {
                                    doors_tag_rear[i].CloseDoor();
                                }
                            }
                        }
                    }
                }
                //nointerlock
                if (!sensors_tag_rear[0].IsActive && !En_Interlock)
                {
                    for (int i = 0; i < doors_tag_rear.Count; i++)
                    {
                        if (doors_tag_rear[i] != null)
                        {
                            if (!doors_tag_rear[i].Enabled)
                            {
                                doors_tag_rear[i].Enabled = true;
                            }
                            if (doors_tag_rear[i].Status != DoorStatus.Open && doors_tag_rear[i].Status != DoorStatus.Opening)
                            {
                                doors_tag_rear[i].OpenDoor();
                            }
                        }
                    }
                }

                if (sensors_tag_rear[0].IsActive && !En_Interlock)
                {
                    for (int i = 0; i < doors_tag_rear.Count; i++)
                    {
                        if (doors_tag_rear[i] != null)
                        {
                            if (!doors_tag_rear[i].Enabled)
                            {
                                doors_tag_rear[i].Enabled = true;
                            }
                            if (doors_tag_rear[i].Closed)
                            {
                                doors_tag_rear[i].OpenDoor();
                            }
                        }
                    }
                }
            }            


        }

        int CntTrueVls(List<bool> list)
        {
            int truCnt = 0;

            foreach (bool value in list)
            {
                if (value)
                {
                    truCnt++;
                }
            }
            return truCnt;
        }
