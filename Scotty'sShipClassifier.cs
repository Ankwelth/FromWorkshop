// Scotty's Ship Classification Script v0.2
//
// This script attempts to classify ships based on a number of criteria:
// - Total Block Volume:
//     Total number of block cubes occupied by the grid.
//     For example, an armor block contributes 1 volume, a basic refinery 2, and a normal refinery 16.
// - Terminal Block Volume:
//     Same as 'Total Block Volume', but only taking into account blocks that show up in the terminal.
// - Terminal Block Mass:
//     The total mass of all terminal blocks.
// - Grid Dimensions:
//     The dimensions of the grid.
//
// This script also has a work-in-progress combat rating that it will give to each ship.
//
// The output of this script has been tuned specifically for my own ship classification system,
// and I do not expect everyone to agree with it, so I have attempted to make this script as modular
// and easily modifiable as possible, with comments to explain how the algorithm works.
//
// In particular, this algorithm is tuned to value physical size and volume very highly, and raw mass very low.

private readonly Vector3I GridMin;
private readonly Vector3I GridMax;
private readonly Vector3I GridSize;
private Vector3I_RangeIterator Iter;
private ProgramMode Mode;

private BlocksData AllBlocks = new BlocksData();
private BlocksData TerminalBlocks = new BlocksData();
private Dictionary<string, uint> BlockCounts = new Dictionary<string, uint>();
private Dictionary<string, uint> WeaponArchetypeCounts = new Dictionary<string, uint>();

// This maps block definitions to a 'block cost' value that will be totaled and shown in the ship summary.
// Once you run the script on a ship, the custom data of the programmable block will have a list of
// definition names corresponding to blocks on the grid and the number of blocks of that type.
// Only terminal blocks can be added here, non-terminal blocks cannot be detected by scripts.
private static Dictionary<string, double> BlockCosts = new Dictionary<string, double> {
  //["MyObjectBuilder_LargeGatlingTurret/"] = 1.0,
};

private struct BlocksData {
  public int Count;
  public int Volume;
  public double Mass;

  public int MassRounded => (int)Math.Round(this.Mass);

  public BlocksData(int count = 0, int volume = 0, double mass = 0.0) {
    this.Count = count;
    this.Volume = volume;
    this.Mass = mass;
  }
}

private static Vector3I SortVec(Vector3I v) {
  int[] a = new int[] { v.X, v.Y, v.Z };
  Array.Sort(a);
  return new Vector3I(a[2], a[1], a[0]);
}

private struct WeaponData {
  public string Name;
  public bool IsFixed;
  public WeaponValues Values;

  public WeaponData(string name, bool isFixed, double antiLightArmor, double antiHeavyArmor, double defensive) {
    this.Name = name;
    this.IsFixed = isFixed;
    this.Values = new WeaponValues(antiLightArmor, antiHeavyArmor, defensive);
  }
}

private struct WeaponValues {
  public double AntiLightArmor;
  public double AntiHeavyArmor;
  public double Defensive;

  public WeaponValues(
    double antiLightArmor = 0.0,
    double antiHeavyArmor = 0.0,
    double defensive = 0.0
  ) {
    this.AntiLightArmor = antiLightArmor;
    this.AntiHeavyArmor = antiHeavyArmor;
    this.Defensive = defensive;
  }
}

