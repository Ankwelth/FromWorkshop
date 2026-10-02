// ============================================================
// РАСЧЁТ ВРЕМЕНИ ДО СТОЛКНОВЕНИЯ + STOP DISTANCE (отдельный метод)
// ============================================================
IMyShipController _controller;
List<IMyCameraBlock> _fsCameras = new List<IMyCameraBlock>();
IMySoundBlock _siren;
IMyTextPanel _lcd;
List<IMyThrust> _thrusters = new List<IMyThrust>();
List<IMyGyro> _gyros = new List<IMyGyro>();
bool _autoOrient = false;  // Включить/выключить автоориентацию


IMyTextPanel _followTP;
string followTPName = "TP";

//Self Speed
Vector3D _targetVelocityVec = Vector3D.Zero;
bool _shouldMatchSpeed = false;
double _thrustSensitivity = 0.05;

//GPS
IMyRemoteControl _rc;
Vector3D _targetGPS = Vector3D.Zero;
bool _hasTarget = false;
string rcCustomName = "Дистанционное управление";


//GPS Points
Dictionary<string, Vector3D> _gpsPoints = new Dictionary<string, Vector3D>();
string _currentTargetName = "";

//msg controller
double _msgTimer = 0;
const double MSG_CLEAR = 10.0;

//TWR
double TWR;

bool _spaceMode;

double minAccel = -100.0;
double maxAccel = 100.0;

double _manualTime = 15.0;
const int SMOOTH_FRAMES = 6; //Количество копий acceleration(Чем больше тем медленнее реакция, но плавнее - BASICAL:6)
bool _uniAuto = true;  // false = MANUAL, true = UNIAUTO
double _safeLandingSpeed = 15.0;
bool safeLanding = false;

double zapasSD = 1000.0;


double _stopDistance = double.PositiveInfinity;
double _maxBrakeAccel = 0;
bool _cannotStop = false;


//xH pressets( Влияют на посадку, если агрессивный режим сирена при низких высотах орет много,
// 				стандарт орет Балансировано non agressive тише на низких высотах xc это базовый коефициент)
string _xhProfile = "s";

double _xa = 0.5;
double _xb = 0.75;
double _xc = 1.0;




// ============================================================
// UNI MULTIPLIER
// ============================================================
// const:1.0  = standard
// const:1.5  = +50% earlier
// const:2.0  = 2x earlier
// const:0.7  = 30% later
// ============================================================
double _uniMultiplier = 1.0; // 1.0 = стандарт; Константа к uniimpact
double _sirenMinHeight = 400.0;
double _sirenCooldown = 0.0;

bool _autoBrake = true;          //Autobrake включение
double _brakeDelayTimer = 0.0;    
const double BRAKE_DELAY = 1.0;   // задержка тормоза (Чтобы не срабатывал с сиреной, ибо каждый раз будет врубаться и мешать)
bool _debugMode = false;
double _massX = 1.0;
double _speedFactor = 1.0;
double xH = 1.0;

string _error = "None";

double _lastSpeed = 0;
string _message = "None";


//SPinner
string _spinner = "|";
string[] _spinnerFrames = new string[] { "|", "/", "-", "\\", "|", "/", "-", "\\" };
int _spinnerIndex = 0;
double _spinnerTimer = 0.0;
const double SPINNER_INTERVAL = 0.2; // 0.2 секунды на кадр (5 кадров/сек)

// Флаги для космо вывода
bool _showMass = false;
bool _showDampners = true;
bool _showFuel = true;
bool _showBattery = false; // только из группы Battery
bool _showReactors = false;

// Флаги для планетарного вывода
bool _showHeight = true;
bool _showDownSpeed = true;
bool _showAccel = true;
bool _showGravity = true;
bool _showMode = true;
bool _showImpactTime = true;
bool _showSiren = false;
bool _showStatus = true;
bool _showImpactMode = true;
bool _showUniImpact = true;
bool _showX = false;
bool _showAutoBrake = true;
bool _showLandingH = true;

//флаг имени
bool _showName = true;

//Sizes LCD
float _debugSize = 0.63f;
float _gravitySize = 0.73f;
float _spaceSize = 0.8f;


//Raycast for SPACE
long _ownGridId = 0;

double _raycastDistance = 3000; //Изменение значения ни на что не повлияет
int _raycastTimer = 0;
int _raycastInterval = 60; // 60 тиков = 1 Raycast/сек

long _lastTargetId = 0;
Vector3D _lastTargetPosition = Vector3D.Zero;
double _lastTargetDistance = 0;

MyDetectedEntityInfo _lastDetectedInfo;
int _targetLostTimer = 0;
const int TARGET_LOST_TICKS = 180; // 3 секунды
//AB S
Vector3D _sidePoint = Vector3D.Zero;
string _hitSide = ""; 
bool _flyActive = false;
string _flyThrustDirection = "";
Vector3D _flyTargetDirection;
double safetyMargin = 120;

//For Radar WMI(Func:Ray for Space)
enum TargetRelation : byte
{
    Neutral = 0,      // НЕ 1
    Other = 0,        // НЕ 128
    Enemy = 1,        // НЕ 4
    Friendly = 2,     // Совпадает
    Locked = 4,       // Флаг захвата
    LargeGrid = 8,    // Совпадает
    SmallGrid = 16,   // Совпадает
    Missile = 32,     // НЕ 64
    Asteroid = 64,    // НЕ 32
    
    RelationMask = Neutral | Enemy | Friendly,
    TypeMask = LargeGrid | SmallGrid | Other | Missile | Asteroid
}


// Переменные для сирены
double _sirenTimer = 0.0;
bool _sirenState = false;
bool _lastDanger = false;

double _currentHeight = 0;
Vector3D _currentVelocity = Vector3D.Zero;
Vector3D _currentGravity = Vector3D.Zero;
double _timeToImpact = 0;
double _downSpeed = 0;
double _downAccel = 0;
double _smoothedAccel = 0;

Vector3D _lastVelocity = Vector3D.Zero;
int _frameCount = 0;
Queue<double> _accelHistory = new Queue<double>();

Queue<double> _brakeHistory = new Queue<double>();

// ============================================================
// ДЛЯ STOP DISTANCE
// ============================================================
double _shipMass = 0;

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    FindBlocks();
	LoadSettings();
}

void LoadSettings()
{
    if (!string.IsNullOrEmpty(Storage))
    {
        // Разделяем на базовые настройки и флаги
        string[] parts = Storage.Split('|');
        if (parts.Length >= 1)
        {
            string[] baseParts = parts[0].Split(';');
            if (baseParts.Length == 9)
            {
                double.TryParse(baseParts[0], out _sirenMinHeight);
                double.TryParse(baseParts[1], out _uniMultiplier);
                double.TryParse(baseParts[2], out _massX);
                bool.TryParse(baseParts[3], out _autoBrake);
                bool.TryParse(baseParts[4], out _uniAuto);
                bool.TryParse(baseParts[5], out _debugMode);
                double.TryParse(baseParts[6], out _manualTime);
                double.TryParse(baseParts[7], out _speedFactor);
				double.TryParse(baseParts[8], out safetyMargin);
				
            }
        }
        
        // Загружаем флаги, если они есть
        if (parts.Length >= 2)
        {
            string[] flags = parts[1].Split(',');
            if (flags.Length >= 19) // Количество флагов
            {
                bool.TryParse(flags[0], out _showMass);
                bool.TryParse(flags[1], out _showDampners);
                bool.TryParse(flags[2], out _showFuel);
                bool.TryParse(flags[3], out _showBattery);
                bool.TryParse(flags[4], out _showReactors);
                bool.TryParse(flags[5], out _showHeight);
                bool.TryParse(flags[6], out _showDownSpeed);
                bool.TryParse(flags[7], out _showAccel);
                bool.TryParse(flags[8], out _showGravity);
                bool.TryParse(flags[9], out _showMode);
                bool.TryParse(flags[10], out _showImpactTime);
                bool.TryParse(flags[11], out _showSiren);
                bool.TryParse(flags[12], out _showStatus);
                bool.TryParse(flags[13], out _showImpactMode);
                bool.TryParse(flags[14], out _showUniImpact);
                bool.TryParse(flags[15], out _showX);
                bool.TryParse(flags[16], out _showAutoBrake);
                bool.TryParse(flags[17], out _showLandingH);
                bool.TryParse(flags[18], out _showName);
            }
        }
    }
}

