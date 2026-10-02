/// <summary> 
/// Train Control v1.0 by Foltast 
/// All questions, ask help or give your suggestions you can on 
/// my discord: https://discord.gg/WAKfeUx8Ag 
/// </summary> 
 
 
string lcdTag = "[TC LCD]"; 
string shipControlTag = "[TC Controller]"; 
 
static float guidersMinimal = 0.01f; 
static float guidersMaximal = 0.26f; 
 
static string lockersTag = "Lockers"; 
static string forwardThrustersTag = "fwd"; 
static string backwardThrustersTag = "bwd"; 
 
static double firstForwardSpeed = 15; 
static double secondForwardSpeed = 30; 
static double firstBackwardSpeed = 5; 
static double secondBackwardSpeed = 30; 
static double couplingSpeed = 1.5; 
 
static bool enableKeyboardControl = true; 
 
 
// DO NOT TOUCH ANYTHING BELOW THIS LINE 
// EXCEPT IF YOU KNOW WHAT YOU ARE DOING  
//--------------------------------------// 
public Dictionary<string, Action> actions; 
 
string lockersStatus = "released"; 
string throttleStatus = "Zero"; 
 
Vector3D previousPosition; 
double currentSpeed = 0; 
double targetSpeed = 0; 
bool throttleIsUp = false; 
 
SpeedStage currentStage; 
SpeedStage previousStage; 
 
string lcdFontFamily = "Monospace"; 
float lcdFontSize = 0.8f; 
Color lcdFontColor = new Color(255, 130, 0); 
TextAlignment lcdFontAlignment = TextAlignment.CENTER; 
 
IMyTextSurface[] lcds = null; 
List<IMyThrust> forwardThrusters = new List<IMyThrust>(); 
List<IMyThrust> backwardThrusters = new List<IMyThrust>(); 
IMyShipController shipControl; 
 
int checkRate = 300; 
int currentCheckPass = 0; 
int throttlePosition = 0; 
 
int currentKeyFrame = 0; 
 
static Dictionary<int, string> throttles = new Dictionary<int, string>() 
{ 
    {-3, "Second Backward" }, 
    {-2, "First Backward" }, 
    {-1, "Coupling BWD" }, 
    {0, "Zero" }, 
    {1, "Coupling FWD" }, 
    {2, "First Forward" }, 
    {3, "Second Forward" }, 
}; 
 
public Program() 
{ 
    actions = new Dictionary<string, Action>() 
    { 
        {"switch lockers", delegate {SwitchLockers(false); } }, 
        {"stop", delegate {SetSpeed(SpeedStage.stop); } }, 
        {"first forward", delegate {SetSpeed(SpeedStage.firstForward); } }, 
        {"second forward", delegate {SetSpeed(SpeedStage.secondForward); } }, 
        {"first backward", delegate {SetSpeed(SpeedStage.firstBackward); } }, 
        {"throttle up", delegate{SetThrottle(ThrottleDirection.up); } }, 
        {"throttle down", delegate{SetThrottle(ThrottleDirection.down); } } 
    }; 
 
    Runtime.UpdateFrequency = UpdateFrequency.Update1; 
    SearchLCDs(); 
    SwitchLockers(true); 
} 
 
public void Main(string argument, UpdateType updateSource) 
{ 
    currentKeyFrame++; 
    currentCheckPass++; 
    currentSpeed = GetCurrentSpeed(); 
 
    if (throttleIsUp) 
    { 
        CheckSpeed(); 
    } 
 
    if (currentKeyFrame == 6) 
    { 
        currentKeyFrame = 0; 
    } 
 
    if (checkRate == currentCheckPass) 
    { 
        currentCheckPass = 0; 
        SearchLCDs(); 
        SearchController(); 
 
        forwardThrusters = GetThrusters(MovingDirection.forward); 
        backwardThrusters = GetThrusters(MovingDirection.backward); 
    } 
 
    UpdateLCDs(); 
 
    if (shipControl != null) 
    { 
        if (enableKeyboardControl && currentKeyFrame == 0) 
        { 
            if (shipControl.RollIndicator > 0) 
            { 
                SetThrottle(ThrottleDirection.up); 
            } 
            else if (shipControl.RollIndicator < 0) 
            { 
                SetThrottle(ThrottleDirection.down); 
            } 
        } 
    } 
 
    Echo("Train Control v1.0 by Foltast"); 
 
    if (string.IsNullOrWhiteSpace(argument)) 
    { 
        return; 
    } 
 
    if (actions.ContainsKey(argument)) 
    { 
        actions[argument](); 
    } 
} 
 
