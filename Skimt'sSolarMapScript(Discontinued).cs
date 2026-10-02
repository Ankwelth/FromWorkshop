/*
 * SKIMT'S SOLARMAP
 * 
 * Version: 		0.9.2b
 * Date: 			September 9 2020
 * Author: 		Michael "Skimt" Winge
 * 
 * DESCRIPTION 
 * 
 * The solar map will project the world coordinates of planets and the grid unto a 
 * text panel in the form of a 2D map, along with detailed information regarding the 
 * planets and moons. If the grid is static or does not have a valid ship controller 
 * it will appear as a red dot instead of an arrow. Background and foreground color 
 * can be changed via the text panel's in-game control panel.
 * 
 * INSTALLATION
 * 
 * Add "[SolarMap]" without the citation marks into the CustomData of a text panel. 
 * The script should automatically pick up new text panels, although it might take 
 * a while because the script has been deliberately written to operate efficiently. 
 * Average wait time can be ~30 seconds, so if you are impatient you should recompile
 * the script.
 * 
 * AVAIALBLE CONFIGURATIONS
 * 
 * GridColor=#FF0000
 * HideGrid=true
 * HideMap=true
 * HideInfo=false
 * OffsetX=50
 * OffsetY=-50
 * 
 * GridColor is written in HTML Hex Color Code (#RGB, #RRGGBB), where 0 is black and
 * F is white on the following scale: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, A, B, C, D, E, F.
 * Example: #FF0000 is red, #00FF00 is green, #0000FF is blue. 
 * 
 * You do not need to add any of the configurations if you don't want to, and you only
 * need to select the configuration you need. Grid color will attempt to automatically
 * adjust if you do not force color using the GridColor configuration.
 */


private const UpdateFrequency FREQUENCY = UpdateFrequency.Update10;

public World world;
public List<CelestialBody> celestialBodies;
private readonly TerminalManager terminalManager;
private readonly ProgrammableBlock programmableBlock;

public Program()
{

	// ---------------------------------------------------------------
	// Celestial bodies - Start.
	celestialBodies = new List<CelestialBody>()
	{
		new CelestialBody
		{
			Name = "EarthLike",
			Radius = 60000,
			Gravity = 1,
			HasAtmosphere = true,
			Oxygen = Oxygen.High,
			Type = CelestialType.Planet,
			Position = new Vector3(0.5f, 0.5f, 0.5f),
			Resources = "All"
		},
		new CelestialBody
		{
			Name = "Moon",
			Radius = 9500,
			Gravity = 0.25f,
			HasAtmosphere = false,
			Oxygen = Oxygen.None,
			Type = CelestialType.Moon,
			Position = new Vector3(16384.5f, 136384.5f, -113615.5f),
			Resources = "All"
		},
		new CelestialBody
		{
			Name = "Mars",
			Radius = 60000,
			Gravity = 0.9f,
			HasAtmosphere = true,
			Oxygen = Oxygen.None,
			Type = CelestialType.Planet,
			Position = new Vector3(1031072.5f, 131072.5f, 1631072.5f),
			Resources = "All"
		},
		new CelestialBody
		{
			Name = "Europa",
			Radius = 9500,
			Gravity = 0.25f,
			HasAtmosphere = true,
			Oxygen = Oxygen.None,
			Type = CelestialType.Moon,
			Position = new Vector3(916384.5f, 16384.5f, 1616384.5f),
			Resources = "All"
		},
		new CelestialBody
		{
			Name = "Alien",
			Radius = 60000,
			Gravity = 1.1f,
			HasAtmosphere = true,
			Oxygen = Oxygen.Low,
			Type = CelestialType.Planet,
			Position = new Vector3(131072.5f, 131072.5f, 5731072.5f),
			Resources = "All"
		},
		//new CelestialBody
		//{
		//	Name = "Alien2",
		//	Radius = 60000,
		//	Gravity = 1.1f,
		//	HasAtmosphere = true,
		//	Oxygen = Oxygen.Low,
		//	Type = CelestialType.Planet,
		//	Position = new Vector3(131072.5f, 131072.5f, -4731072.5f),
		//	Resources = "All"
		//},
		//new CelestialBody
		//{
		//	Name = "Alien3",
		//	Radius = 60000,
		//	Gravity = 1.1f,
		//	HasAtmosphere = true,
		//	Oxygen = Oxygen.Low,
		//	Type = CelestialType.Planet,
		//	Position = new Vector3(-831072.5f, 131072.5f, -8731072.5f),
		//	Resources = "All"
		//},
		new CelestialBody
		{
			Name = "Titan",
			Radius = 9500,
			Gravity = 0.25f,
			HasAtmosphere = true,
			Oxygen = Oxygen.None,
			Type = CelestialType.Moon,
			Position = new Vector3(36384.5f, 226384.5f, 5796384.5f),
			Resources = "All"
		}
		/*,
				new CelestialBody
				{
					Name = "Triton",
					Radius = 40126.5f,
					Gravity = 1,
					HasAtmosphere = true,
					Oxygen = Oxygen.High,
					Type = CelestialType.Planet,
					Position = new Vector3(-284463.5f, -2434463.5, 365536.5f),
					Resources = "All"
				}
				*/
	};
	// Celestial bodies - End.
	// ---------------------------------------------------------------

	programmableBlock = new ProgrammableBlock(this, FREQUENCY);
	world = new World(this);
	terminalManager = new TerminalManager(this);

}