public void Main(string arg, UpdateType updateSource)
{
	if (!string.IsNullOrWhiteSpace(arg))
{
    string cmd = arg.ToLower().Trim();
	
	//Debug mode <ITS NOT FOR USERS>
	if (cmd == "debug")
	{
		_debugMode = !_debugMode;
		Echo($"Debug mode: {(_debugMode ? "ON" : "OFF")}");
		SetMessage($"debug: Changed");
		return;
	}
	//xH changer change profile of xH where:
	//a = h < 2000 && v < 100, b = h < 4500 && v < 150, c = standart number. v - Velocity, h - current Height
	if (cmd.StartsWith("xh:"))
	{
		string profile = cmd.Substring(3).Trim().ToLower();
		if (profile == "s" || profile == "na" || profile == "a")
		{
			SetXHProfile(profile);
			SetMessage($"XH: {profile.ToUpper()}");
			Echo($"XH profile: {profile.ToUpper()}");
		}
		else
		{
			SetError($"Unknown profile: {profile}. Use: s (standart), na (non agressive), a (agressive)");
			Echo(_error);
		}
		return;
	}
	//TWR
	else if (cmd.StartsWith("twr:"))
	{
		// Формат: twr:11.77:U
		string[] parts = cmd.Substring(4).Trim().Split(':');
    
		if (parts.Length == 2)
		{
			double g;
			string dir = parts[1].ToUpper();
        
			if (double.TryParse(parts[0], out g) && g > 0)
			{
				ThrustToWeightRatioManual(g, dir);
			}
			else
			{
				SetError($"Invalid gravity: '{parts[0]}'. Use number like 9.81");
			}
		}
		else
		{
			SetError("Usage: twr:GRAVITY:DIR (e.g. twr:11.77:U)");
			Echo("DIR: U(up), D(down), L(left), R(right), F(forward), B(backward)");
		}
		return;
	}
	//GPS Saver
	else if (cmd == "save")
	{
		SavePoint("SavedPoint");
	}
	else if (cmd == "goto")
	{
		GoToPosition();
	}
	else if (cmd == "stop")
    {
        StopAutopilot();
    }
    else if (cmd == "del")
    {
        DeleteWaypoints();
    }
    else if (cmd == "gps")
    {
        ShowAutopilotStatus();
    }

	//its simple autobrake on your dampners "Z"
	if (cmd == "autobrake")
	{
		_autoBrake = !_autoBrake;
		SaveSettings();
		Echo($"Auto brake: {(_autoBrake ? "ON" : "OFF")}");
		SetMessage($"Auto brake: Changed");
		return;
	}
	// CA - применить настройки (mult:massx:speedfactor:landing)
	double mult, massx, speedfactor, landing;
	if (cmd.StartsWith("ca:"))
	{
		string args = cmd.Substring(3).Trim();
		string[] parts = args.Split(':');

		if (parts.Length == 4 &&
			double.TryParse(parts[0], out mult) &&
			double.TryParse(parts[1], out massx) &&
			double.TryParse(parts[2], out speedfactor) &&
			double.TryParse(parts[3], out landing))
		{
			_uniMultiplier = Math.Max(0.1, mult);
			_massX = Math.Max(0.1, massx);
			_speedFactor = Math.Max(0.1, speedfactor);
			_sirenMinHeight = Math.Max(0, landing);
			SaveSettings();
			Echo($"Applied: mult:{_uniMultiplier:F2}, massx:{_massX:F2}, speedfactor:{_speedFactor:F2}, landing:{_sirenMinHeight:F0}");
			SetMessage("CA: OK");
		}
		else
		{
			SetError("CA: Need 4 numbers: ca:mult:massx:speedfactor:landing");
			Echo(_error);
		}
		return;
	}
	//Landing ccchanger(Height Where is danger not work)
	if (cmd.StartsWith("landing:"))
	{
		string val = cmd.Substring(12).Trim();
		double newHeight;
		if (double.TryParse(val, out newHeight) && newHeight >= 0)
		{
			_sirenMinHeight = newHeight;
			SaveSettings();
			Echo($"LANDING HEIGHT: {_sirenMinHeight:F0} m");
			SetMessage($"Landing H: Changed");
		}
		else
		{
			SetError("Invalid height value. Use positive number.");
		}
		return;
	}
	//Mode Changer (MANUAL or UNIAUTO)
    if (cmd == "mode")
	{
		_uniAuto = !_uniAuto;
		SaveSettings();
		Echo($"Impact mode: {(_uniAuto ? "UNIAUTO" : "MANUAL")}");
		SetMessage($"Impact mode: Changed");
		return;
	}
	//ManualTime Changer
	if (cmd.StartsWith("mt:"))
	{
		string val = cmd.Substring(3).Trim();
		double newTime;
		if (double.TryParse(val, out newTime) && newTime > 0)
		{
			_manualTime = newTime;
			SaveSettings();
			Echo($"MANUAL TIME: {_manualTime:F1} s");
			SetMessage($"Manual time: {_manualTime:F1}s");
		}
		else
		{
			SetError($"Invalid time '{val}'. Use positive number.");
		}
		return;
	}
	//SafeDistanceSpace
	if (cmd.StartsWith("sds:"))
	{
		string val = cmd.Substring(4).Trim();
		if (double.TryParse(val, out safetyMargin) && safetyMargin > 0)
		{
			Echo($"SafetyDistanceSpace: {safetyMargin} m");
			SetMessage($"SafetyDistanceSpace: {safetyMargin} m(DEF:120)");
		}
		else
		{
			SetError($"Invalid num '{val}'. Use positive number.");
		}
		return;
	}
 	//Refresh blocks and groups
    if (cmd == "refresh")
    {
        FindBlocks();
        Echo("Blocks refreshed");
		SetMessage($"Blocks refreshed");
        return;
    }
	//Help Message
	if (cmd == "help" || cmd == "?")
	{
		SetMessage("=== FALLING STAR === | debug | autobrake | mode | refresh |! mult:1.2 | massx:1.5 | speedfactor:0.9 | landing:400 !| = |! ca:1.2:1.5:0.9:400 !| f:name | mt:15");
		Echo(_message);
		return;
	}
	//Speed Factor Changer
	if (cmd.StartsWith("speedfactor:"))
	{
		string val = cmd.Substring(12).Trim();
		double newFactor;
		if (double.TryParse(val, out newFactor) && newFactor > 0)
		{
			_speedFactor = newFactor;
			SaveSettings();
			Echo($"SPEED FACTOR: {_speedFactor:F2}");
			SetMessage($"Speed factor: {_speedFactor:F2}");
		}
		else
		{
			SetError($"Invalid speed factor '{val}'. Use positive number.");
		}
		return;
	}
	//MassX Changer
	if (cmd.StartsWith("massx:"))
	{
		string val = cmd.Substring(6).Trim();
		double newMassX;
		if (double.TryParse(val, out newMassX) && newMassX >= 0)
		{
			_massX = newMassX;
			SaveSettings();
			Echo($"MASS X: {_massX:F2}");
			SetMessage($"mass x:Changed");
		}
		else
		{
			SetError($"Invalid massX value '{val}'. Use positive number.");
		}
		return;
	}
	// STFU - заткнуть сирену на 10 секунд
	if (cmd == "stfu" || cmd == "silence")
	{
		StopSiren();
		_sirenCooldown = 15.0; // 15 секунд тишины
		_lastDanger = false;
		SetMessage("Siren silenced for 10s");
		Echo("Siren silenced for 10 seconds");
		return;
	}
	//Self Command
	else if (cmd == "self")
	{
		if (_controller == null)
		{
			SetMessage("[-] Контроллер не найден");
			return;
		}

		_targetVelocityVec =
			_controller.GetShipVelocities().LinearVelocity;

		_shouldMatchSpeed = true;

		_controller.DampenersOverride = true;

		SetMessage(
			$"[>] Self Speed: {_targetVelocityVec.Length():F1} м/с"
		);
	}
	else if (cmd == "clearself")
		ClearSelfSpeed();
	
    if (cmd.StartsWith("mult:"))
	{
		string val = cmd.Substring(5).Trim();
		double newMult;
		if (double.TryParse(val, out newMult) && newMult > 0)
		{
			_uniMultiplier = newMult;
			SaveSettings();
			Echo($"UNI MULT: {_uniMultiplier:F2}x");
			SetMessage($"UNI MULT: Changed");
		}
		else
		{
			SetError($"Invalid multiplier '{val}'. Use positive number.");
		}
		return;
	}
	if (cmd.StartsWith("f:"))
	{
		string param = cmd.Substring(2).Trim();
		string status = "";
    
		if (param == "mass") { _showMass = !_showMass; status = $"Mass: {(_showMass ? "ON" : "OFF")}"; }
		else if (param == "dampners") { _showDampners = !_showDampners; status = $"Dampners: {(_showDampners ? "ON" : "OFF")}"; }
		else if (param == "fuel") { _showFuel = !_showFuel; status = $"Fuel: {(_showFuel ? "ON" : "OFF")}"; }
		else if (param == "battery") { _showBattery = !_showBattery; status = $"Battery: {(_showBattery ? "ON" : "OFF")}"; }
		else if (param == "reactors") { _showReactors = !_showReactors; status = $"Reactors: {(_showReactors ? "ON" : "OFF")}"; }
		else if (param == "height") { _showHeight = !_showHeight; status = $"Height: {(_showHeight ? "ON" : "OFF")}"; }
		else if (param == "speed") { _showDownSpeed = !_showDownSpeed; status = $"Speed: {(_showDownSpeed ? "ON" : "OFF")}"; }
		else if (param == "accel") { _showAccel = !_showAccel; status = $"Accel: {(_showAccel ? "ON" : "OFF")}"; }
		else if (param == "gravity") { _showGravity = !_showGravity; status = $"Gravity: {(_showGravity ? "ON" : "OFF")}"; }
		else if (param == "mode") { _showMode = !_showMode; status = $"Mode: {(_showMode ? "ON" : "OFF")}"; }
		else if (param == "impacttime") { _showImpactTime = !_showImpactTime; status = $"Impact Time: {(_showImpactTime ? "ON" : "OFF")}"; }
		else if (param == "siren") { _showSiren = !_showSiren; status = $"Siren: {(_showSiren ? "ON" : "OFF")}"; }
		else if (param == "status") { _showStatus = !_showStatus; status = $"Status: {(_showStatus ? "ON" : "OFF")}"; }
		else if (param == "impactmode") { _showImpactMode = !_showImpactMode; status = $"Impact Mode: {(_showImpactMode ? "ON" : "OFF")}"; }
		else if (param == "uniimpact") { _showUniImpact = !_showUniImpact; status = $"Uni Impact: {(_showUniImpact ? "ON" : "OFF")}"; }
		else if (param == "mX") { _showX = !_showX; status = $"Uni Mult: {(_showX ? "ON" : "OFF")}"; }
		else if (param == "autobrake") { _showAutoBrake = !_showAutoBrake; status = $"Auto Brake: {(_showAutoBrake ? "ON" : "OFF")}"; }
		else if (param == "name") { _showName = !_showName; status = $"Name: {(_showName ? "ON" : "OFF")}"; }
		else if (param == "landing") { _showLandingH = !_showLandingH; status = $"Landing H: {(_showLandingH ? "ON" : "OFF")}"; }
		else { SetMessage($"Unknown parameter: {param}"); return; }
		SaveSettings();
		SetMessage(status);
		return;
	}
}
	if (_controller == null)
    {	
		SetError("No ship controller found");
        return;
    }
	    // ===== АВТОМАТИЧЕСКОЕ ОБНОВЛЕНИЕ =====
    UpdateAutopilot();
	UpdateMessages();
	SpeedMatcher();
	UpdateFollowingTP();

    // ============================================================
    // 1. СБОР ДАННЫХ
    // ============================================================

    _currentGravity = _controller.GetNaturalGravity();
    double grav = _currentGravity.Length();

    // Собираем двигатели и массу
    _shipMass = _controller.CalculateShipMass().PhysicalMass;
    _currentVelocity = _controller.GetShipVelocities().LinearVelocity;

    // Если есть гравитация - считаем время до столкновения
    if (grav >= 0.1)
    {
		_spaceMode = false;
        if (!_controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out _currentHeight))
        {
			SetError("ERROR: No planet elevation");
            if (_lcd != null) _lcd.WriteText("ERROR: No planet elevation");
            StopSiren();
            return;
        }

        Vector3D downDir = Vector3D.Normalize(_currentGravity);

        _downSpeed = Vector3D.Dot(_currentVelocity, downDir);
        if (_downSpeed < 0) _downSpeed = 0;
		safeLanding = _downSpeed <= _safeLandingSpeed && _currentHeight > 30;

        _frameCount++;

        if (_frameCount == 1)
        {
            _lastVelocity = _currentVelocity;
            _downAccel = 0;
            _smoothedAccel = 0;
            _accelHistory.Clear();
        }
        else
        {
            double currentDownSpeed = Vector3D.Dot(_currentVelocity, downDir);
            double lastDownSpeed = Vector3D.Dot(_lastVelocity, downDir);
			
			double deltaTime = (double)Runtime.TimeSinceLastRun.TotalSeconds;     
            if (deltaTime < 0.001) deltaTime = 0.001;
			double rawAccel = (currentDownSpeed - lastDownSpeed) / deltaTime;
            
            if (rawAccel > maxAccel) rawAccel = maxAccel;
			if (rawAccel < minAccel) rawAccel = minAccel;
            
            _downAccel = rawAccel;
            
            _accelHistory.Enqueue(rawAccel);
            if (_accelHistory.Count > SMOOTH_FRAMES)
                _accelHistory.Dequeue();
            
            double sum = 0;
            foreach (double val in _accelHistory)
                sum += val;
            _smoothedAccel = sum / _accelHistory.Count;
            
            _lastVelocity = _currentVelocity;
        }
		
        double h = _currentHeight;
        double v = _downSpeed;
        double g = grav;
        double a = _smoothedAccel;

        string mode = "";
        double timeToImpact = 0;
		double uniImpact = CalculateUniversalImpactTime();

        if (v <= 0.01)
        {
            timeToImpact = double.PositiveInfinity;
            mode = "NO DOWN SPEED";
        }
        else if (Math.Abs(a) < 0.5)
        {
            timeToImpact = h / v;
            mode = "FIXED SPEED";
        }
        else if (Math.Abs(a - g) < 0.5)  // a примерно равно g СВОБОДНОЕ ПАДЕНИЕ
        {
            double discriminant = v * v + 2 * g * h;
            if (discriminant < 0)
            {
                timeToImpact = double.PositiveInfinity;
            }
            else
            {
                timeToImpact = (Math.Sqrt(discriminant) - v) / g;
                if (timeToImpact < 0) timeToImpact = double.PositiveInfinity;
            }
            mode = "FREE FALL";
        }
        else if (a < -0.5)
        {
            timeToImpact = h / v;
            mode = "BRAKING";
        }
        else
        {
            double discriminant = v * v + 2 * a * h;
            if (discriminant < 0)
            {
                timeToImpact = double.PositiveInfinity;
            }
            else
            {
                timeToImpact = (Math.Sqrt(discriminant) - v) / a;
                if (timeToImpact < 0) timeToImpact = double.PositiveInfinity;
            }
            mode = "CHANGING";
        }

        _timeToImpact = timeToImpact;
		bool danger;
		if (_uniAuto)
		{
			danger = _timeToImpact < uniImpact && !double.IsInfinity(_timeToImpact);
		}
		else
		{
			danger = _timeToImpact < _manualTime && !double.IsInfinity(_timeToImpact);
		}
		string crashStatus = GetCrashStatus();
		bool sirenTrigger = crashStatus.Contains("(Red)") || crashStatus.Contains("(Orange)");
		
		bool sirenDanger = danger && !safeLanding;
		bool sirenCrash = sirenTrigger && !safeLanding;
        UpdateSiren(sirenDanger || sirenCrash);
		// ============================================================
        // АВТОТОРМОЖЕНИЕ
        // ============================================================

        if (_autoBrake && _controller != null && _currentHeight > _sirenMinHeight)
		{
			if (danger)
			{
				_autoOrient = true;
				AutoOrientToGravity();
				ClearSelfSpeed();
				_brakeDelayTimer += Runtime.TimeSinceLastRun.TotalSeconds;
				if (_brakeDelayTimer >= BRAKE_DELAY)
				{
					_controller.DampenersOverride = true;
					_controller.HandBrake = true;
				}
				
			}
			else
			{
				_brakeDelayTimer = 0.0;

				foreach (var gyro in _gyros)
				{
					gyro.GyroOverride = false;
					gyro.Pitch = 0;
					gyro.Yaw = 0;
					gyro.Roll = 0;
				}
			}
		}
		else
		{
			_brakeDelayTimer = 0.0;
			// Выключаем гироскопы если безопасно
			foreach (var gyro in _gyros)
				gyro.GyroOverride = false;
		}
		

        string timeStr = double.IsInfinity(_timeToImpact) ? "INF" : _timeToImpact.ToString("F1");
        string statusStr = danger ? "DANGER! PULL UP!" : "SAFE";
		if (danger == false)
			statusStr = GetCrashStatus();
		string modeName = _uniAuto ? "UNIAUTO" : "MANUAL";
		
		//Lcd Output Gravity
        string output = 
			"=========================================\n" +
			(_showName ? $"Falling STAR {_spinner}\n" : $"PLANE: (GRAVITY) {_spinner}\n") +
			(_showHeight ? $"HEIGHT:      {_currentHeight:F0} m\n" : "") +
			(_showDownSpeed ? $"DOWN SPEED:  {_downSpeed:F1} m/s\n" : "") +
			(_showAccel ? $"ACCEL:       {_smoothedAccel:F2} m/s²\n" : "") +
			(_showGravity ? $"GRAVITY:     {grav:F2} m/s²\n" : "") +
			(_showMode ? $"MODE:        {mode}\n" : "") +
			(_showImpactTime ? $"IMPACT TIME: {timeStr} s\n" : "") +
			(_showSiren ? $"SIREN:       {(_sirenState ? "ON" : "OFF")}\n" : "") +
			(_showStatus ? $"STATUS:      {statusStr}\n" : "") +
			(_showImpactMode ? $"IMPACT MODE: {modeName}\n" : "") +
			// ← ТОЛЬКО ОДНА СТРОКА! (в зависимости от режима)
			(_uniAuto ? 
				(_showUniImpact ? $"UNI IMPACT:  {(double.IsInfinity(uniImpact) ? "INF" : uniImpact.ToString("F1"))} s\n" : "") :
				$"MANUAL TIME: {_manualTime:F1} s\n") +
			(_showX ? $"UNI MULT:    {_uniMultiplier:F2}x | MASSX:{_massX}x\n" : "") +
			(_showAutoBrake ? $"AUTO BRAKE:  {(_autoBrake ? "ON" : "OFF")}\n" : "") +
			$"LANDING H:   {_sirenMinHeight:F0} m\n" +  // ← НОВАЯ СТРОКА!
			"=========================================";
			
		string outputD = 
			"=========================================\n" +
			$"HEIGHT:      {_currentHeight:F0} m\n" +
			$"DOWN SPEED:  {_downSpeed:F1} m/s\n" +
			$"RAW ACCEL:   {_downAccel:F2} m/s²\n" +
			$"SMOOTH ACCEL:{_smoothedAccel:F2} m/s²\n" +
			$"GRAVITY:     {grav:F2} m/s²\n" +
			$"FRAME:       {_frameCount}\n" +
			$"MODE:        {mode}\n" +
			$"IMPACT TIME: {timeStr} s\n" +
			$"SIREN:       {(_sirenState ? "ON" : "OFF")}\n" +
			$"STATUS:      {statusStr}{_spinner}\n" +
			$"IMPACT MODE: {modeName}\n" +
			// ← ТОЛЬКО ОДНА СТРОКА! (в зависимости от режима)
			(_uniAuto ? 
				$"UNI IMPACT:  {(double.IsInfinity(uniImpact) ? "INF" : uniImpact.ToString("F1"))} s\n" :
				$"MANUAL TIME: {_manualTime:F1} s\n") +
			$"UNI MULT:    {_uniMultiplier:F2}x | MASSX:{_massX}x\n" +
			$"AUTO BRAKE:  {(_autoBrake ? "ON" : "OFF")}\n" +
			(_showLandingH ? $"LANDING H:   {_sirenMinHeight:F0} m\n" : "") +
			"=========================================";
			
		//Вывод в ПБ gravity/Programmable Block Output gravity
		Echo($"Falling STAR:{_spinner}");
        Echo($"H:{_currentHeight:F0}m | V:{_downSpeed:F1}m/s | G:{grav:F2}м/с² ");
		Echo($"TimeToImpact:{timeStr}s | UNImpact:{uniImpact:F1}");
		Echo($"mult:{_uniMultiplier:F2} | massx:{_massX} | SF:{_speedFactor}");
		Echo($"SIREN:{(_sirenState ? "ON" : "OFF")}");
		Echo($"autobr:{(_autoBrake ? "ON" : "OFF")} | debug: {(_debugMode ? "ON" : "OFF")}");
		Echo($"Mode:{(_uniAuto ? "UNIAUTO" : "MANUAL")}");
		Echo($"Landing height: {_sirenMinHeight:F0} m");
		if (_debugMode)
		{
			Echo($"xH:A-{_xa}|B-{_xb}|C-{_xc}");
		}
		Echo($"m: {_message}");
		Echo($"Err:{_error}");
		//Lcd output debug or no =)
        if (_lcd != null)
        {
            _lcd.ContentType = ContentType.TEXT_AND_IMAGE;
            _lcd.Font = "Monospace";
			_lcd.FontColor = _sirenState ? Color.Red : Color.Green;
			if (_debugMode == false)
			{
				_lcd.FontSize = _gravitySize;
				_lcd.WriteText(output);
			}
            else
			{
				_lcd.FontSize = _debugSize;
				_lcd.WriteText(outputD);
			}
			UpdateSpinner();
        }
    }
    else
	{
    // ============================================================
    // КОСМОС
    // ============================================================
		_spaceMode = true;
		RaycastForward();
		double speed = _currentVelocity.Length();

    // ============================================================
    // РАСЧЁТ УСКОРЕНИЯ ПО ИЗМЕНЕНИЮ СКОРОСТИ (как на планете)
    // ============================================================
	
		//cosmo
		double deltaTime = (double)Runtime.TimeSinceLastRun.TotalSeconds;
		if (deltaTime < 0.001) deltaTime = 0.001;

		double rawAccel = (speed - _lastSpeed) / deltaTime;
		_lastSpeed = speed;
    
		if (rawAccel > maxAccel) rawAccel = maxAccel;
		if (rawAccel < minAccel) rawAccel = minAccel;
		
		_brakeHistory.Enqueue(rawAccel);
	if (_brakeHistory.Count > (SMOOTH_FRAMES))
		_brakeHistory.Dequeue();

	double sum = 0;
	foreach (double val in _brakeHistory)
		sum += val;
	double smoothedBrakeAccel = _brakeHistory.Count > 0 ? sum / _brakeHistory.Count : 0;

	double brakeAccel = smoothedBrakeAccel;
	if (Math.Abs(brakeAccel) < 0.40)
		brakeAccel = 0.00;

    // ============================================================
    // РАСЧЁТ ТОРМОЗНОГО ПУТИ (без вызова функции)
    // ============================================================
    
		Vector3D moveDir = Vector3D.Normalize(_currentVelocity);
		double totalThrust = 0;
		foreach (var t in _thrusters)
		{
			if (!t.IsWorking) continue;
			Vector3D thrustDir = t.WorldMatrix.Forward;
			double dot = Vector3D.Dot(thrustDir, moveDir);
			if (dot < 0) totalThrust += t.MaxEffectiveThrust * Math.Abs(dot);
		}
    
		double accel = totalThrust / Math.Max(1, _shipMass);
    
		double stopDist;
		if (accel > 0.01)
		{
			double stopTime = speed / accel;
			stopDist = speed * stopTime - 0.5 * accel * stopTime * stopTime;
		}
		else
		{
			stopDist = double.PositiveInfinity;
		}
    
		string distStr;
		if (double.IsInfinity(stopDist) || stopDist > 1000000)
			distStr = "NO BRAKES";
		else if (stopDist >= 1000)
			distStr = (stopDist / 1000).ToString("F2") + " km";
		else
			distStr = stopDist.ToString("F0") + " m";
		
		double reactionTime = 2.0;
		double reactionDistance = speed * reactionTime;
		double totalStopDistance = reactionDistance + stopDist;
		
		double currentAccel = brakeAccel;
		
		
		if (_lastTargetId != 0 && !double.IsInfinity(totalStopDistance) && _lastTargetDistance < (totalStopDistance + safetyMargin))
		{	
			UpdateSiren(true);
			if (_autoBrake == true)
			{
				ClearSelfSpeed();
				FlyOnVector();	
			}
		}
		else
		{
			if (_autoBrake == true)
				StopOrrient();
			UpdateSiren(false);
		}
		//_message =
		//    "=== HIT ANALYSIS ===\n" +
		//    "ID: " + _lastTargetId + "\n" +
		//    "Entity Pos: " + _lastTargetPosition + "\n" +
		//    "Dist: " + _lastTargetDistance + "\n" +
		//	"Stop: " + (stopDist + safetyMargin) + "\n";

    // ============================================================
    // ВЫВОД В ПБ
    // ============================================================
    
		Echo($"Falling STAR:{_spinner}");
		Echo($"SPEED:{speed:F1}m/s");
		Echo($"MASS:{_shipMass/1000.0:F1}t");
		Echo($"STOP DIST:{distStr}");
		Echo($"ACCEL:{brakeAccel:F2} m/s²");
		Echo($"DAMPNERS:{(_controller.DampenersOverride ? "ACTIVE" : "DRIFT")}");
		Echo($"m: {_message}");
		Echo($"Err:{_error}");

    // ============================================================
    // ВЫВОД НА LCD (из функции CalculateStopDistance)
    // ============================================================
    
		if (_lcd != null)
		{
			_lcd.ContentType = ContentType.TEXT_AND_IMAGE;
			_lcd.FontSize = _spaceSize;
			_lcd.Font = "Monospace";
			_lcd.FontColor = _sirenState ? new Color(255, 100, 0) : new Color(0, 95, 255);
			_lcd.WriteText(CalculateStopDistance(brakeAccel));
		}
    
		UpdateSpinner();
		}
}