void SwitchLockers(bool checkOnly) 
{ 
    IMyBlockGroup lockersGroup = GridTerminalSystem 
        .GetBlockGroupWithName(lockersTag); 
 
    List<IMyTerminalBlock> lockers = new List<IMyTerminalBlock>(); 
    lockersGroup.GetBlocks(lockers); 
 
    if (lockers.Count < 1) 
    { 
        return; 
    } 
 
    float value = guidersMaximal; 
    lockersStatus = "released"; 
 
    if ((lockers[0] as IMyMotorSuspension).Height > guidersMinimal) 
    { 
        value = guidersMinimal; 
        lockersStatus = "locked"; 
    } 
 
    if (checkOnly) 
    { 
        return; 
    } 
 
    foreach (var locker in lockers) 
    { 
        (locker as IMyMotorSuspension).Height = value; 
    } 
} 
 
double GetCurrentSpeed() 
{ 
    Vector3D currentPosition = Me.GetPosition(); 
 
    double speed = ((currentPosition - previousPosition) * 60).Length(); 
    previousPosition = currentPosition; 
 
    return speed; 
} 
 
void SetSpeed(SpeedStage stage) 
{ 
    previousStage = currentStage; 
    currentStage = stage; 
 
    switch (stage) 
    { 
        case SpeedStage.stop: 
            throttleIsUp = false; 
            throttleStatus = "Zero"; 
            throttlePosition = 0; 
            targetSpeed = 0; 
            ChangeThrustersOverride(backwardThrusters, false); 
            ChangeThrustersOverride(forwardThrusters, false); 
            ChangeStateThrusters(backwardThrusters, true); 
            ChangeStateThrusters(forwardThrusters, true); 
            return; 
        case SpeedStage.couplingForward: 
            throttlePosition = 1; 
            targetSpeed = couplingSpeed; 
            break; 
        case SpeedStage.firstForward: 
            throttlePosition = 2; 
            targetSpeed = firstForwardSpeed; 
            break; 
        case SpeedStage.secondForward: 
            throttlePosition = 3; 
            targetSpeed = secondForwardSpeed; 
            break; 
        case SpeedStage.couplingBackward: 
            throttlePosition = -1; 
            targetSpeed = couplingSpeed; 
            break; 
        case SpeedStage.firstBackward: 
            throttlePosition = -2; 
            targetSpeed = firstBackwardSpeed; 
            break; 
        case SpeedStage.secondBackward: 
            throttlePosition = -3; 
            targetSpeed = secondBackwardSpeed; 
            break; 
    } 
 
    throttleIsUp = true; 
    throttleStatus = throttles[throttlePosition]; 
} 
 
void SetThrottle(ThrottleDirection direction) 
{ 
    if (direction == ThrottleDirection.down) 
    { 
        if (throttlePosition > -3) 
        { 
            throttlePosition--; 
        } 
    } 
    else 
    { 
        if (throttlePosition < 3) 
        { 
            throttlePosition++; 
        } 
    } 
 
    SetSpeed((SpeedStage)throttlePosition); 
} 
 
void CheckSpeed() 
{ 
    bool overrideThrusters = currentSpeed < targetSpeed; 
 
    if (currentStage < SpeedStage.stop || previousStage < SpeedStage.stop) 
    { 
        ChangeStateThrusters(backwardThrusters, overrideThrusters); 
        ChangeStateThrusters(forwardThrusters, !overrideThrusters); 
 
        ChangeThrustersOverride(forwardThrusters, false); 
        ChangeThrustersOverride(backwardThrusters, overrideThrusters); 
    } 
    else if (currentStage > SpeedStage.stop || previousStage > SpeedStage.stop) 
    { 
        ChangeStateThrusters(backwardThrusters, !overrideThrusters); 
        ChangeStateThrusters(forwardThrusters, overrideThrusters); 
 
        ChangeThrustersOverride(backwardThrusters, false); 
        ChangeThrustersOverride(forwardThrusters, overrideThrusters); 
    } 
} 
 
void ChangeStateThrusters(List<IMyThrust> thrusters, bool state) 
{ 
    foreach (var thruster in thrusters) 
    { 
        thruster.Enabled = state; 
    } 
} 
 