public void Main(string arg, UpdateType updateType)
{

	// The update type is binary. Must look it up on Malware's wikia to figure out how to manipulate it.
	if ((updateType & FrequencyByUpdateType[FREQUENCY]) == 0)
		return;

	terminalManager.Run();
	programmableBlock.Draw();

}

private readonly Dictionary<UpdateFrequency, UpdateType> FrequencyByUpdateType = new Dictionary<UpdateFrequency, UpdateType>
{
	{ UpdateFrequency.Update1, UpdateType.Update1 },
	{ UpdateFrequency.Update10, UpdateType.Update10 }
};

public class Antenna : Terminal<IMyRadioAntenna, AntennaSetting>
{

	public Antenna(Program program) : base(program) { }

	public override void OnCycle(IMyRadioAntenna terminal, AntennaSetting settings) { }

	public override AntennaSetting CreateSetting(IMyRadioAntenna item)
	{
		return new AntennaSetting();
	}

}

public struct AntennaSetting
{

}

public class CelestialBody
{

	public CelestialType Type;
	public Oxygen Oxygen;
	public Vector3 Position;
	public bool HasAtmosphere;
	public float Radius;
	public float Gravity;
	public string Name;
	public string Resources;


	/// <summary>
			/// OrbitPosition uses standard LCD size.
			/// </summary>
	public Vector2 OrbitPosition = new Vector2(512); // Standard LCD size.

	// Properties used in map.
	public Vector2 PlanetPosition;
	public Vector2 OrbitSize;
	public Vector2 PlanetSize;
	public Vector2 LblTitlePos;
	public Vector2 LblDistancePos;

}

public enum CelestialType
{
	Planet,
	Moon
}

public class ColorManager
{

	private Vector3 hsv = Vector3.Zero; // Hue, Saturation, Value

	public Color Fill { get; private set; }
	public Color Border { get; private set; }
	public Color Grid { get; private set; }
	public Color Text { get; private set; }
	public Color PanelBackground = new Color(0, 0, 0, 50);
	public Color TitleBackground = new Color(0, 0, 0, 150);

	private bool BgIsDark => hsv.Z < 0.125 || (hsv.Y > 0.5 && hsv.Z < 0.25);
	private bool BgIsRed => (hsv.X < 0.1 || hsv.X > 0.9) && hsv.Y > 0.65 && hsv.Z > 0.125;
	private bool BgIsBlue => hsv.X > 0.5 && hsv.X < 0.75 && hsv.Y > 0.25 && hsv.Z > 0.25;

	public void UpdateGeneralColors(IMyTextPanel lcd)
	{
		Fill = lcd.ScriptBackgroundColor;
		Border = new Color(lcd.ScriptForegroundColor, 0.5f);
		Text = lcd.ScriptForegroundColor;
	}

	public void UpdateGridArrowColor(IMyTextPanel lcd, Color? gridColor)
	{

		if (gridColor.HasValue)
		{
			Grid = gridColor.Value;
			return;
		}

		hsv = lcd.ScriptBackgroundColor.ColorToHSV();

		if (BgIsBlue)
		{
			Grid = Color.Red;
		}
		else if (BgIsRed)
		{
			Grid = Color.LimeGreen;
		}
		else
		{
			Grid = Color.Black;
			if (BgIsDark)
			{
				Grid = Color.Red;
			}
		}

	}

}

