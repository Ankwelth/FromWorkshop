/*
	INTENDED FOR USE WITH
	Spug's Easy Auto-Docking 2
	https://steamcommunity.com/sharedfiles/filedetails/?id=2146371052

	INSTRUCTIONS
		add !ad2 to the auto dock 2 programmable block
		add !adm: to blocks where there's only one screen. (Like text panels / LCD blocks etc)
		add !adm:X to blocks with multi-LCD's (e.g. cockpits) where X is the screen number you want, in order of the list in the terminal.
		setup G hotkeys using programmable blocks run command and fill out as follows:
		previous connector
		next connector
		previous grid
		next grid
		previous dock
		next dock
		dock
		run
		save

		NOTE
		When using "save" to save a dock, it will avoid doing overwrite if such dock already exsists under the same name.
		However, using "run" will dock or overwrite saved dock.

		Be mindful of choosing different connectors on ship because Spug didn't implement the proper rotations with different connectors.
		Hopefully he'll add it.

		Waypoint support coming soon.

	Big thanks to Spug for helping me interface with it and doing the necessary additions to his script.
*/

string LcdTag = "!adm:";
string AutoDockPbTag = "!ad2";

bool OverwriteProtection = true;
bool SwitchAndSelect = false;

string AD_Data = "";

bool InitDone = false;
int TimeOut = 10;

int selectedMenuIndex = 0;
int selectedConnectorIndex = 0;
int selectedBaseIndex = 0;
int selectedDockIndex = 0;
Dictionary<int, int> LastDockIndex = new Dictionary<int, int>();

Dictionary<string, List<string>> BaseDocks = new Dictionary<string, List<string>>();
Dictionary<IMyShipConnector, string> ShipConnectors = new Dictionary<IMyShipConnector, string>();
Dictionary<IMyShipConnector, long> ShipConnectorIDs = new Dictionary<IMyShipConnector, long>();

List<IMyTerminalBlock> Blocks = new List<IMyTerminalBlock>();
List<IMyTextSurfaceProvider> LCDs = new List<IMyTextSurfaceProvider>();
//IMyTextPanel
//IMyTextSurface

public Program()
{
	Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
	Me.GetSurface(0).WriteText("Initializing...");
	WriteToLCD("Initializing...");

	Init();
}

void Init() {
	//GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(LCDs, b => b.IsSameConstructAs(Me) && b.CustomName.Contains(LcdTag));

	List<IMyShipConnector> TempConnectors = new List<IMyShipConnector>();
	GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(TempConnectors, b => b.IsSameConstructAs(Me));

	if (TempConnectors.Any()) {
		foreach (var Connector in TempConnectors) {
			ShipConnectors[Connector] = Connector.CustomName;
			ShipConnectorIDs[Connector] = Connector.EntityId;
		}
	}

	TempConnectors.Clear();

	AD_Data = Get_AD_Data(AutoDockPbTag, "[data_output_request]");

	if (string.IsNullOrEmpty(AD_Data))
	{
		Runtime.UpdateFrequency=UpdateFrequency.Update10;
	}
	else
	{
		Load(AD_Data);
		ShowInitMenu();
		InitDone = true;
	}
}

