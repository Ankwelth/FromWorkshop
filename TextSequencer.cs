/*
 * TERMINAL SEQUENCE PROCESSOR (v13 - Final)
 * ---------------------------------------------------
 * INSTRUCTIONS:
 * 1. Load this script into a Programmable Block.
 * 2. Add [AUTO-SCROLL] to the LCD/Cockpit Custom Data.
 * 3. Configure using the template below.
 *
 * --- CONFIGURATION TEMPLATE ---
 * [AUTO-SCROLL]
 * [CONFIG]
 * SPEED=2.0    // LINES PER SECOND. (0 = Instant/No Delay)
 * LENGTH=10    // MAX LINES. How many lines fit on the screen vertically.
 * WIDTH=30     // WRAP LIMIT. Max characters per line before wrapping.
 * PASTE=1      // TYPEWRITER MODE. 0=Instant Print, 1=Type Characters.
 *
 * [0]          // Screen Selection (Index 0)
 * [PASTE 1]    // Inline Command: Turn on typewriter
 * System Boot...
 * [SLEEP 2]    // Inline Command: Wait 2 seconds
 * [CLEAR]      // Inline Command: Clear screen
 * ---------------------------------------------------
 * AVAILABLE COMMANDS (Place on own line):
 * [SLEEP n]   - Pauses execution for n seconds.
 * [CLEAR]     - Clears all text from the screen.
 * [PASTE n]   - 0 = Instant Print, 1 = Typewriter Effect.
 * [SPEED n]   - Sets Flow Rate (Lines Per Second). 0 = Instant Block.
 * [LENGTH n]  - Sets Maximum Visible Lines.
 * [WIDTH n]   - Sets Character Wrap Limit.
 * [LOADING]   - Append to text line to animate "..." (e.g. "Connecting[LOADING]")
 * ---------------------------------------------------
 */

// Dictionary mapping Block EntityId -> BlockHandler
Dictionary<long, BlockHandler> _activeBlocks = new Dictionary<long, BlockHandler>();
int _scanTimer = 0;

public Program()
{
    // Update1 required for smooth typing
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
    ScanGrid();
}

public void Main(string argument, UpdateType updateSource)
{
    // Force full refresh if argument passed
    if (argument.ToLower() == "refresh")
    {
        _activeBlocks.Clear();
        ScanGrid();
        return;
    }

    double deltaTime = 1.0 / 60.0;

    // 1. Tick all active blocks (Animation)
    foreach (var handler in _activeBlocks.Values)
    {
        handler.UpdateTick(deltaTime);
    }

    // 2. Periodic Check for Data Changes (Every 100 ticks / ~1.6s)
    _scanTimer++;
    if (_scanTimer >= 100)
    {
        _scanTimer = 0;
        ScanGrid(); // Check for new blocks or changed data
    }
}

void ScanGrid()
{
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(blocks);

    // 1. Find valid blocks
    HashSet<long> foundIds = new HashSet<long>();

    foreach (var block in blocks)
    {
        if (string.IsNullOrWhiteSpace(block.CustomData) || 
            !block.CustomData.Contains("[AUTO-SCROLL]")) 
            continue;

        if (!(block is IMyTextSurfaceProvider)) continue;

        foundIds.Add(block.EntityId);

        // If we are already managing this block, check for changes
        if (_activeBlocks.ContainsKey(block.EntityId))
        {
            _activeBlocks[block.EntityId].CheckForUpdates(block.CustomData);
        }
        else
        {
            // New block found, create handler
            var handler = new BlockHandler(block as IMyTerminalBlock);
            // Initial Parse
            handler.CheckForUpdates(block.CustomData); 
            _activeBlocks.Add(block.EntityId, handler);
        }
    }

    // 2. Remove blocks that no longer exist or lost the tag
    List<long> toRemove = new List<long>();
    foreach(var id in _activeBlocks.Keys)
    {
        if (!foundIds.Contains(id)) toRemove.Add(id);
    }
    
    foreach(var id in toRemove) _activeBlocks.Remove(id);
    
    // Optional: Debug output
    // Echo($"Active Blocks: {_activeBlocks.Count}");
}

// --- BLOCK HANDLER ---
// Manages one physical block and all its screens
class BlockHandler
{
    IMyTerminalBlock _block;
    IMyTextSurfaceProvider _provider;
    string _lastCustomDataHash = "";
    List<TerminalProcessor> _processors = new List<TerminalProcessor>();