// ============================================================
// ОТДЕЛЬНЫЙ МЕТОД ДЛЯ STOP DISTANCE
// ============================================================

string CalculateStopDistance(double brakeAccel)
{
    Vector3D worldVelocity = _currentVelocity;
    double speed = worldVelocity.Length();
    string flightMode = _controller.DampenersOverride ? "ACTIVE" : "DRIFT";
    
    double fuel = GetFuelPercent();
    string batteryInfo = GetBatteryInfo();
    
    int reactors = GetReactorCount();
    int workingReactors = GetWorkingReactorCount();
    double reactorPower = GetReactorPowerMW();
    
    // Переменные для форматирования
    string distStr = "0 m";
    string timeStr = "0.0 s";
	
    if (speed < 0.5)
    {
        return 
            "=========================================\n" +
			(_showName ? $"(SPACE) Falling STAR {_spinner}\n" : $"NO GRAVITY (SPACE) {_spinner}\n") +
            $"SPEED:         0.0 m/s\n" +
            $"STOP DIST:     0 m\n" +
            $"STOP TIME:     0.0 s\n" +
			$"ACCEL:         {brakeAccel:F2} m/s²\n" +
            (_showMass ? $"MASS:          {_shipMass/1000.0:F1} t\n" : "") +
            (_showDampners ? $"DAMPNERS:      {flightMode}\n" : "") +
			(_showAutoBrake ? $"AUTO BRAKE:    {(_autoBrake ? "ON" : "OFF")}\n" : "") +
            (_showFuel ? $"FUEL (H):      {GetFuelPercent():F0}%\n" : "") +
            (_showBattery ? $"BATTERY:       {batteryInfo}\n" : "") +
            (_showReactors ? $"REACTORS:      {workingReactors}/{reactors} ({reactorPower:F1}MW)\n" : "") +
            "=========================================";
    }
    
    Vector3D moveDir = Vector3D.Normalize(worldVelocity);
    
    double totalThrust = 0;
    foreach (var t in _thrusters)
    {
        if (!t.IsWorking) continue;
        
        Vector3D thrustDir = t.WorldMatrix.Forward;
        double dot = Vector3D.Dot(thrustDir, moveDir);
        if (dot < 0)
        {
            totalThrust += t.MaxEffectiveThrust * Math.Abs(dot);
        }
    }
    
    double acceleration = totalThrust / Math.Max(1, _shipMass);
	
	double maxTotalThrust = 0;
	foreach (var t in _thrusters)
	{
		if (t.IsWorking)
			maxTotalThrust += t.MaxEffectiveThrust;
	}
    
    double stopDistance, stopTime;
    
    if (acceleration > 0.01)
    {
        stopTime = speed / acceleration;
        stopDistance = speed * stopTime - 0.5 * acceleration * stopTime * stopTime;
    }
    else
    {
        stopTime = double.PositiveInfinity;
        stopDistance = double.PositiveInfinity;
    }
    
    // Форматируем вывод
    if (double.IsInfinity(stopDistance) || stopDistance > 1000000)
    {
        distStr = "NO BRAKES";
    }
    else if (stopDistance >= 1000)
    {
        distStr = (stopDistance / 1000).ToString("F2") + " km";
    }
    else
    {
        distStr = stopDistance.ToString("F0") + " m";
    }
    
    timeStr = double.IsInfinity(stopTime) ? "INF" : stopTime.ToString("F1");
    
    return 
        "=========================================\n" +
        $"(SPACE) Falling STAR {_spinner}\n" +
        $"SPEED:         {speed:F1} m/s\n" +
        $"STOP DIST:     {distStr}\n" +
        $"STOP TIME:     {timeStr} s\n" +
		$"ACCEL:         {brakeAccel:F2} m/s²\n" +
        (_showMass ? $"MASS:          {_shipMass/1000.0:F1} t\n" : "") +
        (_showDampners ? $"DAMPNERS:      {flightMode}\n" : "") +
		(_showAutoBrake ? $"AUTO BRAKE:    {(_autoBrake ? "ON" : "OFF")}\n" : "") +
        (_showFuel ? $"FUEL (H):      {GetFuelPercent():F0}%\n" : "") +
        (_showBattery ? $"BATTERY:       {batteryInfo}\n" : "") +
        (_showReactors ? $"REACTORS:      {workingReactors}/{reactors} ({reactorPower:F1}MW)\n" : "") +
        "=========================================";
}

