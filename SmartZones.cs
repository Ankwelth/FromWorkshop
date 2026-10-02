/*
 *   R e a d m e
 *   -----------
 * 
 *  Thanks for choosing SmartZones, the tool for conserving your zone chips while you are 
 *  away from the game. This script is mostly handsfree, the only argument id reccomend
 *  is "store" which will dump SafeZone sizes to its customdata. The data within customData
 *  controls the x, y, z, and spherical sizes respectively. 
 *  
 *  Beyond that, this script is still a largely work in progress but it none-the-less covers
 *  the base cases ive initially set out to code for. 
 *  
 *  Safe zones will by default rest in a sleep mode. When damage, enemies, missing blocks,
 *  or the total missing blocks is below your defined threshold, the safezones will go into
 *  standby. 
 *  
 *  For everything but damage, if any of the conditions which activated it are still present
 *  after a zone has been activated, the zone will fully remain in active state. If not, 
 *  an emp like attack will blast the safe zone to full size to push, harm, and damage anything
 *  that may have wandered in. 
 *  
 *  These criteria should allow for efficiently using zone chips instead of just having zones
 *  activate willy nilly.
 * 
 */
        ////////////////////// USER DEFINED VARIABLES ////////////////
        //int ledge = 39;
        int damageThreshold = 50; // The threshold in which the safe zones should permanently activate
        //bool powerAdjustOverride = true; // when the slider is less than 11 on all safe zones, use instead an approximation of safe zone sizes to power ratio
        //////////////////////////////////////////////////////////////
        List<SafeZone> zones; // Zones
        List<IMyTerminalBlock> blocks;
        List<IMySlimBlock> blocks2; // Cube blocks
        List<IMySensorBlock> sensors; // Sensorss
        List<IMyUserControllableGun> turrets; // Turrets 2
        List<IMyPowerProducer> powerBlocks; // all power produces
        List<MyDetectedEntityInfo> enemies; // Enemies list

        StateMachine<bool> stateMachine1; // Statemachine 1
        StateMachine<int> stateMachine2; // Statemachine 2
        StateMachine<double> stateMachine3; // StateMachine 3


        bool blockWasMissing; // A control that determines if a block was missing previously
        bool currentDamage; // Current damage value
        int blockCounts; // The overall total block counts
        bool currentZone; // Holds truthiness if we should maintain safeZoneFull
        bool settingZone; // Holds truthiness if we need to be prepping a safe zone
        double mwUsed; // The current MW used
        int tickHour; // Time till shield deactivation
        int tickActivate; // Time till activation of shields
        int chipCount; // Chips counts
        // reinitializes state machines
        public void reinitStateMachines()
        {
            // Clean state machines before initializing
            if (stateMachine1 != null) { stateMachine1.clean(); }
            if (stateMachine2 != null) { stateMachine2.clean(); }
            if (stateMachine3 != null) { stateMachine3.clean(); }
            stateMachine1 = new StateMachine<bool>(); // Build new state machines
            stateMachine2 = new StateMachine<int>(); // ^
            stateMachine3 = new StateMachine<double>(); // ^

        }

        /** Rebuild save */
        public void rebuildFromSave()
        {
            if (Storage.Length > 0)
            {
                string store = Storage;
                int index = Storage.IndexOf('|'); // Seperator
                try
                {
                    tickHour = int.Parse(Storage.Substring(0, index));
                    blockCounts = int.Parse(Storage.Substring(index + 1, Storage.Length)); // Indexing starts + 1
                    Echo("Save load success");
                }
                catch
                {
                    Echo("No timetables currently being kept");
                    tickHour = 0;
                    blockCounts = -1;
                    currentZone = false;
                }

            }
            else
            {
                blockCounts = -1;
                tickHour = 0;
                currentZone = false;
            }
        }

        /**construction */
        public Program()
        {
            Me.GetSurface(0).Alignment = VRage.Game.GUI.TextPanel.TextAlignment.LEFT;
            Me.GetSurface(0).ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
            Me.GetSurface(0).FontSize = 1.5f;
            rebuildFromSave(); // Rebuild save should not occur in buildIt
            buildIt();
            chipCount = 0;
            blockWasMissing = false;
            Echo("Block counts before = " + blockCounts);
            Echo("Block counts should be less than 1 = " + (blockCounts < 1));
            // Block counts should only be initialized once
            if (blockCounts < 1)
            {
                Echo("Setting block counts to " + blocks.Count());
                blockCounts = blocks.Count();
            }
            Runtime.UpdateFrequency = UpdateFrequency.Update10; // Sets runtime frequency
        }
        /** Invocable method of the constructor */
        public void buildIt()
        {

            currentDamage = false;
            mwUsed = 0;
            reinitStateMachines(); // Clean and initialize state machines
            tickActivate = 0;
            zones = new List<SafeZone>();
            //terms = new List<IMyTerminalBlock>();
            blocks2 = new List<IMySlimBlock>();
            blocks = new List<IMyTerminalBlock>();
            turrets = new List<IMyUserControllableGun>();
            sensors = new List<IMySensorBlock>();
            powerBlocks = new List<IMyPowerProducer>();
            enemies = new List<MyDetectedEntityInfo>();

            GridTerminalSystem.GetBlocks(blocks);
            stateMachine2.setStateMachine(buildSlimBlocks1(blocks2, Me.CubeGrid)); // Build a first list
            // Cycle through
            foreach (IMyTerminalBlock block in blocks)
            {
                // If block is a safe zone block
                if (block is IMySafeZoneBlock)
                {
                    SafeZone temp = new SafeZone((IMySafeZoneBlock)block);
                    zones.Add(temp); // Pass
                }
                // Add all gun types to the list
                if (block is IMyUserControllableGun)
                {
                    turrets.Add((IMyUserControllableGun)block);
                }
                // Add a sensor block if it detects enemys
                if (block is IMySensorBlock)
                {
                    IMySensorBlock temp = (IMySensorBlock)block; // Cast
                    // only add this if the element detects enemies
                    if (temp.DetectEnemy)
                    {
                        sensors.Add(temp);
                    }
                }
                // power producing block
                if (block is IMyPowerProducer)
                {
                    powerBlocks.Add((IMyPowerProducer)block);
                }
            }
            // Useless operators
            if (mwUsed > 0) { return; }
            // Store block counts
            // Current zone was intiially a time check
            bool checkThresh = (blocks.Count <= blockCounts * (damageThreshold / 100));
            Echo("Block missing " + blockMissing());
            // Obviously when we go to check blockIsMissing(), it will be reported as true since
            // We just rebuilt the lists.
            currentZone = checkThresh || enemyDetected() || enemyTargetted() || blockWasMissing;


        }


        /** Since the script will have to track time for hours at a time, we need to offload to storage */
        public void Save()
        {
            Storage = tickHour.ToString() + "|" + blockCounts.ToString(); // Convert the seconds and block counts to storage

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        /** Main execution */
        public void Main(string argument, UpdateType updateSource)
        {

            // If statemachine 1 is free. and we have built all the freeblocks on stateMachine2
            if (stateMachine1.isFreeState() && stateMachine2.isFreeState())
            {
                stateMachine1.setStateMachine(checkDeformation());
            }
            // Damage detected on statemachine 1
            if (stateMachine1.Current())
            {
                echoWrite("Warning: Damage detected. Running standby");
            }
            else
            {
                echoWrite("No damage detected. Sleeping...");
            }

            if (argument == "store")
            {
                foreach (SafeZone zone in zones)
                {
                    zone.storeData();
                }
            }
            echoWrite("Chip counts = " + chipCount, true);
            // Gets the current power use in mw
            /*if (stateMachine3.isFreeState())
            {
                stateMachine3.setStateMachine(getPowerUse(1d)); // Gets power usage as kw
            }
            else
            {
                mwUsed = stateMachine3.Current();
            }*/
            safeZoneLogic();
            //echoWrite("Current grid power = " + mwUsed, true); // Output power info

            //Echo("Anticipated zone size by power = " + zones.ElementAt(0).zoneSizeByPower(12000, 1000d)[0]);


        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        /**
         * Contains the primary logic of how safe zones should typically function. While I think i have the gist down
         * The concept should be as follows:
         *
         * For situations concerning a standby usecase;
         * - When blocks are detected to have come under damage,
         * - When turrets are detected to be shooting at something
         * - When sensors are detecting an enemy
         *
         * For situations concerning a fullon safecase;
         * - When blocks are detected to have been destroyed
         * - When sensors or turrets are not functional
         * - Sensors and turrets are still detecting entities.
         *
         * When we are in standby, all systems begin operation... All turrets are to come online,
         * all sensors to come online, and the safezone blocks must be primed.
         *
         * When we are in fullon, the safe zone is activated, and we must now check the conditions
         * for a full activation status.
         */
        public void safeZoneLogic()
        {
            Echo("Current Zone = " + currentZone);
            // If one of the zones are activated
            if (zoneActivity())
            {
                settingZone = false; // Relinquish standby control
                // If we need a full zone activation and we havent already checked this
                if (!currentZone && needFullZone(true))
                {
                    bool checkThresh = (blocks.Count <= blockCounts * (damageThreshold / 100));
                    blockWasMissing = blockMissing(); // Calibrate a block was missing check
                    currentZone = checkThresh || enemyDetected() || enemyTargetted() || blockWasMissing;

                    // Current zone control only activates if the threshold has not been satisfied,
                    // Enemies detected, targetted, or blocks are substantially missing
                    tickHour = 0; // Reset hour timer
                    tickActivate = 0; // Reset tick activation
                    setZones(3); // Activate zones
                    buildIt(); // Recompile lists'
                }
                // Give the zone at least 3 seconds to go to full power and kick out any undesireables
                else if (!currentZone && tickToSec(tickHour) > 3)
                {
                    Echo("Running force sleep");
                    setZones(2); // Run sleep mode
                }
                else
                {
                    Echo("Running timed sleep");
                    setZones(); // Run activation mode with timeout on the sim seconds
                }
                echoWrite("Time until shields deactivate = " + (3600 - tickToSec(tickHour)), true);
                tickHour += tickCount(); // Always run seconds while a zones is on
                // Checks if time has been met. Debug at tick + 3575
                if (tickToSec(tickHour) >= 3600)
                {
                    currentZone = false;
                }
            }
            // No zone activity detected. Run standby protocols
            else
            {
                currentZone = false;
                // Damage detected, enemy detected, enemy targetted, etc.
                if (!settingZone && needStandbyZone())
                {
                    tickHour = 0; // Reset tick hour count
                    tickActivate = 0; // Reset tick activation
                    setZones(1); // Run standby mode
                    settingZone = true;
                }
                // If we are not setting a zone
                else if (!settingZone)
                {
                    setZones(2); // Continuing sleep mode

                }
                /** Run seconds calculator */
                if (settingZone)
                {
                    echoWrite("Time till shield activation = " + (120 - (tickActivate / 60)), true);
                    tickActivate += tickCount();
                }
            }
        }

        /** Constrol for handling which zone type should take place */
        public void setZones(int check = -1)
        {
            chipCount = 0;
            foreach (SafeZone zone in zones)
            {
                zone.runZone(check, tickHour / 60); // Pass in seconds
                chipCount += zone.getChips(); // Add chips for external counting
            }
        }
        /** Runs through all zones to find any that have an activated shield */
        public bool zoneActivity()
        {
            foreach (SafeZone zone in zones)
            {
                /** zone isnt closed and is activated */
                if (!Closed(zone.getBlock()) && zone.isActivated())
                {
                    return true;
                }
            }
            return false;
        }

        /** Keeps time whenever any safe zone is activated */
        public int keepTime()
        {
            return tickToSec(tickCount(Runtime.UpdateFrequency)); // Add seconds
        }

        public int tickCount()
        {
            return tickCount(Runtime.UpdateFrequency);
        }


        /** determines if we need the zones in standby */
        public bool needStandbyZone()
        {

            // Check if we need a current safe zone range

            return stateMachine1.Current() || enemyDetected() || enemyTargetted() || blockMissing() || blocks.Count <= blockCounts * (damageThreshold / 100);
        }


        /** Determines if we need a full zone, once the prepared safe zone has been generated
         * True if;
         * -We have zone activity (run time passing)
         * -an enemy is still being targetted or detected
         * -a block is missing
         * -the overal blocks count is less than desired threshold
         */
        public bool needFullZone(bool zoneActivity)
        {
            bool checkThresh = (blocks.Count <= blockCounts * (damageThreshold / 100));
            /*Echo("Zone Activity = " + zoneActivity);
            Echo("Enemy targetted = " + enemyTargetted());
            Echo("Block missing = " + blockMissing());
            Echo("Thresh Check = " + checkThresh);*/
            return (zoneActivity && (enemyDetected() || enemyTargetted() || blockMissing() || checkThresh));
        }

        /** Check if we are targetting any enemies */
        public bool enemyDetected()
        {
            enemies.Clear(); // Clear enemies
            // Check sensors for any enemy activity
            foreach (IMySensorBlock sensor in sensors)
            {
                sensor.DetectedEntities(enemies); // Send the list to enemies
                // Enemy count is greater than 0
                if (enemies.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
        /** Determines if the guns are shooting. Its assumed that if they are shooting,
         * They are targetting something */
        public bool enemyTargetted()
        {
            foreach (IMyUserControllableGun gun in turrets)
            {
                //Echo("Gun is " + gun);
                // If the gun is targetting something
                if (!Closed(gun) && gun.IsShooting())
                {
                    //Echo("Shooting");
                    return true;
                }
            }
            //Echo("No Shooting");
            return false; // No targets
        }

        /** Runs through slimblocks checking to see if any deformation is registered */
        public IEnumerator<bool> checkDeformation()
        {
            bool currDurr = false;
            foreach (IMySlimBlock block in blocks2)
            {
                int count = 0;
                // Check if this block has accumulated damage or destroyed
                if (block == null || !block.IsFullIntegrity)
                {
                    //echoWrite("Damage detected!");
                    currDurr = true;
                    yield return currDurr;
                    break;
                }
                count++;
                // Count hold
                if (count % 1000 == 0)
                {
                    //echoWrite("No damage detected yet");
                    yield return currDurr;
                }

            }
            currentDamage = currDurr; // update with current damage
            yield return currDurr;
        }

        /** Blocks missing */
        public bool blockMissing()
        {
            // Cycle through terminal blocks
            foreach (IMyTerminalBlock block in blocks)
            {
                // Block is no longer on the list
                if (Closed(block))
                {
                    return true;
                }
            }
            return false;
        }


        /** Checks to see if the block is still existing */
        public bool Closed(IMyTerminalBlock block)
        {

            if (block == null || block.WorldMatrix == MatrixD.Identity) return true;
            return !(GridTerminalSystem.GetBlockWithId(block.EntityId) == block);

        }


        /////////////////////////////  helpers //////////////////////////////
        /** Passes a sizeof op to build power at a particular range. 1000d = kw */
        public IEnumerator<double> getPowerUse(double size)
        {
            double power = 0;
            int count = 0;
            foreach (IMyTerminalBlock block in blocks)
            {

                power += block.CurrentPowerUse(size);
                count++;
                if (count % 1000 == 0)
                {
                    yield return power; // Return power use up to this point
                }
            }
            yield return power;
        }
        /** Get the max possible power of the grid */
        public double getPossiblePower()
        {
            double power = 0;
            foreach (IMyPowerProducer block in powerBlocks)
            {
                //power += block.MaxPutAct(GridTerminalSystem);
                if (!Closed(block))
                {
                    power += block.MaxPutAct();
                }

            }
            return power;
        }
        /**
         * Helping class for safezone blocks
         * Zone Chips are "MyObjectBuilder_Component/ZoneChip"
         */
        private class SafeZone
        {

            //private readonly List<ITerminalProperty> tProps = new List<ITerminalProperty>();
            //private readonly List<ITerminalAction> tActs = new List<ITerminalAction>();

            private IMySafeZoneBlock block; // A safe zone block
            private bool isSphereBefore; // A check for validating size type
            private Single sizeBefore; // The default size a user has configured the safe zone to be
            private Single[] sizeBefore3; // Stores the size before given all 3 axis
            /** Default constructor */
            public SafeZone() : this(null) { }
            /** Constructor for a new safezone block */
            public SafeZone(IMySafeZoneBlock temp)
            {
                setBlock(temp);
                Single[] old = loadData(); // first 3 elements are x y and z, last element = sphere
                sizeBefore = old[3]; // Initialize safezone size
                isSphereBefore = isSphere(); // Dictates what the default size setting is
                sizeBefore3 = old; // Store old, which when read, SHOULD read the first 3
            }
            /** Sets a block */
            public void setBlock(IMySafeZoneBlock pass) { block = pass; }
            /** Returns power usage of the block */
            public double getCurrPow() { return block.CurrentPowerUse(1000d); }
            /** Gets max Power */
            public double getMaxPow() { return block.MaxPowerUse(1000d); }
            /** Tells if the bubble is currently activated.
             * 5000 KW, or 5MW is what dictates that a safezone is succesfully activated */
            public bool isActivated() { return getCurrPow() >= 5000; }
            /** Gets the count of chips this sasfezone block has */
            public int getChips()
            {
                return (int)getBlock().GetInventory().GetItemAmount(MyItemType.MakeComponent("ZoneChip"));
            }
            /** Returns this block casted as a terminal block */
            public IMyTerminalBlock getBlock()
            {
                return (IMyTerminalBlock)block;
            }
            /** Checks if safezone is set to spherical mode */
            public bool isSphere()
            {
                return getZoneType() == 0;
            }

            public Int64 getZoneType()
            {
                return getBlock().GetValue<Int64>("SafeZoneShapeCombo");
            }
            /** Gets zone size */
            public Single getZoneSize()
            {
                return getBlock().GetValue<Single>("SafeZoneSlider");
            }
            /** Sets safe zone size (spherical) */
            public void setZoneSize(Single dist)
            {
                getBlock().SetValue<Single>("SafeZoneSlider", dist);
            }
            /** Gets the xZone size */
            public Single getZoneXSize()
            {
                return getBlock().GetValue<Single>("SafeZoneXSlider");
            }
            /** Sets the xZone size */
            public void setZoneXSize(Single dist)
            {
                getBlock().SetValue<Single>("SafeZoneXSlider", dist);
            }
            /** Gets the yZone size */
            public Single getZoneYSize()
            {
                return getBlock().GetValue<Single>("SafeZoneYSlider");
            }
            /** Sets the yZone size */
            public void setZoneYSize(Single dist)
            {
                getBlock().SetValue<Single>("SafeZoneYSlider", dist);
            }
            /** Gets the zZone size */
            public Single getZoneZSize()
            {
                return getBlock().GetValue<Single>("SafeZoneZSlider");
            }
            /** Sets the zZone size */
            public void setZoneZSize(Single dist)
            {
                getBlock().SetValue<Single>("SafeZoneZSlider", dist);
            }
            /** Sets all 3 zone sizes with an array of sizes */
            public void setZoneSize3(Single[] size)
            {
                if (size == null || size.Length < 1)
                {
                    return; // dont work on nulls or bad sized lists
                }
                // If the length is between 1 and 3,
                if (size.Length < 3)
                {
                    setZoneSize(size[0]); // Set a spherical size instead
                    return;
                }
                setZoneXSize(size[0]); // Sets zone sizes
                setZoneYSize(size[1]); // ^
                setZoneZSize(size[2]); // ^
            }
            /** Return an array representation of all xyz zone slider sizes */
            public Single[] getZoneSize3()
            {
                return new Single[3] { getZoneXSize(), getZoneYSize(), getZoneZSize() };
            }
            /** Calculates anticipated power use based on current safe zone sizes.
             *  The ratio is that every 100m (sphere) or combined total of x,y,z = 600m (square)
             *  The power is around a 60000 KW increase.
             */
            public double anticipatePowerUse(Single[] dist, double size)
            {
                return (dist.Length < 1) ? 0 : ((dist.Length < 3) ? 5 * size + (dist[0] - 10) * .6 * size : 5 * size + (dist[0] + dist[1] + dist[2] - 30) * .1 * size);
            }
            /** Returns theoretically possible zone sizes dependent on the mode and power input */
            public Single[] zoneSizeByPower(double power, double size)
            {
                Single part1 = (Single)(power - 5 * size);
                Single sphereAdjust = (Single)(part1 / (.6 * size));
                Single cubeAdjust = (Single)(part1 / (.3 * size));
                // The Actual part size is unknown to us if we are cubical.
                return (isSphere()) ? new Single[1] { sphereAdjust + 10 } : new Single[3] { cubeAdjust + 10, cubeAdjust + 10, cubeAdjust + 10 } ;
            }
            /** Checks to see if the safeZone is enabled to create zones */
            public bool canCreate()
            {
                return getBlock().GetValue<bool>("SafeZoneCreate");
            }
            /** sets the safe zone creation switch*/
            public void setCreate(bool flip)
            {
                // Deal with infinite settings by readjusting the control
                if (canCreate() == flip) { return; }
                getBlock().SetValue<bool>("SafeZoneCreate", flip);
            }
            /** Prepares a safe zone such that intruders arent aware that the station has a safe zone
             * DO NOT RECOMPILE SAFEZONE LISTS WHEN PREPARE ZONES IS ACTIVATED. Rebuild lists after
             * restoring zones!
             */
            public void StandbyZone()
            {

                setCreate(true); // Prepare zone creation
                setZoneSize(10); // Minimize size to hide defense measures
                setZoneXSize(10); // ^
                setZoneYSize(10); // ^
                setZoneZSize(10); // ^
            }
            /** This is like the parent except it except it builds with 3600, the time of 1 hour, bypassing time check */
            public void SleepZone()
            {
                SleepZone(3600);
            }
            /** In the event of whatever was causing the system to lose its collective shit has disapated
             * Restore the safe zone to standby (restore previous slider values, and disable zone creation)
             */
            public void SleepZone(int time)
            {
                // Always set the size before
                setZoneSize(sizeBefore); // Sets the spherical size to size before
                setZoneSize3(sizeBefore3); // Sets the cubical size to size before x3
                if (time < 3600) { return; } // Dont work it unless an hour has passed
                setCreate(false); // Restore to false state

            }

            /** fully activate the zone to size levels */
            public void ActivateZone(bool flip)
            {
                // Runs only on the conition that the zone is already activated.
                if (isActivated())
                {
                    // Restore the previous sizes which causes anything not supposed to be there to be warped out
                    // Hopefully killing them and their grids in the process >=)
                    setZoneSize(sizeBefore);
                    setZoneSize3(sizeBefore3);
                }
            }
            /** Runs safe zone with default being sleep zone on time = 3600 */
            public void runZone(int control)
            {
                runZone(control, 3600);
            }

            /** Runs one of the above zones based on int condition
             * Time is in seconds
             */
            public void runZone(int control, int time)
            {
                switch (control)
                {
                    // 1 = standby
                    case 1:
                        StandbyZone();
                        break;
                    // 2 = sleep
                    case 2:
                        SleepZone();
                        break;
                    // 3 = activate
                    case 3:
                        ActivateZone(true);
                        break;
                    // all else = sleep with time
                    default:
                        SleepZone(time);
                        break;
                }
            }
            /**
             * Stores the info to custom data. Avoids issues with compiling and recompiling bs
             * and avoids mishaps with the frikken building
             */
            public void storeData()
            {
                block.CustomData = "[ssj5]" + getZoneXSize() + "|" + getZoneYSize() + "|" + getZoneZSize() + "|" + getZoneSize();
            }
            /** Loads the sliders from data */
            public Single[] loadData()
            {
                String temp = block.CustomData;
                Single[] fallback = new Single[4] { getZoneXSize(), getZoneYSize(), getZoneZSize(), getZoneSize() };
                if (temp.Length < 1)
                {
                    return fallback;
                }
                temp.Remove(0, 5); // Remove start
                String[] part1 = temp.Split('|');
                Single[] result = new Single[4];
                for (int i = 0; i < part1.Length && i < result.Length; i++)
                {
                    //Single good = (i > 2) ? getZoneSize() : ((i > 1) ? getZoneZSize() : ((i > 0) ? getZoneYSize() : getZoneXSize()));
                    Single good = fallback[i]; // Use ith element
                    Single.TryParse(part1[i], out good);
                    result[i] = good;
                }
                return result;
            }

            /** For later use which should dynamically set a power zone to max available power */
            public bool usePowerThresholds(bool flip)
            {
                // validates if we should use power thresholds for a given safezone
                return flip && ((isSphere() && getZoneSize() < 11) || (!isSphere() && getZoneXSize() < 11 && getZoneYSize() < 11 && getZoneZSize() < 11));
            }


        }

        /**
         * This class is responsible for holding information about statemachines and allows for isFree()
         * to assert with this object instead.
         *
         * Furthermore, it also provides livetime access to the last objects value during its use so even if
         * the state machine is done working, it still has access to the last retrieved value
         */
        private class StateMachine<T>
        {
            private IEnumerator<T> stateMachine; // State machine reference
            T current; // The current information generated during free execution
            /** Default constructor */
            public StateMachine() : this(null)
            {
                //IMyGridProgramRuntimeInfo
            }
            /**
             * Builds a state machine with the given method
             */
            public StateMachine(IEnumerator<T> machine)
            {
                setStateMachine(machine);
            }
            /** Sets the state machine for the object */
            public void setStateMachine(IEnumerator<T> machine)
            {
                stateMachine = machine;
            }
            /** gets the state machine */
            public IEnumerator<T> getStateMachine()
            {
                return stateMachine;
            }
            /** Gets the state machines current info */
            public T Current()
            {
                return current;
            }
            /**
             * Checks to see if the state machine has been released
             */
            public bool isFreeState()
            {
                // If the statemachine is currently running
                if (stateMachine != null)
                {
                    // No more iterations
                    if (!stateMachine.MoveNext())
                    {
                        clean(); // Clean statemachine
                    }
                    else
                    {
                        current = stateMachine.Current; // Update current information
                        // Run time frequency should be adjusted externally
                        //MyGridProgram.Runtime.UpdateFrequency = UpdateFrequency.Once;
                        return false; // If there are elements still left
                    }
                }
                return true; // Code has either been freed or exists. Free to use elsewhere
            }

            /** Cleans up a state machine regardless of its position */
            public void clean()
            {
                if (stateMachine != null) { stateMachine.Dispose(); } // Dispose if not null
                stateMachine = null;  // Reset to null
            }

        }



        /*public int tickCount(UpdateFrequency check)
        {
            int n = (int)check;
            int MSB = ~(~n + 1) & n;
            int MSB1 = ~(~n + 1);
            Echo("MSB= " + MSB1);
            return ( (n / ((n & n) + (1 >> n))) // Divide by 0 check
                * ((( MSB & 4) >> 2) * 100) // Check by 100
                + (( MSB & 2) >> 1) * ((n & 1 * 10)) + (n & 1));
        }*/
        /** Calculates tickcount */
        public int tickCount(UpdateFrequency check)
        {
            return (check == UpdateFrequency.None) ? 0 : (check == UpdateFrequency.Update10) ? 10 : ((check == UpdateFrequency.Update100) ? 100 : 1);
        }
        /** 60 ticks = 1 sim second */
        public int tickToSec(int ticks)
        {
            return ticks / 60;
        }
        /*public void debugTickCount()
        {
            int check = 5 / ((5 & 5) + (1 >> 5));
            Echo("Check when n = 5 is: " + check);
            Echo("0 = " + tickCount(UpdateFrequency.None));
            Echo("1 = " + tickCount(UpdateFrequency.Once));
            Echo("2 = " + tickCount(UpdateFrequency.Update1));
            Echo("3 = " + tickCount(UpdateFrequency.Update10));
            Echo("4 = " + tickCount(UpdateFrequency.Update100));
        }*/




        /////////////////////////////////// JUNKYARD + UNSTABLE /////////////////////////////////////////////////

        /**
         * Building a list in 3 Dimensional space will be quite hard.
         * Not only do we need to remove duplicates from the picture, we need to make
         * sure that we check 6 directions simultaneously. We need an algo that
         * not only addresses duplicates but an organized data structure that can work with
         * LARGE counts of blocks, potentially in the 100,000's.
         */
        /*public void buildSlimBlocks2(List<IMySlimBlock> blocks, IMyCubeGrid grid)
        {
            blocks.Clear(); // Clear out the list before building
            blocks = new List<IMySlimBlock>(); // Make a new list
            Vector3I end = grid.Max; // ends with the max position of the element
            Vector3I start = grid.Min; // Computes the start min
            int rangeX = Math.Abs(start.X) + Math.Abs(end.X); // 0 - abs combined
            int rangeY = Math.Abs(start.Y) + Math.Abs(end.Y); // ^
            int rangeZ = Math.Abs(start.Z) + Math.Abs(end.Z); // ^
            Vector3I range = new Vector3I(rangeX, rangeY, rangeZ);
            List<AxisIterator> tracers = new List<AxisIterator>(); // Starts a list of tracers

        }*/



        /**
         * The max cord location is always the element that will always be the origin location.
         * This means that cords should be normalized to grid by subtracting the reference to the base.
         *
         * IN EXAMPLE, if the edge base is offset by n units, which is to become new origin,
         * then its actual position would be the Step - base in mins case, or Step + base in max case
         */
        public Vector3I normalizeToGrid(Vector3I stepVector, Vector3I baseCase)
        {
            return new Vector3I(stepVector.X - baseCase.X, stepVector.Y - baseCase.Y, stepVector.Z - baseCase.Z);
        }





        /**
        * Inefficiently builds the slim blocks by working cross section by cross section.
        * There is a better way using a path analysis but this far beyond the scope of what
        * a college student can provide.
        *
        * WE are trying to be quick and dirty, and in hindsight, this cost can be rectified
        * over the span of hours.
        */
        public IEnumerator<int> buildSlimBlocks1(List<IMySlimBlock> blocks, IMyCubeGrid grid)
        {
            Vector3I gridCenter = grid.WorldToGridInteger(grid.WorldVolume.Center);
            //grid.WorldVolume.Radius
            // Build a vector of range
            Vector3I range = buildRange(grid);
            int boundX = (int)(gridCenter.X + grid.WorldVolume.Radius);
            int boundY = (int)(gridCenter.Y + grid.WorldVolume.Radius);
            int boundZ = (int)(gridCenter.Z + grid.WorldVolume.Radius);
            int count = 0;
            // Iterate over x
            for (int x = grid.Min.X; x < grid.Max.X; x++)
            {
                // Iterate over y
                for (int y = grid.Min.Y; y < grid.Max.Y; y++)
                {
                    // Iterate over z
                    for (int z = grid.Min.Z; z < grid.Max.Z; z++)
                    {
                        try
                        {
                            IMySlimBlock result = grid.GetCubeBlock(new Vector3I(x, y, z)); // Attempt to get a cube block
                            if (result != null)
                            {
                                blocks.Add(result); // Add result
                                //blockCount++;
                            }

                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                //Echo("Block count = " + count);
                Echo("Building slim block layer " + x + " of max " + grid.Max.X);
                /*if (blocks.Count > 0)
                {
                    Echo("Block at 0 = " + blocks.ElementAt(0));
                }*/
                yield return count; // Return count of this cross section
            }
            yield return count; // End of
        }


        /**
         * Builds range of blocks based on grid
         */
        public Vector3I buildRange(IMyCubeGrid grid)
        {
            return new Vector3I(Math.Abs(grid.Max.X) + Math.Abs(grid.Min.X), Math.Abs(grid.Max.Y) + Math.Abs(grid.Min.Y), Math.Abs(grid.Max.Z) + Math.Abs(grid.Min.Z));
        }
        /**
         * This class is responsible for holding an axis iterator which shows positional data
         * of a tracer object as to how far this block has traversed against from the starting
         * blocks.
         *
         * By default, all axis iterators start at 0
         */
        public class AxisIterator
        {
            private int x; // x axis count
            private int y; // y axis count
            private int z; // z axis count
                           /** Constructor will call primary, will init all axis to 0 */
            public AxisIterator() : this(0, 0, 0) { }
            /** Primary constructor which initializes axis valus to passed params*/
            public AxisIterator(int axis1, int axis2, int axis3)
            {
                x = axis1;
                y = axis2;
                z = axis3;
            }
            /** Get the xy counter */
            public int getX() { return x; }
            /** Get the xz counter */
            public int getY() { return y; }
            /** Get the yz counter */
            public int getZ() { return z; }
            /**
             * Checks to see if the axis in question is either ahead or behind the other
             * Axis iterator is normalized to 0, which means grid cords must also follow suite!
             *
             * Left has priority over a null or greater iterator.
             */
            public bool intersectsOver(AxisIterator check)
            {
                // return check != null && getPythagStep >=
                return check == null || getPythagStep() >= check.getPythagStep();
            }
            /** set the xy */
            public void setX(int temp) { x = temp; }
            /** set the xz */
            public void setY(int temp) { y = temp; }
            /** Set the yz */
            public void setZ(int temp) { z = temp; }
            /**
             * Calculates the square of each axis + each other
             */
            public int getPythagStep()
            {
                return x * x + y * y + z * z;
            }
            /**
             * Checks if two axisIterators are equal
             */
            public bool equals(AxisIterator check)
            {
                return check != null && x == check.getX() && y == check.getY() && z == check.getZ();
            }
        }

        /**
         * This is a tracer class which is designed to traverse block grids building a block list.
         * A tracer class would be formed into each of the cardinal directions of 3 dimensional space
         * then proceed to check those with each respective axis two tracers are said to intersect
         * if their parent refference of an axis iterator is said to be
         */
        public class Tracer
        {
            private List<AxisIterator> tracers; // Axis iterators stored to list
            private int count; // A count of tracer objects
                               /** Constructor default*/
            public Tracer()
            {
                tracers = new List<AxisIterator>();
                count = 0;
            }
            /**
             * Attempts to add the iterator to the list
             */
            public void addIterator(AxisIterator iter)
            {
                tracers.Add(iter); // Add this iterator to the list
                count++;
                //tracersClear(); // Clean tracers
            }
            /**
             * Cleans tracers that intersect. This is done every insertion to prevent
             * overlaps
             */
            public void tracersClear()
            {
                /*int temp = 0;
                for (int i = count; i >= 0; i--)
                {

                }*/
            }


        }
        /** Writes text to a screen */
        public void writeScreen(IMyTextSurface item, bool append, String text)
        {

            Vector2 bounds = item.SurfaceSize;
            StringBuilder temp = new StringBuilder(text); // Build from text
            Vector2 strSize = item.MeasureStringInPixels(temp, item.Font, item.FontSize); // Bu
            // 39 is the number of chars that can fit on the screen at the 1x font, of a 520 x length screen
            int edge = (int)Math.Floor(39 * item.SurfaceSize.X / 520);
            int cutoff = (int)Math.Floor(edge / item.FontSize); // Floor int
            int timeCount = text.Length / cutoff; // Count the number of times this needs to be repeated
            // For each position at the cutoff
            for (int i = cutoff; i < text.Length; i += cutoff)
            {
                temp.Insert(i, '\n'); // Insert the newline character
            }
            // Append
            if (append)
            {
                // Get prior text, append with new line and new string
                item.WriteText(item.GetText() + '\n' + temp.ToString());
            }
            else { item.WriteText(temp.ToString()); }// Write to text surface
        }
        /** Write with pb screen configs */
        public void writeScreen(String text, bool append)
        {
            writeScreen(Me.GetSurface(0), append, text); // Use parent
        }
        /** echos and writes */
        public void echoWrite(String text)
        {
            echoWrite(text, false); // No append
        }
        /** Allows option for appending */
        public void echoWrite(String text, bool append)
        {
            Echo(text);
            writeScreen(text, append);
        }


}
/** Deploys an extensions class. Big thanks to MartinRWolfe */
static public class Extensions
{
    public static readonly MyDefinitionId Electricity = MyDefinitionId.Parse("MyObjectBuilder_GasProperties/Electricity");
    // size value of 1000d coverts to kw.
    static public double CurrentPowerUse(this IMyTerminalBlock terminalBlock, double size) =>
        ((terminalBlock.Components.Get<MyResourceSinkComponent>() != null) ? size * terminalBlock.Components.Get<MyResourceSinkComponent>().CurrentInputByType(Electricity) : 0);
    static public double MaxPowerUse(this IMyTerminalBlock terminalBlock, double size) => size * terminalBlock.Components.Get<MyResourceSinkComponent>().MaxRequiredInputByType(Electricity);
    static public float OptimalMaxOutput(this IMyPowerProducer powerBlock) => powerBlock.Components.Get<MyResourceSourceComponent>().DefinedOutput;
    //static public float MaxPutAct(this IMyPowerProducer powerBlock, IMyGridTerminalSystem script) => (!Closed(powerBlock, script) && powerBlock.IsWorking) ? OptimalMaxOutput(powerBlock): 0;
    static public float MaxPutAct(this IMyPowerProducer powerBlock) => (powerBlock.IsWorking) ? OptimalMaxOutput(powerBlock) : 0;
    static public bool IsShooting(this IMyUserControllableGun gun) => gun.Enabled && (gun.IsShooting || gun.GetValue<bool>("Shoot"));
    //static public bool Closed(this IMyTerminalBlock block, IMyGridTerminalSystem script) =>  (block == null || block.WorldMatrix == MatrixD.Identity || !(script.GetBlockWithId(block.EntityId) == block));