public class InfoPanel
{

	private readonly ColorManager colorManager;
	private readonly World world;

	private Vector2I panelLayout = Vector2I.One;
	private Vector2 size = new Vector2(512 - 14, 20);
	private Vector2 position = Vector2.Zero;
	private Vector2 margin = new Vector2(7, 2);
	private int tick;
	private TextPanelSetting setting;

	public InfoPanel(ColorManager colorManager, World world)
	{
		this.colorManager = colorManager;
		this.world = world;
	}

	public void PaintInfo(List<MySprite> sprites)
	{

		// Create a panel layout and sort them panel roughly every ~5 seconds.
		if (tick % 27 == 0)
		{
			world.CelestialInfo.Sort(SortByDistance);
		}
		tick++;

		sprites.Clear();

		// Panel background.
		if (!setting.HideMap)
		{
			sprites.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", new Vector2(95, 256), new Vector2(183, 505), colorManager.PanelBackground));
		}

		int cbCount = world.CelestialInfo.Count > 5 ? 6 : world.CelestialInfo.Count;
		panelLayout = setting.HideMap ? GetPanelLayout(world.CelestialInfo.Count) : new Vector2I(1, cbCount);

		// Position multiplier.
		int xPosMult = 0;
		int yPosMult = -1;
		float ySizeMult = 1.75f - (panelLayout.Y * (0.125f + (panelLayout.X - 1) * 0.01f)); // 1.75f when there are 1 and 2 rows.

		// Panel content.
		for (int i = 0; i < world.CelestialInfo.Count; i++)
		{

			if (panelLayout.Y - 1 == yPosMult)
			{
				yPosMult = 0;
				xPosMult++;
			}
			else
			{
				yPosMult++;
			}

			CelestialBody cb = world.CelestialInfo[i];

			// General.
			float marginX = setting.HideMap ? margin.X : 0;
			float sizeX = setting.HideMap ? size.X / panelLayout.X : (size.X / 3) + margin.X + 1;
			position.X = margin.X + (sizeX * xPosMult);
			position.Y = margin.X + (size.X / panelLayout.Y * yPosMult);

			// Title: Background.
			Vector2 titleBgPosition = new Vector2(position.X + (sizeX / 2), position.Y + (size.Y * ySizeMult / 2));
			Vector2 titleBgSize = new Vector2(sizeX - marginX, size.Y * ySizeMult);
			sprites.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", titleBgPosition, titleBgSize, colorManager.TitleBackground));

			// Title: Text.
			Vector2 titleTxtPosition = new Vector2(margin.X + position.X, margin.Y * ySizeMult + position.Y);
			float distance = Vector3.Distance(cb.Position, world.GridPosition) / 1000;
			sprites.Add(new MySprite(SpriteType.TEXT, cb.Name + " (" + distance.ToString("F1") + " km" + ")", titleTxtPosition, null, colorManager.Text, null, TextAlignment.LEFT, 0.5f * ySizeMult));

			// Body: Text.
			Vector2 bodyTxtPosition = new Vector2(margin.X + position.X, size.Y * ySizeMult + position.Y);
			string txt = "Radius: " + (cb.Radius / 1000).ToString("F1") + " km\n" +
						"Gravity: " + cb.Gravity.ToString("F1") + " G\n" +
						"Atmosphere: " + cb.HasAtmosphere + "\n" +
						"Oxygen: " + cb.Oxygen + "\n" +
						"Resources: " + cb.Resources + "\n";
			sprites.Add(new MySprite(SpriteType.TEXT, txt, bodyTxtPosition, null, colorManager.Text, null, TextAlignment.LEFT, 0.4f * ySizeMult));

			if (!setting.HideMap && i == 5)
			{
				break;
			}

		}

	}

	public void UpdateSetting(TextPanelSetting setting)
	{
		this.setting = setting;
	}