// ============================================================
// МЕТОДЫ ДЛЯ СИРЕНЫ
// ============================================================

void UpdateSiren(bool danger)
{
	if (_sirenCooldown > 0)
    {
        _sirenCooldown -= Runtime.TimeSinceLastRun.TotalSeconds;
        if (_sirenCooldown < 0) _sirenCooldown = 0;
        StopSiren();
        _lastDanger = false;
        return;
    }
    // Проверка высоты ТОЛЬКО на планете (НЕ в космосе)
    if (!_spaceMode) // Если НЕ в космосе (на планете)
    {
        if (_currentHeight < _sirenMinHeight && _currentHeight > 0)
        {
            StopSiren();
            _lastDanger = false;
            return;
        }
    }
    // Если в космосе (_spaceMode == true) - пропускаем проверку высоты

    if (!danger)
    {
        StopSiren();
        _lastDanger = false;
        return;
    }

    if (!_lastDanger)
    {
        _sirenTimer = 0.0;
        _sirenState = true;
        if (_siren != null) _siren.Play();
        _lastDanger = true;
        return;
    }

    _sirenTimer += Runtime.TimeSinceLastRun.TotalSeconds;
    if (_sirenTimer >= 0.190)
    {
        _sirenTimer -= 0.190;
        _sirenState = !_sirenState;
        if (_siren != null)
        {
            if (_sirenState) _siren.Play();
            else _siren.Stop();
        }
    }
}

void StopSiren()
{
    _sirenTimer = 0.0;
    _sirenState = false;
    if (_siren != null) _siren.Stop();
}

// ============================================================
// ПОИСК БЛОКОВ
// ============================================================

void FindBlocks()
{
    // ===== КОНТРОЛЛЕР =====
    List<IMyShipController> controllers = new List<IMyShipController>();
    GridTerminalSystem.GetBlocksOfType(controllers, b => b.IsSameConstructAs(Me));
    _controller = controllers.Count > 0 ? controllers[0] : null;
    
    // ===== ДВИГАТЕЛИ =====
    _thrusters.Clear();
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(
        _thrusters,
        t => t.IsSameConstructAs(Me)
    );
	//Камеры
	IMyBlockGroup camGroup =
        GridTerminalSystem.GetBlockGroupWithName("CAM(FS)");
	if (camGroup != null)
    {
        camGroup.GetBlocksOfType<IMyCameraBlock>(_fsCameras);

        foreach (var cam in _fsCameras)
        {
            cam.EnableRaycast = true;
        }

        Echo("CAM(FS) found: " + _fsCameras.Count);
    }
    else
    {
        SetError("Group 'CAM(FS)' not found!");
    }
	// ===== ГИРОСКОПЫ =====
	_gyros.Clear();
	GridTerminalSystem.GetBlocksOfType<IMyGyro>(
		_gyros,
		g => g.IsSameConstructAs(Me) && g.IsWorking
	);

    // ===== СИРЕНА =====
    List<IMySoundBlock> sounds = new List<IMySoundBlock>();
    GridTerminalSystem.GetBlocksOfType(sounds, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("SIREN"));
    _siren = sounds.Count > 0 ? sounds[0] : null;

    // ===== LCD =====
    List<IMyTextPanel> panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, b => b.IsSameConstructAs(Me) && b.CustomName.Contains("FallingStar"));
    _lcd = panels.Count > 0 ? panels[0] : null;
	
	// ===== FOLLOWING TP =====
	List<IMyTextPanel> followPanels = new List<IMyTextPanel>();

	GridTerminalSystem.GetBlocksOfType(
		followPanels,
		b => b.IsSameConstructAs(Me) &&
			 b.CustomName.Contains(followTPName)
	);

	_followTP = followPanels.Count > 0 ? followPanels[0] : null;

    // ===== REMOTE CONTROL только с именем "Remote Control" =====
    List<IMyRemoteControl> rcs = new List<IMyRemoteControl>();
	GridTerminalSystem.GetBlocksOfType(rcs, b => b.IsSameConstructAs(Me) && (b.CustomName.Contains("Remote Control") || b.CustomName.Contains(rcCustomName)));
	_rc = rcs.Count > 0 ? rcs[0] : null;
	if (_rc != null)
		_ownGridId = _rc.CubeGrid.EntityId;

    // ===== ПРОВЕРКИ ОШИБОК =====
    if (_siren == null)
        SetError(_error + "\nWARNING: No sound block with 'SIREN' in name found");
    
    if (_lcd == null)
        SetError(_error + "\nWARNING: No LCD with 'FallingStar' in name found");
    
    if (_rc == null)
        SetError(_error + "\nWARNING: No Remote Control found with name 'Remote Control'");
}