public void Main(string argument, UpdateType updateSource)
{
	if (!InitDone)
	{
		TimeOut--;

		AD_Data = Get_AD_Data(AutoDockPbTag, "[data_output_request]");

		if (!string.IsNullOrEmpty(AD_Data) || TimeOut <= 0)
		{
			Load(AD_Data);
			ShowInitMenu();
			InitDone = true;
			Runtime.UpdateFrequency=UpdateFrequency.None;
		}

		return;
	}

	if (argument == "save")
	{
		Echo("Saving...");

		selectedMenuIndex = 2;

		string GridName = "";
		string ConnectorName = "";

		List<IMyShipConnector> Connectors = new List<IMyShipConnector>();
		GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(Connectors, b => b.IsSameConstructAs(Me));
		if (!Connectors.Any()) {
			Echo("ERROR\nNo connectors found on this grid.");
			return;
		}

		foreach (var block in Connectors)
		{
			if (block.Status == MyShipConnectorStatus.Connected || block.Status == MyShipConnectorStatus.Connectable)
			{
				Echo("Connection detected.\n" + block.CustomName);
				GridName = block.OtherConnector.CubeGrid.CustomName;
				ConnectorName = block.OtherConnector.CustomName;
				break;
			}
		}

		if (string.IsNullOrEmpty(GridName) || string.IsNullOrEmpty(ConnectorName)) {
			Echo("\nNo possible connections detected.");
			return;
		}

		if (!BaseDocks.ContainsKey(GridName))
		{
			BaseDocks[GridName] = new List<string>();
			BaseDocks[GridName].Add(ConnectorName);
			selectedBaseIndex = BaseDocks.Count() - 1;
			selectedDockIndex = 0;
		}
		else
		{
			if (!BaseDocks[GridName].Contains(ConnectorName))
			{
				BaseDocks[GridName].Add(ConnectorName);
				selectedDockIndex = BaseDocks[GridName].Count() - 1;
			}
			else
			{
				if (OverwriteProtection)
				{
					Echo("Attempted overwrite\n" + GridName + "\n" + ConnectorName);
					return;
				}
			}
		}

		if (!RunPB(AutoDockPbTag, GridName + " " + ConnectorName)) {
			Echo("Failed at saving.\n" + GridName + "\n" + ConnectorName);
			return;
		}

		Echo("SAVED\n" + GridName + "\n" + ConnectorName);
		MenuShow();
	}
	else if (argument == "up")
	{
		selectedMenuIndex = selectedMenuIndex-- <= 0 ? 2 : selectedMenuIndex--;
		MenuShow();
	}
	else if (argument == "down")
	{
		selectedMenuIndex = selectedMenuIndex++ >= 2 ? 0 : selectedMenuIndex++;
		MenuShow();
	}
	else if (argument == "left")
	{
		if (BaseDocks.Count() == 0)
			return;
		if (selectedMenuIndex == 1)
		{
			LastDockIndex[selectedBaseIndex] = selectedDockIndex;
			selectedBaseIndex = selectedBaseIndex-- < 1 ? (BaseDocks.Count() - 1) : selectedBaseIndex--;
			if (LastDockIndex.ContainsKey(selectedBaseIndex))
				selectedDockIndex = LastDockIndex[selectedBaseIndex];
			else
				selectedDockIndex = 0;
			MenuShow();
		}
		else if (selectedMenuIndex == 2)
		{
			string Grid = BaseDocks.ElementAt(selectedBaseIndex).Key;
			selectedDockIndex = selectedDockIndex-- < 1 ? (BaseDocks[Grid].Count() - 1) : selectedDockIndex--;
			MenuShow();
		}
	}
	else if (argument == "right")
	{
		if (BaseDocks.Count() == 0)
			return;
		if (selectedMenuIndex == 1)
		{
			LastDockIndex[selectedBaseIndex] = selectedDockIndex;
			selectedBaseIndex = selectedBaseIndex++ > (BaseDocks.Count() - 2) ? 0 : selectedBaseIndex++;
			if (LastDockIndex.ContainsKey(selectedBaseIndex))
				selectedDockIndex = LastDockIndex[selectedBaseIndex];
			else
				selectedDockIndex = 0;
			MenuShow();
		}
		else if (selectedMenuIndex == 2)
		{
			string Grid = BaseDocks.ElementAt(selectedBaseIndex).Key;
			selectedDockIndex = selectedDockIndex++ > (BaseDocks[Grid].Count() - 2) ? 0 : selectedDockIndex++;
			MenuShow();
		}
	}
	else if (argument == "run" || argument == "go")
	{
		if (BaseDocks.Count == 0)
			return;
		string Grid = BaseDocks.ElementAt(selectedBaseIndex).Key;
		string Connector = BaseDocks[Grid][selectedDockIndex];
		Echo("Running\n" + Grid + "\n" + Connector);
		RunPB(AutoDockPbTag, Grid + " " + Connector);
		Me.CustomData = Grid + " " + Connector;
	}
	else if (argument == "dock")
	{
		if (BaseDocks.Count == 0)
			return;
		string Grid = BaseDocks.ElementAt(selectedBaseIndex).Key;
		string Connector = BaseDocks[Grid][selectedDockIndex];
		Echo("Running\n" + Grid + "\n" + Connector);
		RunPB(AutoDockPbTag, Grid + " " + Connector + " !" + ShipConnectorIDs[ShipConnectors.ElementAt(selectedConnectorIndex).Key]);
		Me.CustomData = Grid + " " + Connector;
	}
	else if (argument == "next connector" || argument == "nc")
	{
		if (!SwitchAndSelect) {
			if (selectedMenuIndex != 0) {
				selectedMenuIndex = 0;
				MenuShow();
				return;
			}
		}

		if (ShipConnectors.Count() == 0 || BaseDocks.Count() == 0)
			return;
		selectedMenuIndex = 0;
		selectedConnectorIndex = selectedConnectorIndex++ > (ShipConnectors.Count() - 2) ? 0 : selectedConnectorIndex++;
		MenuShow();
	}
	else if (argument == "previous connector" || argument == "pc")
	{
		if (!SwitchAndSelect) {
			if (selectedMenuIndex != 0) {
				selectedMenuIndex = 0;
				MenuShow();
				return;
			}
		}

		if (ShipConnectors.Count() == 0 || BaseDocks.Count() == 0)
			return;
		selectedMenuIndex = 0;
		selectedConnectorIndex = selectedConnectorIndex-- < 1 ? (ShipConnectors.Count() - 1) : selectedConnectorIndex--;
		MenuShow();
	}
	else if (argument == "next grid" || argument == "ng")
	{
		if (!SwitchAndSelect) {
			if (selectedMenuIndex != 1) {
				selectedMenuIndex = 1;
				MenuShow();
				return;
			}
		}

		if (BaseDocks.Count() == 0)
			return;
		selectedMenuIndex = 1;
		LastDockIndex[selectedBaseIndex] = selectedDockIndex;
		selectedBaseIndex = selectedBaseIndex++ > (BaseDocks.Count() - 2) ? 0 : selectedBaseIndex++;
		if (LastDockIndex.ContainsKey(selectedBaseIndex))
			selectedDockIndex = LastDockIndex[selectedBaseIndex];
		else
			selectedDockIndex = 0;
		MenuShow();
	}
	else if (argument == "previous grid" || argument == "pg")
	{
		if (!SwitchAndSelect) {
			if (selectedMenuIndex != 1) {
				selectedMenuIndex = 1;
				MenuShow();
				return;
			}
		}

		if (BaseDocks.Count() == 0)
			return;
		selectedMenuIndex = 1;
		LastDockIndex[selectedBaseIndex] = selectedDockIndex;
		selectedBaseIndex = selectedBaseIndex-- < 1 ? (BaseDocks.Count() - 1) : selectedBaseIndex--;
		if (LastDockIndex.ContainsKey(selectedBaseIndex))
			selectedDockIndex = LastDockIndex[selectedBaseIndex];
		else
			selectedDockIndex = 0;
		MenuShow();
	}
	else if (argument == "next dock" || argument == "nd")
	{
		if (!SwitchAndSelect) {
			if (selectedMenuIndex != 2) {
				selectedMenuIndex = 2;
				MenuShow();
				return;
			}
		}

		if (BaseDocks.Count() == 0)
			return;
		selectedMenuIndex = 2;
		string Grid = BaseDocks.ElementAt(selectedBaseIndex).Key;
		selectedDockIndex = selectedDockIndex++ > (BaseDocks[Grid].Count() - 2) ? 0 : selectedDockIndex++;
		MenuShow();
	}
	else if (argument == "previous dock" || argument == "pd")
	{
		if (!SwitchAndSelect) {
			if (selectedMenuIndex != 2) {
				selectedMenuIndex = 2;
				MenuShow();
				return;
			}
		}

		if (BaseDocks.Count() == 0)
			return;
		selectedMenuIndex = 2;
		string Grid = BaseDocks.ElementAt(selectedBaseIndex).Key;
		selectedDockIndex = selectedDockIndex-- < 1 ? (BaseDocks[Grid].Count() - 1) : selectedDockIndex--;
		MenuShow();
	}
	else if (argument == "reset") {
		Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
		Me.GetSurface(0).WriteText("Initializing...");
		WriteToLCD("Initializing...");

		BaseDocks.Clear();
		ShipConnectors.Clear();
		ShipConnectorIDs.Clear();

		InitDone = false;
		Init();
	}
}

