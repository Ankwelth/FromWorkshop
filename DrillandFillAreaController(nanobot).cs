/*
 * DnF Area Controller v1.2.1
 * 
 * --------------- DESCRIPTION ----------------
 * 
 * Command Drill and Fill System area's position and size
 * By direct Command, Button Command Or Cockpit action
 * whith Custom value
 * 
 * -------------- CONFIGURATION ---------------
 * 
 * Configration by Programable Block's
 * Custom Data on INI Format
 * 
 * If Custom data is void or incomplete,
 * this script fill it with defeaut value
 * 
 * Section : [DAF-Controller]
 * DAF Controller Config
 * 
 * Key : Group_Name
 * Default : DAF-Controller
 * Group name including DAF Unit
 * 
 * Key : Master_Tag
 * Default : Master
 * Master DAF Unit Tag Name
 * 
 * Key : Master_Tag
 * Default : Master
 * Master DAF Unit Tag Name
 * 
 * Key : Position
 * Default : 0 0 0
 * Saved Position Format: X Y Z
 * 
 * Key : Size
 * Default : 25 25 25
 * Saved Size Format: X Y Z
 * 
 * -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
 * 
 * Section : [Screen-Text]
 * Log Screen config
 * 
 * Key : Name
 * Default : 
 * LOG SCREEN Block Name
 * if void BP screen used
 * 
 * Key : Index
 * Default : 0
 * LOG SCREEN Block Screen Index
 * Usefull for multi screen Block like Cockpit
 * 
 * -*-*-*-*-*-*-*-* Exemple *-*-*-*-*-*-*-*-
 * with default value:
 * 
 * [DAF-Controller]
 * Group_Name=DAF-Controller
 * Master_Tag=Master
 * Position=0 0 5
 * Size=25 25 25 
 * 
 * [Screen-Text]
 * Log_Screen_name=
 * Log_Screen_Index=0
 * 
 * ------------- COMMANDS MANUAL --------------
 * 
 * RESET  : Set Areo to 0 position on 3 axis
 * ALIGN  : Align all Unit's Area to Master Unit Area
 * RELOAD : Reload and recheck DAF Unit Cluster, UseFull if You extend or modify the Cluster
 * ON     : Turn ON  all DAF unit
 * OFF    : Turn OFF all DAF unit
 * SHOW   : Toogle Area Showing
 * 
 * OFFSET [arg1] [arg2] [arg3]: Define the position of DAF Area
 * SIZE   [arg1] [arg2] [arg3]: Define size of DAF Area
 * 
 * !! Important : Separate all argument with WhiteSpace !!
 * 
 * -*-*-*-*-*-*-*-*- USAGE -*-*-*-*-*-*-*-*-
 * 
 * Vector Style :
 * [Command] [x] [y] [z] argument : 
 * x y z must be number, define value x to AXIS X,  y to AXIS Y, z to AXIS Z
 * 
 * -+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-
 * Operator Style :
 * [Command] [Axis] [Operator] [value]
 * 
 * value must be a number
 * 
 * Axis : 
 * X   Left / Right
 * Y     Up / Down
 * Z  Front / Back
 * 
 * Operator :
 * = [value] set 
 * + [value] increment 
 * - [value] decrement
 */

const bool KEEP_INI_COMMENT = false;
const int MAX_INIT_TRY = 60;

const string PRONT = "DAF-Control:~$";

struct OP
{
	string op;
	DAF_Controler.AXIS axis;
	Single value;

	public OP(string o, DAF_Controler.AXIS a, Single v)
	{
		if (!"=+-".Contains(o)) throw new Exception("Invalid Operator : " + o);
		op = o;
		axis = a;
		value = v;
	}
	public OP(string o, string a, string v)
	{
		a = a.ToUpper();
		if (!Enum.TryParse<DAF_Controler.AXIS>(a, out axis)) throw new Exception("Invalid Axis " + a);
		if (!Single.TryParse(v, out value)) throw new Exception("Invalid Value " + v);
		if (!"=+-".Contains(o)) throw new Exception("Invalid Operator : " + o);
		op = o;
	}
	Single sign => (op == "-") ? -1 : 1;
	public string Op => op;
	public DAF_Controler.AXIS Axis => axis;
	public Single Value => Value;
	public Single SignedValue => sign * value;
}