	/// <summary>
			/// Returns the amount of columns and rows that the information panel will have.
			/// </summary>
	private Vector2I GetPanelLayout(int num)
	{

		int x, y;

		// X
		if (num < 3)
			x = 1;
		else if (num > 8)
			x = 3;
		else
			x = 2;

		// Y
		if (num < 9)
			y = (int)Math.Round((0.42f * num) + 0.75f);
		else if (num > 9)
			y = (int)Math.Round((0.3f * num) + 0.8f);
		else
			y = 3;

		return new Vector2I(x, y);

	}

	private int SortByDistance(CelestialBody a, CelestialBody b)
	{

		float distanceA = Vector3.Distance(a.Position, world.GridPosition);
		float distanceB = Vector3.Distance(b.Position, world.GridPosition);

		if (distanceA < distanceB)
			return -1;
		else if (distanceB < distanceA)
			return 1;
		return 0;

	}

}

public class Map
{

	private readonly IEnumerable<CelestialBody> planets;
	//private readonly IEnumerable<CelestialBody> moons;
	private readonly World world;
	private readonly ColorManager colorManager;

	private Vector2 planetPositionMultiplier = Vector2.One;
	private Vector2 orbitPositionMultiplier = Vector2.One;
	private Vector2 lcdSize = new Vector2(512);
	private TextPanelSetting setting;

	public Map(ColorManager colorManager, World world)
	{

		this.colorManager = colorManager;
		this.world = world;

		planets = world.CelestialMap.Where(cb => cb.Type == CelestialType.Planet);
		//moons = world.CelestialMap.Where(cb => cb.Type == CelestialType.Moon);

		foreach (CelestialBody planet in planets)
		{

			planet.PlanetPosition = planet.OrbitPosition * world.WorldToMapPercent(planet.Position);
			planet.OrbitSize = new Vector2(Vector2.Distance(planet.PlanetPosition, planet.OrbitPosition)) * 2;
			planet.PlanetSize = planet.OrbitPosition * planet.Radius * 0.000001f - 0.01f;
			planet.LblTitlePos = new Vector2(planet.PlanetPosition.X, planet.PlanetPosition.Y - planet.OrbitPosition.Y * 0.1f);
			planet.LblDistancePos = new Vector2(planet.PlanetPosition.X, planet.PlanetPosition.Y - planet.OrbitPosition.Y * 0.065f);

		}

	}

	public void UpdateSetting(TextPanelSetting setting)
	{
		this.setting = setting;
	}

	public void PaintOrbits(List<MySprite> sprites)
	{

		sprites.Clear();

		foreach (CelestialBody planet in planets)
		{

			Vector2 orbitPosition = planet.OrbitPosition * orbitPositionMultiplier;
			planet.OrbitSize = new Vector2(Vector2.Distance((planet.PlanetPosition + setting.Offset) * planetPositionMultiplier, orbitPosition)) * 2;

			// Border, then fill.
			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", orbitPosition, planet.OrbitSize + 3, colorManager.Border));
			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", orbitPosition, planet.OrbitSize, colorManager.Fill));

		}

	}

	public void PaintPlanets(List<MySprite> sprites)
	{

		sprites.Clear();

		foreach (CelestialBody planet in planets)
		{

			Vector2 planetPosition = (planet.PlanetPosition + setting.Offset) * planetPositionMultiplier;
			float distance = Vector3.Distance(planet.Position, world.GridPosition) / 1000;

			// Text.
			sprites.Add(new MySprite(SpriteType.TEXT, planet.Name, (planet.LblTitlePos + setting.Offset) * planetPositionMultiplier, null, colorManager.Text, null, rotation: 0.7f));
			sprites.Add(new MySprite(SpriteType.TEXT, distance.ToString("F1") + " km", (planet.LblDistancePos + setting.Offset) * planetPositionMultiplier, null, colorManager.Text, null, rotation: 0.55f));

			// Border, then fill.
			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", planetPosition, planet.PlanetSize + 3, colorManager.Border));
			sprites.Add(new MySprite(SpriteType.TEXTURE, "Circle", planetPosition, planet.PlanetSize, colorManager.Fill));

		}

	}

	public MySprite PaintGrid(ShipController shipController)
	{

		Vector2 position = (lcdSize * world.WorldToMapPercent(world.GridPosition) + setting.Offset) * planetPositionMultiplier;

		if (shipController != null && shipController.IsMoveable)
		{
			float azimuth, elevation;
			Vector3.GetAzimuthAndElevation(shipController.Main.WorldMatrix.Forward, out azimuth, out elevation);
			return new MySprite(SpriteType.TEXTURE, "AH_BoreSight", position, lcdSize * 0.05f + 3, colorManager.Grid, null, rotation: -azimuth + (float)(Math.PI / 2f));
		}

		return new MySprite(SpriteType.TEXTURE, "Circle", position, lcdSize * 0.01f, colorManager.Grid);

	}

