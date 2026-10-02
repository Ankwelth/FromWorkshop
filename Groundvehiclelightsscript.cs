// VEHICLE LIGHT SCRIPT 
// Author: TheMaegges
// EDIT BLOCK NAMES/TAGS HERE
// ==============================
string mainCockpitName = "Driver Seat";
string mainSpotlightName = "[Front]";
string brakingLightsName = "[Brake]";
string rearLightsName = "[Rear]";
string ambientLightsName = "[Ambient]";
string blinkerLeftName = "[Turn Left]";
string blinkerRightName = "[Turn Right]";
string reversingLightsName = "[Reverse]";
bool linkAmbientLights = true;

// DO NOT EDIT CODE AFTER THIS LINE UNLESS YOU KNOW WHAT YOU ARE DOING
// ===============================
List<IMyTerminalBlock> cockpits = null;
List<IMyTerminalBlock> spotlights = null;
List<IMyTerminalBlock> rearlights = null;
List<IMyTerminalBlock> brakinglights = null;
List<IMyTerminalBlock> ambilights = null;
List<IMyTerminalBlock> leftBlinkers = null;
List<IMyTerminalBlock> rightBlinkers = null;
List<IMyTerminalBlock> reversingLights = null;
bool spotlightEnableLast = false;
double currentSpeed = 0.0;
double lastSpeed = 0.0;
double speedThreshold = 1.5;
IMyShipController mainCockpit = null;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
    InitializeBlocks();
}

public void InitializeBlocks()
{
    cockpits = new List<IMyTerminalBlock>();
    spotlights = new List<IMyTerminalBlock>();
    rearlights = new List<IMyTerminalBlock>();
    brakinglights = new List<IMyTerminalBlock>();
    ambilights = new List<IMyTerminalBlock>();

    leftBlinkers = new List<IMyTerminalBlock>();
    rightBlinkers = new List<IMyTerminalBlock>();
    reversingLights = new List<IMyTerminalBlock>();

    GridTerminalSystem.SearchBlocksOfName( mainCockpitName, cockpits, block => block.IsSameConstructAs(Me));
    if (cockpits != null)
    {
        Echo("Cockpits found: " + cockpits.Count());
        mainCockpit = (IMyShipController) cockpits.FirstOrDefault(pit => (pit is IMyShipController) && (pit as IMyShipController).IsMainCockpit == true);
        Echo("Main Cockpit " + (mainCockpit != null ? "" : "not ") + "found");
    }
    GridTerminalSystem.SearchBlocksOfName( mainSpotlightName, spotlights, block => block.IsSameConstructAs(Me) && block is IMyLightingBlock);
    if (spotlights != null)
        Echo("Spotlights found: " + spotlights.Count());
    GridTerminalSystem.SearchBlocksOfName( brakingLightsName, brakinglights, block => block.IsSameConstructAs(Me) && block is IMyLightingBlock);
    if (brakinglights != null)
        Echo("Braking Lights found: " + brakinglights.Count());
    GridTerminalSystem.SearchBlocksOfName( rearLightsName, rearlights, block => block.IsSameConstructAs(Me) && block is IMyLightingBlock);
    if (rearlights != null)
        Echo("Rear lights found: " + rearlights.Count());
    GridTerminalSystem.SearchBlocksOfName( ambientLightsName, ambilights, block => block.IsSameConstructAs(Me) && block is IMyLightingBlock);
    if (ambilights != null)
        Echo("Ambient lights found: " + ambilights.Count());

    GridTerminalSystem.SearchBlocksOfName( blinkerLeftName, leftBlinkers, block => block.IsSameConstructAs(Me) && block is IMyLightingBlock);
    if (leftBlinkers != null)
        Echo("Left blinkers found: " + leftBlinkers.Count());
    GridTerminalSystem.SearchBlocksOfName( blinkerRightName, rightBlinkers, block => block.IsSameConstructAs(Me) && block is IMyLightingBlock);
    if (rightBlinkers != null)
        Echo("Right blinkers found: " + rightBlinkers.Count());

    GridTerminalSystem.SearchBlocksOfName( reversingLightsName, reversingLights, block => block.IsSameConstructAs(Me));
    if (reversingLights!= null)
        Echo("Reversing lights found: " + reversingLights.Count());
}

public void Main(string argument)
{
    if (spotlights != null)
    {
           if (spotlightEnableLast != (spotlights[0] as IMyFunctionalBlock).Enabled)
           {
          SetBlocksEnable(rearlights, (spotlights[0] as IMyFunctionalBlock).Enabled);
           if (linkAmbientLights == true)
           {
             SetBlocksEnable(ambilights, (spotlights[0] as IMyFunctionalBlock).Enabled);
            }
            spotlightEnableLast = (spotlights[0] as IMyFunctionalBlock).Enabled;
        }
    }
    if (mainCockpit != null)
    {
        currentSpeed = mainCockpit.GetShipSpeed();
        bool isBraking =  ( (currentSpeed + speedThreshold) < lastSpeed) ;
        SetBlocksEnable(brakinglights, isBraking );
        MyShipVelocities velocities = mainCockpit.GetShipVelocities();
        double zVelocity = velocities.AngularVelocity.Z;
        Vector3D velocity = mainCockpit.GetShipVelocities().LinearVelocity;
        MatrixD mat = mainCockpit.WorldMatrix.GetOrientation();
        Vector3D localVelocity = Vector3D.Transform(velocity, MatrixD.Transpose(mat));
        double forwardSpeed = -localVelocity.Z;

        if (zVelocity > 0.2)
        { 
            SetBlocksEnable( forwardSpeed > 0 ? leftBlinkers : rightBlinkers, true);
            SetBlocksEnable( forwardSpeed > 0 ? rightBlinkers : leftBlinkers, false);
        }
        else if (zVelocity < -0.2)
        {   
            SetBlocksEnable(forwardSpeed  > 0 ? rightBlinkers : leftBlinkers, true);
            SetBlocksEnable( forwardSpeed  > 0 ? leftBlinkers : rightBlinkers, false);
        }
        else 
        {
            SetBlocksEnable(leftBlinkers, false);
            SetBlocksEnable(rightBlinkers, false);
        }
        SetBlocksEnable(reversingLights, forwardSpeed < -0.1 ? true : false);
        lastSpeed = currentSpeed;
    }
    else
    {
        Echo("Error: You must set up a main cockpit for turning and braking lights to work.");
    }
}

public void SetBlocksEnable(List<IMyTerminalBlock> blocks, bool enabled)
{
    if (blocks == null || blocks.Count() < 1)
        return;
    foreach (IMyFunctionalBlock block in blocks)
    {
        if (block != null)
            block.Enabled = enabled;
    }
}