static Program PBI = null;
static MyIni Ini = new MyIni();

ScreenText Screen;
DAF_Controler DAF = null;

int InitTry = 0;
bool IsLoad = false;

public Program()
{
    PBI = this;
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}
public void Save()
{
	if (!IsLoad) return;

	DAF_Controler.Save(DAF, Ini);

    if (IsLoad && !KEEP_INI_COMMENT)
    {
        var txt = Ini.ToString();
        var Lines = txt.Split('\n').ToList();
        Lines.RemoveAll(x => x.StartsWith(";"));
        txt = string.Join("\n", Lines);
        Me.CustomData = txt;
    }
	else Me.CustomData = Ini.ToString();
}
public void Main(string argument, UpdateType updateSource)
{
	if (!IsLoad && InitTry < MAX_INIT_TRY)
		INITsafe();

	if (!IsLoad)
		return;

	Command(new CommandLine(argument ?? ""));
	Save();
}

void INITsafe()
{
	Echo($"Try : {++InitTry} / {MAX_INIT_TRY}");
	try { INIT(); }
	catch (Exception e)
	{
		IsLoad = false;
		Echo($"FATAL > {e.Message}");
	}

	if (IsLoad)
	{
		Echo("INFO > LOADING OK :)");
		Runtime.UpdateFrequency = UpdateFrequency.None;
	}
	else
	{
		Echo("INFO > LOADING FAIL :( \n...trying again");
        Echo("Please Check Custom Data");
        Runtime.UpdateFrequency = UpdateFrequency.Update100;

        if (InitTry > MAX_INIT_TRY)
        {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            Echo("INFO > to Many Init Tentative...");
            Echo("INFO > Recompile to retry");
        }
    }
}
void INIT()
{
	IsLoad = false;
    Ini.Clear();
    if (!Ini.TryParse(Me.CustomData) && !string.IsNullOrWhiteSpace(Me.CustomData))
        Ini.EndContent = Me.CustomData;

	IsLoad = InitScreen() & InitControler();
	Save();
}
bool InitScreen()
{
	Screen = null;
    Echo($"INFO > Initialazing Screen...");
    try { Screen = ScreenText.Build(Ini); }
    catch
    {
        var ScrN = Ini.Get(ScreenText.INI_SECTION, ScreenText.INI_KEY_LogScreenName).ToString();
        var ScrI = Ini.Get(ScreenText.INI_SECTION, ScreenText.INI_KEY_LogScreenIndex).ToInt32();

        Echo($"WARN > Invalid Screen: {ScrN}:{ScrI} Defeaut used");
        Screen = new ScreenText(Me.GetSurface(0));
    }
    Screen.Font = "Monospace";
    Screen.Clear();
    return true;
}
bool InitControler()
{
	DAF = null;
    Echo($"INFO > Initialazing Controller...");

    try { DAF = DAF_Controler.Build(Ini); }
    catch (Exception e)
    {
        var _group = Ini.Get(DAF_Controler.INI_SECTION, DAF_Controler.INI_KEY_GroupName).ToString();

        Echo($"ERROR > Fail to Init Controler");
        Echo($"ERROR > {e.Message}");
        return false;
    }

    if (!DAF_Controler.Restore(DAF, Ini))
        Echo($"WARN > Fail to restore");

    if (DAF.Count > 0)
    {
        var group = Ini.Get(DAF_Controler.INI_SECTION, DAF_Controler.INI_KEY_GroupName).ToString();
        var drills = this.GTS().GetGroup(group).GetBlocks<IMyShipDrill>();

        Echo($"INFO > Check Orientation...");
        foreach (var d in drills)
        {
            if (DAF_Controler.IsValid(d) && !DAF.IsValidOrientation(d))
            {
                Echo($"WARN > DAF unit: {d.CustomName} Excluding Bad Orientation");
                d.Enabled = false;
            }
        }

        Echo($"INFO > Check Alignement...");
        if (!DAF.Align())
        {
            Echo($"ERROR > Fail to Align Drills");
            return false;
        }

    } else
		Echo($"WARNING > No DAF unit Found in Group");

    Echo($"INFO > DAF unit : {DAF.Count}");
    Echo($"INFO > Master   : {DAF.Master?.CustomName ?? "none"}");
    return true;
}

