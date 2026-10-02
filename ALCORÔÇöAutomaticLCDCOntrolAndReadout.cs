/*
╔══════════════════════════════════════════════════════════════════════╗
║          ALCOR  v1.0                                                 ║
║       Automatic LCD COntrol & Readout                                ║
║                    Creatore: ø'mλsτ                                  ║
╠══════════════════════════════════════════════════════════════════════╣
║  SETUP RAPIDO                                                        ║
║  1. Carica questo script nel Programmable Block                     ║
║  2. Aggiungi [LCD] al nome di ogni LCD/cockpit da gestire           ║
║  3. Per LCD standalone: scrivi i comandi nel Custom Data            ║
║  4. Per cockpit (schermi multipli): usa @N per ogni schermo:        ║
║       @0                                                            ║
║       Power                                                         ║
║       Speed                                                         ║
║       @1                                                            ║
║       Cargo                                                         ║
║  5. Avvia lo script — si aggiorna ogni 10 tick (~6 volte/sec)       ║
╠══════════════════════════════════════════════════════════════════════╣
║  COMANDI DISPONIBILI                                                 ║
║                                                                      ║
║  --- TESTO ---                                                       ║
║  Echo <testo>        Testo statico libero                           ║
║  Center <testo>      Testo centrato                                 ║
║  Right <testo>       Testo allineato a destra                       ║
║  Line                Separatore orizzontale ─────────              ║
║  DLine               Separatore doppio      ══════════             ║
║  Blank               Riga vuota                                     ║
║  Time                Ora corrente del server                        ║
║  Time <prefisso>     Es: Time Aggiornato:                          ║
║                                                                      ║
║  --- ENERGIA ---                                                     ║
║  Power               Panoramica energia (tutti i generatori)        ║
║  Battery             Stato batterie con barra di carica             ║
║  Solar               Output pannelli solari                         ║
║                                                                      ║
║  --- INVENTARIO ---                                                  ║
║  Cargo               Capacità cargo totale con barra                ║
║  Ore                 Lista minerali con quantità                    ║
║  Ingot               Lista lingotti con quantità                    ║
║  Component           Lista componenti con quantità                  ║
║  Strumenti           Lista strumenti manuali (drill, welder...)     ║
║  Armi                Lista armi presenti                            ║
║  Munizioni           Lista munizioni e caricatori                   ║
║  Bombole             Conteggio bombole H2 e O2                      ║
║  Cibo                Lista cibo e consumabili (Survival update)     ║
║  Prototech           Lista componenti Prototech                     ║
║                                                                      ║
║  --- GAS ---                                                         ║
║  Hydrogen            Livello serbatoi idrogeno                      ║
║  Oxygen              Livello serbatoi ossigeno                      ║
║                                                                      ║
║  --- STRUTTURA ---                                                   ║
║  Damage              Lista blocchi danneggiati                      ║
║  Integrity           Integrità strutturale percentuale              ║
║                                                                      ║
║  --- SISTEMI ---                                                     ║
║  JumpDrive           Stato e carica motori FTL                      ║
║  Connector           Stato connettori                               ║
║  LandingGear         Stato zampe di atterraggio                     ║
║  Antenna             Stato antenne e beacon                         ║
║  Production          Stato raffinerie e assemblatori                ║
║                                                                      ║
║  --- NAVIGAZIONE ---                                                 ║
║  Speed               Velocità nave corrente (m/s e km/h)           ║
║  Mass                Massa struttura, cargo e totale                ║
║  Position            Coordinate GPS con tag copiabile               ║
║                                                                      ║
║  FILTRAGGIO GRUPPI:                                                  ║
║  Aggiungi {NomeGruppo} dopo il comando:                             ║
║    Cargo {Stiva Principale}                                         ║
║    Battery {Batterie Emergenza}                                     ║
║    Hydrogen {Serbatoi Motori}                                       ║
║                                                                      ║
║  OPZIONI (righe che iniziano con # — DEVONO essere le prime):      ║
║    #style=1  → barre ASCII    [####--------]                        ║
║    #style=2  → barre blocco   [████░░░░░░░░] (default)             ║
║    #style=3  → barre freccia  [>>>>........]                        ║
║    #width=N        → larghezza LCD in caratteri (default: 26)      ║
║    #scroll=off     → disabilita scroll automatico                  ║
║    #scrollspeed=N  → righe/esecuzione (default: 3 = ~0.5s/riga)   ║
║    #scrollpause=N  → pausa in cima/fondo (default: 12 = ~2s)      ║
║                                                                      ║
║  ESEMPIO Custom Data di un LCD:                                     ║
║    #style=2                                                         ║
║    Center === NAVE PRINCIPALE ===                                   ║
║    DLine                                                            ║
║    Power                                                            ║
║    Battery                                                          ║
║    Line                                                             ║
║    Cargo                                                            ║
║    Line                                                             ║
║    Damage                                                           ║
║    Speed                                                            ║
║    Time Aggiornato:                                                 ║
╚══════════════════════════════════════════════════════════════════════╝
*/

// ══════════════════════════════ CONFIGURAZIONE ════════════════════════
const string LCD_TAG = "[LCD]";    // Tag per identificare gli LCD
const int    BAR_W   = 22;         // Larghezza barre di progresso
const float  WARN    = 0.30f;      // Soglia avviso giallo (30%)
const float  CRIT    = 0.10f;      // Soglia critica rossa (10%)
const float  HIGH    = 0.90f;      // Soglia saturazione (90%)
// ══════════════════════════════════════════════════════════════════════

// I colori inline non sono supportati in vanilla Space Engineers.
// Usiamo prefissi testuali per indicare lo stato visivamente.
const string CG = ""; const string CY = ""; const string CR = "";
const string CC = ""; const string CW = ""; const string CD = ""; const string CO = "";

int _tick = 0;
readonly StringBuilder _sb = new StringBuilder(4096);

// Scroll state per pannello (chiave = EntityId)
readonly Dictionary<long, int> _scrollPos   = new Dictionary<long, int>();
readonly Dictionary<long, int> _scrollPause = new Dictionary<long, int>();