double CalculateUniversalImpactTime()
{
	
    if (_currentGravity.Length() < 0.1)
        return double.PositiveInfinity;
    
    double grav = _currentGravity.Length();
    double v = _downSpeed;
    double h = _currentHeight;
    
    double massTons = _shipMass / 1000.0;
    double massFactor = _massX + Math.Log10(1 + massTons / 100.0) * 5.0;
    
    // ============================================================
    // РАСЧЁТ ТОРМОЗНОГО ПУТИ (СОХРАНЯЕМ В ГЛОБАЛЬНЫЕ ПЕРЕМЕННЫЕ)
    // ============================================================
    Vector3D gravityDir = Vector3D.Normalize(_currentGravity);  // КУДА ПАДАЕМ
    double totalUpThrust = 0;
    foreach (var t in _thrusters)
    {
        if (!t.IsWorking) continue;
        double dot = Vector3D.Dot(t.WorldMatrix.Forward, -gravityDir); // Смотрим ПРОТИВ гравитации
        if (dot > 0.3)
            totalUpThrust += t.MaxEffectiveThrust * dot;
    }
    
    _maxBrakeAccel = (totalUpThrust / Math.Max(1, _shipMass)) - grav;
    _cannotStop = _maxBrakeAccel <= 0.05 && v > 20.0;
    
    _stopDistance = double.PositiveInfinity;
    if (_maxBrakeAccel > 0.1 && v > 0.1)
    {
        _stopDistance = (v * v) / (2 * _maxBrakeAccel);
    }
    
    // ============================================================
    // БАЗОВЫЙ РАСЧЁТ UNI
    // ============================================================
	if (h < 2000 && v < 100)
		xH = _xa;
	else if (h < 4500 && v < 150)
		xH = _xb;
	else
		xH = _xc;
    double baseUni = 3.0 + grav * 0.7 + (v / 30) * _speedFactor - (h / (1000 * xH)) + massFactor;
    
    // Коррекция на тормозной путь
    if (_stopDistance > (h - zapasSD) && v > 5)
    {
        double brakeRatio = _stopDistance / Math.Max(h - zapasSD, 1);
		baseUni *= (1 + Math.Min(brakeRatio, 3.0) * 0.5);
    }
    
    double uniImpact = baseUni * _uniMultiplier;
    
    if (uniImpact < 2.0) uniImpact = 2.0;
    if (uniImpact > 60.0) uniImpact = 60.0;
    
    return uniImpact;
}



// ============================================================
// ПОЛУЧЕНИЕ ТОПЛИВА (ТОЛЬКО ВОДОРОД)
// ============================================================

double GetFuelPercent()
{
    List<IMyGasTank> tanks = new List<IMyGasTank>();
    GridTerminalSystem.GetBlocksOfType(tanks, t => t.IsSameConstructAs(Me) && t.BlockDefinition.SubtypeName.Contains("Hydrogen"));
    
    if (tanks.Count == 0)
        return 0;
    
    double totalCapacity = 0;
    double totalFilled = 0;
    
    foreach (var tank in tanks)
    {
        totalCapacity += tank.Capacity;
        totalFilled += tank.FilledRatio * tank.Capacity;
    }
    
    return totalCapacity > 0 ? (totalFilled / totalCapacity) * 100 : 0;
}


// ============================================================
// БАТАРЕИ (количество + процент заряда)
// ============================================================

string GetBatteryInfo()
{
    // Ищем группу "Battery"
    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName("Battery");
    
    if (group == null)
        return "N/A (no group)";
    
    // Получаем все блоки из группы
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    group.GetBlocks(blocks);
    
    // Фильтруем только батареи
    List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
    foreach (var block in blocks)
    {
        IMyBatteryBlock battery = block as IMyBatteryBlock;
        if (battery != null)
            batteries.Add(battery);
    }
    
    if (batteries.Count == 0)
        return "N/A (0)";
    
    int working = 0;
    double totalStored = 0;
    double totalMax = 0;
    
    foreach (var b in batteries)
    {
        if (b.IsWorking) working++;
        totalStored += b.CurrentStoredPower;
        totalMax += b.MaxStoredPower;
    }
    
    double percent = totalMax > 0 ? (totalStored / totalMax) * 100 : 0;
    
    return $"{percent:F0}% ({working}/{batteries.Count})";
}
// ============================================================
// РЕАКТОРЫ (Big + Small)
// ============================================================

int GetReactorCount()
{
    List<IMyReactor> reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(reactors, r => r.IsSameConstructAs(Me));
    
    return reactors.Count;
}

// ============================================================
// МОЩНОСТЬ РЕАКТОРОВ (МВт)
// ============================================================

double GetReactorPowerMW()
{
    List<IMyReactor> reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(reactors, r => r.IsSameConstructAs(Me) && r.IsWorking);
    
    double totalPower = 0;
    foreach (var r in reactors)
    {
        totalPower += r.CurrentOutput;
    }
    
    return totalPower / 1000000.0; // Переводим в МВт
}


// ============================================================
// РАБОТАЮЩИЕ РЕАКТОРЫ
// ============================================================

int GetWorkingReactorCount()
{
    List<IMyReactor> reactors = new List<IMyReactor>();
    GridTerminalSystem.GetBlocksOfType(reactors, r => r.IsSameConstructAs(Me) && r.IsWorking);
    
    return reactors.Count;
}


void UpdateSpinner()
{
    _spinnerTimer += Runtime.TimeSinceLastRun.TotalSeconds;
    
    if (_spinnerTimer >= SPINNER_INTERVAL)
    {
        _spinnerTimer = 0.0;
        _spinnerIndex = (_spinnerIndex + 1) % _spinnerFrames.Length;
        _spinner = _spinnerFrames[_spinnerIndex];
    }
}

// ============================================================
// РАСЧЁТ НЕМИНУЕМОГО ПАДЕНИЯ
// ============================================================

string GetCrashStatus()
{
    if (_currentGravity.Length() < 0.1)
        return "SAFE (SPACE)";

    // Если дампферы выключены — не паникуем, просто предупреждаем
    if (!_controller.DampenersOverride)
    {
        return "(Yellow) DAMPERS OFF!";
    }

    bool willCrash = (_stopDistance + zapasSD) > _currentHeight && _downSpeed > 3;
    bool lowAlt = _currentHeight < 100 && _downSpeed > 20;
    bool critical = _currentHeight < 30 && _downSpeed > 10;

    if (critical)
        return "(Red) CRITICAL!";
    else if (_cannotStop)
        return "(Red) CAN'T STOP!";
    else if (willCrash)
        return "(Orange) WILL CRASH!";
    else if (lowAlt)
        return "(Yellow) LOW ALT!";
    else
        return "SAFE";
}



void SaveSettings()
{
    // Основные настройки
    string baseSettings = $"{_sirenMinHeight};{_uniMultiplier};{_massX};{_autoBrake};{_uniAuto};{_debugMode};{_manualTime};{_speedFactor};{safetyMargin}";
    
    // Флаги LCD (превращаем в строку)
    string flags = $"{_showMass},{_showDampners},{_showFuel},{_showBattery},{_showReactors}," +
                   $"{_showHeight},{_showDownSpeed},{_showAccel},{_showGravity},{_showMode}," +
                   $"{_showImpactTime},{_showSiren},{_showStatus},{_showImpactMode},{_showUniImpact}," +
                   $"{_showX},{_showAutoBrake},{_showLandingH},{_showName}";
    
    Storage = baseSettings + "|" + flags;
}


void SetXHProfile(string profile)
{
    _xhProfile = profile;
    switch (profile)
    {
        case "a":   // agressive
            _xa = 0.75;
            _xb = 0.90;
            _xc = 1.0;
            break;
        case "na":  // non agressive
            _xa = 0.25;
            _xb = 0.50;
            _xc = 1.0;
            break;
        default:    // standart
            _xa = 0.5;
            _xb = 0.75;
            _xc = 1.0;
            break;
    }
    Echo($"XH Profile: {profile.ToUpper()} (xa:{_xa}, xb:{_xb}, xc:{_xc})");
}

// ========== АВТОПИЛОТ ==========


void StopAutopilot()
{
    if (_rc == null)
    {
        SetMessage("[-] RC не найден");
        return;
    }
    _rc.SetAutoPilotEnabled(false);
    SetMessage("[X] Автопилот остановлен");
}


void ShowAutopilotStatus()
{
    string msg = "\n=== АВТОПИЛОТ ===\n";
    msg += $"RC: {(_rc != null ? "[+]" : "[-]")}\n";
    msg += $"Точка: {(_hasTarget ? "[+]" : "[-]")}\n";
    if (_hasTarget)
    {
        msg += $"X: {_targetGPS.X:F0} Y: {_targetGPS.Y:F0} Z: {_targetGPS.Z:F0}\n";
        if (_rc != null)
        {
            double dist = Vector3D.Distance(_rc.GetPosition(), _targetGPS);
            msg += $"Дистанция: {dist:F0} м\n";
        }
    }
    msg += $"Автопилот: {(_rc != null && _rc.IsAutoPilotEnabled ? "ВКЛ" : "ВЫКЛ")}";
    SetMessage(msg); // ← Один вызов, таймер сброшен
}

// ========== GPS POINTS ==========











