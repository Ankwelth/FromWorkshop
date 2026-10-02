/*
	Lunixia's Auto Thrust Controller (A.T.C.)
	Script will automatically turn on/off thrusters based on gravity and atmostpheric conditions.
	Ship is required to have a parachute block to detect atmospheric density. (Cockpit only detects gravity, parachute block does both).
*/

// Script Arguments
// manual
// This will stop the script from running and allow you to take full control over your thrusters and thruster groups.
// Drag the programmable block to your action bar, select "run" and use "manual" for the argument to have a manual hotkey
// Drag the programmable block to your action bar, select "run with default argument" to have an auto hotkey

// Gravity Threshold. Default is 0. Determines when the ship is considered to be "in gravity".
// If you change it, it must be a negative number. Earth gravity max strength is -9.8;
// Hydrogen Thruster will start working at values higher than this and will stop working at 
	float gravityStrengthThreshold = 0;

// Atmosphere Theshold. Default is 0.3. Determines when the ship is considered to be "in atmosphere".
// Atmostpheric/Ion Thrusters will start/stop working based on this value respectively.
	float atmosphereDensityTheshold = 0.3f;

// Output Status to LCD on this Programmable Block
	bool usePGScreen = true;

// LCDscreen Tag, Tag and LCD screen with this the name to display the status of this script.
	string lcdTag = "L:[ATC]";