	/// <summary>
			/// Adjust multipliers for widescreens.
			/// </summary>
	public void UpdateMultipliers(IMyTextPanel lcd)
	{
		if (lcd.SurfaceSize.X > 512)
		{
			planetPositionMultiplier = setting.HideInfo ? Vector2.One : new Vector2(2, 1);
			orbitPositionMultiplier = setting.HideInfo ? Vector2.One : new Vector2(1.5f, 1);
		}
		else
		{
			planetPositionMultiplier = setting.HideInfo ? Vector2.One : new Vector2(1.25f, 1);
			orbitPositionMultiplier = Vector2.One;
		}
	}

}

public enum Oxygen
{
	None,
	Low,
	High
}

public class ProgrammableBlock
{

	protected Program program;
	private readonly IMyTextSurface textSurface;
	private readonly double[] runTimes = new double[10];
	private readonly int[] instructionCounts = new int[10];
	private int tick = 0;
	private TimeSpan time = new TimeSpan();

	public ProgrammableBlock(Program program, UpdateFrequency updateFrequency = UpdateFrequency.Update10)
	{
		this.program = program;
		program.Runtime.UpdateFrequency = updateFrequency;
		textSurface = program.Me.GetSurface(0);
		textSurface.ContentType = ContentType.SCRIPT;
	}

	private string AverageRunTime => runTimes.Average().ToString("F2");
	private int AverageInstructions => (int)instructionCounts.Average();
	private int HighestInstruction => instructionCounts.Max();

	public void Draw()
	{

		if (program.Me.CubeGrid.GridSizeEnum == MyCubeSize.Small)
			return;

		using (MySpriteDrawFrame frame = textSurface.DrawFrame())
		{

			frame.Add(new MySprite(SpriteType.TEXT, "SolarMap v0.9.1", new Vector2(10, 100), null, Color.White, null, TextAlignment.LEFT, 0.7f));
			frame.Add(new MySprite(SpriteType.TEXT, "Average instructions:\nInstruction peak:\nAverage runtime:\nRuntime:\nUpdates:", new Vector2(10, 125), null, Color.White, null, TextAlignment.LEFT, 0.6f));

			// Calculate various.
			runTimes[tick] = program.Runtime.LastRunTimeMs;
			instructionCounts[tick] = program.Runtime.CurrentInstructionCount;
			time += program.Runtime.TimeSinceLastRun;
			tick = tick > 8 ? 0 : ++tick;

			frame.Add(new MySprite(SpriteType.TEXT,
				AverageInstructions + "/" + program.Runtime.MaxInstructionCount + "\n" +
				HighestInstruction + "\n" +
				AverageRunTime + " ms\n" +
				time.ToString(@"dd\.hh\:mm\:ss") + "\n" +
				program.terminalManager.ShipController.UpdateCount,
				new Vector2(200, 125), null, Color.White, null, TextAlignment.LEFT, 0.6f));

			if (!program.terminalManager.ShipController.HasController)
			{
				frame.Add(new MySprite(SpriteType.TEXT, "Status", new Vector2(10, 250), null, Color.White, null, TextAlignment.LEFT, 0.7f));
				frame.Add(new MySprite(SpriteType.TEXT, "- Controller does not exist.", new Vector2(10, 275), null, Color.White, null, TextAlignment.LEFT, 0.6f));
			}

		}

	}

}

public class ShipController : Terminal<IMyShipController, ShipControllerSetting>
{

	private IMyShipController main;

	public ShipController(Program program) : base(program) { }

	public IMyShipController Main {
		get {
			return IsListEmpty || (main != null && (main.WorldMatrix == MatrixD.Identity || !main.IsWorking)) ? null : main;
		}
	}

	public bool HasController => !IsListEmpty && main != null && main.IsWorking;
	public bool IsMoveable => HasController && !program.Me.CubeGrid.IsStatic;