    public BlockHandler(IMyTerminalBlock block)
    {
        _block = block;
        _provider = block as IMyTextSurfaceProvider;
    }

    public void CheckForUpdates(string currentData)
    {
        // Simple string equality check. 
        // If data changed, we re-parse everything for this block.
        if (currentData != _lastCustomDataHash)
        {
            _lastCustomDataHash = currentData;
            Reparse(currentData);
        }
    }

    public void UpdateTick(double dt)
    {
        foreach(var p in _processors)
        {
            p.UpdateTick(dt);
        }
    }

    void Reparse(string data)
    {
        // Stop old processors
        _processors.Clear();

        // DEFAULTS
        float defSpeed = 2.0f; 
        int defMaxLines = 10;
        int defWidth = 30;
        bool defPaste = false;

        string[] lines = data.Split('\n');
        bool inConfig = false;
        
        // PASS 1: Globals
        foreach(var rawLine in lines)
        {
            string line = rawLine.Trim().ToUpper();
            if (line == "[CONFIG]") { inConfig = true; continue; }
            if (line.StartsWith("[") && line != "[CONFIG]") inConfig = false;

            if (inConfig && line.Contains("="))
            {
                string[] parts = line.Split('=');
                if (parts.Length != 2) continue;
                string key = parts[0].Trim();
                string val = parts[1].Trim();

                if (key == "SPEED") float.TryParse(val, out defSpeed);
                if (key == "LENGTH") int.TryParse(val, out defMaxLines);
                if (key == "WIDTH") int.TryParse(val, out defWidth);
                if (key == "PASTE") defPaste = (val == "1");
            }
        }

        // PASS 2: Screens
        int currentScreen = -1;
        List<SequenceCommand> cmds = new List<SequenceCommand>();

        foreach(var rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith("//")) continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                string tag = line.ToUpper();
                if (tag == "[AUTO-SCROLL]" || tag == "[CONFIG]") { currentScreen = -1; continue; }

                if (currentScreen != -1 && IsCommandTag(tag))
                {
                    cmds.Add(CreateCommand(tag));
                    continue; 
                }

                int parsedIndex;
                if (int.TryParse(line.Substring(1, line.Length - 2), out parsedIndex))
                {
                    if (currentScreen != -1 && cmds.Count > 0)
                    {
                        var surf = _provider.GetSurface(currentScreen);
                        if(surf != null) 
                            _processors.Add(new TerminalProcessor(surf, new List<SequenceCommand>(cmds), defSpeed, defMaxLines, defWidth, defPaste));
                    }
                    currentScreen = parsedIndex;
                    cmds.Clear();
                    continue;
                }
            }

            if (currentScreen != -1)
            {
                bool loadAnim = false;
                string text = rawLine;
                if (text.Contains("//")) text = text.Substring(0, text.IndexOf("//"));

                if (text.Trim().ToUpper().EndsWith("[LOADING]"))
                {
                    loadAnim = true;
                    int idx = text.LastIndexOf("[");
                    if (idx > 0) text = text.Substring(0, idx);
                }

                SequenceCommand c = new SequenceCommand();
                c.Type = CommandType.PrintText;
                c.TextPayload = text;
                c.IsLoadingAnimationEnabled = loadAnim;
                cmds.Add(c);
            }
        }

        if (currentScreen != -1 && cmds.Count > 0)
        {
            var surf = _provider.GetSurface(currentScreen);
            if(surf != null) 
                _processors.Add(new TerminalProcessor(surf, new List<SequenceCommand>(cmds), defSpeed, defMaxLines, defWidth, defPaste));
        }
    }

    bool IsCommandTag(string tag)
    {
        if (tag.StartsWith("[SLEEP")) return true;
        if (tag.StartsWith("[CLEAR")) return true;
        if (tag.StartsWith("[PASTE")) return true;
        if (tag.StartsWith("[SPEED")) return true;
        if (tag.StartsWith("[LENGTH")) return true;
        if (tag.StartsWith("[WIDTH")) return true;
        return false;
    }

    SequenceCommand CreateCommand(string tag)
    {
        SequenceCommand c = new SequenceCommand();
        string content = tag.Substring(1, tag.Length - 2); 
        string[] parts = content.Split(' ');
        string key = parts[0];
        string val = parts.Length > 1 ? parts[1] : "";

        if (key == "SLEEP") { c.Type = CommandType.Sleep; float.TryParse(val, out c.ValueFloat); }
        else if (key == "CLEAR") { c.Type = CommandType.ClearScreen; }
        else if (key == "PASTE") { c.Type = CommandType.SetTypewriterMode; c.ValueInt = (val == "1") ? 1 : 0; }
        else if (key == "SPEED") { c.Type = CommandType.SetLinesPerSecond; float.TryParse(val, out c.ValueFloat); }
        else if (key == "LENGTH") { c.Type = CommandType.SetMaxLines; int.TryParse(val, out c.ValueInt); }
        else if (key == "WIDTH") { c.Type = CommandType.SetWrapWidth; int.TryParse(val, out c.ValueInt); }
        
        return c;
    }
}

