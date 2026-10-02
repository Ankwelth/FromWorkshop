// Airlock Controller v1.18 NEXUS LCD
// Tags: [airlock ID], optional door role [inner]/[outer], optional screen tag [lcd] or [lcd:N]
// Commands: rescan | ID | ID inner | ID outer

const string VERSION = "1.18 NEXUS LCD";
const double HIGH = 0.95, LOW = 0.01, STALL_TIME = 2.0, MIN_DROP_RATE = 0.01;

List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
List<IMyGasTank> oxygenTanks = new List<IMyGasTank>();
List<Airlock> locks = new List<Airlock>();
Dictionary<string, Airlock> byId = new Dictionary<string, Airlock>(StringComparer.OrdinalIgnoreCase);
StringBuilder sb = new StringBuilder();

public Program(){ Runtime.UpdateFrequency = UpdateFrequency.Update100; Rescan(); }

public void Main(string argument, UpdateType source){
    string arg = (argument ?? "").Trim(); bool commanded = arg.Length > 0;
    if (commanded) Command(arg);

    double dt = Runtime.TimeSinceLastRun.TotalSeconds;
    if (commanded) dt = 0; else if (dt <= 0 || dt > 5) dt = 1.0 / 6.0;

    bool busy = false; double shipO2 = ShipOxygen();
    for (int i = 0; i < locks.Count; i++){ locks[i].Update(dt, shipO2); busy |= locks[i].Busy; }
    Runtime.UpdateFrequency = busy ? UpdateFrequency.Update10 : UpdateFrequency.Update100;
    EchoStatus();
}

void Command(string arg){
    if (Eq(arg, "rescan") || Eq(arg, "scan")){ Rescan(); return; }
    int side = 0; string id = arg; string low = arg.ToLowerInvariant();
    if (low.StartsWith("outer ")){ side = 2; id = arg.Substring(6).Trim(); }
    else if (low.StartsWith("inner ")){ side = 1; id = arg.Substring(6).Trim(); }
    else if (low.EndsWith(" outer")){ side = 2; id = arg.Substring(0, arg.Length - 6).Trim(); }
    else if (low.EndsWith(" inner")){ side = 1; id = arg.Substring(0, arg.Length - 6).Trim(); }
    Airlock a; if (byId.TryGetValue(id, out a)) a.Request(side);
}

void Rescan(){
    locks.Clear(); byId.Clear(); blocks.Clear(); oxygenTanks.Clear(); GridTerminalSystem.GetBlocks(blocks);
    for (int i = 0; i < blocks.Count; i++){
        IMyTerminalBlock b = blocks[i]; string id;
        if (b == null || b.EntityId == Me.EntityId || b.CubeGrid.EntityId != Me.CubeGrid.EntityId) continue;
        IMyGasTank tank = b as IMyGasTank; if (tank != null && IsOxygenTank(tank)) oxygenTanks.Add(tank);
        if (!Tag(Text(b), "airlock", out id)) continue;
        Airlock a; if (!byId.TryGetValue(id, out a)){ a = new Airlock(id); byId[id] = a; locks.Add(a); }
        a.Add(b);
    }
    for (int i = 0; i < locks.Count; i++) locks[i].FinishScan();
}

void EchoStatus(){
    sb.Clear(); sb.AppendLine("Airlock Controller v" + VERSION); sb.AppendLine("Airlocks: " + locks.Count);
    for (int i = 0; i < locks.Count; i++) sb.AppendLine(locks[i].Line());
    Echo(sb.ToString());
}