void Command(CommandLine cmd)
{

	cmd.Command = cmd.Command.ToUpper();

    Screen.Clear();
    Screen.Log( $"{PRONT} {cmd}");

	switch (cmd.Command)
	{
		case "" : break;
		case "RELOAD": INIT(); break;
		case "RESET": DAF.OFFSET = Vector3.Zero; break;
		case "ALIGN": DAF.Align(); break;
		case "ON": DAF.ForEach(x => x.Enabled = true);break;
		case "OFF" : DAF.ForEach(x => x.Enabled = false);break;
		case "SHOW" : DAF.ShowArea = !DAF.ShowArea; break;
		case "OFFSET": SET(cmd); break;
		case "SIZE": SET(cmd); break;

		default: Screen.LogErr($"Unknow command {cmd.Command} !"); break;
	}
	LogStatus();
}
void SET(CommandLine cmd)
{
	if (cmd.Args.Count < 3)
	{
        Screen.LogErr($"Argument Missing !");
		return;
	}

	Vector3 vec = new Vector3();
	if (TryParseV3(cmd.Args[0], cmd.Args[1], cmd.Args[2], ref vec))
	{
		if (cmd.Command == "OFFSET")
			DAF.OFFSET = vec;
		else
			DAF.SIZE = vec;
		return;
	}

	OP op;
	try { op = new OP(cmd.Args[1], cmd.Args[0], cmd.Args[2]); }
	catch (Exception e)
	{
        Screen.LogErr($"DAF-Control:~$ Parse ERROR {e.Message} !");
		return;
	}

	Action<DAF_Controler.AXIS, Single> _set;
	Func<DAF_Controler.AXIS, Single> _get;
	if (cmd.Command == "OFFSET")
	{
		_set = DAF.OffsetSet;
		_get = DAF.OffsetGet;
	} else
	{
		_set = DAF.SizeSet;
		_get = DAF.SizeGet;
	}

	float current = (op.Op == "=") ? 0 : _get(op.Axis);
	_set(op.Axis, current + op.SignedValue);
}

void LogStatus()
{
	if (Screen == null) return;

	Screen.Text.AppendLine( " --- STATUS ---");
	Screen.Text.AppendLine($"Enabled  : {DAF?.Master.Enabled ?? false}");
	Screen.Text.AppendLine($"DAF Unit : {DAF.Count}");
    Screen.Text.AppendLine($"Master   : {DAF.Master?.CustomName ?? "none"}");
    Screen.Text.AppendLine($"Position : {DAF.OFFSET}");
    Screen.Text.AppendLine($"Size     : {DAF.SIZE}");

    Screen.Update();
}
public static bool TryParseV3(string xyz, ref Vector3 vec)
{
	var XYZ = xyz.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
	if (XYZ.Length < 3) return false;
    return TryParseV3(XYZ[0], XYZ[1], XYZ[2], ref vec);
}
public static bool TryParseV3(string x, string y, string z, ref Vector3 vec)
{
	Single X, Y, Z;
	if (!(Single.TryParse(x, out X) && Single.TryParse(y, out Y) && Single.TryParse(z, out Z))) return false;
	vec = new Vector3(X, Y, Z);
	return true;
}

class CommandLine
{
	const char SEPARATOR = ' ';
	public string Command = "";
	public List<string> Args = new List<string>();

	public CommandLine(params string[] arg)
	{
		if (arg.Length == 0) return;

		Args = arg.ToList();
		Command = arg[0];
		Args.RemoveAt(0);
	}

	public CommandLine(string line) : this(line.Split(new char[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries)) { }

	public override string ToString() => $"{Command} {string.Join(SEPARATOR.ToString(), Args)}";
}

public interface IConfigData
{
	void Write(MyIni ini);
	void Read(MyIni ini);
	void Update(MyIni ini);
}

public abstract class ConfigData : IConfigData
{
	public const string NullString = "null";