void UpdateAutopilot()
{
    // Автоматическое отключение при достижении цели
    if (_hasTarget && _rc != null && _rc.IsAutoPilotEnabled)
    {
        double dist = Vector3D.Distance(_rc.GetPosition(), _targetGPS);
        if (dist < 5)
        {
            _rc.SetAutoPilotEnabled(false);
            SetMessage("[!] ЦЕЛЬ ДОСТИГНУТА!");
        }
    }
}

double GetDistanceToTarget()
{
    if (_rc == null || !_hasTarget) return -1;
    return Vector3D.Distance(_rc.GetPosition(), _targetGPS);
}



//message cleaner
void UpdateMessages()
{
    if (_message != "None" || _error != "None")
    {
        _msgTimer += Runtime.TimeSinceLastRun.TotalSeconds;
        if (_msgTimer >= MSG_CLEAR)
        {
            _message = "None";
            _error = "None";
            _msgTimer = 0;
        }
    }
    else _msgTimer = 0;
}

void SetMessage(string msg)
{
    _message = msg;
    _msgTimer = 0;
}

void SetError(string err)
{
    _error = err;
    _msgTimer = 0;
}



void SpeedMatcher()
{
    if (!_shouldMatchSpeed)
    {
        foreach (var t in _thrusters)
            t.ThrustOverridePercentage = 0f;
        return;
    }

    if (_controller == null) return;

    Vector3D myVelocity = _controller.GetShipVelocities().LinearVelocity;
    Vector3D relativeVelocity = myVelocity - _targetVelocityVec;

    // Получаем направление ввода от игрока (WASD)
    Vector3D inputVec = _controller.MoveIndicator;
    Vector3D desiredDirectionVec = Vector3D.TransformNormal(inputVec, _controller.WorldMatrix);

    // Применяем тягу
    ApplyThrust(_thrusters, relativeVelocity, desiredDirectionVec, _controller);
}



// ========== SELF SPEED THRUST ==========

void ApplyThrust(List<IMyThrust> thrusters, Vector3D travelVec, Vector3D desiredDirectionVec, IMyShipController controller)
{
    if (controller == null || thrusters == null || thrusters.Count == 0)
        return;

    double mass = controller.CalculateShipMass().PhysicalMass;
    Vector3D gravity = controller.GetNaturalGravity();

    // Требуемая тяга: 2 * ошибка скорости + компенсация гравитации
    Vector3D desiredThrust = mass * (2.0 * travelVec + gravity);
    Vector3D thrustToApply = desiredThrust;

    // Если игрок что-то нажимает (WASD) — убираем компоненту в этом направлении
    if (!Vector3D.IsZero(desiredDirectionVec))
    {
        thrustToApply = VectorRejection(desiredThrust, desiredDirectionVec);
    }

    foreach (var t in thrusters)
    {
        if (!t.IsWorking)
        {
            t.ThrustOverridePercentage = 0f;
            continue;
        }

        // Если двигатель направлен в сторону ввода игрока — включаем на 100%
        if (Vector3D.Dot(t.WorldMatrix.Backward, desiredDirectionVec) > 0.7071)
        {
            t.ThrustOverridePercentage = 1f;
        }
        // Если дампферы включены и двигатель может помочь — распределяем тягу
        else if (controller.DampenersOverride && Vector3D.Dot(t.WorldMatrix.Forward, thrustToApply) > 0)
        {
            double neededThrust = Vector3D.Dot(thrustToApply, t.WorldMatrix.Forward);
            double outputProportion = MathHelper.Clamp(neededThrust / t.MaxEffectiveThrust, 0, 1);
            t.ThrustOverridePercentage = (float)outputProportion;
            thrustToApply -= t.WorldMatrix.Forward * outputProportion * t.MaxEffectiveThrust;
        }
        else
        {
            t.ThrustOverridePercentage = 0.000001f; // Почти 0, но не 0 (чтобы не отключались)
        }
    }
}

// Вспомогательная функция — убирает проекцию вектора a на вектор b
Vector3D VectorRejection(Vector3D a, Vector3D b)
{
    if (Vector3D.IsZero(b))
        return a;
    return a - Vector3D.Dot(a, b) / Vector3D.Dot(b, b) * b;
}



void UpdateFollowingTP()
{
    if (_followTP == null || _controller == null)
        return;

    _followTP.ContentType = ContentType.TEXT_AND_IMAGE;
    _followTP.Font = "Monospace";
    _followTP.FontSize = 0.85f;

    // SELF выключен
    if (!_shouldMatchSpeed)
    {
        _followTP.FontColor = Color.Gray;

        _followTP.WriteText(
            "=========================================\n" +
            "             FOLLOWING / SELF\n" +
            "=========================================\n" +
            "STATUS:       OFF\n" +
            "\n" +
            "Use SELF to capture\n" +
            "current velocity.\n" +
            "\n" +
            "CLEARSELF - stop matching\n" +
            "========================================="
        );

        return;
    }

    Vector3D currentVelocity =
        _controller.GetShipVelocities().LinearVelocity;

    Vector3D relativeVelocity =
        currentVelocity - _targetVelocityVec;

    double targetSpeed = _targetVelocityVec.Length();
    double currentSpeed = currentVelocity.Length();
    double error = relativeVelocity.Length();

    double match = 100.0;

    if (targetSpeed > 0.01)
    {
        match = 100.0 - (error / targetSpeed * 100.0);
        match = MathHelper.Clamp(match, 0.0, 100.0);
    }

    string status;

    if (error < 0.5)
        status = "LOCKED";
    else if (error < 2.0)
        status = "MATCHING";
    else
        status = "CORRECTING";

    string thrustStatus =
        error < 0.5 ? "HOLD" : "ACTIVE";

    _followTP.FontColor =
        error < 0.5 ? Color.Green :
        error < 2.0 ? Color.Yellow :
        Color.Red;

    string output =
        "=========================================\n" +
        "             FOLLOWING / SELF\n" +
        "=========================================\n" +
        $"STATUS:       {status}\n" +
        "\n" +
        $"TARGET:       {targetSpeed:F1} m/s\n" +
        $"CURRENT:      {currentSpeed:F1} m/s\n" +
        $"ERROR:        {error:F2} m/s\n" +
        $"MATCH:        {match:F1}%\n" +
        "\n" +
        $"THRUST:       {thrustStatus}\n" +
        "=========================================";

    _followTP.WriteText(output);
}


// ========== SELF SPEED CONTROL ==========

void ClearSelfSpeed()
{
    _shouldMatchSpeed = false;
    _targetVelocityVec = Vector3D.Zero;

    foreach (var thruster in _thrusters)
        thruster.ThrustOverridePercentage = 0f;

    SetMessage("[ ] Self Speed очищена");
}


void SavePoint(string name)
{
    if (_rc == null)
    {
        SetMessage("[-] RC не найден");
        return;
    }
    
    Vector3D pos = _rc.GetPosition();
    _gpsPoints[name] = pos;
    _hasTarget = true;
    _targetGPS = pos;
    _currentTargetName = name;
    
    _rc.AddWaypoint(pos, name);  // ← ТОЧКА ДОБАВЛЯЕТСЯ В RC
    
    SetMessage($"[+] Точка '{name}' сохранена: {pos.X:F0} {pos.Y:F0} {pos.Z:F0}");
}

void GoToPosition()
{
    if (_rc == null)
    {
        SetMessage("[-] RC не найден");
        return;
    }
    
    if (!_hasTarget)
    {
        SetMessage("[-] Нет сохранённой точки! Сначала используйте 'save'");
        return;
    }
    
    // Просто включаем автопилот к уже существующей точке
    _rc.SetAutoPilotEnabled(true);
    
    double dist = Vector3D.Distance(_rc.GetPosition(), _targetGPS);
    SetMessage($"[>] Летим к {_currentTargetName}, дистанция: {dist:F0} м");
}



void DeleteWaypoints()
{
    if (_rc == null)
    {
        SetMessage("[-] RC не найден");
        return;
    }
	
    
    // Очищаем все точки в Remote Control
    _rc.ClearWaypoints();
    
    // Очищаем словарь точек в скрипте
    _gpsPoints.Clear();
    
    // Сбрасываем текущую цель
    _hasTarget = false;
    _targetGPS = Vector3D.Zero;
    _currentTargetName = "";
    
    SetMessage("[-] Все точки удалены");
}


void ThrustToWeightRatioManual(double g, string direction)
{
    if (_controller == null)
    {
        SetMessage("[-] Контроллер не найден");
        return;
    }
    
    // Определяем вектор направления
    Vector3D dirVec = Vector3D.Zero;
    string dirName = "";
    switch (direction)
    {
        case "U": dirVec = _controller.WorldMatrix.Up; dirName = "UP"; break;
        case "D": dirVec = -_controller.WorldMatrix.Up; dirName = "DOWN"; break;
        case "L": dirVec = -_controller.WorldMatrix.Right; dirName = "LEFT"; break;
        case "R": dirVec = _controller.WorldMatrix.Right; dirName = "RIGHT"; break;
        case "F": dirVec = _controller.WorldMatrix.Forward; dirName = "FORWARD"; break;
        case "B": dirVec = -_controller.WorldMatrix.Forward; dirName = "BACKWARD"; break;
        default:
            SetError($"Unknown direction: {direction}. Use U,D,L,R,F,B");
            return;
    }
    
    // Собираем двигатели по направлению
    double totalThrust = 0;
    int thrustCount = 0;
    
    foreach (var t in _thrusters)
    {
        if (!t.IsWorking) continue;
        
        // Проверяем, смотрит ли двигатель в нужном направлении
        double dot = Vector3D.Dot(t.WorldMatrix.Forward, dirVec);
        if (dot > 0.3)  // > 0.3 = достаточно близко к направлению
        {
            totalThrust += t.MaxEffectiveThrust * dot;
            thrustCount++;
        }
    }
    
    if (thrustCount == 0)
    {
        SetMessage($"[-] Нет двигателей в направлении {dirName}!");
        return;
    }
    
    // Считаем массу
    double mass = _controller.CalculateShipMass().PhysicalMass;
    
    // TWR = тяга / (масса * гравитация)
    double TWR = totalThrust / (mass * g);
    
    // Оценка
    string status;
    if (TWR <= 0.8)
        status = "(NO!!)";
    else if (TWR <= 1.0)
        status = "(HARD!)";
    else if (TWR <= 1.2)
        status = "(MED)";
    else
        status = "(EASY)";
    
    // Вывод
    string msg = $"[\nTWR:{TWR:F2} | Thr:{totalThrust/1000000:F2} MN\n{status}]";
    
    Echo(msg);
    SetMessage(msg);
}



