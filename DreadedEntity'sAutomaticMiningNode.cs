/*
| ---------------------------------------------------------------- |
| DreadedEntity's Automatic Mining Node - DAMN |
| ---------------------------------------------------------------- |

i config
piston speed total = 0.0025
rotor speed = 0.5

I config
piston speed total = 0.0050
rotor speed = 0.5

X config
piston speed total = 0.05
rotor speed = 0.5000
*/

//Group and block names for script;
string pistonGroup = "PistonGroup";
string drillGroup = "DrillGroup";
string containerGroup = "Storage";
string lcdName = "LCD";
string rotorName = "Advanced Rotor";
string programmableBlockName = "Programmable block - DAMN";

//Script settings - Do not change
const float TotalPistonVelocity = 0.05F;
const int MaxPercentage = 90;
const int MinPercentage = 80;
const float BrakingTorque = 100000F;
const float RotorVelocity = 0.50F;

public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	((IMyMotorStator)GridTerminalSystem.GetBlockWithName(rotorName)).BrakingTorque = BrakingTorque;
	var lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdName);
	Echo(lcd.ContentType.ToString());
	lcd.ContentType = ContentType.TEXT_AND_IMAGE;
	lcd.FontSize = 10;
	lcd.Font = "Monospace";
	
	var pblcd = (IMyTextSurface)((IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName(programmableBlockName)).GetSurface(0);
	pblcd.ContentType = ContentType.TEXT_AND_IMAGE;
	pblcd.FontSize = 1;
	pblcd.Font = "Monospace";
}

public List<IMyTerminalBlock> GetGroupListByName(string groupName) {
	IMyBlockGroup blockGroup = GridTerminalSystem.GetBlockGroupWithName(groupName);
	List<IMyTerminalBlock> blockGroupList = new List<IMyTerminalBlock>();
	blockGroup.GetBlocks(blockGroupList);
	return blockGroupList;
}
public string GetStorageFullnessText(double percentageFull) {
	StringBuilder lines = new StringBuilder();
	int fullLines = (int)((percentageFull * 10) / 100);
	for (int i = 0; i < fullLines; i++)
		lines.Append("|");
	for (int i = 0; i < 10 - fullLines; i++)
		lines.Append("-");
	return lines.ToString();
}
public void ApplyActionToBlockList(List<IMyTerminalBlock> list, string action) {
		foreach (IMyTerminalBlock block in list)
			block.ApplyAction(action);
}

public double CalculatePercentageFull(string groupName) {
	double TotalCurrentVolume = 0;
	double TotalMaxVolume = 0;
	List<IMyTerminalBlock> containers = GetGroupListByName(groupName);
	foreach (IMyTerminalBlock container in containers) {
		IMyInventory inventory = container.GetInventory(0);
		TotalCurrentVolume += (double)inventory.CurrentVolume;
		TotalMaxVolume += (double)inventory.MaxVolume;
	}
	return (TotalCurrentVolume / TotalMaxVolume) * 100;
}

public void Main(string argument, UpdateType updateSource)
{
	var builder = new StringBuilder();
	List<IMyTerminalBlock> pistons = GetGroupListByName(pistonGroup);
	float pistonSpeed = TotalPistonVelocity / pistons.Count;
	
	builder.Append($"PistonCount: {pistons.Count}\r\n");
	
	if (argument == "") {
		argument = Storage;
	}
	switch (argument) {
		case "i": {
			pistonSpeed /= 4;
			builder.Append($"Configuration: i\r\n");
			break;
		}
		case "I": { 
			pistonSpeed /= 2;
			builder.Append($"Configuration: I\r\n");
			break;
		}
		case "x": {
			builder.Append($"Configuration: x\r\n");
			break;
		}
	}
	Storage = argument;
	
	builder.Append($"PistonSpeed: {pistonSpeed}\r\n");
	
	double percentageFull = CalculatePercentageFull(containerGroup);
	
	List<IMyTerminalBlock> drills = GetGroupListByName(drillGroup);
	
	float pistonTotalLength  = 0;
	foreach (IMyExtendedPistonBase piston in pistons) {
		pistonTotalLength += piston.CurrentPosition;
	}
	float pistonMaxLength  = 0;
	foreach (IMyExtendedPistonBase piston in pistons) {
		pistonMaxLength += piston.HighestPosition;
	}
	
	var rotor = (IMyMotorStator)GridTerminalSystem.GetBlockWithName(rotorName);
	
	if (pistonTotalLength == pistonMaxLength) {
		foreach (IMyExtendedPistonBase piston in pistons) {
			piston.Velocity = -1 /  (float)pistons.Count;
		}
		ApplyActionToBlockList(drills, "OnOff_Off");
		rotor.TargetVelocityRPM = 0;
		var pb = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName(programmableBlockName);
		pb.Enabled = false;
	} else {
		if (percentageFull >= MaxPercentage) {
			ApplyActionToBlockList(pistons, "OnOff_Off");
			ApplyActionToBlockList(drills, "OnOff_Off");
			rotor.TargetVelocityRPM = 0;
		} else if (percentageFull <= MinPercentage) {
			ApplyActionToBlockList(pistons, "OnOff_On");
			ApplyActionToBlockList(drills, "OnOff_On");
			rotor.TargetVelocityRPM = RotorVelocity;
			foreach (IMyExtendedPistonBase piston in pistons) {
				piston.Velocity = pistonSpeed;
			}
		}
	}
	
	builder.Append($"Containers %: {percentageFull:0.00}%\r\n");
	//Echo($"Containers Percentage: {percentageFull:0.00}%");
	
	builder.Append($"Piston Length: {pistonTotalLength:0.00}\r\n");
	builder.Append($"Piston Max: {pistonMaxLength}");
	
	Echo(builder.ToString());

	IMyTextSurface lcd = (IMyTextSurface)GridTerminalSystem.GetBlockWithName(lcdName);
	lcd.WriteText(GetStorageFullnessText(percentageFull));
	
	var pblcd = (IMyTextSurface)((IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName(programmableBlockName)).GetSurface(0);
	pblcd.WriteText(builder.ToString());
	
	//TODO: Once all Pistons are fully extended, Show Rotor on HUD
	//TODO: Add output to the PB
}