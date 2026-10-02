// Cargo Mass Limitter
// by : KnightReaver905
// 
// Designed to allow SAMV2 ships to take off before getting too heavy.
// Can be used with SAMV2.
// Version 1.0 //Date:01-10-2024 1634
// DO NOT EDIT VARABLES HERE - EDIT THEM IN THE CUSTOM DATA OF THE PROGRAMABLE BLOCK
//  
string PBNewName = "Computer: Cargo Mass Limiter";//Name to Rename the Programable Block 
string ThrustTimerName = "[CMLT]";//key word all ThrustTimers with this will trigger!
string WeightTimerName = "[CMLW]";//key word all ThrustTimers with this will trigger!
float MinThrusttoMass = 0f;//The Thrust to mass nessary for takeoff.
float UnDockedMass = 0.0f;
float EffectiveThrust = 0.0f;
float CurrentEffectiveThrust = 0;
string MassLcd = "[CML]";
string CMassLcd = "[CMLC]";
bool ColorChoice = true;
bool Onload = false;
float CargoMass2 = -1;
float CargoPer = 0;
float SetPer = 80;
string saveitem = "";
string StatusColor = "G";
        List<IMyShipController> shipControllers = new List<IMyShipController>();
        List<IMyThrust> thrusters = new List<IMyThrust>();
		List<IMyTerminalBlock> Connectors = new List<IMyTerminalBlock>();
		List<IMyTerminalBlock> Timers = new List<IMyTerminalBlock>();
	    List<IMyTerminalBlock> ThrustTimer = new List<IMyTerminalBlock>();
		List<IMyTerminalBlock> WeightTimer = new List<IMyTerminalBlock>();
		List<IMyTerminalBlock> allInventories = new List<IMyTerminalBlock>();  
        List<IMyTerminalBlock> LCDS = new List<IMyTerminalBlock>();	
        List<IMyTerminalBlock> MassLCD = new List<IMyTerminalBlock>();	
		List<IMyTerminalBlock> CMassLCD = new List<IMyTerminalBlock>();
		
public Program()
{ 
  Runtime.UpdateFrequency = UpdateFrequency.Update100;

    CustomConfig(Me);
	string PBName = Me.CustomName;
    if (PBName != PBNewName) {Me.CustomName = PBNewName;}

   string[] storedData = Storage.Split(';');
    // If there's at least one item in the storage data...
    if (storedData.Length >= 1)
    {
        // Retrieve first item. C# arrays are 0-indexed, meaning the first item in the list is item 0.
        saveitem = storedData[0];
    }

    // If there's at least two items in the storage data...
    if (storedData.Length >= 2)
    {
        // Retrieve the second item. This time we want to try to convert it into a number.
        float.TryParse(storedData[1], out UnDockedMass);
    }
}