	public string Section { get; set; }
	public string Key { get; set; }
	readonly string Comment;

	public ConfigData(string s, string k, string c = null)
	{
		Section = s;
		Key = k;
		Comment = c;
	}

	public void Write(MyIni ini)
	{
		ini.Set(Section, Key, ToString());

		if (!string.IsNullOrWhiteSpace(Comment))
			ini.SetComment(Section, Key, Comment);
	}
	public void Read(MyIni ini)
	{
		MyIniValue val = ini.Get(Section, Key);

		if (!val.IsEmpty) Set(ref val);
		else SetDefault();
	}
	public void Update(MyIni ini)
	{
		Read(ini);
		Write(ini);
	}

	abstract protected void Set(ref MyIniValue val);
	abstract protected void SetDefault();
}
public abstract class ConfigData<T> : ConfigData
{

	public T Value;
	protected T DefaultValue;

	protected ConfigData(string s, string k, string c) : base(s, k, c) { }
	public ConfigData(string section, string name, T defaultValue = default(T), string comment = null)
		: base(section, name, comment)
	{
		DefaultValue = defaultValue;
		SetDefault();
	}

	override protected void SetDefault() => Value = DefaultValue;

	abstract protected bool Parse(ref MyIniValue val);

	public override string ToString() => Value?.ToString() ?? NullString;

	public static implicit operator T(ConfigData<T> cfg) => cfg.Value;
}
public abstract class ConfigValue<T> : ConfigData<T> where T : struct
{
	protected ConfigValue(string s, string k, string c) : base(s, k, c) { }
	public ConfigValue(string section, string name, T value = default(T), string comment = null)
		: base(section, name, value, comment) { }

	override protected void Set(ref MyIniValue val)
	{
		if (!Parse(ref val))
			SetDefault();
	}
}
public abstract class ConfigNullable<T> : ConfigData<T>
{
	public ConfigNullable(string s, string k, T defV = default(T), string c = null)
		: base(s, k, defV, c) { }

	override protected void Set(ref MyIniValue val)
	{
		if (!Parse(ref val))
			SetDefault();
	}

	override protected bool Parse(ref MyIniValue val)
	{
		if (Parseable(ref val))
		{
			Value = ParseValue(ref val);
			return true;
		}
		if (val.ToString() == NullString)
		{
			Value = default(T);
			return true;
		}
		return false;
	}

	abstract protected bool Parseable(ref MyIniValue val);
	abstract protected T ParseValue(ref MyIniValue val);

}

public class ConfigString : ConfigNullable<string>
{
	public ConfigString(string section, string name, string value = "", string comment = null)
		: base(section, name, value, comment) { }

	override protected bool Parseable(ref MyIniValue val) => val.ToString(null) != null;
	override protected string ParseValue(ref MyIniValue val) => val.ToString();
}
public class ConfigInt : ConfigValue<int>
{
    public ConfigInt(string section, string name, int value = 0, string comment = null)
        : base(section, name, value, comment) { }

    protected override bool Parse(ref MyIniValue val) => val.TryGetInt32(out Value);
}

class DAF_Controler : List<IMyShipDrill>
{
	public const string
		INI_SECTION = "DAF-Controller",
		INI_KEY_GroupName = "Group_Name",
		INI_KEY_MstTag = "Master_Tag",
		INI_KEY_Position = "Position",
		INI_KEY_Size = "Size";

	const string
		INI_COM_GroupName = "\n Group name including DAF Unit",
		INI_COM_MstTag = "\n Master DAF Unit TAG Name",
		INI_COM_Position = "\n Saved Position\n Format: X Y Z",
		INI_COM_Size = "\n Saved Size\n Format: X Y Z";

	const string
		INI_DEF_GroupName = "DAF-Controller",
		INI_DEF_MstTag = "Master",
		INI_DEF_Position = "0 0 0",
		INI_DEF_Size = "75 75 75";

    const string DEFINITION_NAME = "DrillSystem";
	const string PROPERTY_PREFIX = "Drill.";