static bool Eq(string a, string b){ return a.Equals(b, StringComparison.OrdinalIgnoreCase); }
bool IsOxygenTank(IMyGasTank t){ return t.BlockDefinition.SubtypeName.IndexOf("oxygen", StringComparison.OrdinalIgnoreCase) >= 0 || t.CustomName.IndexOf("[O2]", StringComparison.OrdinalIgnoreCase) >= 0; }
double ShipOxygen(){ if (oxygenTanks.Count == 0) return -1; double sum = 0; for (int i = 0; i < oxygenTanks.Count; i++) sum += oxygenTanks[i].FilledRatio; return sum / oxygenTanks.Count; }
static string Text(IMyTerminalBlock b){ return (b.CustomName ?? "") + "\n" + (b.CustomData ?? ""); }
static bool Tag(string text, string tag, out string value){
    value = ""; string low = text.ToLowerInvariant(), open = "[" + tag.ToLowerInvariant(); int pos = 0;
    while (true){
        int s = low.IndexOf(open, pos); if (s < 0) return false;
        int e = low.IndexOf("]", s); if (e < 0) return false;
        string raw = text.Substring(s + open.Length, e - s - open.Length).Trim();
        if (raw.StartsWith(":") || raw.StartsWith("=")) raw = raw.Substring(1).Trim();
        if (raw.Length > 0){ value = raw; return true; }
        pos = e + 1;
    }
}
static bool Word(string text, string word){
    char[] sep = new char[]{ ' ', '\t', '\r', '\n', '[', ']', '(', ')', '{', '}', '-', '_', '.', ',', ';', ':', '/', '\\' };
    string[] p = text.ToLowerInvariant().Split(sep, StringSplitOptions.RemoveEmptyEntries); word = word.ToLowerInvariant();
    for (int i = 0; i < p.Length; i++) if (p[i] == word) return true; return false;
}

class Airlock{
    public string Id; public bool Busy;
    List<IMyDoor> inn = new List<IMyDoor>(), outt = new List<IMyDoor>(), one = new List<IMyDoor>();
    List<IMyAirVent> vents = new List<IMyAirVent>(); List<IMyLightingBlock> lights = new List<IMyLightingBlock>();
    List<IMyTextSurface> lcds = new List<IMyTextSurface>();
    bool two, wasOuter, wasOne, scriptOne; int target; double stall, lastP = -1; string state = "Ready";

    public Airlock(string id){ Id = id; }

    public void Add(IMyTerminalBlock b){
        string t = Text(b); IMyDoor d = b as IMyDoor;
        if (d != null){ if (Word(t, "inner") || Word(t, "inside") || Word(t, "internal")) inn.Add(d); else if (Word(t, "outer") || Word(t, "outside") || Word(t, "external") || Word(t, "space")) outt.Add(d); else one.Add(d); return; }
        IMyAirVent v = b as IMyAirVent; if (v != null){ vents.Add(v); return; }
        IMyLightingBlock l = b as IMyLightingBlock; if (l != null) lights.Add(l);
        IMyTextSurface s = b as IMyTextSurface; if (s != null){ lcds.Add(s); return; }
        IMyTextSurfaceProvider p = b as IMyTextSurfaceProvider;
        if (p != null){
            int idx; if (LcdIndex(t, out idx)){ if (idx >= 0 && idx < p.SurfaceCount) lcds.Add(p.GetSurface(idx)); return; }
            if (Word(t, "lcd") || Word(t, "display") || Word(t, "screen")) for (int i = 0; i < p.SurfaceCount; i++) lcds.Add(p.GetSurface(i));
        }
    }

    bool LcdIndex(string t, out int idx){ idx = -1; string raw; if (!Tag(t, "lcd", out raw)) return false; int.TryParse(raw, out idx); return true; }

    public void FinishScan(){ two = inn.Count > 0 && outt.Count > 0; if (!two){ Move(inn, one); Move(outt, one); } }
    void Move(List<IMyDoor> from, List<IMyDoor> to){ for (int i = 0; i < from.Count; i++) to.Add(from[i]); from.Clear(); }

    public void Request(int side){
        target = side; if (target == 0) target = two ? (Open(outt) ? 1 : 2) : (Open(one) ? 1 : 2);
        stall = 0; lastP = P(); scriptOne = false;
        if (two){ Enable(inn, true); Enable(outt, true); Close(inn); Close(outt); }
        else { Enable(one, true); Close(one); }
    }

    public void Update(double dt, double shipO2){ if (two) UpdateTwo(dt); else UpdateOne(dt); Draw(shipO2); Busy = target != 0; }

    void UpdateOne(double dt){
        Enable(one, true);
        if (target == 2){ Close(one); if (!AllClosed(one)){ Depress(false); state = "Sealing"; } else { Depress(true); if (DepressDone(dt)){ OpenDoor(one); target = 0; scriptOne = true; state = "Open"; } else state = "Depressurizing"; } }
        else if (target == 1){ Close(one); if (!AllClosed(one)){ Depress(true); state = "Sealing"; } else { Depress(false); if (P() >= HIGH) target = 0; state = target == 0 ? "Ready" : "Pressurizing"; } }
        else if (Open(one)){ Depress(true); state = (scriptOne || P() <= LOW) ? "Open" : "Manual open, venting"; }
        else { if (wasOne){ target = 1; stall = 0; lastP = P(); } else Depress(false); scriptOne = false; state = P() >= HIGH ? "Ready" : "Pressurizing"; }
        wasOne = Open(one);
    }