public void Save()
{
    // Combine the state variables into a string separated by the ';' character
    Storage = string.Join(";",
        saveitem ?? "",
        UnDockedMass
    );
}
        public void Main(string argument)
        {
		if(argument.ToUpper().StartsWith("ON")){Onload = true;}
		if(argument.ToUpper().StartsWith("OFF")){Onload = false;}
		string ERR_TXT ="";
		string ERR_TXT2 ="";
            // Get controllers (only one REQUIRED)
			if (shipControllers.Count == 0){
			Echo("Getting Controllers");
            GridTerminalSystem.GetBlocksOfType<IMyShipController>(shipControllers, filterThis);
			if(shipControllers.Count == 0) {
			ERR_TXT += "Ship Controller NOT found!\n";
			}
			}
			//Get Connectors
			if (Connectors.Count == 0){
			Echo("Getting Connectors");
			GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(Connectors, filterThis);
			if(Connectors.Count == 0) {
			ERR_TXT += "Connector(s) NOT found!\n";
			}
			}
			//Get LCDS
			if(MassLCD.Count == 0 || CMassLCD.Count == 0) {
            Echo("Getting LCD Panels.");
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(LCDS);
            if(LCDS.Count > 0) {
            for(int i = 0; i < LCDS.Count; i++) {
            if(LCDS[i].CustomName.IndexOf(MassLcd) > -1) {
            MassLCD.Add(LCDS[i]);
            }
			if(LCDS[i].CustomName.IndexOf(CMassLcd) > -1) {
            CMassLCD.Add(LCDS[i]);
            }
            }
			if(MassLCD.Count == 0) {
            ERR_TXT += "LCD/Text Panel "+MassLcd+" Not found\n";
            }
			if(MassLCD.Count == 0) {
            ERR_TXT2 += "LCD/Text Panel "+CMassLcd+" Not found\n";
            }
            }
			else {ERR_TXT += "LCD/Text Panels NOT found!\n";}
            }
      
	  if (Timers.Count == 0){
	  GridTerminalSystem.GetBlocksOfType<IMyTimerBlock>(Timers, filterThis);
	  if(Timers.Count > 0) {
	  for(int i = 0; i < Timers.Count; i++) {
      if(Timers[i].CustomName.IndexOf(ThrustTimerName) > -1) {
        ThrustTimer.Add(Timers[i]);
      }
	  if(Timers[i].CustomName.IndexOf(WeightTimerName) > -1) {
        WeightTimer.Add(Timers[i]);
      }
    } 
	  if(ThrustTimer.Count == 0) {
            ERR_TXT += "Timer "+ThrustTimerName+" Not found\n";
            }
			if(WeightTimer.Count == 0) {
            ERR_TXT2 += "Timer "+WeightTimerName+" Not found\n";
            }
			}
		else {ERR_TXT += "Timers NOT found!\n";}
			
			}
				
			
	// display errors
  if(ERR_TXT != "") {
    Echo("Script Errors:\n"+ERR_TXT+"(If Present DO YOU Own them?)");
    return;
  }
  if(ERR_TXT2 != "") {Echo("Optional Items:\n"+ERR_TXT2+"(If Present DO YOU Own them?)");}
  else {Echo("");}
            ////LOGIC///
			
				int CargoMass = 0;
				float ShipCargoMass = 0;
                IMyShipController shipController = shipControllers.First(filterThis);
                MyShipMass shipMass = shipController.CalculateShipMass();
                float emptyShipMass = shipMass.BaseMass; // mass not including inventory / cargo
                float currentTotalMass = shipMass.TotalMass; // mass including inventory cargo
				
				GridTerminalSystem.GetBlocksOfType<IMyInventoryOwner>(allInventories, filterThis);  
				for(int i = 0; i < allInventories.Count; i++)  
				{  
				CargoMass += (int)((IMyInventory)allInventories[i].GetInventory(0)).CurrentMass;  

				if((IMyInventory)allInventories[i].GetInventory(1) != null)  
				{  
				CargoMass += (int)((IMyInventory)allInventories[i].GetInventory(1)).CurrentMass;  
				}
								
				}
			bool Docked = false;
			for(int i = 0; i < Connectors.Count; i++) {
			if(((IMyShipConnector)Connectors[i]).Status == MyShipConnectorStatus.Connected) {
			Docked = true;
			break;
			}
			}
			if (!Docked && emptyShipMass != UnDockedMass){
			UnDockedMass = emptyShipMass;}
			if (!Docked && CargoMass2 != -1){
			CargoMass2 = -1;}
			
			ShipCargoMass = UnDockedMass + CargoMass;
				// Grab all of the thrusters on the ship
                GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, filterThis);

                // Calculate total max thrust in Newtons
                float maxEffectiveThrust = 0;
                float currentEffectiveThrust = 0;
				

                foreach (IMyThrust thruster in thrusters)
                {
                    if (thruster.Orientation.Forward == Base6Directions.Direction.Up) // only add the thruster if its grid thrust direction is "up"
                    {
                        // Grab max effective thrust for the current atmosphere, assuming the thruster is 100% perpendicular to the natural gravity well...
                        maxEffectiveThrust += thruster.MaxEffectiveThrust;
                        currentEffectiveThrust += thruster.CurrentThrust;
                    }
                }
					EffectiveThrust = maxEffectiveThrust - ShipCargoMass;
  
                
                String lcdText = "";
				String lcdText2 = "";
				//int percentageBlocks = Convert.ToInt32(Math.Floor((CargoMass / maxEffectiveThrust) * 10));
                int percentageBlocks = Convert.ToInt32(Math.Floor((CargoMass / (maxEffectiveThrust - MinThrusttoMass - UnDockedMass)) * 10));
                int blocksToWrite = percentageBlocks >= 10 ? 20 : percentageBlocks <= 0 ? 0 : percentageBlocks * 2;
                int blanksToWrite = 20 - blocksToWrite;
				StatusColor = "G";
                lcdText += $"Ship Current Total Mass:\n";
				lcdText += $"{currentTotalMass.ToString("#,##0.00")}N\n";
				lcdText += $"Ship UnDocked Mass: ";
				if (UnDockedMass != 0){lcdText += $"{UnDockedMass.ToString("#,##0.00")}N\n";}
				else {lcdText += $"UnDock To Update!\n"; StatusColor = "R";}
				CargoPer = CargoMass / (maxEffectiveThrust - MinThrusttoMass - UnDockedMass)*100;
				lcdText += $"Ship CargoMass: ";
				lcdText2 += $"Ship CargoMass:\n";
				if (CargoPer >= 0 && MinThrusttoMass != 0){
				lcdText += $"[{new String('|', blocksToWrite)}{new String('.', blanksToWrite)}]{CargoPer.ToString("#,##0.00")}% \n";
				lcdText2 += $"[{new String('|', blocksToWrite)}{new String('.', blanksToWrite)}]{CargoPer.ToString("#,##0.00")}% \n";
				lcdText += $"                                : {CargoMass.ToString("#,##0.00")}N\n";}
				if (MinThrusttoMass == 0){lcdText += $"ERROR:\n -----Minimal Thrust NOT Set!-----\n";StatusColor = "R";
				lcdText += $"-----Change Minimal Thrust in Custom Data-----\n";
				lcdText2 += $"\nERROR:\n -----Minimal Thrust NOT Set!-----\n";
				lcdText2 += $" -----Change Minimal Thrust in Custom Data-----\n";}
				if (CargoPer < 0 && MinThrusttoMass !=0) {lcdText += $"ERROR:\n -----Minimal Thrust set Too High!-----\n";StatusColor = "R";
				lcdText2 += $"ERROR:\n -----Minimal Thrust set Too High!-----\n";}
				lcdText += $"Ship & Cargo Mass:: ";
                lcdText += $"{ShipCargoMass.ToString("#,##0.00")}N\n";
                lcdText += $"Ship Max Lift Thrust: ";
                lcdText += $"{maxEffectiveThrust.ToString("#,##0.00")}N\n";
                lcdText += $"Current Lift Ratio:::::: ";
                //lcdText += $"{(Minimal Thrust - ShipCargoMass) } [{new String('|', blocksToWrite)}{new String('.', blanksToWrite)}]\n";
                CurrentEffectiveThrust = maxEffectiveThrust - ShipCargoMass;
				lcdText += $"{CurrentEffectiveThrust.ToString("#,##0.00")}N\n";
				lcdText += $":::::::::::::::::::::::::::::::::\n";
				if (Onload){lcdText += $"Cargo: On-loading \n";}
				if (!Onload && CargoMass < CargoMass2){lcdText += $"Cargo: Off-loading \n";}
				if (!Onload && CargoMass2 == -1){lcdText += $"Cargo: -----Secure----- \n";}
				if (!Onload && CargoMass == CargoMass2){lcdText += $"Cargo: No Transfer Detected \n";StatusColor = "Y";}
				lcdText += $":::::::::::::::::::::::::::::::::\n";
				if (Docked && CargoMass2 == -1){
				CargoMass2 = CargoMass;}
				if (Docked && CargoMass > CargoMass2 && CargoMass2 != -1){
				Onload = true;
				}
				if (Docked && CargoMass < CargoMass2){
				Onload = false;
				}
				if (Docked && Onload){
				if (CargoPer >= SetPer){lcdText += $"Increasing Processing Power!\n";
				Runtime.UpdateFrequency = UpdateFrequency.Update1;}
            
				if (EffectiveThrust <= MinThrusttoMass){

				for(int i = 0; i < ThrustTimer.Count; i++) {
				ThrustTimer[i].ApplyAction("TriggerNow"); }
				{lcdText += $"Thrust Timer Triggered!\n";}
				Onload = false;
				CargoMass2 = -1;
				Runtime.UpdateFrequency = UpdateFrequency.Update100;
            }
			    if (EffectiveThrust <= (MinThrusttoMass / 1.10)){
				lcdText += $"Ship Too Heavy!\n";
				if (WeightTimer.Count != 0){
				for(int i = 0; i < WeightTimer.Count; i++) {
				WeightTimer[i].ApplyAction("TriggerNow"); }
				{lcdText += $"Weight Timer Triggered!\n";}
				}}
				 }
				//var LCD = GridTerminalSystem.GetBlockWithName(MassLcd) as IMyTextPanel;
                //LCD?.WriteText(lcdText);
	for(int i = 0; i < MassLCD.Count; i++) {
	MassLCD[i].ApplyAction("OnOff_On");
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER; 
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontSize = 1.0f;
	if (!ColorChoice){(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(255, 255, 255);}
	if (ColorChoice){(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(0, 0, 0);}
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).ClearImagesFromSelection();
    (MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).WriteText(lcdText, false);
	if (StatusColor == "G" && ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(0, 255, 0);}
	if (StatusColor == "Y" && ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(255, 255, 0);}
	if (StatusColor == "R" && ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(255, 0, 0);}
	if (StatusColor == "B" && ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(0, 0, 255);}
	if (StatusColor == "G" && !ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(0, 255, 0);}
	if (StatusColor == "Y" && !ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(255, 255, 0);}
	if (StatusColor == "R" && !ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(255, 0, 0);}
	if (StatusColor == "B" && !ColorChoice){
	(MassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(0, 0, 255);}
	}
	//CMassLCD//
	if (CMassLCD.Count >=1){
	for(int i = 0; i < CMassLCD.Count; i++) {
	CMassLCD[i].ApplyAction("OnOff_On");
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER; 
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontSize = 5.0f;
	if (!ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(255, 255, 255);}
	if (ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(0, 0, 0);}
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).ClearImagesFromSelection();
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).WriteText(lcdText2, false);
	if (StatusColor == "G" && ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(0, 255, 0);}
	if (StatusColor == "Y" && ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(255, 255, 0);}
	if (StatusColor == "R" && ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(255, 0, 0);}
	if (StatusColor == "B" && ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).FontColor = new Color(0, 0, 255);}
	if (StatusColor == "G" && !ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(0, 255, 0);}
	if (StatusColor == "Y" && !ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(255, 255, 0);}
	if (StatusColor == "R" && !ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(255, 0, 0);}
	if (StatusColor == "B" && !ColorChoice){
	(CMassLCD[i] as IMyTextSurfaceProvider).GetSurface(0).BackgroundColor = new Color(0, 0, 255);}
	}
				}
	IMyTextSurface mesurface0=Me.GetSurface(0);
	mesurface0.ContentType = ContentType.TEXT_AND_IMAGE;
	mesurface0.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
	mesurface0.FontSize = 0.8f;
	mesurface0.ClearImagesFromSelection();
	mesurface0.WriteText(lcdText, false);
	if (StatusColor == "Y"){
	mesurface0.BackgroundColor = new Color(100, 100, 0);}
	if (StatusColor == "B"){
	mesurface0.BackgroundColor = new Color(0, 0, 100);}
	if (StatusColor == "G"){
	mesurface0.BackgroundColor = new Color(0, 100, 0);}
	if (StatusColor == "R"){
	mesurface0.BackgroundColor = new Color(100, 0, 0);}
				Echo(lcdText);
        }
    