	public override void OnCycle(IMyShipController terminal, ShipControllerSetting settings)
	{

		if (terminal.IsMainCockpit)
		{
			main = terminal;
			return;
		}

		main = terminal;

	}

	public override bool Collect(IMyShipController terminal)
	{
		return terminal.IsSameConstructAs(program.Me) && terminal.CanControlShip && terminal.IsWorking;
	}

	public override ShipControllerSetting CreateSetting(IMyShipController item)
	{
		return new ShipControllerSetting();
	}
}

public struct ShipControllerSetting
{

}

public class SpriteManager
{

	private readonly ColorManager colorManager = new ColorManager();
	private readonly World world;

	public SpriteManager(World world)
	{

		this.world = world;
		Map = new Map(colorManager, world);
		InfoPanel = new InfoPanel(colorManager, world);

	}

	public Map Map;
	public InfoPanel InfoPanel;

	/// <summary>
			/// Updates the grid position and various colors.
			/// </summary>
	public void Update(IMyTextPanel lcd, TextPanelSetting setting)
	{
		Map.UpdateSetting(setting);
		InfoPanel.UpdateSetting(setting);
		world.UpdateGridPosition();
		colorManager.UpdateGeneralColors(lcd);
		colorManager.UpdateGridArrowColor(lcd, setting.GridColor);
	}

}

/// <summary>
		/// Common behavior to all terminals.
		/// </summary>
public abstract class Terminal<T, U>
	where T : class, IMyTerminalBlock
	where U : struct {

	protected Program program;
	private readonly List<T> terminals = new List<T>();
	private readonly List<U> settings = new List<U>();
	private int tIndex, tUpdate;

	public Terminal(Program program)
	{
		this.program = program;
	}

	public int UpdateCount { get; private set; }
	public MyIni MyIni { get; set; }

	public bool IsListEmpty => terminals.Count == 0;

	/// <summary>
			/// Runs through each terminal in list and attempts to update at the end of the cycle.
			/// Returns a booleans, which makes it possible to enumerate.
			/// </summary>
	public bool Run()
	{

		// Run a terminal, or update list and settings.
		if (tIndex < terminals.Count)
		{

			T terminal = terminals[tIndex];
			U setting = settings[tIndex];

			// Start the cycle or remove the corrupt terminal and setting.
			if (!IsCorrupt(terminal))
			{
				OnCycle(terminal, setting);
			}
			else
			{
				terminals.Remove(terminal);
				settings.Remove(setting);
			}

			tIndex++;

		}
		else
		{

			// Only update terminals and create settings / parse MyIni every ~10 seconds or so.
			if (tUpdate % (32 / (terminals.Count + 1)) == 0)
			{

				program.GridTerminalSystem.GetBlocksOfType(terminals, Collect);
				UpdateCount++;
				settings.Clear();

				foreach (T item in terminals)
				{
					U setting = CreateSetting(item);
					settings.Add(setting);
				}

			}

			tUpdate++;
			tIndex = 0;

		}

		return true;

	}

	/// <summary>
			/// The main method that cycles through terminals and their settings.
			/// </summary>
	public abstract void OnCycle(T terminal, U setting);

	/// <summary>
			/// Specificies which terminals to be collected. Use override.
			/// </summary>
	public virtual bool Collect(T terminal)
	{
		return terminal.IsSameConstructAs(program.Me);
	}

	/// <summary>
			/// For parsing terminal's customdata.
			/// </summary>
	public abstract U CreateSetting(T item);

	/// <summary>
			/// Checks if the terminal is null, gone from world, or broken off from grid.
			/// </summary>
	public bool IsCorrupt(T block)
	{
		if (block == null || block.WorldMatrix == MatrixD.Identity) return true;
		return !(program.GridTerminalSystem.GetBlockWithId(block.EntityId) == block);
	}


}

public class TerminalManager
{

	protected Program program;
	private readonly IEnumerator<bool> cycle;

	public TerminalManager(Program program)
	{
		this.program = program;
		ShipController = new ShipController(program);
		TextPanel = new TextPanel(program);
		//Antenna = new Antenna(program);
		cycle = SetCycle();
	}

	public ShipController ShipController { get; private set; }
	public TextPanel TextPanel { get; private set; }
	//public Antenna Antenna { get; private set; }

	public void Run()
	{
		if (!cycle.MoveNext())
			cycle.Dispose();
	}