    void UpdateTwo(double dt){
        if (target == 2){
            Close(inn); Close(outt); Lock(inn); Lock(outt);
            if (!AllClosed(inn) || !AllClosed(outt)){ Depress(false); state = "Sealing"; }
            else { Depress(true); if (DepressDone(dt)){ OpenDoor(outt); target = 0; state = "Outer open"; } else state = "Depressurizing"; }
        } else if (target == 1){
            Close(inn); Close(outt); Lock(inn); Lock(outt);
            if (!AllClosed(inn) || !AllClosed(outt)){ Depress(true); state = "Sealing"; }
            else { Depress(false); if (P() >= HIGH){ OpenDoor(inn); target = 0; state = "Inner open"; } else state = "Pressurizing"; }
        } else if (Open(outt)){ Depress(true); Lock(inn); Enable(outt, true); state = "Outer open"; }
        else if (Open(inn)){ Depress(false); Lock(outt); Enable(inn, true); state = "Inner open"; }
        else if (wasOuter){ target = 1; stall = 0; lastP = P(); state = "Pressurizing"; }
        else if (P() >= HIGH){ Depress(false); Lock(outt); Enable(inn, true); state = "Ready inside"; }
        else if (P() <= LOW){ Depress(true); Lock(inn); Enable(outt, true); state = "Ready outside"; }
        else state = Depressing() ? "Depressurizing" : "Pressurizing";
        wasOuter = Open(outt);
    }

    bool DepressDone(double dt){
        double p = P(); if (p <= LOW){ stall = 0; lastP = p; return true; }
        double rate = dt > 0 ? (lastP - p) / dt : 0;
        if (lastP < 0 || rate >= MIN_DROP_RATE){ stall = 0; lastP = p; return false; }
        stall += dt; lastP = p; return stall >= STALL_TIME;
    }

    void Draw(double shipO2){
        double p = P(); Color c = StateColor(p);
        for (int i = 0; i < lights.Count; i++){ lights[i].Enabled = true; lights[i].Color = c; }
        for (int i = 0; i < lcds.Count; i++) DrawNexus(lcds[i], shipO2);
    }

    void DrawNexus(IMyTextSurface s, double shipO2){
        Color accent = new Color(160, 80, 0), black = new Color(0, 0, 0), grid = new Color(162, 81, 0), overlay = new Color(255, 255, 255, 31);
        s.ContentType = ContentType.SCRIPT; s.Script = ""; s.BackgroundColor = accent; s.ScriptBackgroundColor = accent;
        RectangleF vp = new RectangleF((s.TextureSize - s.SurfaceSize) * .5f, s.SurfaceSize);
        float u = Math.Max(.25f, Math.Min(vp.Width, vp.Height) / 512f), edge = Math.Max(6f, 14f * u), pad = Math.Max(5f, 12f * u);
        RectangleF panel = new RectangleF(vp.X + edge, vp.Y + edge, vp.Width - edge * 2, vp.Height - edge * 2);
        RectangleF inner = new RectangleF(panel.X + pad, panel.Y + pad, panel.Width - pad * 2, panel.Height - pad * 2);
        float statusPad = inner.Height * .08f;
        RectangleF statusBox = new RectangleF(inner.X + statusPad, inner.Y + inner.Height * .53f, inner.Width - statusPad * 2f, inner.Height * .39f);
        string title = "NEXUS // AIRLOCK " + Id, status = DisplayState();
        using (MySpriteDrawFrame f = s.DrawFrame()){
            Rect(f, vp.Center, new Vector2(vp.Width, vp.Height), accent); Grid(f, vp, grid); RoundRect(f, panel, Math.Max(6f, 16f * u), black);
            float ts = FitText(s, title, inner.Width, inner.Height * .18f, Math.Max(.55f, 1.45f * u));
            Txt(f, title, new Vector2(inner.Center.X, inner.Y + Math.Max(3f, 8f * u)), accent, ts, TextAlignment.CENTER);
            float icon = Math.Max(18f, Math.Min(inner.Height * .17f, statusBox.Width * .12f)), gap = Math.Max(7f, 12f * u), barH = Math.Max(14f, 24f * u), rowY = inner.Y + inner.Height * .36f, rowPad = Math.Max(4f, 8f * u), rowX = statusBox.X + rowPad, rowW = statusBox.Width - rowPad * 2f;
            Texture(f, "IconOxygen", new Vector2(rowX + icon * .5f, rowY), new Vector2(icon, icon), accent);
            OxygenBar(s, f, new RectangleF(rowX + icon + gap, rowY - barH * .5f, rowW - icon - gap, barH), shipO2, accent, black);
            RoundRect(f, statusBox, Math.Max(5f, 12f * u), accent);
            float ss = FitText(s, status, statusBox.Width * .9f, statusBox.Height * .68f, Math.Max(.9f, 4.5f * u));
            Vector2 sm = s.MeasureStringInPixels(new StringBuilder(status), "Debug", ss);
            Txt(f, status, new Vector2(statusBox.Center.X, statusBox.Y + (statusBox.Height - sm.Y) * .5f), black, ss, TextAlignment.CENTER);
            Texture(f, "LCD_Economy_Clear", vp.Center, new Vector2(vp.Width * 1.2f, vp.Height * 1.2f), overlay);
        }
    }