Dictionary<string, double> GetThrustByDirection()
{
    var result = new Dictionary<string, double>
    {
        {"Up", 0}, {"Down", 0}, {"Left", 0}, {"Right", 0}, {"Forward", 0}, {"Backward", 0}
    };
    
    if (_controller == null) return result;
    
    MatrixD world = _controller.WorldMatrix;
    
    foreach (var t in _thrusters)
    {
        if (!t.IsWorking) continue;
        
        Vector3D thrustDir = t.WorldMatrix.Forward;
        
        // Проверяем, в какую сторону дует двигатель
        double up = Vector3D.Dot(thrustDir, world.Up);
        double right = Vector3D.Dot(thrustDir, world.Right);
        double forward = Vector3D.Dot(thrustDir, world.Forward);
        
        // Находим доминирующее направление
        double max = Math.Max(Math.Abs(up), Math.Max(Math.Abs(right), Math.Abs(forward)));
        
        if (Math.Abs(up) == max)
            result[up > 0 ? "Up" : "Down"] += t.MaxEffectiveThrust;
        else if (Math.Abs(right) == max)
            result[right > 0 ? "Right" : "Left"] += t.MaxEffectiveThrust;
        else if (Math.Abs(forward) == max)
            result[forward > 0 ? "Forward" : "Backward"] += t.MaxEffectiveThrust;
    }
    
    return result;
}


void AutoOrientToGravity()
{
    if (_controller == null || _gyros.Count == 0) return;
    if (!_autoOrient) return;

    Vector3D gravity = _controller.GetNaturalGravity();
    if (gravity.LengthSquared() < 0.01) return;

    // Направление падения (вниз)
    Vector3D downDir = Vector3D.Normalize(gravity);

    // Текущая скорость
    Vector3D velocity = _currentVelocity;

    // Если скорость слишком маленькая — не подруливаем
    if (velocity.LengthSquared() < 0.01) return;

    // Направление, куда мы реально летим (вниз)
    Vector3D velocityDir = Vector3D.Normalize(velocity);

    // Проверяем, смотрит ли скорость вниз
    double dot = Vector3D.Dot(velocityDir, downDir);

    // Если скорость уже почти вниз — выключаем
    if (dot > 0.999)
    {
        foreach (var gyro in _gyros)
        {
            gyro.GyroOverride = false;
            gyro.Pitch = 0;
            gyro.Yaw = 0;
            gyro.Roll = 0;
        }
        return;
    }

    // Находим, куда должны смотреть двигатели (против скорости)
    Vector3D targetDir = -velocityDir;
    targetDir.Normalize();

    // Находим самую мощную сторону двигателей
    var thrusts = GetThrustByDirection();
    string bestDir = "";
    double maxThrust = 0;

    foreach (var kvp in thrusts)
    {
        if (kvp.Value > maxThrust)
        {
            maxThrust = kvp.Value;
            bestDir = kvp.Key;
        }
    }

    if (maxThrust < 0.1) return;

    // Направление максимальной тяги в мировых координатах
    Vector3D currentDir;
    switch (bestDir)
    {
        case "Up": currentDir = _controller.WorldMatrix.Up; break;
        case "Down": currentDir = -_controller.WorldMatrix.Up; break;
        case "Right": currentDir = _controller.WorldMatrix.Right; break;
        case "Left": currentDir = -_controller.WorldMatrix.Right; break;
        case "Forward": currentDir = _controller.WorldMatrix.Forward; break;
        case "Backward": currentDir = -_controller.WorldMatrix.Forward; break;
        default: return;
    }

    currentDir.Normalize();

    // Проверяем, уже смотрит куда надо
    double currentDot = Vector3D.Dot(currentDir, targetDir);
    if (currentDot > 0.999)
    {
        foreach (var gyro in _gyros)
        {
            gyro.GyroOverride = false;
            gyro.Pitch = 0;
            gyro.Yaw = 0;
            gyro.Roll = 0;
        }
        return;
    }

    // Ось вращения
    Vector3D axis = Vector3D.Cross(currentDir, targetDir);
    double axisLength = axis.Length();

    // Если направления противоположны (180°)
    if (axisLength < 0.0001)
    {
        axis = Vector3D.Cross(currentDir, _controller.WorldMatrix.Up);
        if (axis.LengthSquared() < 0.0001)
        {
            axis = Vector3D.Cross(currentDir, _controller.WorldMatrix.Right);
        }
        axis.Normalize();
    }
    else
    {
        axis /= axisLength;
    }

    // Угол между направлениями
    double angle = Math.Acos(MathHelper.Clamp(currentDot, -1.0, 1.0));

    // Подруливание: чем больше угол, тем быстрее вращаем
    // Ошибка в процентах от максимальной ошибки (90°)
	double error = angle / (Math.PI / 2.0);
	error = MathHelper.Clamp(error, 0.0, 1.0);

	// Максимальная скорость гироскопа
	double maxAngularSpeed = 0.4;

	// Пропорциональное подруливание
	double angularSpeed = error * maxAngularSpeed;

    // Мировой вектор угловой скорости
    Vector3D worldAngularVelocity = axis * angularSpeed;

    // Каждый гироскоп получает свою локальную команду
    foreach (var gyro in _gyros)
    {
        MatrixD worldToGyro = MatrixD.Transpose(gyro.WorldMatrix);
        Vector3D localAngularVelocity = Vector3D.TransformNormal(worldAngularVelocity, worldToGyro);

        gyro.GyroOverride = true;
        gyro.Pitch = (float)localAngularVelocity.X;
        gyro.Yaw = (float)localAngularVelocity.Y;
        gyro.Roll = (float)localAngularVelocity.Z;
    }
}

//RAYCAST VECTOR MOVING(OBJECTS)


long RaycastForward()
{
	
	// ===== ПРОВЕРКА: ЕСТЬ ЛИ RC =====
    if (_rc == null)
    {
        Echo("Raycast: RC is null!");
        return 0;
    }

    // ===== ПРОВЕРКА: РАБОТАЕТ ЛИ RC =====
    if (!_rc.IsWorking)
    {
        Echo("Raycast: RC not working!");
        return 0;
    }

    _raycastTimer++;

    if (_raycastTimer < _raycastInterval)
        return _lastTargetId;

    _raycastTimer = 0;

    // ===== ПРОВЕРКА: ЕСТЬ ЛИ КАМЕРЫ =====
    if (_fsCameras == null || _fsCameras.Count == 0)
    {
        //SetError($"\nWarning: No cameras found!");
        return 0;
    }

	double speed = _rc.GetShipVelocities().LinearVelocity.Length();
	if (speed <= 7.0)
	{
		_raycastDistance = 150;
		_raycastInterval = 60;
	}
	else
	{
		_raycastDistance = speed * 20.0;
		_raycastDistance = MathHelper.Clamp(_raycastDistance, 3000.0, 8000.0);

    _raycastInterval = (int)(6000.0 / speed);
    _raycastInterval = MathHelper.Clamp(_raycastInterval, 10, 60);
	}
    if (_fsCameras.Count == 0)
        return _lastTargetId;

    // Направление движения
    Vector3D forwardDir =
        GetMovementDirection();

    if (Vector3D.IsZero(forwardDir))
        return _lastTargetId;


    foreach (var cam in _fsCameras)
    {
        if (!cam.IsWorking || cam.Closed)
            continue;

        if (!cam.CanScan(_raycastDistance))
            continue;


        Vector3D camPos =
            cam.GetPosition();

        MatrixD camMatrix =
            cam.WorldMatrix;


        // Направление движения
        // Преобразуем в локальные координаты камеры

        Vector3D localDir =
            Vector3D.TransformNormal(
                -forwardDir,
                MatrixD.Transpose(camMatrix)
            );


        float pitch =
            (float)(
                Math.Atan2(
                    -localDir.Y,
                    localDir.Z
                )
                * 180.0 / Math.PI
            );


        float yaw =
			(float)(
				Math.Atan2(
					-localDir.X,
					localDir.Z
				)
				* 180.0 / Math.PI
			);


        MyDetectedEntityInfo detected =
            cam.Raycast(
                _raycastDistance,
                pitch,
                yaw
            );


        if (!detected.IsEmpty() && detected.EntityId != 0 && detected.EntityId != _ownGridId)
		{
			_lastTargetId = detected.EntityId;
			_lastTargetPosition = detected.Position;
			_lastDetectedInfo = detected;
			_targetLostTimer = 0;

			Vector3D hitPos = detected.HitPosition ?? detected.Position;
			_lastTargetDistance = Vector3D.Distance(camPos, hitPos);
			//AnalyzeHit(detected);
			
			SendTargetToIGC();
			return _lastTargetId;
		}
    }
	_targetLostTimer += _raycastInterval;
	if (_targetLostTimer >= TARGET_LOST_TICKS)
    {
        _lastTargetId = 0;
        _lastTargetDistance = 0;
        _targetLostTimer = 0;
    }
	return _lastTargetId; // Возвращаем старую цель, если таймер не истёк
}


// ============================================================
// НАПРАВЛЕНИЕ ДВИЖЕНИЯ
// ============================================================

Vector3D GetMovementDirection()
{
    if (_rc == null ||
        !_rc.IsWorking)
        return Vector3D.Zero;


    Vector3D velocity =
        _rc.GetShipVelocities().LinearVelocity;


    // Если почти стоим

    if (velocity.LengthSquared() < 1.0)
        return Vector3D.Zero;


    // Только направление движения

    return Vector3D.Normalize(velocity);
}


// ============================================================
// Анализ границ
// ============================================================


void AnalyzeHit(MyDetectedEntityInfo detected)
{
    Vector3D hit;

    if (detected.HitPosition.HasValue)
        hit = detected.HitPosition.Value;
    else
        hit = detected.Position;

    Vector3D min = detected.BoundingBox.Min;
    Vector3D max = detected.BoundingBox.Max;

    double distUp = Math.Abs(max.Y - hit.Y);
    double distDown = Math.Abs(hit.Y - min.Y);

    double distRight = Math.Abs(max.X - hit.X);
    double distLeft = Math.Abs(hit.X - min.X);

    double distFront = Math.Abs(max.Z - hit.Z);
    double distBack = Math.Abs(hit.Z - min.Z);

    double closest = distUp;
    string side = "U";
    Vector3D sidePoint = new Vector3D(hit.X, max.Y, hit.Z); // Точка на верхней грани

    if (distDown < closest)
    {
        closest = distDown;
        side = "D";
        sidePoint = new Vector3D(hit.X, min.Y, hit.Z);
    }

    if (distRight < closest)
    {
        closest = distRight;
        side = "R";
        sidePoint = new Vector3D(max.X, hit.Y, hit.Z);
    }

    if (distLeft < closest)
    {
        closest = distLeft;
        side = "L";
        sidePoint = new Vector3D(min.X, hit.Y, hit.Z);
    }

    if (distFront < closest)
    {
        closest = distFront;
        side = "F";
        sidePoint = new Vector3D(hit.X, hit.Y, max.Z);
    }

    if (distBack < closest)
    {
        closest = distBack;
        side = "B";
        sidePoint = new Vector3D(hit.X, hit.Y, min.Z);
    }

    // Сохраняем точку выхода с этой стороны
    _hitSide = side;
    _sidePoint = sidePoint;
}



