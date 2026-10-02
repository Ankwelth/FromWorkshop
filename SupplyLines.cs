 /*  -= Supply Lines =- */

        /* WHAT IS THIS?
         * see the description on the Steam Workshop https://steamcommunity.com/sharedfiles/filedetails/?id=2869938767
         */

        public static readonly string scriptname = "Supply Lines";
        public static readonly string VERSION = "vTC1.0.2";
        DateTime updateTime = DateTime.Now;
        int updateTimespan = 1000;
        string workingIndicator = "/";
        int workingCounter = 0;
        List<IMyTerminalBlock> storage = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> temp = new List<IMyTerminalBlock>();
        IMyTextPanel sldirectlcd;
        bool isStop = false;
        bool havelcd = false;
        StringBuilder linebuilder = new StringBuilder();
        MyIni _ini = new MyIni();
        string DefaultDIRECTValues = "0+0";
        RectangleF _viewport;
        IMyTextSurface _drawingSurface;
        List<MySprite> sprites = new List<MySprite>();
        Vector2 position;
        List<IMyConveyorSorter> sorters = new List<IMyConveyorSorter>();
        List<MyInventoryItemFilter> filterlist = new List<MyInventoryItemFilter>();

        public Program()
        {
            MyIniParseResult result;
            if (!_ini.TryParse(Me.CustomData, out result))
                throw new Exception(result.ToString());
            createDefs();
            SLSetup();
            if (havelcd)
            {
                _drawingSurface = sldirectlcd;
                _viewport = new RectangleF(
                    (_drawingSurface.TextureSize - _drawingSurface.SurfaceSize) / 2f,
                    _drawingSurface.SurfaceSize
                );
                PrepareTextSurfaceForSprites(_drawingSurface);
            }
            Me.CustomData = _ini.ToString();
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }
        void UpdateWorkingCounter()
        {
            if ((DateTime.Now - updateTime).TotalMilliseconds > updateTimespan)
            {
                updateTime = DateTime.Now;
                if (workingCounter >= 3)
                {
                    workingCounter = 0;
                }
                else
                {
                    workingCounter++;
                }
            }
            switch (workingCounter % 4)
            {
                case 0: workingIndicator = "/"; break;
                case 1: workingIndicator = "-"; break;
                case 2: workingIndicator = "\\"; break;
                case 3: workingIndicator = "|"; break;
            }
        }
        void Main(string arg, UpdateType updateSource)
        {
            UpdateWorkingCounter();
            Echo(scriptname + " Version " + VERSION + " " + workingIndicator + "\n\n");

            if (updateSource.HasFlag(UpdateType.Terminal))
            {
                SLSetup();
            }
            
                CheckSetup();
            
            if (arg != null)
            {
                Process(arg.ToUpper());
            }
            Echo(linebuilder.ToString());
            linebuilder.Clear();
        }
        private bool CheckSetup()
        {
            bool checkok = false;
            if (sldirectlcd != null)
            {
                var panelName = panelHasName(sldirectlcd, "SL-Line");
                if (panelName != "")
                {
                    var frame = _drawingSurface.DrawFrame();
                    DrawSprites(ref frame);
                    frame.Dispose();
                    sprites.Clear();
                    checkok = true;
                }
                else { linebuilder.AppendLine("NO SL-Line LCD found!\n"); }
            }
            else { linebuilder.AppendLine("NO SL-Line LCD found!\n"); }
            IMyBlockGroup sortergroup = GridTerminalSystem.GetBlockGroupWithName("SL-Direct");
            if (sortergroup == null)
            {
                if (checkok)
                {
                    checkok = false;
                }
                linebuilder.AppendLine("No SL-Direct Sorter Group found!\n");
            }
            return checkok;
        }
        MyConveyorSorterMode Blacklist = MyConveyorSorterMode.Blacklist;
        private void Stop(string stopitem, string stopitemtype)
        {
            if (sorters != null)
            {
                foreach (var sorter in sorters)
                {
                    MyDefinitionId myDefinitionId = MyDefinitionId.Parse("MyObjectBuilder_" + stopitem + "/" + stopitemtype);
                    bool isallowed = sorter.IsAllowed(myDefinitionId);
                    if (isallowed)
                    {
                        MyItemType iType = new MyItemType("MyObjectBuilder_" + stopitem, stopitemtype);
                        MyInventoryItemFilter UI = new MyInventoryItemFilter(iType);
                        filterlist.Add(UI);
                        sorter.SetFilter(Blacklist, filterlist);
                        isallowed = sorter.IsAllowed(myDefinitionId);
                    }
                }
            }
        }
        private void Allow(string allowitem, string allowitemtype)
        {
            if (sorters != null)
            {
                foreach (var sorter in sorters)
                {
                    MyItemType iType = new MyItemType("MyObjectBuilder_" + allowitem, allowitemtype);
                    MyDefinitionId myDefinitionId = MyDefinitionId.Parse("MyObjectBuilder_" + allowitem + "/" + allowitemtype);
                    bool isallowed = sorter.IsAllowed(myDefinitionId);
                    if (!isallowed)
                    {
                        MyInventoryItemFilter UI = new MyInventoryItemFilter(iType);
                        filterlist.Remove(UI);
                        sorter.SetFilter(Blacklist, filterlist);
                        isallowed = sorter.IsAllowed(myDefinitionId);
                    }
                }
            }
            else
            {
                linebuilder.AppendLine("NO SORTER GROUP\n");
            }
        }
        static
        System.Text.RegularExpressions.Regex SLDirectMatchRegex(string tags)
        {
            return new System.Text.RegularExpressions.Regex(@"([A - Z])\w +\/\w +\-\d +\+\d +");
        }

        public static int ConvertToInt(String input)
        {
            String inputCleaned = System.Text.RegularExpressions.Regex.Replace(input, "[^0-9]", "");
            int value = 0;
            if (int.TryParse(inputCleaned, out value))
            {
                return value;
            }
            return 0;
        }
        private void Process(string arg)
        {
            string[] item = arg.Split(' ');
            int itemLength = item.Length;
            if (arg.ToUpper().Contains("STOP "))
            {
                string stopitem; string stopitemtype;
                if (itemLength <= 1 || itemLength > 3)
                {
                    // TODO - make an error
                    return;
                }
                if (itemLength == 2)
                {
                    stopitem = "Ore";
                    stopitemtype = FirstLetterToUpper(item[1]);
                }
                else
                {
                    stopitem = FirstLetterToUpper(item[1]);
                    stopitemtype = FirstLetterToUpper(item[2]);
                }

                Stop(stopitem, stopitemtype);
            }
            if (arg.ToUpper().Contains("ALLOW "))
            {
                string allowitem; string allowitemtype;
                if (itemLength <= 1 || itemLength > 3)
                {
                    // TODO - make an error
                    return;
                }
                if (itemLength == 2)
                {
                    allowitem = "Ore";
                    allowitemtype = FirstLetterToUpper(item[1]);
                }
                else
                {
                    allowitem = FirstLetterToUpper(item[1]);
                    allowitemtype = FirstLetterToUpper(item[2]);
                }

                Allow(allowitem, allowitemtype);
            }
            FillTheOrder();
        }
        float current = 0;

        public string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;
            if (str.Length > 1)
            {
                str = str.ToLower();
                str = char.ToUpper(str[0]) + str.Substring(1);
            }
            else { str = str.ToUpper(); }
            return str;
        }
        private string panelHasName(IMyTextPanel panel, string test)
        {
            if (panel.CustomName.Contains(test) || panel.CustomData.Contains(test)) { return test; }
            return "";
        }
        public void PrepareTextSurfaceForSprites(IMyTextSurface textSurface)
        {
            textSurface.ContentType = ContentType.SCRIPT;
            textSurface.Script = "";
        }
        public void DrawSprites(ref MySpriteDrawFrame frame)
        {
            var sprite = new MySprite()
            {
                Type = SpriteType.TEXTURE,
                Data = "Grid",
                Position = _viewport.Center,
                Size = _viewport.Size,
                Color = _drawingSurface.ScriptForegroundColor.Alpha(0.66f),
                Alignment = TextAlignment.CENTER
            };
            position = new Vector2(0, 0) + _viewport.Position;
            sprite = new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = scriptname + " " + VERSION + " " + workingIndicator,
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.White,
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            };
            frame.Add(sprite);
            position = new Vector2(0, 20) + _viewport.Position;
            sprites.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "",
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.White,
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            });
            position += new Vector2(20, 0);
            sprites.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Item",
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.White,
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            });
            position += new Vector2(350, 0);
            sprites.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Min",
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.White,
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            });
            position += new Vector2(130, 0);
            sprites.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Max",
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.White,
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            });
            position += new Vector2(150, 0);
            sprites.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Current",
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.White,
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            });
            position += new Vector2(190, 0);
            sprites.Add(new MySprite()
            {
                Type = SpriteType.TEXT,
                Data = "Status",
                Position = position,
                RotationOrScale = 0.8f,
                Color = Color.White,
                Alignment = TextAlignment.LEFT,
                FontId = "White"
            });
            foreach (MySprite SLsprite in sprites)
            {
                frame.Add(SLsprite);
            }
        }

        private void SLSetup()
        {
            havelcd = false;
            List<IMyTextPanel> panels = new List<IMyTextPanel>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(panels);
            foreach (IMyTextPanel panel in panels)
            {
                if (!panel.IsSameConstructAs(Me)) { continue; }
                string panelName = "";
                panelName = panelHasName(panel, "SL-Line");
                if (panelName != "")
                {
                    sldirectlcd = panel;
                    sldirectlcd.ContentType = ContentType.TEXT_AND_IMAGE;
                    sldirectlcd.FontSize = 1;
                 //   sldirectlcd.CustomData = panelName;
                    havelcd = true;
                    continue;
                }
            }
            IMyBlockGroup sortergroup = GridTerminalSystem.GetBlockGroupWithName("SL-Direct");
            if (sortergroup != null)
            {
                sorters.Clear();
                sortergroup.GetBlocksOfType(sorters);
            }
            GetCargoLevels();
            filterlist.Clear();
            foreach (IMyConveyorSorter sorter in sorters)
            {
                sorter.GetFilterList(filterlist);
                sorter.SetFilter(Blacklist, filterlist);
            }
        }
        private Dictionary<char, int> _fontD = new Dictionary<char, int>
        {
          {' ', 8}, {'!', 8}, {'"', 10}, {'#', 19}, {'$', 20}, {'%', 24}, {'&', 20}, {'(', 9}, {')', 9}, {'*', 11}, {'+', 18}, {',', 9},
          {'-', 10}, {'.', 9}, {'/', 14}, {'0', 19}, {'1', 9}, {'2', 19}, {'3', 17}, {'4', 19}, {'5', 19}, {'6', 19}, {'7', 16}, {'8', 19},
          {'9', 19}, {':', 9}, {';', 9}, {'<', 18}, {'=', 18}, {'>', 18}, {'?', 16}, {'@', 25}, {'A', 21}, {'B', 21}, {'C', 19}, {'D', 21},
          {'E', 18}, {'F', 17}, {'G', 20}, {'H', 20}, {'I', 8}, {'J', 16}, {'K', 17}, {'L', 15}, {'M', 26}, {'N', 21}, {'O', 21}, {'P', 20},
          {'Q', 21}, {'R', 21}, {'S', 21}, {'T', 17}, {'U', 20}, {'V', 20}, {'W', 31}, {'X', 19}, {'Y', 20}, {'Z', 19}, {'[', 9}, {']', 9},
          {'^', 18}, {'_', 15}, {'`', 8}, {'a', 17}, {'b', 17}, {'c', 16}, {'d', 17}, {'e', 17}, {'f', 9}, {'g', 17}, {'h', 17}, {'i', 8},
          {'j', 8}, {'k', 17}, {'l', 8}, {'m', 27}, {'n', 17}, {'o', 17}, {'p', 17}, {'q', 17}, {'r', 10}, {'s', 17}, {'t', 9}, {'u', 17},
          {'v', 15}, {'w', 27}, {'x', 15}, {'y', 17}, {'z', 16}, {'{', 9}, {'|', 6}, {'}', 9}, {'~', 18}, {'\\', 12}, {'\'', 6}
        };
        public float AlignWord(StringBuilder addTo, string word, float maxChars, char fillChar = ' ')
        {
            var fillCharWidth = _fontD[fillChar];
            var maxWidth = fillCharWidth * maxChars;
            int width = 0;
            foreach (var ch in word) width += _fontD[ch] + 1;
            var leftOver = maxWidth - width;
            var numToAdd = (int)(leftOver / fillCharWidth);
            var error = maxWidth - width - (numToAdd * fillCharWidth);
            addTo.Append(word).Append(fillChar, numToAdd);
            return error / fillCharWidth;
        }
        private void GetCargoLevels()
        {
            List<IMyCargoContainer> containers = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(temp, blk => blk.CubeGrid.IsSameConstructAs(Me.CubeGrid));
            foreach (IMyCargoContainer container in temp)
            {
                if (!container.IsSameConstructAs(Me) || container.BlockDefinition.SubtypeId.Contains("Locker") || container.BlockDefinition.SubtypeId.Contains("Weapon"))
                {
                    continue;
                }
                containers.Add(container);
            }
            if (containers == null) { linebuilder.AppendLine("No Containers Found!\n"); return; }
            storage.AddRange(containers);
        }

        Dictionary<string, MyTuple<string, string>> defs = new Dictionary<string, MyTuple<string, string>>();
        Dictionary<string, double> density = new Dictionary<string, double>();
        public void Save()
        {
            MyIniParseResult result;
            if (!_ini.TryParse(Me.CustomData, out result))
                throw new Exception(result.ToString());
            Storage = _ini.ToString();
        }
        private void buildDefs(string t, string itm, string itmt)
        {
            defs.Add(t, new MyTuple<string, string>(itm, itmt));
            string temp = _ini.Get("SupplyLines", t).ToString(DefaultDIRECTValues);
            _ini.Set("SupplyLines", t, temp);
        }
        private void buildDefs(string t, string itm, string itmt, double densness)
        {
            density.Add(itmt + " " + itm, densness);
            buildDefs(t, itm, itmt);
        }
        void createDefs()
        {
            defs.Clear();
            // INGOTS
            buildDefs("Cobalt Ingot", "Ingot", "Cobalt", 8.929);
            buildDefs("Gold Ingot", "Ingot", "Gold", 19.23);
            buildDefs("Gravel", "Ingot", "Stone");
            buildDefs("Iron Ingot", "Ingot", "Iron", 7.85);
            buildDefs("Magnesium Powder", "Ingot", "Magnesium", 1.739);
            buildDefs("Nickel Ingot", "Ingot", "Nickel", 8.929);
            buildDefs("Old Scrap Metal", "Ingot", "Scrap", 3.937);
            buildDefs("Platinum Ingot", "Ingot", "Platinum", 21.276);
            buildDefs("Silicon Wafer", "Ingot", "Silicon", 2.331);
            buildDefs("Silver Ingot", "Ingot", "Silver", 10.526);
            buildDefs("Uranium Ingot", "Ingot", "Uranium", 19.231);
            // ORES
            buildDefs("Cobalt Ore", "Ore", "Cobalt");
            buildDefs("Gold Ore", "Ore", "Gold");
            buildDefs("Ice", "Ore", "Ice");
            buildDefs("Iron Ore", "Ore", "Iron");
            buildDefs("Magnesium Ore", "Ore", "Magnesium");
            buildDefs("Nickel Ore", "Ore", "Nickel");
            buildDefs("Organic", "Ore", "Organic");
            buildDefs("Platinum Ore", "Ore", "Platinum");
            buildDefs("Scrap Metal", "Ore", "Scrap");
            buildDefs("Silicon Ore", "Ore", "Silicon");
            buildDefs("Silver Ore", "Ore", "Silver");
            buildDefs("Stone", "Ore", "Stone");
            buildDefs("Uranium Ore", "Ore", "Uranium");
        }

        private void FillTheOrder()
        {
            MyIniParseResult result;
            if (!_ini.TryParse(Me.CustomData, out result))
            {
                throw new Exception(result.ToString());
            }
            if (Me.CustomData != null && Me.CustomData != "")
            {
                string[] directs = Me.CustomData.Split(new char[] { '\n' });
                int linecount = 2;
                foreach (string direct in directs)
                {
                    if (direct == "[SupplyLines]") { continue; }
                    string[] parts = direct.Split(new char[] { '=' });
                    if (parts.Length != 2) { continue; }
                    string part = parts[0];
                    string _tempValue = _ini.Get("SupplyLines", part).ToString(DefaultDIRECTValues);
                    if (_tempValue == DefaultDIRECTValues) { continue; }
                    var type = defs[part];
                    var item = type.Item1;
                    var itemtype = type.Item2;
                    float ingitems = 0f;
                    float ingcurrent = 0f;
                    bool full = false;
                    bool ingfull = false;
                    string parts1 = parts[1];
                    string[] numbers = parts1.Split(new char[] { '+' });
                    if (numbers.Length != 2) { continue; }
                    string mins = numbers[0].Replace("-", "");
                    string maxs = numbers[1];
                    float imin = float.Parse(mins);
                    float imax = float.Parse(maxs);
                    string ingmins, ingmaxs;
                    float ingmin = 0, ingmax = 0;
                    if (imin > 0 || imax > 0)
                    {
                        current = 0f;
                        ingmin = imin; ingmax = imax;
                        float invmax = 0;
                        float percent = 0; float ingpercent; double denseness = 0; double ingdenseness = 0;
                        string lookup = "";
                        if (item == "Ore")
                        {
                            lookup = itemtype + " Ingot";
                            if (itemtype == "Magnesium")
                            {
                                lookup = itemtype + " Powder";
                            }
                            if (itemtype == "Silicon")
                            {
                                lookup = itemtype + " Wafer";
                            }
                            if (itemtype == "Scrap")
                            {
                                lookup = "Old Scrap Metal";
                            }
                            if (itemtype == "Stone")
                            {
                                lookup = "Gravel";
                            }
                            _tempValue = _ini.Get("SupplyLines", lookup).ToString(DefaultDIRECTValues);
                            if (_tempValue != DefaultDIRECTValues)
                            {
                                if (!density.TryGetValue(itemtype + " Ingot", out ingdenseness))
                                { ingdenseness = 2.7027; }
                                numbers = _tempValue.Split(new char[] { '+' });
                                ingmins = numbers[0].Replace("-", "");
                                ingmaxs = numbers[1];
                                ingmin = float.Parse(ingmins);
                                ingmax = float.Parse(ingmaxs);
                            }
                        }
                        if (!density.TryGetValue(itemtype + " " + item, out denseness))
                        { denseness = 2.7027; }
                        for (int i = 0; i < storage.Count; i++)
                        {
                            var inventoryOwner = (IMyInventoryOwner)storage[i];
                            var inventory = inventoryOwner.GetInventory(0);
                            invmax += (float)inventory.MaxVolume * 1000f;
                            var items = inventory.GetItemAmount(new MyItemType("MyObjectBuilder_" + item, itemtype));
                            current += (float)((double)items / denseness);
                            if (item == "Ore")
                            {
                                if (_tempValue != DefaultDIRECTValues)
                                {
                                    ingitems += (float)inventory.GetItemAmount(new MyItemType("MyObjectBuilder_" + "Ingot", itemtype));
                                    ingcurrent = (float)(ingitems / ingdenseness);
                                }
                            }
                            else
                            {
                                ingitems += (float)items;
                                ingcurrent = current;
                            }
                        }
                        if (item == "Ore")
                        {
                            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(temp, blk => blk.CubeGrid.IsSameConstructAs(Me.CubeGrid));
                            for (int x = 0; x < temp.Count; x++)
                            {
                                var inventoryOwner = (IMyInventoryOwner)temp[x];
                                var inventory = inventoryOwner.GetInventory(0);
                                var items = inventory.GetItemAmount(new MyItemType("MyObjectBuilder_" + item, itemtype));
                                current += (float)((double)items / denseness);
                            }
                        }
                        percent = current != 0 ? current / invmax * 100 : 0;
                        ingpercent = ingcurrent != 0 ? ingcurrent / invmax * 100 : 0;
                        full = percent >= imax;
                        ingfull = ingpercent >= float.Parse(numbers[1]);
                        Color color = Color.White;
                        string status = "";
                        if (full || ingfull)
                        {
                            color = Color.Green;
                            status = "FULL";
                            isStop = true;
                        }
                        if ((item == "Ore" & percent < imax) || (item == "Ingot" && ingpercent < ingmax))
                        {
                            color = Color.Yellow;
                            status = "OPEN";
                            isStop = false;
                        }
                        if ((item == "Ore" & percent <= imin & imin != 0) || (item == "Ingot" & ingpercent < ingmin & ingmin != 0))
                        {
                            color = Color.Red;
                            status = "URGENT";
                            isStop = false;
                        }
                        if (ingfull & item != "Ingot")
                        {
                            color = Color.Green;
                            status = "FULL (Ingots)";
                            isStop = true;
                        }
                        if (isStop == true)
                        {
                            Stop(item, itemtype);
                        }
                        else
                        {
                            Allow(item, itemtype);
                        }
                        /* TODO - FUTURE FEATURE
                        // TODO - CALL SAM.......
                        bool SAMenRoute = false;
                        if (part == "Gold Ore") { SAMenRoute = true; }
                        if (SAMenRoute)
                        {
                            color = Color.Blue;
                            status = "SUPPLYING";
                        }
                        */
                        position = new Vector2(0, 20 * linecount) + _viewport.Position;
                        sprites.Add(new MySprite()
                        {
                            Type = SpriteType.TEXT,
                            Data = "",
                            Position = position,
                            RotationOrScale = 0.8f,
                            Color = color,
                            Alignment = TextAlignment.LEFT,
                            FontId = "White"
                        });
                        position += new Vector2(20, 0);
                        sprites.Add(new MySprite()
                        {
                            Type = SpriteType.TEXT,
                            Data = part,
                            Position = position,
                            RotationOrScale = 0.8f,
                            Color = color,
                            Alignment = TextAlignment.LEFT,
                            FontId = "White"
                        });
                        position += new Vector2(370, 0);
                        sprites.Add(new MySprite()
                        {
                            Type = SpriteType.TEXT,
                            Data = mins,
                            Position = position,
                            RotationOrScale = 0.8f,
                            Color = color,
                            Alignment = TextAlignment.CENTER,
                            FontId = "White"
                        });
                        position += new Vector2(130, 0);
                        sprites.Add(new MySprite()
                        {
                            Type = SpriteType.TEXT,
                            Data = maxs,
                            Position = position,
                            RotationOrScale = 0.8f,
                            Color = color,
                            Alignment = TextAlignment.CENTER,
                            FontId = "White"
                        });
                        position += new Vector2(180, 0);
                        sprites.Add(new MySprite()
                        {
                            Type = SpriteType.TEXT,
                            Data = percent > 0 ? percent.ToString("n2") : current.ToString("n2"),
                            Position = position,
                            RotationOrScale = 0.8f,
                            Color = color,
                            Alignment = TextAlignment.RIGHT,
                            FontId = "White"
                        });
                        position += new Vector2(100, 0);
                        sprites.Add(new MySprite()
                        {
                            Type = SpriteType.TEXT,
                            Data = status,
                            Position = position,
                            RotationOrScale = 0.8f,
                            Color = color,
                            Alignment = TextAlignment.LEFT,
                            FontId = "White"
                        }); linecount++;
                        /* TODO - FUTURE FEATURE
                        // process stuff
                        // check for direct sorters and fill from there first
                        // update CD with new order for those items (leaving other lines alone)

                        // verify once again 

                        // create a JOB for SAM

                        // send SAM
                        */
                    }
                    else { linebuilder.AppendLine (part + " not found!  See https://github.com/malware-dev/MDK-SE/wiki/Type-Definition-Listing#other\n"); return; }
                }
            }
        }