    float FitText(IMyTextSurface s, string t, float maxW, float maxH, float maxS){
        StringBuilder b = new StringBuilder(t); float lo = .1f, hi = maxS;
        for (int i = 0; i < 14; i++){ float m = (lo + hi) * .5f; Vector2 z = s.MeasureStringInPixels(b, "Debug", m); if (z.X <= maxW && z.Y <= maxH) lo = m; else hi = m; }
        return lo;
    }
    string DisplayState(){
        if (state == "Sealing") return target == 2 ? "VENT" : "FILL";
        if (state == "Depressurizing" || state == "Manual open, venting") return "VENT";
        if (state == "Pressurizing") return "FILL";
        if (state == "Outer open") return "OUT";
        if (state == "Inner open") return "IN";
        if (state == "Ready inside" || state == "Ready outside") return "READY";
        return state.ToUpperInvariant();
    }
    Color StateColor(double p){ if (target != 0 || state == "Sealing" || state == "Depressurizing" || state == "Pressurizing" || state == "Manual open, venting") return new Color(255, 60, 45); if (state == "Open" || state == "Outer open" || state == "Ready outside" || p < HIGH) return new Color(255, 150, 35); return new Color(70, 210, 255); }
    void Rect(MySpriteDrawFrame f, Vector2 c, Vector2 s, Color color){ f.Add(new MySprite(SpriteType.TEXTURE, "SquareSimple", c, s, color)); }
    void Texture(MySpriteDrawFrame f, string t, Vector2 p, Vector2 s, Color c){ f.Add(new MySprite(SpriteType.TEXTURE, t, p, s, c)); }
    void Txt(MySpriteDrawFrame f, string t, Vector2 p, Color c, float scale, TextAlignment align){ f.Add(new MySprite(SpriteType.TEXT, t, p, null, c, "Debug", align, scale)); }
    void Grid(MySpriteDrawFrame f, RectangleF r, Color c){
        Vector2 z = new Vector2(r.Width / 2f, r.Height / 2f); float lx = r.X + z.X / 2f, rx = r.X + z.X * 1.5f, ty = r.Y + z.Y / 2f, by = r.Y + z.Y * 1.5f;
        Texture(f, "Grid", new Vector2(lx, ty), z, c); Texture(f, "Grid", new Vector2(rx, ty), z, c); Texture(f, "Grid", new Vector2(lx, by), z, c); Texture(f, "Grid", new Vector2(rx, by), z, c);
    }
    void OxygenBar(IMyTextSurface s, MySpriteDrawFrame f, RectangleF r, double value, Color fill, Color empty){
        Rect(f, r.Center, new Vector2(r.Width, r.Height), fill);
        float inset = 1f, w = Math.Max(0, r.Width - inset * 2), h = Math.Max(0, r.Height - inset * 2), ratio = (float)Math.Max(0, Math.Min(1, value)), rest = w * (1f - ratio);
        if (rest > 0) Rect(f, new Vector2(r.X + inset + w - rest * .5f, r.Center.Y), new Vector2(rest, h), empty);
        string text = value < 0 ? "--" : Percent(value); float scale = FitText(s, text, r.Width * .88f, r.Height * .72f, Math.Max(.45f, r.Height / 22f));
        Vector2 m = s.MeasureStringInPixels(new StringBuilder(text), "Debug", scale), p = new Vector2(r.Center.X, r.Center.Y - m.Y * .5f);
        BarText(f, text, p, empty, scale, new RectangleF(r.X + inset, r.Y + inset, w - rest, h));
        BarText(f, text, p, fill, scale, new RectangleF(r.X + inset + w - rest, r.Y + inset, rest, h));
    }
    void BarText(MySpriteDrawFrame f, string text, Vector2 p, Color c, float scale, RectangleF clip){ if (clip.Width < 1 || clip.Height < 1) return; f.Add(MySprite.CreateClipRect(new Rectangle((int)clip.X, (int)clip.Y, (int)Math.Ceiling(clip.Width), (int)Math.Ceiling(clip.Height)))); Txt(f, text, p, c, scale, TextAlignment.CENTER); f.Add(MySprite.CreateClearClipRect()); }
    void RoundRect(MySpriteDrawFrame f, RectangleF r, float radius, Color c){
        float l = (float)Math.Round(r.X), t = (float)Math.Round(r.Y), rr = (float)Math.Round(r.X + r.Width), b = (float)Math.Round(r.Y + r.Height), w = rr - l, h = b - t, rad = Math.Max(1f, (float)Math.Round(Math.Min(radius, Math.Min(w, h) / 2f)));
        if (rad < 7f){ float cut = Math.Min(2f, rad); Rect(f, new Vector2((l + rr) / 2f, (t + b) / 2f), new Vector2(w - cut * 2f, h), c); Rect(f, new Vector2((l + rr) / 2f, (t + b) / 2f), new Vector2(w, h - cut * 2f), c); return; }
        float d = rad * 2f; Vector2 z = new Vector2(d, d); f.Add(MySprite.CreateClipRect(new Rectangle((int)l, (int)t, (int)w, (int)h)));
        Texture(f, "Circle", new Vector2(l + rad, t + rad), z, c); Texture(f, "Circle", new Vector2(rr - rad, t + rad), z, c); Texture(f, "Circle", new Vector2(l + rad, b - rad), z, c); Texture(f, "Circle", new Vector2(rr - rad, b - rad), z, c);
        Rect(f, new Vector2((l + rr) / 2f, (t + b) / 2f), new Vector2(w - d, h), c); Rect(f, new Vector2((l + rr) / 2f, (t + b) / 2f), new Vector2(w, h - d), c); f.Add(MySprite.CreateClearClipRect());
    }

