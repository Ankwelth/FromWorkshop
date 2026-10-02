/*
 * R e a d m e
 * -----------
 * 
 * This script is intended to manage simple bomb sights.The bomb sight
 * should consist of a camera attached to a hinge attached to either the
 * bottom or the front of the grid, provided that the hinge rotates
 * "up and down" or "forward and backward" rather than "side to side"
 *  Make sure to specify the hinge orientation in the "USER SETTINGS" section.
 * 
 * Note: This should work with multiple hinge-camera assemblies,
 * but the assemblies must all be either horizontal or vertical. The
 * script uses the current, above-ground-level altitude and gravity to calculate
 * bomb fall distance, so it doesn't account for changes in terrain or
 * gravitational strength. Other inaccuracies may arise from crazy flying, but
 * for general bombing runs should give you a ball park for bomb placement.
 */


////////////////////////// TO SET UP AND USE ////////////////////////////
// Add the keyword "BHinge" to the block name of bombsight hinges
// If the hinge connects to the bottom of the craft, make the camera point
// forward before running the script; if your hinge connects to the front,
// make the camera point down.

// If you have multiple cockpits, and want one or more of them to display
// the distance to target, add the keyword "Target" to the name of the
// cockpits.

// This script basically just points the camera to target the place on
// the ground where the bombs would land if released at the current moment.
// Hence, when the target lines up with the camera's crosshairs, that's
// when you ought to drop the bombs. It also outputs the distance from ship
// to target at which to drop the bombs to the cockpit's LCD, so if you happen
// to have a HUD marker for the target (GPS, beacon, or antenna) you can drop
// the bombs accurately that way.
/////////////////////////////////////////////////////////////////////////

