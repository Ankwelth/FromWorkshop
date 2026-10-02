// Configuration Options

	/*
	YOU NEED TO SET THIS!
	This should match up with the MASTER_KEY defined in your defense drone PBs
	*/
	readonly static string MASTER_KEY = "";

	// THIS IS OBSOLETE. 
	// You can now use the "FETCH" command to retrieve the names of all drones in range and populate your LCD menu
	// Leaving this here in case folks still want to hard code their drone names
	/*
	The names of all ships you want to be able to send individual messages to. 
	These should line up with the name you see under ship info.
	You can specify as many ships as you'd like in the same format.
	*/
	readonly static string[] DRONE_NAMES = {};

	// The name of the menu LCD for this script to use. Default is DRONE_COMMAND_LCD
	readonly static string LCD_NAME = "DRONE_COMMAND_LCD";

	
	// The name of the status LCD for this script to use. Default is DRONE_STATUS_LCD
	readonly static string STATUS_LCD_NAME = "DRONE_STATUS_LCD";

/* DONT CHANGE ANYTHING BELOW THIS LINE! **********************************************/
	Transceiver Com;
	
	class Transceiver {
		readonly string IGC_VISION = $"N0zeasDCScript-{MASTER_KEY}";
		readonly string IGC_STATUS = $"N0zeasDCScript-{MASTER_KEY}-StatusUpdates";

		readonly IMyIntergridCommunicationSystem IGC;
		private readonly IMyBroadcastListener _listener;
		private MyGridProgram GridProgram;
		private string[] ships = {
			"~~ALL~~"
		};
		private int currentIndex = 0;
		private string currentStatuses = "";

		public Transceiver(IMyIntergridCommunicationSystem igc, MyGridProgram program) {
			IGC = igc;
			GridProgram = program;
			_listener = IGC.RegisterBroadcastListener(IGC_STATUS);
		}

		public void Broadcast(string argument, bool fetch = false) {
			if(fetch) ships = new string[] { "~~ALL~~" };	
			IGC.SendBroadcastMessage(IGC_VISION, MASTER_KEY + "NAME:" + ships[currentIndex] + ";" + argument, TransmissionDistance.TransmissionDistanceMax);
		}

		public void Receive() {
			IMyTextPanel LCD = GridProgram.GridTerminalSystem.GetBlockWithName(STATUS_LCD_NAME) as IMyTextPanel;
			currentStatuses = LCD?.GetText() ?? "";
			while(_listener.HasPendingMessage) {
				MyIGCMessage message = _listener.AcceptMessage();
				string stringData = message.Data as string;
				string[] parsedMessage = stringData.Split(';');
				switch(parsedMessage[0]) {
					case "FETCH":
					    if(ships.Contains(parsedMessage[0])) break;
						currentIndex = 0;
						ships = ships.Append(parsedMessage[1]).OrderBy(s => s).ToArray();
						PrintNames();
						break;
					case "STATUS":
						PrintStatuses(parsedMessage[1], parsedMessage[2], parsedMessage[3]);
						break;
					default:
						GridProgram.Echo("INVALID MESSAGE TYPE " + parsedMessage[0] + ". EXITING...");
						break;
				}
			}
			LCD?.WriteText(currentStatuses);
		}

		public void SetShipToCommand(string argument) {
			if(argument == "UP") {
				if(currentIndex == 0) currentIndex = ships.Length - 1;
				else currentIndex--;
			}
			else {
				if(currentIndex == ships.Length - 1) currentIndex = 0;
				else currentIndex++;
			}
			PrintNames();
		}

		private void PrintNames() {
			ships = ships.Union(DRONE_NAMES).ToArray();
			IMyTextPanel LCD = GridProgram.GridTerminalSystem.GetBlockWithName(LCD_NAME) as IMyTextPanel;
			string selectedShips = "";
			int i = 0;
			foreach(string ship in ships) {
				selectedShips += currentIndex == i ? ">>" + ship + "<<\n" : ship + "\n";
				i++;
			}
			LCD.WriteText(selectedShips);
		}

		private void PrintStatuses(string status, string behavior, string name) {
			string newStatus = "--" + name + "-- \n" + "STATUS: \n" + status + "BEHAVIOR: " + behavior + "--ENDSTATUS--\n\n";
			string currentText = currentStatuses;
			int statusIndex = currentText.IndexOf("--" + name + "--");
			if(statusIndex >= 0) {
				string oldStatus = currentText.Substring(statusIndex);
				int endStatusIndex = oldStatus.IndexOf("--ENDSTATUS--") + 15;
				oldStatus = oldStatus.Substring(0, endStatusIndex);
				GridProgram.Echo("Old Status for " + name + ": " + oldStatus);
				currentText = currentText.Replace(oldStatus, newStatus);
			} else {
				GridProgram.Echo("New Status for " + name + ": " + newStatus);
				currentText += newStatus;
			}
			currentStatuses = currentText;
		}
	}

	private string PopulateSOS(string argument) {
		Vector3D pos = Me.GetPosition();
		argument += $";{pos.X};{pos.Y};{pos.Z}";
		return argument;
	}

	public Program() {
		Runtime.UpdateFrequency = UpdateFrequency.Update100;
		Com = new Transceiver(IGC, this);
	}

	public void Main(string argument, UpdateType updateSource) {
		if(MASTER_KEY == "") {
			Echo("Master key not set! Please set the MASTER_KEY at the top of this script. Exiting...");
			return;
		}
		if(string.IsNullOrEmpty(argument)) { 
			Com.Receive();
			return;
		}
		if(argument.Equals("SOS")) argument = PopulateSOS(argument);
		if(argument.Equals("UP") || argument.Equals("DOWN")) Com.SetShipToCommand(argument);
		if(argument.Equals("FETCH")) {
			Com.Broadcast(argument, true);
		}
		else Com.Broadcast(argument);
	}