// Configuration Options
	/* 
	YOU NEED TO SET THIS!
	If you are using the Drone Master Control script, make sure this value matches up with the MASTER_KEY in the MC script.
	*/
	readonly static string MASTER_KEY = "";

	// By default, the drone will broadcast at MIN_ANTENNA_RANGE
	// The drone will broadcast at MAX_ANTENNA_RANGE whenever it detects an enemy OR if the PING command is issued via the MC script
	readonly static int MAX_ANTENNA_RANGE = 50000;
	readonly static int MIN_ANTENNA_RANGE = 3000;

	// This script supports integration with Izy's Solar Alignment script in ship mode (https://steamcommunity.com/sharedfiles/filedetails/?id=699142028&searchtext=solar)
	// If utilizing the ISA script in ship mode, set this to true
	public static bool allowSolarMode = false;

	// If operating in atmosphere, setting this to true will cut thrusters and deploy parachutes when power is low
	public static bool allowParachuting = false;

	// If allowSolarMode is set to true, add this tag to the name of your Izy's Solar Alignment script PB 
	// Also be sure to set "useGyroMode" to true in your ISA script
	readonly static string SOLAR_TAG = "[Solar]";

	// If set to true, this script will automatically take over the drone's behavior immediately once compiled
	// If set to false, drone will wait for "START" command to be passed in before taking over the drone
	// In either case, the "STOP" command can be sent to release control of the drone
	public static bool autostart = false;

	// LCD for displaying your drone's logs. Useful for debugging.
	readonly static string LOGGER = "LOG_LCD";

	// Controls how verbose the logs in your logger will be. Comment out any log types you want to ignore.
	public static LogLevel[] logLevels = {
		//LogLevel.Debug,
		LogLevel.Info,
		LogLevel.Error
	};