	const Base6Directions.DirectionFlags POSITIVE_DIRECTION =
	Base6Directions.DirectionFlags.Up |
	Base6Directions.DirectionFlags.Backward |
	Base6Directions.DirectionFlags.Right;

	static readonly string[] OffsetName = new string[] { "FrontBack", "LeftRight", "UpDown" },
							 SizeName = new string[] { "Depth", "Width", "Height" };

	public enum AXIS { Z = 0, X = 1, Y = 2 }

	string _mstTag = "Master";
	IMyShipDrill _master = null;
	public DAF_Controler(IEnumerable<IMyShipDrill> d = null) : base(d)
	{
		RemoveAll(x => !IsValid(x));
		_master = SearchMaster;
		RemoveAll(x => !IsValidOrientation(x));
	}

	public bool ShowArea
	{
		get
		{ return ShowArea = GetShowArea(Master); }
		set
		{ ForEach(x => SetShowArea(x, value)); }
	}
	public Vector3 OFFSET
	{
		get { return new Vector3(OffsetGet(Master, AXIS.X), OffsetGet(Master, AXIS.Y), OffsetGet(Master, AXIS.Z));}
		set
		{
			OffsetSet(AXIS.X, value.X);
			OffsetSet(AXIS.Y, value.Y);
			OffsetSet(AXIS.Z, value.Z);
		}
	}
	public Vector3 SIZE
	{
		get { return new Vector3(SizeGet(Master, AXIS.X), SizeGet(Master, AXIS.Y), SizeGet(Master, AXIS.Z)); }
		set
		{
			SizeSet(AXIS.X, value.X);
			SizeSet(AXIS.Y, value.Y);
			SizeSet(AXIS.Z, value.Z);
		}
	}

	public string MasterTag
	{
		get { return _mstTag; }
		set
		{
			_mstTag = value;
			_master = SearchMaster;
		}
	}

	public IMyShipDrill Master => (IsValid(_master)) ? _master : _master = SearchMaster;
	public IMyShipDrill SearchMaster
	{
		get
		{
			var res = FindAll(IsValid);
            return res.Find(IsMasterTagged) ?? res.FirstOrDefault();
		}
	}

	public new void Add(IMyShipDrill d) { if(IsAddable(d)) base.Add(d);}
	public new void AddRange(IEnumerable<IMyShipDrill> d) => base.AddRange(d.Where(IsAddable));

	public Single OffsetGet(AXIS a) => OffsetGet(Master, a);
	public void OffsetSet(AXIS a, Single v) { OffsetSet(Master, a, v); Align(a); }

	public bool Align() => Align(AXIS.X) & Align(AXIS.Y) & Align(AXIS.Z);
	bool Align(AXIS a)
	{
		var res = true;
		ForEach(d => res &= Align(d, a));
		return res;
	}
	bool Align(IMyShipDrill d, AXIS a)
	{
		var mst = Master;
		if (d == mst) return true;

		float
			off_mst = OffsetGet(mst, a),
			off_dif = OffsetDiff(d, a),
			off_set = off_mst + off_dif;

        OffsetSet(d, a, off_set);

		return OffsetGet(d, a) == off_set;
	}

	Single OffsetDiff(IMyShipDrill d, AXIS a)
		=> OffsetSign(a, Master) * OffsetSize(d) * (Master.Position - d.Position).AxisValue(B6Axis(a, d));

	public Single SizeGet(AXIS a) => SizeGet(Master, a);
	public void SizeSet(AXIS a, Single v) => ForEach(d => SizeSet(d, a, v));

	static Single OffsetGet(IMyShipDrill bar, AXIS axis) => bar.GetValue<Single>(OffsetPropName(axis));
	static void OffsetSet(IMyShipDrill d, AXIS a, Single v) => d.SetValue<Single>(OffsetPropName(a), v);

	static Single OffsetDiff(IMyShipDrill x, IMyShipDrill y, AXIS a)
		=> OffsetSign(a, x) * OffsetSize(x) * (x.Position - y.Position).AxisValue(B6Axis(a, y));

