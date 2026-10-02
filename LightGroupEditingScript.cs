public void Main(string argument)
        {
            try
            {
                //----------------------------------------------------------------
                //-----------------Change only this part-------------------
                //----------------------------------------------------------------

                //Remember to name each individual light
                //So it will group them, e.g. "Light Group A" or "Light Group Alpha"

                string LIGHTstruct = "Light Group";      //Only change if you have a certain light naming style

                //Use the default colours below or change or add a custom one 

                //First, change the names of your custom colours here. Or keep them as they are. 
                //Remember the formatting and dont add a ',' (comma) to the last name.
                
                string[] customCOLOURnames =
                {
                    "Custom1",
                    "Custom2", 
                    "Custom3" 
                };

                //Format is 'new Color(R,G,B)' R = Red, G = Green, B = Blue. 
                //To make it easier test the colour in game first then add values here. 
                //Remember the first colour in the list correlates to the first custom name above in the same position. 
                //When you add more colours above you must also add more lines below.
                //Remember the formatting and dont add a ',' (comma) to the last value.

                Color[] customCOLOURvalues =
                {
                    new Color(0, 0, 0), 
                    new Color(0, 0, 0), 
                    new Color(0, 0, 0) 
                };

                //etc...

                //----------------------------------------------------------------
                //---------------DO NOT EDIT, READ ONLY ----------------
                //----------------------------------------------------------------

                //Colours that can be used
                string[] colourARRAY = {
                "Red",
                "Blue",
                "Yellow",
                "Green",
                "Orange",
                "Purple",
                "Magenta",
                "Cyan",
                "Pink",
                "Black",
                "White",
                "Cream",
                "DarkBlue",
                "LightBlue",
                "CoolBlue",
                "OceanBlue",
                "MintGreen",
                "LimeGreen",
                "DarkGreen",
                "CherryRed",
                "DarkRed",
                "BloodOrange",
                "LightPink",
                "HotPink",
                "Violet",
                "NuclearYellow",
                "Peach",
                "Brown"
                };

                //----------------------------------------------------------------
                //---------------Ignore Everything Below----------------
                //----------------------------------------------------------------

                Color[] colourCODES = {
                new Color(255, 0, 0), //Red
                new Color(0, 127, 255), //Blue
                new Color(255, 255, 0), //Yellow
                new Color(0, 127, 0), //Green
                new Color(255, 159, 0), //Orange
                new Color(127, 0, 255), //Purple
                new Color(255, 0, 255), //Magenta
                new Color(0, 255, 255), //Cyan
                new Color(255, 127, 255), //Pink
                new Color(0, 0, 0), //Black
                new Color(255, 255, 255), //White
                new Color(255, 255, 220), //Cream
                new Color(0, 0, 255), //Dark Blue
                new Color(0, 192, 255), //Light Blue
                new Color(180, 200, 255), //Cool Blue
                new Color(0, 56, 92), // Ocean Blue
                new Color(100, 150, 128), //Mint Green
                new Color(0, 255, 0), //Lime Green
                new Color(0, 35, 0), //Dark Green
                new Color(128, 0, 0), //Cherry Red
                new Color(50, 0, 0), //Dark Red
                new Color(255, 80, 0), //Blood Orange
                new Color(255, 192, 255), //Light Pink
                new Color(255, 0, 127), //Hot Pink
                new Color(159, 128, 255), //Violet
                new Color(230, 255, 0), //Nuclear Yellow
                new Color(255, 190, 159), //Peach
                new Color(75, 45, 0) //Brown
                };

                Echo(argument);

                string[] argumentCUT = argument.Split('-');
                string LIGHTgroup = argumentCUT[0];
                bool PartialName = false;
                string LIGHTfullname = LIGHTstruct + " " + LIGHTgroup;

                string colourNAME = "Unknown";
                string onORoff = "";
                int count = argumentCUT.Count();
                //Echo("Count: " + count);
                Color colour = colourCODES[10];
                float radius = -1;
                float falloff = -1;
                float intensity = -1;
                float offset = -1;
                float blinkInt = -1;
                float blinkLen = -1;
                float blinkOff = -1;
                float MaxRad = 0;
                bool containsInt = false;
                bool DiscoMode = false;
                bool reset = false;
                bool equalsN = false;
                bool onOffon = false;
                bool onOffoff = false;
                bool LIGHTfound = false;
                int LightCount = 0;

                if (count > 1)
                {
                    //DISCO Mode

                    if (argumentCUT[1] == "DISCO")
                    {
                        DiscoMode = true;
                        Echo("\nDISCO mode enabled on: " + LIGHTfullname);
                    }

                    //Reset variables

                    reset = argumentCUT[1].Equals("reset", StringComparison.OrdinalIgnoreCase);
                    //Echo("Equals reset?: " + reset);
                }

                int newColour = 0;
                float newRadius = 0;
                float newFalloff = 0;
                float newIntensity = 0;
                float newOffset = 0;
                float newBlinkInt = 0;
                float newBlinkOff = 0;
                float newBlinkLen = 0;

                //------------------

                //Echo("Starting argument counting");

                for (int i = 1; i < count; i++)
                {
                    equalsN = argumentCUT[i].Equals("n", StringComparison.OrdinalIgnoreCase);

                    if (!equalsN)
                    {
                        containsInt = argumentCUT[i].Any(char.IsDigit);
                        //Echo("Contains Digit?: " + containsInt);

                        if (i > 2 && !containsInt)
                        {
                            Echo("Incorect format used");
                            Echo("\nNo letters after the first three parts of the argument");
                            Echo("\nUse Structure:");
                            Echo("'Block Group' \n'Colour' <OR> \n'On/Off \n<OR> \n'Radius' \n'Falloff' \n'Intensity' \n'Offset' \n'Blink Interval' \n'Blink Length' \n'Blink Offset'");
                            Echo("\nFor Example: \nAlpha-Red\n<OR>\nAlpha-Red-ON<OR>\nAlpha-Red-On-1-2-3-4-5-6-7");
                            Echo("\nIf theres a setting you dont want to change use the letter 'N' or 'n' in its place. \nAlpha-Red-n-1-N-3-n-5-N-7");
                            return;
                        }
                    }
                }

                //User friendly echo's

                if (!DiscoMode)
                {
                    if (!reset)
                    {
                        if (count == 1 || (count > 3 && count < 10))
                        {
                            Echo("\nNot enough arguments. You need either 2/3 or all 10.\n");
                            Echo("\nUse Structure:");
                            Echo("\n'Block Group' \n'Colour' <OR> \n'On/Off \n<OR> \n'Radius' \n'Falloff' \n'Intensity' \n'Offset' \n'Blink Interval' \n'Blink Length' \n'Blink Offset'");
                            Echo("\nFor Example: \nAlpha-Red\n<OR>\nAlpha-Red-ON<OR>\nAlpha-Red-On-1-2-3-4-5-6-7");
                            Echo("\nIf theres a setting you dont want to change use the letter 'N' or 'n' in its place. \nAlpha-Red-n-1-N-3-n-5-N-7");
                            return;
                        }
                        else if (count > 10)
                        {
                            Echo("\nToo many arguments. Maximum 10.\nYou have used " + count);
                            Echo("\nUse Structure:");
                            Echo("\n'Block Group' \n'Colour' <OR> \n'On/Off \n<OR> \n'Radius' \n'Falloff' \n'Intensity' \n'Offset' \n'Blink Interval' \n'Blink Length' \n'Blink Offset'");
                            Echo("\nFor Example: \nAlpha-Red\n<OR>\nAlpha-Red-ON<OR>\nAlpha-Red-On-1-2-3-4-5-6-7");
                            Echo("\nIf theres a setting you dont want to change use the letter 'N' or 'n' in its place. \nAlpha-Red-n-1-N-3-n-5-N-7");
                            return;
                        }
                    }
                    else
                    {
                        if (count != 2)
                        {
                            Echo("\nNot the right amount of arguments. You need 2 to reset lights.");
                            Echo("\nUse Structure:");
                            Echo("\n'Block Group'-'RESET'");
                            Echo("\nFor Example: \nAlpha-RESET");
                            return;
                        }
                        else
                        {
                            if (LIGHTgroup.StartsWith("*"))
                            {
                                string NewLIGHTgroup = LIGHTgroup.TrimStart('*');
                                LIGHTfullname = LIGHTstruct + " " + NewLIGHTgroup;
                                Echo("\nResetting any lights with '" + NewLIGHTgroup + "' in the name to default values.");
                            }
                            else if(!(LIGHTgroup.StartsWith("*")))
                            {
                                string NewLIGHTgroup = LIGHTgroup.TrimStart('*');
                                LIGHTfullname = LIGHTstruct + " " + NewLIGHTgroup;
                                Echo("Cannot reset. '" + LIGHTfullname + "' not found.");
                            }
                            else
                            {
                                Echo("\nResetting all lights with '" + LIGHTfullname + "' to default values");
                            }
                        }
                    }
                }
                else
                {
                    if (count == 1)
                    {
                        Echo("Not enough arguments. You need either 2/3.\n");
                        Echo("\nUse Structure:");
                        Echo("'Block Group' \n'Colour' \n<OR> \n'On/Off");
                        Echo("\nFor Example: \nAlpha-DISCO\n<OR>\nAlpha-DISCO-ON");
                        Echo("\nIf theres a setting you dont want to change use the letter 'N' or 'n' in its place. \nAlpha-Red-n");
                        return;
                    }
                    else if (count > 3)
                    {
                        Echo("Too many arguments. Maximum 3.\nYou have used " + count);
                        Echo("\nUse Structure:");
                        Echo("'Block Group' \n'Colour' \n<OR> \n'On/Off");
                        Echo("\nFor Example: \nAlpha-DISCO\n<OR>\nAlpha-DISCO-ON");
                        Echo("\nIf theres a setting you dont want to change use the letter 'N' or 'n' in its place. \nAlpha-Red-n");
                        return;
                    }
                }

                //Echo("\nArgument correct length.");

                //---------------------
                //Block group selection
                //---------------------

                if (!(LIGHTgroup.StartsWith("*")))
                {
                    var blockExists = GridTerminalSystem.GetBlockWithName(LIGHTfullname) as IMyLightingBlock;

                    if (blockExists == null)
                    {
                        Echo("\nLights with name '" + LIGHTfullname + "' doesn't exist.\n(CASE SENSITIVE)");
                        return;
                    }
                }
                else if (LIGHTgroup.StartsWith("*"))
                {
                    PartialName = true;
                    //Echo("Partial Name : True");
                    string NewLIGHTgroup = LIGHTgroup.TrimStart('*');
                    LIGHTfullname = LIGHTstruct + " " + NewLIGHTgroup;
                    Echo("\nAny lights that start with: '" + LIGHTstruct + "' and include: '" + NewLIGHTgroup + "' will be selected.");
                }
                else
                {
                    if (!(reset == true))
                    {
                        LIGHTfullname = LIGHTstruct + " " + LIGHTgroup;
                        Echo("\nLights with name '" + LIGHTfullname + "' selected");
                    }
                }

                //---------------------
                //Light on/off status
                //---------------------

                if (count > 2)
                {
                    onORoff = argumentCUT[2];
                    onOffon = argumentCUT[2].Equals("on", StringComparison.OrdinalIgnoreCase);
                    onOffoff = argumentCUT[2].Equals("off", StringComparison.OrdinalIgnoreCase);
                }

                if (!reset)
                {
                    if (count > 2)
                    {
                        if (onOffon)
                        {
                            Echo("\nLight status: " + onORoff);
                        }
                        else if (onOffoff)
                        {
                            Echo("\nLight status: " + onORoff);
                        }
                        else if (equalsN)
                        {
                            Echo("\nLight on/off status unchanged");
                        }
                        else
                        {
                            Echo("\nUnknown characters used for on/off status");
                        }
                    }

                    if (DiscoMode == false)
                    {
                        //---------------------
                        //  Colour selection
                        //---------------------

                        colourNAME = argumentCUT[1];

                        if (!(argumentCUT[1].Equals("n", StringComparison.OrdinalIgnoreCase)))
                        {
                            int colourPOS = 10;
                            if (!(colourARRAY.Contains(colourNAME, StringComparer.OrdinalIgnoreCase) || customCOLOURnames.Contains(colourNAME, StringComparer.OrdinalIgnoreCase)))
                            {
                                Echo("\nColour not found (Invalid Name)");
                                Echo("Consider a custom colour?");
                                Echo("\nColours to chose from:");
                                Echo("Red\nBlue\nYellow\nGreen\nOrange\nPurple\nMagenta\nCyan\nPink\nBlack\nWhite\nCream\nDarkBlue\nLightBlue\nCoolBlue\nOceanBlue\nMintGreen\nLimeGreen\nDarkGreen\nCherryRed\nDarkRed\nBloodOrange\nLightPink\nHotPink\nViolet\nNuclearYellow\nPeach\nBrown");
                                return;
                            }
                            else if(colourARRAY.Contains(colourNAME, StringComparer.OrdinalIgnoreCase))
                            {
                                for(int v = 0; v < colourARRAY.Count(); v++)
                                {
                                    if(colourARRAY[v].Equals(colourNAME, StringComparison.OrdinalIgnoreCase))
                                    {
                                        //Echo("Loop finised at: " + v);
                                        colourPOS = v;
                                        colour = colourCODES[colourPOS];
                                        Echo("\nColour '" + colourARRAY[colourPOS] + "' selected");
                                        Echo("Colour value: " + colour);
                                        break;
                                    }
                                }
                                //colourPOS = Array.IndexOf(colourARRAY, colourNAME);
                            }
                            else if(customCOLOURnames.Contains(colourNAME, StringComparer.OrdinalIgnoreCase))
                            {
                                for (int v = 0; v < customCOLOURvalues.Count(); v++)
                                {
                                    if (customCOLOURnames[v].Equals(colourNAME, StringComparison.OrdinalIgnoreCase))
                                    {
                                        //Echo("Custom loop finised at: " + v);
                                        colourPOS = v;
                                        colour = customCOLOURvalues[colourPOS];
                                        Echo("\nCustom colour '" + customCOLOURnames[colourPOS] + "' selected");
                                        Echo("Custom colour value: " + colour);
                                        break;
                                    }
                                }
                                //colourPOS = Array.IndexOf(colourARRAY, colourNAME);
                            }
                        }

                        if (count > 3)
                        {
                            //---------------------
                            //  Radius selection
                            //---------------------

                            string radiusStr = argumentCUT[3];

                            if (!(radiusStr.Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                radius = float.Parse(radiusStr);
                            }

                            if (radiusStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                            {
                                Echo("\nNo radius setting found...");
                            }
                            else if (radius < 1 || radius > 10)
                            {
                                Echo("\nRadius has to be between 1 and 10, NOT: " + radius);
                                Echo("-------Radius NOT set-------");
                                radius = -1;
                            }
                            else
                            {
                                Echo("\nRadius '" + radius + "' selected");
                            }

                            //---------------------
                            //  Falloff selection
                            //---------------------

                            string falloffStr = argumentCUT[4];

                            if (!(falloffStr.Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                falloff = float.Parse(falloffStr);
                            }

                            if (falloffStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                            {
                                Echo("\nNo falloff setting found...");
                            }
                            else if (falloff < 0 || falloff > 2)
                            {
                                Echo("\nFalloff has to be between 0 and 2, NOT: " + falloff);
                                Echo("-------Falloff NOT set-------");
                                falloff = -1;
                            }
                            else
                            {
                                Echo("\nFalloff '" + falloff + "' selected");
                            }

                            //---------------------
                            // Intensity selection
                            //---------------------

                            string intensityStr = argumentCUT[5];

                            if (!(intensityStr.Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                intensity = float.Parse(intensityStr);
                            }

                            if (intensityStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                            {
                                Echo("\nNo intensity setting found...");
                            }
                            else if (intensity < 0.5 || intensity > 5)
                            {
                                Echo("\nIntensity has to be between 0.5 and 5, NOT: " + intensity);
                                Echo("-------Intensity NOT set-------");
                                intensity = -1;
                            }
                            else
                            {
                                Echo("\nIntensity '" + intensity + "' selected");
                            }

                            //---------------------
                            //  Offset selection
                            //---------------------

                            string offsetStr = argumentCUT[6];

                            if (!(offsetStr.Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                offset = float.Parse(offsetStr);
                            }

                            if (offsetStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                            {
                                Echo("\nNo offset setting found...");
                            }
                            else if (offset < 0 || offset > 5)
                            {
                                Echo("\nOffset has to be between 0 and 5, NOT: " + offset);
                                Echo("-------Offset NOT set-------");
                                offset = -1;
                            }
                            else
                            {
                                Echo("\nOffset '" + offset + "' selected");
                            }

                            //---------------------
                            //Blink interval selection
                            //---------------------

                            string blinkIntStr = argumentCUT[7];

                            if (!(blinkIntStr.Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                blinkInt = float.Parse(blinkIntStr);
                            }

                            if (blinkIntStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                            {
                                Echo("\nNo blink interval setting found...");
                            }
                            else if (blinkInt < 0 || blinkInt > 30)
                            {
                                Echo("\nBlink interval has to be between 0 and 30 seconds, NOT: " + blinkInt);
                                Echo("----Blink interval NOT set----");
                                blinkInt = -1;
                            }
                            else
                            {
                                Echo("\nBlink interval '" + blinkInt + "' selected");
                            }

                            //---------------------
                            //Blink Length selection
                            //---------------------

                            string blinkLenStr = argumentCUT[8];

                            if (!(blinkLenStr.Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                blinkLen = float.Parse(blinkLenStr);
                            }

                            if (blinkLenStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                            {
                                Echo("\nNo blink length setting found...");
                            }
                            else if (blinkLen < 0 || blinkLen > 100)
                            {
                                Echo("\nBlink length has to be between 0% and 100%, NOT: " + blinkLen);
                                Echo("----Blink length NOT set----");
                                blinkLen = -1;
                            }
                            else
                            {
                                Echo("\nBlink length '" + blinkLen + "' selected");
                            }

                            //---------------------
                            //Blink Offset selection
                            //---------------------

                            string blinkOffStr = argumentCUT[9];

                            if (!(blinkOffStr.Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                blinkOff = float.Parse(blinkOffStr);
                            }

                            if (blinkOffStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                            {
                                Echo("\nNo blink offset setting found...\n");
                            }
                            else if (blinkOff < 0 || blinkOff > 100)
                            {
                                Echo("\nBlink offset has to be between 0% and 100%, NOT: " + blinkOff);
                                Echo("----Blink offset NOT set----");
                                blinkOff = -1;
                            }
                            else
                            {
                                Echo("\nBlink offset '" + blinkOff + "' selected\n");
                            }
                        }
                        else
                        {
                            Echo("\nNo other parameters found.");
                            Echo("Continuing script...\n");
                        }
                    }
                    else
                    {
                        Echo("\nRandom values will be calculated.\n");
                    }
                }

                //Echo("\nArgument is correct.");
                //Echo("Light Name: " + LIGHTfullname);

                List<IMyTerminalBlock> LIGHTBLOCK_list = new List<IMyTerminalBlock>();
                GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(LIGHTBLOCK_list);

                for (int j = 0; j < LIGHTBLOCK_list.Count; j++)
                {
                    string LightType = "N/A";
                    float NewRad = 0;
                    float NewFall = 0;
                    float NewInt = 0;

                    if (!(PartialName == true))
                    {
                        if (LIGHTBLOCK_list[j].CustomName == LIGHTfullname)
                        {
                            if (DiscoMode)
                            {
                                newColour = (int)RandomNum(0, 0, 27);
                                colour = colourCODES[newColour];
                                colourNAME = colourARRAY[newColour];
                                //Echo("Colour: " + colourNAME);
                                newRadius = RandomNum(2, 1, 10);
                                radius = newRadius;
                                //Echo("radius: " + radius);
                                newFalloff = RandomNum(2, 1, 2);
                                falloff = newFalloff;
                                //Echo("falloff: " + falloff);
                                newIntensity = RandomNum(2, 2, 5);
                                intensity = newIntensity;
                                //Echo("intensity: " + intensity);
                                newOffset = RandomNum(2, 0, 5);
                                offset = newOffset;
                                //Echo("offset: " + offset);
                                newBlinkInt = RandomNum(2, 2, 5);
                                blinkInt = newBlinkInt;
                                //Echo("blinkInt: " + blinkInt);
                                newBlinkLen = RandomNum(2, 1, 5);
                                blinkLen = newBlinkLen;
                                if (blinkLen < 2.8 && blinkLen > 0)
                                {
                                    blinkLen = (float)2.8;
                                }
                                //Echo("blinkLen: " + blinkLen);
                                newBlinkOff = RandomNum(2, 1, 100);
                                blinkOff = newBlinkOff;
                                if (blinkOff < 51.3 && blinkOff > 50)
                                {
                                    blinkOff = (float)51.3;
                                }
                                //Echo("blinkOff: " + blinkOff);

                                LIGHTBLOCK_list[j].SetValue("OnOff", true);
                            }

                            MaxRad = LIGHTBLOCK_list[j].GetMaximum<float>("Radius");
                            //Echo("MaxRad: " + MaxRad);

                            if (!reset)
                            {
                                if (onOffon)
                                {
                                    LIGHTBLOCK_list[j].SetValue("OnOff", true);
                                    //Echo("Light '" + (j + 1) + "' turned " + onORoff);
                                }
                                else if (onOffoff)
                                {
                                    LIGHTBLOCK_list[j].SetValue("OnOff", false);
                                    //Echo("Light '" + (j + 1) + "' turned " + onORoff);
                                }
                            }
                            else
                            {
                                colour = colourCODES[10];
                                LIGHTBLOCK_list[j].SetValue("OnOff", true);
                                NewRad = LIGHTBLOCK_list[j].GetDefaultValue<float>("Radius");
                                //Echo("ResetRad: " + NewRad);
                                NewFall = LIGHTBLOCK_list[j].GetDefaultValue<float>("Falloff");
                                NewInt = LIGHTBLOCK_list[j].GetDefaultValue<float>("Intensity");
                                offset = LIGHTBLOCK_list[j].GetDefaultValue<float>("Offset");
                                blinkInt = 0;
                                blinkLen = 0;
                                blinkOff = 0;
                            }

                            if (MaxRad == 10)
                            {
                                LightType = "Corner Light";
                                if (!reset)
                                {
                                    NewRad = radius;
                                    NewFall = falloff;
                                    NewInt = intensity * 2;
                                }
                            }
                            else if (MaxRad == 20)
                            {
                                LightType = "Interior Light";
                                if (!reset)
                                {
                                    NewRad = radius * 2;
                                    NewFall = falloff * (float)1.5;
                                    NewInt = intensity * 2;
                                }
                            }
                            else if (MaxRad == 30)
                            {
                                LightType = "Rotating Light";
                                if (!reset)
                                {
                                    NewRad = radius * 3;
                                    NewInt = intensity * 2;
                                }
                            }
                            else if (MaxRad == 160)
                            {
                                LightType = "Spotlight";
                                if (!reset)
                                {
                                    NewRad = radius * 16;
                                    NewFall = 0;
                                    NewInt = intensity;
                                }
                            }
                            else
                            {
                                LightType = "Unknown";
                            }

                            if (!(argumentCUT[1].Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                LIGHTBLOCK_list[j].SetValue("Color", colour);
                                //Echo("Set Colour of '" + LIGHTgroup + "' " + j);
                            }

                            if (count > 3 || DiscoMode || reset)
                            {
                                //Echo("Setting Lights");
                                if (NewRad >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Radius", NewRad);
                                    //Echo("Set Radius of " + j);
                                }
                                if (NewFall >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Falloff", NewFall);
                                    //Echo("Set Falloff of " + j);
                                }
                                if (NewInt >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Intensity", NewInt);
                                    //Echo("Set Intensity of " + j);
                                }
                                if (offset >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Offset", offset);
                                    //Echo("Set Offset of " + j);
                                }
                                if (blinkInt >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Blink Interval", blinkInt);
                                    //Echo("Set Blink Interval of " + j);
                                }
                                if (blinkLen >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Blink Lenght", blinkLen);
                                    //Echo("Set Blink Length of " + j);
                                }
                                if (blinkOff >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Blink Offset", blinkOff);
                                    //Echo("Set Blink Offset of " + j);
                                }
                            }
                            LightCount++;
                            Echo("Set '" + LIGHTgroup + "' Light: " + (LightCount) + "' (" + LightType + ")");
                        }
                    }
                    //----------------------------------------------------
                    //If we're looking for any light with the partial name
                    //----------------------------------------------------
                    else
                    {
                        string ThisLight = LIGHTBLOCK_list[j].CustomName;
                        //Echo("Custom Name: " + ThisLight);
                        string NewLIGHTgroup = LIGHTgroup.TrimStart('*');
                        //Echo("NewLIGHTGroup: " + NewLIGHTgroup);
                        //Echo("LIGHTstruct: " + LIGHTstruct);

                        if (ThisLight.StartsWith(LIGHTstruct) && ThisLight.Contains(NewLIGHTgroup))
                        {
                            LIGHTfound = true;
                            //Echo("Setting Light");
                            if (DiscoMode)
                            {
                                newColour = (int)RandomNum(0, 0, 27);
                                colour = colourCODES[newColour];
                                colourNAME = colourARRAY[newColour];
                                //Echo("Colour: " + colourNAME);
                                newRadius = RandomNum(2, 1, 10);
                                radius = newRadius;
                                //Echo("radius: " + radius);
                                newFalloff = RandomNum(2, 1, 2);
                                falloff = newFalloff;
                                //Echo("falloff: " + falloff);
                                newIntensity = RandomNum(2, 2, 5);
                                intensity = newIntensity;
                                //Echo("intensity: " + intensity);
                                newOffset = RandomNum(2, 0, 5);
                                offset = newOffset;
                                //Echo("offset: " + offset);
                                newBlinkInt = RandomNum(2, 2, 5);
                                blinkInt = newBlinkInt;
                                //Echo("blinkInt: " + blinkInt);
                                newBlinkLen = RandomNum(2, 1, 5);
                                blinkLen = newBlinkLen;
                                if (blinkLen < 2.8 && blinkLen > 0)
                                {
                                    blinkLen = (float)2.8;
                                }
                                //Echo("blinkLen: " + blinkLen);
                                newBlinkOff = RandomNum(2, 1, 100);
                                blinkOff = newBlinkOff;
                                if (blinkOff < 51.3 && blinkOff > 50)
                                {
                                    blinkOff = (float)51.3;
                                }
                                //Echo("blinkOff: " + blinkOff);

                                LIGHTBLOCK_list[j].SetValue("OnOff", true);
                            }

                            MaxRad = LIGHTBLOCK_list[j].GetMaximum<float>("Radius");
                            //Echo("MaxRad: " + MaxRad);

                            if (!reset)
                            {
                                if (onOffon)
                                {
                                    LIGHTBLOCK_list[j].SetValue("OnOff", true);
                                    //Echo("Light '" + (j + 1) + "' turned " + onORoff);
                                }
                                else if (onOffoff)
                                {
                                    LIGHTBLOCK_list[j].SetValue("OnOff", false);
                                    //Echo("Light '" + (j + 1) + "' turned " + onORoff);
                                }
                            }
                            else
                            {
                                colour = colourCODES[10];
                                LIGHTBLOCK_list[j].SetValue("OnOff", true);
                                NewRad = LIGHTBLOCK_list[j].GetDefaultValue<float>("Radius");
                                //Echo("ResetRad: " + NewRad);
                                NewFall = LIGHTBLOCK_list[j].GetDefaultValue<float>("Falloff");
                                NewInt = LIGHTBLOCK_list[j].GetDefaultValue<float>("Intensity");
                                offset = LIGHTBLOCK_list[j].GetDefaultValue<float>("Offset");
                                blinkInt = 0;
                                blinkLen = 0;
                                blinkOff = 0;
                            }

                            if (MaxRad == 10)
                            {
                                LightType = "Corner Light";
                                if (!reset)
                                {
                                    NewRad = radius;
                                    NewFall = falloff;
                                    NewInt = intensity * 2;
                                }
                            }
                            else if (MaxRad == 20)
                            {
                                LightType = "Interior Light";
                                if (!reset)
                                {
                                    NewRad = radius * 2;
                                    NewFall = falloff * (float)1.5;
                                    NewInt = intensity * 2;
                                }
                            }
                            else if (MaxRad == 30)
                            {
                                LightType = "Rotating Light";
                                if (!reset)
                                {
                                    NewRad = radius * 3;
                                    NewInt = intensity * 2;
                                }
                            }
                            else if (MaxRad == 160)
                            {
                                LightType = "Spotlight";
                                if (!reset)
                                {
                                    NewRad = radius * 16;
                                    NewFall = 0;
                                    NewInt = intensity;
                                }
                            }
                            else
                            {
                                LightType = "Unknown";
                            }

                            if (!(argumentCUT[1].Equals("n", StringComparison.OrdinalIgnoreCase)))
                            {
                                LIGHTBLOCK_list[j].SetValue("Color", colour);
                                //Echo("Set Colour of '" + LIGHTgroup + "' " + j);
                            }

                            if (count > 3 || DiscoMode || reset)
                            {
                                //Echo("Setting Lights");
                                if (NewRad >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Radius", NewRad);
                                    //Echo("Set Radius of " + j);
                                }
                                if (NewFall >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Falloff", NewFall);
                                    //Echo("Set Falloff of " + j);
                                }
                                if (NewInt >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Intensity", NewInt);
                                    //Echo("Set Intensity of " + j);
                                }
                                if (offset >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Offset", offset);
                                    //Echo("Set Offset of " + j);
                                }
                                if (blinkInt >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Blink Interval", blinkInt);
                                    //Echo("Set Blink Interval of " + j);
                                }
                                if (blinkLen >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Blink Lenght", blinkLen);
                                    //Echo("Set Blink Length of " + j);
                                }
                                if (blinkOff >= 0)
                                {
                                    LIGHTBLOCK_list[j].SetValue("Blink Offset", blinkOff);
                                    //Echo("Set Blink Offset of " + j);
                                }
                            }
                            LightCount++;
                            Echo("Set '" + ThisLight + "'. (" + LightType + ")");
                        }
                    }
                }

                if (LIGHTfound == false)
                {
                    Echo("\nNo lights with '" + LIGHTgroup + "' in the name have been found.");
                    return;
                }
                else
                {
                    Echo("\nLights have been successfully configured!");
                }

            }
            catch (Exception ex)
            {
                Echo("\nAn error has occured, please contact Aleliabro.\nTake note of what you did and steps to take, thanks!.\nProgram terminated.");
                return;
            }
        }

        Random random = new Random();

        public float RandomNum(int decimalPlace, int min, int max)
        {
            if (decimalPlace > 0)
            {
                float firstNum = random.Next(min, max);
                double lastNumdo = random.NextDouble();
                decimal lastNumde = decimal.Round((decimal)lastNumdo, decimalPlace);
                float lastNum = (float)lastNumde;
                float returnNum = firstNum + lastNum;
                return returnNum;
            }
            else
            {
                float returnNum = random.Next(min, max);
                return returnNum;
            }
            
        }
        //END OF SCRIPT