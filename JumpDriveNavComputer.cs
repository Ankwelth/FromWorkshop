        int InfoLCD = 0; //Change number to change which display shows info and errors
        int ListLCD = 1; //Change number to change which display shows waypoint list , THESE TWO CANNOT BE SHOW ON THE SAME DISPLAY
        
        //list of blocks needed by the script, change the name in the quote to match your blocks
        string cockpitName = "Cockpit";
        string jumpDriveName = "Jump Drive";
        string remoteName = "Remote Control";
        string gyroName = "Gyroscope";




        //Changing anything below this line will void the non-existing warranty !

        IMyCockpit cockpit;
        IMyJumpDrive Jumper;
        IMyRemoteControl remote;
        List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>();
        IMyGyro gyro;

        Vector3D Target;
        float RadToDeg = (float)(180 / Math.PI);
        float targetDistance = 0, distancePercent, MaxDistance, time;
        int selection = 0, counter = 0, numJumps, pageCount, pageSelect;
        double yaw, pitch;
        string wayList, MaxDist, driveStatus, systemStatus, targetInfo, chargeTime, errors;

        bool Aligning = false;
        bool Aligned = false;
        bool Charging = false;
        bool Online = true;
        bool denied = false;
        bool error = true;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Main(string argument)
        {
            if (error) { InitializationCheck(); }
            else
            {
                remote.GetWaypointInfo(waypoints); //Get the list of waypoints

                switch (argument)//Check for argument and do something
                {
                    case "down":
                        selection++;
                        if (selection > waypoints.Count - 1) { selection = 0; }
                        break;
                    case "up":
                        selection--;
                        if (selection < 0) { selection = waypoints.Count - 1; }//could in theory go to -1 but should be okay
                        break;
                    case "align":
                        Aligning = !Aligning;
                        break;
                    default:
                        Echo("Unrecognized argument");
                        break;
                }

                wayList = "-Destination selection-\n\n";//Get target and build waypoints selection screen
                if (waypoints.Count > 0)
                {
                    if (selection > waypoints.Count - 1)//Make sure selection doesn't go out of bounds
                    {
                        selection = waypoints.Count - 1;
                    }

                    Target = waypoints[selection].Coords;

                    pageCount = (int)Math.Floor((decimal)(waypoints.Count / 6));
                    pageSelect = (int)Math.Floor((decimal)(selection / 6));
                    Echo(pageCount.ToString());
                    if (selection > 5)
                    {
                        wayList += "^ ^ ^ ^ ^\n";
                    }
                    else
                    {
                        wayList += "\n";
                    }

                    for (int i = 0 + 6 * pageSelect; i < 6 + 6 * pageSelect; i++)
                    {
                        if (i < waypoints.Count)
                        {
                            if (i == selection)
                            {
                                wayList = wayList + "-> " + waypoints[i].Name + " <-\n";
                            }
                            else
                            {
                                wayList = wayList + waypoints[i].Name + "\n";
                            }
                        }
                        else
                        {
                            wayList += "\n";
                        }
                    }
                    if (selection < (pageCount * 6))
                    {
                        wayList += "v v v v v";
                    }
                }
                else
                    wayList += "No waypoints available";
                cockpit.GetSurface(ListLCD).ContentType = ContentType.TEXT_AND_IMAGE;
                cockpit.GetSurface(ListLCD).FontSize = 1.3f;
                cockpit.GetSurface(ListLCD).Alignment = TextAlignment.CENTER;
                cockpit.GetSurface(ListLCD).WriteText(wayList);

                //Check if we're trying to align while charging
                if (Aligning && Charging)
                    denied = true;
                if (denied)
                {
                    counter++;
                    if (counter > 60) { denied = false; }
                }
                else
                    counter = 0;

                //Get chargetime left and max jump distance from Jump Drive detailed info
                String info = Jumper.DetailedInfo; //Stuff detailed info on the Jump Drive into a string
                chargeTime = info.Substring(info.LastIndexOf("in:") + 4, 6);
                chargeTime = chargeTime.Replace("\n", "");
                time = int.Parse(info.Substring(info.LastIndexOf("in:") + 4, 2));
                MaxDist = info.Substring(info.LastIndexOf("distance:") + 9, 4); //go to start of "distance:", move 9 characters forward and then take the 4 characters ahead, eg. max jump distance
                if (MaxDist.Contains("km"))//in case jump distance is less than 4 digits, cut out the "km" that will be included in the 4 characters
                    MaxDist = MaxDist.Substring(0, MaxDist.IndexOf('k') - 1);
                MaxDistance = float.Parse(MaxDist);//convert the cut out jump distance, from a string to a float

                if (time > 0) { Charging = true; Aligning = false; } else { Charging = false; }
                if (Jumper.Enabled) Online = true;  else Online = false; 

                //Determind status of drive

                driveStatus = Online ? Charging ? "Recharging - " + chargeTime + " remaining" : driveStatus = "Ready" : "!! Offline !!";

                if (Aligning)
                {
                    if (Aligned)
                    {
                        systemStatus = "Aligned, ready to jump";
                    }
                    else
                    {
                        systemStatus = "Aligning...";
                    }
                }
                else
                {
                    systemStatus = "Waiting...";
                }

                //Do the alignment, set distance on jump drive and build targetinfo text.
                if (Aligning)
                {
                    GetOrientation();
                    gyro.GyroOverride = true;

                    gyro.Pitch = (float)(pitch / 20);//(float)(pitch / 20) > 0.1 || (float)(pitch / 20) < -0.1 ? (float)(pitch / 20) : 0.1f * (float)(pitch / Math.Abs(pitch)); //(float)(Math.Sqrt(pitch) * (pitch / Math.Abs(pitch)));//
                    gyro.Yaw = (float)(yaw / 20);//(float)(yaw / 20) >= 0.1 || (float)(yaw / 20) <= -0.1 ? (float)(yaw / 20) : 0.1f * (float)(yaw / Math.Abs(yaw));//(float)(Math.Sqrt(yaw) * (yaw / Math.Abs(yaw)));//
                    targetDistance = (float)((cockpit.GetPosition() - Target).Length()) / 1000;
                    distancePercent = ((targetDistance - 5) / (MaxDistance - 5)) * 100; //Convert the desired distance to a percentage of the max distance, taking into account that 0% = 5km
                    Jumper.SetValueFloat("JumpDistance", distancePercent);
                    numJumps = (int)Math.Ceiling(targetDistance / MaxDistance);
                    targetInfo = "\n\nDestination: " + waypoints[selection].Name + "\n\nDistance: " + targetDistance + " KM" + "\nNumber of Jumps: " + numJumps + "\n\n-Coordinates-\n" + waypoints[selection].Coords;
                }
                else
                {
                    gyro.GyroOverride = false;
                    targetInfo = "";
                }
                if (pitch < 0.005 && pitch > -0.005 && yaw < 0.005 && yaw > -0.005)
                    Aligned = true;
                else
                    Aligned = false;

                    cockpit.GetSurface(InfoLCD).ContentType = ContentType.TEXT_AND_IMAGE;
                    cockpit.GetSurface(InfoLCD).FontSize = denied ? 2: 1;
                    cockpit.GetSurface(InfoLCD).BackgroundColor = denied ? Color.Red : Color.Teal;
                    cockpit.GetSurface(InfoLCD).FontColor = denied ? Color.Yellow : Color.White;
                    if(!denied)
                    cockpit.GetSurface(InfoLCD).WriteText("Drive Status: " + driveStatus + "\nSystem Status: " + systemStatus + targetInfo);
                    else
                    cockpit.GetSurface(InfoLCD).WriteText("\n\nJump drive charging\ncontrols locked out");
            }

        }

        public void GetOrientation()//Function for getting heading and elevation in relation to target,
        {
            Vector3D DirVect = Vector3D.TransformNormal(Target - cockpit.GetPosition(), MatrixD.Transpose(cockpit.WorldMatrix));
            yaw = Math.Asin(DirVect.X / DirVect.Length()) * RadToDeg;
            pitch = Math.Asin(DirVect.Y / DirVect.Length()) * RadToDeg;
        }
        public void InitializationCheck()//Checks if all blocks exists, assigns them if they do or throws an error message and loops if they do not.
        {
            error = false;
            errors = "ERROR\n";
            if (GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit != null) { cockpit = GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit; } else { Echo("Cockpit missing!"); error = true; }
            if (GridTerminalSystem.GetBlockWithName(jumpDriveName) as IMyJumpDrive != null) { Jumper = GridTerminalSystem.GetBlockWithName(jumpDriveName) as IMyJumpDrive; } else { errors += "\nJump Drive missing!"; error = true; }
            if (GridTerminalSystem.GetBlockWithName(remoteName) as IMyRemoteControl != null) { remote = GridTerminalSystem.GetBlockWithName(remoteName) as IMyRemoteControl; } else { errors += "\n Remote Control missing!"; error = true; }
            if (GridTerminalSystem.GetBlockWithName(gyroName) as IMyGyro != null) { gyro = GridTerminalSystem.GetBlockWithName(gyroName) as IMyGyro; } else { errors += "\nGyroscope is missing!"; error = true; }


            if (error && (GridTerminalSystem.GetBlockWithName(cockpitName) as IMyCockpit != null))
            {
                cockpit.GetSurface(InfoLCD).FontSize = 1;
                cockpit.GetSurface(InfoLCD).BackgroundColor = Color.Blue;
                cockpit.GetSurface(InfoLCD).FontColor = Color.Yellow;
                cockpit.GetSurface(InfoLCD).WriteText(errors);
            }
        }