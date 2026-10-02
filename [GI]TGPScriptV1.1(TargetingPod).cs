#region Script


        /**********************************************************************************************
         * 
         *  GENERAL INDUSTRIES PROUDLY PRESENTS:
         * 
         *         VANILLA TGP SCRİPT 
         *         Version: 1.1
         *
         *  This script controls rotors and hinges to make a Targetting Pod in SE while providing gravity aligned camera stabilization.
         *
         **********************************************************************************************
         *
         *  HOW TO USE:
         *  1-) Rename blocks as the block names or vice versa.
         *  2-) You can specify which of the cockpits or remote controls will control the pod by adding the tag in their names.
         *  3-) Spare a toolbar for arguments and set up the toolbar.
         *  4-) Have fun!
         *  5-) Don't hesitate to ask for help, report bugs or make requests. You can contact me at Discord: https://discord.gg/gxSyRAvXGh
         *  6-) You can take a look at the example from the workshop page.
         *  
         **********************************************************************************************
         *  
         *  MODES AND ARGUMENTS:
         *  Control:
         *      Enables or disables the control of the targetting pod. And also overrides the gyros in the grid.
         *      
         *  Manual:
         *      No tracking. The camera just moves as you control it.
         *  
         *  SearchMode:
         *      Toggles the Search Mode. Search mode locks and stabilizes the camera to the last point the input has been given. 
         *      If you control the camera it follows your commands, if you don't it keeps tracking where you are looking. (5KM limit. Returns to manual if out of limit)
         *      Note: Does not work in space. If argument is given it will automatically turn back to Manual.
         *
         *  Point Track:
         *      Disables player control. Player can only switch between pre added points or return to SearchMode to add more points.
         *      Note: Does not work in space. If argument is given it will automatically turn back to Manual.
         *  
         *  AddPos:
         *      Switches the state to Point Track and adds the current look position to the list.
         *      
         *  DelPos:
         *      Deletes the current position from the list.
         *      
         *  NextPos: 
         *      Switches to the next position in the list.
         *      
         *  PrevPos: 
         *      Switches to the previous position in the list.
         *      
         ********************************************************************************************** 
         *      
         *  Inputtin or outputting GPS coordinates:
         *  GetPos:
         *      Outputs the current look position from the list to the PB's custom data.
         *      
         *  Inputting GPS coordinates as arguments:
         *      Adds the coordinate's position to the list.
         *      
         **********************************************************************************************
         *  NOTE: You can use this script in your projects, modify it, reupload it to the workshop as long as you give credit to the workshop page of the original.
         */




        //Block Names
        readonly string cameraName = "PodCamera";
        readonly string azimuthRotorName = "AzimuthRotor";
        readonly string elevationHingeName = "ElevationHinge";
        readonly string stabilizerRotorName = "StabilizerRotor";
        readonly string cockpitTag = "[TGP]";

        //Speed Values
        readonly float azimuthSpeed = 60;
        readonly float elevationSpeed = 60;
        readonly float stabilizerSpeedMultiplier = 10; 
        readonly float controlSpeed = 0.002f;

        //PID Controller Values (Don't change if you dont know what your doing!)
        readonly float proportionalGain = 3; //This one affects the aggressiveness of the tracking.
        readonly float integralGain = 0f; //This one affects the secondary agressiveness. (Happens when speed can't reach to target and error keeps getting bigger.)
        readonly float integralSaturation = 1; //Affects secondary agressiveness.
        readonly float derivativeGain = 0; //This one affects dampening.



        // ------------------------------------------
        // DO NOT CHANGE ANYTHING BELOW THIS LINE!!!!
        // ------------------------------------------

        bool debugging = false;
        DebugAPI debug;



        List<IMyTerminalBlock> tempBlocks = new List<IMyTerminalBlock>();
        List<IMyGyro> Gyros = new List<IMyGyro>();

        public static WcPbApi WCAPI;

        bool willWork = true;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            if (debugging)
            debug = new DebugAPI(this);
            
            
            podCamera = GridTerminalSystem.GetBlockWithName(cameraName) as IMyCameraBlock;
            if (podCamera == null)
            {
                Echo($"Error: Camera '{cameraName}' not found! Make sure you entered the camera name right.");
                willWork = false;
            }
            else
            {
                podCamera.EnableRaycast = true;
            }

            azimuthRotor = GridTerminalSystem.GetBlockWithName(azimuthRotorName) as IMyMotorStator;
            if (azimuthRotor == null)
            {
                Echo($"Error: Azimuth Rotor '{azimuthRotorName}' not found! Make sure you entered the rotor name right.");
                willWork = false;
            }

            elevationHinge = GridTerminalSystem.GetBlockWithName(elevationHingeName) as IMyMotorStator;
            if (elevationHinge == null)
            {
                Echo($"Error: Elevation Hinge '{elevationHingeName}' not found! Make sure you entered the hinge name right.");
                willWork = false;
            }

            stabilizerRotor = GridTerminalSystem.GetBlockWithName(stabilizerRotorName) as IMyMotorStator;
            if (stabilizerRotor == null)
            {
                Echo($"Error: Stabilizer Rotor '{stabilizerRotorName}' not found! Make sure you entered the rotor name right.");
                willWork = false;
            }

            //Getting all the cockpits
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(tempBlocks);
            //Checking if there's any cockpit
            if (!tempBlocks.Any())
            {
                Echo("There's no cockpit in the grid! Make sure there's at least one ship controller (Cockpit or Remote Control) in the grid.");
                willWork = false;
            }
            else
            {
                //Getting the ones with tags
                foreach (var shipController in tempBlocks)
                {
                    if (shipController.Name.Contains(cockpitTag))
                    {
                        shipControllers.Add(shipController as IMyShipController);
                    }
                }
                //Getting all of them if there's no tagged ones
                if (!shipControllers.Any())
                {
                    foreach (var shipController in tempBlocks)
                    {
                        shipControllers.Add(shipController as IMyShipController);
                    }
                }
                gravityVector = shipControllers[0].GetNaturalGravity().Normalized();
                lookVector = stabilizerRotor.WorldMatrix.Up;
                crossVector = Vector3D.Cross(lookVector, gravityVector).Normalized() * 2;
            }
            tempBlocks.Clear();


            //Getting all the gyros in the grid.
            GridTerminalSystem.GetBlocksOfType<IMyGyro>(tempBlocks);
            foreach (var item in tempBlocks)
            {
                Gyros.Add(item as IMyGyro);
            }

        }

        IMyCameraBlock podCamera;

        IMyMotorStator azimuthRotor;
        IMyMotorStator elevationHinge;
        IMyMotorStator stabilizerRotor;

        List<IMyShipController> shipControllers = new List<IMyShipController>();

        bool control;
        bool gyroOverriding;
        Vector3D manualControlVector = Vector3D.Zero;

        Vector3D tempSearchPoint = Vector3D.Zero;

        List<Vector3D> trackingPositions = new List<Vector3D>();
        int currentTrackingPosition;

        

        Vector3D gravityVector;
        Vector3D lookVector;
        Vector3D crossVector;

        
        float workingFor = 0;
        public void Main(string argument, UpdateType updateSource)
        {
            if (debugging)
                debug.RemoveDraw();

            if (!willWork)
            {
                Echo("Run program again after fixing the problems above.");
                return;
            }

            ControlByState(argument);

            //AdvencedControl();
            //Vector3D aimpos = elevationRotor.GetPosition() + manualControlVector.Normalized();
            //AimWithPID(aimpos);

            ////debug.DrawLine(elevationRotor.GetPosition(), elevationRotor.GetPosition() + manualControlVector * 3, Color.Yellow)

            //debug.DrawSphere(new BoundingSphereD(aimpos, 0.02f), Color.Orange);

            DoGravityStabilization();

            Echo("Been working for: " + ++workingFor);            
        }

        Vector2 GetInput()
        {
            Vector2 input = new Vector2(0, 0);
            foreach (var shipController in shipControllers)
            {
                input.X += shipController.RotationIndicator.X;
                input.Y += shipController.RotationIndicator.Y;
            }
            return input * controlSpeed;
        }

        float integrationStored = 0;
        float lastAzimuthError;
        float lastElevationError;
        float errorRateOfChange;
        enum PIDCalculationType
        {
            azimuth,
            elevation
        }
        float PID(float error, PIDCalculationType type, Vector3D target)
        {
            float distance = (float)Vector3D.Distance(target, elevationHinge.GetPosition());
            float distanceFactor = MyMath.Clamp(1000 / distance, 0.01f, 1f);
            
            
            //calculate P
            float P = error * proportionalGain * distanceFactor;

            //Calculate D
            lastAzimuthError = CalculateAzimuthAngle(target);
            lastElevationError = CalculateElevationAngle(target);

            switch (type)
            {
                case PIDCalculationType.azimuth:
                    errorRateOfChange = (error - lastAzimuthError);
                    break;
                case PIDCalculationType.elevation:
                    errorRateOfChange = (error - lastElevationError);
                    break;
            }
            float D = errorRateOfChange * derivativeGain;

            //Calculate I
            integrationStored = MyMath.Clamp(integrationStored + error, -integralSaturation, integralSaturation);
            float I = integralGain * integrationStored;


            return P + I + D;
        }

        void AimWithPID(Vector3D target)
        {
            // Azimuth'u döndür
            float azimuthError = CalculateAzimuthAngle(target);
            float azimuthTargetSpeed = MyMath.Clamp(PID(azimuthError, PIDCalculationType.azimuth, target), -azimuthSpeed, azimuthSpeed);
            azimuthRotor.TargetVelocityRPM = azimuthTargetSpeed;

            // Elevation'ı döndür
            float elevationError = CalculateElevationAngle(target);
            float elevationTargetSpeed = MyMath.Clamp(PID(elevationError, PIDCalculationType.elevation, target), -elevationSpeed, elevationSpeed);
            elevationHinge.TargetVelocityRPM = elevationTargetSpeed;
        }

        void DoGravityStabilization()
        {
            if (shipControllers[0].GetNaturalGravity().Length() < 0.001)
            {
                stabilizerRotor.RotateToAngle(MyRotationDirection.AUTO, 0, 60);
                return;
            }

            stabilizerRotor.LowerLimitDeg = float.MinValue;
            stabilizerRotor.UpperLimitDeg = float.MaxValue;

            // İki vektör arasındaki açıyı hesapla
            Vector3 targetDirection = shipControllers[0].GetNaturalGravity().Normalized();
            Vector3 currentDirection = podCamera.WorldMatrix.Down.Normalized();

            Vector3 rotorAxis = -stabilizerRotor.WorldMatrix.Up;
            Vector3 projectedTargetDirection = Vector3.ProjectOnPlane(ref targetDirection, ref rotorAxis).Normalized();
            float stabilizerTargetAngle = -SignedAngleBetweenVectors(-projectedTargetDirection, currentDirection, rotorAxis);

            stabilizerRotor.TargetVelocityRPM = stabilizerTargetAngle * stabilizerSpeedMultiplier;
        }

        float CalculateAzimuthAngle(Vector3D target)
        {
            // İki vektör arasındaki açıyı hesapla
            Vector3D targetDirection = ((target) - elevationHinge.GetPosition()).Normalized();
            Vector3D currentDirection = elevationHinge.WorldMatrix.Backward.Normalized();

            Vector3D rotorAxis = azimuthRotor.WorldMatrix.Up.Normalized();
            
            Vector3D projectedTargetDirection = Vector3D.ProjectOnPlane(ref targetDirection, ref rotorAxis).Normalized();
            float projectedAngleError = SignedAngleBetweenVectors(-projectedTargetDirection, currentDirection, rotorAxis);

            return projectedAngleError;
        }

        float CalculateElevationAngle(Vector3D target)
        {
            // İki vektör arasındaki açıyı hesapla
            Vector3D targetDirection = ((target) - elevationHinge.GetPosition()).Normalized();
            Vector3D currentDirection = stabilizerRotor.WorldMatrix.Up.Normalized();

            Vector3D rotorAxis = elevationHinge.WorldMatrix.Up.Normalized();
            Vector3D projectedTargetDirection = Vector3D.ProjectOnPlane(ref targetDirection, ref rotorAxis).Normalized();
            float projectedAngleError = SignedAngleBetweenVectors(-projectedTargetDirection, currentDirection, rotorAxis);

            return projectedAngleError;
        }

        public float SignedAngleBetweenVectors(Vector3D vector1, Vector3D vector2, Vector3D axis)
        {
            // Vektörleri normalize et
            Vector3D v1Normalized = vector1.Normalized();
            Vector3D v2Normalized = vector2.Normalized();

            // Dot product'ı hesapla
            float dotProduct = (float)Vector3D.Dot(v1Normalized, v2Normalized);

            // Dot product'ı -1 ile 1 arasında tutarak güvenli hale getir
            dotProduct = MyMath.Clamp((float)dotProduct, -1.0f, 1.0f);

            // Arccos ile açıyı bul (radyan cinsinden)
            float angleInRadians = (float)Math.Acos(dotProduct);

            // Çapraz çarpımı hesapla (cross product)
            Vector3D crossProduct = Vector3D.Cross(v1Normalized, v2Normalized);

            // Eksenle cross product'ın aynı yönde olup olmadığını kontrol et
            float sign = Vector3D.Dot(crossProduct, axis) < 0 ? -1.0f : 1.0f;

            // Açıyı dereceye çevir
            float angleInDegrees = angleInRadians * (180.0f / (float)Math.PI);
            angleInDegrees = angleInDegrees - 180;
            // İşaretli açıyı döndür (derece cinsinden)
            return angleInDegrees * sign;
        }

        void BasicControl()
        {
            Vector2 input = GetInput() / controlSpeed;

            azimuthRotor.TargetVelocityRPM = input.Y;
            elevationHinge.TargetVelocityRPM = input.X;
        }

        Vector3D AdvencedControl()
        {
            Vector2 input = GetInput();

            if (manualControlVector == Vector3D.Zero)
            {
                manualControlVector = stabilizerRotor.WorldMatrix.Up;
            }
            else if (input.X != 0 || input.Y != 0)
            {
                
                //No gravity
                if (shipControllers[0].GetNaturalGravity().Length() < 0.001)
                {
                    
                }
                //Gravity
                else
                {
                    Vector3D verticalAxis = shipControllers[0].GetNaturalGravity().Normalized();
                    Vector3D horizontalAxis = Vector3D.Cross(manualControlVector, verticalAxis).Normalized();

                    //debug.DrawLine(elevationRotor.GetPosition(), horizontalAxis, Color.Yellow, 0.02f, 1);
                    //debug.DrawLine(elevationRotor.GetPosition(), verticalAxis, Color.Red, 0.02f, 1);
                    //debug.PrintChat("Elevation Rotor Position: " + elevationRotor.GetPosition());

                    // Quaternion kullanarak eksen etrafında döndürme işlemi oluştur
                    Quaternion rotation1 = Quaternion.CreateFromAxisAngle(horizontalAxis.Normalized(), input.X);
                    Quaternion rotation2 = Quaternion.CreateFromAxisAngle(verticalAxis.Normalized(), input.Y);

                    //Debugging
                    //Sarı yukarı bakacak
                    if (debugging)
                        debug.DrawLine(elevationHinge.GetPosition(), elevationHinge.GetPosition() + verticalAxis, Color.Yellow, 0.02f, 1);

                    //kırmızı sarıya dikey olacak
                    if (debugging)
                        debug.DrawLine(elevationHinge.GetPosition(), elevationHinge.GetPosition() + horizontalAxis, Color.Red, 0.02f, 1);

                    // Vektörü döndürme
                    manualControlVector = (rotation1 * manualControlVector).Normalized();
                    manualControlVector = (rotation2 * manualControlVector).Normalized();
                    //Physics.Raycast(elevationHead.position, elevationHead.forward, out RaycastHit hit);
                }
            }

            //Mavi bizim bakmamız gereken Manual vector
            if (debugging)
                debug.DrawLine(elevationHinge.GetPosition(), elevationHinge.GetPosition() + manualControlVector, Color.Blue, 0.02f, 1);

            return elevationHinge.GetPosition() + manualControlVector.Normalized();
        }

        enum TGPState
        {
            Test,
            Manual,
            Search,
            PointTrack
        }

        TGPState state = TGPState.Manual;
        void ControlByState(string argument = "")
        {
            if (argument != "")
            {
                if (argument == "SearchMode")
                {
                    if (state != TGPState.Search)
                    {
                        state = TGPState.Search;
                        manualControlVector = Vector3D.Zero;
                        MyDetectedEntityInfo hitInfo = podCamera.Raycast(5000);

                        if (!hitInfo.IsEmpty()) tempSearchPoint = (Vector3D)hitInfo.HitPosition;

                        if (debugging)
                            debug.PrintChat("State has been set to Search Mode");
                    }
                    else
                    {
                        state = TGPState.Manual;
                        if (debugging)
                            debug.PrintChat("State has been set to Manual");
                    }
                }

                if (argument == "AddPos")
                {
                    MyDetectedEntityInfo hitInfo = podCamera.Raycast(5000);
                    
                    if (!hitInfo.IsEmpty())
                    {
                        trackingPositions.Add((Vector3D)hitInfo.HitPosition);
                        currentTrackingPosition = trackingPositions.Count - 1;
                        state = TGPState.PointTrack;
                        if (debugging)
                            debug.PrintChat("A new point added to the " + currentTrackingPosition + "th index and tracking.");
                    }
                    else
                    {
                        if (debugging)
                            debug.PrintChat("Point couldn't be added.");
                    }
                    
                }

                if (argument.Length > 10)
                {
                    string[] str = argument.Split(':');
                    if (str[0] == "GPS")
                    {
                        var pos = new Vector3D(Convert.ToDouble(str[2]), Convert.ToDouble(str[3]), Convert.ToDouble(str[4]));
                        trackingPositions.Add(pos);
                        currentTrackingPosition = trackingPositions.Count - 1;
                    }
                }

                if (argument == "GetPos")
                {
                    if (trackingPositions.Count == 0)
                    {
                        Me.CustomData = "There is no point on the list.\nSo no point was able to print";
                    }
                    else
                    {
                        string str = "GPS:" + currentTrackingPosition + "TH Point:" +
                           trackingPositions[currentTrackingPosition].X + ":" +
                           trackingPositions[currentTrackingPosition].Y + ":" +
                           trackingPositions[currentTrackingPosition].Z + ":" + "#F17575";
                        Me.CustomData = "Printed Position:\n" + str;
                    }
                   
                }

                if (argument == "DelPos")
                {
                    if (trackingPositions.Count > 1)
                    {
                        if (currentTrackingPosition + 1 == trackingPositions.Count) // Son elemandaysanız
                        {
                            trackingPositions.RemoveAt(currentTrackingPosition);
                            currentTrackingPosition--; // Silinen eleman sonrası bir önceki indexe geçiyoruz.
                            if (debugging)
                                debug.PrintChat("The point from the " + (currentTrackingPosition + 1) + "th index has been deleted. \n" +
                                                "Current index is: " + currentTrackingPosition);
                        }
                        else if (currentTrackingPosition == 0) // İlk elemandaysanız
                        {
                            trackingPositions.RemoveAt(currentTrackingPosition);
                            // İlk elemanı silince indexi güncellememize gerek yok, çünkü yeni "0" geçerli olur.
                            if (debugging)
                                debug.PrintChat("The point from the " + currentTrackingPosition + "th index has been deleted. \n" +
                                                "Current index is: " + currentTrackingPosition);
                        }
                        else // Arada bir elemandaysanız
                        {
                            trackingPositions.RemoveAt(currentTrackingPosition);
                            // Şu anki indexi aynı bırakıyoruz çünkü listeden bir eleman eksilince
                            // sonraki eleman aynı indexi alıyor.
                            if (debugging)
                                debug.PrintChat("The point from the " + currentTrackingPosition + "th index has been deleted. \n" +
                                                "Current index is: " + currentTrackingPosition);
                        }
                    }
                    else if (trackingPositions.Count == 1) // Listede tek bir eleman varsa
                    {
                        trackingPositions.RemoveAt(0);
                        currentTrackingPosition = -1; // Geçersiz index (hiç eleman yok)
                        state = TGPState.Search;
                        if (debugging)
                            debug.PrintChat("No tracking point has been left in the list. Switching the state to Search Mode");
                    }
                    else // Listede hiç eleman yoksa
                    {
                        if (debugging)
                            debug.PrintChat("The list is already empty. No operation performed.");
                    }
                }

                if (argument == "NextPos")
                {
                    if (state != TGPState.PointTrack)
                    {
                        state = TGPState.PointTrack;
                        if (debugging)
                            debug.PrintChat("State set to Point Track");
                        return;
                    }

                    if (trackingPositions.Count > 1)
                    {
                        if (currentTrackingPosition + 2 > trackingPositions.Count)
                        {
                            currentTrackingPosition = 0;
                        }
                        else
                        {
                            currentTrackingPosition++;
                        }
                        if (debugging)
                            debug.PrintChat("Tracking the " + currentTrackingPosition + "th point.");
                    }
                }

                if (argument == "PrevPos")
                {
                    if (state != TGPState.PointTrack)
                    {
                        state = TGPState.PointTrack;
                        if (debugging)
                            debug.PrintChat("State set to Point Track");
                        return;
                    }

                    if (trackingPositions.Count > 1)
                    {
                        if (currentTrackingPosition == 0)
                        {
                            currentTrackingPosition = trackingPositions.Count - 1;
                        }
                        else
                        {
                            currentTrackingPosition--;
                        }
                        if (debugging)
                            debug.PrintChat("Tracking the " + currentTrackingPosition + "th point.");
                    }
                }

                if (argument == "Control")
                {
                    control = !control;
                    if (debugging)
                    {
                        if (control) debug.PrintChat("Control has been enabled.");

                        else debug.PrintChat("Control has been disabled.");
                    }
                }

                return;
            }


            switch (state)
            {
                case TGPState.Test:

                    break;
                case TGPState.Manual:
                    if (manualControlVector == Vector3D.Zero) manualControlVector = stabilizerRotor.WorldMatrix.Up.Normalized();

                    if (control)
                    {

                        //Override Gyros and Deatcivate FCS (one time)
                        if (!gyroOverriding)
                        {
                            gyroOverriding = true;
                            foreach (var item in Gyros)
                            {
                                item.GyroOverride = true;
                                item.GyroPower = 1;
                            }
                        }

                        //No gravity
                        if (shipControllers[0].GetNaturalGravity().Length() < 0.001)
                        {
                            BasicControl();
                        }
                        //Gravity
                        else
                        {
                            AimWithPID(AdvencedControl());
                        }
                    }
                    else
                    {
                        azimuthRotor.TargetVelocityRPM = 0;
                        elevationHinge.TargetVelocityRPM = 0;

                        //stop overriding gyros and enable FCS (one time)
                        if (gyroOverriding)
                        {
                            gyroOverriding = false;
                            foreach (var item in Gyros)
                            {
                                item.GyroOverride = false;
                                item.GyroPower = 1f;
                            }
                        }
                    }

                    break;
                case TGPState.Search:
                    //SearchMode is not available in space.
                    if (shipControllers[0].GetNaturalGravity().Length() < 0.001)
                    {
                        state = TGPState.Manual;
                        return;
                    }

                    bool input = false;
                    if (control)
                    {
                        //Override Gyros and Deatcivate FCS (one time)
                        if (!gyroOverriding)
                        {
                            gyroOverriding = true;
                            foreach (var item in Gyros)
                            {
                                item.GyroOverride = true;
                                item.GyroPower = 1;
                            }
                        }

                        if (GetInput() != Vector2.Zero) input = true;
                    }
                    else
                    {
                        //stop overriding gyros and enable FCS (one time)
                        if (gyroOverriding)
                        {
                            gyroOverriding = false;
                            foreach (var item in Gyros)
                            {
                                item.GyroOverride = false;
                                item.GyroPower = 1;
                            }
                        }

                        input = false;
                    }
                    //There's no input
                    if (!input)
                    {
                        
                        if (tempSearchPoint == Vector3D.Zero)
                        {
                            podCamera.EnableRaycast = true;
                            MyDetectedEntityInfo hitInfo = podCamera.Raycast(5000);
                            
                            if (hitInfo.IsEmpty())
                            {
                                state = TGPState.Manual;
                                if (debugging)
                                    debug.PrintChat("NO TEMPORARY SEARCH POINT HAS BEEN FOUND! State has been set to Manual");
                                manualControlVector = stabilizerRotor.WorldMatrix.Up.Normalized();
                                return;
                            }
                            tempSearchPoint = (Vector3D)hitInfo.HitPosition;
                        }
                        if (manualControlVector != Vector3D.Zero)
                        {
                            manualControlVector = Vector3D.Zero;
                        }
                        AimWithPID(tempSearchPoint);
                        if (debugging)
                            debug.DrawSphere(new BoundingSphereD(tempSearchPoint,0.1f), Color.Red);

                    }
                    //There's an input
                    if (input)
                    {

                        if (tempSearchPoint != Vector3D.Zero)
                        {
                            tempSearchPoint = Vector3D.Zero;
                        }

                        AimWithPID(AdvencedControl());
                    }


                    break;
                case TGPState.PointTrack:
                    //PointTrack is not available in space.
                    if (shipControllers[0].GetNaturalGravity().Length() < 0.001)
                    {
                        state = TGPState.Manual;
                        return;
                    }

                    AimWithPID(trackingPositions[currentTrackingPosition]);
                    control = false;
                    gyroOverriding = false;
                    foreach (var item in Gyros)
                    {
                        item.GyroOverride = false;
                        item.GyroPower = 1;
                    }
                    if (debugging)
                        debug.DrawSphere(new BoundingSphereD(trackingPositions[currentTrackingPosition], 0.1f), Color.Red);
                    break;
            }
        }

        /// <summary>
        /// Create an instance of this and hold its reference.
        /// </summary>
        public class DebugAPI
        {
            public readonly bool ModDetected;

            /// <summary>
            /// Changing this will affect OnTop draw for all future draws that don't have it specified.
            /// </summary>
            public bool DefaultOnTop;

            /// <summary>
            /// Recommended to be used at start of Main(), unless you wish to draw things persistently and remove them manually.
            /// <para>Removes everything except AdjustNumber and chat messages.</para>
            /// </summary>
            public void RemoveDraw() => _removeDraw?.Invoke(_pb);
            Action<IMyProgrammableBlock> _removeDraw;

            /// <summary>
            /// Removes everything that was added by this API (except chat messages), including DeclareAdjustNumber()!
            /// <para>For calling in Main() you should use <see cref="RemoveDraw"/> instead.</para>
            /// </summary>
            public void RemoveAll() => _removeAll?.Invoke(_pb);
            Action<IMyProgrammableBlock> _removeAll;

            /// <summary>
            /// You can store the integer returned by other methods then remove it with this when you wish.
            /// <para>Or you can not use this at all and call <see cref="RemoveDraw"/> on every Main() so that your drawn things live a single PB run.</para>
            /// </summary>
            public void Remove(int id) => _remove?.Invoke(_pb, id);
            Action<IMyProgrammableBlock, int> _remove;

            public int DrawPoint(Vector3D origin, Color color, float radius = 0.2f, float seconds = DefaultSeconds, bool? onTop = null) => _point?.Invoke(_pb, origin, color, radius, seconds, onTop ?? DefaultOnTop) ?? -1;
            Func<IMyProgrammableBlock, Vector3D, Color, float, float, bool, int> _point;

            public int DrawLine(Vector3D start, Vector3D end, Color color, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _line?.Invoke(_pb, start, end, color, thickness, seconds, onTop ?? DefaultOnTop) ?? -1;
            Func<IMyProgrammableBlock, Vector3D, Vector3D, Color, float, float, bool, int> _line;

            public int DrawAABB(BoundingBoxD bb, Color color, Style style = Style.Wireframe, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _aabb?.Invoke(_pb, bb, color, (int)style, thickness, seconds, onTop ?? DefaultOnTop) ?? -1;
            Func<IMyProgrammableBlock, BoundingBoxD, Color, int, float, float, bool, int> _aabb;

            public int DrawOBB(MyOrientedBoundingBoxD obb, Color color, Style style = Style.Wireframe, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _obb?.Invoke(_pb, obb, color, (int)style, thickness, seconds, onTop ?? DefaultOnTop) ?? -1;
            Func<IMyProgrammableBlock, MyOrientedBoundingBoxD, Color, int, float, float, bool, int> _obb;

            public int DrawSphere(BoundingSphereD sphere, Color color, Style style = Style.Wireframe, float thickness = DefaultThickness, int lineEveryDegrees = 15, float seconds = DefaultSeconds, bool? onTop = null) => _sphere?.Invoke(_pb, sphere, color, (int)style, thickness, lineEveryDegrees, seconds, onTop ?? DefaultOnTop) ?? -1;
            Func<IMyProgrammableBlock, BoundingSphereD, Color, int, float, int, float, bool, int> _sphere;

            public int DrawMatrix(MatrixD matrix, float length = 1f, float thickness = DefaultThickness, float seconds = DefaultSeconds, bool? onTop = null) => _matrix?.Invoke(_pb, matrix, length, thickness, seconds, onTop ?? DefaultOnTop) ?? -1;
            Func<IMyProgrammableBlock, MatrixD, float, float, float, bool, int> _matrix;

            /// <summary>
            /// Adds a HUD marker for a world position.
            /// <para>White is used if <paramref name="color"/> is null.</para>
            /// </summary>
            public int DrawGPS(string name, Vector3D origin, Color? color = null, float seconds = DefaultSeconds) => _gps?.Invoke(_pb, name, origin, color, seconds) ?? -1;
            Func<IMyProgrammableBlock, string, Vector3D, Color?, float, int> _gps;

            /// <summary>
            /// Adds a notification center on screen. Do not give 0 or lower <paramref name="seconds"/>.
            /// </summary>
            public int PrintHUD(string message, Font font = Font.Debug, float seconds = 2) => _printHUD?.Invoke(_pb, message, font.ToString(), seconds) ?? -1;
            Func<IMyProgrammableBlock, string, string, float, int> _printHUD;

            /// <summary>
            /// Shows a message in chat as if sent by the PB (or whoever you want the sender to be)
            /// <para>If <paramref name="sender"/> is null, the PB's CustomName is used.</para>
            /// <para>The <paramref name="font"/> affects the fontface and color of the entire message, while <paramref name="senderColor"/> only affects the sender name's color.</para>
            /// </summary>
            public void PrintChat(string message, string sender = null, Color? senderColor = null, Font font = Font.Debug) => _chat?.Invoke(_pb, message, sender, senderColor, font.ToString());
            Action<IMyProgrammableBlock, string, string, Color?, string> _chat;

            /// <summary>
            /// Used for realtime adjustments, allows you to hold the specified key/button with mouse scroll in order to adjust the <paramref name="initial"/> number by <paramref name="step"/> amount.
            /// <para>Add this once at start then store the returned id, then use that id with <see cref="GetAdjustNumber(int)"/>.</para>
            /// </summary>
            public void DeclareAdjustNumber(out int id, double initial, double step = 0.05, Input modifier = Input.Control, string label = null) => id = _adjustNumber?.Invoke(_pb, initial, step, modifier.ToString(), label) ?? -1;
            Func<IMyProgrammableBlock, double, double, string, string, int> _adjustNumber;

            /// <summary>
            /// See description for: <see cref="DeclareAdjustNumber(double, double, Input, string)"/>.
            /// <para>The <paramref name="noModDefault"/> is returned when the mod is not present.</para>
            /// </summary>
            public double GetAdjustNumber(int id, double noModDefault = 1) => _getAdjustNumber?.Invoke(_pb, id) ?? noModDefault;
            Func<IMyProgrammableBlock, int, double> _getAdjustNumber;

            /// <summary>
            /// Gets simulation tick since this session started. Returns -1 if mod is not present.
            /// </summary>
            public int GetTick() => _tick?.Invoke() ?? -1;
            Func<int> _tick;

            /// <summary>
            /// Gets time from Stopwatch which is accurate to nanoseconds, can be used to measure code execution time.
            /// Returns TimeSpan.Zero if mod is not present.
            /// </summary>
            public TimeSpan GetTimestamp() => _timestamp?.Invoke() ?? TimeSpan.Zero;
            Func<TimeSpan> _timestamp;

            /// <summary>
            /// Use with a using() statement to measure a chunk of code and get the time difference in a callback.
            /// <code>
            /// using(Debug.Measure((t) => Echo($"diff={t}")))
            /// {
            ///    // code to measure
            /// }
            /// </code>
            /// This simply calls <see cref="GetTimestamp"/> before and after the inside code.
            /// </summary>
            public MeasureToken Measure(Action<TimeSpan> call) => new MeasureToken(this, call);

            /// <summary>
            /// <see cref="Measure(Action{TimeSpan})"/>
            /// </summary>
            public MeasureToken Measure(string prefix) => new MeasureToken(this, (t) => PrintHUD($"{prefix} {t.TotalMilliseconds} ms"));

            public struct MeasureToken : IDisposable
            {
                DebugAPI API;
                TimeSpan Start;
                Action<TimeSpan> Callback;

                public MeasureToken(DebugAPI api, Action<TimeSpan> call)
                {
                    API = api;
                    Callback = call;
                    Start = API.GetTimestamp();
                }

                public void Dispose()
                {
                    Callback?.Invoke(API.GetTimestamp() - Start);
                }
            }

            public enum Style { Solid, Wireframe, SolidAndWireframe }
            public enum Input { MouseLeftButton, MouseRightButton, MouseMiddleButton, MouseExtraButton1, MouseExtraButton2, LeftShift, RightShift, LeftControl, RightControl, LeftAlt, RightAlt, Tab, Shift, Control, Alt, Space, PageUp, PageDown, End, Home, Insert, Delete, Left, Up, Right, Down, D0, D1, D2, D3, D4, D5, D6, D7, D8, D9, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, NumPad0, NumPad1, NumPad2, NumPad3, NumPad4, NumPad5, NumPad6, NumPad7, NumPad8, NumPad9, Multiply, Add, Separator, Subtract, Decimal, Divide, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12 }
            public enum Font { Debug, White, Red, Green, Blue, DarkBlue }

            const float DefaultThickness = 0.02f;
            const float DefaultSeconds = -1;

            IMyProgrammableBlock _pb;

            /// <summary>
            /// NOTE: if mod is not present then methods will simply not do anything, therefore you can leave the methods in your released code.
            /// </summary>
            /// <param name="program">pass `this`.</param>
            /// <param name="drawOnTopDefault">set the default for onTop on all objects that have such an option.</param>
            public DebugAPI(MyGridProgram program, bool drawOnTopDefault = false)
            {
                if (program == null) throw new Exception("Pass `this` into the API, not null.");

                DefaultOnTop = drawOnTopDefault;
                _pb = program.Me;

                var methods = _pb.GetProperty("DebugAPI")?.As<IReadOnlyDictionary<string, Delegate>>()?.GetValue(_pb);
                if (methods != null)
                {
                    Assign(out _removeAll, methods["RemoveAll"]);
                    Assign(out _removeDraw, methods["RemoveDraw"]);
                    Assign(out _remove, methods["Remove"]);
                    Assign(out _point, methods["Point"]);
                    Assign(out _line, methods["Line"]);
                    Assign(out _aabb, methods["AABB"]);
                    Assign(out _obb, methods["OBB"]);
                    Assign(out _sphere, methods["Sphere"]);
                    Assign(out _matrix, methods["Matrix"]);
                    Assign(out _gps, methods["GPS"]);
                    Assign(out _printHUD, methods["HUDNotification"]);
                    Assign(out _chat, methods["Chat"]);
                    Assign(out _adjustNumber, methods["DeclareAdjustNumber"]);
                    Assign(out _getAdjustNumber, methods["GetAdjustNumber"]);
                    Assign(out _tick, methods["Tick"]);
                    Assign(out _timestamp, methods["Timestamp"]);

                    RemoveAll(); // cleanup from past compilations on this same PB

                    ModDetected = true;
                }
            }

            void Assign<T>(out T field, object method) => field = (T)method;
        }


        /*
         * WcPbAPI class. Reference: https://steamcommunity.com/sharedfiles/filedetails/?id=2178802013
         * It is highly recommended to delete unneeded api methods
         * Non-API functions:
         *  Activate(pbBlock)
         *  ApiAssign(delegates)
         *  AssignMethod(delegates,name,field)
         */
        public class WcPbApi
        {
            private Action<ICollection<MyDefinitionId>> _getCoreWeapons;
            private Action<ICollection<MyDefinitionId>> _getCoreStaticLaunchers;
            private Action<ICollection<MyDefinitionId>> _getCoreTurrets;
            private Func<IMyTerminalBlock, IDictionary<string, int>, bool> _getBlockWeaponMap;
            private Func<long, MyTuple<bool, int, int>> _getProjectilesLockedOn;
            private Action<IMyTerminalBlock, IDictionary<MyDetectedEntityInfo, float>> _getSortedThreats;
            private Func<long, int, MyDetectedEntityInfo> _getAiFocus;
            private Func<IMyTerminalBlock, long, int, bool> _setAiFocus;
            private Func<IMyTerminalBlock, int, MyDetectedEntityInfo> _getWeaponTarget;
            private Action<IMyTerminalBlock, long, int> _setWeaponTarget;
            private Action<IMyTerminalBlock, bool, int> _fireWeaponOnce;
            private Action<IMyTerminalBlock, bool, bool, int> _toggleWeaponFire;
            private Func<IMyTerminalBlock, int, bool, bool, bool> _isWeaponReadyToFire;
            private Func<IMyTerminalBlock, int, float> _getMaxWeaponRange;
            private Func<IMyTerminalBlock, ICollection<string>, int, bool> _getTurretTargetTypes;
            private Action<IMyTerminalBlock, ICollection<string>, int> _setTurretTargetTypes;
            private Action<IMyTerminalBlock, float> _setBlockTrackingRange;
            private Func<IMyTerminalBlock, long, int, bool> _isTargetAligned;
            private Func<IMyTerminalBlock, long, int, bool> _canShootTarget;
            private Func<IMyTerminalBlock, long, int, Vector3D?> _getPredictedTargetPos;
            private Func<IMyTerminalBlock, float> _getHeatLevel;
            private Func<IMyTerminalBlock, float> _currentPowerConsumption;
            private Func<MyDefinitionId, float> _getMaxPower;
            private Func<long, bool> _hasGridAi;
            private Func<IMyTerminalBlock, bool> _hasCoreWeapon;
            private Func<long, float> _getOptimalDps;
            private Func<IMyTerminalBlock, int, string> _getActiveAmmo;
            private Action<IMyTerminalBlock, int, string> _setActiveAmmo;
            private Action<Action<Vector3, float>> _registerProjectileAdded;
            private Action<Action<Vector3, float>> _unRegisterProjectileAdded;
            private Func<long, float> _getConstructEffectiveDps;
            private Func<IMyTerminalBlock, long> _getPlayerController;
            private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Matrix> _getWeaponAzimuthMatrix;
            private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, Matrix> _getWeaponElevationMatrix;
            private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, long, bool, bool, bool> _isTargetValid;
            private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, int, MyTuple<Vector3D, Vector3D>> _getWeaponScope;
            private Func<Sandbox.ModAPI.Ingame.IMyTerminalBlock, MyTuple<bool, bool>> _isInRange;

            /*
             *  Tries to setup the Api if WC is loaded
             *  @param pbBlock: the block executing this script (WcPbAPI properties are stored in IMyProgrammableBlock)
             *  @throws Exception: thrown if WC is NOT loaded on function call. This should only be the case if WC broke
             *      or wasn't included in the world
             *  @return: ApiAssign()
             */
            public bool Activate(IMyTerminalBlock pbBlock)
            {
                var dict = pbBlock.GetProperty("WcPbAPI")?.As<IReadOnlyDictionary<string, Delegate>>().GetValue(pbBlock);
                if (dict == null) throw new Exception($"WcPbAPI failed to activate");
                return ApiAssign(dict);
            }

            /*
             *  Tries to assign the Api delegates to fields of this class
             *  @param delegates: read-only dictionary of delegates with string keys
             *  @return: true unless delegates is null
             */
            public bool ApiAssign(IReadOnlyDictionary<string, Delegate> delegates)
            {
                if (delegates == null)
                    return false;
                AssignMethod(delegates, "GetCoreWeapons", ref _getCoreWeapons);
                AssignMethod(delegates, "GetCoreStaticLaunchers", ref _getCoreStaticLaunchers);
                AssignMethod(delegates, "GetCoreTurrets", ref _getCoreTurrets);
                AssignMethod(delegates, "GetBlockWeaponMap", ref _getBlockWeaponMap);
                AssignMethod(delegates, "GetProjectilesLockedOn", ref _getProjectilesLockedOn);
                AssignMethod(delegates, "GetSortedThreats", ref _getSortedThreats);
                AssignMethod(delegates, "GetAiFocus", ref _getAiFocus);
                AssignMethod(delegates, "SetAiFocus", ref _setAiFocus);
                AssignMethod(delegates, "GetWeaponTarget", ref _getWeaponTarget);
                AssignMethod(delegates, "SetWeaponTarget", ref _setWeaponTarget);
                AssignMethod(delegates, "FireWeaponOnce", ref _fireWeaponOnce);
                AssignMethod(delegates, "ToggleWeaponFire", ref _toggleWeaponFire);
                AssignMethod(delegates, "IsWeaponReadyToFire", ref _isWeaponReadyToFire);
                AssignMethod(delegates, "GetMaxWeaponRange", ref _getMaxWeaponRange);
                AssignMethod(delegates, "GetTurretTargetTypes", ref _getTurretTargetTypes);
                AssignMethod(delegates, "SetTurretTargetTypes", ref _setTurretTargetTypes);
                AssignMethod(delegates, "SetBlockTrackingRange", ref _setBlockTrackingRange);
                AssignMethod(delegates, "IsTargetAligned", ref _isTargetAligned);
                AssignMethod(delegates, "CanShootTarget", ref _canShootTarget);
                AssignMethod(delegates, "GetPredictedTargetPosition", ref _getPredictedTargetPos);
                AssignMethod(delegates, "GetHeatLevel", ref _getHeatLevel);
                AssignMethod(delegates, "GetCurrentPower", ref _currentPowerConsumption);
                AssignMethod(delegates, "GetMaxPower", ref _getMaxPower);
                AssignMethod(delegates, "HasGridAi", ref _hasGridAi);
                AssignMethod(delegates, "HasCoreWeapon", ref _hasCoreWeapon);
                AssignMethod(delegates, "GetOptimalDps", ref _getOptimalDps);
                AssignMethod(delegates, "GetActiveAmmo", ref _getActiveAmmo);
                AssignMethod(delegates, "SetActiveAmmo", ref _setActiveAmmo);
                AssignMethod(delegates, "RegisterProjectileAdded", ref _registerProjectileAdded);
                AssignMethod(delegates, "UnRegisterProjectileAdded", ref _unRegisterProjectileAdded);
                AssignMethod(delegates, "GetConstructEffectiveDps", ref _getConstructEffectiveDps);
                AssignMethod(delegates, "GetPlayerController", ref _getPlayerController);
                AssignMethod(delegates, "GetWeaponAzimuthMatrix", ref _getWeaponAzimuthMatrix);
                AssignMethod(delegates, "GetWeaponElevationMatrix", ref _getWeaponElevationMatrix);
                AssignMethod(delegates, "IsTargetValid", ref _isTargetValid);
                AssignMethod(delegates, "GetWeaponScope", ref _getWeaponScope);
                AssignMethod(delegates, "IsInRange", ref _isInRange);
                return true;
            }

            /*
             *  Tries to assign delegate methods to fields while checking for identical types.
             *  @param delegates: read-only dictionary of delegates with string keys. If this is null field will be set to null
             *  @param name: name of the delegate to assign
             *  @param field: referenceto a field in this class, to assign the delegate to.
             *  @throws Exception: thrown if either name isn't pointing to a delegate in delegates or field and the delegate aren't of the same type
             */
            private void AssignMethod<T>(IReadOnlyDictionary<string, Delegate> delegates, string name, ref T field) where T : class
            {
                if (delegates == null)
                {
                    field = null;
                    return;
                }
                Delegate del;
                if (!delegates.TryGetValue(name, out del))
                    throw new Exception($"{GetType().Name} :: Couldn't find {name} delegate of type {typeof(T)}");
                field = del as T;
                if (field == null)
                    throw new Exception(
                        $"{GetType().Name} :: Delegate {name} is not type {typeof(T)}, instead it's: {del.GetType()}");
            }
            public void GetAllCoreWeapons(ICollection<MyDefinitionId> collection) => _getCoreWeapons?.Invoke(collection);
            public void GetAllCoreStaticLaunchers(ICollection<MyDefinitionId> collection) =>
                _getCoreStaticLaunchers?.Invoke(collection);
            public void GetAllCoreTurrets(ICollection<MyDefinitionId> collection) => _getCoreTurrets?.Invoke(collection);
            public bool GetBlockWeaponMap(IMyTerminalBlock weaponBlock, IDictionary<string, int> collection) =>
                _getBlockWeaponMap?.Invoke(weaponBlock, collection) ?? false;
            public MyTuple<bool, int, int> GetProjectilesLockedOn(long victim) =>
                _getProjectilesLockedOn?.Invoke(victim) ?? new MyTuple<bool, int, int>();
            public void GetSortedThreats(IMyTerminalBlock pbBlock, IDictionary<MyDetectedEntityInfo, float> collection) =>
                _getSortedThreats?.Invoke(pbBlock, collection);
            public MyDetectedEntityInfo? GetAiFocus(long shooter, int priority = 0) => _getAiFocus?.Invoke(shooter, priority);
            public bool SetAiFocus(IMyTerminalBlock pbBlock, long target, int priority = 0) =>
                _setAiFocus?.Invoke(pbBlock, target, priority) ?? false;
            public MyDetectedEntityInfo? GetWeaponTarget(IMyTerminalBlock weapon, int weaponId = 0) =>
                _getWeaponTarget?.Invoke(weapon, weaponId) ?? null;
            public void SetWeaponTarget(IMyTerminalBlock weapon, long target, int weaponId = 0) =>
                _setWeaponTarget?.Invoke(weapon, target, weaponId);
            public void FireWeaponOnce(IMyTerminalBlock weapon, bool allWeapons = true, int weaponId = 0) =>
                _fireWeaponOnce?.Invoke(weapon, allWeapons, weaponId);
            public void ToggleWeaponFire(IMyTerminalBlock weapon, bool on, bool allWeapons, int weaponId = 0) =>
                _toggleWeaponFire?.Invoke(weapon, on, allWeapons, weaponId);
            public bool IsWeaponReadyToFire(IMyTerminalBlock weapon, int weaponId = 0, bool anyWeaponReady = true,
                bool shootReady = false) =>
                _isWeaponReadyToFire?.Invoke(weapon, weaponId, anyWeaponReady, shootReady) ?? false;
            public float GetMaxWeaponRange(IMyTerminalBlock weapon, int weaponId) =>
                _getMaxWeaponRange?.Invoke(weapon, weaponId) ?? 0f;
            public bool GetTurretTargetTypes(IMyTerminalBlock weapon, IList<string> collection, int weaponId = 0) =>
                _getTurretTargetTypes?.Invoke(weapon, collection, weaponId) ?? false;
            public void SetTurretTargetTypes(IMyTerminalBlock weapon, IList<string> collection, int weaponId = 0) =>
                _setTurretTargetTypes?.Invoke(weapon, collection, weaponId);
            public void SetBlockTrackingRange(IMyTerminalBlock weapon, float range) =>
                _setBlockTrackingRange?.Invoke(weapon, range);
            public bool IsTargetAligned(IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
                _isTargetAligned?.Invoke(weapon, targetEnt, weaponId) ?? false;
            public bool CanShootTarget(IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
                _canShootTarget?.Invoke(weapon, targetEnt, weaponId) ?? false;
            public Vector3D? GetPredictedTargetPosition(IMyTerminalBlock weapon, long targetEnt, int weaponId) =>
                _getPredictedTargetPos?.Invoke(weapon, targetEnt, weaponId) ?? null;
            public float GetHeatLevel(IMyTerminalBlock weapon) => _getHeatLevel?.Invoke(weapon) ?? 0f;
            public float GetCurrentPower(IMyTerminalBlock weapon) => _currentPowerConsumption?.Invoke(weapon) ?? 0f;
            public float GetMaxPower(MyDefinitionId weaponDef) => _getMaxPower?.Invoke(weaponDef) ?? 0f;
            public bool HasGridAi(long entity) => _hasGridAi?.Invoke(entity) ?? false;
            public bool HasCoreWeapon(IMyTerminalBlock weapon) => _hasCoreWeapon?.Invoke(weapon) ?? false;
            public float GetOptimalDps(long entity) => _getOptimalDps?.Invoke(entity) ?? 0f;
            public string GetActiveAmmo(IMyTerminalBlock weapon, int weaponId) =>
                _getActiveAmmo?.Invoke(weapon, weaponId) ?? null;
            public void SetActiveAmmo(IMyTerminalBlock weapon, int weaponId, string ammoType) =>
                _setActiveAmmo?.Invoke(weapon, weaponId, ammoType);
            public void RegisterProjectileAddedCallback(Action<Vector3, float> action) =>
                _registerProjectileAdded?.Invoke(action);
            public void UnRegisterProjectileAddedCallback(Action<Vector3, float> action) =>
                _unRegisterProjectileAdded?.Invoke(action);
            public float GetConstructEffectiveDps(long entity) => _getConstructEffectiveDps?.Invoke(entity) ?? 0f;
            public long GetPlayerController(IMyTerminalBlock weapon) => _getPlayerController?.Invoke(weapon) ?? -1;
            public Matrix GetWeaponAzimuthMatrix(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
                _getWeaponAzimuthMatrix?.Invoke(weapon, weaponId) ?? Matrix.Zero;
            public Matrix GetWeaponElevationMatrix(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
                _getWeaponElevationMatrix?.Invoke(weapon, weaponId) ?? Matrix.Zero;
            public bool IsTargetValid(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, long targetId, bool onlyThreats, bool checkRelations) =>
                _isTargetValid?.Invoke(weapon, targetId, onlyThreats, checkRelations) ?? false;
            public MyTuple<Vector3D, Vector3D> GetWeaponScope(Sandbox.ModAPI.Ingame.IMyTerminalBlock weapon, int weaponId) =>
                _getWeaponScope?.Invoke(weapon, weaponId) ?? new MyTuple<Vector3D, Vector3D>();
            public MyTuple<bool, bool> IsInRange(Sandbox.ModAPI.Ingame.IMyTerminalBlock block) =>
                _isInRange?.Invoke(block) ?? new MyTuple<bool, bool>();
        }


        #endregion