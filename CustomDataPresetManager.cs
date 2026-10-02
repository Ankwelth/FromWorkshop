// Space Engineers – Custom Data Preset Manager (Global / English)
// Features:
// - Save any block's Custom Data as a named "preset"
// - Apply a preset to a single block (exact name) or to a whole group
// - In-game UI on an LCD (list/view/actions)
// - Preview panel that shows full preset content
// - Word wrapping + vertical scrolling on both UI and Preview
// - Optional auto-scroll on Preview
//
// Commands (run on the PB):
//   ui
//   list
//   up | down | enter | back
//   save <name> from:block:<namePart> | from:panel:<panelName>
//   apply <name> to:block:<ExactBlockName> | to:group:<GroupName>
//   view <name>
//   rename <old> <new>
//   delete <name>
//   sendtoeditor <name> panel:<panelName>
//   savefromeditor <name> panel:<panelName>
//   preview on|off|now|panel:<panelName>
//   scroll up|down|pageup|pagedown|top|bottom
//   autoscroll on|off|speed:<ticks>
//
// Notes:
// - Presets are stored in the PB's own Custom Data (Base64). Back it up if needed.
// - `to:block:` uses exact (case-insensitive) match to avoid accidental matches.
// - Default UI panel name: "Preset UI"; default Preview panel name: "Preset Preview".
// - Script targets C# 6 (in-game).

    // ===== Settings =====
    const string UI_PANEL_NAME = "Preset UI";
    const int UI_PAGE_SIZE = 5; // only affects the simple list page; View uses dynamic wrap/scroll
    const string STORAGE_SECTION = "PRESET_MANAGER";
    const string INDEX_KEY = "index";
    const string MODE_KEY = "mode";

    // Preview panel
    const string PREVIEW_PANEL_NAME = "Preset Preview";
    bool _previewEnabled = false;
    string _previewPanelName = PREVIEW_PANEL_NAME;

    // Scroll state
    int _viewScroll = 0;          // start row in UI View mode
    int _previewScroll = 0;       // start row in Preview panel
    bool _autoScroll = false;     // Preview auto-scroll
    int _autoScrollTick = 0;      // tick counter
    int _autoScrollEveryTicks = 6;// every N*Update10 ticks (≈0.6s per line at 1.0 sim speed)

    // ===== State =====
    MyIni _ini = new MyIni();
    List<string> _presetNames = new List<string>();
    int _cursor = 0;
    UIMode _mode = UIMode.List;

    enum UIMode { List, View, Actions }

    public Program()
    {
        Runtime.UpdateFrequency = UpdateFrequency.None;

        MyIniParseResult result;
        if (!_ini.TryParse(Me.CustomData, out result))
            _ini = new MyIni();

        LoadNames();
        LoadUIState();
    }

    public void Save()
    {
        _ini.Set(STORAGE_SECTION, INDEX_KEY, _cursor);
        _ini.Set(STORAGE_SECTION, MODE_KEY, _mode.ToString());
        Me.CustomData = _ini.ToString();
    }

    public void Main(string argument, UpdateType updateSource)
    {
        if (!string.IsNullOrWhiteSpace(argument))
            HandleCommand(argument.Trim());

        if ((updateSource & UpdateType.Update10) != 0)
        {
            DrawUI();

            // Preview auto-scroll
            if (_previewEnabled && _autoScroll)
            {
                _autoScrollTick++;
                if (_autoScrollTick >= _autoScrollEveryTicks)
                {
                    _autoScrollTick = 0;
                    _previewScroll++;
                    UpdatePreviewForCurrent(false);
                }
            }
        }
    }

    // ===== Commands =====
    void HandleCommand(string arg)
    {
        var t = arg.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        var cmd = t[0].ToLowerInvariant();
        var rest = t.Length > 1 ? t[1] : "";

        switch (cmd)
        {
            case "ui":
                Runtime.UpdateFrequency = UpdateFrequency.Update10;
                _viewScroll = 0; _previewScroll = 0;
                DrawUI();
                UpdatePreviewForCurrent(true);
                break;

            case "list":
                _mode = UIMode.List;
                _cursor = Math.Min(_cursor, Math.Max(0, _presetNames.Count - 1));
                _viewScroll = 0;
                DrawUI();
                UpdatePreviewForCurrent(true);
                break;

            case "view":
            {
                string vname;
                if (TryGetName(rest, out vname))
                {
                    _mode = UIMode.View;
                    _cursor = Math.Max(0, _presetNames.IndexOf(vname));
                    _viewScroll = 0;
                    DrawUI();
                    UpdatePreviewForCurrent(true);
                }
                break;
            }

            case "preview":
            {
                // preview on | preview off | preview now | preview panel:<name>
                var p = rest != null ? rest.Trim() : "";
                if (p.StartsWith("panel:", StringComparison.OrdinalIgnoreCase))
                {
                    _previewPanelName = p.Substring("panel:".Length).Trim();
                    Echo("Preview panel: " + _previewPanelName);
                    UpdatePreviewForCurrent(true);
                }
                else if (string.Equals(p, "on", StringComparison.OrdinalIgnoreCase))
                {
                    _previewEnabled = true;
                    Echo("Preview: ON");
                    UpdatePreviewForCurrent(true);
                }
                else if (string.Equals(p, "off", StringComparison.OrdinalIgnoreCase))
                {
                    _previewEnabled = false;
                    Echo("Preview: OFF");
                    ClearPreviewSurface();
                }
                else if (string.Equals(p, "now", StringComparison.OrdinalIgnoreCase))
                {
                    UpdatePreviewForCurrent(false);
                }
                else
                {
                    Echo("Usage: preview on|off|now|panel:<PanelName>");
                }
                break;
            }

            case "autoscroll":
            {
                // autoscroll on|off|speed:<ticks>
                var p = rest != null ? rest.Trim().ToLowerInvariant() : "";
                if (p == "on") { _autoScroll = true; Echo("Auto-scroll: ON"); }
                else if (p == "off") { _autoScroll = false; Echo("Auto-scroll: OFF"); }
                else if (p.StartsWith("speed:"))
                {
                    int n;
                    if (int.TryParse(p.Substring(6).Trim(), out n))
                    {
                        _autoScrollEveryTicks = Math.Max(1, n);
                        Echo("Auto-scroll speed (ticks): " + _autoScrollEveryTicks);
                    }
                    else Echo("autoscroll speed:<number>");
                }
                else Echo("Usage: autoscroll on|off|speed:<ticks>");
                break;
            }

            case "scroll":
            {
                // scroll up|down|pageup|pagedown|top|bottom
                var dir = rest != null ? rest.Trim().ToLowerInvariant() : "";
                int delta = 0;
                if (dir == "up") delta = -1;
                else if (dir == "down") delta = 1;
                else if (dir == "pageup") delta = -5;
                else if (dir == "pagedown") delta = 5;
                else if (dir == "top") { _viewScroll = 0; _previewScroll = 0; DrawUI(); UpdatePreviewForCurrent(false); break; }
                else if (dir == "bottom") { _viewScroll = int.MaxValue; _previewScroll = int.MaxValue; DrawUI(); UpdatePreviewForCurrent(false); break; }
                _viewScroll += delta;
                _previewScroll += delta;
                DrawUI();
                UpdatePreviewForCurrent(false);
                break;
            }

            case "save":
                SavePresetCommand(rest);
                break;

            case "apply":
                ApplyPresetCommand(rest);
                break;

            case "rename":
                RenamePresetCommand(rest);
                break;

            case "delete":
            {
                string dname;
                if (TryGetName(rest, out dname))
                {
                    DeletePreset(dname);
                    DrawUI();
                    UpdatePreviewForCurrent(true);
                }
                break;
            }

            case "sendtoeditor":
                SendToEditor(rest);
                break;

            case "savefromeditor":
                SaveFromEditor(rest);
                break;

            // UI navigation shortcuts
            case "up":
                if (_presetNames.Count > 0)
                    _cursor = (_cursor - 1 + _presetNames.Count) % _presetNames.Count;
                DrawUI();
                UpdatePreviewForCurrent(false);
                break;

            case "down":
                if (_presetNames.Count > 0)
                    _cursor = (_cursor + 1) % _presetNames.Count;
                DrawUI();
                UpdatePreviewForCurrent(false);
                break;

            case "enter":
                if (_presetNames.Count == 0) break;
                _mode = _mode == UIMode.List ? UIMode.View : UIMode.Actions;
                if (_mode == UIMode.View) _viewScroll = 0;
                DrawUI();
                UpdatePreviewForCurrent(true);
                break;

            case "back":
                _mode = _mode == UIMode.Actions ? UIMode.View : UIMode.List;
                DrawUI();
                UpdatePreviewForCurrent(false);
                break;

            default:
                Echo("Unknown command: " + cmd);
                Echo("Examples: list | save X from:block:Refinery | apply X to:group:MyGroup | ui");
                break;
        }
    }

    // ===== Preset Store =====
    void LoadNames()
    {
        _presetNames.Clear();
        var namesCsv = _ini.Get("PRESETS", "names").ToString();
        if (!string.IsNullOrWhiteSpace(namesCsv))
        {
            _presetNames = namesCsv
                .Split('|')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();
        }
    }

    void PersistNames()
    {
        var csv = string.Join("|", _presetNames.ToArray());
        _ini.Set("PRESETS", "names", csv);
        Me.CustomData = _ini.ToString();
    }

    bool HasPreset(string name) { return _presetNames.Contains(name); }

    void SavePreset(string name, string content)
    {
        var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(content ?? ""));
        _ini.Set("PRESET:" + name, "Content", b64);
        if (!HasPreset(name)) _presetNames.Add(name);
        PersistNames();
    }

    bool TryGetPreset(string name, out string content)
    {
        content = "";
        if (!HasPreset(name)) return false;
        var b64 = _ini.Get("PRESET:" + name, "Content").ToString();
        if (string.IsNullOrEmpty(b64)) return true;
        try
        {
            content = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
            return true;
        }
        catch
        {
            return false;
        }
    }

    void DeletePreset(string name)
    {
        if (!HasPreset(name)) return;
        _ini.DeleteSection("PRESET:" + name);
        _presetNames.Remove(name);
        PersistNames();
    }

    void RenamePresetCommand(string rest)
    {
        var parts = rest.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) { Echo("Usage: rename <old> <new>"); return; }
        var oldName = parts[0];
        var newName = parts[1];

        if (!HasPreset(oldName)) { Echo("No preset: " + oldName); return; }
        if (HasPreset(newName)) { Echo("Target name already exists: " + newName); return; }

        string content;
        if (!TryGetPreset(oldName, out content)) { Echo("Failed to read preset."); return; }

        SavePreset(newName, content);
        DeletePreset(oldName);
        _cursor = _presetNames.IndexOf(newName);
        _viewScroll = 0;
        DrawUI();
        UpdatePreviewForCurrent(true);
    }

    // ===== Command helpers =====
    bool TryGetName(string input, out string name)
    {
        name = input != null ? input.Trim() : null;
        if (string.IsNullOrWhiteSpace(name)) { Echo("Name required."); return false; }
        return true;
    }

    void SavePresetCommand(string rest)
    {
        var parts = rest.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) { Echo("Usage: save <name> from:block:<namePart> | from:panel:<panelName>"); return; }
        var name = parts[0];
        var src = parts.Length > 1 ? parts[1] : "";

        if (src.StartsWith("from:block:", StringComparison.OrdinalIgnoreCase))
        {
            var contains = src.Substring("from:block:".Length).Trim();
            var block = FindBlockByNameContains(contains);
            if (block == null) { Echo("Block not found: " + contains); return; }
            SavePreset(name, block.CustomData ?? "");
            Echo("Saved: " + name + " (" + block.CustomName + ")");
        }
        else if (src.StartsWith("from:panel:", StringComparison.OrdinalIgnoreCase))
        {
            var panelName = src.Substring("from:panel:".Length).Trim();
            var panel = FindSurfaceProviderByName(panelName);
            if (panel == null) { Echo("Panel not found: " + panelName); return; }
            var b = panel as IMyTerminalBlock;
            SavePreset(name, b != null ? b.CustomData : "");
            Echo("Saved: " + name + " (panel Custom Data)");
        }
        else
        {
            Echo("Specify source: from:block:<namePart> or from:panel:<panelName>");
        }

        LoadNames();
        _cursor = Math.Max(0, _presetNames.IndexOf(name));
        _viewScroll = 0;
        DrawUI();
        UpdatePreviewForCurrent(true);
    }

    void ApplyPresetCommand(string rest)
    {
        var parts = rest.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) { Echo("Usage: apply <name> to:group:<Group> | to:block:<ExactBlockName>"); return; }
        var name = parts[0];
        var dst = parts[1];

        string content;
        if (!TryGetPreset(name, out content))
        {
            Echo("Preset not found: " + name);
            return;
        }

        int count = 0;
        if (dst.StartsWith("to:group:", StringComparison.OrdinalIgnoreCase))
        {
            var gname = dst.Substring("to:group:".Length).Trim();
            var group = GridTerminalSystem.GetBlockGroupWithName(gname);
            if (group == null) { Echo("Group not found: " + gname); return; }
            var blocks = new List<IMyTerminalBlock>();
            group.GetBlocks(blocks);
            foreach (var b in blocks) { if (ApplyIfSameGrid(b, content)) count++; }
        }
        else if (dst.StartsWith("to:block:", StringComparison.OrdinalIgnoreCase))
        {
            var exact = dst.Substring("to:block:".Length).Trim();
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocks(blocks);
            foreach (var b in blocks)
            {
                if (!IsSameConstruct(b)) continue;
                if (string.Equals(b.CustomName.Trim(), exact, StringComparison.OrdinalIgnoreCase))
                {
                    b.CustomData = content ?? "";
                    count++;
                }
            }
        }
        else
        {
            Echo("Specify target: to:group:<Group> | to:block:<ExactBlockName>");
            return;
        }

        Echo("Applied: " + name + " -> " + count + " block(s)");
    }

    bool ApplyIfSameGrid(IMyTerminalBlock b, string content)
    {
        if (!IsSameConstruct(b)) return false;
        b.CustomData = content ?? "";
        return true;
    }

    bool IsSameConstruct(IMyTerminalBlock b)
    {
        return b.CubeGrid != null && Me.CubeGrid != null && b.CubeGrid.EntityId == Me.CubeGrid.EntityId;
    }

    // ===== UI / LCD =====
    void LoadUIState()
    {
        _cursor = _ini.Get(STORAGE_SECTION, INDEX_KEY).ToInt32(_cursor);
        var modeStr = _ini.Get(STORAGE_SECTION, MODE_KEY).ToString(_mode.ToString());

        UIMode parsed;
        if (Enum.TryParse<UIMode>(modeStr, out parsed))
            _mode = parsed;
    }

    // --- Preview helpers ---
    IMyTextSurface GetPreviewSurface()
    {
        var sp = FindSurfaceProviderByName(_previewPanelName);
        if (sp == null) return null;
        try { return sp.GetSurface(0); } catch { return null; }
    }

    void DrawPreview(string text, bool resetScroll)
    {
        var surf = GetPreviewSurface();
        if (surf == null) { Echo("Preview panel not found: " + _previewPanelName); return; }

        surf.ContentType = ContentType.TEXT_AND_IMAGE;
        surf.Font = "Monospace";
        surf.FontSize = 1.0f;
        surf.Alignment = TextAlignment.LEFT;

        int cols, rows;
        GetSurfaceMetrics(surf, out cols, out rows);
        var wrapped = WrapToWidth(text ?? "", cols).ToList();
        if (resetScroll) _previewScroll = 0;

        int start;
        var slice = SliceRows(wrapped, _previewScroll, rows, out start);
        _previewScroll = start; // clamp
        surf.WriteText(slice, false);
    }

    void ClearPreviewSurface()
    {
        var surf = GetPreviewSurface();
        if (surf != null) surf.WriteText("", false);
    }

    void UpdatePreviewForCurrent(bool resetScroll)
    {
        if (!_previewEnabled) return;
        if (_presetNames.Count == 0) { ClearPreviewSurface(); return; }
        var idx = Math.Max(0, Math.Min(_cursor, _presetNames.Count - 1));
        var name = _presetNames[idx];

        string content;
        if (TryGetPreset(name, out content))
            DrawPreview(content ?? "", resetScroll);
        else
            DrawPreview("(failed to read content)", resetScroll);
    }

    // --- Surface metrics & wrapping ---
    void GetSurfaceMetrics(IMyTextSurface surf, out int maxCols, out int maxRows)
    {
        var pad = surf.TextPadding;
        var innerW = surf.SurfaceSize.X - pad * 2f;
        var innerH = surf.SurfaceSize.Y - pad * 2f;

        var pxCharW = surf.MeasureStringInPixels(new StringBuilder("W"), surf.Font, surf.FontSize).X;
        var pxCharH = surf.MeasureStringInPixels(new StringBuilder("A"), surf.Font, surf.FontSize).Y;

        if (pxCharW <= 0f) pxCharW = 10f;
        if (pxCharH <= 0f) pxCharH = 18f;

        maxCols = Math.Max(8, (int)Math.Floor(innerW / pxCharW) - 1);
        maxRows = Math.Max(4, (int)Math.Floor(innerH / pxCharH) - 1);
    }

    IEnumerable<string> WrapToWidth(string text, int maxCols)
    {
        if (string.IsNullOrEmpty(text) || maxCols < 4) { yield return text ?? ""; yield break; }

        var lines = text.Replace("\r", "").Split('\n');
        foreach (var ln in lines)
        {
            var s = ln;
            while (s.Length > 0)
            {
                if (s.Length <= maxCols) { yield return s; break; }

                int cut = s.LastIndexOf(' ', Math.Min(maxCols, s.Length - 1));
                if (cut <= 0) cut = maxCols;

                yield return s.Substring(0, cut);
                s = s.Substring(cut).TrimStart();
            }
            if (ln.Length == 0) yield return "";
        }
    }

    string SliceRows(List<string> rows, int start, int maxRows, out int clampedStart)
    {
        if (maxRows < 1) { clampedStart = 0; return ""; }
        clampedStart = Math.Max(0, Math.Min(start, Math.Max(0, rows.Count - maxRows)));
        int end = Math.Min(rows.Count, clampedStart + maxRows);
        var window = rows.GetRange(clampedStart, end - clampedStart);
        if (clampedStart > 0 && window.Count > 0) window[0] = "↑ " + window[0];
        if (end < rows.Count && window.Count > 0) window[window.Count - 1] = "↓ " + window[window.Count - 1];
        return string.Join("\n", window);
    }

    // === UI drawing ===
    void DrawUI()
    {
        var surf = FindUIPanel();

        if (surf == null)
        {
            var pb = Me.GetSurface(0);
            int cols, rows;
            GetSurfaceMetrics(pb, out cols, out rows);
            var textAll = RenderText();
            var wrapped = WrapToWidth(textAll, cols).ToList();
            int s;
            var clipped = SliceRows(wrapped, 0, rows, out s);
            Echo(clipped);
            return;
        }

        surf.ContentType = ContentType.TEXT_AND_IMAGE;
        surf.Font = "Monospace";
        surf.FontSize = 1f;
        surf.Alignment = TextAlignment.LEFT;

        int maxCols, maxRows;
        GetSurfaceMetrics(surf, out maxCols, out maxRows);

        if (_mode == UIMode.View && _presetNames.Count > 0)
        {
            var idx = Math.Max(0, Math.Min(_cursor, _presetNames.Count - 1));
            var name = _presetNames[idx];

            string content;
            if (!TryGetPreset(name, out content)) content = "(failed to read)";

            var header = "== Custom Data Preset Manager ==\n[View] " + name + "   (ENTER: Actions, BACK: List)\n";
            var wrappedBody = WrapToWidth(content ?? "", maxCols).ToList();

            int start;
            var body = SliceRows(wrappedBody, _viewScroll, Math.Max(1, maxRows - 2), out start);
            _viewScroll = start;

            surf.WriteText(header + body, false);
        }
        else
        {
            var text = RenderText();
            var wrappedLines = WrapToWidth(text, maxCols).ToList();
            int s;
            var clippedText = SliceRows(wrappedLines, 0, maxRows, out s);
            surf.WriteText(clippedText, false);
        }

        UpdatePreviewForCurrent(false);
    }

    string RenderText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("== Custom Data Preset Manager ==");
        if (_presetNames.Count == 0)
        {
            sb.AppendLine("No presets yet.");
            sb.AppendLine("Example: save Test from:block:Refinery");
            return sb.ToString();
        }

        var current = Math.Max(0, Math.Min(_cursor, _presetNames.Count - 1));
        var pageStart = (current / UI_PAGE_SIZE) * UI_PAGE_SIZE;
        var pageEnd = Math.Min(pageStart + UI_PAGE_SIZE, _presetNames.Count);

        if (_mode == UIMode.List)
        {
            sb.AppendLine("[List]  (UP/DOWN, ENTER)");
            for (int i = pageStart; i < pageEnd; i++)
            {
                var marker = (i == current) ? ">" : " ";
                sb.AppendLine(marker + " " + _presetNames[i]);
            }
            if (pageEnd < _presetNames.Count) sb.AppendLine("... (more)");
            sb.AppendLine();
            sb.AppendLine("Commands: save/apply/view/rename/delete, sendtoeditor/savefromeditor");
        }
        else if (_mode == UIMode.View)
        {
            var name = _presetNames[current];
            sb.AppendLine("[View] " + name + "   (ENTER: Actions, BACK: List)");
            // Body is drawn with wrap/scroll in DrawUI()
        }
        else
        {
            var name = _presetNames[current];
            sb.AppendLine("[Actions] " + name + "   (BACK)");
            sb.AppendLine("- apply to:group:<GroupName>");
            sb.AppendLine("- apply to:block:<ExactBlockName>");
            sb.AppendLine("- rename <newName>");
            sb.AppendLine("- delete");
            sb.AppendLine("- sendtoeditor panel:<PanelName>");
            sb.AppendLine("- savefromeditor panel:<PanelName>");
            sb.AppendLine("- preview on|off|panel:<PanelName>");
            sb.AppendLine("- scroll up/down/pageup/pagedown/top/bottom");
        }

        return sb.ToString();
    }

    // ===== Block Helpers =====
    IMyTextSurface FindUIPanel()
    {
        var prov = FindSurfaceProviderByName(UI_PANEL_NAME);
        if (prov != null) return GetFirstSurface(prov);
        return Me.GetSurface(0);
    }

    IMyTextSurfaceProvider FindSurfaceProviderByName(string nameContains)
    {
        var list = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(list);
        foreach (var b in list)
        {
            if (!IsSameConstruct(b)) continue;
            if (b.CustomName.IndexOf(nameContains, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var sp = b as IMyTextSurfaceProvider;
                if (sp != null && sp.SurfaceCount > 0) return sp;
            }
        }
        return null;
    }

    IMyTextSurface GetFirstSurface(IMyTextSurfaceProvider sp)
    {
        try { return sp.GetSurface(0); }
        catch { return null; }
    }

    IMyTerminalBlock FindBlockByNameContains(string contains)
    {
        var list = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(list);
        return list.FirstOrDefault(b =>
            IsSameConstruct(b) &&
            b.CustomName.IndexOf(contains, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    // Editor flow: send preset content to a panel's Custom Data
    void SendToEditor(string rest)
    {
        // Usage: sendtoeditor <name> panel:<PanelName>
        var parts = rest.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) { Echo("Usage: sendtoeditor <name> panel:<PanelName>"); return; }
        var name = parts[0];
        var dst = parts[1];

        string content;
        if (!TryGetPreset(name, out content)) { Echo("Preset not found: " + name); return; }
        if (!dst.StartsWith("panel:", StringComparison.OrdinalIgnoreCase)) { Echo("panel:<PanelName> required"); return; }

        var panelName = dst.Substring("panel:".Length).Trim();
        var panel = FindSurfaceProviderByName(panelName);
        if (panel == null) { Echo("Panel not found: " + panelName); return; }

        var b = panel as IMyTerminalBlock;
        if (b != null) b.CustomData = content ?? "";
        Echo("Sent to editor: " + name + " -> " + (b != null ? b.CustomName : panelName));
    }

    // Editor flow: save panel's Custom Data back to preset
    void SaveFromEditor(string rest)
    {
        // Usage: savefromeditor <name> panel:<PanelName>
        var parts = rest.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) { Echo("Usage: savefromeditor <name> panel:<PanelName>"); return; }
        var name = parts[0];
        var src = parts[1];

        if (!src.StartsWith("panel:", StringComparison.OrdinalIgnoreCase)) { Echo("panel:<PanelName> required"); return; }
        var panelName = src.Substring("panel:".Length).Trim();
        var panel = FindSurfaceProviderByName(panelName);
        if (panel == null) { Echo("Panel not found: " + panelName); return; }

        var b = panel as IMyTerminalBlock;
        SavePreset(name, b != null ? (b.CustomData ?? "") : "");
        LoadNames();
        _cursor = Math.Max(0, _presetNames.IndexOf(name));
        _viewScroll = 0;
        Echo("Saved from editor: " + name);
        DrawUI();
        UpdatePreviewForCurrent(true);
    }
