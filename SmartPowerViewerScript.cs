//_______Settings_____________________________________________________________________________________________________________________________________________

        // false(hide list but show weak ouput power block); true(show list of the power block)
        bool Give_Wind_Turbine_Detailed_List = false;
        bool Give_Solar_Panel_Detailed_List = false;
        bool Give_Batteries_Detailed_List = true;
        bool Give_Reactor_Detailed_List = false;

        //Change the name of the LCD_Panel where the information will be displayed #Note don't forget to change the name LCD_Panel block and set it to Text and Images
        string LCD_Panel_Name = "Screen";

        //_____________________________________________________________________________________________________________________________________________________________

        public Program()
        {
           
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

        }

        public float TokW(float x)
        {
            return x * 1000;
        }

        public float ToMW(float x)
        {
            return x / 1000;
        }

        public double bsp(double x, double y)
        {
            double awns = Math.Round((x / y) * 100, 2);
            return awns;
        }

        public static string PowerSign(float x)
        {
            string y = "";
            string awns = "";
            if (x >= 1000)
            {
                y = " MW";
                x = x / 1000;
 
            }
            else if (x < 1000 && x >= 1)
            {
                y = " kW";
              
            }
            else if (x < 1)
            {

                y = " W";
                x = x * 1000;

            }

            awns = Math.Round(x,2) + y;
            return awns;
        }
            
        public void Main(string argument, UpdateType updateSource)
        {
                
                List<IMyBatteryBlock> _BPower = new List<IMyBatteryBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(_BPower);

                List<IMySolarPanel> _SPower = new List<IMySolarPanel>();
                GridTerminalSystem.GetBlocksOfType<IMySolarPanel>(_SPower);

                List<IMyReactor> _RPower = new List<IMyReactor>();
                GridTerminalSystem.GetBlocksOfType<IMyReactor>(_RPower);

                IMyTextPanel _TextPanel = GridTerminalSystem.GetBlockWithName(LCD_Panel_Name) as IMyTextPanel;
               
                string a = "";
                string b = "";
                string s = "";
                string r = "";

                float S_output = 0f;
                float W_output = 0f;
                float R_output = 0f;
            float B_output = 0f, B_input = 0f;
                
                //Bateries
                if (_BPower.Count > 0)
                {
                    for (int e = 0; e < _BPower.Count; e++)
                    {
                        
                        B_output += _BPower[e].CurrentOutput;
                        B_input += _BPower[e].CurrentInput;
                    if (Give_Batteries_Detailed_List == true)
                    {
                        b += _BPower[e].DisplayNameText + ": " + "(Output) " + PowerSign(TokW(_BPower[e].CurrentOutput)) + ": " + "(Input) " + PowerSign(TokW(_BPower[e].CurrentInput)) + ": (" + bsp(_BPower[e].CurrentStoredPower, _BPower[e].MaxStoredPower) + "% stored) " + PowerSign(TokW(_BPower[e].CurrentStoredPower)) + "\n";
                    }

                    }
                }
                else { b = "No Batteries found"; }

                //Solar Panel

                if (_SPower.Count > 0)
                {
                    for (int k = 0; k < _SPower.Count; k++)
                    {

                        S_output += _SPower[k].CurrentOutput;
                    if (Give_Solar_Panel_Detailed_List == false)
                    {
                        if (TokW(_SPower[k].CurrentOutput) < 1 || TokW(_SPower[k].CurrentOutput) == 0)
                        {
                            s += "Weak output detected: " + (_SPower[k].DisplayNameText + ": " + PowerSign(TokW(_SPower[k].CurrentOutput))) + "\n";
                        }

                        if (S_output == 0)
                        {
                            s = "Night time ? No output found !";
                        }                     
                    }
                    else { s +=(_SPower[k].DisplayNameText + ": " + PowerSign(TokW(_SPower[k].CurrentOutput))) + "\n"; }
                    }
                }
                else { s = "No Solar Panel found"; }

                //Reactor
                if (_RPower.Count > 0)
                {
                    for (int q = 0; q < _RPower.Count; q++)
                    {
                    if (Give_Reactor_Detailed_List == false)
                    {
                        R_output += _RPower[q].CurrentOutput;
                        if (TokW(_RPower[q].CurrentOutput) < 1)
                        {
                            r += "Weak output detected: " + _RPower[q].DisplayNameText + ": " + PowerSign(TokW(_RPower[q].CurrentOutput)) + "\n";
                        }

                        if (R_output == 0)
                        {
                            r = "No output found!";
                        }
                    }
                    else { r += _RPower[q].DisplayNameText + ": " + PowerSign(TokW(_RPower[q].CurrentOutput)) + "\n"; }
                }
                }
                else { r = "No Reactor found"; }

            try
            {
                List<IMyPowerProducer> _Power = new List<IMyPowerProducer>();
                IMyBlockGroup _wGroup = GridTerminalSystem.GetBlockGroupWithName("Wind Turbines");
                _wGroup.GetBlocksOfType(_Power);

                if (_Power.Count > 0)
                {
                    for (int i = 0; i < _Power.Count; i++)
                    {
                        W_output += _Power[i].CurrentOutput;
                        if (Give_Wind_Turbine_Detailed_List == false)
                        {
                            if (TokW(_Power[i].CurrentOutput) < 1)
                            {
                                a += "Weak output detected: " + (_Power[i].DisplayNameText + ": " + PowerSign(TokW(_Power[i].CurrentOutput)))  + "\n";
                            }
                            if (W_output == 0)
                            {
                                a = "No output found!";
                            }
                        }
                        else { a += (_Power[i].DisplayNameText + ": " + PowerSign(TokW(_Power[i].CurrentOutput))) + "\n"; }
                        //windT = +_Power[i].CurrentOutput;
                    }
                }
                else { a = "No Wind Turbine found"; }
                Echo(B_output.ToString());
                _TextPanel.WriteText("(" + _Power.Count + ")Wind Tubine Total Output: " + PowerSign(TokW(W_output)) + ":" + "\n" + a + "\n" + "(" + _BPower.Count + ")Batteries Total Output: " + PowerSign(TokW(B_output)) +": (Input) " + PowerSign(TokW(B_input)) +"\n" + b + "\n" + "(" + _SPower.Count + ")Solar Panel Total Output: " + PowerSign(TokW(S_output)) + "\n" + s + "\n" + "(" + _RPower.Count + ")Reactors Total Output: " + PowerSign(TokW(R_output)) + "\n" + r);
            }
            catch
            {
                Echo("Put your wind turbines in a group called (Wind Turbines)");
                _TextPanel.WriteText("Wind Tubine not found " + ":" + "\n" + a + "\n" + "(" + _BPower.Count + ")Batteries Total Output: " + PowerSign(TokW(B_output)) + ": (Input) " + PowerSign(TokW(B_input)) + "\n" + b + "\n" + "(" + _SPower.Count + ")Solar Panel Total Output: " + PowerSign(TokW(S_output)) + "\n" + s + "\n" + "(" + _RPower.Count + ")Reactors Total Output: " + PowerSign(TokW(R_output)) + "\n" + r);
            }

        }
