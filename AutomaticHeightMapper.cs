// R e a d m e
// -----------
// 
// In this file you can include any instructions or other comments you want to have injected onto the 
// top of your final script. You can safely delete this file if you do not want any such comments.
// 
// --- Configuration keys (Custom Data or arguments) ---
const string KEY_ROWS = "ROWS";
const string KEY_ROW_LEN = "ROW_LEN";
const string KEY_COLS = "COLS";
const string KEY_COL_WIDTH = "COL_WIDTH";
const string KEY_HOVER = "HOVER_ALT";

// Default values
int cfgRows = 5;
float cfgRowLen = 5f;
int cfgCols = 5;
float cfgColWidth = 5f;
float cfgHover = 2f;

// Block references
IMyRemoteControl remote;
IMyCameraBlock camera;
IMyTextSurface lcd;

// Grid data
struct Cell { public Vector3D pos; public double ground; public int status; }
// status: 0=unknown,1=sampled,2=failed
List<Cell> grid = new List<Cell>();
Vector3D originPos;
MatrixD originOrient; // world matrix

int nextIndex = 0;
bool running = false;

// FSM state for non-blocking nav/sampling
enum ScanState { Idle, NavigateToApproach, DescendSample, AscendRecover, Finished }
ScanState state = ScanState.Idle;
int fsmWaitTicks = 0; // helper counter
const int APPROACH_TIMEOUT = 1200; // ticks

public Program()
{
    ParseCustomData();
    FindBlocks();
    DeserializeStorage();
    Runtime.UpdateFrequency = UpdateFrequency.None;
}

public void Save()
{
    SerializeStorage();
}

string NormalizeDecimal(string s)
{
    if (string.IsNullOrEmpty(s)) return s;
    // replace comma with dot to tolerate locales
    return s.Replace(',', '.');
}

bool TryParseFloatInvariant(string s, out float val)
{
    val = 0f;
    if (string.IsNullOrEmpty(s)) return false;
    s = NormalizeDecimal(s);
    return float.TryParse(s, out val);
}

bool TryParseDoubleInvariant(string s, out double val)
{
    val = 0.0;
    if (string.IsNullOrEmpty(s)) return false;
    s = NormalizeDecimal(s);
    return double.TryParse(s, out val);
}

void ParseCustomData()
{
    try
    {
        var cd = Me.CustomData ?? "";
        foreach (var line in cd.Split(new[] {'\n','\r'}, StringSplitOptions.RemoveEmptyEntries))
        {
            var t = line.Trim();
            if (t.StartsWith("#")) continue;
            var parts = t.Split('=');
            if (parts.Length!=2) continue;
            var k = parts[0].Trim().ToUpper();
            var v = parts[1].Trim();
            switch(k)
            {
                case KEY_ROWS: int.TryParse(v,out cfgRows); break;
                case KEY_ROW_LEN: TryParseFloatInvariant(v, out cfgRowLen); break;
                case KEY_COLS: int.TryParse(v,out cfgCols); break;
                case KEY_COL_WIDTH: TryParseFloatInvariant(v, out cfgColWidth); break;
                case KEY_HOVER: TryParseFloatInvariant(v, out cfgHover); break;
            }
        }
    }
    catch {}
}

void FindBlocks()
{
    if (remote==null) remote = GridTerminalSystem.GetBlockWithName("AHM Remote") as IMyRemoteControl;
    if (remote==null) {
        var remotes = new List<IMyRemoteControl>();
        GridTerminalSystem.GetBlocksOfType(remotes);
        if (remotes.Count>0) remote = remotes[0];
    }
    if (camera==null) camera = GridTerminalSystem.GetBlockWithName("AHM Camera") as IMyCameraBlock;
    if (camera==null) {
        var cams = new List<IMyCameraBlock>();
        GridTerminalSystem.GetBlocksOfType(cams);
        if (cams.Count>0) camera = cams[0];
    }
    lcd = Me.GetSurface(0);
    if (lcd != null)
    {
        lcd.ContentType = ContentType.TEXT_AND_IMAGE;
        lcd.FontSize = 1.2f;
        lcd.Alignment = TextAlignment.LEFT;
    }
}

