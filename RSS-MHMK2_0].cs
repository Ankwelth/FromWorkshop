/* === RSS-MH MK 2.0] === 
// === [ИНСТРУКЦИЯ ПО НАСТРОЙКЕ (CustomData)] ===
// На LCD ( CustomData ):
//   @LCD_diag      - Основной монитор (График)
//   @LCD_diag:1    - Второй монитор (Статы, если провайдер)
//
// В ПБ Навигации ( CustomData ):
//   [Cfg]
//   Ang_Go=-20     - Максимальный угол вылета 
// Команды для ПБ (Аргументы):
//   REFRESH        - Полная перезагрузка блоков и CustomData
// === [ИНСТРУКЦИЯ КОНЕЦ] ===
=================================================================================== */ 

const string GRP = "susp";  
const string TAG = "@LCD_diag"; 
const string DEFAULT_SUS_PB = "PB Susp";
const float FIXED_FONT = 0.57f; 

List<IMyMotorStator> hinges = new List<IMyMotorStator>(); 
List<IMyTextSurface> displays = new List<IMyTextSurface>(); 
StringBuilder sb = new StringBuilder(); 
StringBuilder sbStats = new StringBuilder(); 
MyIni ini = new MyIni();

bool isWorking = true; 
double timer = 0; 
float updateRate = 0.16f; 
float dynamicMaxAngle = 20f; 
string susPBName = DEFAULT_SUS_PB;

public Program() { 
    InitBlocks(); 
    Runtime.UpdateFrequency = UpdateFrequency.Update10; 
} 

void Main(string argument, UpdateType updateSource) { 
    if (!string.IsNullOrEmpty(argument)) { 
        if (argument.ToUpper() == "REFRESH") { InitBlocks(); return; } 
    } 
    if (!isWorking || (updateSource & UpdateType.Update10) == 0) return; 
    UpdateDynamicLimit(); 
    timer += Runtime.TimeSinceLastRun.TotalSeconds; 
    if (timer >= updateRate) { UpdateDisplay(); timer = 0; } 
} 

void UpdateDynamicLimit() {
    var pb = GridTerminalSystem.GetBlockWithName(susPBName) as IMyProgrammableBlock;
    if (pb != null && ini.TryParse(pb.CustomData)) 
        dynamicMaxAngle = Math.Max(1f, Math.Abs(ini.Get("Cfg", "Ang_Go").ToSingle(20f)));
    else dynamicMaxAngle = 20f;
}

void InitBlocks() { 
    ini.Clear();
    if (ini.TryParse(Me.CustomData)) susPBName = ini.Get("Monitoring", "SusPBName").ToString(DEFAULT_SUS_PB);
    else Me.CustomData = $"[Monitoring]\nSusPBName={DEFAULT_SUS_PB}";

    hinges.Clear(); displays.Clear(); 
    var group = GridTerminalSystem.GetBlockGroupWithName(GRP); 
    if (group != null) { 
        List<IMyTerminalBlock> gb = new List<IMyTerminalBlock>(); 
        group.GetBlocks(gb); 
        foreach (var b in gb) if (b is IMyMotorStator) hinges.Add(b as IMyMotorStator); 
    } 
    List<IMyTerminalBlock> all = new List<IMyTerminalBlock>(); 
    GridTerminalSystem.GetBlocks(all); 
    foreach (var b in all) if (b.CustomData.Contains(TAG)) ParseSurface(b); 
     
    hinges.Sort((a, b) => a.CustomName.CompareTo(b.CustomName)); 
    foreach (var s in displays) { 
        s.ContentType = ContentType.TEXT_AND_IMAGE; 
        s.FontSize = FIXED_FONT; 
        s.FontColor = new Color(255, 100, 0); 
        s.Font = "Monospace";
    }
    Echo($"=== RSS-MH v2.0 ===\nMAX LIMIT: {dynamicMaxAngle}°\nPAIRS: {hinges.Count/2}"); 
} 