public Program()
{
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Save() { }

public void Main(string argument, UpdateType updateSource)
{
    _tick++;
    int count = 0;

    // ── 1. LCD standalone (IMyTextPanel)
    var panels = new List<IMyTextPanel>();
    GridTerminalSystem.GetBlocksOfType(panels, p =>
        p.CustomName.Contains(LCD_TAG) && p.IsSameConstructAs(Me));

    foreach (var panel in panels)
    {
        try   { ProcessSurface(panel, panel.CustomData, panel.EntityId); count++; }
        catch (Exception e) { panel.WriteText($"[ALCOR ERRORE]\n{e.Message}"); }
    }

    // ── 2. Cockpit / provider con schermi multipli
    //    Il nome del blocco deve contenere [LCD]
    //    Il Custom Data usa sezioni @N per ogni schermo:
    //      @0
    //      Power
    //      Speed
    //      @1
    //      Cargo
    var allBlocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(allBlocks, b =>
        b.CustomName.Contains(LCD_TAG) &&
        b.IsSameConstructAs(Me) &&
        !(b is IMyTextPanel));

    foreach (var block in allBlocks)
    {
        var provider = block as IMyTextSurfaceProvider;
        if (provider == null) continue;

        try
        {
            var sections = ParseSurfaceSections(block.CustomData, provider.SurfaceCount);
            foreach (var kv in sections)
            {
                int idx = kv.Key;
                if (idx < 0 || idx >= provider.SurfaceCount) continue;
                var surf = provider.GetSurface(idx);
                // ID unico = EntityId del blocco * 100 + indice schermo
                long surfId = block.EntityId * 100 + idx;
                ProcessSurface(surf, kv.Value, surfId);
                count++;
            }
        }
        catch (Exception e)
        {
            provider.GetSurface(0).WriteText($"[ALCOR ERRORE]\n{e.Message}");
        }
    }

    Echo($"ALCOR v1.0\nAutomatic LCD COntrol & Readout\nSuperfici gestite: {count}\nTick: {_tick}");
}

// Divide il Custom Data in sezioni @N → testo
// Se non ci sono marker @N tratta tutto come @0
Dictionary<int, string> ParseSurfaceSections(string customData, int surfCount)
{
    var result = new Dictionary<int, string>();
    var raw = customData ?? "";

    if (!raw.Contains("@"))
    {
        // Nessun marker: tutto va allo schermo 0
        result[0] = raw;
        return result;
    }

    var lines = raw.Split('\n');
    int curIdx = 0;
    var curLines = new List<string>();
    bool started = false;

    foreach (var line in lines)
    {
        var t = line.Trim();
        if (t.StartsWith("@"))
        {
            // Salva sezione precedente
            if (started && curLines.Count > 0)
                result[curIdx] = string.Join("\n", curLines);
            // Inizia nuova sezione
            int.TryParse(t.Substring(1).Trim(), out curIdx);
            curLines.Clear();
            started = true;
        }
        else if (started)
        {
            curLines.Add(line);
        }
    }
    // Salva ultima sezione
    if (started && curLines.Count > 0)
        result[curIdx] = string.Join("\n", curLines);

    return result;
}

// ════════════════════════════ GESTIONE SUPERFICI ══════════════════════

void ProcessSurface(IMyTextSurface surface, string customData, long surfaceId)
{
    var raw = (customData ?? "").Trim();
    if (string.IsNullOrWhiteSpace(raw))
    {
        surface.WriteText("[ALCOR]\nNessun comando nel Custom Data.\n\nScrivi i comandi qui.\nPer cockpit usa @N per ogni schermo.");
        return;
    }

    var cfg   = new PanelConfig();
    var lines = raw.Split('\n');
    _sb.Clear();

    // Leggi riga per riga: le righe che iniziano con # sono configurazione
    int cmdStart = 0;
    for (int i = 0; i < lines.Length; i++)
    {
        var l = lines[i].Trim();
        if (l.StartsWith("#")) { ParseConfig(l, cfg); cmdStart = i + 1; }
        else break;
    }

    // Esegui ogni comando
    for (int i = cmdStart; i < lines.Length; i++)
    {
        var line = lines[i].Trim();
        if (string.IsNullOrEmpty(line) || line.StartsWith("//")) continue;
        ExecuteCommand(line, cfg);
    }

    // Forza font Monospace: indispensabile per allineamento colonne
    surface.Font = "Monospace";

    // Adatta automaticamente la dimensione del font al contenuto
    if (cfg.AutoFit) AutoFitFont(surface, cfg);

    ApplyScroll(surface, cfg, surfaceId);
}

// ════════════════════════════ AUTOFIT FONT ════════════════════════════

void AutoFitFont(IMyTextSurface surface, PanelConfig cfg)
{
    // Margine interno LCD (bordi non scrivibili)
    const float PAD = 0.92f;
    float sw = surface.SurfaceSize.X * PAD;
    float sh = surface.SurfaceSize.Y * PAD;

    var content = _sb.ToString();
    var lines   = content.Split('\n');

    // Trova riga più lunga e conta righe non vuote
    string longestLine = "";
    int lineCount = 0;
    foreach (var l in lines)
    {
        string s = l.TrimEnd('\r');
        if (s.Length > longestLine.Length) longestLine = s;
        if (!string.IsNullOrWhiteSpace(s)) lineCount++;
    }
    if (string.IsNullOrEmpty(longestLine) || lineCount == 0) return;

    var sbLine   = new StringBuilder(longestLine);
    var sbSingle = new StringBuilder("W");  // riga di riferimento per altezza

    // ── Calcola font per LARGHEZZA con ricerca binaria su misura reale ──
    float lo = cfg.MinFont, hi = cfg.MaxFont, fontW = cfg.MinFont;
    for (int i = 0; i < 12; i++)
    {
        float mid = (lo + hi) * 0.5f;
        float w   = surface.MeasureStringInPixels(sbLine, "Monospace", mid).X;
        if (w <= sw) { fontW = mid; lo = mid; }
        else         { hi = mid; }
    }

    float targetFont = fontW;

    // ── Controlla altezza e massimizza se il contenuto entra ──
    float lineH = surface.MeasureStringInPixels(sbSingle, "Monospace", fontW).Y;
    if (lineH > 0)
    {
        int visLines = Math.Max(1, (int)Math.Floor(sh / lineH));

        if (!cfg.Scroll || lineCount <= visLines)
        {
            // Scroll off oppure contenuto che entra → massimizza con altezza
            lo = cfg.MinFont; hi = cfg.MaxFont; float fontH = cfg.MinFont;
            for (int i = 0; i < 12; i++)
            {
                float mid = (lo + hi) * 0.5f;
                float lh  = surface.MeasureStringInPixels(sbSingle, "Monospace", mid).Y;
                if (lh * lineCount <= sh) { fontH = mid; lo = mid; }
                else                      { hi = mid; }
            }
            targetFont = Math.Min(fontW, fontH);
        }
        // Scroll on e contenuto che scorre → usa solo fontW (altezza gestita dallo scroll)
    }

    targetFont = Math.Max(cfg.MinFont, Math.Min(cfg.MaxFont, targetFont));
    targetFont = (float)Math.Round(targetFont, 2);
    surface.FontSize = targetFont;
}

// ════════════════════════════ SCROLL ══════════════════════════════════

void ApplyScroll(IMyTextSurface surface, PanelConfig cfg, long surfaceId)
{
    var allLines = new List<string>(_sb.ToString().Split('\n'));
    while (allLines.Count > 0 && string.IsNullOrWhiteSpace(allLines[allLines.Count - 1]))
        allLines.RemoveAt(allLines.Count - 1);

    // Calcola righe visibili con misura reale del font
    const float PAD_S = 0.92f;
    float shS = surface.SurfaceSize.Y * PAD_S;
    var sbRef = new StringBuilder("W");
    float lineHRef = surface.MeasureStringInPixels(sbRef, "Monospace", surface.FontSize).Y;
    int visLines = lineHRef > 0
        ? Math.Max(1, (int)Math.Floor(shS / lineHRef))
        : Math.Max(1, (int)Math.Floor(17.0 / surface.FontSize));

    long id = surfaceId;
    if (!_scrollPos.ContainsKey(id))   _scrollPos[id]   = 0;
    if (!_scrollPause.ContainsKey(id)) _scrollPause[id] = cfg.ScrollPause;

    int pos   = _scrollPos[id];
    int pause = _scrollPause[id];

    // Se il contenuto entra tutto → nessuno scroll
    if (!cfg.Scroll || allLines.Count <= visLines)
    {
        surface.WriteText(string.Join("\n", allLines));
        _scrollPos[id]   = 0;
        _scrollPause[id] = cfg.ScrollPause;
        return;
    }

    int end = Math.Min(pos + visLines, allLines.Count);
    var window = allLines.GetRange(pos, end - pos);
    surface.WriteText(string.Join("\n", window));

    // Avanza solo ogni ScrollSpeed esecuzioni
    if (_tick % cfg.ScrollSpeed != 0) return;

    if (pause > 0)
    {
        _scrollPause[id] = pause - 1;
        return;
    }

    pos++;
    // Fine lista: pausa e torna in cima
    if (pos + visLines > allLines.Count)
    {
        pos = 0;
        _scrollPause[id] = cfg.ScrollPause;
    }
    // In cima: piccola pausa iniziale
    else if (pos == 0)
    {
        _scrollPause[id] = cfg.ScrollPause;
    }
    else
    {
        _scrollPause[id] = 0;
    }
    _scrollPos[id] = pos;
}

// ════════════════════════════ CONFIGURAZIONE PANNELLO ═════════════════

class PanelConfig
{
    public int  BarStyle    = 2;
    public bool Colors      = true;
    public int  Width       = 26;
    public bool Scroll      = true;    // scroll automatico se testo > schermo
    public int  ScrollSpeed = 3;       // esecuzioni tra una riga e l'altra (~0.5s)
    public int  ScrollPause = 12;      // pausa in cima/fondo (~2s)
    public bool AutoFit     = true;    // adatta font size automaticamente
    public float MinFont    = 0.4f;    // dimensione minima font
    public float MaxFont    = 10.0f;   // dimensione massima font
}

void ParseConfig(string line, PanelConfig cfg)
{
    if (line.StartsWith("#style="))       int.TryParse(line.Substring(7), out cfg.BarStyle);
    if (line.StartsWith("#colors="))      cfg.Colors = !line.Contains("off");
    if (line.StartsWith("#width="))       int.TryParse(line.Substring(7), out cfg.Width);
    if (line.StartsWith("#scroll="))      cfg.Scroll = !line.Contains("off");
    if (line.StartsWith("#scrollspeed=")) int.TryParse(line.Substring(13), out cfg.ScrollSpeed);
    if (line.StartsWith("#scrollpause=")) int.TryParse(line.Substring(13), out cfg.ScrollPause);
    if (line.StartsWith("#autofit="))     cfg.AutoFit = !line.Contains("off");
    if (line.StartsWith("#minfont="))     float.TryParse(line.Substring(9),  System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out cfg.MinFont);
    if (line.StartsWith("#maxfont="))     float.TryParse(line.Substring(9),  System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out cfg.MaxFont);
}

// ════════════════════════════ DISPATCHER COMANDI ══════════════════════

void ExecuteCommand(string line, PanelConfig cfg)
{
    string cmd, group, arg;
    ParseLine(line, out cmd, out group, out arg);

    switch (cmd.ToUpper())
    {
        // ── TESTO
        case "ECHO":         AL(arg); break;
        case "CENTER":       AppCenter(arg, cfg); break;
        case "RIGHT":        AppRight(arg, cfg); break;
        case "LINE":         AL(new string('\u2500', cfg.Width)); break; // ─
        case "DLINE":        AL(new string('\u2550', cfg.Width)); break; // ═
        case "BLANK":        AL(""); break;
        case "TIME":
            string prefix = string.IsNullOrEmpty(arg) ? "" : arg + " ";
            AL($"{prefix}{DateTime.Now:HH:mm:ss}");
            break;

        // ── ENERGIA
        case "POWER":        CmdPower(cfg, group); break;
        case "BATTERY":      CmdBattery(cfg, group); break;
        case "SOLAR":        CmdSolar(cfg, group); break;

        // ── INVENTARIO
        case "CARGO":        CmdCargo(cfg, group); break;
        case "ORE":          CmdInventory("Ore", cfg, group); break;
        case "INGOT":        CmdInventory("Ingot", cfg, group); break;
        case "COMPONENT":    CmdInventory("Component", cfg, group); break;
        case "TOOL":
        case "STRUMENTI":    CmdTools(cfg, group); break;
        case "WEAPON":
        case "ARMI":         CmdWeapons(cfg, group); break;
        case "AMMO":
        case "MUNIZIONI":    CmdInventory("AmmoMagazine", cfg, group); break;
        case "BOTTLE":
        case "BOMBOLE":      CmdBottles(cfg, group); break;
        case "FOOD":
        case "CIBO":         CmdInventory("ConsumableItem", cfg, group); break;
        case "PROTOTECH":    CmdPrototech(cfg, group); break;

        // ── GAS
        case "HYDROGEN":     CmdGas("Hydrogen", cfg, group); break;
        case "OXYGEN":       CmdGas("Oxygen", cfg, group); break;

        // ── STRUTTURA
        case "DAMAGE":       CmdDamage(cfg, group); break;
        case "INTEGRITY":    CmdIntegrity(cfg); break;

        // ── SISTEMI
        case "JUMPDRIVE":    CmdJumpDrive(cfg, group); break;
        case "CONNECTOR":    CmdConnector(cfg, group); break;
        case "LANDINGGEAR":  CmdLandingGear(cfg, group); break;
        case "ANTENNA":      CmdAntenna(cfg, group); break;
        case "PRODUCTION":   CmdProduction(cfg, group); break;

        // ── NAVIGAZIONE
        case "SPEED":        CmdSpeed(cfg); break;
        case "MASS":         CmdMass(cfg); break;
        case "POSITION":     CmdPosition(cfg); break;

        default:
            ALC($"[?] Sconosciuto: {cmd}", CY, cfg.Colors);
            break;
    }
}

void ParseLine(string line, out string cmd, out string group, out string arg)
{
    group = "";
    // Estrai {gruppo}
    int gb = line.IndexOf('{'), ge = line.IndexOf('}');
    if (gb >= 0 && ge > gb)
    {
        group = line.Substring(gb + 1, ge - gb - 1).Trim();
        line  = (line.Substring(0, gb) + line.Substring(ge + 1)).Trim();
    }
    int sp = line.IndexOf(' ');
    if (sp < 0) { cmd = line; arg = ""; }
    else        { cmd = line.Substring(0, sp); arg = line.Substring(sp + 1).Trim(); }
}

// ════════════════════════════ ENERGIA ════════════════════════════════

void CmdPower(PanelConfig cfg, string group)
{
    ALC("ENERGIA", CC, cfg.Colors);

    var producers = GetBlocks<IMyPowerProducer>(group);

    float maxTot=0, curTot=0;
    float maxBat=0, curBat=0, batIn=0;
    float maxSol=0, curSol=0;
    float maxRea=0, curRea=0;
    float maxOth=0, curOth=0;

    foreach (var b in producers)
    {
        if (!b.IsWorking) continue;
        float mo = b.MaxOutput, co = b.CurrentOutput;
        maxTot += mo; curTot += co;

        var bat = b as IMyBatteryBlock;
        if (bat != null)
        {
            maxBat += bat.MaxStoredPower;
            curBat += bat.CurrentStoredPower;
            batIn  += bat.CurrentInput;
        }
        else if (b is IMySolarPanel) { maxSol += mo; curSol += co; }
        else if (b is IMyReactor)   { maxRea += mo; curRea += co; }
        else                        { maxOth += mo; curOth += co; }
    }

    float r      = maxTot > 0 ? curTot / maxTot : 0f;
    string rCol  = r > 0.90f ? CR : (r > 0.70f ? CY : CG);
    AppBar($"  Carico: {FP(curTot)}/{FP(maxTot)}", r, cfg, rCol);

    if (maxRea > 0) AL($"  Reattori  {FP(curRea)}/{FP(maxRea)}");
    if (maxSol > 0) AL($"  Solari    {FP(curSol)}/{FP(maxSol)}");
    if (maxOth > 0) AL($"  H2/Vento  {FP(curOth)}/{FP(maxOth)}");

    if (maxBat > 0)
    {
        float br   = maxBat > 0 ? curBat / maxBat : 0f;
        string bCol= br < CRIT ? CR : (br < WARN ? CY : CG);
        string dir = batIn > curTot ? "[^] Carica" : "[v] Uso";
        ALC($"  Batterie  {br*100:F1}% {dir} {FE(curBat)}/{FE(maxBat)}", bCol, cfg.Colors);
    }
}

void CmdBattery(PanelConfig cfg, string group)
{
    ALC("BATTERIE", CC, cfg.Colors);

    var bats = GetBlocks<IMyBatteryBlock>(group);
    if (bats.Count == 0) { AL("  Nessuna batteria trovata"); return; }

    float maxS=0, curS=0, totIn=0, totOut=0;
    foreach (var b in bats)
    {
        maxS   += b.MaxStoredPower;   curS   += b.CurrentStoredPower;
        totIn  += b.CurrentInput;     totOut += b.CurrentOutput;
    }

    float r    = maxS > 0 ? curS / maxS : 0f;
    string rc  = r < CRIT ? CR : (r < WARN ? CY : CG);
    AppBar($"  Carica: {r*100:F1}% [{FE(curS)}/{FE(maxS)}]", r, cfg, rc);
    AL($"  Entrata: {FP(totIn)}  |  Uscita: {FP(totOut)}");

    string stato = totIn > totOut ? "[^] In carica" : (totOut > totIn ? "[v] In scarica" : "[=] Stabile");
    ALC($"  Stato: {stato}", rc, cfg.Colors);
    if (bats.Count > 1) ALC($"  Blocchi: {bats.Count}", CD, cfg.Colors);
}

void CmdSolar(PanelConfig cfg, string group)
{
    ALC("PANNELLI SOLARI", CC, cfg.Colors);
    var sp = GetBlocks<IMySolarPanel>(group);
    if (sp.Count == 0) { AL("  Nessun pannello solare trovato"); return; }

    float mx=0, cx=0;
    foreach (var p in sp) { mx += p.MaxOutput; cx += p.CurrentOutput; }
    float r = mx > 0 ? cx / mx : 0f;
    AppBar($"  Output: {FP(cx)}/{FP(mx)}", r, cfg);
    AL($"  Attivi: {sp.Count(p => p.IsWorking)}/{sp.Count}");
}

// ════════════════════════════ INVENTARIO ═════════════════════════════

void CmdCargo(PanelConfig cfg, string group)
{
    ALC("CARGO", CC, cfg.Colors);
    var blocks = GetInventoryBlocks(group);
    if (blocks.Count == 0) { AL("  Nessun container trovato"); return; }

    double maxVol=0, curVol=0, curMass=0;
    foreach (var b in blocks)
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv  = b.GetInventory(i);
            maxVol  += (double)inv.MaxVolume;
            curVol  += (double)inv.CurrentVolume;
            curMass += (double)inv.CurrentMass;
        }

    float r   = maxVol > 0 ? (float)(curVol / maxVol) : 0f;
    string rc = r > HIGH ? CR : (r > 0.75f ? CY : CG);
    AppBar($"  Volume: {FV(curVol)}/{FV(maxVol)}", r, cfg, rc);
    AL($"  Massa:   {FM(curMass)}");
    ALC($"  Blocchi: {blocks.Count}", CD, cfg.Colors);
}

