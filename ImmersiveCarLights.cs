/**********************************************************************
        Car Lighting Script: By Morphik
        v. 4.0
**********************************************************************/

// Vehicle Controled by a Remote Control block?
bool useRemoteControl = false;

const string cockpitName = "Cockpit";               // Drivers Cockpit Name.
const string remoteName = "Remote Control"; // The Remote Control used to Drive.
const string headLight = "HeadLight";              // Head Light Name.
const string brakeLight = "BrakeLight";           // Brake Light Name.
const string leftIndicator = "Indicator-Left";     // Left Indicator Name.
const string rightIndicator = "Indicator-Right"; // Right Indicator Name.
const string reverseLight = "ReverseLight";     // Reverse Light Name.
const string speedometer = "Speedometer";   // LCD to display a speedometer on your vehicle. This will be in meters per second.

// Maximum speed before turn indicators stop turning on.
int indicatorSpeed = 10; // this is in m/s

// Let Program use other grids?
bool useSubGrid = false;

List<IMyTerminalBlock> allCockpit = new List<IMyTerminalBlock>();
List<IMyTerminalBlock> allRemoteControl = new List<IMyTerminalBlock>();       
List<IMyTerminalBlock> allLights = new List<IMyTerminalBlock>();   
List<IMyTerminalBlock> allSpotlights = new List<IMyTerminalBlock>();   
List<IMyTerminalBlock> allLcd = new List<IMyTerminalBlock>();   

Vector3D position = new Vector3D(0,0,0);    
Vector3 WASDControl;  

public Program()    
{    
    Runtime.UpdateFrequency = UpdateFrequency.Update10;   
}       