// This maps block definitions to a 'weapon archetype' name in order to simplify giving the same stats to turrets and their re-skins.
private static Dictionary<string, string> WeaponsList = new Dictionary<string, string> {
  ["MyObjectBuilder_InteriorTurret/LargeInteriorTurret"] = "LargeTurretInterior",
  ["MyObjectBuilder_LargeGatlingTurret/"] = "LargeTurretGatling",
  ["MyObjectBuilder_LargeGatlingTurret/LargeGatlingTurretReskin"] = "LargeTurretGatling",
  ["MyObjectBuilder_LargeMissileTurret/"] = "LargeTurretMissile",
  ["MyObjectBuilder_LargeMissileTurret/LargeBlockMediumCalibreTurret"] = "LargeTurretAssault",
  ["MyObjectBuilder_LargeMissileTurret/LargeCalibreTurret"] = "LargeTurretArtillery",
  ["MyObjectBuilder_LargeMissileTurret/LargeMissileTurretReskin"] = "LargeTurretMissile",
  ["MyObjectBuilder_SmallMissileLauncher/LargeBlockLargeCalibreGun"] = "LargeFixedArtillery",
  ["MyObjectBuilder_SmallMissileLauncher/LargeMissileLauncher"] = "LargeFixedMissile",
  ["MyObjectBuilder_SmallMissileLauncherReload/LargeRailgun"] = "LargeFixedRailgun",
  ["MyObjectBuilder_LargeGatlingTurret/AutoCannonTurret"] = "SmallTurretAutocannon",
  ["MyObjectBuilder_LargeGatlingTurret/SmallGatlingTurret"] = "SmallTurretGatling",
  ["MyObjectBuilder_LargeGatlingTurret/SmallGatlingTurretReskin"] = "SmallTurretGatling",
  ["MyObjectBuilder_LargeMissileTurret/SmallBlockMediumCalibreTurret"] = "SmallTurretAssault",
  ["MyObjectBuilder_LargeMissileTurret/SmallMissileTurret"] = "SmallTurretMissile",
  ["MyObjectBuilder_LargeMissileTurret/SmallMissileTurretReskin"] = "SmallTurretMissile",
  ["MyObjectBuilder_SmallGatlingGun/"] = "SmallFixedGatling",
  ["MyObjectBuilder_SmallGatlingGun/SmallBlockAutocannon"] = "SmallFixedAutocannon",
  ["MyObjectBuilder_SmallGatlingGun/SmallGatlingGunWarfare2"] = "SmallFixedGatling",
  ["MyObjectBuilder_SmallMissileLauncher/"] = "SmallFixedMissile",
  ["MyObjectBuilder_SmallMissileLauncher/SmallMissileLauncherWarfare2"] = "SmallFixedMissile",
  ["MyObjectBuilder_SmallMissileLauncherReload/SmallBlockMediumCalibreGun"] = "SmallFixedAssault",
  ["MyObjectBuilder_SmallMissileLauncherReload/SmallRailgun"] = "SmallFixedRailgun",
  ["MyObjectBuilder_SmallMissileLauncherReload/SmallRocketLauncherReload"] = "SmallFixedMissileRel",
};

// This defines weapon values for each weapon archetype. Weapon values are used for calculating combat rating.
// TODO: Improve; These values are work-in-progress, and may give inaccurate or poor quality ratings.
private static Dictionary<string, WeaponData> WeaponArchetypesList = new Dictionary<string, WeaponData> {
  ["LargeFixedArtillery"] = new WeaponData("Artillery Gun", true, 1.50, 2.00, 0.00),
  ["LargeTurretArtillery"] = new WeaponData("Artillery Turret", false, 3.00, 4.00, 0.00),
  ["LargeTurretAssault"] = new WeaponData("Assault Cannon Turret", false, 2.50, 2.50, 0.50),
  ["LargeTurretGatling"] = new WeaponData("Gatling Turret", false, 2.00, 2.00, 3.00),
  ["LargeTurretInterior"] = new WeaponData("Interior Turret", false, 1.00, 1.00, 2.00),
  ["LargeFixedMissile"] = new WeaponData("Missile Launcher", true, 4.00, 2.50, 0.00),
  ["LargeTurretMissile"] = new WeaponData("Missile Turret", false, 4.00, 2.00, 0.00),
  ["LargeFixedRailgun"] = new WeaponData("Railgun", true, 5.00, 3.00, 0.00),
  ["SmallFixedAssault"] = new WeaponData("Assault Gun", true, 1.25, 1.25, 0.25),
  ["SmallTurretAssault"] = new WeaponData("Assault Cannon Turret", false, 1.25, 1.25, 0.25),
  ["SmallFixedAutocannon"] = new WeaponData("Autocannon", true, 2.00, 1.25, 3.00),
  ["SmallTurretAutocannon"] = new WeaponData("Autocannon Turret", false, 2.00, 1.25, 3.00),
  ["SmallFixedGatling"] = new WeaponData("Gatling Gun", true, 1.50, 1.50, 2.25),
  ["SmallTurretGatling"] = new WeaponData("Gatling Turret", false, 2.00, 2.00, 3.00),
  ["SmallFixedMissile"] = new WeaponData("Rocket Launcher", true, 1.00, 0.75, 0.00),
  ["SmallFixedMissileRel"] = new WeaponData("Reloadable Rocket Launcher", true, 2.00, 1.25, 0.00),
  ["SmallTurretMissile"] = new WeaponData("Missile Turret", false, 2.00, 1.00, 0.00),
  ["SmallFixedRailgun"] = new WeaponData("Railgun", true, 2.50, 1.50, 0.00),
};