    public string Line(){ return Id + ": " + state + " | O2 " + Percent(P()); }
    double P(){ if (vents.Count == 0) return 1; double s = 0; for (int i = 0; i < vents.Count; i++) s += vents[i].GetOxygenLevel(); return s / vents.Count; }
    bool Depressing(){ for (int i = 0; i < vents.Count; i++) if (vents[i].Depressurize) return true; return false; }
    void Depress(bool on){ for (int i = 0; i < vents.Count; i++) vents[i].Depressurize = on; }
    void Enable(List<IMyDoor> d, bool on){ for (int i = 0; i < d.Count; i++) d[i].Enabled = on; }
    void OpenDoor(List<IMyDoor> d){ for (int i = 0; i < d.Count; i++){ d[i].Enabled = true; d[i].OpenDoor(); } }
    void Close(List<IMyDoor> d){ for (int i = 0; i < d.Count; i++){ d[i].Enabled = true; d[i].CloseDoor(); } }
    void Lock(List<IMyDoor> d){ for (int i = 0; i < d.Count; i++){ if (d[i].Status == DoorStatus.Closed) d[i].Enabled = false; else { d[i].Enabled = true; d[i].CloseDoor(); } } }
    bool Open(List<IMyDoor> d){ for (int i = 0; i < d.Count; i++) if (d[i].Status == DoorStatus.Open || d[i].Status == DoorStatus.Opening) return true; return false; }
    bool AllClosed(List<IMyDoor> d){ for (int i = 0; i < d.Count; i++) if (d[i].Status != DoorStatus.Closed) return false; return true; }
    string Percent(double v){ int p = (int)Math.Round(v * 100); if (p < 0) p = 0; if (p > 100) p = 100; return p + "%"; }
}