void Main()      
{       
    if (useSubGrid == true)
    {
        GridTerminalSystem.GetBlocksOfType<IMyCockpit>(allCockpit); 
        GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(allRemoteControl);
        GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(allLights); 
        GridTerminalSystem.GetBlocksOfType<IMyReflectorLight>(allSpotlights);         
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allLcd);  
    }
    else if (useSubGrid == false)
    {
        GridTerminalSystem.GetBlocksOfType<IMyCockpit>(allCockpit, b => b.CubeGrid == Me.CubeGrid);
        GridTerminalSystem.GetBlocksOfType<IMyRemoteControl>(allRemoteControl, b => b.CubeGrid == Me.CubeGrid);    
        GridTerminalSystem.GetBlocksOfType<IMyLightingBlock>(allLights, b => b.CubeGrid == Me.CubeGrid);  
        GridTerminalSystem.GetBlocksOfType<IMyReflectorLight>(allSpotlights, b => b.CubeGrid == Me.CubeGrid);
        GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allLcd, b => b.CubeGrid == Me.CubeGrid);  
    }
    
    bool handBrake = false;
    bool lightsOn = false;
    double mySpeed = get_speed(); 
    StringBuilder outputSpeed = new StringBuilder();
    
    outputSpeed.Append("Speed: \n" + mySpeed.ToString() + " m/s");

    foreach(IMyTextPanel lcd in allLcd)
    {
        if (lcd.CustomName.Contains(speedometer))
        {
            lcd.ContentType = ContentType.TEXT_AND_IMAGE;
            lcd.Alignment = TextAlignment.CENTER;
            lcd.FontSize = 2;
            lcd.WriteText(outputSpeed.ToString());
        } 
    }
    
    if (useRemoteControl == false)
    {
    		foreach(IMyCockpit cockpit in allCockpit)     
        		{     
            			if (cockpit.CustomName.Contains(cockpitName))
            			{
                				handBrake = cockpit.HandBrake;   
                				WASDControl	= cockpit.MoveIndicator; 
            			}
        			/********* 
        			A => X:-1 
        			D => X: 1 
        			C => Y:-1 
        			Sp => Y: 1 
        			W => Z:-1 
        			S => Z: 1 
        			**********/
        		}     
    }

    else if (useRemoteControl == true)
    {
		    foreach(IMyRemoteControl remotecontrol in allRemoteControl)    
    	   	{    
            			if (remotecontrol.CustomName.Contains(remoteName))
        	   		{
            	   			handBrake = remotecontrol.HandBrake;  
            	   			WASDControl = remotecontrol.MoveIndicator;
            			}
        			/*********
        			A => X:-1
        			D => X: 1
        			C => Y:-1
        			Sp => Y: 1
        			W => Z:-1
        			S => Z: 1
        			**********/
        		}
    }
    
    foreach (IMyReflectorLight spotLight in allSpotlights)     
    {     
        if (spotLight.CustomName.Contains(headLight))
        {
            lightsOn = spotLight.Enabled;
        }
    }

    foreach (IMyLightingBlock light in allLights)
    {
        if (light.CustomName.Contains(leftIndicator) || light.CustomName.Contains(rightIndicator))
        {
            light.Color = new Color(255, 255, 0);    
            light.BlinkIntervalSeconds = 1;
            light.BlinkLength = 20;
        } 
        if (light.CustomName.Contains(brakeLight))
        {
            light.Color = new Color(255, 0, 0);
        }
        
        if (mySpeed < indicatorSpeed)
        {
            if (WASDControl.X < 0)
            {
                if (light.CustomName.Contains(leftIndicator))
                {
                    light.Enabled = true;
                }
                else if (light.CustomName.Contains(rightIndicator))
                {
                    light.Enabled = false;
                }
            }
            if (WASDControl.X > 0)
            {
                if (light.CustomName.Contains(rightIndicator))
                {
                    light.Enabled = true;
                }
                else if (light.CustomName.Contains(leftIndicator))
                {
                    light.Enabled = false;
                }
            }
        }
        if (light.CustomName.Contains(leftIndicator) || light.CustomName.Contains(rightIndicator))
        {
            if (WASDControl.X == 0)
            {
                
                light.Enabled = false;
                
            }
            
            else if (mySpeed >= indicatorSpeed)
            {
                
                light.Enabled = false;
            }
        }
        
        
        if (light.CustomName.Contains(reverseLight))
        {
            light.Color = new Color(255, 255, 255);
            light.Intensity = 5;  
            light.Radius = 3;
            
            if (mySpeed > 0 && WASDControl.Z > 0)
            {
                light.Enabled = true;
            }
            else
            {
                light.Enabled = false;
            }
        }
        if (light.CustomName.Contains(brakeLight))
        {
            if (handBrake == true && lightsOn == false)
            {
                
                light.Enabled = true;
                light.Intensity = 5;  
                light.Radius = 3;
                
            }
            else if (handBrake == true && lightsOn == true)
            {
                
                light.Enabled = true;
                light.Intensity = 5;  
                light.Radius = 3;
            }
            else if (handBrake == false && lightsOn == false)
            {
                
                if(WASDControl.Y > 0)//Press space 
                { 
                    light.Enabled = true;
                    light.Intensity = 5;  
                    light.Radius = 3;
                } 
                else //Space is free 
                { 
                    light.Enabled = false;
                    light.Intensity = 0;  
                    light.Radius = 0;
                } 
            }
            else if (handBrake == false && lightsOn == true)
            {
                
                if(WASDControl.Y > 0)//Press space 
                { 
                    light.Enabled = true;
                    light.Intensity = 5;  
                    light.Radius = 3;
                } 
                else//Space is free 
                { 
                    light.Enabled = true;
                    light.Intensity = 1;  
                    light.Radius = 2;
                } 
            }
        }
    }     
    allCockpit.Clear();
    	allRemoteControl.Clear();
    allLights.Clear();
    allSpotlights.Clear();
    	allLcd.Clear();
} 

double get_speed()     
{     
    Vector3D current_position = Me.GetPosition(); // the position of this programmable block 
    double speed = ((current_position-position)*6).Length();    
    // double speed = (((current_position-position)*60)*60/1000).Length(); // KMH  
    // double speed = ((current_position-position)*60).Length(); // M/H  
    // double speed = ((current_position-position)*6).Length(); // M/S  
    
    position = current_position; // update the global variable, which will be used on the next run     
    speed = Math.Round(speed,0);  

    return speed;     
}  