public void Main(string argument, UpdateType updateSource) {
  this.Mode = this.Run();
  Runtime.UpdateFrequency = this.Mode == ProgramMode.None
    ? UpdateFrequency.None : UpdateFrequency.Once;
}

public Program() {
  this.GridMin = Me.CubeGrid.Min;
  this.GridMax = Me.CubeGrid.Max;
  // Ship dimensions get sorted here because they depend on how the grid
  // was oriented when it was created, which tends to get messed up pretty often.
  this.GridSize = SortVec(Vector3I.Abs(Me.CubeGrid.Max - Me.CubeGrid.Min) + 1);

  this.Mode = ProgramMode.Start;
  this.Iter = new Vector3I_RangeIterator(ref this.GridMin, ref this.GridMax);
}

private enum ProgramMode {
  Start, BlocksLoop, End, None
}

private ProgramMode Run() {
  switch (this.Mode) {
    case ProgramMode.Start: return this.RunStart();
    case ProgramMode.BlocksLoop: return this.RunBlocksLoop();
    case ProgramMode.End: return this.RunEnd();
    case ProgramMode.None: throw new Exception("unreachable");
  }

  return ProgramMode.None;
}

private ProgramMode RunStart() {
  List<IMyTerminalBlock> terminalBlocks = new List<IMyTerminalBlock>();
  GridTerminalSystem.GetBlocks(terminalBlocks);

  this.TerminalBlocks.Count = terminalBlocks.Count;
  foreach (IMyTerminalBlock block in terminalBlocks) {
    this.TerminalBlocks.Mass += (double)block.Mass;
    string blockId = block.BlockDefinition.ToString();
    Increment(this.BlockCounts, blockId);
    if (WeaponsList.ContainsKey(blockId)) {
      Increment(this.WeaponArchetypeCounts, WeaponsList[blockId]);
    }
  }

  return ProgramMode.BlocksLoop;
}

private ProgramMode RunBlocksLoop() {
  Echo("Checking Blocks...");

  Vector3I position;
  for (int i = 0; i < 2048; i ++) {
    if (!this.Iter.IsValid()) return ProgramMode.End;
    this.Iter.GetNext(out position);
    this.RunBlocksLoopStep(position);
  }

  return ProgramMode.BlocksLoop;
}

private void RunBlocksLoopStep(Vector3I position) {
  if (Me.CubeGrid.CubeExists(position)) {
    IMySlimBlock block = Me.CubeGrid.GetCubeBlock(position);
    if (block != null) this.TerminalBlocks.Volume ++;
    this.AllBlocks.Volume ++;
  }
}

private ProgramMode RunEnd() {
  Echo(this.GetProgramPrintData());
  Me.CustomData = this.GetProgramCustomData();
  return ProgramMode.None;
}