void ShowInitMenu()
{
	if (BaseDocks.Count == 0)
	{
		Me.GetSurface(0).WriteText("No connectors saved.");
		WriteToLCD("No connectors saved.");
	}
	else
		MenuShow();
}

// @Menu @Show
void MenuShow()
{
	if (BaseDocks.Count == 0)
	{
		Me.GetSurface(0).WriteText("ERROR\nNo docks found.");
		return;
	}

	string ShipConnector = ShipConnectors.ElementAt(selectedConnectorIndex).Value;
	string Grid = BaseDocks.ElementAt(selectedBaseIndex).Key;
	List<string> ConnectorList = new List<string>(BaseDocks[Grid]);
	string Connector = ConnectorList[selectedDockIndex];

	string MenuSourceData =
		ShipConnector +
		"\n" +
		Grid +
		"\n" +
		Connector
	;

	string[] menuItems = MenuSourceData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

	string MenuData = "";

	for (int i = 0; i < menuItems.Length; i++)
	{
		string TextData = "";
		if (i == selectedMenuIndex)
		{
			TextData += "\n" + ">" + menuItems[i] + "<" + "\n";
		}
		else
		{
			TextData += "\n" + "  " + menuItems[i] + "" + "\n";
		}

		if (i == 0)
		{
			MenuData += (selectedConnectorIndex + 1).ToString() + "/" + ShipConnectors.Count().ToString() + TextData;
		}
		else if (i == 1)
		{
			MenuData += (selectedBaseIndex + 1).ToString() + "/" + BaseDocks.Count().ToString() + TextData;
		}
		else if (i == 2)
		{
			MenuData += (selectedDockIndex + 1).ToString() + "/" + BaseDocks[Grid].Count().ToString() + TextData;
		}
	}

	Me.GetSurface(0).WriteText(MenuData);

	WriteToLCD(MenuData);
}