void CmdInventory(string typeId, PanelConfig cfg, string group)
{
    string title = typeId == "Ore"             ? "MINERALI"   :
                   typeId == "Ingot"           ? "LINGOTTI"   :
                   typeId == "AmmoMagazine"    ? "MUNIZIONI"  :
                   typeId == "ConsumableItem"  ? "CIBO"       : "COMPONENTI";
    ALC(title, CC, cfg.Colors);

    var blocks = GetInventoryBlocks(group);
    var totals = new Dictionary<string, double>();

    foreach (var b in blocks)
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var item in items)
            {
                if (!item.Type.TypeId.EndsWith(typeId)) continue;
                string name = FriendlyItem(item.Type.SubtypeId);
                double amt  = (double)item.Amount;
                if (totals.ContainsKey(name)) totals[name] += amt;
                else totals[name] = amt;
            }
        }

    if (totals.Count == 0) { ALC($"  Nessun {typeId.ToLower()} trovato", CD, cfg.Colors); return; }

    foreach (var kv in totals.OrderByDescending(x => x.Value).Take(15))
    {
        string name = kv.Key.Length > 20 ? kv.Key.Substring(0, 19) + "." : kv.Key;
        AL($"  {name,-20} {FAI(kv.Value),7}");
    }
}

void CmdTools(PanelConfig cfg, string group)
{
    AL("STRUMENTI");
    var blocks = GetInventoryBlocks(group);
    var totals = new Dictionary<string, double>();

    // I tool hanno TypeId PhysicalGunObject e subtype che contiene
    // Grinder, Drill, Welder (strumenti manuali)
    var toolKeywords = new List<string> { "Grinder","Drill","Welder","Cutter","Scanner","Clang" };

    foreach (var b in blocks)
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var item in items)
            {
                if (!item.Type.TypeId.EndsWith("PhysicalGunObject")) continue;
                bool isTool = false;
                foreach (var kw in toolKeywords)
                    if (item.Type.SubtypeId.Contains(kw)) { isTool = true; break; }
                if (!isTool) continue;

                string name = FriendlyItem(item.Type.SubtypeId);
                double amt  = (double)item.Amount;
                if (totals.ContainsKey(name)) totals[name] += amt;
                else totals[name] = amt;
            }
        }

    if (totals.Count == 0) { AL("  Nessuno strumento trovato"); return; }
    foreach (var kv in totals.OrderByDescending(x => x.Value).Take(15))
    {
        string name = kv.Key.Length > 20 ? kv.Key.Substring(0, 19) + "." : kv.Key;
        AL($"  {name,-20} {FAI(kv.Value),7}");
    }
}