// --- CORE PROCESSOR ---

enum CommandType { PrintText, Sleep, ClearScreen, SetTypewriterMode, SetLinesPerSecond, SetMaxLines, SetWrapWidth }

struct SequenceCommand
{
    public CommandType Type;
    public string TextPayload;
    public float ValueFloat;
    public int ValueInt;
    public bool IsLoadingAnimationEnabled;
}

class TerminalProcessor
{
    IMyTextSurface _textSurface;
    List<SequenceCommand> _commandSequence;
    List<string> _visibleLines;
    
    float _settingLinesPerSecond; 
    int _settingMaxLines;
    int _settingWrapWidth;
    bool _settingTypewriterMode;

    int _instructionPointer = 0;
    float _taskTimer = 0;
    
    bool _isTypingActive = false;
    Queue<string> _pendingWrappedLines; 
    string _lineTextTarget = ""; 
    string _lineTextCurrent = ""; 
    bool _isWaitingForLoadingAnim = false; 
    float _secondsPerCharacter = 0; 

    bool _isLoadingAnimActive = false;
    float _loadingAnimTimer = 0;
    int _loadingAnimFrame = 0;

    public TerminalProcessor(IMyTextSurface surface, List<SequenceCommand> sequence, float speed, int maxLines, int width, bool pasteMode)
    {
        _textSurface = surface;
        _commandSequence = sequence;
        _settingLinesPerSecond = speed;
        _settingMaxLines = maxLines;
        _settingWrapWidth = width;
        _settingTypewriterMode = pasteMode;
        
        _visibleLines = new List<string>();
        _pendingWrappedLines = new Queue<string>();
        _textSurface.ContentType = VRage.Game.GUI.TextPanel.ContentType.TEXT_AND_IMAGE;
        _textSurface.WriteText("");
    }

    public void UpdateTick(double deltaTime)
    {
        // 1. Loading Animation
        if (_isLoadingAnimActive && !_isTypingActive && _visibleLines.Count > 0)
        {
            _loadingAnimTimer += (float)deltaTime;
            if (_loadingAnimTimer > 0.5f)
            {
                _loadingAnimTimer = 0;
                _loadingAnimFrame = (_loadingAnimFrame + 1) % 4;
                string baseText = _lineTextTarget; 
                string dots = new String('.', _loadingAnimFrame);
                _visibleLines[_visibleLines.Count - 1] = baseText + dots;
                RenderDisplay();
            }
        }

        // 2. Typewriter Logic
        if (_isTypingActive)
        {
            _taskTimer += (float)deltaTime;
            if (_taskTimer >= _secondsPerCharacter)
            {
                _taskTimer = 0;
                
                if (_lineTextCurrent.Length < _lineTextTarget.Length)
                {
                    _lineTextCurrent += _lineTextTarget[_lineTextCurrent.Length];
                    if (_visibleLines.Count > 0) _visibleLines[_visibleLines.Count - 1] = _lineTextCurrent;
                    else _visibleLines.Add(_lineTextCurrent);
                    RenderDisplay();
                }
                else
                {
                    if (_pendingWrappedLines.Count > 0)
                    {
                        _lineTextTarget = _pendingWrappedLines.Dequeue();
                        _lineTextCurrent = "";
                        AddLineToBuffer("");
                        CalculateCharacterDelay();
                    }
                    else
                    {
                        _isTypingActive = false;
                        if (_isWaitingForLoadingAnim) {
                            _isLoadingAnimActive = true;
                            _lineTextTarget = _visibleLines[_visibleLines.Count - 1]; 
                        }
                        _instructionPointer++;
                    }
                }
            }
            return;
        }

        // 3. Sleep Timer
        if (_taskTimer > 0)
        {
            _taskTimer -= (float)deltaTime;
            return; 
        }

        // 4. Reset
        if (_instructionPointer >= _commandSequence.Count)
        {
            _instructionPointer = 0;
            _visibleLines.Clear();
            _isLoadingAnimActive = false;
            RenderDisplay();
            return;
        }

        // 5. Execute
        ExecuteCommand(_commandSequence[_instructionPointer]);
    }