void WriteToLCD(string Text)
{
	GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(Blocks, b => b.IsSameConstructAs(Me) && b is IMyTextSurfaceProvider && b.CustomName.Contains(LcdTag));

	if (Blocks.Any())
	{
		foreach (var block in Blocks)
		{
			/*
			if (lcd == null || !lcd.IsFunctional || !lcd.IsWorking) {
				GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(LCDs, b => b.IsSameConstructAs(Me) && b.CustomName.Contains(LcdTag));
				continue;
			}
			else {
				GridTerminalSystem.GetBlocksOfType<IMyTextSurfaceProvider>(LCDs, b => b.IsSameConstructAs(Me) && b.CustomName.Contains(LcdTag));
			}
			*/

			string[] LcdNameLines = block.CustomName.Split(' ', '\r', '\n');
			bool HasScreen = false;
			string Line = "";
			int Screen = 1;
			foreach (var LcdLine in LcdNameLines)
			{
				if (LcdLine.Contains(LcdTag))
				{
					if (LcdLine.Length > LcdTag.Length) {
						Line = LcdLine.Replace(LcdTag, String.Empty);
						HasScreen = int.TryParse(Line, out Screen);
					}
					else
						HasScreen = true;
					
					break;
				}
			}

			if (!HasScreen)
			{
				continue;
			}

			var lcd = block as IMyTextSurfaceProvider;
			if (lcd == null)
				continue;

			if (lcd.SurfaceCount >= Screen)
			{
				lcd.GetSurface(Screen - 1).ContentType = ContentType.TEXT_AND_IMAGE;
				lcd.GetSurface(Screen - 1).WriteText(Text);
			}
		}
	}

	/*
	List<IMyCockpit> Cockpits = new List<IMyCockpit>();
	GridTerminalSystem.GetBlocksOfType<IMyCockpit>(Cockpits, b => b.IsSameConstructAs(Me) && b.CustomName.ToLower().Contains(LcdTag));
	
	if (Cockpits.Any())
	{
		foreach (IMyCockpit Cockpit in Cockpits)
		{
			string[] LCDs = Cockpit.CustomName.Split(' ', '\r', '\n');
			bool HasScreen = false;
			string Line = "";
			int Screen = 0;
			foreach (var LCD in LCDs)
			{
				if (LCD.Contains(LcdTag))
				{
					Line = LCD.Replace(LcdTag, String.Empty);
					HasScreen = int.TryParse(Line, out Screen);
					break;
				}
			}

			if (!HasScreen || Screen == 0)
			{
				return;
			}

			if (Cockpit.SurfaceCount >= Screen)
			{
				Cockpit.GetSurface(Screen - 1).ContentType = ContentType.TEXT_AND_IMAGE;
				Cockpit.GetSurface(Screen - 1).WriteText(Text);
			}
		}
	}
	*/
}

