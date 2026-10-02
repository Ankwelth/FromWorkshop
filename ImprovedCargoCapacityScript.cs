// What the your cargo group.    
const string groupName = "Capacity";

// Flag to disable printing to cockpit:
static bool printToCockpit = false;

// What the cockpits name needs to be.       
const string displayName = "Cockpit"; 
    
// The Timer to be triggered When Full   
const string timerName = "Timer Full Cap"; 
 
// The Timer to be Triggered When Capacity of Cargo Group is at 0 pct  for unloading
 const string timerName2 = "Timer Unloaded"; 
 
// The number of percent integers the Display shows   
const int roundpercent = 1;  
   
// End of Editing!    
      
IMyCockpit cockpit;
IMyTextSurface display;
IMyTimerBlock timer; 
IMyTimerBlock timer2;

 public Program()      
{      
    Runtime.UpdateFrequency = UpdateFrequency.Update10;     
}      
void Main(string arg)     
{     
       
    float usedVolume = 0.0f;     
    float maxVolume = 0.0f;     
      
    timer = GridTerminalSystem.GetBlockWithName(timerName) as IMyTimerBlock;
    timer2 = GridTerminalSystem.GetBlockWithName(timerName2) as IMyTimerBlock;  

    if(printToCockpit) {
        cockpit = GridTerminalSystem.GetBlockWithName(displayName) as IMyCockpit;

        	//Change the number in the following line to match the selected dispaly you want.  Used list below for Reference
        display = cockpit.GetSurface(2);

        	//GetSurface Locations within Cockpits
        		//Large Grid
        			// Industrial - 0= Large Display, 1= Top Left, 2= Top Center, 3= Top Right. 4= Keyboard, 5= Number Pad
        			// Utility - 0= Top Center Screen, 1= Top Left, 2= Top Right, 3= Keyboard, 4= Bottom Left, 5= Bottom Right
        		//Small Grid
        			// Fighter - 0= Top Center Screen, 1= Top Left, 2= Top Right, 3= Keyboard, 4= Bottom Center, 5= Number Pad
        			// Industrial - 0= Top Left, 1= Top Center, 2= Top Right. 3= Keyboard, 4= Number Pad
        			// Utility - 0 = Center Display, 1 = Left Display, 2 = Right Display, 3 = Keyboard 
    }

    var group = this.GridTerminalSystem.GetBlockGroupWithName(groupName);   
    if(group == null)   
    {   
        this.Echo("No Group named " + "\n'' " + groupName + " ''\n");   
    }  
    if(printToCockpit && display == null)   
    {   
        this.Echo("No display named " + "\n'' " + displayName + " ''\n");    
    }    
    if(group == null || (printToCockpit && display == null))
    {  
        return;  
    }  
  
    var groupBlocks = new List<IMyTerminalBlock>();   
    group.GetBlocks(groupBlocks);   
    for(int i = 0; i < groupBlocks.Count; i++)   
    {   
        var block = groupBlocks[i];   
        usedVolume += (float)block.GetInventory(0).CurrentVolume;   
        maxVolume += (float)block.GetInventory(0).MaxVolume;   
    }   
     
    float pctUsed = 100.0f * usedVolume / maxVolume;     
         
         
    string displayText = String.Format("" +groupName +"\nOverall: {0}%\n", (int)pctUsed);      
         
    for (int x = 0; x <= 10; x++) {      
        if (pctUsed >= 100 - x * 10) {      
            displayText += "[ <=========> ]\n";      
        } else {      
            displayText += "|              |\n";      
        }      
    }    
      
    if (pctUsed >= 99) {  
        timer.ApplyAction("TriggerNow"); 
    }  

   if (pctUsed <= 0) {
       timer2.ApplyAction("TriggerNow");
    }
    
    var fill = (float)(usedVolume / maxVolume);
    var roundfill = (int)Math.Round(fill * 100.0 / roundpercent) * roundpercent;
    var log = "Capacity: " + (fill).ToString("P2");
    Echo(log);
    
    if (display != null && group != null) {
        display.WriteText(displayText, false);
    }     
}