	static int OffsetSign(AXIS a, IMyShipDrill d) => ((a == AXIS.Z) ? -1 : 1) * OffsetSign(B6Direction(a, d));
	static int OffsetSign(Base6Directions.Direction dir) => OffsetSign(Base6Directions.GetDirectionFlag(dir));
	static int OffsetSign(Base6Directions.DirectionFlags dir) => (dir & POSITIVE_DIRECTION) > 0 ? 1 : -1;

	static float OffsetSize(IMyShipDrill d) => (d.CubeGrid.GridSizeEnum == MyCubeSize.Large) ? 2.5f : 0.5f;
	static string OffsetPrefix => PROPERTY_PREFIX + "AreaOffset";
	static string OffsetPropName(AXIS o) => OffsetPrefix + OffsetName[(int)o];

	static Single SizeGet(IMyShipDrill d, AXIS a) => d.GetValue<Single>(SizePropName(a));
	static void SizeSet(IMyShipDrill d, AXIS a, Single v) => d.SetValue<Single>(SizePropName(a), v);

	static string SizePrefix => PROPERTY_PREFIX + "Area";
	static string SizePropName(AXIS o) => SizePrefix + SizeName[(int)o];

    static bool GetShowArea(IMyShipDrill d) => d.GetValue<bool>($"{PROPERTY_PREFIX}ShowArea");
	static void SetShowArea(IMyShipDrill d, bool v) => d.SetValue<bool>($"{PROPERTY_PREFIX}ShowArea", v);

    public bool IsAddable(IMyShipDrill d) => IsValid(d) && !Contains(d) && IsValidOrientation(d);
	public bool IsMasterTagged(IMyShipDrill d) => d.CustomName.xContains(_mstTag);
	public bool IsValidOrientation(IMyShipDrill d) => (Master == null) ? true : d.Orientation == Master.Orientation;
	public static bool IsValid(IMyShipDrill d) => d != null && !d.Closed && d.DefinitionDisplayNameText == DEFINITION_NAME;

	static Base6Directions.Axis B6Axis(AXIS axis) => (Base6Directions.Axis)((int)axis);
	static Base6Directions.Axis B6Axis(AXIS axis, IMyCubeBlock cb) => B6Axis(axis, cb.Orientation);
	static Base6Directions.Axis B6Axis(AXIS axis, MyBlockOrientation MBO) => Base6Directions.GetAxis(B6Direction(axis, MBO));

	static Base6Directions.Direction B6Direction(AXIS axis, IMyCubeBlock cb) => B6Direction(axis, cb.Orientation);
	static Base6Directions.Direction B6Direction(AXIS axis, MyBlockOrientation MBO)
	{
		switch (axis)
		{
			case AXIS.Z: return MBO.Forward;
			case AXIS.X: return MBO.Left;
			case AXIS.Y: return MBO.Up;
			default : return 0;
		}
	}

    public static DAF_Controler Build(IMyBlockGroup g, string mstTag)
		=> new DAF_Controler(g.GetBlocks<IMyShipDrill>()) { _mstTag = mstTag };

	public static DAF_Controler Build(MyIni ini)
	{
		ConfigString
			grpNam = new ConfigString(INI_SECTION, INI_KEY_GroupName, INI_DEF_GroupName, INI_COM_GroupName),
			mstTag = new ConfigString(INI_SECTION, INI_KEY_MstTag, INI_DEF_MstTag, INI_COM_MstTag);


        grpNam.Update(ini);
		mstTag.Update(ini);

        return Build(PBI.GTS().GetGroup(grpNam), mstTag);

    }