bool RunPB(string target, string argument)
{
	List<IMyProgrammableBlock> ProgrammableBlocks = new List<IMyProgrammableBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(ProgrammableBlocks, b => b.IsSameConstructAs(Me) && b.CustomName.Contains(target));

	if (!ProgrammableBlocks.Any())
	{
		Echo("No PB's found!\n" + target);
		return false;
	}

	Echo(argument);

	ProgrammableBlocks[0].TryRun(argument);
	//ProgrammableBlocks[0].CustomData = argument;

	return true;
}

string Get_AD_Data(string target, string argument)
{
	List<IMyProgrammableBlock> ProgrammableBlocks = new List<IMyProgrammableBlock>();
	GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(ProgrammableBlocks, b => b.IsSameConstructAs(Me) && b.CustomName.Contains(target));

	if (!ProgrammableBlocks.Any())
	{
		Echo("No PB's found!\n" + target);
		return "";
	}

	Echo(argument);

	ProgrammableBlocks[0].TryRun(argument);

	var Result = ProgrammableBlocks[0].CustomData;
	ProgrammableBlocks[0].CustomData = "";

	return Result;
}

void LoadFromCustomData()
{
	bool WriteGrid = false;
	bool WriteConnector = false;
	string GridName = "";
	string ConnectorName = "";
	string Line = "";

	if (!string.IsNullOrEmpty(Me.CustomData))
	{
		selectedMenuIndex = 1;
		string[] Collection = Me.CustomData.Split('\r', '\n');
		for (int i = 0; i < Collection.Length; i++)
		{
			Line = Collection[i].Trim();
			if (string.IsNullOrEmpty(Line))
				continue;

			if (Line != "!Collection;")
			{
				if (WriteConnector)
				{
					ConnectorName = Line;
					if (!BaseDocks.ContainsKey(GridName))
					{
						BaseDocks[GridName] = new List<string>();
						BaseDocks[GridName].Add(ConnectorName);
					}
					else
					{
						if (!BaseDocks[GridName].Contains(ConnectorName))
						{
							BaseDocks[GridName].Add(ConnectorName);
							selectedDockIndex = BaseDocks[GridName].Count() - 1;
						}
					}
				}

				if (WriteGrid)
				{
					GridName = Line;
					WriteGrid = false;
					WriteConnector = true;
				}
			}

			if (Line == "!Collection;" && WriteGrid == false)
			{
				WriteGrid = true;
				WriteConnector = false;
			}

			Echo("Data Loaded.");
		}
	}
}

public void Load(string Data)
{
	if (string.IsNullOrEmpty(Data))
		return;
	// Main Base 01;Main Base 01 Connector 8;Scout Connector 5;base 01 connector 01;0
	// Main Base 01;Main Base 01 Connector 8;Scout Connector 5;base 01 connector 01 waypoint 01;2
	string[] Docks = Data.Split( '\r','\n' );

	foreach (var Dock in Docks)
	{
		string[] DockData = Dock.Split(';');

		string GridName = DockData[0];
		string ConnectorName = DockData[1];

		if (BaseDocks.ContainsKey(GridName))
		{
			BaseDocks[GridName].Add(ConnectorName);
		}
		else
		{
			BaseDocks[GridName] = new List<string>();
			BaseDocks[GridName].Add(ConnectorName);
		}
	}
}

public void Save()
{

}