private string GetProgramCustomData() {
  List<string> blocksList = new List<string>();
  foreach (KeyValuePair<string, uint> entry in this.BlockCounts) {
    blocksList.Add($"{entry.Key}: {entry.Value.ToString()}");
  }

  string technicalData = string.Join(";", new string[] {
    this.AllBlocks.Volume.ToString(),
    this.TerminalBlocks.Count.ToString(),
    this.TerminalBlocks.MassRounded.ToString(),
    this.TerminalBlocks.Volume.ToString(),
    this.GridSize.X.ToString(),
    this.GridSize.Y.ToString(),
    this.GridSize.Z.ToString(),
    this.GetTurretWeaponCount().ToString(),
    this.GetFixedWeaponCount().ToString()
  });

  string blocksListData = string.Join("\n", blocksList.ToArray());
  
  return technicalData + "\n\n" + blocksListData;
}

private string GetProgramPrintData() {
  double classFromVolume = this.GetClassFromVolume();
  double classFromTVolume = this.GetClassFromTVolume();
  double classFromTMass = this.GetClassFromTMass();
  double classFromDimensions = this.GetClassFromDimensions();
  double classFinal = WeightedAverage(new KeyValuePair<double, double>[] {
    new KeyValuePair<double, double>(8.0, classFromVolume),
    new KeyValuePair<double, double>(4.0, classFromTVolume),
    new KeyValuePair<double, double>(1.0, classFromTMass),
    new KeyValuePair<double, double>(3.0, classFromDimensions)
  });

  WeaponValues actualWeaponsValues = this.GetWeaponsValues();
  WeaponValues medianWeaponsValues = this.GetMedianWeaponsValuesForClass(classFinal);
  double ratingAntiLightArmor = actualWeaponsValues.AntiLightArmor / medianWeaponsValues.AntiLightArmor;
  double ratingAntiHeavyArmor = actualWeaponsValues.AntiHeavyArmor / medianWeaponsValues.AntiHeavyArmor;
  double ratingDefensive = actualWeaponsValues.Defensive / medianWeaponsValues.Defensive;
  double ratingFinal = WeightedAverage(new KeyValuePair<double, double>[] {
    new KeyValuePair<double, double>(1.0, ratingAntiLightArmor),
    new KeyValuePair<double, double>(1.0, ratingAntiHeavyArmor),
    new KeyValuePair<double, double>(1.0, ratingDefensive)
  });

  string shipCostMessage = BlockCosts.Count == 0 ? "N/A" : this.GetShipCost().ToString("0.0");

  return string.Join("\n", new string[] {
    "[Ship Analysis]",
    $"- Dimensions: {this.GridSize.X}, {this.GridSize.Y}, {this.GridSize.Z}",
    $"- Turret Count: {this.GetTurretWeaponCount()}",
    $"- Static Weapon Count: {this.GetFixedWeaponCount()}",
    $"- Ship Cost: {shipCostMessage}",
    "All Blocks:",
    $"- Volume: {this.AllBlocks.Volume}",
    "Terminal Blocks:",
    $"- Count: {this.TerminalBlocks.Count}",
    $"- Mass: {this.TerminalBlocks.Mass:0.00}",
    $"- Volume: {this.TerminalBlocks.Volume}",
    "Ship Class:",
    $"- Final: {classFinal:0.000}, {GetClassName(classFinal)}",
    $"- Based On Block Volume: {classFromVolume:0.000}",
    $"- Based On T-Block Volume: {classFromTVolume:0.000}",
    $"- Based On T-Block Mass: {classFromTMass:0.000}",
    $"- Based On Dimensions: {classFromDimensions:0.000}",
    "Ship Combat Rating (WIP):",
    $"- Final: {ratingFinal:0.000}, {GetRatingName(ratingFinal)}",
    $"- Anti-Light Armor: {ratingAntiLightArmor:0.000}, {GetRatingName(ratingAntiLightArmor)}",
    $"- Anti-Heavy Armor: {ratingAntiHeavyArmor:0.000}, {GetRatingName(ratingAntiHeavyArmor)}",
    $"- Defensive: {ratingDefensive:0.000}, {GetRatingName(ratingDefensive)}",
    "\n" + ClassNameInfo
  });
}