void CmdWeapons(PanelConfig cfg, string group)
{
    AL("ARMI");
    var blocks = GetInventoryBlocks(group);
    var totals = new Dictionary<string, double>();

    // Le armi hanno TypeId PhysicalGunObject ma NON sono tool
    var toolKeywords = new List<string> { "Grinder","Drill","Welder","Cutter","Scanner" };

    foreach (var b in blocks)
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var item in items)
            {
                if (!item.Type.TypeId.EndsWith("PhysicalGunObject")) continue;
                bool isTool = false;
                foreach (var kw in toolKeywords)
                    if (item.Type.SubtypeId.Contains(kw)) { isTool = true; break; }
                if (isTool) continue;

                string name = FriendlyItem(item.Type.SubtypeId);
                double amt  = (double)item.Amount;
                if (totals.ContainsKey(name)) totals[name] += amt;
                else totals[name] = amt;
            }
        }

    if (totals.Count == 0) { AL("  Nessuna arma trovata"); return; }
    foreach (var kv in totals.OrderByDescending(x => x.Value).Take(15))
    {
        string name = kv.Key.Length > 20 ? kv.Key.Substring(0, 19) + "." : kv.Key;
        AL($"  {name,-20} {FAI(kv.Value),7}");
    }
}

void CmdBottles(PanelConfig cfg, string group)
{
    AL("BOMBOLE");
    var blocks = GetInventoryBlocks(group);
    double h2Tot=0, o2Tot=0;
    int h2Count=0, o2Count=0;

    foreach (var b in blocks)
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var item in items)
            {
                if (item.Type.TypeId.EndsWith("GasContainerObject"))
                {
                    h2Tot += (double)item.Amount; h2Count++;
                }
                else if (item.Type.TypeId.EndsWith("OxygenContainerObject"))
                {
                    o2Tot += (double)item.Amount; o2Count++;
                }
            }
        }

    if (h2Count == 0 && o2Count == 0) { AL("  Nessuna bombola trovata"); return; }
    if (h2Count > 0) AL($"  Idrogeno (H2)     {h2Tot:F0} bombole");
    if (o2Count > 0) AL($"  Ossigeno  (O2)    {o2Tot:F0} bombole");
}

