string projectorName = "Small Ship Printer Projector";
string lcdName = "Transparent LCD ProjectorInfo";
IMyProjector projector = null;
IMyTextPanel lcdPanel = null;
string displayText = "";


public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update100;
	projector = GridTerminalSystem.GetBlockWithName(projectorName) as IMyProjector;
	if (projector == null)
	{
		Echo("Projector not found.\n Please check block name and recompile.");
	}
	Echo("Projector was found");
	lcdPanel = GridTerminalSystem.GetBlockWithName(lcdName) as IMyTextPanel;
	if (lcdPanel == null)
	{
		Echo("LCD not found.\n Please check block name and recompile.");
	}
	Echo("LCD Panel was found");
	lcdPanel.ContentType = ContentType.TEXT_AND_IMAGE;
	lcdPanel.FontSize = 1.5f;
	lcdPanel.Alignment = VRage.Game.GUI.TextPanel.TextAlignment.CENTER;
}

public void Main(string argument, UpdateType updateSource)
{
	if (lcdPanel == null || projector == null)
	{
		Echo("LCD or Projector have been removed");
	}
	if (!projector.Enabled || !projector.IsProjecting)
	{
		displayText = "Projector is not projecting";
	}
	displayText = "";
	int barLength = 30;
	string barFill = "|";
	string barEmpty = ".";
	string bar = "Progress:\n[";
	if (projector.IsProjecting)
	{
		int blocksTotal = projector.TotalBlocks;
		int blocksFinished = blocksTotal - projector.RemainingBlocks;
		Echo($"Total: {blocksTotal}");
		Echo($"Finished: {blocksFinished}");
		float amount = (float)blocksFinished / (float)blocksTotal;
		float filledPercentage = amount * 100;
		int filledBars = (int)(barLength * amount);
		for (int i = filledBars; i > 0; i--)
		{
			bar += barFill;
		}

		for (int i = barLength - filledBars; i > 0; i--)
		{
			bar += barEmpty;
		}
		displayText += bar + "]" + "\n" + String.Format("{0:0.00} %", filledPercentage);
		displayText += String.Format("\n\nFinished Blocks:\n {0} of {1}", blocksFinished, blocksTotal);
	}
	Echo(displayText);
	lcdPanel.WriteText(displayText);
}