void UpdateDisplay() { 
    sb.Clear(); sbStats.Clear(); 
    var pairs = GroupPairs(); 

    sb.AppendLine("\n");
    AlignText(sb, "L-SUSPENSION       [ RSS-MH MK 2.0]       R-SUSPENSION", 60); 
    AlignText(sb, "────────────────────────────────────────────────────────────", 60); 

    sbStats.AppendLine("\n === [LIVE PAIR STATS] ===");
    sbStats.AppendLine(" P |  LEFT ANG  |  RIGHT ANG  | ST");
    sbStats.AppendLine(" ─────────────────────────────────");

    var sortedKeys = pairs.Keys.OrderBy(k => int.Parse(k)).ToList(); 
    foreach (var k in sortedKeys) { 
        var p = pairs[k]; 
        var L = p.Find(x => x.CustomName.ToUpper().Contains("_L")); 
        var R = p.Find(x => x.CustomName.ToUpper().Contains("_R")); 

        DrawWideSide(L, sb, false); 
        sb.Append($" {k} "); 
        DrawWideSide(R, sb, true); 
        sb.AppendLine(); 

        float aL = L != null ? (float)MathHelper.ToDegrees(L.Angle) : 0; 
        float aR = R != null ? (float)MathHelper.ToDegrees(R.Angle) : 0; 
        
        sbStats.Append($" {k} |");
        sbStats.Append(aL.ToString("+#0.0").PadLeft(10)).Append("° |");
        sbStats.Append(aR.ToString("+#0.0").PadLeft(10)).Append("° | ");
        
        float percent = (Math.Max(Math.Abs(aL), Math.Abs(aR)) / dynamicMaxAngle) * 101;
        // Статусы внизу оставляем как есть, они информативны
        if (aL > -0.1f || aR > -0.1f) sbStats.Append("[?]"); // Убрали [S], поставили [?] для отрицательных углов, раз сервис вырезан
        else if (percent < 10) sbStats.Append("[!]");
        else if (percent > 90) sbStats.Append("[#]");
        else sbStats.Append("[|]");
        sbStats.AppendLine();
    } 
    
    AlignText(sb, "────────────────────────────────────────────────────────────", 60); 
    AlignText(sb, $"MH v1.5.8 | LIMIT: {dynamicMaxAngle:0}° | {DateTime.Now:HH:mm:ss}", 60); 

    string finalOutput = sb.ToString() + sbStats.ToString();
    foreach (var s in displays) s.WriteText(finalOutput); 
} 
void DrawWideSide(IMyMotorStator m, StringBuilder res, bool isR) { 
    if (m == null) { res.Append(" --------- "); return; } 
    float a = (float)MathHelper.ToDegrees(m.Angle); 
    // Масштабируем: теперь 0 - это начало, а -20 - это 100% вылета
    float p = (Math.Abs(a) / dynamicMaxAngle) * 100; 
    
    string icon = "[|]", bC = "░"; 
    int segments = (int)MathHelper.Clamp(p / 10f, 0, 10);

    // НОВАЯ ИНВЕРТИРОВАННАЯ ЛОГИКА СЁГУНА:
    if (a > 0.5f) { 
        // Если угол ушел в плюс - это ОТБОЙ (удар в корпус)
        icon = "[!]"; bC = "█"; segments = 10; 
    } 
    else if (p > 100) { 
        // Если угол почти -20 (или больше по модулю) - это ПРОВИС
        icon = "[#]"; bC = "█"; segments = 10; 
    }
    else if (p <  1 && a < 0) {
        // Околонулевая зона в минусе - тоже можно подсветить как начало хода
        icon = "[|]"; 
    }

    string bar = new string(bC[0], segments).PadRight(10, ' '); 
    if (isR) res.Append(bar).Append(icon); 
    else res.Append(icon).Append(new string(bar.Reverse().ToArray())); 
}

void AlignText(StringBuilder builder, string text, int width) {
    int padding = Math.Max(0, (width - text.Length) / 2);
    builder.Append(new string(' ', padding)).AppendLine(text);
}

Dictionary<string, List<IMyMotorStator>> GroupPairs() {
    var p = new Dictionary<string, List<IMyMotorStator>>();
    foreach (var h in hinges) {
        var m = System.Text.RegularExpressions.Regex.Match(h.CustomName, @"\d+");
        string id = m.Success ? m.Value : "0";
        if (!p.ContainsKey(id)) p[id] = new List<IMyMotorStator>();
        p[id].Add(h);
    }
    return p;
}

void ParseSurface(IMyTerminalBlock b) { 
    var s = b as IMyTextSurface; if (s != null) { displays.Add(s); return; } 
    var p = b as IMyTextSurfaceProvider; if (p == null) return; 
    var lines = b.CustomData.Split('\n'); 
    foreach (var l in lines) if (l.Contains(TAG)) { 
        int idx = 0; var pts = l.Split(':'); 
        if (pts.Length > 1) int.TryParse(pts[1], out idx); 
        if (idx < p.SurfaceCount) displays.Add(p.GetSurface(idx)); 
        break; 
    } 
}