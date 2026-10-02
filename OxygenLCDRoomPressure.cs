public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

void Main()
{
	IMyTextPanel output;
	IMyAirVent vent;
	float currentLevel;
	string outputText;

	output = GridTerminalSystem.GetBlockWithName("Oxygen LCD") as IMyTextPanel; //Change "Oxygen LCD" to the name of your LCD
	
	vent = GridTerminalSystem.GetBlockWithName("Air Vent Monitor") as IMyAirVent; //Change "Air Vent Monitor" to the name of your Air Vent
	
	if (output == null) throw new Exception("LCD Panel block not found, check name");
	if (vent == null) throw new Exception("Air Vent block not found, check name");
	
	currentLevel = vent.GetOxygenLevel();
	outputText = vent.CustomName + "'s oxygen level: " + String.Format("{0:P2}", currentLevel);
	output.ContentType = ContentType.TEXT_AND_IMAGE;
	output.WriteText(outputText);
}