//================================================================
// MAIN CODE - NO TOUCHY UNLESS YOU KNOW WHAT YOU ARE DOING.
//================================================================
List<IMyTextSurface> lcdBlockList = new List<IMyTextSurface>();
string[] prog = { "/", "-", "\\", "|" };
int c = 0;
public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}
public void LCDOutput(String strTxt)
{
	foreach(IMyTextSurface lcd in lcdBlockList)
	{
		lcd.WriteText(strTxt);
	}
}
public void PGOutput(String strTxt)
{
	if(usePGScreen){
		IMyTextSurface mesurface0 = Me.GetSurface(0);
		mesurface0.ContentType = ContentType.TEXT_AND_IMAGE;
		mesurface0.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
		mesurface0.WriteText(strTxt);
	}
}
public void GetLCDList(List<IMyTerminalBlock> blocks)
{
	lcdBlockList.Clear(); 
	foreach(IMyTerminalBlock block in blocks)
	{
		if(block is IMyTerminalBlock && block.CustomName.Contains(lcdTag))
		{			
			(block as IMyTextSurface).ContentType = ContentType.TEXT_AND_IMAGE;
			(block as IMyTextSurface).Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
			lcdBlockList.Add((block as IMyTextSurface));
		}
	}	
}
public void ToggleThrusters(List<IMyThrust> thrusters, bool setEnabled)
{
	foreach(IMyThrust thrust in thrusters)
	{
		thrust.Enabled = setEnabled;
	}
}
public void Main(string argument)
{
    StringBuilder sb = new StringBuilder();
	List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();
	List<IMyThrust> ion_thrusters = new List<IMyThrust>();
	List<IMyThrust> atmo_thrusters = new List<IMyThrust>();
	List<IMyThrust> hydro_thrusters = new List<IMyThrust>();
	List<IMyParachute> ParachuteBlocks = new List<IMyParachute>();
	GridTerminalSystem.GetBlocks(allBlocks);
	GetLCDList(allBlocks);
	sb.Append("Lunixia's Auto Thrust Controller (A.T.C.)\n");
	if(argument.ToLower() == "manual")
	{
		sb.Append("Mode: Manual\n");
		sb.Append(lcdBlockList.Count.ToString() + " Active LCDs.");
		PGOutput(sb.ToString());
		LCDOutput(sb.ToString());
		Echo(sb.ToString());
		Runtime.UpdateFrequency = UpdateFrequency.None;
		return;
	}
	else
	{
		sb.Append("Mode: Automatic\n");
		Runtime.UpdateFrequency = UpdateFrequency.Update10;
		sb.Append("================\n");
		sb.Append("Working: " + prog[c].ToString() + "\n");
		foreach(IMyTerminalBlock block in allBlocks)
		{
			if(block is IMyThrust && block.BlockDefinition.ToString().ToLower().Contains("hydrogenthrust"))
			{
				hydro_thrusters.Add((block as IMyThrust));
			}
			if(block is IMyThrust && block.BlockDefinition.ToString().ToLower().Contains("atmosphericthrust"))
			{
				atmo_thrusters.Add((block as IMyThrust));
			}
			if(block is IMyThrust && 
				!block.BlockDefinition.ToString().ToLower().Contains("atmosphericthrust") && 
				!block.BlockDefinition.ToString().ToLower().Contains("hydrogenthrust"))
			{
				ion_thrusters.Add((block as IMyThrust));
			}
			if(block is IMyParachute)
			{
				ParachuteBlocks.Add((block as IMyParachute));
			}
		}
		if(ParachuteBlocks.Count == 0)
		{
			sb.Append("Parachute Block(s) not found!\n");
			sb.Append("At least 1 Parachute block is required.\n");
			sb.Append("It does not need to have parachutes loaded.\n");
			Echo(sb.ToString());
			Runtime.UpdateFrequency = UpdateFrequency.None;
			return;
		}
		sb.Append("Grav: "+Math.Abs(Math.Round(ParachuteBlocks[0].GetNaturalGravity().Y,2)).ToString()+" m/s² ::: Threshold: " + Math.Round(gravityStrengthThreshold,2).ToString() +"  m/s²\n");
		sb.Append("Atmo: "+Math.Abs(Math.Round(ParachuteBlocks[0].Atmosphere,2)).ToString()+"% ::: Threshold: " + Math.Round(atmosphereDensityTheshold,2).ToString() + "%\n");
		if(Math.Abs(ParachuteBlocks[0].GetNaturalGravity().Y) <= gravityStrengthThreshold && ParachuteBlocks[0].Atmosphere < atmosphereDensityTheshold)
		{
			ToggleThrusters(ion_thrusters, true);
			ToggleThrusters(hydro_thrusters, false);
			ToggleThrusters(atmo_thrusters, false);
			sb.Append(atmo_thrusters.Count.ToString() + " Atmos: Off\n" + hydro_thrusters.Count.ToString() + " Hydros: Off\n" + ion_thrusters.Count.ToString() + " Ions: On\n");
		}
		if(Math.Abs(ParachuteBlocks[0].GetNaturalGravity().Y) > gravityStrengthThreshold && ParachuteBlocks[0].Atmosphere < atmosphereDensityTheshold)
		{
			ToggleThrusters(ion_thrusters, true);
			ToggleThrusters(hydro_thrusters, true);
			ToggleThrusters(atmo_thrusters, false);
			sb.Append(atmo_thrusters.Count.ToString() + " Atmos: Off\n" + hydro_thrusters.Count.ToString() + " Hydros: On\n" + ion_thrusters.Count.ToString() + " Ions: On\n");
		}
		if(Math.Abs(ParachuteBlocks[0].GetNaturalGravity().Y) > gravityStrengthThreshold && ParachuteBlocks[0].Atmosphere > atmosphereDensityTheshold)
		{
			ToggleThrusters(ion_thrusters, false);
			ToggleThrusters(hydro_thrusters, true);
			ToggleThrusters(atmo_thrusters, true);
			sb.Append(atmo_thrusters.Count.ToString() + " Atmos: On\n" + hydro_thrusters.Count.ToString() + " Hydros: On\n" + ion_thrusters.Count.ToString() + " Ions: Off\n");
		}
		c = c >= 3 ? 0 : c + 1;
		sb.Append(lcdBlockList.Count.ToString() + " Active LCDs.");
		PGOutput(sb.ToString());
		LCDOutput(sb.ToString());
		Echo(sb.ToString());
	}
}