void CmdPrototech(PanelConfig cfg, string group)
{
    AL("PROTOTECH");
    var blocks = GetInventoryBlocks(group);
    var totals = new Dictionary<string, double>();

    // I Prototech sono Component con subtype che inizia per "Prototech"
    // oppure item il cui nome contiene "Prototech"
    foreach (var b in blocks)
        for (int i = 0; i < b.InventoryCount; i++)
        {
            var inv = b.GetInventory(i);
            var items = new List<MyInventoryItem>();
            inv.GetItems(items);
            foreach (var item in items)
            {
                string sub = item.Type.SubtypeId;
                if (!sub.StartsWith("Prototech") && !sub.Contains("Prototech")) continue;
                string name = FriendlyItem(sub);
                double amt  = (double)item.Amount;
                if (totals.ContainsKey(name)) totals[name] += amt;
                else totals[name] = amt;
            }
        }

    if (totals.Count == 0) { AL("  Nessun item Prototech trovato"); return; }
    foreach (var kv in totals.OrderByDescending(x => x.Value).Take(15))
    {
        string name = kv.Key.Length > 20 ? kv.Key.Substring(0, 19) + "." : kv.Key;
        AL($"  {name,-20} {FAI(kv.Value),7}");
    }
}

// ════════════════════════════ GAS ════════════════════════════════════

void CmdGas(string gasType, PanelConfig cfg, string group)
{
    string title = gasType == "Hydrogen" ? "IDROGENO (H2)" : "OSSIGENO (O2)";
    ALC(title, CC, cfg.Colors);

    var tanks = new List<IMyGasTank>();
    if (string.IsNullOrEmpty(group))
    {
        if (gasType == "Hydrogen")
        {
            // Tank idrogeno: SubtypeId contiene "Hydrogen"
            GridTerminalSystem.GetBlocksOfType(tanks, t =>
                t.IsSameConstructAs(Me) &&
                t.BlockDefinition.SubtypeId.Contains("Hydrogen"));
        }
        else
        {
            // Tank ossigeno: SubtypeId è vuoto ("") su large grid,
            // "OxygenTankSmall" su small grid — NON contiene "Hydrogen"
            GridTerminalSystem.GetBlocksOfType(tanks, t =>
                t.IsSameConstructAs(Me) &&
                !t.BlockDefinition.SubtypeId.Contains("Hydrogen"));
        }
    }
    else
        GridTerminalSystem.GetBlockGroupWithName(group)?.GetBlocksOfType(tanks);

    if (tanks.Count == 0) { AL($"  Nessun serbatoio trovato"); return; }

    double total = 0;
    foreach (var t in tanks) total += t.FilledRatio;
    float avg = (float)(total / tanks.Count);

    string rc = avg < CRIT ? CR : (avg < WARN ? CY : CG);
    AppBar($"  Media: {avg*100:F1}%  [{tanks.Count} serbatoi]", avg, cfg, rc);

    // Mostra serbatoi singoli se non troppi
    if (tanks.Count > 1 && tanks.Count <= 8)
        foreach (var t in tanks)
        {
            float r2   = (float)t.FilledRatio;
            string rc2 = r2 < CRIT ? CR : (r2 < WARN ? CY : CG);
            ALC($"  {SN(t.CustomName),-16} {r2*100:F1}%", rc2, cfg.Colors);
        }
}