void ChangeThrustersOverride(List<IMyThrust> thrusters, bool fullOverride) 
{ 
    foreach (var thruster in thrusters) 
    { 
        thruster.ThrustOverridePercentage = fullOverride ? 1 : 0; 
    } 
} 
 
private List<IMyThrust> GetThrusters(MovingDirection direction) 
{ 
    List<IMyThrust> thrusters = new List<IMyThrust>(); 
 
    string targetDirection = direction == MovingDirection.forward ? 
        forwardThrustersTag : backwardThrustersTag; 
 
    GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters, 
        b => b.CustomName.Contains(targetDirection)); 
 
    return thrusters; 
} 
 
private void UpdateLCDs() 
{ 
    foreach (var lcd in lcds) 
    { 
        lcd.WriteText($"Train Control\n{new String('_', 30)}\n\nprev: {GetPrevThrottlePos()}" + 
            $"\n\nTHROTTLE: {throttleStatus.ToUpper()}\n\nnext: {GetNextThrottlePos()}\n" + 
            $"{new String('_', 30)}\n\nGUIDERS MODE: {lockersStatus}\n\nCURRENT SPEED: " + 
            $"{((int)currentSpeed).ToString()}\nTARGET SPEED: {targetSpeed.ToString()}"); 
    } 
} 
 
private string GetNextThrottlePos() 
{ 
    if (throttlePosition + 1 > 3) 
    { 
        return ""; 
    } 
    else return throttles[throttlePosition + 1].ToLower(); 
} 
 
private string GetPrevThrottlePos() 
{ 
    if (throttlePosition - 1 < -3) 
    { 
        return ""; 
    } 
    else return throttles[throttlePosition - 1].ToLower(); 
} 
 
private void SearchLCDs() 
{ 
    List<IMyTerminalBlock> tmp_lcds = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(tmp_lcds, b => b.CubeGrid == Me.CubeGrid && ((b is IMyTextSurfaceProvider && (b as IMyTextSurfaceProvider).SurfaceCount > 0) || b is IMyTextSurface) && b.CustomName.StartsWith(lcdTag)); 
 
    lcds = new IMyTextSurface[tmp_lcds.Count]; 
 
    for (int i = tmp_lcds.Count; i-- > 0;) 
    { 
        if (tmp_lcds[i] is IMyTextSurfaceProvider) 
        { 
            bool cust_si = false; 
            if (tmp_lcds[i].CustomName.Length > (lcdTag.Length + 2) && tmp_lcds[i].CustomName[lcdTag.Length] == '[' && tmp_lcds[i].CustomName[lcdTag.Length + 2] == ']') 
            { 
                int srf_idx = (int)tmp_lcds[i].CustomName[lcdTag.Length + 1] - 48; 
                if ((cust_si = srf_idx > 0 && srf_idx < 10 && (tmp_lcds[i] as IMyTextSurfaceProvider).SurfaceCount > srf_idx)) lcds[i] = ((IMyTextSurfaceProvider)tmp_lcds[i]).GetSurface(srf_idx); 
            } 
            if (!cust_si) lcds[i] = ((IMyTextSurfaceProvider)tmp_lcds[i]).GetSurface(0); 
        } 
        else lcds[i] = (IMyTextSurface)tmp_lcds[i]; 
 
        lcds[i].ContentType = (ContentType)1; 
        lcds[i].Font = lcdFontFamily; 
        lcds[i].FontSize = lcdFontSize; 
        lcds[i].FontColor = lcdFontColor; 
        lcds[i].Alignment = lcdFontAlignment; 
        lcds[i].ContentType = ContentType.TEXT_AND_IMAGE; 
    } 
} 
 
private void SearchController() 
{ 
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocksOfType<IMyTerminalBlock>(blocks, x => x.CustomName.Contains(shipControlTag)); 
 
    if (blocks.Count < 1) 
    { 
        return; 
    } 
 
    shipControl = blocks[0] as IMyShipController; 
} 
 
enum SpeedStage 
{ 
    secondBackward = -3, 
    firstBackward = -2, 
    couplingBackward = -1, 
    stop = 0, 
    couplingForward = 1, 
    firstForward = 2, 
    secondForward = 3 
} 
 
enum MovingDirection 
{ 
    forward, 
    backward 
} 
 
enum ThrottleDirection 
{ 
    up, 
    down 
}