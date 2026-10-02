 // SIMPLE RECEIVER
        // Name an LCD with the EXACT same name as the program block that runs this script
        // Make sure they all have a name that starts with the prefix below and then a number from 1 to (number of LCDS in your group)
        // This way each LCD should have the exact name of one of the channels that is BROADCASTING (sender script)
        // Make sure you set the display of your LCDs as you want (to show text, font, color).   The script ONLY puts the text onto it
        string channel = "sector1";


        //No change after this

        IMyTextPanel MyLCD;
        List<IMyBroadcastListener> listeners = new List<IMyBroadcastListener>();
       // IMyBlockGroup blocksgroup;
        MyIGCMessage packet = new MyIGCMessage();
        List<IMyTextPanel> lcds = new List<IMyTextPanel>();
        bool setupcomplete = false;
        string lcdmessage = "";
        IMyBroadcastListener Mylistener;
        // List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            setupcomplete = false;
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (!setupcomplete)
            {
                init();

            }
            else
            {
                getmessage();
            }

            switch (argument.ToLower())
            {
                case "go":
                    getmessage();
                    break;
                case "reset":
                    findlcd();
                    break;
            }

        }   //end of main


        public void getmessage()
        {
            Mylistener = TheListener(channel);  // Get the channel for this LCD

            if (Mylistener != null) // we have this channel
            {
                if (Mylistener.HasPendingMessage == true)  //there is a message
                {
                    packet = Mylistener.AcceptMessage();
                    if (packet.Data != null)
                    {
                        lcdmessage = packet.Data.ToString();
                    }
                }
            }
            MyLCD.WriteText(lcdmessage);
        } // end of fillscreens

        public IMyBroadcastListener TheListener(string channel)
        {
            IGC.GetBroadcastListeners(listeners);
            Boolean found = false;
            IMyBroadcastListener Mylistener = null;

            foreach (IMyBroadcastListener Alistener in listeners)
            {
                if (Alistener.Tag == channel) { found = true; Mylistener = Alistener; break; }
            }
            if (found == true) { return Mylistener; } else { return null; }
        }

        public void findlcd()

        {
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcds, block => block.IsSameConstructAs(Me));
            if (lcds.Count() > 0)
            {
                for (int i = 0; i < lcds.Count(); i++)
                {
                    if (lcds[i].CustomName == Me.CustomName)
                    { MyLCD = lcds[i]; break; }
                }
            }
            if (MyLCD == null)
            {
                Me.GetSurface(0).WriteText("ERROR !  No LCD found with name " + Me.CustomName + "\nThe script expects an LCD with the same name as this program block");
                setupcomplete = false;
            }
            else
            {
                MyLCD.WriteText("LCD found - waiting for first message to show");
                Me.GetSurface(0).WriteText("LCD found");
            }
        }



        public void init()
        {
            setupcomplete = true;
            findlcd();

            IGC.RegisterBroadcastListener(channel);
            listeners.Clear();
            IGC.GetBroadcastListeners(listeners);
            lcdmessage = "";
        }  // end of init


        // end of code