/* DONT CHANGE ANYTHING BELOW THIS LINE! **********************************************/
	DetectorCollection Detectors;
	EntityDatabase Entities;
	Transceiver Com;
	MeInfo Info;
	MeThrusters Thrusters;
	MeSolarScript SolarScript;
	MeDistress Distress;

	readonly static string ENEMY_FLAG = "~DETECTED~";

	public static IMyIntergridCommunicationSystem MyIGC;
	public static MyGridProgram MyProgram;
	public static IMyGridTerminalSystem Grid;
	public static IMyRadioAntenna antenna;
	public static IMyFlightMovementBlock aiFlightBlock;
	public static IMyRemoteControl rcBlock;
	public static IMyOffensiveCombatBlock combatBlock;
	public static IMyDefensiveCombatBlock defenseBlock;
	public static IMyBasicMissionBlock taskBlock;
	public static IMyBeacon beacon;
	public static IMyProgrammableBlock solarBlock;
	public static IMyTextSurface logger;


	public static List<IMyBatteryBlock> batteries;
	public static List<IMyParachute> parachutes;
	public static List<IMyGasTank> tanks;
	public static List<IMyThrust> thrusters;
	public static List<IMyFunctionalBlock> turrets;

	public enum LogLevel {
		Debug = 1,
		Info = 2,
		Error = 3
	}

	public static string droneStatus = "";
	public static string currentBehavior = "";
	public static string currentKey = "1594583033";

	private static void InitializeAIBlock(IMyFunctionalBlock aiBlock) {
		if(aiBlock == null) return;
		aiBlock.Enabled = true;
		aiBlock?.ApplyAction("ActivateBehavior_On");
	}

	public static void LogMessage(string message, LogLevel level, bool appendToStatus = false) {
		if (logLevels.Contains(level)) {
			MyProgram.Echo(message);
			logger?.WriteText(message + "\n", true);
		}

		if (appendToStatus) droneStatus += message + "\n";
	}

	private static T GetBlockOnGrid<T>(MyGridProgram inProgram) where T: class, IMyTerminalBlock {
		return GetBlocksOnGrid<T>(inProgram).FirstOrDefault();
	}
	
	private static List<T> GetBlocksOnGrid<T>(MyGridProgram inProgram) where T: class, IMyTerminalBlock {
		List<T> blocks = new List<T>();
		inProgram.GridTerminalSystem.GetBlocksOfType(blocks);
		return blocks.Where(b => b.IsSameConstructAs(inProgram.Me)).ToList();
	}

	public static void Initialize(MyGridProgram inProgram) {
		MyProgram = inProgram;
		Grid = inProgram.GridTerminalSystem;
		MyIGC = inProgram.IGC;

		List<IMyRadioAntenna> antennae = new List<IMyRadioAntenna>();
		List<IMyFlightMovementBlock> flightBlocks = new List<IMyFlightMovementBlock>();
		List<IMyRemoteControl> rcBlocks = new List<IMyRemoteControl>();
		List<IMyOffensiveCombatBlock> combatBlocks = new List<IMyOffensiveCombatBlock>();
		List<IMyDefensiveCombatBlock> defenseBlocks = new List<IMyDefensiveCombatBlock>();
		List<IMyBasicMissionBlock> taskBlocks = new List<IMyBasicMissionBlock>();
		List<IMyBeacon> beacons = new List<IMyBeacon>();
		List<IMyShipConnector> connectors = new List<IMyShipConnector>();

		antenna = GetBlockOnGrid<IMyRadioAntenna>(inProgram);
		aiFlightBlock = GetBlockOnGrid<IMyFlightMovementBlock>(inProgram);
		rcBlock = GetBlockOnGrid<IMyRemoteControl>(inProgram);

		//optional blocks
		combatBlock = GetBlockOnGrid<IMyOffensiveCombatBlock>(inProgram);
		defenseBlock = GetBlockOnGrid<IMyDefensiveCombatBlock>(inProgram);
		beacon = GetBlockOnGrid<IMyBeacon>(inProgram);
		tanks = GetBlocksOnGrid<IMyGasTank>(inProgram);
		thrusters = GetBlocksOnGrid<IMyThrust>(inProgram);
		batteries = GetBlocksOnGrid<IMyBatteryBlock>(inProgram);
		parachutes = GetBlocksOnGrid<IMyParachute>(inProgram);

		List<IMyProgrammableBlock> pbs = GetBlocksOnGrid<IMyProgrammableBlock>(inProgram);
		solarBlock = pbs.FirstOrDefault(pb => pb.CustomName.Contains(SOLAR_TAG));

		turrets = GetBlocksOnGrid<IMyFunctionalBlock>(inProgram).Where(x => x is IMyTurretControlBlock || x is IMyLargeTurretBase).ToList();

		//turn on and enable necessary blocks
		antenna.Enabled = true;
		rcBlock.IsMainCockpit = true;
		rcBlock.FlightMode = FlightMode.OneWay;

		//InitializeAIBlock(defenseBlock);
		InitializeAIBlock(aiFlightBlock);
		InitializeAIBlock(combatBlock);
		InitializeAIBlock(taskBlock);

		logger = Grid.GetBlockWithName(LOGGER) as IMyTextPanel;
		if(logger != null) {
			logger.WriteText(""); // clear the log on recompile
			logger.ContentType = ContentType.TEXT_AND_IMAGE;
			logger.FontColor = Color.White;
			logger.BackgroundColor = Color.Black;
		}
	}


	class DetectorCollection {
		List<IMyFunctionalBlock> Detectors;

		public DetectorCollection() {
			Detectors = new List<IMyFunctionalBlock>();
		}

		public void Update() {
			Detectors.Clear();
			Grid.GetBlocksOfType<IMyFunctionalBlock>(Detectors, IsDetector);
		}

		public IEnumerable<MyDetectedEntityInfo> Entities { get {
			return Detectors.Select(Entity).Where(entity => ! entity.IsEmpty());
		}}

		bool IsDetector(IMyFunctionalBlock block) {
			return block is IMyTurretControlBlock || block is IMyLargeTurretBase || block is IMySensorBlock;
		}

		MyDetectedEntityInfo Entity(IMyFunctionalBlock detector) {
			MyDetectedEntityInfo detectedEntity = new MyDetectedEntityInfo();
			if (detector is IMyTurretControlBlock) {
				detectedEntity = RetrieveEntityName((detector as IMyTurretControlBlock).GetTargetedEntity());
			} else if (detector is IMyLargeTurretBase) {
				detectedEntity = RetrieveEntityName((detector as IMyLargeTurretBase).GetTargetedEntity());
			} else if (detector is IMySensorBlock) {
				detectedEntity = (detector as IMySensorBlock).LastDetectedEntity;
			}
			return detectedEntity;
		}

		private MyDetectedEntityInfo RetrieveEntityName(MyDetectedEntityInfo info) {
			string detailedName = info.Name;
			if(info.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies && combatBlock?.SearchEnemyComponent.FoundEnemyId != null) { 
				LogMessage("Enemy Detected! Name: " + ParseName(combatBlock?.DetailedInfo), LogLevel.Info, true); 
				detailedName = ENEMY_FLAG + ParseName(combatBlock?.DetailedInfo); 
			}
			else if(info.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies && defenseBlock?.SearchEnemyComponent.FoundEnemyId != null) {
				LogMessage("Enemy Detected! Name: " + ParseName(defenseBlock?.DetailedInfo), LogLevel.Info, true); 
				detailedName = ENEMY_FLAG + ParseName(defenseBlock?.DetailedInfo); 
			}
			return new MyDetectedEntityInfo(
				info.EntityId,
				detailedName,
				info.Type,
				info.HitPosition,
				info.Orientation,
				info.Velocity,
				info.Relationship,
				info.BoundingBox,
				info.TimeStamp
			);
		}

		private string ParseName(string detailedInfo) {
			if(detailedInfo.Contains("Status: Attacking ")) return detailedInfo.Substring(detailedInfo.LastIndexOf("Attacking") + 10, detailedInfo.IndexOf("\n") - 18).Trim();
			else if(detailedInfo.Contains("Target Locked ")) return detailedInfo.Substring(detailedInfo.LastIndexOf("Target Locked") + 14, detailedInfo.IndexOf("\n")).Trim();
			return "Enemy";
		}
	}

	class EntityDatabase {
		HashSet<MyDetectedEntityInfo> ActualEntities;
		HashSet<MyDetectedEntityInfo> DeprecatedEntities;

		class DetectedEntityComparer : EqualityComparer<MyDetectedEntityInfo> {

			public override bool Equals(MyDetectedEntityInfo entity1, MyDetectedEntityInfo entity2) {
				return entity1.EntityId == entity2.EntityId;
			}

			public override int GetHashCode(MyDetectedEntityInfo entity) {
				return entity.EntityId.GetHashCode();
			}
		}

		public IEnumerable<MyDetectedEntityInfo> Actual { get { return ActualEntities; } }
		public IEnumerable<MyDetectedEntityInfo> Deprecated { get { return DeprecatedEntities; } }
		public IEnumerable<MyDetectedEntityInfo> All { get { return Actual.Concat(Deprecated); } }

		public EntityDatabase() {
			var Comparer = new DetectedEntityComparer();
			ActualEntities = new HashSet<MyDetectedEntityInfo>(Comparer);
			DeprecatedEntities = new HashSet<MyDetectedEntityInfo>(Comparer);
		}

		public void Update(IEnumerable<MyDetectedEntityInfo> entities) {
			DeprecatedEntities.UnionWith(ActualEntities);
			ActualEntities.Clear();
			ActualEntities.UnionWith(entities);
			DeprecatedEntities.ExceptWith(ActualEntities);
			// TODO: Remove Deprecated by timestamp
		}
	}

	class Transceiver {

		readonly string IGC_VISION = $"N0zeasDCScript-{MASTER_KEY}";
		readonly string IGC_STATUS = $"N0zeasDCScript-{MASTER_KEY}-StatusUpdates";

		private List<MyDetectedEntityInfo> PursuedEntities;
		public bool ping = false;
		public bool sentry = false;

		private readonly IMyBroadcastListener _listener;
		private readonly IMyBroadcastListener _aux_listener;

		public Transceiver() {
			_listener = MyIGC.RegisterBroadcastListener(IGC_VISION);
			_aux_listener = MyIGC.RegisterBroadcastListener(currentKey);
			PursuedEntities = new List<MyDetectedEntityInfo>();
		}

		public void Broadcast(IEnumerable<MyDetectedEntityInfo> entities) {
			bool foundEnemy = combatBlock?.SearchEnemyComponent.FoundEnemyId != null || defenseBlock?.SearchEnemyComponent.FoundEnemyId != null;
			if(foundEnemy || ping) antenna.Radius = MAX_ANTENNA_RANGE;
			else antenna.Radius = MIN_ANTENNA_RANGE;
			
			// only broadcast when we are targeting something
			MyIGC.SendBroadcastMessage(IGC_VISION, Serialize(entities), TransmissionDistance.TransmissionDistanceMax);
		}

		public void Broadcast(string status) {
			MyIGC.SendBroadcastMessage(IGC_STATUS, status, TransmissionDistance.TransmissionDistanceMax);
		}

		public IEnumerable<MyDetectedEntityInfo> Receive() {
			while(_listener.HasPendingMessage) {
				LogMessage("Message Received", LogLevel.Debug);
				MyIGCMessage message = _listener.AcceptMessage();
				var stringData = message.Data as string;
				if(stringData != null && stringData.Contains(MASTER_KEY)) ParseCommand(stringData);
				else {
					ImmutableList<
					MyTuple<
					MyTuple<long, string, int, Vector3D, bool, MatrixD>,
					MyTuple<Vector3, int, BoundingBoxD, long>>> data;
					try {
						data = (ImmutableList<MyTuple<MyTuple<long, string, int, Vector3D, bool, MatrixD>, MyTuple<Vector3, int, BoundingBoxD, long>>>)message.Data;
					} catch(Exception) {
						LogMessage($"Unable to parse message with tag {message.Tag}. Continuing...", LogLevel.Error);
						continue;
					}
					IEnumerable<MyDetectedEntityInfo> entities = Deserialize(data);
					PursuedEntities = entities.Union(PursuedEntities.Where(p => !entities.Any(e => p.Name == e.Name))).ToList();
				}
			}
			DetermineEnemies();
			return PursuedEntities;
		}

		public void AuxReceive() {
			while(_aux_listener.HasPendingMessage) {
				MyIGCMessage message = _aux_listener.AcceptMessage();
				var stringData = message.Data as string;
				ping = true;
				if(stringData.GetHashCode() == int.Parse(currentKey)) MyIGC.SendBroadcastMessage(currentKey, MASTER_KEY, TransmissionDistance.TransmissionDistanceMax);
			}
		}

		private void Reset() {
			PursuedEntities.Clear();
			rcBlock.ClearWaypoints();
			rcBlock.SetAutoPilotEnabled(false);
			aiFlightBlock.Enabled = true;
		}
		
		private void ParseCommand(string command) {		
			command = command.Substring(MASTER_KEY.Length);
			if(command.Contains("NAME:" + combatBlock?.CubeGrid.CustomName.ToUpper() + ";") || command.Contains("~~ALL~~")) {
				string[] commands = command.Split(';');
				command = commands[1];
				switch(command) {
					case "PING":
						ping = true;
						break;
					case "PONG":
						ping = false;
						break;
					case "SENTRY":
						combatBlock?.ApplyAction("ActivateBehavior_Off");
						sentry = true;
						Reset();
						break;
					case "ENGAGE":
						combatBlock?.ApplyAction("ActivateBehavior_On");
						sentry = false;
						break;
					case "RESET":
						Reset();
						break;
					case "WEAPONSON":
						turrets.ForEach(x => x.Enabled = true);
						break;
					case "WEAPONSOFF":
						turrets.ForEach(x => x.Enabled = false);
						break;
					case "ENABLESOLAR":
						allowSolarMode = true;
						break;
					case "DISABLESOLAR":
						allowSolarMode = false;
						break;
					case "SOS":
						sentry = false;
						ping = true;
						MyDetectedEntityInfo sosShip = new MyDetectedEntityInfo(
							1337, 
							"~~SOS~~",
							MyDetectedEntityType.Unknown, 
							null, 
							new MatrixD(),
							new Vector3(), 
							MyRelationsBetweenPlayerAndBlock.Friends, 
							new BoundingBoxD(new Vector3D(double.Parse(commands[2]), double.Parse(commands[3]), double.Parse(commands[4])), new Vector3D(double.Parse(commands[2]), double.Parse(commands[3]), double.Parse(commands[4]))), 
							1337
						);
						PursuedEntities.Clear();
						PursuedEntities.Add(sosShip);
						currentBehavior += "Responding to SOS";
						break;
					case "CIRCLE":
						combatBlock?.ApplyAction("ActivateBehavior_On");
						combatBlock.SelectedAttackPattern = 0;
						sentry = false;
						break;
					case "ATRANGE":
						combatBlock?.ApplyAction("ActivateBehavior_On");
						combatBlock.SelectedAttackPattern = 1;
						sentry = false;
						break;
					case "HITANDRUN":
						combatBlock?.ApplyAction("ActivateBehavior_On");
						combatBlock.SelectedAttackPattern = 2;
						sentry = false;
						break;
					case "INTERCEPT":
						combatBlock?.ApplyAction("ActivateBehavior_On");
						combatBlock.SelectedAttackPattern = 3;
						bool collisionOverride = combatBlock.GetValueBool(combatBlock?.GetProperty("OffensiveCombatIntercept_OverrideCollisionAvoidance").Id);
						if(!collisionOverride) combatBlock?.ApplyAction("OffensiveCombatIntercept_OverrideCollisionAvoidance");
						sentry = false;
						break;
					case "FETCH":
						Broadcast("FETCH;" + combatBlock?.CubeGrid.CustomName.ToUpper());
						break;
					default:
						LogMessage("INVALID COMMAND " + command + " SPECIFIED", LogLevel.Error);
						break;
				}
			}
		}

		private void DetermineEnemies() {
			MyDetectedEntityInfo potentialEnemy = new MyDetectedEntityInfo();
			foreach(MyDetectedEntityInfo entity in PursuedEntities) {
				LogMessage("Is entity " + entity.Name + " an enemy?", LogLevel.Debug);
				// Deal with what's closest to you
				if((entity.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies && entity.Name.Contains(ENEMY_FLAG)) || entity.Name == "~~SOS~~") {
					if(potentialEnemy.IsEmpty()) potentialEnemy = entity;
					double newDistance = Vector3D.Distance(rcBlock.GetPosition(), entity.Position);
					double originalDistance = Vector3D.Distance(rcBlock.GetPosition(), potentialEnemy.Position);
					if(newDistance < originalDistance) potentialEnemy = entity;
					LogMessage("YES", LogLevel.Debug);
				}
			}

			if(!potentialEnemy.IsEmpty()) {
				HuntEnemy(potentialEnemy);
			} else if(combatBlock?.SearchEnemyComponent.FoundEnemyId != null || defenseBlock?.SearchEnemyComponent.FoundEnemyId != null) {
				currentBehavior += "Currently engaged with enemy \n";
			} else {
				currentBehavior += "Chillin' like a villain \n";
			}
		}

		private void HuntEnemy(MyDetectedEntityInfo enemy) {
			double currentPositionDifference = Vector3D.Distance(rcBlock.GetPosition(), enemy.Position);
			double enemyPosDifference = Vector3D.Distance(rcBlock.CurrentWaypoint.Coords, enemy.Position);
			bool rcControl = true;
			currentBehavior += "Chasing down enemy waypoints \n";
			if(currentPositionDifference < 2000 || sentry) { 
				LogMessage("Reached enemy at " + enemyPosDifference, LogLevel.Info);
				PursuedEntities.Remove(enemy); // add to ignored entities so we don't keep coming back after already engaging enemy
				rcControl = false;
				SwapFlightControl(rcControl, enemy.Position); // Switch off rc if closer than 2km
				currentBehavior += "Reached enemy's last reported location \n";
			}
			else if(enemyPosDifference > 2000) {
				LogMessage("Still haven't reached enemy at distance of " + enemyPosDifference, LogLevel.Info);
				rcControl = true;
				SwapFlightControl(rcControl, enemy.Position); // Change trajectory if more than 2km off
			}

			rcBlock.SetAutoPilotEnabled(rcControl);
			aiFlightBlock.Enabled = !rcControl;

			LogMessage($"Distance to {enemy.Name}: " + currentPositionDifference.ToString(), LogLevel.Info, true);
		}

		private void SwapFlightControl(bool rcControl, Vector3 enemyCoords) {
			rcBlock.ClearWaypoints();
			if(rcControl) rcBlock.AddWaypoint(new MyWaypointInfo("Enemy Position", enemyCoords));
		}

		ImmutableList<
			MyTuple<
				MyTuple<long, string, int, Vector3D, bool, MatrixD>,
				MyTuple<Vector3, int, BoundingBoxD, long>>>
		Serialize(IEnumerable<MyDetectedEntityInfo> entities) {
			return entities.Select(SerializeEntity).ToImmutableList();
		}

		IEnumerable<MyDetectedEntityInfo>
		Deserialize(ImmutableList<
			MyTuple<
				MyTuple<long, string, int, Vector3D, bool, MatrixD>,
				MyTuple<Vector3, int, BoundingBoxD, long>>> message) {
			List<MyDetectedEntityInfo> entities = new List<MyDetectedEntityInfo>();
			foreach(var item in message) {
				entities.Add(DeserializeEntity(item));
			}
			return entities;					
		}

		MyTuple<
			MyTuple<long, string, int, Vector3D, bool, MatrixD>,
			MyTuple<Vector3, int, BoundingBoxD, long>>
		SerializeEntity(MyDetectedEntityInfo entity) {
			var HitPosition = entity.HitPosition ?? new Vector3D();
			var UsesRaycast = entity.HitPosition == null;
			return MyTuple.Create(
				MyTuple.Create(entity.EntityId, entity.Name, (int)entity.Type, HitPosition, UsesRaycast, entity.Orientation),
				MyTuple.Create(entity.Velocity, (int)entity.Relationship, entity.BoundingBox, DateTime.Now.Ticks)); // timestamp we're getting is shit, use our own
		}

		MyDetectedEntityInfo
		DeserializeEntity(MyTuple<MyTuple<long, string, int, Vector3D, bool, MatrixD>, MyTuple<Vector3, int, BoundingBoxD, long>> serializedEntity) {
			return new MyDetectedEntityInfo(
				serializedEntity.Item1.Item1, 
				serializedEntity.Item1.Item2, 
				(MyDetectedEntityType)serializedEntity.Item1.Item3, 
				serializedEntity.Item1.Item4, 
				serializedEntity.Item1.Item6, 
				serializedEntity.Item2.Item1, 
				(MyRelationsBetweenPlayerAndBlock)serializedEntity.Item2.Item2, 
				serializedEntity.Item2.Item3, 
				serializedEntity.Item2.Item4
			);
		}
	}

	class MeInfo {
		readonly IMyTextSurface LCD;
		private const string HISTORICAL_LCD = "ENEMY_LCD";

		public MeInfo() {
			LCD = Grid.GetBlockWithName(HISTORICAL_LCD) as IMyTextPanel;
			if(LCD != null) {
				LCD.ContentType = ContentType.TEXT_AND_IMAGE;
				LCD.FontColor = Color.White;
				LCD.BackgroundColor = Color.Black;
			}
		}

		public void EnemyDisplay(IEnumerable<MyDetectedEntityInfo> entities) {
			var enemies = entities.Where(x => x.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies);
            if(LCD != null && enemies.Any()) AddEnemies(enemies);
		}

		private void AddEnemies(IEnumerable<MyDetectedEntityInfo> entities) {
			string currentText = LCD.GetText();
			foreach(MyDetectedEntityInfo entity in entities) {
				if(!currentText.Contains(entity.EntityId.ToString())) currentText = AddEnemy(entity) + currentText;
			}
			LCD.WriteText(currentText);
		}

		private string AddEnemy(MyDetectedEntityInfo entity) {
			return 
			"ID: " + entity.EntityId.ToString() + "\n" + 
			"Name: " + entity.Name.Replace(ENEMY_FLAG, "") + "\n" + 
			"Type: " + entity.Type + "\n" + 
			"Orientation: " + entity.Orientation + "\n" + 
			"Velocity: " + entity.Velocity + "\n" + 
			"Relationship: " + entity.Relationship + "\n" + 
			"Bounding Box: " + entity.BoundingBox + "\n" + 
			"Size: " + entity.BoundingBox.Size + "\n" +
			"Time Stamp: " + new DateTime(entity.TimeStamp) + "\n" +
			"------------------------------------------- \n";
		}
	}

	class MeThrusters {
		public void UpdateThrusters() {
			bool stockpile;

			List<MyWaypointInfo> rcWaypoints = new List<MyWaypointInfo>();
			rcBlock.GetWaypointInfo(rcWaypoints);
			if((rcBlock.IsAutoPilotEnabled && rcWaypoints.Count > 0) || combatBlock?.SearchEnemyComponent.FoundEnemyId != null || defenseBlock?.SearchEnemyComponent.FoundEnemyId != null) { 
				thrusters.ForEach(t => t.Enabled = true);
				stockpile = false;
			}
			else {
				List<IMyAutopilotWaypoint> waypoints = new List<IMyAutopilotWaypoint>();
				aiFlightBlock.GetWaypoints(waypoints);
				stockpile = !aiFlightBlock.Enabled && rcBlock.GetShipVelocities().LinearVelocity.IsZero() && rcBlock.GetNaturalGravity().IsZero();
				if(stockpile) {
					thrusters.ForEach(t => t.Enabled = false); // disable thrusters if we're at our destination
				}
				else { 
					thrusters.ForEach(t => t.Enabled = true);
				}
			}
			tanks.ForEach(x => x.Stockpile = stockpile);
		}
	}

	class MeDistress {
		private void SetAntennaInfo(IEnumerable<MyDetectedEntityInfo> enemies) {
			antenna.ShowShipName = false;
			if(enemies.Any() && (combatBlock?.SearchEnemyComponent.FoundEnemyId != null || defenseBlock?.SearchEnemyComponent.FoundEnemyId != null)) {
				MyDetectedEntityInfo enemy = enemies.First();
				string threatLevel = GetThreatLevel(enemy);
				string antennaInfo = MyProgram.Me.CubeGrid.CustomName + " - THREAT LEVEL: " + threatLevel + " - SPEED: " + CalculateSpeed(enemy.Velocity) + " m/s";
				LogMessage(antennaInfo, LogLevel.Info, true);
				antenna.CustomName = antennaInfo;
			} else antenna.CustomName = MyProgram.Me.CubeGrid.CustomName;
		}

		private double CalculateSpeed(Vector3 velocity) {
			return Math.Round(Math.Abs(velocity.X) + Math.Abs(velocity.Y) + Math.Abs(velocity.Z));
		}


		private string GetThreatLevel(MyDetectedEntityInfo enemy) {
			// TODO: Figure out whats wrong with these boundingbox values. They keep changing every run. Theory is that velocity is screwing
			// with the values returned for X, Y and Z. Trying to account for this with the velocity in the formula but still doesn't seem to be working.
			BoundingSphereD mySphere = BoundingSphereD.CreateFromBoundingBox(MyProgram.Me.CubeGrid.WorldAABB);
			BoundingSphere enemySphere = BoundingSphereD.CreateFromBoundingBox(enemy.BoundingBox);
			double enemySize = enemySphere.Radius;
			double droneSize = mySphere.Radius;

			string report = "=== GRID SIZE COMPARISON ===\n";
			report += $"Target Type: {enemy.Type}\n";
			report += $"Target Dimensions: {enemySphere.Radius:F1}m \n";
			report += $"Your Dimensions: {mySphere.Radius:F1}m \n";
			LogMessage(report, LogLevel.Debug);

			if(enemySize < droneSize / 2 || enemy.Type == MyDetectedEntityType.CharacterHuman) return "ALPHA"; // smaller grid or character, not a threat
			if(enemySize < droneSize) return "BETA"; // smaller than our drone, prolly ok
			else if(enemySize > droneSize * 2) return "OMEGA"; // much bigger than our drone
			else return "GAMMA"; // bigger than or equal to our drone, tough fight
		}

		public void UpdateDistress(IEnumerable<MyDetectedEntityInfo> entities) {
			SetAntennaInfo(entities);

			// check low hydrogen
			double tankCount = 0;
			double avgFill = 0;
			foreach(IMyGasTank tank in tanks) {
				tankCount++;
				avgFill += tank.FilledRatio;
			}
			avgFill = avgFill / tankCount;
			LogMessage("Hydrogen: " + (!double.IsNaN(avgFill) ? Math.Round(avgFill * 100).ToString() + "%" : "N/A"), LogLevel.Debug, true);
			if(avgFill < 0.02) {
				tanks.ForEach(x => x.Stockpile = true);
				if(beacon != null) {
					beacon.Enabled = true;
					beacon.CustomName = "LOW HYDROGEN";
				}
			}

			// check low power
			double battCount = 0;
			double avgPower = 0;
			foreach(IMyBatteryBlock batt in batteries) {
				battCount++;
				avgPower += batt.CurrentStoredPower / batt.MaxStoredPower;
			}
			avgPower = avgPower / battCount;
			LogMessage("Battery Charge: " + Math.Round(avgPower * 100).ToString() + "%", LogLevel.Debug, true);
			if(avgPower <= 0.05) {
				if(beacon != null) {
					beacon.Enabled = true;
					beacon.CustomName = "LOW POWER";
					antenna.Radius = 10;
				}
				if (allowParachuting && parachutes.Any()) {
					parachutes.ForEach(p => p.OpenDoor());
					thrusters.ForEach(t => t.Enabled = false);
					combatBlock?.ApplyAction("ActivateBehavior_Off");
					taskBlock?.ApplyAction("ActivateBehavior_Off");
					rcBlock.SetAutoPilotEnabled(false);
				}
			}
		}
	}

	class MeSolarScript {
		private void setSolarMode(bool turnOn, ref bool isSolarMode) {
			if(turnOn && !isSolarMode) {
				bool didRun = solarBlock.TryRun("resume");
				if(didRun) isSolarMode = true;
			} else if(!turnOn && isSolarMode) {
				bool didRun = solarBlock.TryRun("pause");
				if(didRun) isSolarMode = false;
			}
		}

		public void DetermineSolarMode(ref bool isSolarMode, ref DateTime settledAt) {
			if(solarBlock == null) return;
			if(isSolarMode) currentBehavior += "Catching rays \n";
			
			List<MyWaypointInfo> rcWaypoints = new List<MyWaypointInfo>();
			rcBlock.GetWaypointInfo(rcWaypoints);

			List<IMyAutopilotWaypoint> waypoints = new List<IMyAutopilotWaypoint>();
			aiFlightBlock.GetWaypoints(waypoints);
			bool enemyDetected = combatBlock?.SearchEnemyComponent.FoundEnemyId != null || defenseBlock?.SearchEnemyComponent.FoundEnemyId != null;
			bool settled = aiFlightBlock.Enabled && waypoints.Count() == 0 && rcBlock.GetShipVelocities().LinearVelocity.IsZero() && rcBlock.GetShipVelocities().AngularVelocity.IsZero();

			if((DateTime.Now - settledAt).Seconds > 0) {
				LogMessage("Settled:" + settledAt.ToString(), LogLevel.Debug);
				LogMessage("Settled for: " + (DateTime.Now - settledAt).Seconds.ToString(), LogLevel.Debug);
			}
			bool hasBeenSettled = (DateTime.Now - settledAt).TotalSeconds > 30;

 			if((rcBlock.IsAutoPilotEnabled && rcWaypoints.Count > 0) || enemyDetected) { 
				settledAt = DateTime.MaxValue;
				if(enemyDetected) aiFlightBlock.Enabled = true;
				LogMessage("Disabling solar mode...", LogLevel.Debug);
				setSolarMode(false, ref isSolarMode);
			}
			else if(settled) { // ship has fully settled somewhere
				currentBehavior += "Catching rays \n";
				if(settledAt.Year == DateTime.MaxValue.Year) { 
					settledAt = DateTime.Now;
				}
				if(hasBeenSettled) {
					aiFlightBlock.Enabled = false;
					solarBlock.TryRun("resume");
					LogMessage("Grid is calm. Enabling solar mode...", LogLevel.Debug);
					setSolarMode(true, ref isSolarMode);
				}
			} else { 
				currentBehavior += "Catching rays \n";
				settledAt = DateTime.MaxValue;
			}
		}
	}

	public Program() {
		try {
			Initialize(this);
		} catch(Exception e) {
			LogMessage("INITIALIZATION ERROR: " + e, LogLevel.Debug);
			LogMessage("Unable to initialize! Missing blocks. Exiting...", LogLevel.Error);
			return;
		}
		Runtime.UpdateFrequency = UpdateFrequency.Update100;
		Detectors = new DetectorCollection();
		Entities = new EntityDatabase();
		Com = new Transceiver();
		Info = new MeInfo();
		Thrusters = new MeThrusters();
		SolarScript = new MeSolarScript();
		Distress = new MeDistress();
	}

	public void Main(string argument, UpdateType updateSource) {
		if(argument.Equals("START", StringComparison.CurrentCultureIgnoreCase)) autostart = true;
		else if(argument.Equals("STOP", StringComparison.CurrentCultureIgnoreCase)) autostart = false;

		if(!autostart) {
			LogMessage("Idling until START command is issued...", LogLevel.Error); // not really an error, but we always want to show this
			return;
		}

		if(MASTER_KEY == "") {
			LogMessage("Master key not set! Please set the MASTER_KEY at the top of this script. Exiting...", LogLevel.Error);
			return;
		}

		try {
			droneStatus = "";
			string[] storageArr = Storage.Split(';');
			bool solarMode = false;
			DateTime settledAt = DateTime.MaxValue;
			if(storageArr.Length > 1) {
				bool.TryParse(storageArr[0], out solarMode);
				DateTime.TryParse(storageArr[1], out settledAt);
			}
			Detectors.Update();
			Entities.Update(Detectors.Entities);
			Com.Broadcast(Entities.Actual.Where(e => e.Relationship == MyRelationsBetweenPlayerAndBlock.Enemies));
			if(allowSolarMode) SolarScript.DetermineSolarMode(ref solarMode, ref settledAt);
			Storage = solarMode.ToString() + ";" + settledAt;
			IEnumerable<MyDetectedEntityInfo> entities = Com.Receive();
			Com.AuxReceive();
			Info.EnemyDisplay(entities);
			Thrusters.UpdateThrusters();
			Distress.UpdateDistress(Entities.All);
			Com.Broadcast("STATUS;" + droneStatus + ";" + currentBehavior + ";" + Me.CubeGrid.CustomName.ToUpper());
			currentBehavior = "";
		} catch(Exception e) {
			LogMessage("Error running the script! \n" + e.Message, LogLevel.Error);
		}
	}