public void Main(string argument, UpdateType updateSource)
{
    var arg = (argument ?? "").Trim().ToUpper();
    if (arg == "") { EchoStatus(); return; }
    var parts = arg.Split(' ');
    switch (parts[0])
    {
        case "START": HandleStart(); break;
        case "STEP": HandleStep(); break;
        case "STOP": HandleStop(); break;
        case "RESET": HandleReset(); break;
        case "DUMP": HandleDump(); break;
        default: Echo("Unknown cmd: " + parts[0]); break;
    }
}

void HandleStart()
{
    // re-read blocks and configuration in case Custom Data changed
    FindBlocks();
    ParseCustomData();
    if (remote==null) { Echo("No RemoteControl"); return; }
    originPos = remote.GetPosition();
    originOrient = remote.WorldMatrix;
    GenerateGrid();
    nextIndex = 0;
    running = true;
    state = ScanState.NavigateToApproach;
    fsmWaitTicks = 0;
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    Echo("Started: grid " + cfgRows + "x" + cfgCols);
}

void HandleStep()
{
    // Start FSM for a single point
    if (nextIndex >= grid.Count) { Echo("No more points"); return; }
    FindBlocks();
    running = true;
    state = ScanState.NavigateToApproach;
    fsmWaitTicks = 0;
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

void HandleStop()
{
    running = false;
    Runtime.UpdateFrequency = UpdateFrequency.None;
    Echo("Stopped");
}

void HandleReset()
{
    grid.Clear();
    nextIndex = 0;
    running = false;
    Storage = "";
    Echo("Reset");
}

void HandleDump()
{
    var sb = new StringBuilder();
    sb.AppendLine("GRID DUMP:");
    for(int i=0;i<grid.Count;i++){
        var cell = grid[i];
        int r = i / cfgCols;
        int c = i % cfgCols;
        sb.AppendFormat("R{0}C{1}: Pos({2:0.00},{3:0.00},{4:0.00}) Ground:{5:0.00} Status:{6}", r, c, cell.pos.X, cell.pos.Y, cell.pos.Z, cell.ground, cell.status);
        sb.AppendLine();
    }
    if (grid.Count==0) sb.AppendLine("<no grid data>");
    WriteToLCD(sb.ToString());
    Echo("Dumped to LCD");
}

void EchoStatus()
{
    Echo($"AHM v1 Rows:{cfgRows} Cols:{cfgCols} Hover:{cfgHover}");
    Echo($"Next:{nextIndex} Cells:{grid.Count} Running:{running}");
}

void GenerateGrid()
{
    grid.Clear();
    var right = originOrient.Right;
    var forward = originOrient.Forward;
    for (int r=0;r<cfgRows;r++)
    {
        for (int c=0;c<cfgCols;c++)
        {
            var offset = right * (c * cfgColWidth) + forward * (r * cfgRowLen);
            var world = originPos + offset;
            grid.Add(new Cell { pos = world, ground = 0.0, status = 0 });
        }
    }
    SerializeStorage();
}

public void Update()
{
    if (!running) return;
    // FSM driven scanning
    if (nextIndex >= grid.Count) { state = ScanState.Finished; }

    switch (state)
    {
        case ScanState.NavigateToApproach:
            StartApproachWaypoint();
            state = ScanState.DescendSample;
            fsmWaitTicks = 0;
            break;
        case ScanState.DescendSample:
            fsmWaitTicks++;
            if (CheckApproachReached() || fsmWaitTicks > APPROACH_TIMEOUT)
            {
                // attempt sample
                var cell = grid[nextIndex];
                double measuredGround;
                bool ok = SampleHeightAtPoint(cell.pos, out measuredGround);
                if (ok) { cell.ground = measuredGround; cell.status = 1; }
                else { cell.status = 2; }
                grid[nextIndex] = cell;
                SerializeStorage();
                DrawAsciiGrid();
                nextIndex++;
                // prepare next or finish
                if (nextIndex >= grid.Count) { state = ScanState.Finished; }
                else { state = ScanState.NavigateToApproach; fsmWaitTicks = 0; }
            }
            break;
        case ScanState.Finished:
            running = false;
            Runtime.UpdateFrequency = UpdateFrequency.None;
            Echo("Scan finished");
            break;
        default:
            break;
    }
}

void StartApproachWaypoint()
{
    var cell = grid[nextIndex];
    var targetPos = cell.pos + new Vector3D(0,0,cfgHover + 10);
    remote.ClearWaypoints();
    remote.AddWaypoint(targetPos, "AHM_PT");
    remote.SetAutoPilotEnabled(true);
}

bool CheckApproachReached()
{
    var cell = grid[nextIndex];
    var targetPos = cell.pos + new Vector3D(0,0,cfgHover + 10);
    var pos = remote.GetPosition();
    var dx = (pos - targetPos);
    if (dx.Length() < 1.5 && remote.GetShipSpeed() < 0.75)
    {
        remote.SetAutoPilotEnabled(false);
        return true;
    }
    return false;
}

void PerformNextPoint()
{
    if (nextIndex >= grid.Count) { Echo("Scan complete"); running = false; Runtime.UpdateFrequency = UpdateFrequency.None; return; }
    var cell = grid[nextIndex];
    var targetPos = cell.pos + new Vector3D(0,0,cfgHover + 10);
    if (remote==null) { Echo("No remote"); running = false; return; }
    remote.ClearWaypoints();
    remote.AddWaypoint(targetPos, "AHM_PT");
    remote.SetAutoPilotEnabled(true);
    bool reached = WaitUntil(() =>
    {
        var pos = remote.GetPosition();
        var dx = (pos - targetPos);
        return dx.Length() < 1.0 && remote.GetShipSpeed() < 0.5;
    }, 300); // timeout
    remote.SetAutoPilotEnabled(false);
    if (!reached) { Echo("WP failed"); cell.status = 2; grid[nextIndex] = cell; nextIndex++; SerializeStorage(); return; }
    double measuredGround;
    bool ok = SampleHeightAtPoint(cell.pos, out measuredGround);
    if (ok) { cell.ground = measuredGround; cell.status = 1; }
    else { cell.status = 2; }
    grid[nextIndex] = cell;
    SerializeStorage();
    DrawAsciiGrid();
    nextIndex++;
}

bool WaitUntil(Func<bool> cond, int maxIterations)
{
    int i=0;
    while (i++ < maxIterations)
    {
        if (cond()) return true;
        // In PB environment, we can't sleep; instead rely on Update ticks.
        // Yield and continue on next run.
        return false;
    }
    return false;
}

bool SampleHeightAtPoint(Vector3D worldXY, out double groundHeight)
{
    groundHeight = 0.0;
    if (camera != null && camera.IsFunctional)
    {
        camera.EnableRaycast = true;
        var ray = camera.Raycast(1000);
        if (!ray.IsEmpty() && ray.HitPosition.HasValue)
        {
            groundHeight = ray.HitPosition.Value.Y;
            return true;
        }
    }
    var pos = remote.GetPosition();
    groundHeight = pos.Y - cfgHover;
    return true;
}

void DrawAsciiGrid()
{
    var sb = new StringBuilder();
    sb.AppendLine("GRID MAP");
    for (int r=0;r<cfgRows;r++)
    {
        for (int c=0;c<cfgCols;c++)
        {
            int i = r*cfgCols + c;
            if (i < grid.Count)
            {
                var cell = grid[i];
                switch(cell.status)
                {
                    case 0: sb.Append(" ??? "); break;
                    case 1: sb.AppendFormat("{0,5:0.0}", cell.ground); break;
                    case 2: sb.Append(" Err "); break;
                }
            }
            else sb.Append("  -  ");
            if (c < cfgCols-1) sb.Append('|');
        }
        sb.AppendLine();
    }
    WriteToLCD(sb.ToString());
}

void WriteToLCD(string text)
{
    if (lcd != null)
    {
        lcd.WriteText(text, false);
    }
    // always Echo also for PB log
    Echo(text);
}

void SerializeStorage()
{
    try
    {
        var sb = new StringBuilder();
        sb.AppendLine(cfgRows.ToString());
        sb.AppendLine(cfgCols.ToString());
        sb.AppendLine(cfgRowLen.ToString().Replace(',', '.'));
        sb.AppendLine(cfgColWidth.ToString().Replace(',', '.'));
        sb.AppendLine(cfgHover.ToString().Replace(',', '.'));
        // store origin position for validation
        sb.AppendLine(originPos.X.ToString().Replace(',', '.'));
        sb.AppendLine(originPos.Y.ToString().Replace(',', '.'));
        sb.AppendLine(originPos.Z.ToString().Replace(',', '.'));
        sb.AppendLine(nextIndex.ToString());
        sb.AppendLine(grid.Count.ToString());
        foreach(var cell in grid)
        {
            sb.AppendLine(string.Format("{0},{1},{2},{3}", cell.pos.X.ToString().Replace(',', '.'), cell.pos.Y.ToString().Replace(',', '.'), cell.pos.Z.ToString().Replace(',', '.'), cell.ground.ToString().Replace(',', '.')));
            sb.AppendLine(cell.status.ToString());
        }
        Storage = sb.ToString();
    }
    catch {}
}

void DeserializeStorage()
{
    try
    {
        if (string.IsNullOrEmpty(Storage)) return;
        var lines = Storage.Split(new[]{'\n','\r'}, StringSplitOptions.RemoveEmptyEntries);
        int idx = 0;
        int tmpInt;
        float tmpFloat;
        double tmpDouble;
        if (idx < lines.Length && int.TryParse(lines[idx++], out tmpInt)) cfgRows = tmpInt;
        if (idx < lines.Length && int.TryParse(lines[idx++], out tmpInt)) cfgCols = tmpInt;
        if (idx < lines.Length) { TryParseFloatInvariant(lines[idx++], out tmpFloat); cfgRowLen = tmpFloat; }
        if (idx < lines.Length) { TryParseFloatInvariant(lines[idx++], out tmpFloat); cfgColWidth = tmpFloat; }
        if (idx < lines.Length) { TryParseFloatInvariant(lines[idx++], out tmpFloat); cfgHover = tmpFloat; }
        // read origin position
        double ox=0, oy=0, oz=0;
        if (idx < lines.Length) { TryParseDoubleInvariant(lines[idx++], out tmpDouble); ox = tmpDouble; }
        if (idx < lines.Length) { TryParseDoubleInvariant(lines[idx++], out tmpDouble); oy = tmpDouble; }
        if (idx < lines.Length) { TryParseDoubleInvariant(lines[idx++], out tmpDouble); oz = tmpDouble; }
        originPos = new Vector3D(ox, oy, oz);
        if (idx < lines.Length && int.TryParse(lines[idx++], out tmpInt)) nextIndex = tmpInt;
        int count = 0;
        if (idx < lines.Length && int.TryParse(lines[idx++], out tmpInt)) count = tmpInt;
        grid.Clear();
        for (int i=0;i<count;i++)
        {
            if (idx >= lines.Length) break;
            var p = lines[idx++].Split(',');
            double x=0,y=0,z=0,g=0;
            if (p.Length >= 4)
            {
                TryParseDoubleInvariant(p[0], out x);
                TryParseDoubleInvariant(p[1], out y);
                TryParseDoubleInvariant(p[2], out z);
                TryParseDoubleInvariant(p[3], out g);
            }
            int s = 0;
            if (idx < lines.Length && int.TryParse(lines[idx++], out tmpInt)) s = tmpInt;
            grid.Add(new Cell { pos = new Vector3D(x,y,z), ground = g, status = s });
        }
        // validation: if grid contains only zeros, maybe storage corrupted — regenerate
        bool allZero = true;
        foreach(var cell in grid) { if (cell.pos.Length() > 0.0001) { allZero = false; break; } }
        if (allZero)
        {
            grid.Clear();
        }
    }
    catch { }
}