bool filterThis(IMyTerminalBlock block) {
  return block.CubeGrid == Me.CubeGrid;
}
Dictionary<string, string> customsettings = new Dictionary<string, string>();
void CustomConfig(IMyTerminalBlock block)
{
    customsettings.Clear();
	customsettings.Add("ProgramableBlock-Name", PBNewName.ToString());
	customsettings.Add("Minimal Thrust Timer Name Tag", ThrustTimerName.ToString());
	customsettings.Add("Over Weight Timer Name Tag", WeightTimerName.ToString());
	customsettings.Add("Minimal Thrust", MinThrusttoMass.ToString());
	customsettings.Add ("^This line must", "The Lowest Thrust to Mass for takeoff! \nYou should test this with all your inventorys full!");
	customsettings.Add("LCD Name Tag", MassLcd.ToString());
	customsettings.Add("Corner LCD Name Tag", CMassLcd.ToString());
	customsettings.Add("% Mass to Thrust to increass Processing", SetPer.ToString());
    customsettings.Add("Status Color to Text", ColorChoice.ToString());
	customsettings.Add ("^This line", "true LCD Text change color false LCD Background with Status.");
	UpdateConfig(block);
}
void UpdateConfig(IMyTerminalBlock block)
{
    string customData = block.CustomData;
    var lines = customData.Split('\n');

    foreach (var thisLine in lines)
    {
        var words = thisLine.Split('=');
        if (words.Length == 2)
        {
            var variableName = words[0].Trim();
            var variableValue = words[1].Trim();
            string dictValue;
            if (customsettings.TryGetValue(variableName, out dictValue))
            {
                customsettings[variableName] = variableValue;
            }
        }
    }

    GetVariableFromConfig("PB-Name", ref PBNewName);
	GetVariableFromConfig("Minimal Thrust Timer Name Tag", ref ThrustTimerName);
	GetVariableFromConfig("Over Weight Timer Name Tag", ref WeightTimerName);
	GetVariableFromConfig("Minimal Thrust", ref MinThrusttoMass);
	GetVariableFromConfig("LCD Name Tag", ref MassLcd);
	GetVariableFromConfig("Corner LCD Name Tag", ref CMassLcd);
	GetVariableFromConfig("% Mass to Thrust to increass Processing", ref SetPer);
    GetVariableFromConfig("Status Color to Text", ref ColorChoice);

    WriteConfig(block);
}
StringBuilder configSB = new StringBuilder();
void WriteConfig(IMyTerminalBlock block)
{
    configSB.Clear();
    foreach (var keyValue in customsettings)
    {
        configSB.AppendLine($"{keyValue.Key} = {keyValue.Value}");
    }

    block.CustomData = configSB.ToString();
}