    void CalculateCharacterDelay()
    {
        if (_settingLinesPerSecond <= 0 || _settingLinesPerSecond > 60) {
            _secondsPerCharacter = 0; 
            return;
        }
        float totalTimeForLine = 1.0f / _settingLinesPerSecond;
        int charCount = Math.Max(1, _lineTextTarget.Length);
        _secondsPerCharacter = totalTimeForLine / (float)charCount;
    }

    void ExecuteCommand(SequenceCommand command)
    {
        switch (command.Type)
        {
            case CommandType.PrintText:
                _isLoadingAnimActive = false;
                List<string> wrappedLines = PerformWordWrap(command.TextPayload, _settingWrapWidth);

                if (_settingTypewriterMode)
                {
                    _pendingWrappedLines.Clear();
                    foreach(var line in wrappedLines) _pendingWrappedLines.Enqueue(line);
                    
                    _isTypingActive = true;
                    _isWaitingForLoadingAnim = command.IsLoadingAnimationEnabled;
                    
                    if (_pendingWrappedLines.Count > 0) {
                        _lineTextTarget = _pendingWrappedLines.Dequeue();
                        _lineTextCurrent = "";
                        AddLineToBuffer("");
                        CalculateCharacterDelay();
                    }
                }
                else
                {
                    foreach(var line in wrappedLines) AddLineToBuffer(line);
                    
                    if (command.IsLoadingAnimationEnabled) {
                        _isLoadingAnimActive = true;
                        if(_visibleLines.Count > 0) _lineTextTarget = _visibleLines[_visibleLines.Count - 1];
                    }
                    
                    if (_settingLinesPerSecond <= 0 || _settingLinesPerSecond > 60) {
                        _taskTimer = 0;
                    } else {
                        _taskTimer = (1.0f / _settingLinesPerSecond) * wrappedLines.Count;
                    }
                    _instructionPointer++;
                }
                break;

            case CommandType.Sleep:
                _taskTimer = command.ValueFloat;
                _instructionPointer++;
                break;

            case CommandType.ClearScreen:
                _visibleLines.Clear();
                _isLoadingAnimActive = false;
                RenderDisplay();
                _instructionPointer++;
                break;

            case CommandType.SetTypewriterMode: _settingTypewriterMode = (command.ValueInt == 1); _instructionPointer++; break;
            case CommandType.SetLinesPerSecond: _settingLinesPerSecond = command.ValueFloat; _instructionPointer++; break;
            case CommandType.SetMaxLines: _settingMaxLines = command.ValueInt; _instructionPointer++; break;
            case CommandType.SetWrapWidth: _settingWrapWidth = command.ValueInt; _instructionPointer++; break;
        }
    }

    List<string> PerformWordWrap(string input, int limit)
    {
        List<string> lines = new List<string>();
        string[] words = input.Split(' ');
        string currentLine = "";

        foreach (var word in words)
        {
            int projectedLength = currentLine.Length + word.Length + (currentLine.Length > 0 ? 1 : 0);
            if (projectedLength <= limit)
            {
                currentLine += (currentLine.Length > 0 ? " " : "") + word;
            }
            else
            {
                if (currentLine.Length > 0) { lines.Add(currentLine); currentLine = ""; }
                if (word.Length < 6) currentLine = word; 
                else
                {
                    string remaining = word;
                    while (remaining.Length > limit)
                    {
                        lines.Add(remaining.Substring(0, limit - 1) + "-");
                        remaining = remaining.Substring(limit - 1);
                    }
                    currentLine = remaining;
                }
            }
        }
        if (!string.IsNullOrEmpty(currentLine)) lines.Add(currentLine);
        return lines;
    }

    void AddLineToBuffer(string text)
    {
        _visibleLines.Add(text);
        if (_visibleLines.Count > _settingMaxLines) _visibleLines.RemoveAt(0);
        RenderDisplay();
    }

    void RenderDisplay()
    {
        _textSurface.WriteText(string.Join("\n", _visibleLines));
    }
}