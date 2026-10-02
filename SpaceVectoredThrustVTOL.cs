public Dictionary < string, object > inputs = null;
public bool controlModule = true;
public bool forwardIsPressed = false;
public bool backwardIsPressed = false;
public bool upIsPressed = false;
public bool downIsPressed = false;
public float maxRPM = 0f;
public float speed = 8f;
public float currentTargetPos = 0f;
public string face = "front";
public bool firstRun = true;
public float delta;
public float previousPos;
public float targetAngle = 0f;
public float targetAngleR = 0f;
public float targetAngleL = 0f;



public Program()

{

 Echo("Just Compiled");
 Runtime.UpdateFrequency = UpdateFrequency.Update1;

}



public void Save()

{

}



public void Main(string argument, UpdateType updateSource)

{
    //Echo($"Last Runtime {Runtime.LastRunTimeMs.Round(2)}ms");
    Echo("speed : " + speed);

 inputs = Me.GetProperty("ControlModule.Inputs") ? .As < Dictionary < string, object >> () ? .GetValue(Me);

 if (inputs == null)
  throw new Exception("Control Module mod not found!");

  
 doMovementInput(argument);
 firstRun = false;

}


public void doMovementInput(string arg) {
 // Get Blocks v0 are rotors v1 are thrusters 
 string ERR_TXT = "";
 List < IMyTerminalBlock > v0 = new List < IMyTerminalBlock > ();
 List < IMyTerminalBlock > rightRotors = new List < IMyTerminalBlock > ();
 
 // Find all rotors in group Vtol Rotors
 if (GridTerminalSystem.GetBlockGroupWithName("Vtol Rotors") != null) {
  GridTerminalSystem.GetBlockGroupWithName("Vtol Rotors").GetBlocksOfType < IMyMotorAdvancedStator > (v0, filterThis);
  if (v0.Count == 0) {
   ERR_TXT += "group Vtol Rotors has no Advanced Rotor blocks\n";
  }
 } else {
  ERR_TXT += "group Vtol Rotors not found\n";
 }
 
 

 
 
 Echo("maxRPM : " + maxRPM);
 List < IMyTerminalBlock > v1 = new List < IMyTerminalBlock > ();

 var blocks = new List < IMyTerminalBlock > ();

 // Find all thrusters with the word Vtol in it
 GridTerminalSystem.GetBlocksOfType < IMyTerminalBlock > (blocks, block => (block is IMyThrust));

 for (int i = blocks.Count - 1; i >= 0; i--) {
  if (blocks[i] is IMyThrust && blocks[i].CustomName.IndexOf("Vtol") > -1) {
   v1.Add((IMyThrust) blocks[i]);
  }
  blocks.RemoveAt(i);
 }
 if (v1.Count == 0) {
  ERR_TXT += "group Vtol Thrusters not found\n";
 }

 // display errors
 if (ERR_TXT != "") {
  Echo("Script Errors:\n" + ERR_TXT + "(make sure block ownership is set correctly)");
  return;
 } 
 
  if(firstRun && Math.Abs(maxRPM - 0.001f) >= 0) {
	maxRPM = v0[0].GetMaximum<float>("Velocity");
	speed = maxRPM;
 }
 
 // Limit movement to forward and reverse, turned off
 /*
 for(int i = 0; i < v0.Count; i++) {
    v0[i].SetValue("LowerLimit", (float)0);
    v0[i].SetValue("UpperLimit", (float)180);
  }
*/
 // Set up controlModule
 if (controlModule) {
  // setup control module
  Dictionary < string, object > inputs = new Dictionary < string, object > ();
  try {
   this.inputs = Me.GetValue < Dictionary < string, object >> ("ControlModule.Inputs");
   Me.SetValue < string > ("ControlModule.AddInput", "all");
   Me.SetValue < bool > ("ControlModule.RunOnInput", true);
   Me.SetValue < int > ("ControlModule.InputState", 1);
   Me.SetValue < float > ("ControlModule.RepeatDelay", 0.016f);
  } catch (Exception e) {
   controlModule = false;
  }
 }

 // Find if w or s is pressed
 if (controlModule) {

  if (this.inputs.ContainsKey("w")) {
   forwardIsPressed = true;
   Echo("Forward Pressed");
  } else {
   forwardIsPressed = false;
  }

  if (this.inputs.ContainsKey("s")) {
   backwardIsPressed = true;
   Echo("Backward Pressed");
  } else {
   backwardIsPressed = false;
  }
  
  if (this.inputs.ContainsKey("numdecimal")) {
   upIsPressed = true;
   Echo("Up Pressed");
  } else {
   upIsPressed = false;
  }
  if (this.inputs.ContainsKey("leftshift")) {
   downIsPressed = true;
   Echo("Up Pressed");
  } else {
   downIsPressed = false;
  }
 }

 // Check if thrusters are firing, not used
 bool thrustersOn = true;
 for (int i = 0; i < v1.Count; i++) {
  if (!(((IMyThrust) v1[i]).CurrentThrust >= 0)) {
   thrustersOn = false;
   break;
  }
 }
 
  if(forwardIsPressed) {
	// between 270 and 180 3/2pi and pi is 5/4pi
	if(downIsPressed) {
		targetAngle = 1.75f * 3.14f;
		targetAngleR = 2.25f * 3.14f;
	}
	else if(upIsPressed) {
		targetAngle = 1.25f * 3.14f;
		targetAngleR = 0.75f * 3.14f;
	}
	else {
		targetAngle = 3.14f;
		targetAngleR = 3.14f;
	}
 }
 else if(backwardIsPressed) {
	// 0 avged with 3/2pi is 3/4
	if(downIsPressed) {
		targetAngle = .75f * 3.14f;
		targetAngleR = 1.25f * 3.14f;
	}
	// 0 avged with 1/2pi
	else if(upIsPressed) {
		targetAngle = .25f * 3.14f;
		targetAngleR = 1.75f * 3.14f;
	}
	else {
		targetAngle = 0f;
		targetAngleR = 0f;
	}
 }
 else if(downIsPressed) {
	targetAngle = .5f * 3.14f;
	targetAngleR = 1.5f * 3.14f;
 }
 else if(upIsPressed) {
	targetAngle = 1.5f * 3.14f;
	targetAngleR = .5f * 3.14f;
 }

 bool isLockAble = true;
 if(firstRun) {isLockAble = false;}
 Echo("targetAngle : " + targetAngle);
 Echo("targetAngleR : " + targetAngleR);
 Echo("cutangle : " + cutAngle(targetAngle));
 Echo("cutangleR : " + cutAngle(targetAngleR));
 
 for (int i = 0; i < v0.Count; i++) {
   Echo("(IMyMotorAdvancedStator) v0[i]).Angle " + i + " : "+ ((IMyMotorAdvancedStator) v0[i]).Angle);
   //if(v0[i].CustomName.IndexOf("R") > -1 && (Math.Abs(cutAngle(((IMyMotorAdvancedStator) v0[i]).Angle - targetAngleR) ) > .005f) && (Math.Abs(Math.Abs(cutAngle(((IMyMotorAdvancedStator) v0[i]).Angle - targetAngleR) ) - 3.14f) > .005f)) {
   if(v0[i].CustomName.IndexOf("Right") > -1 && !areRadiansEqual(((IMyMotorAdvancedStator) v0[i]).Angle, targetAngleR)) {
		isLockAble = false;
		/*
		Echo("Is lockable check failed R rotor "+i+": ");
		Echo(""+((IMyMotorAdvancedStator) v0[i]).Angle);
		Echo("first desired angle delta R : ");
		Echo(""+(deltaRad(((IMyMotorAdvancedStator) v0[i]).Angle, targetAngleR)));
		*/
		((IMyMotorAdvancedStator)v0[i]).SetValueBool("RotorLock", false);
		
   }
  //else if ((Math.Abs(cutAngle(((IMyMotorAdvancedStator) v0[i]).Angle - targetAngle) ) > .005f) && (Math.Abs(Math.Abs(cutAngle(((IMyMotorAdvancedStator) v0[i]).Angle - targetAngle) ) - 3.14f) > .005f)) {
  else if (v0[i].CustomName.IndexOf("Right") <= 0 && !areRadiansEqual(((IMyMotorAdvancedStator) v0[i]).Angle, targetAngle)) {
   isLockAble = false;
	/*
    Echo("Is lockable check failed L rotor "+i+": ");
	Echo(""+((IMyMotorAdvancedStator) v0[i]).Angle);
	Echo("tar: "+targetAngle);
	Echo("first desired angle delta L : ");
	Echo(""+(deltaRad(((IMyMotorAdvancedStator) v0[i]).Angle, targetAngle)));
	*/
	((IMyMotorAdvancedStator)v0[i]).SetValueBool("RotorLock", false);
	//break;
  }
  else {
	((IMyMotorAdvancedStator)v0[i]).SetValueBool("RotorLock", true);
  }
 }
 
 
 
  Echo("isLockAble : " +isLockAble);
  Echo("down : " + .5f * 3.14f);
  Echo("up : " + 1.5f * 3.14f);
	
 for (int i = 0; i < v0.Count; i++) {
	if (v0[i].CustomName.IndexOf("R") > -1) {
		Echo("Vtol rotor with R found no push");
		/*
		if((((Math.Abs(Math.Abs(targetAngle) - .5f * 3.14f)) < .01f || Math.Abs(Math.Abs(targetAngle) - 1.5f * 3.14f) < .01f))) {
			Echo("Vtol rotor with R found moving in range : "+Math.Abs(targetAngle-3.14f));
			targetAngleR = Math.Abs(targetAngle-3.14f);
		}
		
		else {
			targetAngleR = targetAngle;
		}*/
	}
 }
  for (int i = 0; i < v0.Count; i++) {

	if (v0[i].CustomName.IndexOf("L") > -1){
			Echo("Vtol rotor with L found moving it pi from Target : "+targetAngle);
		}
 }
 
 bool keyIsPressed = forwardIsPressed || backwardIsPressed || upIsPressed || downIsPressed;
 if(keyIsPressed) {
	speed = maxRPM;
   for (int i = 0; i < v0.Count; i++) {
   //v0[i].SetValue("Velocity", (float) 8);
	if(v0[i].CustomName.IndexOf("R") > -1) {
	
		Echo("Vtol rotor with R found.");
		/*
		if(((Math.Abs(Math.Abs(targetAngle) - .5f * 3.14f)) < .01f || Math.Abs(Math.Abs(targetAngle) - 1.5f * 3.14f) < .01f))  {
			Echo("Vtol rotor with R found moving it pi from Target");
			setPos((IMyMotorAdvancedStator)v0[i], (targetAngleR));
		}
		else {
			Echo("Vtol rotor with R found moving it Target");
			setPos((IMyMotorAdvancedStator)v0[i], (targetAngleR));
		}*/
		/*
		if(Math.Abs(targetAngle - targetAngleR) > .1) { setPos((IMyMotorAdvancedStator)v0[i], (targetAngleR)); }
		else { setPos((IMyMotorAdvancedStator)v0[i], (targetAngle)); }
		*/
		setPos((IMyMotorAdvancedStator)v0[i], (targetAngleR));
	}
   currentTargetPos = targetAngle;
  }
  
  
   for (int i = 0; i < v0.Count; i++) {
   //v0[i].SetValue("Velocity", (float) 8);
	if(v0[i].CustomName.IndexOf("L") > -1) {
		Echo("Vtol rotor with L found moving it pi from Target");
		setPos((IMyMotorAdvancedStator)v0[i], targetAngle);
	}
   }
  
  
  Echo("key Pressed");
  if (isLockAble) {
   for (int i = 0; i < v1.Count; i++) {
    v1[i].ApplyAction("IncreaseOverride");
   }
  } 
  else {
	for (int i = 0; i < v0.Count; i++) {
		((IMyMotorAdvancedStator)v0[i]).SetValueBool("RotorLock", false);
	}
  }
 }
  

  // Turn off thrusters when key not pressed or in rotation
 if ((!keyIsPressed) || (!isLockAble)) {
  for (int i = 0; i < v1.Count; i++) {
   v1[i].ApplyAction("DecreaseOverride");
  }
 }
  
  // Single click functionality, not working
  /*
  if(Math.Abs(speed) <= 1) {
	 speed = speed*1.05f;
	 Echo("speed reset : " + speed);
  }
  Echo("currentTargetPos : " + currentTargetPos);
  for (int i = 0; i < v0.Count; i++) {
	if((((IMyMotorAdvancedStator)v0[i]).GetValueBool("RotorLock") == false) && Math.Abs(((IMyMotorAdvancedStator) v0[i]).Angle - currentTargetPos) >= 0.1) {
		Echo("rotor "+i+" : " + ((IMyMotorAdvancedStator) v0[i]).Angle);
		//speed = speed*0.995f;
		Echo("speed : " + speed);
		setPos((IMyMotorAdvancedStator)v0[i], currentTargetPos);
	}
  } */ 
 

}