	public static bool Restore(DAF_Controler daf, MyIni ini)
	{
		bool pPos, pSiz;
        Vector3 vPos = Vector3.Zero,
				vSiz = Vector3.Zero;

        ConfigString
			savPos = new ConfigString(INI_SECTION, INI_KEY_Position, INI_DEF_Position, INI_COM_Position),
			savSiz = new ConfigString(INI_SECTION, INI_KEY_Size, INI_DEF_Size, INI_COM_Size);

        savPos.Update(ini);
        savSiz.Update(ini);

		Vector3 vec = Vector3.Zero;

		if (pPos = TryParseV3(savPos.Value, ref vPos))
			daf.OFFSET = vPos;

		if (pSiz = TryParseV3(savSiz.Value, ref vSiz))
			daf.SIZE = vSiz;

		return pPos & pSiz;
    }
	public static void Save(DAF_Controler daf, MyIni ini)
	{
        ConfigString
           savPos = new ConfigString(INI_SECTION, INI_KEY_Position, INI_DEF_Position, INI_COM_Position),
           savSiz = new ConfigString(INI_SECTION, INI_KEY_Size, INI_DEF_Size, INI_COM_Size);

        savPos.Update(ini);
        savSiz.Update(ini);

        Vector3 pos = daf.OFFSET, siz = daf.SIZE;
		savPos.Value = $"{pos.X} {pos.Y} {pos.Z}";
		savSiz.Value = $"{siz.X} {siz.Y} {siz.Z}";

        savPos.Write(ini);
        savSiz.Write(ini);
    }
}

class ScreenText
{
	public const string
		INI_SECTION = "Screen-Text",
		INI_KEY_LogScreenName = "Name",
		INI_KEY_LogScreenIndex = "Index";

	public const string
		INI_COM_LogScreenName = "\n SCREEN Name",
		INI_COM_LogScreenIndex = "\n SCREEN Index";

	const string INI_DEF_LogScreenName = "";
	const int INI_DEF_LogScreenIndex = 0;

	IMyTextSurface _screen;
	public StringBuilder _text = new StringBuilder();

	public ScreenText(IMyTextSurface scr)
	{
		_screen = scr;
		_screen.ContentType = ContentType.TEXT_AND_IMAGE;
		_screen.Alignment = TextAlignment.LEFT;
		_screen.ReadText(_text);
	}

	public StringBuilder Text
	{
		get { return _text; }
		set { _text = value; Update();}
	}

	public string Font
	{
		get { return _screen.Font; }
		set { _screen.Font = value; }
	}
	public float FontSize
	{
		get { return _screen.FontSize; }
		set { _screen.FontSize = value; }
	}
	public Color FontColor
	{
		get {return _screen.FontColor; }
		set { _screen.FontColor = value; }
	}
	public Color BackgroundColor
	{
		get { return _screen.BackgroundColor; }
		set { _screen.BackgroundColor = value; }
	}

	public void Clear() { _text.Clear(); Update();}
	public void Log(string txt) { _text.AppendLine(txt); Update(); }
	public void LogWarn(string txt) => Log($"WARN : {txt}");
    public void LogErr(string txt) => Log($"ERROR : {txt}");
	public void LogErr(Exception e, bool trace = false)
	{
		LogErr(e.Message);
		if (!trace) return;

		Log("trace :");
		Log(e.StackTrace);
	}

	public void Update() { _screen.WriteText(_text);}

	public void SetScreen(IMyTextSurface scr, bool keepFont = false)
	{
		var old = _screen;
		_screen = scr;
		InitScreen();
		old.WriteText("");

		if (keepFont)
		{
			Font = old.Font;
			FontSize = old.FontSize;
			FontColor = old.FontColor;
			BackgroundColor = old.BackgroundColor;
		}
		Update();
	}

	void InitScreen()
	{
        _screen.ContentType = ContentType.TEXT_AND_IMAGE;
        _screen.Alignment = TextAlignment.LEFT;
        _screen.ReadText(_text);
    }

	public static ScreenText Build(string name, int index = 0)
		=> new ScreenText((string.IsNullOrEmpty(name)) ? PBI.Me.GetSurface(0) : PBI.GTS().GetScreen(name, index));
	public static ScreenText Build(MyIni ini)
	{
		ConfigString name = new ConfigString(INI_SECTION, INI_KEY_LogScreenName, INI_DEF_LogScreenName, INI_COM_LogScreenName);
		ConfigInt	index = new ConfigInt(INI_SECTION, INI_KEY_LogScreenIndex, INI_DEF_LogScreenIndex, INI_COM_LogScreenIndex);

        name.Update(Ini);
		index.Update(Ini);

		return Build(name, index);
    }
}

}
public delegate bool FILTER<T>(T b) where T : class, IMyTerminalBlock;
public delegate bool FILTER(IMyTerminalBlock b);