public Program()
{

    /////////////////////////// USER SETTINGS /////////////////////////////

    // set to true if the hinge connects to the front of your ship

    boresightForward = false;

    //set this to true if you have multiple cockpits

    multipleCockpits = false;

    //change this to false if you don't want the distance to target at which
    //to drop bombs printed on screen

    printToScreen = true;

    //Default true. Set to false if the game is modded and you do not know the modded speed limit
    speedLimitKnown = true;
    //Maximum (freefall) speed in m/s. Change this to agree with the in game max freefall speed. Default vanilla value is 104.38.
    maxSpeed = 104.38;

    ////////////// DO NOT CHANGE ANYTHING BEYOND THIS POINT ///////////////


    //Gets the cockpit of the grid
    List<IMyCockpit> cockpitList = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType(cockpitList, cockpit => cockpit.CanControlShip == true);
    myCockpit = cockpitList[0];

    //Gets auxilary seats
    bombardierList = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType(bombardierList, seat => seat.CustomName.Contains("Target"));

    //Get the intended rotors and hinges
    hinges = new List<IMyMotorAdvancedStator>();
    GridTerminalSystem.GetBlocksOfType(hinges, hinge => hinge.CustomName.Contains("BHinge"));

    //Get the initial angles (this block of code allows engineers
    //to build hinges and rotors in almost any orientation)
    theta90 = new double[hinges.Count];
    string[] storedData = Storage.Split(';');
    int[] thetaArray = new int[theta90.Length];
    //This recalling save states may be jenky.
    for (int i = 0; i < hinges.Count; i++)
    {

        if (Storage == "")
        {
            theta90[i] = hinges[i].Angle;
        } else
        {
            int.TryParse(storedData[i], out thetaArray[i]);
            theta90[i] = thetaArray[i] * Math.PI / 180.0;
        }

        theta90[i] = hinges[i].Angle;

    }

    //Run quickly, but without sacrificing performance.
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public void Save()
{
    for(int i = 0; i<theta90.Length; i++)
    {
        Storage = Storage + ((theta90[i] * (180 / Math.PI))).ToString() + ";";
    }
}
//allocate memory for the variables
public List<IMyMotorAdvancedStator> hinges;
public List<IMyCockpit> bombardierList;
public IMyTextSurface lcd;
public bool boresightForward;
public bool printToScreen;
public bool multipleCockpits;
public bool speedLimitKnown;
public double maxSpeed;
public IMyCockpit myCockpit;
public double[] theta90;

public void Main()
{
    MatrixD gridOrientation = myCockpit.WorldMatrix;
    Vector3D prograde = myCockpit.GetShipVelocities().LinearVelocity;

    Vector3D gravity = myCockpit.GetNaturalGravity();
    Vector3D up = Vector3D.Negate(gravity);
    Vector3D localDown = new Vector3D(gridOrientation.GetDirectionVector(Base6Directions.Direction.Down));
    Vector3D localForward = new Vector3D(gridOrientation.GetDirectionVector(Base6Directions.Direction.Forward));
    Vector3D left = new Vector3D(gridOrientation.GetDirectionVector(Base6Directions.Direction.Left));

    //Define GNF coordinates
    Vector3D forward;
    if (prograde.Length() != 0)
    {
        forward = Vector3D.ProjectOnPlane(ref prograde, ref gravity);
    } else if (Math.Abs(Vector3D.Dot(Vector3D.Normalize(localForward), Vector3D.Normalize(gravity))) != 1){
        forward = localForward;
    } else {
        forward = Vector3D.Negate(localDown);
    }
    Vector3D normal = Vector3D.Cross(forward, gravity);
    //GNF transformation matrix
    MatrixD GNF = new MatrixD();
    GNF.Down = Vector3D.Normalize(gravity);
    GNF.Left = Vector3D.Normalize(normal);
    GNF.Forward = Vector3D.Normalize(forward);

    double altitude; //this is instantiated later on


    //this returns a boolean, but instantiates altitude with the current altitude AGL
    if(!myCockpit.TryGetPlanetElevation(MyPlanetElevation.Surface, out altitude))
    {
        altitude = -1;
    }

    //Calculate bomb travel distance
    double vForward = forward.Length();
    double horizontalDistanceTraveled;
    if (altitude >= 0 && prograde.Length()!=0)
    {
        double vUp;
        if (prograde.Length() != 0)
        {
            vUp = Vector3D.ProjectOnVector(ref prograde, ref up).Length() * Vector3D.Dot(Vector3D.Normalize(VRageMath.Vector3D.ProjectOnVector(ref prograde, ref up)), Vector3D.Normalize(up));
        } else {
            vUp = 0;
        }
        double time = (-vUp + Math.Sqrt(Math.Pow(vUp, 2) + 2 * gravity.Length() * altitude))/gravity.Length();
        if (!speedLimitKnown)// use newtonian ballistic trajectory. If accessing the modded speed limit were possible,
                             // then the calculations for the vanilla speed limit (in the following "else" statement)
                             // could work with maxSpeed replaced with the speed limit.
        {
            horizontalDistanceTraveled = time * vForward;
        }
        else
        {
            //time when reaching speed limit
            double criticalTime = (vUp + Math.Sqrt(Math.Pow(maxSpeed, 2) - Math.Pow(vForward, 2))) / gravity.Length();//time when bomb reaches terminal velocity
            //altitude at critical time
            double criticalAltitude = altitude + vUp * criticalTime - (gravity.Length() * Math.Pow(criticalTime,2))/2;
            if (time  - criticalTime >= 0)
            {
                //time of impact
                double timeZero = (vUp + Math.Sqrt(Math.Pow((gravity.Length()*criticalAltitude)/maxSpeed +Math.Sqrt(Math.Pow(vUp-gravity.Length()*criticalTime,2)+Math.Pow(vForward,2)),2)-Math.Pow(vForward,2)))/gravity.Length();
                //horizontal distance traveled while at terminal velocity
                double variableDistance = (maxSpeed * vForward * Math.Log(Math.Abs((Math.Sqrt(Math.Pow(vUp - gravity.Length() * timeZero, 2) + Math.Pow(vForward, 2)) + gravity.Length() * timeZero-vUp) / (Math.Sqrt(Math.Pow(vUp - gravity.Length() * criticalTime, 2) + Math.Pow(vForward, 2)) + gravity.Length() * criticalTime-vUp))))/gravity.Length();
                //total distance traveled
                horizontalDistanceTraveled = vForward * criticalTime + variableDistance;
            } else
            {
                horizontalDistanceTraveled = vForward * time;
            }
        }
    } else
    {
        horizontalDistanceTraveled = 0;
    }
    Vector3D towardTargetGNF = new Vector3D(0, -altitude, -horizontalDistanceTraveled);
    Vector3D towardTarget = Vector3D.TransformNormal(Vector3D.Normalize(towardTargetGNF), GNF);


    double theta = Math.PI/2.0;
    if (boresightForward)
    {
        theta = Vector3D.Angle(localForward, towardTarget);
    } else {
        theta = Vector3D.Angle(localDown, towardTarget) * Math.Sign(Vector3D.Dot(towardTarget, localForward));
    }

    for (int i = 0; i < hinges.Count; i++)
    {
        float currentAngleH;
        if (theta90[i] < 0)
        {
            currentAngleH = -hinges[i].Angle;
        } else {
            currentAngleH = hinges[i].Angle;
        }
        if (Math.Abs(theta) > Math.PI/2.0)
        {
            theta = Math.PI/2.0;
        }

        if (Math.Abs((float)theta - currentAngleH) <= 0.05)
        {
            hinges[i].TargetVelocityRad = 0;
        } else if (theta90[i] < 0) {
            hinges[i].TargetVelocityRad = - ((float)theta - currentAngleH);
        } else {
            hinges[i].TargetVelocityRad = ((float)theta - currentAngleH);
        }
    }
    if (printToScreen)
    {
        if (!multipleCockpits)
        {
            lcd = myCockpit.GetSurface(0);
            lcd.ContentType = ContentType.TEXT_AND_IMAGE;
            lcd.Alignment = TextAlignment.CENTER;
            lcd.WriteText("\n\n\nBomb Travel Distance: \n\n" + ((int)(Math.Sqrt(Math.Pow(altitude, 2) + Math.Pow(horizontalDistanceTraveled, 2)))).ToString(), false);
        } else {
            for (int i = 0; i < bombardierList.Count; i++)
            {
                lcd = bombardierList[i].GetSurface(0);
                lcd.ContentType = ContentType.TEXT_AND_IMAGE;
                lcd.Alignment = TextAlignment.CENTER;
                lcd.WriteText("\n\n\nBomb Travel Distance: \n\n" + ((int)(Math.Sqrt(Math.Pow(altitude, 2) + Math.Pow(horizontalDistanceTraveled, 2)))).ToString(), false);
            }
        }
    }
}