	private IEnumerator<bool> SetCycle()
	{
		while (true)
		{
			yield return ShipController.Run();
			yield return TextPanel.Run();
			//yield return Antenna.Run();
		}
	}

}

public class TextPanel : Terminal<IMyTextPanel, TextPanelSetting>
{

	private readonly Vector2 centerScreen = new Vector2(512 / 2, 307.2f / 2 * 1.6f);
	public readonly SpriteManager spriteManager;
	private readonly List<MySprite> orbitSprites = new List<MySprite>();
	private readonly List<MySprite> planetSprites = new List<MySprite>();
	private readonly List<MySprite> infoSprites = new List<MySprite>();

	MySpriteDrawFrame frame;
	private IMyTextPanel lcd;
	private TextPanelSetting setting;
	private MySprite grid;
	private bool isOdd;

	public TextPanel(Program program) : base(program)
	{
		spriteManager = new SpriteManager(program.world);
	}

	public override void OnCycle(IMyTextPanel lcd, TextPanelSetting setting)
	{

		this.lcd = lcd;
		this.setting = setting;

		// Setting error.
		if (setting.HasError)
		{
			SetError(setting.Status.ToString());
			return;
		}

		// LCD dimension error.
		if (lcd.SurfaceSize.Y != 512)
		{
			SetError("Solar map cannot be displayed on 5:3 text panel.");
			return;
		}

		// Update propreties.
		spriteManager.Update(lcd, setting);

		// Paint.
		PaintMap();
		PaintInfoPanel();

		// Add paint. Must be added in the correct order.
		using (frame = lcd.DrawFrame())
		{

			// Used to force the texture surface cache to refresh.
			// See Whiplash141's original post: https://support.keenswh.com/spaceengineers/pc/topic/1-192-021-lcd-scripts-using-sprites-dont-work-in-mp
			// See Georgik's original code: https://discordapp.com/channels/125011928711036928/216219467959500800/721065427140083832
			if (isOdd)
				frame.Add(new MySprite());

			frame.AddRange(orbitSprites);
			frame.AddRange(planetSprites);
			frame.Add(grid);
			frame.AddRange(infoSprites);

		}

		// Force texture cache to change every odd and even interval.
		isOdd = !isOdd;

	}

	/// <summary>
			/// Sets up settings for each retrieved.
			/// </summary>
	public override TextPanelSetting CreateSetting(IMyTextPanel lcd)
	{

		MyIni = new MyIni();
		TextPanelSetting setting = new TextPanelSetting();

		// Try to parse CustomData.
		MyIniParseResult status;
		if (!MyIni.TryParse(lcd.CustomData, out status))
		{
			setting.Status = status;
			return setting;
		}

		// GridColor
		string gridColorHex = MyIni.Get("SolarMap", "GridColor").ToString();
		setting.GridColor = ColorExtensions.FromHtml(gridColorHex);

		// Hide Grid
		setting.HideGrid = MyIni.Get("SolarMap", "HideGrid").ToBoolean();

		// Hide Map
		setting.HideMap = MyIni.Get("SolarMap", "HideMap").ToBoolean();

		// Hide InfoPanel
		setting.HideInfo = MyIni.Get("SolarMap", "HideInfo").ToBoolean();

		// Offset X and Y axis.
		int offsetX = MyIni.Get("SolarMap", "OffsetX").ToInt32();
		int offsetY = MyIni.Get("SolarMap", "OffsetY").ToInt32();
		setting.Offset = new Vector2I(offsetX, offsetY);

		return setting;

	}

	public override bool Collect(IMyTextPanel terminal)
	{

		// To be collected.
		bool isSolarmap = terminal.IsSameConstructAs(program.Me) && MyIni.HasSection(terminal.CustomData, "SolarMap");

		// Set content type.
		if (isSolarmap)
		{
			terminal.ContentType = ContentType.SCRIPT;
			terminal.Script = ""; // Resets any mistakes that the user might have done.
		}

		return isSolarmap;

	}

	private void PaintMap()
	{

		// Hide.
		if (setting.HideMap)
		{
			orbitSprites.Clear();
			planetSprites.Clear();
			grid = new MySprite();
			return;
		}

		spriteManager.Map.UpdateMultipliers(lcd);
		spriteManager.Map.PaintOrbits(orbitSprites);
		spriteManager.Map.PaintPlanets(planetSprites);

		// Hide.
		if (setting.HideGrid)
		{
			grid = new MySprite();
			return;
		}

		grid = spriteManager.Map.PaintGrid(program.terminalManager.ShipController);

	}