internal static partial class Extension
{
    public static List<IMyTerminalBlock> GetBlocks(this IMyBlockGroup g, FILTER f = null)
    => g.GetBlocks<IMyTerminalBlock>(ToFilter(f));
    public static List<T> GetBlocks<T>(this IMyBlockGroup g, FILTER<T> f = null) where T : class, IMyTerminalBlock
    {
        var res = new List<T>();
        g.GetBlocksOfType(res, ToFunc(f));
        return res;
    }

	public static IMyTerminalBlock GetBlock(this IMyGridTerminalSystem gts, long id)
	{
		IMyTerminalBlock b = gts.GetBlockWithId(id);

		if (b == null) throw new Exception($"Block id:{id} NOT FOUND");

		return b;
	}
	public static IMyTerminalBlock GetBlock(this IMyGridTerminalSystem gts, string name)
	{
		IMyTerminalBlock b = gts.GetBlockWithName(name);

		if (b == null) throw new Exception($"Block name:{name} NOT FOUND");

		return b;
	}

	public static List<IMyTerminalBlock> GetBlocks(this IMyGridTerminalSystem gts, FILTER f = null)
		=> gts.GetBlocks<IMyTerminalBlock>(ToFilter(f));
	public static List<T> GetBlocks<T>(this IMyGridTerminalSystem gts, FILTER<T> f = null) where T : class, IMyTerminalBlock
	{
		List<T> b = new List<T>();
		gts.GetBlocksOfType(b, ToFunc(f));
		return b;
	}

	public static IMyTextSurface GetScreen(this IMyGridTerminalSystem gts, long id, int i = 0)
		=> gts.GetScreen(gts.GetBlock(id), i);
	public static IMyTextSurface GetScreen(this IMyGridTerminalSystem gts, string n, int i = 0)
		=> gts.GetScreen(gts.GetBlock(n), i);
	public static IMyTextSurface GetScreen(this IMyGridTerminalSystem gts, IMyTerminalBlock b, int i = 0)
	{
		IMyTextSurfaceProvider screen = b as IMyTextSurfaceProvider;

		if (screen == null || screen.SurfaceCount == 0) throw new Exception($"Block {b.CustomName} have no screen");

		i = MathHelper.Clamp(i, 0, screen.SurfaceCount);
		return screen.GetSurface(i);
	}

	public static IMyBlockGroup GetGroup(this IMyGridTerminalSystem gts, string name)
	{
		IMyBlockGroup g = gts.GetBlockGroupWithName(name);
		if (g == null) throw new Exception($"BlocksGroup name:{name} NOT FOUND");
		return g;
	}

    public static T Peek<T>(this List<T> list)
    {
        if (list != null && list.Count > 0) return list[list.Count - 1];
        throw new ArgumentException("list");
    }
    public static void Push<T>(this List<T> list, T i) => list.Add(i);

    static FILTER<IMyTerminalBlock> ToFilter(FILTER f)
    {
        if (f == null) return null;
        return f.Invoke;
    }
    static Func<T, bool> ToFunc<T>(FILTER<T> f) where T : class, IMyTerminalBlock
    {
        if (f == null) return null;
        return f.Invoke;
    }

    public static IMyGridTerminalSystem GTS(this Program p) => p.GridTerminalSystem;

    public static bool xContains(this string str, string search, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        => str.IndexOf(search, comp) >= 0;

    public static bool IsBetwenn<T>(this T x, T min, T max) where T : IComparable<T>
		=> x.CompareTo(min) >= 0 && x.CompareTo(max) <= 0;
    public static T Clamp<T>(this T x, T min, T max) where T : IComparable<T>
    {
        if (min.CompareTo(max) > 0) Swap(ref min, ref max);

        if (x.CompareTo(min) < 0)   return min;
        if (x.CompareTo(max) > 0)   return max;
        return x;
    }
    public static void Swap<T>(ref T x, ref T y)
    {
        T z = x; x = y; y = z;
    }