// set the angle to be between 0 and 2pi radians (0 and 360 degrees)
	// this takes and returns radians
	float cutAngle(float angle) {
		while(angle > Math.PI) {
			angle -= 2*(float)Math.PI;
		}
		while(angle < -Math.PI) {
			angle += 2*(float)Math.PI;
		}
		return angle;
	}

	// move rotor to the angle (radians), make it go the shortest way possible
	public void setPos(IMyMotorAdvancedStator theBlock, float x)
	{
		theBlock.Enabled = true;
		x = cutAngle(x);
		float velocity = speed;
		float x2 = cutAngle(theBlock.Angle);
		if(Math.Abs(x - x2) < Math.PI) {
			//dont cross origin
			if(x2 < x) {
				theBlock.SetValue<float>("Velocity", velocity * Math.Abs(x - x2));
			} else {
				theBlock.SetValue<float>("Velocity", -velocity * Math.Abs(x - x2));
			}
		} else {
			//cross origin
			if(x2 < x) {
				theBlock.SetValue<float>("Velocity", -velocity * Math.Abs(x - x2));
			} else {
				theBlock.SetValue<float>("Velocity", velocity * Math.Abs(x - x2));
			}
		}
	}
	
	public float pinchRad(float rads) {
        while(rads < 0f) rads += 2f * 3.14f;    
        
        if(rads > 3.14f) return rads - 2f* 3.14f;
        else if(rads < -3.14f) return rads + 2f * 3.14f;
        
        return rads;
     }
	 
	public float deltaRad(float rad1, float rad2) {
		/*
		 Echo("pinch 1 : ");
		 Echo(""+pinchRad(rad1));
		 Echo("pinch 2 : ");
		 Echo(""+pinchRad(rad2));
		 */
         return Math.Abs(pinchRad(rad1) - pinchRad(rad2));
     }
     
     public bool areRadiansEqual(float rad1, float rad2) {
         return deltaRad(rad1, rad2) < .05f; //Aproximately two degrees or 1 percent of a circle
     }

bool filterThis(IMyTerminalBlock block) {
 return block.CubeGrid == Me.CubeGrid;
}