void GetVariableFromConfig(string name, ref bool variableToUpdate)
{
    string valueStr;
    if (customsettings.TryGetValue(name, out valueStr))
    {
        bool thisValue;
        if (bool.TryParse(valueStr, out thisValue))
        {
            variableToUpdate = thisValue;
        }
    }
}

void GetVariableFromConfig(string name, ref int variableToUpdate)
{
    string valueStr;
    if (customsettings.TryGetValue(name, out valueStr))
    {
        int thisValue;
        if (int.TryParse(valueStr, out thisValue))
        {
            variableToUpdate = thisValue;
        }
    }
}

void GetVariableFromConfig(string name, ref float variableToUpdate)
{
    string valueStr;
    if (customsettings.TryGetValue(name, out valueStr))
    {
        float thisValue;
        if (float.TryParse(valueStr, out thisValue))
        {
            variableToUpdate = thisValue;
        }
    }
}

void GetVariableFromConfig(string name, ref double variableToUpdate)
{
    string valueStr;
    if (customsettings.TryGetValue(name, out valueStr))
    {
        double thisValue;
        if (double.TryParse(valueStr, out thisValue))
        {
            variableToUpdate = thisValue;
        }
    }
}

void GetVariableFromConfig(string name, ref string variableToUpdate)
{
    string valueStr;
    if (customsettings.TryGetValue(name, out valueStr))
    {
        variableToUpdate = valueStr;
    }
}