private double GetShipCost() {
  double cost = 0.0;
  foreach (KeyValuePair<string, uint> entry in this.BlockCounts) {
    if (BlockCosts.ContainsKey(entry.Key)) {
      cost += BlockCosts[entry.Key] * (double)entry.Value;
    }
  }

  return cost;
}

private uint GetTurretWeaponCount() {
  uint count = 0;
  foreach (KeyValuePair<string, uint> entry in this.WeaponArchetypeCounts) {
    if (WeaponArchetypesList.ContainsKey(entry.Key) && !WeaponArchetypesList[entry.Key].IsFixed) {
      count += entry.Value;
    }
  }

  return count;
}

private uint GetFixedWeaponCount() {
  uint count = 0;
  foreach (KeyValuePair<string, uint> entry in this.WeaponArchetypeCounts) {
    if (WeaponArchetypesList.ContainsKey(entry.Key) && WeaponArchetypesList[entry.Key].IsFixed) {
      count += entry.Value;
    }
  }

  return count;
}

private WeaponValues GetWeaponsValues() {
  WeaponValues weaponsValues = new WeaponValues();
  foreach (KeyValuePair<string, uint> entry in this.WeaponArchetypeCounts) {
    if (WeaponArchetypesList.ContainsKey(entry.Key)) {
      WeaponData data = WeaponArchetypesList[entry.Key];
      weaponsValues.AntiLightArmor += data.Values.AntiLightArmor * (double)entry.Value;
      weaponsValues.AntiHeavyArmor += data.Values.AntiHeavyArmor * (double)entry.Value;
      weaponsValues.Defensive += data.Values.Defensive * (double)entry.Value;
    }
  }

  return weaponsValues;
}

private double InvertedQuadratic(double x, double a, double b) {
  // Inverse of ax^2+bx
  return -(b - Math.Sqrt(b * b + 4.0 * a * x)) / (2 * a);
}

// These functions were created from sample data collected on a number of ships,
// primarily from the steam workshop, including the following collections:
// - Interstellar Mining and Defense Corp (IMDC), by JD. Horx
// - Ares Combat Industries (ACI), by k_medlock
// - Royal Coalition of Spatial Protection (RCSP), by Venom415
//
// As such, this specific algorithm is biased towards 'correctly' ranking ships of similar composition.
//
// These ships were then classified by me and my brother, after which I put
// the data points into a regression calculator to get quadratic functions
// that model the relationship between each stat and the ship class.
//
// This Desmos graph contains all of the formulas for everything here
// https://www.desmos.com/calculator/jy3n6nafam

// Gets a ship classification estimate given the ship's total block volume
private double GetClassFromVolume() {
  // Inverse of 1250\left(v^{2}+v\right)
  return InvertedQuadratic((double)this.AllBlocks.Volume, 1250.0, 1250.0);
}

// Gets a ship classification estimate given the ship's terminal block volume
private double GetClassFromTVolume() {
  // Inverse of 100v^{2}+860v
  return InvertedQuadratic((double)this.TerminalBlocks.Volume, 100.0, 860.0);
}

// Gets a ship classification estimate given the ship's terminal block mass
private double GetClassFromTMass() {
  // Inverse of 300000\left(m^{2}+3m\right)
  return InvertedQuadratic(this.TerminalBlocks.Mass, 300000.0, 900000.0);
}