// ════════════════════════════ STRUTTURA ══════════════════════════════

void CmdDamage(PanelConfig cfg, string group)
{
    ALC("BLOCCHI DANNEGGIATI", CC, cfg.Colors);

    var all = new List<IMyTerminalBlock>();
    if (string.IsNullOrEmpty(group))
        GridTerminalSystem.GetBlocksOfType(all, b => b.IsSameConstructAs(Me));
    else
        GridTerminalSystem.GetBlockGroupWithName(group)?.GetBlocks(all);

    var damaged = all.Where(b => !b.IsFunctional).ToList();

    if (damaged.Count == 0)
    {
        ALC("  [OK] Nessun danno rilevato", CG, cfg.Colors);
        return;
    }

    ALC($"  [!] {damaged.Count} blocco/i compromesso/i:", CY, cfg.Colors);
    foreach (var b in damaged.Take(12))
        ALC($"  [X] {SN(b.CustomName)}", CR, cfg.Colors);
    if (damaged.Count > 12)
        ALC($"  ... e altri {damaged.Count - 12}", CD, cfg.Colors);
}

void CmdIntegrity(PanelConfig cfg)
{
    ALC("INTEGRITA' STRUTTURALE", CC, cfg.Colors);

    var all = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType(all, b => b.IsSameConstructAs(Me));

    int total  = all.Count;
    int func   = all.Count(b => b.IsFunctional);
    int work   = all.Count(b => b.IsWorking);

    float r   = total > 0 ? (float)func / total : 1f;
    string rc = r < 0.50f ? CR : (r < 0.80f ? CY : CG);
    AppBar($"  Blocchi: {func}/{total}", r, cfg, rc);
    AL($"  Funzionanti:  {work}");
    AL($"  Danneggiati:  {total - func}");
}

// ════════════════════════════ SISTEMI SPECIALI ════════════════════════

void CmdJumpDrive(PanelConfig cfg, string group)
{
    ALC("MOTORI FTL (JUMP DRIVE)", CC, cfg.Colors);
    var drives = GetBlocks<IMyJumpDrive>(group);
    if (drives.Count == 0) { AL("  Nessun motore FTL"); return; }

    float maxS=0, curS=0;
    foreach (var d in drives) { maxS += d.MaxStoredPower; curS += d.CurrentStoredPower; }

    float r   = maxS > 0 ? curS / maxS : 0f;
    string rc = r < 0.50f ? CY : (r >= 0.99f ? CG : CW);
    AppBar($"  Carica: {r*100:F1}% [{FE(curS)}/{FE(maxS)}]", r, cfg, rc);

    foreach (var d in drives)
    {
        string st  = d.Status.ToString();
        bool ready = st.Contains("Ready") || st.Contains("Ok");
        bool charg = st.Contains("Charging");
        string sc  = ready ? CG : (charg ? CY : CR);
        string stI = ready ? "[PRONTO]" : (charg ? "[Carica]" : "[N/D]");
        ALC($"  {SN(d.CustomName),-16} {stI}", sc, cfg.Colors);
    }
}

void CmdConnector(PanelConfig cfg, string group)
{
    ALC("CONNETTORI", CC, cfg.Colors);
    var conns = GetBlocks<IMyShipConnector>(group);
    if (conns.Count == 0) { AL("  Nessun connettore trovato"); return; }

    foreach (var c in conns)
    {
        string st, sc;
        switch (c.Status)
        {
            case MyShipConnectorStatus.Connected:
                st = "[CON] CONNESSO";  sc = CG; break;
            case MyShipConnectorStatus.Connectable:
                st = "[ ~ ] In range";  sc = CY; break;
            default:
                st = "[   ] Libero";    sc = CD; break;
        }
        ALC($"  {SN(c.CustomName),-16} {st}", sc, cfg.Colors);
    }
}

void CmdLandingGear(PanelConfig cfg, string group)
{
    ALC("ZAMPE DI ATTERRAGGIO", CC, cfg.Colors);
    var gears = GetBlocks<IMyLandingGear>(group);
    if (gears.Count == 0) { AL("  Nessuna zampa trovata"); return; }

    int locked = 0;
    foreach (var g in gears)
        if (g.IsLocked) locked++;

    string st = locked == gears.Count ? "TUTTE AGGANCIATE"    :
                locked > 0            ? $"{locked}/{gears.Count} agganciate" :
                                        "LIBERE";
    string sc = locked == gears.Count ? CG : (locked > 0 ? CY : CW);
    ALC($"  Stato: {st}", sc, cfg.Colors);

    // Mostra singolarmente se poche
    if (gears.Count <= 8)
        foreach (var g in gears)
        {
            string gSt = g.IsLocked ? "[L]" : "[ ]";
            string gSc = g.IsLocked ? CG : CD;
            ALC($"  {gSt} {SN(g.CustomName)}", gSc, cfg.Colors);
        }
}

void CmdAntenna(PanelConfig cfg, string group)
{
    ALC("COMUNICAZIONI", CC, cfg.Colors);
    var ants  = GetBlocks<IMyRadioAntenna>(group);
    var beacs = GetBlocks<IMyBeacon>(group);

    if (ants.Count == 0 && beacs.Count == 0)
    { AL("  Nessuna antenna o beacon trovata"); return; }

    foreach (var a in ants)
    {
        string sc = a.IsWorking ? CG : CR;
        string st = a.IsWorking ? "[ON] " : "[OFF]";
        ALC($"  ANT {st} {SN(a.CustomName),-12} {a.Radius/1000:F0}km", sc, cfg.Colors);
    }
    foreach (var b in beacs)
    {
        string sc = b.IsWorking ? CG : CR;
        string st = b.IsWorking ? "[ON] " : "[OFF]";
        ALC($"  BCN {st} {SN(b.CustomName),-12} {b.Radius/1000:F0}km", sc, cfg.Colors);
    }
}

