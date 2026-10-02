 /*Introduction
            ----------------------

            Hello and thank you for downloading Rdavs Easy Auto Turret Script 
            This Script adds a performance friendly easily setup
            completely automatic automation of rotor turrets system to a ship.
  
            Simply paste or install this code into the programmable block and refer
            to the custom-data of this block for easily accessible instructions
            and hints and tips.
             
            Additional information can be found on the workshop page for the script
            Any questions or queries don't hesitate to contact me!

            Rdav 07/08/2019

            ChangeLog:
             */

            //-------------- Don't Touch Anything Beneath This Line --------------------

            //Setup Of Code Constants
            static string VERSION = "001_REL"; //Script Version
            static string SCRIPTTITLE = "RDAVS AUTO TURRET";

            //If Initial Setup Has Run
            bool HASRUNSETUP = false;
            string LastRuntimeInfoMessage = "//Syst Reset//\nPlease refer to the setup \nguide in the custom data";

            //Items In Context
            List<RotorTurret> FunctionalTurrets = new List<RotorTurret>();
            List<IMyLargeTurretBase> DIRECTORS = new List<IMyLargeTurretBase>();
            IMyShipController CONTROLLER; //Required For Positional/Locational Reference

            //Logs Damaged Turrets
            int Damaged_Turrets = 0;

            //Options
            string sb = "	  Rdavs Automatic Rotor Turret Script, Hints Tips and Setup\n~~=====================================================~~\n\n\n\nSCRIPT OPTIONS / SETTINGS:\n==================================================\n(only change these if you know what you are doing!)\n\nPut a checkmark |x| in between the || symbols if you want an option turned ON,\nChange numeric or text values inbetween the symbols.\n(these settings will only take effect on script recompilation)\n\n@Only Use NAMED blocks       = ||    (Script only uses blocks with the #B# tag in name)\n@Reverse Turret orientation  = ||    (Script Reverses left/right default of turrets)\n@Turret Max rotation Speed   = |4|   (Max turn rate of the turrets)\n@Projectile Velocity         = |350| (Change this for different munitions)\n@Nametag if Named blocks     = |#B#|    (the name blocks need if option 1 is on)\n\n\n\nSCRIPT BASIC SETUP:\n==================================================\n1: Put this script in a programmable block\n\n2: Ensure that the grid this block is on has:\n   - at least 1 vanilla turret\n   - A cockpit/flight seat or remote control\n\n3: Set up turrets, all turrets require:\n   - Two rotors with the BASE on the grid this is on\n     (refer tutorial images if not sure)\n   - 1 Weapon of choice\n\n4: (optional) Set rotor limits on your turrets\n	(this enables resting positions)\n\n5: Recompile the script to initialise, script should\n   read a runtime indicator and automatically find all turrets on the grid\n\n\n\nSCRIPT FURTHER FEATURES:\n==================================================\n\n- To ONLY enable NAMED blocks and turrets be found by the script \n  (ie all blocks need #B# in their name somewhere) check the \n  OPTION at the top of this page & recompile.\n\n- to REVERSE a rotor (default is pointing right) add #LEFT#\n  to the rotors name\n\n- DUAL ROTOR turrets (ie one base rotor and 2 top rotors)\n  can be set up by making a turret and adding #LEFT#\n  to the left side rotor, the script will call it 2 turrets\n  as opposed to 1, as each side is independant of the other\n  (thus giving you some redundancy)\n\n- To CHANGE PROJECTILE VELOCITY (can be as low as 100m/s for rotor guns!)\n\n\nHINTS TIPS AND TRICKS & DIAGNOSTICSC\n==================================================\n\n- If your turret is trying to rotate in odd ways likely it's an issue\n  with rotor orientation, remember the turrets 'Forward' is when the\n  second rotor is facing RIGHT\n\n- The turrets to ensure low runtime do not account for objects in the \n  way, if the feature is requested a lot I will consider adding it\n\n- Fiddling with the turrets\n\n- Turrets will target and converge on the target found by any of the designators\n  thus unlike contemporary scripts the range of tracking is much more thorough\n";
            bool useOnlyNamedBlocks = false;
            bool ReverseTurretOrientation = false;
            double MaxTurretRotationalSpeed = 4;
            double ProjectileVelocity = 350;
            string blockTag = "#A#";

            //General Runtime
            //==============================================================

            //Main Method Runtime
            void Main(string argument)
            {

                //Sets Up Code Runtime Indicators
                //--------------------------------
                Operation_Bar.DisplayBar();

                //First Time Setup
                if (HASRUNSETUP == false)
                {
                    //Loads Settings
                    var Options = Me.CustomData.Split('@');
                    if (Options.Length < 4)
                    { Me.CustomData = sb; return; }

                    //Loads Options 
                    try { useOnlyNamedBlocks = Options[1].Split('|')[1] == "" ? false : true; } catch (Exception) { Echo("Error reading Option 1" + "Resetting Custom Data" + "\n"); Me.CustomData = sb; throw; }
                    try { ReverseTurretOrientation = Options[2].Split('|')[1] == "" ? false : true; } catch (Exception) { Echo("Error reading Option 2" + "Resetting Custom Data" + "\n"); Me.CustomData = sb; throw; }
                    try { MaxTurretRotationalSpeed = double.Parse(Options[3].Split('|')[1]); } catch (Exception) { Echo("Error reading Option 3" + "Resetting Custom Data" + "\n"); Me.CustomData = sb; throw; }
                    try { ProjectileVelocity = double.Parse(Options[4].Split('|')[1]); } catch (Exception) { Echo("Error reading Option 4" + "Resetting Custom Data" + "\n"); Me.CustomData = sb; throw; }
                    try { blockTag = (Options[5].Split('|')[1]); } catch (Exception) { Echo("Error reading Option 5" + "Resetting Custom Data" + "\n"); Me.CustomData = sb; throw; }

                    //All Setup Works, Go For it
                    Init_RotorTurret();
                    HASRUNSETUP = true;
                }

                //Checks For Null Core Blocks
                if (!RdavUtils.CheckNullblock(CONTROLLER)) { Echo("Missing A Flight Seat \nRemote Control Or \nCockpit, please install\nand recompile "); return; }
                if (DIRECTORS.Count < 1) { Echo("Ship lacks any spotting\nturrets, please install\nand recompile "); return; }

                //General Diagnostics
                Echo("\nGeneral Information:\n----------------------------");
                Echo("Runtime: " + Math.Round(Runtime.LastRunTimeMs, 3) + " Ms");
                Echo("Active Rotor Turrets: " + FunctionalTurrets.Count + " ");
                Echo("Disabled Rotor Turrets: " + Damaged_Turrets + " ");
                Echo("Active Designator Turrets: " + DIRECTORS.Count + " ");
                Echo("\n");
                //Echo("\nRecent Event: " + LastRuntimeInfoMessage);


                //Declares Director And Targeting Director
                //-----------------------------------------
                bool IsSystemTargeting = false;
                IMyLargeTurretBase ActiveDirector = null;
                foreach (var Director in DIRECTORS)
                {
                    if (Director.HasTarget)
                    {
                        IsSystemTargeting = true;
                        ActiveDirector = Director;
                        break;
                    }
                }

                //System Status Diag 
                string StatusReport = IsSystemTargeting ? "Track Target: " + "\n" + Vector3D.Round(((Vector3D)ActiveDirector.GetTargetedEntity().Position), 3) : " Idle \n";
                Echo("Syst Status: " + StatusReport +"\n");
                Echo("\nRecent Event: " + LastRuntimeInfoMessage);

                //Desets /Fires Weaponry/Tracks Target
                //------------------------------------------
                for (int i = 0; i < FunctionalTurrets.Count; i++)
                {
                    RotorTurret Turret = FunctionalTurrets[i];
                    if (!TrackAndTarget(Turret, IsSystemTargeting, ActiveDirector)) i--;
                } 
            }

            //Runs Tracking And Targeting For Each Turret
            bool TrackAndTarget(RotorTurret Turret, bool IsSystemTargeting, IMyLargeTurretBase ActiveDirector)
            {
                //Detects Any Missing Blocks
                if (!RdavUtils.CheckNullblock(Turret.AzimuthRotor)) {Damaged_Turrets++;  FunctionalTurrets.Remove(Turret); LastRuntimeInfoMessage = "Turret destroyed: missing rotor"; return false; }
                if (!RdavUtils.CheckNullblock(Turret.ElevationRotor)) {Damaged_Turrets++; FunctionalTurrets.Remove(Turret); LastRuntimeInfoMessage = "Turret destroyed: missing rotor"; return false; }
                if (!RdavUtils.CheckNullblock(Turret.PrimaryWeapon)) {Damaged_Turrets++; FunctionalTurrets.Remove(Turret); LastRuntimeInfoMessage = "Turret destroyed: weapons damaged"; return false; }

                //If System Tracking, Track And Fire
                if (IsSystemTargeting)
                {
                    Vector3D TargetPos = RdavUtils.GetPredictedTargetPosition2(Turret.PrimaryWeapon, CONTROLLER.GetShipVelocities().LinearVelocity, ActiveDirector.GetTargetedEntity(), (float)ProjectileVelocity); //350
                    TurretControl(Turret, TargetPos);

                    //Shoots
                    if (Vector3D.Dot(Vector3D.Normalize(TargetPos - Turret.PrimaryWeapon.GetPosition()), Turret.PrimaryWeapon.WorldMatrix.Forward) > 0.97)
                    {
                        foreach (var weapon in Turret.Weaponry)
                        {
                            if (!(weapon as IMyUserControllableGun).IsShooting)
                            {
                                weapon.ApplyAction("Shoot_On");
                            }
                        }
                    }

                    //Doesn't shoot
                    else
                    {
                        foreach (var weapon in Turret.Weaponry)
                        {
                            if ((weapon as IMyUserControllableGun).IsShooting)
                            {
                                weapon.ApplyAction("Shoot_Off");
                            }
                        }
                    }
                }

                //Else Desests Weaponry And Turret Control.
                else
                {
                    foreach (var weapon in Turret.Weaponry)
                    {
                        if ((weapon as IMyUserControllableGun).IsShooting)
                        { weapon.ApplyAction("Shoot_Off"); }
                    }
                    (Turret.AzimuthRotor as IMyMotorStator).TargetVelocityRad = 0;
                    (Turret.ElevationRotor as IMyMotorStator).TargetVelocityRad = 0;
                }

                //If Not Targeting Go to Rest Position
                if (!IsSystemTargeting)
                {REST(Turret);}

                return true;
            }

            //Cycles Through All Rotor Turrets and Adds new To Stack (one time op, performance intensive)
            void Init_RotorTurret()
            {

                //Finds Required Blocks
                List<IMyMotorStator> ROTORS = new List<IMyMotorStator>();
                GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(ROTORS);
                List<IMyTerminalBlock> UsedGuns = new List<IMyTerminalBlock>();

                //Finds Ship Primary Controller
                List<IMyShipController> Controllers = new List<IMyShipController>();
                GridTerminalSystem.GetBlocksOfType<IMyShipController>(Controllers);

                //Finds All Potential Designation Turrets
                GridTerminalSystem.GetBlocksOfType<IMyLargeTurretBase>(DIRECTORS); //, b => b.CubeGrid == Me.CubeGrid

                List<IMyTerminalBlock> DIRECTIONAL_FIRE = new List<IMyTerminalBlock>();  //Directional ship weaponry
                GridTerminalSystem.GetBlocksOfType<IMyUserControllableGun>(DIRECTIONAL_FIRE, (block => block.GetType().Name == "MySmallMissileLauncher" || block.GetType().Name == "MySmallGatlingGun" || block.GetType().Name == "MySmallMissileLauncherReload")); //Collects the directional weaponry (in a group)

                //Removes if Not Matching Name
                if (useOnlyNamedBlocks)
                {
                    ROTORS.RemoveAll(a => a.CustomName.Contains(blockTag) == false);
                    Controllers.RemoveAll(a => a.CustomName.Contains(blockTag) == false);
                    DIRECTORS.RemoveAll(a => a.CustomName.Contains(blockTag) == false);
                    DIRECTIONAL_FIRE.RemoveAll(a => a.CustomName.Contains(blockTag) == false);
                }

                //Iterates Through List To Find Complete Rotor Turret
                //Connected to main ship through 2 rotors
                if (Controllers.Count > 0) { CONTROLLER = Controllers[0] as IMyShipController; }
                foreach (var Key_Weapon in DIRECTIONAL_FIRE)
                {
                    if (UsedGuns.Contains(Key_Weapon) == true)
                    { continue; }

                    //Initialises new Turret
                    RotorTurret NewTurret = new RotorTurret();

                    //Identifies If It Is A Turret
                    IMyMotorStator Rotor1 = ROTORS.Find(x => x.TopGrid == Key_Weapon.CubeGrid);
                    if (Rotor1 == null) { continue; }
                    IMyMotorStator Rotor2 = ROTORS.Find(x => x.TopGrid == Rotor1.CubeGrid && x.CubeGrid == Me.CubeGrid);
                    if (Rotor2 == null) { continue; }

                    //Initialises Turret
                    NewTurret.PrimaryWeapon = Key_Weapon; //Assigns Based On Weapon
                    NewTurret.AzimuthRotor = Rotor2;
                    NewTurret.ElevationRotor = Rotor1;
                    NewTurret.Weaponry = DIRECTIONAL_FIRE.FindAll(x => x.CubeGrid == Key_Weapon.CubeGrid);
                    NewTurret.IsLeftRotor = Rotor1.CustomName.Contains("#LEFT#") ? true : false;
                    UsedGuns.AddRange(NewTurret.Weaponry);

                    //Inverts If Default Invert Setting Is Chosen
                    NewTurret.IsLeftRotor = ReverseTurretOrientation ? !NewTurret.IsLeftRotor : NewTurret.IsLeftRotor;

                    //Status All Ship Blocks Found, Init Complete, Generating Class
                    FunctionalTurrets.Add(NewTurret);
                }

            }

            //(on arm removal lock out rotors)

            //Arm Specific Functions
            //===============================================================

            //Base RotorTurret class
            class RotorTurret
            {
                //Terminal Blocks On Each Turret
                public IMyTerminalBlock PrimaryWeapon; //Primary Weapon Of The Group
                public IMyTerminalBlock AzimuthRotor; //
                public IMyTerminalBlock ElevationRotor;
                public List<IMyTerminalBlock> Weaponry = new List<IMyTerminalBlock>();
                public bool IsLeftRotor;
            }

            //Used For Individual Turret Pointing
            void TurretControl(RotorTurret Turret, Vector3D AimPos)
            {

                //Retrieves Forward Direction Of Arm
                Vector3D CurrentFireDirection = Turret.PrimaryWeapon.WorldMatrix.Forward;
                Vector3D DesiredTargetVector = Vector3D.Normalize(AimPos - Turret.PrimaryWeapon.GetPosition());
                Vector3D Down = Turret.AzimuthRotor.WorldMatrix.Down;
                if (Turret.IsLeftRotor) { Down = Down * -1; }

                //Sets Rotor Angles
                double CurrentArmAngle = MathHelper.Clamp((Vector3D.Dot(CurrentFireDirection, DesiredTargetVector)), -1, 1);
                double ArmHemiSphereAngle = Vector3D.Dot(Vector3D.Cross(CurrentFireDirection, DesiredTargetVector), Down);
                double CurrentArmAngleError = (-Math.Acos(CurrentArmAngle)) * Math.Sign(ArmHemiSphereAngle);
                if (Turret.IsLeftRotor) { CurrentArmAngleError = CurrentArmAngleError * -1; }

                double CurrentUpperAngle = Math.Acos(Vector3D.Dot(CurrentFireDirection, -Down));
                double DesiredUpperAngle = Math.Acos(Vector3D.Dot(DesiredTargetVector, -Down));
                double CurrentUpperAngleError = (CurrentUpperAngle - DesiredUpperAngle);

                if (CurrentUpperAngleError == double.NaN ||double.IsInfinity(CurrentUpperAngleError))
                {CurrentUpperAngleError = 0; LastRuntimeInfoMessage = "PepeSuspicious"; }
                if (CurrentArmAngleError == double.NaN || double.IsInfinity(CurrentArmAngleError))
                { CurrentArmAngleError = 0; LastRuntimeInfoMessage = "PepeSuspicious"; }

                (Turret.AzimuthRotor as IMyMotorStator).TargetVelocityRad = MathHelper.Clamp(-(float)CurrentArmAngleError * 4, -1 * (float)MaxTurretRotationalSpeed, (float)MaxTurretRotationalSpeed);
                (Turret.ElevationRotor as IMyMotorStator).TargetVelocityRad = MathHelper.Clamp(-(float)CurrentUpperAngleError * 4, -1 * (float)MaxTurretRotationalSpeed, (float)MaxTurretRotationalSpeed);

            }

            //Used For Resting The Turret Back In Place
            void REST(RotorTurret Turret)
            {
                //Only Runs If a User Has Set It Up
                IMyMotorStator AZ = Turret.AzimuthRotor as IMyMotorStator;
                IMyMotorStator EL = Turret.ElevationRotor as IMyMotorStator;

                if (AZ.UpperLimitDeg > 359 || AZ.LowerLimitDeg < -359 || EL.UpperLimitDeg > 359 || EL.LowerLimitDeg < -359)
                { return; }

                //Creating An Average Rest Position
                Single AZ_AVERAGE = (AZ.UpperLimitDeg + AZ.LowerLimitDeg) / 2;
                Single EL_AVERAGE = (EL.UpperLimitDeg + EL.LowerLimitDeg) / 2;

                Single CURRENTAZ = ((float)57.2958 * AZ.Angle);
                Single CURRENTEL = ((float)57.2958 * EL.Angle);

                Single AZOVER = (AZ_AVERAGE - CURRENTAZ);
                Single ELOVER = (EL_AVERAGE - CURRENTEL);

                //Rests and Relaxes Rotors (Rotor Lock Optional )
                if (AZOVER * AZOVER < 20 && ELOVER * ELOVER < 20)
                { if (AZ.TargetVelocityRad != 0) { AZ.TargetVelocityRad = 0; } if (EL.TargetVelocityRad != 0) { EL.TargetVelocityRad = 0; } return;}

                if (Math.Abs(AZOVER) > MaxTurretRotationalSpeed)
                { AZ.TargetVelocityRad = (float)MaxTurretRotationalSpeed/5 * Math.Sign(AZOVER); }
                else
                { AZ.TargetVelocityRad = AZOVER / 100; }

                if (Math.Abs(ELOVER) > MaxTurretRotationalSpeed)
                { EL.TargetVelocityRad = (float)MaxTurretRotationalSpeed/5 * Math.Sign(ELOVER); }
                else
                { EL.TargetVelocityRad = ELOVER / 100; }

            }

            //General Functions For Every Script
            //================================================================

            //Adds A Runtime Bar To The Operation
            class Operation_Bar
            {


                //Operation Bar
                static public Program thisprog;
                static string[] FUNCTION_BAR = new string[] { "", " ===||===", " ==|==|==", " =|====|=", " |======|", "  ======" };
                static int FUNCTION_TIMER = 4;
                static string Thistitle;
                static string ThisVersion;

                public Operation_Bar(Program ThisProgram_, string Thistitle_, string ThisVersion_)
                {
                    thisprog = ThisProgram_;
                    Thistitle = Thistitle_;
                    ThisVersion = ThisVersion_;
                }

                //Runs Operation
                static public void DisplayBar()
                {
                    FUNCTION_TIMER++;
                    thisprog.Echo("     ~" + Thistitle + "~  \n               " + FUNCTION_BAR[FUNCTION_TIMER] + "");
                    thisprog.Echo("         Version: " + ThisVersion +"\n");
                    if (FUNCTION_TIMER == 5) { FUNCTION_TIMER = 0; }

                }
            }

            //Utils For Maths Functions
            static class RdavUtils
            {

                //Checks For a Null Block
                public static bool CheckNullblock(IMyTerminalBlock block)
                {
                    if (block == null || block.CubeGrid.GetCubeBlock(block.Position) == null)
                    { return false; }
                    return true;
                }

                //Use For Solutions To Quadratic Equation
                public static bool Quadractic_Solv(double a, double b, double c, out double x1, out double x2)
                {
                    //Default Values
                    x1 = 0;
                    x2 = 0;

                    //Discrim Check
                    Double Discr = b * b - 4 * c;
                    if (Discr < 0)
                    { return false; }

                    //Calcs Values
                    else
                    {
                        x1 = (-b + Math.Sqrt(Discr)) / (2 * a);
                        x2 = (-b - Math.Sqrt(Discr)) / (2 * a);
                    }
                    return true;
                }

                //Handles Calculation Of Area Of Diameter
                public static double CalculateArea(double OuterDiam, double InnerDiam)
                {
                    //Handles Calculation Of Area Of Diameter
                    //=========================================
                    double PI = 3.14159;
                    double Output = ((OuterDiam * OuterDiam * PI) / 4) - ((InnerDiam * InnerDiam * PI) / 4);
                    return Output;
                }

                //Use For Magnitudes Of Vectors In Directions
                public static double Vector_Projection(Vector3D IN, Vector3D Axis)
                {
                    double OUT = 0;
                    OUT = Vector3D.Dot(IN, Axis) / IN.Length();
                    if (OUT + "" == "NaN")
                    { OUT = 0; }
                    return OUT;
                }

                //Use For Intersections Of A Sphere And Ray
                public static bool SphereIntersect_Solv(BoundingSphereD Sphere, Vector3D LineStart, Vector3D LineDirection, out Vector3D Point1, out Vector3D Point2)
                {
                    //starting Values
                    Point1 = new Vector3D();
                    Point2 = new Vector3D();

                    //Spherical intersection
                    Vector3D O = LineStart;
                    Vector3D D = LineDirection;
                    Double R = Sphere.Radius;
                    Vector3D C = Sphere.Center;

                    //Calculates Parameters
                    Double b = 2 * (Vector3D.Dot(O - C, D));
                    Double c = Vector3D.Dot((O - C), (O - C)) - R * R;

                    //Calculates Values
                    Double t1, t2;
                    if (!Quadractic_Solv(1, b, c, out t1, out t2))
                    { return false; } //does not intersect
                    else
                    {
                        Point1 = LineStart + LineDirection * t1;
                        Point2 = LineStart + LineDirection * t2;
                        return true;
                    }
                }

                //Basic Gets Predicted Position Of Enemy (Derived From Keen Code)
                public static Vector3D GetPredictedTargetPosition2(IMyTerminalBlock shooter, Vector3 ShipVel, MyDetectedEntityInfo target, float shotSpeed)
                {
                    Vector3D predictedPosition = target.Position;
                    Vector3D dirToTarget = Vector3D.Normalize(predictedPosition - shooter.GetPosition());

                    //Run Setup Calculations
                    Vector3 targetVelocity = target.Velocity;
                    targetVelocity -= ShipVel;
                    Vector3 targetVelOrth = Vector3.Dot(targetVelocity, dirToTarget) * dirToTarget;
                    Vector3 targetVelTang = targetVelocity - targetVelOrth;
                    Vector3 shotVelTang = targetVelTang;
                    float shotVelSpeed = shotVelTang.Length();

                    if (shotVelSpeed > shotSpeed)
                    {
                        // Shot is too slow 
                        return Vector3.Normalize(target.Velocity) * shotSpeed;
                    }
                    else
                    {
                        // Run Calculations
                        float shotSpeedOrth = (float)Math.Sqrt(shotSpeed * shotSpeed - shotVelSpeed * shotVelSpeed);
                        Vector3 shotVelOrth = dirToTarget * shotSpeedOrth;
                        float timeDiff = shotVelOrth.Length() - targetVelOrth.Length();
                        var timeToCollision = timeDiff != 0 ? ((shooter.GetPosition() - target.Position).Length()) / timeDiff : 0;
                        Vector3 shotVel = shotVelOrth + shotVelTang;
                        predictedPosition = timeToCollision > 0.01f ? shooter.GetPosition() + (Vector3D)shotVel * timeToCollision : predictedPosition;
                        return predictedPosition;
                    }
                }

            }

            //QuickEcho Function
            void QuickEcho(object This, string Title = "")
            {
                if (This is Vector3D)
                { Echo(Title + " " + Vector3D.Round(((Vector3D)This), 3)); }
                else if (This is double)
                { Echo(Title + " " + Math.Round(((double)This), 3)); }
                else
                { Echo(Title + " " + This); }
            }

            //Code Constructor For Initialisation
            public Program()
            { Runtime.UpdateFrequency = UpdateFrequency.Update10; Operation_Bar ThisBar = new Operation_Bar(this, SCRIPTTITLE, VERSION );}

            //End Of Script//
