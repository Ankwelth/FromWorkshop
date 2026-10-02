void Main() {  
      
    var airVents = SearchBlocksByName("Air Vent");  
    var pressureStatus = "Pressurized";  

    	string SoundBlockName = "Sound Block Hull Breach";
    string RotatingLightName = "Rotating Light Hull Breach";

    List<IMyTerminalBlock> mySoundBlock = new List<IMyTerminalBlock>();  
    List<IMyTerminalBlock> myRotatingLight = new List<IMyTerminalBlock>();

    	GridTerminalSystem.SearchBlocksOfName(SoundBlockName, mySoundBlock);  
    GridTerminalSystem.SearchBlocksOfName(RotatingLightName, myRotatingLight);

    for (int i = 0; i < airVents.Count; i++) {  
        string pressureInfo = airVents[i].DetailedInfo;  
        if(pressureInfo.IndexOf("Not pressurized") != -1) {  
            if(pressureStatus == "Pressurized")  
            pressureStatus = "Depressurized";  
        }  
    }  
   
	   for(int i = 0; i < mySoundBlock.Count; i++) { 
		      if(pressureStatus == "Depressurized") {  
      mySoundBlock[i].GetActionWithName("PlaySound").Apply(mySoundBlock[i]);  
		      }  
	   } 
    for(int i = 0; i < myRotatingLight.Count; i++) {
        if(pressureStatus == "Depressurized") {
        myRotatingLight[i].GetActionWithName("OnOff_On").Apply(myRotatingLight[i]);
        }
        else {
        myRotatingLight[i].GetActionWithName("OnOff_Off").Apply(myRotatingLight[i]);
        }
    }
}  

   
	   List<IMyTerminalBlock> SearchBlocksByName(string blockName) {  
		      var blocks = new List<IMyTerminalBlock>();  
		      GridTerminalSystem.SearchBlocksOfName(blockName, blocks);  
		      return blocks;  
	   }