void CmdProduction(PanelConfig cfg, string group)
{
    ALC("PRODUZIONE", CC, cfg.Colors);
    var prods = GetBlocks<IMyProductionBlock>(group);
    if (prods.Count == 0) { AL("  Nessun blocco di produzione trovato"); return; }

    int refs=0, asms=0, active=0;
    foreach (var p in prods)
    {
        if (p is IMyRefinery) refs++; else asms++;
        if (p.IsProducing) active++;
    }

    string ac = active > 0 ? CG : CD;
    ALC($"  Attivi: {active}/{prods.Count}", ac, cfg.Colors);
    if (refs > 0) AL($"  Raffinerie:   {refs}");
    if (asms > 0) AL($"  Assemblatori: {asms}");

    // Mostra le prime lavorazioni in coda
    var queue = new List<MyProductionItem>();
    foreach (var p in prods.Where(x => x.IsProducing).Take(5))
    {
        queue.Clear();
        p.GetQueue(queue);
        if (queue.Count > 0)
        {
            string name = FriendlyItem(queue[0].BlueprintId.SubtypeName);
            ALC($"  [>] {name,-16} x{queue[0].Amount:F0}", CO, cfg.Colors);
        }
    }
}

// ════════════════════════════ NAVIGAZIONE ════════════════════════════

void CmdSpeed(PanelConfig cfg)
{
    ALC("VELOCITA'", CC, cfg.Colors);
    var ctrl = GetController();
    if (ctrl == null) { AL("  Nessun controller trovato"); return; }

    double spd = ctrl.GetShipSpeed();
    string sc  = spd > 90 ? CY : CG;
    ALC($"  {spd:F1} m/s  ({spd*3.6:F1} km/h)", sc, cfg.Colors);

    var v = ctrl.GetShipVelocities().LinearVelocity;
    ALC($"  X:{v.X:+0.0;-0.0}  Y:{v.Y:+0.0;-0.0}  Z:{v.Z:+0.0;-0.0}", CD, cfg.Colors);
}

void CmdMass(PanelConfig cfg)
{
    ALC("MASSA", CC, cfg.Colors);
    var ctrl = GetController();
    if (ctrl == null) { AL("  Nessun controller trovato"); return; }

    var m = ctrl.CalculateShipMass();
    AL($"  Struttura:  {FM(m.BaseMass)}");
    AL($"  Totale:     {FM(m.TotalMass)}");
    float cargo = m.TotalMass - m.BaseMass;
    if (cargo > 0) AL($"  Cargo:      {FM(cargo)}");
}

void CmdPosition(PanelConfig cfg)
{
    ALC("POSIZIONE GPS", CC, cfg.Colors);
    var ctrl = GetController();
    if (ctrl == null) { AL("  Nessun controller trovato"); return; }

    var p = ctrl.GetPosition();
    AL($"  X: {p.X:+0.00;-0.00}");
    AL($"  Y: {p.Y:+0.00;-0.00}");
    AL($"  Z: {p.Z:+0.00;-0.00}");
    ALC($"  GPS:POSIZIONE:{p.X:F2}:{p.Y:F2}:{p.Z:F2}:#FFAA00:", CO, cfg.Colors);
}

// ════════════════════════════ METODI DI SUPPORTO ══════════════════════

IMyShipController GetController()
{
    var list = new List<IMyShipController>();
    // Preferisci il controller in uso dal giocatore
    GridTerminalSystem.GetBlocksOfType(list, c => c.IsSameConstructAs(Me) && c.IsUnderControl);
    if (list.Count > 0) return list[0];
    // Altrimenti prendi il primo disponibile
    GridTerminalSystem.GetBlocksOfType(list, c => c.IsSameConstructAs(Me));
    return list.Count > 0 ? list[0] : null;
}

List<T> GetBlocks<T>(string group) where T : class, IMyTerminalBlock
{
    var list = new List<T>();
    if (string.IsNullOrEmpty(group))
        GridTerminalSystem.GetBlocksOfType(list, b => b.IsSameConstructAs(Me));
    else
        GridTerminalSystem.GetBlockGroupWithName(group)?.GetBlocksOfType(list);
    return list;
}

List<IMyTerminalBlock> GetInventoryBlocks(string group)
{
    var all = new List<IMyTerminalBlock>();
    if (string.IsNullOrEmpty(group))
        GridTerminalSystem.GetBlocksOfType(all, b => b.IsSameConstructAs(Me) && b.HasInventory);
    else
        GridTerminalSystem.GetBlockGroupWithName(group)?.GetBlocks(all, b => b.HasInventory);
    return all;
}

// ── Metodi di output ──────────────────────────────────────────────────

// Append Line semplice
void AL(string text) => _sb.AppendLine(text);

// Append Line Colorata → in SE vanilla, uguale ad AL (niente tag colore)
void ALC(string text, string rgb, bool colors) => _sb.AppendLine(text);

// Testo centrato
void AppCenter(string text, PanelConfig cfg)
{
    int pad = Math.Max(0, (cfg.Width - text.Length) / 2);
    if (cfg.Colors)
        _sb.AppendLine(new string(' ', pad) + text);
    else
        _sb.AppendLine(new string(' ', pad) + text);
}

// Testo allineato a destra
void AppRight(string text, PanelConfig cfg)
{
    int pad = Math.Max(0, cfg.Width - text.Length);
    _sb.AppendLine(new string(' ', pad) + text);
}

// Barra di progresso — testo puro, compatibile vanilla SE
void AppBar(string label, float ratio, PanelConfig cfg, string color = null)
{
    ratio = Math.Max(0f, Math.Min(1f, ratio));
    int f = (int)(BAR_W * ratio);
    int e = BAR_W - f;

    string bar;
    switch (cfg.BarStyle)
    {
        case 1:  bar = $"[{new string('#', f)}{new string('-', e)}]"; break;
        case 3:  bar = $"[{new string('>', f)}{new string('.', e)}]"; break;
        default: bar = $"[{new string('\u2588', f)}{new string('\u2591', e)}]"; break;
    }
    _sb.AppendLine(bar);
    if (!string.IsNullOrEmpty(label))
        _sb.AppendLine($"  {label}");
}

// ── Formattatori ──────────────────────────────────────────────────────

string FP(float mw)      // Potenza: kW / MW / GW
{
    if (mw >= 1000f) return $"{mw/1000f:F2} GW";
    if (mw >= 1f)    return $"{mw:F2} MW";
    return $"{mw*1000f:F0} kW";
}

