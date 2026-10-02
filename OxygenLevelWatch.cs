///////////////////////////////////////////////////////// 
////////////////  Oxygen Level Watch  /////////////////  
///////////////////  by Shadow  ////////////////////////  
//////////////////////////////////////////////////////// 
  
 // VERSION: 1.1.0  
// This Script will Show the Pressurestate and Value of each Vent in Group. 
 
// The blocks in this Group will be checked  
const string VentGroup = "WatchedVents";  
  
// Text Panel Group to display status  
const string panelGroup = "OxygenLevelsLCD";  
 
  
void updateLcd(string OutputString) {   
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();     
    GridTerminalSystem.GetBlockGroups(groups);     
    IMyTextPanel lcd;   
  
    for (int i = 0; i < groups.Count; i++) {      
           IMyBlockGroup group = groups[i];      
           if (group.Name == panelGroup) {        
                List<IMyTerminalBlock> groupBlocks = new List<IMyTerminalBlock>();              
                group.GetBlocks(groupBlocks);                    
                for (int j = 0; j < groupBlocks.Count; j++) {      
                     lcd = groupBlocks[j] as IMyTextPanel;     
                     lcd.SetValue("FontSize", 1.2f);   
                     // lcd.ShowTextOnScreen();   
                     lcd.WriteText("", false);
                     lcd.WriteText(OutputString, true);  

                }         
           }      
     }   
}  
 
float GetMyTerminalBlockCurrentDamage(IMyTerminalBlock block)  
{  
    return block.CubeGrid.GetCubeBlock(block.Position).CurrentDamage;  
} 
  
string getDetailedInfoValue(IMyTerminalBlock block, string name)     
{    
    string value = "";    
    string[] lines = block.DetailedInfo.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);    
    for (int i = 0; i < lines.Length; i++)     
    {    
        string[] line = lines[i].Split(':');    
        if (line[0].Contains(name))     
        {    
            value = line[1].Substring(0);    
            break;    
        }    
    }    
    return value;    
}   

void Main(string argument)  
{  
    List<IMyBlockGroup> groups = new List<IMyBlockGroup>();   
    GridTerminalSystem.GetBlockGroups(groups);  
  
    IMyAirVent myVent; 
    string OutString = "Oxygen Distribution\n";

    for (int i = 0; i < groups.Count; i++) {    
           IMyBlockGroup group = groups[i];    
           if (group.Name == VentGroup) {                      
                List<IMyTerminalBlock> groupBlocks = new List<IMyTerminalBlock>();              
                group.GetBlocks(groupBlocks);      
                for (int j = 0; j < groupBlocks.Count; j++) {    
                    myVent = groupBlocks[j] as IMyAirVent;  
                    OutString = OutString +  groupBlocks[j].CustomName.Replace("Air Vent ", "");
                    // OutString = OutString  + ": " + myVent.GetValueFloat("Room pressure") ;

                    //OutString = OutString + ": "  + getDetailedInfoValue(groupBlocks[j], "Room pressure");

                    
 
                   if (!myVent.CanPressurize)  
                    {  
                        //leak = true;  
                        OutString = OutString + ": FAILURE"; 
                     }     
                    else
                    {
                        OutString = OutString + ": "  + getDetailedInfoValue(groupBlocks[j], "Room pressure");
                    }

                     OutString = OutString +  "\n";
                }       
           }    
     } 
  
    updateLcd(OutString);  
}