// Gets a ship classification estimate given the ship's bounding box dimensions
private double GetClassFromDimensions() {
  // Inverse of 10\left(k+1\right)
  // Clamped to be greater than 0.0
  Vector3D g = new Vector3D(this.GridSize);
  double dimensionMetric = Math.Pow(g.X * g.Y * g.Z, 1.0 / 3.0);
  return Math.Max(0.0, dimensionMetric / 10 - 1.0);
}

// Gets the median weapons value expected of a given classification
// This value defines an 'anchor point' (C-tier rating), and is divided into the actual weapons value to get rating
private WeaponValues GetMedianWeaponsValuesForClass(double classFinal) {
  // TODO: Improve; This algorithm is a work-in-progress, and may give inaccurate or poor quality ratings.
  double antiLightArmor = 20.0 * classFinal;
  double antiHeavyArmor = 20.0 * classFinal;
  double defensive = 12.0 * classFinal;

  return new WeaponValues(
    antiLightArmor,
    antiHeavyArmor,
    defensive
  );
}

private static string ClassNameInfo = string.Join("\n", new string[] {
  "[Ship Class Criteria]",
  "- Corvette: 0.0 - 1.0",
  "- Frigate: 1.0 - 2.0",
  "- Cruiser: 2.0 - 3.0",
  "- Battleship: 3.0 - 5.0",
  "- Supercapital: > 5.0"
});

private static string GetClassName(double c) {
  if (c < 0.0) return "Invalid";
  // Battleships range from 3 to 5, this flattens the curve to accomodate
  if (c > 3.0) c = (c + 3.0) / 2.0;

  // Checks if this ship is on the limits between two classes
  // If so, return an ambiguous answer
  if (Math.Abs(1.0 - c) < 0.125) return "Corvette or Frigate";
  if (Math.Abs(2.0 - c) < 0.125) return "Frigate or Cruiser";
  if (Math.Abs(3.0 - c) < 0.125) return "Cruiser or Battleship";
  if (Math.Abs(4.0 - c) < 0.125) return "Battleship or Supercapital";

  // Checks where this ship is within its class (Small, Medium, Large)
  string modifier = "Medium";
  if (c % 1.0 > 0.625) modifier = "Large";
  if (c % 1.0 < 0.375) modifier = "Small";

  switch ((uint)Math.Floor(c)) {
    case 0: return $"Corvette ({modifier})";
    case 1: return $"Frigate ({modifier})";
    case 2: return $"Cruiser ({modifier})";
    case 3: return $"Battleship ({modifier})";
    // Anything above "Battleship" is a "Supercapital" and has no modifier
    default: return "Supercapital";
  }
}

private static string GetRatingName(double r) {
  if (r < 0.0) return "Invalid";
  if (r == 0.0) return "Unrated";

  //r = Math.Log2(r / 2.0);
  r = Math.Log(r, 2.0);

  string modifier = "";
  if (r % 1.0 > 2.0 / 3.0) modifier = "+";
  if (r % 1.0 < 1.0 / 3.0) modifier = "-";

  switch ((int)Math.Floor(r)) {
    case -3: return $"F{modifier}";
    case -2: return $"E{modifier}";
    case -1: return $"D{modifier}";
    case 0: return $"C{modifier}";
    case 1: return $"B{modifier}";
    case 2: return $"A{modifier}";
    case 3: return $"S{modifier}";
    default: return r > 0.0 ? "EX" : "Unrated";
  }
}

private static double WeightedAverage(KeyValuePair<double, double>[] elements) {
  double totalWeight = 0.0;
  double totalValue = 0.0;
  foreach (KeyValuePair<double, double> element in elements) {
    totalWeight += element.Key;
    totalValue += element.Value * element.Key;
  }

  return totalValue / totalWeight;
}

private static void Increment<T>(Dictionary<T, uint> dictionary, T key) {
  if (dictionary.ContainsKey(key)) {
    dictionary[key] += 1;
  } else {
    dictionary.Add(key, 1);
  }
}
