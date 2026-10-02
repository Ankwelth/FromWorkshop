string groupconnectors = "LightCon";

        // Code

        IMyBlockGroup g;
        List<IMyShipConnector> connectors = new List<IMyShipConnector>();
        List<IMyInteriorLight> lights = new List<IMyInteriorLight>();
        public Program()
        {

            g = GridTerminalSystem.GetBlockGroupWithName(groupconnectors) as IMyBlockGroup;
            List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
            g.GetBlocksOfType(connectors);
            g.GetBlocksOfType(lights);
            Echo(connectors.Count.ToString()+"\n"+lights.Count.ToString());
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }


        public void Save()
        {
            
        }

        public void Main(string argument, UpdateType updateSource)
        {
            foreach (IMyShipConnector c in connectors)
            {
                string code = "";
                string name = c.CustomName;
                
               
                for (int i = 0; i < name.Count(); i++)
                {
                    
                    if (name[i].ToString()==" ")
                    {
                        break;
                    }
                    
                        code = code + name[i].ToString();
                   
                }
                Echo(code);
               
                       foreach (IMyInteriorLight light in lights)
                        {
                    Echo(light.CustomName);
                            bool tc = true;
                            if (light.CustomName.Count() < code.Count()) { break; }
                            for(int i=0; i< code.Count(); i++)
                            {
                                
                                if (!code[i].Equals(light.CustomName[i]))
                                {
                            Echo("Breaking: " + light.CustomName[i].ToString());
                                    tc = false;
                                    break;
                                }

                            }
                          Echo(light.CustomName.ToString()+": "+tc.ToString());
                            if (tc)
                            {

                                if (c.Status == MyShipConnectorStatus.Connected)
                                {
                                    light.SetValue<Color>("Color",Color.Green);
                                    light.BlinkIntervalSeconds = 0f;
                                }
                                if(c.Status == MyShipConnectorStatus.Unconnected)
                                {
                                    light.SetValue<Color>("Color", Color.Orange);
                                    light.BlinkIntervalSeconds = 1f;
                                }
                                if (c.Status == MyShipConnectorStatus.Connectable)
                                {
                                    light.SetValue<Color>("Color", Color.Red);
                                    light.BlinkIntervalSeconds = .5f;
                                }
                            }
                        }
                       
            }

        }