string FE(float mwh)     // Energia: kWh / MWh / GWh
{
    if (mwh >= 1000f) return $"{mwh/1000f:F2} GWh";
    if (mwh >= 1f)    return $"{mwh:F2} MWh";
    return $"{mwh*1000f:F0} kWh";
}

string FV(double kl)     // Volume: kL / ML
{
    if (kl >= 1000) return $"{kl/1000:F2} ML";
    return $"{kl:F2} kL";
}

string FM(double kg)     // Massa: kg / t / kt
{
    if (kg >= 1000000) return $"{kg/1000000:F2} kt";
    if (kg >= 1000)    return $"{kg/1000:F2} t";
    return $"{kg:F0} kg";
}

string FA(double amt)    // Quantità decimale: raw / k / M
{
    if (amt >= 1000000) return $"{amt/1000000:F2}M";
    if (amt >= 1000)    return $"{amt/1000:F1}k";
    return $"{amt:F1}";
}

string FAI(double amt)   // Quantità intera: raw / k / M (senza decimali inutili)
{
    if (amt >= 1000000) return $"{amt/1000000:F1}M";
    if (amt >= 1000)    return $"{amt/1000:F1}k";
    return $"{(long)amt}";
}

string SN(string name)   // Nome corto: rimuove tag e tronca
{
    name = name.Replace(LCD_TAG, "").Trim();
    return name.Length > 16 ? name.Substring(0, 14) + ".." : name;
}

// Traduzione subtype ID → nome italiano leggibile
string FriendlyItem(string sub)
{
    var map = new Dictionary<string, string>
    {
        // Minerali
        {"Iron","Ferro"},       {"Nickel","Nichel"},       {"Cobalt","Cobalto"},
        {"Magnesium","Magnesio"},{"Silicon","Silicio"},    {"Silver","Argento"},
        {"Gold","Oro"},         {"Platinum","Platino"},    {"Uranium","Uranio"},
        {"Stone","Pietra"},     {"Scrap","Rottame"},        {"Ice","Ghiaccio"},
        // Componenti comuni
        {"SteelPlate","Piastra Acciaio"},
        {"Construction","Comp. Costruzione"},
        {"SmallTube","Tubo Piccolo"},
        {"LargeTube","Tubo Grande"},
        {"Motor","Motore"},
        {"Computer","Computer"},
        {"Display","Display"},
        {"BulletproofGlass","Vetro Blindato"},
        {"Reactor","Comp. Reattore"},
        {"Thrust","Comp. Motore"},
        {"GravityGenerator","Comp. Gravita'"},
        {"Medical","Kit Medico"},
        {"RadioCommunication","Radio"},
        {"Detector","Rilevatore"},
        {"Explosives","Esplosivi"},
        {"SolarCell","Cella Solare"},
        {"PowerCell","Cella Energia"},
        {"Superconductor","Superconduttore"},
        {"MetalGrid","Griglia Metallica"},
        {"InteriorPlate","Piastra Interna"},
        {"Canvas","Tela"},
        {"ZoneChip","Chip Zona"},
        // Strumenti
        {"AngleGrinderItem","Smerigliatrice Base"},
        {"AngleGrinder2Item","Smerigliatrice Enh."},
        {"AngleGrinder3Item","Smerigliatrice Prof."},
        {"AngleGrinder4Item","Smerigliatrice Elite"},
        {"HandDrillItem","Trapano Base"},
        {"HandDrill2Item","Trapano Enh."},
        {"HandDrill3Item","Trapano Prof."},
        {"HandDrill4Item","Trapano Elite"},
        {"WelderItem","Saldatore Base"},
        {"Welder2Item","Saldatore Enh."},
        {"Welder3Item","Saldatore Prof."},
        {"Welder4Item","Saldatore Elite"},
        // Armi
        {"AutomaticRifleItem","Fucile Base"},
        {"RapidFireAutomaticRifleItem","Fucile Rapido"},
        {"PreciseAutomaticRifleItem","Fucile Preciso"},
        {"UltimateAutomaticRifleItem","Fucile Elite"},
        {"SemiAutoPistolItem","Pistola Semi-Auto"},
        {"FullAutoPistolItem","Pistola Full-Auto"},
        {"ElitePistolItem","Pistola Elite"},
        {"BasicHandHeldLauncherItem","Lanciarazzi Base"},
        {"AdvancedHandHeldLauncherItem","Lanciarazzi Avanzato"},
        // Munizioni
        {"NATO_5p56x45mm","Caricatore 5.56mm"},
        {"NATO_25x184mm","Caricatore 25x184"},
        {"Missile200mm","Missile 200mm"},
        {"AutomaticRifleGun_Mag_20rd","Caricatore Fucile 20"},
        {"RapidFireAutomaticRifleGun_Mag_50rd","Caricatore RF 50"},
        {"PreciseAutomaticRifleGun_Mag_5rd","Caricatore Prec. 5"},
        {"UltimateAutomaticRifleGun_Mag_30rd","Caricatore Elite 30"},
        {"SemiAutoPistolMagazine","Caric. Pistola Semi"},
        {"FullAutoPistolMagazine","Caric. Pistola Full"},
        {"ElitePistolMagazine","Caric. Pistola Elite"},
        // Cibo (Survival update)
        {"Medkit","Kit Medico"},
        {"Powerkit","Kit Energia"},
        {"ClangCola","Cola Clang"},
        {"CosmicCoffee","Caffe' Cosmico"},
        {"SpaceJuice","Space Juice"},
        {"Protein","Barretta Proteica"},
        // Prototech
        {"PrototechFrame","PT: Telaio"},
        {"PrototechCapacitor","PT: Condensatore"},
        {"PrototechPropulsionEngine","PT: Motore Prop."},
        {"PrototechPowerCell","PT: Cella Energia"},
        {"PrototechCoolingUnit","PT: Raffreddamento"},
        {"PrototechHighPressureTank","PT: Serbatoio HP"},
        {"PrototechMachinery","PT: Macchinario"},
        {"PrototechSupercharger","PT: Supercharger"},
        {"PrototechArmor","PT: Armatura"},
    };

    if (map.ContainsKey(sub)) return map[sub];
    // Prova match parziale
    foreach (var kv in map)
        if (sub.Contains(kv.Key)) return kv.Value;

    // Converti PascalCase in parole separate
    var sb2 = new StringBuilder();
    for (int i = 0; i < sub.Length; i++)
    {
        if (i > 0 && char.IsUpper(sub[i]) && char.IsLower(sub[i-1]))
            sb2.Append(' ');
        sb2.Append(sub[i]);
    }
    return sb2.ToString().Trim();
}