	private void PaintInfoPanel()
	{

		// Hide.
		if (setting.HideInfo)
		{
			infoSprites.Clear();
			return;
		}

		spriteManager.InfoPanel.PaintInfo(infoSprites);

	}

	/// <summary>
			/// Sets error text and proceeds to dispose of the DrawFrame.
			/// </summary>
	private void SetError(string text)
	{
		frame = lcd.DrawFrame();
		frame.Add(new MySprite(SpriteType.TEXT, text, centerScreen, null, lcd.ScriptForegroundColor, null, rotation: 0.6f));
		frame.Dispose();
	}

}

public struct TextPanelSetting
{

	public MyIniParseResult Status { get; set; }
	public Color? GridColor { get; set; }
	public bool HideGrid { get; set; }
	public bool HideMap { get; set; }
	public bool HideInfo { get; set; }
	public Vector2I Offset { get; set; }

	public bool HasError => Status.IsDefined && !Status.Success;

}

public class World
{

	private const int SCALE = 2;

	protected readonly Program program;
	private readonly float radius;
	private readonly Vector2 maxOffset;

	public World(Program program)
	{

		this.program = program;

		CelestialMap = program.celestialBodies;
		CelestialInfo = new List<CelestialBody>(CelestialMap); // Copied due to sorting.

		// Setup map specfic properties.
		foreach (CelestialBody celestialBody in CelestialMap)
		{

			// Finds the farthest point from origo in the solar system, and scales it (used to create a sort of margin on the LCD).
			radius = celestialBody.Position.X > radius ? celestialBody.Position.X * SCALE : radius;
			radius = celestialBody.Position.Z > radius ? celestialBody.Position.Z * SCALE : radius;

			// Finds the farthest points from origo in the solar system, to center the map.
			maxOffset.X = celestialBody.Position.X > maxOffset.X ? celestialBody.Position.X : maxOffset.X;
			maxOffset.Y = celestialBody.Position.Z > maxOffset.Y ? celestialBody.Position.Z : maxOffset.Y;

		}

		// The map object needs to be sorted.
		CelestialMap.Sort(SortByDistance);

	}

	public List<CelestialBody> CelestialMap { get; }
	public List<CelestialBody> CelestialInfo { get; }
	public Vector3 GridPosition { get; private set; }

	/// <summary>
			/// Retrieve Vector2 equivalent in percent to be multipled to orbital position later.
			/// </summary>
	public Vector2 WorldToMapPercent(Vector3 position)
	{

		Vector2 mapCoordinates = new Vector2(2 * position.X + radius - maxOffset.X, 2 * position.Z + radius - maxOffset.Y);

		// Turns the mapCoordinates into a percentage between 0 and 1, and reverse so that the coordinates will be flipped on screen.
		Vector2 reversedPosition = Vector2.One - (mapCoordinates / (2 * radius));

		return reversedPosition;

	}

	/// <summary>
			/// Updates the position of the grid.
			/// </summary>
	public void UpdateGridPosition()
	{
		if (program.terminalManager.ShipController.HasController)
		{
			GridPosition = program.terminalManager.ShipController.Main.CenterOfMass;
		}
		else
		{
			GridPosition = program.Me.GetPosition();
		}
	}

	/// <summary>
			/// Used to sort celestial bodies by distance so that sprites will stack nicely.
			/// </summary>
	private int SortByDistance(CelestialBody a, CelestialBody b)
	{

		float distanceA = (new Vector2(a.Position.X, a.Position.Z) - Vector2.One).LengthSquared();
		float distanceB = (new Vector2(b.Position.X, b.Position.Z) - Vector2.One).LengthSquared();

		if (distanceA > distanceB)
			return -1;
		else if (distanceB > distanceA)
			return 1;
		return 0;

	}

	private int SortByType(CelestialBody a, CelestialBody b)
	{
		if (a.Type == CelestialType.Moon && b.Type == CelestialType.Planet)
			return -1;
		else if (b.Type == CelestialType.Moon && a.Type == CelestialType.Planet)
			return 1;
		return 0;
	}

}