void SendTargetToIGC()
{
    if (_lastTargetId == 0) return;

    byte relation = 0;
    
    // Определяем отношение
    switch (_lastDetectedInfo.Relationship)
    {
        case MyRelationsBetweenPlayerAndBlock.Owner:
        case MyRelationsBetweenPlayerAndBlock.Friends:
        case MyRelationsBetweenPlayerAndBlock.FactionShare:
            relation = (byte)TargetRelation.Friendly;
            break;
            
        case MyRelationsBetweenPlayerAndBlock.Enemies:
            relation = (byte)TargetRelation.Enemy;
            break;
            
        default:
            relation = (byte)TargetRelation.Neutral;
            break;
    }
    
    // Определяем тип
    switch (_lastDetectedInfo.Type)
    {
        case MyDetectedEntityType.LargeGrid:
            relation |= (byte)TargetRelation.LargeGrid;
            break;
            
        case MyDetectedEntityType.SmallGrid:
            relation |= (byte)TargetRelation.SmallGrid;
            break;
            
        case MyDetectedEntityType.Asteroid:
            relation = (byte)TargetRelation.Asteroid; // Астероид не враг
            break;
            
        case MyDetectedEntityType.Planet:
            relation = (byte)TargetRelation.Asteroid; // Планета как астероид
            break;
            
        case MyDetectedEntityType.Missile:
            relation |= (byte)TargetRelation.Missile;
            break;
            
        default:
            relation |= (byte)TargetRelation.Other;
            break;
    }
    
    // Радиус цели
    double radiusSq = _lastDetectedInfo.BoundingBox.HalfExtents.LengthSquared();
    
    var data = MyTuple.Create(
        relation,
        _lastTargetId,
        _lastTargetPosition,
        radiusSq
    );
    
    IGC.SendBroadcastMessage("IGC_IFF_MSG", data);
    
    //Echo($"IGC SENT: ID={_lastTargetId} Type={GetRelationName(relation)}");
}

// Вспомогательный метод для отображения типа
string GetRelationName(byte relation)
{
    TargetRelation rel = (TargetRelation)relation;
    string name = "";
    
    if ((rel & TargetRelation.Asteroid) != 0) name = "Asteroid";
    else if ((rel & TargetRelation.Missile) != 0) name = "Missile";
    else if ((rel & TargetRelation.LargeGrid) != 0) name = "LargeGrid";
    else if ((rel & TargetRelation.SmallGrid) != 0) name = "SmallGrid";
    else name = "Other";
    
    if ((rel & TargetRelation.Enemy) != 0) name += " Enemy";
    else if ((rel & TargetRelation.Friendly) != 0) name += " Friendly";
    else if ((rel & TargetRelation.Neutral) != 0) name += " Neutral";
    
    return name;
}


Vector3D RotateVectorByAngle(Vector3D vector, Vector3D axis, double angleDegrees)
{
    // Конвертируем градусы в радианы
    double angleRad = angleDegrees * Math.PI / 180.0;
    
    // Нормализуем ось вращения
    Vector3D normalizedAxis = Vector3D.Normalize(axis);
    
    // Формула Родрига для вращения вектора
    Vector3D rotated = vector * Math.Cos(angleRad) +
                       Vector3D.Cross(normalizedAxis, vector) * Math.Sin(angleRad) +
                       normalizedAxis * Vector3D.Dot(normalizedAxis, vector) * (1 - Math.Cos(angleRad));
    
    return rotated;
}



Vector3D GetAvoidDirection(string hitSide, double angleDegrees = 15.0)
{
    if (_controller == null)
        return Vector3D.Zero;

    // Вектор от корабля к точке ближайшей грани
    Vector3D toSide = _sidePoint - _controller.GetPosition();

    if (toSide.LengthSquared() < 0.0001)
        return Vector3D.Zero;

    toSide.Normalize();

    Vector3D axis;
    double angle = angleDegrees;

    switch (hitSide)
    {
        case "L":
            axis = _controller.WorldMatrix.Up;
            break;

        case "R":
            axis = _controller.WorldMatrix.Up;
            angle = -angleDegrees;
            break;

        case "U":
            axis = _controller.WorldMatrix.Right;
            angle = -angleDegrees;
            break;

        case "D":
            axis = _controller.WorldMatrix.Right;
            break;

        default:
            return toSide;
    }

    return Vector3D.Normalize(
        RotateVectorByAngle(toSide, axis, angle)
    );
}



void FlyOnVector()
{
    if (_controller == null || _gyros.Count == 0)
        return;

    // =========================================================
    // ПЕРВЫЙ ВЫЗОВ — ЗАПОМИНАЕМ СТОРОНУ И НАПРАВЛЕНИЕ ДВИЖЕНИЯ
    // =========================================================

    if (!_flyActive)
    {
        Vector3D velocity = _currentVelocity;

        if (velocity.LengthSquared() < 0.01)
            return;

        // Запоминаем направление движения
        _flyTargetDirection = Vector3D.Normalize(velocity);

        // Находим сторону с максимальной тягой
        var thrusts = GetThrustByDirection();

        double maxThrust = 0;
        _flyThrustDirection = "";

        foreach (var kvp in thrusts)
        {
            if (kvp.Value > maxThrust)
            {
                maxThrust = kvp.Value;
                _flyThrustDirection = kvp.Key;
            }
        }

        if (maxThrust < 0.1 || _flyThrustDirection == "")
            return;

        _flyActive = true;
    }

    // =========================================================
    // ПОЛУЧАЕМ НАПРАВЛЕНИЕ ЗАПОМНЕННОЙ СТОРОНЫ
    // =========================================================

    Vector3D currentDir;

    switch (_flyThrustDirection)
    {
        case "Up":
            currentDir = _controller.WorldMatrix.Up;
            break;

        case "Down":
            currentDir = -_controller.WorldMatrix.Up;
            break;

        case "Right":
            currentDir = _controller.WorldMatrix.Right;
            break;

        case "Left":
            currentDir = -_controller.WorldMatrix.Right;
            break;

        case "Forward":
            currentDir = _controller.WorldMatrix.Forward;
            break;

        case "Backward":
            currentDir = -_controller.WorldMatrix.Forward;
            break;

        default:
            _flyActive = false;
            return;
    }

    currentDir.Normalize();

    // =========================================================
    // СТОРОНА ТЯГИ ДОЛЖНА БЫТЬ ПРОТИВ ДВИЖЕНИЯ
    // =========================================================

    Vector3D targetDir = -_flyTargetDirection;
    targetDir.Normalize();

    // =========================================================
    // ОШИБКА
    // =========================================================

    double dot = Vector3D.Dot(
        currentDir,
        targetDir
    );

    dot = MathHelper.Clamp(dot, -1.0, 1.0);

    // =========================================================
    // УЖЕ НАВЕДЕНО
    // =========================================================

    if (dot > 0.9)
    {

        // =====================================================
        // ТРАСТЕРЫ В НАПРАВЛЕНИИ ДВИЖЕНИЯ — 100%
        // =====================================================

        foreach (var thruster in _thrusters)
        {
            if (!thruster.IsWorking)
                continue;

            double thrustDot = Vector3D.Dot(
                thruster.WorldMatrix.Forward,
                _flyTargetDirection
            );

            if (thrustDot > 0.7)
                thruster.ThrustOverridePercentage = 1.0f;
            else
                thruster.ThrustOverridePercentage = 0.0f;
        }

        _flyActive = false;
        _flyThrustDirection = "";

        return;
    }

    // =========================================================
    // ТОЧНО ТА ЖЕ СХЕМА ОШИБКИ, ЧТО В AutoOrientToGravity()
    // =========================================================

    Vector3D axis = Vector3D.Cross(
        currentDir,
        targetDir
    );

    double axisLength = axis.Length();

    // ---------------------------------------------------------
    // 180°
    // ---------------------------------------------------------

    if (axisLength < 0.0001)
    {
        axis = Vector3D.Cross(
            currentDir,
            _controller.WorldMatrix.Up
        );

        if (axis.LengthSquared() < 0.0001)
        {
            axis = Vector3D.Cross(
                currentDir,
                _controller.WorldMatrix.Right
            );
        }

        axis.Normalize();
    }
    else
    {
        axis /= axisLength;
    }

    // =========================================================
    // УГОЛ
    // =========================================================

    double angle = Math.Acos(
        MathHelper.Clamp(dot, -1.0, 1.0)
    );

    // =========================================================
    // ПРОЦЕНТ ОШИБКИ — КАК В AutoOrientToGravity()
    // =========================================================

    double error =
        angle / (Math.PI / 2.0);

    error =
        MathHelper.Clamp(
            error,
            0.0,
            1.0
        );

    // =========================================================
    // МАКСИМАЛЬНАЯ СКОРОСТЬ
    // =========================================================

    double maxAngularSpeed = 0.2;

    // =========================================================
    // ПРОПОРЦИОНАЛЬНОЕ ПОДРУЛИВАНИЕ
    // =========================================================

    double angularSpeed =
        error * maxAngularSpeed;

    // =========================================================
    // МИРОВОЙ ВЕКТОР
    // =========================================================

    Vector3D worldAngularVelocity =
        axis * angularSpeed;

    // =========================================================
    // ГИРОСКОПЫ
    // =========================================================

    foreach (var gyro in _gyros)
    {
        MatrixD worldToGyro =
            MatrixD.Transpose(
                gyro.WorldMatrix
            );

        Vector3D localAngularVelocity =
            Vector3D.TransformNormal(
                worldAngularVelocity,
                worldToGyro
            );

        gyro.GyroOverride = true;

        gyro.Pitch =
            (float)localAngularVelocity.X;

        gyro.Yaw =
            (float)localAngularVelocity.Y;

        gyro.Roll =
            (float)localAngularVelocity.Z;
    }

    // Пока ориентируемся — двигатели выключены
    foreach (var thruster in _thrusters)
    {
        if (!thruster.IsWorking)
            continue;

        thruster.ThrustOverridePercentage = 0.0f;
    }
}



void StopOrrient()
{
	foreach (var gyro in _gyros)
        {
            gyro.GyroOverride = false;
            gyro.Pitch = 0;
            gyro.Yaw = 0;
            gyro.Roll = 0;
        }
}