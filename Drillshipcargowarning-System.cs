/*   
 *	Author: St3v3   
 *	Created: 02/24/2015   
 *	Description:    
 *	EN:   
 *	This script checks the current volume of all cargo's, connectors and drills and change the color of a light if it reached the limit you set.   
 *	Also it changes the color a second time if the max volume of the ship is reached.   
 *This will help you to find out when the ship can't hold any more items without checking the inventory manually.   
 *	You need to use it with a Timer Block to let the script restart itself.   
 *	DE:   
 *	Dieses Script überprüft den aktuellen Füllstand aller Frachtcontainer, Verbinder und Bohrer und ändert die Farbe eines  
 *Scheinwerfers wenn das von dir festgelegte Limit erreicht wurde.   
 *	Außerdem ändert es ein zweites mal die Farbe, wenn das Maximum erreicht wurde. Dies hilft dir dabei,   
 *optisch festzustellen, ob dein Schiff keine weiteren Items mehr aufnehmen kann, ohne vorher ins Inventar zu gucken.   
 *	Um die Prüfung in einer Schleife laufen zu lassen muss eine Zeitschaltuhr verwendet werden.   
 *   
 */   
string lightname = "warnlight"; // Name of light source to display volume   
   
Color full = new Color(255, 0, 0); // Color for max volume reached (RED)   
Color limit = new Color(255, 175, 0); // Color for first limit of volume reached (ORANGE)   
Color normal = new Color(255, 255, 255); // Color of light if current volume is under first limit (WHITE)   
   
float flimit = 0.90f; // Limit for first color change (0.95f = 95%	;	1.0f = 100%	;	0.50f = 50%)   
                               //you can change it to what you want, but not over 1.0f but this will only deaktivate the first limit.

bool warnfulldrill = false; //If true the light will change to the limit color (defaul orange) if one drill is full	(wished by Pioneer11)						   
   
/*   
 * Do not change anything under this line if you don't know what you do.   
 */   
   
List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock>();    
List<IMyTerminalBlock> connector = new List<IMyTerminalBlock>();    
List<IMyTerminalBlock> drill = new List<IMyTerminalBlock>();   
   
IMyTerminalBlock warnlight = null;   

public Program()
{
   Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
   
void Main()    
{   
    CheckCargo();     
}   
   
void CheckCargo()   
{   
      
    float cvolume = 0;    
    float mvolume = 0;   
	    bool warn = true;
  
    warnlight = GridTerminalSystem.GetBlockWithName(lightname);    
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);      
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connector);      
    GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drill);    
    
    for (int k = 0; k < cargo.Count; k++)    
    {    
        var inv = cargo[k].GetInventory(0);    
    
        cvolume += (float)inv.CurrentVolume;    
        mvolume += (float)inv.MaxVolume;    
            
    }     
        
    for (int k = 0; k < connector.Count; k++)     
    {        
        var inv = connector[k].GetInventory(0);     
     
        cvolume += (float)inv.CurrentVolume;     
        mvolume += (float)inv.MaxVolume;     
             
    }     
    
    for (int k = 0; k < drill.Count; k++)      
    {          
        var inv = drill[k].GetInventory(0);

		if((float)inv.CurrentVolume/(float)inv.MaxVolume >= 0.98f && warnfulldrill == true){
			     warn = true;
		}
      
        cvolume += (float)inv.CurrentVolume;      
        mvolume += (float)inv.MaxVolume;       
    }

		if(cvolume / mvolume >= 0.99f)     
		{     
			warnlight.GetActionWithName("OnOff_On").Apply(warnlight);     
			warnlight.SetValue("Color", full);         
		}else if(cvolume / mvolume >= flimit || warn == true){     
			warnlight.GetActionWithName("OnOff_On").Apply(warnlight);      
			warnlight.SetValue("Color", limit);     
		}else{     
			warnlight.GetActionWithName("OnOff_On").Apply(warnlight);      
			warnlight.SetValue("Color", normal);     
		}
}   